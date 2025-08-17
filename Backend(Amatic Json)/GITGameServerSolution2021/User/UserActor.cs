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
using Newtonsoft.Json;

namespace CommNode
{
    //로그인된 사용자를 표현하는 클라스 
    public class UserActor : ReceiveActor
    {
        #region 사용자정보
        private long                    _userDBID               = 0;
        private string                  _strUserID              = "";
        private string                  _strUserToken           = "";
        private double                  _balance                = 0.0;
        private Currencies            _currency               = Currencies.USD;
        private string                  _agentName              = "";
        private int                     _agentDBID              = 0;
        private string                  _agentIDs               = "";
        private List<int>               _agentHierachy          = null;
        private long                    _lastScoreID            = 0;
        private string                  _ipAddress              = "";
        private bool                    _isCheckedScoreCache    = false;
        private double                  _rollingPer             = 0.0;
        private int                     _companyID              = 0;
        private Dictionary<int, double> _agentRollingFees       = new Dictionary<int, double>();

        //Added by Foresight(2018.09.15)
        private string _strCountry = null;
        #endregion

        #region 유저의 상태변수들       
        private bool            _userDisconnected = false;
        private PlatformTypes   _platformType;
        private Dictionary<object, UserConnection> _userConnections = new Dictionary<object, UserConnection>();
        #endregion

        #region 사용자의 각종 보너스정보
        private UserRangeOddEventItem   _userRangeOddEventItem;
        private List<UserBonus>         _waitingUserBonuses = new List<UserBonus>();
        #endregion

        private IActorRef                       _dbReader     = null;
        private IActorRef                       _dbWriter     = null;
        private IActorRef                       _redisWriter  = null;
        private readonly ILoggingAdapter        _logger       = Logging.GetLogger(Context);
        protected static RealExtensions.Epsilon _epsilion     = new RealExtensions.Epsilon(0.001);

        private bool            _isAgentMustLose    = false;
        private bool            _isUserMustLose     = false;
        private ICancelable     _conCheckCancel     = null;

        public UserActor(CreateNewUserMessage message)
        {
            _dbReader       = message.DBReader;
            _dbWriter       = message.DBWriter;
            _redisWriter    = message.RedisWriter;
            _userDBID       = message.LoginResponse.UserDBID;
            _strUserID      = message.LoginResponse.UserID;
            _balance        = message.LoginResponse.Balance;
            _currency       = message.LoginResponse.Currency;
            _agentName      = message.LoginResponse.AgentName;
            _agentDBID      = message.LoginResponse.AgentID;
            _agentIDs       = message.LoginResponse.AgentIDs;

            _lastScoreID    = message.LoginResponse.LastScoreID;
            _ipAddress      = message.LoginResponse.IPAddress;
            _strUserToken   = message.LoginResponse.UserToken;
            _platformType   = message.PlatformType;
            _strCountry     = message.LoginResponse.Country;
            _rollingPer     = message.LoginResponse.RollingFee;

            _userConnections.Add(message.Connection, new UserConnection(message.Connection));
            _agentHierachy = new List<int>();

            string[] strSplits = _agentIDs.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (strSplits != null && strSplits.Length > 0)
            {
                for (int i = 0; i < strSplits.Length; i++)
                {
                    int agentDBID = 0;
                    if (int.TryParse(strSplits[i], out agentDBID))
                        _agentHierachy.Add(agentDBID);
                }
            }

            if (_agentHierachy.Count > 1)
                _companyID = _agentHierachy[1];

            ReceiveAsync<UserLoggedIn>              (onUserLoginSucceeded);
            ReceiveAsync<FromConnRevMessage>        (onProcMessage);

            //소켓연결 추가/삭제처리
            Receive<SocketConnectionAdded>          (onProcSocketConnectionAdded);
            ReceiveAsync<SocketConnectionClosed>    (onProcSocketConnectionClosed);

            ReceiveAsync<AddScoreMessage>           (onAddScore);
            Receive<UserRangeOddEventItem>          (onUserRangeOddEvent);

            Receive<QuitUserMessage>                (onForceLogoutMessage);
            Receive<string>                         (onCommand);
            ReceiveAsync<SlotGamesNodeShuttingdown> (onSlotGameServerShuttingdown);
            Receive<UserRollingPerUpdated>          (_ =>
            {
                _rollingPer = _.RollingPer;
            });
            Receive<AgentRollingPerUpdated>         (_ =>
            {
                if (_agentRollingFees.ContainsKey(_.AgentID))
                    _agentRollingFees[_.AgentID] = _.RollingPer;
            });
        }

