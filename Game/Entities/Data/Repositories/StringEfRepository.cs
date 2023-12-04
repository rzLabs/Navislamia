using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Navislamia.Data.Entities;
using Navislamia.Game.Entities.Data.Interfaces;

namespace Navislamia.Game.Entities.Data.Repositories;

public class StringEfRepository : IEfRepository
{
    const string query = "select [code],[name],[value] from dbo.StringResource with (nolock)";

    IDbConnection _dbConnection;

    IEnumerable<StringResource> Data;

    public string Name => "StringResoure";

    public int Count => Data?.Count() ?? 0;

    public StringEfRepository(IDbConnection dbConnection) => _dbConnection = dbConnection;

    public IEnumerable<T> GetData<T>() => (IEnumerable<T>)Data;

    public async Task<IEfRepository> Load()
    {
        Data = await _dbConnection.QueryAsync<StringResource>(query);

        return this;
    }
}
