using Akka.Actor;
using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class MagicMoneyMazeGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10mmm";
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
                return 0;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "cfgs=6144&ver=3&reel_set_size=8&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"2002002\",max_rnd_win:\"10000\"}}&wl_i=tbm~10000&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&purInit_e=1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;2000,200,50,5,0;0,150,30,0,0;500,100,20,0,0;500,100,20,0,0;200,50,10,0,0;200,50,10,0,0;100,25,5,0,0;100,25,5,0,0;100,25,5,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;500,100,20,0,0;0,0,0,0,0&total_bet_max=12,000,000.00&reel_set0=3,11,8,3,4,7,6,9,8,8,10,8,1,22,5,6,7~8,6,5,8,6,1,3,3,7,4,11,8,6,3,6,6,6,9,8,6,7,1,4,3,6,22,7,10,5,11,22~6,3,7,5,7,22,8,1,3,7,11,8,8,7,11,8,8,6,8,9,1,5,10,22,7,4,3,3,6,4~7,6,11,5,8,7,8,8,3,6,10,6,6,6,1,6,22,9,4,8,5,1,8,4,11,3,22,3~22,3,8,3,8,10,7,8,6,6,6,1,8,5,7,11,9,6,7,3&reel_set2=11,7,8,5,9,1,3,1,7,10,7,7,5,4,22,3,7,9,10,4,8,1,10,11,7,7,7,6,10,3,8,1,6,22,7,4,1,6,5,7,10,8,6,7,6,8,11,3,8,9,6~11,22,7,6,9,11,8,9,9,4,4,10,11,22,5,4,3,11,5,11,3,22,11,11,22,9,11,9,8,6,4,9,3,5~1,7,4,8,6,3,4,22,5,7,4,5,10,7,9,3,8,9,1,22,8,9,5,6,11,9,11,4,3,6,1,3,9,6,8,8,22,1~6,9,5,4,8,7,22,3,10,22,11,9,7,9,3,5,6,9,7,6,3,4,4,8,11,4,8~3,8,5,9,3,9,7,9,9,6,11,8,6,8,7,22,7,8,3,5,11,9,9,22,10,8,6,7,9&reel_set1=8,6,4,3,8,3,4,7,11,5,8,8,22,8,4,6,4,5,4,4,7,6,9,4,11,10,4,4~4,3,4,4,6,4,10,4,6,3,11,7,22,6,4,11,5,11,9,4,7,11,8~4,4,11,7,4,7,4,5,8,3,4,4,9,3,4,4,3,6,8,5,10,8,4,22,7,8,6,8,7,11~11,4,4,9,4,4,22,3,4,4,6,4,4,4,8,4,9,4,4,6,5,4,4,10,4,3,7~4,4,8,4,4,4,10,4,4,6,4,4,4,11,3,4,9,4,4,4,7,4,7,6,8,22,4,5,4&reel_set4=9,7,6,7,5,10,3,6,11,6,8,6,7,3,6,5,9,4,22,6,11,9,1,7,4,10,22,4,7,22,3,10,1,3,10,6,7,11,8,1,8~4,9,8,11,10,9,11,5,7,9,11,9,22,3,4,9,22,11,3,11,11,6,11,11,22,6,5,5,6,9,4,8,4,22,9,9,3~11,5,11,4,6,22,4,8,6,7,9,7,9,10,3,7,6,22,9,9,8,3,8,8,3,5,7,7,3,4~4,11,8,3,8,3,9,6,3,22,5,7,4,5,6,3,10,9,9,3,8,4,4,7,22,6,9,9,7,11,7,4~6,22,6,11,7,3,8,22,1,7,6,11,7,3,9,9,3,5,3,3,1,10,1,7,8,9,8,6,8,9,5,9,9&purInit=[{bet:1200,type:\"default\"}]&reel_set3=4,3,8,10,7,7,4,6,22,9,22,6,9,7,7,7,11,5,8,10,3,8,10,7,6,8,6,7,3,5,11~3,11,9,1,22,11,7,3,9,11,1,9,6,11,3,10,22,5,3,11,22,9,11,8,9,4,1,9,6,4,8,4,5,5,11~3,5,3,9,7,6,11,8,4,8,7,9,8,6,8,4,11,10,3,4,6,8,8,5,9,22,9,4,7,7,22,3,9,3,7~11,3,7,6,9,4,22,3,3,1,3,6,9,9,11,1,8,6,4,1,7,3,22,5,4,6,8,4,10,1,7,4~11,8,7,5,3,8,6,11,7,6,9,9,22,9,8,6,5,3,8,7,22,9,3,10,3&reel_set6=8,7,22,9,5,10,7,11,7,6,9,6,11,7,7,7,6,4,8,4,3,7,3,6,10,8,22,8,10,5,7,3~4,9,22,11,7,11,5,11,3,11,22,6,9,3,5,3,9,10,9,11,11,4,5,4,5,11,11,4,11,22,8,11,3,9,3,9,4,6,11,9,8,6~8,8,4,5,6,5,9,3,9,6,22,4,4,7,11,3,3,7,8,4,9,3,8,8,7,7,3,6,10,7,22,9,7,5,7,9,8,8,11,4,11~4,11,8,3,4,10,3,9,5,11,8,4,8,22,6,3,6,7,6,5,9,9,7,22~3,7,11,3,22,5,9,3,8,11,6,9,9,8,9,9,5,8,6,9,3,10,7,6,3,8,9,6,7,8,22,8&reel_set5=10,8,6,7,9,11,7,4,6,9,9,8,8,9,22,5,7,11,7,3,6,7,7,7,6,4,10,8,6,3,10,6,4,3,8,11,6,8,7,7,3,11,22,5,7,10,8,5~4,11,9,3,9,9,5,9,11,11,22,8,22,11,4,9,10,6,5,9,4,8,5,22,3,6,11,3,11,7~4,1,7,9,3,7,4,5,9,9,10,6,1,7,4,3,8,22,7,1,11,4,7,9,6,7,5,4,3,8,9,22,11,8,5,9,3,8,3,8,11~22,6,3,7,4,5,8,11,8,4,9,9,6,5,9,7,8,4,10,6,7,11,7,3,6,8,9,22,9,4,4,3~8,9,22,1,6,9,8,1,3,6,5,8,6,8,3,7,3,9,7,22,8,5,8,11,9,10,9,11,1,7&reel_set7=7,5,7,11,1,6,3,8,1,8,3,9,4,10,6,4,1,8,22~1,4,1,4,6,9,3,7,5,6,6,8,7,11,3,1,22,1,3,11,7,8,10,8,1,5~7,4,1,8,5,10,8,9,1,11,22,5,8,3,6,7,11,3,7,1,4,6~22,5,6,7,11,4,8,7,8,9,8,1,3,7,1,5,11,8,1,4,3,6,1,10,6,3,1~22,10,5,11,1,8,7,9,1,3,1,7,1,6,8,7,8,1,3,6,11,5&total_bet_min=20.00";
            }
        }
	
        protected override double PurchaseFreeMultiple
        {
            get { return 120; }
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
        public MagicMoneyMazeGameLogic()
        {
            _gameID = GAMEID.MagicMoneyMaze;
            GameName = "MagicMoneyMaze";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
			dicParams["g"] = "{base:{def_s:\"5,10,7,22,10,4,11,11,6,8,22,9,10,3,9\",def_sa:\"22,8,3,3,8\",def_sb:\"6,10,6,6,7\",s:\"5,10,7,22,10,4,11,11,6,8,22,9,10,3,9\",sa:\"22,8,3,3,8\",sb:\"6,10,6,6,7\",sh:\"3\",st:\"rect\",sw:\"5\"},respin:{def_s:\"18,13,13,16,13,13,19,13,13,13,16,13,13,13,13,13,13,16,13,13,13,16,12,13,12,13,12,16,13,13,13,12,13,13,13,13,13,13,16,13,13,13,20,13,13,16,13,13,21\",def_sa:\"0,0,0,0,0,0,0\",def_sb:\"0,0,0,0,0,0,0\",s:\"18,13,13,16,13,13,19,13,13,13,16,13,13,13,13,13,13,16,13,13,13,16,12,13,12,13,12,16,13,13,13,12,13,13,13,13,13,13,16,13,13,13,20,13,13,16,13,13,21\",sa:\"0,0,0,0,0,0,0\",sb:\"0,0,0,0,0,0,0\",sh:\"7\",st:\"rect\",sw:\"7\"}}";
        }
        protected override void supplementInitResult(Dictionary<string, string> dicParams, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {
            base.supplementInitResult(dicParams, betInfo, spinResult);
            if(!dicParams.ContainsKey("reel_set"))
                dicParams["reel_set"] = "0";

        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("pw"))
                dicParams["pw"] = convertWinByBet(dicParams["pw"], currentBet);
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in MagicMoneyMazeGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in MagicMoneyMazeGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override void onDoCollect(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    _logger.Error("{0} result information has not been found in MagicMoneyMazeGameLogic::onDoCollect.", strGlobalUserID);
                    return;
                }

                BasePPSlotSpinResult result = _dicUserResultInfos[strGlobalUserID];
                if (result.NextAction != ActionTypes.DOCOLLECT)
                {
                    _logger.Error("{0} next action is not DOCOLLECT just {1} in MagicMoneyMazeGameLogic::onDoCollect.", strGlobalUserID, result.NextAction);
                    return;
                }
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                Dictionary<string, string> responseParams = new Dictionary<string, string>();
                responseParams.Add("balance", Math.Round(userBalance, 2).ToString());
                responseParams.Add("balance_cash", Math.Round(userBalance, 2).ToString());
                responseParams.Add("balance_bonus", "0.00");
                responseParams.Add("na", "s");
                responseParams.Add("stime", GameUtils.GetCurrentUnixTimestampMillis().ToString());
                responseParams.Add("sver", "5");
                responseParams.Add("index", index.ToString());
                responseParams.Add("counter", (counter + 1).ToString());

                GITMessage reponseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOCOLLECT);
                string strCollectResponse = convertKeyValuesToString(responseParams);
                reponseMessage.Append(strCollectResponse);

                bool needDelay = false;
                if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    Dictionary<string, string> dicParams = splitResponseToParams(_dicUserResultInfos[strGlobalUserID].ResultString);
                    if (dicParams.ContainsKey("apv"))
                        needDelay = true;
                }
                if (needDelay)
                    Context.System.Scheduler.ScheduleTellOnce(250, Sender, new ToUserMessage((int)_gameID, reponseMessage), Self);
                else
                    Sender.Tell(new ToUserMessage((int)_gameID, reponseMessage), Self);

                addActionHistory(strGlobalUserID, "doCollect", strCollectResponse, index, counter);
                saveHistory(agentID, strGlobalUserID, index, counter, userBalance, currency);

                if (_dicUserFreeSpinInfos.ContainsKey(strGlobalUserID))
                {
                    addFreeSpinBonusParams(reponseMessage, _dicUserFreeSpinInfos[strGlobalUserID], strGlobalUserID, 0.0);
                    checkFreeSpinCompletion(reponseMessage, _dicUserFreeSpinInfos[strGlobalUserID], strGlobalUserID);
                }

                result.NextAction = ActionTypes.DOSPIN;
                saveBetResultInfo(strGlobalUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in MagicMoneyMazeGameLogic::onDoCollect {0}", ex);
            }
        }
    }
}
