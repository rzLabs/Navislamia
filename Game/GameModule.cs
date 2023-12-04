using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

using Navislamia.Configuration.Options;
using Navislamia.Notification;
using Navislamia.Maps;
using Navislamia.Network;
using Navislamia.Scripting;
using Navislamia.Data.Loaders;
using Navislamia.Game.Entities.Data.Interfaces;

namespace Navislamia.Game
{
    public class GameModule : IGameModule
    {
        private readonly ScriptContent _scriptContent;
        private readonly INotificationModule _notificationModule;
        private readonly MapContent _mapContent;
        private readonly INetworkModule _networkModule;
        private readonly ScriptOptions _scriptOptions;
        private readonly MapOptions _mapOptions;

        private List<IEfRepository> _worldRepositories;

        public GameModule() { }

        public GameModule(INotificationModule notificationModule, INetworkModule networkModule, 
            IOptions<ScriptOptions> scriptOptions, IOptions<MapOptions> mapOptions)
        {
            _scriptOptions = scriptOptions.Value;
            _mapOptions = mapOptions.Value;
            _notificationModule = notificationModule;            
            _networkModule = networkModule;

            _scriptContent = new ScriptContent(_notificationModule);
            _mapContent = new MapContent(mapOptions, _notificationModule, _scriptContent);
            _worldRepositories = new List<IEfRepository>();
        }

        public void Start(string ip, int port, int backlog)
        {
            if (!LoadScripts(_scriptOptions.SkipLoading))
                return;

            if (!LoadMaps(_mapOptions.SkipLoading))
                return; 

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

        private bool LoadMaps(bool skip)
        {
            if (skip)
            {
                _notificationModule.WriteWarning("Map loading disabled!");
                return true;
            }

            // TODO: MapContent should be printing messages
            return _mapContent.Initialize($"{Directory.GetCurrentDirectory()}\\Maps");
        }

        private bool LoadScripts(bool skip)
        {
            if (skip)
            {
                _notificationModule.WriteWarning("Script loading disabled!");
                return true;
            }

            return _scriptContent.Init();
        }

        private void LoadDbRepositories()
        {
            var loaders = new List<IRepositoryLoader>
            {
                new MonsterLoader(_notificationModule),
                new PetLoader(_notificationModule),
                new ItemLoader(_notificationModule),
                new NPCLoader(_notificationModule),
                new ETCLoader(_notificationModule),
                new StringLoader(_notificationModule)
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
