using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using Akka.Actor;

namespace SlotGamesNode.GameLogics
{

    public class DiamondStrikeResult : BasePPSlotSpinResult
    {
        public List<int>    BonusSelections { get; set; }

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
    public class DiamondStrikeGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs15diamond";
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
            get
            {
                return 15;
            }
        }
        protected override int ServerResLineCount
        {
            get
            {
                return 15;
            }
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
                return "def_s=6,7,4,2,8,4,3,5,6,7,8,5,7,3,4&bgid=0&cfgs=2693&ver=2&reel_set_size=2&def_sb=7,4,7,5,7&def_sa=3,6,6,8,8&bonusInit=[{bgid:0,bgt:18,bg_i:\"1000,100,30,10\",bg_i_mask:\"pw,pw,pw,pw\"}]&scatters=1~0,0,0,0,0~0,0,8,0,0~1,1,1,1,1&gmb=0,0,0&bg_i=1000,100,30,10&rt=d&bgt=18&sc=15.00,20.00,40.00,60.00,120.00,150.00,400.00,600.00,1000.00,2000.00,3000.00,4000.00,6000.00&defc=150.00&wilds=2~300,60,20,0,0~1,1,1,1,1&bonuses=0&fsbonus=&bg_i_mask=pw,pw,pw,pw&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;200,20,10,0,0;100,20,10,0,0;40,10,5,0,0;40,10,5,0,0;40,10,5,0,0;40,10,5,0,0&rtp=94.16&reel_set0=6,5,7,8,6,2,2,2,7,3,1,4,7,8,4,7,2,2,7,6,3,6,7,6,4,6~6,8,4,8,5,5,3,8,2,2,8,3,8,8,5,4,5,8,4,6,8,3,5,4,5,2,2,2,7~7,3,6,5,2,2,2,8,5,6,5,2,2,5,4,5,5,7,1~6,4,3,6,8,4,7,6,5,6,2,2,2,7,5,8,6,7,6,4,6,8,4,2~4,5,6,5,6,4,5,1,7,5,2,2,2,7,5,7,1,5,8,7,4,5,6,7,5,7,3,7,5,7,3,5,4,7,4,5,7&reel_set1=4,8,3,7,6,4,6,1,4,6,2,2,2,7,4,1,5,7,8,5,7,4,6,8,2,2,5,3~3,4,6,4,3,8,5,2,2,3,6,4,4,5,2,2,2,6,7,8,4,8,5~7,2,2,6,4,3,1,6,2,2,2,3,4,5,4,5,8,7,4,5,6,8~8,6,3,6,4,2,2,2,4,8,4,5,7,6,2,2,5,5,8,6,4,7~1,8,1,6,2,2,2,5,4,8,7,4,5,4,7,4,1,5,6,8,7,8,2,2,3,6";
            }
        }
        #endregion

        public DiamondStrikeGameLogic()
        {
            _gameID = GAMEID.DiamondStrike;
            GameName = "DiamondStrike";
        }

        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["n_reel_set"] = "0";
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "n_reel_set", "gsf_r", "gsf" };
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

        
        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst)
        {
            try
            {
                DiamondStrikeResult spinResult = new DiamondStrikeResult();
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
                _logger.Error("Exception has been occurred in DiamondStrikeGameLogic::calculateResult {0}", ex);
                return null;
            }
        }
        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            DiamondStrikeResult result = new DiamondStrikeResult();
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
                    DiamondStrikeResult result              = _dicUserResultInfos[strGlobalUserID] as DiamondStrikeResult;
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
                        if(dicParams.ContainsKey("status"))
                        {
                            int[] status = new int[12];
                            for (int i = 0; i < result.BonusSelections.Count; i++)
                                status[result.BonusSelections[i]] = i + 1;
                            dicParams["status"] = string.Join(",", status);
                        }
                        if(dicParams.ContainsKey("wins"))
                        {
                            string[] strWins = dicParams["wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            string[] strNewWins = new string[12];
                            for (int i = 0; i < 12; i++)
                                strNewWins[i] = "0";
                            for (int i = 0; i < result.BonusSelections.Count; i++)
                                strNewWins[result.BonusSelections[i]] = strWins[i];
                            dicParams["wins"] = string.Join(",", strNewWins);
                        }
                        if(dicParams.ContainsKey("wins_mask"))
                        {
                            string[] strWinsMask    = dicParams["wins_mask"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
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
                _logger.Error("Exception has been occurred in DiamondStrikeGameLogic::onDoBonus {0}", ex);
            }
        }
    }
}
