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

using RappelzPackets;

//using Serilog;

namespace Objects.Network
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

                //Log.Debug("Transmitting server info to auth...");

                //PacketUtility.DumpToConsole(Data);

                Socket.Send(Data);

                if (beginReceive)
                    Receive();
            }
            catch (Exception ex)
            {
                //Log.Error("An exception occured while attempting to send data to the auth server!\n\nMessaage: {exMessage}\nStack-Trace: {exStackTrace}", ex.Message, ex.StackTrace);
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
            //Log.Debug("Receiving data from the auth server...");

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
                var msg = new TS_AG_LOGIN_RESULT();
                msg.FromArray(auth.Data, MsgVersion, Encoding.Default);

                if (msg.getReceivedId() != TS_AG_LOGIN_RESULT.packetID)
                {
                    //Log.Error("Expected message id: {id} received id: {recvid}", TS_AG_LOGIN_RESULT.packetID, msg.getReceivedId());
                    return;
                }

                // TODO:
                //if (configMgr.Get<bool>("packet.dump_to_console", "Logs"))
                //    PacketUtility.DumpToConsole(Data);

                // TODO:
                if (msg.result == 0) { } // 0 == success in most cpp scenario
                                         //Log.Information("Successfully registered with the Auth Server!");
                else { } // TODO:
                    //Log.Debug("Failed to register with the Auth Server!");
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
