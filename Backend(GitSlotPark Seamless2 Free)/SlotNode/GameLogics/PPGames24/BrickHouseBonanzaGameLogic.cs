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
    class BrickHouseBonanzaBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get
            {
                return BetPerLine * 20;
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

    class BrickHouseBonanzaGameLogic : BasePPSlotGame
    {
        protected double[] _multiTotalFreeSpinWinRates;
        protected double[] _multiMinFreeSpinWinRates;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vswaysbrickhos";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 243; }
        }
        protected override int ServerResLineCount
        {
            get { return 20; }
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
                return "def_s=3,10,11,4,6,5,12,13,7,13,11,8,4,3,6&cfgs=1&ver=3&mo_s=1;2;3&mo_v=12,20,30;40,60,80,100,120,240,1200,480;200,300,400,500,2000,5000,240,1200,10000,100000,480&def_sb=4,10,13,11,12&reel_set_size=10&def_sa=5,6,5,4,13&mo_jp=240;1200;480;10000;100000&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{purchase_1:\"96.50\",purchase_0:\"96.50\",regular:\"96.50\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"12212629\",max_rnd_win:\"10000\",bw_awards:\"240,1200,10000,100000\"}}&wl_i=tbm~10000&mo_jp_mask=jp1;jp3;jp2;jp4;jp5&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1,1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;40,8,3,0,0;25,7,3,0,0;20,5,2,0,0;15,4,1,0,0;7,3,1,0,0;4,2,1,0,0;4,2,1,0,0;4,2,1,0,0;4,2,1,0,0;3,2,1,0,0;3,2,1,0,0;0,0,0,0,0&total_bet_max=20,000,000.00&reel_set0=7,6,6,1,12,6,7,5,7,6,9,3,10,6,4,6,1,7,3,13,10,8,7,1,1,1,5,5,3,14,13,7,11,6,7,8,4,4,6,7,6,3,7,6,6,14,6,3,8,6,7,3,3,3,3,14,3,6,14,7,4,6,14,1,6,7,1,4,7,1,6,3,3,4,9,9,1,7,6,6,11,4,9,13,8,6,7,7,6,11,7,6,1,10,7,5,6,4,6,14,1,12,4,4,4,6,6,7,5,7,4,11,5,7,5,3,7,9,7,1,7,7,5,7,7,7,6,7,7,7,7,11,7,7,5,6,4,1,4,3,1,1,10,5,8,6,3,7,7,1,4,7,14,4,3,3~7,1,5,6,13,5,11,1,5,2,12,3,2,1,11,7,5,6,1,1,1,1,6,3,10,4,2,3,14,7,11,7,13,7,2,5,11,7,10,3,3,3,5,7,6,9,7,1,11,12,1,5,7,7,1,3,5,3,6,6,8,9,5,1,6,4,10,12,7,5,12,2,11,5,7,3,6,6,4,4,4,5,12,7,6,6,3,14,3,7,7,5,6,1,13,14,5,6,5,5,5,1,12,7,5,4,5,5,6,10,5,3,13,6,7,11,1,4,7,6,6,6,11,6,2,10,11,8,5,14,10,5,1,14,7,11,2,6,10,4~8,1,12,3,1,4,7,14,14,6,4,1,1,7,7,5,1,1,1,5,7,12,1,7,7,9,10,5,5,9,14,1,9,4,8,12,5,7,4,3,3,3,8,7,5,11,1,2,5,8,7,12,5,1,5,13,7,4,14,8,7,6,9,7,1,6,14,7,1,7,3,8,8,10,4,4,4,7,4,2,6,6,2,12,7,8,8,7,5,12,1,9,1,7,7,7,4,5,5,6,3,11,7,4,4,3,11,3,1,13,6~7,9,4,7,12,6,1,4,7,11,7,1,7,7,9,11,1,1,1,7,7,8,3,9,7,6,8,10,11,10,10,5,1,1,6,12,3,3,3,9,7,9,7,3,10,8,3,4,4,7,10,6,7,11,7,5,3,4,4,4,7,4,7,3,7,4,8,5,7,2,5,3,3,7,4,2,11,7,7,13,7,6,8,10,3,7,7,10,12,4,7,7,8,4,4,5,5,5,7,13,8,9,14,1,13,5,10,7,7,8,4,9,13,3,6,6,6,7,4,7,7,3,8,3,7,4,4,3,8,7,8,5,5,7,4,7,7,7,11,14,10,11,3,7,12,6,11,10,6,5,6,7,7,5,7,8,4~7,10,9,1,11,4,4,10,7,13,5,3,11,8,7,3,1,7,8,9,7,1,14,7,1,1,1,7,3,10,3,3,7,8,7,7,1,9,4,8,5,4,8,4,1,13,7,7,7,3,3,3,5,10,5,1,6,3,11,6,14,12,7,13,7,1,1,5,9,4,7,5,1,5,7,4,4,4,7,3,8,4,1,4,8,5,8,7,4,3,7,12,7,1,7,7,5,10,5,5,7,1,8,8,11,11,13,8,3,11,8,3,14,11,12,7,11,8,3,6,12,8,10,1,7,11,4,5,5,5,8,1,5,14,4,1,3,14,8,3,7,6,11,6,4,4,3,8,7,3,7,6,7,7,11,14,7,7,7,8,8,1,12,7,11,1,7,8,8,8,4,3,7,7,5,7,11,11,7,5,7,7,1,13,13,7,6&reel_set2=9,6,13,7,4,7,11,1,7,3,4,6,1,4,6,7,11,6,1,1,1,5,10,7,1,11,7,6,1,6,5,5,7,6,6,7,14,3,14,13,6,3,3,3,3,7,6,1,4,7,7,12,8,5,3,6,4,3,7,7,5,14,1,7,4,1,5,7,4,1,4,3,3,6,7,11,4,3,4,4,4,7,6,10,6,7,9,6,6,5,7,6,6,1,7,4,9,7,8,7,7,7,7,6,7,14,3,1,7,8,9,6,8,3,3,12,7,7,13,10,6~6,1,6,7,4,10,1,11,5,6,7,5,1,14,1,1,1,1,7,6,7,6,11,3,9,3,7,1,5,5,11,3,3,3,3,1,5,12,3,4,3,6,7,8,7,10,12,5,11,1,13,5,6,10,4,2,6,11,6,14,12,5,2,4,6,14,5,1,5,4,4,4,12,14,5,6,11,14,6,13,3,7,2,7,2,5,6,5,9,5,5,5,8,7,1,7,11,6,5,10,2,11,11,13,6,6,6,1,3,2,12,7,7,12,7,5,10,4,5,10,5,7~6,3,11,8,11,7,5,4,5,6,4,12,7,1,3,3,7,7,1,1,1,7,6,8,4,2,1,9,10,1,8,14,4,13,7,7,1,6,3,3,3,8,5,1,3,1,1,1,1,1,7,7,9,5,7,5,8,2,7,12,6,5,7,5,1,10,1,2,7,6,6,1,4,4,4,4,4,7,1,7,4,9,9,7,8,8,4,13,7,14,14,7,7,7,8,5,14,7,5,5,12,5,14,12,7,8,7,12,5,12,4,9,6,3~14,10,5,3,7,10,7,11,6,7,7,1,1,1,9,4,7,7,8,9,1,6,3,4,3,3,3,8,8,2,1,7,8,7,9,13,7,5,11,8,4,4,4,7,4,8,5,4,7,3,7,9,11,4,7,1,3,4,4,11,5,7,5,1,3,10,5,5,5,12,7,11,6,6,3,7,6,7,13,10,7,6,6,6,3,7,10,10,5,12,6,7,3,3,7,7,7,7,4,10,7,9,2,7,8,12,4,7,4,11,7,8,13~7,4,3,11,4,3,1,8,5,7,1,5,4,12,7,7,3,7,13,1,1,1,7,7,8,5,7,6,11,3,6,3,8,1,10,7,11,4,1,6,1,5,5,4,5,3,3,3,5,7,5,7,12,1,8,8,7,7,1,11,7,3,11,12,7,1,8,12,7,4,4,4,1,10,8,1,8,4,5,6,8,8,7,7,3,10,8,7,6,13,8,4,9,3,11,8,11,4,7,3,13,7,7,5,11,1,5,8,1,8,8,7,5,5,5,7,8,11,7,11,7,8,5,7,5,7,7,11,4,3,7,8,4,4,6,1,7,7,7,7,13,7,8,7,8,6,3,8,3,8,7,6,1,9,1,3,7,3,4,7,7,3,8,8,8,4,10,7,8,11,11,12,9,13,1,1,11,10,12,7,5,3,7,7,13,9,7,4&reel_set1=9,3,3,6,6,7,1,6,7,5,4,9,7,1,7,6,6,1,9,1,6,1,1,1,10,1,7,7,3,6,3,3,7,6,13,3,5,8,4,4,1,8,6,7,11,3,3,3,3,1,7,4,14,7,6,7,5,4,1,12,6,3,7,5,7,7,3,7,6,5,14,7,6,6,7,10,7,1,4,7,1,3,8,4,10,5,7,14,7,3,9,4,6,5,7,4,4,4,6,11,8,6,5,11,7,13,7,13,9,4,6,14,11,6,12,14,8,7,6,7,7,7,7,6,6,5,3,1,4,1,7,6,4,6,7,6,7,4,3,7,14,6,7,11,6~5,6,10,12,6,10,1,13,3,12,9,5,1,5,12,1,1,1,11,6,7,7,10,11,5,3,5,6,7,3,5,7,7,3,3,3,8,7,5,11,9,5,7,7,2,5,6,1,3,6,6,11,10,7,1,14,1,1,3,12,10,11,5,1,7,4,6,5,3,4,4,4,6,6,5,3,5,7,4,14,2,7,10,6,11,14,2,5,12,5,5,5,8,7,4,7,5,4,14,5,6,4,2,6,6,1,7,7,6,6,6,13,3,1,2,1,11,5,11,1,7,14,6,11,2,12,5,13~2,9,11,14,6,4,11,8,7,7,9,1,12,1,7,1,1,7,10,2,3,8,1,1,1,12,12,8,1,8,4,6,5,10,5,1,6,1,7,7,6,7,9,4,4,14,3,12,3,3,3,7,2,5,1,14,4,14,4,7,7,4,14,6,8,11,1,2,5,8,6,12,13,12,1,7,6,7,3,4,5,1,5,1,13,5,5,7,1,8,1,7,4,4,4,7,5,11,7,14,5,1,7,6,1,9,9,5,8,7,4,3,7,7,7,9,5,3,8,3,7,7,12,4,5,6,1,5,8,8,5,7,7~1,7,7,3,10,7,10,7,10,7,7,6,4,3,1,5,10,7,5,7,1,1,1,13,3,6,3,11,8,7,8,2,1,7,7,9,7,11,8,7,7,8,4,3,3,3,8,4,13,14,11,9,3,10,6,8,6,5,4,8,7,1,5,4,3,4,4,4,3,4,5,4,13,4,11,12,11,7,9,6,11,7,10,7,3,4,7,1,7,10,10,7,8,7,6,5,4,13,8,9,7,7,4,11,14,4,7,5,5,5,3,7,3,7,7,4,7,3,5,9,7,8,6,11,7,12,3,7,5,2,4,6,6,6,7,9,12,10,7,7,12,9,6,4,3,7,8,1,6,7,3,4,11,7,5,10,7,7,7,5,7,4,7,8,8,6,10,11,7,4,5,7,9,4,3,7~8,13,11,3,13,11,11,5,3,7,11,11,7,12,11,7,3,14,5,5,10,11,8,3,3,3,12,7,8,6,7,9,10,7,7,8,7,4,7,7,13,7,4,11,8,7,7,8,9,5,4,4,4,7,6,13,7,3,6,7,3,13,7,5,10,3,5,7,11,4,5,8,7,8,7,5,13,8,3,13,4,7,7,4,7,12,3,8,8,11,7,4,7,7,10,8,7,4,12,7,8,5,5,5,11,5,7,11,5,13,8,11,7,8,8,6,9,8,4,7,7,6,7,11,8,7,7,7,3,10,14,3,6,9,4,13,13,7,4,7,14,7,3,3,7,8,14,14,5,7,12&reel_set4=1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1~7,8,7,11,1,10,3,6,9,3,8,1,1,1,8,7,7,1,6,11,8,7,11,11,10,9,5,5,5,11,11,6,1,9,5,8,10,7,11,7,4,7,7,7,13,7,5,7,7,10,7,9,8,3,9,5,5,8~1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1~5,11,4,7,7,11,1,1,1,7,8,8,7,5,9,4,4,4,10,12,3,6,7,1,8,5,5,5,10,11,11,10,4,4,3,7,7,7,12,5,7,9,13,10,9,8~11,7,9,1,7,8,10,12,7,11,7,7,1,1,1,9,1,1,8,1,1,10,7,5,7,8,6,11,4,7,7,7,1,3,7,1,3,6,10,8,12,11,1,11,7,8,8,8,3,4,7,9,13,8,4,8,8,3,1,1,5,8&purInit=[{bet:2000,type:\"default\"},{bet:4000,type:\"default\"}]&reel_set3=7,7,13,6,5,5,6,3,7,3,4,7,7,6,1,6,13,1,6,8,7,4,11,7,1,1,1,6,13,7,7,1,8,6,6,7,7,5,6,10,7,7,5,7,4,3,8,7,7,3,3,3,3,4,7,1,6,5,8,7,7,1,1,13,4,7,12,10,7,7,7,9,7,8,3,5,4,4,4,3,13,7,6,7,3,4,3,4,3,8,7,11,1,6,8,7,5,11,4,6,5,9,7,5,7,9,6,6,3,1,13,7,11,6,6,3,7,6,1,3,7,6,4,3,5,7,6,7,11,5,5,5,9,6,7,9,1,7,4,4,3,6,1,12,12,11,7,7,8,7,7,4,7,10,7,7,6,7,7,7,7,7,6,5,7,4,1,6,7,5,7,6,7,5,6,9,5,6,5,1,7,6,12,6,4,4~5,7,10,11,10,7,8,5,10,7,4,1,11,6,6,13,3,4,2,1,1,1,9,3,10,3,11,10,6,3,5,1,11,5,4,4,9,5,4,7,5,12,3,3,3,5,5,6,7,5,10,3,5,6,1,6,11,6,10,12,7,7,5,6,2,6,6,6,7,8,1,7,10,7,1,7,5,1,7,7,1,4,4,4,7,11,7,12,5,6,5,7,5,2,10,5,12,7,5,12,7,7,13,6,5,5,5,7,12,9,3,12,6,11,7,4,1,7,2,7,1,13,7,6,11,5,1,4,11,6,6,6,11,1,6,5,1,11,6,4,7,7,5,6,5,10,7,1,2,3,9~9,9,4,10,1,4,5,8,7,7,5,13,9,7,12,1,1,1,8,2,1,4,5,1,10,7,7,8,4,7,6,7,5,7,12,9,6,3,3,3,7,6,5,12,8,5,7,2,7,5,6,2,9,7,5,12,7,6,7,11,13,3,1,5,5,6,7,7,4,7,9,1,4,4,4,8,5,4,11,1,7,4,11,6,4,7,8,3,5,1,7,7,7,7,5,6,7,1,7,3,4,1,7,12,3,6,9,8,7,7~1,6,7,12,7,4,5,8,7,11,7,4,3,3,3,6,6,1,12,7,7,9,6,7,10,5,7,9,7,3,6,1,8,1,1,10,3,4,5,7,10,7,8,4,4,4,3,4,1,4,6,3,10,3,7,1,3,1,7,7,5,5,5,2,7,5,1,8,11,4,7,4,7,11,3,7,6,6,6,7,4,4,9,2,9,7,7,13,7,4,7,7,13,7,7,7,1,4,10,12,3,2,5,7,7,4,7,3,7,3,10~1,8,4,5,5,7,3,1,7,4,1,1,1,7,7,8,7,11,3,8,12,1,1,3,4,3,7,12,3,3,3,11,5,13,7,7,5,10,1,8,5,7,3,7,8,13,5,9,8,7,7,13,7,4,4,4,4,4,9,11,8,3,8,5,11,3,1,8,3,1,5,5,5,6,4,11,13,4,7,6,7,7,4,7,12,6,7,7,7,5,3,1,11,6,7,10,12,5,7,7&reel_set6=6,5,6,4,6,5,9,5,10,3,3,3,7,5,4,7,3,4,4,7,6,4,4,4,6,6,7,8,5,6,3,9,7,6,6,6,11,7,6,7,4,6,7,6,7,7,7,3,3,7,9,8,7,4,7,4,7,3~7,9,6,5,5,3,12,6,5,11,3,6,7,13,7,5,5,5,5,5,4,4,5,4,6,6,7,4,5,5,4,6,6,4,5,7,3,6,6,6,7,3,8,5,7,11,6,5,2,6,7,2,10,5,4,5,7,7,7,5,6,7,7,2,5,4,4,7,12,7,9,5,6,5,5,7~7,5,7,5,6,3,7,7,8,5,5,10,7,7,8,9,5,5,5,7,3,5,2,7,5,5,4,6,5,4,11,6,5,5,8,10,4,6,6,6,9,4,8,6,7,7,2,8,7,6,6,5,5,6,7,12,6,7,7,7,7,5,7,7,3,5,7,6,6,4,4,7,8,11,2,6,4,7,5,8~7,4,5,5,4,5,6,10,3,9,11,6,7,10,3,3,3,4,7,7,5,4,12,2,5,6,3,5,3,5,11,7,4,4,4,4,4,6,7,4,7,3,3,6,8,3,9,3,8,4,5,7,5,5,5,12,8,3,7,2,10,7,6,7,7,3,4,7,7,7,4,11,4,7,4,5,9,5,6,3,4,12,4,7,3,10~7,5,7,5,7,5,5,7,4,3,4,8,4,4,4,3,7,5,4,5,8,6,13,7,6,5,5,6,5,5,5,7,8,5,4,7,4,3,6,4,7,3,7,6,5,6,6,6,7,6,5,10,7,3,5,9,6,8,7,11,13,7,7,7,8,7,6,5,5,9,11,4,6,6,7,3,12,10,8&reel_set5=13,14,3,4,5,9~7,11,6,11,14,7,7,10,12~14,3,5,6,8,10,10~11,12,14,4,7,10~14,4,6,7,7,8&reel_set8=1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1~1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1~1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1~1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1~1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1&reel_set7=1,1,1,1,1,1,1,1,1,1,1,1,6,1,1,1,1,1,1,1,1,1,1,1,7,1,1,1,1,1,1,1,1,1,1,1,1,8,7,1,1,1,3,1,1,1,4,1,1,3,5,1,1,1,4,1,6,1,1,7,1,1,1,1,1~7,5,1,1,1,1,1,1,1,1,1,1,1,1,6,1,5,1,1,1,1,1,1,1,1,1,1,1,1,9,1,1,5,1,1,1,1,1,5,3,1,1,1,1,1,1,1,6,1,1,1,7,4,1,1,4,3,1~1,7,1,6,1,4,1,1,1,1,1,1,7,1,1,1,1,1,1,1,1,1,1,1,1,8,6,1,1,1,1,1,1,1,1,1,1,1,1,7,1,1,1,1,5,1,1,1,1,1,1,7,1,1,1,1,1,4,1,1,1,1,1,5,1,1,1,3,1,1,1,7~1,1,4,1,1,1,5,6,1,1,1,1,1,1,1,1,1,1,5,7,1,1,1,1,1,1,9,3,1,1,1,1,1,1,1,1,1,1,1,1,3,1,1,1,1,6,1,3,1,1,1,7,1,1,1,1,1,1,4,1,1,1,7,1,1,1,1~5,1,3,1,1,1,1,1,1,4,1,1,1,1,1,1,5,1,7,1,1,1,1,7,1,1,1,1,1,1,3,1,1,1,7,1,1,1,1,1,1,1,1,1,1,1,8,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,5,1,1,1,1,1,1,1,7,1,1,1,1,7,1,5,1,7,1,1,1,1,5&reel_set9=7,5,5,11,6,4,5,8,3,6,3,3,3,7,7,4,7,3,7,4,6,6,7,4,4,4,4,6,7,4,8,7,6,7,4,3,4,4,9,5,5,5,4,3,5,6,5,7,9,7,6,7,9,6,6,6,4,6,3,6,6,7,6,10,5,4,5,7,7,7,4,7,7,4,3,6,6,9,5,7,7,6,7~7,5,5,6,5,6,6,4,5,5,6,6,4,4,4,7,7,4,6,6,7,5,7,6,7,5,7,4,7,5,5,5,5,5,5,6,5,5,6,5,7,5,4,7,5,4,5,5,6,6,6,7,7,5,5,7,5,5,7,4,5,7,4,4,7,7,7,7,5,6,7,5,7,6,4,5,6,5,4,6,6,5,7,4~3,8,12,8,11,11,3,8,9,8,10,11,12,3,3,3,3,13,9,3,12,8,3,9,11,3,3,8,3,13,10,9,8,8,8,3,8,10,12,3,13,8,12,3,3,10,11,3,11,8,8~7,9,4,6,6,3,3,3,8,3,9,3,2,4,4,4,4,4,9,10,5,5,4,8,3,5,5,5,5,7,3,4,5,10,11,6,6,6,3,6,4,10,7,7,3,7,7,7,6,7,7,4,5,4,9,9,9,5,7,7,5,12,6,6,7~5,7,11,6,3,8,5,4,4,4,4,7,7,8,3,5,5,10,6,5,5,5,8,10,6,6,7,7,4,5,7,7,6,6,6,5,8,6,8,6,7,5,4,7,4,7,7,7,7,4,8,7,5,9,7,7,5,7,7,8,8,8,5,7,5,9,12,11,5,5,4,5&total_bet_min=200.00";
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
            get { return new double[] { 100, 200 }; }
        }
        #endregion
        public BrickHouseBonanzaGameLogic()
        {
            _gameID     = GAMEID.BrickHouseBonanza;
            GameName    = "BrickHouseBonanza";
        }
        protected override void supplementInitResult(Dictionary<string, string> dicParams, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {
            base.supplementInitResult(dicParams, betInfo, spinResult);
            if (!dicParams.ContainsKey("reel_set"))
                dicParams["reel_set"] = "0";
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            //Dictionary<string, string> resultParams = new Dictionary<string, string>();
            //foreach (KeyValuePair<string, string> pair in spinParams)
            //    resultParams.Add(pair.Key, pair.Value);

            //string[] toCopyParams = new string[] { "fs", "fswin", "fsres", "fsmax", "fsmul", "trail", "reel_set", "g" };
            //for (int i = 0; i < toCopyParams.Length; i++)
            //{
            //    if (!bonusParams.ContainsKey(toCopyParams[i]))
            //        continue;
            //    resultParams[toCopyParams[i]] = bonusParams[toCopyParams[i]];
            //}

            //string[] toRemoveParams = new string[] { "mo", "mo_t" };
            //for (int i = 0; i < toRemoveParams.Length; i++)
            //{
            //    if (resultParams.ContainsKey(toRemoveParams[i]))
            //        resultParams.Remove(toRemoveParams[i]);
            //}

            //return resultParams;

            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "s", "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set", "bw" };
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
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
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


                BrickHouseBonanzaBetInfo betInfo = new BrickHouseBonanzaBetInfo();
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in BrickHouseBonanzaGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                    (oldBetInfo as BrickHouseBonanzaBetInfo).PurchaseType = betInfo.PurchaseType;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BrickHouseBonanzaGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            BrickHouseBonanzaBetInfo betInfo = new BrickHouseBonanzaBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new BrickHouseBonanzaBetInfo();
        }
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as BrickHouseBonanzaBetInfo).PurchaseType;
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
            int purchaseType = (betInfo as BrickHouseBonanzaBetInfo).PurchaseType;
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
                int purchaseType = (betInfo as BrickHouseBonanzaBetInfo).PurchaseType;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, -1, PurchaseMultiples[purchaseType] * 0.2, PurchaseMultiples[purchaseType] * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BrickHouseBonanzaGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int purchaseType = (betInfo as BrickHouseBonanzaBetInfo).PurchaseType;
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BrickHouseBonanzaGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = (betInfo as BrickHouseBonanzaBetInfo).PurchaseType.ToString();
        }
        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            BrickHouseBonanzaBetInfo starBetInfo = betInfo as BrickHouseBonanzaBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, -1, betInfo.PurchaseFree ? starBetInfo.PurchaseType : -1, betMoney);
        }
    }
}
