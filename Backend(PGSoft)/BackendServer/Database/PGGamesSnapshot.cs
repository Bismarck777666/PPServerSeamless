using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using Newtonsoft.Json;

namespace SlotGamesNode.Database
{
    class PGGamesSnapshot
    {
        private static PGGamesSnapshot _sInstance = new PGGamesSnapshot();
        public static PGGamesSnapshot Instance
        {
            get
            {
                return _sInstance;
            }
        }

        public Dictionary<string, GAMEID> _dicGameStringToIDs = new Dictionary<string, GAMEID>();

        public PGGamesSnapshot()
        {
            _dicGameStringToIDs.Add("lobby",                GAMEID.Lobby);            
            _dicGameStringToIDs.Add("treasures-aztec",      GAMEID.TreasureOfAztec);
            _dicGameStringToIDs.Add("lucky-neko",           GAMEID.LuckyNeko);
            _dicGameStringToIDs.Add("mahjong-ways2",        GAMEID.MahjongWays2);
            _dicGameStringToIDs.Add("mahjong-ways",         GAMEID.MahjongWays);
            _dicGameStringToIDs.Add("ways-of-qilin",        GAMEID.WaysOfTheQilin);
            _dicGameStringToIDs.Add("wild-bounty-sd",       GAMEID.WildBountyShowdown);
            _dicGameStringToIDs.Add("wild-bandito",         GAMEID.WildBandito);
            _dicGameStringToIDs.Add("fortune-ox",           GAMEID.FortuneOx);
            _dicGameStringToIDs.Add("ganesha-fortune",      GAMEID.GaneshaFortune);
            _dicGameStringToIDs.Add("asgardian-rs",         GAMEID.AsgardianRising);
            _dicGameStringToIDs.Add("cai-shen-wins",        GAMEID.CaishenWins);
            _dicGameStringToIDs.Add("legend-perseus",       GAMEID.LegendOfPerseus);
            _dicGameStringToIDs.Add("cocktail-nite",        GAMEID.CocktailNights);
            _dicGameStringToIDs.Add("diner-delights",       GAMEID.DinnerDelights);
            _dicGameStringToIDs.Add("midas-fortune",        GAMEID.MidasFortune);
            _dicGameStringToIDs.Add("fortune-tiger",        GAMEID.FortuneTiger);
            _dicGameStringToIDs.Add("thai-river",           GAMEID.ThaiRiverWonders);
            _dicGameStringToIDs.Add("jurassic-kdm",         GAMEID.JurassicKingdom);
            _dicGameStringToIDs.Add("prosper-ftree",        GAMEID.ProsperityFortuneTree);
            _dicGameStringToIDs.Add("egypts-book-mystery",  GAMEID.EgyptBookOfMystery);
            _dicGameStringToIDs.Add("queen-bounty",         GAMEID.QueenOfBounty);
            _dicGameStringToIDs.Add("captains-bounty",      GAMEID.CaptainBounty);
            _dicGameStringToIDs.Add("dragon-hatch",         GAMEID.DragonHatch);
            _dicGameStringToIDs.Add("dreams-of-macau",      GAMEID.DreamsOfMacau);
            _dicGameStringToIDs.Add("leprechaun-riches",    GAMEID.LeprechaunRiches);
            _dicGameStringToIDs.Add("the-great-icescape",   GAMEID.TheGreatIcescape);
            _dicGameStringToIDs.Add("speed-winner",         GAMEID.SpeedWinner);
            _dicGameStringToIDs.Add("lucky-piggy",          GAMEID.LuckyPiggy);
            _dicGameStringToIDs.Add("queen-banquet",        GAMEID.TheQueenBanquet);
            _dicGameStringToIDs.Add("wild-coaster",         GAMEID.WildCoaster);
            _dicGameStringToIDs.Add("btrfly-blossom",       GAMEID.ButterflyBlossom);
            _dicGameStringToIDs.Add("crypto-gold",          GAMEID.CryptoGold);
            _dicGameStringToIDs.Add("double-fortune",       GAMEID.DoubleFortune);
            _dicGameStringToIDs.Add("fortune-mouse",        GAMEID.FortuneMouse);
            _dicGameStringToIDs.Add("oriental-pros",        GAMEID.OrientalProsperity);
            _dicGameStringToIDs.Add("phoenix-rises",        GAMEID.PhoenixRises);
            _dicGameStringToIDs.Add("rise-of-apollo",       GAMEID.RiseOfApollo);
            _dicGameStringToIDs.Add("rooster-rbl",          GAMEID.RoosterRumble);
            _dicGameStringToIDs.Add("shaolin-soccer",       GAMEID.ShaolinSoccer);
            _dicGameStringToIDs.Add("wild-fireworks",       GAMEID.WildFireworks);
            _dicGameStringToIDs.Add("spirit-wonder",        GAMEID.SpiritedWonders);
            _dicGameStringToIDs.Add("majestic-ts",          GAMEID.MajesticTreasures);
            _dicGameStringToIDs.Add("garuda-gems",          GAMEID.GarudaGems);
            _dicGameStringToIDs.Add("sprmkt-spree",         GAMEID.SupermarketSpree);
            _dicGameStringToIDs.Add("muay-thai-champion",   GAMEID.MuayThaiChampion);
            _dicGameStringToIDs.Add("fortune-rabbit",       GAMEID.FortuneRabbit);
            _dicGameStringToIDs.Add("battleground",         GAMEID.BattlegroundRoyale);
            _dicGameStringToIDs.Add("sct-cleopatra",        GAMEID.SecretOfCleopatra);
            _dicGameStringToIDs.Add("alchemy-gold",         GAMEID.AlchemyGold);
            _dicGameStringToIDs.Add("candy-bonanza",        GAMEID.CandyBonanza);

            _dicGameStringToIDs.Add("totem-wonders",        GAMEID.TotemWonders);
            _dicGameStringToIDs.Add("emperors-favour",      GAMEID.EmperorsFavour);
            _dicGameStringToIDs.Add("ganesha-gold",         GAMEID.GaneshaGold);
            _dicGameStringToIDs.Add("heist-stakes",         GAMEID.HeistStakes);
            _dicGameStringToIDs.Add("galactic-gems",        GAMEID.GalacticGems);
            _dicGameStringToIDs.Add("jewels-prosper",       GAMEID.JewelsOfProsperity);
            _dicGameStringToIDs.Add("opera-dynasty",        GAMEID.OperaDynasty);
            _dicGameStringToIDs.Add("circus-delight",       GAMEID.CircusDelight);
            _dicGameStringToIDs.Add("gdn-ice-fire",         GAMEID.GuardiansOfIceAndFire);
            _dicGameStringToIDs.Add("dragon-tiger-luck",    GAMEID.DragonTigerLuck);
            _dicGameStringToIDs.Add("candy-burst",          GAMEID.CandyBurst);
            _dicGameStringToIDs.Add("genies-wishes",        GAMEID.Genies3Wishes);
            _dicGameStringToIDs.Add("gem-saviour-conquest", GAMEID.GemSaviourConquest);
            _dicGameStringToIDs.Add("dest-sun-moon",        GAMEID.DestinyOfSunAndMoon);
            _dicGameStringToIDs.Add("mermaid-riches",       GAMEID.MermaidRiches);
            _dicGameStringToIDs.Add("win-win-won",          GAMEID.WinWinWon);
            _dicGameStringToIDs.Add("fortune-gods",         GAMEID.FortuneGods);
            _dicGameStringToIDs.Add("dragon-legend",        GAMEID.DragonLegend);
            _dicGameStringToIDs.Add("lgd-monkey-kg",        GAMEID.LegendaryMonkeyKing);
            _dicGameStringToIDs.Add("journey-to-the-wealth",GAMEID.JourneyToTheWealth);

            _dicGameStringToIDs.Add("bikini-paradise",      GAMEID.BikiniParadise);
            _dicGameStringToIDs.Add("crypt-fortune",        GAMEID.RaiderJanesCryptOfFortune);
            _dicGameStringToIDs.Add("win-win-fpc",          GAMEID.WinWinFishPrawnCrab);
            _dicGameStringToIDs.Add("jungle-delight",       GAMEID.JungleDelight);
            _dicGameStringToIDs.Add("buffalo-win",          GAMEID.BuffaloWin);
            _dicGameStringToIDs.Add("piggy-gold",           GAMEID.PiggyGold);
            _dicGameStringToIDs.Add("plushie-frenzy",       GAMEID.PlushieFrenzy);
            _dicGameStringToIDs.Add("bali-vacation",        GAMEID.BaliVacation);
            _dicGameStringToIDs.Add("hawaiian-tiki",        GAMEID.HawaiianTiki);
            _dicGameStringToIDs.Add("flirting-scholar",     GAMEID.FlirtingScholar);
            _dicGameStringToIDs.Add("ninja-vs-samurai",     GAMEID.NinjaVsSamurai);
            _dicGameStringToIDs.Add("diaochan",             GAMEID.HoneyTrapOfDiaoChan);
            _dicGameStringToIDs.Add("medusa2",              GAMEID.Medusa2);
            _dicGameStringToIDs.Add("legend-of-hou-yi",     GAMEID.LegendOfHouYi);
            _dicGameStringToIDs.Add("mask-carnival",        GAMEID.MaskCarnival);
            _dicGameStringToIDs.Add("emoji-riches",         GAMEID.EmojiRiches);
            _dicGameStringToIDs.Add("jack-frosts",          GAMEID.JackFrostsWinter);
            _dicGameStringToIDs.Add("mr-hallow-win",        GAMEID.MrHallowWin);
            _dicGameStringToIDs.Add("prosperity-lion",      GAMEID.ProsperityLion);
            _dicGameStringToIDs.Add("reel-love",            GAMEID.ReelLove);
            
            _dicGameStringToIDs.Add("bakery-bonanza",       GAMEID.BakeryBonanza);
            _dicGameStringToIDs.Add("santas-gift-rush",     GAMEID.SantasGiftRush);
            _dicGameStringToIDs.Add("hip-hop-panda",        GAMEID.HipHopPanda);
            _dicGameStringToIDs.Add("medusa",               GAMEID.Medusa);
            _dicGameStringToIDs.Add("vampires-charm",       GAMEID.VampiresCharm);
            _dicGameStringToIDs.Add("gem-saviour-sword",    GAMEID.GemSaviourSword);
            _dicGameStringToIDs.Add("rave-party-fvr",       GAMEID.RavePartyFever);
            _dicGameStringToIDs.Add("songkran-spl",         GAMEID.SongkranSplash);
            _dicGameStringToIDs.Add("spr-golf-drive",       GAMEID.SuperGolfDrive);
            _dicGameStringToIDs.Add("myst-spirits",         GAMEID.MysticalSpirits);

            _dicGameStringToIDs.Add("fruity-candy",         GAMEID.FruityCandy);
            _dicGameStringToIDs.Add("lucky-clover",         GAMEID.LuckyCloverLady);
            _dicGameStringToIDs.Add("cruise-royale",        GAMEID.CruiseRoyale);
            _dicGameStringToIDs.Add("safari-wilds",         GAMEID.SafariWilds);
            _dicGameStringToIDs.Add("gladi-glory",          GAMEID.GladiatorsGlory);
            _dicGameStringToIDs.Add("ninja-raccoon",        GAMEID.NinjaRaccoonFrenzy);
            _dicGameStringToIDs.Add("ult-striker",          GAMEID.UltimateStriker);
            _dicGameStringToIDs.Add("wild-heist-co",        GAMEID.WildHeistCashout);
            _dicGameStringToIDs.Add("forge-wealth",         GAMEID.ForgeOfWealth);
            _dicGameStringToIDs.Add("mafia-mayhem",         GAMEID.MafiaMayhem);
            _dicGameStringToIDs.Add("tsar-treasures",       GAMEID.TsarTreasures);
            _dicGameStringToIDs.Add("werewolf-hunt",        GAMEID.WereWolfsHunt);
            _dicGameStringToIDs.Add("dragon-hatch2",        GAMEID.DragonHatch2);
            _dicGameStringToIDs.Add("fortune-dragon",       GAMEID.FortuneDragon);

            

        }

        public string findGameStringFromID(int id)
        {
            foreach(KeyValuePair<string, GAMEID> pair in _dicGameStringToIDs)
            {
                if ((int) pair.Value == id)
                    return pair.Key;
            }
            return null;
        }
        public GAMEID findGameIDFromString(string strGameID)
        {
            if (_dicGameStringToIDs.ContainsKey(strGameID))
                return _dicGameStringToIDs[strGameID];

            return GAMEID.None;
        }
        public List<GAMEID> getAllGameIDs()
        {
            return new List<GAMEID>(_dicGameStringToIDs.Values);
        }
    }    
}
