using Akka.Actor;
using GITProtocol;
using Newtonsoft.Json.Linq;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    public class LuckyDogResult : BasePPSlotSpinResult
    {
        public List<int> BonusSelections { get; set; }

        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            BonusSelections = SerializeUtils.readIntList(reader);
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            SerializeUtils.writeIntList(writer, BonusSelections);
        }
    }

    class LuckyDogGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs5luckydogly";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 5; }
        }
        protected override int ServerResLineCount
        {
            get { return 5; }
        }
        protected override int ROWS
        {
            get
            {
                return 3;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=7,7,5,4,7,4,4,3,3&cfgs=1&ver=3&def_sb=4,4,7&reel_set_size=2&def_sa=7,7,6&scatters=1~0,0,0~0,0,0~1,1,1&rt=d&gameInfo={rtps:{regular:\"96.50\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"330579\",max_rnd_win:\"1000\"}}&wl_i=tbm~1000&sc=40.00,80.00,120.00,160.00,200.00,400.00,600.00,800.00,1000.00,1500.00,2000.00,3000.00,5000.00,10000.00,15000.00,20000.00&defc=200.00&wilds=2~250,0,0~1,1,1&bonuses=0&ntp=0.00&paytable=0,0,0;0,0,0;0,0,0;100,0,0;15,0,0;10,0,0;6,0,0;4,0,0;3,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;0,0,0&reel_set0=8,7,7,8,4,4,4,4,5,7,3,2,2,2,7,3,7,5,4,5,5,5,8,3,3,3,6,8,6,7,5,5,8,6,6,6,6,6,4,7,8,8,7,8,6,7,7,7,8,7,7,2,6,6,7,6,8,8,8,8,8,7,7,2,5,7,7,8,5,7,7,7,8,6,7,5,8,6,4,4~7,8,8,8,7,5,6,2,8,7,3,3,3,4,6,5,7,8,4,4,4,4,5,7,8,3,2,8,5,6,6,6,7,8,8,4,8,8,7,3,8,7,7,7,7,8,2,8,8,7,5,8,8,7,8,3,5,7,5,8,8,8,8,7,2,2,2,5,3,8,3,6,4,8,4,4,5,2,8,7,5,5,5,8,7,8~8,8,6,6,7,6,6,6,3,4,8,7,7,8,8,3,3,3,2,5,5,5,3,6,4,4,4,7,8,6,2,7,3,6,6,6,6,6,7,8,7,4,6,6,7,7,7,6,4,7,8,8,7,5,8,8,8,8,8,8,6,2,2,2,5,6,5,6,6,8,6,7&reel_set1=7,7,4,2,2,2,7,5,7,6,8,7,7,4,4,4,8,7,6,7,5,7,8,8,7,7,7,7,3,7,8,7,7,8,7,7,7,7,7,5,7,7,7,6,7,3,3,3,6,7,7,7,5,7,7,7,6,6,6,4,7,5,7,8,8,8,5,6,7,6,7,8,7,7,7,7,3,6,7,7,4,7,7,8,2,2,2,7,7,8,5,5,5,7,7,7,7,8,8,7,8,7,6,7,7,7,8,7,6,7,7~6,8,2,6,6,7,6,2,8,6,8,7,7,7,2,6,4,4,4,4,5,6,6,8,6,5,6,6,2,6,6,5,8,8,2,2,2,6,6,8,8,7,8,8,4,8,5,7,6,6,6,6,6,2,2,2,7,6,5,7,8,6,6,7,3,3,3,2,8,7,6,6,8,8,2,6,7,8,6,8,6,6,3,8,6,6,4,6,2,6,5,5,5,3,6,6,6,8,4,6,6,7,6,8,8,8,6,7,6,7,2,6,5,6,6,8,5,8,8,7,6,8,6,6,7,8,6,7,6,8,6,7,6,2,6,7,8,6,8,2,7,2,6,6,8,6,6,6~8,6,8,8,3,8,6,6,6,6,6,7,8,7,8,8,7,6,8,8,8,8,7,7,7,8,6,8,7,8,5,2,2,2,8,8,7,3,3,3,8,8,6,7,7,7,7,7,6,7,8,8,6,6,6,8,8,7,8,8,4,4,4,4,4,8,8,8,7,5,5,5,5,6,8,8,7,8,8,2,2,2,3,8,7,8,8,7,8,8,7,7,8,8,8,8,8,8,4,7,8,8,5,8,8,7,8,8,5,8,8,6,8,8,8,7,4,8,6,7,7";
            }
        }
        #endregion
        public LuckyDogGameLogic()
        {
            _gameID     = GAMEID.LuckyDog;
            GameName    = "LuckyDog";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"]   = "0";
	        dicParams["st"]         = "rect";
	        dicParams["sw"]         = "3";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("g"))
            {
                var gParam = JToken.Parse(dicParams["g"]);
                if (gParam["bg_0"] != null)
                {
                    if (gParam["bg_0"]["rw"] != null)
                        gParam["bg_0"]["rw"] = convertWinByBet(gParam["bg_0"]["rw"].ToString(), currentBet);
                }

                dicParams["g"] = serializeJsonSpecial(gParam);
            }
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "s", "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set", "bw" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]))
                    continue;
                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }

            if (!resultParams.ContainsKey("g") && spinParams.ContainsKey("g"))
                resultParams["g"] = spinParams["g"];
            return resultParams;
        }
        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst)
        {
            try
            {
                LuckyDogResult spinResult = new LuckyDogResult();
                Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(dicParams, betInfo.TotalBet);

                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                spinResult.NextAction = convertStringToActionType(dicParams["na"]);
                if (spinResult.NextAction == ActionTypes.DOCOLLECT || spinResult.NextAction == ActionTypes.DOCOLLECTBONUS)
                    spinResult.TotalWin = double.Parse(dicParams["tw"]);
                else
                    spinResult.TotalWin = 0.0;

                spinResult.ResultString = convertKeyValuesToString(dicParams);

                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in LuckyDogGameLogic::calculateResult {0}", ex);
                return null;
            }
        }
        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            LuckyDogResult result = new LuckyDogResult();
            result.SerializeFrom(reader);
            return result;
        }
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                int ind     = (int)message.Pop();

                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                double              realWin    = 0.0;
                string              strGameLog = "";
                ToUserResultMessage resultMsg  = null;

                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) ||
                    !_dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    LuckyDogResult result                   = _dicUserResultInfos[strGlobalUserID] as LuckyDogResult;
                    BasePPSlotBetInfo betInfo               = _dicUserBetInfos[strGlobalUserID];
                    BasePPActionToResponse actionResponse   = betInfo.pullRemainResponse();
                    if (actionResponse.ActionType != ActionTypes.DOBONUS)
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        Dictionary<string, string> dicParams = splitResponseToParams(actionResponse.Response);

                        convertWinsByBet(dicParams, betInfo.TotalBet);
                        convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                        if (result.BonusSelections == null)
                            result.BonusSelections = new List<int>();

                        if(result.BonusSelections.Contains(ind))
                        {
                            betInfo.pushFrontResponse(actionResponse);
                            saveBetResultInfo(strGlobalUserID);
                            throw new Exception(string.Format("{0} User selected already selected position, Malicious Behavior {1}", strGlobalUserID, ind));
                        }

                        result.BonusSelections.Add(ind);

                        if (dicParams.ContainsKey("g"))
                        {
                            var gParam = JToken.Parse(dicParams["g"]);
                            if (gParam["bg_0"] != null)
                            {
                                if (gParam["bg_0"]["ch_h"] != null)
                                {
                                    string strChh = string.Format("0~{0}", result.BonusSelections[0]);
                                    gParam["bg_0"]["ch_h"] = strChh;
                                }

                                if (gParam["bg_0"]["wins_mask"] != null)
                                {
                                    string[] strWinsMask    = gParam["bg_0"]["wins_mask"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    string[] strNewWinsMask = new string[4];

                                    for (int i = 0; i < 4; i++)
                                        strNewWinsMask[i] = strWinsMask[i];

                                    for (int i = 0; i < result.BonusSelections.Count; i++)
                                        strNewWinsMask[result.BonusSelections[i]] = strWinsMask[i];

                                    gParam["bg_0"]["wins_mask"] = string.Join(",", strNewWinsMask);
                                }

                                if (gParam["bg_0"]["wins"] != null)
                                {
                                    string[] strWins    = gParam["bg_0"]["wins"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    string[] strNewWins = new string[4];

                                    for (int i = 0; i < 4; i++)
                                        strNewWins[i] = strWins[i];

                                    for (int i = 0; i < result.BonusSelections.Count; i++)
                                        strNewWins[result.BonusSelections[i]] = strWins[i];

                                    gParam["bg_0"]["wins"] = string.Join(",", strNewWins);
                                }
                            }
                            dicParams["g"] = serializeJsonSpecial(gParam);
                        }

                        result.BonusResultString = convertKeyValuesToString(dicParams);
                        addDefaultParams(dicParams, userBalance, index, counter);

                        ActionTypes nextAction = convertStringToActionType(dicParams["na"]);
                        string strResponse = convertKeyValuesToString(dicParams);

                        responseMessage.Append(strResponse);

                        //히스토리보관 및 초기화
                        if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                            addActionHistory(strGlobalUserID, "doBonus", strResponse, index, counter);

                        if (nextAction == ActionTypes.DOCOLLECT || nextAction == ActionTypes.DOCOLLECTBONUS)
                        {
                            realWin = double.Parse(dicParams["tw"]);
                            strGameLog = strResponse;

                            if (realWin > 0.0f)
                            {
                                _dicUserHistory[strGlobalUserID].baseBet = betInfo.TotalBet;
                                _dicUserHistory[strGlobalUserID].win = realWin;
                            }
                            resultMsg                   = new ToUserResultMessage((int)_gameID, responseMessage, 0.0, realWin, new GameLogInfo(this.GameName, "0", strGameLog), UserBetTypes.Normal); ;
                            resultMsg.BetTransactionID  = betInfo.BetTransactionID;
                            resultMsg.RoundID           = betInfo.RoundID;
                            resultMsg.TransactionID     = createTransactionID();
                        }
                        copyBonusParamsToResult(dicParams, result);
                        result.NextAction = nextAction;
                    }
                    if (!betInfo.HasRemainResponse)
                        betInfo.RemainReponses = null;

                    saveBetResultInfo(strGlobalUserID);
                }
                if (resultMsg == null)
                    Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
                else
                    Sender.Tell(resultMsg);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in LuckyDogGameLogic::onDoBonus {0}", ex);
            }
        }
    }
}
