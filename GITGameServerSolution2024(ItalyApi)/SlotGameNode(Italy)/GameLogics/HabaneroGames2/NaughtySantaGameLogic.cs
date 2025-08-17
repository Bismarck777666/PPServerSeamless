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
    public class NaughtySantaGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGNaughtySanta";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "b1bf49ea-369f-4ce5-be04-5eb1780980ce";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "0e212b4f394c2aac97198e91d3c2a32708c03a29";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.4966.317";
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
                return 432;
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
                    {1,   new HabaneroLogSymbolIDName{id = "idSanta",           name = "Santa"          } },    
                    {2,   new HabaneroLogSymbolIDName{id = "idScatter",         name = "Scatter"        } },    
                    {3,   new HabaneroLogSymbolIDName{id = "idGirl1",           name = "Girl1"          } },    
                    {4,   new HabaneroLogSymbolIDName{id = "idGirl2",           name = "Girl2"          } },    
                    {5,   new HabaneroLogSymbolIDName{id = "idGirl3",           name = "Girl3"          } },    
                    {6,   new HabaneroLogSymbolIDName{id = "idGirl4",           name = "Girl4"          } },    
                    {7,   new HabaneroLogSymbolIDName{id = "idCracker1",        name = "Cracker1"       } },    
                    {8,   new HabaneroLogSymbolIDName{id = "idCracker2",        name = "Cracker2"       } },    
                    {9,   new HabaneroLogSymbolIDName{id = "idCracker3",        name = "Cracker3"       } },    
                    {10,  new HabaneroLogSymbolIDName{id = "idCracker4",        name = "Cracker4"       } },
                    {11,  new HabaneroLogSymbolIDName{id = "idSantaExpand1",    name = "SantaExpand1"   } },
                    {12,  new HabaneroLogSymbolIDName{id = "idSantaExpand2",    name = "SantaExpand2"   } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 488;
            }
        }
        #endregion

        public NaughtySantaGameLogic()
        {
            _gameID     = GAMEID.NaughtySanta;
            GameName    = "NaughtySanta";
        }

        protected override JObject buildEventItem(string strGlobalUserId, int currentIndex)
        {
            dynamic eventItem = base.buildEventItem(strGlobalUserId, currentIndex);
            dynamic response = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserId].Responses[currentIndex].Response);
            eventItem["multiplier"] = response["thisGameMultiplier"];

            if (!object.ReferenceEquals(response["expandingwilds"], null) && response["expandingwilds"].Count > 0)
            {
                JArray reelsList = buildHabaneroLogReelslist(response);
                eventItem["reelslist"] = reelsList;
            }

            return eventItem;
        }

        protected override JArray buildHabaneroLogReels(string strGlobalUserId,int currentIndex ,dynamic response, bool containWild = false)
        {
            JArray logReels = new JArray();
            for (int i = 0; i < response["virtualreels"].Count; i++)
            {
                JArray Col = new JArray();
                for (int j = 2; j < response["virtualreels"][i].Count - 2; j++)
                {
                    int symbol = (int)response["virtualreels"][i][j];
                    Col.Add(SymbolIdStringForLog[symbol].id);
                }
                logReels.Add(Col);
            }

            for (int i = 0; i < response["reels"].Count; i++)
            {
                for (int j = 0; j < response["reels"][i].Count; j++)
                {
                    if(!object.ReferenceEquals(response["reels"][i][j]["meta"],null))
                    {
                        if (!object.ReferenceEquals(response["reels"][i][j]["meta"]["iswild"], null))
                        {
                            int symbol = (int)response["reels"][i][j]["symbolid"];
                            logReels[i][j] = string.Format("{0}-Wild", SymbolIdStringForLog[symbol].id);
                        }

                        if (!object.ReferenceEquals(response["reels"][i][j]["meta"]["bigsymboltype"], null))
                        {
                            int bigSymbolType = (int)response["reels"][i][j]["meta"]["bigsymboltype"];
                            int bigSymbolSize = (int)response["reels"][i][j]["meta"]["bigsymbolsize"];
                            for(int ii = 0; ii < bigSymbolSize; ii++)
                            {
                                for(int jj = 0; jj < bigSymbolSize; jj++)
                                {
                                    if(ii == 0 && jj == 0)
                                        logReels[i + ii][j + jj] = string.Format("{0}-{1}", SymbolIdStringForLog[bigSymbolType].id, bigSymbolSize);
                                    else
                                        logReels[i + ii][j + jj] = null;
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < response["reels"].Count; i++)
            {
                int expandNum = 0;
                for (int j = 0; j < response["reels"][i].Count; j++)
                {
                    int symbol = Convert.ToInt32(response["reels"][i][j]["symbolid"]);
                    if (symbol > 10)
                    {
                        expandNum = symbol;
                        break;
                    }
                }

                for (int j = 0; j < response["reels"][i].Count; j++)
                {
                    if (expandNum > 0 && j == 0)
                        logReels[i][j] = SymbolIdStringForLog[expandNum].id;
                    else if (expandNum > 0 && j != 0)
                        logReels[i][j] = null;
                }
            }
            return logReels;
        }

        protected override JArray buildHabaneroLogReelslist(dynamic response)
        {
            JArray reelsList        = new JArray();
            JObject reelsListItem   = new JObject();
            JArray reels            = new JArray();

            for(int i = 0; i < response["virtualreels"].Count; i++)
            {
                JArray col = new JArray();
                for (int j = 2; j < response["virtualreels"][i].Count - 2; j++)
                {
                    int symbolid = (int)response["virtualreels"][i][j];
                    if (symbolid > 10)
                        symbolid = 1;
                    col.Add(SymbolIdStringForLog[symbolid].id);
                }
                reels.Add(col);
            }

            for (int i = 0; i < response["reels"].Count; i++)
            {
                for (int j = 0; j < response["reels"][i].Count; j++)
                {
                    if (!object.ReferenceEquals(response["reels"][i][j]["meta"], null))
                    {
                        if (!object.ReferenceEquals(response["reels"][i][j]["meta"]["bigsymboltype"], null))
                        {
                            int bigSymbolType = (int)response["reels"][i][j]["meta"]["bigsymboltype"];
                            int bigSymbolSize = (int)response["reels"][i][j]["meta"]["bigsymbolsize"];
                            for (int ii = 0; ii < bigSymbolSize; ii++)
                            {
                                for (int jj = 0; jj < bigSymbolSize; jj++)
                                {
                                    if (ii == 0 && jj == 0)
                                        reels[i + ii][j + jj] = string.Format("{0}-{1}", SymbolIdStringForLog[bigSymbolType].id, bigSymbolSize);
                                    else
                                        reels[i + ii][j + jj] = null;
                                }
                            }
                        }
                    }
                }
            }

            reelsListItem["reels"]  = reels;
            reelsList.Add(reelsListItem);
            return reelsList;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserId, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            dynamic response        = lastResult as dynamic;
            dynamic resumeGames     = base.buildInitResumeGame(strGlobalUserId, betInfo, lastResult, gameinstanceid, roundid, currentAction);
            string strResumeInfo    = string.Empty;
            
            if (!object.ReferenceEquals(lastResult["thisGameMultiplier"], null))
                strResumeInfo = string.Format("4:{0};", lastResult["thisGameMultiplier"]);

            List<string> wildCrackers = new List<string>();
            List<string> giantSymbols = new List<string>();
            List<string> santaExpands = new List<string>();
            for (int i = 0; i < response["reels"].Count; i++)
            {
                for (int j = 0; j < response["reels"][i].Count; j++)
                {
                    if (!object.ReferenceEquals(response["reels"][i][j]["meta"], null))
                    {
                        if (!object.ReferenceEquals(response["reels"][i][j]["meta"]["iswild"], null))
                        {
                            int symbol = (int)response["reels"][i][j]["symbolid"];
                            string wildCracker = string.Format("1:{0},{1},{2};", i, j, symbol);
                            wildCrackers.Add(wildCracker);
                        }

                        if (!object.ReferenceEquals(response["reels"][i][j]["meta"]["bigsymboltype"], null))
                        {
                            int bigSymbolType = (int)response["reels"][i][j]["meta"]["bigsymboltype"];
                            int bigSymbolSize = (int)response["reels"][i][j]["meta"]["bigsymbolsize"];

                            string giantSymbol = string.Format("2:{0},{1},{2},{3};", i, j, bigSymbolType, bigSymbolSize);
                            giantSymbols.Add(giantSymbol);
                        }
                    }
                }
            }

            for (int i = 0; i < response["reels"].Count; i++)
            {
                for (int j = 0; j < response["reels"][i].Count; j++)
                {
                    int symbol = Convert.ToInt32(response["reels"][i][j]["symbolid"]);
                    if (symbol > 10)
                    {
                        string santaExapnd = string.Format("3:{0},{1};", i, symbol);
                        santaExpands.Add(santaExapnd);
                        break;
                    }
                }
            }

            if (wildCrackers.Count > 0)
                strResumeInfo = string.Format("{0}{1}", string.Join("", wildCrackers.ToArray()), strResumeInfo);
            if (giantSymbols.Count > 0)
                strResumeInfo = string.Format("{0}{1}", string.Join("", giantSymbols.ToArray()), strResumeInfo);
            if (santaExpands.Count > 0)
                strResumeInfo = string.Format("{0}{1}", string.Join("", santaExpands.ToArray()), strResumeInfo);

            resumeGames[0]["NaughtySanta_resumeInfo"] = strResumeInfo;
            return resumeGames;
        }
    }
}
