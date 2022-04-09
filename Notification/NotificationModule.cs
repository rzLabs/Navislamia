using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Serilog;
using Serilog.Events;

using Spectre.Console;

namespace Notification
{
    public class NotificationModule : INotificationService
    {
        public NotificationModule()
        {
            Log.Logger = new LoggerConfiguration()
                            //.MinimumLevel.ControlledBy(LogLevel)
                            .WriteTo.File(".\\Logs\\Navislamia-Log-.txt", rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}")
                            .CreateLogger();
        }

        public void WriteMarkup(string message, LogEventLevel level = LogEventLevel.Verbose)
        {
            AnsiConsole.Write(new Markup($"{message}\n"));
            Log.Write(level, message);
        }

        public void WriteString(string message, LogEventLevel level = LogEventLevel.Verbose)
        {
            AnsiConsole.Write($"{message}\n");
            Log.Write(level, message);
        }

        public void WriteSuccess(string[] messages, bool tabbedIndent)
        {
            string str = $"[bold green]{messages[0]}\n[/]";

            for (int i = 1; i < messages.Length; i++)
            {
                if (tabbedIndent)
                    str += "\t";

                str += $"{messages[i]}\n";
            }

            WriteMarkup(str);
        }

        public void WriteException(Exception exception, LogEventLevel level = LogEventLevel.Error)
        {
            AnsiConsole.WriteException(exception);
            Log.Write(level, exception, "An exception has occured!");
        }

        public void WriteDebug(string message)
        {
#if DEBUG
            WriteMarkup($"[bold orange3]{message}[/]\n");
            Log.Debug(message);
#endif
        }

        public void WriteWarning(string message)
        {
            WriteMarkup($"[bold yellow]{message}[/]\n");
            Log.Warning(message);
        }
    }
}
