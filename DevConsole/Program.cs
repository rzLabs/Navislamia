using System.Threading.Tasks;
using Configuration;
using DevConsole.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Navislamia.Command;
using Navislamia.Configuration.Options;
using Navislamia.Game;
using Navislamia.Game.Contexts;
using Navislamia.Game.Network;
using Navislamia.Game.Network.Entities;
using Navislamia.Game.Repositories;
using Navislamia.Game.Services;
using Navislamia.Notification;
using Serilog;

namespace DevConsole;

public class Program 
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        var scopeFactory = host.Services.GetService<IServiceScopeFactory>();
        using (var scope = scopeFactory.CreateScope())
        {
            var arcadia = scope.ServiceProvider.GetService<ArcadiaContext>();
            var telecaster = scope.ServiceProvider.GetService<TelecasterContext>();
            await arcadia.Database.MigrateAsync();
            await telecaster.Database.MigrateAsync();
            
            Log.Logger.Information("Applied Arcadia migrations: {Migrations}", await arcadia.Database.GetAppliedMigrationsAsync());
            Log.Logger.Information("Applied Telecaster migrations: {Migrations}", await telecaster.Database.GetAppliedMigrationsAsync());
        }

        await host.RunAsync();
        await Log.CloseAndFlushAsync();
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
                ConfigureOptions(services, context);
                ConfigureServices(services);
                ConfigureDataAccess(services);
            })
            .UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration).Enrich.FromLogContext();
            });

    }

    private static void ConfigureOptions(IServiceCollection services, HostBuilderContext context)
    {
        services.Configure<LogOptions>(context.Configuration.GetSection("Logs"));
        services.Configure<DatabaseOptions>(context.Configuration.GetSection("Database"));
        services.Configure<NetworkOptions>(context.Configuration.GetSection("Network"));
        services.Configure<AuthOptions>(context.Configuration.GetSection("Network:Auth"));
        services.Configure<GameOptions>(context.Configuration.GetSection("Network:Game"));
        services.Configure<UploadOptions>(context.Configuration.GetSection("Network:Upload"));
        services.Configure<ScriptOptions>(context.Configuration.GetSection("Script"));
        services.Configure<MapOptions>(context.Configuration.GetSection("Map"));
        services.Configure<ServerOptions>(context.Configuration.GetSection("Server"));
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ICommandModule, CommandModule>();
        services.AddSingleton<INetworkModule, NetworkModule>();
        services.AddSingleton<IGameModule, GameModule>();
        services.AddSingleton<INotificationModule, NotificationModule>();
        services.AddSingleton<IClientService<AuthClientEntity>, ClientService<AuthClientEntity>>();
        services.AddSingleton<IClientService<UploadClientEntity>, ClientService<UploadClientEntity>>();
        services.AddSingleton<IWorldRepository, WorldRepository>();
        services.AddSingleton<ICharacterService, CharacterService>();
        services.AddSingleton<ICharacterRepository, CharacterRepository>();
    }
    
    private static void ConfigureDataAccess(IServiceCollection services)
    {
        services.AddDbContextPool<ArcadiaContext>((serviceProvider, builder) =>
        {
            var config = serviceProvider.GetService<IConfiguration>();
            var dbOptions = config.GetSection("Database").Get<DatabaseOptions>();
            dbOptions.InitialCatalog = "Arcadia";
                
            // https://learn.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
            builder
                .UseNpgsql(dbOptions.ConnectionString(), options => options.EnableRetryOnFailure())
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });
                
        services.AddDbContextPool<TelecasterContext>((serviceProvider, builder) =>
        {
            var config = serviceProvider.GetService<IConfiguration>();
            var dbOptions = config.GetSection("Database").Get<DatabaseOptions>();
            dbOptions.InitialCatalog = "Telecaster";

            // https://learn.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
            builder
                .UseNpgsql(dbOptions.ConnectionString(), options => options.EnableRetryOnFailure())
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

        });
    }
}

