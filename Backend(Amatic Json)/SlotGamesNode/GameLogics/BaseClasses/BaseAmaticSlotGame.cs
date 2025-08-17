using Akka.Actor;
using GITProtocol;
using GITProtocol.Utils;
using Newtonsoft.Json;
using PCGSharp;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using MongoDB.Bson;

namespace SlotGamesNode.GameLogics
{
    public class BaseAmaticSlotGame : IGameLogicActor
    {
        protected double                            _spinDataDefaultBet     = 0.0f;
        protected int                               _normalMaxID            = 0;
        protected int                               _naturalSpinCount       = 0;
        protected int                               _emptySpinCount         = 0;

        //프리스핀구매시 필요. 디비안의 모든(구매가능한) 프리스핀들의 오드별 아이디어레이
        protected double                            _totalFreeSpinWinRate   = 0.0; //스핀디비안의 모든 프리스핀들의 배당평균값
        protected double                            _minFreeSpinWinRate     = 0.0; //구매금액의 20% - 50%사이에 들어가는 모든 프리스핀들의 평균배당값

        //앤티베팅기능이 있을때만 필요하다.(앤티베팅시 감소시켜야할 빈스핀의 갯수)
        protected int                               _anteBetMinusZeroCount  = 0;


        //매유저의 베팅정보 
        protected Dictionary<string, BaseAmaticSlotBetInfo>        _dicUserBetInfos             = new Dictionary<string, BaseAmaticSlotBetInfo>();

        //유정의 마지막결과정보
        protected Dictionary<string, BaseAmaticSlotSpinResult>  _dicUserResultInfos             = new Dictionary<string, BaseAmaticSlotSpinResult>();

        //백업정보
        protected Dictionary<string, BaseAmaticSlotSpinResult>  _dicUserLastBackupResultInfos   = new Dictionary<string, BaseAmaticSlotSpinResult>();
        protected Dictionary<string, byte[]>                    _dicUserLastBackupBetInfos      = new Dictionary<string, byte[]>();

        protected virtual int       FreeSpinTypeCount       => 0; //유저가 선택가능한 프리스핀종류수
        protected virtual bool      HasPurEnableOption      => false;
        protected virtual bool      HasSelectableFreeSpin   => false;
        protected virtual string    SymbolName              => "";
        protected virtual bool      SupportMoreBet          => false;
        protected virtual double    MoreBetMultiple         =>  1.0;
        protected virtual bool      SupportPurchaseFree     => false;
        protected virtual double    PurchaseFreeMultiple    =>  0.0;
        protected virtual long[]    BettingButton           => new long[] { };
        protected virtual long[]    LINES                   => new long[] { };
        protected virtual int       LineTypeCnt              => 1;
        protected virtual int       Cols                    => 5;
        protected virtual int       FreeCols                => 5;
        protected virtual string    InitString              => "";
        protected virtual int       ReelSetBitNum           => 1;   //1 또는 2

        public BaseAmaticSlotGame()
        {
        }

        protected override void LoadSetting()
        {
            base.LoadSetting();
            initGameData();
        }
        protected virtual void initGameData()
        {
        }
        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
           try
            {
                _spinDataDefaultBet = (double)  infoDocument["defaultbet"];
                _normalMaxID        = (int)     infoDocument["normalmaxid"];
                _emptySpinCount     = (int)     infoDocument["emptycount"];
                _naturalSpinCount   = (int)     infoDocument["normalselectcount"];
                if(SupportPurchaseFree)
                {
                    var purchaseOdds        = infoDocument["purchaseodds"] as BsonArray;
                    _totalFreeSpinWinRate   = (double) purchaseOdds[1];
                    _minFreeSpinWinRate     = (double) purchaseOdds[0];
                }

                if (this.SupportMoreBet)
                {
                    _anteBetMinusZeroCount = (int)((1.0 - 1.0 / MoreBetMultiple) * (double) _naturalSpinCount);
                    if (_anteBetMinusZeroCount > _emptySpinCount)
                        _logger.Error("More Bet Probabily calculation doesn't work in {0}", this.GameName);
                }

                if (this.SupportPurchaseFree && this.PurchaseFreeMultiple > _totalFreeSpinWinRate)
                    _logger.Error("freespin win rate doesn't satisfy condition {0}", this.GameName);
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
            }
        }

