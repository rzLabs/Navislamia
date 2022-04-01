//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Common;
//using System.Data.SqlClient;
//using System.Diagnostics;
//using System.Threading.Tasks;

//using Navislamia.Configuration;


//using Serilog;
//using Serilog.Events;

//namespace Navislamia.Utilities
//{
//    / <summary>
//    / Object used to communicate with an SQL Server instance in a generic and asynchronous manner.
//    / </summary>
//    public class DatabaseObject : IDisposable
//    {    
//        / <summary>
//        / Connection being used by this DatabaseObject
//        / </summary>
//        public SqlConnection Connection;

//        / <summary>
//        / Command text to be proceeded upon execution
//        / </summary>
//        public string CommandText
//        {
//            get => command?.CommandText;
//            set => command.CommandText = value;
//        }

//        / <summary>
//        / Parameters to be processed through CommandText
//        / </summary>
//        public SqlParameterCollection Parameters => command?.Parameters;

//        / <summary>
//        / Determine if this DatabaseObject has an open connection
//        / </summary>
//        public bool Connected => command != null && command.Connection.State == ConnectionState.Open;

//        / <summary>
//        / Basis for all SQL driven interactions within this DatabaseObject
//        / </summary>
//        SqlCommand command;

//        / <summary>
//        / Contruct a DatabaseObject by providing atleast a properly formed connection string. 
//        / Providing a CommandText at this time is optional
//        / </summary>
//        / <param name="connection_string">Properly formed MSSQL connection string</param>
//        / <param name="command_text">Text to be parsed during the SQL transaction of this DatabaseObject</param>
//        public DatabaseObject(string connection_string, string command_text = null)
//        {
//            Connection = new SqlConnection(connection_string);

//            command = new SqlCommand() { Connection = Connection };

//            if (command_text != null)
//                command.CommandText = command_text;
//        }

//        / <summary>
//        / Add a collection of SqlParameter to the SqlCommand
//        / </summary>
//        / <param name="parameters">List of parameters to be added</param>
//        public void AddParameters(List<SqlParameter> parameters)
//        {
//            if (command == null)
//            {
//                LogUtility.MessageBoxAndLog("Cannot add paramaters to unitialized DatabaseObject!", "Parameter Exception", LogEventLevel.Error);

//                return;
//            }

//            command.Parameters.AddRange(parameters.ToArray());
//        }

//        / <summary>
//        / Connect to the target database
//        / </summary>
//        / <returns>True or false based on connection state after connection attempt</returns>
//        public async Task<bool> Connect()
//        {
//            if (Connection == null)
//            {
//                LogUtility.MessageBoxAndLog("Connection invalid!", "Database Connection Exception", LogEventLevel.Error);

//                return false;
//            }

//            if (command == null || command.Connection == null)
//            {
//                LogUtility.MessageBoxAndLog("Command invalid!", "Database Connection Exception", LogEventLevel.Error);

//                return false;
//            }

//            if (command.Connection.State == ConnectionState.Open)
//                return true;

//            await command.Connection.OpenAsync();

//            return command.Connection.State == ConnectionState.Open;
//        }

//        / <summary>
//        / Close the connection to the database
//        / </summary>
//        / <returns>True or false based on connection state after close attempt</returns>
//        public bool Disconnect()
//        {
//            command?.Connection?.Close();

//            return command.Connection.State == ConnectionState.Closed;
//        }

//        / <summary>
//        / Reset the sql connection to a completely fresh state
//        / </summary>
//        / <param name="reconnect">If true, automatically reconnect to the database</param>
//        / <returns>True or false depending if the connection was properly reset.</returns>
//        public async Task<bool> Reset(bool reconnect)
//        {
//            command.Connection.Close();
//            command.Dispose();

//            Connection.Dispose();

//            Connection = new SqlConnection(""); // TODO: need conn string bruh

//            command = new SqlCommand() { Connection = Connection };

//            if (reconnect)
//                await Connect();

//            return command != null && command.Connection != null && (reconnect) ? (command.Connection.State == ConnectionState.Open) ? true : false : false;
//        }

//        / <summary>
//        / Execute an asynchronous request to the database for to select a value
//        / </summary>
//        / <typeparam name="T">Type to return selected value as</typeparam>
//        / <returns>Value (if exists) as provided T or default(T)</returns>
//        public async Task<T> ExecuteScalar<T>()
//        {
//            if (!Connected)
//            {
//                LogUtility.MessageBoxAndLog($"Cannot execute against a closed connection!", "Execute Scalar Exception", LogEventLevel.Error);

//                return (T)Convert.ChangeType(-99, typeof(T));
//            }

//            var result = await command.ExecuteScalarAsync();

//            return (result != null) ? (T)Convert.ChangeType(result, typeof(T)) : default(T);
//        }

//        / <summary>
//        / Execute an asynchronous query against the database
//        / </summary>
//        / <returns>Rows affected by CommandText</returns>
//        public async Task<int> ExecuteNonQuery()
//        {
//            if (!Connected)
//            {
//                LogUtility.MessageBoxAndLog($"Cannot execute against a closed connection!", "Execute Scalar Exception", LogEventLevel.Error);

//                return -99;
//            }

//            return await command.ExecuteNonQueryAsync();
//        }

//        / <summary>
//        / Execute an asynchronous database reader against the databast to collect table contents.
//        / </summary>
//        / <returns></returns>
//        public async Task<SqlDataReader> ExecuteReader()
//        {
//            if (!Connected)
//            {
//                LogUtility.MessageBoxAndLog($"Cannot execute against a closed connection!", "Execute Scalar Exception", LogEventLevel.Error);

//                return null;
//            }

//            return await command.ExecuteReaderAsync();
//        }

//        / <summary>
//        / Properly dispose of this DatabaseObject
//        / </summary>
//        public void Dispose()
//        {
//            if (command.Connection.State == ConnectionState.Open)
//                command.Connection.Dispose();

//            command.Dispose();
//        }
//    }

   
//}
