using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GITProtocol
{
    public enum GAMETYPE
    {
        NONE    = 0,
        PP  = 1,
        AP      = 2,
        ARISTO = 3,
        COUNT   = 3,
    }

    public enum GAMEID : ushort
    {
        #region AristoCrat Games
        ACGameStart             = 10,
        QueenOfNile             = 47,
        SaharaGold              = 557,
        DragonEmperor           = 568,
        PandaMagic              = 608,
        HappyProsperous         = 609,
        PeaceLongLife           = 1000,
        Geisha                  = 72,
        LuckyCount              = 58,
        SunMoon                 = 50,
        Dragons50               = 42,
        Lions50                 = 44,
        DoubleHappiness         = 59,
        BigRed                  = 41,
        BigBen                  = 53,
        Buffalo                 = 51,
        MissKitty               = 43,
        TikiTorch               = 46,
        FlameofOlympus          = 57,
        JaguarMist              = 339,
        PelicanPete             = 340,
        DolphinsTreasure        = 341,
        MoonFestival            = 343,
        FireLight               = 344,
        WerewolfWild            = 338,
        Pompeii                 = 388,
        SilkRoad                = 393,
        WildSplash              = 391,
        TigerPrincess           = 394,
        FortunePrincess         = 395,
        HighStakes              = 555,
        HeartThrob              = 564,
        GoldenCentury           = 610,
        ACGameEnd = 2400,
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
        
        CS_PP_PROMOSTART        = 2020,
        CS_PP_PROMOACTIVE       = 2020,
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
        CS_PP_PROMOANNOUNCEMENT = 2031,
        CS_PP_GETMINILOBBY      = 2032,
        CS_PP_PROMOEND          = 2032,


    }
    public enum SCMSG_CODE : ushort
    {
        SC_LOGIN                = 1,
        SC_ENTERGAME            = 18,
        SC_PP_DOINIT            = 2000,
        SC_PP_DOSPIN            = 2001,
        SC_PP_DOCOLLECT         = 2002,
        SC_PP_RELOADBALANCE     = 2003,
        SC_PP_SAVESETTING       = 2004,
        SC_PP_DOBONUS           = 2005,
        SC_PP_DOMYSTERYSCATTER  = 2006,
        SC_PP_DOCOLLECTBONUS    = 2007,

        SC_PP_PROMOACTIVE       = 2020,
        SC_PP_PROMOTOURDETAIL   = 2021,
        SC_PP_PROMORACEDETAIL   = 2022,
        SC_PP_PROMOTOURLEADER   = 2023,
        SC_PP_PROMORACEPRIZES   = 2024,
        SC_PP_PROMORACEWINNER   = 2025,
        SC_PP_PROMOTOUROPTIN    = 2026,
        SC_PP_PROMORACEOPTIN    = 2027,
        SC_PP_PROMOANNOUNCEMENT = 2031,
    }
    public enum EventStatus
    {
        All         = 0, //'All'
        Pending     = 1, //'P',
        Activated   = 2, //'S',
        Opened      = 3, //'O',
        Closed      = 4, //'C',
        Cancelled   = 5, //'CC',
        Completed   = 6, //'CO'
    }

}
