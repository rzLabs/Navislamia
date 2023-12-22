using System.Reflection;
using AutoMapper;
using DevConsole.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MigrateDatabase;
using MigrateDatabase.Mappers;
using MigrateDatabase.MigrationContexts;
using Navislamia.Configuration.Options;
using Navislamia.Game.Contexts;
using Serilog;
using SqlConnectionStringBuilder = System.Data.SqlClient.SqlConnectionStringBuilder;


var path = Assembly.GetEntryAssembly()!.Location;
path = path[..(path.LastIndexOf('\\') + 1)];

var builder = Host.CreateDefaultBuilder(args)
    .UseContentRoot(path)
    .ConfigureAppConfiguration((context, configuration) =>
    {
        var env = context.HostingEnvironment.EnvironmentName;
        configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        configuration.AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true);
        configuration.AddEnvironmentVariables();
        
        
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<Worker>();
        // Automapper
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new ArcadiaResourcesMappingProfile());
        });
        IMapper mapper = mappingConfig.CreateMapper();
        services.AddSingleton(mapper);
        services.Configure<DatabaseOptions>(context.Configuration.GetSection("Database"));
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
        
        services.AddDbContextPool<MssqlArcadiaContext>((serviceProvider, builder) =>
        {
            var config = serviceProvider.GetService<IConfiguration>();
            var dbOptions = config.GetSection("MigrationTool").Get<MigrationToolOptions>();
            var connstring = new SqlConnectionStringBuilder
            {
                DataSource = dbOptions.Ip,
                InitialCatalog = dbOptions.DbName,
                UserID = dbOptions.User,
                Password = dbOptions.Password,
                TrustServerCertificate = true

            };
            // https://learn.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
            builder
                .UseSqlServer(new SqlConnection(connstring.ConnectionString))
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
    })
    .ConfigureLogging((context, logging) => {
        Log.Logger = new LoggerConfiguration().Enrich.FromLogContext()
            .WriteTo.Console().CreateLogger();
        logging.AddConfiguration(context.Configuration.GetSection("Logging"));
        logging.AddConsole();
    })
    .UseConsoleLifetime();

var host = builder.Build();
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
host.Run();