using Akka.Actor;
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
    class PowerOfMerlinMegawaysResult : BasePPSlotSpinResult
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

    class PowerOfMerlinMegawaysGameLogic : BaseSelFreePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vswayspowzeus";
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
                return 0;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "cfgs=8003&ver=3&reel_set_size=24&scatters=1~0,0,0,0,0,0~0,0,0,0,0,0~1,1,1,1,1,1&rt=d&gameInfo={props:{gamble_lvl_4:\"76.58\",gamble_lvl_5:\"78.92\",max_rnd_sim:\"1\",max_rnd_hr:\"14285714\",max_rnd_win:\"40000\",gamble_lvl_2:\"69.37\",gamble_lvl_3:\"73.95\",gamble_lvl_1:\"63.10\"}}&wl_i=tbm~40000&reel_set10=reel_set5&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&reel_set11=reel_set5&reel_set12=7,8,8,11,9,8,11,12,9,5,10,9,6,11,11,10,5,11,6,10,12,12,11,3,3,4,8,7,11,7,12,10,10,7,8,12,7,5,7,8,10,10,9,1,11,10,12,10,6,3,4,8,10,4,9,8,7,8,9,8,7,9,12,12,2,10,9,10,9,11,11,7,10,3,3,12,10,13,10,2,9,8,11,8,2,6,4,10,12,3,6,7,12,8,9,6,11,6,3,10,3,5,7,12,13,8,5,1,2,4,7,11,10,7,12,8,5,2,12,11,11,9,5,11,4,4,10,2,10,2,11,8,9,11,11,6,3,4,11,11,10,7,6,8&purInit_e=1&reel_set13=reel_set12&wilds=2~0,0,0,0,0,0~1,1,1,1,1,1&bonuses=0&reel_set18=reel_set17&reel_set19=reel_set17&reel_set14=reel_set12&paytable=0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;400,200,100,20,10,0;100,50,40,10,0,0;50,30,20,10,0,0;40,15,10,5,0,0;30,12,8,4,0,0;25,10,8,4,0,0;20,10,5,3,0,0;18,10,5,3,0,0;16,8,4,2,0,0;12,8,4,2,0,0;0,0,0,0,0,0;0,0,0,0,0,0&reel_set15=12,7,10,10,12,9,10,10,8,13,12,9,7,4,12,10,11,3,8,4,8,4,10,10,9,7,11,13,11,5,9,7,13,9,11,13,4,2,4,10,5,12,2,7,6,11,10,3,9,8,13,3,11,9,11,11,10,8,7,12,3,6,7,8,5,3,5,11,7,6,7,8&reel_set16=reel_set15&reel_set17=5,7,5,3,3,9,9,3,11,11,5,3,3,3,3,9,7,5,11,11,7,5,7,3,5,7,11,5,5,5,5,9,3,9,9,11,5,9,11,3,7,3,9,7,7,7,7,5,11,9,9,5,11,9,9,3,11,7,9,9,9,9,7,9,9,11,5,5,11,7,5,11,7,11,11,11,11,9,5,11,3,9,7,9,7,3,3,5,3,7,5&total_bet_max=15,000,000.00&reel_set21=reel_set17&reel_set22=reel_set17&reel_set0=8,12,12,8,8,3,12,3,12,5,9,5,5,9,5,3,9,12,4,5,8,11,5,4,5,10,8,8,11,11,11,12,8,10,12,7,9,9,5,5,9,7,3,12,7,6,1,10,7,12,7,5,8,3,5,8,9,5,3,10,12,9,9,9,7,10,8,5,5,12,9,3,8,9,6,8,5,11,9,5,8,12,8,10,8,3,7,12,10,8,10,12,4,10,12,12,12,11,12,10,3,4,12,1,12,9,8,10,8,7,12,11,12,6,10,5,6,10,10,4,12,4,12,3,8,9,6,8,8,8,6,3,10,9,9,3,8,8,7,12,10,7,10,12,3,9,7,10,9,12,10,11,5,6,12,4,11,8,5,10,10,10,8,3,8,3,5,12,12,10,10,9,8,12,8,6,8,11,12,8,9,8,6,9,9,12,5,5,1,9,5,9,7,7,7,11,9,9,8,12,12,8,8,12,10,5,11,9,8,3,9,8,7,12,11,12,10,9,7,9,3,11,6,9,5,6~3,7,7,12,6,1,8,7,11,10,11,8,11,10,7,10,7,6,4,6,11,7,10,8,8,8,11,9,7,7,3,6,12,10,9,7,3,8,11,6,3,9,12,8,11,10,11,4,8,8,11,11,11,3,12,7,5,3,10,10,7,7,10,7,11,12,7,7,3,9,3,11,10,8,9,4,7,8,7,7,7,7,3,11,6,7,4,11,11,5,11,3,11,10,4,12,9,8,8,3,12,7,12,6,11,12,10,10,10,7,12,7,4,1,12,8,7,12,10,7,8,7,6,11,11,5,10,6,10,10,7,11,8,12,12,12,12,7,9,3,3,7,10,7,7,6,4,7,11,7,3,9,7,12,4,11,7,8,10,4,6,9,9,9,7,12,10,4,8,8,11,7,9,7,4,9,5,11,4,7,10,7,11,7,6,10,12,7,9,12~7,9,11,6,11,6,12,12,6,8,12,8,9,12,11,9,12,12,5,8,11,12,9,12,9,4,9,9,9,6,6,7,10,6,7,6,9,9,10,4,6,8,11,6,6,11,6,9,8,4,9,4,5,12,8,8,11,6,6,6,4,12,9,12,10,5,8,5,6,4,11,10,9,6,8,7,8,6,8,11,8,4,3,10,6,11,12,11,11,11,8,12,8,6,6,12,9,4,12,7,8,10,4,8,3,10,4,11,8,12,10,12,4,7,12,6,12,11,10,10,10,8,6,9,9,6,12,4,6,11,8,6,7,8,7,11,8,8,11,9,9,11,12,4,8,9,6,11,8,8,8,5,6,9,7,5,12,11,12,5,8,10,7,12,7,4,12,9,1,4,8,12,6,11,9,8,5,8,12,12,12,7,6,1,6,6,12,6,9,12,11,8,4,6,5,9,9,11,4,12,9,5,9,9,6,5,11,4,10~7,12,10,6,5,7,7,12,9,11,12,6,9,9,9,9,9,6,9,1,4,12,8,6,5,12,9,9,4,4,4,12,10,9,10,5,6,9,7,5,12,10,12,6,9,10,10,10,10,6,5,12,8,6,11,10,3,12,10,10,4,11,6,6,6,6,7,6,6,9,6,9,11,9,10,7,9,12,4,12,12,12,10,9,12,12,6,6,11,9,7,12,9,6,11,8,5,5,5,8,4,12,6,3,7,8,10,8,12,9,12,1,3,3,3,5,10,7,12,3,6,10,3,3,4,10,9,12,7,7,7,7,7,11,11,9,10,12,10,11,10,5,8,9,12,12,4,12~3,6,6,8,7,5,7,6,12,5,11,4,6,8,5,6,6,6,6,6,9,8,10,4,4,8,6,5,5,6,11,12,11,10,6,6,10,8,8,8,8,7,5,5,6,11,6,6,10,11,6,11,10,11,6,3,8,4,4,4,9,10,12,10,8,8,12,7,8,7,6,8,11,11,6,5,4,3,3,3,10,7,6,7,6,6,8,12,12,10,11,10,6,1,4,8,10,10,10,10,8,11,8,11,6,9,12,12,6,12,8,11,11,5,1,6,6,11,11,11,12,8,5,4,9,6,11,6,8,4,8,11,4,8,9,6,5,5,5,11,8,8,9,9,11,11,6,10,5,12,11,10,6,10,7,9,7,7,7,8,10,9,10,12,11,6,4,8,11,12,6,5,8,6,6,11,12~10,6,9,5,10,4,11,5,10,11,4,7,9,9,9,9,9,10,12,6,8,7,4,10,11,12,4,8,12,11,4,4,4,9,11,9,7,10,11,11,12,8,12,7,5,8,10,5,5,5,9,3,9,11,12,7,7,4,9,11,12,5,11,8,8,8,11,8,4,11,12,9,12,7,5,9,12,7,10,12,12,12,11,5,11,4,5,10,9,10,12,8,10,10,11,11,10,10,10,10,5,5,10,5,10,11,10,6,8,9,3,12,4,11,11,11,9,12,9,10,12,5,11,12,12,10,12,12,6,10,7,7,7,7,7,9,1,12,8,12,11,9,8,9,9,1,5,6,4,8&reel_set23=reel_set17&reel_set2=reel_set0&reel_set1=reel_set0&reel_set4=reel_set3&purInit=[{bet:3000,type:\"free_spins\"}]&reel_set3=4,8,10,5,8,4,11,3,10,12,8,12,5,8,12,5,9,6,8,6,11,3,5,10,5,6,12,5,10,8,9,8,3,3,3,9,12,8,4,10,1,5,7,4,9,4,8,12,12,9,7,3,8,7,7,5,11,9,12,7,5,5,10,9,12,9,11,12,10,7,7,7,10,9,4,1,9,3,10,3,11,4,9,10,7,9,10,6,9,9,5,9,7,9,10,3,11,12,12,10,3,8,5,12,8,8,8,10,12,6,7,11,9,6,8,5,11,10,8,4,12,7,11,10,12,8,7,8,8,7,12,12,5,12,8,11,8,5,8,5,12,12~10,7,5,8,10,9,3,10,8,6,1,7,8,9,4,7,12,10,12,4,8,10,11,10,11,11,3,3,3,12,5,8,2,7,8,7,2,8,10,5,8,12,12,9,8,2,10,7,9,12,8,9,12,5,10,11,5,7,7,7,12,5,6,9,11,12,9,11,4,5,5,3,12,4,9,5,9,8,4,3,12,9,4,3,8,8,5,12,8,8,8,3,8,5,10,9,6,7,9,8,5,6,5,1,3,9,8,10,4,12,12,7,7,6,3,10,9,11,11,12~8,9,5,11,6,12,7,12,9,10,8,12,7,12,7,12,5,8,4,9,8,10,8,10,7,12,11,8,12,3,5,8,10,4,9,12,11,7,12,10,8,4,10,5,7,4,9,12,3,3,3,7,11,12,4,8,11,4,1,5,3,8,5,5,6,8,3,5,2,7,8,8,10,8,9,9,4,9,11,7,4,10,12,7,10,11,4,9,5,2,11,7,10,9,9,10,10,12,9,8,7,7,7,9,11,4,5,6,5,8,5,7,9,8,12,6,3,8,9,10,10,9,12,5,8,3,8,5,8,8,11,7,8,4,10,11,12,5,8,6,6,9,12,1,5,9,11,12,9,12,6,5,8,8,8,12,4,3,9,7,9,10,12,12,5,7,5,12,7,5,8,3,12,5,10,8,11,12,3,8,5,6,9,9,3,10,9,11,9,12,10,8,12,10,10,8,12,5,2,3,10,5,9,3~10,9,4,8,12,10,8,11,3,7,10,1,12,9,12,10,12,12,5,9,8,8,7,8,9,7,8,10,5,8,12,8,11,8,3,3,3,11,8,3,7,10,7,12,10,11,8,12,5,5,9,9,5,5,8,11,4,10,9,3,6,10,12,10,6,8,2,10,1,3,12,9,7,7,7,2,10,12,3,5,6,7,10,10,8,5,4,3,7,5,3,3,5,9,8,9,12,12,4,5,9,4,8,9,12,11,9,9,12,5,8,8,8,3,9,5,12,6,8,7,8,5,12,12,5,12,4,2,8,9,5,10,9,11,8,4,7,11,11,7,6,5,9,6,7,8,11,4,4~8,11,12,9,2,3,12,11,3,12,9,9,7,12,6,10,9,8,8,9,4,8,11,9,11,12,10,8,2,10,8,3,5,3,3,3,6,8,6,12,8,5,11,6,11,10,8,10,10,12,9,12,10,8,11,9,10,6,8,10,9,10,10,12,12,5,12,5,11,10,7,7,7,4,5,4,3,7,8,8,7,8,5,7,5,3,11,10,12,5,12,6,4,9,7,8,9,9,4,12,1,8,7,12,4,5,9,8,8,8,7,7,9,9,5,3,10,12,5,10,2,8,5,4,8,12,9,7,5,7,5,1,8,7,4,9,11,3,9,3,5,8,12,5,12~4,9,5,2,1,12,8,12,11,10,5,12,8,6,9,3,4,7,12,9,4,11,5,4,11,9,4,11,12,5,7,9,3,3,3,5,6,3,7,7,12,5,7,3,8,6,12,12,8,9,12,4,11,10,10,12,10,8,9,12,7,5,5,6,8,3,7,10,8,7,7,7,5,8,12,9,8,4,3,5,9,10,11,3,8,10,8,9,10,5,10,6,10,12,5,11,12,8,11,9,11,12,6,9,8,7,8,8,8,7,9,5,2,8,10,5,9,7,7,8,5,2,10,10,8,8,1,9,5,12,12,10,3,4,11,12,9,8,3,8,12,9,10&reel_set20=reel_set17&reel_set6=reel_set5&reel_set5=8,12,12,1,8,12,6,8,10,6,12,12,10,4,4,4,10,1,4,6,6,1,10,4,1,8,6,1,12,12,6,6,6,10,12,1,4,10,1,6,1,8,1,10,10,1,10,1,10,10,10,1,6,10,6,12,1,6,1,8,6,4,1,10,8,12,12,12,8,1,6,4,12,4,4,6,12,8,4,1,6,10,4,1~11,9,5,7,11,5,9,3,9,5,3,3,5,5,3,3,3,3,9,9,11,7,9,9,3,5,5,11,11,3,9,3,3,5,5,5,5,7,11,11,7,9,5,11,5,7,9,5,11,11,9,7,7,7,7,3,9,11,11,7,9,11,3,3,9,11,5,5,9,9,9,9,7,7,11,5,3,3,9,7,7,11,5,5,7,5,3,11,11,11,11,5,9,3,5,9,7,7,9,5,7,7,3,9,5,3,7,11~10,7,5,1,11,7,3,10,9,1,9,6,10,12,6,1,4,7,8,9,11,1,11,1,6,1,5,1,12,7,1,10,1,4,1,12,1,3,1,12,6,10,1,11,9,8,1,8,9,6,3,8,3,1,11,5,1,7,12,3,4,5,1,10,1,7,1,6,7,8,1,3~4,4,6,3,5,11,3,12,6,7,5,8,3,3,3,6,6,7,5,4,3,5,8,3,4,10,4,5,5,5,5,6,8,11,6,10,8,10,9,11,5,6,7,11,4,4,4,4,9,8,9,5,11,12,7,11,9,5,7,11,7,10,10,10,12,8,7,8,8,11,9,5,7,6,5,12,4,6,6,6,6,8,8,10,9,3,9,12,5,3,7,10,6,10,8,8,8,11,9,3,10,12,3,7,5,4,10,6,3,9,12~7,11,5,7,7,1,10,11,6,4,1,11,5,3,12,1,9,1,11,1,8,1,10,1,8,4,6,11,8,7,12,3,1,6,12,1,9,3,5,1,8,10,1,7,1,6,1,12,7,3,6,6,3,1,8,10,4,4,3,7,5,1,9,1,6,5,7,8,12,5,1,10,12,3,1,12,1,10,11,10,1~3,12,1,4,7,1,8,5,11,1,11,5,3,1,7,4,6,5,5,5,8,1,8,8,1,10,9,1,5,7,3,3,10,9,6,8,12,1,6,6,6,5,1,8,10,4,1,10,5,9,4,1,3,6,6,1,11,12,7,9,11&reel_set8=reel_set5&reel_set7=reel_set5&reel_set9=reel_set5&total_bet_min=10.00";
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
        protected override int FreeSpinTypeCount
        {
            get { return 6; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            switch (freeSpinGroup)
            {
                case 0:
                    return new int[] { 200, 201, 202, 203, 204, 205 };
                case 1:
                    return new int[] { 201, 202, 203, 204, 205 };
                case 2:
                    return new int[] { 202, 203, 204, 205 };
                case 3:
                    return new int[] { 203, 204, 205 };
                case 4:
                    return new int[] { 204, 205 };
                case 5:
                    return new int[] { 205 };

            }
            return new int[] { 200, 201, 202, 203, 204 };
        }

        #endregion
        public PowerOfMerlinMegawaysGameLogic()
        {
            _gameID = GAMEID.PowerOfMerlinMegaways;
            GameName = "PowerOfMerlinMegaways";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["g"] = "{base:{def_s:\"5,11,8,12,6,10,4,9,9,3,7,9,6,11,12,11,6,11,4,8,7,3,3,9,6,11,12,6,5,12,7,10,7,4,4,8,5,9,11,5,6,12\",def_sa:\"4,10,7,8,4,12\",def_sb:\"5,11,11,12,7,11\",reel_set:\"0\",s:\"5,11,8,12,6,10,4,9,9,3,7,9,6,11,12,11,6,11,4,8,7,3,3,9,6,11,12,6,5,12,7,10,7,4,4,8,5,9,11,5,6,12\",sa:\"4,10,7,8,4,12\",sb:\"5,11,11,12,7,11\",sh:\"7\",st:\"rect\",sw:\"6\"},main:{def_s:\"14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14\",def_sa:\"4,11,7,9,6,12\",def_sb:\"5,11,8,12,7,11\",s:\"14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14,14\",sa:\"4,11,7,9,6,12\",sb:\"5,11,8,12,7,11\",sh:\"8\",st:\"rect\",sw:\"6\"},train:{def_s:\"13,12,10,13\",def_sa:\"8\",def_sb:\"9\",reel_set:\"12\",s:\"13,12,10,13\",sa:\"8\",sb:\"9\",sh:\"4\",st:\"rect\",sw:\"1\"}}";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("rs_iw"))
                dicParams["rs_iw"] = convertWinByBet(dicParams["rs_iw"], currentBet);

            if (dicParams.ContainsKey("wlc_v"))
            {
                string[] strParts = dicParams["wlc_v"].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strParts.Length; i++)
                {
                    string[] strValues = strParts[i].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                    strValues[1] = convertWinByBet(strValues[1], currentBet);
                    strParts[i] = string.Join("~", strValues);
                }
                dicParams["wlc_v"] = string.Join(";", strParts);
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in PowerOfMerlinMegawaysGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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

                    oldBetInfo.BetPerLine   = betInfo.BetPerLine;
                    oldBetInfo.LineCount    = betInfo.LineCount;
                    oldBetInfo.MoreBet      = betInfo.MoreBet;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in PowerOfMerlinMegawaysGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst)
        {
            try
            {
                PowerOfMerlinMegawaysResult spinResult = new PowerOfMerlinMegawaysResult();
                Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

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

                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in PowerOfMerlinMegawaysGameLogic::calculateResult {0}", ex);
                return null;
            }            
        }
        protected override BasePPSlotSpinResult restoreResultInfo(string strGlobalUserID, BinaryReader reader)
        {
            PowerOfMerlinMegawaysResult result = new PowerOfMerlinMegawaysResult();
            result.SerializeFrom(reader);
            return result;
        }
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                int index              = (int) message.Pop();
                int counter            = (int) message.Pop();
                int ind                = (int) message.Pop();
                double realWin         = 0.0;
                string strGameLog      = "";
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                ToUserResultMessage resultMsg = null;
                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    var betInfo = _dicUserBetInfos[strGlobalUserID];
                    var result  = _dicUserResultInfos[strGlobalUserID] as PowerOfMerlinMegawaysResult;
                    if ((result.NextAction != ActionTypes.DOBONUS) || (betInfo.SpinData == null) || !(betInfo.SpinData is BasePPSlotStartSpinData))
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        BasePPSlotStartSpinData startSpinData = betInfo.SpinData as BasePPSlotStartSpinData;
                        int stage                             = startSpinData.FreeSpinGroup + result.DoBonusCounter;
                        Dictionary<string, string> dicParams  = null;
                        bool isCollect                        = false;
                        bool isEnded                          = false;

                        do
                        {
                            if (ind == 0)
                            {
                                isCollect = true;
                                break;
                            }
                            double[] moveProbs = new double[] { 0.6310, 0.6937, 0.7395, 0.7658, 0.7892 };
                            if (betInfo.SpinData.IsEvent || Pcg.Default.NextDouble(0.0, 1.0) <= moveProbs[stage])
                            {
                                result.DoBonusCounter++;
                                if (startSpinData.FreeSpinGroup + result.DoBonusCounter < 5)
                                {
                                    dicParams = buildDoBonusResponse(result, betInfo, startSpinData, 1, true);
                                }
                                else
                                {
                                    isCollect = true;
                                    break;
                                }
                            }
                            else
                            {
                                dicParams           = buildDoBonusResponse(result, betInfo, startSpinData, 1, false);
                                double selectedWin  = startSpinData.StartOdd * betInfo.TotalBet;
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

                            betInfo.RemainReponses  = buildResponseList(freeSpinStrings, ActionTypes.DOSPIN);
                            double selectedWin      = (startSpinData.StartOdd + freeSpinData.SpinOdd) * betInfo.TotalBet;
                            double maxWin           = startSpinData.MaxOdd * betInfo.TotalBet;

                            //시작스핀시에 최대의 오드에 해당한 윈값을 더해주었으므로 그 차분을 보상해준다.
                            if (!startSpinData.IsEvent)
                                sumUpWebsiteBetWin(agentID, 0.0, selectedWin - maxWin);
                            else if (maxWin > selectedWin)
                                addEventLeftMoney(agentID, strUserID, maxWin - selectedWin);
                            dicParams = buildDoBonusResponse(result, betInfo, startSpinData, 0, false);
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
                            realWin = double.Parse(dicParams["tw"]);
                            strGameLog = strResponse;
                            if (realWin > 0.0f)
                            {
                                _dicUserHistory[strGlobalUserID].baseBet = betInfo.TotalBet;
                                _dicUserHistory[strGlobalUserID].win     = realWin;
                            }
                            else
                            {
                                saveHistory(agentID, strUserID, ind, counter, userBalance, currency);
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
                _logger.Error("Exception has been occurred in PowerOfMerlinMegawaysGameLogic::onDoBonus {0}", ex);
            }
        }
        protected Dictionary<string, string> buildDoBonusResponse(PowerOfMerlinMegawaysResult result, BasePPSlotBetInfo betInfo, BasePPSlotStartSpinData startSpinData, int ind, bool isNextMove)
        {
            Dictionary<string, string> dicParams = new Dictionary<string, string>();

            int[] freeSpinCounts = new int[] { 10, 14, 18, 22, 26, 30 };
            Dictionary<string, string> dicLastParams = splitResponseToParams(result.ResultString);
            dicParams["tw"]         = dicLastParams["tw"];
            JToken gParam           = JToken.Parse(dicLastParams["g"]);
            JToken newGParam        = JToken.Parse("{}");
            newGParam["bg"]         = gParam["bg"];
            if (newGParam["bg"]["ch_h"] == null)
                newGParam["bg"]["ch_h"] = string.Format("0~{0}", ind);
            else
                newGParam["bg"]["ch_h"] = string.Format("{0},0~{1}", newGParam["bg"]["ch_h"].ToString(), ind);

            if (ind == 0)
            {
                dicParams["fsmul"]      = "1";
                dicParams["fsmax"]      = freeSpinCounts[result.DoBonusCounter + startSpinData.FreeSpinGroup].ToString();
                dicParams["na"]         = "s";
                dicParams["fswin"]      = "0.00";
                dicParams["fs"]         = "1";
                dicParams["fsres"]      = "0.00";
                dicParams["trail"]      = string.Format("fs~{0}", freeSpinCounts[result.DoBonusCounter + startSpinData.FreeSpinGroup]);
                newGParam["bg"]["end"]  = "1";
                dicParams["g"]          = serializeJsonSpecial(newGParam);
                return dicParams;
            }
            else
            {
                if (!isNextMove)
                {
                    dicParams["trail"]      = "fs~0";
                    newGParam["bg"]["end"]  = "1";
                    if (double.Parse(dicParams["tw"]) > 0.0)
                        dicParams["na"] = "c";
                    else
                        dicParams["na"] = "s";
                }
                else
                {
                    dicParams["trail"]     = string.Format("try~4;fs~{0}", freeSpinCounts[result.DoBonusCounter + startSpinData.FreeSpinGroup]);
                    newGParam["bg"]["end"] = "0";
                    dicParams["na"]        = "b";
                }
                dicParams["g"] = serializeJsonSpecial(newGParam);
                return dicParams;
            }
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set", "bw" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]))
                    continue;
                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }

            if (resultParams.ContainsKey("g") && spinParams.ContainsKey("g"))
            {
                JToken gParam       = JToken.Parse(resultParams["g"]);
                JToken resultGParam = JToken.Parse(spinParams["g"]);
                gParam["base"]  = resultGParam["base"];
                gParam["main"]  = resultGParam["main"];
                gParam["train"] = resultGParam["train"];
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
        protected override void supplementInitResult(Dictionary<string, string> dicParams, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {
            if (dicParams.ContainsKey("sh"))
                dicParams.Remove("sh");
            if (!dicParams.ContainsKey("reel_set"))
                dicParams["reel_set"] = "0";
        }
    }
}
