using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public interface INetworkService
    {
        public int ConnectToAuth();

        public int StartListener();
    }
}
