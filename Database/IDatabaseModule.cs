using System.Threading.Tasks;
using System.Data;
using Navislamia.Database.Enums;
using System.Collections;
using System.Collections.Generic;

namespace Navislamia.Database
{
    public interface IDatabaseModule
    {
        public IDbConnection WorldConnection { get;  }

        public IDbConnection PlayerConnection { get;  }

        public async Task<IDataReader> ExecuteReaderAsync(string command, IDbConnection connection, DbContextType type = DbContextType.World) => null;

        public async Task<IEnumerable<T>> ExecuteQueryAsync<T>(string command, IDbConnection connection, DbContextType type = DbContextType.World) => null;

        public async Task<int> ExecuteScalar(string command, DbContextType type = DbContextType.Player) => 0;

        public async Task<int> ExecuteStoredProcedure<T>(string storedProcedure, T parameters) => 0;
    }
}
