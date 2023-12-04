using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;
using DevConsole.Models.Arcadia;
using Microsoft.Extensions.Logging;
using Navislamia.Data.Entities;
using Navislamia.Database.Contexts;
using Navislamia.Game.Contexts;
using Navislamia.Game.Repositories;

namespace Navislamia.Data.Repositories;

public class LevelResourceRepository : EfRepository<LevelResourceEntity>
{
    public LevelResourceRepository(ArcadiaContext context, ILogger<EfRepository<LevelResourceEntity>> logger) : base(context, logger)
    {
    }

    // IEnumerable<LevelExp> Data;

    // public string Name => "LevelResource";

    // public int Count => Data?.Count() ?? 0;

    // public LevelResourceRepository(IDbConnection dbConnection) => _dbConnection = dbConnection;

    // public IEnumerable<T> GetData<T>() => (IEnumerable<T>)Data;
    //
    // public async Task<IEfRepository> Load()
    // {
    //     var levelExps = new List<LevelExp>();
    //
    //     using IDataReader sqlRdr = await _dbConnection.ExecuteReaderAsync(query);
    //
    //     while (sqlRdr.Read())
    //     {
    //         var levelExp = new LevelExp()
    //         {
    //             Level = sqlRdr.GetInt32(0),
    //             Exp = sqlRdr.GetInt64(1),
    //             JP = new int[4]
    //             {
    //                 sqlRdr.GetInt32(2),
    //                 sqlRdr.GetInt32(3),
    //                 sqlRdr.GetInt32(4),
    //                 sqlRdr.GetInt32(5)
    //             }
    //         };
    //
    //         levelExps.Add(levelExp);
    //     }
    //
    //     Data = levelExps;
    //
    //     return this;
    // }

}
