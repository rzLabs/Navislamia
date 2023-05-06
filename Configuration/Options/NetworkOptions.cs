using Navislamia.Configuration.Options;

namespace Configuration;

public class NetworkOptions
{
    public AuthOptions Auth { get; set; }
    
    public GameOptions Game { get; set; }
    
    public UploadOptions Upload { get; set; }
    
    public int Backlog { get; set; }
    
    public int BufferSize { get; set; }
    
    public int MaxConnections { get; set; }
    
    public string CipherKey { get; set; }

}