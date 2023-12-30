using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Navislamia.Game.Network.Packets.Interfaces;

namespace Navislamia.Game.Network.Packets;

public class Packet<T> : IPacket where T : new()
{
    private readonly int _headerLen;
    private readonly int _dataLen;

    public Header HeaderStruct;
    public T DataStruct;

    public ushort Id => HeaderStruct.ID;

    public uint Length => HeaderStruct.Length;

    public byte Checksum => HeaderStruct.Checksum;

    public byte[] Data { get; set; }

    public string StructName => typeof(T).Name;

    public Packet(ushort id, T dataStruct, int length = 0)
    {
        HeaderStruct = new Header(id);
        DataStruct = dataStruct;

        _headerLen = 7;
        _dataLen = Marshal.SizeOf(DataStruct);

        Data = new byte[_headerLen + ((length == 0) ?_dataLen : length)];

        Serialize();
    }

    public Packet(byte[] data)
    {
        _headerLen = 7;
        _dataLen = Marshal.SizeOf(DataStruct);

        HeaderStruct = new Header();
        DataStruct = new T();

        Data = data;

        Deserialize();
    }


    public TS GetDataStruct<TS>() => (TS)(object)DataStruct;

    private void Serialize()
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

    private void Deserialize()
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

    public string DumpStructToString()
    {
        var output = $"[orchid]Packet Info:[/]\n\nName: {DataStruct.GetType().Name}\n\nHeader:\n\n- [steelblue3]ID:[/] {Id}\n- [steelblue3]Length:[/] {Length}\n- [steelblue3]Checksum:[/] {Checksum}\n\nProperties:\n\n";

        foreach (var field in DataStruct.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            var str = $"- [steelblue3]{field.Name}[/] [deeppink4_2]({field.FieldType.Name})[/]: ";
            var value = field.GetValue(DataStruct);

            if (value is byte[])
                continue;

            output += $"{str + value}\n";
        }

        return output;
    }

    public string DumpDataToHexString()
    {
        Span<byte> buffer = Data;

        var maxWidth = Math.Min(16, buffer.Length);
        var rowHeader = 0;

        string outStr = null;
        string curRowStr = null;
        var curCol = 0;

        for (var i = 0; i < buffer.Length; i++)
        {
            var byteStr = $"{buffer[i]:x2}";
            curRowStr += $"{byteStr} ";
            curCol++;

            if (curCol != maxWidth)
            {
                continue;
            }
            
            rowHeader += 10;

            Span<byte> lineBuffer = buffer.Slice(i + 1 - maxWidth, maxWidth).ToArray();
            var lineBufferStr = string.Empty;

            foreach (var b in lineBuffer)
            {
                if (b == 0)
                    lineBufferStr += ".";
                else
                    lineBufferStr += System.Text.Encoding.Default.GetString(new byte[] { b });
            }

            if (maxWidth < 16) // There was not 16 bytes of data, so we must pad the rest to keep output aligned
            {
                var remainder = 16 - maxWidth;

                for (var j = 0; j < remainder; j++)
                {
                    curRowStr += "00 ";
                    lineBufferStr += ".";
                }
            }

            outStr += $"{rowHeader:D8}: {curRowStr}  {lineBufferStr}\n";
            curRowStr = null;
            curCol = 0;

            if (buffer.Length - i < maxWidth)
                maxWidth = buffer.Length - (i + 1);
        }

        return outStr;
    }
}