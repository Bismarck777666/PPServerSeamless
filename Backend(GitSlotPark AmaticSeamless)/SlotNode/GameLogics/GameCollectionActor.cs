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
            #region Amatic 게임
            _dicGameLogicActors.Add(GAMEID.WildShark,               Context.ActorOf(Props.Create(()     => new WildSharkGameLogic()),               "WildShark"));
            _dicGameLogicActors.Add(GAMEID.FlyingDutchman,          Context.ActorOf(Props.Create(()     => new FlyingDutchmanGameLogic()),          "FlyingDutchman"));
            _dicGameLogicActors.Add(GAMEID.WildDragon,              Context.ActorOf(Props.Create(()     => new WildDragonGameLogic()),              "WildDragon"));
            _dicGameLogicActors.Add(GAMEID.AllwaysFruits,           Context.ActorOf(Props.Create(()     => new AllwaysFruitsGameLogic()),           "AllwaysFruits"));
            _dicGameLogicActors.Add(GAMEID.Billyonaire,             Context.ActorOf(Props.Create(()     => new BillyonaireGameLogic()),             "Billyonaire"));
            _dicGameLogicActors.Add(GAMEID.DragonsKingdom,          Context.ActorOf(Props.Create(()     => new DragonsKingdomGameLogic()),          "DragonsKingdom"));
            _dicGameLogicActors.Add(GAMEID.AMWolfMoon,              Context.ActorOf(Props.Create(()     => new AMWolfMoonGameLogic()),              "WolfMoon"));
            _dicGameLogicActors.Add(GAMEID.BellsOnFireRombo,        Context.ActorOf(Props.Create(()     => new BellsOnFireRomboGameLogic()),        "BellsOnFireRombo"));
            _dicGameLogicActors.Add(GAMEID.GrandX,                  Context.ActorOf(Props.Create(()     => new GrandXGameLogic()),                  "GrandX"));
            _dicGameLogicActors.Add(GAMEID.BellsOnFire,             Context.ActorOf(Props.Create(()     => new BellsOnFireGameLogic()),             "BellsOnFire"));
            _dicGameLogicActors.Add(GAMEID.MagicOwl,                Context.ActorOf(Props.Create(()     => new MagicOwlGameLogic()),                "MagicOwl"));
            _dicGameLogicActors.Add(GAMEID.DiamondMonkey,           Context.ActorOf(Props.Create(()     => new DiamondMonkeyGameLogic()),           "DiamondMonkey"));
            _dicGameLogicActors.Add(GAMEID.HotFruits20,             Context.ActorOf(Props.Create(()     => new HotFruits20GameLogic()),             "HotFruits20"));
            _dicGameLogicActors.Add(GAMEID.AllwaysHotFruits,        Context.ActorOf(Props.Create(()     => new AllwaysHotFruitsGameLogic()),        "AllwaysHotFruits"));
            _dicGameLogicActors.Add(GAMEID.Wild7,                   Context.ActorOf(Props.Create(()     => new Wild7GameLogic()),                   "Wild7"));
            _dicGameLogicActors.Add(GAMEID.MagicIdol,               Context.ActorOf(Props.Create(()     => new MagicIdolGameLogic()),               "MagicIdol"));
            _dicGameLogicActors.Add(GAMEID.WildStars,               Context.ActorOf(Props.Create(()     => new WildStarsGameLogic()),               "WildStars"));
            _dicGameLogicActors.Add(GAMEID.GrandTiger,              Context.ActorOf(Props.Create(()     => new GrandTigerGameLogic()),              "GrandTiger"));
            _dicGameLogicActors.Add(GAMEID.LuckyJoker5,             Context.ActorOf(Props.Create(()     => new LuckyJoker5GameLogic()),             "LuckyJoker5"));
            _dicGameLogicActors.Add(GAMEID.RoyalUnicorn,            Context.ActorOf(Props.Create(()     => new RoyalUnicornGameLogic()),            "RoyalUnicorn"));
            _dicGameLogicActors.Add(GAMEID.BookOfFortune,           Context.ActorOf(Props.Create(()     => new BookOfFortuneGameLogic()),           "BookOfFortune"));
            _dicGameLogicActors.Add(GAMEID.LightningHot,            Context.ActorOf(Props.Create(()     => new LightningHotGameLogic()),            "LightningHot"));
            _dicGameLogicActors.Add(GAMEID.LuckyZodiac,             Context.ActorOf(Props.Create(()     => new LuckyZodiacGameLogic()),             "LuckyZodiac"));
            _dicGameLogicActors.Add(GAMEID.Casanova,                Context.ActorOf(Props.Create(()     => new CasanovaGameLogic()),                "Casanova"));
            _dicGameLogicActors.Add(GAMEID.BillysGame,              Context.ActorOf(Props.Create(()     => new BillysGameGameLogic()),              "BillysGame"));
            _dicGameLogicActors.Add(GAMEID.WildRespin,              Context.ActorOf(Props.Create(()     => new WildRespinGameLogic()),              "WildRespin"));
            _dicGameLogicActors.Add(GAMEID.DragonsPearl,            Context.ActorOf(Props.Create(()     => new DragonsPearlGameLogic()),            "DragonsPearl"));
            _dicGameLogicActors.Add(GAMEID.PartyTime,               Context.ActorOf(Props.Create(()     => new PartyTimeGameLogic()),               "PartyTime"));
            _dicGameLogicActors.Add(GAMEID.BigPanda,                Context.ActorOf(Props.Create(()     => new BigPandaGameLogic()),                "BigPanda"));
            _dicGameLogicActors.Add(GAMEID.TweetyBirds,             Context.ActorOf(Props.Create(()     => new TweetyBirdsGameLogic()),             "TweetyBirds"));
            _dicGameLogicActors.Add(GAMEID.MerryFruits,             Context.ActorOf(Props.Create(()     => new MerryFruitsGameLogic()),             "MerryFruits"));
            _dicGameLogicActors.Add(GAMEID.HotChoice,               Context.ActorOf(Props.Create(()     => new HotChoiceGameLogic()),               "HotChoice"));
            _dicGameLogicActors.Add(GAMEID.Hot81,                   Context.ActorOf(Props.Create(()     => new Hot81GameLogic()),                   "Hot81"));
            _dicGameLogicActors.Add(GAMEID.ArisingPhoenix,          Context.ActorOf(Props.Create(()     => new ArisingPhoenixGameLogic()),          "ArisingPhoenix"));
            _dicGameLogicActors.Add(GAMEID.DiamondCats,             Context.ActorOf(Props.Create(()     => new DiamondCatsGameLogic()),             "DiamondCats"));
            _dicGameLogicActors.Add(GAMEID.AllwaysWin,              Context.ActorOf(Props.Create(()     => new AllwaysWinGameLogic()),              "AllwaysWin"));
            _dicGameLogicActors.Add(GAMEID.HotTwenty,               Context.ActorOf(Props.Create(()     => new HotTwentyGameLogic()),               "HotTwenty"));
            _dicGameLogicActors.Add(GAMEID.UltraSeven,              Context.ActorOf(Props.Create(()     => new UltraSevenGameLogic()),              "UltraSeven"));
            _dicGameLogicActors.Add(GAMEID.Vampires,                Context.ActorOf(Props.Create(()     => new VampiresGameLogic()),                "Vampires"));
            _dicGameLogicActors.Add(GAMEID.EyeOfRa,                 Context.ActorOf(Props.Create(()     => new EyeOfRaGameLogic()),                 "EyeOfRa"));
            _dicGameLogicActors.Add(GAMEID.HotStar,                 Context.ActorOf(Props.Create(()     => new HotStarGameLogic()),                 "HotStar"));
            _dicGameLogicActors.Add(GAMEID.HotNeon,                 Context.ActorOf(Props.Create(()     => new HotNeonGameLogic()),                 "HotNeon"));
            _dicGameLogicActors.Add(GAMEID.Admiral,                 Context.ActorOf(Props.Create(()     => new AdmiralGameLogic()),                 "Admiral"));
            _dicGameLogicActors.Add(GAMEID.RedChilli,               Context.ActorOf(Props.Create(()     => new RedChilliGameLogic()),               "RedChilli"));
            _dicGameLogicActors.Add(GAMEID.LuckyJoker20,            Context.ActorOf(Props.Create(()     => new LuckyJoker20GameLogic()),            "LuckyJoker20"));
            _dicGameLogicActors.Add(GAMEID.CoolDiamondsII,          Context.ActorOf(Props.Create(()     => new CoolDiamondsIIGameLogic()),          "CoolDiamondsII"));
            _dicGameLogicActors.Add(GAMEID.EnchantedCleopatra,      Context.ActorOf(Props.Create(()     => new EnchantedCleopatraGameLogic()),      "EnchantedCleopatra"));
            _dicGameLogicActors.Add(GAMEID.LadyJoker,               Context.ActorOf(Props.Create(()     => new LadyJokerGameLogic()),               "LadyJoker"));
            _dicGameLogicActors.Add(GAMEID.HotDiamonds,             Context.ActorOf(Props.Create(()     => new HotDiamondsGameLogic()),             "HotDiamonds"));
            _dicGameLogicActors.Add(GAMEID.HotScatter,              Context.ActorOf(Props.Create(()     => new HotScatterGameLogic()),              "HotScatter"));
            _dicGameLogicActors.Add(GAMEID.LuckyBells,              Context.ActorOf(Props.Create(()     => new LuckyBellsGameLogic()),              "LuckyBells"));
            _dicGameLogicActors.Add(GAMEID.GoldenBook,              Context.ActorOf(Props.Create(()     => new GoldenBookGameLogic()),              "GoldenBook"));
            _dicGameLogicActors.Add(GAMEID.BellsOnFireHot,          Context.ActorOf(Props.Create(()     => new BellsOnFireHotGameLogic()),          "BellsOnFireHot"));
            _dicGameLogicActors.Add(GAMEID.HottestFruits20,         Context.ActorOf(Props.Create(()     => new HottestFruits20GameLogic()),         "HottestFruits20"));
            _dicGameLogicActors.Add(GAMEID.DragonsGift,             Context.ActorOf(Props.Create(()     => new DragonsGiftGameLogic()),             "DragonsGift"));
            _dicGameLogicActors.Add(GAMEID.Hot7,                    Context.ActorOf(Props.Create(()     => new Hot7GameLogic()),                    "Hot7"));
            _dicGameLogicActors.Add(GAMEID.MagicForest,             Context.ActorOf(Props.Create(()     => new MagicForestGameLogic()),             "MagicForest"));
            _dicGameLogicActors.Add(GAMEID.LuckyCoin,               Context.ActorOf(Props.Create(()     => new LuckyCoinGameLogic()),               "LuckyCoin"));
            _dicGameLogicActors.Add(GAMEID.LuckyLittleDevil,        Context.ActorOf(Props.Create(()     => new LuckyLittleDevilGameLogic()), "LuckyLittleDevil"));
            _dicGameLogicActors.Add(GAMEID.DiamondsOnFire,          Context.ActorOf(Props.Create(()     => new DiamondsOnFireGameLogic()),          "DiamondsOnFire"));
            _dicGameLogicActors.Add(GAMEID.Casinova,                Context.ActorOf(Props.Create(()     => new CasinovaGameLogic()),                "Casinova"));
            _dicGameLogicActors.Add(GAMEID.LadyLuck,                Context.ActorOf(Props.Create(()     => new LadyLuckGameLogic()),                "LadyLuck"));
            _dicGameLogicActors.Add(GAMEID.HotFruits40,             Context.ActorOf(Props.Create(()     => new HotFruits40GameLogic()),             "HotFruits40"));
            _dicGameLogicActors.Add(GAMEID.LaGranAventura,          Context.ActorOf(Props.Create(()     => new LaGranAventuraGameLogic()),          "LaGranAventura"));
            _dicGameLogicActors.Add(GAMEID.Bluedolphin,             Context.ActorOf(Props.Create(()     => new BluedolphinGameLogic()),             "Bluedolphin"));
            _dicGameLogicActors.Add(GAMEID.KingsCrown,              Context.ActorOf(Props.Create(()     => new KingsCrownGameLogic()),              "KingsCrown"));
            _dicGameLogicActors.Add(GAMEID.BookOfQueen,             Context.ActorOf(Props.Create(()     => new BookOfQueenGameLogic()),             "BookOfQueen"));
            _dicGameLogicActors.Add(GAMEID.FortunasFruits,          Context.ActorOf(Props.Create(()     => new FortunasFruitsGameLogic()),          "FortunasFruits"));
            _dicGameLogicActors.Add(GAMEID.TwentySeven,             Context.ActorOf(Props.Create(()     => new TwentySevenGameLogic()),             "TwentySeven"));
            _dicGameLogicActors.Add(GAMEID.MegaShark,               Context.ActorOf(Props.Create(()     => new MegaSharkGameLogic()),               "MegaShark"));
            _dicGameLogicActors.Add(GAMEID.PlentyDragons,           Context.ActorOf(Props.Create(()     => new PlentyDragonsGameLogic()),           "PlentyDragons"));
            _dicGameLogicActors.Add(GAMEID.BookOfLords,             Context.ActorOf(Props.Create(()     => new BookOfLordsGameLogic()),             "BookOfLords"));
            _dicGameLogicActors.Add(GAMEID.GrandCasanova,           Context.ActorOf(Props.Create(()     => new GrandCasanovaGameLogic()),           "GrandCasanova"));
            _dicGameLogicActors.Add(GAMEID.HotFruits100,            Context.ActorOf(Props.Create(()     => new HotFruits100GameLogic()),            "HotFruits100"));
            _dicGameLogicActors.Add(GAMEID.AztecSecret,             Context.ActorOf(Props.Create(()     => new AztecSecretGameLogic()),             "AztecSecret"));
            _dicGameLogicActors.Add(GAMEID.LadyFruits20,            Context.ActorOf(Props.Create(()     => new LadyFruits20GameLogic()),            "LadyFruits20"));
            _dicGameLogicActors.Add(GAMEID.GoldenJoker,             Context.ActorOf(Props.Create(()     => new GoldenJokerGameLogic()),             "GoldenJoker"));
            _dicGameLogicActors.Add(GAMEID.MermaidsGold,            Context.ActorOf(Props.Create(()     => new MermaidsGoldGameLogic()),            "MermaidsGold"));
            _dicGameLogicActors.Add(GAMEID.HotFruitsDeluxe,         Context.ActorOf(Props.Create(()     => new HotFruitsDeluxeGameLogic()),         "HotFruitsDeluxe"));
            _dicGameLogicActors.Add(GAMEID.FruitBox,                Context.ActorOf(Props.Create(()     => new FruitBoxGameLogic()),                "FruitBox"));
            _dicGameLogicActors.Add(GAMEID.GrandFruits,             Context.ActorOf(Props.Create(()     => new GrandFruitsGameLogic()),             "GrandFruits"));
            _dicGameLogicActors.Add(GAMEID.Oktoberfest,             Context.ActorOf(Props.Create(()     => new OktoberfestGameLogic()),             "Oktoberfest"));
            _dicGameLogicActors.Add(GAMEID.FireAndIce,              Context.ActorOf(Props.Create(()     => new FireAndIceGameLogic()),              "FireAndIce"));
            _dicGameLogicActors.Add(GAMEID.HotChoiceDeluxe,         Context.ActorOf(Props.Create(()     => new HotChoiceDeluxeGameLogic()),         "HotChoiceDeluxe"));
            _dicGameLogicActors.Add(GAMEID.NicerDice40,             Context.ActorOf(Props.Create(()     => new NicerDice40GameLogic()),             "NicerDice40"));
            _dicGameLogicActors.Add(GAMEID.PartyNight,              Context.ActorOf(Props.Create(()     => new PartyNightGameLogic()),              "PartyNight"));
            _dicGameLogicActors.Add(GAMEID.CrazyBee,                Context.ActorOf(Props.Create(()     => new CrazyBeeGameLogic()),                "CrazyBee"));
            _dicGameLogicActors.Add(GAMEID.BeautyWarrior,           Context.ActorOf(Props.Create(()     => new BeautyWarriorGameLogic()),           "BeautyWarrior"));
            _dicGameLogicActors.Add(GAMEID.LuckyJoker100,           Context.ActorOf(Props.Create(()     => new LuckyJoker100GameLogic()),           "LuckyJoker100"));
            _dicGameLogicActors.Add(GAMEID.HotScatterDice,          Context.ActorOf(Props.Create(()     => new HotScatterDiceGameLogic()),          "HotScatterDice"));
            _dicGameLogicActors.Add(GAMEID.HotScatterDeluxe,        Context.ActorOf(Props.Create(()     => new HotScatterDeluxeGameLogic()),        "HotScatterDeluxe"));
            _dicGameLogicActors.Add(GAMEID.GemStar,                 Context.ActorOf(Props.Create(()     => new GemStarGameLogic()),                 "GemStar"));
            _dicGameLogicActors.Add(GAMEID.ChineseSpider,           Context.ActorOf(Props.Create(()     => new ChineseSpiderGameLogic()),           "ChineseSpider"));
            _dicGameLogicActors.Add(GAMEID.AMFireQueen,             Context.ActorOf(Props.Create(()     => new AMFireQueenGameLogic()),             "FireQueen"));
            _dicGameLogicActors.Add(GAMEID.Hot40,                   Context.ActorOf(Props.Create(()     => new Hot40GameLogic()),                   "Hot40"));
            _dicGameLogicActors.Add(GAMEID.LuckyJoker40,            Context.ActorOf(Props.Create(()     => new LuckyJoker40GameLogic()),            "LuckyJoker40"));
            _dicGameLogicActors.Add(GAMEID.SakuraFruits,            Context.ActorOf(Props.Create(()     => new SakuraFruitsGameLogic()),            "SakuraFruits"));
            _dicGameLogicActors.Add(GAMEID.Hot27Dice,               Context.ActorOf(Props.Create(()     => new Hot27DiceGameLogic()),               "Hot27Dice"));
            _dicGameLogicActors.Add(GAMEID.BookOfFruits20,          Context.ActorOf(Props.Create(()     => new BookOfFruits20GameLogic()),          "BookOfFruits20"));
            _dicGameLogicActors.Add(GAMEID.LovelyLadyXmas,          Context.ActorOf(Props.Create(()     => new LovelyLadyXmasGameLogic()),          "LovelyLadyXmas"));
            _dicGameLogicActors.Add(GAMEID.DragonsMystery,          Context.ActorOf(Props.Create(()     => new DragonsMysteryGameLogic()),          "DragonsMystery"));
            _dicGameLogicActors.Add(GAMEID.GoldenQuest,             Context.ActorOf(Props.Create(()     => new GoldenQuestGameLogic()),             "GoldenQuest"));
            _dicGameLogicActors.Add(GAMEID.CrystalFruits,           Context.ActorOf(Props.Create(()     => new CrystalFruitsGameLogic()),           "CrystalFruits"));
            _dicGameLogicActors.Add(GAMEID.BookOfFruitsHalloween,   Context.ActorOf(Props.Create(()     => new BookOfFruitsHalloweenGameLogic()),   "BookOfFruitsHalloween"));
            _dicGameLogicActors.Add(GAMEID.BookOfFruits,            Context.ActorOf(Props.Create(()     => new BookOfFruitsGameLogic()),            "BookOfFruits"));
            _dicGameLogicActors.Add(GAMEID.FortuneGirl,             Context.ActorOf(Props.Create(()     => new FortuneGirlGameLogic()),             "FortuneGirl"));
            _dicGameLogicActors.Add(GAMEID.HotFruits27,             Context.ActorOf(Props.Create(()     => new HotFruits27GameLogic()),             "HotFruits27"));
            _dicGameLogicActors.Add(GAMEID.LuckyRespin,             Context.ActorOf(Props.Create(()     => new LuckyRespinGameLogic()),             "LuckyRespin"));
            _dicGameLogicActors.Add(GAMEID.Hot7Dice,                Context.ActorOf(Props.Create(()     => new Hot7DiceGameLogic()),                "Hot7Dice"));
            _dicGameLogicActors.Add(GAMEID.HottestFruits40,         Context.ActorOf(Props.Create(()     => new HottestFruits40GameLogic()),         "HottestFruits40"));
            _dicGameLogicActors.Add(GAMEID.SakuraSecret,            Context.ActorOf(Props.Create(()     => new SakuraSecretGameLogic()),            "SakuraSecret"));
            _dicGameLogicActors.Add(GAMEID.BeautyFairy,             Context.ActorOf(Props.Create(()     => new BeautyFairyGameLogic()),             "BeautyFairy"));
            _dicGameLogicActors.Add(GAMEID.DiaMuertos,              Context.ActorOf(Props.Create(()     => new DiaMuertosGameLogic()),              "DiaMuertos"));
            _dicGameLogicActors.Add(GAMEID.AllwaysJoker,            Context.ActorOf(Props.Create(()     => new AllwaysJokerGameLogic()),            "AllwaysJoker"));
            _dicGameLogicActors.Add(GAMEID.BookOfAztec,             Context.ActorOf(Props.Create(()     => new BookOfAztecGameLogic()),             "BookOfAztec"));
            _dicGameLogicActors.Add(GAMEID.HotFruits10,             Context.ActorOf(Props.Create(()     => new HotFruits10GameLogic()),             "HotFruits10"));
            _dicGameLogicActors.Add(GAMEID.SantasFruits,            Context.ActorOf(Props.Create(()     => new SantasFruitsGameLogic()),            "SantasFruits"));
            _dicGameLogicActors.Add(GAMEID.GrandWildDragon20,       Context.ActorOf(Props.Create(()     => new GrandWildDragon20GameLogic()),       "GrandWildDragon20"));
            _dicGameLogicActors.Add(GAMEID.BookOfAdmiral,           Context.ActorOf(Props.Create(()     => new BookOfAdmiralGameLogic()),           "BookOfAdmiral"));
            _dicGameLogicActors.Add(GAMEID.SunGoddess,              Context.ActorOf(Props.Create(()     => new SunGoddessGameLogic()),              "SunGoddess"));
            _dicGameLogicActors.Add(GAMEID.LadyFruits100Easter,     Context.ActorOf(Props.Create(()     => new LadyFruits100EasterGameLogic()),     "LadyFruits100Easter"));
            _dicGameLogicActors.Add(GAMEID.HotDice20,               Context.ActorOf(Props.Create(()     => new HotDice20GameLogic()),               "HotDice20"));
            _dicGameLogicActors.Add(GAMEID.HotDice40,               Context.ActorOf(Props.Create(()     => new HotDice40GameLogic()),               "HotDice40"));
            _dicGameLogicActors.Add(GAMEID.BurningBells20Dice,      Context.ActorOf(Props.Create(()     => new BurningBells20DiceGameLogic()),      "BurningBells20Dice"));
            _dicGameLogicActors.Add(GAMEID.HotDice27,               Context.ActorOf(Props.Create(()     => new HotDice27GameLogic()),               "HotDice27"));
            _dicGameLogicActors.Add(GAMEID.LuckyJoker10Dice,        Context.ActorOf(Props.Create(()     => new LuckyJoker10DiceGameLogic()),        "LuckyJoker10Dice"));
            _dicGameLogicActors.Add(GAMEID.BurningBells10,          Context.ActorOf(Props.Create(()     => new BurningBells10GameLogic()),          "BurningBells10"));
            _dicGameLogicActors.Add(GAMEID.FruitStar,               Context.ActorOf(Props.Create(()     => new FruitStarGameLogic()),               "FruitStar"));
            _dicGameLogicActors.Add(GAMEID.TripleWild,              Context.ActorOf(Props.Create(()     => new TripleWildGameLogic()),              "TripleWild"));
            _dicGameLogicActors.Add(GAMEID.DoubleDiamonds,          Context.ActorOf(Props.Create(()     => new DoubleDiamondsGameLogic()),          "DoubleDiamonds"));
            _dicGameLogicActors.Add(GAMEID.Hot7Deluxe,              Context.ActorOf(Props.Create(()     => new Hot7DeluxeGameLogic()),              "Hot7Deluxe"));
            _dicGameLogicActors.Add(GAMEID.LuckyGoldenSeven,        Context.ActorOf(Props.Create(()     => new LuckyGoldenSevenGameLogic()),        "LuckyGoldenSeven"));
            _dicGameLogicActors.Add(GAMEID.JokerX,                  Context.ActorOf(Props.Create(()     => new JokerXGameLogic()),                  "JokerX"));
            _dicGameLogicActors.Add(GAMEID.WildAnubis,              Context.ActorOf(Props.Create(()     => new WildAnubisGameLogic()),              "WildAnubis"));
            _dicGameLogicActors.Add(GAMEID.WildWitches,             Context.ActorOf(Props.Create(()     => new WildWitchesGameLogic()),             "WildWitches"));
            _dicGameLogicActors.Add(GAMEID.AllwaysHotDice,          Context.ActorOf(Props.Create(()     => new AllwaysHotDiceGameLogic()),          "AllwaysHotDice"));
            _dicGameLogicActors.Add(GAMEID.HotChoiceDice,           Context.ActorOf(Props.Create(()     => new HotChoiceDiceGameLogic()),           "HotChoiceDice"));
            _dicGameLogicActors.Add(GAMEID.RelicRiches,             Context.ActorOf(Props.Create(()     => new RelicRichesGameLogic()),             "RelicRiches"));
            _dicGameLogicActors.Add(GAMEID.HotDice10,               Context.ActorOf(Props.Create(()     => new HotDice10GameLogic()),               "HotDice10"));
            _dicGameLogicActors.Add(GAMEID.BurningBells20,          Context.ActorOf(Props.Create(()     => new BurningBells20GameLogic()),          "BurningBells20"));
            _dicGameLogicActors.Add(GAMEID.BurningBells40,          Context.ActorOf(Props.Create(()     => new BurningBells40GameLogic()),          "BurningBells40"));
            _dicGameLogicActors.Add(GAMEID.LuckyJoker10,            Context.ActorOf(Props.Create(()     => new LuckyJoker10GameLogic()),            "LuckyJoker10"));
            _dicGameLogicActors.Add(GAMEID.BountyBonanza,           Context.ActorOf(Props.Create(()     => new BountyBonanzaGameLogic()),           "BountyBonanza"));
            _dicGameLogicActors.Add(GAMEID.BillyonaireBonusBuy,     Context.ActorOf(Props.Create(()     => new BillyonaireBonusBuyGameLogic()),     "BillyonaireBonusBuy"));
            _dicGameLogicActors.Add(GAMEID.BookOfAztecBonusBuy,     Context.ActorOf(Props.Create(()     => new BookOfAztecBonusBuyGameLogic()),     "BookOfAztecBonusBuy"));
            _dicGameLogicActors.Add(GAMEID.ChilliWillie,            Context.ActorOf(Props.Create(()     => new ChilliWillieGameLogic()),            "ChilliWillie"));
            _dicGameLogicActors.Add(GAMEID.WildSharkBonusBuy,       Context.ActorOf(Props.Create(()     => new WildSharkBonusBuyGameLogic()),       "WildSharkBonusBuy"));
            _dicGameLogicActors.Add(GAMEID.AllwaysHottestFruits,    Context.ActorOf(Props.Create(()     => new AllwaysHottestFruitsGameLogic()),    "AllwaysHottestFruits"));
            _dicGameLogicActors.Add(GAMEID.AllwaysCandy,            Context.ActorOf(Props.Create(()     => new AllwaysCandyGameLogic()),            "AllwaysCandy"));
            _dicGameLogicActors.Add(GAMEID.BookOfAztecSelect,       Context.ActorOf(Props.Create(()     => new BookOfAztecSelectGameLogic()),       "BookOfAztecSelect"));
            _dicGameLogicActors.Add(GAMEID.NicerDice100,            Context.ActorOf(Props.Create(()     => new NicerDice100GameLogic()),            "NicerDice100"));
            _dicGameLogicActors.Add(GAMEID.CasanovasLadies,         Context.ActorOf(Props.Create(()     => new CasanovasLadiesGameLogic()),         "CasanovasLadies"));
            _dicGameLogicActors.Add(GAMEID.LovelyLadyDeluxe,        Context.ActorOf(Props.Create(()     => new LovelyLadyDeluxeGameLogic()),        "LovelyLadyDeluxe"));
            _dicGameLogicActors.Add(GAMEID.TikiMadness100,          Context.ActorOf(Props.Create(()     => new TikiMadness100GameLogic()),          "TikiMadness100"));
            _dicGameLogicActors.Add(GAMEID.LadyFruits40Easter,      Context.ActorOf(Props.Create(()     => new LadyFruits40EasterGameLogic()),      "LadyFruits40Easter"));
            _dicGameLogicActors.Add(GAMEID.FieryFruits,             Context.ActorOf(Props.Create(()     => new FieryFruitsGameLogic()),             "FieryFruits"));
            _dicGameLogicActors.Add(GAMEID.BookOfFruits10,          Context.ActorOf(Props.Create(()     => new BookOfFruits10GameLogic()),          "BookOfFruits10"));
            _dicGameLogicActors.Add(GAMEID.HotSoccer,               Context.ActorOf(Props.Create(()     => new HotSoccerGameLogic()),               "HotSoccer"));
            _dicGameLogicActors.Add(GAMEID.WildVolcano,             Context.ActorOf(Props.Create(()     => new WildVolcanoGameLogic()),             "WildVolcano"));
            _dicGameLogicActors.Add(GAMEID.HarleQueen,              Context.ActorOf(Props.Create(()     => new HarleQueenGameLogic()),              "HarleQueen"));
            _dicGameLogicActors.Add(GAMEID.LuckyDouble,             Context.ActorOf(Props.Create(()     => new LuckyDoubleGameLogic()),             "LuckyDouble"));
            _dicGameLogicActors.Add(GAMEID.PharaohsGold20,          Context.ActorOf(Props.Create(()     => new PharaohsGold20GameLogic()),          "PharaohsGold20"));
            _dicGameLogicActors.Add(GAMEID.BookOfAztecDice,         Context.ActorOf(Props.Create(()     => new BookOfAztecDiceGameLogic()),         "BookOfAztecDice"));
            _dicGameLogicActors.Add(GAMEID.SuperCats,               Context.ActorOf(Props.Create(()     => new SuperCatsGameLogic()),               "SuperCats"));
            _dicGameLogicActors.Add(GAMEID.BookOfPharao,            Context.ActorOf(Props.Create(()     => new BookOfPharaoGameLogic()),            "BookOfPharao"));
            _dicGameLogicActors.Add(GAMEID.GoldenFish,              Context.ActorOf(Props.Create(()     => new GoldenFishGameLogic()),              "GoldenFish"));
            _dicGameLogicActors.Add(GAMEID.KingOfDwarves,           Context.ActorOf(Props.Create(()     => new KingOfDwarvesGameLogic()),           "KingOfDwarves"));
            _dicGameLogicActors.Add(GAMEID.FruitExpress,            Context.ActorOf(Props.Create(()     => new FruitExpressGameLogic()),            "FruitExpress"));
            _dicGameLogicActors.Add(GAMEID.BuffaloThunderstacks,    Context.ActorOf(Props.Create(()     => new BuffaloThunderstacksGameLogic()),    "BuffaloThunderstacks"));
            _dicGameLogicActors.Add(GAMEID.CashDiamonds,            Context.ActorOf(Props.Create(()     => new CashDiamondsGameLogic()),            "CashDiamonds"));
            _dicGameLogicActors.Add(GAMEID.BookOfMontezuma,         Context.ActorOf(Props.Create(()     => new BookOfMontezumaGameLogic()),         "BookOfMontezuma"));
            _dicGameLogicActors.Add(GAMEID.HotFootball,             Context.ActorOf(Props.Create(()     => new HotFootballGameLogic()),             "HotFootball"));
            _dicGameLogicActors.Add(GAMEID.MistressOfMonsters,      Context.ActorOf(Props.Create(()     => new MistressOfMonstersGameLogic()),      "MistressOfMonsters"));
            _dicGameLogicActors.Add(GAMEID.DiamondStaxx,            Context.ActorOf(Props.Create(()     => new DiamondStaxxGameLogic()),            "DiamondStaxx"));
            _dicGameLogicActors.Add(GAMEID.WildBoost,               Context.ActorOf(Props.Create(()     => new WildBoostGameLogic()),               "WildBoost"));
            _dicGameLogicActors.Add(GAMEID.HotFruits5,              Context.ActorOf(Props.Create(()     => new HotFruits5GameLogic()),              "HotFruits5"));
            _dicGameLogicActors.Add(GAMEID.DoubleDiamonds50,        Context.ActorOf(Props.Create(()     => new DoubleDiamonds50GameLogic()),        "DoubleDiamonds50"));
            _dicGameLogicActors.Add(GAMEID.MrMagic,                 Context.ActorOf(Props.Create(()     => new MrMagicGameLogic()),                 "MrMagic"));
            _dicGameLogicActors.Add(GAMEID.GrandWildDragon,         Context.ActorOf(Props.Create(()     => new GrandWildDragonGameLogic()),         "GrandWildDragon"));
            _dicGameLogicActors.Add(GAMEID.DoubleJoker20,           Context.ActorOf(Props.Create(()     => new DoubleJoker20GameLogic()),           "DoubleJoker20"));
            _dicGameLogicActors.Add(GAMEID.SunGoddessII,            Context.ActorOf(Props.Create(()     => new SunGoddessIIGameLogic()),            "SunGoddessII"));
            _dicGameLogicActors.Add(GAMEID.HotFruitsWheel,          Context.ActorOf(Props.Create(()     => new HotFruitsWheelGameLogic()),          "HotFruitsWheel"));
            _dicGameLogicActors.Add(GAMEID.DoubleFruits,            Context.ActorOf(Props.Create(()     => new DoubleFruitsGameLogic()),            "DoubleFruits"));
            _dicGameLogicActors.Add(GAMEID.KittyBet,                Context.ActorOf(Props.Create(()     => new KittyBetGameLogic()),                "KittyBet"));
            _dicGameLogicActors.Add(GAMEID.BlazingCoins20,          Context.ActorOf(Props.Create(()     => new BlazingCoins20GameLogic()),          "BlazingCoins20"));
            _dicGameLogicActors.Add(GAMEID.BlazingCoins40,          Context.ActorOf(Props.Create(()     => new BlazingCoins40GameLogic()),          "BlazingCoins40"));
            _dicGameLogicActors.Add(GAMEID.BlazingCoins100,         Context.ActorOf(Props.Create(()     => new BlazingCoins100GameLogic()),         "BlazingCoins100"));
            _dicGameLogicActors.Add(GAMEID.CashAndCrab,             Context.ActorOf(Props.Create(()     => new CashAndCrabGameLogic()),             "CashAndCrab"));
            _dicGameLogicActors.Add(GAMEID.LuckyPiggies,            Context.ActorOf(Props.Create(()     => new LuckyPiggiesGameLogic()),            "LuckyPiggies"));
            _dicGameLogicActors.Add(GAMEID.DragonPot,               Context.ActorOf(Props.Create(()     => new DragonPotGameLogic()),               "DragonPot"));
            _dicGameLogicActors.Add(GAMEID.HotFruits20Cashspins,    Context.ActorOf(Props.Create(()     => new HotFruits20CashspinsGameLogic()),    "HotFruits20Cashspins"));
            _dicGameLogicActors.Add(GAMEID.LuckyJoker10Cashspins,   Context.ActorOf(Props.Create(()     => new LuckyJoker10CashspinsGameLogic()),   "LuckyJoker10Cashspins"));
            _dicGameLogicActors.Add(GAMEID.LuckyShark,              Context.ActorOf(Props.Create(()     => new LuckySharkGameLogic()),              "LuckyShark"));
            _dicGameLogicActors.Add(GAMEID.LuckyEgyptLinked,        Context.ActorOf(Props.Create(()     => new LuckyEgyptLinkedGameLogic()),        "LuckyEgyptLinked"));
            _dicGameLogicActors.Add(GAMEID.LuckyJoker10ExtraGifts,  Context.ActorOf(Props.Create(()     => new LuckyJoker10ExtraGiftsGameLogic()),  "LuckyJoker10ExtraGifts"));
            _dicGameLogicActors.Add(GAMEID.LuckyJoker20ExtraGifts,  Context.ActorOf(Props.Create(()     => new LuckyJoker20ExtraGiftsGameLogic()),  "LuckyJoker20ExtraGifts"));
            _dicGameLogicActors.Add(GAMEID.LuckyJokerDiceExtraGifts,Context.ActorOf(Props.Create(()     => new LuckyJokerDiceExtraGiftsGameLogic()),"LuckyJokerDiceExtraGifts"));
            _dicGameLogicActors.Add(GAMEID.LuckyJoker5ExtraGifts,   Context.ActorOf(Props.Create(()     => new LuckyJoker5ExtraGiftsGameLogic()),   "LuckyJoker5ExtraGifts"));
            _dicGameLogicActors.Add(GAMEID.LuckyJokerXmas,          Context.ActorOf(Props.Create(()     => new LuckyJokerXmasGameLogic()),          "LuckyJokerXmas"));
            _dicGameLogicActors.Add(GAMEID.LuckyJokerXmasDice,      Context.ActorOf(Props.Create(()     => new LuckyJokerXmasDiceGameLogic()),      "LuckyJokerXmasDice"));
            _dicGameLogicActors.Add(GAMEID.LuckyJoker40ExtraGifts,  Context.ActorOf(Props.Create(()     => new LuckyJoker40ExtraGiftsGameLogic()),  "LuckyJoker40ExtraGifts"));
            _dicGameLogicActors.Add(GAMEID.MultiBillyonaire,        Context.ActorOf(Props.Create(()     => new MultiBillyonaireGameLogic()),        "MultiBillyonaire"));
            _dicGameLogicActors.Add(GAMEID.LuckyPiggies2,           Context.ActorOf(Props.Create(()     => new LuckyPiggies2GameLogic()),           "LuckyPiggies2"));
            _dicGameLogicActors.Add(GAMEID.BillysGang,              Context.ActorOf(Props.Create(()     => new BillysGangGameLogic()),              "BillysGang"));


            _dicGameLogicActors.Add(GAMEID.RouletteRoyal,           Context.ActorOf(Props.Create(()     => new RouletteRoyalGameLogic()),           "RouletteRoyal"));


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
                Sender.Tell(new EnterGameResponse((int) gameID, Self, 1));  //해당 게임이 존재하지 않음
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
