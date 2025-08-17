using Akka.Actor;
using SlotGamesNode.Database;
using GITProtocol;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SlotGamesNode.HTTPService;
using Akka.Routing;
using GITProtocol.Utils;

namespace SlotGamesNode.GameLogics
{
    class Medusa2BetInfo : BasePGSlotBetInfo
    {
        public Dictionary<double, List<BasePGSlotSpinData>> SpinDataPerBet      { get; set; }
        public Dictionary<double, BasePGSlotSpinResult>     LastResultsPerBet   { get; set; }
        public Medusa2BetInfo(int baseBet) : base(baseBet) 
        {
            this.SpinDataPerBet    = new Dictionary<double, List<BasePGSlotSpinData>>();
            this.LastResultsPerBet = new Dictionary<double, BasePGSlotSpinResult>();
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(SpinDataPerBet.Count);
            foreach(KeyValuePair<double, List<BasePGSlotSpinData>> pair in SpinDataPerBet)
            {
                writer.Write(pair.Key);
                writer.Write(pair.Value.Count);
                for (int i = 0; i < pair.Value.Count; i++)
                    pair.Value[i].SerializeTo(writer);
            }
            writer.Write(LastResultsPerBet.Count);
            foreach(KeyValuePair<double, BasePGSlotSpinResult> pair in LastResultsPerBet)
            {
                writer.Write(pair.Key);
                pair.Value.SerializeTo(writer);
            }
        }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            SpinDataPerBet = new Dictionary<double, List<BasePGSlotSpinData>>();
            int count      = reader.ReadInt32();
            for(int i = 0; i < count; i++)
            {
                double key = reader.ReadDouble();
                int count2 = reader.ReadInt32();
                List<BasePGSlotSpinData> spinDataList = new List<BasePGSlotSpinData>();
                for(int j = 0; j < count2; j++)
                {
                    var spinData = new BasePGSlotSpinData();
                    spinData.SerializeFrom(reader);
                    spinDataList.Add(spinData);
                }
                SpinDataPerBet[key] = spinDataList;
            }
            LastResultsPerBet = new Dictionary<double, BasePGSlotSpinResult>();
            count             = reader.ReadInt32();
            for(int i = 0; i < count; i++)
            {
                double key = reader.ReadDouble();
                BasePGSlotSpinResult lastResult = new BasePGSlotSpinResult();
                lastResult.SerializeFrom(reader);
                LastResultsPerBet[key] = lastResult;
            }
        }
    }
    public class GroupedSpinData
    {
        public List<BasePGSlotSpinData> SpinDatas   { get; private set; }
        public double                   GroupOdd    { get; private set; }
        public GroupedSpinData(List<BasePGSlotSpinData> spinDatas, double groupOdd)
        {
            SpinDatas   = spinDatas;
            GroupOdd    = groupOdd;
        }
    }

    class Medusa2GameLogic : BasePGSlotGame
    {
        private double  _minGroupWinRate   = 0.0;
        private double  _totalGroupWinRate = 0.0;

        protected override bool SupportPurchaseFree
        {
            get { return false; }
        }
        protected override double DefaultBetSize
        {
            get { return 0.1; }
        }
        protected override int DefaultBetLevel
        {
            get { return 5; }
        }
        protected override int BaseBet
        {
            get
            {
                return 30;
            }
        }
        protected override string DefaultResult
        {
            get
            {
                return "{\"si\":{\"wp\":null,\"lw\":null,\"sr\":null,\"ssc\":0,\"fs\":null,\"bns\":null,\"ft\":null,\"msb\":0,\"ms\":0,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":5,\"cs\":0.01,\"rl\":[3,9,5,9,3,8,8,5,4,3,7,5,8,2,3],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":22.14,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":94.96,\"max\":94.96}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public Medusa2GameLogic()
        {
            _gameID = GAMEID.Medusa2;
            GameName = "Medusa2";
        }
        protected override void initGameData()
        {
            base.initGameData();
            _pgGameConfig.ml.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            _pgGameConfig.cs.AddRange(new double[] { 0.01, 0.05, 0.1 });
        }
        protected override void convertWinsByBet(dynamic jsonParams, float currentBet)
        {
            base.convertWinsByBet((object)jsonParams, currentBet);
            if (!IsNullOrEmpty(jsonParams["ctw"]))
                jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["bns"]) && IsArrayOrObject(jsonParams["bns"]) && !IsNullOrEmpty(jsonParams["bns"]["wa"]))
                jsonParams["bns"]["wa"] = convertWinByBet((double)jsonParams["bns"]["wa"], currentBet);

            if (!IsNullOrEmpty(jsonParams["bns"]) && IsArrayOrObject(jsonParams["bns"]) && !IsNullOrEmpty(jsonParams["bns"]["ca"]))
                jsonParams["bns"]["ca"] = convertWinByBet((double)jsonParams["bns"]["ca"], currentBet);

        }
        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBet = (double)  infoDocument["defaultbet"];
                _normalMaxID        = (int)     infoDocument["normalmaxid"];
                _emptySpinCount     = (int)     infoDocument["mincount"];
                _naturalSpinCount   = (int)     infoDocument["normalselectcount"];
                var rtps            = infoDocument["rtps"] as BsonArray;
                _totalGroupWinRate  = (double)rtps[0];
                _minGroupWinRate    = (double)rtps[1];

                if (_totalGroupWinRate < 1.0)
                    _logger.Error("Group win rate doesn't satisfy condition {0}", this.GameName);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
            }
        }
        protected override async Task<BasePGSlotSpinResult> generateSpinResult(BasePGSlotBetInfo betInfo, string strUserID, double userBalance, UserBonus userBonus, bool usePayLimit)
        {
            BasePGSlotSpinData   spinData = null;
            BasePGSlotSpinResult result = null;

            if (betInfo.HasRemainResponse)
            {
                BasePGResponse nextResponse = betInfo.pullRemainResponse();
                result                      = calculateResult(betInfo, nextResponse.Response, false, userBalance, 0.0);

                if (!betInfo.HasRemainResponse)
                    betInfo.RemainReponses = null;
                return result;
            }

            float totalBet = betInfo.TotalBet;
            double realBetMoney = totalBet;

            if (SupportPurchaseFree && betInfo.PurchaseFree)
                realBetMoney = totalBet * getPurchaseMultiple(betInfo);

            spinData = await selectRandomStop(userBonus, totalBet, betInfo);
            result   = calculateResult(betInfo, spinData.SpinStrings[0], true, userBalance, realBetMoney);
            if (spinData.SpinStrings.Count > 1)
                betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
            return result;
        }
        protected override async Task<BasePGSlotSpinData> selectRandomStop(BasePGSlotBetInfo betInfo)
        {
            Medusa2BetInfo  medusaBetInfo   = betInfo as Medusa2BetInfo;
            double          totalBet        = betInfo.TotalBet;

            if (medusaBetInfo.SpinDataPerBet.ContainsKey(totalBet))
            {
                if (medusaBetInfo.SpinDataPerBet[totalBet].Count > 0)
                {
                    BasePGSlotSpinData pickedSpinData = medusaBetInfo.SpinDataPerBet[totalBet][0];
                    medusaBetInfo.SpinDataPerBet[totalBet].RemoveAt(0);
                    return pickedSpinData;
                }
                medusaBetInfo.SpinDataPerBet.Remove(totalBet);
            }

            double payoutRate = _config.PayoutRate;
            double targetC    = 1.0 * payoutRate / 100.0;
            
            if (targetC >= _totalGroupWinRate)
                targetC = _totalGroupWinRate;

            if (targetC < _minGroupWinRate)
                targetC = _minGroupWinRate;

            double x = (_totalGroupWinRate - targetC) / (_totalGroupWinRate - _minGroupWinRate);
            double y = 1.0 - x;

            int spinGroupID = 0;
            if (Pcg.Default.NextDouble(0.0, 1.0) <= x)
                spinGroupID = Pcg.Default.Next(1, _emptySpinCount + 1);
            else
                spinGroupID = Pcg.Default.Next(1, _naturalSpinCount + 1);
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, spinGroupID), TimeSpan.FromSeconds(10.0));
            var groupSpinData    = convertBsonToGroupSpinData(spinDataDocument);
            int count            = groupSpinData.SpinDatas.Count;

            if (!checkPayoutRate(totalBet * count, count * totalBet * groupSpinData.GroupOdd))
            {
                spinGroupID         = Pcg.Default.Next(1, _emptySpinCount + 1);
                spinDataDocument    = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, spinGroupID), TimeSpan.FromSeconds(10.0));
                groupSpinData       = convertBsonToGroupSpinData(spinDataDocument);
                count               = groupSpinData.SpinDatas.Count;
                sumUpBetWin(totalBet * count, count * totalBet * groupSpinData.GroupOdd);
            }

            BasePGSlotSpinData spinData = groupSpinData.SpinDatas[0];
            groupSpinData.SpinDatas.RemoveAt(0);
            medusaBetInfo.SpinDataPerBet.Add(totalBet, groupSpinData.SpinDatas);
            return spinData;
        }
        protected GroupedSpinData convertBsonToGroupSpinData(BsonDocument document)
        {
            double   spinOdd        = (double)document["odd"];
            string   strData        = (string)document["data"];
            string[] strSpinDatas   = strData.Split(new string[] { "####" }, StringSplitOptions.RemoveEmptyEntries);
            int      count          = (int)document["spincount"];
            var      spinDatas      = new List<BasePGSlotSpinData>();

            for (int i = 0; i < count; i++)
            {
                List<string> spinResponses = new List<string>(strSpinDatas[i].Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                spinDatas.Add(new BasePGSlotSpinData(0, 0.0, spinResponses));
            }
            return new GroupedSpinData(spinDatas, spinOdd);
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                Medusa2BetInfo betInfo  = new Medusa2BetInfo(this.BaseBet);
                betInfo.BetSize         = (float)Math.Round((double)message.Pop(), 2);
                betInfo.BetLevel        = (int)message.Pop();
                betInfo.PurchaseFree    = (int)message.Pop() == 2;

                if (betInfo.BetSize <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetSize <= 0 in Medusa2GameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetSize);
                    return;
                }
                BasePGSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strUserID, out oldBetInfo))
                {
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetSize      = betInfo.BetSize;
                    oldBetInfo.BetLevel     = betInfo.BetLevel;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in WinWinWonGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePGSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            Medusa2BetInfo betInfo = new Medusa2BetInfo(this.BaseBet);
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override async Task onProcMessage(string strUserID, GITMessage message, UserBonus userBonus, double userBalance)
        {
            if(message.MsgCode == MsgCodes.UPDATEGAMEINFO)
            {
                onUpdateGameInfo(strUserID, message, userBalance);
                return;
            }
            await base.onProcMessage(strUserID, message, userBonus, userBalance);
        }
        private void onUpdateGameInfo(string strUserID, GITMessage message, double userBalance)
        {
            try
            {
                double  cs = (double)   message.Pop();
                int     ml = (int)      message.Pop();
                double  totalBet = Math.Round(cs * ml * BaseBet, 2);                
                if(_dicUserBetInfos.ContainsKey(strUserID))
                {
                    Medusa2BetInfo betInfo = _dicUserBetInfos[strUserID] as Medusa2BetInfo;
                    BasePGSlotSpinResult lastResult = null;
                    if (betInfo.LastResultsPerBet.ContainsKey(totalBet))
                    {

                        lastResult = betInfo.LastResultsPerBet[totalBet];

                        var si   = JsonConvert.DeserializeObject<dynamic>(lastResult.ResultString);
                        si["bl"] = Math.Round(userBalance, 2);
                        si["cs"] = Math.Round(cs, 2);
                        si["ml"] = ml;
                        var dt   = new { si = si };

                        var response        = new HTTPEnterGameResponse(_pgGameConfig, dt, userBalance);
                        var responseData    = new GetGameInfoResponse(response);

                        GITMessage responseMessage = new GITMessage(MsgCodes.UPDATEGAMEINFO);
                        responseMessage.Append(JsonConvert.SerializeObject(responseData.dt));

                        Sender.Tell(new ToUserMessage((int)_gameID, responseMessage));
                        return;
                    }
                }
                dynamic defaultResult = JsonConvert.DeserializeObject<dynamic>(this.DefaultResult);
                defaultResult["si"]["bl"] = Math.Round(userBalance, 2);
                defaultResult["si"]["cs"] = Math.Round(cs, 2);
                defaultResult["si"]["ml"] = ml;

                var updateResponse        = new HTTPEnterGameResponse(_pgGameConfig, defaultResult, userBalance);
                var updateResponseData    = new GetGameInfoResponse(updateResponse);
                var updateResponseMessage = new GITMessage(MsgCodes.UPDATEGAMEINFO);
                updateResponseMessage.Append(JsonConvert.SerializeObject(updateResponseData.dt));
                Sender.Tell(new ToUserMessage((int)_gameID, updateResponseMessage));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in Medusa2GameLogic::onUpdateGameInfo {0}", ex);
            }
        }
        protected override async Task spinGame(string strUserID, UserBonus userBonus, double userBalance)
        {
            try
            {
                BasePGSlotBetInfo betInfo = null;
                if (!_dicUserBetInfos.TryGetValue(strUserID, out betInfo))
                    return;

                byte[] betInfoBytes = backupBetInfo(betInfo);
                byte[] historyBytes = backupHistoryInfo(strUserID);

                BasePGSlotSpinResult lastResult = null;
                if (_dicUserResultInfos.ContainsKey(strUserID))
                    lastResult = _dicUserResultInfos[strUserID];

                double betMoney = betInfo.TotalBet;
                if (betInfo.HasRemainResponse)
                    betMoney = 0.0;

                if (this.SupportPurchaseFree && betInfo.PurchaseFree)
                    betMoney = Math.Round(betMoney * getPurchaseMultiple(betInfo), 2);

                if (userBalance.LT(betMoney, _epsilion) || betMoney < 0.0)
                {
                    _logger.Error("user balance is less than bet money in BasePGSlotGame::spinGame {0} balance:{1}, bet money: {2} game id:{3}",
                        strUserID, userBalance, betMoney, _gameID);
                    return;
                }

                BasePGSlotSpinResult spinResult = await this.generateSpinResult(betInfo, strUserID, userBalance, userBonus, true);
                _dicUserResultInfos[strUserID]  = spinResult;
                var medusaBetInfo               = _dicUserBetInfos[strUserID] as Medusa2BetInfo;
                medusaBetInfo.LastResultsPerBet[Math.Round(betInfo.TotalBet, 2)] = spinResult;

                saveBetResultInfo(strUserID);

                sendGameResult(betInfo, spinResult, strUserID, betMoney, spinResult.WinMoney, userBalance, userBonus);
                addResultToHistory(strUserID, spinResult);

                if (!betInfo.HasRemainResponse && (betInfo.SpinData == null || !(betInfo.SpinData is BasePGSlotSpinData)))
                    saveHistory(strUserID);

                _dicUserLastBackupBetInfos[strUserID]       = betInfoBytes;
                _dicUserLastBackupResultInfos[strUserID]    = lastResult;
                _dicUserLastBackupHistory[strUserID]        = historyBytes;

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::spinGame {0}", ex);
            }
        }

    }
}
