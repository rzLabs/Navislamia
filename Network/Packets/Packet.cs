using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Network.Packets
{
    public class Packet
    {
        public uint Version { get; set; } = 0x070300;

        public uint Length { get; set; }

        public ushort ID { get; set; } = 0x0000;

        public byte Checksum { get; set; }

        public byte[] Data { get; set; }

        public Packet(ushort id, byte[] buffer)
        {
            ID = id;
            Length = Convert.ToUInt32(buffer.Length);
            Data = buffer;
        }

        public Packet(ushort id)
        {
            ID = id;
            Length = 0;
            Data = new byte[0];
        }
    }
}
