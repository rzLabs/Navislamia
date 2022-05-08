using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public interface INetworkService
    {
        public int Initialize();

        public bool Ready { get; }

        public int StartListener();
    }
}
