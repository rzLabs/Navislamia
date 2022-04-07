using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using System.Net;
using System.Net.Sockets;

//using Navislamia.Configuration;
using Utilities;
using Network.Packets;

//using Serilog;

using Navislamia.Network.Packets;
using Notification;
using Network;

namespace Navislamia.Network.Objects
{
    public class AuthClient : Client
    {
        public AuthClient(Socket socket, int length, INotificationService notificationService, INetworkService networkService) : base(socket, length, notificationService, networkService) { }

        public override void Send(ISerializablePacket msg, bool beginReceive = true)
        {
            if (!Socket.Connected)
                return;

            Socket.Send(msg.Data);

            Data = new byte[512];

            Socket.BeginReceive(Data, 0, Data.Length, SocketFlags.None, ReceiveCallback, this);
        }

        public override void Receive()
        {
            if (!Socket.Connected)
                return;

            Socket.BeginReceive(Data, 0, Data.Length, SocketFlags.None, ReceiveCallback, this);
        }

        private void ReceiveCallback(IAsyncResult ar) // TODO: should be verifying the checksum
        {
            NotificationService.WriteMarkup("[orange3]Receiving data from the auth server...[/]");

            Client auth = (Client)ar.AsyncState;

            int readCnt = auth.Socket.EndReceive(ar);

            if (readCnt <= 0)
            {
                NotificationService.WriteMarkup("[bold red]Failed to read data from the Auth server![/]");
                return;
            }

            NotificationService.WriteMarkup($"[orange3]{readCnt} bytes received from the Auth server![/]");

            try
            {
                if (auth.MessageID == 20002)
                {
                    Span<byte> data = auth.Data;

                    byte checksum = data.Slice(6, 7).ToArray()[0];

                    TS_AG_LOGIN_RESULT msg = new TS_AG_LOGIN_RESULT(data);

                    NotificationService.WriteString(Packets.PacketUtility.DumpToString(msg));

                    if (checksum != msg.Checksum)
                    {
                        NotificationService.WriteMarkup("[bold red]TS_AG_LOGIN_RESULT bears an invalid checksum![/]");
                        return;
                    }

                    if (msg.Result == 0)
                        NetworkService.StartListener();
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
