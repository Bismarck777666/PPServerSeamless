using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GITProtocol
{
    public enum GAMETYPE
    {
        NONE        = 0,
        PP          = 1,
        BNG         = 2,
        CQ9         = 3,
        HABANERO    = 4,
        PLAYSON     = 5,
        AMATIC      = 6,
        COUNT       = 6,
    }

    public enum GAMEID : ushort
    {
        #region Pragmatic Play Games
        PPGameStart                 = 2000,
        PandasFortune               = 2001,
        TheDogHouse                 = 2002,
        AncientEgyptClassic         = 2003,
        HerculesAndPegasus          = 2004,
        WestWildGold                = 2005,
        TheDogHouseMega             = 2006,
        StarzMegaWays               = 2007,
        WolfGold                    = 2008,
        EgyptianFortunes            = 2009,
        FiveLionsGold               = 2010,
        BuffaloKing                 = 2011,
        JuicyFruit                  = 2012,
        GreatRhino                  = 2013,
        SpartaKing                  = 2015,
        GreatRhinoDelux             = 2016,
        BeoWulf                     = 2017,
        DiamondStrike               = 2018,
        UltraHoldAndSpin            = 2019,
        ReturnDead                  = 2020,
        PirateGold                  = 2021,
        HotToBurnHoldAndSpin        = 2022,
        FiveLionsMega               = 2023,
        MadameDestinyMegaWays       = 2024,
        GoldenOx                    = 2025,
        PandaFortune2               = 2026,
        OlympusGates                = 2027,
        GreatRhinoMega              = 2028,
        FruitParty                  = 2029,
        FiveLions                   = 2030,
        UltraBurn                   = 2031,
        LuckyNewYear                = 2032,
        HotToBurn                   = 2033,
        FiveLionsDance              = 2034,
        MonkeyWarrior               = 2035,
        WildBooster                 = 2036,

        DanceParty                  = 2037,
        DragonHoldAndSpin           = 2038,
        BuffaloKingMega             = 2039,
        VegasMagic                  = 2040,
        AztecKingMega               = 2041,
        HeartOfRio                  = 2042,
        JokerKing                   = 2043,
        MustangGold                 = 2044,
        Asgard                      = 2045,
        BroncoSpirit                = 2046,

        BiggerBassBonanza           = 2047,
        CowBoysGold                 = 2048,
        MoneyMouse                  = 2049,
        BigBassBonanza              = 2050,
        AztecGems                   = 2051,
        HandOfMidas                 = 2052,
        PirateGoldDelux             = 2053,
        PeKingLuck                  = 2054,
        TheTweetyHouse              = 2055,
        EightDragons                = 2056,
        PyramidBonanza              = 2057,
        PowerOfThorMega             = 2058,
        CashElevator                = 2059,

        FloatingDragonHoldAndSpin   = 2060,
        HotFiesta                   = 2061,
        ChristmasBigBassBonanza     = 2062,
        FortuneOfGiza               = 2063,

        CashPatrol                  = 2064,
        RockVegas                   = 2065,
        EmptyTheBank                = 2066,
        Queenie                     = 2067,
        Cleocatra                   = 2068,
        ZombieCarnival              = 2069,
        TheUltimate5                = 2070,
        BigJuan                     = 2071,
        KoiPond                     = 2072,
        CongoCash                   = 2073,
        TreasureWild                = 2074,

        SugarRush                   = 2075,
        StarlightPrincess           = 2076,
        WildWestGoldMega            = 2081,
        BarnFestival                = 2084,
        DragonHero                  = 2092,
        StarlightChristmas          = 2097,
        RiseOfSamurai3              = 2100,
        YumYumPowerWays             = 2113,
        FishEye                     = 2115,
        MonsterSuperlanche          = 2116,
        CandyVillage                = 2123,
        FloatingDragonMega          = 2124,
        LuckyLightning              = 2125,
        Mochimon                    = 2126,
        ClubTropicana               = 2147,
        FireArcher                  = 2148,
        RabbitGarden                = 2226,
        MysteryOfTheOrient          = 2275,
        TheDogHouseMultihold        = 2293,
        TheKnightKing               = 2295,

        PPGameEnd                   = 2300,
        #endregion
        
        #region BNG Games
        BNGGameStart    = 2500,
        ScarabRiches    = 2501,
        GoldExpress     = 2502,
        TigerJungle     = 2503,
        WolfNight       = 2504,
        LordFortune     = 2505,
        HitTheGold      = 2506,
        SunOfEgypt      = 2507,
        BlackWolf       = 2508,
        MoonSisters     = 2509,
        DragonPearls    = 2510,
        AztecSun        = 2511,
        GreatPanda      = 2512,
        SunOfEgypt2     = 2513,
        TigerStone      = 2514,
        ScarabTemple    = 2515,
        BookOfSun       = 2516,
        SunOfEgypt3     = 2517,
        WolfSaga        = 2518,
        QueenOfTheSun   = 2519,
        GreenChilli     = 2520,
        BigHeist        = 2521,
        MagicApple2     = 2522,
        HappyFish       = 2523,
        MagicBall       = 2524,
        TigersGold      = 2525,
        StickyPiggy     = 2526,
        SuperRichGod    = 2527,
        DragonPearls15  = 2528,
        AztecFire       = 2529,

        BNGGameEnd      = 2600,
        #endregion

        #region CQ9 Games
        GodOfWar            = 3001,
        JumpHigh            = 3003,
        RaveJump            = 3004,
        GuGuGu              = 3005,
        ShouXin             = 3006,
        JumpHigh2           = 3007,
        GuGuGu2             = 3008,
        CricketFever        = 3009,
        Thor                = 3010,
        Thor2               = 3011,
        InvincibleElephant  = 3012,
        FlowerFortunes      = 3013,
        TheBeastWar         = 3014,
        FireChibi           = 3015,
        FireChibi2          = 3016,
        Meow                = 3017,
        ZeusM               = 3018,
        SnowQueen           = 3019,
        Seven77             = 3020, //777
        FunnyAlpaca         = 3021,
        MoveNJump           = 3022,
        MirrorMirror        = 3023,
        FaCaiShen           = 3024,
        LuckyBats           = 3025,
        JumpingMobile       = 3026,
        SoSweet             = 3027,
        Wonderland          = 3028,
        Super5              = 3029,
        Apsaras             = 3030,
        RaveHigh            = 3031,
        FireQueen           = 3032,
        FireQueen2          = 3033,
        FaCaiShenM          = 3034,
        DiscoNight          = 3035,
        SixCandy            = 3036,
        ZhongKui            = 3037,
        RaveJump2           = 3038,
        LuckyBatsM          = 3039,
        WingChun            = 3040,
        Zeus                = 3041,
        GodOfWarM           = 3042,
        RaveJumpM           = 3043,
        FireChibiM          = 3044,
        MuayThai            = 3045,
        DiscoNightM         = 3046,
        RaveJump2M          = 3047,
        GuGuGu2M            = 3048,
        GuGuGuM             = 3049,
        LuckyBoxes          = 3050,
        OrientalBeauty      = 3051,
        FruitKing2          = 3052,
        CrazyBundesliga     = 3053,
        PremiorLeague       = 3054,
        FootballEuro        = 3055,
        LaLiga              = 3056,

        WukongAndPeaches    = 3057,
        TreasureBowl        = 3058,
        WaterWorld          = 3059,
        WaterBalloons       = 3060,
        HappyRichYear       = 3061,
        OGLegend            = 3062,
        VampireKiss         = 3063,
        WildTarzan          = 3064,
        Chameleon           = 3065,
        FiveBoxing          = 3066,
        GodOfChess          = 3067,
        FootballBaby        = 3068,
        FruitKing           = 3069,
        WorldCupRussia2018  = 3070,
        WorldCupField       = 3071,
        WolfDisco           = 3072,
        GreekGods           = 3073,
        TreasureIsland      = 3074,
        GoldenEggs          = 3075,
        SakuraLegend        = 3076,
        FortuneDragon       = 3077,
        BigWolf             = 3078,
        MonkeyOfficeLegend  = 3079,
        ZumaWild            = 3080,
        SkrSkr              = 3081,
        MyeongRyang         = 3082,
        GaneshaJr           = 3083,
        HotDJ               = 3084,
        SuperDiamonds       = 3088,
        Poseidon            = 3089,
        FortuneTotem        = 3090,
        NeZhaAdvent         = 3091,
        Hephaestus          = 3092,
        MrMiser             = 3093,
        LuckyTigers         = 3094,
        Hercules            = 3095,
        DiamondTreasure     = 3096,
        Apollo              = 3097,
        LordGanesha         = 3098,
        GreatLion           = 3099,
        RedPhoenix          = 3100,
        MagicWorld          = 3101,
        TreasureHouse       = 3102,
        DragonHeart         = 3103,
        StrikerWild         = 3104,
        WanbaoDino          = 3105,
        DetectiveDee        = 3106,
        RunningAnimals      = 3107,
        SummerMood          = 3108,
        AllWild             = 3109,
        GophersWar          = 3110,
        YuanBao             = 3111,
        Acrobatics          = 3112,
        Fire777             = 3113,
        DragonTreasure      = 3114,
        GoldStealer         = 3115,
        SixGacha            = 3116,
        NightCity           = 3117,
        DetectiveDee2       = 3118,
        MahJongKing         = 3119,
        SongkranFestival    = 3120,
        UproarInHeaven      = 3121,
        DaFaCai             = 3122,
        DaHongZhong         = 3123,
        GoodFortune         = 3146,
        GoodFortuneM        = 3147,
        FootballAllStar     = 3148,
        FootballBoots       = 3150,
        WonWonWon           = 3151,
        LionKing            = 3152,
        ThreePandas         = 3153,
        FootballJerseys     = 3154,
        OGFaFaFa            = 3155,
        Eight88CaiShen      = 3156,
        EcstaticCircus      = 3157,
        PharaohsGold        = 3158,
        SixToros            = 3160,
        SweetPop            = 3161,


        GuGuGu3             = 3002,
        FootballFever       = 3085,
        FootballFeverM      = 3086,
        HeroOfThreeKingdomsCaoCao = 3087,
        AladdinsLamp        = 3124,
        RunningToro         = 3125,
        FloatingMarket      = 3126,
        HappyMagpies        = 3127,
        AllStarTeam         = 3128,
        FootballStar        = 3129,
        SherlockHolmes      = 3130,
        TheCupids           = 3131,
        OoGaChaKa           = 3132,
        BurningXiYou        = 3133,
        NinjaRaccoon        = 3134,
        DollarBomb          = 3135,
        KingKongShake       = 3136,
        LoyKrathong         = 3137,
        FaCaiFuWa           = 3138,
        KingOfAtlantis      = 3139,
        Kronos              = 3140,
        DoubleFly           = 3141,
        HotSpin             = 3142,
        Eight88             = 3143,
        WheelMoney          = 3144,
        WolfMoon            = 3145,
        FaCaiShen2          = 3149,
        HotPinatas          = 3159,
        


        #endregion

        #region HABANERO Games
        MysticFortuneDeluxe         = 3501,//1
        FengHuang                   = 3502,//1
        TheKoiGate                  = 3503,//1
        DiscoBeats                  = 3504,//1
        Jump                        = 3505,//1
        FireRooster                 = 3506,//2
        FiveLuckyLions              = 3507,//2
        TabernaDeLosMuertosUltra    = 3508,//1
        PandaPanda                  = 3509,//1
        NineTails                   = 3510,//1
        FourDivineBeasts            = 3511,//2
        LaughingBuddha              = 3512,//1
        MightyMedusa                = 3513,//1
        SpaceGoonz                  = 3514,//1
        LuckyDurian                 = 3515,//1
        LanternLuck                 = 3516,//1
        Prost                       = 3517,//1
        NewYearsBash                = 3518,//1
        BeforeTimeRunsOut           = 3519,//1
        TotemTowers                 = 3520,//1
        TabernaDeLosMuertos         = 3521,//1
        WealthInn                   = 3522,//1
        HeySushi                    = 3523,//1
        GoldenUnicornDeluxe         = 3524,//1
        FaCaiShenDeluxe             = 3525,//1
        LuckyFortuneCat             = 3526,//2
        HappyApe                    = 3527,//2
        NaughtySanta                = 3528,//1
        HotHotHalloween             = 3529,//1
        ColossalGems                = 3530,//2
        Nuwa                        = 3531,//2
        LuckyLucky                  = 3532,//1
        MountMazuma                 = 3533,//1
        TaikoBeats                  = 3534,//1
        HotHotFruit                 = 3535,//1
        WaysOfFortune               = 3536,//1
        FiveMariachis               = 3537,//2
        ScruffyScallywags           = 3538,//2
        WeirdScience                = 3539,//1
        VikingsPlunder              = 3540,//1
        DrFeelgood                  = 3541,//1
        DoubleODollars              = 3542,//2
        LittleGreenMoney            = 3543,//2
        RodeoDrive                  = 3544,//1
        ShaolinFortunes100          = 3545,//2
        MonsterMashCash             = 3546,//2
        TheDragonCastle             = 3547,//1
        ShaolinFortunes243          = 3548,//2
        TreasureDiver               = 3549,//2
        KingTutsTomb                = 3550,//1
        CarnivalCash                = 3551,//2
        KanesInferno                = 3552,//2
        TreasureTomb                = 3553,//1
        HaZeus2                     = 3554,//2
        BuggyBonus                  = 3555,//2
        GalacticCash                = 3556,//2
        GlamRock                    = 3557,//2
        PamperMe                    = 3558,//1
        SpaceFortune                = 3559,//1
        HaZeus                      = 3560,//1
        PoolShark                   = 3561,//1
        SOS                         = 3562,//1
        TheDeadEscape               = 3563,//2
        ArcticWonders               = 3564,//2
        JungleRumble                = 3565,//1
        SuperStrike                 = 3566,//1
        BarnstormerBucks            = 3567,//1
        EgyptianDreams              = 3568,//1
        BikiniIsland                = 3569,//1
        MummyMoney                  = 3570,//1
        TowerOfPizza                = 3571,//1
        MysticFortune               = 3572,//2
        MrBling                     = 3573,//2
        AllForOne                   = 3574,//2
        FlyingHigh                  = 3575,//2
        DragonsRealm                = 3576,//2
        QueenOfQueens243            = 3577,//2
        QueenOfQueens1024           = 3578,//2
        CashReef                    = 3579,//2
        GoldenUnicorn               = 3580,//1
        PuckerUpPrince              = 3581,//2
        SirBlingalot                = 3582,//1
        IndianCashCatcher           = 3583,//2
        TheBigDeal                  = 3584,//2
        DiscoFunk                   = 3585,//1
        GoldRush                    = 3586,//1
        Cashosaurus                 = 3587,//1
        SkysTheLimit                = 3588,//1
        WickedWitch                 = 3589,//1
        BombsAway                   = 3590,//2
        RuffledUp                   = 3591,//1
        RomanEmpire                 = 3592,//1
        HaFaCaiShen                 = 3593,//1
        DragonsThrone               = 3594,//1
        BirdOfThunder               = 3595,//1
        CoyoteCrash                 = 3596,//1
        Jugglenaut                  = 3597,//2
        OceansCall                  = 3598,//2
        SuperTwister                = 3599,//1
        TwelveZodiacs               = 3600,//2

        SantasInn                   = 3601,//1
        FruityHalloween             = 3602,//1
        SlimeParty                  = 3603,//1
        MeowJanken                  = 3604,//1
        BikiniIslandDeluxe          = 3605,//1
        WitchesTome                 = 3606,//1
        LegendOfNezha               = 3607,//1
        TootyFruityFruits           = 3608,//1
        SirensSpell                 = 3609,//1
        Crystopia                   = 3610,//1
        TheBigDealDeluxe            = 3611,//1
        NaughtyWukong               = 3612,//1
        LegendaryBeasts             = 3613,//1
        Rainbowmania                = 3614,//1
        DragonTigerGate             = 3615,//1
        SojuBomb                    = 3616,//1
        TukTukThailand              = 3617,//1

        #endregion

        #region PLAYSON games
        SolarQueen                  = 4001,
        SolarQueenMegaways          = 4002,
        BookDelSol                  = 4003,
        RoyalJoker                  = 4004,
        CoinStrike                  = 4005,
        GizaNights                  = 4006,
        MammothPeak                 = 4007,
        HitTheBank                  = 4008,
        TreasuresFire               = 4009,
        PirateChest                 = 4010,
        PirateSharky                = 4011,
        RoyalCoins2                 = 4012,
        UltraFort                   = 4013,
        RubyHit                     = 4014,
        LuxorGold                   = 4015,
        LionGems                    = 4016,
        BurningFort                 = 4017,
        WolfPowerMega               = 4018,
        JokersCoins                 = 4019,
        DiamondFort                 = 4020,
        BuffaloXmas                 = 4021,
        BurningWinsX2               = 4022,
        RoyalCoins                  = 4023,
        SpiritOfEgypt               = 4024,
        EaglePower                  = 4025,
        FiveFortunator              = 4026,
        HotBurningWins              = 4027,
        NineHappyPharaohs           = 4028,
        BuffaloMegaways             = 4029,
        BookOfGoldDh                = 4030,
        RichDiamonds                = 4031,
        DivineDragon                = 4032,
        WolfPower                   = 4033,
        HandOfGold                  = 4034,
        ThreeFruitsWin2Hit          = 4035,
        HotCoins                    = 4036,
        StarsNFruits2Hit            = 4037,
        FiveSuperSevensNFruits6     = 4038,
        SuperSunnyFruits            = 4039,
        LegendOfCleopatraMega       = 4040,
        SolarKing                   = 4041,
        DiamondWins                 = 4042,
        RiseOfEgyptDeluxe           = 4043,
        BookOfGoldMultichance       = 4044,
        
        PearlBeauty                 = 4045,
        BuffaloPower                = 4046,
        SevensNFruits6              = 4047,
        SolarTemple                 = 4048,
        SunnyFruits                 = 4049,
        SuperBurningWinsRespin      = 4051,
        ImperialFruits100           = 4054,
        ImperialFruits40            = 4055,
        BookOfGoldClassic           = 4056,
        ThreeFruitsWin10            = 4057,
        FruitsAndJokers100          = 4058,
        VikingsFortune              = 4059,
        ImperialFruits5             = 4060,
        SevensNFruits20             = 4061,
        FruitsAndClovers20          = 4062,
        MightyAfrica                = 4063,
        HundredJokerStaxx           = 4064,
        WildWarriors                = 4065,
        FruitsAndJokers40           = 4066,
        BookOfGold                  = 4067,
        JokerExpand40               = 4068,
        FruitsAndJokers20           = 4069,
        SuperBurningWins            = 4070,
        RiseOfEgypt                 = 4071,
        JokerExpand                 = 4072,
        BurningWins                 = 4073,
        FourtyJokerStaxx            = 4074,
        SevensNFruits               = 4075,
        LegendOfCleopatra           = 4076,
        FruitsNStarsC               = 4077,
        JuiceAndFruitsC             = 4078,
        BuffaloPower2               = 4079,
        RoyalFort                   = 4080,
        EmpireGold                  = 4081,
        WolfLand                    = 4082,


        BookOfGoldChoice            = 4052,     //옵션게임
        CloverRiches                = 4053,     //옵션게임
        RomeCaesarsGlory            = 4050,     //프로그레스스트레인지
        #endregion
    }

    public enum CSMSG_CODE : ushort
    {
        CS_HEARTBEAT        = 0,
        CS_LOGIN            = 1,
        CS_ENTERGAME        = 18,
        CS_FORCEOUTUSER     = 26,   //유저강퇴



        CS_SLOTGAMESTART        = 2000,
        
        CS_PP_DOINIT            = 2000,
        CS_PP_DOSPIN            = 2001,
        CS_PP_DOCOLLECT         = 2002,
        CS_PP_RELOADBALANCE     = 2003,
        CS_PP_NOTPROCDRESULT    = 2004,
        CS_PP_SAVESETTING       = 2005,
        CS_PP_DOBONUS           = 2006,
        CS_PP_DOMYSTERYSCATTER  = 2007,
        CS_PP_FSOPTION          = 2008,
        CS_PP_DOCOLLECTBONUS    = 2009,
        CS_SLOTGAMEEND          = 2009, 
        
        CS_PP_PROMOACTIVE       = 2020, 
        CS_PP_PROMOSTART        = 2020, 
        CS_PP_PROMOTOURDETAIL   = 2021, 
        CS_PP_PROMORACEDETAIL   = 2022, 
        CS_PP_PROMOTOURLEADER   = 2023, 
        CS_PP_PROMORACEPRIZES   = 2024, 
        CS_PP_PROMORACEWINNER   = 2025, 
        CS_PP_PROMOV3TOURLEADER = 2026, 
        CS_PP_PROMOV2RACEWINNER = 2027, 
        CS_PP_PROMOTOUROPTIN    = 2028, 
        CS_PP_PROMORACEOPTIN    = 2029, 
        CS_PP_PROMOSCORES       = 2030, 
        CS_PP_GETMINILOBBY      = 2031, 
        CS_PP_PROMOEND          = 2031, 

        CS_BNGSLOTGAMESTART     = 2100,
        CS_BNG_DOLOGIN          = 2100,
        CS_BNG_DOSTART          = 2101,
        CS_BNG_DOSYNC           = 2102,
        CS_BNG_DOPLAY           = 2103,
        CS_BNGSLOTGAMEEND       = 2103,

        CS_PLAYSONSLOTGAMESTART = 2200,
        CS_PLAYSON_DOCONNECT    = 2200,
        CS_PLAYSON_DOSTART      = 2201,
        CS_PLAYSON_DORECONNECT  = 2202,
        CS_PLAYSON_DOSYNC       = 2203,
        CS_PLAYSON_DOPLAY       = 2204,
        CS_PLAYSONSLOTGAMEEND   = 2204,

        CS_AMATICSLOTGAMESTART  = 2300,
        CS_AMATIC_DOINIT        = 2300,
        CS_AMATIC_DOSPIN        = 2301,
        CS_AMATIC_DOCOLLECT     = 2302,
        CS_AMATIC_DOGAMBLEPICK  = 2303,
        CS_AMATIC_DOGAMBLEHALF  = 2304,
        CS_AMATIC_DOHEARTBEAT   = 2305,
        CS_AMATIC_FSOPTION      = 2306,
        CS_AMATICSLOTGAMEEND    = 2306,

        CS_CQ9_START                    = 3000,
        CS_CQ9_InitGame1Request         = 3011,
        CS_CQ9_InitGame2Request         = 3012,

        CS_CQ9_NormalSpinRequest        = 3031,
        CS_CQ9_CollectRequest           = 3032,
        CS_CQ9_TembleSpinRequest        = 3033,
        CS_CQ9_FreeSpinStartRequest     = 3041,
        CS_CQ9_FreeSpinRequest          = 3042,
        CS_CQ9_FreeSpinSumRequest       = 3043,
        CS_CQ9_FreeSpinOptionRequest    = 3044,
        CS_CQ9_FreeSpinOptSelectRequest = 3045,
        CS_CQ9_FreeSpinOptResultRequest = 3046,
        CS_CQ9_END                      = 3200,

        CS_HABANERO_SLOTSTART           = 3300,
        CS_HABANERO_DOINIT              = 3300,
        CS_HABANERO_DOBALANCE           = 3301,
        CS_HABANERO_DOSPIN              = 3302,
        CS_HABANERO_DOCLIENT            = 3303,
        CS_HABANERO_DOPICKGAMEDONEPAY   = 3304,
        CS_HABANERO_DOUPDATECLIENTPICK  = 3305, 
        CS_HABANERO_SLOTEND             = 3305,

    }

    public enum SCMSG_CODE : ushort
    {
        SC_LOGIN                            = 1,
        SC_ENTERGAME                        = 18,
        SC_PP_DOINIT                        = 2000,
        SC_PP_DOSPIN                        = 2001,
        SC_PP_DOCOLLECT                     = 2002,
        SC_PP_RELOADBALANCE                 = 2003,
        SC_PP_SAVESETTING                   = 2004,
        SC_PP_DOBONUS                       = 2005,
        SC_PP_DOMYSTERYSCATTER              = 2006,
        SC_PP_DOCOLLECTBONUS                = 2007,

        SC_PP_PROMOACTIVE                   = 2020, 
        SC_PP_PROMOTOURDETAIL               = 2021, 
        SC_PP_PROMORACEDETAIL               = 2022, 
        SC_PP_PROMOTOURLEADER               = 2023, 
        SC_PP_PROMORACEPRIZES               = 2024, 
        SC_PP_PROMORACEWINNER               = 2025, 
        SC_PP_PROMOTOUROPTIN                = 2026, 
        SC_PP_PROMORACEOPTIN                = 2027, 
        SC_PP_PROMOTOURSCORE                = 2028, 
        SC_PP_GETMINILOBBY                  = 2029, 

        SC_BNG_DOLOGIN                      = 2100,
        SC_BNG_DOSTART                      = 2101,
        SC_BNG_DOSYNC                       = 2102,
        SC_BNG_DOPLAY                       = 2103,

        SC_PLAYSONSLOTGAMESTART             = 2200,
        SC_PLAYSON_DOCONNECT                = 2200,
        SC_PLAYSON_DOSTART                  = 2201,
        SC_PLAYSON_DORECONNECT              = 2201,
        SC_PLAYSON_DOSYNC                   = 2202,
        SC_PLAYSON_DOPLAY                   = 2203,
        SC_PLAYSONSLOTGAMEEND               = 2203,

        SC_AMATICSLOTGAMESTART              = 2300,
        SC_AMATIC_DOINIT                    = 2300,
        SC_AMATIC_DOSPIN                    = 2301,
        SC_AMATIC_DOCOLLECT                 = 2302,
        SC_AMATIC_DOGAMBLEPICK              = 2303,
        SC_AMATIC_DOGAMBLEHALF              = 2304,
        SC_AMATIC_DOHEARTBEAT               = 2305,
        SC_AMATIC_FSOPTION                  = 2306,
        SC_AMATICSLOTGAMEEND                = 2306,

        SC_CQ9_InitGame1Response            = 3111,
        SC_CQ9_InitGame2Response            = 3112,
        SC_CQ9_NormalSpinResponse           = 3131,
        SC_CQ9_CollectResponse              = 3132,
        SC_CQ9_TemlbleSpinResponse          = 3133,
        SC_CQ9_FreeSpinStartResponse        = 3141,
        SC_CQ9_FreeSpinResponse             = 3142,
        SC_CQ9_FreeSpinSumResponse          = 3143,
        SC_CQ9_FreeSpinOptionResponse       = 3144,
        SC_CQ9_FreeSpinOptSelectResponse    = 3145,
        SC_CQ9_FreeSpinOptResultResponse    = 3146,

        SC_HABANERO_SLOTSTART               = 3300,
        SC_HABANERO_DOINIT                  = 3300,
        SC_HABANERO_DOBALANCE               = 3301,
        SC_HABANERO_DOSPIN                  = 3302,
        SC_HABANERO_DOCLIENT                = 3303,
        SC_HABANERO_DOPICKGAMEDONEPAY       = 3304,
        SC_HABANERO_DOUPDATECLIENTPICK      = 3305,
    }
}
