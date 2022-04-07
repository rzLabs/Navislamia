using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notification;

using Spectre.Console;
using Spectre.Console.Cli;

namespace Navislamia.Command.Commands
{
    public class WaitCommand : Command<WaitCommand.Settings>
    {
        ICommandService cmdSVC;
        INotificationService notificationSVC;

        public WaitCommand(ICommandService commandService, INotificationService notificationService)
        {
            cmdSVC = commandService;
            notificationSVC = notificationService;
        }

        public class Settings : CommandSettings { }

        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            cmdSVC.Input = AnsiConsole.Ask<string>("Waiting for input...");

            return 0;
        }
    }
}
