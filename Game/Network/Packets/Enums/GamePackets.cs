namespace Navislamia.Game.Network.Packets.Enums;

public enum GamePackets : ushort
{
    TM_SC_RESULT = 0,

    TM_CS_LOGIN = 1,

    TM_SC_DISCONNECT_DESC = 28,

    TM_CS_VERSION = 50,

    TM_CS_CHARACTER_LIST = 2001,

    TM_CS_CREATE_CHARACTER = 2002,

    TM_CS_DELETE_CHARACTER = 2003,

    TM_CS_ACCOUNT_WITH_AUTH = 2005,

    TM_CS_CHECK_CHARACTER_NAME = 2006,

    TM_CS_REPORT = 8000,

    TM_NONE = 9999
}
