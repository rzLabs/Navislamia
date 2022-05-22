using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Enums
{
    public enum UploadPackets : ushort
    {
        TS_SU_LOGIN = 50001,
        TS_US_LOGIN_RESULT = 50002
    }
}
