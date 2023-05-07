using System;
using Serilog.Events;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Navislamia.Notification
{
    public interface INotificationModule
    {
        public void Write(IRenderable renderable, LogEventLevel level = LogEventLevel.Verbose);

        public void WriteNewLine() => AnsiConsole.Write("\n");

        public void WriteString(string message, LogEventLevel level = LogEventLevel.Verbose);

        public void WriteMarkup(string message, LogEventLevel level = LogEventLevel.Verbose);

        public void WriteDebug(string message);

        public void WriteDebug(string[] messages, bool indented = true);

        public void WriteWarning(string message);

        public void WriteError(string message);

        public void WriteSuccess(string message);

        public void WriteSuccess(string[] messages, bool tabbedIndent);

        public void WriteException(Exception exception, LogEventLevel level = LogEventLevel.Error);

    }
}
