using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using Newtonsoft.Json;

namespace FrontNode.Database
{
    class PGGamesSnapshot
    {
        private static PGGamesSnapshot _sInstance = new PGGamesSnapshot();
        public static PGGamesSnapshot Instance
        {
            get
            {
                return _sInstance;
            }
        }

        public Dictionary<string, GAMEID> _dicGameStringToIDs = new Dictionary<string, GAMEID>();

        public PGGamesSnapshot()
        {
            
        }

        public string findGameStringFromID(int id)
        {
            foreach(KeyValuePair<string, GAMEID> pair in _dicGameStringToIDs)
            {
                if ((int) pair.Value == id)
                    return pair.Key;
            }
            return null;
        }
        public GAMEID findGameIDFromString(string strGameID)
        {
            if (_dicGameStringToIDs.ContainsKey(strGameID))
                return _dicGameStringToIDs[strGameID];

            return GAMEID.None;
        }
        public List<GAMEID> getAllGameIDs()
        {
            return new List<GAMEID>(_dicGameStringToIDs.Values);
        }
        public void setGameString(GAMEID gameID, string strSymbol)
        {
            _dicGameStringToIDs[strSymbol] = gameID;
        }
    }    
}
