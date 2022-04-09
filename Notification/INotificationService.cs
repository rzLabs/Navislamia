using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Serilog;
using Serilog.Events;

using Spectre.Console;

namespace Notification
{
    public interface INotificationService
    {
        public void WriteString(string message, LogEventLevel level = LogEventLevel.Verbose);

        public void WriteMarkup(string message, LogEventLevel level = LogEventLevel.Verbose);

        public void WriteDebug(string message);

        public void WriteWarning(string message);

        public void WriteSuccess(string[] messages, bool tabbedIndent);

        public void WriteException(Exception exception, LogEventLevel level = LogEventLevel.Error);

    }
}
