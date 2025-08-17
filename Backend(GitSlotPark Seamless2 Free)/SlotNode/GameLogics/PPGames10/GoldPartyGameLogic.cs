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
    class GoldPartyGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25goldparty";
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
                return "def_s=3,4,5,6,7,8,9,10,3,4,5,6,7,8,9&screenOrchInit={type:\"mini_slots\"}&cfgs=4969&ver=2&def_sb=4,5,6,7,8&reel_set_size=2&def_sa=4,5,6,7,8&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"21739130\",jp1:\"125000\",max_rnd_win:\"5163\",jp3:\"1250\",jp2:\"5000\",jp4:\"500\"}}&wl_i=tbm~5163&sc=8.00,16.00,24.00,32.00,40.00,80.00,120.00,160.00,200.00,300.00,400.00,600.00,1000.00,2000.00,3000.00,4000.00&defc=40.00&purInit_e=1&wilds=2~0,0,0,0,0,0,0~1,1,1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;100,40,20,5,0;50,25,15,0,0;30,20,10,0,0;30,15,5,0,0;30,15,5,0,0;25,15,5,0,0;25,15,5,0,0;20,10,5,0,0;20,10,5,0,0;15,10,5,0,0;15,10,5,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=13,15,15,15,11,9,4,14,10,8,14,14,14,3,6,3,3,3,7,12,15,5,14,3,14,15,4,3,14,5,3,12,3,10,11,14,6,9,3,5,3,14,9,15,10,9,3,14,3,9,15,3,6,3,14,3,9,15,6,9,5,6,15,14,7,15,3,14,9,15,14,10,11,8,3,15,3,14,10,3,8,9,15,11,12,6,3,9,3,15,7,3,5,12,3,14,6,7,11,3,6,10,4,9,15,3,5,15,10,15,3,14,3,10,14,3~15,15,15,14,9,3,13,4,3,3,3,6,14,14,14,10,2,2,2,12,2,15,8,11,5,7,14,2,10,2,7,4,2,13,10,14,12,2,9,14,4,7,2,14,2,7,12,2,10,3,14,3,2,14,2,7,14,2,3,14,2,14,13,3,2,7,2,3,14,6,4,13,5,2,14,7,12,7,12,14,13,2,8,12,14,6,14,7,2,7,3,14,13,14,2~6,2,2,2,5,2,8,10,13,12,9,15,15,15,11,14,14,14,4,14,7,15,3,3,3,3,13,7,14,3,15,13,2,7,4,5,7,2,15,7,13,14,3,7,9,15,3,7,2,15,13,7,3,15,2,15,14,13,5,7,9,15,2,8,5,15,13,5,10,2,14,2,3,13,14,3,13,7,4,3,13,14,15,2,3,4,5,15,2,14,12,14,7,15,7,2,3,14,9,13,14,2,10,15,14,7,3,14,4,13,5,3,13,15,5,8,9,13,2,13,15,2,9,14,3,13,12,11,2,9,15,4,14~16,15,15,15,10,2,6,3,3,3,5,3,14,14,14,14,4,2,2,2,15,12,9,7,11,13,8,2,15,2,3,2,15,4,5,4,15,11,3,14,2,15,9,2,5,15,14,4,12,2,4,3,9~10,10,10,16,15,13,8,5,7,15,15,15,12,11,3,3,3,6,10,14,14,14,4,14,3,9,3,15,11,4,3,15,3,12,15,4,8,16,7,12,3,15,7,3,15,9,12,14,15,4,7,3,14,12,9,12,3,12,15,3,15,3,12,7,15,4,3,11,7,3,11,3,7,3,11,4,11,3,15,4,3,11,3,16,15,3,16,3,14,3,12,14,11,14,3,4,7,3,15,3,9,15,3,9,11,3,14&reel_set1=5,11,4,8,5,6,10,4,11,15,15,8,6,5,9,6,17,3,10,6,9,5,4,8,15,15,15,11,6,9,7,4,11,3,7,8,18,6,7,11,5,3,10,7,9,4,5,9,15,10,7,3,8&purInit=[{type:\"d\",bet:2500}]&total_bet_min=8.00";
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
        public GoldPartyGameLogic()
        {
            _gameID = GAMEID.GoldParty;
            GameName = "GoldParty";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["g"] = "{gp:{def_s:\"3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3\",reel_set:\"1\",sh:\"6\",st:\"rect\",sw:\"10\"}}";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("g"))
            {
                var gParam = JToken.Parse(dicParams["g"]);
                //Found Error(2023.09.15)
                if (gParam["gp"] != null && gParam["gp"]["apwa"] != null)
                {
                    string[] strParts = gParam["gp"]["apwa"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    for(int i = 0; i < strParts.Length; i++)
                        strParts[i] = convertWinByBet(strParts[i], currentBet);
                    gParam["gp"]["apwa"] = string.Join(",", strParts);
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in GoldPartyGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in GoldPartyGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
	
    }
}
