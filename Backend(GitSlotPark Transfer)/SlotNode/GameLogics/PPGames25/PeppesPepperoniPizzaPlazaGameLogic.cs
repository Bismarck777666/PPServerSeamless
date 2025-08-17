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
    class PeppesPepperoniPizzaPlazaGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vswaystonypzz";
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
                return 18;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,4,8,15,15,6,4,8,7,3,5,6,9,7,4,5,6,9,1,4,7,1,5,8,4,15,11,5,8,4,15,11,5,5,4,15,15,5,5,4,15,15,10,5,7,15,15,15,5,7,15,15,15,5,1,15,15,15,15,8,15,15,15,15,8,15,15,15,15,15&cfgs=1&ver=3&def_sb=12,8,7,4,9&reel_set_size=5&def_sa=12,12,12,10,12&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{ante:\"96.55\",purchase:\"96.55\",regular:\"96.55\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"591104\",max_rnd_win:\"6000\",max_rnd_win_a:\"4000\",max_rnd_hr_a:\"357132\"}}&wl_i=tbm~6000;tbm_a~4000&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&purInit_e=1&wilds=2~0,0,0,0,0~1,1,1,1,1;13~0,0,0,0,0~1,1,1,1,1;14~0,0,0,0,0~1,1,1,1,1&bonuses=0&bls=10,15&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;30,10,6,2,0;20,8,4,1,0;15,6,3,0,0;15,6,3,0,0;15,6,3,0,0;10,4,2,0,0;10,4,2,0,0;5,2,1,0,0;5,2,1,0,0;5,2,1,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0&total_bet_max=12,000,000.00&reel_set0=6,9,7,9,12,10,4,9,7,7,8,5,10,8,10,6,5,3,10,3,12,12,6,11,8,11,5,7,11,11,8,4,3,9,5,7,12,4,6,5,8,11,12,10,6,3,9,3,4,1,4~11,7,8,7,9,3,11,5,8,10,4,3,3,3,7,6,12,9,1,6,6,12,12,1,8,11,6,4,4,4,12,11,4,6,3,6,7,9,7,1,5,3,5,5,5,9,7,5,3,6,4,12,8,5,9,11,9,8,6,6,6,7,5,6,7,12,10,7,4,7,8,5,4,7,7,7,7,3,3,11,6,5,12,7,5,10,9,5,8,4,11~5,7,11,7,6,1,3,3,3,3,7,5,3,8,6,4,12,8,4,4,4,4,3,10,5,9,12,11,10,9,5,5,5,5,3,11,5,9,4,6,4,6,6,6,6,5,8,11,8,12,12,9,7,7,7,7,7,6,5,4,7,3,7,7,6,7~7,9,1,3,11,8,11,3,3,3,3,3,7,3,11,1,3,12,7,7,5,4,4,4,4,4,8,6,4,7,7,3,5,12,5,5,5,5,5,9,4,12,7,3,5,4,6,1,6,6,6,6,6,4,11,6,4,7,5,10,6,12,7,7,7,7,7,7,9,6,10,5,5,6,6,9,8,8~5,9,4,12,4,3,3,3,3,3,3,5,7,3,6,3,4,4,4,4,4,4,11,5,11,7,12,7,5,5,5,5,5,5,9,8,7,7,4,6,6,6,6,6,6,11,5,9,10,3,3,7,7,7,7,7,7,7,6,8,6,6,8,1,6,12&accInit=[{id:0,mask:\"cp;cl;mx\"}]&reel_set2=3,3,6,5,9,6,10,12,8,3,6,11,10,12,11,4,9,8,4,5,7,7,5,7,12,10,11,4,8,9~12,12,4,7,3,3,3,1,10,8,1,4,4,4,3,7,6,9,9,5,5,5,12,9,10,11,8,6,6,6,11,6,6,7,5,7,7,7,7,4,11,5,5,8,7~8,8,4,11,3,3,3,3,7,9,9,8,6,3,4,4,4,4,5,6,7,12,11,5,5,5,5,12,7,3,12,6,11,6,6,6,6,10,10,4,5,3,7,7,7,7,7,6,7,9,7,5,4,5~5,7,3,9,6,3,3,3,3,3,6,4,5,11,9,4,4,4,4,4,8,4,3,4,12,7,5,5,5,5,5,9,7,6,11,5,6,6,6,6,6,8,7,11,1,12,8,7,7,7,7,7,7,5,6,7,10,6,1,12~9,7,12,11,4,7,7,3,3,3,3,3,3,5,3,12,3,5,8,6,7,4,4,4,4,4,4,5,6,9,8,3,8,5,6,5,5,5,5,5,5,9,7,11,7,5,6,12,10,6,6,6,6,6,6,10,6,6,4,3,8,11,3,7,7,7,7,7,7,7,6,12,5,4,7,4,5,7,11,9&reel_set1=12,4,10,8,12,3,7,4,4,8,1,6,6,11,5,12,9,5,3,3,9,11,10,10,11,9,6,1,7,8,7,5,1~5,6,5,12,7,3,3,3,5,4,11,9,12,5,5,4,4,4,11,7,9,9,12,9,8,5,5,5,4,11,6,8,11,4,6,6,6,3,6,12,8,8,7,7,7,7,10,6,3,7,7,3,10~12,8,1,4,3,3,3,3,9,6,7,7,12,3,4,4,4,4,1,7,1,8,11,12,5,5,5,5,11,7,6,9,4,8,6,6,6,6,10,11,5,5,3,5,7,7,7,7,7,3,9,6,6,10,5,4~11,10,5,3,4,5,3,4,6,7,11,11,11,11,11,7,5,12,4,3,6,4,10,11,10,7,12,12,12,12,12,3,6,7,5,12,5,7,6,3,12,3,3,3,3,3,8,12,8,9,7,10,8,5,9,4,8,4,4,4,4,4,10,9,12,11,9,12,5,5,7,6,7,5,5,5,5,5,8,4,7,6,9,12,10,10,4,10,4,6,6,6,6,6,8,9,9,3,12,11,4,3,7,8,12,7,7,7,7,7,11,5,7,11,11,10,5,11,6,9,12,8,8,8,8,8,6,10,5,6,6,8,4,3,11,9,10,9,9,9,9,9,11,7,11,10,4,7,12,8,8,7,7,10,10,10,10,10,3,11,9,9,3,12,8,4,9,3,6,8,6~3,7,12,3,12,12,9,10,6,9,7,11,11,11,11,11,11,4,12,9,9,5,11,5,6,3,5,8,12,4,12,12,12,12,12,12,6,4,7,11,12,8,11,4,3,3,4,12,3,3,3,3,3,3,8,11,4,10,12,9,10,10,11,6,7,5,9,4,4,4,4,4,4,5,4,6,5,11,6,9,11,4,7,4,8,10,5,5,5,5,5,5,12,5,10,6,10,8,7,3,11,7,11,6,6,6,6,6,6,3,7,3,8,7,5,3,7,7,12,4,5,10,7,7,7,7,7,7,7,5,6,10,9,5,11,4,7,10,6,7,3,10,8,8,8,8,8,8,10,11,11,4,11,8,3,8,12,8,3,12,9,9,9,9,9,6,8,7,10,9,6,8,8,6,8,10,9,12,10,10,10,10,10,10,7,5,7,12,7,3,6,4,5,3,4,8,6,11&reel_set4=12,10,7,10,6,12,10,8,10,11,10,7,3,8,4,8,6,10,3,12,5,10,8,5,8,12,12,10,9,8,11,11,5,4,12,12,3,9,6,6,4,10,8,12,9,7,11,5,4,8,11,12,8,9~11,7,8,9,11,11,11,4,11,7,10,3,12,12,12,3,11,8,7,5,3,3,3,9,9,11,4,11,4,4,4,5,8,9,4,7,5,5,5,9,7,8,12,8,6,6,6,11,5,9,3,10,7,7,7,11,6,6,12,6,9,9,9,10,7,11,3,12,10,10,10,12,10,9,9,10,5~6,8,11,11,11,11,9,7,8,6,12,12,12,12,8,7,11,10,3,3,3,3,11,5,11,10,4,4,4,4,10,4,7,6,5,5,5,5,3,7,7,6,6,6,6,4,10,4,12,7,7,7,7,3,5,8,12,8,8,8,8,7,3,5,7,9,9,9,9,6,7,9,9,10,10,10,10,4,12,7,9,7~4,6,7,11,11,11,11,11,12,7,3,5,12,12,12,12,12,8,9,4,6,3,3,3,3,3,5,7,5,6,4,4,4,4,4,12,12,7,3,4,5,5,5,5,5,3,7,11,10,6,6,6,6,6,8,11,3,7,9,7,7,7,7,7,8,9,4,10,8,8,8,8,8,11,6,7,4,5,9,9,9,9,9,7,10,8,6,10,10,10,10,10,7,9,5,7,11,10~7,12,10,10,5,11,11,11,11,11,11,7,7,12,10,8,12,12,12,12,12,12,11,6,3,4,7,11,9,3,3,3,3,3,3,8,4,6,9,7,3,4,4,4,4,4,4,7,3,10,4,7,7,5,5,5,5,5,5,7,12,7,3,7,4,6,6,6,6,6,6,8,11,7,8,6,9,7,7,7,7,7,7,7,6,12,3,7,10,8,8,8,8,8,8,6,5,4,9,5,9,11,9,9,9,9,9,9,7,8,8,9,7,11,10,10,10,10,10,10,12,10,5,7,6,12,5,4&purInit=[{bet:1200,type:\"default\"}]&reel_set3=8,4,8,11,12,9,11,8,10,4,8,12,10,11,12,10,6,7,12,5,9,7,3,8,3,5,10,10,6,12~7,11,12,11,11,11,10,5,12,12,12,4,7,9,10,3,3,3,4,6,3,4,4,4,8,10,8,12,5,5,5,11,4,3,6,6,6,11,7,9,10,7,7,7,12,8,8,9,9,9,6,9,5,5,10,10,10,7,9,7,9,11~6,9,8,11,11,11,11,7,11,7,3,7,5,12,12,12,12,7,7,12,9,7,3,3,3,3,8,3,11,8,9,6,4,4,4,4,7,11,7,8,10,6,5,5,5,5,3,5,7,12,4,7,6,6,6,6,10,7,4,9,12,7,7,7,7,5,10,4,7,12,5,8,8,8,8,6,6,9,4,4,10,9,9,9,9,12,11,9,5,8,8,10,10,10,10,7,10,5,10,4,7,7~7,6,8,3,11,11,11,11,11,7,7,10,8,4,8,12,12,12,12,12,6,11,3,4,9,9,3,3,3,3,3,11,3,7,10,7,4,4,4,4,4,7,4,10,6,7,5,5,5,5,5,12,6,11,3,11,5,6,6,6,6,6,8,5,4,8,9,5,7,7,7,7,7,12,6,9,7,3,8,8,8,8,8,10,4,5,9,12,6,9,9,9,9,9,10,12,4,5,7,6,10,10,10,10,10,5,7,5,7,11,7,12~5,7,10,5,11,11,11,11,11,11,9,12,10,7,11,7,12,12,12,12,12,12,3,6,3,7,11,3,3,3,3,3,3,5,6,11,4,12,10,4,4,4,4,4,4,12,9,7,8,5,4,5,5,5,5,5,5,9,7,7,12,6,3,8,6,6,6,6,6,6,7,7,9,4,11,7,7,7,7,7,7,7,10,11,7,3,7,8,8,8,8,8,8,7,8,9,5,6,10,9,9,9,9,9,9,7,5,4,6,4,8,10,10,10,10,10,10,3,9,7,7,8,10,4,12&total_bet_min=200.00";
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
        protected override double MoreBetMultiple
        {
            get { return 1.5; }
        }
        protected override bool SupportMoreBet
        {
            get { return true; }
        }
	
        #endregion
        public PeppesPepperoniPizzaPlazaGameLogic()
        {
            _gameID     = GAMEID.PeppesPepperoniPizzaPlaza;
            GameName    = "PeppesPepperoniPizzaPlaza";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["g"] = "{aperture_s:{def_s:\"15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,16,16,15,15,16,16,16,16,16,5,6,9,7,4,5,6,9,1,4,16,1,5,8,4,15,16,5,8,4,15,16,16,5,4,15,15,16,16,4,15,15,16,16,16,15,15,15,16,16,15,15,15,16,16,15,15,15,15,16,15,15,15,15,16,15,15,15,15,15\",def_sa:\"15,15,15,15,15\",def_sb:\"15,15,15,15,15\",s:\"15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,16,16,15,15,16,16,16,16,16,5,6,9,7,4,5,6,9,1,4,16,1,5,8,4,15,16,5,8,4,15,16,16,5,4,15,15,16,16,4,15,15,16,16,16,15,15,15,16,16,15,15,15,16,16,15,15,15,15,16,15,15,15,15,16,15,15,15,15,15\",sa:\"15,15,15,15,15\",sb:\"15,15,15,15,15\",sh:\"18\",st:\"rect\",sw:\"5\"}}";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
	        dicParams["bl"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);

            if (dicParams.ContainsKey("wlc_v"))
            {
                string strWlcv = dicParams["wlc_v"];
                string[] strParts = strWlcv.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < strParts.Length; i++)
                {
                    string[] strSubParts = strParts[i].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                    if (strSubParts.Length > 1)
                        strSubParts[1] = convertWinByBet(strSubParts[1], currentBet);

                    strParts[i] = string.Join("~", strSubParts);
                }
                dicParams["wlc_v"] = string.Join(";", strParts);
            }

            if (dicParams.ContainsKey("g"))
            {
                var gParam = JToken.Parse(dicParams["g"]);
                if (gParam["ps"] != null && gParam["ps"]["mo_tw"] != null)
                    gParam["ps"]["mo_tw"] = convertWinByBet(gParam["ps"]["mo_tw"].ToString(), currentBet);

                dicParams["g"] = serializeJsonSpecial(gParam);
            }
        }
	
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                BasePPSlotBetInfo betInfo = new BasePPSlotBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();
		
                int bl = (int)message.Pop();
                if (bl == 0)
                    betInfo.MoreBet = false;
                else
                    betInfo.MoreBet = true;
		
		
                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;
		
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in vswaystonypzzGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in vswaystonypzzGameLogic::readBetInfoFromMessage", strUserID);
                    return;
                }
		
                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1}", strUserID, betInfo.LineCount);
                    return;
                }

                BasePPSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strUserID, out oldBetInfo))
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
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in vswaystonypzzGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
	
    }
}
