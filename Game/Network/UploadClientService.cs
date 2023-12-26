using Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Navislamia.Game.Network.Entities;
using Navislamia.Game.Network.Interfaces;

namespace Navislamia.Game.Network;

public class UploadClientService : IUploadClientService 
{
    private readonly ILogger<UploadClientService> _logger;
    private readonly IUploadActionService _uploadActionService;
    private readonly NetworkOptions _networkOptions;
    
    private UploadClient Client { get; set; }

    public UploadClientService(IUploadActionService uploadActionService, IOptions<NetworkOptions> networkOptions, 
        ILogger<UploadClientService> logger)
    {
        _logger = logger;
        _networkOptions = networkOptions.Value;
        _uploadActionService = uploadActionService;
    }
    
    public void CreateUploadClient()
    {
        if (Client != null)
        {
            _logger.LogWarning("Client already exists. Skipping creation");
            return;
        }

        Client = new UploadClient();
        Client.CreateClientConnection(_networkOptions.Upload.Ip, _networkOptions.Upload.Port);
    }

    public UploadClient GetClient()
    {
        return Client;
    }
}