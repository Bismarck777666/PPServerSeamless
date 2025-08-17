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
    public class SirensSpellGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGSirensSpell";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "d2d2301a-3119-465f-a3e2-58009c0b15f5";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "6ffbfaf6049aabf31283bcae0dbbe961d473ab27";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.11369.0";
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
                    {1,     new HabaneroLogSymbolIDName{id = "idWild",      name = "Wild"       } },
                    {2,     new HabaneroLogSymbolIDName{id = "idScatter",   name = "Scatter"    } },
                    {3,     new HabaneroLogSymbolIDName{id = "idJellyfish", name = "Jellyfish"  } },
                    {4,     new HabaneroLogSymbolIDName{id = "idSeahorse",  name = "Seahorse"   } },
                    {5,     new HabaneroLogSymbolIDName{id = "idShark",     name = "Shark"      } },
                    {6,     new HabaneroLogSymbolIDName{id = "idTurtle",    name = "Turtle"     } },
                    {7,     new HabaneroLogSymbolIDName{id = "idShell1",    name = "Shell1"     } },
                    {8,     new HabaneroLogSymbolIDName{id = "idShell2",    name = "Shell2"     } },
                    {9,     new HabaneroLogSymbolIDName{id = "idShell3",    name = "Shell3"     } },
                    {10,    new HabaneroLogSymbolIDName{id = "idShell4",    name = "Shell4"     } },
                    {11,    new HabaneroLogSymbolIDName{id = "idWild1",     name = "Wild1"      } },
                    {12,    new HabaneroLogSymbolIDName{id = "idWild12",    name = "Wild12"     } },
                    {13,    new HabaneroLogSymbolIDName{id = "idWild23",    name = "Wild23"     } },
                    {14,    new HabaneroLogSymbolIDName{id = "idWild3",     name = "Wild3"      } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 813;
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double[] PurchaseFreeMultiple
        {
            get { return new double[] { 1252 / 25.0 }; }
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

        public SirensSpellGameLogic()
        {
            _gameID     = GAMEID.SirensSpell;
            GameName    = "SirensSpell";
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
                        _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in SirensSpellGameLogic::readBetInfoFromMessage", strGlobalUserID);
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
                        _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in SirensSpellGameLogic::readBetInfoFromMessage", strGlobalUserID);
                        return;
                    }

                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SirensSpellGameLogic::readBetInfoFromMessage {0}", ex);
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
            eventItem["multiplier"] = multiplier;

            bool containWild = false;
            for(int i = 0; i < response["virtualreels"].Count; i++)
            {
                for(int j = 2; j < response["virtualreels"][i].Count - 2; j++)
                {
                    if(response["virtualreels"][i][j] == 1)
                    {
                        containWild = true;
                        break;
                    }
                }

                if (containWild)
                    break;
            }

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
                        if (symbolid == 1)
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
                                reels1[reelIndex][symbolIndex] = SymbolIdStringForLog[14].id;
                            }
                            else if (wildOffset == -1)
                            {
                                reels1[reelIndex][symbolIndex] = SymbolIdStringForLog[13].id;
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
                                reels1[reelIndex][symbolIndex] = SymbolIdStringForLog[12].id;
                                if (symbolIndex < 2)
                                    reels1[reelIndex][symbolIndex + 1] = null;
                            }
                            else if (wildOffset == 2)
                            {
                                reels1[reelIndex][symbolIndex] = SymbolIdStringForLog[11].id;
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

            JObject replay_meta = new JObject();
            replay_meta["collectionindex"]      = response["collectionindex"];
            replay_meta["collectionindexstart"] = response["collectionindexstart"];
            replay_meta["collectionround"]      = response["collectionround"];
            eventItem["replay_meta"] = replay_meta;

            return eventItem;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            JArray resumeGames = base.buildInitResumeGame(strGlobalUserID, betInfo, lastResult, gameinstanceid, roundid, currentAction);
            dynamic response = lastResult as dynamic;
            
            if(!object.ReferenceEquals(response["collectionindex"], null))
            {
                int collectionindex         = response["collectionindex"];

                resumeGames[0]["SirensSpell_collectionIndex"]       = collectionindex;
            }

            int cnt = 0;
            for(int i = 0; i < _dicUserHistory[strGlobalUserID].Responses.Count; i++)
            {
                HabaneroHistoryResponses strResponse = _dicUserHistory[strGlobalUserID].Responses[i];
                dynamic resObj = JsonConvert.DeserializeObject<dynamic>(strResponse.Response);

                if (!object.ReferenceEquals(resObj["collectionround"], null))
                {
                    int newcnt = resObj["collectionround"];
                    cnt += newcnt;

                }
            }
            resumeGames[0]["SirensSpell_activeSymbolWinCount"]  = cnt % 40;
            resumeGames[0]["SirensSpell_featureBuy"]            = betInfo.PurchaseFree;
            resumeGames[0]["SirensSpell_superBet"]              = betInfo.MoreBet;
            resumeGames[0]["gamemode"]                          = lastResult["nextgamestate"];
            return resumeGames;
        }
    }
}
