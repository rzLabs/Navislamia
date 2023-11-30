using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Data.Entities;

public class MonsterItemDrop
{
    public int ID;

    public List<MonsterItemDropInfo> Drops;

    public MonsterItemDrop(int id, List<MonsterItemDropInfo> drops)
    {
        ID = id;
        Drops = drops;
    }
}

public struct MonsterItemDropInfo
{
    public int ItemID;
    public float Percentage;
    public short MinCount, MaxCount;
    public short MinLevel, MaxLevel;
}
