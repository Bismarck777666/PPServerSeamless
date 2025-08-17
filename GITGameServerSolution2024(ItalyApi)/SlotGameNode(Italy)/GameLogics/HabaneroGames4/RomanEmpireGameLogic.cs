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
    public class RomanEmpireGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGRomanEmpire";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "021e4d85-be45-44fa-940e-20dd37eaf851";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "ead4ccd4ef6b03dbbd63481f86ddbb35f77f38be";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.1331.93";
            }
        }
        protected override float MiniCoin
        {
            get
            {
                return 25.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 25;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idCaesar",      name = "Caesar"     } },
                    {2,   new HabaneroLogSymbolIDName{id = "idColiseum",    name = "Coliseum"   } },
                    {3,   new HabaneroLogSymbolIDName{id = "idCleopatra",   name = "Cleopatra"  } },
                    {4,   new HabaneroLogSymbolIDName{id = "idHelmet",      name = "Helmet"     } },
                    {5,   new HabaneroLogSymbolIDName{id = "idShield",      name = "Shield"     } },
                    {6,   new HabaneroLogSymbolIDName{id = "idFruitBowl",   name = "FruitBowl"  } },
                    {7,   new HabaneroLogSymbolIDName{id = "idA",           name = "A"          } },
                    {8,   new HabaneroLogSymbolIDName{id = "idK",           name = "K"          } },
                    {9,   new HabaneroLogSymbolIDName{id = "idQ",           name = "Q"          } },
                    {10,  new HabaneroLogSymbolIDName{id = "idJ",           name = "J"          } },
                    {11,  new HabaneroLogSymbolIDName{id = "id10",          name = "10"         } },
                    {12,  new HabaneroLogSymbolIDName{id = "id9",           name = "9"          } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 266;
            }
        }
        #endregion

        public RomanEmpireGameLogic()
        {
            _gameID     = GAMEID.RomanEmpire;
            GameName    = "RomanEmpire";
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);
            
            dynamic resultContext   = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strUserId].Responses[currentIndex].Response);
            if (!object.ReferenceEquals(resultContext["currentfreegame"], null))
                eventItem["multiplier"] = 2;
            
            JArray subEvents = new JArray();
            dynamic reels           = resultContext["reels"];

            JArray winWindows = new JArray();
            int winFreeGameCnt = 0;
            for(int i = 0; i < reels.Count; i++)
            {
                for(int j = 0; j < reels[i].Count; j++)
                {
                    if(!object.ReferenceEquals(reels[i][j]["winfreegames"], null))
                    {
                        JArray winWindow = new JArray();
                        int winFreeGame = (int)reels[i][j]["winfreegames"];
                        winFreeGameCnt += winFreeGame;

                        winWindow.Add(i);
                        winWindow.Add(j);
                        winWindows.Add(winWindow);
                    }
                }
            }

            if(winFreeGameCnt > 0)
            {
                JObject subEventItem = new JObject();
                subEventItem["type"]            = "scatter";
                subEventItem["wincash"]         = 0;
                subEventItem["symbol"]          = SymbolIdStringForLog[2].name;
                subEventItem["multiplier"]      = 0;
                subEventItem["winfreegames"]    = winFreeGameCnt;
                JArray lineWinArray = new JArray();
                subEventItem["windows"]     = winWindows;
                subEvents.Add(subEventItem);
            }

            if (!object.ReferenceEquals(resultContext["linewins"], null))
            {
                for (int j = 0; j < resultContext["linewins"].Count; j++)
                {
                    JObject subEventItem = new JObject();
                    subEventItem["type"]        = "payline";
                    subEventItem["wincash"]     = resultContext["linewins"][j]["wincash"];
                    int symbol          = Convert.ToInt32(resultContext["linewins"][j]["symbolid"]);
                    string symbolName   = SymbolIdStringForLog[symbol].name;
                    subEventItem["symbol"]      = symbolName;
                    subEventItem["multiplier"]  = resultContext["linewins"][j]["multiplier"];
                    subEventItem["lineno"]      = resultContext["linewins"][j]["paylineindex"];
                    JArray lineWinArray = new JArray();
                    for (int k = 0; k < resultContext["linewins"][j]["winningwindows"].Count; k++)
                    {
                        JArray lineWinItem = new JArray();
                        lineWinItem.Add(resultContext["linewins"][j]["winningwindows"][k]["reelindex"]);
                        lineWinItem.Add(resultContext["linewins"][j]["winningwindows"][k]["symbolindex"]);
                        lineWinArray.Add(lineWinItem);
                    }
                    subEventItem["windows"]     = lineWinArray;
                    subEvents.Add(subEventItem);
                }
            }

            if (!object.ReferenceEquals(resultContext["scatterwins"], null))
            {
                for (int j = 0; j < resultContext["scatterwins"].Count; j++)
                {
                    JObject subEventItem = new JObject();
                    subEventItem["type"]        = "scatter";
                    subEventItem["wincash"]     = resultContext["scatterwins"][j]["wincash"];
                    subEventItem["symbol"]      = SymbolIdStringForLog[2].name;
                    subEventItem["multiplier"]  = resultContext["scatterwins"][j]["multiplier"];
                    JArray scaterArray = new JArray();
                    for (int k = 0; k < resultContext["scatterwins"][j]["winningwindows"].Count; k++)
                    {
                        JArray scatterItem = new JArray();
                        scatterItem.Add(resultContext["scatterwins"][j]["winningwindows"][k]["reelindex"]);
                        scatterItem.Add(resultContext["scatterwins"][j]["winningwindows"][k]["symbolindex"]);
                        scaterArray.Add(scatterItem);
                    }
                    subEventItem["windows"] = scaterArray;
                    subEvents.Add(subEventItem);
                }
            }

            if (subEvents.Count > 0)
                eventItem["subevents"] = subEvents;

            return eventItem;
        }
    }
}
