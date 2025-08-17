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
    class MightyMunchingMelonsGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20mmmelon";
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
                return "def_s=4,8,8,5,10,5,7,7,4,7,4,10,6,5,9&cfgs=11375&ver=3&mo_s=12&mo_v=2,5,10,20,50,100,200,500,1000,2000,5000,10000&def_sb=5,10,6,5,8&reel_set_size=4&def_sa=4,7,7,6,9&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{ante:\"94.03\",purchase:\"94.03\",regular:\"94.03\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"712312\",max_rnd_win:\"3000\",max_rnd_win_a:\"2000\",max_rnd_hr_a:\"450910\"}}&wl_i=tbm~3000;tbm_a~2000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&bls=20,30&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;20000,4000,400,0,0;4000,1000,200,0,0;400,200,100,0,0;200,100,40,0,0;100,40,20,0,0;100,40,20,0,0;40,20,10,0,0;40,20,10,4,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=10,8,6,9,1,6,8,7,9,9,9,3,9,4,3,7,1,9,5,10,7,5~7,9,10,6,10,4,7,4,10,9,5,8,6,3,5,8,9,1,9,8,7,9,10,7,3,8,10~5,6,3,7,9,1,8,9,7,10,4,9,8,5,10,4,6,10~10,4,10,7,3,9,7,9,6,9,10,9,1,8,5,10~6,9,10,9,8,10,7,8,1,10,5,6,7,4,9,7,3,9,10,9,4&reel_set2=1,3,4,5,6,7,8,9,10~6,7,5,10,7,9,7,9,7,8,9,8,4,9,8,3~10,9,7,10,10,10,6,8,5,10~1,3,4,7,8,9,10~8,10,9,10,7,6,4,5,6,3,6,7,4&reel_set1=1,3,4,5,6,7,8,9,10~6,7,9,3,7,8,9,9,9,4,8,9,8,10,5,7,9~10,5,1,9,10,10,10,6,10,9,10,7,10,8~3,4,7,8,9,10~3,7,5,9,7,6,10,6,4,9,8,4&purInit=[{bet:2000,type:\"default\"}]&reel_set3=3,4,5,6,7,8,9,10~8,6,4,8,5,7,10,9,1,9,7,8,3~7,6,1,8,10,10,10,9,10,10,5,10,10~3,4,7,8,9,10~3,4,5,6,7,8,9,10&total_bet_min=10.00";
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
	
	
        protected override double MoreBetMultiple
        {
            get { return 1.5; }
        }
        protected override bool SupportMoreBet
        {
            get { return true; }
        }
	
        #endregion
        public MightyMunchingMelonsGameLogic()
        {
            _gameID = GAMEID.MightyMunchingMelons;
            GameName = "MightyMunchingMelons";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
	        dicParams["bl"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("pw"))
                dicParams["pw"] = convertWinByBet(dicParams["pw"], currentBet);

            if (dicParams.ContainsKey("mo_tw"))
                dicParams["mo_tw"] = convertWinByBet(dicParams["mo_tw"], currentBet);

            if (dicParams.ContainsKey("rs_win"))
            {
                string[] strParts = dicParams["rs_win"].Split(new string[] { "," }, StringSplitOptions.None);
                for(int i = 0; i < strParts.Length; i++)
                {
                    if (!string.IsNullOrEmpty(strParts[i]))
                        strParts[i] = convertWinByBet(strParts[i], currentBet);
                }
                dicParams["rs_win"] = string.Join(",", strParts);
            }
            if (dicParams.ContainsKey("rs_iw"))
            {
                string[] strParts = dicParams["rs_iw"].Split(new string[] { "," }, StringSplitOptions.None);
                for (int i = 0; i < strParts.Length; i++)
                {
                    if (!string.IsNullOrEmpty(strParts[i]))
                        strParts[i] = convertWinByBet(strParts[i], currentBet);
                }
                dicParams["rs_iw"] = string.Join(",", strParts);
            }
            if(dicParams.ContainsKey("g"))
            {
                var gParam = JToken.Parse(dicParams["g"]);
                foreach (var pair in gParam as JObject)
                {
                    if(pair.Key.StartsWith("mp") && pair.Value["mo_tw"] != null)
                        pair.Value["mo_tw"] = convertWinByBet(pair.Value["mo_tw"].ToString(), currentBet);
                }
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in MightyMunchingMelonsGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in MightyMunchingMelonsGameLogic::readBetInfoFromMessage", strUserID);
                    return;
                }

                betInfo.LineCount = ClientReqLineCount;
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
                _logger.Error("Exception has been occurred in MightyMunchingMelonsGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override void supplementInitResult(Dictionary<string, string> dicParams, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {
            base.supplementInitResult(dicParams, betInfo, spinResult);
            if (!dicParams.ContainsKey("reel_set"))
                dicParams["reel_set"] = "0";
        }
    }
}
