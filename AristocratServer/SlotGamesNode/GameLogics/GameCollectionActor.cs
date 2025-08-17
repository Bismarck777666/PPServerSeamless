using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using GITProtocol;
using SlotGamesNode.Database;
using Akka.Event;
using SlotGamesNode.GameLogics.AristoGames;
//using SlotGamesNode.GameLogics.AristoGames;

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
            Receive<PayoutConfigUpdated>        (_ => onPayoutConfigUpdated(_));
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
            _dicGameLogicActors.Add(GAMEID.QueenOfNile, Context.ActorOf(Props.Create(() => new QueenOfNileGameLogic()), "QueenOfNile"));
            _dicGameLogicActors.Add(GAMEID.SaharaGold, Context.ActorOf(Props.Create(() => new SaharaGoldGameLogic()), "SaharaGold"));
            _dicGameLogicActors.Add(GAMEID.DragonEmperor, Context.ActorOf(Props.Create(() => new DragonEmperorGameLogic()), "DragonEmperor"));
            _dicGameLogicActors.Add(GAMEID.PandaMagic, Context.ActorOf(Props.Create(() => new PandaMagicGameLogic()), "PandaMagic"));
            _dicGameLogicActors.Add(GAMEID.HappyProsperous, Context.ActorOf(Props.Create(() => new HappyProsperousGameLogic()), "HappyProsperous"));
            _dicGameLogicActors.Add(GAMEID.PeaceLongLife, Context.ActorOf(Props.Create(() => new PeaceLongLifeGameLogic()), "PeaceLongLife"));
            _dicGameLogicActors.Add(GAMEID.Geisha, Context.ActorOf(Props.Create(() => new GeishaGameLogic()), "Geisha"));
            _dicGameLogicActors.Add(GAMEID.LuckyCount, Context.ActorOf(Props.Create(() => new LuckyCountGameLogic()), "LuckyCount"));
            _dicGameLogicActors.Add(GAMEID.SunMoon, Context.ActorOf(Props.Create(() => new SunMoonGameLogic()), "SunMoon"));
            _dicGameLogicActors.Add(GAMEID.Dragons50, Context.ActorOf(Props.Create(() => new Dragons50GameLogic()), "Dragons50"));
            _dicGameLogicActors.Add(GAMEID.Lions50, Context.ActorOf(Props.Create(() => new Lions50GameLogic()), "Lions50"));
            _dicGameLogicActors.Add(GAMEID.DoubleHappiness, Context.ActorOf(Props.Create(() => new DoubleHappinessGameLogic()), "DoubleHappiness"));
            _dicGameLogicActors.Add(GAMEID.BigRed, Context.ActorOf(Props.Create(() => new BigRedGameLogic()), "BigRed"));
            _dicGameLogicActors.Add(GAMEID.BigBen, Context.ActorOf(Props.Create(() => new BigBenGameLogic()), "BigBen"));
            _dicGameLogicActors.Add(GAMEID.Buffalo, Context.ActorOf(Props.Create(() => new BuffaloGameLogic()), "Buffalo"));
            _dicGameLogicActors.Add(GAMEID.MissKitty, Context.ActorOf(Props.Create(() => new MissKittyGameLogic()), "MissKitty"));
            _dicGameLogicActors.Add(GAMEID.TikiTorch, Context.ActorOf(Props.Create(() => new TikiTorchGameLogic()), "TikiTorch"));
            _dicGameLogicActors.Add(GAMEID.FlameofOlympus, Context.ActorOf(Props.Create(() => new FlameofOlympusGameLogic()), "FlameofOlympus"));
            _dicGameLogicActors.Add(GAMEID.JaguarMist, Context.ActorOf(Props.Create(() => new JaguarMistGameLogic()), "JaguarMist"));
            _dicGameLogicActors.Add(GAMEID.PelicanPete, Context.ActorOf(Props.Create(() => new PelicanPeteGameLogic()), "PelicanPete"));
            _dicGameLogicActors.Add(GAMEID.DolphinsTreasure, Context.ActorOf(Props.Create(() => new DolphinsTreasureGameLogic()), "DolphinsTreasure"));
            _dicGameLogicActors.Add(GAMEID.MoonFestival, Context.ActorOf(Props.Create(() => new MoonFestivalGameLogic()), "MoonFestival"));
            _dicGameLogicActors.Add(GAMEID.FireLight, Context.ActorOf(Props.Create(() => new FireLightGameLogic()), "FireLight"));
            _dicGameLogicActors.Add(GAMEID.WerewolfWild, Context.ActorOf(Props.Create(() => new WerewolfWildGameLogic()), "WerewolfWild"));
            _dicGameLogicActors.Add(GAMEID.Pompeii, Context.ActorOf(Props.Create(() => new PompeiiGameLogic()), "Pompeii"));
            _dicGameLogicActors.Add(GAMEID.SilkRoad, Context.ActorOf(Props.Create(() => new SilkRoadGameLogic()), "SilkRoad"));
            _dicGameLogicActors.Add(GAMEID.WildSplash, Context.ActorOf(Props.Create(() => new WildSplashGameLogic()), "WildSplash"));
            _dicGameLogicActors.Add(GAMEID.TigerPrincess, Context.ActorOf(Props.Create(() => new TigerPrincessGameLogic()), "TigerPrincess"));
            _dicGameLogicActors.Add(GAMEID.FortunePrincess, Context.ActorOf(Props.Create(() => new FortunePrincessGameLogic()), "FortunePrincess"));
            _dicGameLogicActors.Add(GAMEID.HighStakes, Context.ActorOf(Props.Create(() => new HighStakesGameLogic()), "HighStakes"));
            _dicGameLogicActors.Add(GAMEID.HeartThrob, Context.ActorOf(Props.Create(() => new HeartThrobGameLogic()), "HeartThrob"));
            _dicGameLogicActors.Add(GAMEID.GoldenCentury, Context.ActorOf(Props.Create(() => new GoldenCenturyGameLogic()), "GoldenCentury"));

            List<string> strSQLs = new List<string>();
            foreach (KeyValuePair<GAMEID, IActorRef> pair in _dicGameLogicActors)
            {
                string strSQL = pair.Value.Ask<string>(new InsertStatement()).Result;
                strSQLs.Add(strSQL);
            }
            
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
                foreach (KeyValuePair<GAMEID, IActorRef> pair in _dicGameLogicActors)
                {
                    bool isSuccess = await pair.Value.Ask<bool>(request);
                    if (!isSuccess)
                    {
                        Sender.Tell(false);
                        return;
                    }
                }
                Sender.Tell(true);
            }
            catch(Exception ex)
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
    public class CopyDataRequest
    {

    }
    public class InsertStatement
    {

    }

}
