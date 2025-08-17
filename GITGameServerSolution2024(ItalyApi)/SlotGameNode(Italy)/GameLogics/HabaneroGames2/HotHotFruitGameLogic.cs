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
    public class HotHotFruitGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGHotHotFruit";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "b16d79a7-743d-4565-bbd0-1b4f31bdc544";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "eccfc303cee4775252736f5e95cb46a55f82bf91";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.3609.274";
            }
        }
        protected override float MiniCoin
        {
            get
            {
                return 15.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 15;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,     new HabaneroLogSymbolIDName{id = "idWildDouble",        name = "WildDouble"         } },
                    {2,     new HabaneroLogSymbolIDName{id = "idWild",              name = "Wild"               } },
                    
                    {3,     new HabaneroLogSymbolIDName{id = "id7Triple",           name = "7Triple"            } },
                    {4,     new HabaneroLogSymbolIDName{id = "idBarDouble",         name = "BarDouble"          } },
                    {5,     new HabaneroLogSymbolIDName{id = "idCherryDouble",      name = "CherryDouble"       } },
                    {6,     new HabaneroLogSymbolIDName{id = "idOrangeDouble",      name = "OrangeDouble"       } },
                    {7,     new HabaneroLogSymbolIDName{id = "idWaterMelonDouble",  name = "WaterMelonDouble"   } },
                    
                    {8,     new HabaneroLogSymbolIDName{id = "id7",                 name = "7"                  } },
                    {9,     new HabaneroLogSymbolIDName{id = "idBar",               name = "Bar"                } },
                    {10,    new HabaneroLogSymbolIDName{id = "idCherry",            name = "Cherry"             } },
                    {11,    new HabaneroLogSymbolIDName{id = "idOrange",            name = "Orange"             } },
                    {12,    new HabaneroLogSymbolIDName{id = "idWaterMelon",        name = "WaterMelon"         } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 435;
            }
        }
        #endregion

        public HotHotFruitGameLogic()
        {
            _gameID     = GAMEID.HotHotFruit;
            GameName    = "HotHotFruit";
        }

        protected override JObject buildEventItem(string strGlobalUserId, int currentIndex)
        {
            dynamic response    = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserId].Responses[currentIndex].Response);
            dynamic eventItem   = base.buildEventItem(strGlobalUserId, currentIndex);

            if (!object.ReferenceEquals(response["HotHotFruit_symbolLockMatrix"], null))
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
                            col.Add(SymbolIdStringForLog[symbol - 5].id);
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
            resumeGames[0]["HotHotFruit_symbolLockMatrix"] = lastResult["HotHotFruit_symbolLockMatrix"];
            return resumeGames;
        }

        private JArray buildHabaneroLogLockReels(dynamic response)
        {
            JArray lockReels = new JArray();
            for(int i = 0;i < response["HotHotFruit_symbolLockMatrix"].Count; i++)
            {
                JArray col = new JArray();
                for(int j = 0; j < response["HotHotFruit_symbolLockMatrix"][i].Count; j++)
                {
                    if ((int)response["HotHotFruit_symbolLockMatrix"][i][j] == 0)
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
