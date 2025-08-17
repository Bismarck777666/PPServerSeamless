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
        private static  DBMonitorSnapshot   _sInstance      = new DBMonitorSnapshot();
        public static   DBMonitorSnapshot   Instance        => _sInstance;

        public List<int>    ClosedGameIDs                   { get; set; }
        public DateTime     GameConfigUpdateTime            { get; set; }
        public long         LastQuitUserID                  { get; set; }
        public long         LastRangeEventPlayerID          { get; set; }
        public DateTime     LastRangeEventUpdateTime        { get; set; }
        public DateTime     LastUserUpdateTime              { get; set; }
        public DateTime     LastAgentUpdateTime             { get; set; }
        public DateTime     LastBNGGameUpdateTime           { get; set; }
        
        protected ConcurrentDictionary<int, GAMETYPE>       _gameTypeMap            = new ConcurrentDictionary<int, GAMETYPE>();
        protected ConcurrentDictionary<string, int>[]       _gameSymbolToIDMap      = new ConcurrentDictionary<string, int>[(int) GAMETYPE.COUNT];
        protected ConcurrentDictionary<string, string>      _bngGameDrawVersions    = new ConcurrentDictionary<string, string>();
        
        public DBMonitorSnapshot()
        {
            ClosedGameIDs           = new List<int>();
            GameConfigUpdateTime    = new DateTime(1, 1, 1);
            LastQuitUserID          = -1;

            LastRangeEventPlayerID  = -1;
            LastUserUpdateTime      = new DateTime(1, 1, 1);
            LastAgentUpdateTime     = new DateTime(1, 1, 1);
            LastBNGGameUpdateTime   = new DateTime(1, 1, 1);
        }

        public void setBNGGameDrawVer(string strGameName, string strDrawVer)
        {
            _bngGameDrawVersions.AddOrUpdate(strGameName, strDrawVer, (key, oldValue) => { return strDrawVer; });
        }

        public void setGameType(int gameID, GAMETYPE gameType)
        {
            _gameTypeMap.AddOrUpdate(gameID, gameType, (key, oldValue) => { return gameType; });
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
        }

        public string getBNGGameDrawVer(string strGameName)
        {
            string strDrawVersion = "";
            if (_bngGameDrawVersions.TryGetValue(strGameName, out strDrawVersion))
                return strDrawVersion;

            return "";              
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

        public GAMETYPE getGameType(int gameID)
        {
            GAMETYPE gameType = GAMETYPE.NONE;
            if (_gameTypeMap.TryGetValue(gameID, out gameType))
                return gameType;

            return GAMETYPE.NONE;
        }
    }
}
