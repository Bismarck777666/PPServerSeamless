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
    public class GoldenUnicornDeluxeGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGGoldenUnicornDeluxe";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "d83cf378-7c4a-431d-aaa4-02e4b06339b2";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "852ef2e86f08fb986cc25d12d7e2537c377f63d9";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.9969.412";
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
                    {1, new HabaneroLogSymbolIDName{id = "idWild",              name = "idWild"             } },    //와일드
                    {2, new HabaneroLogSymbolIDName{id = "idWildGoldenUnicorn", name = "WildGoldenUnicorn"  } },    //골드유니콘
                    {3, new HabaneroLogSymbolIDName{id = "idScatter",           name = "Scatter"            } },    //스캐터
                    {4, new HabaneroLogSymbolIDName{id = "idChest",             name = "Chest"              } },    //박스
                    {5, new HabaneroLogSymbolIDName{id = "idPrincess",          name = "Princess"           } },    //공주
                    {6, new HabaneroLogSymbolIDName{id = "idBeetle",            name = "Beetle"             } },    //벌레
                    {7, new HabaneroLogSymbolIDName{id = "idA",                 name = "A"                  } },    //A
                    {8, new HabaneroLogSymbolIDName{id = "idK",                 name = "K"                  } },    //K
                    {9, new HabaneroLogSymbolIDName{id = "idQ",                 name = "Q"                  } },    //Q
                    {10,new HabaneroLogSymbolIDName{id = "idJ",                 name = "J"                  } },    //J
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 648;
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double[] PurchaseFreeMultiple
        {
            get { 
                return new double[] { 1020 / 25.0f, 376/25.0f }; 
            }
        }
        #endregion

        //프리스핀구매기능이 있을떄만 필요하다. 디비안의 모든 프리스핀들의 오드별 아이디어레이
        protected SortedDictionary<double, int[]>[] _totalFreeSpinOddIdses    = new SortedDictionary<double, int[]>[]
        {
            new SortedDictionary<double, int[]>(),
            new SortedDictionary<double, int[]>()
        };
        protected int[]           _freeSpinTotalCounts      = new int[] { 0, 0 };
        protected int[]           _minFreeSpinTotalCounts   = new int[] { 0, 0 };
        protected double[]        _totalFreeSpinWinRates    = new double[] { 0.0, 0.0 }; //스핀디비안의 모든 프리스핀들의 배당평균값
        protected double[]        _minFreeSpinWinRates      = new double[] { 0.0, 0.0 }; //구매금액의 20% - 50%사이에 들어가는 모든 프리스핀들의 평균배당값

        public GoldenUnicornDeluxeGameLogic()
        {
            _gameID     = GAMEID.GoldenUnicornDeluxe;
            GameName    = "GoldenUnicornDeluxe";
        }

        protected override async Task<bool> loadSpinData()
        {
            try
            {
                _spinDatabase = Context.ActorOf(Akka.Actor.Props.Create(() => new SpinDatabase(_providerName, this.GameName)), "spinDatabase");
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
                    new Dictionary<double, List<int>>()
                };

                double[]    freeSpinTotalOdds        = new double[] {0.0, 0.0};
                double[]    minFreeSpinTotalOdds     = new double[] {0.0, 0.0};
                int[]       freeSpinTotalCounts      = new int[] { 0, 0 };
                int[]       minFreeSpinTotalCounts   = new int[] { 0, 0 };

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
                        new SortedDictionary<double, int[]>()
                    };
                    for(int i = 0; i < 2; i++)
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

                //for(int i = 0; i < 2; i++)
                //{
                //    if (this.PurchaseFreeMultiple[i] > _totalFreeSpinWinRates[i])
                //        _logger.Error("freespin win rate doesn't satisfy condition {0}", this.GameName);
                //}

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

        protected override async Task onProcMessage(string strUserID, int agentID, CurrencyEnum currency, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            if (message.MsgCode == (ushort)CSMSG_CODE.CS_HABANERO_DOCLIENT)
            {
                await onDoSpin(strUserID, agentID, (int)currency, message, userBonus, userBalance, isMustLose);
            }
            else
            {
                await base.onProcMessage(strUserID, agentID, currency, message, userBonus, userBalance, isMustLose);
            }
        }

        protected override async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int agentID, BaseHabaneroSlotBetInfo betInfo, double baseBet, UserBonus userBonus)
        {
            if (userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus rangeOddBonus = userBonus as UserRangeOddEventBonus;
                OddAndIDData oddAndID = selectOddAndIDFromProbsWithRange(_totalFreeSpinOddIdses[betInfo.PurchaseFree - 1], rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd);
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

            double targetC = PurchaseFreeMultiple[betInfo.PurchaseFree - 1] * payoutRate / 100.0;
            if (targetC >= _totalFreeSpinWinRates[betInfo.PurchaseFree - 1])
                targetC = _totalFreeSpinWinRates[betInfo.PurchaseFree - 1];

            if (targetC < _minFreeSpinWinRates[betInfo.PurchaseFree - 1])
                targetC = _minFreeSpinWinRates[betInfo.PurchaseFree - 1];

            double x = (_totalFreeSpinWinRates[betInfo.PurchaseFree - 1] - targetC) / (_totalFreeSpinWinRates[betInfo.PurchaseFree - 1] - _minFreeSpinWinRates[betInfo.PurchaseFree - 1]);

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
                OddAndIDData oddAndID = selectOddAndIDFromProbsWithRange(_totalFreeSpinOddIdses[betInfo.PurchaseFree - 1], _minFreeSpinTotalCounts[betInfo.PurchaseFree - 1], PurchaseFreeMultiple[betInfo.PurchaseFree - 1] * 0.2, PurchaseFreeMultiple[betInfo.PurchaseFree - 1] * 0.5);
                return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in GoldenUnicornDeluxeGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }

        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BaseHabaneroSlotBetInfo betInfo)
        {
            try
            {
                OddAndIDData oddAndID = selectOddAndIDFromProbs(_totalFreeSpinOddIdses[betInfo.PurchaseFree - 1], _freeSpinTotalCounts[betInfo.PurchaseFree - 1]);
                return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in GoldenUnicornDeluxeGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);

            BaseHabaneroSlotBetInfo betInfo = _dicUserBetInfos[strUserId];

            if((string)eventItem["gamemode"] == "CHEST")
            {
                int multiplier = 1;
                for(int i = 1; i <= currentIndex; i++)
                {
                    dynamic stepResponse = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strUserId].Responses[i].Response);
                    if (stepResponse["chestPickResult"] == 1)
                        multiplier++;
                    if(i == currentIndex)
                    {
                        JArray subEvents        = new JArray();
                        JObject subEventItem    = new JObject();
                        subEventItem["type"]                = "pick";
                        subEventItem["wincash"]             = stepResponse["wincash"];
                        subEventItem["wincashmultiplier"]   = (long)(stepResponse["wincash"] / betInfo.BetLevel * betInfo.CoinValue);
                        switch ((int)stepResponse["chestPickResult"])
                        {
                            case 1:
                                subEventItem["symbol"] = "GemBlue";
                                break;
                            case 2:
                                subEventItem["symbol"] = "GemGreen";
                                break;
                            case 3:
                                subEventItem["symbol"] = "GemRed";
                                break;
                        }
                        subEvents.Add(subEventItem);
                        eventItem["subevents"] = subEvents;
                    }
                }
                eventItem["multiplier"] = multiplier;
            }
            return eventItem;
        }

        protected override JArray buildHabaneroLogReels(string strUserId, int currentIndex, dynamic response, bool containWild = false)
        {
            JArray reels = base.buildHabaneroLogReels(strUserId, currentIndex, response as JObject, containWild);

            for (int j = 0; j < response["virtualreels"].Count; j++)
            {
                JArray col = new JArray();
                for (int k = 2; k < response["virtualreels"][j].Count - 2; k++)
                {
                    int symbol = Convert.ToInt32(response["virtualreels"][j][k]);
                    if(symbol == 2)
                    {
                        if(k < 4)
                            reels[j][k - 1] = "";
                        else
                        {
                            reels[j][1] = SymbolIdStringForLog[symbol].id;
                            reels[j][2] = "";
                        }
                    }
                }
            }
            return reels;
        }
        
        protected override JArray buildInitResumeGame(string strUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            dynamic lastContext = lastResult as dynamic;
            dynamic resumeGames = base.buildInitResumeGame(strUserID, betInfo, lastResult, gameinstanceid, roundid, currentAction);
            int multiplier = 1;
            
            if(betInfo.PurchaseFree > 0)
                resumeGames[0]["GoldenUnicornDeluxe_featureBuy"] = betInfo.PurchaseFree;

            string gamemode = lastContext["nextgamestate"];
            resumeGames[0]["gamemode"] = gamemode;
            if (string.Equals(gamemode, "chest"))
            {
                multiplier = findChestMultiplier(strUserID);
                resumeGames[0]["virtualreels"] = findVirtualReelsForChest(strUserID);
            }
            
            resumeGames[0]["multiplier"] = multiplier;

            return resumeGames;
        }

        private int findChestMultiplier(string strUserID)
        {
            int multiplier = 1;

            if (!_dicUserHistory.ContainsKey(strUserID) || _dicUserHistory[strUserID].Responses.Count == 0)
                return multiplier;

            for(int i = 0; i < _dicUserHistory[strUserID].Responses.Count; i++)
            {
                dynamic response = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strUserID].Responses[i].Response);
                if ((int)response["chestPickResult"] == 1)
                    multiplier++;
            }
            return multiplier;
        }

        private JArray findVirtualReelsForChest(string strUserID)
        {
            if (!_dicUserHistory.ContainsKey(strUserID) || _dicUserHistory[strUserID].Responses.Count == 0)
                return new JArray();

            dynamic firstResult = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strUserID].Responses[0].Response);
            return firstResult["virtualreels"];
        }
    }
}
