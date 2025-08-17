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
    public class TheBigDealDeluxeGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGTheBigDealDeluxe";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "f1a56374-0a35-4ce6-a847-302751f47c66";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "76a9059b9a4eca7d67dbc8841444cce60e2d0876";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.11241.0";
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
                    {1,     new HabaneroLogSymbolIDName{id = "idWild",          name = "Wild"           } },
                    {2,     new HabaneroLogSymbolIDName{id = "idScatter",       name = "Scatter"        } },
                    {3,     new HabaneroLogSymbolIDName{id = "idCasino",        name = "Casino"         } },
                    {4,     new HabaneroLogSymbolIDName{id = "idGirl",          name = "Girl"           } },
                    {5,     new HabaneroLogSymbolIDName{id = "idGuard",         name = "Guard"          } },
                    {6,     new HabaneroLogSymbolIDName{id = "idCar",           name = "Car"            } },
                    {7,     new HabaneroLogSymbolIDName{id = "idCash",          name = "Cash"           } },
                    {8,     new HabaneroLogSymbolIDName{id = "idChips",         name = "Chips"          } },
                    {9,     new HabaneroLogSymbolIDName{id = "idWatch",         name = "Watch"          } },
                    {10,    new HabaneroLogSymbolIDName{id = "idCufflinks",     name = "Cufflinks"      } },
                    {11,    new HabaneroLogSymbolIDName{id = "idMartini",       name = "Martini"        } },
                    {12,    new HabaneroLogSymbolIDName{id = "idRing",          name = "Ring"           } },

                    {13,    new HabaneroLogSymbolIDName{id = "idWild1",         name = "Wild1"          } },
                    {14,    new HabaneroLogSymbolIDName{id = "idWild12",        name = "Wild12"         } },
                    {15,    new HabaneroLogSymbolIDName{id = "idWild23",        name = "Wild23"         } },
                    {16,    new HabaneroLogSymbolIDName{id = "idWild3",         name = "Wild3"          } },
                    {17,    new HabaneroLogSymbolIDName{id = "idScatter5",      name = "Scatter5"       } },
                    {18,    new HabaneroLogSymbolIDName{id = "idScatter10",     name = "Scatter10"      } },
                    {19,    new HabaneroLogSymbolIDName{id = "idScatter20",     name = "Scatter20"      } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 793;
            }
        }
        protected override bool SupportMoreBet
        {
            get
            {
                return true;
            }
        }
        protected override double MoreBetMultiple
        {
            get
            {
                return 2.0;
            }
        }
        #endregion

        public TheBigDealDeluxeGameLogic()
        {
            _gameID     = GAMEID.TheBigDealDeluxe;
            GameName    = "TheBigDealDeluxe";
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                BaseHabaneroSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strGlobalUserID, out oldBetInfo))
                {
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    float coinValue = (float)message.Pop();
                    int lineCount   = (int)message.Pop();
                    int betLevel    = (int)message.Pop();
                    oldBetInfo.CoinValue    = coinValue;
                    oldBetInfo.LineCount    = lineCount;
                    oldBetInfo.BetLevel     = betLevel;
                    oldBetInfo.PurchaseFree = (int)message.Pop();
                    oldBetInfo.MoreBet      = (int)message.Pop();

                    if (oldBetInfo.MoreBet != 0 && oldBetInfo.PurchaseFree != 0)
                    {
                        _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in TheBigDealDeluxeGameLogic::readBetInfoFromMessage", strGlobalUserID);
                        return;
                    }
                }
                else
                {
                    float coinValue = (float)message.Pop();
                    int lineCount   = (int)message.Pop();
                    int betLevel    = (int)message.Pop();

                    BaseHabaneroSlotBetInfo betInfo  = new BaseHabaneroSlotBetInfo(MiniCoin);
                    betInfo.CoinValue       = coinValue;
                    betInfo.LineCount       = lineCount;
                    betInfo.BetLevel        = betLevel;
                    betInfo.PurchaseFree    = (int)message.Pop();
                    betInfo.MoreBet         = (int)message.Pop();

                    if (betInfo.MoreBet != 0 && betInfo.PurchaseFree != 0)
                    {
                        _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in TheBigDealDeluxeGameLogic::readBetInfoFromMessage", strGlobalUserID);
                        return;
                    }

                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in TheBigDealDeluxeGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override JObject buildEventItem(string strGlobalUserID, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strGlobalUserID, currentIndex);
            
            HabaneroHistoryResponses responses = _dicUserHistory[strGlobalUserID].Responses[currentIndex];
            dynamic response = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            int multiplier = 1;
            if (!object.ReferenceEquals(response["currgamemultiplier"], null))
                multiplier = response["currgamemultiplier"];

            if (responses.Action == HabaneroActionType.FREEGAME)
                multiplier = 2;

            eventItem["multiplier"] = multiplier;

            bool containWild = false;
            for(int i = 0; i < response["virtualreels"].Count; i++)
            {
                for(int j = 2; j < response["virtualreels"][i].Count - 2; j++)
                {
                    if(response["virtualreels"][i][j] == 1 || response["virtualreels"][i][j] == 13)
                    {
                        containWild = true;
                        break;
                    }
                }

                if (containWild)
                    break;
            }

            JArray SymbolMetaList = new JArray();

            if (containWild)
            {
                JArray reels            = buildHabaneroLogReels(strGlobalUserID, currentIndex, response);
                JArray reels1           = buildHabaneroLogReels(strGlobalUserID, currentIndex, response);
                
                JArray reelslist        = new JArray();
                JObject reelslistitem   = new JObject();
                for (int i = 0; i < response["reels"].Count; i++)
                {
                    for (int j = 0; j < response["reels"][i].Count; j++)
                    {
                        int symbolid = response["reels"][i][j]["symbolid"];
                        if (symbolid == 1 || symbolid == 13)
                        {
                            reels[i][0] = SymbolIdStringForLog[1].id;
                            reels[i][1] = SymbolIdStringForLog[1].id;
                            reels[i][2] = SymbolIdStringForLog[1].id;

                            int reelIndex   = i;
                            int symbolIndex = j;
                            int wildOffset  = symbolIndex;

                            if (!object.ReferenceEquals(response["reels"][i][j]["meta"], null))
                            {
                                reelIndex   = response["reels"][i][j]["meta"]["reelIndex"];
                                symbolIndex = response["reels"][i][j]["meta"]["symbolIndex"];
                                wildOffset  = response["reels"][i][j]["meta"]["wildOffset"];
                            }

                            if (wildOffset == -2)
                            {
                                reels1[reelIndex][symbolIndex] = SymbolIdStringForLog[16].id;
                            }
                            else if (wildOffset == -1)
                            {
                                reels1[reelIndex][symbolIndex] = SymbolIdStringForLog[15].id;
                                if (symbolIndex < 2)
                                    reels1[reelIndex][symbolIndex + 1] = null;
                            }
                            else if (wildOffset == 0)
                            {
                                reels1[reelIndex][symbolIndex] = SymbolIdStringForLog[1].id;
                                if (symbolIndex < 2)
                                    reels1[reelIndex][symbolIndex + 1] = null;
                                if (symbolIndex < 1)
                                    reels1[reelIndex][symbolIndex + 2] = null;
                            }
                            else if (wildOffset == 1)
                            {
                                reels1[reelIndex][symbolIndex] = SymbolIdStringForLog[14].id;
                                if (symbolIndex < 2)
                                    reels1[reelIndex][symbolIndex + 1] = null;
                            }
                            else if (wildOffset == 2)
                            {
                                reels1[reelIndex][symbolIndex] = SymbolIdStringForLog[13].id;
                            }

                            break;
                        }
                    }
                }
                reelslistitem["reels"] = reels1;
                reelslist.Add(reelslistitem);

                eventItem["reels"]      = reels;
                eventItem["reelslist"]  = reelslist;
            }

            for (int i = 0; i < response["reels"].Count; i++)
            {
                for (int j = 0; j < response["reels"][i].Count; j++)
                {
                    if (!object.ReferenceEquals(response["reels"][i][j]["meta"], null))
                    {
                        SymbolMetaList.Add(response["reels"][i][j]["meta"]);

                        int symbolid = response["reels"][i][j]["symbolid"];
                        if(symbolid == 2)
                        {
                            int reelIndex       = response["reels"][i][j]["meta"]["reelIndex"];
                            int symbolIndex     = response["reels"][i][j]["meta"]["symbolIndex"];
                            int numFreeGames    = response["reels"][i][j]["meta"]["numFreeGames"];

                            if(numFreeGames == 5)
                                eventItem["reels"][reelIndex][symbolIndex] = SymbolIdStringForLog[17].id;
                            else if(numFreeGames == 10)
                                eventItem["reels"][reelIndex][symbolIndex] = SymbolIdStringForLog[18].id;
                            else if(numFreeGames == 20)
                                eventItem["reels"][reelIndex][symbolIndex] = SymbolIdStringForLog[19].id;
                        }
                    }
                }
            }

            if (SymbolMetaList.Count > 0)
            {
                JObject replay_meta = new JObject();
                replay_meta["SymbolMetaList"] = SymbolMetaList;

                eventItem["replay_meta"] = replay_meta;
            }

            return eventItem;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            JArray resumeGames = base.buildInitResumeGame(strGlobalUserID, betInfo, lastResult, gameinstanceid, roundid, currentAction);
            
            resumeGames[0]["TheBigDealDeluxe_superBet"] = betInfo.MoreBet;
            resumeGames[0]["gamemode"]                  = lastResult["nextgamestate"];
            return resumeGames;
        }
    }
}
