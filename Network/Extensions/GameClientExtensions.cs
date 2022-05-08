using Navislamia.Network.Objects;
using Navislamia.Network.Packets;
using Network.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Extensions
{
    public static class GameClientExtensions
    {
        public static XRC4Cipher RecvCipher = new XRC4Cipher();
        public static XRC4Cipher SendCipher = new XRC4Cipher();

        public static PacketHeader PeekHeader(this GameClient client, int length = 7)
        {
            int len = Math.Min(length, client.BufferLen);

            byte[] headerBuffer = new byte[len];

            RecvCipher.Decode(client.Data, headerBuffer, len, true);

            return Header.GetPacketHeader(headerBuffer);
        }

        public static int Read(this GameClient client, byte[] buffer, int length)
        {
            int len = Math.Min(length, client.BufferLen);

            RecvCipher.Decode(buffer, client.Data, len);

            //Buffer.BlockCopy(buffer, len, client.Data, 0, client.Data.Length - len);

            return buffer.Length;
        }
    }
}
