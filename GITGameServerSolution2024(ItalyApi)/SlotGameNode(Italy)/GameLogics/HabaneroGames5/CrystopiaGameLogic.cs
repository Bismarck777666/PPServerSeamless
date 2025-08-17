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
    public class CrystopiaGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGCrystopia";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "5c98d945-b53d-432a-89a4-5af557e0142f";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "afbbc453748bb0f123dda5cbe33b4cb8985ea322";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.11825.0";
            }
        }
        protected override float MiniCoin
        {
            get
            {
                return 9.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 27;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,     new HabaneroLogSymbolIDName{id = "idWild",          name = "Wild"           } },
                    {2,     new HabaneroLogSymbolIDName{id = "idSquareGold",    name = "SquareGold"     } },
                    {3,     new HabaneroLogSymbolIDName{id = "idSquareGreen",   name = "SquareGreen"    } },
                    {4,     new HabaneroLogSymbolIDName{id = "idSquareBlue",    name = "SquareBlue"     } },
                    {5,     new HabaneroLogSymbolIDName{id = "idRoundGold",     name = "RoundGold"      } },
                    {6,     new HabaneroLogSymbolIDName{id = "idRoundGreen",    name = "RoundGreen"     } },
                    {7,     new HabaneroLogSymbolIDName{id = "idRoundBlue",     name = "RoundBlue"      } },
                    {8,     new HabaneroLogSymbolIDName{id = "idWildX2",        name = "WildX2"         } },
                    {9,     new HabaneroLogSymbolIDName{id = "idSquareGoldX2",  name = "SquareGoldX2"   } },
                    {10,    new HabaneroLogSymbolIDName{id = "idSquareGreenX2", name = "SquareGreenX2"  } },
                    {11,    new HabaneroLogSymbolIDName{id = "idSquareBlueX2",  name = "SquareBlueX2"   } },
                    {12,    new HabaneroLogSymbolIDName{id = "idRoundGoldX2",   name = "RoundGoldX2"    } },
                    {13,    new HabaneroLogSymbolIDName{id = "idRoundGreenX2",  name = "RoundGreenX2"   } },
                    {14,    new HabaneroLogSymbolIDName{id = "idRoundBlueX2",   name = "RoundBlueX2"    } },
                    {15,    new HabaneroLogSymbolIDName{id = "idWildX4",        name = "WildX4"         } },
                    {16,    new HabaneroLogSymbolIDName{id = "idSquareGoldX4",  name = "SquareGoldX4"   } },
                    {17,    new HabaneroLogSymbolIDName{id = "idSquareGreenX4", name = "SquareGreenX4"  } },
                    {18,    new HabaneroLogSymbolIDName{id = "idSquareBlueX4",  name = "SquareBlueX4"   } },
                    {19,    new HabaneroLogSymbolIDName{id = "idRoundGoldX4",   name = "RoundGoldX4"    } },
                    {20,    new HabaneroLogSymbolIDName{id = "idRoundGreenX4",  name = "RoundGreenX4"   } },
                    {21,    new HabaneroLogSymbolIDName{id = "idRoundBlueX4",   name = "RoundBlueX4"    } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 808;
            }
        }
        protected override string BetType
        {
            get
            {
                return "Ways";
            }
        }

        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double[] PurchaseFreeMultiple
        {
            get { return new double[] { 420 / 9.0 }; }
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

        public CrystopiaGameLogic()
        {
            _gameID     = GAMEID.Crystopia;
            GameName    = "Crystopia";
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
                        _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in FruityHalloweenGameLogic::readBetInfoFromMessage", strGlobalUserID);
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
                        _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in FruityHalloweenGameLogic::readBetInfoFromMessage", strGlobalUserID);
                        return;
                    }

                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SantasInnGameLogic::readBetInfoFromMessage {0}", ex);
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

            bool containUpgrade = false;
            if (!object.ReferenceEquals(response["upgradeList"], null))
                containUpgrade = true;

            bool containSplit = false;
            for(int i = 0; i < response["reels"].Count; i++)
            {
                for(int j = 0; j < response["reels"][i].Count; j++)
                {
                    if(!object.ReferenceEquals(response["reels"][i][j]["wincashmultiplier"],null))
                    {
                        containSplit = true;
                        break;
                    }
                }

                if (containSplit)
                    break;
            }

            if (containUpgrade && containSplit)
            {
                JArray reelslist = buildHabaneroLogReelslist(response as JObject);
                JObject lastItem = (dynamic)reelslist[reelslist.Count - 1];

                eventItem["reels"] = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(lastItem["reels"]));
                reelslist.RemoveAt(reelslist.Count - 1);
                eventItem["reelslist"] = reelslist;
            }

            if (containUpgrade)
            {
                JObject replay_meta = new JObject();
                replay_meta["upgradeList"] = response["upgradeList"];

                eventItem["replay_meta"] = replay_meta;
            }

            return eventItem;
        }

        protected override JArray buildHabaneroLogReelslist(dynamic currentResult)
        {
            JArray reelslist        = new JArray();
            
            JArray reelArray        = new JArray();
            for (int i = 0; i < currentResult["reels"].Count; i++)
            {
                JArray reelColArray     = new JArray();
                for (int j = 0; j < currentResult["reels"][i].Count; j++)
                {
                    reelColArray.Add((int)currentResult["reels"][i][j]["symbolid"]);
                }
                reelArray.Add(reelColArray);
            }

            addReelListItem(reelArray, reelslist);

            if (!object.ReferenceEquals(currentResult["upgradeList"], null))
            {
                for (int i = 0; i < currentResult["upgradeList"].Count; i++)
                {
                    int symbol      = Convert.ToInt32(currentResult["upgradeList"][i]["symbolId"]);
                    for(int j = 0; j < currentResult["upgradeList"][i]["posList"].Count; j++)
                    {
                        int reelindex   = Convert.ToInt32(currentResult["upgradeList"][i]["posList"][j]["reelindex"]);
                        int symbolindex = Convert.ToInt32(currentResult["upgradeList"][i]["posList"][j]["symbolindex"]);
                        reelArray[reelindex][symbolindex] = symbol;
                    }

                    addReelListItem(reelArray, reelslist);
                }
            }

            bool hasSplit = false;
            for(int i = 0; i < currentResult["reels"].Count; i++)
            {
                for(int j = 0; j < currentResult["reels"][i].Count; j++)
                {
                    if(!object.ReferenceEquals(currentResult["reels"][i][j]["wincashmultiplier"], null))
                    {
                        hasSplit = true;

                        int wincashmultiplier = currentResult["reels"][i][j]["wincashmultiplier"];
                        reelArray[i][j] = (int)reelArray[i][j] + (wincashmultiplier / 2 * 7);
                    }
                }
            }

            if(hasSplit)
                addReelListItem(reelArray, reelslist);

            return reelslist;
        }

        private void addReelListItem(JArray reelArray, JArray reelslist)
        {
            JObject reelslistItem = new JObject();
            JArray  reelsListCols = new JArray();
            
            for (int i = 0; i < reelArray.Count; i++)
            {
                JArray reelsCol = new JArray();
                for (int j = 0; j < ((dynamic)reelArray[i]).Count; j++)
                    reelsCol.Add(SymbolIdStringForLog[(int)reelArray[i][j]].id);

                reelsListCols.Add(reelsCol);
            }
            reelslistItem["reels"] = reelsListCols;
            reelslist.Add(JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(reelslistItem)));
        }

        protected override JArray buildInitResumeGame(string strGlobalUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            JArray resumeGames = base.buildInitResumeGame(strGlobalUserID, betInfo, lastResult, gameinstanceid, roundid, currentAction);
            
            resumeGames[0]["Crystopia_featureBuy"]  = betInfo.PurchaseFree;
            resumeGames[0]["Crystopia_superBet"]    = betInfo.MoreBet;
            resumeGames[0]["gamemode"]              = lastResult["nextgamestate"];
            return resumeGames;
        }
    }
}
