using Navislamia.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets.Upload
{
    public class TS_US_LOGIN_RESULT : Packet, ISerializablePacket
    {
        const int id = (int)UploadPackets.TS_US_LOGIN_RESULT;

        public ushort Result { get; set; }

        public TS_US_LOGIN_RESULT(ushort result) : base(id)
        {
            Result = result;
        }

        public TS_US_LOGIN_RESULT(Span<byte> buffer) : base(id)
        {
            Data = buffer.ToArray();

            Deserialize();
        }

        public void Deserialize() // We will never serialize this packet
        {
            Span<byte> _data = Data;

            Length = BitConverter.ToUInt32(_data.Slice(0, 4));

            base.ID = BitConverter.ToUInt16(_data.Slice(4, 2));

            Checksum = _data.Slice(6, 1)[0];

            Result = BitConverter.ToUInt16(_data.Slice(7, 2));
        }

        public void Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
