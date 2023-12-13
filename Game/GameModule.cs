using System;
using System.IO;
using Microsoft.Extensions.Options;

using Navislamia.Configuration.Options;
using Navislamia.Game.Models.Navislamia;
using Navislamia.Game.Network;
using Navislamia.Game.Repositories;
using Navislamia.Notification;
using Navislamia.Game.Maps;
using Navislamia.Game.Scripting;
using Navislamia.Network;
using Navislamia.Scripting;

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

        private readonly IWorldRepository _worldRepository;
        private readonly WorldEntity _worldEntity;

        public GameModule(INotificationModule notificationModule, INetworkModule networkModule,
            IOptions<ScriptOptions> scriptOptions, IOptions<MapOptions> mapOptions, IWorldRepository worldRepository)
        {
            _scriptOptions = scriptOptions.Value;
            _mapOptions = mapOptions.Value;
            _notificationModule = notificationModule;            
            _networkModule = networkModule;
            _worldRepository = worldRepository;

            _worldEntity = worldRepository.LoadWorldIntoMemory();

            _scriptContent = new ScriptContent(_notificationModule);
            
            _mapContent = new MapContent(mapOptions, _notificationModule, _scriptContent);
        }

        public void Start(string ip, int port, int backlog)
        {
            if (!LoadScripts(_scriptOptions.SkipLoading))
                return;

            if (!LoadMaps(_mapOptions.SkipLoading))
                return; 

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
    }
}
