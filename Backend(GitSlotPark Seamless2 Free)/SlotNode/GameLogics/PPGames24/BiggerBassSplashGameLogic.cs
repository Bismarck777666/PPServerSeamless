using Akka.Actor;
using GITProtocol;
using MongoDB.Bson;
using PCGSharp;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class BiggerBassSplashBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get
            {
                return BetPerLine * 10;
            }
        }

        public int PurchaseType { get; set; }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            this.PurchaseType = reader.ReadInt32();
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(PurchaseType);
        }
    }


    class BiggerBassSplashGameLogic : BasePPSlotGame
    {
        protected double[] _multiTotalFreeSpinWinRates;
        protected double[] _multiMinFreeSpinWinRates;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs12bgrbspl";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 12; }
        }
        protected override int ServerResLineCount
        {
            get { return 10; }
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
                return "def_s=11,4,5,12,3,8,10,10,6,5,9,3,4,12,3,8,10,11,6,10&cfgs=1&ver=3&mo_s=7&mo_v=20,50,100,150,200,250,500,1000,2000,5000,16660,25000,50000&def_sb=11,3,4,12,5&reel_set_size=14&def_sa=12,4,9,11,10&scatters=1~0,0,0,0,0,0~0,0,0,0,0,0~1,1,1,1,1,1&rt=d&gameInfo={rtps:{ante:\"96.50\",purchase_1:\"96.50\",purchase_0:\"96.50\",regular:\"96.50\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"1683555\",max_rnd_win:\"5000\",max_rnd_sim_a:\"1\",max_rnd_win_a:\"3334\",max_rnd_hr_a:\"972768\"}}&wl_i=tbm~5000;tbm_a~3334&reel_set10=11,12,3,4,5,6,7,7,8,9,10~11,12,3,4,5,6,7,7,8,9,10~5,8,10,6,10,11,7,7,5,11,9,12,3,7,8,12,9,4,6~11,12,3,4,5,6,7,7,8,9,10~12,4,8,9,5,10,9,12,6,8,11,3,5,6,7,7,10,7&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&reel_set11=8,11,7,7,8,10,6,10,8,10,5,12,9,8,4,12,10,12,3~11,10,8,11,12,3,9,11,5,8,9,3,7,7,11,9,7,12,5,9,4,10,9,6,7,11~4,6,5,10,3,9,5,6,4,11,6,8,4,11,7,12,9,12,7,10,7~11,12,3,4,5,6,7,7,8,9,10~8,10,11,3,7,6,7,5,9,7,12,4&reel_set12=8,12,7,4,11,10,8,3,11,12,10,7,7,7,12,10,8,9,8,5,4,7,10,9,8,7,6~3,7,8,11,9,7,10,11,3,9,8,5,7,11,9,10,9,5,9,11,6,7,12,4,12~7,9,4,3,6,11,5,7,7,7,8,7,6,5,7,10,7,4,12~3,10,3,7,7,6,4,7,7,7,11,9,8,4,12,7,5,6,5~7,6,10,11,5,7,7,4,3,7,7,7,8,10,9,4,12,6,8,9,12,11,7,5&purInit_e=1,1&reel_set13=7,10,8,10,5,6,5,10,6,5,12,6,4,9,8,6,8,7,7,12,11,12,7,7,7,7,5,9,10,4,11,9,7,11,12,9,7,8,6,7,7,4,11,7,3,11,9,8,7,7,6,12~7,4,6,3,7,4,8,11,7,6,12,8,10,5,6,7,6,8,7,9,7,11,3,7,7,7,7,8,7,10,11,7,10,5,12,7,12,7,10,12,11,7,12,7,6,5,8,9,11,9,4,5~4,12,5,11,10,6,8,7,7,8,5,4,5,4,11,6,3,7,8,7,11,7,7,7,7,12,9,12,10,6,9,10,7,12,11,10,7,10,9,8,7,7,6,12,9,3,9,8~7,6,7,10,8,7,11,9,8,12,11,9,5,6,11,7,7,7,7,12,10,9,6,12,4,7,6,7,7,10,12,7,11,9,3,5,4,5~10,7,4,8,7,5,7,3,10,12,6,10,7,9,7,7,7,7,8,12,11,6,5,6,4,8,7,12,5,9,11,7,9,6&wilds=2~0,0,0,0,0,0~1,1,1,1,1,1&bonuses=0&bls=10,15&ntp=0.00&paytable=0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;2000,200,50,5,0;1000,150,30,0,0;500,100,20,0,0;500,100,20,0,0;0,50,10,0,0;100,25,2,0,0;100,25,2,0,0;50,10,2,0,0;50,10,2,0,0;50,10,2,0,0&total_bet_max=35,000,000.00&reel_set0=12,7,6,12,9,11,7,11,7,1,7,4,11,9,8,9,7,7,7,7,9,6,8,7,4,10,12,6,10,5,6,10,5,12,6,8,3,8,7,5~7,8,9,10,7,11,5,12,8,6,1,7,8,4,11,8,7,7,7,7,4,7,5,11,6,12,5,6,9,10,12,6,7,7,12,7,3,9,10~8,12,8,12,11,7,9,7,8,10,7,6,4,12,10,12,7,7,7,7,6,9,7,9,7,8,9,10,11,5,4,7,6,1,3,5,7,6,10~6,7,8,6,7,7,8,7,6,7,10,11,10,12,6,3,11,4,9,11,8,6,3,10,12,7,7,7,7,5,7,5,7,8,7,9,7,4,8,7,5,9,1,11,12,9,12,8,7,11,7,5,6,12,10,9,11~10,7,12,7,6,1,8,7,7,8,11,6,9,4,7,12,3,6,9,8,7,7,7,7,6,4,10,4,7,7,11,9,12,7,11,5,10,11,12,9,5,7,8,3,5,9,12,10&accInit=[{id:0,mask:\"cp;lvl;tp\"}]&reel_set2=6,5,11,1,4,9,7,3,5,6,11,10,7,8,10,7,12,7,7,7,7,8,1,9,4,10,7,6,7,5,6,12,11,10,12,8,9,8,7,7,12~6,10,8,12,11,8,7,6,7,6,11,7,10,9,7,8,12,1,4,11,1,7,11,7,7,7,7,3,10,6,8,7,1,5,4,7,4,8,10,12,7,9,12,6,5,7,7,9,3,6,12,5~5,6,8,10,11,12,7,8,9,5,3,7,12,7,7,7,7,9,11,7,8,9,7,4,10,7,12,7,12,4,6,9,10~5,4,8,6,3,8,11,7,10,5,9,7,7,6,12,7,7,7,7,12,6,12,5,6,7,9,1,11,7,4,10,8,7,7,10,11,9~8,7,12,11,3,7,8,9,7,12,6,9,7,12,5,6,11,12,7,8,7,7,7,7,11,6,3,4,6,7,4,8,5,11,7,10,4,9,10,7,10,9,5,7,6&reel_set1=7,8,11,4,1,12,11,9,8,12,6,9,5,8,7,6,11,10,9,7,11,10,7,7,7,7,3,1,8,10,7,6,10,11,12,4,7,7,12,6,9,7,10,6,5,4,1,7,5,12,8~8,6,3,11,8,11,12,11,4,7,1,12,9,7,9,7,10,5,11,4,12,9,8,7,7,7,7,6,8,5,6,7,12,1,7,7,6,12,10,4,7,7,1,6,10,5,10,6,7,3,7~11,8,7,10,8,12,10,12,5,7,8,5,7,5,11,12,8,7,6,10,7,7,9,7,11,4,10,7,7,7,7,10,6,9,12,9,7,6,4,12,6,5,11,12,9,10,11,3,7,9,7,3,8,9,8,7,6,1,8~11,9,3,7,7,11,7,4,12,6,8,11,9,5,7,4,10,9,7,4,5,7,7,7,7,6,8,3,12,8,9,8,6,12,7,12,10,9,11,6,8,5,12,11,6,7~7,3,7,10,11,7,7,6,5,10,6,10,3,5,8,11,9,6,9,12,6,8,7,7,7,7,9,6,12,7,4,7,5,7,8,9,7,4,10,11,12,4,7,9,12,11,8,7,6,5&reel_set4=7,3,12,7,9,10,12,8,7,4,9,4,8,6,5,7,7,7,7,8,12,6,5,1,11,6,7,9,7,6,10,1,10,11,10,8,12~8,7,11,10,9,7,6,8,11,6,7,9,7,8,7,7,10,12,6,7,7,7,7,11,6,12,3,8,7,5,10,11,8,3,4,7,10,6,5,7,5,12,12~11,10,12,9,5,7,7,11,8,7,4,5,10,11,1,7,7,7,7,6,10,9,8,12,7,4,12,3,7,6,8,6,1,6,9,8,7~6,9,6,4,6,7,7,4,11,12,8,9,8,5,4,7,11,10,8,6,10,3,7,7,7,7,5,12,10,11,7,7,6,9,8,7,6,11,9,7,5,9,7,12,7,7,3,5,8,1,7~7,11,9,7,8,12,10,6,7,9,6,9,5,12,7,7,7,7,6,11,7,8,11,3,7,4,10,4,6,12,5,10,8,7&purInit=[{bet:1000,type:\"default\"},{bet:3500,type:\"default\"}]&reel_set3=7,11,7,5,12,9,1,8,7,11,7,9,3,9,8,12,1,6,7,11,12,11,7,4,10,5,8,7,7,7,7,8,10,12,4,10,12,6,1,10,9,8,12,7,8,9,6,5,7,7,11,10,6,9,11,5,4~11,4,8,10,6,7,5,9,6,10,7,3,9,1,5,8,3,4,8,7,7,12,8,1,6,7,7,7,7,12,6,11,7,9,10,8,11,10,7,7,12,5,6,10,6,11,8,12,4,12,7,5,1,7,9,7,7~7,8,9,11,12,10,7,7,12,7,6,12,10,8,11,7,11,12,11,7,7,7,7,10,9,8,4,7,5,8,5,10,9,3,7,3,6,8,4,6,5,6~7,7,5,8,10,6,8,3,11,6,11,5,7,12,7,7,7,7,10,4,7,11,6,4,9,8,9,5,7,7,9,12,7,6,10~3,1,6,9,7,5,7,12,8,6,9,7,12,11,7,10,9,12,11,7,4,7,7,7,7,10,5,8,11,4,6,9,10,6,3,12,9,4,8,11,5,7,10,6,7,12,7&reel_set6=9,10,4,7,7,9,11,6,12,6,8,7,7,10,6,8,5,7,7,10,8,10,4,11,1,8,11,12,7,7,7,7,11,7,12,6,7,6,5,3,10,5,7,11,10,6,9,12,5,8,1,10,6,12,8,11,4,12,1,9,7,9,1~3,7,10,9,8,12,11,10,7,4,5,8,5,10,7,9,4,5,8,10,7,6,7,10,3,7,7,7,7,12,7,9,6,11,12,7,4,8,5,11,6,11,9,7,8,12,7,7,12,6,8,6,7~9,8,6,7,11,6,8,4,10,7,12,4,7,9,6,12,8,12,11,6,3,7,3,8,5,12,7,7,7,7,8,11,9,5,7,5,11,10,7,10,4,7,12,6,9,7,5,8,7,8,11,10,7,7,6,9~10,1,6,9,11,7,7,12,7,3,12,4,11,6,4,7,7,7,7,8,5,7,1,8,6,5,12,1,7,8,11,6,10,9,7,9~11,12,7,4,6,7,12,9,8,11,9,5,7,9,6,7,7,7,7,5,10,8,11,4,5,7,3,10,8,7,12,10,6,7,6,1,7,9&reel_set5=10,1,10,12,1,9,12,3,5,6,7,1,8,10,8,7,4,9,7,7,7,7,11,6,7,11,5,7,7,4,10,8,11,9,8,6,11,7,6,12,9,5,6,7~6,4,7,10,6,7,12,7,9,11,4,7,8,5,7,3,11,7,7,9,8,12,11,7,7,7,7,11,6,11,7,7,10,6,8,10,5,8,5,7,12,6,12,7,12,7,10,4,12,10,9,8,9,8,3~11,1,5,9,7,3,7,6,7,9,12,6,8,11,4,8,6,11,9,10,12,8,12,5,7,6,9,12,7,7,7,7,6,10,7,12,8,3,1,12,6,1,10,9,10,7,7,10,5,11,7,5,7,7,6,10,1,11,8,4,8,4,8~12,10,6,9,7,11,9,7,9,10,12,6,7,12,5,11,9,7,8,5,3,7,7,7,7,9,8,7,7,11,12,10,7,6,7,6,4,3,8,10,11,4,5,6,11,4,6,8,7,5~11,7,10,5,12,8,3,9,8,7,10,9,5,9,1,7,7,7,7,12,7,11,7,11,4,6,8,7,7,10,12,6,4,7,5,6&reel_set8=10,11,8,5,10,8,11,6,12,10,7,7,9,4,12,7,7,7,7,6,9,12,10,11,5,7,7,3,5,8,11,9,8,6,7,6~11,9,7,12,5,11,4,8,9,7,11,8,11,12,1,7,10,7,1,4,12,6,7,7,7,7,4,9,10,6,8,3,1,7,6,12,8,7,7,10,8,3,5,6,7,5,6,10,7,12~8,7,9,12,6,3,9,7,4,8,5,8,1,11,6,7,5,7,7,7,7,11,9,7,1,10,7,11,8,10,3,6,12,6,7,9,5,10,12,4,10~8,6,11,12,11,4,5,7,9,3,11,7,10,9,12,7,7,7,7,4,7,6,12,11,8,6,7,7,8,7,12,7,6,7,6,10,3,8,5,9~12,6,11,7,7,8,10,9,5,4,8,11,5,6,5,7,7,7,7,9,7,7,8,6,12,1,11,7,3,10,12,10,4,9,7,7,6&reel_set7=8,11,6,11,9,6,7,4,7,12,10,9,4,7,7,12,10,5,7,5,7,10,7,7,7,7,11,6,8,10,9,11,5,4,7,9,3,6,10,5,12,7,12,6,12,8,7,8,9~7,5,10,4,9,6,12,6,7,7,9,7,11,3,12,5,1,6,8,10,7,7,7,7,8,10,5,6,8,4,6,1,11,7,9,11,3,7,11,12,8,10,7,1,12,7~7,12,8,7,11,6,12,5,9,3,1,8,7,6,10,7,6,8,6,10,7,7,7,7,9,7,11,4,12,10,7,7,5,8,7,4,10,5,9,1,12,11,3,6,8,9~6,7,5,8,6,7,4,6,12,8,12,4,7,12,4,5,10,7,7,1,11,3,7,3,7,11,8,7,7,7,7,11,12,11,5,9,7,7,10,8,7,12,7,8,12,9,6,7,6,9,5,11,9,10,8,9,11,9,10,6~12,3,6,7,6,4,7,8,10,6,11,7,5,10,8,3,6,4,7,9,5,11,7,7,7,7,9,11,12,7,4,8,9,7,10,7,9,10,6,12,6,10,8,12,5,11,7,11,12,7,5&reel_set9=10,6,12,10,5,8,7,5,7,7,12,10,6,11,9,6,9,7,4,12,8,7,7,7,7,4,12,9,8,6,9,7,5,11,6,10,7,11,6,8,11,10,8,3,12,11,9~1,7,5,11,7,1,9,7,10,8,10,5,6,7,12,1,3,7,11,7,7,7,7,4,12,3,7,4,9,6,11,8,6,8,6,7,9,12,6,12,10,8,7,5~9,8,7,9,8,7,4,12,9,5,4,11,7,5,12,6,7,7,7,7,10,11,10,8,6,3,7,12,6,7,9,8,11,7,6,10,12~10,3,9,6,5,7,7,11,12,8,12,7,12,1,9,4,8,11,10,1,9,8,7,9,7,7,7,7,12,11,3,6,5,9,7,4,12,8,6,1,7,7,10,1,5,7,8,1,10,7,11,7,11,6,4,6~12,8,9,4,12,4,6,12,9,7,9,1,8,7,6,11,4,11,7,7,8,5,7,7,7,7,5,12,6,11,6,3,10,6,10,5,10,3,7,11,8,6,10,7,9,12,7,7,9,5&total_bet_min=200.00";
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
        protected int FreePurCount
        {
            get { return 2; }
        }
        public double[] PurchaseMultiples
        {
            get { return new double[] { 100, 350 }; }
        }
        #endregion
        public BiggerBassSplashGameLogic()
        {
            _gameID     = GAMEID.BiggerBassSplash;
            GameName    = "BiggerBassSplash";
        }
        protected override void supplementInitResult(Dictionary<string, string> dicParams, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {
            base.supplementInitResult(dicParams, betInfo, spinResult);
            if (!dicParams.ContainsKey("reel_set"))
                dicParams["reel_set"] = "0";
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in spinParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "fs", "fswin", "fsres", "fsmax", "fsmul", "trail", "reel_set", "g" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!bonusParams.ContainsKey(toCopyParams[i]))
                    continue;
                resultParams[toCopyParams[i]] = bonusParams[toCopyParams[i]];
            }

            string[] toRemoveParams = new string[] { "mo", "mo_t" };
            for (int i = 0; i < toRemoveParams.Length; i++)
            {
                if (resultParams.ContainsKey(toRemoveParams[i]))
                    resultParams.Remove(toRemoveParams[i]);
            }

            return resultParams;
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
            if (dicParams.ContainsKey("mo_tw"))
                dicParams["mo_tw"] = convertWinByBet(dicParams["mo_tw"], currentBet);

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


                BiggerBassSplashBetInfo betInfo = new BiggerBassSplashBetInfo();
                betInfo.BetPerLine  = (float)message.Pop();
                betInfo.LineCount   = (int)message.Pop();

                int bl = (int)message.Pop();
                if (bl == 0)
                    betInfo.MoreBet = false;
                else
                    betInfo.MoreBet = true;

                if (message.DataNum >= 3)
                {
                    betInfo.PurchaseFree = true;
                    betInfo.PurchaseType = (int)message.GetData(2);
                }
                else
                    betInfo.PurchaseFree = false;

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in BiggerBassSplashGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                    (oldBetInfo as BiggerBassSplashBetInfo).PurchaseType = betInfo.PurchaseType;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BiggerBassSplashGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            BiggerBassSplashBetInfo betInfo = new BiggerBassSplashBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new BiggerBassSplashBetInfo();
        }
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as BiggerBassSplashBetInfo).PurchaseType;
            return this.PurchaseMultiples[purchaseType];
        }
        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBet = (double)infoDocument["defaultbet"];
                _normalMaxID        = (int)infoDocument["normalmaxid"];
                _emptySpinCount     = (int)infoDocument["emptycount"];
                _naturalSpinCount   = (int)infoDocument["normalselectcount"];

                _multiTotalFreeSpinWinRates = new double[FreePurCount];
                _multiMinFreeSpinWinRates   = new double[FreePurCount];

                if (SupportPurchaseFree)
                {
                    var purchaseOdds = infoDocument["purchaseodds"] as BsonArray;
                    for (int i = 0; i < FreePurCount; i++)
                    {
                        _multiMinFreeSpinWinRates[i] = (double)purchaseOdds[2 * i];
                        _multiTotalFreeSpinWinRates[i] = (double)purchaseOdds[2 * i + 1];

                        if (this.PurchaseMultiples[i] > _multiTotalFreeSpinWinRates[i])
                            _logger.Error("freespin win rate doesn't satisfy condition {0}", this.GameName);
                    }
                }

                if (this.SupportMoreBet)
                {
                    _anteBetMinusZeroCount = (int)((1.0 - 1.0 / MoreBetMultiple) * (double)_naturalSpinCount);
                    if (_anteBetMinusZeroCount > _emptySpinCount)
                        _logger.Error("More Bet Probabily calculation doesn't work in {0}", this.GameName);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
            }
        }
        protected override async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int agentID, BasePPSlotBetInfo betInfo, double baseBet, UserBonus userBonus, bool isAffiliate)
        {
            int purchaseType = (betInfo as BiggerBassSplashBetInfo).PurchaseType;
            double payoutRate = getPayoutRate(agentID, isAffiliate);

            double targetC = PurchaseMultiples[purchaseType] * payoutRate / 100.0;
            if (targetC >= _multiTotalFreeSpinWinRates[purchaseType])
                targetC = _multiTotalFreeSpinWinRates[purchaseType];

            if (targetC < _multiMinFreeSpinWinRates[purchaseType])
                targetC = _multiMinFreeSpinWinRates[purchaseType];

            double x = (_multiTotalFreeSpinWinRates[purchaseType] - targetC) / (_multiTotalFreeSpinWinRates[purchaseType] - _multiMinFreeSpinWinRates[purchaseType]);
            double y = 1.0 - x;

            BasePPSlotSpinData spinData = null;
            if (Pcg.Default.NextDouble(0.0, 1.0) <= x)
                spinData = await selectMinStartFreeSpinData(betInfo);
            else
                spinData = await selectRandomStartFreeSpinData(betInfo);
            return spinData;
        }
        protected override async Task<BasePPSlotSpinData> selectMinStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                BsonDocument spinDataDocument = null;
                int purchaseType = (betInfo as BiggerBassSplashBetInfo).PurchaseType;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, -1, PurchaseMultiples[purchaseType] * 0.2, PurchaseMultiples[purchaseType] * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BiggerBassSplashGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int purchaseType = (betInfo as BiggerBassSplashBetInfo).PurchaseType;
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BiggerBassSplashGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = (betInfo as BiggerBassSplashBetInfo).PurchaseType.ToString();
        }
        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            BiggerBassSplashBetInfo starBetInfo = betInfo as BiggerBassSplashBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, -1, betInfo.PurchaseFree ? starBetInfo.PurchaseType : -1, betMoney);
        }
    }
}
