using Akka.Actor;
using Akka.Event;
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
    class JackpotHunterBetInfo : BasePPSlotBetInfo
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

    class JackpotHunterGameLogic : BasePPSlotGame
    {
        protected double[] _multiTotalFreeSpinWinRates;
        protected double[] _multiMinFreeSpinWinRates;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20jhunter";
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
                return "def_s=6,10,3,4,12,7,9,5,7,8,6,10,4,4,12,7,11,11,3,8&cfgs=1&ver=3&def_sb=7,9,10,3,12&reel_set_size=3&def_sa=6,12,3,7,10&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{purchase_1:\"96.00\",purchase_0:\"96.00\",regular:\"96.00\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"13020833\",max_rnd_win:\"5000\"}}&wl_i=tbm~5000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1,1&wilds=2~1000,200,100,20,0~1,1,1,1,1&bonuses=0&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;1000,200,100,20,0;400,160,80,16,0;300,120,60,12,0;250,100,50,0,0;200,80,40,0,0;150,60,30,0,0;100,40,20,0,0;100,40,20,0,0;50,20,10,0,0;50,20,10,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=6,000,000.00&reel_set0=4,8,10,12,7,8,10,7,2,4,5,12,11,10,6,11,7,9,11,7,10,6,12,10,9,5,8,13,11,8,11,13,11,8,9,11,12,11,6,13,3,5,10~9,8,12,10,6,11,9,12,11,4,8,10,11,9,7,12,9,6,7,12,5,2,6,5,10,11,7,4,10,12,10,11,7,12,6,9,12,3,5,11,7,8,1,12,4,6,5,8,10,3,9,1,10,9,1,4,9,11,9,7,12~9,12,8,9,12,9,8,11,10,8,13,9,7,12,7,3,11,1,9,4,10,12,9,4,6,3,11,9,11,3,13,8,12,6,5,10,6,8,4,11,5,1,5,13,12,7,12,2~3,9,10,5,1,4,11,4,8,9,4,5,8,6,9,7,10,7,10,1,12,8,9,6,3,11,12,11,8,11,8,3,10,9,2,6,8,6,12,10,5,11,6,11,12,10,7,9,10~6,13,4,11,12,5,8,7,12,3,7,10,8,10,12,9,10,11,8,5,11,12,3,11,7,5,12,3,7,6,11,4,11,7,12,10,9,4,8,11,7,5,4,3,9,13,12,2,6,5,9,13,6,8,10,7,12,7&reel_set2=13,11,6,5,8,9,3,12,13,7~6,4,9,7,10,12,5,8,11~8,7,9,6,5,12,13,8,13,11,13,3~3,6,4,9,10,12,5,8,11~13,7,13,10,12,6,3,5,7,13,9,8&reel_set1=10,11,6,9,7,9,3,5,11,12,6,9,6,2,8,9,3,7,10,5,2,11,12,2,4,6,14,12,11,4,7,4,3,8,9,10,5,4,3,14,8,10,12,7,10,7,11~3,9,7,6,9,11,6,5,10,8,11,4,8,9,8,12,10,3,1,9,11,8,2,10,11,4,9,10,12,14,3,10,1,2,1,12,5,3,12,7,3,7,10,9,11,4,6,2,4,5,14,7,11,7,12,4,12,4,8,2,12,11,2,5,6,8,10,9~4,14,8,9,11,3,1,14,8,5,1,9,12,9,2,3,4,7,11,6,4,2,7,10,7,6,9,12,8,11,6,12,2,1,8,2,12,5,9,11,3,2,8,14,9,3,12,5,4,11,10,7,11,10,7~9,11,9,11,14,10,1,2,12,8,3,4,2,4,5,12,8,3,7,12,14,11,6,7,4,10,9,12,4,8,7,9,6,12,7,9,7,12,2,12,7,9,4,5,12,11,6,8,11,8,12,2,3,6,9,3,10,4,8,11,10,3,1,8,1,10~11,9,12,11,8,10,6,3,5,10,4,6,3,14,8,12,8,4,12,8,7,10,9,11,10,7,6,8,2,9,7,2,10,9&purInit=[{bet:400,type:\"default\"},{bet:1200,type:\"default\"}]&total_bet_min=10.00";
            }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 20; }
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
            get { return new double[] { 20, 60 }; }
        }
        #endregion

        public JackpotHunterGameLogic()
        {
            _gameID     = GAMEID.JackpotHunter;
            GameName    = "JackpotHunter";
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
            if (dicParams.ContainsKey("apwa")) //FiveLionsDance 보너스당첨
                dicParams["apwa"] = convertWinByBet(dicParams["apwa"], currentBet);
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                JackpotHunterBetInfo betInfo = new JackpotHunterBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();
		
		
                if (message.DataNum >= 3)
                {
                    betInfo.PurchaseFree = true;
                    betInfo.PurchaseType = (int)message.GetData(2);
                }
                else
                    betInfo.PurchaseFree = false;
		
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in JackpotHunterGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1}", strUserID, betInfo.LineCount);
                    return;
                }

                BasePPSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strUserID, out oldBetInfo))
                {
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine   = betInfo.BetPerLine;
                    oldBetInfo.LineCount    = betInfo.LineCount;
                    oldBetInfo.MoreBet      = betInfo.MoreBet;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                    (oldBetInfo as JackpotHunterBetInfo).PurchaseType = betInfo.PurchaseType;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in JackpotHunterGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            JackpotHunterBetInfo betInfo = new JackpotHunterBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new JackpotHunterBetInfo();
        }
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as JackpotHunterBetInfo).PurchaseType;
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
        protected override async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int agentID, BasePPSlotBetInfo betInfo, double baseBet, UserBonus userBonus)
        {
            int purchaseType = (betInfo as JackpotHunterBetInfo).PurchaseType;
            double payoutRate = getPayoutRate(agentID);

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
                int purchaseType = (betInfo as JackpotHunterBetInfo).PurchaseType;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, -1, PurchaseMultiples[purchaseType] * 0.2, PurchaseMultiples[purchaseType] * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in JackpotHunterGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int purchaseType = (betInfo as JackpotHunterBetInfo).PurchaseType;
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in JackpotHunterGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = (betInfo as JackpotHunterBetInfo).PurchaseType.ToString();
        }
        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            JackpotHunterBetInfo starBetInfo = betInfo as JackpotHunterBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, -1, betInfo.PurchaseFree ? starBetInfo.PurchaseType : -1, betMoney);
        }
    }
}
