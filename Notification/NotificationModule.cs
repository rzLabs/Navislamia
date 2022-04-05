using Autofac;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Configuration;

using Serilog;

using Serilog.Events;

using Spectre.Console;

namespace Notification
{
    public class NotificationModule : Autofac.Module, INotificationService
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

        public void WriteSuccess(string message, LogEventLevel level = LogEventLevel.Verbose)
        {
            throw new NotImplementedException();
        }

        public void WriteException(Exception exception, LogEventLevel level = LogEventLevel.Error)
        {
            AnsiConsole.WriteException(exception);
            Log.Write(level, exception, "An exception has occured!");
        }

        protected override void Load(ContainerBuilder builder)
        {
            var configServiceTypes = Directory.EnumerateFiles(Environment.CurrentDirectory)
                .Where(filename => filename.Contains("Modules") && filename.EndsWith("Notification.dll"))
                .Select(filepath => Assembly.LoadFrom(filepath))
                .SelectMany(assembly => assembly.GetTypes()
                .Where(type => typeof(INotificationService).IsAssignableFrom(type) && type.IsClass));

            foreach (var configServiceType in configServiceTypes)
                builder.RegisterType(configServiceType).As<INotificationService>();
        }
    }
}
