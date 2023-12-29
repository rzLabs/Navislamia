using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

using Network.Security;
using System.Collections.Concurrent;
using System.Threading;

using Navislamia.Game.Network.Interfaces;

namespace Navislamia.Game.Network
{
    /// <summary>
    /// Socket wrapper for handling Auth/Upload/Game connections
    /// </summary>
    public class Connection : IConnection
    {
        internal Socket Socket;

        internal bool DisconnectSignaled;

        internal volatile int BytesSent;
        internal volatile int BytesReceived;

        private int _dataLength = 0;
        internal byte[] ReceiveBuffer = new byte[32768];

        internal ConcurrentQueue<byte[]> SendQueue = new();

        internal CancellationTokenSource CancellationTokenSource = new();
        internal CancellationToken CancellationToken;

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
                if (Socket?.LocalEndPoint is IPEndPoint localEp)
                    return localEp.Address.ToString();

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
                if (Socket?.LocalEndPoint is IPEndPoint localEp)
                    return localEp.Port;

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
                if ( Socket?.RemoteEndPoint is IPEndPoint remoteEp)
                    return remoteEp.Address.ToString();

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
                if (Socket?.RemoteEndPoint is IPEndPoint remoteEp)
                    return remoteEp.Port;

                return -1;
            }
        }

        /// <summary>
        /// Checks if the wrapped socket is connected via polling
        /// </summary>
        public bool Connected => Socket.Connected && (!Socket.Poll(1000, SelectMode.SelectRead) || Socket.Available != 0) && !DisconnectSignaled;

        /// <summary>
        /// Starts internal processes like checking for disconnect, sending messages and begins listening
        /// </summary>
        public virtual void Start()
        {
            CancellationToken = CancellationTokenSource.Token;

            Task.Run(CheckForDisconnect, CancellationToken);
            Task.Run(SendLoop, CancellationToken);

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
        protected virtual async Task SendLoop()
        {
            while (true)
            {
                while (!SendQueue.IsEmpty)
                {
                    if (DisconnectSignaled)
                    {
                        continue;
                    }

                    if (!SendQueue.TryDequeue(out var sendBuffer))
                    {
                        continue;
                    }
                    
                    var sentBytes = await Socket.SendAsync(sendBuffer, SocketFlags.None);

                    if (sentBytes > 0)
                        OnDataSent(sentBytes);
                }

                await Task.Delay(100, CancellationToken);
            }
        }

        /// <summary>
        /// Listen for new data sent by the remote connection as long as the connection hasn't been disconnected
        /// </summary>
        protected virtual void Listen()
        {
            if (!DisconnectSignaled)
            {
                // receive the data into the receive buffer @ the current data length (to preserve any partial packets that may remain in the buffer)
                Socket.BeginReceive(ReceiveBuffer, _dataLength, ReceiveBuffer.Length - _dataLength, SocketFlags.None, OnReceive, Socket);
            }
        }

        /// <summary>
        /// Receives data from the remote connection and add trigger hooked actions for the data to be processed, then trigger listen again
        /// </summary>
        /// <param name="ar"></param>
        private void OnReceive(IAsyncResult ar)
        {
            if (DisconnectSignaled)
            {
                return;
            }
            
            try
            {
                var receiveBytes = Socket?.EndReceive(ar) ?? 0;

                if (receiveBytes <= 0)
                {
                    return;
                }

                BytesReceived += receiveBytes;

                // set the available data tracker
                _dataLength += receiveBytes;

                OnDataReceived(_dataLength);

                Listen();
            }
            catch (SocketException sockEx)
            {
                // likely a disconnection by the client
                // this will be caught on the next poll of the client, so lets be silent here
            }
        }

        /// <summary>
        /// Check if the remote connection has disconnected and trigger disconnect events accordingly
        /// </summary>
        /// <returns>Nothing</returns>
        private async Task CheckForDisconnect()
        {
            while (true) 
            {
                if (!Connected)
                {
                    OnDisconnect();
                }

                await Task.Delay(100, CancellationToken);
            }
        }

        /// <summary>
        /// Perform actions related to a connection disconnecting
        /// </summary>
        private void OnDisconnect()
        {
            CancellationTokenSource.Cancel();

            if (!DisconnectSignaled)
            {
                OnDisconnected();
            }

            DisconnectSignaled = true;
        }
    }
}