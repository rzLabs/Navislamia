using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Text;

using Configuration;
using System.Data;
using Navislamia.Database.Enums;

namespace Database
{
    public interface IDatabaseService
    {
        public IDbConnection WorldConnection { get;  }

        public IDbConnection PlayerConnection { get;  }

        public async Task<int> ExecuteScalar(string command, DbContextType type = DbContextType.Player) => 0;

        public async Task<int> ExecuteStoredProcedure<T>(string storedProcedure, T parameters) => 0;
    }
}
