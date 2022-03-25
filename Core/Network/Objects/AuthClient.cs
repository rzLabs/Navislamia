using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using System.Net;
using System.Net.Sockets;

using Navislamia.Utilities;

using RappelzPackets;

using Serilog;

namespace Navislamia.Network
{
    public class AuthClient : Client
    {
        public AuthClient(Socket socket, int length) : base(socket, length) { }

        public override void Send(ISerializableStruct msg, bool beginReceive = true)
        {
            try
            {
                Stream = new MemoryStream(new byte[BufferLen]);

                msg.serialize(Stream, new packet_version_t(0x070300), Encoding.ASCII);

                Log.Debug("Transmitting server info to auth...");
                
                PacketUtility.DumpToConsole(Data);

                Socket.Send(Data);

                if (beginReceive)
                    Receive();
            }
            catch (Exception ex)
            {
                Log.Error("An exception occured while attempting to send data to the auth server!\n\nMessaage: {exMessage}\nStack-Trace: {exStackTrace}", ex.Message, ex.StackTrace);
                return;
            }
        }

        public override void Receive()
        {
            if (!Socket.Connected)
                return;

            Stream = new MemoryStream(new byte[BufferLen]);

            Socket.BeginReceive(Data, 0, Data.Length, SocketFlags.None, ReceiveCallback, this);
        }

        private void ReceiveCallback(IAsyncResult ar) // TODO: should be verifying the checksum
        {
            Log.Debug("Receiving data from the auth server...");

            Client auth = (Client)ar.AsyncState;

            Log.Debug("{count} bytes received from the Auth server!", auth.Socket.EndReceive(ar));

            try
            {
                var msg = new TS_AG_LOGIN_RESULT();
                msg.deserialize(Stream, new packet_version_t(0x070300), Encoding.ASCII);

                PacketUtility.DumpToConsole(Data);

                if (msg.result == 0) // 0 == success in most cpp scenario
                    Log.Information("Successfully registered with the Auth Server!");
                else
                    Log.Debug("Failed to register with the Auth Server!");
            }
            catch (Exception ex)
            {
                Log.Error("An exception occured while attempting to receive data from the auth server!\n\nMessaage: {exMessage}\nStack-Trace: {exStackTrace}", ex.Message, ex.StackTrace);
                return;
            }
        }
    }
}
