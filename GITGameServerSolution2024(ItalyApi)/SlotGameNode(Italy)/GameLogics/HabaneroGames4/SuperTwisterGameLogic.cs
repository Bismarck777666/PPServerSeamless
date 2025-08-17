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
    public class SuperTwisterGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGSuperTwister";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "0b6e5aee-09fe-49f9-aadd-c39f38d197bf";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "dff04cdabb4339c07ed7e87c517e2d7f04d366d8";
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
                    {1,     new HabaneroLogSymbolIDName{id = "idTwister",   name = "Twister"    } },
                    {2,     new HabaneroLogSymbolIDName{id = "idBarn",      name = "Barn"       } },
                    {3,     new HabaneroLogSymbolIDName{id = "idSheep",     name = "Sheep"      } },
                    {4,     new HabaneroLogSymbolIDName{id = "idGoose",     name = "Goose"      } },
                    {5,     new HabaneroLogSymbolIDName{id = "idFarmer",    name = "Farmer"     } },
                    {6,     new HabaneroLogSymbolIDName{id = "idFarmerWife",name = "FarmerWife" } },
                    {7,     new HabaneroLogSymbolIDName{id = "idCow",       name = "Cow"        } },
                    {8,     new HabaneroLogSymbolIDName{id = "idDog",       name = "Dog"        } },
                    {9,     new HabaneroLogSymbolIDName{id = "idTractor",   name = "Tractor"    } },
                    {10,    new HabaneroLogSymbolIDName{id = "idTruck",     name = "Truck"      } },
                    {11,    new HabaneroLogSymbolIDName{id = "idWindmill",  name = "Windmill"   } },
                    {12,    new HabaneroLogSymbolIDName{id = "idBucket",    name = "Bucket"     } },
                    {13,    new HabaneroLogSymbolIDName{id = "idHay",       name = "Hay"        } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 262;
            }
        }
        #endregion

        public SuperTwisterGameLogic()
        {
            _gameID     = GAMEID.SuperTwister;
            GameName    = "SuperTwister";
        }

        protected override JArray buildHabaneroLogReels(string strUserId, int currentIndex, dynamic response, bool containWild = false)
        {
            JArray reels = base.buildHabaneroLogReels(strUserId, currentIndex, response as JObject, containWild);
            if (!ReferenceEquals(response["expandingwilds"], null) && response["expandingwilds"].Count > 0)
            {
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
            }

            if (!ReferenceEquals(response["st_twisterindices"], null) && response["st_twisterindices"].Count > 0)
            {
                for (int i = 0; i < response["st_twisterindices"].Count; i++)
                {
                    int reelindex   = (int)response["st_twisterindices"][i];

                    for (int j = 0; j < 3; j++)
                    {
                        reels[reelindex][j] = SymbolIdStringForLog[1].id;
                        reels[reelindex][j] = SymbolIdStringForLog[1].id;
                    }
                }
            }

            return reels;
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);
            
            if ((string)eventItem["gamemode"] == "FREEGAME")
                eventItem["multiplier"] = 1;

            dynamic eventContext = eventItem as dynamic;
            if (!object.ReferenceEquals(eventContext["subevents"], null) && eventContext["subevents"].Count > 0)
            {
                for (int i = 0; i < eventContext["subevents"].Count; i++)
                {
                    if (eventContext["subevents"][i]["type"] == "scatter")
                    {
                        eventContext["subevents"][i]["symbol"] = SymbolIdStringForLog[2].name;
                    }
                }
            }
            return eventItem;
        }

        protected override JArray buildInitResumeGame(string strUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            JArray resumeGames = base.buildInitResumeGame(strUserID, betInfo, lastResult, gameinstanceid, roundid, currentAction);

            dynamic response = lastResult as dynamic;
            if (!ReferenceEquals(response["expandingwilds"], null) && response["expandingwilds"].Count > 0)
            {
                for (int i = 0; i < response["expandingwilds"].Count; i++)
                {
                    int reelindex   = (int)response["expandingwilds"][i]["reelindex"];
                    int symbolid    = (int)response["expandingwilds"][i]["symbolid"];

                    for (int j = 0; j < 3; j++)
                    {
                        resumeGames[0]["virtualreels"][reelindex][j] = symbolid;
                    }
                }
            }

            if (!ReferenceEquals(response["st_twisterindices"], null) && response["st_twisterindices"].Count > 0)
            {
                for (int i = 0; i < response["st_twisterindices"].Count; i++)
                {
                    int reelindex   = (int)response["st_twisterindices"][i];

                    for (int j = 0; j < 3; j++)
                    {
                        resumeGames[0]["virtualreels"][reelindex][j + 2] = 1;
                    }
                }
            }

            return resumeGames;
        }
    }
}
