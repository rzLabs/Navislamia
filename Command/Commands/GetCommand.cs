using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configuration;
using Notification;
using Spectre.Console.Cli;

namespace Navislamia.Command.Commands
{
    public class GetCommand : Command<GetCommand.Settings>
    {
        IConfigurationService configSVC;
        INotificationService notificationSVC;

        public GetCommand(IConfigurationService configurationService, INotificationService notificationService)
        {
            configSVC = configurationService;
            notificationSVC = notificationService;
        }

        public class Settings : CommandSettings
        {
            [CommandArgument(0, "[Category]")]
            public string Category { get; set; }

            [CommandArgument(1, "[Key]")]
            public string Key { get; set; }

        }

        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            var config = configSVC.Get(settings.Key, settings.Category);

            if (config is null)
                return 1;

            notificationSVC.WriteMarkup($"[bold orange3]{settings.Key}[/] : [bold yellow]{config}[/]");

            return 0;
        }
    }
}
