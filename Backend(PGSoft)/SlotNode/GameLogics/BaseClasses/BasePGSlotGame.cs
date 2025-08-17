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
using Akka.Util;
using MongoDB.Bson;
using Microsoft.Extensions.Logging;

namespace SlotGamesNode.GameLogics
{
    public class BasePGSlotGame : IGameLogicActor
    {
        protected IActorRef                         _spinDatabase           = null;        
        protected double                            _spinDataDefaultBet     = 0.0f;

        protected int                               _normalMaxID        = 0;
        protected int                               _naturalSpinCount   = 0;
        protected int                               _emptySpinCount     = 0;
        protected double                            _naturalRTP         = 0.0;

        protected double                            _totalFreeSpinWinRate = 0.0; 
        protected double                            _minFreeSpinWinRate   = 0.0; 

        protected int                               _anteBetMinusZeroCount = 0;


        protected Dictionary<string, BasePGSlotBetInfo>     _dicUserBetInfos                = new Dictionary<string, BasePGSlotBetInfo>();
        protected Dictionary<string, BasePGHistory>         _dicUserHistory                 = new Dictionary<string, BasePGHistory>();
        protected Dictionary<string, BasePGSlotSpinResult>  _dicUserResultInfos             = new Dictionary<string, BasePGSlotSpinResult>();


        protected Dictionary<string, BasePGSlotSpinResult>  _dicUserLastBackupResultInfos   = new Dictionary<string, BasePGSlotSpinResult>();
        protected Dictionary<string, byte[]>                _dicUserLastBackupBetInfos      = new Dictionary<string, byte[]>();
        protected Dictionary<string, byte[]>                _dicUserLastBackupHistory       = new Dictionary<string, byte[]>();

        protected PGGameConfig                              _pgGameConfig                   = new PGGameConfig();
        protected virtual bool HasPurEnableOption
        {
            get { return false; }
        }
        protected virtual string SymbolName
        {
            get
            {
                return "";
            }
        }
        protected virtual int BaseBet
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
        protected virtual bool SupportAnteBet
        {
            get { return false; }
        }
        protected virtual double AnteBetMultiple
        {
            get { return 0.0; }
        }
        protected virtual string DefaultResult
        {
            get
            {
                return "";
            }
        }
        protected virtual double DefaultBetSize
        {
            get { return 0.1; }
        }
        protected virtual int DefaultBetLevel
        {
            get { return 5;  }
        }
        protected virtual string GameRuleString
        {
            get { return "";  }
        }
        protected virtual string ErrorResultString
        {
            get { return ""; }
        }
        public BasePGSlotGame()
        {
        }
        protected override void LoadSetting()
        {
            base.LoadSetting();
            initGameData();
            if (SupportPurchaseFree && !HasPurEnableOption)
                _logger.Info(this.GameName);
        }

