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
    public class MonsterItemDropRepository : IRepository
    {
        const int itemDropsPerRow = 10;
        const string query = "select * from dbo.MonsterDropTableResource order by id, sub_id";

        IDbConnection dbConnection;

        IEnumerable<MonsterItemDropInfo> Data;

        public string Name => "MonsterItemDropInfo";

        public int Count => Data?.Count() ?? 0;

        public IEnumerable<T> GetData<T>() => (IEnumerable<T>)Data;

        public MonsterItemDropRepository(IDbConnection connection) => dbConnection = connection;

        public async Task<IRepository> Load()
        {
            int lastID = 0;

            List<MonsterItemDropInfo> itemDrops = new List<MonsterItemDropInfo>();

            try
            {
                using IDataReader sqlRdr = await dbConnection.ExecuteReaderAsync(query);

                var itemIdx = 1;
                var itemDrop = new MonsterItemDropInfo();

                while (sqlRdr.Read())
                {
                    var itemID = Convert.ToInt32(sqlRdr[$"drop_item_id_{(itemIdx - 1):D2}"]);

                    if (itemID != lastID)
                    {
                        itemDrops.Add(itemDrop);

                        itemDrop = new MonsterItemDropInfo();

                        lastID = itemID;
                    }

                    for (int i = 0; i < itemDropsPerRow; i++) // we must add every valid drop in this loop (up to 10)
                    {
                        if (itemID == 0) // there is no valid skill here, proceed
                            continue;

                        itemDrop.ItemID = itemID;
                        itemDrop.Percentage = Convert.ToSingle(sqlRdr[$"drop_percentage_{(itemIdx - 1):D2}"]);
                        itemDrop.MinCount = Convert.ToInt16(sqlRdr[$"drop_min_count_{(itemIdx - 1):D2}"]);
                        itemDrop.MinCount = Convert.ToInt16(sqlRdr[$"drop_max_count_{(itemIdx - 1):D2}"]);
                        itemDrop.MinLevel = Convert.ToInt16(sqlRdr[$"drop_min_level_{(itemIdx - 1):D2}"]);
                        itemDrop.MinLevel = Convert.ToInt16(sqlRdr[$"drop_max_level_{(itemIdx - 1):D2}"]);

                        itemDrops.Add(itemDrop);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            Data = itemDrops;

            return this;
        }
    }
}
