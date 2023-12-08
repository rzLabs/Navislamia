// using System;
// using System.Collections.Generic;
// using System.Data;
// using System.Linq;
// using System.Threading.Tasks;
// using Dapper;
// using Navislamia.Data.Entities;
//
// namespace Navislamia.Data.Repositories;
//
// internal class SetItemEffectEfRepository : IEfRepository
// {
//     const string query = "select * from dbo.SetItemEffectResource";
//
//     IDbConnection dbConnection;
//
//     IEnumerable<SetItemEffect> Data;
//
//     public string Name => "SetItemEffectResource";
//
//     public int Count => Data?.Count() ?? 0;
//
//     public IEnumerable<T> GetData<T>() => (IEnumerable<T>)Data;
//
//     public SetItemEffectEfRepository(IDbConnection connection) => dbConnection = connection;
//
//     public async Task<IEfRepository> Load()
//     {
//         var setEffects = new List<SetItemEffect>();
//
//         using IDataReader sqlRdr = await dbConnection.ExecuteReaderAsync(query);
//
//         while (sqlRdr.Read())
//         {
//             var setEffect = new SetItemEffect();
//
//             setEffect.SetID = sqlRdr.GetInt32(0);
//             setEffect.SetPartID = sqlRdr.GetInt32(1);
//
//             for (int i = 0; i < SetItemEffect.MAX_OPTIONS; i++)
//             {
//                 var baseTypeName = $"base_type_{i}";
//                 var baseVar1Name = $"base_var1_{i}";
//                 var baseVar2Name = $"base_var2_{i}";
//                 var optTypeName = $"opt_type_{i}";
//                 var optVar1Name = $"opt_var1_{i}";
//                 var optVar2Name = $"opt_var2_{i}";
//
//                 setEffect.BaseType[i] = Convert.ToInt16(sqlRdr[baseTypeName]);
//                 setEffect.BaseVar1[i] = Convert.ToInt32(sqlRdr[baseVar1Name]) * 1000;
//                 setEffect.BaseVar2[i] = Convert.ToInt32(sqlRdr[baseVar2Name]) * 1000;
//                 setEffect.OptType[i] = Convert.ToInt16(sqlRdr[optTypeName]);
//                 setEffect.OptVar1[i] = Convert.ToInt32(sqlRdr[optVar1Name]) * 1000;
//                 setEffect.OptVar2[i] = Convert.ToInt32(sqlRdr[optVar2Name]) * 1000;
//             }
//
//             setEffect.EffectID = sqlRdr.GetInt32(28);
//
//             setEffects.Add(setEffect);
//         }
//
//         Data = setEffects;
//
//         return this;
//     }
// }
