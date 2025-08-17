using Akka.Actor;
using Amazon.Runtime;
using SlotGamesNode.Database;
using GITProtocol;
using Google.Protobuf.WellKnownTypes;
using MongoDB.Bson;
using MongoDB.Driver.Core.WireProtocol.Messages;
using Newtonsoft.Json;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    public enum CaishenGambleTypes
    {
        None       = 0,   
        FreeSpins  = 1,
        Multiplier = 2
    }
    class CaishenWinsStartSpinData : BasePGSlotStartSpinData
    {
        public int FreeSpinCount        { get; set; }
        public int Multiplier           { get; set; }

        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            this.FreeSpinCount  = reader.ReadInt32();
            this.Multiplier     = reader.ReadInt32();
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            writer.Write(this.FreeSpinCount);
            writer.Write(this.Multiplier);
        }
    }
    class CaishenWinsBetInfo : BasePGSlotBetInfo
    {
        public CaishenWinsBetInfo(int baseBet) : base(baseBet)
        {
        }
        public override void SerializeFrom(BinaryReader reader)
        {
            this.BetSize = reader.ReadSingle();
            this.BetLevel = reader.ReadInt32();
            this.BaseBet = reader.ReadInt32();
            this.TransactionID = reader.ReadInt64();
            this.PurchaseFree = reader.ReadBoolean();
            int remainCount = reader.ReadInt32();
            if (remainCount == 0)
            {
                this.RemainReponses = null;
            }
            else
            {
                this.RemainReponses = new List<BasePGResponse>();
                for (int i = 0; i < remainCount; i++)
                {
                    string strResponse = (string)reader.ReadString();
                    this.RemainReponses.Add(new BasePGResponse(strResponse));
                }
            }
            int spinDataType = reader.ReadInt32();
            if (spinDataType == 0)
            {
                this.SpinData = null;
            }
            else if (spinDataType == 1)
            {
                this.SpinData = new BasePGSlotSpinData();
                this.SpinData.SerializeFrom(reader);
            }
            else if (spinDataType == 2)
            {
                this.SpinData = new CaishenWinsStartSpinData();
                this.SpinData.SerializeFrom(reader);
            }
        }
    }
    class CaishenWinsGameLogic : BasePGSlotGame
    {
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 50.0; }
        }
        protected override bool HasPurEnableOption
        {
            get { return true; }
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
                return 20;
            }
        }
        protected override string DefaultResult
        {
            get
            {
                return "{\"si\":{\"wp\":null,\"twp\":null,\"bwp\":null,\"lw\":null,\"lwa\":0.00,\"orl\":[8,4,1,10,7,2,2,2,3,3,0,0,0,11,10,0,9,12,7,7,4,4,0,5,5,12,5,1,11,6],\"trl\":[3,6,2,4],\"now\":7200,\"nowpr\":[5,3,4,6,4,5],\"snww\":null,\"esb\":{\"1\":[5,6,7],\"2\":[8,9],\"3\":[10,11,12],\"4\":[20,21],\"5\":[23,24]},\"ebb\":{\"1\":{\"fp\":5,\"lp\":7,\"bt\":1,\"ls\":1},\"2\":{\"fp\":8,\"lp\":9,\"bt\":1,\"ls\":2},\"3\":{\"fp\":10,\"lp\":12,\"bt\":2,\"ls\":1},\"4\":{\"fp\":20,\"lp\":21,\"bt\":1,\"ls\":2},\"5\":{\"fp\":23,\"lp\":24,\"bt\":1,\"ls\":1}},\"es\":{\"1\":[5,6,7],\"2\":[8,9],\"3\":[10,11,12],\"4\":[20,21],\"5\":[23,24]},\"eb\":{\"1\":{\"fp\":5,\"lp\":7,\"bt\":1,\"ls\":1},\"2\":{\"fp\":8,\"lp\":9,\"bt\":1,\"ls\":2},\"3\":{\"fp\":10,\"lp\":12,\"bt\":2,\"ls\":1},\"4\":{\"fp\":20,\"lp\":21,\"bt\":1,\"ls\":2},\"5\":{\"fp\":23,\"lp\":24,\"bt\":1,\"ls\":1}},\"ssaw\":0.00,\"ptbr\":null,\"tptbr\":null,\"sc\":2,\"rs\":null,\"fs\":null,\"mrl\":[[0],[0],[0],[0]],\"nmrl\":[[0],[0],[0],[0]],\"gm\":1,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"ml\":3,\"cs\":20.0,\"rl\":[8,4,1,10,7,2,2,2,3,3,0,0,0,11,10,0,9,12,7,7,4,4,0,5,5,12,5,1,11,6],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":16454.50,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.76,\"max\":96.76}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public CaishenWinsGameLogic()
        {
            _gameID = GAMEID.CaishenWins;
            GameName = "CaishenWins";
        }
        protected override void initGameData()
        {
            base.initGameData();
            _pgGameConfig.ml.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            _pgGameConfig.cs.AddRange(new double[] { 0.01, 0.05, 0.1 });
        }

        protected override async Task onProcMessage(string strUserID, GITMessage message, UserBonus userBonus, double userBalance)
        {
            if (message.MsgCode == MsgCodes.SPIN)
            {
                if(_dicUserBetInfos.ContainsKey(strUserID))
                {
                    BasePGSlotBetInfo betInfo = _dicUserBetInfos[strUserID];
                    if(!betInfo.HasRemainResponse && betInfo.SpinData != null && betInfo.SpinData is BasePGSlotStartSpinData)
                    {
                        onGambleCollect(strUserID, message, userBalance, betInfo, betInfo.SpinData as CaishenWinsStartSpinData);
                        return;
                    }
                }
            }
            await base.onProcMessage(strUserID, message, userBonus, userBalance);
        }
        private void onGambleCollect(string strUserID, GITMessage message, double userBalance, BasePGSlotBetInfo betInfo, CaishenWinsStartSpinData startSpinData)
        {
            try
            {
                long lastTransID     = (long)message.Pop();
                float betSize        = (float)  Math.Round((double)message.Pop(), 2);
                float   betLevel     = (int)    message.Pop();
                bool    purchaseFree = (int)    message.Pop() == 2;
                string  wk           = (string) message.Pop();
                bool    ig           = (bool)   message.Pop();
                int     gt           = (int)    message.Pop();
                
                CaishenGambleTypes gambleType = CaishenGambleTypes.None;
                if (ig)
                    gambleType = (CaishenGambleTypes) gt;

                if (!_dicUserResultInfos.ContainsKey(strUserID))
                    return;

                BasePGSlotSpinResult lastResult  = _dicUserResultInfos[strUserID];
                dynamic              spinContext = JsonConvert.DeserializeObject<dynamic>(lastResult.ResultString);

                spinContext["psid"]     = betInfo.TransactionID.ToString();
                spinContext["sid"]      = createTransactionID().ToString();
                spinContext["blb"]      = Math.Round(userBalance, 2);
                spinContext["blab"]     = Math.Round(userBalance, 2);
                spinContext["bl"]       = Math.Round(userBalance, 2);

                if (gambleType == CaishenGambleTypes.FreeSpins && startSpinData.FreeSpinCount >= 20)
                    return;

                if (gambleType == CaishenGambleTypes.Multiplier && startSpinData.Multiplier >= 20)
                    return;

                spinContext["tw"] = 0.0;
                spinContext["tb"] = 0.0;
                spinContext["lw"] = null;
                spinContext["st"] = 5;
                if (gambleType != CaishenGambleTypes.None)
                    spinContext["gt"] = (int)gambleType;
                else
                    spinContext["gt"] = 0;


                bool isCollect    = false;
                bool isFailed     = false;
                do
                {
                    if (gambleType == CaishenGambleTypes.None)
                    {
                        isCollect = true;
                        break;
                    }
                    if (gambleType == CaishenGambleTypes.FreeSpins)
                    {
                        int currentFreeSpinCount = startSpinData.FreeSpinCount;
                        if (PCGSharp.Pcg.Default.Next(0, currentFreeSpinCount + 2) >= currentFreeSpinCount)
                        {
                            isFailed = true;
                            break;
                        }
                    }
                    else
                    {
                        int currentMultiplier = startSpinData.Multiplier;
                        if (PCGSharp.Pcg.Default.Next(0, currentMultiplier + 2) >= currentMultiplier)
                        {
                            isFailed = true;
                            break;
                        }
                    }
                    if(gambleType == CaishenGambleTypes.FreeSpins)
                        startSpinData.FreeSpinCount += 2;
                    else
                        startSpinData.Multiplier += 2;

                    if (startSpinData.FreeSpinCount >= 20 && startSpinData.Multiplier >= 20)
                    {
                        isCollect = true;
                        break;
                    }
                    if(gambleType == CaishenGambleTypes.Multiplier)
                        spinContext["fs"]["m"]      = startSpinData.Multiplier - 2;

                    spinContext["fs"]["nm"]     = startSpinData.Multiplier;
                    spinContext["fs"]["s"]      = startSpinData.FreeSpinCount;
                    spinContext["fs"]["ts"]     = startSpinData.FreeSpinCount;
                    spinContext["fs"]["nosa"]   = startSpinData.FreeSpinCount;                           
                } while (false);    
                
                if(isCollect)
                {
                    spinContext["fs"]["m"]    = startSpinData.Multiplier;
                    spinContext["fs"]["nm"]   = startSpinData.Multiplier;
                    spinContext["fs"]["s"]    = startSpinData.FreeSpinCount;
                    spinContext["fs"]["ts"]   = startSpinData.FreeSpinCount;
                    spinContext["fs"]["nosa"] = 0;
                    spinContext["nst"]        = 21;
                    int freeSpinIndex = (startSpinData.FreeSpinCount - 8) / 2 - startSpinData.FreeSpinGroup;
                    BasePGSlotSpinData  freeSpinData  = convertBsonToSpinData(startSpinData.FreeSpins[freeSpinIndex]);
                    betInfo.SpinData = null;

                    List<string> freeSpinStrings = new List<string>();
                    for (int i = 0; i < freeSpinData.SpinStrings.Count; i++)
                        freeSpinStrings.Add(addStartWinToResponse(freeSpinData.SpinStrings[i], startSpinData.StartOdd, startSpinData.Multiplier / 8.0, startSpinData.Multiplier));

                    betInfo.RemainReponses = buildResponseList(freeSpinStrings, 0);

                    double selectedWin = (startSpinData.StartOdd + freeSpinData.SpinOdd * startSpinData.Multiplier / 8.0) * betInfo.TotalBet;
                    double maxWin      = startSpinData.MaxOdd * betInfo.TotalBet;
                    sumUpBetWin(0.0, selectedWin - maxWin);                    
                }
                else if(isFailed)
                {
                    spinContext["fs"]  = null;
                    spinContext["nst"] = 1;
                    sumUpBetWin(0.0, (startSpinData.StartOdd - startSpinData.MaxOdd) * betInfo.TotalBet);
                    betInfo.SpinData   = null;
                }
                else
                { 
                    spinContext["nst"] = 5;
                }
                BasePGSlotSpinResult newResult  = new BasePGSlotSpinResult();
                newResult.TotalWin              = 0.0;
                newResult.ResultString          = JsonConvert.SerializeObject(spinContext);
                _dicUserResultInfos[strUserID] = newResult;

                saveBetResultInfo(strUserID);
                addResultToHistory(strUserID, newResult);
                GITMessage responseMessage = new GITMessage(MsgCodes.SPIN);
                responseMessage.Append(newResult.ResultString);
                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
                if(isFailed)
                    saveHistory(strUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in CaishenWinsGameLogic::onGambleCollect {0}", ex);
            }
        }

        protected string addStartWinToResponse(string strResponse, double startOdd, double multiplier, int multiple)
        {
            dynamic jsonParams = JsonConvert.DeserializeObject<dynamic>(strResponse);
            if (!IsNullOrEmpty(jsonParams["aw"]))
                jsonParams["aw"] = Math.Round((double)jsonParams["aw"] * multiplier + startOdd * _spinDataDefaultBet, 2);

            if (!IsNullOrEmpty(jsonParams["tw"]))
                jsonParams["tw"] = Math.Round((double)jsonParams["tw"] * multiplier, 2);

            if (!IsNullOrEmpty(jsonParams["ssaw"]))
                jsonParams["ssaw"] = Math.Round((double)jsonParams["ssaw"] * multiplier, 2);

            if (!IsNullOrEmpty(jsonParams["np"]))
                jsonParams["np"] = Math.Round((double)jsonParams["np"] * multiplier, 2);

            if (!IsNullOrEmpty(jsonParams["fs"]) && IsArrayOrObject(jsonParams["fs"]) && !IsNullOrEmpty(jsonParams["fs"]["aw"]))
                jsonParams["fs"]["aw"] = Math.Round((double)jsonParams["fs"]["aw"] * multiplier, 2);

            if (!IsNullOrEmpty(jsonParams["fs"]) && IsArrayOrObject(jsonParams["fs"]) && !IsNullOrEmpty(jsonParams["fs"]["m"]))
                jsonParams["fs"]["m"] = multiple;

            if (!IsNullOrEmpty(jsonParams["fs"]) && IsArrayOrObject(jsonParams["fs"]) && !IsNullOrEmpty(jsonParams["fs"]["nm"]))
                jsonParams["fs"]["nm"] = multiple;
            return JsonConvert.SerializeObject(jsonParams);
        }
        protected override async Task<BasePGSlotSpinResult> generateSpinResult(BasePGSlotBetInfo betInfo, string strUserID, double userBalance, UserBonus userBonus, bool usePayLimit)
        {
            BasePGSlotSpinData  spinData = null;
            BasePGSlotSpinResult result  = null;

            if (betInfo.HasRemainResponse)
            {
                BasePGResponse nextResponse = betInfo.pullRemainResponse();
                result                      = calculateResult(betInfo, nextResponse.Response, false, userBalance, 0.0);
                if (!betInfo.HasRemainResponse)
                    betInfo.RemainReponses = null;
                return result;
            }

            float totalBet      = betInfo.TotalBet;
            double realBetMoney = totalBet;

            if (SupportPurchaseFree && betInfo.PurchaseFree)
                realBetMoney = totalBet * getPurchaseMultiple(betInfo);

            spinData = await selectRandomStop(userBonus, totalBet, betInfo);

            double totalWin = 0.0;
            if (spinData is BasePGSlotStartSpinData)
                totalWin = totalBet * (spinData as BasePGSlotStartSpinData).MaxOdd;
            else
                totalWin = totalBet * spinData.SpinOdd;

            if (!usePayLimit || spinData.IsEvent || checkPayoutRate(realBetMoney, totalWin))
            {
                if (spinData.IsEvent)
                {
                    _bonusSendMessage = null;
                    _rewardedBonusMoney = totalWin;
                    _isRewardedBonus = true;
                }

                if (spinData is BasePGSlotStartSpinData)
                    betInfo.SpinData = spinData;
                else
                    betInfo.SpinData = null;

                result = calculateResult(betInfo, spinData.SpinStrings[0], true, userBalance, realBetMoney);
                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
                return result;
            }

            double emptyWin = 0.0;
            if (SupportPurchaseFree && betInfo.PurchaseFree)
            {
                spinData    = await selectMinStartFreeSpinData(betInfo);
                await buildMinStartFreeSpinData(spinData as BasePGSlotStartSpinData);
                result      = calculateResult(betInfo, spinData.SpinStrings[0], true, userBalance, realBetMoney);
                emptyWin    = totalBet * (spinData as BasePGSlotStartSpinData).MaxOdd;
                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
            }
            else
            {
                spinData = await selectEmptySpin(betInfo);
                result = calculateResult(betInfo, spinData.SpinStrings[0], true, userBalance, realBetMoney);
            }
            if (spinData is BasePGSlotStartSpinData)
                betInfo.SpinData = spinData;
            else
                betInfo.SpinData = null;
            sumUpBetWin(realBetMoney, emptyWin);
            return result;
        }

        protected override async Task<BasePGSlotSpinData> selectRandomStop(BasePGSlotBetInfo betInfo)
        {
            OddAndIDData selectedOddAndID   = selectRandomOddAndID(betInfo);
            var spinDataDocument            = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, selectedOddAndID.ID), TimeSpan.FromSeconds(10.0));
            BasePGSlotSpinData spinData     =  convertBsonToSpinData(spinDataDocument);
            if (!(spinData is BasePGSlotStartSpinData))
                return spinData;

            BasePGSlotStartSpinData startSpinData = spinData as BasePGSlotStartSpinData;
            await  buildStartFreeSpinData(startSpinData);
            return startSpinData;
        }
        protected int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            List<int> childSpinTypes = new List<int>();
            for (int i = freeSpinGroup; i < 7; i++)
                childSpinTypes.Add(i + 200);
            return childSpinTypes.ToArray();
        }
        protected async Task buildStartFreeSpinData(BasePGSlotStartSpinData startSpinData)
        {
            startSpinData.FreeSpins = new List<BsonDocument>();
            int[] freeSpinTypes = PossibleFreeSpinTypes(startSpinData.FreeSpinGroup);
            double maxFreeOdd = 0.0;
            for (int i = 0; i < freeSpinTypes.Length; i++)
            {
                BsonDocument childFreeSpin = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeRandomRequest(GameName, freeSpinTypes[i]), TimeSpan.FromSeconds(10.0));

                double childOdd = (double)childFreeSpin["odd"];
                if (childOdd > maxFreeOdd)
                    maxFreeOdd = childOdd;
                startSpinData.FreeSpins.Add(childFreeSpin);
            }
            startSpinData.MaxOdd = startSpinData.StartOdd + maxFreeOdd * 2.5;
        }
        protected override BasePGSlotSpinData convertBsonToSpinData(BsonDocument document)
        {
            int spinType    = (int)document["spintype"];
            double spinOdd  = (double)document["odd"];
            string strData  = (string)document["data"];

            List<string> spinResponses = new List<string>(strData.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));

            if (spinType == 100)
            {
                CaishenWinsStartSpinData startSpinData = new CaishenWinsStartSpinData();
                startSpinData.StartOdd                = spinOdd;
                startSpinData.FreeSpinGroup           = (int)document["freespintype"];
                startSpinData.Multiplier              = 8;
                startSpinData.FreeSpinCount           = 8 + startSpinData.FreeSpinGroup * 2;
                startSpinData.SpinStrings             = spinResponses;
                return startSpinData;
            }
            else
            {
                return new BasePGSlotSpinData(spinType, spinOdd, spinResponses);
            }
        }
        protected override BasePGSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            CaishenWinsBetInfo betInfo = new CaishenWinsBetInfo(this.BaseBet);
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                CaishenWinsBetInfo betInfo  = new CaishenWinsBetInfo(this.BaseBet);
                betInfo.BetSize             = (float)Math.Round((double)message.Pop(), 2);
                betInfo.BetLevel            = (int)message.Pop();
                betInfo.PurchaseFree        = (int)message.Pop() == 2;

                if (betInfo.BetSize <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetSize <= 0 in BasePGSlotGame::readBetInfoFromMessage {1}", strUserID, betInfo.BetSize);
                    return;
                }
                BasePGSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strUserID, out oldBetInfo))
                {
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetSize = betInfo.BetSize;
                    oldBetInfo.BetLevel = betInfo.BetLevel;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePGSlotGame::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override async Task<BasePGSlotSpinData> selectPurchaseFreeSpin(BasePGSlotBetInfo betInfo, double baseBet, UserBonus userBonus)
        {
            double payoutRate = _config.PayoutRate;
            double targetC = PurchaseFreeMultiple * payoutRate / 100.0;
            if (targetC >= _totalFreeSpinWinRate)
                targetC = _totalFreeSpinWinRate;

            if (targetC < _minFreeSpinWinRate)
                targetC = _minFreeSpinWinRate;

            double x = (_totalFreeSpinWinRate - targetC) / (_totalFreeSpinWinRate - _minFreeSpinWinRate);
            double y = 1.0 - x;

            BasePGSlotSpinData spinData = null;
            if (Pcg.Default.NextDouble(0.0, 1.0) <= x)
            {
                spinData = await selectMinStartFreeSpinData(betInfo);
                await buildStartFreeSpinData(spinData as BasePGSlotStartSpinData);
            }
            else
            {
                spinData = await selectRandomStartFreeSpinData(betInfo);
                await buildStartFreeSpinData(spinData as BasePGSlotStartSpinData);
            }
            return spinData;
        }
        protected override async Task<BasePGSlotSpinData> selectMinStartFreeSpinData(BasePGSlotBetInfo betInfo)
        {
            try
            {
                BsonDocument spinDataDocument = null;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSelFreeMinStartRequest(GameName, 0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePGSlotGame::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected async Task buildMinStartFreeSpinData(BasePGSlotStartSpinData startSpinData)
        {
            startSpinData.FreeSpins = new List<BsonDocument>();
            int[] freeSpinTypes     = PossibleFreeSpinTypes(startSpinData.FreeSpinGroup);
            double maxFreeOdd       = 0.0;
            double maxLimitOdd      = (0.5 * PurchaseFreeMultiple - startSpinData.StartOdd) / 2.5; 

            for (int i = 0; i < freeSpinTypes.Length; i++)
            {
                BsonDocument childFreeSpin = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, freeSpinTypes[i], 0.0, maxLimitOdd), TimeSpan.FromSeconds(10.0));

                double childOdd = (double)childFreeSpin["odd"];
                if (childOdd > maxFreeOdd)
                    maxFreeOdd = childOdd;
                startSpinData.FreeSpins.Add(childFreeSpin);
            }
            startSpinData.MaxOdd = startSpinData.StartOdd + maxFreeOdd * 2.5;
        }
    }
}
