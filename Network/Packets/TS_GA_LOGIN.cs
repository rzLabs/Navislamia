using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network.Packets
{
    /// <summary>
    /// Packet class inspired and partially ported from Glandu2 Packet CLI Serializer
    /// <seealso cref="https://github.com/glandu2/rzu_packet_dotnet/blob/4e179816ae03de067d299342a90250e284c15ac3/packets/AuthGame/TS_GA_LOGIN.h#L5"/>
    /// </summary>
    public class TS_GA_LOGIN : Packet, ISerializablePacket
    {
        const int id = 20011;

        public ushort ServerIndex;
        public string ServerIP;
        public short ServerPort;
        public string ServerName;
        public string ServerScreenshotURL;
        public bool IsAdultServer;

        public TS_GA_LOGIN(byte[] buffer) : base(id, buffer) => throw new NotImplementedException(); // We will never receive this packet. No reason to implement this constructor

        public TS_GA_LOGIN(ushort index, string ip, short port, string name, string screenshotUrl = "about:blank", bool isAdult = false) : base(id, 2 + (ip.Length + 1) + 2 + (name.Length + 1) + (screenshotUrl.Length + 1) + 1) // TODO: this is not sustainable!
        {
            ServerIndex = index;
            ServerIP = ip;
            ServerPort = port;
            ServerName = name;
            ServerScreenshotURL = screenshotUrl;
            IsAdultServer = isAdult;

            Serialize(); // just go ahead and serialize the data. One less call later
        }

        public void Serialize()
        {
            int offset = 0;

            base.Checksum = Network.Packets.Checksum.Calculate((uint)Length, id);

            byte[] headerBuffer = Header.Generate(id, Length, Checksum);
            Data = new byte[headerBuffer.Length + Length];

            Buffer.BlockCopy(headerBuffer, 0, Data, 0, headerBuffer.Length);

            offset = headerBuffer.Length;

            Buffer.BlockCopy(BitConverter.GetBytes(ServerIndex), 0, Data, offset, 2);

            offset += 2;

            byte[] buffer = Encoding.Default.GetBytes($"{ServerName}\0");
            Buffer.BlockCopy(buffer, 0, Data, offset, buffer.Length);

            offset += buffer.Length;

            buffer = Encoding.Default.GetBytes($"{ServerScreenshotURL}\0");
            Buffer.BlockCopy(buffer, 0, Data, offset, buffer.Length);

            offset += buffer.Length;

            Buffer.BlockCopy(BitConverter.GetBytes(IsAdultServer), 0, Data, offset, 1);

            offset++;

            buffer = Encoding.Default.GetBytes($"{ServerIP}\0");
            Buffer.BlockCopy(buffer, 0, Data, offset, buffer.Length);

            offset += buffer.Length;

            Buffer.BlockCopy(BitConverter.GetBytes(ServerPort), 0, Data, offset, 2);
        }

        public void Deserialize(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
