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
    class FonzosFelineFortunesBetInfo : BasePPSlotBetInfo
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

    class FonzosFelineFortunesGameLogic : BasePPSlotGame
    {
        protected double[] _multiTotalFreeSpinWinRates;
        protected double[] _multiMinFreeSpinWinRates;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10fonzofff";
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
                return "def_s=3,11,5,5,11,6,8,9,4,10,5,7,11,5,7&cfgs=1&ver=3&def_sb=6,7,6,4,10&reel_set_size=16&def_sa=3,8,4,6,8&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{ante:\"96.50\",purchase_1:\"96.50\",purchase_0:\"96.50\",regular:\"96.50\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"384692307\",max_rnd_win:\"5000\",max_rnd_win_a:\"3334\",max_rnd_hr_a:\"196117647\"}}&wl_i=tbm~5000;tbm_a~3334&reel_set10=11,5,6,7,9,1,4,5,11,6,10,5,4,9,5,4,7,6,6,11,3,9,4,7,3,9,2,11,7,9,3,7,8,1,11,9,8,3,2,9,4,7,11,2,5,7,11,5,3,9,10,11,7,2,2,2,2,2~5,3,2,8,6,10,2,7,5,2,8,6,10,4,9,5,2,8,6,6,11,2,3,10,2,5,1,8,4,10,8,5,10,3,8,2,4,3,10,5,2,4,10,1,11,8,9,3,2,10,4,1,8,7,2,2,2,2,2~5,11,1,2,5,6,10,2,4,6,9,3,2,4,1,8,3,4,5,6,6,11,8,4,3,2,7,4,5,10,3,5,1,9,2,5,3,2,7,2,2,2,2,2~3,4,6,5,1,11,4,1,7,6,5,3,10,2,8,6,6,9,5,2,4,1,2,2,2,2,2~4,3,5,6,10,1,3,7,6,4,2,11,1,6,6,9,1,5,8,2,2,2,2,2&sc=10.00,20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&reel_set11=11,5,6,7,9,1,4,5,11,6,10,5,4,9,5,4,7,6,6,11,3,9,4,7,3,9,2,11,7,9,3,7,8,1,11,9,8,3,2,9,4,7,11,2,5,7,11,5,3,9,10,11,7,2,2,2,2,2~5,3,2,8,6,10,2,7,5,2,8,6,10,4,9,5,2,8,6,6,11,2,3,10,2,5,1,8,4,10,8,5,10,3,8,2,4,3,10,5,2,4,10,1,11,8,9,3,2,10,4,1,8,7,2,2,2,2,2~5,11,1,2,5,6,10,2,4,6,9,3,2,4,1,8,3,4,5,6,6,11,8,4,3,2,7,4,5,10,3,5,1,9,2,5,3,2,7,2,2,2,2,2~3,4,6,5,1,11,4,1,7,6,5,3,10,2,8,6,6,9,5,2,4,1,2,2,2,2,2~4,3,5,6,10,1,3,7,6,4,2,11,1,6,6,9,1,5,8,2,2,2,2,2&reel_set12=11,5,6,7,9,1,4,5,11,6,10,5,4,9,5,4,7,6,6,11,3,9,4,7,3,9,2,11,7,9,3,7,8,1,11,9,8,3,2,9,4,7,11,2,5,7,11,5,3,9,10,11,7,2,2,2,2,2~5,3,2,8,6,10,2,7,5,2,8,6,10,4,9,5,2,8,6,6,11,2,3,10,2,5,1,8,4,10,8,5,10,3,8,2,4,3,10,5,2,4,10,1,11,8,9,3,2,10,4,1,8,7,2,2,2,2,2~5,11,1,2,5,6,10,2,4,6,9,3,2,4,1,8,3,4,5,6,6,11,8,4,3,2,7,4,5,10,3,5,1,9,2,5,3,2,7,2,2,2,2,2~3,4,6,5,1,11,4,1,7,6,5,3,10,2,8,6,6,9,5,2,4,1,2,2,2,2,2~4,3,5,6,10,1,3,7,6,4,2,11,1,6,6,9,1,5,8,2,2,2,2,2&purInit_e=1,1&reel_set13=11,5,6,7,9,1,4,5,11,6,10,5,4,9,5,4,7,6,6,11,3,9,4,7,3,9,2,11,7,9,3,7,8,1,11,9,8,3,2,9,4,7,11,2,5,7,11,5,3,9,10,11,7,2,2,2,2,2~5,3,2,8,6,10,2,7,5,2,8,6,10,4,9,5,2,8,6,6,11,2,3,10,2,5,1,8,4,10,8,5,10,3,8,2,4,3,10,5,2,4,10,1,11,8,9,3,2,10,4,1,8,7,2,2,2,2,2~5,11,1,2,5,6,10,2,4,6,9,3,2,4,1,8,3,4,5,6,6,11,8,4,3,2,7,4,5,10,3,5,1,9,2,5,3,2,7,2,2,2,2,2~3,4,6,5,1,11,4,1,7,6,5,3,10,2,8,6,6,9,5,2,4,1,2,2,2,2,2~4,3,5,6,10,1,3,7,6,4,2,11,1,6,6,9,1,5,8,2,2,2,2,2&wilds=2~1000,100,30,0,0~1,1,1,1,1&bonuses=0&bls=10,15&ntp=0.00&reel_set14=11,5,6,7,9,1,4,5,11,6,10,5,4,9,5,4,7,6,6,11,3,9,4,7,3,9,2,11,7,9,3,7,8,1,11,9,8,3,2,9,4,7,11,2,5,7,11,5,3,9,10,11,7,2,2,2,2,2~5,3,2,8,6,10,2,7,5,2,8,6,10,4,9,5,2,8,6,6,11,2,3,10,2,5,1,8,4,10,8,5,10,3,8,2,4,3,10,5,2,4,10,1,11,8,9,3,2,10,4,1,8,7,2,2,2,2,2~5,11,1,2,5,6,10,2,4,6,9,3,2,4,1,8,3,4,5,6,6,11,8,4,3,2,7,4,5,10,3,5,1,9,2,5,3,2,7,2,2,2,2,2~3,4,6,5,1,11,4,1,7,6,5,3,10,2,8,6,6,9,5,2,4,1,2,2,2,2,2~4,3,5,6,10,1,3,7,6,4,2,11,1,6,6,9,1,5,8,2,2,2,2,2&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;1000,100,30,0,0;250,50,10,0,0;100,20,5,0,0;100,20,5,0,0;50,10,2,0,0;50,10,2,0,0;30,5,1,0,0;30,5,1,0,0;30,5,1,0,0&reel_set15=11,5,6,7,9,1,4,5,11,6,10,5,4,9,5,4,7,6,6,11,3,9,4,7,3,9,2,11,7,9,3,7,8,1,11,9,8,3,2,9,4,7,11,2,5,7,11,5,3,9,10,11,7,2,2,2,2,2~5,3,2,8,6,10,2,7,5,2,8,6,10,4,9,5,2,8,6,6,11,2,3,10,2,5,1,8,4,10,8,5,10,3,8,2,4,3,10,5,2,4,10,1,11,8,9,3,2,10,4,1,8,7,2,2,2,2,2~5,11,1,2,5,6,10,2,4,6,9,3,2,4,1,8,3,4,5,6,6,11,8,4,3,2,7,4,5,10,3,5,1,9,2,5,3,2,7,2,2,2,2,2~3,4,6,5,1,11,4,1,7,6,5,3,10,2,8,6,6,9,5,2,4,1,2,2,2,2,2~4,3,5,6,10,1,3,7,6,4,2,11,1,6,6,9,1,5,8,2,2,2,2,2&total_bet_max=27,000,000.00&reel_set0=11,5,6,7,9,1,4,5,11,6,10,5,4,9,5,4,7,6,6,11,3,9,4,7,3,9,2,11,7,9,3,7,8,1,11,9,8,3,2,9,4,7,11,2,5,7,11,5,3,9,10,11,7,2,2,2,2,2~5,3,2,8,6,10,2,7,5,2,8,6,10,4,9,5,2,8,6,6,11,2,3,10,2,5,1,8,4,10,8,5,10,3,8,2,4,3,10,5,2,4,10,1,11,8,9,3,2,10,4,1,8,7,2,2,2,2,2~5,11,1,2,5,6,10,2,4,6,9,3,2,4,1,8,3,4,5,6,6,11,8,4,3,2,7,4,5,10,3,5,1,9,2,5,3,2,7,2,2,2,2,2~3,4,6,5,1,11,4,1,7,6,5,3,10,2,8,6,6,9,5,2,4,1,2,2,2,2,2~4,3,5,6,10,1,3,7,6,4,2,11,1,6,6,9,1,5,8,2,2,2,2,2&reel_set2=11,5,6,7,9,1,4,5,11,6,10,5,4,9,5,4,7,6,6,11,3,9,4,7,3,9,2,11,7,9,3,7,8,1,11,9,8,3,2,9,4,7,11,2,5,7,11,5,3,9,10,11,7,2,2,2,2,2~5,3,2,8,6,10,2,7,5,2,8,6,10,4,9,5,2,8,6,6,11,2,3,10,2,5,1,8,4,10,8,5,10,3,8,2,4,3,10,5,2,4,10,1,11,8,9,3,2,10,4,1,8,7,2,2,2,2,2~5,11,1,2,5,6,10,2,4,6,9,3,2,4,1,8,3,4,5,6,6,11,8,4,3,2,7,4,5,10,3,5,1,9,2,5,3,2,7,2,2,2,2,2~3,4,6,5,1,11,4,1,7,6,5,3,10,2,8,6,6,9,5,2,4,1,2,2,2,2,2~4,3,5,6,10,1,3,7,6,4,2,11,1,6,6,9,1,5,8,2,2,2,2,2&reel_set1=11,5,6,7,9,1,4,5,11,6,10,5,4,9,5,4,7,6,6,11,3,9,4,7,3,9,2,11,7,9,3,7,8,1,11,9,8,3,2,9,4,7,11,2,5,7,11,5,3,9,10,11,7,2,2,2,2,2~5,3,2,8,6,10,2,7,5,2,8,6,10,4,9,5,2,8,6,6,11,2,3,10,2,5,1,8,4,10,8,5,10,3,8,2,4,3,10,5,2,4,10,1,11,8,9,3,2,10,4,1,8,7,2,2,2,2,2~5,11,1,2,5,6,10,2,4,6,9,3,2,4,1,8,3,4,5,6,6,11,8,4,3,2,7,4,5,10,3,5,1,9,2,5,3,2,7,2,2,2,2,2~3,4,6,5,1,11,4,1,7,6,5,3,10,2,8,6,6,9,5,2,4,1,2,2,2,2,2~4,3,5,6,10,1,3,7,6,4,2,11,1,6,6,9,1,5,8,2,2,2,2,2&reel_set4=11,5,6,7,9,1,4,5,11,6,10,5,4,9,5,4,7,6,6,11,3,9,4,7,3,9,2,11,7,9,3,7,8,1,11,9,8,3,2,9,4,7,11,2,5,7,11,5,3,9,10,11,7,2,2,2,2,2~5,3,2,8,6,10,2,7,5,2,8,6,10,4,9,5,2,8,6,6,11,2,3,10,2,5,1,8,4,10,8,5,10,3,8,2,4,3,10,5,2,4,10,1,11,8,9,3,2,10,4,1,8,7,2,2,2,2,2~5,11,1,2,5,6,10,2,4,6,9,3,2,4,1,8,3,4,5,6,6,11,8,4,3,2,7,4,5,10,3,5,1,9,2,5,3,2,7,2,2,2,2,2~3,4,6,5,1,11,4,1,7,6,5,3,10,2,8,6,6,9,5,2,4,1,2,2,2,2,2~4,3,5,6,10,1,3,7,6,4,2,11,1,6,6,9,1,5,8,2,2,2,2,2&purInit=[{bet:1000},{bet:2700}]&reel_set3=11,5,6,7,9,1,4,5,11,6,10,5,4,9,5,4,7,6,6,11,3,9,4,7,3,9,2,11,7,9,3,7,8,1,11,9,8,3,2,9,4,7,11,2,5,7,11,5,3,9,10,11,7,2,2,2,2,2~5,3,2,8,6,10,2,7,5,2,8,6,10,4,9,5,2,8,6,6,11,2,3,10,2,5,1,8,4,10,8,5,10,3,8,2,4,3,10,5,2,4,10,1,11,8,9,3,2,10,4,1,8,7,2,2,2,2,2~5,11,1,2,5,6,10,2,4,6,9,3,2,4,1,8,3,4,5,6,6,11,8,4,3,2,7,4,5,10,3,5,1,9,2,5,3,2,7,2,2,2,2,2~3,4,6,5,1,11,4,1,7,6,5,3,10,2,8,6,6,9,5,2,4,1,2,2,2,2,2~4,3,5,6,10,1,3,7,6,4,2,11,1,6,6,9,1,5,8,2,2,2,2,2&reel_set6=11,5,6,7,9,1,4,5,11,6,10,5,4,9,5,4,7,6,6,11,3,9,4,7,3,9,2,11,7,9,3,7,8,1,11,9,8,3,2,9,4,7,11,2,5,7,11,5,3,9,10,11,7,2,2,2,2,2~5,3,2,8,6,10,2,7,5,2,8,6,10,4,9,5,2,8,6,6,11,2,3,10,2,5,1,8,4,10,8,5,10,3,8,2,4,3,10,5,2,4,10,1,11,8,9,3,2,10,4,1,8,7,2,2,2,2,2~5,11,1,2,5,6,10,2,4,6,9,3,2,4,1,8,3,4,5,6,6,11,8,4,3,2,7,4,5,10,3,5,1,9,2,5,3,2,7,2,2,2,2,2~3,4,6,5,1,11,4,1,7,6,5,3,10,2,8,6,6,9,5,2,4,1,2,2,2,2,2~4,3,5,6,10,1,3,7,6,4,2,11,1,6,6,9,1,5,8,2,2,2,2,2&reel_set5=11,5,6,7,9,1,4,5,11,6,10,5,4,9,5,4,7,6,6,11,3,9,4,7,3,9,2,11,7,9,3,7,8,1,11,9,8,3,2,9,4,7,11,2,5,7,11,5,3,9,10,11,7,2,2,2,2,2~5,3,2,8,6,10,2,7,5,2,8,6,10,4,9,5,2,8,6,6,11,2,3,10,2,5,1,8,4,10,8,5,10,3,8,2,4,3,10,5,2,4,10,1,11,8,9,3,2,10,4,1,8,7,2,2,2,2,2~5,11,1,2,5,6,10,2,4,6,9,3,2,4,1,8,3,4,5,6,6,11,8,4,3,2,7,4,5,10,3,5,1,9,2,5,3,2,7,2,2,2,2,2~3,4,6,5,1,11,4,1,7,6,5,3,10,2,8,6,6,9,5,2,4,1,2,2,2,2,2~4,3,5,6,10,1,3,7,6,4,2,11,1,6,6,9,1,5,8,2,2,2,2,2&reel_set8=11,5,6,7,9,1,4,5,11,6,10,5,4,9,5,4,7,6,6,11,3,9,4,7,3,9,2,11,7,9,3,7,8,1,11,9,8,3,2,9,4,7,11,2,5,7,11,5,3,9,10,11,7,2,2,2,2,2~5,3,2,8,6,10,2,7,5,2,8,6,10,4,9,5,2,8,6,6,11,2,3,10,2,5,1,8,4,10,8,5,10,3,8,2,4,3,10,5,2,4,10,1,11,8,9,3,2,10,4,1,8,7,2,2,2,2,2~5,11,1,2,5,6,10,2,4,6,9,3,2,4,1,8,3,4,5,6,6,11,8,4,3,2,7,4,5,10,3,5,1,9,2,5,3,2,7,2,2,2,2,2~3,4,6,5,1,11,4,1,7,6,5,3,10,2,8,6,6,9,5,2,4,1,2,2,2,2,2~4,3,5,6,10,1,3,7,6,4,2,11,1,6,6,9,1,5,8,2,2,2,2,2&reel_set7=11,5,6,7,9,1,4,5,11,6,10,5,4,9,5,4,7,6,6,11,3,9,4,7,3,9,2,11,7,9,3,7,8,1,11,9,8,3,2,9,4,7,11,2,5,7,11,5,3,9,10,11,7,2,2,2,2,2~5,3,2,8,6,10,2,7,5,2,8,6,10,4,9,5,2,8,6,6,11,2,3,10,2,5,1,8,4,10,8,5,10,3,8,2,4,3,10,5,2,4,10,1,11,8,9,3,2,10,4,1,8,7,2,2,2,2,2~5,11,1,2,5,6,10,2,4,6,9,3,2,4,1,8,3,4,5,6,6,11,8,4,3,2,7,4,5,10,3,5,1,9,2,5,3,2,7,2,2,2,2,2~3,4,6,5,1,11,4,1,7,6,5,3,10,2,8,6,6,9,5,2,4,1,2,2,2,2,2~4,3,5,6,10,1,3,7,6,4,2,11,1,6,6,9,1,5,8,2,2,2,2,2&reel_set9=11,5,6,7,9,1,4,5,11,6,10,5,4,9,5,4,7,6,6,11,3,9,4,7,3,9,2,11,7,9,3,7,8,1,11,9,8,3,2,9,4,7,11,2,5,7,11,5,3,9,10,11,7,2,2,2,2,2~5,3,2,8,6,10,2,7,5,2,8,6,10,4,9,5,2,8,6,6,11,2,3,10,2,5,1,8,4,10,8,5,10,3,8,2,4,3,10,5,2,4,10,1,11,8,9,3,2,10,4,1,8,7,2,2,2,2,2~5,11,1,2,5,6,10,2,4,6,9,3,2,4,1,8,3,4,5,6,6,11,8,4,3,2,7,4,5,10,3,5,1,9,2,5,3,2,7,2,2,2,2,2~3,4,6,5,1,11,4,1,7,6,5,3,10,2,8,6,6,9,5,2,4,1,2,2,2,2,2~4,3,5,6,10,1,3,7,6,4,2,11,1,6,6,9,1,5,8,2,2,2,2,2&total_bet_min=100.00";
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
        public FonzosFelineFortunesGameLogic()
        {
            _gameID = GAMEID.FonzosFelineFortunes;
            GameName = "FonzosFelineFortunes";
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


                FonzosFelineFortunesBetInfo betInfo = new FonzosFelineFortunesBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();

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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in FonzosFelineFortunesGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                    (oldBetInfo as FonzosFelineFortunesBetInfo).PurchaseType = betInfo.PurchaseType;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in FonzosFelineFortunesGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            FonzosFelineFortunesBetInfo betInfo = new FonzosFelineFortunesBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new FonzosFelineFortunesBetInfo();
        }
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as FonzosFelineFortunesBetInfo).PurchaseType;
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
        protected override async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int agentID, BasePPSlotBetInfo betInfo, double baseBet, UserBonus userBonus, bool isAffiliate)
        {
            int purchaseType = (betInfo as FonzosFelineFortunesBetInfo).PurchaseType;
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
                int purchaseType = (betInfo as FonzosFelineFortunesBetInfo).PurchaseType;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, -1, PurchaseMultiples[purchaseType] * 0.2, PurchaseMultiples[purchaseType] * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in FonzosFelineFortunesGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int purchaseType = (betInfo as FonzosFelineFortunesBetInfo).PurchaseType;
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in FonzosFelineFortunesGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = (betInfo as FonzosFelineFortunesBetInfo).PurchaseType.ToString();
        }
        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            FonzosFelineFortunesBetInfo starBetInfo = betInfo as FonzosFelineFortunesBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, -1, betInfo.PurchaseFree ? starBetInfo.PurchaseType : -1, betMoney);
        }
    }
}
