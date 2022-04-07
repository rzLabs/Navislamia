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
using System.ComponentModel;

namespace Navislamia.Command
{
    public interface ICommandService
    {
        public object Input { get; set; }

        public void Wait();
    }
    public class CommandModule : ICommandService
    {
        CommandApp<WaitCommand> commandApp;
        
        

        IConfigurationService configSVC;
        INotificationService notificationSVC;

        object _input;

        public object Input 
        {
            get => _input;
            set => _input = value;
        }


        public CommandModule(IContainer container, IConfigurationService configurationService, INotificationService notificaftionService)
        {
            configSVC = configurationService;
            notificationSVC = notificaftionService;

            // TODO: We need to register our types!
            commandApp = new CommandApp<WaitCommand>();

            commandApp.Configure(config =>
            {
                config.AddCommand<WaitCommand>("wait").WithDescription("Wait for user input").WithExample(new string[] { "wait" });
                config.AddCommand<GetCommand>("get").WithAlias("GetConfig").WithDescription("Print configuration value").WithExample(new string[] { "get", "io.ip" });
            });

        }

        public void Wait()
        {
            commandApp.Run(new[] { "" });
        }
    }
}
