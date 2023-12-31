namespace Navislamia.Game.Network.Packets.Enums;

public enum AuthPackets : ushort
{
    TS_GA_LOGIN = 20001,
    TS_AG_LOGIN_RESULT = 20002,

    TS_GA_CLIENT_LOGIN = 20010,
    TS_AG_CLIENT_LOGIN = 20011,
    
    TS_GA_CLIENT_LOGOUT = 20012,
    
    TM_AG_KICK_CLIENT		= 20013,
    TM_GA_CLIENT_KICK_FAILED	= 20014,
}
