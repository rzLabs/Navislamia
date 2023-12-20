using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Network.Security;
using System.Collections.Concurrent;
using System.Threading;
using static Navislamia.Game.Network.Entities.Connection;

namespace Navislamia.Game.Network.Entities
{
    public class Connection : IConnection
    {
        internal Socket Socket;

        internal bool _disconnectSignaled = false;

        internal volatile int BytesSent;
        internal volatile int BytesReceived;

        internal byte[] ReceiveBuffer = new byte[32768];

        internal ConcurrentQueue<byte[]> SendQueue = new ConcurrentQueue<byte[]>();

        internal CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        internal CancellationToken cancellationToken;

        public Action<int> OnDataSent { get; set; }   
        public Action<byte[]> OnDataReceived { get; set; }
        public Action OnDisconnected { get; set; }

        public Connection(Socket socket)
        {
            Socket = socket;
        }

        public void Connect(string ip, int port)
        {
            try
            {
                Socket.Connect(ip, port);

            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        public void Disconnect()
        {
            if (!Connected)
                return;

            Socket.Disconnect(false);
        }

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

        public bool Connected
        {
            get
            {
                if (Socket.Poll(1000, SelectMode.SelectRead) && Socket.Available == 0)
                {
                    if (!_disconnectSignaled)
                    {
                        onDisconnect();
                    }

                    return false;
                }

                return true;
            }
        }

        public virtual void Start()
        {
            cancellationToken = cancellationTokenSource.Token;

            Task.Run(SendLoop);
            Task.Run(ListenLoop);
        }

        public virtual void Send(byte[] buffer)
        {
            try
            {
                SendQueue.Enqueue(buffer);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        public virtual async Task SendLoop()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                while (!SendQueue.IsEmpty)
                {
                    try
                    {
                        if (Connected)
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
                    catch (Exception ex)
                    {
                        OnException(ex);
                    }
                }

                await Task.Delay(100);
            }
        }

        public virtual async Task ListenLoop()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (Connected)
                {
                    var receiveBytes = await Socket.ReceiveAsync(ReceiveBuffer, SocketFlags.None);

                    if (receiveBytes > 0)
                    {
                        var receiveBuffer = new byte[receiveBytes];

                        Buffer.BlockCopy(ReceiveBuffer, 0, receiveBuffer, 0, receiveBytes);

                        OnDataReceived(receiveBuffer);
                    }
                }

                await Task.Delay(100);
            }
        }

        internal void OnException(Exception ex)
        {
            Log.Logger.Error(ex.Message);
        }

        private void onDisconnect()
        {
            _disconnectSignaled = true;

            cancellationTokenSource.Cancel();

            OnDisconnected();
        }

        [Flags]
        public enum ConnectionStateFlag
        {
            Connected = 0,
            Writable = 1,
            Readable = 2,
            Send = 4,
            Receive = 8,
            Error = 16,
            Ready = Connected | Writable | Readable
        }
    }

    public class CipherConnection : Connection, IConnection
    {
        XRC4Cipher _sendCipher = new XRC4Cipher();
        XRC4Cipher _receiveCipher = new XRC4Cipher();

        public CipherConnection(Socket socket, string cipherKey) : base(socket)
        {
            _sendCipher.SetKey(cipherKey);
            _receiveCipher.SetKey(cipherKey);
        }

        public override void Start()
        {
            Task.Run(SendLoop);
            Task.Run(ListenLoop);
        }

        public override void Send(byte[] buffer)
        {
            try
            {
                _sendCipher.Encode(buffer, buffer, buffer.Length);

                SendQueue.Enqueue(buffer);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        public override async Task ListenLoop()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (Connected)
                {
                    try
                    {
                        var receiveBytes = Socket.Receive(ReceiveBuffer, SocketFlags.None);

                        if (receiveBytes > 0)
                        {
                            var receiveBuffer = new byte[receiveBytes];

                            Buffer.BlockCopy(ReceiveBuffer, 0, receiveBuffer, 0, receiveBytes);

                            _receiveCipher.Decode(receiveBuffer, receiveBuffer, receiveBytes);

                            OnDataReceived(receiveBuffer);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

                await Task.Delay(100);
            }
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

        void Send(byte[] buffer);

        Action<byte[]> OnDataReceived { get; set; }

        Action<int> OnDataSent { get; set; }

        Action OnDisconnected { get; set; }
    }
}
