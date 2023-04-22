namespace Configuration;

public class NetworkOptions
{
    public string AuthIp { get; set; }
    public int AuthPort { get; set; }
    public int AuthServerIndex { get; set; }

    public string UploadIp { get; set; }
    public int UploadPort { get; set; }
    
    public string Ip { get; set; }
    public int Port { get; set; }
    public int Backlog { get; set; }
    public int BufferSize { get; set; }
    
    public string CipherKey { get; set; }

}