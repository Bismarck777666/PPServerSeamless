using Akka.Actor;
using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    public class LeprechaunCarolResult : BasePPSlotSpinResult
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
    class LeprechaunCarolGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20leprexmas";
            }
        }
        protected override bool SupportReplay
        {
            get
            {
                return false;
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 20; }
        }
        protected override int ServerResLineCount
        {
            get { return 20; }
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
                return "def_s=3,3,7,7,4,11,5,6,7,11,10,5,9,8,8&cfgs=1978&ver=2&reel_set_size=6&def_sb=7,11,8,10,6&def_sa=6,3,5,8,11&prg_cfg_m=cp,wm,s&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&base_aw=tt~rwf;t;tt~rwgsf;n&prg_cfg=0,4,12&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&wilds=2~1000,400,100,4,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;1000,200,50,2,0;500,150,40,0,0;250,100,30,0,0;200,75,20,0,0;175,50,10,0,0;150,30,10,0,0;125,20,8,0,0;100,10,6,0,0;100,10,4,0,0;0,0,0,0,0&reel_set0=9,8,6,3,3,3,11,5,10,5,2,4,3,3,3,10,1,7,7,8,11,1,9,10,6~7,10,9,7,11,4,3,3,3,10,8,3,3,3,11,9,10,6,11,5,2,8,9,6~2,9,4,11,1,6,10,9,7,7,4,3,3,3,8,10,8,11,5,10,5,3,3,3,6~8,7,3,3,3,10,7,6,11,10,7,11,8,11,4,9,7,11,3,3,3,9,5,10,8,6,8,9,2,9,10~1,11,4,9,7,1,3,3,3,8,10,10,5,6,6,8,2,7,11,9,9,11,7,8,8,10,7,3,3,3,10,9&reel_set2=2,9,3,2,10,7,2,11,8,2,5,4,6~2,9,2,3,10,8,2,7,11,2,6,4,5~2,3,4,5,6,7,8,9,10,11~2,11,8,2,9,7,2,3,6,4,2,10,5~2,5,11,8,9,2,11,4,7,2,10,6,9,8,7,10,4,3&reel_set1=10,7,10,6,9,3,3,3,11,8,8,10,11,8,9,4,3,3,3,7,5~6,11,8,4,10,7,3,6,10,9,11,8,3,3,3,10,11,9,9,5~11,6,3,3,3,7,11,10,4,8,8,6,5,7,10,9,9,5~10,7,11,8,8,10,6,3,3,3,6,9,5,10,9,7,11,8,9,7,10,4,9,11,8,7,11~7,10,10,5,6,9,11,7,11,10,4,3,3,3,11,9,8,8,5,10,7,11,8,9,9,7,10,6,8&reel_set4=7,9,11,2,8,6,12,10,9,10,4,12,11,6,7,8,3,3,3,5,3,8,10~9,3,3,3,7,5,10,4,9,6,11,9,10,8,8,11,11,2,10,6~3,3,3,11,10,12,10,6,9,4,8,11,10,9,2,7,4,5,5,8~10,4,6,8,8,9,9,6,3,3,3,5,2,8,7,9,10,7,8,9,7,10,11,11,10,11,7,11~6,2,7,8,4,11,10,11,9,10,3,3,3,9,10,7,11,9,12,8,7,9,12,8,10,8,4,6,11,7,5&reel_set3=4,10,2,11,5,10,9,6,8,11,7,7,8,3,3,3,10,9,5~2,10,6,8,3,3,3,11,4,8,6,7,9,10,3,3,3,11,11,9,5,9,10~4,11,7,8,9,10,5,4,5,8,3,3,3,10,6,11,2,11,9~6,8,4,8,11,7,7,11,5,9,6,11,10,3,3,3,11,9,10,8,7,2,9,7,10,10,9,8~8,10,9,10,6,3,3,3,11,6,5,8,4,9,8,10,4,9,7,11,11,2,7,11,8,9,8,7,7&reel_set5=2,7,4,2,5,6,2,10,8,2,11,9,3~2,4,9,2,10,3,2,5,8,2,11,7,6~2,3,4,5,6,7,8,9,10,11~2,11,9,2,7,6,2,8,10,2,5,4,3~2,8,3,10,5,2,6,7,11,4,2,5,6,8,7,9,9,11,10&awt=rsf";
            }
        }
	
	
        #endregion
        public LeprechaunCarolGameLogic()
        {
            _gameID = GAMEID.LeprechaunCarol;
            GameName = "LeprechaunCarol";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
        private int getStatusLength(string strStatus)
        {
            string[] strParts = strStatus.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            return strParts.Length;
        }
        private bool isAllStatusZero(string strStatus)
        {
            string[] strParts = strStatus.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i < strParts.Length; i++)
            {
                if (strParts[i] != "0")
                    return false;
            }
            return true;
        }
        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst)
        {
            try
            {
                LeprechaunCarolResult spinResult = new LeprechaunCarolResult();
                Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(dicParams, betInfo.TotalBet);
                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                if (SupportPurchaseFree && betInfo.PurchaseFree && isFirst)
                    dicParams["purtr"] = "1";

                spinResult.NextAction = convertStringToActionType(dicParams["na"]);
                spinResult.ResultString = convertKeyValuesToString(dicParams);
                if (spinResult.NextAction == ActionTypes.DOCOLLECT || spinResult.NextAction == ActionTypes.DOCOLLECTBONUS)
                    spinResult.TotalWin = double.Parse(dicParams["tw"]);
                else
                    spinResult.TotalWin = 0.0;

                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::calculateResult {0}", ex);
                return null;
            }
        }
        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            LeprechaunCarolResult result = new LeprechaunCarolResult();
            result.SerializeFrom(reader);
            return result;
        }
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                int ind     = -1;
                GITMessage responseMessage  = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                double realWin              = 0.0;
                string strGameLog           = "";
                string strGlobalUserID      = string.Format("{0}_{1}", agentID, strUserID);
                ToUserResultMessage resultMsg = null;

                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) ||
                    !_dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    var                 result  = _dicUserResultInfos[strGlobalUserID] as LeprechaunCarolResult;
                    BasePPSlotBetInfo   betInfo = _dicUserBetInfos[strGlobalUserID];
                    BasePPActionToResponse actionResponse = betInfo.pullRemainResponse();
                    if (actionResponse.ActionType != ActionTypes.DOBONUS)
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        Dictionary<string, string> dicParams = splitResponseToParams(actionResponse.Response);
                        if (dicParams.ContainsKey("status"))
                        {
                            int statusLength = getStatusLength(dicParams["status"]);
                            if (statusLength == 3 || statusLength == 5)
                            {
                                ind = (int)message.Pop();
                                if (dicParams.ContainsKey("status"))
                                {
                                    int[] status = new int[statusLength];
                                    status[ind] = 1;
                                    dicParams["status"] = string.Join(",", status);
                                }
                                if (dicParams.ContainsKey("wins_mask"))
                                {
                                    string[] strWinsMask = dicParams["wins_mask"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    if (ind != 0)
                                    {
                                        string strTemp = strWinsMask[ind];
                                        strWinsMask[ind] = strWinsMask[0];
                                        strWinsMask[0] = strTemp;
                                    }
                                    dicParams["wins_mask"] = string.Join(",", strWinsMask);
                                }
                                if (dicParams.ContainsKey("wins"))
                                {
                                    string[] strWins = dicParams["wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    if (ind != 0)
                                    {
                                        string strTemp = strWins[ind];
                                        strWins[ind] = strWins[0];
                                        strWins[0] = strTemp;
                                    }
                                    dicParams["wins"] = string.Join(",", strWins);
                                }
                            }
                            else if (statusLength == 12 && !isAllStatusZero(dicParams["status"]))
                            {
                                ind = (int)message.Pop();
                                if (result.BonusSelections == null)
                                    result.BonusSelections = new List<int>();

                                if (result.BonusSelections.Contains(ind))
                                {
                                    betInfo.pushFrontResponse(actionResponse);
                                    saveBetResultInfo(strGlobalUserID);
                                    throw new Exception(string.Format("{0} User selected already selected position, Malicious Behavior {1}", strGlobalUserID, ind));
                                }
                                result.BonusSelections.Add(ind);
                                if (dicParams.ContainsKey("status"))
                                {
                                    int[] status = new int[12];
                                    for (int i = 0; i < result.BonusSelections.Count; i++)
                                        status[result.BonusSelections[i]] = i + 1;
                                    dicParams["status"] = string.Join(",", status);
                                }
                                if (dicParams.ContainsKey("wins"))
                                {
                                    string[] strWins    = dicParams["wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    string[] strNewWins = new string[12];
                                    for (int i = 0; i < 12; i++)
                                        strNewWins[i] = "0";
                                    for (int i = 0; i < result.BonusSelections.Count; i++)
                                        strNewWins[result.BonusSelections[i]] = strWins[i + 1];
                                    dicParams["wins"] = string.Join(",", strNewWins);
                                }
                                if (dicParams.ContainsKey("wins_mask"))
                                {
                                    string[] strWinsMask = dicParams["wins_mask"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    string[] strNewWinsMask = new string[12];
                                    for (int i = 0; i < 12; i++)
                                        strNewWinsMask[i] = "h";
                                    for (int i = 0; i < result.BonusSelections.Count; i++)
                                        strNewWinsMask[result.BonusSelections[i]] = strWinsMask[i + 1];
                                    dicParams["wins_mask"] = string.Join(",", strNewWinsMask);
                                }
                            }
                        }

                        convertWinsByBet(dicParams, betInfo.TotalBet);
                        convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);
                        result.BonusResultString = convertKeyValuesToString(dicParams);
                        addDefaultParams(dicParams, userBalance, index, counter);

                        ActionTypes nextAction  = convertStringToActionType(dicParams["na"]);
                        string strResponse      = convertKeyValuesToString(dicParams);

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
                            resultMsg = new ToUserResultMessage((int)_gameID, responseMessage, 0.0, realWin, new GameLogInfo(this.GameName, "0", strGameLog), UserBetTypes.Normal); ;
                            resultMsg.BetTransactionID = betInfo.BetTransactionID;
                            resultMsg.RoundID = betInfo.RoundID;
                            resultMsg.TransactionID = createTransactionID();

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
                _logger.Error("Exception has been occurred in LeprechaunCarolGameLogic::onDoBonus {0}", ex);
            }
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set", "gsf_r", "gsf" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]))
                    continue;
                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }
            if (!resultParams.ContainsKey("s") && spinParams.ContainsKey("s"))
                resultParams["s"] = spinParams["s"];
            return resultParams;
        }

    }
}
