using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

using Navislamia.Configuration.Options;
using Navislamia.Database;
using Navislamia.Notification;
using Navislamia.Maps;
using Navislamia.Network;
using Navislamia.Scripting;
using Navislamia.Data.Interfaces;
using Navislamia.Data.Loaders;

namespace Navislamia.Game
{
    public class GameModule : IGameModule
    {
        private readonly DbConnectionManager _dbConnectionManager;
        private readonly ScriptContent _scriptContent;
        private readonly INotificationModule _notificationModule;
        private readonly MapContent _mapContent;
        private readonly INetworkModule _networkModule;
        private readonly ScriptOptions _scriptOptions;
        private readonly MapOptions _mapOptions;

        private List<IRepository> _worldRepositories;

        public GameModule() { }

        public GameModule(INotificationModule notificationModule, INetworkModule networkModule, 
            IOptions<ScriptOptions> scriptOptions, IOptions<MapOptions> mapOptions, IOptions<WorldOptions> worldOptions, IOptions<PlayerOptions> playerOptions)
        {
            _scriptOptions = scriptOptions.Value;
            _mapOptions = mapOptions.Value;
            _notificationModule = notificationModule;            
            _networkModule = networkModule;

            _dbConnectionManager = new DbConnectionManager(worldOptions, playerOptions);
            _scriptContent = new ScriptContent(_notificationModule);
            _mapContent = new MapContent(mapOptions, _notificationModule, _scriptContent);
            _worldRepositories = new List<IRepository>();
        }

        public void Start(string ip, int port, int backlog)
        {
            LoadScripts(_scriptOptions.SkipLoading);
            LoadMaps(_mapOptions.SkipLoading);
            LoadDbRepositories();

            _networkModule.Initialize();
            
            while (!_networkModule.IsReady())
            {
                var maxTime = DateTime.UtcNow.AddSeconds(30);

                if (DateTime.UtcNow < maxTime)
                {
                    continue;
                }
                
                _notificationModule.WriteError("Network service timed out!");
                return;
            }
            
            _networkModule.StartListener();
        }

        private void LoadMaps(bool skip)
        {
            if (skip)
            {
                _notificationModule.WriteWarning("Map loading disabled!");
                return;
            }

            if (!_mapContent.Initialize($"{Directory.GetCurrentDirectory()}\\Maps"))
            {
                _notificationModule.WriteError("Failed to start the map service!");
            }

            _notificationModule.WriteSuccess(
                new[]
                {
                    "Map service started successfully!",
                    $"[green]{_mapContent.MapCount.CX + _mapContent.MapCount.CY}[/] files loaded!"
                }, true);
        }

        private void LoadScripts(bool skip)
        {
            if (skip)
            {
                _notificationModule.WriteWarning("Script loading disabled!");
                return;
            }

            _scriptContent.Init();
        }

        private void LoadDbRepositories()
        {
            var loaders = new List<IRepositoryLoader>
            {
                new MonsterLoader(_notificationModule, _dbConnectionManager),
                new PetLoader(_notificationModule, _dbConnectionManager),
                new ItemLoader(_notificationModule, _dbConnectionManager),
                new NPCLoader(_notificationModule, _dbConnectionManager),
                new ETCLoader(_notificationModule, _dbConnectionManager),
                new StringLoader(_notificationModule, _dbConnectionManager)
            };

            foreach (var loader in loaders)
            {
                try
                {
                    loader.Init();
                }
                catch (Exception e)
                {
                    _notificationModule.WriteError($"{loader.GetType().Name} failed to load![/]{e.Message}");
                    throw new Exception($"{loader.GetType().Name} failed to load!");
                }

                _worldRepositories.AddRange(loader.Repositories);
            }

            _notificationModule.WriteNewLine();
        }
    }
}
