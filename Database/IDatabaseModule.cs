using System.Threading.Tasks;
using System.Data;
using Navislamia.Database.Enums;

namespace Navislamia.Database
{
    public interface IDatabaseModule
    {
        public IDbConnection WorldConnection { get;  }

        public IDbConnection PlayerConnection { get;  }

        public async Task<int> ExecuteScalar(string command, DbContextType type = DbContextType.Player) => 0;

        public async Task<int> ExecuteStoredProcedure<T>(string storedProcedure, T parameters) => 0;
    }
}
