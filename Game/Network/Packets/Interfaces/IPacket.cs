namespace Navislamia.Game.Network.Packets.Interfaces
{
    public interface IPacket
    {
        public ushort Id { get; }

        public uint Length { get; }

        public byte Checksum { get; }

        public byte[] Data { get; set; }

        public string StructName { get; }

        public TS GetDataStruct<TS>();

        public string DumpStructToString();

        public string DumpDataToHexString();
    }
}
