using Configuration;
using Navislamia.Network.Enums;
using Navislamia.Network.Interfaces;
using Navislamia.Network.Packets;
using Navislamia.Network.Packets.Upload;
using Network;
using Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Objects
{
    public class UploadClient : Client, IClient
    {
        bool debugPackets = false;

        public UploadClient(Socket socket, int length, IConfigurationService configurationService, INotificationService notificationService, INetworkService networkService) : base(socket, length, configurationService, notificationService, networkService)
        {
            debugPackets = ConfigurationService.Get<bool>("packet.debug", "Logs", false);
        }
        public override void Send(Packet msg, bool beginReceive = true)
        {
            if (!Socket.Connected)
                return;

            Socket.Send(msg.Data);

            if (ConfigurationService.Get<bool>("packet.debug", "Logs", false))
            {
                NotificationService.WriteDebug($"[orange3]Sending {msg.GetType().Name} ({msg.Data.Length} bytes) to the Upload Server...[/]");
                NotificationService.WriteString((msg).DumpToString());
            }

            Data = new byte[512];

            if (beginReceive)
                Socket.BeginReceive(Data, 0, Data.Length, SocketFlags.None, ReceiveCallback, this);
        }

        public override void Receive()
        {
            if (!Socket.Connected)
                return;

            Socket.BeginReceive(Data, 0, Data.Length, SocketFlags.None, ReceiveCallback, this);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            if (ConfigurationService.Get<bool>("debug", "Runtime", false))
                NotificationService.WriteDebug("Receiving data from the Upload server...");

            Client upload = (Client)ar.AsyncState;

            int readCnt = upload.Socket.EndReceive(ar);

            if (readCnt <= 0)
            {
                NotificationService.WriteMarkup("[bold red]Failed to read data from the Upload server![/]");
                return;
            }

            if (debugPackets)
                NotificationService.WriteDebug($"{readCnt} bytes received from the Upload server!");

            try
            {
                PacketHeader header = Header.GetPacketHeader(upload.Data);

                if (header.ID == (uint)UploadPackets.TS_US_LOGIN_RESULT)
                {
                    TS_US_LOGIN_RESULT msg = new TS_US_LOGIN_RESULT(upload.Data);

                    if (debugPackets)
                        NotificationService.WriteString(msg.DumpToString());

                    if (header.Checksum != msg.Checksum)
                    {
                        NotificationService.WriteMarkup($"[bold red]{msg.GetType().Name} bears an invalid checksum![/]");
                        return;
                    }

                    if (msg.Result == 0)
                        if (NetworkService.StartListener() > 0)
                            NotificationService.WriteError("Failed to start network listener!");
                }
            }
            catch (Exception ex)
            {
                NotificationService.WriteException(ex);
                return;
            }

            Receive();
        }
    }
}
