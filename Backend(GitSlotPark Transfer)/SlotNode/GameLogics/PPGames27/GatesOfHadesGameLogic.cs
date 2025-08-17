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
    class GatesOfHadesBetInfo : BasePPSlotBetInfo
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

    class GatesOfHadesGameLogic : BasePPSlotGame
    {
        protected double[] _multiTotalFreeSpinWinRates;
        protected double[] _multiMinFreeSpinWinRates;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20gtsofhades";
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
                return 6;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=4,10,8,3,4,7,5,9,6,5,6,9,3,8,10,7,3,8,4,9,6,5,4,9,3,8,10,8,3,7,5,9,4,4,5,9&cfgs=1&ver=3&def_sb=5,10,3,5,5,8&reel_set_size=4&def_sa=3,8,3,9,3,7&scatters=1~0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0~0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0~1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1&rt=d&gameInfo={rtps:{purchase_1:\"96.47\",purchase_0:\"96.50\",regular:\"96.52\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"42194093\",max_rnd_win:\"10000\"}}&wl_i=tbm~10000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1,1&wilds=2~3000,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0~1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1&bonuses=0&ntp=0.00&paytable=0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,1500,750,360,240,120,90,75,60,45,30,0,0,0,0;2400,2400,2400,2400,2400,2400,2400,2400,2400,2400,2400,2400,2400,2400,2400,2400,2400,2400,2400,2400,2400,2400,1200,600,270,180,90,75,60,50,30,25,0,0,0,0;1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,600,300,180,120,75,60,50,30,25,20,0,0,0,0;900,900,900,900,900,900,900,900,900,900,900,900,900,900,900,900,900,900,900,900,900,900,480,210,150,90,60,45,30,24,18,15,0,0,0,0;240,240,240,240,240,240,240,240,240,240,240,240,240,240,240,240,240,240,240,240,240,240,120,60,40,25,15,10,8,6,5,4,0,0,0,0;200,200,200,200,200,200,200,200,200,200,200,200,200,200,200,200,200,200,200,200,200,200,100,50,30,20,10,8,6,5,4,3,0,0,0,0;150,150,150,150,150,150,150,150,150,150,150,150,150,150,150,150,150,150,150,150,150,150,75,40,20,10,8,6,5,4,3,2,0,0,0,0;100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,50,20,10,8,6,5,4,3,2,1,0,0,0,0;0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0&total_bet_max=50,000,000.00&reel_set0=3,10,7,9,10,6,8,10,6,9,9,3,9,7,10,9,4,9,6,5,6,6,5,10,10,10,10,4,8,6,7,6,10,3,8,2,5,8,4,10,9,10,8,8,9,6,3,1,6,9,10,6,6,6,8,4,8,6,10,3,8,5,9,6,10,9,8,1,9,4,8,7,9,6,9,9,8,9,9,9,2,9,5,10,10,9,7,7,10,5,8,9,3,5,8,9,7,6,4,8,10,4,9,7,8,8,8,8,9,8,7,9,10,4,7,9,8,10,9,3,8,10,8,9,9,10,7,9,6,8,8,7,7,7,9,8,8,5,6,7,5,7,4,10,5,5,8,7,7,6,10,9,7,6,6,10,9,5,9,10~4,6,8,8,9,10,10,9,8,5,8,9,6,7,9,10,10,1,10,9,2,10,8,8,9,10,8,6,6,7,5,10,7,8,10,8,8,8,6,10,4,8,9,9,5,6,8,9,5,7,6,6,10,10,8,1,9,10,10,8,7,9,10,6,7,5,9,10,4,6,9,9,10,8,10,10,10,10,8,10,9,6,9,8,10,8,10,7,9,9,8,10,4,7,6,8,10,5,9,2,9,10,8,7,8,5,7,6,8,8,10,7,10,5,9,9,9,9,7,6,4,5,9,3,6,6,10,3,9,7,3,9,8,10,8,8,9,3,10,4,6,9,10,10,7,8,10,8,3,7,10,9,8,6,10~6,10,8,6,7,6,10,10,9,7,8,7,4,3,8,6,2,7,7,9,5,6,10,9,6,9,9,8,9,9,5,10,6,10,10,8,5,8,8,8,7,8,4,7,3,9,9,10,9,8,5,10,5,10,9,7,9,8,8,9,8,5,8,2,7,9,6,5,9,5,10,9,6,8,8,6,6,6,4,10,10,4,8,8,3,9,10,8,10,10,7,8,6,9,6,7,5,10,10,8,10,1,9,10,8,9,6,8,9,4,1,10,9,9,5,8,7~10,10,5,8,9,7,7,10,7,6,3,5,9,9,6,8,10,9,5,9,3,9,6,6,7,9,7,5,6,10,6,6,3,10,6,9,9,10,9,9,9,8,10,10,8,9,10,10,6,10,8,9,9,4,2,5,5,8,8,9,6,7,5,10,10,5,10,8,7,7,6,5,1,8,8,10,8,6,6,6,9,4,10,6,10,5,8,9,2,10,8,8,3,8,7,7,9,8,7,5,8,8,10,4,10,9,5,9,10,10,8,10,1,9,9,5,10,7,7,8~9,4,8,7,7,6,7,8,10,10,4,9,5,9,8,7,6,9,9,6,7,7,7,10,8,7,9,3,5,8,9,10,10,8,8,9,9,8,5,7,9,5,8,5,9,9,9,9,5,6,10,10,7,10,3,8,9,7,4,8,10,3,2,10,4,10,10,8,8,8,10,1,6,10,8,10,10,7,7,5,9,1,8,4,6,8,8,4,6,7,8,10,10,10,9,7,9,2,10,8,9,7,10,10,3,8,7,4,6,6,9,5,9,9,5,5~8,5,6,8,10,10,9,9,7,6,8,6,9,10,9,5,6,5,6,4,6,6,2,9,10,6,8,9,8,2,4,10,8,9,7,9,7,7,7,1,10,8,9,6,6,8,10,9,6,3,9,10,10,7,10,8,8,10,7,10,10,7,8,9,4,3,7,10,7,10,7,5,10,9,8,6,10,10,10,7,4,7,7,8,9,9,5,6,4,10,8,4,10,7,5,10,8,6,10,3,10,9,7,8,9,6,7,3,6,9,7,8,8,9,7,8,7,8,8,8,8,7,9,9,8,6,8,9,6,9,8,9,7,7,8,9,6,8,4,7,4,8,8,6,10,8,4,9,3,9,8,8,6,7,9,5,9,3,8,9,9,9,8,7,10,4,10,9,8,10,10,5,8,10,5,9,9,10,8,10,4,5,7,1,6,8,5,6,10,8,10,8,8,3,7,8,6,10,5&reel_set2=4,9,7,1,7,10,8,10,6,1,9,8,8,7,1,8,5,1,6,6,8,10,1,6,8,5,9,9,8,5,10,9,1,8,8,6,9,9,9,8,9,9,2,6,8,8,9,4,5,9,7,9,10,6,9,9,10,6,1,3,7,5,8,10,6,8,6,9,2,8,9,6,5,9,4,1,10,8,8,8,6,5,1,6,8,10,8,1,10,3,1,9,9,7,3,1,7,10,6,10,10,7,7,1,6,1,4,8,9,8,10,1,9,7,1,10,6,6,6,8,8,1,5,4,6,10,7,10,1,10,4,10,9,5,9,5,8,1,7,7,9,6,8,10,10,9,7,7,9,1,9,8,4,9,4,7,9,6,9,1~1,9,4,3,8,10,9,7,9,9,10,6,10,10,7,5,5,6,8,10,3,9,10,9,4,5,10,10,6,8,9,9,9,8,10,8,10,7,8,9,9,7,10,9,5,10,10,9,8,5,7,6,10,8,10,1,6,6,7,6,10,6,6,8,6,6,6,8,5,5,7,8,10,8,5,8,6,8,7,6,5,4,7,8,4,9,9,8,7,9,10,5,9,9,7,9,7,7,10,8,8,8,10,7,10,5,3,8,8,5,3,10,9,10,8,2,10,9,5,6,10,5,6,7,8,8,9,9,2,6,9,10,7,10,10~5,9,6,2,8,7,10,7,5,3,6,7,6,10,9,8,6,8,8,5,9,7,8,5,7,7,7,8,4,7,10,8,7,10,3,8,9,8,6,9,9,10,8,10,6,10,7,1,9,5,6,7,10,10,10,9,10,6,10,4,10,9,8,4,7,9,7,7,8,9,9,8,4,6,3,10,8,7,8,9,7,8,8,8,8,6,7,10,8,10,9,5,6,8,3,7,10,5,7,1,3,9,6,9,10,8,9,2,9,9,9,6,7,4,8,6,9,8,8,5,7,8,6,8,10,10,9,3,4,9,10,10,8,5,7,10,4~7,10,9,1,6,10,1,10,4,8,1,9,5,8,6,1,5,1,8,8,10,1,9,10,8,10,1,10,7,1,10,10,8,1,6,7,9,8,4,10,9,6,10,10,10,10,6,2,7,9,10,9,7,8,3,1,3,4,6,5,10,3,8,9,9,5,10,9,10,8,6,9,10,8,1,7,6,8,1,6,1,6,7,9,10,9,8,3,6,5,8,9,9,9,6,6,10,1,10,7,1,5,6,6,9,9,4,8,8,10,1,5,1,7,8,1,5,10,5,7,3,9,1,7,8,6,2,9,10,9,9,8,10,9,10,1,10,4,8,6,9~3,7,8,10,9,6,7,9,4,9,10,8,4,10,9,6,10,7,9,9,6,9,4,10,10,6,9,10,9,4,8,5,3,8,9,6,10,7,6,9,4,1,7,7,7,5,8,2,6,10,9,5,9,7,9,10,8,10,7,7,9,10,9,8,8,10,2,10,8,10,10,8,4,9,8,10,4,9,9,8,8,10,8,10,9,8,10,7,3,9,9,9,9,8,5,10,6,8,3,9,7,8,7,6,10,7,8,9,5,10,7,6,7,5,7,7,9,9,8,5,8,1,5,9,7,8,5,10,10,9,7,6,8,4,8,10,5~1,10,9,8,1,8,7,4,9,9,8,7,5,9,9,8,1,10,1,10,1,9,6,10,8,10,5,7,7,6,9,6,1,5,7,9,8,9,9,2,7,9,10,8,10,1,10,1,9,4,6,7,9,2,10,1,8,1,10,8,8,9,9,7,9,7,10,1,9,10,10,1,9,8,9,6,5,6,6,6,10,9,6,9,9,1,10,9,9,1,6,1,8,1,5,10,9,4,1,8,10,1,6,9,6,8,8,3,8,7,10,6,10,1,9,3,9,8,9,4,9,5,8,1,6,5,6,6,8,5,8,8,7,8,10,10,5,8,1,10,6,6,7,1,6,5,9,5,8,10,5,9,5,9,10,8,6,10,1&reel_set1=9,4,10,9,4,8,8,2,9,10,8,6,5,7,9,3,4,9,7,6,10,10,10,10,7,7,10,3,10,6,6,10,6,8,5,6,9,10,4,7,10,9,9,10,8,8,5,9,9,9,9,8,3,9,8,9,8,10,10,9,10,5,8,9,6,8,3,9,8,4,8,9,4,6,8,8,8,8,9,7,6,10,6,9,7,9,6,9,8,9,10,10,1,8,6,9,8,10,10,9,4,7,7,7,7,10,9,4,7,6,9,3,10,10,9,7,4,8,9,9,5,6,5,3,8,6,8,5,5,5,7,10,10,5,6,6,8,10,8,7,9,6,8,8,9,9,10,7,10,6,7,4,5,6,6,6,6,5,7,8,10,3,8,5,7,10,8,8,7,2,9,7,7,3,7,6,9,7,4,4,4,8,8,7,4,10,5,7,10,10,8,7,1,10,9,7,6,7,3,4,10,7,3,5,5~9,5,6,3,10,3,10,5,10,10,4,9,8,4,8,8,8,8,5,5,3,10,8,9,2,7,7,8,8,4,9,3,6,10,10,10,10,5,8,7,5,8,1,10,10,7,4,9,10,6,9,7,8,5,5,5,10,10,8,10,10,4,9,9,4,10,10,4,4,9,8,9,6,9,9,9,9,8,10,6,8,5,5,7,1,8,10,4,6,10,10,9,6,6,6,8,3,10,3,8,9,8,7,6,9,10,7,9,5,2,8,7,7,7,4,5,9,6,6,5,9,6,6,8,10,8,7,10,7,3,4,4,4,4,10,7,9,5,5,10,8,6,7,10,9,9,6,6,9,4,4,7~9,9,6,10,10,4,8,5,8,4,5,8,5,10,7,9,8,9,9,9,9,6,5,5,6,9,6,10,3,3,7,5,9,5,7,5,8,9,10,10,10,10,7,10,6,8,7,7,8,8,2,9,10,6,5,8,3,4,2,8,8,8,8,6,10,9,10,10,9,3,4,8,10,9,9,10,7,6,7,8,9,6,7,7,7,9,10,10,6,5,9,7,6,9,8,7,4,1,6,10,6,10,10,6,6,6,6,8,7,5,3,6,8,10,7,10,9,3,8,8,3,9,10,7,8,5,5,5,9,10,7,8,8,4,10,8,9,7,7,1,8,10,4,6,9,9,4,4,4,6,7,9,8,9,6,7,9,8,7,8,4,8,10,6,9,9,7,3,7~6,10,10,7,7,8,9,2,6,7,10,7,9,7,1,9,8,8,3,9,9,4,6,10,9,8,9,8,9,10,7,9,9,9,10,3,8,6,6,4,3,7,3,9,8,9,10,8,5,8,4,10,5,8,5,10,9,8,7,9,5,6,9,7,7,5,10,10,10,8,5,4,10,8,8,6,10,6,4,5,2,5,8,7,8,10,8,9,6,9,6,8,8,10,10,8,9,8,6,10,9,8,8,8,8,10,10,8,6,4,4,10,7,8,9,5,9,6,8,9,5,5,6,6,9,5,9,10,1,9,8,7,9,5,9,7,8,6,6,6,7,10,10,7,10,9,8,9,10,8,4,10,9,10,3,6,5,5,8,6,10,8,8,9,4,9,3,10,10,4,10,7,8,3~5,9,3,7,9,9,10,8,10,9,9,10,8,8,10,7,9,8,8,8,8,4,10,8,4,7,7,2,9,9,5,9,5,8,3,4,8,9,8,10,10,10,10,9,4,3,10,8,10,3,9,5,10,5,3,8,10,10,7,9,4,6,9,9,9,9,10,10,6,6,8,3,6,9,4,9,10,10,3,4,10,10,8,9,6,6,6,6,9,5,7,8,9,8,10,1,5,10,8,8,6,5,4,9,8,9,7,7,7,6,10,5,8,4,6,7,9,6,7,10,10,9,10,9,10,4,6,5,5,5,6,7,2,6,5,5,7,10,9,9,8,8,7,5,10,8,5,6,4,4,4,4,6,10,7,10,6,8,4,4,8,3,10,8,8,9,6,7,10,7,1,3~5,6,4,6,7,9,9,3,8,6,8,5,4,9,10,9,5,10,10,10,10,9,6,1,6,7,8,8,2,7,10,10,8,5,4,5,9,10,10,8,4,4,4,7,7,5,7,8,9,3,8,5,8,6,4,6,6,10,6,9,10,4,9,9,9,9,9,6,7,7,9,10,7,10,10,8,8,5,9,9,8,6,7,7,10,8,7,7,7,7,6,9,3,4,7,4,9,8,5,8,9,10,10,9,8,7,10,3,9,8,8,8,8,6,10,7,8,8,6,10,5,7,9,6,9,9,10,9,8,8,3,10,6,6,6,6,9,7,6,6,7,8,10,8,3,4,10,10,7,10,8,7,2,7,8,5,5,5,1,3,6,10,6,5,9,7,10,10,3,9,7,4,9,9,8,10,9,4&purInit=[{bet:2000,type:\"default\"},{bet:10000,type:\"default\"}]&reel_set3=5,2,7,8,6,6,3,8,9,7,8,6,8,10,8,7,10,10,10,10,3,10,7,3,5,8,7,7,6,7,10,8,10,9,5,9,8,8,8,8,10,9,7,10,8,5,7,10,10,6,9,9,6,10,10,9,8,6,6,6,6,9,9,10,9,10,4,10,6,3,10,7,9,7,6,1,3,9,9,9,9,10,1,5,10,4,8,5,6,8,8,7,9,10,9,4,6,10,4,4,4,8,8,7,5,4,7,2,7,7,6,9,6,7,10,3,10,7,7,7,7,6,8,5,10,10,9,7,10,6,10,7,7,8,8,7,8,9,5,5,5,3,5,4,10,9,4,8,10,4,8,9,9,6,10,7,10,9,3,3,3,3,9,5,6,9,6,8,10,6,8,8,4,7,3,4,9,4,6,9,8~10,10,8,6,9,7,7,6,7,9,6,4,10,10,7,4,9,8,8,8,8,9,7,7,9,4,6,8,8,3,8,5,10,4,10,5,6,9,6,10,10,10,10,8,6,8,3,10,9,9,4,8,5,8,4,3,10,6,3,6,5,5,5,5,5,8,6,9,10,5,10,8,4,6,9,6,10,9,3,7,5,8,3,4,4,4,4,6,6,10,10,5,4,10,9,5,8,4,6,6,8,9,5,3,10,9,9,9,9,8,4,7,9,4,10,9,7,10,10,7,4,10,4,6,5,10,3,3,3,9,6,9,10,6,8,4,10,10,6,8,10,3,5,9,8,8,9,6,6,6,6,8,5,6,2,8,8,5,8,10,8,5,7,8,6,2,8,5,4,7,7,7,1,4,10,10,8,4,9,8,9,5,10,7,1,10,7,10,9,9,7~10,6,9,6,7,10,8,7,8,9,8,10,9,10,10,7,9,7,6,10,3,5,3,9,4,5,9,7,7,1,8,3,5,7,8,10,3,8,9,4,3,2,8,7,9,10,6,8,9,4,7,7,8,8,9,4,5,6,8,4,4,1,8,7,8,6,6,9,9,8,10,4,8,6,6,8,9,10,4,4,6,10,5,3,10,5,7,10,7,9,6,8,5,10,5,6,6,8,9,8,6,10,9,7,9,8,8,6,4,7,8,5,8,8,9,7,7,10,10,8,7,9,5,6,8,5,9,7,5,9,10,3,2,8,10,7,10,7,10,9,5,3,9,5,8,5,8,9,10,8,10,9,8,9~7,4,8,6,9,8,9,7,4,9,8,7,9,10,8,4,9,5,8,5,9,9,9,8,10,6,5,10,10,6,8,8,2,3,5,10,6,8,6,7,7,10,8,9,9,6,6,6,10,3,8,8,10,5,9,9,8,7,9,8,10,10,4,6,1,5,10,10,8,10,8,10,10,10,8,6,9,2,6,9,8,8,7,8,5,10,7,3,9,10,10,6,9,6,7,7,8,8,8,8,10,9,4,9,9,4,6,7,9,10,3,9,1,10,10,9,10,9,9,5,10,6,7,5~10,10,9,6,10,9,6,2,6,9,7,6,7,10,10,10,10,7,6,9,10,1,9,10,7,5,6,9,7,7,1,8,8,8,8,6,6,8,9,10,8,10,8,5,8,3,8,7,10,6,6,6,6,9,8,9,9,10,7,3,6,5,4,6,9,4,8,9,9,9,9,4,4,8,6,4,5,10,6,7,8,7,6,5,7,8,4,4,4,9,4,8,10,7,9,9,3,5,10,10,9,8,10,7,7,7,7,2,5,9,10,8,8,7,5,3,10,10,9,7,8,5,5,5,8,6,9,10,10,4,7,10,8,7,7,8,10,3,3,3,3,8,9,10,4,10,6,3,9,8,8,9,7,5,6,6,3~10,6,10,9,10,7,9,8,9,5,3,6,4,8,8,3,1,10,8,8,8,8,3,10,4,10,8,6,8,7,6,10,5,5,8,6,6,1,9,4,6,9,10,10,10,10,9,6,3,7,5,4,6,9,4,2,10,5,10,10,9,4,6,7,8,9,5,5,5,5,5,7,6,8,3,8,7,7,10,9,7,4,10,8,10,8,5,10,8,10,8,4,4,4,4,10,5,8,5,10,6,8,5,9,6,6,5,6,8,6,10,8,6,6,8,9,9,9,9,10,10,5,3,8,4,10,9,9,8,4,8,10,6,7,8,9,5,7,8,3,3,3,10,9,10,8,9,4,8,4,3,9,8,9,4,6,8,10,4,8,9,10,6,6,6,6,5,3,4,6,10,5,3,9,9,4,4,7,7,8,10,10,6,5,8,4,7,7,7,9,8,5,10,6,5,7,9,9,7,9,2,10,6,4,3,10,10,7,4,9&total_bet_min=200.00";
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
            get { return 2; }
        }
        public double[] PurchaseMultiples
        {
            get { return new double[] { 100, 500 }; }
        }
        #endregion
        public GatesOfHadesGameLogic()
        {
            _gameID = GAMEID.GatesOfHades;
            GameName = "GatesOfHades";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                GatesOfHadesBetInfo betInfo = new GatesOfHadesBetInfo();
                betInfo.BetPerLine  = (float)message.Pop();
                betInfo.LineCount   = (int)message.Pop();

                if (message.DataNum >= 3)
                {
                    betInfo.PurchaseFree = true;
                    betInfo.PurchaseType = (int)message.GetData(2);
                }
                else
                    betInfo.PurchaseFree = false;

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in GatesOfHadesGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                    (oldBetInfo as GatesOfHadesBetInfo).PurchaseType = betInfo.PurchaseType;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in GatesOfHadesGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            GatesOfHadesBetInfo betInfo = new GatesOfHadesBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new GatesOfHadesBetInfo();
        }
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as GatesOfHadesBetInfo).PurchaseType;
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
        protected override async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int agentID, BasePPSlotBetInfo betInfo, double baseBet, UserBonus userBonus)
        {
            int purchaseType = (betInfo as GatesOfHadesBetInfo).PurchaseType;
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
                int purchaseType = (betInfo as GatesOfHadesBetInfo).PurchaseType;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, -1, PurchaseMultiples[purchaseType] * 0.2, PurchaseMultiples[purchaseType] * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in GatesOfHadesGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int purchaseType = (betInfo as GatesOfHadesBetInfo).PurchaseType;
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in GatesOfHadesGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = (betInfo as GatesOfHadesBetInfo).PurchaseType.ToString();
        }
        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            GatesOfHadesBetInfo starBetInfo = betInfo as GatesOfHadesBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, -1, betInfo.PurchaseFree ? starBetInfo.PurchaseType : -1, betMoney);
        }
    }
}
