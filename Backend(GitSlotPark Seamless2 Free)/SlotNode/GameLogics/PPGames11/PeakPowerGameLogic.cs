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
    class PeakPowerBetInfo : BasePPSlotBetInfo
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
    class PeakPowerGameLogic : BasePPSlotGame
    {
        protected double[] _multiTotalFreeSpinWinRates;
        protected double[] _multiMinFreeSpinWinRates;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10powerlines";
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
            get { return 10; }
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
                return "def_s=9,3,6,7,3,8,5,4,8,4,9,3,9,9,3&cfgs=7146&ver=3&def_sb=7,4,8,7,3&reel_set_size=2&def_sa=8,6,3,7,4&scatters=1~3,3,3,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"5434782\",max_rnd_win:\"10000\"}}&wl_i=tbm~10000&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&purInit_e=1,1&wilds=&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;100,40,10,2,0;50,20,8,1,0;30,10,6,0,0;20,8,5,0,0;15,6,4,0,0;10,4,3,0,0;4,3,2,0,0&total_bet_max=30,000,000.00&reel_set0=5,3,4,6,6,3,3,5,5,7,8,3,9,9,4,4,4,4,3,9,7,5,4,3,8,5,8,4,3,9,7,5,3,7,7,7,7,7,3,4,8,4,8,5,7,9,9,6,3,3,6,4,9,6,6,6,7,8,3,6,9,9,7,5,9,4,5,9,6,7,6,8,3,3,3,3,8,3,7,4,5,6,7,6,7,8,9,6,9,8,4,8,8,8,6,5,7,8,4,9,4,9,5,5,8,3,7,7,4,9,9,9,9,3,5,4,7,6,8,6,6,8,6,4,5,1,6,5,5,5,6,4,4,8,9,9,7,7,3,7,5,8,8,1,3,4,5~9,1,6,7,7,9,9,4,8,3,4,9,6,7,3,4,7,4,3,6,8,6,8,8,6,8,8,8,8,7,4,5,9,4,9,3,5,7,4,6,8,3,4,6,5,3,7,7,3,3,7,6,5,3,6,5,7,7,7,5,6,7,9,6,6,9,8,9,5,5,9,5,6,3,7,6,7,4,9,6,6,1,3,7,3,3,3,6,4,3,4,5,9,4,9,8,4,4,6,5,8,5,8,5,1,3,8,4,4,5,9,5,8,5,6,6,6,5,9,5,7,3,8,4,3,3,9,5,8,9,5,4,7,9,9,6,8,4,8,8,9,8,7,5,5,5,5,3,7,3,7,8,7,4,9,6,6,9,8,5,3,6,4,3,9,7,6,5,8,3,3,9,3,4,4,4,8,5,6,8,4,3,5,4,7,8,6,4,6,7,3,9,8,7,5,1,8,3,8,4,5,7,7,9~3,9,8,3,5,7,7,5,5,3,4,7,4,8,6,7,7,7,8,4,5,7,6,9,4,8,6,3,7,8,4,7,3,8,3,3,3,3,5,4,6,5,9,8,3,8,5,7,4,9,4,8,9,9,5,5,5,4,8,5,7,4,6,5,3,9,7,8,3,7,5,8,7,8,8,8,6,9,6,6,7,9,7,8,4,4,7,8,3,6,7,3,6,6,6,4,9,7,3,7,8,3,5,9,3,5,4,5,6,9,8,4,4,4,6,8,3,9,9,5,6,5,6,6,4,5,4,5,9,6,9,9,9,3,1,9,3,8,4,7,3,1,4,6,6,3,5,9,6~3,5,7,4,5,7,7,4,8,3,3,1,7,4,6,4,6,6,6,6,6,5,5,4,6,5,9,4,9,3,7,6,9,7,5,5,9,8,9,4,4,4,8,5,9,3,8,5,8,3,7,4,9,6,7,7,3,7,5,7,7,7,8,3,3,6,9,5,3,1,9,7,3,3,6,9,3,8,8,4,5,5,5,9,6,7,8,9,7,6,7,6,5,8,8,6,3,8,1,5,9,9,9,8,7,3,4,4,6,5,7,4,6,6,9,4,8,4,8,7,3,3,3,3,8,8,3,5,5,6,5,8,7,4,9,9,8,6,6,3,6,8,8,8,9,7,9,6,4,3,4,5,5,4,4,5,4,9,6,7,3,9,6~9,5,9,5,4,6,9,5,3,7,4,8,8,4,8,8,8,8,4,9,3,8,6,1,7,3,3,4,5,9,8,7,8,9,9,9,5,5,3,7,4,7,6,8,4,3,9,9,5,8,3,8,7,7,7,7,3,5,5,4,6,7,7,8,9,8,3,4,6,6,1,3,3,3,5,9,5,9,7,5,6,3,7,9,4,8,6,3,6,5,5,5,8,9,7,8,4,6,1,7,3,4,7,9,9,6,6,6,4,5,7,3,6,3,3,4,6,7,4,7,8,8,9,6,5&reel_set1=8,9,5,6,9,3,7,7,4,8,9,4,6,5,3,7,8,6,4,4,7,8,8,8,9,9,5,9,4,8,7,4,4,9,6,7,3,7,8,3,8,4,4,3,5,5,3,3,3,9,9,3,8,5,9,7,9,3,4,4,5,8,6,8,4,6,8,8,4,5,7,4,4,4,4,7,8,8,9,3,9,8,5,5,7,3,6,3,3,9,6,6,9,4,6,8,7,6,6,6,5,6,6,5,9,8,6,3,7,6,7,6,4,8,6,5,6,7,7,6,6,7,7,7,4,8,4,3,5,5,9,3,5,3,9,8,8,9,5,3,4,4,7,6,3,3,5,5,5,7,7,4,5,9,4,6,9,5,5,3,4,6,7,5,8,3,5,7,3,9,8,3~7,9,3,7,8,8,4,9,6,6,7,4,4,8,6,8,3,5,7,6,9,3,8,4,8,9,9,9,4,4,6,7,8,7,5,7,5,5,8,7,8,9,9,3,6,9,4,3,7,5,5,8,5,6,3,3,3,9,8,4,8,4,5,6,9,5,9,5,5,7,4,8,7,4,5,4,6,7,5,6,7,9,9,4,4,4,5,5,9,6,3,8,6,3,6,4,4,3,7,9,5,7,3,4,7,5,8,8,3,6,7,6,6,6,5,5,9,7,6,3,6,9,3,4,3,7,9,8,8,3,5,9,8,3,4,6,9,3,4,6,5,5,5,7,8,9,9,3,4,6,7,3,4,4,7,5,8,3,4,9,7,5,3,7,4,3,4,6,5,8,8,8,6,9,8,5,9,8,5,4,9,3,3,6,3,7,6,7,6,8,3,8,6,6,9,8,3,9,5~4,7,6,3,5,7,5,9,5,3,4,9,6,7,6,5,3,6,5,7,9,9,9,9,9,4,3,3,9,3,6,9,8,9,9,4,4,7,4,5,9,8,5,4,8,4,6,3,3,3,9,8,8,9,8,6,9,9,4,6,5,9,8,5,7,4,6,6,5,6,6,9,6,6,6,6,3,8,9,4,8,6,5,5,7,5,4,3,3,5,8,3,7,8,9,7,6,7,4,4,4,4,9,7,8,4,7,3,3,5,3,8,9,5,5,8,8,7,8,7,4,5,7,7,7,7,6,3,7,3,9,7,5,4,5,6,3,3,8,4,6,7,6,7,3,6,6,8,8,8,5,9,3,3,6,7,8,9,3,4,5,3,4,8,6,3,4,9,4,7,8,3,5,5,5,5,8,3,6,6,5,4,7,5,6,9,7,8,9,4,5,9,4,4,9,7,8,7,8~9,4,4,8,4,9,3,8,9,8,6,9,9,6,7,8,4,8,4,6,8,3,3,5,5,5,4,4,6,4,6,8,9,9,3,6,3,6,8,9,3,5,7,4,7,6,6,9,6,6,3,3,3,6,6,3,5,3,5,8,7,8,4,3,5,3,5,9,8,3,7,9,8,9,5,5,6,8,8,8,3,5,7,4,5,9,7,7,9,6,5,6,5,9,3,8,6,3,4,6,8,5,9,4,9,9,9,3,5,3,3,9,7,9,7,6,8,5,7,9,9,5,5,4,4,6,7,6,8,4,7,4,4,4,4,5,8,3,8,7,4,8,3,8,4,5,4,5,4,3,5,8,8,3,6,7,7,4,4,6,6,6,8,5,9,8,5,8,6,9,4,7,7,3,7,6,7,3,5,3,6,7,5,7,4,8,7,7,7,8,7,3,4,6,5,3,4,9,5,9,5,7,9,6,6,7,7,9,7,9,8,4,3,5~9,3,5,3,7,5,6,8,3,9,3,5,8,4,6,6,6,8,3,6,4,7,3,4,4,5,3,5,8,8,6,9,7,4,4,4,3,8,7,8,4,7,4,6,3,4,4,5,6,8,6,7,3,3,3,3,8,3,5,3,5,4,8,8,3,5,7,7,9,4,8,9,9,9,9,5,5,6,4,9,7,3,9,6,7,6,5,8,4,9,7,7,7,4,8,8,4,7,9,4,9,7,9,6,6,9,5,3,8,8,8,6,7,8,6,9,9,4,5,3,4,7,9,9,6,9,6,5,5,5,3,9,7,6,9,4,5,8,5,7,6,3,7,5,6,8,5&purInit=[{bet:1000,type:\"default\"},{bet:3000,type:\"free_spins\"}]&total_bet_min=20.00";
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
        protected int FreePurCount
        {
            get { return 2; }
        }

        public double[] PurchaseMultiples
        {
            get { return new double[] { 100, 300 }; }
        }
        #endregion
        public PeakPowerGameLogic()
        {
            _gameID = GAMEID.PeakPower;
            GameName = "PeakPower";
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
        }
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as PeakPowerBetInfo).PurchaseType;
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

        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


                PeakPowerBetInfo betInfo    = new PeakPowerBetInfo();
                betInfo.BetPerLine          = (float)message.Pop();
                betInfo.LineCount           = (int)message.Pop();

                if (message.DataNum >= 3)
                {
                    betInfo.PurchaseFree = true;
                    betInfo.PurchaseType = (int)message.GetData(2);
                }
                else
                {
                    betInfo.PurchaseFree = false;
                }
		
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in PeakPowerGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                    (oldBetInfo as PeakPowerBetInfo).PurchaseType = betInfo.PurchaseType;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in PeakPowerGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            PeakPowerBetInfo betInfo = new PeakPowerBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new PeakPowerBetInfo();
        }
        protected override async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int agentID, BasePPSlotBetInfo betInfo, double baseBet, UserBonus userBonus, bool isAffiliate)
        {
            int purchaseType  = (betInfo as PeakPowerBetInfo).PurchaseType;
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
                int purchaseType = (betInfo as PeakPowerBetInfo).PurchaseType;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, -1, PurchaseMultiples[purchaseType] * 0.2, PurchaseMultiples[purchaseType] * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int purchaseType = (betInfo as PeakPowerBetInfo).PurchaseType;
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRangeSpinData(int websiteID, double minOdd, double maxOdd, BasePPSlotBetInfo betInfo, bool isAffiliate)
        {
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                    new SelectSpinTypeOddRangeRequestSkipSpecificPuri(GameName, minOdd, maxOdd, 1), TimeSpan.FromSeconds(10.0));

            if (spinDataDocument == null)
                return null;

            return convertBsonToSpinData(spinDataDocument);
        }

        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = (betInfo as PeakPowerBetInfo).PurchaseType.ToString();
        }

        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            PeakPowerBetInfo peakBetInfo = betInfo as PeakPowerBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, -1, betInfo.PurchaseFree ? peakBetInfo.PurchaseType : -1, betMoney);
        }
    }
}
