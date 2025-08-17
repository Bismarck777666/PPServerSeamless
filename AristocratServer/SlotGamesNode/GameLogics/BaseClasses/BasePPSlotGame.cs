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
using Google.Protobuf.WellKnownTypes;
using Akka.Util;
using Microsoft.Extensions.Logging;
using System.Data;
using Akka.IO;
using System.Reflection;

namespace SlotGamesNode.GameLogics
{
    public class BasePPSlotGame : IGameLogicActor
    {
        protected IActorRef _spinDatabase = null;
        protected double _spinDataDefaultBet = 0.0f;

        protected int _normalMaxID = 0;
        protected int _naturalSpinCount = 0;
        protected SortedDictionary<double, int> _naturalSpinOddProbs = new SortedDictionary<double, int>();
        protected SortedDictionary<double, int[]> _totalSpinOddIds = new SortedDictionary<double, int[]>();
        protected List<int> _emptySpinIDs = new List<int>();
        protected SortedDictionary<int, double[]> _lineSpinOddProbs = new SortedDictionary<int, double[]>();

        protected SortedDictionary<double, int[]> _totalFreeSpinOddIds = new SortedDictionary<double, int[]>();
        protected int _freeSpinTotalCount = 0;
        protected int _minFreeSpinTotalCount = 0;
        protected double _totalFreeSpinWinRate = 0.0;
        protected double _minFreeSpinWinRate = 0.0;

        protected int _anteBetMinusZeroCount = 0;

        protected Dictionary<string, PPFreeSpinInfo> _dicUserFreeSpinInfos = new Dictionary<string, PPFreeSpinInfo>();

        protected Dictionary<string, BasePPSlotBetInfo> _dicUserBetInfos = new Dictionary<string, BasePPSlotBetInfo>();

        protected Dictionary<string, BasePPHistory> _dicUserHistory = new Dictionary<string, BasePPHistory>();

        protected Dictionary<string, BasePPSlotSpinResult> _dicUserResultInfos = new Dictionary<string, BasePPSlotSpinResult>();

        protected Dictionary<string, string> _dicUserSettings = new Dictionary<string, string>();

        protected Dictionary<string, BasePPSlotSpinResult> _dicUserLastBackupResultInfos = new Dictionary<string, BasePPSlotSpinResult>();
        protected Dictionary<string, byte[]> _dicUserLastBackupBetInfos = new Dictionary<string, byte[]>();
        protected Dictionary<string, byte[]> _dicUserLastBackupHistory = new Dictionary<string, byte[]>();
        protected Dictionary<string, byte[]> _dicUserLastBackupFreespinInfos = new Dictionary<string, byte[]>();

        protected string _defaultSetting = "SoundState=true_true_true_true_true;FastPlay=false;Intro=true;StopMsg=0;TurboSpinMsg=0;BetInfo=0_0;BatterySaver=false;ShowCCH=true;ShowFPH=true;CustomGameStoredData=;Coins=false;Volume=1;InitialScreen=8,9,6_5,3,7_7,5,4_3,6,3_9,7,8;SBPLock=true";

        protected Dictionary<int, double> _companyPayoutRates = new Dictionary<int, double>();

