using Akka.Actor;
using Akka.Event;
using GITProtocol;
using PCGSharp;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class FishinReelsGameLogic : BaseSelFreePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10goldfish";
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
                return "def_s=5,8,7,9,8,8,7,3,4,4,11,6,8,11,10&cfgs=3960&ver=2&mo_s=13&def_sb=5,3,4,6,7&mo_v=5,10,20,30,40,50,60,80,100,120,150,200,250,300,400,500,1000,2000,2500&reel_set_size=4&def_sa=11,5,10,8,9&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"243956075\",max_rnd_win:\"1200\"}}&wl_i=tbm~1200&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;1000,200,50,5,0;500,150,25,0,0;400,125,20,0,0;300,100,15,0,0;200,50,10,0,0;200,50,10,0,0;150,20,5,0,0;150,20,5,0,0;100,20,5,0,0;100,20,5,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&reel_set0=5,3,6,7,4,9,12,10,11,1,8,6,9,3,11,9,6,9,6,3,7,3,7,11,8,9~11,12,3,10,2,6,8,7,1,5,4,9,7,12,8,2,6,1,8,6,1,7,2,1,3,2,8,12,3,10,1,2,1,3,10~10,3,12,11,6,9,7,4,1,8,5,2,5,12,3,7,5,7,11,2,9,12~4,8,9,3,6,12,1,11,7,5,10,2,5,6,10,5,8,5,6,8,5,1,11,6,8,5,6,5,6,8,7,2~12,9,1,3,6,4,7,5,11,8,10,8,10,1,8,1,3,1,3,1,5,10,7,9,7,1,4,1,8,1,4,1,3,5,1,8,1,7,1,11,6,7&reel_set2=13,13,13,10,3,11,4,5,8,9,7,13,12,6,3,9,4,3,12,3,12,9,12,4,11,4,12,9,8,10,12,11,9,8,12,6,10,12,11,10,11,10,4,11,12,10,6~13,13,13,2,12,11,7,6,8,9,4,10,13,3,5,10,3,10,8,3,2,10,3,8,2,10,2,10,7,9,3,10,3,8,10,9,8,3~4,7,10,9,12,13,11,13,13,13,2,3,5,8,6,13,3,2,13,3,6,13,3,13,2,13,11,3,13,5,13,3,13,11,2,13,5,13,5,2,8,2,13,2~13,13,13,8,10,10,10,4,5,3,2,12,6,13,11,7,9,10,11,9,11,10,5,4,10,12,5,10,9,11,9,10,4,6,10,6,4,9,10,7,12~9,8,11,6,7,10,5,14,4,12,3,5,3,6,3,10,6,4,12,5,12,8,10,3,11,8,14,11,10,14,11,10,6,4,11,8,7,4,10,3,5,11,4,5,6&reel_set1=5,11,12,10,6,7,4,9,8,3,4,7,11,9,7,8,12,4,11,10,8,10,3,7,10,8,11,9,4,7~9,6,5,2,3,12,8,10,7,4,11,12,10,6,5,7,2,12,10,7,10,6,7,6,10,2~6,4,7,8,5,2,12,11,10,9,3,9,12,11,10,9,4,5,7,8,7,4,11,10,11,4,11,8,9,4,12,9~5,2,10,10,10,4,3,8,11,9,6,10,12,7,12,6,8,11,10,7,4,7,12,11,10,12,3,10,12~12,3,5,7,8,10,9,4,6,11,6,4,11,8,10,3,10,3,9,5,10,6,5,10,11,4&reel_set3=10,3,12,7,11,8,9,3,11,9,7,12,3,9,3,7,3,11,8,11,9,3,11,3,8,7,9,7,3,9,8,7,9,12,11,3,8,12~2,11,12,10,3,8,7,9,8,3,8,11,10,11,8,3,7,3,8,10,8,3,8,10,3,10,7,9,8,10,8,10,3,8,10,3,10,8,3,10,8,10,11,3,8,10,3,11,8,7,10,3,8,3,11,10,7,10,3,8,10,7,11,3,10,7,9,10,11,7,8,10,8,3,10,11,3,12,10,3,10,3,10,11,3,10,3,11,10,8,3,10,3,10,3,8,11,7,8~8,7,10,9,12,3,11,2,3,9,2,3,10,3,2,12,2,12,2,3,2,3,2,3,2,12,9,12,2,12,10,3,10,12,2,3,9,7,12,3,2,12,2,10,2,9,2,9,12,3,12,9,3,12,3,12,2,3,2,3,2,9,2,10,3,12,3,9,3,12,2,3,2,11,3,9,10,3,12,11,2,9,12,3,9,12,10,12,3,9~10,7,11,8,2,9,3,12,9,7,8,2,7,8,9,8,7,8,9,8,7,8,12,7,9,2,9,7,9,7,8,9,2,8,7,3,12,3,7,2,8,12,2,7,8,12,8,9,8,12,7,2,7,8,11,12,9,8,12,9,8,7,9,8,7,9,2,8,7,2,7,2,8,2,7,2,9,2,7,8,9,2,9~8,3,11,7,12,10,12,10,7,10,7,12,7,10,12,11,7,12,10,11,10,12,7,12,7,12,10,11,7,12,7,10,12,7,10,7,12,7,10,3,12,10,12,7,12,11,12,10,11,10";
            }
        }
        protected override int FreeSpinTypeCount
        {
            get { return 6; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            switch (freeSpinGroup)
            {
                case 0:
                    return new int[] { 200, 201 };
                case 1:
                    return new int[] { 202, 203 };
                default:
                    return new int[] { 204, 205 };
            }
        }

        #endregion
        public FishinReelsGameLogic()
        {
            _gameID = GAMEID.FishinReels;
            GameName = "FishinReels";
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
        protected override async Task buildStartFreeSpinData(BasePPSlotStartSpinData startSpinData, StartSpinBuildTypes buildType, double minOdd, double maxOdd)
        {
            if (buildType == StartSpinBuildTypes.IsNaturalRandom)
                await base.buildStartFreeSpinData(startSpinData, StartSpinBuildTypes.IsTotalRandom, minOdd, maxOdd);
            else
                await base.buildStartFreeSpinData(startSpinData, buildType, minOdd, maxOdd);
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "s", "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set" };
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
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                int ind     = (int)message.Pop();
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BasePPSlotBetInfo       betInfo = _dicUserBetInfos[strGlobalUserID];
                    BasePPSlotSpinResult    result  = _dicUserResultInfos[strGlobalUserID];
                    if ((result.NextAction != ActionTypes.DOBONUS) || (betInfo.SpinData == null) || !(betInfo.SpinData is BasePPSlotStartSpinData))
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        BasePPSlotStartSpinData startSpinData = betInfo.SpinData as BasePPSlotStartSpinData;
                        if (ind >= startSpinData.FreeSpins.Count)
                        {
                            responseMessage.Append("unlogged");
                        }
                        else
                        {
                            BasePPSlotSpinData freeSpinData = convertBsonToSpinData(startSpinData.FreeSpins[ind]);
                            preprocessSelectedFreeSpin(freeSpinData, betInfo);

                            betInfo.SpinData = freeSpinData;
                            List<string> freeSpinStrings = new List<string>();
                            for (int i = 0; i < freeSpinData.SpinStrings.Count; i++)
                                freeSpinStrings.Add(addStartWinToResponse(freeSpinData.SpinStrings[i], startSpinData.StartOdd));

                            string strSpinResponse = freeSpinStrings[0];
                            if (freeSpinStrings.Count > 1)
                                betInfo.RemainReponses = buildResponseList(freeSpinStrings);

                            double selectedWin = (startSpinData.StartOdd + freeSpinData.SpinOdd) * betInfo.TotalBet;
                            double maxWin = startSpinData.MaxOdd * betInfo.TotalBet;

                            //시작스핀시에 최대의 오드에 해당한 윈값을 더해주었으므로 그 차분을 보상해준다.
                            sumUpWebsiteBetWin(agentID, 0.0, selectedWin - maxWin);

                            Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

                            convertWinsByBet(dicParams, betInfo.TotalBet);
                            convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);
                            result.BonusResultString = convertKeyValuesToString(dicParams);
                            addDefaultParams(dicParams, userBalance, index, counter);
                            ActionTypes nextAction = convertStringToActionType(dicParams["na"]);
                            string strResponse = convertKeyValuesToString(dicParams);

                            responseMessage.Append(strResponse);

                            //히스토리보관 및 초기화
                            if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                                addDoBonusActionHistory(strGlobalUserID, ind, strResponse, index, counter);

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
                _logger.Error("Exception has been occurred in FishinReelsGameLogic::onDoBonus {0}", ex);
            }
        }

    }
}
