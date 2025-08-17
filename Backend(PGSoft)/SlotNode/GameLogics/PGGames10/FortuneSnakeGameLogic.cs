using GITProtocol;
using GITProtocol.Utils;
using Newtonsoft.Json.Linq;
using SlotGamesNode.GameLogics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendServer.GameLogics
{
    class FortuneSnakeBetInfo : BasePGSlotBetInfo
    {
        public bool AnteBet { get; set; }
        public FortuneSnakeBetInfo(int baseBet) : base(baseBet)
        {
        }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            this.AnteBet = reader.ReadBoolean();
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(this.AnteBet);
        }
    }

    internal class FortuneSnakeGameLogic : BasePGSlotGame
    {
        protected override double DefaultBetSize
        {
            get { return 0.2; }
        }
        protected override int DefaultBetLevel
        {
            get { return 5; }
        }
        protected override int BaseBet
        {
            get
            {
                return 10;
            }
        }
        protected override string DefaultResult
        {
            get
            {
                return "{\"si\":{\"wp\":null,\"lw\":null,\"irfs\":false,\"itr\":false,\"itff\":false,\"it\":false,\"ifsw\":false,\"gm\":0,\"crtw\":0,\"imw\":false,\"orl\":[4,4,2,99,5,5,5,7,5,4,5,99],\"rcs\":0,\"gwt\":-1,\"pmt\":null,\"ab\":null,\"ml\":1,\"cs\":0.1,\"rl\":[4,4,2,99,5,5,5,7,5,4,5,99],\"ctw\":0,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"fb\":null,\"sid\":\"1937437815612719619\",\"psid\":\"1937437815612719619\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":291603.67,\"blab\":291602.67,\"bl\":291602.67,\"tb\":1,\"tbb\":1,\"tw\":0,\"np\":-1,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"lw\":null,\"snww\":null,\"wpl\":null,\"ptbr\":null,\"ssaw\":0,\"orl\":[97,4,97,97,6,97,97,5,7,3,99,99,8,8,11,11,10,10,5,5,7,99,99,9,7,3,99,99,8,8,11,11,97,4,97,97,6,97,97,5],\"gm\":1,\"bpf\":{\"1\":[9,10,11],\"2\":[20,21,22],\"3\":[25,26,27]},\"bp\":{\"1\":[9,10,11],\"2\":[20,21,22],\"3\":[25,26,27]},\"mp\":[2,4,8,16,32,64,128,256,512,1024],\"mib\":-1,\"mi\":-1,\"rns\":null,\"twbm\":0,\"crtw\":0,\"imw\":false,\"sc\":0,\"now\":13824,\"nowpr\":[8,6,6,6,8],\"fs\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.5,\"rl\":[97,4,97,97,6,97,97,5,7,3,99,99,8,8,11,11,10,10,5,5,7,99,99,9,7,3,99,99,8,8,11,11,97,4,97,97,6,97,97,5],\"sid\":\"1849585488751627777\",\"psid\":\"1849585488751627777\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":16.42,\"blab\":16.42,\"bl\":16.42,\"tb\":0,\"tbb\":0,\"tw\":0,\"np\":0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.76,\"max\":96.76}},\"grtpi\":[{\"gt\":\"Default\",\"grtps\":[{\"t\":\"min\",\"tphr\":null,\"rtp\":96.76},{\"t\":\"max\",\"tphr\":null,\"rtp\":96.76}]}],\"ows\":{\"itare\":true,\"tart\":1,\"igare\":true,\"gart\":2160},\"jws\":null}";
            }
        }
        protected override bool SupportAnteBet 
        {
            get { return true; }
        }
        protected override double AnteBetMultiple
        {
            get { return 1.5; }
        }
        public FortuneSnakeGameLogic()
        {
            _gameID     = GAMEID.FortuneSnake;
            GameName    = "FortuneSnake";
        }
        protected override void initGameData()
        {
            base.initGameData();
            _pgGameConfig.ml.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            _pgGameConfig.cs.AddRange(new double[] { 0.02, 0.1, 0.2 });
        }
        protected override void convertWinsByBet(dynamic jsonParams, float currentBet)
        {
            base.convertWinsByBet((object)jsonParams, currentBet);
            if (!IsNullOrEmpty(jsonParams["ctw"]))
                jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["twbm"]))
                jsonParams["twbm"] = convertWinByBet((double)jsonParams["twbm"], currentBet);
            
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                FortuneSnakeBetInfo betInfo = new FortuneSnakeBetInfo(this.BaseBet);
                betInfo.BetSize         = (float)Math.Round((double)message.Pop(), 2);
                betInfo.BetLevel        = (int)message.Pop();
                betInfo.PurchaseFree    = (int)message.Pop() == 2;
                if(message.DataNum > 4 && (int)message.GetData(4) > 0)
                    betInfo.AnteBet         = true;

                if (betInfo.BetSize <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetSize <= 0 in FortuneSnakeGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetSize);
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
                    (oldBetInfo as FortuneSnakeBetInfo).AnteBet = betInfo.AnteBet;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in FortuneSnakeGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePGSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            FortuneSnakeBetInfo betInfo = new FortuneSnakeBetInfo(this.BaseBet);
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override async Task<BasePGSlotSpinResult> generateSpinResult(BasePGSlotBetInfo betInfo, string strUserID, int websiteID, UserBonus userBonus, double userBalance, bool isAffiliate)
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

            if (SupportAnteBet && (betInfo as FortuneSnakeBetInfo).AnteBet)
                realBetMoney = totalBet * getAnteBetMultiple(betInfo);

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
        protected override async Task spinGame(string strUserID, int websiteID, UserBonus userBonus, double userBalance, bool isAffiliate)
        {
            try
            {
                BasePGSlotBetInfo betInfo = null;
                string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);
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

                if (this.SupportAnteBet && (betInfo as FortuneSnakeBetInfo).AnteBet)
                    betMoney = Math.Round(betMoney * getAnteBetMultiple(betInfo), 2);

                if (userBalance.LT(betMoney, _epsilion) || betMoney < 0.0)
                {
                    GITMessage message = new GITMessage(MsgCodes.NOTENOUGHBALANCE);
                    message.Append(buildNotEnoughResult(userBalance, betInfo));
                    Sender.Tell(new ToUserMessage((int)_gameID, message), Self);
                    return;
                }

                BasePGSlotSpinResult spinResult = await this.generateSpinResult(betInfo, strUserID, websiteID, userBonus, userBalance, isAffiliate);
                _dicUserResultInfos[strGlobalUserID] = spinResult;

                saveBetResultInfo(strGlobalUserID);
                addResultToHistory(strGlobalUserID, spinResult);

                if (!betInfo.HasRemainResponse && (betInfo.SpinData == null || !(betInfo.SpinData is BasePGSlotStartSpinData)))
                {
                    var historyItem = saveHistory(websiteID, strUserID);
                    sendGameResult(betInfo, spinResult, betMoney, spinResult.WinMoney, userBalance, true, historyItem);
                }
                else
                {
                    sendGameResult(betInfo, spinResult, betMoney, spinResult.WinMoney, userBalance, false, null);
                }
                _dicUserLastBackupBetInfos[strGlobalUserID]     = betInfoBytes;
                _dicUserLastBackupResultInfos[strGlobalUserID]  = lastResult;
                _dicUserLastBackupHistory[strGlobalUserID]      = historyBytes;

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in FortuneSnakeGameLogic::spinGame {0}", ex);
            }
        }
        protected override void overideSomeParam(BasePGSlotBetInfo betInfo, dynamic jsonParams)
        {
            FortuneSnakeBetInfo fortuneSnakeBetInfo = (FortuneSnakeBetInfo)betInfo;
            if (fortuneSnakeBetInfo.AnteBet)
                jsonParams["ab"] = 1;
        }
    }
}
