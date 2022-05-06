using Navislamia.Database.Contexts;
using Navislamia.Database.Entities;
using Notification;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Navislamia.Database.Repositories
{
    public class StringResourceRespository : ResourceRepository<List<StringResource>>
    {
        private readonly INotificationService _notificationSVC;

        public List<StringResource> Strings { get; }

        public StringResourceRespository(INotificationService notificationService, WorldDbContext context) : base("StringResource", context)
        {
            _notificationSVC = notificationService;
            Strings = new List<StringResource>();
        }

        public override List<StringResource> Get()
        {        
            var queryString = "SELECT * FROM dbo.StringResource";
            SqlConnection conn = (SqlConnection)DbContext.CreateConnection();
            var cmd = new SqlCommand(queryString, conn);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                StringResource strResource = new StringResource();
                strResource.Code = Convert.ToInt32(reader["code"]);
                strResource.Name = reader["name"].ToString();
                strResource.Value = reader["value"].ToString();
                Strings.Add(strResource);
            }

            conn.Close();

            return Strings;
        }
    }
}
