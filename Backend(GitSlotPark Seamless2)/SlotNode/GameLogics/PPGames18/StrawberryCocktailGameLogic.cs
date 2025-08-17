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
    class StrawberryCocktailBetInfo : BasePPSlotBetInfo
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
    class StrawberryCocktailGameLogic : BasePPSlotGame
    {
        protected double[] _multiTotalFreeSpinWinRates;
        protected double[] _multiMinFreeSpinWinRates;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10strawberry";
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
                return "def_s=5,9,4,3,7,4,8,5,4,8,5,9,4,5,7&cfgs=9623&ver=3&def_sb=3,7,9,5,7&reel_set_size=4&def_sa=5,8,7,4,7&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"25000000\",max_rnd_win:\"16000\"}}&wl_i=tbm~16000&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1,1,1&wilds=2~20000,2000,400,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;20000,2000,400,0,0;2000,200,60,0,0;600,100,40,0,0;300,80,20,0,0;200,60,15,0,0;150,40,12,0,0;100,25,10,0,0&total_bet_max=60,000,000.00&reel_set0=3,9,7,6,9,6,7,1,7,5,6,4,8,9,8,4,7,3,9,1,8,4,7,4,5,9,8,5,1,8,1,7,9,3,6,4,8,2,1,9,7,4,9,3,6,9,5,8,3,9,8,7,4,7,5,8,7,4,2,9,8,9,6,8,2,3,1,5,6,7,7~6,7,8,9,5,8,7,5,9,4,9,6,8,9,7,9,3,6,7,4,7,9,1,6,5,4,8,7,8,6,7,5,9,7,8,1,2,4,5,8,6,2,7,3,4,1,4,9,5,1,7,4~7,6,8,7,5,6,3,6,8,6,2,6,4,8,4,1,6,7,3,9,3,4,5,6,8,7,8,9,5,9,6,8,7,4,9,8,6,5,8,6,5,8,1,9,6,1,3,7,5,9,5,6,8,3,6,8,2,9,7,4,1,6,8,3,9,3,6,5,6,7,9,6,7,8~6,8,1,8,9,6,4,8,7,9,7,3,8,7,9,1,3,9,4,7,9,6,5,4,9,4,7,6,5,7,1,6,9,5,9,2,5,7,4,5,7,2,7,5,8,7,9,7,9,6~9,7,5,9,8,6,5,7,6,7,4,7,6,8,9,7,9,3,7,9,5,8,4,8,3,1,8,5,4,7,9,2,5,8,9,8,6,4,6,9,8,9,6,7,4,8,7,8,7,4,6,1,8,6,4,9,5,6,8,9,6,2,9,7,7,9&reel_set2=7,4,7,4,9,7,4,8,3,7,1,7,5,6,7,3,8,5,8,5,9,5,8,9,7,6,9,1,9,6,5,6,5,8,5,6,9,4,8,3,9,4,8,1,9,6,3,6,8,3,5~5,7,9,5,9,3,9,3,7,5,7,7,5,7,5,3,5,9,5,9,7,5,7,5,9,5,9,7,3,9,5,7,9,3,9,3,7~8,4,8,8,6,4,4,8,6,4,4,8,4,4,4,8,6,4,6,8,4,8,6,8,6,4,4,6~9,7,5,7,5,3,7,3,5,3,9,5,5,9,7,3,5,9,7,5,3,7,9,7,9,5,3,7,5,9~4,6,7,5,4,3,7,5,9,5,8,6,8,9,7,4,6,4,7,8,5,8,6,5,9,4,3,1,9,7,3,1,6&reel_set1=7,5,9,7,6,9,6,9,8,2,8,7,6,7,9,8,9,7,6,9,4,1,6,3,7,6,9,6,7,4,8,1,4,5,8,7,8,9,6,8,9,2,7,5,6,5,7,6,1,5,4,6,8,5,8,9,6,4~8,8,3,6,4,8,9,3,5,8,7,6,6,3,1,9,4,7,8,2,8,7,8,8,8,2,6,7,9,6,6,8,5,4,8,5,6,5,7,6,3,8,8,1,8,6,6,5,9,8,6~8,4,3,4,7,9,5,7,4,9,8,5,7,9,8,9,4,3,7,4,8,9,7,8,1,7,8,7,4,9,7,6,7,9,3,6,9,6,5,9,5,7,9,6,7,2,7,9,1,8,4,8,5,6,5,5,4~1,3,9,8,3,5,9,8,7,4,7,9,6,4,9,7,2,9,5,2,7,3,8,7,8,7,5,3,8,6,4,7,8,9,1,6,8,5,3,7,3,6,5,6,5,4,2,5,6,8,6,8,1,8,6,5,7,9,5,8,7,6~6,7,6,7,9,7,4,6,9,7,9,5,8,4,5,9,8,7,8,7,2,7,6,9,6,2,7,3,7,9,8,6,7,5,4,9,8,3,5,6,8,4,8,6,5,8,9,6,3,5,7,4,1,5,9,6,8,4,7,8,6,3,9,1,8,9,6,4,7,8&purInit=[{bet:1200,type:\"default\"},{bet:4000,type:\"default\"},{bet:12000,type:\"default\"}]&reel_set3=4,6,8~9,6,8,4,3,8,5,7,9,7,8,3,5,4,3,8,7,5,6,9,6,4,5,9,2~7,9,7,3,9,5,3,5,3,5,7,3~7,2,5,6,7,5,7,8,9,3,4,3,8,5,6,9,3,9,4,6,3,4,8,6,9,8,7,8,5,6,3,7,5,9~4,6,8&total_bet_min=10.00";
            }
        }
	
        protected override double PurchaseFreeMultiple
        {
            get { return 60; }
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
            get { return new double[] { 60, 200, 600 }; }
        }
        #endregion
        public StrawberryCocktailGameLogic()
        {
            _gameID = GAMEID.StrawberryCocktail;
            GameName = "StrawberryCocktail";
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
            if (dicParams.ContainsKey("pw"))
                dicParams["pw"] = convertWinByBet(dicParams["pw"], currentBet);

            if(dicParams.ContainsKey("g"))
            {
                var gParam = JToken.Parse(dicParams["g"]);
                if (gParam["bg"] != null && gParam["bg"]["rw"] != null)
                {
                    gParam["bg"]["rw"] = convertWinByBet(gParam["bg"]["rw"].ToString(), currentBet);
                    dicParams["g"] = serializeJsonSpecial(gParam);
                }
            }
            if (dicParams.ContainsKey("trail"))
            {
                string strTrail = dicParams["trail"];
                string[] strParts = strTrail.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strParts.Length; i++)
                {
                    string[] strSubParts = strParts[i].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                    if (strSubParts.Length == 2 && strSubParts[0] == "rw")
                    {
                        strSubParts[1] = convertWinByBet(strSubParts[1], currentBet);
                        strParts[i] = string.Join("~", strSubParts);
                    }
                }
                dicParams["trail"] = string.Join(";", strParts);

            }
        }
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as StrawberryCocktailBetInfo).PurchaseType;
            return this.PurchaseMultiples[purchaseType];
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                StrawberryCocktailBetInfo betInfo = new StrawberryCocktailBetInfo();
                betInfo.BetPerLine  = (float)message.Pop();
                betInfo.LineCount   = (int)message.Pop();
	
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in StrawberryCocktailGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                    (oldBetInfo as StrawberryCocktailBetInfo).PurchaseType = betInfo.PurchaseType;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in StrawberryCocktailGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            StrawberryCocktailBetInfo betInfo = new StrawberryCocktailBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
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
        protected override async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int agentID, BasePPSlotBetInfo betInfo, double baseBet, UserBonus userBonus)
        {
            int purchaseType = (betInfo as StrawberryCocktailBetInfo).PurchaseType;
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
                int purchaseType = (betInfo as StrawberryCocktailBetInfo).PurchaseType;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, -1, PurchaseMultiples[purchaseType] * 0.2, PurchaseMultiples[purchaseType] * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in StrawberryCocktailGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int purchaseType = (betInfo as StrawberryCocktailBetInfo).PurchaseType;
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in StrawberryCocktailGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = (betInfo as StrawberryCocktailBetInfo).PurchaseType.ToString();
        }
        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            StrawberryCocktailBetInfo strawBetInfo = betInfo as StrawberryCocktailBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, -1, betInfo.PurchaseFree ? strawBetInfo.PurchaseType : -1, betMoney);
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set", "s", "st", "sw" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]) || resultParams.ContainsKey(toCopyParams[i]))
                    continue;
                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }
            return resultParams;
        }
    }
}
