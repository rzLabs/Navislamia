using System.Net.Sockets;
using Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Navislamia.Game.Network.Entities;
using Navislamia.Game.Network.Interfaces;
using Navislamia.Game.Network.Packets;

namespace Navislamia.Game.Network;
// TODO: Poll connections by configuration interval
// TODO: Disconnect/Destroy

public class NetworkService : INetworkService
{
    private readonly ILogger<NetworkService> _logger;

    private readonly IAuthClientService _authClientService;
    private readonly IUploadClientService _uploadClientService;
    private readonly IGameClientService _gameClientService;
    
    public NetworkService(ILogger<NetworkService> logger,
        IUploadClientService uploadClientService, IAuthClientService authClientService,
        IAuthActionService authActionService, IUploadActionService uploadActionService, 
        IGameActionService gameActionService, IGameClientService gameClientService)
    {
        _logger = logger;
        
        _authClientService = authClientService;
        _uploadClientService = uploadClientService;
        _gameClientService = gameClientService;
        
    }
    
    public bool IsReady()
    {
        return _authClientService.GetClient().Connection.Connected &&
               _uploadClientService.GetClient().Connection.Connected;
    }
    
    public void SendMessageToAuth(IPacket packet)
    {
        _authClientService.SendMessage(_authClientService.GetClient(), packet);
    }
    
    public void SendMessageToUpload(IPacket packet)
    {
        _uploadClientService.SendMessage(_uploadClientService.GetClient(), packet);
    }

    public void CreateAuthClient()
    {
        _authClientService.CreateAuthClient();
    }

    public void CreateUploadClient()
    {
        _uploadClientService.CreateUploadClient();
    }

    public ClientEntity GetAuthClient()
    {
        return _authClientService.GetClient();
    }

    public ClientEntity GetUploadClient()
    {
        return _uploadClientService.GetClient();
    }

    public ClientEntity CreateGameClient(Socket socket)
    {
       return  _gameClientService.CreateGameClient(socket);
    }

}