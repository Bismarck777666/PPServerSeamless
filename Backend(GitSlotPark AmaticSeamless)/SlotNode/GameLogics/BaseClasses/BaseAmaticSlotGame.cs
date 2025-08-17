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
        protected virtual int       LineTypeCnt             => 1;
        protected virtual int       Cols                    => 5;
        protected virtual int       FreeCols                => 5;
        protected virtual string    InitString              => "";
        protected virtual int       ReelSetColBitNum        => 1;
        protected virtual int       ReelSetBitNum           => 1;   //1 또는 2(릴셋자료를 읽을때 단위비트수)

        public BaseAmaticSlotGame()
        {
            ReceiveAsync<PerformanceTestRequest>(onPerformanceTest);
        }
        protected virtual async Task onPerformanceTest(PerformanceTestRequest _)
        {
            try
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                //자연빵 1만개스핀 선택
                double sumOdd1 = 0.0;
                BaseAmaticSlotBetInfo betInfo = new BaseAmaticSlotBetInfo();
                betInfo.MoreBet         = -1;
                betInfo.PurchaseStep    = -1;
                for (int i = 0; i < 100000; i++)
                {
                    BasePPSlotSpinData spinData = await selectRandomStop(0, betInfo);
                    sumOdd1 += spinData.SpinOdd;
                }
                stopWatch.Stop();
                long elapsed1 = stopWatch.ElapsedMilliseconds;

                stopWatch.Reset();
                stopWatch.Start();

                double sumOdd2 = 0.0;
                //MoreBet 1만개
                if (SupportMoreBet)
                {
                    betInfo.MoreBet = 0;
                    for (int i = 0; i < 100000; i++)
                    {
                        BasePPSlotSpinData spinData = await selectRandomStop(0, betInfo);
                        sumOdd2 += spinData.SpinOdd;
                    }
                }
                stopWatch.Stop();
                long elapsed2 = stopWatch.ElapsedMilliseconds;
                stopWatch.Reset();
                stopWatch.Start();

                double sumOdd3 = 0.0;
                if (SupportPurchaseFree)
                {
                    for (int i = 0; i < 100000; i++)
                    {
                        BasePPSlotSpinData spinData = await selectPurchaseFreeSpin(0, betInfo, 0.0);
                        sumOdd3 += spinData.SpinOdd;
                    }
                }

                stopWatch.Stop();
                long elapsed3 = stopWatch.ElapsedMilliseconds;
                long elapsed4 = stopWatch.ElapsedMilliseconds;


                _logger.Info("{0} Performance Test Results:  \r\nPayrate: {8}%, {1}s, {2}%\t{3}s {4}%\t{5}s {6}%\t{7}s", this.GameName,
                    Math.Round((double)elapsed1 / 1000.0, 3), Math.Round(sumOdd1 / 1000, 3),
                    Math.Round((double)elapsed2 / 1000.0, 3), Math.Round(sumOdd2 / (1000 * MoreBetMultiple), 3),
                    Math.Round((double)elapsed3 / 1000.0, 3), Math.Round(sumOdd3 / 1000, 3),
                    Math.Round((double)elapsed4 / 1000.0, 3), _config.PayoutRate);
                Sender.Tell(true);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::onPerformanceTest {0}", ex);
                Sender.Tell(false);
            }
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
                    _anteBetMinusZeroCount = (int)((1.0 - 1.0 / MoreBetMultiple) * _naturalSpinCount);
                    if (_anteBetMinusZeroCount > _emptySpinCount)
                        _logger.Error("More Bet Probabily calculation doesn't work in {0}", GameName);
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
        protected override async Task onProcMessage(string strUserID, int websiteID, GITMessage message, double userBalance, Currencies currency)
        {
            string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);
            switch ((CSMSG_CODE) message.MsgCode)
            {
                case CSMSG_CODE.CS_AMATIC_DOINIT:
                    onDoInit(strGlobalUserID, message, userBalance, currency);
                    break;
                case CSMSG_CODE.CS_AMATIC_DOSPIN:
                    await onDoSpin(strUserID, websiteID, message, userBalance, currency);
                    break;
                case CSMSG_CODE.CS_AMATIC_DOCOLLECT:
                    onDoCollect(strGlobalUserID, message, userBalance, currency);
                    break;
                case CSMSG_CODE.CS_AMATIC_DOHEARTBEAT:
                    onDoHeartBeat(strGlobalUserID, message, userBalance, currency);
                    break;
                case CSMSG_CODE.CS_AMATIC_DOGAMBLEPICK:
                    await onGamblePick(strUserID, websiteID, message, userBalance);
                    break;
                case CSMSG_CODE.CS_AMATIC_DOGAMBLEHALF:
                    onGambleHalf(strGlobalUserID, userBalance);
                    break;
                case CSMSG_CODE.CS_AMATIC_NOTPROCDRESULT:
                    onDoUndoUserSpin(strGlobalUserID);
                    break;
            }
        }
        protected virtual void onDoInit(string strGlobalUserID, GITMessage message, double balance, Currencies currency)
        {
            try
            {
                string strResponse      = buildInitString(strGlobalUserID, balance, currency);

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_AMATIC_DOINIT);
                responseMessage.Append(strResponse);

                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::onDoInit GameID: {0}, {1}", _gameID, ex);
            }
        }
        protected virtual void onDoHeartBeat(string strGlobalUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                BaseAmaticSlotBetInfo betInfo = new BaseAmaticSlotBetInfo();
                betInfo.CurrencyInfo = currency;
                string strResponse = buildResMsgString(strGlobalUserID, userBalance, 0, betInfo, "", AmaticMessageType.HeartBeat);

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_AMATIC_DOHEARTBEAT);
                responseMessage.Append(strResponse);

                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::onDoHeartBeat GameID: {0}, {1}", _gameID, ex);
            }
        }
        private long getUnixMiliTimestamp()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            long unixTimeMilliseconds = now.ToUnixTimeMilliseconds();
            return unixTimeMilliseconds;
        }
        protected string createRoundID()
        {
            return string.Format("{0}{1}", getUnixMiliTimestamp(), Guid.NewGuid().ToString().Replace("-", ""));
        }
        protected string createTransactionID()
        {
            return string.Format("{0}{1}", getUnixMiliTimestamp(), Guid.NewGuid().ToString().Replace("-", ""));
        }
        protected virtual async Task onDoSpin(string strUserID, int websiteID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);

                bool isNewBet = true;
                if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                    isNewBet = false;

                if (message.MsgCode == (ushort)CSMSG_CODE.CS_AMATIC_DOSPIN)
                    readBetInfoFromMessage(message, strGlobalUserID, currency);

                await spinGame(strUserID, websiteID, userBalance, isNewBet);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::onDoSpin GameID: {0}, {1}", _gameID, ex);
            }
        }
        protected void onDoCollect(string strGlobalUserID, GITMessage message, double balance, Currencies currency)
        {
            try
            {
                if (!_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    _logger.Error("{0} bet information has not been found in BaseAmaticSlotGame::onDoCollect.", strGlobalUserID);
                    return;
                }
                BaseAmaticSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];

                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    _logger.Error("{0} result information has not been found in BaseAmaticSlotGame::onDoCollect.", strGlobalUserID);
                    return;
                }

                BaseAmaticSlotSpinResult result = _dicUserResultInfos[strGlobalUserID];
                balance += result.TotalWin;

                _dicUserResultInfos[strGlobalUserID]              = result;
                _dicUserLastBackupResultInfos[strGlobalUserID]    = result;

                string strResponse = buildResMsgString(strGlobalUserID, balance, 0, new BaseAmaticSlotBetInfo() { CurrencyInfo = currency }, "", AmaticMessageType.Collect);
                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_AMATIC_DOCOLLECT);
                responseMessage.Append(strResponse);

                if(result.TotalWin > 0.0)
                {
                    ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, responseMessage, 0.0, result.TotalWin, new GameLogInfo(GameName, "0", "Collect"), UserBetTypes.Normal);
                    toUserResult.BetTransactionID   = betInfo.BetTransactionID;
                    toUserResult.RoundID            = betInfo.RoundID;
                    toUserResult.TransactionID      = createTransactionID();
                    toUserResult.RoundEnd           = true;

                    saveBetResultInfo(strGlobalUserID);
                    Sender.Tell(toUserResult);

                    result.TotalWin = 0.0;
                    return;
                }
                
                Sender.Tell(new ToUserMessage((ushort)_gameID, responseMessage));
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::onDoCollect {0}", ex);
            }
        }
        protected async Task onGamblePick(string strUserID, int websiteID, GITMessage message, double balance)
        {
            try
            {
                int gambletype = (int)message.Pop();
                string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);

                if (!_dicUserBetInfos.ContainsKey(strGlobalUserID) || !_dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    _logger.Error("Can't gamble without betInfo and spinResult in BaseAmaticSlotGame::onGamblePick");
                    return;
                }

                BaseAmaticSlotBetInfo betInfo       = _dicUserBetInfos[strGlobalUserID];
                BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];
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

                await gamblePickGame(strUserID, websiteID, balance);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::onGamblePick {0}", ex);
            }
        }
        protected void onGambleHalf(string strGlobalUserID, double balance)
        {
            try
            {
                if (!_dicUserBetInfos.ContainsKey(strGlobalUserID) || !_dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    _logger.Error("Can't gamble without betInfo and spinResult in BaseAmaticSlotGame::onGambleHalf");
                    return;
                }

                BaseAmaticSlotBetInfo betInfo       = _dicUserBetInfos[strGlobalUserID];
                BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];
                if (betInfo.HasRemainResponse)
                {
                    _logger.Error("Can't half until spin finish in BaseAmaticSlotGame::onGambleHalf");
                    return;
                }
                    
                betInfo.GambleHalf = true;
                betInfo.GambleType = 0;

                gambleHalfGame(strGlobalUserID, balance);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::onGambleHalf {0}", ex);
            }
        }
        protected void onDoUndoUserSpin(string strGlobalUserID)
        {
            undoUserResultInfo(strGlobalUserID);
            undoUserBetInfo(strGlobalUserID);
            saveBetResultInfo(strGlobalUserID);
        }
        protected virtual void undoUserResultInfo(string strGlobalUserID)
        {
            try
            {
                if (!_dicUserLastBackupResultInfos.ContainsKey(strGlobalUserID))
                    return;

                BaseAmaticSlotSpinResult lastResult = _dicUserLastBackupResultInfos[strGlobalUserID];
                if (lastResult == null)
                    _dicUserResultInfos.Remove(strGlobalUserID);
                else
                    _dicUserResultInfos[strGlobalUserID] = lastResult;
                _dicUserLastBackupResultInfos.Remove(strGlobalUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::undoUserResultInfo {0}", ex);
            }
        }
        protected virtual void undoUserBetInfo(string strGlobalUserID)
        {
            try
            {
                if (!_dicUserLastBackupBetInfos.ContainsKey(strGlobalUserID))
                    return;

                byte[] userBetInfoBytes = _dicUserLastBackupBetInfos[strGlobalUserID];
                using (MemoryStream ms = new MemoryStream(userBetInfoBytes))
                {
                    using (BinaryReader binaryReader = new BinaryReader(ms))
                    {
                        BaseAmaticSlotBetInfo betInfo   = restoreBetInfo(strGlobalUserID, binaryReader);
                        _dicUserBetInfos[strGlobalUserID] = betInfo;
                    }
                }
                _dicUserLastBackupBetInfos.Remove(strGlobalUserID);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::undoUserBetInfo {0}", ex);
            }
        }
        #endregion
        
        protected virtual void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                BaseAmaticSlotBetInfo betInfo   = new BaseAmaticSlotBetInfo();
                int line = (int)message.Pop();
                int lineType = getLineTypeFromPlayLine(line);
                betInfo.PlayLine            = (int)LINES[lineType];
                betInfo.PlayBet             = (int)message.Pop();
                betInfo.PurchaseStep        = (int)message.Pop();
                betInfo.MoreBet             = (int)message.Pop();
                betInfo.CurrencyInfo        = currency;
                betInfo.GambleType          = 0;
                betInfo.GambleHalf          = false;

                if (BettingButton[betInfo.PlayBet] * betInfo.RelativeTotalBet <= 0)
                {
                    _logger.Error("{0} betInfo 0 or infinite in BaseAmaticSlotGame::readBetInfoFromMessage", strGlobalUserID);
                    return;
                }

                BaseAmaticSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strGlobalUserID, out oldBetInfo))
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
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::readBetInfoFromMessage {0}", ex);
            }
        }
        protected virtual int getLineTypeFromPlayLine(int playline)
        {
            return 0;
        }
        protected AmaticMessageType convertRespMsgCodeToAction(long actionType)
        {
            return (AmaticMessageType)actionType;
        }
        protected virtual BaseAmaticSlotSpinResult calculateResult(BaseAmaticSlotBetInfo betInfo,string strGlobalUserID, string strSpinResponse, bool isFirst, double userBalance, double betMoney)
        {
            try
            {
                BaseAmaticSlotSpinResult spinResult = new BaseAmaticSlotSpinResult();
                AmaticPacket packet                 = new AmaticPacket(strSpinResponse, Cols, FreeCols);
                double pointUnit                    = getPointUnit(betInfo);

                AmaticMessageType currentAction = convertRespMsgCodeToAction((int) packet.messagetype);
                if(currentAction == AmaticMessageType.NormalSpin || currentAction == AmaticMessageType.LastFree ||
                     currentAction == AmaticMessageType.LastWheel || currentAction == AmaticMessageType.LastRespin ||
                     currentAction == AmaticMessageType.LastPower || currentAction == AmaticMessageType.BonusEnd || 
                     currentAction == AmaticMessageType.DiamondEnd ||
                    currentAction == AmaticMessageType.GamblePick || currentAction == AmaticMessageType.GambleHalf)
                {
                    spinResult.TotalWin = Math.Round((double)packet.win * BettingButton[betInfo.PlayBet] / _spinDataDefaultBet * pointUnit, 2);
                }

                spinResult.ResultString = buildResMsgString(strGlobalUserID, userBalance,betMoney, betInfo, strSpinResponse, currentAction);
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
            double pointUnit = DicCurrencyInfo.Instance._currencyInfo[betInfo.CurrencyInfo].Rate / 100.0;
            return pointUnit;
        }
        protected virtual async Task<BaseAmaticSlotSpinResult> generateSpinResult(BaseAmaticSlotBetInfo betInfo, string strUserID, int websiteID, double userBalance, double betMoney, bool usePayLimit)
        {
            BasePPSlotSpinData          spinData    = null;
            BaseAmaticSlotSpinResult    result      = null;

            string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);
            if (betInfo.HasRemainResponse)
            {
                BaseAmaticActionToResponse nextResponse = betInfo.pullRemainResponse();
                result = calculateResult(betInfo, strGlobalUserID, nextResponse.Response, false, userBalance, betMoney);

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

            spinData = await selectRandomStop(websiteID, totalBet, betInfo);

            //첫자료를 가지고 결과를 계산한다.
            double totalWin = totalBet * spinData.SpinOdd;
            
            if (!usePayLimit || await checkWebsitePayoutRate(websiteID, realBetMoney, totalWin))
            {
                do
                {
                    result = calculateResult(betInfo, strGlobalUserID, spinData.SpinStrings[0], true, userBalance, betMoney);
                    if (spinData.SpinStrings.Count > 1)
                        betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
                    return result;
                } while (false);
            }

            double emptyWin = 0.0;

            if (SupportPurchaseFree && betInfo.isPurchase)
            {
                spinData    = await selectMinStartFreeSpinData(betInfo);
                result      = calculateResult(betInfo, strGlobalUserID, spinData.SpinStrings[0], true, userBalance, betMoney);
                emptyWin    = totalBet * spinData.SpinOdd;

                //뒤에 응답자료가 또 있다면
                if (spinData.SpinStrings.Count > 1)
                    betInfo.RemainReponses = buildResponseList(spinData.SpinStrings);
            }
            else
            {
                spinData    = await selectEmptySpin(betInfo);
                result      = calculateResult(betInfo, strGlobalUserID, spinData.SpinStrings[0], true, userBalance, betMoney);
            }

            sumUpWebsiteBetWin(websiteID, realBetMoney, emptyWin);
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
        protected virtual async Task spinGame(string strUserID, int websiteID, double userBalance, bool isNewBet)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);

                //해당 유저의 베팅정보를 얻는다. 만일 베팅정보가 없다면(례외상황) 그대로 리턴한다.
                BaseAmaticSlotBetInfo betInfo = null;
                if (!_dicUserBetInfos.TryGetValue(strGlobalUserID, out betInfo))
                    return;

                byte[] betInfoBytes = backupBetInfo(betInfo);

                BaseAmaticSlotSpinResult lastResult = null;
                if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                    lastResult = _dicUserResultInfos[strGlobalUserID];

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
                    GITMessage message = new GITMessage((ushort)SCMSG_CODE.SC_AMATIC_DOSPIN);
                    message.Append("-1User balance is less that bet money");
                    ToUserMessage toUserResult = new ToUserMessage((int)_gameID, message);
                    Sender.Tell(toUserResult, Self);
                    _logger.Error("user balance is less than bet money in BaseAmaticSlotGame::spinGame {0} balance:{1}, bet money: {2} game id:{3}",
                        strGlobalUserID, userBalance, betMoney, _gameID);
                    return;
                }

                if (isNewBet)
                {
                    betInfo.BetTransactionID    = createTransactionID();
                    betInfo.RoundID             = createRoundID();
                }

                //결과를 생성한다.
                BaseAmaticSlotSpinResult spinResult = await generateSpinResult(betInfo, strUserID, websiteID, userBalance, betMoney, true);

                //게임로그
                string strGameLog               = spinResult.ResultString;
                _dicUserResultInfos[strGlobalUserID]  = spinResult;

                //결과를 보내기전에 베팅정보를 디비에 보관한다
                saveBetResultInfo(strGlobalUserID);

                //생성된 게임결과를 유저에게 보낸다.(윈은 Collect요청시에 처리한다.)
                sendGameResult(betInfo, spinResult, strGlobalUserID, betMoney, 0.0, strGameLog, userBalance, betType);

                _dicUserLastBackupBetInfos[strGlobalUserID]       = betInfoBytes;
                _dicUserLastBackupResultInfos[strGlobalUserID]    = lastResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::spinGame {0}", ex);
            }
        }
        protected virtual async Task gamblePickGame(string strUserID, int websiteID, double userbalance)
        {
            string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);

            BaseAmaticSlotBetInfo betInfo       = _dicUserBetInfos[strGlobalUserID];
            BaseAmaticSlotSpinResult lastResult = _dicUserResultInfos[strGlobalUserID];
            string strSpinResponse = lastResult.ResultString;
            AmaticPacket packet = new AmaticPacket(strSpinResponse, Cols, FreeCols);

            double pointUnit = getPointUnit(betInfo);
            int cardNumber = Pcg.Default.Next(0, 13);
            int cardSymbol = await selectCardSymbol(websiteID, betInfo, lastResult);

            int winMultiple = 0;
            if (betInfo.GambleType == (int)GambleTypes.Red && (cardSymbol == (int)CardTypes.Diamond || cardSymbol == (int)CardTypes.Heart)) 
                winMultiple = 2;
            else if (betInfo.GambleType == (int)GambleTypes.Black && (cardSymbol == (int)CardTypes.Club || cardSymbol == (int)CardTypes.Spade)) 
                winMultiple = 2;
            else if (betInfo.GambleType == (int)GambleTypes.Diamond && cardSymbol == (int)CardTypes.Diamond)
                winMultiple = 4;
            else if (betInfo.GambleType == (int)GambleTypes.Heart && cardSymbol == (int)CardTypes.Heart)
                winMultiple = 4;
            else if (betInfo.GambleType == (int)GambleTypes.Crub && cardSymbol == (int)CardTypes.Club)
                winMultiple = 4;
            else if (betInfo.GambleType == (int)GambleTypes.Spade && cardSymbol == (int)CardTypes.Spade)
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

            saveBetResultInfo(strGlobalUserID);
            _dicUserResultInfos[strGlobalUserID]            = lastResult;
            sendGamblePickResult(lastResult, strGlobalUserID, 0.0, 0.0, "GamblePick", userbalance);
            _dicUserLastBackupResultInfos[strGlobalUserID]  = lastResult;
        }
        private async Task<int> selectCardSymbol(int websiteID, BaseAmaticSlotBetInfo betInfo, BaseAmaticSlotSpinResult lastResult)
        {
            int cardSymbol = Pcg.Default.Next(0, 4);

            double payoutRate   = getPayoutRate(websiteID);
            double randomDouble = Pcg.Default.NextDouble(0.0, 100.0);

            //환수율이 이상이면 틀린카드
            if (randomDouble >= payoutRate || payoutRate == 0.0)
            {
                if (betInfo.GambleType == (int)GambleTypes.Red && (cardSymbol == (int)CardTypes.Diamond || cardSymbol == (int)CardTypes.Heart))
                {
                    List<int> noRedSymbol = new List<int>() { (int)CardTypes.Club, (int)CardTypes.Spade };
                    cardSymbol = noRedSymbol[Pcg.Default.Next(0, 2)];
                }
                else if (betInfo.GambleType == (int)GambleTypes.Black && (cardSymbol == (int)CardTypes.Club || cardSymbol == (int)CardTypes.Spade))
                {
                    List<int> noBlackSymbol = new List<int>() { (int)CardTypes.Diamond, (int)CardTypes.Heart };
                    cardSymbol = noBlackSymbol[Pcg.Default.Next(0, 2)];
                }
                else if (betInfo.GambleType == (int)GambleTypes.Diamond && cardSymbol == (int)CardTypes.Diamond)
                {
                    List<int> noDiamondSymbol = new List<int>() { (int)CardTypes.Heart, (int)CardTypes.Club, (int)CardTypes.Spade };
                    cardSymbol = noDiamondSymbol[Pcg.Default.Next(0, 3)];
                }
                else if (betInfo.GambleType == (int)GambleTypes.Heart && cardSymbol == (int)CardTypes.Heart)
                {
                    List<int> noHeartSymbol = new List<int>() { (int)CardTypes.Diamond, (int)CardTypes.Club, (int)CardTypes.Spade };
                    cardSymbol = noHeartSymbol[Pcg.Default.Next(0, 3)];
                }
                else if (betInfo.GambleType == (int)GambleTypes.Crub && cardSymbol == (int)CardTypes.Club)
                {
                    List<int> noCrabSymbol = new List<int>() { (int)CardTypes.Diamond, (int)CardTypes.Heart, (int)CardTypes.Spade };
                    cardSymbol = noCrabSymbol[Pcg.Default.Next(0, 3)];
                }
                else if (betInfo.GambleType == (int)GambleTypes.Spade && cardSymbol == (int)CardTypes.Spade)
                {
                    List<int> noSpadeSymbol = new List<int>() { (int)CardTypes.Diamond, (int)CardTypes.Heart, (int)CardTypes.Club };
                    cardSymbol = noSpadeSymbol[Pcg.Default.Next(0, 3)];
                }
            }
            else
            {
                if (betInfo.GambleType == (int)GambleTypes.Red || betInfo.GambleType == (int)GambleTypes.Black)
                {
                    if (!await checkWebsitePayoutRate(websiteID, lastResult.TotalWin, lastResult.TotalWin * 2, 0))
                    {
                        if (betInfo.GambleType == (int)GambleTypes.Red && (cardSymbol == (int)CardTypes.Diamond || cardSymbol == (int)CardTypes.Heart))
                        {
                            List<int> noRedSymbol = new List<int>() { (int)CardTypes.Club, (int)CardTypes.Spade };
                            cardSymbol = noRedSymbol[Pcg.Default.Next(0, 2)];
                        }
                        else if (betInfo.GambleType == (int)GambleTypes.Black && (cardSymbol == (int)CardTypes.Club || cardSymbol == (int)CardTypes.Spade))
                        {
                            List<int> noBlackSymbol = new List<int>() { (int)CardTypes.Diamond, (int)CardTypes.Heart };
                            cardSymbol = noBlackSymbol[Pcg.Default.Next(0, 2)];
                        }
                    }
                    else
                    {
                        if (betInfo.GambleType == (int)GambleTypes.Red && (cardSymbol != (int)CardTypes.Diamond && cardSymbol != (int)CardTypes.Heart))
                            sumUpWebsiteBetWin(websiteID, 0, -lastResult.TotalWin * 2, 0);
                        else if (betInfo.GambleType == (int)GambleTypes.Black && (cardSymbol != (int)CardTypes.Club && cardSymbol != (int)CardTypes.Spade))
                            sumUpWebsiteBetWin(websiteID, 0, -lastResult.TotalWin * 2, 0);
                    }
                }
                else
                {
                    if (!await checkWebsitePayoutRate(websiteID, lastResult.TotalWin, lastResult.TotalWin * 4, 0))
                    {
                        if (betInfo.GambleType == (int)GambleTypes.Diamond && cardSymbol == (int)CardTypes.Diamond)
                        {
                            List<int> noDiamondSymbol = new List<int>() { (int)CardTypes.Heart, (int)CardTypes.Club, (int)CardTypes.Spade };
                            cardSymbol = noDiamondSymbol[Pcg.Default.Next(0, 3)];
                        }
                        else if (betInfo.GambleType == (int)GambleTypes.Heart && cardSymbol == (int)CardTypes.Heart)
                        {
                            List<int> noHeartSymbol = new List<int>() { (int)CardTypes.Diamond, (int)CardTypes.Club, (int)CardTypes.Spade };
                            cardSymbol = noHeartSymbol[Pcg.Default.Next(0, 3)];
                        }
                        else if (betInfo.GambleType == (int)GambleTypes.Crub && cardSymbol == (int)CardTypes.Club)
                        {
                            List<int> noCrabSymbol = new List<int>() { (int)CardTypes.Diamond, (int)CardTypes.Heart, (int)CardTypes.Spade };
                            cardSymbol = noCrabSymbol[Pcg.Default.Next(0, 3)];
                        }
                        else if (betInfo.GambleType == (int)GambleTypes.Spade && cardSymbol == (int)CardTypes.Spade)
                        {
                            List<int> noSpadeSymbol = new List<int>() { (int)CardTypes.Diamond, (int)CardTypes.Heart, (int)CardTypes.Club };
                            cardSymbol = noSpadeSymbol[Pcg.Default.Next(0, 3)];
                        }
                    }
                    else
                    {
                        if (betInfo.GambleType == (int)GambleTypes.Diamond && cardSymbol != (int)CardTypes.Diamond)
                            sumUpWebsiteBetWin(websiteID, 0, -lastResult.TotalWin * 4, 0);
                        else if (betInfo.GambleType == (int)GambleTypes.Heart && cardSymbol != (int)CardTypes.Heart)
                            sumUpWebsiteBetWin(websiteID, 0, -lastResult.TotalWin * 4, 0);
                        else if (betInfo.GambleType == (int)GambleTypes.Crub && cardSymbol != (int)CardTypes.Club)
                            sumUpWebsiteBetWin(websiteID, 0, -lastResult.TotalWin * 4, 0);
                        else if (betInfo.GambleType == (int)GambleTypes.Spade && cardSymbol != (int)CardTypes.Spade)
                            sumUpWebsiteBetWin(websiteID, 0, -lastResult.TotalWin * 4, 0);
                    }
                }
            }

            return cardSymbol;
        }
        protected virtual void gambleHalfGame(string strGlobalUserID, double userbalance)
        {
            BaseAmaticSlotBetInfo betInfo       = _dicUserBetInfos[strGlobalUserID];
            BaseAmaticSlotSpinResult lastResult = _dicUserResultInfos[strGlobalUserID];
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

            saveBetResultInfo(strGlobalUserID);
            _dicUserResultInfos[strGlobalUserID]              = lastResult;
            sendGambleHalfResult(lastResult, strGlobalUserID, 0.0, winMoney, "GambleHalf", userbalance);
            _dicUserLastBackupResultInfos[strGlobalUserID]    = lastResult;
        }
        protected virtual void sendGameResult(BaseAmaticSlotBetInfo betInfo, BaseAmaticSlotSpinResult spinResult, string strGlobalUserID, double betMoney, double winMoney, string strGameLog, double userBalance, UserBetTypes betType)
        {
            GITMessage message = new GITMessage((ushort) SCMSG_CODE.SC_AMATIC_DOSPIN);
            message.Append(spinResult.ResultString);

            ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, message, betMoney, winMoney, new GameLogInfo(GameName, "0", strGameLog), UserBetTypes.Normal);

            toUserResult.BetTransactionID   = betInfo.BetTransactionID;
            toUserResult.RoundID            = betInfo.RoundID;

            if(!betInfo.HasRemainResponse && spinResult.TotalWin == 0)
            {
                toUserResult.TransactionID  = createTransactionID();
                toUserResult.RoundEnd       = true;
            }

            Sender.Tell(toUserResult, Self);
        }
        protected virtual void sendGamblePickResult(BaseAmaticSlotSpinResult spinResult, string strGlobalUserID,double betMoney,double winMoney, string strGameLog, double userBalance)
        {
            BaseAmaticSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
            GITMessage message = new GITMessage((ushort)SCMSG_CODE.SC_AMATIC_DOGAMBLEPICK);
            message.Append(spinResult.ResultString);

            ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, message, betMoney, winMoney, new GameLogInfo(GameName, "0", strGameLog), UserBetTypes.Normal);
            toUserResult.BetTransactionID   = betInfo.BetTransactionID;
            toUserResult.RoundID            = betInfo.RoundID;
            toUserResult.TransactionID      = createTransactionID();
            toUserResult.RoundEnd           = false;
            if (spinResult.WinMoney == 0)
                toUserResult.RoundEnd       = true;

            saveBetResultInfo(strGlobalUserID);
            Sender.Tell(toUserResult, Self);
        }
        protected virtual void sendGambleHalfResult(BaseAmaticSlotSpinResult spinResult, string strGlobalUserID, double betMoney, double winMoney, string strGameLog, double userBalance)
        {
            BaseAmaticSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
            GITMessage message = new GITMessage((ushort)SCMSG_CODE.SC_AMATIC_DOGAMBLEHALF);
            message.Append(spinResult.ResultString);

            ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, message, betMoney, winMoney, new GameLogInfo(GameName, "0", strGameLog), UserBetTypes.Normal);
            toUserResult.BetTransactionID   = betInfo.BetTransactionID;
            toUserResult.RoundID            = betInfo.RoundID;
            toUserResult.TransactionID      = createTransactionID();
            toUserResult.RoundEnd           = false;
            
            saveBetResultInfo(strGlobalUserID);
            Sender.Tell(toUserResult, Self);
        }

        #region 스핀자료처리부분
        protected virtual async Task<BasePPSlotSpinData> selectRandomStop(int websiteID, BaseAmaticSlotBetInfo betInfo)
        {
            OddAndIDData selectedOddAndID = selectRandomOddAndID(websiteID, betInfo);
            
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
            selectedOddAndID.ID = selectedID;
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
        protected virtual async Task<BasePPSlotSpinData> selectPurchaseFreeSpin(int websiteID, BaseAmaticSlotBetInfo betInfo, double baseBet)
        {
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
        protected virtual async Task<BasePPSlotSpinData> selectRandomStop(int websiteID, double baseBet, BaseAmaticSlotBetInfo betInfo)
        {
            //프리스핀만 테스트할때
            //return await selectRandomStartFreeSpinData(betInfo);

            //프리스핀구입을 먼저 처리한다.
            if (SupportPurchaseFree && betInfo.isPurchase)
                return await selectPurchaseFreeSpin(websiteID, betInfo, baseBet);

            return await selectRandomStop(websiteID, betInfo);
        }
        protected override async Task<bool> loadUserHistoricalData(string strGlobalUserID, bool isNewEnter)
        {
            try
            {
                string strKey           = string.Format("{0}_{1}", strGlobalUserID, _gameID);
                
                byte[] betInfoData = await RedisDatabase.RedisCache.StringGetAsync(strKey);
                if (betInfoData != null)
                {
                    using (var stream = new MemoryStream(betInfoData))
                    {
                        BinaryReader reader = new BinaryReader(stream);
                        BaseAmaticSlotBetInfo betInfo = restoreBetInfo(strGlobalUserID, reader);
                        if (betInfo != null)
                            _dicUserBetInfos[strGlobalUserID] = betInfo;
                    }
                }

                strKey = string.Format("{0}_{1}_result", strGlobalUserID, _gameID);
                byte[] resultInfoData = await RedisDatabase.RedisCache.StringGetAsync(strKey);
                if (resultInfoData != null)
                {
                    using (var stream = new MemoryStream(resultInfoData))
                    {
                        BinaryReader reader = new BinaryReader(stream);
                        BaseAmaticSlotSpinResult resultInfo = restoreResultInfo(strGlobalUserID, reader);
                        if (resultInfo != null)
                            _dicUserResultInfos[strGlobalUserID] = resultInfo;
                    }
                }
            }

            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::loadUserHistoricalData {0}", ex);
                return false;
            }
            return await base.loadUserHistoricalData(strGlobalUserID, isNewEnter);
        }
        protected virtual BaseAmaticSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            BaseAmaticSlotBetInfo betInfo = new BaseAmaticSlotBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected virtual BaseAmaticSlotSpinResult restoreResultInfo(string strGlobalUserID, BinaryReader reader)
        {
            BaseAmaticSlotSpinResult result = new BaseAmaticSlotSpinResult();
            result.SerializeFrom(reader);
            return result;
        }
        protected virtual void saveBetResultInfo(string strGlobalUserID)
        {
            try
            {
                if (_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    byte[] betInfoBytes = _dicUserBetInfos[strGlobalUserID].convertToByte();
                    _redisWriter.Tell(new UserBetInfoWrite(strGlobalUserID, _gameID, betInfoBytes, false), Self);
                }
                else
                {
                    _redisWriter.Tell(new UserBetInfoWrite(strGlobalUserID, _gameID, null, false), Self);
                }
                if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    byte[] resultInfoBytes = _dicUserResultInfos[strGlobalUserID].convertToByte();
                    _redisWriter.Tell(new UserResultInfoWrite(strGlobalUserID, _gameID, resultInfoBytes, false), Self);
                }
                else
                {
                    _redisWriter.Tell(new UserResultInfoWrite(strGlobalUserID, _gameID, null, false), Self);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::saveBetInfo {0}", ex);
            }
        }
        protected override async Task onExitUserMessage(ExitGameRequest message)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", message.WebsiteID, message.UserID);
                //유저나가기전에 트랜잭션정보만 뽑아낸다
                BaseAmaticSlotBetInfo betInfo = null;
                
                if(_dicUserBetInfos.ContainsKey(strGlobalUserID))
                    betInfo = _dicUserBetInfos[strGlobalUserID];

                if (betInfo == null)
                {
                    Sender.Tell(new AmaticExitResponse(null));
                    return;
                }
                string betTransactionID = betInfo.BetTransactionID;
                string roundID          = betInfo.RoundID;

                double winMoney = await procUserExitGame(strGlobalUserID, message.Balance, message.Currency, message.UserRequested);

                _dicEnteredUsers.Remove(strGlobalUserID);
                if (winMoney == 0.0)
                {
                    Sender.Tell(new AmaticExitResponse(null));
                    return;
                }

                ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, null, 0.0, winMoney, new GameLogInfo(GameName, "0", "Collect"), UserBetTypes.Normal);
                toUserResult.BetTransactionID   = betTransactionID;
                toUserResult.RoundID            = roundID;
                toUserResult.TransactionID      = createTransactionID();
                toUserResult.RoundEnd           = true;
                Sender.Tell(new AmaticExitResponse(toUserResult));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::onExitUserMessage {0}", ex);
            }
        }
        protected async Task<double> procUserExitGame(string strGlobalUserID,double userbalance, Currencies currency, bool userRequested)
        {
            try
            {
                double winMoney = 0.0;

                if (_dicUserResultInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos[strGlobalUserID].Action != AmaticMessageType.Collect)
                {
                    BaseAmaticSlotSpinResult result = _dicUserResultInfos[strGlobalUserID];
                    winMoney = result.TotalWin;

                    if(!_dicUserBetInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                    {
                        string strResponse = buildResMsgString(strGlobalUserID, userbalance, 0, new BaseAmaticSlotBetInfo() { CurrencyInfo = currency }, "", AmaticMessageType.Collect);
                        result.Action   = AmaticMessageType.Collect;
                        result.TotalWin = 0;
                    }

                    byte[] resultInfoBytes = _dicUserResultInfos[strGlobalUserID].convertToByte();
                    await _redisWriter.Ask(new UserResultInfoWrite(strGlobalUserID, _gameID, resultInfoBytes, true));
                    _dicUserResultInfos.Remove(strGlobalUserID);
                }

                if (_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    if (_dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                    {
                        BaseAmaticSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
                        if (betInfo.HasRemainResponse)
                            winMoney = 0.0;
                    }

                    byte[] betInfoBytes = _dicUserBetInfos[strGlobalUserID].convertToByte();
                    await _redisWriter.Ask(new UserBetInfoWrite(strGlobalUserID, _gameID, betInfoBytes, true));
                    _dicUserBetInfos.Remove(strGlobalUserID);
                }
                
                _dicUserLastBackupResultInfos.Remove(strGlobalUserID);
                _dicUserLastBackupBetInfos.Remove(strGlobalUserID);
                return winMoney;

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::onUserExitGame GameID:{0} {1}", _gameID, ex);
                return 0.0;
            }
        }
        protected virtual string buildInitString(string strGlobalUserID, double balance, Currencies currency)
        {
            AmaticEncrypt encrypt = new AmaticEncrypt();
            string initString = string.Empty;

            InitPacket initPacket = new InitPacket(InitString, Cols, FreeCols, ReelSetColBitNum, ReelSetBitNum);
            initPacket.betstepamount    = BettingButton.ToList();
            initPacket.laststep         = 0;
            initPacket.minbet           = BettingButton[0];
            initPacket.maxbet           = BettingButton.ToList().Last() * LINES.ToList().Last();
            initPacket.lastline         = LINES.ToList().Last();

            if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                BaseAmaticSlotBetInfo betInfo       = _dicUserBetInfos[strGlobalUserID];
                BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];

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
        protected virtual string buildResMsgString(string strGlobalUserID, double balance,double betMoney, BaseAmaticSlotBetInfo betInfo, string spinString, AmaticMessageType type)
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

                if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];

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

    public enum GambleTypes
    {
        Red         = 1,
        Black       = 2,
        Diamond     = 3,
        Heart       = 4,
        Crub        = 5,
        Spade       = 6,
    }

    public enum CardTypes
    {
        Diamond     = 0,
        Heart       = 1,
        Club        = 2,
        Spade       = 3,
    }
}
