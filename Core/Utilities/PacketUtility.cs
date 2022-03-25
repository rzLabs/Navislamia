using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Navislamia.Configuration;

using Serilog;

namespace Navislamia.Utilities
{
    public static class PacketUtility
    {
        public static void DumpToConsole(byte[] data)
        {
            int count = ConfigurationManager.Instance.Get<int>("packet.dump_max_length", "Logs");

            string dataStr = StringExt.ByteArrayToString(data, count);

            Log.Debug("\nPacket Dump:\n\n{dump}", dataStr);
        }
    }
}
