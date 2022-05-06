using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets.Auth
{
    public class TS_AG_LOGIN_RESULT : Packet, ISerializablePacket
    {
        const int id = 20002;

        public ushort Result { get; set; }

        public TS_AG_LOGIN_RESULT(Span<byte> buffer) : base(buffer.ToArray()) => Deserialize();

        public void Serialize()
        {
        }

        public void Deserialize()
        {
            Span<byte> data = Data;

            Result = BitConverter.ToUInt16(data.Slice(0, 2));
        }
    }
}
