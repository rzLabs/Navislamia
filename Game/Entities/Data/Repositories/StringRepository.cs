using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Navislamia.Database;

using Navislamia.Data.Interfaces;
using Navislamia.Data.Entities;

namespace Navislamia.Data.Repositories;

public class StringRepository : IRepository
{
    const string query = "select [code],[name],[value] from dbo.StringResource with (nolock)";

    IDbConnection _dbConnection;

    IEnumerable<StringResource> Data;

    public string Name => "StringResoure";

    public int Count => Data?.Count() ?? 0;

    public StringRepository(IDbConnection dbConnection) => _dbConnection = dbConnection;

    public IEnumerable<T> GetData<T>() => (IEnumerable<T>)Data;

    public async Task<IRepository> Load()
    {
        Data = await _dbConnection.QueryAsync<StringResource>(query);

        return this;
    }
}
