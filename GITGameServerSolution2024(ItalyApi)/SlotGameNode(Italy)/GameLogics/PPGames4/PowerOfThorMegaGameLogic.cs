using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using Akka.Actor;
using SlotGamesNode.Database;
using System.IO;

namespace SlotGamesNode.GameLogics
{
    class PowerOfThorMegaResult : BasePPSlotSpinResult
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
    
    class PowerOfThorMegaGameLogic : BaseSelFreePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vswayshammthor";
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
        protected override string InitDataString
        {
            get
            {
                return "def_s=19,8,7,10,12,19,10,7,3,9,5,12,11,9,1,11,12,5,11,9,8,5,12,6,11,9,10,5,19,9,11,9,19,19,19,9,19,9,19,19,19,9,19,19,19,19,19,12&nas=19&cfgs=4831&accm=cp~mp&ver=2&reel_set_size=10&scatters=1~0,0,0,0,0,0~0,0,0,0,0,0~1,1,1,1,1,1&gmb=0,0,0&rt=d&gameInfo={rtps:{purchase:\"94.77\",regular:\"94.59\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"465332\",max_rnd_win:\"5000\",gamble_lvl_2:\"67.43 % \",gamble_lvl_3:\"73.38 % \",gamble_lvl_1:\"59.28 % \"}}&wl_i=tbm~5000&sc=0.01,0.02,0.03,0.04,0.05,0.10,0.20,0.30,0.40,0.50,0.75,1.00,2.00,3.00,4.00,5.00&defc=0.10&wilds=2~0,0,0,0,0,0~1,1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;400,200,100,20,10,0;100,50,40,10,0,0;50,30,20,10,0,0;40,15,10,5,0,0;30,12,8,4,0,0;25,10,8,4,0,0;20,10,5,3,0,0;18,10,5,3,0,0;16,8,4,2,0,0;12,8,4,2,0,0;0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0&rtp=94.59,94.77&total_bet_max=10,000.00&reel_set0=6,2,11,12,9,11,10,5,12,5,9,12,4,9,5,7,4,12,4,17,11,14,12,8,2,7,7,12,11,3,8,12,12,5,5,12,9,10,7,9,5,14,16,17,12,10,2,5,17,9,10,4,11,12,9,14,8,2,4,9,9,2,2,8,9,5,7,9,12,9,10,7,12,12,4,17,12,5,5,3,7,12,10,4,11,10,10,7,3,12,7,5,2,7,6,5,18,3,3,4,8,9,9,9,12,12,9,9,12,10,9,9,12,7,5,7,2,9,5,10,9,5,4,10,9,4,9,17,2,7,12,10,12,5,9,10,9,15,12,7,4,4,8,9,7,9,17,3,16,8,4,9,8,17,2,10,5,3,17,2,10,9,9,4,12,12,10,10,18,8,7,12,10,3,9,7,17,7,11,17,2,4,2,4,17,2,12,7,6,3,14,10,9,4,6,12,18,3,9,7,7,10,2,6,12,3,10,10,10,4,8,9,6,16,12,7,3,2,9,6,11,7,4,5,5,12,11,10,4,5,4,9,9,2,15,12,3,4,2,10,10,12,9,2,5,12,7,3,3,9,4,2,3,5,17,2,4,9,4,7,17,9,8,9,12,7,10,12,15,8,7,10,9,3,15,6,7,2,18,17,3,9,5,17,12,4,2,9,12,9,9,3,5,10,12,10,2,4,17,6,2,10,5,16,12,11,8,6,7,7,4,7&accInit=[{id:0,mask:\"cp; mp\"}]&reel_set2=2,2,11,3,10,7,8,12,18,5,2,6,12,12,4,11,10,7,2,17,11,10,2,12,4,2,8,16,15,7,10,11,2,11,5,12,9,10,2,8,11,9,6,6,10,10,10,12,12,14,8,6,11,18,10,2,3,2,4,4,7,12,6,8,11,10,7,10,2,13,10,10,14,11,10,2,8,7,6,5,16,8,11,8,10,11,3,4,12,9,11,9,9,9,9,10,9,9,3,17,11,10,8,4,18,9,7,18,7,7,3,11,3,8,9,8,10,12,10,3,2,15,9,10,10,7,13,10,2,11,7,11,11,5,9,12,8,7,9,9,11&t=243&reel_set1=11,11,4,5,10,1,5,11,10,11,10,9,7,5,1,9,8,6,10,5,5,5,12,11,5,10,5,6,5,4,6,4,11,7,11,4,4,9,5,3,11,3,11,10,10,10,10,1,9,8,9,5,5,10,4,7,10,7,4,11,12,5,10,3,5,5,4,11,11,11,10,12,5,11,6,12,11,1,10,5,12,1,4,5,11,7,4,10,1,1,10,4,4,4,11,4,11,5,10,9,8,7,4,4,11,5,5,10,4,10,5,4,7,11,5,8~12,8,9,12,5,12,11,9,12,4,9,6,10,5,9,7,7,9,9,9,1,12,9,9,8,1,5,12,8,10,7,8,6,10,9,10,7,8,8,8,8,10,7,3,12,12,7,7,12,5,7,12,7,9,8,4,4,11,7,7,7,7,7,1,3,12,12,9,5,7,7,9,12,4,4,9,7,9,9,6,12,12,12,12,11,7,7,5,1,9,4,11,8,12,6,11,9,9,8,6,7,8,7~6,6,10,10,9,8,7,8,9,6,4,4,8,6,1,7,12,5,8,5,10,11,11,4,12,8,7,11,10,10,7,6,9,11,8,1,6,12,8,8,12,7,9,11,8,10,7,4,11,3,5,6,6,6,1,1,9,5,4,12,4,5,7,4,10,6,8,7,3,12,4,6,4,5,9,12,5,10,6,4,6,10,6,12,8,6,6,4,10,4,4,5,11,7,3,12,5,9,4,10,6,7,10,4,6,1,4,12~9,4,12,12,11,5,7,4,10,3,10,3,5,10,5,9,5,11,6,9,5,6,3,10,10,10,5,6,4,1,5,3,10,5,6,9,8,10,5,12,5,9,4,9,8,6,5,5,11,8,12,5,5,5,6,6,4,7,9,10,5,10,5,10,10,11,4,8,6,7,6,10,6,3,6,10,3,3,6,6,6,10,5,5,10,7,9,4,5,1,6,5,7,5,7,5,7,10,5,8,7,9,8,9,9,5,9,9,9,5,6,6,8,1,6,9,12,10,10,12,4,10,8,5,5,7,1,12,3,8,6,5,9,12,5~10,5,11,1,11,10,4,8,11,9,8,6,6,12,6,6,5,3,6,5,7,9,12,7,11,3,8,1,6,10,6,9,11,1,6,5,4,11,12,9,4,9,12,4,11,6,9,11,5,11,6,5,8,7,9,11,4,1,10,6,6,6,6,12,5,11,5,6,12,11,12,8,9,6,11,10,6,8,1,5,7,7,9,5,9,7,12,5,10,6,6,4,11,10,10,5,9,9,7,11,6,1,6,6,12,4,12,9,9,6,1,12,7,11,4,1,5,8,12,3,12,5,6,9~8,4,7,9,5,9,5,9,4,4,10,10,9,10,5,10,12,4,9,12,11,7,11,12,12,8,5,10,1,10,7,9,9,9,4,11,10,4,5,8,3,11,9,12,9,5,9,11,8,4,12,5,5,7,12,1,12,12,11,6,11,7,5,11,9,5,12,12,12,10,10,9,9,6,6,12,11,8,11,4,9,3,1,4,6,7,6,9,9,4,11,12,5,9,1,6,7,4,1,12,9,10&reel_set4=10,10,3,12,13,7,12,10,4,3,11,11,8,16,9,12,3,6,2,15,11,8,18,2,11,9,18,12,2,18,3,10,11,18,12,11,7,10,10,18,3,8,9,2,6,17,10,10,9,8,12,12,18,10,18,5,8,11,14,7,10,17,18,8,12,7,4,13,8,7,2,11,10,2,7,5,11,8,10,6,8,9,9,11,7,7,12,5,12,11,9,10,12,12,11,10,7,3,10,10,10,9,11,6,7,8,7,7,14,11,6,11,8,11,10,11,10,12,11,10,10,11,10,12,11,11,4,12,3,2,16,16,2,11,3,11,3,10,5,5,18,10,4,4,3,8,9,10,6,8,18,9,8,2,8,7,8,9,9,13,12,7,9,10,4,7,11,8,11,9,18,11,5,8,16,8,10,6,14,4,3,7,11,3,8,5,10,13,11,10,10,12,4,11,6,4,18,10,7,2,11,9,9,9,11,2,11,8,15,10,18,10,11,2,9,15,4,11,12,5,6,11,9,10,10,9,4,17,7,18,11,14,2,11,8,7,9,7,9,9,12,12,10,3,10,18,5,7,2,9,8,17,11,8,10,2,12,3,7,4,10,11,7,18,11,9,8,8,7,11,11,15,3,11,5,8,9,10,7,10,7,7,6,2,11,7,9,9,10,11,7,3,12,10,10,11,8,9,10,7,9,7,10,4,9&purInit=[{type:\"fsbl\",bet:2000,bet_level:0}]&reel_set3=10,8,9,9,8,5,12,12,4,12,5,7,8,12,8,5,5,11,9,9,9,9,12,8,5,5,12,12,5,12,9,4,5,9,12,12,8,9,9,8,5,5,5,5,8,12,10,9,8,10,3,7,8,6,5,9,12,8,6,5,8,4,8,8,8,10,9,9,3,10,5,8,5,12,6,7,12,12,8,9,5,7,8,6,12,12,12,9,9,11,7,8,9,7,8,12,12,7,12,11,8,8,9,12,8,5,5,4~9,10,6,10,11,11,6,7,6,8,7,10,10,10,11,4,11,12,7,4,10,7,7,3,4,11,10,4,4,4,9,5,11,5,10,6,12,7,7,8,8,4,7,7,7,7,7,9,12,5,3,7,10,10,8,11,9,11,12,11,11,11,10,11,10,11,7,7,4,10,11,7,10,8,7,11~10,7,6,11,9,5,6,9,12,11,11,9,11,6,4,12,4,8,7,9,12,5,5,4,7,6,4,4,10,8,6,10,12,8,6,6,6,6,6,8,6,9,4,7,9,8,5,9,6,8,11,11,8,8,4,8,9,6,12,7,9,11,4,3,7,6,6,8,10,4,12,12,8~4,12,9,4,9,3,10,3,9,10,10,10,6,12,9,12,11,3,8,12,10,9,12,12,12,12,6,8,7,3,12,12,4,6,7,9,9,9,6,10,9,12,6,11,10,7,5,6,8,12~12,6,10,10,8,3,6,6,12,9,5,11,12,7,11,8,6,4,6,10,5,5,6,10,12,7,12,10,11,8,12,8,10,10,5,11,12,8,3,3,5,6,5,10,9,9,4,12,10,6,12,5,9,8,9,5,7,8,9,6,5,8,6,10,6,11,5,11,7,7,12,10,10,8,10,6,5,12,5,4,7,8,6,4,5,6,12,10,11,8,6,6,5,12,4,8,11,8,5,10,6,11,6,4,12,6,5,11,10,4,10,5,3,6,5,6,6,5,11,11,4,5,11,11,9,9,3,12,6,12,11,11,5,10,6,7,12,10,6,11,11,9,7,12,9,9,12,6,6,5,6,11~4,11,8,8,5,7,9,10,10,10,10,9,8,12,6,3,12,7,9,10,11,11,11,12,10,10,11,5,11,6,11,5,4&reel_set6=12,12,8,10,5,1,12,11,4,8,15,16,10,3,12,7,8,12,9,8,9,3,11,11,10,5,14,9,2,3,7,3,5,10,10,17,7,11,8,12,2,11,10,12,12,12,3,10,7,10,10,11,3,4,9,2,12,10,11,10,7,11,12,7,9,11,7,10,12,11,11,10,8,7,9,12,11,7,5,11,12,14,7,13,10,9,2,13,3,8,5,11,11,11,10,13,3,12,6,10,12,7,4,11,8,12,6,4,11,7,9,12,5,9,12,11,13,9,9,7,9,6,10,2,8,12,7,10,8,8,10,12,9,10,11,2,11,12,1,1,1,1,15,16,8,12,11,17,4,11,9,8,11,7,9,12,3,13,13,11,12,11,10,8,10,12,11,9,6,6,8,11,10,11,7,2,7,7,6,12,11,8,2,3,10,10,4&reel_set5=4,12,9,5,10,6,8,6,8,6,6,12,11,8,11,12,5,9,9,9,9,8,8,10,12,9,7,10,9,8,12,8,8,9,12,12,7,9,8,12,8,12,12,12,12,12,8,4,12,8,6,4,10,11,9,12,9,8,7,6,12,8,12,6,8,8,8,3,6,4,11,8,9,8,8,9,6,12,6,5,7,9,10,9,11,6,5,8~7,12,12,4,9,8,9,12,12,10,9,8,8,11,9,4,8,8,10,9,12,6,8,10,12,5,10,7,8,5,11,11,12,12,6,3,6,9,8,12,8,3,6,8,3,9,12,5,6,12,8,7,8,9,9,6,9,8~8,7,5,4,12,12,5,10,7,12,10,11,6,11,11,10,10,12,5,12,12,8,10,6,7,9,4,12,10,4,11,7,9,10,10,8,12,10,11,9,11,6,10,10,3,9,6,11,4,9,11,8,10,9,12,7,7,7,10,9,10,12,11,5,8,9,3,8,7,7,9,7,6,8,9,9,8,4,9,8,6,11,7,7,8,9,4,10,4,7,5,7,12,4,7,8,10,5,12,6,4,7,6,9,6,7,11,8,10,11,9,7,9,10,4,6~11,8,7,9,7,5,3,9,5,3,12,9,8,4,8,9,12,3,4,12,8,7,7,6,9,11,12,9,12,3,12,9,7,9,7,12,3,12,9,5,5,5,9,10,4,12,9,8,12,4,7,5,12,7,12,7,10,7,5,8,12,11,12,12,8,5,12,3,10,12,11,4,9,3,12,12,9,12,9,7,5,12,12,12,12,12,11,7,11,8,7,7,12,10,8,7,9,7,9,12,3,5,7,7,9,12,4,5,10,12,4,10,7,7,10,7,9,12,12,9,7,3,12,3,4,9,9,9,5,8,5,8,3,3,12,12,10,9,5,3,3,12,5,9,12,11,11,9,3,12,7,7,5,4,8,5,12,11,12,10,7,6,9,10,5,9,4,12,9,12~11,4,6,6,11,9,7,6,7,6,9,5,4,10,10,5,7,8,5,11,12,12,8,12,3,7~11,7,11,6,11,9,6,10,5,11,7,5,11,10,8,11,5,10,9,5,10,6,10,9,12,10,11,6,6,5,8,9,11,3,12,5,4,10,5,9,8,11,5,4,9,10,8,12,11,11,12,11,9,6,11,5,5,5,12,6,5,12,4,9,9,10,10,11,10,11,4,5,6,8,8,5,6,3,11,7,10,11,10,11,5,9,8,5,9,9,6,11,10,12,11,7,5,3,4,6,10,11,8,10,6,8,7,5,9,5,11,5,6,9,11,11,11,8,5,6,9,9,10,12,7,5,8,12,12,4,5,12,9,12,5,8,5,7,4,6,7,7,9,6,12,5,11,10,12,4,10,12,6,10,5,12,5,12,11,4,7,11,11,3,12,5,12,12,10,7,6,12,5,5,6&reel_set8=10,4,3,10,11,12,11,13,7,8,11,12,3,9,9,10,10,11,12,12,10,16,13,3,3,7,12,15,13,10,6,10,16,3,11,7,3,3,2,3,9,12,10,1,9,7,15,12,12,12,12,12,6,4,13,7,5,6,13,11,11,8,7,11,13,15,12,7,4,11,9,14,11,10,15,8,2,12,10,15,10,13,11,16,2,7,4,7,9,4,4,11,2,9,10,8,9,15,11,12,11,12,8,11,11,11,8,5,9,9,6,11,5,8,9,7,6,16,5,2,8,9,11,9,10,14,2,8,12,10,12,7,1,10,10,7,10,7,11,13,14,17,12,12,10,11,13,8,14,7,10,16,7,17,12,10,3,1,1,1,10,5,5,11,7,11,11,8,17,11,8,11,13,17,13,14,12,2,10,9,11,13,3,13,17,7,11,10,5,14,16,8,14,12,16,1,17,9,8,17,4,9,10,11,8,3,9,15,10,2,2,11&reel_set7=8,9,8,10,11,10,8,12,5,9,4,12,12,6,10,9,9,10,12,10,9,6,9,6,7,6,9,8,11,12,8,11,8,9,6,8,12,8,8,6,12,9,9,9,3,8,9,6,11,4,8,5,4,6,8,9,12,8,3,12,8,11,12,12,5,9,8,12,9,7,6,6,9,6,10,12,12,8,9,7,9,6,8,6,11,8,12,12,12,12,9,8,9,11,8,12,9,12,12,8,11,8,6,9,4,8,7,12,10,12,10,12,12,8,12,10,5,6,4,5,4,6,7,9,4,8,8,6,6,12,12,8,8,8,9,12,9,8,12,8,8,5,8,8,6,8,9,12,11,8,6,7,7,9,12,8,8,12,8,11,5,6,12,8,10,6,6,8,12,12,11,10,6,5,9,9,5~5,9,10,8,8,6,12,6,8,8,10,5,12,4,9,11,12,8,12,12,5,8,9,9,11,9,6,8,6,9,12,8,7,7,8,8,3,9,9,4,10,3,11,12,12,10~4,10,9,6,8,8,9,9,6,6,10,10,7,9,7,7,12,11,8,11,11,12,9,10,10,4,7,10,8,3,9,7,5,12,7,8,11,9,11,11,9,5,4,10,10,7,11,10,12,11,10,8,9,10,8,6,10,4,5,7,9,9,4,11,8,4,11,5,4,7,7,7,8,11,6,9,9,4,4,12,6,12,5,9,7,9,3,8,12,10,12,9,6,9,6,6,7,9,7,10,7,7,4,6,12,5,12,9,11,10,10,12,7,10,12,7,5,10,12,8,6,7,11,11,4,8,8,12,7,11,9,10,12,5,8,10,4,9,10,8,6,7,10,7~4,4,12,5,7,5,11,7,9,3,7,12,7,12,12,9,5,10,12,12,7,7,6,8,9,7,5,9,9,12,12,3,7,7,3,11,12,4,11,6,7,5,5,5,7,4,10,12,3,9,4,4,11,12,12,9,8,12,9,9,12,3,12,10,8,12,7,7,12,3,11,7,9,8,9,3,9,7,12,10,9,10,7,4,9,5,12,12,12,12,10,12,8,3,12,9,10,12,4,7,3,7,12,12,9,5,5,11,5,7,3,9,12,9,10,7,7,5,9,5,3,3,12,9,8,12,12,7,3,5,12,9,9,9,8,7,12,3,11,4,7,12,5,12,9,12,7,8,3,3,5,11,5,12,12,9,8,9,8,10,10,12,7,9,12,10,5,8,9,9,4,8,11,12,4,12,5~6,5,7,8,9,12,11,12,5,6,7,11,11,6,5,6,7,8,7,9,5,10,11,5,3,6,6,11,10,8,9,12,8,4,6,7,6,3,11,8,7,7,9,7,7,10,11,4,5,9,5,12,11,5,12,7,11,12,10,10,11,6,4,12,6,7,11,10,5,9,7,7,5,5,7,10,12,7,12,7,5,12,5,4,4,11,11,12,9,6,10,11,7,8,8,5,12,8,7,12,12,11,7,12,3,12,5,4,8,7,11,6,5,6,6,11,6,3,12,12,9,5,3,7,5,9,6,3,11,6,9,7,7,12,8,4,7,10,9,6,6,7,4,6,8,11,5,7,5,8,5,5,7,7,11,7,6,12,6,12,8,8,6,6,5,4,9,11~12,9,11,8,5,7,5,5,6,9,12,4,11,10,6,9,7,6,10,10,6,11,10,8,4,9,9,6,10,10,7,12,9,11,9,5,10,9,11,5,12,5,4,5,11,8,5,10,12,8,9,7,7,11,4,5,5,5,11,11,12,12,5,11,11,3,6,8,12,5,10,5,10,7,5,6,12,11,11,6,7,12,10,8,8,12,4,11,5,10,5,11,11,12,5,10,3,9,12,6,6,11,8,6,5,3,11,5,11,12,6,5,11,10,11,11,11,7,7,6,5,5,6,5,8,10,5,8,12,12,11,11,10,10,12,9,4,9,6,8,11,5,5,6,9,7,10,12,11,4,5,9,5,6,3,4,11,6,5,10,11,4,9,12,5,10,8,7,12,12,9,10,6,9&reel_set9=8,9,5,12,5,12,7,9,7,9,9,9,9,5,11,9,7,12,7,5,7,8,6,12,12,12,12,9,7,12,4,11,12,5,5,3,6,7,7,7,7,10,10,5,4,10,12,7,9,9,12,7~12,7,5,9,7,8,6,3,9,5,8,12,10,12,10,3,7,7,6,12,9,7,7,9,9,5,9,7,7,12,11,7,12,12,6,12,12,5,7,9,12,12,7,10,9,12,6,3,7,9,11,7,8,6,7,9,3,7,5,10,5,12,11,4,12,9,11,12,5,9,7,7,12,9,4,10,8,7,11,10,7,10,9,4,9,5,7~10,8,8,12,9,11,5,4,10,10,6,10,11,7,10,8,4,8,9,10,5,4,8,11,8,9,11,9,7,6,11,8,8,8,8,12,7,12,9,4,8,5,12,9,11,9,4,11,9,10,10,7,4,12,10,8,6,8,12,9,4,4,6,3,5,10,8,7~9,9,10,11,4,8,6,3,12,8,9,7,10,8,10,12,12,8,8,4,12,12,6,11,4,8,4,12,9,11,4,6,8,4,12,12,10,6,6,6,8,9,12,12,3,12,8,8,11,8,9,3,6,8,10,3,3,12,8,9,12,7,6,3,8,9,8,4,9,7,12,9,12,10,3,9,12,3,12,12,12,8,7,7,12,9,12,3,8,3,12,9,7,7,12,8,7,3,9,6,11,12,12,6,12,4,8,9,12,12,3,9,9,10,10,6,10,12,9,9,9,9,12,8,6,5,7,3,8,6,9,6,11,12,9,5,8,12,3,7,12,12,9,12,12,7,9,6,9,6,12,6,8,9,11,4,8,12,12,11,10,4~7,10,11,5,9,6,8,5,12,3,8,8,4,12,7,11,6~11,4,7,11,12,3,5,6,12,6,6,6,12,8,11,6,4,12,10,9,10,6,11,11,11,6,7,5,9,8,11,10,10,9,6,5&total_bet_min=0.01";
            }
        }

