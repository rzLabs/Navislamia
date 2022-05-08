using Navislamia.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets.Game
{
    public class TS_SC_RESULT : Packet, ISerializablePacket
    {
        public ushort RequestMsgID { get; set; }

        public ushort Result { get; set; }

        public int Value { get; set; }

        public TS_SC_RESULT(ushort id, ushort result, int value = 0) : base((ushort)ClientPackets.TM_SC_RESULT)
        {
            RequestMsgID = id;
            Result = result;
            Value = value;

            Serialize();
        }

        public void Deserialize()
        {
            throw new NotImplementedException();
        }

        public void Serialize()
        {
            int offset = 0;

            Length = this.GetLength();

            byte[] headerBuffer = this.CreateHeader();
            Data = new byte[512];

            Buffer.BlockCopy(headerBuffer, 0, Data, 0, headerBuffer.Length);

            offset = headerBuffer.Length;

            byte[] buffer = BitConverter.GetBytes(RequestMsgID);

            Buffer.BlockCopy(buffer, 0, Data, offset, buffer.Length);

            offset += buffer.Length;

            buffer = BitConverter.GetBytes(Result);

            Buffer.BlockCopy(buffer, 0, Data, offset, buffer.Length);

            offset += buffer.Length;

            buffer = BitConverter.GetBytes(Value);

            Buffer.BlockCopy(buffer, 0, Data, offset, buffer.Length);
        }
    }
}
