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
    class RideTheLightningBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get
            {
                return BetPerLine * 10;
            }
        }

        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
        }
    }
    class RideTheLightningGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs9ridelightng";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 9; }
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
                return "def_s=10,5,9,8,10,10,10,8,2&cfgs=1&ver=3&def_sb=6,10,10&reel_set_size=6&def_sa=5,10,10&scatters=1~0,0,0~0,0,0~1,1,1&rt=d&gameInfo={rtps:{purchase:\"96.48\",regular:\"96.50\"},props:{max_rnd_sim:\"0\",any_bar:\"3\",max_rnd_hr:\"22573363\",max_rnd_win:\"10000\",any_w:\"250\",any_7:\"10\"}}&wl_i=tbm~10000&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&purInit_e=1&wilds=2~5000,0,0~1,1,1;3~2500,0,0~1,1,1;4~1000,0,0~1,1,1&bonuses=0&ntp=0.00&paytable=0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;70,0,0;40,0,0;25,0,0;10,0,0;5,0,0;0,0,0&total_bet_max=10,000,000.00&reel_set0=9,10,4,10,8,10,9,10,8,10,6,10,2,10,5,10,8,10,9,10,3,10,7,10~6,10,8,10,9,10,8,10,4,10,2,10,8,10,5,10,9,10,3,10,7,10,9,10~5,10,8,10,2,10,9,10,7,10,8,10,1,10,4,10,9,10,3,10,6,10&reel_set2=9,10,5,10,9,10,3,10,2,10,7,10,4,10,8,10,6,10,8,10~5,10,4,10,2,10,9,10,8,10,3,10,6,10,8,10,7,10,8,10,9,10,9,10~7,10,8,10,2,10,5,10,8,10,4,10,1,10,6,10,9,10,9,10,3,10,9,10&reel_set1=8,10,8,10,9,10,5,10,4,10,6,10,9,10,9,10,7,10,2,10,8,10,3,10~4,10,3,10,5,10,7,10,6,10,9,10,8,10,8,10,9,10,2,10~1,10,5,10,4,10,8,10,9,10,7,10,8,10,3,10,9,10,6,10,2,10&reel_set4=8,10,7,10,5,10,6,10,2,10,9,10,4,10,3,10,8,10,9,10~5,10,6,10,9,10,9,10,8,10,4,10,2,10,8,10,3,10,8,10,9,10,7,10~1,10,8,10,8,10,9,10,6,10,7,10,3,10,9,10,4,10,5,10,2,10&purInit=[{bet:1000,type:\"default\"}]&reel_set3=7,10,2,10,9,10,8,10,3,10,9,10,8,10,4,10,9,10,8,10,6,10,5,10~4,10,7,10,5,10,9,10,8,10,6,10,3,10,8,10,2,10,9,10~8,10,7,10,4,10,9,10,2,10,1,10,3,10,5,10,8,10,9,10,6,10,9,10&reel_set5=8,10,9,10,2,10,3,10,4,10,9,10,5,10,7,10,6,10,8,10~6,10,9,10,3,10,7,10,2,10,8,10,9,10,4,10,5,10,8,10,8,10,9,10~8,10,9,10,9,10,6,10,8,10,2,10,4,10,5,10,1,10,7,10,3,10,9,10&total_bet_min=200.00";
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
        
        public RideTheLightningGameLogic()
        {
            _gameID = GAMEID.RideTheLightning;
            GameName = "RideTheLightning";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "3";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
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


                RideTheLightningBetInfo betInfo = new RideTheLightningBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();
		
		
                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;
		
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in RideTheLightningGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                _logger.Error("Exception has been occurred in RideTheLightningGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            RideTheLightningBetInfo betInfo = new RideTheLightningBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new RideTheLightningBetInfo();
        }
    }
}
