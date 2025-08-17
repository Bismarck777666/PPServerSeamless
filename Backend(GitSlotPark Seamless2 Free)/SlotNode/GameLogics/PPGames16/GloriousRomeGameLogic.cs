using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class GloriousRomeGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20rome";
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
                return "tw=0.00&wsc=1~200,20,2,0,0~10,10,10,0,0&def_s=4,3,5,6,3,5,6,2,4,3,4,3,3,4,3&msg_code=0&pbalance=0.00&cfgs=1076&reel1=3,6,8,8,8,6,4,11,11,11,2,2,2,2,2,9,9,9,9,3,6,7,7,7,4,5,10,10,10,10,6,5,8,8,8,6,4,11,11,6,11,11,5,6,9,9,9,4,6,7,7,7,1,10,10,10,5,4,8,8,8,6,5,11,11,11,1,9,9,9,3,7,7,7,5,3,10,10,10,6,4,8,8,8,1,11,11,11,5,3,9,9,9,9,4,5,7,7,7,3,10,10,10,10,3,5,8,8,8,4,6,11,11,11,4,5,9,9,9,1,7,7,7,6,3,10,10,10&ver=2&reel0=3,6,8,8,8,6,4,11,11,11,11,2,2,2,2,2,9,9,9,9,3,6,7,7,7,4,5,10,10,10,10,6,5,8,8,8,6,4,11,11,6,11,11,5,6,9,9,9,4,6,7,7,7,1,10,10,10,5,4,8,8,8,6,5,11,11,11,1,9,9,9,3,7,7,7,5,3,10,10,10,6,4,8,8,8,1,11,11,11,5,3,9,9,9,4,5,7,7,7,3,10,10,10,10,3,5,8,8,8,4,6,11,11,11,4,5,9,9,9,1,7,7,7,6,3,10,10,10&reel3=3,6,8,8,8,6,4,11,11,11,11,2,2,2,2,2,2,9,9,9,3,6,7,7,7,4,5,10,10,10,10,6,5,8,8,8,6,4,11,11,11,11,5,6,9,9,9,4,6,7,7,7,1,10,10,10,5,4,8,8,8,6,5,11,11,11,1,9,9,9,3,7,7,7,5,3,10,10,10,6,4,8,8,8,1,11,11,11,5,3,9,9,9,9,4,5,7,7,7,3,10,10,10,3,5,8,8,8,4,6,11,11,11,4,5,9,9,9,9,1,7,7,7,6,3,10,10,10&reel2=3,6,8,8,8,6,4,11,11,11,11,2,2,2,2,2,9,9,9,9,3,6,7,7,7,4,5,10,10,10,10,6,5,8,8,8,6,4,11,11,11,11,5,6,9,9,9,4,6,7,7,7,1,10,10,10,5,4,8,8,8,6,5,11,11,11,1,9,9,9,3,7,7,7,5,3,10,10,10,6,4,8,8,8,1,11,11,11,5,3,9,9,9,9,4,5,7,7,7,3,10,10,10,10,3,5,8,8,8,4,6,11,11,11,4,5,9,9,9,1,7,7,7,6,3,10,10,10&reel4=3,6,8,8,8,6,4,11,11,11,11,2,2,2,2,2,9,9,9,9,3,6,7,7,7,4,5,10,10,10,10,6,5,8,8,8,6,4,11,11,3,11,11,5,6,9,9,9,4,6,7,7,7,1,10,10,10,5,4,8,8,8,6,5,11,11,11,1,9,9,9,3,7,7,7,5,3,10,10,10,6,4,8,8,8,1,11,11,11,5,3,9,9,9,9,4,5,7,7,7,3,10,10,10,3,5,8,8,8,4,6,11,11,11,4,5,9,9,9,1,7,7,7,6,3,10,10,10&gmb=0,0,0&sc=0.01,0.02,0.05,0.10,0.25,0.50,1.00,3.00,5.00&defc=0.01&pos=10,12,100,112,61&wilds=2~1,1,1,1,1&bonuses=0&a=0&gs=0&paytable=0,0,0,0,0;0,0,0,0,0;1000,150,50,0,0;500,75,25,0,0;350,50,20,0,0;250,40,15,0,0;200,25,10,0,0;120,20,7,0,0;120,20,7,0,0;100,15,5,0,0;100,15,5,0,0;100,15,5,0,0";
            }
        }
	
	
        #endregion
        public GloriousRomeGameLogic()
        {
            _gameID = GAMEID.GloriousRome;
            GameName = "GloriousRome";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string strInitString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, strInitString);
	    
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("psym"))
            {
                string[] strParts = dicParams["psym"].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                if (strParts.Length == 2)
                {
                    strParts[1] = convertWinByBet(strParts[1], currentBet);
                    dicParams["psym"] = string.Join("~", strParts);
                }
            }
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
                int index = (int)message.Pop();
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
                _logger.Error("Exception has been occurred in GloriousRomeGameLogic::onDoMysteryScatter {0}", ex);
            }
        }

        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "n_reel_set", "s" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]) || resultParams.ContainsKey(toCopyParams[i]))
                    continue;
                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }
            return resultParams;
        }
    }
}
