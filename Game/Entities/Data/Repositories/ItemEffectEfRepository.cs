// using System;
// using System.Collections.Generic;
// using System.Data;
// using System.Linq;
// using System.Threading.Tasks;
// using Dapper;
// using Navislamia.Data.Entities;
// using Navislamia.Game.Repositories;
// using Navislamia.World.Entities;
//
// namespace Navislamia.Data.Repositories;
//
// internal class ItemEffectEfRepository : IEfRepository<>
// {
//     const string query = "select * from dbo.ItemEffectResource ORDER BY id, ordinal_id";
//
//     IDbConnection dbConnection;
//
//     IEnumerable<EffectInfo> Data;
//
//     public string Name => "ItemEffectResource";
//
//     public int Count => Data?.Count() ?? 0;
//
//     public IEnumerable<T> GetData<T>() => (IEnumerable<T>)Data;
//
//     public ItemEffectEfRepository(IDbConnection connection) => dbConnection = connection;
//
//     public async Task<IEfRepository> Load()
//     {
//         var itemEffects = new List<EffectInfo>();
//
//         using IDataReader sqlRdr = await dbConnection.ExecuteReaderAsync(query);
//
//         while (sqlRdr.Read())
//         {
//             var effect = new EffectInfo()
//             {
//                 ID = sqlRdr.GetInt32(0),
//                 OrdinalID = sqlRdr.GetInt32(1),
//                 Type = (EffectType)Convert.ToInt32(sqlRdr.GetByte(3)),
//                 EffectID = sqlRdr.GetInt16(4),
//                 EffectLevel = (ushort)sqlRdr.GetInt16(5),
//                 Value = new VNumber[EffectInfo.VALUE_COUNT]
//                 {
//                     (long)sqlRdr.GetDecimal(6) * 1000,
//                     (long)sqlRdr.GetDecimal(7) * 1000,
//                     (long)sqlRdr.GetDecimal(8) * 1000,
//                     (long)sqlRdr.GetDecimal(9) * 1000,
//                     (long)sqlRdr.GetDecimal(10) * 1000,
//                     (long)sqlRdr.GetDecimal(11) * 1000,
//                     (long)sqlRdr.GetDecimal(12) * 1000,
//                     (long)sqlRdr.GetDecimal(13) * 1000,
//                     (long)sqlRdr.GetDecimal(14) * 1000,
//                     (long)sqlRdr.GetDecimal(15) * 1000,
//                     (long)sqlRdr.GetDecimal(16) * 1000,
//                     (long)sqlRdr.GetDecimal(17) * 1000,
//                     (long)sqlRdr.GetDecimal(18) * 1000,
//                     (long)sqlRdr.GetDecimal(19) * 1000,
//                     (long)sqlRdr.GetDecimal(20) * 1000,
//                     (long)sqlRdr.GetDecimal(21) * 1000,
//                     (long)sqlRdr.GetDecimal(22) * 1000,
//                     (long)sqlRdr.GetDecimal(23) * 1000,
//                     (long)sqlRdr.GetDecimal(24) * 1000,
//                     (long)sqlRdr.GetDecimal(25) * 1000
//                 }
//             };
//
//             itemEffects.Add(effect);
//         }
//
//         Data = itemEffects;
//
//         return this;
//     }
// }
