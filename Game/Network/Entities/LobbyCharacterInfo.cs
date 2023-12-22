using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Navislamia.Game.Network.Entities
{
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

    /*
	struct LOBBY_CHARACTER_INFO : MODEL_INFO
	{
		int				level;
		int				job;
		int				job_level;
		int				exp_percentage;	// ( 0 ~ 100 )
		int				hp;
		int				mp;
		int				permission;
		bool			is_banned;
		char			name[19];
		unsigned long	skin_color;
		char			szCreateTime[30];
		char			szDeleteTime[30];
		int				wear_item_enhance_info[ItemBase::MAX_ITEM_WEAR]; //96
		int				wear_item_level_info[ItemBase::MAX_ITEM_WEAR]; // 96
		char			wear_item_elemental_type[ItemBase::MAX_ITEM_WEAR];
	};

	 struct MODEL_INFO 
	{
		int				sex;
		int				race;
		int				model_id[5];
		int				hair_color_index;
		unsigned int	hair_color_rgb;
		unsigned int	hide_equip_flag;
		int				texture_id;
		int				wear_info[ItemBase::MAX_ITEM_WEAR];
	};
     */
}
