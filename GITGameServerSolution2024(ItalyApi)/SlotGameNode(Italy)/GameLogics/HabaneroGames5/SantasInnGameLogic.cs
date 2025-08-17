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
    public class SantasInnGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGSantasInn";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "f1384317-cbde-431d-8988-b3db9766351d";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "5e04a89e245f6e180119849d1f2f2f2cad629e98";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.12495.0";
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
                    {1,     new HabaneroLogSymbolIDName{id = "idWild",          name = "Wild"           } },
                    {2,     new HabaneroLogSymbolIDName{id = "idWildTrain",     name = "WildTrain"      } },
                    {3,     new HabaneroLogSymbolIDName{id = "idGoldenTeddy",   name = "GoldenTeddy"    } },
                    {4,     new HabaneroLogSymbolIDName{id = "idGoldStar",      name = "GoldStar"       } },
                    {5,     new HabaneroLogSymbolIDName{id = "idGreen7",        name = "Green7"         } },
                    {6,     new HabaneroLogSymbolIDName{id = "idBlue7",         name = "Blue7"          } },
                    {7,     new HabaneroLogSymbolIDName{id = "idRed7",          name = "Red7"           } },
                    {8,     new HabaneroLogSymbolIDName{id = "idWildX2",        name = "WildX2"         } },
                    {9,     new HabaneroLogSymbolIDName{id = "idWildTrainX2",   name = "WildTrainX2"    } },
                    {10,    new HabaneroLogSymbolIDName{id = "idGoldenTeddyX2", name = "GoldenTeddyX2"  } },
                    {11,    new HabaneroLogSymbolIDName{id = "idGoldStarX2",    name = "GoldStarX2"     } },
                    {12,    new HabaneroLogSymbolIDName{id = "idGreen7X2",      name = "Green7X2"       } },
                    {13,    new HabaneroLogSymbolIDName{id = "idBlue7X2",       name = "Blue7X2"        } },
                    {14,    new HabaneroLogSymbolIDName{id = "idRed7X2",        name = "Red7X2"         } },
                    {15,    new HabaneroLogSymbolIDName{id = "idWildX3",        name = "WildX3"         } },
                    {16,    new HabaneroLogSymbolIDName{id = "idWildTrainX3",   name = "WildTrainX3"    } },
                    {17,    new HabaneroLogSymbolIDName{id = "idGoldenTeddyX3", name = "GoldenTeddyX3"  } },
                    {18,    new HabaneroLogSymbolIDName{id = "idGoldStarX3",    name = "GoldStarX3"     } },
                    {19,    new HabaneroLogSymbolIDName{id = "idGreen7X3",      name = "Green7X3"       } },
                    {20,    new HabaneroLogSymbolIDName{id = "idBlue7X3",       name = "Blue7X3"        } },
                    {21,    new HabaneroLogSymbolIDName{id = "idRed7X3",        name = "Red7X3"         } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 943;
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double[] PurchaseFreeMultiple
        {
            get { return new double[] { 55.0 }; }
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
                return 20 / 9.0;
            }
        }
        #endregion

        public SantasInnGameLogic()
        {
            _gameID     = GAMEID.SantasInn;
            GameName    = "SantasInn";
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
                        _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in SantasInnGameLogic::readBetInfoFromMessage", strGlobalUserID);
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
                        _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in SantasInnGameLogic::readBetInfoFromMessage", strGlobalUserID);
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
            if (!object.ReferenceEquals(response["trainList"], null))
            {
                for(int i = 0; i < response["trainList"].Count; i++)
                {
                    int mul = Convert.ToInt32(response["trainList"][i]["multiplier"]);
                    if (mul > multiplier)
                        multiplier = mul;
                }
            }
            eventItem["multiplier"] = multiplier;

            bool hasReelList = false;

            if (!object.ReferenceEquals(response["trainList"], null) || !object.ReferenceEquals(response["upgradeList"], null))
            {
                hasReelList = true;
                JObject replay_meta = new JObject();
                if(!object.ReferenceEquals(response["trainList"], null))
                    replay_meta["trainList"] = response["trainList"];

                if (!object.ReferenceEquals(response["upgradeList"], null))
                    replay_meta["upgradeList"] = response["upgradeList"];

                eventItem["replay_meta"] = replay_meta;
            }

            if (!hasReelList)
            {
                for (int i = 0; i < response["reels"].Count; i++)
                {
                    for (int j = 0; j < response["reels"][i].Count; j++)
                    {
                        if(!object.ReferenceEquals(response["reels"][i][j]["splitcount"], null))
                        {
                            hasReelList = true;
                            break;
                        }
                    }
                    if (hasReelList)
                        break;
                }
            }

            if (hasReelList)
            {
                JArray reelslist = buildHabaneroLogReelslist(response as JObject);

                JObject lastItem = (dynamic)reelslist[reelslist.Count - 1];

                eventItem["reels"]      = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(lastItem["reels"]));
                reelslist.RemoveAt(reelslist.Count - 1);
                eventItem["reelslist"]  = reelslist;
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

            if (!object.ReferenceEquals(currentResult["expandingwilds"], null))
            {
                for(int i = 0; i < currentResult["expandingwilds"].Count; i++)
                {
                    int reelindex   = currentResult["expandingwilds"][i]["reelindex"];
                    int symbolid    = currentResult["expandingwilds"][i]["symbolid"];
                    
                    for(int j = 0; j < 3; j++)
                        reelArray[reelindex][j] = symbolid;
                }
            }

            addReelListItem(reelArray, reelslist);

            if (!object.ReferenceEquals(currentResult["trainList"], null))
            {
                for (int i = 0; i < currentResult["trainList"].Count; i++)
                {
                    int reelindex   = Convert.ToInt32(currentResult["trainList"][i]["pos"]["reelindex"]);
                    int symbolindex = Convert.ToInt32(currentResult["trainList"][i]["pos"]["symbolindex"]);
                    int symbol      = Convert.ToInt32(currentResult["trainList"][i]["replaceId"]);

                    reelArray[reelindex][symbolindex] = symbol;
                    addReelListItem(reelArray, reelslist);
                }
            }

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
                    if(!object.ReferenceEquals(currentResult["reels"][i][j]["splitcount"], null))
                    {
                        hasSplit = true;

                        int split       = currentResult["reels"][i][j]["splitcount"];
                        reelArray[i][j] = (int)reelArray[i][j] + (split - 1) * 7;
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
    }
}
