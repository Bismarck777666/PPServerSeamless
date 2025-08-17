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
    class MummysJewelsBetInfo : BasePPSlotBetInfo
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

    class MummysJewelsGameLogic : BasePPSlotGame
    {
        protected double[] _multiTotalFreeSpinWinRates;
        protected double[] _multiMinFreeSpinWinRates;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vswaysmjwl";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 15; }
        }
        protected override int ServerResLineCount
        {
            get { return 15; }
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
                return "def_s=8,10,6,12,9,11,15,13,14,7,8,5,5,12,5&cfgs=1&ver=3&mo_s=16&mo_v=10,15,20,25,30,35,75,100,150,200,375,750,1500,180,375,4500,15000,150000&def_sb=6,7,7,12,9&reel_set_size=5&def_sa=14,5,14,12,15&mo_jp=180;375;4500;15000;150000&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{purchase_1:\"96.51\",purchase_0:\"96.50\",regular:\"96.50\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"888968\",max_rnd_win:\"10000\"}}&wl_i=tbm~10000&mo_jp_mask=jp5;jp4;jp3;jp2;jp1&sc=12.00,24.00,28.00,48.00,60.00,120.00,180.00,240.00,300.00,500.00,700.00,1000.00,1600.00,3200.00,5000.00,7200.00&defc=60.00&purInit_e=1,1&wilds=2~0,0,0,0,0~1,1,1,1,1;3~0,0,0,0,0~1,1,1,1,1;4~0,0,0,0,0~1,1,1,1,1&bonuses=0&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;75,20,10,0,0;20,4,2,0,0;20,4,2,0,0;20,4,2,0,0;10,2,1,0,0;10,2,1,0,0;10,2,1,0,0;5,2,1,0,0;5,2,1,0,0;5,2,1,0,0;5,2,1,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=10,800,000.00&reel_set0=7,6,8,9,8,8,5,5,5,7,13,8,5,10,7,9,6,6,6,1,15,9,9,11,12,14,8,8,8,11,7,6,5,14,8,9,9,9,6,1,9,8,9,6,9,6~9,9,4,9,16,7,8,10,14,5,5,5,6,5,9,12,7,13,16,7,8,16,7,7,7,4,8,7,7,5,16,6,8,15,7,8,8,8,12,5,9,9,7,16,8,14,5,5,9,9,9,11,8,9,10,8,7,15,7,13,7,5~14,9,15,8,16,6,16,8,10,9,9,10,8,10,6,8,12,9,16,7,8,6,6,16,7,7,5,16,3,16,13,5,5,5,16,12,13,9,16,6,11,8,3,12,15,9,8,14,7,5,15,16,11,16,12,5,10,9,10,16,7,3,7,16,7,16,7~15,16,2,12,16,9,7,5,16,10,8,5,16,14,6,7,8,11,16,15,13,16,8,16,8,8,8,15,16,8,13,14,9,7,16,10,15,2,11,10,11,5,16,9,9,10,16,7,11,13,6,12,8,6,2~16,8,13,7,6,11,5,16,9,15,16,6,16,5,5,5,10,6,9,15,12,8,14,7,16,7,16,10,5,6,6,6,5,12,10,15,16,14,12,8,9,9,5,13,9,13,15&reel_set2=14,9,6,10,9,9,8,13,8,14,5,5,5,11,6,6,5,7,8,8,9,7,8,8,6,6,6,9,11,15,9,9,1,6,9,7,7,12,8,8,8,11,14,5,8,8,6,9,1,6,7,6,9,9,9,13,9,9,7,1,5,5,15,8,6,9,12,10~8,9,5,7,13,9,10,5,5,5,7,8,7,13,16,7,8,7,7,7,12,5,5,8,9,15,6,11,8,8,8,7,5,16,4,5,11,4,5,7,8,8,14,8,4,7,10,4,10~7,16,5,12,7,11,7,15,6,16,10,8,6,10,8,5,6,8,5,5,5,9,16,7,8,10,12,8,16,3,14,3,11,6,3,8,3,9,10,3,8,3,15,5,5,6,7,7,3~7,5,16,7,16,10,9,16,12,11,10,16,11,16,9,6,16,8,8,8,14,16,10,8,6,15,7,7,9,2,9,2,13,15,6,8,7,11,2,6,12,2,10,2,10,2,6,2~6,5,9,14,16,5,6,15,16,8,7,9,16,15,5,5,5,10,7,16,6,9,10,16,14,16,12,13,5,8,11,6,9,6,6,6,7,11,13,15,7,10,12,5,12,14,9,12,8,10,16,15,8&reel_set1=1,1,1,1,1,1~16,17,17,16,17,17,16,17,17,17,16,17,17,17,17,17,16,17,17,17,17,17,16,17,17,17,17,17,17,17,17,17,17,17,16,17,17,17,17,17,17,17,17,16,17,17,17,17,17,17,17,17,17,17,17,16,17,17~17,17,17,16,17,17,17,17,17,17,17,16,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,16,17,17,17,17,17,17,17,16~17,17,17,17,17,17,17,17,17,17,17,17,17,16,17,16,17,17,17,17,17,17,17,17,17,17,17,17,17,16,17,17,17,17,17,16,17,17,16,17,16,17,17,17,17,17,17,17,17,17,17,17,16~17,17,17,17,17,16,17,17,17,17,17,17,17,17,17,16,17,17,16,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,16,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,17,16,17,17,17,17,16,17,17,17&reel_set4=13,14,15,14,12,14,10,14,13,12,11,10,15,12,10,15,13,11,9,15,11,9,15,9,12,10,14,13,10,13,9,15,11,13,9,11,12,9,15,14,9,12,9,15,14~14,10,4,7,8,9,7,5,10,4,5,4,7,4,10,5,4,5,4,8,4,9,12,7,10,4,5,6,4,8,7,4,5,5,13,8,9,8,9,4,5,4,7,11,4,9,9~9,8,9,14,3,6,6,12,6,10,9,8,3,11,3,8,5,7,9,7,3,10,15,13,14,3,7,11,15,3,10,3,6,6,3,7,8,3,10,3,13,8,5,10~5,8,10,2,11,2,6,2,11,2,13,2,15,13,14,12,7,11,2,7,2,13,7,6,5,2,8,2,7,2,11,6,5,2,11,2,8,15,9,10,15,9,15,2,9,14,10~7,6,6,7,16,7,7,16,5,5,5,8,8,6,5,8,5,7,5,8,6,6,6,5,8,8,5,8,5,16,5,8,16,7,7,7,5,5,7,5,7,6,16,6,7,8,8,8,6,6,8,5,6,16,6,16,6,7&purInit=[{bet:750,type:\"default\"},{bet:1500,type:\"default\"}]&reel_set3=14,9,6,10,9,9,8,13,8,14,5,5,5,11,6,6,5,7,8,8,9,7,8,8,6,6,6,9,11,15,9,9,1,6,9,7,7,12,8,8,8,11,14,5,8,8,6,9,1,6,7,6,9,9,9,13,9,9,7,1,5,5,15,8,6,9,12,10~8,9,5,7,13,9,10,5,5,5,7,8,7,13,16,7,8,7,7,7,12,5,5,8,9,15,6,11,8,8,8,7,5,16,4,5,11,4,5,7,8,8,14,8,4,7,10,4,10~7,16,5,12,7,11,7,15,6,16,10,8,6,10,8,5,6,8,5,5,5,9,16,7,8,10,12,8,16,3,14,3,11,6,3,8,3,9,10,3,8,3,15,5,5,6,7,7,3~7,5,16,7,16,10,9,16,12,11,10,16,11,16,9,6,16,8,8,8,14,16,10,8,6,15,7,7,9,2,9,2,13,15,6,8,7,11,2,6,12,2,10,2,10,2,6,2~6,5,9,14,16,5,6,15,16,8,7,9,16,15,5,5,5,10,7,16,6,9,10,16,14,16,12,13,5,8,11,6,9,6,6,6,7,11,13,15,7,10,12,5,12,14,9,12,8,10,16,15,8&total_bet_min=180.00";
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
        public MummysJewelsGameLogic()
        {
            _gameID     = GAMEID.MummysJewels;
            GameName    = "MummysJewels";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);

            if (dicParams.ContainsKey("rs_iw"))
                dicParams["rs_iw"] = convertWinByBet(dicParams["rs_iw"], currentBet);

            if (dicParams.ContainsKey("mo_tw"))
                dicParams["mo_tw"] = convertWinByBet(dicParams["mo_tw"], currentBet);

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
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


                MummysJewelsBetInfo betInfo = new MummysJewelsBetInfo();
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in MummysJewelsGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                    (oldBetInfo as MummysJewelsBetInfo).PurchaseType = betInfo.PurchaseType;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in MummysJewelsGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            MummysJewelsBetInfo betInfo = new MummysJewelsBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new MummysJewelsBetInfo();
        }
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as MummysJewelsBetInfo).PurchaseType;
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
                        _multiMinFreeSpinWinRates[i]    = (double)purchaseOdds[2 * i];
                        _multiTotalFreeSpinWinRates[i]  = (double)purchaseOdds[2 * i + 1];

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
            int purchaseType = (betInfo as MummysJewelsBetInfo).PurchaseType;
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
                int purchaseType = (betInfo as MummysJewelsBetInfo).PurchaseType;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, -1, PurchaseMultiples[purchaseType] * 0.2, PurchaseMultiples[purchaseType] * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in MummysJewelsGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int purchaseType        = (betInfo as MummysJewelsBetInfo).PurchaseType;
                var spinDataDocument    = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in MummysJewelsGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = (betInfo as MummysJewelsBetInfo).PurchaseType.ToString();
        }
        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            MummysJewelsBetInfo starBetInfo = betInfo as MummysJewelsBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, -1, betInfo.PurchaseFree ? starBetInfo.PurchaseType : -1, betMoney);
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "s", "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set","mo", "mo_t" };
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