        protected virtual void initGameData()
        {
            _pgGameConfig.iuwe           = false;
            _pgGameConfig.inwe           = false;
            _pgGameConfig.mxl            = this.BaseBet;
            _pgGameConfig.fb.isSupported = this.SupportPurchaseFree;
            _pgGameConfig.fb.bm          = (int) this.PurchaseFreeMultiple;
            _pgGameConfig.fb.t           = 1200;

            _pgGameConfig.gcs.ab.isSupported    = this.SupportAnteBet;
            _pgGameConfig.gcs.ab.bm             = 0;
            if (this.SupportAnteBet)
            {
                _pgGameConfig.gcs.ab.bms = new List<double>() { this.AnteBetMultiple };
            }
            _pgGameConfig.gcs.ab.t              = 0;

            _pgGameConfig.wt.mw          = 5;
            _pgGameConfig.wt.bw          = 20;
            _pgGameConfig.wt.mgw         = 35;
            _pgGameConfig.wt.smgw        = 50;
        }
        protected override void onUserEnterGame(string strGlobalUserID, double balance, Currencies currency)
        {
            ChipsetManager.Instance.convertTo(currency, _gameID, _pgGameConfig);
            if (!_dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                dynamic defaultResult       = JsonConvert.DeserializeObject<dynamic>(this.DefaultResult);
                defaultResult["si"]["bl"]   = Math.Round(balance, 2);

                string strDefaultChipset    = ChipsetManager.Instance.getDefaultChipset(currency, _gameID);
                if(string.IsNullOrEmpty(strDefaultChipset))
                    defaultResult["si"]["cs"] = Math.Round(DefaultBetSize, 2);
                else
                    defaultResult["si"]["cs"] = Math.Round(double.Parse(strDefaultChipset), 2);

                int defaultBetLevel = ChipsetManager.Instance.getDefaultBetLevel(currency, _gameID);
                if(defaultBetLevel == 0)                       
                    defaultResult["si"]["ml"] = DefaultBetLevel;
                else
                    defaultResult["si"]["ml"] = defaultBetLevel;

                Sender.Tell(new EnterGameResponse(_gameID, Self, 0, JsonConvert.SerializeObject(_pgGameConfig), JsonConvert.SerializeObject(defaultResult)));
            }
            else
            {
                BasePGSlotSpinResult result = _dicUserResultInfos[strGlobalUserID];
                var dt = new { si = JsonConvert.DeserializeObject<dynamic>(result.ResultString) };
                Sender.Tell(new EnterGameResponse(_gameID, Self, 0, JsonConvert.SerializeObject(_pgGameConfig), JsonConvert.SerializeObject(dt)));
            }
        }
        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBet = (double)  infoDocument["defaultbet"];
                _normalMaxID        = (int)     infoDocument["normalmaxid"];
                _emptySpinCount     = (int)     infoDocument["emptycount"];
                _naturalSpinCount   = (int)     infoDocument["normalselectcount"];
                _naturalRTP         = (double)  infoDocument["normalrtp"];

                if (SupportPurchaseFree)
                {
                    var purchaseOdds        = infoDocument["purchaseodds"] as BsonArray;
                    _minFreeSpinWinRate     = (double)purchaseOdds[0];
                    _totalFreeSpinWinRate   = (double)purchaseOdds[1];
                }

