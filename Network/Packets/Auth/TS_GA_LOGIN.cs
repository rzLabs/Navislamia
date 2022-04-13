using Navislamia.Network.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets.Auth
{
    /// <summary>
    /// Packet class inspired and partially ported from Glandu2 Packet CLI Serializer
    /// <seealso cref="https://github.com/glandu2/rzu_packet_dotnet/blob/4e179816ae03de067d299342a90250e284c15ac3/packets/AuthGame/TS_GA_LOGIN.h#L5"/>
    /// </summary>
    public class TS_GA_LOGIN : Packet, ISerializablePacket
    {
        const int id = 20001;

        public ushort ServerIndex { get; set; }

        public CString ServerIP { get; set; } = new CString(16);

        public int ServerPort { get; set; }

        public CString ServerName { get; set; } = new CString(21);

        public CString ServerScreenshotURL { get; set; } = new CString(256);

        public bool IsAdultServer { get; set; }

        public TS_GA_LOGIN(byte[] buffer) : base(id, buffer) => throw new NotImplementedException(); // We will never receive this packet. No reason to implement this constructor

        public TS_GA_LOGIN(ushort index, string ip, short port, string name, string screenshotUrl = "about:blank", bool isAdult = false) : base(id) // TODO: this is not sustainable!
        {

            ServerIndex = index;
            ServerIP.String = $"{ip}";
            ServerPort = port;
            ServerName.String = $"{name}";
            ServerScreenshotURL.String = $"{screenshotUrl}";
            IsAdultServer = isAdult;

            Serialize(); // just go ahead and serialize the data. One less call later
        }

        public void Serialize()
        {
            int offset = 0;

            Length = this.GetLength();

            byte[] headerBuffer = this.CreateHeader();
            Data = new byte[512/*Length + headerBuffer.Length*/];

            Buffer.BlockCopy(headerBuffer, 0, Data, 0, headerBuffer.Length);

            offset = headerBuffer.Length;

            Buffer.BlockCopy(BitConverter.GetBytes(ServerIndex), 0, Data, offset, 2);

            offset += 2;

            byte[] buffer = ServerName.Data;
            Buffer.BlockCopy(buffer, 0, Data, offset, buffer.Length);

            offset += buffer.Length;

            buffer = ServerScreenshotURL.Data;
            Buffer.BlockCopy(buffer, 0, Data, offset, buffer.Length);

            offset += buffer.Length;

            Buffer.BlockCopy(BitConverter.GetBytes(IsAdultServer), 0, Data, offset, 1);

            offset++;

            buffer = ServerIP.Data;
            Buffer.BlockCopy(buffer, 0, Data, offset, buffer.Length);

            offset += buffer.Length;

            Buffer.BlockCopy(BitConverter.GetBytes(ServerPort), 0, Data, offset, 2);
        }

        public void Deserialize()
        {
            throw new NotImplementedException();
        }
    }
}
