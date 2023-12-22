using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using Network.Security;
using System.Collections.Concurrent;
using System.Threading;

namespace Navislamia.Game.Network.Entities
{
    /// <summary>
    /// Socket wrapper for handling Auth/Upload/Game connections
    /// </summary>
    public class Connection : IConnection
    {
        internal Socket Socket;

        internal bool _disconnectSignaled = false;

        internal volatile int BytesSent;
        internal volatile int BytesReceived;

        private int _dataLength = 0;
        internal byte[] ReceiveBuffer = new byte[32768];

        internal ConcurrentQueue<byte[]> SendQueue = new ConcurrentQueue<byte[]>();

        internal CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        internal CancellationToken cancellationToken;

        /// <summary>
        /// Event triggered when data has been sent to the remote connection. (includes count of bytes sent)
        /// </summary>
        public Action<int> OnDataSent { get; set; }   

        /// <summary>
        /// Event triggered when data has been received from the remote connection (includes count of bytes received)
        /// </summary>
        public Action<int> OnDataReceived { get; set; }

        /// <summary>
        /// Event triggered when the remote connection has disconnected
        /// </summary>
        public Action OnDisconnected { get; set; }


        /// <summary>
        /// Creates a new instance of the connection wrapper
        /// </summary>
        /// <param name="socket">Socket being wrapped</param>
        public Connection(Socket socket)
        {
            Socket = socket;
        }

        /// <summary>
        /// Connects to the remote host at provided ip and port
        /// </summary>
        /// <param name="ip">Remote host ip address</param>
        /// <param name="port">Remote host port</param>
        public void Connect(string ip, int port)
        {
            Socket.Connect(ip, port);
        }

        /// <summary>
        /// Gracefully disconnects the connection
        /// </summary>
        public void Disconnect()
        {
            if (!Connected)
                return;

            Socket.Disconnect(false);
        }

        /// <summary>
        /// Local IP Address of the wrapped socket
        /// </summary>
        public string LocalIp
        {
            get
            {
                var _localEP = Socket?.LocalEndPoint as IPEndPoint;

                if (_localEP is not null)
                    return _localEP.Address.ToString();

                return default;
            }
        }

        /// <summary>
        /// Local port of the wrapped socket
        /// </summary>
        public int LocalPort
        {
            get
            {
                var _localEP = Socket?.LocalEndPoint as IPEndPoint;

                if (_localEP is not null)
                    return _localEP.Port;

                return -1;
            }
        }

        /// <summary>
        /// Remote IP Address of the wrapped socket
        /// </summary>
        public string RemoteIp
        {
            get
            {
                var _remoteEP = Socket?.RemoteEndPoint as IPEndPoint;

                if ( _remoteEP is not null)
                    return _remoteEP.Address.ToString();

                return default;
            }
        }

        /// <summary>
        /// Remote port of the wrapped socket
        /// </summary>
        public int RemotePort
        {
            get
            {
                var _remoteEP = Socket?.RemoteEndPoint as IPEndPoint;

                if (_remoteEP is not null)
                    return _remoteEP.Port;

                return -1;
            }
        }

