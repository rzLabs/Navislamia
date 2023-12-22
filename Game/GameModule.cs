using System;
using System.IO;
using Microsoft.Extensions.Options;

using Navislamia.Configuration.Options;
using Navislamia.Game.Models.Navislamia;
using Navislamia.Game.Network;
using Navislamia.Game.Repositories;
using Navislamia.Game.Maps;
using Navislamia.Game.Scripting;
using Navislamia.Game.Services;
using Microsoft.Extensions.Logging;

namespace Navislamia.Game
{
    public class GameModule : IGameModule
    {
        private readonly IScriptService _scriptService;
        private readonly IMapService _mapService;
        private readonly INetworkModule _networkModule;
        private readonly ScriptOptions _scriptOptions;
        private readonly MapOptions _mapOptions;

        private readonly IWorldRepository _worldRepository;
        private readonly ICharacterService _characterService;
        private readonly WorldEntity _worldEntity;

        private readonly ILogger<GameModule> _logger;

        public GameModule(INetworkModule networkModule, IOptions<ScriptOptions> scriptOptions, IOptions<MapOptions> mapOptions, ILogger<GameModule> logger, IScriptService scriptService, IMapService mapService, IWorldRepository worldRepository, ICharacterService characterService)
        {
            _scriptOptions = scriptOptions.Value;
            _mapOptions = mapOptions.Value;
            _networkModule = networkModule;
            _worldRepository = worldRepository;
            _characterService = characterService;

            _logger = logger;

            _worldEntity = worldRepository.LoadWorldIntoMemory();

            _scriptService = scriptService;
            _mapService = mapService;
        }

        public void Start(string ip, int port, int backlog)
        {   
            if (!LoadScripts(_scriptOptions.SkipLoading))
                return;

            if (!LoadMaps(_mapOptions.SkipLoading))
                return;

            if (!_networkModule.Start())
                return;

        }

        private bool LoadMaps(bool skip)
        {
            if (skip)
            {
                _logger.LogWarning("Map loading disabled!");
                return true;
            }

            // TODO: MapContent should be printing messages
            return _mapService.Start($"{Directory.GetCurrentDirectory()}\\Maps");
        }

        private bool LoadScripts(bool skip)
        {
            if (skip)
            {
                _logger.LogWarning("Script loading disabled!");
                return true;
            }

            return _scriptService.Start();
        }
    }
}
