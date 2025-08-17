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
    class CowboyCoinsBetInfo : BasePPSlotBetInfo
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

    class CowboyCoinsGameLogic : BasePPSlotGame
    {
        protected double[]                          _multiTotalFreeSpinWinRates;
        protected double[]                          _multiMinFreeSpinWinRates;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vswaysultrcoin";
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
                return "def_s=5,9,1,5,7,12,6,15,15,4,3,11,8,10,5,8,7,9,13,11,6,3,4,13&cfgs=7274&ver=3&mo_s=16;17;18;19;15&mo_v=1,2,3,4,5,6,7,8,9,10;1,2,3,4,5,6,7,8,9,10;1,2,3,4,5,6,7,8,9,10;1,2,3,4,5,6,7,8,9,10;1,2,3,4,5,6,7,8,9,10&def_sb=6,10,6,9,5,10&reel_set_size=4&def_sa=3,10,11,15,8,17&scatters=1~0,0,0,0,0,0~0,0,0,0,0,0~1,1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"9896275\",max_rnd_win:\"30000\"}}&wl_i=tbm~30000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1,1,1,1,1&wilds=2~0,0,0,0,0,0~1,1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;200,125,80,40,0,0;120,80,60,30,0,0;90,65,50,20,0,0;75,50,40,15,0,0;60,40,30,12,0,0;40,25,12,7,0,0;40,25,12,7,0,0;25,12,7,3,0,0;25,12,7,3,0,0;25,12,7,3,0,0;0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0&total_bet_max=50,000,000.00&reel_set0=11,4,10,9,8,7,9,9,11,8,8,5,11,12,11,8,11,10,9,11,11,5,10,3,6,7,11,11,7,12,3,4,6,10,8,3,7,10,9,10,9,8,12,5,5,8,10,12,9,3,8,5,12,10,5,7,12,10,12,11,6,17,11,7,4,12,17,6,6,12,11,10,17,17,17,17,17,17,11,12,4,10,11,7,4,12,5,7,9,4,9,5,7,5,10,7,11,9,11,6,7,12,7,12,3,7,4,10,6,6,3,8,8,5,9,9,12,12,3,11,12,4,3,11,5,10,6,10,6,12,8,11,6,9,8,5,5,4,9,8,11,3,6,6,10,12,8,6,10,10,5,7~8,9,11,3,8,9,10,1,12,12,7,8,4,6,11,12,10,11,7,8,12,10,10,12,1,15,11,8,10,6,6,4,15,11,11,6,5,10,6,7,4,5,5,7,8,1,10,7,8,11,12,15,10,9,7,7,12,4,15,6,9,12,4,8,10,10,6,5,3,6,4,12,6,5,6,3,5,11,15,7,4,5,10,5,4,9,12,9,9,6,3,9,15,15,15,15,15,15,15,15,7,4,10,8,3,12,6,12,9,11,11,5,8,3,9,11,8,8,10,6,3,7,9,7,5,12,5,4,7,10,9,3,3,11,9,12,8,4,5,9,15,8,7,4,12,3,8,7,8,1,10,12,4,5,12,10,10,9,12,4,12,9,9,1,5,10,11,10,6,11,12,5,6,10,9,11,11,15,7,11,8,10,9,11,15,3,12,5,7,10,12,6,3,8,9~12,11,9,15,12,3,5,15,9,8,6,11,6,7,6,12,6,3,4,8,4,7,11,7,3,7,4,3,11,10,10,12,12,8,12,10,12,8,4,11,10,5,9,9,11,8,6,12,4,5,10,15,15,15,15,15,15,15,15,10,9,1,6,15,5,10,9,8,11,12,4,9,11,4,1,6,4,6,7,3,5,11,4,9,7,8,8,12,7,11,7,6,10,3,9,7,9,1,5,10,9,11,5,10,15,3,5,11,12,10,8,11~8,10,4,5,12,9,7,7,10,3,3,12,12,11,12,5,1,10,9,9,11,12,9,5,6,6,7,5,8,8,11,15,5,9,11,10,10,12,6,4,15,4,9,3,11,6,11,3,9,8,12,6,5,7,10,7,12,10,10,15,12,4,10,10,9,6,7,5,4,15,15,15,15,15,15,15,15,11,7,10,8,12,4,12,10,7,12,6,6,3,8,11,11,8,5,10,5,8,5,4,3,11,11,9,7,3,8,11,5,9,12,6,1,5,8,6,4,5,8,10,11,10,6,9,8,6,9,8,3,8,12,15,9,1,3,11,7,10,9,7,11,12,15,5,4,8,11,10,11,9~4,12,4,8,9,5,8,7,3,15,11,9,7,11,5,11,9,12,11,7,15,3,4,6,1,12,11,9,11,8,12,7,15,11,6,6,12,11,10,10,4,8,3,7,11,6,9,10,8,7,9,3,6,7,8,10,8,10,15,15,15,15,15,15,15,15,5,8,10,12,11,10,12,12,5,7,4,9,6,3,11,12,6,5,6,5,6,12,3,6,9,9,5,12,7,12,9,12,5,3,5,10,9,1,4,7,11,8,8,10,3,9,11,15,4,15,11,10,4,11,5,8,10,11,10,12~11,12,5,8,9,11,6,5,9,6,10,11,5,12,8,6,6,7,11,3,8,9,12,10,7,9,12,12,3,5,10,8,4,10,11,4,8,8,7,10,9,7,3,4,12,7,4,11,16,8,12,5,9,7,3,16,16,16,16,16,16,5,11,6,8,9,12,5,6,8,6,7,12,6,9,8,3,10,6,9,10,11,4,6,6,3,8,8,7,4,4,9,12,9,12,12,11,7,5,7,11,10,12,10,10,4,5,8,16,3,6,4,10,6,10,11,7,11&reel_set2=5,11,6,12,10,9,8,5,9,12,5,12,10,8,9,5,12,12,10,12,12,6,9,6,11,6,11,6,11,12,10,7,11,17,9,7,6,11,5,8,9,11,11,4,6,4,5,7,10,12,9,8,8,9,10,12,3,9,8,11,7,10,6,17,17,17,17,17,17,11,11,4,8,5,7,10,17,3,5,3,4,3,4,11,5,3,10,9,6,8,11,6,7,7,3,10,7,11,9,10,7,4,5,10,11,11,12,11,3,7,8,12,10,7,6,12,4,8,12,4,5,8,8,11,7,10,10,9,10,3,12,5,6,12~8,3,1,7,1,5,9,9,12,10,1,11,1,9,1,3,4,1,6,10,8,6,12,1,12,9,10,8,8,10,9,12,10,12,1,11,1,10,6,11,11,1,7,8,1,8,10,4,1,10,9,8,8,1,5,1,4,7,12,10,1,3,9,1,11,11,1,12,5,1,3,9,6,6,12,4,7,5,12,8,1,6,7,4,4,9,7,4,5,12,4,6,1,7,11,10,1,5,5,11~3,11,1,11,12,7,4,6,1,7,11,9,1,10,1,8,11,8,4,10,1,5,12,3,5,9,1,4,4,1,9,6,8,1,12,3,12,8,1,6,11,6,8,9,6,1,11,10,1,6,11,9,7,1,12,1,5,9,7,6,12,7,3,4,1,7,9,1,5,4,5,11,10,9,12,10,11,1,8,11,4,4,7,1,12,5,3,10,6,10,12,3,1,10,12,1,5,1,8,1,11~12,8,11,4,9,1,3,11,3,1,4,1,5,10,9,1,9,1,7,1,4,3,5,5,1,7,11,8,3,5,1,6,1,7,7,9,1,3,11,8,11,1,10,6,3,5,8,10,7,10,8,9,3,8,1,10,11,4,1,9,11,10,7,1,12,5,12,8,7,6,10,5,11,5,10,6,1,10,1,4,11,4,1,3,1,11,1,3,1,9,1,8,8,1,11,12,4,12,7,12,12,6,12,9,1,10,1,11,4,9,12,10,1,12,1,10,10,1,10,10,1,10,1,5,1,9,4,7,10,6,9,10,1,5,1,12,11,6,6,12,8,1,5,12,6,12,1,8,8,5,7,1,11,11,10,7,11,9~12,10,9,1,3,1,7,1,9,6,12,6,1,11,11,6,5,12,9,1,12,7,3,8,9,1,8,5,10,12,11,1,5,1,4,10,1,4,7,8,10,6,8,9,9,3,7,7,4,3,1,12,8,8,10,7,9,1,5,4,10,1,11,12,8,1,9,7,1,11,1,10,11,1,10,4,1,8,12,6,9,12,9,1,4,1,9,4,9,5,4,5,11,1,12,1,8,1,10,10,8,12,10,1,10,3,1,10,12,1,4,11,7,8,7,1,12,1,11,11,4,4,6,1,6,12,11,6,8,6,5,1,5,5,1,10~6,5,16,11,10,4,10,11,9,8,7,4,9,6,3,10,7,11,11,8,8,9,5,5,11,10,10,3,12,4,6,4,12,12,10,6,8,10,4,12,11,4,9,8,8,3,6,6,9,5,8,3,4,6,16,12,12,6,9,16,16,16,16,16,16,7,12,4,8,12,12,5,10,6,11,10,7,5,11,4,11,12,9,7,3,7,7,10,7,8,8,10,5,12,11,11,8,6,3,9,9,12,6,12,5,8,12,6,3,10,5,10,6,8,11,9,8,11,7,6,9,7,5,4,3,7&reel_set1=12,4,3,12,3,9,8,3,11,12,6,9,10,8,8,9,10,5,11,10,10,7,5,4,10,9,9,10,7,7,6,11,12,6,4,4,12,7,12,5,8,6,6,11,5,10,7,9,7,7,12,8,6,12,5,9,6,8,11,4,12,5,4,7,6,5,11,7,6,8,5,8,8,12,10,12,6,10,12,11,9,11,12,10,12,4,6,4,7,6,11,3,7,9,12,8,6,6,4,4,10,11,5,4,11,11,8,3,12,12,3,10,6,7,8,11,10,7,4,9,5,11,3,3,8,8,11,12,9,10,6,10,9,3,7,5,9,11,5,10,10,9,5,7,11,5,6,9,8~4,9,1,6,11,4,10,6,12,1,5,1,11,12,1,5,1,8,1,8,1,5,9,11,11,10,8,11,10,6,9,9,1,5,1,10,4,1,11,8,7,1,10,5,8,4,1,12,3,4,1,5,5,4,10,1,8,3,8,9,12,10,1,9,7,9,1,10,8,8,10,1,6,7,1,10,9,12,3,5,1,12,11,7,7,4,1,11,1,12,12,1,12,8,5,7,6,1,7,7,5,12,1,3,8,10,10,9,6,11,3,11,1,6,1,7,4,9,1,8,9,4,8,8,9,4,3,4,1,6,10,1,7,12,10,9,10,7,8,1,4,12,1,6,9,12,11,6,1,12,11,5,4,11,10,1,12,9,1,12,1,6~10,5,7,1,12,3,1,4,11,12,12,4,4,1,9,1,6,12,1,8,1,5,11,4,7,11,1,10,9,11,9,3,7,4,12,4,11,4,1,10,12,1,5,1,9,11,1,10,1,11,4,6,4,1,10,4,12,1,9,9,10,1,12,7,1,3,1,11,12,1,10,1,10,5,3,4,5,8,12,12,11,5,9,8,1,7,3,1,6,8,3,5,11,10,12,1,12,6,4,8,6,6,9,6,12,1,8,10,11,5,1,7,1,4,1,5,1,10,7,11,9,12,11,9,8,7,6,1,11,8,10,11,1,8,6,6,7,7,6,1,6,8,5,1,9,8,11,1,7,11,1,3,1,5,12,1,11,3,12,1,3,1,10,1,9,6,9~5,11,11,9,11,1,5,11,7,10,7,12,6,3,4,1,12,10,4,1,8,12,5,3,9,1,6,9,12,10,1,9,12,1,9,1,7,4,1,10,11,7,11,4,1,10,10,8,7,12,1,8,1,5,7,1,11,1,11,1,5,1,11,1,5,12,9,7,1,5,6,1,10,8,1,10,6,5,4,11,11,1,5,6,3,1,7,1,10,8,9,1,9,1,5,3,1,7,1,4,9,1,12,8,9,8,10,1,8,1,10,12,10,1,5,12,10,1,9,11,11,10,3,1,3,6,1,12,1,4,10,12,1,8,3,8,11,8,8,4,7,1,10,5,11,1,6,12,1,3,6,10,9,10~15,8,5,9,1,6,7,5,9,12,5,9,8,10,7,11,4,10,3,1,3,4,4,8,4,6,5,15,8,8,12,7,11,15,11,10,11,11,7,6,3,12,7,12,12,3,5,9,10,11,12,3,15,15,15,15,10,8,9,9,8,12,8,6,11,7,9,12,11,7,11,12,15,8,7,6,10,15,1,11,4,5,5,10,7,11,3,11,10,5,12,4,4,6,12,6,9,3,9,5,11,3,6,8,10,6,10,12,11,10,9~4,7,8,6,7,10,12,11,6,10,11,11,7,6,5,12,7,6,7,12,5,4,12,3,6,5,8,12,9,11,10,7,9,9,3,5,9,11,11,4,10,8,3,3,12,8,10,6,7,11,4,6,6,12,10,5,11,5,12,11,10,5,10,3,10,10,6,9,7,7,12,3,6,12,3,3,9,4,11,12,5,10,7,12,4,9,9,9,11,8,8,11,6,3,8,5,5,12,11,9,8,6,8,7,6,10,10,7,12,8,8,6,9,9,6,4,5,9,8,9,11,6,11,12,8,4,9,11,12,11,10,5,3,12,5,5,8,11,10,7,11,4,11,7,8,9,10,10,12,10,12,8,7,9,9,11,12,7,11,9,9,10,7,5,11,9,10,4,12,7,8,9,6,11,7,12&purInit=[{bet:2000},{bet:10000},{bet:10000},{bet:10000},{bet:10000}]&reel_set3=5,11,6,12,10,9,8,5,9,12,5,12,10,8,9,5,12,12,10,12,12,6,9,6,11,6,11,6,11,12,10,7,11,17,9,7,6,11,5,8,9,11,11,4,6,4,5,7,10,12,9,8,8,9,10,12,3,9,8,11,7,10,6,17,17,17,17,17,17,11,11,4,8,5,7,10,17,3,5,3,4,3,4,11,5,3,10,9,6,8,11,6,7,7,3,10,7,11,9,10,7,4,5,10,11,11,12,11,3,7,8,12,10,7,6,12,4,8,12,4,5,8,8,11,7,10,10,9,10,3,12,5,6,12~6,5,10,10,11,3,15,6,11,9,3,12,8,12,11,5,7,3,15,12,4,7,11,12,10,4,5,15,10,10,8,8,5,12,5,6,8,9,10,5,9,12,11,10,9,6,12,6,12,4,5,10,3,7,11,8,11,15,11,4,5,12,8,8,4,9,3,15,8,12,4,15,15,15,15,11,4,9,7,12,7,6,10,11,15,10,10,9,8,9,8,3,9,9,6,8,6,12,11,5,4,8,10,12,4,6,4,3,12,7,8,10,8,3,11,5,7,9,7,7,11,7,7,11,6,12,3,10,9,5,10,9,8,10,11,12,9,12,10,5,5,7,5,6,6,4,10,9~5,10,3,11,5,6,10,11,9,7,3,11,7,6,5,6,7,6,8,3,3,9,10,12,8,9,9,11,3,3,11,10,8,5,11,11,5,5,4,11,7,12,9,10,10,8,12,4,6,4,8,11,12,12,10,9,12,11,9,15,12,9,10,12,5,4,7,15,4,10,10,12,8,7,15,15,15,15,11,5,9,9,4,5,6,11,10,9,7,12,9,8,15,7,15,8,11,6,7,7,4,12,3,9,4,8,3,5,9,6,11,6,9,3,7,15,4,4,8,5,12,6,10,12,8,10,3,10,8,5,10,9,4,12,3,12,7,15,4,11,6,6,12,7,5,9,11,11,15,11,10,8,11,6,4~11,3,5,10,6,5,6,15,10,10,11,6,12,5,11,7,12,11,3,8,7,5,10,6,11,6,3,10,8,5,4,9,11,5,10,9,4,11,9,5,8,3,11,12,7,10,8,9,7,5,5,7,15,5,6,12,5,9,8,7,4,9,6,7,8,9,8,4,9,10,6,12,8,4,5,6,12,15,15,15,15,9,7,12,11,10,10,6,11,12,10,8,5,5,11,7,4,4,3,10,10,15,4,7,6,9,11,15,7,3,15,11,11,6,6,12,10,12,15,3,12,12,4,5,3,12,9,12,10,12,12,4,7,11,9,3,10,8,9,7,9,11,10,10,12,8,8,11,10,11,8,3,9,8,11,12,8,15,8,9~6,9,10,12,8,12,7,9,12,11,10,7,5,10,4,8,7,4,7,3,10,9,7,9,11,5,4,11,6,9,15,3,11,8,6,11,3,10,12,3,8,10,8,10,6,10,10,4,4,9,10,12,6,12,11,7,4,4,12,7,9,8,11,8,9,5,5,9,4,15,15,15,15,7,15,8,5,5,12,10,11,6,3,12,12,8,9,12,11,11,15,5,9,6,5,10,5,3,11,8,15,6,6,11,8,8,3,10,9,12,11,6,12,9,11,11,9,12,12,7,3,11,11,4,8,5,10,7,12,3,5,7,6,10,15,7,3,9,15,6,5,3,11,11~6,5,16,11,10,4,10,11,9,8,7,4,9,6,3,10,7,11,11,8,8,9,5,5,11,10,10,3,12,4,6,4,12,12,10,6,8,10,4,12,11,4,9,8,8,3,6,6,9,5,8,3,4,6,16,12,12,6,9,16,16,16,16,16,16,7,12,4,8,12,12,5,10,6,11,10,7,5,11,4,11,12,9,7,3,7,7,10,7,8,8,10,5,12,11,11,8,6,3,9,9,12,6,12,5,8,12,6,3,10,5,10,6,8,11,9,8,11,7,6,9,7,5,4,3,7&total_bet_min=10.00";
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
            get { return 5; }
        }

        public double[] PurchaseMultiples
        {
            get { return new double[] { 100, 500,500,500,500 }; }
        }

        #endregion
        public CowboyCoinsGameLogic()
        {
            _gameID  = GAMEID.CowboyCoins;
            GameName = "CowboyCoins";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["g"] = "{fg:{def_s:\"14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14\",def_sa:\"14,14,14,14,14,14\",def_sb:\"14,14,14,14,14,14\",s:\"14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14\",sa:\"14,14,14,14,14,14\",sb:\"14,14,14,14,14,14\",sh:\"6\",st:\"rect\",sw:\"6\"}}";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "6";
        }
        protected override void supplementInitResult(Dictionary<string, string> dicParams, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {
            base.supplementInitResult(dicParams, betInfo, spinResult);
            if(!dicParams.ContainsKey("reel_set"))
                dicParams["reel_set"] = "0";
            if(!dicParams.ContainsKey("g"))
                dicParams["g"] = "{fg:{def_s:\"14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14\",def_sa:\"14,14,14,14,14,14\",def_sb:\"14,14,14,14,14,14\",s:\"14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14\",sa:\"14,14,14,14,14,14\",sb:\"14,14,14,14,14,14\",sh:\"6\",st:\"rect\",sw:\"6\"}}";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("rs_iw"))
                dicParams["rs_iw"] = convertWinByBet(dicParams["rs_iw"], currentBet);

            if (dicParams.ContainsKey("rs_win"))
                dicParams["rs_win"] = convertWinByBet(dicParams["rs_win"], currentBet);

            if (dicParams.ContainsKey("mo_tw"))
                dicParams["mo_tw"] = convertWinByBet(dicParams["mo_tw"], currentBet);

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
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as CowboyCoinsBetInfo).PurchaseType;
            return this.PurchaseMultiples[purchaseType];
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


                CowboyCoinsBetInfo betInfo  = new CowboyCoinsBetInfo();
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in CowboyCoinsGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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

                    oldBetInfo.BetPerLine       = betInfo.BetPerLine;
                    oldBetInfo.LineCount        = betInfo.LineCount;
                    oldBetInfo.MoreBet          = betInfo.MoreBet;
                    oldBetInfo.PurchaseFree     = betInfo.PurchaseFree;
                    (oldBetInfo as CowboyCoinsBetInfo).PurchaseType = betInfo.PurchaseType;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in CowboyCoinsGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            CowboyCoinsBetInfo betInfo = new CowboyCoinsBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new CowboyCoinsBetInfo();
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
            int purchaseType    = (betInfo as CowboyCoinsBetInfo).PurchaseType;
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
                int purchaseType = (betInfo as CowboyCoinsBetInfo).PurchaseType;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, -1, PurchaseMultiples[purchaseType] * 0.2, PurchaseMultiples[purchaseType] * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in CowboyCoinsGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int purchaseType = (betInfo as CowboyCoinsBetInfo).PurchaseType;
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in CowboyCoinsGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }

        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = (betInfo as CowboyCoinsBetInfo).PurchaseType.ToString();
        }
        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            CowboyCoinsBetInfo cowboyBetInfo = betInfo as CowboyCoinsBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, -1, betInfo.PurchaseFree ? cowboyBetInfo.PurchaseType : -1, betMoney);
        }
        protected override void sendGameResult(BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult, string strUserID, int agentID, double betMoney, double winMoney, string strGameLog, double userBalance, int index, int counter, UserBetTypes betType, Currencies currency, PPFreeSpinInfo freeSpinInfo)
        {
            string strSpinResult = "";
            if (freeSpinInfo != null && !freeSpinInfo.Pending)
                strSpinResult = makeSpinResultString(betInfo, spinResult, 0.0, userBalance, index, counter, false);
            else
                strSpinResult = makeSpinResultString(betInfo, spinResult, betMoney, userBalance, index, counter, false);

            GITMessage message = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOSPIN);
            message.Append(strSpinResult);

            ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, message, betMoney, winMoney, new GameLogInfo(GameName, "0", strGameLog), betType);

            string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
            if (_isRewardedBonus)
            {
                toUserResult.setBonusReward(_rewardedBonusMoney);
                toUserResult.insertFirstMessage(_bonusSendMessage);
            }
            if (freeSpinInfo != null && !freeSpinInfo.Pending)
                toUserResult.FreeSpinID = freeSpinInfo.FreeSpinID;

            toUserResult.RoundID = betInfo.RoundID;
            toUserResult.BetTransactionID = betInfo.BetTransactionID;
            if (_dicUserHistory.ContainsKey(strGlobalUserID))
            {
                if (_dicUserHistory[strGlobalUserID].log.Count == 0)
                    _dicUserHistory[strGlobalUserID].bet = betMoney;
            }

            //빈스핀인 경우에 히스토리보관을 여기서 진행한다.
            if (addSpinResultToHistory(strGlobalUserID, index, counter, strSpinResult, betInfo, spinResult))
            {
                saveHistory(agentID, strUserID, index, counter, userBalance - betMoney, currency);
                toUserResult.TransactionID = createTransactionID();
                if (freeSpinInfo != null && !freeSpinInfo.Pending)
                    checkFreeSpinCompletion(message, freeSpinInfo, strGlobalUserID);
            }
            else if (spinResult.NextAction == ActionTypes.DOCOLLECT || spinResult.NextAction == ActionTypes.DOCOLLECTBONUS)
            {
                toUserResult.TransactionID = createTransactionID();
                if (freeSpinInfo != null && !freeSpinInfo.Pending)
                    addFreeSpinBonusParams(message, freeSpinInfo, strGlobalUserID, winMoney);
            }
            var dicParams = splitResponseToParams(strSpinResult);
            if (dicParams.ContainsKey("rs_t"))
                Context.System.Scheduler.ScheduleTellOnce(100, Sender, toUserResult, Self);
            else
                Sender.Tell(toUserResult, Self);
        }
    }
}
