using GITProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    public class SpaceGoonzGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGSpaceGoonz";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "f6575cef-9875-49b2-a4b1-ddbc47497b0a";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "36283b63f84e84140e2e4df207e1c14d4e3a301e";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.10224.414";
            }
        }
        protected override float MiniCoin
        {
            get
            {
                return 20.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 1024;
            }
        }
        protected override string BetType
        {
            get
            {
                return "Ways";
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idWild",    name = "Wild"   } },    
                    {2,   new HabaneroLogSymbolIDName{id = "idHi1U",    name = "Hi1U"   } },    
                    {3,   new HabaneroLogSymbolIDName{id = "idHi2U",    name = "Hi2U"   } },    
                    {4,   new HabaneroLogSymbolIDName{id = "idHi3U",    name = "Hi3U"   } },    
                    {5,   new HabaneroLogSymbolIDName{id = "idHi4U",    name = "Hi4U"   } },    
                    {6,   new HabaneroLogSymbolIDName{id = "idHi5U",    name = "Hi5U"   } },    
                    {7,   new HabaneroLogSymbolIDName{id = "idHi1",     name = "Hi1"    } },    
                    {8,   new HabaneroLogSymbolIDName{id = "idHi2",     name = "Hi2"    } },    
                    {9,   new HabaneroLogSymbolIDName{id = "idHi3",     name = "Hi3"    } },    
                    {10,  new HabaneroLogSymbolIDName{id = "idHi4",     name = "Hi4"    } },    
                    {11,  new HabaneroLogSymbolIDName{id = "idHi5",     name = "Hi5"    } },
                    {12,  new HabaneroLogSymbolIDName{id = "idLo1",     name = "Lo1"    } },
                    {13,  new HabaneroLogSymbolIDName{id = "idLo2",     name = "Lo2"    } },
                    {14,  new HabaneroLogSymbolIDName{id = "idLo5",     name = "Lo3"    } },
                    {15,  new HabaneroLogSymbolIDName{id = "idLo4",     name = "Lo4"    } },
                    {16,  new HabaneroLogSymbolIDName{id = "idLo5",     name = "Lo5"    } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 658;
            }
        }
        #endregion

        public SpaceGoonzGameLogic()
        {
            _gameID     = GAMEID.SpaceGoonz;
            GameName    = "SpaceGoonz";
        }

        protected override JObject buildEventItem(string strGlobalUserID, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strGlobalUserID, currentIndex);
            
            HabaneroHistoryResponses responses = _dicUserHistory[strGlobalUserID].Responses[currentIndex];
            dynamic response    = JsonConvert.DeserializeObject<dynamic>(responses.Response);
            
            if (!object.ReferenceEquals(response["currentfreegame"], null))
                eventItem["multiplier"] = 1;

            if (!object.ReferenceEquals(response["SpaceGoonz_upgradeMessage"], null))
            {
                JArray reelslist = buildHabaneroLogReelslist(response);
                eventItem["reelslist"] = reelslist;
            }

            return eventItem;
        }
        
        protected override JArray buildHabaneroLogReels(string strGlobalUserID,int currentIndex ,dynamic response, bool containWild = false)
        {
            if (object.ReferenceEquals(response["SpaceGoonz_upgradeMessage"], null))
                return base.buildHabaneroLogReels(strGlobalUserID, currentIndex, response as JObject, containWild);
            
            List<int> upgradeFromList   = new List<int>();
            List<int> upgradeToList     = new List<int>();

            for(int i = 0; i < response["SpaceGoonz_upgradeMessage"]["upgradeFromList"].Count; i++)
            {
                upgradeFromList.Add((int)response["SpaceGoonz_upgradeMessage"]["upgradeFromList"][i]);
                upgradeToList.Add((int)response["SpaceGoonz_upgradeMessage"]["upgradeToList"][i]);
            }

            JArray reels = new JArray();
            for (int i = 0; i < response["reels"].Count; i++)
            {
                JArray col = new JArray();
                for (int j = 0; j < response["reels"][i].Count; j++)
                {
                    int symbol          = Convert.ToInt32(response["reels"][i][j]["symbolid"]);
                    int upgradeIndex    = upgradeFromList.FindIndex(_ => _ == symbol);
                    
                    if (upgradeIndex != -1)
                        symbol = upgradeToList[upgradeIndex];
                    
                    string symbolid = SymbolIdStringForLog[symbol].id;
                    col.Add(symbolid);
                }
                reels.Add(col);
            }
            return reels;
        }

        protected override JArray buildHabaneroLogReelslist(dynamic currentResult)
        {
            JArray reelslist        = new JArray();
            JObject reelslistItem   = new JObject();
            JArray reelsListCols    = new JArray();

            for (int i = 0; i < currentResult["virtualreels"].Count; i++)
            {
                JArray reelsCol = new JArray();
                for (int j = 2; j < currentResult["virtualreels"][i].Count - 2; j++)
                    reelsCol.Add(SymbolIdStringForLog[(int)currentResult["virtualreels"][i][j]].id);
                reelsListCols.Add(reelsCol);
            }
            reelslistItem["reels"] = reelsListCols;
            reelslist.Add(reelslistItem);
            
            return reelslist;
        }
    }
}
