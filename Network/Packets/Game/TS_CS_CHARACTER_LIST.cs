using Navislamia.Network.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets.Game
{
    public class TS_CS_CHARACTER_LIST : Packet, ISerializablePacket
    {
        public CString Account { get; set; } = new CString(61);

        public TS_CS_CHARACTER_LIST(Span<byte> buffer) : base(buffer) => Serialize();

        public void Deserialize()
        {
            throw new NotImplementedException();
        }

        public void Serialize()
        {
            Account.Data = ((Span<byte>)Data).Slice(0, Account.Length).ToArray();
        }
    }
}
