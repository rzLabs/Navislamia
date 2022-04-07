using Configuration;
using DevConsole.Properties;
using Navislamia.Game;
using Notification;
using Serilog.Events;
using System;
using System.Collections.Generic;
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

        public Application(IConfigurationService configurationService, INotificationService notificationService, IGameService gameService)
        {
            this.configurationService = configurationService;
            this.notificationService = notificationService;
            this.gameService = gameService;
        }

        public void Run()
        {
            notificationService.WriteString($"{Resources.arcadia}\n\nNavislamia starting...\n");

            string ip = configurationService.Get<string>("io.ip", "Network", "127.0.0.1");
            short port = configurationService.Get<short>("io.port", "Network", 4502);
            int backlog = configurationService.Get<int>("io.backlog", "Network", 100);

            if (gameService.Start("", 4502, 100) == 1)
            {
                notificationService.WriteMarkup("[bold red]Failed to start the game service![/]");

                return;
            }

            notificationService.WriteString("Successfully started and subscribed to the game service!", LogEventLevel.Information);
        }
    }
}
