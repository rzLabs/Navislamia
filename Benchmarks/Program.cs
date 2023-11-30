using BenchmarkDotNet.Running;
using Benchmarks.Benchmarks;

BenchmarkRunner.Run<StringEncodingBenchmarks>();

public interface IExPacket<T>
{
    public Ex_PacketHeader GetHeader();

    public byte[] GetHeaderData();

    public T GetDataEntity();

    public byte[] GetMessageData();
}

public class ExPacket<T> : IExPacket<T>
{
    public int Length { get; set; }
    private byte[] Header { get; set; }
    private byte[] Data { get; set; }
    private Ex_PacketHeader HeaderEntity { get; set; }
    private T DataEntity { get; set; }

    public byte[] GetMessageData()
    {
        byte[] buffer = new byte[Header.Length + Data.Length];

        Buffer.BlockCopy(Header, 0, buffer, 0, Header.Length);
        Buffer.BlockCopy(Data, 0, buffer, 7, Data.Length);

        return buffer;
    }

    public void Serialize(ushort id, T entity)
    {
        // Serialize the data first so we know its length
        DataEntity = entity;
        Data = MemoryPackSerializer.Serialize(DataEntity);

        // Now we can create and serialize the header
        HeaderEntity = Header.Create(this, id);
        Header = MemoryPackSerializer.Serialize(HeaderEntity);
    }

    public void Deserialize(byte[] data)
    {
        // Copy the message data into header and data buffers
        Buffer.BlockCopy(data, 0, Header, 0, 7);
        Buffer.BlockCopy(data, 7, Data, 0, data.Length - 7);

        //Deserialize into their respective entities
        HeaderEntity = MemoryPackSerializer.Deserialize<Ex_PacketHeader>(Header);
        DataEntity = MemoryPackSerializer.Deserialize<T>(Data);
    }
}