using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Enums
{
    public enum AuthPackets : ushort
    {
        TS_GA_LOGIN = 20001,
        TS_AG_LOGIN_RESULT = 20002,

        TS_GA_CLIENT_LOGIN = 20010,
        TS_AG_CLIENT_LOGIN = 20011
    }
}
