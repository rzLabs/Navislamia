using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.Network.Packets
{
    public interface IPacket
    {
        public ushort ID { get; }

        public uint Length { get; }

        public byte Checksum { get; }

        public byte[] Data { get; set; }

        public string StructName { get; }

        public S GetDataStruct<S>();

        public string DumpStructToString();

        public string DumpDataToHexString();
    }
}
