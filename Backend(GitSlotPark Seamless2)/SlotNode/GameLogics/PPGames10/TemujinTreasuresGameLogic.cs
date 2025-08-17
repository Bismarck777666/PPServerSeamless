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
    class TemujinTreasuresBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get { return BetPerLine * 38.0f; }
        }
    }
    class TemujinTreasuresGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs1024temuj";
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
            get { return 20; }
        }
        protected override int ServerResLineCount
        {
            get { return 38; }
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
                return "def_s=3,4,11,9,10,9,6,5,6,7,9,10,8,10,8,6,7,8,3,7&cfgs=3980&ver=2&mo_s=14&mo_v=100,200,250,500,1000,2000,5000,10944,3344,1064&def_sb=5,3,4,6,7&reel_set_size=2&def_sa=11,11,10,8,9&mo_jp=10944;3344;1064&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"8665511\",jwt_jp:\"jp1,jp2,jp3,jp4\",max_rnd_win:\"9000\",ma_jp:\"337744,10944,3344,1064\"}}&wl_i=tbm~9000&mo_jp_mask=jp2;jp3;jp4&sc=5.00,10.00,15.00,20.00,25.00,50.00,75.00,100.00,125.00,200.00,250.00,375.00,700.00,1250.00,2000.00,2750.00&defc=25.00&purInit_e=1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;100,40,20,0,0;25,15,10,0,0;15,10,5,0,0;10,5,3,0,0;10,5,3,0,0;5,3,2,0,0;5,3,2,0,0;5,3,2,0,0;5,3,2,0,0;5,3,2,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=10,450,000.00&reel_set0=8,8,8,6,10,10,10,10,9,3,13,5,4,7,7,7,8,12,12,12,12,11,9,9,9,7,11,11,11,5,5,5,6,6,6,13,13,13,12,7,9,5,10,11,7,11,10,7,6,10,9,10,7,10,11,6~9,7,12,12,12,1,3,2,5,10,10,10,13,10,6,9,9,9,8,11,12,4,7,7,7,13,13,13,2,2,2,3,3,3,5,13,2,3,12,5,3,5,3,10,7,10,3,2,3~10,1,7,12,5,5,5,2,2,2,2,6,5,9,8,13,13,13,11,8,8,8,3,13,11,11,11,4,4,4,4,7,7,7,4,3,11,2,4,2,6,2,11,9,5,11,7,4,8,1,8,11,7,12,8,11,7,5,13,4,13,6~13,13,13,2,12,12,12,7,4,9,9,9,10,11,4,4,4,8,6,12,2,2,2,1,5,9,13,10,10,10,3,10,7,12,2,4,9,2,10,2,10,9,10,9,1,10,2,4,2,4,10,2,10,9,3,9,10,2,4,10,9,10,9,4,12,2,10,12,2,10,9,5,2,1,2,9,12,10,1,4,2,10,3,10,2,12,10,8,10,3~3,13,13,13,7,11,10,10,10,5,11,11,11,4,13,8,6,9,10,12,4,6,12,13,10,13,9,7,13,4&t=243&reel_set1=7,10,10,10,11,9,5,4,10,6,11,11,11,12,13,3,3,3,3,13,13,13,8,7,7,7,5,5,5,3,10~12,2,2,2,3,4,13,5,5,5,5,11,6,4,4,4,10,13,13,13,7,8,8,8,14,9,2,8,12,12,12,4,7,5,4,10,6,4,9,14,4,2,9,11,13,4,8,9,7,9,7,11,9,4,9,2,4,8,2,9,4,7,4,7,9,6,7,4,11~4,11,7,7,7,14,2,12,9,9,9,3,2,2,2,8,6,5,10,9,7,3,3,3,13,11,11,11,13,13,13,12,12,12,2,7,3,7,10,14,2,6,3,7,9,11,9,11,2,3,11,6,2,13~8,5,7,2,2,2,11,10,10,10,9,12,11,11,11,4,13,13,13,10,3,13,6,14,2,13,10,11,2,11,5,11,5,13,2,13,10,5,2,10,4,9,10,9,13,2,7,6,3,5,7,5,11,5,3,4,5,10,2~12,6,6,6,10,7,13,6,9,9,9,8,5,3,9,7,7,7,4,12,12,12,11,8,8,8,11,11,11,13,13,13,10,10,10,13,8,13,11,13,9,13,11&purInit=[{type:\"d\",bet:3800}]&total_bet_min=5.00";
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
        public TemujinTreasuresGameLogic()
        {
            _gameID = GAMEID.TemujinTreasures;
            GameName = "TemujinTreasures";
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
            if (dicParams.ContainsKey("mo_tw"))
                dicParams["mo_tw"] = convertWinByBet(dicParams["mo_tw"], currentBet);
        }

        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            TemujinTreasuresBetInfo betInfo = new TemujinTreasuresBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                TemujinTreasuresBetInfo betInfo = new TemujinTreasuresBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();
		
		
                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;
		
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in TemujinTreasuresGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in TemujinTreasuresGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul","fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "bl","reel_set",
                "s", "purtr", "w", "tw", "g" };
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
