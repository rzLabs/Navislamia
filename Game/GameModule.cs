using System;
using System.IO;
using Navislamia.Database;
using Navislamia.Notification;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Navislamia.Maps;
using Navislamia.Network;
using Navislamia.Scripting;
using Navislamia.World.Repositories;
using Navislamia.World.Repositories.Loaders;


namespace Navislamia.Game
{
    public class GameModule : IGameModule
    {
        private readonly IDatabaseModule _databaseModule;
        private readonly IScriptingModule _scriptingModule;
        private readonly INotificationModule _notificationModule;
        private readonly IMapModule _mapModule;
        private readonly INetworkModule _networkModule;
        private readonly ScriptOptions _scriptOptions;
        private readonly MapOptions _mapOptions;

        private List<IRepository> _worldRepositories;

        public GameModule() { }

        public GameModule(INotificationModule notificationModule, IDatabaseModule databaseModule,
            IScriptingModule scriptingService, IMapModule mapModule, INetworkModule networkModule, 
            IOptions<ScriptOptions> scriptOptions, IOptions<MapOptions> mapOptions)
        {
            _scriptOptions = scriptOptions.Value;
            _mapOptions = mapOptions.Value;
            _notificationModule = notificationModule;
            _databaseModule = databaseModule;
            _scriptingModule = scriptingService;
            _mapModule = mapModule;
            _networkModule = networkModule;

            _worldRepositories = new List<IRepository>();
        }

        public void Start(string ip, int port, int backlog)
        {
            LoadScripts(_scriptOptions.SkipLoading);
            LoadMapService(_mapOptions.SkipLoading);
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

        private void LoadMapService(bool skip)
        {
            if (skip)
            {
                _notificationModule.WriteWarning("Map loading disabled!");
                return;
            }

            if (!_mapModule.Initialize($"{Directory.GetCurrentDirectory()}\\Maps"))
            {
                _notificationModule.WriteError("Failed to start the map service!");
            }

            _notificationModule.WriteSuccess(
                new[]
                {
                    "Map service started successfully!",
                    $"[green]{_mapModule.MapCount.CX + _mapModule.MapCount.CY}[/] files loaded!"
                }, true);
        }

        private void LoadScripts(bool skip)
        {
            if (skip)
            {
                _notificationModule.WriteWarning("Script loading disabled!");
                return;
            }

            _scriptingModule.Init();
        }

        private void LoadDbRepositories()
        {
            var loaders = new List<IRepositoryLoader>
            {
                new MonsterLoader(_notificationModule, _databaseModule),
                new PetLoader(_notificationModule, _databaseModule),
                new ItemLoader(_notificationModule, _databaseModule),
                new NPCLoader(_notificationModule, _databaseModule),
                new StringLoader(_notificationModule, _databaseModule)
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
