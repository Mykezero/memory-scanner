using System;
using System.Runtime.InteropServices;

namespace Vivisection
{
    // FFACE V4 structures passed to and from the library. Standard file
    // used by a lot of applications, most of this comes from FFACETools.
    #region Player Info
    // Used for NPCs also.
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct PLAYERINFO
    {

        public int HPMax;
        public int MPMax;
        public Job MainJob;
        public byte MainJobLVL;
        public Job SubJob;
        public byte SubJobLVL;
        public ushort EXPIntoLVL;
        public ushort EXPForLVL;
        public PlayerStats Stats;
        public PlayerStats StatModifiers;
        public short Attack;
        public short Defense;
        public Resistances Elements;
        public short Title;
        public short Rank;
        public short RankPts;
        public byte Nation;
        public byte Residence;
        public int HomePoint;
        public CombatSkills CombatSkills;
        public MagicSkills MagicSkills;
        public CraftSkills CraftLevels;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 146)]
        private byte[] _empty0_;
        public ushort LimitPoints;
        public byte MeritPoints;
        public byte LimitMode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 78)]
        private byte[] _empty1_;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public StatusEffect[] Buffs;
    }
    #endregion

    #region Party Member
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct PARTYMEMBER
    {
        public int pad0;
        public byte Index;
        public byte MemberNumber;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
        public string Name;
        public int SvrID;
        public int ID;
        private int unknown0;
        public int CurrentHP;
        public int CurrentMP;
        public int CurrentTP;
        public byte CurrentHPP;
        public byte CurrentMPP;
        public short Zone;
        private int _blank_;
        public uint FlagMask;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        private byte[] _blank2_;
        public int SvrIDDupe;
        public byte CurrentHPPDupe;
        public byte CurrentMPPDupe;
        public bool Active;
        private byte _blank3_;

    }
    #endregion

    #region Target Info
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct TARGETINFO
    {
        public int CurrentID;
        public int SubID;
        public int CurrentSvrID;
        public int SubSrvID;
        public ushort CurrentMask;
        public ushort SubMask;
        public byte IsLocked;
        public byte IsSub;
        public byte HPP;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Name;
    }
    #endregion

    #region Alliance Info
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct ALLIANCEINFO
    {
        public int AllianceLeaderID;
        public int Party0LeaderID;
        public int Party1LeaderID;
        public int Party2LeaderID;
        public byte Party0Visible;
        public byte Party1Visible;
        public byte Party2Visible;
        public byte Party0Count;
        public byte Party1Count;
        public byte Party2Count;
        public byte Invited;
        private byte _blank_;
    }
    #endregion

    #region Player Stats
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct PlayerStats
    {
        public short Str;
        public short Dex;
        public short Vit;
        public short Agi;
        public short Int;
        public short Mnd;
        public short Chr;
    }
    #endregion

    #region Resistances
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct Resistances
    {
        public ushort Fire;
        public ushort Ice;
        public ushort Wind;
        public ushort Earth;
        public ushort Lightning;
        public ushort Water;
        public ushort Light;
        public ushort Dark;
    }
    #endregion

    #region Combat Skills
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct CombatSkills
    {
        public ushort HandToHand;
        public ushort Dagger;
        public ushort Sword;
        public ushort GreatSword;
        public ushort Axe;
        public ushort GreatAxe;
        public ushort Scythe;
        public ushort Polearm;
        public ushort Katana;
        public ushort GreatKatana;
        public ushort Club;
        public ushort Staff;
        private ushort blank0;
        private ushort blank1;
        private ushort blank2;
        private ushort blank3;
        private ushort blank4;
        private ushort blank5;
        private ushort blank6;
        private ushort blank7;
        private ushort blank8;
        private ushort blank9;
        private ushort blankA;
        private ushort blankB;
        public ushort Archery;
        public ushort Marksmanship;
        public ushort Throwing;
        public ushort Guarding;
        public ushort Evasion;
        public ushort Shield;
        public ushort Parrying;
    }
    #endregion

    #region Magic Skills
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct MagicSkills
    {
        public ushort Divine;
        public ushort Healing;
        public ushort Enhancing;
        public ushort Enfeebling;
        public ushort Elemental;
        public ushort Dark;
        public ushort Summon;
        public ushort Ninjitsu;
        public ushort Singing;
        public ushort String;
        public ushort Wind;
        public ushort BlueMagic;
        private ushort blank0;
        private ushort blank1;
        private ushort blank2;
        private ushort blank3;
    }
    #endregion

    #region Craft Skills
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct CraftSkills
    {
        public ushort Fishing;
        public ushort Woodworking;
        public ushort Smithing;
        public ushort Goldsmithing;
        public ushort Clothcraft;
        public ushort Leathercraft;
        public ushort Bonecraft;
        public ushort Alchemy;
        public ushort Cooking;
    }
    #endregion

    #region Inventory Item
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct INVENTORYITEM
    {
        public ushort ID;
        public byte Index;
        public uint Count;
        public uint Flag;
        public uint Price;
        public ushort Extra;
    }
    #endregion

    #region Trade Item
    public struct TRADEITEM
    {
        public ushort ItemID;
        public char Index;
        public char Count;
    }

    public struct TRADEINFO
    {
        public uint Gil;
        public int TargetID;
        public char SelectedBox;
        public TRADEITEM[] Items;
        public void Init() { Items = new TRADEITEM[8]; }
    }
    #endregion

    #region Treasure Item
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct TREASUREITEM
    {
        public byte Flag; //3=no item, 2=item	
        public short ItemID;
        public byte Count;
        public TreasureStatus Status;
        public short MyLot;
        public short WinLot;
        public int WinPlayerSrvID;
        public int WinPlayerID;
        public int TimeStamp; //utc timestamp
    }
    #endregion
}