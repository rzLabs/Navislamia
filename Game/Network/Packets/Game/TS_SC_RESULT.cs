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
    public struct TS_SC_RESULT
    {
        public ushort RequestMsgID { get; set; }

        public ushort Result { get; set; }

        public int Value { get; set; }

        public TS_SC_RESULT(ushort id, ushort result, int value = 0)
        {
            RequestMsgID = id;
            Result = result;
            Value = value;
        }
    }
}
