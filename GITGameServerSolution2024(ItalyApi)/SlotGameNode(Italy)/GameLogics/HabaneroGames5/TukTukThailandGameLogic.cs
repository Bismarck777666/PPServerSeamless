using Akka.Actor;
using GITProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCGSharp;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    public class TukTukThailandGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGTukTukThailand";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "8d2564a2-c614-4528-b563-3227709c4421";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "3e4ffcc60fce72d0d80d7b35ce084bb558d6214e";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.10500.419";
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
                return 243;
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
                    {2,     new HabaneroLogSymbolIDName{id = "idScatter",       name = "Scatter"        } },
                    {3,     new HabaneroLogSymbolIDName{id = "idLotus",         name = "Lotus"          } },
                    {4,     new HabaneroLogSymbolIDName{id = "idDragon",        name = "Dragon"         } },
                    {5,     new HabaneroLogSymbolIDName{id = "idRoyalBarge",    name = "RoyalBarge"     } },
                    {6,     new HabaneroLogSymbolIDName{id = "idTiger",         name = "Tiger"          } },
                    {7,     new HabaneroLogSymbolIDName{id = "idThaiPrawnLaksa",name = "ThaiPrawnLaksa" } },
                    {8,     new HabaneroLogSymbolIDName{id = "idA",             name = "A"              } },
                    {9,     new HabaneroLogSymbolIDName{id = "idK",             name = "K"              } },
                    {10,    new HabaneroLogSymbolIDName{id = "idQ",             name = "Q"              } },
                    {11,    new HabaneroLogSymbolIDName{id = "idJ",             name = "J"              } },
                    {12,    new HabaneroLogSymbolIDName{id = "idBlank",         name = "Blank"          } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 683;
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double[] PurchaseFreeMultiple
        {
            get { return new double[] { 2966 / 50.0, 4380 / 50.0, 11760 / 50.0 }; }
        }
        #endregion

        //프리스핀구매기능이 있을떄만 필요하다. 디비안의 모든 프리스핀들의 오드별 아이디어레이
        protected SortedDictionary<double, int[]>[] _totalFreeSpinOddIdses    = new SortedDictionary<double, int[]>[]
        {
            new SortedDictionary<double, int[]>(),
            new SortedDictionary<double, int[]>(),
            new SortedDictionary<double, int[]>()
        };
        protected int[]     _freeSpinTotalCounts      = new int[] { 0, 0, 0 };
        protected int[]     _minFreeSpinTotalCounts   = new int[] { 0, 0, 0 };
        protected double[]  _totalFreeSpinWinRates    = new double[] { 0.0, 0.0, 0.0 }; //스핀디비안의 모든 프리스핀들의 배당평균값
        protected double[]  _minFreeSpinWinRates      = new double[] { 0.0, 0.0, 0.0 }; //구매금액의 20% - 50%사이에 들어가는 모든 프리스핀들의 평균배당값

        public TukTukThailandGameLogic()
        {
            _gameID     = GAMEID.TukTukThailand;
            GameName    = "TukTukThailand";
        }

        protected override async Task<bool> loadSpinData()
        {
            try
            {
                _spinDatabase = Context.ActorOf(Akka.Actor.Props.Create(() => new SpinDatabase(this._providerName, this.GameName)), "spinDatabase");
                bool isSuccess = await _spinDatabase.Ask<bool>("initialize", TimeSpan.FromSeconds(5.0));
                if (!isSuccess)
                {
                    _logger.Error("couldn't load spin data of game {0}", this.GameName);
                    return false;
                }

                ReadSpinInfoResponse response = await _spinDatabase.Ask<ReadSpinInfoResponse>(new ReadSpinInfoRequest(SpinDataTypes.Normal), TimeSpan.FromSeconds(30.0));
                if (response == null)
                {
                    _logger.Error("couldn't load spin odds information of game {0}", this.GameName);
                    return false;
                }

                _normalMaxID            = response.NormalMaxID;
                _naturalSpinOddProbs    = new SortedDictionary<double, int>();
                _spinDataDefaultBet     = response.DefaultBet;
                _naturalSpinCount       = 0;
                _emptySpinIDs           = new List<int>();

                Dictionary<double, List<int>> totalSpinOddIds   = new Dictionary<double, List<int>>();
                Dictionary<double, List<int>>[] freeSpinOddIdses= new Dictionary<double, List<int>>[] { 
                    new Dictionary<double, List<int>>(),
                    new Dictionary<double, List<int>>(),
                    new Dictionary<double, List<int>>()
                };

                double[]    freeSpinTotalOdds        = new double[] {0.0, 0.0, 0.0};
                double[]    minFreeSpinTotalOdds     = new double[] {0.0, 0.0, 0.0};
                int[]       freeSpinTotalCounts      = new int[] { 0, 0, 0 };
                int[]       minFreeSpinTotalCounts   = new int[] { 0, 0, 0 };

                for (int i = 0; i < response.SpinBaseDatas.Count; i++)
                {
                    SpinBaseData spinBaseData = response.SpinBaseDatas[i];
                    if (spinBaseData.ID <= response.NormalMaxID)
                    {
                        _naturalSpinCount++;
                        if (_naturalSpinOddProbs.ContainsKey(spinBaseData.Odd))
                            _naturalSpinOddProbs[spinBaseData.Odd]++;
                        else
                            _naturalSpinOddProbs[spinBaseData.Odd] = 1;
                    }
                    
                    if (!totalSpinOddIds.ContainsKey(spinBaseData.Odd))
                        totalSpinOddIds.Add(spinBaseData.Odd, new List<int>());

                    if (spinBaseData.SpinType == 0 && spinBaseData.Odd == 0.0)
                        _emptySpinIDs.Add(spinBaseData.ID);

                    totalSpinOddIds[spinBaseData.Odd].Add(spinBaseData.ID);
                    if (SupportPurchaseFree)
                    {
                        if(spinBaseData.SpinType != 0)
                        {
                            freeSpinTotalCounts[spinBaseData.SpinType - 1]++;
                            freeSpinTotalOdds[spinBaseData.SpinType - 1] += spinBaseData.Odd;
                            if (!freeSpinOddIdses[spinBaseData.SpinType - 1].ContainsKey(spinBaseData.Odd))
                                freeSpinOddIdses[spinBaseData.SpinType - 1].Add(spinBaseData.Odd, new List<int>());
                            freeSpinOddIdses[spinBaseData.SpinType - 1][spinBaseData.Odd].Add(spinBaseData.ID);

                            if (spinBaseData.Odd >= PurchaseFreeMultiple[spinBaseData.SpinType - 1] * 0.2 && spinBaseData.Odd <= PurchaseFreeMultiple[spinBaseData.SpinType - 1] * 0.5)
                            {
                                minFreeSpinTotalCounts[spinBaseData.SpinType - 1]++;
                                minFreeSpinTotalOdds[spinBaseData.SpinType - 1] += spinBaseData.Odd;
                            }
                        }
                    }
                }
                
                _totalSpinOddIds = new SortedDictionary<double, int[]>();
                foreach (KeyValuePair<double, List<int>> pair in totalSpinOddIds)
                    _totalSpinOddIds.Add(pair.Key, pair.Value.ToArray());

                if (SupportPurchaseFree)
                {
                    _totalFreeSpinOddIdses = new SortedDictionary<double, int[]>[]
                    {
                        new SortedDictionary<double, int[]>(),
                        new SortedDictionary<double, int[]>(),
                        new SortedDictionary<double, int[]>()
                    };
                    
                    for(int i = 0; i < 3; i++)
                    {
                        foreach (KeyValuePair<double, List<int>> pair in freeSpinOddIdses[i])
                            _totalFreeSpinOddIdses[i].Add(pair.Key, pair.Value.ToArray());

                        _freeSpinTotalCounts[i]     = freeSpinTotalCounts[i];
                        _minFreeSpinTotalCounts[i]  = minFreeSpinTotalCounts[i];
                        _totalFreeSpinWinRates[i]   = freeSpinTotalOdds[i] / freeSpinTotalCounts[i];
                        _minFreeSpinWinRates[i]     = minFreeSpinTotalOdds[i] / minFreeSpinTotalCounts[i];

                        if (_totalFreeSpinWinRates[i] <= _minFreeSpinWinRates[i] || _minFreeSpinTotalCounts[i] == 0)
                        {
                            _logger.Error("min freespin rate doesn't satisfy condition {0}", this.GameName);
                            break;
                        }
                    }
                }

                for (int i = 0; i < 3; i++)
                {
                    if (this.PurchaseFreeMultiple[i] > _totalFreeSpinWinRates[i])
                        _logger.Error("freespin win rate doesn't satisfy condition {0}", this.GameName);
                }

                double winRate = 0.0;
                foreach (KeyValuePair<double, int> pair in _naturalSpinOddProbs)
                    winRate += (pair.Key * pair.Value / _naturalSpinCount);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
                return false;
            }
        }

        protected override double getPurchaseMultiple(BaseHabaneroSlotBetInfo betInfo)
        {
            return this.PurchaseFreeMultiple[betInfo.PurchaseFree - 3];
        }

        protected override async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int agentID, BaseHabaneroSlotBetInfo betInfo, double baseBet, UserBonus userBonus)
        {
            if (userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus rangeOddBonus = userBonus as UserRangeOddEventBonus;
                OddAndIDData oddAndID = selectOddAndIDFromProbsWithRange(_totalFreeSpinOddIdses[betInfo.PurchaseFree - 3], rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd);
                if (oddAndID != null)
                {
                    BasePPSlotSpinData spinDataEvent = await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
                    spinDataEvent.IsEvent = true;
                    return spinDataEvent;
                }
            }

            double payoutRate = _config.PayoutRate;
            if (_agentPayoutRates.ContainsKey(agentID))
                payoutRate = _agentPayoutRates[agentID];

            double targetC = PurchaseFreeMultiple[betInfo.PurchaseFree - 3] * payoutRate / 100.0;
            if (targetC >= _totalFreeSpinWinRates[betInfo.PurchaseFree - 3])
                targetC = _totalFreeSpinWinRates[betInfo.PurchaseFree - 3];

            if (targetC < _minFreeSpinWinRates[betInfo.PurchaseFree - 3])
                targetC = _minFreeSpinWinRates[betInfo.PurchaseFree - 3];

            double x = (_totalFreeSpinWinRates[betInfo.PurchaseFree - 3] - targetC) / (_totalFreeSpinWinRates[betInfo.PurchaseFree - 3] - _minFreeSpinWinRates[betInfo.PurchaseFree - 3]);

            BasePPSlotSpinData spinData = null;
            if (Pcg.Default.NextDouble(0.0, 1.0) <= x)
                spinData = await selectMinStartFreeSpinData(betInfo);
            else
                spinData = await selectRandomStartFreeSpinData(betInfo);
            return spinData;
        }

        protected override async Task<BasePPSlotSpinData> selectMinStartFreeSpinData(BaseHabaneroSlotBetInfo betInfo)
        {
            try
            {
                OddAndIDData oddAndID = selectOddAndIDFromProbsWithRange(_totalFreeSpinOddIdses[betInfo.PurchaseFree - 3], _minFreeSpinTotalCounts[betInfo.PurchaseFree - 3], PurchaseFreeMultiple[betInfo.PurchaseFree - 3] * 0.2, PurchaseFreeMultiple[betInfo.PurchaseFree - 3] * 0.5);
                return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in TukTukThailandGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }

        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BaseHabaneroSlotBetInfo betInfo)
        {
            try
            {
                OddAndIDData oddAndID = selectOddAndIDFromProbs(_totalFreeSpinOddIdses[betInfo.PurchaseFree - 3], _freeSpinTotalCounts[betInfo.PurchaseFree - 3]);
                return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in TukTukThailandGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
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
                        _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in TukTukThailandGameLogic::readBetInfoFromMessage", strGlobalUserID);
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
                        _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in TukTukThailandGameLogic::readBetInfoFromMessage", strGlobalUserID);
                        return;
                    }

                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in TukTukThailandGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override JObject buildEventItem(string strGlobalUserID, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strGlobalUserID, currentIndex);
            
            HabaneroHistoryResponses responses = _dicUserHistory[strGlobalUserID].Responses[currentIndex];
            dynamic response = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            int multiplier  = 1;
            if (!object.ReferenceEquals(response["currgamemultiplier"], null))
                multiplier = response["currgamemultiplier"];

            eventItem["multiplier"] = multiplier;

            bool containMoney = false;
            for (int j = 0; j < response["reels"].Count; j++)
            {
                for (int k = 0; k < response["reels"][j].Count; k++)
                {
                    string symbol = Convert.ToString(response["reels"][j][k]["symbolid"]);
                    if (symbol == "3" && !object.ReferenceEquals(response["reels"][j][k]["wincashmultiplier"], null))
                    {
                        containMoney = true;
                        break;
                    }
                }

                if (containMoney)
                    break;
            }

            if (containMoney)
            {
                JArray reelsmeta = buildHabaneroLogReelsMeta(response as JObject);
                eventItem["reels_meta"] = reelsmeta;
            }

            if (!object.ReferenceEquals(response["wildPosList"], null))
            {
                eventItem["reelslist"] = buildHabaneroLogReelslist(response);
            }
            
            return eventItem;
        }

        protected override JArray buildHabaneroLogReelsMeta(dynamic response)
        {
            JArray reelsListMeta = new JArray();

            for (int j = 0; j < response["reels"].Count; j++)
            {
                bool hasMoneyInCol = false;
                for (int k = 0; k < response["reels"][j].Count; k++)
                {
                    string symbol = Convert.ToString(response["reels"][j][k]["symbolid"]);
                    if(symbol == "3")
                    {
                        hasMoneyInCol = true;
                        break;
                    }
                }
                
                if (!hasMoneyInCol)
                {
                    reelsListMeta.Add(null);
                    continue;
                }
                
                JArray reelsMetaCol = new JArray();
                for (int k = 0; k < response["reels"][j].Count; k++)
                {
                    string symbol = Convert.ToString(response["reels"][j][k]["symbolid"]);
                    if(symbol == "3")
                    {
                        JObject moneyMul = new JObject();
                        moneyMul["cash_linebet"] = response["reels"][j][k]["wincashmultiplier"];
                        reelsMetaCol.Add(moneyMul);
                    }
                    else
                        reelsMetaCol.Add(null);
                }
                reelsListMeta.Add(reelsMetaCol);
            }
            return reelsListMeta;
        }

        protected override JArray buildHabaneroLogReelslist(dynamic response)
        {
            JArray reelslist        = new JArray();
            JObject reelslistitem   = new JObject();
            JArray reels            = new JArray();

            for (int i = 0; i < response["wildPosList"].Count; i++)
            {
                JArray reelscol = new JArray();
                for(int j = 0; j < 3; j++)
                {
                    reelscol.Add(SymbolIdStringForLog[12].id);
                }

                for(int j = 0; j < response["wildPosList"][i].Count; j++)
                {
                    if(response["wildPosList"][i][j] != null)
                    {
                        int symbolindex         = response["wildPosList"][i][j];
                        reelscol[symbolindex]   = SymbolIdStringForLog[1].id;
                    }
                }

                reels.Add(JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(reelscol)));
            }

            reelslistitem["reels"] = reels;
            reelslist.Add(reelslistitem);

            return reelslist;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            JArray resumeGames = base.buildInitResumeGame(strGlobalUserID, betInfo, lastResult, gameinstanceid, roundid, currentAction);

            if(!object.ReferenceEquals(lastResult["newWildPosList"], null))
            {
                resumeGames[0]["resumeWildPosList"] = lastResult["newWildPosList"];
            }
            
            if (betInfo.PurchaseFree > 0)
                resumeGames[0]["TukTukThailand_featureBuy"] = betInfo.PurchaseFree;

            return resumeGames;
        }
    }
}
