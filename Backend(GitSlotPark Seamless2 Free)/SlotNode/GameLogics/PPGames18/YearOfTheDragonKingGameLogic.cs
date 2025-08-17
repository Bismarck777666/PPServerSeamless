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
    class YearOfTheDragonKingBetInfo : BasePPSlotBetInfo
    {
        public int PurchaseType { get; set; }
        public override float TotalBet
        {
            get { return this.BetPerLine * 10.0f; }
        }
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
    class YearOfTheDragonKingGameLogic : BasePPSlotGame
    {
        protected double[] _multiTotalFreeSpinWinRates;
        protected double[] _multiMinFreeSpinWinRates;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20yotdk";
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
                return "def_s=3,7,5,5,6,4,6,3,2,8,5,9,8,4,9&cfgs=9384&ver=3&def_sb=5,8,9,2,7&reel_set_size=10&def_sa=3,8,2,3,9&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"9233610\",max_rnd_win:\"5000\",max_rnd_win_a:\"3334\",max_rnd_hr_a:\"5060729\"}}&wl_i=tbm~5000;tbm_a~3334&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&purInit_e=1,1,1,1&wilds=2~250,100,50,0,0~1,1,1,1,1;13~10,0,0~1,1,1;14~10,0,0~1,1,1;15~10,0,0~1,1,1&bonuses=0&bls=10,15&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;250,100,50,0,0;100,50,30,0,0;50,30,20,0,0;30,20,10,0,0;30,20,10,0,0;20,10,5,0,0;20,10,5,0,0;200,0,0;50,0,0;20,0,0;0,0,0;0,0,0;0,0,0;0,0,0&total_bet_max=35,000,000.00&reel_set0=9,8,6,7,9,3,4,7,5,6,9,6,8,4,4,4,7,5,7,3,7,9,7,7,4,9,6,9,6,5,5,5,7,6,9,9,7,6,3,7,4,5,7,6,7,5,6,6,6,9,6,4,6,5,5,6,4,9,4,7,5,9,6,7,7,7,4,5,7,5,4,9,4,9,4,9,9,6,6,4,9,9,9,5,5,9,8,7,4,6,8,6,6,7,4,9,5,7,8~9,7,8,5,8,3,7,8,9,8,6,9,8,8,3,3,7,3,7,9,3,3,3,9,7,8,7,8,5,5,9,7,9,8,5,7,5,9,5,9,5,7,7,8,9,5,5,5,9,5,9,6,8,4,8,8,9,3,5,8,9,8,9,7,8,7,3,9,9,7,7,7,8,7,9,8,8,9,8,8,7,8,8,9,8,7,7,8,4,5,8,9,8,9,8,8,8,9,8,3,9,3,9,8,3,9,9,3,8,9,9,5,3,9,8,8,7,5,9,9,9,5,9,8,7,9,8,5,8,9,9,8,9,9,8,6,8,9,7,7,8,9,9,3~4,3,6,3,3,8,8,6,7,9,7,4,6,7,3,3,3,6,6,8,6,4,6,8,4,4,9,8,6,5,4,9,4,4,4,6,4,6,3,8,3,7,6,3,6,2,3,8,5,6,6,6,4,4,8,8,4,8,3,8,6,3,6,3,8,6,6,8,8,8,3,3,8,6,8,4,9,8,8,4,9,4,8,8,6,8,6~8,3,3,7,7,8,7,8,2,2,9,2,2,2,4,5,3,3,8,5,4,8,7,3,7,3,3,3,6,7,5,9,4,3,2,3,3,8,5,2,7,4,4,4,3,4,9,5,6,5,3,9,4,5,6,5,5,5,7,4,3,5,5,7,6,8,3,6,6,5,8,6,6,6,8,6,9,3,9,7,4,5,9,3,9,7,7,7,4,3,9,8,6,4,9,5,3,6,4,6,7,8,8,8,4,3,9,4,9,5,7,6,8,9,8,9,9,9,6,6,2,3,8,4,6,3,9,5,2,7,3,3~4,4,3,2,3,9,9,3,7,8,9,3,2,2,2,9,4,3,3,5,6,5,8,5,4,5,3,7,3,3,3,4,8,3,5,6,6,8,9,3,6,5,7,9,3,4,4,4,9,5,8,3,2,4,5,8,9,3,7,7,5,5,5,9,7,2,3,6,9,7,9,8,8,6,4,7,6,6,6,7,3,4,4,6,8,6,8,5,5,4,5,5,3,7,7,7,6,7,6,5,4,3,6,2,7,3,4,3,9,8,8,8,2,7,8,3,6,8,7,7,4,6,2,9,6,9,9,9,3,9,8,3,2,9,4,5,5,7,6,3,8,2,8&reel_set2=7,3,7,3,8,6,8,9,9,8,9,7,3,7,5,5,8,9,3,5,9,3,3,3,8,8,9,8,9,8,7,8,9,3,9,8,9,8,8,7,5,8,9,3,7,8,5,5,5,8,5,9,7,9,7,8,9,5,8,8,5,9,9,4,9,6,8,8,9,4,9,7,7,7,3,8,5,3,9,3,9,8,8,7,8,5,8,7,5,7,9,9,3,9,9,8,8,8,7,9,8,7,7,8,8,9,9,3,8,9,9,7,9,9,8,5,8,5,6,9,9,9,7,3,8,5,8,9,8,7,9,8,9,8,9,8,7,5,9,9,5,8,8,7~6,8,9,8,4,6,2,6,3,4,9,3,3,3,4,8,6,6,8,4,3,6,8,8,5,4,4,4,6,8,6,4,6,7,4,8,8,3,3,6,6,6,7,8,6,6,8,4,3,3,8,6,4,4,8,8,8,5,6,8,8,3,8,6,3,9,6,4,9,7~4,6,7,6,5,5,7,9,6,7,7,9,5,7,4,4,4,7,5,9,4,5,9,6,6,4,9,9,4,3,4,6,9,5,5,5,7,9,9,5,6,6,8,5,8,8,6,4,9,3,6,5,6,6,6,5,7,5,5,7,6,7,5,4,4,9,8,6,7,6,4,7,7,7,9,8,6,9,3,9,5,6,7,9,4,7,9,9,5,7,9,9,9,7,4,4,7,6,7,6,4,9,7,7,6,9,4,4,5,6~6,7,3,3,9,3,5,8,2,5,7,3,4,3,2,2,2,4,5,7,5,8,7,9,4,4,8,8,7,7,9,8,3,3,3,6,6,8,5,2,7,8,6,2,2,3,6,4,7,5,4,4,4,5,8,9,9,3,4,6,8,3,8,3,3,8,6,9,5,5,5,3,9,7,4,5,3,6,7,8,8,7,9,4,7,4,5,6,6,6,7,5,2,3,3,5,3,9,5,3,6,3,7,9,2,7,7,7,6,6,4,6,6,5,9,9,4,5,7,7,3,3,4,8,8,8,4,9,6,4,2,3,4,4,9,4,3,6,7,8,3,9,9,9,3,5,5,3,2,3,8,9,6,5,2,3,9,8,6,8,9~3,3,8,9,6,5,8,3,7,9,5,2,2,2,4,6,8,5,5,3,3,6,4,7,4,4,3,3,3,6,7,8,7,3,8,3,5,9,7,3,3,8,4,4,4,3,6,9,5,5,6,3,8,7,2,9,5,5,5,9,4,7,3,4,3,8,9,4,8,9,8,6,6,6,9,2,3,4,7,8,2,2,7,3,2,4,5,7,7,7,5,6,8,3,6,8,7,7,5,4,9,7,8,8,8,4,7,5,6,9,9,6,3,2,6,9,6,9,9,9,6,5,8,7,5,3,5,4,9,3,4,3,2,6&reel_set1=2,8,9,6,4,8,6,6,7,8,6,7,3,3,3,4,5,4,8,3,9,6,4,9,3,5,8,6,4,4,4,6,8,3,8,8,6,4,7,3,4,4,6,4,9,6,6,6,3,6,8,3,8,8,6,8,4,8,6,6,4,8,8,8,7,8,3,3,6,4,8,8,6,4,6,3,8,3,6~5,6,5,6,7,6,9,7,3,4,8,9,4,7,4,4,4,9,5,7,3,4,7,5,6,6,7,9,6,7,9,5,5,5,9,6,4,5,6,9,9,7,6,9,4,6,3,9,7,6,6,6,5,6,5,9,6,5,5,7,6,4,5,4,9,4,6,5,7,7,7,9,7,4,9,4,7,6,6,8,7,7,5,9,6,9,9,9,4,6,8,7,5,7,8,9,6,5,9,9,7,9,7,8,4~9,8,8,9,8,5,6,3,8,5,8,3,3,3,7,9,6,9,8,9,8,9,8,5,7,3,8,5,5,5,9,8,9,7,9,9,7,9,7,9,7,9,7,7,7,8,9,7,8,9,5,9,7,9,8,8,9,8,8,8,7,8,5,3,9,3,7,8,8,7,5,3,9,9,9,8,4,9,3,8,3,5,9,8,9,9,5,8,5~9,9,6,6,5,3,2,3,6,3,7,7,2,2,2,4,3,6,2,3,3,9,3,2,3,4,7,6,3,3,3,8,7,9,4,4,2,7,6,3,6,6,5,5,4,4,4,9,4,9,3,3,7,9,3,7,6,8,9,7,5,5,5,7,5,5,8,8,5,9,9,8,7,4,5,4,6,6,6,9,5,4,2,3,8,3,5,3,7,5,3,9,5,7,7,7,3,6,4,6,8,4,4,2,2,8,6,6,8,8,8,5,5,7,3,8,2,8,7,3,9,7,5,9,9,9,3,3,8,8,9,4,7,5,9,4,6,4,6,4,8~3,4,3,2,7,9,8,2,2,2,3,5,4,9,8,9,5,7,7,3,3,3,9,8,4,3,9,6,5,8,3,4,4,4,8,7,5,7,3,2,5,6,8,5,5,5,2,2,3,3,6,3,3,6,4,6,6,6,3,4,8,8,6,6,8,6,8,7,7,7,9,7,6,6,4,6,3,4,5,8,8,8,4,5,3,7,7,9,9,5,3,9,9,9,2,9,5,5,9,7,7,3,4,4&reel_set4=5,7,5,4,9,4,7,4,5,4,5,9,4,4,4,9,7,4,7,4,9,4,7,4,9,4,9,4,5~4,6,4,3,4,8,4,3,4,4,4,8,6,4,4,6,3,4,4,8~4,6,2,9,2,9,3,8,5,6,4,8,4,4,4,3,4,4,7,6,3,9,7,5,4,4,8,4,5,7~9,6,8,4,4,8,5,3,4,4,6,4,4,4,5,6,4,7,3,4,2,3,7,2,9,5,9,4~5,4,6,7,4,6,9,5,3,6,3,4,7,3,8,4,4,8,2,4,7,4,4,4,8,2,7,6,4,3,4,3,4,9,4,2,5,5,9,8,4,9,9,5,8,4&purInit=[{bet:1000},{bet:600},{bet:1400},{bet:3500}]&reel_set3=3,7,3,7,3,9,3,5,3,3,9,5,9,3,3,3,5,3,5,3,5,3,7,3,9,3,3,9,7,5,9~4,3,8,6,3,3,8,4,8,4,3,3,3,6,3,3,4,3,6,3,8,3,3,6,3,3~5,3,4,2,3,8,6,3,6,7,4,8,3,3,3,4,3,9,7,3,9,3,3,7,5,8,5,2,6~4,9,3,2,7,3,3,8,7,6,3,8,3,5,6,3,3,3,6,7,5,9,4,8,5,6,4,8,2,3,3,9,7,4,5~5,3,3,5,2,3,8,7,3,8,4,3,6,5,4,6,3,3,3,6,4,9,7,3,6,9,5,8,3,3,4,9,7,8,7,2,9,3&reel_set6=6,7,9,6,5,9,6,5,6,9,5,6,6,6,7,6,7,9,7,5,6,9,7,6,7,6,6,5,6,9~3,8,3,6,6,4,6,6,3,6,8,6,6,6,8,4,6,6,3,4,6,4,6,6~4,8,7,4,6,5,3,6,8,3,9,3,5,2,4,3,2,6,6,6,5,6,2,7,8,6,6,9,6,6,4,9,5,9,6,6,8,7~6,4,6,9,7,8,2,4,8,9,2,5,9,7,6,3,6,6,6,3,7,9,8,3,4,7,6,5,6,6,5,6,8,4,6,5,3~6,6,4,6,4,3,9,8,9,6,5,7,8,2,5,3,9,6,5,6,6,6,7,3,6,7,8,4,6,5,6,6,3,9,6,6,3,8,7,2,8,5,9,2&reel_set5=7,5,5,9,3,5,5,3,5,3,9,5,5,5,3,5,7,9,7,3,9,5,9,5,5,7,5~6,5,8,5,5,4,5,4,5,5,5,4,6,5,8,5,8,5,5,6~3,9,8,5,2,2,4,5,3,5,5,5,7,5,6,5,5,7,4,8,9,6,5~5,3,5,5,9,6,7,8,6,9,8,2,5,7,6,4,5,5,5,4,5,3,7,4,9,2,3,8,9,5,6,5,8,5,5,4~5,9,2,5,9,5,8,5,8,6,7,6,5,9,5,9,3,9,4,2,5,5,5,8,3,4,7,4,3,8,6,5,4,7,2,8,5,3,5,5,6,4,3,6,7&reel_set8=8,5,8,9,8,8,9,8,7,8,8,8,5,9,7,5,7,8,9,8,7~8,4,8,3,4,8,3,8,8,4,6,8,8,8,3,8,8,6,8,4,3,8,8,6,8,6,8~5,4,2,7,8,6,7,9,3,8,4,8,8,8,3,8,6,8,4,2,5,3,9,5,8,8,6,8,9~7,9,3,4,5,8,7,9,7,8,7,8,6,4,6,2,3,5,8,5,8,8,8,2,5,6,8,4,3,8,8,2,5,6,9,3,4,8,9,4,9,6,8,7,8,8~4,6,5,5,4,3,6,5,8,9,3,4,8,3,2,4,7,3,8,8,8,3,7,5,8,7,2,8,8,2,7,9,9,8,9,8,7,6,8,5,8,9,8&reel_set7=5,7,5,7,9,7,7,9,3,5,7,7,7,5,3,9,7,7,5,9,7,3,7,3,9~4,6,7,7,6,8,7,7,7,8,4,7,7,6,7,7,8,4,7,7~2,3,9,2,4,8,7,6,5,7,4,2,7,6,7,9,7,7,7,9,3,7,7,5,7,8,9,5,4,8,7,3,7,3,7,6,5,8,7~6,2,7,3,7,3,5,3,5,7,4,6,7,6,9,4,2,9,5,6,9,7,7,7,8,9,7,5,3,7,4,9,8,7,2,7,7,4,7,8,7,3,4,5,8,7,6~4,7,7,6,9,3,2,5,7,6,3,7,9,3,4,5,7,7,7,8,2,8,6,4,5,3,8,9,9,6,7,4,7,5&reel_set9=7,5,9,5,3,7,9,7,9,9,9,3,7,9,3,9,9,5~9,6,8,9,6,9,8,9,9,9,4,9,4,9,8,9,9,6,9,9,4~4,8,9,4,3,5,9,6,4,2,9,2,9,9,9,7,5,9,9,6,7,2,9,3,9,8,7,8,5~8,5,6,8,3,8,9,9,3,4,9,3,9,9,8,9,6,9,2,7,3,9,9,9,7,2,6,5,4,8,9,5,7,2,6,7,4,6,5,9,3,9,7,4,9~9,3,9,4,7,6,9,5,9,9,8,9,4,5,2,6,3,4,8,5,9,9,9,3,7,2,7,9,3,9,9,6,7,8,4,8,5,4,8,9,3,2,5,7&total_bet_min=20.00";
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
            get { return 4; }
        }
        public double[] PurchaseMultiples
        {
            get { return new double[] { 100, 60, 140, 350 }; }
        }
        #endregion
        public YearOfTheDragonKingGameLogic()
        {
            _gameID = GAMEID.YearOfTheDragonKing;
            GameName = "YearOfTheDragonKing";
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
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["g"] = "{reel_0:{def_s:\"10,11,16\",def_sa:\"10,11,16\",def_sb:\"10,11,16\",s:\"10,11,16\",sa:\"10,11,16\",sb:\"10,11,16\",sh:\"1\",st:\"rect\",sw:\"3\"},reel_1:{def_s:\"10,11,16\",def_sa:\"10,11,16\",def_sb:\"10,11,16\",s:\"10,11,16\",sa:\"10,11,16\",sb:\"10,11,16\",sh:\"1\",st:\"rect\",sw:\"3\"},reel_2:{def_s:\"10,11,16\",def_sa:\"10,11,16\",def_sb:\"10,11,16\",s:\"10,11,16\",sa:\"10,11,16\",sb:\"10,11,16\",sh:\"1\",st:\"rect\",sw:\"3\"},reel_3:{def_s:\"10,11,16\",def_sa:\"10,11,16\",def_sb:\"10,11,16\",s:\"10,11,16\",sa:\"10,11,16\",sb:\"10,11,16\",sh:\"1\",st:\"rect\",sw:\"3\"},reel_4:{def_s:\"10,11,16\",def_sa:\"10,11,16\",def_sb:\"10,11,16\",s:\"10,11,16\",sa:\"10,11,16\",sb:\"10,11,16\",sh:\"1\",st:\"rect\",sw:\"3\"}}";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
	        dicParams["bl"] = "0";
        }
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as YearOfTheDragonKingBetInfo).PurchaseType;
            return this.PurchaseMultiples[purchaseType];
        }

        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if(dicParams.ContainsKey("g"))
            {
                var gParam = JToken.Parse(dicParams["g"]);
                for(int i = 0; i < 5; i++)
                {
                    var key = string.Format("reel_{0}", i);
                    if (gParam[key] != null)
                    {
                        int winLineID = 0;
                        do
                        {
                            string strKey = string.Format("l{0}", winLineID);
                            if (gParam[key][strKey] == null)
                                break;

                            string[] strParts = gParam[key][strKey].ToString().Split(new string[] { "~" }, StringSplitOptions.None);
                            if (strParts.Length >= 2)
                            {
                                strParts[1] = convertWinByBet(strParts[1], currentBet);
                                gParam[key][strKey] = string.Join("~", strParts);
                            }
                            winLineID++;
                        } while (true);
                    }
                }
                dicParams["g"] = serializeJsonSpecial(gParam);
            }
            if (dicParams.ContainsKey("rs_win"))
                dicParams["rs_win"] = convertWinByBet(dicParams["rs_win"], currentBet);

            if(dicParams.ContainsKey("trail"))
            {
                string[] strParts = dicParams["trail"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for(int i = 0; i < strParts.Length; i++)
                {
                    string[] strSubParts = strParts[i].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                    if (strSubParts[0] != "dk_wins")
                        continue;

                    string[] strValues = strSubParts[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    for(int j = 0; j < strValues.Length;j++)
                        strValues[j] = convertWinByBet(strValues[j], currentBet);

                    strSubParts[1]  = string.Join(",", strValues);
                    strParts[i]     = string.Join("~", strSubParts);
                }
                dicParams["trail"] = string.Join(";", strParts);
            }
        }

        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            YearOfTheDragonKingBetInfo betInfo = new YearOfTheDragonKingBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new YearOfTheDragonKingBetInfo();
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


                YearOfTheDragonKingBetInfo betInfo  = new YearOfTheDragonKingBetInfo();
                betInfo.BetPerLine                  = (float)message.Pop();
                betInfo.LineCount                   = (int)message.Pop();
		
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in YearOfTheDragonKingGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in YearOfTheDragonKingGameLogic::readBetInfoFromMessage", strGlobalUserID);
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
                    (oldBetInfo as YearOfTheDragonKingBetInfo).PurchaseType = betInfo.PurchaseType;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in YearOfTheDragonKingGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int agentID, BasePPSlotBetInfo betInfo, double baseBet, UserBonus userBonus, bool isAffiliate)
        {
            int purchaseType = (betInfo as YearOfTheDragonKingBetInfo).PurchaseType;
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
                int purchaseType = (betInfo as YearOfTheDragonKingBetInfo).PurchaseType;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, -1, PurchaseMultiples[purchaseType] * 0.2, PurchaseMultiples[purchaseType] * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in YearOfTheDragonKingGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int purchaseType = (betInfo as YearOfTheDragonKingBetInfo).PurchaseType;
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in YearOfTheDragonKingGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }

        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            YearOfTheDragonKingBetInfo yearBetInfo = betInfo as YearOfTheDragonKingBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, betInfo.MoreBet ? 1 : 0, betInfo.PurchaseFree ? yearBetInfo.PurchaseType : -1, betMoney);
        }

    }
}
