using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets.Game
{
    public class TS_CS_REPORT : Packet, ISerializablePacket
    {
        public ushort ReportLen { get; set; }

        public TS_CS_REPORT(Span<byte> buffer) : base(buffer) => Deserialize();

        public void Deserialize()
        {
            Span<byte> data = Data;

            ReportLen = BitConverter.ToUInt16(data.Slice(0, 2));
        }

        public void Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
