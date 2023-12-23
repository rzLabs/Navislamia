using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Network.Packets;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TS_CS_REPORT
{
    public ushort ReportLen;

}
