using Navislamia.Network.Entities;
using Navislamia.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets.Upload
{
    public class TS_SU_LOGIN : Packet, ISerializablePacket
    {
        const int id = (int)UploadPackets.TS_SU_LOGIN;

        public CString ServerName { get; set; } = new CString(21);

        public TS_SU_LOGIN(string serverName) : base(id)
        {
            ServerName.String = serverName;

            Serialize();
        }

        public void Serialize()
        {
            int offset = 0;

            Length = this.GetLength();

            byte[] headerBuffer = this.CreateHeader();
            Data = new byte[Length];

            Buffer.BlockCopy(headerBuffer, 0, Data, 0, headerBuffer.Length);

            offset = headerBuffer.Length;

            byte[] buffer = ServerName.Data;

            Buffer.BlockCopy(buffer, 0, Data, offset, buffer.Length);
        }

        public void Deserialize() // We will never deserialize this packet
        {
            throw new NotImplementedException();
        }
    }
}
