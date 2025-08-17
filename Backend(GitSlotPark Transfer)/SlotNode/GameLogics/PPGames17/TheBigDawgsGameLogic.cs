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
    public class TheBigDawgsBetInfo : BasePPSlotBetInfo
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
            writer.Write(this.PurchaseType);
        }

    }
    class TheBigDawgsGameLogic : BasePPSlotGame
    {
        protected double[] _totalFreeSpinWinRates   = new double[2];
        protected double[] _minFreeSpinWinRates     = new double[2];

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20bigdawgs";
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
                return 5;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=4,11,6,5,8,6,10,5,4,10,3,11,11,3,12,7,8,7,5,10,6,12,10,3,11&cfgs=10965&ver=3&def_sb=3,10,11,7,9&reel_set_size=6&def_sa=3,11,11,4,8&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{purchase_1:\"93.96\",purchase_0:\"94.00\",regular:\"93.99\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"9746590\",max_rnd_win:\"5000\"}}&wl_i=tbm~5000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1,1&wilds=2~500,50,10,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;500,50,10,0,0;400,40,8,0,0;300,30,7,0,0;250,25,6,0,0;200,20,5,0,0;100,15,4,0,0;100,15,4,0,0;50,10,3,0,0;50,10,3,0,0;50,10,3,0,0;0,0,0,0,0&total_bet_max=25,000,000.00&reel_set0=8,6,4,10,9,12,12,4,6,9,11,11,5,12,3,8,7,8,8,12,9,10,12,9,12,3,4~8,10,6,3,5,12,12,9,8,11,3,4,8,7,11,8~12,6,11,8,12,5,10,4,3,8,12,6,9,10,12,7,9,11,9,7,4,10~7,12,11,9,6,11,10,3,9,5,12,4,11,11,8,9,9,7,11,7,8,3,7,11,10,11~3,10,6,4,10,9,11,8,12,8,7,8,9,9,6,12,8,6,4,9,11,5,9,8&reel_set2=10,4,11,11,11,3,12,3,3,3,8,8,11,4,4,4,11,7,4,8,8,8,8,11,7,6,10,10,10,8,10,8,5,3~5,7,11,11,12,12,12,12,5,7,12,9,11,12,6,6,6,8,10,9,10,7,10,10,10,3,6,11,4,6,12~4,5,10,6,10,8,9,11,7,8,11,8,12,4,7,6,10,12,11,3~8,11,12,11,6,12,11,11,11,11,11,3,12,5,12,9,6,10,7,12,12,12,12,12,11,4,12,7,12,11,5,9,11~3,8,8,10,6,8,11,8,8,8,8,8,4,12,9,5,12,8,11,10,10,10,10,10,11,7,10,10,6,7,12,10,8,11,6&reel_set1=11,11,12,5,5,5,12,4,5,5,7,7,7,6,7,7,8,10,10,10,8,9,9,10,10~11,8,11,11,11,3,4,12,9,6,6,6,8,12,5,8,8,8,11,6,10,9,9,9,7,8,10,10,10,11,11,5,10,6~8,5,12,9,5,8,11,12,7,9,4,9,3,10,6,12,11,8,10,11,10,7,6~9,7,12,10,12,5,11,11,11,6,11,10,8,5,12,11,4,4,4,11,12,6,6,4,12,10,6,6,6,9,4,9,3,6,11,11,4~6,12,3,7,11,4,5,5,5,8,9,11,12,5,7,10,7,7,7,12,10,9,10,11,8,5,10,10,10,6,7,11,5,7,12,12,8&reel_set4=4,8,6,3,4,12,5,6,12,8,4,10,11,9,7,9,3~9,8,7,4,3,7,5,7,5,11,10,12,4,11,6,7,3,7,5,10~4,7,6,10,4,3,9,8,6,12,4,5,12,9,4,7,10,12,11,7,5,8,4,6,12,3,6~11,12,3,4,4,10,7,11,6,3,10,8,6,11,3,5,7,9,7,9,7,11,7~9,6,3,6,9,9,8,6,7,6,4,6,5,5,10,3,4,4,11,9,6,12,6,8&purInit=[{bet:1600,type:\"default\"},{bet:5000,type:\"default\"}]&reel_set3=8,12,6,12,12,3,5,8,9,7,5,7,8,4,12,9,9,10,6,9,8,11,12,10,4,11,12~10,12,12,5,8,9,8,11,7,10,8,5,12,4,8,11,11,3,8,3,4,7,9,6~7,11,8,10,12,10,5,12,6,4,3,9,5,4,11,12,6,9,8,12,11,12,11,12,7,9,9~11,9,10,10,12,8,11,11,7,3,7,12,7,6,8,11,3,11,9,5,4,9,11~8,9,4,12,10,6,9,12,10,11,11,8,6,4,8,6,8,8,5,9,7,9,9,3,6,8,9&reel_set5=12,3,8,9,3,10,4,6,7,4,5,12,6,11,4,9,8~7,8,5,11,9,4,7,7,8,3,12,5,5,6,3,10,10,5,11,4,3,11,3,10,8,10~11,12,3,4,5,6,7,8,9,10~12,10,3,7,4,10,3,7,7,6,10,11,9,7,5,9,7,6,7,11,9,11,8,11,4~9,8,9,8,3,11,8,9,9,10,9,6,4,6,5,5,6,7,6,6,3,4,4,6,6,12&total_bet_min=10.00";
            }
        }
	
        protected override double PurchaseFreeMultiple
        {
            get { return 80; }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override bool HasPurEnableOption
        {
            get { return true; }
        }
        protected double[] PurchaseFreeMultiples
        {
            get { return new double[] { 80, 250 }; }
        }

        #endregion
        public TheBigDawgsGameLogic()
        {
            _gameID = GAMEID.TheBigDawgs;
            GameName = "TheBigDawgs";
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

            if (dicParams.ContainsKey("rs_win"))
                dicParams["rs_win"] = convertWinByBet(dicParams["rs_win"], currentBet);            
        }
        protected override void supplementInitResult(Dictionary<string, string> dicParams, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {
            base.supplementInitResult(dicParams, betInfo, spinResult);
            if(!dicParams.ContainsKey("reel_set"))
                dicParams["reel_set"] = "0";
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                TheBigDawgsBetInfo betInfo  = new TheBigDawgsBetInfo();
                betInfo.BetPerLine          = (float)message.Pop();
                betInfo.LineCount           = (int)message.Pop();

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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in TheBigDawgsGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                    (oldBetInfo as TheBigDawgsBetInfo).PurchaseType = betInfo.PurchaseType;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in TheBigDawgsGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            if (betInfo.PurchaseFree)
                dicParams["puri"] = (betInfo as TheBigDawgsBetInfo).PurchaseType.ToString();
        }
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            return PurchaseFreeMultiples[(betInfo as TheBigDawgsBetInfo).PurchaseType];
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            var betInfo = new TheBigDawgsBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new TheBigDawgsBetInfo();
        }
        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBet     = (double)infoDocument["defaultbet"];
                _normalMaxID            = (int)infoDocument["normalmaxid"];
                _emptySpinCount         = (int)infoDocument["emptycount"];
                _naturalSpinCount       = (int)infoDocument["normalselectcount"];
                var purchaseOdds        = infoDocument["purchaseodds"] as BsonArray;
                for (int i = 0; i < 2; i++)
                {
                    _totalFreeSpinWinRates[i] = (double)purchaseOdds[2 * i + 1];
                    _minFreeSpinWinRates[i]   = (double)purchaseOdds[2 * i];

                    if (PurchaseFreeMultiples[i] > _totalFreeSpinWinRates[i])
                        _logger.Error("freespin win rate doesn't satisfy condition {0}", this.GameName);
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
            }
        }
        protected override async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int websiteID, BasePPSlotBetInfo betInfo, double baseBet, UserBonus userBonus)
        {
            int purchaseType = (betInfo as TheBigDawgsBetInfo).PurchaseType;
            double payoutRate = getPayoutRate(websiteID);
            double targetC = PurchaseFreeMultiples[purchaseType] * payoutRate / 100.0;
            if (targetC >= _totalFreeSpinWinRates[purchaseType])
                targetC = _totalFreeSpinWinRates[purchaseType];

            if (targetC < _minFreeSpinWinRates[purchaseType])
                targetC = _minFreeSpinWinRates[purchaseType];

            double x = (_totalFreeSpinWinRates[purchaseType] - targetC) / (_totalFreeSpinWinRates[purchaseType] - _minFreeSpinWinRates[purchaseType]);
            double y = 1.0 - x;

            BasePPSlotSpinData spinData = null;
            if (Pcg.Default.NextDouble(0.0, 1.0) <= x)
                spinData = await selectMinStartFreeSpinData(betInfo);
            else
                spinData = await selectRandomStartFreeSpinData(betInfo);
            return spinData;
        }

        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as TheBigDawgsBetInfo).PurchaseType;
            try
            {
                BsonDocument document = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectPurchaseSpinRequest(this.GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType), TimeSpan.FromSeconds(10.0));
                BasePPSlotSpinData spinData = convertBsonToSpinData(document);
                return spinData;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in TheBigDawgsGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectMinStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as TheBigDawgsBetInfo).PurchaseType;
            try
            {
                double purMultiple = getPurchaseMultiple(betInfo);
                BsonDocument document = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinTypeOddRangeRequest(this.GameName, 1, purMultiple * 0.2, purMultiple * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                BasePPSlotSpinData spinData = convertBsonToSpinData(document);
                return spinData;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in TheBigDawgsGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            var floatingBetInfo = betInfo as TheBigDawgsBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, betInfo.MoreBet ? 1 : 0, betInfo.PurchaseFree ? floatingBetInfo.PurchaseType : -1, betMoney);
        }
    }
}
