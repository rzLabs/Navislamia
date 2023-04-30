using System.Threading.Tasks;
using Configuration;
using Configuration.Options;
using Maps;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Navislamia.Command;
using Navislamia.Configuration.Options;
using Navislamia.Database;
using Navislamia.Game;
using Navislamia.Network;
using Navislamia.Notification;
using Navislamia.World;
using Network;
using Scripting;


namespace DevConsole;

public class Program 
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
                services.Configure<WorldOptions>(context.Configuration.GetSection("Database:World"));
                services.Configure<PlayerOptions>(context.Configuration.GetSection("Database:Player"));
                services.Configure<ServerOptions>(context.Configuration.GetSection("Server"));
                services.Configure<LogOptions>(context.Configuration.GetSection("Log"));


                // Services
                services.AddSingleton<ICommandService, CommandModule>();
                services.AddSingleton<IDatabaseService, DatabaseModule>();
                services.AddSingleton<IWorldService, WorldModule>();
                services.AddSingleton<IScriptingService, ScriptModule>();
                services.AddSingleton<IMapService, MapModule>();
                services.AddSingleton<INetworkService, NetworkModule>();
                services.AddSingleton<IGameService, GameModule>();
                services.AddSingleton<INotificationService, NotificationModule>();



            })
            .ConfigureLogging((context, logging) => {
                logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                logging.AddConsole();
            })
            .UseConsoleLifetime();
    }
}

