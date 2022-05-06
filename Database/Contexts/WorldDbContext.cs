using Configuration;
using Navislamia.Database.Interfaces;
using Notification;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Navislamia.Database.Contexts
{
    public class WorldDbContext : IDbContext
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
    }
}