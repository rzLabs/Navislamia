using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network.Packets
{
    public class TS_GA_LOGIN : Packet, ISerializablePacket
    {
        const int id = 20011;
        
        public string ServerIP;
        public short ServerPort;
        public string ServerName;
        public string ServerScreenshotURL;
        public bool IsAdultServer;

        short ipLength = 0;
        short nameLength = 0;
        short screenshotLength = 0;

        public TS_GA_LOGIN(byte[] buffer) : base(id, buffer)
        {
            int offset = 0;

            Span<byte> data = Data;

            ipLength = BitConverter.ToInt16(data.Slice(0, 2));
            nameLength = BitConverter.ToInt16(data.Slice(2, 2));
            screenshotLength = BitConverter.ToInt16(data.Slice(4, 2));

            offset = 6;

            ServerIP = Encoding.Default.GetString(data.Slice(offset, ipLength));
            offset += ipLength;

            ServerPort = BitConverter.ToInt16(data.Slice(offset, 2));
            offset += 2;

            ServerName = Encoding.Default.GetString(data.Slice(offset, nameLength));
            offset += nameLength;

            ServerScreenshotURL = Encoding.Default.GetString(data.Slice(offset, screenshotLength));
            offset += screenshotLength;

            IsAdultServer = BitConverter.ToBoolean(data.Slice(offset, 1));
        }

        public TS_GA_LOGIN(string ip, short port, string name, string screenshotUrl = "about:blank", bool isAdult = false) : base(id, ip.Length + 2 + name.Length + screenshotUrl.Length + 1)
        {
            ServerIP = ip;
            ipLength = (short)ip.Length;
            ServerPort = port;
            ServerName = name;
            nameLength = (short)name.Length;
            ServerScreenshotURL = screenshotUrl;
            screenshotLength = (short)screenshotUrl.Length;
            IsAdultServer = isAdult;
        }

        public void Serialize()
        {
            int offset = 0;

            byte[] headerBuffer = Header.Generate(id, Length, Checksum);
            Data = new byte[headerBuffer.Length + Length];

            Buffer.BlockCopy(headerBuffer, 0, Data, 0, headerBuffer.Length);

            offset = headerBuffer.Length;

            byte[] buffer = Encoding.Default.GetBytes($"{ServerIP}\0");
            Buffer.BlockCopy(buffer, 0, Data, offset, buffer.Length);

            offset += buffer.Length;

            Buffer.BlockCopy(BitConverter.GetBytes(ServerPort), 0, Data, offset, 2);

            offset += 2;

            buffer = Encoding.Default.GetBytes($"{ServerName}\0");
            Buffer.BlockCopy(buffer, 0, Data, offset, buffer.Length);

            offset += buffer.Length;

            buffer = Encoding.Default.GetBytes($"{ServerScreenshotURL}\0");
            Buffer.BlockCopy(buffer, 0, Data, offset, buffer.Length);

            offset += buffer.Length;

            Buffer.BlockCopy(BitConverter.GetBytes(IsAdultServer), 0, Data, offset, 1);
        }

        public void Deserialize(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
