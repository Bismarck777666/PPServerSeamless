using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using SlotGamesNode.Database;
using GITProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using GITProtocol.Utils;
using PCGSharp;
using System.Xml;

namespace SlotGamesNode.GameLogics
{
    class BasePlaysonSlotGame : IGameLogicActor
    {
        protected string                            _providerName           = "playson";
        //스핀디비관리액터
        protected IActorRef                         _spinDatabase           = null;
        protected double                            _spinDataDefaultBet     = 0.0f;

        protected int                               _normalMaxID            = 0;
        protected int                               _naturalSpinCount       = 0;
        protected SortedDictionary<double, int>     _naturalSpinOddProbs    = new SortedDictionary<double, int>();
        protected SortedDictionary<double, int[]>   _totalSpinOddIds        = new SortedDictionary<double, int[]>();
        protected List<int>                         _emptySpinIDs           = new List<int>();

        //프리스핀구매기능이 있을떄만 필요하다. 디비안의 모든 프리스핀들의 오드별 아이디어레이
        protected SortedDictionary<double, int[]> _totalFreeSpinOddIds = new SortedDictionary<double, int[]>();
        protected int       _freeSpinTotalCount     = 0;
        protected int       _minFreeSpinTotalCount  = 0;
        protected double    _totalFreeSpinWinRate   = 0.0; //스핀디비안의 모든 프리스핀들의 배당평균값
        protected double    _minFreeSpinWinRate     = 0.0; //구매금액의 20% - 50%사이에 들어가는 모든 프리스핀들의 평균배당값

        //앤티베팅기능이 있을때만 필요하다.(앤티베팅시 감소시켜야할 빈스핀의 갯수)
        protected int       _anteBetMinusZeroCount  = 0;


        //매유저의 베팅정보 
        protected Dictionary<string, BasePlaysonSlotBetInfo>    _dicUserBetInfos                = new Dictionary<string, BasePlaysonSlotBetInfo>();

        //유정의 마지막결과정보
        protected Dictionary<string, BasePlaysonSlotSpinResult> _dicUserResultInfos             = new Dictionary<string, BasePlaysonSlotSpinResult>();

        //백업정보
        protected Dictionary<string, BasePlaysonSlotSpinResult> _dicUserLastBackupResultInfos   = new Dictionary<string, BasePlaysonSlotSpinResult>();
        protected Dictionary<string, byte[]>                    _dicUserLastBackupBetInfos      = new Dictionary<string, byte[]>();
        protected Dictionary<string, byte[]>                    _dicUserLastBackupHistory       = new Dictionary<string, byte[]>();


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
        protected virtual int ClientReqLineCount
        {
            get
            {
                return 20;
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
            get { return false; }
        }
        protected virtual double MoreBetMultiple
        {
            get { return 0.0; }
        }
        protected virtual int[] StakeIncrement
        {
            get
            {
                return new int[]{ };
            }
        }
        protected virtual string InitDataString
        {
            get
            {
                return "";
            }
        }

        public BasePlaysonSlotGame()
        {
            ReceiveAsync<LoadSpinDataRequest>(onLoadSpinData);
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

                double companyPayoutRate = getPayoutRate(agentID);

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
                _spinDatabase = Context.ActorOf(Props.Create(() => new SpinDatabase(_providerName, this.GameName)), "spinDatabase");
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
                        if (freeSpinDatas[i].Odd >= PurchaseFreeMultiple * 0.2 && freeSpinDatas[i].Odd <= PurchaseFreeMultiple * 0.5)
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

                    _freeSpinTotalCount = freeSpinTotalCount;
                    _minFreeSpinTotalCount = minFreeSpinTotalCount;
                    _totalFreeSpinWinRate = freeSpinTotalOdd / freeSpinTotalCount;
                    _minFreeSpinWinRate = minFreeSpinTotalOdd / minFreeSpinTotalCount;

                    if (_totalFreeSpinWinRate <= _minFreeSpinWinRate || _minFreeSpinTotalCount == 0)
                        _logger.Error("min freespin rate doesn't satisfy condition {0}", this.GameName);
                }

                if (SupportMoreBet)
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

                if (SupportPurchaseFree && PurchaseFreeMultiple > _totalFreeSpinWinRate)
                    _logger.Error("freespin win rate doesn't satisfy condition {0}", GameName);

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
            if (message.MsgCode == (ushort)CSMSG_CODE.CS_PLAYSON_DOCONNECT)
            {
                onDoConnect(strGlobalUserID, (int)currency, message, userBonus, userBalance, isMustLose);
            }
            else if (message.MsgCode == (ushort)CSMSG_CODE.CS_PLAYSON_DOSTART || message.MsgCode == (ushort)CSMSG_CODE.CS_PLAYSON_DORECONNECT)
            {
                onDoStart(strGlobalUserID, message, userBalance);
            }
            else if (message.MsgCode == (ushort)CSMSG_CODE.CS_PLAYSON_DOSYNC)
            {
                onDoSync(strGlobalUserID, message, userBalance);
            }
            else if (message.MsgCode == (ushort)CSMSG_CODE.CS_PLAYSON_DOPLAY)
            {
                await onDoPlay(strUserID, agentID, (int)currency, message, userBonus, userBalance, isMustLose);
            }
            else if (message.MsgCode == (ushort)CSMSG_CODE.CS_PP_NOTPROCDRESULT)
            {
                onDoUndoUserSpin(strGlobalUserID);
            }
        }
        
