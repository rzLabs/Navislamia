using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Navislamia.Data.Entities;
using Navislamia.Game.Entities.Data.Interfaces;

namespace Navislamia.Data.Repositories;

public class MonsterItemDropEfRepository : IEfRepository
{
    const int itemDropsPerRow = 10;
    const string query = "select * from dbo.MonsterDropTableResource order by id, sub_id";

    IDbConnection dbConnection;

    IEnumerable<MonsterItemDrop> Data;

    public string Name => "MonsterDropTableResource";

    public int Count => Data?.Count() ?? 0;

    public IEnumerable<T> GetData<T>() => (IEnumerable<T>)Data;

    public MonsterItemDropEfRepository(IDbConnection connection) => dbConnection = connection;

    public async Task<IEfRepository> Load()
    {
        var monsterDrops = new List<MonsterItemDrop>();

        using IDataReader sqlRdr = await dbConnection.ExecuteReaderAsync(query);

        MonsterItemDropInfo itemDrop;
        List<MonsterItemDropInfo> itemDrops = new List<MonsterItemDropInfo>();

        var lastID = 0;

        while (sqlRdr.Read())
        {
            var id = Convert.ToInt32(sqlRdr["id"]);

            for (int i = 0; i < itemDropsPerRow; i++) // we must add every valid drop in this loop (up to 10)
            {
                var itemID = Convert.ToInt32(sqlRdr[$"drop_item_id_{(i):D2}"]);

                if (itemID == 0)
                    continue;

                itemDrop = new MonsterItemDropInfo();
                
                itemDrop.ItemID = itemID;
                itemDrop.Percentage = Convert.ToSingle(sqlRdr[$"drop_percentage_{(i):D2}"]);
                itemDrop.MinCount = Convert.ToInt16(sqlRdr[$"drop_min_count_{(i):D2}"]);
                itemDrop.MaxCount = Convert.ToInt16(sqlRdr[$"drop_max_count_{(i):D2}"]);
                itemDrop.MinLevel = Convert.ToInt16(sqlRdr[$"drop_min_level_{(i):D2}"]);
                itemDrop.MaxLevel = Convert.ToInt16(sqlRdr[$"drop_max_level_{(i):D2}"]);

                itemDrops.Add(itemDrop);
            }

            if (id != lastID)
            {
                lastID = id;

                monsterDrops.Add(new MonsterItemDrop(lastID, itemDrops));

                itemDrops.Clear();
            }
        }

        Data = monsterDrops;

        return this;
    }
}