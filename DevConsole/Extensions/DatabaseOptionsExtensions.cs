using Navislamia.Configuration.Options;
using Npgsql;

namespace DevConsole.Extensions;

public static class DatabaseOptionsExtensions
{
    public static string ConnectionString(this DatabaseOptions options)
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = options.DataSource,
            Database = options.InitialCatalog,
            Port = options.Port,
            Username = options.User,
            Password = options.Password,
            SslMode = SslMode.Prefer,
            MaxPoolSize = options.MaxPoolSize ?? 5,
            CommandTimeout = options.CommandTimeout
        };
        
        return connectionStringBuilder.ConnectionString;
    }
}