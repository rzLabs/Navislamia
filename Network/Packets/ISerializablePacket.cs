namespace Navislamia.Network.Packets
{
    public interface ISerializablePacket
    {
        public ushort ID { get; set; }

        public uint Length { get; set; }

        public byte Checksum { get; set; }

        public byte[] Data { get; set; }

        public void Serialize();

        public void Deserialize();
    }
}
