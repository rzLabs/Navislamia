using Network.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets
{
    public class TS_AG_LOGIN_RESULT : Packet, ISerializablePacket
    {
        const int id = 20002;

        public ushort Result { get; set; }

        public TS_AG_LOGIN_RESULT(Span<byte> buffer) : base(id) 
        {
            Data = buffer.ToArray();

            Deserialize();
        }

        public void Serialize()
        {
            Length = PacketUtility.GetLength(this);

            byte[] headerBuffer = Header.Generate(this);
            Data = new byte[512/*Length + headerBuffer.Length*/];

            byte[] buffer = BitConverter.GetBytes(Result);
            Buffer.BlockCopy(buffer, 0, Data, 7, 2);
        }

        public void Deserialize()
        {
            Span<byte> _data = Data;

            Result = BitConverter.ToUInt16(_data.Slice(7, 2));
        }
    }
}
