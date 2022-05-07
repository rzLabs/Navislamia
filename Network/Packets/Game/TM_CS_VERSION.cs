using Navislamia.Network.Enums;
using Navislamia.Network.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets.Game
{
    public class TM_CS_VERSION : Packet, ISerializablePacket
    {
        public CString Version = new CString(20);

        public TM_CS_VERSION() : base((ushort)ClientPackets.TM_CS_VERSION) { }

        public TM_CS_VERSION(Span<byte> buffer) : base(buffer) => Deserialize();

        public void Deserialize()
        {
            Span<byte> data = Data;

            Version.Data = data.Slice(0, Version.Length).ToArray();      
        }

        public void Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
