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
            //슬롯게임들
            _dicGameLogicActors.Add(GAMEID.AfricanElephant,         Context.ActorOf(Props.Create(() => new AfricanElephantGameLogic()), "AfricanElephant"));
            _dicGameLogicActors.Add(GAMEID.GodsOfGiza,              Context.ActorOf(Props.Create(() => new GodsOfGizaGameLogic()), "GodsOfGiza"));
            _dicGameLogicActors.Add(GAMEID.JaneHunter,              Context.ActorOf(Props.Create(() => new JaneHunterGameLogic()), "JaneHunter"));
            _dicGameLogicActors.Add(GAMEID.WildCelebrityBusMega,    Context.ActorOf(Props.Create(() => new WildCelebrityBusMegaGameLogic()), "WildCelebrityBusMega"));

            _dicGameLogicActors.Add(GAMEID.TheRedQueen,             Context.ActorOf(Props.Create(() => new TheRedQueenGameLogic()), "TheRedQueen"));
            _dicGameLogicActors.Add(GAMEID.MoonShot,                Context.ActorOf(Props.Create(() => new MoonShotGameLogic()), "MoonShot"));
            _dicGameLogicActors.Add(GAMEID.TheDogHouseMultihold,    Context.ActorOf(Props.Create(() => new TheDogHouseMultiholdGameLogic()), "TheDogHouseMultihold"));
            _dicGameLogicActors.Add(GAMEID.BigBassHoldSpinner,      Context.ActorOf(Props.Create(() => new BigBassHoldSpinnerGameLogic()), "BigBassHoldSpinner"));
            _dicGameLogicActors.Add(GAMEID.TheKnightKing,           Context.ActorOf(Props.Create(() => new TheKnightKingGameLogic()), "TheKnightKing"));
            _dicGameLogicActors.Add(GAMEID.FruitsOfTheAmazon,       Context.ActorOf(Props.Create(() => new FruitsOfTheAmazonGameLogic()), "FruitsOfTheAmazon"));
            _dicGameLogicActors.Add(GAMEID.ThreeDancingMonkeys,     Context.ActorOf(Props.Create(() => new ThreeDancingMonkeysGameLogic()), "ThreeDancingMonkeys"));
            _dicGameLogicActors.Add(GAMEID.LeprechaunCarol,         Context.ActorOf(Props.Create(() => new LeprechaunCarolGameLogic()), "LeprechaunCarol"));
            _dicGameLogicActors.Add(GAMEID.BusyBees,                Context.ActorOf(Props.Create(() => new BusyBeesGameLogic()), "BusyBees"));
            _dicGameLogicActors.Add(GAMEID.DragonKingdomEyesOfFire, Context.ActorOf(Props.Create(() => new DragonKingdomEyesOfFireGameLogic()), "DragonKingdomEyesOfFire"));
            _dicGameLogicActors.Add(GAMEID.EmeraldKingClassic,      Context.ActorOf(Props.Create(() => new EmeraldKingClassicGameLogic()), "EmeraldKingClassic"));
            _dicGameLogicActors.Add(GAMEID.ReleaseTheKraken2,       Context.ActorOf(Props.Create(() => new ReleaseTheKraken2GameLogic()), "ReleaseTheKraken2"));
            
            _dicGameLogicActors.Add(GAMEID.GemsBonanza,             Context.ActorOf(Props.Create(() => new GemsBonanzaGameLogic()), "GemsBonanza"));
            _dicGameLogicActors.Add(GAMEID.GoldParty,               Context.ActorOf(Props.Create(() => new GoldPartyGameLogic()), "GoldParty"));
            _dicGameLogicActors.Add(GAMEID.SantasWonderland,        Context.ActorOf(Props.Create(() => new SantasWonderlandGameLogic()), "SantasWonderland"));
            _dicGameLogicActors.Add(GAMEID.FruitParty2,             Context.ActorOf(Props.Create(() => new FruitParty2GameLogic()), "FruitParty2"));
            _dicGameLogicActors.Add(GAMEID.GoblinHeistPowerNudge,   Context.ActorOf(Props.Create(() => new GoblinHeistPowerNudgeGameLogic()), "GoblinHeistPowerNudge"));
            _dicGameLogicActors.Add(GAMEID.RiseOfGizaPowerNudge,    Context.ActorOf(Props.Create(() => new RiseOfGizaPowerNudgeGameLogic()), "RiseOfGizaPowerNudge"));
            _dicGameLogicActors.Add(GAMEID.ReleaseTheKraken,        Context.ActorOf(Props.Create(() => new ReleaseTheKrakenGameLogic()), "ReleaseTheKraken"));
            _dicGameLogicActors.Add(GAMEID.CrystalCavernsMega,      Context.ActorOf(Props.Create(() => new CrystalCavernsMegaGameLogic()), "CrystalCavernsMega"));
            _dicGameLogicActors.Add(GAMEID.WildBeachParty,          Context.ActorOf(Props.Create(() => new WildBeachPartyGameLogic()), "WildBeachParty"));
            _dicGameLogicActors.Add(GAMEID.DragoJewelsOfFortune,    Context.ActorOf(Props.Create(() => new DragoJewelsOfFortuneGameLogic()), "DragoJewelsOfFortune"));
            _dicGameLogicActors.Add(GAMEID.NorthGuardians,          Context.ActorOf(Props.Create(() => new NorthGuardiansGameLogic()), "NorthGuardians"));
            _dicGameLogicActors.Add(GAMEID.ChilliHeatMega,          Context.ActorOf(Props.Create(() => new ChilliHeatMegaGameLogic()), "ChilliHeatMega"));
            _dicGameLogicActors.Add(GAMEID.JohnHunterAndTheQuest,   Context.ActorOf(Props.Create(() => new JohnHunterAndTheQuestGameLogic()), "JohnHunterAndTheQuest"));
            _dicGameLogicActors.Add(GAMEID.TemujinTreasures,        Context.ActorOf(Props.Create(() => new TemujinTreasuresGameLogic()), "TemujinTreasures"));
            _dicGameLogicActors.Add(GAMEID.GatesOfValhalla,         Context.ActorOf(Props.Create(() => new GatesOfValhallaGameLogic()), "GatesOfValhalla"));
            _dicGameLogicActors.Add(GAMEID.ChickenDrop,             Context.ActorOf(Props.Create(() => new ChickenDropGameLogic()), "ChickenDrop"));
            _dicGameLogicActors.Add(GAMEID.CurseOfTheWerewolfMega,  Context.ActorOf(Props.Create(() => new CurseOfTheWerewolfMegaGameLogic()), "CurseOfTheWerewolfMega"));
            _dicGameLogicActors.Add(GAMEID.GorillaMayhem,           Context.ActorOf(Props.Create(() => new GorillaMayhemGameLogic()), "GorillaMayhem"));
            _dicGameLogicActors.Add(GAMEID.ClubTropicana,           Context.ActorOf(Props.Create(() => new ClubTropicanaGameLogic()), "ClubTropicana"));
            _dicGameLogicActors.Add(GAMEID.FireArcher,              Context.ActorOf(Props.Create(() => new FireArcherGameLogic()), "FireArcher"));
            _dicGameLogicActors.Add(GAMEID.BookOfTut,               Context.ActorOf(Props.Create(() => new BookOfTutGameLogic()), "BookOfTut"));
            _dicGameLogicActors.Add(GAMEID.OctobeerFortunes,        Context.ActorOf(Props.Create(() => new OctobeerFortunesGameLogic()), "OctobeerFortunes"));
            _dicGameLogicActors.Add(GAMEID.MagicianSecrets,         Context.ActorOf(Props.Create(() => new MagicianSecretsGameLogic()), "MagicianSecrets"));
            _dicGameLogicActors.Add(GAMEID.BookOfTheFallen,         Context.ActorOf(Props.Create(() => new BookOfTheFallenGameLogic()), "BookOfTheFallen"));
            _dicGameLogicActors.Add(GAMEID.CandyStars,              Context.ActorOf(Props.Create(() => new CandyStarsGameLogic()), "CandyStars"));
            _dicGameLogicActors.Add(GAMEID.JokerJewels,             Context.ActorOf(Props.Create(() => new JokerJewelsGameLogic()), "JokerJewels"));
            _dicGameLogicActors.Add(GAMEID.Triple8Dragons,          Context.ActorOf(Props.Create(() => new Triple8DragonsGameLogic()), "Triple8Dragons"));
            _dicGameLogicActors.Add(GAMEID.WildHopAndDrop,          Context.ActorOf(Props.Create(() => new WildHopAndDropGameLogic()), "WildHopAndDrop"));
            _dicGameLogicActors.Add(GAMEID.BookOfGoldenSands,       Context.ActorOf(Props.Create(() => new BookOfGoldenSandsGameLogic()), "BookOfGoldenSands"));
            _dicGameLogicActors.Add(GAMEID.AztecBonanza,            Context.ActorOf(Props.Create(() => new AztecBonanzaGameLogic()), "AztecBonanza"));
            _dicGameLogicActors.Add(GAMEID.StrikingHot5,            Context.ActorOf(Props.Create(() => new StrikingHot5GameLogic()), "StrikingHot5"));
            _dicGameLogicActors.Add(GAMEID.SnakesAndLaddersSnakeEyes, Context.ActorOf(Props.Create(() => new SnakesAndLaddersSnakeEyesGameLogic()), "SnakesAndLaddersSnakeEyes"));
            _dicGameLogicActors.Add(GAMEID.BlackBull,               Context.ActorOf(Props.Create(() => new BlackBullGameLogic()), "BlackBull"));
            _dicGameLogicActors.Add(GAMEID.MagicMoneyMaze,          Context.ActorOf(Props.Create(() => new MagicMoneyMazeGameLogic()), "MagicMoneyMaze"));
            _dicGameLogicActors.Add(GAMEID.CrownOfFire,             Context.ActorOf(Props.Create(() => new CrownOfFireGameLogic()), "CrownOfFire"));
            _dicGameLogicActors.Add(GAMEID.GreedyWolf,              Context.ActorOf(Props.Create(() => new GreedyWolfGameLogic()), "GreedyWolf"));
            _dicGameLogicActors.Add(GAMEID.FireHot5,                Context.ActorOf(Props.Create(() => new FireHot5GameLogic()), "FireHot5"));
            _dicGameLogicActors.Add(GAMEID.FireHot20,               Context.ActorOf(Props.Create(() => new FireHot20GameLogic()), "FireHot20"));
            _dicGameLogicActors.Add(GAMEID.FireHot40,               Context.ActorOf(Props.Create(() => new FireHot40GameLogic()), "FireHot40"));
            _dicGameLogicActors.Add(GAMEID.FireHot100,              Context.ActorOf(Props.Create(() => new FireHot100GameLogic()), "FireHot100"));
            _dicGameLogicActors.Add(GAMEID.ShiningHot5,             Context.ActorOf(Props.Create(() => new ShiningHot5GameLogic()), "ShiningHot5"));
            _dicGameLogicActors.Add(GAMEID.ShiningHot20,            Context.ActorOf(Props.Create(() => new ShiningHot20GameLogic()), "ShiningHot20"));
            _dicGameLogicActors.Add(GAMEID.ShiningHot40,            Context.ActorOf(Props.Create(() => new ShiningHot40GameLogic()), "ShiningHot40"));
            _dicGameLogicActors.Add(GAMEID.ShiningHot100,           Context.ActorOf(Props.Create(() => new ShiningHot100GameLogic()), "ShiningHot100"));
            _dicGameLogicActors.Add(GAMEID.AztecGemsDeluxe,         Context.ActorOf(Props.Create(() => new AztecGemsDeluxeGameLogic()), "AztecGemsDeluxe"));
            _dicGameLogicActors.Add(GAMEID.DownTheRails,            Context.ActorOf(Props.Create(() => new DownTheRailsGameLogic()), "DownTheRails"));
            _dicGameLogicActors.Add(GAMEID.BombBonanza,             Context.ActorOf(Props.Create(() => new BombBonanzaGameLogic()), "BombBonanza"));
            _dicGameLogicActors.Add(GAMEID.FireStrike,              Context.ActorOf(Props.Create(() => new FireStrikeGameLogic()), "FireStrike"));
            _dicGameLogicActors.Add(GAMEID.FireStrike2,             Context.ActorOf(Props.Create(() => new FireStrike2GameLogic()), "FireStrike2"));
            _dicGameLogicActors.Add(GAMEID.HotToBurnExtreme,        Context.ActorOf(Props.Create(() => new HotToBurnExtremeGameLogic()), "HotToBurnExtreme"));
            _dicGameLogicActors.Add(GAMEID.CheekyEmperor,           Context.ActorOf(Props.Create(() => new CheekyEmperorGameLogic()), "CheekyEmperor"));
            _dicGameLogicActors.Add(GAMEID.CaishenCash,             Context.ActorOf(Props.Create(() => new CaishenCashGameLogic()), "CaishenCash"));
            _dicGameLogicActors.Add(GAMEID.LittleGem,               Context.ActorOf(Props.Create(() => new LittleGemGameLogic()), "LittleGem"));
            _dicGameLogicActors.Add(GAMEID.QueenOfGods,             Context.ActorOf(Props.Create(() => new QueenOfGodsGameLogic()), "QueenOfGods"));
            _dicGameLogicActors.Add(GAMEID.CosmicCash,              Context.ActorOf(Props.Create(() => new CosmicCashGameLogic()), "CosmicCash"));
            _dicGameLogicActors.Add(GAMEID.TheGreatStickUp,         Context.ActorOf(Props.Create(() => new TheGreatStickUpGameLogic()), "TheGreatStickUp"));
            _dicGameLogicActors.Add(GAMEID.EyeOfCleopatra,          Context.ActorOf(Props.Create(() => new EyeOfCleopatraGameLogic()), "EyeOfCleopatra"));
            _dicGameLogicActors.Add(GAMEID.SpiritOfAdventure,       Context.ActorOf(Props.Create(() => new SpiritOfAdventureGameLogic()), "SpiritOfAdventure"));
            _dicGameLogicActors.Add(GAMEID.DrillThatGold,           Context.ActorOf(Props.Create(() => new DrillThatGoldGameLogic()), "DrillThatGold"));
            _dicGameLogicActors.Add(GAMEID.CloverGold,              Context.ActorOf(Props.Create(() => new CloverGoldGameLogic()), "CloverGold"));
            _dicGameLogicActors.Add(GAMEID.JohnHunterTombScarabQueen, Context.ActorOf(Props.Create(() => new JohnHunterTombScarabQueenGameLogic()), "JohnHunterTombScarabQueen"));
            _dicGameLogicActors.Add(GAMEID.TheAmazingMoneyMachine,  Context.ActorOf(Props.Create(() => new TheAmazingMoneyMachineGameLogic()), "TheAmazingMoneyMachine"));
            _dicGameLogicActors.Add(GAMEID.ElementalGemsMega,       Context.ActorOf(Props.Create(() => new ElementalGemsMegaGameLogic()), "ElementalGemsMega"));
            _dicGameLogicActors.Add(GAMEID.PhoenixForge,            Context.ActorOf(Props.Create(() => new PhoenixForgeGameLogic()), "PhoenixForge"));

            
            _dicGameLogicActors.Add(GAMEID.CandyVillage,            Context.ActorOf(Props.Create(() => new CandyVillageGameLogic()), "CandyVillage"));
            _dicGameLogicActors.Add(GAMEID.ChristmasCarolMega,      Context.ActorOf(Props.Create(() => new ChristmasCarolMegaGameLogic()), "ChristmasCarolMega"));
            _dicGameLogicActors.Add(GAMEID.CoffeeWild,              Context.ActorOf(Props.Create(() => new CoffeeWildGameLogic()), "CoffeeWild"));
            _dicGameLogicActors.Add(GAMEID.ExtraJuicyMega,          Context.ActorOf(Props.Create(() => new ExtraJuicyMegaGameLogic()), "ExtraJuicyMega"));
            _dicGameLogicActors.Add(GAMEID.FloatingDragonMega,      Context.ActorOf(Props.Create(() => new FloatingDragonMegaGameLogic()), "FloatingDragonMega"));
            _dicGameLogicActors.Add(GAMEID.GatotKacaFury,           Context.ActorOf(Props.Create(() => new GatotKacaFuryGameLogic()), "GatotKacaFury"));
            _dicGameLogicActors.Add(GAMEID.LegendOfHeroesMega,      Context.ActorOf(Props.Create(() => new LegendOfHeroesMegaGameLogic()), "LegendOfHeroesMega"));
            _dicGameLogicActors.Add(GAMEID.LuckyLightning,          Context.ActorOf(Props.Create(() => new LuckyLightningGameLogic()), "LuckyLightning"));
            _dicGameLogicActors.Add(GAMEID.Mochimon,                Context.ActorOf(Props.Create(() => new MochimonGameLogic()), "Mochimon"));
            _dicGameLogicActors.Add(GAMEID.MuertosMultiplierMega,   Context.ActorOf(Props.Create(() => new MuertosMultiplierMegaGameLogic()), "MuertosMultiplierMega"));
            _dicGameLogicActors.Add(GAMEID.TropicalTiki,            Context.ActorOf(Props.Create(() => new TropicalTikiGameLogic()), "TropicalTiki"));
            _dicGameLogicActors.Add(GAMEID.AncientEgyptClassic,         Context.ActorOf(Props.Create(() => new AncientEgyptClassicGameLogic()), "AncientEgyptClassic"));
            _dicGameLogicActors.Add(GAMEID.BeoWulf,                     Context.ActorOf(Props.Create(() => new BeoWulfGameLogic()), "BeoWulf"));
            _dicGameLogicActors.Add(GAMEID.BuffaloKing,                 Context.ActorOf(Props.Create(() => new BuffaloKingGameLogic()), "BuffaloKing"));
            _dicGameLogicActors.Add(GAMEID.JuicyFruit,                  Context.ActorOf(Props.Create(() => new JuicyFruitGameLogic()), "JuicyFruit"));
            _dicGameLogicActors.Add(GAMEID.PandasFortune,               Context.ActorOf(Props.Create(() => new PandasFortuneGameLogic()), "PandasFortune"));
            _dicGameLogicActors.Add(GAMEID.SpartaKing,                  Context.ActorOf(Props.Create(() => new SpartaKingGameLogic()), "SpartaKing"));
            _dicGameLogicActors.Add(GAMEID.StarzMegaWays,               Context.ActorOf(Props.Create(() => new StarzMegawaysGameLogic()), "StarzMegaWays"));
            _dicGameLogicActors.Add(GAMEID.TheDogHouse,                 Context.ActorOf(Props.Create(() => new TheDogHouseGameLogic()), "TheDogHouse"));
            _dicGameLogicActors.Add(GAMEID.WestWildGold,                Context.ActorOf(Props.Create(() => new WestWildGoldGameLogic()), "WestWildGold"));
            _dicGameLogicActors.Add(GAMEID.PandaFortune2,               Context.ActorOf(Props.Create(() => new PandaFortune2GameLogic()), "PandasFortune2"));
            _dicGameLogicActors.Add(GAMEID.ReturnDead,                  Context.ActorOf(Props.Create(() => new ReturnDeadGameLogic()), "ReturnDead"));
            _dicGameLogicActors.Add(GAMEID.UltraBurn,                   Context.ActorOf(Props.Create(() => new UltraBurnGameLogic()), "UltraBurn"));
            _dicGameLogicActors.Add(GAMEID.PirateGold,                  Context.ActorOf(Props.Create(() => new PirateGoldGameLogic()), "PirateGold"));
            _dicGameLogicActors.Add(GAMEID.HotToBurnHoldAndSpin,        Context.ActorOf(Props.Create(() => new HotToBurnHoldAndSpinGameLogic()), "HotToBurnHoldAndSpin"));
            _dicGameLogicActors.Add(GAMEID.UltraHoldAndSpin,            Context.ActorOf(Props.Create(() => new UltraHoldAndSpinGameLogic()), "UltraHoldAndSpin"));
            _dicGameLogicActors.Add(GAMEID.OlympusGates,                Context.ActorOf(Props.Create(() => new OlympusGatesGameLogic()), "OlympusGates"));
            _dicGameLogicActors.Add(GAMEID.DiamondStrike,               Context.ActorOf(Props.Create(() => new DiamondStrikeGameLogic()), "DiamondStrike"));
            _dicGameLogicActors.Add(GAMEID.WolfGold,                    Context.ActorOf(Props.Create(() => new WolfGoldGameLogic()), "WolfGold"));
            _dicGameLogicActors.Add(GAMEID.GoldenOx,                    Context.ActorOf(Props.Create(() => new GoldenOxGameLogic()), "GoldenOx"));
            _dicGameLogicActors.Add(GAMEID.MadameDestinyMegaWays,       Context.ActorOf(Props.Create(() => new MadameDestinyMegaGameLogic()), "MadameDestinyMegaWays"));
            _dicGameLogicActors.Add(GAMEID.FruitParty,                  Context.ActorOf(Props.Create(() => new FruitPartyGameLogic()), "FruitParty"));
            _dicGameLogicActors.Add(GAMEID.GreatRhinoMega,              Context.ActorOf(Props.Create(() => new GreatRhinoMegaGameLogic()), "GreatRhinoMega"));
            _dicGameLogicActors.Add(GAMEID.GreatRhino,                  Context.ActorOf(Props.Create(() => new GreatRhinoGameLogic()), "GreatRhino"));
            _dicGameLogicActors.Add(GAMEID.GreatRhinoDelux,             Context.ActorOf(Props.Create(() => new GreatRhinoDeluxGameLogic()), "GreatRhinoDelux"));
            _dicGameLogicActors.Add(GAMEID.FiveLionsMega,               Context.ActorOf(Props.Create(() => new FiveLionsMegaGameLogic()), "FiveLionsMega"));
            _dicGameLogicActors.Add(GAMEID.FiveLions,                   Context.ActorOf(Props.Create(() => new FiveLionsGameLogic()), "FiveLions"));
            _dicGameLogicActors.Add(GAMEID.FiveLionsGold,               Context.ActorOf(Props.Create(() => new FiveLionsGoldGameLogic()), "FiveLionsGold"));
            _dicGameLogicActors.Add(GAMEID.TheDogHouseMega,             Context.ActorOf(Props.Create(() => new TheDogHouseMegaGameLogic()), "TheDogHouseMega"));
            _dicGameLogicActors.Add(GAMEID.HerculesAndPegasus,          Context.ActorOf(Props.Create(() => new HerculesAndPegasusGameLogic()), "HerculesAndPegasus"));
            _dicGameLogicActors.Add(GAMEID.EgyptianFortunes,            Context.ActorOf(Props.Create(() => new EgyptianFortunesGameLogic()), "EgyptianFortunes"));
            _dicGameLogicActors.Add(GAMEID.FiveLionsDance,              Context.ActorOf(Props.Create(() => new FiveLionsDanceGameLogic()), "FiveLionsDance"));
            _dicGameLogicActors.Add(GAMEID.HotToBurn,                   Context.ActorOf(Props.Create(() => new HotToBurnGameLogic()), "HotToBurn"));
            _dicGameLogicActors.Add(GAMEID.LuckyNewYear,                Context.ActorOf(Props.Create(() => new LuckyNewYearGameLogic()), "LuckyNewYear"));
            _dicGameLogicActors.Add(GAMEID.WildBooster,                 Context.ActorOf(Props.Create(() => new WildBoosterGameLogic()), "WildBooster"));
            _dicGameLogicActors.Add(GAMEID.MonkeyWarrior,               Context.ActorOf(Props.Create(() => new MonkeyWarriorGameLogic()), "MonkeyWarrior"));
            _dicGameLogicActors.Add(GAMEID.DanceParty,                  Context.ActorOf(Props.Create(() => new DancePartyGameLogic()), "DanceParty"));
            _dicGameLogicActors.Add(GAMEID.DragonHoldAndSpin,           Context.ActorOf(Props.Create(() => new DragonHoldAndSpinGameLogic()), "DragonHoldAndSpin"));
            _dicGameLogicActors.Add(GAMEID.BuffaloKingMega,             Context.ActorOf(Props.Create(() => new BuffaloKingMegaGameLogic()), "BuffaloKingMega"));
            _dicGameLogicActors.Add(GAMEID.VegasMagic,                  Context.ActorOf(Props.Create(() => new VegasMagicGameLogic()), "VegasMagic"));
            _dicGameLogicActors.Add(GAMEID.AztecKingMega,               Context.ActorOf(Props.Create(() => new AztecKingMegaGameLogic()), "AztecKingMega"));
            _dicGameLogicActors.Add(GAMEID.HeartOfRio,                  Context.ActorOf(Props.Create(() => new HeartOfRioGameLogic()), "HeartOfRio"));
            _dicGameLogicActors.Add(GAMEID.JokerKing,                   Context.ActorOf(Props.Create(() => new JokerKingGameLogic()), "JokerKing"));
            _dicGameLogicActors.Add(GAMEID.MustangGold,                 Context.ActorOf(Props.Create(() => new MustangGoldGameLogic()), "MustangGold"));
            _dicGameLogicActors.Add(GAMEID.BroncoSpirit,                Context.ActorOf(Props.Create(() => new BroncoSpiritGameLogic()), "BroncoSpirit"));
            _dicGameLogicActors.Add(GAMEID.Asgard,                      Context.ActorOf(Props.Create(() => new AsgardGameLogic()), "Asgard"));
            _dicGameLogicActors.Add(GAMEID.BiggerBassBonanza,           Context.ActorOf(Props.Create(() => new BiggerBassBonanzaGameLogic()), "BiggerBassBonanza"));
            _dicGameLogicActors.Add(GAMEID.AztecGems,                   Context.ActorOf(Props.Create(() => new AztecGemsGameLogic()), "AztecGems"));
            _dicGameLogicActors.Add(GAMEID.BigBassBonanza,              Context.ActorOf(Props.Create(() => new BigBassBonanzaGameLogic()), "BigBassBonanza"));
            _dicGameLogicActors.Add(GAMEID.CashElevator,                Context.ActorOf(Props.Create(() => new CashElevatorGameLogic()), "CashElevator"));
            _dicGameLogicActors.Add(GAMEID.CowBoysGold,                 Context.ActorOf(Props.Create(() => new CowBoysGoldGameLogic()), "CowBoysGold"));
            _dicGameLogicActors.Add(GAMEID.EightDragons,                Context.ActorOf(Props.Create(() => new EightDragonsGameLogic()), "EightDragons"));
            _dicGameLogicActors.Add(GAMEID.HandOfMidas,                 Context.ActorOf(Props.Create(() => new HandOfMidasGameLogic()), "HandOfMidas"));
            _dicGameLogicActors.Add(GAMEID.MoneyMouse,                  Context.ActorOf(Props.Create(() => new MoneyMouseGameLogic()), "MoneyMouse"));
            _dicGameLogicActors.Add(GAMEID.PeKingLuck,                  Context.ActorOf(Props.Create(() => new PeKingLuckGameLogic()), "PeKingLuck"));
            _dicGameLogicActors.Add(GAMEID.PirateGoldDelux,             Context.ActorOf(Props.Create(() => new PirateGoldDeluxGameLogic()), "PirateGoldDelux"));
            _dicGameLogicActors.Add(GAMEID.PowerOfThorMega,             Context.ActorOf(Props.Create(() => new PowerOfThorMegaGameLogic()), "PowerOfThorMega"));
            _dicGameLogicActors.Add(GAMEID.PyramidBonanza,              Context.ActorOf(Props.Create(() => new PyramidBonanzaGameLogic()), "PyramidBonanza"));
            _dicGameLogicActors.Add(GAMEID.TheTweetyHouse,              Context.ActorOf(Props.Create(() => new TheTweetyHouseGameLogic()), "TheTweetyHouse"));
            _dicGameLogicActors.Add(GAMEID.ChristmasBigBassBonanza,     Context.ActorOf(Props.Create(() => new ChristmasBigBassBonanzaGameLogic()), "ChristmasBigBassBonanza"));
            _dicGameLogicActors.Add(GAMEID.FloatingDragonHoldAndSpin,   Context.ActorOf(Props.Create(() => new FloatingDragonHoldAndSpinGameLogic()), "FloatingDragonHoldAndSpin"));
            _dicGameLogicActors.Add(GAMEID.FortuneOfGiza,               Context.ActorOf(Props.Create(() => new FortuneOfGizaGameLogic()), "FortuneOfGiza"));
            _dicGameLogicActors.Add(GAMEID.HotFiesta,                   Context.ActorOf(Props.Create(() => new HotFiestaGameLogic()), "HotFiesta"));
            _dicGameLogicActors.Add(GAMEID.BigJuan,                     Context.ActorOf(Props.Create(() => new BigJuanGameLogic()), "BigJuan"));
            _dicGameLogicActors.Add(GAMEID.CashPatrol,                  Context.ActorOf(Props.Create(() => new CashPatrolGameLogic()), "CashPatrol"));
            _dicGameLogicActors.Add(GAMEID.Cleocatra,                   Context.ActorOf(Props.Create(() => new CleocatraGameLogic()), "Cleocatra"));
            _dicGameLogicActors.Add(GAMEID.EmptyTheBank,                Context.ActorOf(Props.Create(() => new EmptyTheBankGameLogic()), "EmptyTheBank"));
            _dicGameLogicActors.Add(GAMEID.KoiPond,                     Context.ActorOf(Props.Create(() => new KoiPondGameLogic()), "KoiPond"));
            _dicGameLogicActors.Add(GAMEID.Queenie,                     Context.ActorOf(Props.Create(() => new QueenieGameLogic()), "Queenie"));
            _dicGameLogicActors.Add(GAMEID.RockVegas,                   Context.ActorOf(Props.Create(() => new RockVegasGameLogic()), "RockVegas"));
            _dicGameLogicActors.Add(GAMEID.TheUltimate5,                Context.ActorOf(Props.Create(() => new TheUltimate5GameLogic()), "TheUltimate5"));
            _dicGameLogicActors.Add(GAMEID.TreasureWild,                Context.ActorOf(Props.Create(() => new TreasureWildGameLogic()), "TreasureWild"));
            _dicGameLogicActors.Add(GAMEID.ZombieCarnival,              Context.ActorOf(Props.Create(() => new ZombieCarnivalGameLogic()), "ZombieCarnival"));
            _dicGameLogicActors.Add(GAMEID.SugarRush,                   Context.ActorOf(Props.Create(() => new SugarRushGameLogic()), "SugarRush"));
            _dicGameLogicActors.Add(GAMEID.BonanzaGold,                 Context.ActorOf(Props.Create(() => new BonanzaGoldGameLogic()), "BonanzaGold"));
            _dicGameLogicActors.Add(GAMEID.StarlightPrincess,           Context.ActorOf(Props.Create(() => new StarlightPrincessGameLogic()), "StarlightPrincess"));
            _dicGameLogicActors.Add(GAMEID.SweetBonanza,                Context.ActorOf(Props.Create(() => new SweetBonanzaGameLogic()), "SweetBonanza"));
            _dicGameLogicActors.Add(GAMEID.SweetBonanzaXmas,            Context.ActorOf(Props.Create(() => new SweetBonanzaXmasGameLogic()), "SweetBonanzaXmas"));
            _dicGameLogicActors.Add(GAMEID.SwordOfAres,                 Context.ActorOf(Props.Create(() => new SwordOfAresGameLogic()), "SwordOfAres"));
            

            _dicGameLogicActors.Add(GAMEID.WildWestGoldMega, Context.ActorOf(Props.Create(() => new WildWestGoldMegaGameLogic()), "WildWestGoldMega"));
            _dicGameLogicActors.Add(GAMEID.ColossalCashZone, Context.ActorOf(Props.Create(() => new ColossalCashZoneGameLogic()), "ColossalCashZone"));
            _dicGameLogicActors.Add(GAMEID.LeprechaunSong, Context.ActorOf(Props.Create(() => new LeprechaunSongGameLogic()), "LeprechaunSong"));
            _dicGameLogicActors.Add(GAMEID.BarnFestival, Context.ActorOf(Props.Create(() => new BarnFestivalGameLogic()), "BarnFestival"));
            _dicGameLogicActors.Add(GAMEID.RiseOfSamuraiMega, Context.ActorOf(Props.Create(() => new RiseOfSamuraiMegaGameLogic()), "RiseOfSamuraiMega"));
            _dicGameLogicActors.Add(GAMEID.BigBassSplash, Context.ActorOf(Props.Create(() => new BigBassSplashGameLogic()), "BigBassSplash"));
            _dicGameLogicActors.Add(GAMEID.WildWildBananas, Context.ActorOf(Props.Create(() => new WildWildBananasGameLogic()), "WildWildBananas"));
            _dicGameLogicActors.Add(GAMEID.PizzaPizzaPizza, Context.ActorOf(Props.Create(() => new PizzaPizzaPizzaGameLogic()), "PizzaPizzaPizza"));
            _dicGameLogicActors.Add(GAMEID.FuryOfOdinMega, Context.ActorOf(Props.Create(() => new FuryOfOdinMegaGameLogic()), "FuryOfOdinMega"));
            _dicGameLogicActors.Add(GAMEID.ReelBanks, Context.ActorOf(Props.Create(() => new ReelBanksGameLogic()), "ReelBanks"));
            _dicGameLogicActors.Add(GAMEID.SweetPowernudge, Context.ActorOf(Props.Create(() => new SweetPowernudgeGameLogic()), "SweetPowernudge"));
            _dicGameLogicActors.Add(GAMEID.DragonHero, Context.ActorOf(Props.Create(() => new DragonHeroGameLogic()), "DragonHero"));
            _dicGameLogicActors.Add(GAMEID.BigBassKeepingItReel, Context.ActorOf(Props.Create(() => new BigBassKeepingItReelGameLogic()), "BigBassKeepingItReel"));
            _dicGameLogicActors.Add(GAMEID.PirateGoldenAge, Context.ActorOf(Props.Create(() => new PirateGoldenAgeGameLogic()), "PirateGoldenAge"));
            _dicGameLogicActors.Add(GAMEID.FirebirdSpirit, Context.ActorOf(Props.Create(() => new FirebirdSpiritGameLogic()), "FirebirdSpirit"));
            _dicGameLogicActors.Add(GAMEID.ToweringFortunes, Context.ActorOf(Props.Create(() => new ToweringFortunesGameLogic()), "ToweringFortunes"));
            _dicGameLogicActors.Add(GAMEID.StarlightChristmas, Context.ActorOf(Props.Create(() => new StarlightChristmasGameLogic()), "StarlightChristmas"));
            _dicGameLogicActors.Add(GAMEID.GemsOfSerengeti, Context.ActorOf(Props.Create(() => new GemsOfSerengetiGameLogic()), "GemsOfSerengeti"));
            _dicGameLogicActors.Add(GAMEID.BiggerBassBlizzard, Context.ActorOf(Props.Create(() => new BiggerBassBlizzardGameLogic()), "BiggerBassBlizzard"));
            _dicGameLogicActors.Add(GAMEID.RiseOfSamurai3, Context.ActorOf(Props.Create(() => new RiseOfSamurai3GameLogic()), "RiseOfSamurai3"));
            _dicGameLogicActors.Add(GAMEID.GatesOfAztec, Context.ActorOf(Props.Create(() => new GatesOfAztecGameLogic()), "GatesOfAztec"));
            _dicGameLogicActors.Add(GAMEID.GatesOfGatotKaca, Context.ActorOf(Props.Create(() => new GatesOfGatotKacaGameLogic()), "GatesOfGatotKaca"));
            _dicGameLogicActors.Add(GAMEID.HotPepper, Context.ActorOf(Props.Create(() => new HotPepperGameLogic()), "HotPepper"));
            _dicGameLogicActors.Add(GAMEID.BookOfTutRespin, Context.ActorOf(Props.Create(() => new BookOfTutRespinGameLogic()), "BookOfTutRespin"));
            _dicGameLogicActors.Add(GAMEID.FiveRabbitsMega, Context.ActorOf(Props.Create(() => new FiveRabbitsMegaGameLogic()), "FiveRabbitsMega"));
            _dicGameLogicActors.Add(GAMEID.KingdomOfAsgard, Context.ActorOf(Props.Create(() => new KingdomOfAsgardGameLogic()), "KingdomOfAsgard"));
            _dicGameLogicActors.Add(GAMEID.SantaGreatGifts, Context.ActorOf(Props.Create(() => new SantaGreatGiftsGameLogic()), "SantaGreatGifts"));
            _dicGameLogicActors.Add(GAMEID.ShieldOfSparta, Context.ActorOf(Props.Create(() => new ShieldOfSpartaGameLogic()), "ShieldOfSparta"));
            _dicGameLogicActors.Add(GAMEID.AztecBlaze, Context.ActorOf(Props.Create(() => new AztecBlazeGameLogic()), "AztecBlaze"));
            _dicGameLogicActors.Add(GAMEID.LuckyFishing, Context.ActorOf(Props.Create(() => new LuckyFishingGameLogic()), "LuckyFishing"));
            _dicGameLogicActors.Add(GAMEID.SpinAndScoreMega, Context.ActorOf(Props.Create(() => new SpinAndScoreMegaGameLogic()), "SpinAndScoreMega"));
            _dicGameLogicActors.Add(GAMEID.MammothGoldMega, Context.ActorOf(Props.Create(() => new MammothGoldMegaGameLogic()), "MammothGoldMega"));
            _dicGameLogicActors.Add(GAMEID.YumYumPowerWays, Context.ActorOf(Props.Create(() => new YumYumPowerWaysGameLogic()), "YumYumPowerWays"));
            _dicGameLogicActors.Add(GAMEID.SecretCityGold, Context.ActorOf(Props.Create(() => new SecretCityGoldGameLogic()), "SecretCityGold"));
            _dicGameLogicActors.Add(GAMEID.FishEye, Context.ActorOf(Props.Create(() => new FishEyeGameLogic()), "FishEye"));
            _dicGameLogicActors.Add(GAMEID.MonsterSuperlanche, Context.ActorOf(Props.Create(() => new MonsterSuperlancheGameLogic()), "MonsterSuperlanche"));
            _dicGameLogicActors.Add(GAMEID.PinupGirls, Context.ActorOf(Props.Create(() => new PinupGirlsGameLogic()), "PinupGirls"));


            _dicGameLogicActors.Add(GAMEID.BigBassBonanzaMega,  Context.ActorOf(Props.Create(() => new BigBassBonanzaMegaGameLogic()), "BigBassBonanzaMega"));
            _dicGameLogicActors.Add(GAMEID.BountyGold,          Context.ActorOf(Props.Create(() => new BountyGoldGameLogic()), "BountyGold"));
            _dicGameLogicActors.Add(GAMEID.MightOfRa,           Context.ActorOf(Props.Create(() => new MightOfRaGameLogic()), "MightOfRa"));
            _dicGameLogicActors.Add(GAMEID.Fire88,              Context.ActorOf(Props.Create(() => new Fire88GameLogic()), "Fire88"));
            _dicGameLogicActors.Add(GAMEID.GreekGods,           Context.ActorOf(Props.Create(() => new GreekGodsGameLogic()), "GreekGods"));
            _dicGameLogicActors.Add(GAMEID.LuckyNewYearTigerTreasures, Context.ActorOf(Props.Create(() => new LuckyNewYearTigerTreasuresGameLogic()), "LuckyNewYearTigerTreasures"));
            _dicGameLogicActors.Add(GAMEID.MasterJoker,         Context.ActorOf(Props.Create(() => new MasterJokerGameLogic()), "MasterJoker"));
            _dicGameLogicActors.Add(GAMEID.WildWildRiches,      Context.ActorOf(Props.Create(() => new WildWildRichesGameLogic()), "WildWildRiches"));
            _dicGameLogicActors.Add(GAMEID.AztecKing,           Context.ActorOf(Props.Create(() => new AztecKingGameLogic()), "AztecKing"));
            _dicGameLogicActors.Add(GAMEID.BullFiesta,          Context.ActorOf(Props.Create(() => new BullFiestaGameLogic()), "BullFiesta"));
            _dicGameLogicActors.Add(GAMEID.DiscoLady,           Context.ActorOf(Props.Create(() => new DiscoLadyGameLogic()), "DiscoLady"));
            _dicGameLogicActors.Add(GAMEID.TicTacTake,          Context.ActorOf(Props.Create(() => new TicTacTakeGameLogic()), "TicTacTake"));
            _dicGameLogicActors.Add(GAMEID.ExtraJuicy,          Context.ActorOf(Props.Create(() => new ExtraJuicyGameLogic()), "ExtraJuicy"));
            _dicGameLogicActors.Add(GAMEID.EyeOfTheStorm,       Context.ActorOf(Props.Create(() => new EyeOfTheStormGameLogic()), "EyeOfTheStorm"));
            _dicGameLogicActors.Add(GAMEID.ChilliHeat,          Context.ActorOf(Props.Create(() => new ChilliHeatGameLogic()), "ChilliHeat"));
            _dicGameLogicActors.Add(GAMEID.BookOfAztecKing,     Context.ActorOf(Props.Create(() => new BookOfAztecKingGameLogic()), "BookOfAztecKing"));
            _dicGameLogicActors.Add(GAMEID.HokkaidoWolf,        Context.ActorOf(Props.Create(() => new HokkaidoWolfGameLogic()), "HokkaidoWolf"));
            _dicGameLogicActors.Add(GAMEID.TreeOfRiches,        Context.ActorOf(Props.Create(() => new TreeOfRichesGameLogic()), "TreeOfRiches"));
            _dicGameLogicActors.Add(GAMEID.BookOfKingdoms,      Context.ActorOf(Props.Create(() => new BookOfKingdomsGameLogic()), "BookOfKingdoms"));
            _dicGameLogicActors.Add(GAMEID.BubblePop,           Context.ActorOf(Props.Create(() => new BubblePopGameLogic()), "BubblePop"));
            _dicGameLogicActors.Add(GAMEID.PyramidKing,         Context.ActorOf(Props.Create(() => new PyramidKingGameLogic()), "PyramidKing"));
            _dicGameLogicActors.Add(GAMEID.GoldenPig,           Context.ActorOf(Props.Create(() => new GoldenPigGameLogic()), "GoldenPig"));
            _dicGameLogicActors.Add(GAMEID.CaishenGold,         Context.ActorOf(Props.Create(() => new CaishenGoldGameLogic()), "CaishenGold"));
            _dicGameLogicActors.Add(GAMEID.PiggyBankBills,      Context.ActorOf(Props.Create(() => new PiggyBankBillsGameLogic()), "PiggyBankBills"));
            _dicGameLogicActors.Add(GAMEID.Super7s,             Context.ActorOf(Props.Create(() => new Super7sGameLogic()), "Super7s"));
            _dicGameLogicActors.Add(GAMEID.LuckyGraceCharm,     Context.ActorOf(Props.Create(() => new LuckyGraceCharmGameLogic()), "LuckyGraceCharm"));
            _dicGameLogicActors.Add(GAMEID.TripleTigers,        Context.ActorOf(Props.Create(() => new TripleTigersGameLogic()), "TripleTigers"));
            _dicGameLogicActors.Add(GAMEID.MysteriousEgypt,     Context.ActorOf(Props.Create(() => new MysteriousEgyptGameLogic()), "MysteriousEgypt"));
            _dicGameLogicActors.Add(GAMEID.MysticChief,         Context.ActorOf(Props.Create(() => new MysticChiefGameLogic()), "MysticChief"));
            _dicGameLogicActors.Add(GAMEID.TheTigerWarrior,     Context.ActorOf(Props.Create(() => new TheTigerWarriorGameLogic()), "TheTigerWarrior"));
            _dicGameLogicActors.Add(GAMEID.DragonTiger,         Context.ActorOf(Props.Create(() => new DragonTigerGameLogic()), "DragonTiger"));
            _dicGameLogicActors.Add(GAMEID.MahjongPanda,        Context.ActorOf(Props.Create(() => new MahjongPandaGameLogic()), "MahjongPanda"));
            _dicGameLogicActors.Add(GAMEID.PeakPower,           Context.ActorOf(Props.Create(() => new PeakPowerGameLogic()), "PeakPower"));
            _dicGameLogicActors.Add(GAMEID.RabbitGarden,        Context.ActorOf(Props.Create(() => new RabbitGardenGameLogic()), "RabbitGarden"));
            _dicGameLogicActors.Add(GAMEID.MoneyMoneyMoney,     Context.ActorOf(Props.Create(() => new MoneyMoneyMoneyGameLogic()), "MoneyMoneyMoney"));
            _dicGameLogicActors.Add(GAMEID.SuperX,              Context.ActorOf(Props.Create(() => new SuperXGameLogic()), "SuperX"));
            _dicGameLogicActors.Add(GAMEID.EmperorCaishen,      Context.ActorOf(Props.Create(() => new EmperorCaishenGameLogic()), "EmperorCaishen"));
            _dicGameLogicActors.Add(GAMEID.Triple8Gold,         Context.ActorOf(Props.Create(() => new Triple8GoldGameLogic()), "Triple8Gold"));
            _dicGameLogicActors.Add(GAMEID.MoneyRoll,           Context.ActorOf(Props.Create(() => new MoneyRollGameLogic()), "MoneyRoll"));
            _dicGameLogicActors.Add(GAMEID.WildWildRichesMega,  Context.ActorOf(Props.Create(() => new WildWildRichesMegaGameLogic()), "WildWildRichesMega"));
            _dicGameLogicActors.Add(GAMEID.IrishCharms,         Context.ActorOf(Props.Create(() => new IrishCharmsGameLogic()), "IrishCharms"));
            _dicGameLogicActors.Add(GAMEID.DiamondsAreForever,  Context.ActorOf(Props.Create(() => new DiamondsAreForeverGameLogic()), "DiamondsAreForever"));
            _dicGameLogicActors.Add(GAMEID.SmugglersCove,       Context.ActorOf(Props.Create(() => new SmugglersCoveGameLogic()), "SmugglersCove"));
            _dicGameLogicActors.Add(GAMEID.HockeyAttack,        Context.ActorOf(Props.Create(() => new HockeyAttackGameLogic()), "HockeyAttack"));
            _dicGameLogicActors.Add(GAMEID.StarPiratesCode,     Context.ActorOf(Props.Create(() => new StarPiratesCodeGameLogic()), "StarPiratesCode"));
            _dicGameLogicActors.Add(GAMEID.DayOfDead,           Context.ActorOf(Props.Create(() => new DayOfDeadGameLogic()), "DayOfDead"));
            _dicGameLogicActors.Add(GAMEID.CashBonanza,         Context.ActorOf(Props.Create(() => new CashBonanzaGameLogic()), "CashBonanza"));
            _dicGameLogicActors.Add(GAMEID.TheMagicCauldron,    Context.ActorOf(Props.Create(() => new TheMagicCauldronGameLogic()), "TheMagicCauldron"));
            _dicGameLogicActors.Add(GAMEID.VoodooMagic,         Context.ActorOf(Props.Create(() => new VoodooMagicGameLogic()), "VoodooMagic"));
            _dicGameLogicActors.Add(GAMEID.JohnHunterAndTheMayanGods, Context.ActorOf(Props.Create(() => new JohnHunterAndTheMayanGodsGameLogic()), "JohnHunterAndTheMayanGods"));
            _dicGameLogicActors.Add(GAMEID.WildWalker,          Context.ActorOf(Props.Create(() => new WildWalkerGameLogic()), "WildWalker"));
            _dicGameLogicActors.Add(GAMEID.FuFuFu,              Context.ActorOf(Props.Create(() => new FuFuFuGameLogic()), "FuFuFu"));
            _dicGameLogicActors.Add(GAMEID.ThreeStarFortune,    Context.ActorOf(Props.Create(() => new ThreeStarFortuneGameLogic()), "ThreeStarFortune"));
            _dicGameLogicActors.Add(GAMEID.LuckyDragonBall,     Context.ActorOf(Props.Create(() => new LuckyDragonBallGameLogic()), "LuckyDragonBall"));
            _dicGameLogicActors.Add(GAMEID.FruitRainbow,        Context.ActorOf(Props.Create(() => new FruitRainbowGameLogic()), "FruitRainbow"));
            _dicGameLogicActors.Add(GAMEID.StarBounty,          Context.ActorOf(Props.Create(() => new StarBountyGameLogic()), "StarBounty"));
            _dicGameLogicActors.Add(GAMEID.MagicJourney,        Context.ActorOf(Props.Create(() => new MagicJourneyGameLogic()), "MagicJourney"));
            _dicGameLogicActors.Add(GAMEID.AladdinAndTheSorcerer, Context.ActorOf(Props.Create(() => new AladdinAndTheSorcererGameLogic()), "AladdinAndTheSorcerer"));
            _dicGameLogicActors.Add(GAMEID.SuperJoker,          Context.ActorOf(Props.Create(() => new SuperJokerGameLogic()), "SuperJoker"));
            _dicGameLogicActors.Add(GAMEID.HotChilli,           Context.ActorOf(Props.Create(() => new HotChilliGameLogic()), "HotChilli"));
            _dicGameLogicActors.Add(GAMEID.JohnHunterAndTheAztecTreasure, Context.ActorOf(Props.Create(() => new JohnHunterAndTheAztecTreasureGameLogic()), "JohnHunterAndTheAztecTreasure"));
            _dicGameLogicActors.Add(GAMEID.WildPixies,          Context.ActorOf(Props.Create(() => new WildPixiesGameLogic()), "WildPixies"));
            _dicGameLogicActors.Add(GAMEID.WildGladiators,      Context.ActorOf(Props.Create(() => new WildGladiatorsGameLogic()), "WildGladiators"));
            _dicGameLogicActors.Add(GAMEID.TreasureHorse,       Context.ActorOf(Props.Create(() => new TreasureHorseGameLogic()), "TreasureHorse"));
            _dicGameLogicActors.Add(GAMEID.SafariKing,          Context.ActorOf(Props.Create(() => new SafariKingGameLogic()), "SafariKing"));
            _dicGameLogicActors.Add(GAMEID.TripleDragons,       Context.ActorOf(Props.Create(() => new TripleDragonsGameLogic()), "TripleDragons"));
            _dicGameLogicActors.Add(GAMEID.MasterChensFortune,  Context.ActorOf(Props.Create(() => new MasterChensFortuneGameLogic()), "MasterChensFortune"));
            _dicGameLogicActors.Add(GAMEID.MadameDestiny,       Context.ActorOf(Props.Create(() => new MadameDestinyGameLogic()), "MadameDestiny"));
            _dicGameLogicActors.Add(GAMEID.AncientEgypt,        Context.ActorOf(Props.Create(() => new AncientEgyptGameLogic()), "AncientEgypt"));
            _dicGameLogicActors.Add(GAMEID.MonkeyMadness,       Context.ActorOf(Props.Create(() => new MonkeyMadnessGameLogic()), "MonkeyMadness"));
            _dicGameLogicActors.Add(GAMEID.GoldRush,            Context.ActorOf(Props.Create(() => new GoldRushGameLogic()), "GoldRush"));
            _dicGameLogicActors.Add(GAMEID.Santa,               Context.ActorOf(Props.Create(() => new SantaGameLogic()), "Santa"));
            _dicGameLogicActors.Add(GAMEID.SevenPiggies,        Context.ActorOf(Props.Create(() => new SevenPiggiesGameLogic()), "SevenPiggies"));
            _dicGameLogicActors.Add(GAMEID.VegasNights,         Context.ActorOf(Props.Create(() => new VegasNightsGameLogic()), "VegasNights"));
            _dicGameLogicActors.Add(GAMEID.ThreeGenieWishes,    Context.ActorOf(Props.Create(() => new ThreeGenieWishesGameLogic()), "ThreeGenieWishes"));
            _dicGameLogicActors.Add(GAMEID.HerculesSonOfZeus,   Context.ActorOf(Props.Create(() => new HerculesSonOfZeusGameLogic()), "HerculesSonOfZeus"));
            _dicGameLogicActors.Add(GAMEID.LuckyDragons,        Context.ActorOf(Props.Create(() => new LuckyDragonsGameLogic()), "LuckyDragons"));
            _dicGameLogicActors.Add(GAMEID.DwarvenGoldDeluxe,   Context.ActorOf(Props.Create(() => new DwarvenGoldDeluxeGameLogic()), "DwarvenGoldDeluxe"));
            _dicGameLogicActors.Add(GAMEID.HotSafari,           Context.ActorOf(Props.Create(() => new HotSafariGameLogic()), "HotSafari"));
            _dicGameLogicActors.Add(GAMEID.SevenMonkeys,        Context.ActorOf(Props.Create(() => new SevenMonkeysGameLogic()), "SevenMonkeys"));
            _dicGameLogicActors.Add(GAMEID.Devils13,            Context.ActorOf(Props.Create(() => new Devils13GameLogic()), "Devils13"));
            _dicGameLogicActors.Add(GAMEID.MysteryOfTheOrient,  Context.ActorOf(Props.Create(() => new MysteryOfTheOrientGameLogic()), "MysteryOfTheOrient"));
            _dicGameLogicActors.Add(GAMEID.WildWestDuels,       Context.ActorOf(Props.Create(() => new WildWestDuelsGameLogic()), "WildWestDuels"));
            _dicGameLogicActors.Add(GAMEID.GreatReef,           Context.ActorOf(Props.Create(() => new GreatReefGameLogic()), "GreatReef"));
            _dicGameLogicActors.Add(GAMEID.EmeraldKing,         Context.ActorOf(Props.Create(() => new EmeraldKingGameLogic()), "EmeraldKing"));
            _dicGameLogicActors.Add(GAMEID.DaVinciTreasure,     Context.ActorOf(Props.Create(() => new DaVinciTreasureGameLogic()), "DaVinciTreasure"));
            _dicGameLogicActors.Add(GAMEID.FishinReels,         Context.ActorOf(Props.Create(() => new FishinReelsGameLogic()), "FishinReels"));
            _dicGameLogicActors.Add(GAMEID.TheWildMachine,      Context.ActorOf(Props.Create(() => new TheWildMachineGameLogic()), "TheWildMachine"));
            _dicGameLogicActors.Add(GAMEID.HoneyHoneyHoney,     Context.ActorOf(Props.Create(() => new HoneyHoneyHoneyGameLogic()), "HoneyHoneyHoney"));
            _dicGameLogicActors.Add(GAMEID.VampiresVSWolves,    Context.ActorOf(Props.Create(() => new VampiresVSWolvesGameLogic()), "VampiresVSWolves"));
            _dicGameLogicActors.Add(GAMEID.PixieWings,          Context.ActorOf(Props.Create(() => new PixieWingsGameLogic()), "PixieWings"));
            _dicGameLogicActors.Add(GAMEID.DragonKingdom,       Context.ActorOf(Props.Create(() => new DragonKingdomGameLogic()), "DragonKingdom"));
            _dicGameLogicActors.Add(GAMEID.JourneyToTheWest,    Context.ActorOf(Props.Create(() => new JourneyToTheWestGameLogic()), "JourneyToTheWest"));
            _dicGameLogicActors.Add(GAMEID.StreetRacer,         Context.ActorOf(Props.Create(() => new StreetRacerGameLogic()), "StreetRacer"));
            _dicGameLogicActors.Add(GAMEID.RiseOfSamurai,       Context.ActorOf(Props.Create(() => new RiseOfSamuraiGameLogic()), "RiseOfSamurai"));
            _dicGameLogicActors.Add(GAMEID.LadyGodiva,          Context.ActorOf(Props.Create(() => new LadyGodivaGameLogic()), "LadyGodiva"));
            _dicGameLogicActors.Add(GAMEID.MightyKong,          Context.ActorOf(Props.Create(() => new MightyKongGameLogic()), "MightyKong"));
            _dicGameLogicActors.Add(GAMEID.FairytaleFortune,    Context.ActorOf(Props.Create(() => new FairytaleFortuneGameLogic()), "FairytaleFortune"));
            _dicGameLogicActors.Add(GAMEID.WildSpells,          Context.ActorOf(Props.Create(() => new WildSpellsGameLogic()), "WildSpells"));
            _dicGameLogicActors.Add(GAMEID.TheGreatChickenEscape, Context.ActorOf(Props.Create(() => new TheGreatChickenEscapeGameLogic()), "TheGreatChickenEscape"));

            _dicGameLogicActors.Add(GAMEID.WildBisonCharge,         Context.ActorOf(Props.Create(() => new WildBisonChargeGameLogic()), "WildBisonCharge"));
            _dicGameLogicActors.Add(GAMEID.JewelRush,               Context.ActorOf(Props.Create(() => new JewelRushGameLogic()), "JewelRush"));
            _dicGameLogicActors.Add(GAMEID.ExcaliburUnleashed,      Context.ActorOf(Props.Create(() => new ExcaliburUnleashedGameLogic()), "ExcaliburUnleashed"));
            _dicGameLogicActors.Add(GAMEID.KingdomOfTheDead,        Context.ActorOf(Props.Create(() => new KingdomOfTheDeadGameLogic()), "KingdomOfTheDead"));
            _dicGameLogicActors.Add(GAMEID.LampOfInfinity,          Context.ActorOf(Props.Create(() => new LampOfInfinityGameLogic()), "LampOfInfinity"));
            _dicGameLogicActors.Add(GAMEID.KnightHotSpotz,          Context.ActorOf(Props.Create(() => new KnightHotSpotzGameLogic()), "KnightHotSpotz"));
            _dicGameLogicActors.Add(GAMEID.ZeusVsHadesGodsOfWar,    Context.ActorOf(Props.Create(() => new ZeusVsHadesGodsOfWarGameLogic()), "ZeusVsHadesGodsOfWar"));
            _dicGameLogicActors.Add(GAMEID.DiamondsOfEgypt,         Context.ActorOf(Props.Create(() => new DiamondsOfEgyptGameLogic()), "DiamondsOfEgypt"));
            _dicGameLogicActors.Add(GAMEID.StickyBees,              Context.ActorOf(Props.Create(() => new StickyBeesGameLogic()), "StickyBees"));
            _dicGameLogicActors.Add(GAMEID.PiratesPub,              Context.ActorOf(Props.Create(() => new PiratesPubGameLogic()), "PiratesPub"));
            _dicGameLogicActors.Add(GAMEID.FloatingDragonDragonBoatFestival, Context.ActorOf(Props.Create(() => new FloatingDragonDragonBoatFestivalGameLogic()), "FloatingDragonDragonBoatFestival"));
            _dicGameLogicActors.Add(GAMEID.BigBassAmazonXtreme,     Context.ActorOf(Props.Create(() => new BigBassAmazonXtremeGameLogic()), "BigBassAmazonXtreme"));
            _dicGameLogicActors.Add(GAMEID.FatPanda,                Context.ActorOf(Props.Create(() => new FatPandaGameLogic()), "FatPanda"));
            _dicGameLogicActors.Add(GAMEID.WisdomOfAthena,          Context.ActorOf(Props.Create(() => new WisdomOfAthenaGameLogic()), "WisdomOfAthena"));
            _dicGameLogicActors.Add(GAMEID.SpellbindingMystery,     Context.ActorOf(Props.Create(() => new SpellbindingMysteryGameLogic()), "SpellbindingMystery"));
            _dicGameLogicActors.Add(GAMEID.HeistForTheGoldenNuggets, Context.ActorOf(Props.Create(() => new HeistForTheGoldenNuggetsGameLogic()), "HeistForTheGoldenNuggets"));
            _dicGameLogicActors.Add(GAMEID.JasmineDreams,           Context.ActorOf(Props.Create(() => new JasmineDreamsGameLogic()), "JasmineDreams"));
            _dicGameLogicActors.Add(GAMEID.CowboyCoins,             Context.ActorOf(Props.Create(() => new CowboyCoinsGameLogic()), "CowboyCoins"));
            _dicGameLogicActors.Add(GAMEID.FrogsAndBugs,            Context.ActorOf(Props.Create(() => new FrogsAndBugsGameLogic()), "FrogsAndBugs"));
            _dicGameLogicActors.Add(GAMEID.RobberStrike,            Context.ActorOf(Props.Create(() => new RobberStrikeGameLogic()), "RobberStrike"));
            _dicGameLogicActors.Add(GAMEID.CashBox,                 Context.ActorOf(Props.Create(() => new CashBoxGameLogic()), "CashBox"));
            _dicGameLogicActors.Add(GAMEID.HellvisWild,             Context.ActorOf(Props.Create(() => new HellvisWildGameLogic()), "HellvisWild"));
            _dicGameLogicActors.Add(GAMEID.LobsterBobsCrazyCrabShack, Context.ActorOf(Props.Create(() => new LobsterBobsCrazyCrabShackGameLogic()), "LobsterBobsCrazyCrabShack"));
            _dicGameLogicActors.Add(GAMEID.DiamondCascade,          Context.ActorOf(Props.Create(() => new DiamondCascadeGameLogic()), "DiamondCascade"));
            _dicGameLogicActors.Add(GAMEID.SkyBounty,               Context.ActorOf(Props.Create(() => new SkyBountyGameLogic()), "SkyBounty"));
            _dicGameLogicActors.Add(GAMEID.PiggyBankers,            Context.ActorOf(Props.Create(() => new PiggyBankersGameLogic()), "PiggyBankers"));
            _dicGameLogicActors.Add(GAMEID.PubKings,                Context.ActorOf(Props.Create(() => new PubKingsGameLogic()), "PubKings"));
            _dicGameLogicActors.Add(GAMEID.ForgeOfOlympus,          Context.ActorOf(Props.Create(() => new ForgeOfOlympusGameLogic()), "ForgeOfOlympus"));
            _dicGameLogicActors.Add(GAMEID.MustangTrail,            Context.ActorOf(Props.Create(() => new MustangTrailGameLogic()), "MustangTrail"));
            _dicGameLogicActors.Add(GAMEID.CandyBlitz,              Context.ActorOf(Props.Create(() => new CandyBlitzGameLogic()), "CandyBlitz"));
            _dicGameLogicActors.Add(GAMEID.BigBassHoldAndSpinnerMegaways, Context.ActorOf(Props.Create(() => new BigBassHoldAndSpinnerMegawaysGameLogic()), "BigBassHoldAndSpinnerMegaways"));
            _dicGameLogicActors.Add(GAMEID.CyclopsSmash,            Context.ActorOf(Props.Create(() => new CyclopsSmashGameLogic()), "CyclopsSmash"));
            _dicGameLogicActors.Add(GAMEID.FrozenTropics,           Context.ActorOf(Props.Create(() => new FrozenTropicsGameLogic()), "FrozenTropics"));
            _dicGameLogicActors.Add(GAMEID.BookOfTutMegaways,       Context.ActorOf(Props.Create(() => new BookOfTutMegawaysGameLogic()), "BookOfTutMegaways"));
            _dicGameLogicActors.Add(GAMEID.FortunesOfAztec,         Context.ActorOf(Props.Create(() => new FortunesOfAztecGameLogic()), "FortunesOfAztec"));
            _dicGameLogicActors.Add(GAMEID.EightGoldenDragonChallenge, Context.ActorOf(Props.Create(() => new EightGoldenDragonChallengeGameLogic()), "EightGoldenDragonChallenge"));
            _dicGameLogicActors.Add(GAMEID.GoldOasis,               Context.ActorOf(Props.Create(() => new GoldOasisGameLogic()), "GoldOasis"));
            _dicGameLogicActors.Add(GAMEID.PowerOfMerlinMegaways,   Context.ActorOf(Props.Create(() => new PowerOfMerlinMegawaysGameLogic()), "PowerOfMerlinMegaways"));
            _dicGameLogicActors.Add(GAMEID.ThreeBuzzingWilds,       Context.ActorOf(Props.Create(() => new ThreeBuzzingWildsGameLogic()), "ThreeBuzzingWilds"));
            _dicGameLogicActors.Add(GAMEID.RocketBlastMegaways,     Context.ActorOf(Props.Create(() => new RocketBlastMegawaysGameLogic()), "RocketBlastMegaways"));

            _dicGameLogicActors.Add(GAMEID.RainbowReels,            Context.ActorOf(Props.Create(() => new RainbowReelsGameLogic()),             "RainbowReels"));
            _dicGameLogicActors.Add(GAMEID.GravityBonanza,          Context.ActorOf(Props.Create(() => new GravityBonanzaGameLogic()),           "GravityBonanza"));
            _dicGameLogicActors.Add(GAMEID.TwilightPrincess,        Context.ActorOf(Props.Create(() => new TwilightPrincessGameLogic()),         "TwilightPrincess"));
            _dicGameLogicActors.Add(GAMEID.DemonPots,               Context.ActorOf(Props.Create(() => new DemonPotsGameLogic()),                "DemonPots"));
            _dicGameLogicActors.Add(GAMEID.CashChips,               Context.ActorOf(Props.Create(() => new CashChipsGameLogic()),                "CashChips"));
            _dicGameLogicActors.Add(GAMEID.TundrasFortune,          Context.ActorOf(Props.Create(() => new TundrasFortuneGameLogic()),           "TundrasFortune"));
            _dicGameLogicActors.Add(GAMEID.InfectiveWild,           Context.ActorOf(Props.Create(() => new InfectiveWildGameLogic()),            "InfectiveWild"));
            _dicGameLogicActors.Add(GAMEID.BigBassHalloween,        Context.ActorOf(Props.Create(() => new BigBassHalloweenGameLogic()),         "BigBassHalloween"));
            _dicGameLogicActors.Add(GAMEID.TheMoneyMenMegaways,     Context.ActorOf(Props.Create(() => new TheMoneyMenMegawaysGameLogic()),      "TheMoneyMenMegaways"));
            _dicGameLogicActors.Add(GAMEID.StarlightPrincess1000,   Context.ActorOf(Props.Create(() => new StarlightPrincess1000GameLogic()),    "StarlightPrincess1000"));
            _dicGameLogicActors.Add(GAMEID.VikingForge,             Context.ActorOf(Props.Create(() => new VikingForgeGameLogic()),              "VikingForge"));

            _dicGameLogicActors.Add(GAMEID.HappyHooves,             Context.ActorOf(Props.Create(() => new HappyHoovesGameLogic()), "HappyHooves"));
            _dicGameLogicActors.Add(GAMEID.PantherQueen,            Context.ActorOf(Props.Create(() => new PantherQueenGameLogic()), "PantherQueen"));
            _dicGameLogicActors.Add(GAMEID.QueenOfAtlantis,         Context.ActorOf(Props.Create(() => new QueenOfAtlantisGameLogic()), "QueenOfAtlantis"));
            _dicGameLogicActors.Add(GAMEID.TheCatfatherPartII,      Context.ActorOf(Props.Create(() => new TheCatfatherPartIIGameLogic()), "TheCatfatherPartII"));
            _dicGameLogicActors.Add(GAMEID.RomeoAndJuliet,          Context.ActorOf(Props.Create(() => new RomeoAndJulietGameLogic()), "RomeoAndJuliet"));
            _dicGameLogicActors.Add(GAMEID.HockeyLeagueWildMatch,   Context.ActorOf(Props.Create(() => new HockeyLeagueWildMatchGameLogic()), "HockeyLeagueWildMatch"));
            _dicGameLogicActors.Add(GAMEID.TheCatfather,            Context.ActorOf(Props.Create(() => new TheCatfatherGameLogic()), "TheCatfather"));
            _dicGameLogicActors.Add(GAMEID.MagicCrystals,           Context.ActorOf(Props.Create(() => new MagicCrystalsGameLogic()), "MagicCrystals"));
            _dicGameLogicActors.Add(GAMEID.HockeyLeague,            Context.ActorOf(Props.Create(() => new HockeyLeagueGameLogic()), "HockeyLeague"));
            _dicGameLogicActors.Add(GAMEID.LadyOfTheMoon,           Context.ActorOf(Props.Create(() => new LadyOfTheMoonGameLogic()), "LadyOfTheMoon"));
            _dicGameLogicActors.Add(GAMEID.JokersJewelDice,         Context.ActorOf(Props.Create(() => new JokersJewelDiceGameLogic()), "JokersJewelDice"));
            _dicGameLogicActors.Add(GAMEID.SweetBonanzaDice,        Context.ActorOf(Props.Create(() => new SweetBonanzaDiceGameLogic()), "SweetBonanzaDice"));
            _dicGameLogicActors.Add(GAMEID.TalesOfEgypt,            Context.ActorOf(Props.Create(() => new TalesOfEgyptGameLogic()), "TalesOfEgypt"));
            _dicGameLogicActors.Add(GAMEID.TheDogHouseDiceShow,     Context.ActorOf(Props.Create(() => new TheDogHouseDiceShowGameLogic()), "TheDogHouseDiceShow"));
            _dicGameLogicActors.Add(GAMEID.DwarvenGold,             Context.ActorOf(Props.Create(() => new DwarvenGoldGameLogic()), "DwarvenGold"));
            _dicGameLogicActors.Add(GAMEID.GloriousRome,            Context.ActorOf(Props.Create(() => new GloriousRomeGameLogic()), "GloriousRome"));
            _dicGameLogicActors.Add(GAMEID.TripleJokers,            Context.ActorOf(Props.Create(() => new TripleJokersGameLogic()), "TripleJokers"));
            _dicGameLogicActors.Add(GAMEID.JurassicGiants,          Context.ActorOf(Props.Create(() => new JurassicGiantsGameLogic()), "JurassicGiants"));
            _dicGameLogicActors.Add(GAMEID.ThreeKingdomsBattleOfRedCliffs, Context.ActorOf(Props.Create(() => new ThreeKingdomsBattleOfRedCliffsGameLogic()), "ThreeKingdomsBattleOfRedCliffs"));
            _dicGameLogicActors.Add(GAMEID.AladdinsTreasure,        Context.ActorOf(Props.Create(() => new AladdinsTreasureGameLogic()), "AladdinsTreasure"));
            _dicGameLogicActors.Add(GAMEID.CountryFarming,          Context.ActorOf(Props.Create(() => new CountryFarmingGameLogic()), "CountryFarming"));
            _dicGameLogicActors.Add(GAMEID.SugarSupremePowernudge,  Context.ActorOf(Props.Create(() => new SugarSupremePowernudgeGameLogic()), "SugarSupremePowernudge"));
            _dicGameLogicActors.Add(GAMEID.WildDepths,              Context.ActorOf(Props.Create(() => new WildDepthsGameLogic()), "WildDepths"));
            _dicGameLogicActors.Add(GAMEID.BookOfVikings,           Context.ActorOf(Props.Create(() => new BookOfVikingsGameLogic()), "BookOfVikings"));
            _dicGameLogicActors.Add(GAMEID.TimberStacks,            Context.ActorOf(Props.Create(() => new TimberStacksGameLogic()), "TimberStacks"));
            _dicGameLogicActors.Add(GAMEID.ChaseForGlory,            Context.ActorOf(Props.Create(() => new ChaseForGloryGameLogic()),   "ChaseForGlory"));
            _dicGameLogicActors.Add(GAMEID.NileFortune,              Context.ActorOf(Props.Create(() => new NileFortuneGameLogic()),     "NileFortune")); ;

            _dicGameLogicActors.Add(GAMEID.TheWildGang,              Context.ActorOf(Props.Create(() => new TheWildGangGameLogic()), "TheWildGang"));
            _dicGameLogicActors.Add(GAMEID.BigBassChristmasBash,     Context.ActorOf(Props.Create(() => new BigBassChristmasBashGameLogic()), "BigBassChristmasBash"));
            _dicGameLogicActors.Add(GAMEID.FiveFrozenCharmsMegaways, Context.ActorOf(Props.Create(() => new FiveFrozenCharmsMegawaysGameLogic()), "FiveFrozenCharmsMegaways"));
            _dicGameLogicActors.Add(GAMEID.SugarRushXmas,            Context.ActorOf(Props.Create(() => new SugarRushXmasGameLogic()), "SugarRushXmas"));
            _dicGameLogicActors.Add(GAMEID.DingDongChristmasBells,   Context.ActorOf(Props.Create(() => new DingDongChristmasBellsGameLogic()), "DingDongChristmasBells"));
            _dicGameLogicActors.Add(GAMEID.CandyJarClusters,         Context.ActorOf(Props.Create(() => new CandyJarClustersGameLogic()), "CandyJarClusters"));
            _dicGameLogicActors.Add(GAMEID.FloatingDragonNewYearFestivalUltraMegawaysHoldAndSpin, Context.ActorOf(Props.Create(() => new FloatingDragonNewYearFestivalUltraMegawaysHoldAndSpinGameLogic()), "FloatingDragonNewYearFestivalUltraMegaways"));
            _dicGameLogicActors.Add(GAMEID.FireStampede,             Context.ActorOf(Props.Create(() => new FireStampedeGameLogic()), "FireStampede"));
            _dicGameLogicActors.Add(GAMEID.JuicyFruitsMultihold,     Context.ActorOf(Props.Create(() => new JuicyFruitsMultiholdGameLogic()), "JuicyFruitsMultihold"));
            _dicGameLogicActors.Add(GAMEID.GatesOfOlympus1000,       Context.ActorOf(Props.Create(() => new GatesOfOlympus1000GameLogic()), "GatesOfOlympus1000"));
            _dicGameLogicActors.Add(GAMEID.BladeAndFangs,            Context.ActorOf(Props.Create(() => new BladeAndFangsGameLogic()),  "BladeAndFangs"));
            _dicGameLogicActors.Add(GAMEID.TheBigDawgs,              Context.ActorOf(Props.Create(() => new TheBigDawgsGameLogic()),    "TheBigDawgs"));
            _dicGameLogicActors.Add(GAMEID.CastleOfFire,             Context.ActorOf(Props.Create(() => new CastleOfFireGameLogic()),   "CastleOfFire"));
            _dicGameLogicActors.Add(GAMEID.BlazingWildsMegaways,    Context.ActorOf(Props.Create(() => new BlazingWildsMegawaysGameLogic()),    "BlazingWildsMegaways"));
            _dicGameLogicActors.Add(GAMEID.GoodLuckAndGoodFortune,  Context.ActorOf(Props.Create(() => new GoodLuckAndGoodFortuneGameLogic()),  "GoodLuckAndGoodFortune"));
            _dicGameLogicActors.Add(GAMEID.YearOfTheDragonKing,     Context.ActorOf(Props.Create(() => new YearOfTheDragonKingGameLogic()),     "YearOfTheDragonKing"));
            _dicGameLogicActors.Add(GAMEID.LokisRiches,             Context.ActorOf(Props.Create(() => new LokisRichesGameLogic()),     "LokisRiches"));
            _dicGameLogicActors.Add(GAMEID.RedHotLuck,              Context.ActorOf(Props.Create(() => new RedHotLuckGameLogic()),      "RedHotLuck"));
            _dicGameLogicActors.Add(GAMEID.TreesOfTreasure,         Context.ActorOf(Props.Create(() => new TreesOfTreasureGameLogic()), "TreesOfTreasure"));
            _dicGameLogicActors.Add(GAMEID.TheAlterEgo,             Context.ActorOf(Props.Create(() => new TheAlterEgoGameLogic()),     "TheAlterEgo"));
            _dicGameLogicActors.Add(GAMEID.BigBassFloatsMyBoat,      Context.ActorOf(Props.Create(() => new BigBassFloatsMyBoatGameLogic()), "BigBassFloatsMyBoat"));
            _dicGameLogicActors.Add(GAMEID.PompeiiMegareelsMegaways, Context.ActorOf(Props.Create(() => new PompeiiMegareelsMegawaysGameLogic()), "PompeiiMegareelsMegaways"));
            _dicGameLogicActors.Add(GAMEID.StrawberryCocktail,       Context.ActorOf(Props.Create(() => new StrawberryCocktailGameLogic()),          "StrawberryCocktail"));
            _dicGameLogicActors.Add(GAMEID.MightyMunchingMelons,     Context.ActorOf(Props.Create(() => new MightyMunchingMelonsGameLogic()),        "MightyMunchingMelons"));
            _dicGameLogicActors.Add(GAMEID.WheelOGold,               Context.ActorOf(Props.Create(() => new WheelOGoldGameLogic()),                  "WheelOGold"));
            _dicGameLogicActors.Add(GAMEID.GearsOfHorus,             Context.ActorOf(Props.Create(() => new GearsOfHorusGameLogic()),                "GearsOfHorus"));
            _dicGameLogicActors.Add(GAMEID.BigBassDayAtTheRaces,     Context.ActorOf(Props.Create(() => new BigBassDayAtTheRacesGameLogic()),        "BigBassDayAtTheRaces"));
            _dicGameLogicActors.Add(GAMEID.PotOfFortune,            Context.ActorOf(Props.Create(() => new PotOfFortuneGameLogic()),            "PotOfFortune"));
            _dicGameLogicActors.Add(GAMEID.SugarRush1000,           Context.ActorOf(Props.Create(() => new SugarRush1000GameLogic()),           "SugarRush1000"));
            _dicGameLogicActors.Add(GAMEID.FirePortals,             Context.ActorOf(Props.Create(() => new FirePortalsGameLogic()),             "FirePortals"));
            _dicGameLogicActors.Add(GAMEID.BigBurgerLoad,           Context.ActorOf(Props.Create(() => new BigBurgerLoadGameLogic()),           "BigBurgerLoad"));
            _dicGameLogicActors.Add(GAMEID.TheDogHouseDogOrAlive,   Context.ActorOf(Props.Create(() => new TheDogHouseDogOrAliveGameLogic()),   "TheDogHouseDogOrAlive"));
            _dicGameLogicActors.Add(GAMEID.RipeRewards,             Context.ActorOf(Props.Create(() => new RipeRewardsGameLogic()),             "RipeRewards"));
            _dicGameLogicActors.Add(GAMEID.OViralataCaramelo,       Context.ActorOf(Props.Create(() => new OViralataCarameloGameLogic()),       "OViralataCaramelo"));
            _dicGameLogicActors.Add(GAMEID.AztecPowernudge,                 Context.ActorOf(Props.Create(() => new AztecPowernudgeGameLogic()),                 "AztecPowernudge"));
            _dicGameLogicActors.Add(GAMEID.BigBassSecretsOfTheGoldenLake,   Context.ActorOf(Props.Create(() => new BigBassSecretsOfTheGoldenLakeGameLogic()),   "BigBassSecretsOfTheGoldenLake"));
            _dicGameLogicActors.Add(GAMEID.IceLobster,                      Context.ActorOf(Props.Create(() => new IceLobsterGameLogic()),                      "IceLobster"));
            _dicGameLogicActors.Add(GAMEID.BarnyardMegahaysMegaways,        Context.ActorOf(Props.Create(() => new BarnyardMegahaysMegawaysGameLogic()),        "BarnyardMegahaysMegaways"));
            _dicGameLogicActors.Add(GAMEID.LobsterBobsSeaFoodAndWinIt,  Context.ActorOf(Props.Create(() => new LobsterBobsSeaFoodAndWinItGameLogic()),  "LobsterBobsSeaFoodAndWinIt"));
            _dicGameLogicActors.Add(GAMEID.ReleaseTheBison,             Context.ActorOf(Props.Create(() => new ReleaseTheBisonGameLogic()),             "ReleaseTheBison"));
            _dicGameLogicActors.Add(GAMEID.CandyBlitzBombs,             Context.ActorOf(Props.Create(() => new CandyBlitzBombsGameLogic()),             "CandyBlitzBombs"));
            _dicGameLogicActors.Add(GAMEID.FruityTreats,                Context.ActorOf(Props.Create(() => new FruityTreatsGameLogic()),                "FruityTreats"));
            _dicGameLogicActors.Add(GAMEID.HeartOfCleopatra,            Context.ActorOf(Props.Create(() => new HeartOfCleopatraGameLogic()),            "HeartOfCleopatra"));
            _dicGameLogicActors.Add(GAMEID.FrontRunnerOddsOn,           Context.ActorOf(Props.Create(() => new FrontRunnerOddsOnGameLogic()),           "FrontRunnerOddsOn"));
            _dicGameLogicActors.Add(GAMEID.DwarfAndDragon,              Context.ActorOf(Props.Create(() => new DwarfAndDragonGameLogic()),              "DwarfAndDragon"));
            _dicGameLogicActors.Add(GAMEID.RiseOfPyramids,              Context.ActorOf(Props.Create(() => new RiseOfPyramidsGameLogic()),              "RiseOfPyramids"));
            _dicGameLogicActors.Add(GAMEID.HeroicSpins,                 Context.ActorOf(Props.Create(() => new HeroicSpinsGameLogic()),                 "HeroicSpins"));
            _dicGameLogicActors.Add(GAMEID.StarlightPrincessPachi,      Context.ActorOf(Props.Create(() => new StarlightPrincessPachiGameLogic()),      "StarlightPrincessPachi"));
            _dicGameLogicActors.Add(GAMEID.BigBassBonanzaReelAction,    Context.ActorOf(Props.Create(() => new BigBassBonanzaReelActionGameLogic()),    "BigBassBonanzaReelAction"));
            _dicGameLogicActors.Add(GAMEID.SweetBonanza1000,            Context.ActorOf(Props.Create(() => new SweetBonanza1000GameLogic()),            "SweetBonanza1000"));
            _dicGameLogicActors.Add(GAMEID.Wildies,                     Context.ActorOf(Props.Create(() => new WildiesGameLogic()),                     "Wildies"));
            _dicGameLogicActors.Add(GAMEID.Devilicious,                 Context.ActorOf(Props.Create(() => new DeviliciousGameLogic()),                 "Devilicious"));
            _dicGameLogicActors.Add(GAMEID.MedusasStone,                Context.ActorOf(Props.Create(() => new MedusasStoneGameLogic()),                "MedusasStone"));
            _dicGameLogicActors.Add(GAMEID.BigBassMissionFishin,        Context.ActorOf(Props.Create(() => new BigBassMissionFishinGameLogic()),        "BigBassMissionFishin"));
            _dicGameLogicActors.Add(GAMEID.BuffaloKingUntamedMega,      Context.ActorOf(Props.Create(() => new BuffaloKingUntamedMegaGameLogic()),      "BuffaloKingUntamedMega"));

            _dicGameLogicActors.Add(GAMEID.HotToBurnMultiplier,         Context.ActorOf(Props.Create(() => new HotToBurnMultiplierGameLogic()),         "HotToBurnMultiplier"));
            _dicGameLogicActors.Add(GAMEID.HandOfMidas2,                Context.ActorOf(Props.Create(() => new HandOfMidas2GameLogic()),                "HandOfMidas2"));
            _dicGameLogicActors.Add(GAMEID.SweetKingdom,                Context.ActorOf(Props.Create(() => new SweetKingdomGameLogic()),                "SweetKingdom"));
            _dicGameLogicActors.Add(GAMEID.CrankItUp,                   Context.ActorOf(Props.Create(() => new CrankItUpGameLogic()),                   "CrankItUp"));
            _dicGameLogicActors.Add(GAMEID.SamuraiCode,                 Context.ActorOf(Props.Create(() => new SamuraiCodeGameLogic()),                 "SamuraiCode"));
            _dicGameLogicActors.Add(GAMEID.JokersJewelsWild,            Context.ActorOf(Props.Create(() => new JokersJewelsWildGameLogic()),            "JokersJewelsWild"));
            _dicGameLogicActors.Add(GAMEID.DynamiteDigginDoug,          Context.ActorOf(Props.Create(() => new DynamiteDigginDougGameLogic()),          "DynamiteDigginDoug"));
            _dicGameLogicActors.Add(GAMEID.JackpotHunter,               Context.ActorOf(Props.Create(() => new JackpotHunterGameLogic()),               "JackpotHunter"));
            _dicGameLogicActors.Add(GAMEID.BowOfArtemis,                Context.ActorOf(Props.Create(() => new BowOfArtemisGameLogic()),                "BowOfArtemis"));
            _dicGameLogicActors.Add(GAMEID.DragonGold88,                Context.ActorOf(Props.Create(() => new DragonGold88GameLogic()),                "DragonGold88"));
            _dicGameLogicActors.Add(GAMEID.RunningSushi,                Context.ActorOf(Props.Create(() => new RunningSushiGameLogic()),                "RunningSushi"));
            _dicGameLogicActors.Add(GAMEID.TheConqueror,                Context.ActorOf(Props.Create(() => new TheConquerorGameLogic()),                "TheConqueror"));
            _dicGameLogicActors.Add(GAMEID.SarayRuyasi,                 Context.ActorOf(Props.Create(() => new SarayRuyasiGameLogic()),                 "SarayRuyasi"));
            
            _dicGameLogicActors.Add(GAMEID.AngelVSSinner,               Context.ActorOf(Props.Create(() => new AngelVSSinnerGameLogic()),               "AngelVSSinner"));
            _dicGameLogicActors.Add(GAMEID.AztecTreasureHunt,           Context.ActorOf(Props.Create(() => new AztecTreasureHuntGameLogic()),           "AztecTreasureHunt"));
            _dicGameLogicActors.Add(GAMEID.BigBassDoubleDownDeluxe,     Context.ActorOf(Props.Create(() => new BigBassDoubleDownDeluxeGameLogic()),     "BigBassDoubleDownDeluxe"));
            _dicGameLogicActors.Add(GAMEID.ForgingWilds,                Context.ActorOf(Props.Create(() => new ForgingWildsGameLogic()),                "ForgingWilds"));
            _dicGameLogicActors.Add(GAMEID.GemElevator,                 Context.ActorOf(Props.Create(() => new GemElevatorGameLogic()),                 "GemElevator"));
            _dicGameLogicActors.Add(GAMEID.HotToBurn7Deadly,            Context.ActorOf(Props.Create(() => new HotToBurn7DeadlyGameLogic()),            "HotToBurn7Deadly"));
            _dicGameLogicActors.Add(GAMEID.MysteryMice,                 Context.ActorOf(Props.Create(() => new MysteryMiceGameLogic()),                 "MysteryMice"));
            _dicGameLogicActors.Add(GAMEID.SumoSupremeMega,             Context.ActorOf(Props.Create(() => new SumoSupremeMegaGameLogic()),             "SumoSupremeMega"));
            _dicGameLogicActors.Add(GAMEID.SweetArgentina,              Context.ActorOf(Props.Create(() => new SweetArgentinaGameLogic()),              "SweetArgentina"));
            _dicGameLogicActors.Add(GAMEID.YetiQuest,                   Context.ActorOf(Props.Create(() => new YetiQuestGameLogic()),                   "YetiQuest"));

            _dicGameLogicActors.Add(GAMEID.BadgeBlitz,                  Context.ActorOf(Props.Create(() => new BadgeBlitzGameLogic()),                  "BadgeBlitz"));
            _dicGameLogicActors.Add(GAMEID.CandyCorner,                 Context.ActorOf(Props.Create(() => new CandyCornerGameLogic()),                 "CandyCorner"));
            _dicGameLogicActors.Add(GAMEID.Moleionaire,                 Context.ActorOf(Props.Create(() => new MoleionaireGameLogic()),                 "Moleionaire"));
            _dicGameLogicActors.Add(GAMEID.HimalayanWild,               Context.ActorOf(Props.Create(() => new HimalayanWildGameLogic()),               "HimalayanWild"));
            _dicGameLogicActors.Add(GAMEID.JokersJewelsHot,             Context.ActorOf(Props.Create(() => new JokersJewelsHotGameLogic()),             "JokersJewelsHot"));
            _dicGameLogicActors.Add(GAMEID.OodlesOfNoodles,             Context.ActorOf(Props.Create(() => new OodlesOfNoodlesGameLogic()),             "OodlesOfNoodles"));
            _dicGameLogicActors.Add(GAMEID.FortuneHitnRoll,             Context.ActorOf(Props.Create(() => new FortuneHitnRollGameLogic()),             "FortuneHitnRoll"));
            _dicGameLogicActors.Add(GAMEID.WolfGoldUltimate,            Context.ActorOf(Props.Create(() => new WolfGoldUltimateGameLogic()),            "WolfGoldUltimate"));
            _dicGameLogicActors.Add(GAMEID.FangtasticFreespins,         Context.ActorOf(Props.Create(() => new FangtasticFreespinsGameLogic()),         "FangtasticFreespins"));
            _dicGameLogicActors.Add(GAMEID.BigBassHalloween2,           Context.ActorOf(Props.Create(() => new BigBassHalloween2GameLogic()),           "BigBassHalloween2"));
            _dicGameLogicActors.Add(GAMEID.WisdomOfAthena1000,          Context.ActorOf(Props.Create(() => new WisdomOfAthena1000GameLogic()),          "WisdomOfAthena1000"));
            _dicGameLogicActors.Add(GAMEID.SevenCloversOfFortune,       Context.ActorOf(Props.Create(() => new SevenCloversOfFortuneGameLogic()),       "SevenCloversOfFortune"));
            _dicGameLogicActors.Add(GAMEID.TheDogHouseMuttleyCrew,      Context.ActorOf(Props.Create(() => new TheDogHouseMuttleyCrewGameLogic()),      "TheDogHouseMuttleyCrew"));
            _dicGameLogicActors.Add(GAMEID.ReleaseTheKrakenMega,        Context.ActorOf(Props.Create(() => new ReleaseTheKrakenMegaGameLogic()),        "ReleaseTheKrakenMega"));
            _dicGameLogicActors.Add(GAMEID.TigreSortudo,                Context.ActorOf(Props.Create(() => new TigreSortudoGameLogic()),                "TigreSortudo"));

            _dicGameLogicActors.Add(GAMEID.SixJokers,                   Context.ActorOf(Props.Create(() => new SixJokersGameLogic()),                   "SixJokers"));
            _dicGameLogicActors.Add(GAMEID.MightOfFreyaMega,            Context.ActorOf(Props.Create(() => new MightOfFreyaMegaGameLogic()),            "MightOfFreyaMega"));
            _dicGameLogicActors.Add(GAMEID.PenguinsChristmasPartyTime,  Context.ActorOf(Props.Create(() => new PenguinsChristmasPartyTimeGameLogic()),  "PenguinsChristmasPartyTime"));
            _dicGameLogicActors.Add(GAMEID.TinyToads,                   Context.ActorOf(Props.Create(() => new TinyToadsGameLogic()),                   "TinyToads"));
            _dicGameLogicActors.Add(GAMEID.DragonKingHotPots,           Context.ActorOf(Props.Create(() => new DragonKingHotPotsGameLogic()),           "DragonKingHotPots"));
            _dicGameLogicActors.Add(GAMEID.MiningRush,                  Context.ActorOf(Props.Create(() => new MiningRushGameLogic()),                  "MiningRush"));
            _dicGameLogicActors.Add(GAMEID.BigBassBonanza3Reeler,       Context.ActorOf(Props.Create(() => new BigBassBonanza3ReelerGameLogic()),       "BigBassBonanza3Reeler"));
            _dicGameLogicActors.Add(GAMEID.EternalEmpressFreezeTime,    Context.ActorOf(Props.Create(() => new EternalEmpressFreezeTimeGameLogic()),    "EternalEmpressFreezeTime"));
            _dicGameLogicActors.Add(GAMEID.BigBassXmasExtreme,          Context.ActorOf(Props.Create(() => new BigBassXmasExtremeGameLogic()),          "BigBassXmasExtreme"));
            _dicGameLogicActors.Add(GAMEID.GatesOfOlympusXmas1000,      Context.ActorOf(Props.Create(() => new GatesOfOlympusXmas1000GameLogic()),      "GatesOfOlympusXmas1000"));
            _dicGameLogicActors.Add(GAMEID.SantasXmasRush,              Context.ActorOf(Props.Create(() => new SantasXmasRushGameLogic()),              "SantasXmasRush"));

            _dicGameLogicActors.Add(GAMEID.AztecSmash,                  Context.ActorOf(Props.Create(() => new AztecSmashGameLogic()),                  "AztecSmash"));
            _dicGameLogicActors.Add(GAMEID.IrishCrown,                  Context.ActorOf(Props.Create(() => new IrishCrownGameLogic()),                  "IrishCrown"));
            _dicGameLogicActors.Add(GAMEID.FloatingDragonYearOfTheSnake, Context.ActorOf(Props.Create(() => new FloatingDragonYearOfTheSnakeGameLogic()), "FloatingDragonYearOfTheSnake"));
            _dicGameLogicActors.Add(GAMEID.FonzosFelineFortunes,        Context.ActorOf(Props.Create(() => new FonzosFelineFortunesGameLogic()),        "FonzosFelineFortunes"));
            _dicGameLogicActors.Add(GAMEID.WildWildPearls,              Context.ActorOf(Props.Create(() => new WildWildPearlsGameLogic()),              "WildWildPearls"));
            _dicGameLogicActors.Add(GAMEID.AztecGemsMega,               Context.ActorOf(Props.Create(() => new AztecGemsMegaGameLogic()),               "AztecGemsMega"));
            _dicGameLogicActors.Add(GAMEID.BrickHouseBonanza,           Context.ActorOf(Props.Create(() => new BrickHouseBonanzaGameLogic()),           "BrickHouseBonanza"));
            _dicGameLogicActors.Add(GAMEID.WildWildebeestWins,          Context.ActorOf(Props.Create(() => new WildWildebeestWinsGameLogic()),          "WildWildebeestWins"));
            _dicGameLogicActors.Add(GAMEID.EscapeThePyramidFireIce,     Context.ActorOf(Props.Create(() => new EscapeThePyramidFireIceGameLogic()),     "EscapeThePyramidFireIce"));
            _dicGameLogicActors.Add(GAMEID.BiggerBassSplash,            Context.ActorOf(Props.Create(() => new BiggerBassSplashGameLogic()),            "BiggerBassSplash"));
            _dicGameLogicActors.Add(GAMEID.SavannahLegend,              Context.ActorOf(Props.Create(() => new SavannahLegendGameLogic()),              "SavannahLegend"));
            _dicGameLogicActors.Add(GAMEID.AncientIslandMega,           Context.ActorOf(Props.Create(() => new AncientIslandMegaGameLogic()),           "AncientIslandMega"));
            _dicGameLogicActors.Add(GAMEID.GreedyFortunePig,            Context.ActorOf(Props.Create(() => new GreedyFortunePigGameLogic()),            "GreedyFortunePig"));
            _dicGameLogicActors.Add(GAMEID.MahjongWins,                 Context.ActorOf(Props.Create(() => new MahjongWinsGameLogic()),                 "MahjongWins"));
            _dicGameLogicActors.Add(GAMEID.MahjongWins3BlackScatter,    Context.ActorOf(Props.Create(() => new MahjongWins3BlackScatterGameLogic()),    "MahjongWins3BlackScatter"));

            _dicGameLogicActors.Add(GAMEID.TouroSortudo,                Context.ActorOf(Props.Create(() => new TouroSortudoGameLogic()),                "TouroSortudo"));
            _dicGameLogicActors.Add(GAMEID.SweetBaklava,                Context.ActorOf(Props.Create(() => new SweetBaklavaGameLogic()),                "SweetBaklava"));
            _dicGameLogicActors.Add(GAMEID.PeppesPepperoniPizzaPlaza,   Context.ActorOf(Props.Create(() => new PeppesPepperoniPizzaPlazaGameLogic()),   "PeppesPepperoniPizzaPlaza"));
            //_dicGameLogicActors.Add(GAMEID.JohnHunterAndGalileosSecrets,Context.ActorOf(Props.Create(() => new JohnHunterAndGalileosSecretsGameLogic()),"JohnHunterAndGalileosSecrets"));
            _dicGameLogicActors.Add(GAMEID.BigBassReturnToTheRaces,     Context.ActorOf(Props.Create(() => new BigBassReturnToTheRacesGameLogic()),     "BigBassReturnToTheRaces"));
            _dicGameLogicActors.Add(GAMEID.RagingWaterfallMega,         Context.ActorOf(Props.Create(() => new RagingWaterfallMegaGameLogic()),         "RagingWaterfallMega"));
            _dicGameLogicActors.Add(GAMEID.TriplePotGold,               Context.ActorOf(Props.Create(() => new TriplePotGoldGameLogic()),               "TriplePotGold"));
            //_dicGameLogicActors.Add(GAMEID.LuckyWildPub,                Context.ActorOf(Props.Create(() => new LuckyWildPubGameLogic()),                "LuckyWildPub"));
            _dicGameLogicActors.Add(GAMEID.WildWildJoker,               Context.ActorOf(Props.Create(() => new WildWildJokerGameLogic()),               "WildWildJoker"));
            _dicGameLogicActors.Add(GAMEID.BlitzSuperWheel,             Context.ActorOf(Props.Create(() => new BlitzSuperWheelGameLogic()),             "BlitzSuperWheel"));
            _dicGameLogicActors.Add(GAMEID.LuckyMouse,                  Context.ActorOf(Props.Create(() => new LuckyMouseGameLogic()),                  "LuckyMouse"));
            _dicGameLogicActors.Add(GAMEID.LuckyTiger,                  Context.ActorOf(Props.Create(() => new LuckyTigerGameLogic()),                  "LuckyTiger"));
            _dicGameLogicActors.Add(GAMEID.TheDogHouseRoyalHunt,        Context.ActorOf(Props.Create(() => new TheDogHouseRoyalHuntGameLogic()),        "TheDogHouseRoyalHunt"));
            _dicGameLogicActors.Add(GAMEID.BanditMega,                  Context.ActorOf(Props.Create(() => new BanditMegaGameLogic()),                  "BanditMega"));

            _dicGameLogicActors.Add(GAMEID.JokersJewelsCash,            Context.ActorOf(Props.Create(() => new JokersJewelsCashGameLogic()),            "JokersJewelsCash"));
            _dicGameLogicActors.Add(GAMEID.RideTheLightning,            Context.ActorOf(Props.Create(() => new RideTheLightningGameLogic()),            "RideTheLightning"));
            _dicGameLogicActors.Add(GAMEID.LuckyDog,                    Context.ActorOf(Props.Create(() => new LuckyDogGameLogic()),                    "LuckyDog"));
            _dicGameLogicActors.Add(GAMEID.BigBassBonanza1000,          Context.ActorOf(Props.Create(() => new BigBassBonanza1000GameLogic()),          "BigBassBonanza1000"));
            _dicGameLogicActors.Add(GAMEID.GatesOfOlympusSuperScatter,  Context.ActorOf(Props.Create(() => new GatesOfOlympusSuperScatterGameLogic()),  "GatesOfOlympusSuperScatter"));
            _dicGameLogicActors.Add(GAMEID.JellyCandy,                  Context.ActorOf(Props.Create(() => new JellyCandyGameLogic()),                  "JellyCandy"));
            _dicGameLogicActors.Add(GAMEID.ResurrectingRiches,          Context.ActorOf(Props.Create(() => new ResurrectingRichesGameLogic()),          "ResurrectingRiches"));
            _dicGameLogicActors.Add(GAMEID.RizkBonanza,                 Context.ActorOf(Props.Create(() => new RizkBonanzaGameLogic()),                 "RizkBonanza"));
            _dicGameLogicActors.Add(GAMEID.PeruCandyland,               Context.ActorOf(Props.Create(() => new PeruCandylandGameLogic()),               "PeruCandyland"));
            _dicGameLogicActors.Add(GAMEID.BisonSpirit,                 Context.ActorOf(Props.Create(() => new BisonSpiritGameLogic()),                 "BisonSpirit"));
            _dicGameLogicActors.Add(GAMEID.FruitPartyDice,              Context.ActorOf(Props.Create(() => new FruitPartyDiceGameLogic()),              "FruitPartyDice"));
            _dicGameLogicActors.Add(GAMEID.Tukanito,                    Context.ActorOf(Props.Create(() => new TukanitoGameLogic()),                    "Tukanito"));
            _dicGameLogicActors.Add(GAMEID.CandyStashBonanza,           Context.ActorOf(Props.Create(() => new CandyStashBonanzaGameLogic()),           "CandyStashBonanza"));
            _dicGameLogicActors.Add(GAMEID.HadesInferno1000,            Context.ActorOf(Props.Create(() => new HadesInferno1000GameLogic()),            "HadesInferno1000"));
            _dicGameLogicActors.Add(GAMEID.MadMuertos,                  Context.ActorOf(Props.Create(() => new MadMuertosGameLogic()),                  "MadMuertos"));
            _dicGameLogicActors.Add(GAMEID.SweetCherryBlossom,          Context.ActorOf(Props.Create(() => new SweetCherryBlossomGameLogic()),          "SweetCherryBlossom"));
            _dicGameLogicActors.Add(GAMEID.SleepingDragon,              Context.ActorOf(Props.Create(() => new SleepingDragonGameLogic()),              "SleepingDragon"));
            _dicGameLogicActors.Add(GAMEID.FiestaFortune,               Context.ActorOf(Props.Create(() => new FiestaFortuneGameLogic()),               "FiestaFortune"));
            _dicGameLogicActors.Add(GAMEID.MajesticExpressGoldRun,      Context.ActorOf(Props.Create(() => new MajesticExpressGoldRunGameLogic()),      "MajesticExpressGoldRun"));
            _dicGameLogicActors.Add(GAMEID.WitchHeartMega,              Context.ActorOf(Props.Create(() => new WitchHeartMegaGameLogic()),              "WitchHeartMega"));
            _dicGameLogicActors.Add(GAMEID.GamesInOlympus1000,          Context.ActorOf(Props.Create(() => new GamesInOlympus1000GameLogic()),          "GamesInOlympus1000"));
            _dicGameLogicActors.Add(GAMEID.SweetBonanza1000Dice,        Context.ActorOf(Props.Create(() => new SweetBonanza1000DiceGameLogic()),        "SweetBonanza1000Dice"));
            _dicGameLogicActors.Add(GAMEID.MahjongWinsSuperScatter,     Context.ActorOf(Props.Create(() => new MahjongWinsSuperScatterGameLogic()),     "MahjongWinsSuperScatter"));
            _dicGameLogicActors.Add(GAMEID.HotSake,                     Context.ActorOf(Props.Create(() => new HotSakeGameLogic()),                     "HotSake"));
            _dicGameLogicActors.Add(GAMEID.LuckyMonkey,                 Context.ActorOf(Props.Create(() => new LuckyMonkeyGameLogic()),                 "LuckyMonkey"));
            _dicGameLogicActors.Add(GAMEID.EmotiWins,                   Context.ActorOf(Props.Create(() => new EmotiWinsGameLogic()),                   "EmotiWins"));
            _dicGameLogicActors.Add(GAMEID.WealthyFrog,                 Context.ActorOf(Props.Create(() => new WealthyFrogGameLogic()),                 "WealthyFrog"));
            _dicGameLogicActors.Add(GAMEID.PlushieWins,                 Context.ActorOf(Props.Create(() => new PlushieWinsGameLogic()),                 "PlushieWins"));
            _dicGameLogicActors.Add(GAMEID.LuckyTiger1000,              Context.ActorOf(Props.Create(() => new LuckyTiger1000GameLogic()),              "LuckyTiger1000"));
            _dicGameLogicActors.Add(GAMEID.LuckyPhoenix,                Context.ActorOf(Props.Create(() => new LuckyPhoenixGameLogic()),                "LuckyPhoenix"));
            _dicGameLogicActors.Add(GAMEID.JumboSafari,                 Context.ActorOf(Props.Create(() => new JumboSafariGameLogic()),                 "JumboSafari"));
            _dicGameLogicActors.Add(GAMEID.EyeOfSpartacus,              Context.ActorOf(Props.Create(() => new EyeOfSpartacusGameLogic()),              "EyeOfSpartacus"));
            _dicGameLogicActors.Add(GAMEID.WildWestGoldBlazingBounty,   Context.ActorOf(Props.Create(() => new WildWestGoldBlazingBountyGameLogic()),   "WildWestGoldBlazingBounty"));

            _dicGameLogicActors.Add(GAMEID.BigBassBoxingBonusRound,     Context.ActorOf(Props.Create(() => new BigBassBoxingBonusRoundGameLogic()),     "BigBassBoxingBonusRound"));
            _dicGameLogicActors.Add(GAMEID.TempleGuardians,             Context.ActorOf(Props.Create(() => new TempleGuardiansGameLogic()),             "TempleGuardians"));
            _dicGameLogicActors.Add(GAMEID.FingerLicknFreeSpins,        Context.ActorOf(Props.Create(() => new FingerLicknFreeSpinsGameLogic()),        "FingerLicknFreeSpins"));
            _dicGameLogicActors.Add(GAMEID.PigFarm,                     Context.ActorOf(Props.Create(() => new PigFarmGameLogic()),                     "PigFarm"));
            _dicGameLogicActors.Add(GAMEID.GoldParty2AfterHours,        Context.ActorOf(Props.Create(() => new GoldParty2AfterHoursGameLogic()),        "GoldParty2AfterHours"));
            _dicGameLogicActors.Add(GAMEID.GemFireFortune,              Context.ActorOf(Props.Create(() => new GemFireFortuneGameLogic()),              "GemFireFortune"));
            _dicGameLogicActors.Add(GAMEID.WavesOfPoseidon,             Context.ActorOf(Props.Create(() => new WavesOfPoseidonGameLogic()),             "WavesOfPoseidon"));
            _dicGameLogicActors.Add(GAMEID.GatesOfHades,                Context.ActorOf(Props.Create(() => new GatesOfHadesGameLogic()),                "GatesOfHades"));
            _dicGameLogicActors.Add(GAMEID.ClubTropicanaHappyHour,      Context.ActorOf(Props.Create(() => new ClubTropicanaHappyHourGameLogic()),      "ClubTropicanaHappyHour"));
            _dicGameLogicActors.Add(GAMEID.FortuneOfAztec,              Context.ActorOf(Props.Create(() => new FortuneOfAztecGameLogic()),              "FortuneOfAztec"));
            _dicGameLogicActors.Add(GAMEID.AlienInvaders,               Context.ActorOf(Props.Create(() => new AlienInvadersGameLogic()),               "AlienInvaders"));
            _dicGameLogicActors.Add(GAMEID.OlympusWinsSuperScatter,     Context.ActorOf(Props.Create(() => new OlympusWinsSuperScatterGameLogic()),     "OlympusWinsSuperScatter"));
            _dicGameLogicActors.Add(GAMEID.MummysJewels,                Context.ActorOf(Props.Create(() => new MummysJewelsGameLogic()),                "MummysJewels"));
            _dicGameLogicActors.Add(GAMEID.SweetBonanzaSuperScatter,    Context.ActorOf(Props.Create(() => new SweetBonanzaSuperScatterGameLogic()),    "SweetBonanzaSuperScatter"));
            _dicGameLogicActors.Add(GAMEID.ChilliHeatSpicySpins,        Context.ActorOf(Props.Create(() => new ChilliHeatSpicySpinsGameLogic()),        "ChilliHeatSpicySpins"));
            _dicGameLogicActors.Add(GAMEID.MasterGems,                  Context.ActorOf(Props.Create(() => new MasterGemsGameLogic()),                  "MasterGems"));
            _dicGameLogicActors.Add(GAMEID.YouCanPiggyBankOnIt,         Context.ActorOf(Props.Create(() => new YouCanPiggyBankOnItGameLogic()),         "YouCanPiggyBankOnIt"));
            _dicGameLogicActors.Add(GAMEID.GemTrio,                     Context.ActorOf(Props.Create(() => new GemTrioGameLogic()),                     "GemTrio"));
            _dicGameLogicActors.Add(GAMEID.ZombieSchoolMega,            Context.ActorOf(Props.Create(() => new ZombieSchoolMegaGameLogic()),            "ZombieSchoolMega"));
            _dicGameLogicActors.Add(GAMEID.Argonauts,                   Context.ActorOf(Props.Create(() => new ArgonautsGameLogic()),                   "Argonauts"));
            _dicGameLogicActors.Add(GAMEID.BigBassReelRepeat,           Context.ActorOf(Props.Create(() => new BigBassReelRepeatGameLogic()),           "BigBassReelRepeat"));
            
            
            
            
            
            
            


            List<string> symbols = new List<string>();
            foreach (KeyValuePair<GAMEID, IActorRef> pair in _dicGameLogicActors)
            {
                pair.Value.Tell(new DBProxyInform(_dbReader, _dbWriter, _redisWriter));
                pair.Value.Tell("loadSetting");
                var response = pair.Value.Ask<SymbolAndReplayResponse>(new SymbolAndReplayRequest()).Result;
                symbols.Add(response.Symbol);
                _hashAllChildActors.Add(pair.Value.Path.Name);
                Context.Watch(pair.Value);
            }
            string strLines = string.Join("\r\n", symbols.ToArray());
        }

        private async Task onLoadSpinDatabase(LoadSpinDataRequest request)
        {
            try
            {
                var infoDocuments = await Context.System.ActorSelection("/user/spinDBReaders").Ask<List<BsonDocument>>(new ReadInfoCollectionRequest(), TimeSpan.FromSeconds(10.0));
                foreach(BsonDocument infoDocument in infoDocuments)
                {
                    string      strGameName = (string) infoDocument["name"];
                    IActorRef   gameActor   = Context.Child(strGameName);
                    if (gameActor != ActorRefs.Nobody)
                        gameActor.Tell(infoDocument);
                    else
                        continue;
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
