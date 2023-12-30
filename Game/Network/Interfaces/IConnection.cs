using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.Network.Interfaces
{
    public interface IConnection
    {
        string RemoteIp { get; }

        int RemotePort { get; }

        bool Connected { get; }

        void Start();

        void Connect(string ip, int port);

        void Disconnect();

        ReadOnlySpan<byte> Peek(int length);

        byte[] Read(int input);

        void Send(byte[] buffer);

        Action<int> OnDataReceived { get; set; }

        Action<int> OnDataSent { get; set; }

        Action OnDisconnected { get; set; }
    }
}
