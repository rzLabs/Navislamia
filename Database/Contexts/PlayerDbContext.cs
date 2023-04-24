using Configuration;
using Navislamia.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Database.Contexts
{
    public class PlayerDbContext : IDbContext
    {
        private IConfigurationService _configSVC;
        private string _connString;

        public PlayerDbContext(IConfigurationService configSVC)
        {
            _configSVC = configSVC;
            buildConnString();
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connString);

        private void buildConnString()
        {
            string ip = _configSVC.Get<string>("player.ip", "database", "127.0.0.1");
            string name = _configSVC.Get<string>("player.name", "database", "Telecaster");
            string user = _configSVC.Get<string>("player.user", "database", "sa");
            string pass = _configSVC.Get<string>("player.user.pass", "database", "");
            bool trusted = _configSVC.Get<bool>("player.trusted_connection", "database", false);

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
