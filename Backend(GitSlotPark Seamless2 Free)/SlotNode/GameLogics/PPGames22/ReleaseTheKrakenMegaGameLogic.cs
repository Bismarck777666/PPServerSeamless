using Akka.Actor;
using GITProtocol;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using PCGSharp;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Google.Protobuf;

namespace SlotGamesNode.GameLogics
{
    public class ReleaseTheKrakenMegaResult : BasePPSlotSpinResult
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
    class ReleaseTheKrakenMegaBetInfo : BasePPSlotBetInfo
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

    class ReleaseTheKrakenMegaGameLogic : BasePPSlotGame
    {
        protected double[] _multiTotalFreeSpinWinRates;
        protected double[] _multiMinFreeSpinWinRates;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vswayskrakenmw";
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
                return 7;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=4,8,10,10,5,11,6,12,7,12,7,9,13,10,3,3,3,8,13,13,9,11,7,11,13,13,13,3,3,8,13,13,13,6,5,11,13,13,13,13,13,10&cfgs=1&ver=3&def_sb=6,8,8,7,4,9&reel_set_size=8&def_sa=3,9,3,11,5,8&scatters=1~0,0,0,0,0,0~0,0,0,0,0,0~1,1,1,1,1,1&rt=d&gameInfo={rtps:{ante:\"96.53\",purchase_1:\"96.52\",purchase_0:\"96.51\",regular:\"96.40\"},props:{max_rnd_sim:\"1\",prizes:\"s:1,s:2,s:3,s:4,w,m:1,m:2,mw:2000,mw:5000,mw:10000\",max_rnd_hr:\"19607800\",max_rnd_win:\"10000\",max_rnd_win_a:\"8000\",max_rnd_hr_a:\"12658200\"}}&wl_i=tbm~10000;tbm_a~8000&sc=5.00,10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1,1&wilds=2~0,0,0,0,0,0~1,1,1,1,1,1&bonuses=0&bls=20,25&ntp=0.00&paytable=0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;150,60,40,20,10,0;80,40,20,15,0,0;60,30,15,10,0,0;40,20,10,8,0,0;20,15,8,6,0,0;16,10,6,4,0,0;16,10,6,4,0,0;16,10,6,4,0,0;12,7,5,3,0,0;12,7,5,3,0,0;0,0,0,0,0,0;0,0,0,0,0,0&total_bet_max=15,000,000.00&reel_set0=11,12,9,4,3,9,12,11,9,9,11,11,11,3,8,5,10,7,8,11,7,5,9,9,9,12,11,12,12,9,12,8,11,12,11,7,3,3,3,8,7,4,4,3,5,7,11,12,7,9,7,7,7,7,10,6,6,12,10,7,10,11,5,5,7,12,12,12,12,11,11,8,3,10,7,4,5,1,6,9,9~9,9,10,4,3,9,6,8,12,4,11,7,7,7,5,9,11,3,3,2,5,9,9,4,12,10,9,9,9,9,12,9,6,8,10,10,7,11,12,10,8,3,3,3,6,12,4,9,5,8,12,10,10,2,7,8,8,8,8,9,8,4,1,6,2,12,11,10,11,2,7,11,11,11,11,6,7,8,10,12,4,11,11,7,12,4,3,2~7,5,7,5,7,4,4,4,4,11,8,3,9,11,5,9,9,9,12,1,12,10,3,9,7,7,7,8,4,11,3,8,11,11,11,3,12,9,10,5,5,3,3,3,3,10,5,10,6,2,11,10,10,10,4,9,9,2,4,7,12,12,12,12,8,10,2,11,4,4,5~11,5,2,12,2,12,4,10,9,3,7,5,7,7,7,7,3,10,3,12,3,6,11,3,11,10,10,11,8,10,5,5,5,10,4,5,4,12,12,3,3,12,8,5,4,8,10,12,12,12,4,5,8,11,4,9,10,8,12,6,7,12,10,7,4,4,4,12,7,7,12,10,9,10,4,11,5,12,8,11,3,3,3,3,8,6,5,6,3,5,7,12,10,12,12,6,5,12,8,8,8,8,8,4,12,9,12,7,11,8,9,1,9,10,9,9,7,11,11,11,11,4,5,8,9,4,11,9,9,1,7,5,12,9,12,11~9,9,11,8,4,12,7,5,5,5,6,10,1,10,8,11,3,11,9,6,6,6,6,11,1,11,7,8,9,4,2,9,11,4,4,4,8,2,6,5,10,12,5,11,7,7,7,4,8,6,9,4,5,8,12,11,11,11,4,5,6,5,12,11,12,7,7,5,10,10,10,7,10,3,9,5,4,6,3,12,12,12,12,6,9,11,4,6,10,8,12,7,8,6~1,5,6,4,12,5,5,8,12,6,9,9,9,3,9,9,11,5,9,3,5,3,5,12,10,5,5,5,11,11,10,8,5,3,5,5,9,5,5,4,4,4,4,9,9,6,3,9,6,7,5,9,8,12,10,7,7,7,11,7,9,12,11,4,5,6,7,5,3,5,11,11,11,4,7,11,11,4,12,7,7,9,9,1,6,6,6,6,11,5,10,7,2,1,4,4,11,6,6,9,10,10,10,10,11,5,6,4,7,12,7,3,6,3,11,7,12,12,12,12,10,11,12,10,10,8,11,10,6,7,7,8,8,8,9,12,4,12,11,4,3,8,4,6,8,12,3,3,3,3,3,9,4,9,8,11,8,2,12,9,11,12,5,7&reel_set2=8,10,11,8,6,9,9,7,9,9,9,12,10,11,11,8,4,10,6,8,10,10,10,12,12,4,11,7,7,3,6,9,9,12,12,12,4,7,9,5,7,12,5,3,5,11,11,11,10,6,12,9,11,11,6,9,10,12,10~5,8,7,3,12,8,12,12,9,12,4,4,12,6,8,11,4,10,9,4,12,12,12,5,9,9,8,6,8,4,4,10,10,4,12,5,9,6,11,11,9,10,7,8,8,8,8,6,5,3,10,3,10,10,11,10,6,9,9,4,11,7,10,12,9,12,10,9,10,4~10,9,7,11,12,7,4,11,5,4,4,4,4,3,5,5,12,3,11,9,3,8,3,9,9,9,7,11,7,8,10,8,11,8,11,11,11,5,9,9,5,12,7,10,10,12,7,10,3,3,3,10,5,6,6,7,8,5,9,4,4,10,10,10,10,6,11,5,4,8,12,8,9,10,3,12,12,12,11,3,11,12,4,11,8,9,11,5,4,6~3,12,5,6,4,3,3,7,7,7,7,5,12,9,9,10,5,7,7,5,5,5,10,12,12,3,10,9,9,5,12,12,12,5,11,12,11,12,7,12,4,12,4,4,4,10,9,10,10,7,8,4,7,3,3,3,3,8,11,5,8,6,5,8,3,8,8,8,8,8,7,4,5,11,12,11,6,8,11,11,11,12,10,11,9,4,12,5,12,8,9~11,3,5,9,10,5,5,5,9,10,5,8,10,3,3,6,6,6,6,12,12,11,4,11,7,12,3,3,3,3,8,5,9,12,4,7,4,4,4,11,11,7,7,11,5,4,11,11,11,11,6,4,8,12,3,12,10,10,10,10,6,6,10,11,3,3,6,12,12,12,8,7,9,5,9,11,11,9~9,12,9,3,6,12,7,5,11,4,8,9,9,11,11,4,12,11,9,9,9,12,9,5,3,11,5,7,10,3,3,8,4,4,5,9,3,12,6,10,12,12,12,7,8,7,7,5,9,12,6,8,10,11,12,7,11,1,10,5,3,9,9,12&reel_set1=5,10,12,10,5,3,9,12,11,11,11,5,11,6,7,8,7,9,8,4,6,9,9,9,7,5,3,11,9,12,10,9,4,3,3,3,3,4,12,12,8,5,11,7,8,7,11,7,7,7,7,6,7,11,9,4,7,9,11,3,12,12,12,12,3,9,11,12,11,9,3,11,7,11,3~10,4,12,4,12,4,4,4,12,9,11,9,10,11,9,9,9,9,8,4,6,12,9,12,12,12,5,11,10,12,9,7,3,3,3,11,12,12,3,10,11,8,8,8,8,9,6,4,4,8,5,11,11,11,11,3,8,9,6,10,3,12,4~7,5,12,12,4,4,4,4,11,11,8,6,4,8,9,9,9,4,4,11,3,12,7,7,7,4,10,7,3,11,11,11,9,8,10,9,8,3,3,3,5,11,10,5,3,10,10,10,10,4,3,10,12,7,5,12,12,12,12,12,7,9,9,3,12,5,9~12,12,11,6,12,3,7,7,7,7,5,5,12,9,8,10,10,9,5,5,5,11,9,11,11,12,10,8,12,12,12,6,9,10,9,5,7,12,3,4,4,4,7,12,5,3,8,6,3,3,3,3,5,11,7,8,10,4,5,4,8,8,8,8,8,10,11,12,7,4,9,12,11,11,11,11,4,4,12,10,4,12,7,8,9~5,9,11,7,5,5,5,4,5,11,8,12,4,6,6,6,6,10,3,3,7,11,3,3,3,3,12,3,11,9,12,4,4,4,5,11,11,7,4,7,7,7,9,5,6,4,8,6,11,11,11,11,3,9,7,8,4,10,10,10,10,7,12,12,11,10,3,12,12,12,12,11,9,6,10,10,5~10,3,11,7,5,5,5,4,11,3,6,4,9,9,9,4,10,5,9,3,9,3,3,3,3,3,6,9,11,9,5,6,4,4,4,4,5,8,12,7,12,7,7,7,11,12,5,7,3,11,11,11,12,11,8,9,7,6,6,6,6,12,3,10,12,11,6,10,10,10,10,4,4,8,7,6,5,12,12,12,12,10,12,4,9,9,5,3&reel_set4=5,8,8,5,12,8,5,12,1,12,10~9,1,12,12,11,11,6,6~1,5,10,10,3,3,8,8~9,9,1,11,3,3,8,6,6~3,8,5,3,10,10,3,1,5,10,10,8,10,5,1~9,6,6,6,6,1,10,10,6,8,6&purInit=[{bet:2000,type:\"default\"},{bet:3000,type:\"default\"}]&reel_set3=12,7,5,9,11,5,7,11,11,11,6,7,11,5,9,11,12,11,9,9,9,3,7,5,11,8,8,3,10,3,3,3,3,9,10,7,4,8,3,4,12,7,7,7,7,11,11,7,8,7,5,11,12,12,12,12,9,4,3,12,3,9,9,1,6~12,4,3,11,3,3,10,11,3,3,3,3,11,2,9,12,8,4,6,12,10,9,9,9,6,5,9,10,2,5,11,6,12,8,8,8,8,3,4,8,4,12,10,10,9,9,11,11,11,11,10,9,12,1,2,9,8,9,7,3,4~7,4,5,4,11,9,9,9,3,5,11,12,7,7,10,9,4,4,4,4,3,10,8,3,8,7,9,5,7,7,7,9,5,7,11,4,3,9,8,11,11,11,12,7,6,11,9,5,6,3,3,3,11,4,10,8,5,12,7,5,10,10,10,10,4,5,9,1,11,4,8,10,12,12,12,12,8,4,5,3,12,5,11,9,10~5,4,5,12,12,11,7,7,7,7,4,11,4,3,3,10,6,10,5,5,5,3,8,7,4,6,9,9,5,12,12,12,9,11,1,11,12,9,12,7,8,4,4,4,10,10,4,2,8,9,8,4,3,3,3,3,11,10,5,6,12,11,7,8,8,8,8,8,9,4,10,12,12,3,10,11,11,11,11,9,12,3,2,12,5,5,7,5,12~6,8,3,8,4,10,5,5,5,9,4,7,12,11,11,4,7,9,9,9,6,5,5,4,1,11,8,9,6,6,6,6,3,7,4,7,5,11,3,3,3,3,11,5,4,9,5,10,11,11,4,4,4,7,5,11,11,3,8,8,12,7,7,7,12,10,9,5,11,7,11,11,11,11,5,3,10,9,12,6,9,10,10,10,10,11,3,6,12,11,4,3,12,12,12,12,9,10,12,6,9,12,5,9,7~11,8,5,5,5,10,5,3,6,11,9,9,9,12,5,11,12,3,3,3,3,3,8,9,3,10,10,4,4,4,4,9,2,1,4,7,7,7,10,9,9,7,7,11,11,11,4,9,10,7,12,6,6,6,6,5,6,12,3,7,10,10,10,10,5,4,6,3,11,12,12,12,12,8,11,11,6,7,12,9&reel_set6=6,11,12,9,9,7,9,9,9,11,4,7,12,9,10,5,6,3,3,3,3,12,1,9,11,11,10,12,4,7,7,7,7,8,10,4,9,8,12,5,8,11,11,11,3,10,3,3,7,5,11,7,12,12,12,12,11,7,3,9,3,7,5,11,8~6,3,4,4,11,6,9,10,12,12,4,4,4,10,5,9,4,12,7,6,3,4,9,8,12,9,9,9,11,4,10,6,5,1,8,8,12,2,11,4,3,3,3,8,12,4,8,11,12,11,9,10,9,10,10,8,8,8,8,4,10,3,3,4,9,9,2,10,9,11,8,11,11,11,11,6,7,2,12,9,12,1,8,11,5,10,9~7,9,10,2,5,11,4,4,4,4,8,4,3,1,12,1,9,9,9,10,5,7,2,10,8,3,7,7,7,5,2,8,10,5,12,12,10,11,11,11,4,12,4,9,8,5,11,3,3,3,4,8,10,11,5,6,3,10,10,10,10,11,10,9,11,6,9,11,12,12,12,12,4,11,9,7,3,7,7,5,5~5,4,6,9,10,8,2,9,9,7,7,7,7,1,12,10,10,3,11,5,12,12,11,12,5,5,5,4,12,4,3,9,10,3,12,5,8,12,12,12,10,8,7,12,3,7,4,4,7,11,3,4,4,4,6,10,5,8,8,7,11,10,11,9,5,3,3,3,3,9,12,5,4,10,7,9,5,8,9,8,8,8,8,8,5,5,12,4,9,11,12,12,3,12,12,11,11,11,11,7,6,10,10,1,4,9,6,8,11,12,7~2,12,9,11,5,5,5,10,6,9,7,7,9,6,6,6,6,12,3,5,11,8,3,3,3,3,7,4,3,6,7,5,4,4,4,11,11,9,11,10,7,7,7,8,1,10,6,4,11,11,11,11,4,11,12,5,10,10,10,12,3,12,3,8,5,12,12,12,12,5,4,9,5,11,1,4~9,7,1,5,6,7,9,11,8,11,9,9,9,11,4,3,12,9,10,7,11,6,6,5,5,5,3,3,7,11,10,7,3,6,12,4,12,3,3,3,3,3,7,3,4,9,12,11,11,12,12,1,7,4,4,4,4,2,5,8,6,10,7,10,10,5,3,4,7,7,7,6,3,12,5,7,5,5,6,11,11,9,11,11,11,7,2,4,9,9,11,11,5,3,4,9,6,6,6,6,8,7,6,10,10,12,11,7,9,9,5,10,10,10,10,3,9,8,3,8,1,3,7,5,5,3,12,12,12,12,11,8,3,12,4,9,11,9,4,6,8,8,8,5,4,12,12,5,9,10,4,6,5,11,5&reel_set5=8,7,12,3,7,12,5,3,12,8,10,10,8,3,5,5,10,7~9,4,11,7,6,4,11,12,4,11,9,12,7,9,9,7,12,11,12,7,6,4,6,6~3,8,3,3,8,10,10,5,8,10,3,10,8,3,3,8,5,3,8,3,8,3,8,3,10,8,5,5,8,5~8,3,4,6,8,8,11,4,4,3,9,9,11,3,11,6,9,6,8,6,11,9,3,4~3,8,5,5,3,8,10,8,8,10,3,3,10,3,10,3,3,5,10,3,10,3,5,5,10,8,8,10,10,3,5,3,10,10~3,6,9,6,8,6,9,8,8,10,6,3,6,9,6,6,6,6,9,6,8,6,10,3,10,9,6,8,3,6,10,3,10,6&reel_set7=11,5,3,12,11,8,11,9,9,9,3,9,7,5,9,6,3,3,3,3,9,12,4,3,11,11,3,12,7,7,7,7,8,3,8,7,7,4,7,11,11,11,6,5,7,10,10,4,11,9,5,3~9,12,12,11,11,12,4,4,4,5,4,10,12,10,9,3,2,7,9,9,9,4,12,9,6,11,9,4,6,12,12,12,9,12,10,5,3,8,6,7,3,3,3,9,2,12,9,8,3,11,8,10,8,8,8,8,10,4,10,10,11,8,12,4,9,11,11,11,4,12,12,6,10,5,8,4,4,6~9,12,8,12,8,7,3,11,4,4,4,4,3,2,9,11,10,11,4,10,6,9,9,9,5,9,8,9,7,6,7,7,10,7,7,7,4,3,4,8,11,8,7,3,12,3,3,3,5,3,4,5,12,5,5,3,4,10,10,10,10,2,5,4,7,11,11,10,5,10,9,3~12,7,4,5,4,5,9,12,12,12,4,4,7,8,10,7,6,8,7,7,7,7,12,11,3,3,8,11,10,12,5,5,5,10,12,5,3,7,11,12,9,4,4,4,12,9,5,12,9,5,10,10,3,3,3,3,8,6,12,9,6,12,3,8,8,8,8,7,12,9,10,3,4,5,7,12~11,12,8,4,5,5,5,10,9,3,12,7,5,6,6,6,6,10,9,7,3,3,3,3,11,7,11,7,11,9,4,4,4,4,3,9,11,4,3,7,7,7,6,11,12,8,8,5,10,10,10,11,4,5,5,12,11,11,11,11,10,4,3,12,4,7,6~11,5,3,4,7,9,3,5,5,5,9,10,11,4,11,7,11,5,8,9,9,9,7,9,6,6,3,7,3,6,11,3,3,3,3,3,11,5,3,7,12,9,3,3,4,4,4,4,5,12,6,11,6,6,5,5,7,7,7,8,12,4,9,4,5,12,10,7,6,6,6,6,4,10,8,5,9,10,4,4,8,10,10,10,10,3,7,12,10,11,9,9,3,4,7&total_bet_min=100.00";
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
            get { return 1.25; }
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
            get { return new double[] { 100, 150 }; }
        }

        #endregion
        public ReleaseTheKrakenMegaGameLogic()
        {
            _gameID     = GAMEID.ReleaseTheKrakenMega;
            GameName    = "ReleaseTheKrakenMega";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "6";
	        dicParams["bl"] = "0";
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
                dicParams["index"]      = index.ToString();
                dicParams["counter"]    = (counter + 1).ToString();
            }

            ActionTypes nextAction = convertStringToActionType(dicParams["na"]);
            if (isInit)
            {
                dicParams["na"]     = convertActionTypeToString(spinResult.NextAction);
                dicParams["action"] = convertActionTypeToFullString(spinResult.NextAction);

                if (dicParams.ContainsKey("g"))
                {
                    var gParam = JToken.Parse(dicParams["g"]);
                    int bg0End = -1, bg1End = -1;
                    if (gParam["bg_0"] != null)
                    {
                        if (gParam["bg_0"]["end"].ToString() == "1")
                            bg0End = 1;
                        else
                            bg0End = 0;
                    }

                    if (gParam["bg_1"] != null)
                    {
                        if (gParam["bg_1"]["end"].ToString() == "1")
                            bg1End = 1;
                        else
                            bg1End = 0;
                    }

                    if (bg0End == 1 && bg1End == 0)
                    {
                        dicParams["action"] = convertActionTypeToFullString(ActionTypes.DOSPIN);
                    }
                }
            }
            else
            {
                dicParams["na"] = convertActionTypeToString(spinResult.NextAction);
            }

            dicParams["balance"]        = Math.Round(userBalance - (isInit ? 0.0 : betMoney), 2).ToString();        //밸런스
            dicParams["balance_cash"]   = Math.Round(userBalance - (isInit ? 0.0 : betMoney), 2).ToString();        //밸런스케시

            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = "0";
            else
                dicParams.Remove("puri");

            if (SupportMoreBet)
            {
                if (betInfo.MoreBet)
                    dicParams["bl"] = "1";
                else
                    dicParams["bl"] = "0";
            }
            if (isInit)
                supplementInitResult(dicParams, betInfo, spinResult);

            overrideSomeParams(betInfo, dicParams);
            writeRoundID(betInfo, dicParams);

            return convertKeyValuesToString(dicParams);
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


                ReleaseTheKrakenMegaBetInfo betInfo = new ReleaseTheKrakenMegaBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();

                if (message.DataNum >= 4)
                {
                    betInfo.PurchaseFree = true;
                    betInfo.PurchaseType = (int)message.GetData(3);
                }
                else
                    betInfo.PurchaseFree = false;

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in ReleaseTheKrakenMegaGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                    (oldBetInfo as ReleaseTheKrakenMegaBetInfo).PurchaseType = betInfo.PurchaseType;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in ReleaseTheKrakenMegaGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            ReleaseTheKrakenMegaBetInfo betInfo = new ReleaseTheKrakenMegaBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new ReleaseTheKrakenMegaBetInfo();
        }
        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst, PPFreeSpinInfo freeSpinInfo)
        {
            try
            {
                ReleaseTheKrakenMegaResult spinResult = new ReleaseTheKrakenMegaResult();
                Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(dicParams, betInfo.TotalBet);

                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                spinResult.NextAction = convertStringToActionType(dicParams["na"]);
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
                _logger.Error("Exception has been occurred in ReleaseTheKrakenMegaGameLogic::calculateResult {0}", ex);
                return null;
            }
        }
        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            ReleaseTheKrakenMegaResult result = new ReleaseTheKrakenMegaResult();
            result.SerializeFrom(reader);
            return result;
        }
        private int getStatusLength(string strStatus)
        {
            string[] strParts = strStatus.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            return strParts.Length;
        }
        private bool isStatusAllZero(string strStatus)
        {
            string[] strParts = strStatus.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < strParts.Length; i++)
            {
                if (strParts[i] != "0")
                    return false;
            }
            return true;
        }
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency, bool isAffiliate)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                double realWin      = 0.0;
                string strGameLog   = "";
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                ToUserResultMessage resultMsg = null;
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) ||
                    !_dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    ReleaseTheKrakenMegaResult result       = _dicUserResultInfos[strGlobalUserID] as ReleaseTheKrakenMegaResult;
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

                        int ind = -1;

                        if (dicParams.ContainsKey("g"))
                        {
                            var gParam = JToken.Parse(dicParams["g"]);
                            int bg0End = -1;
                            if (gParam["bg_0"] != null)
                            {
                                if (gParam["bg_0"]["end"].ToString() == "1")
                                    bg0End = 1;
                                else
                                    bg0End = 0;
                            }

                            if (gParam["bg_0"]["status"] != null && !isStatusAllZero((string)gParam["bg_0"]["status"]))
                            {
                                ind = (int)message.Pop();
                                int statusLength = getStatusLength((string)gParam["bg_0"]["status"]);
                                if (statusLength == 12)
                                {
                                    if (result.BonusSelections == null)
                                        result.BonusSelections = new List<int>();

                                    if (result.BonusSelections.Contains(ind))
                                    {
                                        betInfo.pushFrontResponse(actionResponse);
                                        saveBetResultInfo(strGlobalUserID);
                                        throw new Exception(string.Format("{0} User selected already selected position, Malicious Behavior {1}", strGlobalUserID, ind));
                                    }

                                    result.BonusSelections.Add(ind);
                                    int[] status = new int[statusLength];
                                    for (int i = 0; i < result.BonusSelections.Count; i++)
                                        status[result.BonusSelections[i]] = i + 1;
                                    gParam["bg_0"]["status"] = string.Join(",", status);

                                    if (gParam["bg_0"]["wins"] != null)
                                    {
                                        string[] strWins = ((string)gParam["bg_0"]["wins"]).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                        string[] strNewWins = new string[statusLength];
                                        for (int i = 0; i < statusLength; i++)
                                            strNewWins[i] = "0";
                                        for (int i = 0; i < result.BonusSelections.Count; i++)
                                            strNewWins[result.BonusSelections[i]] = strWins[i];
                                        gParam["bg_0"]["wins"] = string.Join(",", strNewWins);
                                    }

                                    if (gParam["bg_0"]["wins_mask"] != null)
                                    {
                                        string[] strWinsMask    = ((string)gParam["bg_0"]["wins_mask"]).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                        string[] strNewWinsMask = new string[statusLength];
                                        for (int i = 0; i < statusLength; i++)
                                            strNewWinsMask[i] = "h";
                                        for (int i = 0; i < result.BonusSelections.Count; i++)
                                            strNewWinsMask[result.BonusSelections[i]] = strWinsMask[i];
                                        gParam["bg_0"]["wins_mask"] = string.Join(",", strNewWinsMask);
                                    }
                                }
                                else
                                {
                                    int[] status = new int[statusLength];
                                    status[ind] = 1;
                                    gParam["bg_0"]["status"] = string.Join(",", status);

                                    if (gParam["bg_0"]["wins"] != null)
                                    {
                                        string[] strWins = ((string)gParam["bg_0"]["wins"]).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                        if (ind != 0)
                                        {
                                            string strTemp  = strWins[ind];
                                            strWins[ind]    = strWins[0];
                                            strWins[0]      = strTemp;
                                        }
                                        gParam["bg_0"]["wins"] = string.Join(",", strWins);
                                    }
                                    if (gParam["bg_0"]["wins_mask"] != null)
                                    {
                                        string[] strWinsMask = ((string)gParam["bg_0"]["wins_mask"]).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                        if (ind != 0)
                                        {
                                            string strTemp      = strWinsMask[ind];
                                            strWinsMask[ind]    = strWinsMask[0];
                                            strWinsMask[0]      = strTemp;
                                        }
                                        gParam["bg_0"]["wins_mask"] = string.Join(",", strWinsMask);
                                    }
                                }
                            }
                            dicParams["g"] = JsonConvert.SerializeObject(gParam);
                        }
                        
                        result.BonusResultString = convertKeyValuesToString(dicParams);
                        addDefaultParams(dicParams, userBalance, index, counter);

                        ActionTypes nextAction  = convertStringToActionType(dicParams["na"]);
                        string      strResponse = convertKeyValuesToString(dicParams);

                        responseMessage.Append(strResponse);

                        //히스토리보관 및 초기화
                        if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                        {
                            if (ind >= 0)
                                addIndActionHistory(strGlobalUserID, "doBonus", strResponse, index, counter, ind);
                            else
                                addActionHistory(strGlobalUserID, "doBonus", strResponse, index, counter);
                        }

                        if (nextAction == ActionTypes.DOCOLLECT || nextAction == ActionTypes.DOCOLLECTBONUS)
                        {
                            realWin = double.Parse(dicParams["tw"]);
                            strGameLog = strResponse;
                            if (realWin > 0.0f)
                            {
                                _dicUserHistory[strGlobalUserID].baseBet = betInfo.TotalBet;
                                _dicUserHistory[strGlobalUserID].win     = realWin;
                            }

                            resultMsg = new ToUserResultMessage((int)_gameID, responseMessage, 0.0, realWin, new GameLogInfo(this.GameName, "0", strGameLog), UserBetTypes.Normal); ;
                            resultMsg.BetTransactionID  = betInfo.BetTransactionID;
                            resultMsg.RoundID           = betInfo.RoundID;
                            resultMsg.TransactionID     = createTransactionID();
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
                    Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
                else
                    Sender.Tell(resultMsg);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in ReleaseTheKrakenMegaGameLogic::onDoBonus {0}", ex);
            }
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "s", "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set" };
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

        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as ReleaseTheKrakenMegaBetInfo).PurchaseType;
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
                _multiMinFreeSpinWinRates   = new double[FreePurCount];

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
            int purchaseType = (betInfo as ReleaseTheKrakenMegaBetInfo).PurchaseType;
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
                int purchaseType = (betInfo as ReleaseTheKrakenMegaBetInfo).PurchaseType;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, -1, PurchaseMultiples[purchaseType] * 0.2, PurchaseMultiples[purchaseType] * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in ReleaseTheKrakenMegaGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int purchaseType = (betInfo as ReleaseTheKrakenMegaBetInfo).PurchaseType;
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in ReleaseTheKrakenMegaGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = (betInfo as ReleaseTheKrakenMegaBetInfo).PurchaseType.ToString();
        }
        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            ReleaseTheKrakenMegaBetInfo starBetInfo = betInfo as ReleaseTheKrakenMegaBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, -1, betInfo.PurchaseFree ? starBetInfo.PurchaseType : -1, betMoney);
        }
    }
}