        protected override int FreeSpinTypeCount
        {
            get { return 6; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            switch(freeSpinGroup)
            {
                case 0:
                    return new int[] { 200, 201, 202, 203 };
                case 1:
                    return new int[] {201, 202, 203 };
                case 2:
                    return new int[] { 202, 203 };
                case 3:
                    return new int[] { 203 };
                case 4:
                    return new int[] { 204 };
                case 5:
                    return new int[] { 205 };

            }
            return new int[] { 200, 201, 202, 203, 204 };
        }
        #endregion

        public PowerOfThorMegaGameLogic()
        {
            _gameID = GAMEID.PowerOfThorMega;
            GameName = "PowerOfThorMega";
        }
        protected override void onDoCollect(string strUserID, int agentID, GITMessage message, double userBalance)
        {
            try
            {
                int index = (int)message.Pop();
                int counter = (int)message.Pop();

                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                Dictionary<string, string> responseParams = new Dictionary<string, string>();
                responseParams.Add("balance",       Math.Round(userBalance, 2).ToString());
                responseParams.Add("balance_cash",  Math.Round(userBalance, 2).ToString());
                responseParams.Add("balance_bonus", "0.00");
                responseParams.Add("na",            "s");
                responseParams.Add("stime",         GameUtils.GetCurrentUnixTimestampMillis().ToString());
                responseParams.Add("sver",          "5");
                responseParams.Add("index",         index.ToString());
                responseParams.Add("counter",       (counter + 1).ToString());

                GITMessage reponseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOCOLLECT);
                string strCollectResponse = convertKeyValuesToString(responseParams);
                reponseMessage.Append(strCollectResponse);

                Context.System.Scheduler.ScheduleTellOnce(100, Sender, new ToUserMessage((int)_gameID, reponseMessage), Self);

                addActionHistory(strGlobalUserID, "doCollect", strCollectResponse, index, counter);
                saveHistory(strUserID, agentID, index, counter, userBalance);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in PowerOfThorMegaGameLogic::onDoCollect {0}", ex);
            }
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in PowerOfThorMegaGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine = betInfo.BetPerLine;
                    oldBetInfo.LineCount = betInfo.LineCount;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in PowerOfThorMegaGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, int currency, double userBalance, int index, int counter)
        {
            Dictionary<string, string> dicInitParams = splitResponseToParams(InitDataString);
            dicParams.Add("balance",        Math.Round(userBalance, 2).ToString());
            dicParams.Add("balance_cash",   Math.Round(userBalance, 2).ToString());
            dicParams.Add("balance_bonus",  "0.00");
            dicParams.Add("na",             "s");
            dicParams.Add("stime",          GameUtils.GetCurrentUnixTimestampMillis().ToString());
            dicParams.Add("sver",           "5");
            dicParams.Add("sh",             ROWS.ToString());
            dicParams.Add("s",              dicInitParams["def_s"]); //def_s
            dicParams.Add("c",              dicInitParams["defc"]);  //defc
            dicParams.Add("l",              ServerResLineCount.ToString());
            if (index > 0)
            {
                dicParams.Add("index", index.ToString());
                dicParams.Add("counter", (counter + 1).ToString());
            }
            dicParams["acci"]   = "0";
            dicParams["accv"]   = "0~2";
            dicParams["st"]     = "0";
            dicParams["acci"]   = "rect";
            dicParams["g"]      = "{reg:{def_s:\"10,7,3,9,5,12,11,9,1,11,12,5,11,9,8,5,12,6,11,9,10,5,19,9,11,9,19,19,19,9,19,9,19,19,19,9,19,19,19,19,19,12\",def_sa:\"10,5,5,8,6,4\",def_sb:\"1,9,10,5,11,12\",reel_set:\"1\",s:\"10,7,3,9,5,12,11,9,1,11,12,5,11,9,8,5,12,6,11,9,10,5,19,9,11,9,19,19,19,9,19,9,19,19,19,9,19,19,19,19,19,12\",sa:\"10,5,5,8,6,4\",sb:\"1,9,10,5,11,12\",sh:\"7\",st:\"rect\",sw:\"6\"},top:{def_s:\"12,10,7,8\",def_sa:\"10\",def_sb:\"8\",reel_set:\"0\",s:\"12,10,7,8\",sa:\"10\",sb:\"8\",sh:\"4\",st:\"rect\",sw:\"1\"}}";
        }

