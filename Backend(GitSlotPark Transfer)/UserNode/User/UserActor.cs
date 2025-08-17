using Akka.Actor;
using Akka.Event;
using Akka.Routing;
using GITProtocol;
using GITProtocol.Utils;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UserNode.Database;

namespace UserNode
{
    public class UserActor : ReceiveActor
    {
        private long                                _userDBID               = 0;
        private string                              _strUserID              = "";
        private string                              _strGlobalUserID        = "";
        private int                                 _agentDBID              = 0;
        private double                              _balance                = 0.0;
        private long                                _lastScoreCounter       = 0;
        private Currencies                          _currency               = Currencies.USD;
        private bool                                _userDisconnected       = false;
        private Dictionary<string, UserConnection>  _userConnections        = new Dictionary<string, UserConnection>();
        private IActorRef                           _dbReader               = null;
        private IActorRef                           _dbWriter               = null;
        private readonly ILoggingAdapter            _logger                 = Context.GetLogger();
        protected static RealExtensions.Epsilon     _epsilion               = new RealExtensions.Epsilon(0.001);
        private ICancelable                         _conCheckCancel         = null;
        private IActorRef                           _afterQuitNotifyActor   = null;

        public UserActor(CreateNewUserMessage message, IActorRef dbReader, IActorRef dbWriter)
        {
            _dbReader           = dbReader;
            _dbWriter           = dbWriter;
            _agentDBID          = message.AgentDBID;
            _userDBID           = message.UserDBID;
            _strUserID          = message.UserID;
            _lastScoreCounter   = message.LastScoreCounter;
            _balance            = message.UserBalance;
            _currency           = message.Currency;
            _strGlobalUserID    = string.Format("{0}_{1}", _agentDBID, _strUserID);
            _userConnections.Add(message.SessionToken, new UserConnection(message.SessionToken));

            ReceiveAsync<UserLoggedIn>              (onUserLoginSucceeded);
            ReceiveAsync<FromConnRevMessage>        (onProcMessage);
            Receive<HttpSessionAdded>               (onProcHttpSessionAdded);
            ReceiveAsync<HttpSessionClosed>         (onProcHttpSessionClosed);
            ReceiveAsync<CloseHttpSession>          (onCloseHttpSession);
            Receive<QuitUserMessage>                (onForceLogoutMessage);
            Receive<QuitAndNotifyMessage>           (onForceQuitAndNotifyMessage);
            Receive<string>                         (onCommand);
            ReceiveAsync<SlotGamesNodeShuttingdown> (onSlotGameServerShuttingdown);
            Receive<ApiUserDepositMessage>          (onApiDepositMoney);
        }

        public static Props Props(CreateNewUserMessage message,IActorRef dbReader,IActorRef dbWriter)
        {
            return Akka.Actor.Props.Create(() => new UserActor(message, dbReader, dbWriter));
        }

        protected override void PreStart()
        {
            Self.Tell(new UserLoggedIn());
            base.PreStart();
        }

        protected override void PostStop()
        {
            if (_conCheckCancel != null)
                _conCheckCancel.Cancel();

            base.PostStop();
        }

        private void onApiDepositMoney(ApiUserDepositMessage request)
        {
            if (request.LastScoreCounter <= _lastScoreCounter)
                return;
            _lastScoreCounter = request.LastScoreCounter;
            _balance += request.Amount;
        }

