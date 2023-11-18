using Navislamia.Network.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Navislamia.Network.Enums;
using MemoryPack;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Navislamia.Network.Packets.Auth
{
    /// <summary>
    /// Packet class inspired and partially ported from Glandu2 Packet CLI Serializer
    /// <seealso cref="https://github.com/glandu2/rzu_packet_dotnet/blob/4e179816ae03de067d299342a90250e284c15ac3/packets/AuthGame/TS_GA_LOGIN.h#L5"/>
    /// </summary>
    public class TS_GA_LOGIN : Packet, ISerializablePacket
    {
        const int id = 20001;

        public ushort ServerIndex { get; set; }

        public CString ServerName { get; set; } = new CString(21);

        public CString ServerScreenshotURL { get; set; } = new CString(256);

        public bool IsAdultServer { get; set; }

        public CString ServerIP { get; set; } = new CString(16);

        public int ServerPort { get; set; }

        public TS_GA_LOGIN(ushort index, string ip, ushort port, string name, string screenshotUrl = "about:blank", bool isAdult = false) : base(id) 
        {

            ServerIndex = index;
            ServerIP.String = $"{ip}";
            ServerPort = port;
            ServerName.String = $"{name}";
            ServerScreenshotURL.String = $"{screenshotUrl}";
            IsAdultServer = isAdult;

            Serialize(); // just go ahead and serialize the data. One less call later
        }

        public void Serialize()
        {
            int offset = 0;

            Length = this.GetLength();

            byte[] headerBuffer = this.CreateHeader();
            Data = new byte[Length];

            Buffer.BlockCopy(headerBuffer, 0, Data, 0, headerBuffer.Length);

            offset = headerBuffer.Length;

            Buffer.BlockCopy(BitConverter.GetBytes(ServerIndex), 0, Data, offset, 2);

            offset += 2;

            byte[] buffer = ServerName.Data;
            Buffer.BlockCopy(buffer, 0, Data, offset, buffer.Length);

            offset += buffer.Length;

            buffer = ServerScreenshotURL.Data;
            Buffer.BlockCopy(buffer, 0, Data, offset, buffer.Length);

            offset += buffer.Length;

            Buffer.BlockCopy(BitConverter.GetBytes(IsAdultServer), 0, Data, offset, 1);

            offset++;

            buffer = ServerIP.Data;
            Buffer.BlockCopy(buffer, 0, Data, offset, buffer.Length);

            offset += buffer.Length;

            Buffer.BlockCopy(BitConverter.GetBytes(ServerPort), 0, Data, offset, 2);
        }

        public void Deserialize()
        {
            throw new NotImplementedException();
        }
    }

    // TODO: Nexitis here
    public interface IExPacket
    {
        public byte[] HeaderData { get; set; }

        public byte[] PacketData { get; set; }

        public byte[] MessageData { get;}
    }

    public class ExPacket : IExPacket
    {
        [MemoryPackIgnore]
        public int Length { get; set; }

        [MemoryPackIgnore]
        public ushort ID { get; set; }

        [MemoryPackIgnore]
        public byte Checksum { get; set; }

        [MemoryPackIgnore]
        public byte[] HeaderData { get; set; }

        [MemoryPackIgnore]
        public byte[] PacketData { get; set; }

        [MemoryPackIgnore]
        public byte[] MessageData
        {
            get
            {
                int headerLen = HeaderData?.Length ?? 0;
                int dataLen = PacketData?.Length ?? 0;

                if (headerLen == 0 && dataLen == 0)
                    return new byte[0];

                byte[] _buffer = new byte[headerLen + dataLen];

                Buffer.BlockCopy(HeaderData, 0, _buffer, 0, HeaderData.Length);
                Buffer.BlockCopy(PacketData, 0, _buffer, 7, PacketData.Length);

                return _buffer;
            }
        }

        public void Serialize<T>(ushort id, T entity)
        {
            PacketData = MemoryPackSerializer.Serialize<T>(entity);

            ID = id;
            Length = PacketData.Length + 7;
            Checksum = Packets.Checksum.Calculate(this);

            HeaderData = new byte[7];
            Buffer.BlockCopy(BitConverter.GetBytes(Length), 0, HeaderData, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(ID), 0, HeaderData, 4, 2);

            HeaderData[6] = Checksum;
        }

        public object Deserialize<T>(byte[] data)
        {
            // Copy the message data into header and data buffers
            Buffer.BlockCopy(data, 0, HeaderData, 0, 7);
            Buffer.BlockCopy(data, 7, PacketData, 0, data.Length - 7);

            // Parse the HeaderData
            Length = BitConverter.ToInt32(HeaderData, 0);
            ID = BitConverter.ToUInt16(HeaderData, 4);
            Checksum = HeaderData[6];

            return MemoryPackSerializer.Deserialize<T>(PacketData);
        }
    }

    [MemoryPackable]
    public partial class EX_TS_GA_LOGIN : ExPacket
    {
        public ushort ServerIndex;
        public string ServerName;
        public string ServerScreenshotURL;
        public byte IsAdultServer;
        public string ServerIP;
        public ushort ServerPort;
        
        public EX_TS_GA_LOGIN(ushort serverIndex, string serverName, string serverScreenshotURL, byte isAdultServer, string serverIP, ushort serverPort)
        {
            ServerIndex = serverIndex;
            ServerName = $"{serverName,-21}";
            ServerScreenshotURL = $"{serverScreenshotURL,-256}";
            IsAdultServer = isAdultServer;
            ServerIP = $"{serverIP,-16}";
            ServerPort = serverPort;

            Serialize((ushort)AuthPackets.TS_GA_LOGIN, this);
        }
    }
}
