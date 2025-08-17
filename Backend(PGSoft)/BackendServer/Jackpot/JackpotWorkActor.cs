using Akka.Actor;
using SlotGamesNode.HTTPService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Routing.Constraints;

namespace SlotGamesNode.Jackpot
{
    public class JackpotWorkActor : ReceiveActor
    {
        private Dictionary<int, GameJackpotConfig>  _dicGameJackpotConfigs = new Dictionary<int, GameJackpotConfig>();
        private ICancelable                         _tickScheduler         = null;
        public JackpotWorkActor()
        {
            Receive<List<GameJackpotConfig>>                        (onInitGameJackpotThemes);
            Receive<Dictionary<int, List<GameJackpotTypeConfig>>>   (onInitGameJackpotTypes);
            Receive<GameJackpotConfig>                              (onChangedGameJackpotTheme);
            Receive<GameJackpotTypeChanged>                         (onGameJackpotTypeChanged);
            Receive<GetGameJackpotThemeRequest>                     (onGetGameJackpotTheme);
            Receive<GetGameJackpotInfoRequest>                      (onGetGameJackpotInfo);
            Receive<CheckJackpotAvailable>                          (onCheckJackpotAvailable);
            Receive<string>(onProcCommand);
        }
        private void onCheckJackpotAvailable(CheckJackpotAvailable request)
        {
            if(_dicGameJackpotConfigs.ContainsKey(request.GameId))
            {
                GameJackpotConfig config = _dicGameJackpotConfigs[request.GameId];
                foreach(GameJackpotTypeConfig jackpotConfig in config.Jackpots)
                {
                    if(jackpotConfig.Category == request.Category)
                    {
                        Sender.Tell(true);
                        return;
                    }
                }
            }
            Sender.Tell(false);
        }
        private void onGetGameJackpotInfo(GetGameJackpotInfoRequest request)
        {
            int gameID = request.GameId;
            if(!_dicGameJackpotConfigs.ContainsKey(gameID))
            {
                Sender.Tell(new List<GameJackpotTypeInfo>());
                return;
            }
            List<GameJackpotTypeInfo>   result      = new List<GameJackpotTypeInfo>();
            int                         typeCount   = _dicGameJackpotConfigs[gameID].TypeCount;
            for(int i = 1; i <= typeCount; i++)
            {
                bool isFound = false;
                foreach(GameJackpotTypeConfig typeConfig in _dicGameJackpotConfigs[gameID].Jackpots)
                {
                    if(typeConfig.Level == i)
                    {
                        result.Add(new GameJackpotTypeInfo((int)typeConfig.TypeID, typeConfig.Min, typeConfig.Max, typeConfig.Category,
                            typeConfig.RateUp, typeConfig.CurrentAmt));
                        isFound = true;
                        break;
                    }
                }
                if (!isFound)
                    result.Add(new GameJackpotTypeInfo(0, 0.0, 0.0, 0, 0.0, 0.0));
            }
            Sender.Tell(result);
        }
        private void onGetGameJackpotTheme(GetGameJackpotThemeRequest request)
        {
            if(!_dicGameJackpotConfigs.ContainsKey(request.GameId))
            {
                Sender.Tell(new GameJackpotThemeResult(0, 0, 0));
                return;
            }
            Sender.Tell(new GameJackpotThemeResult(1, _dicGameJackpotConfigs[request.GameId].ThemeId, _dicGameJackpotConfigs[request.GameId].TypeCount));
        }
        private void onInitGameJackpotThemes(List<GameJackpotConfig> themes)
        {
            _dicGameJackpotConfigs.Clear();
            for (int i = 0; i < themes.Count; i++)
                _dicGameJackpotConfigs[themes[i].GameId] = themes[i];
        }
        private void onInitGameJackpotTypes(Dictionary<int, List<GameJackpotTypeConfig>> configs)
        {
            foreach(KeyValuePair<int, List<GameJackpotTypeConfig>> pair in configs)
            {
                if (!_dicGameJackpotConfigs.ContainsKey(pair.Key))
                    continue;

                _dicGameJackpotConfigs[pair.Key].Jackpots = pair.Value;
            }
        }
        private void onChangedGameJackpotTheme(GameJackpotConfig jackpotTheme)
        {
            if(_dicGameJackpotConfigs.ContainsKey(jackpotTheme.GameId))
            {
                _dicGameJackpotConfigs[jackpotTheme.GameId].ThemeId   = jackpotTheme.ThemeId;
                _dicGameJackpotConfigs[jackpotTheme.GameId].TypeCount = jackpotTheme.TypeCount;
            }
            else
            {
                _dicGameJackpotConfigs[jackpotTheme.GameId] = jackpotTheme;
            }
        }
        private void onGameJackpotTypeChanged(GameJackpotTypeChanged changed)
        {
            if (!_dicGameJackpotConfigs.ContainsKey(changed.GameId))
                return;

            GameJackpotConfig gameJackpotConfig = _dicGameJackpotConfigs[changed.GameId];
            int insertId = -1;
            for (int i = 0; i < gameJackpotConfig.Jackpots.Count; i++)
            {
                if(changed.JackpotTypeConfig.Level == gameJackpotConfig.Jackpots[i].Level)
                {
                    insertId = i;
                    break;
                }
            }
            if(insertId >= 0)
            {
                gameJackpotConfig.Jackpots[insertId].TypeID     = changed.JackpotTypeConfig.TypeID;
                gameJackpotConfig.Jackpots[insertId].Min        = changed.JackpotTypeConfig.Min;
                gameJackpotConfig.Jackpots[insertId].Max        = changed.JackpotTypeConfig.Max;
                gameJackpotConfig.Jackpots[insertId].Category   = changed.JackpotTypeConfig.Category;
                gameJackpotConfig.Jackpots[insertId].RateUp     = changed.JackpotTypeConfig.RateUp;
                if(changed.JackpotTypeConfig.ResetAmt)
                    gameJackpotConfig.Jackpots[insertId].CurrentAmt = changed.JackpotTypeConfig.CurrentAmt;
            }
            else
            {
                gameJackpotConfig.Jackpots.Add(changed.JackpotTypeConfig);
            }
        }
        private void onProcCommand(string strCommand)
        {
            if(strCommand == "start")
            {
                _tickScheduler = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(1000, 1000, Self, "tick", Self);
            }
            else if(strCommand == "tick")
            {
                foreach(KeyValuePair<int, GameJackpotConfig> pair in _dicGameJackpotConfigs)
                {
                    foreach(GameJackpotTypeConfig config in pair.Value.Jackpots)
                    {
                        if(config.TypeID == GameJackpotTypes.ProgAmt)
                        {
                            if(config.CurrentAmt < config.Min || config.CurrentAmt > config.Max)
                                config.CurrentAmt = config.Min;
                            else
                                config.CurrentAmt += config.RateUp / 60.0;
                        }
                    }
                }
            }
        }
    }

