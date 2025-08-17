using Akka.Actor;
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
    class ThreeBuzzingWildsGameLogic : BaseSelFreePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20wildparty";
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
                return 4;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=3,9,7,3,9,6,8,6,6,8,5,7,4,5,7,6,10,9,3,9&cfgs=8160&ver=3&def_sb=5,9,3,3,9&reel_set_size=8&def_sa=3,7,6,6,7&scatters=1~50,10,2,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"5787037\",max_rnd_win:\"5000\"}}&wl_i=tbm~5000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1&wilds=2~1000,300,100,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;1000,300,100,0,0;400,100,50,0,0;300,75,40,0,0;200,60,30,0,0;100,40,20,0,0;80,25,10,0,0;50,10,5,0,0;50,10,5,0,0&total_bet_max=10,000,000.00&reel_set0=4,4,9,4,10,3,10,7,4,10,8,10,6,3,9,2,10,9,10,5,9,9,3,3,10,9,10,5,6,5,4,9,7,9,1,6,9,4,10~9,7,8,6,6,4,6,9,4,9,7,6,8,9,10,9,4,3,9,6,5,8,6,6,8,7,8,6,9,7,6,6,9,6,5,9,7,3,6,8,9,8,8,7,10,2,3,8,8,1,8,6,10,5,9,8,2,8,6,6,9~10,10,5,10,10,5,5,1,6,10,8,5,10,4,10,5,4,7,10,8,7,5,10,7,10,10,5,3,7,4,2,7,3,10,10,9,10,10,5,8,4,9,7,4,5,6,7,3,5,7,10,10,8,10,5,10,10,7,4,7~7,10,6,8,9,6,4,8,8,9,9,8,7,3,9,8,3,5,1,6,9,6,9,9,8,6,6,2,1,5,8,9,3,10,5,7,10,9,7,9~9,10,10,6,3,4,5,9,7,8,10,10,7,10,10,6,10,10,7,4,7,10,8,7,10,1,6,10,7,2,4,9,10,5,6,3,8,10,3,10,7,10,3,9,3,4,7,9,5,3,10,4,7,10,5&reel_set2=8,5,10,9,4,9,8,10,2,6,10,9,10,9,9,8,4,10,7,6,10,9,7,10,5,2,9,9,7,3,9,6,10,8,10,4,3,7,9,3,5,8,7,9~6,8,8,10,10,9,8,8,3,4,8,9,2,9,7,8,2,6,9,6,10,2,6,9,10,10,5,10,6,2,6,8,7,10,7,9,7,8,2,5,9,8,5,9,7,9,8,6,9,10,2,8,9,6,10,8,3,7,8,6~5,3,10,7,10,5,4,10,7,10,10,8,3,10,2,9,2,8,9,4,10,10,7,6,8,2,8,10,6,10,10,5,10,4,7,10,7,3,10,5,10,7,7,10,2,9,2,5,6,10,7,10,9,4,10,5,7,10,7,8~8,7,6,6,9,6,8,9,7,2,9,9,7,9,8,9,10,6,8,8,4,10,8,10,3,8,9,6,8,2,9,7,8,9,3,9,3,9,9,5,6,5,6,6~4,10,10,9,7,10,10,2,10,2,3,7,5,10,9,10,10,3,6,10,10,9,7,6,5,4,7,7,10,8,7,4,7,3,10,3,9,8,10,10,6,5,4&reel_set1=10,9,9,10,4,7,9,10,9,9,4,5,10,3,8,4,3,8,7,10,5,7,9,6,7,10,3,4,6,8,9~8,5,7,6,4,9,10,5,6,10,9,6,3,9,8,6,8,8,10,6,10,3,10,7,8,8,7,9,8,9,6,9,4,6,10,9,6,3,8,9,7~5,10,8,10,9,7,3,10,5,3,5,7,4,10,10,7,3,10,5,8,10,4,10,7,4,5,10,9,10,5,7,10,4,10,10,8,10,10,7,3,8,6,10,9,10,8,7,10,6,10,5,7,10,6,9,4,7,7~9,5,6,8,3,6,6,9,7,9,8,9,10,8,9,4,3,9,9,4,6,5,7,9,9,8,9,9,6,6,10,9,6,5,10,5,3,8,6,8,7,8,6,8,9,7,9,8,3,9,8,9,6,9,8,8,4,8~7,10,6,4,3,4,10,5,4,7,9,3,10,3,10,9,5,7,10,4,5,10,7,6,10,6,7,10,3,8,10,9,8,3,9,10,10,7&reel_set4=9,10,9,6,9,3,9,8,10,9,10,8,10,8,10,10,4,7,4,8,7,10,3,9,9,8,9,9,5,10,10,5,9,4,10,9,6,3,5,7,10~8,9,8,5,6,10,4,9,6,3,6,8,9,10,10,8,8,10,6,7,6,9,8,8,5,7,6,9,8,7,10,8,10,3,10,7,3,8,9,10,10,9,4,5,8,7~9,10,10,4,10,10,3,5,8,10,10,9,8,6,10,10,9,5,9,3,4,7,10,7,10,8,10,7,7,9,4,10,8,9,7,4,5,7,5,10,7,10,7,5,10,10,4~8,6,8,9,6,9,5,10,8,9,4,3,8,10,7,8,5,3,8,9,9,4,9,8,8,9,8,3,7,8,4,9,6,8,9,9,10,3,9,10,9,6,10,5,7,8,9,7,6,9,9,10~10,3,9,6,5,7,9,10,4,10,7,7,10,10,7,3,10,7,10,8,10,10,7,10,10,3,7,6,8,10,8,10,9,7,10,7,10,3,9,5,10,8,4,7,8,7,9,3,5,4,3,10,5,7,10,10,4,8,4,9&purInit=[{bet:2000,type:\"fs\"}]&reel_set3=7,2,3,9,9,4,7,10,8,9,5,9,8,9,10,9,6,7,6,8,10,4,6,5,3,8,10,9,4,8,3,4,10,9,10,4,9,9,10,3,10,6,9,5,7,8,3,10,5,7,10,9~2,9,5,7,8,10,3,7,6,6,3,8,8,9,3,9,10,6,9,8,7,6,8,10,8,9,8,7,6,9,4,10,6,9,6,10~10,4,5,10,6,4,8,7,5,3,10,10,4,7,10,3,5,8,2,10,5,10,10,8,9,3,7,7,5,10,7,10,9,10,6,7,7,9,10,8,10,9,10~9,9,10,9,8,6,8,7,9,9,5,6,8,5,8,2,5,9,8,9,6,8,3,6,3,10,9,7,10,6,4,8,3,6,8,7,9,9,2~5,8,4,9,10,9,3,10,3,4,10,10,5,10,6,7,10,10,4,10,10,3,6,10,7,2,7,3,4,6,7,10,10,7,7,10,10,3,7,8&reel_set6=10,6,10,10,3,9,3,7,10,9,5,7,4,10,7,4,8,9,6,10,3,9,3,9,8,5,10,10,9,8,9,7,5,9,10,9,9,4,10,8~7,10,9,3,8,7,8,10,9,5,8,8,7,10,10,8,5,8,9,7,4,6,5,10,7,10,6,3,9,6,9,8,8,9,10,6,10,8,6,8,8,9,3,10,6,4,7,9,10,3,8,9~7,10,10,4,5,10,9,10,7,4,6,10,10,9,7,4,8,4,7,10,5,10,10,8,3,10,8,7,8,10,9,10,7,10,10,9,7,10,8,7,9,10,7,5,10,5,8,10,3,4,8,10,10,4,7,3,5,9,5,4,5,3~8,9,8,5,6,10,9,8,9,9,10,8,9,6,8,7,9,8,8,9,7,9,7,9,9,6,10,5,6,10,3,9,4,10,8,8,9,6,3,7,8,7,3,9,4~10,8,10,7,3,10,4,10,10,7,6,10,10,5,9,10,4,8,9,10,9,10,8,7,5,7,4,7,3,7,3,7,3,10&reel_set5=10,3,8,2,7,9,6,10,6,4,9,9,10,10,5,8,10,10,7,8,7,10,6,10,4,9,6,10,6,8,7,6,7,10,10,7,10,10,5,9,7,10,9,10,9,10,10,9,9,5,7,9,8,4,9,9,10,9,7,6,10,9,9,7,10,5,10,10,8,6,4,10,9,4,9,6,8,4,5,9,10,10,9,9,10,9,3,8,10,7,9,9,4,7,3,9,4,10,9,5,8,4,9,9,10,8,2,8,9,4,10,3,8,9,3,6,9,10,5,9,7,10,10,4,5,9,5,8,10,8,6,9,10,10,7,6,9,10,5,9,9,7,9,5~8,8,9,3,10,2,9,4,10,9,8,9,8,9,10,8,8,10,6,8,6,9,10,6,10,7,6,8,7,8,7,5,7,5~7,10,10,4,8,2,8,5,4,10,7,10,7,9,6,8,3,10,5,7,8,10,9,10,7,10,9,4,9,8,10,5,9,10~3,9,6,4,8,9,4,9,7,9,8,6,4,8,9,10,9,9,8,8,3,8,9,7,6,7,8,7,3,7,6,8,6,8,10,8,9,6,10,9,10,9,7,9,9,7,10,3,9,10,3,9,8,8,10,9,7,3,10,8,10,9,5,9,9,8,8,9,8,4,9,9,8,9,8,2,8,10,9,8,5,6,8,10,5,9,4,9,8,8,6,7,3,9~10,8,4,8,2,10,7,10,7,10,8,9,10,10,7,5,9,5,9,10,7,10,4,7,7,9,10,3,9,10,3,10,5,7,9,10,10,6,8,7,3,7,10,2,4,7,4,3,10,7,7,10,8,10,7,3,10,4,10,6,4,10,3,10,10,9,4,7,10,10,9,10,9,3,4,3,9,7,5,10,4,8,3,7,10,5,8,10,10,3,10,9,7,7,6,10,5,8,10,3,10,7,6,9,7,5,10,7,5,7,9,10,3,10,8,6,10,10,3,10,7,4,9,10,10,9,10,7,8,7,10,10,4,3,10,8,10,10,7,3,9,10,10,9,8,7,5,7,10,8,10,7,9,10,8,7,3&reel_set7=10,4,4,7,4,9,4,6,10,8,9,10,10,9,5,3,9,9,10,10,9,5,3,10,4,10,10,3,7,9,6,10,3,10,8,10,10,6,9,3,9,9,4,2,7,4,8,9,10,9,9,4,6,5,4,3,5~7,9,7,6,8,9,3,7,9,6,4,7,9,10,8,2,9,8,6,6,8,6,8,8,9,6,10,5,9,6,8,9,6,9,2,6,3,4,8,6,8,6,8,5~7,5,8,5,5,10,5,4,10,4,7,10,5,7,7,5,7,5,10,6,4,3,10,7,8,10,10,5,9,10,10,5,4,10,4,10,7,10,10,5,9,10,7,7,4,6,7,10,2,10,3,8,10,8,10,5,10,3~7,6,2,8,10,9,3,9,5,8,5,6,8,4,8,6,10,9,10,9,8,4,3,9,8,7,9,3,7,9,6,8,7,9,9,7,6,8,9,5,3,9,8,6,9,9,8,6,6,8,10,8,9,9,6~7,8,3,10,4,3,6,9,10,2,10,10,6,8,10,9,4,9,4,7,10,7,4,9,10,6,7,9,10,4,10,3,9,10,7,7,5,7,10,7,10,3,10,10,5,7,8,3,10,7,5,3,10,10,5,6,10,6,10,3,7,10&total_bet_min=10.00";
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
        protected override int FreeSpinTypeCount
        {
            get { return 3; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            return new int[] { 200, 201, 202};
        }
        #endregion
        public ThreeBuzzingWildsGameLogic()
        {
            _gameID = GAMEID.ThreeBuzzingWilds;
            GameName = "ThreeBuzzingWilds";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"]   = "0";
	        dicParams["st"]         = "rect";
	        dicParams["sw"]         = "5";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in ThreeBuzzingWildsGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in ThreeBuzzingWildsGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override async Task buildStartFreeSpinData(BasePPSlotStartSpinData startSpinData, StartSpinBuildTypes buildType, double minOdd, double maxOdd)
        {
            if (buildType == StartSpinBuildTypes.IsNaturalRandom)
                await base.buildStartFreeSpinData(startSpinData, StartSpinBuildTypes.IsTotalRandom, minOdd, maxOdd);
            else
                await base.buildStartFreeSpinData(startSpinData, buildType, minOdd, maxOdd);
        }
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency, bool isAffiliate)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                int ind     = (int)message.Pop();
                var strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BasePPSlotBetInfo    betInfo    = _dicUserBetInfos[strGlobalUserID];
                    BasePPSlotSpinResult result     = _dicUserResultInfos[strGlobalUserID];
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
                            if (!startSpinData.IsEvent && !isAffiliate)
                                sumUpWebsiteBetWin(agentID, 0.0, selectedWin - maxWin);
                            else if (maxWin > selectedWin)
                                addEventLeftMoney(agentID, strUserID, maxWin - selectedWin);

                            Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);
                            convertWinsByBet(dicParams, betInfo.TotalBet);
                            convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);
                            if (SupportMoreBet)
                            {
                                if (betInfo.MoreBet)
                                    dicParams["bl"] = "1";
                                else
                                    dicParams["bl"] = "0";
                            }
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
                _logger.Error("Exception has been occurred in ThreeBuzzingWildsGameLogic::onDoBonus {0}", ex);
            }
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "s", "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set", "sw", "st", "bw" };
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

    }
}
