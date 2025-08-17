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
    public class TheDeadEscapeGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGTheDeadEscape";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "05c5d441-c3b1-45ac-a2d6-bbcfa4d6bf70";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "d106eaeeb7ffccf3646966d8b3c14caaf633b01c";
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
                return 30.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 30;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idZombie",      name = "Zombie"     } },
                    {2,   new HabaneroLogSymbolIDName{id = "idVehicle",     name = "Vehicle"    } },
                    {3,   new HabaneroLogSymbolIDName{id = "idFather",      name = "Father"     } },
                    {4,   new HabaneroLogSymbolIDName{id = "idDaughter",    name = "Daughter"   } },
                    {5,   new HabaneroLogSymbolIDName{id = "idToxicSign",   name = "ToxicSign"  } },
                    {6,   new HabaneroLogSymbolIDName{id = "idHand",        name = "Hand"       } },
                    {7,   new HabaneroLogSymbolIDName{id = "idAce",         name = "Ace"        } },
                    {8,   new HabaneroLogSymbolIDName{id = "idKing",        name = "King"       } },
                    {9,   new HabaneroLogSymbolIDName{id = "idQueen",       name = "Queen"      } },
                    {10,  new HabaneroLogSymbolIDName{id = "idJack",        name = "Jack"       } },
                    {11,  new HabaneroLogSymbolIDName{id = "idTen",         name = "Ten"        } },
                    {12,  new HabaneroLogSymbolIDName{id = "idNine",        name = "Nine"       } },
                    {13,  new HabaneroLogSymbolIDName{id = "idFuelTank",    name = "FuelTank"   } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 337;
            }
        }
        #endregion

        public TheDeadEscapeGameLogic()
        {
            _gameID     = GAMEID.TheDeadEscape;
            GameName    = "TheDeadEscape";
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);
            HabaneroHistoryResponses responses = _dicUserHistory[strUserId].Responses[currentIndex];
            dynamic response = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            if (!object.ReferenceEquals(response["videoslotstate"]["expandingwildlist"], null) && response["videoslotstate"]["expandingwildlist"].Count > 0)
            {
                for(int i = 0; i < response["videoslotstate"]["expandingwildlist"].Count; i++)
                {
                    int reelIndex   = response["videoslotstate"]["expandingwildlist"][i]["reelindex"];
                    int symbolId    = response["videoslotstate"]["expandingwildlist"][i]["symbolid"];

                    string symbol = SymbolIdStringForLog[symbolId].id;
                    for(int j = 0; j < 3; j++)
                    {
                        eventItem["reels"][reelIndex][j] = symbol;
                    }
                }
            }

            if (!object.ReferenceEquals(response["videoslotstate"]["animatesymbollist"], null) && response["videoslotstate"]["animatesymbollist"].Count > 0)
            {
                for (int i = 0; i < response["videoslotstate"]["animatesymbollist"].Count; i++)
                {
                    if(response["videoslotstate"]["animatesymbollist"][i]["type"] == "humanWin")
                        continue;

                    int x = Convert.ToInt32(response["videoslotstate"]["animatesymbollist"][i]["x"]);
                    int y = Convert.ToInt32(response["videoslotstate"]["animatesymbollist"][i]["y"]);
                    eventItem["reels"][x][y] = SymbolIdStringForLog[1].id;
                }
            }

            JArray customSubEvents      = new JArray();

            if (!object.ReferenceEquals(response["videoslotstate"]["animatesymbollist"], null) && response["videoslotstate"]["animatesymbollist"].Count > 0)
            {
                JObject customSubEventItem = new JObject();
                string customEventData = buildHabaneroCustomData(response);
                customSubEventItem["type"] = "DeadEscapeDuel";
                customSubEventItem["data"] = customEventData;
                customSubEvents.Add(customSubEventItem);
                eventItem["customsubevents"] = customSubEvents;
            }

            return eventItem;
        }

        private string buildHabaneroCustomData(dynamic response)
        {
            JObject data    = new JObject();
            JArray di       = new JArray();
            for(int i = 0; i < response["videoslotstate"]["animatesymbollist"].Count; i++)
            {
                JObject diItem = new JObject();

                int     x           = Convert.ToInt32(response["videoslotstate"]["animatesymbollist"][i]["x"]);
                int     y           = Convert.ToInt32(response["videoslotstate"]["animatesymbollist"][i]["y"]);
                string  type        = Convert.ToString(response["videoslotstate"]["animatesymbollist"][i]["type"]);
            
                dynamic animData    = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(response["videoslotstate"]["animatesymbollist"][i]["data"]));
                string direction    = Convert.ToString(animData["direction"]);

                diItem["hx"] = x;
                diItem["hy"] = y;
                diItem["zy"] = y;
                if (direction == "e")
                    diItem["zx"] = x + 1;
                else //"w"
                    diItem["zx"] = x - 1;
                diItem["do"] = type;

                JArray reels = buildCustomDataReels(response);
                diItem["reel"] = reels;
                di.Add(JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(diItem)));
            }

            for (int i = 0; i < response["videoslotstate"]["animatesymbollist"].Count; i++)
            {
                for(int j = 0; j < i; j++)
                {
                    int     x           = Convert.ToInt32(response["videoslotstate"]["animatesymbollist"][j]["x"]);
                    int     y           = Convert.ToInt32(response["videoslotstate"]["animatesymbollist"][j]["y"]);
                    string  type        = Convert.ToString(response["videoslotstate"]["animatesymbollist"][j]["type"]);
            
                    dynamic animData    = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(response["videoslotstate"]["animatesymbollist"][j]["data"]));
                    string direction    = Convert.ToString(animData["direction"]);
                    if (type == "humanWin")
                    {
                        if(direction == "e")
                        {
                            for(int k = 0; k < 3; k++)
                                di[i]["reel"][x + 1][k] = SymbolIdStringForLog[1].id;

                        }
                        else //"w"
                        {
                            for (int k = 0; k < 3; k++)
                                di[i]["reel"][x - 1][k] = SymbolIdStringForLog[1].id;
                        }
                    }
                    else if (type == "humanLose")
                    {
                        di[i]["reel"][x][y] = SymbolIdStringForLog[1].id;
                    }
                }
            }

            data["di"] = di;
            return JsonConvert.SerializeObject(data);
        }

        private JArray buildCustomDataReels(dynamic response)
        {
            JArray reels = new JArray();
            for (int j = 0; j < response["videoslotstate"]["virtualreellist"].Count; j++)
            {
                JArray col = new JArray();
                for (int k = 2; k < response["videoslotstate"]["virtualreellist"][j].Count - 2; k++)
                {
                    int symbol      = Convert.ToInt32(response["videoslotstate"]["virtualreellist"][j][k]);
                    string symbolid = SymbolIdStringForLog[symbol].id;
                    col.Add(symbolid);
                }
                reels.Add(col);
            }
            return reels;
        }
    }
}
