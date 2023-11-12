using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using Spectre.Console.Cli;

using Navislamia.Notification;

namespace Navislamia.Command.Commands
{
    public interface IConfigurationGetter
    {
        public string Get(string key);
    }

    public sealed class ConfigurationGetter : IConfigurationGetter
    {
        private readonly IConfiguration _configuratiion;

        public ConfigurationGetter(IConfiguration configuration)
        {
            _configuratiion = configuration;
        }

<<<<<<< Updated upstream
        public string Get(params string[] keys)
=======
        public string Get(string key) // TODO: must validate keys actually exist
>>>>>>> Stashed changes
        {
            var value = _configuratiion.GetSection(key.Replace(".", ":")).Value;

            return value;
        }
    }

    public sealed class GetConfiguration : Command<GetConfiguration.Settings>
    {
        INotificationModule _notificationModule;
        IConfigurationGetter _configurationGetter;

        public GetConfiguration(INotificationModule notificationModule, IConfigurationGetter configurationGetter)
        {
            _notificationModule = notificationModule;
            _configurationGetter = configurationGetter;
        }

        public class Settings : CommandSettings
        {
            [CommandArgument(0, "[Keys]")]
            public string Key { get; set; }

        }

        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            var value = _configurationGetter.Get(settings.Key);

            if (value is null)
                _notificationModule.WriteMarkup($"\n[red]Could not locate configuration[/]\n");
            else
                _notificationModule.WriteMarkup($"\n[orange3]{settings.Key}[/] : [yellow]{value}[/]\n\n");

            return 0;
        }
    }
}
