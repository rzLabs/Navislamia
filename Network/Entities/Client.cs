using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using System.Net;
using System.Net.Sockets;
using Notification;
using Configuration;
using Network;
using Navislamia.Network.Packets;
using System.Collections.Concurrent;
using Network.Security;
using Navislamia.Network.Packets.Actions;
using Navislamia.Network.Packets.Actions.Interfaces;

namespace Navislamia.Network.Entities
{
    public class Client
    {
        public bool Ready = false;

        public IConfigurationService ConfigurationService;
        public INotificationService NotificationService;
        public INetworkService NetworkService;

        public IAuthActionService authActionSVC;
        public IGameActionService gameActionSVC;
        public IUploadActionService uploadActionSVC;

        public XRC4Cipher RecvCipher = new XRC4Cipher();
        public XRC4Cipher SendCipher = new XRC4Cipher();

        protected ConcurrentQueue<ISerializablePacket> recvMsgQueue = new ConcurrentQueue<ISerializablePacket>();
        public BlockingCollection<ISerializablePacket> RecvMsgCollection;

        protected ConcurrentQueue<ISerializablePacket> sendMsgQueue = new ConcurrentQueue<ISerializablePacket>();
        public BlockingCollection<ISerializablePacket> SendMsgCollection;

        public uint MsgVersion = 0x070300;

        public Client(Socket socket, int length, IConfigurationService configurationService, INotificationService notificationService, INetworkService networkService, IAuthActionService authActionService, IUploadActionService uploadActionService, IGameActionService gameActionService)
        {
            Socket = socket;
            BufferLen = length;
            ConfigurationService = configurationService;
            NotificationService = notificationService;
            NetworkService = networkService;
            authActionSVC = authActionService;
            uploadActionSVC = uploadActionService;
            gameActionSVC = gameActionService;

            DebugPackets = configurationService.Get<bool>("packet.debug", "Logs", false);

            var key = configurationService.Get<string>("cipher.key", "Network");

            RecvCipher.SetKey(key);
            SendCipher.SetKey(key);

            RecvMsgCollection = new BlockingCollection<ISerializablePacket>(recvMsgQueue);
            SendMsgCollection = new BlockingCollection<ISerializablePacket>(sendMsgQueue);

            initTasks();
        }

        public int DataLength => Data?.Length ?? -1;

        public int BufferLen = -1;

        public bool DebugPackets = false;

        public byte[] Data;

        public int DataOffset;

        public Socket Socket = null;

        public bool Connected => Socket?.Connected ?? false;

        public string IP
        {
            get
            {
                if (Socket is not null)
                {
                    IPEndPoint ep = Socket.RemoteEndPoint as IPEndPoint;

                    return ep.Address.ToString();
                }

                return null;
            }
        }

        public int Connect(IPEndPoint ep)
        {
            try
            {
                Socket.Connect(ep);
            }
            catch (Exception ex)
            {
                NotificationService.WriteException(ex);

                return 1;
            }

            return 0;
        }

        public virtual void Send(ISerializablePacket msg) { }

        public virtual void Listen() { }

        void initTasks() // Do not wait, these tasks will run for the lifetime of the server instance
        {
            List<Task> tasks = new List<Task>();

            tasks.Add(Task.Run(() =>
            {
                ISerializablePacket msg;

                while (true)
                    if (SendMsgCollection.IsAddingCompleted && !SendMsgCollection.IsCompleted)
                        while (SendMsgCollection.TryTake(out msg))
                            Send(msg);
            }));

            tasks.Add(Task.Run(() =>
            {
                ISerializablePacket msg;

                while (true)
                {
                    if (RecvMsgCollection.IsAddingCompleted && !RecvMsgCollection.IsCompleted)
                        while (RecvMsgCollection.TryTake(out msg))
                        {
                            if (this is AuthClient)
                                authActionSVC.Execute(this, msg);
                            else if (this is GameClient)
                                gameActionSVC.Execute(this, msg);
                            else if (this is UploadClient)
                                uploadActionSVC.Execute(this, msg);
                        }
                }
            }));

            Task.WhenAll(tasks);
        }
    }
}
