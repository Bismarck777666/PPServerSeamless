using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using GITProtocol;
using SlotGamesNode.Database;
using Akka.Event;

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
            ReceiveAsync<PerformanceTestRequest>(onPerformanceTest);
        }

        protected override void PreStart()
        {
            base.PreStart();
            createGameLogicActors();
        }

        protected void createGameLogicActors()
        {
            //슬롯게임들
            #region PP Games
            _dicGameLogicActors.Add(GAMEID.PandasFortune,               Context.ActorOf(Props.Create(() => new PandasFortuneGameLogic()),           "PandasFortune"));
            _dicGameLogicActors.Add(GAMEID.AncientEgyptClassic,         Context.ActorOf(Props.Create(() => new AncientEgyptClassicGameLogic()),     "AncientEgyptClassic"));
            _dicGameLogicActors.Add(GAMEID.BeoWulf,                     Context.ActorOf(Props.Create(() => new BeoWulfGameLogic()),                 "BeoWulf"));
            _dicGameLogicActors.Add(GAMEID.BuffaloKing,                 Context.ActorOf(Props.Create(() => new BuffaloKingGameLogic()),             "BuffaloKing"));
            _dicGameLogicActors.Add(GAMEID.JuicyFruit,                  Context.ActorOf(Props.Create(() => new JuicyFruitGameLogic()),              "JuicyFruit"));
            _dicGameLogicActors.Add(GAMEID.SpartaKing,                  Context.ActorOf(Props.Create(() => new SpartaKingGameLogic()),              "SpartaKing"));
            _dicGameLogicActors.Add(GAMEID.StarzMegaWays,               Context.ActorOf(Props.Create(() => new StarzMegawaysGameLogic()),           "StarzMegaWays"));
            _dicGameLogicActors.Add(GAMEID.TheDogHouse,                 Context.ActorOf(Props.Create(() => new TheDogHouseGameLogic()),             "TheDogHouse"));
            _dicGameLogicActors.Add(GAMEID.WestWildGold,                Context.ActorOf(Props.Create(() => new WestWildGoldGameLogic()),            "WestWildGold"));
            _dicGameLogicActors.Add(GAMEID.PandaFortune2,               Context.ActorOf(Props.Create(() => new PandaFortune2GameLogic()),           "PandaFortune2"));
            _dicGameLogicActors.Add(GAMEID.ReturnDead,                  Context.ActorOf(Props.Create(() => new ReturnDeadGameLogic()),              "ReturnDead"));
            _dicGameLogicActors.Add(GAMEID.UltraBurn,                   Context.ActorOf(Props.Create(() => new UltraBurnGameLogic()),               "UltraBurn"));
            _dicGameLogicActors.Add(GAMEID.PirateGold,                  Context.ActorOf(Props.Create(() => new PirateGoldGameLogic()),              "PirateGold"));
            _dicGameLogicActors.Add(GAMEID.HotToBurnHoldAndSpin,        Context.ActorOf(Props.Create(() => new HotToBurnHoldAndSpinGameLogic())    ,"HotToBurnHoldAndSpin"));
            _dicGameLogicActors.Add(GAMEID.UltraHoldAndSpin,            Context.ActorOf(Props.Create(() => new UltraHoldAndSpinGameLogic()),        "UltraHoldAndSpin"));
            _dicGameLogicActors.Add(GAMEID.OlympusGates,                Context.ActorOf(Props.Create(() => new OlympusGatesGameLogic()),            "OlympusGates"));
            _dicGameLogicActors.Add(GAMEID.DiamondStrike,               Context.ActorOf(Props.Create(() => new DiamondStrikeGameLogic()),           "DiamondStrike"));
            _dicGameLogicActors.Add(GAMEID.WolfGold,                    Context.ActorOf(Props.Create(() => new WolfGoldGameLogic()),                "WolfGold"));
            _dicGameLogicActors.Add(GAMEID.GoldenOx,                    Context.ActorOf(Props.Create(() => new GoldenOxGameLogic()),                "GoldenOx"));
            _dicGameLogicActors.Add(GAMEID.MadameDestinyMegaWays,       Context.ActorOf(Props.Create(() => new MadameDestinyMegaGameLogic()),       "MadameDestinyMegaWays"));
            _dicGameLogicActors.Add(GAMEID.FruitParty,                  Context.ActorOf(Props.Create(() => new FruitPartyGameLogic()),              "FruitParty"));
            _dicGameLogicActors.Add(GAMEID.GreatRhinoMega,              Context.ActorOf(Props.Create(() => new GreatRhinoMegaGameLogic()),          "GreatRhinoMega"));
            _dicGameLogicActors.Add(GAMEID.GreatRhino,                  Context.ActorOf(Props.Create(() => new GreatRhinoGameLogic()),              "GreatRhino"));
            _dicGameLogicActors.Add(GAMEID.GreatRhinoDelux,             Context.ActorOf(Props.Create(() => new GreatRhinoDeluxGameLogic()),         "GreatRhinoDelux"));
            _dicGameLogicActors.Add(GAMEID.FiveLionsMega,               Context.ActorOf(Props.Create(() => new FiveLionsMegaGameLogic()),           "FiveLionsMega"));
            _dicGameLogicActors.Add(GAMEID.FiveLions,                   Context.ActorOf(Props.Create(() => new FiveLionsGameLogic()),               "FiveLions"));
            _dicGameLogicActors.Add(GAMEID.FiveLionsGold,               Context.ActorOf(Props.Create(() => new FiveLionsGoldGameLogic()),           "FiveLionsGold"));
            _dicGameLogicActors.Add(GAMEID.TheDogHouseMega,             Context.ActorOf(Props.Create(() => new TheDogHouseMegaGameLogic()),         "TheDogHouseMega"));
            _dicGameLogicActors.Add(GAMEID.HerculesAndPegasus,          Context.ActorOf(Props.Create(() => new HerculesAndPegasusGameLogic()),      "HerculesAndPegasus"));
            _dicGameLogicActors.Add(GAMEID.EgyptianFortunes,            Context.ActorOf(Props.Create(() => new EgyptianFortunesGameLogic()),        "EgyptianFortunes"));
            _dicGameLogicActors.Add(GAMEID.FiveLionsDance,              Context.ActorOf(Props.Create(() => new FiveLionsDanceGameLogic()),          "FiveLionsDance"));
            _dicGameLogicActors.Add(GAMEID.HotToBurn,                   Context.ActorOf(Props.Create(() => new HotToBurnGameLogic()),               "HotToBurn"));
            _dicGameLogicActors.Add(GAMEID.LuckyNewYear,                Context.ActorOf(Props.Create(() => new LuckyNewYearGameLogic()),            "LuckyNewYear"));
            _dicGameLogicActors.Add(GAMEID.WildBooster,                 Context.ActorOf(Props.Create(() => new WildBoosterGameLogic()),             "WildBooster"));
            _dicGameLogicActors.Add(GAMEID.MonkeyWarrior,               Context.ActorOf(Props.Create(() => new MonkeyWarriorGameLogic()),           "MonkeyWarrior"));            
            _dicGameLogicActors.Add(GAMEID.DanceParty,                  Context.ActorOf(Props.Create(() => new DancePartyGameLogic()),              "DanceParty"));            
            _dicGameLogicActors.Add(GAMEID.DragonHoldAndSpin,           Context.ActorOf(Props.Create(() => new DragonHoldAndSpinGameLogic()),       "DragonHoldAndSpin"));
            _dicGameLogicActors.Add(GAMEID.BuffaloKingMega,             Context.ActorOf(Props.Create(() => new BuffaloKingMegaGameLogic()),         "BuffaloKingMega"));
            _dicGameLogicActors.Add(GAMEID.VegasMagic,                  Context.ActorOf(Props.Create(() => new VegasMagicGameLogic()),              "VegasMagic"));
            _dicGameLogicActors.Add(GAMEID.AztecKingMega,               Context.ActorOf(Props.Create(() => new AztecKingMegaGameLogic()),           "AztecKingMega"));
            _dicGameLogicActors.Add(GAMEID.HeartOfRio,                  Context.ActorOf(Props.Create(() => new HeartOfRioGameLogic()),              "HeartOfRio"));
            _dicGameLogicActors.Add(GAMEID.JokerKing,                   Context.ActorOf(Props.Create(() => new JokerKingGameLogic()),               "JokerKing"));
            _dicGameLogicActors.Add(GAMEID.MustangGold,                 Context.ActorOf(Props.Create(() => new MustangGoldGameLogic()),             "MustangGold"));
            _dicGameLogicActors.Add(GAMEID.BroncoSpirit,                Context.ActorOf(Props.Create(() => new BroncoSpiritGameLogic()),            "BroncoSpirit"));
            _dicGameLogicActors.Add(GAMEID.Asgard,                      Context.ActorOf(Props.Create(() => new AsgardGameLogic()),                  "Asgard"));            
            _dicGameLogicActors.Add(GAMEID.BiggerBassBonanza,           Context.ActorOf(Props.Create(() => new BiggerBassBonanzaGameLogic()),       "BiggerBassBonanza"));
            _dicGameLogicActors.Add(GAMEID.AztecGems,                   Context.ActorOf(Props.Create(() => new AztecGemsGameLogic()),               "AztecGems"));
            _dicGameLogicActors.Add(GAMEID.BigBassBonanza,              Context.ActorOf(Props.Create(() => new BigBassBonanzaGameLogic()),          "BigBassBonanza"));
            _dicGameLogicActors.Add(GAMEID.CashElevator,                Context.ActorOf(Props.Create(() => new CashElevatorGameLogic()),            "CashElevator"));
            _dicGameLogicActors.Add(GAMEID.CowBoysGold,                 Context.ActorOf(Props.Create(() => new CowBoysGoldGameLogic()),             "CowBoysGold"));
            _dicGameLogicActors.Add(GAMEID.EightDragons,                Context.ActorOf(Props.Create(() => new EightDragonsGameLogic()),            "EightDragons"));
            _dicGameLogicActors.Add(GAMEID.HandOfMidas,                 Context.ActorOf(Props.Create(() => new HandOfMidasGameLogic()),             "HandOfMidas"));
            _dicGameLogicActors.Add(GAMEID.MoneyMouse,                  Context.ActorOf(Props.Create(() => new MoneyMouseGameLogic()),              "MoneyMouse"));
            _dicGameLogicActors.Add(GAMEID.PeKingLuck,                  Context.ActorOf(Props.Create(() => new PeKingLuckGameLogic()),              "PeKingLuck"));
            _dicGameLogicActors.Add(GAMEID.PirateGoldDelux,             Context.ActorOf(Props.Create(() => new PirateGoldDeluxGameLogic()),         "PirateGoldDelux"));
            _dicGameLogicActors.Add(GAMEID.PowerOfThorMega,             Context.ActorOf(Props.Create(() => new PowerOfThorMegaGameLogic()),         "PowerOfThorMega"));
            _dicGameLogicActors.Add(GAMEID.PyramidBonanza,              Context.ActorOf(Props.Create(() => new PyramidBonanzaGameLogic()),          "PyramidBonanza"));
            _dicGameLogicActors.Add(GAMEID.TheTweetyHouse,              Context.ActorOf(Props.Create(() => new TheTweetyHouseGameLogic()),          "TheTweetyHouse"));
            _dicGameLogicActors.Add(GAMEID.FloatingDragonHoldAndSpin,   Context.ActorOf(Props.Create(() => new FloatingDragonHoldAndSpinGameLogic()),   "FloatingDragonHoldAndSpin"));
            _dicGameLogicActors.Add(GAMEID.HotFiesta,                   Context.ActorOf(Props.Create(() => new HotFiestaGameLogic()),               "HotFiesta"));
            _dicGameLogicActors.Add(GAMEID.ChristmasBigBassBonanza,     Context.ActorOf(Props.Create(() => new ChristmasBigBassBonanzaGameLogic()), "ChristmasBigBassBonanza"));
            _dicGameLogicActors.Add(GAMEID.FortuneOfGiza,               Context.ActorOf(Props.Create(() => new FortuneOfGizaGameLogic()),           "FortuneOfGiza"));
            _dicGameLogicActors.Add(GAMEID.CashPatrol,                  Context.ActorOf(Props.Create(() => new CashPatrolGameLogic()),              "CashPatrol"));
            _dicGameLogicActors.Add(GAMEID.RockVegas,                   Context.ActorOf(Props.Create(() => new RockVegasGameLogic()),               "RockVegas"));
            _dicGameLogicActors.Add(GAMEID.EmptyTheBank,                Context.ActorOf(Props.Create(() => new EmptyTheBankGameLogic()),            "EmptyTheBank"));
            _dicGameLogicActors.Add(GAMEID.Queenie,                     Context.ActorOf(Props.Create(() => new QueenieGameLogic()),                 "Queenie"));
            _dicGameLogicActors.Add(GAMEID.Cleocatra,                   Context.ActorOf(Props.Create(() => new CleocatraGameLogic()),               "Cleocatra"));
            _dicGameLogicActors.Add(GAMEID.ZombieCarnival,              Context.ActorOf(Props.Create(() => new ZombieCarnivalGameLogic()),          "ZombieCarnival"));
            _dicGameLogicActors.Add(GAMEID.TheUltimate5,                Context.ActorOf(Props.Create(() => new TheUltimate5GameLogic()),            "TheUltimate5"));
            _dicGameLogicActors.Add(GAMEID.BigJuan,                     Context.ActorOf(Props.Create(() => new BigJuanGameLogic()),                 "BigJuan"));
            _dicGameLogicActors.Add(GAMEID.KoiPond,                     Context.ActorOf(Props.Create(() => new KoiPondGameLogic()),                 "KoiPond"));
            _dicGameLogicActors.Add(GAMEID.TreasureWild,                Context.ActorOf(Props.Create(() => new TreasureWildGameLogic()),            "TreasureWild"));
            _dicGameLogicActors.Add(GAMEID.SugarRush,                   Context.ActorOf(Props.Create(() => new SugarRushGameLogic()),               "SugarRush"));
            _dicGameLogicActors.Add(GAMEID.StarlightPrincess,           Context.ActorOf(Props.Create(() => new StarlightPrincessGameLogic()),       "StarlightPrincess"));
            _dicGameLogicActors.Add(GAMEID.WildWestGoldMega,            Context.ActorOf(Props.Create(() => new WildWestGoldMegaGameLogic()),        "WildWestGoldMega"));
            _dicGameLogicActors.Add(GAMEID.BarnFestival,                Context.ActorOf(Props.Create(() => new BarnFestivalGameLogic()),            "BarnFestival"));
            _dicGameLogicActors.Add(GAMEID.DragonHero,                  Context.ActorOf(Props.Create(() => new DragonHeroGameLogic()),              "DragonHero"));
            _dicGameLogicActors.Add(GAMEID.StarlightChristmas,          Context.ActorOf(Props.Create(() => new StarlightChristmasGameLogic()),      "StarlightChristmas"));
            _dicGameLogicActors.Add(GAMEID.RiseOfSamurai3,              Context.ActorOf(Props.Create(() => new RiseOfSamurai3GameLogic()),          "RiseOfSamurai3"));
            _dicGameLogicActors.Add(GAMEID.YumYumPowerWays,             Context.ActorOf(Props.Create(() => new YumYumPowerWaysGameLogic()),         "YumYumPowerWays"));
            _dicGameLogicActors.Add(GAMEID.FishEye,                     Context.ActorOf(Props.Create(() => new FishEyeGameLogic()),                 "FishEye"));
            _dicGameLogicActors.Add(GAMEID.MonsterSuperlanche,          Context.ActorOf(Props.Create(() => new MonsterSuperlancheGameLogic()),      "MonsterSuperlanche"));
            _dicGameLogicActors.Add(GAMEID.CandyVillage,                Context.ActorOf(Props.Create(() => new CandyVillageGameLogic()),            "CandyVillage"));
            _dicGameLogicActors.Add(GAMEID.FloatingDragonMega,          Context.ActorOf(Props.Create(() => new FloatingDragonMegaGameLogic()),      "FloatingDragonMega"));
            _dicGameLogicActors.Add(GAMEID.LuckyLightning,              Context.ActorOf(Props.Create(() => new LuckyLightningGameLogic()),          "LuckyLightning"));
            _dicGameLogicActors.Add(GAMEID.Mochimon,                    Context.ActorOf(Props.Create(() => new MochimonGameLogic()),                "Mochimon"));
            _dicGameLogicActors.Add(GAMEID.ClubTropicana,               Context.ActorOf(Props.Create(() => new ClubTropicanaGameLogic()),           "ClubTropicana"));
            _dicGameLogicActors.Add(GAMEID.FireArcher,                  Context.ActorOf(Props.Create(() => new FireArcherGameLogic()),              "FireArcher"));
            _dicGameLogicActors.Add(GAMEID.RabbitGarden,                Context.ActorOf(Props.Create(() => new RabbitGardenGameLogic()),            "RabbitGarden"));
            _dicGameLogicActors.Add(GAMEID.MysteryOfTheOrient,          Context.ActorOf(Props.Create(() => new MysteryOfTheOrientGameLogic()),      "MysteryOfTheOrient"));
            _dicGameLogicActors.Add(GAMEID.TheDogHouseMultihold,        Context.ActorOf(Props.Create(() => new TheDogHouseMultiholdGameLogic()),    "TheDogHouseMultihold"));
            _dicGameLogicActors.Add(GAMEID.TheKnightKing,               Context.ActorOf(Props.Create(() => new TheKnightKingGameLogic()),           "TheKnightKing"));
            #endregion

            #region BNG Games
            _dicGameLogicActors.Add(GAMEID.ScarabRiches,                Context.ActorOf(Props.Create(() => new ScarabRichesGameLogic()),            "ScarabRiches"));
            _dicGameLogicActors.Add(GAMEID.GoldExpress,                 Context.ActorOf(Props.Create(() => new GoldExpressGameLogic()),             "GoldExpress"));
            _dicGameLogicActors.Add(GAMEID.TigerJungle,                 Context.ActorOf(Props.Create(() => new TigerJungleGameLogic()),             "TigerJungle"));
            _dicGameLogicActors.Add(GAMEID.WolfNight,                   Context.ActorOf(Props.Create(() => new WolfNightGameLogic()),               "WolfNight"));
            _dicGameLogicActors.Add(GAMEID.HitTheGold,                  Context.ActorOf(Props.Create(() => new HitTheGoldGameLogic()),              "HitTheGold"));
            _dicGameLogicActors.Add(GAMEID.SunOfEgypt,                  Context.ActorOf(Props.Create(() => new SunOfEgyptGameLogic()),              "SunOfEgypt"));
            _dicGameLogicActors.Add(GAMEID.BlackWolf,                   Context.ActorOf(Props.Create(() => new BlackWolfGameLogic()),               "BlackWolf"));
            _dicGameLogicActors.Add(GAMEID.MoonSisters,                 Context.ActorOf(Props.Create(() => new MoonSistersGameLogic()),             "MoonSisters"));

            #endregion

            #region CQ9 Games
            _dicGameLogicActors.Add(GAMEID.GodOfWar,                    Context.ActorOf(Props.Create(() => new GodOfWarGameLogic()),                "GodOfWar"));
            _dicGameLogicActors.Add(GAMEID.JumpHigh,                    Context.ActorOf(Props.Create(() => new JumpHighGameLogic()),                "JumpHigh"));
            _dicGameLogicActors.Add(GAMEID.RaveJump,                    Context.ActorOf(Props.Create(() => new RaveJumpGameLogic()),                "RaveJump"));
            _dicGameLogicActors.Add(GAMEID.GuGuGu,                      Context.ActorOf(Props.Create(() => new GuGuGuGameLogic()),                  "GuGuGu"));
            _dicGameLogicActors.Add(GAMEID.ShouXin,                     Context.ActorOf(Props.Create(() => new ShouXinGameLogic()),                 "ShouXin"));
            _dicGameLogicActors.Add(GAMEID.JumpHigh2,                   Context.ActorOf(Props.Create(() => new JumpHigh2GameLogic()),               "JumpHigh2"));
            _dicGameLogicActors.Add(GAMEID.GuGuGu2,                     Context.ActorOf(Props.Create(() => new GuGuGu2GameLogic()),                 "GuGuGu2"));
            _dicGameLogicActors.Add(GAMEID.CricketFever,                Context.ActorOf(Props.Create(() => new CricketFeverGameLogic()),            "CricketFever"));
            _dicGameLogicActors.Add(GAMEID.Thor,                        Context.ActorOf(Props.Create(() => new ThorGameLogic()),                    "Thor"));
            _dicGameLogicActors.Add(GAMEID.Thor2,                       Context.ActorOf(Props.Create(() => new Thor2GameLogic()),                   "Thor2"));
            _dicGameLogicActors.Add(GAMEID.InvincibleElephant,          Context.ActorOf(Props.Create(() => new InvincibleElephantGameLogic()),      "InvincibleElephant"));
            _dicGameLogicActors.Add(GAMEID.FlowerFortunes,              Context.ActorOf(Props.Create(() => new FlowerFortunesGameLogic()),          "FlowerFortunes"));
            _dicGameLogicActors.Add(GAMEID.TheBeastWar,                 Context.ActorOf(Props.Create(() => new TheBeastWarGameLogic()),             "TheBeastWar"));
            _dicGameLogicActors.Add(GAMEID.FireChibi,                   Context.ActorOf(Props.Create(() => new FireChibiGameLogic()),               "FireChibi"));
            _dicGameLogicActors.Add(GAMEID.FireChibi2,                  Context.ActorOf(Props.Create(() => new FireChibi2GameLogic()),              "FireChibi2"));
            _dicGameLogicActors.Add(GAMEID.Meow,                        Context.ActorOf(Props.Create(() => new MeowGameLogic()),                    "Meow"));
            _dicGameLogicActors.Add(GAMEID.ZeusM,                       Context.ActorOf(Props.Create(() => new ZeusMGameLogic()),                   "ZeusM"));
            _dicGameLogicActors.Add(GAMEID.SnowQueen,                   Context.ActorOf(Props.Create(() => new SnowQueenGameLogic()),               "SnowQueen"));
            _dicGameLogicActors.Add(GAMEID.Seven77,                     Context.ActorOf(Props.Create(() => new Seven77GameLogic()),                 "Seven77"));
            _dicGameLogicActors.Add(GAMEID.FunnyAlpaca,                 Context.ActorOf(Props.Create(() => new FunnyAlpacaGameLogic()),             "FunnyAlpaca"));
            _dicGameLogicActors.Add(GAMEID.MoveNJump,                   Context.ActorOf(Props.Create(() => new MoveNJumpGameLogic()),               "MoveNJump"));
            _dicGameLogicActors.Add(GAMEID.MirrorMirror,                Context.ActorOf(Props.Create(() => new MirrorMirrorGameLogic()),            "MirrorMirror"));
            _dicGameLogicActors.Add(GAMEID.FaCaiShen,                   Context.ActorOf(Props.Create(() => new FaCaiShenGameLogic()),               "FaCaiShen"));
            _dicGameLogicActors.Add(GAMEID.LuckyBats,                   Context.ActorOf(Props.Create(() => new LuckyBatsGameLogic()),               "LuckyBats"));
            _dicGameLogicActors.Add(GAMEID.JumpingMobile,               Context.ActorOf(Props.Create(() => new JumpingMobileGameLogic()),           "JumpingMobile"));
            _dicGameLogicActors.Add(GAMEID.SoSweet,                     Context.ActorOf(Props.Create(() => new SoSweetGameLogic()),                 "SoSweet"));
            _dicGameLogicActors.Add(GAMEID.Wonderland,                  Context.ActorOf(Props.Create(() => new WonderlandGameLogic()),              "Wonderland"));
            _dicGameLogicActors.Add(GAMEID.Super5,                      Context.ActorOf(Props.Create(() => new Super5GameLogic()),                  "Super5"));
            _dicGameLogicActors.Add(GAMEID.Apsaras,                     Context.ActorOf(Props.Create(() => new ApsarasGameLogic()),                 "Apsaras"));
            _dicGameLogicActors.Add(GAMEID.RaveHigh,                    Context.ActorOf(Props.Create(() => new RaveHighGameLogic()),                "RaveHigh"));
            _dicGameLogicActors.Add(GAMEID.FireQueen,                   Context.ActorOf(Props.Create(() => new FireQueenGameLogic()),               "FireQueen"));
            _dicGameLogicActors.Add(GAMEID.FireQueen2,                  Context.ActorOf(Props.Create(() => new FireQueen2GameLogic()),              "FireQueen2"));
            _dicGameLogicActors.Add(GAMEID.FaCaiShenM,                  Context.ActorOf(Props.Create(() => new FaCaiShenMGameLogic()),              "FaCaiShenM"));
            _dicGameLogicActors.Add(GAMEID.DiscoNight,                  Context.ActorOf(Props.Create(() => new DiscoNightGameLogic()),              "DiscoNight"));
            _dicGameLogicActors.Add(GAMEID.SixCandy,                    Context.ActorOf(Props.Create(() => new SixCandyGameLogic()),                "SixCandy"));
            _dicGameLogicActors.Add(GAMEID.ZhongKui,                    Context.ActorOf(Props.Create(() => new ZhongKuiGameLogic()),                "ZhongKui"));
            _dicGameLogicActors.Add(GAMEID.RaveJump2,                   Context.ActorOf(Props.Create(() => new RaveJump2GameLogic()),               "RaveJump2"));
            _dicGameLogicActors.Add(GAMEID.LuckyBatsM,                  Context.ActorOf(Props.Create(() => new LuckyBatsMGameLogic()),              "LuckyBatsM"));
            _dicGameLogicActors.Add(GAMEID.WingChun,                    Context.ActorOf(Props.Create(() => new WingChunGameLogic()),                "WingChun"));
            _dicGameLogicActors.Add(GAMEID.Zeus,                        Context.ActorOf(Props.Create(() => new ZeusGameLogic()),                    "Zeus"));
            _dicGameLogicActors.Add(GAMEID.GodOfWarM,                   Context.ActorOf(Props.Create(() => new GodOfWarMGameLogic()),               "GodOfWarM"));
            _dicGameLogicActors.Add(GAMEID.RaveJumpM,                   Context.ActorOf(Props.Create(() => new RaveJumpMGameLogic()),               "RaveJumpM"));
            _dicGameLogicActors.Add(GAMEID.FireChibiM,                  Context.ActorOf(Props.Create(() => new FireChibiMGameLogic()),              "FireChibiM"));
            _dicGameLogicActors.Add(GAMEID.MuayThai,                    Context.ActorOf(Props.Create(() => new MuayThaiGameLogic()),                "MuayThai"));
            _dicGameLogicActors.Add(GAMEID.DiscoNightM,                 Context.ActorOf(Props.Create(() => new DiscoNightMGameLogic()),             "DiscoNightM"));
            _dicGameLogicActors.Add(GAMEID.RaveJump2M,                  Context.ActorOf(Props.Create(() => new RaveJump2MGameLogic()),              "RaveJump2M"));
            _dicGameLogicActors.Add(GAMEID.GuGuGu2M,                    Context.ActorOf(Props.Create(() => new GuGuGu2MGameLogic()),                "GuGuGu2M"));
            _dicGameLogicActors.Add(GAMEID.GuGuGuM,                     Context.ActorOf(Props.Create(() => new GuGuGuMGameLogic()),                 "GuGuGuM"));
            _dicGameLogicActors.Add(GAMEID.LuckyBoxes,                  Context.ActorOf(Props.Create(() => new LuckyBoxesGameLogic()),              "LuckyBoxes"));
            _dicGameLogicActors.Add(GAMEID.OrientalBeauty,              Context.ActorOf(Props.Create(() => new OrientalBeautyGameLogic()),          "OrientalBeauty"));
            _dicGameLogicActors.Add(GAMEID.FruitKing2,                  Context.ActorOf(Props.Create(() => new FruitKing2GameLogic()),              "FruitKing2"));
            _dicGameLogicActors.Add(GAMEID.CrazyBundesliga,             Context.ActorOf(Props.Create(() => new CrazyBundesligaGameLogic()),         "CrazyBundesliga"));
            _dicGameLogicActors.Add(GAMEID.PremiorLeague,               Context.ActorOf(Props.Create(() => new PremiorLeagueGameLogic()),           "PremiorLeague"));
            _dicGameLogicActors.Add(GAMEID.FootballEuro,                Context.ActorOf(Props.Create(() => new FootballEuroGameLogic()),            "FootballEuro"));
            _dicGameLogicActors.Add(GAMEID.LaLiga,                      Context.ActorOf(Props.Create(() => new LaLigaGameLogic()),                  "LaLiga"));
            _dicGameLogicActors.Add(GAMEID.WukongAndPeaches,            Context.ActorOf(Props.Create(() => new WukongAndPeachesGameLogic()),        "WukongAndPeaches"));
            _dicGameLogicActors.Add(GAMEID.TreasureBowl,                Context.ActorOf(Props.Create(() => new TreasureBowlGameLogic()),            "TreasureBowl"));
            _dicGameLogicActors.Add(GAMEID.WaterWorld,                  Context.ActorOf(Props.Create(() => new WaterWorldGameLogic()),              "WaterWorld"));
            _dicGameLogicActors.Add(GAMEID.WaterBalloons,               Context.ActorOf(Props.Create(() => new WaterBalloonsGameLogic()),           "WaterBalloons"));
            _dicGameLogicActors.Add(GAMEID.HappyRichYear,               Context.ActorOf(Props.Create(() => new HappyRichYearGameLogic()),           "HappyRichYear"));
            _dicGameLogicActors.Add(GAMEID.OGLegend,                    Context.ActorOf(Props.Create(() => new OGLegendGameLogic()),                "OGLegend"));
            _dicGameLogicActors.Add(GAMEID.VampireKiss,                 Context.ActorOf(Props.Create(() => new VampireKissGameLogic()),             "VampireKiss"));
            _dicGameLogicActors.Add(GAMEID.WildTarzan,                  Context.ActorOf(Props.Create(() => new WildTarzanGameLogic()),              "WildTarzan"));
            _dicGameLogicActors.Add(GAMEID.Chameleon,                   Context.ActorOf(Props.Create(() => new ChameleonGameLogic()),               "Chameleon"));
            _dicGameLogicActors.Add(GAMEID.FiveBoxing,                  Context.ActorOf(Props.Create(() => new FiveBoxingGameLogic()),              "FiveBoxing"));
            _dicGameLogicActors.Add(GAMEID.GodOfChess,                  Context.ActorOf(Props.Create(() => new GodOfChessGameLogic()),              "GodOfChess"));
            _dicGameLogicActors.Add(GAMEID.FootballBaby,                Context.ActorOf(Props.Create(() => new FootballBabyGameLogic()),            "FootballBaby"));
            _dicGameLogicActors.Add(GAMEID.FruitKing,                   Context.ActorOf(Props.Create(() => new FruitKingGameLogic()),               "FruitKing"));
            _dicGameLogicActors.Add(GAMEID.WorldCupRussia2018,          Context.ActorOf(Props.Create(() => new WorldCupRussia2018GameLogic()),      "WorldCupRussia2018"));
            _dicGameLogicActors.Add(GAMEID.WorldCupField,               Context.ActorOf(Props.Create(() => new WorldCupFieldGameLogic()),           "WorldCupField"));
            _dicGameLogicActors.Add(GAMEID.WolfDisco,                   Context.ActorOf(Props.Create(() => new WolfDiscoGameLogic()),               "WolfDisco"));
            _dicGameLogicActors.Add(GAMEID.GreekGods,                   Context.ActorOf(Props.Create(() => new GreekGodsGameLogic()),               "GreekGods"));
            _dicGameLogicActors.Add(GAMEID.TreasureIsland,              Context.ActorOf(Props.Create(() => new TreasureIslandGameLogic()),          "TreasureIsland"));
            _dicGameLogicActors.Add(GAMEID.GoldenEggs,                  Context.ActorOf(Props.Create(() => new GoldenEggsGameLogic()),              "GoldenEggs"));
            _dicGameLogicActors.Add(GAMEID.SakuraLegend,                Context.ActorOf(Props.Create(() => new SakuraLegendGameLogic()),            "SakuraLegend"));
            _dicGameLogicActors.Add(GAMEID.FortuneDragon,               Context.ActorOf(Props.Create(() => new FortuneDragonGameLogic()),           "FortuneDragon"));
            _dicGameLogicActors.Add(GAMEID.BigWolf,                     Context.ActorOf(Props.Create(() => new BigWolfGameLogic()),                 "BigWolf"));
            _dicGameLogicActors.Add(GAMEID.MonkeyOfficeLegend,          Context.ActorOf(Props.Create(() => new MonkeyOfficeLegendGameLogic()),      "MonkeyOfficeLegend"));
            _dicGameLogicActors.Add(GAMEID.ZumaWild,                    Context.ActorOf(Props.Create(() => new ZumaWildGameLogic()),                "ZumaWild"));
            _dicGameLogicActors.Add(GAMEID.SkrSkr,                      Context.ActorOf(Props.Create(() => new SkrSkrGameLogic()),                  "SkrSkr"));
            _dicGameLogicActors.Add(GAMEID.MyeongRyang,                 Context.ActorOf(Props.Create(() => new MyeongRyangGameLogic()),             "MyeongRyang"));
            _dicGameLogicActors.Add(GAMEID.GaneshaJr,                   Context.ActorOf(Props.Create(() => new GaneshaJrGameLogic()),               "GaneshaJr"));
            _dicGameLogicActors.Add(GAMEID.HotDJ,                       Context.ActorOf(Props.Create(() => new HotDJGameLogic()),                   "HotDJ"));
            _dicGameLogicActors.Add(GAMEID.SuperDiamonds,               Context.ActorOf(Props.Create(() => new SuperDiamondsGameLogic()),           "SuperDiamonds"));
            _dicGameLogicActors.Add(GAMEID.Poseidon,                    Context.ActorOf(Props.Create(() => new PoseidonGameLogic()),                "Poseidon"));
            _dicGameLogicActors.Add(GAMEID.FortuneTotem,                Context.ActorOf(Props.Create(() => new FortuneTotemGameLogic()),            "FortuneTotem"));
            _dicGameLogicActors.Add(GAMEID.NeZhaAdvent,                 Context.ActorOf(Props.Create(() => new NeZhaAdventGameLogic()),             "NeZhaAdvent"));
            _dicGameLogicActors.Add(GAMEID.Hephaestus,                  Context.ActorOf(Props.Create(() => new HephaestusGameLogic()),              "Hephaestus"));
            _dicGameLogicActors.Add(GAMEID.MrMiser,                     Context.ActorOf(Props.Create(() => new MrMiserGameLogic()),                 "MrMiser"));
            _dicGameLogicActors.Add(GAMEID.LuckyTigers,                 Context.ActorOf(Props.Create(() => new LuckyTigersGameLogic()),             "LuckyTigers"));
            _dicGameLogicActors.Add(GAMEID.Hercules,                    Context.ActorOf(Props.Create(() => new HerculesGameLogic()),                "Hercules"));
            _dicGameLogicActors.Add(GAMEID.DiamondTreasure,             Context.ActorOf(Props.Create(() => new DiamondTreasureGameLogic()),         "DiamondTreasure"));
            _dicGameLogicActors.Add(GAMEID.Apollo,                      Context.ActorOf(Props.Create(() => new ApolloGameLogic()),                  "Apollo"));
            _dicGameLogicActors.Add(GAMEID.LordGanesha,                 Context.ActorOf(Props.Create(() => new LordGaneshaGameLogic()),             "LordGanesha"));
            _dicGameLogicActors.Add(GAMEID.GreatLion,                   Context.ActorOf(Props.Create(() => new GreatLionGameLogic()),               "GreatLion"));
            _dicGameLogicActors.Add(GAMEID.RedPhoenix,                  Context.ActorOf(Props.Create(() => new RedPhoenixGameLogic()),              "RedPhoenix"));
            _dicGameLogicActors.Add(GAMEID.MagicWorld,                  Context.ActorOf(Props.Create(() => new MagicWorldGameLogic()),              "MagicWorld"));
            _dicGameLogicActors.Add(GAMEID.TreasureHouse,               Context.ActorOf(Props.Create(() => new TreasureHouseGameLogic()),           "TreasureHouse"));
            _dicGameLogicActors.Add(GAMEID.DragonHeart,                 Context.ActorOf(Props.Create(() => new DragonHeartGameLogic()),             "DragonHeart"));
            _dicGameLogicActors.Add(GAMEID.StrikerWild,                 Context.ActorOf(Props.Create(() => new StrikerWildGameLogic()),             "StrikerWild"));
            _dicGameLogicActors.Add(GAMEID.WanbaoDino,                  Context.ActorOf(Props.Create(() => new WanbaoDinoGameLogic()),              "WanbaoDino"));
            _dicGameLogicActors.Add(GAMEID.DetectiveDee,                Context.ActorOf(Props.Create(() => new DetectiveDeeGameLogic()),            "DetectiveDee"));
            _dicGameLogicActors.Add(GAMEID.RunningAnimals,              Context.ActorOf(Props.Create(() => new RunningAnimalsGameLogic()),          "RunningAnimals"));
            _dicGameLogicActors.Add(GAMEID.SummerMood,                  Context.ActorOf(Props.Create(() => new SummerMoodGameLogic()),              "SummerMood"));
            _dicGameLogicActors.Add(GAMEID.AllWild,                     Context.ActorOf(Props.Create(() => new AllWildGameLogic()),                 "AllWild"));
            _dicGameLogicActors.Add(GAMEID.GophersWar,                  Context.ActorOf(Props.Create(() => new GophersWarGameLogic()),              "GophersWar"));
            _dicGameLogicActors.Add(GAMEID.YuanBao,                     Context.ActorOf(Props.Create(() => new YuanBaoGameLogic()),                 "YuanBao"));
            _dicGameLogicActors.Add(GAMEID.Acrobatics,                  Context.ActorOf(Props.Create(() => new AcrobaticsGameLogic()),              "Acrobatics"));
            _dicGameLogicActors.Add(GAMEID.Fire777,                     Context.ActorOf(Props.Create(() => new Fire777GameLogic()),                 "Fire777"));
            _dicGameLogicActors.Add(GAMEID.DragonTreasure,              Context.ActorOf(Props.Create(() => new DragonTreasureGameLogic()),          "DragonTreasure"));
            _dicGameLogicActors.Add(GAMEID.GoldStealer,                 Context.ActorOf(Props.Create(() => new GoldStealerGameLogic()),             "GoldStealer"));
            _dicGameLogicActors.Add(GAMEID.SixGacha,                    Context.ActorOf(Props.Create(() => new SixGachaGameLogic()),                "SixGacha"));
            _dicGameLogicActors.Add(GAMEID.NightCity,                   Context.ActorOf(Props.Create(() => new NightCityGameLogic()),               "NightCity"));
            _dicGameLogicActors.Add(GAMEID.DetectiveDee2,               Context.ActorOf(Props.Create(() => new DetectiveDee2GameLogic()),           "DetectiveDee2"));
            _dicGameLogicActors.Add(GAMEID.MahJongKing,                 Context.ActorOf(Props.Create(() => new MahJongKingGameLogic()),             "MahJongKing"));
            _dicGameLogicActors.Add(GAMEID.SongkranFestival,            Context.ActorOf(Props.Create(() => new SongkranFestivalGameLogic()),        "SongkranFestival"));
            _dicGameLogicActors.Add(GAMEID.UproarInHeaven,              Context.ActorOf(Props.Create(() => new UproarInHeavenGameLogic()),          "UproarInHeaven"));
            _dicGameLogicActors.Add(GAMEID.DaFaCai,                     Context.ActorOf(Props.Create(() => new DaFaCaiGameLogic()),                 "DaFaCai"));
            _dicGameLogicActors.Add(GAMEID.DaHongZhong,                 Context.ActorOf(Props.Create(() => new DaHongZhongGameLogic()),             "DaHongZhong"));
            _dicGameLogicActors.Add(GAMEID.GoodFortune,                 Context.ActorOf(Props.Create(() => new GoodFortuneGameLogic()),             "GoodFortune"));
            _dicGameLogicActors.Add(GAMEID.GoodFortuneM,                Context.ActorOf(Props.Create(() => new GoodFortuneMGameLogic()),            "GoodFortuneM"));
            _dicGameLogicActors.Add(GAMEID.FootballAllStar,             Context.ActorOf(Props.Create(() => new FootballAllStarGameLogic()),         "FootballAllStar"));
            _dicGameLogicActors.Add(GAMEID.FootballBoots,               Context.ActorOf(Props.Create(() => new FootballBootsGameLogic()),           "FootballBoots"));
            _dicGameLogicActors.Add(GAMEID.WonWonWon,                   Context.ActorOf(Props.Create(() => new WonWonWonGameLogic()),               "WonWonWon"));
            _dicGameLogicActors.Add(GAMEID.LionKing,                    Context.ActorOf(Props.Create(() => new LionKingGameLogic()),                "LionKing"));
            _dicGameLogicActors.Add(GAMEID.ThreePandas,                 Context.ActorOf(Props.Create(() => new ThreePandasGameLogic()),             "ThreePandas"));
            _dicGameLogicActors.Add(GAMEID.FootballJerseys,             Context.ActorOf(Props.Create(() => new FootballJerseysGameLogic()),         "FootballJerseys"));
            _dicGameLogicActors.Add(GAMEID.OGFaFaFa,                    Context.ActorOf(Props.Create(() => new OGFaFaFaGameLogic()),                "OGFaFaFa"));
            _dicGameLogicActors.Add(GAMEID.Eight88CaiShen,              Context.ActorOf(Props.Create(() => new Eight88CaiShenGameLogic()),          "Eight88CaiShen"));
            _dicGameLogicActors.Add(GAMEID.EcstaticCircus,              Context.ActorOf(Props.Create(() => new EcstaticCircusGameLogic()),          "EcstaticCircus"));
            _dicGameLogicActors.Add(GAMEID.PharaohsGold,                Context.ActorOf(Props.Create(() => new PharaohsGoldGameLogic()),            "PharaohsGold"));
            _dicGameLogicActors.Add(GAMEID.SixToros,                    Context.ActorOf(Props.Create(() => new SixTorosGameLogic()),                "SixToros"));
            _dicGameLogicActors.Add(GAMEID.SweetPop,                    Context.ActorOf(Props.Create(() => new SweetPopGameLogic()),                "SweetPop"));
            #endregion

            #region Habanero Games
            _dicGameLogicActors.Add(GAMEID.MysticFortuneDeluxe,         Context.ActorOf(Props.Create(() => new MysticFortuneDeluxeGameLogic()),     "MysticFortuneDeluxe"));
            _dicGameLogicActors.Add(GAMEID.FengHuang,                   Context.ActorOf(Props.Create(() => new FengHuangGameLogic()),               "FengHuang"));
            _dicGameLogicActors.Add(GAMEID.TheKoiGate,                  Context.ActorOf(Props.Create(() => new TheKoiGateGameLogic()),              "TheKoiGate"));
            _dicGameLogicActors.Add(GAMEID.DiscoBeats,                  Context.ActorOf(Props.Create(() => new DiscoBeatsGameLogic()),              "DiscoBeats"));
            _dicGameLogicActors.Add(GAMEID.Jump,                        Context.ActorOf(Props.Create(() => new JumpGameLogic()),                    "Jump"));
            _dicGameLogicActors.Add(GAMEID.FireRooster,                 Context.ActorOf(Props.Create(() => new FireRoosterGameLogic()),             "FireRooster"));
            _dicGameLogicActors.Add(GAMEID.FiveLuckyLions,              Context.ActorOf(Props.Create(() => new FiveLuckyLionsGameLogic()),          "FiveLuckyLions"));
            _dicGameLogicActors.Add(GAMEID.TabernaDeLosMuertosUltra,    Context.ActorOf(Props.Create(() => new TabernaDeLosMuertosUltraGameLogic()),"TabernaDeLosMuertosUltra"));
            
            _dicGameLogicActors.Add(GAMEID.PandaPanda,                  Context.ActorOf(Props.Create(() => new PandaPandaGameLogic()),              "PandaPanda"));
            _dicGameLogicActors.Add(GAMEID.NineTails,                   Context.ActorOf(Props.Create(() => new NineTailsGameLogic()),               "NineTails"));
            _dicGameLogicActors.Add(GAMEID.FourDivineBeasts,            Context.ActorOf(Props.Create(() => new FourDivineBeastsGameLogic()),        "FourDivineBeasts"));
            _dicGameLogicActors.Add(GAMEID.MightyMedusa,                Context.ActorOf(Props.Create(() => new MightyMedusaGameLogic()),            "MightyMedusa"));
            _dicGameLogicActors.Add(GAMEID.SpaceGoonz,                  Context.ActorOf(Props.Create(() => new SpaceGoonzGameLogic()),              "SpaceGoonz"));
            _dicGameLogicActors.Add(GAMEID.LuckyDurian,                 Context.ActorOf(Props.Create(() => new LuckyDurianGameLogic()),             "LuckyDurian"));
            _dicGameLogicActors.Add(GAMEID.LanternLuck,                 Context.ActorOf(Props.Create(() => new LanternLuckGameLogic()),             "LanternLuck"));
            _dicGameLogicActors.Add(GAMEID.Prost,                       Context.ActorOf(Props.Create(() => new ProstGameLogic()),                   "ProstGame"));
            _dicGameLogicActors.Add(GAMEID.NewYearsBash,                Context.ActorOf(Props.Create(() => new NewYearsBashGameLogic()),            "NewYearsBash"));
            _dicGameLogicActors.Add(GAMEID.BeforeTimeRunsOut,           Context.ActorOf(Props.Create(() => new BeforeTimeRunsOutGameLogic()),       "BeforeTimeRunsOut"));
            _dicGameLogicActors.Add(GAMEID.TotemTowers,                 Context.ActorOf(Props.Create(() => new TotemTowersGameLogic()),             "TotemTowers"));
            _dicGameLogicActors.Add(GAMEID.TabernaDeLosMuertos,         Context.ActorOf(Props.Create(() => new TabernaDeLosMuertosGameLogic()),     "TabernaDeLosMuertos"));
            _dicGameLogicActors.Add(GAMEID.WealthInn,                   Context.ActorOf(Props.Create(() => new WealthInnGameLogic()),               "WealthInn"));
            _dicGameLogicActors.Add(GAMEID.HeySushi,                    Context.ActorOf(Props.Create(() => new HeySushiGameLogic()),                "HeySushi"));
            _dicGameLogicActors.Add(GAMEID.GoldenUnicornDeluxe,         Context.ActorOf(Props.Create(() => new GoldenUnicornDeluxeGameLogic()),     "GoldenUnicornDeluxe"));
            _dicGameLogicActors.Add(GAMEID.FaCaiShenDeluxe,             Context.ActorOf(Props.Create(() => new FaCaiShenDeluxeGameLogic()),         "FaCaiShenDeluxe"));
            _dicGameLogicActors.Add(GAMEID.LuckyFortuneCat,             Context.ActorOf(Props.Create(() => new LuckyFortuneCatGameLogic()),         "LuckyFortuneCat"));
            _dicGameLogicActors.Add(GAMEID.HappyApe,                    Context.ActorOf(Props.Create(() => new HappyApeGameLogic()),                "HappyApe"));
            _dicGameLogicActors.Add(GAMEID.NaughtySanta,                Context.ActorOf(Props.Create(() => new NaughtySantaGameLogic()),            "NaughtySanta"));
            _dicGameLogicActors.Add(GAMEID.HotHotHalloween,             Context.ActorOf(Props.Create(() => new HotHotHalloweenGameLogic()),         "HotHotHalloween"));
            _dicGameLogicActors.Add(GAMEID.ColossalGems,                Context.ActorOf(Props.Create(() => new ColossalGemsGameLogic()),            "ColossalGems"));
            _dicGameLogicActors.Add(GAMEID.Nuwa,                        Context.ActorOf(Props.Create(() => new NuwaGameLogic()),                    "Nuwa"));
            _dicGameLogicActors.Add(GAMEID.LuckyLucky,                  Context.ActorOf(Props.Create(() => new LuckyLuckyGameLogic()),              "LuckyLucky"));
            _dicGameLogicActors.Add(GAMEID.MountMazuma,                 Context.ActorOf(Props.Create(() => new MountMazumaGameLogic()),             "MountMazuma"));
            _dicGameLogicActors.Add(GAMEID.TaikoBeats,                  Context.ActorOf(Props.Create(() => new TaikoBeatsGameLogic()),              "TaikoBeats"));
            _dicGameLogicActors.Add(GAMEID.HotHotFruit,                 Context.ActorOf(Props.Create(() => new HotHotFruitGameLogic()),             "HotHotFruit"));
            _dicGameLogicActors.Add(GAMEID.WaysOfFortune,               Context.ActorOf(Props.Create(() => new WaysOfFortuneGameLogic()),           "WaysOfFortune"));
            _dicGameLogicActors.Add(GAMEID.FiveMariachis,               Context.ActorOf(Props.Create(() => new FiveMariachisGameLogic()),           "5Mariachis"));
            _dicGameLogicActors.Add(GAMEID.ScruffyScallywags,           Context.ActorOf(Props.Create(() => new ScruffyScallywagsGameLogic()),       "ScruffyScallywags"));
            
            _dicGameLogicActors.Add(GAMEID.WeirdScience,                Context.ActorOf(Props.Create(() => new WeirdScienceGameLogic()),            "WeirdScience"));
            _dicGameLogicActors.Add(GAMEID.VikingsPlunder,              Context.ActorOf(Props.Create(() => new VikingsPlunderGameLogic()),          "VikingsPlunder"));
            _dicGameLogicActors.Add(GAMEID.DrFeelgood,                  Context.ActorOf(Props.Create(() => new DrFeelgoodGameLogic()),              "DrFeelgood"));
            _dicGameLogicActors.Add(GAMEID.DoubleODollars,              Context.ActorOf(Props.Create(() => new DoubleODollarsGameLogic()),          "DoubleODollars"));
            _dicGameLogicActors.Add(GAMEID.LittleGreenMoney,            Context.ActorOf(Props.Create(() => new LittleGreenMoneyGameLogic()),        "LittleGreenMoney"));
            _dicGameLogicActors.Add(GAMEID.RodeoDrive,                  Context.ActorOf(Props.Create(() => new RodeoDriveGameLogic()),              "RodeoDrive"));
            _dicGameLogicActors.Add(GAMEID.ShaolinFortunes100,          Context.ActorOf(Props.Create(() => new ShaolinFortunes100GameLogic()),      "ShaolinFortunes100"));
            _dicGameLogicActors.Add(GAMEID.MonsterMashCash,             Context.ActorOf(Props.Create(() => new MonsterMashCashGameLogic()),         "MonsterMashCash"));
            _dicGameLogicActors.Add(GAMEID.TheDragonCastle,             Context.ActorOf(Props.Create(() => new TheDragonCastleGameLogic()),         "TheDragonCastle"));
            _dicGameLogicActors.Add(GAMEID.ShaolinFortunes243,          Context.ActorOf(Props.Create(() => new ShaolinFortunes243GameLogic()),      "ShaolinFortunes243"));
            _dicGameLogicActors.Add(GAMEID.TreasureDiver,               Context.ActorOf(Props.Create(() => new TreasureDiverGameLogic()),           "TreasureDiver"));
            _dicGameLogicActors.Add(GAMEID.KingTutsTomb,                Context.ActorOf(Props.Create(() => new KingTutsTombGameLogic()),            "KingTutsTomb"));
            _dicGameLogicActors.Add(GAMEID.CarnivalCash,                Context.ActorOf(Props.Create(() => new CarnivalCashGameLogic()),            "CarnivalCash"));
            _dicGameLogicActors.Add(GAMEID.KanesInferno,                Context.ActorOf(Props.Create(() => new KanesInfernoGameLogic()),            "KanesInferno"));
            _dicGameLogicActors.Add(GAMEID.TreasureTomb,                Context.ActorOf(Props.Create(() => new TreasureTombGameLogic()),            "TreasureTomb"));
            _dicGameLogicActors.Add(GAMEID.HaZeus2,                     Context.ActorOf(Props.Create(() => new HaZeus2GameLogic()),                 "HaZeus2"));
            _dicGameLogicActors.Add(GAMEID.BuggyBonus,                  Context.ActorOf(Props.Create(() => new BuggyBonusGameLogic()),              "BuggyBonus"));
            _dicGameLogicActors.Add(GAMEID.GalacticCash,                Context.ActorOf(Props.Create(() => new GalacticCashGameLogic()),            "GalacticCash"));
            _dicGameLogicActors.Add(GAMEID.GlamRock,                    Context.ActorOf(Props.Create(() => new GlamRockGameLogic()),                "GlamRock"));
            _dicGameLogicActors.Add(GAMEID.PamperMe,                    Context.ActorOf(Props.Create(() => new PamperMeGameLogic()),                "PamperMe"));
            _dicGameLogicActors.Add(GAMEID.SpaceFortune,                Context.ActorOf(Props.Create(() => new SpaceFortuneGameLogic()),            "SpaceFortune"));
            _dicGameLogicActors.Add(GAMEID.HaZeus,                      Context.ActorOf(Props.Create(() => new HaZeusGameLogic()),                  "HaZeus"));
            _dicGameLogicActors.Add(GAMEID.PoolShark,                   Context.ActorOf(Props.Create(() => new PoolSharkGameLogic()),               "PoolShark"));
            _dicGameLogicActors.Add(GAMEID.SOS,                         Context.ActorOf(Props.Create(() => new SOSGameLogic()),                     "SOS"));
            _dicGameLogicActors.Add(GAMEID.TheDeadEscape,               Context.ActorOf(Props.Create(() => new TheDeadEscapeGameLogic()),           "TheDeadEscape"));

            _dicGameLogicActors.Add(GAMEID.ArcticWonders,               Context.ActorOf(Props.Create(() => new ArcticWondersGameLogic()),           "ArcticWonders"));
            _dicGameLogicActors.Add(GAMEID.JungleRumble,                Context.ActorOf(Props.Create(() => new JungleRumbleGameLogic()),            "JungleRumble"));
            _dicGameLogicActors.Add(GAMEID.SuperStrike,                 Context.ActorOf(Props.Create(() => new SuperStrikeGameLogic()),             "SuperStrike"));
            _dicGameLogicActors.Add(GAMEID.BarnstormerBucks,            Context.ActorOf(Props.Create(() => new BarnstormerBucksGameLogic()),        "BarnstormerBucks"));
            _dicGameLogicActors.Add(GAMEID.EgyptianDreams,              Context.ActorOf(Props.Create(() => new EgyptianDreamsGameLogic()),          "EgyptianDreams"));
            _dicGameLogicActors.Add(GAMEID.BikiniIsland,                Context.ActorOf(Props.Create(() => new BikiniIslandGameLogic()),            "BikiniIsland"));
            _dicGameLogicActors.Add(GAMEID.MummyMoney,                  Context.ActorOf(Props.Create(() => new MummyMoneyGameLogic()),              "MummyMoney"));
            _dicGameLogicActors.Add(GAMEID.TowerOfPizza,                Context.ActorOf(Props.Create(() => new TowerOfPizzaGameLogic()),            "TowerOfPizza"));
            _dicGameLogicActors.Add(GAMEID.MysticFortune,               Context.ActorOf(Props.Create(() => new MysticFortuneGameLogic()),           "MysticFortune"));
            _dicGameLogicActors.Add(GAMEID.MrBling,                     Context.ActorOf(Props.Create(() => new MrBlingGameLogic()),                 "MrBling"));
            _dicGameLogicActors.Add(GAMEID.AllForOne,                   Context.ActorOf(Props.Create(() => new AllForOneGameLogic()),               "AllForOne"));
            _dicGameLogicActors.Add(GAMEID.FlyingHigh,                  Context.ActorOf(Props.Create(() => new FlyingHighGameLogic()),              "FlyingHigh"));
            _dicGameLogicActors.Add(GAMEID.DragonsRealm,                Context.ActorOf(Props.Create(() => new DragonsRealmGameLogic()),            "DragonsRealm"));
            _dicGameLogicActors.Add(GAMEID.QueenOfQueens243,            Context.ActorOf(Props.Create(() => new QueenOfQueens243GameLogic()),        "QueenOfQueens243"));
            _dicGameLogicActors.Add(GAMEID.QueenOfQueens1024,           Context.ActorOf(Props.Create(() => new QueenOfQueens1024GameLogic()),       "QueenOfQueens1024"));
            _dicGameLogicActors.Add(GAMEID.CashReef,                    Context.ActorOf(Props.Create(() => new CashReefGameLogic()),                "CashReef"));
            _dicGameLogicActors.Add(GAMEID.GoldenUnicorn,               Context.ActorOf(Props.Create(() => new GoldenUnicornGameLogic()),           "GoldenUnicorn"));
            _dicGameLogicActors.Add(GAMEID.PuckerUpPrince,              Context.ActorOf(Props.Create(() => new PuckerUpPrinceGameLogic()),          "PuckerUpPrince"));
            _dicGameLogicActors.Add(GAMEID.SirBlingalot,                Context.ActorOf(Props.Create(() => new SirBlingalotGameLogic()),            "SirBlingalot"));
            _dicGameLogicActors.Add(GAMEID.IndianCashCatcher,           Context.ActorOf(Props.Create(() => new IndianCashCatcherGameLogic()),       "IndianCashCatcher"));
            _dicGameLogicActors.Add(GAMEID.TheBigDeal,                  Context.ActorOf(Props.Create(() => new TheBigDealGameLogic()),              "TheBigDeal"));
            _dicGameLogicActors.Add(GAMEID.DiscoFunk,                   Context.ActorOf(Props.Create(() => new DiscoFunkGameLogic()),               "DiscoFunk"));
            _dicGameLogicActors.Add(GAMEID.GoldRush,                    Context.ActorOf(Props.Create(() => new GoldRushGameLogic()),                "GoldRush"));
            _dicGameLogicActors.Add(GAMEID.Cashosaurus,                 Context.ActorOf(Props.Create(() => new CashosaurusGameLogic()),             "Cashosaurus"));
            _dicGameLogicActors.Add(GAMEID.SkysTheLimit,                Context.ActorOf(Props.Create(() => new SkysTheLimitGameLogic()),            "SkysTheLimit"));
            _dicGameLogicActors.Add(GAMEID.WickedWitch,                 Context.ActorOf(Props.Create(() => new WickedWitchGameLogic()),             "WickedWitch"));
            _dicGameLogicActors.Add(GAMEID.BombsAway,                   Context.ActorOf(Props.Create(() => new BombsAwayGameLogic()),               "BombsAway"));
            _dicGameLogicActors.Add(GAMEID.RuffledUp,                   Context.ActorOf(Props.Create(() => new RuffledUpGameLogic()),               "RuffledUp"));
            _dicGameLogicActors.Add(GAMEID.RomanEmpire,                 Context.ActorOf(Props.Create(() => new RomanEmpireGameLogic()),             "RomanEmpire"));
            _dicGameLogicActors.Add(GAMEID.HaFaCaiShen,                 Context.ActorOf(Props.Create(() => new HaFaCaiShenGameLogic()),             "HaFaCaiShen"));
            _dicGameLogicActors.Add(GAMEID.DragonsThrone,               Context.ActorOf(Props.Create(() => new DragonsThroneGameLogic()),           "DragonsThrone"));
            _dicGameLogicActors.Add(GAMEID.BirdOfThunder,               Context.ActorOf(Props.Create(() => new BirdOfThunderGameLogic()),           "BirdOfThunder"));
            _dicGameLogicActors.Add(GAMEID.CoyoteCrash,                 Context.ActorOf(Props.Create(() => new CoyoteCrashGameLogic()),             "CoyoteCrash"));
            _dicGameLogicActors.Add(GAMEID.Jugglenaut,                  Context.ActorOf(Props.Create(() => new JugglenautGameLogic()),              "Jugglenaut"));
            _dicGameLogicActors.Add(GAMEID.OceansCall,                  Context.ActorOf(Props.Create(() => new OceansCallGameLogic()),              "OceansCall"));
            _dicGameLogicActors.Add(GAMEID.SuperTwister,                Context.ActorOf(Props.Create(() => new SuperTwisterGameLogic()),            "SuperTwister"));
            _dicGameLogicActors.Add(GAMEID.TwelveZodiacs,               Context.ActorOf(Props.Create(() => new TwelveZodiacsGameLogic()),           "12Zodiacs"));

            _dicGameLogicActors.Add(GAMEID.SantasInn,                   Context.ActorOf(Props.Create(() => new SantasInnGameLogic()),               "SantasInn"));
            _dicGameLogicActors.Add(GAMEID.FruityHalloween,             Context.ActorOf(Props.Create(() => new FruityHalloweenGameLogic()),         "FruityHalloween"));
            _dicGameLogicActors.Add(GAMEID.SlimeParty,                  Context.ActorOf(Props.Create(() => new SlimePartyGameLogic()),              "SlimeParty"));
            _dicGameLogicActors.Add(GAMEID.MeowJanken,                  Context.ActorOf(Props.Create(() => new MeowJankenGameLogic()),              "MeowJanken"));
            _dicGameLogicActors.Add(GAMEID.BikiniIslandDeluxe,          Context.ActorOf(Props.Create(() => new BikiniIslandDeluxeGameLogic()),      "BikiniIslandDeluxe"));
            _dicGameLogicActors.Add(GAMEID.WitchesTome,                 Context.ActorOf(Props.Create(() => new WitchesTomeGameLogic()),             "WitchesTome"));
            _dicGameLogicActors.Add(GAMEID.LegendOfNezha,               Context.ActorOf(Props.Create(() => new LegendOfNezhaGameLogic()),           "LegendOfNezha"));
            _dicGameLogicActors.Add(GAMEID.TootyFruityFruits,           Context.ActorOf(Props.Create(() => new TootyFruityFruitsGameLogic()),       "TootyFruityFruits"));
            _dicGameLogicActors.Add(GAMEID.SirensSpell,                 Context.ActorOf(Props.Create(() => new SirensSpellGameLogic()),             "SirensSpell"));
            _dicGameLogicActors.Add(GAMEID.Crystopia,                   Context.ActorOf(Props.Create(() => new CrystopiaGameLogic()),               "Crystopia"));
            _dicGameLogicActors.Add(GAMEID.TheBigDealDeluxe,            Context.ActorOf(Props.Create(() => new TheBigDealDeluxeGameLogic()),        "TheBigDealDeluxe"));
            _dicGameLogicActors.Add(GAMEID.NaughtyWukong,               Context.ActorOf(Props.Create(() => new NaughtyWukongGameLogic()),           "NaughtyWukong"));
            _dicGameLogicActors.Add(GAMEID.LegendaryBeasts,             Context.ActorOf(Props.Create(() => new LegendaryBeastsGameLogic()),         "LegendaryBeasts"));
            _dicGameLogicActors.Add(GAMEID.Rainbowmania,                Context.ActorOf(Props.Create(() => new RainbowmaniaGameLogic()),            "Rainbowmania"));
            _dicGameLogicActors.Add(GAMEID.DragonTigerGate,             Context.ActorOf(Props.Create(() => new DragonTigerGateGameLogic()),         "DragonTigerGate"));
            _dicGameLogicActors.Add(GAMEID.SojuBomb,                    Context.ActorOf(Props.Create(() => new SojuBombGameLogic()),                "SojuBomb"));
            _dicGameLogicActors.Add(GAMEID.TukTukThailand,              Context.ActorOf(Props.Create(() => new TukTukThailandGameLogic()),          "TukTukThailand"));
            _dicGameLogicActors.Add(GAMEID.LaughingBuddha,              Context.ActorOf(Props.Create(() => new LaughingBuddhaGameLogic()),          "LaughingBuddha"));


            #endregion

            #region Playson Games
            _dicGameLogicActors.Add(GAMEID.BookDelSol,              Context.ActorOf(Props.Create(() => new BookDelSolGameLogic()),          "BookDelSol"));
            _dicGameLogicActors.Add(GAMEID.SolarQueen,              Context.ActorOf(Props.Create(() => new SolarQueenGameLogic()),          "SolarQueen"));
            _dicGameLogicActors.Add(GAMEID.SolarQueenMegaways,      Context.ActorOf(Props.Create(() => new SolarQueenMegawaysGameLogic()),  "SolarQueenMegaways"));
            _dicGameLogicActors.Add(GAMEID.RoyalJoker,              Context.ActorOf(Props.Create(() => new RoyalJokerGameLogic()),          "RoyalJoker"));
            _dicGameLogicActors.Add(GAMEID.CoinStrike,              Context.ActorOf(Props.Create(() => new CoinStrikeGameLogic()),          "CoinStrike"));
            _dicGameLogicActors.Add(GAMEID.GizaNights,              Context.ActorOf(Props.Create(() => new GizaNightsGameLogic()),          "GizaNights"));
            _dicGameLogicActors.Add(GAMEID.MammothPeak,             Context.ActorOf(Props.Create(() => new MammothPeakGameLogic()),         "MammothPeak"));
            _dicGameLogicActors.Add(GAMEID.HitTheBank,              Context.ActorOf(Props.Create(() => new HitTheBankGameLogic()),          "HitTheBank"));
            _dicGameLogicActors.Add(GAMEID.TreasuresFire,           Context.ActorOf(Props.Create(() => new TreasuresFireGameLogic()),       "TreasuresFire"));
            _dicGameLogicActors.Add(GAMEID.PirateChest,             Context.ActorOf(Props.Create(() => new PirateChestGameLogic()),         "PirateChest"));
            _dicGameLogicActors.Add(GAMEID.PirateSharky,            Context.ActorOf(Props.Create(() => new PirateSharkyGameLogic()),        "PirateSharky"));
            _dicGameLogicActors.Add(GAMEID.WolfPowerMega,           Context.ActorOf(Props.Create(() => new WolfPowerMegaGameLogic()),       "WolfPowerMega"));
            _dicGameLogicActors.Add(GAMEID.RoyalCoins2,             Context.ActorOf(Props.Create(() => new RoyalCoins2GameLogic()),         "RoyalCoins2"));
            _dicGameLogicActors.Add(GAMEID.UltraFort,               Context.ActorOf(Props.Create(() => new UltraFortGameLogic()),           "UltraFort"));
            _dicGameLogicActors.Add(GAMEID.HandOfGold,              Context.ActorOf(Props.Create(() => new HandOfGoldGameLogic()),          "HandOfGold"));
            _dicGameLogicActors.Add(GAMEID.RubyHit,                 Context.ActorOf(Props.Create(() => new RubyHitGameLogic()),             "RubyHit"));
            _dicGameLogicActors.Add(GAMEID.LuxorGold,               Context.ActorOf(Props.Create(() => new LuxorGoldGameLogic()),           "LuxorGold"));
            _dicGameLogicActors.Add(GAMEID.LionGems,                Context.ActorOf(Props.Create(() => new LionGemsGameLogic()),            "LionGems"));
            _dicGameLogicActors.Add(GAMEID.BurningFort,             Context.ActorOf(Props.Create(() => new BurningFortGameLogic()),         "BurningFort"));
            _dicGameLogicActors.Add(GAMEID.JokersCoins,             Context.ActorOf(Props.Create(() => new JokersCoinsGameLogic()),         "JokersCoins"));
            _dicGameLogicActors.Add(GAMEID.DiamondFort,             Context.ActorOf(Props.Create(() => new DiamondFortGameLogic()),         "DiamondFort"));
            _dicGameLogicActors.Add(GAMEID.BuffaloXmas,             Context.ActorOf(Props.Create(() => new BuffaloXmasGameLogic()),         "BuffaloXmas"));
            _dicGameLogicActors.Add(GAMEID.BurningWinsX2,           Context.ActorOf(Props.Create(() => new BurningWinsX2GameLogic()),       "BurningWinsX2"));
            _dicGameLogicActors.Add(GAMEID.RoyalCoins,              Context.ActorOf(Props.Create(() => new RoyalCoinsGameLogic()),          "RoyalCoins"));
            _dicGameLogicActors.Add(GAMEID.SpiritOfEgypt,           Context.ActorOf(Props.Create(() => new SpiritOfEgyptGameLogic()),       "SpiritOfEgypt"));
            _dicGameLogicActors.Add(GAMEID.BookOfGoldMultichance,   Context.ActorOf(Props.Create(() => new BookOfGoldMultichanceGameLogic()), "BookOfGoldMultichance"));
            _dicGameLogicActors.Add(GAMEID.EaglePower,              Context.ActorOf(Props.Create(() => new EaglePowerGameLogic()),          "EaglePower"));
            _dicGameLogicActors.Add(GAMEID.FiveFortunator,          Context.ActorOf(Props.Create(() => new FiveFortunatorGameLogic()),      "FiveFortunator"));
            _dicGameLogicActors.Add(GAMEID.HotBurningWins,          Context.ActorOf(Props.Create(() => new HotBurningWinsGameLogic()),      "HotBurningWins"));
            _dicGameLogicActors.Add(GAMEID.NineHappyPharaohs,       Context.ActorOf(Props.Create(() => new NineHappyPharaohsGameLogic()),   "NineHappyPharaohs"));
            _dicGameLogicActors.Add(GAMEID.BuffaloMegaways,         Context.ActorOf(Props.Create(() => new BuffaloMegawaysGameLogic()),     "BuffaloMegaways"));
            _dicGameLogicActors.Add(GAMEID.BookOfGoldDh,            Context.ActorOf(Props.Create(() => new BookOfGoldDhGameLogic()),        "BookOfGoldDh"));
            _dicGameLogicActors.Add(GAMEID.ThreeFruitsWin2Hit,      Context.ActorOf(Props.Create(() => new ThreeFruitsWin2HitGameLogic()),  "ThreeFruitsWin2Hit"));
            _dicGameLogicActors.Add(GAMEID.HotCoins,                Context.ActorOf(Props.Create(() => new HotCoinsGameLogic()),            "HotCoins"));
            _dicGameLogicActors.Add(GAMEID.StarsNFruits2Hit,        Context.ActorOf(Props.Create(() => new StarsNFruits2HitGameLogic()),    "StarsNFruits2Hit"));
            _dicGameLogicActors.Add(GAMEID.FiveSuperSevensNFruits6, Context.ActorOf(Props.Create(() => new FiveSuperSevensNFruits6GameLogic()), "FiveSuperSevensNFruits6"));
            _dicGameLogicActors.Add(GAMEID.SuperSunnyFruits,        Context.ActorOf(Props.Create(() => new SuperSunnyFruitsGameLogic()),    "SuperSunnyFruits"));
            _dicGameLogicActors.Add(GAMEID.LegendOfCleopatraMega,   Context.ActorOf(Props.Create(() => new LegendOfCleopatraMegaGameLogic()),"LegendOfCleopatraMega"));
            _dicGameLogicActors.Add(GAMEID.RichDiamonds,            Context.ActorOf(Props.Create(() => new RichDiamondsGameLogic()),            "RichDiamonds"));
            _dicGameLogicActors.Add(GAMEID.DivineDragon,            Context.ActorOf(Props.Create(() => new DivineDragonGameLogic()),            "DivineDragon"));
            _dicGameLogicActors.Add(GAMEID.WolfPower,               Context.ActorOf(Props.Create(() => new WolfPowerGameLogic()),               "WolfPower"));
            _dicGameLogicActors.Add(GAMEID.SolarKing,               Context.ActorOf(Props.Create(() => new SolarKingGameLogic()),               "SolarKing"));
            _dicGameLogicActors.Add(GAMEID.DiamondWins,             Context.ActorOf(Props.Create(() => new DiamondWinsGameLogic()),             "DiamondWins"));
            _dicGameLogicActors.Add(GAMEID.RiseOfEgyptDeluxe,       Context.ActorOf(Props.Create(() => new RiseOfEgyptDeluxeGameLogic()),       "RiseOfEgyptDeluxe"));
            _dicGameLogicActors.Add(GAMEID.PearlBeauty,             Context.ActorOf(Props.Create(() => new PearlBeautyGameLogic()),             "PearlBeauty"));
            _dicGameLogicActors.Add(GAMEID.BuffaloPower,            Context.ActorOf(Props.Create(() => new BuffaloPowerGameLogic()),            "BuffaloPower"));
            _dicGameLogicActors.Add(GAMEID.SevensNFruits,           Context.ActorOf(Props.Create(() => new SevensNFruitsGameLogic()),           "SevensNFruits"));
            _dicGameLogicActors.Add(GAMEID.SunnyFruits,             Context.ActorOf(Props.Create(() => new SunnyFruitsGameLogic()),             "SunnyFruits"));
            _dicGameLogicActors.Add(GAMEID.SevensNFruits6,          Context.ActorOf(Props.Create(() => new SevensNFruits6GameLogic()),          "SevensNFruits6"));
            _dicGameLogicActors.Add(GAMEID.SolarTemple,             Context.ActorOf(Props.Create(() => new SolarTempleGameLogic()),             "SolarTemple"));
            _dicGameLogicActors.Add(GAMEID.SuperBurningWinsRespin,  Context.ActorOf(Props.Create(() => new SuperBurningWinsRespinGameLogic()),  "SuperBurningWinsRespin"));
            _dicGameLogicActors.Add(GAMEID.ImperialFruits100,       Context.ActorOf(Props.Create(() => new ImperialFruits100GameLogic()),       "ImperialFruits100"));
            _dicGameLogicActors.Add(GAMEID.ImperialFruits40,        Context.ActorOf(Props.Create(() => new ImperialFruits40GameLogic()),        "ImperialFruits40"));
            _dicGameLogicActors.Add(GAMEID.BookOfGoldClassic,       Context.ActorOf(Props.Create(() => new BookOfGoldClassicGameLogic()),       "BookOfGoldClassic"));
            _dicGameLogicActors.Add(GAMEID.ThreeFruitsWin10,        Context.ActorOf(Props.Create(() => new ThreeFruitsWin10GameLogic()),        "ThreeFruitsWin10"));
            _dicGameLogicActors.Add(GAMEID.FruitsAndJokers100,      Context.ActorOf(Props.Create(() => new FruitsAndJokers100GameLogic()),      "FruitsAndJokers100"));
            _dicGameLogicActors.Add(GAMEID.VikingsFortune,          Context.ActorOf(Props.Create(() => new VikingsFortuneGameLogic()),          "VikingsFortune"));
            _dicGameLogicActors.Add(GAMEID.ImperialFruits5,         Context.ActorOf(Props.Create(() => new ImperialFruits5GameLogic()),         "ImperialFruits5"));
            _dicGameLogicActors.Add(GAMEID.SevensNFruits20,         Context.ActorOf(Props.Create(() => new SevensNFruits20GameLogic()),         "SevensNFruits20"));
            _dicGameLogicActors.Add(GAMEID.FruitsAndClovers20,      Context.ActorOf(Props.Create(() => new FruitsAndClovers20GameLogic()),      "FruitsAndClovers20"));
            _dicGameLogicActors.Add(GAMEID.MightyAfrica,            Context.ActorOf(Props.Create(() => new MightyAfricaGameLogic()),            "MightyAfrica"));
            _dicGameLogicActors.Add(GAMEID.HundredJokerStaxx,       Context.ActorOf(Props.Create(() => new HundredJokerStaxxGameLogic()),       "HundredJokerStaxx"));
            _dicGameLogicActors.Add(GAMEID.WildWarriors,            Context.ActorOf(Props.Create(() => new WildWarriorsGameLogic()),            "WildWarriors"));
            _dicGameLogicActors.Add(GAMEID.FruitsAndJokers40,       Context.ActorOf(Props.Create(() => new FruitsAndJokers40GameLogic()),       "FruitsAndJokers40"));
            _dicGameLogicActors.Add(GAMEID.BookOfGold,              Context.ActorOf(Props.Create(() => new BookOfGoldGameLogic()),              "BookOfGold"));
            _dicGameLogicActors.Add(GAMEID.JokerExpand40,           Context.ActorOf(Props.Create(() => new JokerExpand40GameLogic()),           "JokerExpand40"));
            _dicGameLogicActors.Add(GAMEID.FruitsAndJokers20,       Context.ActorOf(Props.Create(() => new FruitsAndJokers20GameLogic()),       "FruitsAndJokers20"));
            _dicGameLogicActors.Add(GAMEID.SuperBurningWins,        Context.ActorOf(Props.Create(() => new SuperBurningWinsGameLogic()),        "SuperBurningWins"));
            _dicGameLogicActors.Add(GAMEID.RiseOfEgypt,             Context.ActorOf(Props.Create(() => new RiseOfEgyptGameLogic()),             "RiseOfEgypt"));
            _dicGameLogicActors.Add(GAMEID.JokerExpand,             Context.ActorOf(Props.Create(() => new JokerExpandGameLogic()),             "JokerExpand"));
            _dicGameLogicActors.Add(GAMEID.BurningWins,             Context.ActorOf(Props.Create(() => new BurningWinsGameLogic()),             "BurningWins"));
            _dicGameLogicActors.Add(GAMEID.FourtyJokerStaxx,        Context.ActorOf(Props.Create(() => new FourtyJokerStaxxGameLogic()),        "FourtyJokerStaxx"));
            _dicGameLogicActors.Add(GAMEID.LegendOfCleopatra,       Context.ActorOf(Props.Create(() => new LegendOfCleopatraGameLogic()),       "LegendOfCleopatra"));
            _dicGameLogicActors.Add(GAMEID.FruitsNStarsC,           Context.ActorOf(Props.Create(() => new FruitsNStarsCGameLogic()),           "FruitsNStarsC"));
            _dicGameLogicActors.Add(GAMEID.JuiceAndFruitsC,         Context.ActorOf(Props.Create(() => new JuiceAndFruitsCGameLogic()),         "JuiceAndFruitsC"));
            _dicGameLogicActors.Add(GAMEID.BuffaloPower2,           Context.ActorOf(Props.Create(() => new BuffaloPower2GameLogic()),           "BuffaloPower2"));
            _dicGameLogicActors.Add(GAMEID.RoyalFort,               Context.ActorOf(Props.Create(() => new RoyalFortGameLogic()),               "RoyalFort"));
            _dicGameLogicActors.Add(GAMEID.EmpireGold,              Context.ActorOf(Props.Create(() => new EmpireGoldGameLogic()),              "EmpireGold"));
            _dicGameLogicActors.Add(GAMEID.WolfLand,                Context.ActorOf(Props.Create(() => new WolfLandGameLogic()),                "WolfLand"));
            #endregion

            foreach (KeyValuePair<GAMEID, IActorRef> pair in _dicGameLogicActors)
            {
                pair.Value.Tell(new DBProxyInform(_dbReader, _dbWriter, _redisWriter));
                pair.Value.Tell("loadSetting");
                _hashAllChildActors.Add(pair.Value.Path.Name);
                Context.Watch(pair.Value);
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
        public GAMEID GameID { get; private set; }
        public PerformanceTestRequest(GAMEID gameID)
        {
            this.GameID = gameID;
        }
    }
}
