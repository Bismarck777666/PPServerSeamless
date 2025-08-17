using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class DragoJewelsOfFortuneBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get { return BetPerLine * 20.0f; }
        }
    }
    class DragoJewelsOfFortuneGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs1600drago";
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
            get { return 1600; }
        }
        protected override int ServerResLineCount
        {
            get { return 20; }
        }
        protected override int ROWS
        {
            get
            {
                return 5;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=3,9,10,8,6,3,9,10,8,6,3,9,10,8,6,3,9,10,8,6,3,20,20,20,6&cfgs=3088&nas=20&ver=2&reel_set_size=3&def_sb=10,7,9,11,8&def_sa=11,4,9,7,5&bonusInit=[{bgid:0,bgt:45,bg_i:\"13,16,17,15,18\",bg_i_mask:\"r,wd,ms,fs,m\"}]&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&gameInfo={}&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1&wilds=2~0,0,0,0,0~1,1,1,1,1;19~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;200,50,25,0,0;100,40,20,0,0;50,25,15,0,0;40,20,10,0,0;20,10,8,0,0;15,8,6,0,0;10,8,6,0,0;10,6,4,0,0;10,6,4,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=9,10,11,3,3,3,3,9,10,8,6,10,6,9,4,10,4,11,9,7,5,8,9,10,6,3,10~5,9,8,5,7,9,5,8,9,6,7,11,8,11,7,8,5,6,2,10,6,9,5,9,8,5,7,11,7,7,6,8,9,5,11,6,9,7,11,3,4,5,6,11,6,9,5,9,8,5,7,9,5,8,9,6,7,11,8,3,11,8,7,9,11,7,7,6,8,9,5,11,6,9,8,11,7,8,5,6,10,6,9,5,9,8,5,7,11,5,8,9,6,7,11,8,7,11,8,11,9,11,7,7,6,8,9,5,11,6,9,7,11,3,4,5,6,11,6,9~7,11,8,6,11,10,5,7,3,11,4,7,10,7,11,5,10,4,7,2,10,8,7,11,5,4,11,11,3,8,10,4,8,5,10,7,9,10,7,11,8,10,4,7,10,8~11,10,7,6,10,11,6,9,3,3,3,3,5,6,11,2,10,7,10,6,9,2,9,7,10,9,7,4,9,7,6,7,4,11,6,10,9,8,7,3,4,5,6,11,10~6,10,11,3,3,3,3,9,10,7,11,10,3,9,8,5,10,9,7,8,9,6,10,5,8,4,7,11,4,8,10,5,7,6,9,7,5,11,6,7,4,8,11,7,5,9,11,10&reel_set2=10,11,3,3,3,3,10,6,11,4,10,11,6,4,10,4,11,5,11,10,6,11,10~5,9,8,5,7,11,5,8,11,9,11,7,7,6,9,5,11,6,9,11,5,6,6,9~7,8,10,7,8,4,7,4,8,3,10,9,7,10,7,8,10,4,7,10,8~11,10,7,6,10,11,5,4,9,7,6,7,4,11,6,10,2,8,7,3,4,5,6,11,2,10,7,10,6,9,10,9,7,4,6~6,10,9,3,3,3,3,6,10,9,9,8,5,10,9,7,8,9,6,10,5,8,4,7,10,4,8,10,5,7,6,9,7,5,10,6,7,4,8,11,7,5,9,11,10&t=243&reel_set1=9,10,11,3,3,3,3,9,10,7,6,10,10,6,11,8,6,9,4,10,4,11,9,7,6,9,11,10,6,11,5~5,9,8,5,7,11,5,8,9,6,7,11,11,6,9,11,11,2,7,4,5,6,10,8,7,11,8,11,9,11,5,7,6,10,9,5,11,6,9,7,11,3,9,5,6,8,6,9,8,7,9,6,11~7,11,8,6,11,10,5,7,8,11,4,7,10,7,8,10,9,7,11,8,10,2,8,11,4,7,11,3,4,5,6,10,11,7,11,10,4,8,5,10,7,8,10,7,11,8,10,11,7,4,8,5,10~11,10,7,6,10,11,5,8,7,11,9,10,7,3,4,5,6,11,9,10,7,5,9,7,2,11,9,10,9,7,4,9,8,6,7,4,11,6,10,11,8,7,3,4,5,6,11,9,10,7,10,8,9~6,10,9,3,3,3,3,9,10,7,8,10,7,8,10,10,9,8,5,10,9,7,8,9,6,10,5,8,4,7,10,4,8,10,5,7,6,9,7,5,10,6,7,4,8,9,7,5,9,11,10&purInit=[{type:\"wbg\",bet:2000,game_ids:[0]}]&total_bet_min=10.00";
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
        public DragoJewelsOfFortuneGameLogic()
        {
            _gameID = GAMEID.DragoJewelsOfFortune;
            GameName = "DragoJewelsOfFortune";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("wlc_v"))
            {
                string[] strParts = dicParams["wlc_v"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strParts.Length; i++)
                {
                    string[] strValues = strParts[i].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                    strValues[1] = convertWinByBet(strValues[1], currentBet);
                    strParts[i] = string.Join("~", strValues);
                }
                dicParams["wlc_v"] = string.Join(";", strParts);
            }
        }
	
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                DragoJewelsOfFortuneBetInfo betInfo = new DragoJewelsOfFortuneBetInfo();
                betInfo.BetPerLine                  = (float)message.Pop();
                betInfo.LineCount                   = (int)message.Pop();
		
		
                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;
		
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in DragoJewelsOfFortuneGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in DragoJewelsOfFortuneGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            DragoJewelsOfFortuneBetInfo betInfo = new DragoJewelsOfFortuneBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
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
    }
}
