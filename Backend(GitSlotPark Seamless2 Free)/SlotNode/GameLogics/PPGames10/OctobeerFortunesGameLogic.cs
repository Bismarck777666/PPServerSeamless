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
    public class OctobeerFortunesBetInfo : BasePPSlotBetInfo
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
    class OctobeerFortunesGameLogic : BasePPSlotGame
    {
        protected double[]  _multiTotalFreeSpinWinRates;
        protected double[] _multiMinFreeSpinWinRates;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20octobeer";
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
                return "def_s=7,5,4,3,6,7,9,4,10,8,6,6,4,9,8&cfgs=6200&accm=cp&ver=2&acci=0&def_sb=3,8,4,7,10&reel_set_size=7&def_sa=7,4,6,10,10&accv=0&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"5907995\",max_rnd_win:\"4500\"}}&wl_i=tbm~4500&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&purInit_e=1,1,1,1,1,1,1,1,1,1,1,1&wilds=2~300,100,40,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;300,100,40,0,0;300,100,40,0,0;150,50,25,0,0;120,40,20,0,0;90,30,15,0,0;60,20,10,0,0;36,12,6,0,0;36,12,6,0,0;30,10,5,0,0;30,10,5,0,0;0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=12,12,12,4,11,7,8,5,11,4,8,5,10,7,11,6,5,3,6,2,2,2,7,11,9~2,2,2,8,5,9,4,8,10,6,9,5,6,8,4,5,9,7,4,9,6,10,9,8,6,10,5,12,12,12,9,3,7,10,8,3,10,9,11~5,6,9,8,2,2,2,10,4,7,10,3,11,8,7,10,11,7,8,3,7,12,12,12~12,12,12,3,6,9,5,11,10,7,8,6,11,4,9,8,6,4,8,7,9,11,7,6,8,10,5,9,6,8,11,7,9,3,5,8,7,11,10,9,4,6,9,7,4,8,3,9,7,11,8,10,5,9,4,7,8,5,2,2,2~12,12,12,12,11,6,10,7,6,10,4,9,7,8,10,6,4,5,11,10,7,4,10,11,6,8,5,6,2,2,2,8,3&accInit=[{id:0,mask:\"cp;mp\"}]&reel_set2=2,2,2,2,11,10,5,9,8,7,10,5,7,8,11,4,5,11,10,8,5,9,6,7,4,9,5,7,10,6,11,5,4,8,10,3,7,11,12,12,12~12,12,12,5,8,3,4,7,9,8,5,9,4,6,3,7,8,6,5,7,9,6,8,4,5,11,6,7,2,2,2,2,4,8,5,9,6,10~2,2,2,2,6,9,3,11,7,6,10,7,11,8,6,11,5,10,4,11,5,10,6,11,12,12,12~2,2,2,2,4,3,5,6,4,9,8,4,3,8,6,10,4,6,9,11,6,9,4,3,8,7,6,5,9,6,7,3,8,4,9,6,7,9,4,11,8,3,5,9,8,5,6,9,4,8,5,3,9,6,11,5,6,8,4,9,8,5,3,4,10,5,9,8,6,4,5,11,10,12,12,12~12,12,12,5,4,7,11,4,5,7,8,5,9,6,7,10,11,7,10,4,9,11,7,3,10,4,5,11,10,7,4,11,8,6,10,5,6,8,3,4,2,2,2,2&reel_set1=2,2,2,11,7,9,11,6,5,11,9,8,7,11,5,8,3,5,11,9,5,6,7,9,5,7,9,6,3,12,12,12,6,9,7,11,9,6,11,3,6,11,9,7,11,6,9,11,6,5,4,6,9,3,10~2,2,2,10,9,5,8,7,6,9,10,3,7,4,11,5,9,8,7,9,6,8,5,6,10,5,8,10,9,6,4,9,6,10,5,6,12,12,12~12,12,12,10,7,3,10,7,8,9,3,11,8,7,4,11,7,10,8,4,7,10,3,11,5,8,10,6,9,8,10,2,2,2~12,12,12,3,6,9,5,11,10,7,8,6,11,4,9,8,6,4,8,7,9,11,7,6,8,10,5,9,2,2,2~2,2,2,7,11,10,7,3,11,5,3,10,9,7,5,9,8,7,4,12,12,12,9,7,4,8,10,4,5,11,10,7,4,10,11,6&reel_set4=2,2,2,2,10,5,7,8,11,4,5,11,10,8,5,9,6,7,4,9,5,7,10,6,11,5,4,8,10,3,7,11,12,12,12~12,12,12,6,9,4,6,7,8,5,4,3,8,4,9,3,11,6,5,8,3,4,7,9,8,5,9,4,6,3,7,8,6,5,7,9,6,8,4,5,11,6,7,4,8,5,9,6,10,4,9,8,6,11,5,6,3,5,10,2,2,2,2~12,12,12,9,10,7,11,6,8,9,10,8,6,10,4,5,11,7,6,11,7,6,11,4,5,7,6,9,7,6,8,10,6,7,10,6,11,9,6,2,2,2,2,7,6,9,8,6,10,3~2,2,2,2,6,5,9,6,7,3,8,4,9,6,7,9,4,11,8,3,5,9,8,5,6,9,12,12,12,4,8,5,3,9,6,11,5,6,8,4,9,8,5,3,4,10~12,12,12,11,4,5,11,10,3,7,5,3,10,9,7,10,4,5,8,7,10,4,3,7,11,10,7,4,10,11,6,5,8,3,10,4,11,7,3,5,10,3,11,4,9,7,4,11,5,10,7,11,3,7,4,5,11,9,5,4,7,11,4,5,7,8,5,2,2,2,2&purInit=[{type:\"fs\",bet:2000,fs_count:9},{type:\"fs\",bet:2200,fs_count:9},{type:\"fs\",bet:2400,fs_count:9},{type:\"fs\",bet:2600,fs_count:9},{type:\"fs\",bet:2800,fs_count:9},{type:\"fs\",bet:3000,fs_count:9},{type:\"fs\",bet:3200,fs_count:9},{type:\"fs\",bet:3400,fs_count:9},{type:\"fs\",bet:3600,fs_count:9},{type:\"fs\",bet:3800,fs_count:9},{type:\"fs\",bet:4000,fs_count:9},{type:\"r\",bet:3000}]&reel_set3=2,2,2,2,4,8,10,3,7,6,11,9,4,8,11,10,3,11,9,7,11,8,5,9,7,5,4,9,3,6,10,7,4,10,7,11,8,3,9,7,4,11,10,5,9,8,7,10,5,7,8,11,4,5,11,10,8,5,9,7,4,9,5,7,10,6,11,5,4,8,10,3,7,11,12,12,12,12~11,6,10,5,8,3,4,5,9,4,6,3,9,6,8,9,4,3,2,2,2,2,9,6,5,8,3,4,7,9,12,12,12,12~12,12,12,12,7,10,11,6,9,8,6,7,10,6,5,11,6,10,7,8,11,3,10,7,11,10,6,11,10,7,6,11,10,7,9,2,2,2,2,3,7,11,6,7,5,8,7,6,9,7,6,8,4~2,2,2,2,6,3,5,6,8,4,3,5,6,4,9,8,4,3,8,6,10,4,7,6,9,11,6,9,4,3,8,6,5,12,12,12,12~8,7,10,4,2,2,2,2,7,11,6,3,10,4,11,7,3,5,10,3,11,4,9,7,12,12,12,12&reel_set6=11,5,8,3,7,8,3,10~6,9,10,5,4,9,8~9,3,7,6,11,3,4~5,9,3,4,8,6,10,7,6,9,11~10,5,6,8,11,7,4,5,3,11,5,6,8,4,9&reel_set5=9,7,10,8,5,2,2,2,2,4,6,10,7,12,12,12,12,11,5,10,4,6,3~7,12,12,12,12,4,8,5,9,6,10,4,12,12,12,12,9,8,6,11,12,12,12,12,5,6,3,5,10,2,2,2,2~4,5,7,6,2,2,2,2,9,7,6,8,12,12,12,12,10,6,7,10,2,2,2,2,6,11,9,6,12,12,12,12,7,6,9,8,6,12,12,12,12,10,3~11,2,2,2,2,10,3,8,6,12,12,12,12,4,7,6,5,9~8,3,4,12,12,12,12,7,6,9,5,2,2,2,2,3,10,6,3,11&total_bet_min=10.00";
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
            get { return 12; }
        }
        public double[] PurchaseMultiples
        {
            get { return new double[] { 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 150 }; }
        }

        #endregion
        public OctobeerFortunesGameLogic()
        {
            _gameID = GAMEID.OctobeerFortunes;
            GameName = "OctobeerFortunes";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("apwa"))
                dicParams["apwa"] = convertWinByBet(dicParams["apwa"], currentBet);
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


                var betInfo         = new OctobeerFortunesBetInfo();
                betInfo.BetPerLine  = (float)message.Pop();
                betInfo.LineCount   = (int)message.Pop();

                if (message.DataNum >= 3)
                {
                    betInfo.PurchaseFree = true;
                    betInfo.PurchaseType = (int)message.GetData(2);
                }
                else
                {
                    betInfo.PurchaseFree = false;
                    betInfo.PurchaseType = 0;
                }		
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in OctobeerFortunesGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                    (oldBetInfo as OctobeerFortunesBetInfo).PurchaseType = betInfo.PurchaseType;

                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in OctobeerFortunesGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            OctobeerFortunesBetInfo betInfo = new OctobeerFortunesBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new OctobeerFortunesBetInfo();
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
            int     purchaseType    = (betInfo as OctobeerFortunesBetInfo).PurchaseType;
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
                int purchaseType = (betInfo as OctobeerFortunesBetInfo).PurchaseType;
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
                int purchaseType = (betInfo as OctobeerFortunesBetInfo).PurchaseType;
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

        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            return this.PurchaseMultiples[(betInfo as OctobeerFortunesBetInfo).PurchaseType];
        }
        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = (betInfo as OctobeerFortunesBetInfo).PurchaseType.ToString();
        }
        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            OctobeerFortunesBetInfo octobeerBetInfo = betInfo as OctobeerFortunesBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, -1, betInfo.PurchaseFree ? octobeerBetInfo.PurchaseType : -1, betMoney);
        }
    }
}
