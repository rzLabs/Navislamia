using System.Threading.Tasks;
using Configuration;
using Configuration.Options;
using Database;
using Maps;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Navislamia.Command;
using Navislamia.Configuration.Options;
using Navislamia.Game;
using Navislamia.World;
using Network;
using Scripting;


namespace DevConsole
{
    internal static class Program
    {
        public static Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var app = ActivatorUtilities.CreateInstance<Application>(host.Services);
            app.Run();
            return Task.CompletedTask;
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration((context, configuration) =>
                    {
                        configuration.Sources.Clear();
                        configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                        configuration.AddEnvironmentVariables();
                        configuration.AddCommandLine(args);
                    })
                    .ConfigureServices((context, services) =>
                    {
                        // Options
                        services.AddOptions<DatabaseOptions>();
                        services.AddOptions<NetworkOptions>();
                        services.AddOptions<ScriptOptions>();
                        services.AddOptions<MapOptions>();
                        services.AddOptions<WorldOptions>();
                        services.AddOptions<PlayerOptions>();

                        // Services
                        services.AddSingleton<ICommandService, CommandModule>();
                        services.AddSingleton<IDatabaseService, DatabaseModule>();
                        services.AddSingleton<IWorldService, WorldModule>();
                        services.AddSingleton<IScriptingService, ScriptModule>();
                        services.AddSingleton<IMapService, MapModule>();
                        services.AddSingleton<INetworkService, NetworkModule>();
                        services.AddSingleton<IGameService, GameModule>();
                    })
                    .ConfigureLogging((context, logging) => {
                        logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                        logging.AddConsole();
                    });
        }
    }
}
