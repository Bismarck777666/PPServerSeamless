using Akka.Util;
using GITProtocol;
using Newtonsoft.Json.Linq;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class TheDogHouseMultiholdGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20doghousemh";
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
                return 3;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=4,13,7,7,10,3,11,4,5,13,6,12,8,4,10&cfgs=7278&ver=3&def_sb=3,12,4,3,12&reel_set_size=7&def_sa=7,13,12,4,8&scatters=1~0,0,2,0,0~0,0,0,0,0~1,1,1,1,1;14~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"5266088\",max_rnd_win:\"9000\"}}&wl_i=tbm~9000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;500,100,40,0,0;350,75,30,0,0;250,50,20,0,0;150,30,15,0,0;100,20,12,0,0;75,15,8,0,0;50,10,5,0,0;50,10,5,0,0;25,5,2,0,0;25,5,2,0,0;25,5,2,0,0;0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=4,10,6,5,9,6,4,7,1,12,1,11,12,11,8,10,13,11,7,4,3,3,3,10,4,1,12,6,4,11,12,9,7,10,7,3,10,3,13,7,11,13,7,3,9,12,11,8,4,4,4,7,11,6,12,13,6,9,4,13,8,9,6,4,8,6,10,9,3,5,9,11,12,1,10,5,5,5,12,8,5,9,4,9,8,13,5,5,12,8,13,9,1,3,13,5,13,8,1,13,6,11,10~5,9,10,12,2,9,8,3,4,6,8,10,13,9,4,9,11,13,6,10,3,3,3,12,6,5,12,9,9,3,4,7,3,6,10,13,7,5,9,4,3,6,12,10,12,4,4,4,7,4,11,9,9,11,13,10,3,11,12,5,7,2,8,12,11,8,7,12,8,5,5,5,4,13,10,7,2,10,13,5,11,7,10,6,8,13,11,13,7,6,8,6,2,11~12,8,13,8,13,11,4,13,6,1,6,2,8,12,11,8,9,11,9,11,7,6,12,3,1,13,10,7,5,10,4,11,9,8,13,5,5,5,8,12,4,2,1,10,9,11,8,12,11,10,12,11,5,10,6,9,13,12,5,4,6,10,2,5,5,10,6,7,4,9,13,12,4,4,4,6,5,9,11,3,1,12,11,8,7,13,10,9,13,10,5,6,7,11,13,6,4,2,5,5,7,10,7,10,12,10,7,13,3,3,12,3,3,3,7,3,2,5,6,5,9,11,8,7,10,7,3,13,7,1,6,13,9,6,11,1,11,8,12,3,10,8,4,13,9,1,13,8,2,6,4,9,11,9~7,12,9,10,9,10,12,9,2,11,6,11,10,7,10,9,7,3,6,6,2,8,3,3,3,12,5,7,9,4,13,5,3,8,6,6,12,13,2,11,5,8,10,11,10,7,13,9,8,4,4,4,10,5,13,12,9,10,11,13,12,13,8,2,5,6,12,9,12,3,4,10,12,7,5,5,5,12,10,7,8,4,6,7,3,13,3,11,9,8,13,9,6,13,4,9,13,8,11,6,6,6,10,4,5,4,2,13,2,13,11,12,11,4,7,8,10,5,13,12,11,8,11,3,3,6~13,9,5,13,10,4,6,13,11,3,6,12,8,4,7,9,13,3,3,3,13,8,7,12,6,11,12,1,11,1,3,7,10,9,13,5,10,12,9,10,6,4,4,4,13,1,12,11,6,8,6,11,12,6,10,7,4,9,3,7,11,9,7,8,1,5,5,5,4,8,6,1,5,13,4,1,7,12,3,8,10,5,13,10,9,5,9,5,11,4&accInit=[{id:0,mask:\"cp;tp;lvl;sc;cl\"}]&reel_set2=8,7,11,9,13,7,5,10,8,4,3,9,12,13,10,13,12,10,12,11,13,11,8,12,7,12,10,9,6,10,7,13,6,8,10,11,7,8,9,13,9,4,6,13,7,5,6,8,5,11,12,4,12,9,3,9,10~13,12,5,11,2,8,7,5,7,12,8,10,5,11,9,13,4,10,7,3,4,9,13,5,6,11,6,10,13,10,4,10,6,8,7,11,13,7,9,11,9,12,7,11,6,12,9,13,9,8,7,12,6,13,12,10,3,12,13,3,6,9,5,12,13,8,11~11,7,3,13,7,9,11,6,11,10,5,7,8,13,2,12,9,10,8,13,11,10,6,4,13,6,9,10,8,4,12,11,13,12,7,6,9,13,12,8,11,3,5,9,10,5,4,12,8~6,7,4,9,10,5,2,13,12,9,10,5,13,7,3,4,5,11,12,11,9,5,2,13,11,4,11,10,3,9,12,8,6,9,8,9,7,9,11,13,11,6,13,10,8,6,11,9,8,12,10,8,7,12,7~13,7,13,11,13,12,6,9,5,6,5,7,9,12,9,6,3,8,11,3,5,10,4,11,9,11,7,13,4,7,8,6,10,8,11,10,9,12,8,10,12,8,6,12,3,4,7&reel_set1=13,11,8,13,6,8,10,8,13,12,7,4,12,5,12,4,10,12,11,6,11,7,3,14,11,12,11,13,10,14,6,9,12,10,9,10,13,3,6,9,5,14,9,7,3,11,9,3,4,9,5,8,7,4,5,11,8,13,10,14,7,12,10,9,6,4,10,9,6,11,12,7,13,7,12,13,9,8,5,10,13,6,13~6,5,11,4,9,6,9,2,10,8,12,10,11,12,11,6,7,11,14,2,3,12,5,13,12,7,9,3,10,13,4,12,8,3,5,12,7,6,8,14,9,4,13,12,5,8,4,2,10,6,10,12,13,9,10,11,8,9,13,10,2~9,7,8,10,14,13,7,6,13,7,3,2,6,5,8,6,2,13,7,9,3,6,8,12,7,12,13,12,9,12,14,8,6,7,9,13,5,9,6,2,11,8,2,12,9,10,13,12,13,10,13,4,11,7,11,14,5,11,3,4,10,12,10,4,13,11,4,11,5,10~7,11,13,5,11,9,7,5,4,10,11,12,10,6,3,10,4,9,8,12,10,9,12,3,8,12,11,10,5,13,8,10,13,7,14,11,3,11,13,2,8,12,9,14,13,4,3,12,10,12,10,13,12,7,6,2,6,9,10,13,12,9,13,9,13,8,10,8,11,10,6,7,2,9,2,13,7,9,11,8,5,11~5,9,7,4,11,12,9,6,7,13,4,13,10,13,5,6,9,6,11,13,10,9,8,5,6,12,13,8,12,7,6,9,10,3,9,7,11,10,14,11,8,3,4,10,12,5,12,7,8,14,13,12,5,12,8,11,13&reel_set4=5,6,10,1,9,13,1,12,1,10,11,13,8,9,1,8,9,1,5,1,11,10,3,1,7,12,1,10,12,6,4,1,8,3,11,1,9,1,4,11,1,13,7,6,9,1,5,1,10,12,10,11,13,1,8,1,4,6,13,1,13,1,3,1,12,9,12,1,12,1,10~13,8,2,9,13,10,13,9,10,12,13,2,3,5,6,4,11,9,3,9,10,9,5,12,8,7,8,11,7,13,6,4,3,2,12,2,11,9,12,7,11,9,9,6,9,11,9,12,10,9,9,12,8,10,9,5,9,13,12,4,9,12~3,11,1,8,2,1,6,5,10,1,3,1,13,7,12,1,6,8,13,4,7,2,11,13,12,1,5,11,1,7,1,13,8,1,13,9,10,1,5,5,13,6,1,13,1,8,10,7,1,8,10,11,1,10,11~2,10,2,11,10,11,11,12,5,6,4,7,11,11,12,13,2,5,9,10,10,12,12,10,10,9,13,6,8,10,3,12,8,10,9,6,10,3,10,10,12,13,12,11,9,8,7,11,2~10,1,11,1,12,1,7,5,9,1,5,1,13,1,10,1,8,9,1,6,11,4,8,1,9,1,10,8,1,10,8,12,4,13,11,5,12,3,1,6,7,9,11,3,12,10,1,13,1,6,1,7,1,12,8,9,11&purInit=[{bet:2000,type:\"default\"}]&reel_set3=11,10,8,13,9,12,11,8,13,7,10,6,9,5,4,3,6,7,12,10,13,11,7,3,3,3,6,13,9,7,9,12,6,10,11,3,12,13,8,11,5,11,9,3,4,12,7,3,10~7,3,3,13,9,10,8,3,7,11,12,3,9,6,13,8,9,6,5,7,11,3,3,3,11,13,10,9,5,8,6,8,10,4,9,12,9,12,10,4,12,10,5,12,11,6,13~13,6,13,8,12,6,4,12,5,9,7,5,11,8,11,6,9,3,12,10,11,13,11,11,13,3,9,6,7,12,10,9,4,10,13,10,3,3,3,13,9,11,9,10,11,6,8,3,7,8,13,9,5,9,12,11,11,8,6,12,10,7,8,3,13,4,3,9,7,10,6,8,10,12,4,9,7,5,4~11,7,9,13,3,12,8,5,8,12,5,12,6,3,11,8,13,9,11,7,9,11,3,9,10,3,3,3,7,13,6,9,5,10,6,7,10,11,4,8,4,6,8,9,12,9,10,3,9,13,5,13,7,9,10,12~8,6,9,7,6,12,5,8,3,13,10,13,9,10,8,9,5,9,7,12,3,3,3,13,7,11,4,3,6,9,6,13,7,10,11,12,3,11,12,5,10,4,3,8,13&reel_set6=7,5,11,9,10,13,8,11,12,13,10,11,9,4,6,11,9,8,11,3,9,13,12,13,12,8,6,13,7,10,12,13,11,3,12,9,7,8,6,13,12,11,9,12,7,6,10,12,13,4,8,7,10,5,6,7,3,8,11,9,11,12,10,4,8,13,9,10,6,5,7,5,7~9,10,8,4,12,13,8,2,11,3,10,11,9,11,9,10,11,9,5,7,13,4,9,12,10,12,13,2,4,7,6,2,13,8,6,13,11,5,6,12,3,7,3,10,11,5,7,8,12,6,10,2,12,13~6,2,5,11,12,9,6,13,7,8,2,8,9,7,13,5,6,2,12,8,5,9,7,11,3,12,11,13,11,8,12,10,4,3,10,13,2,11,10,2,4,10,4,10,6,9,8,6,13,9~7,4,10,3,13,7,9,12,2,12,7,5,4,10,7,8,10,9,12,9,13,9,5,6,13,8,9,10,12,10,11,6,11,9,11,2,7,12,6,13,6,5,9,2,11,9,4,11,10,8,3,5,10,12,2,13,8,13,11,2~3,8,13,9,11,7,4,10,13,6,7,6,8,11,9,12,4,6,10,5,10,6,5,6,12,13,3,9,11,7,12,4,12,11,8,10,12,9,3,7,13,11,5,8,9,9&reel_set5=12,8,13,14,11,4,7,3,7,14,13,3,12,10,12,6,13,8,7,4,11,12,8,11,5,8,9,11,13,12,8,6,14,9,6,9,10,13,10,5,11,7,5,12,11,7,12,13,11,9,10,9,4,3,13,5,10,11,4,14,10,7,9,7,13,11,9,10,13,7,6,3,10~9,8,9,13,12,6,5,10,9,4,10,12,13,4,13,3,12,11,3,8,9,6,12,7,10,7,3,10,5,9,7,8,13,12,2,11,12,6,14,4,3,11,5,14,11,5,6,7,10,9,2,10,13,10,4,8,11,13,8,2,12,11~7,12,6,5,2,13,3,14,12,13,3,14,7,13,10,11,8,9,11,6,13,6,12,10,5,8,10,7,10,2,3,14,5,9,11,2,12,4,11,12,4,8,9,11,10,7,13,9,11,13~13,4,9,12,11,13,10,14,10,8,13,11,10,13,12,9,8,11,5,8,6,8,11,7,10,7,9,4,9,11,12,14,2,9,12,5,3,10,12,11,2,5,7,11,13,10,11,6,13,6,12,5,8,3,8,3,10,2,13,9,7,13,6,9,10~3,5,7,8,10,4,6,12,10,13,8,9,4,5,6,12,9,3,13,11,10,4,13,12,7,5,9,12,13,12,9,11,10,14,11,7,14,7,5,11,10,8,9,7,3,8,6,10,9,8,11,9,13,8,14,4,9,13,6,11,6,10,12,5,6,12,8,11,3,13,11,12,6,12,10,9,5,13,7&total_bet_min=10.00";
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
        public TheDogHouseMultiholdGameLogic()
        {
            _gameID = GAMEID.TheDogHouseMultihold;
            GameName = "TheDogHouseMultihold";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["g"] = "{screen1:{def_s:\"4,13,7,7,10,3,11,4,5,13,6,12,8,4,10\",def_sa:\"7,13,12,4,8\",def_sb:\"3,12,4,3,12\",reel_set:\"1\",sh:\"3\",st:\"rect\",sw:\"5\"},screen2:{def_s:\"4,13,7,7,10,3,11,4,5,13,6,12,8,4,10\",def_sa:\"7,13,12,4,8\",def_sb:\"3,12,4,3,12\",reel_set:\"1\",sh:\"3\",st:\"rect\",sw:\"5\"},screen3:{def_s:\"4,13,7,7,10,3,11,4,5,13,6,12,8,4,10\",def_sa:\"7,13,12,4,8\",def_sb:\"3,12,4,3,12\",reel_set:\"1\",sh:\"3\",st:\"rect\",sw:\"5\"},screen4:{def_s:\"4,13,7,7,10,3,11,4,5,13,6,12,8,4,10\",def_sa:\"7,13,12,4,8\",def_sb:\"3,12,4,3,12\",reel_set:\"2\",sh:\"3\",st:\"rect\",sw:\"5\"}}";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if(dicParams.ContainsKey("g"))
            {
                var gParam = JToken.Parse(dicParams["g"]);
                for(int i = 1; i <= 4; i++)
                {
                    string strParam = string.Format("screen{0}", i);
                    if (gParam[strParam] != null)
                    {
                        int winLineID = 0;
                        do
                        {
                            string strKey = string.Format("l{0}", winLineID);
                            if (gParam[strParam][strKey] == null)
                                break;

                            string[] strParts = gParam[strParam][strKey].ToString().Split(new string[] { "~" }, StringSplitOptions.None);
                            if (strParts.Length >= 2)
                            {
                                strParts[1] = convertWinByBet(strParts[1], currentBet);
                                gParam[strParam][strKey] = string.Join("~", strParts);
                            }
                            winLineID++;
                        } while (true);
                    }
                }
                dicParams["g"] = serializeJsonSpecial(gParam);
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in TheDogHouseMultiholdGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in TheDogHouseMultiholdGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override void supplementInitResult(Dictionary<string, string> dicParams, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {
            base.supplementInitResult(dicParams, betInfo, spinResult);
            var defaultGParam = JToken.Parse("{screen1:{def_s:\"4,13,7,7,10,3,11,4,5,13,6,12,8,4,10\",def_sa:\"7,13,12,4,8\",def_sb:\"3,12,4,3,12\",reel_set:\"1\",sh:\"3\",st:\"rect\",sw:\"5\"},screen2:{def_s:\"4,13,7,7,10,3,11,4,5,13,6,12,8,4,10\",def_sa:\"7,13,12,4,8\",def_sb:\"3,12,4,3,12\",reel_set:\"1\",sh:\"3\",st:\"rect\",sw:\"5\"},screen3:{def_s:\"4,13,7,7,10,3,11,4,5,13,6,12,8,4,10\",def_sa:\"7,13,12,4,8\",def_sb:\"3,12,4,3,12\",reel_set:\"1\",sh:\"3\",st:\"rect\",sw:\"5\"},screen4:{def_s:\"4,13,7,7,10,3,11,4,5,13,6,12,8,4,10\",def_sa:\"7,13,12,4,8\",def_sb:\"3,12,4,3,12\",reel_set:\"2\",sh:\"3\",st:\"rect\",sw:\"5\"}}");
            if (dicParams.ContainsKey("g"))
            {
                var gParam = JToken.Parse(dicParams["g"]);
                for(int i = 1; i <= 4; i++)
                {
                    string strParam = string.Format("screen{0}", i);
                    if (gParam[strParam] == null)
                        gParam[strParam] = defaultGParam[strParam];
                }
                dicParams["g"] = serializeJsonSpecial(gParam);
            }
            else
            {
                dicParams["g"] = serializeJsonSpecial(defaultGParam);
            }
        }
    }
}
