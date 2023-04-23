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
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;
using Navislamia.World;

namespace Navislamia.Database.Contexts
{
    public class WorldDbContext : IDbContext
    {
        private readonly WorldOptions _worldOptions;
        private string _connString;
        
        public WorldDbContext(IOptions<WorldOptions> worldOptions)
        {
            _worldOptions = worldOptions.Value;
            BuildConnString();
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connString);

        private void BuildConnString()
        {
            string ip = _worldOptions.Ip;
            string dbName = _worldOptions.DbName;
            string user = _worldOptions.User;
            string pass = _worldOptions.Password;
            bool trusted = _worldOptions.IsTrustedConnection;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Server={0};Database={1};", ip, dbName);
            if (trusted)
                sb.Append("Trusted_Connection=true;");
            else
                sb.AppendFormat("User ID={0};Password={1}", user, pass);

            _connString = sb.ToString();
        }
    }
}