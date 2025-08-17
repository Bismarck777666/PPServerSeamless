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
    class PiggyBankersGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20piggybank";
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
                return "def_s=4,9,6,12,9,4,11,5,4,10,3,8,4,6,11,6,10,9,4,9&cfgs=8274&ver=3&mo_s=15&mo_v=10,20,40,100,200,400,1000,2000,20000&def_sb=3,11,12,2,8&reel_set_size=20&def_sa=4,10,3,2,7&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"12000000\",max_rnd_win:\"10000\"}}&wl_i=tbm~10000&reel_set10=14,15,14,15,15,14,14,15,14,14,15,14,15,14,14,15,14,15,14,14,15,14,15,14,14,15,15,15,14,14,14,15,14,15,14,15,14,15,14,14,15,14,15,14,14,15,14,15,14,14,15,14,15,14,15,15,14,15,14,14&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&reel_set11=14,15,14,14,15,14,14,15,14,16,14,14,15,14,14,15,14,15,14,16,14,14,15,14,14,15,15,15,14,14,15,14,14,15,14,16,14,14,15,14,14,15,14,15,14,14&reel_set12=14,15,14,14,15,14,14,15,14,14,14,14,15,14,14,15,14,15,14,14,14,14,15,14,14,15,15,15,14,14&purInit_e=1&reel_set13=14,15,14,14,15,14,14,15,14,14,14,14,15,14,14,15,14,15,14,14,15,14,15,14,14,15,14,15,14,14&wilds=2~250,100,50,0,0~1,1,1,1,1;3~250,100,50,0,0~1,1,1,1,1;13~250,100,50,0,0~1,1,1,1,1&bonuses=0&reel_set18=14,15,14,14,15,14,14,15,14,14,14,14,15,14,14,15,14,15,14,14,14,14,15,14,14,15,15,15,14,14&reel_set19=14,15,14,15,15,14,15,15,15,14,15,15,15,15,14,15,15,15,15,15,15,14,15,14,15,15,15,15,14,14&reel_set14=14,15,14,15,15,14,14,15,14,14,15,14,15,14,14,15,14,15,14,14,15,14,15,14,14,15,15,15,14,14&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;250,100,50,0,0;150,50,25,0,0;100,40,20,0,0;80,30,15,0,0;60,20,10,0,0;60,20,10,0,0;40,10,5,0,0;40,10,5,0,0;40,10,5,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&reel_set15=14,15,14,15,15,14,14,15,14,14,15,14,15,14,14,15,14,15,14,14,15,14,15,14,14,15,15,15,14,14,14,15,14,15,14,15,14,15,14,14,15,14,15,14,14,15,14,15,14,14,15,14,15,14,15,15,14,15,14,14&reel_set16=14,15,14,15,15,14,14,15,14,14,15,14,15,14,14,15,14,15,14,14,15,14,15,14,14,15,15,15,14,14&reel_set17=14,15,14,14,15,14,14,15,14,14,15,14,15,14,14,15,14,15,14,14,15,14,15,14,14,14,15,15,15,14,14,15,14,14,15,15,14,15,14,15,14,15,14,15,14,14,15,15,14,14,15,14,14,15,15,14,14,15,14,14,14,15,14,14,15,15,14,14,14,15,14,14,15,14,14,15,14,14,15,14,15,14,14,15,14,15,14,14,15,14,15,14,15,14,15,14,15,14,14,15,14,15,14,15,14,15,14,15,14,15,14,15,14,14,15,15,14,14,15,14,14,15,15,14,14,15,14,14,14,15,14,14,15,15,14,14&total_bet_max=8,000,000.00&reel_set0=11,11,11,12,12,12,3,3,3,3,8,8,8,8,4,10,10,10,5,6~11,11,11,12,12,12,3,3,3,3,8,8,8,8,4,10,10,10,5,6~11,12,2,2,2,2,3,4,3,3,3,3,5,6,7,8~11,11,11,12,12,12,2,2,2,2,8,8,8,8,4,10,10,10,5,6~11,11,11,12,12,12,2,2,2,2,8,8,8,8,4,10,10,10,5,6&reel_set2=11,12,4,5,6,7,8,9,10~11,12,3,4,3,3,3,3,5,6,7,8,9,10~11,12,4,5,6,7,8,9,10~11,12,4,5,6,7,8,9,10~11,12,4,5,6,7,8,9,10&reel_set1=11,11,11,12,12,12,8,8,8,4,10,10,10,5,6~11,11,11,12,12,12,8,8,8,4,10,10,10,5,6~11,11,11,12,12,12,8,8,8,4,10,10,10,5,6~11,11,11,12,12,12,8,8,8,4,10,10,10,5,6~11,11,11,12,12,12,8,8,8,4,10,10,10,5,6&reel_set4=11,12,5,6,7,8,9,10~3,3,3,3,3~11,12,4,5,6,7,8,9,10~2,2,2,2,2~11,12,5,6,7,8,9,10&purInit=[{bet:1600,type:\"default\"}]&reel_set3=11,11,11,12,12,12,8,8,8,8,4,10,10,10,5,6~11,11,11,12,12,12,8,8,8,8,4,10,10,10,5,6~11,12,4,5,6,7,8,9,10~11,11,11,12,12,12,2,2,2,2,8,8,8,8,4,10,10,10,5,6~11,11,11,12,12,12,8,8,8,8,4,10,10,10,5,6&reel_set6=11,12,4,5,6,7,8,9,10~3,3,3,3,11,3~11,12,4,5,6,7,8,9,10~2,2,2,2,11,2~11,12,4,5,6,7,8,9,10&reel_set5=11,12,2,4,2,2,2,2,5,6,7,8,9,10~11,12,2,4,2,2,2,2,5,6,7,8,9,10~11,12,2,2,2,2,3,4,3,3,3,3,5,6,7,8~11,12,3,4,3,3,3,3,5,6,7,8,9,10~11,12,3,4,3,3,3,3,5,6,7,8,9,10&reel_set8=11,11,11,12,12,12,8,8,8,4,10,10,10,5,6~11,11,11,12,12,12,8,8,8,4,10,10,10,5,6~11,11,11,12,12,12,8,8,8,4,10,10,10,5,6~11,11,11,12,12,12,8,8,8,4,10,10,10,5,6~11,11,11,12,12,12,8,8,8,4,10,10,10,5,6&reel_set7=11,2,2,2,2,12,2,3,3,3,3,4,8,8,8,5,6,7~11,2,2,2,2,12,2,3,3,3,3,4,8,8,8,5,6,7~11,12,2,2,2,2,3,4,3,3,3,3,5,6,7,8~11,2,2,2,2,12,2,3,3,3,3,4,8,8,8,5,6,7~11,2,2,2,2,12,2,3,3,3,3,4,8,8,8,5,6,7&reel_set9=11,2,2,2,2,12,2,3,3,3,3,4,8,8,8,5,6,7~11,2,2,2,2,12,2,3,3,3,3,4,8,8,8,5,6,7~11,12,2,2,2,2,3,4,3,3,3,3,5,6,7,8~11,2,2,2,2,12,2,3,3,3,3,4,8,8,8,5,6,7~11,2,2,2,2,12,2,3,3,3,3,4,8,8,8,5,6,7&total_bet_min=10.00";
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
        public PiggyBankersGameLogic()
        {
            _gameID = GAMEID.PiggyBankers;
            GameName = "PiggyBankers";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["g"] = "{top:{def_s:\"14,14,15,14,14\",def_sa:\"14\",def_sb:\"15\",reel_set:\"10\",s:\"14,14,15,14,14\",sa:\"14\",sb:\"15\",sh:\"5\",st:\"rect\",sw:\"1\"}}";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if(dicParams.ContainsKey("g"))
            {
                var gParam = JToken.Parse(dicParams["g"]);
                for(int i = 0; i < 5; i++)
                {
                    string strParam = string.Format("reel_{0}", i);
                    if (gParam[strParam] != null && gParam[strParam]["mo_tw"] != null)
                        gParam[strParam]["mo_tw"] =  convertWinByBet(gParam[strParam]["mo_tw"].ToString(), currentBet);
                }
                if (gParam["top"] != null && gParam["top"]["mo_tw"] != null)
                    gParam["top"]["mo_tw"] = convertWinByBet(gParam["top"]["mo_tw"].ToString(), currentBet);

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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in PiggyBankersGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in PiggyBankersGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
	
    }
}
