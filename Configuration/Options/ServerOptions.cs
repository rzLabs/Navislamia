namespace Navislamia.Configuration.Options;

public class ServerOptions
{
    public string Name { get; set; }
    public ushort Index { get; set; }
    public string ScreenshotUrl { get; set; }
    public byte IsAdultServer { get; set; }
    public uint MaxCharactersPerAccount { get; set; }
}