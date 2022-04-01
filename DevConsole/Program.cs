using System;
using System.Diagnostics;
using System.IO;

using Game;


using Autofac;

using Game;
using Scripting;
using Configuration;
using Network;
using Notification;

using Serilog;
using Serilog.Events;

namespace DevConsole
{
    class Program
    {
        static IContainer depsContainer;
        public static IConfigurationService ConfigurationService;
        public static INotificationService NotificationService;
        public static INetworkService NetworkService;
        public static IScriptingService ScriptingService;
        public static IGameService GameService;

        static void Main(string[] args)
        {
            CommandUtility cmdUtil = new CommandUtility(ConfigurationService);

            try
            {
                var builder = new ContainerBuilder();
                builder.RegisterModule<ConfigurationModule>();
                builder.RegisterModule<NotificationModule>();
                builder.RegisterModule<NetworkModule>();
                builder.RegisterModule<ScriptModule>();
                builder.RegisterModule<GameModule>();

                depsContainer = builder.Build();

                ConfigurationService = depsContainer.Resolve<IConfigurationService>();
                ConfigurationService.Load();

                configureSerilog();

                NotificationService = depsContainer.Resolve<INotificationService>(new NamedParameter("configurationService", ConfigurationService));

                NotificationService.WriteConsoleLog("Navislamia starting...");
                NotificationService.WriteConsole("Starting configuration service...");
                NotificationService.WriteConsoleLog("{0} settings loaded from Configuration.json", new object[] { ConfigurationService.TotalCount }, LogEventLevel.Debug);

                NotificationService.WriteConsoleLog("Starting script service...", null, LogEventLevel.Debug);

                ScriptingService = depsContainer.Resolve<IScriptingService>(new NamedParameter("configurationService", ConfigurationService), new NamedParameter("notificationService", NotificationService));
                ScriptingService.Init();

                NotificationService.WriteConsoleLog("Successfully started and subscribed to the script service!", null, LogEventLevel.Debug);

                NotificationService.WriteConsoleLog("Starting network service...", null, LogEventLevel.Debug);

                NetworkService = depsContainer.Resolve<INetworkService>(new NamedParameter("configurationService", ConfigurationService), new NamedParameter("notficationService", NotificationService));
                NetworkService.Start();

                NotificationService.WriteConsoleLog("Successfully started and subscribed to the network service!", null, LogEventLevel.Debug);

                GameService = depsContainer.Resolve<IGameService>(new NamedParameter("configurationService", ConfigurationService), new NamedParameter("scriptService", ScriptingService), new NamedParameter("networkService", NetworkService), new NamedParameter("notificationService", NotificationService));
                GameService.Start("", 4502, 100);

                NotificationService.WriteConsoleLog("Successfully started and subscribed to the game service!", null, LogEventLevel.Information);
            }
            catch (Exception ex)
            {
                if (NotificationService == null)
                    Log.Error($"An exception occured that kept the NotificationService from being started!\nMessage: {ex.Message}\nStack-Trace: {ex.StackTrace}");
                else
                    NotificationService.WriteConsoleLog("An exception has occured while attempting to start!\nMessage: {0}\nStack-Trace: {1}", new object[] { ex.Message, ex.StackTrace }, LogEventLevel.Error);

                return;
            }

            cmdUtil.Wait("Waiting for input...");
        }

        static void configureSerilog() // We do this just incase we fail to subscribe to our NotificationService
        {
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Verbose()
                            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}", theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code)
                            .WriteTo.File(".\\Logs\\log-.txt", rollingInterval: RollingInterval.Day, outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}")
                            .CreateLogger();
        }

        static object waitForInput() => Console.ReadLine();
    }
}
