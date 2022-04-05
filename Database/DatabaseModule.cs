using System;
using System.Collections.Generic;
using System.Text;

using System.Data.SqlClient;
using System.Data;

using System.Threading.Tasks;

using System.IO;
using System.Linq;
using System.Reflection;

using Configuration;
using Notification;
using Serilog.Events;
using Autofac;
using Database.GameContent;

namespace Database
{
    public class DatabaseModule : Autofac.Module, IDatabaseService
    {
        IConfigurationService configSVC;
        INotificationService notificationSVC;
        WorldDbContext worldDbContext;

        public DatabaseModule() { }

        public DatabaseModule(IConfigurationService configurationService, INotificationService notificationService)
        {
            configSVC = configurationService;
            notificationSVC = notificationService;
        }

        public void Init() // TODO: arcadia table loading logic should occur here
        {
            worldDbContext = new WorldDbContext(configSVC);

            var stringRepo = new StringResourceRespository(notificationSVC, worldDbContext);

            var strings = stringRepo.GetStrings();

            notificationSVC.WriteMarkup($"[italic green]{strings.Count()}[/] strings loaded!",  LogEventLevel.Debug);
        }

        protected override void Load(ContainerBuilder builder)
        {
            var serviceTypes = Directory.EnumerateFiles(Environment.CurrentDirectory)
                .Where(filename => filename.Contains("Modules") && filename.EndsWith("Database.dll"))
                .Select(filepath => Assembly.LoadFrom(filepath))
                .SelectMany(assembly => assembly.GetTypes()
                .Where(type => typeof(IDatabaseService).IsAssignableFrom(type) && type.IsClass));

            foreach (var serviceType in serviceTypes)
                builder.RegisterType(serviceType).As<IDatabaseService>();
        }
    }
}
