using System;
using Configuration;
using DevConsole.Exceptions;
using DevConsole.Properties;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Navislamia.Game;

namespace DevConsole
{
    public class Application
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Application> _logger;
        private readonly IHostEnvironment _environment;
        
        private IGameService _gameService;
        
        public Application(IConfiguration configuration, IHostEnvironment environment, ILogger<Application> logger, IGameService gameService)
        {
            _logger = logger;
            _configuration = configuration;
            _environment = environment;
            _gameService = gameService;
        }
        
        public void Run()
        {
            _logger.LogInformation("{Arcadia}\\n\\nNavislamia starting...\\n", Resources.arcadia);
            _logger.LogInformation("Environment: {EnvironmentName}", _environment.EnvironmentName);

            var networkOptions = _configuration.GetSection("Network").Get<NetworkOptions>();
            var ip = networkOptions.Ip;
            var port = networkOptions.Port;
            var backlog = networkOptions.Backlog;
            
            if (string.IsNullOrWhiteSpace(ip) || port == null)
            {
                throw new InvalidConfigurationException("IP, Port or Backlog is either invalid or missing in configuration");
            }

            try
            {
                _gameService.Start(ip, Convert.ToInt32(port), Convert.ToInt32(backlog));
            }
            catch (Exception e)
            {
                throw new Exception($"Could not start Gameserver {e.InnerException}");
            }
        }
    }
}
