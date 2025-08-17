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

            Receive<EnterGameRequest>               (_ => OnEnterGameUser(_));
            Receive<string>                         (_ => ProcessCommand(_));
            Receive<PayoutConfigUpdated>            (_ => OnPayoutConfigUpdated(_));
            Receive<Terminated>                     (_ => OnTerminated(_));
            ReceiveAsync<LoadSpinDataRequest>       (OnLoadSpinDatabase);
            ReceiveAsync<PerformanceTestRequest>    (OnPerformanceTest);
        }

        protected override void PreStart()
        {
            base.PreStart();
            CreateGameLogicActors();
        }

        protected void CreateGameLogicActors()
        {
            #region EGT games
            _dicGameLogicActors.Add(GAMEID.DiceOfMagic, Context.ActorOf(Props.Create(() => new DiceOfMagicGameLogic()), "DiceOfMagic"));
            _dicGameLogicActors.Add(GAMEID.SupremeDice, Context.ActorOf(Props.Create(() => new SupremeDiceGameLogic()), "SupremeDice"));
            _dicGameLogicActors.Add(GAMEID.N100Cats, Context.ActorOf(Props.Create(() => new N100CatsGameLogic()), "100Cats"));
            _dicGameLogicActors.Add(GAMEID.N100Dice, Context.ActorOf(Props.Create(() => new N100DiceGameLogic()), "100Dice"));
            _dicGameLogicActors.Add(GAMEID.N100SuperDice, Context.ActorOf(Props.Create(() => new N100SuperDiceGameLogic()), "100SuperDice"));
            _dicGameLogicActors.Add(GAMEID.N40BurningDice, Context.ActorOf(Props.Create(() => new N40BurningDiceGameLogic()), "40BurningDice"));
            _dicGameLogicActors.Add(GAMEID.N40SuperDice, Context.ActorOf(Props.Create(() => new N40SuperDiceGameLogic()), "40SuperDice"));
            _dicGameLogicActors.Add(GAMEID.N50Horses, Context.ActorOf(Props.Create(() => new N50HorsesGameLogic()), "50Horses"));
            _dicGameLogicActors.Add(GAMEID.AztecGlory, Context.ActorOf(Props.Create(() => new AztecGloryGameLogic()), "AztecGlory"));
            _dicGameLogicActors.Add(GAMEID.CasinoMania, Context.ActorOf(Props.Create(() => new CasinoManiaGameLogic()), "CasinoMania"));
            _dicGameLogicActors.Add(GAMEID.CoralIsland, Context.ActorOf(Props.Create(() => new CoralIslandGameLogic()), "CoralIsland"));
            _dicGameLogicActors.Add(GAMEID.DiceOfRa, Context.ActorOf(Props.Create(() => new DiceOfRaGameLogic()), "DiceOfRa"));
            _dicGameLogicActors.Add(GAMEID.FastMoney, Context.ActorOf(Props.Create(() => new FastMoneyGameLogic()), "FastMoney"));
            _dicGameLogicActors.Add(GAMEID.HotDeco, Context.ActorOf(Props.Create(() => new HotDecoGameLogic()), "HotDeco"));
            _dicGameLogicActors.Add(GAMEID.IncaGoldII, Context.ActorOf(Props.Create(() => new IncaGoldIIGameLogic()), "IncaGoldII"));
            _dicGameLogicActors.Add(GAMEID.LikeADiamond, Context.ActorOf(Props.Create(() => new LikeADiamondGameLogic()), "LikeADiamond"));
            _dicGameLogicActors.Add(GAMEID.MagellanPlus, Context.ActorOf(Props.Create(() => new MagellanPlusGameLogic()), "MagellanPlus"));
            _dicGameLogicActors.Add(GAMEID.MoreDiceAndRoll, Context.ActorOf(Props.Create(() => new MoreDiceAndRollGameLogic()), "MoreDiceAndRoll"));
            _dicGameLogicActors.Add(GAMEID.MoreLikeADiamond, Context.ActorOf(Props.Create(() => new MoreLikeADiamondGameLogic()), "MoreLikeADiamond"));
            _dicGameLogicActors.Add(GAMEID.OilCompanyII, Context.ActorOf(Props.Create(() => new OilCompanyIIGameLogic()), "OilCompanyII"));
            _dicGameLogicActors.Add(GAMEID.RetroStyle, Context.ActorOf(Props.Create(() => new RetroStyleGameLogic()), "RetroStyle"));
            _dicGameLogicActors.Add(GAMEID.RollingDice, Context.ActorOf(Props.Create(() => new RollingDiceGameLogic()), "RollingDice"));
            _dicGameLogicActors.Add(GAMEID.RoyalGardens, Context.ActorOf(Props.Create(() => new RoyalGardensGameLogic()), "RoyalGardens"));
            _dicGameLogicActors.Add(GAMEID.SummerBliss, Context.ActorOf(Props.Create(() => new SummerBlissGameLogic()), "SummerBliss"));
            _dicGameLogicActors.Add(GAMEID.TheGreatEgypt, Context.ActorOf(Props.Create(() => new TheGreatEgyptGameLogic()), "TheGreatEgypt"));
            _dicGameLogicActors.Add(GAMEID.N20Diamonds, Context.ActorOf(Props.Create(() => new N20DiamondsGameLogic()), "20Diamonds"));
            _dicGameLogicActors.Add(GAMEID.N20SuperDice, Context.ActorOf(Props.Create(() => new N20SuperDiceGameLogic()), "20SuperDice"));
            _dicGameLogicActors.Add(GAMEID.N2Dragons, Context.ActorOf(Props.Create(() => new N2DragonsGameLogic()), "2Dragons"));
            _dicGameLogicActors.Add(GAMEID.N40MegaClover, Context.ActorOf(Props.Create(() => new N40MegaCloverGameLogic()), "40MegaClover"));
            _dicGameLogicActors.Add(GAMEID.N5HotDice, Context.ActorOf(Props.Create(() => new N5HotDiceGameLogic()), "5HotDice"));
            _dicGameLogicActors.Add(GAMEID.BurningDice, Context.ActorOf(Props.Create(() => new BurningDiceGameLogic()), "BurningDice"));
            _dicGameLogicActors.Add(GAMEID.DiceAndRoll, Context.ActorOf(Props.Create(() => new DiceAndRollGameLogic()), "DiceAndRoll"));
            _dicGameLogicActors.Add(GAMEID.FlamingDice, Context.ActorOf(Props.Create(() => new FlamingDiceGameLogic()), "FlamingDice"));
            _dicGameLogicActors.Add(GAMEID.GameOfLuck, Context.ActorOf(Props.Create(() => new GameOfLuckGameLogic()), "GameOfLuck"));
            _dicGameLogicActors.Add(GAMEID.GreekFortune, Context.ActorOf(Props.Create(() => new GreekFortuneGameLogic()), "GreekFortune"));
            _dicGameLogicActors.Add(GAMEID.LuckyAndWild, Context.ActorOf(Props.Create(() => new LuckyAndWildGameLogic()), "LuckyAndWild"));
            _dicGameLogicActors.Add(GAMEID.LuckyHot, Context.ActorOf(Props.Create(() => new LuckyHotGameLogic()), "LuckyHot"));
            _dicGameLogicActors.Add(GAMEID.MoreLuckyAndWild, Context.ActorOf(Props.Create(() => new MoreLuckyAndWildGameLogic()), "MoreLuckyAndWild"));
            _dicGameLogicActors.Add(GAMEID.NeonDice, Context.ActorOf(Props.Create(() => new NeonDiceGameLogic()), "NeonDice"));
            _dicGameLogicActors.Add(GAMEID.Super20, Context.ActorOf(Props.Create(() => new Super20GameLogic()), "Super20"));
            _dicGameLogicActors.Add(GAMEID.TheWhiteWolf, Context.ActorOf(Props.Create(() => new TheWhiteWolfGameLogic()), "TheWhiteWolf"));
            _dicGameLogicActors.Add(GAMEID.FlamingHot, Context.ActorOf(Props.Create(() => new FlamingHotGameLogic()), "FlamingHot"));
            _dicGameLogicActors.Add(GAMEID.N100BurningHot, Context.ActorOf(Props.Create(() => new N100BurningHotGameLogic()), "100BurningHot"));
            _dicGameLogicActors.Add(GAMEID.N100SuperHot, Context.ActorOf(Props.Create(() => new N100SuperHotGameLogic()), "100SuperHot"));
            _dicGameLogicActors.Add(GAMEID.N10BurningHeart, Context.ActorOf(Props.Create(() => new N10BurningHeartGameLogic()), "10BurningHeart"));
            _dicGameLogicActors.Add(GAMEID.N20BurningHot, Context.ActorOf(Props.Create(() => new N20BurningHotGameLogic()), "20BurningHot"));
            _dicGameLogicActors.Add(GAMEID.N20HotBlast, Context.ActorOf(Props.Create(() => new N20HotBlastGameLogic()), "20HotBlast"));
            _dicGameLogicActors.Add(GAMEID.N40BurningHot, Context.ActorOf(Props.Create(() => new N40BurningHotGameLogic()), "40BurningHot"));
            _dicGameLogicActors.Add(GAMEID.N40BurningHot5, Context.ActorOf(Props.Create(() => new N40BurningHot5GameLogic()), "40BurningHot5"));
            _dicGameLogicActors.Add(GAMEID.N40LuckyKing, Context.ActorOf(Props.Create(() => new N40LuckyKingGameLogic()), "40LuckyKing"));
            _dicGameLogicActors.Add(GAMEID.N5BurningHeart, Context.ActorOf(Props.Create(() => new N5BurningHeartGameLogic()), "5BurningHeart"));
            _dicGameLogicActors.Add(GAMEID.AgeOfTroy, Context.ActorOf(Props.Create(() => new AgeOfTroyGameLogic()), "AgeOfTroy"));
            _dicGameLogicActors.Add(GAMEID.AmazingAmazonia, Context.ActorOf(Props.Create(() => new AmazingAmazoniaGameLogic()), "AmazingAmazonia"));
            _dicGameLogicActors.Add(GAMEID.BlueHeart, Context.ActorOf(Props.Create(() => new BlueHeartGameLogic()), "BlueHeart"));
            _dicGameLogicActors.Add(GAMEID.BookOfMagic, Context.ActorOf(Props.Create(() => new BookOfMagicGameLogic()), "BookOfMagic"));
            _dicGameLogicActors.Add(GAMEID.BurningHot, Context.ActorOf(Props.Create(() => new BurningHotGameLogic()), "BurningHot"));
            _dicGameLogicActors.Add(GAMEID.DazzlingHot, Context.ActorOf(Props.Create(() => new DazzlingHotGameLogic()), "DazzlingHot"));
            _dicGameLogicActors.Add(GAMEID.DragonReels, Context.ActorOf(Props.Create(() => new DragonReelsGameLogic()), "DragonReels"));
            _dicGameLogicActors.Add(GAMEID.FlamingHot6, Context.ActorOf(Props.Create(() => new FlamingHot6GameLogic()), "FlamingHot6"));
            _dicGameLogicActors.Add(GAMEID.FortuneSpells, Context.ActorOf(Props.Create(() => new FortuneSpellsGameLogic()), "FortuneSpells"));
            _dicGameLogicActors.Add(GAMEID.FruitsKingdom, Context.ActorOf(Props.Create(() => new FruitsKingdomGameLogic()), "FruitsKingdom"));
            _dicGameLogicActors.Add(GAMEID.GraceOfCleopatra, Context.ActorOf(Props.Create(() => new GraceOfCleopatraGameLogic()), "GraceOfCleopatra"));
            _dicGameLogicActors.Add(GAMEID.MajesticForest, Context.ActorOf(Props.Create(() => new MajesticForestGameLogic()), "MajesticForest"));
            _dicGameLogicActors.Add(GAMEID.OlympusGlory, Context.ActorOf(Props.Create(() => new OlympusGloryGameLogic()), "OlympusGlory"));
            _dicGameLogicActors.Add(GAMEID.RiseOfRa, Context.ActorOf(Props.Create(() => new RiseOfRaGameLogic()), "RiseOfRa"));
            _dicGameLogicActors.Add(GAMEID.RoyalSecrets, Context.ActorOf(Props.Create(() => new RoyalSecretsGameLogic()), "RoyalSecrets"));
            _dicGameLogicActors.Add(GAMEID.ShiningCrown, Context.ActorOf(Props.Create(() => new ShiningCrownGameLogic()), "ShiningCrown"));
            _dicGameLogicActors.Add(GAMEID.SuperHot20, Context.ActorOf(Props.Create(() => new SuperHot20GameLogic()), "SuperHot20"));
            _dicGameLogicActors.Add(GAMEID.SuperHot40, Context.ActorOf(Props.Create(() => new SuperHot40GameLogic()), "SuperHot40"));
            _dicGameLogicActors.Add(GAMEID.SupremeHot, Context.ActorOf(Props.Create(() => new SupremeHotGameLogic()), "SupremeHot"));
            _dicGameLogicActors.Add(GAMEID.TheBigJourney, Context.ActorOf(Props.Create(() => new TheBigJourneyGameLogic()), "TheBigJourney"));
            _dicGameLogicActors.Add(GAMEID.UltimateHot, Context.ActorOf(Props.Create(() => new UltimateHotGameLogic()), "UltimateHot"));
            _dicGameLogicActors.Add(GAMEID.VersaillesGold, Context.ActorOf(Props.Create(() => new VersaillesGoldGameLogic()), "VersaillesGold"));
            _dicGameLogicActors.Add(GAMEID.WirchesCharm, Context.ActorOf(Props.Create(() => new WirchesCharmGameLogic()), "WirchesCharm"));
            _dicGameLogicActors.Add(GAMEID.ZodiacWheel, Context.ActorOf(Props.Create(() => new ZodiacWheelGameLogic()), "ZodiacWheel"));
            _dicGameLogicActors.Add(GAMEID.CandyPalace, Context.ActorOf(Props.Create(() => new CandyPalaceGameLogic()), "CandyPalace"));
            _dicGameLogicActors.Add(GAMEID.CavemenAndDinosaurs, Context.ActorOf(Props.Create(() => new CavemenAndDinosaursGameLogic()), "CavemenAndDinosaurs"));
            _dicGameLogicActors.Add(GAMEID.DiceAndDinosaurs, Context.ActorOf(Props.Create(() => new DiceAndDinosaursGameLogic()), "DiceAndDinosaurs"));
            _dicGameLogicActors.Add(GAMEID.CoinGobblerChristmasEdition, Context.ActorOf(Props.Create(() => new CoinGobblerChristmasEditionGameLogic()), "CoinGobblerChristmasEdition"));
            _dicGameLogicActors.Add(GAMEID.CandyPalaceChristmasEdition, Context.ActorOf(Props.Create(() => new CandyPalaceChristmasEditionGameLogic()), "CandyPalaceChristmasEdition"));
            _dicGameLogicActors.Add(GAMEID.CoinGobbler, Context.ActorOf(Props.Create(() => new CoinGobblerGameLogic()), "CoinGobbler"));
            _dicGameLogicActors.Add(GAMEID.Stoichkov, Context.ActorOf(Props.Create(() => new StoichkovGameLogic()), "Stoichkov"));
            _dicGameLogicActors.Add(GAMEID.AztecDice, Context.ActorOf(Props.Create(() => new AztecDiceGameLogic()), "AztecDice"));
            _dicGameLogicActors.Add(GAMEID.KemetsTreasures, Context.ActorOf(Props.Create(() => new KemetsTreasuresGameLogic()), "KemetsTreasures"));
            _dicGameLogicActors.Add(GAMEID.AztectForest, Context.ActorOf(Props.Create(() => new AztectForestGameLogic()), "AztectForest"));
            _dicGameLogicActors.Add(GAMEID.CandyDice, Context.ActorOf(Props.Create(() => new CandyDiceGameLogic()), "CandyDice"));
            _dicGameLogicActors.Add(GAMEID.FruityTime, Context.ActorOf(Props.Create(() => new FruityTimeGameLogic()), "FruityTime"));
            _dicGameLogicActors.Add(GAMEID.N20PirateBombs, Context.ActorOf(Props.Create(() => new N20PirateBombsGameLogic()), "20PirateBombs"));
            _dicGameLogicActors.Add(GAMEID.N40WenshiLion, Context.ActorOf(Props.Create(() => new N40WenshiLionGameLogic()), "40WenshiLion"));
            _dicGameLogicActors.Add(GAMEID.LuckyWood, Context.ActorOf(Props.Create(() => new LuckyWoodGameLogic()), "LuckyWood"));
            _dicGameLogicActors.Add(GAMEID.SecretsOfAlchemy, Context.ActorOf(Props.Create(() => new SecretsOfAlchemyGameLogic()), "SecretsOfAlchemy"));
            _dicGameLogicActors.Add(GAMEID.MythicalTreasure, Context.ActorOf(Props.Create(() => new MythicalTreasureGameLogic()), "MythicalTreasure"));
            _dicGameLogicActors.Add(GAMEID.N20GoldenCoins, Context.ActorOf(Props.Create(() => new N20GoldenCoinsGameLogic()), "20GoldenCoins"));
            _dicGameLogicActors.Add(GAMEID.N100GoldenCoins, Context.ActorOf(Props.Create(() => new N100GoldenCoinsGameLogic()), "100GoldenCoins"));
            _dicGameLogicActors.Add(GAMEID.Great27, Context.ActorOf(Props.Create(() => new Great27GameLogic()), "Great27"));
            _dicGameLogicActors.Add(GAMEID.N100BulkyFruits, Context.ActorOf(Props.Create(() => new N100BulkyFruitsGameLogic()), "100BulkyFruits"));
            _dicGameLogicActors.Add(GAMEID.N40BulkyFruits, Context.ActorOf(Props.Create(() => new N40BulkyFruitsGameLogic()), "40BulkyFruits"));
            _dicGameLogicActors.Add(GAMEID.N40GoldenCoins, Context.ActorOf(Props.Create(() => new N40GoldenCoinsGameLogic()), "40GoldenCoins"));
            _dicGameLogicActors.Add(GAMEID.ExtraCrown, Context.ActorOf(Props.Create(() => new ExtraCrownGameLogic()), "ExtraCrown"));
            _dicGameLogicActors.Add(GAMEID.N7Crystals, Context.ActorOf(Props.Create(() => new N7CrystalsGameLogic()), "7Crystals"));
            #endregion

            foreach (KeyValuePair<GAMEID, IActorRef> pair in _dicGameLogicActors)
            {
                pair.Value.Tell(new DBProxyInform(_dbReader, _dbWriter, _redisWriter));
                pair.Value.Tell("loadSetting");
                _hashAllChildActors.Add(pair.Value.Path.Name);
                Context.Watch(pair.Value);
            }
        }

        private async Task OnLoadSpinDatabase(LoadSpinDataRequest request)
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

        private async Task OnPerformanceTest(PerformanceTestRequest request)
        {
            foreach (KeyValuePair<GAMEID, IActorRef> pair in _dicGameLogicActors)
            {
                await _dicGameLogicActors[pair.Key].Ask<bool>(request);
            }

            Sender.Tell(true);
        }

        private void OnTerminated(Terminated terminated)
        {
            _hashAllChildActors.Remove(terminated.ActorRef.Path.Name);
            if(_hashAllChildActors.Count == 0)
            {
                Self.Tell(PoisonPill.Instance);
            }
        }
        private void ProcessCommand(string command)
        {
            if(command == "terminate")
            {
                foreach (KeyValuePair<GAMEID, IActorRef> pair in _dicGameLogicActors)
                    pair.Value.Tell(PoisonPill.Instance);
            }
        }
        private void OnEnterGameUser(EnterGameRequest enterGameMessage)
        {
            GAMEID gameID = (GAMEID) enterGameMessage.GameID;
            if (!_dicGameLogicActors.ContainsKey(gameID))
            {
                Sender.Tell(new EnterGameResponse((int) gameID, Self, 1));  //해당 게임이 존재하지 않음
                return;
            }

            _dicGameLogicActors[gameID].Forward(enterGameMessage);
        }
        private void OnPayoutConfigUpdated(PayoutConfigUpdated updated)
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
