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
    class SixJokersBetInfo : BasePPSlotBetInfo
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
    class SixJokersGameLogic : BasePPSlotGame
    {
        protected double[]      _spinDataDefaultBets    = null;
        protected int []        _normalMaxIDs           = null;
        protected int []        _naturalSpinCounts      = null;
        protected int []        _emptySpinCounts        = null;
        protected int []        _startIDs               = null;
        protected int BetTypeCount => 5;   //앤티베트타압카운터야 - 여기서는

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs5magicdoor";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 5; }
        }
        protected override int ServerResLineCount
        {
            get { return 5; }
        }
        protected override int ROWS
        {
            get
            {
                return 5;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=5,7,7,4,3,8,6,10,11,11,6,11,3,7,3,5,5,8,6,10,4,4,6,10,3,11,9,3,5,8&cfgs=1&ver=3&def_sb=4,8,5,6,3,8&reel_set_size=8&def_sa=3,8,11,5,5,11&scatters=1~0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0~0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0~1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1&rt=d&gameInfo={rtps:{ante_a3:\"96.55\",ante_a2:\"96.55\",ante_a1:\"96.55\",ante_a4:\"96.55\",regular:\"96.55\"},props:{max_rnd_win_a1:\"3572\",max_rnd_hr_a1:\"45045045\",max_rnd_win_a3:\"893\",max_rnd_win_a2:\"1786\",max_rnd_sim:\"1\",max_rnd_hr_a4:\"2785515\",max_rnd_hr_a3:\"6535948\",max_rnd_hr_a2:\"14970060\",max_rnd_hr:\"156250000\",max_rnd_win:\"5000\",max_rnd_win_a4:\"417\"}}&wl_i=tbm~5000;tbm_a1~3572;tbm_a2~1786;tbm_a3~893;tbm_a4~417&sc=40.00,80.00,120.00,160.00,200.00,400.00,600.00,800.00,1000.00,1500.00,2000.00,3000.00,5000.00,10000.00,15000.00,20000.00&defc=200.00&wilds=2~0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0~1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1&bonuses=0&bls=5,7,14,28,60&ntp=0.00&paytable=0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;50,50,50,50,50,50,50,50,50,50,50,50,50,50,50,50,50,50,50,50,50,20,20,0,0,0,0,0,0,0;20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,12,12,0,0,0,0,0,0,0;15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15,10,10,0,0,0,0,0,0,0;10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,7,7,0,0,0,0,0,0,0;6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,5,5,0,0,0,0,0,0,0;5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,4,4,0,0,0,0,0,0,0;4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,3,3,0,0,0,0,0,0,0;3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,2,2,0,0,0,0,0,0,0;2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,1,1,0,0,0,0,0,0,0&reel_set0=8,10,3,4,4,11,11,11,5,3,10,7,8,3,3,3,3,9,11,10,11,7,4,3,4,4,4,4,3,6,3,9,5,5,5,8,7,8,3,3,8,5,6,6,6,6,8,3,5,10,3,7,7,7,5,10,9,6,9,11,11,8,8,8,7,11,5,7,6,5,9,9,9,5,4,5,7,6,10,9,6~9,8,9,3,5,4,5,9,5,9,8,11,11,11,10,5,3,7,9,4,8,6,10,10,9,4,3,3,3,7,3,11,11,9,3,4,6,3,7,4,11,4,4,4,8,3,11,6,10,7,11,4,5,11,6,6,5,5,5,5,10,9,6,7,8,6,5,4,4,7,6,4,6,6,6,6,8,6,8,9,4,5,7,6,9,6,7,3,7,7,7,7,6,8,10,11,5,3,6,5,10,6,7,5,9,9,9,7,4,8,8,5,3,11,3,6,7,7,3,9~9,7,10,6,5,4,9,11,11,11,11,10,6,11,5,3,4,5,3,4,3,3,3,4,6,8,8,10,8,7,9,4,4,4,4,7,9,8,8,5,7,5,5,3,5,5,5,5,4,11,9,10,7,7,10,8,6,6,6,6,10,9,3,4,11,6,6,10,7,7,7,10,3,6,11,5,6,5,11,3,8,8,8,3,11,11,9,4,5,5,7,6,10,10,10,10,8,3,8,4,5,4,5,9,5,7~9,3,10,6,11,8,9,3,7,11,11,11,11,8,11,4,7,10,6,10,11,6,3,4,4,4,3,8,7,9,11,10,11,9,5,10,9,5,5,5,3,10,4,9,5,5,4,9,5,8,6,7,7,7,6,11,11,4,11,8,7,10,10,7,9,8,8,8,6,4,9,5,5,4,8,5,9,4,7,9,9,9,7,3,7,4,4,6,8,3,8,10,5,10,10,10,11,10,3,6,9,8,7,11,5,6,3,5~8,5,5,11,6,3,3,3,9,11,6,8,3,8,4,4,4,4,3,5,10,10,9,7,4,5,5,5,10,6,9,4,9,6,6,6,9,4,4,11,8,11,10,7,7,7,5,3,6,10,5,7,11,8,8,8,9,6,3,3,7,9,9,9,11,8,6,7,10,10,8,10,10,10,9,3,6,4,6,5,5,4~5,9,3,3,10,6,9,11,11,11,11,9,7,10,5,3,6,4,10,6,3,3,3,3,6,5,6,10,3,11,5,5,4,4,4,4,3,7,7,11,4,3,8,10,4,5,5,5,5,9,8,8,6,4,11,9,8,5,6,6,6,8,10,11,7,4,5,6,10,7,7,7,10,6,7,8,10,4,9,7,4,8,8,8,8,5,5,4,5,7,4,9,11,9,9,9,9,11,7,5,8,6,11,11,5,3,10,10,10,10,3,8,10,11,4,3,11,11,8,10&reel_set2=7,11,7,10,6,3,8,10,7,11,9,9,7,6,8,9,10,5,9,11,11,11,11,9,6,9,10,5,11,10,9,3,4,11,9,5,7,4,11,6,5,6,11,3,3,3,7,4,9,5,7,6,10,6,10,4,11,5,5,11,3,9,7,8,7,3,9,9,9,9,10,10,5,10,8,6,6,9,7,11,10,10,7,3,9,10,9,5,9,10,10,10,10,8,8,11,8,11,11,10,8,9,7,9,11,8,3,10,8,8,10,10,4,4~11,8,11,7,5,5,10,6,10,7,3,9,11,5,4,4,6,11,11,11,11,10,7,5,10,4,10,11,8,11,6,8,10,9,7,9,8,5,9,9,4,4,4,8,11,6,6,9,5,11,3,9,11,10,7,11,9,10,9,10,4,10,10,10,10,11,3,11,11,6,9,9,11,4,6,4,10,9,8,5,11,4,10,8~7,3,11,10,8,9,5,4,11,7,6,11,6,11,11,11,11,8,3,9,5,5,10,6,11,10,6,8,4,9,10,9,9,9,9,8,10,11,4,11,11,9,5,11,10,9,7,11,10,10,10,4,8,9,11,5,9,11,5,9,10,9,11,8,7,7,6~9,5,6,3,9,8,10,10,4,11,10,3,11,11,11,7,8,10,4,9,11,7,4,8,11,10,11,8,8,8,11,11,7,7,9,8,10,8,5,8,7,7,3,9,9,9,11,6,6,10,4,9,11,9,7,9,7,5,6,10,10,10,10,11,6,8,11,5,9,9,10,5,3,10,9,10~11,3,6,8,11,4,4,11,9,5,7,6,6,11,11,11,11,8,6,7,11,8,8,6,11,10,3,10,5,7,11,9,4,4,4,9,11,11,4,11,10,9,9,10,10,7,10,8,11,10,10,10,11,4,5,10,11,11,9,11,3,10,7,7,5,9,9~9,9,11,3,9,11,4,8,10,11,4,11,10,8,11,9,6,11,8,3,9,4,10,3,11,11,11,6,8,5,10,8,10,6,6,8,5,9,9,6,10,7,7,11,9,11,9,10,11,9,8,5,9,9,9,7,10,10,8,9,4,9,6,9,3,7,10,7,11,7,7,5,9,5,6,6,7,4,10,10,10,8,9,11,8,4,11,4,9,8,7,11,9,8,9,9,3,10,9,3,11,6,10,11,4,10,5&reel_set1=7,7,9,11,3,9,10,6,8,11,11,11,11,11,4,4,9,8,8,11,6,6,4,9,7,5,5,5,10,6,7,4,9,11,8,8,9,7,8,6,6,6,5,10,5,6,6,11,5,11,10,9,9,7,7,7,7,8,9,8,10,6,3,6,11,7,11,7,8,8,8,9,5,9,7,9,7,11,9,5,4,10,9,9,9,11,3,5,4,10,10,11,9,8,3,5,10,10,10,3,9,11,10,7,10,11,6,6,11,5,4~10,3,8,6,8,8,11,6,10,7,10,11,11,11,11,8,8,7,5,7,11,9,9,10,9,11,6,6,6,11,10,5,10,10,9,10,3,9,3,10,4,5,8,8,8,8,6,7,3,7,6,4,5,8,6,8,8,9,9,9,9,3,4,9,11,11,7,9,5,11,8,8,7,9,10,10,10,10,5,9,9,10,6,6,4,9,7,7,11,11,5,4~10,10,8,6,9,5,7,8,11,11,11,6,11,9,9,10,11,9,8,3,7,7,7,9,11,5,3,8,5,4,6,4,7,8,8,8,7,9,10,4,11,9,3,6,11,9,9,9,9,4,6,6,10,10,9,11,3,5,10,10,10,10,6,9,8,4,10,9,8,10,9,5,7~4,6,11,6,11,11,10,11,9,11,5,9,9,6,11,3,5,11,11,11,11,7,9,7,9,8,10,10,9,4,4,9,10,8,6,7,8,6,4,8,8,8,6,5,10,10,6,10,11,8,11,7,4,8,5,7,8,3,11,10,9,9,9,9,10,11,9,11,6,6,10,3,8,6,5,8,8,7,5,3,11,10,10,10,10,10,5,9,3,8,4,11,5,10,9,3,10,11,9,9,7,5,8,10,11~9,11,7,3,7,7,5,11,5,11,9,6,6,3,7,9,8,11,11,11,11,6,8,8,9,8,4,11,11,10,10,9,8,11,9,9,7,11,10,6,6,6,11,6,6,8,6,10,5,6,5,8,4,4,10,8,9,8,8,7,6,9,9,9,3,9,8,9,11,5,7,5,5,7,7,3,7,9,11,9,7,11,10,10,10,10,7,6,10,6,10,10,4,9,4,10,10,4,3,9,5,10,11,4,11,10~4,10,8,10,9,8,9,11,11,3,8,3,11,11,8,11,11,11,11,6,7,10,7,9,10,8,5,4,7,10,10,8,10,9,6,7,7,7,9,5,9,5,10,11,6,5,11,11,5,11,4,7,9,7,8,8,8,8,5,9,11,9,7,7,3,8,11,7,10,3,11,8,10,10,9,9,9,6,11,6,8,6,7,9,10,9,10,9,8,11,8,6,11,7,10,10,10,11,6,10,6,6,4,3,11,11,9,9,11,6,10,9,5,3,6&reel_set4=5,3,9,6,6,9,6,6,7,9,5,6,5,7,9,9,3,7,6,3,6,5,7,9,5,5,7,9,6,5,7,3,3,6,7,7,5,6,9,9,6,7,3,5,6,9,9,5,6,7,7,9,9,6,6,5,3,9,5,6,9,7,6,9,3,5,5,3,6,6,3,9,6,3,5,5,6,7,7,5,9,5,9,5,7,6~10,7,10,11,11,7,11,4,7,10,10,7,11,10,4,11,3,11,7,3,4,10,3,10,7,10,7,4,7,3,11,4,10,4,10,10,11,7,11,10,3,7,11,11,10,3,11,4,7,11,10,7,11,7,10,11,3,10,11,10,10,11,4,10,4,11,11,10,11,3,7,10,3,4,3,4,10,11,11,4,10,11,4,7,10,4,10,4,7,11,11,4,7,3,11,3,7,10,4,4~7,11,6,7,6,7,6,11,11,7,9,6,7,9,9,7,9,9,11,6,7,11,6,6,11,9,11,4,11,6,11,9,7,11,7,11,4,11,9,11,7,6,9,11,9,9,7,7,4,6,11,6,6,7,9,7,7,9,9,6,4,11,9,7,9~8,4,5,10,5,4,4,10,5,5,8,8,5,4,8,8,10,10,8,5,10,7,5,8,5,4,10,7,8,4,4,5,8,8,10,7,5,4,5,4,8,10,4,8,10,8,8,10,8,10,4,4,8,10,5,4,4,7,4,10,8,5,5,7,4,4,10,4,7,8~6,6,11,3,6,3,8,11,10,6,6,10,6,3,10,6,11,6,3,8,6,10,8,3,10,6,3,6,3,10,10,6,6,3,10,11,10,3,8,11,8,10,10,6,6,3,8,6,3,11,6,8,8,3,10,8,3,10,6,3,10,8,10,3,8,8,3,11,3,8,8,3,11,8,6,6,3,10,3,8~5,11,4,4,9,9,5,11,11,8,8,9,5,11,4,8,11,4,5,9,4,11,11,5,5,9,8,4,5,5,9,11,5,8,9,8,9,8,11,8,4,9,9,8,5,5,11,5,4,11,8,8,9,11,5,5,8,11,9,4,8,9,8,9,9,11,4,4,9,9&reel_set3=11,10,9,4,7,11,8,7,8,11,11,11,11,9,10,9,10,8,4,9,10,10,9,5,7,7,7,3,11,9,8,8,9,7,10,8,7,10,8,8,8,8,8,5,6,7,6,9,9,11,8,3,7,9,9,9,9,9,9,3,11,6,11,8,9,9,5,10,11,9,10,10,10,10,10,7,11,3,10,10,11,4,4,11,5,6,8~3,10,6,7,11,11,11,11,10,9,4,4,8,11,10,4,4,4,8,10,4,7,8,3,7,5,5,5,5,11,5,9,10,7,9,6,6,6,11,5,8,11,9,8,4,7,7,7,7,6,11,9,9,10,11,3,8,8,8,8,8,6,6,10,6,5,5,11,9,9,9,9,9,10,8,9,6,9,10,8,10,10,10,10,9,6,10,7,9,11,11,8~9,10,8,7,11,10,11,9,7,9,4,7,11,11,11,11,11,11,6,8,11,8,11,9,8,9,4,9,7,10,11,4,4,4,9,7,4,10,11,11,9,9,7,9,8,11,10,7,7,7,6,8,10,10,5,11,5,9,11,9,10,9,9,8,8,8,10,5,11,10,3,11,11,4,8,11,11,10,10,9,9,9,9,8,11,10,4,11,10,5,3,6,7,4,8,4,10,10,10,10,10,8,5,5,11,7,11,10,10,7,6,6,9,10,11,6~9,9,5,7,4,5,6,11,11,11,11,10,6,11,10,10,8,9,8,9,8,5,5,5,7,11,9,11,11,7,11,7,10,7,7,7,5,9,10,7,6,6,11,7,11,11,8,8,8,9,8,8,7,3,8,9,10,9,6,9,9,9,9,9,11,7,9,4,10,5,6,6,5,10,10,10,10,10,3,9,3,10,9,5,3,10,10,4,10~3,8,6,4,7,9,11,11,11,11,11,7,5,6,10,4,7,10,3,3,3,6,9,6,9,11,10,10,6,6,6,8,6,10,5,8,6,11,7,7,7,7,5,7,11,11,9,11,8,8,8,6,3,10,4,5,10,8,9,9,9,11,5,6,10,11,10,10,10,10,10,11,8,8,10,11,7,9,4~11,11,7,7,10,5,6,11,11,11,11,6,5,9,5,10,11,9,8,6,6,6,9,4,10,8,9,10,8,10,7,7,7,11,4,11,7,4,8,8,10,8,8,8,8,8,8,5,11,5,8,10,9,6,5,9,9,9,9,9,4,10,8,7,11,10,11,8,10,10,10,10,10,10,3,10,9,11,10,6,8,11,6,8&reel_set6=6,2,10,4,4,11,10,10,5,3,7,6,10,11,11,11,8,7,9,9,5,10,7,5,8,4,8,5,4,7,6,6,6,8,6,11,11,8,11,11,6,2,5,6,7,9,11,7,7,7,11,4,7,9,8,8,9,9,2,11,10,11,9,10,8,8,8,9,8,11,9,8,4,6,4,8,10,7,2,11,2,9,9,9,2,4,7,2,3,9,9,8,6,6,11,10,8,10,10,10,5,6,7,7,3,3,7,9,2,6,6,10,10,5,5~7,3,6,10,11,2,11,3,9,11,8,3,4,8,6,11,11,10,7,2,10,11,2,8,8,8,10,9,10,4,5,10,3,11,2,9,8,5,8,2,10,9,9,5,10,4,11,8,10,6,9,9,9,8,6,7,10,9,2,8,5,6,7,10,4,8,11,5,10,5,9,11,8,7,7,11,7,8,10,10,10,5,8,6,9,11,7,9,7,8,3,8,7,4,2,6,9,6,6,11,8,5,2,4,4,10,8~8,5,10,8,4,4,7,9,9,2,11,10,2,8,6,6,6,8,9,5,8,11,6,6,10,11,3,9,3,9,9,7,9,10,9,9,9,6,7,6,8,3,2,4,5,5,2,8,11,9,3,11,10,10,10,4,5,7,5,7,7,2,11,10,6,6,2,11,11,7,9,9~5,10,10,11,9,11,4,7,11,4,4,4,7,10,5,9,6,9,11,5,4,2,5,5,5,8,7,4,2,7,9,10,6,9,7,6,6,6,2,8,11,2,11,9,9,3,7,3,7,7,7,5,8,6,6,8,10,8,6,10,5,3,9,9,9,5,7,10,2,11,9,3,3,10,10,10,8,6,4,5,4,8,8,5,9,7,6,2~4,7,11,4,10,8,10,8,2,9,9,11,11,11,4,6,6,11,5,4,10,9,4,2,9,9,10,4,4,4,10,3,5,8,10,8,7,6,7,6,9,3,7,11,5,5,5,8,3,2,10,5,3,2,8,5,4,2,6,6,7,7,7,5,5,3,4,8,11,6,11,7,8,2,10,4,8,8,8,10,11,5,11,8,7,9,2,10,7,9,11,2,9,9,9,11,2,5,4,6,10,9,8,3,7,5,7,5,7,11~9,4,2,7,11,8,4,5,9,11,2,10,7,11,5,3,8,10,4,8,2,4,2,7,7,7,2,10,7,11,5,6,5,8,6,10,7,8,8,10,3,8,5,10,5,9,6,11,2,5,7,8,8,8,7,9,11,6,11,10,2,7,10,2,11,9,2,7,9,11,6,9,10,11,6,4,8,9,5,10,10,10,3,10,10,9,3,11,7,6,3,6,6,4,10,10,8,6,11,10,4,8,9,10,8,3,10,7&reel_set5=10,8,6,7,5,10,8,3,3,6,7,11,11,11,7,6,7,11,9,8,7,10,9,10,6,9,8,5,5,5,9,8,5,4,11,4,3,8,4,6,9,8,7,7,7,4,8,6,7,4,9,5,5,7,5,11,3,9,8,8,8,5,9,9,7,2,11,11,4,11,5,11,2,9,9,9,4,10,6,10,11,9,5,6,6,10,10,2,7,3~9,6,7,5,10,11,3,11,8,11,11,11,9,4,11,7,10,2,5,10,8,5,10,5,5,5,8,11,7,7,6,7,11,9,4,11,10,7,7,7,6,9,9,7,6,7,8,4,6,8,7,8,8,8,7,11,4,11,4,2,3,7,2,8,7,9,9,9,3,11,10,7,8,8,5,5,6,7,10,10,10,9,8,9,9,10,5,11,3,11,11,7,5~11,11,2,5,11,8,7,5,9,6,11,6,11,11,11,9,10,8,7,6,4,6,10,2,7,7,8,7,7,7,11,8,11,6,11,9,3,5,3,11,10,7,9,9,9,10,8,7,9,11,5,10,5,4,9,10,7,3,10,10,10,8,4,5,11,8,9,2,10,9,6,4,3,10~6,11,8,7,10,6,9,5,11,11,11,4,7,2,9,6,11,2,9,6,3,3,3,7,9,11,8,7,10,11,4,6,9,4,4,4,6,9,4,5,9,2,3,11,5,5,5,3,7,7,3,8,10,8,10,9,8,8,8,10,6,3,3,8,10,11,6,7,6,9,9,9,5,7,10,8,8,5,11,4,6,10,10,10,3,11,10,2,10,11,9,7,9,8,10~4,5,10,11,8,3,7,7,11,7,10,4,3,3,11,9,11,11,11,7,2,6,8,10,7,5,4,8,10,6,11,7,10,11,10,9,8,5,5,5,9,8,10,10,5,11,7,9,2,10,10,7,5,3,7,11,3,7,7,7,11,11,10,11,5,8,8,6,10,3,11,2,4,9,5,11,7,9,10,10,10,2,5,7,4,10,10,6,9,8,3,6,11,9,9,8,11,5,11,9~6,8,11,8,9,8,4,4,4,6,3,8,10,3,10,7,3,5,5,5,2,3,9,8,10,3,11,6,6,6,4,4,10,11,4,6,2,7,7,7,10,8,7,8,6,11,4,4,8,8,8,11,5,7,8,7,10,10,9,9,9,5,7,10,5,6,8,9,6,10,10,10,7,11,9,5,4,11,5,9,5&reel_set7=11,6,11,7,6,10,6,3,3,3,9,5,7,5,4,9,7,11,5,4,4,4,5,6,6,11,10,3,10,7,6,5,5,5,7,11,4,8,7,3,9,8,7,7,7,5,3,8,4,4,10,11,9,6,8,8,8,2,5,5,9,10,8,7,4,9,9,9,4,9,7,4,8,3,10,3,11,7~11,5,5,8,3,11,11,11,9,5,11,4,9,3,3,3,6,11,9,8,7,4,4,4,6,7,9,4,6,7,7,5,5,5,10,10,6,4,2,6,6,6,8,8,7,9,2,8,7,7,7,5,8,11,10,5,9,6,8,8,8,10,3,10,4,3,11,9,9,9,4,7,5,8,10,5,10,10,10,9,6,7,4,6,7,3,11~6,8,5,9,6,9,9,7,3,11,11,11,4,11,7,3,10,6,9,6,3,11,3,3,3,11,7,10,3,10,11,10,2,9,8,5,5,5,8,7,5,6,8,8,9,9,4,5,6,6,6,4,9,11,3,4,11,6,11,6,10,7,7,7,10,9,7,6,6,5,6,8,10,3,9,9,9,5,5,11,4,9,4,7,9,7,10,10,10,11,11,8,5,8,7,11,5,7,9,10~4,3,5,5,6,8,3,11,11,11,4,9,7,10,11,3,8,7,7,3,3,3,6,9,5,3,8,9,8,9,11,4,4,4,3,6,4,7,6,6,3,11,9,5,5,5,10,10,3,5,7,10,8,11,11,6,6,6,9,9,6,9,6,6,11,9,7,7,7,10,11,6,10,6,10,7,8,4,8,8,8,3,4,7,4,8,11,4,11,7,9,9,9,8,9,5,10,10,6,2,5,10,10,10,5,3,11,8,6,7,6,7,8,5~10,5,2,3,8,4,8,10,4,5,4,7,11,11,11,7,3,9,10,9,10,11,9,3,2,7,11,10,11,4,4,4,7,6,8,3,10,7,3,11,3,6,7,10,8,9,5,5,5,10,6,4,6,4,9,8,9,9,6,11,7,10,4,9,6,6,6,10,7,5,5,6,7,10,7,8,5,10,4,6,7,7,7,5,4,8,9,4,5,5,3,8,9,11,6,11,6,10,10,10,8,10,11,9,5,8,5,10,5,7,3,11,11,4,10~5,4,10,9,3,10,6,3,9,8,11,3,3,3,8,3,9,7,4,7,5,10,10,6,11,9,4,4,4,10,10,5,6,11,8,7,9,6,11,4,5,7,5,5,5,6,8,8,3,7,4,11,3,10,7,5,3,6,6,6,11,7,5,3,11,5,4,3,11,4,10,6,8,8,8,7,4,8,5,10,11,7,6,4,8,3,9,5,9,9,9,8,10,4,8,4,8,9,3,11,6,8,6,10,10,10,2,6,4,5,7,8,6,11,10,3,9,4,9,11";
            }
        }
        protected override bool SupportMoreBet
        {
            get { return true; }
        }
        protected double[] MoreBetMultiples
        {
            get { return new double[] { 1.4, 2.8, 5.6, 12 }; }
        }
        #endregion

        public SixJokersGameLogic()
        {
            _gameID     = GAMEID.SixJokers;
            GameName    = "SixJokers";
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
        protected override async Task spinGame(string strUserID, int agentID, UserBonus userBonus, double userBalance, int index, int counter, Currencies currency, bool isNewBet, bool isAffiliate, PPFreeSpinInfo freeSpinInfo)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                BasePPSlotBetInfo betInfo = null;
                if (!_dicUserBetInfos.TryGetValue(strGlobalUserID, out betInfo))
                    return;

                byte[] betInfoBytes = backupBetInfo(betInfo);
                byte[] historyBytes = backupHistoryInfo(strGlobalUserID);
                byte[] freeSpinBytes = backupFreeSpinInfo(strGlobalUserID);

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
                if (freeSpinInfo == null && (userBalance.LT(betMoney, _epsilion) || betMoney < 0.0))
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
                    if (freeSpinInfo != null)
                        freeSpinInfo.RemainCount -= 1;
                }

                BasePPSlotSpinResult spinResult = await this.generateSpinResult(betInfo, strUserID, agentID, userBonus, true, isAffiliate, freeSpinInfo);
                await overrideResult(betInfo, spinResult, agentID, isAffiliate);

                string strGameLog = spinResult.ResultString;
                _dicUserResultInfos[strGlobalUserID] = spinResult;

                saveBetResultInfo(strGlobalUserID);

                sendGameResult(betInfo, spinResult, strUserID, agentID, betMoney, spinResult.WinMoney, strGameLog, userBalance, index, counter, betType, currency, freeSpinInfo);

                _dicUserLastBackupBetInfos[strGlobalUserID]         = betInfoBytes;
                _dicUserLastBackupResultInfos[strGlobalUserID]      = lastResult;
                _dicUserLastBackupHistory[strGlobalUserID]          = historyBytes;
                _dicUserLastBackupFreeSpinInfos[strGlobalUserID]    = freeSpinBytes;

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::spinGame {0}", ex);
            }
        }
        protected double getMoreBetMultiple(BasePPSlotBetInfo betInfo)
        {
            int moreBetType = (betInfo as SixJokersBetInfo).MorebetType;
            return this.MoreBetMultiples[moreBetType - 1];
        }
        protected override async Task<BasePPSlotSpinResult> generateSpinResult(BasePPSlotBetInfo betInfo, string strUserID,  int websiteID, UserBonus userBonus, bool usePayLimit, bool isAffiliate, PPFreeSpinInfo freeSpinInfo)
        {
            BasePPSlotSpinData      spinData = null;
            BasePPSlotSpinResult    result   = null;

            if (betInfo.HasRemainResponse)
            {
                BasePPActionToResponse nextResponse = betInfo.pullRemainResponse();
                result = calculateResult(betInfo, nextResponse.Response, false, freeSpinInfo);

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

            spinData = await selectRandomStop(websiteID, userBonus, totalBet, false, betInfo, isAffiliate);

            //첫자료를 가지고 결과를 계산한다.
            double totalWin = realBetMoney * spinData.SpinOdd;
            if (isAffiliate || (freeSpinInfo != null) || !usePayLimit || spinData.IsEvent || await checkWebsitePayoutRate(websiteID, realBetMoney, totalWin))
            {               
                do
                {
                    if (spinData.IsEvent)
                    {
                        bool checkRet = await subtractEventMoney(websiteID, strUserID, totalWin);
                        if (!checkRet)
                            break;

                        _bonusSendMessage   = null;
                        _rewardedBonusMoney = totalWin;
                        _isRewardedBonus    = true;
                    }
                    result = calculateResult(betInfo, spinData.SpinStrings[0], true, freeSpinInfo);
                    if (spinData.SpinStrings.Count > 1)
                        betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
                    return result;
                } while (false);
            }

            double emptyWin = 0.0;
            if (SupportPurchaseFree && betInfo.PurchaseFree)
            {
                spinData    = await selectMinStartFreeSpinData(betInfo);
                result      = calculateResult(betInfo, spinData.SpinStrings[0], true, freeSpinInfo);
                emptyWin    = totalBet * spinData.SpinOdd;

                //뒤에 응답자료가 또 있다면
                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
            }
            else
            {
                spinData = await selectEmptySpin(websiteID, betInfo, isAffiliate);
                result   = calculateResult(betInfo, spinData.SpinStrings[0], true, freeSpinInfo);
            }
            sumUpWebsiteBetWin(websiteID, realBetMoney, emptyWin);
            return result;
        }
        protected override async Task<OddAndIDData> selectRandomOddAndID(int agentID, BasePPSlotBetInfo betInfo, bool isMoreBet, bool isAffiliate)
        {
      
            int     betType         = ((SixJokersBetInfo)betInfo).MorebetType;
            double  payoutRate      = getPayoutRate(agentID, isAffiliate);
            double  randomDouble    = Pcg.Default.NextDouble(0.0, 100.0);
            int selectedID  = 0;
            int affliateCnt = 0;

            if (randomDouble >= payoutRate || payoutRate == 0.0)
                selectedID = _startIDs[betType] + Pcg.Default.Next(0, _emptySpinCounts[betType]);
            else
            {
                if(!isAffiliate)
                    selectedID = _startIDs[betType] + Pcg.Default.Next(0, _naturalSpinCounts[betType]);
                else
                {
                    affliateCnt = _naturalSpinCounts[betType];
                    selectedID = _startIDs[betType] + Pcg.Default.Next(0, _naturalSpinCounts[betType]);
                }
            }

            OddAndIDData selectedOddAndID = new OddAndIDData();
            selectedOddAndID.ID = selectedID;
            return selectedOddAndID;
        }
        protected override async Task<BasePPSlotSpinData> selectEmptySpin(int companyID, BasePPSlotBetInfo betInfo, bool isAffiliate)
        {
            int betType = ((SixJokersBetInfo)betInfo).MorebetType;
            int id = _startIDs[betType] + Pcg.Default.Next(0, _emptySpinCounts[betType]);
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, id), TimeSpan.FromSeconds(10.0));
            return convertBsonToSpinData(spinDataDocument);
        }
        protected override async Task<BasePPSlotSpinData> selectRangeSpinData(int websiteID, double minOdd, double maxOdd, BasePPSlotBetInfo betInfo, bool isAffiliate)
        {
            int betType = ((SixJokersBetInfo)betInfo).MorebetType;
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                    new SelectSpinTypeOddRangeRequestWithBetType(GameName, -1, minOdd, maxOdd, -1, betType), TimeSpan.FromSeconds(10.0));
            if (spinDataDocument == null)
                return null;
            return convertBsonToSpinData(spinDataDocument);
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"]   = "0";
	        dicParams["st"]         = "rect";
	        dicParams["sw"]         = "6";
	        dicParams["bl"]         = "0";
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


                SixJokersBetInfo betInfo = new SixJokersBetInfo();
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in SixJokersGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                    (oldBetInfo as SixJokersBetInfo).MorebetType = betInfo.MorebetType;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SixJokersGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            SixJokersBetInfo betInfo = new SixJokersBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new SixJokersBetInfo();
        }
        protected override string makeSpinResultString(BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult, double betMoney, double userBalance, int index, int counter, bool isInit)
        {
            Dictionary<string, string> dicParams = splitResponseToParams(spinResult.ResultString);
            
            if (spinResult.HasBonusResult)
            {
                Dictionary<string, string> dicBonusParams = splitResponseToParams(spinResult.BonusResultString);
                dicParams = mergeSpinToBonus(dicParams, dicBonusParams);
            }
            
            dicParams["balance_bonus"]  = "0.00";
            dicParams["stime"]          = GameUtils.GetCurrentUnixTimestampMillis().ToString();
            dicParams["sver"]           = "5";
            dicParams["l"]              = ServerResLineCount.ToString();
            dicParams["sh"]             = ROWS.ToString();
            dicParams["c"]              = Math.Round(betInfo.BetPerLine, 2).ToString();
            if (index > 0)
            {
                dicParams["index"]      =  index.ToString();
                dicParams["counter"]    = (counter + 1).ToString();
            }

            ActionTypes nextAction = convertStringToActionType(dicParams["na"]);
            if (isInit)
            {
                dicParams["na"]     = convertActionTypeToString(spinResult.NextAction);
                dicParams["action"] = convertActionTypeToFullString(spinResult.NextAction);
            }
            else
            {
                dicParams["na"]     = convertActionTypeToString(spinResult.NextAction);
            }

            dicParams["balance"]        = Math.Round(userBalance - (isInit ? 0.0 : betMoney), 2).ToString();        //밸런스
            dicParams["balance_cash"]   = Math.Round(userBalance - (isInit ? 0.0 : betMoney), 2).ToString();        //밸런스케시

            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = "0";
            else
                dicParams.Remove("puri");

            dicParams["bl"] = ((SixJokersBetInfo)betInfo).MorebetType.ToString();
            if (isInit)
                supplementInitResult(dicParams, betInfo, spinResult);

            overrideSomeParams(betInfo, dicParams);
            writeRoundID(betInfo, dicParams);

            return convertKeyValuesToString(dicParams);
        }
        
    }
}
