// using System;
// using System.Collections.Generic;
// using System.Data;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
//
// using Dapper;
// using Navislamia.Data.Entities;
//
// namespace Navislamia.Data.Repositories;
//
// public class NpcEfRepository : IEfRepository
// {
//     const string query = "select * from dbo.NpcResource with (nolock)";
//
//     IDbConnection _dbConnection;
//
//     IEnumerable<NPCBase> Data;
//
//     public string Name => "NPCResource";
//
//     public int Count => Data?.Count() ?? 0;
//
//     public IEnumerable<T> GetData<T>() => (IEnumerable<T>)Data;
//
//     public NpcEfRepository(IDbConnection dbConnection) => _dbConnection = dbConnection;
//
//     public async Task<IEfRepository> Load()
//     {
//         Data = await _dbConnection.QueryAsync<NPCBase>(query);
//
//         return this;
//     }
// }
