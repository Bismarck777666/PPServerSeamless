using Akka.Actor;
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
    class ChestsOfCaiShenBetInfo : BasePPSlotBetInfo
    {
        public int          PurchaseType    { get; set; }
        public List<int>    ChestLevels     { get; set; }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            this.PurchaseType   = reader.ReadInt32();
            int count           = reader.ReadInt32();
            ChestLevels = new List<int>();
            for (int i = 0; i < count; i++) 
            { 
                int level = reader.ReadInt32();
                ChestLevels.Add(level);
            }
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(PurchaseType);
            if(ChestLevels == null)
            {
                writer.Write(0);
                return;
            }
            writer.Write(ChestLevels.Count);
            for (int i = 0;i < ChestLevels.Count; i++)
            {
                writer.Write(ChestLevels[i]);
            }
        }
    }

    class ChestsOfCaiShenGameLogic : BasePPSlotGame
    {
        protected double[] _multiTotalFreeSpinWinRates;
        protected double[] _multiMinFreeSpinWinRates;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25checaishen";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 25; }
        }
        protected override int ServerResLineCount
        {
            get { return 25; }
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
                return "def_s=5,8,5,3,8,3,10,7,4,9,5,12,12,7,12&cfgs=1&ver=3&mo_s=13&mo_v=15,20,25,50,75,100,125,150,200,250,300,350,400,500,750,250,500,3750&def_sb=4,8,5,7,8&reel_set_size=7&def_sa=4,8,3,3,9&mo_jp=250;500;3750&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{purchase_1:\"96.48\",purchase_0:\"96.46\",regular:\"96.47\"},props:{gj:\"125000\",max_rnd_sim:\"1\",cmp:\"6\",max_rnd_hr:\"6918789\",max_rnd_win:\"10000\"}}&wl_i=tbm~10000&mo_jp_mask=jp;jp;jp&sc=8.00,16.00,24.00,32.00,40.00,80.00,120.00,160.00,200.00,300.00,400.00,600.00,1000.00,2000.00,3000.00,4000.00&defc=40.00&purInit_e=1,1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;50,15,5,2,0;50,10,5,0,0;30,10,5,0,0;30,10,5,0,0;30,10,5,0,0;30,10,5,0,0;15,5,2,0,0;15,5,2,0,0;15,5,2,0,0;15,5,2,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=7,7,8,5,8,15,4,16,8,7,6,8,6,9,10,17,7,4,9,10,3,9,10,6,12,9,3,11,5,4,4,10,15,9,8,4,5,6,9,4,3,11,11,11,7,11,7,4,12,7,7,16,4,8,6,8,12,9,17,12,10,6,5,12,9,12,3,12,15,5,11,7,11,10,10,9,3,8,7,7,9,6,10,6,10,5,6~2,9,4,8,2,5,16,10,9,9,4,8,17,4,4,6,11,12,2,8,5,8,2,2,2,5,17,11,7,4,15,3,3,8,6,7,3,4,12,7,16,12,11,7,5,3,3,2,11,5,5,5,15,2,10,2,10,10,11,5,6,8,10,10,7,5,10,4,8,7,10,6,8,8,6,5,6,6,6,4,6,7,3,12,2,15,9,2,5,3,5,3,2,9,16,2,2,3,5,2,6,9,7,17,10~10,2,10,4,11,9,10,15,4,8,9,5,7,7,3,2,2,16,5,2,2,2,6,6,8,17,9,3,6,6,7,12,10,15,7,7,9,3,3,7,9,16,2,2,9,6,6,6,9,2,3,8,11,17,2,2,6,10,10,15,3,8,16,10,6,8,7,12,11~11,4,12,17,4,5,11,12,4,7,2,2,2,4,16,2,2,5,7,10,5,15,10,6,9,7,11,11,11,17,7,8,9,12,16,7,8,3,8,11,2,15,8,11,8,8,8,4,7,8,9,2,2,10,12,2,9,3,6,8,3,3,3,2,7,2,3,4,11,12,8,8,9,2,5,4,11~2,5,17,4,11,9,16,10,4,10,6,11,2,12,5,12,6,3,3,3,6,12,7,17,12,9,4,15,8,9,12,10,3,11,5,3,5,2,2,2,10,4,4,15,11,10,9,4,4,8,12,16,5,7,8,3,7,2,2,10,10,10,6,9,6,11,3,16,3,4,8,15,12,9,9,5,12,8,9,9,2&accInit=[{id:0,mask:\"bcd;bcp\"},{id:1,mask:\"gcd;gcp\"},{id:2,mask:\"rcd;rcp\"},{id:3,mask:\"bdp;bpp\"},{id:4,mask:\"gdp;gpp\"},{id:5,mask:\"rdp;rpp\"}]&reel_set2=12,15,6,4,16,9,17,11,9,15,7,4,8,3,9,16,11,10,8,7,6,7,17,10,5,5,10,9,4,3,9,15,9,7,6,16,5,8,17,12,15,6,7,16,6,17,6,15,6,16,4,17,3,15,7,16,9,11,8,10,5,4,17,12,9,7,15,4,16,3~7,12,5,4,4,6,3,3,5,10,10,11,6,2,12,3,11,5,2,2,2,6,5,7,2,5,9,9,5,4,10,4,3,2,12,8,10,7,4,17,5,5,5,15,10,2,3,7,8,2,7,6,4,7,4,16,10,2,8,9,12,5,6,6,6,9,8,9,11,2,2,3,8,2,11,2,8,17,5,10,3,8,7,6~10,11,15,3,10,9,3,9,6,7,8,6,2,4,7,3,9,2,10,12,7,10,2,8,3,2,2,2,8,5,7,16,3,17,9,2,11,2,9,7,2,6,8,10,15,5,10,8,9,7,6,11,4,3,2,6,6,6,11,10,9,9,6,2,6,10,16,12,8,12,3,6,9,7,7,10,2,2,7,6,3,4,11,17,8,5~15,4,9,2,7,2,8,2,2,11,11,4,10,2,2,2,9,2,3,8,7,8,7,12,11,8,10,3,4,12,3,11,11,11,16,11,4,9,12,4,4,2,11,2,11,11,6,8,8,8,2,7,7,6,12,7,9,5,2,8,7,9,8,3,5,3,3,3,12,2,17,8,12,3,8,4,5,10,5,8,4,7,6,12~8,5,11,15,10,10,12,5,16,10,3,2,7,17,3,3,3,5,6,9,9,8,9,11,15,4,11,4,12,2,5,4,2,2,2,16,4,2,2,12,9,10,6,8,4,17,10,9,3,6,15,10,10,10,11,3,6,12,16,9,17,9,3,3,6,2,7,8,7,4,5&reel_set1=15,7,9,15,7,5,3,15,5,15,7,15,9,15,9,5,15,7,12,5,5,15,9,15,12,3,12,9~11,10,6,11,11,8,4,11,11,11,8,4,4,6,8,11,6,8,6,4,4,4,11,11,4,6,4,4,11,4,6,6,6,10,10,6,4,8,4,10,8,6~5,9,16,4,16,4,6,7,10,4,16,5,9,10,7,9,16,12,16,8,9,3,10,9,5,16,6,7,3,16,4,10~9,11,5,11,12,8,6,5,4,6,4,3,11,11,11,4,11,12,3,7,4,7,4,8,10,8,9,11,7~3,6,4,17,7,9,5,10,12,7,17,8,11,17,6,17,3,4,8,12,17,9,5,17,11,5&reel_set4=3,4,10,4,5,7,9,10,12,6,9,12,9,6,12,3,7,4,4,5,17,7,5,9,15,11,3,5,10,10,11,3,11,11,11,6,8,12,10,8,16,7,3,6,9,8,10,6,9,8,12,6,17,8,7,7,8,4,4,12,7,8,7,11,9,6,5,10,4,7~7,2,10,2,2,8,3,10,9,7,4,5,6,2,2,2,11,3,5,8,4,3,12,5,6,6,4,8,15,11,5,5,5,9,16,10,12,6,5,3,7,2,9,11,7,2,17,6,6,6,5,2,8,5,12,10,2,8,4,9,4,10,3,7,2~9,15,9,4,3,9,16,10,8,16,3,6,15,9,17,2,9,5,8,6,16,12,9,15,2,17,7,10,4,16,11,15,7,17,12,2,8,6,3,2,10,16,2,7,10,8,15,3,17,2,11,12,2,6,8,16,10,15,9,8,3,17,9,4,5,7,2,16,11,15,3,11,7,17,10,16~12,11,4,5,8,2,2,4,2,8,2,8,9,2,10,2,2,2,12,4,8,11,7,9,3,6,12,15,11,17,4,3,3,5,4,11,11,11,5,10,11,2,6,2,11,11,7,7,2,8,5,3,7,8,4,8,8,8,7,16,11,7,9,7,15,5,12,10,3,11,4,2,4,9,4,3,3,3,8,8,2,12,6,9,2,7,10,7,12,17,12,8,9,12,2,8~16,10,9,5,12,2,15,4,2,17,11,7,3,3,3,16,4,2,8,9,15,4,10,12,17,4,9,7,6,16,4,2,2,2,15,11,8,4,9,9,10,12,7,9,6,12,11,3,2,10,10,10,3,5,3,17,9,5,10,10,6,6,5,11,16,5,8,6,2&purInit=[{bet:1250,type:\"default\"},{bet:2500,type:\"default\"}]&reel_set3=5,10,4,8,6,11,6,15,3,16,3,7,11,9,7,5,5,9,11,8,3,9,10,3,6,4,7,5,7,12,9,10,9,10,3,4,7,8,9,11,11,11,8,7,9,8,5,7,6,9,6,6,9,12,6,4,8,6,9,4,17,10,12,4,4,12,7,12,7,10,7,10,6,15,8,12,10,5,4,16,10,11,12,7~17,12,10,15,10,16,2,12,5,12,6,8,17,8,11,15,3,16,2,7,17,4,15,10,16,4,7,2,10,11,7,5,9,2,17,8,7,15,10,16,8,17,8,15,4,11,2,16,5,17,4,9,15,8,2,16,10,17,9,5,4,2,15,7,6,9,5,3,8,16,7,6,17,2,3,15,11,3,12,5,16,3,2,6,2,17,6~2,9,8,4,2,4,11,3,11,2,6,7,2,10,5,3,15,9,3,10,7,2,2,2,10,5,16,4,5,10,9,6,9,8,6,10,12,17,11,7,7,8,3,9,6,10,12,7,6,6,6,10,8,12,7,6,9,2,3,8,9,2,2,6,7,2,9,2,15,8,7,10,3,6,3~16,8,9,4,2,8,3,11,2,7,12,8,5,8,4,2,2,2,5,12,11,12,2,8,12,5,4,3,7,12,8,10,6,11,11,11,2,2,4,2,10,11,7,6,4,7,8,9,2,3,7,10,4,8,8,8,4,9,5,17,6,9,15,4,11,8,2,8,3,10,16,8,3,3,3,11,4,11,2,7,5,7,7,3,12,9,2,12,11,3,7,2,8~6,17,9,2,2,11,8,12,4,9,15,2,2,8,9,11,3,11,3,3,3,4,3,16,10,5,12,3,17,10,15,12,12,7,6,5,3,12,7,5,10,9,11,2,2,2,3,6,12,16,10,9,17,11,4,15,5,11,5,3,4,2,6,16,10,7,8,2,10,10,10,7,9,9,8,9,17,2,3,5,15,6,10,4,6,5,6,9,4,16,10,4,9,2,9&reel_set6=10,7,11,5,9,6,8,9,3,9,7,8,4,5,8,7,5,3,6,12,8,12,4,6,11,10,11,11,11,8,11,7,4,6,9,3,7,17,9,16,7,9,4,12,6,15,10,10,7,9,6,12,3,10,10,4,5,12~3,4,11,8,8,12,8,12,4,5,9,2,2,2,7,2,6,6,9,7,4,17,5,10,2,16,10,2,5,5,5,10,9,2,4,7,6,3,3,5,7,5,8,5,10,6,6,6,2,9,15,4,11,3,10,2,7,11,8,2,3,6,5~17,8,12,7,9,2,11,10,7,10,7,3,10,6,9,6,4,9,2,2,2,11,16,10,10,2,6,8,8,2,2,11,8,9,6,4,9,10,15,5,6,6,6,5,3,7,2,8,2,6,3,9,7,7,9,3,17,2,10,7,3,2,12~8,16,9,11,11,8,8,4,2,8,5,2,2,12,15,2,11,7,9,5,8,7,5,9,6,2,12,2,2,2,4,11,7,17,3,8,9,4,7,10,2,3,12,11,2,7,11,8,7,4,10,7,2,10,2,4,9,16,2,8,8,8,15,9,4,4,12,17,8,16,8,3,8,3,12,2,4,12,15,2,12,3,10,6,8,17,7,11,4~16,9,15,11,7,12,5,17,8,16,12,15,7,4,17,12,7,4,8,9,2,16,2,3,10,15,12,17,6,8,16,6,15,12,5,6,17,7,5,16,11,15,3,17,9,16,8,10,4,10,11,15,8,4,17,5,9,12,10,3,16,2,9,15,4,11,2,17,5,15,9,12,10,6,3&reel_set5=11,3,6,6,12,4,11,7,10,10,6,10,8,11,8,7,7,4,5,12,9,4,7,6,7,7,15,3,4,9,6,17,7,3,7,5,5,12,7,10,11,11,11,10,8,3,16,6,12,4,8,12,9,3,12,9,5,6,4,8,10,4,5,9,10,4,12,9,5,8,10,3,7,7,15,8,9,12,5,6,6,10,9,7,9,11~3,3,10,8,9,7,11,3,5,7,3,8,7,2,2,2,12,10,10,6,12,6,11,3,3,4,2,2,4,2,2,5,5,5,10,11,2,8,8,2,5,10,4,9,9,4,2,7,6,6,6,5,6,8,10,8,9,7,17,12,2,16,5,4,15,5,6~4,9,3,2,9,3,5,17,2,8,4,3,7,8,2,2,2,9,11,16,10,2,6,2,15,7,12,6,10,6,6,6,9,11,9,5,2,8,10,7,8,6,2,3,7,10,12,7,10~17,9,16,4,15,4,17,7,16,10,15,11,17,3,12,11,3,2,16,7,15,4,8,5,2,8,17,6,4,16,5,15,2,2,9,2,4,6,10,4,3,5,8,11,8,9,4,17,12,2,16,9,7,4,15,2,17,5,16,12,8,3,15,7,8,17,9,16,12,8,11,3,15,2,10,7~2,4,3,11,17,2,3,9,16,2,12,5,3,3,3,4,15,11,2,9,17,10,4,16,6,3,10,9,3,8,2,2,2,10,7,6,11,5,2,9,8,15,3,6,11,9,5,10,10,10,5,12,17,4,12,10,7,10,5,16,9,12,4,15,9,6,6&total_bet_min=200.00";
            }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 50; }
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
            get { return 2; }
        }
        public double[] PurchaseMultiples
        {
            get { return new double[] { 50, 100 }; }
        }
        #endregion

        public ChestsOfCaiShenGameLogic()
        {
            _gameID     = GAMEID.ChestsOfCaiShen;
            GameName    = "ChestsOfCaiShen";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"]   = "0";
	        dicParams["g"]          = "{red1:{def_s:\"14,14,14,14,14,14,14,14,14,14,14,14,14,14,14\",def_sa:\"14,14,14,14,14\",def_sb:\"14,14,14,14,14\",s:\"14,14,14,14,14,14,14,14,14,14,14,14,14,14,14\",sa:\"14,14,14,14,14\",sb:\"14,14,14,14,14\",sh:\"3\",st:\"rect\",sw:\"5\"},red2:{def_s:\"14,14,14,14,14,14,14,14,14,14,14,14,14,14,14\",def_sa:\"14,14,14,14,14\",def_sb:\"14,14,14,14,14\",s:\"14,14,14,14,14,14,14,14,14,14,14,14,14,14,14\",sa:\"14,14,14,14,14\",sb:\"14,14,14,14,14\",sh:\"3\",st:\"rect\",sw:\"5\"}}";
	        dicParams["st"]         = "rect";
	        dicParams["sw"]         = "5";
            dicParams["accm"]       = "bcd~bcp;gcd~gcp;rcd~rcp";
            dicParams["accv"]       = "0~0;0~0;0~0";
            dicParams["acci"]       = "0;1;2";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("rs_win"))
                dicParams["rs_win"] = convertWinByBet(dicParams["rs_win"], currentBet);
            if (dicParams.ContainsKey("mo_tw"))
                dicParams["mo_tw"] = convertWinByBet(dicParams["mo_tw"], currentBet);
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


                ChestsOfCaiShenBetInfo betInfo = new ChestsOfCaiShenBetInfo();
                betInfo.BetPerLine  = (float)message.Pop();
                betInfo.LineCount   = (int)message.Pop();

                if (message.DataNum >= 3)
                {
                    betInfo.PurchaseFree = true;
                    betInfo.PurchaseType = (int)message.GetData(2);
                }
                else
                    betInfo.PurchaseFree = false;

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in ChestsOfCaiShenGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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

                    oldBetInfo.BetPerLine = betInfo.BetPerLine;
                    oldBetInfo.LineCount = betInfo.LineCount;
                    oldBetInfo.MoreBet = betInfo.MoreBet;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                    (oldBetInfo as ChestsOfCaiShenBetInfo).PurchaseType = betInfo.PurchaseType;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in ChestsOfCaiShenGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            ChestsOfCaiShenBetInfo betInfo = new ChestsOfCaiShenBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new ChestsOfCaiShenBetInfo();
        }
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as ChestsOfCaiShenBetInfo).PurchaseType;
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
            int purchaseType = (betInfo as ChestsOfCaiShenBetInfo).PurchaseType;
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
                int purchaseType = (betInfo as ChestsOfCaiShenBetInfo).PurchaseType;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, -1, PurchaseMultiples[purchaseType] * 0.2, PurchaseMultiples[purchaseType] * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in ChestsOfCaiShenGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int purchaseType = (betInfo as ChestsOfCaiShenBetInfo).PurchaseType;
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in ChestsOfCaiShenGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            base.overrideSomeParams(betInfo, dicParams);
            if (!dicParams.ContainsKey("reel_set"))
                dicParams["reel_set"] = "0";

            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = (betInfo as ChestsOfCaiShenBetInfo).PurchaseType.ToString();

            if (dicParams.ContainsKey("trail"))
            {
                string rst = dicParams["trail"];



            }
        }
        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            ChestsOfCaiShenBetInfo starBetInfo = betInfo as ChestsOfCaiShenBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, -1, betInfo.PurchaseFree ? starBetInfo.PurchaseType : -1, betMoney);
        }
    }
}
