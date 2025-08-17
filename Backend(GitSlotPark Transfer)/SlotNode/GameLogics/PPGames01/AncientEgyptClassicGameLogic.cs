using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Event;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    public class AncientEgyptClassicGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10egyptcls";
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
                return 10;
            }
        }
        protected override int ServerResLineCount
        {
            get
            {
                return 10;
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
                return "wsc=1~bg~200,20,2,0,0~0,0,0,0,0&def_s=6,7,10,4,9,5,11,1,8,3,8,4,8,10,6&cfgs=2479&ver=2&reel_set_size=2&def_sb=5,3,4,6,7&def_sa=11,11,10,8,9&scatters=&gmb=0,0,0&rt=d&sc=20.00,30.00,40.00,50.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,2000.00,3000.00,4000.00,5000.00,6000.00,7000.00,8000.00,10000.00&defc=200.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;5000,1000,100,10,0;2000,400,40,5,0;750,100,25,5,0;750,100,25,5,0;150,40,5,0,0;150,40,5,0,0;150,40,5,0,0;100,25,5,0,0;100,25,5,0,0&rtp=95.50&reel_set0=9,6,9,5,4,10,8,7,9,8,7,11,8,5,1,10,5,11,8,4,3,8~7,8,9,10,5,11,6,10,7,10,8,8,11,1,3,4,4,11,6,8,10,11~5,10,9,6,8,11,1,11,7,9,11,3,10,3,9,4,10,8,5,7,7~9,7,3,10,10,11,11,9,1,3,8,11,7,5,6,4,5,10,8,9~10,1,10,11,9,6,4,9,8,3,7,5,8,5,9,1,7,10&reel_set1=5,7,5,1,9,8,11,4,4,11,7,6,6,5,3,9,8,3,10,10,8~4,1,9,6,8,6,7,4,9,11,3,5,8,7,3,10,11,7,10,10~11,1,4,10,9,11,3,9,3,8,8,6,10,9,6,5,5,10,4,11,11,7~4,11,9,9,10,7,11,5,8,11,4,7,3,10,6,1,7,10~9,9,8,4,7,4,6,5,3,11,11,10,7,3,7,10,10,1,9,1,8,6";
            }
        }
        #endregion

        public AncientEgyptClassicGameLogic()
        {
            _gameID     = GAMEID.AncientEgyptClassic;
            GameName    = "AncientEgyptClassic";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["n_reel_set"] = "0";
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
                        BasePPSlotBetInfo       betInfo         = _dicUserBetInfos[strGlobalUserID];
                        BasePPActionToResponse  actionResponse  = betInfo.pullRemainResponse();
                        if(actionResponse.ActionType != ActionTypes.DOMYSTERY)
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
                _logger.Error("Exception has been occurred in AncientEgyptClassicGameLogic::onDoMysteryScatter {0}", ex);
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
