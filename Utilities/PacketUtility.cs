using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Configuration;
using Notification;

//using Serilog;

namespace Utilities
{
    public class PacketUtility
    {
        INotificationService notificationSVC;

        public void DumpToConsole(byte[] data, int count = 0)
        {
            string dataStr = StringExt.ByteArrayToString(data, count);

            // TODO:
            //Log.Debug("\nPacket Dump:\n\n{dump}", dataStr);
        }
    }
}
