using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Navislamia.Database;
using Navislamia.World.Repositories.Entities;

namespace Navislamia.World.Repositories
{
    public class StringRepository : IRepository
    {
        const string query = "select [code],[name],[value] from dbo.StringResource with (nolock)";

        IDatabaseModule _databaseModule;

        IEnumerable<StringResource> Data;

        public string Name => "StringResoure";

        public int Count => Data?.Count() ?? 0;

        public StringRepository(IDatabaseModule databaseModule) => _databaseModule = databaseModule;

        public IEnumerable<T> GetData<T>() => (IEnumerable<T>)Data;

        public IRepository Load()
        {
            using IDbConnection dbConn = _databaseModule.WorldConnection;

            Data = _databaseModule.ExecuteQueryAsync<StringResource>(query, dbConn).Result;

            return this;
        }
    }
}
