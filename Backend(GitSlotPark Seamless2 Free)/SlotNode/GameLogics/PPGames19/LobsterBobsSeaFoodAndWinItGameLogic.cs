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
    class LobsterBobsSeaFoodAndWinItBetInfo : BasePPSlotBetInfo
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
    class LobsterBobsSeaFoodAndWinItGameLogic : BasePPSlotGame
    {
        protected double[] _multiTotalFreeSpinWinRates;
        protected double[] _multiMinFreeSpinWinRates;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20lobseafd";
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
                return 3;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=3,13,9,4,8,7,10,12,7,12,5,13,9,3,13&cfgs=11723&ver=3&def_sb=3,11,5,4,12&reel_set_size=8&def_sa=6,13,9,5,11&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1;2~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{ante:\"94.01\",purchase_1:\"94.02\",purchase_0:\"94.02\",regular:\"94.02\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"920217\",max_rnd_win:\"3000\",max_rnd_win_a:\"2000\",max_rnd_hr_a:\"1279099\"}}&wl_i=tbm~3000;tbm_a~2000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1,1&wilds=&bonuses=0&bls=20,30&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;2000,1000,400,0,0;1000,400,200,0,0;400,200,100,0,0;200,100,40,0,0;200,100,40,0,0;40,20,10,0,0;40,20,10,0,0;40,20,10,0,0;20,10,5,0,0;20,10,5,0,0;20,10,5,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=5,000,000.00&reel_set0=5,6,10,8,10,10,10,11,2,12,13,11,11,9,9,9,8,2,3,4,8,7,11,12,12,12,5,13,9,13,13,11,11,11,7,12,9,9,12,2,10,6~7,8,8,11,10,11,13,1,5,2,13,11,2,13,13,13,6,4,10,10,13,12,7,8,9,4,1,12,7,2,12,12,12,10,12,5,6,12,13,9,3,1,9,9,11,11,8~1,4,8,10,13,13,13,5,6,11,6,9,10,10,10,1,2,13,11,13,12,12,12,7,2,11,10,3,11,11,11,12,13,12,12,7,9,8~10,9,8,9,1,10,10,10,11,7,6,8,11,11,10,13,13,13,2,10,12,7,12,13,6,12,12,12,9,4,11,5,12,1,11,11,11,13,13,2,12,13,5,3,8~11,10,10,2,13,13,13,11,9,10,6,8,12,9,9,9,8,13,2,12,9,11,11,11,3,11,2,12,5,9,12,12,12,4,7,8,13,13,6&reel_set2=9,5,11,2,11,10,10,10,7,8,11,8,9,10,12,9,9,9,10,12,12,13,2,3,13,12,12,12,7,9,2,13,6,13,8,11,11,11,12,4,5,6,7,10~13,7,8,11,12,12,11,9,12,1,12,4,13,13,13,9,13,11,10,13,3,9,6,7,6,10,8,4,12,12,12,5,13,10,6,11,10,12,9,11,7,8,13,8,5~4,9,12,8,11,13,13,13,9,10,8,9,12,11,10,10,10,13,3,10,8,5,4,12,12,12,6,11,12,13,7,11,11,11,10,11,7,12,5,13,13,1~10,6,12,9,10,10,10,4,11,12,10,5,13,13,13,7,5,13,7,8,12,12,12,11,12,11,4,9,9,11,11,11,13,3,8,6,10,1,8~11,12,10,9,11,13,13,13,5,10,7,6,7,13,9,9,9,6,12,13,7,6,10,4,11,11,11,13,8,3,9,11,5,11,12,12,12,8,12,12,8,8,10,9,9&reel_set1=11,7,6,8,13,9,2,10,10,10,7,11,11,10,7,13,12,2,9,9,9,4,11,9,10,13,3,12,8,12,12,12,13,12,6,8,5,10,9,11,11,11,8,9,2,13,10,2,6,5,11,12~11,13,6,9,1,6,7,10,7,10,8,13,13,13,9,13,7,2,4,8,10,3,6,12,4,1,12,12,12,11,8,2,5,13,12,12,11,11,12,9,13,2~12,3,11,13,8,13,13,13,10,13,13,9,9,5,10,10,10,1,10,2,8,4,10,12,12,12,5,9,6,12,4,11,11,11,7,1,7,12,6,2,8,11~7,2,12,9,11,13,10,10,10,11,7,10,13,1,5,8,13,13,13,12,13,12,12,11,7,11,12,12,12,2,10,4,6,2,9,11,11,11,9,10,8,3,6,8,5,1,13~11,3,7,13,8,13,13,13,10,6,12,11,11,12,13,9,9,9,10,7,13,2,8,5,9,11,11,11,8,9,11,7,6,2,5,12,12,12,2,12,9,6,12,10,13,4&reel_set4=11,10,6,10,10,10,5,12,11,9,9,9,13,3,4,11,12,12,12,7,9,8,13,11,11,11,12,9,13,7,6,8~12,6,11,5,10,13,7,9,11,13,13,13,8,6,4,3,7,11,12,9,5,1,12,12,12,10,13,10,8,4,11,8,13,12,12,13,9~12,9,13,8,8,2,13,13,13,12,4,10,13,10,6,13,10,10,10,13,11,6,9,12,7,3,12,12,12,6,7,4,11,11,10,11,11,11,2,9,8,5,7,12~13,4,13,4,6,12,10,10,10,11,1,9,5,7,10,6,11,13,13,13,9,5,12,10,13,13,10,8,12,12,12,11,12,7,11,8,7,9,11,11,11,9,12,3,6,8,10,8,12,13~4,11,13,8,13,13,13,6,3,7,12,9,10,9,9,9,10,11,12,11,9,6,11,11,11,7,13,8,5,13,12,12,12,10,9,5,12,13,8&purInit=[{bet:1000,type:\"default\"},{bet:1000,type:\"default\"}]&reel_set3=10,13,6,5,12,7,10,10,10,13,8,11,12,5,9,11,9,9,9,3,6,11,8,9,11,8,13,12,12,12,6,9,12,7,10,13,7,11,11,11,4,9,10,8,12,13,11,10,12~12,9,8,3,6,10,9,13,4,11,5,11,13,13,13,12,7,8,9,12,11,8,2,7,13,8,6,4,11,12,12,12,7,13,12,10,2,11,10,2,6,5,10,9,13,13~11,13,10,10,4,11,13,13,13,9,9,12,10,12,1,8,10,10,10,6,8,12,3,5,7,11,12,12,12,8,6,13,7,13,10,13,11,11,11,7,5,4,6,12,9,8,11,9~9,3,10,8,8,9,10,10,10,9,11,8,11,12,7,5,11,13,13,13,12,10,13,6,13,12,12,11,12,12,12,13,4,6,7,4,13,8,10,11,11,11,5,10,11,13,7,12,6,9,1~6,13,10,9,4,7,13,13,13,5,12,9,7,3,12,9,9,9,8,10,12,10,7,11,8,6,11,11,11,6,11,13,12,9,8,12,12,12,11,5,8,13,9,13,11,10&reel_set6=6,10,6,9,12,10,10,10,4,11,7,3,12,8,9,9,9,11,10,7,10,11,13,12,12,12,11,7,13,9,8,11,11,11,13,12,12,13,5,6,9~8,4,8,9,10,4,9,8,5,7,1,6,13,13,13,5,6,11,13,11,7,10,10,13,6,11,13,12,12,12,7,12,11,10,13,13,3,8,9,12,11,12,12,9~13,13,5,8,6,4,13,13,13,5,8,11,12,6,13,12,13,10,10,10,7,7,1,4,8,9,10,11,12,12,12,9,13,12,11,12,3,8,9,11,11,11,6,10,7,9,12,10,10,11,11~12,1,6,8,10,10,10,5,4,8,5,12,13,13,13,3,11,9,7,9,12,12,12,4,13,13,12,11,10,11,11,11,8,9,11,10,6,7,13~2,12,9,6,9,13,13,13,8,11,4,5,12,5,9,9,9,2,13,11,10,12,7,3,11,11,11,12,6,9,2,7,11,6,12,12,12,10,8,10,11,8,7,13,13&reel_set5=13,7,9,11,10,5,10,10,10,8,13,9,7,12,12,13,9,9,9,8,9,3,13,9,6,8,12,12,12,10,11,12,12,8,11,10,11,11,11,10,5,6,11,6,11,4,13,7~9,3,12,4,10,11,10,7,11,13,13,13,8,5,13,9,13,9,5,6,8,7,12,12,12,1,10,4,13,11,12,12,6,8,12,11,13~1,9,12,8,13,13,13,11,13,9,8,4,11,3,4,10,10,10,6,7,5,12,12,13,10,12,12,12,7,10,7,12,13,11,10,11,11,11,5,8,12,9,6,9,11,8,6~13,5,8,12,11,2,10,10,10,8,6,9,12,13,13,11,13,13,13,11,2,11,10,6,5,12,12,12,7,12,8,2,6,10,4,11,11,11,9,7,10,3,9,4,7,13~8,10,6,5,6,13,13,13,11,9,6,10,13,5,9,9,9,8,7,11,9,13,10,11,11,11,13,12,13,12,3,4,12,12,12,8,7,11,12,7,11,9,12&reel_set7=8,11,12,9,2,7,8,10,10,10,7,8,13,8,12,5,13,3,9,9,9,7,6,2,11,13,11,13,4,12,12,12,6,11,12,10,10,5,13,11,11,11,2,12,10,2,9,10,11,6,9,9~13,5,6,7,9,11,6,13,13,13,3,8,12,2,4,9,2,10,12,12,12,10,11,13,1,12,12,13,8,7,11~6,12,2,13,2,8,13,13,13,11,13,9,10,8,7,4,10,10,10,9,8,12,6,1,11,10,12,12,12,11,13,9,4,10,11,12,11,11,11,5,2,5,13,3,7,12,1~12,11,9,12,10,8,5,10,10,10,4,8,13,2,13,11,5,12,6,13,13,13,4,10,13,9,12,2,6,11,12,12,12,2,10,7,11,6,3,13,9,13,11,11,11,7,8,1,10,12,8,7,1,9~2,8,13,5,10,13,13,13,11,8,6,12,3,10,9,9,9,4,13,2,13,9,9,11,11,11,9,7,6,12,11,12,12,12,13,8,7,12,10,2,5,12&total_bet_min=10.00";
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
	
	
        protected override double MoreBetMultiple
        {
            get { return 1.5; }
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
            get { return new double[] { 50, 50 }; }
        }
        #endregion
        public LobsterBobsSeaFoodAndWinItGameLogic()
        {
            _gameID = GAMEID.LobsterBobsSeaFoodAndWinIt;
            GameName = "LobsterBobsSeaFoodAndWinIt";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
	        dicParams["bl"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("rs_iw"))
            {
                string[] strParts = dicParams["rs_iw"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strParts.Length; i++)
                    strParts[i] = convertWinByBet(strParts[i], currentBet);
                dicParams["rs_iw"] = string.Join(",", strParts);
            }
            if (dicParams.ContainsKey("mo_tw"))
            {
                string[] strParts = dicParams["mo_tw"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strParts.Length; i++)
                    strParts[i] = convertWinByBet(strParts[i], currentBet);
                dicParams["mo_tw"] = string.Join(",", strParts);
            }
            if (dicParams.ContainsKey("rs_win"))
            {
                string[] strParts = dicParams["rs_win"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strParts.Length; i++)
                    strParts[i] = convertWinByBet(strParts[i], currentBet);
                dicParams["rs_win"] = string.Join(",", strParts);
            }

        }
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as LobsterBobsSeaFoodAndWinItBetInfo).PurchaseType;
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


                LobsterBobsSeaFoodAndWinItBetInfo betInfo = new LobsterBobsSeaFoodAndWinItBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();
		
                int bl = (int)message.Pop();
                if (bl == 0)
                    betInfo.MoreBet = false;
                else
                    betInfo.MoreBet = true;
		
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in LobsterBobsSeaFoodAndWinItGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }
		
                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in LobsterBobsSeaFoodAndWinItGameLogic::readBetInfoFromMessage", strGlobalUserID);
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
                    (oldBetInfo as LobsterBobsSeaFoodAndWinItBetInfo).PurchaseType = betInfo.PurchaseType;

                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in LobsterBobsSeaFoodAndWinItGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            LobsterBobsSeaFoodAndWinItBetInfo betInfo = new LobsterBobsSeaFoodAndWinItBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new LobsterBobsSeaFoodAndWinItBetInfo();
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
            int     purchaseType    = (betInfo as LobsterBobsSeaFoodAndWinItBetInfo).PurchaseType;
            double  payoutRate      = getPayoutRate(agentID, isAffiliate);

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
                int purchaseType = (betInfo as LobsterBobsSeaFoodAndWinItBetInfo).PurchaseType;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, -1, PurchaseMultiples[purchaseType] * 0.2, PurchaseMultiples[purchaseType] * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in LobsterBobsSeaFoodAndWinItGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int purchaseType = (betInfo as LobsterBobsSeaFoodAndWinItBetInfo).PurchaseType;
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in LobsterBobsSeaFoodAndWinItGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = (betInfo as LobsterBobsSeaFoodAndWinItBetInfo).PurchaseType.ToString();
        }
        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            LobsterBobsSeaFoodAndWinItBetInfo lobsterBetInfo = betInfo as LobsterBobsSeaFoodAndWinItBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, -1, betInfo.PurchaseFree ? lobsterBetInfo.PurchaseType : -1, betMoney);
        }
    }
}
