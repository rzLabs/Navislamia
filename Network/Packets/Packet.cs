using System;

namespace Navislamia.Network.Packets
{
    public class Packet
    {
        public uint Epic { get; set; } = 0x070300;

        public uint Length { get; set; }

        public ushort ID { get; set; } = 0x0000;

        public byte Checksum { get; set; }

        public byte[] Data { get; set; }

        public Packet(Span<byte> buffer)
        {
            if (buffer.Length < 7) // not enough header data
                throw new Exception("Not enough data present in the packet stream to define a packet header!");

            Length = BitConverter.ToUInt32(buffer.Slice(0, 4));
            ID = BitConverter.ToUInt16(buffer.Slice(4, 2));
            Checksum = this.Calculate();  

            Data = buffer[7..(int)Length].ToArray();
        }

        public Packet(ushort id)
        {
            ID = id;
            Length = 0;
            Data = new byte[0];
        }
    }
}
