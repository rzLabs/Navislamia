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
    public class StringRepository : IRepository
    {
        const string query = "select [code],[name],[value] from dbo.StringResource";

        IDbConnection dbConnection;

        IEnumerable<StringResource> Data;

        public string Name => "StringResoure";

        public int Count => Data?.Count() ?? 0;

        public StringRepository(IDbConnection connection) => dbConnection = connection;

        public IEnumerable<T> GetData<T>() => (IEnumerable<T>)Data;

        public async Task<IRepository> Init()
        {
            Data = await dbConnection.QueryAsync<StringResource>(query);

            return this;
        }
    }
}
