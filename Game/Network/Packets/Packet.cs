using Navislamia.Network.Entities;
using System;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

namespace Navislamia.Network.Packets
{
    public interface IPacket
    {
        public ushort ID { get; }

        public uint Length { get; }

        public byte Checksum { get; }

        public byte[] Data { get; }

        public S GetDataStruct<S>();

        public string StructName { get;  }
    }

    public class Packet<T> : IPacket where T : new()
    {
        public Packet(ushort id, T dataStruct)
        {
            HeaderStruct = new Header(id);
            DataStruct = dataStruct;

            _headerLen = 7;
            _dataLen = Marshal.SizeOf(DataStruct);

            Data = new byte[_headerLen + _dataLen];

            serialize();
        }

        public Packet(byte[] data)
        {
            _headerLen = 7;
            _dataLen = Marshal.SizeOf(DataStruct);

            HeaderStruct = new Header();
            DataStruct = new T();

            Data = data;

            deserialize();
        }

        int _headerLen;
        int _dataLen;

        public Header HeaderStruct;
        public T DataStruct;

        public ushort ID => HeaderStruct.ID;

        public uint Length => HeaderStruct.Length;

        public byte Checksum => HeaderStruct.Checksum;

        public byte[] Data { get; set; }

        public S GetDataStruct<S>() => (S)(object)DataStruct;

        public string StructName => DataStruct.GetType().Name;

        private void serialize()
        {
            HeaderStruct.Length = (uint)(Data.Length);
            HeaderStruct.CalculateChecksum();

            var ptr = Marshal.AllocHGlobal(_headerLen);

            Marshal.StructureToPtr(HeaderStruct, ptr, true);
            Marshal.Copy(ptr, Data, 0, _headerLen);
            Marshal.FreeHGlobal(ptr);

            ptr = Marshal.AllocHGlobal(_dataLen);

            Marshal.StructureToPtr(DataStruct, ptr, false);
            Marshal.Copy(ptr, Data, _headerLen, _dataLen);
            Marshal.FreeHGlobal(ptr);
        }

        private void deserialize()
        {
            var ptr = Marshal.AllocHGlobal(_headerLen);
            Marshal.Copy(Data, 0, ptr, _headerLen);

            HeaderStruct = Marshal.PtrToStructure<Header>(ptr);

            Marshal.FreeHGlobal(ptr);

            ptr = Marshal.AllocHGlobal(_dataLen);
            Marshal.Copy(Data, 7, ptr, _dataLen);

            DataStruct = Marshal.PtrToStructure<T>(ptr);

            Marshal.FreeHGlobal(ptr);
        }
    }
}
