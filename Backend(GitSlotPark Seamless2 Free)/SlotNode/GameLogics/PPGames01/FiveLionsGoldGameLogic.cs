using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using System.IO;
using Akka.Actor;
using Microsoft.Extensions.Logging;

namespace SlotGamesNode.GameLogics
{
    public class FiveLionsGoldBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get { return this.BetPerLine * 18.0f; }
        }
    }
    public class FiveLionsGoldResult : BasePPSlotSpinResult
    {
        public List<int> BonusSelections { get; set; }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            BonusSelections = SerializeUtils.readIntList(reader);
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            SerializeUtils.writeIntList(writer, BonusSelections);
        }
    }
    class FiveLionsGoldGameLogic : BaseSelFreePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs243lionsgold";
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
                return 243;
            }
        }
        protected override int ServerResLineCount
        {
            get { return 18; }
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
                return "def_s=4,3,7,7,4,11,5,6,7,11,10,5,9,8,8&cfgs=3001&ver=2&reel_set_size=8&def_sb=7,11,8,10,6&def_sa=6,3,5,8,11&bonusInit=[{bgid:0,bgt:33,bg_i:\"15,150,2000\",bg_i_mask:\"wp,wp,wp\"},{bgid:2,bgt:18,bg_i:\"2000,150,15\",bg_i_mask:\"pw,pw,pw\"}]&wrlm_sets=2~0~2,3,5~1~3,5,8~2~5,8,10~3~8,10,15~4~10,15,30~5~15,30,40&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&fs_aw=t;n&gmb=0,0,0&rt=d&base_aw=t;n&sc=10.00,20.00,30.00,50.00,80.00,100.00,200.00,500.00,1000.00,2000.00,3000.00,4000.00,6000.00&defc=100.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;126,45,15,2,0;81,30,12,0,0;81,30,12,0,0;63,24,9,0,0;63,24,9,0,0;36,12,4,0,0;36,12,4,0,0;27,9,3,0,0;27,9,3,0,0;18,6,2,0,0;18,6,2,0,0&rtp=95.5,95.46,95.45,95.41,95.39,95.37,94.95&reel_set0=9,8,10,5,10,13,5,7,13,8,12,5,4,7,7,3,4,9,9,8,5,5,12,3,3,6,6,4,4,11,11,13,4,8,8,13,3,9,7,11,13,7,6,11,7,12,9,13,13,3,8,6,12,13,4,11,7,8,11,10,10,3,6,12,10,12,5,6,11,10,6,10,13,9,10,7,12,8,12,11,9,11,8,9,13,12,5,9,6,10,4~4,6,9,9,1,11,11,13,2,7,8,8,12,6,10,2,3,13,7,11,9,3,9,12,8,6,5,5,3,7,11,13,8,1,5,8,9,4,6,10,4,4,13,13,1,5,10,10,3,6,9,11,1,10,6,4,12,2,6,6,4,9,13,8,1,3,10,12,12,11,5,3,12,13,2,4,12,7,7,10,13,9,2,11,5,5,9,8,1,6,7,11,2,10,12,10,8,1,7,4,13,12,7,8~4,12,9,13,11,6,2,7,4,4,5,8,3,12,9,9,1,8,11,8,10,11,2,12,4,3,10,11,12,5,1,8,6,4,13,13,13,11,11,2,6,8,3,7,10,5,5,3,10,7,6,6,4,1,3,6,7,10,9,13,1,12,5,12,12,2,8,8,5,12,11,9,2,6,7,13,13,4,6,2,7,13,12,3,6,7,7,10,10,8,1,5,10,8,7,13,9,9,13,11,1,9,9,10,11~13,6,13,7,7,10,6,1,8,9,9,6,10,2,12,13,10,10,8,11,2,12,6,5,13,7,2,5,9,5,4,1,11,11,11,12,12,4,7,4,6,8,1,9,13,13,12,6,3,10,5,10,7,3,8,4,9,3,4,6,6,12,1,8,8,12,7,3,10,10,9,11,2,12,10,5,9,2,13,11,8,11,11,2,13,9,5,5,3,12,9,8,11,3,7,4,4,13,6,7,13,8,12,4,5,7,11~13,8,10,13,5,13,5,5,9,11,10,6,13,6,10,10,11,9,9,13,13,11,6,3,8,10,7,7,5,3,12,7,10,4,12,12,11,8,11,13,12,4,6,8,11,6,6,3,8,12,7,13,4,6,9,3,12,4,4,9,12,11,11,7,8,8,10,5,4,12,3,13,7,11,6,9,8,7,3,9,10,5,9,7,4,12,10,9,8&reel_set2=9,8,10,5,10,13,5,7,7,13,8,12,12,5,4,7,3,4,9,8,5,12,3,6,4,11,13,4,8,13,3,9,7,11,13,7,6,11,7,12,9,13,3,8,8,6,12,13,13,4,11,7,8,11,10,3,6,12,10,12,5,5,6,6,11,10,6,10,13,9,10,7,12,8,12,11,9,11,8,9,13,12,5,9,6,10,4~4,6,9,1,11,11,13,2,7,8,12,6,10,2,3,13,7,11,9,3,9,12,8,6,6,5,5,3,3,7,11,13,8,1,5,8,9,4,6,10,4,13,13,1,5,10,3,6,9,9,11,1,10,6,4,12,2,6,6,4,9,13,8,1,3,10,12,12,11,5,3,12,13,13,13,2,4,12,7,7,10,13,9,2,11,5,5,9,8,1,6,7,11,2,10,12,10,8,1,7,4,13,12,7,8~4,12,9,13,11,6,2,7,4,5,8,3,12,9,1,8,11,8,10,11,2,12,4,3,10,11,12,5,1,8,6,4,13,11,11,2,6,8,3,7,10,5,5,3,10,10,7,6,4,1,3,6,7,10,9,13,1,12,5,12,2,8,5,12,11,9,2,6,7,13,13,4,6,6,2,7,13,12,3,6,7,10,8,1,5,10,8,7,7,13,9,9,13,11,1,9,9,10,11~13,6,13,7,10,6,1,8,9,6,10,2,12,13,10,8,11,2,12,6,5,13,7,2,5,9,5,4,1,11,12,12,4,7,4,6,8,8,1,9,13,13,6,3,10,5,10,7,3,8,4,9,3,4,6,12,1,8,12,7,7,3,10,10,9,9,11,2,12,10,5,9,2,13,11,8,11,2,13,9,5,3,12,9,8,11,3,7,4,4,13,6,7,13,8,12,4,5,7,11~13,8,10,13,5,13,5,5,9,11,10,6,13,6,10,11,9,13,11,11,6,3,8,10,10,7,5,3,12,7,10,4,4,12,11,8,11,13,12,4,6,8,11,6,6,6,3,8,12,7,7,13,4,6,9,3,12,4,9,9,12,11,7,8,10,5,4,12,3,13,7,11,6,9,8,7,3,9,10,5,9,7,4,12,10,9,8&t=243&reel_set1=9,8,10,5,10,13,5,7,7,13,8,12,12,5,4,7,3,4,9,8,5,12,3,6,4,11,13,4,8,13,3,9,7,11,13,7,6,11,7,12,9,13,3,8,8,6,12,13,13,4,11,7,8,11,10,3,6,12,10,12,5,5,6,6,11,10,6,10,13,9,10,7,12,8,12,11,9,11,8,9,13,12,5,9,6,10,4~4,6,9,1,11,11,13,2,7,8,12,6,10,2,3,13,7,11,9,3,9,12,8,6,6,5,5,3,3,7,11,13,8,1,5,8,9,4,6,10,4,13,13,1,5,10,3,6,9,9,11,1,10,6,4,12,2,6,6,4,9,13,8,1,3,10,12,12,11,5,3,12,13,13,13,2,4,12,7,7,10,13,9,2,11,5,5,9,8,1,6,7,11,2,10,12,10,8,1,7,4,13,12,7,8~4,12,9,13,11,6,2,7,4,5,8,3,12,9,1,8,11,8,10,11,2,12,4,3,10,11,12,5,1,8,6,4,13,11,11,2,6,8,3,7,10,5,5,3,10,10,7,6,4,1,3,6,7,10,9,13,1,12,5,12,2,8,5,12,11,9,2,6,7,13,13,4,6,6,2,7,13,12,3,6,7,10,8,1,5,10,8,7,7,13,9,9,13,11,1,9,9,10,11~13,6,13,7,10,6,1,8,9,6,10,2,12,13,10,8,11,2,12,6,5,13,7,2,5,9,5,4,1,11,12,12,4,7,4,6,8,8,1,9,13,13,6,3,10,5,10,7,3,8,4,9,3,4,6,12,1,8,12,7,7,3,10,10,9,9,11,2,12,10,5,9,2,13,11,8,11,2,13,9,5,3,12,9,8,11,3,7,4,4,13,6,7,13,8,12,4,5,7,11~13,8,10,13,5,13,5,5,9,11,10,6,13,6,10,11,9,13,11,11,6,3,8,10,10,7,5,3,12,7,10,4,4,12,11,8,11,13,12,4,6,8,11,6,6,6,3,8,12,7,7,13,4,6,9,3,12,4,9,9,12,11,7,8,10,5,4,12,3,13,7,11,6,9,8,7,3,9,10,5,9,7,4,12,10,9,8&reel_set4=9,8,10,5,10,13,5,7,7,13,8,12,12,5,4,7,3,4,9,8,5,12,3,6,4,11,13,4,8,13,3,9,7,11,13,7,6,11,7,12,9,13,3,8,8,6,12,13,13,4,11,7,8,11,10,3,6,12,10,12,5,5,6,6,11,10,6,10,13,9,10,7,12,8,12,11,9,11,8,9,13,12,5,9,6,10,4~4,6,9,1,11,11,13,2,7,8,12,6,10,2,3,13,7,11,9,3,9,12,8,6,6,5,5,3,3,7,11,13,8,1,5,8,9,4,6,10,4,13,13,1,5,10,3,6,9,9,11,1,10,6,4,12,2,6,6,4,9,13,8,1,3,10,12,12,11,5,3,12,13,13,2,4,12,7,7,10,13,9,2,11,5,5,9,8,1,6,7,11,2,10,12,10,8,1,7,4,13,12,7,8~4,12,9,13,11,6,2,7,4,5,8,3,12,9,1,8,11,8,10,11,2,12,4,3,10,11,12,5,1,8,6,4,13,11,11,2,6,8,3,7,10,5,5,3,10,10,7,6,4,1,3,6,7,10,9,13,1,12,5,12,2,8,5,12,11,9,2,6,7,13,13,4,6,6,2,7,13,12,3,6,7,10,8,1,5,10,8,7,7,13,9,9,13,11,1,9,9,10,11~13,6,13,7,10,6,1,8,9,6,10,2,12,13,10,8,11,2,12,6,5,13,7,2,5,9,5,4,1,11,12,12,4,7,4,6,8,8,1,9,13,13,6,3,10,5,10,7,3,8,4,9,3,4,6,12,1,8,12,7,7,3,10,10,9,9,11,2,12,10,5,9,2,13,11,8,11,2,13,9,5,3,12,9,8,11,3,7,4,4,13,6,7,13,8,12,4,5,7,11~13,8,10,13,5,13,5,5,9,11,10,6,13,6,10,11,9,13,11,11,6,3,8,10,10,7,5,3,12,7,10,4,4,12,11,8,11,13,12,4,6,8,11,6,6,3,8,12,7,7,13,4,6,9,3,12,4,9,9,12,11,7,8,10,5,4,12,3,13,7,11,6,9,8,7,3,9,10,5,9,7,4,12,10,9,8&reel_set3=9,8,10,5,10,13,5,7,7,13,8,12,12,5,4,7,3,4,9,8,5,12,3,6,4,11,13,4,8,13,3,9,7,11,13,7,6,11,7,12,9,13,3,8,8,6,12,13,13,4,11,7,8,11,10,3,6,12,10,12,5,5,6,6,11,10,6,10,13,9,10,7,12,8,12,11,9,11,8,9,13,12,5,9,6,10,4~4,6,9,1,11,11,13,2,7,8,12,6,10,2,3,13,7,11,9,3,9,12,8,6,6,5,5,3,3,7,11,13,8,1,5,8,9,4,6,10,4,13,13,1,5,10,3,6,9,9,11,1,10,6,4,12,2,6,6,4,9,13,8,1,3,10,12,12,11,5,3,12,13,13,13,2,4,12,7,7,10,13,9,2,11,5,5,9,8,1,6,7,11,2,10,12,10,8,1,7,4,13,12,7,8~4,12,9,13,11,6,2,7,4,5,8,3,12,9,1,8,11,8,10,11,2,12,4,3,10,11,12,5,1,8,6,4,13,11,11,2,6,8,3,7,10,5,5,3,10,10,7,6,4,1,3,6,7,10,9,13,1,12,5,12,2,8,5,12,11,9,2,6,7,13,13,4,6,6,2,7,13,12,3,6,7,10,8,1,5,10,8,7,7,13,9,9,13,11,1,9,9,10,11~13,6,13,7,10,6,1,8,9,6,10,2,12,13,10,8,11,2,12,6,5,13,7,2,5,9,5,4,1,11,12,12,4,7,4,6,8,8,1,9,13,13,6,3,10,5,10,7,3,8,4,9,3,4,6,12,1,8,12,7,7,3,10,10,9,9,11,2,12,10,5,9,2,13,11,8,11,2,13,9,5,3,12,9,8,11,3,7,4,4,13,6,7,13,8,12,4,5,7,11~13,8,10,13,5,13,5,5,9,11,10,6,13,6,10,11,9,13,11,11,6,3,8,10,10,7,5,3,12,7,10,4,4,12,11,8,11,13,12,4,6,8,11,6,6,6,3,8,12,7,7,13,4,6,9,3,12,4,9,9,12,11,7,8,10,5,4,12,3,13,7,11,6,9,8,7,3,9,10,5,9,7,4,12,10,9,8&reel_set6=9,8,10,5,10,13,5,7,7,13,8,12,12,5,4,7,3,4,9,8,5,12,3,6,4,11,13,4,8,13,3,9,7,11,13,7,6,11,7,12,9,13,3,8,8,6,12,13,13,4,11,7,8,11,10,3,6,12,10,12,5,5,6,6,11,10,6,10,13,9,10,7,12,8,12,11,9,11,8,9,13,12,5,9,6,10,4~4,6,9,1,11,11,13,2,7,8,12,6,10,2,3,13,7,11,9,3,9,12,8,6,6,5,5,3,3,7,11,13,8,1,5,8,9,4,6,10,4,13,13,1,5,10,3,6,9,9,11,1,10,6,4,12,2,6,6,4,9,13,8,1,3,10,12,12,11,5,3,12,13,13,2,4,12,7,7,10,13,9,2,11,5,5,9,8,1,6,7,11,2,10,12,10,8,1,7,4,13,12,7,8~4,12,9,13,11,6,2,7,4,5,8,3,12,9,1,8,11,8,10,11,2,12,4,3,10,11,12,5,1,8,6,4,13,11,11,2,6,8,3,7,10,5,5,3,10,10,7,6,4,1,3,6,7,10,9,13,1,12,5,12,2,8,5,12,11,9,2,6,7,13,13,4,6,6,2,7,13,12,3,6,7,10,8,1,5,10,8,7,7,13,9,9,13,11,1,9,9,10,11~13,6,13,7,10,6,1,8,9,6,10,2,12,13,10,8,11,2,12,6,5,13,7,2,5,9,5,4,1,11,12,12,4,7,4,6,8,8,1,9,13,13,6,3,10,5,10,7,3,8,4,9,3,4,6,12,1,8,12,7,7,3,10,10,9,9,11,2,12,10,5,9,2,13,11,8,11,2,13,9,5,3,12,9,8,11,3,7,4,4,13,6,7,13,8,12,4,5,7,11~13,8,10,13,5,13,5,5,9,11,10,6,13,6,10,11,9,13,11,11,6,3,8,10,10,7,5,3,12,7,10,4,4,12,11,8,11,13,12,4,6,8,11,6,6,3,8,12,7,7,13,4,6,9,3,12,4,9,9,12,11,7,8,10,5,4,12,3,13,7,11,6,9,8,7,3,9,10,5,9,7,4,12,10,9,8&reel_set5=9,8,10,5,10,13,5,7,7,13,8,12,12,5,4,7,3,4,9,8,5,12,3,6,4,11,13,4,8,13,3,9,7,11,13,7,6,11,7,12,9,13,3,8,8,6,12,13,13,4,11,7,8,11,10,3,6,12,10,12,5,5,6,6,11,10,6,10,13,9,10,7,12,8,12,11,9,11,8,9,13,12,5,9,6,10,4~4,6,9,1,11,11,13,2,7,8,12,6,10,2,3,13,7,11,9,3,9,12,8,6,6,5,5,3,3,7,11,13,8,1,5,8,9,4,6,10,4,13,13,1,5,10,3,6,9,9,11,1,10,6,4,12,2,6,6,4,9,13,8,1,3,10,12,12,11,5,3,12,13,13,2,4,12,7,7,10,13,9,2,11,5,5,9,8,1,6,7,11,2,10,12,10,8,1,7,4,13,12,7,8~4,12,9,13,11,6,2,7,4,5,8,3,12,9,1,8,11,8,10,11,2,12,4,3,10,11,12,5,1,8,6,4,13,11,11,2,6,8,3,7,10,5,5,3,10,10,7,6,4,1,3,6,7,10,9,13,1,12,5,12,2,8,5,12,11,9,2,6,7,13,13,4,6,6,2,7,13,12,3,6,7,10,8,1,5,10,8,7,7,13,9,9,13,11,1,9,9,10,11~13,6,13,7,10,6,1,8,9,6,10,2,12,13,10,8,11,2,12,6,5,13,7,2,5,9,5,4,1,11,12,12,4,7,4,6,8,8,1,9,13,13,6,3,10,5,10,7,3,8,4,9,3,4,6,12,1,8,12,7,7,3,10,10,9,9,11,2,12,10,5,9,2,13,11,8,11,2,13,9,5,3,12,9,8,11,3,7,4,4,13,6,7,13,8,12,4,5,7,11~13,8,10,13,5,13,5,5,9,11,10,6,13,6,10,11,9,13,11,11,6,3,8,10,10,7,5,3,12,7,10,4,4,12,11,8,11,13,12,4,6,8,11,6,6,3,8,12,7,7,13,4,6,9,3,12,4,9,9,12,11,7,8,10,5,4,12,3,13,7,11,6,9,8,7,3,9,10,5,9,7,4,12,10,9,8&reel_set7=9,8,10,5,10,13,5,7,7,13,8,12,12,5,4,7,3,4,9,8,5,12,3,6,4,11,13,4,8,13,3,9,7,11,13,7,6,11,7,12,9,13,3,8,8,6,12,13,13,4,11,7,8,11,10,3,6,12,10,12,5,5,6,6,11,10,6,10,13,9,10,7,12,8,12,11,9,11,8,9,13,12,5,9,6,10,4~4,6,9,1,11,11,13,2,7,8,12,6,10,2,3,13,7,11,9,3,9,12,8,6,6,5,5,3,3,7,11,13,8,1,5,8,9,4,6,10,4,13,13,1,5,10,3,6,9,9,11,1,10,6,4,12,2,6,6,4,9,13,8,1,3,10,12,12,11,5,3,12,13,13,2,4,12,7,7,10,13,9,2,11,5,5,9,8,1,6,7,11,2,10,12,10,8,1,7,4,13,12,7,8~4,12,9,13,11,6,2,7,4,5,8,3,12,9,1,8,11,8,10,11,2,12,4,3,10,11,12,5,1,8,6,4,13,11,11,2,6,8,3,7,10,5,5,3,10,10,7,6,4,1,3,6,7,10,9,13,1,12,5,12,2,8,5,12,11,9,2,6,7,13,13,4,6,6,2,7,13,12,3,6,7,10,8,1,5,10,8,7,7,13,9,9,13,11,1,9,9,10,11~13,6,13,7,10,6,1,8,9,6,10,2,12,13,10,8,11,2,12,6,5,13,7,2,5,9,5,4,1,11,12,12,4,7,4,6,8,8,1,9,13,13,6,3,10,5,10,7,3,8,4,9,3,4,6,12,1,8,12,7,7,3,10,10,9,9,11,2,12,10,5,9,2,13,11,8,11,2,13,9,5,3,12,9,8,11,3,7,4,4,13,6,7,13,8,12,4,5,7,11~13,8,10,13,5,13,5,5,9,11,10,6,13,6,10,11,9,13,11,11,6,3,8,10,10,7,5,3,12,7,10,4,4,12,11,8,11,13,12,4,6,8,11,6,6,3,8,12,7,7,13,4,6,9,3,12,4,9,9,12,11,7,8,10,5,4,12,3,13,7,11,6,9,8,7,3,9,10,5,9,7,4,12,10,9,8";
            }
        }

        protected override int FreeSpinTypeCount
        {
            get { return 7; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            return new int[] { 200, 201, 202, 203, 204, 205, 206 };
        }
        #endregion

        public FiveLionsGoldGameLogic()
        {
            _gameID = GAMEID.FiveLionsGold;
            GameName = "FiveLionsGold";
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            FiveLionsGoldBetInfo betInfo = new FiveLionsGoldBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new FiveLionsGoldBetInfo();
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


                FiveLionsGoldBetInfo betInfo = new FiveLionsGoldBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in FiveLionsGoldGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine = betInfo.BetPerLine;
                    oldBetInfo.LineCount = betInfo.LineCount;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in FiveLionsGoldGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["n_reel_set"] = "0";
            dicParams["awt"]        = "rsf";
        }

        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst, PPFreeSpinInfo freeSpinInfo)
        {
            try
            {
                FiveLionsGoldResult         spinResult  = new FiveLionsGoldResult();
                Dictionary<string, string>  dicParams   = splitResponseToParams(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(dicParams, betInfo.TotalBet);

                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                if (SupportPurchaseFree && betInfo.PurchaseFree && isFirst)
                    dicParams["purtr"] = "1";

                spinResult.NextAction   = convertStringToActionType(dicParams["na"]);
                if (spinResult.NextAction == ActionTypes.DOCOLLECT || spinResult.NextAction == ActionTypes.DOCOLLECTBONUS)
                    spinResult.TotalWin = double.Parse(dicParams["tw"]);
                else
                    spinResult.TotalWin = 0.0;

                if (freeSpinInfo != null)
                {
                    
                    dicParams["fra"] = Math.Round(freeSpinInfo.TotalWin, 2).ToString();
                    dicParams["frn"] = freeSpinInfo.RemainCount.ToString();
                    dicParams["frt"] = "N";
                }
                spinResult.ResultString = convertKeyValuesToString(dicParams);
                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in FiveLionsGoldGameLogic::calculateResult {0}", ex);
                return null;
            }
        }
        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            FiveLionsGoldResult result = new FiveLionsGoldResult();
            result.SerializeFrom(reader);
            return result;
        }
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency, bool isAffiliate)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                int ind     = -1;
                if (message.DataNum > 0)
                    ind = (int)message.Pop();

                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                double              realWin     = 0.0;
                string              strGameLog  = "";
                ToUserResultMessage resultMsg   = null;
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) ||
                    !_dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    FiveLionsGoldResult     result          = _dicUserResultInfos[strGlobalUserID] as FiveLionsGoldResult;
                    BasePPSlotBetInfo       betInfo         = _dicUserBetInfos[strGlobalUserID];
                    BasePPActionToResponse  actionResponse  = betInfo.pullRemainResponse();
                    if (actionResponse.ActionType != ActionTypes.DOBONUS)
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        Dictionary<string, string> dicParams = splitResponseToParams(actionResponse.Response);

                        convertWinsByBet(dicParams, betInfo.TotalBet);
                        convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                        //0인 경우 필요없음
                        if (ind >= 0 && dicParams.ContainsKey("bg_i") && dicParams.ContainsKey("level"))
                        {
                            int level = int.Parse(dicParams["level"]);
                            if (level >= 1)
                            {
                                if(result.BonusSelections == null)  
                                    result.BonusSelections = new List<int>();                           

                                if (result.BonusSelections.Contains(ind))
                                {
                                    betInfo.pushFrontResponse(actionResponse);
                                    saveBetResultInfo(strGlobalUserID);
                                    throw new Exception(string.Format("{0} User selected already selected position, Malicious Behavior {1}", strGlobalUserID, ind));
                                }
                                result.BonusSelections.Add(ind);
                                if (dicParams.ContainsKey("status"))
                                {
                                    int[] status = new int[9];
                                    for (int i = 0; i < result.BonusSelections.Count; i++)
                                        status[result.BonusSelections[i]] = i + 1;
                                    dicParams["status"] = string.Join(",", status);
                                }
                                if (dicParams.ContainsKey("wins"))
                                {
                                    string[] strWins = dicParams["wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    string[] strNewWins = new string[9];
                                    for (int i = 0; i < 9; i++)
                                        strNewWins[i] = "0";
                                    for (int i = 0; i < result.BonusSelections.Count; i++)
                                        strNewWins[result.BonusSelections[i]] = strWins[i];
                                    dicParams["wins"] = string.Join(",", strNewWins);
                                }
                                if (dicParams.ContainsKey("wins_mask"))
                                {
                                    string[] strWinsMask = dicParams["wins_mask"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    string[] strNewWinsMask = new string[9];
                                    for (int i = 0; i < 9; i++)
                                        strNewWinsMask[i] = "h";
                                    for (int i = 0; i < result.BonusSelections.Count; i++)
                                        strNewWinsMask[result.BonusSelections[i]] = strWinsMask[i];
                                    dicParams["wins_mask"] = string.Join(",", strNewWinsMask);
                                }
                            }
                        }

                        result.BonusResultString    = convertKeyValuesToString(dicParams);
                        addDefaultParams(dicParams, userBalance, index, counter);
                        ActionTypes nextAction      = convertStringToActionType(dicParams["na"]);
                        string strResponse          = convertKeyValuesToString(dicParams);

                        responseMessage.Append(strResponse);

                        //히스토리보관 및 초기화
                        if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                            addActionHistory(strGlobalUserID, "doBonus", strResponse, index, counter);

                        if (nextAction == ActionTypes.DOCOLLECT || nextAction == ActionTypes.DOCOLLECTBONUS)
                        {
                            realWin = double.Parse(dicParams["tw"]);
                            strGameLog = strResponse;

                            if (realWin > 0.0f)
                            {
                                _dicUserHistory[strGlobalUserID].baseBet = betInfo.TotalBet;
                                _dicUserHistory[strGlobalUserID].win = realWin;
                            }

                            resultMsg = new ToUserResultMessage((int)_gameID, responseMessage, 0.0, realWin, new GameLogInfo(this.GameName, "0", strGameLog), UserBetTypes.Normal); ;
                            resultMsg.BetTransactionID = betInfo.BetTransactionID;
                            resultMsg.RoundID = betInfo.RoundID;
                            resultMsg.TransactionID = createTransactionID();
                            if (_dicUserFreeSpinInfos.ContainsKey(strGlobalUserID) && !_dicUserFreeSpinInfos[strGlobalUserID].Pending)
                            {
                                resultMsg.FreeSpinID = _dicUserFreeSpinInfos[strGlobalUserID].FreeSpinID;
                                addFreeSpinBonusParams(responseMessage, _dicUserFreeSpinInfos[strGlobalUserID], strGlobalUserID, realWin);
                            }
                        }
                        copyBonusParamsToResult(dicParams, result);
                        result.NextAction = nextAction;
                    }
                    if (!betInfo.HasRemainResponse)
                        betInfo.RemainReponses = null;

                    saveBetResultInfo(strGlobalUserID);
                }
                if (resultMsg == null)
                    Context.System.Scheduler.ScheduleTellOnce(250, Sender, new ToUserMessage((int)_gameID, responseMessage), Self);
                else
                    Context.System.Scheduler.ScheduleTellOnce(250, Sender, resultMsg, Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in FiveLionsGoldGameLogic::onDoBonus {0}", ex);
            }
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul","fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total","n_reel_set",
                "s", "purtr", "w", "tw", "awt" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]) || resultParams.ContainsKey(toCopyParams[i]))
                    continue;

                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }
            if (!resultParams.ContainsKey("na") || resultParams["na"] != "fso")
            {
                resultParams.Remove("fs_opt");
                resultParams.Remove("fs_opt_mask");
            }
            return resultParams;
        }
    }
}
