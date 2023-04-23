using System;
using System.Text;

using Database;
using Maps;
using Network;
using Notification;
using Scripting;

using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Navislamia.Game.DbLoaders;

namespace Navislamia.Game
{
    public class GameModule : IGameService
    {
        private readonly IDatabaseService dbSVC;
        private readonly IScriptingService scriptSVC;
        private readonly INotificationService notificationSVC;
        private readonly IMapService mapSVC;
        private readonly INetworkService networkSVC;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GameModule> _logger;
        private readonly ScriptOptions _scriptOptions;
        private readonly MapOptions _mapOptions;

        public GameModule(INotificationService notificationService, IDatabaseService databaseService,
            IScriptingService scriptingService, IMapService mapService, INetworkService networkService,
            IConfiguration configuration, ILogger<GameModule> logger, IOptions<ScriptOptions> scriptOptions,
            IOptions<MapOptions> mapOptions)
        {
            notificationSVC = notificationService;
            dbSVC = databaseService;
            scriptSVC = scriptingService;
            mapSVC = mapService;
            networkSVC = networkService;
            
            _configuration = configuration;
            _logger = logger;
            _scriptOptions = scriptOptions.Value;
            _mapOptions = mapOptions.Value;

            if (_scriptOptions == null || _mapOptions == null)
            {
                throw new Exception("Script and/or Map options could not be loaded");
            }
        }

        public int Start(string ip, int port, int backlog)
        {
            if (_scriptOptions.SkipLoading)
            {
                _logger.LogWarning("Script loading disabled!");
            }
            else
            {
                if (!scriptSVC.Initialize())
                {
                    notificationSVC.WriteError("Failed to start script service!");

                    return 1;
                }

                notificationSVC.WriteSuccess(new string[] { $"Script service started successfully!", $"[green]{scriptSVC.ScriptCount}[/] scripts loaded!" }, true);
            }
           
            if (_mapOptions.SkipLoading)
            {
                _logger.LogWarning("Map loading disabled!");
            }
            else
            {
                if (!mapSVC.Initialize())
                {
                    notificationSVC.WriteError("Failed to start the map service!");

                    return 1;
                }

                notificationSVC.WriteSuccess(new string[] { $"Map service started successfully!", $"[green]{mapSVC.MapCount.CX + mapSVC.MapCount.CY}[/] files loaded!" }, true);
            }

            if (!LoadDbRepositories())
                return 1;

            if (networkSVC.Initialize() > 0)
                return 1;

            int curTime = 0;
            int maxTime = 5000;
            while (!networkSVC.Ready)
            {
                if (curTime >= maxTime)
                {
                    notificationSVC.WriteError("Network service timed out!");
                    return 1;
                }

                curTime += 250;

                // Wait for the auth/upload servers to respond
                Thread.Sleep(250);
            }

            networkSVC.StartListener();

            return 0;
        }

        bool LoadDbRepositories()
        {
            StringBuilder sb = new StringBuilder("Loading database repositories...\n");

            GameContent.Strings = new StringLoader(dbSVC, notificationSVC).Strings;

            sb.AppendLine($"- [orange3]{GameContent.Strings.Count}[/] strings loaded!");

            var monsterLoader = new MonsterLoader(dbSVC, notificationSVC);

            GameContent.MonsterInfo = monsterLoader.Monsters;

            sb.AppendLine($"- [orange3]{monsterLoader.Skills.Count}[/] monster skills loaded!");
            sb.AppendLine($"- [orange3]{monsterLoader.Drops.Count}[/] monster drops loaded!");
            sb.AppendLine($"- [orange3]{monsterLoader.Monsters.Count}[/] monsters loaded!");

            GameContent.NpcInfo = new NpcLoader(dbSVC, notificationSVC).Npc;

            sb.AppendLine($"- [orange3]{GameContent.NpcInfo.Count}[/] npc loaded!");

            notificationSVC.WriteMarkup(sb.ToString());

            return true;
        }
    }
}
