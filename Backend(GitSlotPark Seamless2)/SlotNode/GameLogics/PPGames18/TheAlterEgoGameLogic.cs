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
    class TheAlterEgoBetInfo : BasePPSlotBetInfo
    {
        public int PurchaseType { get; set; }
        public override float TotalBet
        {
            get { return this.BetPerLine * 20.0f; }
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
    class TheAlterEgoGameLogic : BasePPSlotGame
    {
        protected double[] _multiTotalFreeSpinWinRates;
        protected double[] _multiMinFreeSpinWinRates;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vswaysalterego";
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
            get { return 576; }
        }
        protected override int ServerResLineCount
        {
            get { return 20; }
        }
        protected override int ROWS
        {
            get
            {
                return 10;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=4,9,7,5,12,7,12,4,8,9,8,13,9,5,11,3,10,10,4,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3&cfgs=9548&ver=3&def_sb=4,11,4,4,10&reel_set_size=9&def_sa=6,13,9,5,11&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"10989011\",max_rnd_win:\"10000\"}}&wl_i=tbm~10000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1,1,1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;120,40,20,0,0;60,20,10,0,0;50,18,8,0,0;40,15,6,0,0;35,12,5,0,0;35,12,5,0,0;35,12,5,0,0;30,10,4,0,0;30,10,4,0,0;30,10,4,0,0;0,0,0,0,0&total_bet_max=20,000,000.00&reel_set0=8,7,10,4,12,7,7,4,4,4,9,11,6,5,6,11,10,6,6,6,12,5,11,10,9,8,8,13,7,11,11,11,6,10,10,5,10,6,11,6,12,12,12,13,4,5,6,13,4,13,13,13,12,12,13,8,12,11,8,4,6,10,10,10,5,9,5,7,8,4,9,13,8,8,8,4,12,5,12,13,6,10,7,9,9,9,10,8,9,11,5,8,13,11,11,9~8,4,11,6,10,11,8,6,4,4,4,4,8,2,6,6,9,13,2,6,13,8,8,8,8,7,10,12,5,13,6,6,9,4,6,6,6,6,13,11,7,2,8,5,7,4,12,5,5,5,5,4,11,12,9,8,12,6,12,11,11,11,5,9,11,10,5,13,5,7,10,7,7,7,7,12,8,12,5,9,10,13,11,8,13,13,13,13,8,6,6,8,7,12,4,10,6,12,12,12,6,10,8,5,4,7,13,4,11,10,10,10,13,8,11,4,4,7,6,9,4,9,13~4,11,13,12,12,6,5,4,4,4,4,9,13,8,7,6,10,7,7,13,13,13,5,8,8,6,12,9,10,13,9,9,9,13,6,7,8,6,6,12,5,11,11,11,8,11,6,13,9,12,6,11,6,6,6,6,12,13,13,11,8,11,11,5,10,10,10,7,10,10,5,6,5,2,4,12,12,12,4,8,13,12,11,4,10,9,7,7,7,9,13,12,12,10,13,9,6,8,8,8,4,11,9,8,6,7,7,5,5,5,6,6,4,11,4,7,13,5,10~4,11,4,11,12,9,6,10,7,6,9,9,9,9,6,4,6,4,6,7,13,8,9,9,11,7,7,7,7,4,9,13,5,12,5,5,9,6,11,4,12,12,12,12,10,10,13,10,4,12,12,13,6,6,10,5,5,5,12,5,7,8,11,5,8,6,13,8,7,6,6,6,6,8,6,11,6,7,6,12,5,10,10,4,10,10,10,9,13,12,8,13,12,10,8,9,7,12,13,13,13,13,13,7,2,11,8,13,4,8,13,11,9,13,8,8,8,10,4,6,7,13,8,12,5,11,4,9,5,13~4,4,10,7,12,6,6,4,4,4,4,11,7,4,10,8,12,5,9,13,13,13,13,6,5,6,10,11,13,4,8,7,7,7,7,10,13,10,9,9,11,8,5,11,11,11,11,5,10,7,12,13,13,4,12,6,6,6,6,8,8,9,12,6,6,8,12,8,8,8,9,9,13,4,11,7,4,4,12,12,12,12,13,11,10,9,5,8,7,9,9,9,9,5,8,7,8,13,13,10,11,10,10,10,10,13,10,6,12,10,5,11,11,13,7&reel_set2=5,11,5,13,11,9,13,7,5,11,5,13,11,7,11,13,13,5,5,11,5,7,11,13,11,5,9,9,13,5,11,13,9,7,7,13,11,11,13,7,13,7,9,11,9,13,5,9,13,11,9,11,5,11,5,13,11,5,9,13,7,9,5,7,7,5,9,7,13,9,5,7,9,7,13,11,11,9,9,7,9,11,7,5,9,11,13,7,5,11,5,9,5,11,11,9,7,7,9,11~8,6,12,12,4,8,6,4,4,12,8,6,4,10,6,4,12,4,4,4,4,4,10,4,4,10,6,6,4,8,12,12,10,4,12,6,10,12,12,4,4,8,8,8,8,8,12,8,4,8,8,10,8,4,8,8,4,8,10,8,6,10,10,8,12,10,6~5,13,6,6,4,6,2,11,5,9,11,11,5,6,13,11,9,13,12,7,8,10,5,5,5,9,10,5,4,12,4,6,8,13,11,9,8,5,12,4,11,12,10,5,13,11,9,10,11,11,11,5,5,13,6,10,12,2,8,13,12,13,7,9,4,7,10,7,8,12,7,5,7,13,8,4~12,11,7,8,5,10,7,7,4,10,8,5,13,4,8,13,9,5,12,5,10,5,13,9,5,4,13,4,6,7,10,6,5,7,5,13,10,6,10,6,9,11,5,7,13,13,11,2,8,12,12,11,13,12,5,9,10,4,5,11,9,9,12,9,8,11,8,11,9,7,12,8,12,4,2,6,6,4,8,10,13,11,6,13~5,9,4,9,13,10,11,8,6,9,13,7,5,8,13,8,13,11,7,5,10,6,4,10,5,6,12,9,13,6,11,7,10,10,12,10,5,5,11,5,4,10,13,11,13,8,12,9,10,5,8,8,11,12,11,7,10,11,7,12,10,13,13,8,4,12,9,12,4,8,13,12,6,10,7,8,11,12,11,6,4,5,11,6,7,11,8,10,13,6,4,9,11,4,9,8,7,12,13&reel_set1=13,10,6,6,7,5,8,4,12,11,9,4,4,5,13,9,12,6,13,10,5,6,6,5,8,13,6,11,7,8,9,4,5,5,5,7,12,12,5,9,9,12,10,4,6,8,5,6,11,12,10,8,13,8,4,7,9,10,7,11,11,7,4,11,10,11,9,11,12,12,12,10,9,10,8,4,5,9,11,6,5,12,11,9,13,7,12,7,13,4,11,10,13,12,9,12,5,13,4,6,10,4,10,8~10,7,10,13,2,9,4,7,10,6,11,8,4,4,12,9,7,6,11,4,4,4,12,5,8,13,5,4,13,7,9,12,2,10,12,9,8,11,13,5,4,12,5,9,9,9,11,8,6,5,9,4,5,4,9,4,10,11,12,6,13,5,11,6,13,6,8~12,4,12,10,10,12,7,9,5,12,4,9,12,9,4,13,10,7,11,11,8,4,8,5,6,8,13,13,4,13,2,6,4,4,4,7,13,11,4,6,13,9,5,4,5,6,11,9,12,4,13,7,12,9,6,10,12,10,8,6,4,2,12,9,10,11,11,11,10,13,7,7,5,11,7,4,12,6,10,12,12,11,8,5,4,5,9,13,7,11,8,6,7,5,8,9,6,8,13,11,4,12~10,5,11,10,6,5,6,12,5,9,7,5,10,7,12,8,9,13,8,4,8,12,5,4,8,8,9,12,12,11,13,8,10,6,4,5,11,9,8,12,7,11,9,13,7,13,12,4,13,9,11,5,7,9,13,12,4,4,6,11,5,10,6,13,4,11,10,4,12,4,7,4,4,13,6,9,10~7,10,12,11,4,11,4,5,7,13,4,8,11,11,8,11,12,12,10,4,13,11,12,12,10,9,6,9,11,13,7,10,9,12,5,4,6,10,10,13,6,7,6,5,8,11,5,8,13,9,7,12,5,6,9,13,10,8,9&reel_set4=9,6,13,4,6,10,6,13,9,13,10,13,11,6,9,10,6,6,6,5,7,6,12,10,7,7,12,12,13,12,12,8,4,10,8,5,11,5,5,5,4,10,8,5,11,9,10,11,13,11,13,5,10,6,4,9,11,9,12,12,12,10,12,8,6,6,5,4,10,8,11,7,7,9,12,11,13,11,5,13,13,13,5,8,7,13,8,11,12,5,8,4,6,8,6,4,5,7,8,4~5,4,9,13,4,13,13,2,7,7,13,5,13,11,8,8,8,8,7,10,8,4,8,11,6,6,11,12,13,8,6,7,8,9,6,6,6,6,7,4,11,8,8,5,4,6,8,9,2,6,6,5,4,7,7,7,8,12,10,6,5,9,2,6,12,11,12,2,4,7,8,9,12,12,12,10,10,12,13,7,5,8,4,10,6,12,6,4,10,5,11,11,11,8,12,2,11,11,9,5,2,6,13,10,11,13,9,6,4,6~6,7,6,2,13,8,6,6,4,6,2,5,5,13,9,13,2,7,6,6,6,10,10,7,13,6,10,10,13,4,9,7,12,9,4,12,4,12,5,8,12,12,12,8,5,11,12,13,6,13,6,10,11,10,13,7,11,8,7,5,8,4,5,5,5,6,5,11,12,9,6,9,7,8,11,13,10,11,12,9,11,8,11,6,9,7,7,7,12,6,7,6,5,13,5,12,11,6,12,4,13,13,4,11,7,8,10,12,13~11,10,9,7,10,11,9,4,2,11,5,13,7,10,9,9,9,10,7,13,13,6,13,6,10,5,6,4,12,8,6,11,4,7,7,7,7,13,4,8,6,7,8,5,6,9,12,8,7,8,6,5,6,6,6,13,11,4,12,12,13,9,12,4,9,5,10,10,5,6,8,13,13,13,6,7,8,6,11,9,4,13,9,11,4,2,8,13,2,12,12~10,8,9,11,6,4,8,13,4,12,13,9,12,4,4,4,4,12,6,7,13,12,5,4,12,11,6,8,13,4,11,13,13,13,13,12,11,6,4,13,4,10,11,13,10,11,10,7,11,11,11,8,4,5,10,4,8,12,5,10,13,9,5,11,6,6,6,8,13,13,8,4,10,13,9,10,9,11,6,4,9,9,9,8,7,8,5,8,7,6,12,6,9,10,12,11,9,10,10,10,10,9,5,7,10,13,11,7,5,12,8,10,9,7,13,7,10&purInit=[{bet:2000,type:\"default\"},{bet:3000,type:\"default\"},{bet:4000,type:\"default\"}]&reel_set3=4,8,8,10,6,8,10,4,10,6,6,4,12,4,6,4,4,12,4,4,12,10,10,4,4,4,4,4,8,8,12,4,4,6,6,8,6,8,6,12,4,8,12,6,12,8,4,8,4,12,4,12,6,8,8,8,8,8,12,10,8,8,10,8,10,6,4,10,12,8,6,4,8,10,4,4,8,10,10,12,4,12,12,10~5,7,9,11,9,7,11,5,7,7,11,11,7,9,11,7,9,11,13,11,5,13,9,9,11,7,13,11,9,13,13,11,7,11,7,9,5,5,11,5,13,7,11,13,5,9,13,7,11,13,13,11,5,9,13,9,9,5,5,9,5,13,5,5,7~13,5,11,12,7,4,5,7,6,6,8,13,11,4,11,6,13,10,13,11,10,6,11,5,5,8,5,11,13,12,7,5,5,5,4,9,5,5,8,10,7,13,9,5,11,4,12,8,10,12,7,6,9,13,12,9,13,10,4,8,12,9,12,11,8,5,6,11,11,11,13,7,10,5,6,7,8,6,7,10,13,10,12,7,2,6,9,13,11,13,4,5,8,4,9,12,8,2,9,5,12,5,4,11~5,9,8,4,10,11,5,2,5,10,6,13,7,6,5,5,12,9,2,13,4,10,5,8,7,12,8,6,8,13,11,6,12,4,7,10,12,12,4,5,6,13,13,11,13,8,4,12,11,10,9,9,7,9,11,4,13,5,7,8~5,7,8,12,8,11,7,9,11,10,7,7,8,9,7,13,8,12,4,8,4,9,13,9,13,13,10,5,11,6,11,6,5,13,10,5,12,13,6,4,11,12,12,8,6,4,10,5,10,6,12,11,10,10,13,8,11,11&reel_set6=13,6,6,6,13,6,6~6,6,6,6,12,6,13,6,9~6,13,6,6,6,6,13,6,11,6~6,5,10,13,10,9,6,8,11,10,4,9,5,13,12,8,7,6,9,5,11,4,2,8,8,5,9,13,12,6,7,12,6,12,5,10,6,5,6,11,7,10,4,12,11,12,7,9,4,9,13,12,5,10,11,13,2,9,8,4,12,13,6,6,7,11,8,13~10,11,13,7,7,9,6,11,7,8,13,4,13,9,13,12,10,6,4,10,8,11,9,5,4,10,6,6,12,9,10,13,8,5,11,8,10,8,12,4,12,11,11,9,8,6,7,5,11,12,10,5,12,10,11,13,9,4,12,7,6,12,13,9&reel_set5=4,4,4,4,4~4,4,4,4,4,4~4,4,4,4,4,4~9,5,7,11,6,9,9,8,13,4,8,8,4,5,13,10,5,4,9,12,12,7,6,12,7,10,5,4,9,10,13,6,10,13,11,13,12,9,5,13,12,8,10,13,7,4,10,11,8,5,7,6,11,4,12,6,13,2,6,4,7,7,8,11,4,12,4,12,12,10,9,12,5,9,5,6,11,5,8,13,13,4,9,8,2,4,11,10,6,10,12,9,7,5,4,11,12,11,8,4~13,11,6,4,6,11,5,10,6,9,4,4,9,4,4,10,13,10,11,10,11,10,8,10,5,8,5,6,11,11,12,7,4,13,8,12,5,12,12,8,11,9,10,12,9,7,8,13,12,7,9,9,13,10,8,12,8,5,4,7,11,12,7,13,11,5,12,11,7,9,10,13,13,6,6,9&reel_set8=12,11,13,4,8,6,13,4,8,13,8,7,9,6,11,13,11,5,13,11,4,4,4,10,13,9,11,12,12,6,5,4,5,10,4,8,11,9,9,12,7,9,6,6,10,13,13,13,11,10,8,6,7,5,10,12,5,10,5,8,6,4,13,10,5,8,4,7,12,7~12,7,8,4,5,12,6,8,13,8,9,10,9,5,6,10,11,7,10,9,9,13,4,4,11,11,8,12,5,10,5,5,5,7,13,5,11,11,13,4,8,12,5,10,11,11,7,10,8,10,13,6,11,5,8,9,10,13,11,13,2,5,4,4,5,8,8,8,12,5,4,13,6,8,4,8,6,12,8,4,5,7,9,9,12,5,6,6,9,13,8,5,12,4,7,6,7,6,2,5,12~10,6,5,8,9,11,4,4,11,13,13,7,12,6,12,10,8,13,4,13,13,5,5,5,4,10,7,9,13,7,9,9,10,5,6,8,6,5,9,13,11,8,4,12,11,12,11,11,11,13,5,5,8,8,12,2,10,7,12,6,5,11,12,5,7,11,5,6,5,11,7~7,10,13,4,5,4,13,9,10,9,9,5,6,2,11,11,5,11,8,12,13,6,7,5,8,5,8,9,8,8,13,11,12,10,5,2,12,13,13,11,4,13,9,9,4,7,13,4,10,4,7,12,11,12,10,6,12,10,6,6,5,8,5,7~11,7,10,8,10,13,11,4,11,13,13,9,13,5,8,13,9,5,7,8,9,10,12,12,8,6,10,6,5,12,11,11,9,11,12,6,7,10,5,13,8,6,4,8,5,5,7,6,7,8,12,10,7,4,12,5,10,13,11,11,7,11,10,7,10,4,8,4,6,4,5,5,4,6,9,11,13,11,13,10,11,13,9,8,12,8,12,9,12,6,10,13&reel_set7=7,7,7,13,7,7~13,7,7,7,7,12,7,9,7~7,11,7,7,7,7,13,13,7~9,8,5,6,10,6,9,9,11,2,11,7,12,5,12,9,8,11,10,13,11,13,12,8,7,4,6,9,5,4,13,5,12,7,7,13,8,4,2,6,4,11,12,12,13,6,9,12,7,10,10,6,7,8,10,7,10,8,4,5,4,12,5,7,13,5,13,11,7,9~12,7,9,12,8,6,9,9,5,9,8,11,12,7,4,9,12,10,11,11,7,13,12,7,6,10,13,13,8,6,6,12,7,13,5,13,11,10,4,8,9,10,13,5,9,11,8,12,9,4,10,12,13,5,9,7,11,9,10,8,10,11,10,7,11,10,11,6,4,9,13,5,4,6,10,11,13,12,11,5,10,4,4,5,12,6,9,12,4,8,10,11,7,13,5,6,4,7,7,11,11,5,12,10,12,6,13,8&total_bet_min=10.00";
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
        protected int FreePurCount
        {
            get { return 3; }
        }
        public double[] PurchaseMultiples
        {
            get { return new double[] { 100, 150, 200 }; }
        }

        #endregion
        public TheAlterEgoGameLogic()
        {
            _gameID = GAMEID.TheAlterEgo;
            GameName = "TheAlterEgo";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
        }
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as TheAlterEgoBetInfo).PurchaseType;
            return this.PurchaseMultiples[purchaseType];
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("rs_iw"))
                dicParams["rs_iw"] = convertWinByBet(dicParams["rs_iw"], currentBet);

            if (dicParams.ContainsKey("rs_win"))
                dicParams["rs_win"] = convertWinByBet(dicParams["rs_win"], currentBet);

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
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                TheAlterEgoBetInfo betInfo  = new TheAlterEgoBetInfo();
                betInfo.BetPerLine          = (float)message.Pop();
                betInfo.LineCount           = (int)message.Pop();
		
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in TheAlterEgoGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                    (oldBetInfo as TheAlterEgoBetInfo).PurchaseType = betInfo.PurchaseType;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in TheAlterEgoGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            TheAlterEgoBetInfo betInfo = new TheAlterEgoBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBet     = (double)infoDocument["defaultbet"];
                _normalMaxID            = (int)infoDocument["normalmaxid"];
                _emptySpinCount         = (int)infoDocument["emptycount"];
                _naturalSpinCount       = (int)infoDocument["normalselectcount"];

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
        protected override async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int agentID, BasePPSlotBetInfo betInfo, double baseBet, UserBonus userBonus)
        {
            int purchaseType = (betInfo as TheAlterEgoBetInfo).PurchaseType;
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
        protected override async Task<BasePPSlotSpinData> selectMinStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                BsonDocument spinDataDocument = null;
                int purchaseType = (betInfo as TheAlterEgoBetInfo).PurchaseType;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, -1, PurchaseMultiples[purchaseType] * 0.2, PurchaseMultiples[purchaseType] * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in TheAlterEgoGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int purchaseType = (betInfo as TheAlterEgoBetInfo).PurchaseType;
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in TheAlterEgoGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = (betInfo as TheAlterEgoBetInfo).PurchaseType.ToString();
        }
        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            TheAlterEgoBetInfo alterBetInfo = betInfo as TheAlterEgoBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, -1, betInfo.PurchaseFree ? alterBetInfo.PurchaseType : -1, betMoney);
        }
    }
}
