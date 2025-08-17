using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using GITProtocol;
using Akka.Event;
using GITProtocol.Utils;
using CommNode.Database;
using System.Net;
using Akka.Routing;
using System.Threading;
using StackExchange.Redis;
using Akka.Cluster;
using CommNode.PPPromo;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CommNode
{
    //로그인된 사용자를 표현하는 클라스 
    public class UserActor : ReceiveActor
    {
        #region 사용자정보
        private long                    _userDBID           = 0;
        private string                  _strUserID          = "";
        private string                  _strGlobalUserID    = "";
        private int                     _agentDBID          = 0;
        private double                  _balance            = 0.0;
        private int                     _currency           = 0;
        private long                    _lastScoreID        = 0;
        private string                  _agentName          = "";
        private string                  _strCountry         = null;
        #endregion

        #region 유저의 상태변수들       
        private bool                                _userDisconnected   = false;
        private PlatformTypes                       _platformType;
        private Dictionary<object, UserConnection>  _userConnections    = new Dictionary<object, UserConnection>();
        #endregion

        #region 사용자의 각종 보너스정보
        private UserRangeOddEventItem           _userRangeOddEventItem;
        private UserPPRacePrizeBonus            _userRacePrizeItem;
        private PPPromoStatus                   _ppPromoStatus;
        private List<UserBonus>                 _waitingUserBonuses     = new List<UserBonus>();
        #endregion

        private IActorRef                       _dbReader       = null;
        private IActorRef                       _dbWriter       = null;
        private IActorRef                       _redisWriter    = null;
        private readonly ILoggingAdapter        _logger         = Logging.GetLogger(Context);
        protected static RealExtensions.Epsilon _epsilion       = new RealExtensions.Epsilon(0.001);

        private ICancelable                     _conCheckCancel         = null;
        private IActorRef                       _afterQuitNotifyActor   = null;

        public UserActor(CreateNewUserMessage message)
        {
            _dbReader           = message.DBReader;
            _dbWriter           = message.DBWriter;
            _redisWriter        = message.RedisWriter;
            _userDBID           = message.LoginResponse.UserDBID;
            _strUserID          = message.LoginResponse.UserID;
            _balance            = message.LoginResponse.UserBalance;
            _currency           = message.LoginResponse.Currency;
            _agentDBID          = message.LoginResponse.AgentDBID;
            _strGlobalUserID    = string.Format("{0}_{1}", _agentDBID, _strUserID);
            _lastScoreID        = message.LoginResponse.LastScoreCounter;
            _platformType       = message.PlatformType;

            _userConnections.Add(message.Connection, new UserConnection(message.Connection));

            ReceiveAsync<UserLoggedIn>              (onUserLoginSucceeded);
            ReceiveAsync<FromConnRevMessage>        (onProcMessage);

            //소켓연결 추가/삭제처리
            Receive<SocketConnectionAdded>          (onProcSocketConnectionAdded);
            ReceiveAsync<SocketConnectionClosed>    (onProcSocketConnectionClosed);

            //HTTP세션 추가/삭제처리
            Receive<HttpSessionAdded>               (onProcHttpSessionAdded);
            ReceiveAsync<HttpSessionClosed>         (onProcHttpSessionClosed);
            ReceiveAsync<CloseHttpSession>          (onCloseHttpSession);

            Receive<UserRangeOddEventItem>          (onUserRangeOddEvent);
            Receive<UserEventCancelled>             (onUserEventCancelled);

            Receive<ApiDepositMessage>              (onApiDepositMessage);

            Receive<QuitUserMessage>                (onForceLogoutMessage);
            Receive<QuitAndNotifyMessage>           (onForceQuitAndNotifyMessage);
            Receive<string>                         (onCommand);
            ReceiveAsync<SlotGamesNodeShuttingdown> (onSlotGameServerShuttingdown);
            Receive<PPPromoStatus>                  (onPromoUpdateEvent);
            Receive<BalanceFromUserRequest>         (_ =>
            {
                Sender.Tell(new BalanceFromUserResponse(_balance));
            });
        }

        public static Props Props(CreateNewUserMessage message)
        {
            return Akka.Actor.Props.Create(() => new UserActor(message));
        }
        
        protected override void PreStart()
        {
            Self.Tell(new UserLoggedIn());                                                  // 로그인성공후에 보낼 메세지들을 먼저 보낸다.
            Context.System.EventStream.Subscribe(Self, typeof(PPPromoStatus));
            base.PreStart();
        }
        
        protected override void PostStop()
        {
            Context.System.EventStream.Unsubscribe(Self, typeof(PPPromoStatus));
            if (_conCheckCancel != null)
                _conCheckCancel.Cancel();

            base.PostStop();
        }

        private async Task onUserLoginSucceeded(UserLoggedIn message)
        {
            try
            {
                _dbWriter.Tell(new UserLoginStateItem(_userDBID, (int)_platformType));

                //프라그마틱 promo정보요청
                _ppPromoStatus = await Context.System.ActorSelection("/user/promofetcher").Ask<PPPromoStatus>(new PPPromoGetStatus());

                //유저에게 수여된 각종 보너스정보들을 디비에서 읽어서 전송한다.
                _dbReader.Tell(new GetUserBonusItems(_agentDBID, _strUserID));

                _logger.Info("{0} has been logged in successfully {1}", _strUserID, _platformType);

                _conCheckCancel = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(1000, 1000, Self, "checkConn", Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::onUserLoginSucceeded {0}", ex);
            }
        }

        private void onApiDepositMessage(ApiDepositMessage message)
        {
            _balance += message.Amount;
        }

        //유저가 로그인 되는 동안 적용된 스코변화를 감지하여 적용한다.
        #region 각종 사건처리부
        private void onPromoUpdateEvent(PPPromoStatus status)
        {
            _ppPromoStatus = status;
        }

        private void onForceQuitAndNotifyMessage(QuitAndNotifyMessage _)
        {
            _afterQuitNotifyActor = Sender;
            onForceLogoutMessage(new QuitUserMessage(_agentDBID, _strUserID));
        }

        private void onForceLogoutMessage(QuitUserMessage _)
        {
            GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_FORCEOUTUSER);

            //모든 연결들에 통지한다.
            for (int i = 0; i < _userConnections.Count; i++)
            {
                object key = _userConnections.Keys.ElementAt(i);
                if (key is string)
                {
                    //HTTP Session
                    Self.Tell(new CloseHttpSession(key as string));
                }
                else
                {
                    //Socket
                    (key as IActorRef).Tell(new SendMessageToUser(message, _balance));
                    (key as IActorRef).Tell("closeConnection");
                }
            }

            _logger.Info("User {0} has been kicked by admin", _strUserID);
        }

        private void onCommand(string strCommand)
        {
            if(strCommand == "checkConn")
            {
                foreach(KeyValuePair<object, UserConnection> pair in _userConnections)
                {
                    if(pair.Key is string)
                    {
                        if (DateTime.Now.Subtract(pair.Value.LastActiveTime) >= TimeSpan.FromMinutes(5))
                            Self.Tell(new CloseHttpSession(pair.Key as string));
                    }
                }
            }
        }
        
        private void onProcSocketConnectionAdded(SocketConnectionAdded _)
        {
            if (_userConnections.ContainsKey(Sender))
                return;

            _userConnections.Add(Sender, new UserConnection(Sender));
        }

        private void onProcHttpSessionAdded(HttpSessionAdded message)
        {
            if (_userConnections.ContainsKey(message.SessionToken))
                return;

            _userConnections.Add(message.SessionToken, new UserConnection(message.SessionToken));
        }

        private async Task onCloseHttpSession(CloseHttpSession message)
        {
            try
            {
                //레디스에서 해당 세션토큰을 삭제한다.
                await RedisDatabase.RedisCache.HashDeleteAsync(_strUserID + "_tokens", message.SessionToken);

                if (!_userConnections.ContainsKey(message.SessionToken))
                    return;

                UserConnection userConn = _userConnections[message.SessionToken];
                await onClosedUserConnection(userConn);

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in UserActor::onCloseHttpSession {0}", ex);
            }
        }
        
        private async Task onProcHttpSessionClosed(HttpSessionClosed message)
        {
            try
            {
                if (!_userConnections.ContainsKey(message.SessionToken))
                    return;

                UserConnection userConn = _userConnections[message.SessionToken];
                await onClosedUserConnection(userConn);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in UserActor::onProcHttpSessionClosed {0}", ex);
            }
        }

        private async Task onClosedUserConnection(UserConnection userConn)
        {
            int lastGameID = 0;
            if (userConn.GameActor != null)
            {
                if (userConn.GameType >= GAMETYPE.PP && userConn.GameType <= GAMETYPE.COUNT)
                {
                    ExitGameResponse response = null;
                    try
                    {
                        response = await userConn.GameActor.Ask<ExitGameResponse>(new ExitGameRequest(_strUserID, _agentDBID, _currency, _balance, true, false), Constants.RemoteTimeOut);
                        if (response is CQ9ExitGameResponse)
                            await procCQ9ExitGameResponse(response as CQ9ExitGameResponse);
                        if (response is AmaticExitResponse)
                            await procAmaticExitGameResponse(response as AmaticExitResponse);
                    }
                    catch (Exception ex)
                    {
                        _logger.Warning("{0} exit game {1} Failed : Exception {2}", _strGlobalUserID, userConn.GameID, ex);
                    }
                }
                lastGameID = userConn.GameID;
            }

            _userConnections.Remove(userConn.Connection);

            //유저의 모든 연결이 다 끊어졌다면
            if (_userConnections.Count == 0)
            {
                //디비에서 플레이어 상태갱신
                _dbWriter.Tell(new UserGameStateItem(_userDBID, 0, lastGameID));

                try
                {
                    await RedisDatabase.RedisCache.HashDeleteAsync("onlineusers", new RedisValue[] { _strGlobalUserID, _strGlobalUserID + "_path" });
                    await RedisDatabase.RedisCache.KeyDeleteAsync(_strGlobalUserID + "_tokens");

                }
                catch (Exception ex)
                {
                    _logger.Error("Exception has been occured in UserActor::onProcSocketConnectionClosed User ID: {0} {1}", _strGlobalUserID, ex);
                }

                _userDisconnected = true;
                _logger.Info("{0} user has been logged out", _strGlobalUserID);
                Context.Stop(Self);

                if (_afterQuitNotifyActor != null)
                {
                    _afterQuitNotifyActor.Tell(_balance);
                    _afterQuitNotifyActor = null;
                }
            }
        }
        
        private async Task onProcSocketConnectionClosed(SocketConnectionClosed message)
        {
            try
            {

                //만일 이미 게임에 입장한 상태라면 
                IActorRef connection = Sender;

                if (!_userConnections.ContainsKey(connection))
                    return;

                UserConnection userConn = _userConnections[connection];
                await onClosedUserConnection(userConn);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in UserActor::onProcUserConnectionClosed {0}", ex);
            }
        }
        #endregion

        #region 메세지처리함수들
        private async Task onProcMessage(FromConnRevMessage fromConnRevMsg)
        {
            //이미 로그아웃된 유저에 한해서 모든 메세지처리를 무시한다.
            if (_userDisconnected)
                return;

            if (!_userConnections.ContainsKey(fromConnRevMsg.Connection))
                return;

            UserConnection userConn = _userConnections[fromConnRevMsg.Connection];
            GITMessage message      = fromConnRevMsg.Message;
            if (message.MsgCode == (ushort)CSMSG_CODE.CS_HEARTBEAT)
            {
                userConn.LastActiveTime = DateTime.Now;

                //소켓연결인 경우에만 하트비트응답을 보낸다.
                if (userConn.Connection is IActorRef)
                {
                    GITMessage responseMessage = new GITMessage((ushort)CSMSG_CODE.CS_HEARTBEAT);
                    Sender.Tell(new SendMessageToUser(responseMessage, _balance));
                }
            }
            else if (message.MsgCode == (ushort)CSMSG_CODE.CS_ENTERGAME)
            {
                userConn.LastActiveTime = DateTime.Now;
                await procEnterGame(message, userConn);
            }
            else if (message.MsgCode >= (ushort)CSMSG_CODE.CS_SLOTGAMESTART && message.MsgCode <= (ushort)CSMSG_CODE.CS_SLOTGAMEEND)
            {
                userConn.LastActiveTime = DateTime.Now;
                await procSlotGameMsg(message, userConn);
            }
            else if (message.MsgCode >= (ushort)CSMSG_CODE.CS_BNGSLOTGAMESTART && message.MsgCode <= (ushort)CSMSG_CODE.CS_BNGSLOTGAMEEND)
            {
                if(message.MsgCode != (ushort) CSMSG_CODE.CS_BNG_DOSYNC)
                    userConn.LastActiveTime = DateTime.Now;                
                await procSlotGameMsg(message, userConn);
            }
            else if (message.MsgCode >= (ushort)CSMSG_CODE.CS_CQ9_START && message.MsgCode <= (ushort)CSMSG_CODE.CS_CQ9_END)
            {
                userConn.LastActiveTime = DateTime.Now;
                await procSlotGameMsg(message, userConn);
            }
            else if (message.MsgCode >= (ushort)CSMSG_CODE.CS_HABANERO_SLOTSTART && message.MsgCode <= (ushort)CSMSG_CODE.CS_HABANERO_SLOTEND)
            {
                userConn.LastActiveTime = DateTime.Now;
                await procSlotGameMsg(message, userConn);
            }
            else if (message.MsgCode >= (ushort)CSMSG_CODE.CS_PLAYSONSLOTGAMESTART && message.MsgCode <= (ushort)CSMSG_CODE.CS_PLAYSONSLOTGAMEEND)
            {
                if (message.MsgCode != (ushort)CSMSG_CODE.CS_PLAYSON_DOSYNC)
                    userConn.LastActiveTime = DateTime.Now;
                await procSlotGameMsg(message, userConn);
            }
            else if (message.MsgCode >= (ushort)CSMSG_CODE.CS_AMATICSLOTGAMESTART && message.MsgCode <= (ushort)CSMSG_CODE.CS_AMATICSLOTGAMEEND)
            {
                if (message.MsgCode != (ushort)CSMSG_CODE.CS_AMATIC_DOHEARTBEAT)
                    userConn.LastActiveTime = DateTime.Now;
                await procSlotGameMsg(message, userConn);
            }
            else if (message.MsgCode >= (ushort) CSMSG_CODE.CS_PP_PROMOSTART && message.MsgCode <= (ushort)CSMSG_CODE.CS_PP_PROMOEND)
            {
                userConn.LastActiveTime = DateTime.Now;
                await procPPPromoMsg(message, userConn);
            }
            else
            {
                _logger.Warning("Unknown paket received from {0} Message code:{1} User ID:{2}", "", message.MsgCode, _strGlobalUserID);
            }
        }

        private async Task procEnterGame(GITMessage message, UserConnection userConn)
        {
            int      gameID     = (int)(ushort)message.Pop();
            GAMETYPE gameType   = DBMonitorSnapshot.Instance.getGameType(gameID);

            //등록된 게임아이디가 아님
            if (gameType == GAMETYPE.NONE)
            {
                _logger.Warning("{0} tried to enter game for not registered game id {1}", _strGlobalUserID, (int)gameID);
                Sender.Tell("closeConnection");
                return;
            }

            //이미 게임에 입장함
            if (userConn.GameActor != null && userConn.GameID != gameID)
            {
                //이미 다른 게임에 입장하였다면
                _logger.Warning("{0} tried to enter game while it has already been entered to other game", _strGlobalUserID);
                Sender.Tell("closeConnection");
                return;
            }

            //이미 해당게임에 입장하였다면 처리를 진행하지 않는다.

            bool enterGameSucceeded = false;
            if (userConn.GameActor != null && userConn.GameID == gameID)
            {
                enterGameSucceeded = true;
            }
            else if (userConn.GameActor == null)
            {
                //같은 게임에 입장한 연결이 있는가를 검사한다.
                UserConnection oldConn = null;
                foreach (KeyValuePair<object, UserConnection> pair in _userConnections)
                {
                    if (pair.Value.GameActor != null && pair.Value.GameID == gameID)
                    {
                        oldConn = pair.Value;
                        break;
                    }
                }

                //원래의 연결을 비활성화시키고 새 연결에로 게임정보를 이관한다.
                if (oldConn != null)
                {
                    userConn.GameActor  = oldConn.GameActor;
                    userConn.GameID     = gameID;
                    userConn.GameType   = gameType;

                    oldConn.resetGame();
                    if (!oldConn.IsHttpSession)
                        (oldConn.Connection as IActorRef).Tell("closeConnection");
                    else
                        Self.Tell(new CloseHttpSession(oldConn.Connection as string));
                    enterGameSucceeded = true;
                }
                else
                {
                    if (gameType >= GAMETYPE.PP && gameType <= GAMETYPE.COUNT)
                    {
                        try
                        {
                            EnterGameRequest requestMsg     = new EnterGameRequest(gameID, _agentDBID, _strUserID, _currency, Self);
                            EnterGameResponse responseMsg   = await Context.System.ActorSelection(Constants.SlotGameRouterPath).Ask<EnterGameResponse>(requestMsg, Constants.RemoteTimeOut);
                            //게임입장성공
                            if (responseMsg.Ack == 0)
                            {
                                userConn.GameActor  = responseMsg.GameActor;
                                userConn.GameID     = gameID;
                                userConn.GameType   = gameType;
                                enterGameSucceeded  = true;

                                //유저상태갱신(게임입장)
                                _dbWriter.Tell(new UserGameStateItem(_userDBID, 2, (int)gameID));
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Warning("{0} enter slot game {1} Failed : Exception {2}", _strGlobalUserID, gameID, ex);
                        }
                    }
                }
            }
   
            //방입장결과메세지를 클라이언트에 보낸다.
            GITMessage enterResponseMessage = new GITMessage((ushort)SCMSG_CODE.SC_ENTERGAME);
            enterResponseMessage.Append(enterGameSucceeded ? (byte)0 : (byte)1);
            Sender.Tell(new SendMessageToUser(enterResponseMessage, _balance));
        }
        
        private async Task procSlotGameMsg(GITMessage message, UserConnection userConn)
        {
            //게임에 입장한 상태가 아니라면
            if (userConn.GameActor == null || userConn.GameType == GAMETYPE.NONE)
            {
                if (userConn.IsHttpSession)
                    Sender.Tell("invalidaction"); //유저의 고의적인 액션    
                return;
            }

            UserBonus       waitingBonus    = pickUserBonus(userConn.GameID);
            ToUserMessage   toUserMessage   = null;
            try
            {
                toUserMessage   = await userConn.GameActor.Ask<ToUserMessage>(new FromUserMessage(_strUserID, _agentDBID, _balance, _currency, Self, message, waitingBonus, false), Constants.RemoteTimeOut);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in UserActor::procSlotGameMsg {0} gameid: {1}, message code: {2}, {3}", 
                    _strGlobalUserID, userConn.GameID, message.MsgCode, ex);
            }

            //만일 메세지수신에 성공했다면 
            if (toUserMessage != null)
            {
                await procToUserMessage(toUserMessage as ToUserMessage, waitingBonus, message, userConn);
            }
            else
            {
                if (userConn.IsHttpSession)
                    Sender.Tell("nomessagefromslotnode");
            }
        }
        
        private async Task procToUserMessage(ToUserMessage message, UserBonus askedBonus, GITMessage gameMessage, UserConnection userConn)
        {           
            //만일 게임결과처리와 관련된 메세지라면
            if (message is ToUserResultMessage)
            {
                bool isSuccess = await processResultMessage(message as ToUserResultMessage);

                //HTTP세션이고 PP게임 스핀요청이면
                if((userConn.Connection is string) && (gameMessage.MsgCode == (ushort) CSMSG_CODE.CS_PP_DOSPIN || gameMessage.MsgCode == (ushort)CSMSG_CODE.CS_PP_DOBONUS))
                {
                    //머니부족
                    if(!isSuccess)
                    {
                        //슬롯노드쪽에서 2번째 메세지로 머니부족오류상황에 대비한 메세지를 준비하여 보낸다.
                        if (message.Messages.Count >= 2 && message.Messages[1] != null)
                            Sender.Tell(message.Messages[1]);

                        GITMessage errorResultMsg = new GITMessage((ushort)CSMSG_CODE.CS_PP_NOTPROCDRESULT);
                        userConn.GameActor.Tell(new FromUserMessage(_strUserID, _agentDBID, _balance, _currency, Self, errorResultMsg, null, false));
                        return;
                    }
                }
            }

            //만일 요청한 보너스를 처리했다면 
            if (askedBonus != null && message.IsRewardedBonus)
            {
                //대기목록에서 삭제한다.
                _waitingUserBonuses.Remove(askedBonus);
                onRewardCompleted(askedBonus, message.RewardBonusMoney, message.GameID);
            }

            for (int i = 0; i < message.Messages.Count; i++)
            {
                if(message.Messages[i] != null)
                {
                    Sender.Tell(new SendMessageToUser(message.Messages[i], _balance));

                    //HTTP세션인 경우 1개의 응답메세지만을 보낼수 있다.
                    if (userConn.IsHttpSession)
                        break;
                }
            }
        }    
        
        private void addRolling(int gameID, double betMoney)
        {
            //게임롤링
            DateTime nowTime   = DateTime.Now;
            DateTime dateTime  = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day);

            if(betMoney > 0.0 && _agentDBID > 0)
                _dbWriter.Tell(new GameReportItem(gameID, _agentDBID, betMoney, dateTime));              //게임리포트갱신

            if (betMoney > 0.0)
                _dbWriter.Tell(new UserBetMoneyUpdateItem(_userDBID, betMoney));
        }
        
        private async Task<bool> processResultMessage(ToUserResultMessage resultMessage)
        {
             DateTime nowReportTime     = DateTime.UtcNow;
            DateTime nowHourReportTime  = new DateTime(nowReportTime.Year, nowReportTime.Month, nowReportTime.Day, nowReportTime.Hour, 0, 0);

            double turnover = 0.0;
            if (resultMessage is ToUserSpecialResultMessage)
            {
                ToUserSpecialResultMessage specialResultMsg = resultMessage as ToUserSpecialResultMessage;

                //보유머니를 검사한다.
                double betMoney     = Math.Round(specialResultMsg.BetMoney, 2);
                double realBet      = Math.Round(specialResultMsg.RealBet,  2);
                double winMoney     = Math.Round(specialResultMsg.WinMoney, 2);
                double turnover2    = Math.Round(specialResultMsg.TurnOver, 2);
                if (_balance.LT(realBet, _epsilion))
                    return false;

                if (specialResultMsg.IsJustBet)
                {
                    //보유머니를 갱신한다.
                    _balance -= realBet;
                    turnover  = realBet;

                    //디비를 갱신한다.
                    _dbWriter.Tell(new PlayerBalanceUpdateItem(_userDBID, -realBet));                                                      //유저머니갱신
                    _dbWriter.Tell(new ReportUpdateItem(_strUserID, _agentDBID, nowHourReportTime, realBet, 0.0, turnover2));              //리포트갱신
                    addRolling((int)specialResultMsg.GameID, realBet);                                                                      //롤링적립
                }
                else
                {
                    //보유머니를 갱신한다.
                    double beforeBalance    = _balance + (betMoney - realBet);
                    _balance                += (winMoney - realBet);
                    turnover                = realBet;

                    //디비를 갱신한다.
                    _dbWriter.Tell(new PlayerBalanceUpdateItem(_userDBID, winMoney - realBet));                                                         //유저머니갱신
                    _dbWriter.Tell(new ReportUpdateItem(_strUserID, _agentDBID, nowHourReportTime, realBet, winMoney, turnover2));                      //리포트갱신
                    _dbWriter.Tell(new GameLogItem(_agentDBID, _strUserID, (int)specialResultMsg.GameID, specialResultMsg.GameLog.GameName,             //게임로그추가
                        betMoney, winMoney, beforeBalance, _balance, specialResultMsg.GameLog.LogString,(int)specialResultMsg.BetType,nowReportTime));

                    addRolling((int)specialResultMsg.GameID, realBet);                                                                                  //롤링적립
                }
            }
            else
            {
                //보유머니를 검사한다.

                double betMoney  = Math.Round(resultMessage.BetMoney, 2);
                double winMoney  = Math.Round(resultMessage.WinMoney, 2);
                double turnover2 = Math.Round(resultMessage.TurnOver, 2);
                if (_balance.LT(betMoney, _epsilion))
                    return false;

                if (betMoney != 0.0 || winMoney != 0.0)
                {

                    //보유머니를 갱신한다.
                    double beforeBalance = _balance;
                    _balance    += (winMoney - betMoney);
                    turnover    = turnover2;

                    //디비를 갱신한다.
                    _dbWriter.Tell(new PlayerBalanceUpdateItem(_userDBID, winMoney - betMoney));                                                            //유저머니갱신
                    _dbWriter.Tell(new ReportUpdateItem(_strUserID, _agentDBID, nowHourReportTime, betMoney, winMoney, turnover2));                         //리포트갱신
                    addRolling((int)resultMessage.GameID, betMoney);                                                                                                                   //롤링적립
                    string strTableName = resultMessage.GameLog.TableName;

                    _dbWriter.Tell(new GameLogItem(_agentDBID, _strUserID, (int)resultMessage.GameID, resultMessage.GameLog.GameName,                      //게임로그추가
                    betMoney, winMoney, beforeBalance, _balance, resultMessage.GameLog.LogString,(int)resultMessage.BetType ,nowReportTime));

                    //프라그마틱플레이게임인 경우
                    //if (resultMessage.GameID > (int)GAMEID.PPGameStart && resultMessage.GameID < (int)GAMEID.PPGameEnd)
                    //    await doProcessPPTournament(betMoney, (GAMEID)resultMessage.GameID);
                }
            }
            return true;
        }

        private async Task<string> buildPPTournamentLeaderboardV3()
        {
            try
            {
                if (string.IsNullOrEmpty(_ppPromoStatus.TournamentLeaderboards))
                    return "{\"error\":0,\"description\":\"OK\",\"leaderboards\":[]}";
                
                string strLeaderboard   = _ppPromoStatus.TournamentLeaderboards;
                JToken leaderBoard      = JToken.Parse(strLeaderboard);
                JArray tourLeaders      = leaderBoard["leaderboards"] as JArray;

                for (int i = 0; i < tourLeaders.Count; i++)
                {
                    int         tournamentID        = (int)tourLeaders[i]["tournamentID"];
                    string      strTournamentBetKey = string.Format("tournament_{0}_scores", tournamentID);
                    RedisValue  scoreValue          = await RedisDatabase.RedisCache.HashGetAsync(strTournamentBetKey, _strGlobalUserID);
                    
                    if (scoreValue.HasValue)
                    {
                        tourLeaders[i]["index"] = (JToken)501;
                        JArray items        = tourLeaders[i]["items"] as JArray;
                        long userTourScore  = (long)scoreValue;

                        items.Add(string.Format("3384394|*****6427|{0}|5175.0|5175.0|5175.0|IDR|ID",userTourScore));
                        items.Add(string.Format("3384395|*****6196|{0}|1000.0|1000.0|1000.0|KRW|KR",userTourScore));
                        items.Add(string.Format("3384396|*****7703|{0}|5520.0|5520.0|5520.0|IDR|ID",userTourScore));
                    }
                }
                
                return leaderBoard.ToString();
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::buildPPTournamentLeaderboardV3 {0}", ex.ToString());

                return "{\"error\":0,\"description\":\"OK\",\"leaderboards\":[]}";
            }
        }

        private async Task<string> buildPPTournamentLeaderboard()
        {
            try
            {
                string strLeaderboard   = await buildPPTournamentLeaderboardV3();
                JToken leaderBoard      = JToken.Parse(strLeaderboard);
                JArray tourLeaders      = leaderBoard["leaderboards"] as JArray;

                for (int i = 0; i < tourLeaders.Count; i++)
                {
                    JArray items    = tourLeaders[i]["items"] as JArray;
                    JArray v0Items  = new JArray();

                    for (int j = 0; j < items.Count; j++)
                    {
                        string[] strParts = ((string)items[j]).Split(new string[1] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        JObject voItem = new JObject();
                        voItem["position"]                      = int.Parse(strParts[0]);
                        voItem["playerID"]                      = strParts[1];
                        voItem["score"]                         = double.Parse(strParts[2]);
                        voItem["scoreBet"]                      = double.Parse(strParts[3]);
                        voItem["effectiveBetForFreeRounds"]     = double.Parse(strParts[4]);
                        voItem["effectiveBetForBetMultiplier"]  = double.Parse(strParts[5]);
                        voItem["memberCurrency"]                = strParts[6];
                        voItem["countryID"]                     = strParts[7];
                        v0Items.Add(voItem);
                    }

                    tourLeaders[i]["items"] = v0Items;
                }

                return leaderBoard.ToString();
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::buildPPTournamentLeaderboardV3 {0}", ex.ToString());
                
                return "{\"error\":0,\"description\":\"OK\",\"leaderboards\":[]}";
            }
        }

        private string convertRaceWinnerInfoToV0(string raceWinnersV2)
        {
            JToken jtoken = JToken.Parse(raceWinnersV2);
            JArray jarray1 = jtoken["winners"] as JArray;
            for (int i = 0; i < jarray1.Count; i++)
            {
                JArray jarray2 = jarray1[i]["items"] as JArray;
                for (int j = 0; j < jarray2.Count; j++)
                {
                    string[] strArray = ((string)jarray2[j]).Split(new string[1] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    JObject jobject = JObject.Parse("{}");
                    jobject["prizeID"]                      = int.Parse(strArray[0]);
                    jobject["playerID"]                     = strArray[1];
                    jobject["bet"]                          = double.Parse(strArray[2]);
                    jobject["effectiveBetForBetMultiplier"] = double.Parse(strArray[3]);
                    jobject["effectiveBetForFreeRounds"]    = double.Parse(strArray[4]);
                    jobject["memberCurrency"]               = strArray[5];
                    jobject["countryID"]                    = strArray[6];
                    
                    jarray2[j] = jobject;
                }
            }
            return jtoken.ToString();
        }

        private async Task<string> buildPPTourScores(GITMessage requestMessage)
        {
            try
            {
                int tourCount       = (int)requestMessage.Pop();
                List<int> tourIds   = new List<int>();
                for (int i = 0; i < tourCount; ++i)
                    tourIds.Add((int)requestMessage.Pop());

                JObject response = JObject.Parse("{}");
                response["description"] = "OK";
                response["error"]       = 0;

                JArray scores = new JArray();
                for (int i = 0; i < tourCount; i++)
                {
                    string      strRedisKey = string.Format("tournament_{0}_scores", tourIds[i]);
                    RedisValue  scoreValue  = await RedisDatabase.RedisCache.HashGetAsync(strRedisKey, _strGlobalUserID);

                    if (!scoreValue.IsNullOrEmpty)
                    {
                        JObject tour = new JObject();
                        tour["position"]        = 3384347;
                        tour["score"]           = (long)scoreValue;
                        tour["tournamentID"]    = tourIds[i];
                    }
                }

                if (scores.Count > 0)
                    response["scores"] = scores;

                return response.ToString();
            }
            catch (Exception ex1)
            {
                return "{\"error\":0,\"description\":\"OK\"}";
            }
        }

        private GITMessage buildPPRaceWinners(GITMessage requestMessage)
        {
            try
            {
                GITMessage respondMessage = new GITMessage((ushort)CSMSG_CODE.CS_PP_PROMORACEWINNER);
                respondMessage.Append(convertRaceWinnerInfoToV0(_ppPromoStatus.RaceWinners));
                return respondMessage;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::buildPPRaceWinners {0}", ex.ToString());
                return null;
            }
        }

        private GITMessage buildPPRaceWinnersV2(GITMessage requestMessage)
        {
            try
            {
                GITMessage respondMessage = new GITMessage((ushort)CSMSG_CODE.CS_PP_PROMORACEWINNER);
                respondMessage.Append(_ppPromoStatus.RaceWinners);
                return respondMessage;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::buildPPRaceWinners {0}", ex.ToString());
                return null;
            }
        }

        private async Task<string> buildActivePromos()
        {
            try
            {
                if (string.IsNullOrEmpty(_ppPromoStatus.ActivePromos))
                    return "{\"error\":0,\"description\":\"OK\",\"serverTime\":1676462782,\"tournaments\":[],\"races\":[]}";

                JToken activePromos         = JToken.Parse(_ppPromoStatus.ActivePromos);
                JArray activeTournaments    = activePromos["tournaments"] as JArray;
                for (int i = 0; i < activeTournaments.Count; i++)
                {
                    int tournamentID    = (int)activeTournaments[i]["id"];
                    string strKey       = string.Format("tournament_{0}_optin", tournamentID);

                    if (await RedisDatabase.RedisCache.HashExistsAsync(strKey, _strGlobalUserID))
                        activeTournaments[i]["optin"] = true;
                    else
                        activeTournaments[i]["optin"] = false;
                }

                JArray activeRaces = activePromos["races"] as JArray;
                for (int i = 0; i < activeRaces.Count; i++)
                {
                    int raceID      = (int)activeRaces[i]["id"];
                    string strKey   = string.Format("race_{0}_optin", raceID);

                    if (await RedisDatabase.RedisCache.HashExistsAsync(strKey, _strGlobalUserID))
                        activeRaces[i]["optin"] = true;
                    else
                        activeRaces[i]["optin"] = false;
                }

                activePromos["serverTime"] = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                return JsonConvert.SerializeObject(activePromos);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::buildActivePromos {0}", ex.ToString());
                return _ppPromoStatus.ActivePromos;
            }
        }

        private async Task procOptIn(bool isTournament, int promoID)
        {
            try
            {
                string strHashKey = isTournament ? string.Format("tournament_{0}_optin", promoID) : string.Format("race_{0}_optin", promoID);
                await RedisDatabase.RedisCache.HashSetAsync(strHashKey, _strGlobalUserID, 0);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::procOptIn {0}", ex.ToString());
            }
        }

        private async Task<GITMessage> onPromoActive(GITMessage message)
        {
            GITMessage respondMessage = new GITMessage((ushort)CSMSG_CODE.CS_PP_PROMOACTIVE);
            try
            {
                string strGameSymbol = (string)message.Pop();
                
                string str = await buildActivePromos();
                respondMessage.Append(str);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::onPromoActive {0}", ex.ToString());
            }

            return respondMessage;
        }

        private async Task procPPPromoMsg(GITMessage message, UserConnection userConn)
        {
            GITMessage respondMessage = null;
            if (_currency != (int)CurrencyEnum.KRW)
            {
                respondMessage = new GITMessage((ushort)message.MsgCode);
                respondMessage.Append("{\"error\":0,\"description\":\"OK\"}");
                Sender.Tell(new SendMessageToUser(respondMessage, _balance));
                return;
            }

            switch ((CSMSG_CODE)message.MsgCode)
            {
                case CSMSG_CODE.CS_PP_PROMOSTART:
                    respondMessage = await onPromoActive(message);
                    break;
                case CSMSG_CODE.CS_PP_PROMOTOURDETAIL:
                    respondMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_PROMOTOURDETAIL);
                    respondMessage.Append(_ppPromoStatus.TournamentDetails);
                    break;
                case CSMSG_CODE.CS_PP_PROMORACEDETAIL:
                    respondMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_PROMORACEDETAIL);
                    respondMessage.Append(_ppPromoStatus.RaceDetails);
                    break;
                case CSMSG_CODE.CS_PP_PROMOTOURLEADER:
                    respondMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_PROMOTOURLEADER);
                    respondMessage.Append(await buildPPTournamentLeaderboard());
                    break;
                case CSMSG_CODE.CS_PP_PROMORACEPRIZES:
                    respondMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_PROMORACEPRIZES);
                    respondMessage.Append(_ppPromoStatus.RacePrizes);
                    break;
                case CSMSG_CODE.CS_PP_PROMORACEWINNER:
                    respondMessage = buildPPRaceWinners(message);
                    break;
                case CSMSG_CODE.CS_PP_PROMOV3TOURLEADER:
                    respondMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_PROMORACEPRIZES);
                    respondMessage.Append(await buildPPTournamentLeaderboardV3());
                    break;
                case CSMSG_CODE.CS_PP_PROMOV2RACEWINNER:
                    respondMessage = buildPPRaceWinnersV2(message);
                    break;
                case CSMSG_CODE.CS_PP_PROMOTOUROPTIN:
                case CSMSG_CODE.CS_PP_PROMORACEOPTIN:
                    int promoID = (int)message.Pop();
                    if (message.MsgCode == (ushort)CSMSG_CODE.CS_PP_PROMOTOUROPTIN)
                    {
                        respondMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_PROMOTOUROPTIN);
                        await procOptIn(true, promoID);
                    }
                    else
                    {
                        respondMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_PROMORACEOPTIN);
                        await procOptIn(false, promoID);
                    }
                    respondMessage.Append("{\"error\":0,\"description\":\"OK\"}");
                    break;
                case CSMSG_CODE.CS_PP_PROMOSCORES:
                    respondMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_PROMOTOURSCORE);
                    respondMessage.Append(await buildPPTourScores(message));
                    break;
                case CSMSG_CODE.CS_PP_GETMINILOBBY:
                    respondMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_GETMINILOBBY);
                    respondMessage.Append(_ppPromoStatus.MiniLobbyGames);
                    break;
            }

            if (respondMessage != null)
                Sender.Tell(new SendMessageToUser(respondMessage, _balance));
        }
        #endregion

        private async Task procCQ9ExitGameResponse(CQ9ExitGameResponse response)
        {
            try
            {
                if (response.ResultMsg == null)
                    return;

                await processResultMessage(response.ResultMsg);
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::procCQ9ExitGameResponse {0}", ex);
            }
        }

        private async Task procAmaticExitGameResponse(AmaticExitResponse response)
        {
            try
            {
                if (response.ResultMsg == null)
                    return;

                await processResultMessage(response.ResultMsg);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::procAmaticExitGameResponse {0}", ex);
            }
        }

        private async Task replaceSlotGameNode(string strSlotNodePath, UserConnection userConn)
        {
            //해당 슬롯게임노드가 shutdown 되므로 다른 노드에 가입한다.
            int remainServerCount = 0;
            try
            {
                var routees = await Context.System.ActorSelection(Constants.SlotGameRouterPath).Ask<Routees>(new GetRoutees());
                foreach (Routee routee in routees.Members)
                    remainServerCount++;

                _logger.Info("{0} Exiting from slot game node {1}", _strUserID, strSlotNodePath);
                ExitGameResponse response = await userConn.GameActor.Ask<ExitGameResponse>(new ExitGameRequest(_strUserID, _agentDBID, _currency, _balance, false, remainServerCount > 0), TimeSpan.FromSeconds(5));
                if(response is CQ9ExitGameResponse)
                    await procCQ9ExitGameResponse(response as CQ9ExitGameResponse);
                else if (response is AmaticExitResponse)
                    await procAmaticExitGameResponse(response as AmaticExitResponse);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in UserActor::onSlotGameServerShuttingdown {0}", ex);
            }

            try
            {
                _logger.Info("{0} Reentering slot game node", _strUserID);
                if (remainServerCount == 0)
                {
                    _logger.Info("{0} no more slot game node found", _strUserID);
                    if (userConn.IsHttpSession)
                        Self.Tell(new HttpSessionClosed(userConn.Connection as string));
                    else
                        (userConn.Connection as IActorRef).Tell("closeConnection");

                    userConn.GameActor  = null;
                    userConn.GameType   = GAMETYPE.NONE;
                    userConn.GameID     = 0;
                    return;
                }

                //다른 슬롯게임노드에 입장한다.
                EnterGameRequest requestMsg     = new EnterGameRequest(userConn.GameID, _agentDBID, _strUserID, _currency, Self, false);
                EnterGameResponse responseMsg   = await Context.System.ActorSelection(Constants.SlotGameRouterPath).Ask<EnterGameResponse>(requestMsg, Constants.RemoteTimeOut);

                //게임입장성공
                if (responseMsg.Ack == 0)
                    userConn.GameActor = responseMsg.GameActor;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in UserActor::replaceSlotGameNode {0}", ex);
            }
        }
        
        private async Task onSlotGameServerShuttingdown(SlotGamesNodeShuttingdown message)
        {
            if (_userConnections.Count == 0)
                return;

            for(int i = 0; i < _userConnections.Count;i++)
            {
                object          connection  = _userConnections.Keys.ElementAt(i);
                UserConnection  userConn    = _userConnections[connection];

                if (userConn.GameActor == null || userConn.GameType == GAMETYPE.NONE)
                    continue;

                if (!userConn.GameActor.Path.ToString().Contains(message.Path))
                    continue;

                await replaceSlotGameNode(message.Path, userConn);
            }            
        }

        #region 보너스처리관련 함수들
        private void onRewardCompleted(UserBonus completedBonus, double rewardedMoney, int gameID)
        {
            if(completedBonus is UserRangeOddEventBonus)
            {
                fetchNewUserRangeEvent(rewardedMoney, gameID);
            }
        }
        
        private void onUserRangeOddEvent(UserRangeOddEventItem userRangeEventItem)
        {
            if (_userRangeOddEventItem != null)
                return;

            _userRangeOddEventItem = userRangeEventItem;
            _waitingUserBonuses.Add(new UserRangeOddEventBonus(userRangeEventItem.BonusID, userRangeEventItem.MinOdd, userRangeEventItem.MaxOdd,userRangeEventItem.MaxBet));
        }
        
        private void onUserEventCancelled(UserEventCancelled message)
        {
            if (_userRangeOddEventItem == null || _userRangeOddEventItem.BonusID != message.BonusID)
                return;

            int cnt = 0;
            while (cnt < _waitingUserBonuses.Count && (_waitingUserBonuses[cnt].BonusType != UserBonusType.USEREVENT || _waitingUserBonuses[cnt].BonusID != message.BonusID))
                cnt++;

            if (cnt < _waitingUserBonuses.Count)
                _waitingUserBonuses.RemoveAt(cnt);

            _dbReader.Tell(new ClaimedUserRangeEventMessage(_userRangeOddEventItem.BonusID, _agentDBID, _strUserID, 0.0, ""));
            _userRangeOddEventItem = null;
        }

        private void fetchNewUserRangeEvent(double rewardedMoney, int gameID)
        {
            _dbWriter.Tell(new ClaimedUserRangeEventItem(_userRangeOddEventItem.BonusID, rewardedMoney, gameID.ToString(), DateTime.UtcNow));
            _dbReader.Tell(new ClaimedUserRangeEventMessage(_userRangeOddEventItem.BonusID,_agentDBID ,_strUserID, rewardedMoney, gameID.ToString()));
            _userRangeOddEventItem = null;
        }

        private UserBonus pickUserBonus(int gameID)
        {       
            //PP게임에 한해서만
            if(gameID >= (int) GAMEID.PPGameStart && gameID <= (int) GAMEID.PPGameEnd)
            {
                if (_userRacePrizeItem != null)
                    return _userRacePrizeItem;
            }

            for (int i = 0; i < _waitingUserBonuses.Count; i++)
            {
                if (_waitingUserBonuses[i] is UserRangeOddEventBonus)
                    return _waitingUserBonuses[i];
            }
            return null;
        }
        #endregion

        #region Messages
        public class UserLoggedIn
        {
        }
        #endregion

        //유저의 연결객체(tcp, websocket, http session)
        public class UserConnection
        {
            public object       Connection      { get; set; } //socket연결: IActorRef, http세션: string
            public IActorRef    GameActor       { get; set; } //입장한 게임액터(null: 게임에 입장하지 않은 상태)
            public int          GameID          { get; set; } //입장한 게임아이디(0: 게임에 입장하지 않은 상태)
            public GAMETYPE     GameType        { get; set; } //입장한 게임유형
            public DateTime     LastActiveTime  { get; set; } //유저가 마지막으로 서버와 접촉한 시간

            public bool IsHttpSession
            {
                get
                {
                    return Connection is string;
                }
            }
            
            public UserConnection(object connection)
            {
                this.Connection     = connection;
                this.GameActor      = null;
                this.GameID         = 0;
                this.GameType       = GAMETYPE.NONE;
                this.LastActiveTime = DateTime.Now;
            }

            public void resetGame()
            {
                this.GameActor      = null;
                this.GameID         = 0;
                this.GameType       = GAMETYPE.NONE;
            }
        }
        public class PPPromoStatus
        {
            public string   ActivePromos            { get; set; }
            public string   TournamentDetails       { get; set; }
            public string   RaceDetails             { get; set; }
            public string   RaceWinners             { get; set; }
            public string   RacePrizes              { get; set; }
            public int      OpenTournamentID        { get; set; }
            public double   OpenTournamentMinBet    { get; set; }
            public string   TournamentLeaderboards  { get; set; }
            public string   MiniLobbyGames          { get; set; }

            public PPPromoStatus(string strActivePromos,string strTournamentDetails,string strRaceDetails,string strRaceWinners,string strRacePrizes,int openTournamentID,double openTournamentMinBet,string tournamentLeaderboards,string miniLobbyGames)
            {
                ActivePromos            = strActivePromos;
                TournamentDetails       = strTournamentDetails;
                RaceDetails             = strRaceDetails;
                RaceWinners             = strRaceWinners;
                RacePrizes              = strRacePrizes;
                OpenTournamentID        = openTournamentID;
                OpenTournamentMinBet    = openTournamentMinBet;
                TournamentLeaderboards  = tournamentLeaderboards;
                MiniLobbyGames          = miniLobbyGames;
            }
        }
        public enum CurrencyEnum
        {
            USD = 0,
            EUR = 1,
            TND = 2,
            KRW = 3,
            GMD = 4,
            CNY = 5,
        }
    }
}
