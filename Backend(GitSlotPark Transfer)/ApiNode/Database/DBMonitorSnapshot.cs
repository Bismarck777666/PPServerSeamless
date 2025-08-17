using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace QueenApiNode.Database
{
    class DBMonitorSnapshot
    {
        private static DBMonitorSnapshot    _sInstance  = new DBMonitorSnapshot();
        public static DBMonitorSnapshot     Instance    => _sInstance;
        
        protected ConcurrentDictionary<int, GameInfo> _gameInfos = new ConcurrentDictionary<int, GameInfo>();
        protected List<GameInfo> _sortedGameList = new List<GameInfo>();
        public Dictionary<string, string>   AgentHashKeys           { get; set; }
        public DateTime                     LastAgentUpdateTime     { get; set; }
        public DateTime                     GameConfigUpdateTime    { get; set; }

        public DBMonitorSnapshot()
        {
            LastAgentUpdateTime     = new DateTime(1970, 1, 1);
            AgentHashKeys           = new Dictionary<string, string>();
            GameConfigUpdateTime    = new DateTime(1, 1, 1);
        }

        public void setGameInfo(int gameID,int gameType,string symbol,string name,DateTime releaseDate)
        {
            GameInfo updatedInfo = new GameInfo(gameID, gameType, name, symbol, releaseDate);
            if (!_gameInfos.ContainsKey(gameID))
            {
                _sortedGameList.Add(updatedInfo);
                _gameInfos[gameID] = updatedInfo;
            }
            else
                _gameInfos[gameID].updateInfo(updatedInfo);
        }

        public void sortGameInfos()
        {
            for (int i = 0; i < _sortedGameList.Count - 1; ++i)
            {
                for (int j = i + 1; j < _sortedGameList.Count; ++j)
                {
                    if (_sortedGameList[i].ReleaseDate > _sortedGameList[j].ReleaseDate)
                    {
                        GameInfo sortedGame = _sortedGameList[i];
                        _sortedGameList[i] = _sortedGameList[j];
                        _sortedGameList[j] = sortedGame;
                    }
                }
            }
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
            return new List<GameInfo>(_sortedGameList);
        }
    }

    public class GameInfo
    {
        public int      GameProvider    { get; set; }
        public string   Symbol          { get; set; }
        public string   Name            { get; set; }
        public int      GameID          { get; set; }
        public DateTime ReleaseDate     { get; set; }

        public GameInfo(int gameID, int gameType, string name, string symbol, DateTime releaseDate)
        {
            GameID          = gameID;
            GameProvider    = gameType;
            Name            = name;
            Symbol          = symbol;
            ReleaseDate     = releaseDate;
        }

        public void updateInfo(GameInfo updatedInfo)
        {
            Symbol          = updatedInfo.Symbol;
            GameProvider    = updatedInfo.GameProvider;
            Name            = updatedInfo.Name;
            ReleaseDate     = updatedInfo.ReleaseDate;
        }
    }
}
