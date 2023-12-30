using System;
using System.Threading;
using Navislamia.Game;
using System.Threading.Tasks;
using DevConsole.Exceptions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Navislamia.Configuration.Options;

namespace DevConsole;

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

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var ip = _networkOptions.Game.Ip;
        var port = _networkOptions.Game.Port;
        var backlog = _networkOptions.Backlog;
            
        try
        {
            if (string.IsNullOrWhiteSpace(ip))
            {
                throw new InvalidConfigurationException("IP and/or Port is either invalid or missing");
            }
                
            _gameModule.Start(ip, port, backlog);
            _logger.LogInformation("Press {combination} to stop", "CTRL + C");
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to start the game service! {exception}", e);
            await StopAsync(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // To stuff here required to gracefully stop the server
        _logger.LogWarning("Stopping Navislamia");
        return Task.CompletedTask;
    }

}