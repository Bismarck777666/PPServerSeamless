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
    class VoodooMagicBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get { return this.BetPerLine * 20.0f; }
        }
    }
    class VoodooMagicGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs40voodoo";
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
            get { return 40; }
        }
        protected override int ServerResLineCount
        {
            get { return 20; }
        }
        protected override int ROWS
        {
            get
            {
                return 4;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=3,9,10,8,8,9,8,5,11,11,11,11,8,7,7,4,5,6,7,8&cfgs=3700&ver=2&def_sb=10,10,11,9,5&reel_set_size=6&def_sa=11,7,3,4,6&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&cls_s=13&gmb=0,0,0&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"141443\",max_rnd_win:\"1000\"}}&wl_i=tbm~1000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1&wilds=2~250,100,25,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;250,100,25,0,0;200,50,20,0,0;100,40,10,0,0;75,30,10,0,0;75,30,10,0,0;50,20,4,0,0;50,20,4,0,0;40,10,4,0,0;40,10,4,0,0;20,5,2,0,0;20,5,2,0,0;0,0,0,0,0,0&total_bet_max=8,000,000.00&reel_set0=2,2,2,2,7,10,8,12,3,3,12,3,8,8,5,5,12,12,11,11,11,8,8,11,11,12,12,6,12,6,6,12,11,11,12,12,9,12,4~2,2,2,2,7,12,12,12,9,9,8,8,11,11,10,10,9,6,6,4,9,4,4,8,9,10,3,10,5,9,4,11,6,8,7,7,12,12,7,7,3,3,8,3,3,11,8,8,5,5,11,5,11,12,12,4,4,10,4,9,11,11,9,5,5,12,7,1~2,2,2,2,9,10,1,10,4,4,8,4,3,7,3,3,8,4,8,4,12,9,9,5,5,10,5,9,6,6,10,3,3,11~2,2,2,2,8,5,5,9,7,1,9,4,4,9,8,12,6,3,3,9,12,6,6,8,11,4,11,4,10~5,7,6,12,6,10,8,11,8,2,2,2,2,7,4,4,9,3&reel_set2=5,12,12,8,11,11,11,8,13,13,13,13,13,13,13,13,13,13,8,4,9,4,9,11,3,3,13,13,13,13,13,7,12,8,11,12,7,12,7,13,13,13,13,13,9,12,12,11,4,11,6,6,11,4,11,2,2,2,2,7,10~4,8,10,2,2,2,2,2,10,9,11,8,7,7,12,12,7,7,13,13,13,13,11,5,11,3,13,13,13,13,12,10,7,10,6~2,2,2,2,6,11,10,4,4,10,13,13,13,13,13,13,13,13,13,13,12,9,9,5,5,10,13,13,13,13,13,3,3,8,10,3,8,3,3,6,7~8,12,10,4,4,11,4,4,12,4,13,13,13,13,13,5,12,5,5,12,7,2,2,2,2,2,10,9,5,5,12,12,6,6,12,3~6,8,2,2,2,2,2,8,11,12,9,4,4,7,3,7,13,13,13,13,13,8,8,4,9,12,12,10,5&reel_set1=5,12,12,8,11,11,11,8,13,13,13,13,8,4,9,4,9,11,3,3,13,13,13,13,7,12,8,11,12,7,12,7,13,13,13,13,13,9,12,12,11,4,11,6,6,11,4,11,2,2,2,2,7,10~2,2,2,2,12,7,13,13,13,13,3,3,8,11,4,8,4,8,5,13,13,13,13,6,6,9,10~3,7,4,4,11,5,5,8,9,2,2,2,2,9,10,4,11,4,4,10,11,7,12,6,6,10,13,13,13,13~4,6,2,2,2,2,11,12,10,8,5,5,9,9,9,7,7,9,13,13,13,13,3~2,2,2,2,11,8,4,7,7,10,5,5,9,8,3,3,8,11,6,6,8,7,11,11,9,12,10,13,13,13,13&reel_set4=2,2,2,2,7,10,8,12,3,3,12,3,8,8,5,5,12,12,11,11,11,8,8,11,11,12,12,6,12,6,6,12,11,11,12,12,9,12,4~11,1,12,7,8,1,9,10,7,1,5,12,7,1,6~4,10,11,1,3,9,10,1,2~4,10,10,1,5,9,12,1,8,12,7,1,6,11~5,7,6,12,6,10,8,11,8,2,2,2,2,2,7,4,4,9,3&purInit=[{type:\"fs\",bet:1600}]&reel_set3=5,12,12,8,11,11,11,8,13,13,13,13,8,4,9,4,9,11,3,3,13,13,13,13,13,7,12,8,11,12,7,12,7,13,13,13,13,9,12,12,11,4,11,6,6,11,4,11,2,2,2,2,7,10~6,13,13,13,13,13,10,12,11,13,13,13,13,13,4,4,8,9,10,10,2,2,2,2,12,7,13,13,13,13,4,8,5~2,2,2,2,6,11,10,4,4,10,13,13,13,13,13,13,13,13,13,13,12,9,9,5,5,10,13,13,13,13,13,3,3,8,10,3,8,3,3,6,7~9,13,13,13,13,11,4,11,4,10,6,11,6,6,12,10,2,2,2,2,8,12,10,4,4,11,4,4,12,4,13,13,13,13,13,5,12,5,5,12,7,2,2,2,2,12,6,6,12,3~6,8,7,11,11,9,13,13,13,13,13,13,13,13,13,10,8,11,8,2,2,2,2,2,7,4,4,9,3,3,7,9,8,3,13,13,13,13,4,12,4,10,5&reel_set5=5,9,11,11,11,11,7,3,3,3,3~6,1,8,12,12,12,10,10,10,1,4,4,4~2,2,2,1,10,10,4,1,4,8,4,1,7,3,3,1,4,8,4,1,9,9,5,1,10,5,9,1,6,10,3,1,11,12~2,2,2,8,5,1,9,7,9,1,4,4,9,1,12,6,3,1,9,12,6,1,8,11,4,1,4,10~5,7,6,12,6,10,8,11,8,2,2,2,2,7,4,4,9,3&total_bet_min=10.00";
            }
        }
	
        protected override double PurchaseFreeMultiple
        {
            get { return 80; }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override bool HasPurEnableOption
        {
            get { return true; }
        }
	
	
        #endregion
        public VoodooMagicGameLogic()
        {
            _gameID = GAMEID.VoodooMagic;
            GameName = "VoodooMagic";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);

            if (dicParams.ContainsKey("wl_ta"))
                dicParams["wl_ta"] = convertWinByBet(dicParams["wl_ta"], currentBet);

            if (dicParams.ContainsKey("wl_utw"))
                dicParams["wl_utw"] = convertWinByBet(dicParams["wl_utw"], currentBet);
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            VoodooMagicBetInfo betInfo = new VoodooMagicBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        private int getStatusLength(string strStatus)
        {
            string[] strParts = strStatus.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            return strParts.Length;
        }
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                int     index           = (int)message.Pop();
                int     counter         = (int)message.Pop();
                string  strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                double realWin = 0.0;
                string strGameLog = "";
                ToUserResultMessage resultMsg = null;
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) ||
                    !_dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    var result = _dicUserResultInfos[strGlobalUserID];
                    BasePPSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
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
                            int ind = (int)message.Pop();
                            int statusLength = getStatusLength(dicParams["status"]);
                            if (dicParams.ContainsKey("status"))
                            {
                                int[] status = new int[statusLength];
                                status[ind]  = 1;
                                dicParams["status"] = string.Join(",", status);
                            }                            
                            if (dicParams.ContainsKey("wins_mask"))
                            {
                                string[] strWinsMask = dicParams["wins_mask"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                if(ind != 0)
                                {
                                    string strTemp   = strWinsMask[ind];
                                    strWinsMask[ind] = strWinsMask[0];
                                    strWinsMask[0]   = strTemp;
                                }
                                dicParams["wins_mask"] = string.Join(",", strWinsMask);
                            }                            
                        }

                        convertWinsByBet(dicParams, betInfo.TotalBet);
                        convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);
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
                _logger.Error("Exception has been occurred in VoodooMagicGameLogic::onDoBonus {0}", ex);
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
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                VoodooMagicBetInfo betInfo = new VoodooMagicBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();
		
		
                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;
		
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in VoodooMagicGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1}", strUserID, betInfo.LineCount);
                    return;
                }

                BasePPSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strUserID, out oldBetInfo))
                {
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine   = betInfo.BetPerLine;
                    oldBetInfo.LineCount    = betInfo.LineCount;
                    oldBetInfo.MoreBet      = betInfo.MoreBet;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in VoodooMagicGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
	
    }
}
