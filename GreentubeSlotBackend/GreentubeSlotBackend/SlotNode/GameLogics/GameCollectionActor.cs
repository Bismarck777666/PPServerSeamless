using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using GITProtocol;
using SlotGamesNode.Database;
using Akka.Event;
using MongoDB.Bson;

namespace SlotGamesNode.GameLogics
{
    public class GameCollectionActor : ReceiveActor
    {
        private IActorRef                       _dbReader;
        private IActorRef                       _dbWriter;
        private IActorRef                       _redisWriter;
        private Dictionary<GAMEID, IActorRef>   _dicGameLogicActors = new Dictionary<GAMEID, IActorRef>();
        private HashSet<string>                 _hashAllChildActors = new HashSet<string>();
        protected readonly ILoggingAdapter      _logger             = Logging.GetLogger(Context);

        public GameCollectionActor(IActorRef dbReader, IActorRef dbWriter, IActorRef redisWriter)
        {
            _dbReader    = dbReader;
            _dbWriter    = dbWriter;
            _redisWriter = redisWriter;

            Receive<EnterGameRequest>               (_ => onEnterGameUser(_));
            Receive<string>                         (_ => processCommand(_));
            Receive<PayoutConfigUpdated>            (_ => onPayoutConfigUpdated(_));
            Receive<Terminated>                     (_ => onTerminated(_));
            ReceiveAsync<LoadSpinDataRequest>       (onLoadSpinDatabase);
            ReceiveAsync<PerformanceTestRequest>    (onPerformanceTest);
        }

        protected override void PreStart()
        {
            base.PreStart();
            createGameLogicActors();
        }

