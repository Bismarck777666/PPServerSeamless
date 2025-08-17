using Akka.Actor;
using GITProtocol;
using GITProtocol.Utils;
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
    class SantasXmasRushBetInfo : BasePPSlotBetInfo
    {
        public int MorebetType { get; set; }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            this.MorebetType = reader.ReadInt32();
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(MorebetType);
        }
    }
    class SantasXmasRushGameLogic : BasePPSlotGame
    {
        protected double[]      _spinDataDefaultBets    = null;
        protected int []        _normalMaxIDs           = null;
        protected int []        _naturalSpinCounts      = null;
        protected int []        _emptySpinCounts        = null;
        protected int []        _startIDs               = null;
        protected int BetTypeCount => 3;   //앤티베트타압카운터야 - 여기서는

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20rainbowrsh";
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
                return "def_s=4,6,8,9,4,9,4,6,7,9,5,9,3,9,7,4,5,6,3,9,3,4,3,6,5,7,3,6,3,9,5,7,8,6,7,9&cfgs=1&ver=3&def_sb=3,6,8,7,7,8&reel_set_size=6&def_sa=6,4,8,5,4,5&scatters=1~0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0~0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0~1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1&rt=d&gameInfo={rtps:{ante_a2:\"96.52\",ante_a1:\"96.50\",purchase:\"96.55\",regular:\"96.51\"},props:{max_rnd_win_a1:\"6667\",max_rnd_hr_a1:\"25125628\",max_rnd_win_a2:\"3334\",max_rnd_sim:\"1\",max_rnd_hr_a2:\"11173184\",prizes:\"v:10,v:20,v:40,v:60,v:100,v:200,v:300,v:400,v:600,v:1000,v:2000,v:4000,v:50000,rs:1\",max_rnd_hr:\"50000000\",max_rnd_win:\"10000\"}}&wl_i=tbm~10000;tbm_a1~6667;tbm_a2~3334&sc=5.00,10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1&wilds=2~0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0~1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1&bonuses=0&bls=20,30,60&ntp=0.00&paytable=0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,200,200,80,80,40,40,0,0,0,0,0;400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,150,150,60,60,30,30,0,0,0,0,0;250,250,250,250,250,250,250,250,250,250,250,250,250,250,250,250,250,250,250,250,250,250,250,250,250,100,100,40,40,20,20,0,0,0,0,0;150,150,150,150,150,150,150,150,150,150,150,150,150,150,150,150,150,150,150,150,150,150,150,150,150,60,60,30,30,10,10,0,0,0,0,0;120,120,120,120,120,120,120,120,120,120,120,120,120,120,120,120,120,120,120,120,120,120,120,120,120,50,50,20,20,8,8,0,0,0,0,0;80,80,80,80,80,80,80,80,80,80,80,80,80,80,80,80,80,80,80,80,80,80,80,80,80,30,30,15,15,6,6,0,0,0,0,0;50,50,50,50,50,50,50,50,50,50,50,50,50,50,50,50,50,50,50,50,50,50,50,50,50,20,20,10,10,5,5,0,0,0,0,0;0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0&total_bet_max=12,000,000.00&reel_set0=6,6,8,8,6,6,5,5,6,6,8,8,4,4,7,7,8,8,5,5,8,8,9,9,7,7,9,9,5,5,8,8,6,6,5,5,6,6,5,5,8,8,6,6,8,8,6,6,8,8,6,6,8,8,3,3,6,6,1,8,8,6,6,8,8,6,6,8,8,6,6,1,8,8,6,6,3,3,4,4,6,6,8,8,3,3,5,5,6,6,8,8,6,6,8,8,5,5,6,6,3,3,6,6,3,3,6,6,5,5,6,6,8,8~9,9,5,5,7,7,9,9,1,7,7,4,4,8,8,7,7,9,9,6,6,5,5,9,9,7,7,9,9,7,7,9,9,7,7,5,5,4,4,9,9,7,7,9,9,7,7,4,4,9,9,7,7,3,3,9,9,4,4,5,5,7,7,9,9,4,4,7,7,6,6,4,4,9,9,7,7,6,6,7,7,9,9,3,3,9,9,7,7,5,5,7,7~8,8,3,3,6,6,8,8,3,3,6,6,8,8,6,6,7,7,6,6,8,8,6,6,4,4,7,7,8,8,6,6,8,8,6,6,8,8,6,6,8,8,5,5,8,8,6,6,8,8,1,6,6,3,3,8,8,5,5,8,8,6,6,9,9,6,6,5,5,3,3,8,8,6,6,5,5,4,4,6,6,5,5,1,6,6,8,8,6,6,3,3,8,8,6,6,5,5,8,8,6,6,8,8,3,3,8,8,6,6,3,3,9,9,5,5,8,8,6,6~7,7,4,4,7,7,6,6,3,3,4,4,7,7,5,5,7,7,1,7,7,4,4,9,9,7,7,4,4,7,7,5,5,4,4,9,9,4,4,7,7,9,9,5,5,9,9,7,7,9,9,4,4,6,6,9,9,5,5,9,9,7,7,9,9,5,5,9,9,7,7,3,3,4,4,7,7,5,5,7,7,5,5,9,9,8,8,9,9,7,7,9,9,1,4,4,9,9,4,4,9,9,7,7,5,5,6,6,8,8,9,9~3,3,8,8,6,6,8,8,6,6,3,3,6,6,8,8,3,3,8,8,6,6,8,8,6,6,5,5,8,8,5,5,1,6,6,8,8,6,6,3,3,5,5,3,3,6,6,5,5,6,6,7,7,8,8,6,6,1,8,8,6,6,3,3,1,6,6,7,7,8,8,6,6,3,3,6,6,8,8,4,4,6,6,8,8,3,3,6,6,5,5,8,8,3,3,6,6,9,9,6,6,8,8,6,6,3,3,8,8,5,5,4,4,5,5,8,8,6,6~4,4,9,9,7,7,9,9,1,5,5,9,9,8,8,7,7,9,9,7,7,9,9,6,6,9,9,8,8,5,5,4,4,7,7,9,9,7,7,4,4,9,9,7,7,9,9,7,7,4,4,5,5,3,3,9,9,4,4,9,9,5,5,7,7,6,6,9,9,7,7,4,4,1,9,9,7,7&reel_set2=8,8,6,6,5,5,6,6,5,5,10,8,8,4,4,8,8,6,6,3,3,6,6,8,8,5,5,8,8,6,6,8,8,9,9,8,8,6,6,7,7,6,6,8,8,6,6,5,5,8,8,6,6,3,3,6,6,8,8,6,6,8,8,6,6,8,8,9,9,8,8,6,6,7,7,3,3,9,9,5,5,8,8,6,6,7,7,6,6,3,3,6,6,7,7,3,3,8,8,6,6,3,3,8,8,3,3,6,6,7,7,8,8,5,5,4,4,6,6,5,5,6,6,8,8,6,6,5,5,7,7,8,8,6,6,9,9,8,8,5,5,6,6,8,8,3,3,9,9,6,6,8,8,9,9,8,8,3,3,8,8,6,6,5,5~6,6,7,7,9,9,4,4,5,5,7,7,5,5,7,7,4,4,7,7,8,8,7,7,9,9,7,7,8,8,9,9,7,7,9,9,7,7,9,9,4,4,7,7,9,9,7,7,5,5,9,9,10,4,4,6,6,4,4,7,7,9,9,4,4,8,8,9,9,8,8,9,9,7,7,9,9,7,7,10,9,9,6,6,5,5,7,7,4,4,7,7,5,5,3,3,5,5,9,9,4,4,9,9,3,3,9,9,7,7,4,4,9,9,6,6,7,7,9,9,8,8,9,9~6,6,3,3,4,4,8,8,7,7,8,8,5,5,8,8,6,6,7,7,8,8,6,6,7,7,9,9,6,6,8,8,6,6,9,9,5,5,8,8,9,9,8,8,3,3,6,6,3,3,8,8,4,4,6,6,8,8,5,5,8,8,6,6,9,9,8,8,6,6,3,3,8,8,5,5,6,6,8,8,3,3,8,8,3,3,8,8,6,6,8,8,6,6,5,5,8,8,3,3,6,6,3,3,5,5,7,7,8,8,6,6,8,8,10,8,8,6,6,8,8,6,6,5,5,8,8,3,3,6,6,9,9,5,5~6,6,9,9,6,6,9,9,4,4,7,7,6,6,8,8,9,9,4,4,3,3,7,7,4,4,9,9,5,5,4,4,9,9,7,7,4,4,9,9,5,5,7,7,9,9,7,7,4,4,10,9,9,7,7,9,9,7,7,9,9,7,7,6,6,7,7,3,3,7,7,9,9,10,9,9,5,5,8,8,5,5,7,7,8,8,4,4,7,7,9,9,5,5,7,7,8,8,9,9,7,7,9,9,7,7,5,5,7,7,9,9,7,7,4,4,9,9,4,4,7,7,5,5,9,9,5,5,4,4,8,8,4,4~8,8,6,6,5,5,6,6,8,8,3,3,10,7,7,8,8,5,5,8,8,6,6,8,8,7,7,3,3,7,7,8,8,5,5,6,6,8,8,5,5,6,6,3,3,8,8,4,4,9,9,8,8,6,6,8,8,6,6,8,8,7,7,8,8,3,3,5,5,3,3,6,6,5,5,3,3,6,6,7,7,9,9,8,8,6,6,8,8,6,6,4,4,3,3,8,8,6,6,9,9,6,6,5,5,8,8,6,6,3,3,9,9,6,6,8,8,6,6,8,8,9,9,8,8,5,5,6,6,8,8,6,6,8,8,5,5,6,6,3,3,10~4,4,7,7,9,9,7,7,9,9,3,3,7,7,4,4,7,7,9,9,7,7,8,8,9,9,5,5,9,9,7,7,8,8,5,5,4,4,7,7,6,6,4,4,9,9,10,5,5,9,9,7,7,9,9,6,6,9,9,7,7,8,8,7,7,9,9,4,4,9,9,4,4,5,5,7,7,6,6,7,7,9,9,6,6,5,5,4,4,9,9,3,3,7,7,9,9,7,7,9,9,7,7,9,9,8,8,4,4,6,6&reel_set1=7,7,3,3,8,8,7,7,9,9,8,8,7,7,4,4,7,7,6,6,5,5,9,9,8,8,9,9,8,8,9,9,7,7,3,3,7,7,8,8,9,9,7,7,8,8,5,5,8,8,5,5,7,7,6,6,3,3,8,8,3,3,5,5,8,8,7,7,8,8,7,7,4,4,9,9,6,6,9,9,7,7,8,8,7,7,8,8,3,3,8,8,7,7,8,8,9,9,7,7,8,8,6,6,7,7,9,9,7,7,8,8,7,7,9,9,3,3,6,6,3,3,7,7,9,9,7,7,6,6,5,5,7,7,9,9,5,5,8,8,7,7,4,4,5,5,9,9,7,7,6,6,8,8,9,9,8,8,9,9,7,7,3,3,8,8,5,5,8,8,9,9,6,6,3,3,6,6,7,7,4,4,8,8,4,4,8,8,7,7,4,4,8,8,7,7,8,8,9,9,5,5,6,6,9,9,7,7,9,9,8,8,9,9,8,8,5,5,9,9,7,7,6,6,9,9,8,8,9,9,4,4,5,5,4,4,3,3,9,9,8,8,4,4,5,5,8,8,9,9,7,7,9,9,5,5,3,3,9,9,8,8,7,7,9,9,4,4,7,7,8,8,3,3,8,8,7,7,9,9,8,8,3,3,7,7,8,8,9,9,4,4,6,6,9,9,4,4,3,3,4,4,7,7,5,5,6,6,3,3,8,8,9,9,3,3,6,6,9,9,8,8,9,9,8,8,3,3,4,4,9,9,7,7,9,9,8,8,5,5~9,9,7,7,8,8,6,6,9,9,5,5,8,8,4,4,9,9,3,3,7,7,8,8,4,4,7,7,9,9,8,8,4,4,8,8,4,4,6,6,7,7,8,8,7,7,4,4,9,9,7,7,3,3,5,5,8,8,9,9,8,8,9,9,4,4,5,5,3,3,7,7,8,8,5,5,7,7,9,9,7,7,9,9,8,8,7,7,9,9,7,7,9,9,8,8,3,3,7,7,9,9,5,5,3,3,8,8,6,6,5,5,8,8,5,5,7,7,9,9,6,6,8,8,9,9,3,3,7,7,8,8,7,7,9,9,8,8,7,7,3,3,9,9,8,8,4,4,9,9,8,8,7,7,9,9,6,6,5,5,4,4,9,9,8,8,9,9,8,8,9,9,6,6,3,3,9,9,7,7,6,6,8,8,9,9,6,6~9,9,5,5,7,7,9,9,7,7,4,4,7,7,8,8,3,3,4,4,5,5,9,9,8,8,3,3,9,9,5,5,7,7,9,9,7,7,3,3,9,9,5,5,8,8,7,7,5,5,8,8,9,9,3,3,9,9,7,7,4,4,8,8,6,6,8,8,9,9,8,8,6,6,9,9,7,7,8,8,3,3,8,8,6,6,7,7,8,8,6,6,7,7,9,9,8,8,9,9,8,8,9,9,7,7,8,8,9,9,8,8,6,6,7,7,8,8,9,9,7,7,8,8,9,9,7,7,3,3,8,8,7,7,8,8,4,4,5,5,8,8,7,7,8,8,5,5,8,8,7,7,6,6,7,7,8,8,7,7,4,4,6,6,9,9,4,4,7,7,8,8,9,9,7,7,6,6,7,7,4,4,3,3,9,9,6,6,7,7,9,9,7,7,9,9,8,8,4,4,3,3,6,6,8,8,7,7,9,9,8,8,7,7,8,8,5,5,8,8,4,4,8,8,9,9,7,7,8,8,4,4,8,8,6,6,9,9,6,6,7,7,6,6,9,9,7,7,8,8~7,7,9,9,8,8,4,4,6,6,4,4,5,5,4,4,5,5,9,9,4,4,8,8,7,7,8,8,7,7,8,8,6,6,7,7,9,9,3,3,8,8,7,7,5,5,9,9,8,8,5,5,9,9,7,7,9,9,3,3,6,6,8,8,9,9,7,7,9,9,7,7,9,9,7,7,8,8,7,7,8,8,7,7,8,8,9,9,7,7,6,6,7,7,9,9,5,5,7,7,8,8,9,9,3,3,7,7,9,9,8,8,5,5,8,8,7,7,3,3,9,9,8,8,7,7,3,3,9,9,8,8,5,5,8,8,4,4,3,3,9,9,8,8,7,7,8,8,5,5,9,9,8,8,6,6,7,7,4,4,6,6,7,7,9,9,8,8,7,7,6,6,9,9,5,5,8,8,9,9,6,6,8,8,7,7,3,3,9,9,7,7,9,9,3,3,7,7,9,9~8,8,7,7,8,8,9,9,8,8,7,7,9,9,6,6,9,9,8,8,9,9,6,6,9,9,8,8,6,6,7,7,5,5,7,7,5,5,8,8,9,9,5,5,4,4,8,8,7,7,9,9,6,6,9,9,7,7,6,6,8,8,7,7,9,9,7,7,6,6,7,7,3,3,4,4,9,9,6,6,3,3,8,8,7,7,9,9,8,8,4,4,8,8,9,9,7,7,9,9,8,8,9,9,5,5,3,3,9,9,5,5,9,9,7,7,8,8,3,3,8,8,5,5,8,8,7,7,9,9,8,8,6,6,8,8,9,9,8,8,9,9,7,7,4,4,9,9,7,7,9,9,8,8,7,7,4,4,8,8,3,3,9,9,8,8,7,7,3,3,7,7,3,3,8,8,5,5,7,7,3,3,8,8,3,3,6,6,9,9,4,4,9,9~7,7,9,9,4,4,3,3,7,7,9,9,8,8,7,7,4,4,9,9,7,7,5,5,7,7,3,3,8,8,3,3,7,7,8,8,4,4,8,8,9,9,8,8,7,7,8,8,4,4,7,7,8,8,5,5,9,9,7,7,8,8,9,9,7,7,6,6,9,9,7,7,8,8,6,6,4,4,7,7,3,3,9,9,8,8,7,7,3,3,7,7,5,5,4,4,6,6,7,7,8,8,7,7,9,9,7,7,3,3,5,5,9,9,7,7,9,9,4,4,9,9,5,5,7,7,8,8,6,6,9,9,7,7,5,5,7,7,5,5,8,8,7,7,6,6,8,8,5,5,3,3,4,4,9,9,7,7,9,9,8,8,9,9,5,5,9,9,8,8,9,9,8,8,3,3,4,4,9,9,8,8,4,4,7,7,9,9,8,8,7,7,8,8,9,9,5,5,9,9,7,7,4,4,5,5,9,9,8,8,7,7,9,9,7,7,9,9,7,7,9,9,5,5,8,8,6,6,7,7,8,8,7,7,3,3,9,9,7,7,3,3,9,9,8,8,3,3,7,7,4,4,8,8,9,9,8,8,9,9,8,8,7,7,9,9,6,6,9,9,5,5,8,8,9,9,5,5,8,8,3,3,7,7,8,8,9,9,7,7,6,6,8,8,4,4,8,8,5,5,4,4,3,3,4,4&reel_set4=6,6,8,8,9,9,6,6,1,6,6,3,3,8,8,6,6,8,8,6,6,8,8,6,6,4,4,8,8,3,3,6,6,8,8,3,3,5,5,6,6,4,4,6,6,9,9,8,8,7,7,8,8,6,6,5,5,8,8,3,3,6,6,8,8,3,3,8,8,6,6,8,8,5,5,8,8,6,6,8,8,6,6,8,8,5,5,6,6,8,8,5,5,8,8,1,5,5,7,7,1,8,8,4,4~7,7,4,4,9,9,7,7,9,9,7,7,5,5,7,7,9,9,4,4,9,9,4,4,7,7,6,6,9,9,7,7,9,9,1,6,6,7,7,6,6,8,8,7,7,1,7,7,8,8,7,7,4,4,9,9,5,5,9,9,7,7,9,9,7,7,9,9,3,3,9,9,7,7,9,9,4,4,9,9,5,5,4,4,9,9,3,3,7,7,9,9,4,4,5,5,7,7,9,9,7,7,5,5,4,4,1,7,7,4,4,9,9~8,8,6,6,3,3,8,8,6,6,8,8,6,6,8,8,5,5,8,8,3,3,7,7,6,6,3,3,6,6,8,8,6,6,8,8,6,6,4,4,6,6,5,5,8,8,6,6,8,8,6,6,1,7,7,5,5,8,8,5,5,6,6,8,8,6,6,5,5,8,8,1,6,6,8,8,6,6,8,8,4,4,6,6,3,3,6,6,1,3,3,8,8,5,5,6,6,3,3,8,8,9,9,1,5,5,8,8,9,9,8,8,6,6,5,5,8,8,6,6,3,3~9,9,7,7,4,4,9,9,3,3,7,7,9,9,5,5,9,9,7,7,9,9,4,4,9,9,5,5,7,7,9,9,4,4,7,7,9,9,6,6,9,9,4,4,9,9,7,7,5,5,1,7,7,9,9,7,7,5,5,9,9,5,5,9,9,4,4,7,7,8,8,3,3,4,4,7,7,4,4,7,7,5,5,7,7,4,4,9,9,5,5,6,6,9,9,1,7,7,4,4,9,9,7,7,9,9,7,7,9,9,7,7,5,5,9,9,6,6,9,9,8,8,4,4,5,5,7,7,1,6,6~8,8,6,6,8,8,5,5,6,6,4,4,8,8,6,6,4,4,3,3,8,8,1,7,7,3,3,6,6,8,8,3,3,5,5,8,8,6,6,8,8,9,9,5,5,3,3,5,5,6,6,5,5,6,6,8,8,6,6,8,8,6,6,8,8,6,6,8,8,6,6,3,3,1,6,6,3,3,8,8,6,6,8,8,3,3,6,6,8,8,9,9,8,8,1,3,3,6,6,8,8,5,5,6,6,7,7,3,3,8,8,6,6~9,9,7,7,9,9,7,7,1,9,9,7,7,4,4,9,9,4,4,9,9,7,7,4,4,7,7,4,4,9,9,7,7,9,9,7,7,4,4,7,7,4,4,9,9,7,7,1,9,9,7,7,9,9,7,7,4,4,7,7,9,9,7,7,9,9,7,7,4,4,9,9,5,5,9,9,5,5,7,7,6,6,7,7,5,5,7,7,9,9,5,5,7,7,5,5,3,3,9,9,4,4,8,8,9,9,7,7,1,4,4,5,5,7,7,9,9,7,7,5,5,9,9,7,7,4,4,1,9,9,8,8,7,7,3,3,1,9,9,5,5,9,9,7,7,4,4,7,7,4,4,9,9,4,4,9,9,4,4,9,9,1,6,6,5,5,9,9,7,7&purInit=[{bet:2400,type:\"default\"}]&reel_set3=3,3,9,9,6,6,8,8,5,5,3,3,8,8,5,5,8,8,6,6,3,3,8,8,6,6,4,4,7,7,6,6,8,8,6,6,8,8,6,6,5,5,8,8,5,5,1,6,6,8,8,5,5,6,6,8,8,1,8,8,7,7,8,8,3,3,6,6,8,8,5,5,8,8,6,6,1,8,8,6,6,8,8,6,6,9,9,8,8,6,6,4,4,6,6,8,8,6,6,8,8,6,6,8,8,5,5,6,6,8,8,6,6,5,5,3,3,6,6~9,9,4,4,9,9,4,4,9,9,7,7,6,6,9,9,7,7,9,9,7,7,9,9,7,7,9,9,5,5,9,9,1,6,6,5,5,9,9,6,6,4,4,5,5,9,9,7,7,9,9,7,7,8,8,3,3,9,9,4,4,7,7,9,9,7,7,9,9,7,7,5,5,7,7,3,3,6,6,8,8,9,9,7,7,4,4,1~8,8,6,6,7,7,6,6,8,8,5,5,6,6,8,8,6,6,8,8,6,6,8,8,6,6,8,8,9,9,8,8,6,6,8,8,6,6,8,8,6,6,5,5,6,6,5,5,8,8,6,6,1,8,8,3,3,6,6,8,8,3,3,1,5,5,8,8,6,6,8,8,6,6,8,8,3,3,8,8,6,6,8,8,6,6,8,8,5,5,6,6,5,5,8,8,1,3,3,6,6,8,8,3,3,6,6,8,8,5,5,8,8,9,9,8,8,3,3,6,6,8,8,6,6,7,7,4,4,5,5,3,3,6,6,3,3,8,8,5,5,6,6,3,3,8,8,5,5,6,6,4,4,6,6~7,7,9,9,5,5,7,7,5,5,4,4,3,3,9,9,6,6,9,9,4,4,5,5,7,7,9,9,7,7,9,9,5,5,7,7,4,4,9,9,3,3,4,4,5,5,7,7,4,4,9,9,6,6,7,7,9,9,4,4,7,7,9,9,5,5,1,9,9,7,7,6,6,4,4,9,9,5,5,9,9,4,4,5,5,8,8,9,9,5,5,9,9,7,7,4,4,9,9,7,7,9,9,5,5,7,7,4,4,8,8,9,9,6,6,9,9,7,7,4,4,9,9,7,7,4,4,9,9,7,7,4,4,7,7,9,9,7,7,1,9,9,1,5,5,9,9,7,7,9,9~6,6,8,8,4,4,6,6,8,8,6,6,8,8,9,9,6,6,8,8,3,3,8,8,6,6,8,8,6,6,8,8,7,7,6,6,5,5,6,6,5,5,8,8,6,6,4,4,8,8,3,3,8,8,6,6,3,3,8,8,6,6,1,3,3,5,5,6,6,8,8,1,3,3,8,8,3,3,9,9,8,8,6,6,8,8,6,6,8,8,6,6,5,5,8,8,3,3,6,6,3,3,5,5,8,8,6,6,7,7,6,6,8,8,6,6,1,8,8,6,6,8,8,6,6,5,5,6,6,5,5,6,6,3,3,8,8,5,5,6,6,5,5,6,6,8,8,7,7~4,4,9,9,7,7,9,9,7,7,5,5,1,4,4,7,7,9,9,8,8,1,9,9,7,7,6,6,7,7,9,9,7,7,4,4,9,9,8,8,4,4,9,9,7,7,9,9,5,5,4,4,9,9,3,3,7,7,4,4,7,7,9,9,4,4,7,7,5,5,9,9,5,5,8,8,4,4,5,5,9,9,4,4,9,9,7,7,9,9,7,7,4,4,6,6,9,9,4,4,9,9,4,4,9,9,7,7,6,6,7,7,9,9,5,5,1,7,7,5,5,3,3&reel_set5=9,9,5,5,9,9,7,7,9,9,8,8,3,3,7,7,4,4,8,8,4,4,8,8,9,9,10,3,3,7,7,8,8,4,4,8,8,9,9,3,3,6,6,8,8,7,7,6,6,3,3,9,9,3,3,7,7,5,5,9,9,4,4,6,6,8,8,5,5,7,7,3,3,9,9,7,7,4,4,8,8,9,9,8,8,9,9,3,3,6,6,5,5,8,8,9,9,7,7,3,3,7,7~8,8,9,9,8,8,6,6,4,4,6,6,3,3,9,9,8,8,4,4,7,7,4,4,8,8,4,4,6,6,3,3,9,9,8,8,9,9,7,7,3,3,4,4,7,7,8,8,9,9,7,7,8,8,5,5,4,4,8,8,4,4,6,6,8,8,9,9,7,7,5,5,9,9,7,7,9,9,7,7,9,9,3,3,9,9,8,8,3,3,10,8,8,9,9,8,8,9,9,5,5,7,7,4,4,8,8,7,7,8,8,7,7,8,8,9,9,8,8,7,7,3,3,9,9,7,7~9,9,6,6,9,9,6,6,7,7,9,9,8,8,7,7,3,3,7,7,6,6,9,9,8,8,9,9,5,5,3,3,6,6,9,9,5,5,8,8,7,7,8,8,6,6,8,8,6,6,4,4,9,9,5,5,7,7,9,9,7,7,8,8,4,4,7,7,9,9,8,8,4,4,7,7,8,8,3,3,7,7,9,9,7,7,4,4,8,8,7,7,4,4,8,8,9,9,8,8,9,9,3,3,6,6,9,9,4,4,8,8,9,9,7,7,8,8,9,9,7,7,8,8,9,9,7,7,8,8,3,3,9,9,7,7,8,8,5,5,10,8,8~3,3,9,9,7,7,8,8,5,5,9,9,8,8,3,3,8,8,4,4,7,7,3,3,8,8,5,5,6,6,8,8,9,9,7,7,4,4,7,7,8,8,7,7,9,9,5,5,6,6,4,4,8,8,9,9,3,3,9,9,5,5,3,3,8,8,6,6,9,9,7,7,8,8,9,9,10,8,8,9,9,7,7,8,8~7,7,3,3,8,8,10,8,8,4,4,8,8,6,6,7,7,9,9,3,3,9,9,8,8,7,7,5,5,8,8,9,9,8,8,9,9,6,6,4,4,7,7,9,9,7,7,6,6,7,7,9,9,8,8,3,3,9,9,8,8,7,7,8,8,7,7,8,8,7,7,9,9,3,3,9,9,3,3,6,6,7,7,8,8,9,9,8,8,5,5,9,9,7,7,5,5,3,3,8,8,3,3,9,9,8,8,9,9,7,7,4,4,9,9,3,3,7,7,5,5,9,9,4,4,9,9,6,6,4,4,9,9~9,9,7,7,9,9,6,6,9,9,6,6,7,7,4,4,8,8,7,7,9,9,8,8,5,5,9,9,7,7,9,9,5,5,8,8,4,4,3,3,7,7,5,5,7,7,8,8,4,4,3,3,9,9,8,8,7,7,9,9,8,8,3,3,7,7,8,8,7,7,8,8,4,4,9,9,9,7,7,4,4,8,8,4,4,3,3,8,8,7,7,9,9,3,3,7,7,6,6,5,5,8,8,3,3,7,7,4,4,7,7,9,9,8,8,9,9,5,5,8,8,9,9,10,8,8,3,3,6,6,9,9,8,8,7,7,4,4,9,9,7,7,9,9,8,8,7,7,9,9&total_bet_min=100.00";
            }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 120; }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override bool HasPurEnableOption
        {
            get { return true; }
        }
        protected override bool SupportMoreBet
        {
            get { return true; }
        }
        protected double[] MoreBetMultiples
        {
            get { return new double[] { 1.5, 3 }; }
        }
        #endregion
        public SantasXmasRushGameLogic()
        {
            _gameID     = GAMEID.SantasXmasRush;
            GameName    = "SantasXmasRush";
        }
        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBets            = new double[BetTypeCount];
                _normalMaxIDs                   = new int[BetTypeCount];
                _naturalSpinCounts              = new int[BetTypeCount];
                _emptySpinCounts                = new int[BetTypeCount];
                _startIDs                       = new int[BetTypeCount];
                var defaultBetsArray            = infoDocument["defaultbet"] as BsonArray;
                var normalMaxIDArray            = infoDocument["normalmaxid"] as BsonArray;
                var emptyCountArray             = infoDocument["emptycount"] as BsonArray;
                var normalSelectCountArray      = infoDocument["normalselectcount"] as BsonArray;
                var startIDArray                = infoDocument["startid"] as BsonArray;

                for (int i = 0; i < BetTypeCount; i++)
                {
                    _spinDataDefaultBets[i] = (double)defaultBetsArray[i];
                    _normalMaxIDs[i]        = (int)normalMaxIDArray[i];
                    _emptySpinCounts[i]     = (int)emptyCountArray[i];
                    _naturalSpinCounts[i]   = (int)normalSelectCountArray[i];
                    _startIDs[i]            = (int)startIDArray[i];
                }
                _spinDataDefaultBet         = _spinDataDefaultBets[0];
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
            }
        }
        protected override async Task spinGame(string strUserID, int agentID, UserBonus userBonus, double userBalance, int index, int counter, Currencies currency, bool isNewBet)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                BasePPSlotBetInfo betInfo = null;
                if (!_dicUserBetInfos.TryGetValue(strGlobalUserID, out betInfo))
                    return;

                byte[] betInfoBytes = backupBetInfo(betInfo);
                byte[] historyBytes = backupHistoryInfo(strGlobalUserID);

                BasePPSlotSpinResult lastResult = null;
                if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                    lastResult = _dicUserResultInfos[strGlobalUserID];

                double betMoney = betInfo.TotalBet;
                if (betInfo.HasRemainResponse)
                    betMoney = 0.0;

                UserBetTypes betType = UserBetTypes.Normal;
                //베팅머니의 100배로 프리스핀구입
                if (this.SupportPurchaseFree && betInfo.PurchaseFree)
                {
                    betMoney = Math.Round(betMoney * getPurchaseMultiple(betInfo), 2);
                    if (betMoney > 0.0)
                        betType = UserBetTypes.PurchaseFree;
                }
                else if (this.SupportMoreBet && betInfo.MoreBet)
                {
                    betMoney = Math.Round(betMoney * getMoreBetMultiple(betInfo), 2);
                    if (betMoney > 0.0)
                        betType = UserBetTypes.AnteBet;
                }
                if (lastResult != null && lastResult.NextAction != ActionTypes.DOSPIN)
                {
                    GITMessage message = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOSPIN);
                    message.Append(string.Format("balance={0}&balance_cash={0}&balance_bonus=0.0&frozen=Internal+server+error.+The+game+will+be+restarted.+&msg_code=11&ext_code=SystemError", Math.Round(userBalance, 2)));
                    ToUserMessage toUserResult = new ToUserMessage((int)_gameID, message);
                    Sender.Tell(toUserResult, Self);
                    _logger.Warning("{0} user did DOSPIN but last result's next action is {1}", strGlobalUserID, lastResult.NextAction);
                    return;
                }
                if (userBalance.LT(betMoney, _epsilion) || betMoney < 0.0)
                {
                    GITMessage message = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOSPIN);
                    message.Append(string.Format("balance={0}&balance_cash={0}&balance_bonus=0.0&frozen=Internal+server+error.+The+game+will+be+restarted.+&msg_code=11&ext_code=SystemError", Math.Round(userBalance, 2)));
                    ToUserMessage toUserResult = new ToUserMessage((int)_gameID, message);
                    Sender.Tell(toUserResult, Self);
                    _logger.Warning("user balance is less than bet money in BasePPSlotGame::spinGame {0} balance:{1}, bet money: {2} game id:{3}",
                        strUserID, userBalance, Math.Round(betMoney, 2), _gameID);
                    return;
                }

                if (isNewBet)
                {
                    betInfo.BetTransactionID = createTransactionID();
                    betInfo.RoundID = createRoundID();
                }

                BasePPSlotSpinResult spinResult = await this.generateSpinResult(betInfo, strUserID, agentID, userBonus, true);
                await overrideResult(betInfo, spinResult, agentID);

                string strGameLog = spinResult.ResultString;
                _dicUserResultInfos[strGlobalUserID] = spinResult;

                saveBetResultInfo(strGlobalUserID);

                sendGameResult(betInfo, spinResult, strUserID, agentID, betMoney, spinResult.WinMoney, strGameLog, userBalance, index, counter, betType, currency);

                _dicUserLastBackupBetInfos[strGlobalUserID]         = betInfoBytes;
                _dicUserLastBackupResultInfos[strGlobalUserID]      = lastResult;
                _dicUserLastBackupHistory[strGlobalUserID]          = historyBytes;

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::spinGame {0}", ex);
            }
        }
        protected double getMoreBetMultiple(BasePPSlotBetInfo betInfo)
        {
            int moreBetType = (betInfo as SantasXmasRushBetInfo).MorebetType;
            return this.MoreBetMultiples[moreBetType - 1];
        }
        protected override async Task<BasePPSlotSpinResult> generateSpinResult(BasePPSlotBetInfo betInfo, string strUserID,  int websiteID, UserBonus userBonus, bool usePayLimit)
        {
            BasePPSlotSpinData      spinData = null;
            BasePPSlotSpinResult    result   = null;

            if (betInfo.HasRemainResponse)
            {
                BasePPActionToResponse nextResponse = betInfo.pullRemainResponse();
                result = calculateResult(betInfo, nextResponse.Response, false);

                //프리게임이 끝났는지를 검사한다.
                if (!betInfo.HasRemainResponse)
                    betInfo.RemainReponses = null;
                return result;
            }

            //유저의 총 베팅액을 얻는다.
            float   totalBet        = betInfo.TotalBet;
            double  realBetMoney    = totalBet;

            if (SupportPurchaseFree && betInfo.PurchaseFree)
                realBetMoney = totalBet * getPurchaseMultiple(betInfo); //100.0
            
            if (SupportMoreBet && betInfo.MoreBet)
                realBetMoney = totalBet * getMoreBetMultiple(betInfo);

            spinData = await selectRandomStop(websiteID, userBonus, totalBet, false, betInfo);

            //첫자료를 가지고 결과를 계산한다.
            double totalWin = realBetMoney * spinData.SpinOdd;
            if (!usePayLimit || spinData.IsEvent || await checkWebsitePayoutRate(websiteID, realBetMoney, totalWin))
            {               
                do
                {
                    if (spinData.IsEvent)
                    {
                        //bool checkRet = await subtractEventMoney(websiteID, strUserID, totalWin);
                        //if (!checkRet)
                        //    break;

                        _bonusSendMessage   = null;
                        _rewardedBonusMoney = totalWin;
                        _isRewardedBonus    = true;
                    }
                    result = calculateResult(betInfo, spinData.SpinStrings[0], true);
                    if (spinData.SpinStrings.Count > 1)
                        betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
                    return result;
                } while (false);
            }

            double emptyWin = 0.0;
            if (SupportPurchaseFree && betInfo.PurchaseFree)
            {
                spinData    = await selectMinStartFreeSpinData(betInfo);
                result      = calculateResult(betInfo, spinData.SpinStrings[0], true);
                emptyWin    = totalBet * spinData.SpinOdd;

                //뒤에 응답자료가 또 있다면
                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
            }
            else
            {
                spinData = await selectEmptySpin(websiteID, betInfo);
                result   = calculateResult(betInfo, spinData.SpinStrings[0], true);
            }
            sumUpWebsiteBetWin(websiteID, realBetMoney, emptyWin);
            return result;
        }

        protected override async Task<OddAndIDData> selectRandomOddAndID(int agentID, BasePPSlotBetInfo betInfo, bool isMoreBet)
        {

            int betType         = ((SantasXmasRushBetInfo)betInfo).MorebetType;
            double payoutRate   = getPayoutRate(agentID);
            double randomDouble = Pcg.Default.NextDouble(0.0, 100.0);
            int selectedID  = 0;

            if (randomDouble >= payoutRate || payoutRate == 0.0)
                selectedID = _startIDs[betType] + Pcg.Default.Next(0, _emptySpinCounts[betType]);
            else
            {
                selectedID = _startIDs[betType] + Pcg.Default.Next(0, _naturalSpinCounts[betType]);
            }

            OddAndIDData selectedOddAndID = new OddAndIDData();
            selectedOddAndID.ID = selectedID;
            return selectedOddAndID;
        }
        protected override async Task<BasePPSlotSpinData> selectEmptySpin(int companyID, BasePPSlotBetInfo betInfo)
        {
            int betType = ((SantasXmasRushBetInfo)betInfo).MorebetType;
            int id      = _startIDs[betType] + Pcg.Default.Next(0, _emptySpinCounts[betType]);
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, id), TimeSpan.FromSeconds(10.0));
            return convertBsonToSpinData(spinDataDocument);
        }
        protected override async Task<BasePPSlotSpinData> selectRangeSpinData(int websiteID, double minOdd, double maxOdd, BasePPSlotBetInfo betInfo)
        {
            int betType = ((SantasXmasRushBetInfo)betInfo).MorebetType;
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                    new SelectSpinTypeOddRangeRequestWithBetType(GameName, -1, minOdd, maxOdd, -1, betType), TimeSpan.FromSeconds(10.0));
            if (spinDataDocument == null)
                return null;
            return convertBsonToSpinData(spinDataDocument);
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "6";
	        dicParams["bl"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("mo_tw"))
                dicParams["mo_tw"] = convertWinByBet(dicParams["mo_tw"], currentBet);
            if (dicParams.ContainsKey("rs_iw"))
                dicParams["rs_iw"] = convertWinByBet(dicParams["rs_iw"], currentBet);
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                SantasXmasRushBetInfo betInfo = new SantasXmasRushBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();
		
                int bl = (int)message.Pop();
                if (bl == 0)
                    betInfo.MoreBet = false;
                else
                    betInfo.MoreBet = true;
                betInfo.MorebetType = bl;

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in SantasXmasRushGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }

                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;

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
                    (oldBetInfo as SantasXmasRushBetInfo).MorebetType = betInfo.MorebetType;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SantasXmasRushGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            SantasXmasRushBetInfo betInfo = new SantasXmasRushBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new SantasXmasRushBetInfo();
        }
    }
}