        protected virtual void onDoConnect(string strGlobalUserID,int currency, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            try
            {
                string strRnd   = (string)message.Pop();
                string strToken = (string)message.Pop();

                XmlDocument responseDoc = new XmlDocument();

                XmlElement serverNode = responseDoc.CreateElement("server");
                serverNode.SetAttribute("command",  "connect");
                serverNode.SetAttribute("rnd",      strRnd);
                serverNode.SetAttribute("session",  strToken);
                serverNode.SetAttribute("status",   "ok");
                if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
                    serverNode.SetAttribute("status", "reconnect");

                XmlElement systemNode           = responseDoc.CreateElement("system");
                systemNode.SetAttribute("guidcreate", strToken);
                XmlElement extraNode            = responseDoc.CreateElement("extra");
                XmlElement stakeIncrementNode   = responseDoc.CreateElement("stakeIncrement");

                List<int> newStakeIncrement = new List<int>();
                for(int i = 0; i < StakeIncrement.Length; i++)
                {
                    newStakeIncrement.Add(StakeIncrement[i] * new Currencies()._currencyInfo[currency].Rate);
                }

                stakeIncrementNode.InnerText    = string.Join(",", newStakeIncrement);
                XmlElement defaultBetNode       = responseDoc.CreateElement("defaultbet");
                defaultBetNode.InnerText = 7.ToString();

                extraNode.AppendChild(stakeIncrementNode);
                extraNode.AppendChild(defaultBetNode);

                XmlElement sourceNode   = responseDoc.CreateElement("source");
                sourceNode.SetAttribute("server-ver", "0.0.0");
                XmlElement gameNode     = responseDoc.CreateElement("game");
                gameNode.SetAttribute("name", SymbolName);
                XmlElement userNode     = responseDoc.CreateElement("user");
                userNode.SetAttribute("currency-id", new Currencies()._currencyInfo[currency].CurrencyText);
                userNode.SetAttribute("is_test",        "false");
                userNode.SetAttribute("type",           "real");
                userNode.SetAttribute("gs_id",          "0000");
                userNode.SetAttribute("wlid",           "0000");
                userNode.SetAttribute("wl_code_id",     "star");
                userNode.SetAttribute("cash_type",      "real");
                userNode.SetAttribute("wlcode",         "star");
                userNode.SetAttribute("game_id",        "0000");
                XmlElement user_newNode = responseDoc.CreateElement("user_new");
                user_newNode.SetAttribute("cash", ((long)(userBalance * 100)).ToString());

                serverNode.AppendChild(systemNode);
                serverNode.AppendChild(extraNode);
                serverNode.AppendChild(sourceNode);
                serverNode.AppendChild(gameNode);
                serverNode.AppendChild(userNode);
                serverNode.AppendChild(user_newNode);

                responseDoc.AppendChild(serverNode);

                GITMessage responseMessage      = new GITMessage((ushort)SCMSG_CODE.SC_PLAYSON_DOCONNECT);
                responseMessage.Append(serverNode.OuterXml);
                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePlaysonSlotGame::onDoConnect GameID: {0}, {1}", _gameID, ex);
            }
        }
        
        protected virtual void onDoSync(string strGlobalUserID, GITMessage message, double userBalance)
        {
            try
            {
                string strRnd   = (string)message.Pop();
                string strToken = (string)message.Pop();

                XmlDocument responseDoc = new XmlDocument();

                XmlElement serverNode = responseDoc.CreateElement("server");
                serverNode.SetAttribute("command",  "sync");
                serverNode.SetAttribute("rnd",      strRnd);
                serverNode.SetAttribute("session",  strToken);
                serverNode.SetAttribute("status",   "ok");

                XmlElement moneyNode    = responseDoc.CreateElement("money");
                moneyNode.SetAttribute("type", "real");
                moneyNode.SetAttribute("denominator", "1");
                moneyNode.SetAttribute("cashcurrent", ((long)(100 * userBalance)).ToString());

                XmlElement user_newNode = responseDoc.CreateElement("user_new");
                user_newNode.SetAttribute("cash", ((long)(userBalance * 100)).ToString());

                serverNode.AppendChild(moneyNode);
                serverNode.AppendChild(user_newNode);
                responseDoc.AppendChild(serverNode);

                GITMessage responseMessage      = new GITMessage((ushort)SCMSG_CODE.SC_PLAYSON_DOSYNC);
                responseMessage.Append(serverNode.OuterXml);
                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePlaysonSlotGame::onDoSync GameID: {0}, {1}", _gameID, ex);
            }
        }
        
        protected virtual void addLastResultForStart(XmlDocument responseDoc, string strGlobalUserID)
        {
            if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                XmlDocument lastResult = new XmlDocument();
                lastResult.LoadXml(_dicUserResultInfos[strGlobalUserID].ResultString);
                XmlNode lastResultServerNode = lastResult.SelectSingleNode("/server");

                XmlElement spin_cmdNode = responseDoc.CreateElement("spin_cmd");
                spin_cmdNode.SetAttribute("status", "ok");

                foreach (XmlNode xn in lastResultServerNode.ChildNodes)
                {
                    XmlNode copiedNode = responseDoc.ImportNode(xn, true);
                    spin_cmdNode.AppendChild(copiedNode);
                }

                XmlNode lastGameNode    = lastResult.SelectSingleNode("/server/game");
                XmlNode gameNode        = responseDoc.SelectSingleNode("/server/game");

                if (lastGameNode.Attributes["spec_symbol_id"] != null)
                    ((XmlElement)gameNode).SetAttribute("spec_symbol_id", lastGameNode.Attributes["spec_symbol_id"].Value);

                if(_dicUserResultInfos[strGlobalUserID].NextAction == PlaysonActionTypes.FREESPIN)
                    ((XmlElement)gameNode).SetAttribute("last_nfs_win", _dicUserResultInfos[strGlobalUserID].FreeTrigerWin.ToString());

                string roundnum = _dicUserResultInfos[strGlobalUserID].RoundID;

                XmlElement roundNumNode = responseDoc.CreateElement("roundnum");
                roundNumNode.SetAttribute("value", roundnum);

                XmlNode serverNode = responseDoc.SelectSingleNode("/server");
                serverNode.AppendChild(spin_cmdNode);
                serverNode.AppendChild(roundNumNode);
            }
        }
        
