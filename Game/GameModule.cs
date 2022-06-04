using System;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Configuration;
using Database;
using Maps;
using Network;
using Notification;
using Scripting;

using Serilog.Events;
using System.Collections.Generic;
using Navislamia.World;
using System.Threading;
using Navislamia.Game.DbLoaders;
using Navislamia.Database.Interfaces;

namespace Navislamia.Game
{
    public class GameModule : IGameService
    {
        IConfigurationService configSVC;
        IWorldService worldSVC;
        IDatabaseService dbSVC;
        IScriptingService scriptSVC;
        INotificationService notificationSVC;
        IMapService mapSVC;
        INetworkService networkSVC;

        public GameModule() { }

        public GameModule(IConfigurationService configurationService, IWorldService contentService, INotificationService notificationService, IDatabaseService databaseService, 
            IScriptingService scriptingService, IMapService mapService, INetworkService networkService)
        {
            configSVC = configurationService;
            worldSVC = contentService;
            notificationSVC = notificationService;
            dbSVC = databaseService;
            scriptSVC = scriptingService;
            mapSVC = mapService;
            networkSVC = networkService;
        }

        public int Start(string ip, int port, int backlog)
        {
            if (!configSVC.Get<bool>("skip_loading", "Scripts", false))
            {
                if (!scriptSVC.Initialize())
                {
                    notificationSVC.WriteError("Failed to start script service!");

                    return 1;
                }

                notificationSVC.WriteSuccess(new string[] { $"Script service started successfully!", $"[green]{scriptSVC.ScriptCount}[/] scripts loaded!" }, true);
            }
            else
                notificationSVC.WriteWarning("Script loading disabled!");

            if (!configSVC.Get<bool>("skip_loading", "Maps", false))
            {
                if (!mapSVC.Initialize())
                {
                    notificationSVC.WriteError("Failed to start the map service!");

                    return 1;
                }

                notificationSVC.WriteSuccess(new string[] { $"Map service started successfully!", $"[green]{mapSVC.MapCount.CX + mapSVC.MapCount.CY}[/] files loaded!" }, true);
            }
            else
                notificationSVC.WriteWarning("Map loading disabled!");

            if (!loadDbRepositories())
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

        bool loadDbRepositories()
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
