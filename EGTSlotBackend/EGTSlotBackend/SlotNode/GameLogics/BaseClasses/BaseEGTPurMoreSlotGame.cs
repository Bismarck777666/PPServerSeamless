using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using GITProtocol;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using PCGSharp;
using SlotGamesNode.Database;

namespace SlotGamesNode.GameLogics.BaseClasses
{
    internal class BaseEGTPurMoreSlotGame : BaseEGTSlotGame
    {
        protected int _emptySpinCountMorebetMinus = 0;
        
        protected virtual bool SupportPurchaseFreespin
        {
            get { return false; }
        }
        protected virtual double PurchaseFeespinMultiple
        { 
            get { return 1; } 
        }
        protected virtual bool SupportMoreBet
        { 
            get { return false; } 
        }
        protected virtual double MoreBetMultiple
        {
            get { return 1; }
        }

        public BaseEGTPurMoreSlotGame() 
        { 
        }

        protected override async Task OnLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBet = infoDocument["defaultbet"].AsDouble;
                _emptySpinCount = infoDocument["emptycount"].AsInt32;
                _totalSpinCount = infoDocument["totalcount"].AsInt32;
                if(SupportMoreBet)
                {
                    _emptySpinCountMorebetMinus = (int)((1 - 1 / MoreBetMultiple) * (double)_emptySpinCount);
                    if (_emptySpinCount < _emptySpinCountMorebetMinus)
                        _logger.Error("MoreBet Probability Calculation doesn't work in {0}", this.GameName);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
            }
        }

        protected override void ReadBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                BaseClasses.BaseEGTSlotBetInfo betInfo = new BaseClasses.BaseEGTSlotBetInfo();
                betInfo.CurrencyInfo = currency;
                betInfo.GambleType = 0;
                betInfo.GambleHalf = false;

                int betAmount = int.Parse(message.Pop().ToString());
                int featureId = int.Parse(message.Pop().ToString());
                double betMultiplier = double.Parse(message.Pop().ToString());

                if (SupportMoreBet && (BetFeatureType)featureId == BetFeatureType.MOREBET)
                {
                    betInfo.MoreBet = 0;
                } 
                else
                {
                    betInfo.MoreBet = -1;
                }
                    
                if (SupportPurchaseFreespin && (BetFeatureType)featureId == BetFeatureType.FREESPIN)
                {
                    betInfo.isPurchase = true;
                }
                else
                {
                    betInfo.isPurchase = false;
                }

