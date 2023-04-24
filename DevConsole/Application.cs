using Configuration;
using DevConsole.Properties;
using Navislamia.Command;
using Navislamia.Game;
using Navislamia.Notification;
using Serilog.Events;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevConsole
{
    public class Application : IApplication
    {
        private IConfigurationService configurationService;
        private INotificationService notificationService;
        private IGameService gameService;
        private ICommandService commandService;

        public Application(IConfigurationService configurationService, INotificationService notificationService, IGameService gameService, ICommandService commandService)
        {
            this.configurationService = configurationService;
            this.notificationService = notificationService;
            this.gameService = gameService;
            this.commandService = commandService;
        }

        public async Task<int> Run()
        {
            notificationService.WriteString($"{Resources.arcadia}\n\nNavislamia starting...\n");

            string ip = configurationService.Get<string>("io.ip", "network", "127.0.0.1");
            short port = configurationService.Get<short>("io.port", "network", 4502);
            int backlog = configurationService.Get<int>("io.backlog", "network", 100);

            if (await gameService.Start(ip, port, backlog) >= 1)
            {
                notificationService.WriteMarkup("[bold red]Failed to start the game service![/]");

                return 1;
            }

            return 0;
        }
    }
}
