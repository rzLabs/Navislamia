using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using System.Net;
using System.Net.Sockets;

using Navislamia.Configuration;

using RappelzPackets;

namespace Navislamia.Network
{
    public class Client
    {
        ConfigurationManager configMgr = ConfigurationManager.Instance;
        public packet_version_t MsgVersion = new packet_version_t(0x07030);

        public Client(Socket socket, int length)
        {
            Socket = socket;
            BufferLen = length;
        }

        public int DataLength => Data?.Length ?? -1;

        public int BufferLen = -1;

        protected byte[] data;
        public byte[] Data
        {
            get
            {
                if (stream != null && data == null)
                    data = stream.ToArray();

                return data;
            }
            set => data = value;
        }

        protected MemoryStream stream;
        /// <summary>
        /// Return the Client data buffer wrapped in a MemoryStream object
        /// </summary>
        public MemoryStream Stream
        {
            get
            {
                if (data == null)
                {
                    stream = new MemoryStream();
                    data = new byte[stream.Length];
                }

                stream = new MemoryStream(data);

                return stream;
            }
            set
            {
                stream = value;
                data = new byte[stream.Length];
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

        public virtual void Send(ISerializableStruct msg, bool beginReceive = true) { }

        public virtual void Receive() { }
    }
}
