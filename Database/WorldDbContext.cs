using Autofac;
using Configuration;
using Notification;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Database
{
    public class WorldDbContext : Autofac.Module
    {
        private IConfigurationService _configSVC;
        private string _connString;
        
        public WorldDbContext(IConfigurationService configSVC)
        {
            _configSVC = configSVC;
            buildConnString();
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connString);

        private void buildConnString()
        {
            string ip = _configSVC.Get<string>("world.ip", "Database", "127.0.0.1");
            string name = _configSVC.Get<string>("world.name", "Database", "Arcadia");
            string user = _configSVC.Get<string>("world.user", "Database", "sa");
            string pass = _configSVC.Get<string>("world.user.pass", "Database", "");
            bool trusted = _configSVC.Get<bool>("world.trusted_connection", "Database", false);

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Server={0};Database={1};", ip, name);
            if (trusted)
                sb.Append("Trusted_Connection=true;");
            else
                sb.AppendFormat("User ID={0};Password={1}", user, pass);

            _connString = sb.ToString();
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
