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
        INotificationService notificationSVC;

        string _input;

        public string Input 
        {
            get => _input;
            set => _input = value;
        }

        public CommandModule(INotificationService notificaftionService)
        {
            notificationSVC = notificaftionService;
        }

        // TODO: register all CommandModule.Commands and Implementations here!
        public int Init(ITypeRegistrar registrar)
        {
            registrar.Register(typeof(IAbout), typeof(AboutPrinter));

            commandApp = new CommandApp(registrar);

            commandApp.Configure(config =>
            {
                config.AddCommand<About>("about").WithDescription("Print information about the Navislamia Framework").WithExample(new string[] { "about" });
            });

            return 0;
        }

        public int Wait()
        {
            string idleMessage = "[orange3]Idle... [/][italic orange3](Press ` to enter a command)\n[/]";

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

                    AnsiConsole.Write(new Markup(idleMessage));
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    notificationSVC.WriteString("Navislamia shutting down...");
                    break;
                }
            }

            return 0;
        }

        public int Execute(string[] args)
        {
            return commandApp.Run(args);
        }
    }
}
