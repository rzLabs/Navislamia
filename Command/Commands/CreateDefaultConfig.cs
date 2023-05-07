using Navislamia.Notification;
using Spectre.Console.Cli;
using System;
using System.Diagnostics.CodeAnalysis;
using Navislamia.Command.Interfaces;

namespace Navislamia.Command.Commands
{

    public class CreateDefaultConfig : Command<CreateDefaultConfig.Settings>
    {
        INotificationModule notificationSVC;
        IConfigurationCreator _creator;

        public CreateDefaultConfig(INotificationModule notificationModule, IConfigurationCreator creator)
        {
            notificationSVC = notificationModule;
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
