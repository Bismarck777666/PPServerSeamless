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

namespace SlotGamesNode.GameLogics
{
    class GemTrioBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get
            {
                return BetPerLine * 10.0f;
            }
        }
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

    class GemTrioGameLogic : BasePPSlotGame
    {
        protected double[] _multiTotalFreeSpinWinRates;
        protected double[] _multiMinFreeSpinWinRates;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs9gemtrio";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 9; }
        }
        protected override int ServerResLineCount
        {
            get { return 10; }
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
                return "def_s=6,9,5,7,8,8,6,9,5&reel_set25=10,2,9,8,4,9,10,3,8,10,11,10,5,10,7,13,10,6,9,10~13,10,9,10,9,10,8,9,10,8,12,8,10,3,10,6,10,2,7,4,10,5~9,10,12,10,11,4,9,7,8,10,6,10,5,3,10,9,10,2,8,10,10&reel_set26=13,10,5,8,10,7,9,10,8,10,8,6,10,11,10,4,10,3,2,9~8,2,10,8,6,3,9,4,10,13,12,10,7,9,10,5,10~10,8,9,4,6,10,9,11,5,10,2,8,10,12,10,9,10,7,3,8&cfgs=1&ver=3&def_sb=7,9,8&reel_set_size=27&def_sa=7,8,8&scatters=1~0,0,0~0,0,0~1,1,1&rt=d&gameInfo={rtps:{purchase_1:\"96.51\",purchase_0:\"96.50\",regular:\"96.50\",purchase_2:\"96.49\"},props:{mn:\"500\",max_rnd_sim:\"1\",jp:\"100000\",max_rnd_hr:\"27247956\",max_rnd_win:\"20000\",mi:\"200\",mj:\"3000\"}}&wl_i=tbm~20000&reel_set10=6,4,8,11,10,2,10,3,8,9,10,5,9,7,10,10,11~8,9,10,11,10,6,10,11,2,8,10,3,7,10,9,5,4~8,11,9,2,10,3,10,7,10,8,5,9,4,6,10&sc=0.01,0.02,0.05,0.10,0.15,0.20,0.25,0.38,0.50,1.00,1.50,2.00,2.50,3.00,3.50,4.00,4.50,5.00,5.50,6.00,6.50,7.00,7.50,8.00,9.00,10.00,11.00,12.00,12.50&defc=0.10&reel_set11=9,7,10,11,10,4,10,8,9,5,2,3,6,10,8,10,11,9~10,11,5,7,10,9,10,8,3,8,10,11,8,10,9,2,4,10,6~2,7,6,9,10,9,10,11,8,10,5,3,10,4,11,9,8,10&reel_set12=9,8,4,6,7,9,11,10,2,10,3,10,11,10,8,10,5~10,9,2,10,7,8,11,8,10,6,10,8,9,10,9,11,10,4,10,5,10,3~10,4,10,8,10,5,11,10,9,11,10,3,9,10,2,8,7,6&purInit_e=1,1,1&reel_set13=6,11,10,9,8,10,3,10,7,2,10,5,8,10,4,11,9~11,8,7,3,8,10,11,9,5,6,10,2,10,4,10,9,10~2,3,10,9,8,10,5,10,4,7,10,8,11,9,10,11,10,8,10,9,6&wilds=2~2500,0,0~1,1,1;3~1250,0,0~1,1,1;4~500,0,0~1,1,1&bonuses=0&reel_set18=10,8,12,4,10,3,8,6,9,10,7,10,12,10,2,9,5,10,9,8,10~9,5,12,9,10,12,6,8,10,4,10,3,2,10,7,10,8,10~8,12,10,9,5,10,9,10,9,12,8,10,8,3,7,4,10,6,10,2&reel_set19=2,9,10,3,10,9,8,5,12,10,6,8,7,10,4,12,9,10,8~10,9,10,5,8,6,10,12,2,10,12,4,10,9,7,3,10~10,8,6,10,8,10,12,9,10,5,9,12,4,2,10,8,3,7&ntp=0.00&reel_set14=4,8,10,9,2,11,10,8,5,9,6,10,7,10,3~9,5,8,11,10,9,6,7,8,10,3,4,10,11,10,2,10~10,8,10,9,2,10,4,3,10,5,7,9,6,11,10,8&paytable=0,0,0;0,0,0;0,0,0;0,0,0;0,0,0;70,0,0;40,0,0;25,0,0;10,0,0;5,0,0;0,0,0;0,0,0;0,0,0;0,0,0&reel_set15=7,3,12,8,10,9,2,4,12,10,6,5,10,9,10,8~6,10,2,8,10,8,12,9,10,4,7,9,10,5,3~10,7,4,10,3,9,12,8,2,9,6,8,10,5,10,12&reel_set16=12,4,7,10,8,10,5,2,10,8,9,3,9,6,10,12,10~6,10,3,12,8,10,8,5,4,7,9,10,9,10,9,2,8,10,12,10~10,8,10,12,10,2,9,10,8,7,3,12,10,5,8,10,9,4,10,10,6&reel_set17=4,10,8,2,6,9,10,12,8,10,7,3,10,9,5,10,12,10~10,12,10,8,10,5,12,9,10,6,8,10,4,10,7,9,8,3,10,9,10,2~10,3,10,4,10,9,6,5,8,7,9,10,9,10,12,8,10,2,12,8,10&total_bet_max=25,000.00&reel_set21=9,3,10,12,10,8,10,9,11,10,8,6,2,10,12,7,9,8,10,4,11,5~10,11,8,9,12,10,3,2,10,5,4,10,8,9,12,7,10,11,6,9,8~2,10,8,9,10,7,4,11,9,12,3,10,6,10,11,5,8,10,9&reel_set22=11,9,5,10,2,10,4,9,12,6,10,8,3,10,8,10,7,12,8,10,9,11,10~8,5,7,9,11,8,2,10,3,8,4,10,9,10,11,12,10,6,10,12,10~12,10,11,8,6,9,2,7,10,11,12,10,8,5,10,4,10,3&reel_set0=10,4,2,9,10,13,11,10,3,8,9,10,6,5,10,8,7~10,9,10,3,8,10,13,10,7,10,2,10,6,10,12,8,10,4,10,8,9,5,10,9~9,11,10,8,9,5,4,10,12,10,6,10,9,10,3,7,8,2,10,8,10&reel_set23=3,8,10,12,10,7,11,4,9,12,11,10,2,10,8,6,5,10,9~9,6,4,10,8,10,12,5,10,11,10,7,3,12,8,11,2,9~7,5,12,2,10,9,3,10,8,11,10,9,11,12,8,10,4,6,10&reel_set24=6,10,12,4,9,10,7,10,3,10,11,9,2,11,10,8,5,12,9,10,8~2,3,10,11,7,10,12,8,5,10,11,10,12,6,9,10,4,9~3,7,12,8,2,10,12,11,10,8,4,5,6,10,9,11,10&accInit=[{id:0,mask:\"ed;ep\"},{id:1,mask:\"dd;dp\"},{id:2,mask:\"rd;rp\"},{id:3,mask:\"sp0;sp1\"},{id:4,mask:\"g_a;mj_a;mn_a;mi_a;g;mj;mn;mi\"},{id:5,mask:\"edp;epp\"},{id:6,mask:\"ddp;dpp\"},{id:7,mask:\"rdp;rpp\"}]&reel_set2=13,9,8,9,5,10,8,10,2,6,9,10,11,10,7,10,8,3,10,10,4~10,5,8,2,10,8,10,9,13,4,9,7,6,12,8,10,9,3,10~7,8,11,10,3,8,10,12,9,10,6,9,10,2,5,8,10,4,9,10,10&reel_set1=9,8,7,10,11,10,2,10,6,5,10,4,10,13,9,3,8~4,10,9,6,10,9,5,10,7,10,12,8,3,10,9,10,8,10,2,10,13,8~8,10,11,9,10,6,8,9,10,9,5,10,8,12,3,7,2,4&reel_set4=8,10,9,10,11,10,5,10,4,9,10,8,13,2,6,10,3,8,10,9,7~8,10,5,3,9,7,12,10,2,8,10,6,10,9,8,10,13,10,4,10~8,10,9,10,11,10,5,10,12,10,4,7,10,3,10,6,2,9&purInit=[{bet:500},{bet:1000},{bet:2000}]&reel_set3=7,8,6,10,2,11,8,10,13,4,10,9,5,10,3,9,10,8,10,10~6,2,10,7,10,9,10,9,5,10,13,8,10,3,4,10,8,9,12~9,10,3,5,10,2,10,7,10,12,6,4,9,10,9,10,8,10,8,11&reel_set20=8,5,10,6,9,10,7,10,11,10,9,2,11,12,10,4,10,8,12,3~8,10,9,8,10,9,3,10,11,2,10,12,5,9,10,7,11,6,4,8,12~6,9,8,5,10,2,10,4,10,8,3,8,10,7,9,11,12,10,9,10,12,11&reel_set6=2,9,10,4,8,10,3,10,7,9,10,5,8,6,10~10,2,7,10,8,10,5,8,9,10,6,10,8,4,9,10,9,3,10~4,10,3,10,8,10,7,2,8,10,9,10,9,10,9,10,6,10,5&reel_set5=10,5,9,7,9,6,10,2,4,8,10,8,10,3,10~10,7,4,10,9,10,8,10,6,10,2,10,9,10,9,10,8,5,10,10,3~6,10,8,5,4,9,7,2,10,9,10,8,10,3,10&reel_set8=10,7,10,5,3,8,10,2,9,10,9,6,10,4,8,10,9,10,10,8~9,10,6,10,9,10,5,10,8,10,2,10,7,10,8,4,10,3~9,10,6,10,8,3,10,7,8,10,2,10,4,10,9,8,10,5,10&reel_set7=9,10,6,10,4,9,10,2,10,9,10,8,10,5,8,10,8,7,3~8,10,3,6,9,10,5,9,10,2,10,8,4,10,8,10,9,10,7,10~4,9,10,3,10,9,10,2,10,6,8,10,5,8,10,8,10,7,10,9&reel_set9=10,9,10,6,9,2,10,9,10,8,4,10,5,8,10,7,10,10,3,10,8~8,10,9,2,10,3,7,10,6,10,4,10,9,5,10~8,10,5,9,2,9,10,6,10,9,3,10,4,10,8,10,7,10,10&total_bet_min=0.10";
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
            get { return 3; }
        }
        public double[] PurchaseMultiples
        {
            get { return new double[] { 50, 100, 200 }; }
        }
        #endregion
        
        public GemTrioGameLogic()
        {
            _gameID     = GAMEID.GemTrio;
            GameName    = "GemTrio";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["g"] = "{ruby:{def_s:\"6,9,5,7,8,8,6,9,5\",def_sa:\"7,8,8\",def_sb:\"7,9,8\",reel_set:\"0\",s:\"6,9,5,7,8,8,6,9,5\",sa:\"7,8,8\",sb:\"7,9,8\",sh:\"3\",st:\"rect\",sw:\"3\"}}";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "3";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);

            if (dicParams.ContainsKey("g"))
            {
                var gParam = JToken.Parse(dicParams["g"]);
                if (gParam["ruby"] != null)
                {
                    int winLineID = 0;
                    do
                    {
                        if (winLineID >= 9)
                            break;

                        string strKey = string.Format("l{0}", winLineID);
                        if (gParam["ruby"][strKey] == null)
                        {
                            winLineID++;
                            continue;
                        }

                        string[] strParts = gParam["ruby"][strKey].ToString().Split(new string[] { "~" }, StringSplitOptions.None);
                        if (strParts.Length >= 2)
                        {
                            strParts[1] = convertWinByBet(strParts[1], currentBet);
                            gParam["ruby"][strKey] = string.Join("~", strParts);
                        }
                        winLineID++;
                    } while (true);
                    dicParams["g"] = serializeJsonSpecial(gParam);
                }
            }


        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


                GemTrioBetInfo betInfo = new GemTrioBetInfo();
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in GemTrioGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                    (oldBetInfo as GemTrioBetInfo).PurchaseType = betInfo.PurchaseType;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in GemTrioGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            GemTrioBetInfo betInfo = new GemTrioBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new GemTrioBetInfo();
        }
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as GemTrioBetInfo).PurchaseType;
            return this.PurchaseMultiples[purchaseType];
        }
        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBet     = (double)infoDocument["defaultbet"];
                _normalMaxID            = (int)infoDocument["normalmaxid"];
                _emptySpinCount         = (int)infoDocument["emptycount"];
                _naturalSpinCount       = (int)infoDocument["normalselectcount"];

                _multiTotalFreeSpinWinRates = new double[FreePurCount];
                _multiMinFreeSpinWinRates = new double[FreePurCount];

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
            int purchaseType = (betInfo as GemTrioBetInfo).PurchaseType;
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
                int purchaseType = (betInfo as GemTrioBetInfo).PurchaseType;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, -1, PurchaseMultiples[purchaseType] * 0.2, PurchaseMultiples[purchaseType] * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in GemTrioGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int purchaseType = (betInfo as GemTrioBetInfo).PurchaseType;
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in GemTrioGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = (betInfo as GemTrioBetInfo).PurchaseType.ToString();
        }
        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            GemTrioBetInfo starBetInfo = betInfo as GemTrioBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, -1, betInfo.PurchaseFree ? starBetInfo.PurchaseType : -1, betMoney);
        }
    }
}