        protected override BasePPSlotSpinResult calculateResult(string strGlobalUserID, BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst, ActionTypes action)
        {
            try
            {
                PowerOfThorMegaResult spinResult = new PowerOfThorMegaResult();
                Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(dicParams, betInfo.TotalBet);

                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                if (SupportPurchaseFree && betInfo.PurchaseFree && isFirst)
                    dicParams["purtr"] = "1";

                spinResult.NextAction   = convertStringToActionType(dicParams["na"]);
                spinResult.ResultString = convertKeyValuesToString(dicParams);
                if (spinResult.NextAction == ActionTypes.DOCOLLECT || spinResult.NextAction == ActionTypes.DOCOLLECTBONUS)
                {
                    spinResult.TotalWin = double.Parse(dicParams["tw"]);
                }
                else
                {
                    spinResult.TotalWin = 0.0;
                }

                if (!_dicUserHistory.ContainsKey(strGlobalUserID))
                    spinResult.RoundID = ((long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
                else
                    spinResult.RoundID = _dicUserHistory[strGlobalUserID].roundid;

                if (spinResult.NextAction == ActionTypes.DOBONUS)
                    spinResult.DoBonusCounter = 0;

                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in PowerOfThorMegaGameLogic::calculateResult {0}", ex);
                return null;
            }
        }

        protected override BasePPSlotSpinResult restoreResultInfo(string strGlobalUserID, BinaryReader reader)
        {
            PowerOfThorMegaResult result = new PowerOfThorMegaResult();
            result.SerializeFrom(reader);
            return result;
        }
        protected string buildStatus(int index)
        {
            int[] intArray = new int[] { 0, 0, 0, 0 };
            intArray[index] = 1;
            return string.Join(",", intArray);
        }
        protected Dictionary<string, string> buildDoBonusResponse(PowerOfThorMegaResult result, BasePPSlotBetInfo betInfo, BasePPSlotStartSpinData startSpinData, int doBonusCounter, int ind, bool isNextMove)
        {
            Dictionary<string, string> dicParams = new Dictionary<string, string>();

            int[] freeSpinCounts = new int[] { 10, 14, 18, 22 };
            Dictionary<string, string> dicLastParams = splitResponseToParams(result.ResultString);
            dicParams["tw"]         = dicLastParams["tw"];
            dicParams["bgid"]       = "0";
            dicParams["wins"]       = "10,14,18,22";
            dicParams["coef"]       = Math.Round(betInfo.TotalBet, 2).ToString();
            dicParams["level"]      = (result.DoBonusCounter + startSpinData.FreeSpinGroup + 1).ToString();
            dicParams["status"]     = buildStatus(result.DoBonusCounter + startSpinData.FreeSpinGroup);
            dicParams["bgt"]        = "35";
            dicParams["wins_mask"]  = "nff,nff,nff,nff";
            dicParams["wp"]         = "0";

            if (ind == 0)
            {
                dicParams["fsmul"]  = "1";
                dicParams["fsmax"]  = freeSpinCounts[result.DoBonusCounter + startSpinData.FreeSpinGroup].ToString();
                dicParams["na"]     = "s";
                dicParams["fswin"]  = "0.00";
                dicParams["rw"]     = "0.00";
                dicParams["fs"]     = "1";
                dicParams["lifes"]  = "1";
                dicParams["end"]    = "1";
                dicParams["fsres"]  = "0.00";
                return dicParams;
            }
            else
            {
                if (!isNextMove)
                {
                    dicParams["rw"]     = "0.0";
                    dicParams["lifes"]  = "0";
                    dicParams["end"]    = "1";

                    if (double.Parse(dicParams["tw"]) > 0.0)
                        dicParams["na"] = "c";
                    else
                        dicParams["na"] = "s";
                }
                else
                {
                    dicParams["lifes"] = "1";
                    dicParams["end"]   = "0";
                    dicParams["na"]    = "b";
                }
                return dicParams;
            }
        }
        protected override async Task onProcMessage(string strUserID, int agentID, CurrencyEnum currency, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            if (message.MsgCode == (ushort)CSMSG_CODE.CS_PP_DOBONUS)
                await onDoBonus(agentID, strUserID, message, userBalance);
            else
                await base.onProcMessage(strUserID, agentID, currency, message, userBonus, userBalance, isMustLose);
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("tmb_win"))
                dicParams["tmb_win"] = convertWinByBet(dicParams["tmb_win"], currentBet);
            if (dicParams.ContainsKey("tmb_res"))
                dicParams["tmb_res"] = convertWinByBet(dicParams["tmb_res"], currentBet);
        }
        protected async Task onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance)
        {
            try
            {
                int index    = (int)message.Pop();
                int counter  = (int)message.Pop();
                int ind      = (int)message.Pop();
                double realWin      = 0.0;
                string strGameLog   = "";

                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BasePPSlotBetInfo       betInfo     = _dicUserBetInfos[strGlobalUserID];
                    PowerOfThorMegaResult   result      = _dicUserResultInfos[strGlobalUserID] as PowerOfThorMegaResult;
                    if ((result.NextAction != ActionTypes.DOBONUS) || (betInfo.SpinData == null) || !(betInfo.SpinData is BasePPSlotStartSpinData))
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        BasePPSlotStartSpinData startSpinData = betInfo.SpinData as BasePPSlotStartSpinData;
                        int stage                             = startSpinData.FreeSpinGroup + result.DoBonusCounter;
                        Dictionary<string, string> dicParams  = null;
                        bool isCollect = false;
                        bool isEnded   = false;

                        do
                        {
                            if(ind == 0)
                            {
                                isCollect = true;
                                break;
                            }
                            double[] moveProbs = new double[] { 0.5928, 0.6743, 0.7338 };
                            if (betInfo.SpinData.IsEvent || PCGSharp.Pcg.Default.NextDouble(0.0, 1.0) <= moveProbs[stage])
                            {
                                result.DoBonusCounter++;
                                if (startSpinData.FreeSpinGroup + result.DoBonusCounter < 3)
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
                                dicParams = buildDoBonusResponse(result, betInfo, startSpinData, result.DoBonusCounter, 1, false);
                                double maxWin = startSpinData.MaxOdd * betInfo.TotalBet;
                                sumUpCompanyBetWin(agentID, 0.0, -maxWin);
                                isEnded = true;
                            }
                        } while (false);
                        if(isCollect)
                        {
                            OddAndIDData        selectedFreeSpinInfo = startSpinData.FreeSpins[result.DoBonusCounter];
                            BasePPSlotSpinData  freeSpinData         = await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(selectedFreeSpinInfo.ID), TimeSpan.FromSeconds(10.0));                            
                            preprocessSelectedFreeSpin(freeSpinData, betInfo);

                            betInfo.SpinData = freeSpinData;
                            List<string> freeSpinStrings = new List<string>();
                            for (int i = 0; i < freeSpinData.SpinStrings.Count; i++)
                                freeSpinStrings.Add(addStartWinToResponse(freeSpinData.SpinStrings[i], startSpinData.StartOdd));

                            betInfo.RemainReponses  = buildResponseList(freeSpinStrings, ActionTypes.DOSPIN);
                            double selectedWin      = (startSpinData.StartOdd + freeSpinData.SpinOdd) * betInfo.TotalBet;
                            double maxWin           = startSpinData.MaxOdd * betInfo.TotalBet;

                            //시작스핀시에 최대의 오드에 해당한 윈값을 더해주었으므로 그 차분을 보상해준다.
                            sumUpCompanyBetWin(agentID, 0.0, selectedWin - maxWin);
                            dicParams = buildDoBonusResponse(result, betInfo, startSpinData, result.DoBonusCounter, 0, false);
                        }
                        else
                        {
                            //필요없는 응답을 삭제한다.
                            betInfo.pullRemainResponse();
                        }
                        result.BonusResultString            = convertKeyValuesToString(dicParams);
                        addDefaultParams(dicParams, userBalance, index, counter);
                        ActionTypes nextAction              = convertStringToActionType(dicParams["na"]);
                        string strResponse                  = convertKeyValuesToString(dicParams);
                        responseMessage.Append(strResponse);

                        //히스토리보관 및 초기화
                        if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                            addActionHistory(strGlobalUserID, "doBonus", strResponse, index, counter);

                        result.NextAction = nextAction;
                        if (!betInfo.HasRemainResponse)
                            betInfo.RemainReponses = null;

                        if (isEnded)
                        {
                            realWin = double.Parse(dicParams["tw"]);
                            strGameLog = strResponse;
                            if (realWin > 0.0f)
                            {
                                _dicUserHistory[strGlobalUserID].baseBet    = betInfo.TotalBet;
                                _dicUserHistory[strGlobalUserID].win        = realWin;
                            }
                            else
                            {
                                saveHistory(strUserID, agentID, ind, counter, userBalance);
                            }
                        }
                        saveBetResultInfo(strGlobalUserID);
                    }
                }
                if(realWin == 0.0)
                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
                else
                    Sender.Tell(new ToUserResultMessage((int)_gameID, responseMessage, 0.0, realWin, new GameLogInfo(this.GameName, "0", strGameLog),UserBetTypes.Normal));

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

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set" };
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
    }
}
