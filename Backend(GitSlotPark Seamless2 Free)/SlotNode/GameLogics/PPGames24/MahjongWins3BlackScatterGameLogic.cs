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
    class MahjongWins3BlackScatterBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get
            {
                return BetPerLine * 20;
            }
        }
    }

    class MahjongWins3BlackScatterGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vswaysmahwblck";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 1024; }
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
                return "reel_set29=reel_set10&def_s=5,8,3,3,7,4,10,6,6,10,3,11,3,3,9,5,8,4,5,10,15,11,6,3,15&reel_set25=reel_set10&reel_set26=reel_set10&reel_set27=reel_set10&reel_set28=reel_set10&reel_set32=reel_set10&reel_set33=reel_set10&reel_set34=reel_set10&reel_set35=reel_set10&reel_set30=reel_set10&reel_set31=reel_set10&cfgs=1&ver=3&def_sb=4,9,9,5,9&reel_set_size=44&def_sa=4,7,7,6,10&reel_set36=reel_set10&reel_set37=reel_set10&reel_set38=reel_set10&reel_set39=reel_set10&reel_set43=14,14,14,14,14~14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,13,14,14,14,14,14,14,13,14,13,14,14,13,14,14,14,14,13,14,14,14,14,14,14,14,14,13,13,14,14,14,14,14,14,14,14,14,14,14,14,14,14,13,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,13,14,14,14,14,14,14,14,13,14,14,14~13,13,13,13,13~14,14,13,14,14,14,14,14,13,14,13,13,14,14,14,14,13,14,14,14,14,14,14,14,14,13,14,14,14,13,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,13,14,14,13,14,14,14,14,13,14,14,14,14,14,14,14,14,13,13,14,14,14,14,14,14,14,14,14,14,14,14,14,14,13,14,14,14,14,14,14,14~14,14,14,14,14&reel_set40=10,5,10,10,5,5,5,8,1,10,8,6,6,10,10,1,5,5,7,7,7,10,6,8,8,10,5,5,8,5,6,10,10,1,10,7,7,6,6,10,10,1,10,7,7,5,5,8,10,10,7,7,7,8,10,10,10,8,5,5,10,7,8,8,10,6,10,10,7,7,7,6,6,10,8,8,7,10,7,7,8,8,7,7,8,6,6,10,10,5,7,7,10,5,10,10,5,7,8,8~3,3,3,9,12,11,11,6,11,1,9,3,9,11,1,4,3,6,11,12,9,9,9,11,1,6,11,3,6,12,11,3,11,11,1,9,3,6,6,12,11,6,9,9,1,11,9,11,6,12,6,4,9,3,1,11,6,6,9,12,4,6,9,9,1,3,9,6,9,1,3,6,11,9,12,9,11,3,6,1,4,6,9,11,12,6,11,9,6,1,11,3,9,11,12,3,9,4,6,1~10,4,10,8,12,4,7,10,4,1,3,8,5,10,1,4,4,4,11,12,3,4,8,8,1,10,8,5,7,12,10,9,4,4,1,10,8,3,4,12,10,4,10,8,1,5,7,7,10,12,10,8,7,7,1,10,4,11,10,12,10,8,3,7,1,8,8,10,8,1,3,3,8,4,12,8,5,5,5,1,4,10,8,3,12,7,10,4,10,1,3,8,7,4,12,8,4,10,8,1~6,8,4,4,12,9,11,9,11,1,8,4,11,11,1,7,11,11,6,12,11,9,4,11,1,7,9,5,5,12,11,9,8,11,1,7,8,6,6,12,8,10,4,8,1,6,9,7,4,12,7,5,6,7,1,4,7,8,9,12,11,11,7,9,1,4,7,8,11,1,6,8,11,9,12,3,11,11,7,1,5,5,5,9,12,7,8,3,3,1,9,7,4,8,12,6,9,4,8,1~10,5,9,10,10,6,3,11,10,1,5,5,5,11,11,4,10,10,9,1,10,3,11,11,9,6,9,10,1,11,7,11,9,11,4,11,10,10,5,5,10,9,9,8,4,4,4,10,5,9,9,1,10,9,11,9,11,3,3,3,9,10,5,10,10,5,7,11,11,11,9,10,5,10,11,11,6,6,10,11,9,11,5,10,7,1,11,9,8,9,10,11,5,10,7,9,5,5,10,11&reel_set41=7,5,3,5,5,5,11,5,11,9,5,3~12,3,10,12,4,3,3,12,10,5,3,5,3,5~11,12,4,7~6,10,12,7,3,12,9,3,3,9,10,3,6~11,9,6,5,11,4,10,11,9&reel_set42=14,14,14,14,14~14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,13,14,14,14,14,14,14,13,14,13,14,14,13,14,14,14,14,13,14,14,14,14,14,14,14,14,13,13,14,14,14,14,14,14,14,14,14,14,14,14,14,14,13,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,13,14,14,14,14,14,14,14,13,14,14,14~14,14,14,14,14,14,14,14,14,14,14,14,14,13,14,14,14,14,14,14,13,14,14,14,14,14,13,14,13,13,14,14,14,14,13,14,14,14,14,14,14,14,14,13,14,14,14,13,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,13,14,14,14,14,14,14,14,14,14,14,14,14,14,14,13,14,14,14,14,14,14,14~14,14,13,14,14,14,14,14,13,14,13,13,14,14,14,14,13,14,14,14,14,14,14,14,14,13,14,14,14,13,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,13,14,14,13,14,14,14,14,13,14,14,14,14,14,14,14,14,13,13,14,14,14,14,14,14,14,14,14,14,14,14,14,14,13,14,14,14,14,14,14,14~14,14,14,14,14&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1;12~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{purchase:\"97.00\",regular:\"97.00\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"1000000000\",max_rnd_win:\"100000\"}}&wl_i=tbm~100000&reel_set10=6,7,8,7,8,7,10,7,5,5,1,6,10,8,8,11,8,5,3,3,3,5,3,6,5,6,10,5,6,6,8,10,6,9,7,4,10,5,10,4,4,4,10,5,10,8,9,10,7,10,5,4,5,6,4,6,10,9,4,10,8,5,5,5,8,5,10,8,4,6,3,8,10,10,5,4,10,4,8,5,4,6,11,6,6,6,3,8,7,9,8,4,10,8,10,8,10,5,11,8,10,5,6,8,10,10~9,11,4,6,3,6,11,11,4,6,9,8,3,11,5,3,6,8,6,7,3,7,3,3,3,10,8,3,9,9,3,11,3,9,10,6,7,9,6,6,8,7,4,3,6,11,9,8,4,4,4,3,11,4,11,4,6,8,11,9,9,5,9,7,8,4,7,3,10,3,9,11,3,9,6,6,6,1,7,3,9,11,6,11,9,4,11,11,10,5,7,3,3,5,11,9,6,9,9,11~3,5,6,10,9,7,10,7,3,7,7,8,10,8,3,3,3,8,10,3,10,8,10,9,11,3,8,5,3,8,8,10,8,5,4,4,4,6,4,10,6,10,5,5,8,8,11,5,7,10,1,5,4,7,5,5,5,8,3,6,5,3,5,6,3,10,8,7,4,8,4,9,10,6,6,6,4,11,8,5,8,5,10,11,9,5,7,5,8,5,4,3~11,11,8,9,11,6,8,10,4,11,7,3,3,3,8,11,1,7,4,6,9,5,8,4,5,6,6,4,4,4,7,5,6,9,4,9,3,11,8,11,5,5,5,6,7,4,7,4,9,11,11,10,4,8,3,8,6,6,6,9,3,8,11,11,9,7,6,7,5,7,6,7,9~11,10,4,6,11,7,10,10,5,10,9,4,10,4,10,11,11,11,5,9,11,5,10,1,9,3,5,11,10,10,4,9,3,3,3,11,3,10,11,5,6,6,9,10,9,11,11,8,9,5,6,4,4,4,9,10,9,5,5,10,11,9,3,10,9,10,4,10,9,5,5,5,3,6,11,3,7,5,10,9,11,1,11,10,1,11,9,6,6,6,4,5,5,10,5,6,9,11,10,9,7,10,8,9,10,9,7&sc=5.00,10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&reel_set11=reel_set10&reel_set12=reel_set10&purInit_e=1&reel_set13=reel_set10&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&reel_set18=reel_set10&reel_set19=reel_set10&ntp=0.00&reel_set14=reel_set10&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;50,25,10,0,0;40,20,8,0,0;30,15,6,0,0;15,10,5,0,0;12,5,3,0,0;12,5,3,0,0;10,4,2,0,0;6,3,1,0,0;6,3,1,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&reel_set15=reel_set10&reel_set16=reel_set10&reel_set17=reel_set10&total_bet_max=10,000,000.00&reel_set21=reel_set10&reel_set22=reel_set10&reel_set0=6,10,10,5,4,4,10,5,10,10,8,5,10,6,10,10,8,8,6,3,3,3,7,8,7,5,7,10,10,6,5,10,11,7,10,5,4,10,5,3,5,5,4,4,4,8,6,8,10,10,8,8,10,4,10,10,5,8,8,6,7,6,10,6,6,5,5,5,10,8,5,5,10,4,10,3,10,7,3,8,3,8,6,8,4,7,6,6,6,10,10,4,4,1,5,8,6,9,8,6,10,10,8,5,8,8,7,5,10,4~8,11,11,6,3,9,10,9,9,7,11,9,7,9,9,8,3,3,9,3,4,9,3,3,3,6,3,9,6,5,3,9,11,4,11,5,3,6,3,7,6,6,3,9,11,9,6,11,3,4,4,4,9,6,3,7,9,1,4,6,9,11,8,3,9,3,11,9,11,11,4,11,8,11,6,6,6,3,9,11,9,12,3,6,7,9,6,4,3,9,9,6,11,4,7,11,9,7,6,6,9~5,3,10,4,10,8,7,10,8,5,11,3,3,3,5,3,8,10,5,10,5,7,5,3,8,5,3,4,4,4,6,7,8,9,3,10,6,3,8,7,5,7,5,5,5,10,6,8,12,7,5,11,8,10,11,9,4,8,6,6,6,10,8,8,5,10,5,6,5,8,8,7,3,4,5~6,9,7,8,9,4,4,7,8,4,3,7,6,8,9,8,3,3,3,6,11,9,11,3,8,6,7,9,11,10,8,4,9,5,7,8,5,10,4,4,4,7,11,8,3,4,9,11,11,6,7,5,6,5,6,7,11,4,9,5,5,5,11,5,11,8,6,7,11,9,11,7,11,3,8,7,7,6,6,11,6,6,6,12,4,11,6,4,4,8,5,9,11,7,4,7,11,6,4,8,9,11,4~11,9,10,10,11,7,4,9,9,5,11,11,11,3,11,10,10,11,10,9,6,6,10,10,7,6,3,3,3,9,9,11,10,1,11,10,10,3,4,5,9,6,4,4,4,11,8,10,5,11,5,1,10,11,9,9,1,5,5,5,10,9,5,5,10,8,11,7,3,4,10,10,9,6,6,6,11,9,3,6,10,5,11,4,5,5,9,4,5,9&reel_set23=reel_set10&reel_set24=reel_set10&reel_set2=reel_set0&reel_set1=reel_set0&reel_set4=reel_set0&purInit=[{bet:2000,type:\"default\"}]&reel_set3=reel_set0&reel_set20=reel_set10&reel_set6=reel_set0&reel_set5=reel_set0&reel_set8=reel_set0&reel_set7=reel_set0&reel_set9=reel_set0&total_bet_min=100.00";
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
        public MahjongWins3BlackScatterGameLogic()
        {
            _gameID     = GAMEID.MahjongWins3BlackScatter;
            GameName    = "MahjongWins3BlackScatter";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["g"] = "{gp:{def_s:\"14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14\",def_sa:\"14,14,14,14,14\",def_sb:\"14,14,14,14,14\",reel_set:\"42\",s:\"14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14\",sa:\"14,14,14,14,14\",sb:\"14,14,14,14,14\",sh:\"5\",st:\"rect\",sw:\"5\"}}";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            if (dicParams.ContainsKey("rs_iw"))
                dicParams["rs_iw"] = convertWinByBet(dicParams["rs_iw"], currentBet);

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


                MahjongWins3BlackScatterBetInfo betInfo = new MahjongWins3BlackScatterBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();
		
		
                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;
		
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in MahjongWins3BlackScatterGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in MahjongWins3BlackScatterGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            MahjongWins3BlackScatterBetInfo betInfo = new MahjongWins3BlackScatterBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new MahjongWins3BlackScatterBetInfo();
        }
    }
}
