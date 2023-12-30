using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

using Navislamia.Game.Network.Interfaces;

namespace Navislamia.Game.Network;

/// <summary>
/// Socket wrapper for handling Auth/Upload/Game connections
/// </summary>
public class Connection : IConnection
{
    private readonly Socket _socket;

    private bool _disconnectSignaled;

    internal volatile int BytesSent;
    internal volatile int BytesReceived;

    private int _dataLength = 0;
    internal readonly byte[] ReceiveBuffer = new byte[32768];

    internal readonly ConcurrentQueue<byte[]> SendQueue = new();

    internal readonly CancellationTokenSource CancellationTokenSource = new();
    private CancellationToken _cancellationToken;

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
        _socket = socket;
    }

    /// <summary>
    /// Connects to the remote host at provided ip and port
    /// </summary>
    /// <param name="ip">Remote host ip address</param>
    /// <param name="port">Remote host port</param>
    public void Connect(string ip, int port)
    {
        _socket.Connect(ip, port);
    }

    /// <summary>
    /// Gracefully disconnects the connection
    /// </summary>
    public void Disconnect()
    {
        if (!Connected)
        {
            return;
        }

        _socket.Disconnect(false);
    }

    /// <summary>
    /// Local IP Address of the wrapped socket
    /// </summary>
    public string LocalIp
    {
        get
        {
            if (_socket?.LocalEndPoint is IPEndPoint localEp)
            {
                return localEp.Address.ToString();
            }

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
            if (_socket?.LocalEndPoint is IPEndPoint localEp)
            {
                return localEp.Port;
            }

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
            if (_socket?.RemoteEndPoint is IPEndPoint remoteEp)
            {
                return remoteEp.Address.ToString();
            }

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
            if (_socket?.RemoteEndPoint is IPEndPoint remoteEp)
            {
                return remoteEp.Port;
            }

            return -1;
        }
    }

    /// <summary>
    /// Checks if the wrapped socket is connected via polling
    /// </summary>
    public bool Connected => _socket.Connected &&
                             (!_socket.Poll(1000, SelectMode.SelectRead) || _socket.Available != 0) &&
                             !_disconnectSignaled;

    /// <summary>
    /// Starts internal processes like checking for disconnect, sending messages and begins listening
    /// </summary>
    public virtual void Start()
    {
        _cancellationToken = CancellationTokenSource.Token;

        Task.Run(CheckForDisconnect, _cancellationToken);
        Task.Run(SendLoop, _cancellationToken);

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
    /// <param name="input">Amount of data to be read</param>
    /// <returns>Byte array containing read data</returns>
    public virtual byte[] Read(int input)
    {
        // Set the read input to be the smaller of available data in the buffer or the input provided
        var length = Math.Min(_dataLength, input);
        var readBuffer = new byte[length];

        // copy the data from the ReceiveBuffer into a message buffer
        Buffer.BlockCopy(ReceiveBuffer, 0, readBuffer, 0, length);

        // reduce the available data input
        _dataLength -= input;

        // move the remaining data into the front of the buffer
        Buffer.BlockCopy(ReceiveBuffer, input, ReceiveBuffer, 0, _dataLength);

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
    protected async Task SendLoop()
    {
        while (true)
        {
            while (!SendQueue.IsEmpty)
            {
                if (_disconnectSignaled)
                {
                    continue;
                }

                if (!SendQueue.TryDequeue(out var sendBuffer))
                {
                    continue;
                }
                    
                var sentBytes = await _socket.SendAsync(sendBuffer, SocketFlags.None);
                if (sentBytes > 0)
                {
                    OnDataSent(sentBytes);
                }
            }

            await Task.Delay(100, _cancellationToken);
        }
    }

    /// <summary>
    /// Listen for new data sent by the remote connection as long as the connection hasn't been disconnected
    /// </summary>
    protected virtual void Listen()
    {
        if (!_disconnectSignaled)
        {
            // receive the data into the receive buffer @ the current data input (to preserve any partial packets that may remain in the buffer)
            _socket.BeginReceive(ReceiveBuffer, _dataLength, ReceiveBuffer.Length - _dataLength, SocketFlags.None, OnReceive, _socket);
        }
    }

    /// <summary>
    /// Receives data from the remote connection and add trigger hooked actions for the data to be processed, then trigger listen again
    /// </summary>
    /// <param name="ar"></param>
    private void OnReceive(IAsyncResult ar)
    {
        if (_disconnectSignaled)
        {
            return;
        }
            
        try
        {
            var receiveBytes = _socket?.EndReceive(ar) ?? 0;
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

            await Task.Delay(100, _cancellationToken);
        }
    }

    /// <summary>
    /// Perform actions related to a connection disconnecting
    /// </summary>
    private void OnDisconnect()
    {
        CancellationTokenSource.Cancel();

        if (!_disconnectSignaled)
        {
            OnDisconnected();
        }

        _disconnectSignaled = true;
    }
}