        protected void createGameLogicActors()
        {
            #region Greentube games
            _dicGameLogicActors.Add(GAMEID.SizzlingHotDeluxe, Context.ActorOf(Props.Create(() => new SizzlingHotDeluxeGameLogic()), "SizzlingHotDeluxe"));
            _dicGameLogicActors.Add(GAMEID.BookOfRaDeluxe, Context.ActorOf(Props.Create(() => new BookOfRaDeluxeGameLogic()), "BookOfRaDeluxe"));
            _dicGameLogicActors.Add(GAMEID.Katana, Context.ActorOf(Props.Create(() => new KatanaGameLogic()), "Katana"));
            _dicGameLogicActors.Add(GAMEID.BookOfRaMagic, Context.ActorOf(Props.Create(() => new BookOfRaMagicGameLogic()), "BookOfRaMagic"));
            _dicGameLogicActors.Add(GAMEID.FairyQueen, Context.ActorOf(Props.Create(() => new FairyQueenGameLogic()), "FairyQueen"));
            _dicGameLogicActors.Add(GAMEID.RoaringForties, Context.ActorOf(Props.Create(() => new RoaringFortiesGameLogic()), "RoaringForties"));
            _dicGameLogicActors.Add(GAMEID.LordOfTheOceanMagic, Context.ActorOf(Props.Create(() => new LordOfTheOceanMagicGameLogic()), "LordOfTheOceanMagic"));
            _dicGameLogicActors.Add(GAMEID.JustJewelsDeluxe, Context.ActorOf(Props.Create(() => new JustJewelsDeluxeGameLogic()), "JustJewelsDeluxe"));
            _dicGameLogicActors.Add(GAMEID.Faust, Context.ActorOf(Props.Create(() => new FaustGameLogic()), "Faust"));
            _dicGameLogicActors.Add(GAMEID.BookOfRaDeluxe6, Context.ActorOf(Props.Create(() => new BookOfRaDeluxe6GameLogic()), "BookOfRaDeluxe6"));
            _dicGameLogicActors.Add(GAMEID.SizzlingHot6ExtraGold, Context.ActorOf(Props.Create(() => new SizzlingHot6ExtraGoldGameLogic()), "SizzlingHot6ExtraGold"));
            _dicGameLogicActors.Add(GAMEID.AlwaysHotDeluxe, Context.ActorOf(Props.Create(() => new AlwaysHotDeluxeGameLogic()), "AlwaysHotDeluxe"));
            _dicGameLogicActors.Add(GAMEID.ColumbusDeluxe, Context.ActorOf(Props.Create(() => new ColumbusDeluxeGameLogic()), "ColumbusDeluxe"));
            _dicGameLogicActors.Add(GAMEID.CaptainVenture, Context.ActorOf(Props.Create(() => new CaptainVentureGameLogic()), "CaptainVenture"));
            _dicGameLogicActors.Add(GAMEID.MegaJoker, Context.ActorOf(Props.Create(() => new MegaJokerGameLogic()), "MegaJoker"));
            _dicGameLogicActors.Add(GAMEID.ChiliBomba, Context.ActorOf(Props.Create(() => new ChiliBombaGameLogic()), "ChiliBomba"));
            _dicGameLogicActors.Add(GAMEID.EyeOfTheDragon, Context.ActorOf(Props.Create(() => new EyeOfTheDragonGameLogic()), "EyeOfTheDragon"));
            _dicGameLogicActors.Add(GAMEID.BookOfRaClassic, Context.ActorOf(Props.Create(() => new BookOfRaClassicGameLogic()), "BookOfRaClassic"));
            _dicGameLogicActors.Add(GAMEID.LuckyLadysCharmDeluxe, Context.ActorOf(Props.Create(() => new LuckyLadysCharmDeluxeGameLogic()), "LuckyLadysCharmDeluxe"));
            _dicGameLogicActors.Add(GAMEID.DolphinsPearlDeluxe, Context.ActorOf(Props.Create(() => new DolphinsPearlDeluxeGameLogic()), "DolphinsPearlDeluxe"));
            _dicGameLogicActors.Add(GAMEID.Gorilla, Context.ActorOf(Props.Create(() => new GorillaGameLogic()), "Gorilla"));
            _dicGameLogicActors.Add(GAMEID.XtraHot, Context.ActorOf(Props.Create(() => new XtraHotGameLogic()), "XtraHot"));
            _dicGameLogicActors.Add(GAMEID.MysticSecrets, Context.ActorOf(Props.Create(() => new MysticSecretsGameLogic()), "MysticSecrets"));
            _dicGameLogicActors.Add(GAMEID.LuckyLadysCharmDeluxe6, Context.ActorOf(Props.Create(() => new LuckyLadysCharmDeluxe6GameLogic()), "LuckyLadysCharmDeluxe6"));
            _dicGameLogicActors.Add(GAMEID.LuckyLadysCharmDeluxe10, Context.ActorOf(Props.Create(() => new LuckyLadysCharmDeluxe10GameLogic()), "LuckyLadysCharmDeluxe10"));
            _dicGameLogicActors.Add(GAMEID.PlentyOnTwenty, Context.ActorOf(Props.Create(() => new PlentyOnTwentyGameLogic()), "PlentyOnTwenty"));
            _dicGameLogicActors.Add(GAMEID.RumpelWildspins, Context.ActorOf(Props.Create(() => new RumpelWildspinsGameLogic()), "RumpelWildspins"));
            _dicGameLogicActors.Add(GAMEID.DiamondLinkMightyBuffalo, Context.ActorOf(Props.Create(() => new DiamondLinkMightyBuffaloGameLogic()), "DiamondLinkMightyBuffalo"));
            _dicGameLogicActors.Add(GAMEID.GryphonsGoldDeluxe, Context.ActorOf(Props.Create(() => new GryphonsGoldDeluxeGameLogic()), "GryphonsGoldDeluxe"));
            _dicGameLogicActors.Add(GAMEID.HotChance, Context.ActorOf(Props.Create(() => new HotChanceGameLogic()), "HotChance"));
            _dicGameLogicActors.Add(GAMEID.HotCherry, Context.ActorOf(Props.Create(() => new HotCherryGameLogic()), "HotCherry"));
            _dicGameLogicActors.Add(GAMEID.UltraHotDeluxe, Context.ActorOf(Props.Create(() => new UltraHotDeluxeGameLogic()), "UltraHotDeluxe"));
            _dicGameLogicActors.Add(GAMEID.ShootingStars, Context.ActorOf(Props.Create(() => new ShootingStarsGameLogic()), "ShootingStars"));
            _dicGameLogicActors.Add(GAMEID.BookOfStars, Context.ActorOf(Props.Create(() => new BookOfStarsGameLogic()), "BookOfStars"));
            _dicGameLogicActors.Add(GAMEID.PharaohsRing, Context.ActorOf(Props.Create(() => new PharaohsRingGameLogic()), "PharaohsRing"));
            _dicGameLogicActors.Add(GAMEID.QueenOfHeartsDeluxe, Context.ActorOf(Props.Create(() => new QueenOfHeartsDeluxeGameLogic()), "QueenOfHeartsDeluxe"));
            _dicGameLogicActors.Add(GAMEID.SummerQueen, Context.ActorOf(Props.Create(() => new SummerQueenGameLogic()), "SummerQueen"));
            _dicGameLogicActors.Add(GAMEID.BookOfRaDeluxe10Winways, Context.ActorOf(Props.Create(() => new BookOfRaDeluxe10WinwaysGameLogic()), "BookOfRaDeluxe10Winways"));
            _dicGameLogicActors.Add(GAMEID.TopOTheMoneyPotsOfWealth, Context.ActorOf(Props.Create(() => new TopOTheMoneyPotsOfWealthGameLogic()), "TopOTheMoneyPotsOfWealth"));
            _dicGameLogicActors.Add(GAMEID.LoneStarJackpot, Context.ActorOf(Props.Create(() => new LoneStarJackpotGameLogic()), "LoneStarJackpot"));
            _dicGameLogicActors.Add(GAMEID.SpringQueen, Context.ActorOf(Props.Create(() => new SpringQueenGameLogic()), "SpringQueen"));
            _dicGameLogicActors.Add(GAMEID.DolphinsPearlClassic, Context.ActorOf(Props.Create(() => new DolphinsPearlClassicGameLogic()), "DolphinsPearlClassic"));
            _dicGameLogicActors.Add(GAMEID.SizzlingHotDeluxe10, Context.ActorOf(Props.Create(() => new SizzlingHotDeluxe10GameLogic()), "SizzlingHotDeluxe10"));
            _dicGameLogicActors.Add(GAMEID.WinterQueen, Context.ActorOf(Props.Create(() => new WinterQueenGameLogic()), "WinterQueen"));
            _dicGameLogicActors.Add(GAMEID.GoldenArk, Context.ActorOf(Props.Create(() => new GoldenArkGameLogic()), "GoldenArk"));
            _dicGameLogicActors.Add(GAMEID.DiamondMysteryFruitiliciousDeluxe, Context.ActorOf(Props.Create(() => new DiamondMysteryFruitiliciousDeluxeGameLogic()), "DiamondMysteryFruitiliciousDeluxe"));
            _dicGameLogicActors.Add(GAMEID.EmpireOfDead, Context.ActorOf(Props.Create(() => new EmpireOfDeadGameLogic()), "EmpireOfDead"));
            _dicGameLogicActors.Add(GAMEID.MagnificentMerlin, Context.ActorOf(Props.Create(() => new MagnificentMerlinGameLogic()), "MagnificentMerlin"));
            _dicGameLogicActors.Add(GAMEID.GreatFortune, Context.ActorOf(Props.Create(() => new GreatFortuneGameLogic()), "GreatFortune"));
            _dicGameLogicActors.Add(GAMEID.CashConnectionCharmingLady, Context.ActorOf(Props.Create(() => new CashConnectionCharmingLadyGameLogic()), "CashConnectionCharmingLady"));
            _dicGameLogicActors.Add(GAMEID.CashConnectionBookOfRa, Context.ActorOf(Props.Create(() => new CashConnectionBookOfRaGameLogic()), "CashConnectionBookOfRa"));
            _dicGameLogicActors.Add(GAMEID.CashConnectionDolphinsPearl, Context.ActorOf(Props.Create(() => new CashConnectionDolphinsPearlGameLogic()), "CashConnectionDolphinsPearl"));
            _dicGameLogicActors.Add(GAMEID.CashConnectionGoldenBookOfRa, Context.ActorOf(Props.Create(() => new CashConnectionGoldenBookOfRaGameLogic()), "CashConnectionGoldenBookOfRa"));
            _dicGameLogicActors.Add(GAMEID.GreatAmericanWilds, Context.ActorOf(Props.Create(() => new GreatAmericanWildsGameLogic()), "GreatAmericanWilds"));
            _dicGameLogicActors.Add(GAMEID.CharmingLadysBoom, Context.ActorOf(Props.Create(() => new CharmingLadysBoomGameLogic()), "CharmingLadysBoom"));
            _dicGameLogicActors.Add(GAMEID.PrizeOfTheNile, Context.ActorOf(Props.Create(() => new PrizeOfTheNileGameLogic()), "PrizeOfTheNile"));
            _dicGameLogicActors.Add(GAMEID.WelcomeFortune, Context.ActorOf(Props.Create(() => new WelcomeFortuneGameLogic()), "WelcomeFortune"));
            _dicGameLogicActors.Add(GAMEID.TheGreatGambinisNightMagic, Context.ActorOf(Props.Create(() => new TheGreatGambinisNightMagicGameLogic()), "TheGreatGambinisNightMagic"));
            _dicGameLogicActors.Add(GAMEID.JuicyRiches, Context.ActorOf(Props.Create(() => new JuicyRichesGameLogic()), "JuicyRiches"));
            _dicGameLogicActors.Add(GAMEID.DiamondLinkMightyElephant, Context.ActorOf(Props.Create(() => new DiamondLinkMightyElephantGameLogic()), "DiamondLinkMightyElephant"));
            _dicGameLogicActors.Add(GAMEID.LotusFlower, Context.ActorOf(Props.Create(() => new LotusFlowerGameLogic()), "LotusFlower"));
            _dicGameLogicActors.Add(GAMEID.SmokingHot7s, Context.ActorOf(Props.Create(() => new SmokingHot7sGameLogic()), "SmokingHot7s"));
            _dicGameLogicActors.Add(GAMEID.RisingJoker, Context.ActorOf(Props.Create(() => new RisingJokerGameLogic()), "RisingJoker"));
            _dicGameLogicActors.Add(GAMEID.CaptainVentureTreasuresOfTheSea, Context.ActorOf(Props.Create(() => new CaptainVentureTreasuresOfTheSeaGameLogic()), "CaptainVentureTreasuresOfTheSea"));

            _dicGameLogicActors.Add(GAMEID.CaribbeanHolidays, Context.ActorOf(Props.Create(() => new CaribbeanHolidaysGameLogic()), "CaribbeanHolidays"));
            _dicGameLogicActors.Add(GAMEID.N10ImperialCrownDeluxe, Context.ActorOf(Props.Create(() => new N10ImperialCrownDeluxeGameLogic()), "10ImperialCrownDeluxe"));
            _dicGameLogicActors.Add(GAMEID.N25RedHot7CloverLink, Context.ActorOf(Props.Create(() => new N25RedHot7CloverLinkGameLogic()), "25RedHot7CloverLink"));
            _dicGameLogicActors.Add(GAMEID.N25RedHotBurningCloverLink, Context.ActorOf(Props.Create(() => new N25RedHotBurningCloverLinkGameLogic()), "25RedHotBurningCloverLink"));
            _dicGameLogicActors.Add(GAMEID.RichesOfBabylon, Context.ActorOf(Props.Create(() => new RichesOfBabylonGameLogic()), "RichesOfBabylon"));
            _dicGameLogicActors.Add(GAMEID.SuperCircus, Context.ActorOf(Props.Create(() => new SuperCircusGameLogic()), "SuperCircus"));
            _dicGameLogicActors.Add(GAMEID.BurningWild, Context.ActorOf(Props.Create(() => new BurningWildGameLogic()), "BurningWild"));
            _dicGameLogicActors.Add(GAMEID.QueenCleopatra, Context.ActorOf(Props.Create(() => new QueenCleopatraGameLogic()), "QueenCleopatra"));
            _dicGameLogicActors.Add(GAMEID.PiggyPrizesWandOfRiches, Context.ActorOf(Props.Create(() => new PiggyPrizesWandOfRichesGameLogic()), "PiggyPrizesWandOfRiches"));

            _dicGameLogicActors.Add(GAMEID.ChilliElToro, Context.ActorOf(Props.Create(() => new ChilliElToroGameLogic()), "ChilliElToro"));
            _dicGameLogicActors.Add(GAMEID.StarCandy, Context.ActorOf(Props.Create(() => new StarCandyGameLogic()), "StarCandy"));

            _dicGameLogicActors.Add(GAMEID.StarlightJackpotsAthenaGoddessOfWar, Context.ActorOf(Props.Create(() => new StarlightJackpotsAthenaGoddessOfWarGameLogic()), "StarlightJackpotsAthenaGoddessOfWar"));
            _dicGameLogicActors.Add(GAMEID.SmashingSevensWinWays, Context.ActorOf(Props.Create(() => new SmashingSevensWinWaysGameLogic()), "SmashingSevensWinWays"));
            _dicGameLogicActors.Add(GAMEID.DiamondMysteryMegaBlaze, Context.ActorOf(Props.Create(() => new DiamondMysteryMegaBlazeGameLogic()), "DiamondMysteryMegaBlaze"));
            _dicGameLogicActors.Add(GAMEID.WishUponAStar, Context.ActorOf(Props.Create(() => new WishUponAStarGameLogic()), "WishUponAStar"));
            _dicGameLogicActors.Add(GAMEID.ManicPotions, Context.ActorOf(Props.Create(() => new ManicPotionsGameLogic()), "ManicPotions"));
            _dicGameLogicActors.Add(GAMEID.CyberWildz, Context.ActorOf(Props.Create(() => new CyberWildzGameLogic()), "CyberWildz"));
            _dicGameLogicActors.Add(GAMEID.MythOfMedusaGold, Context.ActorOf(Props.Create(() => new MythOfMedusaGoldGameLogic()), "MythOfMedusaGold"));
            _dicGameLogicActors.Add(GAMEID.RomeoJulietSealedWithAKiss, Context.ActorOf(Props.Create(() => new RomeoJulietSealedWithAKissGameLogic()), "RomeoJulietSealedWithAKiss"));
            _dicGameLogicActors.Add(GAMEID.DiamondTalesTheEmperorsNewClothes, Context.ActorOf(Props.Create(() => new DiamondTalesTheEmperorsNewClothesGameLogic()), "DiamondTalesTheEmperorsNewClothes"));
            _dicGameLogicActors.Add(GAMEID.DiamondLinkMightySanta, Context.ActorOf(Props.Create(() => new DiamondLinkMightySantaGameLogic()), "DiamondLinkMightySanta"));
            _dicGameLogicActors.Add(GAMEID.DiamondLinkAlmightyKraken, Context.ActorOf(Props.Create(() => new DiamondLinkAlmightyKrakenGameLogic()), "DiamondLinkAlmightyKraken"));
            _dicGameLogicActors.Add(GAMEID.DiamondLinkCopsNRobbers, Context.ActorOf(Props.Create(() => new DiamondLinkCopsNRobbersGameLogic()), "DiamondLinkCopsNRobbers"));
            _dicGameLogicActors.Add(GAMEID.N40SupremeFruits, Context.ActorOf(Props.Create(() => new N40SupremeFruitsGameLogic()), "40SupremeFruits"));
            _dicGameLogicActors.Add(GAMEID.StarSupreme, Context.ActorOf(Props.Create(() => new StarSupremeGameLogic()), "StarSupreme"));
            _dicGameLogicActors.Add(GAMEID.BingoStaxxAmazonFury, Context.ActorOf(Props.Create(() => new BingoStaxxAmazonFuryGameLogic()), "BingoStaxxAmazonFury"));
            _dicGameLogicActors.Add(GAMEID.ClashOfLegendsBattleLinesAnteBet, Context.ActorOf(Props.Create(() => new ClashOfLegendsBattleLinesAnteBetGameLogic()), "ClashOfLegendsBattleLinesAnteBet"));
            _dicGameLogicActors.Add(GAMEID.ElectricFlamingo, Context.ActorOf(Props.Create(() => new ElectricFlamingoGameLogic()), "ElectricFlamingo"));
            _dicGameLogicActors.Add(GAMEID.FuzanglongFireWilds, Context.ActorOf(Props.Create(() => new FuzanglongFireWildsGameLogic()), "FuzanglongFireWilds"));
            _dicGameLogicActors.Add(GAMEID.IrishCoins, Context.ActorOf(Props.Create(() => new IrishCoinsGameLogic()), "IrishCoins"));
            _dicGameLogicActors.Add(GAMEID.GoddessOfegypt, Context.ActorOf(Props.Create(() => new GoddessOfegyptGameLogic()), "GoddessOfegypt"));
            _dicGameLogicActors.Add(GAMEID.SilverTrails, Context.ActorOf(Props.Create(() => new SilverTrailsGameLogic()), "SilverTrails"));
            _dicGameLogicActors.Add(GAMEID.DiamondLinkOasisRiches, Context.ActorOf(Props.Create(() => new DiamondLinkOasisRichesGameLogic()), "DiamondLinkOasisRiches"));

            _dicGameLogicActors.Add(GAMEID.FruitKingSuperCash, Context.ActorOf(Props.Create(() => new FruitKingSuperCashGameLogic()), "FruitKingSuperCash"));
            _dicGameLogicActors.Add(GAMEID.SantasRiches, Context.ActorOf(Props.Create(() => new SantasRichesGameLogic()), "SantasRiches"));
            _dicGameLogicActors.Add(GAMEID.IslandHeat, Context.ActorOf(Props.Create(() => new IslandHeatGameLogic()), "IslandHeat"));
            _dicGameLogicActors.Add(GAMEID.FortuneFishing, Context.ActorOf(Props.Create(() => new FortuneFishingGameLogic()), "FortuneFishing"));
            _dicGameLogicActors.Add(GAMEID.RisingTreasures, Context.ActorOf(Props.Create(() => new RisingTreasuresGameLogic()), "RisingTreasures"));
            _dicGameLogicActors.Add(GAMEID.Fruchteparadies10, Context.ActorOf(Props.Create(() => new Fruchteparadies10GameLogic()), "Fruchteparadies10"));
            _dicGameLogicActors.Add(GAMEID.AncientGoddess, Context.ActorOf(Props.Create(() => new AncientGoddessGameLogic()), "AncientGoddess"));
            _dicGameLogicActors.Add(GAMEID.EgyptianUnderworld, Context.ActorOf(Props.Create(() => new EgyptianUnderworldGameLogic()), "EgyptianUnderworld"));
            _dicGameLogicActors.Add(GAMEID.BookOfRaTempleOfGold, Context.ActorOf(Props.Create(() => new BookOfRaTempleOfGoldGameLogic()), "BookOfRaTempleOfGold"));
            _dicGameLogicActors.Add(GAMEID.EyeOfTheQueen, Context.ActorOf(Props.Create(() => new EyeOfTheQueenGameLogic()), "EyeOfTheQueen"));
            _dicGameLogicActors.Add(GAMEID.DragonWarrior, Context.ActorOf(Props.Create(() => new DragonWarriorGameLogic()), "DragonWarrior")); 
            _dicGameLogicActors.Add(GAMEID.TalesOfDarknessFullMoon, Context.ActorOf(Props.Create(() => new TalesOfDarknessFullMoonGameLogic()), "TalesOfDarknessFullMoon"));
            _dicGameLogicActors.Add(GAMEID.PowerStars, Context.ActorOf(Props.Create(() => new PowerStarsGameLogic()), "PowerStars"));                                        
            _dicGameLogicActors.Add(GAMEID.AfricanSimba, Context.ActorOf(Props.Create(() => new AfricanSimbaGameLogic()), "AfricanSimba"));                                  
            _dicGameLogicActors.Add(GAMEID.DynamiteFortunes, Context.ActorOf(Props.Create(() => new DynamiteFortunesGameLogic()), "DynamiteFortunes"));                      
            _dicGameLogicActors.Add(GAMEID.SuperCherry2000, Context.ActorOf(Props.Create(() => new SuperCherry2000GameLogic()), "SuperCherry2000"));                       
            _dicGameLogicActors.Add(GAMEID.N50RedHot7CloverLink, Context.ActorOf(Props.Create(() => new N50RedHot7CloverLinkGameLogic()), "50RedHot7CloverLink"));  
            _dicGameLogicActors.Add(GAMEID.SuperCherryLockNWin, Context.ActorOf(Props.Create(() => new SuperCherryLockNWinGameLogic()), "SuperCherryLockNWin"));  
            _dicGameLogicActors.Add(GAMEID.N50ExtremeHot, Context.ActorOf(Props.Create(() => new N50ExtremeHotGameLogic()), "50ExtremeHot"));  
            _dicGameLogicActors.Add(GAMEID.N50RedHotBurningCloverLink, Context.ActorOf(Props.Create(() => new N50RedHotBurningCloverLinkGameLogic()), "50RedHotBurningCloverLink"));
            _dicGameLogicActors.Add(GAMEID.AutumnQueen, Context.ActorOf(Props.Create(() => new AutumnQueenGameLogic()), "AutumnQueen"));             
            _dicGameLogicActors.Add(GAMEID.HaulOfHades, Context.ActorOf(Props.Create(() => new HaulOfHadesGameLogic()), "HaulOfHades"));             
            _dicGameLogicActors.Add(GAMEID.FeelinFruityWinWays, Context.ActorOf(Props.Create(() => new FeelinFruityWinWaysGameLogic()), "FeelinFruityWinWays"));
            _dicGameLogicActors.Add(GAMEID.AlmightyJackpotsGardenOfPersephone, Context.ActorOf(Props.Create(() => new AlmightyJackpotsGardenOfPersephoneGameLogic()), "AlmightyJackpotsGardenOfPersephone"));
            _dicGameLogicActors.Add(GAMEID.ApolloGodOfTheSun, Context.ActorOf(Props.Create(() => new ApolloGodOfTheSunGameLogic()), "ApolloGodOfTheSun")); 
            _dicGameLogicActors.Add(GAMEID.DiamondLinkMightySevens, Context.ActorOf(Props.Create(() => new DiamondLinkMightySevensGameLogic()), "DiamondLinkMightySevens")); 
            _dicGameLogicActors.Add(GAMEID.DragonBlitz, Context.ActorOf(Props.Create(() => new DragonBlitzGameLogic()), "DragonBlitz"));          
            _dicGameLogicActors.Add(GAMEID.AChristmasFullOfWilds, Context.ActorOf(Props.Create(() => new AChristmasFullOfWildsGameLogic()), "AChristmasFullOfWilds")); 
            _dicGameLogicActors.Add(GAMEID.ArcticRace, Context.ActorOf(Props.Create(() => new ArcticRaceGameLogic()), "ArcticRace"));               
            _dicGameLogicActors.Add(GAMEID.GoldStarFruits, Context.ActorOf(Props.Create(() => new GoldStarFruitsGameLogic()), "GoldStarFruits"));    
            _dicGameLogicActors.Add(GAMEID.JokerParty6, Context.ActorOf(Props.Create(() => new JokerParty6GameLogic()), "JokerParty6"));    
            _dicGameLogicActors.Add(GAMEID.StarsInferno, Context.ActorOf(Props.Create(() => new StarsInfernoGameLogic()), "StarsInferno"));
            #endregion

            foreach (KeyValuePair<GAMEID, IActorRef> pair in _dicGameLogicActors)
            {
                pair.Value.Tell(new DBProxyInform(_dbReader, _dbWriter, _redisWriter));
                pair.Value.Tell("loadSetting");
                _hashAllChildActors.Add(pair.Value.Path.Name);
                Context.Watch(pair.Value);
            }
        }

