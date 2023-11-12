using Microsoft.Extensions.Configuration;
using Navislamia.Notification;
using Newtonsoft.Json.Linq;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Command.Commands
{
    public interface IConfigurationSetter
    {
        public void Set(string key, string value);
    }

    public sealed class ConfigurationSetter : IConfigurationSetter
    {
        private readonly IConfiguration _configuratiion;

        public ConfigurationSetter(IConfiguration configuration)
        {
            _configuratiion = configuration;
        }

        public void Set(string key, string value) 
        {
            var configSection = _configuratiion.GetSection(key.Replace(".", ":"));

            if (configSection is null)
            {
                // TODO: log me

                return;
            }

            configSection.Value = value;
        }
    }

    public sealed class SetConfiguration : Command<SetConfiguration.Settings>
    {
        readonly INotificationModule _notificationModule;
        readonly IConfigurationSetter _configurationSaver;

        public SetConfiguration(INotificationModule notificationModule, IConfigurationSetter configurationSaver)
        {
            _notificationModule = notificationModule;
            _configurationSaver = configurationSaver;
        }

        public class Settings : CommandSettings
        {
            [CommandArgument(0, "[Key]")]
            public string Key { get; set; }

            [CommandArgument(1, "[Value]")]
            public string Value { get; set; }

               
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            _configurationSaver.Set(settings.Key, settings.Value);

            _notificationModule.WriteMarkup($"\nConfiguration [orange3]{settings.Key}[/] value updated to: [yellow]{settings.Value}[/]\n\n");

            return 0;
        }
    }
}
