using Akka.Actor;
using GITProtocol;
using GITProtocol.Utils;
using Newtonsoft.Json;
using PCGSharp;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace SlotGamesNode.GameLogics
{
    public class BasePPSlotGame : IGameLogicActor
    {
        protected string                            _providerName           = "pragmatic";
        //스핀디비관리액터
        protected IActorRef                         _spinDatabase           = null;        
        protected double                            _spinDataDefaultBet     = 0.0f;

        protected int                               _normalMaxID            = 0;
        protected int                               _naturalSpinCount       = 0;
        protected SortedDictionary<double, int>     _naturalSpinOddProbs    = new SortedDictionary<double, int>();
        protected SortedDictionary<double, int[]>   _totalSpinOddIds        = new SortedDictionary<double, int[]>();
        protected List<int>                         _emptySpinIDs           = new List<int>();

        //프리스핀구매기능이 있을떄만 필요하다. 디비안의 모든 프리스핀들의 오드별 아이디어레이
        protected SortedDictionary<double, int[]>   _totalFreeSpinOddIds    = new SortedDictionary<double, int[]>();
        protected int                               _freeSpinTotalCount     = 0;
        protected int                               _minFreeSpinTotalCount  = 0;
        protected double                            _totalFreeSpinWinRate   = 0.0; //스핀디비안의 모든 프리스핀들의 배당평균값
        protected double                            _minFreeSpinWinRate     = 0.0; //구매금액의 20% - 50%사이에 들어가는 모든 프리스핀들의 평균배당값

        //앤티베팅기능이 있을때만 필요하다.(앤티베팅시 감소시켜야할 빈스핀의 갯수)
        protected int                               _anteBetMinusZeroCount = 0;


        //매유저의 베팅정보 
        protected Dictionary<string, BasePPSlotBetInfo>     _dicUserBetInfos                    = new Dictionary<string, BasePPSlotBetInfo>();

        //유저의 게임이력정보
        protected Dictionary<string, BasePPHistory>         _dicUserHistory                 = new Dictionary<string, BasePPHistory>();

        //유정의 마지막결과정보
        protected Dictionary<string, BasePPSlotSpinResult>  _dicUserResultInfos             = new Dictionary<string, BasePPSlotSpinResult>();

        //유저의 설정정보
        protected Dictionary<string, string>                _dicUserSettings                = new Dictionary<string, string>();

        //백업정보
        protected Dictionary<string, BasePPSlotSpinResult>  _dicUserLastBackupResultInfos   = new Dictionary<string, BasePPSlotSpinResult>();
        protected Dictionary<string, byte[]>                _dicUserLastBackupBetInfos      = new Dictionary<string, byte[]>();
        protected Dictionary<string, byte[]>                _dicUserLastBackupHistory       = new Dictionary<string, byte[]>();


        protected string _defaultSetting = "SoundState=true_true_true_true_true;FastPlay=false;Intro=true;StopMsg=0;TurboSpinMsg=0;BetInfo=0_0;BatterySaver=false;ShowCCH=true;ShowFPH=true;CustomGameStoredData=;Coins=false;Volume=1;InitialScreen=8,9,6_5,3,7_7,5,4_3,6,3_9,7,8;SBPLock=true";

        protected virtual int FreeSpinTypeCount
        {
            get
            {
                return 0; //유저가 선택가능한 프리스핀종류수
            }
        }
        protected virtual bool HasPurEnableOption
        {
            get { return false; }
        }

        protected virtual bool HasSelectableFreeSpin
        {
            get
            {
                return false;
            }
        }
        protected virtual string SymbolName
        {
            get
            {
                return "";
            }
        }
        protected virtual bool SupportReplay
        {
            get
            {
                return true;
            }
        }
        protected virtual int ClientReqLineCount
        {
            get
            {
                return 20;
            }
        }
        protected virtual int ServerResLineCount
        {
            get { return 0; }
        }
        protected virtual int ROWS
        {
            get
            {
                return 3;
            }
        }
        protected virtual string InitDataString
        {
            get
            {
                return "";
            }
        }
        protected virtual bool SupportPurchaseFree
        {
            get { return false; }
        }
        protected virtual double PurchaseFreeMultiple
        {
            get { return 0.0; }
        }
        protected virtual bool SupportMoreBet
        {
            get { return false;  }
        }
        protected virtual double MoreBetMultiple
        {
            get { return 0.0; }
        }

        public BasePPSlotGame()
        {
            ReceiveAsync<LoadSpinDataRequest>       (onLoadSpinData);
            ReceiveAsync<PerformanceTestRequest>    (onPerformanceTest);
        }

        protected virtual async Task onPerformanceTest(PerformanceTestRequest _)
        {
            try
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                //자연빵 1만개스핀 선택
                double sumOdd1 = 0.0;

                BasePPSlotBetInfo betInfo = new BasePPSlotBetInfo();
                for (int i = 0; i < 1000000; i++)
                {
                    BasePPSlotSpinData spinData = await selectRandomStop(0, betInfo, false);
                    sumOdd1 += spinData.SpinOdd;
                }
                stopWatch.Stop();
                long elapsed1 = stopWatch.ElapsedMilliseconds;

                stopWatch.Reset();
                stopWatch.Start();

                double sumOdd2 = 0.0;
                //MoreBet 1만개
                for (int i = 0; i < 1000000; i++)
                {
                    BasePPSlotSpinData spinData = await selectRandomStop(0, betInfo, true);
                    sumOdd2 += spinData.SpinOdd;
                }
                stopWatch.Stop();
                long elapsed2 = stopWatch.ElapsedMilliseconds;

                stopWatch.Reset();
                stopWatch.Start();

                double sumOdd3 = 0.0;
                if (SupportPurchaseFree)
                {
                    for (int i = 0; i < 1000000; i++)
                    {
                        BasePPSlotSpinData spinData = await selectPurchaseFreeSpin(0, betInfo, 0.0, null);
                        sumOdd3 += spinData.SpinOdd;
                    }
                }

                stopWatch.Stop();
                long elapsed3 = stopWatch.ElapsedMilliseconds;

                stopWatch.Reset();
                stopWatch.Start();
                //이벤트각 구간마다 2천개
                for (int i = 0; i < 6; i++)
                {
                    double[] rangeMins = new double[] { 10, 50, 100, 300, 500, 1000 };
                    double[] rangeMaxs = new double[] { 50, 100, 300, 500, 1000, 3000 };

                    for (int j = 0; j < 1000; j++)
                    {
                        OddAndIDData oddAndID = selectOddAndIDFromProbsWithRange(_totalSpinOddIds, rangeMins[i], rangeMaxs[i]);
                        if(oddAndID != null)
                        {
                            BasePPSlotSpinData spinDataEvent = await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
                            spinDataEvent.IsEvent = true;
                        }
                    }
                }

                stopWatch.Stop();
                long elapsed4 = stopWatch.ElapsedMilliseconds;

                _logger.Info("{0} Performance Test Results:  \r\nPayrate: {8}%, {1}s, {2}%\t{3}s {4}%\t{5}s {6}%\t{7}s", this.GameName, 
                    Math.Round((double) elapsed1 / 1000.0, 3), Math.Round(sumOdd1 / 1000000, 3),
                    Math.Round((double) elapsed2 / 1000.0, 3), Math.Round(sumOdd2 / 1000000, 3),
                    Math.Round((double) elapsed3 / 1000.0, 3), Math.Round(sumOdd3 / 1000000, 3),
                    Math.Round((double) elapsed4 / 1000.0, 3), _config.PayoutRate);
                Sender.Tell(true);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseSelFreePPSlotGame::onPerformanceTest {0}", ex);
                Sender.Tell(false);
            }
        }
        protected override void LoadSetting()
        {
            base.LoadSetting();
            initGameData();

            if (PayoutConfigSnapshot.Instance.AgentPayoutConfigs.ContainsKey(_gameID))
                _agentPayoutRates = PayoutConfigSnapshot.Instance.AgentPayoutConfigs[_gameID];
            else
                _agentPayoutRates = new Dictionary<int, double>();
        }
        protected virtual void addDefaultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter)
        {
            dicParams["balance"]        = Math.Round(userBalance, 2).ToString();        //밸런스
            dicParams["balance_cash"]   = Math.Round(userBalance, 2).ToString();        //밸런스
            dicParams["balance_bonus"]  = "0.0";
            dicParams["stime"]          = GameUtils.GetCurrentUnixTimestampMillis().ToString();
            dicParams["index"]          = index.ToString();
            dicParams["counter"]        = (counter + 1).ToString();
            dicParams["sver"]           = "5";
        }
        protected void sumUpCompanyBetWin(int companyID, double betMoney, double winMoney, int poolIndex = 0)
        {
            if (companyID == 0)
            {
                _totalBets[poolIndex] += betMoney;
                _totalWins[poolIndex] += winMoney;
            }
            else
            {
                if (!_agentTotalBets.ContainsKey(companyID))
                    _agentTotalBets[companyID] = new double[PoolCount];

                if (!_agentTotalWins.ContainsKey(companyID))
                    _agentTotalWins[companyID] = new double[PoolCount];

                _agentTotalBets[companyID][poolIndex] += betMoney;
                _agentTotalWins[companyID][poolIndex] += winMoney;
            }
        }
        protected bool checkCompanyPayoutRate(int agentID, double betMoney, double winMoney, int poolIndex = 0)
        {
            if (betMoney == 0.0 && winMoney == 0.0)
                return true;

            if (agentID == 0)
            {
                double totalBet = _totalBets[poolIndex] + betMoney;
                double totalWin = _totalWins[poolIndex] + winMoney;

                double maxTotalWin = totalBet * _config.PayoutRate / 100.0 + _config.PoolRedundency;
                if (totalWin > maxTotalWin)
                    return false;

                _totalBets[poolIndex] = totalBet;
                _totalWins[poolIndex] = totalWin;
                return true;
            }
            else
            {
                if (!_agentTotalBets.ContainsKey(agentID))
                    _agentTotalBets[agentID] = new double[PoolCount];

                if (!_agentTotalWins.ContainsKey(agentID))
                    _agentTotalWins[agentID] = new double[PoolCount];

                double companyPayoutRate = _config.PayoutRate;
                if (_agentPayoutRates.ContainsKey(agentID))
                    companyPayoutRate = _agentPayoutRates[agentID];

                double totalBet = _agentTotalBets[agentID][poolIndex] + betMoney;
                double totalWin = _agentTotalWins[agentID][poolIndex] + winMoney;

                double maxTotalWin = totalBet * companyPayoutRate / 100.0 + _config.PoolRedundency;
                if (totalWin > maxTotalWin)
                    return false;

                _agentTotalBets[agentID][poolIndex] = totalBet;
                _agentTotalWins[agentID][poolIndex] = totalWin;
                return true;
            }
        }

        protected void resetCompanyPayoutPool(int agentID)
        {
            if (!_agentTotalBets.ContainsKey(agentID) || !_agentTotalWins.ContainsKey(agentID))
                return;

            for (int i = 0; i < PoolCount; i++)
            {
                _agentTotalBets[agentID][i] = 0.0;
                _agentTotalWins[agentID][i] = 0.0;
            }
        }

        protected virtual void initGameData()
        {
        }

        protected async Task onLoadSpinData(LoadSpinDataRequest request)
        {
            bool result = await loadSpinData();
            Sender.Tell(result);
        }
        protected virtual async Task<bool> loadSpinData()
        {
            try
            {
                _spinDatabase   = Context.ActorOf(Akka.Actor.Props.Create(() => new SpinDatabase(_providerName, this.GameName)), "spinDatabase");
                bool isSuccess  = await _spinDatabase.Ask<bool>("initialize", TimeSpan.FromSeconds(5.0));
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
                Dictionary<double, List<int>> freeSpinOddIds  = new Dictionary<double, List<int>>();

                double freeSpinTotalOdd         = 0.0;
                double minFreeSpinTotalOdd      = 0.0;
                int    freeSpinTotalCount       = 0;
                int    minFreeSpinTotalCount    = 0;
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
                    if(SupportPurchaseFree && !HasPurEnableOption && spinBaseData.SpinType == 1)
                    {
                        freeSpinTotalCount++;
                        freeSpinTotalOdd += spinBaseData.Odd;
                        if (!freeSpinOddIds.ContainsKey(spinBaseData.Odd))
                            freeSpinOddIds.Add(spinBaseData.Odd, new List<int>());
                        freeSpinOddIds[spinBaseData.Odd].Add(spinBaseData.ID);

                        if(spinBaseData.Odd >= PurchaseFreeMultiple * 0.2 && spinBaseData.Odd <= PurchaseFreeMultiple * 0.5)
                        {
                            minFreeSpinTotalCount++;
                            minFreeSpinTotalOdd += spinBaseData.Odd;
                        }    
                    }
                }
                _totalSpinOddIds = new SortedDictionary<double, int[]>();
                foreach (KeyValuePair<double, List<int>> pair in totalSpinOddIds)
                    _totalSpinOddIds.Add(pair.Key, pair.Value.ToArray());

                if(SupportPurchaseFree && HasPurEnableOption)
                {
                    //따로 읽는다.
                    List<SpinBaseData>  freeSpinDatas = await _spinDatabase.Ask<List<SpinBaseData>>(new ReadSpinInfoPurEnabledRequest(), TimeSpan.FromSeconds(30.0));
                    for(int i = 0; i < freeSpinDatas.Count; i++)
                    {
                        freeSpinTotalOdd += freeSpinDatas[i].Odd;
                        freeSpinTotalCount++;

                        if (!freeSpinOddIds.ContainsKey(freeSpinDatas[i].Odd))
                            freeSpinOddIds.Add(freeSpinDatas[i].Odd, new List<int>());
                        freeSpinOddIds[freeSpinDatas[i].Odd].Add(freeSpinDatas[i].ID);
                        if (freeSpinDatas[i].Odd >= PurchaseFreeMultiple * 0.2 && freeSpinDatas[i].Odd <= PurchaseFreeMultiple * 0.5)
                        {
                            minFreeSpinTotalCount++;
                            minFreeSpinTotalOdd += freeSpinDatas[i].Odd;
                        }
                    }
                }
                if(SupportPurchaseFree)
                {
                    _totalFreeSpinOddIds = new SortedDictionary<double, int[]>();
                    foreach (KeyValuePair<double, List<int>> pair in freeSpinOddIds)
                        _totalFreeSpinOddIds.Add(pair.Key, pair.Value.ToArray());

                    _freeSpinTotalCount     = freeSpinTotalCount;
                    _minFreeSpinTotalCount  = minFreeSpinTotalCount;
                    _totalFreeSpinWinRate   = freeSpinTotalOdd / freeSpinTotalCount;
                    _minFreeSpinWinRate     = minFreeSpinTotalOdd / minFreeSpinTotalCount;

                    if (_totalFreeSpinWinRate <= _minFreeSpinWinRate || _minFreeSpinTotalCount == 0)
                        _logger.Error("min freespin rate doesn't satisfy condition {0}", this.GameName);
                }

                if (this.SupportMoreBet)
                {
                    int naturalEmptyCount   = _naturalSpinOddProbs[0.0];
                    _anteBetMinusZeroCount  = (int)((1.0 - 1.0 / MoreBetMultiple) * (double)_naturalSpinCount);

                    double moreBetWinRate = 0.0;
                    foreach (KeyValuePair<double, int> pair in _naturalSpinOddProbs)
                    {
                        moreBetWinRate += (pair.Key * pair.Value / (_naturalSpinCount - _anteBetMinusZeroCount));
                    }
                    if (_anteBetMinusZeroCount > naturalEmptyCount)
                        _logger.Error("More Bet Probabily calculation doesn't work in {0}", this.GameName);
                }

                if (this.SupportPurchaseFree && this.PurchaseFreeMultiple > _totalFreeSpinWinRate)
                    _logger.Error("freespin win rate doesn't satisfy condition {0}", this.GameName);

                double winRate = 0.0;
                foreach (KeyValuePair<double, int> pair in _naturalSpinOddProbs)
                    winRate += (pair.Key * pair.Value / _naturalSpinCount);

                /*
                _logger.Info("Loading completed spin data of game {0} winrate: {1}, freespinwinrate: {2}, antezerominuscount: {3}, empty spin count: {4}, minfreespinrate: {5}",
                        this.GameName, Math.Round(winRate, 2), Math.Round(_totalFreeSpinWinRate, 2), _anteBetMinusZeroCount, _emptySpinIDs.Count, Math.Round(_minFreeSpinWinRate, 2));
                */
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
                return false;
            }
        }

        #region 메세지처리함수들
        protected override async Task onProcMessage(string strUserID, int agentID, CurrencyEnum currency, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
            if (message.MsgCode == (ushort)CSMSG_CODE.CS_PP_DOINIT)
            {
                onDoInit(strGlobalUserID, (int)currency, message, userBonus, userBalance, isMustLose);
            }
            else if (message.MsgCode == (ushort)CSMSG_CODE.CS_PP_SAVESETTING)
            {
                onSaveSetting(strGlobalUserID, message, userBonus, userBalance, isMustLose);
            }
            else if (message.MsgCode == (ushort)CSMSG_CODE.CS_PP_DOSPIN)
            {
                await onDoSpin(strUserID, agentID, (int)currency, message, userBonus, userBalance, isMustLose);
            }
            else if (message.MsgCode == (ushort)CSMSG_CODE.CS_PP_DOCOLLECT)
            {
                onDoCollect(strUserID,agentID ,message, userBalance);
            }
            else if (message.MsgCode == (ushort)CSMSG_CODE.CS_PP_RELOADBALANCE)
            {
                onReloadBalance(strUserID, message, userBalance);
            }
            else if (message.MsgCode == (ushort)CSMSG_CODE.CS_PP_NOTPROCDRESULT)
            {
                onDoUndoUserSpin(strGlobalUserID);                
            }
            else if(message.MsgCode == (ushort)CSMSG_CODE.CS_PP_DOBONUS)
            {
                onDoBonus(strUserID, agentID, message, userBalance);
            }
            else if(message.MsgCode == (ushort)CSMSG_CODE.CS_PP_DOCOLLECTBONUS)
            {
                onDoCollectBonus(strUserID, agentID, message, userBalance);
            }
        }
        protected virtual void onDoInit(string strGlobalUserID, int currency, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            try
            {
                int index           = (int)message.Pop();
                int counter         = (int)message.Pop();

                string strResponse  = genInitResponse(strGlobalUserID, currency, userBalance, index, counter, false);

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOINIT);
                responseMessage.Append(strResponse);

                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::onDoInit GameID: {0}, {1}", _gameID, ex);
            }
        }
        protected virtual void onReloadBalance(string strUserID, GITMessage message, double userBalance)
        {
            try
            {
                Dictionary<string, string> responseParams = new Dictionary<string, string>();
                responseParams.Add("balance",       Math.Round(userBalance, 2).ToString());
                responseParams.Add("balance_cash",  Math.Round(userBalance, 2).ToString());
                responseParams.Add("balance_bonus", "0.00");
                responseParams.Add("stime",         GameUtils.GetCurrentUnixTimestampMillis().ToString());
                GITMessage reponseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_RELOADBALANCE);
                reponseMessage.Append(convertKeyValuesToString(responseParams));
                Sender.Tell(new ToUserMessage((int)_gameID, reponseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::onDoCollect {0}", ex);
            }
        }
        protected virtual void onSaveSetting(string strGlobalUserID, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            try
            {
                bool isLoad = (bool)message.Pop();
                if (!isLoad)
                {
                    string strNewSetting                = (string)message.Pop();
                    _dicUserSettings[strGlobalUserID]   = strNewSetting;
                }

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_SAVESETTING);
                if (_dicUserSettings.ContainsKey(strGlobalUserID))
                    responseMessage.Append(_dicUserSettings[strGlobalUserID]);
                else
                    responseMessage.Append(_defaultSetting);

                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::onSaveSetting GameID: {0}, {1}", _gameID, ex);
            }
        }
        protected void removeJTokenField(JToken token, string strFieldName)
        {
            JContainer container = token as JContainer;
            if (container == null)
                return;

            JToken removeToken = null;
            foreach (JToken el in container.Children())
            {
                JProperty p = el as JProperty;
                if (p != null && p.Name == strFieldName)
                {
                    removeToken = el;
                    break;
                }
            }
            if (removeToken != null)
                removeToken.Remove();
        }
        protected string serializeJsonSpecial(JToken token)
        {
            var serializer      = new JsonSerializer();
            var stringWriter    = new StringWriter();
            using (var writer = new JsonTextWriter(stringWriter))
            {
                writer.QuoteName = false;
                serializer.Serialize(writer, token);
            }
            return stringWriter.ToString();
        }

        protected virtual async Task onDoSpin(string strUserID, int agentID, int currency,GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            try
            {
                _isRewardedBonus        = false;
                _bonusSendMessage       = null;
                _rewardedBonusMoney     = 0.0;

                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                readBetInfoFromMessage(message, strGlobalUserID);
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();

                if (!_dicUserHistory.ContainsKey(strGlobalUserID))
                {
                    _dicUserHistory.Add(strGlobalUserID, new BasePPHistory());
                    _dicUserHistory[strGlobalUserID].roundid = ((long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
                }

                if (_dicUserHistory[strGlobalUserID].log.Count == 0)
                    _dicUserHistory[strGlobalUserID].init = genInitResponse(strGlobalUserID,currency, userBalance, 0, 0, true);

                await spinGame(strUserID, agentID, userBonus, isMustLose, userBalance, index, counter);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::onDoSpin GameID: {0}, {1}", _gameID, ex);
            }
        }
        protected virtual void onDoCollect(string strUserID,int agentID ,GITMessage message, double userBalance)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID))
                    return;

                BasePPSlotSpinResult result = _dicUserResultInfos[strGlobalUserID];
                if (result.NextAction != ActionTypes.DOCOLLECT)
                    return;

                int index                                = (int)message.Pop();
                int counter                              = (int)message.Pop();
                Dictionary<string,string> responseParams = new Dictionary<string, string>();
                responseParams.Add("balance",       Math.Round(userBalance, 2).ToString());
                responseParams.Add("balance_cash",  Math.Round(userBalance, 2).ToString());
                responseParams.Add("balance_bonus", "0.00");
                responseParams.Add("na",            "s");
                responseParams.Add("stime",         GameUtils.GetCurrentUnixTimestampMillis().ToString());
                responseParams.Add("sver",          "5");
                responseParams.Add("index",         index.ToString());
                responseParams.Add("counter",       (counter + 1).ToString());

                GITMessage reponseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOCOLLECT);
                string strCollectResponse = convertKeyValuesToString(responseParams);
                reponseMessage.Append(strCollectResponse);

                addActionHistory(strGlobalUserID, "doCollect", strCollectResponse, index, counter);
                saveHistory(strUserID,agentID ,index, counter, userBalance);

                result.NextAction = ActionTypes.DOSPIN;
                saveBetResultInfo(strGlobalUserID);
                Sender.Tell(new ToUserMessage((int)_gameID, reponseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPFreeSlotGame::onDoCollect {0}", ex);
            }
        }
        protected virtual void onDoCollectBonus(string strUserID,int agentID ,GITMessage message, double userBalance)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();

                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOCOLLECTBONUS);
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BasePPSlotBetInfo betInfo   = _dicUserBetInfos[strGlobalUserID];
                    BasePPSlotSpinResult result = _dicUserResultInfos[strGlobalUserID];
                    if (!betInfo.HasRemainResponse || result.NextAction != ActionTypes.DOCOLLECTBONUS)
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        BasePPActionToResponse actionResponse = betInfo.pullRemainResponse();
                        if (actionResponse.ActionType != ActionTypes.DOCOLLECTBONUS)
                        {
                            responseMessage.Append("unlogged");
                        }
                        else
                        {
                            Dictionary<string, string> dicParams = splitResponseToParams(actionResponse.Response);
                            addDefaultParams(dicParams, userBalance, index, counter);
                            convertWinsByBet(dicParams, betInfo.TotalBet);
                            convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);
                            responseMessage.Append(convertKeyValuesToString(dicParams));

                            result.NextAction = convertStringToActionType(dicParams["na"]);

                            //히스토리보관 및 초기화
                            if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                            {
                                addActionHistory(strGlobalUserID, "doCollectBonus", convertKeyValuesToString(dicParams), index, counter);
                                saveHistory(strUserID,agentID ,index, counter, userBalance);
                            }
                        }
                        if (!betInfo.HasRemainResponse)
                            betInfo.RemainReponses = null;

                        saveBetResultInfo(strGlobalUserID);
                    }
                    Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::onDoBonus {0}", ex);
            }
        }
        protected virtual async Task onDoBonus(string strUserID,int agentID ,GITMessage message, double userBalance)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                double realWin      = 0.0;
                string strGameLog   = "";

                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BasePPSlotSpinResult result             = _dicUserResultInfos[strGlobalUserID];
                    BasePPSlotBetInfo betInfo               = _dicUserBetInfos[strGlobalUserID];
                    BasePPActionToResponse actionResponse   = betInfo.pullRemainResponse();
                    if (actionResponse.ActionType != ActionTypes.DOBONUS)
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        Dictionary<string, string> dicParams    = splitResponseToParams(actionResponse.Response);

                        convertWinsByBet(dicParams, betInfo.TotalBet);
                        convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                        result.BonusResultString    = convertKeyValuesToString(dicParams);

                        addDefaultParams(dicParams, userBalance, index, counter);

                        ActionTypes nextAction      = convertStringToActionType(dicParams["na"]);
                        string      strResponse     = convertKeyValuesToString(dicParams);

                        responseMessage.Append(strResponse);

                        //히스토리보관 및 초기화
                        if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                            addActionHistory(strGlobalUserID, "doBonus", strResponse, index, counter);

                        if (nextAction == ActionTypes.DOCOLLECT || nextAction == ActionTypes.DOCOLLECTBONUS)
                        {
                            realWin              = double.Parse(dicParams["tw"]);
                            strGameLog           = strResponse;

                            if (realWin > 0.0f)
                            {
                                _dicUserHistory[strGlobalUserID].baseBet  = betInfo.TotalBet;
                                _dicUserHistory[strGlobalUserID].win      = realWin;
                            }
                        }
                        copyBonusParamsToResult(dicParams, result);
                        result.NextAction = nextAction;
                    }
                    if (!betInfo.HasRemainResponse)
                        betInfo.RemainReponses = null;

                    saveBetResultInfo(strGlobalUserID);
                }
                if (realWin == 0.0)
                    Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
                else
                    Sender.Tell(new ToUserResultMessage((int)_gameID, responseMessage, 0.0, realWin, new GameLogInfo(this.GameName, "0", strGameLog), UserBetTypes.Normal));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::onDoBonus {0}", ex);
            }
        }
        protected void copyBonusParamsToResult(Dictionary<string, string> dicParams, BasePPSlotSpinResult result)
        {
            Dictionary<string, string> dicResultParams = splitResponseToParams(result.ResultString);
            copyBonusParamsToResultParams(dicParams, dicResultParams);
            result.ResultString = convertKeyValuesToString(dicResultParams);
        }
        protected virtual void copyBonusParamsToResultParams(Dictionary<string, string> bonusParams, Dictionary<string, string> resultParams)
        {
           
        }
        protected void onDoUndoUserSpin(string strGlobalUserID)
        {
            undoUserResultInfo(strGlobalUserID);
            undoUserHistory(strGlobalUserID);
            undoUserBetInfo(strGlobalUserID);
            saveBetResultInfo(strGlobalUserID);
        }
        #endregion
        protected virtual void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                BasePPSlotBetInfo betInfo   = new BasePPSlotBetInfo();
                betInfo.BetPerLine          = (float)   message.Pop();
                betInfo.LineCount           = (int)     message.Pop();

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in BasePPSlotGame::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }

                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1}", strGlobalUserID, betInfo.LineCount);
                    return;
                }

                BasePPSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strGlobalUserID, out oldBetInfo))
                {
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine   = betInfo.BetPerLine;
                    oldBetInfo.LineCount    = betInfo.LineCount;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::readBetInfoFromMessage {0}", ex);
            }
        }
        protected virtual void undoUserResultInfo(string strGlobalUserID)
        {
            try
            {
                if (!_dicUserLastBackupResultInfos.ContainsKey(strGlobalUserID))
                    return;

                BasePPSlotSpinResult lastResult = _dicUserLastBackupResultInfos[strGlobalUserID];
                if (lastResult == null)
                    _dicUserResultInfos.Remove(strGlobalUserID);
                else
                    _dicUserResultInfos[strGlobalUserID] = lastResult;
                _dicUserLastBackupResultInfos.Remove(strGlobalUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::undoUserResultInfo {0}", ex);
            }
        }
        protected virtual void undoUserHistory(string strGlobalUserID)
        {
            try
            {
                if (!_dicUserLastBackupHistory.ContainsKey(strGlobalUserID))
                    return;

                byte[] userHistoryBytes = _dicUserLastBackupHistory[strGlobalUserID];
                if (userHistoryBytes == null)
                {
                    _dicUserHistory.Remove(strGlobalUserID);
                }
                else
                {
                    using (MemoryStream ms = new MemoryStream(userHistoryBytes))
                    {
                        using (BinaryReader binaryReader = new BinaryReader(ms))
                        {
                            BasePPHistory history = restoreHistory(binaryReader);
                            _dicUserHistory[strGlobalUserID] = history;
                        }
                    }
                }
                _dicUserLastBackupHistory.Remove(strGlobalUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::undoUserHistory {0}", ex);
            }
        }
        protected virtual void undoUserBetInfo(string strGlobalUserID)
        {
            try
            {
                if (!_dicUserLastBackupBetInfos.ContainsKey(strGlobalUserID))
                    return;

                byte[] userBetInfoBytes = _dicUserLastBackupBetInfos[strGlobalUserID];
                using (MemoryStream ms = new MemoryStream(userBetInfoBytes))
                {
                    using (BinaryReader binaryReader = new BinaryReader(ms))
                    {
                        BasePPSlotBetInfo betInfo   = restoreBetInfo(strGlobalUserID, binaryReader);
                        _dicUserBetInfos[strGlobalUserID] = betInfo;
                    }
                }
                _dicUserLastBackupBetInfos.Remove(strGlobalUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::undoUserBetInfo {0}", ex);
            }
        }
        protected void addActionHistory(string strGlobalUserID, string strAction, string strResponse, int index, int counter)
        {
            if (!_dicUserHistory.ContainsKey(strGlobalUserID) || _dicUserHistory[strGlobalUserID].log.Count == 0)
                return;

            if (_dicUserHistory[strGlobalUserID].bet == 0.0)
                return;

            BasePPHistoryItem item  = new BasePPHistoryItem();
            item.cr                 = string.Format("symbol={0}&repeat=0&action={3}&index={1}&counter={2}", SymbolName, index, counter, strAction);
            item.sr                 = strResponse;
            _dicUserHistory[strGlobalUserID].log.Add(item);
        }
        protected virtual string genInitResponse(string strGlobalUserID,int currency, double userBalance, int index, int counter, bool useDefault)
        {
            string strLastSpinData = "";
            if (!useDefault && _dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                strLastSpinData = makeSpinResultString(_dicUserBetInfos[strGlobalUserID], _dicUserResultInfos[strGlobalUserID], 0.0, userBalance, index, counter, true);
            }
            else
            {
                strLastSpinData = makeDefaultSpinResultString(currency, userBalance, index, counter);
            }

            return setUpInitStringByCurrency(currency) + "&" + strLastSpinData;
        }

        protected virtual string setUpInitStringByCurrency(int currency)
        {
            Dictionary<string, string> dicInitParams    = splitResponseToParams(InitDataString);
            Dictionary<string, string> dicInitNewParams = new Dictionary<string, string>();

            int rateMultiplier = new Currencies()._currencyInfo[currency].Rate;
            foreach(KeyValuePair<string, string> pair in dicInitParams)
            {
                if(pair.Key == "sc")
                {
                    List<string> sces       = pair.Value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    List<string> newSces    = new List<string>();
                    foreach(string scItem in sces)
                    {
                        double sc = Convert.ToDouble(scItem);
                        newSces.Add((sc * rateMultiplier).ToString());
                    }
                    dicInitNewParams[pair.Key] = string.Join(",", newSces);
                }
                else if(pair.Key == "total_bet_min" || pair.Key == "total_bet_max" || pair.Key == "defc")
                {
                    double value = Convert.ToDouble(pair.Value);
                    dicInitNewParams[pair.Key] = (value * rateMultiplier).ToString();
                }
                else
                {
                    dicInitNewParams[pair.Key] = dicInitParams[pair.Key];
                }
            }
            return convertKeyValuesToString(dicInitNewParams);
        }

        protected string makeDefaultSpinResultString(int currency, double userBalance, int index, int counter)
        {
            Dictionary<string, string> dicParams = new Dictionary<string, string>();
            setupDefaultResultParams(dicParams, currency, userBalance, index, counter);
            return convertKeyValuesToString(dicParams);
        }
        protected virtual void setupDefaultResultParams(Dictionary<string,string> dicParams, int currency,double userBalance, int index, int counter)
        {
            Dictionary<string, string> dicInitParams = splitResponseToParams(InitDataString);
            dicParams.Add("balance",        Math.Round(userBalance, 2).ToString());
            dicParams.Add("balance_cash",   Math.Round(userBalance, 2).ToString());
            dicParams.Add("balance_bonus",  "0.00");                                     
            dicParams.Add("na",             "s");                                        
            dicParams.Add("stime",          GameUtils.GetCurrentUnixTimestampMillis().ToString());
            dicParams.Add("sver",           "5");
            if(dicInitParams.ContainsKey("def_sa"))
                dicParams.Add("sa",         dicInitParams["def_sa"]);
            if (dicInitParams.ContainsKey("def_sb"))
                dicParams.Add("sb",         dicInitParams["def_sb"]); 
            dicParams.Add("sh",             ROWS.ToString());
            if (dicInitParams.ContainsKey("def_s"))
                dicParams.Add("s",          dicInitParams["def_s"]); //def_s
            dicParams.Add("c",              string.Format("{0:N2}", Convert.ToDouble(dicInitParams["defc"]) * new Currencies()._currencyInfo[currency].Rate));  //defc
            dicParams.Add("l",              ServerResLineCount.ToString());
            if (index > 0)
            {
                dicParams.Add("index",      index.ToString());
                dicParams.Add("counter",    (counter + 1).ToString());
            }
        }
        protected virtual void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {

        }
        protected virtual string makeSpinResultString(BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult, double betMoney, double userBalance, int index, int counter, bool isInit)
        {
            Dictionary<string, string> dicParams = splitResponseToParams(spinResult.ResultString);

            if(spinResult.HasBonusResult)
            {
                Dictionary<string, string> dicBonusParams = splitResponseToParams(spinResult.BonusResultString);
                dicParams = mergeSpinToBonus(dicParams, dicBonusParams);
            }

            dicParams["rid"]            = spinResult.RoundID;
            dicParams["balance_bonus"]  = "0.00";
            dicParams["stime"]          = GameUtils.GetCurrentUnixTimestampMillis().ToString();
            dicParams["sver"]           = "5";
            dicParams["l"]              = ServerResLineCount.ToString();
            dicParams["sh"]             = ROWS.ToString();
            dicParams["c"]              = Math.Round(betInfo.BetPerLine, 2).ToString();
            if (index > 0)
            {
                dicParams["index"]      =  index.ToString();
                dicParams["counter"]    = (counter + 1).ToString();
            }

            ActionTypes nextAction = convertStringToActionType(dicParams["na"]);
            if (isInit)
            {
                dicParams["na"]     = convertActionTypeToString(spinResult.NextAction);
                dicParams["action"] = convertActionTypeToFullString(spinResult.NextAction);
            }
            else
            {
                dicParams["na"]     = convertActionTypeToString(spinResult.NextAction);
            }

            dicParams["balance"]        = Math.Round(userBalance - (isInit ? 0.0 : betMoney), 2).ToString();        //밸런스
            dicParams["balance_cash"]   = Math.Round(userBalance - (isInit ? 0.0 : betMoney), 2).ToString();        //밸런스케시

            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = "0";

            if(SupportMoreBet)
            {
                if(betInfo.MoreBet)
                    dicParams["bl"] = "1";
                else
                    dicParams["bl"] = "0";
            }
            if (isInit)
                supplementInitResult(dicParams, betInfo, spinResult);

            overrideSomeParams(betInfo, dicParams);
            return convertKeyValuesToString(dicParams);
        }

        protected virtual void supplementInitResult(Dictionary<string, string> dicParams, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {

        }
        protected virtual Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string,string> bonusParams)
        {
            return spinParams;
        }
        protected string convertWinByBet(string strWin, float currentBet)
        {
            double win = double.Parse(strWin);
            win = win / _spinDataDefaultBet * currentBet;
            return Math.Round(win, 2).ToString();
        }
        protected virtual void convertBetsByBet(Dictionary<string, string> dicParams, float betPerLine, float totalBet)
        {
            if (dicParams.ContainsKey("coef"))
            {
                double coef = Math.Round(double.Parse(dicParams["coef"]), 2);
                if (coef == _spinDataDefaultBet)
                    dicParams["coef"] = Math.Round(totalBet, 2).ToString();
                else
                    dicParams["coef"] = Math.Round(betPerLine, 2).ToString();
            }
        }
        protected virtual void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            if(dicParams.ContainsKey("tw"))
                dicParams["tw"] = convertWinByBet(dicParams["tw"], currentBet);
            
            if(dicParams.ContainsKey("w"))
                dicParams["w"] = convertWinByBet(dicParams["w"], currentBet);
            
            if (dicParams.ContainsKey("rw"))
                dicParams["rw"] = convertWinByBet(dicParams["rw"], currentBet);
            
            if (dicParams.ContainsKey("fswin"))
                dicParams["fswin"] = convertWinByBet(dicParams["fswin"], currentBet);
            
            if (dicParams.ContainsKey("fsres"))
                dicParams["fsres"] = convertWinByBet(dicParams["fsres"], currentBet);
            
            if (dicParams.ContainsKey("fswin_total"))
                dicParams["fswin_total"] = convertWinByBet(dicParams["fswin_total"], currentBet);
            
            if (dicParams.ContainsKey("fsres_total"))
                dicParams["fsres_total"] = convertWinByBet(dicParams["fsres_total"], currentBet);

            if (dicParams.ContainsKey("fscwin_total"))
                dicParams["fscwin_total"] = convertWinByBet(dicParams["fscwin_total"], currentBet);

            if (dicParams.ContainsKey("fscres_total"))
                dicParams["fscres_total"] = convertWinByBet(dicParams["fscres_total"], currentBet);

            if (dicParams.ContainsKey("bpw"))
                dicParams["bpw"] = convertWinByBet(dicParams["bpw"], currentBet);

            if (dicParams.ContainsKey("e_aw"))
                dicParams["e_aw"] = convertWinByBet(dicParams["e_aw"], currentBet);

            if (dicParams.ContainsKey("fscres_total"))
                dicParams["fscres_total"] = convertWinByBet(dicParams["fscres_total"], currentBet);

            //스캐터정보
            if(dicParams.ContainsKey("psym"))
            {
                string[] strParts = dicParams["psym"].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                if(strParts.Length == 3)
                {
                    strParts[1] = convertWinByBet(strParts[1], currentBet);
                    dicParams["psym"] = string.Join("~", strParts);
                }
            }

            //WinLine정보
            int winLineID = 0;
            do
            {
                string strKey = string.Format("l{0}", winLineID);
                if (!dicParams.ContainsKey(strKey))
                    break;

                string[] strParts = dicParams[strKey].Split(new string[] { "~" }, StringSplitOptions.None);
                if(strParts.Length >= 2)
                {
                    strParts[1] = convertWinByBet(strParts[1], currentBet);
                    dicParams[strKey] = string.Join("~", strParts);
                }
                winLineID++;
            } while (true);
        }
        protected virtual BasePPSlotSpinResult calculateResult(string strGlobalUserID, BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst, ActionTypes action)
        {
            try
            {
                BasePPSlotSpinResult        spinResult  = new BasePPSlotSpinResult();
                Dictionary<string, string>  dicParams   = splitResponseToParams(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(dicParams, betInfo.TotalBet);

                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                if (SupportPurchaseFree && betInfo.PurchaseFree && isFirst)
                    dicParams["purtr"] = "1";

                spinResult.CurrentAction    = action;
                spinResult.NextAction       = convertStringToActionType(dicParams["na"]);
                spinResult.ResultString     = convertKeyValuesToString(dicParams);
                if (spinResult.NextAction == ActionTypes.DOCOLLECT || spinResult.NextAction == ActionTypes.DOCOLLECTBONUS)
                {
                    spinResult.TotalWin = double.Parse(dicParams["tw"]);
                }
                else
                {
                    spinResult.TotalWin = 0.0;
                }

                if (!_dicUserHistory.ContainsKey(strGlobalUserID))
                    spinResult.RoundID = ((long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
                else
                    spinResult.RoundID = _dicUserHistory[strGlobalUserID].roundid;

                return spinResult;
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::calculateResult {0}", ex);
                return null;
            }
        }
        protected Dictionary<string, string> splitResponseToParams(string strResponse)
        {
            string[] strParts = strResponse.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            if (strParts == null || strParts.Length == 0)
                return new Dictionary<string, string>();

            Dictionary<string, string> dicParamValues = new Dictionary<string, string>();
            for(int i = 0; i < strParts.Length; i++)
            {
                string[] strParamValues = strParts[i].Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (strParamValues.Length == 2)
                    dicParamValues[strParamValues[0]] = strParamValues[1];
                else if(strParamValues.Length == 1)
                    dicParamValues[strParamValues[0]] = null;
            }
            return dicParamValues;
        }
        protected string convertKeyValuesToString(Dictionary<string, string> keyValues)
        {
            List<string> parts = new List<string>();
            if(keyValues.ContainsKey("rid"))
            {
                if (keyValues["rid"] == null)
                    parts.Add(string.Format("{0}=", "rid"));
                else
                    parts.Add(string.Format("{0}={1}", "rid", keyValues["rid"]));
            }
            
            foreach (KeyValuePair<string, string> pair in keyValues)
            {
                if (pair.Key == "rid")
                    continue;

                if(pair.Value == null)
                    parts.Add(string.Format("{0}=", pair.Key));
                else
                    parts.Add(string.Format("{0}={1}", pair.Key, pair.Value));
            }
            return string.Join("&", parts.ToArray());
        }
        protected ActionTypes convertStringToActionType(string strAction)
        {
            switch(strAction.Trim())
            {
                case "s":
                    return ActionTypes.DOSPIN;
                case "c":
                    return ActionTypes.DOCOLLECT;
                case "cb":
                    return ActionTypes.DOCOLLECTBONUS;
                case "m":
                    return ActionTypes.DOMYSTERY;
                case "fso":
                    return ActionTypes.DOFSOPTION;
                case "b":
                    return ActionTypes.DOBONUS;
            }
            return ActionTypes.NONE;
        }
        protected string convertActionTypeToString(ActionTypes actionType)
        {
            switch (actionType)
            {
                case ActionTypes.DOSPIN:
                    return "s";
                case ActionTypes.DOCOLLECT:
                    return "c";
                case ActionTypes.DOCOLLECTBONUS:
                    return "cb";
                case ActionTypes.DOMYSTERY:
                    return "m";
                case ActionTypes.DOFSOPTION:
                    return "fso";
                case ActionTypes.DOBONUS:
                    return "b";
            }
            return "";
        }
        protected string convertActionTypeToFullString(ActionTypes actionType)
        {
            switch (actionType)
            {
                case ActionTypes.DOSPIN:
                    return "doSpin";
                case ActionTypes.DOCOLLECT:
                    return "doCollect";
                case ActionTypes.DOCOLLECTBONUS:
                    return "doCollectBonus";
                case ActionTypes.DOMYSTERY:
                    return "doMysteryScatter";
                case ActionTypes.DOFSOPTION:
                    return "doFSOption";
                case ActionTypes.DOBONUS:
                    return "doBonus";
            }
            return "";
        }
        protected List<BasePPActionToResponse> buildResponseList(List<string> responseList)
        {
            List<BasePPActionToResponse> actionResponseList = new List<BasePPActionToResponse>();
            for (int i = 1; i < responseList.Count; i++)
            {
                Dictionary<string, string> dicParamValues = splitResponseToParams(responseList[i - 1]);
                if (!dicParamValues.ContainsKey("na"))
                    continue;

                ActionTypes actionType = convertStringToActionType(dicParamValues["na"]);
                actionResponseList.Add(new BasePPActionToResponse(actionType, responseList[i]));
            }
            return actionResponseList;
        }
        protected List<BasePPActionToResponse> buildResponseList(List<string> responseList, ActionTypes action)
        {
            List<BasePPActionToResponse> actionResponseList = new List<BasePPActionToResponse>();
            for (int i = 0; i < responseList.Count; i++)
            {
                if (i >= 1)
                {
                    Dictionary<string, string> dicParamValues = splitResponseToParams(responseList[i - 1]);
                    if (!dicParamValues.ContainsKey("na"))
                        continue;

                    ActionTypes actionType = convertStringToActionType(dicParamValues["na"]);
                    actionResponseList.Add(new BasePPActionToResponse(actionType, responseList[i]));
                }
                else
                {
                    actionResponseList.Add(new BasePPActionToResponse(action, responseList[i]));
               
                }
            }
            return actionResponseList;
        }
        protected virtual double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            return this.PurchaseFreeMultiple;
        }
        protected virtual async Task<BasePPSlotSpinResult> generateSpinResult(BasePPSlotBetInfo betInfo, string strUserID, int agentID, UserBonus userBonus, bool usePayLimit, bool isMustLose)
        {
            BasePPSlotSpinData      spinData = null;
            BasePPSlotSpinResult    result   = null;

            ActionTypes action = ActionTypes.DOSPIN;

            string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
            if (betInfo.HasRemainResponse)
            {
                BasePPActionToResponse nextResponse = betInfo.pullRemainResponse();
                action = nextResponse.ActionType;
                result = calculateResult(strGlobalUserID, betInfo, nextResponse.Response, false, action);

                //프리게임이 끝났는지를 검사한다.
                if (!betInfo.HasRemainResponse)
                    betInfo.RemainReponses = null;
                return result;
            }

            //유저의 총 베팅액을 얻는다.
            float   totalBet        = betInfo.TotalBet;
            double  realBetMoney    = totalBet;

            if (SupportPurchaseFree && betInfo.PurchaseFree)
                realBetMoney = totalBet * getPurchaseMultiple(betInfo); //100.0
            
            if (SupportMoreBet && betInfo.MoreBet)
                realBetMoney = totalBet * MoreBetMultiple;

            spinData = await selectRandomStop(agentID, userBonus, totalBet, false, isMustLose, betInfo);

            //첫자료를 가지고 결과를 계산한다.
            double totalWin = totalBet * spinData.SpinOdd;

            if (!usePayLimit || spinData.IsEvent || checkCompanyPayoutRate(agentID, realBetMoney, totalWin))
            {
                do
                {
                    if (spinData.IsEvent)
                    {
                        bool checkRet = await subtractEventMoney(agentID, strUserID, totalWin);
                        if (!checkRet)
                            break;

                        _bonusSendMessage   = null;
                        _rewardedBonusMoney = totalWin;
                        _isRewardedBonus    = true;
                    }

                    result = calculateResult(strGlobalUserID, betInfo, spinData.SpinStrings[0], true, action);
                    if (spinData.SpinStrings.Count > 1)
                        betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
                    return result;
                }
                while (false);
            }

            double emptyWin = 0.0;
            if (SupportPurchaseFree && betInfo.PurchaseFree)
            {
                spinData    = await selectMinStartFreeSpinData(betInfo);
                result      = calculateResult(strGlobalUserID, betInfo, spinData.SpinStrings[0], true, action);
                emptyWin    = totalBet * spinData.SpinOdd;

                //뒤에 응답자료가 또 있다면
                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
            }
            else
            {
                spinData = await selectEmptySpin(agentID, betInfo);
                result   = calculateResult(strGlobalUserID, betInfo, spinData.SpinStrings[0], true, action);
            }
            sumUpCompanyBetWin(agentID, realBetMoney, emptyWin);
            return result;
        }

        protected byte[] backupBetInfo(BasePPSlotBetInfo betInfo)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    betInfo.SerializeTo(writer);
                }
                return ms.ToArray();
            }
        }
        protected byte[] backupHistoryInfo(string strGlobalUserID)
        {
            if (!_dicUserHistory.ContainsKey(strGlobalUserID))
                return null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    _dicUserHistory[strGlobalUserID].serializeTo(writer);
                }
                return ms.ToArray();
            }
        }
        protected virtual async Task spinGame(string strUserID, int agentID, UserBonus userBonus, bool isMustLose, double userBalance, int index, int counter)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                //해당 유저의 베팅정보를 얻는다. 만일 베팅정보가 없다면(례외상황) 그대로 리턴한다.
                BasePPSlotBetInfo betInfo = null;
                if (!_dicUserBetInfos.TryGetValue(strGlobalUserID, out betInfo))
                    return;

                byte[] betInfoBytes = backupBetInfo(betInfo);
                byte[] historyBytes = backupHistoryInfo(strGlobalUserID);

                BasePPSlotSpinResult lastResult = null;
                if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                    lastResult = _dicUserResultInfos[strGlobalUserID];

                double betMoney = betInfo.TotalBet;
                if (betInfo.HasRemainResponse)
                    betMoney = 0.0;

                UserBetTypes betType = UserBetTypes.Normal;
                //베팅머니의 100배로 프리스핀구입
                if (this.SupportPurchaseFree && betInfo.PurchaseFree)
                {
                    betMoney = Math.Round(betMoney * getPurchaseMultiple(betInfo), 2);
                    if (betMoney > 0)
                        betType = UserBetTypes.PurchaseFree;
                }
                else if(this.SupportMoreBet && betInfo.MoreBet)
                {
                    betMoney = Math.Round(betMoney * this.MoreBetMultiple, 2);
                    if (betMoney > 0)
                        betType = UserBetTypes.AnteBet;
                }

                //만일 베팅머니가 유저의 밸런스보다 크다면 끝낸다.(2020.02.15)
                if (userBalance.LT(betMoney, _epsilion) || betMoney < 0.0)
                {
                    _logger.Error("user balance is less than bet money in BasePPSlotGame::spinGame {0} balance:{1}, bet money: {2} game id:{3}",
                        strUserID, userBalance, betMoney, _gameID);
                    return;
                }

                //결과를 생성한다.
                BasePPSlotSpinResult spinResult = await this.generateSpinResult(betInfo, strUserID, agentID, userBonus, true, isMustLose);

                //게임로그
                string strGameLog                       = spinResult.ResultString;
                _dicUserResultInfos[strGlobalUserID]    = spinResult;

                //결과를 보내기전에 베팅정보를 디비에 보관한다.(2018.06.12)
                saveBetResultInfo(strGlobalUserID);

                //생성된 게임결과를 유저에게 보낸다.
                sendGameResult(betInfo, spinResult, strUserID, agentID, betMoney, spinResult.WinMoney, strGameLog, userBalance, index, counter, betType);

                _dicUserLastBackupBetInfos[strGlobalUserID]     = betInfoBytes;
                _dicUserLastBackupResultInfos[strGlobalUserID]  = lastResult;
                _dicUserLastBackupHistory[strGlobalUserID]      = historyBytes;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::spinGame {0}", ex);
            }
        }
        protected virtual void sendGameResult(BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult, string strUserID,int agentID ,double betMoney, double winMoney, string strGameLog, double userBalance, int index, int counter,UserBetTypes betType)
        {
            string strSpinResult = makeSpinResultString(betInfo, spinResult, betMoney, userBalance, index, counter, false);

            GITMessage message = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOSPIN);
            message.Append(strSpinResult);

            ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, message, betMoney, winMoney, new GameLogInfo(GameName, "0", strGameLog),betType);

            string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
            if (_isRewardedBonus)
            {
                toUserResult.setBonusReward(_rewardedBonusMoney);
                toUserResult.insertFirstMessage(_bonusSendMessage);
            }

            Sender.Tell(toUserResult, Self);
            if (_dicUserHistory.ContainsKey(strGlobalUserID))
            {
                if (_dicUserHistory[strGlobalUserID].log.Count == 0)
                    _dicUserHistory[strGlobalUserID].bet = betMoney;

                _dicUserHistory[strGlobalUserID].roundid = spinResult.RoundID;
            }

            //빈스핀인 경우에 히스토리보관을 여기서 진행한다.
            if (addSpinResultToHistory(strGlobalUserID, index, counter, strSpinResult, betInfo, spinResult))
                saveHistory(strUserID,agentID ,index, counter, userBalance - betMoney);
        }

        protected virtual void saveHistory(string strUserID,int agentID ,int index, int counter, double userBalance)
        {
            string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
            //히스토리보관 및 초기화
            if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0 && _dicUserHistory[strGlobalUserID].bet > 0.0)
            {
                //리플레이데이터를 디비에 보관
                if (SupportReplay)
                {
                    string strDetailLog = JsonConvert.SerializeObject(_dicUserHistory[strGlobalUserID]);
                    _dicUserHistory[strGlobalUserID].rtp = _dicUserHistory[strGlobalUserID].win / _dicUserHistory[strGlobalUserID].baseBet;
                    if (_dicUserHistory[strGlobalUserID].rtp > 1.0)
                    {
                        _dbWriter.Tell(new PPGameHistoryDBItem(agentID, strUserID, (int)_gameID, _dicUserHistory[strGlobalUserID].bet, _dicUserHistory[strGlobalUserID].baseBet, _dicUserHistory[strGlobalUserID].win, _dicUserHistory[strGlobalUserID].rtp,
                            _dicUserHistory[strGlobalUserID].roundid, strDetailLog, GameUtils.GetCurrentUnixTimestampMillis()));
                    }
                }

                //게임히스토리디베에 보관
                string strHistoryDetail = JsonConvert.SerializeObject(_dicUserHistory[strGlobalUserID].log);
                _dbWriter.Tell(new PPGameRecentHistoryDBItem(agentID, strUserID, (int)_gameID, userBalance, _dicUserHistory[strGlobalUserID].bet, _dicUserHistory[strGlobalUserID].win, "", 
                    _dicUserHistory[strGlobalUserID].roundid, strHistoryDetail, GameUtils.GetCurrentUnixTimestampMillis()));
            }
            _dicUserHistory.Remove(strGlobalUserID);
        }
        
        protected virtual bool addSpinResultToHistory(string strGlobalUserID, int index, int counter, string strSpinResult, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {
            if (!_dicUserHistory.ContainsKey(strGlobalUserID))
                return false;

            BasePPHistoryItem historyItem = new BasePPHistoryItem();
            historyItem.cr = string.Format("symbol={0}&c={1}&repeat=0&action=doSpin&index={2}&counter={3}&l={4}", SymbolName, betInfo.BetPerLine, index, counter, ClientReqLineCount);
            if (SupportPurchaseFree && betInfo.PurchaseFree)
                historyItem.cr += "&pur=0";
            if(SupportMoreBet)
            {
                if(betInfo.MoreBet)
                    historyItem.cr += "&bl=1";
                else
                    historyItem.cr += "&bl=0";
            }
            historyItem.sr                = strSpinResult;

            _dicUserHistory[strGlobalUserID].log.Add(historyItem);
            if (betInfo.HasRemainResponse)
                return false;

            _dicUserHistory[strGlobalUserID].baseBet    = betInfo.TotalBet;
            _dicUserHistory[strGlobalUserID].win        = spinResult.TotalWin;

            //빈스핀인 경우이다.
            if (spinResult.NextAction == ActionTypes.DOSPIN)
                return true;
            
            return false;
        }

        #region 스핀자료처리부분
        protected OddAndIDData selectOddAndIDFromProbsWithRange(SortedDictionary<double, int[]> oddProbs, int totalCount, double minOdd, double maxOdd)
        {
            int random  = Pcg.Default.Next(0, totalCount);
            int sum     = 0;
            foreach (KeyValuePair<double, int[]> pair in oddProbs)
            {
                if (pair.Key < minOdd)
                    continue;

                if (pair.Key > maxOdd)
                    break;

                sum += pair.Value.Length;
                if (random < sum)
                {
                    OddAndIDData ret = new OddAndIDData();
                    ret.ID  = pair.Value[Pcg.Default.Next(0, pair.Value.Length)];
                    ret.Odd = pair.Key;
                    return ret;
                }
            }
            return null;
        }
        protected OddAndIDData selectOddAndIDFromProbsWithRange(SortedDictionary<double, int[]> oddProbs, double minOdd, double maxOdd)
        {
            int totalCount = 0;
            foreach (KeyValuePair<double, int[]> pair in oddProbs)
            {
                if (pair.Key < minOdd)
                    continue;
                if (pair.Key > maxOdd)
                    break;
                totalCount += pair.Value.Length;
            }

            if (totalCount == 0)
                return null;

            int random = Pcg.Default.Next(0, totalCount);
            int sum = 0;
            foreach (KeyValuePair<double, int[]> pair in oddProbs)
            {
                if (pair.Key < minOdd)
                    continue;

                if (pair.Key > maxOdd)
                    break;

                sum += pair.Value.Length;
                if (random < sum)
                {
                    OddAndIDData ret = new OddAndIDData();
                    ret.ID = pair.Value[Pcg.Default.Next(0, pair.Value.Length)];
                    ret.Odd = pair.Key;
                    return ret;
                }
            }
            return null;
        }
        protected OddAndIDData selectOddAndIDFromProbs(SortedDictionary<double, int[]> oddProbs, int totalCount)
        {
            int random  = Pcg.Default.Next(0, totalCount);
            int sum     = 0;
            foreach (KeyValuePair<double, int[]> pair in oddProbs)
            {
                sum += pair.Value.Length;
                if (random < sum)
                {
                    OddAndIDData ret = new OddAndIDData();
                    ret.ID  = pair.Value[Pcg.Default.Next(0, pair.Value.Length)];
                    ret.Odd = pair.Key;
                    return ret;
                }
            }
            return null;
        }
        protected double selectOddFromProbs(SortedDictionary<double, int> oddProbs, int totalCount)
        {
            int random = Pcg.Default.Next(0, totalCount);
            int sum    = 0;
            foreach(KeyValuePair<double, int> pair in oddProbs)
            {
                sum += pair.Value;
                if (random < sum)
                    return pair.Key;
            }
            return oddProbs.First().Key;
        }
        
        protected virtual async Task<BasePPSlotSpinData> selectRandomStop(int agentID, BasePPSlotBetInfo betInfo, bool isMoreBet)
        {
            OddAndIDData selectedOddAndID = selectRandomOddAndID(agentID, betInfo, isMoreBet);
            return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(selectedOddAndID.ID), TimeSpan.FromSeconds(10.0));
        }
        protected virtual OddAndIDData selectRandomOddAndID(int agentID, BasePPSlotBetInfo betInfo, bool isMoreBet)
        {
            double payoutRate = getPayoutRate(agentID);
            
            double randomDouble = Pcg.Default.NextDouble(0.0, 100.0);

            double selectedOdd = 0.0;
            if (randomDouble >= payoutRate || payoutRate == 0.0)
            {
                selectedOdd = 0.0;
            }
            else if (SupportMoreBet && isMoreBet)
            {
                int random = Pcg.Default.Next(0, _naturalSpinCount - _anteBetMinusZeroCount);
                int sum = 0;
                selectedOdd = _naturalSpinOddProbs.Keys.First();
                foreach (KeyValuePair<double, int> pair in _naturalSpinOddProbs)
                {
                    if (pair.Key == 0.0)
                        sum += (pair.Value - _anteBetMinusZeroCount);
                    else
                        sum += pair.Value;
                    if (random < sum)
                    {
                        selectedOdd = pair.Key;
                        break;
                    }
                }
            }
            else
            {
                selectedOdd = selectOddFromProbs(_naturalSpinOddProbs, _naturalSpinCount);
            }

            if (!_totalSpinOddIds.ContainsKey(selectedOdd))
                return null;

            int selectedID = _totalSpinOddIds[selectedOdd][Pcg.Default.Next(0, _totalSpinOddIds[selectedOdd].Length)];
            OddAndIDData selectedOddAndID = new OddAndIDData();
            selectedOddAndID.ID     = selectedID;
            selectedOddAndID.Odd    = selectedOdd;
            return selectedOddAndID;
        }

        protected int pickNaturalIDFromTotalOddIds(double selectedOdd)
        {
            int maxCount = 0;
            for(int i = 0; i < _totalSpinOddIds[selectedOdd].Length; i++)
            {
                if (_totalSpinOddIds[selectedOdd][i] > _normalMaxID)
                    break;
                maxCount++;
            }
            return _totalSpinOddIds[selectedOdd][Pcg.Default.Next(0, maxCount)];
        }
        protected double getBestMatchOdd(double maxOdd)
        {
            maxOdd = Math.Round(maxOdd, 2);
            if (_totalSpinOddIds.ContainsKey(maxOdd))
                return maxOdd;

            double bestMatchedOdd = 0.0;    //제일 작은 오드값으로 초기화한다.
            foreach (KeyValuePair<double, int[]> pair in _totalSpinOddIds)
            {
                if (maxOdd < pair.Key)
                    break;
                bestMatchedOdd = pair.Key;
            }
            return bestMatchedOdd;
        }
        protected virtual async Task<BasePPSlotSpinData> selectEmptySpin(int agentID, BasePPSlotBetInfo betInfo)
        {
            int id = _emptySpinIDs[Pcg.Default.Next(0, _emptySpinIDs.Count)];
            return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(id), TimeSpan.FromSeconds(10.0));
        }

        protected virtual async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                OddAndIDData oddAndID = selectOddAndIDFromProbs(_totalFreeSpinOddIds, _freeSpinTotalCount);
                return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected virtual async Task<BasePPSlotSpinData> selectMinStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                OddAndIDData oddAndID  = selectOddAndIDFromProbsWithRange(_totalFreeSpinOddIds, _minFreeSpinTotalCount, PurchaseFreeMultiple * 0.2, PurchaseFreeMultiple * 0.5);
                return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        #endregion

        protected virtual async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int companyID, BasePPSlotBetInfo betInfo, double baseBet, UserBonus userBonus)
        {
            if(userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus rangeOddBonus = userBonus as UserRangeOddEventBonus;
                OddAndIDData oddAndID = selectOddAndIDFromProbsWithRange(_totalFreeSpinOddIds, rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd);
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

            double targetC = PurchaseFreeMultiple * payoutRate / 100.0;
            if (targetC >= _totalFreeSpinWinRate)
                targetC = _totalFreeSpinWinRate;

            if (targetC < _minFreeSpinWinRate)
                targetC = _minFreeSpinWinRate;

            double x = (_totalFreeSpinWinRate - targetC) / (_totalFreeSpinWinRate - _minFreeSpinWinRate);
            double y = 1.0 - x;

            BasePPSlotSpinData spinData = null;
            if (Pcg.Default.NextDouble(0.0, 1.0) <= x)
                spinData = await selectMinStartFreeSpinData(betInfo);
            else
                spinData = await selectRandomStartFreeSpinData(betInfo);
            return spinData;
        }
        public virtual async Task<BasePPSlotSpinData> selectRandomStop(int agentID, UserBonus userBonus, double baseBet, bool isChangedLineCount, bool isMustLose, BasePPSlotBetInfo betInfo)
        {
            //프리스핀구입을 먼저 처리한다.
            if(SupportPurchaseFree && betInfo.PurchaseFree)
                return await selectPurchaseFreeSpin(agentID, betInfo, baseBet, userBonus);

            if(userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus  rangeOddBonus = userBonus as UserRangeOddEventBonus;
                OddAndIDData oddAndID = selectOddAndIDFromProbsWithRange(_totalSpinOddIds, rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd);
                if(oddAndID != null)
                {
                    BasePPSlotSpinData spinDataEvent = await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
                    spinDataEvent.IsEvent = true;
                    return spinDataEvent;
                }
            }

            if (SupportMoreBet && betInfo.MoreBet)
                return await selectRandomStop(agentID, betInfo, true);
            else
                return await selectRandomStop(agentID, betInfo, false);
        }
        protected override async Task<bool> loadUserHistoricalData(int agentID, string strUserID, bool isNewEnter)
        {
            try
            {
                string strGlobalUserID  = string.Format("{0}_{1}", agentID, strUserID);
                string strKey           = string.Format("{0}_{1}", strGlobalUserID, _gameID);
                
                byte[] betInfoData = await RedisDatabase.RedisCache.StringGetAsync(strKey);
                if (betInfoData != null)
                {
                    using (var stream = new MemoryStream(betInfoData))
                    {
                        BinaryReader        reader  = new BinaryReader(stream);
                        BasePPSlotBetInfo   betInfo = restoreBetInfo(strGlobalUserID, reader);
                        if (betInfo != null)
                            _dicUserBetInfos[strGlobalUserID] = betInfo;
                    }
                }

                strKey = string.Format("{0}_{1}_result", strGlobalUserID, _gameID);
                byte[] resultInfoData = await RedisDatabase.RedisCache.StringGetAsync(strKey);
                if (resultInfoData != null)
                {
                    using (var stream = new MemoryStream(resultInfoData))
                    {
                        BinaryReader            reader      = new BinaryReader(stream);
                        BasePPSlotSpinResult    resultInfo  = restoreResultInfo(strGlobalUserID, reader);
                        if (resultInfo != null)
                            _dicUserResultInfos[strGlobalUserID] = resultInfo;
                    }
                }

                strKey = string.Format("{0}_{1}_history", strGlobalUserID, _gameID);
                byte[] historyInfoData = await RedisDatabase.RedisCache.StringGetAsync(strKey);
                if (historyInfoData != null)
                {
                    using (var stream = new MemoryStream(historyInfoData))
                    {
                        BinaryReader    reader      = new BinaryReader(stream);
                        BasePPHistory   userHistory = restoreHistory(reader);
                        if (userHistory != null)
                            _dicUserHistory[strGlobalUserID] = userHistory;
                    }
                }
                
                strKey = string.Format("{0}_{1}_setting", strGlobalUserID, _gameID);
                string strSetting = await RedisDatabase.RedisCache.StringGetAsync(strKey);
                if (strSetting != null)
                    _dicUserSettings[strGlobalUserID] = strSetting;
            }

            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::loadUserHistoricalData {0}", ex);
                return false;
            }
            return await base.loadUserHistoricalData(agentID, strUserID, isNewEnter);
        }
        protected virtual BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            BasePPSlotBetInfo betInfo = new BasePPSlotBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected BasePPHistory restoreHistory(BinaryReader reader)
        {
            try
            {
                BasePPHistory history = new BasePPHistory();
                history.serializeFrom(reader);
                return history;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::restoreHistory {0}", ex);
                return null;
            }
        }
        protected virtual BasePPSlotSpinResult restoreResultInfo(string strGlobalUserID, BinaryReader reader)
        {
            BasePPSlotSpinResult result = new BasePPSlotSpinResult();
            result.SerializeFrom(reader);
            return result;
        }
        protected virtual void saveBetResultInfo(string strGlobalUserID)
        {
            try
            {
                if(_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    byte[] betInfoBytes = _dicUserBetInfos[strGlobalUserID].convertToByte();
                    _redisWriter.Tell(new UserBetInfoWrite(strGlobalUserID, _gameID, betInfoBytes, false), Self);
                }
                if(_dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    byte[] resultInfoBytes = _dicUserResultInfos[strGlobalUserID].convertToByte();
                    _redisWriter.Tell(new UserResultInfoWrite(strGlobalUserID, _gameID, resultInfoBytes, false), Self);
                }
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::saveBetInfo {0}", ex);
            }
        }
        protected override async Task onUserExitGame(int agentID, string strUserID, bool userRequested)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                if (_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    byte[] betInfoBytes = _dicUserBetInfos[strGlobalUserID].convertToByte();
                    await _redisWriter.Ask(new UserBetInfoWrite(strGlobalUserID, _gameID, betInfoBytes, true));
                    _dicUserBetInfos.Remove(strGlobalUserID);
                }
                if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    byte[] resultInfoBytes = _dicUserResultInfos[strGlobalUserID].convertToByte();
                    await _redisWriter.Ask(new UserResultInfoWrite(strGlobalUserID, _gameID, resultInfoBytes, true));
                    _dicUserResultInfos.Remove(strGlobalUserID);
                }
                if (_dicUserHistory.ContainsKey(strGlobalUserID))
                {
                    byte[] historyByteData = _dicUserHistory[strGlobalUserID].convertToByte();
                    await _redisWriter.Ask(new UserHistoryWrite(strGlobalUserID, _gameID, historyByteData, true));
                    _dicUserHistory.Remove(strGlobalUserID);
                }
                else
                {
                    await _redisWriter.Ask(new UserHistoryWrite(strGlobalUserID, _gameID, null, true));
                }

                if (_dicUserSettings.ContainsKey(strGlobalUserID))
                {
                    string strSetting = _dicUserSettings[strGlobalUserID];
                    await _redisWriter.Ask(new UserSettingWrite(strGlobalUserID, _gameID, strSetting, true));
                    _dicUserSettings.Remove(strGlobalUserID);
                }
                _dicUserLastBackupResultInfos.Remove(strGlobalUserID);
                _dicUserLastBackupBetInfos.Remove(strGlobalUserID);
                _dicUserLastBackupHistory.Remove(strGlobalUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::onUserExitGame GameID:{0} {1}", _gameID, ex);
            }

            await base.onUserExitGame(agentID, strUserID, userRequested);
        }
    }

    public class OddAndIDData
    {
        public int      ID      { get; set; }
        public double   Odd     { get; set; }

        public OddAndIDData()
        {

        }
        public OddAndIDData(int id, double odd)
        {
            this.ID     = id;
            this.Odd    = odd;
        }
    }
}
