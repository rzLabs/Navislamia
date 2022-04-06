using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using System.Net;
using System.Net.Sockets;
using Network.Packets;

namespace Navislamia.Network.Objects
{
    public interface IClient
    {
        public void Connect(IPEndPoint ep);
    }


    public class Client : IClient
    {
        public uint MsgVersion = 0x070300;

        public Client(Socket socket, int length)
        {
            Socket = socket;
            BufferLen = length;
        }

        public int DataLength => Data?.Length ?? -1;

        public int BufferLen = -1;

        public byte[] Data;

        public uint MessageID
        {
            get
            {
                if (Data == null)
                    return 0;

                Span<byte> data = Data;

                return BitConverter.ToUInt16(data.Slice(4, 6));
            }
        }

        public Socket Socket = null;

        public bool Connected => Socket?.Connected ?? false;

        public void Connect(IPEndPoint ep)
        {
            if (Socket.Connected)
                return;

            try
            {
                Socket.Connect(ep);
            }
            catch (Exception ex)
            {

            }
        }

        public virtual void Send(ISerializablePacket msg, bool beginReceive = true) { }

        public virtual void Receive() { }
    }
}