        private async Task onUserLoginSucceeded(UserLoggedIn message)
        {
            try
            {
                _dbWriter.Tell(new UserLoginStateItem(_userDBID));
                _logger.Info("{0} has been logged in successfully", _strGlobalUserID);
                
                _conCheckCancel = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(1000, 1000, Self, "checkConn", Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::onUserLoginSucceeded {0}", ex);
            }
        }

        private void addScore(long scoreID, double score)
        {
            if (scoreID <= _lastScoreCounter)
                return;

            _lastScoreCounter = scoreID;
            _balance += score;
        }

        private void onForceQuitAndNotifyMessage(QuitAndNotifyMessage _)
        {
            _afterQuitNotifyActor = Sender;
            onForceLogoutMessage(new QuitUserMessage(_agentDBID, _strUserID));
        }

        private void onForceLogoutMessage(QuitUserMessage _)
        {
            for (int i = 0; i < this._userConnections.Count; i++)
                Self.Tell(new CloseHttpSession(_userConnections.Keys.ElementAt(i)));

            _logger.Info("User {0} has been kicked by admin", _strGlobalUserID);
        }

        private void onCommand(string strCommand)
        {
            if (strCommand == "checkConn")
            {
                foreach (KeyValuePair<string, UserConnection> pair in _userConnections)
                {
                    if (DateTime.Now.Subtract(pair.Value.LastActiveTime) >= TimeSpan.FromMinutes(5.0))
                        Self.Tell(new CloseHttpSession(pair.Key));
                }
            }
        }

        private void onProcHttpSessionAdded(HttpSessionAdded message)
        {
            if (_userConnections.ContainsKey(message.SessionToken))
            {
                if (message.SessionToken == "PrevWS")
                    _userConnections[message.SessionToken].LastActiveTime = DateTime.Now;
            }
            else
                _userConnections.Add(message.SessionToken, new UserConnection(message.SessionToken));
        }

        private async Task onCloseHttpSession(CloseHttpSession message)
        {
            try
            {
                //레디스에서 해당 세션토큰을 삭제한다.
                await RedisDatabase.RedisCache.HashDeleteAsync((RedisKey)string.Format("{0}_tokens", _strGlobalUserID), message.SessionToken);
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
                ExitGameResponse response = null;
                try
                {
                    response = await userConn.GameActor.Ask<ExitGameResponse>(new ExitGameRequest(_strUserID, _agentDBID, _balance, true, false), Constants.RemoteTimeOut);
                }
                catch (Exception ex)
                {
                    _logger.Warning("{0} exit game {1} Failed : Exception {2}", _strGlobalUserID, userConn.GameID, ex);
                }
                lastGameID = userConn.GameID;
            }

            _userConnections.Remove(userConn.Token);
            if (_userConnections.Count == 0)
            {
                try
                {
                    //밸런스변화가 있다면
                    double balanceUpdate = await _dbWriter.Ask<double>(new FetchUserBalanceUpdate(_userDBID));
                    await _dbReader.Ask(new UserOfflineStateItem(_userDBID, lastGameID, balanceUpdate));
                }
                catch (Exception ex)
                {
                    _logger.Error("Exception has been occured in UserActor::onProcSocketConnectionClosed User ID: {0} {1}", _strGlobalUserID, ex);
                }

                try
                {
                    await RedisDatabase.RedisCache.HashDeleteAsync((RedisKey)"onlineusers", new RedisValue[2] { _strGlobalUserID, _strGlobalUserID + "_path" });
                    await RedisDatabase.RedisCache.KeyDeleteAsync((RedisKey)(_strGlobalUserID + "_tokens"));
                }
                catch (Exception ex)
                {
                    _logger.Error("Exception has been occured in UserActor::onClosedUserConnection User ID: {0} {1}", _strGlobalUserID, ex);
                }

                _userDisconnected = true;
                _logger.Info("{0} user has been logged out", _strGlobalUserID);
                Context.Stop(Self);

                if (_afterQuitNotifyActor != null)
                {
                    _afterQuitNotifyActor.Tell(true);
                    _afterQuitNotifyActor = null;
                }
            }
        }

        private async Task onProcMessage(FromConnRevMessage fromConnRevMsg)
        {
            if (_userDisconnected)
                return;
            
            if (!_userConnections.ContainsKey(fromConnRevMsg.SessionToken))
                return;

            UserConnection  userConn    = _userConnections[fromConnRevMsg.SessionToken];
            GITMessage      message     = fromConnRevMsg.Message;

            if (message.MsgCode == (ushort)CSMSG_CODE.CS_HEARTBEAT)
            {
                userConn.LastActiveTime = DateTime.Now;
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
            else if (message.MsgCode >= (ushort)CSMSG_CODE.CS_PP_PROMOSTART && message.MsgCode <= (ushort)CSMSG_CODE.CS_PP_PROMOEND)
            {
                userConn.LastActiveTime = DateTime.Now;
                procPPPromoMsg(message, userConn);
            }
            else
            {
                _logger.Warning("Unknown paket received from {0} Message code:{1} User ID:{2}", "", message.MsgCode, _strGlobalUserID);
            }
        }

        private async Task procEnterGame(GITMessage message, UserConnection userConn)
        {
            int             gameID          = (int)(ushort)message.Pop();
            GameProviders   gameProvider    = DBMonitorSnapshot.Instance.getGITGameProvider(gameID);

            //등록된 게임아이디가 아님
            GITMessage enterResponseMessage;
            if (gameProvider == GameProviders.NONE)
            {
                _logger.Warning("{0} tried to enter game for not registered game id {1}", _strGlobalUserID, gameID);
                Sender.Tell("closeConnection");
                Self.Tell(new CloseHttpSession(userConn.Token));
                return;
            }

            //이미 게임에 입장함
            if (userConn.GameActor != null && userConn.GameID != gameID)
            {
                _logger.Warning("{0} tried to enter game while it has already been entered to other game", _strGlobalUserID);
                Sender.Tell("closeConnection");
                Self.Tell(new CloseHttpSession(userConn.Token));
                return;
            }

            bool enterGameSucceeded = false;

            if (userConn.GameActor != null && userConn.GameID == gameID)
            {
                enterGameSucceeded = true;
            }
            else
            {
                //같은 게임에 입장한 연결이 있는가를 검사한다.
                UserConnection oldConn = null;
                foreach (KeyValuePair<string, UserConnection> pair in _userConnections)
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
                    userConn.GameActor      = oldConn.GameActor;
                    userConn.GameID         = gameID;
                    userConn.GameProvider   = (int)gameProvider;

                    oldConn.resetGame();
                    Self.Tell(new CloseHttpSession(oldConn.Token));

                    enterGameSucceeded = true;
                }
                else
                {
                    if (gameProvider >= GameProviders.PP)
                    {
                        try
                        {
                            EnterGameRequest requestMsg     = new EnterGameRequest(gameID, _agentDBID, _strUserID, Self, true);
                            EnterGameResponse responseMsg   = await Context.System.ActorSelection(Constants.SlotGameRouterPath).Ask<EnterGameResponse>(requestMsg, Constants.RemoteTimeOut);
                            if (responseMsg.Ack == 0)
                            {
                                userConn.GameActor      = responseMsg.GameActor;
                                userConn.GameID         = gameID;
                                userConn.GameProvider   = (int)gameProvider;

                                enterGameSucceeded = true;
                                _dbWriter.Tell(new UserGameStateItem(_userDBID, 2, gameID));
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Warning("{0} enter slot game {1} Failed : Exception {2}", _strGlobalUserID, gameID, ex);
                        }
                    }
                }
            }

            if (enterGameSucceeded)
            {
                foreach (KeyValuePair<string, UserConnection> pair in _userConnections)
                {
                    if (pair.Key != userConn.Token)
                    {
                        UserConnection conn = pair.Value;
                        Self.Tell(new CloseHttpSession(conn.Token));
                    }
                }
            }

            //방입장결과메세지를 클라이언트에 보낸다.
            enterResponseMessage = new GITMessage((ushort)SCMSG_CODE.SC_ENTERGAME);
            enterResponseMessage.Append((enterGameSucceeded ? (byte)0 : (byte)1));
            Sender.Tell(new SendMessageToUser(enterResponseMessage, _balance, 0.0));
        }

        private async Task procSlotGameMsg(GITMessage message, UserConnection userConn)
        {
            if (userConn.GameActor == null || userConn.GameProvider == (int)GameProviders.NONE)
            {
                Self.Tell(new CloseHttpSession(userConn.Token));
                Sender.Tell("invalidaction");
                return;
            }

            UserBonus waitingBonus      = null;
            ToUserMessage toUserMessage = null;
            try
            {
                toUserMessage = await userConn.GameActor.Ask<ToUserMessage>(new FromUserMessage(_strUserID, _agentDBID, _balance, Self, message, waitingBonus, _currency), Constants.RemoteTimeOut);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in UserActor::procSlotGameMsg {0} gameid: {1}, message code: {2}, {3}", _strGlobalUserID, userConn.GameID, message.MsgCode, ex);
            }
            
            if (toUserMessage != null)
                procToUserMessage(toUserMessage, waitingBonus, message, userConn);
            else
                Sender.Tell("nomessagefromslotnode");
        }

        private void procToUserMessage(ToUserMessage message,UserBonus askedBonus,GITMessage gameMessage,UserConnection userConn)
        {
            //만일 게임결과처리와 관련된 메세지라면
            if (message is ToUserResultMessage)
            {
                bool isSuccess = processResultMessage(message as ToUserResultMessage);
                if (gameMessage.MsgCode == (ushort)CSMSG_CODE.CS_PP_DOSPIN || gameMessage.MsgCode == (ushort)CSMSG_CODE.CS_PP_DOBONUS)
                {
                    if (!isSuccess)
                    {
                        GITMessage balanceNotEnough = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOSPIN);
                        balanceNotEnough.Append(string.Format("balance={0}&balance_cash={0}&balance_bonus=0.0&frozen=Internal+server+error.+The+game+will+be+restarted.+&msg_code=11&ext_code=SystemError", Math.Round(_balance, 2)));
                        Sender.Tell(new SendMessageToUser(balanceNotEnough, _balance, 0.0));
                        
                        GITMessage errorResultMsg = new GITMessage((ushort)CSMSG_CODE.CS_PP_NOTPROCDRESULT);
                        userConn.GameActor.Tell(new FromUserMessage(_strUserID, _agentDBID, _balance, Self, errorResultMsg, null, _currency));
                        return;
                    }
                }

                if (!isSuccess)
                {
                    Self.Tell(new CloseHttpSession(userConn.Token));
                    Sender.Tell("invalidaction");
                    return;
                }
            }

            for (int i = 0; i < message.Messages.Count; i++)
            {
                if (message.Messages[i] != null)
                {
                    Sender.Tell(new SendMessageToUser(message.Messages[i], _balance, 0.0));
                    break;
                }
            }
        }

        private void addPoint(int providerID, int gameID, double betMoney, double winMoney)
        {
            DateTime nowTime    = DateTime.UtcNow;
            DateTime dateTime   = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day);

            if (betMoney != 0.0 || winMoney != 0.0)
                _dbWriter.Tell(new GameReportItem(gameID, _agentDBID, betMoney, winMoney, dateTime));
        }

