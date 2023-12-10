using Navislamia.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets.Auth
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TS_GA_CLIENT_LOGIN
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 61)]
        public string Account;

        public long OneTimeKey;

        public TS_GA_CLIENT_LOGIN(string account, long oneTimeKey)
        {
            Account = account;
            OneTimeKey = oneTimeKey;

        }
    }
}
