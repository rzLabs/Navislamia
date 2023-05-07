using System;
using System.Threading;
using DevConsole.Properties;
using Navislamia.Game;
using Navislamia.Notification;
using System.Threading.Tasks;
using Configuration;
using DevConsole.Exceptions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Navislamia.Command;

namespace DevConsole
{
    public class Application : IHostedService
    {
        private readonly IHostEnvironment _environment;
        private readonly IGameModule _gameModule;
        private readonly NetworkOptions _networkOptions;
        private readonly INotificationModule _notificationModule;
        private readonly ICommandModule _commandModule;

        public Application(IHostEnvironment environment, IGameModule gameModule,
            IOptions<NetworkOptions> networkOptions, INotificationModule notificationModule, ICommandModule commandModule)
        {
            _environment = environment;
            _gameModule = gameModule;
            _notificationModule = notificationModule;
            _networkOptions = networkOptions.Value;
            _commandModule = commandModule;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _notificationModule.WriteString(Resources.arcadia);
            _notificationModule.WriteString("Navislamia starting...\n");
            _notificationModule.WriteMarkup($"Environment: [bold yellow]{_environment.EnvironmentName}[/]\n");

            var ip = _networkOptions.Game.Ip;
            var port = _networkOptions.Game.Port;
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
                _notificationModule.WriteMarkup($"[bold red]Failed to start the game service![/] {e.Message}");
            }

            _commandModule.Init();

            while (true)
                if (_commandModule.Wait() == 0)
                    break;

            //Console.ReadLine();
            return null;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
