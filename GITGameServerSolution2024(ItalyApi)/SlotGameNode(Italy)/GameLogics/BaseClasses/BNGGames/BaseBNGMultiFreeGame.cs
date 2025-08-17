using Akka.Actor;
using GITProtocol.Utils;
using GITProtocol;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCGSharp;
using System.IO;

namespace SlotGamesNode.GameLogics
{
    class BaseBNGMultiFreeGame : BaseBNGSlotGame
    {
        //프리스핀구매기능이 있을떄만 필요하다. 디비안의 모든 프리스핀들의 오드별 아이디어레이
        protected SortedDictionary<double, int[]> [] _multiTotalFreeSpinOddIds   = null;
        protected int                             [] _multiFreeSpinTotalCounts   = null;
        protected int                             [] _multiMinFreeSpinTotalCounts = null;
        protected double                          [] _multiTotalFreeSpinWinRates = null; //스핀디비안의 모든 프리스핀들의 배당평균값
        protected double                          [] _multiMinFreeSpinWinRates   = null; //구매금액의 20% - 50%사이에 들어가는 모든 프리스핀들의 평균배당값


        public virtual int FreeSpinTypeCount
        {
            get { return 3; }
        }
        protected double[] PurMutiples
        {
            get
            {
                return new double[] { 100, 200, 400 };
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected virtual int getFreeSpinTypeFromScatterNum(int scatterNum)
        {
            int freeSpinType = scatterNum - 3;
            if (freeSpinType < 0)
                freeSpinType = 0;
            if (freeSpinType >= FreeSpinTypeCount)
                freeSpinType = FreeSpinTypeCount - 1;
            return freeSpinType;
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

                ReadSpinInfoResponse response = await _spinDatabase.Ask<ReadSpinInfoResponse>(new ReadSpinInfoRequest(HasSelectableFreeSpin ? SpinDataTypes.SelFreeSpin : SpinDataTypes.Normal), TimeSpan.FromSeconds(30.0));
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

                Dictionary<double, List<int>> totalSpinOddIds = new Dictionary<double, List<int>>();

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
                }
                _totalSpinOddIds = new SortedDictionary<double, int[]>();
                foreach (KeyValuePair<double, List<int>> pair in totalSpinOddIds)
                    _totalSpinOddIds.Add(pair.Key, pair.Value.ToArray());

                _multiTotalFreeSpinOddIds   = new SortedDictionary<double, int[]>[FreeSpinTypeCount];
                _multiFreeSpinTotalCounts   = new int[FreeSpinTypeCount];
                _multiTotalFreeSpinWinRates = new double[FreeSpinTypeCount];
                _multiMinFreeSpinWinRates   = new double[FreeSpinTypeCount];
                _multiMinFreeSpinTotalCounts= new int[FreeSpinTypeCount];

                for (int i = 0; i < FreeSpinTypeCount; i++)
                {
                    double  freeSpinTotalOdd        = 0.0;
                    double  minFreeSpinTotalOdd     = 0.0;
                    int     freeSpinTotalCount      = 0;
                    int     minFreeSpinTotalCount   = 0;

                    Dictionary<double, List<int>> freeSpinOddIds = new Dictionary<double, List<int>>();

                    List<SpinBaseData> freeSpinDatas = await _spinDatabase.Ask<List<SpinBaseData>>(new ReadSpinInfoMultiPurEnabledRequest(i), TimeSpan.FromSeconds(30.0));
                    for (int j = 0; j < freeSpinDatas.Count; j++)
                    {
                        freeSpinTotalOdd += freeSpinDatas[j].Odd;
                        freeSpinTotalCount++;

                        if (!freeSpinOddIds.ContainsKey(freeSpinDatas[j].Odd))
                            freeSpinOddIds.Add(freeSpinDatas[j].Odd, new List<int>());
                        freeSpinOddIds[freeSpinDatas[j].Odd].Add(freeSpinDatas[j].ID);
                        if (freeSpinDatas[j].Odd >= PurMutiples[i] * 0.2 && freeSpinDatas[j].Odd <= PurMutiples[i] * 0.5)
                        {
                            minFreeSpinTotalCount++;
                            minFreeSpinTotalOdd += freeSpinDatas[j].Odd;
                        }
                    }

                    _multiTotalFreeSpinOddIds[i] = new SortedDictionary<double, int[]>();
                    foreach (KeyValuePair<double, List<int>> pair in freeSpinOddIds)
                        _multiTotalFreeSpinOddIds[i].Add(pair.Key, pair.Value.ToArray());

                    _multiFreeSpinTotalCounts[i]    = freeSpinTotalCount;
                    _multiMinFreeSpinTotalCounts[i]  = minFreeSpinTotalCount;
                    _multiTotalFreeSpinWinRates[i]  = freeSpinTotalOdd / freeSpinTotalCount;
                    _multiMinFreeSpinWinRates[i]    = minFreeSpinTotalOdd / minFreeSpinTotalCount;

                    if (_multiTotalFreeSpinWinRates[i] <= _multiMinFreeSpinWinRates[i] || _multiMinFreeSpinTotalCounts[i] == 0)
                        _logger.Error("min freespin rate doesn't satisfy condition {0}", this.GameName);

                    if (this.PurMutiples[i] > _multiTotalFreeSpinWinRates[i])
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

        protected override void readBetInfoFromMessage(GITMessage message, string strUserID, BNGActionTypes action)
        {
            try
            {
                float betPerLine = (float)(double)message.Pop();
                int   lineCount  = (int) message.Pop();
                int   scatterNum = 0;
                if(action == BNGActionTypes.BUYSPIN)
                    scatterNum = (int) message.Pop();

                BaseBNGSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strUserID, out oldBetInfo))
                {
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse || (action != BNGActionTypes.SPIN && action != BNGActionTypes.BUYSPIN))
                        return;

                    if (betPerLine <= 0.0f)
                    {
                        _logger.Error("{0} betInfo.BetPerLine <= 0 in BaseBNGSlotGame::readBetInfoFromMessage {1}", strUserID, betPerLine);
                        return;
                    }

                    if (lineCount != this.ClientReqLineCount)
                    {
                        _logger.Error("{0} betInfo.LineCount is not matched {1}", strUserID, lineCount);
                        return;
                    }

                    oldBetInfo.BetPerLine   = betPerLine;
                    oldBetInfo.LineCount    = lineCount;
                    if(action == BNGActionTypes.BUYSPIN)
                        (oldBetInfo as BaseBNGMultiFreeSlotBetInfo).FreeSpinType = getFreeSpinTypeFromScatterNum(scatterNum);

                }
                else if (action == BNGActionTypes.SPIN || action == BNGActionTypes.BUYSPIN)
                {
                    if (betPerLine <= 0.0f)
                    {
                        _logger.Error("{0} betInfo.BetPerLine <= 0 in BaseBNGSlotGame::readBetInfoFromMessage {1}", strUserID, betPerLine);
                        return;
                    }
                    if (lineCount != this.ClientReqLineCount)
                    {
                        _logger.Error("{0} betInfo.LineCount is not matched {1}", strUserID, lineCount);
                        return;
                    }
                    BaseBNGMultiFreeSlotBetInfo betInfo = new BaseBNGMultiFreeSlotBetInfo();
                    betInfo.BetPerLine                  = betPerLine;
                    betInfo.LineCount                   = lineCount;
                    if(action == BNGActionTypes.BUYSPIN)
                        betInfo.FreeSpinType = getFreeSpinTypeFromScatterNum(scatterNum);
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseBNGSlotGame::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BaseBNGSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            BaseBNGMultiFreeSlotBetInfo betInfo = new BaseBNGMultiFreeSlotBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override async Task spinGame(string strUserID, int agentID,int currency, UserBonus userBonus, double userBalance, string strRequestID, string strToken, BNGPlayOriginData originData, BNGActionTypes action)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                //해당 유저의 베팅정보를 얻는다. 만일 베팅정보가 없다면(례외상황) 그대로 리턴한다.
                BaseBNGSlotBetInfo baseBetInfo = null;
                if (!_dicUserBetInfos.TryGetValue(strGlobalUserID, out baseBetInfo))
                    return;

                BaseBNGMultiFreeSlotBetInfo betInfo = baseBetInfo as BaseBNGMultiFreeSlotBetInfo;
                byte[] betInfoBytes = backupBetInfo(betInfo);

                BaseBNGSlotSpinResult lastResult = null;
                if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                    lastResult = _dicUserResultInfos[strGlobalUserID];

                double betMoney = betInfo.TotalBet;
                if (action == BNGActionTypes.BUYSPIN)
                    betMoney = betMoney * PurMutiples[betInfo.FreeSpinType];

                if (betInfo.HasRemainResponse)
                    betMoney = 0.0;

                //만일 베팅머니가 유저의 밸런스보다 크다면 끝낸다.(2020.02.15)
                if (userBalance.LT(betMoney, _epsilion) || betMoney < 0.0)
                {
                    string strBalanceErrorResult = makeBalanceNotEnoughResult(strGlobalUserID, currency, userBalance, strToken, strRequestID, originData, betInfo);
                    GITMessage message = new GITMessage((ushort)SCMSG_CODE.SC_BNG_DOPLAY);
                    message.Append(strBalanceErrorResult);
                    Sender.Tell(new ToUserMessage((int)_gameID, message));
                    saveBetResultInfo(strGlobalUserID);
                    return;
                }

                //결과를 생성한다.
                BaseBNGSlotSpinResult spinResult = await this.generateSpinResult(betInfo, strUserID, currency, agentID, userBonus, true, action);

                //게임로그
                string strGameLog = spinResult.ResultString;
                _dicUserResultInfos[strGlobalUserID] = spinResult;

                //결과를 보내기전에 베팅정보를 디비에 보관한다.(2018.06.12)
                saveBetResultInfo(strGlobalUserID);

                //생성된 게임결과를 유저에게 보낸다.
                sendGameResult(betInfo, spinResult, strGlobalUserID, currency, betMoney, spinResult.WinMoney, strGameLog, userBalance, strRequestID, strToken, originData,UserBetTypes.Normal);

                _dicUserLastBackupBetInfos[strGlobalUserID]     = betInfoBytes;
                _dicUserLastBackupResultInfos[strGlobalUserID]  = lastResult;

                //게임결과를 디비에 보관한다.
                saveResultToHistory(spinResult, agentID, strUserID, currency, userBalance, betMoney, spinResult.WinMoney, action);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseBNGMultiFreeSlotGame::spinGame {0}", ex);
            }
        }
        protected override async Task<BaseBNGSlotSpinResult> generateSpinResult(BaseBNGSlotBetInfo betInfo, string strUserID, int agentID, int currency, UserBonus userBonus, bool usePayLimit, BNGActionTypes action)
        {
            BasePPSlotSpinData      spinData    = null;
            BaseBNGSlotSpinResult   result      = null;

            string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);

            if (betInfo.HasRemainResponse)
            {
                BaseBNGActionToResponse nextResponse = betInfo.pullRemainResponse();
                result = calculateResult(strGlobalUserID, currency, betInfo, nextResponse.Response, false, action);

                //프리게임이 끝났는지를 검사한다.
                if (!betInfo.HasRemainResponse)
                    betInfo.RemainReponses = null;
                return result;
            }

            //유저의 총 베팅액을 얻는다.
            float   totalBet     = betInfo.TotalBet;
            double  realBetMoney = totalBet;
            if (action == BNGActionTypes.BUYSPIN && SupportPurchaseFree)
                realBetMoney = totalBet * PurMutiples[(betInfo as BaseBNGMultiFreeSlotBetInfo).FreeSpinType]; 

            spinData = await selectRandomStop(agentID, userBonus, totalBet, betInfo, action);

            //첫자료를 가지고 결과를 계산한다.
            double totalWin = totalBet * spinData.SpinOdd;

            if (!usePayLimit || spinData.IsEvent || checkCompanyPayoutRate(agentID, realBetMoney, totalWin))
            {
                if (spinData.IsEvent)
                {
                    _bonusSendMessage = null;
                    _rewardedBonusMoney = totalWin;
                    _isRewardedBonus = true;
                }

                result = calculateResult(strGlobalUserID, currency, betInfo, spinData.SpinStrings[0], true, action);
                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
                return result;
            }

            double emptyWin = 0.0;
            if (SupportPurchaseFree && action == BNGActionTypes.BUYSPIN)
            {
                spinData = await selectMinStartFreeSpinData(betInfo);
                result   = calculateResult(strGlobalUserID, currency, betInfo, spinData.SpinStrings[0], true, action);
                emptyWin = totalBet * spinData.SpinOdd;

                //뒤에 응답자료가 또 있다면
                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
            }
            else
            {
                spinData = await selectEmptySpin(agentID, betInfo);
                result = calculateResult(strGlobalUserID, currency, betInfo, spinData.SpinStrings[0], true, action);
            }
            sumUpCompanyBetWin(agentID, realBetMoney, emptyWin);
            return result;
        }

