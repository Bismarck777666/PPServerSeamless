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
        private static DBMonitorSnapshot                _sInstance          = new DBMonitorSnapshot();
        protected ConcurrentDictionary<string, bool>    _agentGameCloses    = new ConcurrentDictionary<string, bool>();

        public static DBMonitorSnapshot Instance
        {
            get { return _sInstance; }
        }

        protected ConcurrentDictionary<int, GameInfo> _gameInfos = new ConcurrentDictionary<int, GameInfo>();
        public Dictionary<string, string>   AgentHashKeys               { get; set; }
        public DateTime                     LastAgentUpdateTime         { get; set; }
        public DateTime                     GameConfigUpdateTime        { get; set; }
        public DateTime                     AgentGameCloseUpdateTime    { get; set; }

        public DBMonitorSnapshot()
        {
            this.LastAgentUpdateTime        = new DateTime(1970, 1, 1);
            this.AgentHashKeys              = new Dictionary<string, string>();
            this.GameConfigUpdateTime       = new DateTime(1970, 1, 1);
            this.AgentGameCloseUpdateTime   = new DateTime(1970, 1, 1);
        }

        public void setGameInfo(int gameID, int gameType, string symbol, string name, DateTime publishtime)
        {
            _gameInfos[gameID] = new GameInfo(gameID, gameType, name, symbol, publishtime);
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
            gameInfos = gameInfos.OrderByDescending(_ => _.PublishTime).ToList();
            return gameInfos;
        }

        public void setAgentGameOpenClose(int agentID, int gameID, int isClose)
        {
            string agentGame    = string.Format("{0}_{1}", agentID, gameID);
            bool isClosed       = isClose == 1;
            _agentGameCloses.AddOrUpdate(agentGame, isClosed,(key, oldValue) => { return isClosed; } );
        }

        public bool isAgentGameClosed(int agentID, int gameID)
        {
            bool isClosed;
            return _agentGameCloses.TryGetValue(string.Format("{0}_{1}", agentID, gameID), out isClosed) && isClosed;
        }
    }
    public class GameInfo
    {
        public int      GameProvider    { get; set; }
        public string   Symbol          { get; set; }
        public string   Name            { get; set; }
        public int      GameID          { get; set; }
        public DateTime PublishTime     { get; set; }

        public GameInfo(int gameID, int gameType, string name, string symbol, DateTime publishtime)
        {
            this.GameID         = gameID;
            this.GameProvider   = gameType;
            this.Name           = name;
            this.Symbol         = symbol;
            this.PublishTime    = publishtime;
        }
    }
}
