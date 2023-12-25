using System;
using System.Threading;
using Navislamia.Game;
using System.Threading.Tasks;
using Configuration;
using DevConsole.Exceptions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace DevConsole
{
    public class Application : IHostedService
    {
        private readonly IGameModule _gameModule;
        private readonly NetworkOptions _networkOptions;

        private readonly ILogger<Application> _logger;

        public Application(IGameModule gameModule,
            IOptions<NetworkOptions> networkOptions, ILogger<Application> logger)
        {
            _gameModule = gameModule;
            _networkOptions = networkOptions.Value;

            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
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
                // TODO: write the message and stack trace
                _logger.LogError("Failed to start the game service! {exception}", e);
                StopAsync(cancellationToken);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
