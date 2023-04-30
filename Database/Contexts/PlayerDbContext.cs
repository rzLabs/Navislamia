using Navislamia.Database.Interfaces;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Microsoft.Extensions.Options;
using Navislamia.Configuration.Options;

namespace Navislamia.Database.Contexts
{
    public class PlayerDbContext : IDbContext
    {
        private readonly PlayerOptions _playerOptions;
        private string _connString;

        public PlayerDbContext(IOptions<PlayerOptions> playerOptions)
        {
            _playerOptions = playerOptions.Value;
            BuildConnString();
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connString);

        private void BuildConnString()
        {
            string ip = _playerOptions.Ip;
            string name = _playerOptions.DbName;
            string user = _playerOptions.User;
            string pass = _playerOptions.Password;
            bool trusted = _playerOptions.IsTrustedConnection;

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
