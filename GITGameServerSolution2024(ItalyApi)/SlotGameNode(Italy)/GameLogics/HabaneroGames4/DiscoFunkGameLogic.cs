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
    public class DiscoFunkGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGDiscoFunk";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "ba953396-5a4a-4172-b2a7-1fd98766fe80";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "b1228a6e2518e75302c6c0132465dd28cf523d1f";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idDivaDancer1",     name = "DivaDancer1"    } },
                    {2,   new HabaneroLogSymbolIDName{id = "idDivaDancer2",     name = "DivaDancer2"    } },
                    {3,   new HabaneroLogSymbolIDName{id = "idMirrorBall",      name = "MirrorBall"     } },
                    {4,   new HabaneroLogSymbolIDName{id = "idCar",             name = "Car"            } },
                    {5,   new HabaneroLogSymbolIDName{id = "idGuy",             name = "Guy"            } },
                    {6,   new HabaneroLogSymbolIDName{id = "idRecord",          name = "Record"         } },
                    {7,   new HabaneroLogSymbolIDName{id = "idShoes",           name = "Shoes"          } },
                    {8,   new HabaneroLogSymbolIDName{id = "idLamp",            name = "Lamp"           } },
                    {9,   new HabaneroLogSymbolIDName{id = "idA",               name = "A"              } },
                    {10,  new HabaneroLogSymbolIDName{id = "idK",               name = "K"              } },
                    {11,  new HabaneroLogSymbolIDName{id = "idQ",               name = "Q"              } },
                    {12,  new HabaneroLogSymbolIDName{id = "idJ",               name = "J"              } },
                    {13,  new HabaneroLogSymbolIDName{id = "id10",              name = "10"             } },
                    {14,  new HabaneroLogSymbolIDName{id = "id9",               name = "9"              } },
                    {15,  new HabaneroLogSymbolIDName{id = "idDisco",           name = "Disco"          } },
                    {16,  new HabaneroLogSymbolIDName{id = "idFunk",            name = "Funk"           } },

                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 64;
            }
        }
        #endregion

        public DiscoFunkGameLogic()
        {
            _gameID     = GAMEID.DiscoFunk;
            GameName    = "DiscoFunk";
        }

        protected override JArray buildHabaneroLogReels(string strUserId, int currentIndex, dynamic response, bool containWild = false)
        {
            JArray reels = base.buildHabaneroLogReels(strUserId, currentIndex, response as JObject, containWild);
            if (object.ReferenceEquals(response["expandingwilds"], null) || response["expandingwilds"].Count == 0)
                return reels;

            for (int i = 0; i < response["expandingwilds"].Count; i++)
            {
                int reelindex   = (int)response["expandingwilds"][i]["reelindex"];
                int symbolid    = (int)response["expandingwilds"][i]["symbolid"];

                for (int j = 0; j < 3; j++)
                {
                    reels[reelindex][j] = SymbolIdStringForLog[symbolid].id;
                    reels[reelindex][j] = SymbolIdStringForLog[symbolid].id;
                }
            }
            return reels;
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);

            dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strUserId].Responses[currentIndex].Response);

            if ((string)eventItem["gamemode"] == "FREEGAME")
            {
                int multiplier = 1;
                if (!object.ReferenceEquals(resultContext["reels"], null))
                {
                    if (object.ReferenceEquals(eventItem["subevents"], null))
                        eventItem["subevents"] = new JArray();

                    JArray reelArray = resultContext["reels"];
                    for (int i = 0; i < reelArray.Count; i++)
                    {
                        JArray reelRow = reelArray[i] as JArray;
                        for (int j = 0; j < reelRow.Count; j++)
                        {
                            if (!object.ReferenceEquals(reelRow[j]["wincash"], null))
                            {
                                JObject subEventItem = new JObject();
                                subEventItem["type"]                = "scatter";
                                subEventItem["wincash"]             = reelRow[j]["wincash"];
                                subEventItem["winfreegames"]        = 0;
                                subEventItem["wincashmultiplier"]   = reelRow[j]["wincashmultiplier"];
                                int symbolid = Convert.ToInt32(reelRow[j]["symbolid"]);
                                subEventItem["symbol"]              = SymbolIdStringForLog[symbolid].name;

                                JArray lineWinArray = new JArray();
                                JArray lineWinItem  = new JArray();
                                lineWinItem.Add(i);
                                lineWinItem.Add(j);
                                lineWinArray.Add(lineWinItem);
                            
                                subEventItem["windows"]             = lineWinArray;
                                subEventItem["multiplier"]          = 1;
                                (eventItem["subevents"] as JArray).Add(subEventItem);
                            }
                        }
                    }
                }
            
                eventItem["multiplier"] = multiplier;
            }
            return eventItem;
        }
    }
}
