using Navislamia.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets.Game
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TM_CS_ACCOUNT_WITH_AUTH
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 61)]
        public string Account;

        public long OneTimePassword;

    }
}
