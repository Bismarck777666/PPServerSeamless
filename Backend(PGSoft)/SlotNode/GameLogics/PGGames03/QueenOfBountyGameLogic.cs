using Akka.Actor;
using SlotGamesNode.Database;
using GITProtocol;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SlotGamesNode.GameLogics
{
    class QueenOfBountyGameLogic : BasePGSlotGame
    {
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
                return 20;
            }
        }
        protected override string DefaultResult
        {
            get
            {
                return "{\"si\":{\"wp\":null,\"lw\":null,\"ssaw\":0.0,\"rns\":null,\"orl\":null,\"cbf\":null,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"ml\":3,\"cs\":0.1,\"rl\":[3,5,8,3,0,2,1,7,1,6,0,4,0,2,1,7,3,5,8,3],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":1.86,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"lw\":null,\"ssaw\":0.0,\"rns\":null,\"orl\":[3,5,8,3,7,2,4,7,8,6,7,2,7,2,4,7,3,5,8,3],\"cbf\":null,\"fs\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.1,\"rl\":[3,5,8,3,7,2,4,7,8,6,7,2,7,2,4,7,3,5,8,3],\"sid\":\"1762869959077177861\",\"psid\":\"1762869959077177861\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.74,\"max\":96.74}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public QueenOfBountyGameLogic()
        {
            _gameID = GAMEID.QueenOfBounty;
            GameName = "QueenOfBounty";
        }
        protected override void initGameData()
        {
            base.initGameData();
            _pgGameConfig.ml.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            _pgGameConfig.cs.AddRange(new double[] { 0.01, 0.05, 0.1 });
        }

        protected override async Task onProcMessage(string strUserID, int websiteID, GITMessage message, UserBonus userBonus, double userBalance, Currencies currency, bool isAffiliate)
        {
            if (message.MsgCode == MsgCodes.SPIN)
            {
                var strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);
                if (_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    BasePGSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
                    if (!betInfo.HasRemainResponse && betInfo.SpinData != null && betInfo.SpinData is BasePGSlotStartSpinData)
                    {
                        onFreespinSelect(strUserID, websiteID,  message, userBalance, betInfo, betInfo.SpinData as BasePGSlotStartSpinData, isAffiliate);
                        return;
                    }
                }
            }
            await base.onProcMessage(strUserID, websiteID, message,userBonus, userBalance, currency, isAffiliate);
        }
        private void onFreespinSelect(string strUserID, int websiteID, GITMessage message, double userBalance, BasePGSlotBetInfo betInfo, BasePGSlotStartSpinData startSpinData, bool isAffiliate)
        {
            try
            {
                long    lastTransID     = (long)message.Pop();
                float   betSize         = (float)Math.Round((double)message.Pop(), 2);
                float   betLevel        = (int)message.Pop();
                bool    purchaseFree    = (int)message.Pop() == 2;
                string  wk              = (string)message.Pop();
                bool    ig              = (bool)message.Pop();
                int     gt              = (int)message.Pop();
                int     fss             = (int)message.Pop();

                var     strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);
                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || fss < 0 || fss > 2)
                    return;

                BasePGSlotSpinResult lastResult = _dicUserResultInfos[strGlobalUserID];
                BasePGSlotSpinData freeSpinData = convertBsonToSpinData(startSpinData.FreeSpins[fss]);
                betInfo.SpinData = null;

                List<string> freeSpinStrings = new List<string>();
                for (int i = 0; i < freeSpinData.SpinStrings.Count; i++)
                    freeSpinStrings.Add(addStartWinToResponse(freeSpinData.SpinStrings[i], startSpinData.StartOdd));

                betInfo.RemainReponses = buildResponseList(freeSpinStrings, 1);
                double selectedWin = (startSpinData.StartOdd + freeSpinData.SpinOdd) * betInfo.TotalBet;
                double maxWin = startSpinData.MaxOdd * betInfo.TotalBet;
                if(!isAffiliate)
                    sumUpWebsiteBetWin(websiteID, 0.0, selectedWin - maxWin);

                dynamic lastSpinContext= JsonConvert.DeserializeObject<dynamic>(lastResult.ResultString);
                dynamic newSpinContext = JsonConvert.DeserializeObject<dynamic>(freeSpinStrings[0]);
                newSpinContext["psid"] = betInfo.TransactionID.ToString();
                newSpinContext["sid"]  = createTransactionID().ToString();
                newSpinContext["blb"]  = Math.Round(userBalance, 2);
                newSpinContext["blab"] = Math.Round(userBalance, 2);
                newSpinContext["bl"]   = Math.Round(userBalance, 2);
                newSpinContext["trl"]  = lastSpinContext["trl"];
                newSpinContext["torl"] = lastSpinContext["torl"];
                newSpinContext["orl"]  = lastSpinContext["orl"];
                newSpinContext["esb"]  = lastSpinContext["esb"];
                newSpinContext["ebb"]  = lastSpinContext["ebb"];
                newSpinContext["es"]   = lastSpinContext["es"];
                newSpinContext["eb"]   = lastSpinContext["eb"];

                convertWinsByBet(newSpinContext, betInfo.TotalBet);
                convertBetsByBet(newSpinContext, betInfo.BetSize, betInfo.BetLevel, betInfo.TotalBet);

                BasePGSlotSpinResult newResult = new BasePGSlotSpinResult();
                newResult.TotalWin = 0.0;
                newResult.ResultString = JsonConvert.SerializeObject(newSpinContext);
                _dicUserResultInfos[strGlobalUserID] = newResult;

                saveBetResultInfo(strGlobalUserID);
                addResultToHistory(strGlobalUserID, newResult);
                GITMessage responseMessage = new GITMessage(MsgCodes.SPIN);
                responseMessage.Append(newResult.ResultString);
                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in QueenOfBountyGameLogic::onFreespinSelect {0}", ex);
            }
        }

        protected string addStartWinToResponse(string strResponse, double startOdd)
        {
            dynamic jsonParams = JsonConvert.DeserializeObject<dynamic>(strResponse);
            if (!IsNullOrEmpty(jsonParams["aw"]))
                jsonParams["aw"] = Math.Round((double)jsonParams["aw"] + startOdd * _spinDataDefaultBet, 2);

            return JsonConvert.SerializeObject(jsonParams);
        }
        protected override async Task<BasePGSlotSpinResult> generateSpinResult(BasePGSlotBetInfo betInfo, string strUserID, int websiteID, UserBonus userBonus, double userBalance, bool isAffiliate)
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

            spinData = await selectRandomStop(totalBet, betInfo, websiteID,userBonus, isAffiliate);

            double totalWin = 0.0;
            if (spinData is BasePGSlotStartSpinData)
                totalWin = totalBet * (spinData as BasePGSlotStartSpinData).MaxOdd;
            else
                totalWin = totalBet * spinData.SpinOdd;

            if (isAffiliate || await checkWebsitePayoutRate(websiteID, realBetMoney, totalWin))
            {
                if (spinData is BasePGSlotStartSpinData)
                    betInfo.SpinData = spinData;
                else
                    betInfo.SpinData = null;

                if (spinData.IsEvent)
                {
                    _bonusSendMessage   = null;
                    _rewardedBonusMoney = totalWin;
                    _isRewardedBonus    = true;
                }

                result = calculateResult(betInfo, spinData.SpinStrings[0], true, userBalance, realBetMoney);
                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
                return result;
            }

            double emptyWin = 0.0;
            if (SupportPurchaseFree && betInfo.PurchaseFree)
            {
                spinData = await selectMinStartFreeSpinData(betInfo);
                result = calculateResult(betInfo, spinData.SpinStrings[0], true, userBalance, realBetMoney);
                emptyWin = totalBet * spinData.SpinOdd;

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
            sumUpWebsiteBetWin(websiteID, realBetMoney, emptyWin);
            return result;
        }

        protected override async Task<BasePGSlotSpinData> selectRandomStop(BasePGSlotBetInfo betInfo, int websiteID, bool isAffiliate)
        {
            OddAndIDData selectedOddAndID = selectRandomOddAndID(betInfo, websiteID, isAffiliate);
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
            if(freeSpinGroup == 0)
            return new int[] { 200, 201, 202 };
            else
                return new int[] { 203, 204, 205 };

        }

        protected override async Task<BasePGSlotSpinData> selectRandomStartFreeSpinData(BasePGSlotBetInfo betInfo)
        {
            try
            {
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.SELFREE),
                        TimeSpan.FromSeconds(10.0));

                var spinData = convertBsonToSpinData(spinDataDocument);
                if (!(spinData is BasePGSlotStartSpinData))
                    return spinData;

                BasePGSlotStartSpinData startSpinData = spinData as BasePGSlotStartSpinData;
                await buildStartFreeSpinData(startSpinData, -1.0, -1.0);
                return startSpinData;

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in EgyptBookOfMysteryGameLogic::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected override async Task<BasePGSlotSpinData> selectMinStartFreeSpinData(BasePGSlotBetInfo betInfo)
        {
            try
            {
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                            new SelectSelFreeMinStartRequest(GameName), TimeSpan.FromSeconds(10.0));

                BasePGSlotStartSpinData spinData = convertBsonToSpinData(spinDataDocument) as BasePGSlotStartSpinData;
                double minOdd = 0.0;
                double maxOdd = PurchaseFreeMultiple * 0.5 - spinData.SpinOdd;

                await buildStartFreeSpinData(spinData, minOdd, maxOdd);
                return spinData;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePGSlotGame::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
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
    }
}
