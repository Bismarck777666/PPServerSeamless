using GITProtocol;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Akka.Actor;

namespace SlotGamesNode.GameLogics
{
    class FlirtingScholarGameLogic : BasePGSlotGame
    {
        protected override bool SupportPurchaseFree
        {
            get { return false; }
        }
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
                return 20;
            }
        }
        protected override string DefaultResult
        {
            get
            {
                return "{\"si\":{\"wp\":null,\"lw\":null,\"fs\":null,\"orl\":[2,2,2,4,1,8,0,0,0,7,1,3,2,2,2],\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":5,\"cs\":0.01,\"rl\":[2,2,2,4,1,8,0,0,0,7,1,3,2,2,2],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":22.14,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":97.44,\"max\":97.44}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public FlirtingScholarGameLogic()
        {
            _gameID = GAMEID.FlirtingScholar;
            GameName = "FlirtingScholar";
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
        }
        protected override async Task onDoSpin(string strUserID, GITMessage message, UserBonus userBonus, double userBalance)
        {
            try
            {
                _isRewardedBonus = false;
                _bonusSendMessage = null;
                _rewardedBonusMoney = 0.0;

                long lastTransID = (long)message.Pop();
                readBetInfoFromMessage(message, strUserID);
                int fpSelId = -1;
                if (message.DataNum == 5)
                    fpSelId = (int) message.GetData(4);

                if (!_dicUserHistory.ContainsKey(strUserID))
                    _dicUserHistory.Add(strUserID, new BasePGHistory((int)_gameID));

                if (fpSelId == -1)
                    await spinGame(strUserID, userBonus, userBalance);
                else
                    onFreeSelect(strUserID, userBonus, userBalance, fpSelId);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::onDoSpin GameID: {0}, {1}", _gameID, ex);
            }
        }
        private void onFreeSelect(string strUserID, UserBonus userBonus, double userBalance, int fpSelId)
        {
            try
            {
                if (!_dicUserBetInfos.ContainsKey(strUserID) || !_dicUserResultInfos.ContainsKey(strUserID) || fpSelId < 0)
                    return;

                var betInfo = _dicUserBetInfos[strUserID];
                if (!betInfo.HasRemainResponse)
                    return;

                BasePGResponse response = betInfo.pullRemainResponse();
                dynamic jsonParams = JsonConvert.DeserializeObject(response.Response);

                if (jsonParams["fs"] == null)
                    return;

                convertWinsByBet(jsonParams, betInfo.TotalBet);
                convertBetsByBet(jsonParams, betInfo.BetSize, betInfo.BetLevel, betInfo.TotalBet);

                jsonParams["psid"]  = betInfo.TransactionID.ToString();
                jsonParams["sid"]   = createTransactionID().ToString();
                jsonParams["blb"]   = Math.Round(userBalance, 2);
                jsonParams["blab"]  = Math.Round(userBalance, 2);
                jsonParams["bl"]    = Math.Round(userBalance, 2);
                if (!IsNullOrEmpty(jsonParams["fs"]["bf"]))
                {
                    var bfArray = jsonParams["fs"]["bf"] as JArray;
                    int i = 0;
                    for (i = 0; i < bfArray.Count; i++)
                    {
                        int fp = (int) bfArray[i]["fp"];
                        if (fp == -1)
                            break;
                    }
                    if (i == 0)
                        return;
                    bfArray[i - 1]["fp"] = fpSelId;
                }
 
                BasePGSlotSpinResult newResult  = new BasePGSlotSpinResult();
                newResult.TotalWin              = 0.0;
                newResult.ResultString          = JsonConvert.SerializeObject(jsonParams);
                _dicUserResultInfos[strUserID]  = newResult;

                saveBetResultInfo(strUserID);
                addResultToHistory(strUserID, newResult);
                GITMessage responseMessage      = new GITMessage(MsgCodes.SPIN);
                responseMessage.Append(newResult.ResultString);
                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in FlirtingScholarGameLogic::onFreeNumMulSelect {0}", ex);
            }
        }

    }
}
