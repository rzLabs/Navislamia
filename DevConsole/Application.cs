using System;
using System.Threading;
using Configuration;
using DevConsole.Properties;
using Navislamia.Game;
using Navislamia.Notification;
using System.Threading.Tasks;
using DevConsole.Exceptions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DevConsole
{
    public class Application : IHostedService
    {
        private readonly IHostEnvironment _environment;
        private readonly IGameModule _gameModule;
        private readonly NetworkOptions _networkOptions;
        private readonly INotificationService _notificationService;

        public Application(IHostEnvironment environment, IGameModule gameModule, IOptions<NetworkOptions> networkOptions, INotificationService notificationService)
        {
            _environment = environment;
            _gameModule = gameModule;
            _notificationService = notificationService;
            _networkOptions = networkOptions.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _notificationService.WriteString(Resources.arcadia);
            _notificationService.WriteString("Navislamia starting...\n");
            _notificationService.WriteMarkup($"Environment: [bold yellow]{_environment.EnvironmentName}[/]\n");

            var ip = _networkOptions.Ip;
            var port = _networkOptions.Port;
            var backlog = _networkOptions.Backlog;
            
            if (string.IsNullOrWhiteSpace(ip) || port <= 0)
            {
                throw new InvalidConfigurationException("IP and/or Port or is either invalid or missing in configuration");
            }

            try
            {
                _gameModule.Start(ip, port, backlog);
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