                if (this.SupportPurchaseFree && this.PurchaseFreeMultiple > _totalFreeSpinWinRate)
                    _logger.Error("freespin win rate doesn't satisfy condition {0}", this.GameName);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
            }
        }


        protected override async Task onProcMessage(string strUserID, int websiteID, GITMessage message,UserBonus userBonus,  double userBalance, Currencies currency, bool isAffiliate)
        {
            if (message.MsgCode == MsgCodes.SPIN)
                await onDoSpin(strUserID, websiteID, message, userBonus, userBalance, currency, isAffiliate);
            else if (message.MsgCode == MsgCodes.GETGAMERULE)
                onGetGameRule();
            else if (message.MsgCode == MsgCodes.NOTPROCDRESULT)
                onDoUndoUserSpin(strUserID, websiteID);
        }
        protected virtual void onGetGameRule()
        {
            Sender.Tell(GameRuleString);
        }
        protected virtual async Task onDoSpin(string strUserID,  int websiteID, GITMessage message, UserBonus userBonus, double userBalance, Currencies currency, bool isAffiliate)
        {
            try
            {
                _isRewardedBonus        = false;
                _bonusSendMessage       = null;
                _rewardedBonusMoney     = 0.0;

                long    lastTransID     = (long)message.Pop();
                string  strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);
                readBetInfoFromMessage(message, strGlobalUserID);
                if (!_dicUserHistory.ContainsKey(strGlobalUserID))
                {
                    var history = new BasePGHistory((int)_gameID);
                    history.cc  = currency.ToString();
                    _dicUserHistory.Add(strGlobalUserID, history);
                }

                await spinGame(strUserID, websiteID, userBonus, userBalance, isAffiliate);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::onDoSpin GameID: {0}, {1}", _gameID, ex);
            }
        }

        protected void onDoUndoUserSpin(string strUserID, int websiteID)
        {
            string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);
            undoUserResultInfo(strGlobalUserID);
            undoUserHistory(strGlobalUserID);
            undoUserBetInfo(strGlobalUserID);
            saveBetResultInfo(strGlobalUserID);
        }
        protected virtual void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                BasePGSlotBetInfo betInfo   = new BasePGSlotBetInfo(this.BaseBet);
                betInfo.BetSize             = (float) Math.Round((double) message.Pop(), 2);
                betInfo.BetLevel            = (int)     message.Pop();
                betInfo.PurchaseFree        = (int)     message.Pop() == 2;
               
                if (betInfo.BetSize <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetSize <= 0 in BasePGSlotGame::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetSize);
                    return;
                }
                BasePGSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strGlobalUserID, out oldBetInfo))
                {
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetSize      = betInfo.BetSize;
                    oldBetInfo.BetLevel     = betInfo.BetLevel;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePGSlotGame::readBetInfoFromMessage {0}", ex);
            }
        }
        protected virtual void undoUserResultInfo(string strGlobalUserID)
        {
            try
            {
                if (!_dicUserLastBackupResultInfos.ContainsKey(strGlobalUserID))
                    return;

                BasePGSlotSpinResult lastResult = _dicUserLastBackupResultInfos[strGlobalUserID];
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
                            BasePGHistory history = restoreHistory(binaryReader);
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
                        BasePGSlotBetInfo betInfo   = restoreBetInfo(strGlobalUserID, binaryReader);
                        _dicUserBetInfos[strGlobalUserID] = betInfo;
                    }
                }
                _dicUserLastBackupBetInfos.Remove(strGlobalUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePGSlotGame::undoUserBetInfo {0}", ex);
            }
        }

        protected double convertWinByBet(double win, float currentBet)
        {
            win = win / _spinDataDefaultBet * currentBet;
            return Math.Round(win, 2);
        }
        protected virtual void convertBetsByBet(dynamic jsonParams, double betSize, int betLevel, float totalBet)
        {
            if (!IsNullOrEmpty(jsonParams["cs"]))
                jsonParams["cs"] = Math.Round(betSize, 2);

            if (!IsNullOrEmpty(jsonParams["ml"]))
                jsonParams["ml"] = betLevel;

            if (jsonParams["fb"] != null && jsonParams["fb"] == 2)
            {
                if (!IsNullOrEmpty(jsonParams["tb"]))
                    jsonParams["tb"] = convertWinByBet((double)jsonParams["tb"], totalBet / (float)  PurchaseFreeMultiple);

                if (!IsNullOrEmpty(jsonParams["tbb"]))
                    jsonParams["tbb"] = convertWinByBet((double)jsonParams["tbb"], totalBet / (float) PurchaseFreeMultiple);
            }
            else
            {
                if (!IsNullOrEmpty(jsonParams["tb"]))
                    jsonParams["tb"] = convertWinByBet((double)jsonParams["tb"], totalBet);

                if (!IsNullOrEmpty(jsonParams["tbb"]))
                    jsonParams["tbb"] = convertWinByBet((double)jsonParams["tbb"], totalBet);
            }

        }
        public static bool IsNullOrEmpty(JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                   (token.Type == JTokenType.Null);
        }
        public static bool IsArrayOrObject(JToken token)
        {
            return (token.Type == JTokenType.Array) ||
                   (token.Type == JTokenType.Object);
        }
        protected virtual void convertWinsByBet(dynamic jsonParams, float currentBet)
        {
            if (!IsNullOrEmpty(jsonParams["aw"]))
                jsonParams["aw"] = convertWinByBet((double)jsonParams["aw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["ssaw"]))
                jsonParams["ssaw"] = convertWinByBet((double)jsonParams["ssaw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["tw"]))
                jsonParams["tw"] = convertWinByBet((double)jsonParams["tw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["fs"]) && IsArrayOrObject(jsonParams["fs"]) && !IsNullOrEmpty(jsonParams["fs"]["aw"]))
                jsonParams["fs"]["aw"] = convertWinByBet((double)jsonParams["fs"]["aw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["np"]))
                jsonParams["np"] = convertWinByBet((double)jsonParams["np"], currentBet);

            if (!IsNullOrEmpty(jsonParams["lw"]))
            {
                string strLw = jsonParams["lw"].ToString();
                Dictionary<int, double> lineWins = JsonConvert.DeserializeObject<Dictionary<int, double>>(strLw);
                Dictionary<int, double> convertedLineWins = new Dictionary<int, double>();
                foreach (KeyValuePair<int, double> pair in lineWins)
                {
                    convertedLineWins[pair.Key] = convertWinByBet(pair.Value, currentBet);
                }
                jsonParams["lw"] = JObject.FromObject(convertedLineWins);
            }
        }
        protected long createTransactionID()
        {
            return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).Ticks * 100;
        }
        protected long createTimestamp()
        {
            return (long) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
        protected virtual BasePGSlotSpinResult calculateResult(BasePGSlotBetInfo betInfo, string strSpinResponse, bool isFirst, double userBalance, double realBet)
        {
            try
            {
                BasePGSlotSpinResult        spinResult  = new BasePGSlotSpinResult();
                dynamic                     jsonParams  = JsonConvert.DeserializeObject(strSpinResponse);

                convertWinsByBet(jsonParams, betInfo.TotalBet);
                if (SupportPurchaseFree && betInfo.PurchaseFree && isFirst)
                {
                    convertBetsByBet(jsonParams, betInfo.BetSize, betInfo.BetLevel, (float) realBet);
                    jsonParams["fb"] = 2;
                }
                else
                {
                    jsonParams["fb"] = null;
                    convertBetsByBet(jsonParams, betInfo.BetSize, betInfo.BetLevel, betInfo.TotalBet);
                }

                if (isFirst)
                {
                    betInfo.TransactionID       = createTransactionID();
                    betInfo.RoundID             = genRoundID();
                    betInfo.BetTransactionID    = genTransactionID();
                }

                double totalWin    = jsonParams["tw"];
                jsonParams["psid"] = betInfo.TransactionID.ToString();
                jsonParams["sid"]  = createTransactionID().ToString();
                jsonParams["blb"]  = Math.Round(userBalance, 2);
                jsonParams["blab"] = Math.Round(userBalance - realBet, 2);
                jsonParams["bl"]   = Math.Round(userBalance - realBet + totalWin, 2);

                spinResult.TotalWin     = totalWin;
                overideSomeParam(betInfo, jsonParams);
                spinResult.ResultString = JsonConvert.SerializeObject(jsonParams);
                return spinResult;
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePGSlotGame::calculateResult {0}", ex);
                return null;
            }
        }
        protected virtual void overideSomeParam(BasePGSlotBetInfo betInfo, dynamic jsonParams)
        {

        }
        protected List<BasePGResponse> buildResponseList(List<string> responseList, int startIndex = 1)
        {
            List<BasePGResponse> pgResponseList = new List<BasePGResponse>();
            for (int i = startIndex; i < responseList.Count; i++)
                pgResponseList.Add(new BasePGResponse(responseList[i]));
 
            return pgResponseList;
        }
        protected virtual double getPurchaseMultiple(BasePGSlotBetInfo betInfo)
        {
            return this.PurchaseFreeMultiple;
        }
        protected virtual double getAnteBetMultiple(BasePGSlotBetInfo betInfo)
        {
            return this.AnteBetMultiple;
        }
        protected virtual async Task<BasePGSlotSpinResult> generateSpinResult(BasePGSlotBetInfo betInfo, string strUserID, int websiteID, UserBonus userBonus, double userBalance, bool isAffiliate)
        {
            BasePGSlotSpinData      spinData = null;
            BasePGSlotSpinResult    result   = null;

            if (betInfo.HasRemainResponse)
            {
                BasePGResponse nextResponse = betInfo.pullRemainResponse();
                result                      = calculateResult(betInfo, nextResponse.Response, false, userBalance, 0.0);

                if (!betInfo.HasRemainResponse)
                    betInfo.RemainReponses = null;
                return result;
            }

            float   totalBet        = betInfo.TotalBet;
            double  realBetMoney    = totalBet;

            if (SupportPurchaseFree && betInfo.PurchaseFree)
                realBetMoney = totalBet * getPurchaseMultiple(betInfo);

            spinData = await selectRandomStop(totalBet, betInfo, websiteID, userBonus, isAffiliate);

            double totalWin = totalBet * spinData.SpinOdd;
            if (isAffiliate || await checkWebsitePayoutRate(websiteID, realBetMoney, totalWin))
            {
                if (spinData.IsEvent)
                {
                    _bonusSendMessage   = null;
                    _rewardedBonusMoney = totalWin;
                    _isRewardedBonus    = true;
                }

                result = calculateResult(betInfo, spinData.SpinStrings[0], true, userBalance, realBetMoney);
                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
                return result;
            }

            double emptyWin = 0.0;
            if (SupportPurchaseFree && betInfo.PurchaseFree)
            {
                spinData    = await selectMinStartFreeSpinData(betInfo);
                result      = calculateResult(betInfo, spinData.SpinStrings[0], true, userBalance, realBetMoney);
                emptyWin    = totalBet * spinData.SpinOdd;

                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
            }
            else
            {
                spinData = await selectEmptySpin(betInfo);
                result   = calculateResult(betInfo, spinData.SpinStrings[0], true, userBalance, realBetMoney);
            }
            sumUpWebsiteBetWin(websiteID, realBetMoney, emptyWin);
            return result;
        }

        protected byte[] backupBetInfo(BasePGSlotBetInfo betInfo)
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
        protected virtual async Task spinGame(string strUserID, int websiteID,UserBonus userBonus, double userBalance, bool isAffiliate)
        {
            try
            {
                BasePGSlotBetInfo   betInfo         = null;
                string              strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);
                if (!_dicUserBetInfos.TryGetValue(strGlobalUserID, out betInfo))
                    return;

                byte[] betInfoBytes = backupBetInfo(betInfo);
                byte[] historyBytes = backupHistoryInfo(strGlobalUserID);

                BasePGSlotSpinResult lastResult = null;
                if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                    lastResult = _dicUserResultInfos[strGlobalUserID];

                double betMoney = betInfo.TotalBet;
                if (betInfo.HasRemainResponse)
                    betMoney = 0.0;

                if (this.SupportPurchaseFree && betInfo.PurchaseFree)
                    betMoney = Math.Round(betMoney * getPurchaseMultiple(betInfo), 2);

                if (userBalance.LT(betMoney, _epsilion) || betMoney < 0.0)
                {
                    GITMessage message = new GITMessage(MsgCodes.NOTENOUGHBALANCE);
                    message.Append(buildNotEnoughResult(userBalance, betInfo));
                    Sender.Tell(new ToUserMessage((int) _gameID, message), Self);
                    return;
                }

                BasePGSlotSpinResult spinResult = await this.generateSpinResult(betInfo, strUserID, websiteID, userBonus, userBalance, isAffiliate);                
                _dicUserResultInfos[strGlobalUserID]  = spinResult;

                saveBetResultInfo(strGlobalUserID);
                addResultToHistory(strGlobalUserID, spinResult);

                if(!betInfo.HasRemainResponse && (betInfo.SpinData == null || !(betInfo.SpinData is BasePGSlotStartSpinData)))
                {
                    var historyItem = saveHistory(websiteID, strUserID);
                    sendGameResult(betInfo, spinResult, betMoney, spinResult.WinMoney, userBalance, true, historyItem);
                }
                else
                {
                    sendGameResult(betInfo, spinResult, betMoney, spinResult.WinMoney, userBalance, false, null);
                }
                _dicUserLastBackupBetInfos[strGlobalUserID]       = betInfoBytes;
                _dicUserLastBackupResultInfos[strGlobalUserID]    = lastResult;
                _dicUserLastBackupHistory[strGlobalUserID]        = historyBytes;

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::spinGame {0}", ex);
            }
        }
        protected virtual string buildNotEnoughResult(double userBalance, BasePGSlotBetInfo betInfo)
        {
            JToken errResponse          = JToken.Parse("{}");
            errResponse["si"]           = JToken.Parse(this.ErrorResultString);
            errResponse["si"]["blb"]    = Math.Round(userBalance, 2);
            errResponse["si"]["blab"]   = Math.Round(userBalance, 2);
            errResponse["si"]["bl"]     = Math.Round(userBalance, 2);
            errResponse["si"]["psid"]   = betInfo.TransactionID.ToString();
            errResponse["si"]["sid"]    = betInfo.TransactionID.ToString();
            errResponse["si"]["cs"]     = Math.Round(betInfo.BetSize, 2);
            errResponse["si"]["ml"]     = betInfo.BetLevel;
            return JsonConvert.SerializeObject(errResponse);

        }
        private long getUnixMiliTimestamp()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            long unixTimeMilliseconds = now.ToUnixTimeMilliseconds();
            return unixTimeMilliseconds;
        }
        protected string genRoundID()
        {
            return string.Format("{0}{1}", getUnixMiliTimestamp(), Guid.NewGuid().ToString().Replace("-", ""));
        }
        protected string genTransactionID()
        {
            return string.Format("{0}{1}", getUnixMiliTimestamp(), Guid.NewGuid().ToString().Replace("-", ""));
        }

        protected virtual void sendGameResult(BasePGSlotBetInfo betInfo, BasePGSlotSpinResult spinResult, double betMoney, double winMoney, double userBalance, bool isEndTransaction, PGGameHistoryDBItem historyItem)
        {
            string      strResultString = spinResult.ResultString;            
            GITMessage  message         = new GITMessage(MsgCodes.SPIN);
            message.Append(strResultString);

            ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, message, betMoney, winMoney, new GameLogInfo(GameName, "0", ""), UserBetTypes.Normal);
            if (_isRewardedBonus)
            {
                toUserResult.setBonusReward(_rewardedBonusMoney);
                toUserResult.insertFirstMessage(_bonusSendMessage);
            }

            toUserResult.BetTransactionID    = betInfo.BetTransactionID;
            toUserResult.RoundID             = betInfo.RoundID;
            if (isEndTransaction)
            {
                toUserResult.EndTransaction = true;
                toUserResult.TransactionID  = genTransactionID();
            }
            else
            {
                toUserResult.EndTransaction = false;
                if (winMoney > 0.0)
                    toUserResult.TransactionID = genTransactionID();
            }
            toUserResult.HistoryItem = historyItem;
            Sender.Tell(toUserResult, Self);
        }
        protected virtual void addResultToHistory(string strGlobalUserID, BasePGSlotSpinResult result)
        {
            if (!_dicUserHistory.ContainsKey(strGlobalUserID))
                return;

            dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(result.ResultString);

            BasePGHistoryItem historyItem = new BasePGHistoryItem((double) resultContext["bl"], createTimestamp(), (double) resultContext["tb"], result.TotalWin, (string) resultContext["sid"], resultContext);
            _dicUserHistory[strGlobalUserID].bd.Add(historyItem);


            if (!IsNullOrEmpty(resultContext["st"]))
            {
                int state = (int) resultContext["st"];
                if(state == 4)
                    _dicUserHistory[strGlobalUserID].mgcc++;
                else if(state == 22)
                    _dicUserHistory[strGlobalUserID].fscc++;
            }
        }

        protected virtual PGGameHistoryDBItem saveHistory(int websiteID, string strUserID)
        {
            PGGameHistoryDBItem historyItem = null;
            var strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);
            if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].bd.Count > 0)
            {
                _dicUserHistory[strGlobalUserID].doSummary();
                historyItem = new PGGameHistoryDBItem(websiteID, strUserID, (int)_gameID, _dicUserHistory[strGlobalUserID].gtba,
                    _dicUserHistory[strGlobalUserID].gtwla, _dicUserHistory[strGlobalUserID].tid, _dicUserHistory[strGlobalUserID].bt, 
                    JsonConvert.SerializeObject(_dicUserHistory[strGlobalUserID]));
            }
            _dicUserHistory.Remove(strGlobalUserID);
            return historyItem;
        }
        protected virtual async Task<BasePGSlotSpinData> selectRandomStop(BasePGSlotBetInfo betInfo, int websiteID, bool isAffiliate)
        {
            OddAndIDData selectedOddAndID = selectRandomOddAndID(betInfo, websiteID, isAffiliate);
            var          spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, selectedOddAndID.ID), TimeSpan.FromSeconds(10.0));
            return convertBsonToSpinData(spinDataDocument);
        }
        protected virtual OddAndIDData selectRandomOddAndID(BasePGSlotBetInfo betInfo, int websiteID, bool isAffiliate)
        {
            if(isAffiliate)
            {
                OddAndIDData oddIDAndOdd = new OddAndIDData();
                oddIDAndOdd.ID           = Pcg.Default.Next(1, _naturalSpinCount + 1);
                return oddIDAndOdd;
            }
            else
            {
                double payoutRate   = getPayoutRate(websiteID);
                double randomDouble = Pcg.Default.NextDouble(0.0, 100.0);
                int selectedID = 0;
                if (randomDouble >= payoutRate || payoutRate == 0.0)
                {
                    selectedID = Pcg.Default.Next(1, _emptySpinCount + 1);
                }
                else
                {
                    double magicValue = 100.0 / _naturalRTP;
                    if (_naturalRTP <= 1.0 || Pcg.Default.NextDouble(0, 100.0) < magicValue)
                        selectedID = Pcg.Default.Next(1, _naturalSpinCount + 1);
                    else
                        selectedID = Pcg.Default.Next(1, _emptySpinCount + 1);
                }
                OddAndIDData oddIDAndOdd = new OddAndIDData();
                oddIDAndOdd.ID = selectedID;
                return oddIDAndOdd;
            }
        }
        protected virtual async Task<BasePGSlotSpinData> selectEmptySpin(BasePGSlotBetInfo betInfo)
        {
            int id = Pcg.Default.Next(1, _emptySpinCount + 1);
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, id), TimeSpan.FromSeconds(10.0));
            return convertBsonToSpinData(spinDataDocument);
        }
        protected virtual BasePGSlotSpinData convertBsonToSpinData(BsonDocument document)
        {
            int     spinType    = (int)     document["spintype"];
            double  spinOdd     = (double)  document["odd"];
            string  strData     = (string)  document["data"];

            List<string> spinResponses = new List<string>(strData.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
            return new BasePGSlotSpinData(spinType, spinOdd, spinResponses);
        }
        protected virtual async Task<BasePGSlotSpinData> selectRandomStartFreeSpinData(BasePGSlotBetInfo betInfo)
        {
            try
            {
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, HasPurEnableOption ? StartSpinSearchTypes.SPECIFIC : StartSpinSearchTypes.GENERAL),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePGSlotGame::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }

        }
        protected virtual async Task<BasePGSlotSpinData> selectMinStartFreeSpinData(BasePGSlotBetInfo betInfo)
        {
            try
            {
                BsonDocument spinDataDocument = null;
                if (HasPurEnableOption)
                {
                    spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                            new SelectSpinTypeOddRangeRequest(GameName, -1, PurchaseFreeMultiple * 0.0, PurchaseFreeMultiple * 0.5, 0), TimeSpan.FromSeconds(10.0));
                }
                else
                {
                    spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                            new SelectSpinTypeOddRangeRequest(GameName, 1, PurchaseFreeMultiple * 0.0, PurchaseFreeMultiple * 0.5), TimeSpan.FromSeconds(10.0));
                }
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePGSlotGame::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected virtual async Task<BasePGSlotSpinData> selectPurchaseFreeSpin(int websiteID, BasePGSlotBetInfo betInfo, double baseBet, bool isAffiliate)
        {
            if (isAffiliate)
            {
                BasePGSlotSpinData spinData = await selectRandomStartFreeSpinData(betInfo);
                return spinData;
            }
            else
            {
                double payoutRate = getPayoutRate(websiteID);
                double targetC    = PurchaseFreeMultiple * payoutRate / 100.0;
                if (targetC >= _totalFreeSpinWinRate)
                    targetC = _totalFreeSpinWinRate;

                if (targetC < _minFreeSpinWinRate)
                    targetC = _minFreeSpinWinRate;

                double x = (_totalFreeSpinWinRate - targetC) / (_totalFreeSpinWinRate - _minFreeSpinWinRate);
                double y = 1.0 - x;

                BasePGSlotSpinData spinData = null;
                if (Pcg.Default.NextDouble(0.0, 1.0) <= x)
                    spinData = await selectMinStartFreeSpinData(betInfo);
                else
                    spinData = await selectRandomStartFreeSpinData(betInfo);
                return spinData;
            }
        }
        protected virtual async Task<BasePGSlotSpinData> selectRangeSpinData(int websiteID, double minOdd, double maxOdd, BasePGSlotBetInfo betInfo, bool isAffiliate)
        {
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                    new SelectSpinTypeOddRangeRequest(GameName, -1, minOdd, maxOdd), TimeSpan.FromSeconds(10.0));

            if (spinDataDocument == null)
                return null;

            return convertBsonToSpinData(spinDataDocument);
        }
        public virtual async Task<BasePGSlotSpinData> selectRandomStop(double baseBet, BasePGSlotBetInfo betInfo, int websiteID, UserBonus userBonus, bool isAffiliate)
        {
            if(this.SupportPurchaseFree && betInfo.PurchaseFree)
                return await selectPurchaseFreeSpin(websiteID, betInfo, baseBet, isAffiliate);

            if (userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus rangeOddBonus = userBonus as UserRangeOddEventBonus;
                if (baseBet.LE(rangeOddBonus.MaxBet, _epsilion))
                {
                    BasePGSlotSpinData spinDataEvent = await selectRangeSpinData(websiteID, rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd, betInfo, isAffiliate);
                    if (spinDataEvent != null)
                    {
                        spinDataEvent.IsEvent = true;
                        return spinDataEvent;
                    }
                }
            }

            return await selectRandomStop(betInfo, websiteID, isAffiliate);
        }
        protected override async Task<bool> loadUserHistoricalData(string strGlobalUserID, bool isNewEnter)
        {
            try
            {
                string strKey = string.Format("{0}_{1}", strGlobalUserID, _gameID);
                byte[] betInfoData = await RedisDatabase.RedisCache.StringGetAsync(strKey);
                if (betInfoData != null)
                {
                    using (var stream = new MemoryStream(betInfoData))
                    {
                        BinaryReader        reader  = new BinaryReader(stream);
                        BasePGSlotBetInfo   betInfo = restoreBetInfo(strGlobalUserID, reader);
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
                        BasePGSlotSpinResult    resultInfo  = restoreResultInfo(strGlobalUserID, reader);
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
                        BasePGHistory userHistory = restoreHistory(reader);
                        if (userHistory != null)
                            _dicUserHistory[strGlobalUserID] = userHistory;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePGSlotGame::loadUserHistoricalData {0}", ex);
                return false;
            }
            return await base.loadUserHistoricalData(strGlobalUserID, isNewEnter);
        }
        protected virtual BasePGSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            BasePGSlotBetInfo betInfo = new BasePGSlotBetInfo(this.BaseBet);
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected BasePGHistory restoreHistory(BinaryReader reader)
        {
            try
            {
                BasePGHistory history = new BasePGHistory((int) _gameID);
                history.serializeFrom(reader);
                return history;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePGSlotGame::restoreHistory {0}", ex);
                return null;
            }
        }
        protected virtual BasePGSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            BasePGSlotSpinResult result = new BasePGSlotSpinResult();
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
        protected override async Task onUserExitGame(string strGlobalUserID, bool userRequested)
        {
            try
            {
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
                _dicUserLastBackupResultInfos.Remove(strGlobalUserID);
                _dicUserLastBackupBetInfos.Remove(strGlobalUserID);
                _dicUserLastBackupHistory.Remove(strGlobalUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::onUserExitGame GameID:{0} {1}", _gameID, ex);
            }

            await base.onUserExitGame(strGlobalUserID, userRequested);
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
