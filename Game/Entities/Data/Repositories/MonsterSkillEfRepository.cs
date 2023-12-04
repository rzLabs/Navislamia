using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Navislamia.Data.Entities;
using Navislamia.Game.Entities.Data.Interfaces;

namespace Navislamia.Data.Repositories;

public class MonsterSkillEfRepository : IEfRepository
{
    const string query = "select * from dbo.MonsterSkillResource order by id, sub_id";

    IDbConnection dbConnection;

    IEnumerable<MonsterSkill> Data;

    public string Name => "MonsterSkillResource";

    public int Count => Data?.Count() ?? 0;

    public IEnumerable<T> GetData<T>() => (IEnumerable<T>)Data;

    public MonsterSkillEfRepository(IDbConnection connection) => dbConnection = connection;

    public async Task<IEfRepository> Load()
    {
        List<MonsterSkill> skills = new List<MonsterSkill>();

        using IDataReader sqlRdr = await dbConnection.ExecuteReaderAsync(query);

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
        
        Data = skills;

        return this;
    }
}
