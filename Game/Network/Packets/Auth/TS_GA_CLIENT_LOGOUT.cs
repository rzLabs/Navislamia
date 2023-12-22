using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.Network.Packets.Auth
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TS_GA_CLIENT_LOGOUT
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 61)]
        public string Account;
        public uint ContinousPlayTime;

        public TS_GA_CLIENT_LOGOUT(string account, uint continousPlayTime)
        {
            Account = account;
            ContinousPlayTime = continousPlayTime;
        }
    }
}