        #region 베팅풀정보(각 company마다 있다)
        protected Dictionary<int, double[]> _companyTotalBets = new Dictionary<int, double[]>();
        protected Dictionary<int, double[]> _companyTotalWins = new Dictionary<int, double[]>();
        #endregion
        protected virtual int FreeSpinTypeCount
        {
            get
            {
                return 0;
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
                return 5;
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
            get { return false; }
        }
        protected virtual double MoreBetMultiple
        {
            get { return 0.0; }
        }

        public BasePPSlotGame()
        {
            ReceiveAsync<LoadSpinDataRequest>(onLoadSpinData);
            Receive<CopyDataRequest>(onCopyData);
            Receive<InsertStatement>(onInsertStatement);
        }

        private void onInsertStatement(InsertStatement _)
        {

            Sender.Tell(this.SymbolName);
        }
        protected override void LoadSetting()
        {
            base.LoadSetting();
            initGameData();

            if (PayoutConfigSnapshot.Instance.CompanyPayoutConfigs.ContainsKey(_gameID))
                _companyPayoutRates = PayoutConfigSnapshot.Instance.CompanyPayoutConfigs[_gameID];
            else
                _companyPayoutRates = new Dictionary<int, double>();
        }
        protected void onAgentPayoutConfigUpdated(AgentPayoutConfigUpdated updated)
        {
            int agentID = updated.AgentID;
            if (updated.ChangedPayout)
                resetCompanyPayoutPool(agentID);
        }

        protected virtual void addDefaultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter)
        {
            dicParams["balance"] = Math.Round(userBalance, 2).ToString();
            dicParams["balance_cash"] = Math.Round(userBalance, 2).ToString();
            dicParams["balance_bonus"] = "0.0";
            dicParams["stime"] = GameUtils.GetCurrentUnixTimestampMillis().ToString();
            dicParams["index"] = index.ToString();
            dicParams["counter"] = (counter + 1).ToString();
            dicParams["sver"] = "5";
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
                if (!_companyTotalBets.ContainsKey(companyID))
                    _companyTotalBets[companyID] = new double[PoolCount];

                if (!_companyTotalWins.ContainsKey(companyID))
                    _companyTotalWins[companyID] = new double[PoolCount];

                _companyTotalBets[companyID][poolIndex] += betMoney;
                _companyTotalWins[companyID][poolIndex] += winMoney;

            }
        }
        protected bool checkCompanyPayoutRate(int companyID, double betMoney, double winMoney, int poolIndex = 0)
        {
            if (betMoney == 0.0 && winMoney == 0.0)
                return true;

            if (companyID == 0)
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
                if (!_companyTotalBets.ContainsKey(companyID))
                    _companyTotalBets[companyID] = new double[PoolCount];

                if (!_companyTotalWins.ContainsKey(companyID))
                    _companyTotalWins[companyID] = new double[PoolCount];

                double companyPayoutRate = _config.PayoutRate;
                if (_companyPayoutRates.ContainsKey(companyID))
                    companyPayoutRate = _companyPayoutRates[companyID];

                double totalBet = _companyTotalBets[companyID][poolIndex] + betMoney;
                double totalWin = _companyTotalWins[companyID][poolIndex] + winMoney;

                double maxTotalWin = totalBet * companyPayoutRate / 100.0 + _config.PoolRedundency;
                if (totalWin > maxTotalWin)
                    return false;

                _companyTotalBets[companyID][poolIndex] = totalBet;
                _companyTotalWins[companyID][poolIndex] = totalWin;
                return true;
            }
        }

        protected void resetCompanyPayoutPool(int agentID)
        {
            if (!_companyTotalBets.ContainsKey(agentID) || !_companyTotalWins.ContainsKey(agentID))
                return;

            for (int i = 0; i < PoolCount; i++)
            {
                _companyTotalBets[agentID][i] = 0.0;
                _companyTotalWins[agentID][i] = 0.0;
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
        private void onCopyData(CopyDataRequest request)
        {
            string strSourceFile = string.Format("F:\\Jobs\\GodCasino\\GodCasinoServer\\SlotGamesNode(TextDB)\\bin\\Debug\\slotdata\\{0}.db", this.GameName);
            string strDestFile = string.Format("F:\\Jobs\\MYCasino\\SlotCityCasinoServer\\SlotGamesNode\\bin\\Debug\\slotdata\\{0}.db", this.GameName);

            File.Copy(strSourceFile, strDestFile, true);

            string strSourcePath = string.Format("F:\\Jobs\\GodCasino\\GodCasinoClient\\GodCasinoClient\\PPGame\\common\\games-html5\\games\\vs\\{0}", SymbolName);
            string strDestPath = string.Format("F:\\Jobs\\MYCasino\\SlotCityCasinoClient\\SlotCityCasinoClient\\PPGame\\common\\games-html5\\games\\vs\\{0}", SymbolName);
            CopyFilesRecursively(strSourcePath, strDestPath);

        }

        private void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }
        protected virtual async Task<bool> loadSpinData()
        {
            try
            {
                _spinDatabase = Context.ActorOf(Akka.Actor.Props.Create(() => new SpinDatabase(this.GameName)), "spinDatabase");
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

                _normalMaxID = response.NormalMaxID;
                _naturalSpinOddProbs = new SortedDictionary<double, int>();
                _spinDataDefaultBet = response.DefaultBet;
                _naturalSpinCount = 0;
                _emptySpinIDs = new List<int>();

                Dictionary<double, List<int>> totalSpinOddIds = new Dictionary<double, List<int>>();
                Dictionary<double, List<int>> freeSpinOddIds = new Dictionary<double, List<int>>();
                Dictionary<int, List<double>> lineSpinOddIds = new Dictionary<int, List<double>>();

                double freeSpinTotalOdd = 0.0;
                double minFreeSpinTotalOdd = 0.0;
                int freeSpinTotalCount = 0;
                int minFreeSpinTotalCount = 0;
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
        protected override async Task onProcMessage(string strUserID, int companyID, GITMessage message, UserBonus bonus, double userBalance, Currencies currency, int agentMoneyMode)
        {
            if (message.MsgCode == (ushort)CSMSG_CODE.CS_PP_DOINIT)
            {
                onDoInit(strUserID, message, userBalance, bonus, currency, agentMoneyMode);
            }
            else if (message.MsgCode == (ushort)CSMSG_CODE.CS_PP_SAVESETTING)
            {
                onSaveSetting(strUserID, message, userBalance);
            }
            else if (message.MsgCode == (ushort)CSMSG_CODE.CS_PP_DOSPIN)
            {
                await onDoSpin(strUserID, companyID, message, bonus, userBalance, currency, agentMoneyMode);
            }
            else if (message.MsgCode == (ushort)CSMSG_CODE.CS_PP_DOCOLLECT)
            {
                onDoCollect(strUserID, message, userBalance, bonus);
            }
            else if (message.MsgCode == (ushort)CSMSG_CODE.CS_PP_RELOADBALANCE)
            {
                onReloadBalance(strUserID, message, userBalance);
            }
            else if (message.MsgCode == (ushort)CSMSG_CODE.CS_PP_NOTPROCDRESULT)
            {
                onDoUndoUserSpin(strUserID);
            }
            else if (message.MsgCode == (ushort)CSMSG_CODE.CS_PP_DOBONUS)
            {
                await onDoBonus(companyID, strUserID, message, userBalance, bonus);
            }
            else if (message.MsgCode == (ushort)CSMSG_CODE.CS_PP_DOCOLLECTBONUS)
            {
                onDoCollectBonus(strUserID, message, userBalance, bonus);
            }
        }
        protected virtual void onDoInit(string strUserID, GITMessage message, double userBalance, UserBonus userBonus, Currencies currency, int moneyMode)
        {
            try
            {
                string strResponse = genInitResponse(strUserID, userBalance, false, currency, moneyMode, userBonus);

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
                var now = DateTime.Now;
                var dateTimeStr = now.ToString("yyyy-MM-dd HH:mm:ss");
                Dictionary<string, string> responseParams = new Dictionary<string, string>();
                responseParams.Add("balance", Math.Round(userBalance, 2).ToString());
                responseParams.Add("dateTime", dateTimeStr);
                GITMessage reponseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_RELOADBALANCE);
                string jsonString = JsonConvert.SerializeObject(responseParams);
                reponseMessage.Append(jsonString);
                Sender.Tell(new ToUserMessage((int)_gameID, reponseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::onDoCollect {0}", ex);
            }
        }
        protected virtual void onSaveSetting(string strUserID, GITMessage message, double userBalance)
        {
            try
            {
                bool isLoad = (bool)message.Pop();
                if (!isLoad)
                {
                    string strNewSetting = (string)message.Pop();
                    _dicUserSettings[strUserID] = strNewSetting;
                }

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_SAVESETTING);
                if (_dicUserSettings.ContainsKey(strUserID))
                    responseMessage.Append(_dicUserSettings[strUserID]);
                else
                    responseMessage.Append(_defaultSetting);

                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::onSaveSetting GameID: {0}, {1}", _gameID, ex);
            }
        }
        private long getUnixMiliTimestamp()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            long unixTimeMilliseconds = now.ToUnixTimeMilliseconds();
            return unixTimeMilliseconds;
        }
        protected string createRoundID()
        {
            return string.Format("{0}{1}", getUnixMiliTimestamp(), Guid.NewGuid().ToString().Replace("-", ""));
        }
        protected string createTransactionID()
        {
            return string.Format("{0}{1}", getUnixMiliTimestamp(), Guid.NewGuid().ToString().Replace("-", ""));
        }
        protected virtual async Task onDoSpin(string strUserID, int companyID, GITMessage message, UserBonus bonus, double userBalance, Currencies currency, int moneyMode)
        {
            try
            {
                BasePPSlotBetInfo betInfo = null;
                bool isNewBet = true;
                if (_dicUserBetInfos.TryGetValue(strUserID, out betInfo))
                {
                    if (betInfo.HasRemainResponse)
                        isNewBet = false;
                }

                readBetInfoFromMessage(message, strUserID);
                if (isNewBet && _dicUserBetInfos.TryGetValue(strUserID, out betInfo))
                {
                    betInfo.BetTransactionID = createTransactionID();
                    betInfo.RoundID = createRoundID();
                }
                var freeSpinInfo = checkFreeSpinEvent(strUserID, isNewBet, message, bonus);
                int index = 0;
                int counter = 0;

                if (!_dicUserHistory.ContainsKey(strUserID))
                    _dicUserHistory.Add(strUserID, new BasePPHistory());

                if (_dicUserHistory[strUserID].log.Count == 0)
                    _dicUserHistory[strUserID].init = genInitResponse(strUserID, userBalance, true, currency, moneyMode, bonus);

                await spinGame(strUserID, companyID, bonus, userBalance, index, counter, isNewBet, freeSpinInfo, bonus);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::onDoSpin GameID: {0}, {1}", _gameID, ex);
            }
        }
        protected virtual void onDoCollect(string strUserID, GITMessage message, double userBalance, UserBonus bonus)
        {
            try
            {
                if (!_dicUserResultInfos.ContainsKey(strUserID) || !_dicUserBetInfos.ContainsKey(strUserID))
                    return;

                BasePPSlotBetInfo betInfo = _dicUserBetInfos[strUserID];
                BasePPSlotSpinResult result = _dicUserResultInfos[strUserID];
                //if (result.NextAction != ActionTypes.DOCOLLECT)
                //    return;

                int index = (int)message.Pop();
                int counter = (int)message.Pop();
                Dictionary<string, string> responseParams = new Dictionary<string, string>();
                responseParams.Add("balance", Math.Round(userBalance + _dicUserHistory[strUserID].win, 2).ToString());
                responseParams.Add("balance_cash", Math.Round(userBalance + _dicUserHistory[strUserID].win, 2).ToString());
                responseParams.Add("balance_bonus", "0.00");
                responseParams.Add("na", "s");
                responseParams.Add("stime", GameUtils.GetCurrentUnixTimestampMillis().ToString());
                responseParams.Add("sver", "5");
                responseParams.Add("index", index.ToString());
                responseParams.Add("counter", (counter + 1).ToString());

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOCOLLECT);
                string strCollectResponse = convertKeyValuesToString(responseParams);
                responseMessage.Append(strCollectResponse);

                addActionHistory(strUserID, "doCollect", strCollectResponse, index, counter);
                double totalBet = _dicUserHistory[strUserID].bet;
                double totalWin = _dicUserHistory[strUserID].win;
                string strDetailLog = saveHistory(strUserID, index, counter, userBalance);

                result.NextAction = ActionTypes.DOSPIN;

                PPFreeSpinInfo freeSpinInfo = null;
                if (_dicUserFreeSpinInfos.TryGetValue(strUserID, out freeSpinInfo))
                {
                    addFreeSpinBonusParams(responseMessage, freeSpinInfo, strUserID, 0.0);
                    checkFreeSpinCompletion(responseMessage, _dicUserFreeSpinInfos[strUserID], strUserID);
                }

                saveBetResultInfo(strUserID);
                var resultMsg = new ToUserResultMessage((int)_gameID, responseMessage, totalBet, totalWin, 0.0, new GameLogInfo(this.GameName, "0", strDetailLog), freeSpinInfo?.BonusID ?? 0, bonus); ;
                resultMsg.BetTransactionID = betInfo.BetTransactionID;
                resultMsg.RoundID = betInfo.RoundID;
                resultMsg.TransactionID = createTransactionID();

                Sender.Tell(resultMsg, Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPFreeSlotGame::onDoCollect {0}", ex);
            }
        }

        protected virtual void onDoCollectBonus(string strUserID, GITMessage message, double userBalance, UserBonus bonus)
        {
            try
            {
                int index = 0;
                int counter = 0;

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOCOLLECTBONUS);
                ToUserResultMessage resultMessage = null;

                if (!_dicUserResultInfos.ContainsKey(strUserID) || !_dicUserBetInfos.ContainsKey(strUserID))
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BasePPSlotBetInfo betInfo = _dicUserBetInfos[strUserID];
                    BasePPSlotSpinResult result = _dicUserResultInfos[strUserID];

                    string strDetailLog = saveHistory(strUserID, index, counter, userBalance);
                    result.NextAction = ActionTypes.DOSPIN;
                    JObject jsonObject = Newtonsoft.Json.Linq.JObject.Parse(result.ResultString);
                    double initWinMony = Math.Round(double.Parse(jsonObject["content"]["win"].ToString()) / 100.0, 2);
                    double betMoney = 0;
                    double winMoney = 0;
                    if (result.BonusResultString != "")
                    {
                        jsonObject = Newtonsoft.Json.Linq.JObject.Parse(result.BonusResultString);
                        winMoney = Math.Round(double.Parse(jsonObject["content"]["win"].ToString()) / 100.0, 2) - initWinMony;
                    }

                    BasePPTakeWinData takewinData = new BasePPTakeWinData();
                    TakeWinContent takecontentData = new TakeWinContent();

                    var now = DateTime.Now;
                    var dateTimeStr = now.ToString("yyyy-MM-dd HH:mm:ss");

                    takewinData.dateTime = dateTimeStr;
                    takecontentData.balance = double.Parse(jsonObject["content"]["balance"].ToString());
                    takewinData.content = takecontentData;
                    string takeWinString = JsonConvert.SerializeObject(takewinData);
                    responseMessage.Append(takeWinString);
                    addActionHistory(strUserID, "doCollectBonus", takeWinString, index, counter);

                    resultMessage = new ToUserResultMessage((int)_gameID, responseMessage, betMoney, winMoney, 0.0, new GameLogInfo(GameName, "0", strDetailLog), 0, bonus);
                    resultMessage.RoundID = betInfo.RoundID;
                    resultMessage.BetTransactionID = betInfo.BetTransactionID;
                    resultMessage.TransactionID = createTransactionID();

                    saveBetResultInfo(strUserID);

                    if (resultMessage == null)
                        Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
                    else
                        Sender.Tell(resultMessage, Self);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::onDoBonus {0}", ex);
            }
        }
        protected virtual async Task onDoBonus(int companyID, string strUserID, GITMessage message, double userBalance, UserBonus bonus)
        {
            try
            {
                string color = (string)message.Pop();

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                if (!_dicUserResultInfos.ContainsKey(strUserID) || !_dicUserBetInfos.ContainsKey(strUserID))
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BasePPSlotSpinResult result = _dicUserResultInfos[strUserID];
                    BasePPSlotBetInfo betInfo = _dicUserBetInfos[strUserID];

                    string jsonString = result.ResultString;
                    JObject jsonObject = Newtonsoft.Json.Linq.JObject.Parse(jsonString);
                    Random random = new Random();
                    int typeindex = random.Next(0, 2);

                    List<string> redCardType = new List<string>() { "heart", "diamond", "red" };
                    List<string> blackCardType = new List<string>() { "spade", "club", "black" };

                    if (checkCompanyPayoutRate(companyID, 0, result.LastTotalWin))
                    {
                        string tmp = "";
                        bool isred = false;
                        bool isblack = false;

                        //choose random card
                        if (typeindex == 0)
                        {//random choose red
                            tmp = redCardType[random.Next(0, 2)];
                            isred = true;
                        }
                        else
                        {//random choose black
                            tmp = blackCardType[random.Next(0, 2)];
                            isblack = true;
                        }
                        result.Card = tmp;

                        if (redCardType.Contains(color))
                        {
                            if (isred)
                            {
                                if (color == "red")
                                {
                                    result.LastTotalWin *= 2;
                                }
                                else if (color == tmp)
                                {
                                    result.LastTotalWin *= 4;
                                }
                                else
                                {
                                    result.LastTotalWin = 0;
                                }
                            }
                            else
                            {
                                result.LastTotalWin = 0;
                            }
                        }
                        if (blackCardType.Contains(color))
                        {
                            if (isblack)
                            {
                                if (color == "black")
                                {
                                    result.LastTotalWin *= 2;
                                }
                                else if(color == tmp)
                                {
                                    result.LastTotalWin *= 4;
                                }
                                else
                                {
                                    result.LastTotalWin = 0;
                                }
                            }
                            else
                            {
                                result.LastTotalWin = 0;
                            }
                        }
                        result.Round++;
                    }
                    else
                    {
                        if (redCardType.Contains(color))
                        {
                            result.Card = blackCardType[typeindex];
                        }
                        else
                        {
                            result.Card = redCardType[typeindex];
                        }
                        result.LastTotalWin = 0;
                        result.Round = 0;
                    }
                    result.LastCard.Add(result.Card);

                    var now = DateTime.Now;
                    var dateTimeStr = now.ToString("yyyy-MM-dd HH:mm:ss");
                    BasePPGambleData gambleData = new BasePPGambleData();
                    gambleContent gamblecont = new gambleContent();
                    gambleData.dateTime = dateTimeStr;
                    gamblecont.balance = Math.Round((userBalance - result.TotalWin + result.LastTotalWin) * 100, 2);
                    if (result.LastTotalWin == 0)
                    {
                        gamblecont.balance = userBalance * 100 - double.Parse(jsonObject["content"]["win"].ToString());
                    }
                    gamblecont.card = result.Card;
                    gamblecont.win = result.LastTotalWin * 100;
                    gamblecont.actionNext = result.TotalWin > 0 ? "gamble/takeWin" : "spin";
                    gamblecont.lastCard = result.LastCard.AsEnumerable().Reverse().ToList();
                    gamblecont.round = result.Round;
                    gambleData.content = gamblecont;

                    string gambleString = JsonConvert.SerializeObject(gambleData);
                    result.BonusResultString = gambleString;

                    ActionTypes nextAction = result.LastTotalWin > 0 ? ActionTypes.DOBONUS : ActionTypes.DOSPIN;
                    string strResponse = gambleString;

                    responseMessage.Append(strResponse);

                    if (_dicUserHistory.ContainsKey(strUserID) && _dicUserHistory[strUserID].log.Count > 0)
                        addActionHistory(strUserID, "doBonus", strResponse, 0, result.Round);

                    result.NextAction = nextAction;

                    saveBetResultInfo(strUserID);

                    if(result.LastTotalWin == 0)
                    {
                        double winmoney = Math.Round(double.Parse(jsonObject["content"]["win"].ToString()) / 100.0, 2);
                        ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, responseMessage, 0.0, 0.0 - winmoney, 0.0, new GameLogInfo(GameName, "0", strResponse), 0, bonus);
                        toUserResult.RoundID = betInfo.RoundID;
                        toUserResult.BetTransactionID = betInfo.BetTransactionID;
                        toUserResult.TransactionID = createTransactionID();
                        Sender.Tell(toUserResult, Self);
                        return;
                    }
                }
                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
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
        protected void onDoUndoUserSpin(string strUserID)
        {
            undoUserResultInfo(strUserID);
            undoUserHistory(strUserID);
            undoUserBetInfo(strUserID);
            saveBetResultInfo(strUserID);
        }
        #endregion
        protected virtual void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                BasePPSlotBetInfo betInfo = new BasePPSlotBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();
                betInfo.Denomination = (float)message.Pop();

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in BasePPSlotGame::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }

                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1}", strUserID, betInfo.LineCount);
                    return;
                }

                BasePPSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strUserID, out oldBetInfo))
                {
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine = betInfo.BetPerLine;
                    oldBetInfo.LineCount = betInfo.LineCount;
                    oldBetInfo.Denomination = betInfo.Denomination;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::readBetInfoFromMessage {0}", ex);
            }
        }
        protected virtual void undoUserResultInfo(string strUserID)
        {
            try
            {
                if (!_dicUserLastBackupResultInfos.ContainsKey(strUserID))
                    return;

                BasePPSlotSpinResult lastResult = _dicUserLastBackupResultInfos[strUserID];
                if (lastResult == null)
                    _dicUserResultInfos.Remove(strUserID);
                else
                    _dicUserResultInfos[strUserID] = lastResult;
                _dicUserLastBackupResultInfos.Remove(strUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::undoUserResultInfo {0}", ex);
            }
        }
        protected virtual void undoUserHistory(string strUserID)
        {
            try
            {
                if (!_dicUserLastBackupHistory.ContainsKey(strUserID))
                    return;

                byte[] userHistoryBytes = _dicUserLastBackupHistory[strUserID];
                if (userHistoryBytes == null)
                {
                    _dicUserHistory.Remove(strUserID);
                }
                else
                {
                    using (MemoryStream ms = new MemoryStream(userHistoryBytes))
                    {
                        using (BinaryReader binaryReader = new BinaryReader(ms))
                        {
                            BasePPHistory history = restoreHistory(binaryReader);
                            _dicUserHistory[strUserID] = history;
                        }
                    }
                }
                _dicUserLastBackupHistory.Remove(strUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::undoUserHistory {0}", ex);
            }
        }
        protected virtual void undoUserBetInfo(string strUserID)
        {
            try
            {
                if (!_dicUserLastBackupBetInfos.ContainsKey(strUserID))
                    return;

                byte[] userBetInfoBytes = _dicUserLastBackupBetInfos[strUserID];
                using (MemoryStream ms = new MemoryStream(userBetInfoBytes))
                {
                    using (BinaryReader binaryReader = new BinaryReader(ms))
                    {
                        BasePPSlotBetInfo betInfo = restoreBetInfo(strUserID, binaryReader);
                        _dicUserBetInfos[strUserID] = betInfo;
                    }
                }
                _dicUserLastBackupBetInfos.Remove(strUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::undoUserBetInfo {0}", ex);
            }
        }
        protected void addActionHistory(string strUserID, string strAction, string strResponse, int index, int counter)
        {
            if (!_dicUserHistory.ContainsKey(strUserID) || _dicUserHistory[strUserID].log.Count == 0)
                return;

            if (_dicUserHistory[strUserID].bet == 0.0)
                return;

            BasePPHistoryItem item = new BasePPHistoryItem();
            item.cr = string.Format("symbol={0}&repeat=0&action={3}&index={1}&counter={2}", SymbolName, index, counter, strAction);
            item.sr = strResponse;
            _dicUserHistory[strUserID].log.Add(item);
        }
        protected string convertTo100x(string strInitString)
        {
            var dicParams = splitResponseToParams(strInitString);
            if (dicParams.ContainsKey("sc") && !string.IsNullOrEmpty(dicParams["sc"]))
            {
                string[] strParts = dicParams["sc"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strParts.Length; i++)
                    strParts[i] = Math.Round(double.Parse(strParts[i]) * 100.0, 2).ToString();
                dicParams["sc"] = string.Join(",", strParts);
            }
            if (dicParams.ContainsKey("defc") && !string.IsNullOrEmpty(dicParams["defc"]))
                dicParams["defc"] = Math.Round(double.Parse(dicParams["defc"]) * 100.0, 2).ToString();

            if (dicParams.ContainsKey("total_bet_min") && !string.IsNullOrEmpty(dicParams["total_bet_min"]))
                dicParams["total_bet_min"] = Math.Round(double.Parse(dicParams["total_bet_min"]) * 100.0, 2).ToString();

            if (dicParams.ContainsKey("total_bet_max") && !string.IsNullOrEmpty(dicParams["total_bet_max"]))
                dicParams["total_bet_max"] = Math.Round(double.Parse(dicParams["total_bet_max"]) * 100.0, 2).ToString();

            return convertKeyValuesToString(dicParams);
        }

        protected virtual string genInitResponse(string strUserID, double userBalance, bool useDefault, Currencies currency, int moneyMode, UserBonus userBonus)
        {
            string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
            if(this.SymbolName == "dragonemperor")
            {
                var dicParams = splitResponseToParams(strInitString.Trim('&'));
                dicParams["AB"] = (userBalance * 100).ToString();
                dicParams["B"] = (userBalance * 100).ToString();
                var resStr = convertKeyValuesToString(dicParams);
                return $"&{resStr}&";
            }
            else
            {
                var jsonObject = Newtonsoft.Json.Linq.JObject.Parse(strInitString);
                var now = DateTime.Now;
                var dateTimeStr = now.ToString("yyyy-MM-dd HH:mm:ss");
                if (!useDefault && _dicUserBetInfos.ContainsKey(strUserID) && _dicUserResultInfos.ContainsKey(strUserID))
                {
                    var jsonObjectOri = Newtonsoft.Json.Linq.JObject.Parse(_dicUserResultInfos[strUserID].ResultString);
                    jsonObject["content"]["symbols"] = jsonObjectOri["content"]["symbols"];
                    bool isBetInfo = false;
                    if (jsonObjectOri["content"]?["freeSpin"]?.ToString() != "[]")
                    {
                        if (jsonObject["content"] is JObject contentObj)
                        {
                            contentObj["freeSpin"] = jsonObjectOri["content"]?["freeSpin"];
                            contentObj["restore"] = true;
                            userBalance = Math.Round(double.Parse(contentObj["balance"].ToString()) / 100.0, 2);
                        }
                        
                        isBetInfo = true;
                    }

                    if (jsonObjectOri["content"]?["bonus"]?.ToString() != null)
                    {
                        if (jsonObject["content"] is JObject contentObj)
                        {
                            contentObj["bonus"] = jsonObjectOri["content"]?["bonus"];
                            contentObj["restore"] = true;
                        }
                        isBetInfo = true;
                    }

                    if (isBetInfo)
                    {
                        jsonObject["content"]["betInfo"]["bet"] = _dicUserBetInfos[strUserID].BetPerLine;
                        jsonObject["content"]["betInfo"]["lines"] = _dicUserBetInfos[strUserID].LineCount;
                    }
                }

                jsonObject["dateTime"] = dateTimeStr;
                jsonObject["content"]["balance"] = userBalance * 100;
                return jsonObject.ToString();
            }
        }
        #region Freespin Functions
        private long getUnixTimestamp(DateTime time)
        {
            long timestamp = (long)time.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            return timestamp;
        }
        protected void getMinMaxChip(string sc, ref double minChip, ref double maxChip)
        {
            string[] chips = sc.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            minChip = double.Parse(chips[0]);
            maxChip = double.Parse(chips[chips.Length - 1]);
        }
        protected PPFreeSpinInfo checkFreeSpinEvent(string strGlobalUserID, bool isNewBet, GITMessage message, UserBonus userBonus)
        {
            PPFreeSpinInfo freeSpinInfo = null;
            if (_dicUserFreeSpinInfos.TryGetValue(strGlobalUserID, out freeSpinInfo))
            {
                if (isNewBet)
                {
                    if (freeSpinInfo.Pending)
                    {
                        if (message.FreeSpinPlayLater)
                        {
                            freeSpinInfo = null;
                            _dicUserFreeSpinInfos.Remove(strGlobalUserID);
                        }
                        else
                        {
                            //if (userBonus != null && (userBonus is UserFreeSpinBonus))
                            //{
                            UserFreeSpinBonus fromUser = (UserFreeSpinBonus)userBonus;
                            //if (fromUser.BonusID == freeSpinInfo.BonusID)
                            //{
                            freeSpinInfo.Pending = false;
                            var freeBetInfo = new BasePPSlotBetInfo();
                            freeBetInfo.BetPerLine = (float)Math.Round(freeSpinInfo.BetPerLine, 2);
                            freeBetInfo.LineCount = this.ClientReqLineCount;
                            _dicUserBetInfos[strGlobalUserID] = freeBetInfo;
                            _isRewardedBonus = true;
                            //}
                            //else
                            //{
                            //    freeSpinInfo = null;
                            //    _dicUserFreeSpinInfos.Remove(strGlobalUserID);
                            //}                        
                        }
                    }
                }
                else
                {
                    if (freeSpinInfo.Pending)
                        freeSpinInfo = null;
                }
            }
            return freeSpinInfo;
        }
        protected virtual void addFreeSpinBonusParams(GITMessage message, PPFreeSpinInfo freeSpinInfo, string strGlobalUserID, double win)
        {
            if (freeSpinInfo == null)
                return;

            freeSpinInfo.TotalWin += win;

            string strResultMsg = (string)message.Pop();
            var dicParams = splitResponseToParams(strResultMsg);
            dicParams["fra"] = Math.Round(freeSpinInfo.TotalWin, 2).ToString();
            dicParams["frn"] = freeSpinInfo.RemainCount.ToString();
            dicParams["frt"] = "N";
            message.Append(convertKeyValuesToString(dicParams));
        }
        protected virtual void checkFreeSpinCompletion(GITMessage message, PPFreeSpinInfo freeSpinInfo, string strGlobalUserID)
        {
            if (freeSpinInfo == null || freeSpinInfo.RemainCount > 0)
                return;

            string strResultMsg = (string)message.Pop();
            var dicParams = splitResponseToParams(strResultMsg);

            dicParams["ev"] = string.Format("FR1~{0},{1},{2},,", Math.Round(freeSpinInfo.BetPerLine, 2), ServerResLineCount, Math.Round(freeSpinInfo.TotalWin, 2));
            message.Append(convertKeyValuesToString(dicParams));
            //_dbWriter.Tell(new PPGameHistoryDBItem(strUserID, (int)_gameID, _dicUserHistory[strUserID].bet, _dicUserHistory[strUserID].baseBet, _dicUserHistory[strUserID].win, _dicUserHistory[strUserID].rtp,
            //               strDetailLog, GameUtils.GetCurrentUnixTimestampMillis()));
            _dbWriter.Tell(new PPFreeSpinReportDBItem(freeSpinInfo.BonusID, freeSpinInfo.AgentID, strGlobalUserID, freeSpinInfo.Currency, 0,
                freeSpinInfo.TotalWin, freeSpinInfo.AwardedCount, freeSpinInfo.RemainCount, GameName, DateTime.UtcNow, DateTime.UtcNow));
            _dicUserFreeSpinInfos.Remove(strGlobalUserID);
        }
        protected byte[] backupFreespinInfo(string strGlobalUserID)
        {
            if (!_dicUserFreeSpinInfos.ContainsKey(strGlobalUserID))
                return null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    _dicUserFreeSpinInfos[strGlobalUserID].SerializeTo(writer);
                }
                return ms.ToArray();
            }
        }
        #endregion
        protected string makeDefaultSpinResultString(double userBalance, string initString)
        {
            Dictionary<string, string> dicParams = new Dictionary<string, string>();
            setupDefaultResultParams(dicParams, userBalance, initString);
            return convertKeyValuesToString(dicParams);
        }
        protected virtual void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, string initString)
        {
            Dictionary<string, string> dicInitParams = splitResponseToParams(initString);
            dicParams.Add("balance", Math.Round(userBalance, 2).ToString());
            dicParams.Add("balance_cash", Math.Round(userBalance, 2).ToString());
            dicParams.Add("balance_bonus", "0.00");
            dicParams.Add("na", "s");
            dicParams.Add("stime", GameUtils.GetCurrentUnixTimestampMillis().ToString());
            dicParams.Add("sver", "5");
            dicParams.Add("sh", ROWS.ToString());
            if (dicInitParams.ContainsKey("def_sa"))
                dicParams.Add("sa", dicInitParams["def_sa"]);
            if (dicInitParams.ContainsKey("def_sb"))
                dicParams.Add("sb", dicInitParams["def_sb"]);
            if (dicInitParams.ContainsKey("def_s"))
                dicParams.Add("s", dicInitParams["def_s"]);
            dicParams.Add("c", dicInitParams["defc"]);
            dicParams.Add("l", ServerResLineCount.ToString());
        }
        protected virtual void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {

        }
        protected virtual void overrideResult(BasePPSlotBetInfo betInfo, BasePPSlotSpinResult result, int companyID)
        {

        }
        protected virtual string makeSpinResultString(BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult, double betMoney, double userBalance, bool isInit)
        {
            if (spinResult.ResultString.Contains("&AB="))
            {
                var dicparam = splitResponseToParams(spinResult.ResultString.Trim('&'));
                if(dicparam.ContainsKey("MSGID") && dicparam["MSGID"] == "FREE_GAME")
                {

                }
                dicparam["AB"] = ((userBalance - (isInit ? 0.0 : betMoney)) * 100).ToString();
                dicparam["B"] = ((userBalance - (isInit ? 0.0 : betMoney) + spinResult.TotalWin) * 100).ToString();

                var retStr = convertKeyValuesToString(dicparam);
                return $@"&{retStr}&";
            }
            else
            {
                JObject jsonObject = Newtonsoft.Json.Linq.JObject.Parse(spinResult.ResultString);
                Dictionary<string, string> dicParams = splitResponseToParams(spinResult.ResultString);

                if (spinResult.HasBonusResult)
                {
                    Dictionary<string, string> dicBonusParams = splitResponseToParams(spinResult.BonusResultString);
                    dicParams = mergeSpinToBonus(dicParams, dicBonusParams);
                }

                var now = DateTime.Now;
                var dateTimeStr = now.ToString("yyyy-MM-dd HH:mm:ss");
                jsonObject["dateTime"] = dateTimeStr;
                jsonObject["content"]["balance"] = ((userBalance - (isInit ? 0.0 : betMoney) + spinResult.LastTotalWin) * 100).ToString();

                if (isInit)
                    supplementInitResult(dicParams, betInfo, spinResult);

                //overrideSomeParams(betInfo, dicParams);
                return jsonObject.ToString();
            }
            
        }

        protected virtual void supplementInitResult(Dictionary<string, string> dicParams, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {

        }
        protected virtual Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
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
            if (dicParams.ContainsKey("tw"))
                dicParams["tw"] = convertWinByBet(dicParams["tw"], currentBet);

            if (dicParams.ContainsKey("w"))
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

            if (dicParams.ContainsKey("psym"))
            {
                string[] strParts = dicParams["psym"].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                if (strParts.Length == 3)
                {
                    strParts[1] = convertWinByBet(strParts[1], currentBet);
                    dicParams["psym"] = string.Join("~", strParts);
                }
            }

            int winLineID = 0;
            do
            {
                string strKey = string.Format("l{0}", winLineID);
                if (!dicParams.ContainsKey(strKey))
                    break;

                string[] strParts = dicParams[strKey].Split(new string[] { "~" }, StringSplitOptions.None);
                if (strParts.Length >= 2)
                {
                    strParts[1] = convertWinByBet(strParts[1], currentBet);
                    dicParams[strKey] = string.Join("~", strParts);
                }
                winLineID++;
            } while (true);
        }
        protected virtual BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst, PPFreeSpinInfo freeSpinInfo)
        {
            try
            {
                BasePPSlotSpinResult spinResult = new BasePPSlotSpinResult();
                Dictionary<string, string> dicParams = new Dictionary<string, string>();
                if (strSpinResponse.Contains("&AB="))
                {
                    dicParams = splitResponseToParams(strSpinResponse.Trim('&'));
                    spinResult.NextAction = ActionTypes.DOSPIN;
                    spinResult.TotalWin = Math.Round(double.Parse(dicParams["TW"]) * betInfo.BetPerLine / 100.0, 2);
                    spinResult.LastTotalWin = Math.Round(double.Parse(dicParams["TW"]) * betInfo.BetPerLine / 100.0, 2);
                    dicParams["TW"] = (double.Parse(dicParams["TW"]) * betInfo.BetPerLine).ToString();
                    if (dicParams.ContainsKey("CW"))
                    {
                        dicParams["CW"] = (double.Parse(dicParams["CW"]) * betInfo.BetPerLine).ToString();
                    }

                    if (dicParams.ContainsKey("WS"))
                    {
                        string[] wsparams = dicParams["WS"].Trim(new char[] { '|' }).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                        List<string> updatedwsparams = new List<string>();

                        foreach (var wsparam in wsparams)
                        {
                            string[] wsparts = wsparam.Trim(new char[] { ';' }).Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                            if (wsparts.Length > 1 && double.TryParse(wsparts[1], out double value))
                            {
                                value *= betInfo.BetPerLine;
                                wsparts[1] = value.ToString();
                            }
                            updatedwsparams.Add(string.Join(";", wsparts) + ";");
                        }
                        dicParams["WS"] = string.Join("|", updatedwsparams) + "|";
                    }
                    
                    spinResult.ResultString = "&" + convertKeyValuesToString(dicParams) + "&";

                    return spinResult;
                }
                else
                {
                    JObject jsonObject = Newtonsoft.Json.Linq.JObject.Parse(strSpinResponse);

                    spinResult.NextAction = ActionTypes.DOSPIN;
                    spinResult.TotalWin = Math.Round(double.Parse(jsonObject["content"]["win"].ToString()) * betInfo.BetPerLine / 100.0, 2);
                    spinResult.LastTotalWin = Math.Round(double.Parse(jsonObject["content"]["win"].ToString()) * betInfo.BetPerLine / 100.0, 2);

                    jsonObject["content"]["win"] = double.Parse(jsonObject["content"]["win"].ToString()) * betInfo.BetPerLine;

                    if (jsonObject["content"]["freeSpin"] != null && jsonObject["content"]["freeSpin"].ToString() != "[]")
                    {
                        spinResult.LastTotalWin = Math.Round(double.Parse(jsonObject["content"]["freeSpin"]["totalWin"].ToString()) * betInfo.BetPerLine / 100.0, 2);
                        jsonObject["content"]["freeSpin"]["totalWin"] = double.Parse(jsonObject["content"]["freeSpin"]["totalWin"].ToString()) * betInfo.BetPerLine;
                    }
                    if (jsonObject["content"] is JObject winlinesObj && winlinesObj.ContainsKey("winLines"))
                    {
                        if (jsonObject["content"]["winLines"].Count() > 0)
                        {
                            for (int i = 0; i < jsonObject["content"]["winLines"].Count(); i++)
                            {
                                jsonObject["content"]["winLines"][i]["win"] = double.Parse(jsonObject["content"]["winLines"][i]["win"].ToString()) * betInfo.BetPerLine;
                            }
                        }
                    }
                    if (jsonObject["content"]["symbols"] is JObject symbolsObj && symbolsObj.ContainsKey("coins"))
                    {
                        for (int c = 0; c < jsonObject["content"]["symbols"]["coins"].Count(); c++)
                        {
                            jsonObject["content"]["symbols"]["coins"][c][2] = double.Parse(jsonObject["content"]["symbols"]["coins"][c][2].ToString()) * betInfo.BetPerLine;
                        }
                    }
                    //if(jsonObject["content"] is JObject contentObj && contentObj.ContainsKey("reelSetNext"))
                    //{
                    //    if (jsonObject["content"]["reelSetNext"].ToString() == "h&s" && jsonObject["content"]["winLines"].Count() > 0)
                    //    {
                    //        for (int w = 0; w < jsonObject["content"]["winLines"][0]["elements"].Count(); w++)
                    //        {
                    //            jsonObject["content"]["winLines"][0]["elements"][w][2] = double.Parse(jsonObject["content"]["winLines"][0]["elements"][w][2].ToString()) * betInfo.BetPerLine;
                    //        }
                    //    }
                    //}

                    spinResult.ResultString = jsonObject.ToString();

                    return spinResult;
                }
            }
            catch (Exception ex)
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
            for (int i = 0; i < strParts.Length; i++)
            {
                string[] strParamValues = strParts[i].Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (strParamValues.Length == 2)
                    dicParamValues[strParamValues[0]] = strParamValues[1];
                else if (strParamValues.Length == 1)
                    dicParamValues[strParamValues[0]] = null;
            }
            return dicParamValues;
        }
        protected string convertKeyValuesToString(Dictionary<string, string> keyValues)
        {
            List<string> parts = new List<string>();
            foreach (KeyValuePair<string, string> pair in keyValues)
            {
                if (pair.Value == null)
                    parts.Add(string.Format("{0}=", pair.Key));
                else
                    parts.Add(string.Format("{0}={1}", pair.Key, pair.Value));
            }
            return string.Join("&", parts.ToArray());
        }
        protected ActionTypes convertStringToActionType(string strAction)
        {
            switch (strAction.Trim())
            {
                case "s":
                    return ActionTypes.DOSPIN;
                case "base":
                    return ActionTypes.DOSPIN;
                case "free":
                    return ActionTypes.DOSPIN;
                case "h&s":
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
                if(responseList[i - 1].Contains("&AB="))
                {
                    actionResponseList.Add(new BasePPActionToResponse(ActionTypes.DOSPIN, responseList[i]));
                }
                else
                {
                    var jsonObject = Newtonsoft.Json.Linq.JObject.Parse(responseList[i - 1]);
                    string checkNa = jsonObject["content"]?["reelSetNext"]?.ToString();
                    ActionTypes actionType = convertStringToActionType(checkNa != null ? jsonObject["content"]["reelSetNext"].ToString() : "s");
                    actionResponseList.Add(new BasePPActionToResponse(actionType, responseList[i]));
                }
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
        protected virtual async Task<BasePPSlotSpinResult> generateSpinResult(BasePPSlotBetInfo betInfo, string strUserID, int companyID, UserBonus userBonus, bool usePayLimit, PPFreeSpinInfo freeSpinInfo)
        {
            BasePPSlotSpinData spinData = null;
            BasePPSlotSpinResult result = null;
            _isRewardedCashback = false;
            if (betInfo.HasRemainResponse)
            {
                BasePPActionToResponse nextResponse = betInfo.pullRemainResponse();
                result = calculateResult(betInfo, nextResponse.Response, false, freeSpinInfo);

                if (!betInfo.HasRemainResponse)
                {
                    betInfo.RemainReponses = null;
                    _logger.Info($"generateSpinResult: {JsonConvert.SerializeObject(userBonus)}");
                }
                return result;
            }

            float totalBet = betInfo.TotalBet;
            double realBetMoney = totalBet;

            spinData = await selectRandomStop(companyID, totalBet, betInfo);

            double totalWin = totalBet * spinData.SpinOdd;

            if (!usePayLimit || checkCompanyPayoutRate(companyID, realBetMoney, totalWin))
            {
                result = calculateResult(betInfo, spinData.SpinStrings[0], true, freeSpinInfo);
                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);

                return result;
            }

            double emptyWin = 0.0;
            spinData = await selectEmptySpin(companyID, betInfo);
            result = calculateResult(betInfo, spinData.SpinStrings[0], true, freeSpinInfo);
            sumUpCompanyBetWin(companyID, realBetMoney, emptyWin);

            return result;
        }

        protected string getCurrencySymbol(string currency)
        {
            if (currency == "MYR")
                return "RM";
            else if (currency == "THB")
                return "฿";
            else if (currency == "MMK")
                return "K";
            else if (currency == "IDR")
                return "Rp";
            else if (currency == "BDT")
                return "৳";
            else if (currency == "INR")
                return "₹";
            else if (currency == "CNY")
                return "¥";
            return "$";
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
        protected byte[] backupHistoryInfo(string strUserID)
        {
            if (!_dicUserHistory.ContainsKey(strUserID))
                return null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    _dicUserHistory[strUserID].serializeTo(writer);
                }
                return ms.ToArray();
            }
        }
        protected virtual async Task spinGame(string strUserID, int companyID, UserBonus userBonus, double userBalance, int index, int counter, bool isNewBet, PPFreeSpinInfo freespinInfo, UserBonus bonus)
        {
            try
            {
                BasePPSlotBetInfo betInfo = null;
                if (!_dicUserBetInfos.TryGetValue(strUserID, out betInfo))
                    return;

                byte[] betInfoBytes = backupBetInfo(betInfo);
                byte[] historyBytes = backupHistoryInfo(strUserID);
                byte[] freeSpinBytes = backupFreespinInfo(strUserID);

                BasePPSlotSpinResult lastResult = null;
                if (_dicUserResultInfos.ContainsKey(strUserID))
                    lastResult = _dicUserResultInfos[strUserID];

                if (lastResult != null && lastResult.NextAction != ActionTypes.DOSPIN)
                {
                    GITMessage message = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOSPIN);
                    message.Append(string.Format("balance={0}&balance_cash={0}&balance_bonus=0.0&frozen=Internal+server+error.+The+game+will+be+restarted.+&msg_code=11&ext_code=SystemError", Math.Round(userBalance, 2)));
                    ToUserMessage toUserResult = new ToUserMessage((int)_gameID, message);
                    Sender.Tell(toUserResult, Self);
                    return;
                }

                double betMoney = betInfo.TotalBet;
                if (betInfo.HasRemainResponse)
                    betMoney = 0.0;

                if (freespinInfo == null && userBalance.LT(betMoney, _epsilion) || betMoney < 0.0)
                {
                    _logger.Error("user balance is less than bet money in BasePPSlotGame::spinGame {0} balance:{1}, bet money: {2} game id:{3}",
                        strUserID, userBalance, betMoney, _gameID);
                    return;
                }

                if (isNewBet)
                {

                    if (freespinInfo != null)
                        freespinInfo.RemainCount -= 1;
                }

                BasePPSlotSpinResult spinResult = await this.generateSpinResult(betInfo, strUserID, companyID, userBonus, true, freespinInfo);
                overrideResult(betInfo, spinResult, companyID);

                string strGameLog = spinResult.ResultString;
                _dicUserResultInfos[strUserID] = spinResult;

                saveBetResultInfo(strUserID);

                sendGameResult(betInfo, spinResult, strUserID, betMoney, spinResult.WinMoney, strGameLog, userBalance, index, counter, freespinInfo, bonus);

                _dicUserLastBackupBetInfos[strUserID] = betInfoBytes;
                _dicUserLastBackupResultInfos[strUserID] = lastResult;
                _dicUserLastBackupHistory[strUserID] = historyBytes;
                _dicUserLastBackupFreespinInfos[strUserID] = freeSpinBytes;

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::spinGame {0}", ex);
            }
        }
        protected virtual void sendGameResult(BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult, string strUserID, double betMoney, double winMoney, string strGameLog, double userBalance, int index, int counter, PPFreeSpinInfo freeSpinInfo, UserBonus bonus)
        {
            //_logger.Info($"sendGameResult BetMoney: {betMoney},{winMoney}, {userBalance}");
            string strSpinResult = "";

            if (freeSpinInfo != null && !freeSpinInfo.Pending)
                strSpinResult = makeSpinResultString(betInfo, spinResult, 0.0, userBalance, false);
            else
                strSpinResult = makeSpinResultString(betInfo, spinResult, betMoney, userBalance, false);

            GITMessage message = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOSPIN);
            message.Append(strSpinResult);


            if (_dicUserHistory.ContainsKey(strUserID))
            {
                if (_dicUserHistory[strUserID].log.Count == 0)
                    _dicUserHistory[strUserID].bet = betMoney;
            }

            if (addSpinResultToHistory(strUserID, index, counter, strSpinResult, betInfo, spinResult))
            {

                string strDetailLog = saveHistory(strUserID, index, counter, freeSpinInfo != null && !freeSpinInfo.Pending ? userBalance : userBalance - betMoney);
                ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, message, betMoney, spinResult.LastWinMoney, betMoney, new GameLogInfo(GameName, "0", strDetailLog), freeSpinInfo?.BonusID ?? 0, bonus);
                toUserResult.RoundID = betInfo.RoundID;
                toUserResult.BetTransactionID = betInfo.BetTransactionID;
                toUserResult.TransactionID = createTransactionID();
                if (freeSpinInfo != null && !freeSpinInfo.Pending)
                    checkFreeSpinCompletion(message, freeSpinInfo, strUserID);
                Sender.Tell(toUserResult);

            }
            else
            {
                ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, message, betMoney, 0.0, betMoney, new GameLogInfo(GameName, "0", strGameLog), freeSpinInfo?.BonusID ?? 0, bonus);
                toUserResult.RoundID = betInfo.RoundID;
                toUserResult.BetTransactionID = betInfo.BetTransactionID;
                if (freeSpinInfo != null && !freeSpinInfo.Pending)
                    addFreeSpinBonusParams(message, freeSpinInfo, strUserID, winMoney);
                Sender.Tell(toUserResult, Self);
            }
        }

        protected virtual string saveHistory(string strUserID, int index, int counter, double userBalance)
        {
            string strHistoryDetail = "";
            if (_dicUserHistory.ContainsKey(strUserID) && _dicUserHistory[strUserID].log.Count > 0 && _dicUserHistory[strUserID].bet > 0.0)
            {
                if (SupportReplay)
                {
                    string strDetailLog = JsonConvert.SerializeObject(_dicUserHistory[strUserID]);
                    _dicUserHistory[strUserID].rtp = _dicUserHistory[strUserID].win / _dicUserHistory[strUserID].baseBet;
                    if (_dicUserHistory[strUserID].rtp > 1.0)
                    {
                        _dbWriter.Tell(new PPGameHistoryDBItem(strUserID, (int)_gameID, _dicUserHistory[strUserID].bet, _dicUserHistory[strUserID].baseBet, _dicUserHistory[strUserID].win, _dicUserHistory[strUserID].rtp,
                            strDetailLog, GameUtils.GetCurrentUnixTimestampMillis()));
                    }
                }
                strHistoryDetail = JsonConvert.SerializeObject(_dicUserHistory[strUserID].log);
                _dbWriter.Tell(new PPGameRecentHistoryDBItem(strUserID, (int)_gameID, userBalance + _dicUserHistory[strUserID].win, _dicUserHistory[strUserID].bet, _dicUserHistory[strUserID].win, "", strHistoryDetail, GameUtils.GetCurrentUnixTimestampMillis()));

            }
            _dicUserHistory.Remove(strUserID);
            return strHistoryDetail;
        }
        protected virtual bool addSpinResultToHistory(string strUserID, int index, int counter, string strSpinResult, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {
            if (!_dicUserHistory.ContainsKey(strUserID))
                return false;

            BasePPHistoryItem historyItem = new BasePPHistoryItem();
            historyItem.cr = string.Format("symbol={0}&c={1}&repeat=0&action=doSpin&index={2}&counter={3}&l={4}", SymbolName, betInfo.BetPerLine, index, counter, ClientReqLineCount);
            if (SupportPurchaseFree && betInfo.PurchaseFree)
                historyItem.cr += "&pur=0";
            if (SupportMoreBet)
            {
                if (betInfo.MoreBet)
                    historyItem.cr += "&bl=1";
                else
                    historyItem.cr += "&bl=0";
            }
            historyItem.sr = strSpinResult;

            _dicUserHistory[strUserID].log.Add(historyItem);
            if (betInfo.HasRemainResponse)
                return false;

            _dicUserHistory[strUserID].baseBet = betInfo.TotalBet;
            _dicUserHistory[strUserID].win = spinResult.TotalWin;

            if (spinResult.NextAction == ActionTypes.DOSPIN)
                return true;

            return false;
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
        protected virtual async Task<BasePPSlotSpinData> selectRandomStop(int companyID, BasePPSlotBetInfo betInfo, bool isMoreBet)
        {
            OddAndIDData selectedOddAndID = selectRandomOddAndID(companyID, betInfo, isMoreBet);
            return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(selectedOddAndID.ID), TimeSpan.FromSeconds(10.0));
        }
        protected virtual OddAndIDData selectRandomOddAndID(int companyID, BasePPSlotBetInfo betInfo, bool isMoreBet)
        {
            double payoutRate = _config.PayoutRate;
            if (_companyPayoutRates.ContainsKey(companyID))
                payoutRate = _companyPayoutRates[companyID];

            double randomDouble = Pcg.Default.NextDouble(0.0, 100.0);

            double selectedOdd = 0.0;
            if (randomDouble >= payoutRate || payoutRate == 0.0)
            {
                selectedOdd = 0.0;
            }
            else
            {
                selectedOdd = selectOddFromProbs(_naturalSpinOddProbs, _naturalSpinCount);
            }

            if (!_totalSpinOddIds.ContainsKey(selectedOdd))
                return null;
            int selectedID = _totalSpinOddIds[selectedOdd][Pcg.Default.Next(0, _totalSpinOddIds[selectedOdd].Length)];
            OddAndIDData selectedOddAndID = new OddAndIDData();
            selectedOddAndID.ID = selectedID;
            selectedOddAndID.Odd = selectedOdd;
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
        protected virtual async Task<BasePPSlotSpinData> selectEmptySpin(int companyID, BasePPSlotBetInfo betInfo)
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
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected virtual async Task<BasePPSlotSpinData> selectMinStartFreeSpinData(BasePPSlotBetInfo betInfo)
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

        protected virtual async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int companyID, BasePPSlotBetInfo betInfo, double baseBet)
        {
            double payoutRate = _config.PayoutRate;
            if (_companyPayoutRates.ContainsKey(companyID))
                payoutRate = _companyPayoutRates[companyID];

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
        public virtual async Task<BasePPSlotSpinData> selectRandomStop(int companyID, double baseBet, BasePPSlotBetInfo betInfo)
        {
            //if (this.SupportPurchaseFree && betInfo.PurchaseFree)
            //    return await selectPurchaseFreeSpin(companyID, betInfo, baseBet);

            //if (SupportMoreBet && betInfo.MoreBet)
            //    return await selectRandomStop(companyID, betInfo, true);
            //else
                return await selectRandomStop(companyID, betInfo, false);
        }
        protected override async Task<bool> loadUserHistoricalData(string strUserID, bool isNewEnter)
        {
            try
            {
                string strKey = string.Format("{0}_{1}", strUserID, _gameID);
                byte[] betInfoData = await RedisDatabase.RedisCache.StringGetAsync(strKey);
                if (betInfoData != null)
                {
                    using (var stream = new MemoryStream(betInfoData))
                    {
                        BinaryReader reader = new BinaryReader(stream);
                        BasePPSlotBetInfo betInfo = restoreBetInfo(strUserID, reader);
                        if (betInfo != null)
                            _dicUserBetInfos[strUserID] = betInfo;
                    }
                }

                strKey = string.Format("{0}_{1}_result", strUserID, _gameID);
                byte[] resultInfoData = await RedisDatabase.RedisCache.StringGetAsync(strKey);
                if (resultInfoData != null)
                {
                    using (var stream = new MemoryStream(resultInfoData))
                    {
                        BinaryReader reader = new BinaryReader(stream);
                        BasePPSlotSpinResult resultInfo = restoreResultInfo(strUserID, reader);
                        if (resultInfo != null)
                            _dicUserResultInfos[strUserID] = resultInfo;
                    }
                }

                strKey = string.Format("{0}_{1}_freespin", strUserID, (int)_gameID);
                byte[] freeSpinInfoData = await RedisDatabase.RedisCache.StringGetAsync(strKey);
                if (freeSpinInfoData != null)
                {
                    using (var stream = new MemoryStream(freeSpinInfoData))
                    {
                        BinaryReader reader = new BinaryReader(stream);
                        PPFreeSpinInfo freeSpinInfo = restoreFreeSpinInfo(strUserID, reader);
                        if (freeSpinInfo != null)
                            _dicUserFreeSpinInfos[strUserID] = freeSpinInfo;
                    }
                }

                strKey = string.Format("{0}_{1}_history", strUserID, _gameID);
                byte[] historyInfoData = await RedisDatabase.RedisCache.StringGetAsync(strKey);
                if (historyInfoData != null)
                {
                    using (var stream = new MemoryStream(historyInfoData))
                    {
                        BinaryReader reader = new BinaryReader(stream);
                        BasePPHistory userHistory = restoreHistory(reader);
                        if (userHistory != null)
                            _dicUserHistory[strUserID] = userHistory;
                    }
                }
                strKey = string.Format("{0}_{1}_setting", strUserID, _gameID);
                string strSetting = await RedisDatabase.RedisCache.StringGetAsync(strKey);
                if (strSetting != null)
                    _dicUserSettings[strUserID] = strSetting;
            }

            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::loadUserHistoricalData {0}", ex);
                return false;
            }
            return await base.loadUserHistoricalData(strUserID, isNewEnter);
        }
        protected virtual BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
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
        protected virtual BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            BasePPSlotSpinResult result = new BasePPSlotSpinResult();
            result.SerializeFrom(reader);
            return result;
        }
        protected virtual PPFreeSpinInfo restoreFreeSpinInfo(string strGlobalUserID, BinaryReader reader)
        {
            PPFreeSpinInfo freeSpinInfo = new PPFreeSpinInfo();
            freeSpinInfo.SerializeFrom(reader);
            return freeSpinInfo;
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
            var serializer = new JsonSerializer();
            var stringWriter = new StringWriter();
            using (var writer = new JsonTextWriter(stringWriter))
            {
                writer.QuoteName = false;
                serializer.Serialize(writer, token);
            }
            return stringWriter.ToString();
        }
        protected void addIndActionHistory(string strUserID, string strAction, string strResponse, int index, int counter, int ind)
        {
            if (!_dicUserHistory.ContainsKey(strUserID) || _dicUserHistory[strUserID].log.Count == 0)
                return;

            if (_dicUserHistory[strUserID].bet == 0.0)
                return;

            BasePPHistoryItem item = new BasePPHistoryItem();
            item.cr = string.Format("symbol={0}&repeat=0&action={3}&index={1}&counter={2}&ind={4}", SymbolName, index, counter, strAction, ind);
            item.sr = strResponse;
            _dicUserHistory[strUserID].log.Add(item);
        }

        protected virtual void saveBetResultInfo(string strUserID)
        {
            try
            {
                if (_dicUserBetInfos.ContainsKey(strUserID))
                {
                    byte[] betInfoBytes = _dicUserBetInfos[strUserID].convertToByte();
                    _redisWriter.Tell(new UserBetInfoWrite(strUserID, _gameID, betInfoBytes, false), Self);
                }
                if (_dicUserResultInfos.ContainsKey(strUserID))
                {
                    byte[] resultInfoBytes = _dicUserResultInfos[strUserID].convertToByte();
                    _redisWriter.Tell(new UserResultInfoWrite(strUserID, _gameID, resultInfoBytes, false), Self);
                }
                if (_dicUserFreeSpinInfos.ContainsKey(strUserID))
                {
                    byte[] freeSpinInfoBytes = _dicUserFreeSpinInfos[strUserID].convertToByte();
                    _redisWriter.Tell(new UserFreeSpinInfoWrite(strUserID, _gameID, freeSpinInfoBytes, false), Self);
                }
                else
                {
                    _redisWriter.Tell(new UserFreeSpinInfoWrite(strUserID, _gameID, null, false), Self);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::saveBetInfo {0}", ex);
            }
        }
        protected override async Task onUserExitGame(string strUserID, bool userRequested)
        {
            try
            {
                if (_dicUserBetInfos.ContainsKey(strUserID))
                {
                    byte[] betInfoBytes = _dicUserBetInfos[strUserID].convertToByte();
                    await _redisWriter.Ask(new UserBetInfoWrite(strUserID, _gameID, betInfoBytes, true));
                    _dicUserBetInfos.Remove(strUserID);
                }
                if (_dicUserResultInfos.ContainsKey(strUserID))
                {
                    byte[] resultInfoBytes = _dicUserResultInfos[strUserID].convertToByte();
                    await _redisWriter.Ask(new UserResultInfoWrite(strUserID, _gameID, resultInfoBytes, true));
                    _dicUserResultInfos.Remove(strUserID);
                }
                if (_dicUserHistory.ContainsKey(strUserID))
                {
                    byte[] historyByteData = _dicUserHistory[strUserID].convertToByte();
                    await _redisWriter.Ask(new UserHistoryWrite(strUserID, _gameID, historyByteData, true));
                    _dicUserHistory.Remove(strUserID);
                }
                else
                {
                    await _redisWriter.Ask(new UserHistoryWrite(strUserID, _gameID, null, true));
                }
                if (_dicUserSettings.ContainsKey(strUserID))
                {
                    string strSetting = _dicUserSettings[strUserID];
                    await _redisWriter.Ask(new UserSettingWrite(strUserID, _gameID, strSetting, true));
                    _dicUserSettings.Remove(strUserID);
                }
                _dicUserLastBackupResultInfos.Remove(strUserID);
                _dicUserLastBackupBetInfos.Remove(strUserID);
                _dicUserLastBackupHistory.Remove(strUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::onUserExitGame GameID:{0} {1}", _gameID, ex);
            }

            await base.onUserExitGame(strUserID, userRequested);
        }
    }

    public class OddAndIDData
    {
        public int ID { get; set; }
        public double Odd { get; set; }

        public OddAndIDData()
        {

        }
        public OddAndIDData(int id, double odd)
        {
            this.ID = id;
            this.Odd = odd;
        }
    }
}