        public static Props Props(CreateNewUserMessage message)
        {
            return Akka.Actor.Props.Create(() => new UserActor(message));
        }
        
        protected override void PreStart()
        {
            Self.Tell(new UserLoggedIn());                                                  // 로그인성공후에 보낼 메세지들을 먼저 보낸다.
            base.PreStart();
        }
        
        protected override void PostStop()
        {
            if (_conCheckCancel != null)
                _conCheckCancel.Cancel();

            base.PostStop();
        }

        private async Task onUserLoginSucceeded(UserLoggedIn message)
        {
            try
            {
                _agentRollingFees           = await _dbReader.Ask<Dictionary<int, double>>(new GetAgentRollingFees(_agentHierachy));

                //유저온라인상태 갱신
                _dbWriter.Tell(new UserLoginStateItem(_userDBID, (int)_platformType));
                //유저에게 수여된 각종 보너스정보들을 디비에서 읽어서 전송한다.
                _dbReader.Tell(new GetUserBonusItems(_strUserID));

                _logger.Info("{0} has been logged in successfully {1}", _strUserID, _platformType);

                //스코캐시를 검사한다.
                if (!_isCheckedScoreCache)
                    await checkScoreCache();

                _conCheckCancel = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(1000, 1000, Self, "checkConn", Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::onUserLoginSucceeded {0}", ex);
            }
        }

        //유저가 로그인 되는 동안 적용된 스코변화를 감지하여 적용한다.
        private async Task checkScoreCache()
        {
            _isCheckedScoreCache = true;
            SortedList<long, SetScoreData> scoreCacheData = null;
            try
            {
                scoreCacheData = await Context.System.ActorSelection("/user/scoreCacheActor").Ask<SortedList<long, SetScoreData>>(new GetScoreCacheMessage(this._strUserID), TimeSpan.FromSeconds(5));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::checkScoreCache {0} {1}", ex);
            }
            if (scoreCacheData == null)
                return;

            try
            {
                if (scoreCacheData.Count > 0)
                {
                    foreach (KeyValuePair<long, SetScoreData> pair in scoreCacheData)
                    {
                        SetScoreData scoreData = pair.Value;
                        if (_lastScoreID >= scoreData.ID)
                            continue;

                        addScore(scoreData.ID, scoreData.Score);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::checkScoreCache {0} {1}", ex);
            }
        }

        #region 각종 사건처리부
        private void onForceLogoutMessage(QuitUserMessage _)
        {
            GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_FORCEOUTUSER);

            //모든 연결들에 통지한다.
            for (int i = 0; i < _userConnections.Count; i++)
            {
                object key = _userConnections.Keys.ElementAt(i);
                if (!(key is string))
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

            }
        }
        
        private void addScore(long scoreID, double score)
        {
            if (scoreID <= _lastScoreID)
                return;

            //게임머니갱신
            _lastScoreID = scoreID;
            _balance += score;

            //게임로그추가
            _dbWriter.Tell(new GameLogItem(_strUserID, 0, "setscore", "", 0.0, 0.0, _balance - score, _balance, "", DateTime.UtcNow));

        }
        
        private async Task onAddScore(AddScoreMessage addScoreMsg)
        {
            if (!_isCheckedScoreCache)
                await checkScoreCache();

            addScore(addScoreMsg.ScoreID, addScoreMsg.AddedScore);
        }

        private void onProcSocketConnectionAdded(SocketConnectionAdded _)
        {
            if (_userConnections.ContainsKey(Sender))
                return;

            _userConnections.Add(Sender, new UserConnection(Sender));
        }

        private async Task onClosedUserConnection(UserConnection userConn)
        {
            int lastGameID = 0;
            if (userConn.GameActor != null)
            {
                if (userConn.GameType >= GameProviders.PP && userConn.GameType <= GameProviders.COUNT)
                {
                    ExitGameResponse response = null;
                    try
                    {
                        response = await userConn.GameActor.Ask<ExitGameResponse>(new ExitGameRequest(_strUserID, _companyID, _balance, _currency, true, false), Constants.RemoteTimeOut);
                        if(response is AmaticExitResponse)
                            await procAmaticExitGameResponse(response as AmaticExitResponse);
                    }
                    catch (Exception ex)
                    {
                        _logger.Warning("{0} exit game {1} Failed : Exception {2}", _strUserID, userConn.GameID, ex);
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

                //Changed by Foresight(2019.09.27)
                try
                {
                    await RedisDatabase.RedisCache.HashDeleteAsync("onlineusers", new RedisValue[] { _strUserID, _strUserID + "_path" });
                    await RedisDatabase.RedisCache.KeyDeleteAsync(_strUserID + "_tokens");

                }
                catch (Exception ex)
                {
                    _logger.Error("Exception has been occured in UserActor::onProcSocketConnectionClosed User ID: {0} {1}", _strUserID, ex);
                }

                _userDisconnected = true;
                _logger.Info("{0} user has been logged out", _strUserID);
                Context.Stop(Self);
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
            GITMessage message = fromConnRevMsg.Message;
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
            else if(message.MsgCode >= (ushort)CSMSG_CODE.CS_AMATICSLOTGAMESTART && message.MsgCode <= (ushort)CSMSG_CODE.CS_AMATICSLOTGAMEEND)
            {
                if(message.MsgCode != (ushort)CSMSG_CODE.CS_AMATIC_DOHEARTBEAT)
                    userConn.LastActiveTime = DateTime.Now;
                await procSlotGameMsg(message, userConn);
            }
            else
            {
                _logger.Warning("Unknown paket received from {0} Message code:{1} User ID:{2}", _ipAddress, message.MsgCode, _strUserID);
            }
        }

        private async Task procEnterGame(GITMessage message, UserConnection userConn)
        {
            if(!DBMonitorSnapshot.Instance.IsServerUp)
            {
                _logger.Warning("{0} tried to enter game while service maintenance", _strUserID);
                Sender.Tell("closeConnection");
                return;
            }
            int      gameID     = (int)(ushort)message.Pop();
            GameProviders gameType   = DBMonitorSnapshot.Instance.getGameType(gameID);

            //등록된 게임아이디가 아님
            if (gameType == GameProviders.NONE)
            {
                _logger.Warning("{0} tried to enter game for not registered game id {1}", _strUserID, (int)gameID);
                Sender.Tell("closeConnection");
                return;
            }

            //이미 게임에 입장함
            if (userConn.GameActor != null && userConn.GameID != gameID)
            {
                //이미 다른 게임에 입장하였다면
                _logger.Warning("{0} tried to enter game while it has already been entered to other game", _strUserID);
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

                    enterGameSucceeded = true;
                }
                else
                {
                    if (gameType > GameProviders.NONE && gameType <= GameProviders.COUNT)
                    {
                        try
                        {
                            EnterGameRequest requestMsg = new EnterGameRequest(gameID, _strUserID, Self);
                            EnterGameResponse responseMsg = await Context.System.ActorSelection(Constants.SlotGameRouterPath).Ask<EnterGameResponse>(requestMsg, Constants.RemoteTimeOut);
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
                            _logger.Warning("{0} enter slot game {1} Failed : Exception {2}", _strUserID, gameID, ex);
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
            if (userConn.GameActor == null || userConn.GameType == GameProviders.NONE)
            {
                if (userConn.IsHttpSession)
                    Sender.Tell("invalidaction"); //유저의 고의적인 액션    
                return;
            }

            UserBonus       waitingBonus    = pickUserBonus(userConn.GameID);
            ToUserMessage   toUserMessage   = null;
            try
            {
                bool isMustLose = _isAgentMustLose || _isUserMustLose;
                toUserMessage   = await userConn.GameActor.Ask<ToUserMessage>(new FromUserMessage(_strUserID, _companyID, _balance,_currency, Self, message, waitingBonus), Constants.RemoteTimeOut);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in UserActor::procSlotGameMsg {0} gameid: {1}, message code: {2}, {3}", 
                    _strUserID, userConn.GameID, message.MsgCode, ex);
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
                await processResultMessage(message as ToUserResultMessage);
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
            //1. 유저롤링
            double rollingPoint = betMoney * _rollingPer / 100.0;
            if (rollingPoint > 0.0)
                _dbWriter.Tell(new UserRollingAdded(_userDBID, rollingPoint));

            double prevRollingPer = _rollingPer;

            //2. 업체롤링(운영본사까지만 롤링을 적립한다..)
            for(int i = _agentHierachy.Count -1; i >= 1; i--)
            {
                int     agentID     = _agentHierachy[i];
                double  rollPercent = 0.0;

                if (_agentRollingFees.ContainsKey(agentID))
                    rollPercent = _agentRollingFees[agentID];

                double realRollPercent = rollPercent - prevRollingPer;
                if (realRollPercent < 0.0)
                    realRollPercent = 0.0;

                double agentRollingPoint = betMoney * realRollPercent / 100.0;
                if (agentRollingPoint > 0.0)
                    _dbWriter.Tell(new AgentRollingAdded(agentID, agentRollingPoint));

                if (rollPercent > prevRollingPer)
                    prevRollingPer = rollPercent;
            }

            //게임롤링
            DateTime nowTime   = DateTime.Now;
            DateTime dateTime  = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day);

            if(betMoney > 0.0 && _companyID > 0)
                _dbWriter.Tell(new GameReportItem(gameID, _companyID, betMoney, dateTime));              //게임리포트갱신

            if (betMoney > 0.0)
                _dbWriter.Tell(new UserBetMoneyUpdateItem(_userDBID, betMoney));
        }
        
        private async Task<bool> processResultMessage(ToUserResultMessage resultMessage)
        {
            DateTime nowReportTime     = DateTime.UtcNow;
            DateTime nowHourReportTime = new DateTime(nowReportTime.Year, nowReportTime.Month, nowReportTime.Day, nowReportTime.Hour, 0, 0);

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
#if FORTEST
                    //테스트유저방식인 경우 리포트갱신을 하지 않는다
#else
                    _dbWriter.Tell(new ReportUpdateItem(_strUserID, _agentDBID, nowHourReportTime, realBet, 0.0, turnover2));              //리포트갱신
                    addRolling((int)specialResultMsg.GameID, realBet);    //롤링적립
#endif
                }
                else
                {
                    //보유머니를 갱신한다.
                    double beforeBalance     = _balance + (betMoney - realBet);
                    _balance                += (winMoney - realBet);
                    turnover                 = realBet;

                    //디비를 갱신한다.
                    _dbWriter.Tell(new PlayerBalanceUpdateItem(_userDBID, winMoney - realBet));                                                   //유저머니갱신
#if FORTEST
                    //테스트유저방식인 경우 리포트갱신과 게임로그추가를 하지 않는다
#else
                    _dbWriter.Tell(new ReportUpdateItem(_strUserID, _agentDBID, nowHourReportTime, realBet, winMoney, turnover2));            //리포트갱신
                    _dbWriter.Tell(new GameLogItem(_strUserID, (int)specialResultMsg.GameID, specialResultMsg.GameLog.GameName, specialResultMsg.GameLog.TableName,                 //게임로그추가
                        betMoney, winMoney, beforeBalance, _balance, specialResultMsg.GameLog.LogString, nowReportTime));

                    addRolling((int)specialResultMsg.GameID, realBet);    //롤링적립
#endif
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

                //보유머니를 갱신한다.
                double beforeBalance = _balance;
                _balance  += (winMoney - betMoney);
                turnover   = turnover2;

                //디비를 갱신한다.
                _dbWriter.Tell(new PlayerBalanceUpdateItem(_userDBID, winMoney - betMoney));                                                            //유저머니갱신
                _dbWriter.Tell(new ReportUpdateItem(_strUserID, _agentDBID, nowHourReportTime, betMoney, winMoney, turnover2));                         //리포트갱신
                addRolling((int)resultMessage.GameID, betMoney);                                                                                                                   //롤링적립

                string strTableName = resultMessage.GameLog.TableName;

                _dbWriter.Tell(new GameLogItem(_strUserID, (int)resultMessage.GameID, resultMessage.GameLog.GameName, strTableName,                      //게임로그추가
                betMoney, winMoney, beforeBalance, _balance, resultMessage.GameLog.LogString, nowReportTime));
            }
            return true;
        }
        #endregion

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
                ExitGameResponse response = await userConn.GameActor.Ask<ExitGameResponse>(new ExitGameRequest(_strUserID, _companyID, _balance, _currency, false, remainServerCount > 0), TimeSpan.FromSeconds(5));
                if(response is AmaticExitResponse)
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
                    if (!userConn.IsHttpSession)
                        (userConn.Connection as IActorRef).Tell("closeConnection");

                    userConn.GameActor  = null;
                    userConn.GameType   = GameProviders.NONE;
                    userConn.GameID     = 0;
                    return;
                }

                //다른 슬롯게임노드에 입장한다.
                EnterGameRequest requestMsg   = new EnterGameRequest(userConn.GameID, _strUserID, Self, false);
                EnterGameResponse responseMsg = await Context.System.ActorSelection(Constants.SlotGameRouterPath).Ask<EnterGameResponse>(requestMsg, Constants.RemoteTimeOut);

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

                if (userConn.GameActor == null || userConn.GameType == GameProviders.NONE)
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
        
        private void fetchNewUserRangeEvent(double rewardedMoney, int gameID)
        {
            _dbWriter.Tell(new ClaimedUserRangeEventItem(_userRangeOddEventItem.BonusID, rewardedMoney, gameID.ToString(), DateTime.UtcNow));
            _dbReader.Tell(new ClaimedUserRangeEventMessage(_userRangeOddEventItem.BonusID, _strUserID, rewardedMoney, gameID.ToString()));
            _userRangeOddEventItem = null;
        }

        private UserBonus pickUserBonus(int gameID)
        {       
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
            public object           Connection      { get; set; } //socket연결: IActorRef, http세션: string
            public IActorRef        GameActor       { get; set; } //입장한 게임액터(null: 게임에 입장하지 않은 상태)
            public int              GameID          { get; set; } //입장한 게임아이디(0: 게임에 입장하지 않은 상태)
            public GameProviders    GameType        { get; set; } //입장한 게임유형
            public DateTime         LastActiveTime  { get; set; } //유저가 마지막으로 서버와 접촉한 시간

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
                this.GameType       = GameProviders.NONE;
                this.LastActiveTime = DateTime.Now;
            }

            public void resetGame()
            {
                this.GameActor      = null;
                this.GameID         = 0;
                this.GameType       = GameProviders.NONE;
            }
        }
    }
}
