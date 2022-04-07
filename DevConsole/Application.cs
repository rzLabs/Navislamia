using Configuration;
using Navislamia.Game;
using Notification;
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
            notificationService.WriteMarkup("It works!", Serilog.Events.LogEventLevel.Information);
        }
    }
}
