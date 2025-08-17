using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class BookOfAztecKingGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10bookazteck";
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
            get { return 10; }
        }
        protected override int ServerResLineCount
        {
            get { return 10; }
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
                return "wsc=1~bg~200,20,2,0,0~10,10,10,0,0&def_s=5,8,7,9,8,8,7,3,4,4,11,6,8,11,10&cfgs=5400&ver=2&ms=10&def_sb=5,3,4,6,7&reel_set_size=12&def_sa=11,5,10,8,9&scatters=&gmb=0,0,0&rt=d&gameInfo={}&reel_set10=4,8,3,9,11,7,10,5,6,7,10,5,6,3,10,3,6,7,3,10,7,10,3,5,3,7,3,5,6,7,8,9,3,10,5,3,9,5,7,6,10,6,10,3,7,6,3,10,5,3,7,5,6,10,9~10,9,5,3,11,8,7,4,6,4,7,3,7,11,3,7,3,9,4,3,4~4,9,5,3,10,11,6,8,7,10,9,5,6,3,6,3,7,6,7,3,7,5,3,8,3,6,8,10,3,5,6,9,5,3,9,10,5,7,3,10,5,9,7,9,7,10,3,7,9,6,5,3,5,6,10,7,5~6,5,8,7,11,10,4,3,9,8,7,11,8,11,10,11,10,4,7,3,8,10,7,9,10,9,3,8,4,8,10,9,8,3,7,8,4,8,7,8,7,8,9,8,10,7,8,9,7,3,8,5,3,8,10~10,8,4,3,5,6,9,7,11,4,11,6,7,3,8,6,8,11,7,8,11,7,3,11,4,7,9,5,7,11&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&reel_set11=4,9,10,11,8,7,5,3,6,1,5,11,5,3,10,11,7,10,5,7,3,10,5,10,7~5,10,11,4,9,8,3,6,7,1,8,9,11,8,4,10,1,4,9,8,4,11,10,8,4,9,3,9,4,10,8~6,7,9,8,11,10,1,5,4,3,11,3,7,5,9,10,1,4,11,7,1,11,5,1~5,1,7,10,8,4,6,9,3,11,4,9,6,10,9,4,11,10,6,9,6,9,11,9,7,4,6,9,8,10,4~11,5,7,9,8,4,6,10,1,3,1,6,7,3,5,6,3,8,10,5,1,9&purInit_e=1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;5000,1000,100,10,0;2000,400,40,5,0;750,100,30,5,0;750,100,30,5,0;150,40,5,0,0;150,40,5,0,0;100,25,5,0,0;100,25,5,0,0;100,25,5,0,0&total_bet_max=10,000,000.00&reel_set0=8,1,5,11,7,4,6,3,9,10,1,6,11,3,6,5,10,6,3,6,1,4,10,3,4,5,10,11,6~5,1,8,7,4,11,9,10,3,6,7,6,1,6,7,10,1~4,7,3,9,10,1,6,11,8,5,8,10,8,11,8,3,10,8,10,8,7,8,11,7,8,5,8,10,7,10,6,7,3,10,6,8,10,8,10,3,8,7,10,11,10,11,10,8~4,10,11,6,8,9,1,7,5,3,1,10,1,6,7,10,3,10,7,9,10,3,6,10,9,8,11,10,6,1,8,1,6,10,1,11,7,10,11,6,11,10,6,10,6,10,6,9,7,9,6~6,1,10,8,5,4,11,9,3,7,4,7,4,7,10,11,10,8,4,5,4,10,8,5,7,4,5,4,5,7,9,4,5,4,10,4,1,7,5,4,5,9,10,4,10,5,10,5,7,4,7,3,4,11,5,4,7,4,9,4,9,4,10,5,3&reel_set2=3,5,6,10,11,7,8,1,9,4,1~6,3,1,9,8,10,11,5,4,7,8,1,8,9,8,5,3,8,9,3,8,1,8,5,9,11,8,11,8,3,8,3,9,8,5,10,11,5,8,10,8,10,8,5,8,1,3,5,3,8,1,4,8,1,7,8,9,11,8,5,11,4,8,11,10,5,7~5,10,6,8,7,4,9,11,1,3,11,9,11,10,6,9,11,8,11,9,11,9,6,7,4,10,4,6,3,11~9,11,7,4,8,5,6,10,3,1,8,6,8,6,8,1,6,7,1,3,4,8,6,10,1,4,8,10,6,3,10,6,8,6,5,7,6,4,6,4,8,10,3,7,8,5,6,10,3,6,7,5,6,3,11,3,5,11,1,6,3,6~3,10,9,7,8,5,11,6,4,1,6,11,9,8,7,6,11,6,8,6,10,8,1,6,11,9,7,8,10,8,11,8,9,10,11,9,11&reel_set1=3,6,7,10,8,1,9,4,5,11,7,10~11,10,1,4,5,8,9,7,6,3,5,9,10,5,9,3,6,5,10,6,10,9,3,10,6,3,7,5,1,7,5,3,6,7,3,1,10,6,1,6,1,6,7,3,7,6,9,5,10,1,6,5,6,8,7,6~6,4,9,10,1,5,11,8,3,7,3,5,8,10,1,10,1,10,3,11,1,5,10,9,8,7,3,7,9,4,1~4,9,8,1,5,7,3,10,6,11,7,6,8,9,11,3,6,7,8~9,5,6,3,8,4,11,1,10,7,1,8,5,8,4,5,8,4,5,4,5,8,4,5,4,1,4,5,4,5,1,5,8,5,11,6,5,10,5,1,3,10,5&reel_set4=11,10,3,5,6,9,7,1,8,4,3,4,1,7,9,1,4,1,8,9,4,3,5,4,7,3,8~4,6,10,9,1,8,3,7,11,5,1,9,11,3,11,1,10~6,5,1,4,10,7,11,8,9,3,7,5,3,5,3,5,9,3,5,9,5,1,9,10,4,3,7,3,4,9,5,3,7,5,9,3,8~7,4,11,3,1,8,10,9,6,5,4~7,8,10,1,3,6,9,11,5,4,10,5&purInit=[{type:\"fs\",bet:1000}]&reel_set3=4,7,11,3,9,1,6,8,5,10,6,8,6,3,1,8,1,8,6,1,8,7,11,1,8,5,9,3,1,6,9,8,1,8,3,6,8,5,6,7,6,9,6,8,1,6~3,5,1,10,4,6,8,9,11,7,9,10,1,9,8,4,11,10,5,7,9,10,9,11,10,9,10,11,9,5,8,9~4,10,5,7,3,9,1,6,11,8,1,3,9,5,3,5,9,5,7,5,11,5,3,5,11,9,3,5,7,1,11,9,5,11,5,3,5,3,9,3,11,9,5,3,9~5,7,3,11,9,1,6,10,8,4,1,10,6,4,3,11,6,4,7,8,7,4,10,11,1,7,1,10,6,4,10,8,10,9,4,10,9,10,8,4,10~7,1,3,9,4,6,5,11,10,8,5,3,4,1,10,4,10,4,5,6,4,10,8,10,1,3,1,11,4,1,4,10,1,3,4,1&reel_set6=8,9,4,1,11,5,3,10,6,7,11~8,6,10,3,4,1,5,11,9,7,3,4,5,1,7,5,3,7,5,1,7,3,1,3,11,1,11,7,11,1,9~4,8,7,5,9,11,10,3,1,6,11,10,11,9,11,5,8,11,5,8,5,8,9,3,6,5,11,10,9,11,5,9,11,8,7,3,11,5,8,9,5,9,6,8,3~8,9,11,3,5,1,6,4,10,7,6,5,6,9,5,9,6,10,5,6,4,5,6,7,5,1,9,10,6,9,11,9,6,9,6,1,9,5,9,11,9,6,9,6,10~10,4,1,3,5,8,7,6,9,11,9,6,4,6,8,9,8,6,11,1,6,4,9,7,6,1,4,8,6,8,6,3,6,7,11,4,9,8,1,7&reel_set5=4,7,8,10,1,6,3,9,5,11,1,11,1~5,3,9,4,6,7,1,10,11,8,1,7,1,3,7,1,8,1,7,6,4,11,7,3,7,4,3,10,7,10,4,7,1,7,4,3,10,7,6,1,10,8,1,7,3,1,8,1,4~5,1,7,10,8,11,3,9,4,6,3,6,9,3,1,7,6~5,3,1,6,4,8,7,11,10,9,11,6,11,8,6,8,4,6,3,8,3,6,11,8,3,6,8,6,11,6,8,6,4,8,6,11,6,3,6,9,6~7,5,9,4,8,3,1,10,11,6,10,3,9,4,10,3,5,9,10,9,4,9,3,4,10,11,5,10,5,11,5&reel_set8=10,1,5,7,3,6,8,9,4,11,7,6,7,4,1,11,5,11,1,11,7,4,7,6,5,4,8,11,5,1,7,6,9,7~1,4,11,9,3,6,7,5,10,8,3,4,11,10,3,4,3,7,11,8,10,11,3,10,8,4,7,10,4,7,6,7,8,7,10,7,8,7,8,7~8,4,10,3,6,7,9,5,11,1,11,1,10,1,3,4,10,11,1,5,3,1,10,6,1,10~3,7,4,5,9,1,6,11,8,10,7,6,10,11,6,7,6,7,6,4,6,7,4,11,7,4,7,6,11,4,6,11,4,11,5~11,5,6,9,8,3,1,7,10,4,9,4,10,6,10,5&reel_set7=9,3,6,4,8,1,5,7,11,10,1,8,1,7,6,7,8,3,5,3,8,7,6,3,5,1,5,3,7,3,6~5,7,10,3,6,8,11,9,1,4,6,3,9,10,4,6,10,3,1,10,3,4,3,1,6~7,1,9,4,6,10,8,11,3,5,4,8,3,10,5~3,6,9,7,8,5,11,1,4,10,4,10,8,4,6,5,10,1,10,4,6,1,10,7,1,10,8,6,10,6,4,5,10,5,10,4,1,10,11,1,10,6,8,10,1,10,11,10,8,11,1~3,10,7,6,9,4,5,11,1,8,7,9,7,11,8,11,4,1,4,9,6,4,8,11,9,8,5,4&reel_set9=9,1,4,11,7,5,8,10,6,3,7,1,4,1,10,4,6,4,5,1,5,6,10,4,11,6,4,11,5,6,4,11,6,4,6,5,4,11,4,1~1,4,9,7,5,6,3,10,8,11,9,7,8,9,3,6,3,8,9,4,9,10,9~3,6,4,7,9,11,5,8,1,10,1,6,1,5,10,5~1,8,10,9,3,11,4,6,7,5,10,6,8,6,8,4,10,8,5,3,5,4,9,8,11,9,5,3~8,1,9,7,10,3,5,6,11,4,1,11,3,6,4,7,9,3,4,1,7,6,3,7,6,1,5,1&total_bet_min=20.00";
            }
        }
	
        protected override double PurchaseFreeMultiple
        {
            get { return 100; }
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
        public BookOfAztecKingGameLogic()
        {
            _gameID = GAMEID.BookOfAztecKing;
            GameName = "BookOfAztecKing";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "7";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);            
        }
        protected override async Task onProcMessage(string strUserID, int agentID,  GITMessage message, UserBonus userBonus, double userBalance, Currencies currency, bool isAffiliate)
        {
            if (message.MsgCode == (ushort)CSMSG_CODE.CS_PP_DOMYSTERYSCATTER)
                onDoMysteryScatter(agentID, strUserID, message, userBonus, userBalance);
            else
                await base.onProcMessage(strUserID, agentID, message, userBonus, userBalance, currency, isAffiliate);
        }
        protected void onDoMysteryScatter(int agentID, string strUserID, GITMessage message, UserBonus userBonus, double userBalance)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOMYSTERYSCATTER);
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BasePPSlotSpinResult result = _dicUserResultInfos[strGlobalUserID];
                    if (result.NextAction != ActionTypes.DOMYSTERY)
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        BasePPSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
                        BasePPActionToResponse actionResponse = betInfo.pullRemainResponse();
                        if (actionResponse.ActionType != ActionTypes.DOMYSTERY)
                        {
                            responseMessage.Append("unlogged");
                        }
                        else
                        {
                            Dictionary<string, string> dicParams = splitResponseToParams(actionResponse.Response);
                            result.BonusResultString = convertKeyValuesToString(dicParams);
                            addDefaultParams(dicParams, userBalance, index, counter);
                            responseMessage.Append(convertKeyValuesToString(dicParams));

                            ActionTypes nextAction = convertStringToActionType(dicParams["na"]);

                            //히스토리보관 및 초기화
                            if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                                addActionHistory(strGlobalUserID, "doMysteryScatter", convertKeyValuesToString(dicParams), index, counter);

                            result.NextAction = nextAction;
                        }
                        if (!betInfo.HasRemainResponse)
                            betInfo.RemainReponses = null;

                        saveBetResultInfo(strGlobalUserID);
                    }

                }
                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BookOfAztecKingGameLogic::onDoMysteryScatter {0}", ex);
            }
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


                BasePPSlotBetInfo betInfo = new BasePPSlotBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();
		
		
                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;
		
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in BookOfAztecKingGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (!isNotIntergerMultipleBetPerLine(betInfo.BetPerLine, minChip))
                {
                    _logger.Error("{0} betInfo.BetPerLine is illegual: {1} != {2} * integer", strGlobalUserID, betInfo.BetPerLine, minChip);
                    return;
                }

                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1} != {2}", strGlobalUserID, betInfo.LineCount, this.ClientReqLineCount);
                    return;
                }
                BasePPSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strGlobalUserID, out oldBetInfo))
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
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BookOfAztecKingGameLogic::readBetInfoFromMessage {0}", ex);
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
