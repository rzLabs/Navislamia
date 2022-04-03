using Configuration;
using Notification;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Database.GameContent
{
    public class DbStringResource : DbContent, IDbContent<Dictionary<int, KeyValuePair<string, string>>>
    {
        Dictionary<int, KeyValuePair<string, string>> strings = new Dictionary<int, KeyValuePair<string, string>>();

        public DbStringResource(IConfigurationService configurationService, IDatabaseService databaseService, INotificationService notificationService) : base(configurationService, databaseService, notificationService)
        {
            TableName = "StringResource";
            SelectStatement = DbSelectStatements.StringResource;
        }

        public int Count => strings?.Count ?? 0;

        public Dictionary<int, KeyValuePair<string, string>> Load()
        {
            try
            {
                using (DatabaseObject dbObj = new DatabaseObject(ConfigurationService, NotificationService))
                {
                    if (!dbObj.Connect(DbConnectionType.World))
                    {
                        // TODO: log to notification module

                        return null;
                    }

                    dbObj.CommandText = SelectStatement;

                    using (SqlDataReader sqlRdr = dbObj.ExecuteReader().Result)
                        while (sqlRdr.Read())
                        {
                            int code = Convert.ToInt32(sqlRdr["code"]);
                            string name = sqlRdr["name"].ToString();
                            string value = sqlRdr["value"].ToString();

                            strings.Add(code, new KeyValuePair<string, string>(name, value));
                        }
                }
            }
            catch { }

            return strings;
        }

        public int Save()
        {
            throw new NotImplementedException();
        }
    }
}
