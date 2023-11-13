using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Navislamia.Database;
using Navislamia.World.Repositories.Entities;

namespace Navislamia.World.Repositories
{
    public class NPCRepository : IRepository
    {
        const string query = "select * from dbo.NpcResource with (nolock)";

        IDatabaseModule _databaseModule;

        IEnumerable<NPCBase> Data;

        public string Name => "NPCResource";

        public int Count => Data?.Count() ?? 0;

        public IEnumerable<T> GetData<T>() => (IEnumerable<T>)Data;

        public NPCRepository(IDatabaseModule databaseModule) => _databaseModule = databaseModule;

        public IRepository Load()
        {
            using IDbConnection dbConn = _databaseModule.WorldConnection;

            Data = _databaseModule.ExecuteQueryAsync<NPCBase>(query, dbConn).Result;

            return this;
        }
    }
}
