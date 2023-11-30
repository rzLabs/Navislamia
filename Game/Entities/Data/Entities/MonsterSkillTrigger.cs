using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Data.Entities;

public class MonsterSkillTrigger
{
    public int ID;

    public float[] Value { get; set; } = new float[2];

    public string Script;
}
