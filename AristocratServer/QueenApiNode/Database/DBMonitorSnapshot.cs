using GITProtocol;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueenApiNode.Database
{
    class DBMonitorSnapshot
    {
        private static DBMonitorSnapshot _sInstance = new DBMonitorSnapshot();
        public static DBMonitorSnapshot Instance
        {
            get { return _sInstance; }
        }

        public Dictionary<string, string> AgentHashKeys
        {
            get;set;
        }
        public DateTime LastAgentUpdateTime { get; set; }

        protected ConcurrentDictionary<string, int>[] _gameSymbolToIDMap    = new ConcurrentDictionary<string, int>[(int)GAMETYPE.COUNT];
        protected ConcurrentDictionary<int, GAMETYPE> _gameTypeMap          = new ConcurrentDictionary<int, GAMETYPE>();

        public DateTime GameConfigUpdateTime { get; set; }

        public DBMonitorSnapshot()
        {
            this.LastAgentUpdateTime = new DateTime(1970, 1, 1);
            this.AgentHashKeys       = new Dictionary<string, string>();
        }
        public void setGameType(int gameID, GAMETYPE gameType)
        {
            _gameTypeMap.AddOrUpdate(gameID, gameType, (key, oldValue) => { return gameType; });
        }
        public GAMETYPE getGameType(int gameID)
        {
            if (_gameTypeMap.ContainsKey(gameID))
                return _gameTypeMap[gameID];

            return GAMETYPE.NONE;
        }

        public void setGameSymbol(string strGameSymbol, GAMETYPE gameType, int gameID)
        {
            if (string.IsNullOrEmpty(strGameSymbol))
                return;

            int gameTypeID = (int)gameType - 1;
            if (gameTypeID < 0 || gameTypeID >= (int)GAMETYPE.COUNT)
                return;

            if (_gameSymbolToIDMap[gameTypeID] == null)
                _gameSymbolToIDMap[gameTypeID] = new ConcurrentDictionary<string, int>();

            string oldSymbol = null;
            foreach (KeyValuePair<string, int> pair in _gameSymbolToIDMap[gameTypeID])
            {
                if (gameID == pair.Value)
                {
                    oldSymbol = pair.Key;
                    break;
                }
            }

            if (oldSymbol != null)
            {
                int oldGameID = 0;
                _gameSymbolToIDMap[gameTypeID].TryRemove(oldSymbol, out oldGameID);
            }
            _gameSymbolToIDMap[gameTypeID].AddOrUpdate(strGameSymbol, gameID, (key, oldValue) => { return gameID; });
        }

        public string getGameSymbolFromID(GAMETYPE gameType, int gameID)
        {
            int gameTypeID = (int)gameType - 1;
            if (gameTypeID < 0 || gameTypeID >= (int)GAMETYPE.COUNT)
                return "";

            foreach (KeyValuePair<string, int> pair in _gameSymbolToIDMap[gameTypeID])
            {
                if (pair.Value == gameID)
                    return pair.Key;
            }
            return "";
        }
    }
}
