using System;
using System.Net.Sockets;

using Navislamia.Game.Network.Interfaces;
using Network.Security;

namespace Navislamia.Game.Network
{
    /// <summary>
    /// Abstraction of the Connection class that provides encode/decode capabilities for Game BaseClientService connections
    /// </summary>
    public class CipherConnection : Connection, IConnection
    {
        XRC4Cipher _sendCipher = new XRC4Cipher();
        XRC4Cipher _receiveCipher = new XRC4Cipher();

        /// <summary>
        /// Creates a new instance of the cipher connection wrapper abstraction
        /// </summary>
        /// <param name="socket">Socket being wrapped</param>
        /// <param name="cipherKey">Key to be used in cipher operations</param>
        public CipherConnection(Socket socket, string cipherKey) : base(socket)
        {
            _sendCipher.SetKey(cipherKey);
            _receiveCipher.SetKey(cipherKey);
        }

        /// <summary>
        /// Peeks encoded data in the receive buffer for data.
        /// </summary>
        /// <param name="length">Amount of data to be peeked from the receive buffer</param>
        /// <returns>ReadOnlySpan pointing to the data inside the receive buffer</returns>
        public override ReadOnlySpan<byte> Peek(int length)
        {
            var _peekBuffer = new byte[length];

            Buffer.BlockCopy(ReceiveBuffer, 0, _peekBuffer, 0, length);

            _receiveCipher.Decode(_peekBuffer, _peekBuffer, length, true);

            return new ReadOnlySpan<byte>(_peekBuffer, 0, length);
        }

        /// <summary>
        /// Reads encoded data from the receive buffer and moves remaining data to the front of the receive buffer
        /// </summary>
        /// <param name="length">Amount of data to be read</param>
        /// <returns>Byte array containing read data</returns>
        public override byte[] Read(int length)
        {
            var _readBuffer = base.Read(length);

            _receiveCipher.Decode(_readBuffer, _readBuffer, length);

            return _readBuffer;
        }

        /// <summary>
        /// Adds an encoded message to the send queue 
        /// </summary>
        /// <param name="buffer">Message data to be sent</param>
        public override void Send(byte[] buffer)
        {
            _sendCipher.Encode(buffer, buffer, buffer.Length);

            SendQueue.Enqueue(buffer);
        }
    }
}
