using Akka.Actor;
using GITProtocol;
using GITProtocol.Utils;
using MongoDB.Bson;
using PCGSharp;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class ZeusVsHadesGodsOfWarBetInfo : BasePPSlotBetInfo
    {
        public int BetType      { get; set; }
        public int PurchaseType { get; set; }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            this.BetType        = reader.ReadInt32();
            this.PurchaseType   = reader.ReadInt32();
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(this.BetType);
            writer.Write(PurchaseType);
        }
        public override float TotalBet
        {
            get { return this.BetPerLine * 10; }
        }
    }
    class ZeusVsHadesGodsOfWarGameLogic : BasePPSlotGame
    {
        protected int           _normalMaxID2       = 0;
        protected int           _naturalSpinCount2  = 0;
        protected int           _emptySpinCount2    = 0;
        protected int           _anteStartID        = 0;

        protected double[]      _multiTotalFreeSpinWinRates;
        protected double[]      _multiMinFreeSpinWinRates;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs15godsofwar";
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
            get { return 15; }
        }
        protected override int ServerResLineCount
        {
            get { return 10; }
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
                return "def_s=4,10,9,7,9,6,8,10,6,11,3,11,5,3,8,5,8,9,4,9,4,9,7,6,11&cfgs=7762&ver=3&def_sb=7,12,11,3,8&reel_set_size=19&def_sa=3,10,5,3,8&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr_400:\"1831501\",max_rnd_win:\"15000\",max_rnd_hr_200:\"1394700\"}}&wl_i=tbm~15000&reel_set10=11,7,7,6,10,10,8,12,6,12,5,9,10,11,7,11,5,8,11,6,12,12,12,11,4,7,4,8,5,9,12,7,11,9,4,3,10,8,12,3,9,3,9,4,7,9,5~7,9,6,5,12,5,7,11,7,10,6,6,8,6,12,7,3,5,10,3,12,3,12,4,8,12,8,9,11,9,8,11,4,11,13,11,12,7,9,8,5,13,11,4,6,4,8,7,11,5,4,5,4,10,10,11,4,10,9,9~6,7,10,11,8,3,10,9,12,8,12,8,4,12,8,4,5,8,12,7,10,9,5,9,5,7,10,7,5,9,11,7,6,3,6,12,3,10,4,10,7,11,8,11,12,6~8,11,10,12,10,3,12,11,8,7,4,6,12,4,9,4,5,9,8,7,3,9,6,7,12,5,7,8,11,5,10,6,11,11,10,5,8,10,12,3,6,10,8,7,5,4,7,9~5,9,12,4,12,6,7,9,9,7,4,10,11,6,8,11,13,9,11,5,7,4,4,7,12,10,11,7,11,12,8,11,10,3,12,4,8,7,3,9,9,5,10,6,9,7,3,8,5,8,3,11,8,12,10,10,11,12,7,6&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&reel_set11=8,11,10,6,6,11,4,10,7,8,4,7,8,5,8,6,11,5,10,9,6,12,6,9,4,9,6,11,3,9,6,10,6,12,6,6,6,6,12,8,12,7,12,10,4,12,3,7,8,9,11,12,7,12,8,7,9,5,7,6,5,11,8,7,6,5,9,10,11,10,3,9,8~8,9,7,12,5,10,6,9,9,7,7,10,5,6,8,11,8,10,4,7,9,11,11,10,5,3,7,5,7,11,3,4,7,7,7,7,9,12,8,6,8,11,6,7,4,10,6,7,4,7,13,3,12,5,10,7,4,9,12,4,3,12,5,8,6,5,12,5,11,7~4,12,6,10,8,4,6,9,6,5,12,7,9,10,5,9,5,9,8,7,7,11,12,11,10,11,10,10,10,10,6,5,13,9,11,12,6,10,4,7,4,3,12,3,3,11,8,3,5,4,10,8,13,8,3,5,13,7~9,3,10,3,11,11,9,11,8,3,5,10,12,11,4,5,10,5,5,5,6,9,5,10,6,9,4,12,5,11,12,7,6,8,7,12,7,7,7,8,9,8,4,4,8,6,7,6,4,11,8,8,10,3,12,7,9,12~5,9,8,9,4,10,7,6,8,9,5,3,12,8,11,12,3,5,5,7,7,7,3,10,7,11,7,12,11,9,6,13,6,3,10,7,6,11,4,7,8,12,4,10,4&reel_set12=4,11,7,10,13,7,9,3,11,10,9,8,12,12,12,3,12,9,12,10,13,10,5,11,7,5,9,9,5,9,9,9,6,7,11,6,11,12,10,11,7,6,7,7,10,10,10,12,7,12,10,12,6,5,4,11,10,8,4,8~5,10,12,8,6,12,12,8,3,8,11,5,7,9,9,9,7,4,6,5,6,12,7,11,9,3,7,4,10,9,11,11,11,11,12,5,11,8,7,7,12,11,6,9,8,6,7,12,12,12,12,6,9,4,9,10,11,10,12,11,6,3,5,6,5,4,10,10,10,11,11,9,10,9,11,7,3,10,11,9,8,12,11,4,8~4,7,4,7,3,7,10,6,3,5,12,4,11,5,8,7,11,10,11,6,9,9,10,10,4,12,12,6,11,10,3,7,8,8,9,8,12,8,7,3,5,7,5,9,13,10,11,12,8,12,12,4,6,9~4,8,12,8,7,9,5,10,9,8,9,5,6,10,4,9,9,9,3,7,11,4,9,8,11,6,12,12,11,10,6,13,12,3,5,7,7,7,8,11,13,5,6,3,3,7,11,10,7,8,10,12,11,10,7~6,4,5,11,6,4,3,7,11,9,7,4,10,8,9,10,12,4,10,7,12,12,5,10,4,11,7,9,10,10,10,8,3,7,8,3,10,12,3,11,5,6,9,6,7,10,8,12,9,6,10,8,11,5,7,11,7,10,8,5,8&purInit_e=1,1,1,1&reel_set13=5,7,11,2,10,7,4,6,10,9,10,9,7,9,8,7,11,8,7,12,8,4,6,8,4,3,12,4,5,4,9,11,3,8,12,12,12,8,9,11,5,9,6,7,11,7,6,5,9,11,7,4,10,12,9,11,12,5,12,10,5,8,10,11,3,9,11,5,4,8,7,12,12,3~8,6,9,11,7,8,7,5,6,6,8,12,9,10,13,12,5,8,11,10,4,5,4,11,5,11,4,10,10,12,3,4,12,7,8,3,6,3,5,8,11,7,4,12,11,9,11,8,9,6,12,11,10,6,8,10,5,7,3,9,8,9,2,4~12,5,10,6,8,3,12,10,9,3,11,4,10,12,9,10,8,7,7,11,12,3,5,8,11,5,6,5,8,7,7,10,8,10,7,9,4,7,6,4,12,12,8,9,6,11,4~10,11,9,9,8,4,7,9,6,7,4,11,3,8,10,12,10,12,3,12,5,5,12,10,4,10,7,10,4,11,6,7,9,10,8,4,5,7,7,10,10,8,5,4,8,11,3,12,6,3,5,6,8,6,5,7,8,11,11,7,6,9,12,12,11,5,7,8,8,9~5,12,3,4,4,5,4,7,11,12,4,12,11,7,11,10,8,12,7,6,3,4,6,11,9,13,5,7,6,10,12,11,9,9,8,7,9,7,6,8,5,10,9,9,12,5,4,11,12,8,3,7,9,10,8,10,11,10,6,11,10,8,2,7,5,6,5,7&wilds=2~200,100,20,0,0~1,1,1,1,1&bonuses=0&reel_set18=8,7,3,5,12,5,4,9,9,11,12,12,12,6,7,11,7,9,10,12,3,12,7,8,10,9,9,9,10,12,6,10,8,6,11,3,7,5,10,7,10,10,10,4,8,11,12,5,6,11,7,4,10,9,11,13~3,11,6,5,8,6,6,10,12,10,6,7,8,9,9,9,12,11,12,12,8,10,10,6,11,4,7,9,11,11,11,11,8,12,5,10,12,9,3,7,11,4,6,4,3,5,12,12,12,12,9,12,5,5,9,6,11,5,11,4,11,7,7,9,10,10,10,11,7,7,8,11,6,8,9,4,8,10,7,3,11,11~3,11,5,10,12,7,8,4,10,7,4,11,10,11,3,5,8,12,9,7,12,4,4,6,3,11,8,12,10,6,7,12,7,10,12,9,11,6,12,4,13,8,8,6,9,5,10,9,3,6,7,5,7,8,9,8~9,4,11,9,13,12,5,11,7,8,6,3,3,9,6,9,9,9,8,11,6,5,13,7,11,3,11,7,10,12,8,12,10,10,4,7,7,7,4,12,10,10,8,5,7,10,8,5,8,11,12,3,9,7,7,6~7,6,10,10,3,10,12,6,7,3,7,4,8,9,11,10,5,3,11,9,7,5,5,7,6,10,10,10,8,4,8,10,9,5,4,3,11,7,9,12,12,10,7,6,11,8,12,5,6,4,11,7,4,12,8,10,9&reel_set14=4,7,4,6,11,5,10,7,9,10,8,9,7,8,9,4,6,6,7,9,11,9,10,7,6,6,6,8,3,10,5,3,12,12,11,10,5,8,11,12,11,12,10,12,12,5,8,3,6,10,2,9,12,6,8,6~9,7,11,11,7,4,9,3,8,4,5,8,7,7,10,9,4,10,4,10,7,12,10,8,4,11,7,7,7,7,5,6,7,5,8,7,8,11,12,9,5,9,12,5,12,5,10,2,7,3,6,6,3,6,11,7,6~12,3,9,10,3,3,12,4,5,10,5,7,9,8,7,3,4,11,10,10,10,10,12,6,9,11,6,11,6,9,6,7,8,11,8,5,8,10,4,5,10,13,12~5,9,8,10,11,9,4,8,3,10,8,11,7,8,12,12,11,5,5,5,11,6,5,3,9,6,4,12,6,10,4,12,9,6,4,11,3,7,7,7,9,10,7,11,8,5,7,8,7,12,8,9,5,6,10,3,7,12,4~7,10,5,8,3,11,4,6,11,12,4,10,6,7,7,12,3,5,7,12,8,5,7,10,10,9,11,8,11,7,4,2,7,7,7,3,10,12,8,9,7,10,5,9,5,8,4,7,6,8,3,12,2,11,4,12,5,9,7,9,3,4,4,11,5,12,8,9,7,10,3,6&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;200,100,20,0,0;100,50,10,0,0;100,50,10,0,0;50,25,5,0,0;50,25,5,0,0;10,5,1,0,0;10,5,1,0,0;10,5,1,0,0;10,5,1,0,0;10,5,1,0,0;0,0,0,0,0&reel_set15=12,10,8,3,12,7,11,12,7,7,11,6,4,12,12,12,4,3,7,12,6,10,12,6,7,11,10,10,5,8,9,9,9,6,7,7,9,9,8,12,9,10,5,11,10,12,7,11,10,10,10,8,4,6,2,4,7,9,11,10,3,3,5,9,8,7,11,10~11,12,10,9,8,4,9,12,9,9,9,5,11,8,5,3,9,11,4,7,11,11,11,11,7,3,11,7,10,10,4,3,12,12,12,12,5,11,6,8,7,8,10,10,10,6,6,5,9,8,7,9,11,12,6,6~10,11,7,11,6,7,4,3,5,12,11,9,8,3,8,7,8,6,8,8,5,10,7,9,7,10,6,12,10,6,5,6,12,6,9,9,4,4,10,7,10,12,4,2,12,11,9,12,10,5,3,7,12,12,5,10,9,11,3,12,4,8,11,4,3,8,8~11,6,8,3,12,3,6,4,12,6,9,8,5,4,9,11,5,9,8,4,11,9,9,9,2,8,7,11,7,3,6,10,7,11,4,3,7,10,6,12,5,4,12,12,10,9,7,7,7,5,11,8,13,10,8,9,13,5,7,9,7,8,5,7,8,10,7,10,12,10,12,3,10~2,10,5,4,8,3,7,12,6,12,11,5,10,9,7,8,10,5,7,10,8,4,10,11,7,4,8,3,10,7,10,10,10,12,4,11,10,11,9,9,7,9,6,11,4,8,7,5,11,8,12,5,12,6,11,7,10,6,8,9,3,7,3&reel_set16=8,6,7,7,9,8,5,12,3,11,8,6,9,3,10,4,7,11,12,9,10,7,12,4,10,6,9,8,11,3,5,12,11,12,12,12,5,10,11,7,4,9,7,9,9,11,7,11,7,9,12,3,4,8,3,5,8,12,7,8,9,5,11,11,4,10,10,6,5,4,12~12,4,10,4,10,7,3,8,10,3,9,4,9,12,4,11,8,5,9,7,9,12,8,13,5,11,5,11,7,6,5,11,6,8,11,6~8,7,10,5,10,9,4,9,10,3,12,10,7,10,4,12,11,7,7,12,8,4,11,6,12,8,7,5,11,8,5,6,4,6,5,3,7,9,8,9,6,12,12,11,3,10~9,11,5,8,6,11,8,6,7,4,9,6,8,11,5,5,12,12,8,10,12,3,9,7,7,10,3,6,10,12,5,10,11,4,8,10,7,7,4~9,9,5,6,12,10,11,6,13,8,11,5,4,9,8,5,9,7,4,4,12,8,3,11,6,7,10,5,12,5,12,7,10,9,10,9,12,11,9,7,11,8,7,4,8,3,3,6,7,7,6,12,3,6,7,8,10,11,5,12,8,4,4,11,10&reel_set17=6,7,4,11,8,12,9,8,6,9,10,9,8,7,12,3,10,5,4,3,7,6,7,5,8,6,8,3,10,7,10,12,6,6,6,6,9,11,7,11,7,6,11,12,8,4,12,10,9,4,8,5,10,11,12,5,9,12,6,6,10,7,9,11,5,8,6,6,12,8,11~4,6,12,7,8,3,6,5,8,7,5,8,7,4,8,11,10,9,3,7,7,12,13,7,7,7,7,11,4,6,9,7,8,4,7,6,10,4,9,10,6,12,3,11,7,5,12,10,9,11,12,5,11~3,7,6,5,3,9,8,9,6,8,3,8,9,10,11,4,12,6,5,3,12,5,13,3,7,10,11,5,8,3,8,6,7,11,10,10,10,10,12,6,10,11,10,11,11,5,13,9,13,5,6,3,9,12,7,10,10,12,7,4,5,9,9,8,4,6,7,4,8,11,5~6,6,8,6,12,10,10,8,9,4,3,12,3,5,5,5,3,9,11,7,9,4,9,11,5,11,9,11,7,11,7,7,7,6,5,12,8,10,12,5,7,8,10,12,5,7,4,4,8~4,3,7,6,8,11,12,4,4,12,5,11,8,3,4,5,8,7,10,12,5,6,3,12,5,10,11,6,10,6,9,3,9,4,7,7,7,12,4,8,9,3,10,3,11,4,11,8,7,5,9,11,7,8,12,9,5,7,12,7,5,11,8,7,7,11,9,10,7,6,3,10,8,10,7&total_bet_max=30,000,000.00&reel_set0=12,7,3,5,3,12,5,4,9,7,12,10,6,10,1,10,7,11,8,11,1,9,12,5,9,8,7,10,5,10,7,9,11,4,7,11,9,6,11,6,8,7,8,10,8,5,6,9,10,9,4,11,12,6,2,6,9~5,4,5,11,10,2,7,9,8,2,5,7,6,7,9,7,12,8,3,6,10,6,11,6,3,4,5,9,8,12,3,12,4,8,11,10,4,8~5,12,10,6,9,8,7,4,7,12,10,1,8,12,5,9,8,10,11,5,9,4,3,2,6,11,7,8,7,12,6,9,11~11,2,6,5,12,4,7,8,12,9,12,3,10,7,9,5,6,8,6,7,11,4,7,4,6,5,3,12,10~5,7,8,10,6,4,1,6,7,12,10,8,5,4,10,6,4,12,2,1,6,11,7,6,9,3,11,6,7,4,8,7,9,8,2,5,12,7,12,7,4,12,11,4,8,7,12,8,5,7,5,9,7,12,9,8,7,11,6&reel_set2=6,7,6,4,5,4,8,4,3,5,8,5,3,12,5,8,9,3,4,8,9,7,11,10,9,6,8,12,3,5,10,7,6,3,5,6,5,10,10,10,13,7,7,11,4,11,10,10,4,8,10,7,6,12,12,6,9,6,3,11,5,3,4,11,6,5,12,7,3,7,12,8,5,7,7,8,10,4,12,11,12~8,7,3,12,11,13,7,6,3,8,12,5,10,4,9,4,7,3,10,6,12,6,5,3,12,8,11,9,12,9,9,11,6,9,4,4,4,7,5,4,6,7,3,5,7,5,10,9,10,11,4,7,4,5,6,4,5,3,10,4,3,7,5,11,12,4,3,7,6,12,8,12,13,8~11,5,4,4,6,5,4,7,8,11,3,12,5,7,6,6,6,12,7,10,10,7,4,8,12,11,12,10,4,10,12,7,7,3,3,3,4,11,6,6,7,11,5,11,8,12,3,12,3,8,9,9,6,12,12,12,6,9,11,9,5,12,8,7,5,8,3,9,3,7,3,3,9,9,6,12,6,9,3,5,9,7,8,13,11,6,10,6,12,9,5~3,10,9,5,11,10,8,8,10,12,12,8,9,6,5,4,11,4,5,3,5,6,7,6,6,4,7,4,6,13,4,11,3,11,4,9,9,10,10,7,6,3,4,3,12,11,6,9,7,11,10,7,9,5,7,8,11,4,3,10,12,12,10,13,10,5,3,9,7,4,6,8,12,4,4,12,8,5,6,5~4,5,7,10,9,12,7,11,7,6,5,7,12,11,9,6,11,5,8,10,6,4,8,12,12,12,12,3,9,3,7,3,4,9,5,4,7,11,3,8,7,10,6,10,3,12,12,8,11,13,12&reel_set1=10,6,3,8,7,8,5,11,8,10,11,10,12,8,2,3,8,11,9,7,5,9,4,7,3,12,4,6,5,7,5,4,7,9,4,6~3,2,5,8,6,3,4,3,11,8,11,10,9,6,7,12,11,2,4,6,4,10,9,7,5,9,7,4,12,4,8,12,8,7,6,9,5,12~4,11,12,8,9,4,12,9,6,7,9,10,5,9,12,4,10,8,7,3,8,11,6,4,6,12,5,12,5,7,5,3,5,12,9,12,3,11,12,7,6,5,4,5,4,8,4,8,3,8,7,13,3,10,7,6,3,7,2~9,6,8,11,3,6,9,8,5,12,10,5,12,7,12,7,11,10,11,3,8,4,7,10,6,8,11,5,12,7,8,4,12,4,9,12,11,8,5,6,8,6,7,2,4,10~10,7,11,6,12,8,6,4,10,5,12,7,5,8,11,7,9,12,8,4,2,3,10,5,9,4,12,3,13,3,11,7,5,7,6,8,5,11,12,7&reel_set4=4,7,12,4,4,4,12,8,10,7,4,12,12,12,7,12,10,5,10,8,8,8,5,12,4,5,8,8~3,8,9,3,11,11,11,6,11,11,8,9,9,9,8,6,3,6,3,8,8,8,6,11,9,9,11~12,7,12,5,4,12,12,12,12,9,12,9,4,10,12,4,10,10,10,5,10,7,10,9,10,7~5,4,9,12,6,7,9,12,6,8,10,7,4,8,5,10,7,11,3,11~12,9,7,8,8,3,11,12,12,12,7,6,12,11,10,5,3,4&purInit=[{bet:1500,type:\"default\"},{bet:3000,type:\"default\"},{bet:750,type:\"default\"},{bet:3000,type:\"default\"}]&reel_set3=10,6,1,5,11,8,3,9,3,12,7,4,9,8,9,12,11,6,8,6,6,6,6,8,9,10,9,5,2,4,12,5,5,7,6,3,2,4,5,11,4,3,9,4,10,10,10,4,8,3,9,8,12,8,3,1,7,8,2,7,11,10,7,6,2,5,7,4,4,4,10,12,11,1,6,8,7,3,10,7,4,9,4,7,6,10,5,6,6,10,6~11,3,9,2,6,6,12,6,6,6,6,2,5,13,9,10,3,6,4,7,7,7,7,8,12,6,10,8,11,11,12,3,4,4,4,7,10,4,7,4,5,8,9,3,3,3,5,9,7,7,5,10,4,7,10,10,10,11,6,11,7,5,8,6,4,9,10~9,5,7,9,3,10,4,8,6,1,10,7,7,7,7,7,6,7,5,6,2,5,6,8,6,9,4,5,4,4,4,9,5,8,2,9,4,4,11,4,3,5,7,7,8,8,8,8,10,4,12,8,7,1,11,5,6,10,8,3,6,6,6,6,7,8,1,11,6,8,7,2,11,4,8,7,9,9,9,3,5,10,8,12,7,7,9,12,11,7,6~6,6,5,5,11,6,7,7,7,7,10,12,4,9,8,5,12,11,11,11,4,8,3,9,11,7,12,5,3,3,3,12,10,8,4,8,9,6,5,9,9,9,8,7,11,3,3,6,9,11,6,6,6,6,6,2,7,7,12,2,8,10,7,11~11,5,4,8,5,6,7,12,12,12,8,3,10,7,10,9,11,1,7,6,6,6,4,10,5,12,4,1,12,9,9,9,6,5,3,6,9,8,4,5,9,5,5,5,7,11,7,9,5,10,2,8,11,3&reel_set6=12,6,7,5,8,10,2,11,11,2,10,5,9,7,8,3,7,4,7,11,3,9,6,5,4,9,8,12,5,12,10,7,8,7,11,11,10,11,10,12,12,11,9,4,8,9,4,6,12,6,12,8,8,5,12,10,5,3,7,6,8,8~11,9,10,10,12,6,11,8,4,5,8,9,12,7,2,9,5,8,7,6,3,4,5,7,10,9,10,4,6,7,8,11,5,6,3,8,12,8,8,5,3,10~10,6,7,8,7,9,12,11,2,4,5,7,9,11,10,4,8,5,12,10,5,12,7,10,3,5,7,6,11,10,10,5,11,6,8,11,5,11,9,9,6,3,8,7,4,7,4,11,8,12,8,9,7,6,3,12,5,4~2,6,8,5,9,11,4,8,6,7,3,8,8,3,10,9,5,10,4,6,5,8,11,4,12,8,5,11,10,12,10,12,7,9,6,7,8,11,9,4,5~4,9,12,10,2,4,9,8,12,7,5,11,8,10,8,7,6,6,12,3,12,3,7,8,10,11,10,5,6,6,6,5,7,8,4,10,7,11,6,8,6,7,5,8,3,4,6,9,5,9,3,6,9,10,6,8,5,11,2,12,9&reel_set5=10,9,10,7,12,9,4,10,5,12,12,12,12,7,5,9,9,4,12,4,4,12,10,10,10,4,5,12,9,7,12,12,9,1,10,7,10~7,10,4,4,4,10,8,5,12,12,12,7,8,12,12,8,8,8,4,5,8,4,12~8,6,6,3,3,6,11,11,11,8,11,8,3,3,11,9,9,9,6,6,9,8,11,9,9,3,8,8,8,11,9,6,1,11,9,3,9~6,7,6,5,4,5,7,8,12,10,9,4,3,9,8,12,11,7,11,10~7,3,6,12,5,11,12,11,3,8,12,3,7,12,12,12,11,6,1,7,8,10,5,9,8,11,7,8,4,9,7&reel_set8=7,6,12,7,2,7,6,8,4,8,9,4,7,3,11,10,9,6,10,5,6,6,6,9,8,6,12,6,10,11,12,5,8,12,11,5,11,7,9,11,8,4,7,12,3,6,9~7,4,9,7,8,6,6,7,4,5,11,6,7,11,10,5,9,10,9,4,9,11,3,10,9,8,2,9,7,4,4,3,8,12,4,7,7,7,7,6,12,7,10,5,4,7,8,6,6,5,11,5,3,6,12,7,8,11,7,10,12,11,12,10,7,11,5,8,5,12,3,12,8,7~9,3,9,5,7,5,10,11,5,4,8,11,6,10,10,9,3,12,11,10,10,10,10,6,7,8,5,4,3,6,7,3,11,10,4,6,12,8,9,4,5,12,13,7~4,9,10,4,5,7,6,4,10,7,8,9,12,8,10,12,5,5,5,11,8,7,9,10,3,9,8,12,11,6,5,7,4,6,11,12,7,7,7,12,5,5,6,5,12,11,11,8,3,4,10,8,11,9,7,9,3~9,7,7,9,3,11,12,13,9,4,12,10,4,7,6,5,5,7,12,6,5,11,8,11,10,7,7,7,11,10,8,6,7,5,8,4,3,4,8,3,4,7,8,13,5,10,3,12,11,9,6,10,3,9,2,12&reel_set7=5,11,6,12,5,10,7,5,3,4,5,2,12,7,4,9,3,8,9,7,10,11,9,4,10,7,9,8,12,12,12,11,4,5,9,6,8,6,8,9,12,8,3,5,10,8,11,9,4,8,7,7,11,12,12,7,4,11,11,10,12,9~11,4,9,9,5,6,12,8,3,4,6,8,9,3,7,9,7,10,3,10,5,11,8,12,6,7,5,4,12,11,9,12,5,4,6,11,7,4,8,11,8,10,9,10,12,6,2,12,7,8,4,8,11,8,10,9,8,3,12,10,8,5,7,6,5,10,13,7,11~10,7,7,8,10,7,9,12,8,11,8,7,10,4,8,7,11,8,4,5,8,6,5,4,12,9,12,5,10,5,11,10,3,12,9,11,11,6,7,3,6,6,12,6,4,7,9,11,9,5,10,12,8,3,7,3,10,8,12,12,4,5,6,4,3,7,10,9,11,12,10,6~12,6,10,9,6,8,4,9,10,4,7,5,11,3,7,11,12,11,7,7,10,12,3,7,11,8,5,9,4,5,8,8,5,10,12,8,6,10~9,11,7,10,4,12,5,8,11,6,12,2,12,9,5,6,6,11,3,7,5,10,9,7,3,7,8,11,5,4,7,8,9,12,5,11,7,9,3,12,9,4,10,12,11,4,8,6,10,4,8,7,10,8,6&reel_set9=10,10,9,11,4,5,11,9,12,8,4,12,5,7,9,5,12,12,12,7,12,11,7,5,7,5,3,9,9,12,11,8,12,11,12,7,9,9,9,6,7,2,8,6,11,10,4,12,11,3,6,10,6,10,10,11,10,10,10,7,9,3,8,4,6,10,3,10,4,7,8,12,11,7,10,7,7,9~7,11,5,9,4,10,7,4,8,11,5,10,11,9,9,9,7,7,8,6,3,9,10,6,7,8,12,4,6,5,11,11,11,11,3,7,10,8,5,9,5,11,9,6,8,11,11,6,11,12,12,12,12,9,3,5,8,6,12,12,10,8,3,10,7,4,10,10,10,11,6,8,7,12,12,11,9,9,11,6,12,9,11,12,12~11,7,12,12,7,11,8,9,8,3,6,8,11,10,6,5,10,6,8,10,12,7,4,7,11,12,8,12,6,9,3,2,8,12,10,4,10,7,4,9,6,11,7,8,12,3,5,9,5,10,4,7,5,3,4,9~2,9,4,12,5,3,4,12,10,6,12,11,7,12,8,9,6,3,4,10,11,13,9,9,9,3,7,10,7,5,5,10,7,11,8,3,4,9,7,6,12,7,13,6,5,10,8,9,10,7,7,7,3,5,8,6,10,7,12,10,4,8,12,8,7,11,9,11,8,13,5,9,11,12,8,11~6,8,7,8,7,5,4,10,5,7,4,3,9,9,7,3,12,11,10,5,4,7,8,7,8,11,8,10,3,8,11,4,3,6,12,9,10,10,10,6,11,10,6,11,9,12,7,4,8,11,7,10,3,12,7,6,11,2,7,10,12,9,9,5,7,10,6,12,5,8,7,5,11,10&total_bet_min=20.00";
            }
        }
	
        protected override double PurchaseFreeMultiple
        {
            get { return 150; }
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
            get { return 4; }
        }
        public double[] PurchaseMultiples
        {
            get { return new double[] { 150, 300, 75, 300 }; }
        }
        #endregion
        public ZeusVsHadesGodsOfWarGameLogic()
        {
            _gameID = GAMEID.ZeusVsHadesGodsOfWar;
            GameName = "ZeusVsHadesGodsOfWar";
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
        }	
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


                ZeusVsHadesGodsOfWarBetInfo betInfo = new ZeusVsHadesGodsOfWarBetInfo();
                betInfo.BetPerLine                  = (float)message.Pop();
                betInfo.LineCount                   = (int)message.Pop();		
                if (message.DataNum >= 4)
                {
                    betInfo.PurchaseFree = true;
                    betInfo.PurchaseType = (int)message.GetData(2);
                    betInfo.BetType      = (int)message.GetData(3);
                }
                else
                {
                    betInfo.PurchaseFree = false;
                    betInfo.BetType      = (int)message.GetData(2);
                }
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in ZeusVsHadesGodsOfWarGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                    oldBetInfo.BetPerLine                                       = betInfo.BetPerLine;
                    oldBetInfo.LineCount                                        = betInfo.LineCount;
                    oldBetInfo.PurchaseFree                                     = betInfo.PurchaseFree;
                    (oldBetInfo as ZeusVsHadesGodsOfWarBetInfo).BetType         = betInfo.BetType;
                    (oldBetInfo as ZeusVsHadesGodsOfWarBetInfo).PurchaseType    = betInfo.PurchaseType;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in ZeusVsHadesGodsOfWarGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBet     = (double)infoDocument["defaultbet"];
                _normalMaxID            = (int)infoDocument["normalmaxid1"];
                _emptySpinCount         = (int)infoDocument["emptycount1"];
                _naturalSpinCount       = (int)infoDocument["normalselectcount1"];
                _normalMaxID2           = (int)infoDocument["normalmaxid2"];
                _emptySpinCount2        = (int)infoDocument["emptycount2"];
                _naturalSpinCount2      = (int)infoDocument["normalselectcount2"];
                _anteStartID            = (int)infoDocument["antestartid"];

                _multiTotalFreeSpinWinRates = new double[FreePurCount];
                _multiMinFreeSpinWinRates       = new double[FreePurCount];

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
            int purchaseType = (betInfo as ZeusVsHadesGodsOfWarBetInfo).PurchaseType;
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
                int purchaseType = (betInfo as ZeusVsHadesGodsOfWarBetInfo).PurchaseType;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, -1, PurchaseMultiples[purchaseType] * 0.2, PurchaseMultiples[purchaseType] * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int purchaseType = (betInfo as ZeusVsHadesGodsOfWarBetInfo).PurchaseType;
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            return this.PurchaseMultiples[(betInfo as ZeusVsHadesGodsOfWarBetInfo).PurchaseType];
        }
        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = (betInfo as ZeusVsHadesGodsOfWarBetInfo).PurchaseType.ToString();
        }
        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            ZeusVsHadesGodsOfWarBetInfo zeusBetInfo = betInfo as ZeusVsHadesGodsOfWarBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, -1, betInfo.PurchaseFree ? zeusBetInfo.PurchaseType : -1, betMoney);
        }

        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            ZeusVsHadesGodsOfWarBetInfo betInfo = new ZeusVsHadesGodsOfWarBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new ZeusVsHadesGodsOfWarBetInfo();
        }
        protected override async Task<BasePPSlotSpinData> selectEmptySpin(int companyID, BasePPSlotBetInfo betInfo, bool isAffiliate)
        {
            int betType = (betInfo as ZeusVsHadesGodsOfWarBetInfo).BetType;
            int id = 0;           
            if (betType == 0)
                id = Pcg.Default.Next(1, _emptySpinCount + 1);
            else
                id = Pcg.Default.Next(_anteStartID, _anteStartID + _emptySpinCount2);
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, id), TimeSpan.FromSeconds(10.0));
            return convertBsonToSpinData(spinDataDocument);
        }
        protected override async Task<OddAndIDData> selectRandomOddAndID(int websiteID, BasePPSlotBetInfo betInfo, bool isMoreBet, bool isAffiliate)
        {
            double  payoutRate   = getPayoutRate(websiteID, isAffiliate);
            double  randomDouble = Pcg.Default.NextDouble(0.0, 100.0);
            int     betType      = (betInfo as ZeusVsHadesGodsOfWarBetInfo).BetType; ;
            int     selectedID   = 0;
            int     affliateCnt  = 0;

            if (randomDouble >= payoutRate || payoutRate == 0.0)
            {
                if (betType == 0)
                    selectedID = Pcg.Default.Next(1, _emptySpinCount + 1);
                else
                    selectedID = Pcg.Default.Next(_anteStartID, _anteStartID + _emptySpinCount2);

            }
            else if (betType == 1)
            {
                if(!isAffiliate)
                    selectedID = Pcg.Default.Next(_anteStartID, _anteStartID + _naturalSpinCount2);
                else
                {
                    affliateCnt = _naturalSpinCount2;
                    selectedID = Pcg.Default.Next(_anteStartID, _anteStartID + _naturalSpinCount2);
                }
            }
            else
            {
                if(!isAffiliate)
                    selectedID = Pcg.Default.Next(1, _naturalSpinCount + 1);
                else
                {
                    affliateCnt = _naturalSpinCount;
                    selectedID = Pcg.Default.Next(1, _naturalSpinCount + 1);
                }
            }

            OddAndIDData selectedOddAndID = new OddAndIDData();
            selectedOddAndID.ID = selectedID;
            return selectedOddAndID;
        }

        protected virtual async Task<BasePPSlotSpinData> selectRangeSpinDataByBetType(int websiteID, double minOdd, double maxOdd, int betType, BasePPSlotBetInfo betInfo)
        {
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                    new SelectSpinTypeOddRangeRequestWithBetType(GameName, -1, minOdd, maxOdd, -1, betType), TimeSpan.FromSeconds(10.0));

            if (spinDataDocument == null)
                return null;

            return convertBsonToSpinData(spinDataDocument);
        }
        public override async Task<BasePPSlotSpinData> selectRandomStop(int websiteID, UserBonus userBonus, double baseBet, bool isChangedLineCount, BasePPSlotBetInfo betInfo, bool isAffiliate)
        {
            if (this.SupportPurchaseFree && betInfo.PurchaseFree)
                return await selectPurchaseFreeSpin(websiteID, betInfo, baseBet, userBonus, isAffiliate);

            if (userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus rangeOddBonus = userBonus as UserRangeOddEventBonus;
                if (baseBet.LE(rangeOddBonus.MaxBet, _epsilion))
                {
                    int betType = (betInfo as ZeusVsHadesGodsOfWarBetInfo).BetType;
                    BasePPSlotSpinData spinDataEvent = await selectRangeSpinDataByBetType(websiteID, rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd, betType, betInfo);
                    if (spinDataEvent != null)
                    {
                        spinDataEvent.IsEvent = true;
                        return spinDataEvent;
                    }
                }
            }
            if (SupportMoreBet && betInfo.MoreBet)
                return await selectRandomStop(websiteID, betInfo, true, isAffiliate);
            else
                return await selectRandomStop(websiteID, betInfo, false, isAffiliate);
        }
        protected override bool addSpinResultToHistory(string strGlobalUserID, int index, int counter, string strSpinResult, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {
            if (!_dicUserHistory.ContainsKey(strGlobalUserID))
                return false;

            BasePPHistoryItem historyItem = new BasePPHistoryItem();

            int betType     = (betInfo as ZeusVsHadesGodsOfWarBetInfo).BetType;
            historyItem.cr  = string.Format("symbol={0}&c={1}&repeat=0&action=doSpin&index={2}&counter={3}&l={4}&ind={5}", SymbolName, betInfo.BetPerLine, index, counter, ClientReqLineCount, betType);
            if (SupportPurchaseFree && betInfo.PurchaseFree)
            {
                int purchaseType = (betInfo as ZeusVsHadesGodsOfWarBetInfo).PurchaseType;
                historyItem.cr += string.Format("&pur={0}", purchaseType.ToString());
            }
            if (SupportMoreBet)
            {
                if (betInfo.MoreBet)
                    historyItem.cr += "&bl=1";
                else
                    historyItem.cr += "&bl=0";
            }
            historyItem.sr = strSpinResult;

            _dicUserHistory[strGlobalUserID].log.Add(historyItem);
            if (betInfo.HasRemainResponse)
                return false;

            _dicUserHistory[strGlobalUserID].baseBet = betInfo.TotalBet;
            _dicUserHistory[strGlobalUserID].win = spinResult.TotalWin;

            //빈스핀인 경우이다.
            if (spinResult.NextAction == ActionTypes.DOSPIN)
                return true;

            return false;
        }
    }
}
