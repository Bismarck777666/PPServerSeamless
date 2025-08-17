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
    public class BombsAwayGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGBombsAway";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "b650d3a0-952b-4535-bb18-47fb4e87e0f5";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "48a62406089c5a62ae065943156c1c4ea2aa412a";
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
                return 50.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 50;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idBomb",        name = "Bomb"       } },
                    {2,   new HabaneroLogSymbolIDName{id = "idWild",        name = "Wild"       } },
                    {3,   new HabaneroLogSymbolIDName{id = "idBombsAway",   name = "BombsAway"  } },
                    {4,   new HabaneroLogSymbolIDName{id = "idPlane",       name = "Plane"      } },
                    {5,   new HabaneroLogSymbolIDName{id = "idTank",        name = "Tank"       } },
                    {6,   new HabaneroLogSymbolIDName{id = "idSoldier",     name = "Soldier"    } },
                    {7,   new HabaneroLogSymbolIDName{id = "idTruck",       name = "Truck"      } },
                    {8,   new HabaneroLogSymbolIDName{id = "idGrenade",     name = "Grenade"    } },
                    {9,   new HabaneroLogSymbolIDName{id = "idBackpack",    name = "Backpack"   } },
                    {10,  new HabaneroLogSymbolIDName{id = "idAce",         name = "Ace"        } },
                    {11,  new HabaneroLogSymbolIDName{id = "idKing",        name = "King"       } },
                    {12,  new HabaneroLogSymbolIDName{id = "idQueen",       name = "Queen"      } },
                    {13,  new HabaneroLogSymbolIDName{id = "idJack",        name = "Jack"       } },
                    {14,  new HabaneroLogSymbolIDName{id = "idTen",         name = "Ten"        } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 282;
            }
        }
        #endregion

        public BombsAwayGameLogic()
        {
            _gameID     = GAMEID.BombsAway;
            GameName    = "BombsAway";
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);

            dynamic resultContext   = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strUserId].Responses[currentIndex].Response);
            dynamic eventContext    = eventItem as dynamic;
            if (!object.ReferenceEquals(eventContext["subevents"], null) && eventContext["subevents"].Count > 0)
            {
                for (int i = 0; i < eventContext["subevents"].Count; i++)
                {
                    if (eventContext["subevents"][i]["type"] == "scatter")
                    {
                        eventContext["subevents"][i]["symbol"] = SymbolIdStringForLog[3].name;
                    }
                }
            }

            if (!object.ReferenceEquals(resultContext["videoslotstate"]["bombsaway"], null))
            {
                if(!object.ReferenceEquals(resultContext["videoslotstate"]["bombsaway"]["bombdroplist"], null))
                {
                    dynamic bombdroplist    = resultContext["videoslotstate"]["bombsaway"]["bombdroplist"];
                    for(int i = 0; i < bombdroplist.Count; i++)
                    {
                        int reelIndex   = Convert.ToInt32(bombdroplist[i]["position"][0]);
                        int symbolIndex = Convert.ToInt32(bombdroplist[i]["position"][1]);
                        int symbolId    = Convert.ToInt32(bombdroplist[i]["symbolid"]);

                        eventItem["reels"][reelIndex][symbolIndex] = SymbolIdStringForLog[symbolId].id;
                    }
                }

                if(!object.ReferenceEquals(resultContext["videoslotstate"]["bombsaway"]["explodingwildlist"], null))
                {
                    dynamic explodingwildlist   = resultContext["videoslotstate"]["bombsaway"]["explodingwildlist"];
                    for (int i = 0; i < explodingwildlist.Count; i++)
                    {
                        int reelIndex   = Convert.ToInt32(explodingwildlist[i]["position"][0]);
                        int symbolIndex = Convert.ToInt32(explodingwildlist[i]["position"][1]);
                        int symbolId    = Convert.ToInt32(explodingwildlist[i]["symbolid"]);

                        eventItem["reels"][reelIndex][symbolIndex] = SymbolIdStringForLog[symbolId].id;
                    }
                }
            }

            return eventItem;
        }

        protected override JArray buildInitResumeGame(string strUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction)
        {
            JArray resumeGames = base.buildInitResumeGame(strUserID, betInfo, lastResult, gameinstanceid, roundid, currentAction);

            dynamic resultContext = lastResult;
            if (!object.ReferenceEquals(resultContext["videoslotstate"]["bombsaway"], null))
            {
                if(!object.ReferenceEquals(resultContext["videoslotstate"]["bombsaway"]["bombdroplist"], null))
                {
                    dynamic bombdroplist    = resultContext["videoslotstate"]["bombsaway"]["bombdroplist"];
                    for(int i = 0; i < bombdroplist.Count; i++)
                    {
                        int reelIndex   = Convert.ToInt32(bombdroplist[i]["position"][0]);
                        int symbolIndex = Convert.ToInt32(bombdroplist[i]["position"][1]);
                        int symbolId    = Convert.ToInt32(bombdroplist[i]["symbolid"]);
                        resumeGames[0]["states"][0]["virtualreels"][reelIndex][symbolIndex + 2] = symbolId;
                    }
                }

                if(!object.ReferenceEquals(resultContext["videoslotstate"]["bombsaway"]["explodingwildlist"], null))
                {
                    dynamic explodingwildlist   = resultContext["videoslotstate"]["bombsaway"]["explodingwildlist"];
                    for (int i = 0; i < explodingwildlist.Count; i++)
                    {
                        int reelIndex   = Convert.ToInt32(explodingwildlist[i]["position"][0]);
                        int symbolIndex = Convert.ToInt32(explodingwildlist[i]["position"][1]);
                        int symbolId    = Convert.ToInt32(explodingwildlist[i]["symbolid"]);
                        resumeGames[0]["states"][0]["virtualreels"][reelIndex][symbolIndex + 2] = symbolId;
                    }
                }
            }

            return resumeGames;
        }
    }
}