        #region 메세지처리함수들
        protected override async Task onProcMessage(string strUserID, int companyID, GITMessage message, UserBonus userBonus, double userBalance, Currencies currency)
        {
            switch((CSMSG_CODE) message.MsgCode)
            {
                case CSMSG_CODE.CS_AMATIC_DOINIT:
                    onDoInit(strUserID, message, userBalance, currency);
                    break;
                case CSMSG_CODE.CS_AMATIC_DOSPIN:
                    await onDoSpin(strUserID, companyID, message, userBonus, userBalance, currency);
                    break;
                case CSMSG_CODE.CS_AMATIC_DOCOLLECT:
                    onCollectSpin(strUserID, message, userBalance, currency);
                    break;
                case CSMSG_CODE.CS_AMATIC_DOHEARTBEAT:
                    onDoHeartBeat(strUserID, message, userBalance, currency);
                    break;
                case CSMSG_CODE.CS_AMATIC_DOGAMBLEPICK:
                    onGamblePick(strUserID, companyID, message, userBalance);
                    break;
                case CSMSG_CODE.CS_AMATIC_DOGAMBLEHALF:
                    onGambleHalf(strUserID, userBalance);
                    break;
            }
        }
        protected virtual void onDoInit(string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                string strResponse          = buildInitString(strUserID, userBalance, currency);

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_AMATIC_DOINIT);
                responseMessage.Append(strResponse);

                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::onDoInit GameID: {0}, {1}", _gameID, ex);
            }
        }
        protected virtual void onDoHeartBeat(string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                BaseAmaticSlotBetInfo betInfo = new BaseAmaticSlotBetInfo();
                betInfo.CurrencyInfo = currency;
                string strResponse = buildResMsgString(strUserID, userBalance, 0, betInfo, "", AmaticMessageType.HeartBeat);

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_AMATIC_DOHEARTBEAT);
                responseMessage.Append(strResponse);

                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::onDoHeartBeat GameID: {0}, {1}", _gameID, ex);
            }
        }
        protected virtual async Task onDoSpin(string strUserID, int companyID, GITMessage message, UserBonus userBonus, double userBalance, Currencies currency)
        {
            try
            {
                _isRewardedBonus        = false;
                _bonusSendMessage       = null;
                _rewardedBonusMoney     = 0.0;

                if (message.MsgCode == (ushort)CSMSG_CODE.CS_AMATIC_DOSPIN)
                    readBetInfoFromMessage(message, strUserID, currency);

                await spinGame(strUserID, companyID, userBonus, userBalance);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::onDoSpin GameID: {0}, {1}", _gameID, ex);
            }
        }
        protected void onCollectSpin(string strUserID, GITMessage message, double balance, Currencies currency)
        {
            BaseAmaticSlotSpinResult result = new BaseAmaticSlotSpinResult() { Action = AmaticMessageType.Collect, TotalWin = 0, ResultString = "" };
            if (_dicUserResultInfos.ContainsKey(strUserID))
            {
                result = _dicUserResultInfos[strUserID];
                balance += result.TotalWin;
            }

            string strResponse = buildResMsgString(strUserID, balance, 0, new BaseAmaticSlotBetInfo() { CurrencyInfo = currency }, "", AmaticMessageType.Collect);
            GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_AMATIC_DOCOLLECT);
            responseMessage.Append(strResponse);

            if (_dicUserResultInfos.ContainsKey(strUserID))
            {
                result = _dicUserResultInfos[strUserID];
                if(result.TotalWin > 0.0)
                {
                    ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, responseMessage, 0.0, result.TotalWin, new GameLogInfo(GameName, "0", "Collect"));
                    Sender.Tell(toUserResult);
                    
                    result.TotalWin = 0.0;
                    return;
                }
            }

            _dicUserResultInfos[strUserID]              = result;
            _dicUserLastBackupResultInfos[strUserID]    = result;

            Sender.Tell(new ToUserMessage((ushort)_gameID, responseMessage));
        }
        protected void onGamblePick(string strUserID, int companyID, GITMessage message, double balance)
        {
            int gambletype = (int)message.Pop();
            try
            {
                if (_dicUserBetInfos.ContainsKey(strUserID) && _dicUserResultInfos.ContainsKey(strUserID))
                {
                    BaseAmaticSlotBetInfo betInfo       = _dicUserBetInfos[strUserID];
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strUserID];
                    if (betInfo.HasRemainResponse)
                    {
                        _logger.Error("Can't gamble until spin finish in BaseAmaticSlotGame::onGamblePick");
                        return;
                    }
                    if(spinResult.WinMoney <= 0)
                    {
                        _logger.Error("Can't gamble if spinmoney is 0 in BaseAmaticSlotGame::onGamblePick");
                        return;
                    }

                    betInfo.GambleHalf  = false;
                    betInfo.GambleType  = gambletype;
                    if (betInfo.GambleType == 0)
                    {
                        _logger.Error("Gamble index is failed in BaseAmaticSlotGame::onGamblePick");
                        return;
                    }

                    gamblePickGame(strUserID, companyID, balance);
                }
                else
                {
                    _logger.Error("Can't gamble without betInfo and spinResult in BaseAmaticSlotGame::onGamblePick");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::onGamblePick {0}", ex);
            }
        }
        protected void onGambleHalf(string strUserID, double balance)
        {
            try
            {
                if (_dicUserBetInfos.ContainsKey(strUserID) && _dicUserResultInfos.ContainsKey(strUserID))
                {
                    BaseAmaticSlotBetInfo betInfo       = _dicUserBetInfos[strUserID];
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strUserID];
                    if (betInfo.HasRemainResponse)
                    {
                        _logger.Error("Can't half until spin finish in BaseAmaticSlotGame::onGambleHalf");
                        return;
                    }
                    
                    betInfo.GambleHalf = true;
                    betInfo.GambleType = 0;

                    gambleHalfGame(strUserID, balance);
                }
                else
                {
                    _logger.Error("Can't gamble without betInfo and spinResult in BaseAmaticSlotGame::onGambleHalf");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::onGambleHalf {0}", ex);
            }
        }
        protected void onDoUndoUserSpin(string strUserID)
        {
            undoUserResultInfo(strUserID);
            undoUserBetInfo(strUserID);
        }
        protected virtual void undoUserResultInfo(string strUserID)
        {
            try
            {
                if (!_dicUserLastBackupResultInfos.ContainsKey(strUserID))
                    return;

                BaseAmaticSlotSpinResult lastResult = _dicUserLastBackupResultInfos[strUserID];
                if (lastResult == null)
                    _dicUserResultInfos.Remove(strUserID);
                else
                    _dicUserResultInfos[strUserID] = lastResult;
                _dicUserLastBackupResultInfos.Remove(strUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::undoUserResultInfo {0}", ex);
            }
        }
        protected virtual void undoUserBetInfo(string strUserID)
        {
            try
            {
                if (!_dicUserLastBackupBetInfos.ContainsKey(strUserID))
                    return;

                byte[] userBetInfoBytes = _dicUserLastBackupBetInfos[strUserID];
                using (MemoryStream ms = new MemoryStream(userBetInfoBytes))
                {
                    using (BinaryReader binaryReader = new BinaryReader(ms))
                    {
                        BaseAmaticSlotBetInfo betInfo   = restoreBetInfo(strUserID, binaryReader);
                        _dicUserBetInfos[strUserID] = betInfo;
                    }
                }
                _dicUserLastBackupBetInfos.Remove(strUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::undoUserBetInfo {0}", ex);
            }
        }
        #endregion
        
        protected virtual void readBetInfoFromMessage(GITMessage message, string strUserID, Currencies currency)
        {
            try
            {
                BaseAmaticSlotBetInfo betInfo   = new BaseAmaticSlotBetInfo();
                betInfo.PlayLine            = (int)message.Pop();
                betInfo.PlayBet             = (int)message.Pop();
                betInfo.PurchaseStep        = (int)message.Pop();
                betInfo.MoreBet             = (int)message.Pop();
                betInfo.CurrencyInfo        = currency;
                betInfo.GambleType          = 0;
                betInfo.GambleHalf          = false;

                if (BettingButton[betInfo.PlayBet] * betInfo.RelativeTotalBet <= 0)
                {
                    _logger.Error("{0} betInfo 0 or infinite in BaseAmaticSlotGame::readBetInfoFromMessage", strUserID);
                    return;
                }

                BaseAmaticSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strUserID, out oldBetInfo))
                {
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.PlayLine     = betInfo.PlayLine;
                    oldBetInfo.PlayBet      = betInfo.PlayBet;
                    oldBetInfo.PurchaseStep = betInfo.PurchaseStep;
                    oldBetInfo.MoreBet      = betInfo.MoreBet;
                    oldBetInfo.CurrencyInfo = betInfo.CurrencyInfo;
                    oldBetInfo.GambleType   = betInfo.GambleType;
                    oldBetInfo.GambleHalf   = betInfo.GambleHalf;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::readBetInfoFromMessage {0}", ex);
            }
        }
        protected AmaticMessageType convertRespMsgCodeToAction(long actionType)
        {
            return (AmaticMessageType)actionType;
        }
        protected virtual BaseAmaticSlotSpinResult calculateResult(BaseAmaticSlotBetInfo betInfo,string strUserID, string strSpinResponse, bool isFirst, double userBalance, double betMoney)
        {
            try
            {
                BaseAmaticSlotSpinResult spinResult = new BaseAmaticSlotSpinResult();
                AmaticPacket packet                 = new AmaticPacket(strSpinResponse, Cols, FreeCols);
                double pointUnit                    = getPointUnit(betInfo);

                AmaticMessageType currentAction = convertRespMsgCodeToAction((int) packet.messagetype);
                if(currentAction == AmaticMessageType.NormalSpin || currentAction == AmaticMessageType.LastFree ||
                     currentAction == AmaticMessageType.LastWheel || currentAction == AmaticMessageType.LastRespin ||
                     currentAction == AmaticMessageType.LastPower ||
                    currentAction == AmaticMessageType.GamblePick || currentAction == AmaticMessageType.GambleHalf)
                {
                    spinResult.TotalWin = Math.Round((double)packet.win * BettingButton[betInfo.PlayBet] / _spinDataDefaultBet * pointUnit, 2);
                }

                spinResult.ResultString = buildResMsgString(strUserID, userBalance,betMoney, betInfo, strSpinResponse, currentAction);
                spinResult.Action       = currentAction;
                return spinResult;
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::calculateResult {0}", ex);
                return null;
            }
        }
        protected virtual List<BaseAmaticActionToResponse> buildResponseList(List<string> responseList)
        {
            List<BaseAmaticActionToResponse> actionResponseList = new List<BaseAmaticActionToResponse>();
            for (int i = 1; i < responseList.Count; i++)
            {
                AmaticPacket packet = new AmaticPacket(responseList[i], Cols, FreeCols);
                AmaticMessageType action    = convertRespMsgCodeToAction(packet.messagetype);
                
                actionResponseList.Add(new BaseAmaticActionToResponse(action, responseList[i]));
            }
            return actionResponseList;
        }
        protected virtual double getPurchaseMultiple(BaseAmaticSlotBetInfo betInfo)
        {
            return this.PurchaseFreeMultiple;
        }
        protected virtual double getMoreBetMultiple(BaseAmaticSlotBetInfo betInfo)
        {
            return this.MoreBetMultiple;
        }
        protected virtual double getPointUnit(BaseAmaticSlotBetInfo betInfo)
        {
            double pointUnit = new DicCurrencyInfo()._currencyInfo[(int)betInfo.CurrencyInfo].Rate / 100.0;
            return pointUnit;
        }
        protected virtual async Task<BaseAmaticSlotSpinResult> generateSpinResult(BaseAmaticSlotBetInfo betInfo, string strUserID, int companyID, double userBalance, double betMoney, UserBonus userBonus, bool usePayLimit)
        {
            BasePPSlotSpinData          spinData    = null;
            BaseAmaticSlotSpinResult    result      = null;

            if (betInfo.HasRemainResponse)
            {
                BaseAmaticActionToResponse nextResponse = betInfo.pullRemainResponse();
                result = calculateResult(betInfo, strUserID, nextResponse.Response, false, userBalance, betMoney);

                //프리게임이 끝났는지를 검사한다.
                if (!betInfo.HasRemainResponse)
                    betInfo.RemainReponses = null;
                return result;
            }

            //유저의 총 베팅액을 얻는다.
            double pointUnit        = getPointUnit(betInfo);
            double totalBet         = betInfo.RelativeTotalBet * BettingButton[betInfo.PlayBet] * pointUnit;
            double  realBetMoney    = totalBet;

            if (SupportPurchaseFree && betInfo.isPurchase)
                realBetMoney = totalBet * getPurchaseMultiple(betInfo);

            if (SupportMoreBet && betInfo.isMoreBet)
                realBetMoney = totalBet * getMoreBetMultiple(betInfo);

            spinData = await selectRandomStop(companyID, userBonus, totalBet, false, betInfo);

            //첫자료를 가지고 결과를 계산한다.
            double totalWin = totalBet * spinData.SpinOdd;
            
            if (!usePayLimit || spinData.IsEvent || await checkWebsitePayoutRate(companyID, realBetMoney, totalWin))
            {
                do
                {
                    if (spinData.IsEvent)
                    {
                        //bool checkRet = await subtractEventMoney(companyID, strUserID, totalWin);
                        //if (!checkRet)
                        //    break;

                        _bonusSendMessage   = null;
                        _rewardedBonusMoney = totalWin;
                        _isRewardedBonus    = true;
                    }
                    result = calculateResult(betInfo, strUserID, spinData.SpinStrings[0], true, userBalance, betMoney);
                    if (spinData.SpinStrings.Count > 1)
                        betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
                    return result;
                } while (false);
            }

            double emptyWin = 0.0;

            if (SupportPurchaseFree && betInfo.isPurchase)
            {
                spinData    = await selectMinStartFreeSpinData(betInfo);
                result      = calculateResult(betInfo, strUserID, spinData.SpinStrings[0], true, userBalance, betMoney);
                emptyWin    = totalBet * spinData.SpinOdd;

                //뒤에 응답자료가 또 있다면
                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
            }
            else
            {
                spinData    = await selectEmptySpin(betInfo);
                result      = calculateResult(betInfo, strUserID, spinData.SpinStrings[0], true, userBalance, betMoney);
            }

            sumUpWebsiteBetWin(companyID, realBetMoney, emptyWin);
            return result;
        }
        protected byte[] backupBetInfo(BaseAmaticSlotBetInfo betInfo)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    betInfo.SerializeTo(writer);
                }
                return ms.ToArray();
            }
        }
        protected virtual async Task spinGame(string strUserID, int companyID, UserBonus userBonus, double userBalance)
        {
            try
            {
                //해당 유저의 베팅정보를 얻는다. 만일 베팅정보가 없다면(례외상황) 그대로 리턴한다.
                BaseAmaticSlotBetInfo betInfo = null;
                if (!_dicUserBetInfos.TryGetValue(strUserID, out betInfo))
                    return;

                byte[] betInfoBytes = backupBetInfo(betInfo);

                BaseAmaticSlotSpinResult lastResult = null;
                if (_dicUserResultInfos.ContainsKey(strUserID))
                    lastResult = _dicUserResultInfos[strUserID];

                double pointUnit    = getPointUnit(betInfo);
                double betMoney     = betInfo.RelativeTotalBet * BettingButton[betInfo.PlayBet] * pointUnit;

                UserBetTypes betType = UserBetTypes.Normal;
                //프리스핀구입
                if (this.SupportPurchaseFree && betInfo.isPurchase)
                {
                    betMoney = Math.Round((double)getPurchaseMultiple(betInfo) * betMoney, 2);
                    if (betMoney > 0)
                        betType = UserBetTypes.PurchaseFree;
                }
                else if (this.SupportMoreBet && betInfo.isMoreBet)
                {
                    betMoney = Math.Round((double)getMoreBetMultiple(betInfo) * betMoney, 2);
                    if (betMoney > 0)
                        betType = UserBetTypes.AnteBet;
                }

                if (betInfo.HasRemainResponse || betInfo.GambleType > 0 || betInfo.GambleHalf)
                    betMoney = 0.0;

                //만일 베팅머니가 유저의 밸런스보다 크다면 끝낸다.
                if (userBalance.LT(betMoney, _epsilion) || betMoney < 0.0)
                {
                    _logger.Error("user balance is less than bet money in BaseAmaticSlotGame::spinGame {0} balance:{1}, bet money: {2} game id:{3}",
                        strUserID, userBalance, betMoney, _gameID);
                    return;
                }

                //결과를 생성한다.
                BaseAmaticSlotSpinResult spinResult = await generateSpinResult(betInfo, strUserID, companyID, userBalance, betMoney, userBonus, true);

                //게임로그
                string strGameLog               = spinResult.ResultString;
                _dicUserResultInfos[strUserID]  = spinResult;

                //생성된 게임결과를 유저에게 보낸다.(윈은 Collect요청시에 처리한다.)
                sendGameResult(betInfo, spinResult, strUserID, betMoney, 0.0, strGameLog, userBalance,betType);

                _dicUserLastBackupBetInfos[strUserID]       = betInfoBytes;
                _dicUserLastBackupResultInfos[strUserID]    = lastResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::spinGame {0}", ex);
            }
        }
        protected virtual async void gamblePickGame(string strUserID, int companyID,double userbalance)
        {
            BaseAmaticSlotBetInfo betInfo       = _dicUserBetInfos[strUserID];
            BaseAmaticSlotSpinResult lastResult = _dicUserResultInfos[strUserID];
            string strSpinResponse = lastResult.ResultString;
            AmaticPacket packet = new AmaticPacket(strSpinResponse, Cols, FreeCols);

            double pointUnit = getPointUnit(betInfo);
            int cardSymbol = Pcg.Default.Next(0, 4);
            int cardNumber = Pcg.Default.Next(0, 13);
            
            if(!await checkWebsitePayoutRate(companyID, 0.0, lastResult.TotalWin))
            {
                if(betInfo.GambleType == 1)
                {
                    if(cardSymbol == 0 || cardSymbol == 1)
                    {
                        List<int> noRedSymbol = new List<int>() { 2, 3 };
                        cardSymbol =  noRedSymbol[Pcg.Default.Next(0, 2)];
                    }
                }
                else if(betInfo.GambleType == 2)
                {
                    if (cardSymbol == 2 || cardSymbol == 3)
                    {
                        List<int> noBlackSymbol = new List<int>() { 0, 1 };
                        cardSymbol = noBlackSymbol[Pcg.Default.Next(0, 2)];
                    }
                }
                else if (betInfo.GambleType == 3)
                {
                    if (cardSymbol == 0)
                    {
                        List<int> noDiamondSymbol = new List<int>() { 1, 2, 3 };
                        cardSymbol = noDiamondSymbol[Pcg.Default.Next(0, 3)];
                    }
                }
                else if (betInfo.GambleType == 4)
                {
                    if (cardSymbol == 1)
                    {
                        List<int> noHeartSymbol = new List<int>() { 0, 2, 3 };
                        cardSymbol = noHeartSymbol[Pcg.Default.Next(0, 3)];
                    }
                }
                else if (betInfo.GambleType == 5)
                {
                    if (cardSymbol == 2)
                    {
                        List<int> noCrabSymbol = new List<int>() { 0, 1, 3 };
                        cardSymbol = noCrabSymbol[Pcg.Default.Next(0, 3)];
                    }
                }
                else if (betInfo.GambleType == 6)
                {
                    if (cardSymbol == 3)
                    {
                        List<int> noSpadeSymbol = new List<int>() { 0, 1, 2 };
                        cardSymbol = noSpadeSymbol[Pcg.Default.Next(0, 3)];
                    }
                }
            }

            int winMultiple = 0;
            if(betInfo.GambleType == 1 && (cardSymbol == 0 || cardSymbol == 1))
                winMultiple = 2;
            else if (betInfo.GambleType == 2 && (cardSymbol == 2 || cardSymbol == 3))
                winMultiple = 2;
            else if (betInfo.GambleType == 3 && cardSymbol == 0)
                winMultiple = 4;
            else if (betInfo.GambleType == 4 && cardSymbol == 1)
                winMultiple = 4;
            else if (betInfo.GambleType == 5 && cardSymbol == 2)
                winMultiple = 4;
            else if (betInfo.GambleType == 6 && cardSymbol == 3)
                winMultiple = 4;

            int gambleNumber = cardNumber * 4 + cardSymbol;
            for(int i = packet.gamblelogs.Count; i > 1; i--)
                packet.gamblelogs[i - 1] = packet.gamblelogs[i - 2];
            packet.gamblelogs[0] = gambleNumber;
            packet.win           = (long)(packet.win * winMultiple);
            packet.messagetype   = (long)AmaticMessageType.GamblePick;
            
            strSpinResponse = buildSpinString(packet);

            lastResult.ResultString = strSpinResponse;
            lastResult.TotalWin     = Math.Round((double)packet.win * pointUnit, 2);
            lastResult.Action       = AmaticMessageType.GamblePick;

            _dicUserResultInfos[strUserID]              = lastResult;
            sendGamblePickResult(lastResult, strUserID, 0.0, 0.0, "Gamble", userbalance);
            _dicUserLastBackupResultInfos[strUserID]    = lastResult;
        }
        protected virtual void gambleHalfGame(string strUserID, double userbalance)
        {
            BaseAmaticSlotBetInfo betInfo       = _dicUserBetInfos[strUserID];
            BaseAmaticSlotSpinResult lastResult = _dicUserResultInfos[strUserID];
            string strSpinResponse = lastResult.ResultString;
            AmaticPacket packet = new AmaticPacket(strSpinResponse, Cols, FreeCols);

            long leftMoney = 0;
            if(packet.win > 1)
                leftMoney = packet.win / 2;

            double pointUnit    = getPointUnit(betInfo);
            double winMoney     = Math.Round((double)leftMoney * pointUnit, 2);
            userbalance += winMoney;

            packet.balance      = (long)Math.Round(userbalance / pointUnit);
            packet.win          = (long)(packet.win - leftMoney);
            packet.messagetype  = (long)AmaticMessageType.GambleHalf;
            
            strSpinResponse = buildSpinString(packet);

            lastResult.ResultString = strSpinResponse;
            lastResult.TotalWin     = Math.Round((double)packet.win * pointUnit, 2);
            lastResult.Action       = AmaticMessageType.GambleHalf;

            _dicUserResultInfos[strUserID]              = lastResult;
            sendGambleHalfResult(lastResult, strUserID, 0.0, winMoney, "Half", userbalance);
            _dicUserLastBackupResultInfos[strUserID]    = lastResult;
        }
        protected virtual void sendGameResult(BaseAmaticSlotBetInfo betInfo, BaseAmaticSlotSpinResult spinResult, string strUserID, double betMoney, double winMoney, string strGameLog, double userBalance, UserBetTypes betType)
        {
            GITMessage message = new GITMessage((ushort) SCMSG_CODE.SC_AMATIC_DOSPIN);
            message.Append(spinResult.ResultString);

            ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, message, betMoney, winMoney, new GameLogInfo(GameName, "0", strGameLog));

            if (_isRewardedBonus)
            {
                toUserResult.setBonusReward(_rewardedBonusMoney);
                toUserResult.insertFirstMessage(_bonusSendMessage);
            }
            
            Sender.Tell(toUserResult, Self);
        }
        protected virtual void sendGamblePickResult(BaseAmaticSlotSpinResult spinResult, string strUserID,double betMoney,double winMoney, string strGameLog, double userBalance)
        {
            GITMessage message = new GITMessage((ushort)SCMSG_CODE.SC_AMATIC_DOGAMBLEPICK);
            message.Append(spinResult.ResultString);

            ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, message, betMoney, winMoney, new GameLogInfo(GameName, "0", strGameLog));
            Sender.Tell(toUserResult, Self);
        }
        protected virtual void sendGambleHalfResult(BaseAmaticSlotSpinResult spinResult, string strUserID, double betMoney, double winMoney, string strGameLog, double userBalance)
        {
            GITMessage message = new GITMessage((ushort)SCMSG_CODE.SC_AMATIC_DOGAMBLEHALF);
            message.Append(spinResult.ResultString);

            ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, message, betMoney, winMoney, new GameLogInfo(GameName, "0", strGameLog));
            Sender.Tell(toUserResult, Self);
        }

        #region 스핀자료처리부분
        protected OddAndIDData selectOddAndIDFromProbsWithRange(SortedDictionary<double, int[]> oddProbs, int totalCount, double minOdd, double maxOdd)
        {
            int random  = Pcg.Default.Next(0, totalCount);
            int sum     = 0;
            foreach (KeyValuePair<double, int[]> pair in oddProbs)
            {
                if (pair.Key < minOdd)
                    continue;

                if (pair.Key > maxOdd)
                    break;

                sum += pair.Value.Length;
                if (random < sum)
                {
                    OddAndIDData ret = new OddAndIDData();
                    ret.ID  = pair.Value[Pcg.Default.Next(0, pair.Value.Length)];
                    ret.Odd = pair.Key;
                    return ret;
                }
            }
            return null;
        }
        protected OddAndIDData selectOddAndIDFromProbsWithRange(SortedDictionary<double, int[]> oddProbs, double minOdd, double maxOdd)
        {
            int totalCount = 0;
            foreach (KeyValuePair<double, int[]> pair in oddProbs)
            {
                if (pair.Key < minOdd)
                    continue;
                if (pair.Key > maxOdd)
                    break;
                totalCount += pair.Value.Length;
            }

            if (totalCount == 0)
                return null;

            int random = Pcg.Default.Next(0, totalCount);
            int sum = 0;
            foreach (KeyValuePair<double, int[]> pair in oddProbs)
            {
                if (pair.Key < minOdd)
                    continue;

                if (pair.Key > maxOdd)
                    break;

                sum += pair.Value.Length;
                if (random < sum)
                {
                    OddAndIDData ret = new OddAndIDData();
                    ret.ID = pair.Value[Pcg.Default.Next(0, pair.Value.Length)];
                    ret.Odd = pair.Key;
                    return ret;
                }
            }
            return null;
        }
        protected OddAndIDData selectOddAndIDFromProbs(SortedDictionary<double, int[]> oddProbs, int totalCount)
        {
            int random = Pcg.Default.Next(0, totalCount);
            int sum = 0;
            foreach (KeyValuePair<double, int[]> pair in oddProbs)
            {
                sum += pair.Value.Length;
                if (random < sum)
                {
                    OddAndIDData ret = new OddAndIDData();
                    ret.ID  = pair.Value[Pcg.Default.Next(0, pair.Value.Length)];
                    ret.Odd = pair.Key;
                    return ret;
                }
            }
            return null;
        }
        protected double selectOddFromProbs(SortedDictionary<double, int> oddProbs, int totalCount)
        {
            int random = Pcg.Default.Next(0, totalCount);
            int sum    = 0;
            foreach(KeyValuePair<double, int> pair in oddProbs)
            {
                sum += pair.Value;
                if (random < sum)
                    return pair.Key;
            }
            return oddProbs.First().Key;
        }
        protected virtual async Task<BasePPSlotSpinData> selectRandomStop(int companyID, BaseAmaticSlotBetInfo betInfo)
        {
            OddAndIDData selectedOddAndID = selectRandomOddAndID(companyID, betInfo);
            
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, selectedOddAndID.ID), TimeSpan.FromSeconds(10.0));
            return convertBsonToSpinData(spinDataDocument);
        }
        protected virtual OddAndIDData selectRandomOddAndID(int websiteID, BaseAmaticSlotBetInfo betInfo)
        {
            double payoutRate   = getPayoutRate(websiteID);
            double randomDouble = Pcg.Default.NextDouble(0.0, 100.0);

            int selectedID = 0;
            if (randomDouble >= payoutRate || payoutRate == 0.0)
            {
                selectedID = Pcg.Default.Next(1, _emptySpinCount + 1);
            }
            else if (SupportMoreBet && betInfo.isMoreBet)
            {
                selectedID = _anteBetMinusZeroCount + Pcg.Default.Next(1, _naturalSpinCount - _anteBetMinusZeroCount + 1);
            }
            else
            {
                selectedID = Pcg.Default.Next(1, _naturalSpinCount + 1);
            }

            OddAndIDData selectedOddAndID = new OddAndIDData();
            selectedOddAndID.ID     = selectedID;
            return selectedOddAndID;
        }
        protected virtual async Task<BasePPSlotSpinData> selectEmptySpin(BaseAmaticSlotBetInfo betInfo)
        {
            int id = Pcg.Default.Next(1, _emptySpinCount + 1);
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, id), TimeSpan.FromSeconds(10.0));
            return convertBsonToSpinData(spinDataDocument);
        }
        protected virtual BasePPSlotSpinData convertBsonToSpinData(BsonDocument document)
        {
            int spinType    = (int)document["spintype"];
            double spinOdd  = (double)document["odd"];
            string strData  = (string)document["data"];

            List<string> spinResponses = new List<string>(strData.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));

            return new BasePPSlotSpinData(spinType, spinOdd, spinResponses);
        }
        protected virtual async Task<BasePPSlotSpinData> selectRandomStartFreeSpinData(BaseAmaticSlotBetInfo betInfo)
        {
            try
            {
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectPurchaseSpinRequest(GameName, HasPurEnableOption ? StartSpinSearchTypes.SPECIFIC : StartSpinSearchTypes.GENERAL),
                        TimeSpan.FromSeconds(10.0));

                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::selectRandomStartFreeSpinData {0}", ex);
                return null;
            }
        }
        protected virtual async Task<BasePPSlotSpinData> selectMinStartFreeSpinData(BaseAmaticSlotBetInfo betInfo)
        {
            try
            {
                BsonDocument spinDataDocument = null;
                if (HasPurEnableOption)
                {
                    spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                            new SelectSpinTypeOddRangeRequest(GameName, -1, PurchaseFreeMultiple * 0.2, PurchaseFreeMultiple * 0.5, 0), TimeSpan.FromSeconds(10.0));
                }
                else
                {
                    spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                            new SelectSpinTypeOddRangeRequest(GameName, 1, PurchaseFreeMultiple * 0.2, PurchaseFreeMultiple * 0.5), TimeSpan.FromSeconds(10.0));
                }
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BasePPSlotGame::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }
        #endregion
        protected virtual async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int websiteID, BaseAmaticSlotBetInfo betInfo, double baseBet, UserBonus userBonus)
        {
            if (userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus rangeOddBonus = userBonus as UserRangeOddEventBonus;
                if (baseBet.LE(rangeOddBonus.MaxBet, _epsilion))
                {
                    BasePPSlotSpinData spinDataEvent = await selectRangeFreeSpinData(websiteID, rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd, betInfo);
                    if (spinDataEvent != null)
                    {
                        spinDataEvent.IsEvent = true;
                        return spinDataEvent;
                    }
                }
            }

            double payoutRate = _config.PayoutRate;
            if (_websitePayoutRates.ContainsKey(websiteID))
                payoutRate = _websitePayoutRates[websiteID];

            double targetC = getPurchaseMultiple(betInfo) * payoutRate / 100.0;
            if (targetC >= _totalFreeSpinWinRate)
                targetC = _totalFreeSpinWinRate;

            if (targetC < _minFreeSpinWinRate)
                targetC = _minFreeSpinWinRate;

            double x = (_totalFreeSpinWinRate - targetC) / (_totalFreeSpinWinRate - _minFreeSpinWinRate);

            BasePPSlotSpinData spinData = null;
            if (Pcg.Default.NextDouble(0.0, 1.0) <= x)
                spinData = await selectMinStartFreeSpinData(betInfo);
            else
                spinData = await selectRandomStartFreeSpinData(betInfo);
            return spinData;
        }
        protected virtual async Task<BasePPSlotSpinData> selectRangeSpinData(int websiteID, double minOdd, double maxOdd, BaseAmaticSlotBetInfo betInfo)
        {
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                    new SelectSpinTypeOddRangeRequest(GameName, -1, minOdd, maxOdd), TimeSpan.FromSeconds(10.0));

            if (spinDataDocument == null)
                return null;

            return convertBsonToSpinData(spinDataDocument);
        }
        protected virtual async Task<BasePPSlotSpinData> selectRangeFreeSpinData(int websiteID, double minOdd, double maxOdd, BaseAmaticSlotBetInfo betInfo)
        {
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                    new SelectSpinTypeOddRangeRequest(GameName, 1, minOdd, maxOdd), TimeSpan.FromSeconds(10.0));

            if (spinDataDocument == null)
                return null;

            return convertBsonToSpinData(spinDataDocument);
        }
        public virtual async Task<BasePPSlotSpinData> selectRandomStop(int websiteID, UserBonus userBonus, double baseBet, bool isChangedLineCount, BaseAmaticSlotBetInfo betInfo)
        {
            //프리스핀구입을 먼저 처리한다.
            if (SupportPurchaseFree && betInfo.isPurchase)
                return await selectPurchaseFreeSpin(websiteID, betInfo, baseBet, userBonus);

            if(userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus  rangeOddBonus = userBonus as UserRangeOddEventBonus;
                if (baseBet.LE(rangeOddBonus.MaxBet, _epsilion))
                {
                    BasePPSlotSpinData spinDataEvent = await selectRangeSpinData(websiteID, rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd, betInfo);
                    if (spinDataEvent != null)
                    {
                        spinDataEvent.IsEvent = true;
                        return spinDataEvent;
                    }
                }
            }

            if (SupportMoreBet && betInfo.isMoreBet)
                return await selectRandomStop(websiteID, betInfo);
            else
                return await selectRandomStop(websiteID, betInfo);
        }
        protected override async Task<bool> loadUserHistoricalData(string strUserID, bool isNewEnter)
        {
            try
            {
                string strKey = string.Format("{0}_{1}", strUserID, _gameID);
                byte[] betInfoData = await RedisDatabase.RedisCache.StringGetAsync(strKey);
                if (betInfoData != null)
                {
                    using (var stream = new MemoryStream(betInfoData))
                    {
                        BinaryReader reader = new BinaryReader(stream);
                        BaseAmaticSlotBetInfo betInfo = restoreBetInfo(strUserID, reader);
                        if (betInfo != null)
                            _dicUserBetInfos[strUserID] = betInfo;
                    }
                }

                strKey = string.Format("{0}_{1}_result", strUserID, _gameID);
                byte[] resultInfoData = await RedisDatabase.RedisCache.StringGetAsync(strKey);
                if (resultInfoData != null)
                {
                    using (var stream = new MemoryStream(resultInfoData))
                    {
                        BinaryReader reader = new BinaryReader(stream);
                        BaseAmaticSlotSpinResult resultInfo = restoreResultInfo(strUserID, reader);
                        if (resultInfo != null)
                            _dicUserResultInfos[strUserID] = resultInfo;
                    }
                }
            }

            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::loadUserHistoricalData {0}", ex);
                return false;
            }
            return await base.loadUserHistoricalData(strUserID, isNewEnter);
        }
        protected virtual BaseAmaticSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            BaseAmaticSlotBetInfo betInfo = new BaseAmaticSlotBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected virtual BaseAmaticSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            BaseAmaticSlotSpinResult result = new BaseAmaticSlotSpinResult();
            result.SerializeFrom(reader);
            return result;
        }
        protected override async Task onExitUserMessage(ExitGameRequest message)
        {
            try
            {
                double winMoney = await procUserExitGame(message.UserID, message.Balance, message.Currency, message.UserRequested);
                _dicEnteredUsers.Remove(message.UserID);
                if (winMoney == 0.0)
                {
                    Sender.Tell(new AmaticExitResponse(null));
                    return;
                }

                ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, null, 0.0, winMoney, new GameLogInfo(GameName, "0", "Collect"));
                Sender.Tell(new AmaticExitResponse(toUserResult));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::onExitUserMessage {0}", ex);
            }
        }
        protected async Task<double> procUserExitGame(string strUserID,double userbalance, Currencies currency, bool userRequested)
        {
            try
            {
                double winMoney = 0.0;

                if (_dicUserResultInfos.ContainsKey(strUserID) && _dicUserResultInfos[strUserID].Action != AmaticMessageType.Collect)
                {
                    BaseAmaticSlotSpinResult result = _dicUserResultInfos[strUserID];
                    winMoney = result.TotalWin;

                    if(!_dicUserBetInfos.ContainsKey(strUserID) || !_dicUserBetInfos[strUserID].HasRemainResponse)
                    {
                        string strResponse = buildResMsgString(strUserID, userbalance, 0, new BaseAmaticSlotBetInfo() { CurrencyInfo = currency }, "", AmaticMessageType.Collect);
                        result.Action   = AmaticMessageType.Collect;
                        result.TotalWin = 0;
                    }

                    byte[] resultInfoBytes = _dicUserResultInfos[strUserID].convertToByte();
                    await _redisWriter.Ask(new UserResultInfoWrite(strUserID, _gameID, resultInfoBytes, true));
                    _dicUserResultInfos.Remove(strUserID);
                }

                if (_dicUserBetInfos.ContainsKey(strUserID))
                {
                    if (_dicUserBetInfos[strUserID].HasRemainResponse)
                    {
                        BaseAmaticSlotBetInfo betInfo = _dicUserBetInfos[strUserID];
                        if (betInfo.HasRemainResponse)
                            winMoney = 0.0;
                    }

                    byte[] betInfoBytes = _dicUserBetInfos[strUserID].convertToByte();
                    await _redisWriter.Ask(new UserBetInfoWrite(strUserID, _gameID, betInfoBytes, true));
                    _dicUserBetInfos.Remove(strUserID);
                }
                
                _dicUserLastBackupResultInfos.Remove(strUserID);
                _dicUserLastBackupBetInfos.Remove(strUserID);
                return winMoney;

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::onUserExitGame GameID:{0} {1}", _gameID, ex);
                return 0.0;
            }
        }
        protected virtual string buildInitString(string strUserID, double balance, Currencies currency)
        {
            AmaticEncrypt encrypt = new AmaticEncrypt();
            string initString = string.Empty;

            InitPacket initPacket = new InitPacket(InitString, Cols, FreeCols, ReelSetBitNum);
            initPacket.betstepamount    = BettingButton.ToList();
            initPacket.laststep         = 0;
            initPacket.minbet           = BettingButton[0];
            initPacket.maxbet           = BettingButton.ToList().Last() * LINES.ToList().Last();
            initPacket.lastline         = LINES.ToList().Last();

            if (_dicUserBetInfos.ContainsKey(strUserID) && _dicUserResultInfos.ContainsKey(strUserID))
            {
                BaseAmaticSlotBetInfo betInfo       = _dicUserBetInfos[strUserID];
                BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strUserID];

                initPacket.laststep = betInfo.PlayBet;
                initPacket.lastline = betInfo.PlayLine;
                initPacket.win      = 0;

                AmaticPacket amaPacket = new AmaticPacket(spinResult.ResultString, Cols, FreeCols);
                initPacket.messagetype      = amaPacket.messagetype;
                if ((AmaticMessageType)amaPacket.messagetype == AmaticMessageType.GamblePick || (AmaticMessageType)amaPacket.messagetype == AmaticMessageType.GambleHalf
                    || (AmaticMessageType)amaPacket.messagetype == AmaticMessageType.LastFree || (AmaticMessageType)amaPacket.messagetype == AmaticMessageType.LastRespin
                    || (AmaticMessageType)amaPacket.messagetype == AmaticMessageType.LastPower || (AmaticMessageType)amaPacket.messagetype == AmaticMessageType.LastWheel)
                    initPacket.messagetype = (int)AmaticMessageType.NormalSpin;
                initPacket.reelstops        = amaPacket.reelstops;
                initPacket.freereelstops    = amaPacket.freereelstops;
                initPacket.gamblelogs       = amaPacket.gamblelogs;
                if (betInfo.HasRemainResponse || amaPacket.messagetype == (long)AmaticMessageType.FreeOption)
                {
                    if(amaPacket.messagetype == (long)AmaticMessageType.FreeTrigger || amaPacket.messagetype == (long)AmaticMessageType.FreeSpin || amaPacket.messagetype == (long)AmaticMessageType.ExtendFree)
                        initPacket.messagetype = (long)AmaticMessageType.FreeReopen;

                    if (amaPacket.messagetype == (long)AmaticMessageType.WheelTrigger || amaPacket.messagetype == (long)AmaticMessageType.Wheel)
                        initPacket.messagetype = (long)AmaticMessageType.WheelTrigger;

                    if (amaPacket.messagetype == (long)AmaticMessageType.TriggerPower || amaPacket.messagetype == (long)AmaticMessageType.PowerRespin)
                        initPacket.messagetype = (long)AmaticMessageType.FreeSpin;

                    initPacket.totalfreecnt     = amaPacket.totalfreecnt;
                    initPacket.curfreecnt       = amaPacket.curfreecnt;
                    initPacket.curfreewin       = amaPacket.curfreewin;
                    initPacket.freeunparam1     = amaPacket.freeunparam1;
                    initPacket.freeunparam2     = amaPacket.freeunparam2;
                    initPacket.totalfreewin     = amaPacket.totalfreewin;

                    initPacket.linewins         = amaPacket.linewins;
                    initPacket.win              = amaPacket.win;
                }
            }

            initString = encrypt.WriteDecHex(initString, initPacket.messageheader);
            initString = encrypt.WriteDecHex(initString, initPacket.reelset.Count);
            for(int i = 0; i < initPacket.reelset.Count; i++)
            {
                if(ReelSetBitNum == 1)
                    initString = encrypt.Write1BitNumArray(initString, initPacket.reelset[i]);
                else if(ReelSetBitNum == 2)
                    initString = encrypt.Write2BitNumArray(initString, initPacket.reelset[i]);
            }
            
            initString = encrypt.WriteDecHex(initString, initPacket.freereelset.Count);
            for (int i = 0; i < initPacket.freereelset.Count; i++)
            {
                if(ReelSetBitNum == 1)
                    initString = encrypt.Write1BitNumArray(initString, initPacket.freereelset[i]);
                else if(ReelSetBitNum == 2)
                    initString = encrypt.Write2BitNumArray(initString, initPacket.freereelset[i]);
            }
            
            initString = encrypt.WriteDec2Hex(initString, initPacket.messagetype);
            initString = encrypt.WriteDecHex(initString, initPacket.sessionclose);

            int reelStopCnt = initPacket.reelstops.Count > 5 ? initPacket.reelstops.Count : 5;
            if(initPacket.reelstops.Count >= 5)
            {
                for (int i = 0; i < reelStopCnt; i++)
                    initString = encrypt.WriteLengthAndDec(initString, initPacket.reelstops[i]);
            }
            else
            {
                for (int i = 0; i < initPacket.reelstops.Count; i++)
                    initString = encrypt.WriteLengthAndDec(initString, initPacket.reelstops[i]);

                for (int i = initPacket.reelstops.Count; i < reelStopCnt; i++)
                    initString = encrypt.WriteLengthAndDec(initString, 0);
            }

            initString = encrypt.WriteLengthAndDec(initString, initPacket.messageid);

            double pointUnit = getPointUnit(new BaseAmaticSlotBetInfo() { CurrencyInfo = currency });
            long balanceUnit = (long)Math.Round(balance / pointUnit, 0);
            initString = encrypt.WriteLengthAndDec(initString, balanceUnit);        //현재 화페와 단위금액으로 변환된 발란스
            initString = encrypt.WriteLengthAndDec(initString, initPacket.win);     //당첨금(인이트의 경우에는 0)
            initString = encrypt.WriteDec2Hex(initString, initPacket.laststep);     //마지막스핀 스텝
            initString = encrypt.WriteLengthAndDec(initString, initPacket.minbet);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.maxbet);  
            initString = encrypt.WriteDec2Hex(initString, initPacket.lastline);     //마지막스핀 라인

            initString = encrypt.WriteLengthAndDec(initString, initPacket.totalfreecnt);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.curfreecnt);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.curfreewin);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.freeunparam1);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.freeunparam2);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.totalfreewin);

            initString = encrypt.WriteLengthAndDec(initString, initPacket.unknownparam1);
            initString = encrypt.WriteDec2Hex(initString, initPacket.minbetline);
            initString = encrypt.WriteDec2Hex(initString, initPacket.maxbetline);
            initString = encrypt.WriteDec2Hex(initString, initPacket.unitbetline);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.unknownparam2);
            initString = encrypt.WriteDec2Hex(initString, initPacket.unknownparam3);

            int freeReelStopCnt = initPacket.freereelstops.Count > 5 ? initPacket.freereelstops.Count : 5;
            if (initPacket.freereelstops.Count >= 5)
            {
                for (int i = 0; i < freeReelStopCnt; i++)
                    initString = encrypt.WriteLengthAndDec(initString, initPacket.freereelstops[i]);
            }
            else
            {
                for (int i = 0; i < initPacket.freereelstops.Count; i++)
                    initString = encrypt.WriteLengthAndDec(initString, initPacket.freereelstops[i]);

                for (int i = initPacket.freereelstops.Count; i < freeReelStopCnt; i++)
                    initString = encrypt.WriteLengthAndDec(initString, 0);
            }

            for (int i = 0; i < initPacket.gamblelogs.Count; i++)
            {
                initString = encrypt.WriteDec2Hex(initString, initPacket.gamblelogs[i]);
            }
            
            initString = encrypt.WriteDec2Hex(initString, initPacket.betstepamount.Count);
            for(int i = 0; i < initPacket.betstepamount.Count; i++)
            {
                initString = encrypt.WriteLengthAndDec(initString, initPacket.betstepamount[i]);
            }
            
            initString = encrypt.WriteDec2Hex(initString, initPacket.linewins.Count);
            for (int i = 0; i < initPacket.linewins.Count; i++)
            {
                initString = encrypt.WriteLengthAndDec(initString, initPacket.linewins[i]);
            }
            return initString;
        }
        protected virtual string buildResMsgString(string strUserID, double balance,double betMoney, BaseAmaticSlotBetInfo betInfo, string spinString, AmaticMessageType type)
        {
            AmaticPacket packet = null;
            double pointUnit = getPointUnit(betInfo);

            if (!string.IsNullOrEmpty(spinString))
            {
                packet = new AmaticPacket(spinString, Cols, FreeCols);
                
                packet.betstep = betInfo.PlayBet;
                packet.balance = (long)Math.Round(((balance - betMoney) / pointUnit));
                convertWinsByBet(balance, packet, betInfo);
            }
            else
            {
                packet = new AmaticPacket(Cols, FreeCols, (int)type, (int)LINES.Last());
                
                packet.balance  = (long)Math.Round(balance / pointUnit, 0);
                packet.betstep  = 0;
                packet.betline  = LINES.Last();

                if (_dicUserBetInfos.ContainsKey(strUserID) && _dicUserResultInfos.ContainsKey(strUserID))
                {
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strUserID];

                    AmaticPacket oldPacket = new AmaticPacket(spinResult.ResultString, Cols, FreeCols);
                    packet.betstep          = oldPacket.betstep;
                    packet.betline          = oldPacket.betline;
                    packet.reelstops        = oldPacket.reelstops;
                    packet.freereelstops    = oldPacket.freereelstops;

                    if (type == AmaticMessageType.HeartBeat)
                    {
                        int cnt = oldPacket.linewins.Count;
                        packet.linewins     = new List<long>();
                        for(int i = 0; i < cnt; i++)
                        {
                            packet.linewins.Add(0);
                        }
                    }
                    else if(type == AmaticMessageType.Collect)
                    {
                        packet.totalfreecnt = oldPacket.totalfreecnt;
                        packet.curfreecnt   = oldPacket.curfreecnt;
                        packet.curfreewin   = oldPacket.curfreewin;
                        packet.freeunparam1 = oldPacket.freeunparam1;
                        packet.freeunparam2 = oldPacket.freeunparam2;
                        packet.totalfreewin = oldPacket.totalfreewin;

                        int cnt = oldPacket.linewins.Count;
                        packet.linewins     = new List<long>();
                        for(int i = 0; i < cnt; i++)
                        {
                            packet.linewins.Add(0);
                        }
                    }
                }
            }

            return buildSpinString(packet);
        }
        protected virtual string buildSpinString(AmaticPacket packet)
        {
            AmaticEncrypt encrypt = new AmaticEncrypt();
            string newSpinString = string.Empty;

            newSpinString = encrypt.WriteDecHex(newSpinString, packet.messageheader);
            newSpinString = encrypt.WriteDec2Hex(newSpinString, packet.messagetype);
            newSpinString = encrypt.WriteDecHex(newSpinString, packet.sessionclose);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.messageid);

            newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.balance);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.win);

            int reelStopCnt = packet.reelstops.Count > 5 ? packet.reelstops.Count : 5;
            if (packet.reelstops.Count >= 5)
            {
                for (int i = 0; i < reelStopCnt; i++)
                    newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.reelstops[i]);
            }
            else
            {
                for (int i = 0; i < packet.reelstops.Count; i++)
                    newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.reelstops[i]);

                for (int i = packet.reelstops.Count; i < reelStopCnt; i++)
                    newSpinString = encrypt.WriteLengthAndDec(newSpinString, 0);
            }
            
            newSpinString = encrypt.WriteDec2Hex(newSpinString, packet.betstep);
            newSpinString = encrypt.WriteDec2Hex(newSpinString, packet.betline);

            newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.totalfreecnt);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.curfreecnt);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.curfreewin);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.freeunparam1);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.freeunparam2);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.totalfreewin);

            int freeReelStopCnt = packet.freereelstops.Count > 5 ? packet.freereelstops.Count : 5;
            if (packet.freereelstops.Count >= 5)
            {
                for (int i = 0; i < freeReelStopCnt; i++)
                    newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.freereelstops[i]);
            }
            else
            {
                for (int i = 0; i < packet.freereelstops.Count; i++)
                    newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.freereelstops[i]);

                for (int i = packet.freereelstops.Count; i < freeReelStopCnt; i++)
                    newSpinString = encrypt.WriteLengthAndDec(newSpinString, 0);
            }

            newSpinString = encrypt.WriteDec2Hex(newSpinString, packet.linewins.Count);
            for(int i = 0; i < packet.linewins.Count; i++)
            {
                newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.linewins[i]);
            }

            for (int i = 0; i < packet.gamblelogs.Count; i++)
            {
                newSpinString = encrypt.WriteDec2Hex(newSpinString, packet.gamblelogs[i]);
            }

            return newSpinString;
        }
        protected long convertWinByBet(double win, BaseAmaticSlotBetInfo betInfo)
        {
            win = win / _spinDataDefaultBet * BettingButton[betInfo.PlayBet];
            return (long)win;
        }
        protected virtual void convertWinsByBet(double balance, AmaticPacket packet, BaseAmaticSlotBetInfo betInfo)
        {
            packet.win          = convertWinByBet(packet.win,           betInfo);
            packet.curfreewin   = convertWinByBet(packet.curfreewin,    betInfo);
            packet.totalfreewin = convertWinByBet(packet.totalfreewin,  betInfo);
        }
    }
}
