﻿using Akka.Actor;
using Akka.Event;
using GITProtocol;
using MongoDB.Bson;
using PCGSharp;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    internal class BasePPClassicGame : BasePPSlotGame
    {
        protected double[]  _spinDataDefaultBets    = null;
        protected int[]     _normalMaxIDs           = null;
        protected int[]     _naturalSpinCounts      = null;
        protected int[]     _emptySpinCounts        = null;
        protected int[]     _startIDs               = null;

        protected virtual int LineCount => 5;

        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBets    = new double[LineCount];
                _normalMaxIDs           = new int[LineCount];
                _naturalSpinCounts      = new int[LineCount];
                _emptySpinCounts        = new int[LineCount];
                _startIDs               = new int[LineCount];

                BsonArray defaultBetsArray          = infoDocument["defaultbet"] as BsonArray;
                BsonArray normalMaxIDArray          = infoDocument["normalmaxid"] as BsonArray;
                BsonArray emptyCountArray           = infoDocument["emptycount"] as BsonArray;
                BsonArray normalSelectCountArray    = infoDocument["normalselectcount"] as BsonArray;
                BsonArray startIDArray              = infoDocument["startid"] as BsonArray;
                for (int i = 0; i < LineCount; i++)
                {
                    _spinDataDefaultBets[i] = (double)defaultBetsArray[i];
                    _normalMaxIDs[i]        = (int)normalMaxIDArray[i];
                    _emptySpinCounts[i]     = (int)emptyCountArray[i];
                    _naturalSpinCounts[i]   = (int)normalSelectCountArray[i];
                    _startIDs[i]            = (int)startIDArray[i];
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
            }
        }

        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo,string strSpinResponse,bool isFirst)
        {
            try
            {
                BasePPSlotSpinResult result = new BasePPSlotSpinResult();
                Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

                int betType = betInfo.LineCount - 1;
                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                if (dicParams.ContainsKey("w"))
                {
                    double num = double.Parse(dicParams["w"]) / _spinDataDefaultBets[betType] * (double)betInfo.TotalBet;
                    dicParams["w"] = Math.Round(num, 2).ToString();
                }

                //WinLine정보
                int winLineID = 0;
                do
                {
                    string strKey = string.Format("l{0}", winLineID);
                    if (!dicParams.ContainsKey(strKey))
                        break;
                    
                    string[] strParts = dicParams[strKey].Split(new string[1] { "~" }, StringSplitOptions.None);
                    if (strParts.Length >= 3)
                    {
                        double win = double.Parse(strParts[2]) / _spinDataDefaultBets[betType] * (double)betInfo.TotalBet;
                        strParts[2] = Math.Round(win, 2).ToString();
                        dicParams[strKey] = string.Join("~", strParts);
                    }
                    winLineID++;
                }
                while (true);

                result.NextAction   = ActionTypes.DOSPIN;
                result.ResultString = convertKeyValuesToString(dicParams);
                result.TotalWin     = double.Parse(dicParams["w"]);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPClassicGame::calculateResult {0}", ex);
                return null;
            }
        }

        protected override bool addSpinResultToHistory(string strGlobalUserID,int index,int counter,string strSpinResult,BasePPSlotBetInfo betInfo,BasePPSlotSpinResult spinResult)
        {
            if (!_dicUserHistory.ContainsKey(strGlobalUserID))
                return false;
            
            _dicUserHistory[strGlobalUserID].log.Add(new BasePPHistoryItem()
            {
                cr = string.Format("symbol={0}&c={1}&repeat=0&action=doSpin&index={2}&counter={3}&l={4}", SymbolName, betInfo.BetPerLine, index, counter, betInfo.LineCount),
                sr = strSpinResult
            });
            _dicUserHistory[strGlobalUserID].baseBet    = (double)betInfo.TotalBet;
            _dicUserHistory[strGlobalUserID].win        = spinResult.TotalWin;
            return true;
        }

        protected override async Task<OddAndIDData> selectRandomOddAndID(int agentID,BasePPSlotBetInfo betInfo,bool isMoreBet)
        {
            int     lineCount       = betInfo.LineCount;
            int     betType         = lineCount - 1;
            double  payoutRate      = getPayoutRate(agentID);
            double  randomDouble    = Pcg.Default.NextDouble(0.0, 100.0);

            int selectedID = 0;

            if (randomDouble >= payoutRate || payoutRate == 0.0)
                selectedID = _startIDs[betType] + Pcg.Default.Next(0, _emptySpinCounts[betType]);
            else
                selectedID = _startIDs[betType] + Pcg.Default.Next(0, _naturalSpinCounts[betType]);

            OddAndIDData selectedOddAndID = new OddAndIDData();
            selectedOddAndID.ID = selectedID;
            return selectedOddAndID;
        }

        protected override async Task<BasePPSlotSpinData> selectEmptySpin(int companyID,BasePPSlotBetInfo betInfo)
        {
            int betType             = betInfo.LineCount - 1;
            int id                  = _startIDs[betType] + Pcg.Default.Next(0, _emptySpinCounts[betType]);
            var spinDataDocument    = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, id), TimeSpan.FromSeconds(10.0));
            return convertBsonToSpinData(spinDataDocument);
        }

        protected override async Task<BasePPSlotSpinData> selectRangeSpinData(int websiteID,double minOdd,double maxOdd,BasePPSlotBetInfo betInfo)
        {
            int betType             = betInfo.LineCount - 1;
            var spinDataDocument    = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                new SelectSpinTypeOddRangeRequestWithBetType(GameName, -1, minOdd, maxOdd, -1, betType), TimeSpan.FromSeconds(10.0));

            if (spinDataDocument == null)
                return null;
            return convertBsonToSpinData(spinDataDocument);
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                BasePPSlotBetInfo betInfo = new BasePPSlotBetInfo();
                betInfo.BetPerLine  = (float)message.Pop();
                betInfo.LineCount   = (int)message.Pop();

                if (betInfo.BetPerLine <= 0.0 || float.IsNaN(betInfo.BetPerLine) || float.IsInfinity(betInfo.BetPerLine))
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in BasePPSlotGame::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }
                
                if (betInfo.LineCount < 1 || betInfo.LineCount > LineCount)
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

                    oldBetInfo.BetPerLine   = betInfo.BetPerLine;
                    oldBetInfo.LineCount    = betInfo.LineCount;
                }
                else
                    _dicUserBetInfos.Add(strUserID, betInfo);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPClassicGame::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override string makeSpinResultString(BasePPSlotBetInfo betInfo,BasePPSlotSpinResult spinResult,double betMoney,double userBalance,int index,int counter,bool isInit)
        {
            Dictionary<string, string> dicParams = splitResponseToParams(spinResult.ResultString);
            if (spinResult.HasBonusResult)
            {
                Dictionary<string, string> dicBonusParams = splitResponseToParams(spinResult.BonusResultString);
                dicParams = mergeSpinToBonus(dicParams, dicBonusParams);
            }

            dicParams["balance_bonus"]  = "0.00";
            dicParams["stime"]          = GameUtils.GetCurrentUnixTimestampMillis().ToString();
            dicParams["sver"]           = "5";
            dicParams["l"]              = betInfo.LineCount.ToString();
            dicParams["c"]              = Math.Round((double)betInfo.BetPerLine, 2).ToString();

            if (index > 0)
            {
                dicParams[nameof(index)]    = index.ToString();
                dicParams[nameof(counter)]  = (counter + 1).ToString();
            }

            if (isInit)
                dicParams["action"] = "doSpin";
            
            dicParams["balance"]        = Math.Round(userBalance - (isInit ? 0.0 : betMoney), 2).ToString();
            dicParams["balance_cash"]   = Math.Round(userBalance - (isInit ? 0.0 : betMoney), 2).ToString();

            if (SupportPurchaseFree && betInfo.PurchaseFree)
                dicParams["puri"] = "0";
            else
                dicParams.Remove("puri");
            
            if (SupportMoreBet)
            {
                if (betInfo.MoreBet)
                    dicParams["bl"] = "1";
                else
                    dicParams["bl"] = "0";
            }

            if (isInit)
                supplementInitResult(dicParams, betInfo, spinResult);

            overrideSomeParams(betInfo, dicParams);
            return convertKeyValuesToString(dicParams);
        }
    }
}