        private async Task onLoadSpinDatabase(LoadSpinDataRequest request)
        {
            try
            {
                var infoDocuments = await Context.System.ActorSelection("/user/spinDBReaders").Ask<List<BsonDocument>>(new ReadInfoCollectionRequest(), TimeSpan.FromSeconds(10.0));
                foreach (BsonDocument infoDocument in infoDocuments)
                {
                    string strGameName = (string)infoDocument["name"];
                    IActorRef gameActor = Context.Child(strGameName);
                    if (gameActor != ActorRefs.Nobody)
                        gameActor.Tell(infoDocument);
                    else
                        continue;
                }
                Sender.Tell(true);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in GameCollectionActor::onLoadSpinDatabase {0}", ex);
                Sender.Tell(false);
            }
        }

        private async Task onPerformanceTest(PerformanceTestRequest request)
        {
            foreach (KeyValuePair<GAMEID, IActorRef> pair in _dicGameLogicActors)
            {
                await _dicGameLogicActors[pair.Key].Ask<bool>(request);
            }

            Sender.Tell(true);
        }

        private void onTerminated(Terminated terminated)
        {
            _hashAllChildActors.Remove(terminated.ActorRef.Path.Name);
            if(_hashAllChildActors.Count == 0)
            {
                Self.Tell(PoisonPill.Instance);
            }
        }
        private void processCommand(string command)
        {
            if(command == "terminate")
            {
                foreach (KeyValuePair<GAMEID, IActorRef> pair in _dicGameLogicActors)
                    pair.Value.Tell(PoisonPill.Instance);
            }
        }
        private void onEnterGameUser(EnterGameRequest enterGameMessage)
        {
            GAMEID gameID = (GAMEID) enterGameMessage.GameID;
            if (!_dicGameLogicActors.ContainsKey(gameID))
            {
                Sender.Tell(new EnterGameResponse((int) gameID, Self, 1)); 
                return;
            }

            _dicGameLogicActors[gameID].Forward(enterGameMessage);
        }
        private void onPayoutConfigUpdated(PayoutConfigUpdated updated)
        {
            GAMEID gameID = (GAMEID)updated.GameID;
            if (!_dicGameLogicActors.ContainsKey(gameID))
                return;

            _dicGameLogicActors[gameID].Tell(updated);
        }
    }

    public class LoadSpinDataRequest
    {

    }

    public class PerformanceTestRequest
    {
        public PerformanceTestRequest()
        {
        }
    }
}
