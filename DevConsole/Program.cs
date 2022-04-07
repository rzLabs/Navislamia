using System;
using System.Diagnostics;
using System.IO;

using Autofac;

using Navislamia.Game;
using Scripting;
using Configuration;
using Maps;
using Network;
using Notification;

using Serilog;
using Serilog.Events;
using System.Resources;
using DevConsole.Properties;
using System.Collections.Generic;
using Navislamia.Data;
using Database;

namespace DevConsole
{
    class Program
    {
        static IContainer depsContainer;
        public static IConfigurationService ConfigurationService;
        public static INotificationService NotificationService;
        public static IGameService GameService;

        static void Main(string[] args)
        {
            CommandUtility cmdUtil = new CommandUtility(ConfigurationService);

            try
            {
                var builder = new ContainerBuilder();
                builder.Register<IConfigurationService>(c=> new ConfigurationModule());
                builder.Register<INotificationService>(c=>new NotificationModule());
                builder.Register<IGameService>(c => new GameModule(ConfigurationService, NotificationService));

                depsContainer = builder.Build();

                NotificationService = depsContainer.Resolve<INotificationService>();

                NotificationService.WriteString($"{Resources.arcadia}\n\nNavislamia starting...\n");

                ConfigurationService = depsContainer.Resolve<IConfigurationService>();
                ConfigurationService.Load();

                NotificationService.WriteMarkup($"[green]{ConfigurationService.TotalCount}[/] settings loaded from [bold yellow]Configuration.json[/]\n", LogEventLevel.Debug);

                GameService = depsContainer.Resolve<IGameService>();
                
                if (GameService.Start("", 4502, 100) == 1)
                {
                    NotificationService.WriteMarkup("[bold red]Failed to start the game service![/]");

                    return;
                }    

                NotificationService.WriteString("Successfully started and subscribed to the game service!", LogEventLevel.Information);
            }
            catch (Exception ex)
            {
                if (NotificationService == null)
                    Log.Error($"An exception occured that kept the NotificationService from being started!\nMessage: {ex.Message}\nStack-Trace: {ex.StackTrace}");
                else
                    NotificationService.WriteException(ex);

                return;
            }

            cmdUtil.Wait("Waiting for input...");
        }

        static object waitForInput() => Console.ReadLine();
    }
}