        private bool processResultMessage(ToUserResultMessage resultMessage)
        {
            DateTime nowReportTime      = DateTime.UtcNow;
            DateTime nowDayReportTime   = new DateTime(nowReportTime.Year, nowReportTime.Month, nowReportTime.Day);
            GameProviders providerID    = DBMonitorSnapshot.Instance.getGITGameProvider(resultMessage.GameID);

            if (resultMessage is ToUserSpecialResultMessage)
            {
                ToUserSpecialResultMessage specialResultMessage = resultMessage as ToUserSpecialResultMessage;

                //보유머니를 검사한다.
                double betMoney     = Math.Round(specialResultMessage.BetMoney, 2);
                double realBet      = Math.Round(specialResultMessage.RealBet, 2);
                double winMoney     = Math.Round(specialResultMessage.WinMoney, 2);
                if (_balance.LT(realBet, _epsilion))
                    return false;

                if (specialResultMessage.IsJustBet)
                {
                    //보유머니를 갱신한다.
                    _balance -= realBet;

                    //디비를 갱신한다.
                    addPoint((int)providerID, specialResultMessage.GameID, realBet, 0.0);
                    _dbWriter.Tell(new PlayerBalanceUpdateItem(_userDBID, -realBet));
                    _dbWriter.Tell(new ReportUpdateItem(_strUserID, _agentDBID, nowDayReportTime, realBet, 0.0));
                }
                else
                {
                    //보유머니를 갱신한다.
                    double beginMoney = _balance + (betMoney - realBet);
                    _balance += winMoney - realBet;
                    
                    addPoint((int)providerID, specialResultMessage.GameID, realBet, winMoney);
                    _dbWriter.Tell(new PlayerBalanceUpdateItem(_userDBID, winMoney - realBet));
                    _dbWriter.Tell(new ReportUpdateItem(_strUserID, _agentDBID, nowDayReportTime, realBet, winMoney));
                    _dbWriter.Tell(new GameLogItem(_strUserID, specialResultMessage.GameID, specialResultMessage.GameLog.GameName, betMoney, winMoney, beginMoney, _balance, specialResultMessage.GameLog.LogString, (int)specialResultMessage.BetType, nowReportTime, _agentDBID));
                }
            }
            else
            {
                //보유머니를 검사한다.
                double betMoney = Math.Round(resultMessage.BetMoney, 2);
                double winMoney = Math.Round(resultMessage.WinMoney, 2);
                
                if (_balance.LT(betMoney, _epsilion))
                    return false;

                if (betMoney != 0.0 || winMoney != 0.0)
                {
                    //보유머니를 갱신한다.
                    double beforeBalance = _balance;
                    _balance += winMoney - betMoney;

                    //디비를 갱신한다.
                    addPoint((int)providerID, resultMessage.GameID, betMoney, winMoney);
                    _dbWriter.Tell(new PlayerBalanceUpdateItem(_userDBID, winMoney - betMoney));
                    _dbWriter.Tell(new ReportUpdateItem(_strUserID, _agentDBID, nowDayReportTime, betMoney, winMoney));
                    _dbWriter.Tell(new GameLogItem(_strUserID, resultMessage.GameID, resultMessage.GameLog.GameName, betMoney, winMoney, beforeBalance, _balance, resultMessage.GameLog.LogString, (int)resultMessage.BetType, nowReportTime, _agentDBID));
                }
            }
            return true;
        }

