using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Configuration;
using Notification;

namespace Database.GameContent
{
    public interface IDbContent<T>
    {
        public int Count { get; }

        public T Load();

        public int Save();
    }

    public class DbContent
    {
        public string TableName = string.Empty;

        public string SelectStatement = string.Empty;

        public DbContent(IConfigurationService configurationService, IDatabaseService databaseService, INotificationService notificationService)
        {
            ConfigurationService = configurationService;
            DatabaseService = databaseService;
            NotificationService = notificationService;
        }

        public IConfigurationService ConfigurationService;
        public IDatabaseService DatabaseService;
        public INotificationService NotificationService;
    }

    public class DbSelectStatements
    {
        public const string StringResource = "select * from dbo.StringResource";
    }
}
