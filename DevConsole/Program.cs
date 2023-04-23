using System;
using System.IO;
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


namespace DevConsole;


class Program 
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        await host.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, configuration) =>
            {
                configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                configuration.AddEnvironmentVariables("NAVISLAMIA_");
            })
            .ConfigureServices((context, services) =>
            {
                //Options
                services.AddHostedService<Application>();
                services.Configure<DatabaseOptions>(context.Configuration.GetSection("Database"));
                services.Configure<NetworkOptions>(context.Configuration.GetSection("Network"));
                services.Configure<ScriptOptions>(context.Configuration.GetSection("Script"));
                services.Configure<MapOptions>(context.Configuration.GetSection("Map"));
                services.Configure<WorldOptions>(context.Configuration.GetSection("World"));
                services.Configure<PlayerOptions>(context.Configuration.GetSection("Player"));

                // Services
                services.AddTransient<ICommandService, CommandModule>();
                services.AddTransient<IDatabaseService, DatabaseModule>();
                services.AddTransient<IWorldService, WorldModule>();
                services.AddTransient<IScriptingService, ScriptModule>();
                services.AddTransient<IMapService, MapModule>();
                services.AddTransient<INetworkService, NetworkModule>();
                services.AddTransient<IGameService, GameModule>();
            })
            .ConfigureLogging((context, logging) => {
                logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                logging.AddConsole();
            })
            .UseConsoleLifetime();
    }
}

