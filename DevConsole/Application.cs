using System;
using System.Threading;
using System.Threading.Tasks;
using Configuration;
using DevConsole.Exceptions;
using DevConsole.Properties;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Navislamia.Game;
using Navislamia.Notification;

namespace DevConsole
{
    public class Application : IHostedService
    {
        private readonly IHostEnvironment _environment;
        private readonly IGameService _gameService;
        private readonly NetworkOptions _networkOptions;
        private readonly INotificationService _notificationService;

        public Application(IHostEnvironment environment, IGameService gameService, IOptions<NetworkOptions> networkOptions, INotificationService notificationService)
        {
            _environment = environment;
            _gameService = gameService;
            _notificationService = notificationService;
            _networkOptions = networkOptions.Value;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _notificationService.WriteString(Resources.arcadia);
            _notificationService.WriteString("Navislamia starting...");
            _notificationService.WriteString("Environment: " +  _environment.EnvironmentName);

            var ip = _networkOptions.Ip;
            var port = _networkOptions.Port;
            var backlog = _networkOptions.Backlog;
            
            if (string.IsNullOrWhiteSpace(ip) || port == null)
            {
                throw new InvalidConfigurationException("IP and/or Port or is either invalid or missing in configuration");
            }

            try
            {
                _gameService.Start(ip, port, backlog);
            }
            catch (Exception e)
            {
                StopAsync(cancellationToken);
                _notificationService.WriteMarkup("[bold red]Failed to start the game service![/]");

            }

            Console.ReadLine();
            return null;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
