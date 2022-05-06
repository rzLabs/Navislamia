using Navislamia.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets.Upload
{
    using static Navislamia.Network.Packets.Checksum;


    public class TS_US_LOGIN_RESULT : Packet, ISerializablePacket
    {
        const int id = (int)UploadPackets.TS_US_LOGIN_RESULT;

        public ushort Result { get; set; }

        public TS_US_LOGIN_RESULT(ushort result) : base(id) => Result = result;

        public TS_US_LOGIN_RESULT(Span<byte> buffer) : base(buffer) => Deserialize();

        public void Deserialize() 
        {
            Span<byte> _data = Data;

            Result = BitConverter.ToUInt16(_data.Slice(0, 2));
        }

        public void Serialize() // We will never serialize this packet
        {
        }
    }
}
