using System.Runtime.InteropServices;

namespace Navislamia.Game.Network.Entities;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct LobbyCharacterInfo
{
    public int Sex;
    public int Race;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
    public int[] ModelId;

    public int HairColorIndex;

    public uint HairColorRGB;
    public uint HideEquipFlag;
    public int TextureID;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
    public int[] WearInfo;

    public int Level;
    public int Job;
    public int JobLevel;
    public int ExpPercentage;
    public int HP;
    public int MP;
    public int Permission;
    public byte IsBanned;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 19)]
    public string Name;

    public uint SkinColor;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
    public string CreateTime;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
    public string DeleteTime;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
    public int[] WearItemEnhanceInfo;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
    public int[] WearItemLevelInfo;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
    public char[] WearItemElementalType;

    public LobbyCharacterInfo()
    {
        WearInfo = new int[24];
        WearItemLevelInfo = new int[24];
        WearItemEnhanceInfo = new int[24];
        WearItemElementalType = new char[24];
    }
}