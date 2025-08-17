using Akka.Actor;
using Akka.Util;
using GITProtocol;
using Newtonsoft.Json.Linq;
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
    class DownTheRailsResult : BasePPSlotSpinResult
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
    class DownTheRailsGameLogic : BaseSelFreePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20underground";
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
                return 3;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=3,9,7,4,11,6,11,3,7,9,4,10,9,4,8&cfgs=6260&ver=3&mo_s=16;3;4;5;6;7&mo_v=1,2,3,4,5,10,15,20,25,30,35,40,45,50,100,200,300,500,1000,2500;10,20,50;5,10,30;3,6,15;2,4,10;1,2,5&def_sb=4,10,6,5,8&reel_set_size=9&def_sa=3,11,10,3,8&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{kc_cw_win_chance:\"66.02\",max_rnd_sim:\"1\",pn_kc_win_chance:\"79.34\",bp_eol_win_chance:\"50.59\",max_rnd_hr:\"10009000\",max_rnd_win:\"5000\",cw_bp_win_chance:\"56.82\"}}&wl_i=tbm~5000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&wilds=2~500,100,40,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;500,100,40,0,0;250,80,20,0,0;250,80,20,0,0;200,60,10,0,0;200,60,10,0,0;100,40,8,0,0;80,20,6,0,0;60,10,4,0,0;50,10,4,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=7,5,8,6,3,6,10,6,11,8,9,10,10,3,4,11,8,3,3,3,3,9,7,10,11,7,6,9,7,6,5,3,11,3,9,8,4,5,6,6,6,4,5,1,11,5,2,10,4,2,1,9,10,3,7,5,11,8,9,10,4~11,11,9,11,5,11,9,3,10,5,6,10,6,7,11,4,2,7,3,2,6,4,6,4,10,9,5,5,9,11,10,3,3,3,3,7,6,4,3,10,3,4,10,3,5,8,5,4,11,10,9,11,10,5,6,6,10,9,8,11,8,9,7,1,5,1,6,7~7,11,10,4,7,7,2,7,5,8,2,10,11,10,4,8,9,7,9,7,11,2,10,3,1,3,5,10,7,7,1,4,9,6,5,3,8,9,11,9,3,8,10,7,4,5,10,9,8,10,6,7,3,2,3,8,1,3,7,9,3,4,3,4,10,8,9,5,7,5,3,3,3,3,7,4,4,1,11,9,6,3,11,6,3,9,10,4,4,7,9,5,11,1,9,8,11,5,3,6,5,11,7,10,3,10,8,3,5,10,2,10,4,3,11,4,3,8,5,5,6,11,6,5,11,4,3,6,8,7,9,8,6,2,5,3,4,2,7,4,2,3,10,9,3~8,4,9,4,3,10,5,1,8,6,3,3,4,4,11,6,3,11,6,8,4,4,8,5,5,2,9,6,3,7,8,10,6,8,2,6,9,8,10,3,3,3,3,2,9,10,9,11,3,3,2,10,7,5,9,4,3,4,4,3,2,7,8,3,9,7,6,7,4,7,3,6,11,1,3,3,11,3,8,8,10,5,6~5,2,9,4,5,6,6,4,9,6,6,11,8,7,7,4,7,4,11,9,8,9,11,6,10,4,5,3,9,6,11,9,2,7,9,11,6,6,2,5,9,8,11,10,1,11,8,3,3,3,3,8,5,5,11,4,9,10,11,3,8,2,10,8,9,6,3,4,11,6,1,10,6,10,8,11,10,3,3,5,10,5,7,3,10,1,10,4,8,9,3,6,2,10,11,7,6,10,1,4,8&accInit=[{id:0,mask:\"el_c;el_l;el_t;el_a;xp_c;xp_l;xp_t;b_c;b_l;b_t;m_c;m_l;m_t;m_v\"}]&reel_set2=4,9,7,6,8,2,11,8,9,11,3,11,8,4,7,2,6,10,9,7,3,3,3,2,10,5,6,8,10,4,2,10,11,9,8,11,10,5,6,3,10,8,11,7,2~9,10,6,8,2,11,2,8,6,10,9,11,9,8,11,7,9,5,4,5,4,7,6,9,3,5,9,3,3,3,10,10,11,10,6,3,2,5,2,11,10,4,10,5,10,9,2,7,11,6,4,11,8,2,7,4,9,3~8,9,6,5,11,2,9,8,5,5,9,3,6,4,5,3,7,4,6,11,7,7,10,8,11,10,9,2,9,2,3,3,3,4,10,7,4,7,10,11,7,9,11,8,11,4,5,11,6,10,6,7,3,2,8,7,10,11,10,11,8,10~9,2,11,2,11,6,4,2,10,5,10,9,8,3,4,10,2,7,9,4,11,10,11,5,10,5,8,9,10,5,6,4,9,8,10,5,11,3,3,3,9,10,5,8,11,7,11,4,7,10,3,6,11,6,10,11,4,8,7,3,8,10,8,11,9,6,5,6,11,7,8,6,9,8,2,7,8,7,9~10,7,7,10,5,4,10,5,8,9,10,9,8,4,4,9,8,10,11,7,10,7,9,5,6,4,6,6,11,10,7,11,7,2,7,6,7,8,4,9,5,9,8,9,10,8,6,8,7,3,3,3,9,6,4,8,10,9,11,8,6,9,8,9,8,7,5,10,5,8,10,5,5,9,6,10,3,10,2,11,6,11,10,9,6,5,8,6,9,6,4,6,7,5,3,10,11,6,5,4,6,9,6,10,8,3,9&reel_set1=12,12,12,12,11,12,12,12,12,8,12,12,12,12,9,12,12,11,10,12,8,6,12,12,12,12,9,12,10,12,12,12,9,12,12,5,12,12,12,10,12,3,12,12,12,12,2,7,12,8,12,4,9~12,12,12,12,11,12,12,12,12,11,12,12,12,12,2,12,12,12,12,4,7,12,12,12,12,8,12,12,12,12,5,9,12,12,12,12,8,12,12,11,12,12,6,12,12,6,11,12,3,10,12,5,12~2,12,12,8,10,12,10,12,12,12,6,12,12,12,7,6,12,12,6,9,12,9,12,12,12,12,12,12,12,11,12,12,12,12,12,12,12,8,5,12,12,12,12,12,12,12,5,9,12~12,4,9,11,7,5,12,12,10,6,8,12,9,11,9,12,11,8,12,12,10,12,12,6,7,5,11,4,3,12,12,8,12,7,12,10,12,12,10,12,3,12,5,12,12,6,7,2,7,11,8,12,4,6,5,9~9,12,12,8,9,12,12,5,10,7,3,12,12,11,9,11,12,12,4,10,9,12,12,8,12,8,12,12,12,12,4,12,5,12,5,12,2,6,11,10,12,6,10,9,12,7,8,10,12,4,8,12,8,12,12,10,7,12,12&reel_set4=11,8,9,7,10,8,4,9,5,6,6,11,10,4,7,10,3,6,9,3,3,3,3,11,8,9,4,9,5,5,8,11,3,3,10,3,4,11,7,5,6,7,9,8,10~3,11,3,10,11,6,8,3,5,11,6,8,10,4,9,11,9,5,6,4,8,9,4,9,11,6,11,10,8,3,5,3,3,3,3,8,10,5,9,7,5,3,9,11,7,8,10,6,7,3,10,11,6,10,11,9,10,4,7,7,5,5,4,4,9,11,9,8,10,3,5,7,9~11,4,10,8,6,8,4,9,7,3,6,6,8,11,3,6,8,3,11,11,5,10,8,9,5,6,5,10,7,10,3,3,9,11,8,11,3,3,3,3,9,8,7,3,7,7,4,4,10,5,9,4,7,5,4,4,10,5,6,8,5,9,7,7,9,11,8,11,9,3,11,4,3,9,7,11,9,10,5,7~3,10,5,6,7,7,8,11,10,3,8,3,10,6,8,9,6,9,8,10,4,10,9,8,3,7,3,7,9,7,11,9,7,6,8,11,8,7,5,3,3,3,8,9,6,4,11,7,10,8,4,8,10,9,11,10,9,10,7,9,5,8,6,6,8,6,11,6,5,4,11,7,10,11,6,10,4,11,9,11,11,3,4~11,10,3,9,5,10,8,6,10,11,10,5,11,8,6,5,8,11,9,5,3,6,6,4,9,4,9,6,11,7,10,8,9,6,5,9,7,8,11,10,3,4,10,4,3,8,9,10,6,11,7,7,10,8,9,3,3,3,3,4,6,6,8,7,10,8,6,8,10,5,11,9,5,10,8,5,7,6,11,7,9,10,4,5,7,5,8,9,11,4,4,3,3,6,8,11,9,8,4,9,11,5,4,6,11,5,10,3,5,8,6,7,7,9&reel_set3=7,11,3,6,8,4,6,10,5,8,4,5,9,10,6,9,11,3,3,7,10,11,7,4,6,9,6,7,3,3,3,3,8,9,4,3,8,10,7,5,8,7,10,9,11,3,9,5,11,7,3,6,8,9,5,8,3,11,6,11,8,9,10,4~10,9,5,7,8,6,10,8,4,3,11,3,5,8,11,7,3,8,9,11,3,8,3,3,3,3,9,11,10,9,6,6,10,7,10,9,11,4,9,3,4,7,10,11,9,5,5,11,4,5,6~11,8,7,11,11,5,10,3,8,9,9,10,3,7,7,8,7,7,6,3,6,7,9,10,8,11,5,4,5,4,11,6,3,3,3,3,4,8,5,8,10,5,7,4,9,9,6,11,5,9,10,8,11,3,5,4,8,6,10,7,3,10,4,9,10,9,3,3,4,11~10,8,11,4,8,10,11,10,3,3,8,6,8,9,5,11,11,9,10,9,7,8,5,8,4,11,11,10,6,7,6,4,9,3,3,3,7,6,7,7,10,9,11,5,10,4,7,7,11,6,3,8,10,8,7,7,6,6,10,4,9,8,10,9,3,6,6,3,11~4,5,3,8,9,7,4,7,6,5,7,3,7,8,5,4,9,8,3,8,10,7,11,8,4,11,9,11,9,11,6,6,8,7,10,6,11,10,5,4,3,11,9,5,11,10,8,11,10,7,9,11,4,8,4,5,6,3,3,3,3,3,10,8,7,4,8,9,6,3,9,6,3,10,6,10,6,9,8,9,5,3,6,6,8,6,5,9,10,8,6,3,10,11,8,5,11,10,8,6,11,9,10,5,11,10,5,8,5,8,11,7,9,11,4,7,9,5,10,4&reel_set6=3,9,11,5,9,10,10,4,4,10,4,8,11,9,6,11,7,10,10,11,5,6,6,3,3,3,3,5,3,3,5,7,8,10,7,9,6,4,8,3,7,9,8,11,11,8,3,7,9,8,11,6~10,9,10,8,9,11,5,6,3,4,3,6,3,4,9,11,4,11,9,10,10,5,11,9,5,7,5,3,3,3,3,7,8,10,11,11,9,8,7,5,7,9,11,3,11,10,3,6,4,8,3,11,8,9,8,5,10,9,6,8~9,6,4,9,9,10,11,4,3,3,10,5,9,8,5,6,11,11,7,8,10,3,10,8,9,11,11,3,3,8,7,3,3,3,3,9,6,10,3,11,5,11,6,7,7,6,7,8,4,8,9,4,8,5,10,4,10,10,5,7,8,7,9,5,7,4,10,11~11,10,9,4,11,10,3,9,8,4,10,9,8,7,10,9,8,10,6,9,10,5,11,10,3,6,8,8,7,8,10,6,8,7,4,11,7,8,4,11,3,3,3,10,7,11,11,8,6,4,3,11,9,6,3,10,5,5,7,4,4,7,9,3,11,6,9,6,9,7,11,9,6,6,9,8,8,11,5,8,10,7,10,8,10~7,11,3,11,6,7,9,9,11,8,11,4,8,6,9,5,4,11,5,6,7,10,5,5,10,8,11,4,6,10,9,8,9,4,8,11,6,9,6,4,5,10,6,11,9,10,9,11,6,10,8,3,3,3,3,3,8,9,9,8,3,6,3,8,11,4,8,7,8,7,9,9,10,11,7,7,11,9,10,6,10,11,8,4,3,3,8,11,5,10,5,10,9,5,6,8,5,10,7,5,10,11,10,6,4,5,5,9,9,4&reel_set5=9,10,4,8,4,3,3,4,2,5,11,2,8,10,10,2,10,9,5,6,3,11,2,11,2,10,5,7,7,9,2,11,7,3,3,3,3,9,6,8,7,11,8,7,2,10,8,11,9,3,3,4,6,7,8,11,6,10,8,5,11,9,3,8,9,11,4,2,10,9,6,5~8,10,3,8,11,5,3,10,9,8,5,8,9,2,4,5,9,6,9,2,3,10,4,2,10,10,11,11,10,5,11,10,4,6,11,9,2,8,4,9,3,7,8,3,3,3,3,7,3,7,10,6,11,8,7,2,11,4,9,8,2,7,9,5,4,11,6,5,10,3,7,11,8,11,9,2,10,8,2,3,11,3,6,9,2,11,6,5,5~8,9,9,5,9,9,10,9,7,4,10,10,2,5,8,7,6,8,3,6,11,11,10,9,8,4,7,11,8,4,3,7,10,5,9,4,2,10,8,9,11,3,3,3,3,8,11,7,10,3,8,6,6,5,10,11,10,4,7,10,5,3,2,11,9,6,3,3,5,7,8,3,7,5,2,11,7,7,4,6,11,2,10,11,9,4,8,10~9,6,8,9,4,11,10,8,6,8,8,11,10,11,7,2,5,10,8,8,4,11,3,3,3,6,10,5,9,6,6,11,9,10,3,7,4,3,8,7,3,7,9,10,9,8,11,7,2,4~8,4,10,4,4,11,4,6,7,9,10,8,6,10,8,11,8,3,3,8,9,8,7,10,9,7,2,10,11,9,11,6,2,10,5,8,9,9,5,9,7,6,7,5,9,4,8,10,9,8,3,3,3,3,3,6,3,6,5,11,8,11,5,7,10,11,9,5,7,8,3,6,5,11,5,10,11,6,11,3,8,9,9,6,8,4,4,8,10,6,10,7,4,3,11,4,10,5,11,6,2,9,9,11,5,9&reel_set8=9,11,9,10,3,10,6,11,6,10,9,11,10,7,9,10,10,11,10,3,3,3,10,9,11,8,7,9,11,9,10,9,9,7,10,4,8,7,10,8,8,5,10,10,7,6,10,11,6,3,6,9,3,8,11,8,9,6,8,5,8,9,11,11,8~9,8,4,11,9,7,10,10,9,10,10,5,11,5,10,11,7,10,11,7,8,11,9,8,8,6,11,6,11,7,3,3,3,6,7,7,11,9,4,10,4,10,5,9,8,8,3,10,4,6,8,8,9,11,9,6,3,7,5,10,9,9,3,11,5,8~7,9,10,8,10,8,9,5,11,10,3,11,8,5,11,10,4,7,11,5,4,8,4,7,4,9,10,8,10,11,9,11,5,11,11,8,9,11,7,9,11,8,10,5,9,11,9,11,7,8,9,11,8,7,11,6,3,3,3,9,9,3,11,9,8,9,10,7,10,9,8,10,10,6,5,4,8,5,10,10,5,6,9,6,7,6,6,11,7,10,6,11,7,8,10,10,9,8,10,7,10,8,9,9,3,10,8,5,9,8,5,10,10,11,4,8~9,10,7,4,10,11,9,5,7,5,11,8,10,7,9,11,10,8,11,7,7,8,10,6,11,7,8,7,8,3,3,3,10,3,8,10,11,7,8,9,6,10,11,8,9,8,5,6,6,9,10,11,4,3,9,8,6,7,8,11,10,8,6~9,10,8,7,8,10,8,9,9,11,11,9,10,8,5,11,9,6,11,9,10,8,10,11,10,8,4,9,11,6,8,9,7,9,8,6,4,10,10,11,8,9,8,8,5,8,4,8,10,3,8,8,6,5,11,9,7,9,3,6,5,3,3,3,6,9,9,11,11,10,10,5,10,10,11,11,7,9,6,8,8,11,11,9,5,9,10,8,3,11,10,10,8,8,5,11,8,9,11,8,8,7,8,5,7,11,10,10,7,8,11,9,9,10,11,5,7,10,9,10,9,6,11,7,11,6,10,4&reel_set7=10,2,9,6,7,6,7,8,9,10,10,9,5,2,7,4,10,10,11,7,8,3,3,3,2,9,5,11,8,3,8,10,5,11,8,9,11,6,9,10,10,8,2,3,11,11,10,10,7,7,3,11,11,4,6,9,6,5,4,2,10,5,7,10,9,8,8,6,8,11,6~7,7,10,6,2,9,10,4,10,11,9,4,3,11,8,9,10,11,8,8,11,11,10,9,10,5,8,6,8,5,3,3,3,8,11,11,10,9,5,5,9,3,2,10,6,7,11,6,5,7,8,9,8,7,5,2,6,11,7,11,9,2,7,5,10,4~9,5,11,10,6,7,4,5,10,4,9,8,9,8,6,5,6,11,8,10,7,4,8,11,11,6,10,3,8,7,7,8,5,6,4,11,10,5,9,11,6,9,8,10,5,7,9,6,9,10,7,7,11,7,7,4,11,7,3,5,9,5,4,9,11,3,3,3,11,10,7,6,7,11,5,10,9,5,5,11,2,11,10,10,8,6,8,10,11,10,11,9,3,7,10,4,11,11,8,10,11,7,4,6,6,7,9,7,9,8,10,4,7,4,8,9,10,8,9,10,9,10,10,8,9,7,5,9,7,8,2,8,5,8,5,7~8,5,4,9,10,6,9,5,10,8,11,11,7,7,9,6,8,7,6,7,9,11,8,9,11,8,7,11,10,7,8,3,8,2,4,3,7,10,3,3,3,4,8,8,10,6,7,5,11,4,11,10,8,9,10,8,11,7,6,9,6,9,6,6,5,8,10,7,9,3,9,2,11,10,11,6,10,9,8,11,6,7~10,10,6,8,11,3,11,10,10,11,6,6,8,6,10,6,10,5,6,9,7,9,9,8,7,8,8,11,6,5,10,6,5,9,5,8,5,7,4,11,6,10,4,8,4,4,5,8,9,4,7,9,8,5,10,9,11,10,3,3,3,7,9,2,9,8,11,10,9,10,9,11,5,8,10,6,2,6,7,4,8,2,9,7,5,10,7,7,8,11,11,3,8,10,6,9,3,5,7,11,11,8,9,10,9,8,8,11,8,9,7,11,4,5,8,6,11,10,8,9,11,11&total_bet_min=10.00";
            }
        }
        protected override int FreeSpinTypeCount
        {
            get { return 5; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            switch (freeSpinGroup)
            {
                case 0:
                    return new int[] { 200, 201, 202, 203, 204 };
                case 1:
                    return new int[] { 201, 202, 203, 204 };
                case 2:
                    return new int[] { 202, 203, 204 };
                case 3:
                    return new int[] { 203, 204 };
                case 4:
                    return new int[] { 204 };
            }
            return new int[] { 200, 201, 202, 203, 204 };
        }
        #endregion
        public DownTheRailsGameLogic()
        {
            _gameID  = GAMEID.DownTheRails;
            GameName = "DownTheRails";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["g"] = "{eol_g:{def_s:\"14,14,14,14,14,14,14,14,14,14,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15\",def_sa:\"15,15,15,15,15\",def_sb:\"15,15,15,15,15\",sh:\"5\",st:\"rect\",sw:\"5\"}}";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
        }
        protected override void supplementInitResult(Dictionary<string, string> dicParams, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {
            base.supplementInitResult(dicParams, betInfo, spinResult);
            var oldGParam = JToken.Parse("{eol_g:{def_s:\"14,14,14,14,14,14,14,14,14,14,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15\",def_sa:\"15,15,15,15,15\",def_sb:\"15,15,15,15,15\",sh:\"5\",st:\"rect\",sw:\"5\"}}");
            if (dicParams.ContainsKey("g"))
            {
                var gParam = JToken.Parse(dicParams["g"]);
                if (gParam["eol_g"] == null)
                {
                    gParam["eol_g"] = oldGParam["eol_g"];
                }
                else
                {
                    gParam["eol_g"]["def_s"]  = "14,14,14,14,14,14,14,14,14,14,15,15,15,15,15,15,15,15,15,15,15,15,15,15,15";
                    gParam["eol_g"]["def_sa"] = "15,15,15,15,15";
                    gParam["eol_g"]["def_sb"] = "15,15,15,15,15";
                }
                dicParams["g"] = serializeJsonSpecial(gParam);
            }
            else
            {
                dicParams["g"] = serializeJsonSpecial(oldGParam);
            }
            if (!dicParams.ContainsKey("reel_set"))
                dicParams["reel_set"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("g"))
            {
                var gParam = JToken.Parse(dicParams["g"]);
                if (gParam["eol_g"] != null && gParam["eol_g"]["mo_tw"] != null)
                {
                    gParam["eol_g"]["mo_tw"] = convertWinByBet(gParam["eol_g"]["mo_tw"].ToString(), currentBet);
                    dicParams["g"] = serializeJsonSpecial(gParam);
                }                
            }
            if (dicParams.ContainsKey("pw"))
                dicParams["pw"] = convertWinByBet(dicParams["pw"], currentBet);

            if (dicParams.ContainsKey("mo_tw"))
                dicParams["mo_tw"] = convertWinByBet(dicParams["mo_tw"], currentBet);

            if (dicParams.ContainsKey("rs_win"))
                dicParams["rs_win"] = convertWinByBet(dicParams["rs_win"], currentBet);


        }
        protected override void onDoCollect(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    _logger.Error("{0} result information has not been found in DownTheRailsGameLogic::onDoCollect.", strGlobalUserID);
                    return;
                }

                BasePPSlotSpinResult result = _dicUserResultInfos[strGlobalUserID];
                if (result.NextAction != ActionTypes.DOCOLLECT)
                {
                    _logger.Error("{0} next action is not DOCOLLECT just {1} in DownTheRailsGameLogic::onDoCollect.", strGlobalUserID, result.NextAction);
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
                _logger.Error("Exception has been occurred in DownTheRailsGameLogic::onDoCollect {0}", ex);
            }
        }
        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst)
        {
            try
            {
                DownTheRailsResult          spinResult  = new DownTheRailsResult();
                Dictionary<string, string>  dicParams   = splitResponseToParams(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(dicParams, betInfo.TotalBet);
                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);
                if (SupportPurchaseFree && betInfo.PurchaseFree && isFirst)
                    dicParams["purtr"] = "1";

                spinResult.NextAction = convertStringToActionType(dicParams["na"]);
                spinResult.ResultString = convertKeyValuesToString(dicParams);
                if (spinResult.NextAction == ActionTypes.DOCOLLECT || spinResult.NextAction == ActionTypes.DOCOLLECTBONUS)
                    spinResult.TotalWin = double.Parse(dicParams["tw"]);
                else
                    spinResult.TotalWin = 0.0;

                if (spinResult.NextAction == ActionTypes.DOBONUS)
                    spinResult.DoBonusCounter = 0;

                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in DownTheRailsGameLogic::calculateResult {0}", ex);
                return null;
            }
        }
        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            DownTheRailsResult result = new DownTheRailsResult();
            result.SerializeFrom(reader);
            return result;
        }        
        protected string buildTrail(int index)
        {
            switch(index)
            {
                case 0:
                    return "fs_t~pv;fs_gmb_p~kc";
                case 1:
                    return "fs_t~kc;fs_gmb_p~cw";
                case 2:
                    return "fs_t~cw;fs_gmb_p~bp";
                case 3:
                    return "fs_t~bp;fs_gmb_p~eol";
                case 4:
                    return "init_mo~0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;fs_t~eol";
            }
            return "";
        }
        protected Dictionary<string, string> buildDoBonusResponse(DownTheRailsResult result, BasePPSlotBetInfo betInfo, BasePPSlotStartSpinData startSpinData, int doBonusCounter, int ind, bool isNextMove)
        {
            Dictionary<string, string> dicParams            = new Dictionary<string, string>();
            Dictionary<string, string> dicLastParams        = splitResponseToParams(result.BonusResultString);
            Dictionary<string, string> dicLastResultParams  = splitResponseToParams(result.ResultString);

            JToken gParam = JToken.Parse(dicLastResultParams["g"]);
            if (!string.IsNullOrEmpty(result.BonusResultString))
                gParam = JToken.Parse(dicLastParams["g"]);

            if (gParam["gmb"]["ch_h"] == null)
                gParam["gmb"]["ch_h"] = string.Format("0~{0}", ind);
            else
                gParam["gmb"]["ch_h"] = string.Format("{1},0~{0}", ind, (string)gParam["gmb"]["ch_h"]);

            dicParams["tw"] = dicLastResultParams["tw"];
            int bonusIndex = result.DoBonusCounter + startSpinData.FreeSpinGroup;
            if (ind == 1 || (isNextMove && bonusIndex == 4))
            {
                if(bonusIndex < 4)
                {
                    int freeSpinCount = 5;
                    if (bonusIndex == 1)
                        freeSpinCount = 6;
                    else if (bonusIndex >= 2)
                        freeSpinCount = 8;
                    dicParams["fsmul"] = "1";
                    dicParams["fsmax"] = freeSpinCount.ToString();
                    dicParams["fswin"] = "0.00";
                    dicParams["fs"]    = "1";
                    dicParams["fsres"] = "0.00";
                }
                else
                {
                    dicParams["rs_p"] = "0";
                    dicParams["rs_c"] = "1";
                    dicParams["rs_m"] = "3";
                }
                dicParams["na"]         = "s";
                dicParams["trail"]      = buildTrail(bonusIndex);
                gParam["gmb"]["end"] = "1";
                dicParams["g"] = serializeJsonSpecial(gParam);
                return dicParams;
            }
            else
            {
                if (!isNextMove)
                {
                    dicParams["trail"]   = buildTrail(bonusIndex);
                    gParam["gmb"]["end"] = "1";
                    dicParams["na"]      = "cb";

                    int[] minOdds = new int[] { 4, 5, 6, 8    };
                    int[] maxOdds = new int[] { 8, 11, 18, 24 };
                    int   apv     = Pcg.Default.Next(minOdds[bonusIndex], maxOdds[bonusIndex] + 1);

                    double lastWin       = double.Parse(dicLastResultParams["tw"]);
                    dicParams["tw"]      = Math.Round(lastWin + betInfo.TotalBet * apv, 2).ToString();
                    dicParams["apv"]     = (apv * 20).ToString();
                    dicParams["apt"]     = "ma";
                    dicParams["apwa"]    = Math.Round(betInfo.TotalBet * apv, 2).ToString();
                }
                else
                {
                    dicParams["trail"] = buildTrail(bonusIndex);
                    dicParams["na"] = "b";
                }
                dicParams["g"] = serializeJsonSpecial(gParam);
                return dicParams;
            }
        }
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                int             index           = (int)message.Pop();
                int             counter         = (int)message.Pop();
                double          realWin         = 0.0;
                string          strGameLog      = "";
                GITMessage      responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                string          strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                ToUserResultMessage resultMsg   = null;
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BasePPSlotBetInfo  betInfo = _dicUserBetInfos[strGlobalUserID];
                    DownTheRailsResult result  = _dicUserResultInfos[strGlobalUserID] as DownTheRailsResult;
                    if ((result.NextAction != ActionTypes.DOBONUS) || (betInfo.SpinData == null) || !(betInfo.SpinData is BasePPSlotStartSpinData))
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        bool isCollect = false;
                        bool isEnded   = false;
                        Dictionary<string, string> dicParams = null;

                        int ind             = (int)message.Pop();
                        var startSpinData   = betInfo.SpinData as BasePPSlotStartSpinData;
                        int stage           = startSpinData.FreeSpinGroup + result.DoBonusCounter;
                        do
                        {
                            if (ind == 1)
                            {
                                dicParams = buildDoBonusResponse(result, betInfo, startSpinData, result.DoBonusCounter, 1, true);
                                isCollect = true;
                                break;
                            }
                            double[] moveProbs = new double[] { 0.7928, 0.6624, 0.5652, 0.5057 };
                            if (betInfo.SpinData.IsEvent || Pcg.Default.NextDouble(0.0, 1.0) <= moveProbs[stage])
                            {
                                result.DoBonusCounter++;
                                dicParams = buildDoBonusResponse(result, betInfo, startSpinData, result.DoBonusCounter, 0, true);
                                if (startSpinData.FreeSpinGroup + result.DoBonusCounter == 4)
                                    isCollect = true;
                            }
                            else
                            {
                                dicParams           = buildDoBonusResponse(result, betInfo, startSpinData, result.DoBonusCounter, 0, false);
                                double selectedWin  = startSpinData.StartOdd * betInfo.TotalBet + double.Parse(dicParams["apwa"]);
                                double maxWin       = startSpinData.MaxOdd * betInfo.TotalBet;

                                if (!startSpinData.IsEvent)
                                    sumUpWebsiteBetWin(agentID, 0.0, selectedWin - maxWin);
                                else if (maxWin > selectedWin)
                                    addEventLeftMoney(agentID, strUserID, maxWin - selectedWin);
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
                            double selectedWin     = (startSpinData.StartOdd + freeSpinData.SpinOdd) * betInfo.TotalBet;
                            double maxWin          = startSpinData.MaxOdd * betInfo.TotalBet;

                            //시작스핀시에 최대의 오드에 해당한 윈값을 더해주었으므로 그 차분을 보상해준다.
                            if (!startSpinData.IsEvent)
                                sumUpWebsiteBetWin(agentID, 0.0, selectedWin - maxWin);
                            else if (maxWin > selectedWin)
                                addEventLeftMoney(agentID, strUserID, maxWin - selectedWin);
                        }
                        else
                        {
                            //필요없는 응답을 삭제한다.
                            betInfo.pullRemainResponse();
                        }

                        result.BonusResultString = convertKeyValuesToString(dicParams);
                        addDefaultParams(dicParams, userBalance, index, counter);
                        ActionTypes nextAction = convertStringToActionType(dicParams["na"]);
                        string strResponse = convertKeyValuesToString(dicParams);
                        responseMessage.Append(strResponse);

                        //히스토리보관 및 초기화
                        if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                            addIndActionHistory(strGlobalUserID, "doBonus", strResponse, index, counter, ind);

                        result.NextAction = nextAction;
                        if (!betInfo.HasRemainResponse)
                            betInfo.RemainReponses = null;

                        if (isEnded)
                        {
                            Dictionary<string, string> dicLastResultParams = splitResponseToParams(result.BonusResultString);
                            realWin = double.Parse(dicLastResultParams["tw"].ToString());
                            betInfo.RemainReponses = new List<BasePPActionToResponse>();
                            betInfo.RemainReponses.Add(new BasePPActionToResponse(ActionTypes.DOCOLLECTBONUS, "na=s"));
                            strGameLog = strResponse;
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
                            resultMsg.BetTransactionID = betInfo.BetTransactionID;
                            resultMsg.RoundID = betInfo.RoundID;
                            resultMsg.TransactionID = createTransactionID();
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
                _logger.Error("Exception has been occurred in DownTheRailsGameLogic::onDoBonus {0}", ex);
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

            if (spinParams.ContainsKey("g") && resultParams.ContainsKey("g"))
            {
                JToken gParam = JToken.Parse(spinParams["g"]);
                JToken gBonusParam = JToken.Parse(resultParams["g"]);
                gParam["gmb"] = gBonusParam["gmb"];
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
