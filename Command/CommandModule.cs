using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configuration;
using Notification;

using Spectre.Console;
using Spectre.Console.Cli;


using Navislamia.Command.Commands;
using Navislamia.Command.Interfaces;

namespace Navislamia.Command
{
    public class CommandModule : ICommandService
    {
        CommandApp commandApp;

        IConfigurationService configSVC;
        INotificationService notificationSVC;

        string _input;

        public string Input 
        {
            get => _input;
            set => _input = value;
        }


        public CommandModule(IConfigurationService configurationService, INotificationService notificaftionService)
        {
            configSVC = configurationService;
            notificationSVC = notificaftionService;
        }

        public int Init(ITypeRegistrar registrar)
        {
            registrar.Register(typeof(IGetter), typeof(ConfigurationGetter));

            // TODO: We need to register our types!
            commandApp = new CommandApp(registrar);

            commandApp.Configure(config =>
            {
                config.AddCommand<GetCommand>("get").WithAlias("GetConfig").WithDescription("Print configuration value").WithExample(new string[] { "get", "io.ip" });

            });

            return 0;
        }

        public int Wait()
        {
            string idleMessage = "[orange3]Idle... [/][italic orange3](Press ` to enter a command)\n[/]"; // TODO: need to set this message everytime a new notification write call is completed as-well!

            notificationSVC.WriteMarkup(idleMessage);

            while (true)
            {
                while (true)
                {
                    if (Console.KeyAvailable)
                        break;
                    else
                        System.Threading.Thread.Sleep(100);
                }

                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Oem3)
                {
                    string input = AnsiConsole.Ask<string>("~ Command: ");

                    string[] inputBlocks = input.TrimEnd().Split(new char[] { ' ' });

                    if (Execute(inputBlocks) == 1)
                        break;
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    notificationSVC.WriteString("Navislamia shutting down...");
                    break;
                }

                AnsiConsole.Write(new Markup(idleMessage));
            }

            return 0;
        }

        public int Execute(string[] args)
        {
            return commandApp.Run(args);
        }
    }
}
