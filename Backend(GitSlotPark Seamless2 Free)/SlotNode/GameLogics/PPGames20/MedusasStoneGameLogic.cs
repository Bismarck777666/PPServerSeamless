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
    class MedusasStoneBetInfo : BasePPSlotBetInfo
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
    class MedusasStoneGameLogic : BasePPSlotGame
    {
        protected double[] _multiTotalFreeSpinWinRates;
        protected double[] _multiMinFreeSpinWinRates;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20medusast";
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
                return 4;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=4,11,3,3,10,5,8,9,5,8,6,7,7,4,11,3,8,5,3,9&cfgs=10463&ver=3&def_sb=6,8,11,6,9&reel_set_size=5&def_sa=6,9,11,6,7&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"26881700\",max_rnd_win:\"7000\"}}&wl_i=tbm~7000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1,1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;500,100,50,0,0;250,75,25,0,0;200,50,25,0,0;100,40,20,0,0;40,20,10,0,0;40,20,10,0,0;20,10,5,0,0;20,10,5,0,0;20,10,5,0,0;0,0,0,0,0&total_bet_max=20,000,000.00&reel_set0=4,7,5,9,10,8,5,3,3,9,3,7,11,1,8,7,10,11,4,6,7,3,10,4,8,9,7,8,6,5~3,8,6,8,5,10,7,11,5,7,4,10,7,3,3,9,6,9,6,11,8,3,8~3,10,5,8,1,9,6,3,11,10,3,8,7,9,7,3,4,5,4,9,7,10,7,6,11,7,8,3,6,5,1,9,8,4,5,8,4,11,1,10~11,10,4,5,3,5,6,4,10,9,7,6,10,9,7,9,11,5,3,8,3,8,8,3,3,6,8,8~9,1,5,3,1,4,7,8,7,8,7,6,10,7,5,11,7,6,3,8,5,3,9,5,11,5,3,7,10,3,9&reel_set2=4,10,3,10,5,1,7,5,8,9,8,4,9,6,10,3,8,7,9,5,11,7,6,3,11~6,7,11,3,3,5,6,3,10,6,10,9,7,3,4,9,5,7,11,10,9,6,8,5,11,3,6,9,6,4,10,8~6,7,9,10,5,8,1,11,3,4,3,11,8,5,4,1,5,5,10,8,9,10,9,3~6,9,5,9,11,7,3,3,4,11,4,7,11,6,3,8,3,10,5,7,9,6,6,5,6,8,10,8~6,5,3,5,1,10,11,7,5,9,4,3,6,9,5,7,4,3,9,5,9,7,10,11,1,10,8,3,8,3&reel_set1=10,11,7,10,8,9,10,9,3,6,7,9,11,7,11,8,8,5,11,10,5,9,10,6,4,5,8,4,7~10,6,7,6,4,9,11,10,9,10,3,10,11,5,11,9,3,7,7,8,11,7,6,5,8,6,8,7,10,11,4,10,11,8,8,5,10,7,8,9,6,9~10,11,7,6,9,11,10,9,4,6,8,7,9,4,8,7,8,8,11,6,9,7,8,10,11,9,10,5,11,8,3,7,5,8,10,7,5,9,11,10,5~3,7,7,6,10,11,6,10,8,11,6,10,5,4,9,8,5,9,11,8,10,9,7,10,7,8,11~8,9,10,11,5,6,10,5,8,10,4,11,7,9,6,7,7,11,10,3,11,8,8,5,10,9,7&reel_set4=8,10,3,3,9,4,5~8,8,11,10~9,7,7,11~9,7,3,10,8,4,9,3,3,10,6,5,8,6,6,8~8,10,3,3,6,9,7,4,5,5&purInit=[{bet:1500,type:\"default\"},{bet:4000,type:\"default\"}]&reel_set3=4,7,11,9,8,9,3,11,9,10,5,11,10,8,7,10,8,10,8,7,5,11,7,8,5,11,6,4,5,10,9,10,6,9~5,11,3,7,6,3,10,11,8,6,11,8,9,7,10,9,8,6,11,9,3,7,10,6,9,10,4,5,11,6,10,5,6,9,7,10~8,9,4,9,8,10,5,7,8,6,10,5,11,7,10,9,9,6,11,9,11,7,11,9,7,10,4,5,6,10,7,10,8,11,9,10,3,4,11,8~8,5,8,9,8,10,8,6,11,3,7,11,10,3,5,7,10,9,9,4,7,10,6,7,11,10,9~7,9,9,10,6,9,8,7,5,7,10,11,8,9,9,7,8,11,3,6,7,10,11,8,9,10,7,9,10,4,8,4,6,10,11,5,6,11,8,11&total_bet_min=10.00";
            }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 75; }
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
            get { return new double[] { 75, 200 }; }
        }
        #endregion
        public MedusasStoneGameLogic()
        {
            _gameID = GAMEID.MedusasStone;
            GameName = "MedusasStone";
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


                MedusasStoneBetInfo betInfo = new MedusasStoneBetInfo();
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in MedusasStoneGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                    (oldBetInfo as MedusasStoneBetInfo).PurchaseType = betInfo.PurchaseType;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in MedusasStoneGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            MedusasStoneBetInfo betInfo = new MedusasStoneBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new MedusasStoneBetInfo();
        }
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as MedusasStoneBetInfo).PurchaseType;
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
            int purchaseType = (betInfo as MedusasStoneBetInfo).PurchaseType;
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
                int purchaseType = (betInfo as MedusasStoneBetInfo).PurchaseType;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, -1, PurchaseMultiples[purchaseType] * 0.2, PurchaseMultiples[purchaseType] * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in MedusasStoneGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int purchaseType = (betInfo as MedusasStoneBetInfo).PurchaseType;
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in MedusasStoneGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = (betInfo as MedusasStoneBetInfo).PurchaseType.ToString();
        }
        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            MedusasStoneBetInfo starBetInfo = betInfo as MedusasStoneBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, -1, betInfo.PurchaseFree ? starBetInfo.PurchaseType : -1, betMoney);
        }
    }
}
