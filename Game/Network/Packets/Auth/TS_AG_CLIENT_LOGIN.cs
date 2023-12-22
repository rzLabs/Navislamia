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
    public struct TS_AG_CLIENT_LOGIN
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 61)]
        public string Account;

        public int AccountID;

        public ushort Result;

        public byte PcBangMode;

        public int EventCode;

        public int Age;

        public float ContinuousPlayTime;

        public float ContinuousLogoutTime;
    }
}
