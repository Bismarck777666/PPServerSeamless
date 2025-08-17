using Akka.Actor;
using GITProtocol;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PCGSharp;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using Akka.Event;

namespace SlotGamesNode.GameLogics
{
    class DragonKingdomEyesOfFireBetInfo : BasePPSlotBetInfo
    {
        public Dictionary<double, List<BasePPSlotSpinData>> SpinDataPerBet { get; set; }
        public Dictionary<double, string> LastAccvPerBet { get; set; }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(this.SpinDataPerBet.Count);
            foreach (KeyValuePair<double, List<BasePPSlotSpinData>> pair in this.SpinDataPerBet)
            {
                writer.Write(pair.Key);
                writer.Write(pair.Value.Count);
                for (int i = 0; i < pair.Value.Count; i++)
                    pair.Value[i].SerializeTo(writer);
            }
            writer.Write(this.LastAccvPerBet.Count);
            foreach (KeyValuePair<double, string> pair in this.LastAccvPerBet)
            {
                writer.Write(pair.Key);
                writer.Write(pair.Value);
            }
        }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            int count = reader.ReadInt32();
            this.SpinDataPerBet = new Dictionary<double, List<BasePPSlotSpinData>>();
            for (int i = 0; i < count; i++)
            {
                double key = reader.ReadDouble();
                var spinDatas = new List<BasePPSlotSpinData>();
                int spinDataCount = reader.ReadInt32();
                for (int j = 0; j < spinDataCount; j++)
                {
                    BasePPSlotSpinData spinData = new BasePPSlotSpinData();
                    spinData.SerializeFrom(reader);
                    spinDatas.Add(spinData);
                }
                this.SpinDataPerBet.Add(key, spinDatas);
            }
            count = reader.ReadInt32();
            this.LastAccvPerBet = new Dictionary<double, string>();
            for (int i = 0; i < count; i++)
            {
                double key = reader.ReadDouble();
                string value = reader.ReadString();
                this.LastAccvPerBet[key] = value;
            }
        }
        public DragonKingdomEyesOfFireBetInfo()
        {
            this.SpinDataPerBet = new Dictionary<double, List<BasePPSlotSpinData>>();
            this.LastAccvPerBet = new Dictionary<double, string>();
        }
    }
    class DragonKingdomEyesOfFireGameLogic : BasePPSlotGame
    {
        private double _spinDataRTP     = 0.0;
        private double _minSpinDataRTP  = 0.0;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs5drmystery";
            }
        }
        protected override bool SupportReplay
        {
            get
            {
                return false;
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 5; }
        }
        protected override int ServerResLineCount
        {
            get { return 5; }
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
                return "def_s=4,3,5,6,4,9,7,5,3&cfgs=3840&accm=cp;cp~tp~lvl~sc;cp~pp&reel1=8,9,9,9,7,2,5,7,7,7,6,5,5,5,3,3,3,3,4,9,8,8,8,2,2,2,4,4,4,6,6,6,2,6,3,6,9,3,6,5,9,2,7,4,6,3,6,3,4,7,5,3,7,3,6,3,7,9,2,9,3,7,6,2,7,6,3,9,7,9,7,6,2,3,4,2,6,3,9,6,9,6,7,6,2,9,2&ver=2&reel0=9,9,9,4,3,4,4,4,6,9,8,2,7,7,7,7,5,2,2,2,3,3,3,5,5,5,8,8,8,6,6,6,4,5,7,2,5&acci=0;1;2&def_sb=7,4,6&def_sa=7,5,4&reel2=6,6,6,4,4,4,4,8,6,3,7,2,8,8,8,9,5,5,5,5,9,9,9,3,3,3,7,7,7,2,2,2,5,2,8,5,2,8,3,4,5,4,2,3,8,2,8,5,7,3&accv=0;0~1~0~0;0~0&scatters=1~0,0,0~0,0,0~1,1,1&gmb=0,0,0&rt=d&gameInfo={props:{lvls:\"1:1,2:5,3:10,4:15,5:20,6:25\"}}&sc=40.00,80.00,120.00,160.00,200.00,400.00,600.00,800.00,1000.00,1500.00,2000.00,3000.00,5000.00,10000.00,15000.00,20000.00&defc=200.00&wilds=2~25,0,0~1,1,1&bonuses=0&fsbonus=&paytable=0,0,0;0,0,0;0,0,0;20,0,0;15,0,0;10,0,0;8,0,0;5,0,0;3,0,0;2,0,0&accInit=[{id:0,mask:\"cp;mp\"},{id:1,mask:\"cp;tp;lvl;sc;cl\"},{id:2,mask:\"cp;pp;mp\"}]";
            }
        }


        #endregion

        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBet = (double)infoDocument["defaultbet"];
                _normalMaxID        = (int)infoDocument["normalmaxid"];
                _emptySpinCount     = (int)infoDocument["emptycount"];
                _naturalSpinCount   = (int)infoDocument["normalselectcount"];
                _spinDataRTP        = (double)infoDocument["normalRTP"];
                _minSpinDataRTP     = (double)infoDocument["emptyRTP"];

                if (SupportPurchaseFree)
                {
                    var purchaseOdds        = infoDocument["purchaseodds"] as BsonArray;
                    _totalFreeSpinWinRate   = (double)purchaseOdds[1];
                    _minFreeSpinWinRate     = (double)purchaseOdds[0];
                }

                if (this.SupportMoreBet)
                {
                    _anteBetMinusZeroCount = (int)((1.0 - 1.0 / MoreBetMultiple) * (double)_naturalSpinCount);
                    if (_anteBetMinusZeroCount > _emptySpinCount)
                        _logger.Error("More Bet Probabily calculation doesn't work in {0}", this.GameName);
                }

                if (this.SupportPurchaseFree && this.PurchaseFreeMultiple > _totalFreeSpinWinRate)
                    _logger.Error("freespin win rate doesn't satisfy condition {0}", this.GameName);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
            }
        }


        public DragonKingdomEyesOfFireGameLogic()
        {
            _gameID = GAMEID.DragonKingdomEyesOfFire;
            GameName = "DragonKingdomEyesOfFire";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);	    
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            DragonKingdomEyesOfFireBetInfo betInfo = new DragonKingdomEyesOfFireBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new DragonKingdomEyesOfFireBetInfo();
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                DragonKingdomEyesOfFireBetInfo betInfo = new DragonKingdomEyesOfFireBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount  = (int)message.Pop();
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in DragonKingdomEyesOfFireGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine = betInfo.BetPerLine;
                    oldBetInfo.LineCount  = betInfo.LineCount;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in DragonKingdomEyesOfFireGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        public override async Task<BasePPSlotSpinData> selectRandomStop(int agentID, UserBonus userBonus, double baseBet, bool isChangedLineCount, BasePPSlotBetInfo betInfo)
        {
            if (this.SupportPurchaseFree && betInfo.PurchaseFree)
                return await selectPurchaseFreeSpin(agentID, betInfo, baseBet, userBonus);

            return await selectRandomSpinData(agentID, betInfo);
        }
        protected async Task<BasePPSlotSpinData> selectRandomSpinData(int agentID, BasePPSlotBetInfo betInfo)
        {
            var dragonBetInfo = betInfo as DragonKingdomEyesOfFireBetInfo;
            double totalBet = Math.Round(betInfo.TotalBet, 2);

            if (dragonBetInfo.SpinDataPerBet.ContainsKey(totalBet))
            {
                if (dragonBetInfo.SpinDataPerBet[totalBet].Count > 0)
                {
                    BasePPSlotSpinData pickedSpinData = dragonBetInfo.SpinDataPerBet[totalBet][0];
                    dragonBetInfo.SpinDataPerBet[totalBet].RemoveAt(0);
                    return pickedSpinData;
                }
                dragonBetInfo.SpinDataPerBet.Remove(totalBet);
            }
            double payoutRate = getPayoutRate(agentID);
            double targetC = 1.0 * payoutRate / 100.0;
            if (targetC >= _spinDataRTP)
                targetC = _spinDataRTP;

            if (targetC < _minSpinDataRTP)
                targetC = _minSpinDataRTP;

            double x = (_spinDataRTP - targetC) / (_spinDataRTP - _minSpinDataRTP);
            double y = 1.0 - x;

            int spinGroupID = 0;
            if (Pcg.Default.NextDouble(0.0, 1.0) <= x)
                spinGroupID = Pcg.Default.Next(1, _emptySpinCount + 1);
            else
                spinGroupID = Pcg.Default.Next(1, _naturalSpinCount + 1);

            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, spinGroupID), TimeSpan.FromSeconds(10.0));
            var groupSpinData    = convertBsonToGroupSpinData(spinDataDocument);
            int count            = groupSpinData.SpinDatas.Count;

            if (!await checkWebsitePayoutRate(agentID, totalBet * count, count * totalBet * groupSpinData.GroupOdd))
            {
                spinGroupID      = Pcg.Default.Next(1, _emptySpinCount + 1);
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, spinGroupID), TimeSpan.FromSeconds(10.0));
                groupSpinData    = convertBsonToGroupSpinData(spinDataDocument);
                count           = groupSpinData.SpinDatas.Count;
                sumUpWebsiteBetWin(agentID, totalBet * count, count * totalBet * groupSpinData.GroupOdd);
            }
            BasePPSlotSpinData spinData = groupSpinData.SpinDatas[0];
            groupSpinData.SpinDatas.RemoveAt(0);
            dragonBetInfo.SpinDataPerBet.Add(totalBet, groupSpinData.SpinDatas);
            return spinData;
        }
        protected virtual GroupedSpinData convertBsonToGroupSpinData(BsonDocument document)
        {
            double spinOdd          = (double)document["odd"];
            string strData          = (string)document["data"];
            string strOddsData      = (string)document["odds"];
            string[] strSpinDatas   = strData.Split(new string[] { "###" }, StringSplitOptions.RemoveEmptyEntries);
            string[] strSpinOdds    = strOddsData.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            int count = (int)document["count"];
            List<BasePPSlotSpinData> spinDatas = new List<BasePPSlotSpinData>();
            for (int i = 0; i < count; i++)
            {
                List<string> spinResponses = new List<string>(strSpinDatas[i].Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                double childSpinOdd        = double.Parse(strSpinOdds[i]);
                spinDatas.Add(new BasePPSlotSpinData(0, childSpinOdd, spinResponses));
            }
            return new GroupedSpinData(spinDatas, spinOdd);
        }
        protected override async Task<BasePPSlotSpinResult> generateSpinResult(BasePPSlotBetInfo betInfo, string strUserID, int websiteID, UserBonus userBonus, bool usePayLimit)
        {
            BasePPSlotSpinData spinData = null;
            BasePPSlotSpinResult result = null;

            if (betInfo.HasRemainResponse)
            {
                BasePPActionToResponse nextResponse = betInfo.pullRemainResponse();
                result = calculateResult(betInfo, nextResponse.Response, false);

                //프리게임이 끝났는지를 검사한다.
                if (!betInfo.HasRemainResponse)
                    betInfo.RemainReponses = null;
                return result;
            }

            //유저의 총 베팅액을 얻는다.
            float totalBet = betInfo.TotalBet;
            double realBetMoney = totalBet;

            if (SupportPurchaseFree && betInfo.PurchaseFree)
                realBetMoney = totalBet * getPurchaseMultiple(betInfo); //100.0

            spinData = await selectRandomStop(websiteID, userBonus, totalBet, false, betInfo);

            //첫자료를 가지고 결과를 계산한다.
            double totalWin = totalBet * spinData.SpinOdd;
            if (!usePayLimit || !betInfo.PurchaseFree || await checkWebsitePayoutRate(websiteID, realBetMoney, totalWin))
            {
                result = calculateResult(betInfo, spinData.SpinStrings[0], true);
                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);

                return result;
            }

            double emptyWin = 0.0;
            if (betInfo.PurchaseFree)
            {
                spinData    = await selectMinStartFreeSpinData(betInfo);
                result      = calculateResult(betInfo, spinData.SpinStrings[0], true);
                emptyWin    = totalBet * spinData.SpinOdd;

                //뒤에 응답자료가 또 있다면
                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
            }
            sumUpWebsiteBetWin(websiteID, realBetMoney, emptyWin);
            return result;
        }

        protected override void saveBetResultInfo(string strGlobalUserID)
        {
            try
            {
                if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    var betInfo = _dicUserBetInfos[strGlobalUserID] as DragonKingdomEyesOfFireBetInfo;
                    Dictionary<string, string> dicParamas = splitResponseToParams(_dicUserResultInfos[strGlobalUserID].ResultString);

                    double totalBet = Math.Round(betInfo.TotalBet, 2);
                    if (dicParamas.ContainsKey("accv"))
                        betInfo.LastAccvPerBet[totalBet] = dicParamas["accv"];
                }
                base.saveBetResultInfo(strGlobalUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in DragonKingdomEyesOfFireGameLogic::saveBetInfo {0}", ex);
            }
        }
        protected override void supplementInitResult(Dictionary<string, string> dicParams, BasePPSlotBetInfo baseBetInfo, BasePPSlotSpinResult spinResult)
        {
            base.supplementInitResult(dicParams, baseBetInfo, spinResult);
            DragonKingdomEyesOfFireBetInfo betInfo = baseBetInfo as DragonKingdomEyesOfFireBetInfo;
            if (betInfo.LastAccvPerBet != null && betInfo.LastAccvPerBet.Count > 0)
            {
                JArray accbArray = new JArray();
                foreach (KeyValuePair<double, string> pair in betInfo.LastAccvPerBet)
                {
                    JToken accbObj      = JToken.Parse("{}");
                    accbObj["b"]        = pair.Key.ToString();
                    accbObj["v"]        = JToken.Parse("{}");

                    string[] strParts   = pair.Value.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    accbObj["v"]["0"]   = strParts[0];
                    accbObj["v"]["1"]   = strParts[1];
                    accbObj["v"]["2"]   = strParts[2];
                    accbArray.Add(accbObj);
                }
                dicParams["accb"] = JsonConvert.SerializeObject(accbArray);
            }
        }
    }
}
