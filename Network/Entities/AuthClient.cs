using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using System.Net;
using System.Net.Sockets;


using Navislamia.Network.Enums;
using Navislamia.Network.Packets;
using Navislamia.Network.Packets.Auth;
using Navislamia.Network.Packets.Actions;
using static Navislamia.Network.Packets.Extensions;

using Navislamia.Notification;
using Network;
using Configuration;
using Microsoft.Extensions.Options;
using Navislamia.Network.Interfaces;
using Navislamia.Network.Packets.Actions.Interfaces;

namespace Navislamia.Network.Entities
{
    public class AuthClient : ClientBase<AuthClientEntity>, IClient
    {
        private readonly NetworkOptions _networkOptions;
        
        public AuthClient(IOptions<NetworkOptions> networkOptions, INotificationService notificationService, IAuthActionService authActionService, IGameActionService gameActionService, IUploadActionService uploadActionService) :
        base(notificationService, authActionService, gameActionService, uploadActionService)
        {
            _networkOptions = networkOptions.Value;
        }

        public override void Create(Socket socket)
        {
            var bufferLen = _networkOptions.BufferSize;

            AuthClientEntity gameClient = new AuthClientEntity()
            {
                Socket = socket,
                Data = new byte[bufferLen],
                MessageBuffer = new byte[bufferLen]
            };

            Entity = gameClient;
        }

        public override AuthClientEntity GetEntity() => Entity as AuthClientEntity;

        public override void PendMessage(ISerializablePacket msg)
        {
            messageQueue.PendSend(this, msg);

            messageQueue.Finalize(QueueType.Send);
        }

        public override void Listen()
        {
            if (!Entity.Socket.Connected)
                return;

            try
            {
                Entity.Socket.BeginReceive(Entity.MessageBuffer, 0, Entity.MessageBuffer.Length, SocketFlags.None, listenCallback, Entity as AuthClientEntity);
            }
            catch (Exception ex)
            {
                notificationSVC.WriteError($"An error occured while attempting to read listen for data from connection! {Entity.IP}:{Entity.Port}");
                notificationSVC.WriteException(ex);
            }
        }

        private void listenCallback(IAsyncResult ar)
        {
            AuthClientEntity entity = (AuthClientEntity)ar.AsyncState;

            if (!entity.Socket.Connected)
            {
                notificationSVC.WriteError($"Read attempted for closed connection! {Entity.IP}:{Entity.Port}");
                return;
            }

            try
            {
                int availableBytes = entity.Socket.EndReceive(ar);

                if (availableBytes == 0)
                    Listen();

                messageQueue.ProcessClientData(this, entity.MessageBuffer, availableBytes);
            }
            catch (Exception ex)
            {
                notificationSVC.WriteError($"An error occured while attempting to read data from connection! {Entity.IP}:{Entity.Port}");
                notificationSVC.WriteException(ex);
            }
        }
    }
}
