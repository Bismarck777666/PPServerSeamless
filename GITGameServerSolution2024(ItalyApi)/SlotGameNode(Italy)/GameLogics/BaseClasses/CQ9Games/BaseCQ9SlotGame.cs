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
    public class BaseCQ9SlotGame : IGameLogicActor
    {
        protected string                            _providerName           = "cq9";
        //스핀디비관리액터
        protected IActorRef                         _spinDatabase           = null;        
        protected double                            _spinDataDefaultBet     = 0.0f;

        protected int                               _normalMaxID            = 0;
        protected int                               _naturalSpinCount       = 0;
        protected SortedDictionary<double, int>     _naturalSpinOddProbs    = new SortedDictionary<double, int>();
        protected SortedDictionary<double, int[]>   _totalSpinOddIds        = new SortedDictionary<double, int[]>();
        protected List<int>                         _emptySpinIDs           = new List<int>();

        //프리스핀구매기능이 있을떄만 필요하다. 디비안의 모든(구매가능한) 프리스핀들의 오드별 아이디어레이
        protected SortedDictionary<double, int[]>   _totalFreeSpinOddIds    = new SortedDictionary<double, int[]>();
        protected int                               _freeSpinTotalCount     = 0;
        protected int                               _minFreeSpinTotalCount  = 0;
        protected double                            _totalFreeSpinWinRate   = 0.0; //스핀디비안의 모든 프리스핀들의 배당평균값
        protected double                            _minFreeSpinWinRate     = 0.0; //구매금액의 20% - 50%사이에 들어가는 모든 프리스핀들의 평균배당값

        //앤티베팅기능이 있을때만 필요하다.(앤티베팅시 감소시켜야할 빈스핀의 갯수)
        protected int                               _anteBetMinusZeroCount = 0;


        //매유저의 베팅정보 
        protected Dictionary<string, BaseCQ9SlotBetInfo>      _dicUserBetInfos                       = new Dictionary<string, BaseCQ9SlotBetInfo>();
        //유정의 마지막결과정보
        protected Dictionary<string, BaseCQ9SlotSpinResult>   _dicUserResultInfos                    = new Dictionary<string, BaseCQ9SlotSpinResult>();
        protected Dictionary<string, CQ9HistoryItem>          _dicUserHistory                        = new Dictionary<string, CQ9HistoryItem>();
        //백업정보
        protected Dictionary<string, BaseCQ9SlotSpinResult>  _dicUserLastBackupResultInfos           = new Dictionary<string, BaseCQ9SlotSpinResult>();
        protected Dictionary<string, byte[]>                 _dicUserLastBackupBetInfos              = new Dictionary<string, byte[]>();

        protected virtual int FreeSpinTypeCount
        {
            get
            {
                return 0; //유저가 선택가능한 프리스핀종류수
            }
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
        protected virtual string CQ9GameName
        {
            get
            {
                return "";
            }
        }
        protected virtual string CQ9GameNameSet
        {
            get
            {
                return "";
            }
        }
        protected virtual bool SupportMoreBet
        {
            get { return false; }
        }
        protected virtual double[] MoreBetMultiples
        {
            get
            {
                return new double[] { 1.0 };
            }
        }
        protected virtual int ClientReqMinBet
        {
            get
            {
                return 20;
            }
        }
        protected virtual int[] DenomDefine 
        { 
            get 
            {
                return new int[] { };
            } 
        }
        protected virtual int[] BetButton
        {
            get
            {
                return new int[] { };
            }
        }
        protected virtual int MaxBet
        {
            get
            {
                return 10000;
            }
        }
        protected virtual string InitReelSetString
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

        protected CQ9InitData _initData = new CQ9InitData();

        public BaseCQ9SlotGame()
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
                for (int i = 0; i < 100000; i++)
                {
                    BasePPSlotSpinData spinData = await selectRandomStop(0, 0);
                    sumOdd1 += spinData.SpinOdd;
                }
                stopWatch.Stop();
                long elapsed1 = stopWatch.ElapsedMilliseconds;

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

                _logger.Info("{0} Performance Test Results:  payoutrate: {4}, {1}, {2}, {3}", this.GameName, elapsed1, sumOdd1 / 100000, elapsed4, _config.PayoutRate);
                Sender.Tell(true);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseSelFreeCQ9SlotGame::onPerformanceTest {0}", ex);
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
        protected void sumUpCompanyBetWin(int agentID, double betMoney, double winMoney, int poolIndex = 0)
        {
            if (agentID == 0)
            {
                _totalBets[poolIndex] += betMoney;
                _totalWins[poolIndex] += winMoney;
            }
            else
            {
                if (!_agentTotalBets.ContainsKey(agentID))
                    _agentTotalBets[agentID] = new double[PoolCount];

                if (!_agentTotalWins.ContainsKey(agentID))
                    _agentTotalWins[agentID] = new double[PoolCount];

                _agentTotalBets[agentID][poolIndex] += betMoney;
                _agentTotalWins[agentID][poolIndex] += winMoney;
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
                    if (SupportPurchaseFree && spinBaseData.SpinType == 1)
                    {
                        freeSpinTotalCount++;
                        freeSpinTotalOdd += spinBaseData.Odd;
                        if (!freeSpinOddIds.ContainsKey(spinBaseData.Odd))
                            freeSpinOddIds.Add(spinBaseData.Odd, new List<int>());
                        freeSpinOddIds[spinBaseData.Odd].Add(spinBaseData.ID);

                        if (spinBaseData.Odd >= PurchaseFreeMultiple * 0.2 && spinBaseData.Odd <= PurchaseFreeMultiple * 0.5)
                        {
                            minFreeSpinTotalCount++;
                            minFreeSpinTotalOdd += spinBaseData.Odd;
                        }
                    }
                }
                
                _totalSpinOddIds = new SortedDictionary<double, int[]>();
                foreach (KeyValuePair<double, List<int>> pair in totalSpinOddIds)
                    _totalSpinOddIds.Add(pair.Key, pair.Value.ToArray());

                if (SupportPurchaseFree)
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

                if (SupportMoreBet)
                {
                    int naturalEmptyCount   = _naturalSpinOddProbs[0.0];
                    _anteBetMinusZeroCount  = (int)((1.0 - 1.0 / MoreBetMultiples[1]) * (double)_naturalSpinCount);

                    double moreBetWinRate   = 0.0;
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
            switch((CSMSG_CODE) message.MsgCode)
            {
                case CSMSG_CODE.CS_CQ9_InitGame1Request:
                    onDoInitUI(strGlobalUserID, (int)currency, message, userBonus, userBalance, isMustLose);
                    break;
                case CSMSG_CODE.CS_CQ9_InitGame2Request:
                    onDoInitReelSet(strGlobalUserID, message, userBonus, userBalance, isMustLose);
                    break;
                case CSMSG_CODE.CS_CQ9_NormalSpinRequest:
                case CSMSG_CODE.CS_CQ9_TembleSpinRequest:
                case CSMSG_CODE.CS_CQ9_FreeSpinStartRequest:
                case CSMSG_CODE.CS_CQ9_FreeSpinRequest:
                case CSMSG_CODE.CS_CQ9_FreeSpinSumRequest:
                case CSMSG_CODE.CS_CQ9_FreeSpinOptionRequest:
                case CSMSG_CODE.CS_CQ9_FreeSpinOptSelectRequest:
                case CSMSG_CODE.CS_CQ9_FreeSpinOptResultRequest:
                    await onDoSpin(strUserID, agentID, message, userBonus, userBalance, isMustLose);
                    break;
                case CSMSG_CODE.CS_CQ9_CollectRequest:
                    onCollectSpin(agentID,strUserID, (int)currency, userBalance);
                    break;
                case CSMSG_CODE.CS_PP_NOTPROCDRESULT:
                    onDoUndoUserSpin(strGlobalUserID);
                    break;
            }           
        }
        protected virtual void onDoInitUI(string strGlobalUserID, int currency, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            try
            {
                updateCQ9RecommendItem();
                string token                = (string)message.Pop();
                _initData.PlayerOrderURL    = string.Format("{0}?gametoken={1}", CQ9Config.Instance.HistoryURL, token);
                //_initData.PromotionData     = CQ9Config.Instance.Promotion;
                //_initData.RecommendList     = CQ9Config.Instance.RecommendList;
                _initData.PromotionData     = null;
                _initData.RecommendList     = null;

                CQ9InitData initData        = JsonConvert.DeserializeObject<CQ9InitData>(JsonConvert.SerializeObject(_initData));

                List<int> betButons = new List<int>();
                for(int i = 0; i < BetButton.Length; i++)
                {
                    betButons.Add(BetButton[i] * new Currencies()._currencyInfo[currency].Rate);
                }
                
                initData.BetButton          = betButons.ToArray();
                initData.MaxBet             = _initData.MaxBet * new Currencies()._currencyInfo[currency].Rate;
                initData.DenomDefine        = DenomDefine;
                initData.Currency           = new Currencies()._currencyInfo[currency].CurrencyText;
                switch (currency)
                {
                    case (int)CurrencyEnum.USD:
                        initData.DollarSignId = 2;
                        break;
                    case (int)CurrencyEnum.EUR:
                        initData.DollarSignId = 10;
                        break;
                    case (int)CurrencyEnum.TND:
                        initData.DollarSignId = 91;
                        break;
                    case (int)CurrencyEnum.KRW:
                        initData.DollarSignId = 4;
                        break;
                    case (int)CurrencyEnum.GMD:
                        initData.DollarSignId = 69;
                        break;
                    case (int)CurrencyEnum.CNY:
                        initData.DollarSignId = 1;
                        break;
                    case (int)CurrencyEnum.JPY:
                        initData.DollarSignId = 1;
                        break;
                    case (int)CurrencyEnum.MYR:
                        initData.DollarSignId = 8;
                        break;
                    case (int)CurrencyEnum.THB:
                        initData.DollarSignId = 7;
                        break;
                    case (int)CurrencyEnum.PHP:
                        initData.DollarSignId = 6;
                        break;
                    case (int)CurrencyEnum.VND:
                        initData.DollarSignId = 9;
                        break;
                    case (int)CurrencyEnum.INR:
                        initData.DollarSignId = 16;
                        break;
                    case (int)CurrencyEnum.IDR:
                        initData.DollarSignId = 12;
                        break;
                    case (int)CurrencyEnum.PKR:
                        initData.DollarSignId = 70;
                        break;
                    case (int)CurrencyEnum.BDT:
                        initData.DollarSignId = 65;
                        break;
                    case (int)CurrencyEnum.NPR:
                        initData.DollarSignId = 71;
                        break;
                    case (int)CurrencyEnum.UGX:
                        initData.DollarSignId = 93;
                        break;
                    case (int)CurrencyEnum.TRY:
                        initData.DollarSignId = 60;
                        break;
                    case (int)CurrencyEnum.RUB:
                        initData.DollarSignId = 23;
                        break;
                }
                string strResponse          = JsonConvert.SerializeObject(initData);

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_CQ9_InitGame1Response);
                responseMessage.Append(true);
                responseMessage.Append(strResponse);

                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseCQ9SlotGame::onDoInit GameID: {0}, {1}", _gameID, ex);
            }
        }
        private void updateCQ9RecommendItem()
        {
            List<CQ9GameDBItem> sortedGameList = CQ9Config.Instance.CQ9GameList.OrderByDescending(_ => _.MaxScore).ToList();
            CQ9Config.Instance.RecommendList = new CQ9RecommendItem();

            List<CQ9RecommendGameItem> recommendGameLists   = new List<CQ9RecommendGameItem>();
            List<CQ9HotGameItem> hotRankingGameLists               = new List<CQ9HotGameItem>();
            for (int i = 0; i < Math.Min(sortedGameList.Count, 10); i++)
            {
                recommendGameLists.Add(new CQ9RecommendGameItem(sortedGameList[i]));
            }
            for (int i = 0; i < Math.Min(sortedGameList.Count, 10); i++)
            {
                hotRankingGameLists.Add(new CQ9HotGameItem(sortedGameList[i]));
            }
            CQ9Config.Instance.RecommendList = new CQ9RecommendItem();
            CQ9Config.Instance.RecommendList.recommendGameList  = recommendGameLists.ToArray();
            CQ9Config.Instance.RecommendList.hotRankingGameList = hotRankingGameLists.ToArray();
        }
        protected virtual void onDoInitReelSet(string strGlobalUserID, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            try
            {
                string strResponse = InitReelSetString;

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_CQ9_InitGame2Response);
                responseMessage.Append(false);
                responseMessage.Append(strResponse);

                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseCQ9SlotGame::onDoInit GameID: {0}, {1}", _gameID, ex);
            }
        }        
        protected void onCollectSpin(int agentID, string strUserID, int currency, double balance)
        {
            CQ9EndSpinResponse response = new CQ9EndSpinResponse();
            response.Type       = 3;
            response.ID         = 132;
            response.Version    = 0;
            response.ErrorCode  = 0;
            response.IsAllowFreeHand = false;

            string strResponse = JsonConvert.SerializeObject(response);

            GITMessage message = new GITMessage((ushort)SCMSG_CODE.SC_CQ9_CollectResponse);
            message.Append(true);
            message.Append(strResponse);

            string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
            if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                BaseCQ9SlotSpinResult result = _dicUserResultInfos[strGlobalUserID];
                if(result.TotalWin > 0.0)
                {
                    ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, message, 0.0, result.TotalWin, new GameLogInfo(GameName, "0", "Collect"),UserBetTypes.Normal);
                    Sender.Tell(toUserResult);
                    saveHistory(agentID,strUserID, currency, balance, result.TotalWin);
                    result.TotalWin = 0.0;
                    return;
                }
            }
            Sender.Tell(new ToUserMessage((ushort)_gameID, message));
            saveHistory(agentID, strUserID, currency, balance, 0.0);
        }
        protected virtual async Task onDoSpin(string strUserID, int agentID, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            try
            {
                _isRewardedBonus        = false;
                _bonusSendMessage       = null;
                _rewardedBonusMoney     = 0.0;

                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                if (message.MsgCode == (ushort)CSMSG_CODE.CS_CQ9_NormalSpinRequest)
                    readBetInfoFromMessage(message, strGlobalUserID);

                await spinGame(strUserID, agentID, userBonus, isMustLose, userBalance);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseCQ9SlotGame::onDoSpin GameID: {0}, {1}", _gameID, ex);
            }
        }
        protected void onDoUndoUserSpin(string strGlobalUserID)
        {
            undoUserResultInfo(strGlobalUserID);
            undoUserHistory(strGlobalUserID);
            undoUserBetInfo(strGlobalUserID);
        }
        #endregion
        protected virtual void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                BaseCQ9SlotBetInfo betInfo   = new BaseCQ9SlotBetInfo();
                betInfo.PlayLine            = (int) message.Pop();
                betInfo.IsExtraBet          = (int) message.Pop();
                betInfo.PlayBet             = (int) message.Pop();
                betInfo.PlayDenom           = (int) message.Pop();
                betInfo.MiniBet             = (int) message.Pop();
                betInfo.ReelPay             = (double) message.Pop();
                betInfo.ReelSelected        = new List<int>();
                int reelSelectCnt           = (int) message.Pop();
                for(int i = 0; i < reelSelectCnt; i++)
                    betInfo.ReelSelected.Add((int) message.Pop());


                if (betInfo.PlayBet * betInfo.PlayLine * betInfo.MiniBet <= 0 || betInfo.PlayDenom <= 0)
                {
                    _logger.Error("{0} betInfo 0 or infinite in BaseCQ9SlotGame::readBetInfoFromMessage", strGlobalUserID);
                    return;
                }

                if (betInfo.MiniBet / this.MoreBetMultiples[betInfo.IsExtraBet] != this.ClientReqMinBet)
                {
                    _logger.Error("{0} betInfo.MiniBet is not matched {1}", strGlobalUserID, betInfo.MiniBet);
                    return;
                }

                BaseCQ9SlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strGlobalUserID, out oldBetInfo))
                {
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.PlayBet      = betInfo.PlayBet;
                    oldBetInfo.MiniBet      = betInfo.MiniBet;
                    oldBetInfo.PlayLine     = betInfo.PlayLine;
                    oldBetInfo.PlayDenom    = betInfo.PlayDenom;
                    oldBetInfo.ReelPay      = betInfo.ReelPay;
                    oldBetInfo.ReelSelected = betInfo.ReelSelected;
                    oldBetInfo.IsExtraBet   = betInfo.IsExtraBet;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseCQ9SlotGame::readBetInfoFromMessage {0}", ex);
            }
        }
        protected virtual void undoUserResultInfo(string strGlobalUserID)
        {
            try
            {
                if (!_dicUserLastBackupResultInfos.ContainsKey(strGlobalUserID))
                    return;

                BaseCQ9SlotSpinResult lastResult = _dicUserLastBackupResultInfos[strGlobalUserID];
                if (lastResult == null)
                    _dicUserResultInfos.Remove(strGlobalUserID);
                else
                    _dicUserResultInfos[strGlobalUserID] = lastResult;
                _dicUserLastBackupResultInfos.Remove(strGlobalUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseCQ9SlotGame::undoUserResultInfo {0}", ex);
            }
        }
        protected virtual void undoUserHistory(string strGlobalUserID)
        {

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
                        BaseCQ9SlotBetInfo betInfo   = restoreBetInfo(strGlobalUserID, binaryReader);
                        _dicUserBetInfos[strGlobalUserID] = betInfo;
                    }
                }
                _dicUserLastBackupBetInfos.Remove(strGlobalUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseCQ9SlotGame::undoUserBetInfo {0}", ex);
            }
        }
        protected double convertWinByBet(double win, BaseCQ9SlotBetInfo betInfo)
        {
            double currentBet = betInfo.TotalBet;
            currentBet = currentBet / MoreBetMultiples[betInfo.IsExtraBet];

            win = win / _spinDataDefaultBet * currentBet * 10000.0 / betInfo.PlayDenom;
            return Math.Round(win, 2);
        }
        protected virtual void convertWinsByBet(dynamic resultContext, BaseCQ9SlotBetInfo betInfo)
        {
            if (!object.ReferenceEquals(resultContext["BaseWin"], null))
                resultContext["BaseWin"] = convertWinByBet((double) resultContext["BaseWin"], betInfo);

            if (!object.ReferenceEquals(resultContext["TotalWin"], null))
                resultContext["TotalWin"] = convertWinByBet((double)resultContext["TotalWin"], betInfo);

            if (!object.ReferenceEquals(resultContext["TotalWinAmt"], null))
                resultContext["TotalWinAmt"] = convertWinByBet((double)resultContext["TotalWinAmt"], betInfo);

            if (!object.ReferenceEquals(resultContext["ScatterPayFromBaseGame"], null))
                resultContext["ScatterPayFromBaseGame"] = convertWinByBet((double)resultContext["ScatterPayFromBaseGame"], betInfo);

            if (!object.ReferenceEquals(resultContext["AccumlateJPAmt"], null))
                resultContext["AccumlateJPAmt"] = convertWinByBet((double)resultContext["AccumlateJPAmt"], betInfo);

            if (!object.ReferenceEquals(resultContext["AccumlateWinAmt"], null))
                resultContext["AccumlateWinAmt"] = convertWinByBet((double)resultContext["AccumlateWinAmt"], betInfo);

            if (!object.ReferenceEquals(resultContext["udsOutputWinLine"], null))
            {
                JArray lineWinsArray = resultContext["udsOutputWinLine"];
                for(int i = 0; i < lineWinsArray.Count; i++)
                {
                    if (object.ReferenceEquals(lineWinsArray[i]["LinePrize"], null))
                        continue;

                    lineWinsArray[i]["LinePrize"] = convertWinByBet((double)lineWinsArray[i]["LinePrize"], betInfo);
                }
                resultContext["udsOutputWinLine"] = lineWinsArray;
            }                
            
            if (!object.ReferenceEquals(resultContext["ReelPay"], null))
            {
                JArray reelPay = resultContext["ReelPay"];
                for(int i = 0; i < reelPay.Count; i++)
                {
                    if (Convert.ToDouble(reelPay[i]) != 1.0)
                        reelPay[i] = (long) convertWinByBet((double)resultContext["ReelPay"][i], betInfo);
                }
                resultContext["ReelPay"] = reelPay;
            }
            
            if (!object.ReferenceEquals(resultContext["ExtendFeatureByGame2"], null))
            {
                JArray ExtendFeatureByGame2 = resultContext["ExtendFeatureByGame2"];
                for(int i = 0; i < ExtendFeatureByGame2.Count; i++)
                {
                    if (!object.ReferenceEquals(ExtendFeatureByGame2[i]["Name"], null))
                    {

                        if (!object.ReferenceEquals(ExtendFeatureByGame2[i]["Value"], null))
                        {
                            string nameStr = ExtendFeatureByGame2[i]["Name"].ToString();
                            if(string.Equals(nameStr, "FeatureMinBet"))
                                ExtendFeatureByGame2[i]["Value"] = ((long)convertWinByBet((double)ExtendFeatureByGame2[i]["Value"], betInfo)).ToString();
                            if (string.Equals(nameStr, "TotalScatterWin"))
                                ExtendFeatureByGame2[i]["Value"] = ((long)convertWinByBet((double)ExtendFeatureByGame2[i]["Value"], betInfo)).ToString();
                        }
                        else
                        {
                            ExtendFeatureByGame2[i]["Value"] = null;
                        }
                    }
                }

                resultContext["ExtendFeatureByGame2"] = ExtendFeatureByGame2;
            }
        }
        protected CQ9Actions convertRespMsgCodeToAction(int id)
        {
            switch ((CQ9MessageCode) id)
            {
                case CQ9MessageCode.NormalSpinResponse:
                    return CQ9Actions.NormalSpin;
                case CQ9MessageCode.SpinEndResponse:
                    return CQ9Actions.EndSpin;
                case CQ9MessageCode.TembleSpinResponse:
                    return CQ9Actions.TembleSpin;
                case CQ9MessageCode.FreeSpinStartResponse:
                    return CQ9Actions.FreeSpinStart;
                case CQ9MessageCode.FreeSpinResponse:
                    return CQ9Actions.FreeSpin;
                case CQ9MessageCode.FreeSpinSumResponse:
                    return CQ9Actions.FreeSpinResult;
                case CQ9MessageCode.FreeSpinOptionStartResponse:
                    return CQ9Actions.FreeSpinOptionStart;
                case CQ9MessageCode.FreeSpinOptionSelectResponse:
                    return CQ9Actions.FreeSpinOptionSelect;
                case CQ9MessageCode.FreeSpinOptionResultResponse:
                    return CQ9Actions.FreeSpinOptionResult;
            }
            return CQ9Actions.None;
        }
        protected virtual void addDefaultParams(dynamic resultContext, CQ9Actions action)
        {
            resultContext["Type"]       = 3;
            resultContext["Version"]    = 0;
            resultContext["ErrorCode"]  = 0;
            
            if(action == CQ9Actions.NormalSpin)
            {
                resultContext["GamePlaySerialNumber"] = ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds / 10 + 455000000000);
                resultContext["EmulatorType"]         = 0;
            }
            else if(action == CQ9Actions.FreeSpin)
            {
                resultContext["EmulatorType"] = 0;
            }
        }

        protected virtual BaseCQ9SlotSpinResult calculateResult(BaseCQ9SlotBetInfo betInfo, string strSpinResponse, bool isFirst)
        {
            try
            {
                BaseCQ9SlotSpinResult        spinResult  = new BaseCQ9SlotSpinResult();
                dynamic resultContext                    = JsonConvert.DeserializeObject<dynamic>(strSpinResponse);
                convertWinsByBet(resultContext, betInfo);

                CQ9Actions currentAction = convertRespMsgCodeToAction((int) resultContext["ID"]);
                if(currentAction == CQ9Actions.NormalSpin)
                {
                    int nextModule = (int)resultContext["NextModule"];
                    if(nextModule == 0)
                        spinResult.TotalWin = Math.Round((double)resultContext["TotalWin"] * betInfo.PlayDenom / 10000.0, 2);
                }
                else if(currentAction == CQ9Actions.TembleSpin)
                {
                    int nextModule = (int)resultContext["NextModule"];
                    if (nextModule == 0)
                    {
                        spinResult.TotalWin = Math.Round((double)resultContext["TotalWin"] * betInfo.PlayDenom / 10000.0, 2);
                    }
                }
                else if(currentAction == CQ9Actions.FreeSpinResult)
                {
                    spinResult.TotalWin = Math.Round((double)resultContext["TotalWinAmt"] * betInfo.PlayDenom / 10000.0, 2);
                }
                spinResult.MessageID    = (int)resultContext["ID"];
                addDefaultParams(resultContext, currentAction);
                spinResult.ResultString = JsonConvert.SerializeObject(resultContext);
                spinResult.Action       = currentAction;
                return spinResult;
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseCQ9SlotGame::calculateResult {0}", ex);
                return null;
            }
        }
        protected List<BaseCQ9ActionToResponse> buildResponseList(List<string> responseList)
        {
            List<BaseCQ9ActionToResponse> actionResponseList = new List<BaseCQ9ActionToResponse>();
            for (int i = 1; i < responseList.Count; i++)
            {
                dynamic    resultContext = JsonConvert.DeserializeObject<dynamic>(responseList[i]);
                CQ9Actions action = convertRespMsgCodeToAction((int)resultContext["ID"]);
                actionResponseList.Add(new BaseCQ9ActionToResponse(action, responseList[i]));

                if (action == CQ9Actions.FreeSpinOptionStart)
                    actionResponseList.Add(new BaseCQ9ActionToResponse(CQ9Actions.FreeSpinOptionSelect, ""));
            }
            return actionResponseList;
        }
        protected virtual async Task<BaseCQ9SlotSpinResult> generateSpinResult(BaseCQ9SlotBetInfo betInfo, string strUserID, int agentID, UserBonus userBonus, bool usePayLimit, bool isMustLose)
        {
            BasePPSlotSpinData      spinData = null;
            BaseCQ9SlotSpinResult    result   = null;

            string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
            if (betInfo.HasRemainResponse)
            {
                BaseCQ9ActionToResponse nextResponse = betInfo.pullRemainResponse();
                result = calculateResult(betInfo, nextResponse.Response, false);

                //텀블스핀인경우에 윈값은 루적된다
                dynamic resultContext       = JsonConvert.DeserializeObject<dynamic>(result.ResultString);
                CQ9Actions currentAction    = convertRespMsgCodeToAction((int)resultContext["ID"]);
                if(currentAction == CQ9Actions.TembleSpin)
                    result.TotalWin += _dicUserResultInfos[strGlobalUserID].TotalWin;
                
                //프리게임이 끝났는지를 검사한다.
                if (!betInfo.HasRemainResponse)
                    betInfo.RemainReponses = null;
                return result;
            }

            //유저의 총 베팅액을 얻는다.
            double  totalBet        = betInfo.TotalBet;
            double  realBetMoney    = totalBet;

            if (SupportPurchaseFree && betInfo.ReelPay > 0)
                realBetMoney = betInfo.ReelPay; //100.0

            spinData = await selectRandomStop(agentID, userBonus, totalBet, false, isMustLose, betInfo);

            //첫자료를 가지고 결과를 계산한다.
            double totalWin = totalBet * spinData.SpinOdd;

            if (SupportMoreBet)
                totalWin = totalBet * spinData.SpinOdd / MoreBetMultiples[betInfo.IsExtraBet];

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

                    result = calculateResult(betInfo, spinData.SpinStrings[0], true);
                    if (spinData.SpinStrings.Count > 1)
                        betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
                    return result;
                }
                while (false);
            }

            double emptyWin = 0.0;

            if (SupportPurchaseFree && betInfo.ReelPay > 0)
            {
                spinData    = await selectMinStartFreeSpinData(betInfo);
                result      = calculateResult(betInfo, spinData.SpinStrings[0], true);
                emptyWin    = totalBet * spinData.SpinOdd;

                //뒤에 응답자료가 또 있다면
                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
            }
            else
            {
                spinData    = await selectEmptySpin(betInfo);
                result      = calculateResult(betInfo, spinData.SpinStrings[0], true);
            }

            sumUpCompanyBetWin(agentID, realBetMoney, emptyWin);
            return result;
        }
        protected byte[] backupBetInfo(BaseCQ9SlotBetInfo betInfo)
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
        protected virtual async Task spinGame(string strUserID, int agentID, UserBonus userBonus, bool isMustLose, double userBalance)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                //해당 유저의 베팅정보를 얻는다. 만일 베팅정보가 없다면(례외상황) 그대로 리턴한다.
                BaseCQ9SlotBetInfo betInfo = null;
                if (!_dicUserBetInfos.TryGetValue(strGlobalUserID, out betInfo))
                    return;

                byte[] betInfoBytes = backupBetInfo(betInfo);

                BaseCQ9SlotSpinResult lastResult = null;
                if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                    lastResult = _dicUserResultInfos[strGlobalUserID];

                double betMoney = betInfo.TotalBet;
                //프리스핀구입,또는 리스핀릴인경우 
                if (betInfo.ReelPay > 0)
                    betMoney = Math.Round(betInfo.ReelPay * betInfo.PlayDenom / 10000, 1);

                if (betInfo.HasRemainResponse)
                    betMoney = 0.0;

                //만일 베팅머니가 유저의 밸런스보다 크다면 끝낸다.(2020.02.15)
                if (userBalance.LT(betMoney, _epsilion) || betMoney < 0.0)
                {
                    _logger.Error("user balance is less than bet money in BaseCQ9SlotGame::spinGame {0} balance:{1}, bet money: {2} game id:{3}",
                        strGlobalUserID, userBalance, betMoney, _gameID);
                    return;
                }

                if(betMoney > 0.0)
                    _dicUserHistory[strGlobalUserID] = new CQ9HistoryItem();

                //결과를 생성한다.
                BaseCQ9SlotSpinResult spinResult = await this.generateSpinResult(betInfo, strUserID, agentID, userBonus, true, isMustLose);

                //게임로그
                string strGameLog                       = spinResult.ResultString;
                _dicUserResultInfos[strGlobalUserID]    = spinResult;

                //생성된 게임결과를 유저에게 보낸다.(윈은 Collect요청시에 처리한다.)
                sendGameResult(betInfo, spinResult, strGlobalUserID, betMoney, 0.0, strGameLog, userBalance);

                _dicUserLastBackupBetInfos[strGlobalUserID]     = betInfoBytes;
                _dicUserLastBackupResultInfos[strGlobalUserID]  = lastResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseCQ9SlotGame::spinGame {0}", ex);
            }
        }
        protected virtual void sendGameResult(BaseCQ9SlotBetInfo betInfo, BaseCQ9SlotSpinResult spinResult, string strGlobalUserID, double betMoney, double winMoney, string strGameLog, double userBalance)
        {
            GITMessage message = new GITMessage((ushort) (spinResult.MessageID + 3000));
            message.Append(true);
            message.Append(spinResult.ResultString);

            ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, message, betMoney, winMoney, new GameLogInfo(GameName, "0", strGameLog),UserBetTypes.Normal);

            if (_isRewardedBonus)
            {
                toUserResult.setBonusReward(_rewardedBonusMoney);
                toUserResult.insertFirstMessage(_bonusSendMessage);
            }
            
            if (_dicUserHistory.ContainsKey(strGlobalUserID))
                _dicUserHistory[strGlobalUserID].Responses.Add(new CQ9ResponseHistory(spinResult.Action, DateTime.UtcNow, changeResultStringForHistory(spinResult.ResultString , betInfo))); 
            Sender.Tell(toUserResult, Self);
        }
        protected virtual string changeResultStringForHistory(string resultString, BaseCQ9SlotBetInfo betInfo)
        {
            return resultString;
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
                    ret.ID = pair.Value[Pcg.Default.Next(0, pair.Value.Length)];
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
        protected virtual async Task<BasePPSlotSpinData> selectRandomStop(int agentID, int isExtraBet)
        {
            OddAndIDData selectedOddAndID = selectRandomOddAndID(agentID, isExtraBet);
            return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(selectedOddAndID.ID), TimeSpan.FromSeconds(10.0));
        }
        protected virtual OddAndIDData selectRandomOddAndID(int agentID, int isExtraBet)
        {
            double payoutRate = getPayoutRate(agentID);

            double randomDouble = Pcg.Default.NextDouble(0.0, 100.0);

            double selectedOdd = 0.0;
            if (randomDouble >= payoutRate || payoutRate == 0.0)
                selectedOdd = 0.0;
            else
                selectedOdd = selectOddFromProbs(_naturalSpinOddProbs, _naturalSpinCount);

            if (!_totalSpinOddIds.ContainsKey(selectedOdd))
                return null;
            int selectedID = _totalSpinOddIds[selectedOdd][Pcg.Default.Next(0, _totalSpinOddIds[selectedOdd].Length)];
            OddAndIDData selectedOddAndID = new OddAndIDData();
            selectedOddAndID.ID     = selectedID;
            selectedOddAndID.Odd    = selectedOdd;
            return selectedOddAndID;
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
        protected virtual async Task<BasePPSlotSpinData> selectEmptySpin(BaseCQ9SlotBetInfo betInfo)
        {
            int id = _emptySpinIDs[Pcg.Default.Next(0, _emptySpinIDs.Count)];
            return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(id), TimeSpan.FromSeconds(10.0));
        }
        protected virtual async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BaseCQ9SlotBetInfo betInfo)
        {
            try
            {
                OddAndIDData oddAndID = selectOddAndIDFromProbs(_totalFreeSpinOddIds, _freeSpinTotalCount);
                return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseCQ9SlotGame::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected virtual async Task<BasePPSlotSpinData> selectMinStartFreeSpinData(BaseCQ9SlotBetInfo betInfo)
        {
            try
            {
                OddAndIDData oddAndID = selectOddAndIDFromProbsWithRange(_totalFreeSpinOddIds, _minFreeSpinTotalCount, PurchaseFreeMultiple * 0.2, PurchaseFreeMultiple * 0.5);
                return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        #endregion
        protected virtual async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int agentID, BaseCQ9SlotBetInfo betInfo, double baseBet, UserBonus userBonus)
        {
            if (userBonus != null && userBonus is UserRangeOddEventBonus)
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

            double payoutRate = getPayoutRate(agentID);

            double targetC = PurchaseFreeMultiple * payoutRate / 100.0;
            if (targetC >= _totalFreeSpinWinRate)
                targetC = _totalFreeSpinWinRate;

            if (targetC < _minFreeSpinWinRate)
                targetC = _minFreeSpinWinRate;

            double x = (_totalFreeSpinWinRate - targetC) / (_totalFreeSpinWinRate - _minFreeSpinWinRate);

            BasePPSlotSpinData spinData = null;
            if (Pcg.Default.NextDouble(0.0, 1.0) <= x)
                spinData = await selectMinStartFreeSpinData(betInfo);
            else
                spinData = await selectRandomStartFreeSpinData(betInfo);
            return spinData;
        }
        public virtual async Task<BasePPSlotSpinData> selectRandomStop(int agentID, UserBonus userBonus, double baseBet, bool isChangedLineCount, bool isMustLose, BaseCQ9SlotBetInfo betInfo)
        {
            //프리스핀구입을 먼저 처리한다.
            if (SupportPurchaseFree && betInfo.ReelPay > 0.0)
                return await selectPurchaseFreeSpin(agentID, betInfo, baseBet, userBonus);

            if(userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus  rangeOddBonus = userBonus as UserRangeOddEventBonus;
                OddAndIDData oddAndID = selectOddAndIDFromProbsWithRange(_totalSpinOddIds, rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd);
                if (oddAndID != null)
                {
                    BasePPSlotSpinData spinDataEvent = await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
                    spinDataEvent.IsEvent = true;
                    return spinDataEvent;
                }
            }

            return await selectRandomStop(agentID, betInfo.IsExtraBet);
        }
        protected virtual void saveHistory(int agentID, string strUserID, int currency, double userBalance, double winMoney)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                if (!_dicUserHistory.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID))
                    return;

                CQ9GameLogItem gameLogItem = _dicUserHistory[strGlobalUserID].buildGameLogItem(agentID, strUserID, new Currencies()._currencyInfo[currency].CurrencyText, (int)_gameID, _dicUserBetInfos[strGlobalUserID],
                    userBalance, winMoney, this.SymbolName, CQ9GameName, JsonConvert.DeserializeObject<List<CQ9GameName>>(CQ9GameNameSet));

                updateCQ9MiniLobbyInfo(gameLogItem);
                _dbWriter.Tell(gameLogItem);
                _dicUserHistory.Remove(strGlobalUserID);
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseCQ9SlotGame::saveHistory {0}", ex);
            }
        }
        private void updateCQ9MiniLobbyInfo(CQ9GameLogItem logItem)
        {
            int index = CQ9Config.Instance.CQ9GameList.FindIndex(_ => _.Symbol == this.SymbolName);

            if(index > -1)
            {
                if (CQ9Config.Instance.CQ9GameList[index].MaxScore < logItem.Win)
                    CQ9Config.Instance.CQ9GameList[index].MaxScore = logItem.Win;
                if (logItem.Bet > 0.0f && CQ9Config.Instance.CQ9GameList[index].MaxMultiple < logItem.Win / logItem.Bet)
                    CQ9Config.Instance.CQ9GameList[index].MaxMultiple = logItem.Win / logItem.Bet;
            }
        }
        protected override async Task<bool> loadUserHistoricalData(int agentID, string strUserID, bool isNewEnter)
        {
            try
            {
                if(!isNewEnter)
                {
                    string strGlobalUserID  = string.Format("{0}_{1}", agentID, strUserID);
                    string strKey           = string.Format("{0}_{1}", strGlobalUserID, _gameID);
                    byte[] betInfoData = await RedisDatabase.RedisCache.StringGetAsync(strKey);
                    if (betInfoData != null)
                    {
                        using (var stream = new MemoryStream(betInfoData))
                        {
                            BinaryReader reader = new BinaryReader(stream);
                            BaseCQ9SlotBetInfo betInfo = restoreBetInfo(strGlobalUserID, reader);
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
                            BinaryReader reader = new BinaryReader(stream);
                            BaseCQ9SlotSpinResult resultInfo = restoreResultInfo(strGlobalUserID, reader);
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
                            BinaryReader reader = new BinaryReader(stream);
                            CQ9HistoryItem userHistory = restoreHistory(reader);
                            if (userHistory != null)
                                _dicUserHistory[strGlobalUserID] = userHistory;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseCQ9SlotGame::loadUserHistoricalData {0}", ex);
                return false;
            }
            return await base.loadUserHistoricalData(agentID, strUserID, isNewEnter);
        }
        protected virtual BaseCQ9SlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            BaseCQ9SlotBetInfo betInfo = new BaseCQ9SlotBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected virtual BaseCQ9SlotSpinResult restoreResultInfo(string strGlobalUserID, BinaryReader reader)
        {
            BaseCQ9SlotSpinResult result = new BaseCQ9SlotSpinResult();
            result.SerializeFrom(reader);
            return result;
        }
        protected virtual CQ9HistoryItem restoreHistory(BinaryReader reader)
        {
            CQ9HistoryItem history = new CQ9HistoryItem();
            history.SerializeFrom(reader);
            return history;
        }
        protected virtual async Task<double> procRemainResponse(string strUserID, int agentID, BaseCQ9SlotBetInfo betInfo, double userBalance)
        {
            double winMoney = 0.0;
            do
            {
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                BaseCQ9ActionToResponse actionToResponse = betInfo.pullRemainResponse();
                BaseCQ9SlotSpinResult result = calculateResult(betInfo, actionToResponse.Response, false);

                if (_dicUserHistory.ContainsKey(strGlobalUserID))
                    _dicUserHistory[strGlobalUserID].Responses.Add(new CQ9ResponseHistory(result.Action, DateTime.UtcNow, result.ResultString));

                //프리게임이 끝났는지를 검사한다.
                if (!betInfo.HasRemainResponse)
                {
                    betInfo.RemainReponses = null;
                    if (result.TotalWin > 0.0)
                        winMoney = result.TotalWin;
                    break;
                }
            } while (true);

            return winMoney;
        }
        protected override async Task onExitUserMessage(ExitGameRequest message)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", message.AgentID, message.UserID);
                double winMoney = await procUserExitGame(message.UserID,message.Currency, message.Balance, message.AgentID, message.UserRequested, message.IsNewServerReady);
                _dicEnteredUsers.Remove(strGlobalUserID);
                if (winMoney == 0.0)
                {
                    Sender.Tell(new CQ9ExitGameResponse(null));
                    return;
                }

                ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, null, 0.0, winMoney, new GameLogInfo(GameName, "0", "Collect"),UserBetTypes.Normal);
                Sender.Tell(new CQ9ExitGameResponse(toUserResult));
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseCQ9SlotGame::onExitUserMessage {0}", ex);
            }
        }
        protected async Task<double> procUserExitGame(string strUserID, int currency, double userBalance, int agentID, bool userRequested, bool isNewServerReady)
        {
            try
            {
                string strGlobalUserID  = string.Format("{0}_{1}", agentID, strUserID);
                double winMoney         = 0.0;
                if(userRequested || !isNewServerReady)
                {
                    if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                    {
                        BaseCQ9SlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
                        if (betInfo.HasRemainResponse)
                            winMoney = await procRemainResponse(strUserID, agentID, betInfo, userBalance);
                    }
                    else if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                    {
                        BaseCQ9SlotSpinResult result = _dicUserResultInfos[strGlobalUserID];
                        winMoney = result.TotalWin;
                    }

                    if (_dicUserHistory.ContainsKey(strGlobalUserID))
                    {
                        saveHistory(agentID, strUserID, currency, userBalance, winMoney);
                        _dicUserHistory.Remove(strGlobalUserID);
                    }

                    _dicUserBetInfos.Remove(strGlobalUserID);
                    _dicUserResultInfos.Remove(strGlobalUserID);
                    await _redisWriter.Ask(new UserBetInfoWrite(strGlobalUserID, _gameID, null, true));
                    await _redisWriter.Ask(new UserResultInfoWrite(strGlobalUserID, _gameID, null, true));
                    await _redisWriter.Ask(new UserHistoryWrite(strGlobalUserID, _gameID, null, true));
                }
                else
                {
                    if (_dicUserBetInfos.ContainsKey(strGlobalUserID))
                    {
                        byte[] betInfoBytes = _dicUserBetInfos[strGlobalUserID].convertToByte();
                        await _redisWriter.Ask(new UserBetInfoWrite(strGlobalUserID, _gameID, betInfoBytes, true));
                        _dicUserBetInfos.Remove(strGlobalUserID);
                    }
                    else
                    {
                        await _redisWriter.Ask(new UserBetInfoWrite(strGlobalUserID, _gameID, null, true));
                    }

                    if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                    {
                        byte[] resultInfoBytes = _dicUserResultInfos[strGlobalUserID].convertToByte();
                        await _redisWriter.Ask(new UserResultInfoWrite(strGlobalUserID, _gameID, resultInfoBytes, true));
                        _dicUserResultInfos.Remove(strGlobalUserID);
                    }
                    else
                    {
                        await _redisWriter.Ask(new UserResultInfoWrite(strGlobalUserID, _gameID, null, true));
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
                }

                _dicUserLastBackupResultInfos.Remove(strGlobalUserID);
                _dicUserLastBackupBetInfos.Remove(strGlobalUserID);
                return winMoney;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseCQ9SlotGame::onUserExitGame GameID:{0} {1}", _gameID, ex);
                return 0.0;
            }
        }
    }

    public class CQ9EndSpinResponse
    {
        public int  Type            { get; set; }
        public int  ID              { get; set; }
        public int  Version         { get; set; }
        public int  ErrorCode       { get; set; }
        public bool IsAllowFreeHand { get; set; }
    }
}
