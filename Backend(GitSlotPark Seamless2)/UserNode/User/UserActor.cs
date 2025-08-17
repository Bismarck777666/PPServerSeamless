using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using GITProtocol;
using Akka.Event;
using GITProtocol.Utils;
using UserNode.Database;
using System.Net;
using Akka.Routing;
using System.Threading;
using StackExchange.Redis;
using Akka.Cluster;
using Newtonsoft.Json;
using System.Net.Http;
using Pipelines.Sockets.Unofficial;
using System.Transactions;
using Newtonsoft.Json.Linq;

namespace UserNode
{
    //로그인된 사용자를 표현하는 클라스 
    public class UserActor : ReceiveActor
    {
        #region 사용자정보
        private long                    _userDBID               = 0;
        private string                  _strUserID              = "";
        private string                  _strGlobalUserID        = "";
        private string                  _agentID                = "";
        private int                     _agentDBID              = 0;
        private double                  _balance                = 0.0;
        private long                    _lastScoreCounter       = 0;
        private Currencies              _currency               = Currencies.USD;
        #endregion

        #region 유저의 상태변수들       
        private bool                                _userDisconnected   = false;
        private Dictionary<string, UserConnection>  _userConnections    = new Dictionary<string, UserConnection>();
        #endregion

        #region 사용자의 각종 보너스정보
        private UserRangeOddEventItem   _userRangeOddEventItem;
        private List<UserBonus>         _waitingUserBonuses = new List<UserBonus>();
        #endregion

        private IActorRef                       _dbReader       = null;
        private IActorRef                       _dbWriter       = null;
        private readonly ILoggingAdapter        _logger         = Logging.GetLogger(Context);
        protected static RealExtensions.Epsilon _epsilion       = new RealExtensions.Epsilon(0.001);

        private ICancelable                     _conCheckCancel       = null;
        private IActorRef                       _afterQuitNotifyActor = null;
        private HttpClient                      _httpClient           = new HttpClient();
        public UserActor(CreateNewUserMessage message, IActorRef dbReader, IActorRef dbWriter)
        {
            _dbReader           = dbReader;
            _dbWriter           = dbWriter;
            _agentDBID          = message.AgentDBID;
            _agentID            = message.AgentID;
            _userDBID           = message.UserDBID;
            _strUserID          = message.UserID;
            _lastScoreCounter   = message.LastScoreCounter;
            _balance            = message.UserBalance;
            _currency           = message.Currency;
            _strGlobalUserID    = string.Format("{0}_{1}", _agentDBID, _strUserID);
            _userConnections.Add(message.SessionToken, new UserConnection(message.SessionToken));

            Receive<UserLoggedIn>                       (onUserLoginSucceeded);
            ReceiveAsync<FromConnRevMessage>            (onProcMessage);
            Receive<HttpSessionAdded>                   (onProcHttpSessionAdded);
            ReceiveAsync<HttpSessionClosed>             (onProcHttpSessionClosed);
            ReceiveAsync<CloseHttpSession>              (onCloseHttpSession);
            Receive<QuitUserMessage>                    (onForceLogoutMessage);
            Receive<QuitAndNotifyMessage>               (onForceQuitAndNotifyMessage);
            Receive<string>                             (onCommand);            
            ReceiveAsync<SlotGamesNodeShuttingdown>     (onSlotGameServerShuttingdown);
            Receive<UserRangeOddEventItem>              (onUserRangeOddEvent);
            Receive<UserEventCancelled>                 (onUserEventCancelled);
        }

