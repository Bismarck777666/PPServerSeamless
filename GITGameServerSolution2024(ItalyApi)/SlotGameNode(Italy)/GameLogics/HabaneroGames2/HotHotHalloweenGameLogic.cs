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
    public class HotHotHalloweenGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGHotHotHalloween";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "1ef80a2f-2846-4d56-b67e-edb4faa2b1b7";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "bdba39ebec4ed54c095cd075c153a1b0514ae2dd";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.4702.314";
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
                return 5;
            }
        }
        protected override string BetType 
        {
            get
            {
                return "HorizontalPays";
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,     new HabaneroLogSymbolIDName{id = "idWildDouble",    name = "WildDouble"     } },
                    {2,     new HabaneroLogSymbolIDName{id = "idWild",          name = "Wild"           } },
                    
                    {3,     new HabaneroLogSymbolIDName{id = "id7Triple",       name = "7Triple"        } },
                    {4,     new HabaneroLogSymbolIDName{id = "idCoffinDouble",  name = "CoffinDouble"   } },
                    {5,     new HabaneroLogSymbolIDName{id = "idEyeDouble",     name = "EyeDouble"      } },
                    {6,     new HabaneroLogSymbolIDName{id = "idGhostDouble",   name = "GhostDouble"    } },
                    {7,     new HabaneroLogSymbolIDName{id = "idHandDouble",    name = "HandDouble"     } },
                    {8,     new HabaneroLogSymbolIDName{id = "idStarDouble",    name = "StarDouble"     } },
                    {9,     new HabaneroLogSymbolIDName{id = "idCandy1Double",  name = "Candy1Double"   } },
                    {10,    new HabaneroLogSymbolIDName{id = "idCandy2Double",  name = "Candy2Double"   } },
                    {11,    new HabaneroLogSymbolIDName{id = "idCandy3Double",  name = "Candy3Double"   } },
                    {12,    new HabaneroLogSymbolIDName{id = "idCandy4Double",  name = "Candy4Double"   } },
                    
                    {13,    new HabaneroLogSymbolIDName{id = "id7",             name = "7"              } },
                    {14,    new HabaneroLogSymbolIDName{id = "idCoffin",        name = "Coffin"         } },
                    {15,    new HabaneroLogSymbolIDName{id = "idEye",           name = "Eye"            } },
                    {16,    new HabaneroLogSymbolIDName{id = "idGhost",         name = "Ghost"          } },
                    {17,    new HabaneroLogSymbolIDName{id = "idHand",          name = "Hand"           } },
                    {18,    new HabaneroLogSymbolIDName{id = "idStar",          name = "Star"           } },
                    {19,    new HabaneroLogSymbolIDName{id = "idCandy1",        name = "Candy1"         } },
                    {20,    new HabaneroLogSymbolIDName{id = "idCandy2",        name = "Candy2"         } },
                    {21,    new HabaneroLogSymbolIDName{id = "idCandy3",        name = "Candy3"         } },
                    {22,    new HabaneroLogSymbolIDName{id = "idCandy4",        name = "Candy4"         } }, 
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 470;
            }
        }
        #endregion

        public HotHotHalloweenGameLogic()
        {
            _gameID     = GAMEID.HotHotHalloween;
            GameName    = "HotHotHalloween";
        }

        protected override JObject buildEventItem(string strGlobalUserId, int currentIndex)
        {
            dynamic response    = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserId].Responses[currentIndex].Response);
            dynamic eventItem   = base.buildEventItem(strGlobalUserId, currentIndex);
            if(!object.ReferenceEquals(eventItem["subevents"],null) && eventItem["subevents"].Count > 0)
            {
                for(int i = 0; i < eventItem["subevents"].Count; i++)
                    eventItem["subevents"][i]["type"] = "horizontalpay";
            }

            if (!object.ReferenceEquals(response["HotHotHalloween_symbolLockMatrix"], null))
            {
                JArray reels10 = buildHabaneroLogLockReels(response);
                eventItem["reels10"] = reels10;
            }

            bool hasMeta = false;
            for(int i = 0; i < response["reels"].Count; i++)
            {
                for(int j = 0; j < response["reels"][i].Count; j++)
                {
                    if (!object.ReferenceEquals(response["reels"][i][j]["meta"], null))
                    {
                        hasMeta = true;
                        break;
                    }
                }
                if (hasMeta)
                    break;
            }

            if (hasMeta)
                eventItem["reelslist"] = buildHabaneroLogReelslist(response);

            if (!object.ReferenceEquals(response["currentfreegame"], null))
                eventItem["spinno"] = response["currentfreegame"];

            return eventItem;
        }

        protected override JArray buildHabaneroLogReels(string strGlobalUserId,int currentIndex ,dynamic response, bool containWild = false)
        {
            JArray reels = new JArray();
            for (int i = 0; i < response["reels"].Count; i++)
            {
                JArray col = new JArray();
                for (int j = 0; j < response["reels"][i].Count; j++)
                {
                    int symbol = (int)response["reels"][i][j]["symbolid"];

                    if (!object.ReferenceEquals(response["reels"][i][j]["meta"],null) && !object.ReferenceEquals(response["reels"][i][j]["meta"]["is2"], null))
                    {
                        if(symbol == 2)
                            col.Add(SymbolIdStringForLog[1].id);
                        else
                            col.Add(SymbolIdStringForLog[symbol - 10].id);
                    }
                    else
                        col.Add(SymbolIdStringForLog[(int)response["reels"][i][j]["symbolid"]].id);
                }
                reels.Add(col);
            }
            return reels;
        }

        protected override JArray buildHabaneroLogReelslist(dynamic response)
        {
            JArray reelsList        = new JArray();
            JObject reelsListItem   = new JObject();
            JArray reels            = new JArray();
            for(int i = 0; i < response["reels"].Count; i++)
            {
                JArray col = new JArray();
                for(int j = 0; j < response["reels"][i].Count; j++)
                    col.Add(SymbolIdStringForLog[(int)response["reels"][i][j]["symbolid"]].id);
                reels.Add(col);
            }
            reelsListItem["reels"] = reels;
            reelsList.Add(reelsListItem);
            return reelsList;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserId, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            dynamic resumeGames = base.buildInitResumeGame(strGlobalUserId, betInfo, lastResult, gameinstanceid, roundid, currentAction);
            resumeGames[0]["HotHotHalloween_symbolLockMatrix"] = lastResult["HotHotHalloween_symbolLockMatrix"];
            return resumeGames;
        }

        private JArray buildHabaneroLogLockReels(dynamic response)
        {
            JArray lockReels = new JArray();
            for(int i = 0;i < response["HotHotHalloween_symbolLockMatrix"].Count; i++)
            {
                JArray col = new JArray();
                for(int j = 0; j < response["HotHotHalloween_symbolLockMatrix"][i].Count; j++)
                {
                    if ((int)response["HotHotHalloween_symbolLockMatrix"][i][j] == 0)
                        col.Add(null);
                    else
                        col.Add("idLock");
                }
                lockReels.Add(col);
            }
            return lockReels;
        }

    }
}
