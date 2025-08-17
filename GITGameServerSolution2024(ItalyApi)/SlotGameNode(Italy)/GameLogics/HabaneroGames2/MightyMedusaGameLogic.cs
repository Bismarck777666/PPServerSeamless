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
    public class MightyMedusaGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGMightyMedusa";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "c5f8e096-56c5-4d55-9600-fc57777b0669";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "8edb9dbf2097655224b523759544684a55d3e2fc";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.9790.408";
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
                return 466;
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
                    {1,   new HabaneroLogSymbolIDName{id = "idWildMedusa",      name = "WildMedusa"         } },    //메두사와일드
                    {2,   new HabaneroLogSymbolIDName{id = "idWildSnakeHead",   name = "WildSnakeHead"      } },    //뱀대가리와일드
                    {3,   new HabaneroLogSymbolIDName{id = "idWildStonePerseus",name = "WildStonePerseus"   } },    //돌상페르세우스와일드
                    {4,   new HabaneroLogSymbolIDName{id = "idScatter",         name = "Scatter"            } },    //스캐터
                    {5,   new HabaneroLogSymbolIDName{id = "idPegasus",         name = "Pegasus"            } },    //페가수스(말)
                    {6,   new HabaneroLogSymbolIDName{id = "idPerseus",         name = "Perseus"            } },    //페르세우스(무사)
                    {7,   new HabaneroLogSymbolIDName{id = "idWingedSandals",   name = "WingedSandals"      } },    //신발
                    {8,   new HabaneroLogSymbolIDName{id = "idHelmet",          name = "Helmet"             } },    //파도
                    {9,   new HabaneroLogSymbolIDName{id = "idA",               name = "A"                  } },    //A
                    {10,  new HabaneroLogSymbolIDName{id = "idK",               name = "K"                  } },    //K
                    {11,  new HabaneroLogSymbolIDName{id = "idQ",               name = "Q"                  } },    //Q
                    {12,  new HabaneroLogSymbolIDName{id = "idJ",               name = "J"                  } },    //J
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 628;
            }
        }
        #endregion

        public MightyMedusaGameLogic()
        {
            _gameID     = GAMEID.MightyMedusa;
            GameName    = "MightyMedusa";
        }

        protected override JObject buildEventItem(string strGlobalUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strGlobalUserId, currentIndex);
            
            HabaneroHistoryResponses responses = _dicUserHistory[strGlobalUserId].Responses[currentIndex];
            dynamic response    = JsonConvert.DeserializeObject<dynamic>(responses.Response);
            JArray reels        = buildHabaneroLogReels(strGlobalUserId, currentIndex, response as JObject);
            eventItem["reels"]  = reels;
            
            if (!object.ReferenceEquals(response["currentfreegame"], null))
                eventItem["multiplier"] = 1;

            if (!object.ReferenceEquals(response["MightyMedusa_FGMessage"], null))
            {
                JObject metaObj     = buildMeta(response["MightyMedusa_FGMessage"]);
                eventItem["meta"]   = metaObj;
            }

            bool hasReelList = false;
            for(int i = 0; i < response["reels"].Count; i++)
            {
                for(int j = 0; j < response["reels"][i].Count; j++)
                {
                    int symbol = (int)response["reels"][i][j]["symbolid"];
                    if (symbol == 2 || symbol == 3)
                    {
                        hasReelList = true;
                        break;
                    }
                }
                if (hasReelList)
                    break;
            }

            if (hasReelList)
            {
                JArray reelslist        = buildHabaneroLogReelslist(response as JObject);
                eventItem["reelslist"]  = reelslist;
            }

            return eventItem;
        }
        protected override JArray buildHabaneroLogReels(string strGlobalUserId,int currentIndex ,dynamic response, bool containWild = false)
        {
            dynamic currentResult   = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserId].Responses[currentIndex].Response);

            JArray reels = new JArray();
            for (int i = 0; i < currentResult["reels"].Count; i++)
            {
                JArray col = new JArray();
                for (int j = 0; j < currentResult["reels"][i].Count; j++)
                {
                    int symbol      = Convert.ToInt32(currentResult["reels"][i][j]["symbolid"]);
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
            
            if(object.ReferenceEquals(currentResult["numfreegames"], null))
                return reelslist;
            
            bool hasSnake = false;
            for (int i = 0; i < currentResult["reels"].Count; i++)
            {
                for (int j = 0; j < currentResult["reels"][i].Count; j++)
                {
                    int symbol = (int)currentResult["reels"][i][j]["symbolid"];
                    if (symbol == 2)
                    {
                        hasSnake = true;
                        break;
                    }
                }
                if (hasSnake)
                    break;
            }
            if (!hasSnake)
                return reelslist;

            for (int i = 0; i < currentResult["reels"].Count; i++)
            {
                for (int j = 0; j < currentResult["reels"][i].Count; j++)
                {
                    int symbol = (int)currentResult["reels"][i][j]["symbolid"];
                    if (symbol == 3)
                        reelsListCols[i][j] = SymbolIdStringForLog[6].id;
                    if(symbol == 2)
                        reelsListCols[i][j] = SymbolIdStringForLog[2].id;
                }
            }
            reelslistItem["reels"] = reelsListCols;
            reelslist.Add(reelslistItem);

            return reelslist;
        }

        private JObject buildMeta(dynamic MedusaFG)
        {
            JObject metaObj = new JObject();
            if (!object.ReferenceEquals(MedusaFG["medusaPos"], null))
                metaObj["mp"] = MedusaFG["medusaPos"];

            metaObj["pl"] = new JArray();
            if (!object.ReferenceEquals(MedusaFG["pegasusPosList"], null))
                metaObj["pl"] = MedusaFG["pegasusPosList"];

            return metaObj;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserId, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            JArray initResumeGame = base.buildInitResumeGame(strGlobalUserId, betInfo, lastResult, gameinstanceid, roundid, currentAction);
            dynamic resultContext = lastResult;
            JArray virtualreels = lastResult["virtualreels"] as JArray;
            for (int i = 0; i < resultContext["reels"].Count; i++)
            {
                for(int j = 0; j < resultContext["reels"][i].Count; j++)
                    virtualreels[i][j + 2] = resultContext["reels"][i][j]["symbolid"];
            }
            if (!object.ReferenceEquals(resultContext["MightyMedusa_FGMessage"], null) && !object.ReferenceEquals(resultContext["MightyMedusa_FGMessage"]["pegasusPosList"], null))
            {
                for (int i = 0; i < resultContext["MightyMedusa_FGMessage"]["pegasusPosList"].Count; i++)
                {
                    int col = (int)resultContext["MightyMedusa_FGMessage"]["pegasusPosList"][i]["reelindex"];
                    int row = (int)resultContext["MightyMedusa_FGMessage"]["pegasusPosList"][i]["symbolindex"];
                    virtualreels[col][row + 2] = 105;
                }
            }

            initResumeGame[0]["virtualreels"] = virtualreels;
            return initResumeGame;
        }
    }
}