        private string convertRaceWinnerInfoToV0(string raceWinnersV2)
        {
            JToken raceWinnerObj    = JToken.Parse(raceWinnersV2);
            JArray winnersArray     = raceWinnerObj["winners"] as JArray;
            for (int i = 0; i < winnersArray.Count; i++)
            {
                JArray winnerItems = winnersArray[i]["items"] as JArray;
                for (int j = 0; j < winnerItems.Count; j++)
                {
                    string[] strArray = ((string)winnerItems[j]).Split(new string[1] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    JObject jobject = JObject.Parse("{}");
                    jobject["prizeID"]                      = int.Parse(strArray[0]);
                    jobject["playerID"]                     = strArray[1];
                    jobject["bet"]                          = double.Parse(strArray[2]);
                    jobject["effectiveBetForBetMultiplier"] = double.Parse(strArray[3]);
                    jobject["effectiveBetForFreeRounds"]    = double.Parse(strArray[4]);
                    jobject["memberCurrency"]               = strArray[5];
                    jobject["countryID"]                    = strArray[6];

                    winnerItems[j]                          = jobject;
                }
            }
            return raceWinnerObj.ToString();
        }

        private async Task<string> buildPPTourScores(GITMessage requestMessage)
        {
            try
            {
                int tourCount = (int)requestMessage.Pop();
                List<int> tourIds = new List<int>();
                for (int i = 0; i < tourCount; i++)
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
                        JObject tour        = new JObject();
                        tour["position"]        = (JToken)3384347;
                        tour["score"]           = (JToken)(long)scoreValue;
                        tour["tournamentID"]    = (JToken)tourIds[i];
                        scores.Add(tour);
                    }
                }

                if (scores.Count > 0)
                    response["scores"] = scores;

                return response.ToString();
            }
            catch (Exception ex)
            {
                return "{\"error\":0,\"description\":\"OK\"}";
            }
        }

