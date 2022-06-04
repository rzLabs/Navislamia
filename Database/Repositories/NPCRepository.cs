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
    public class NPCRepository
    {
        const string query = "select * from dbo.NpcResource with (nolock)";

        IDbConnection dbConnection;

        public string Name => "NPCResource";

        public NPCRepository(IDbConnection connection) => dbConnection = connection;

        public List<NPCBase> Fetch() => dbConnection.QueryAsync<NPCBase>(query).Result.ToList();
    }
}
