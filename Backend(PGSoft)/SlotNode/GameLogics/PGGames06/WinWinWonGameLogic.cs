using Akka.Actor;
using SlotGamesNode.Database;
using GITProtocol;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SlotGamesNode.GameLogics
{
    class WinWinWonBetInfo : BasePGSlotBetInfo
    {
        public int BetType { get; set; }
        public WinWinWonBetInfo(int baseBet) : base(baseBet)
        {
        }
        public override float TotalBet
        {
            get
            {
                return BetSize * BetLevel * BaseBet * (this.BetType + 1);
            }
        }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            this.BetType = reader.ReadInt32();
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(this.BetType);
        }
    }
    class WinWinWonGameLogic : BasePGSlotGame
    {
        private double[]    _spinDataDefaultBets    = new double[3];
        private int[]       _normalMaxIDs           = new int[3];
        private int[]       _emptySpinCounts        = new int[3];
        private int[]       _naturalSpinCounts      = new int[3];
        private double[]    _naturalRTPs            = new double[3];
        private int[]       _startIds               = new int[3];

        protected override bool SupportPurchaseFree
        {
            get { return false; }
        }
        protected override double DefaultBetSize
        {
            get { return 5.0; }
        }
        protected override int DefaultBetLevel
        {
            get { return 1; }
        }
        protected override int BaseBet
        {
            get
            {
                return 1;
            }
        }
        protected override string DefaultResult
        {
            get
            {
                return "{\"si\":{\"wp\":null,\"lw\":null,\"mv\":null,\"mp\":null,\"bn\":1,\"pp\":null,\"pprl\":null,\"frl\":[4,5,6,1,2,3,6,5,4],\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":1,\"cs\":0.25,\"rl\":[1,2,3],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":22.13,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"lw\":null,\"mv\":null,\"mp\":null,\"bn\":1,\"pp\":null,\"pprl\":null,\"frl\":[4,5,6,1,2,3,6,5,4],\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":1,\"cs\":0.25,\"rl\":[1,2,3],\"sid\":\"1762870740425649157\",\"psid\":\"1762870740425649157\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":94.14,\"max\":94.14}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public WinWinWonGameLogic()
        {
            _gameID = GAMEID.WinWinWon;
            GameName = "WinWinWon";
        }
        protected override void initGameData()
        {
            base.initGameData();
            _pgGameConfig.ml.AddRange(new int[] { 1 });
            _pgGameConfig.cs.AddRange(new double[] { 0.25, 1.5, 5.0, 15.0});
        }
        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    _spinDataDefaultBets[i] = (double)  (infoDocument["defaultbets"] as BsonArray)[i];
                    _normalMaxIDs[i]        = (int)     (infoDocument["normalmaxids"] as BsonArray)[i];
                    _emptySpinCounts[i]     = (int)     (infoDocument["emptycounts"] as BsonArray)[i];
                    _naturalSpinCounts[i]   = (int)     (infoDocument["normalselectcounts"] as BsonArray)[i];
                    _naturalRTPs[i]         = (double)  (infoDocument["normalrtps"] as BsonArray)[i];
                    _startIds[i]            = (int)     (infoDocument["startids"] as BsonArray)[i];
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
            }
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                WinWinWonBetInfo betInfo    = new WinWinWonBetInfo(this.BaseBet);
                betInfo.BetSize             = (float)Math.Round((double)message.Pop(), 2);
                betInfo.BetLevel            = (int)message.Pop();
                betInfo.PurchaseFree        = (int)message.Pop() == 2;
                betInfo.BetType             = (int)message.GetData(4) - 1;

                if (betInfo.BetSize <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetSize <= 0 in WinWinWonGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetSize);
                    return;
                }
                BasePGSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strGlobalUserID, out oldBetInfo))
                {
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetSize      = betInfo.BetSize;
                    oldBetInfo.BetLevel     = betInfo.BetLevel;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                    (oldBetInfo as WinWinWonBetInfo).BetType = betInfo.BetType;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in WinWinWonGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePGSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            WinWinWonBetInfo betInfo = new WinWinWonBetInfo(this.BaseBet);
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override OddAndIDData selectRandomOddAndID(BasePGSlotBetInfo baseBetInfo, int websiteID, bool isAffiliate)
        {
            var betInfo = baseBetInfo as WinWinWonBetInfo;
            int betType = betInfo.BetType;

            if (isAffiliate)
            {
                int selectedID = Pcg.Default.Next(_startIds[betType], _startIds[betType] + _naturalSpinCounts[betType]);
                OddAndIDData oddIDAndOdd = new OddAndIDData();
                oddIDAndOdd.ID = selectedID;
                return oddIDAndOdd;
            }
            else
            {
                double payoutRate = getPayoutRate(websiteID);
                double randomDouble = Pcg.Default.NextDouble(0.0, 100.0);
                int selectedID = 0;
                if (randomDouble >= payoutRate || payoutRate == 0.0)
                {
                    selectedID = Pcg.Default.Next(_startIds[betType], _startIds[betType] + _emptySpinCounts[betType]);
                }
                else
                {
                    double magicValue = 100.0 / _naturalRTPs[betType];
                    if (_naturalRTPs[betType] <= 1.0 || Pcg.Default.NextDouble(0, 100.0) < magicValue)
                        selectedID = Pcg.Default.Next(_startIds[betType], _startIds[betType] + _naturalSpinCounts[betType]);
                    else
                        selectedID = Pcg.Default.Next(_startIds[betType], _startIds[betType] + _emptySpinCounts[betType]);
                }
                OddAndIDData oddIDAndOdd = new OddAndIDData();
                oddIDAndOdd.ID = selectedID;
                return oddIDAndOdd;
            }
            
        }
        protected override async Task<BasePGSlotSpinData> selectEmptySpin(BasePGSlotBetInfo baseBetInfo)
        {
            var betInfo = baseBetInfo as WinWinWonBetInfo;
            int betType = betInfo.BetType;
            int id = Pcg.Default.Next(_startIds[betType], _startIds[betType] + _emptySpinCounts[betType]);
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, id), TimeSpan.FromSeconds(10.0));
            return convertBsonToSpinData(spinDataDocument);
        }
        protected double convertWinByBet(double win, float currentBet, int betType)
        {
            win = win / _spinDataDefaultBets[betType] * currentBet;
            return Math.Round(win, 2);
        }
        protected override void convertBetsByBet(dynamic jsonParams, double betSize, int betLevel, float totalBet)
        {
            int betType = (int)jsonParams["bn"] - 1;
            if (!IsNullOrEmpty(jsonParams["cs"]))
                jsonParams["cs"] = Math.Round(betSize, 2);

            if (!IsNullOrEmpty(jsonParams["ml"]))
                jsonParams["ml"] = betLevel;

            if (!IsNullOrEmpty(jsonParams["tb"]))
                jsonParams["tb"] = convertWinByBet((double)jsonParams["tb"], totalBet, betType);

            if (!IsNullOrEmpty(jsonParams["tbb"]))
                jsonParams["tbb"] = convertWinByBet((double)jsonParams["tbb"], totalBet, betType);
        }
        protected override void convertWinsByBet(dynamic jsonParams, float currentBet)
        {
            int betType = (int)jsonParams["bn"] - 1;
            if (!IsNullOrEmpty(jsonParams["aw"]))
                jsonParams["aw"] = convertWinByBet((double)jsonParams["aw"], currentBet, betType);

            if (!IsNullOrEmpty(jsonParams["ssaw"]))
                jsonParams["ssaw"] = convertWinByBet((double)jsonParams["ssaw"], currentBet, betType);

            if (!IsNullOrEmpty(jsonParams["tw"]))
                jsonParams["tw"] = convertWinByBet((double)jsonParams["tw"], currentBet, betType);

            if (!IsNullOrEmpty(jsonParams["fs"]) && IsArrayOrObject(jsonParams["fs"]) && !IsNullOrEmpty(jsonParams["fs"]["aw"]))
                jsonParams["fs"]["aw"] = convertWinByBet((double)jsonParams["fs"]["aw"], currentBet, betType);

            if (!IsNullOrEmpty(jsonParams["np"]))
                jsonParams["np"] = convertWinByBet((double)jsonParams["np"], currentBet, betType);

            if (!IsNullOrEmpty(jsonParams["lw"]))
            {
                string strLw = jsonParams["lw"].ToString();
                Dictionary<int, double> lineWins = JsonConvert.DeserializeObject<Dictionary<int, double>>(strLw);
                Dictionary<int, double> convertedLineWins = new Dictionary<int, double>();
                foreach (KeyValuePair<int, double> pair in lineWins)
                {
                    convertedLineWins[pair.Key] = convertWinByBet(pair.Value, currentBet, betType);
                }
                jsonParams["lw"] = JObject.FromObject(convertedLineWins);
            }
        }
    }
}
