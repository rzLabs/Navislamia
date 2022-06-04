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
    public class StringRepository : IRepository<StringResource>
    {
        const string query = "select [code],[name],[value] from dbo.StringResource with (nolock)";

        IDbConnection dbConnection;

        public string Name => "StringResoure";

        public StringRepository(IDbConnection connection) => dbConnection = connection;

        public List<StringResource> Fetch() => dbConnection.QueryAsync<StringResource>(query).Result.ToList();
    }
}
