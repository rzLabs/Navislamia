using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Serilog;
using Serilog.Events;

namespace Notification
{
    public interface INotificationService
    {
        public void WriteConsoleLog(string message, object[] args = null, LogEventLevel level = LogEventLevel.Verbose);

        public void WriteConsole(string message, object[] args = null, LogEventLevel level = LogEventLevel.Verbose);

        public void WriteColorConsole(string message, object[] args, Color[] colors);

        public void WriteLog(string message, object[] args = null, LogEventLevel level = LogEventLevel.Verbose);
    }
}
