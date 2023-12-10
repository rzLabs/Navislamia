namespace Navislamia.Game.Models.Arcadia
{
    public class LevelResourceEntity
    {
        public int Level { get; set; }
        public long NormalExp { get; set; }
        public int[] Jlvls { get; set; } = new int[4];
    }
}