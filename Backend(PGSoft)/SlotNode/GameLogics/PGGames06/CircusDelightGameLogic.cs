using Akka.Actor;
using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlotGamesNode.GameLogics
{
    class CircusDelightGameLogic : BasePGSlotGame
    {
        protected override bool SupportPurchaseFree
        {
            get { return false; }
        }
        protected override double DefaultBetSize
        {
            get { return 0.1; }
        }
        protected override int DefaultBetLevel
        {
            get { return 5; }
        }
        protected override int BaseBet
        {
            get
            {
                return 25;
            }
        }
        protected override string DefaultResult
        {
            get
            {
                return "{\"si\":{\"wp\":null,\"lw\":null,\"lwm\":null,\"stp\":null,\"sc\":0,\"orl\":null,\"ss\":0,\"fs\":null,\"rs\":null,\"bns\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.01,\"rl\":[2,7,3,8,1,9,0,0,0,8,1,9,2,7,3],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":22.12,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"lw\":null,\"lwm\":null,\"stp\":null,\"sc\":2,\"orl\":[2,7,3,8,11,9,4,12,2,7,4,12,5,6,9],\"ss\":0,\"fs\":null,\"rs\":null,\"bns\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.01,\"rl\":[2,7,3,8,11,9,4,12,2,7,4,12,5,6,9],\"sid\":\"1762870431754243584\",\"psid\":\"1762870431754243584\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.7,\"max\":96.7}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public CircusDelightGameLogic()
        {
            _gameID = GAMEID.CircusDelight;
            GameName = "CircusDelight";
        }
        protected override void initGameData()
        {
            base.initGameData();
            _pgGameConfig.ml.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            _pgGameConfig.cs.AddRange(new double[] { 0.01, 0.05, 0.1 });
        }
        protected override async Task onDoSpin(string strUserID, int websiteID, GITMessage message, UserBonus userBonus, double userBalance, Currencies currency, bool isAffiliate)
        {
            try
            {
                long    lastTransID     = (long)message.Pop();
                string  strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);

                readBetInfoFromMessage(message, strGlobalUserID);

                string  wk    = (string) message.Pop();
                bool    ig    = (bool)   message.Pop();
                int     gt    = (int)    message.Pop();
                int     ps    = (int)    message.Pop();
                int     nmSel = -1;
                if (message.DataNum > 0)
                    nmSel = (int)message.Pop();

                if (!_dicUserHistory.ContainsKey(strGlobalUserID))
                {
                    var history = new BasePGHistory((int)_gameID);
                    history.cc = currency.ToString();
                    _dicUserHistory.Add(strGlobalUserID, history);
                }
                if (nmSel == -1)
                    await spinGame(strUserID, websiteID, userBonus, userBalance, isAffiliate);
                else
                    onFreeNumMulSelect(strUserID, websiteID, userBalance, nmSel);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::onDoSpin GameID: {0}, {1}", _gameID, ex);
            }
        }
        private void onFreeNumMulSelect(string strUserID, int websiteID, double userBalance, int nmSel)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);
                if (!_dicUserBetInfos.ContainsKey(strGlobalUserID) || !_dicUserResultInfos.ContainsKey(strGlobalUserID) || nmSel < 0 || nmSel > 2)
                    return;

                var betInfo = _dicUserBetInfos[strGlobalUserID];
                if (!betInfo.HasRemainResponse)
                    return;

                BasePGResponse  response        = betInfo.pullRemainResponse();
                dynamic         jsonParams      = JsonConvert.DeserializeObject(response.Response);

                if (jsonParams["fs"] == null)
                    return;

                convertWinsByBet(jsonParams, betInfo.TotalBet);
                convertBetsByBet(jsonParams, betInfo.BetSize, betInfo.BetLevel, betInfo.TotalBet);

                jsonParams["psid"]  = betInfo.TransactionID.ToString();
                jsonParams["sid"]   = createTransactionID().ToString();
                jsonParams["blb"]   = Math.Round(userBalance, 2);
                jsonParams["blab"]  = Math.Round(userBalance, 2);
                jsonParams["bl"]    = Math.Round(userBalance, 2);
                if (!IsNullOrEmpty(jsonParams["fs"]["fsmls"]))
                {
                    var fsmls = jsonParams["fs"]["fsmls"] as JArray;
                    if(nmSel != 0)
                    {
                        var temp        = fsmls[0];
                        fsmls[0]        = fsmls[nmSel];
                        fsmls[nmSel]    = temp;
                    }
                    jsonParams["fs"]["fsmli"] = nmSel;
                }
                else if (!IsNullOrEmpty(jsonParams["fs"]["fsns"]))
                {
                    var fsns = jsonParams["fs"]["fsns"] as JArray;
                    if(nmSel != 0)
                    {
                        var temp    = fsns[0];
                        fsns[0]     = fsns[nmSel];
                        fsns[nmSel] = temp;
                    }
                    jsonParams["fs"]["fsni"] = nmSel;
                }

                BasePGSlotSpinResult newResult  = new BasePGSlotSpinResult();
                newResult.TotalWin              = 0.0;
                newResult.ResultString          = JsonConvert.SerializeObject(jsonParams);
                _dicUserResultInfos[strGlobalUserID]  = newResult;

                saveBetResultInfo(strGlobalUserID);
                addResultToHistory(strGlobalUserID, newResult);
                GITMessage responseMessage = new GITMessage(MsgCodes.SPIN);
                responseMessage.Append(newResult.ResultString);
                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in CircusDelightGameLogic::onFreeNumMulSelect {0}", ex);
            }
        }
        protected override void convertWinsByBet(dynamic jsonParams, float currentBet)
        {
            base.convertWinsByBet((object)jsonParams, currentBet);
            if (!IsNullOrEmpty(jsonParams["fs"]) && IsArrayOrObject(jsonParams["fs"]) && !IsNullOrEmpty(jsonParams["fs"]["fsaw"]))
                jsonParams["fs"]["fsaw"] = convertWinByBet((double)jsonParams["fs"]["fsaw"], currentBet);

        }

    }
}
