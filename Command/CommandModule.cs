using System;
using Spectre.Console;
using Spectre.Console.Cli;
using Navislamia.Command.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Navislamia.Command
{
    public class CommandModule : ICommandModule
    {
        CommandApp commandApp;
        IConfiguration _configService;
        ILogger<CommandModule> _logger;

        string _input;

        public string Input
        {
            get => _input;
            set => _input = value;
        }

        public CommandModule(ILogger<CommandModule> logger, IConfiguration configurationService)
        {
            _logger = logger;
            _configService = configurationService;
        }

        // TODO: register all CommandModule.Commands and Implementations here!
        public int Init()
        {
            var registrations = new ServiceCollection();
            var registrar = new TypeRegistrar(registrations);
            
            registrar.RegisterInstance(typeof(IConfiguration), _configService);

            registrar.Register(typeof(IAbout), typeof(AboutPrinter));
            registrar.Register(typeof(IConfigurationGetter), typeof(ConfigurationGetter));
            registrar.Register(typeof(IConfigurationSetter), typeof(ConfigurationSetter));

            commandApp = new CommandApp(registrar);

            commandApp.Configure(config =>
            {
                config.AddCommand<About>("about").WithDescription("Print information about the Navislamia Framework").WithExample(new string[] { "about" });
                config.AddCommand<GetConfiguration>("get").WithDescription("Get stored configuration value by category and name").WithExample(new[] { "get", "server", "name" });
                config.AddCommand<SetConfiguration>("set").WithDescription("Set stored configuration value by category and name").WithExample(new[] { "set", "server", "name", "navislamia" });
            });

            return 0;
        }

        public int Wait()
        {
            string idleMessage = "Idle... (Press ` to enter a command)\n";

            _logger.LogInformation(idleMessage);

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

                if (key.KeyChar == '`')
                {
                    string input = AnsiConsole.Ask<string>("~ Command: ");

                    string[] inputBlocks = input.TrimEnd().Split(new char[] { ' ' });

                    if (Execute(inputBlocks) == 1)
                        break;

                    AnsiConsole.Write(new Markup(idleMessage));
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    _logger.LogInformation("Navislamia shutting down...");
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
