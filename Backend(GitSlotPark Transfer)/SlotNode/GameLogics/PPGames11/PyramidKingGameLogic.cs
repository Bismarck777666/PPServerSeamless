using Akka.Event;
using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class PyramidKingGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25pyramid";
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
            get { return 25; }
        }
        protected override int ServerResLineCount
        {
            get { return 25; }
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
                return "def_s=7,5,11,3,2,1,9,11,10,2,6,6,11,9,2&cfgs=3085&ver=2&mo_s=11&reel_set_size=11&def_sb=5,9,3,10,3&mo_v=25,50,75,100,125,150,175,200,250,350,400,450,500,600,750,2500&def_sa=4,9,7,9,10&mo_jp=750;2500;25000&scatters=1~0,0,1,0,0~0,0,10,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&mo_jp_mask=jp3;jp2;jp1&reel_set10=9,5,7,6,10,7,8,3,10,8,7,9,11,11,8,8,9,5,8,2,5,7,8,5,4,8,11,9,8,2,3,4,5,1,4,8,6,7,9~4,2,9,3,7,5,9,6,7,10,3,8,10,6,11,11,8,4,8,7,6,2,3,10,9,6,4,8,11,6,8,2,10,4,5,7,8,10,9,7,10~2,6,9,8,9,6,5,9,1,10,4,2,10,11,11,9,5,6,7,10,2,9,10,4,3,10,9,11,10,6,2,10,8,5,10,8,4,9,7,4~10,8,2,4,6,10,7,9,5,8,6,7,8,2,7,11,11,5,9,7,8,3,2,6,9,3,8,4,5,11,9,3,2,10,5,4,10,7,4,9,7,6~2,5,10,9,7,3,5,1,10,4,8,2,6,11,11,8,3,6,1,9,2,10,9,4,10,9,8,11,9,5,2,10,1,4,10,7,8,9,1&sc=8.00,16.00,24.00,32.00,40.00,80.00,120.00,160.00,200.00,300.00,400.00,600.00,1000.00,2000.00,3000.00,4000.00&defc=40.00&wilds=2~500,250,25,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;500,250,25,0,0;400,150,20,0,0;300,100,15,0,0;200,50,10,0,0;50,20,10,0,0;50,20,5,0,0;50,20,5,0,0;50,20,5,0,0;0,0,0,0,0;0,0,0,0,0&reel_set0=6,8,7,1,6,9,4,5,8,4,2,2,2,7,6,10,7,3,10,8,7,9,11,11,11,8,9,8,9,5,7,8,5,6,8,5,9,8~9,7,8,10,9,7,2,2,7,5,9,6,7,10,3,8,6,11,11,11,4,8,7,6,9,3,10,9,6,4,8,10,8,5,10,4,5,7,8,9,7,10~7,1,10,8,6,9,10,3,2,2,2,8,9,5,9,1,10,4,6,10,11,11,11,5,6,7,10,6,9,4,3,10,9,4,10,6,9,10,8,5~9,7,3,10,8,5,7,9,10,8,2,2,3,10,7,9,5,8,6,7,8,10,7,11,11,6,9,7,8,3,4,6,9,3,8,4,5,6,9,3,6~7,9,1,10,7,5,3,9,10,3,2,2,2,9,7,3,5,1,10,4,8,10,6,11,11,11,3,6,1,9,5,10,9,4,10,9,8,10,9,5,4&reel_set2=8,3,9,8,7,9,11,10,3,8,9,5,8,9,5,7,2,5,3,8,5,11,11,5,3,8,5,1,4,6,2,7,9~5,8,11,11,7,4,2,10,9,7,5,3,6,7,10,3,8,6,10,11,6,10,4,8,7,6,9,3,10,2,6,4,8,10,11,8,5,2,3,5,7,8,2,9,7,6~10,4,2,3,8,4,9,6,5,9,1,10,4,6,10,11,9,10,3,6,7,4,6,9,10,2,3,10,6,4,11,11,9,10,3,5,10,8,2,5,7,6~11,11,10,8,2,4,3,5,7,3,5,8,6,7,2,10,7,9,11,7,9,6,7,3,4,6,9,2,8,4,5,6,11,3,2,10,5,4,10,7,2,9,7,6~3,2,8,4,9,2,6,5,1,10,4,2,10,6,11,9,3,10,6,1,9,5,3,9,2,10,4,8,10,11,11,2,10,1,4,6,2,7,9,1,8&reel_set1=7,8,4,9,8,7,6,5,7,8,3,9,8,7,9,11,10,3,8,9,5,8,9,5,7,2,5,3,8,5,11,11,5,3,8,5,1,4,6,2,7,9~11,11,7,4,2,10,9,7,5,3,6,7,10,3,8,6,10,11,6,10,4,8,7,6,9,3,10,2,6,4,8,10,11,8,5,2,3,5,7,8,2,9,7,6~11,9,10,4,2,3,8,4,9,6,5,9,1,10,4,6,10,11,9,10,5,6,7,4,6,9,10,2,3,10,6,4,11,11,9,10,3,5,10,8,10,5,7,6~9,3,7,10,4,5,11,11,10,8,2,4,3,5,7,3,5,8,6,7,2,10,7,9,11,7,9,6,7,3,4,6,9,2,8,4,5,6~8,4,9,2,6,5,1,10,4,2,10,6,11,9,3,10,6,1,9,5,3,9,2,10,4,8,10,11,11,2,10,1,4,6,9,7,9,1,8&reel_set4=7,8,4,9,8,7,6,5,7,8,3,9,8,7,9,11,5,3,8,9,5,10,8,9,5,7,2,5,3,8,5,11,11,5,3,8,5,1,4,6,2,7,9~9,8,7,10,5,8,11,11,7,5,6,10,11,6,10,4,8,7,6,9,3,10,2,6,4,8,10,11,8,5,2,3,5,7,8,2,9,7,6~9,10,4,2,3,8,4,9,6,5,9,1,10,5,6,10,11,9,10,5,6,7,4,6,9,10,2,3,10,6,4,11,11,9,10,3,5,10,8,2,5,7,6~9,3,7,10,4,5,11,11,6,8,2,4,9,11,7,9,6,7,3,4,6,9,2,8,4,5,6,11,3,2,10,5,4,10,7,2,9,7,6~9,10,3,2,8,4,9,2,6,5,1,10,4,2,10,6,11,9,5,10,6,1,9,5,3,9,2,10,4,8,10,11,11,2,10,1,4,6,2,7,9,1,8&reel_set3=11,4,7,8,4,9,8,4,6,5,7,8,3,9,8,7,9,11,10,3,8,9,5,8,9,5,7,2,5,4,8,5,11,11,5,3,8,5,1,4,6,2,7,9~9,8,7,10,5,8,11,11,7,4,2,10,9,7,5,3,6,7,10,3,8,6,10,11,6,10,4,8,7,4,9,3,10,2,6,4,8,10~9,10,4,2,3,8,4,9,6,5,9,1,10,4,6,10,11,9,10,5,6,7,4,6,9,10,2,3,10,6,4,11,11,9,10,3,5,10,8,2,5,7,6~11,11,10,8,2,4,3,5,7,3,5,8,6,7,2,10,7,9,11,7,9,6,7,3,4,6,9,2,8,4,5,6,11,3,2,10,5,4,10,7,2,4,7,6~9,10,3,2,8,4,9,2,6,5,1,10,4,2,10,6,11,9,3,10,6,1,9,5,3,9,2,10,4,8,10,11,11,2,10,1,4,6,2,7,9,1,8&reel_set6=4,7,8,4,9,8,7,6,5,7,8,3,9,8,7,9,11,10,3,8,9,7,8,9,5,7,2,5,3,8,5,11,11,5,3,8,5,1,4,6,2,7,9~11,11,7,4,2,10,9,7,5,3,6,7,10,3,8,6,10,11,7,10,4,8,7,6,9,3,10,7,6,4,8,7,11,8,5,2,3,5,7,8,2,9,7,6~9,10,4,2,3,8,4,9,6,5,9,1,10,4,6,10,11,9,10,5,6,7,4,6,7,10,2,3,10,6,4,11,11,9,10,3,5,7,8,2,5,7,6~11,11,10,7,2,4,3,5,7,3,5,8,6,7,2,10,7,9,11,7,9,6,7,3,4,6,9,2,8,4,5,6,11,3,2,10,5,4,10,7,2,9,7,6~9,10,3,2,8,4,9,2,6,7,1,10,4,2,10,6,11,9,3,10,6,1,9,5,7,9,2,10,4,7,10,11,11,2,10,1,4,6,2,7,9,1,8&reel_set5=7,8,4,9,8,7,6,5,7,8,3,9,8,7,9,11,10,3,8,6,5,8,9,5,7,2,5,3,6,5,11,11,5,3,8,5,1,4,6,2,7,9~9,8,7,10,5,8,11,11,7,4,2,10,9,7,5,3,6,7,10,3,8,6,10,11,6,10,4,8,7,6,9,3,10,2,6,4,8,10~9,6,4,2,3,8,4,9,6,5,9,1,10,4,6,10,11,9,10,5,6,7,4,6,9,10,2,3,10,6,4,11,11,9,10,3,5,10,8,2,5,7,6~9,3,7,10,4,5,11,11,10,8,2,4,3,5,7,3,5,8,6,7,2,10,6,9,11,7,9,6,7,3,4,6,9~9,10,3,2,8,4,9,2,6,5,1,10,4,2,10,6,11,9,3,10,6,1,9,5,3,9,2,10,4,8,10,11,11,2,10,1,4,6,2,7,9,1,8&reel_set8=9,7,8,4,9,8,7,6,9,7,8,3,9,8,7,9,11,10,3,8,9,5,8,9,5,7,2,9,3,8,5,11,11,5,3,8,9,1,4,9,2,7,9~11,11,7,4,2,10,9,7,5,9,6,7,10,3,8,9,10,11,6,10,4,9,7,6,9,3,10,2,6,4,8,10,11,9,5,2,3,5,9,8,2,9,7,6~9,10,4,2,3,8,4,9,6,5,9,1,10,4,6,10,11,9,10,5,6,7,4,6,9,10,2,3,10,6,9,11,11,9,10,3,5,10,8,2,5,7,6~11,11,10,8,2,4,3,5,7,3,5,8,6,7,2,10,7,9,11,7,9,6,7,3,4,6,9,2,8,4,5,6,11,3,2,10,5,9,10,7,2,9,7,6~9,10,3,2,8,4,9,2,6,5,1,10,9,2,10,6,11,9,3,10,6,1,9,5,3,9,2,10,4,8,10,11,11,2,10,1,4,6,2,7,9,1,8&reel_set7=4,7,8,4,9,8,7,6,5,7,8,3,9,8,7,9,11,10,3,8,9,5,8,9,5,7,2,5,3,8,5,11,11,5,3,8,5,1,4,6,2,8,9~11,11,7,4,2,10,9,7,5,3,6,7,10,3,8,6,10,11,8,10,4,8,7,6,9,3,10,2,6,4,8,10,11,8,5,2,3,5,7,8,2,9,7,6~9,10,4,2,3,8,4,9,6,5,9,1,10,8,6,10,11,9,10,5,6,7,4,6,9,10,2,8,10,6,4,11,11,8,10,3,5,10,8,2,5,7,8~11,11,10,8,2,4,8,5,7,3,5,8,6,7,2,10,7,8,11,7,8,6,7,3,8,6,9,2,8,4,5,6,11,3,2,10,5,8,10,7,2,9,8,6~9,8,3,2,8,4,9,2,6,5,1,10,4,2,10,6,11,8,3,10,6,1,9,5,3,8,2,10,4,8,10,11,11,2,10,1,4,6,2,7,9,1,8&reel_set9=11,10,4,9,10,7,6,5,7,8,3,9,10,7,9,11,10,3,8,10,5,8,9,10,7,2,5,3,8,10,11,11,5,3,8,10,1,4,6,2,7,9~11,11,7,4,2,10,9,7,5,3,6,7,10,3,8,6,10,11,6,10,4,8,7,6,9,3,10,2,6,4,8,10,11,8,5,2,3,5,7,8,2,9,7,6~9,10,4,2,3,8,4,10,6,5,9,1,10,4,6,10,11,9,10,5,6,7,4,6,9,10,2,3,10,6,4,11,11,9,10,3,5,10,8,2,5,7,6~11,11,10,8,2,4,10,5,7,3,5,10,6,7,2,10,7,9,11,10,9,6,7,3,10,6,9,2,8,4,5,10,11,3,2,10,5,4,10,7,2,9,7,6~9,10,3,2,8,4,9,2,6,5,1,10,4,2,10,6,11,9,3,10,6,1,9,5,3,9,2,10,4,8,10,11,11,2,10,1,4,6,2,7,9,1,8";
            }
        }
	
	
        #endregion
        public PyramidKingGameLogic()
        {
            _gameID = GAMEID.PyramidKing;
            GameName = "PyramidKing";
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
        protected override async Task onProcMessage(string strUserID, int agentID,  GITMessage message, UserBonus userBonus, double userBalance, Currencies currency)
        {
            if (message.MsgCode == (ushort)CSMSG_CODE.CS_PP_DOMYSTERYSCATTER)
                onDoMysteryScatter(agentID, strUserID, message, userBonus, userBalance);
            else
                await base.onProcMessage(strUserID, agentID, message, userBonus, userBalance, currency);
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
                _logger.Error("Exception has been occurred in PyramidKingGameLogic::onDoMysteryScatter {0}", ex);
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
