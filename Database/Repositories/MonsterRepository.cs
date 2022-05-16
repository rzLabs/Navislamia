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
    public class MonsterRepository : IRepository
    {
        const string query = "select * from dbo.MonsterResource with (nolock)";

        IDbConnection dbConnection;

        IEnumerable<MonsterBase> Data;

        public string Name => "MonsterResource";

        public int Count => Data?.Count() ?? 0;

        public IEnumerable<T> GetData<T>() => (IEnumerable<T>)Data;

        public MonsterRepository(IDbConnection connection) => dbConnection = connection;

        public async Task<IRepository> Load()
        {
            List<MonsterBase> monsters = new List<MonsterBase>();

            try
            {
                using IDataReader sqlRdr = await dbConnection.ExecuteReaderAsync(query);

                while (sqlRdr.Read())
                {
                    MonsterBase monster = new MonsterBase();

                    monster.ID = sqlRdr.GetInt32(0);
                    monster.MonsterGroup = sqlRdr.GetInt32(1);


                }
            }
            catch (Exception ex)
            {

            }

            Data = monsters;

            return this;
        }

    }
}
