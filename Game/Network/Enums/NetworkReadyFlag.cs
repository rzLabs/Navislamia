using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.Network.Enums
{
    [Flags]
    public enum NetworkReadiness
    {
        NotReady = 0,
        AuthReady = 1,
        UploadReady = 2,

        AuthServerReady = AuthReady | UploadReady
    }
}
