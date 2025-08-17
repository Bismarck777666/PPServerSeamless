using Akka.Actor;
using Akka.Event;
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
    class ReleaseTheKraken2StartSpinData : BasePPSlotStartSpinData
    {
        public string FreeSpinTypes { get; set; }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            this.FreeSpinTypes = reader.ReadString();
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(this.FreeSpinTypes);
        }
    }
    class ReleaseTheKraken2BetInfo : BasePPSlotBetInfo
    {
        public int PurchaseType { get; set; }

        public override void SerializeFrom(BinaryReader reader)
        {
            this.BetPerLine = reader.ReadSingle();
            this.LineCount = reader.ReadInt32();
            this.PurchaseFree = reader.ReadBoolean();
            this.MoreBet = reader.ReadBoolean();
            int remainCount = reader.ReadInt32();
            if (remainCount == 0)
            {
                this.RemainReponses = null;
            }
            else
            {
                this.RemainReponses = new List<BasePPActionToResponse>();
                for (int i = 0; i < remainCount; i++)
                {
                    ActionTypes actionType = (ActionTypes)(byte)reader.ReadByte();
                    string strResponse = (string)reader.ReadString();
                    this.RemainReponses.Add(new BasePPActionToResponse(actionType, strResponse));
                }
            }
            int spinDataType = reader.ReadInt32();
            if (spinDataType == 0)
            {
                this.SpinData = null;
            }
            else if (spinDataType == 1)
            {
                this.SpinData = new BasePPSlotSpinData();
                this.SpinData.SerializeFrom(reader);
            }
            else
            {
                this.SpinData = new ReleaseTheKraken2StartSpinData();
                this.SpinData.SerializeFrom(reader);
            }
            bool hasValue = reader.ReadBoolean();
            if (hasValue)
                this.RoundID = reader.ReadString();

            hasValue = reader.ReadBoolean();
            if (hasValue)
                this.BetTransactionID = reader.ReadString();
            this.PurchaseType = reader.ReadInt32();
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(PurchaseType);
        }
    }
    class ReleaseTheKraken2Result : BasePPSlotSpinResult
    {
        public int          DoBonusCounter      { get; set; }
        public List<int>    BonusSelections     { get; set; }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            this.DoBonusCounter = reader.ReadInt32();
            BonusSelections     = SerializeUtils.readIntList(reader);

        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(this.DoBonusCounter);
            SerializeUtils.writeIntList(writer, BonusSelections);
        }
    }
    class ReleaseTheKraken2GameLogic : BaseSelFreePPSlotGame
    {
        protected double[] _multiTotalFreeSpinWinRates;
        protected double[] _multiMinFreeSpinWinRates;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20kraken2";
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
                return 4;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=4,10,7,10,3,11,5,4,7,7,11,5,10,3,11,4,10,10,8,5&cfgs=6666&ver=3&def_sb=4,9,9,6,7&reel_set_size=9&def_sa=4,10,8,6,10&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{bg_all_prizes:\"f4_m2,f5_m1,f5_m2,f6_m1,f6_m2,f8_m1,f8_m2,f8_m4,f8_m6,f10_m2,f10_m4,f10_m6,f12_m2,f12_m4,f12_m6,f15_m4,f15_m10,f20_m6,f20_m8,f20_m10\",max_rnd_sim:\"1\",max_rnd_hr:\"21162500\",max_rnd_win:\"5000\",max_rnd_win_a:\"4000\",max_rnd_hr_a:\"15947550\"}}&wl_i=tbm~5000;tbm_a~4000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1,1&wilds=2~400,200,100,10,0~1,1,1,1,1;13~400,200,100,10,0~1,1,1,1,1&bonuses=0&bls=20,25&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;200,100,50,10,0;100,60,40,0,0;80,40,20,0,0;60,20,10,0,0;40,10,6,0,0;40,10,6,0,0;20,8,4,0,0;20,8,4,0,0;20,8,4,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=25,000,000.00&reel_set0=10,7,11,11,5,6,10,9,3,11,12,12,12,12,12,12,12,12,5,6,9,2,8,7,8,11,8,6,3,4,11,11,11,10,10,7,6,8,7,10,10,7,9,1,10,10,10,4,5,12,7,12,8,9,8,12,2,12,11,8,8,9,9,10,4,6,8,12,10,5,4,12,11,12~7,10,9,10,1,7,8,10,11,11,9,9,7,7,7,12,9,10,7,8,10,10,8,9,6,10,6,9,8,10,10,10,8,7,11,4,5,11,10,12,8,4,6,9,12,10,11,11,11,3,7,12,3,7,6,4,4,5,7,12,5,2,7,12,12,12,12,12,12,12,12,11,2,6,5,9,4,4,5,10,11,6,3,8,10,5~4,11,10,4,11,2,12,6,9,5,9,5,10,2,12,12,12,12,12,12,12,12,9,5,6,8,12,8,4,8,11,10,3,9,11,12,6,11,11,11,12,8,4,9,4,12,7,7,3,4,9,8,1,5,4,9,9,9,7,4,10,11,5,12,11,7,6,4,12,9,7,10,10,2~11,2,12,6,9,7,7,11,9,10,8,11,1,7,6,9,7,8,5,11,10,8,9,8,4,3,6,9,8,11,11,7,2,12,10,11,2,7,11,10,3,7,4,2,7,3,12,12,12,12,12,12,12,12,10,9,10,9,4,12,12,4,6,8,2,7,10,4,8,10,6,11,9,12,2,3,9,12,2,8,12,9,4,8,5,8,10,10,12,9,5,7,10,7,8,7,11,6,12,8,11,6~10,7,10,8,9,2,12,1,12,7,9,12,2,7,5,12,7,10,4,5,11,6,9,12,12,10,4,12,10,12,10,8,5,5,11,6,11,5,9,10,6,7,6,7,10,4,6,10,11,4,6,11,9,7,11,10,12,9,10,7,2,8,12,12,12,12,12,12,12,12,7,9,8,8,2,5,1,9,6,1,11,9,5,4,3,8,10,8,6,7,6,12,11,7,11,10,1,5,2,8,10,6,2,12,8,1,4,8,9,6,7,3,10,1,7,12,8,8,9,7,12,6,8,8,6,5,10,5,7,4,2,5,8&reel_set2=4,8,6,10,11,8,10,4,8,1,10,8,11,10~8,11,9,5,7,3,1~8,6,1,10~8,11,6,10,1~8,6,7,9,1&reel_set1=5,6,9,8,12,9,12,8,2,7,11,12,12,12,12,12,12,12,12,7,5,4,10,11,4,9,10,10,12,12,8,11,11,11,8,5,6,9,6,8,12,10,11,7,12,2,11,10,10,10,12,11,3,5,4,6,8,7,10,8,1,4,8,8,9,4,10,11,7,6,9,7,8,3,10,7,10,10~11,4,7,7,6,9,10,4,10,5,1,9,4,7,12,1,7,7,7,8,4,8,7,9,12,6,5,8,10,10,7,10,8,10,5,10,10,10,8,12,7,4,12,5,7,11,8,5,9,2,8,2,9,7,3,11,11,11,3,6,10,4,11,9,3,11,12,6,10,10,6,11,10,8,7,12,12,12,12,12,12,12,12,10,9,7,12,12,7,6,5,8,9,6,12,5,3,9,9,11,10,10~7,4,10,4,6,7,8,2,6,11,12,6,7,3,2,11,4,4,10,4,5,10,12,9,2,8,4,12,12,12,12,12,12,12,12,6,9,9,3,12,1,5,7,4,11,8,5,9,10,7,12,11,10,4,10,11,8,10,9,6,9,9,11,9,4,9,9,9,10,9,4,9,6,7,11,9,4,5,7,9,12,11,8,5,8,12,12,2,12,8,4,5,4,5,10,2,12,7,11~8,9,11,8,7,9,7,12,7,5,9,5,2,10,11,11,1,7,7,6,4,8,10,6,9,8,11,9,11,4,6,8,3,7,9,12,5,4,12,12,12,12,12,12,12,12,8,10,3,8,10,10,7,12,8,4,8,11,2,9,6,8,3,6,11,12,7,10,6,4,2,7,10,2,12,12,2,10,2,11,10,9,12,3,11,7,2~10,7,4,11,2,8,8,6,8,12,1,8,6,7,8,6,9,12,6,5,6,4,8,1,12,6,8,2,12,7,10,7,2,11,12,12,12,12,12,12,12,12,11,7,10,6,9,5,9,4,9,12,7,10,6,4,10,12,7,10,4,8,11,10,11,1,12,2,12,5,7,5,5,3,9,10&reel_set4=7,9,10,4,8,12,8,10,10,11,12,6,9,11,10,3,6,9,8,7,10,8,12,12,12,5,5,6,7,12,10,9,12,7,12,9,12,4,7,11,8,9,4,11,8,5,6,6,5~8,9,10,4,10,7,4,7,6,4,10,12,10,7,9,8,3,8,7,5,11,9,10,7,9,3,10,8,12,8,12,12,12,5,7,3,6,6,10,8,7,8,6,12,12,4,9,8,9,9,8,10,4,11,11,5,11,5,9,11,10,12,7,5,12~7,11,12,9,3,4,11,11,4,12,8,7,4,4,7,10,8,11,8,4,4,10,6,8,7,5,9,6,12,12,12,5,4,9,10,9,6,10,8,5,11,7,9,9,11,9,4,10,5,8,9,7,5,8,3,7,6,6,10,9,8,6,10~9,8,10,10,7,6,8,5,11,5,3,8,3,6,3,8,4,9,6,3,11,8,9,12,10,9,10,11,12,11,7,10,8,5,12,12,12,6,4,10,7,9,7,12,7,8,9,10,6,7,4,9,10,7,7,6,11,4,9,4,10,9,11,7,10,8,7,11,8,11,8,4,10,7,9~5,10,6,7,6,10,8,9,8,8,11,6,6,10,9,7,10,9,5,8,10,9,6,11,9,7,8,8,5,4,10,7,9,11,7,6,8,9,5,4,7,9,9,4,11,8,5,8,10,9,7,10,8,12,12,12,7,9,7,4,6,10,7,11,5,7,5,5,8,11,12,7,6,9,5,8,10,8,7,12,9,10,8,7,12,6,8,4,10,8,6,5,7,10,7,11,7,9,7,4,6,4,6,5,6,4,10,11,7,6,8,10,9&purInit=[{bet:2000,type:\"default\"},{bet:5000,type:\"default\"}]&reel_set3=8,9,6,11,10,5,7,9,6,8,5,9,7,9,12,6,8,4,12,10,4,10,12,12,12,10,8,12,8,9,4,6,8,7,9,11,3,7,8,7,10,12,11,7,12,10,5,5,9,11~8,5,6,12,5,7,8,10,11,7,4,11,9,9,7,3,12,10,12,11,3,9,12,12,12,10,8,6,7,8,10,9,4,7,5,4,9,7,8,5,12,4,9,11,10,8,6,10,10,8~5,8,3,9,9,10,7,10,7,10,3,10,9,4,4,10,8,4,8,6,7,12,4,5,11,10,4,4,6,8,12,7,6,12,12,12,6,8,7,6,7,9,5,10,4,6,3,9,9,11,9,6,8,9,9,11,5,10,11,8,9,9,4,5,11,5,8,7,4,11,11~6,8,10,7,8,6,10,12,8,9,8,7,9,8,9,11,10,11,11,5,8,10,11,3,10,7,4,8,12,7,11,5,9,4,10,8,7,6,9,7,6,9,12,12,12,6,10,5,9,5,9,10,7,10,7,9,12,4,11,11,7,4,11,8,7,3,4,7,3,7,10,11,4,3,4,8,10,10,3,8,9,6,11,6,8,6,10,7,9,7,9~4,10,8,6,10,9,7,9,5,5,7,11,10,11,7,5,10,8,7,10,6,10,6,8,9,7,8,10,7,8,9,10,12,6,12,12,12,11,4,7,8,7,6,7,4,9,6,7,10,7,5,5,10,8,4,11,5,9,9,6,8,6,9,9,8,11,8,4,6,8,10,7,5&reel_set6=6,7,10,11,6,4,4,8,10,8,6,7,5,8,9,7,10,11,5,9,10,5,5,11,9,10,8,7,3,9,7,11,9,9,8,7,10,8,6,8,9,4~8,9,4,10,9,8,6,9,7,4,7,5,10,6,11,3,8,9,9,10,10,8,5,7,8,7,10,7,7,9,11,5,10,11,8,6,5,4~6,7,8,10,5,9,11,4,4,9,10,8,9,7,10,9,8,4,11,8,3,8,4,9,6,4,11,7,9,11,5,4,6,6,7,5,3,8,4,5,9,10,7,11,7,9,8,5,10,6,11,10,6,9,10~9,11,9,10,9,10,8,5,3,6,4,3,7,3,9,4,6,6,10,10,8,5,7,6,4,7,8,11,7,11,6,7,7,10,8,9,8,4,11,9,6,6,8,4,9,7,8,10,10,7,11,11,8,9,7,11,8,4,3,10,7,4,7,10,11,11,8,7,5,9,5,10,7,10~8,5,10,7,9,6,7,8,10,6,9,10,5,4,6,8,8,6,8,11,11,8,8,5,9,11,10,8,5,10,8,6,7,11,9,9,6,9,5,9,7,11,9,8,7,10,6,8,7,5,10,8,10,5,11,7,10,3,6,11,7,6,7,6,7,10,8,7,9,7,10,6,10,4,7,6,9,9,4,6,4,8,10,5,9,4,9,9,3,8,5,7,11,7,5,8,4,10,7,4,8,9,7,6,7&reel_set5=11,12,11,6,8,8,12,10,6,5,6,4,10,10,5,10,7,9,11,10,7,10,7,12,9,10,7,8,4,11,10,9,7,12,12,12,12,12,12,3,9,9,7,4,8,5,9,12,8,3,6,9,5,6,12,7,4,4,9,6,8,9,7,9,5,8,5,8,11,8,11,10,8,7~8,9,9,10,6,11,6,5,8,8,5,7,4,6,3,4,9,8,12,8,10,4,10,7,10,3,12,12,12,12,12,12,8,8,7,7,5,10,8,11,4,5,7,11,12,6,5,9,9,10,9,12,11,12,7,10,9,7,9,10~10,6,5,9,8,11,5,3,10,7,12,9,10,9,3,10,6,9,6,8,9,6,11,7,4,10,8,7,9,4,12,10,12,9,11,12,12,12,12,12,12,8,7,10,9,4,8,10,11,4,7,8,12,7,6,5,7,6,4,9,11,12,7,5,5,9,11,4,5,10,6,4,9,12,8,4,8,11~8,10,6,7,9,8,11,4,10,11,7,6,12,5,4,5,11,10,10,7,8,9,11,8,3,7,12,9,8,11,6,12,12,12,12,12,12,10,9,4,6,9,7,8,9,7,8,9,7,11,3,7,10,4,4,8,6,9,7,9,8,12,11,10,5,10,10,6,12,7~10,4,10,5,7,4,9,9,11,9,8,11,8,10,6,7,8,6,7,4,5,11,6,8,12,5,6,7,12,9,8,10,11,10,4,9,8,9,12,10,7,5,9,12,12,12,12,12,12,7,4,8,11,6,9,9,10,7,8,7,6,12,5,6,11,8,10,8,10,8,5,7,9,8,5,6,7,8,7,4,10,9,9,10,6,6,7,3,6,7,5,5,10&reel_set8=7,7,4,8,4,7,8,6,8,7,10,6,10~5,9,9,9,3,9,11,5,11,11,11,5,3,11,9~7,3,8,5,9,7,6,11,4,9,11,9,4,10,2,8,2,5,6,11,11,4,11,10,10,4,7,4,9,7,10,3,6,8,6,4,10~11,2,3,2,9,6,11,8,10,7,10,7,10,8,4,9,8,7,8,4,11,6,9,7,10,9,6,2,3,9,10,5,4,10,11,8,2,11,6,7~10,8,7,2,9,3,11,6,9,7,9,4,8,7,5,6,8,10,2,8,7,11,10,6,3,4,7&reel_set7=12,12,12,12,12,12,12,12~12,12,12,12,12,12,12,12~12,12,12,12,12,12,12,12~12,12,7,12,12,12,12,12,12,12,12,10,12,12,9,12,12,8~7,12,12,5,7,12,12,12,12,12,12,12,12,9,11,12,12,10,12,9,12,8&total_bet_min=10.00";
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
            get { return 1.25; }
        }
        protected override bool SupportMoreBet
        {
            get { return true; }
        }
        protected override int FreeSpinTypeCount
        {
            get
            {
                return 20;
            }
        }
        protected int FreePurCount
        {
            get { return 2; }
        }
        protected int[] PurchaseMultiples
        {
            get { return new int[] { 100, 250 }; }
        }
        #endregion
        public ReleaseTheKraken2GameLogic()
        {
            _gameID = GAMEID.ReleaseTheKraken2;
            GameName = "ReleaseTheKraken2";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
	        dicParams["bl"] = "0";
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
                _multiMinFreeSpinWinRates = new double[FreePurCount];

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
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }	
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                ReleaseTheKraken2BetInfo betInfo = new ReleaseTheKraken2BetInfo();
                betInfo.BetPerLine               = (float)message.Pop();
                betInfo.LineCount                = (int)message.Pop();
		
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
                {
                    betInfo.PurchaseFree = false;
                }

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in ReleaseTheKraken2GameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in ReleaseTheKraken2GameLogic::readBetInfoFromMessage", strUserID);
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
                    (oldBetInfo as ReleaseTheKraken2BetInfo).PurchaseType = betInfo.PurchaseType;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in ReleaseTheKraken2GameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            var bookBetInfo = betInfo as ReleaseTheKraken2BetInfo;
            if (bookBetInfo.PurchaseType == 0)
                return 100.0;
            else
                return 250.0;
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            ReleaseTheKraken2BetInfo betInfo = new ReleaseTheKraken2BetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new ReleaseTheKraken2BetInfo();
        }
        protected override async Task<BasePPSlotSpinData> selectMinStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int purchaseType = (betInfo as ReleaseTheKraken2BetInfo).PurchaseType;
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSelFreeMinStartRequest(GameName, purchaseType), TimeSpan.FromSeconds(10.0));

                var spinData = convertBsonToSpinData(spinDataDocument);
                if (!(spinData is BasePPSlotStartSpinData))
                    return spinData;

                BasePPSlotStartSpinData startSpinData = spinData as BasePPSlotStartSpinData;
                double minOdd = PurchaseMultiples[purchaseType] * 0.2 - spinData.SpinOdd;
                if (minOdd < 0.0)
                    minOdd = 0.0;
                double maxOdd = PurchaseMultiples[purchaseType] * 0.5 - spinData.SpinOdd;
                await buildStartFreeSpinData(startSpinData, StartSpinBuildTypes.IsRangeLimited, minOdd, maxOdd);
                return spinData;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in ReleaseTheKraken2GameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int purchaseType = (betInfo as ReleaseTheKraken2BetInfo).PurchaseType;
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType),
                        TimeSpan.FromSeconds(10.0));

                var spinData =  convertBsonToSpinData(spinDataDocument);
                if (!(spinData is BasePPSlotStartSpinData))
                    return spinData;

                BasePPSlotStartSpinData startSpinData = spinData as BasePPSlotStartSpinData;
                await buildStartFreeSpinData(startSpinData, StartSpinBuildTypes.IsTotalRandom, 0.0, 0.0);
                return startSpinData;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in ReleaseTheKraken2GameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int agentID, BasePPSlotBetInfo betInfo, double baseBet, UserBonus userBonus)
        {
            int purchaseType = (betInfo as ReleaseTheKraken2BetInfo).PurchaseType;
            double payoutRate = getPayoutRate(agentID);

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
        protected override async Task buildStartFreeSpinData(BasePPSlotStartSpinData startSpinData, StartSpinBuildTypes buildType, double minOdd, double maxOdd)
        {
            if (buildType == StartSpinBuildTypes.IsNaturalRandom)
                buildType = StartSpinBuildTypes.IsTotalRandom;

            startSpinData.FreeSpins         = new List<BsonDocument>();
            string      strFreeSpinTypes    = (startSpinData as ReleaseTheKraken2StartSpinData).FreeSpinTypes;
            string[]    strParts            = strFreeSpinTypes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int[] freeSpinTypes = new int[strParts.Length];
            for (int i = 0; i < strParts.Length; i++)
                freeSpinTypes[i] = 200 + int.Parse(strParts[i]);

            double maxFreeOdd       = 0.0;
            for (int i = 0; i < freeSpinTypes.Length; i++)
            {
                BsonDocument childFreeSpin = null;
                if (buildType == StartSpinBuildTypes.IsNaturalRandom)
                {
                    childFreeSpin = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeIDRangeRequest(GameName, freeSpinTypes[i], _normalMaxID), TimeSpan.FromSeconds(10.0));
                }
                else if (buildType == StartSpinBuildTypes.IsTotalRandom)
                {
                    childFreeSpin = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeIDRangeRequest(GameName, freeSpinTypes[i], -1), TimeSpan.FromSeconds(10.0));
                }
                else
                {
                    childFreeSpin = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                            new SelectSpinTypeOddRangeRequest(GameName, freeSpinTypes[i], minOdd, maxOdd), TimeSpan.FromSeconds(10.0));
                }
                double childOdd = (double)childFreeSpin["odd"];
                if (childOdd > maxFreeOdd)
                    maxFreeOdd = childOdd;
                startSpinData.FreeSpins.Add(childFreeSpin);
            }
            startSpinData.MaxOdd = startSpinData.StartOdd + maxFreeOdd;
        }
        protected override BasePPSlotSpinData convertBsonToSpinData(BsonDocument document)
        {
            int     spinType    = (int)document["spintype"];
            double  spinOdd     = (double)document["odd"];
            string  strData     = (string)document["data"];

            List<string> spinResponses = new List<string>(strData.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
            if (spinType == 100)
            {
                ReleaseTheKraken2StartSpinData startSpinData = new ReleaseTheKraken2StartSpinData();
                startSpinData.StartOdd                       = spinOdd;
                startSpinData.FreeSpinTypes                  = (string) document["freespintypes"];
                startSpinData.SpinStrings                    = spinResponses;
                return startSpinData;
            }
            else
            {
                return new BasePPSlotSpinData(spinType, spinOdd, spinResponses);
            }
        }
        protected void overrideBonusParams(Dictionary<string, string> dicParams, ReleaseTheKraken2Result result, int ind, bool isCollect)
        {
            var          gParam         = JToken.Parse(dicParams["g"]);
            List<string> strParams      = new List<string>();
            int[]        startParams    = new int[] { 0, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 1, 2, 2, 2, 3 };

            int index = 0;
            for(int i = 0; i < result.DoBonusCounter + 1; i++)
            {
                if(i == result.DoBonusCounter)
                {
                    strParams.Add(string.Format("{0}~{1}", startParams[i], ind));
                }
                else
                {
                    if (startParams[i] == 2)
                    {
                        strParams.Add(string.Format("{0}~{1}", startParams[i], result.BonusSelections[index]));
                        index++;
                    }
                    else
                    {
                        strParams.Add(string.Format("{0}~{1}", startParams[i], 1));
                    }
                }
            }
            int[]       status      = new int[16];
            string[]    newWinsMask = new string[16];
            string[]    winsMask    = gParam["bg_0"]["wins_mask"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < 16; i++)
                newWinsMask[i] = "h";

            for(int i = 0; i < result.BonusSelections.Count; i++)
            {
                status[result.BonusSelections[i]]       = 1;
                newWinsMask[result.BonusSelections[i]]  = winsMask[i];
            }
            gParam["bg_0"]["ch_h"]      = string.Join(",", strParams);
            gParam["bg_0"]["status"]    = string.Join(",", status);
            gParam["bg_0"]["wins"]      = string.Join(",", status);
            gParam["bg_0"]["wins_mask"] = string.Join(",", newWinsMask);
            if (result.BonusSelections.Count > 0)
                gParam["bg_0"]["wi"] = result.BonusSelections[result.BonusSelections.Count - 1].ToString();

            if (isCollect)
            {
                gParam["bg_0"]["level"] = (result.DoBonusCounter + 1).ToString();
                if (result.DoBonusCounter == 0)
                {
                    removeJTokenField(gParam["bg_0"], "status");
                    removeJTokenField(gParam["bg_0"], "wins");
                    removeJTokenField(gParam["bg_0"], "wins_mask");
                    removeJTokenField(gParam["bg_0"], "wi");
                }
            }
            dicParams["g"] = serializeJsonSpecial(gParam);
        }
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                int        index           = (int)message.Pop();
                int        counter         = (int)message.Pop();
                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                string     strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BasePPSlotBetInfo       betInfo = _dicUserBetInfos[strGlobalUserID];
                    ReleaseTheKraken2Result result  = _dicUserResultInfos[strGlobalUserID] as ReleaseTheKraken2Result;
                    if ((result.NextAction != ActionTypes.DOBONUS) || (betInfo.SpinData == null) || !(betInfo.SpinData is BasePPSlotStartSpinData))
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        bool isCollect = false;
                        Dictionary<string, string> dicParams = null;

                        if (result.BonusSelections == null)
                            result.BonusSelections = new List<int>();

                        int ind             = (int)message.Pop();
                        var startSpinData   = betInfo.SpinData as ReleaseTheKraken2StartSpinData;
                        int stage           = startSpinData.FreeSpinGroup + result.DoBonusCounter;
                        int selectedFreeId  = 0;
                        if(result.DoBonusCounter == 0 || result.DoBonusCounter == 5
                            || result.DoBonusCounter == 10 || result.DoBonusCounter == 14)                                
                        {
                            if(ind == 0)
                            {
                                if (result.DoBonusCounter == 0)
                                    selectedFreeId = 0;
                                else if (result.DoBonusCounter == 5)
                                    selectedFreeId = 1;
                                else if (result.DoBonusCounter == 10)
                                    selectedFreeId = 2;
                                else
                                    selectedFreeId = 3;
                                isCollect = true;
                            }
                        }
                        else if(result.DoBonusCounter == 18)
                        {
                            isCollect = true;
                            if (ind == 0)
                                selectedFreeId = 4;
                            else
                                selectedFreeId = 0;
                        }
                        else
                        {
                            if (result.BonusSelections.Contains(ind))
                                throw new Exception(string.Format("{0} User selected already selected position, Malicious Behavior {1}", strGlobalUserID, ind));

                            result.BonusSelections.Add(ind);
                        }
                        if (isCollect)
                        {
                            BasePPSlotSpinData freeSpinData = convertBsonToSpinData(startSpinData.FreeSpins[selectedFreeId]);
                            preprocessSelectedFreeSpin(freeSpinData, betInfo);

                            betInfo.SpinData = freeSpinData;
                            List<string> freeSpinStrings = new List<string>();
                            for (int i = 0; i < freeSpinData.SpinStrings.Count; i++)
                                freeSpinStrings.Add(addStartWinToResponse(freeSpinData.SpinStrings[i], startSpinData.StartOdd));

                            betInfo.RemainReponses = buildResponseList(freeSpinStrings, ActionTypes.DOSPIN);
                            double selectedWin     = (startSpinData.StartOdd + freeSpinData.SpinOdd) * betInfo.TotalBet;
                            double maxWin          = startSpinData.MaxOdd * betInfo.TotalBet;
                            
                            sumUpWebsiteBetWin(agentID, 0.0, selectedWin - maxWin);
                        }
                        var response = betInfo.pullRemainResponse();
                        dicParams    = splitResponseToParams(response.Response);
                        overrideBonusParams(dicParams, result, ind, isCollect);
                        convertWinsByBet(dicParams, betInfo.TotalBet);
                        convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                        result.DoBonusCounter++;
                        result.BonusResultString = convertKeyValuesToString(dicParams);
                        addDefaultParams(dicParams, userBalance, index, counter);
                        ActionTypes nextAction = convertStringToActionType(dicParams["na"]);
                        string strResponse = convertKeyValuesToString(dicParams);
                        responseMessage.Append(strResponse);

                        //히스토리보관 및 초기화
                        if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                            addIndActionHistory(strGlobalUserID, "doBonus", strResponse, index, counter, ind);

                        result.NextAction = nextAction;
                        if (!betInfo.HasRemainResponse)
                            betInfo.RemainReponses = null;
                        
                        saveBetResultInfo(strGlobalUserID);
                    }
                }
                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in ReleaseTheKraken2GameLogic::onDoBonus {0}", ex);
            }
        }
        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            ReleaseTheKraken2Result result = new ReleaseTheKraken2Result();
            result.SerializeFrom(reader);
            return result;

        }      
        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst)
        {
            try
            {
                ReleaseTheKraken2Result spinResult   = new ReleaseTheKraken2Result();
                Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(dicParams, betInfo.TotalBet);

                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                if (SupportPurchaseFree && betInfo.PurchaseFree && isFirst)
                    dicParams["purtr"] = "1";

                spinResult.NextAction = convertStringToActionType(dicParams["na"]);
                if (spinResult.NextAction == ActionTypes.DOCOLLECT || spinResult.NextAction == ActionTypes.DOCOLLECTBONUS)
                    spinResult.TotalWin = double.Parse(dicParams["tw"]);
                else
                    spinResult.TotalWin = 0.0;

                spinResult.ResultString = convertKeyValuesToString(dicParams);
                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in ReleaseTheKraken2GameLogic::calculateResult {0}", ex);
                return null;
            }
        }
        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            ReleaseTheKraken2BetInfo releaseBetInfo = betInfo as ReleaseTheKraken2BetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, betInfo.MoreBet ? 1 : 0, betInfo.PurchaseFree ? releaseBetInfo.PurchaseType : -1, betMoney);
        }
    }
}
