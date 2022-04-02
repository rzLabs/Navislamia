using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Text;

using Configuration;

namespace Database
{
    public interface IDatabaseService
    {
        public bool Connected { get; }

        public string CommandText { get; set; }

        public void AddParameters(List<SqlParameter> parameters);

        public bool Connect(DbConnectionType type);

        public bool Disconnect();

        public async Task<bool> Reset(bool reconnect) { return false; }

        public async Task<T> ExecuteScalar<T>() { return default(T); }

        public async Task<int> ExecuteNonQuery() { return -1; }

        public async Task<SqlDataReader> ExecuteReader() { return null; }
    }
}
