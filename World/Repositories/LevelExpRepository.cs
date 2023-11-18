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
    public class LevelExpRepository : IRepository
    {
        const string query = "select * from dbo.LevelResource";

        IDatabaseModule _databaseModule;

        IEnumerable<LevelExp> Data;

        public string Name => "LevelResource";

        public int Count => Data?.Count() ?? 0;

        public LevelExpRepository(IDatabaseModule databaseModule) => _databaseModule = databaseModule;

        public IEnumerable<T> GetData<T>() => (IEnumerable<T>)Data;

        public async Task<IRepository> Load()
        {
            var levelExps = new List<LevelExp>();

            using IDbConnection sqlConn = _databaseModule.WorldConnection;
            using IDataReader sqlRdr = await _databaseModule.ExecuteReaderAsync(query, sqlConn);

            while (sqlRdr.Read())
            {
                var levelExp = new LevelExp()
                {
                    Level = sqlRdr.GetInt32(0),
                    Exp = sqlRdr.GetInt64(1),
                    JP = new int[4]
                    {
                        sqlRdr.GetInt32(2),
                        sqlRdr.GetInt32(3),
                        sqlRdr.GetInt32(4),
                        sqlRdr.GetInt32(5)
                    }
                };

                levelExps.Add(levelExp);
            }

            Data = levelExps;

            return this;
        }
    }
}
