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
    class JohnHunterAndGalileosSecretsBetInfo : BasePPSlotBetInfo
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
    class JohnHunterAndGalileosSecretsGameLogic : BasePPSlotGame
    {
        protected double[] _multiTotalFreeSpinWinRates;
        protected double[] _multiMinFreeSpinWinRates;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10booklight";
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
                return "def_s=6,9,7,5,10,3,11,11,3,11,7,9,6,4,8&cfgs=1&ver=3&mo_s=13&mo_v=10,20,30,50,100,150,200,300,500,750,1000,2000,3000,4000,5000,50000,1,1,1&def_sb=5,10,8,6,9&reel_set_size=7&def_sa=6,12,12,7,12&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1;15~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{purchase_1:\"96.50\",purchase_0:\"96.50\",regular:\"96.50\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"2277904\",max_rnd_win:\"5000\",gamble_lvl_3:\"87.55\",gamble_lvl_6:\"66.28\",gamble_s_7:\"86.38\",gamble_lvl_7:\"57.25\",gamble_s_8:\"87.82\",gamble_lvl_4:\"74.27\",gamble_lvl_11:\"62.03\",gamble_s_9:\"75.73\",gamble_lvl_5:\"59.97\",gamble_lvl_12:\"59.19\",gamble_s_3:\"87.55\",gamble_lvl_10:\"64.52\",gamble_s_4:\"84.83\",gamble_lvl_8:\"50.28\",gamble_s_5:\"80.75\",gamble_lvl_9:\"66.63\",gamble_s_6:\"79.78\",gamble_s_11:\"96.14\",gamble_s_10:\"96.84\",gamble_s_12:\"95.42\"}}&wl_i=tbm~5000&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&purInit_e=1,1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;1000,150,40,10,0;750,125,30,5,0;500,100,20,3,0;250,75,15,0,0;200,60,10,0,0;150,50,8,0,0;100,25,5,0,0;100,25,5,0,0;75,20,5,0,0;75,20,5,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=20,000,000.00&reel_set0=8,4,10,11,4,10,9,6,12,5,4,9,11,3,9,13,4,12,8,13,13,13,7,12,11,6,5,6,14,9,10,11,13,14,13,9,3,9,11,10,7,14,14,14,12,14,13,7,13,3,14,11,9,14,14,8,10,12,10,13,11,10,13,11,13,7~11,1,3,7,6,7,14,14,12,1,10,14,9,12,9,14,3,14,5,10,14,14,6,13,12,14,8,11,12,4,14,14,14,8,1,12,11,1,5,14,12,14,5,8,10,9,14,4,12,10,11,9,6,14,10,14,5,9,14,7,14,13~3,12,14,11,1,13,14,10,6,14,14,11,14,14,9,8,13,8,10,14,14,1,10,9,3,12,14,14,14,9,11,14,14,7,8,14,14,8,11,13,4,1,9,5,14,10,11,6,4,11,14,12,14,14,5,7,14,12,6,10~14,9,6,9,11,13,7,5,6,10,12,6,8,11,1,11,9,7,9,11,10,11,12,5,10,11,8,1,12,10,12,8,6,15,11,7,5,11,6,1,12,1,8,11,14,14,14,7,1,14,9,7,12,13,12,6,8,5,12,8,13,6,4,12,3,9,10,9,12,4,1,4,3,7,1,10,13,1,4,9,14,10,9,7,9,12,3,1,7~8,7,9,10,11,14,6,13,11,10,4,11,10,9,14,5,3,14,9,10,14,7,11,6,14,12,8,14,6,13,13,13,7,11,13,9,5,11,10,14,9,10,7,5,13,13,3,7,9,13,9,11,12,11,8,6,11,9,6,7,4,14,13,14,14,14,8,6,12,4,12,13,8,14,10,3,13,12,9,14,4,8,6,11,9,3,11,5,13,10,9,4,5,14,12,11,13,10&accInit=[{id:0,mask:\"sn\"}]&reel_set2=8,4,10,11,4,10,9,6,12,5,4,9,11,3,9,13,4,12,8,13,13,13,7,12,11,6,5,6,14,9,10,11,13,14,13,9,3,9,11,10,7,14,14,14,12,14,13,7,13,3,14,11,9,14,14,8,10,12,10,13,11,10,13,11,13,7~11,1,3,7,6,7,14,14,12,1,10,14,9,12,9,14,3,14,5,10,14,14,6,13,12,14,8,11,12,4,14,14,14,8,1,12,11,1,5,14,12,14,5,8,10,9,14,4,12,10,11,9,6,14,10,14,5,9,14,7,14,13~3,12,14,11,1,13,14,10,6,14,14,11,14,14,9,8,13,8,10,14,14,1,10,9,3,12,14,14,14,9,11,14,14,7,8,14,14,8,11,13,4,1,9,5,14,10,11,6,4,11,14,12,14,14,5,7,14,12,6,10~14,9,6,9,11,13,7,5,6,10,12,6,8,11,1,11,9,7,9,11,10,11,12,5,10,11,8,1,12,10,12,8,6,11,7,5,11,6,1,12,1,8,11,14,14,14,7,1,14,9,7,12,13,12,6,8,5,12,8,13,6,4,12,3,9,10,9,12,4,1,4,3,7,1,10,13,1,4,9,14,10,9,7,9,12,3,1,7~8,7,9,10,11,14,6,13,11,10,4,11,10,9,14,5,3,14,9,10,14,7,11,6,14,12,8,14,6,13,13,13,7,11,13,9,5,11,10,14,9,10,7,5,13,13,3,7,9,13,9,11,12,11,8,6,11,9,6,7,4,14,13,14,14,14,8,6,12,4,12,13,8,14,10,3,13,12,9,14,4,8,6,11,9,3,11,5,13,10,9,4,5,14,12,11,13,10&reel_set1=8,4,10,11,4,10,9,6,12,5,4,9,11,3,9,13,4,12,8,13,13,13,7,12,11,6,5,6,14,9,10,11,13,14,13,9,3,9,11,10,7,14,14,14,12,14,13,7,13,3,14,11,9,14,14,8,10,12,10,13,11,10,13,11,13,7~11,1,3,7,6,7,14,14,12,1,10,14,9,12,9,14,3,14,5,10,14,14,6,13,12,14,8,11,12,4,14,14,14,8,1,12,11,1,5,14,12,14,5,8,10,9,14,4,12,10,11,9,6,14,10,14,5,9,14,7,14,13~3,12,14,11,1,13,14,10,6,14,14,11,14,14,9,8,13,8,10,14,14,1,10,9,3,12,14,14,14,9,11,14,14,7,8,14,14,8,11,13,4,1,9,5,14,10,11,6,4,11,14,12,14,14,5,7,14,12,6,10~14,9,6,9,11,13,7,5,6,10,12,6,8,11,1,11,9,7,9,11,10,11,12,5,10,11,8,1,12,10,12,8,6,11,7,5,11,6,1,12,1,8,11,14,14,14,7,1,14,9,7,12,13,12,6,8,5,12,8,13,6,4,12,3,9,10,9,12,4,1,4,3,7,1,10,13,1,4,9,14,10,9,7,9,12,3,1,7~8,7,9,10,11,14,6,13,11,10,4,11,10,9,14,5,3,14,9,10,14,7,11,6,14,12,8,14,6,13,13,13,7,11,13,9,5,11,10,14,9,10,7,5,13,13,3,7,9,13,9,11,12,11,8,6,11,9,6,7,4,14,13,14,14,14,8,6,12,4,12,13,8,14,10,3,13,12,9,14,4,8,6,11,9,3,11,5,13,10,9,4,5,14,12,11,13,10&reel_set4=8,4,10,11,4,10,9,6,12,5,4,9,11,3,9,13,4,12,8,13,13,13,7,12,11,6,5,6,14,9,10,11,13,14,13,9,3,9,11,10,7,14,14,14,12,14,13,7,13,3,14,11,9,14,14,8,10,12,10,13,11,10,13,11,13,7~11,1,3,7,6,7,14,14,12,1,10,14,9,12,9,14,3,14,5,10,14,14,6,13,12,14,8,11,12,4,14,14,14,8,1,12,11,1,5,14,12,14,5,8,10,9,14,4,12,10,11,9,6,14,10,14,5,9,14,7,14,13~3,12,14,11,1,13,14,10,6,14,14,11,14,14,9,8,13,8,10,14,14,1,10,9,3,12,14,14,14,9,11,14,14,7,8,14,14,8,11,13,4,1,9,5,14,10,11,6,4,11,14,12,14,14,5,7,14,12,6,10~14,9,6,9,11,13,7,5,6,10,12,6,8,11,1,11,9,7,9,11,10,11,12,5,10,11,8,1,12,10,12,8,6,11,7,5,11,6,1,12,1,8,11,14,14,14,7,1,14,9,7,12,13,12,6,8,5,12,8,13,6,4,12,3,9,10,9,12,4,1,4,3,7,1,10,13,1,4,9,14,10,9,7,9,12,3,1,7~8,7,9,10,11,14,6,13,11,10,4,11,10,9,14,5,3,14,9,10,14,7,11,6,14,12,8,14,6,13,13,13,7,11,13,9,5,11,10,14,9,10,7,5,13,13,3,7,9,13,9,11,12,11,8,6,11,9,6,7,4,14,13,14,14,14,8,6,12,4,12,13,8,14,10,3,13,12,9,14,4,8,6,11,9,3,11,5,13,10,9,4,5,14,12,11,13,10&purInit=[{bet:800,type:\"default\"},{bet:2000,type:\"default\"}]&reel_set3=8,4,10,11,4,10,9,6,12,5,4,9,11,3,9,13,4,12,8,13,13,13,7,12,11,6,5,6,14,9,10,11,13,14,13,9,3,9,11,10,7,14,14,14,12,14,13,7,13,3,14,11,9,14,14,8,10,12,10,13,11,10,13,11,13,7~11,1,3,7,6,7,14,14,12,1,10,14,9,12,9,14,3,14,5,10,14,14,6,13,12,14,8,11,12,4,14,14,14,8,1,12,11,1,5,14,12,14,5,8,10,9,14,4,12,10,11,9,6,14,10,14,5,9,14,7,14,13~3,12,14,11,1,13,14,10,6,14,14,11,14,14,9,8,13,8,10,14,14,1,10,9,3,12,14,14,14,9,11,14,14,7,8,14,14,8,11,13,4,1,9,5,14,10,11,6,4,11,14,12,14,14,5,7,14,12,6,10~14,9,6,9,11,13,7,5,6,10,12,6,8,11,1,11,9,7,9,11,10,11,12,5,10,11,8,1,12,10,12,8,6,11,7,5,11,6,1,12,1,8,11,14,14,14,7,1,14,9,7,12,13,12,6,8,5,12,8,13,6,4,12,3,9,10,9,12,4,1,4,3,7,1,10,13,1,4,9,14,10,9,7,9,12,3,1,7~8,7,9,10,11,14,6,13,11,10,4,11,10,9,14,5,3,14,9,10,14,7,11,6,14,12,8,14,6,13,13,13,7,11,13,9,5,11,10,14,9,10,7,5,13,13,3,7,9,13,9,11,12,11,8,6,11,9,6,7,4,14,13,14,14,14,8,6,12,4,12,13,8,14,10,3,13,12,9,14,4,8,6,11,9,3,11,5,13,10,9,4,5,14,12,11,13,10&reel_set6=6,9,10,9,7,5,6,4,8,3,10,4,9,4,10,8,10,5,3,10,9,7,10,9~12,1,11,1,12,10,11,12,11,10,1,10,11,1,12,10,1~12,9,12,1,12,1,11,9,1,9,11,9,1,11~10,9,11,15,12,9,11,15,12,11,9,15,9,10,11~9,5,7,3,9,4,10,9,10,6,8,10,9&reel_set5=9,10,4,10,3,7,5,9,10,9,10,6,9,8,6,9,4,9,7,8,9,10,3,10,5,10~11,1,12,10,1,12,10,11,12,1,11,1,10~9,1,9,12,1,9,11,1,11,1,11,12,1,12~1,11,10,1,12,1,11,1,11,15,9,1,10,11,1,9,1,11,10,9,1,10,1,10,11,9,10,9,12,11,1,12,1~9,7,5,9,10,9,4,10,8,6,4,9,10,3&total_bet_min=200.00";
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
        protected int FreePurCount
        {
            get { return 2; }
        }
        public double[] PurchaseMultiples
        {
            get { return new double[] { 80, 200 }; }
        }
        #endregion

        public JohnHunterAndGalileosSecretsGameLogic()
        {
            _gameID     = GAMEID.JohnHunterAndGalileosSecrets;
            GameName    = "JohnHunterAndGalileosSecrets";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"]   = "0";
	        dicParams["st"]         = "rect";
	        dicParams["sw"]         = "5";
        }
        protected override void supplementInitResult(Dictionary<string, string> dicParams, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {
            base.supplementInitResult(dicParams, betInfo, spinResult);
            if (!dicParams.ContainsKey("reel_set"))
                dicParams["reel_set"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                JohnHunterAndGalileosSecretsBetInfo betInfo = new JohnHunterAndGalileosSecretsBetInfo();
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in JohnHunterAndGalileosSecretsGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                    (oldBetInfo as JohnHunterAndGalileosSecretsBetInfo).PurchaseType = betInfo.PurchaseType;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in JohnHunterAndGalileosSecretsGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            JohnHunterAndGalileosSecretsBetInfo betInfo = new JohnHunterAndGalileosSecretsBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new JohnHunterAndGalileosSecretsBetInfo();
        }
        protected override double getPurchaseMultiple(BasePPSlotBetInfo betInfo)
        {
            int purchaseType = (betInfo as JohnHunterAndGalileosSecretsBetInfo).PurchaseType;
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
                _multiMinFreeSpinWinRates = new double[FreePurCount];

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
            int purchaseType = (betInfo as JohnHunterAndGalileosSecretsBetInfo).PurchaseType;
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
                int purchaseType = (betInfo as JohnHunterAndGalileosSecretsBetInfo).PurchaseType;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, -1, PurchaseMultiples[purchaseType] * 0.2, PurchaseMultiples[purchaseType] * 0.5, purchaseType), TimeSpan.FromSeconds(10.0));
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in JohnHunterAndGalileosSecretsGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                int purchaseType = (betInfo as JohnHunterAndGalileosSecretsBetInfo).PurchaseType;
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.MULTISPECIFIC, purchaseType),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in JohnHunterAndGalileosSecretsGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = (betInfo as JohnHunterAndGalileosSecretsBetInfo).PurchaseType.ToString();
        }
        protected override UserSpinItem createUserSpinItem(int agentID, string strUserID, BasePPSlotBetInfo betInfo, double betMoney)
        {
            JohnHunterAndGalileosSecretsBetInfo starBetInfo = betInfo as JohnHunterAndGalileosSecretsBetInfo;
            return new UserSpinItem(agentID, strUserID, this.SymbolName, betInfo.BetPerLine, betInfo.LineCount, -1, betInfo.PurchaseFree ? starBetInfo.PurchaseType : -1, betMoney);
        }

    }
}
