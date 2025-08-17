using Akka.Actor;
using Akka.Util;
using GITProtocol;
using GITProtocol.Utils;
using MongoDB.Bson;
using PCGSharp;
using SlotGamesNode.Database;
using SlotGamesNode.GameLogics.BaseClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    internal class BaseGreenSlotGame : IGameLogicActor
    {
        protected Dictionary<int, int> _spinDataDefaultBet = new Dictionary<int, int>();
        protected Dictionary<int, int[]> _emptySpinIndex = new Dictionary<int, int[]>();
        protected Dictionary<int, int> _normalStartId = new Dictionary<int, int>();
        protected Dictionary<int, int> _normalMaxID = new Dictionary<int, int>();
        protected Dictionary<int, int> _naturalSpinCount = new Dictionary<int, int>();
        protected int[] _supportLines;
        public int _defaultBetPerLine { get; set; }
        public int _defaultLine { get; set; }
        public bool _canChangeLine { get; set; }

        protected Dictionary<string, BaseClasses.BaseGreenSlotBetInfo> _dicUserBetInfos = new Dictionary<string, BaseClasses.BaseGreenSlotBetInfo>();
        protected Dictionary<string, BaseClasses.BaseGreenSlotSpinResult> _dicUserResultInfos = new Dictionary<string, BaseClasses.BaseGreenSlotSpinResult>();

        protected Dictionary<string, BaseClasses.BaseGreenSlotSpinResult> _dicUserLastBackupResultInfos = new Dictionary<string, BaseClasses.BaseGreenSlotSpinResult>();
        protected Dictionary<string, byte[]> _dicUserLastBackupBetInfos = new Dictionary<string, byte[]>();
        
        protected virtual string ScatterSymbol => "";
        protected virtual int MinCountToFreespin => 0;
        protected virtual bool SupportPurchaseFree => false;
        protected virtual double PurchaseFreeMultiple => 0.0;

        protected virtual string VersionCheckString => "";
        protected virtual string[] SupportCurrencyList
        {
            get
            {
                return new string[]
                {
                    "\fUSDÿ$",
                    "\fEURÿâ\u0082¬",
                    "\fGBPÿÂ£",
                    "\fPENÿS/",
                    "\fCHFÿFr",
                    "\fCZKÿKÄ\u008d",
                    "\fBYNÿBr",
                    "\fHRKÿKn",
                    "\fMYRÿRM",
                    "\fGELÿGEL",
                    "\fRSDÿdin",
                    "\fALLÿALL",
                    "\fCOPÿ$",
                    "\fHUFÿFt",
                    "\fNZDÿ$",
                    "\fAUDÿ$",
                    "\fNOKÿkr",
                    "\fPLNÿzÅ\u0082",
                    "\fDKKÿkr",
                    "\fRONÿlei",
                    "\fSGDÿ$",
                    "\fAMDÿÕ¤Ö\u0080",
                    "\fCADÿ$",
                    "\fSEKÿkr",
                    "\fBRLÿR$",
                    "\fRUBÿpyÐ±",
                    "\fMXNÿ$",
                    "\fBAMÿKM",
                };
            }
        }
        protected virtual string GameUniqueString => "";
        private int TestNumber { get; set; }

        public BaseGreenSlotGame()
        {
            ReceiveAsync<PerformanceTestRequest>(onPerformanceTest);
            TestNumber = 0;
        }
        public string getCurrencyCode(Currencies currency)
        {
            string currencyString = SupportCurrencyList[(int)currency];
            string currencyCode = currencyString.Split('ÿ')[0];
            return currencyCode.TrimStart('\f');
        }
        public string getCurrencySymbol(Currencies currency)
        {
            string currencyString = SupportCurrencyList[(int)currency];
            return currencyString.Split('ÿ')[1];
        }
        protected virtual async Task onPerformanceTest(PerformanceTestRequest _)
        {
            try
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                double sumOdd1 = 0.0;
                BaseClasses.BaseGreenSlotBetInfo betInfo = new BaseClasses.BaseGreenSlotBetInfo();
                betInfo.MoreBet = -1;
                betInfo.PurchaseStep = -1;
                for (int i = 0; i < 100000; i++)
                {
                    BasePPSlotSpinData spinData = await selectRandomStop(0, betInfo);
                    sumOdd1 += spinData.SpinOdd;
                }
                stopWatch.Stop();
                long elapsed1 = stopWatch.ElapsedMilliseconds;

                _logger.Info("{0} Performance Test Results:  \r\nPayrate: {3}%, {1}s, {2}%", this.GameName,
                    Math.Round((double)elapsed1 / 1000.0, 3), Math.Round(sumOdd1 / 1000, 3),
                    _config.PayoutRate);
                Sender.Tell(true);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseGreentubeSlotGame::onPerformanceTest {0}", ex);
                Sender.Tell(false);
            }
        }

        protected override void LoadSetting()
        {
            base.LoadSetting();
            initGameData();
        }
        protected virtual void initGameData()
        {
        }
        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBet = infoDocument["defaultbet"].AsBsonArray.OfType<BsonDocument>().ToDictionary(doc => doc["line"].AsInt32, doc => doc["value"].AsInt32);
                _emptySpinIndex = infoDocument["emptyindex"].AsBsonArray.OfType<BsonDocument>().ToDictionary(doc => doc["line"].AsInt32, doc => new int[] { doc["value"]["startIndex"].AsInt32, doc["value"]["endIndex"].AsInt32, });
                _normalStartId = infoDocument["startid"].AsBsonArray.OfType<BsonDocument>().ToDictionary(doc => doc["line"].AsInt32, doc => doc["value"].AsInt32);
                _normalMaxID = infoDocument["normalmaxid"].AsBsonArray.OfType<BsonDocument>().ToDictionary(doc => doc["line"].AsInt32, doc => doc["value"].AsInt32);
                _naturalSpinCount = infoDocument["normalselectcount"].AsBsonArray.OfType<BsonDocument>().ToDictionary(doc => doc["line"].AsInt32, doc => doc["value"].AsInt32);
                _supportLines = _spinDataDefaultBet.Keys.ToArray();
                _defaultLine = _supportLines[_supportLines.Length - 1];
                _defaultBetPerLine = _spinDataDefaultBet[_defaultLine] / _defaultLine;
                _canChangeLine = _supportLines.Length > 0;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
            }
        }

        #region
        protected override async Task onProcMessage(string strUserID, int websiteID, GITMessage message, double userBalance, Currencies currency)
        {
            string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);
            switch ((CSMSG_CODE)message.MsgCode)
            {
                case CSMSG_CODE.CS_GREENTUBE_DOINIT:
                    onDoInit(strGlobalUserID, message, userBalance, currency);
                    break;
                case CSMSG_CODE.CS_GREENTUBE_BALANCECONFIRM:
                    onBalanceConfirm(strGlobalUserID, message, userBalance, currency);
                    break;
                case CSMSG_CODE.CS_GREENTUBE_CHANGELINEBET:
                    onChangeLineAndBet(strGlobalUserID, message, userBalance, currency);
                    break;
                case CSMSG_CODE.CS_GREENTUBE_DOSPIN:
                    await onDoSpin(strUserID, websiteID, message, userBalance, currency);
                    break;
                case CSMSG_CODE.CS_GREENTUBE_DOCOLLECT:
                    onDoCollect(strGlobalUserID, message, userBalance, currency);
                    break;
                case CSMSG_CODE.CS_GREENTUBE_DOGAMBLEPICK:
                    await onGamblePick(strUserID, websiteID, message, userBalance, currency);
                    break;
                case CSMSG_CODE.CS_GREENTUBE_DOGAMBLEHALF:
                    await onGambleHalf(strUserID, websiteID, message, userBalance, currency);
                    break;
            }
        }
        protected virtual void onDoInit(string strGlobalUserID, GITMessage message, double balance, Currencies currency)
        {
            
        }
        protected virtual void onBalanceConfirm(string strGlobalUserID, GITMessage message, double balance, Currencies currency)
        {

        }

        protected virtual void onChangeLineAndBet(string strGlobalUserID, GITMessage message, double balance, Currencies currency)
        {
            int betPerLine = int.Parse(message.Pop().ToString());
            int lineIndex = int.Parse(message.Pop().ToString());

            BaseClasses.BaseGreenSlotBetInfo oldBetInfo = null;
            if (_dicUserBetInfos.TryGetValue(strGlobalUserID, out oldBetInfo))
            {
                oldBetInfo.PlayLine = _supportLines[lineIndex];
                oldBetInfo.PlayBet = _supportLines[lineIndex] * betPerLine;
            }
            else
            {
                BaseClasses.BaseGreenSlotBetInfo betInfo = new BaseClasses.BaseGreenSlotBetInfo();
                betInfo.PlayLine = _supportLines[lineIndex];
                betInfo.PlayBet = _supportLines[lineIndex] * betPerLine;
                _dicUserBetInfos.Add(strGlobalUserID, betInfo);
            }

            saveBetResultInfo(strGlobalUserID);
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
        protected virtual async Task onDoSpin(string strUserID, int websiteID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);

                bool isNewBet = true;
                if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                    isNewBet = false;

                if (message.MsgCode == (ushort)CSMSG_CODE.CS_GREENTUBE_DOSPIN)
                    readBetInfoFromMessage(message, strGlobalUserID, currency);

                await spinGame(strUserID, websiteID, userBalance, isNewBet, currency);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseGreentubeSlotGame::onDoSpin GameID: {0}, {1}", _gameID, ex);
            }
        }
        protected void onDoCollect(string strGlobalUserID, GITMessage message, double balance, Currencies currency)
        {
            try
            {
                if (!_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    _logger.Error("{0} bet information has not been found in BaseGreentubeSlotGame::onDoCollect.", strGlobalUserID);
                    return;
                }
                BaseClasses.BaseGreenSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];

                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    _logger.Error("{0} result information has not been found in BaseGreentubeSlotGame::onDoCollect.", strGlobalUserID);
                    return;
                }

                BaseClasses.BaseGreenSlotSpinResult result = _dicUserResultInfos[strGlobalUserID];
                //if(!betInfo.HasRemainResponse)
                balance += result.TotalWin;

                _dicUserResultInfos[strGlobalUserID] = result;
                _dicUserLastBackupResultInfos[strGlobalUserID] = result;

                string strResponse = buildCollectResMsgString(strGlobalUserID, balance, betInfo, result, GreenMessageType.Collect, currency);
                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_GREENTUBE_DOCOLLECT);
                responseMessage.Append("%t0ÿq0");
                responseMessage.Append(string.Format("\u001f{0}", result.TotalWin));
                responseMessage.Append(strResponse);
                //responseMessage.Append("\u001d");

                if (result.TotalWin > 0.0)
                {
                    //if (betInfo.HasRemainResponse)
                    //    result.TotalWin = 0;
                    ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, responseMessage, 0.0, result.TotalWin, new GameLogInfo(GameName, "0", "Collect"), UserBetTypes.Normal);
                    toUserResult.BetTransactionID = betInfo.BetTransactionID;
                    toUserResult.RoundID = betInfo.RoundID;
                    toUserResult.TransactionID = createTransactionID();
                    toUserResult.RoundEnd = true;

                    saveBetResultInfo(strGlobalUserID);
                    Sender.Tell(toUserResult);

                    result.TotalWin = 0.0;
                    return;
                }

                Sender.Tell(new ToUserMessage((ushort)_gameID, responseMessage));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseGreentubeSlotGame::onDoCollect {0}", ex);
            }
        }
        protected async Task onGamblePick(string strUserID, int websiteID, GITMessage message, double balance, Currencies currency = Currencies.USD)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);

                if (!_dicUserBetInfos.ContainsKey(strGlobalUserID) || !_dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    _logger.Error("Can't gamble without betInfo and spinResult in BaseGreentubeSlotGame::onGamblePick");
                    return;
                }

                BaseClasses.BaseGreenSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
                BaseClasses.BaseGreenSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];
                //if (betInfo.HasRemainResponse)
                //{
                //    _logger.Error("Can't gamble until spin finish in BaseGreentubeSlotGame::onGamblePick");
                //    return;
                //}

                if (spinResult.WinMoney <= 0)
                {
                    _logger.Error("Can't gamble if spinmoney is 0 in BaseGreentubeSlotGame::onGamblePick");
                    return;
                }

                betInfo.GambleHalf = false;
                betInfo.GambleType = 9;
                betInfo.GambleRound = 0;
                betInfo.GambleInitTotalWin = spinResult.TotalWin;
                betInfo.GambleInitCollectWin = spinResult.CollectWin;

                if (betInfo.GambleType == 0)
                {
                    _logger.Error("Gamble index is failed in BaseGreentubeSlotGame::onGamblePick");
                    return;
                }

                string strResponse = buildGamblePickResMsgString(strGlobalUserID, balance, betInfo, spinResult, GreenMessageType.Collect, currency);
                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_GREENTUBE_DOCOLLECT);
                responseMessage.Append("%t0ÿq0");
                responseMessage.Append(strResponse);

                Sender.Tell(new ToUserMessage((ushort)_gameID, responseMessage));

                //await gamblePickGame(strUserID, websiteID, balance);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseGreentubeSlotGame::onGamblePick {0}", ex);
            }
        }
        protected async Task onGambleHalf(string strUserID, int websiteID, GITMessage message, double balance, Currencies currency = Currencies.USD)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);

                if (!_dicUserBetInfos.ContainsKey(strGlobalUserID) || !_dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    _logger.Error("Can't gamble without betInfo and spinResult in BaseGreentubeSlotGame::onGambleHalf");
                    return;
                }

                BaseClasses.BaseGreenSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
                BaseClasses.BaseGreenSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];

                betInfo.GambleHalf = true;
                
                if (message.MessageContent == "5r")
                    betInfo.GambleType = 1;
                else if (message.MessageContent == "5b")
                    betInfo.GambleType = 2;
                   

                if (betInfo.GambleType == 0)
                {
                    _logger.Error("Gamble index is failed in BaseGreentubeSlotGame::onGamblePick");
                    return;
                }

                ++betInfo.GambleRound;

                await gamblePickGame(strUserID, websiteID, balance, currency);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseGreentubeSlotGame::onGambleHalf {0}", ex);
            }
        }
        protected void onDoUndoUserSpin(string strGlobalUserID)
        {
            undoUserResultInfo(strGlobalUserID);
            undoUserBetInfo(strGlobalUserID);
            saveBetResultInfo(strGlobalUserID);
        }
        protected virtual void undoUserResultInfo(string strGlobalUserID)
        {
            try
            {
                if (!_dicUserLastBackupResultInfos.ContainsKey(strGlobalUserID))
                    return;

                BaseClasses.BaseGreenSlotSpinResult lastResult = _dicUserLastBackupResultInfos[strGlobalUserID];
                if (lastResult == null)
                    _dicUserResultInfos.Remove(strGlobalUserID);
                else
                    _dicUserResultInfos[strGlobalUserID] = lastResult;
                _dicUserLastBackupResultInfos.Remove(strGlobalUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseGreentubeSlotGame::undoUserResultInfo {0}", ex);
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
                        BaseClasses.BaseGreenSlotBetInfo betInfo = restoreBetInfo(strGlobalUserID, binaryReader);
                        _dicUserBetInfos[strGlobalUserID] = betInfo;
                    }
                }
                _dicUserLastBackupBetInfos.Remove(strGlobalUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseGreentubeSlotGame::undoUserBetInfo {0}", ex);
            }
        }
        #endregion

        protected virtual void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                BaseClasses.BaseGreenSlotBetInfo betInfo = new BaseClasses.BaseGreenSlotBetInfo();
                betInfo.PurchaseStep = 0;
                betInfo.MoreBet = 0;
                betInfo.CurrencyInfo = currency;
                betInfo.GambleType = 0;
                betInfo.GambleHalf = false;

                BaseClasses.BaseGreenSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strGlobalUserID, out oldBetInfo))
                {
                    if (oldBetInfo.HasRemainResponse)
                    {
                        oldBetInfo.GambleType = betInfo.GambleType;
                        oldBetInfo.GambleHalf = betInfo.GambleHalf;
                        return;
                    }
                        
                    oldBetInfo.PurchaseStep = betInfo.PurchaseStep;
                    oldBetInfo.MoreBet = betInfo.MoreBet;
                    oldBetInfo.CurrencyInfo = betInfo.CurrencyInfo;
                    oldBetInfo.GambleType = betInfo.GambleType;
                    oldBetInfo.GambleHalf = betInfo.GambleHalf;
                }
                else
                {
                    betInfo.PlayLine = this._defaultLine;
                    betInfo.PlayBet = this._defaultLine * this._defaultBetPerLine;
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseGreentubeSlotGame::readBetInfoFromMessage {0}", ex);
            }
        }

        protected GreenMessageType convertRespMsgCodeToAction(long actionType)
        {
            return (GreenMessageType)actionType;
        }
        protected virtual string changeSpinString(string strSpinStr, BaseClasses.BaseGreenSlotBetInfo betInfo, double totalWin, double collectWin, bool isCollect)
        {
            string[] selections = strSpinStr.Split('ÿ');
            if (selections.Length == 2)
                return strSpinStr;
            bool isFreeSpin = false;
            for (int i = 0; i < selections.Length - 1; i++)
            {
                string[] winlineSectors = selections[i].Split(',');
                int lineWin = 0;
                int realLineWin = 0;
                if (winlineSectors[0] == "f")
                {
                    isFreeSpin = true;
                    winlineSectors[6] = isCollect ? (collectWin).ToString() : (collectWin - totalWin).ToString();
                    winlineSectors[7] = collectWin.ToString();
                }
                if (winlineSectors[0] == "l")
                {
                    lineWin = int.Parse(winlineSectors[6]);
                    realLineWin = (int)((double)lineWin * ((double)betInfo.PlayBet / (double)_spinDataDefaultBet[betInfo.PlayLine]));
                    winlineSectors[6] = realLineWin.ToString();
                }
                else if (winlineSectors[0] == "o")
                {
                    lineWin = int.Parse(winlineSectors[5]);
                    realLineWin = (int)((double)lineWin * ((double)betInfo.PlayBet / (double)_spinDataDefaultBet[betInfo.PlayLine]));
                    winlineSectors[5] = realLineWin.ToString();
                }
                else if (winlineSectors[0] == "c")
                {
                    lineWin = int.Parse(winlineSectors[5]);
                    realLineWin = (int)((double)lineWin * ((double)betInfo.PlayBet / (double)_spinDataDefaultBet[betInfo.PlayLine]));
                    winlineSectors[5] = realLineWin.ToString();
                }
                else if (winlineSectors[0] == "x")
                {
                    if (betInfo.BonusOption >= 0 && winlineSectors.Length > 4 && winlineSectors[4] == "0|0")
                    {
                        winlineSectors[5] = betInfo.BonusOption.ToString();
                        winlineSectors[8] = betInfo.BonusOption == 1 ? "B" : (betInfo.BonusOption == 2 ? "C" : "A");
                    }
                }
                selections[i] = String.Join(",", winlineSectors.ToArray());
            }
            selections[selections.Length - 1] = string.Format("rw,{0}", isFreeSpin ? collectWin : totalWin);

            ////test code
            //int insertIndex = 1;

            //string[] newArr = new string[selections.Length + 2];

            //for (int i = 0; i < insertIndex; i++)
            //{
            //    newArr[i] = selections[i];
            //}

            //newArr[insertIndex] = $":x,{collectWin}";
            //newArr[insertIndex + 1] = "s,5";

            //for (int i = insertIndex; i < selections.Length; i++)
            //{
            //    newArr[i + 2] = selections[i];
            //}
            //////////

            //if(isCollect)
            //    return string.Join("ÿ", newArr.ToArray());
            //else
                return string.Join("ÿ", selections.ToArray());
        }
        protected virtual string changeSpinStringForGamble(string strSpinStr, double betMoney, double line, int round, double gambleInitTotalWin, double gambleInitCollectWin, double totalWin, double collectWin)
        {
            string[] selections = strSpinStr.Split('ÿ');
            if (selections.Length == 2)
                return strSpinStr;
            bool isFreeSpin = false;
            for (int i = 0; i < selections.Length; i++)
            {
                string[] winlineSectors = selections[i].Split(',');
                int lineWin = 0;
                int realLineWin = 0;
                int win = 0;
                if (winlineSectors[0] == "f")
                {
                    isFreeSpin = true;
                    if (totalWin == 0) {
                        int a = 100;
                    }
                    winlineSectors[6] = totalWin > 0 ? (gambleInitCollectWin - gambleInitTotalWin).ToString() : (collectWin - totalWin).ToString();
                    winlineSectors[7] = totalWin > 0 ? gambleInitCollectWin.ToString() : collectWin.ToString();
                }
                if (winlineSectors[0] == "l")
                {
                    lineWin = int.Parse(winlineSectors[6]);
                    realLineWin = (int)((double)lineWin * ((double)betMoney / (double)_spinDataDefaultBet[(int)line]));
                    winlineSectors[6] = realLineWin.ToString();
                }
                else if (winlineSectors[0] == "o")
                {
                    lineWin = int.Parse(winlineSectors[5]);
                    realLineWin = (int)((double)lineWin * ((double)betMoney / (double)_spinDataDefaultBet[(int)line]));
                    winlineSectors[5] = realLineWin.ToString();
                }
                else if (winlineSectors[0] == "c")
                {
                    lineWin = int.Parse(winlineSectors[5]);
                    realLineWin = (int)((double)lineWin * ((double)betMoney / (double)_spinDataDefaultBet[(int)line]));
                    winlineSectors[5] = realLineWin.ToString();
                }
                selections[i] = String.Join(",", winlineSectors.ToArray());
            }

            string result = "";

            result = string.Join("ÿ", selections.Take(selections.Length - 1).ToArray());

            if (totalWin > 0)
            {
                string[] roundInfo = new string[round];
                for (int idx = 0; idx < round; idx++)
                {
                    roundInfo[idx] = string.Format("g,{0},0", gambleInitTotalWin * Math.Pow(2, idx));
                }
                result = result + "ÿ" + string.Join("ÿ", roundInfo) + "ÿ" + string.Format("rw,{0}", isFreeSpin ? collectWin : totalWin);
            }
            else
            {
                if(isFreeSpin)
                {
                    result = selections[0] + "ÿ" + selections[1] + "ÿ" + (selections[2].StartsWith("x") ? selections[2] + "ÿ" : "") + string.Format("rw,{0}", isFreeSpin ? collectWin : 0);
                } else
                {
                    result = selections[0] + "ÿ" + (selections[1].StartsWith("x") ? selections[1] + "ÿ" : "") + string.Format("rw,{0}", isFreeSpin ? collectWin : 0);
                }
            }
            return result;
        }
        protected virtual string buildSpinResMsgString(string strGlobalUserID, double balance, double totalWin, double collectWin, BaseClasses.BaseGreenSlotBetInfo betInfo, string spinString, GreenMessageType type, Currencies currency = Currencies.USD)
        {
            List<string> sectorList = new List<string>();
            sectorList.Add(string.Format("\u0006S{0}", balance));
            sectorList.Add("A1");
            sectorList.Add($"C,100.0,{getCurrencySymbol(currency)},0,{getCurrencyCode(currency)}");
            sectorList.Add(string.Format("T,{0},{1},{2}", betInfo.BetTransactionID, betInfo.RelativeTotalBet, type == GreenMessageType.NormalSpin ? totalWin : collectWin - totalWin));
            sectorList.Add(string.Format("R{0}", betInfo.RoundID));
            sectorList.Add("M,1,10000,1,0,1000000");
            if(totalWin > 0 || betInfo.HasRemainResponse)
            {
                sectorList.Add("I10");
                sectorList.Add(string.Format("W,{0}", totalWin));
            }
            else
                sectorList.Add("I11");

            sectorList.Add("H,0,0,0,0,0,0");
            sectorList.Add("X");
            sectorList.Add("Y,10,10");
            sectorList.Add("V25");
            sectorList.Add("bs,0,1,0");
            sectorList.Add(":k,null");
            sectorList.Add(string.Format("e,{0},{1},{2},{3}", betInfo.PlayLine, betInfo.PlayBet / betInfo.PlayLine, this._canChangeLine ? Array.IndexOf(_supportLines, betInfo.PlayLine) : 0, this._canChangeLine ? Array.IndexOf(_supportLines, betInfo.PlayLine) : -1));
            sectorList.Add("b,1000,1000,0");
            sectorList.Add(":x,50");
            
            if (spinString.Contains("f,"))
            {
                if(totalWin > 0)
                {
                    sectorList.Add("s,11");
                    sectorList.Add(string.Format("q,{0}", spinString.Contains("ÿc," + ScatterSymbol + ",") && (int.Parse(spinString.Substring(spinString.IndexOf("ÿc," + ScatterSymbol)).Split(',')[2]) >= MinCountToFreespin) ? 0 : 1));
                }
                else
                    sectorList.Add("s,5");
            }
            else if (spinString.Contains("v,"))
            {
                sectorList.Add("s,13");
            }
            else if ((spinString.Contains("c,N") || spinString.Contains("c,I") || spinString.Contains("c,F")) && spinString.Contains("rw,0"))
            {
                sectorList.Add("s,11");
                sectorList.Add("q,0");
            }
            else
            {
                if (totalWin > 0)
                {
                    sectorList.Add("s,11");
                    sectorList.Add("q,1");
                }
                else
                    sectorList.Add("s,1");
            }

            sectorList.Add(changeSpinString(spinString, betInfo, totalWin, collectWin, false));

            return String.Join("ÿ", sectorList.ToArray());
        }
        protected virtual int getSpinWinMoney(string strSpinData, BaseClasses.BaseGreenSlotBetInfo betInfo, bool isFreespin = false)
        {
            string[] selections = strSpinData.Split('ÿ');
            int totalWin = 0;
            for(int i = 0; i < selections.Length; i++)
            {
                if (selections[i].Substring(0, 1) == "f")
                {
                    string[] eles = selections[i].Split(',');
                    totalWin = (int)((double)(int.Parse(eles[7]) - int.Parse(eles[6])) * ((double)betInfo.PlayBet / (double)_spinDataDefaultBet[betInfo.PlayLine]));
                    isFreespin = true;
                }
            }
            if(!isFreespin)
                totalWin = (int)((double)int.Parse(selections[selections.Length - 1].Split(',')[1]) * ((double)betInfo.PlayBet / (double)_spinDataDefaultBet[betInfo.PlayLine]));
            return totalWin;
        }
        protected virtual string buildCollectResMsgString(string strGlobalUserID, double balance, BaseClasses.BaseGreenSlotBetInfo betInfo, BaseClasses.BaseGreenSlotSpinResult spinResult, GreenMessageType type, Currencies currency = Currencies.USD)
        {

            List<string> sectorList = new List<string>();
            sectorList.Add(string.Format("\u0006S{0}", balance));
            sectorList.Add("A1");
            sectorList.Add($"C,100.0,{getCurrencySymbol(currency)},0,{getCurrencyCode(currency)}");

            bool isFreespin = betInfo.SpinInfo.StartsWith("f");
            sectorList.Add(string.Format("T,{0},{1},{2}", betInfo.BetTransactionID, betInfo.PlayBet, isFreespin ? spinResult.CollectWin : spinResult.TotalWin));
            
            sectorList.Add(string.Format("R{0}", betInfo.RoundID));
            sectorList.Add("M,1,10000,1,0,1000000");
            
            sectorList.Add("H,0,0,0,0,0,0");
            sectorList.Add("X");
            sectorList.Add("Y,10,10");
            sectorList.Add("V25");
            sectorList.Add("bs,0,1,0");
            sectorList.Add(":k,null");
            sectorList.Add(string.Format("e,{0},{1},{2},{3}", betInfo.PlayLine, betInfo.PlayBet / betInfo.PlayLine, this._canChangeLine ? Array.IndexOf(_supportLines, betInfo.PlayLine) : 0, this._canChangeLine ? Array.IndexOf(_supportLines, betInfo.PlayLine) : -1));
            sectorList.Add("b,1000,1000,0");
            //sectorList.Add(":x,50");
            if (betInfo.HasRemainResponse)
            {
                sectorList.Add("s,5");

                if (betInfo.HasRemainResponse)
                    sectorList.Add("I10");
                else
                    sectorList.Add("I11");

                sectorList.Add("W,4296");
                sectorList.Add(changeSpinString(betInfo.SpinInfo, betInfo, spinResult.TotalWin, spinResult.CollectWin, true));
            }
            else
            {
                sectorList.Add("I11");
                sectorList.Add(changeSpinString(betInfo.SpinInfo, betInfo, spinResult.TotalWin, spinResult.CollectWin, true));
                sectorList.Add("s,1");
            }

            return String.Join("ÿ", sectorList.ToArray());
        }
        protected virtual string buildGamblePickResMsgString(string strGlobalUserID, double balance, BaseClasses.BaseGreenSlotBetInfo betInfo, BaseClasses.BaseGreenSlotSpinResult spinResult, GreenMessageType type, Currencies currency = Currencies.USD)
        {
            List<string> sectorList = new List<string>();
            sectorList.Add(string.Format("\u0006S{0}", balance));
            sectorList.Add("A1");
            sectorList.Add($"C,100.0,{getCurrencySymbol(currency)},0,{getCurrencyCode(currency)}");
            sectorList.Add(string.Format("T,{0},{1},{2}", betInfo.BetTransactionID, betInfo.PlayBet, spinResult.TotalWin));
            sectorList.Add(string.Format("R{0}", betInfo.RoundID));
            sectorList.Add("M,5,10000,1,0,1000000");
            sectorList.Add("I10");
            sectorList.Add(string.Format("W,{0}", spinResult.TotalWin));
            sectorList.Add("H,0,0,0,0,0,0");
            sectorList.Add("X");
            sectorList.Add("Y,10,10");
            sectorList.Add("V25");
            sectorList.Add("bs,0,1,0");
            sectorList.Add(":k,null");
            sectorList.Add(string.Format("e,{0},{1},{2},{3}", betInfo.PlayLine, betInfo.PlayBet / betInfo.PlayLine, this._canChangeLine ? Array.IndexOf(_supportLines, betInfo.PlayLine) : 0, this._canChangeLine ? Array.IndexOf(_supportLines, betInfo.PlayLine) : -1));
            sectorList.Add("b,1000,1000,0");
            sectorList.Add(":x,50");
            sectorList.Add("s,3");
            for(int i = 1; i < 9; i++)
            {
                sectorList.Add(string.Format("q,{0},0", betInfo.GambleInitTotalWin * Math.Pow(2, i)));
            }
            if (betInfo.GambleCardHistory.Count > 0)
            {
                sectorList.Add(string.Format("h,{0}", string.Join("", betInfo.GambleCardHistory.ToArray())));
            }
            sectorList.Add(changeSpinString(betInfo.SpinInfo, betInfo, spinResult.TotalWin, spinResult.CollectWin, true));

            return String.Join("ÿ", sectorList.ToArray());
        }
        protected virtual string buildGambleResultResMsgString(string strGlobalUserID, double balance, BaseClasses.BaseGreenSlotBetInfo betInfo, BaseClasses.BaseGreenSlotSpinResult spinResult, GreenMessageType type, Currencies currency = Currencies.USD)
        {
            List<string> sectorList = new List<string>();
            sectorList.Add(string.Format("\u0006S{0}", balance));
            sectorList.Add("A1");
            sectorList.Add($"C,100.0,{getCurrencySymbol(currency)},0,{getCurrencyCode(currency)}");
            bool isFreespin = betInfo.SpinInfo.StartsWith("f");
            sectorList.Add(string.Format("T,{0},{1},{2}", betInfo.BetTransactionID, betInfo.PlayBet, isFreespin ? betInfo.GambleInitCollectWin : betInfo.GambleInitTotalWin));
            sectorList.Add(string.Format("R{0}", betInfo.RoundID));
            sectorList.Add("M,5,10000,1,0,1000000");
            if(spinResult.TotalWin > 0)
            {
                sectorList.Add("I10");
                sectorList.Add(string.Format("W,{0}", spinResult.TotalWin));
            }
            else
                sectorList.Add("I11");
            sectorList.Add("H,0,0,0,0,0,0");
            sectorList.Add("X");
            sectorList.Add("Y,10,10");
            sectorList.Add("V25");
            sectorList.Add("bs,0,1,0");
            sectorList.Add(":k,null");
            sectorList.Add(string.Format("e,{0},{1},{2},{3}", betInfo.PlayLine, betInfo.PlayBet / betInfo.PlayLine, this._canChangeLine ? Array.IndexOf(_supportLines, betInfo.PlayLine) : 0, this._canChangeLine ? Array.IndexOf(_supportLines, betInfo.PlayLine) : -1));
            sectorList.Add("b,1000,1000,0");
            sectorList.Add(":x,50");

            if (spinResult.TotalWin > 0)
            {
                sectorList.Add("s,3");
                for (int i = 1; i < 9; i++)
                {
                    sectorList.Add(string.Format("q,{0},0", betInfo.GambleInitTotalWin * Math.Pow(2, i)));
                }
            }
            else
            {
                if(betInfo.HasRemainResponse)
                    sectorList.Add("s,5");
                else
                    sectorList.Add("s,1");
            }
            if (betInfo.GambleCardHistory.Count > 0)
                sectorList.Add(string.Format("h,{0}", string.Join("", betInfo.GambleCardHistory.ToArray())));
            sectorList.Add(changeSpinStringForGamble(betInfo.SpinInfo, betInfo.PlayBet, betInfo.PlayLine, betInfo.GambleRound, betInfo.GambleInitTotalWin, betInfo.GambleInitCollectWin, spinResult.TotalWin, spinResult.CollectWin));

            return String.Join("ÿ", sectorList.ToArray());
        }
        protected virtual BaseClasses.BaseGreenSlotSpinResult calculateResult(BaseClasses.BaseGreenSlotBetInfo betInfo, BaseClasses.BaseGreenSlotSpinResult lastResult, string strGlobalUserID, string strSpinResponse, bool isFirst, double userBalance, double betMoney, double spinOdd, Currencies currency = Currencies.USD)
        {
            try
            {
                BaseClasses.BaseGreenSlotSpinResult spinResult = new BaseClasses.BaseGreenSlotSpinResult();
                bool isFreespin = strSpinResponse.StartsWith("f");
                spinResult.TotalWin = getSpinWinMoney(strSpinResponse, betInfo);
                if (isFreespin)
                {
                    if (isFirst)
                        spinResult.CollectWin = spinResult.TotalWin;
                    else
                        spinResult.CollectWin += (lastResult.CollectWin + spinResult.TotalWin);
                } else
                {
                    spinResult.CollectWin = 0;
                }
                spinResult.ResultString = buildSpinResMsgString(strGlobalUserID, userBalance, spinResult.TotalWin, spinResult.CollectWin, betInfo, strSpinResponse, isFreespin ? GreenMessageType.FreeSpin : GreenMessageType.NormalSpin, currency);
                spinResult.Action = GreenMessageType.NormalSpin;
                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseGreentubeSlotGame::calculateResult {0}", ex);
                return null;
            }
        }
        protected virtual List<BaseClasses.BaseGreenActionToResponse> buildResponseList(List<string> responseList)
        {
            List<BaseClasses.BaseGreenActionToResponse> actionResponseList = new List<BaseClasses.BaseGreenActionToResponse>();
            for (int i = 1; i < responseList.Count; i++)
            {
                GreenMessageType action = convertRespMsgCodeToAction((long)GreenMessageType.FreeSpin);

                actionResponseList.Add(new BaseClasses.BaseGreenActionToResponse(action, responseList[i]));
            }
            return actionResponseList;
        }
        protected virtual double getPurchaseMultiple(BaseClasses.BaseGreenSlotBetInfo betInfo)
        {
            return this.PurchaseFreeMultiple;
        }
        protected virtual double getPointUnit(BaseClasses.BaseGreenSlotBetInfo betInfo)
        {
            double pointUnit = DicCurrencyInfo.Instance._currencyInfo[betInfo.CurrencyInfo].Rate / 100.0;
            return pointUnit;
        }
        protected virtual async Task<BaseClasses.BaseGreenSlotSpinResult> generateSpinResult(BaseClasses.BaseGreenSlotBetInfo betInfo, BaseClasses.BaseGreenSlotSpinResult lastResult, string strUserID, int websiteID, double userBalance, double betMoney, bool usePayLimit, Currencies currency = Currencies.USD)
        {
            BasePPSlotSpinData spinData = null;
            BaseClasses.BaseGreenSlotSpinResult result = null;

            string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);
            if (betInfo.HasRemainResponse)
            {
                BaseClasses.BaseGreenActionToResponse nextResponse = betInfo.pullRemainResponse();
                result = calculateResult(betInfo, lastResult, strGlobalUserID, nextResponse.Response,  false, userBalance, betInfo.PlayBet, 0, currency);
                betInfo.SpinInfo = nextResponse.Response;

                if (!betInfo.HasRemainResponse)
                    betInfo.RemainReponses = null;
                return result;
            }
            ++TestNumber;
            //double pointUnit = getPointUnit(betInfo);
            //double totalBet = betInfo.RelativeTotalBet * betInfo.PlayBet;
            double realBetMoney = betInfo.PlayBet;

            spinData = await selectRandomStop(websiteID, realBetMoney, betInfo);

            //test code
            if (TestNumber % 3 == 0)
            {
                int itest = 0;
                if (spinData.SpinType == 100)
                    itest = 1;

                if (spinData.SpinStrings.Count > 1)
                {
                    result = calculateResult(betInfo, lastResult, strGlobalUserID, spinData.SpinStrings[0], true, userBalance, betMoney, spinData.SpinOdd);
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
                    betInfo.SpinInfo = spinData.SpinStrings[0];
                    return result;
                }
            }
            // end test code

            double totalWin = 0;
            if (spinData.SpinOdd > 0)
                totalWin = (int)realBetMoney * spinData.SpinOdd;
            
            if (!usePayLimit || await checkWebsitePayoutRate(websiteID, realBetMoney, totalWin))
            {
                do
                {
                    result = calculateResult(betInfo, lastResult, strGlobalUserID, spinData.SpinStrings[0], true, userBalance, betMoney, spinData.SpinOdd, currency);
                    if (spinData.SpinStrings.Count > 1)
                        betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
                    betInfo.SpinInfo = spinData.SpinStrings[0];
                    return result;
                } while (false);
            }

            double emptyWin = 0.0;

            if (SupportPurchaseFree && betInfo.isPurchase)
            {
                spinData = await selectMinStartFreeSpinData(betInfo);
                result = calculateResult(betInfo, lastResult, strGlobalUserID, spinData.SpinStrings[0], true, userBalance, betMoney, spinData.SpinOdd, currency);
                emptyWin = realBetMoney * spinData.SpinOdd;

                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
            }
            else
            {
                spinData = await selectEmptySpin(betInfo);
                result = calculateResult(betInfo, lastResult, strGlobalUserID, spinData.SpinStrings[0], true, userBalance, betMoney, spinData.SpinOdd, currency);
            }

            sumUpWebsiteBetWin(websiteID, realBetMoney, emptyWin);
            betInfo.SpinInfo = spinData.SpinStrings[0];
            return result;
        }
        protected byte[] backupBetInfo(BaseGreenSlotBetInfo betInfo)
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
        protected virtual async Task spinGame(string strUserID, int websiteID, double userBalance, bool isNewBet, Currencies currency = Currencies.USD)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);

                BaseClasses.BaseGreenSlotBetInfo betInfo = null;
                if (!_dicUserBetInfos.TryGetValue(strGlobalUserID, out betInfo))
                    return;

                //byte[] betInfoBytes = backupBetInfo(betInfo);

                BaseClasses.BaseGreenSlotSpinResult lastResult = null;
                if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                    lastResult = _dicUserResultInfos[strGlobalUserID];

                double betMoney = betInfo.PlayBet;

                UserBetTypes betType = UserBetTypes.Normal;
                
                if (betInfo.HasRemainResponse || betInfo.GambleType > 0 || betInfo.GambleHalf)
                    betMoney = 0.0;

                if (userBalance.LT(betMoney, _epsilion) || betMoney < 0.0)
                {
                    GITMessage message = new GITMessage((ushort)SCMSG_CODE.SC_GREENTUBE_DOSPIN);
                    message.Append("-1User balance is less that bet money");
                    ToUserMessage toUserResult = new ToUserMessage((int)_gameID, message);
                    Sender.Tell(toUserResult, Self);
                    _logger.Error("user balance is less than bet money in BaseGreentubeSlotGame::spinGame {0} balance:{1}, bet money: {2} game id:{3}",
                        strGlobalUserID, userBalance, betMoney, _gameID);
                    return;
                }

                if (isNewBet)
                {
                    betInfo.BetTransactionID = createTransactionID();
                    betInfo.RoundID = createRoundID();
                }
                userBalance -= betMoney;

                BaseClasses.BaseGreenSlotSpinResult spinResult = await generateSpinResult(betInfo, lastResult, strUserID, websiteID, userBalance, betMoney, true, currency);

                string strGameLog = spinResult.ResultString;
                _dicUserResultInfos[strGlobalUserID] = spinResult;

                saveBetResultInfo(strGlobalUserID);

                sendGameResult(betInfo, spinResult, strGlobalUserID, betMoney, 0.0, strGameLog, userBalance, betType);

                //_dicUserLastBackupBetInfos[strGlobalUserID] = betInfoBytes;
                _dicUserLastBackupResultInfos[strGlobalUserID] = lastResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseGreentubeSlotGame::spinGame {0}", ex);
            }
        }
        protected virtual async Task gamblePickGame(string strUserID, int websiteID, double userbalance, Currencies currency = Currencies.USD)
        {
            string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);

            BaseClasses.BaseGreenSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
            BaseClasses.BaseGreenSlotSpinResult lastResult = _dicUserResultInfos[strGlobalUserID];
            string strSpinResponse = lastResult.ResultString;

            int cardSymbol = await selectCardSymbol(websiteID, betInfo, lastResult);

            int winMultiple = 0;
            if (betInfo.GambleType == (int)GambleTypes.Red && (cardSymbol == (int)CardTypes.Diamond || cardSymbol == (int)CardTypes.Heart))
                winMultiple = 2;
            else if (betInfo.GambleType == (int)GambleTypes.Black && (cardSymbol == (int)CardTypes.Club || cardSymbol == (int)CardTypes.Spade))
                winMultiple = 2;

            if (cardSymbol == (int)CardTypes.Heart)
                betInfo.GambleCardHistory.Add("H");
            else if (cardSymbol == (int)CardTypes.Diamond)
                betInfo.GambleCardHistory.Add("D");
            else if (cardSymbol == (int)CardTypes.Club)
                betInfo.GambleCardHistory.Add("C");
            else if (cardSymbol == (int)CardTypes.Spade)
                betInfo.GambleCardHistory.Add("S");

            if(lastResult.CollectWin > 0)
            {
                if (winMultiple == 0)
                    lastResult.CollectWin -= lastResult.TotalWin;
                if (winMultiple == 2)
                    lastResult.CollectWin += lastResult.TotalWin;
            }
            lastResult.TotalWin = (double)lastResult.TotalWin * winMultiple;
            lastResult.Action = GreenMessageType.GamblePick;

            strSpinResponse = buildGambleResultResMsgString(strGlobalUserID, userbalance, betInfo, lastResult, lastResult.Action, currency);

            lastResult.ResultString = strSpinResponse;
            
            saveBetResultInfo(strGlobalUserID);
            _dicUserResultInfos[strGlobalUserID] = lastResult;
            sendGamblePickResult(lastResult, strGlobalUserID, 0.0, 0.0, "GamblePick", userbalance);
            _dicUserLastBackupResultInfos[strGlobalUserID] = lastResult;
        }
        private async Task<int> selectCardSymbol(int websiteID, BaseClasses.BaseGreenSlotBetInfo betInfo, BaseClasses.BaseGreenSlotSpinResult lastResult)
        {
            int cardSymbol = (int)CardTypes.Diamond;

            double payoutRate = getPayoutRate(websiteID);
            double randomDouble = Pcg.Default.NextDouble(0.0, 100.0);

            if (randomDouble >= payoutRate || payoutRate == 0.0)
            {
                if (betInfo.GambleType == (int)GambleTypes.Red)
                {
                    List<int> noRedSymbol = new List<int>() { (int)CardTypes.Club, (int)CardTypes.Spade };
                    cardSymbol = noRedSymbol[Pcg.Default.Next(0, 2)];
                }
                else if (betInfo.GambleType == (int)GambleTypes.Black)
                {
                    List<int> noBlackSymbol = new List<int>() { (int)CardTypes.Diamond, (int)CardTypes.Heart };
                    cardSymbol = noBlackSymbol[Pcg.Default.Next(0, 2)];
                }
            }
            else
            {
                if (!await checkWebsitePayoutRate(websiteID, lastResult.TotalWin, lastResult.TotalWin * 2, 0))
                {
                    if (betInfo.GambleType == (int)GambleTypes.Red)
                    {
                        List<int> noRedSymbol = new List<int>() { (int)CardTypes.Club, (int)CardTypes.Spade };
                        cardSymbol = noRedSymbol[Pcg.Default.Next(0, 2)];
                    }
                    else if (betInfo.GambleType == (int)GambleTypes.Black)
                    {
                        List<int> noBlackSymbol = new List<int>() { (int)CardTypes.Diamond, (int)CardTypes.Heart };
                        cardSymbol = noBlackSymbol[Pcg.Default.Next(0, 2)];
                    }
                }
                else 
                {
                    cardSymbol = Pcg.Default.Next(0, 4);
                
                    if (betInfo.GambleType == (int)GambleTypes.Red && (cardSymbol != (int)CardTypes.Diamond && cardSymbol != (int)CardTypes.Heart))
                        sumUpWebsiteBetWin(websiteID, 0, -lastResult.TotalWin * 2, 0);
                    else if (betInfo.GambleType == (int)GambleTypes.Black && (cardSymbol != (int)CardTypes.Club && cardSymbol != (int)CardTypes.Spade))
                        sumUpWebsiteBetWin(websiteID, 0, -lastResult.TotalWin * 2, 0);
                }
            }

            return cardSymbol;
        }
        protected virtual void sendGameResult(BaseClasses.BaseGreenSlotBetInfo betInfo, BaseClasses.BaseGreenSlotSpinResult spinResult, string strGlobalUserID, double betMoney, double winMoney, string strGameLog, double userBalance, UserBetTypes betType)
        {
            GITMessage message = new GITMessage((ushort)SCMSG_CODE.SC_GREENTUBE_DOSPIN);
            //message.Append(spinResult.ResultString);
            if (!betInfo.HasRemainResponse)
                message.Append("%t0ÿq0");
            message.Append(string.Format("\u001f{0}", betInfo.PlayBet));
            message.Append(spinResult.ResultString);
            if(!betInfo.HasRemainResponse)
                message.Append("\u001d");
            ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, message, betMoney, winMoney, new GameLogInfo(GameName, "0", strGameLog), UserBetTypes.Normal);

            toUserResult.BetTransactionID = betInfo.BetTransactionID;
            toUserResult.RoundID = betInfo.RoundID;

            if (!betInfo.HasRemainResponse && spinResult.TotalWin == 0)
            {
                toUserResult.TransactionID = createTransactionID();
                toUserResult.RoundEnd = true;
            }

            Sender.Tell(toUserResult, Self);
        }
        protected virtual void sendGamblePickResult(BaseClasses.BaseGreenSlotSpinResult spinResult, string strGlobalUserID, double betMoney, double winMoney, string strGameLog, double userBalance)
        {
            BaseClasses.BaseGreenSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
            GITMessage message = new GITMessage((ushort)SCMSG_CODE.SC_GREENTUBE_DOGAMBLEPICK);
            message.Append(spinResult.ResultString);

            ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, message, betMoney, winMoney, new GameLogInfo(GameName, "0", strGameLog), UserBetTypes.Normal);
            toUserResult.BetTransactionID = betInfo.BetTransactionID;
            toUserResult.RoundID = betInfo.RoundID;
            toUserResult.TransactionID = createTransactionID();
            toUserResult.RoundEnd = false;
            if (spinResult.WinMoney == 0)
                toUserResult.RoundEnd = true;

            saveBetResultInfo(strGlobalUserID);
            Sender.Tell(toUserResult, Self);
        }
        protected virtual void sendGambleHalfResult(BaseClasses.BaseGreenSlotSpinResult spinResult, string strGlobalUserID, double betMoney, double winMoney, string strGameLog, double userBalance)
        {
            BaseClasses.BaseGreenSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
            GITMessage message = new GITMessage((ushort)SCMSG_CODE.SC_GREENTUBE_DOGAMBLEHALF);
            message.Append(spinResult.ResultString);

            ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, message, betMoney, winMoney, new GameLogInfo(GameName, "0", strGameLog), UserBetTypes.Normal);
            toUserResult.BetTransactionID = betInfo.BetTransactionID;
            toUserResult.RoundID = betInfo.RoundID;
            toUserResult.TransactionID = createTransactionID();
            toUserResult.RoundEnd = false;

            saveBetResultInfo(strGlobalUserID);
            Sender.Tell(toUserResult, Self);
        }

        #region
        protected virtual async Task<BasePPSlotSpinData> selectRandomStop(int websiteID, BaseClasses.BaseGreenSlotBetInfo betInfo)
        {
            OddAndIDData selectedOddAndID = selectRandomOddAndID(websiteID, betInfo);

            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, selectedOddAndID.ID), TimeSpan.FromSeconds(10.0));
            return convertBsonToSpinData(spinDataDocument);
        }
        protected virtual OddAndIDData selectRandomOddAndID(int websiteID, BaseClasses.BaseGreenSlotBetInfo betInfo)
        {
            double payoutRate = getPayoutRate(websiteID);
            double randomDouble = Pcg.Default.NextDouble(0.0, 100.0);

            int selectedID = 0;
            if (randomDouble >= payoutRate || payoutRate == 0.0)
            {
                selectedID = Pcg.Default.Next(_emptySpinIndex[betInfo.PlayLine][0], _emptySpinIndex[betInfo.PlayLine][1] + 1);
            }
            else
            {
                //selectedID = Pcg.Default.Next(1, _naturalSpinCount + 1);
                selectedID = Pcg.Default.Next(1, (_emptySpinIndex[betInfo.PlayLine][1] - _emptySpinIndex[betInfo.PlayLine][0] + 1) + _naturalSpinCount[betInfo.PlayLine] + 1);
                int emptyCount = _emptySpinIndex[betInfo.PlayLine][1] - _emptySpinIndex[betInfo.PlayLine][0] + 1;
                if (selectedID > emptyCount)
                    selectedID = selectedID - emptyCount + _normalStartId[betInfo.PlayLine] - 1;
                else
                    selectedID = _emptySpinIndex[betInfo.PlayLine][0] + selectedID - 1;
            }

            OddAndIDData selectedOddAndID = new OddAndIDData();
            selectedOddAndID.ID = selectedID;
            return selectedOddAndID;
        }
        protected virtual async Task<BasePPSlotSpinData> selectEmptySpin(BaseClasses.BaseGreenSlotBetInfo betInfo)
        {
            int id = Pcg.Default.Next(_emptySpinIndex[betInfo.PlayLine][0], _emptySpinIndex[betInfo.PlayLine][1] + 1);
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, id), TimeSpan.FromSeconds(10.0));
            return convertBsonToSpinData(spinDataDocument);
        }
        protected virtual BasePPSlotSpinData convertBsonToSpinData(BsonDocument document)
        {
            int spinType = (int)document["spintype"];
            double spinOdd = (double)document["odd"];
            string strData = (string)document["data"];

            List<string> spinResponses = new List<string>(strData.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));

            return new BasePPSlotSpinData(spinType, spinOdd, spinResponses);
        }
        protected virtual async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BaseClasses.BaseGreenSlotBetInfo betInfo)
        {
            try
            {
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.GENERAL, 0, _normalStartId[betInfo.PlayLine], _normalMaxID[betInfo.PlayLine]),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected virtual async Task<BasePPSlotSpinData> selectMinStartFreeSpinData(BaseClasses.BaseGreenSlotBetInfo betInfo)
        {
            try
            {
                BsonDocument spinDataDocument = null;
                 spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                            new SelectSpinTypeOddRangeRequest(GameName, 1, PurchaseFreeMultiple * 0.2, PurchaseFreeMultiple * 0.5), TimeSpan.FromSeconds(10.0));
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        #endregion
        
        protected virtual async Task<BasePPSlotSpinData> selectRandomStop(int websiteID, double baseBet, BaseClasses.BaseGreenSlotBetInfo betInfo)
        {
            //test for freespin
            if (TestNumber % 3 == 0)
                return await selectRandomStartFreeSpinData(betInfo);
           
            return await selectRandomStop(websiteID, betInfo);
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
                        BinaryReader reader = new BinaryReader(stream);
                        BaseClasses.BaseGreenSlotBetInfo betInfo = restoreBetInfo(strGlobalUserID, reader);
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
                        BaseClasses.BaseGreenSlotSpinResult resultInfo = restoreResultInfo(strGlobalUserID, reader);
                        if (resultInfo != null)
                            _dicUserResultInfos[strGlobalUserID] = resultInfo;
                    }
                }
            }

            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseGreentubeSlotGame::loadUserHistoricalData {0}", ex);
                return false;
            }
            return await base.loadUserHistoricalData(strGlobalUserID, isNewEnter);
        }
        protected virtual BaseClasses.BaseGreenSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            BaseClasses.BaseGreenSlotBetInfo betInfo = new BaseClasses.BaseGreenSlotBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected virtual BaseClasses.BaseGreenSlotSpinResult restoreResultInfo(string strGlobalUserID, BinaryReader reader)
        {
            BaseClasses.BaseGreenSlotSpinResult result = new BaseClasses.BaseGreenSlotSpinResult();
            result.SerializeFrom(reader);
            return result;
        }
        protected virtual void saveBetResultInfo(string strGlobalUserID)
        {
            try
            {
                if (_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    byte[] betInfoBytes = _dicUserBetInfos[strGlobalUserID].convertToByte();
                    _redisWriter.Tell(new UserBetInfoWrite(strGlobalUserID, _gameID, betInfoBytes, false), Self);
                }
                else
                {
                    _redisWriter.Tell(new UserBetInfoWrite(strGlobalUserID, _gameID, null, false), Self);
                }
                if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    byte[] resultInfoBytes = _dicUserResultInfos[strGlobalUserID].convertToByte();
                    _redisWriter.Tell(new UserResultInfoWrite(strGlobalUserID, _gameID, resultInfoBytes, false), Self);
                }
                else
                {
                    _redisWriter.Tell(new UserResultInfoWrite(strGlobalUserID, _gameID, null, false), Self);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseGreentubeSlotGame::saveBetInfo {0}", ex);
            }
        }
        protected override async Task onExitUserMessage(ExitGameRequest message)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", message.WebsiteID, message.UserID);
                BaseClasses.BaseGreenSlotBetInfo betInfo = null;

                if (_dicUserBetInfos.ContainsKey(strGlobalUserID))
                    betInfo = _dicUserBetInfos[strGlobalUserID];

                if (betInfo == null)
                {
                    Sender.Tell(new GreenExitResponse(null));
                    return;
                }
                string betTransactionID = betInfo.BetTransactionID;
                string roundID = betInfo.RoundID;

                double winMoney = await procUserExitGame(strGlobalUserID, message.Balance, message.Currency, message.UserRequested);

                _dicEnteredUsers.Remove(strGlobalUserID);
                if (winMoney == 0.0)
                {
                    Sender.Tell(new GreenExitResponse(null));
                    return;
                }

                ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, null, 0.0, winMoney, new GameLogInfo(GameName, "0", "Collect"), UserBetTypes.Normal);
                toUserResult.BetTransactionID = betTransactionID;
                toUserResult.RoundID = roundID;
                toUserResult.TransactionID = createTransactionID();
                toUserResult.RoundEnd = true;
                Sender.Tell(new GreenExitResponse(toUserResult));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseGreentubeSlotGame::onExitUserMessage {0}", ex);
            }
        }
        protected async Task<double> procUserExitGame(string strGlobalUserID, double userbalance, Currencies currency, bool userRequested)
        {
            try
            {
                double winMoney = 0.0;

                if (_dicUserResultInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos[strGlobalUserID].Action != GreenMessageType.Collect)
                {
                    BaseClasses.BaseGreenSlotSpinResult result = _dicUserResultInfos[strGlobalUserID];
                    winMoney = result.TotalWin;

                    if (!_dicUserBetInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                    {
                        //string strResponse = buildResMsgString(strGlobalUserID, userbalance, 0, new BaseGreenSlotBetInfo() { CurrencyInfo = currency }, "", GreenMessageType.Collect);
                        string strResponse = "user exit";
                        result.Action = GreenMessageType.Collect;
                        result.TotalWin = 0;
                    }

                    byte[] resultInfoBytes = _dicUserResultInfos[strGlobalUserID].convertToByte();
                    await _redisWriter.Ask(new UserResultInfoWrite(strGlobalUserID, _gameID, resultInfoBytes, true));
                    _dicUserResultInfos.Remove(strGlobalUserID);
                }

                if (_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    if (_dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                    {
                        BaseClasses.BaseGreenSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
                        if (betInfo.HasRemainResponse)
                            winMoney = 0.0;
                    }

                    byte[] betInfoBytes = _dicUserBetInfos[strGlobalUserID].convertToByte();
                    await _redisWriter.Ask(new UserBetInfoWrite(strGlobalUserID, _gameID, betInfoBytes, true));
                    _dicUserBetInfos.Remove(strGlobalUserID);
                }

                _dicUserLastBackupResultInfos.Remove(strGlobalUserID);
                _dicUserLastBackupBetInfos.Remove(strGlobalUserID);
                return winMoney;

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseGreentubeSlotGame::onUserExitGame GameID:{0} {1}", _gameID, ex);
                return 0.0;
            }
        }
        protected virtual string buildInitString(string strGlobalUserID, double balance, Currencies currency)
        {
            string initString = string.Empty;

            initString = string.Format(" TICKETSÿ{0}ÿMINOFFERÿ50ÿINDEPENDENTLINESANDBETSÿ1ÿCCODEÿruÿMAXLIMITÿ50000ÿREALCURRENCYFACTORÿ100.000000ÿREALMONEYCURRENCYCODEISOÿEURÿTOURNISSUBSCRIBEDÿ0ÿNATIVEJACKPOTCURRENCYÿEURÿISSERVERAUTOREBUYÿ0ÿMINBUYINÿ1ÿPAYOUTRATEÿ95.02%ÿALLOWCATEGORYÿ0,1,3,4,5ÿNATIVEJACKPOTCURRENCYFACTORÿ100.000000ÿUSERIDÿ3ÿLANGUAGEIDÿ2ÿJACKPOTFEEPERCENTAGEÿ0.0000ÿMAXBUYINÿ2147483647ÿLIMITÿ50000ÿREALMONEYÿ0ÿERRNUMBERÿ42ÿOFFERÿ50ÿPLAYERBALANCEÿ{1}ÿDISALLOWRECONNECTAFTERINACTIVITYÿ0ÿAUTOKICKÿ0ÿCLIENT_TO_WRAPPERÿ1ÿISDEEPWALLETÿ1ÿSITEIDÿ2369ÿRELIABILITYÿ14ÿWRAPPER_TO_CLIENTÿ1ÿNEEDSSESSIONTIMEOUTÿ0ÿLANGUAGEISOCODEÿENÿEXTUSERIDÿ350897482ÿTICKETTYPEÿ1ÿCURRENCYFACTORÿ100ÿSESSIONTIMEOUTMINUTESÿ20ÿNICKNAMEÿ#831070350897400ÿREALCURRENCYÿEURÿMINLIMITÿ50", balance, balance);
            return initString;
        }

        protected virtual string buildLineBetString(string strGlobalUserID, double balance)
        {
            return "";
        }
    }

    public enum GambleTypes
    {
        Red = 1,
        Black = 2,
        Diamond = 3,
        Heart = 4,
        Crub = 5,
        Spade = 6,
    }

    public enum CardTypes
    {
        Diamond = 0,
        Heart = 1,
        Club = 2,
        Spade = 3,
    }
}
