using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Enums
{
    public enum ClientPackets : ushort
    {
        TM_SC_RESULT = 0,

        TM_CS_VERSION = 50,

        TS_CS_CHARACTER_LIST = 2001,

        TM_CS_ACCOUNT_WITH_AUTH = 2005,

        TS_CS_REPORT = 8000,

        TM_NONE = 9999
    }
}
