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

namespace Navislamia.Network.Objects
{

    public class AuthClient : Client
    {
        public AuthClient(Socket socket, int length) : base(socket, length) { }

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
            //("Receiving data from the auth server...");

            Client auth = (Client)ar.AsyncState;

            int readCnt = auth.Socket.EndReceive(ar);

            if (readCnt <= 0)
            {
                //Log.Error("Failed to read data from the Auth server!");
                return;
            }

            //Log.Debug("{count} bytes received from the Auth server!", readCnt);

            try
            {
                if (auth.MessageID == 20002)
                {
                    Span<byte> data = auth.Data;

                    TS_AG_LOGIN_RESULT msg = new TS_AG_LOGIN_RESULT(data.Slice(7, data.Length - 7));

                    if (msg.Result == 0)
                    {
                        // TODO: success
                    }
                }
            }
            catch (Exception ex)
            {
                //Log.Error("An exception occured while attempting to receive data from the auth server!\n\nMessaage: {exMessage}\nStack-Trace: {exStackTrace}", ex.Message, ex.StackTrace);
                return;
            }

            Receive();
        }
    }
}
