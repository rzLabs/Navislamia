using Navislamia.Network.Entities;
using Navislamia.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets.Auth
{
    public class TS_GA_CLIENT_LOGIN : Packet, ISerializablePacket
    {
        public CString Account { get; set; } = new CString(61);
        
        public long OneTimeKey { get; set; }

        public TS_GA_CLIENT_LOGIN(CString account, long oneTimeKey) : base((ushort)AuthPackets.TS_GA_CLIENT_LOGIN)
        {
            Account = account;
            OneTimeKey = oneTimeKey;

            Serialize();
        }

        public void Deserialize() { } // This packet will never be deserialized

        public void Serialize()
        {
            int offset = 0;

            Length = this.GetLength();

            byte[] headerBuffer = this.CreateHeader();
            Data = new byte[512];

            Buffer.BlockCopy(headerBuffer, 0, Data, 0, headerBuffer.Length);

            offset = headerBuffer.Length;

            byte[] buffer = Account.Data;

            Buffer.BlockCopy(buffer, 0, Data, offset, buffer.Length);

            offset += buffer.Length;

            buffer = BitConverter.GetBytes(OneTimeKey);

            Buffer.BlockCopy(buffer, 0, Data, offset, buffer.Length);
        }
    }
}
