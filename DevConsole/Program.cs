using System.IO;
using System.Threading.Tasks;
using Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Navislamia.Command;
using Navislamia.Command.Commands;
using Navislamia.Configuration.Options;
using Navislamia.Database;
using Navislamia.Game;
using Navislamia.Maps;
using Navislamia.Network;
using Navislamia.Network.Entities;
using Navislamia.Notification;
using Navislamia.Scripting;
using Navislamia.World;


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
                var env = context.HostingEnvironment.EnvironmentName;
                configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                configuration.AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true);
                configuration.AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                services.AddHostedService<Application>();

                //Options
                services.Configure<LogOptions>(context.Configuration.GetSection("Logs"));
                services.Configure<DatabaseOptions>(context.Configuration.GetSection("Database"));
                services.Configure<WorldOptions>(context.Configuration.GetSection("Database:World"));
                services.Configure<PlayerOptions>(context.Configuration.GetSection("Database:Player"));
                services.Configure<NetworkOptions>(context.Configuration.GetSection("Network"));
                services.Configure<AuthOptions>(context.Configuration.GetSection("Network:Auth"));
                services.Configure<GameOptions>(context.Configuration.GetSection("Network:Game"));
                services.Configure<UploadOptions>(context.Configuration.GetSection("Network:Upload"));
                services.Configure<ScriptOptions>(context.Configuration.GetSection("Script"));
                services.Configure<MapOptions>(context.Configuration.GetSection("Map"));
                services.Configure<ServerOptions>(context.Configuration.GetSection("Server"));

                // Services
                services.AddSingleton<ICommandModule, CommandModule>();
                services.AddSingleton<IDatabaseModule, DatabaseModule>();
                services.AddSingleton<IWorldModule, WorldModule>();
                services.AddSingleton<IScriptingModule, ScriptModule>();
                services.AddSingleton<IMapModule, MapModule>();
                services.AddSingleton<INetworkModule, NetworkModule>();
                services.AddSingleton<IGameModule, GameModule>();
                services.AddSingleton<INotificationModule, NotificationModule>();
                services.AddSingleton<IClientService<AuthClientEntity>, ClientService<AuthClientEntity>>();
                services.AddSingleton<IClientService<UploadClientEntity>, ClientService<UploadClientEntity>>();
            })
            .ConfigureLogging((context, logging) => {
                logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                logging.AddConsole();
            })
            .UseConsoleLifetime();
    }
}

