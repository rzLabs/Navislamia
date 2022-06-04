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
    public class MonsterSkillRepository : IRepository<MonsterSkill>
    {
        const string query = "select * from dbo.MonsterSkillResource order by id, sub_id";

        IDbConnection dbConnection;

        public string Name => "MonsterSkillResource";

        public MonsterSkillRepository(IDbConnection connection) => dbConnection = connection;

       public List<MonsterSkill> Fetch()
        {
            List<MonsterSkill> skills = new List<MonsterSkill>();

            using IDataReader sqlRdr = dbConnection.ExecuteReaderAsync(query).Result;

            while (sqlRdr.Read())
            {
                MonsterSkill skill = new MonsterSkill();

                skill.ID = sqlRdr.GetInt32(0);

                // sub_id = 1

                for (int i = 1; i < 6; i++) // Need to loop trigger/skill fields and set their values appropriately
                {
                    skill.Trigger[i - 1].ID = Convert.ToInt32(sqlRdr[$"trigger_{i}_type"]);
                    skill.Trigger[i - 1].Value[0] = Convert.ToSingle(sqlRdr[$"trigger_{i}_value_1"]);
                    skill.Trigger[i - 1].Value[1] = Convert.ToSingle(sqlRdr[$"trigger_{i}_value_2"]);
                    skill.Trigger[i - 1].Script = sqlRdr[$"trigger_{i}_function"].ToString();

                    skill.Skill[i - 1].ID = Convert.ToInt32(sqlRdr[$"skill{i}_id"]);
                    skill.Skill[i - 1].LV = Convert.ToInt32(sqlRdr[$"skill{i}_lv"]);
                    skill.Skill[i - 1].Probability = Convert.ToSingle(sqlRdr[$"skill{1}_probability"]);
                }

                skills.Add(skill);
            }
            
            return skills;
        } 
    }
}
