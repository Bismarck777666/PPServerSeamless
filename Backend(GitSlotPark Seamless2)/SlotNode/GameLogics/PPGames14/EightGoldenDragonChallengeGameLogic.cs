using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class EightGoldenDragonChallengeGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10gdchalleng";
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
                return "def_s=6,7,4,5,9,3,10,11,3,8,6,9,7,5,7&cfgs=8593&ver=3&def_sb=6,11,9,3,9&reel_set_size=3&def_sa=5,10,5,4,10&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"1000000\",max_rnd_win:\"8800\",max_rnd_win_a:\"8880\",max_rnd_hr_a:\"1000000\"}}&wl_i=tbm~8800;tbm_a~5920&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&purInit_e=1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&bls=10,15&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;1000,500,200,50,0;500,200,100,20,0;200,100,50,10,0;100,50,20,0,0;50,20,10,0,0;50,20,10,0,0;20,10,5,0,0;20,10,5,0,0;20,10,5,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=11,6,9,3,10,10,10,11,6,11,5,9,1,9,9,9,7,7,10,7,8,11,11,11,8,1,4,5,8,10,10~9,9,10,8,6,4,10,11,5,7,9,7,10,3,11,4,8,6,11~10,9,3,10,11,8,9,6,10,10,10,8,10,11,7,7,3,8,5,11,11,11,9,11,1,11,4,9,6,1,10,4~4,10,11,5,6,11,10,10,10,4,7,9,10,10,8,10,9,9,9,10,6,11,7,8,11,11,11,7,9,5,9,8,3,9,11~1,3,9,11,10,11,6,9,9,9,10,9,1,11,8,8,11,4,9,11,11,11,10,9,4,7,8,7,6,5&reel_set2=12,13,14,15,15,15,15,15~12,13,14,15,15,15,15,15~12,13,14,15,15,15,15,15&reel_set1=4,7,6,10,8,11,10,8,10,10,10,1,5,9,6,11,9,7,9,5,11,11,11,1,11,8,11,10,4,3,6,9,7,10~9,8,6,7,5,11,10,11,5,11,6,9,8,11,4,6,10,8,3,9,7,10,10~4,5,8,9,3,9,1,11,6,7,9,11,11,9,10,10,6,11,7,1,10,6,10,7,8,4,5,8,11,5~10,11,8,10,11,7,10,10,10,11,8,6,11,7,4,9,10,9,9,9,5,9,3,9,9,6,7,5,8,4~11,7,9,7,1,10,8,5,4,3,9,11,11,11,6,8,7,11,10,11,1,5,9,9,6,4,10,10,8&purInit=[{bet:1000,type:\"default\"}]&total_bet_min=20.00";
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
        public EightGoldenDragonChallengeGameLogic()
        {
            _gameID = GAMEID.EightGoldenDragonChallenge;
            GameName = "EightGoldenDragonChallenge";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["g"] = "{d1:{def_s:\"15,12,13,15,15,15,15,15,15\",def_sa:\"15,15,14\",def_sb:\"14,15,15\",s:\"15,12,13,15,15,15,15,15,15\",sa:\"15,15,14\",sb:\"14,15,15\",sh:\"3\",st:\"rect\",sw:\"3\"},d2:{def_s:\"15,12,13,15,15,15,15,15,12\",def_sa:\"15,15,15\",def_sb:\"14,12,13\",s:\"15,12,13,15,15,15,15,15,12\",sa:\"15,15,15\",sb:\"14,12,13\",sh:\"3\",st:\"rect\",sw:\"3\"},d3:{def_s:\"12,13,14,14,15,15,15,12,13\",def_sa:\"15,13,15\",def_sb:\"15,15,14\",s:\"12,13,14,14,15,15,15,12,13\",sa:\"15,13,15\",sb:\"15,15,14\",sh:\"3\",st:\"rect\",sw:\"3\"},d4:{def_s:\"15,15,12,15,15,15,15,12,13\",def_sa:\"15,14,15\",def_sb:\"13,15,14\",s:\"15,15,12,15,15,15,15,12,13\",sa:\"15,14,15\",sb:\"13,15,14\",sh:\"3\",st:\"rect\",sw:\"3\"},d5:{def_s:\"15,15,15,13,14,15,15,12,13\",def_sa:\"15,12,15\",def_sb:\"12,15,14\",s:\"15,15,15,13,14,15,15,12,13\",sa:\"15,12,15\",sb:\"12,15,14\",sh:\"3\",st:\"rect\",sw:\"3\"},d6:{def_s:\"15,15,12,15,12,13,15,15,15\",def_sa:\"15,15,15\",def_sb:\"13,14,12\",s:\"15,15,12,15,12,13,15,15,15\",sa:\"15,15,15\",sb:\"13,14,12\",sh:\"3\",st:\"rect\",sw:\"3\"},d7:{def_s:\"15,15,15,15,15,12,14,15,15\",def_sa:\"14,15,13\",def_sb:\"15,13,15\",s:\"15,15,15,15,15,12,14,15,15\",sa:\"14,15,13\",sb:\"15,13,15\",sh:\"3\",st:\"rect\",sw:\"3\"},d8:{def_s:\"15,12,13,15,15,15,13,14,15\",def_sa:\"15,15,12\",def_sb:\"14,15,15\",s:\"15,12,13,15,15,15,13,14,15\",sa:\"15,15,12\",sb:\"14,15,15\",sh:\"3\",st:\"rect\",sw:\"3\"}}";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
	        dicParams["bl"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("apwa"))
                dicParams["apwa"] = convertWinByBet(dicParams["apwa"], currentBet);
            if (dicParams.ContainsKey("rs_iw"))
                dicParams["rs_iw"] = convertWinByBet(dicParams["rs_iw"], currentBet);
            if (dicParams.ContainsKey("rs_win"))
                dicParams["rs_win"] = convertWinByBet(dicParams["rs_win"], currentBet);
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                BasePPSlotBetInfo betInfo = new BasePPSlotBetInfo();
                betInfo.BetPerLine        = (float)message.Pop();
                betInfo.LineCount         = (int)message.Pop();
		
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in EightGoldenDragonChallengeGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in EightGoldenDragonChallengeGameLogic::readBetInfoFromMessage", strGlobalUserID);
                    return;
                }
		
                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1}", strGlobalUserID, betInfo.LineCount);
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
                _logger.Error("Exception has been occurred in EightGoldenDragonChallengeGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
	
    }
}
