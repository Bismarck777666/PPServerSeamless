using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class RedHotLuckGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20powerpays";
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
                return 7;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=5,7,6,4,9,3,6,4,6,4,6,6,4,8,3,8,8,9,8,3,6,5,6,7,6,6,5,7,3,8,6,7,9,3,8,5,7,3,8,6,4,9,4,9,5,9,3,5,7&cfgs=11119&ver=3&def_sb=3,8,6,7,9,4,8&reel_set_size=5&def_sa=5,7,9,3,4,5,8&scatters=1~0,0,0,0,0,0,0~0,0,0,0,0,0,0~1,1,1,1,1,1,1&rt=d&gameInfo={rtps:{purchase:\"94.07\",regular:\"94.07\"},props:{max_rnd_sim:\"1\",points_per_level:\"50,75,100,125,150,200,250,300,350,400,500,600,700,800,900,1100,1300,1500,1700,1900,2200,2500,2800,3100,3400,3900,4400,4900,5400,5900,6700,7500,8300,9100,10000\",max_rnd_hr:\"1433091\",max_rnd_win:\"5000\",payout_per_level:\"0.2,0.3,0.4,0.5,0.6,0.8,1,1.25,1.5,1.75,2.5,3.25,4,4.75,5.5,7,8.5,10.5,12,13.5,15,17.5,20,22.5,25,30,35,40,45,50,60,70,80,90,100\"}}&wl_i=tbm~5000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1&wilds=2~0,0,0,0,0,0,0~1,1,1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0,0,0;0,0,0,0,0,0,0;0,0,0,0,0,0,0;2450,2400,2350,2300,2250,2200,2150,2100,2050,2000,1950,1900,1850,1800,1750,1700,1650,1600,1550,1500,1450,1400,1350,1300,1250,1200,1150,1100,1050,1000,950,900,850,800,750,700,650,600,550,500,450,400,350,300,250,0,0,0,0;1960,1920,1880,1840,1800,1760,1720,1680,1640,1600,1560,1520,1480,1440,1400,1360,1320,1280,1240,1200,1160,1120,1080,1040,1000,960,920,880,840,800,760,720,680,640,600,560,520,480,440,400,360,320,280,240,200,0,0,0,0;1470,1440,1410,1380,1350,1320,1290,1260,1230,1200,1170,1140,1110,1080,1050,1020,990,960,930,900,870,840,810,780,750,720,690,660,630,600,570,540,510,480,450,420,390,360,330,300,270,240,210,180,150,0,0,0,0;1225,1200,1175,1150,1125,1100,1075,1050,1025,1000,975,950,925,900,875,850,825,800,775,750,725,700,675,650,625,600,575,550,525,500,475,450,425,400,375,350,325,300,275,250,225,200,175,150,125,0,0,0,0;980,960,940,920,900,880,860,840,820,800,780,760,740,720,700,680,660,640,620,600,580,560,540,520,500,480,460,440,420,400,380,360,340,320,300,280,260,240,220,200,180,160,140,120,100,0,0,0,0;735,720,705,690,675,660,645,630,615,600,585,570,555,540,525,510,495,480,465,450,435,420,405,390,375,360,345,330,315,300,285,270,255,240,225,210,195,180,165,150,135,120,105,90,75,0,0,0,0;490,480,470,460,450,440,430,420,410,400,390,380,370,360,350,340,330,320,310,300,290,280,270,260,250,240,230,220,210,200,190,180,170,160,150,140,130,120,110,100,90,80,70,60,50,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=8,3,1,7,6,4,5,4,9,1,7,7,6,8,1,4,3,9,8,6,9,5,5,3~5,3,7,4,9,3,9,7,1,6,9,8,9,5,1,6,4,3,8,1,6,7,1,6,5,4,3,8,7,4,5,8~7,1,9,8,1,3,4,3,8,5,4,7,1,4,9,5,9,6,6,7,3,5,8,6~8,4,6,4,5,9,9,8,1,8,4,6,7,9,7,6,6,6,7,3,5,3,5,6,3,1,5,1,6,4,1,8,9,3,7~8,3,4,9,1,3,6,7,8,6,5,7,5,3,4,8,9,4,9,7,1,6,5~7,1,8,4,3,6,9,6,9,5,1,9,5,1,4,3,8,7,6,4,3,1,7,5,5,4,3,9,6,8,7,8~1,3,3,4,4,5,5,6,6,7,7,8,8,9,9&reel_set2=3,4,3,3,3,1,9,7,4,4,4,1,7,3,5,5,5,7,4,5,6,6,6,9,8,7,7,7,4,1,5,8,8,8,9,3,8,9,9,9,6,5,8,6~1,6,3,3,3,4,4,8,4,4,4,1,4,7,9,5,5,5,6,3,1,6,6,6,9,5,9,5,7,7,7,9,7,3,5,8,8,8,3,6,8,8,9,9,9,4,7,8,5,7~1,3,3,3,9,8,9,4,4,4,6,1,7,5,5,5,9,3,6,6,6,4,5,8,7,7,7,5,7,3,8,8,8,4,1,6,9,9,9,7,8,5,4~4,6,3,3,3,6,5,4,4,4,9,1,5,5,5,1,8,3,6,6,6,8,6,1,7,7,7,9,7,7,8,8,8,5,4,9,9,9,8,3,7,5~5,9,3,3,3,9,8,6,4,4,4,1,7,4,5,5,5,9,5,3,6,6,6,8,7,7,7,4,3,3,8,8,8,7,7,1,9,9,9,8,4,5,1~4,7,8,3,3,3,5,4,8,6,4,4,4,3,1,9,5,5,5,3,8,1,6,6,6,5,9,5,7,7,7,6,9,8,4,8,8,8,6,7,9,5,9,9,9,7,7,3,3,1~4,7,3,3,3,6,1,4,4,4,6,3,3,5,5,5,4,8,5,6,6,6,1,5,7,7,7,5,3,8,8,8,9,6,9,9,9,8,4,9,9&reel_set1=9,6,5,3,3,3,9,7,8,8,4,4,4,1,9,4,4,5,5,5,6,1,4,6,6,6,3,1,3,6,7,7,7,4,7,1,8,8,8,7,3,3,8,9,9,9,7,5,5,9,6~4,8,3,3,3,9,7,7,4,4,4,5,5,3,5,5,5,9,4,1,6,6,6,3,3,7,7,7,8,7,1,8,8,8,1,4,9,9,9,6,5,9,6~1,8,6,3,3,3,8,7,7,3,4,4,4,9,9,8,5,5,5,1,4,6,6,6,7,1,5,6,7,7,7,5,3,8,6,8,8,8,4,7,9,3,9,9,9,3,9,4,5,5~8,7,3,3,3,4,1,3,4,4,4,7,5,9,5,5,5,4,9,5,6,6,6,3,5,1,7,7,7,6,6,4,8,8,8,1,8,7,9,9,9,3,6,8,9~5,9,3,3,3,1,6,4,4,4,5,9,4,5,5,5,6,8,8,6,6,6,7,4,7,7,7,6,7,9,8,8,8,5,1,9,9,9,1,3,4,3~7,3,5,3,3,3,6,5,9,8,4,4,4,8,4,4,1,5,5,5,8,1,9,6,6,6,1,6,5,7,7,7,3,3,4,4,8,8,8,9,9,7,6,9,9,9,7,5,8,3,1~7,8,9,3,3,3,5,5,6,7,4,4,4,9,4,8,1,5,5,5,1,4,9,7,6,6,6,8,4,5,3,7,7,7,3,8,3,6,8,8,8,1,6,6,9,9,9,5,9,4,7,3&reel_set4=3,4,6,7,9~4,5,8,9~3,5,6,7,8~3,4,6,7,9~4,5,8,9~3,5,6,7,8~3,4,6,7,9&purInit=[{bet:2000,type:\"default\"}]&reel_set3=4,7,4,3,3,3,5,8,6,4,4,4,5,8,8,6,5,5,5,7,3,3,1,6,6,6,4,3,1,7,7,7,3,7,7,9,8,8,8,9,5,8,9,9,9,1,5,6,1~7,7,5,3,3,3,7,6,7,4,4,4,9,6,3,8,5,5,5,8,6,9,6,6,6,9,5,1,7,7,7,1,4,8,5,8,8,8,3,8,1,3,9,9,9,4,4,1,4~6,1,3,3,3,5,8,4,4,4,9,5,7,5,5,5,9,3,1,6,6,6,4,3,4,7,7,7,8,1,6,8,8,8,9,8,5,9,9,9,4,7,7,6~7,6,5,3,3,3,6,5,9,3,4,4,4,9,1,7,9,5,5,5,8,7,1,3,6,6,6,7,4,1,5,7,7,7,6,6,8,3,8,8,8,9,8,4,4,9,9,9,1,4,5,8,3~9,1,3,3,3,1,8,3,4,4,4,1,6,6,5,5,5,4,9,6,6,6,5,5,3,7,7,7,8,9,7,8,8,8,4,7,8,9,9,9,3,5,4,7~1,6,3,3,3,9,7,7,4,4,4,8,4,9,5,5,5,1,6,5,6,6,6,5,4,3,7,7,7,1,3,8,8,8,9,5,8,9,9,9,8,6,4,3~9,6,3,3,3,5,4,4,4,5,8,7,5,5,5,6,1,4,6,6,6,8,3,9,7,7,7,6,1,7,8,8,8,3,9,7,9,9,9,3,8,5,1&total_bet_min=10.00";
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
        public RedHotLuckGameLogic()
        {
            _gameID = GAMEID.RedHotLuck;
            GameName = "RedHotLuck";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "7";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("rs_win"))
                dicParams["rs_win"] = convertWinByBet(dicParams["rs_win"], currentBet);

            if (dicParams.ContainsKey("rs_iw"))
                dicParams["rs_iw"] = convertWinByBet(dicParams["rs_iw"], currentBet);

            
            if (dicParams.ContainsKey("apwa"))
                dicParams["apwa"] = convertWinByBet(dicParams["apwa"], currentBet);

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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in RedHotLuckGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in RedHotLuckGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
	
    }
}
