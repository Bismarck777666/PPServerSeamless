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
    public class TheAmazingMoneyMachineResult : BasePPSlotSpinResult
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

    class TheAmazingMoneyMachineGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10amm";
            }
        }
        protected override bool SupportReplay
        {
            get
            {
                return true;
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
                return 5;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "msi=14&def_s=19,19,19,19,19,19,19,19,19,19,5,10,3,13,7,4,8,10,7,6,3,8,9,11,5&msr=2&bgid=0&nas=19&cfgs=4106&ver=2&mo_s=16;17;18;15&mo_v=20,25,40,50,75,100,125,150,200,250,500,800,1000,1500,2000,2500,4000,5000;20,25,40,50,75,100,125,150,200,250,500,800,1000,1500,2000,2500,4000,5000;20,25,40,50,75,100,125,150,200,250,500,800,1000,1500,2000,2500,4000,5000;20,25,40,50,75,100,125,150,200,250,500,800,1000,1500,2000,2500,4000,5000&def_sb=4,13,4,7,8&reel_set_size=3&def_sa=10,3,5,3,7&bonusInit=[{bgid:0,bgt:18,bg_i:\"5000,500,50,25\",bg_i_mask:\"pw,pw,pw,pw\"}]&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&bg_i=5000,500,50,25&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"13315579\",max_rnd_win:\"5100\"}}&wl_i=tbm~5100&bgt=18&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&wilds=2~1000,200,50,0,0~1,1,1,1,1;16~1000,200,50,0,0~1,1,1,1,1;20~1000,200,50,0,0~1,1,1,1,1&bonuses=0&fsbonus=&bg_i_mask=pw,pw,pw,pw&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;200,100,20,0,0;150,50,15,0,0;100,30,10,0,0;100,30,10,0,0;50,20,5,0,0;50,20,5,0,0;40,15,5,0,0;40,15,5,0,0;25,10,5,0,0;25,10,5,0,0;25,10,5,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&reel_set0=12,14,14,14,11,2,2,2,8,6,14,15,4,3,5,2,10,9,13,7,14,8,2,11,14,2,14,7,14,2,3,2~11,9,2,2,2,7,8,14,2,13,6,1,3,12,15,14,14,14,10,5,4,2,4,8,6,2,9,13,14,2,15,14,7,12,13,2,5,7,3,14,9,8,10,2,9,13,2,8,2,5,8,2,13,10,15,13~15,15,15,13,8,3,15,10,12,6,4,1,14,14,14,5,14,9,2,2,2,2,11,7,2,6,4,2,3,2,12,10,14,2,11,14,2,14,3,2,14,2,14,2,13,3,11,10,2,10,3,13,5,14,2,14,2,14,10,3,6,14,1,11,14,13,14,12,6,9,2,10,3,14,3,2,14,5,14,2,8,14,11,13,11,2,14,2,13,12~13,1,9,15,2,2,2,7,3,12,6,10,4,11,5,2,14,14,14,8,14,10,11,14,1,15,2,10,2,1,10,2,10,2,5,10,9,2,9,5,14,10,2,10,1,5,1,8,9,2,5,1,5,10,5,10,14,2,10,6,5,2,10,2,14,5,8,2,10,14,8,2,5,10,11,14,2,11,12,14,5,11,5,2,15,2,10,2,5,2,5,10,15,10,14,2,5,8,14,12,10,15,5,14,5,2,5,8,1,11,14,8,2,8,10,5,14,10,2,15,14,7,1,10,5,2,10,11,2,10,15,5,10,14,5,1,12,1,5,9,14,5,2,14,1,12,2,14,2,14,10,2,15,14,1,9,7,11,8,2,11,8,15,10,2,8,5,11,10,2,14,2,10,14,5,2,8,14,9,8,5,14,2,14,10,8,2,12,2,14,12,9,14,1,14,9,8,5,2,11,1,8,14,10,5,15,1~13,14,14,14,4,2,2,2,9,15,14,12,8,2,6,11,7,5,10,3,10,2,9,11,2,14,11,2,11&accInit=[{id:0,mask:\"cp;mp\"}]&reel_set2=13,5,8,10,3,12,7,11,6,9,4,4,3,9,3,4,10,11,4,3,4,6,7,3,7,10,4,7,3,5,3,4,12,7,4,7,4,12,4,4~13,5,8,10,3,12,7,11,6,9,4,4,3,9,3,4,10,11,4,3,4,6,7,3,7,10,4,7,3,5,3,4,12,7,4,7,4,12,4,4~13,5,8,10,3,12,7,11,6,9,4,4,3,9,3,4,10,11,4,3,4,6,7,3,7,10,4,7,3,5,3,4,12,7,4,7,4,12,4,4~13,5,8,10,3,12,7,11,6,9,4,4,3,9,3,4,10,11,4,3,4,6,7,3,7,10,4,7,3,5,3,4,12,7,4,7,4,12,4,4~13,5,8,10,3,12,7,11,6,9,4,4,3,9,3,4,10,11,4,3,4,6,7,3,7,10,4,7,3,5,3,4,12,7,4,7,4,12,4,4&reel_set1=13,9,6,5,7,10,15,4,2,2,2,12,14,11,14,14,14,8,2,3,14,4~14,14,14,5,6,2,2,2,8,4,7,10,14,12,9,2,15,11,3,13,15,10,9,3,9,5,11,3,13,2,15,13,15,10,15,10,2,9,5,2,10,3,15,5,10,5,4,12,13~3,8,2,2,2,4,5,14,14,14,7,2,10,12,15,11,13,6,9,14,11,9,14,2,9,14,2,10,11,9,14,15,14,2,7,6,9,2,9~13,4,14,14,14,9,5,12,2,2,2,3,15,8,7,14,10,6,11,2,9,2,9,15,6,15,2,14,7,10,5,2,15,2,7,5,15,14,2,15,7,2,14,9,2,5,2,14,2,15,4~14,2,2,2,8,3,7,6,12,14,14,14,5,13,10,4,2,9,15,11,2,10,9,11";
            }
        }
	
	
        #endregion
        public TheAmazingMoneyMachineGameLogic()
        {
            _gameID = GAMEID.TheAmazingMoneyMachine;
            GameName = "TheAmazingMoneyMachine";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("mo_tw"))
                dicParams["mo_tw"] = convertWinByBet(dicParams["mo_tw"], currentBet);

            if (dicParams.ContainsKey("pw"))
                dicParams["pw"] = convertWinByBet(dicParams["pw"], currentBet);

        }
        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst)
        {
            try
            {
                TheAmazingMoneyMachineResult spinResult = new TheAmazingMoneyMachineResult();
                Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(dicParams, betInfo.TotalBet);

                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

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
                _logger.Error("Exception has been occurred in TheAmazingMoneyMachineGameLogic::calculateResult {0}", ex);
                return null;
            }
        }
        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            TheAmazingMoneyMachineResult result = new TheAmazingMoneyMachineResult();
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

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                double realWin          = 0.0;
                string strGameLog       = "";
                string strGlobalUserID  = string.Format("{0}_{1}", agentID, strUserID);
                ToUserResultMessage resultMsg = null;
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) ||
                    !_dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    TheAmazingMoneyMachineResult result = _dicUserResultInfos[strGlobalUserID] as TheAmazingMoneyMachineResult;
                    BasePPSlotBetInfo betInfo  = _dicUserBetInfos[strGlobalUserID];
                    BasePPActionToResponse actionResponse = betInfo.pullRemainResponse();
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
                            string[] strWins = dicParams["wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            string[] strNewWins = new string[12];
                            for (int i = 0; i < 12; i++)
                                strNewWins[i] = "0";
                            for (int i = 0; i < result.BonusSelections.Count; i++)
                                strNewWins[result.BonusSelections[i]] = strWins[i];
                            dicParams["wins"] = string.Join(",", strNewWins);
                        }
                        if (dicParams.ContainsKey("wins_mask"))
                        {
                            string[] strWinsMask = dicParams["wins_mask"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            string[] strNewWinsMask = new string[12];
                            for (int i = 0; i < 12; i++)
                                strNewWinsMask[i] = "h";
                            for (int i = 0; i < result.BonusSelections.Count; i++)
                                strNewWinsMask[result.BonusSelections[i]] = strWinsMask[i];
                            dicParams["wins_mask"] = string.Join(",", strNewWinsMask);
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
                _logger.Error("Exception has been occurred in TheAmazingMoneyMachineGameLogic::onDoBonus {0}", ex);
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