        protected virtual void onDoStart(string strGlobalUserID, GITMessage message, double userBalance)
        {
            try
            {
                string strRnd   = (string)message.Pop();
                string strToken = (string)message.Pop();

                XmlDocument responseDoc = new XmlDocument();
                responseDoc.LoadXml(InitDataString);

                XmlElement serverNode = responseDoc["server"];
                if(message.MsgCode == (ushort)CSMSG_CODE.CS_PLAYSON_DORECONNECT)
                    serverNode.SetAttribute("command",  "reconnect");
                else
                    serverNode.SetAttribute("command", "start");
                serverNode.SetAttribute("rnd",      strRnd);
                serverNode.SetAttribute("session",  strToken);
                serverNode.SetAttribute("status",   "ok");

                addLastResultForStart(responseDoc, strGlobalUserID);

                XmlElement user_newNode = responseDoc.CreateElement("user_new");
                user_newNode.SetAttribute("cash", ((long)(userBalance * 100)).ToString());

                serverNode.AppendChild(user_newNode);
                responseDoc.AppendChild(serverNode);

                GITMessage responseMessage      = new GITMessage((ushort)SCMSG_CODE.SC_PLAYSON_DOSTART);
                responseMessage.Append(serverNode.OuterXml);
                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePlaysonSlotGame::onDoStart GameID: {0}, {1}", _gameID, ex);
            }
        }
        
        protected virtual async Task onDoPlay(string strUserID, int agentID, int currency, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            try
            {
                _isRewardedBonus    = false;
                _bonusSendMessage   = null;
                _rewardedBonusMoney = 0.0;

                string strRnd       = (string) message.Pop();
                string strToken     = (string)message.Pop();

                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                readBetInfoFromMessage(message, strGlobalUserID);
                string strPlayClient = (string)message.Pop();
                await spinGame(strUserID, agentID, currency, userBonus, userBalance, strRnd, strToken, strPlayClient);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePlaysonSlotGame::onDoPlay GameID: {0}, {1}", _gameID, ex);
            }
        }
        
        protected PlaysonActionTypes convertStringToAction(string strAction)
        {
            switch(strAction)
            {
                case "idle":
                    return PlaysonActionTypes.SPIN;
                case "fs":
                    return PlaysonActionTypes.FREESPIN;
                case "bonus":
                    return PlaysonActionTypes.BONUS;
                case "respin":
                    return PlaysonActionTypes.Respin;

            }
            return PlaysonActionTypes.NONE;
        }
        
        protected string convertActionToString(PlaysonActionTypes action)
        {
            switch(action)
            {
                case PlaysonActionTypes.SPIN:
                    return "idle";
                case PlaysonActionTypes.FREESPIN:
                    return "fs";
                case PlaysonActionTypes.BONUS:
                    return "bonus";
                case PlaysonActionTypes.Respin:
                    return "respin";
            }
            return "";
        }
        
        protected void onDoUndoUserSpin(string strGlobalUserID)
        {
            undoUserResultInfo(strGlobalUserID);
            undoUserBetInfo(strGlobalUserID);
            saveBetResultInfo(strGlobalUserID);
        }
        #endregion