        /// <summary>
        /// Checks if the wrapped socket is connected via polling
        /// </summary>
        public bool Connected
        {
            get
            {
                if (!Socket.Connected || Socket.Poll(1000, SelectMode.SelectRead) && Socket.Available == 0 || _disconnectSignaled)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Starts internal processes like checking for disconnect, sending messages and begins listening
        /// </summary>
        public virtual void Start()
        {
            cancellationToken = cancellationTokenSource.Token;

            Task.Run(checkForDisconnect, cancellationToken);
            Task.Run(SendLoop, cancellationToken);

            Listen();
        }

        /// <summary>
        /// Peeks the receive buffer for data.
        /// </summary>
        /// <param name="length">Amount of data to be peeked from the receive buffer</param>
        /// <returns>ReadOnlySpan pointing to the data inside the receive buffer</returns>
        public virtual ReadOnlySpan<byte> Peek(int length)
        {
            return new ReadOnlySpan<byte>(ReceiveBuffer, 0, length);
        }

        /// <summary>
        /// Reads data from the receive buffer and moves remaining data to the front of the receive buffer
        /// </summary>
        /// <param name="length">Amount of data to be read</param>
        /// <returns>Byte array containing read data</returns>
        public virtual byte[] Read(int length)
        {
            // Set the read length to be the smaller of available data in the buffer or the length provided
            var _length = Math.Min(_dataLength, length);

            var readBuffer = new byte[_length];

            // copy the data from the ReceiveBuffer into a message buffer
            Buffer.BlockCopy(ReceiveBuffer, 0, readBuffer, 0, _length);

            // reduce the available data length
            _dataLength -= length;

            // move the remaining data into the front of the buffer
            Buffer.BlockCopy(ReceiveBuffer, length, ReceiveBuffer, 0, _dataLength);

            return readBuffer;
        }

        /// <summary>
        /// Adds a message to the send queue 
        /// </summary>
        /// <param name="buffer">Message data to be sent</param>
        public virtual void Send(byte[] buffer)
        {

            SendQueue.Enqueue(buffer);
        }

        /// <summary>
        /// Loop through the send queue and send any available data as long as the connection hasn't been disconnected
        /// </summary>
        /// <returns>Nothing</returns>
        public virtual async Task SendLoop()
        {
            while (true)
            {
                while (!SendQueue.IsEmpty)
                {
                    if (!_disconnectSignaled)
                    {
                        byte[] sendBuffer;

                        if (SendQueue.TryDequeue(out sendBuffer))
                        {
                            var sentBytes = await Socket.SendAsync(sendBuffer, SocketFlags.None);

                            if (sentBytes > 0)
                                OnDataSent(sentBytes);
                        }
                    }
                }

                await Task.Delay(100);
            }
        }

        /// <summary>
        /// Listen for new data sent by the remote connection as long as the connection hasn't been disconnected
        /// </summary>
        public virtual void Listen()
        {
            if (!_disconnectSignaled)
            {
                // receive the data into the receive buffer @ the current data length (to preserve any partial packets that may remain in the buffer)
                Socket.BeginReceive(ReceiveBuffer, _dataLength, ReceiveBuffer.Length - _dataLength, SocketFlags.None, onReceive, Socket);
            }
        }

        /// <summary>
        /// Receives data from the remote connection and add trigger hooked actions for the data to be processed, then trigger listen again
        /// </summary>
        /// <param name="ar"></param>
        private void onReceive(IAsyncResult ar)
        {
            if (!_disconnectSignaled)
            {
                var receiveBytes = Socket.EndReceive(ar);

                if (receiveBytes > 0)
                {
                    BytesReceived += receiveBytes;

                    // set the available data tracker
                    _dataLength += receiveBytes;

                    OnDataReceived(_dataLength);

                    Listen();
                }
            }
        }

        /// <summary>
        /// Check if the remote connection has disconnected and trigger disconnect events accordingly
        /// </summary>
        /// <returns>Nothing</returns>
        private async Task checkForDisconnect()
        {
            while (true) 
            {
                if (!Connected)
                {
                    onDisconnect();
                }

                await Task.Delay(100);
            }
        }

        /// <summary>
        /// Perform actions related to a connection disconnecting
        /// </summary>
        private void onDisconnect()
        {

            cancellationTokenSource.Cancel();

            if (!_disconnectSignaled)
            {
                OnDisconnected();
            }

            _disconnectSignaled = true;
        }
    }

    /// <summary>
    /// Abstraction of the Connection class that provides encode/decode capabilities for Game Client connections
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

    public interface IConnection
    {
        string RemoteIp { get; }

        int RemotePort { get; }

        bool Connected { get; }

        void Start();

        void Connect(string ip, int port);

        void Disconnect();

        ReadOnlySpan<byte> Peek(int length);

        byte[] Read(int length);

        void Send(byte[] buffer);

        Action<int> OnDataReceived { get; set; }

        Action<int> OnDataSent { get; set; }

        Action OnDisconnected { get; set; }
    }
}