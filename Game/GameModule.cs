using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Extensions.Options;

using Navislamia.Configuration.Options;
using Navislamia.Game.Models.Navislamia;
using Navislamia.Game.Network;
using Navislamia.Game.Repositories;
using Navislamia.Notification;
using Navislamia.Game.Maps;
using Navislamia.Game.Scripting;
using Navislamia.Game.Services;
using Navislamia.Network;
using Navislamia.Scripting;
using System.Threading;

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
        private readonly ICharacterService _characterService;
        private readonly WorldEntity _worldEntity;

        public GameModule(INotificationModule notificationModule, INetworkModule networkModule,
            IOptions<ScriptOptions> scriptOptions, IOptions<MapOptions> mapOptions, IWorldRepository worldRepository, ICharacterService characterService)
        {
            _scriptOptions = scriptOptions.Value;
            _mapOptions = mapOptions.Value;
            _notificationModule = notificationModule;            
            _networkModule = networkModule;
            _worldRepository = worldRepository;
            _characterService = characterService;

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

            if (_networkModule.Initialize())
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
