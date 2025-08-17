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
    class SugarRush1000BetInfo : BasePPSlotBetInfo
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
    class SugarRush1000GameLogic : BasePPSlotGame
    {
        protected double[] _multiTotalFreeSpinWinRates;
        protected double[] _multiMinFreeSpinWinRates;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20sugarrushx";
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
                return 7;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=5,6,5,3,9,6,9,8,7,9,5,3,6,6,8,4,5,6,7,8,9,5,6,5,3,9,6,9,8,7,9,5,3,6,6,8,4,5,6,7,8,9,7,3,9,6,9,3,5&cfgs=9968&ver=3&def_sb=6,8,1,2,7,1,3&reel_set_size=1&def_sa=8,6,5,7,8,9,1&scatters=1~0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0~0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0~1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"12800000\",max_rnd_win:\"25000\"}}&wl_i=tbm~25000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1,1&wilds=2~0~1&bonuses=0&paytable=0;0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;0;3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,3000,1400,700,300,150,100,50,40,35,30,20,0,0,0,0;2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,1200,600,250,120,80,40,30,25,20,15,0,0,0,0;1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,1200,800,400,200,90,60,30,25,20,15,10,0,0,0,0;800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,800,400,200,100,60,40,25,20,15,10,8,0,0,0,0;600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,600,300,160,70,50,30,20,15,10,8,6,0,0,0,0;500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,240,120,60,40,25,15,10,8,6,5,0,0,0,0;400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,400,200,100,50,30,20,10,8,6,5,4,0,0,0,0;0;0&total_bet_max=50,000,000.00&reel_set0=9,9,6,6,5,9,7,7,7,5,8,8,8,7,9,5,5,6,4,9,9,7,8,8,1,3,3,4,3,3,3,9,9,9,6,6,7,9,9,5,5,4,5,5,9,7,8,5,5,5,5,6,4,9,9,4,8,8,6,9,9,1,7,6,6,8,8,9,9,9,9,5,8,4,8,8,3,8,8,6,6,8,8,8,5,5,6,4~4,4,7,7,8,8,8,7,8,8,9,9,5,5,8,8,1,5,5,3,9,9,9,7,3,3,4,4,7,7,9,9,9,8,8,7,8,4,5,3,3,5,5,4,7,7,5,3,7,8,8,8,8,5,5,8,8,1,9,7,9,9,7,8,8,7,3,3,8,8,8,7,8,8,8,8,5,9,9,9,6,8,8,8,6,3,3,6,7,7,4,4,4,4,7~9,9,9,7,7,7,7,4,5,5,7,7,5,5,5,9,9,4,4,6,6,4,4,8,8,1,5,9,5,3,8,7,7,6,6,8,9,9,9,9,6,3,3,3,9,8,8,6,6,8,9,9,8,3,7,7,9,1,7,7,7,8,9,4,7,7,9,5,5,9,8,8,5,6,9,7,7,3,3,9,9,1,1,6,6,4,4,4,9,6,6~7,7,3,3,6,6,6,5,5,5,6,7,3,3,7,4,4,5,5,5,7,7,1,3,3,4,4,4,4,5,8,8,6,6,8,8,8,8,1,7,8,7,7,6,6,7,7,9,9,7,7,4,4,6,6,8,3,4,4,9,9,9,9,9,4,4,7,4,9,6,6,9,9,7,7,7,5,5,5,9,9,9,8,6,7,7,8,7,5,9,9,6,6,5,5~4,6,6,8,3,3,6,9,7,4,5,5,3,3,7,1,9,4,4,7,7,9,4,4,6,4,5,3,8,6,8,8,8,4,4,7,3,3,3,4,4,8,8,3,4,4,8,8,6,6,6,8,8,7,7,5,5,1,7,7,7,6,4,4,4,8,8,4,3,3,9,9,8,8,9,9,9,5,5,3,3,3,9,3,7,7,3,4,4~5,6,6,4,4,6,6,6,9,6,6,6,7,9,9,8,8,9,8,8,7,7,3,3,8,8,8,9,9,9,9,9,8,8,1,9,4,4,6,6,6,7,5,3,3,7,7,5,5,4,4,8,8,7,7,8,8,7,7,5,8,8,8,8,6,6,9,9,9,9,7,7,9,9,5,5,5,1,6,8,8,5,5,5,5,3,3,9,9,3,8,8,4,4,5,3,3,5,7,7~8,8,6,6,6,7,4,4,4,4,9,9,9,9,5,5,5,1,7,4,6,9,9,4,9,9,7,7,4,4,3,3,9,9,7,7,9,7,7,3,9,8,8,9,9,9,6,6,7,7,6,4,4,6,6,4,9,9,9,9,9,9,1,7,4,4,5,5,8,8,6,6,8,8,5,5,4,4,8,8,8,8,8,8,8,8,9,9,4,4,7,4,4&purInit=[{bet:2000,type:\"default\"},{bet:10000,type:\"default\"}]&total_bet_min=10.00";
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
            get { return new double[] { 100, 500 }; }
        }

        #endregion
        public SugarRush1000GameLogic()
        {
            _gameID = GAMEID.SugarRush1000;
            GameName = "SugarRush1000";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["st"] = "rect";
	        dicParams["sw"] = "7";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as SugarRush1000BetInfo).PurchaseType;
            return this.PurchaseMultiples[purchaseType];
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


                SugarRush1000BetInfo betInfo = new SugarRush1000BetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();


                if (message.DataNum >= 3)
                {
                    betInfo.PurchaseType = (int)message.GetData(2);
                    betInfo.PurchaseFree = true;
                }
                else
                {
                    betInfo.PurchaseFree = false;
                }
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in SugarRush1000GameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                    (oldBetInfo as SugarRush1000BetInfo).PurchaseType = betInfo.PurchaseType;

                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SugarRush1000GameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            SugarRush1000BetInfo betInfo = new SugarRush1000BetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new SugarRush1000BetInfo();
        }
        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBet     = (double)infoDocument["defaultbet"];
                _normalMaxID            = (int)infoDocument["normalmaxid"];
                _emptySpinCount         = (int)infoDocument["emptycount"];
                _naturalSpinCount       = (int)infoDocument["normalselectcount"];

                _multiTotalFreeSpinWinRates     = new double[FreePurCount];
                _multiMinFreeSpinWinRates       = new double[FreePurCount];

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
            int purchaseType = (betInfo as SugarRush1000BetInfo).PurchaseType;
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
                int purchaseType = (betInfo as SugarRush1000BetInfo).PurchaseType;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, -1, PurchaseMultiples[purchaseType] * 0.2, PurchaseMultiples[purchaseType] * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SugarRush1000GameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int purchaseType = (betInfo as SugarRush1000BetInfo).PurchaseType;
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SugarRush1000GameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = (betInfo as SugarRush1000BetInfo).PurchaseType.ToString();
        }
        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            SugarRush1000BetInfo sugarBetInfo = betInfo as SugarRush1000BetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, -1, betInfo.PurchaseFree ? sugarBetInfo.PurchaseType : -1, betMoney);
        }
    }
}
