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
    public class MonsterRepository : IRepository<MonsterBase>
    {
        const string query = "select * from dbo.MonsterResource with (nolock)";

        IDbConnection dbConnection;

        public string Name => "MonsterResource";

        public MonsterRepository(IDbConnection connection) => dbConnection = connection;

        public List<MonsterBase> Fetch()
        {
            List<MonsterBase> monsters = new List<MonsterBase>();

            using IDataReader sqlRdr = dbConnection.ExecuteReaderAsync(query).Result;

            while (sqlRdr.Read())
            {
                MonsterBase monster = new MonsterBase();

                monster.ID = sqlRdr.GetInt32(0);
                monster.MonsterGroup = sqlRdr.GetInt32(1);
                monster.TransformLevel = sqlRdr.GetInt32(6);
                monster.WalkType = sqlRdr.GetByte(7);
                monster.Size = Convert.ToSingle(sqlRdr[9]);
                monster.Scale = Convert.ToSingle(sqlRdr[10]);
                monster.Level = sqlRdr.GetInt32(18);
                monster.Group = sqlRdr.GetInt32(19);
                monster.MagicType = sqlRdr.GetInt32(20);
                monster.Race = sqlRdr.GetInt32(21);
                monster.VisibleRange = sqlRdr.GetInt32(22);
                monster.ChaseRange = sqlRdr.GetInt32(23);
                monster.MonsterType = sqlRdr.GetByte(29);
                monster.StatID = sqlRdr.GetInt32(30);
                monster.FightType = sqlRdr.GetInt32(31);
                monster.SkillLinkID = sqlRdr.GetInt32(32);
                monster.Ability = sqlRdr.GetInt32(36);
                monster.WalkSpeed = sqlRdr.GetInt32(39);
                monster.RunSpeed = sqlRdr.GetInt32(40);
                monster.AttackRange = Convert.ToSingle(sqlRdr[41]);
                monster.HidesenseRange = Convert.ToSingle(sqlRdr[42]);
                monster.HP = sqlRdr.GetInt32(43);
                monster.MP = sqlRdr.GetInt32(44);
                monster.AttackPoint = sqlRdr.GetInt32(45);
                monster.MagicPoint = sqlRdr.GetInt32(46);
                monster.Defense = sqlRdr.GetInt32(47);
                monster.MagicDefense = sqlRdr.GetInt32(48);
                monster.AttackSpeed = sqlRdr.GetInt32(49);
                monster.CastingSpeed = sqlRdr.GetInt32(50);
                monster.Accuracy = sqlRdr.GetInt32(51);
                monster.Avoid = sqlRdr.GetInt32(52);
                monster.MagicAccuracy = sqlRdr.GetInt32(53);
                monster.MagicAvoid = sqlRdr.GetInt32(54);
                monster.TamingCode = sqlRdr.GetInt32(55);
                monster.TamingPercentage = Convert.ToSingle(sqlRdr[56]);
                monster.Exp = sqlRdr.GetInt32(58);
                monster.JP = sqlRdr.GetInt32(59);
                monster.GoldDropPercentage = sqlRdr.GetInt32(60);
                monster.GoldMin = sqlRdr.GetInt32(61);
                monster.GoldMax = sqlRdr.GetInt32(62);
                monster.ChaosDropPercentage = sqlRdr.GetInt32(63);
                monster.ChaosMin = sqlRdr.GetInt32(64);
                monster.ChaosMax = sqlRdr.GetInt32(65);
                monster.Exp2 = sqlRdr.GetInt32(66);
                monster.JP2 = sqlRdr.GetInt32(67);
                monster.GoldMin2 = sqlRdr.GetInt32(68);
                monster.GoldMax2 = sqlRdr.GetInt32(69);
                monster.ChaosMin2 = sqlRdr.GetInt32(70);
                monster.ChaosMax2 = sqlRdr.GetInt32(71);
                monster.DropLinkID = sqlRdr.GetInt32(72);
                monster.ScriptOnDead = sqlRdr.GetString(75);

                monsters.Add(monster);
            }

            return monsters;
        } 
    }
}
