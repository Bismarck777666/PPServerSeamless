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
    class SleepingDragonBetInfo : BasePPSlotBetInfo
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

    class SleepingDragonGameLogic : BasePPSlotGame
    {
        protected double[] _multiTotalFreeSpinWinRates;
        protected double[] _multiMinFreeSpinWinRates;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25sleepdrag";
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
                return "def_s=8,5,6,6,5,9,10,10,7,3,6,4,11,6,11&cfgs=1&ver=3&def_sb=8,10,3,9,11&reel_set_size=9&def_sa=7,5,3,6,10&scatters=1~2,2,2,0,0~0,0,0,0,0~1,1,1,1,1#1~0,0,0,0,1~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{purchase_1:\"96.50\",purchase_0:\"96.50\",regular:\"96.50\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"50337437\",max_rnd_win:\"15000\"}}&wl_i=tbm~15000&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&purInit_e=1,1&wilds=2~0,0,0,0,0~1,1,1,1,1#2~0,0,0,0,0~1,1,1,1,1&bonuses=0&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;60,20,4,1,0;40,10,2,0,0;40,10,2,0,0;40,10,2,0,0;30,8,2,0,0;20,4,2,0,0;20,4,2,0,0;20,4,2,0,0;20,4,2,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0#0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;40,4,0,0,0;20,4,0,0,0;10,4,0,0,0;10,4,0,0,0;8,2,0,0,0;6,2,0,0,0;6,2,0,0,0;6,2,0,0,0;6,2,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=4,8,3,11,4,10,9,3,4,12,5,8,11,10,5,11,7,3,11,10,12,12,12,8,9,12,5,6,11,1,10,11,8,10,11,4,1,6,8,4,5,8,11,12,12,5,8,11~7,3,8,12,9,4,9,2,11,9,3,7,3,9,6,5,7,12,8,12,12,12,7,6,1,6,9,10,9,5,4,2,7,3,4,12,6,4,6,2,2,2,2,2,2,2,9,7,10,7,10,9,3,2,4,6,9,12,11,3,6,11,2,7,2,2,7,1,2~1,4,6,2,5,3,7,9,12,5,12,9,6,7,11,7,4,12,12,12,9,8,4,3,2,8,5,8,9,3,2,5,9,7,4,7,2,2,10,2,2,2,2,2,2,2,2,6,12,6,7,8,3,4,7,9,2,9,2,4,6,9,8,11,12,1~2,3,6,5,8,7,6,10,2,1,10,12,12,12,12,2,11,6,8,12,10,5,10,5,11,8,12,1,12,2,2,2,2,2,2,6,9,4,3,8,9,4,10,9,11,2,7,6,4~1,8,5,4,8,4,6,3,5,8,11,12,2,12,11,7,2,12,12,12,7,10,1,5,9,6,10,7,3,4,9,10,5,2,2,2,2,7,10,11,4,11,3,12,8,3,9,6,7,6,4,8,7,9,7&reel_set2=1,5,4,12,10,11,12,6,4,8,11,7,11,8,4,5,11,8,12,12,12,8,5,11,12,10,8,1,10,11,3,5,9,7,3,5,4,10,3,9,6,11,10~10,12,7,6,7,2,2,4,8,5,4,12,2,6,9,10,7,12,12,12,9,7,11,4,9,7,1,12,3,9,6,12,7,1,3,6,2,2,2,2,2,2,2,6,11,3,4,3,6,5,9,7,2,10,9,2,2,8,3,6,9~7,3,6,5,7,4,12,5,2,2,11,4,9,1,4,7,1,9,6,12,12,12,10,9,8,7,9,7,2,4,7,12,7,3,7,4,6,9,12,6,7,2,2,2,2,2,2,2,2,9,11,5,3,8,2,8,9,5,12,12,6,2,10,6,2,9,6,3,8,5~8,1,6,10,7,9,5,8,6,2,11,4,12,12,12,12,9,5,4,6,5,3,1,12,4,12,2,10,2,2,2,2,2,2,12,6,8,11,8,3,5,2,7,2,6,8,11,10,9~8,10,6,11,5,4,9,12,2,4,6,11,7,6,9,3,5,7,11,7,3,4,9,10,7,5,4,9,2,2,2,2,7,8,11,8,5,4,7,2,6,5,12,4,8,3,7,10,1,2,12,3,10,7,10,5,10,8,7,1,3,8,11,6&reel_set1=8,9,9,7,4,11,10,9,11,8,9,5,10,6,5,11,3,11,8,3,10,10,3,5,4,10,11,7,9,6,8,9,6,5,8,7,8,3,6,11,6,8,8,10,5,11,4,10,10,6,4,7,5,5,11~10,6,4,11,6,2,8,9,10,9,5,10,11,11,11,2,7,2,7,12,9,9,11,2,4,7,2,2,12,12,12,11,11,6,2,3,1,2,11,12,12,6,2,5,1,1,1,10,7,12,5,3,4,7,11,6,9,4,2,7,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,10,6,4,8,5,2,7,5,5,2,11,2,4,3,3,3,10,7,10,10,6,9,9,7,10,9,12,2,9,4,4,4,9,3,2,2,6,10,7,12,10,1,8,11,5,10,5,5,5,12,10,8,2,1,8,6,11,8,10,7,6,3,6,6,6,12,7,4,5,10,7,6,10,9,7,12,7,6,7,7,7,10,6,2,11,9,9,3,11,9,9,11,8,8,8,2,12,5,3,9,8,5,7,1,2,5,8,6,9,9,9,3,3,9,10,10,1,1,7,8,12,6,4,3,10,10,10,9,10,2,4,4,9,8,10,7,4,6,6,11,11,6~6,10,4,9,9,8,6,11,6,10,6,4,8,11,11,11,8,6,3,4,12,3,2,9,10,12,9,9,10,7,3,12,12,12,10,7,7,10,11,12,2,7,10,11,6,12,4,6,9,1,1,1,4,6,7,1,7,2,11,11,9,12,7,11,3,11,7,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,3,10,12,2,9,10,4,2,8,2,2,5,2,11,3,3,3,4,7,12,5,7,6,7,10,11,6,10,11,12,7,1,4,4,4,5,2,6,9,9,6,1,12,11,2,2,11,7,2,2,5,5,5,10,6,9,2,2,10,4,2,5,12,10,8,11,5,3,6,6,6,12,6,10,2,6,1,9,7,7,4,2,10,8,6,7,7,7,9,5,2,5,8,12,3,7,5,7,4,5,7,5,6,8,8,8,11,10,3,2,10,7,6,9,9,6,8,8,5,7,2,9,9,9,10,4,10,9,9,7,5,4,2,9,2,10,9,9,1,10,10,10,8,11,11,3,10,10,1,9,6,7,9,10,3,2,8,8~7,11,6,9,6,11,4,7,11,11,11,8,4,10,7,9,2,8,12,8,5,12,12,12,2,6,10,6,12,4,2,6,3,1,1,1,7,8,11,10,7,1,9,3,11,4,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,9,6,7,5,8,6,8,6,2,3,3,3,9,5,6,10,9,10,7,2,9,3,4,4,4,10,2,2,10,9,10,5,7,9,5,5,5,9,1,11,10,4,7,6,10,2,2,6,6,6,10,6,10,9,8,3,4,9,2,7,7,7,8,2,12,6,5,2,7,12,10,5,8,8,8,5,12,9,1,1,2,11,2,11,9,9,9,3,5,7,11,3,12,11,4,4,3,10,10,10,12,12,7,7,6,11,2,7,9~5,11,9,8,7,10,10,4,5,6,4,10,9,7,6,4,10,6,5,6,11,6,11,9,10,3,7,9,11,5,2,2,2,2,7,11,2,5,8,11,4,9,6,7,7,8,8,7,8,3,8,10,9,11,4,5,11,2,9,8,10,6,7,11,5,2&reel_set4=9,10,7,6,8,8,5,7,6,11,5,8,11,6,10,5,8,9,10,7,10,8,9,9,11,10,7,8,4,11,10,5,11,3,11,8,6,4,8,4,9,6,5,11,3,5,10~2,2,12,7,3,12,3,2,1,11,11,11,12,9,10,3,10,10,7,12,7,2,9,12,12,12,8,11,10,9,1,7,8,6,12,10,12,1,1,1,10,11,2,2,5,3,12,9,2,9,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,7,10,4,3,7,2,3,8,7,4,3,3,3,9,7,10,5,12,5,6,4,8,11,12,4,4,4,11,6,2,6,12,1,2,12,10,2,3,5,5,5,12,9,4,11,11,10,5,8,9,9,7,6,6,6,1,5,2,7,10,3,9,5,10,11,7,7,7,2,8,7,4,10,12,2,6,7,6,2,8,8,8,9,5,4,12,4,12,5,12,6,9,9,9,11,9,10,11,7,12,6,11,11,6,10,10,10,12,6,10,6,9,8,6,12,4,7,2,2,9~8,10,12,9,3,5,3,12,4,8,11,11,11,10,12,8,1,2,12,2,4,7,1,5,12,12,12,8,6,2,11,10,7,10,3,9,7,2,1,1,1,6,7,2,6,3,12,2,6,7,12,4,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,9,12,9,9,6,10,2,2,12,7,9,3,3,3,5,12,4,10,7,6,11,6,11,12,12,4,4,4,9,6,9,10,11,8,2,5,4,10,11,2,5,5,5,7,3,5,7,1,12,11,6,2,11,6,6,6,11,6,12,12,7,3,2,10,2,4,10,7,7,7,12,8,8,1,12,2,9,11,10,2,4,8,8,8,6,2,9,9,11,7,9,2,1,12,9,9,9,7,4,5,4,9,12,11,2,7,7,10,10,10,12,8,3,3,10,11,6,10,10,9,7,5~2,8,2,12,9,7,10,7,2,11,11,11,5,2,12,2,2,11,9,2,10,8,9,11,2,12,12,12,1,12,3,6,8,4,11,2,9,7,12,10,1,1,1,2,6,10,2,10,5,12,5,12,6,11,2,1,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,9,3,7,2,11,8,12,8,6,12,6,1,9,3,3,3,2,11,10,10,3,7,12,5,4,2,5,9,4,4,4,6,2,1,6,11,10,6,6,9,9,3,7,9,5,5,5,11,9,2,2,7,10,7,8,7,7,5,6,6,6,1,7,6,4,10,9,6,8,11,10,12,10,2,7,7,7,12,12,4,12,4,9,7,12,7,9,12,12,9,8,8,8,5,12,4,5,12,10,3,8,11,7,12,10,9,9,9,12,8,4,7,12,9,4,11,8,11,6,3,7,10,10,10,3,6,9,10,5,10,3,4,12,3,4,7,2~4,7,11,8,11,5,8,7,2,10,5,11,6,4,11,6,7,5,4,7,11,9,5,2,6,2,10,10,2,2,2,2,8,9,10,4,7,5,8,3,6,11,11,9,7,10,5,5,10,9,8,9,9,7,3,6,4,6,9,8,6,11,10&purInit=[{bet:1000,type:\"default\"},{bet:1000,type:\"default\"}]&reel_set3=12,11,4,11,9,5,7,1,4,8,9,10,5,11,8,4,10,11,12,12,12,3,6,5,12,8,4,8,10,8,7,10,1,10,5,3,12,4,5,11,8,6,3~3,6,9,12,6,8,7,3,2,7,12,9,3,12,12,12,5,6,3,5,2,7,6,1,11,12,4,7,8,2,2,2,2,2,2,2,9,10,4,11,2,6,9,10,7,1,4,2,9~2,7,8,6,2,8,12,10,9,5,2,3,9,12,12,12,5,9,6,3,9,11,5,12,4,1,11,4,9,4,2,2,2,2,2,2,2,2,7,1,8,7,6,2,9,7,9,6,12,7,2,10~2,2,4,9,3,11,7,11,10,6,8,6,12,1,7,1,5,12,12,12,12,10,2,6,8,2,3,9,8,6,9,11,12,9,5,11,10,5,8,6,4,2,2,2,2,2,2,5,9,7,5,4,6,2,6,2,5,11,12,8,9,3,4,10,8,6,10,12~5,2,5,10,7,8,10,2,7,11,9,4,9,6,9,4,9,3,7,5,10,7,12,12,12,5,8,2,3,10,4,12,2,7,6,12,4,12,4,6,7,11,9,3,8,11,7,12,10,9,2,2,2,2,11,7,8,6,3,10,11,10,4,5,3,10,8,5,8,5,8,11,6,10,4,7,6,11,10&reel_set6=7,5,3,5,3,5,7,5,1,3,1,7,1,5,1,5,1,3,7,1,3,5,3,7,5,7,3,5,3,1,5,1,5,1,7,1,3,1,7,1,1,7,1~5,9,5,9,1,7,9,7,1,5,1,5,9,7,1,7,1,5,7,1,5,1,11,1,5,1,5,1,7,9,1,7,11,1,5,7,9,5,9,1,10,9,1,1,5~1,4,11,6,9,1,8,10,1,9,8,1,6,1,10,6,4,8,1,9,11,1,10,1,10,4,1,9,4~3,9,4,7,8,3,7,5,6,11,6,4,8,4,7,9,5,9,3,8,9,7,6,8,4,11,10,8,1,8,9,6,8,7,6,7,5,3,8,7,8,7,3,7,6,9,7,10,1,5,4,5,10,7,5,8,5,8,3,1,6,1,11,8,6,7,11,10,4,6,7,5,7,11,5,9,3,9,8,4,11,9,3,5,9,3,11,4,3,9,5,6,7,11,6,7,5,7,6,7,8,5,3,11,10,7,11,8,10,4,7,6,3,8,6,4,5,1~10,3,8,5,6,8,5,6,9,5,7,3,4,7,11,6,7,8,7,11,3,7,9,7,5,1,11,9,3,4,6,3,8,3,4,7,6,11,8,6,5,7,4,8,7,10,6,5,3,8,7,10,8,7,1,3,9,11,3,6,8,6,9,4,11,9,7,11,8,10,7,8,7,9,5,9,4,6,5,7,4,8,11,7,5,3,7,10,5,6,8,9,8,10,11,1,11,6,10,5,7,11,3,5,7,3,4&reel_set5=12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12~9,10,4,7,10,4,5,7,8,6,3,7,4,6,7,9,11,3,9,7,6,7,3,9,6,7,3,10,5,4,9,8,9,11,6,10~9,10,9,6,7,3,8,6,5,7,9,11,9,6,3,6,4,5,6,7,4,3,6,9,6,4,6,3,7,9,4,8,9,10,7,6,9,4,9,11,3,7,3,5,7,9,8,11,5,7,3,9,7,11,5,7,10,9,4~6,9,8,5,7,6,10,3,8,7,6,11,3,11,7,8,9,3,7,4,9,6,8,6,5,9,11,8,6,5,10,9,8,6,7,9,6,3,4,8,7,6,4,6,11,6,3,4,6,8,10,5,6,7,10,11,5,4~12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12,12&reel_set8=8,6,1,6,1,5,1,11,10,7,10,6,4,10,1,4,1,11,1,10,4,1,8,1,5,4,5,7,8,9,11,8,4,1,9,5,10,1,1,8~9,4,6,7,1,3,10,4,1,7,11,6,4,7,1,6,1,8,1,11,9,5,3,1,9,6,7,1,11,1,9,1~3,9,11,6,8,1,3,6,1,9,7,1,7,1,6,7,10,7,1,7,4,9,11,1,6,1,4,7,1,6,4,1,6,1,6,5,8,1,8,1,9,5,7,1,4,1,5,1,5,1,10,9~9,1,4,1,11,8,1,11,10,11,6,9,1,5,8,3,7,1,9,1,6,8,1,6,7,10,5,6,1,8,9,1,4,10,1,5,4,1,3~7,9,3,8,5,8,7,8,1,6,9,1,9,8,1,10,1,11,1,7,9,1,11,1,10,6,1,7,1,7,1,8,9,5,1,4,8,1,6,7,10,7,5,9,11,1,3,1,5,10,1,3,4,1,8,1,7,5,7,11,10,4,5,3,10,1,7,3,6,1,4,1&reel_set7=5,4,3,11,7,10,5,8,10,4,10,9,11,10,3,10,11,5,8,12,7,11,8,11,10,3,12,11,4,10,9,6,5,4,8,12,12,4,6,3,8,11,8,11,4,8,11,10,11,7,4,5,6~9,7,4,2,7,9,2,9,4,7,5,11,7,6,12,3,9,3,11,9,6,4,9,2,2,2,2,2,2,2,7,9,2,2,9,6,3,6,12,3,6,2,8,9,6,7,3,7,12,6,4,10,7,8~7,2,10,11,9,7,6,2,6,2,9,4,7,2,2,7,12,8,9,2,2,2,2,2,2,2,2,6,9,6,9,11,3,8,3,9,8,7,4,3,6,5,10,5,12,4,12,6~8,7,2,12,9,5,2,4,8,3,12,9,7,6,8,6,2,10,4,10,5,8,9,6,9,8,5,10,2,2,2,2,2,2,6,12,11,8,6,11,2,2,5,11,5,8,6,9,2,6,10,12,6,10,4,11,10,11,3,5,3,9,4,6,8~6,3,2,7,6,3,4,6,5,8,4,11,4,12,7,5,3,8,11,4,9,7,11,2,2,2,2,5,3,9,7,2,10,9,11,7,4,5,8,10,7,12,8,7,2,10,9,10,8,12,9,6,4&total_bet_min=200.00";
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
            get { return new double[] { 100, 100 }; }
        }
        #endregion

        public SleepingDragonGameLogic()
        {
            _gameID     = GAMEID.SleepingDragon;
            GameName    = "SleepingDragon";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"]   = "0";
	        dicParams["st"]         = "rect";
	        dicParams["sw"]         = "5";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);

            if (dicParams.ContainsKey("pw"))
                dicParams["pw"] = convertWinByBet(dicParams["pw"], currentBet);

            if (dicParams.ContainsKey("apwa"))
                dicParams["apwa"] = convertWinByBet(dicParams["apwa"], currentBet);
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                SleepingDragonBetInfo betInfo = new SleepingDragonBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();

                if (message.DataNum >= 3)
                {
                    betInfo.PurchaseFree = true;
                    betInfo.PurchaseType = (int)message.GetData(2);
                }
                else
                    betInfo.PurchaseFree = false;

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in SleepingDragonGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                    (oldBetInfo as SleepingDragonBetInfo).PurchaseType = betInfo.PurchaseType;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SleepingDragonGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            SleepingDragonBetInfo betInfo = new SleepingDragonBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new SleepingDragonBetInfo();
        }
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as SleepingDragonBetInfo).PurchaseType;
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
        protected override async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int agentID, BasePPSlotBetInfo betInfo, double baseBet, UserBonus userBonus)
        {
            int purchaseType = (betInfo as SleepingDragonBetInfo).PurchaseType;
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
                int purchaseType = (betInfo as SleepingDragonBetInfo).PurchaseType;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, -1, PurchaseMultiples[purchaseType] * 0.2, PurchaseMultiples[purchaseType] * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SleepingDragonGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int purchaseType = (betInfo as SleepingDragonBetInfo).PurchaseType;
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SleepingDragonGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            base.overrideSomeParams(betInfo, dicParams);
            
            if (!dicParams.ContainsKey("reel_set"))
                dicParams["reel_set"] = "5";

            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = (betInfo as SleepingDragonBetInfo).PurchaseType.ToString();
        }
        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            SleepingDragonBetInfo starBetInfo = betInfo as SleepingDragonBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, -1, betInfo.PurchaseFree ? starBetInfo.PurchaseType : -1, betMoney);
        }
    }
}
