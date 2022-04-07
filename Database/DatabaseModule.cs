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
using Navislamia.Database.GameContent;
using Navislamia.Data;

namespace Database
{
    public class DatabaseModule : Autofac.Module, IDatabaseService
    {
        IConfigurationService configSVC;
        INotificationService notificationSVC;
        IDataService dataSVC;
        WorldDbContext worldDbContext;

        public DatabaseModule() { }

        public DatabaseModule(IConfigurationService configurationService, INotificationService notificationService, IDataService dataService)
        {
            configSVC = configurationService;
            notificationSVC = notificationService;
            dataSVC = dataService;
        }

        public void Init() // TODO: arcadia table loading logic should occur here
        {
            worldDbContext = new WorldDbContext(configSVC);

            var stringRepo = new StringResourceRespository(notificationSVC, worldDbContext);
            dataSVC.Set<List<StringResource>>("strings", stringRepo.GetStrings());

            int stringCnt = dataSVC.Get<List<StringResource>>("strings").Count;

            notificationSVC.WriteMarkup($"[italic green]{stringCnt}[/] strings loaded!",  LogEventLevel.Debug);
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
