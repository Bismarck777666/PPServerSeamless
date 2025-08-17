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
    class StarlightPrincessPachiBetInfo : BasePPSlotBetInfo
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
    class StarlightPrincessPachiGameLogic : BasePPSlotGame
    {
        protected double[] _multiTotalFreeSpinWinRates;
        protected double[] _multiMinFreeSpinWinRates;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vswaysjapan";
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
                return "def_s=5,6,7,6,4,11,4,5,8&reel_set25=7,7,11,1,7,4,4,3,8,1,7,7,10,3,8,11,5,1,3,6,1,6,11,7,3,9,1,5,9,4,8,10,3,6,1,10,9,7,11,11,8,1,5,1,9,5,4,5,9,8,10,8,4,10,1,6,1,11,6,7,9,6,9,1,7,1,10,6,10,3,10,1,9,10,4~10,7,8,5,9,8,6,7,8,6,10,7,9,1,9,5,7,4,6,10,4,3,1,5,7,1,7,10,5,7,10,5,9,7,6,3,4,10,3,8,5,9,8,7,8,7,8,5,9,3,8,10,4,9,8,6,5,6,8,10,1,7,9,10,5,1,10,9,5,6,1,3~6,5,9,6,5,11,9,8,5,11,7,6,10,3,11,5,8,6,7,3,6,11,11,11,7,4,10,5,10,8,9,4,9,10,8,11,5,7,10,3,4,8,10&reel_set26=8,7,1,4,11,10,4,11,5,1,5,1,10,9,8,1,7,1,10,1,7,8~6,1,4,5,8,6,10,7,6,8,10,7,5,10,7,8,3,5,1,8,9,6,3,8,1,9,7,5,6,8,9,5,7,5,9,3,7,4,9,8,10,9,4,10~1,11,9,4,1,3,10,7,8,7,6,3,8,5,7,1,9,1,11,9,7,1,4,1,6,1,5,8&reel_set27=12,12,12,12,12~12,12,12,12,12~12,12,12,12,12&cfgs=12037&ver=3&def_sb=4,4,9&reel_set_size=28&def_sa=5,3,10&scatters=1~0,0,0~0,0,0~1,1,1&rt=d&gameInfo={rtps:{purchase_1:\"94.04\",purchase_0:\"94.04\",regular:\"94.04\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"1009693\",max_rnd_win:\"5000\"}}&wl_i=tbm~5000&reel_set10=6,8,9,7,4,7,4,6,7,7,7,3,11,7,9,3,11,8,6,6,11,8,8,8,6,5,7,7,3,1,9,10,6,6,6,5,7,6,5,9,7,7,5,10,8,11,11,11,5,5,8,7,7,11,10,6,6,4,8~9,7,5,11,8,5,8,8,7,7,7,5,6,11,8,9,5,7,3,11,7,8,8,8,10,6,10,5,6,4,7,6,7,4,6,6,6,10,9,9,7,11,6,5,3,7,6,1,11,11,11,3,7,8,8,6,7,6,7,5,6,4,9~6,6,9,6,8,7,7,8,10,7,7,7,9,4,7,10,11,6,5,9,6,4,7,8,8,8,5,5,1,8,11,6,5,5,6,5,6,6,6,8,6,11,7,5,11,9,8,3,7,6,11,11,11,7,3,9,8,3,7,8,7,10,4,7,7&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&reel_set11=6,7,10,9,10,9,9,9,10,10,8,10,8,7,9,8,9,10,10,10,9,8,10,4,11,4,8,11,8,8,8,10,8,8,9,9,7,10,9,9,11,11,11,1,5,8,6,11,9,1,11,5,9~10,9,10,8,9,11,10,10,8,9,8,9,9,9,11,9,10,8,7,7,4,9,9,10,9,9,10,10,10,11,10,8,5,11,9,1,9,6,8,10,1,11,8,8,8,9,8,8,9,9,10,4,10,8,9,9,6,11,11,11,10,8,7,8,8,10,9,11,6,9,7,9,5,10~8,10,1,10,4,9,8,9,9,9,7,6,8,1,9,9,5,8,10,10,10,8,9,9,11,11,9,8,5,8,8,8,9,10,10,7,9,9,8,7,4,11,11,11,9,9,11,8,10,10,6,11,10,9&reel_set12=9,11,9,11,7,9,10,9,8,10,10,11,10,10,10,8,7,11,9,10,1,9,11,9,6,10,9,9,9,11,8,10,8,8,11,11,10,1,6,10,7,9,9,11,11,11,8,9,11,6,10,6,9,10,8,10,10,7,9,1,11~10,9,1,9,7,11,10,9,10,7,9,7,10,9,10,10,10,1,10,10,11,11,10,10,9,1,11,9,6,8,8,11,6,9,9,9,7,9,8,11,9,8,11,9,10,9,11,10,6,10,9,8,11,11,11,8,11,10,11,8,10,9,8,9,10,11,7,11,8,10,6,9,9~10,9,11,11,9,11,10,9,7,8,10,10,10,7,10,10,11,10,1,6,9,8,9,7,9,9,9,6,11,9,8,7,6,10,11,10,11,1,10,11,11,11,9,10,10,8,9,9,11,8,10,9,8,10&purInit_e=1,1&reel_set13=9,10,11,11,7,8,10,11,10,9,11,10,10,10,11,11,9,10,7,8,11,10,10,8,11,9,10,11,11,11,10,10,9,9,11,10,9,9,11,1,9,11,9,9,9,8,9,10,10,11,1,11,10,9,9,11,10,10,8,11~9,10,10,9,10,9,9,10,11,8,9,9,10,10,10,11,7,11,1,11,11,1,11,9,10,11,10,9,8,11,11,11,7,10,10,9,10,11,10,10,8,11,11,10,11,9,9,9,11,8,11,9,10,11,10,11,8,10,10,9,10,9,11~9,11,11,7,9,11,10,10,8,10,10,10,11,10,11,10,10,8,9,10,10,9,9,11,11,11,1,11,1,11,11,10,9,10,11,11,9,9,9,11,8,7,9,10,11,8,10,10,11,10,9&wilds=2~0,0,0~1,1,1&bonuses=0&reel_set18=11,10,10,9,11,10,11,11,8,10,11,10,10,10,8,11,10,9,10,11,10,11,7,10,11,9,11,11,11,8,9,10,10,9,9,8,10,11,9,10,9,9,9,10,11,7,11,11,9,10,9,8,9,11,9,9,10~8,10,10,8,10,11,11,10,10,8,9,10,10,10,11,9,7,11,9,10,10,7,11,10,11,10,11,11,11,10,9,8,11,9,9,11,8,10,9,9,9,11,11,9,10,11,9,9,10,11,9,10,10,11,11~11,8,11,11,10,10,11,9,10,11,9,10,7,10,10,10,9,11,11,10,9,10,11,10,11,11,10,11,11,9,9,11,11,11,10,10,8,11,11,9,7,10,11,8,10,9,9,10,9,9,9,11,11,10,8,9,9,10,11,10,8,9,10,9,8,10&reel_set19=5,1,6,1,8,11,10,4,1,4,3~11,8,11,7,5,10,1,10,9,10,1,9~1,10,9,10,1,6,1,8,11,7,6,7,5,8,1,4&ntp=0.00&reel_set14=7,3,8,5,6,5,4,3,3,3,4,6,8,3,10,7,3,7,7,7,3,7,8,8,5,9,6,5,5,5,4,10,9,3,4,3,6,3,4,5~3,6,8,3,9,10,5,7,6,4,9,3,3,3,5,3,8,3,4,6,10,3,8,5,4,7,7,7,4,5,7,3,3,8,10,3,5,3,7,6,5,5,5,7,4,9,4,5,4,3,4,3,8,3,5,7,6~3,3,5,7,4,4,3,8,10,3,8,5,6,3,3,3,9,5,3,7,3,4,3,6,4,8,4,7,5,6,7,7,7,4,3,4,3,7,10,8,6,5,3,7,5,9,4,5,5,5,3,10,6,3,3,8,5,8,3,3,4,7,3,5,6,9&paytable=0,0,0;0,0,0;0,0,0;200,0,0;50,0,0;20,0,0;10,0,0;8,0,0;6,0,0;4,0,0;2,0,0;1,0,0;0,0,0&reel_set15=10,11,6,5,9,7,8,9,7,7,7,3,6,6,11,7,7,4,7,3,5,4,8,8,8,7,5,9,11,8,6,5,7,3,9,6,6,6,8,5,7,6,5,5,10,6,9,8,11,11,11,7,4,7,8,6,7,5,10,8,6,8,11~6,7,6,7,11,5,7,7,7,9,8,8,7,3,8,5,8,8,8,7,4,5,10,6,11,5,6,6,6,11,9,7,6,10,7,11,11,11,4,3,9,7,6,8,8,7,5~5,3,6,3,10,4,10,8,9,6,8,6,7,7,7,8,11,9,7,8,7,8,5,7,11,7,6,8,8,8,10,5,8,7,5,8,5,7,7,6,9,11,7,6,6,6,3,11,6,9,9,7,5,7,7,4,5,7,11,11,11,8,5,3,4,6,11,4,6,7,6,7,10,6,9,5&reel_set16=10,5,8,9,8,10,10,9,9,9,8,10,9,8,9,10,9,10,11,10,10,10,9,5,9,6,9,8,9,7,8,8,8,9,8,10,11,11,7,8,10,8,11,11,11,4,6,11,9,10,9,9,8,7,4~11,10,4,10,11,11,9,8,8,9,9,9,11,6,9,9,11,8,10,9,10,9,9,10,10,10,7,8,11,9,10,9,10,5,9,9,7,8,8,8,5,10,8,6,10,9,9,8,8,11,11,11,10,8,8,10,9,6,4,9,9,7,10,7~10,11,10,9,9,5,8,9,9,9,8,10,10,9,9,8,9,9,11,10,10,10,8,4,9,10,9,8,11,8,8,8,7,9,9,10,8,6,11,10,8,11,11,11,4,7,5,7,8,10,10,9,9,6&reel_set17=8,9,11,10,10,11,11,10,10,10,9,11,9,11,10,8,10,7,9,9,9,8,10,11,9,6,9,10,10,11,11,11,8,7,9,8,8,10,11,8,8,8,9,11,7,10,6,10,9,10,9,8~9,10,8,7,10,11,10,10,10,8,11,9,7,11,11,8,9,9,9,7,9,10,8,8,11,11,11,10,10,8,10,11,11,10,8,8,8,10,9,6,11,9,8,6,9,10~9,11,11,10,8,10,10,9,7,11,10,10,10,9,9,11,11,6,7,8,10,9,6,10,9,9,9,7,8,10,11,8,9,8,10,8,8,11,11,11,10,11,9,10,7,10,8,9,10,9,8,8,8,10,10,6,9,11,11,9,8,10,8,9&total_bet_max=30,000,000.00&reel_set21=6,1,10,7,6,1,8,3,4,11,10,9,7,8,9,3,5,9,11,4,5,1,4,1~8,1,8,1,10,11,11,1,11,1,9,6,8,1,11,3,1,4,3,7,1,11,5,7,6,9~8,7,8,11,5,9,11,11,7,10,8,6,5,9,8,7,11,6,10,8,9,11,10,9,4,7,9,6,10,6,10,6,9,7,3,5,4,7,10,6,10,11,11,10,9,3,8,11,6,11,5,7,9,4,7,4,8,10,4,9,11,9,5,10&reel_set22=8,7,3,4,6,10,11,11,9~5,11,1,11,10,1,3,1,4,7~3,9,10,11,7,11,6,9,10,8,10,6,11,4,10,4,11,8,10,9,8,9,5,7,10,7,9,5,11,9,6,11,7,8,6,8,4,5,3,11,6,9,7,8,10,11,5,11,7,10,7,9,4,6,9,4,10,6,5,11,10,9,10,8,5,8,3,6,7&reel_set0=11,3,4,5,8,3,9,8,10,5,6,9,4,1,3,11,9,8,7,4,10,9,6,4,10,9,5,11,10,11,5,6,3,4,4,5,7,10,8,1,7,5,8,1,10,4,8,11,6,7,11,5,9,11,6,5,1,6,9,5,8,3,11,4,11,3,11,6,4,4,8,9,1,10,6~1,10,7,9,7,3,4,7,10,11,6,9,10,8,3,5,3,8,11,11,10,5,1,9,1,8,5,7,11,7,6,11,9,10,1,7,11,10,11,10,4,8,5,6,11,7,8,5,8,4,7,10,9,1,6,8,3,6,1,5,3,10,4,1,8,9,1,8,6,5,8,11,6,11,11,9,7,8,1,10~6,10,6,8,5,11,6,9,7,9,5,9,7,10,11,5,10,4,11,3,11,4,9,10,9,4,3,7,6,11,7,3,8,1,3,8,4,5,6,8,10,10,5,1,8,9,10,10,11,7,9,9,10,5,11,8,4,10,7,7,6,4,9,6,1,8,7,8,3,9,8,6,5&reel_set23=9,7,8,11,8,5,9,9,9,4,11,9,11,4,9,7,8,8,8,9,9,11,5,5,10,9,7,7,7,5,7,7,5,11,8,11,5~1,11,9,5,5,9,9,8,8,11,11,1,8,1,11,1,7,3,1,5,8,1,11,9,11,1,7,1,5,1,11~11,11,6,6,3,8,3,10,3,3,3,8,3,3,9,7,11,1,6,7,4,4,4,5,5,8,10,8,8,4,9,7,10,10,10,4,7,7,10,10,4,11,3,9,9,9,6,8,8,5,9,7,9,5,4,5,5,5,11,9,9,11,3,8,5,5,11,6,6,6,4,6,11,7,6,11,5,1,10,7,7,7,5,7,8,6,4,10,9,5,10,8,10&reel_set24=6,3,11,3,10,3,4,7,10,5,10,8,6,11,7,8,9,6,7,3,10,6,5,11,8,5,8,7,11,4,11,9,5,4,9,10,4,7,5,6,4,11,11,10,8,6,10,9,8,5,7,10~1,5,9,7,8,5,7,5,6,7,9,4,8,1,7,8,3,10,5,3,9,5,10,7,6,3,8,10,7,8,1,6,1,5,10,8,9,4,9,10,8,4~10,1,8,5,6,5,4,6,1,11,10,11,1,3,9,1,9,1,8,4,7,1,3,9,7,6,10,4,7,11,10,3,11,7,9,8,11,1,11,1,6,1,10,6,1,5,4,10,6,10,11,9,3,7,11,8,1,5,8,4,8&reel_set2=7,10,8,3,11,4,11,11,5,6,9,8,4,8,9,6,11,9,5,10,7,11,10,9,10,7,3,10,6,7,6,4,9,3,8,4,7,5,8,8,6,10,3,8,9,7,6,7,10,8,4,9,6,9,4,9,8,5,10,3,10,8,9,6,8,3,4,7,11,8,11,10,11,10,5,7,8,5,8~7,10,7,7,3,8,9,10,4,9,3,10,11,3,5,4,6,8,5,4,11,11,8,7,3,8,11,10,11,11,5,9,8,9,11,9,8,10,7,4,11,10,5,6,8,7,6,3,11,8,7,6,7,3,11,7,7,5,9,5,11,8,7,4,7,4,7,3,4,11,10,6,11~11,6,7,5,3,7,6,7,8,9,10,6,8,5,5,10,7,3,4,9,10,4,9,5,3,10,8,4,6,7,3,8,4,9,4,4,7,11,4,5,6,10,7,3,5,11&reel_set1=9,3,11,9,8,7,4,11,10,1,8,5,4,6,6,9,5,1,10,4,6,11,5,3,8,11,3,10,9,8,10,7,7,4,7,8,4,9,10,5,3,7,7,4,6,11,10,11~8,3,6,10,5,11,6,4,10,4,3,3,1,8,7,7,5,9,6,3,8,7,7,9,8,10,4,3,7,8,9,8,5,4,10,3,1,5,6,5,4,5,6,1,6,1,8,7,9,3,11,7,9,11,1,9,6,6,10,3,9,10,4,3,3,1,3,6,10,10,5,10~9,8,5,11,10,6,4,7,3,6,10,9,7,6,9,7,7,5,1,10,3,9,8,7,6,9,11,11,9,3,4,7,5,8,1,10,7,9,9,10,8,6,10,11,5,9,8,11,4,3,4,4,9,6,4,1,10,8,11,3,5,11,3,5,10,11,9,9,7,8,7,4,8,5,9,8,3&reel_set4=10,5,11,10,8,3,5,8,9,10,10,10,4,9,9,8,8,9,5,6,5,9,9,9,4,3,4,8,3,7,4,10,4,5,10,6,6,6,11,10,10,11,3,9,9,8,6,5,4,4,4,6,8,4,3,6,11,6,11,8,10,8,8,8,11,11,10,10,4,8,10,8,7,11,7,3,3,3,9,3,4,7,6,9,9,5,8,7,7,7,6,10,10,11,5,6,7,5,7,8,3,7~11,8,10,7,10,9,8,7,6,3,3,10,10,10,7,9,10,5,11,8,5,4,8,10,11,6,6,6,4,5,6,1,10,5,10,10,9,6,9,7,7,7,10,4,10,4,6,5,8,4,8,9,10,3,3,8,8,8,5,9,7,6,10,10,6,3,4,8,6,3,9,9,9,11,10,6,10,4,11,7,9,5,10,5,6,3,3,3,11,10,1,11,3,1,8,11,8,6,4,11,4,4,4,5,8,9,8,1,5,3,9,8,8,10,11,10,3~5,11,9,7,8,6,5,5,3,3,3,8,10,5,7,3,1,6,7,9,4,4,4,6,3,11,5,11,11,1,7,8,10,10,10,8,10,8,7,11,8,3,8,7,10,9,9,9,11,4,10,5,9,5,6,8,9,3,5,5,5,8,5,11,4,10,9,4,7,8,6,6,6,11,10,7,8,9,4,4,9,4,11,7,7,7,3,6,10,9,5,9,6,7,10,6,10&purInit=[{bet:700,type:\"default\"},{bet:3000,type:\"default\"}]&reel_set3=9,11,8,10,11,3,5,8,11,7,6,10,11,8,9,8,7,11,7,4,3,11,4,7,9,6,4,10,5,8,7,8,7,10,6,4,6,8,11,10,4,5,6,9,3,10,1,10,8,7,10,7,1,6,1,6,4,10,8,9,11,9,8~4,9,1,10,7,11,11,6,7,4,11,10,3,5,8,11,4,7,11,9,6,11,5,8,5,10,8,5,9,10,7,3,11,11,11,5,4,8,10,7,8,9,1,9,8,11,10,8,10,6,9,1,11,10,3,4,9,8,4,1,6,7,5,9,6,8,7,10,9,3~7,6,5,6,7,9,6,10,11,9,7,4,11,8,6,10,5,8,3,4,5,9,10,11,6,7,10,8,5,4,11,11,8,7,9,10,9,3,11,4,10&reel_set20=7,1,6,7,1,4,10,4,4,1,4,10,9,10,7,10,1,10,8,1,8~9,5,6,11,1,3,1,6,5,7,1,7,6,8,1,8~1,9,8,1,11,5,7,1,10,6&reel_set6=11,3,8,6,7,6,4,5,11,10,7,10,3,6,11,6,5,7,5,4,8,6,4,7,5,9,10,8,10,11,6,9,10,4,11,4,10,7,6,8,11,11,8,10,3,10,5,11,3,9,8,5~3,7,5,4,6,1,9,6,4,8,9,8,10,6,7,8,6,5,4,9,5,9,10,6,7,10,5,9,5,1,7,9,8,6,10,1,8,3,5,7,3,9,1,8,5,1,7,8,5,7,3,10,5,1,10,8,7,8,1,9,3,5,4,10,7,3,10,5,8,5,7~8,1,10,8,11,7,8,1,7,11,1,11,7,1,11,1,6,5,6,10,4,4,6,10,1,8,3,4,9,7,4,5,6,3,11,11,1,9,3,8,6,5,1,3,1,10,11,9,6,1,10,9,5,7,8&reel_set5=3,4,5,6,6,11,7,5,11,4,10,3,3,3,6,11,11,7,7,10,7,11,3,3,8,6,5,5,5,4,3,5,7,8,7,5,4,11,8,11,10,10,10,5,10,9,3,7,3,7,10,8,6,3,5,6,4,4,4,10,10,8,11,11,4,4,9,3,8,5,10,7,7,7,3,5,4,11,6,10,10,6,7,9,7,5,6,6,6,9,8,6,3,4,7,9,6,7,11,7,8,8,8,9,8,5,7,9,10,7,5,8,4,3,6,11,6~11,6,10,5,7,6,8,8,5,5,4,11,3,3,3,7,3,5,9,8,9,7,4,5,10,4,10,4,11,10,5,5,5,8,4,9,9,10,10,3,8,6,3,11,4,5,9,10,10,10,7,8,6,7,9,3,9,5,3,11,6,9,6,9,8,4,4,4,5,10,8,7,8,3,9,6,8,10,5,6,7,9,9,9,4,8,7,6,4,10,11,4,8,7,5,5,11,6,10,10~9,9,10,5,3,3,3,10,8,4,6,3,11,4,4,4,5,10,7,11,6,10,10,10,4,10,9,8,6,7,9,9,9,3,8,7,5,3,5,5,5,6,10,11,9,7,10,6,6,6,3,5,8,11,7,11,11,11,8,9,4,8,4,9,7,7,7,8,4,6,8,3,8,8,8,5,3,7,6,8,10,11&reel_set8=8,10,8,10,9,11,6,9,3,1,5,6,8,1,10,11,5,11,7,11,5,4,5,9,4,9,7,11,3,1,8,4,6,5,10,7,11,4,8,6,3,7,10,4,8,5,11,6,3,4,9,10,7~4,9,1,6,9,7,10,3,5,6,7,5,8,5,6,5,6,5,8,7,8,10,1,5,4,9,3,7,10,3,1,7,10,9,10,7,1,8,4,9,3,8~10,6,3,7,6,5,7,3,5,10,7,9,8,11,10,6,11,3,8,3,6,7,5,4,11,1,7,10,9,6,1,7,11,8,4,8,5,3,8,7,5,1,9,6,1,9,4,11,8,9,7,9,10,5,3,4,1,8,6,9,8,10,4,9,8,3,6,10,1,11,4,11,5&reel_set7=11,8,1,6,9,6,11,9,10,11,3,1,3,5,6,4,7,5,7,7,1,6,4,4,1,5,4,1,9,5,10,5,3,10,8,7,4,1,10,7,1,7,11,8,10,6,1,9,8,9,1,9,10,8,11,3,3~5,7,1,5,10,5,6,3,10,5,10,4,7,6,5,10,9,3,4,3,5,9,3,9,6,8,7,10,8,4,7,1,8,1,5,9,8,1,9,8,6,7~4,7,3,4,6,8,9,4,5,9,8,11,5,6,10,4,11,3,11,6,4,10,7,9,8,10,11,11,11,10,5,7,3,10,7,9,5,8,5,9,6,10,9,10,6,11,8,6,11,3,7,4,5,7,5,3,5,8&reel_set9=8,3,9,4,7,6,4,10,3,3,3,8,5,9,4,7,10,5,7,7,7,6,8,3,4,7,3,7,8,5,5,5,3,4,3,6,3,3,4,5,1,3~4,7,8,3,7,8,5,3,7,3,6,3,3,3,9,3,9,5,3,4,5,1,9,8,10,6,7,7,7,5,8,3,4,6,10,7,4,3,8,3,6,5,5,5,3,4,7,5,1,3,4,3,10,5,3,4,7,4~3,8,7,5,7,3,4,3,3,3,4,5,8,3,7,6,3,4,3,7,7,7,6,6,3,8,7,8,4,4,3,9,5,5,5,4,5,10,1,10,4,9,5,6,5,3,1&total_bet_min=20.00";
            }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 70; }
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
            get { return 2; }
        }
        public double[] PurchaseMultiples
        {
            get { return new double[] { 70, 300 }; }
        }
        #endregion
        public StarlightPrincessPachiGameLogic()
        {
            _gameID = GAMEID.StarlightPrincessPachi;
            GameName = "StarlightPrincessPachi";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"]   = "0";
	        dicParams["g"]          = "{minigame_screen:{def_s:\"0,0,0,0,0,0,0,0,0\",def_sa:\"0,0,0\",def_sb:\"0,0,0\",s:\"0,0,0,0,0,0,0,0,0\",sa:\"0,0,0\",sb:\"0,0,0\",sh:\"3\",st:\"rect\",sw:\"3\"}}";
	        dicParams["st"]         = "rect";
	        dicParams["sw"]         = "3";
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


                StarlightPrincessPachiBetInfo betInfo   = new StarlightPrincessPachiBetInfo();
                betInfo.BetPerLine                      = (float)   message.Pop();
                betInfo.LineCount                       = (int)     message.Pop();
		
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in StarlightPrincessPachiGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                    (oldBetInfo as StarlightPrincessPachiBetInfo).PurchaseType = betInfo.PurchaseType;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in StarlightPrincessPachiGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            StarlightPrincessPachiBetInfo betInfo = new StarlightPrincessPachiBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new StarlightPrincessPachiBetInfo();
        }
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as StarlightPrincessPachiBetInfo).PurchaseType;
            return this.PurchaseMultiples[purchaseType];
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
        protected override async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int agentID, BasePPSlotBetInfo betInfo, double baseBet, UserBonus userBonus, bool isAffiliate)
        {
            int purchaseType = (betInfo as StarlightPrincessPachiBetInfo).PurchaseType;
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
                int purchaseType = (betInfo as StarlightPrincessPachiBetInfo).PurchaseType;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, -1, PurchaseMultiples[purchaseType] * 0.2, PurchaseMultiples[purchaseType] * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in StarlightPrincessPachiGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int purchaseType = (betInfo as StarlightPrincessPachiBetInfo).PurchaseType;
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in StarlightPrincessPachiGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = (betInfo as StarlightPrincessPachiBetInfo).PurchaseType.ToString();
        }
        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            StarlightPrincessPachiBetInfo starBetInfo = betInfo as StarlightPrincessPachiBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, -1, betInfo.PurchaseFree ? starBetInfo.PurchaseType : -1, betMoney);
        }
    }
}