                BaseClasses.BaseEGTSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strGlobalUserID, out oldBetInfo))
                {
                    oldBetInfo.GambleType = betInfo.GambleType;
                    oldBetInfo.GambleHalf = betInfo.GambleHalf;

                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.MoreBet = betInfo.MoreBet;
                    oldBetInfo.CurrencyInfo = betInfo.CurrencyInfo;
                    oldBetInfo.PlayBet = betAmount;
                    oldBetInfo.MoreBet = betInfo.MoreBet;
                    oldBetInfo.isPurchase = betInfo.isPurchase;
                    oldBetInfo.FeatureId = featureId;
                    oldBetInfo.BetMultiplier = betMultiplier;
                }
                else
                {
                    betInfo.PlayBet = betAmount;
                    betInfo.FeatureId = featureId;
                    betInfo.BetMultiplier = betMultiplier;
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseEGTSlotGame::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override async Task<BaseClasses.BaseEGTSlotSpinResult> GenerateSpinResult(BaseClasses.BaseEGTSlotBetInfo betInfo, BaseClasses.BaseEGTSlotSpinResult lastResult, string strUserID, int websiteID, double userBalance, double betMoney, bool usePayLimit, string sessionKey, string messageId)
        {
            BasePPSlotSpinData spinData = null;
            BaseClasses.BaseEGTSlotSpinResult result = null;

            string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);
            if (betInfo.HasRemainResponse)
            {
                BaseClasses.BaseEGTActionToResponse nextResponse = betInfo.pullRemainResponse();
                result = CalculateResult(betInfo, lastResult, strGlobalUserID, nextResponse.Response, false, userBalance, betInfo.PlayBet, 0, sessionKey, messageId);
                betInfo.SpinInfo = nextResponse.Response;
                ++betInfo.FreespinRoundUsed;

                if (!betInfo.HasRemainResponse)
                    betInfo.RemainReponses = null;
                return result;
            }

            ++TestNumber;
            double realBetMoney = betInfo.PlayBet;

            if (SupportPurchaseFreespin && betInfo.isPurchase)
            {
                spinData = await SelectMinStartFreeSpinData(betInfo);
                result = CalculateResult(betInfo, lastResult, strGlobalUserID, spinData.SpinStrings[0], true, userBalance, betMoney, spinData.SpinOdd, sessionKey, messageId);

                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = BuildResponseList(spinData.SpinStrings);

                SumUpWebsiteBetWin(websiteID, realBetMoney, GetBetUnitMoney(realBetMoney, betInfo));
                betInfo.SpinInfo = spinData.SpinStrings[0];
                return result;
            }

            spinData = await SelectRandomStop(websiteID, realBetMoney, betInfo);

            // test code
            //if(TestNumber % 300 == 0)
            //{
            //    if (spinData.SpinStrings.Count > 1)
            //    {
            //        result = calculateResult(betInfo, lastResult, strGlobalUserID, spinData.SpinStrings[0], true, userBalance, betMoney, spinData.SpinOdd, sessionKey, messageId);
            //        betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
            //        betInfo.SpinInfo = spinData.SpinStrings[0];
            //        return result;
            //    }
            //}
            // end test code

            double totalWin = 0;
            if (spinData.SpinOdd > 0)
                totalWin = (int)GetBetUnitMoney(realBetMoney, betInfo) * spinData.SpinOdd;

            if (!usePayLimit || await CheckWebsitePayoutRate(websiteID, realBetMoney, totalWin))
            {
                do
                {
                    result = CalculateResult(betInfo, lastResult, strGlobalUserID, spinData.SpinStrings[0], true, userBalance, betMoney, spinData.SpinOdd, sessionKey, messageId);
                    if (spinData.SpinStrings.Count > 1)
                        betInfo.RemainReponses = BuildResponseList(spinData.SpinStrings);
                    betInfo.SpinInfo = spinData.SpinStrings[0];
                    return result;
                } while (false);
            }

            spinData = await SelectEmptySpin(betInfo);
            result = CalculateResult(betInfo, lastResult, strGlobalUserID, spinData.SpinStrings[0], true, userBalance, betMoney, spinData.SpinOdd, sessionKey, messageId);

            SumUpWebsiteBetWin(websiteID, realBetMoney, 0.0);
            betInfo.SpinInfo = spinData.SpinStrings[0];
            return result;
        }

        protected override OddAndIDData SelectRandomOddAndID(int websiteID, BaseClasses.BaseEGTSlotBetInfo betInfo)
        {
            double payoutRate = GetPayoutRate(websiteID);
            double randomDouble = Pcg.Default.NextDouble(0.0, 100.0);

            int selectedID = 0;
            if (randomDouble >= payoutRate || payoutRate == 0.0)
            {
                selectedID = Pcg.Default.Next(1, _emptySpinCount + 1);
            }
            else if (SupportMoreBet && betInfo.isMoreBet)
            {
                selectedID = _emptySpinCountMorebetMinus + Pcg.Default.Next(1, _totalSpinCount - _emptySpinCountMorebetMinus + 1);
            }
            else
            {
                selectedID = Pcg.Default.Next(1, _totalSpinCount + 1);
            }

            OddAndIDData selectedOddAndID = new OddAndIDData();
            selectedOddAndID.ID = selectedID;
            return selectedOddAndID;
        }

        protected virtual async Task<BasePPSlotSpinData> SelectMinStartFreeSpinData(BaseClasses.BaseEGTSlotBetInfo betInfo)
        {
            try
            {
                BsonDocument spinDataDocument = null;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                           new SelectSpinTypeOddRangeRequest(GameName, 100, PurchaseFeespinMultiple * 0.1, PurchaseFeespinMultiple * 0.9), TimeSpan.FromSeconds(10.0));

                //var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                //        new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.GENERAL),
                //        TimeSpan.FromSeconds(10.0));

                return ConvertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseEGTSlotGame::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }

        protected virtual double GetBetUnitMoney(double betAmount, BaseEGTSlotBetInfo betInfo)
        {
            if (SupportMoreBet && betInfo.isMoreBet)
                betAmount = betAmount / MoreBetMultiple;
            if(SupportPurchaseFreespin && betInfo.isPurchase)
                betAmount = betAmount / PurchaseFeespinMultiple;
            return betAmount;
        }
        protected override int GetSpinWinMoney(string strSpinData, BaseClasses.BaseEGTSlotBetInfo betInfo, bool isFreespin = false)
        {
            int totalWin = (int)((double)int.Parse(JObject.Parse(strSpinData)["winAmount"]?.ToString() ?? "") * ((double)GetBetUnitMoney(betInfo.PlayBet, betInfo) / (double)_spinDataDefaultBet));
            return totalWin;
        }

        protected override void ChangeSpinString(JToken spinResponse, BaseEGTSlotBetInfo betInfo)
        {
            // Apply Muliplier to all winAmount of spinResponse

            if (spinResponse.Type == JTokenType.Object)
            {
                foreach (var prop in spinResponse.Children<JProperty>())
                {
                    if (prop.Name == "winAmount" && prop.Value.Type == JTokenType.Integer)
                    {
                        int original = prop.Value.Value<int>();
                        prop.Value = original * GetBetUnitMoney(betInfo.PlayBet, betInfo) / _spinDataDefaultBet;
                    }
                    else if (prop.Name == "winAmount" && prop.Value.Type == JTokenType.Float)
                    {
                        double original = prop.Value.Value<double>();
                        prop.Value = original * GetBetUnitMoney(betInfo.PlayBet, betInfo) / _spinDataDefaultBet;
                    }
                    else
                    {
                        ChangeSpinString(prop.Value, betInfo);
                    }
                }
            }
            else if (spinResponse.Type == JTokenType.Array)
            {
                foreach (var child in spinResponse.Children())
                {
                    ChangeSpinString(child, betInfo);
                }
            }
        }

        protected override bool CheckGamblePossible(double winmoney, int betmoney, BaseClasses.BaseEGTSlotBetInfo betInfo)
        {
            double maxMoneyToGamble = Math.Min(MaxWinunitToGamble * GetBetUnitMoney(betmoney, betInfo), MaxWinToGamble);
            return winmoney <= maxMoneyToGamble && winmoney > 0;
        }
    }
}
