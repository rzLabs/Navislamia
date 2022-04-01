using Autofac;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Configuration;

using Console = Colorful.Console;

using Serilog;

using Serilog.Events;
using System.Drawing;

namespace Notification
{
    public class NotificationModule : Autofac.Module, INotificationService
    {
        IConfigurationService configSVC;
        ILogger filelog;
        ILogger consolelog;

        public NotificationModule() { }

        public NotificationModule(IConfigurationService configurationService)
        {
            configSVC = configurationService;

            filelog = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(".\\Logs\\log-.txt", rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            consolelog = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}", theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code)
                .CreateLogger();
        }

        public void WriteConsoleLog(string message, object[] args = null, LogEventLevel level = LogEventLevel.Verbose) 
        {
            filelog.Write(level, message, args);
            consolelog.Write(level, message, args);
        }

        public void WriteConsole(string message, object[] args = null, LogEventLevel level = LogEventLevel.Verbose) => consolelog.Write(level, message, args);

        public void WriteColorConsole(string message, object[] args, Color[] colors) { }

        public void WriteLog(string message, object[] args = null, LogEventLevel level = LogEventLevel.Verbose) => filelog.Write(level, message, args);

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