    public class GameJackpotConfig
    {
        public int                          GameId      { get; set; }
        public int                          ThemeId     { get; set; }
        public int                          TypeCount   { get; set; }
        public List<GameJackpotTypeConfig>   Jackpots    { get; set; } 
        public GameJackpotConfig()
        {
            this.Jackpots = new List<GameJackpotTypeConfig>();
        }
    }
    public enum GameJackpotTypes
    {
        None    = 0,
        RandMul = 1,
        RandAmt = 2,
        FixMul  = 3,
        ProgAmt = 4,
    }
    public class GameJackpotTypeChanged
    {
        public int                      GameId              { get; set; }
        public GameJackpotTypeConfig    JackpotTypeConfig   { get; set; }
    }
    public class GameJackpotTypeConfig
    {
        public GameJackpotTypes TypeID      { get; set; }
        public int              Level       { get; set; }
        public double           Min         { get; set; }
        public double           Max         { get; set; }
        public int              Category    { get; set; }
        public double           RateUp      { get; set; }
        public bool             ResetAmt    { get; set; }
        public double           CurrentAmt  { get; set; }
    }
    public class GetGameJackpotThemeRequest
    {
        public int GameId { get; set; }
        public GetGameJackpotThemeRequest(int gameId)
        {
            GameId = gameId;
        }
    }
    public class GetGameJackpotInfoRequest
    {
        public int GameId { get; set; }
        public GetGameJackpotInfoRequest(int gameId)
        {
            GameId = gameId;
        }
    }
    public class GameJackpotTypeInfo
    {
        public int      type        { get; set; }
        public double   min         { get; set; }
        public double   max         { get; set; }
        public int      category    { get; set; }
        public double   rateUp      { get; set; }
        public double   currentAmt  { get; set; }

        public GameJackpotTypeInfo(int type, double min, double max, int category, double rateUp, double currentAmt)
        {
            this.type       = type;
            this.min        = min;
            this.max        = max;
            this.category   = category;   
            this.rateUp     = rateUp;
            this.currentAmt = currentAmt;
        }
    }
    public class CheckJackpotAvailable
    {
        public int GameId   { get; set; }
        public int Category { get; set; }

        public CheckJackpotAvailable(int gameId, int category)
        {
            this.GameId     = gameId;
            this.Category   = category;
        }
    }
}

