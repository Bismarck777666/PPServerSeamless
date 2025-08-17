using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GITProtocol
{
    public class GITMessage
    {
        public List<object> DataArray
        {
            get;
            private set;
        }
        public List<TypeCode> DataTypes
        {
            get;
            private set;
        }
        public int DataNum
        {
            get
            {
                return DataArray.Count;
            }
        }

        public MsgCodes MsgCode
        {
            get;
            private set;
        }

        public GITMessage(MsgCodes msgCode)
        {
            this.MsgCode    = msgCode;
            this.DataArray  = new List<object>();
            this.DataTypes  = new List<TypeCode>();
        }

        public void Append(object value)
        {
            DataArray.Add(value);
            TypeCode tcode = Type.GetTypeCode(value.GetType());
            DataTypes.Add(tcode);
        }

        public void ClearData()
        {
            DataArray.Clear();
            DataTypes.Clear();
        }

        public object GetData(int i)
        {
            if (i >= this.DataNum)
            {
                throw (new Exception("index exceeds limit."));
            }
            return DataArray[i];
        }
        public object Pop()
        {
            if (this.DataNum == 0)
                throw new Exception("There are no data to pop");

            object obj = GetDataObject(0);

            DataArray.RemoveAt(0);
            DataTypes.RemoveAt(0);
            return obj;
        }

        protected object GetDataObject(int index)
        {
            object obj = (object)DataArray[index];
            if (DataTypes[index] == TypeCode.Byte)
                obj = Convert.ToByte(obj);
            else if (DataTypes[index] == TypeCode.UInt16)
                obj = Convert.ToUInt16(obj);
            else if (DataTypes[index] == TypeCode.Int64)
                obj = Convert.ToInt64(obj);

            return obj;
        }
        public object Pop(int i)
        {
            if (i >= this.DataNum)
            {
                throw new Exception("index exceeds limit.");
            }

            object obj = GetDataObject(i);
            DataArray.RemoveAt(i);
            DataTypes.RemoveAt(i);
            return obj;
        }
        public object PopLast()
        {
            int i = this.DataNum - 1;
            return Pop(i);
        }
        public void ResetData()
        {
            DataArray.Clear();
        }
    }
    public enum GAMEID : int
    {
        None                = -1,
        Lobby               = 0,
        TreasureOfAztec     = 87,
        LuckyNeko           = 89,
        MahjongWays2        = 74,
        MahjongWays         = 65,
        WaysOfTheQilin      = 106,
        WildBountyShowdown  = 135,
        WildBandito         = 104,
        FortuneOx           = 98,
        GaneshaFortune      = 75,
        CaishenWins         = 71,
        AsgardianRising     = 1340277,
        LegendOfPerseus     = 128,
        CocktailNights      = 117,
        DinnerDelights      = 1372643,
        MidasFortune        = 1402846,
        FortuneTiger        = 126,
        ThaiRiverWonders    = 92,
        JurassicKingdom     = 110,
        ProsperityFortuneTree = 1312883,
        EgyptBookOfMystery  = 73,
        QueenOfBounty       = 84,
        CaptainBounty       = 54,
        DragonHatch         = 57,
        DreamsOfMacau       = 79,
        LeprechaunRiches    = 60,
        LuckyPiggy          = 130,
        SpeedWinner         = 127,
        TheGreatIcescape    = 53,
        TheQueenBanquet     = 120,
        WildCoaster         = 132,
        WildFireworks       = 83,
        ShaolinSoccer       = 67,
        OrientalProsperity  = 112,
        CryptoGold          = 103,
        PhoenixRises        = 82,
        RiseOfApollo        = 101,
        RoosterRumble       = 123,
        DoubleFortune       = 48,
        FortuneMouse        = 68,
        ButterflyBlossom    = 125,
        SpiritedWonders     = 119,
        MajesticTreasures   = 95,
        GarudaGems          = 122,
        SupermarketSpree    = 115,
        MuayThaiChampion    = 64,
        FortuneRabbit       = 1543462,
        BattlegroundRoyale  = 124,
        SecretOfCleopatra   = 90,
        AlchemyGold         = 1368367,
        CandyBonanza        = 100,
        TotemWonders        = 1338274,
        EmperorsFavour      = 44,
        GaneshaGold         = 42,
        HeistStakes         = 105,
        GalacticGems        = 86,
        JewelsOfProsperity      = 88,
        OperaDynasty            = 93,
        CircusDelight           = 80,
        GuardiansOfIceAndFire   = 91,
        DragonTigerLuck         = 63,
        CandyBurst              = 70,
        Genies3Wishes           = 85,
        GemSaviourConquest      = 62,
        DestinyOfSunAndMoon     = 121,
        MermaidRiches           = 102,
        WinWinWon               = 24,
        FortuneGods             = 3,
        DragonLegend            = 29,
        LegendaryMonkeyKing     = 107,
        JourneyToTheWealth      = 50,

        BikiniParadise              = 69,
        RaiderJanesCryptOfFortune   = 113,
        WinWinFishPrawnCrab         = 129,
        JungleDelight               = 40,
        BuffaloWin                  = 108,
        PiggyGold                   = 39,
        PlushieFrenzy               = 25,
        BaliVacation                = 94,
        HawaiianTiki                = 1381200,
        FlirtingScholar             = 61,
        NinjaVsSamurai              = 59,
        ReelLove                    = 20,
        HoneyTrapOfDiaoChan         = 1,
        Medusa2                     = 6,
        LegendOfHouYi               = 34,
        MaskCarnival                = 118,
        EmojiRiches                 = 114,
        JackFrostsWinter            = 97,
        MrHallowWin                 = 35,
        ProsperityLion              = 36,

        BakeryBonanza               = 1418544,
        SantasGiftRush              = 37,
        HipHopPanda                 = 33,
        Medusa                      = 7,
        VampiresCharm               = 58,
        GemSaviourSword             = 38,
        RavePartyFever              = 1420892,
        MysticalSpirits             = 1432733,
        SongkranSplash              = 1448762,
        SuperGolfDrive              = 1513328,

        LuckyCloverLady             = 1601012,
        FruityCandy                 = 1397455,
        CruiseRoyale                = 1473388,
        SafariWilds                 = 1594259,
        GladiatorsGlory             = 1572362,
        NinjaRaccoonFrenzy          = 1529867,
        UltimateStriker             = 1489936,
        WildHeistCashout            = 1568554,
        ForgeOfWealth               = 1555350,
        MafiaMayhem                 = 1580541,
        TsarTreasures               = 1655268,
        WereWolfsHunt               = 1615454,
        DragonHatch2                = 1451122,
        FortuneDragon               = 1695365,
    }

    public enum MsgCodes : ushort
    {
        HEARTBEAT                   = 0,
        ENTERGAME                   = 1,
        GETGAMENAME                 = 2,
        GETGAMERESOURCE             = 3,
        SPIN                        = 4,
        GETHISTORYSUMMARY           = 5,
        GETHISTORYITEMS             = 6,
        GETGAMERULE                 = 7,
        GETGAMEWALLET               = 8,
        GETWEBLOBBYGET              = 9,
        GETINITTOURNAMENTS          = 10,
        SELECTCHARACTER             = 11,
        UPDATEGAMEINFO              = 12,   
    }

}

