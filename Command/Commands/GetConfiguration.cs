using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using cli = Spectre.Console.Cli;

using Navislamia.Notification;

namespace Navislamia.Command.Commands
{
    public interface IConfigurationGetter
    {
        public string Get(params string[] keys);
    }

    public sealed class ConfigurationGetter : IConfigurationGetter
    {
        private readonly IConfiguration _configuratiion;

        public ConfigurationGetter(IConfiguration configuration)
        {
            _configuratiion = configuration;
        }

        public string Get(params string[] keys)
        {
            var value = _configuratiion.GetSection(string.Join(":", keys)).Value;

            return value;
        }
    }

    public sealed class GetConfiguration : cli.Command<GetConfiguration.Settings>
    {
        INotificationModule _notificationModule;
        IConfigurationGetter _configurationGetter;

        public GetConfiguration(INotificationModule notificationModule, IConfigurationGetter configurationGetter)
        {
            _notificationModule = notificationModule;
            _configurationGetter = configurationGetter;
        }

        public class Settings : cli.CommandSettings
        {
            [cli.CommandArgument(0, "[Keys]")]
            public string[] Keys { get; set; }

        }

        public override int Execute([NotNull] cli.CommandContext context, [NotNull] Settings settings)
        {
            var value = _configurationGetter.Get(settings.Keys);

            if (value is null)
                _notificationModule.WriteMarkup($"\n[red]Could not locate configuration[/]\n");
            else
                _notificationModule.WriteMarkup($"\n[orange3]{string.Join(" ", settings.Keys)}[/] : [yellow]{value}[/]\n\n");

            return 0;
        }
    }
}
