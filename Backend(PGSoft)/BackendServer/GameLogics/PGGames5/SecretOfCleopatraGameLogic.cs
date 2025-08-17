using GITProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCGSharp;
using Akka.Actor;
using SlotGamesNode.Database;
using MongoDB.Bson;

namespace SlotGamesNode.GameLogics
{
    class SecretOfCleopatraGameLogic : BasePGSlotGame
    {
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 75.0; }
        }
        protected override bool HasPurEnableOption
        {
            get { return true; }
        }
        protected override double DefaultBetSize
        {
            get { return 0.2; }
        }
        protected override int DefaultBetLevel
        {
            get { return 5; }
        }
        protected override int BaseBet
        {
            get
            {
                return 10;
            }
        }
        protected override string DefaultResult
        {
            get
            {
                return "{\"si\":{\"acw\":0.0,\"wp\":null,\"wwp\":null,\"sp\":null,\"wpl\":null,\"sw\":null,\"swm\":null,\"twwm\":0.0,\"sc\":null,\"ix\":false,\"orl\":null,\"gm\":0,\"prm\":0,\"ifsg\":false,\"rp\":null,\"rs\":null,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":6,\"cs\":0.05,\"rl\":[7,2,4,4,1,3,3,2,8,2,5,5],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":0.01,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.74,\"max\":96.74}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public SecretOfCleopatraGameLogic()
        {
            _gameID = GAMEID.SecretOfCleopatra;
            GameName = "SecretOfCleopatra";
        }
        protected override void initGameData()
        {
            base.initGameData();
            _pgGameConfig.ml.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            _pgGameConfig.cs.AddRange(new double[] { 0.02, 0.1, 0.2 });
        }
        protected override void convertWinsByBet(dynamic jsonParams, float currentBet)
        {
            base.convertWinsByBet((object)jsonParams, currentBet);
            if (!IsNullOrEmpty(jsonParams["ctw"]))
                jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["acw"]))
                jsonParams["acw"] = convertWinByBet((double)jsonParams["acw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["twwm"]))
                jsonParams["twwm"] = convertWinByBet((double)jsonParams["twwm"], currentBet);

            if (!IsNullOrEmpty(jsonParams["sw"]) && IsArrayOrObject(jsonParams["sw"]))
            {
                var swValues = jsonParams["sw"].ToObject<Dictionary<string, object>>();
                foreach (KeyValuePair<string, object> pair in swValues)
                {
                    JArray swValueArrays = pair.Value as JArray;
                    for (int i = 0; i < swValueArrays.Count; i++)
                        jsonParams["sw"][pair.Key][i] = convertWinByBet((double)swValueArrays[i], currentBet);
                }
            }
            if (!IsNullOrEmpty(jsonParams["swm"]) && IsArrayOrObject(jsonParams["swm"]))
            {
                var swValues = jsonParams["swm"].ToObject<Dictionary<string, double>>();
                foreach (KeyValuePair<string, double> pair in swValues)
                    jsonParams["swm"][pair.Key] = convertWinByBet(pair.Value, currentBet);
            }
        }
        protected override async Task onProcMessage(string strUserID, GITMessage message, UserBonus userBonus, double userBalance)
        {
            if (message.MsgCode == MsgCodes.SPIN)
            {
                if (_dicUserBetInfos.ContainsKey(strUserID))
                {
                    BasePGSlotBetInfo betInfo = _dicUserBetInfos[strUserID];
                    if (!betInfo.HasRemainResponse && betInfo.SpinData != null && betInfo.SpinData is BasePGSlotStartSpinData)
                    {
                        onFreespinSelect(strUserID, message, userBalance, betInfo, betInfo.SpinData as BasePGSlotStartSpinData);
                        return;
                    }
                }
            }
            await base.onProcMessage(strUserID, message, userBonus, userBalance);
        }
        private void onFreespinSelect(string strUserID, GITMessage message, double userBalance, BasePGSlotBetInfo betInfo, BasePGSlotStartSpinData startSpinData)
        {
            try
            {
                long   lastTransID  = (long)message.Pop();
                float  betSize      = (float)Math.Round((double)message.Pop(), 2);
                float  betLevel     = (int)message.Pop();
                bool   purchaseFree = (int)message.Pop() == 2;
                string wk           = (string)message.Pop();
                bool   ig           = (bool)message.Pop();
                int    gt           = (int)message.Pop();
                int    fss          = (int)message.Pop();

                if (!_dicUserResultInfos.ContainsKey(strUserID))
                    return;

                BasePGSlotSpinResult lastResult         = _dicUserResultInfos[strUserID];
                var                  lastResultContext  = JToken.Parse(lastResult.ResultString);

                double[] probs = new double[] { 0.55, 0.7, 0.85 };
                int      fsm   = (int) lastResultContext["fs"]["fsm"];

                JToken  newResultContext = null;
                bool    isEnded          = false;
                do
                {
                    newResultContext = JToken.Parse(lastResult.ResultString);
                    newResultContext["orl"] = null;
                    newResultContext["rs"] = null;
                    newResultContext["st"] = 3;
                    newResultContext["gm"] = 1;
                    if (newResultContext["fstc"]["3"] != null)
                        newResultContext["fstc"]["3"] = (int)newResultContext["fstc"]["3"] + 1;
                    else
                        newResultContext["fstc"]["3"] = 1;
                    if (gt == 2)
                    {
                        if (Pcg.Default.NextDouble() > probs[fsm - 1]) //Gamble Fail
                        {
                            newResultContext["nst"]  = 1;
                            newResultContext["fs"]   = null;
                            newResultContext["ifsg"] = false;
                            betInfo.SpinData         = null;
                            isEnded = true;
                            break;
                        }
                        else //Gamble Success
                        {
                            fsm++;
                            newResultContext["fs"]["fsm"] = fsm;
                            if (fsm < 4)
                            {
                                newResultContext["nst"]   = 3;
                                newResultContext["ifsg"]  = true;
                                break;
                            }
                       }
                    }
                    newResultContext["ifsg"] = false;
                    newResultContext["nst"]  = 21;
                    BasePGSlotSpinData freeSpinData = convertBsonToSpinData(startSpinData.FreeSpins[fsm - 1]);
                    betInfo.SpinData                = null;
                    List<string> freeSpinStrings = new List<string>();
                    for (int i = 0; i < freeSpinData.SpinStrings.Count; i++)
                        freeSpinStrings.Add(addStartWinToResponse(freeSpinData.SpinStrings[i], startSpinData.StartOdd));

                    betInfo.RemainReponses = buildResponseList(freeSpinStrings, 0);
                    double selectedWin      = (startSpinData.StartOdd + freeSpinData.SpinOdd) * betInfo.TotalBet;
                    double maxWin           = startSpinData.MaxOdd * betInfo.TotalBet;
                    sumUpBetWin(0.0, selectedWin - maxWin);
                } while (false);

                newResultContext["psid"]    = betInfo.TransactionID.ToString();
                newResultContext["sid"]     = createTransactionID().ToString();
                newResultContext["blb"]     = Math.Round(userBalance, 2);
                newResultContext["blab"]    = Math.Round(userBalance, 2);
                newResultContext["bl"]      = Math.Round(userBalance, 2);

                BasePGSlotSpinResult newResult = new BasePGSlotSpinResult();
                newResult.TotalWin      = 0.0;
                newResult.ResultString  = JsonConvert.SerializeObject(newResultContext);
                _dicUserResultInfos[strUserID] = newResult;

                saveBetResultInfo(strUserID);
                addResultToHistory(strUserID, newResult);
    
                if(isEnded)
                {
                    double selectedWin = startSpinData.StartOdd  * betInfo.TotalBet;
                    double maxWin      = startSpinData.MaxOdd * betInfo.TotalBet;
                    sumUpBetWin(0.0, selectedWin - maxWin);
                    saveHistory(strUserID);
                }
                GITMessage responseMessage = new GITMessage(MsgCodes.SPIN);
                responseMessage.Append(newResult.ResultString);
                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SecretOfCleopatraGameLogic::onGambleCollect {0}", ex);
            }
        }
        protected string addStartWinToResponse(string strResponse, double startOdd)
        {
            dynamic jsonParams = JsonConvert.DeserializeObject<dynamic>(strResponse);
            if (!IsNullOrEmpty(jsonParams["aw"]))
                jsonParams["aw"] = Math.Round((double)jsonParams["aw"] + startOdd * _spinDataDefaultBet, 2);

            return JsonConvert.SerializeObject(jsonParams);
        }
        protected override async Task<BasePGSlotSpinResult> generateSpinResult(BasePGSlotBetInfo betInfo, string strUserID, double userBalance, UserBonus userBonus, bool usePayLimit)
        {
            BasePGSlotSpinData spinData = null;
            BasePGSlotSpinResult result = null;

            if (betInfo.HasRemainResponse)
            {
                BasePGResponse nextResponse = betInfo.pullRemainResponse();
                result = calculateResult(betInfo, nextResponse.Response, false, userBalance, 0.0);
                if (!betInfo.HasRemainResponse)
                    betInfo.RemainReponses = null;
                return result;
            }

            float totalBet = betInfo.TotalBet;
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
                spinData        = await selectMinStartFreeSpinData(betInfo);
                double maxOdd   = 0.5 * PurchaseFreeMultiple - (spinData as BasePGSlotStartSpinData).StartOdd;
                await buildStartFreeSpinData(spinData as BasePGSlotStartSpinData, 0.0, maxOdd);
                result          = calculateResult(betInfo, spinData.SpinStrings[0], true, userBalance, realBetMoney);
                emptyWin        = totalBet * (spinData as BasePGSlotStartSpinData).MaxOdd;
            }
            else
            {
                spinData = await selectEmptySpin(betInfo);
                result   = calculateResult(betInfo, spinData.SpinStrings[0], true, userBalance, realBetMoney);
            }

            if (spinData.SpinStrings.Count > 1)
                betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);

            if (spinData is BasePGSlotStartSpinData)
                betInfo.SpinData = spinData;
            else
                betInfo.SpinData = null;
            sumUpBetWin(realBetMoney, emptyWin);
            return result;
        }

        protected override async Task<BasePGSlotSpinData> selectRandomStop(BasePGSlotBetInfo betInfo)
        {
            OddAndIDData selectedOddAndID = selectRandomOddAndID(betInfo);
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, selectedOddAndID.ID), TimeSpan.FromSeconds(10.0));
            BasePGSlotSpinData spinData = convertBsonToSpinData(spinDataDocument);
            if (!(spinData is BasePGSlotStartSpinData))
                return spinData;

            BasePGSlotStartSpinData startSpinData = spinData as BasePGSlotStartSpinData;
            await buildStartFreeSpinData(startSpinData, -1.0, -1.0);
            return startSpinData;
        }
        protected int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            if (freeSpinGroup == 0)
                return new int[] { 200, 201, 202, 203 };
            else if (freeSpinGroup == 1)
                return new int[] { 204, 205, 206, 207 };
            else
                return new int[] { 208, 209, 210, 211 };
        }
        protected async Task buildStartFreeSpinData(BasePGSlotStartSpinData startSpinData, double minOdd, double maxOdd)
        {
            startSpinData.FreeSpins = new List<BsonDocument>();
            int[] freeSpinTypes = PossibleFreeSpinTypes(startSpinData.FreeSpinGroup);
            double maxFreeOdd = 0.0;
            for (int i = 0; i < freeSpinTypes.Length; i++)
            {
                BsonDocument childFreeSpin = null;
                if (minOdd < 0.0 || maxOdd < 0.0)
                {
                    childFreeSpin = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeRandomRequest(GameName, freeSpinTypes[i]), TimeSpan.FromSeconds(10.0));
                }
                else
                {
                    childFreeSpin = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, freeSpinTypes[i], minOdd, maxOdd), TimeSpan.FromSeconds(10.0));
                }

                double childOdd = (double)childFreeSpin["odd"];
                if (childOdd > maxFreeOdd)
                    maxFreeOdd = childOdd;
                startSpinData.FreeSpins.Add(childFreeSpin);
            }
            startSpinData.MaxOdd = startSpinData.StartOdd + maxFreeOdd;
        }
        protected override BasePGSlotSpinData convertBsonToSpinData(BsonDocument document)
        {
            int spinType = (int)document["spintype"];
            double spinOdd = (double)document["odd"];
            string strData = (string)document["data"];

            List<string> spinResponses = new List<string>(strData.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
            if (spinType == 100)
            {
                BasePGSlotStartSpinData startSpinData = new BasePGSlotStartSpinData();
                startSpinData.StartOdd = spinOdd;
                startSpinData.FreeSpinGroup = (int)document["freespintype"];
                startSpinData.SpinStrings = spinResponses;
                return startSpinData;
            }
            else
            {
                return new BasePGSlotSpinData(spinType, spinOdd, spinResponses);
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
                spinData        = await selectMinStartFreeSpinData(betInfo);
                double maxOdd   = 0.5 * PurchaseFreeMultiple - (spinData as BasePGSlotStartSpinData).StartOdd;
                await buildStartFreeSpinData(spinData as BasePGSlotStartSpinData, 0.0, maxOdd);
            }
            else
            {
                spinData = await selectRandomStartFreeSpinData(betInfo);
                await buildStartFreeSpinData(spinData as BasePGSlotStartSpinData, -1.0, -1.0);
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

    }

}
