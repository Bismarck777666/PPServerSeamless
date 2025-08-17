using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using System.Collections.Concurrent;

namespace CommNode.Database
{
    public class DBMonitorSnapshot
    {
        private static  DBMonitorSnapshot _sInstance = new DBMonitorSnapshot();
        public static DBMonitorSnapshot Instance
        {
            get
            {
                return _sInstance;
            }
        }        
        public DateTime         GameConfigUpdateTime        { get; set; }
        public long             LastQuitUserID              { get; set; }

        protected ConcurrentDictionary<int, GAMETYPE>       _gameTypeMap            = new ConcurrentDictionary<int, GAMETYPE>();
        protected ConcurrentDictionary<string, string>      _gameDataMap            = new ConcurrentDictionary<string, string>();
        protected ConcurrentDictionary<string, int>[]       _gameSymbolToIDMap      = new ConcurrentDictionary<string, int>[(int) GAMETYPE.COUNT];
        public DateTime     LastUserUpdateTime                      { get; set; }
        public DateTime     LastAgentUpdateTime                     { get; set; }

        public bool         IsNowMaintenance                        { get; set; }
        public DateTime     MaintenanceUpdateTime                   { get; set; }
        public DBMonitorSnapshot()
        {
            this.GameConfigUpdateTime           = new DateTime(1, 1, 1);
            this.LastQuitUserID                 = -1;
            this.LastUserUpdateTime             = new DateTime(1, 1, 1);
            this.LastAgentUpdateTime            = new DateTime(1, 1, 1);
            this.MaintenanceUpdateTime          = new DateTime(1, 1, 1);
            this.IsNowMaintenance               = false;
        }
        public void setGameType(int gameID, GAMETYPE gameType)
        {
            _gameTypeMap.AddOrUpdate(gameID, gameType, (key, oldValue) => { return gameType; });
        }
        public void setGameSymbol(string strGameSymbol, GAMETYPE gameType, int gameID, string strGameData)
        {
            if (string.IsNullOrEmpty(strGameSymbol))
                return;

            int gameTypeID = (int)gameType - 1;
            if (gameTypeID < 0 || gameTypeID >= (int)GAMETYPE.COUNT)
                return;

            if (_gameSymbolToIDMap[gameTypeID] == null)
                _gameSymbolToIDMap[gameTypeID] = new ConcurrentDictionary<string, int>();

            string oldSymbol = null;
            foreach(KeyValuePair<string, int> pair in _gameSymbolToIDMap[gameTypeID])
            {
                if(gameID == pair.Value)
                {
                    oldSymbol = pair.Key;
                    break;
                }
            }

            if(oldSymbol != null)
            {
                int oldGameID = 0;
                _gameSymbolToIDMap[gameTypeID].TryRemove(oldSymbol, out oldGameID);
            }            
            _gameSymbolToIDMap[gameTypeID].AddOrUpdate(strGameSymbol, gameID, (key, oldValue) => { return gameID; });
            if(gameType == GAMETYPE.ARISTO)
                _gameDataMap.AddOrUpdate(strGameSymbol, strGameData, (key, oldValue) => { return strGameData; });

        }
        public string getGameSymbolFromID(GAMETYPE gameType, int gameID)
        {
            int gameTypeID = (int)gameType - 1;
            if (gameTypeID < 0 || gameTypeID >= (int)GAMETYPE.COUNT)
                return "";

            foreach(KeyValuePair<string, int> pair in _gameSymbolToIDMap[gameTypeID])
            {
                if (pair.Value == gameID)
                    return pair.Key;
            }
            return "";
        }
        public int getGameIDFromString(GAMETYPE gameType, string strGameID)
        {
            int gameID = 0;
            int gameTypeID = (int)gameType - 1;
            if (gameTypeID < 0 || gameTypeID >= (int)GAMETYPE.COUNT)
                return 0;

            if (_gameSymbolToIDMap[gameTypeID].TryGetValue(strGameID, out gameID))
                return gameID;

            return 0;
        }

        public string getGameData(string strSymbol)
        {
            string gameData = null;
            if (_gameDataMap.TryGetValue(strSymbol, out gameData))
                return gameData;
            else
                return null;
        }
        public GAMETYPE getGameType(int gameID)
        {
            GAMETYPE gameType = GAMETYPE.NONE;
            if (_gameTypeMap.TryGetValue(gameID, out gameType))
                return gameType;

            return GAMETYPE.NONE;
        }
    }
}
