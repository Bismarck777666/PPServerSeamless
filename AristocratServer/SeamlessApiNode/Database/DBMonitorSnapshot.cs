using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace QueenApiNode.Database
{
    class DBMonitorSnapshot
    {
        private static DBMonitorSnapshot _sInstance = new DBMonitorSnapshot();

        public static DBMonitorSnapshot Instance
        {
            get { return _sInstance; }
        }

        protected ConcurrentDictionary<int, GameInfo> _gameInfos = new ConcurrentDictionary<int, GameInfo>();
        public Dictionary<string, string> AgentHashKeys
        {
            get;set;
        }
        public DateTime LastAgentUpdateTime { get; set; }
        public DateTime GameConfigUpdateTime { get; set; }

        public DBMonitorSnapshot()
        {
            this.LastAgentUpdateTime  = new DateTime(1970, 1, 1);
            this.AgentHashKeys        = new Dictionary<string, string>();
            this.GameConfigUpdateTime = new DateTime(1, 1, 1);
        }

        public void setGameInfo(int gameID, int gameType, string symbol, string name, string coverImg, string title, double monthTurnover, double weekTurnover, DateTime releaseDate)
        {
            _gameInfos[gameID] = new GameInfo(gameID, gameType, name, symbol, coverImg, title, monthTurnover, weekTurnover, releaseDate);
        }
        public GameInfo getGameInfo(int gameID)
        {
            GameInfo gameInfo = null;
            if (_gameInfos.TryGetValue(gameID, out gameInfo))
                return gameInfo;
            else
                return null;
        }
        public List<GameInfo> getAllGameInfo()
        {
            List<GameInfo> gameInfos = new List<GameInfo>(_gameInfos.Values);
            return gameInfos;
        }
    }
    public class GameInfo
    {
        public int      GameProvider    { get; set; }
        public string   Symbol          { get; set; }
        public string   Title           { get; set; }
        public string   Name            { get; set; }
        public int      GameID          { get; set; }
        public string   CoverImg        { get; set; }
        public double   MonthTurnover   { get; set; }
        public double   WeekTurnover    { get; set; }
        public DateTime ReleaseDate     { get; set; }
        public GameInfo(int gameID, int gameType, string name, string symbol, string coverImg, string title, double monthturover, double weekturnover, DateTime releaseDate)
        {
            this.GameID         = gameID;
            this.GameProvider   = gameType;
            this.Name           = name;
            this.Symbol         = symbol;
            this.CoverImg       = coverImg;
            this.Title          = title;
            this.ReleaseDate    = releaseDate;
            this.MonthTurnover  = monthturover;
            this.WeekTurnover   = weekturnover;
        }
    }
}
