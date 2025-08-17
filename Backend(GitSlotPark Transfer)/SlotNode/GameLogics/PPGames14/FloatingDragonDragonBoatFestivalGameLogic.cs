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
    class FloatingDragonDragonBoatFestivalBetInfo : BasePPSlotBetInfo
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
    class FloatingDragonDragonBoatFestivalGameLogic : BasePPSlotGame
    {
        protected double[]                          _multiTotalFreeSpinWinRates;
        protected double[]                          _multiMinFreeSpinWinRates;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10fdrasbf";
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
                return "def_s=4,8,5,3,12,5,10,8,5,8,6,8,12,3,12&cfgs=7788&ver=3&def_sb=6,10,3,6,10&reel_set_size=23&def_sa=4,12,5,3,10&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"2793296\",max_rnd_win:\"10000\",max_rnd_win_a:\"6666\",max_rnd_hr_a:\"1588058\"}}&wl_i=tbm~10000;tbm_a~6666&reel_set10=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,13,12,6,4,10,11,13,12,8,7,7,7,7,7~6,4,3,9,7,13,11,3,8,6,3,9,13,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,13,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,13,10,4,3,5,1,9,4,5,6,7,7,13,12,9,5,4,3,8,13,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,13,6,4,11,13,3,9,7,7,10,13,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,13,7,5,13,3,12,1,7,7,10,1,6,9,13,7,7,7,7,7&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&reel_set11=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,13,12,6,4,10,11,13,12,8,7,7,7,7,7~6,4,3,9,7,13,11,3,8,6,3,9,13,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,13,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,13,10,4,3,5,1,9,4,5,6,7,7,13,12,9,5,4,3,8,13,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,13,6,4,11,13,3,9,7,7,10,13,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,13,7,5,13,3,12,1,7,7,10,1,6,9,13,7,7,7,7,7&reel_set12=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,13,12,6,4,10,11,13,12,8,7,7,7,7,7~6,4,3,9,7,13,11,3,8,6,3,9,13,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,13,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,13,10,4,3,5,1,9,4,5,6,7,7,13,12,9,5,4,3,8,13,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,13,6,4,11,13,3,9,7,7,10,13,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,13,7,5,13,3,12,1,7,7,10,1,6,9,13,7,7,7,7,7&purInit_e=1,1&reel_set13=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,13,12,6,4,10,11,13,12,8,7,7,7,7,7~6,4,3,9,7,13,11,3,8,6,3,9,13,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,13,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,13,10,4,3,5,1,9,4,5,6,7,7,13,12,9,5,4,3,8,13,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,13,6,4,11,13,3,9,7,7,10,13,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,13,7,5,13,3,12,1,7,7,10,1,6,9,13,7,7,7,7,7&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&bls=10,15&reel_set18=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,13,12,6,4,10,11,13,12,8,7,7,7,7,7~6,4,3,9,7,13,11,3,8,6,3,9,13,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,13,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,13,10,4,3,5,1,9,4,5,6,7,7,13,12,9,5,4,3,8,13,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,13,6,4,11,13,3,9,7,7,10,13,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,13,7,5,13,3,12,1,7,7,10,1,6,9,13,7,7,7,7,7&reel_set19=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,13,12,6,4,10,11,13,12,8,7,7,7,7,7~6,4,3,9,7,13,11,3,8,6,3,9,13,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,13,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,13,10,4,3,5,1,9,4,5,6,7,7,13,12,9,5,4,3,8,13,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,13,6,4,11,13,3,9,7,7,10,13,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,13,7,5,13,3,12,1,7,7,10,1,6,9,13,7,7,7,7,7&reel_set14=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,13,12,6,4,10,11,13,12,8,7,7,7,7,7~6,4,3,9,7,13,11,3,8,6,3,9,13,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,13,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,13,10,4,3,5,1,9,4,5,6,7,7,13,12,9,5,4,3,8,13,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,13,6,4,11,13,3,9,7,7,10,13,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,13,7,5,13,3,12,1,7,7,10,1,6,9,13,7,7,7,7,7&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;2000,200,50,5,0;1000,150,30,0,0;500,100,20,0,0;500,100,20,0,0;200,50,10,0,0;100,25,2,0,0;100,25,2,0,0;50,10,2,0,0;50,10,2,0,0;50,10,2,0,0;0,0,0,0,0;0,0,0,0,0&reel_set15=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,13,12,6,4,10,11,13,12,8,7,7,7,7,7~6,4,3,9,7,13,11,3,8,6,3,9,13,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,13,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,13,10,4,3,5,1,9,4,5,6,7,7,13,12,9,5,4,3,8,13,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,13,6,4,11,13,3,9,7,7,10,13,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,13,7,5,13,3,12,1,7,7,10,1,6,9,13,7,7,7,7,7&reel_set16=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,13,12,6,4,10,11,13,12,8,7,7,7,7,7~6,4,3,9,7,13,11,3,8,6,3,9,13,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,13,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,13,10,4,3,5,1,9,4,5,6,7,7,13,12,9,5,4,3,8,13,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,13,6,4,11,13,3,9,7,7,10,13,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,13,7,5,13,3,12,1,7,7,10,1,6,9,13,7,7,7,7,7&reel_set17=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,13,12,6,4,10,11,13,12,8,7,7,7,7,7~6,4,3,9,7,13,11,3,8,6,3,9,13,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,13,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,13,10,4,3,5,1,9,4,5,6,7,7,13,12,9,5,4,3,8,13,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,13,6,4,11,13,3,9,7,7,10,13,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,13,7,5,13,3,12,1,7,7,10,1,6,9,13,7,7,7,7,7&total_bet_max=10,000,000.00&reel_set21=12,6,7,8,10,7,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,7,12,10,9,4,3,10,5,8,12,3,6,8,12,6,4,10,11,12,8,7,7,7,7,7~6,4,3,9,7,11,3,8,6,3,9,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,7,9,5,11,9,6,11,4,9,3,5,4,11,6,3,5,11,7,12,9,10,4,3,11,5,7,9,8,7,7,7,7,7~6,12,7,3,6,7,11,3,5,7,10,4,3,5,7,9,4,5,6,7,7,12,9,5,4,3,8,5,6,11,4,6,7,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,7,12,5,7,8,7,6,4,11,3,9,7,7,10,6,3,5,7,7,7,7,7,7~5,4,6,7,11,7,4,8,7,7,5,3,12,7,7,10,7,6,9,7,7,7,7,7&reel_set22=6,8,6,8,7,5,11,9,7,7,10,7,4,7,6,12,7,4,9,7,11,12,7,7,7,7,7,7,9,10,5,12,3,7,10,7,6,8,12,8,4,3,9,10,6,7,5,11,4,11,8~9,6,12,7,7,3,5,4,7,8,10,6,10,4,12,11,12,7,7,7,7,7,7,6,10,11,8,7,3,12,5,8,6,4,7,7,11,9,5,8,7,10~10,6,7,3,8,10,5,6,12,10,9,7,8,9,12,7,7,7,7,7,7,8,4,9,5,4,6,7,7,10,7,11,12,7,11,7,8~10,6,9,7,4,6,7,5,9,11,8,9,4,7,5,12,7,10,7,7,8,9,10,7,7,7,7,7,7,3,7,7,5,6,4,6,8,11,4,12,11,6,9,8,6,8,11,7,12,7,3,12,10~8,11,6,10,11,12,9,6,4,7,9,7,5,11,7,7,7,7,7,7,9,8,6,7,12,7,10,12,8,7,7,10,3,4,5&reel_set0=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,13,12,6,4,10,11,13,12,8,7,7,7,7,7~6,4,3,9,7,13,11,3,8,6,3,9,13,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,13,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,13,10,4,3,5,1,9,4,5,6,7,7,13,12,9,5,4,3,8,13,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,13,6,4,11,13,3,9,7,7,10,13,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,13,7,5,13,3,12,1,7,7,10,1,6,9,13,7,7,7,7,7&accInit=[{id:0,mask:\"cp;tp;lvl;sc;cl\"}]&reel_set2=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,13,12,6,4,10,11,13,12,8,7,7,7,7,7~6,4,3,9,7,13,11,3,8,6,3,9,13,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,13,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,13,10,4,3,5,1,9,4,5,6,7,7,13,12,9,5,4,3,8,13,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,13,6,4,11,13,3,9,7,7,10,13,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,13,7,5,13,3,12,1,7,7,10,1,6,9,13,7,7,7,7,7&reel_set1=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,13,12,6,4,10,11,13,12,8,7,7,7,7,7~6,4,3,9,7,13,11,3,8,6,3,9,13,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,13,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,13,10,4,3,5,1,9,4,5,6,7,7,13,12,9,5,4,3,8,13,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,13,6,4,11,13,3,9,7,7,10,13,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,13,7,5,13,3,12,1,7,7,10,1,6,9,13,7,7,7,7,7&reel_set4=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,13,12,6,4,10,11,13,12,8,7,7,7,7,7~6,4,3,9,7,13,11,3,8,6,3,9,13,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,13,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,13,10,4,3,5,1,9,4,5,6,7,7,13,12,9,5,4,3,8,13,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,13,6,4,11,13,3,9,7,7,10,13,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,13,7,5,13,3,12,1,7,7,10,1,6,9,13,7,7,7,7,7&purInit=[{bet:1000,type:\"fs\"},{bet:1000}]&reel_set3=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,13,12,6,4,10,11,13,12,8,7,7,7,7,7~6,4,3,9,7,13,11,3,8,6,3,9,13,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,13,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,13,10,4,3,5,1,9,4,5,6,7,7,13,12,9,5,4,3,8,13,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,13,6,4,11,13,3,9,7,7,10,13,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,13,7,5,13,3,12,1,7,7,10,1,6,9,13,7,7,7,7,7&reel_set20=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,13,12,6,4,10,11,13,12,8,7,7,7,7,7~6,4,3,9,7,13,11,3,8,6,3,9,13,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,13,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,13,10,4,3,5,1,9,4,5,6,7,7,13,12,9,5,4,3,8,13,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,13,6,4,11,13,3,9,7,7,10,13,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,13,7,5,13,3,12,1,7,7,10,1,6,9,13,7,7,7,7,7&reel_set6=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,13,12,6,4,10,11,13,12,8,7,7,7,7,7~6,4,3,9,7,13,11,3,8,6,3,9,13,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,13,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,13,10,4,3,5,1,9,4,5,6,7,7,13,12,9,5,4,3,8,13,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,13,6,4,11,13,3,9,7,7,10,13,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,13,7,5,13,3,12,1,7,7,10,1,6,9,13,7,7,7,7,7&reel_set5=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,13,12,6,4,10,11,13,12,8,7,7,7,7,7~6,4,3,9,7,13,11,3,8,6,3,9,13,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,13,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,13,10,4,3,5,1,9,4,5,6,7,7,13,12,9,5,4,3,8,13,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,13,6,4,11,13,3,9,7,7,10,13,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,13,7,5,13,3,12,1,7,7,10,1,6,9,13,7,7,7,7,7&reel_set8=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,13,12,6,4,10,11,13,12,8,7,7,7,7,7~6,4,3,9,7,13,11,3,8,6,3,9,13,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,13,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,13,10,4,3,5,1,9,4,5,6,7,7,13,12,9,5,4,3,8,13,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,13,6,4,11,13,3,9,7,7,10,13,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,13,7,5,13,3,12,1,7,7,10,1,6,9,13,7,7,7,7,7&reel_set7=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,13,12,6,4,10,11,13,12,8,7,7,7,7,7~6,4,3,9,7,13,11,3,8,6,3,9,13,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,13,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,13,10,4,3,5,1,9,4,5,6,7,7,13,12,9,5,4,3,8,13,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,13,6,4,11,13,3,9,7,7,10,13,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,13,7,5,13,3,12,1,7,7,10,1,6,9,13,7,7,7,7,7&reel_set9=12,6,7,8,10,1,5,6,12,7,11,6,5,10,6,5,8,7,7,12,4,10,5,8,4,10,3,12,8,10,4,8,9,1,12,10,9,4,3,10,5,8,12,3,6,8,13,12,6,4,10,11,13,12,8,7,7,7,7,7~6,4,3,9,7,13,11,3,8,6,3,9,13,7,11,5,10,6,3,9,7,7,12,3,4,11,3,6,1,9,5,11,9,6,13,11,4,9,3,5,4,11,6,3,5,11,1,12,9,10,4,3,11,5,1,9,8,7,7,7,7,7~6,12,1,3,6,7,11,3,5,7,13,10,4,3,5,1,9,4,5,6,7,7,13,12,9,5,4,3,8,13,5,6,11,4,6,1,10,3,6,4,3,8,7,7,7,7,7~4,5,7,6,1,12,5,1,8,7,13,6,4,11,13,3,9,7,7,10,13,6,3,5,1,7,7,7,7,7~5,4,6,7,11,1,4,8,13,7,5,13,3,12,1,7,7,10,1,6,9,13,7,7,7,7,7&total_bet_min=20.00";
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
            get { return new double[] { 100, 100 }; }
        }
        #endregion
        public FloatingDragonDragonBoatFestivalGameLogic()
        {
            _gameID = GAMEID.FloatingDragonDragonBoatFestival;
            GameName = "FloatingDragonDragonBoatFestival";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
	        dicParams["bl"] = "0";
        }
        protected override void supplementInitResult(Dictionary<string, string> dicParams, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {
            base.supplementInitResult(dicParams, betInfo, spinResult);
            if (!dicParams.ContainsKey("reel_set"))
                dicParams["reel_set"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("mo_tw"))
                dicParams["mo_tw"] = convertWinByBet(dicParams["mo_tw"], currentBet);

            if (dicParams.ContainsKey("rs_iw"))
                dicParams["rs_iw"] = convertWinByBet(dicParams["rs_iw"], currentBet);

            if (dicParams.ContainsKey("rs_win"))
                dicParams["rs_win"] = convertWinByBet(dicParams["rs_win"], currentBet);
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                FloatingDragonDragonBoatFestivalBetInfo betInfo = new FloatingDragonDragonBoatFestivalBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount  = (int)message.Pop();
                int bl             = (int)message.Pop();
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in FloatingDragonDragonBoatFestivalGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }

                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1}", strGlobalUserID, betInfo.LineCount);
                    return;
                }

                BasePPSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strGlobalUserID, out oldBetInfo))
                {
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine = betInfo.BetPerLine;
                    oldBetInfo.LineCount = betInfo.LineCount;
                    oldBetInfo.MoreBet = betInfo.MoreBet;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                    (oldBetInfo as FloatingDragonDragonBoatFestivalBetInfo).PurchaseType = betInfo.PurchaseType;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in FloatingDragonDragonBoatFestivalGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            FloatingDragonDragonBoatFestivalBetInfo betInfo = new FloatingDragonDragonBoatFestivalBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new FloatingDragonDragonBoatFestivalBetInfo();
        }
        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBet = (double)infoDocument["defaultbet"];
                _normalMaxID = (int)infoDocument["normalmaxid"];
                _emptySpinCount = (int)infoDocument["emptycount"];
                _naturalSpinCount = (int)infoDocument["normalselectcount"];

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
            int purchaseType = (betInfo as FloatingDragonDragonBoatFestivalBetInfo).PurchaseType;
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
                int purchaseType = (betInfo as FloatingDragonDragonBoatFestivalBetInfo).PurchaseType;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, -1, PurchaseMultiples[purchaseType] * 0.2, PurchaseMultiples[purchaseType] * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in FloatingDragonDragonBoatFestivalGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int purchaseType = (betInfo as FloatingDragonDragonBoatFestivalBetInfo).PurchaseType;
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in FloatingDragonDragonBoatFestivalGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            return this.PurchaseMultiples[(betInfo as FloatingDragonDragonBoatFestivalBetInfo).PurchaseType];
        }
        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = (betInfo as FloatingDragonDragonBoatFestivalBetInfo).PurchaseType.ToString();
        }
        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            FloatingDragonDragonBoatFestivalBetInfo floatingBetInfo = betInfo as FloatingDragonDragonBoatFestivalBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, -1, betInfo.PurchaseFree ? floatingBetInfo.PurchaseType : -1, betMoney);
        }
    }
}