        public static Props Props(CreateNewUserMessage message, IActorRef dbReader, IActorRef dbWriter)
        {
            return Akka.Actor.Props.Create(() => new UserActor(message, dbReader, dbWriter));
        }
        protected override void PreStart()
        {
            Self.Tell(new UserLoggedIn());                                                  
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
        private void onUserLoginSucceeded(UserLoggedIn message)
        {
            try
            {
                //유저온라인상태 갱신
                _dbWriter.Tell(new UserLoginStateItem(_userDBID));
                _dbReader.Tell(new GetUserBonusItems(_agentDBID, _strUserID));

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
            _balance         += score;
        }
        #region 각종 사건처리부
        private void onUserEventCancelled(UserEventCancelled message)
        {
            if (_userRangeOddEventItem != null && _userRangeOddEventItem.BonusID == message.BonusID)
            {
                int i = 0;
                for (i = 0; i < _waitingUserBonuses.Count; i++)
                {
                    if (_waitingUserBonuses[i].BonusType == UserBonusType.USEREVENT && _waitingUserBonuses[i].BonusID == message.BonusID)
                        break;
                }
                if (i < _waitingUserBonuses.Count)
                    _waitingUserBonuses.RemoveAt(i);

                _dbReader.Tell(new ClaimedUserRangeEventMessage(_userRangeOddEventItem.BonusID, _agentDBID, _strUserID, 0.0, ""));
                _userRangeOddEventItem = null;
            }
        }
        private void onUserRangeOddEvent(UserRangeOddEventItem userRangeEventItem)
        {
            if (_userRangeOddEventItem != null)
                return;

            _userRangeOddEventItem = userRangeEventItem;
            _waitingUserBonuses.Add(new UserRangeOddEventBonus(userRangeEventItem.BonusID, userRangeEventItem.MinOdd, userRangeEventItem.MaxOdd, userRangeEventItem.MaxBet));
        }
        private void fetchNewUserRangeEvent(double rewardedMoney, int gameID)
        {
            _dbWriter.Tell(new ClaimedUserRangeEventItem(_userRangeOddEventItem.BonusID, rewardedMoney, gameID.ToString(), DateTime.Now));
            _dbReader.Tell(new ClaimedUserRangeEventMessage(_userRangeOddEventItem.BonusID, _agentDBID, _strUserID, rewardedMoney, gameID.ToString()));
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
        private void onForceQuitAndNotifyMessage(QuitAndNotifyMessage _)
        {
            _afterQuitNotifyActor = Sender;
            onForceLogoutMessage(new QuitUserMessage(_agentDBID, _strUserID));
        }

        private void onForceLogoutMessage(QuitUserMessage _)
        {
            //모든 연결들에 통지한다.
            for (int i = 0; i < _userConnections.Count; i++)
            {
                string strToken = _userConnections.Keys.ElementAt(i);
                Self.Tell(new CloseHttpSession(strToken));
            }
            _logger.Info("User {0} has been kicked by admin", _strGlobalUserID);
        }

        private void onCommand(string strCommand)
        {
            if(strCommand == "checkConn")
            {
                foreach(KeyValuePair<string, UserConnection> pair in _userConnections)
                {
                    if (DateTime.Now.Subtract(pair.Value.LastActiveTime) >= TimeSpan.FromMinutes(5))
                        Self.Tell(new CloseHttpSession(pair.Key as string));
                }
            }
        }        
        private void onProcHttpSessionAdded(HttpSessionAdded message)
        {
            if (_userConnections.ContainsKey(message.SessionToken))
            {
                if (message.SessionToken == "PrevWS")
                    _userConnections[message.SessionToken].LastActiveTime = DateTime.Now;
                return;
            }
            _userConnections.Add(message.SessionToken, new UserConnection(message.SessionToken));
        }
        private async Task onCloseHttpSession(CloseHttpSession message)
        {
            try
            {
                //레디스에서 해당 세션토큰을 삭제한다.
                await RedisDatabase.RedisCache.HashDeleteAsync(string.Format("{0}_tokens", _strGlobalUserID), message.SessionToken);
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
                    await RedisDatabase.RedisCache.HashDeleteAsync("onlineusers", new RedisValue[] { _strGlobalUserID, _strGlobalUserID + "_path" });
                    await RedisDatabase.RedisCache.KeyDeleteAsync(_strGlobalUserID + "_tokens");

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
                    _afterQuitNotifyActor.Tell(_balance);
                    _afterQuitNotifyActor = null;
                }
            }
        }
        #endregion

        #region 메세지처리함수들
        private async Task onProcMessage(FromConnRevMessage fromConnRevMsg)
        {
            //이미 로그아웃된 유저에 한해서 모든 메세지처리를 무시한다.
            if (_userDisconnected)
                return;

            if (!_userConnections.ContainsKey(fromConnRevMsg.SessionToken))
                return;

            UserConnection userConn = _userConnections[fromConnRevMsg.SessionToken];
            GITMessage message      = fromConnRevMsg.Message;
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
            else if(message.MsgCode >= (ushort) CSMSG_CODE.CS_PP_PROMOSTART && message.MsgCode <= (ushort)CSMSG_CODE.CS_PP_PROMOEND)
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
            if (gameProvider == GameProviders.NONE)
            {
                _logger.Warning("{0} tried to enter game for not registered game id {1}", _strGlobalUserID, (int)gameID);
                Sender.Tell("closeConnection");
                Self.Tell(new CloseHttpSession(userConn.Token));
                return;
            }

            //이미 게임에 입장함
            if (userConn.GameActor != null && userConn.GameID != gameID)
            {
                //이미 다른 게임에 입장하였다면
                _logger.Warning("{0} tried to enter game while it has already been entered to other game", _strGlobalUserID);
                Sender.Tell("closeConnection");
                Self.Tell(new CloseHttpSession(userConn.Token));
                return;
            }

            bool enterGameSucceeded = false;
            do
            {
                //유저의 보유머니를 API로 부터 불러온다.
                var balanceResponse = await callGetBalance(gameID);
                if (balanceResponse == null || balanceResponse.code != 0 || balanceResponse.balance < 0.0M)
                {
                    enterGameSucceeded = false;
                    break;
                }
                _balance = Math.Round((double) balanceResponse.balance, 2);
                _dbWriter.Tell(new PlayerBalanceUpdateItem(_userDBID, _balance));                                                            //유저머니갱신

                //이미 해당게임에 입장하였다면 처리를 진행하지 않는다.
                if (userConn.GameActor != null && userConn.GameID == gameID)
                {
                    enterGameSucceeded = true;
                    break;
                }

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
                    userConn.GameProvider   = (int) gameProvider;
                    oldConn.resetGame();
                    Self.Tell(new CloseHttpSession(oldConn.Token));
                    enterGameSucceeded      = true;
                    break;
                }
                if (gameProvider >= GameProviders.PP)
                {
                    try
                    {
                        EnterGameRequest  requestMsg  = new EnterGameRequest(gameID, _agentDBID, _strUserID, Self);
                        EnterGameResponse responseMsg = await Context.System.ActorSelection(Constants.SlotGameRouterPath).Ask<EnterGameResponse>(requestMsg, Constants.RemoteTimeOut);
                        if (responseMsg.Ack == 0)
                        {
                            userConn.GameActor      = responseMsg.GameActor;
                            userConn.GameID         = gameID;
                            userConn.GameProvider   = (int) gameProvider;
                            enterGameSucceeded      = true;
                            _dbWriter.Tell(new UserGameStateItem(_userDBID, 2, (int)gameID));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Warning("{0} enter slot game {1} Failed : Exception {2}", _strGlobalUserID, gameID, ex);
                    }
                }
            } while (false);
            
            if(enterGameSucceeded)
            {
                foreach (KeyValuePair<string, UserConnection> pair in _userConnections)
                {
                    if (pair.Key == userConn.Token)
                        continue;

                    UserConnection conn = pair.Value;
                    Self.Tell(new CloseHttpSession(conn.Token));
                }
            }
            
            GITMessage enterResponseMessage = new GITMessage((ushort)SCMSG_CODE.SC_ENTERGAME);
            enterResponseMessage.Append(enterGameSucceeded ? (byte)0 : (byte)1);
            Sender.Tell(new SendMessageToUser(enterResponseMessage, _balance, 0.0));
        }

        #region Callback API관련처리
        public static string createDataSign(string key, string message)
        {
            var hmac    = System.Security.Cryptography.HMAC.Create("HMACSHA256");
            hmac.Key    = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            return BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();
        }

        private async Task<GetBalanceResponse> callGetBalance(int gameID)
        {
            try
            {
                AgentConfig agentConfig = AgentSnapshot.Instance.findAgentConfig(_agentDBID);
                if (agentConfig == null)
                    return null;

                GetBalanceRequest request = new GetBalanceRequest();
                request.agentID = _agentID;
                request.userID  = _strUserID;
                request.gameID  = gameID;
                request.sign    = createDataSign(agentConfig.SecretKey, string.Format("{0}{1}{2}", _agentID, _strUserID, gameID));

                string      strURL          = string.Format("{0}/GetBalance", agentConfig.CallbackURL);
                HttpResponseMessage message = await _httpClient.PostAsync(strURL, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
                message.EnsureSuccessStatusCode();

                string strContent = await message.Content.ReadAsStringAsync();
                GetBalanceResponse response = JsonConvert.DeserializeObject<GetBalanceResponse>(strContent);
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::callGetBalance {0} {1} {2}", ex, _agentID, _strUserID);
                return null;
            }
        }
        private async Task<WithdrawResponse> callWithdraw(int gameID, double amount, string roundID, string transactionID)
        {
            AgentConfig agentConfig = AgentSnapshot.Instance.findAgentConfig(_agentDBID);
            if (agentConfig == null)
                return null;

            try
            {

                WithdrawRequest request = new WithdrawRequest();
                request.agentID         = _agentID;
                request.userID          = _strUserID;
                request.gameID          = gameID;
                request.amount          = (decimal)Math.Round(amount, 2);
                request.roundID         = roundID;
                request.transactionID   = transactionID;
                request.sign = createDataSign(agentConfig.SecretKey, string.Format("{0}{1}{2}{3}{4}{5}", _agentID, _strUserID, request.amount.ToString("0.00"),
                    transactionID, roundID, gameID));

                string strURL = string.Format("{0}/Withdraw", agentConfig.CallbackURL);
                HttpResponseMessage message = await _httpClient.PostAsync(strURL, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
                message.EnsureSuccessStatusCode();
                string strContent = await message.Content.ReadAsStringAsync();

                var response = JsonConvert.DeserializeObject<WithdrawResponse>(strContent);
                if(response.code == 0)
                    _dbWriter.Tell(new ApiTransactionItem(_agentID, _strUserID, gameID, amount, 0.0, transactionID, "", response.platformTransactionID, roundID, TransactionTypes.Withdraw, DateTime.UtcNow));

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::callWithdraw {0} {1} {2}", ex, _agentID, _strUserID);
            }

            Context.System.ActorSelection("/user/retryWorkers").Tell(new RollbackRequestWithRetry(agentConfig.CallbackURL,
                _agentID, _strUserID, agentConfig.SecretKey, gameID, 50, DateTime.Now.Subtract(TimeSpan.FromSeconds(10.0)), transactionID, amount, 0.0));

            return null;
        }
        private async Task<DepositResponse> callDeposit(int gameID, double amount, string roundID, string betTransactionID, string transactionID)
        {
            AgentConfig agentConfig = AgentSnapshot.Instance.findAgentConfig(_agentDBID);
            if (agentConfig == null)
                return null;

            try
            {

                DepositRequest request = new DepositRequest();
                request.agentID          = _agentID;
                request.userID           = _strUserID;
                request.gameID           = gameID;
                request.amount           = (decimal)Math.Round(amount, 2);
                request.roundID          = roundID;
                request.transactionID    = transactionID;
                request.refTransactionID = betTransactionID;

                request.sign = createDataSign(agentConfig.SecretKey, string.Format("{0}{1}{2}{3}{4}{5}{6}", _agentID, _strUserID, request.amount.ToString("0.00"),
                    betTransactionID, transactionID, roundID, gameID));

                string strURL = string.Format("{0}/Deposit", agentConfig.CallbackURL);
                HttpResponseMessage message = await _httpClient.PostAsync(strURL, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
                message.EnsureSuccessStatusCode();
                string strContent = await message.Content.ReadAsStringAsync();

                var response = JsonConvert.DeserializeObject<DepositResponse>(strContent);
                if (response.code == 0 || response.code == 11)
                    _dbWriter.Tell(new ApiTransactionItem(_agentID, _strUserID, gameID, 0.0, amount, transactionID, betTransactionID, response.platformTransactionID, roundID, TransactionTypes.Deposit, DateTime.UtcNow));
                else
                    _dbWriter.Tell(new FailedTransactionItem(_agentID, _strUserID, TransactionTypes.Deposit, transactionID, betTransactionID, 0.0, amount, gameID, DateTime.UtcNow));
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::callDeposit {0} {1} {2}", ex, _agentID, _strUserID);
            }

            _dbWriter.Tell(new ApiTransactionItem(_agentID, _strUserID, gameID, 0.0, amount, transactionID, betTransactionID, "", roundID, TransactionTypes.Deposit, DateTime.UtcNow));
            Context.System.ActorSelection("/user/retryWorkers").Tell(new DepositRequestWithRetry(agentConfig.CallbackURL,
                _agentID, _strUserID, agentConfig.SecretKey, gameID, 50, DateTime.Now.Subtract(TimeSpan.FromSeconds(10.0)), transactionID, betTransactionID, roundID, amount));

            return null;
        }
        private async Task<WithdrawResponse> callBetWin(int gameID, double betAmount, double winAmount, string roundID, string transactionID)
        {
            AgentConfig agentConfig = AgentSnapshot.Instance.findAgentConfig(_agentDBID);
            if (agentConfig == null)
                return null;

            try
            {

                BetWinRequest request   = new BetWinRequest();
                request.agentID         = _agentID;
                request.userID          = _strUserID;
                request.gameID          = gameID;
                request.betAmount       = (decimal)Math.Round(betAmount, 2);
                request.winAmount       = (decimal)Math.Round(winAmount, 2);
                request.roundID         = roundID;
                request.transactionID   = transactionID;
                request.sign            = createDataSign(agentConfig.SecretKey, string.Format("{0}{1}{2}{3}{4}{5}{6}", _agentID, _strUserID, request.betAmount.ToString("0.00"),
                    request.winAmount.ToString("0.00"), transactionID, roundID, gameID));

                string strURL = string.Format("{0}/BetWin", agentConfig.CallbackURL);

                HttpResponseMessage message = await _httpClient.PostAsync(strURL, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
                message.EnsureSuccessStatusCode();
                string strContent = await message.Content.ReadAsStringAsync();

                var response = JsonConvert.DeserializeObject<WithdrawResponse>(strContent);
                if (response.code == 0)
                    _dbWriter.Tell(new ApiTransactionItem(_agentID, _strUserID, gameID, betAmount, winAmount, transactionID, "", response.platformTransactionID, roundID, TransactionTypes.BetWin, DateTime.UtcNow));

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::callBetWin {0} {1} {2}", ex, _agentID, _strUserID);
            }

            Context.System.ActorSelection("/user/retryWorkers").Tell(new RollbackRequestWithRetry(agentConfig.CallbackURL,
                _agentID, _strUserID, agentConfig.SecretKey, gameID, 50, DateTime.Now.Subtract(TimeSpan.FromSeconds(10.0)), transactionID, betAmount, winAmount));

            return null;
        }

        public class GetBalanceRequest
        {
            public string   agentID   { get; set; }
            public string   sign      { get; set; }
            public string   userID    { get; set; }
            public int      gameID    { get; set; }
        }
        public class GitDebitCreditRequest
        {
            public string userid { get; set; }
            public decimal debitamount { get; set; }
            public decimal creditamount { get; set; }
            public int vendor { get; set; }
            public string game { get; set; }
            public string transactionid { get; set; }

        }
        public class GitDebitCreditResponse
        {
            public int status { get; set; }
            public double balance { get; set; }
            public string error { get; set; }
        }
        public class GetBalanceResponse
        {
            public int      code    { get; set; }
            public string   message { get; set; }
            public decimal  balance { get; set; }
        }
        #endregion
        private async Task procSlotGameMsg(GITMessage message, UserConnection userConn)
        {
            //게임에 입장한 상태가 아니라면
            if (userConn.GameActor == null || userConn.GameProvider == (int) GameProviders.NONE)
            {
                Self.Tell(new CloseHttpSession(userConn.Token));
                Sender.Tell("invalidaction"); //유저의 고의적인 액션    
                return;
            }
            UserBonus       waitingBonus    = pickUserBonus(userConn.GameID);
            ToUserMessage   toUserMessage   = null;
            try
            {
                toUserMessage   = await userConn.GameActor.Ask<ToUserMessage>(new FromUserMessage(_strUserID, _agentDBID, _balance, Self, message, waitingBonus, _currency), Constants.RemoteTimeOut);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in UserActor::procSlotGameMsg {0} gameid: {1}, message code: {2}, {3}",
                    _strGlobalUserID, userConn.GameID, message.MsgCode, ex);
            }
            if (toUserMessage != null)
                await procToUserMessage(toUserMessage, waitingBonus, message, userConn);
            else
                Sender.Tell("nomessagefromslotnode");
        }
        private async Task procToUserMessage(ToUserMessage message, UserBonus askedBonus, GITMessage gameMessage, UserConnection userConn)
        {
            //만일 게임결과처리와 관련된 메세지라면
            if (message is ToUserResultMessage)
            {
                bool isSuccess = await processResultMessage(message as ToUserResultMessage);
                if((gameMessage.MsgCode == (ushort) CSMSG_CODE.CS_PP_DOSPIN || gameMessage.MsgCode == (ushort)CSMSG_CODE.CS_PP_DOBONUS))
                {
                    if(!isSuccess)
                    {
                        GITMessage balanceNotEnough = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOSPIN);
                        balanceNotEnough.Append(string.Format("balance={0}&balance_cash={0}&balance_bonus=0.0&frozen=Internal+server+error.+The+game+will+be+restarted.+&msg_code=11&ext_code=SystemError", Math.Round(_balance, 2)));
                        Sender.Tell(new SendMessageToUser(balanceNotEnough, _balance, 0.0));

                        GITMessage errorResultMsg = new GITMessage((ushort)CSMSG_CODE.CS_PP_NOTPROCDRESULT);
                        userConn.GameActor.Tell(new FromUserMessage(_strUserID, _agentDBID, _balance, Self, errorResultMsg, null, _currency));
                        return;
                    }
                }
                if(!isSuccess)
                {
                    Self.Tell(new CloseHttpSession(userConn.Token));
                    Sender.Tell("invalidaction");
                    return;
                }
            }
            if (askedBonus != null && message.IsRewardedBonus)
            {
                _waitingUserBonuses.Remove(askedBonus);
                onRewardCompleted(askedBonus, message.RewardBonusMoney, message.GameID);
            }
            for (int i = 0; i < message.Messages.Count; i++)
            {
                if(message.Messages[i] != null)
                {
                    Sender.Tell(new SendMessageToUser(message.Messages[i], _balance, 0.0));
                    break;
                }
            }
        }
        private void onRewardCompleted(UserBonus completedBonus, double rewardedMoney, int gameID)
        {
            if (completedBonus is UserRangeOddEventBonus)
                fetchNewUserRangeEvent(rewardedMoney, gameID);
        }
        private void addPoint(int providerID, int gameID, double betMoney, double winMoney)
        {
            DateTime nowTime   = DateTime.UtcNow.AddHours(9.0);
            DateTime dateTime  = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day);

            if(betMoney > 0.0)
                _dbWriter.Tell(new UserBetMoneyUpdateItem(_userDBID, betMoney));

            if(betMoney != 0.0 || winMoney != 0.0)
                _dbWriter.Tell(new GameReportItem(gameID, _agentDBID, betMoney, winMoney, dateTime)); 
        }
        private async Task<bool> processResultMessage(ToUserResultMessage resultMessage)
        {
            DateTime        nowReportTime     = DateTime.UtcNow;
            DateTime        nowDayReportTime  = new DateTime(nowReportTime.Year, nowReportTime.Month, nowReportTime.Day);
            GameProviders   providerID        = DBMonitorSnapshot.Instance.getGITGameProvider(resultMessage.GameID);
            //보유머니를 검사한다.

            double betMoney  = Math.Round(resultMessage.BetMoney, 2);
            double winMoney  = Math.Round(resultMessage.WinMoney, 2);

            double beforeBalance = _balance;
            if(betMoney > 0.0 && !string.IsNullOrEmpty(resultMessage.TransactionID))
            {
                var response = await callBetWin(resultMessage.GameID, betMoney, winMoney, resultMessage.RoundID, resultMessage.BetTransactionID);
                if (response == null || response.code != 0)
                    return false;
                _balance = Math.Round((double)response.balance, 2);
            }
            else if (betMoney > 0.0)
            {
                var response = await callWithdraw(resultMessage.GameID, betMoney, resultMessage.RoundID, resultMessage.BetTransactionID);
                if (response == null || response.code != 0)
                    return false;
                _balance = Math.Round((double)response.balance, 2);
            }
            else if (!string.IsNullOrEmpty(resultMessage.TransactionID))
            {
                var response = await callDeposit(resultMessage.GameID, winMoney, resultMessage.RoundID, resultMessage.BetTransactionID, resultMessage.TransactionID);
                if (response == null || (response.code != 0 && response.code != 11))
                    _balance = _balance + winMoney;
                else
                    _balance = Math.Round((double)response.balance, 2);
            }
            if (betMoney != 0.0 || winMoney != 0.0)
            {
                addPoint((int)providerID, resultMessage.GameID, betMoney, winMoney);
                _dbWriter.Tell(new PlayerBalanceUpdateItem(_userDBID, _balance));                       
                _dbWriter.Tell(new ReportUpdateItem(_strUserID, _agentDBID, nowDayReportTime, betMoney, winMoney));
                _dbWriter.Tell(new GameLogItem(_strUserID, (int)resultMessage.GameID, resultMessage.GameLog.GameName, 
                                    betMoney, winMoney, beforeBalance, _balance, resultMessage.GameLog.LogString, (int) resultMessage.BetType, nowReportTime, _agentDBID));
            }
            return true;
        }

        private string buildActivePromos()
        {
            return "{\"error\":0,\"description\":\"OK\",\"serverTime\":1676462782,\"tournaments\":[],\"races\":[]}";
        }
        private GITMessage onPromoActive(GITMessage message)
        {
            GITMessage respondMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_PROMOACTIVE);
            try
            {
                string strGameSymbol = (string) message.Pop();
                respondMessage.Append(buildActivePromos());
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::onPromoActive {0}", ex);
            }
            return respondMessage;
        }
        private void procPPPromoMsg(GITMessage message, UserConnection userConn)
        {
            GITMessage respondMessage = null;
            switch ((CSMSG_CODE) message.MsgCode)
            {
                case CSMSG_CODE.CS_PP_PROMOACTIVE:
                    {
                        respondMessage = onPromoActive(message);
                    }
                    break;                
            }
            if (respondMessage != null)
                Sender.Tell(new SendMessageToUser(respondMessage, _balance, 0.0));
        }
#endregion

        private async Task replaceSlotGameNode(string strSlotNodePath, UserConnection userConn)
        {
            int remainServerCount = 0;
            try
            {
                var routees = await Context.System.ActorSelection(Constants.SlotGameRouterPath).Ask<Routees>(new GetRoutees());
                foreach (Routee routee in routees.Members)
                    remainServerCount++;

                _logger.Info("{0} Exiting from slot game node {1}", _strGlobalUserID, strSlotNodePath);
                ExitGameResponse response = await userConn.GameActor.Ask<ExitGameResponse>(new ExitGameRequest(_strUserID, _agentDBID, _balance, false, remainServerCount > 0), TimeSpan.FromSeconds(5));
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
                    userConn.GameProvider   = (int) GameProviders.NONE;
                    userConn.GameID         = 0;
                    return;
                }

                //다른 슬롯게임노드에 입장한다.
                EnterGameRequest  requestMsg  = new EnterGameRequest(userConn.GameID, _agentDBID, _strUserID, Self, false);
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
                string          strToken  = _userConnections.Keys.ElementAt(i);
                UserConnection  userConn  = _userConnections[strToken];

                if (userConn.GameActor == null || userConn.GameProvider == (int) GameProviders.NONE)
                    continue;

                if (!userConn.GameActor.Path.ToString().Contains(message.Path))
                    continue;

                await replaceSlotGameNode(message.Path, userConn);
            }            
        }

#region Messages
        public class UserLoggedIn
        {
        }
#endregion

        //유저의 연결객체(tcp, websocket, http session)
        public class UserConnection
        {
            public string       Token           { get; set; } //HTTP Session Token
            public IActorRef    GameActor       { get; set; } //입장한 게임액터(null: 게임에 입장하지 않은 상태)
            public int          GameID          { get; set; } //입장한 게임아이디(0: 게임에 입장하지 않은 상태)            
            public int          GameProvider    { get; set; } //입장한 게임사
            public DateTime     LastActiveTime  { get; set; } //유저가 마지막으로 서버와 접촉한 시간

            public UserConnection(string sessionToken)
            {
                this.Token          = sessionToken;
                this.GameActor      = null;
                this.GameID         = 0;
                this.GameProvider   = 0;
                this.LastActiveTime = DateTime.Now;
            }

            public void resetGame()
            {
                this.GameActor          = null;
                this.GameID             = 0;
                this.GameProvider       = 0;
            }
        }
    }
    public class PPPromoStatus
    {
        public string                   ActivePromos            { get; set; }
        public string                   TournamentDetails       { get; set; }
        public string                   RaceDetails             { get; set; }
        public string                   RaceWinners             { get; set; }
        public string                   RacePrizes              { get; set; }
        public int                      OpenTournamentID        { get; set; }
        public double                   OpenTournamentMinBet    { get; set; }
        public string                   TournamentLeaderboards  { get; set; }
        public string                   MiniLobbyGames          { get; set; }
        public PPPromoStatus(string strActivePromos, string strTournamentDetails, string strRaceDetails, string strRaceWinners,
            string strRacePrizes, int openTournamentID, double openTournamentMinBet, string tournamentLeaderboards, string miniLobbyGames)
        {
            this.ActivePromos           = strActivePromos;
            this.TournamentDetails      = strTournamentDetails;
            this.RaceDetails            = strRaceDetails;
            this.RaceWinners            = strRaceWinners;
            this.RacePrizes             = strRacePrizes;
            this.OpenTournamentID       = openTournamentID;
            this.OpenTournamentMinBet   = openTournamentMinBet;
            this.TournamentLeaderboards = tournamentLeaderboards;
            this.MiniLobbyGames         = miniLobbyGames;
        }

    }

}