        protected override async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int companyID, BaseBNGSlotBetInfo baseBetInfo, double baseBet, UserBonus userBonus)
        {
            BaseBNGMultiFreeSlotBetInfo betInfo = baseBetInfo as BaseBNGMultiFreeSlotBetInfo;
            int freeSpinType = betInfo.FreeSpinType;
            if (userBonus != null && userBonus is UserRangeOddEventBonus)
            {

                UserRangeOddEventBonus rangeOddBonus = userBonus as UserRangeOddEventBonus;
                OddAndIDData oddAndID = selectOddAndIDFromProbsWithRange(_multiTotalFreeSpinOddIds[freeSpinType], rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd);
                if (oddAndID != null)
                {
                    BasePPSlotSpinData spinDataEvent = await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
                    spinDataEvent.IsEvent = true;
                    return spinDataEvent;
                }
            }

            double payoutRate = _config.PayoutRate;
            if (_agentPayoutRates.ContainsKey(companyID))
                payoutRate = _agentPayoutRates[companyID];

            double targetC = PurMutiples[freeSpinType] * payoutRate / 100.0;
            if (targetC >= _multiTotalFreeSpinWinRates[freeSpinType])
                targetC = _multiTotalFreeSpinWinRates[freeSpinType];

            if (targetC < _multiMinFreeSpinWinRates[freeSpinType])
                targetC = _multiMinFreeSpinWinRates[freeSpinType];

            double x = (_multiTotalFreeSpinWinRates[freeSpinType] - targetC) / (_multiTotalFreeSpinWinRates[freeSpinType] - _multiMinFreeSpinWinRates[freeSpinType]);
            BasePPSlotSpinData spinData = null;
            if (Pcg.Default.NextDouble(0.0, 1.0) <= x)
                spinData = await selectMinStartFreeSpinData(betInfo);
            else
                spinData = await selectRandomStartFreeSpinData(betInfo);
            return spinData;
        }
        protected override async Task<BasePPSlotSpinData> selectMinStartFreeSpinData(BaseBNGSlotBetInfo betInfo)
        {
            try
            {
                int freeSpinType = (betInfo as BaseBNGMultiFreeSlotBetInfo).FreeSpinType;
                OddAndIDData oddAndID = selectOddAndIDFromProbsWithRange(_multiTotalFreeSpinOddIds[freeSpinType], _multiMinFreeSpinTotalCounts[freeSpinType], 
                    PurMutiples[freeSpinType] * 0.2, PurMutiples[freeSpinType] * 0.5);
                return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseBNGMultiFreeSlotGame::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BaseBNGSlotBetInfo betInfo)
        {
            try
            {
                int freeSpinType        = (betInfo as BaseBNGMultiFreeSlotBetInfo).FreeSpinType;
                OddAndIDData oddAndID   = selectOddAndIDFromProbs(_multiTotalFreeSpinOddIds[freeSpinType], _multiFreeSpinTotalCounts[freeSpinType]);;
                return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseBNGMultiFreeSlotGame::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }

    }
}
