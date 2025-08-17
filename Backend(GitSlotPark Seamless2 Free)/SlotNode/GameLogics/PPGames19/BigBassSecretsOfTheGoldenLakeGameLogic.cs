using Akka.Actor;
using GITProtocol;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
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
    class BigBassSecretsOfTheGoldenLakeBetInfo : BasePPSlotBetInfo
    {
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
    class BigBassSecretsOfTheGoldenLakeGameLogic : BasePPSlotGame
    {
        protected double[] _multiTotalFreeSpinWinRates;
        protected double[] _multiMinFreeSpinWinRates;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10bblotgl";
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
                return "def_s=1,12,3,3,10,5,11,12,5,11,3,7,11,6,8&cfgs=9982&ver=3&def_sb=3,8,7,4,10&reel_set_size=14&def_sa=6,7,11,6,12&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"3216468\",max_rnd_win:\"5000\",max_rnd_win_a:\"3334\",max_rnd_hr_a:\"1885370\"}}&wl_i=tbm~5000;tbm_a~3334&reel_set10=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&reel_set11=12,6,7,8,10,2,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,2,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,2,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,2,12,9,10,4,3,11,5,2,9,8,7,7,7,7,7~6,12,2,3,6,7,11,3,5,7,10,4,3,5,2,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,2,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,2,12,5,2,8,7,6,4,11,3,9,7,7,10,6,3,5,2,7,7,7,7,7~5,4,6,7,11,2,4,8,2,7,5,3,12,7,7,10,2,6,9,7,7,7,7,7&reel_set12=7,12,8,12,9,6,10,4,9,12,11,8,7,7,7,10,8,11,12,6,10,8,10,7,8,5,4,3,7~11,5,4,11,7,10,7,12,9,7,11,12,11,7,7,3,7,7,7,10,7,8,9,7,9,11,9,11,7,7,3,5,6,8,9,11,9~11,6,7,7,4,5,7,6,5,4,8,7,11,12,7,7,7,6,10,8,7,10,5,7,7,4,6,9,12,4,3~5,10,7,11,5,4,8,3,7,7,7,12,7,6,4,3,7,6,7,7,9~11,5,7,12,9,7,8,7,11,12,7,7,7,4,10,5,7,9,3,6,10,6,8,7&purInit_e=1,1&reel_set13=9,4,9,6,7,8,10,7,7,12,11,9,6,7,11,6,9,7,5,11,5,4,8,6,4,7,7,7,7,7,7,6,4,10,11,3,10,8,5,7,6,7,8,3,7,9,11,12,8,7,7,6,9,10,8,10,7,12,12~10,8,4,7,7,11,7,4,10,4,11,9,6,8,6,7,7,7,7,7,7,12,7,8,9,7,12,6,7,9,6,11,5,3,12,10,5,8,5~12,5,7,12,7,9,5,10,8,3,8,7,6,8,12,10,6,10,6,9,7,8,11,4,7,7,7,7,7,7,6,3,7,7,11,8,4,9,4,12,5,10,6,8,9,7,10,9,11,7,6,4,7,10,9,7,11,7,11~11,5,3,7,5,10,9,7,7,4,7,7,12,6,4,8,12,7,7,7,7,7,7,11,6,11,8,3,6,11,9,12,7,10,7,10,9,7,6,9,8,6,8,4~8,12,10,7,9,10,8,11,4,6,7,10,4,7,6,7,7,7,7,7,7,4,7,5,11,6,12,3,5,7,5,7,8,12,9,11,7,6,9&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&bls=10,15&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;2000,200,50,5,0;1000,150,30,0,0;500,100,20,0,0;500,100,20,0,0;200,50,10,0,0;100,25,2,0,0;100,25,2,0,0;50,10,2,0,0;50,10,2,0,0;50,10,2,0,0;0,0,0,0,0&total_bet_max=27,000,000.00&reel_set0=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&accInit=[{id:0,mask:\"cp;tp;lvl;sc;cl\"}]&reel_set2=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&reel_set1=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&reel_set4=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&purInit=[{bet:1000},{bet:2700}]&reel_set3=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&reel_set6=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&reel_set5=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&reel_set8=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&reel_set7=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&reel_set9=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,10,4,3,5,1,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,6,4,11,3,9,7,7,10,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,7,5,3,12,1,7,7,10,1,6,9,7,7,7,7,7&total_bet_min=20.00";
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
            get { return new double[] { 100, 270 }; }
        }

        #endregion
        public BigBassSecretsOfTheGoldenLakeGameLogic()
        {
            _gameID = GAMEID.BigBassSecretsOfTheGoldenLake;
            GameName = "BigBassSecretsOfTheGoldenLake";
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
            if (dicParams.ContainsKey("mo_tw"))
                dicParams["mo_tw"] = convertWinByBet(dicParams["mo_tw"], currentBet);
        }
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as BigBassSecretsOfTheGoldenLakeBetInfo).PurchaseType;
            return this.PurchaseMultiples[purchaseType];
        }
        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBet         = (double)infoDocument["defaultbet"];
                _normalMaxID                = (int)infoDocument["normalmaxid"];
                _emptySpinCount             = (int)infoDocument["emptycount"];
                _naturalSpinCount           = (int)infoDocument["normalselectcount"];

                _multiTotalFreeSpinWinRates = new double[FreePurCount];
                _multiMinFreeSpinWinRates   = new double[FreePurCount];

                if (SupportPurchaseFree)
                {
                    var purchaseOdds = infoDocument["purchaseodds"] as BsonArray;
                    for (int i = 0; i < FreePurCount; i++)
                    {
                        _multiMinFreeSpinWinRates[i]    = (double)purchaseOdds[2 * i];
                        _multiTotalFreeSpinWinRates[i]  = (double)purchaseOdds[2 * i + 1];

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
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            BigBassSecretsOfTheGoldenLakeBetInfo betInfo = new BigBassSecretsOfTheGoldenLakeBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new BigBassSecretsOfTheGoldenLakeBetInfo();
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


                BigBassSecretsOfTheGoldenLakeBetInfo betInfo = new BigBassSecretsOfTheGoldenLakeBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();
		
                int bl = (int)message.Pop();
                if (bl == 0)
                    betInfo.MoreBet = false;
                else
                    betInfo.MoreBet = true;


                if (message.DataNum >= 3)
                {
                    betInfo.PurchaseType = (int)message.GetData(2);
                    betInfo.PurchaseFree = true;
                }
                else
                {
                    betInfo.PurchaseFree = false;
                }

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in BigBassSecretsOfTheGoldenLakeGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in BigBassSecretsOfTheGoldenLakeGameLogic::readBetInfoFromMessage", strGlobalUserID);
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
                    (oldBetInfo as BigBassSecretsOfTheGoldenLakeBetInfo).PurchaseType = betInfo.PurchaseType;

                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BigBassSecretsOfTheGoldenLakeGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int agentID, BasePPSlotBetInfo betInfo, double baseBet, UserBonus userBonus, bool isAffiliate)
        {
            int purchaseType    = (betInfo as BigBassSecretsOfTheGoldenLakeBetInfo).PurchaseType;
            double payoutRate   = getPayoutRate(agentID, isAffiliate);

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
                int purchaseType = (betInfo as BigBassSecretsOfTheGoldenLakeBetInfo).PurchaseType;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, -1, PurchaseMultiples[purchaseType] * 0.2, PurchaseMultiples[purchaseType] * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BigBassSecretsOfTheGoldenLakeGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int purchaseType = (betInfo as BigBassSecretsOfTheGoldenLakeBetInfo).PurchaseType;
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BigBassSecretsOfTheGoldenLakeGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }

        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            BigBassSecretsOfTheGoldenLakeBetInfo bigBetInfo = betInfo as BigBassSecretsOfTheGoldenLakeBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, betInfo.MoreBet ? 1 : 0, betInfo.PurchaseFree ? bigBetInfo.PurchaseType : -1, betMoney);
        }

        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set", "s", "bw" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]) || resultParams.ContainsKey(toCopyParams[i]))
                    continue;
                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }
            return resultParams;
        }

        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency, bool isAffiliate)
        {
            try
            {
                int index = (int)message.Pop();
                int counter = (int)message.Pop();
                int ind = -1;
                if (message.DataNum >= 1)
                    ind = (int)message.Pop();

                string strGlobalUserID      = string.Format("{0}_{1}", agentID, strUserID);
                GITMessage responseMessage  = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                double realWin              = 0.0;
                string strGameLog           = "";
                ToUserResultMessage resultMsg = null;
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) ||
                    !_dicUserBetInfos[strGlobalUserID].HasRemainResponse || ind < 0 || ind > 11)
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    var                         result          = _dicUserResultInfos[strGlobalUserID];
                    BasePPSlotBetInfo           betInfo         = _dicUserBetInfos[strGlobalUserID];
                    BasePPActionToResponse      actionResponse  = betInfo.pullRemainResponse();
                    if (actionResponse.ActionType != ActionTypes.DOBONUS)
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        Dictionary<string, string> dicParams = splitResponseToParams(actionResponse.Response);

                        convertWinsByBet(dicParams, betInfo.TotalBet);
                        convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                        if (dicParams.ContainsKey("g") && ind != 0)
                        {
                            var gParam = JToken.Parse(dicParams["g"]);
                            var status = gParam["bg_0"]["status"].ToString();

                            string[] strParts = status.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            string strTemp  = "";
                            strTemp         = strParts[0];
                            strParts[0]     = strParts[ind];
                            strParts[ind]   = strTemp;
                            gParam["bg_0"]["status"] = string.Join(",", strParts);

                            var winsMask = gParam["bg_0"]["wins_mask"].ToString();
                            strParts = winsMask.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            strTemp = strParts[0];
                            strParts[0] = strParts[ind];
                            strParts[ind] = strTemp;
                            gParam["bg_0"]["wins_mask"] = string.Join(",", strParts);

                            gParam["bg_0"]["ch_h"] = string.Format("0~{0}", ind);
                            dicParams["g"] = serializeJsonSpecial(gParam);
                        }
                        result.BonusResultString = convertKeyValuesToString(dicParams);
                        addDefaultParams(dicParams, userBalance, index, counter);
                        ActionTypes nextAction = convertStringToActionType(dicParams["na"]);
                        string strResponse = convertKeyValuesToString(dicParams);
                        responseMessage.Append(strResponse);

                        //히스토리보관 및 초기화
                        if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                            addIndActionHistory(strGlobalUserID, "doBonus", strResponse, index, counter, ind);

                        if (nextAction == ActionTypes.DOCOLLECT || nextAction == ActionTypes.DOCOLLECTBONUS)
                        {
                            realWin = double.Parse(dicParams["tw"]);
                            strGameLog = strResponse;

                            if (realWin > 0.0f)
                            {
                                _dicUserHistory[strGlobalUserID].baseBet = betInfo.TotalBet;
                                _dicUserHistory[strGlobalUserID].win = realWin;
                            }

                            resultMsg = new ToUserResultMessage((int)_gameID, responseMessage, 0.0, realWin, new GameLogInfo(this.GameName, "0", strGameLog), UserBetTypes.Normal); ;
                            resultMsg.BetTransactionID = betInfo.BetTransactionID;
                            resultMsg.RoundID = betInfo.RoundID;
                            resultMsg.TransactionID = createTransactionID();

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
                _logger.Error("Exception has been occurred in DaVinciTreasure::onDoBonus {0}", ex);
            }
        }

    }
}
