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

namespace Database
{
    public class DatabaseModule : Autofac.Module, IDatabaseService, IDisposable
    {
        ///  <summary>
        //  Connection being used by this DatabaseObject
        ///  </summary>
        public SqlConnection Connection;

        ///  <summary>
        //  Command text to be proceeded upon execution
        ///  </summary>
        public string CommandText
        {
            get => command?.CommandText;
            set => command.CommandText = value;
        }

        ///  <summary>
        //  Determine if this DatabaseObject has an open connection
        ///  </summary>
        public bool Connected => command != null && command.Connection.State == ConnectionState.Open;

        public string WorldConnectionString
        {
            get
            {
                string ip = configSVC.Get<string>("world.ip", "Database", "127.0.0.1");
                string name = configSVC.Get<string>("world.name", "Database", "Arcadia");
                string user = configSVC.Get<string>("world.user", "Database", "sa");
                string pass = configSVC.Get<string>("world.user.pass", "Database", "");
                bool trusted = configSVC.Get<bool>("world.trusted_connection", "Database", false);

                string connStr = $"Server={ip};Database={name};";

                if (trusted)
                    connStr += "Trusted_Connection=true;";
                else
                    connStr += $"User ID={user};Password={pass}";

                return connStr;
            }
        }

        public string PlayerConnectionString
        {
            get
            {
                string ip = configSVC.Get<string>("player.ip", "Database", "127.0.0.1");
                string name = configSVC.Get<string>("player.name", "Database", "Telecaster");
                string user = configSVC.Get<string>("player.user", "Database", "sa");
                string pass = configSVC.Get<string>("player.user.pass", "Database", "");
                bool trusted = configSVC.Get<bool>("player.trusted_connection", "Database", false);

                string connStr = $"Server={ip};Database={name};";

                if (trusted)
                    connStr += "Trusted_Connection=true;";
                else
                    connStr += $"User ID={user};Password={pass}";

                return connStr;
            }
        }

        ///  <summary>
        //  Basis for all SQL driven interactions within this DatabaseObject
        ///  </summary>
        SqlCommand command;

        IConfigurationService configSVC;
        INotificationService notificationSVC;

        public DatabaseModule() { }

        ///  <summary>
        //  Contruct a DatabaseObject by providing atleast a properly formed connection string. 
        //  Providing a CommandText at this time is optional
        ///  </summary>
        ///  <param name = "connection_string" > Properly formed MSSQL connection string</param>
        ///  <param name = "command_text" > Text to be parsed during the SQL transaction of this DatabaseObject</param>
        public DatabaseModule(IConfigurationService configurationService, INotificationService notificationService)
        {

            configSVC = configurationService;
            notificationSVC = notificationService;
        }

        ///  <summary>
        //  Add a collection of SqlParameter to the SqlCommand
        ///  </summary>
        ///  <param name = "parameters" > List of parameters to be added</param>
        public void AddParameters(List<SqlParameter> parameters)
        {
            if (command == null)
            {
                notificationSVC.WriteConsoleLog("Cannot add paramaters to unitialized DatabaseObject!", null, LogEventLevel.Error);

                return;
            }

            command.Parameters.AddRange(parameters.ToArray());
        }

        ///  <summary>
        ///   Connect to the target database
        ///  </summary>
        ///  <returns>True or false based on connection state after connection attempt</returns>
        public bool Connect(DbConnectionType type)
        {
            if (Connection != null)
                Connection.Dispose();

            Connection = new SqlConnection((type == DbConnectionType.World) ? WorldConnectionString : PlayerConnectionString);

            if (command == null || command.Connection == null)
            {
                notificationSVC.WriteConsoleLog("Command invalid!", null, LogEventLevel.Error);

                return false;
            }

            if (command.Connection.State == ConnectionState.Open)
                return true;

            command.Connection.Open();

            return command.Connection.State == ConnectionState.Open;
        }

        ///  <summary>
        //  Close the connection to the database
        ///  </summary>
        ///  <returns>True or false based on connection state after close attempt</returns>
        public bool Disconnect()
        {
            command?.Connection?.Close();

            return command.Connection.State == ConnectionState.Closed;
        }

        ///  <summary>
        //  Execute an asynchronous request to the database for to select a value
        ///  </summary>
        ///  <typeparam name = "T" > Type to return selected value as</typeparam>
        ///  <returns>Value(if exists) as provided T or default(T)</returns>
        public async Task<T> ExecuteScalar<T>()
        {
            if (!Connected)
            {
                notificationSVC.WriteConsoleLog("Cannot execute against a closed connection!", null, LogEventLevel.Error);

                return (T)Convert.ChangeType(-99, typeof(T));
            }

            var result = await command.ExecuteScalarAsync();

            return (result != null) ? (T)Convert.ChangeType(result, typeof(T)) : default(T);
        }

        ///  <summary>
        //  Execute an asynchronous query against the database
        ///  </summary>
        ///  <returns>Rows affected by CommandText</returns>
        public async Task<int> ExecuteNonQuery()
        {
            if (!Connected)
            {
                notificationSVC.WriteConsoleLog($"Cannot execute against a closed connection!", null, LogEventLevel.Error);

                return -99;
            }

            return await command.ExecuteNonQueryAsync();
        }

        ///  <summary>
        //  Execute an asynchronous database reader against the databast to collect table contents.
        ///  </summary>
        ///  <returns></returns>
        public async Task<SqlDataReader> ExecuteReader()
        {
            if (!Connected)
            {
                notificationSVC.WriteConsoleLog($"Cannot execute against a closed connection!", null, LogEventLevel.Error);

                return null;
            }

            return await command.ExecuteReaderAsync();
        }

        ///  <summary>
        //  Properly dispose of this DatabaseObject
        ///  </summary>
        public void Dispose()
        {
            if (command.Connection.State == ConnectionState.Open)
                command.Connection.Dispose();

            command.Dispose();
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
