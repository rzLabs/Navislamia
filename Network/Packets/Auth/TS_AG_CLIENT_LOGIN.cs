using Navislamia.Network.Entities;
using Navislamia.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets.Auth
{
    public class TS_AG_CLIENT_LOGIN : Packet, ISerializablePacket
    {
        public CString Account { get; set; } = new CString(61);

        public int AccountID { get; set; }

        public ushort Result { get; set; }

        public byte PcBangMode { get; set; }

        public int EventCode { get; set; }

        public int Age { get; set; }

        public float ContinuousPlayTime { get; set; }

        public float ContinuousLogoutTime { get; set; }

        public TS_AG_CLIENT_LOGIN() : base((ushort)AuthPackets.TS_AG_CLIENT_LOGIN) => Serialize();

        public TS_AG_CLIENT_LOGIN(Span<byte> buffer) : base(buffer) => Deserialize();

        public void Deserialize()
        {
            Span<byte> data = Data;

            int offset = 0;

            Account.Data = data.Slice(0, Account.Length).ToArray();

            offset += Account.Length;

            AccountID = BitConverter.ToInt32(data.Slice(offset, 4));

            offset += 4;

            Result = BitConverter.ToUInt16(data.Slice(offset, 2));

            offset += 2;

            PcBangMode = data.Slice(offset, 1)[0];

            offset++;

            EventCode = BitConverter.ToUInt16(data.Slice(offset, 2));

            offset += 2;

            Age = BitConverter.ToInt32(data.Slice(offset, 4));

            offset += 4;

            ContinuousPlayTime = BitConverter.ToSingle(data.Slice(offset, 4));

            offset += 4;

            ContinuousLogoutTime = BitConverter.ToSingle(data.Slice(offset, 4));
        }

        public void Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
