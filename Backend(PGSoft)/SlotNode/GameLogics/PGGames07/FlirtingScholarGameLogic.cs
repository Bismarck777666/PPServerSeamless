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
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"lw\":null,\"fs\":null,\"orl\":[2,2,2,4,9,8,5,0,6,7,10,3,2,2,2],\"gwt\":-1,\"fb\":null,\"ctw\":1.0,\"pmt\":null,\"cwc\":1,\"fstc\":null,\"pcwc\":1,\"rwsp\":{\"2\":20.0},\"hashr\":null,\"ml\":5,\"cs\":0.01,\"rl\":[2,2,2,4,9,8,5,0,6,7,10,3,2,2,2],\"sid\":\"1762870829831483908\",\"psid\":\"1762870829831483908\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
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
        protected override async Task onDoSpin(string strUserID, int websiteID, GITMessage message, UserBonus userBonus, double userBalance, Currencies currency, bool isAffiliate)
        {
            try
            {
                long lastTransID        = (long)message.Pop();
                string strGlobalUserID  = string.Format("{0}_{1}", websiteID, strUserID);
                readBetInfoFromMessage(message, strGlobalUserID);
                int fpSelId = -1;
                if (message.DataNum == 5)
                    fpSelId = (int) message.GetData(4);

                if (!_dicUserHistory.ContainsKey(strGlobalUserID))
                {
                    var history = new BasePGHistory((int)_gameID);
                    history.cc = currency.ToString();
                    _dicUserHistory.Add(strGlobalUserID, history);
                }

                if (fpSelId == -1)
                    await spinGame(strUserID, websiteID, userBonus, userBalance, isAffiliate);
                else
                    onFreeSelect(strUserID, websiteID, userBalance, fpSelId);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::onDoSpin GameID: {0}, {1}", _gameID, ex);
            }
        }
        private void onFreeSelect(string strUserID, int websiteID, double userBalance, int fpSelId)
        {
            try
            {
                var strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);
                if (!_dicUserBetInfos.ContainsKey(strGlobalUserID) || !_dicUserResultInfos.ContainsKey(strGlobalUserID) || fpSelId < 0)
                    return;

                var betInfo = _dicUserBetInfos[strGlobalUserID];
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
                _dicUserResultInfos[strGlobalUserID]  = newResult;

                saveBetResultInfo(strGlobalUserID);
                addResultToHistory(strGlobalUserID, newResult);
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
