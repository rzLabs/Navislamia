using Database.GameContent;
using Notification;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Database
{
    public class StringResourceRespository
    {
        private readonly INotificationService _notificationSVC;
        private readonly WorldDbContext _dbContext;

        public List<StringResource> Strings { get; }


        public StringResourceRespository(INotificationService notificationService, WorldDbContext context)
        {
            _notificationSVC = notificationService;
            _dbContext = context;
            Strings = new List<StringResource>();
        }

        public IEnumerable<StringResource> GetStrings()
        {
            var queryString = "SELECT * FROM dbo.StringResource";
            SqlConnection conn = (SqlConnection)_dbContext.CreateConnection();
            var cmd = new SqlCommand(queryString, conn);

            try
            {
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    StringResource strResource = new StringResource();
                    strResource.Code = Convert.ToInt32(reader["code"]);
                    strResource.Name = reader["name"].ToString();
                    strResource.Value = reader["value"].ToString();
                    Strings.Add(strResource);
                }
            }
            catch (Exception ex)
            {
                _notificationSVC.WriteException(ex);
            }
            finally
            {
                conn.Close();
            }

            return Strings;
        }
    }
}
