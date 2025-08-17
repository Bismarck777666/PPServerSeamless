using Akka.Actor;
using GITProtocol;
using GITProtocol.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCGSharp;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    public class BaseHabaneroSlotGame : IGameLogicActor
    {
        protected string    _providerName           = "habanero";
        //스핀디비관리액터
        protected IActorRef _spinDatabase           = null;
        protected double    _spinDataDefaultBet     = 0.0f;

        protected int       _normalMaxID            = 0;
        protected int       _naturalSpinCount       = 0;
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
        protected int                               _anteBetMinusZeroCount  = 0;


        //유저의 베팅정보 
        protected Dictionary<string, BaseHabaneroSlotBetInfo>       _dicUserBetInfos    = new Dictionary<string, BaseHabaneroSlotBetInfo>();

        //유정의 마지막결과정보
        protected Dictionary<string, BaseHabaneroSlotSpinResult>    _dicUserResultInfos = new Dictionary<string, BaseHabaneroSlotSpinResult>();

        //유저의 게임이력정보
        protected Dictionary<string, HabaneroHistoryItem>           _dicUserHistory     = new Dictionary<string, HabaneroHistoryItem>();

        //백업정보
        protected Dictionary<string, BaseHabaneroSlotSpinResult>    _dicUserLastBackupResultInfos   = new Dictionary<string, BaseHabaneroSlotSpinResult>();
        protected Dictionary<string, byte[]>                        _dicUserLastBackupBetInfos      = new Dictionary<string, byte[]>();

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
        protected virtual string BrandGameId
        {
            get
            {
                return "";
            }
        }
        protected virtual string GameHash
        {
            get
            {
                return "";
            }
        }
        protected virtual string JPHash
        {
            get
            {
                return "4ccbcc6c2e2607c4c0b3bf17ce5eabe17a98ec01";
            }
        }
        protected virtual string RngHash
        {
            get
            {
                return "eead3c403b3721bb62da82546e60c5fb52174559";
            }
        }
        protected virtual string GameVersion
        {
            get
            {
                return "";
            }
        }
        protected virtual string JPVersion
        {
            get
            {
                return "5.1.4326.325";
            }
        }
        protected virtual string RngVersion
        {
            get
            {
                return "5.1.4478.308";
            }
        }
        protected virtual int[] CoinsIncrement
        {
            get
            {
                return new int[] { 1, 2, 5, 7, 10 };
            }
        }
        protected virtual double[] StakeIncrement
        {
            get
            {
                return new double[] { 0.01, 0.02, 0.05, 0.10, 0.20, 0.50, 1, 2, 5 };
            }
        }
        protected virtual float MiniCoin
        {
            get
            {
                return 10.0f;
            }
        }
        protected virtual double MaxPayLimit
        {
            get
            {
                return 40000.0;
            }
        }
        protected virtual Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>();
            }
        }
        protected virtual int InitReelStatusNo
        {
            get
            {
                return 111;
            }
        }
        protected virtual int ClientReqLineCount
        {
            get
            {
                return 20;
            }
        }
        protected virtual string BetType
        {
            get
            {
                return "Line";
            }
        }
        protected virtual bool SupportPurchaseFree
        {
            get { return false; }
        }
        protected virtual double[] PurchaseFreeMultiple
        {
            get { return new double[] { 0.0f }; }
        }
        protected virtual bool SupportMoreBet
        {
            get { return false; }
        }
        protected virtual double MoreBetMultiple
        {
            get { return 0.0; }
        }
        public BaseHabaneroSlotGame()
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
                    BasePPSlotSpinData spinData = await selectRandomStop(0,new BaseHabaneroSlotBetInfo(this.MiniCoin) ,false);
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
                        if (oddAndID != null)
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

                double agentPayoutRate = getPayoutRate(agentID);

                double totalBet = _agentTotalBets[agentID][poolIndex] + betMoney;
                double totalWin = _agentTotalWins[agentID][poolIndex] + winMoney;

                double maxTotalWin = totalBet * agentPayoutRate / 100.0 + _config.PoolRedundency;
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

                Dictionary<double, List<int>> totalSpinOddIds   = new Dictionary<double, List<int>>();
                Dictionary<double, List<int>> freeSpinOddIds    = new Dictionary<double, List<int>>();

                double freeSpinTotalOdd     = 0.0;
                double minFreeSpinTotalOdd  = 0.0;
                int freeSpinTotalCount      = 0;
                int minFreeSpinTotalCount   = 0;
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
                    if (SupportPurchaseFree && !HasPurEnableOption && spinBaseData.SpinType == 1)
                    {
                        freeSpinTotalCount++;
                        freeSpinTotalOdd += spinBaseData.Odd;
                        if (!freeSpinOddIds.ContainsKey(spinBaseData.Odd))
                            freeSpinOddIds.Add(spinBaseData.Odd, new List<int>());
                        freeSpinOddIds[spinBaseData.Odd].Add(spinBaseData.ID);

                        if (spinBaseData.Odd >= PurchaseFreeMultiple[0] * 0.2 && spinBaseData.Odd <= PurchaseFreeMultiple[0] * 0.5)
                        {
                            minFreeSpinTotalCount++;
                            minFreeSpinTotalOdd += spinBaseData.Odd;
                        }
                    }
                }
                
                _totalSpinOddIds = new SortedDictionary<double, int[]>();
                foreach (KeyValuePair<double, List<int>> pair in totalSpinOddIds)
                    _totalSpinOddIds.Add(pair.Key, pair.Value.ToArray());

                if (SupportPurchaseFree && HasPurEnableOption)
                {
                    //따로 읽는다.
                    List<SpinBaseData> freeSpinDatas = await _spinDatabase.Ask<List<SpinBaseData>>(new ReadSpinInfoPurEnabledRequest(), TimeSpan.FromSeconds(30.0));
                    for (int i = 0; i < freeSpinDatas.Count; i++)
                    {
                        freeSpinTotalOdd += freeSpinDatas[i].Odd;
                        freeSpinTotalCount++;

                        if (!freeSpinOddIds.ContainsKey(freeSpinDatas[i].Odd))
                            freeSpinOddIds.Add(freeSpinDatas[i].Odd, new List<int>());
                        freeSpinOddIds[freeSpinDatas[i].Odd].Add(freeSpinDatas[i].ID);
                        if (freeSpinDatas[i].Odd >= PurchaseFreeMultiple[0] * 0.2 && freeSpinDatas[i].Odd <= PurchaseFreeMultiple[0] * 0.5)
                        {
                            minFreeSpinTotalCount++;
                            minFreeSpinTotalOdd += freeSpinDatas[i].Odd;
                        }
                    }
                }
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

                if (this.SupportMoreBet)
                {
                    int naturalEmptyCount = _naturalSpinOddProbs[0.0];
                    _anteBetMinusZeroCount = (int)((1.0 - 1.0 / MoreBetMultiple) * (double)_naturalSpinCount);

                    double moreBetWinRate = 0.0;
                    foreach (KeyValuePair<double, int> pair in _naturalSpinOddProbs)
                    {
                        moreBetWinRate += (pair.Key * pair.Value / (_naturalSpinCount - _anteBetMinusZeroCount));
                    }
                    if (_anteBetMinusZeroCount > naturalEmptyCount)
                        _logger.Error("More Bet Probabily calculation doesn't work in {0}", this.GameName);
                }

                if (this.SupportPurchaseFree && this.PurchaseFreeMultiple[0] > _totalFreeSpinWinRate)
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
            if (message.MsgCode == (ushort)CSMSG_CODE.CS_HABANERO_DOINIT)
            {
                onDoInit(strGlobalUserID, (int)currency, message, userBonus, userBalance, isMustLose);
            }
            else if (message.MsgCode == (ushort)CSMSG_CODE.CS_HABANERO_DOBALANCE)
            {
                onDoBalance(strGlobalUserID, (int)currency, message, userBalance);
            }
            else if (message.MsgCode == (ushort)CSMSG_CODE.CS_HABANERO_DOSPIN)
            {
                await onDoSpin(strUserID, agentID, (int)currency, message, userBonus, userBalance, isMustLose);
            }
        }
        protected virtual void onDoInit(string strGlobalUserID, int currency, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            try
            {
                string strGrid          = (string)message.Pop();
                string strToken         = (string)message.Pop();

                HabaneroResponse response = new HabaneroResponse();
                
                HabaneroResponseHeader header = makeHabaneroResponseHeader(strGlobalUserID, currency, userBalance, strToken);

                JObject portMessage = new JObject();
                portMessage["reelid"] = InitReelStatusNo;
                if(_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    BaseHabaneroSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
                    string gameInstanceId   = _dicUserResultInfos[strGlobalUserID].GameId;
                    string gameRoundId      = _dicUserResultInfos[strGlobalUserID].RoundId;

                    dynamic lastResult = JsonConvert.DeserializeObject<dynamic>(_dicUserResultInfos[strGlobalUserID].ResultString);
                    if(!object.ReferenceEquals(lastResult["isgamedone"], null) && !Convert.ToBoolean(lastResult["isgamedone"]))
                    {
                        JArray resumeGames = buildInitResumeGame(strGlobalUserID, betInfo,lastResult,gameInstanceId,gameRoundId);
                        portMessage["resumegames"]  = resumeGames;
                        portMessage["gssid"]        = lastResult["gssid"];
                    }
                }

                JObject game = new JObject();
                game["action"]      = "init";
                game["apiversion"]  = "5.1.10768.643";
                game["brandgameid"] = BrandGameId;
                game["friendlyid"]  = 0;
                game["gamehash"]    = GameHash;
                game["gameid"]      = "00000000-0000-0000-0000-000000000000";
                game["gameversion"] = "5.1.8539.397";
                game["jphash"]      = JPHash;
                game["jpversion"]   = JPVersion;
                game["rnghash"]     = RngHash;
                game["rngversion"]  = RngVersion;
                game["sessionid"]   = Guid.NewGuid().ToString();
                game["init"]        = new JObject();
                game["init"]["coinsincrement"]  = string.Join("|", CoinsIncrement);

                List<double> newStakeIncrement = new List<double>();
                for (int i = 0; i < StakeIncrement.Length; i++)
                {
                    newStakeIncrement.Add(StakeIncrement[i] * new Currencies()._currencyInfo[currency].Rate);
                }

                game["init"]["stakeincrement"]  = string.Join("|", newStakeIncrement);
                game["init"]["configid"]        = Guid.NewGuid().ToString();
                game["init"]["defaultstake"]    = newStakeIncrement[4];
                game["init"]["maxpaylimit"]     = MaxPayLimit * newStakeIncrement[newStakeIncrement.Count - 1];
                game["init"]["maxstake"]        = MiniCoin * CoinsIncrement[CoinsIncrement.Length - 1] * newStakeIncrement[newStakeIncrement.Count - 1];
                game["init"]["minstake"]        = MiniCoin * CoinsIncrement[0] * newStakeIncrement[0];

                response.game           = game;
                response.header         = header;
                response.grid           = strGrid;
                response.portmessage    = portMessage;
                
                GITMessage responseMessage      = new GITMessage((ushort)SCMSG_CODE.SC_HABANERO_DOINIT);
                responseMessage.Append(JsonConvert.SerializeObject(response));
                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseHabaneroSlotGame::onDoInit GameID: {0}, {1}", _gameID, ex);
            }
        }
        protected virtual JArray buildInitResumeGame(string strGlobalUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult,string gameinstanceid, string roundid,HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            JArray  resumeGames = new JArray();
            JObject resumeGame  = new JObject();

            resumeGame["friendlygameid"]    = roundid;
            resumeGame["gameinstanceid"]    = gameinstanceid;
            resumeGame["betcoin"]           = betInfo.CoinValue;
            resumeGame["betlevel"]          = betInfo.BetLevel;
            resumeGame["betlines"]          = betInfo.LineCount;
            resumeGame["virtualreels"]      = lastResult["virtualreels"];
            resumeGame["totalwincash"]      = lastResult["totalwincash"];
            resumeGame["numfreegames"]      = lastResult["numfreegames"];
            resumeGame["currfreegame"]      = lastResult["currentfreegame"];
            resumeGame["gamemode"]          = "freegame";
            resumeGames.Add(resumeGame);

            return resumeGames;
        }
        protected virtual JObject buildBetStates(BaseHabaneroSlotBetInfo betInfo)
        {
            return null;
        }
        protected virtual List<List<int>> convertJArrayToListIntArray(JArray virtualReelsJArray)
        {
            List<List<int>> virtualReels = new List<List<int>>();

            for (int i = 0; i < virtualReelsJArray.Count; i++)
            {
                List<int> virtualCol    = new List<int>();
                JArray virtaulColJArray = virtualReelsJArray[i] as JArray;
                for (int j = 0; j < virtaulColJArray.Count; j++)
                {
                    virtualCol.Add(Convert.ToInt32(virtaulColJArray[j]));
                }
                virtualReels.Add(virtualCol);
            }
            return virtualReels;
        }
        protected virtual JArray convertListIntArrayToJArray(List<List<int>> virtualReels)
        {
            JArray virtualReelsJArray = new JArray();

            for (int i = 0; i < virtualReels.Count; i++)
            {
                JArray virtaulColJArray = new JArray();
                List<int> virtualCol    = virtualReels[i];
                for (int j = 0; j < virtualCol.Count; j++)
                    virtaulColJArray.Add(virtualCol[j]);

                virtualReelsJArray.Add(virtaulColJArray);
            }
            return virtualReelsJArray;
        }
        protected virtual void onDoBalance(string strGlobalUserID,int currency, GITMessage message, double userBalance)
        {
            try
            {
                string strSessionID = (string)message.Pop();
                string strToken     = (string)message.Pop();

                HabaneroBaseResponse response = new HabaneroBaseResponse();
                response.header = makeHabaneroResponseHeader(strGlobalUserID,currency,userBalance,strToken);

                GITMessage reponseMessage = new GITMessage((ushort)SCMSG_CODE.SC_HABANERO_DOBALANCE);
                reponseMessage.Append(JsonConvert.SerializeObject(response));
                Sender.Tell(new ToUserMessage((int)_gameID, reponseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseHabaneroSlotGame::onDoBalance {0}", ex);
            }
        }
        protected virtual async Task onDoSpin(string strUserID, int agentID, int currency, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            try
            {
                _isRewardedBonus    = false;
                _bonusSendMessage   = null;
                _rewardedBonusMoney = 0.0;

                string strSessionID = (string) message.Pop();
                string strGrid      = (string) message.Pop();
                string strToken     = (string) message.Pop();

                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                readBetInfoFromMessage(message, strGlobalUserID);

                await spinGame(strUserID, agentID, currency, userBonus, userBalance, strSessionID, strToken,strGrid);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseHabaneroSlotGame::onDoSpin GameID: {0}, {1}", _gameID, ex);
            }
        }
        protected HabaneroResponseHeader makeHabaneroResponseHeader(string strGlobalUserID,int currency,double userBalance,string strToken)
        {
            HabaneroResponseHeader header = new HabaneroResponseHeader();
            header.st      = DateTime.UtcNow.ToString("o", CultureInfo.GetCultureInfo("en-US"));
            header.timing  = 50;
            header.player = new HabaneroResponseHeaderPlayer();
            header.player.brandid          = strGlobalUserID;
            header.player.curexp           = 0;
            header.player.displaybalance   = userBalance;
            header.player.gamesymbol       = new Currencies()._currencyInfo[currency].CurrencySymbol;
            header.player.hasbonus         = false;
            header.player.realbalance      = userBalance;
            header.player.ssotoken         = strToken;

            return header;
        }
        private HabaneroResponseGame makeHabaneroResponseGame(string action,string sessionId,string gameid,long friendlyid)
        {
            HabaneroResponseGame game = new HabaneroResponseGame();
            game.action         = action;
            game.brandgameid    = BrandGameId;
            game.dts            = DateTime.UtcNow.ToString("o", CultureInfo.GetCultureInfo("en-US"));
            game.friendlyid     = friendlyid;
            game.gameid         = gameid;
            game.sessionid      = sessionId;
            
            return game;
        }
        protected virtual HabaneroActionType convertStringToAction(string strAction)
        {
            switch (strAction)
            {
                case "main":
                    return HabaneroActionType.MAIN;
                case "freegame":
                    return HabaneroActionType.FREEGAME;
                case "symbolpick":
                    return HabaneroActionType.SYMBOLPICK;
                case "respin":
                    return HabaneroActionType.RESPIN;
                case "pick":
                    return HabaneroActionType.PICKOPTION;
                case "freegame0":
                    return HabaneroActionType.FREEGAME0;
                case "penguinfreegame":
                    return HabaneroActionType.PENGUINFREEGAME;
                case "polarbearfreegame":
                    return HabaneroActionType.POLARBEARFREEGAME;
                case "planetfreegame":
                    return HabaneroActionType.PLANETFREEGAME;
                case "bonusfreegame":
                    return HabaneroActionType.BONUSFREEGAME;
                case "bonus1":
                    return HabaneroActionType.BONUS1;
                case "bonus2":
                    return HabaneroActionType.BONUS2;
                case "bonus3":
                    return HabaneroActionType.BONUS3;
                case "bonus4":
                    return HabaneroActionType.BONUS4;
                case "bonus5":
                    return HabaneroActionType.BONUS5;
                case "bonus1respin":
                    return HabaneroActionType.BONUS1RESPIN;
                case "bonus2respin":
                    return HabaneroActionType.BONUS2RESPIN;
                case "bonus3respin":
                    return HabaneroActionType.BONUS3RESPIN;
                case "bonus4respin":
                    return HabaneroActionType.BONUS4RESPIN;
                case "fg_hans":
                    return HabaneroActionType.FG_HANS;
                case "fg_heidi":
                    return HabaneroActionType.FG_HEIDI;
                case "maincascade":
                    return HabaneroActionType.MAINCASECADE;
                case "freegamecascade":
                    return HabaneroActionType.FREEGAMECASCADE;
                case "chest":
                    return HabaneroActionType.CHEST;
                case "hollywoodstarpick":
                    return HabaneroActionType.HOLLYWOODSTARPICK;
                case "hollywoodstarfreegame":
                    return HabaneroActionType.HOLLYWOODSTARFREEGAME;
            }
            return HabaneroActionType.NONE;
        }
        protected virtual string convertActionToString(HabaneroActionType action)
        {
            switch (action)
            {
                case HabaneroActionType.MAIN:
                    return "main";
                case HabaneroActionType.FREEGAME:
                    return "freegame";
                case HabaneroActionType.SYMBOLPICK:
                    return "symbolpick";
                case HabaneroActionType.RESPIN:
                    return "respin";
                case HabaneroActionType.PICKOPTION:
                    return "pick";
                case HabaneroActionType.FREEGAME0:
                    return "freegame0";
                case HabaneroActionType.PENGUINFREEGAME:
                    return "penguinfreegame";
                case HabaneroActionType.POLARBEARFREEGAME:
                    return "polarbearfreegame";
                case HabaneroActionType.PLANETFREEGAME:
                    return "planetfreegame";
                case HabaneroActionType.BONUSFREEGAME:
                    return "bonusfreegame";
                case HabaneroActionType.BONUS1:
                    return "bonus1";
                case HabaneroActionType.BONUS2:
                    return "bonus2";
                case HabaneroActionType.BONUS3:
                    return "bonus3";
                case HabaneroActionType.BONUS4:
                    return "bonus4";
                case HabaneroActionType.BONUS5:
                    return "bonus5";
                case HabaneroActionType.BONUS1RESPIN:
                    return "bonus1respin";
                case HabaneroActionType.BONUS2RESPIN:
                    return "bonus2respin";
                case HabaneroActionType.BONUS3RESPIN:
                    return "bonus3respin";
                case HabaneroActionType.BONUS4RESPIN:
                    return "bonus4respin";
                case HabaneroActionType.FG_HANS:
                    return "fg_hans";
                case HabaneroActionType.FG_HEIDI:
                    return "fg_heidi";
                case HabaneroActionType.MAINCASECADE:
                    return "maincascade";
                case HabaneroActionType.FREEGAMECASCADE:
                    return "freegamecascade";
                case HabaneroActionType.CHEST:
                    return "chest";
                case HabaneroActionType.HOLLYWOODSTARPICK:
                    return "hollywoodstarpick";
                case HabaneroActionType.HOLLYWOODSTARFREEGAME:
                    return "hollywoodstarfreegame";
            }
            return "";
        }
        #endregion
        
        protected virtual void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
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
                    if (message.DataNum > 0)
                        oldBetInfo.PurchaseFree = (int)message.Pop();
                    else
                         oldBetInfo.PurchaseFree = 0;
                }
                else
                {
                    float coinValue = (float)message.Pop();
                    int lineCount   = (int)message.Pop();
                    int betLevel    = (int)message.Pop();

                    BaseHabaneroSlotBetInfo betInfo  = new BaseHabaneroSlotBetInfo(MiniCoin);
                    betInfo.CoinValue   = coinValue;
                    betInfo.LineCount   = lineCount;
                    betInfo.BetLevel    = betLevel;
                    if (message.DataNum > 0)
                        betInfo.PurchaseFree = (int)message.Pop();
                    else
                        betInfo.PurchaseFree = 0;

                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseHabaneroSlotGame::readBetInfoFromMessage {0}", ex);
            }
        }
        protected virtual string makeSpinResultString(BaseHabaneroSlotBetInfo betInfo, BaseHabaneroSlotSpinResult spinResult, double betMoney, double userBalance, string strUserID, string strSessionID, string strToken, string strGrid, HabaneroResponseHeader responseHeader)
        {
            HabaneroResponse response = new HabaneroResponse();
            HabaneroResponseGame game = new HabaneroResponseGame();
            game.action         = "game";
            game.brandgameid    = BrandGameId;
            game.dts            = DateTime.UtcNow.ToString("o", CultureInfo.GetCultureInfo("en-US"));
            game.gameid         = spinResult.GameId;
            game.sessionid      = strSessionID;
            game.friendlyid     = Convert.ToInt64(spinResult.RoundId);

            response.game           = game;
            response.header         = responseHeader;
            response.grid           = strGrid;
            dynamic portMessage     = JsonConvert.DeserializeObject<dynamic>(spinResult.ResultString);
            response.portmessage    = portMessage;
            return JsonConvert.SerializeObject(response);
        }
        protected double convertWinByBet(double win, double currentBet)
        {
            return Math.Round(win / _spinDataDefaultBet * currentBet, 2);
        }
        protected virtual void convertWinsByBet(dynamic resultContext, float currentBet)
        {
            if (!object.ReferenceEquals(resultContext["wincash"], null))
                resultContext["wincash"] = convertWinByBet((double)resultContext["wincash"], currentBet);
            if (!object.ReferenceEquals(resultContext["totalwincash"], null))
                resultContext["totalwincash"] = convertWinByBet((double)resultContext["totalwincash"], currentBet);
            if (!object.ReferenceEquals(resultContext["linewinscash"], null))
                resultContext["linewinscash"] = convertWinByBet((double)resultContext["linewinscash"], currentBet);
            if (!object.ReferenceEquals(resultContext["scatterwinscash"], null))
                resultContext["scatterwinscash"] = convertWinByBet((double)resultContext["scatterwinscash"], currentBet);
            if (!object.ReferenceEquals(resultContext["multiplierFeature"], null))
            {
                resultContext["multiplierFeature"]["winCashEnd"]    = convertWinByBet((double)resultContext["multiplierFeature"]["winCashEnd"], currentBet);
                resultContext["multiplierFeature"]["winCashStart"]  = convertWinByBet((double)resultContext["multiplierFeature"]["winCashStart"], currentBet);
            }
            if (!object.ReferenceEquals(resultContext["chestPickCash"], null))
                resultContext["chestPickCash"] = convertWinByBet((double)resultContext["chestPickCash"], currentBet);

            if (!object.ReferenceEquals(resultContext["reels"], null))
            {
                JArray reelArray = resultContext["reels"];
                for (int i = 0; i < reelArray.Count; i++)
                {
                    JArray reelRow = reelArray[i] as JArray;
                    for(int j = 0; j < reelRow.Count; j++)
                    {
                        if (!object.ReferenceEquals(reelRow[j]["wincash"], null))
                            reelRow[j]["wincash"] = convertWinByBet((double)reelRow[j]["wincash"], currentBet);
                    }
                }
            }
            if (!object.ReferenceEquals(resultContext["linewins"], null))
            {
                JArray lineWinsArray = resultContext["linewins"];
                for (int i = 0; i < lineWinsArray.Count; i++)
                {
                    if (!object.ReferenceEquals(lineWinsArray[i]["wincash"], null))
                        lineWinsArray[i]["wincash"] = convertWinByBet((double)lineWinsArray[i]["wincash"], currentBet);
                }
            }
            if (!object.ReferenceEquals(resultContext["scatterwins"], null))
            {
                JArray scatterWinsArray = resultContext["scatterwins"];
                for (int i = 0; i < scatterWinsArray.Count; i++)
                {
                    if (!object.ReferenceEquals(scatterWinsArray[i]["wincash"], null))
                        scatterWinsArray[i]["wincash"] = convertWinByBet((double)scatterWinsArray[i]["wincash"], currentBet);
                }
            }
            if (!object.ReferenceEquals(resultContext["FGMessage"], null) && !object.ReferenceEquals(resultContext["FGMessage"]["winCash"], null))
            {
                resultContext["FGMessage"]["winCash"] = convertWinByBet((double)resultContext["FGMessage"]["winCash"], currentBet);
            }

        }
        protected virtual BaseHabaneroSlotSpinResult calculateResult(string strGlobalUserID, BaseHabaneroSlotBetInfo betInfo, string strSpinResponse, bool isFirst,HabaneroActionType currentAction)
        {
            try
            {
                BaseHabaneroSlotSpinResult spinResult     = new BaseHabaneroSlotSpinResult();
                dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(resultContext, betInfo.TotalBet);

                string strNextAction    = (string)resultContext["nextgamestate"];
                spinResult.NextAction   = convertStringToAction(strNextAction);

                if (spinResult.NextAction == HabaneroActionType.NONE)
                {
                    _logger.Error("Unknown Action in BaseHabaneroSlotGame::calculateResult Action is {0}", strNextAction);
                    return null;
                }

                spinResult.CurrentAction = currentAction;

                if (isFirst)
                {
                    spinResult.RoundId  = (((long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds) * 5).ToString();
                    spinResult.GameId   = Guid.NewGuid().ToString();
                }
                else
                {
                    spinResult.RoundId  = _dicUserResultInfos[strGlobalUserID].RoundId;
                    spinResult.GameId   = _dicUserResultInfos[strGlobalUserID].GameId;
                }
                
                if (spinResult.NextAction == HabaneroActionType.MAIN)
                    spinResult.TotalWin = Convert.ToDouble(resultContext["totalwincash"]);
                
                spinResult.ResultString = JsonConvert.SerializeObject(resultContext);
                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseHabaneroSlotGame::calculateResult {0}", ex);
                return null;
            }
        }
        protected virtual List<BaseHabaneroActionToResponse> buildResponseList(List<string> responseList)
        {
            List<BaseHabaneroActionToResponse> actionResponseList = new List<BaseHabaneroActionToResponse>();
            for (int i = 1; i < responseList.Count; i++)
            {
                dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(responseList[i - 1]);
                HabaneroActionType actionType = convertStringToAction((string)resultContext["nextgamestate"]);
                actionResponseList.Add(new BaseHabaneroActionToResponse(actionType,responseList[i]));
            }
            return actionResponseList;
        }
        protected virtual double getPurchaseMultiple(BaseHabaneroSlotBetInfo betInfo)
        {
            return this.PurchaseFreeMultiple[betInfo.PurchaseFree - 1];
        }
        protected virtual async Task<BaseHabaneroSlotSpinResult> generateSpinResult(BaseHabaneroSlotBetInfo betInfo, string strUserID, int agentID, UserBonus userBonus, bool usePayLimit)
        {
            BasePPSlotSpinData          spinData    = null;
            BaseHabaneroSlotSpinResult  result      = null;

            string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
            if (betInfo.HasRemainResponse)
            {
                BaseHabaneroActionToResponse nextResponse = betInfo.pullRemainResponse();
                result = calculateResult(strGlobalUserID, betInfo, nextResponse.Response, false,nextResponse.ActionType);

                //프리게임이 끝났는지를 검사한다.
                if (!betInfo.HasRemainResponse)
                    betInfo.RemainReponses = null;
                return result;
            }

            //유저의 총 베팅액을 얻는다.
            float   totalBet     = betInfo.TotalBet;
            double  realBetMoney = totalBet;

            if (SupportPurchaseFree && betInfo.PurchaseFree > 0)
                realBetMoney = totalBet * getPurchaseMultiple(betInfo);//구매베팅금액

            spinData = await selectRandomStop(agentID, userBonus, totalBet, betInfo);

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

                    result = calculateResult(strGlobalUserID, betInfo, spinData.SpinStrings[0], true,HabaneroActionType.MAIN);
                    if (spinData.SpinStrings.Count > 1)
                        betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
                    return result;
                }
                while (false);
            }

            double emptyWin = 0.0;
            if (SupportPurchaseFree && betInfo.PurchaseFree > 0)
            {
                spinData    = await selectMinStartFreeSpinData(betInfo);
                result      = calculateResult(strGlobalUserID, betInfo, spinData.SpinStrings[0], true,HabaneroActionType.MAIN);
                emptyWin    = totalBet * spinData.SpinOdd;

                //뒤에 응답자료가 또 있다면
                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
            }
            else
            {
                spinData    = await selectEmptySpin(agentID, betInfo);
                result      = calculateResult(strGlobalUserID, betInfo, spinData.SpinStrings[0], true,HabaneroActionType.MAIN);
            }

            sumUpCompanyBetWin(agentID, realBetMoney, emptyWin);
            return result;
        }
        protected byte[] backupBetInfo(BaseHabaneroSlotBetInfo betInfo)
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
                    _dicUserHistory[strGlobalUserID].SerializeTo(writer);
                }
                return ms.ToArray();
            }
        }
        protected virtual async Task spinGame(string strUserID, int agentID, int currency, UserBonus userBonus, double userBalance, string strSessionID, string strToken,string strGrid)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                //해당 유저의 베팅정보를 얻는다. 만일 베팅정보가 없다면(례외상황) 그대로 리턴한다.
                BaseHabaneroSlotBetInfo betInfo = null;
                if (!_dicUserBetInfos.TryGetValue(strGlobalUserID, out betInfo))
                    return;

                byte[] betInfoBytes = backupBetInfo(betInfo);
                byte[] historyBytes = backupHistoryInfo(strGlobalUserID);

                BaseHabaneroSlotSpinResult lastResult = null;
                if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                    lastResult = _dicUserResultInfos[strGlobalUserID];

                double betMoney = betInfo.TotalBet;
                if (betInfo.HasRemainResponse)
                    betMoney = 0.0;

                if (this.SupportPurchaseFree && betInfo.PurchaseFree > 0)
                    betMoney = Math.Round(betMoney * getPurchaseMultiple(betInfo), 1);
                if (this.SupportMoreBet && betInfo.MoreBet > 0)
                    betMoney = Math.Round(betMoney * this.MoreBetMultiple, 1);


                //만일 베팅머니가 유저의 밸런스보다 크다면 끝낸다.(2020.02.15)
                if (userBalance.LT(betMoney, _epsilion) || betMoney < 0.0)
                {
                    _logger.Error("user balance is less than bet money in BaseHabaneroSlotGame::spinGame {0} balance:{1}, bet money: {2} game id:{3}", strUserID, userBalance, betMoney, _gameID);
                    return;
                }

                if (betMoney > 0.0)
                    _dicUserHistory[strGlobalUserID] = new HabaneroHistoryItem();

                //결과를 생성한다.
                BaseHabaneroSlotSpinResult spinResult = await this.generateSpinResult(betInfo, strUserID, agentID, userBonus, true);

                //게임로그
                string strGameLog                       = spinResult.ResultString;
                _dicUserResultInfos[strGlobalUserID]    = spinResult;
                
                if (betMoney > 0.0)
                {
                    _dicUserHistory[strGlobalUserID].GameId     = spinResult.GameId;
                    _dicUserHistory[strGlobalUserID].RoundId    = spinResult.RoundId;
                }

                //결과를 보내기전에 베팅정보를 디비에 보관한다.
                saveBetResultInfo(strGlobalUserID);

                HabaneroResponseHeader responseHeader = null;
                if (spinResult.NextAction == HabaneroActionType.MAIN)
                    responseHeader = makeHabaneroResponseHeader(strGlobalUserID, currency, userBalance - betMoney + spinResult.TotalWin, strToken);
                else
                    responseHeader = makeHabaneroResponseHeader(strGlobalUserID, currency, userBalance - betMoney, strToken);

                //생성된 게임결과를 유저에게 보낸다.
                sendGameResult(betInfo, spinResult, agentID, strUserID, currency, betMoney, spinResult.WinMoney, strGameLog, userBalance, strSessionID, strToken, strGrid, responseHeader);

                _dicUserLastBackupBetInfos[strGlobalUserID]     = betInfoBytes;
                _dicUserLastBackupResultInfos[strGlobalUserID]  = lastResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseHabaneroSlotGame::spinGame {0}", ex);
            }
        }
        protected virtual HabaneroLogItem buildHabaneroLogItem(BaseHabaneroSlotBetInfo betInfo, HabaneroHistoryItem history,int agentID ,string strUserID, int currency, double balance,double betMoney ,double winMoney)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                HabaneroLogItem historyItem = new HabaneroLogItem();
                historyItem.AgentID     = agentID;
                historyItem.UserID      = strUserID;
                historyItem.GameID      = (int)this._gameID;
                historyItem.Time        = DateTime.UtcNow;
                historyItem.RoundID     = history.RoundId;
                historyItem.GameLogID   = history.GameId;
                historyItem.Bet         = betInfo.TotalBet;
                if (betInfo.MoreBet > 0)
                    historyItem.Bet = betInfo.TotalBet * this.MoreBetMultiple;
                if (betInfo.PurchaseFree > 0)
                    historyItem.Bet = betInfo.TotalBet * getPurchaseMultiple(betInfo);
                historyItem.Win         = winMoney;

                HabaneroLogItemOverview overView = new HabaneroLogItemOverview();
                overView.CurrencyCode       = new Currencies()._currencyInfo[currency].CurrencyText;
                overView.DateToShow         = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
                overView.DtCompleted        = string.Format("/Date({0})/",overView.DateToShow);
                overView.Exponent           = 0;
                overView.ExtRoundId         = null;
                overView.FriendlyId         = Convert.ToInt64(history.RoundId);
                overView.GameInstanceId     = history.GameId;
                overView.GameKeyName        = this.SymbolName;
                overView.GameStateId        = 3;
                overView.IsTestSite         = false;
                overView.RealPayout         = winMoney;
                overView.RealStake          = historyItem.Bet;
                overView.ReplayURL          = HabaneroConfig.Instance.ReplayURL + history.GameId;
                overView.BrandGameId        = BrandGameId;
                overView.IsSpecialBrandGame = false;
                overView.GameTypeId         = 11;
                historyItem.Overview = JsonConvert.SerializeObject(overView);

                JObject detailValue = new JObject();
                detailValue["GameType"]              = 11;
                detailValue["GameState"]             = "GameState_3";
                detailValue["GameKeyName"]           = SymbolName;
                detailValue["CurrencyCode"]          = new Currencies()._currencyInfo[currency].CurrencyText;
                detailValue["FriendlyId"]            = history.RoundId;
                detailValue["DtStarted"]             = overView.DateToShow - 10;
                detailValue["DtCompleted"]           = overView.DateToShow;
                detailValue["RealStake"]             = historyItem.Bet;
                detailValue["RealPayout"]            = winMoney;
                detailValue["BonusStake"]            = 0;
                detailValue["BonusPayout"]           = 0;
                detailValue["BonusToReal"]           = 0;
                detailValue["CurrencyExponent"]      = 2;
                detailValue["BalanceAfter"]          = Math.Round(balance - betMoney + winMoney, 2);
                detailValue["IsCheat"]               = false;

                dynamic detailResult            = new JObject();
                dynamic detailResultReportInfo  = new JObject();
                detailResultReportInfo["numcoins"]          = this.MiniCoin;
                detailResultReportInfo["bettype"]           = "Lines";
                detailResultReportInfo["paylinecount"]      = this.ClientReqLineCount;
                detailResultReportInfo["coindenomination"]  = betInfo.CoinValue;
                detailResultReportInfo["linebet"]           = betInfo.CoinValue * betInfo.BetLevel;
                detailResultReportInfo["betlevel"]          = betInfo.BetLevel;
                detailResultReportInfo["totalbet"]          = betInfo.TotalBet;
                detailResultReportInfo["wincash"]           = winMoney;
                
                int winFreeGames = 0;
                dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(history.Responses[0].Response);
                if (!object.ReferenceEquals(resultContext["winfreegames"], null))
                    winFreeGames = Convert.ToInt32(resultContext["winfreegames"]);
                detailResultReportInfo["winfreegames"]      = winFreeGames;

                JArray eventArray = new JArray();
                for(int i = 0; i < history.Responses.Count; i++)
                {
                    JObject eventItem = buildEventItem(strGlobalUserID, i);
                    eventArray.Add(eventItem);
                }

                detailResultReportInfo["events"]        = eventArray;
                detailResult["ReportInfo"]              = JsonConvert.SerializeObject(detailResultReportInfo);
                detailValue["VideoSlotGameDetails"]     = detailResult;

                JObject detail = new JObject();
                detail["d"]         = JsonConvert.SerializeObject(detailValue);
                historyItem.Detail  = JsonConvert.SerializeObject(detail);

                return historyItem;
            }
            catch (Exception ex)
            {
                 _logger.Error("Exception has been occurred in BaseHabaneroSlotGame::saveResultToHistory {0}", ex);
                return null;
            }
        }
        protected virtual JObject buildEventItem(string strGlobalUserID, int currentIndex)
        {
            HabaneroHistoryResponses responses = _dicUserHistory[strGlobalUserID].Responses[currentIndex];
            JObject eventItem = new JObject();
            eventItem["type"]       = "spin";
            switch (responses.Action)
            {
                case HabaneroActionType.FREEGAME:
                    eventItem["gamemode"] = "FREEGAME";
                    break;
                case HabaneroActionType.RESPIN:
                    eventItem["gamemode"] = "RESPIN";
                    break;
                case HabaneroActionType.SYMBOLPICK:
                    eventItem["gamemode"]   = "SYMBOLPICK";
                    eventItem["type"]       = "pick";
                    break;
                case HabaneroActionType.FG_HEIDI:
                    eventItem["gamemode"] = "FG_HEIDI";
                    break;
                case HabaneroActionType.FG_HANS:
                    eventItem["gamemode"] = "FG_HANS";
                    break;
                case HabaneroActionType.FREEGAMECASCADE:
                    eventItem["gamemode"] = "FREEGAMECASCADE";
                    break;
                case HabaneroActionType.MAINCASECADE:
                    eventItem["gamemode"] = "MAINCASCADE";
                    break;
                case HabaneroActionType.CHEST:
                    eventItem["type"]       = "pick";
                    eventItem["gamemode"]   = "CHEST";
                    break;
                case HabaneroActionType.HOLLYWOODSTARPICK:
                    eventItem["type"]       = "pick";
                    eventItem["gamemode"]   = "HOLLYWOODSTARPICK";
                    break;
                case HabaneroActionType.HOLLYWOODSTARFREEGAME:
                    eventItem["gamemode"]   = "HOLLYWOODSTARFREEGAME";
                    break;
                case HabaneroActionType.MAIN:
                default:
                    eventItem["gamemode"] = "MAIN";
                    break;
            }
            eventItem["dt"]             = 637957353915568900;
            dynamic response = JsonConvert.DeserializeObject<dynamic>(responses.Response);
            
            if (!object.ReferenceEquals(response["reels"], null))
            {
                JArray reels = buildHabaneroLogReels(strGlobalUserID, currentIndex, response);
                eventItem["reels"] = reels;
            }

            eventItem["wincash"]        = 0;
            eventItem["winfreegames"]   = 0;
            eventItem["winmultiplier"]  = 0;
            eventItem["numwinlines"]    = 0;

            if (!object.ReferenceEquals(response["wincash"], null))
                eventItem["wincash"] = response["wincash"];
            if (!object.ReferenceEquals(response["winfreegames"], null))
                eventItem["winfreegames"] = response["winfreegames"];
            if (!object.ReferenceEquals(response["currgamemultiplier"], null))
                eventItem["multiplier"] = response["currgamemultiplier"];
            if (!object.ReferenceEquals(response["currentfreegame"], null))
                eventItem["spinno"] = response["currentfreegame"];

            JArray subEvents = new JArray();
            if (!object.ReferenceEquals(response["scatterwins"], null))
            {
                for(int j = 0; j < response["scatterwins"].Count; j++)
                {
                    JObject subEventItem = new JObject();
                    subEventItem["type"]        = "scatter";
                    subEventItem["wincash"]     = response["scatterwins"][j]["wincash"];
                    subEventItem["symbol"]      = "Scatter";
                    subEventItem["multiplier"]  = response["scatterwins"][j]["multiplier"];
                    JArray scaterArray = new JArray();
                    for(int k = 0; k < response["scatterwins"][j]["winningwindows"].Count; k++)
                    {
                        JArray scatterItem = new JArray();
                        scatterItem.Add(response["scatterwins"][j]["winningwindows"][k]["reelindex"]);
                        scatterItem.Add(response["scatterwins"][j]["winningwindows"][k]["symbolindex"]);
                        scaterArray.Add(scatterItem);
                    }
                    subEventItem["windows"]     = scaterArray;
                    subEvents.Add(subEventItem);
                }
            }

            if (!object.ReferenceEquals(response["linewins"], null))
            {
                for (int j = 0; j < response["linewins"].Count; j++)
                {
                    JObject subEventItem = new JObject();
                    subEventItem["type"]        = "payline";
                    subEventItem["wincash"]     = response["linewins"][j]["wincash"];
                    try
                    {
                        int symbol = Convert.ToInt32(response["linewins"][j]["symbolid"]);
                        string symbolName = SymbolIdStringForLog[symbol].name;
                        subEventItem["symbol"] = symbolName;
                        subEventItem["multiplier"] = response["linewins"][j]["multiplier"];
                        subEventItem["lineno"] = response["linewins"][j]["paylineindex"];
                        JArray lineWinArray = new JArray();
                        for (int k = 0; k < response["linewins"][j]["winningwindows"].Count; k++)
                        {
                            JArray lineWinItem = new JArray();
                            lineWinItem.Add(response["linewins"][j]["winningwindows"][k]["reelindex"]);
                            lineWinItem.Add(response["linewins"][j]["winningwindows"][k]["symbolindex"]);
                            lineWinArray.Add(lineWinItem);
                        }
                        subEventItem["windows"] = lineWinArray;
                    }
                    catch (Exception ex)
                    {

                    }
                    subEvents.Add(subEventItem);
                }
            }

            if (!object.ReferenceEquals(response["FGMessage"], null))
            {
                JObject cashPrizeItem = buildFGMessage(response);

                if(cashPrizeItem != null)
                {
                    JObject subEventItem = new JObject();
                    subEventItem["type"]    = cashPrizeItem["type"];
                    subEventItem["wincash"] = cashPrizeItem["wincash"];
                    subEvents.Add(subEventItem);

                    JObject customsubeventItem = new JObject();
                    customsubeventItem["type"]  = cashPrizeItem["customtype"];
                    customsubeventItem["pId"]   = cashPrizeItem["pId"];

                    JArray customSubEvent = new JArray();
                    customSubEvent.Add(customsubeventItem);
                    eventItem["customsubevents"] = customSubEvent;
                }
            }

            if (subEvents.Count > 0)
                eventItem["subevents"] = subEvents;

            return eventItem;
        }
        protected virtual JArray buildHabaneroLogReels(string strGlobalUserID, int currentIndex, dynamic response,bool containWild = false)
        {
            JArray reels = new JArray();
            for (int j = 0; j < response["virtualreels"].Count; j++)
            {
                JArray col = new JArray();
                for (int k = 2; k < response["virtualreels"][j].Count - 2; k++)
                {
                    int symbol      = Convert.ToInt32(response["virtualreels"][j][k]);
                    string symbolid = SymbolIdStringForLog[symbol].id;
                    col.Add(symbolid);
                }
                reels.Add(col);
            }
            return reels;
        }
        protected virtual JObject buildFGMessage(dynamic response)
        {
            return null;
        }
        protected virtual JArray buildHabaneroLogReelslist(dynamic response)
        {
            return null;
        }
        protected virtual JArray buildHabaneroLogReelsMeta(dynamic response)
        {
            return null;
        }
        protected virtual void sendGameResult(BaseHabaneroSlotBetInfo betInfo, BaseHabaneroSlotSpinResult spinResult,int agentID, string strUserID, int currency, double betMoney, double winMoney, string strGameLog, double userBalance, string strSessionID, string strToken,string strGrid ,HabaneroResponseHeader responseHeader)
        {
            string strGlobalUserID  = string.Format("{0}_{1}", agentID, strUserID);
            string strSpinResult    = makeSpinResultString(betInfo, spinResult, betMoney, userBalance, strGlobalUserID, strSessionID, strToken,strGrid ,responseHeader);
            GITMessage message      = new GITMessage((ushort)SCMSG_CODE.SC_HABANERO_DOSPIN);
            message.Append(strSpinResult);

            ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, message, betMoney, winMoney, new GameLogInfo(GameName, "0", strGameLog),UserBetTypes.Normal);
            if (_isRewardedBonus)
            {
                toUserResult.setBonusReward(_rewardedBonusMoney);
                toUserResult.insertFirstMessage(_bonusSendMessage);
            }

            if (_dicUserHistory.ContainsKey(strGlobalUserID))
                _dicUserHistory[strGlobalUserID].Responses.Add(new HabaneroHistoryResponses(spinResult.CurrentAction, DateTime.UtcNow, spinResult.ResultString));

            if (spinResult.NextAction == HabaneroActionType.MAIN)
                saveHistory(agentID, strUserID, currency, userBalance, betMoney, spinResult.TotalWin);

            Sender.Tell(toUserResult, Self);
        }

        #region 스핀자료처리부분
        protected OddAndIDData selectOddAndIDFromProbsWithRange(SortedDictionary<double, int[]> oddProbs, int totalCount, double minOdd, double maxOdd)
        {
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
            int random = Pcg.Default.Next(0, totalCount);
            int sum = 0;
            foreach (KeyValuePair<double, int[]> pair in oddProbs)
            {
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
        protected double selectOddFromProbs(SortedDictionary<double, int> oddProbs, int totalCount)
        {
            int random = Pcg.Default.Next(0, totalCount);
            int sum = 0;
            foreach (KeyValuePair<double, int> pair in oddProbs)
            {
                sum += pair.Value;
                if (random < sum)
                    return pair.Key;
            }
            return oddProbs.First().Key;
        }
        protected virtual async Task<BasePPSlotSpinData> selectRandomStop(int agentID, BaseHabaneroSlotBetInfo betInfo, bool isMoreBet)
        {
            OddAndIDData selectedOddAndID = selectRandomOddAndID(agentID, betInfo, isMoreBet);
            return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(selectedOddAndID.ID), TimeSpan.FromSeconds(10.0));
        }
        protected virtual OddAndIDData selectRandomOddAndID(int agentID, BaseHabaneroSlotBetInfo betInfo, bool isMoreBet)
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
        protected virtual async Task<BasePPSlotSpinData> selectEmptySpin(int agentID, BaseHabaneroSlotBetInfo betInfo)
        {
            int id = _emptySpinIDs[Pcg.Default.Next(0, _emptySpinIDs.Count)];
            return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(id), TimeSpan.FromSeconds(10.0));
        }
        protected virtual async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BaseHabaneroSlotBetInfo betInfo)
        {
            try
            {
                OddAndIDData oddAndID = selectOddAndIDFromProbs(_totalFreeSpinOddIds, _freeSpinTotalCount);
                return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseHabaneroSlotGame::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected virtual async Task<BasePPSlotSpinData> selectMinStartFreeSpinData(BaseHabaneroSlotBetInfo betInfo)
        {
            try
            {
                OddAndIDData oddAndID = selectOddAndIDFromProbsWithRange(_totalFreeSpinOddIds, _minFreeSpinTotalCount, PurchaseFreeMultiple[betInfo.PurchaseFree - 1] * 0.2, PurchaseFreeMultiple[betInfo.PurchaseFree - 1] * 0.5);
                return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseHabaneroSlotGame::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        #endregion

        protected virtual async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int agentID, BaseHabaneroSlotBetInfo betInfo, double baseBet, UserBonus userBonus)
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

            double targetC = PurchaseFreeMultiple[betInfo.PurchaseFree - 1] * payoutRate / 100.0;
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
        public virtual async Task<BasePPSlotSpinData> selectRandomStop(int agentID, UserBonus userBonus, double baseBet, BaseHabaneroSlotBetInfo betInfo)
        {
            //프리스핀구입을 먼저 처리한다.
            if (this.SupportPurchaseFree && betInfo.PurchaseFree > 0)
                return await selectPurchaseFreeSpin(agentID, betInfo, baseBet, userBonus);

            if (userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus rangeOddBonus = userBonus as UserRangeOddEventBonus;
                OddAndIDData oddAndID = selectOddAndIDFromProbsWithRange(_totalSpinOddIds, rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd);
                if (oddAndID != null)
                {
                    BasePPSlotSpinData spinDataEvent = await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
                    spinDataEvent.IsEvent = true;
                    return spinDataEvent;
                }
            }
            return await selectRandomStop(agentID, betInfo, false);
        }
        protected virtual void saveHistory(int agentID, string strUserID, int currency, double userBalance,double betMoney ,double winMoney)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                if (!_dicUserHistory.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID))
                    return;

                BaseHabaneroSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];

                HabaneroLogItem logItem = buildHabaneroLogItem(betInfo, _dicUserHistory[strGlobalUserID],agentID ,strUserID, currency, userBalance,betMoney ,winMoney);
                
                //게임히스토리디비에 보관
                _dbWriter.Tell(logItem);
                _dicUserHistory.Remove(strGlobalUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseHabaneroSlotGame::saveHistory {0}", ex);
            }
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
                        BinaryReader reader = new BinaryReader(stream);
                        BaseHabaneroSlotBetInfo betInfo = restoreBetInfo(strGlobalUserID, reader);
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
                        BaseHabaneroSlotSpinResult resultInfo = restoreResultInfo(strGlobalUserID, reader);
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
                        HabaneroHistoryItem userHistory = restoreHistory(reader);
                        if (userHistory != null)
                            _dicUserHistory[strGlobalUserID] = userHistory;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseHabaneroSlotGame::loadUserHistoricalData {0}", ex);
                return false;
            }
            return await base.loadUserHistoricalData(agentID, strUserID, isNewEnter);
        }
        protected virtual BaseHabaneroSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            BaseHabaneroSlotBetInfo betInfo = new BaseHabaneroSlotBetInfo(MiniCoin);
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected virtual BaseHabaneroSlotSpinResult restoreResultInfo(string strGlobalUserID, BinaryReader reader)
        {
            BaseHabaneroSlotSpinResult result = new BaseHabaneroSlotSpinResult();
            result.SerializeFrom(reader);
            return result;
        }
        protected virtual HabaneroHistoryItem restoreHistory(BinaryReader reader)
        {
            HabaneroHistoryItem history = new HabaneroHistoryItem();
            history.SerializeFrom(reader);
            return history;
        }
        protected virtual void saveBetResultInfo(string strGlobalUserID)
        {
            try
            {
                if (_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    byte[] betInfoBytes = _dicUserBetInfos[strGlobalUserID].convertToByte();
                    _redisWriter.Tell(new UserBetInfoWrite(strGlobalUserID, _gameID, betInfoBytes, false));
                }
                if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    byte[] resultInfoBytes = _dicUserResultInfos[strGlobalUserID].convertToByte();
                    _redisWriter.Tell(new UserResultInfoWrite(strGlobalUserID, _gameID, resultInfoBytes, false));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseHabaneroSlotGame::saveBetInfo {0}", ex);
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
                    byte[] historyInfoBytes = _dicUserHistory[strGlobalUserID].convertToByte();
                    await _redisWriter.Ask(new UserHistoryWrite(strGlobalUserID, _gameID, historyInfoBytes, true));
                    _dicUserHistory.Remove(strGlobalUserID);
                }
                else
                {
                    await _redisWriter.Ask(new UserHistoryWrite(strGlobalUserID, _gameID, null, true));
                }

                _dicUserLastBackupResultInfos.Remove(strGlobalUserID);
                _dicUserLastBackupBetInfos.Remove(strGlobalUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseHabaneroSlotGame::onUserExitGame GameID:{0} {1}", _gameID, ex);
            }

            await base.onUserExitGame(agentID, strUserID, userRequested);
        }
        protected string buildErrorResponse()
        {
            HabaneroErrorResponse response  = new HabaneroErrorResponse();
            response.header.st      = DateTime.UtcNow.ToString("o", CultureInfo.GetCultureInfo("en-US"));
            response.header.player.brandid           = "00000000-0000-0000-0000-000000000000";
            response.header.player.gamesymbol        = "";
            response.header.player.curexp            = 2;
            response.header.player.displaybalance    = 0.0;
            response.header.player.realbalance       = 0.0;
            response.header.player.ssotoken          = "";

            response.error.code     = "GENERAL_SESSIONEXPIRED";
            response.error.message  = "User session expired";
            response.error.type     = 2;
            response.error.suppress = false;
            response.error.msgcode  = "U.01";
            return JsonConvert.SerializeObject(response);
        }
    }
    public class HabaneroBaseResponse
    {
        public HabaneroResponseHeader header { get; set; }
    }
    public class HabaneroErrorResponse
    {
        public HabaneroResponseHeader       header  { get; set; }
        public HabaneroResponseSessionError error   { get; set; }
        public HabaneroErrorResponse()
        {
            header  = new HabaneroResponseHeader();
            error   = new HabaneroResponseSessionError();
        }

    }
    public class HabaneroResponseSessionError
    {
        public string   code        { get; set; }
        public string   message     { get; set; }
        public int      type        { get; set; }
        public bool     suppress    { get; set; }
        public string   msgcode     { get; set; }
    }
    public class HabaneroResponse : HabaneroBaseResponse
    {
        public dynamic  game                { get; set; }
        public string   grid                { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public dynamic  portmessage         { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public dynamic  resumevsgamelist    { get; set; }
    }
    public class HabaneroResponseHeader
    {
        public HabaneroResponseHeaderPlayer player  { get; set; }
        public string                       st      { get; set; }
        public int                          timing  { get; set; }
    }
    public class HabaneroResponseHeaderPlayer
    {
        public string   brandid         { get; set; }
        public int      curexp          { get; set; }
        public double   displaybalance  { get; set; }
        public string   gamesymbol      { get; set; }
        public bool     hasbonus        { get; set; }
        public double   realbalance     { get; set; }
        public string   ssotoken        { get; set; }
    }
    public class HabaneroResponseGame
    {
        public string   action      { get; set; }
        public string   brandgameid { get; set; }
        public string   dts         { get; set; }
        public long     friendlyid  { get; set; }
        public string   gameid      { get; set; }
        public string   sessionid   { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public dynamic  play        { get; set; }
    }
    public class HabaneroLogSymbolIDName
    {
        public string id    { get; set; }
        public string name  { get; set; }
    }
}
