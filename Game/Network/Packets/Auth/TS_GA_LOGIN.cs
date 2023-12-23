using System.Runtime.InteropServices;

namespace Navislamia.Game.Network.Packets;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TS_GA_LOGIN
{
    public ushort ServerIndex;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
    public string ServerName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    public string ServerScreenshotURL;
    public byte IsAdultServer;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
    public string ServerIP;
    public int ServerPort;

    public TS_GA_LOGIN(ushort serverIndex, string serverName, string serverScreenshotURL, byte isAdultServer, string serverIP, int serverPort)
    {
        ServerIndex = serverIndex;
        ServerName = serverName;
        ServerScreenshotURL = serverScreenshotURL;
        IsAdultServer = isAdultServer;
        ServerIP = serverIP;
        ServerPort = serverPort;
    }
}
