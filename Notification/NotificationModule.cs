using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Serilog;
using Serilog.Events;

using Spectre.Console;
using Spectre.Console.Rendering;

namespace Navislamia.Notification
{
    public class NotificationModule : INotificationService
    {
        public NotificationModule()
        {
            Log.Logger = new LoggerConfiguration()
                            //.MinimumLevel.ControlledBy(LogLevel) // TODO this should be controlled via a configuration setting
                            .MinimumLevel.Verbose()
                            .WriteTo.File(".\\Logs\\Navislamia-Log-.txt", rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}")
                            .CreateLogger();
        }

        public void WriteString(string message, LogEventLevel level = LogEventLevel.Verbose)
        {
            AnsiConsole.Write($"{message}\n");
            Log.Write(level, message);
        }

        public void WriteMarkup(string message, LogEventLevel level = LogEventLevel.Verbose)
        {
            AnsiConsole.Write(new Markup($"{message}\n"));
            Log.Write(level, message);
        }

        public void WriteDebug(string message)
        {
#if DEBUG
            WriteMarkup($"[bold orange3]{message}[/]\n");
            Log.Debug(message);
#endif
        }

        public void WriteDebug(string[] messages, bool indented = true)
        {
#if DEBUG
            string str = $"[bold orange3]{messages[0]}\n";

            for (int i = 1; i < messages.Length; i++)
            {
                if (indented)
                    str += "\t";

                str += $"{messages[i]}\n";
            }

            WriteMarkup($"{str}[/]");
#endif
        }

        public void WriteWarning(string message)
        {
            WriteMarkup($"[bold yellow]{message}[/]\n");
            Log.Warning(message);
        }

        public void WriteError(string message)
        {
            WriteMarkup($"[bold red]{message}[/]\n");
            Log.Error(message);
        }

        public void WriteSuccess(string message) => WriteSuccess(new string[] { message }, false);

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
            AnsiConsole.Write("\n");
            Log.Write(level, exception, "An exception has occured!");
        }

        public void Write(IRenderable renderable, LogEventLevel level = LogEventLevel.Verbose)
        {
            AnsiConsole.Write(renderable);
            Log.Write(level, renderable.ToString());
        }
    }
}
