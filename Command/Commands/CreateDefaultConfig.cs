using Configuration;
using Notification;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Navislamia.Command.Interfaces;

namespace Navislamia.Command.Commands
{
    public class ConfigurationCreator : IConfigurationCreator
    {
        IConfigurationService configSVC;

        public ConfigurationCreator(IConfigurationService configurationService) => configSVC = configurationService;

        public void Create(string path = null) => configSVC.CreateDefault(path);
    }

    public class CreateDefaultConfig : Command<CreateDefaultConfig.Settings>
    {
        IConfigurationService configSVC;
        INotificationService notificationSVC;
        IConfigurationCreator _creator;

        public CreateDefaultConfig(IConfigurationService configurationService, INotificationService notificationService, IConfigurationCreator creator)
        {
            configSVC = configurationService;
            notificationSVC = notificationService;
            _creator = creator;
        }

        public class Settings : CommandSettings
        {
            [CommandArgument(0, "[Path]")]
            public string Path { get; set; }
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            try
            {
                _creator.Create(settings.Path);

                notificationSVC.WriteSuccess(new string[] { "\nDefault Configuration.json written!", $"[bold green]{settings.Path}[/]" }, true);
            }
            catch (Exception ex)
            {
                notificationSVC.WriteException(ex);

                return 1;
            }

            return 0;
        }
    }
}
