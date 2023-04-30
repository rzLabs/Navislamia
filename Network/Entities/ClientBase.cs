﻿
using Navislamia.Network.Enums;
using Navislamia.Network.Packets;
using Navislamia.Notification;
using System.Net.Sockets;
using System.Net;
using System;
using Configuration;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Navislamia.Network.Packets.Actions.Interfaces;
using Serilog;

namespace Navislamia.Network.Entities
{
    public abstract class ClientBase<T> where T : ClientEntity
    {
        internal readonly INotificationService notificationSVC;
        internal readonly IOptions<NetworkOptions> _networkTmpOptions; // Temporary to merge options pattern. Refactor Clients to be injectable
        internal readonly IOptions<LogOptions> _logTmpOptions; // Temporary to merge options pattern. Refactor Clients to be injectable
        internal readonly IMessageQueue _messageQueue;

        public ClientEntity Entity;

        public ClientBase(INotificationService notificationService, IAuthActionService authActionService, IGameActionService gameActionService, IUploadActionService uploadActionService, IMessageQueue messageQueue)
        {
            notificationSVC = notificationService;
            _messageQueue = messageQueue;
        }

        public virtual ClientEntity GetEntity()
        {
            return Entity;
        }

        public virtual void Create(Socket socket) { }

        public int Connect(IPEndPoint ep)
        {
            try
            {
                Entity.Socket.Connect(ep);
            }
            catch (Exception ex)
            {
                notificationSVC.WriteError($"An error occured while attempting to connect to remote endpoint!");
                notificationSVC.WriteException(ex);

                return 1;
            }

            return 0;
        }

        public virtual void Send(byte[] data)
        {
            try
            {
                Entity.Socket.BeginSend(data, 0, data.Length, SocketFlags.None, sendCallback, Entity);
            }
            catch (Exception ex)
            {
                notificationSVC.WriteError($"An error occured while attempting to send data to connection! {Entity.IP}:{Entity.Port}");
                notificationSVC.WriteException(ex);
                return;
            }
        }

        private void sendCallback(IAsyncResult ar)
        {
            ClientEntity entity = (ClientEntity)ar.AsyncState;

            int bytesSent = entity.Socket.EndSend(ar);

            Listen();
        }

        public virtual void Listen() { }

        public virtual void PendMessage(ISerializablePacket msg) { }

    }
}


