using Akka.Actor;
using Akka.Event;
using GITProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class MammothGoldMegaMegaResult : BasePPSlotSpinResult
    {
        public int DoBonusCounter { get; set; }

        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            this.DoBonusCounter = reader.ReadInt32();
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(this.DoBonusCounter);
        }
    }
    class MammothGoldMegaGameLogic : BaseSelFreePPSlotGame
    {

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20mammoth";
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
            get
            {
                return 20;
            }
        }
        protected override int ServerResLineCount
        {
            get { return 20; }
        }
        protected override int ROWS
        {
            get
            {
                return 8;
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 100.0; }
        }
        protected override bool HasPurEnableOption
        {
            get { return true; }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=13,10,4,4,5,13,4,9,11,12,5,12,6,9,3,7,7,8,4,9,11,4,5,10,6,5,3,6,7,12,10,9,13,4,5,10,13,9,13,13,13,12,13,13,13,13,13,11&cfgs=7006&ver=3&def_sb=5,7,8,5,10,6&reel_set_size=22&def_sa=5,8,10,3,6,6&scatters=1~0,0,0,0,0,0~0,0,0,0,0,0~1,1,1,1,1,1&rt=d&gameInfo={props:{gamble_lvl_6:\"80.64%\",gamble_lvl_7:\"82.84%\",gamble_lvl_4:\"75.26%\",gamble_lvl_5:\"78.18%\",max_rnd_sim:\"1\",gamble_lvl_8:\"84.68%\",gamble_lvl_9:\"86.28%\",max_rnd_hr:\"24390200\",max_rnd_win:\"10000\",gamble_lvl_2:\"68.33%\",gamble_lvl_3:\"72.02%\",gamble_lvl_1:\"64.52%\"}}&wl_i=tbm~10000&reel_set10=12,5,10,12,12,11,9,10,9,12,11,12,9,1,11,7,6,12,12,11,7,8,8,5,1,12,12,8,12,9,11,12,12,7,10,12,10,11,4,10,6,10,7,8,11~5,6,11,8,6,10,12,4,5,12,10,7,9,12,10,1,12,3,4,10,9,11,12,3,9,8,12,5,12,5,7,12,10,1,3,5,12,11,6,12,4,12,9,6,10,12,6,5,7,10~8,3,6,1,12,11,10,7,10,5,4,10,12,7,12,9,7,12,4,10,12,7,10,1,3,5,3,12,11,12,11,12,6,10,12,6,5,12,10,9,8,5,10,12,11,5,7,12,12,11,4,12,8~1,9,12,5,3,3,10,5,7,11,7,11,5,11,5,4,4,11,4,8,10,4,12,6,3,5,11,8,11,12,11,11,5,12,12,12,4,11,4,8,4,7,12,5,10,11,4,6,10,8,6,6,8,7,12,8,5,8,8,9,5,6,9,4,11,4,3,4,10,10,5,11,11,11,5,4,9,5,3,12,8,5,4,8,11,4,11,4,8,5,3,7,12,11,4,9,8,6,11,10,7,12,7,12,12,11,8,8,3,3,3,3,3,3,3,12,4,4,12,8,3,4,5,7,12,12,11,8,4,3,10,3,8,4,4,1,9,11,7,9,5,8,7,11,8,6,4,10,9,11~8,12,11,8,8,3,12,4,6,5,12,10,10,5,12,6,12,11,5,8,12,10,6,6,7,9,8,6,8,7,3,10,12,8,7,1,8,5,8,6,12,8,8,10,8,4,4,6,11,10,3,3,3,3,3,3,1,12,8,4,12,12,8,5,4,10,3,11,4,6,7,12,12,11,4,11,3,11,11,4,6,4,10,9,12,7,8,4,6,7,9,5,5,9,5,10,4,4,7,8,11,7,6,9,6,4,6,5,7,3~3,7,5,5,12,12,11,3,10,4,5,8,12,5,6,12,10,6,11,9,11,6,4,6,5,10,4,12,8,6,12,12,12,7,11,4,6,12,10,11,10,1,9,5,7,1,12,10,4,9,12,10,8,11,10,7,7,3,12,4,10,6,4,12,10,3,3,3,3,3,9,7,4,10,12,8,6,12,12,4,11,8,11,12,3,4,7,8,12,10,4,4,7,4,9,7,7,6,11,6,11,6&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&reel_set11=9,11,12,12,3,4,3,8,11,2,12,12,5,10,2,10,12,9,8,9&reel_set12=12,10,5,10,6,10,4,11,4,9,9,11,4,11,9,7,8,9,9,4,7,9,8,11,11,11,3,10,6,9,12,6,9,8,7,8,7,5,10,9,9,8,9,8,9,9,3,8,9,9,9,9,9,9,9,11,9,12,3,8,11,11,6,5,1,11,9,12,8,11,10,4,9,5,10,6,5,10,10,7,11~12,4,5,8,10,6,8,7,12,10,4,7,11,7,7,6,11,10,9,8,10,8,3,9,7,5,10,9,8,11,10,12,8,5,11,11,11,11,11,11,3,8,6,9,6,8,11,9,11,10,11,4,8,9,10,11,1,12,6,5,9,4,5,10,10,11,11,4,7,10,6,11,9,11,3,6,12,12,12,12,12,9,7,9,10,1,12,11,12,9,11,8,10,9,12,8,9,11,12,5,11,11,8,4,5,9,11,8,4,5,9,7,12,4,10,3,10,8~10,11,8,6,9,8,10,10,11,12,9,10,11,3,7,10,6,7,10,5,6,5,9,11,10,9,5,9,10,8,11,5,11,4,4,10,10,12,9,10,10,10,10,10,10,10,11,4,10,7,9,8,8,10,6,6,10,6,10,7,10,5,8,11,4,9,4,9,7,11,9,8,4,5,3,12,10,12,10,8,8,9,8,10,10,1,7,11~6,5,5,11,10,8,12,4,8,4,10,5,4,12,9,10,10,9,4,12,4,10,12,4,12,12,8,4,9,11,8,9,12,9,12,6,7,8,8,10,5,12,10,10,11,10,6,12,1,11,4,11,7,8,3,5,4,11,10,7,11,8,11,11,12,12,12,9,4,4,11,11,8,9,12,12,7,5,5,11,11,6,6,9,10,9,5,12,6,4,7,4,9,11,6,9,3,5,12,10,6,4,4,9,8,9,11,10,12,9,8,7,4,6,4,9,11,4,5,12,1,6,7,4,9,11,12,6,6,3,9,7,8~4,6,12,4,4,11,7,7,4,11,11,12,6,5,12,12,9,4,12,6,7,7,9,4,4,5,8,6,10,10,9,6,9,11,10,7,10,9,10,7,9,7,10,12,7,4,10,11,11,7,3,7,6,4,4,5,11,6,9,9,5,8,7,7,9,7,9,4,5,6,8,4,12,5,10,7,6,10,8,1,6,4,11,11,8,10,5,12,10,11,11,9,9,11,7,5,8,10,10,5,8,8,7,9,10,5,9,12,9,11,5,6,8,6,10,11,6,12,12,3,4,8,12,4,3,1,12,12,6,12,7,1,6,7,11,6,8,8,5,10,5,11,12~6,12,9,10,6,6,3,3,6,11,6,10,9,6,11,7,7,3,11,4,6,4,9,12,12,1,9,11,10,4,5,8,4,8,5,9,9,5,7,8,12,11,10,8,9,10,5,11,11,10,5,12,5,9,11,4,5,10,11,11,11,4,4,1,4,11,8,6,5,9,4,10,7,10,10,8,4,12,10,6,7,9,11,10,12,12,11,11,4,11,10,4,7,5,8,6,4,6,5,4,12,7,6,4,12,6,8,7,8,3,11,12,9,11,9,10,8,7,9,8,8,12,12,12,11,7,12,4,12,9,4,12,11,12,8,10,11,4,9,4,12,5,7,11,4,11,9,9,6,12,5,8,9,8,12,10,9,10,12,12,8,12,6,11,9,10,11,6,5,9,9,11,10,9,12,11,4,4,12,4,12,6,9,4,8&purInit_e=1&reel_set13=11,9,12,12,7,8,4,8,4,6,11,10,9,9,6,8,7,8,2,12,5,11,4,10,10,9,12,10,9,11,5,8,8,7,12,2,12,2,6,10,5,11,3,11,9,10&wilds=2~0,0,0,0,0,0~1,1,1,1,1,1&bonuses=0&reel_set18=10,9,10,11,12,4,11,10,4,5,12,11,4,11,9,10,9,12,9,10,12,11,5~12,10,3,8,11,4,5,11,8,10,6,4,5,11,4,5,11,4,10,5,12,11,5,10,5,10,4,9,10,11,8,9,11,10,7,10,10,3,11~10,11,4,10,9,5,4,10,10,4,8,10,6,11,4,5,10,11,9,11,10,12,11,8,9,9,4,11,9,6,11,5,10,11,12,11,5,9,4,3,4,11,8,11,10,10,3,4~7,8,12,3,7,11,7,7,11,4,3,7,7,6,11,3,6,6,8,7,7,11,6,7,8,11,8,7,7,6,8,7,6,7,7,12,4,12,7,8,12,3,12,6,3,7,11,7,11,7,3,7,8,3,6,11,8,4,6,8,12,4,6,7,3,3,6,12~7,8,5,12,7,5,8,7,12,7,6,3,10,5,10,3,5,7,8,7,7,6,10,3,11,8,3,5,8,6,7,8,8,5,5,10,7,5,11,8,7,8,7,8,3,5,5,6,5,6,3,8,5,8,7,5,5,7,8,3,3,8,10,8,7,12,3~12,8,7,10,12,3,10,12,9,6,3,9,3,6,7,5,6,7,10,9,6,5,7,5,9,3,7,5,7,4,12,7,12,6,8,7,5,8,7,6,9,7,10,3,6,4,5&reel_set19=12,10,9,10,3,8,12,2,8,2,9,12,12,7,4,12,7,9,11,12,3,12,9,10,12,2,4,12,9,12,12,11,4,7,3,4,10,11,9,12,11,4,12,10,12&reel_set14=12,11,10,9,7,9,12,6,11,10,6,11,7,10,11,9,11,9,10,10,11,12,7~12,9,11,6,10,7,11,7,6,10,3,11,6,8,12,8,10,5,10,11,6,10,6,10,8,7,4,10,11,7,11,12,9,10,3,11,9,11,10,10,11,7,11,10,6,11,9,7,8,6,10~10,6,10,12,9,10,3,10,10,11,7,5,11,7,9,7,5,6,11,6,9,11,8,10,10,12,7,6,7,3,8,11,10,10,11,7,11,7,10,9,11,7,8~8,3,11,4,4,12,4,4,7,11,4,5,11,3,3,4,4,5,5,3,4,4,12,11,5,5,3,8,5,12,11,3,4,11,8,8,7,12,11,12,8,4,5,4,3,7,3,4,11,4,5,8,7,8,3,4,5,4,4,12,4,5,8,4,8~8,6,3,3,10,6,8,4,8,3,8,4,4,8,3,12,5,8,11,6,4,12,4,4,5,6,8,8,3,8,4,6,8,6,12,3,11,6,3,4,4,10,5,6,5,6,10,4,6,4,8,8,5,6~3,12,5,4,3,4,3,9,12,6,4,9,5,3,8,10,9,5,12,7,9,4,8,4,8,10,7,5,4,9,12,5,5,10,4,9,4,10,6,5,12,3,6,3,6,4,6,7,5,4,10,6,3,12,4,6,5,6,4,8,10,4,12,4,7&paytable=0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;100,50,30,20,10,0;60,30,15,10,0,0;40,20,15,5,0,0;20,15,10,5,0,0;15,10,6,4,0,0;15,10,6,4,0,0;12,8,5,3,0,0;12,8,5,3,0,0;10,6,4,2,0,0;10,6,4,2,0,0;0,0,0,0,0,0&reel_set15=11,4,9,12,12,4,12,12,7,12,7,11,12,2,9,2,9,12,11,2,12,12,8,12,10,12,12,7,12,3,2,10,7,10,9,7,12,12,8,12,2,9,10,8,9,4,11,3,12,4,9,2,10,9,7,3,10&reel_set16=9,11,5,9,9,9,9,9,9,5,11,11,5,5,5,5,5,5,5,9,5,5,9,11,11,11,11,9,11,5,9,5,5~7,10,7,10,10,10,10,10,10,10,7,10,6,10,10,6,6,6,6,6,6,6,10,7,6,10,10,7,7,7,7,7,7,6,10,6,10,7,10,10~12,12,8,8,8,8,12,8,12,8,12,12,12,12,4,12,8,12,4~8,4,8,4,7,9,8,6,10,7,8,8,3,12,4,6,9,12,5,10,12,4,11,7,8,10,7,12,12,12,9,10,11,6,11,12,4,8,11,5,9,4,5,4,10,4,8,11,3,8,9,8,8,3,10,12,12,11,11,11,3,11,11,5,11,6,4,12,8,12,8,11,5,5,3,3,5,7,5,4,4,7,7,6,12,11,4,6,3,3,3,3,3,3,3,11,9,10,5,4,8,4,10,8,11,12,11,8,12,4,5,4,6,8,4,4,11,4,10,4,5,4,11,5~11,7,8,12,5,9,6,7,11,8,6,12,6,8,4,5,12,11,7,8,8,5,4,7,4,5,6,12,8,3,12,4,10,8,8,10,10,6,8,3,3,12,4,4,8,11,5,6,12,7,5,4,7,4,3,3,8,4,7,11,12,6,5,7,11,4,11,8,12,8,12,8,6,6,3,3,3,3,3,3,9,8,10,12,7,3,11,4,6,4,8,4,11,8,10,6,12,8,9,11,8,9,12,8,7,4,9,7,8,12,9,10,10,8,4,4,6,10,6,8,4,7,8,10,11,10,12,8,6,5,4,12,5,10,5,6,8,4,12,6,6,7,8,6,8,11,4,9,6,11,12,6,7,12,10,10~10,6,11,7,9,12,8,4,8,4,12,6,12,5,11,5,4,5,10,10,5,4,8,4,7,9,4,12,10,4,12,6,8,7,12,4,10,6,3,4,12,3,6,3,5,3,12,9,6,12,4,11,5,6,12,12,12,12,9,10,11,7,10,6,11,7,7,4,12,11,4,7,10,12,12,11,7,9,12,11,9,10,8,5,6,4,8,4,6,9,4,11,10,4,6,7,11,6,5,10,4,8,9,12,12,5,10,7,10,6,12,3,3,3,3,3,4,10,12,6,4,10,3,4,7,12,10,9,11,11,7,12,6,7,4,9,5,12,11,8,3,10,7,6,4,12,12,10,12,6,11,8,12,4,7,4,7,7,10,5,11,12,10,3,7,4,7,7,6,6,10,6,8,11&reel_set17=7,8,10,11,8,9,12,11,12,4,10,9,6,8,6,2,9,11,7,10,8,7,4,12,5,12,2,10,11,8,12,6,6,4,10,12,3,8,2,5,4,11,12,5,10,10,11,10,3,4,8,10,2,12,8,5,12,8,12,9,8,8,2,9,10,7,10,8,12,10,6,11,12,6&total_bet_max=10,000,000.00&reel_set21=10,2,6,8,9,11,3,5,9,11,8,9,8,3,11,7,2,5,12,5,10,3,8,12,12,10,12,3,11,9,12,5,10,11,6,11,10,12,9,10,2,8,7,8,12,10,12,9,2,9,6,8,4,12,8,5,8,11,10,11,4,2,8,9,11,12,3,8,9,9,4,3,8,10,11,2&reel_set0=12,10,4,6,4,6,6,6,6,6,6,6,12,6,6,10,10,12,10,10,10,10,10,10,6,10,6,4,12,6,12,12,12,12,10,6,10,12,6,12,10~9,5,9,9,7,9,5,9,9,9,9,9,9,9,7,7,9,9,5,9,9,5,5,5,5,5,5,5,9,9,7,7,9,9,7,7,7,7,7,7,9,5,9,9,7,5,9,5,5,9~11,3,8,8,8,8,11,8,11,11,8,11,11,11,11,3,11,8,3,11~12,8,3,12,3,12,9,7,8,3,6,10,4,9,8,12,3,12,3,11,6,3,8,9,12,11,9,9,7,7,11,6,11,9,5,6,11,11,11,3,6,12,11,5,12,11,8,4,8,3,10,8,6,1,8,8,11,3,3,10,3,7,7,8,7,9,3,12,5,8,3,10,8,8,3,5,4,12,12,12,10,6,4,3,10,12,3,9,3,9,12,3,12,11,3,5,5,10,7,8,6,4,10,8,12,9,12,12,3,7,3,7,11,9,4,6,6,4,4,4,4,4,4,4,7,6,3,11,8,6,11,4,8,12,6,12,8,8,3,3,5,12,11,10,6,11,10,3,3,12,11,3,8,12,8,6,1,4,5,8,5,5,6~5,8,7,3,5,5,8,8,6,12,11,9,4,10,8,8,9,5,6,6,3,8,3,5,11,3,11,12,5,11,5,11,3,8,11,3,8,6,10,5,9,12,5,6,1,12,10,3,9,3,6,6,8,8,4,5,7,8,12,6,5,3,8,7,12,8,8,12,11,4,6,7,8,3,4,4,4,4,4,4,5,5,12,5,8,7,3,4,7,10,7,9,11,9,8,9,3,11,3,11,9,11,3,9,4,12,12,7,3,9,5,11,5,7,12,7,9,11,5,8,7,9,12,11,8,11,5,3,10,8,1,8,7,10,8,5,5,12,9,3,6,9,8,8,12,7,11,3,3,10,8,4,7,11,3,4,8,8~11,3,9,6,11,7,7,10,11,9,10,6,7,10,7,9,3,12,11,9,5,5,9,11,4,3,4,9,5,3,5,11,5,7,8,11,10,6,11,7,8,11,10,7,3,11,11,11,11,6,6,3,10,3,3,12,8,5,7,6,3,7,11,8,9,10,9,8,11,8,7,3,7,6,3,9,9,12,5,5,7,3,9,4,12,6,8,11,3,12,9,12,3,3,4,4,4,4,4,11,4,7,8,11,6,9,12,10,5,12,11,7,5,7,3,3,5,11,3,1,11,1,12,11,12,9,10,12,3,5,11,12,4,7,8,5,3,11,9,9,5,4,11,12,5,9&reel_set2=10,7,12,7,12,8,5,12,4,12,12,5,7,11,7,12,1,9,5,12,9,9,12,12,3,10,12,12,12,12,12,12,12,7,11,4,9,3,6,9,3,12,9,12,5,4,12,7,10,12,10,7,10,9,5,5,7,7,9,7,5,5,5,5,5,5,5,4,5,12,10,12,11,9,7,4,12,9,3,10,5,9,7,12,7,5,9,10,12,7,9,8,7,8,9,9,9,9,9,9,5,10,6,9,7,4,7,4,7,5,7,7,5,4,11,9,9,12,4,12,1,9,7,3,10,12,9,7,7,7,7,7,7,10,9,5,9,1,7,7,12,11,9,12,3,6,6,9,11,12,8,9,3,8,12,11,9,12,7,3,3,3,3,3,3,3,4,12,8,9,9,12,7,6,5,9,9,8,6,9,12,9,1,9,9,4,12,6,9,9,5,5,12,5~6,6,1,3,10,6,11,4,6,7,4,5,11,11,6,11,11,11,11,11,11,11,11,12,10,8,4,1,11,4,8,10,3,10,3,10,7,6,6,6,6,6,6,6,10,4,10,3,3,10,11,3,4,6,5,11,1,12,11,7,10,10,10,10,10,10,10,11,10,11,6,10,6,11,6,6,11,6,10,10,9,10,7,7,7,7,7,7,4,4,12,11,5,8,11,3,3,5,5,4,10,10,11,4,4,4,4,4,11,10,6,7,9,12,12,6,10,11,10,10,4,7,5,9,12,3,3,3,3,3,3,3,10,8,4,3,4,10,7,12,9,5,12,5,6,3,11,11,5,5,5,10,11,6,9,7,4,6,10,4,6,10,12,9,7,10,12,6,4~8,3,8,3,11,7,9,4,10,8,5,5,11,7,10,9,7,9,12,12,9,10,11,5,8,7,5,12,7,8,11,10,1,4,8,8,8,8,9,11,8,8,7,11,10,1,6,3,10,6,9,5,10,3,8,10,11,4,10,9,7,6,12,5,4,7,11,4,5,11,8,7,8,3,3,3,3,3,5,5,12,9,8,3,5,10,4,7,3,9,6,10,4,8,5,8,3,11,8,6,11,5,1,4,4,12,11,5,10,4,11,5,10,12,8~7,11,4,9,8,11,8,5,7,10,7,11,11,12,9,3,10,6,12,12,3,9,10,8,4,3,12,8,4,8,4,8,5,11,4,8,8,3,11,12,12,12,4,3,4,3,9,10,9,4,5,4,12,4,12,8,12,6,4,9,6,7,12,8,8,12,10,8,5,6,5,5,8,12,7,1,8,4,11,4,6,10,11,11,11,8,4,5,6,4,4,5,11,11,10,10,11,11,6,4,7,6,8,7,5,12,8,1,7,4,3,7,12,5,3,5,11,5,4,12,8,4,12,4,3,3,3,3,3,3,3,5,11,5,9,12,1,5,3,5,11,1,4,10,11,8,3,4,6,11,4,5,7,11,8,12,9,8,9,11,10,7,9,4,4,5,11,11,7,4,12,4~8,5,4,7,8,7,10,4,3,7,10,6,10,12,8,6,8,9,12,10,4,4,8,4,3,11,5,12,8,12,12,7,12,4,10,11,12,4,10,7,1,8,6,6,12,8,6,8,11,6,8,4,5,4,5,4,6,5,3,6,11,5,3,3,3,3,3,3,7,11,12,3,8,7,8,11,12,4,3,12,11,10,11,12,10,5,8,12,6,12,8,6,7,9,4,6,6,4,7,8,9,8,9,11,6,12,11,7,10,10,6,8,4,5,6,6,4,12,12,7,11,10,5,8,8,4,8,1,3,1,5,9~10,8,11,4,12,11,10,12,12,4,12,6,3,4,1,3,10,3,8,7,7,5,10,9,8,11,4,9,4,12,12,12,12,6,12,10,7,6,4,12,10,10,3,12,10,9,11,10,8,7,11,5,4,12,10,7,1,4,12,6,10,4,7,11,3,3,3,3,3,12,6,7,6,9,12,5,12,4,7,6,11,11,7,9,4,7,8,4,11,5,4,8,11,12,10,9,12,6,5,5,6,6&reel_set1=9,12,11,3,10,9,10,12,10,5,6,11,10,8,10,12,5,10,2,9,8,4,3,11,8,12,12,11,11,9,8,3,11,7,10,10,8,10,11,2,12,12,9,8,8,3,8,9,11,9,3,6,9,5,2,9,4,5,8,7,6,12,2,12,6&reel_set4=9,11,11,1,9,9,6,5,10,9,7,11,8,11,8,9,10,9,11,9,10,11,8,11,4,9,10,8,4,6,3,9,3,8,11,11,11,9,12,7,11,9,11,6,5,7,9,10,9,9,10,7,1,8,10,6,10,7,9,5,9,4,12,9,4,9,11,5,6,10,4,9,3,9,9,9,9,9,9,9,8,10,6,6,9,11,8,5,8,10,11,5,9,6,12,4,9,8,9,7,7,4,12,7,11,8,3,9,8,8,10,12,10,5,12,5,9~1,9,9,7,8,8,11,8,3,10,7,7,9,11,10,4,9,9,11,8,11,11,8,10,10,8,11,11,11,11,11,11,10,3,12,10,5,7,7,6,9,12,9,5,11,11,12,9,4,11,11,12,5,11,6,5,9,4,5,12,12,12,12,12,4,11,5,8,12,10,4,10,12,8,9,6,6,8,6,10,10,7,8,8,11,9,11,3,4,10,9~8,11,6,12,12,1,6,9,10,11,10,10,5,8,3,9,10,6,8,12,8,10,6,12,5,11,9,7,5,11,10,6,10,10,9,8,4,10,11,3,10,10,10,10,10,10,10,9,10,7,6,10,11,4,5,10,9,9,10,4,5,7,10,11,7,8,8,7,4,9,5,10,8,10,10,4,11,9,11,7,9,12,10,9,8,10,11,4,8~11,10,12,4,1,4,5,5,11,6,4,9,11,6,4,6,12,4,9,12,1,8,12,11,8,6,10,7,12,4,10,5,3,11,8,6,11,10,11,4,6,9,12,12,8,9,8,5,12,11,3,7,8,11,4,10,9,12,10,6,5,12,6,9,12,9,4,3,12,4,9,10,10,9,9,6,4,8,9,12,4,6,8,5,12,12,12,7,11,9,5,8,10,10,12,9,11,4,4,5,3,10,8,4,3,11,8,6,10,7,11,9,6,4,12,9,5,9,12,4,11,11,8,7,11,12,4,6,4,11,6,5,8,10,7,8,7,10,11,10,5,8,11,12,9,4,6,11,12,7,4,6,10,11,7,12,3,10,7,9,7,12,12,9,10,4,4,8,11,12,11,5,9,9~4,11,10,7,3,12,7,7,4,8,12,5,6,5,6,7,11,5,7,7,10,7,4,6,10,5,7,11,4,9,5,7,8,4,12,6,6,9,12,9,12,4,6,6,11,6,8,12,1,5,10,6,10,9,4,9,9,11,12,8,6,8,11,4,9,11,4,11,12,10,11,10,5,12,8,6,11,8,5,12,10,10,6,12,11,10,9,6,9,3,11,10,8,9,10,6,5,4,1,9,7,11,12,9,8,8,7,7,5,9,7,7,9,10,12,12,11,7,4,10,4,3,5~10,5,6,10,11,5,3,9,4,9,6,5,9,8,12,9,12,11,11,6,4,9,8,10,9,8,1,6,11,4,9,8,10,9,9,11,11,11,12,11,7,8,4,12,12,11,10,5,4,5,10,7,5,1,6,4,7,8,10,11,9,9,8,4,12,4,7,3,12,7,11,11,1,12,12,12,5,4,6,10,10,11,3,12,7,10,4,12,5,9,8,12,4,8,6,11,4,7,6,12,12,11,9,11,12,9,8,11,4,10,4,6,6&purInit=[{bet:2000,type:\"default\"}]&reel_set3=2,3,10,12,6,7,9,8,12,5,10,11,9,9,12,8,11,12,11,11,9,9,6,10,11,4,10,12,3,10,8,2,8,9,5,9,8,12,10,9,2,4,11,8,2,12,7,5,11,2,8,11,10,8,12,3,8,5,8,2&reel_set20=12,12,4,12,7,7,7,9,9,6,3,10,11,4,9,10,9,10,5,10,6,5,5,11,3,9,5,9,9,12,5,7,7,12,4,3,3,3,5,9,7,4,8,5,6,12,10,4,9,10,7,6,10,5,12,9,12,10,9,12,8,7,7,11,10,7,11,9,8,5,5,9,4,12,7,5,12,12,7,10,10,5,11,7,6,7,11,7,9,1,5,8,7,6,5,12,9,12,9,8,10,5,12,7,7,11,9,4,4,11,12,7,9,9,12,10,6,10,6,9,12,4,3,9,7,5,12,9,7,6,3,8,9,9,9,9,3,12,9,12,11,7,9,7,10,8,9,12,4,4,9,9,12,7,7,8,7,7,12,12,4,12,12,12,12,9,10,4,12,1,9,5,1,9,1,7,9,3,11,9,11,8,7,4,5,9,10~8,10,6,4,12,10,4,5,12,1,12,11,11,11,4,7,7,11,1,11,10,12,10,6,10,12,11,9,9,6,10,12,12,3,10,11,6,6,8,10,10,10,10,5,6,4,6,12,11,9,11,11,6,10,6,10,10,4,4,10,10,10,5,3,8,6,10,11,11,6,6,12,12,9,7,9,9,8,8,10,5,11,7,11,10,4,10,8,4,9,6,6,11,11,12,11,12,6,4,6,10,10,3,11,8,11,4,9,3,6,3,12,7,4,6,4,10,7,4,4,4,6,5,5,5,4,10,7,4,10,9,6,6,3,11,11,10,10,3,6,7,10,4,7,11,1,9,4,10,10,1,5,3,3,3,9,6,9,7,5,11,5,10,12,5,3,10,4,11,7,5,10,10,10,5,11,10,4,3,10,6,11,10,5,6,11,10~4,10,9,11,9,11,4,11,11,6,11,12,11,10,5,7,5,3,10,8,5,7,8,6,6,10,7,10,12,7,8,4,12,8,11,4,8,8,3,7,4,4,7,8,11,5,12,11,5,6,12,3,12,5,4,11,6,8,7,8,8,8,9,10,11,5,3,5,8,10,8,5,5,3,11,11,10,8,7,5,11,4,12,5,9,5,7,10,9,11,8,12,8,8,5,5,9,9,10,4,12,7,9,10,12,4,3,10,10,5,11,4,4,11,9,5,9,11,12,3,8,4,7,3,4,5,4,8,4,8,7,8,12,10,8,3,10,9,9,1,9,6,6,7,12,1,10,11,10,8,10,6,10,5,8,10,9,5,1,9,5,4,5,11,11,4,9,6,8,10,10,8,5,5,9,5,10,7,7,10,7,7~4,8,4,11,4,12,11,11,4,5,11,8,10,11,3,4,7,11,12,10,7,11,4,3,6,4,10,12,5,8,8,12,6,4,11,12,11,12,4,9,4,7,8,11,12,8,4,10,12,11,12,7,1,11,12,7,5,8,4,8,11,8,6,9,5,9,11,7,5,10,10,8,9,4,7,3,8,12,4,11,4,11,10,5,8,12,9,7,1,3,4,6,9,5,4,3,7,6,3,4,4,5,10,4,4,11,5,10,12,5,7,4,8,8,11,10,4,4,8,8,7,12,4,12,5,5,8,8,12,9,9,5,9,6,5,11,12,7,3,4,11,11,5,12,5,11,11,12,4,9,4,5,6,6,1,7,6,5,4,12,5,8,1,4,3,10,5,12,12,12,4,1,9,8,4,5,9,6,11,10,10,8,11,3,3,3~3,3,3,12,12,4,10,11,8,9,8,12,4,5,10,7,6,7,8,8,6,8,5,5,11,12,7,8,7,3,5,7,11,8,4,4,3,4,8,8,4,4,10,1,6,8,6,6,4,12,5,9,7,10,12,4,12,7,6,10,10,4,6,5,1,1,12,11,10,4,12,11,12,10,6,12,5,12,11,4,12,7,9,6,6,9,8,6,7,8,10,7,8,10,8,6,12,6,4,12,11,6,7,9,8,8,3,4,6,5,12,12,5,7,12,11,11,5,7,12,4,8,12,11,7,7,8,3,8,11,3,6,9,8,4,10,4,11,7,4,6,8,10,8,9,12,5,1,9,11,6,12,4,11,4,4,8,11,6,12,8,8,10,8,4,10,6,5,5,7,7,11,8,11,5,4,6,1,12,9,10,3,8,6,6,8~4,12,8,10,6,8,11,5,1,7,7,9,7,6,10,9,9,8,10,7,11,11,4,10,12,11,12,4,10,11,12,10,7,12,11,1,5,12,9,6,10,8,1,4,8,4,12,11,4,10,8,4,12,11,10,4,12,4,4,6,4,10,11,4,12,7,12,4,9,8,4,9,6,7,10,5,4,1,5,8,4,5,10,7,4,6,3,5,5,7,4,7,6,4,11,4,10,4,11,3,3,12,10,6,7,6,9,11,5,12,9,5,12,10,7,9,12,8,4,11,10,9,12,11,7,12,12,7,5,4,6,9,11,11,10,12,6,10,7,4,6,8,7,9,11,3,4,7,5,11,8,7,1,12,6,10,4,7,6,11,12,12,3,10,6,11,12,7,5,12,4,10,6,12,11,10,12,10,6,6,12&reel_set6=3,11,5,12,11,3,12,10,3,6,9,10,5,11,4,6,8,10,5,10,9,8,7,4,11,6,8,9,11,7,10,7,6,9,11,9,8,8,7,10,6,11,8,4,12,6,4,12,9,8,10,1,11,3,5,4,6,10,3,7,5,3,8,12,3,10,9,5,12,9,4~8,7,11,7,4,7,11,8,6,8,12,4,10,5,12,9,7,3,10,4,10,5,5,11,11,7,4,10,6,4,10,9,9,6,12,9,9,9,4,8,11,4,4,12,12,6,8,11,11,9,7,5,12,9,9,12,3,8,5,9,9,12,3,7,5,10,4,12,10,1,11,10,4,7,12,12,12,12,12,11,11,3,12,12,5,11,8,3,10,7,11,5,10,3,12,10,8,11,9,3,12,5,3,11,7,11,10,9,10,3,8,11,10,12,5,1,8~9,12,8,3,12,5,10,10,4,7,10,10,5,7,9,8,5,3,6,9,9,10,12,12,9,4,11,11,12,8,5,9,12,3,10,4,10,9,10,10,6,10,10,5,12,12,10,10,10,10,10,10,4,6,10,12,3,8,10,7,9,12,11,8,7,10,10,6,9,11,1,5,11,8,5,11,11,7,10,7,5,9,8,4,9,8,10,6,6,11,8,8,7,6,12,10,9,11,7,11,10~5,3,7,6,8,12,8,4,5,11,4,9,5,6,3,9,10,11,6,11,4,10,1,5,9,11,7,9,4,11,11,9,10,6,4,8,9,12,12,8,9,8,5,10,7,10,8,9,6,9,7,10,6,9,12,12,3,11,12,4,7,12,11,8,4,7,5,7,7,6,9,10,6,11,7,9,11,6,6,10,12,1,6,12,12,10,11,12,11,9,10,4,8,11,10,12,5,9,8,10,4,11,9,12,9,8,6,6,11,3,8,5,9,5,6,6,4,7,12,11,8,10,12,9,5,10,12,12,4,6,3,4,12,8,4,12,6,11~12,8,7,6,9,11,4,11,10,7,8,12,6,11,8,12,12,3,9,4,12,5,4,4,6,11,12,12,5,9,7,7,4,5,11,12,1,11,9,6,10,11,10,5,9,7,4,8,10,5,10,11,9,12,12,5,9,4,6,12,7,9,12,11,5,10,12,10,9,7,3,8,4,6,9,10,10,8,1,5,10,4,8,10,11,8,7,10,5,6,6,11,7,9,11,7,8,10,12,11,7,6,10,5,12,6,11,5,4,10,3,7,11,11,6,12,3,7~11,7,12,7,11,4,9,8,9,7,12,5,11,4,9,9,3,3,10,4,9,11,6,12,9,8,11,12,10,10,8,12,11,4,6,9,4,12,3,10,7,8,10,12,12,4,9,9,7,4,9,5,4,4,12,9,11,11,11,8,5,12,8,6,4,12,8,8,9,12,10,10,5,12,12,7,11,10,6,6,12,5,4,8,10,8,4,8,6,6,10,4,6,10,3,5,7,5,10,6,4,4,12,4,10,9,1,11,12,11,4,9,11,11,8,8,12,12,12,4,7,12,11,9,10,5,9,8,6,4,5,11,11,9,12,10,7,12,6,4,11,11,9,5,12,4,4,3,11,3,9,11,12,9,6,12,11,1,9,10,12,5,5,10,11,7,12,10,8,6,6,11,11,8,7,6,9,5&reel_set5=7,8,8,11,11,3,9,3,5,10,9,6,6,12,12,8,11,5,5,4,2,10,6,11,2,10,12,7,8,8,10,12,8,12,9,11,10,6,8,12,6,9,10,11,9,11,10,5,12,9,11,4,4,7,8,5,10,8,7,11,9,10,2,6,9,8,5,10,9,4,12,12,9,2,11,8&reel_set8=8,7,10,5,7,7,9,10,6,7,3,3,4,3,8,11,7,12,9,7,10,1,5,9,4,8,8,11,5,5,5,12,3,6,5,9,3,9,4,5,5,6,6,11,7,4,11,12,9,10,7,11,5,10,6,9,10,12,3,5,7,7,7,7,7,7,7,7,9,7,9,7,10,3,8,7,9,6,3,10,4,9,11,8,7,5,11,10,6,8,4,12,11,6,6,11,7,8,10~6,3,8,5,10,9,7,12,7,12,8,12,12,7,9,12,4,10,4,5,11,7,3,9,9,9,10,5,9,6,6,9,7,4,7,3,10,12,5,1,11,9,10,11,3,7,9,10,9,12,12,12,12,12,4,11,10,10,11,3,12,8,9,10,6,4,5,7,6,10,8,12,10,11,11,3,11,8,10,10,10,10,11,4,12,11,11,6,5,12,6,10,10,6,10,8,4,8,5,11,10,12,8,9,4,5,4~3,9,8,11,1,6,4,4,11,10,8,9,12,7,7,3,12,7,11,12,3,7,8,11,10,10,8,9,12,12,12,12,5,6,5,6,3,11,8,11,11,12,5,8,4,9,8,11,9,9,5,7,7,3,9,6,8,12,10,5,8,12,8,8,8,8,8,8,11,6,6,11,4,8,9,9,3,8,6,5,8,7,5,8,12,3,8,7,8,9,3,4,12,12,9,5,3,6,3~4,5,3,12,9,4,12,6,12,3,10,6,12,6,7,9,4,12,6,10,5,12,12,6,12,11,6,10,8,4,4,1,9,10,9,12,4,11,8,7,6,12,5,12,9,9,12,11,7,9,11,7,9,6,8,3,3,3,9,4,12,11,9,9,7,10,10,11,10,12,10,6,6,5,11,11,1,5,3,10,12,11,4,8,8,6,9,5,4,5,6,11,7,12,3,11,6,6,10,8,3,8,7,10,8,11,10,7,9,9,11,8,11,4,5~6,4,12,11,11,12,7,11,1,5,10,7,10,12,7,11,12,7,5,6,9,6,9,6,7,10,4,9,7,8,12,7,8,12,6,4,9,10,11,3,3,8,10,4,4,9,7,10,11,6,6,12,11,9,12,5,4,4,10,8,12,12,7,3,6,4,11,6,7,12,10,6,4,5,12,9,11,10,3,7,11,8,10,10,12,9,12,5,12,10,10,7,9,5,6,4,7,11,5,12,10,9,10,5,5,6,7,10,4,5,3,11,5,9,12,11,10,4,10,11,11,10,10,11,6,9,5,3,7,8,5,10,6,12,5,5,11,12,8,6,12,7,11,11,7,4,9,12,3,12,8,11,9,7,3,9,11,5,7,12,9,5,4,1,7,11,8,3,10,8,3,8,12,6,6,9,11,8,11,8,7,12,10,6,11,8,9,5~6,3,6,9,6,5,6,10,4,12,10,8,6,10,5,12,11,4,5,6,9,9,7,6,9,5,11,10,9,7,10,12,10,12,10,9,10,11,4,12,5,5,7,4,11,11,11,4,12,7,11,11,10,7,10,9,11,11,12,11,12,5,9,4,1,11,8,10,3,9,10,11,9,12,12,11,8,9,12,3,7,6,11,4,11,8,7,12,11,12,10,7,3,12,12,12,5,4,8,11,5,4,8,10,8,5,9,11,8,12,9,4,4,6,4,9,4,10,4,12,12,11,4,8,12,7,8,1,4,8,9,12,9,12,12,4,3,8,6,6,9,6,3&reel_set7=10,12,6,7,10,8,12,10,11,10,6,8,11,12,9,2,7,11,4,9,10,8,7,11,6,2,12,8,2,12,11,7,12,5,9,8,4,4,2,11,7,10,5,11,9,3,9,10,12,12,6,9,12,4,8,9,8,9,11,6,10,4,7,11,2,6,6,9,10,9,10,12,12,4,9,6,11,4,5,7,12,4,5,2,11,12,2,11,11,12,10,6,11,4,2,9,11,5,9,6,2,9,4,9,12,9,4,5,6,10,8,4,12,4,2,12,7,11,5,12,12,11,10,7,11,10,7,11,11,4,11,3,8,7,8,6,4,4,10,5,9,11,11,10,8,2,4,7,8,9,9,12,10,10,12,9,11,5,9,7,10,10,11,2,10,10,9,11,11,10,9,12,7,12,11,4,11,12,9,12,5,2,12,4,12,9&reel_set9=11,4,7,11,4,12,7,6,9,10,8,3,9,12,6,4,7,3,7,8,12,10,3,4,5,12,5,5,11,4,12,10,7,11,7,4,10,4,10,6,3,11,12,11,6,10,7,11,11,12,10,12,5,4,3,11,6,9,12,11,11,7,6,3,9,10,11,4,11,12,10,10,4,8,3,7,9,9,5,3,9,5,10,6,10,11,6,8,8,12,3,8,5,11,11,11,12,8,9,6,9,12,10,9,4,11,11,9,6,3,9,7,11,4,9,7,10,9,9,11,7,12,11,8,12,10,12,3,6,9,3,4,9,6,12,12,9,4,10,12,5,12,5,5,4,7,11,11,5,10,4,5,9,11,8,8,12,6,10,9,11,12,9,6,9,9,4,10,8,8,12,10,12,9,5,12,10,12,7,12,3,8,12,12,3,10,8,11,5,9,11,4&total_bet_min=10.00";
            }
        }

        protected override int FreeSpinTypeCount
        {
            get { return 10; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            List<int> freeSpinTypes = new List<int>();
            for (int i = freeSpinGroup; i < 10; i++)
                freeSpinTypes.Add(200 + i);

            return freeSpinTypes.ToArray();
        }
        #endregion

        public MammothGoldMegaGameLogic()
        {
            _gameID = GAMEID.MammothGoldMega;
            GameName = "MammothGoldMega";
        }
        protected override void onDoCollect(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    _logger.Error("{0} result information has not been found in BasePPSlotGame::onDoCollect.", strGlobalUserID);
                    return;
                }

                BasePPSlotSpinResult result = _dicUserResultInfos[strGlobalUserID];
                if (result.NextAction != ActionTypes.DOCOLLECT)
                {
                    _logger.Error("{0} next action is not DOCOLLECT just {1} in BasePPSlotGame::onDoCollect.", strGlobalUserID, result.NextAction);
                    return;
                }

                int index = (int)message.Pop();
                int counter = (int)message.Pop();
                Dictionary<string, string> responseParams = new Dictionary<string, string>();
                responseParams.Add("balance", Math.Round(userBalance, 2).ToString());
                responseParams.Add("balance_cash", Math.Round(userBalance, 2).ToString());
                responseParams.Add("balance_bonus", "0.00");
                responseParams.Add("na", "s");
                responseParams.Add("stime", GameUtils.GetCurrentUnixTimestampMillis().ToString());
                responseParams.Add("sver", "5");
                responseParams.Add("index", index.ToString());
                responseParams.Add("counter", (counter + 1).ToString());

                GITMessage reponseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOCOLLECT);
                string strCollectResponse = convertKeyValuesToString(responseParams);
                reponseMessage.Append(strCollectResponse);

                Context.System.Scheduler.ScheduleTellOnce(100, Sender, new ToUserMessage((int)_gameID, reponseMessage), Self);

                addActionHistory(strGlobalUserID, "doCollect", strCollectResponse, index, counter);
                saveHistory(agentID, strUserID, index, counter, userBalance, currency);
                
                result.NextAction = ActionTypes.DOSPIN;
                saveBetResultInfo(strGlobalUserID);

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in MammothGoldMegaGameLogic::onDoCollect {0}", ex);
            }
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                BasePPSlotBetInfo betInfo = new BasePPSlotBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();

                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in MammothGoldMegaGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine = betInfo.BetPerLine;
                    oldBetInfo.LineCount = betInfo.LineCount;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in MammothGoldMegaGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["st"] = "rect";
            dicParams["sw"] = "6";
            dicParams["g"] = "{main:{def_s:\"4,9,11,12,5,12,6,9,3,7,7,8,4,9,11,4,5,10,6,5,3,6,7,12,10,9,13,4,5,10,13,9,13,13,13,12,13,13,13,13,13,11\",def_sa:\"3,3,3,3,3,3\",def_sb:\"4,4,4,4,4,4\",reel_set:\"0\",s:\"4,9,11,12,5,12,6,9,3,7,7,8,4,9,11,4,5,10,6,5,3,6,7,12,10,9,13,4,5,10,13,9,13,13,13,12,13,13,13,13,13,11\",sa:\"3,3,3,3,3,3\",sb:\"4,4,4,4,4,4\",sh:\"7\",st:\"rect\",sw:\"6\"},top:{def_s:\"10,4,4,5\",def_sa:\"3\",def_sb:\"4\",reel_set:\"1\",s:\"10,4,4,5\",sa:\"3\",sb:\"4\",sh:\"4\",st:\"rect\",sw:\"1\"}}";
        }
        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst)
        {
            try
            {
                MammothGoldMegaMegaResult spinResult = new MammothGoldMegaMegaResult();
                Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(dicParams, betInfo.TotalBet);

                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                if (SupportPurchaseFree && betInfo.PurchaseFree && isFirst)
                    dicParams["purtr"] = "1";

                spinResult.NextAction = convertStringToActionType(dicParams["na"]);
                if (spinResult.NextAction == ActionTypes.DOCOLLECT || spinResult.NextAction == ActionTypes.DOCOLLECTBONUS)
                    spinResult.TotalWin = double.Parse(dicParams["tw"]);
                else
                    spinResult.TotalWin = 0.0;

                if (spinResult.NextAction == ActionTypes.DOBONUS)
                    spinResult.DoBonusCounter = -1;

                spinResult.ResultString = convertKeyValuesToString(dicParams);
                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in MammothGoldMegaGameLogic::calculateResult {0}", ex);
                return null;
            }
        }
        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            MammothGoldMegaMegaResult result = new MammothGoldMegaMegaResult();
            result.SerializeFrom(reader);
            return result;
        }        
        protected Dictionary<string, string> buildDoBonusResponse(MammothGoldMegaMegaResult result, BasePPSlotBetInfo betInfo, BasePPSlotStartSpinData startSpinData, int doBonusCounter, int ind, bool isNextMove)
        {
            Dictionary<string, string> dicParams            = new Dictionary<string, string>();
            Dictionary<string,string>  dicLastParams        = splitResponseToParams(result.BonusResultString);
            Dictionary<string, string> dicLastResultParams  = splitResponseToParams(result.ResultString);

            JToken gParam = JToken.Parse(dicLastParams["g"]);
            if (gParam["bg_1"]["ch_h"] == null)
                gParam["bg_1"]["ch_h"] = string.Format("0~{0}", ind);
            else
                gParam["bg_1"]["ch_h"] = string.Format("{1},0~{0}", ind, (string)gParam["bg_1"]["ch_h"]);

            int freeSpinCount = 5 + result.DoBonusCounter + startSpinData.FreeSpinGroup;
            if (ind == 0)
            {
                
                dicParams["fsmul"]      = "1";
                dicParams["fsmax"]      = freeSpinCount.ToString();
                dicParams["na"]         = "s";
                dicParams["fswin"]      = "0.00";
                dicParams["rw"]         = "0.00";
                dicParams["fs"]         = "1";
                dicParams["fsres"]      = "0.00";
                dicParams["trail"]      = string.Format("fs_n~{0}", freeSpinCount);
                gParam["bg_1"]["end"]   = "1";
                removeJTokenField(gParam["bg_1"], "ask");
                dicParams["g"]          = serializeJsonSpecial(gParam);
                return dicParams;
            }
            else
            {
                if (!isNextMove)
                {
                    dicParams["trail"]      = "fs_n~0";
                    gParam["bg_1"]["end"]   = "1";
                    if (double.Parse(dicLastResultParams["tw"]) > 0.0)
                        dicParams["na"] = "c";
                    else
                        dicParams["na"] = "s";
                }
                else
                {
                    dicParams["trail"]  = string.Format("fs_n~{0}", freeSpinCount);
                    dicParams["na"]     = "b";
                }
                dicParams["g"] = serializeJsonSpecial(gParam);
                return dicParams;
            }
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("tmb_win"))
                dicParams["tmb_win"] = convertWinByBet(dicParams["tmb_win"], currentBet);
            if (dicParams.ContainsKey("tmb_res"))
                dicParams["tmb_res"] = convertWinByBet(dicParams["tmb_res"], currentBet);

            if (dicParams.ContainsKey("wlc_v"))
            {
                string strWlc_v = dicParams["wlc_v"];
                string[] strParts = strWlc_v.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strParts.Length; i++)
                {
                    string[] strSubParts = strParts[i].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                    if (strSubParts.Length >= 2)
                        strSubParts[1] = convertWinByBet(strSubParts[1], currentBet);

                    strParts[i] = string.Join("~", strSubParts);
                }
                dicParams["wlc_v"] = string.Join(";", strParts);
            }

        }
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                double              realWin         = 0.0;
                string              strGameLog      = "";
                string              strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                ToUserResultMessage resultMsg       = null;
                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BasePPSlotBetInfo           betInfo = _dicUserBetInfos[strGlobalUserID];
                    MammothGoldMegaMegaResult   result  = _dicUserResultInfos[strGlobalUserID] as MammothGoldMegaMegaResult;
                    if ((result.NextAction != ActionTypes.DOBONUS) || (betInfo.SpinData == null) || !(betInfo.SpinData is BasePPSlotStartSpinData))
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        bool                        isCollect = false;
                        bool                        isEnded   = false;
                        Dictionary<string, string>  dicParams = null;
                        var                         startSpinData   = betInfo.SpinData as BasePPSlotStartSpinData;
                        if (result.DoBonusCounter == -1)
                        {
                            BasePPActionToResponse actionResponse = betInfo.pullRemainResponse();
                            dicParams                             = splitResponseToParams(actionResponse.Response);
                            result.DoBonusCounter++;
                            if (startSpinData.FreeSpinGroup == 9)
                            {
                                BasePPSlotSpinData freeSpinData = convertBsonToSpinData(startSpinData.FreeSpins[result.DoBonusCounter]);
                                preprocessSelectedFreeSpin(freeSpinData, betInfo);

                                betInfo.SpinData = freeSpinData;
                                List<string> freeSpinStrings = new List<string>();
                                for (int i = 0; i < freeSpinData.SpinStrings.Count; i++)
                                    freeSpinStrings.Add(addStartWinToResponse(freeSpinData.SpinStrings[i], startSpinData.StartOdd));

                                betInfo.RemainReponses = buildResponseList(freeSpinStrings, ActionTypes.DOSPIN);
                            }
                        }
                        else
                        {
                            int ind             = (int)message.Pop();
                            int stage           = startSpinData.FreeSpinGroup + result.DoBonusCounter;
                            do
                            {
                                if (ind == 0)
                                {
                                    isCollect = true;
                                    break;
                                }
                                double[] moveProbs = new double[] { 0.6452, 0.6833, 0.7202, 0.7526, 0.7818, 0.8064, 0.8284, 0.8468, 0.8628 };
                                if (betInfo.SpinData.IsEvent || PCGSharp.Pcg.Default.NextDouble(0.0, 1.0) <= moveProbs[stage])
                                {
                                    result.DoBonusCounter++;
                                    if (startSpinData.FreeSpinGroup + result.DoBonusCounter < 9)
                                    {
                                        dicParams = buildDoBonusResponse(result, betInfo, startSpinData, result.DoBonusCounter, 1, true);
                                    }
                                    else
                                    {
                                        isCollect = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    dicParams           = buildDoBonusResponse(result, betInfo, startSpinData, result.DoBonusCounter, 1, false);
                                    double selectedWin  = startSpinData.StartOdd * betInfo.TotalBet;
                                    double maxWin       = startSpinData.MaxOdd * betInfo.TotalBet;

                                    //시작스핀시에 최대의 오드에 해당한 윈값을 더해주었으므로 그 차분을 보상해준다.
                                    sumUpWebsiteBetWin(agentID, 0.0, selectedWin - maxWin);

                                    isEnded = true;
                                }
                            } while (false);

                            if (isCollect)
                            {
                                BasePPSlotSpinData freeSpinData = convertBsonToSpinData(startSpinData.FreeSpins[result.DoBonusCounter]);
                                preprocessSelectedFreeSpin(freeSpinData, betInfo);

                                betInfo.SpinData = freeSpinData;
                                List<string> freeSpinStrings = new List<string>();
                                for (int i = 0; i < freeSpinData.SpinStrings.Count; i++)
                                    freeSpinStrings.Add(addStartWinToResponse(freeSpinData.SpinStrings[i], startSpinData.StartOdd));

                                betInfo.RemainReponses = buildResponseList(freeSpinStrings, ActionTypes.DOSPIN);
                                double selectedWin = (startSpinData.StartOdd + freeSpinData.SpinOdd) * betInfo.TotalBet;
                                double maxWin      = startSpinData.MaxOdd * betInfo.TotalBet;

                                //시작스핀시에 최대의 오드에 해당한 윈값을 더해주었으므로 그 차분을 보상해준다.
                                sumUpWebsiteBetWin(agentID, 0.0, selectedWin - maxWin);
                                
                                dicParams = buildDoBonusResponse(result, betInfo, startSpinData, result.DoBonusCounter, 0, false);
                            }
                            else
                            {
                                //필요없는 응답을 삭제한다.
                                betInfo.pullRemainResponse();
                            }
                        }

                        result.BonusResultString = convertKeyValuesToString(dicParams);
                        addDefaultParams(dicParams, userBalance, index, counter);
                        ActionTypes nextAction  = convertStringToActionType(dicParams["na"]);
                        string      strResponse = convertKeyValuesToString(dicParams);
                        responseMessage.Append(strResponse);

                        //히스토리보관 및 초기화
                        if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                            addActionHistory(strGlobalUserID, "doBonus", strResponse, index, counter);

                        result.NextAction = nextAction;
                        if (!betInfo.HasRemainResponse)
                            betInfo.RemainReponses = null;

                        if (isEnded)
                        {
                            Dictionary<string, string> dicLastResultParams = splitResponseToParams(result.ResultString);

                            realWin     = double.Parse(dicLastResultParams["tw"].ToString());
                            strGameLog  = strResponse;

                            if (realWin > 0.0f)
                            {
                                _dicUserHistory[strGlobalUserID].baseBet = betInfo.TotalBet;
                                _dicUserHistory[strGlobalUserID].win = realWin;
                            }
                            else
                            {
                                saveHistory(agentID, strUserID, index, counter, userBalance, currency);
                            }
                            resultMsg = new ToUserResultMessage((int)_gameID, responseMessage, 0.0, realWin, new GameLogInfo(this.GameName, "0", strGameLog), UserBetTypes.Normal); ;
                            resultMsg.BetTransactionID  = betInfo.BetTransactionID;
                            resultMsg.RoundID           = betInfo.RoundID;
                            resultMsg.TransactionID     = createTransactionID();
                        }
                        saveBetResultInfo(strGlobalUserID);
                    }
                }
                if (resultMsg == null)
                    Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
                else
                    Sender.Tell(resultMsg);

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseSelFreePPSlotGame::onFSOption {0}", ex);
            }
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "s", "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set", "tw", "sw", "st", "wmt", "wmv", "rs_t", "rs_win", "gwm", "bw" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]))
                    continue;
                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }

            if(spinParams.ContainsKey("g") && resultParams.ContainsKey("g"))
            {
                JToken gParam      = JToken.Parse(spinParams["g"]);
                JToken gBonusParam = JToken.Parse(resultParams["g"]);
                gParam["bg_0"]     = gBonusParam["bg_0"];
                gParam["bg_1"]     = gBonusParam["bg_1"];
                resultParams["g"] = serializeJsonSpecial(gParam);
            }
            return resultParams;
        }
        protected override async Task buildStartFreeSpinData(BasePPSlotStartSpinData startSpinData, StartSpinBuildTypes buildType, double minOdd, double maxOdd)
        {
            if (buildType == StartSpinBuildTypes.IsNaturalRandom)
                await base.buildStartFreeSpinData(startSpinData, StartSpinBuildTypes.IsTotalRandom, minOdd, maxOdd);
            else
                await base.buildStartFreeSpinData(startSpinData, buildType, minOdd, maxOdd);
        }

    }
}
