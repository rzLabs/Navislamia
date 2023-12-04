namespace DevConsole.Models.Arcadia;

public class StatResourceEntity : Entity
{
    public int Str { get; set; }
    public int Vit { get; set; }
    public int Dex { get; set; }
    public int Agi { get; set; }
    public int Int { get; set; }
    public int Men { get; set; } // TODO rename to Wisdom?
    public int Luk { get; set; }
}