        protected virtual void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                int betPerLine  = (int)message.Pop();
                int totalBet    = (int)message.Pop();
                int purFree     = (int)message.Pop();
                BasePlaysonSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strGlobalUserID, out oldBetInfo))
                {
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    if (betPerLine <= 0.0f)
                    {
                        _logger.Error("{0} betInfo.BetPerLine <= 0 in BasePlaysonSlotGame::readBetInfoFromMessage {1}", strGlobalUserID, betPerLine);
                        return;
                    }

                    oldBetInfo.BetPerLine   = betPerLine;
                    oldBetInfo.TotalBet     = totalBet;
                    if(purFree == 1)
                        oldBetInfo.PurchaseFree = true;
                    else
                        oldBetInfo.PurchaseFree = false;
                }
                else
                {
                    if (betPerLine <= 0.0f)
                    {
                        _logger.Error("{0} betInfo.BetPerLine <= 0 in BasePlaysonSlotGame::readBetInfoFromMessage {1}", strGlobalUserID, betPerLine);
                        return;
                    }
                    
                    BasePlaysonSlotBetInfo betInfo  = new BasePlaysonSlotBetInfo();
                    betInfo.BetPerLine      = betPerLine;
                    betInfo.TotalBet        = totalBet;
                    if (purFree == 1)
                        betInfo.PurchaseFree = true;
                    else
                        betInfo.PurchaseFree = false;

                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePlaysonSlotGame::readBetInfoFromMessage {0}", ex);
            }
        }
        
        protected virtual void undoUserResultInfo(string strGlobalUserID)
        {
            try
            {
                if (!_dicUserLastBackupResultInfos.ContainsKey(strGlobalUserID))
                    return;

                BasePlaysonSlotSpinResult lastResult = _dicUserLastBackupResultInfos[strGlobalUserID];
                if (lastResult == null)
                    _dicUserResultInfos.Remove(strGlobalUserID);
                else
                    _dicUserResultInfos[strGlobalUserID] = lastResult;
                _dicUserLastBackupResultInfos.Remove(strGlobalUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePlaysonSlotGame::undoUserResultInfo {0}", ex);
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
                        BasePlaysonSlotBetInfo betInfo = restoreBetInfo(strGlobalUserID, binaryReader);
                        _dicUserBetInfos[strGlobalUserID] = betInfo;
                    }
                }
                _dicUserLastBackupBetInfos.Remove(strGlobalUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePlaysonSlotGame::undoUserBetInfo {0}", ex);
            }
        }

        protected virtual string makeBalanceNotEnoughResult(string strGlobalUserID, double userBalance, string strToken, string strRnd, BasePlaysonSlotBetInfo betInfo)
        {
            XmlDocument responseDoc = new XmlDocument();
            
            XmlElement serverNode   = responseDoc.CreateElement("server");
            if (strRnd == "0")
                strRnd = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
            
            serverNode.SetAttribute("command",  "bet");
            serverNode.SetAttribute("session",  strToken);
            serverNode.SetAttribute("rnd",      strRnd);
            serverNode.SetAttribute("status",   "excess");

            XmlElement extraNode    = responseDoc.CreateElement("extra");
            XmlElement errorNode    = responseDoc.CreateElement("error");
            errorNode.SetAttribute("code",      "NOT_ENOUGH_MONEY");
            XmlElement msgNode      = responseDoc.CreateElement("msg");
            msgNode.InnerText       = "Low balance";

            XmlElement balanceNode  = responseDoc.CreateElement("balance");
            balanceNode.SetAttribute("balance", (userBalance * 100).ToString());

            errorNode.AppendChild(msgNode);
            extraNode.AppendChild(errorNode);
            extraNode.AppendChild(balanceNode);
            serverNode.AppendChild(extraNode);
            responseDoc.AppendChild(serverNode);

            return responseDoc.InnerXml;
        }
        
        protected virtual void convertBetsAndWinsByBet(XmlDocument resultDoc, float betPerLine, float totalBet)
        {
            XmlNodeList winswinNodeList = resultDoc.SelectNodes("/server/wins/win");
            foreach(XmlNode xn in winswinNodeList)
            {
                if(xn.Attributes["cash"] != null)
                    ((XmlElement)xn).SetAttribute("cash", convertWinByBet(Convert.ToDouble(xn.Attributes["cash"].Value), totalBet).ToString());
            }

            XmlNodeList winsNewWinNodeList = resultDoc.SelectNodes("/server/wins/newwin");
            foreach (XmlNode xn in winsNewWinNodeList)
            {
                if (xn.Attributes["cash"] != null)
                    ((XmlElement)xn).SetAttribute("cash", convertWinByBet(Convert.ToDouble(xn.Attributes["cash"].Value), totalBet).ToString());
                 if (xn.Attributes["freespin_start_win"] != null)
                    ((XmlElement)xn).SetAttribute("freespin_start_win", convertWinByBet(Convert.ToDouble(xn.Attributes["freespin_start_win"].Value), totalBet).ToString());
                if (xn.Attributes["freespins_win"] != null)
                    ((XmlElement)xn).SetAttribute("freespins_win", convertWinByBet(Convert.ToDouble(xn.Attributes["freespins_win"].Value), totalBet).ToString());
            }

            XmlNodeList winsBoostNodeList = resultDoc.SelectNodes("/server/wins/boost");
            foreach (XmlNode xn in winsBoostNodeList)
            {
                if (xn.Attributes["cash"] != null)
                    ((XmlElement)xn).SetAttribute("cash", convertWinByBet(Convert.ToDouble(xn.Attributes["cash"].Value), totalBet).ToString());
            }

            XmlNodeList gameNodeList = resultDoc.SelectNodes("/server/game");
            foreach (XmlNode xn in gameNodeList)
            {
                if (xn.Attributes["cash-bet"] != null)
                    ((XmlElement)xn).SetAttribute("cash-bet",       convertWinByBet(Convert.ToDouble(xn.Attributes["cash-bet"].Value), totalBet).ToString());
                if (xn.Attributes["cash-bet-game"] != null)
                    ((XmlElement)xn).SetAttribute("cash-bet-game",  convertWinByBet(Convert.ToDouble(xn.Attributes["cash-bet-game"].Value), totalBet).ToString());
                if (xn.Attributes["cash-win"] != null)
                    ((XmlElement)xn).SetAttribute("cash-win",       convertWinByBet(Convert.ToDouble(xn.Attributes["cash-win"].Value), totalBet).ToString());
                if (xn.Attributes["scatters"] != null)
                    ((XmlElement)xn).SetAttribute("scatters",       convertWinByBet(Convert.ToDouble(xn.Attributes["scatters"].Value), totalBet).ToString());
                if (xn.Attributes["free-win"] != null)
                    ((XmlElement)xn).SetAttribute("free-win",       convertWinByBet(Convert.ToDouble(xn.Attributes["free-win"].Value), totalBet).ToString());
                if (xn.Attributes["fs_total_win"] != null)
                    ((XmlElement)xn).SetAttribute("fs_total_win",   convertWinByBet(Convert.ToDouble(xn.Attributes["fs_total_win"].Value), totalBet).ToString());
                if (xn.Attributes["current_win"] != null)
                    ((XmlElement)xn).SetAttribute("current_win",    convertWinByBet(Convert.ToDouble(xn.Attributes["current_win"].Value), totalBet).ToString());
                if (xn.Attributes["last_nfs_win"] != null)
                    ((XmlElement)xn).SetAttribute("last_nfs_win",   convertWinByBet(Convert.ToDouble(xn.Attributes["last_nfs_win"].Value), totalBet).ToString());
                if (xn.Attributes["total-win"] != null)
                    ((XmlElement)xn).SetAttribute("total-win",      convertWinByBet(Convert.ToDouble(xn.Attributes["total-win"].Value), totalBet).ToString());
                if (xn.Attributes["round_win"] != null)
                    ((XmlElement)xn).SetAttribute("round_win",      convertWinByBet(Convert.ToDouble(xn.Attributes["round_win"].Value), totalBet).ToString());
            }

            XmlNodeList gameBonusNodeList = resultDoc.SelectNodes("/server/game/bonus");
            foreach(XmlNode xn in gameBonusNodeList)
            {
                if(xn.Attributes["bet"] != null)
                    ((XmlElement)xn).SetAttribute("bet", convertWinByBet(Convert.ToDouble(xn.Attributes["bet"].Value), totalBet).ToString());
                if (xn.Attributes["win"] != null)
                    ((XmlElement)xn).SetAttribute("win", convertWinByBet(Convert.ToDouble(xn.Attributes["win"].Value), totalBet).ToString());
                if (xn.Attributes["boost_win"] != null)
                    ((XmlElement)xn).SetAttribute("boost_win", convertWinByBet(Convert.ToDouble(xn.Attributes["boost_win"].Value), totalBet).ToString());

            }

            XmlNodeList bonusNodeList = resultDoc.SelectNodes("/server/bonus");
            foreach(XmlNode xn in bonusNodeList)
            {
                if(xn.Attributes["bet"] != null)
                    ((XmlElement)xn).SetAttribute("bet", convertWinByBet(Convert.ToDouble(xn.Attributes["bet"].Value), totalBet).ToString());
                if (xn.Attributes["win"] != null)
                    ((XmlElement)xn).SetAttribute("win", convertWinByBet(Convert.ToDouble(xn.Attributes["win"].Value), totalBet).ToString());
                if (xn.Attributes["boost_win"] != null)
                    ((XmlElement)xn).SetAttribute("boost_win", convertWinByBet(Convert.ToDouble(xn.Attributes["boost_win"].Value), totalBet).ToString());
                if (xn.Attributes["cash"] != null)
                    ((XmlElement)xn).SetAttribute("cash", convertWinByBet(Convert.ToDouble(xn.Attributes["cash"].Value), totalBet).ToString());
            }

            XmlNodeList winSpecialWinList = resultDoc.SelectNodes("/server/win_special/win");
            foreach(XmlNode xn in winSpecialWinList)
            {
                if (xn.Attributes["cash"] != null)
                    ((XmlElement)xn).SetAttribute("cash", convertWinByBet(Convert.ToDouble(xn.Attributes["cash"].Value), totalBet).ToString());
            }
        }

        protected double convertWinByBet(double win, double currentBet)
        {
            return Math.Round(win / _spinDataDefaultBet * currentBet, 2);
        }

        protected virtual void convertLineBetByBetPerLine(XmlDocument resultDoc, float betPerLine, float totalBet)
        {
            XmlNodeList gameNodeList = resultDoc.SelectNodes("/server/game");
            foreach (XmlNode xn in gameNodeList)
            {
                if (xn.Attributes["line-bet"] != null)
                    ((XmlElement)xn).SetAttribute("line-bet", ((int)betPerLine).ToString());
            }
        }

        protected virtual BasePlaysonSlotSpinResult calculateResult(string strGlobalUserID, BasePlaysonSlotBetInfo betInfo, string strSpinResponse, bool isFirst,PlaysonActionTypes currentAction)
        {
            try
            {
                BasePlaysonSlotSpinResult spinResult    = new BasePlaysonSlotSpinResult();

                XmlDocument resultDoc = new XmlDocument();
                resultDoc.LoadXml(strSpinResponse);

                convertLineBetByBetPerLine(resultDoc, betInfo.BetPerLine, betInfo.TotalBet);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertBetsAndWinsByBet(resultDoc, betInfo.BetPerLine, betInfo.TotalBet);

                XmlNode serverNode  = resultDoc.SelectSingleNode("/server");
                XmlNode stateNode   = resultDoc.SelectSingleNode("/server/state");
                XmlNode gameNode    = resultDoc.SelectSingleNode("/server/game");
                XmlNode bonusNode   = resultDoc.SelectSingleNode("/server/bonus");

                string strNextAction        = stateNode.Attributes["current_state"].Value;
                spinResult.NextAction       = convertStringToAction(strNextAction);
                spinResult.CurrentAction    = currentAction;

                double roundWin = 0.0;
                if (gameNode != null && gameNode.Attributes["cash-win"] != null)
                    roundWin = Convert.ToDouble(gameNode.Attributes["cash-win"].Value);

                if (bonusNode != null && bonusNode.Attributes["win"] != null)
                {
                    if (spinResult.NextAction != PlaysonActionTypes.BONUS)
                        roundWin += Convert.ToDouble(bonusNode.Attributes["win"].Value);
                }

                double totalWin             = roundWin;
                spinResult.TotalWin         = roundWin / 100.0;
                spinResult.FreeTrigerWin    = 0.0;
                spinResult.TransactionID    = Guid.NewGuid().ToString().Replace("-", "") + "1";
                spinResult.RoundID          = ((long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString() + "001";

                if (gameNode != null && gameNode.Attributes["last_nfs_win"] != null)
                {
                    spinResult.FreeTrigerWin = Convert.ToDouble(gameNode.Attributes["last_nfs_win"].Value);
                }
                else if (spinResult.NextAction != PlaysonActionTypes.SPIN)
                {
                    if (spinResult.CurrentAction == PlaysonActionTypes.SPIN)
                        spinResult.FreeTrigerWin = totalWin;
                    else if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                        spinResult.FreeTrigerWin = _dicUserResultInfos[strGlobalUserID].FreeTrigerWin;
                }

                if (betInfo.PurchaseFree && gameNode != null && gameNode.Attributes["freegame_type"] != null)
                    ((XmlElement)gameNode).SetAttribute("freegame_type", "bought");

                if (betInfo.PurchaseFree && gameNode != null && gameNode.Attributes["bonus_game_type"] != null)
                    ((XmlElement)gameNode).SetAttribute("bonus_game_type", "bought");

                spinResult.ResultString = resultDoc.InnerXml;
                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePlaysonSlotGame::calculateResult {0}", ex);
                return null;
            }
        }
        
        protected List<BasePlaysonActionToResponse> buildResponseList(List<string> responseList)
        {
            List<BasePlaysonActionToResponse> actionResponseList = new List<BasePlaysonActionToResponse>();
            for (int i = 1; i < responseList.Count; i++)
            {
                XmlDocument rootDoc = new XmlDocument();
                rootDoc.LoadXml(responseList[i - 1]);
                XmlNode stateNode = rootDoc.SelectSingleNode("/server/state");
                PlaysonActionTypes  actionType     = convertStringToAction(stateNode.Attributes["current_state"].Value);
                actionResponseList.Add(new BasePlaysonActionToResponse(actionType, responseList[i]));
            }
            return actionResponseList;
        }
        
        protected virtual async Task<BasePlaysonSlotSpinResult> generateSpinResult(BasePlaysonSlotBetInfo betInfo, string strUserID, int agentID, UserBonus userBonus, bool usePayLimit)
        {
            BasePPSlotSpinData          spinData    = null;
            BasePlaysonSlotSpinResult   result      = null;

            string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
            if (betInfo.HasRemainResponse)
            {
                BasePlaysonActionToResponse nextResponse = betInfo.pullRemainResponse();
                result = calculateResult(strGlobalUserID, betInfo, nextResponse.Response, false,nextResponse.ActionType);

                //프리게임이 끝났는지를 검사한다.
                if (!betInfo.HasRemainResponse)
                    betInfo.RemainReponses = null;
                return result;
            }

            //유저의 총 베팅액을 얻는다.
            float   totalBet        = betInfo.TotalBet;
            double realBetMoney     = totalBet;

            if (SupportPurchaseFree && betInfo.PurchaseFree)
                realBetMoney = totalBet * PurchaseFreeMultiple;//구매베팅금액

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
                        _rewardedBonusMoney = totalWin / 100;
                        _isRewardedBonus    = true;
                    }

                    result = calculateResult(strGlobalUserID, betInfo, spinData.SpinStrings[0], true, PlaysonActionTypes.SPIN);
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
                result      = calculateResult(strGlobalUserID, betInfo, spinData.SpinStrings[0], true, PlaysonActionTypes.SPIN);
                emptyWin    = totalBet * spinData.SpinOdd;

                //뒤에 응답자료가 또 있다면
                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
            }
            else
            {
                spinData    = await selectEmptySpin(agentID, betInfo);
                result      = calculateResult(strGlobalUserID, betInfo, spinData.SpinStrings[0], true,PlaysonActionTypes.SPIN);
            }
            sumUpCompanyBetWin(agentID, realBetMoney, emptyWin);
            return result;
        }

        protected byte[] backupBetInfo(BasePlaysonSlotBetInfo betInfo)
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
        
        protected virtual async Task spinGame(string strUserID, int agentID, int currency, UserBonus userBonus, double userBalance, string strRnd, string strToken,string strPlayClient)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                //해당 유저의 베팅정보를 얻는다. 만일 베팅정보가 없다면(례외상황) 그대로 리턴한다.
                BasePlaysonSlotBetInfo betInfo = null;
                if (!_dicUserBetInfos.TryGetValue(strGlobalUserID, out betInfo))
                    return;

                byte[] betInfoBytes = backupBetInfo(betInfo);

                BasePlaysonSlotSpinResult lastResult = null;
                if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                    lastResult = _dicUserResultInfos[strGlobalUserID];

                double betMoney = betInfo.TotalBet / 100;
                if (betInfo.HasRemainResponse)
                    betMoney = 0.0;

                if (this.SupportPurchaseFree && betInfo.PurchaseFree)
                    betMoney = Math.Round(betMoney * PurchaseFreeMultiple, 1);

                //만일 베팅머니가 유저의 밸런스보다 크다면 끝낸다.
                if (userBalance.LT(betMoney, _epsilion) || betMoney < 0.0)
                {
                    string strBalanceErrorResult = makeBalanceNotEnoughResult(strGlobalUserID, userBalance, strToken, strRnd, betInfo);
                    GITMessage message = new GITMessage((ushort)SCMSG_CODE.SC_PLAYSON_DOPLAY);
                    message.Append(strBalanceErrorResult);
                    Sender.Tell(new ToUserMessage((int)_gameID, message));
                    saveBetResultInfo(strGlobalUserID);
                    return;
                }

                //결과를 생성한다.
                BasePlaysonSlotSpinResult spinResult = await generateSpinResult(betInfo, strUserID, agentID, userBonus, true);
                changeSessionAndRndToResponse(spinResult, strToken, strRnd);
                addBalanceInfo(spinResult, userBalance);

                //게임로그
                string strGameLog = spinResult.ResultString;
                _dicUserResultInfos[strGlobalUserID] = spinResult;

                //결과를 보내기전에 베팅정보를 디비에 보관한다
                saveBetResultInfo(strGlobalUserID);

                //생성된 게임결과를 유저에게 보낸다.
                sendGameResult(betInfo, spinResult, strGlobalUserID, betMoney, spinResult.WinMoney, strGameLog, userBalance, strRnd, strToken);

                _dicUserLastBackupBetInfos[strGlobalUserID]     = betInfoBytes;
                _dicUserLastBackupResultInfos[strGlobalUserID]  = lastResult;

                //게임결과를 디비에 보관한다.
                saveResultToHistory(spinResult,agentID ,strUserID, currency, userBalance, betMoney, spinResult.WinMoney,spinResult.CurrentAction, strPlayClient);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePlaysonSlotGame::spinGame {0}", ex);
            }
        }
        
        protected void changeSessionAndRndToResponse(BasePlaysonSlotSpinResult spinResult,string strToken,string strRnd)
        {
            XmlDocument responseDoc = new XmlDocument();
            responseDoc.LoadXml(spinResult.ResultString);
            XmlNode serverNode = responseDoc.SelectSingleNode("/server");
            ((XmlElement)serverNode).SetAttribute("session",    strToken);
            ((XmlElement)serverNode).SetAttribute("rnd",        strRnd);

            spinResult.ResultString = responseDoc.InnerXml;
        }

        protected void addBalanceInfo(BasePlaysonSlotSpinResult spinResult, double userBalance)
        {
            XmlDocument responseDoc = new XmlDocument();
            responseDoc.LoadXml(spinResult.ResultString);
            XmlNode serverNode  = responseDoc.SelectSingleNode("/server");
            XmlNode gameNode    = responseDoc.SelectSingleNode("/server/game");

            double betMoney = 0;
            double winMoney = 0;

            if (gameNode != null && gameNode.Attributes["cash-bet"] != null)
                betMoney = Convert.ToDouble(gameNode.Attributes["cash-bet"].Value);
            
            if (gameNode != null && gameNode.Attributes["cash-win"] != null)
                winMoney = Convert.ToDouble(gameNode.Attributes["cash-win"].Value);
            
            if(gameNode != null && gameNode.Attributes["usercash"] != null)
                ((XmlElement)gameNode).SetAttribute("usercash", ((long)(userBalance * 100 - betMoney)).ToString());

            XmlElement user_newNode = responseDoc.CreateElement("user_new");
            user_newNode.SetAttribute("cash", ((long)(userBalance * 100 - betMoney + winMoney)).ToString());
            serverNode.AppendChild(user_newNode);

            spinResult.ResultString = responseDoc.InnerXml;
        }

        protected virtual void saveResultToHistory(BasePlaysonSlotSpinResult result,int agentID ,string strUserID, int currency, double balance, double betMoney, double winMoney, PlaysonActionTypes action,string strPlayClient)
        {
            try
            {
                PlaysonHistoryItem historyItem = new PlaysonHistoryItem();
                historyItem.AgentID         = agentID;
                historyItem.UserID          = strUserID;
                historyItem.Bet             = betMoney;
                historyItem.Win             = winMoney;
                historyItem.TransactionID   = result.TransactionID;
                historyItem.RoundID         = result.RoundID;
                historyItem.Time            = DateTime.UtcNow;
                historyItem.GameID          = (int) this._gameID;

                PlaysonHistoryItemOverview overview = new PlaysonHistoryItemOverview();
                overview.balance_before         = Math.Round(balance, 2).ToString();
                overview.balance_after          = Math.Round(balance - betMoney + winMoney, 2).ToString();
                if(betMoney == 0.0)
                    overview.bet = null;
                else
                    overview.bet = Math.Round(betMoney, 2).ToString();
                overview.win                    = Math.Round(winMoney, 2).ToString();
                overview.brand                  = "*";
                overview.c_at                   = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                overview.round_id               = result.RoundID;
                overview.transaction_id         = result.TransactionID;
                overview.currency               = new Currencies()._currencyInfo[currency].CurrencyText;
                overview.exceed_code            = "";
                overview.exceed_message         = "";
                overview.mode                   = "REAL";
                overview.tag                    = "";
                overview.status                 = "OK";
                overview.type                   = "COMMIT";
                overview.player_id              = strUserID;
                overview.game_id                = ((int) this._gameID).ToString();
                overview.game_name              = this.SymbolName;
                overview.is_bonus               = false;
                overview.outcome                = Math.Round(winMoney - betMoney, 2).ToString();
                overview.profit                 = Math.Round(betMoney - winMoney, 2).ToString();
                if (action == PlaysonActionTypes.SPIN)
                    overview.round_started = true;
                else
                    overview.round_started = false;
                if (result.NextAction == PlaysonActionTypes.SPIN)
                    overview.round_finished = true;
                else
                    overview.round_finished = false;
                
                historyItem.Overview = JsonConvert.SerializeObject(overview);

                string detail = "<request>" + strPlayClient + "</request>" + result.ResultString;
                detail = detail.Replace("<server", "<response").Replace("/server>", "/response>");
                historyItem.Detail              = detail;

                _dbWriter.Tell(historyItem);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePlaysonSlotGame::saveResultToHistory {0}", ex);
            }
        }
        
        protected virtual void sendGameResult(BasePlaysonSlotBetInfo betInfo, BasePlaysonSlotSpinResult spinResult, string strGlobalUserID, double betMoney, double winMoney, string strGameLog, double userBalance, string strRnd, string strToken)
        {
            GITMessage message = new GITMessage((ushort)SCMSG_CODE.SC_PLAYSON_DOPLAY);
            message.Append(spinResult.ResultString);

            ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, message, betMoney, winMoney, new GameLogInfo(GameName, "0", strGameLog),UserBetTypes.Normal);
            if (_isRewardedBonus)
            {
                toUserResult.setBonusReward(_rewardedBonusMoney);
                toUserResult.insertFirstMessage(_bonusSendMessage);
            }
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
        protected virtual async Task<BasePPSlotSpinData> selectRandomStop(int agentID, BasePlaysonSlotBetInfo betInfo, bool isMoreBet)
        {
            OddAndIDData selectedOddAndID = selectRandomOddAndID(agentID, betInfo, isMoreBet);
            return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(selectedOddAndID.ID), TimeSpan.FromSeconds(10.0));
        }
        protected virtual OddAndIDData selectRandomOddAndID(int agentID, BasePlaysonSlotBetInfo betInfo, bool isMoreBet)
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
            for (int i = 0; i < _totalSpinOddIds[selectedOdd].Length; i++)
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
        protected virtual async Task<BasePPSlotSpinData> selectEmptySpin(int agentID, BasePlaysonSlotBetInfo betInfo)
        {
            int id = _emptySpinIDs[Pcg.Default.Next(0, _emptySpinIDs.Count)];
            return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(id), TimeSpan.FromSeconds(10.0));
        }
        protected virtual async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePlaysonSlotBetInfo betInfo)
        {
            try
            {
                OddAndIDData oddAndID = selectOddAndIDFromProbs(_totalFreeSpinOddIds, _freeSpinTotalCount);
                return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePlaysonSlotGame::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected virtual async Task<BasePPSlotSpinData> selectMinStartFreeSpinData(BasePlaysonSlotBetInfo betInfo)
        {
            try
            {
                OddAndIDData oddAndID = selectOddAndIDFromProbsWithRange(_totalFreeSpinOddIds, _minFreeSpinTotalCount, PurchaseFreeMultiple * 0.2, PurchaseFreeMultiple * 0.5);
                return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePlaysonSlotGame::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        #endregion

        protected virtual async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int agentID, BasePlaysonSlotBetInfo betInfo, double baseBet, UserBonus userBonus)
        {
            if (userBonus != null && userBonus is UserRangeOddEventBonus && (userBonus as UserRangeOddEventBonus).MaxBet * 100 >= betInfo.TotalBet)
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

        public virtual async Task<BasePPSlotSpinData> selectRandomStop(int agentID, UserBonus userBonus, double baseBet, BasePlaysonSlotBetInfo betInfo)
        {
            //프리스핀구입을 먼저 처리한다.
            if (SupportPurchaseFree && betInfo.PurchaseFree)
                return await selectPurchaseFreeSpin(agentID, betInfo, baseBet, userBonus);

            if (userBonus != null && userBonus is UserRangeOddEventBonus && (userBonus as UserRangeOddEventBonus).MaxBet * 100 >= betInfo.TotalBet)
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

            if (SupportMoreBet && betInfo.MoreBet)
                return await selectRandomStop(agentID, betInfo, true);
            else
                return await selectRandomStop(agentID, betInfo, false);
        }
        
        protected override async Task<bool> loadUserHistoricalData(int agentID, string strUserID, bool isNewEnter)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                string strKey = string.Format("{0}_{1}", strGlobalUserID, _gameID);
                byte[] betInfoData = await RedisDatabase.RedisCache.StringGetAsync(strKey);
                if (betInfoData != null)
                {
                    using (var stream = new MemoryStream(betInfoData))
                    {
                        BinaryReader reader = new BinaryReader(stream);
                        BasePlaysonSlotBetInfo betInfo = restoreBetInfo(strGlobalUserID, reader);
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
                        BasePlaysonSlotSpinResult resultInfo = restoreResultInfo(strGlobalUserID, reader);
                        if (resultInfo != null)
                            _dicUserResultInfos[strGlobalUserID] = resultInfo;
                    }
                }
            }

            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePlaysonSlotGame::loadUserHistoricalData {0}", ex);
                return false;
            }
            return await base.loadUserHistoricalData(agentID, strUserID, isNewEnter);
        }
        
        protected virtual BasePlaysonSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            BasePlaysonSlotBetInfo betInfo = new BasePlaysonSlotBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        
        protected virtual BasePlaysonSlotSpinResult restoreResultInfo(string strGlobalUserID, BinaryReader reader)
        {
            BasePlaysonSlotSpinResult result = new BasePlaysonSlotSpinResult();
            result.SerializeFrom(reader);
            return result;
        }
        
        protected virtual void saveBetResultInfo(string strGlobalUserID)
        {
            try
            {
                if (_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    byte[] betInfoBytes = (_dicUserBetInfos[strGlobalUserID]).convertToByte();
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
                _logger.Error("Exception has been occurred in BasePlaysonSlotGame::saveBetInfo {0}", ex);
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
                
                _dicUserLastBackupResultInfos.Remove(strGlobalUserID);
                _dicUserLastBackupBetInfos.Remove(strGlobalUserID);
                _dicUserLastBackupHistory.Remove(strGlobalUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePlaysonSlotGame::onUserExitGame GameID:{0} {1}", _gameID, ex);
            }

            await base.onUserExitGame(agentID, strUserID, userRequested);
        }
    }
}
