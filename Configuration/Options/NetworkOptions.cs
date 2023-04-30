namespace Configuration;

public class NetworkOptions
{
    public string AuthIp { get; set; }
    public ushort AuthPort { get; set; }
    public int AuthServerIndex { get; set; }

    public string UploadIp { get; set; }
    public ushort UploadPort { get; set; }
    
    public string Ip { get; set; }
    public ushort Port { get; set; }
    public int Backlog { get; set; }
    public int BufferSize { get; set; }
    public int MaxConnections { get; set; }
    
    public string CipherKey { get; set; }

}