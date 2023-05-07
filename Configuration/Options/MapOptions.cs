namespace Navislamia.Configuration.Options;

public class MapOptions
{    
    public int Width { get; set; }
    public int Height { get; set; }
    public int MaxLayer { get; set; }
    public bool SkipLoading { get; set; }
    public bool SkipLoadingNfa { get; set; }
    public bool NoCollisionCheck { get; set; }
}