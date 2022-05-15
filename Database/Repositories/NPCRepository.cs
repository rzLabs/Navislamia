using Dapper;
using Navislamia.Database.Entities;
using Navislamia.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Database.Repositories
{
    public class NPCRepository : IRepository
    {
        const string query = "select * from dbo.NpcResource with (nolock)";

        IDbConnection dbConnection;

        IEnumerable<NPCBase> Data;

        public string Name => "NPCResource";

        public int Count => Data?.Count() ?? 0;

        public IEnumerable<T> GetData<T>() => (IEnumerable<T>)Data;

        public NPCRepository(IDbConnection connection) => dbConnection = connection;

        public async Task<IRepository> Load()
        {
            Data = await dbConnection.QueryAsync<NPCBase>(query);

            return this;
        }
    }
}
