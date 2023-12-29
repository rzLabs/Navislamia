namespace Navislamia.Game.DataAccess.Entities.Arcadia;

public class LevelResourceEntity
{
    public int Level { get; set; }
    public long NormalExp { get; set; }
    public int[] JLvs { get; set; } // = new int[4];
}
