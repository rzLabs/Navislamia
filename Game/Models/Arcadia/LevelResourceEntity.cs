namespace DevConsole.Models.Arcadia;

public class LevelResourceEntity
{
    public int Level { get; set; }
    public long NormalExp { get; set; }
    public int[] Jl { get; set; } = new int[4];
}