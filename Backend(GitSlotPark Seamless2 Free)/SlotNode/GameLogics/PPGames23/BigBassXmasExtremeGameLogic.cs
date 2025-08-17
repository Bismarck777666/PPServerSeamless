using Akka.Actor;
using GITProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    public class BigBassXmasExtremeResult : BasePPSlotSpinResult
    {
        public List<int> BonusSelections { get; set; }

        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            BonusSelections = SerializeUtils.readIntList(reader);
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            SerializeUtils.writeIntList(writer, BonusSelections);
        }
    }
    class BigBassXmasExtremeGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10bbxext";
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
                return "def_s=4,11,6,4,9,7,10,7,5,8,6,11,12,3,9&cfgs=1&ver=3&def_sb=4,10,6,6,9&reel_set_size=14&def_sa=5,10,6,4,11&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{ante:\"96.06\",purchase:\"96.07\",regular:\"96.07\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"14321518\",max_rnd_win:\"10000\",max_rnd_win_a:\"6666\",max_rnd_hr_a:\"8097166\"}}&wl_i=tbm~10000;tbm_a~6666&reel_set10=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&reel_set11=12,6,7,8,10,2,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,2,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,2,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,2,12,9,10,4,3,11,5,2,9,8,7,7,7,7,7~6,12,2,3,6,7,11,3,5,7,10,4,3,5,2,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,2,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,2,12,5,2,8,7,6,4,11,3,9,7,7,10,6,3,5,2,7,7,7,7,7~5,4,6,7,11,2,4,8,2,7,5,3,12,7,7,10,2,6,9,7,7,7,7,7&reel_set12=12,6,7,8,10,2,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,2,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,2,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,2,12,9,10,4,3,11,5,2,9,8,7,7,7,7,7~6,12,2,3,6,7,11,3,5,7,10,4,3,5,2,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,2,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,2,12,5,2,8,7,6,4,11,3,9,7,7,10,6,3,5,2,7,7,7,7,7~5,4,6,7,11,2,4,8,2,7,5,3,12,7,7,10,2,6,9,7,7,7,7,7&purInit_e=1&reel_set13=4,7,12,9,11,5,7,3,1,10,3,8,12,8,7,7,5,8,12,9,10,6,8,9,6,10,7,9,1,6,9,7,4,7,6,12,4,11,10,9,8,5,11,1,6,7,8,11,4,11,12,10,6,7,5~12,6,10,4,5,6,9,7,7,1,11,12,7,11,3,9,7,7,8,10,1,6,4,5,11,10,8,12,6,8,7~10,6,8,5,8,9,7,4,1,12,7,4,9,7,11,9,11,8,7,3,1,7,7,11,8,12,5,12,6,10,4,6,3,1,5,10,11,6,8,7,10~8,5,9,10,5,7,4,8,9,10,1,7,11,3,12,3,8,6,12,8,7,12,1,7,7,7,7,6,7,10,5,7,7,11,9,5,11,1,6,8,6,4,12,7,6,11,7,9,1,10,4,11,9~7,5,7,3,10,7,7,11,7,7,1,12,4,8,6,12,8,12,9,7,7,7,7,9,1,8,12,3,4,9,11,5,9,6,4,10,1,11,8,6,10,5,11,6&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&bls=10,15&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;2000,200,50,5,0;1000,150,30,0,0;500,100,20,0,0;500,100,20,0,0;0,0,0,0,0;100,25,2,0,0;100,25,2,0,0;50,10,2,0,0;50,10,2,0,0;50,10,2,0,0&total_bet_max=10,000,000.00&reel_set0=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&accInit=[{id:0,mask:\"cp;lvl;tp;sc\"}]&reel_set2=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&reel_set1=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&reel_set4=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&purInit=[{bet:1000,type:\"default\"}]&reel_set3=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&reel_set6=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&reel_set5=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&reel_set8=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&reel_set7=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&reel_set9=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&total_bet_min=20.00";
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
        public BigBassXmasExtremeGameLogic()
        {
            _gameID     = GAMEID.BigBassXmasExtreme;
            GameName    = "BigBassXmasExtreme";
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
                betInfo.BetPerLine  = (float)message.Pop();
                betInfo.LineCount   = (int)message.Pop();
		
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in vs10bbxextGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in vs10bbxextGameLogic::readBetInfoFromMessage", strGlobalUserID);
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
                _logger.Error("Exception has been occurred in BigBassXmasExtremeGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst, PPFreeSpinInfo freeSpinInfo)
        {
            try
            {
                BigBassXmasExtremeResult spinResult = new BigBassXmasExtremeResult();
                Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(dicParams, betInfo.TotalBet);

                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                spinResult.NextAction = convertStringToActionType(dicParams["na"]);
                if (spinResult.NextAction == ActionTypes.DOCOLLECT || spinResult.NextAction == ActionTypes.DOCOLLECTBONUS)
                    spinResult.TotalWin = double.Parse(dicParams["tw"]);
                else
                    spinResult.TotalWin = 0.0;

                if (freeSpinInfo != null)
                {

                    dicParams["fra"] = Math.Round(freeSpinInfo.TotalWin, 2).ToString();
                    dicParams["frn"] = freeSpinInfo.RemainCount.ToString();
                    dicParams["frt"] = "N";
                }
                spinResult.ResultString = convertKeyValuesToString(dicParams);

                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BigBassXmasExtremeGameLogic::calculateResult {0}", ex);
                return null;
            }
        }
        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            BigBassXmasExtremeResult result = new BigBassXmasExtremeResult();
            result.SerializeFrom(reader);
            return result;
        }
        private int getStatusLength(string strStatus)
        {
            string[] strParts = strStatus.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            return strParts.Length;
        }
        private bool isStatusAllZero(string strStatus)
        {
            string[] strParts = strStatus.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < strParts.Length; i++)
            {
                if (strParts[i] != "0")
                    return false;
            }
            return true;
        }
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency, bool isAffiliate)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                double realWin      = 0.0;
                string strGameLog   = "";
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                ToUserResultMessage resultMsg = null;
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) ||
                    !_dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BigBassXmasExtremeResult result = _dicUserResultInfos[strGlobalUserID] as BigBassXmasExtremeResult;
                    BasePPSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
                    BasePPActionToResponse actionResponse = betInfo.pullRemainResponse();
                    if (actionResponse.ActionType != ActionTypes.DOBONUS)
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        Dictionary<string, string> dicParams = splitResponseToParams(actionResponse.Response);
                        convertWinsByBet(dicParams, betInfo.TotalBet);
                        convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                        int ind = -1;

                        if (dicParams.ContainsKey("g"))
                        {
                            var gParam = JToken.Parse(dicParams["g"]);
                            int bg0End = -1;
                            if (gParam["bg_0"] != null)
                            {
                                if (gParam["bg_0"]["end"].ToString() == "1")
                                    bg0End = 1;
                                else
                                    bg0End = 0;
                            }

                            if (gParam["bg_0"]["status"] != null && !isStatusAllZero((string)gParam["bg_0"]["status"]))
                            {
                                ind = (int)message.Pop();
                                int statusLength = getStatusLength((string)gParam["bg_0"]["status"]);
                                if (statusLength == 14)
                                {
                                    if (result.BonusSelections == null)
                                        result.BonusSelections = new List<int>();

                                    if (result.BonusSelections.Contains(ind))
                                    {
                                        betInfo.pushFrontResponse(actionResponse);
                                        saveBetResultInfo(strGlobalUserID);
                                        throw new Exception(string.Format("{0} User selected already selected position, Malicious Behavior {1}", strGlobalUserID, ind));
                                    }

                                    result.BonusSelections.Add(ind);
                                    int[] status = new int[statusLength];
                                    for (int i = 0; i < result.BonusSelections.Count; i++)
                                        status[result.BonusSelections[i]] = i + 1;
                                    gParam["bg_0"]["status"] = string.Join(",", status);

                                    if (gParam["bg_0"]["wins"] != null)
                                    {
                                        string[] strWins    = ((string)gParam["bg_0"]["wins"]).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                        string[] strNewWins = new string[statusLength];
                                        for (int i = 0; i < statusLength; i++)
                                            strNewWins[i] = "0";
                                        for (int i = 0; i < result.BonusSelections.Count; i++)
                                            strNewWins[result.BonusSelections[i]] = strWins[i];
                                        gParam["bg_0"]["wins"] = string.Join(",", strNewWins);
                                    }

                                    if (gParam["bg_0"]["wins_mask"] != null)
                                    {
                                        string[] strWinsMask    = ((string)gParam["bg_0"]["wins_mask"]).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                        string[] strNewWinsMask = new string[statusLength];
                                        for (int i = 0; i < statusLength; i++)
                                            strNewWinsMask[i] = "h";
                                        for (int i = 0; i < result.BonusSelections.Count; i++)
                                            strNewWinsMask[result.BonusSelections[i]] = strWinsMask[i];
                                        gParam["bg_0"]["wins_mask"] = string.Join(",", strNewWinsMask);
                                    }
                                }
                                else
                                {
                                    int[] status = new int[statusLength];
                                    status[ind] = 1;
                                    gParam["bg_0"]["status"] = string.Join(",", status);

                                    if (gParam["bg_0"]["wins"] != null)
                                    {
                                        string[] strWins = ((string)gParam["bg_0"]["wins"]).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                        if (ind != 0)
                                        {
                                            string strTemp = strWins[ind];
                                            strWins[ind] = strWins[0];
                                            strWins[0] = strTemp;
                                        }
                                        gParam["bg_0"]["wins"] = string.Join(",", strWins);
                                    }
                                    if (gParam["bg_0"]["wins_mask"] != null)
                                    {
                                        string[] strWinsMask = ((string)gParam["bg_0"]["wins_mask"]).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                        if (ind != 0)
                                        {
                                            string strTemp      = strWinsMask[ind];
                                            strWinsMask[ind]    = strWinsMask[0];
                                            strWinsMask[0]      = strTemp;
                                        }
                                        gParam["bg_0"]["wins_mask"] = string.Join(",", strWinsMask);
                                    }
                                }
                            }
                            dicParams["g"] = JsonConvert.SerializeObject(gParam);
                        }

                        result.BonusResultString = convertKeyValuesToString(dicParams);
                        addDefaultParams(dicParams, userBalance, index, counter);

                        ActionTypes nextAction = convertStringToActionType(dicParams["na"]);
                        string strResponse = convertKeyValuesToString(dicParams);

                        responseMessage.Append(strResponse);

                        //히스토리보관 및 초기화
                        if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                        {
                            if (ind >= 0)
                                addIndActionHistory(strGlobalUserID, "doBonus", strResponse, index, counter, ind);
                            else
                                addActionHistory(strGlobalUserID, "doBonus", strResponse, index, counter);
                        }

                        if (nextAction == ActionTypes.DOCOLLECT || nextAction == ActionTypes.DOCOLLECTBONUS)
                        {
                            realWin = double.Parse(dicParams["tw"]);
                            strGameLog = strResponse;
                            if (realWin > 0.0f)
                            {
                                _dicUserHistory[strGlobalUserID].baseBet    = betInfo.TotalBet;
                                _dicUserHistory[strGlobalUserID].win        = realWin;
                            }

                            resultMsg = new ToUserResultMessage((int)_gameID, responseMessage, 0.0, realWin, new GameLogInfo(this.GameName, "0", strGameLog), UserBetTypes.Normal); ;
                            resultMsg.BetTransactionID  = betInfo.BetTransactionID;
                            resultMsg.RoundID           = betInfo.RoundID;
                            resultMsg.TransactionID     = createTransactionID();
                            if (_dicUserFreeSpinInfos.ContainsKey(strGlobalUserID) && !_dicUserFreeSpinInfos[strGlobalUserID].Pending)
                            {
                                resultMsg.FreeSpinID = _dicUserFreeSpinInfos[strGlobalUserID].FreeSpinID;
                                addFreeSpinBonusParams(responseMessage, _dicUserFreeSpinInfos[strGlobalUserID], strGlobalUserID, realWin);
                            }
                        }
                        copyBonusParamsToResult(dicParams, result);
                        result.NextAction = nextAction;
                    }
                    if (!betInfo.HasRemainResponse)
                        betInfo.RemainReponses = null;

                    saveBetResultInfo(strGlobalUserID);
                }

                if (resultMsg == null)
                    Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
                else
                    Sender.Tell(resultMsg);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BigBassXmasExtremeGameLogic::onDoBonus {0}", ex);
            }
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
