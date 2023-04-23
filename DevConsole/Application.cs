using System;
using System.Threading;
using System.Threading.Tasks;
using Configuration;
using DevConsole.Exceptions;
using DevConsole.Properties;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Navislamia.Game;

namespace DevConsole
{
    public class Application : IHostedService
    {
        private readonly ILogger<Application> _logger;
        private readonly IHostEnvironment _environment;
        private readonly IGameService _gameService;
        private readonly NetworkOptions _networkOptions;

        public Application(IHostEnvironment environment, ILogger<Application> logger, IGameService gameService, IOptions<NetworkOptions> networkOptions)
        {
            _logger = logger;
            _environment = environment;
            _gameService = gameService;
            _networkOptions = networkOptions.Value;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{Arcadia}", Resources.arcadia);
            _logger.LogInformation("Navislamia starting...");
            _logger.LogInformation("Environment: {EnvironmentName}", _environment.EnvironmentName);

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
                throw new Exception($"Could not start Gameserver {e.InnerException}");
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
