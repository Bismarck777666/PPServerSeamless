using Akka.Actor;
using Akka.Event;
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
    public class TheWildGangBetInfo : BasePPSlotBetInfo
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
            writer.Write(this.PurchaseType);
        }

    }
    class TheWildGangGameLogic : BasePPSlotGame
    {
        protected double[] _totalFreeSpinWinRates   = new double[2];
        protected double[] _minFreeSpinWinRates     = new double[2];

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vswayswildgang";
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
                return "def_s=6,11,5,11,8,9,5,7,4,8,11,7,3,9,9,7,9,10,4,7,10,3,4,8&cfgs=10773&ver=3&def_sb=6,9,8,11,11,10&reel_set_size=16&def_sa=3,7,11,8,11,10&scatters=1~0,0,0,0,0,0~0,0,0,0,0,0~1,1,1,1,1,1&rt=d&gameInfo={rtps:{purchase_1:\"93.93\",purchase_0:\"94.03\",regular:\"94.03\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"12376200\",max_rnd_win:\"10000\"}}&wl_i=tbm~10000&reel_set10=3,10,10,6,8,7,5,4,5,9,11,4,10,5,9,6,4,7,10,6,8,11,6,4,8,5,3,8,8,10,8,9,4~5,7,10,8,6,9,9,5,11,8,7,9,8,10,11,7,10,5,11,8,7,5,2,4,8,7,3,7,10,6,7,9,11,8,11,9,9,3,9,9,8,11,11,7,4,6,10,11,6,9,2,6,11,4,10,4,5,3,6,8,7,10,8,3,11~10,7,11,5,10,10,5,4,9,6,8,8,10,3,11,3,10,7,8,6,9,8,4,4,4,10,9,5,7,6,8,5,8,8,11,4,4,2,4,6,9,5,8,5,8,8,5,6,10~11,9,10,6,7,8,5,4,11,10,9,9,7,6,10,8,11,11,9,8,8,8,5,8,7,5,11,7,4,10,8,8,2,3,4,9,11,6,9,11,6,6,6,9,11,6,11,7,11,6,9,9,6,11,10,7,6,7,6,7,11,6,7,7,6~10,6,8,9,4,7,10,11,10,7,2,11,4,10,9,2,6,5,2,11,8,9,2,10,7,9,2,10,9,6,2,5,11,2,6,7,9,8,2,11,9,3,2,8,2,11,4~3,8,10,8,8,10,10,8,11,10,9,7,11,7,5,10,8,8,11,5,6,10,9,11,4,4,5,3,6,8,9,5,5,4,4&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&reel_set11=10,11,5,4,10,4,5,3,11,11,10,3,11~9,7,9,8,6,7,9,9,8,6,9,7,6~7,9,10,5,3,9,10,9,9,10,5,4,9,10,7,10,10,8,10,8,7,8,5,9,8,10,10,9,8,5,6,11,8,9,3,8,7,2,7,9,4,10,9,8,10,5,10,11,5,6,10,5,9,8,4,7,10,8,7,9,5,11,7,3,8,11,8,8,4,11,4,7,8,6,6~9,7,8,11,10,11,5,9,7,11,9,10,9,10,8,11,6,2,10,6,6,9,5,11,7,10,10,9,4,4,5,8,5,7,9,11,11,9,4,10,6,11,10,10,3,10,7,7,10,9,7,10,9,5,5,6,3,6,8,10,5,11,9,10,9~10,8,4,11,5,5,6,4,8,11,5,6,3,8,4,8,6,4,10,9,11,10,9,7,8,11,6,11,10,8,11,9,9,7,8,10,8,8,10,11,4,4,9,9,6,2,11,11,8,6,7,8,8,5,6,4,4,9,11,11,7,11,8,10,3,6,6,11~10,4,9,5,7,3,8,11,11,6,5,10,7,10,11,11,9,9,10,5,11,8,8,10,5,9,10,9,11,8,11,6,10,5,9,4,8,3,6,4,11,10,6,8,8,9,6,8,11,9,6,10,4,6,10,9,10,4,9,11,5,4,5,7,8,3&reel_set12=8,11,9,10,5,7,6,11,4,8,4,10,4,11,9,10,9,6,5,8,6,8,3,5~2,5,7,10,9,2,4,10,11,6,3,8,2,9,8,10,9,6,10,11,9,8,11,2,7,6,4,7,2,4,9,8,6,2,10,11,10,7,2,8,11~6,11,11,2,10,10,8,9,3,9,2,8,9,2,6,11,7,4,5,4,10,8,2,5,10,6,9,6,9,8,10,9,7,8,8,8,9,10,6,11,5,11,8,10,8,8,7,11,6,10,8,3,9,2,11,8,2,11,2,5,10,2,5,2,6,2,11,11,5,8~9,10,5,7,5,8,6,8,11,10,3,11,11,10,9,11,2,9,10,9,5,7,11,3,6,8,8,2,5,4,11,9,11,11,7,9,6,5,8,7,10,4,7,10,10,2,11,7,4,10,2,8,6,11~4,11,9,11,6,4,10,10,7,5,11,7,3,8,10,9,9,8,6,8,6,9,10,5,7,5,2,7,11,11,2,11,7,9,8,10,10,8,6,11,3~6,4,3,9,4,8,5,11,11,11,10,6,10,6,10,5,7,5,4,10,10,10,5,10,11,3,8,10,8,10,11,8,5,5,5,4,9,10,7,5,11,11,9,11,8,9,11&purInit_e=1,1&reel_set13=8,5,9,9,6~11,4,10,7,3,7,11,4,10,11,10~8,7,6,11,6,9,7,4,3,10,10,9,8,7,5,11,9,10,11,6,10,5,4,9,10,3,10,10,9,10,9,3,4,4,8,9,9,8,6,7,5,8,11,6,10,5,8,5,5,9,4,4,5,6,8,7,9,10,8,11,9,10,11,6,4,9,7,5,4,6,8,10,8,5,11,10,7~10,8,7,11,8,4,11,6,9,7,6,11,10,11,4,7,9,3,6,7,8,7,8,9,10,7,6,3,9,10,7,9,11,11,5,11,4,9,9,8,11,9,4,6,8,11,11,8,6,5,11,6,11,9,9,11,5,7,6,11,7,6~10,8,5,8,5,3,6,9,10,4,6,10,9,7,8,11,10,7,8,9,8,9,10,6,10,9,11,5,5,4,8,8,4,4,9,8,5,5,4,10,4,7,6,4,7,10,5,8,5,8,4,11,8,4,3,10,3,8,9~11,10,8,3,5,10,4,5,9,7,11,10,7,8,4,11,8,5,11,3,4,6,8,9,10,9,6&wilds=2~0,0,0,0,0,0~1,1,1,1,1,1&bonuses=0&reel_set14=9,8,4,7,8,6,8,10,3,9,11,5,4,4,9,8,8,8,4,8,8,3,8,4,6,10,11,5,10,10,8,8,10,5,5~7,4,7,8,10,8,11,8,11,11,6,11,5,8,9,9,7,7,10,9,6,8,6,11,9,9,7,11,11,7,11,10,7,6,7,10,9,11,6,11,11,9,10,6,11,6,8,3,11,10,11,9,4,7,7,7,5,9,7,3,10,9,11,9,5,6,7,9,9,6,8,5,9,7,11,7,11,5,9,7,6,10,8,10,11,3,6,4,11,11,10,11,8,7,9,7,10,9,11,11,4,6,8,7,6,9,11,4,11,8,6,3,4,11~11,8,8,8~8,8,8,11,8,8,11~8,8,8,11,8,8,11~4,3,10,11,10,11,8,7,11,11,10,8,10,9,4,10,9,3,4,4,8,5,10,8,7,6,9,5,5,6,8&paytable=0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;150,50,30,15,0,0;60,40,20,10,0,0;40,25,15,8,0,0;30,20,12,6,0,0;20,15,10,5,0,0;14,10,6,4,0,0;14,10,6,4,0,0;12,8,5,3,0,0;12,8,5,3,0,0;0,0,0,0,0,0;0,0,0,0,0,0&reel_set15=7,4,10,10,9,9,10,6,11,5,8,4,4,10,9,10,8,8,5,3,4,5,5,11,10,9,8,8,3,5,8,8,5,9,6,7,3,8,6,4~11,11,9,8,11,6,11,6,4,11,6,11,11,6,7,4,7,3,7,11,10,8,10,8,4,9,11,11,9,7,5,9,10,6,6,6,7,9,11,6,7,11,5,6,6,8,6,5,6,9,7,7,8,3,9,3,11,9,7,6,8,8,7,11,10,10,9,3,10,6,9,5~10,10,3,5,9,9,4,8,8,9,10,3,11,11,8,6,5,4,6,5,8,9,8,8,9,8,10,8,8,9,5,8,8,8,4,8,6,10,8,5,4,8,6,10,5,10,10,8,11,5,5,3,4,7,10,11,8,4,10,3,8,4,7,9,10,6~8,11,4,5,9,11,8,8,11,9,11,6,9,11,6,7,4,8,10,9,10,9,3,11,6,6,7,3,7,10,7,11,5,7,11,9,6,7,10~6,9,5,8,10,3,9,10,10,8,8,3,10,10,5,8,6,8,10,9,9,10,8,7,8,9,8,8,8,10,8,4,6,8,5,5,4,9,7,5,8,11,10,4,4,8,5,4,8,10,4,4,6,8,3,10,5,8~4,10,4,9,5,5,11,8,7,4,10,7,10,11,6,8,10,10,8,9,5,5,11,8,8,9,3,11,9,4,3,10,10,11,11,6,8&total_bet_max=20,000,000.00&reel_set0=5,10,4,8,4,8,5,11,6,9,5,8,8,8,4,8,4,10,10,9,8,10,8,7,8,8,3~9,11,6,7,9,9,4,11,7,11,11,6,11,6,4,7,5,11,11,8,6,10,6,9,3,8,7,8,11,8,10,4,9,6,9,6,8,5,7,7,7,11,11,9,4,11,7,7,8,9,11,5,11,10,5,10,9,11,8,9,10,2,7,10,7,7,11,9,6,8,7,7,3,11,6,9,11,9,11,8,6,7,10~8,8,2,11~8,8,2,11~2,11,8,2,8,8,11~4,8,10,11,11,7,4,9,3,5,8,11,11,7,5,9,3,9,6,8,10,5,10,6,4,10,10,8,4&accInit=[{id:0,mask:\"cp;sc\"}]&reel_set2=6,8,4,9,7,10,10,8,10,8,8,10,5,4,9,8,8,8,3,8,9,4,3,6,11,8,5,8,5,11,10,4,8,8,4,5~9,4,6,10,6,4,11,9,6,7,11,6,10,8,6,10,7,11,9,10,9,9,6,6,6,11,7,7,3,8,7,11,10,10,11,9,11,7,6,6,10,10,5,8,7,11,6,9,8,9,11~8,10,3,8,10,4,4,7,10,4,5,8,11,9,8,11,8,8,6,10,4,8,5,8,6,8,10,8,4,10,8,10,6,7,11,2,5,8,9,4,9,10,9,6,3,2,5,8,9,5~6,11,7,4,8,9,11,8,7,11,10,6,9,11,9,8,6,9,5,6,9,6,10,6,7,3,4,11,6,9,6,7,11,10,7,10,7,9,11,11,5,10,7,10,3,11,7,11,8,4,9,8,9,11,11,5~8,10,10,8,10,3,9,2,5,11,4,6,4,8,9,9,5,5,10,11,8,5,8,10,8,9,11,4,4,9,10,10,9,8,8,8,10,4,5,8,8,9,8,3,10,8,5,10,8,5,6,5,10,6,8,7,4,11,8,3,8,3,8,6,8,10,4,9,4,8,7,5,8,6,4~4,5,7,5,6,8,11,3,10,8,11,10,4,9,8,10,8,7&reel_set1=7,8,4,6,11,10,9,4,10,10,4,8,5,7,3,8,5,9,9,7,10,5,6,9,8,10,8~7,2,11,3,6,11,9,7,11,10,10,9,7,6,6,11,11,7,8,7,6,10,11,11,5,10,11,9,8,7,9,4,6,9,8,9,8,4~6,9,8,4,10,10,7,8,11,9,5,7,8,6,10,8,8,4,5,5,8,5,8,10,4,4,9,3,5,7,8,10,8,5,10,9,9,8,9,10,4,11,9,7,6,8,10,4,8,4,8,3,5,6,11,10,3~11,8,9,7,4,11,11,9,11,7,11,3,10,7,9,6,6,9,9,11,10,4,9,10,9,10,8,11,11,6,6,8,5,6,9,7,7,11,8,6,8,2,6,7,11,5,11,7~3,11,9,8,8,10,8,8,5,9,10,8,3,5,4,4,9,4,8,9,5,10,8,7,11,10,8,4,10,5,8,10,6,4,10,10,5,8,7,9,6,8,8,3,10,4,6,9,8,8,10,8,5,5,6,5,4,10~3,9,10,11,4,9,10,5,6,4,9,10,5,10,8,11,10,8,3,5,7,8,9,7,8,6,4,11,10,11,8&reel_set4=4,8,4,10,5,10,5,8,9,3,5,8,8,9,10,10,8,11,5,8,8,8,4,6,5,8,10,3,9,5,8,10,11,7,8,4,9,10,8,8,4,8~9,5,9,6,8,7,6,11,7,6,10,4,10,10,5,10,6,3,8,11,5,11,9,11,9,11,3,11,9,8,4,6,6,6,4,11,8,6,10,11,8,7,7,11,11,2,8,9,9,6,7,11,7,7,6,8,6,11,11,9,6,11,11,6,7,9,7~6,5,2,5,3,8,11,7,5,10,10,4,6,10,3,4,10,6,9,8,6,10,11,9,10,8,10,5,10,7,8,9,8,8,11,6,8,8,8,4,8,9,8,4,4,8,8,4,9,8,5,8,5,8,9,10,5,8,8,5,6,8,9,8,3,11,10,11,10,7,10,4,9,4,5,8,8~8,7,6,8,11,10,7,8,8,10,2,5,11,7,4,11,9,7,11,9,5,7,6,10,3,4,11,9,11,6,10,8,11,7,6,10,11,9,9,4,6,6,9,9,11~7,3,8,4,5,10,9,8,9,10,8,10,8,10,8,6,8,10,8,8,3,4,10,11,8,6,7,8,8,8,4,4,8,10,10,3,5,9,10,8,4,4,6,5,5,4,8,9,5,8,5,5,2,9,6,11,10,8,9~10,6,11,9,10,7,10,8,4,5,4,9,11,7,3,4,8,6,7,9,4,8,11,3,11,5,11,10,10,5,8,8,5,9,10&purInit=[{bet:1600,type:\"fs\"},{bet:4000,type:\"fs\"}]&reel_set3=3,5,6,7,5,8,4,9,10,8,6,10,3,4,9,5,8,9,6,11,8,6,9,4,5,5,7,11,10~10,6,7,7,11,11,6,7,6,11,9,10,9,10,9,11,4,7,5,7,8,11,9,4,9,8,7,8,10,6,8,4,3,8,7,5,9,5,11,3,6,3,11,9,6,5,7,5,8,10,8,9~11,4,7,3,10,9,9,4,9,11,10,9,5,10,7,9,11,4,9,10,6,4,9,11,5,3,6,8,7,8,6,7,10,8,10,6,8,7,6,11,10,10,4,4,5,11,9,9,7,11,10,10,6,4,8,5,9,9,10,5,8,5,5,8,6,3,5,8~7,6,7,11,5,8,6,7,5,10,9,8,10,7,11,9,10,11,6,9,11,10,3,4,6,4,5,9,10,11,11,9,11,9,7,11,8,11,6,8,4,7,4,9,6,8,11,6,11,11,9,6,9,7,10,7,6,7,11,8,3,8,11,9,9~8,3,9,10,10,9,7,5,4,3,7,10,8,9,8,3,4,4,8,8,4,8,11,5,9,6,10,5,6,10,5,10,4,9,8,5,8,6,10,7,11,5,4,8,4,11,6~4,8,11,10,10,6,8,8,7,4,3,5,5,9,4,10,9,5,11,6,10,11,3,8,7,9&reel_set6=10,5,8,9,7,8,5,6,3,6,10,4,5,9,8,11,10,5,9,8,10,9,4,7,10,11,4,9,10,8,11,7,3,11,4,6~6,10,6,8,7,4,8,6,5,2,6,2,11,9,8,9,2,8,2,9,10,11,7,11,11,4,2,8,6,11,7,2,7,9,8,4,8,2,9,5,3,8,9,2,10,10,2,3,10,2,5,2,10~5,4,11,9,9,10,7,10,5,8,4,11,11,6,3,11,7,5,11,8,6,10,9,10,6,11,7,9,3,4,10,8,11,7,6,10,9,11,2,6,7,11,9,11,9,3,8,5,2,4,8,7,8,8,10,10,6,7~8,10,11,9,2,9,11,8,2,4,11,9,2,5,2,4,6,4,6,10,6,2,7,2,8,10,7,2,8,6,9,10,4,10,8,9,10,11,9,8,3,11,6,8,9,5,8,7,5,2,7~7,3,11,8,10,11,10,3,11,7,10,8,6,10,5,8,9,8,7,6,11,10,5,6,6,2,6,9,10,5,7,4,6,4,11,9,6,8,3,6,5,9,10,7,11,4,11,8,11,9,8,10,5,11,11,10,9,7,7,11,2,9~8,11,8,8,7,6,8,8,8,5,10,4,10,7,5,7,10,10,10,9,8,11,10,10,4,8,4,4,4,11,6,4,9,3,8,5,5,5,11,3,8,9,10,10,5,5,10&reel_set5=8,8,6,7,9,9,6,7,9~11,3,10,3,11,10,5,4,11,10~8,11,4,7,6,6,9,8,9,10,9,2,6,3,10,8,4,6,7,9,5,9,7,10,7,5,5,4,3,7,8,5,11,7,10,8,10,11,11,8,4,8,10,7,9,8,8,5,5,10,8,9,10,10,11,10~11,6,5,10,10,8,9,5,7,10,10,5,5,2,10,7,11,10,9,3,8,5,10,10,6,5,10,8,10,9,7,9,11,4,11,6,9,9,6,4,9,11,9,11,11,7,3,7,6~4,8,11,4,5,3,10,11,10,6,10,11,11,9,5,5,11,11,8,8,9,7,6,2,4,7,11,10,8,6,4,10,4,8,10,6,6,9,8,6,8,9,7,6,9,11,8,11,11,5,9,8,8,11,4,9,4,6,8,7,3,4,11,10,10,8,9,4,6,8,6,11,11,6,8~10,11,11,9,6,8,5,10,10,4,9,5,7,8,8,9,4,6,5,3,10,5,9,11,5,8,4,11,11,9,8,10,10,8,10,4,6,8,9,10,6,3,9,6,7,11,7,6,11&reel_set8=10,9,11,8,5,8,11,6,8,3,6,8,10,7,4,5,4,5,6,10,8,9~11,6,6,10,8,6,8,11,7,10,9,11,8,5,9,11,11,8,5,8,8,8,6,10,5,8,8,4,9,3,6,11,6,2,10,10,9,4,10,4,5,7,7~11,9,3,10,5,8,5,9,2,9,9,10,2,10,2,5,8,4,2,10,2,7,9,4,3,2,11,2,6,11,8,11,10,7,8,10,4,11,7,2,9,2,7,6,9,2,6,11,6,8~4,8,11,7,6,11,5,11,5,10,3,10,8,6,7,10,7,8,11,7,11,9,10,2,5,9,11,10,8,7,9,8,5,9,4,6,6,11,11,9,6,10,4,5~6,11,10,7,6,7,5,11,5,6,9,5,11,8,11,9,6,11,5,8,6,11,7,10,4,8,7,11,11,10,6,10,7,7,6,10,2,10,3,6,4,7,8,9,11,9,7,8,9,8,5,10,9,5,8,4,11,10,10,11,4,6,10,11,9,6,9,8,3~6,9,5,9,10,8,7,3,5,11,11,11,4,3,4,10,10,5,10,11,10,6,11,10,10,10,11,4,7,8,10,11,8,11,9,10,5,5,5,9,7,11,9,4,8,5,8,10,6&reel_set7=3,11,6,10,8,10,11,8,4,7,4,8,9,4,11,8,5,4,6,8,5,9,8,10,5,5,3,10,7,10,11,10,6,8,9,8,9,5,8,6~2,11,7,8,9,8,9,2,10,8,2,10,3,2,9,10,11,2,5,6,2,11,4,11,7,10,11,6,8,7,2,4,9,9,2,11,7,9,2,5,2,10,9,2,6,4~9,8,5,6,8,3,9,5,10,9,3,6,5,8,8,9,11,9,8,8,7,10,11,10,11,2,10,4,9,8,8,8,10,11,10,6,11,5,9,11,10,11,8,11,9,11,7,7,5,6,7,8,7,10,6,6,10,5,6,4,6,4,6,8~5,5,8,10,9,11,2,7,9,4,6,11,5,8,10,9,8,11,10,11,6,4,10,8,7,6,10,11,5,9,11,5,6,3,7,10,7,3,9,11,8,7~7,9,8,6,4,9,6,10,5,10,3,8,11,2,9,6,4,7,6,6,5,8,11,8,7,10,10,5,11,7,11,11,6,10,10,11,11,9~4,11,8,4,10,11,11,11,4,11,3,10,6,8,10,10,10,3,5,5,9,11,9,5,5,5,8,10,10,6,5,7,10&reel_set9=8,10,3,5,11,9,7,6,8,8,8,10,8,9,10,5,6,10,4,10,8,3,10,10,10,8,8,10,9,8,4,5,11,4,7,8,7~9,4,11,11,10,9,5,7,6,2,7,3,9,7,7,9,8,9,10,10,3,7,5,11,4,5,7,7,4,11,11,5,9,11,4,10,11,4,11,8,10,8,7,11,8,4,8,10,6,9,6,7,10,8,8,5,10,11,11,6,10,6~11,6,10,3,7,11,9,7,11,7,8,8,7,5,8,7,10,6,11,4,10,9,9,10,11,8,11,11,8,11,10,9,7,11,10,2,3,9,6,8,7,4,5,4,5,5,10,10,6,9,4,8,5,9,4,3,11,4,7,10,6,5,8,9,5,11,11,10~9,4,10,2,7,9,6,10,2,9,8,2,11,2,10,6,4,2,7,6,2,3,9,10,7,2,10,5,8,5,6,8,11,10,2,8,2,11,8,11,2,7,8,11,3,2,5,9,10,2,4,9,6,9,11,4,7,2,9,9,2,8~10,8,6,7,8,7,10,6,3,10,11,6,8,11,6,9,9,11,7,5,8,5,10,9,7,8,7,3,11,5,10,10,2,11,4,6,4,9,11,4,11,9~7,8,11,10,7,8,8,4,9,4,9,8,8,8,3,6,3,8,7,11,3,7,10,9,10,6,4,5,10,10,10,5,8,6,8,10,10,11,10,10,4,10,11,8,9,8&total_bet_min=10.00";
            }
        }
	
        protected override double PurchaseFreeMultiple
        {
            get { return 80; }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override bool HasPurEnableOption
        {
            get { return true; }
        }
	    protected double[] PurchaseFreeMultiples
        {
            get { return new double[] { 80, 200 }; }
        }
	
        #endregion
        public TheWildGangGameLogic()
        {
            _gameID = GAMEID.TheWildGang;
            GameName = "TheWildGang";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string strInitString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, strInitString);
	    	dicParams["reel_set"] = "0";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "6";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
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
                TheWildGangBetInfo betInfo = new TheWildGangBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();


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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in TheWildGangGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                    (oldBetInfo as TheWildGangBetInfo).PurchaseType = betInfo.PurchaseType;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in TheWildGangGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            if (betInfo.PurchaseFree)
                dicParams["puri"] = (betInfo as TheWildGangBetInfo).PurchaseType.ToString();
        }
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            return PurchaseFreeMultiples[(betInfo as TheWildGangBetInfo).PurchaseType];
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            TheWildGangBetInfo betInfo = new TheWildGangBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new TheWildGangBetInfo();
        }
        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBet = (double)infoDocument["defaultbet"];
                _normalMaxID        = (int)infoDocument["normalmaxid"];
                _emptySpinCount     = (int)infoDocument["emptycount"];
                _naturalSpinCount   = (int)infoDocument["normalselectcount"];
                var purchaseOdds = infoDocument["purchaseodds"] as BsonArray;
                for (int i = 0; i < 2; i++)
                {
                    _totalFreeSpinWinRates[i] = (double)purchaseOdds[2 * i + 1];
                    _minFreeSpinWinRates[i]   = (double)purchaseOdds[2 * i];

                    if (PurchaseFreeMultiples[i] > _totalFreeSpinWinRates[i])
                        _logger.Error("freespin win rate doesn't satisfy condition {0}", this.GameName);
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
            }
        }
        protected override async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int websiteID, BasePPSlotBetInfo betInfo, double baseBet, UserBonus userBonus)
        {
            int purchaseType  = (betInfo as TheWildGangBetInfo).PurchaseType;
            double payoutRate = getPayoutRate(websiteID);
            double targetC    = PurchaseFreeMultiples[purchaseType] * payoutRate / 100.0;
            if (targetC >= _totalFreeSpinWinRates[purchaseType])
                targetC = _totalFreeSpinWinRates[purchaseType];

            if (targetC < _minFreeSpinWinRates[purchaseType])
                targetC = _minFreeSpinWinRates[purchaseType];

            double x = (_totalFreeSpinWinRates[purchaseType] - targetC) / (_totalFreeSpinWinRates[purchaseType] - _minFreeSpinWinRates[purchaseType]);
            double y = 1.0 - x;

            BasePPSlotSpinData spinData = null;
            if (Pcg.Default.NextDouble(0.0, 1.0) <= x)
                spinData = await selectMinStartFreeSpinData(betInfo);
            else
                spinData = await selectRandomStartFreeSpinData(betInfo);
            return spinData;
        }

        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as TheWildGangBetInfo).PurchaseType;
            try
            {
                BsonDocument document = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectPurchaseSpinRequest(this.GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType), TimeSpan.FromSeconds(10.0));
                BasePPSlotSpinData spinData = convertBsonToSpinData(document);
                return spinData;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in TheWildGangGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectMinStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as TheWildGangBetInfo).PurchaseType;
            try
            {
                double purMultiple = getPurchaseMultiple(betInfo);
                BsonDocument document = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinTypeOddRangeRequest(this.GameName, 1, purMultiple * 0.2, purMultiple * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                BasePPSlotSpinData spinData = convertBsonToSpinData(document);
                return spinData;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in TheWildGangGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            TheWildGangBetInfo wildGangBetInfo = betInfo as TheWildGangBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, betInfo.MoreBet ? 1 : 0, betInfo.PurchaseFree ? wildGangBetInfo.PurchaseType : -1, betMoney);
        }
    }
}
