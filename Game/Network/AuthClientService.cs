using Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Navislamia.Game.Network.Entities;
using Navislamia.Game.Network.Interfaces;

namespace Navislamia.Game.Network;

public class AuthClientService : IAuthClientService
{
    private readonly ILogger<AuthClientService> _logger;
    private readonly NetworkOptions _networkOptions;

    private AuthClient Client { get; set; }

    public AuthClientService(IOptions<NetworkOptions> networkOptions,
        ILogger<AuthClientService> logger)
    {
        _logger = logger;
        _networkOptions = networkOptions.Value;
    }

    public void CreateAuthClient()
    {
        if (Client != null)
        {
            _logger.LogWarning("AuthClient already exists. Skipping creation");
            return;
        }

        Client = new AuthClient();
        Client.CreateClientConnection(_networkOptions.Auth.Ip, _networkOptions.Auth.Port);
    }

    public AuthClient GetClient()
    {
        return Client;
    }
}
