using Navislamia.Network.Enums;
using Navislamia.Network.Interfaces;
using Navislamia.Network.Packets;
using Navislamia.Network.Packets.Actions.Interfaces;
using Navislamia.Network.Packets.Game;
using Navislamia.Notification;
using System;
using System.Net.Sockets;
using Configuration;
using Microsoft.Extensions.Options;

namespace Navislamia.Network.Entities
{

    public class GameClient : ClientBase<GameClientEntity>, IClient
    {
        private readonly NetworkOptions _networkOptions;

        public GameClient(IOptions<NetworkOptions> networkOptions, INotificationService notificationService, IAuthActionService authActionService,
            IGameActionService gameActionService, IUploadActionService uploadActionService) : base(notificationService,
            authActionService, gameActionService, uploadActionService)
        {
            _networkOptions = networkOptions.Value;
        }

        public Tag Info
        {
            get => ((GameClientEntity)Entity).Info;
            set => ((GameClientEntity)Entity).Info = value;
        }

        public override void Create(Socket socket)
        {
            var bufferLen = _networkOptions.BufferSize;

            GameClientEntity gameClient = new GameClientEntity()
            {
                Socket = socket,
                Data = new byte[bufferLen],
                MessageBuffer = new byte[bufferLen]
            };

            Entity = gameClient;
        }

        public override GameClientEntity GetEntity() => Entity as GameClientEntity;

        public override void PendMessage(ISerializablePacket msg)
        {
            messageQueue.PendSend(this, msg);

            messageQueue.Finalize(QueueType.Send);
        }

        public void SendResult(ushort id, ushort result, int value = 0)
        {
            PendMessage(new TS_SC_RESULT(id, result, value));
        }

        public override void Listen()
        {
            if (!Entity.Socket.Connected)
                return;

            try
            {
                Entity.Socket.BeginReceive(Entity.MessageBuffer, 0, Entity.MessageBuffer.Length, SocketFlags.None, listenCallback, Entity as GameClientEntity);
            }
            catch (Exception ex)
            {
                notificationSVC.WriteError($"An error occured while attempting to read listen for data from connection! {Entity.IP}:{Entity.Port}");
                notificationSVC.WriteException(ex);
            }
        }

        private void listenCallback(IAsyncResult ar)
        {
            GameClientEntity entity = (GameClientEntity)ar.AsyncState;

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

                messageQueue.LoadEncryptedBuffer(this, entity.MessageBuffer, availableBytes);
            }
            catch (Exception ex)
            {
                notificationSVC.WriteError($"An error occured while attempting to read data from connection! {Entity.IP}:{Entity.Port}");
                notificationSVC.WriteException(ex);
            }
        }
    }
}
