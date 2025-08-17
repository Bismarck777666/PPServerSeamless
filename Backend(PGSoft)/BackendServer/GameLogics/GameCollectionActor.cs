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
using SlotGamesNode.GameLogics.PGGames;

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

            Receive<EnterGameRequest>           (_ => onEnterGameUser(_));
            Receive<string>                     (_ => processCommand(_));
            Receive<Terminated>                 (_ => onTerminated(_));
            ReceiveAsync<LoadSpinDataRequest>   (onLoadSpinDatabase);
        }

        protected override void PreStart()
        {
            base.PreStart();
            createGameLogicActors();
        }

        protected void createGameLogicActors()
        {
            _dicGameLogicActors.Add(GAMEID.TreasureOfAztec,     Context.ActorOf(Props.Create(() => new TreasureOfAztecGameLogic()),     "TreasureOfAztec"));
            _dicGameLogicActors.Add(GAMEID.LuckyNeko,           Context.ActorOf(Props.Create(() => new LuckyNekoGameLogic()),           "LuckyNeko"));
            _dicGameLogicActors.Add(GAMEID.MahjongWays2,        Context.ActorOf(Props.Create(() => new MahjongWays2GameLogic()),        "MahjongWays2"));
            _dicGameLogicActors.Add(GAMEID.MahjongWays,         Context.ActorOf(Props.Create(() => new MahjongWaysGameLogic()),         "MahjongWays"));
            _dicGameLogicActors.Add(GAMEID.WaysOfTheQilin,      Context.ActorOf(Props.Create(() => new WaysOfTheQilinGameLogic()),      "WaysOfTheQilin"));
            _dicGameLogicActors.Add(GAMEID.WildBountyShowdown,  Context.ActorOf(Props.Create(() => new WildBountyShowdownGameLogic()),  "WildBountyShowdown"));
            _dicGameLogicActors.Add(GAMEID.WildBandito,         Context.ActorOf(Props.Create(() => new WildBanditoGameLogic()),         "WildBandito"));
            _dicGameLogicActors.Add(GAMEID.FortuneOx,           Context.ActorOf(Props.Create(() => new FortuneOxGameLogic()),           "FortuneOx"));
            _dicGameLogicActors.Add(GAMEID.GaneshaFortune,      Context.ActorOf(Props.Create(() => new GaneshaFortuneGameLogic()),      "GaneshaFortune"));
            _dicGameLogicActors.Add(GAMEID.AsgardianRising,     Context.ActorOf(Props.Create(() => new AsgardianRisingGameLogic()),     "AsgardianRising"));
            _dicGameLogicActors.Add(GAMEID.CaishenWins,         Context.ActorOf(Props.Create(() => new CaishenWinsGameLogic()),         "CaishenWins"));

            _dicGameLogicActors.Add(GAMEID.CocktailNights,          Context.ActorOf(Props.Create(() => new CocktailNightsGameLogic()),          "CocktailNights"));
            _dicGameLogicActors.Add(GAMEID.DinnerDelights,          Context.ActorOf(Props.Create(() => new DinnerDelightsGameLogic()),          "DinnerDelights"));
            _dicGameLogicActors.Add(GAMEID.EgyptBookOfMystery,      Context.ActorOf(Props.Create(() => new EgyptBookOfMysteryGameLogic()),      "EgyptBookOfMystery"));
            _dicGameLogicActors.Add(GAMEID.FortuneTiger,            Context.ActorOf(Props.Create(() => new FortuneTigerGameLogic()),            "FortuneTiger"));
            _dicGameLogicActors.Add(GAMEID.JurassicKingdom,         Context.ActorOf(Props.Create(() => new JurassicKingdomGameLogic()),         "JurassicKingdom"));
            _dicGameLogicActors.Add(GAMEID.LegendOfPerseus,         Context.ActorOf(Props.Create(() => new LegendOfPerseusGameLogic()),         "LegendOfPerseus"));
            _dicGameLogicActors.Add(GAMEID.MidasFortune,            Context.ActorOf(Props.Create(() => new MidasFortuneGameLogic()),            "MidasFortune"));
            _dicGameLogicActors.Add(GAMEID.ProsperityFortuneTree,   Context.ActorOf(Props.Create(() => new ProsperityFortuneTreeGameLogic()),   "ProsperityFortuneTree"));
            _dicGameLogicActors.Add(GAMEID.ThaiRiverWonders,        Context.ActorOf(Props.Create(() => new ThaiRiverWondersGameLogic()),        "ThaiRiverWonders"));
            

            _dicGameLogicActors.Add(GAMEID.CaptainBounty,           Context.ActorOf(Props.Create(() => new CaptainBountyGameLogic()), "CaptainBounty"));
            _dicGameLogicActors.Add(GAMEID.DragonHatch,             Context.ActorOf(Props.Create(() => new DragonHatchGameLogic()), "DragonHatch"));
            _dicGameLogicActors.Add(GAMEID.DreamsOfMacau,           Context.ActorOf(Props.Create(() => new DreamsOfMacauGameLogic()), "DreamsOfMacau"));
            _dicGameLogicActors.Add(GAMEID.LeprechaunRiches,        Context.ActorOf(Props.Create(() => new LeprechaunRichesGameLogic()), "LeprechaunRiches"));
            _dicGameLogicActors.Add(GAMEID.LuckyPiggy,              Context.ActorOf(Props.Create(() => new LuckyPiggyGameLogic()), "LuckyPiggy"));
            _dicGameLogicActors.Add(GAMEID.QueenOfBounty,           Context.ActorOf(Props.Create(() => new QueenOfBountyGameLogic()), "QueenOfBounty"));
            _dicGameLogicActors.Add(GAMEID.SpeedWinner,             Context.ActorOf(Props.Create(() => new SpeedWinnerGameLogic()), "SpeedWinner"));
            _dicGameLogicActors.Add(GAMEID.TheGreatIcescape,        Context.ActorOf(Props.Create(() => new TheGreatIcescapeGameLogic()), "TheGreatIcescape"));
            _dicGameLogicActors.Add(GAMEID.TheQueenBanquet,         Context.ActorOf(Props.Create(() => new TheQueenBanquetGameLogic()), "TheQueenBanquet"));
            _dicGameLogicActors.Add(GAMEID.WildCoaster,             Context.ActorOf(Props.Create(() => new WildCoasterGameLogic()), "WildCoaster"));

            _dicGameLogicActors.Add(GAMEID.ButterflyBlossom,    Context.ActorOf(Props.Create(() => new ButterflyBlossomGameLogic()), "ButterflyBlossom"));
            _dicGameLogicActors.Add(GAMEID.CryptoGold,          Context.ActorOf(Props.Create(() => new CryptoGoldGameLogic()), "CryptoGold"));
            _dicGameLogicActors.Add(GAMEID.DoubleFortune,       Context.ActorOf(Props.Create(() => new DoubleFortuneGameLogic()), "DoubleFortune"));
            _dicGameLogicActors.Add(GAMEID.FortuneMouse,        Context.ActorOf(Props.Create(() => new FortuneMouseGameLogic()), "FortuneMouse"));
            _dicGameLogicActors.Add(GAMEID.OrientalProsperity,  Context.ActorOf(Props.Create(() => new OrientalProsperityGameLogic()), "OrientalProsperity"));
            _dicGameLogicActors.Add(GAMEID.PhoenixRises,        Context.ActorOf(Props.Create(() => new PhoenixRisesGameLogic()), "PhoenixRises"));
            _dicGameLogicActors.Add(GAMEID.RiseOfApollo,        Context.ActorOf(Props.Create(() => new RiseOfApolloGameLogic()), "RiseOfApollo"));
            _dicGameLogicActors.Add(GAMEID.RoosterRumble,       Context.ActorOf(Props.Create(() => new RoosterRumbleGameLogic()), "RoosterRumble"));
            _dicGameLogicActors.Add(GAMEID.ShaolinSoccer,       Context.ActorOf(Props.Create(() => new ShaolinSoccerGameLogic()), "ShaolinSoccer"));
            _dicGameLogicActors.Add(GAMEID.WildFireworks,       Context.ActorOf(Props.Create(() => new WildFireworksGameLogic()), "WildFireworks"));


            _dicGameLogicActors.Add(GAMEID.AlchemyGold,         Context.ActorOf(Props.Create(() => new AlchemyGoldGameLogic()), "AlchemyGold"));
            _dicGameLogicActors.Add(GAMEID.BattlegroundRoyale,  Context.ActorOf(Props.Create(() => new BattlegroundRoyaleGameLogic()), "BattlegroundRoyale"));
            _dicGameLogicActors.Add(GAMEID.CandyBonanza,        Context.ActorOf(Props.Create(() => new CandyBonanzaGameLogic()), "CandyBonanza"));
            _dicGameLogicActors.Add(GAMEID.FortuneRabbit,       Context.ActorOf(Props.Create(() => new FortuneRabbitGameLogic()), "FortuneRabbit"));
            _dicGameLogicActors.Add(GAMEID.GarudaGems,          Context.ActorOf(Props.Create(() => new GarudaGemsGameLogic()), "GarudaGems"));
            _dicGameLogicActors.Add(GAMEID.MajesticTreasures,   Context.ActorOf(Props.Create(() => new MajesticTreasuresGameLogic()), "MajesticTreasures"));
            _dicGameLogicActors.Add(GAMEID.MuayThaiChampion,    Context.ActorOf(Props.Create(() => new MuayThaiChampionGameLogic()), "MuayThaiChampion"));
            _dicGameLogicActors.Add(GAMEID.SecretOfCleopatra,   Context.ActorOf(Props.Create(() => new SecretOfCleopatraGameLogic()), "SecretOfCleopatra"));
            _dicGameLogicActors.Add(GAMEID.SpiritedWonders,     Context.ActorOf(Props.Create(() => new SpiritedWondersGameLogic()), "SpiritedWonders"));
            _dicGameLogicActors.Add(GAMEID.SupermarketSpree,    Context.ActorOf(Props.Create(() => new SupermarketSpreeGameLogic()), "SupermarketSpree"));
            
            _dicGameLogicActors.Add(GAMEID.CandyBurst,              Context.ActorOf(Props.Create(() => new CandyBurstGameLogic()), "CandyBurst"));
            _dicGameLogicActors.Add(GAMEID.CircusDelight,           Context.ActorOf(Props.Create(() => new CircusDelightGameLogic()), "CircusDelight"));
            _dicGameLogicActors.Add(GAMEID.DestinyOfSunAndMoon,     Context.ActorOf(Props.Create(() => new DestinyOfSunAndMoonGameLogic()), "DestinyOfSunAndMoon"));
            _dicGameLogicActors.Add(GAMEID.DragonLegend,            Context.ActorOf(Props.Create(() => new DragonLegendGameLogic()), "DragonLegend"));
            _dicGameLogicActors.Add(GAMEID.DragonTigerLuck,         Context.ActorOf(Props.Create(() => new DragonTigerLuckGameLogic()), "DragonTigerLuck"));
            _dicGameLogicActors.Add(GAMEID.EmperorsFavour,          Context.ActorOf(Props.Create(() => new EmperorsFavourGameLogic()), "EmperorsFavour"));
            _dicGameLogicActors.Add(GAMEID.FortuneGods,             Context.ActorOf(Props.Create(() => new FortuneGodsGameLogic()), "FortuneGods"));
            _dicGameLogicActors.Add(GAMEID.GalacticGems,            Context.ActorOf(Props.Create(() => new GalacticGemsGameLogic()), "GalacticGems"));
            _dicGameLogicActors.Add(GAMEID.GaneshaGold,             Context.ActorOf(Props.Create(() => new GaneshaGoldGameLogic()), "GaneshaGold"));
            _dicGameLogicActors.Add(GAMEID.GemSaviourConquest,      Context.ActorOf(Props.Create(() => new GemSaviourConquestGameLogic()), "GemSaviourConquest"));
            _dicGameLogicActors.Add(GAMEID.Genies3Wishes,           Context.ActorOf(Props.Create(() => new Genies3WishesGameLogic()), "Genies3Wishes"));
            _dicGameLogicActors.Add(GAMEID.GuardiansOfIceAndFire,   Context.ActorOf(Props.Create(() => new GuardiansOfIceAndFireGameLogic()), "GuardiansOfIceAndFire"));
            _dicGameLogicActors.Add(GAMEID.HeistStakes,             Context.ActorOf(Props.Create(() => new HeistStakesGameLogic()), "HeistStakes"));
            _dicGameLogicActors.Add(GAMEID.JewelsOfProsperity,      Context.ActorOf(Props.Create(() => new JewelsOfProsperityGameLogic()), "JewelsOfProsperity"));
            _dicGameLogicActors.Add(GAMEID.JourneyToTheWealth,      Context.ActorOf(Props.Create(() => new JourneyToTheWealthGameLogic()), "JourneyToTheWealth"));
            _dicGameLogicActors.Add(GAMEID.LegendaryMonkeyKing,     Context.ActorOf(Props.Create(() => new LegendaryMonkeyKingGameLogic()), "LegendaryMonkeyKing"));
            _dicGameLogicActors.Add(GAMEID.MermaidRiches,           Context.ActorOf(Props.Create(() => new MermaidRichesGameLogic()), "MermaidRiches"));
            _dicGameLogicActors.Add(GAMEID.OperaDynasty,            Context.ActorOf(Props.Create(() => new OperaDynastyGameLogic()), "OperaDynasty"));
            _dicGameLogicActors.Add(GAMEID.TotemWonders,            Context.ActorOf(Props.Create(() => new TotemWondersGameLogic()), "TotemWonders"));
            _dicGameLogicActors.Add(GAMEID.WinWinWon,               Context.ActorOf(Props.Create(() => new WinWinWonGameLogic()), "WinWinWon"));

            
            _dicGameLogicActors.Add(GAMEID.BaliVacation,        Context.ActorOf(Props.Create(() => new BaliVacationGameLogic()), "BaliVacation"));
            _dicGameLogicActors.Add(GAMEID.BikiniParadise,      Context.ActorOf(Props.Create(() => new BikiniParadiseGameLogic()), "BikiniParadise"));
            _dicGameLogicActors.Add(GAMEID.BuffaloWin,          Context.ActorOf(Props.Create(() => new BuffaloWinGameLogic()), "BuffaloWin"));
            _dicGameLogicActors.Add(GAMEID.EmojiRiches,         Context.ActorOf(Props.Create(() => new EmojiRichesGameLogic()), "EmojiRiches"));
            _dicGameLogicActors.Add(GAMEID.FlirtingScholar,     Context.ActorOf(Props.Create(() => new FlirtingScholarGameLogic()), "FlirtingScholar"));
            _dicGameLogicActors.Add(GAMEID.HawaiianTiki,        Context.ActorOf(Props.Create(() => new HawaiianTikiGameLogic()), "HawaiianTiki"));
            _dicGameLogicActors.Add(GAMEID.HoneyTrapOfDiaoChan, Context.ActorOf(Props.Create(() => new HoneyTrapOfDiaoChanGameLogic()), "HoneyTrapOfDiaoChan"));
            _dicGameLogicActors.Add(GAMEID.JackFrostsWinter,    Context.ActorOf(Props.Create(() => new JackFrostsWinterGameLogic()), "JackFrostsWinter"));
            _dicGameLogicActors.Add(GAMEID.JungleDelight,       Context.ActorOf(Props.Create(() => new JungleDelightGameLogic()), "JungleDelight"));
            _dicGameLogicActors.Add(GAMEID.LegendOfHouYi,       Context.ActorOf(Props.Create(() => new LegendOfHouYiGameLogic()), "LegendOfHouYi"));
            _dicGameLogicActors.Add(GAMEID.MaskCarnival,        Context.ActorOf(Props.Create(() => new MaskCarnivalGameLogic()), "MaskCarnival"));
            _dicGameLogicActors.Add(GAMEID.Medusa2,             Context.ActorOf(Props.Create(() => new Medusa2GameLogic()), "Medusa2"));
            _dicGameLogicActors.Add(GAMEID.MrHallowWin,         Context.ActorOf(Props.Create(() => new MrHallowWinGameLogic()), "MrHallowWin"));
            _dicGameLogicActors.Add(GAMEID.NinjaVsSamurai,      Context.ActorOf(Props.Create(() => new NinjaVsSamuraiGameLogic()), "NinjaVsSamurai"));
            _dicGameLogicActors.Add(GAMEID.PiggyGold,           Context.ActorOf(Props.Create(() => new PiggyGoldGameLogic()), "PiggyGold"));
            _dicGameLogicActors.Add(GAMEID.PlushieFrenzy,       Context.ActorOf(Props.Create(() => new PlushieFrenzyGameLogic()), "PlushieFrenzy"));
            _dicGameLogicActors.Add(GAMEID.ProsperityLion,      Context.ActorOf(Props.Create(() => new ProsperityLionGameLogic()), "ProsperityLion"));
            _dicGameLogicActors.Add(GAMEID.RaiderJanesCryptOfFortune, Context.ActorOf(Props.Create(() => new RaiderJanesCryptOfFortuneGameLogic()), "RaiderJanesCryptOfFortune"));
            _dicGameLogicActors.Add(GAMEID.ReelLove,            Context.ActorOf(Props.Create(() => new ReelLoveGameLogic()), "ReelLove"));
            _dicGameLogicActors.Add(GAMEID.WinWinFishPrawnCrab, Context.ActorOf(Props.Create(() => new WinWinFishPrawnCrabGameLogic()), "WinWinFishPrawnCrab"));

            
            _dicGameLogicActors.Add(GAMEID.BakeryBonanza,       Context.ActorOf(Props.Create(() => new BakeryBonanzaGameLogic()), "BakeryBonanza"));
            _dicGameLogicActors.Add(GAMEID.GemSaviourSword,     Context.ActorOf(Props.Create(() => new GemSaviourSwordGameLogic()), "GemSaviourSword"));
            _dicGameLogicActors.Add(GAMEID.HipHopPanda,         Context.ActorOf(Props.Create(() => new HipHopPandaGameLogic()), "HipHopPanda"));
            _dicGameLogicActors.Add(GAMEID.Medusa,              Context.ActorOf(Props.Create(() => new MedusaGameLogic()), "Medusa"));
            _dicGameLogicActors.Add(GAMEID.MysticalSpirits,     Context.ActorOf(Props.Create(() => new MysticalSpiritsGameLogic()), "MysticalSpirits"));
            _dicGameLogicActors.Add(GAMEID.RavePartyFever,      Context.ActorOf(Props.Create(() => new RavePartyFeverGameLogic()), "RavePartyFever"));
            _dicGameLogicActors.Add(GAMEID.SantasGiftRush,      Context.ActorOf(Props.Create(() => new SantasGiftRushGameLogic()), "SantasGiftRush"));
            _dicGameLogicActors.Add(GAMEID.SongkranSplash,      Context.ActorOf(Props.Create(() => new SongkranSplashGameLogic()), "SongkranSplash"));
            _dicGameLogicActors.Add(GAMEID.SuperGolfDrive,      Context.ActorOf(Props.Create(() => new SuperGolfDriveGameLogic()), "SuperGolfDrive"));
            _dicGameLogicActors.Add(GAMEID.VampiresCharm,       Context.ActorOf(Props.Create(() => new VampiresCharmGameLogic()), "VampiresCharm"));
            
            _dicGameLogicActors.Add(GAMEID.FruityCandy,         Context.ActorOf(Props.Create(() => new FruityCandyGameLogic()),             "FruityCandy"));
            _dicGameLogicActors.Add(GAMEID.LuckyCloverLady,     Context.ActorOf(Props.Create(() => new LuckyCloverLadyGameLogic()),         "LuckyCloverLady"));
            _dicGameLogicActors.Add(GAMEID.CruiseRoyale,        Context.ActorOf(Props.Create(() => new CruiseRoyaleGameLogic()),            "CruiseRoyale"));
            _dicGameLogicActors.Add(GAMEID.SafariWilds,         Context.ActorOf(Props.Create(() => new SafariWildsGameLogic()),             "SafariWilds"));
            _dicGameLogicActors.Add(GAMEID.GladiatorsGlory,     Context.ActorOf(Props.Create(() => new GladiatorsGloryGameLogic()),         "GladiatorsGlory"));
            _dicGameLogicActors.Add(GAMEID.NinjaRaccoonFrenzy,  Context.ActorOf(Props.Create(() => new NinjaRaccoonFrenzyGameLogic()),      "NinjaRaccoonFrenzy"));
            _dicGameLogicActors.Add(GAMEID.UltimateStriker,     Context.ActorOf(Props.Create(() => new UltimateStrikerGameLogic()),         "UltimateStriker"));
            _dicGameLogicActors.Add(GAMEID.WildHeistCashout,    Context.ActorOf(Props.Create(() => new WildHeistCashoutGameLogic()),        "WildHeistCashout"));
            _dicGameLogicActors.Add(GAMEID.ForgeOfWealth,       Context.ActorOf(Props.Create(() => new ForgeOfWealthGameLogic()),           "ForgeOfWealth"));
            _dicGameLogicActors.Add(GAMEID.MafiaMayhem,         Context.ActorOf(Props.Create(() => new MafiaMayhemGameLogic()),             "MafiaMayhem"));
            _dicGameLogicActors.Add(GAMEID.TsarTreasures,       Context.ActorOf(Props.Create(() => new TsarTreasuresGameLogic()),           "TsarTreasures"));
            _dicGameLogicActors.Add(GAMEID.WereWolfsHunt,       Context.ActorOf(Props.Create(() => new WereWolfsHuntGameLogic()),           "WereWolfsHunt"));
            _dicGameLogicActors.Add(GAMEID.DragonHatch2,        Context.ActorOf(Props.Create(() => new DragonHatch2GameLogic()),            "DragonHatch2"));
            _dicGameLogicActors.Add(GAMEID.FortuneDragon,       Context.ActorOf(Props.Create(() => new FortuneDragonGameLogic()),           "FortuneDragon"));


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
                    string strGameName  = (string)infoDocument["name"];
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
                Sender.Tell(new EnterGameResponse(gameID, Self, 1, null, null));  
                return;
            }
            _dicGameLogicActors[gameID].Forward(enterGameMessage);
        }
    }

    public class LoadSpinDataRequest
    {

    }

    public class PayoutRateTest
    {

    }
}