        private string buildActivePromos()
        {
            return "{\"error\":0,\"description\":\"OK\",\"serverTime\":1676462782,\"tournaments\":[],\"races\":[]}";
        }

        private async Task procOptIn(bool isTournament, int promoID)
        {
            try
            {
                string strHashKey = "";
                if (isTournament)
                    strHashKey = string.Format("tournament_{0}_optin", promoID);
                else 
                    strHashKey = string.Format("race_{0}_optin", promoID);

                await RedisDatabase.RedisCache.HashSetAsync(strHashKey, _strGlobalUserID, 0);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::procOptIn {0}", ex);
            }
        }

        private GITMessage onPromoActive(GITMessage message)
        {
            GITMessage respondMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_PROMOACTIVE);
            try
            {
                string strGameSymbol = (string)message.Pop();
                respondMessage.Append(buildActivePromos());
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::onPromoActive {0}", ex);
            }
            return respondMessage;
        }

        private void procPPPromoMsg(GITMessage message, UserConnection userConn)
        {
            GITMessage responseMessage = null;
            if (message.MsgCode == (ushort)CSMSG_CODE.CS_PP_PROMOACTIVE)
                responseMessage = onPromoActive(message);
            
            if (responseMessage != null)
                Sender.Tell(new SendMessageToUser(responseMessage, _balance, 0.0));
        }

        private async Task replaceSlotGameNode(string strSlotNodePath,UserConnection userConn)
        {
            int remainServerCount = 0;
            try
            {
                Routees routees = await Context.System.ActorSelection(Constants.SlotGameRouterPath).Ask<Routees>(new GetRoutees());
                foreach (Routee routee in routees.Members)
                    remainServerCount++;

                _logger.Info("{0} Exiting from slot game node {1}", _strGlobalUserID, strSlotNodePath);

                ExitGameResponse response = null;
                if (remainServerCount > 0)
                    response = await userConn.GameActor.Ask<ExitGameResponse>(new ExitGameRequest(_strUserID, _agentDBID, _balance, false, true), TimeSpan.FromSeconds(5.0));            
                else
                    response = await userConn.GameActor.Ask<ExitGameResponse>(new ExitGameRequest(_strUserID, _agentDBID, _balance, false, false), TimeSpan.FromSeconds(5.0));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in UserActor::onSlotGameServerShuttingdown {0}", ex);
            }

            try
            {
                _logger.Info("{0} Reentering slot game node", _strGlobalUserID);
                if (remainServerCount == 0)
                {
                    _logger.Info("{0} no more slot game node found", _strGlobalUserID);
                    Self.Tell(new HttpSessionClosed(userConn.Token));
                    userConn.GameActor      = null;
                    userConn.GameProvider   = 0;
                    userConn.GameID         = 0;
                    return;
                }
                
                EnterGameRequest    requestMsg  = new EnterGameRequest(userConn.GameID, _agentDBID, _strUserID, Self, false);
                EnterGameResponse   responseMsg = await Context.System.ActorSelection(Constants.SlotGameRouterPath).Ask<EnterGameResponse>(requestMsg, Constants.RemoteTimeOut);
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

            for (int i = 0; i < _userConnections.Count; i++)
            {
                string          strToken    = _userConnections.Keys.ElementAt(i);
                UserConnection  userConn    = _userConnections[strToken];

                if (userConn.GameActor == null || userConn.GameProvider == (int)GameProviders.NONE)
                    continue;

                if(userConn.GameActor.Path.ToString().Contains(message.Path))
                    continue;
                    
                await replaceSlotGameNode(message.Path, userConn);
            }
        }

        public class UserLoggedIn
        {
        }

        public class UserConnection
        {
            public string       Token           { get; set; }
            public IActorRef    GameActor       { get; set; }
            public int          GameID          { get; set; }
            public int          GameProvider    { get; set; }
            public DateTime     LastActiveTime  { get; set; }

            public UserConnection(string sessionToken)
            {
                Token           = sessionToken;
                GameActor       = null;
                GameID          = 0;
                GameProvider    = 0;
                LastActiveTime  = DateTime.Now;
            }

            public void resetGame()
            {
                GameActor       = null;
                GameID          = 0;
                GameProvider    = 0;
            }
        }
    }
}
