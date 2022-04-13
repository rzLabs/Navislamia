using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configuration;
using Navislamia.Command.Interfaces;
using Notification;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Navislamia.Command.Commands
{
    public sealed class ConfigurationGetter : IGetter
    {
        IConfigurationService configSVC;

        public ConfigurationGetter(IConfigurationService configurationService)
        {
            configSVC = configurationService;
        }

        public dynamic Get(string key, string parent) => configSVC.Get(key, parent);
    }

    public sealed class GetConfiguration : Command<GetConfiguration.Settings>
    {
        INotificationService notificationSVC;
        IGetter _getter;

        public GetConfiguration(INotificationService notificationService, IGetter getter)
        {
            notificationSVC = notificationService;
            _getter = getter;
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
            var config = _getter.Get(settings.Key, settings.Category);

            if (config is null)
                return 1;

            notificationSVC.WriteMarkup($"\n[bold orange3]{settings.Key}[/] : [bold yellow]{config}[/]\n");

            return 0;
        }
    }
}
