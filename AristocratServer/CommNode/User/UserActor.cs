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
using CommNode.HTTPService;
using System.Net.Http;
using Akka.Util.Internal;
using Newtonsoft.Json.Linq;
using PCGSharp;
using System.IO;
using Akka.Util;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Globalization;
using WebSocketSharp;
using System.ServiceModel.Channels;
using System.Windows.Input;

namespace CommNode
{
    public class UserActor : ReceiveActor
    {
        private long _userDBID = 0;
        private string _strUserID = "";
        private string _strUserToken = "";
        private double _balance = 0.0;
        private string _agentName = "";
        private int _agentDBID = 0;
        private int _agentMoneyMode = 0;
        private long _lastScoreID = 0;
        private string _ipAddress = "";
        private Currencies _currency = Currencies.MYR;
        private bool _userDisconnected = false;
        private Dictionary<object, UserConnection> _userConnections = new Dictionary<object, UserConnection>();
        private Dictionary<string, bool> receivedCashbacks = new Dictionary<string, bool>();


        private PPPromoStatus _ppPromoStatus;
        private UserReportItem _userReport = null;

        private IActorRef _dbReader = null;
        private IActorRef _dbWriter = null;
        private IActorRef _redisWriter = null;
        private readonly ILoggingAdapter _logger = Logging.GetLogger(Context);
        protected static RealExtensions.Epsilon _epsilion = new RealExtensions.Epsilon(0.001);
        private ICancelable _conCheckCancel = null;
        private IActorRef _afterQuitNotifyActor = null;

        public UserActor(CreateNewUserMessage message)
        {
            _dbReader = message.DBReader;
            _dbWriter = message.DBWriter;
            _redisWriter = message.RedisWriter;
            _userDBID = message.LoginResponse.UserDBID;
            _strUserID = message.LoginResponse.UserID;
            _balance = message.LoginResponse.Balance;
            _agentName = message.LoginResponse.AgentName;
            _agentDBID = message.LoginResponse.AgentID;

            _lastScoreID = message.LoginResponse.LastScoreID;
            _ipAddress = message.LoginResponse.IPAddress;
            _strUserToken = message.LoginResponse.UserToken;
            _currency = message.LoginResponse.AgentCurrency;
            _agentMoneyMode = message.LoginResponse.AgentMoneyMode;

            _userConnections.Add(message.Connection, new UserConnection(message.Connection));


            ReceiveAsync<UserLoggedIn>(onUserLoginSucceeded);
            ReceiveAsync<FromConnRevMessage>(onProcMessage);

            Receive<HttpSessionAdded>(onProcHttpSessionAdded);
            ReceiveAsync<HttpSessionClosed>(onProcHttpSessionClosed);
            ReceiveAsync<CloseHttpSession>(onCloseHttpSession);
            Receive<QuitUserMessage>(onForceLogoutMessage);
            Receive<string>(onCommand);
            ReceiveAsync<SlotGamesNodeShuttingdown>(onSlotGameServerShuttingdown);
            ReceiveAsync<PPPromoUpdateEvent>(onPromoUpdateEvent);
            Receive<ApiDepositMessage>(onApiDepositMoney);
            Receive<QuitAndNotifyMessage>(onForceQuitAndNotifyMessage);
            ReceiveAsync<CallbackGetBalanceRequest>(onCallbackGetBalance);
            ReceiveAsync<CallbackWithdrawRequest>(onCallbackWithdraw);
            ReceiveAsync<CallbackDepositRequest>(onCallbackDeposit);
            ReceiveAsync<CallbackRollbackRequest>(onCallbackRollback);
        }

        public static Props Props(CreateNewUserMessage message)
        {
            return Akka.Actor.Props.Create(() => new UserActor(message));
        }
        protected override void PreStart()
        {
            Self.Tell(new UserLoggedIn());
            Context.System.EventStream.Subscribe(Self, typeof(PPPromoUpdateEvent));
            base.PreStart();
        }
        protected override void PostStop()
        {
            Context.System.EventStream.Unsubscribe(Self, typeof(PPPromoUpdateEvent));
            if (_conCheckCancel != null)
                _conCheckCancel.Cancel();

            base.PostStop();
        }
        private void onForceQuitAndNotifyMessage(QuitAndNotifyMessage _)
        {
            _afterQuitNotifyActor = Sender;
            onForceLogoutMessage(new QuitUserMessage(_strUserID));
        }
        private void onApiDepositMoney(ApiDepositMessage request)
        {
            if (request.LastScoreCounter <= _lastScoreID)
                return;

            _lastScoreID = request.LastScoreCounter;
            _balance += request.Amount;
        }
        private async Task onUserLoginSucceeded(UserLoggedIn message)
        {
            try
            {
                _dbWriter.Tell(new UserLoginStateItem(_userDBID, (int)0));
                this.receivedCashbacks = await _dbReader.Ask<Dictionary<string, bool>>(new GetUserReceivedCashbacks(_strUserID));

                _ppPromoStatus = new PPPromoStatus();
                Context.System.ActorSelection("/user/promofetcher").Tell(new PPPromoGetStatus());

                _logger.Info("{0} has been logged in successfully", _strUserID);

                _conCheckCancel = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(1000, 10000, Self, "checkConn", Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::onUserLoginSucceeded {0}", ex);
            }
        }
        #region 각종 사건처리부

        private void onForceLogoutMessage(QuitUserMessage _)
        {
            GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_FORCEOUTUSER);

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
            if (strCommand == "checkConn")
            {
                foreach (KeyValuePair<object, UserConnection> pair in _userConnections)
                {
                    if (pair.Key is string)
                    {
                        if (DateTime.Now.Subtract(pair.Value.LastActiveTime) >= TimeSpan.FromMinutes(5))
                            Self.Tell(new CloseHttpSession(pair.Key as string));
                    }
                }
            }
        }
        private void addScore(long scoreID, double score)
        {
            if (scoreID <= _lastScoreID)
                return;

            _lastScoreID = scoreID;
            _balance += score;

            _dbWriter.Tell(new GameLogItem(_agentDBID, _strUserID, 0, "setscore", "", 0.0, 0.0, _balance - score, _balance, "", DateTime.UtcNow));

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
                ExitGameResponse response = null;
                try
                {
                    response = await userConn.GameActor.Ask<ExitGameResponse>(new ExitGameRequest(_strUserID, _agentDBID, Math.Round(_balance, 2), true, false), Constants.RemoteTimeOut);
                }
                catch (Exception ex)
                {
                    _logger.Warning("{0} exit game {1} Failed : Exception {2}", _strUserID, userConn.GameID, ex);
                }
                lastGameID = userConn.GameID;
            }

            _userConnections.Remove(userConn.Connection);

            if (_userConnections.Count == 0)
            {
                _dbWriter.Tell(new UserGameStateItem(_userDBID, 0, lastGameID));
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

                if (_afterQuitNotifyActor != null)
                {
                    _afterQuitNotifyActor.Tell(true);
                    _afterQuitNotifyActor = null;
                }
            }
        }
        #endregion

        private async Task onProcMessage(FromConnRevMessage fromConnRevMsg)
        {
            if (_userDisconnected)
                return;

            if (!_userConnections.ContainsKey(fromConnRevMsg.Connection))
                return;

            UserConnection userConn = _userConnections[fromConnRevMsg.Connection];
            GITMessage message = fromConnRevMsg.Message;
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
                await procPPPromoMsg(message, userConn);
            }
            else
            {
                _logger.Warning("Unknown paket received from {0} Message code:{1} User ID:{2}", _ipAddress, message.MsgCode, _strUserID);
            }
        }
        private async Task procEnterGame(GITMessage message, UserConnection userConn)
        {
            int gameID = (int)(ushort)message.Pop();
            GAMETYPE gameType = DBMonitorSnapshot.Instance.getGameType(gameID);

            if (gameType == GAMETYPE.NONE)
            {
                _logger.Warning("{0} tried to enter game for not registered game id {1}", _strUserID, (int)gameID);
                Sender.Tell("closeConnection");
                Self.Tell(new CloseHttpSession(userConn.Connection as string));
                return;
            }

            if (userConn.GameActor != null && userConn.GameID != gameID)
            {
                _logger.Warning("{0} tried to enter game while it has already been entered to other game", _strUserID);
                Sender.Tell("closeConnection");
                Self.Tell(new CloseHttpSession(userConn.Connection as string));
                return;
            }

            bool enterGameSucceeded = false;
            do
            {
                var agentAPIConfig = AgentSnapshot.Instance.findAgentConfig(_agentDBID);
                if (agentAPIConfig != null && agentAPIConfig.ApiMode == 1)
                {
                    var balanceResponse = await callGetBalance(gameID);
                    if (balanceResponse == null || balanceResponse.code != 0 || balanceResponse.balance < 0.0M)
                    {
                        enterGameSucceeded = false;
                        break;
                    }
                    _balance = Math.Round((double)balanceResponse.balance, 2);
                    _dbWriter.Tell(new PlayerBalanceResetItem(_userDBID, _balance));
                }

                if (userConn.GameActor != null && userConn.GameID == gameID)
                {
                    enterGameSucceeded = true;
                }
                else if (userConn.GameActor == null)
                {
                    UserConnection oldConn = null;
                    foreach (KeyValuePair<object, UserConnection> pair in _userConnections)
                    {
                        if (pair.Value.GameActor != null && pair.Value.GameID == gameID)
                        {
                            oldConn = pair.Value;
                            break;
                        }
                    }
                    if (oldConn != null)
                    {
                        userConn.GameActor = oldConn.GameActor;
                        userConn.GameID = gameID;
                        userConn.GameType = gameType;

                        oldConn.resetGame();
                        if (!oldConn.IsHttpSession)
                            (oldConn.Connection as IActorRef).Tell("closeConnection");
                        else
                            Self.Tell(new CloseHttpSession(oldConn.Connection as string));
                        enterGameSucceeded = true;
                    }
                    else
                    {
                        try
                        {
                            EnterGameRequest requestMsg = new EnterGameRequest(gameID, _strUserID, Self);
                            EnterGameResponse responseMsg = await Context.System.ActorSelection(Constants.SlotGameRouterPath).Ask<EnterGameResponse>(requestMsg, Constants.RemoteTimeOut);

                            if (responseMsg.Ack == 0)
                            {
                                userConn.GameActor = responseMsg.GameActor;
                                userConn.GameID = gameID;
                                userConn.GameType = gameType;
                                enterGameSucceeded = true;

                                _dbWriter.Tell(new UserGameStateItem(_userDBID, 2, (int)gameID));
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Warning("{0} enter slot game {1} Failed : Exception {2}", _strUserID, gameID, ex);
                        }
                    }
                }
            } while (false);

            GITMessage enterResponseMessage = new GITMessage((ushort)SCMSG_CODE.SC_ENTERGAME);
            enterResponseMessage.Append(enterGameSucceeded ? (byte)0 : (byte)1);
            Sender.Tell(new SendMessageToUser(enterResponseMessage, _balance));
        }
        private async Task procSlotGameMsg(GITMessage message, UserConnection userConn)
        {
            if (userConn.GameActor == null || userConn.GameType == GAMETYPE.NONE)
            {
                Self.Tell(new CloseHttpSession(userConn.Connection as string));
                if (userConn.IsHttpSession)
                    Sender.Tell("invalidaction");
                return;
            }

            UserBonus waitingBonus = null;

            ToUserMessage toUserMessage = null;
            try
            {
                toUserMessage = await userConn.GameActor.Ask<ToUserMessage>(new FromUserMessage(_strUserID, _agentDBID, Math.Round(_balance, 2), Self, message, waitingBonus, _currency, _agentMoneyMode), Constants.RemoteTimeOut);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in UserActor::procSlotGameMsg {0} gameid: {1}, message code: {2}, {3}",
                    _strUserID, userConn.GameID, message.MsgCode, ex);
            }

            if (toUserMessage != null)
            {
                await procToUserMessage(toUserMessage, message, userConn);
            }
            else
            {
                if (userConn.IsHttpSession)
                    Sender.Tell("nomessagefromslotnode");
            }
        }
        private async Task procToUserMessage(ToUserMessage message, GITMessage gameMessage, UserConnection userConn)
        {
            if (message is ToUserResultMessage)
            {
                bool isSuccess = await processResultMessage(message as ToUserResultMessage);
                if ((userConn.Connection is string) && (gameMessage.MsgCode == (ushort)CSMSG_CODE.CS_PP_DOSPIN || gameMessage.MsgCode == (ushort)CSMSG_CODE.CS_PP_DOBONUS))
                {
                    if (!isSuccess)
                    {
                        if (message.Messages.Count >= 2 && message.Messages[1] != null)
                            Sender.Tell(message.Messages[1]);

                        GITMessage errorResultMsg = new GITMessage((ushort)CSMSG_CODE.CS_PP_NOTPROCDRESULT);
                        userConn.GameActor.Tell(new FromUserMessage(_strUserID, _agentDBID, Math.Round(_balance, 2), Self, errorResultMsg, null, _currency, _agentMoneyMode));
                        return;
                    }
                }
            }
            for (int i = 0; i < message.Messages.Count; i++)
            {
                if (message.Messages[i] != null)
                {
                    Sender.Tell(new SendMessageToUser(message.Messages[i], _balance));
                    break;
                }
            }
        }
        private void addTurnover(double amount, int gameID)
        {
            DateTime utcNow = DateTime.UtcNow;
            _dbWriter.Tell(new GameReportItem(gameID, (int)_currency, amount, new DateTime(utcNow.Year, utcNow.Month, utcNow.Day)));
        }
        private async Task<bool> processResultMessage(ToUserResultMessage resultMessage)
        {
            DateTime nowReportTime = DateTime.UtcNow;
            DateTime nowHourReportTime = new DateTime(nowReportTime.Year, nowReportTime.Month, nowReportTime.Day, nowReportTime.Hour, 0, 0);

            double betMoney = Math.Round(resultMessage.BetMoney, 2);
            double winMoney = Math.Round(resultMessage.WinMoney, 2);
            double realBet = Math.Round(resultMessage.RealBet, 2);


            if (resultMessage.FreeSpinID != 0)
            {
                betMoney = 0;
                realBet = 0;
            }

            if (_agentMoneyMode == 1)
            {
                betMoney = Math.Round(betMoney / 100.0, 2);
                winMoney = Math.Round(winMoney / 100.0, 2);
                realBet = Math.Round(realBet / 100.0, 2);
            }

            var agentAPIConfig = AgentSnapshot.Instance.findAgentConfig(_agentDBID);
            double beforeBalance = _balance;

            if (realBet > 0.0)
                addTurnover(realBet, resultMessage.GameID);

            if (agentAPIConfig != null && agentAPIConfig.ApiMode == 1)
            {
                if (realBet > 0.0)
                {
                    var response = await callWithdraw(resultMessage.GameID, realBet, resultMessage.RoundID, resultMessage.BetTransactionID);
                    if (response == null || response.code != 0)
                        return false;
                    _balance = Math.Round((double)response.balance, 2);
                }
                if (!string.IsNullOrEmpty(resultMessage.TransactionID))
                {
                    var response = await callDeposit(resultMessage.GameID, winMoney, resultMessage.RoundID, resultMessage.BetTransactionID, resultMessage.TransactionID);
                    if (response == null || (response.code != 0 && response.code != 11))
                        _balance = _balance + winMoney;
                    else
                        _balance = Math.Round((double)response.balance, 2);
                }
                _dbWriter.Tell(new PlayerBalanceResetItem(_userDBID, _balance));
            }
            else
            {
                if (_balance.LT(realBet, _epsilion))
                    return false;

                if (realBet != 0.0 || winMoney != 0.0)
                {
                    _balance -= realBet;
                    if (!string.IsNullOrEmpty(resultMessage.TransactionID))
                        _balance += winMoney;

                    _dbWriter.Tell(new PlayerBalanceUpdateItem(_userDBID, _balance - beforeBalance));
                }
            }
            var reportUpdateItem = new ReportUpdateItem(_strUserID, _agentDBID, nowHourReportTime, realBet, winMoney, realBet);
            //_logger.Info($"REAL BET2:   {_userReport.DailyBet}, {_userReport.DailyRound}, {_userReport.DailyKey}");
            _dbWriter.Tell(reportUpdateItem);
            if(_userReport == null)
            {
                _userReport = new UserReportItem(_strUserID);
            }
            _userReport.mergeReport(reportUpdateItem);
            _redisWriter.Tell(new UserReportinfoWrite(_strUserID, _userReport.convertToByte(), false));

            string strRoundID = resultMessage.BetTransactionID;
            if(resultMessage.BetTransactionID.IsNullOrEmpty())
            {
                _logger.Error("BetTransactionID---> Null");
                strRoundID = "";
            }
            string strPromoData = getEventDataFromResultString(resultMessage.GameLog.LogString);
            double promoWin = getPromoWinMoneyFromPromo(strPromoData);
            if (!string.IsNullOrEmpty(resultMessage.TransactionID))
            {
                beforeBalance = _balance - winMoney + realBet;
                _dbWriter.Tell(new GameLogItem(_agentDBID, _strUserID, (int)resultMessage.GameID, resultMessage.GameLog.GameName,
                    strRoundID + strPromoData,
                    realBet, Math.Round(Math.Max(winMoney - promoWin, 0), 2),
                    Math.Round(beforeBalance, 2), Math.Round(_balance, 2)
                    , resultMessage.GameLog.LogString, nowReportTime));
            }
            else if (realBet > 0.0)
            {
                beforeBalance = _balance + realBet;
                _dbWriter.Tell(new GameLogItem(_agentDBID, _strUserID, (int)resultMessage.GameID, resultMessage.GameLog.GameName,
                    strRoundID,
                    Math.Round(realBet, 2), 0.0,
                    Math.Round(beforeBalance, 2), Math.Round(_balance, 2)
                    , "", nowReportTime));
            }

            //checkPPTournamentValid(_agentName, _strUserID, resultMessage.GameID, resultMessage.BetMoney, resultMessage.RealBet, resultMessage.WinMoney);
            return true;
        }
        protected void checkPPTournamentValid(string agentID, string userName, int gameID, double betMoney, double realBetMoney, double winMoney)
        {
            foreach (PPActiveTournament detail in PPPromoSnapshot.Instance.ActivePromos.tournaments)
            {
                int isAgent = string.IsNullOrEmpty(detail.agentid) ? 0 : 1;
                int index1 = PPPromoSnapshot.Instance.TournamentList.FindIndex(_ => _.id == detail.id);
                if (index1 == -1)
                    continue;

                PPTournament selectedTournament = PPPromoSnapshot.Instance.TournamentList[index1];
                string[] games = selectedTournament.games.Split(',');
                string[] playersinc = selectedTournament.playersinc.Split(',');
                string[] playersexc = selectedTournament.playersexc.Split(',');

                if (!string.IsNullOrEmpty(selectedTournament.games) && !games.Contains(((int)gameID).ToString()))
                    continue;

                if (!string.IsNullOrEmpty(selectedTournament.playersinc) && !playersinc.Contains(userName))
                    continue;

                if (!string.IsNullOrEmpty(selectedTournament.playersexc) && playersexc.Contains(userName))
                    continue;

                int index2 = PPPromoSnapshot.Instance.TournamentList[index1].leaderBoard.FindIndex(_ => _.playerID == userName);

                PPTournamentLeaderItem selectedItem = new PPTournamentLeaderItem();
                if (index2 != -1)
                    selectedItem = PPPromoSnapshot.Instance.TournamentList[index1].leaderBoard[index2];

                double addScore = 0.0;

                if (detail.type == (int)PPTourType.HighestSingleSpinWinAmount && selectedItem.score < winMoney)
                    addScore = winMoney - selectedItem.score;
                else if (detail.type == (int)PPTourType.HighestSingleSpinWinRate && betMoney > 0 && selectedItem.score <= winMoney / betMoney)
                    addScore = (winMoney / betMoney) - selectedItem.score;
                else if (detail.type == (int)PPTourType.HighestTotalofAllBetAmounts && betMoney > 0)
                    addScore = betMoney;
                else if (detail.type == (int)PPTourType.HighestTotalofAllWinAmounts && winMoney > 0)
                    addScore = winMoney;
                else if (detail.type == (int)PPTourType.HighestTotalofAllWinRates && selectedItem.totalbet + betMoney > 0)
                    addScore = (selectedItem.totalwin + winMoney) / (selectedItem.totalbet + betMoney) - selectedItem.score;

                _dbWriter.Tell(new PPTourLeaderboardDBItem(agentID, isAgent, detail.id, detail.type, userName, ((Currencies)detail.currency).ToString().Substring(0, 2),
                    ((Currencies)detail.currency).ToString(), realBetMoney, winMoney, addScore, betMoney, DateTime.UtcNow));
            }
        }

        private string getEventDataFromResultString(string resultString)
        {
            try
            {
                string lastResultResponse = string.Empty;
                if (!resultString.StartsWith("["))
                {
                    lastResultResponse = resultString;
                }
                else
                {
                    List<PPGameLogType> logList = JsonConvert.DeserializeObject<List<PPGameLogType>>(resultString);
                    if (logList.Count > 1)
                        lastResultResponse = logList[logList.Count - 2].sr;
                    else
                        lastResultResponse = logList.Last().sr;
                }

                Dictionary<string, string> dicResponse = splitResponseToParams(lastResultResponse);

                string strKey = "ev";
                if (dicResponse.ContainsKey(strKey))
                {
                    string strValue = dicResponse[strKey];
                    strValue = strValue.Replace("MR", "PRZ");

                    return "::" + strValue;
                }
            }
            catch (Exception ex)
            {
            }
            return "";
        }

        private double getPromoWinMoneyFromPromo(string strPromoData)
        {
            if (string.IsNullOrEmpty(strPromoData))
                return 0.0;

            try
            {
                string[] promos = strPromoData.Split(',');
                return double.Parse(promos[2]);
            }
            catch (Exception ex)
            {
            }
            return 0.0;
        }

        private Dictionary<string, string> splitResponseToParams(string strResponse)
        {
            string[] strParts = strResponse.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            if (strParts == null || strParts.Length == 0)
                return new Dictionary<string, string>();

            Dictionary<string, string> dicParamValues = new Dictionary<string, string>();
            for (int i = 0; i < strParts.Length; i++)
            {
                string[] strParamValues = strParts[i].Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (strParamValues.Length == 2)
                    dicParamValues[strParamValues[0]] = strParamValues[1];
                else if (strParamValues.Length == 1)
                    dicParamValues[strParamValues[0]] = null;
            }
            return dicParamValues;
        }

        #region 보너스관련 함수들
        private UserBonus pickUserBonus(int gameID)
        {
            try
            {
                //PPActivePromos activePromos = JsonConvert.DeserializeObject<PPActivePromos>(_ppPromoStatus.ActivePromos);
                PPActivePromos activePromos = _ppPromoStatus.ActivePromos;
                var prizeDrops = activePromos.races.Where(r => r.type == 0).ToList();
                int openedRaceCnt = 0;
                foreach (PPActiveRace race in prizeDrops)
                {
                    if (race.status == "O" && (string.IsNullOrEmpty(race.agentid) || race.agentid == _agentName))
                        openedRaceCnt++;
                }

                if (openedRaceCnt == 0)
                    return null;

                int rndRace = Pcg.Default.Next(0, openedRaceCnt);

                int selectedRaceIndex = 0;
                PPActiveRace selectedRace = null;

                foreach (PPActiveRace race in prizeDrops)
                {
                    if (race.status != "O" || !(string.IsNullOrEmpty(race.agentid) || race.agentid == _agentName))
                        continue;

                    if (selectedRaceIndex == rndRace)
                    {
                        selectedRace = race;
                        break;
                    }
                    selectedRaceIndex++;
                }

                if (selectedRace == null)
                    return null;

                string strWinnersInfo = _ppPromoStatus.ActiveRaceWinners[selectedRace.id];
                PPRaceWinnerInfo raceWinnerInfo = JsonConvert.DeserializeObject<PPRaceWinnerInfo>(strWinnersInfo);

                if (selectedRace.prizeslimit != 0)
                {
                    int myWinCnt = 0;
                    foreach (PPRaceWinner item in raceWinnerInfo.items)
                    {
                        if (item.playerID == _strUserID)
                            myWinCnt++;
                    }

                    if (myWinCnt >= selectedRace.prizeslimit)
                        return null;
                }

                if (!string.IsNullOrEmpty(selectedRace.agentid) && selectedRace.agentid != _agentName)
                    return null;

                if (!string.IsNullOrEmpty(selectedRace.games))
                {
                    try
                    {
                        string[] gameids = selectedRace.games.Split(',');
                        string curGameID = gameID.ToString();
                        if (!gameids.Contains(curGameID))
                            return null;
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }

                if (!string.IsNullOrEmpty(selectedRace.playersinc))
                {
                    try
                    {
                        string[] playerIncs = selectedRace.playersinc.Split(',');

                        if (!playerIncs.Contains(_strUserID))
                            return null;
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
                else if (!string.IsNullOrEmpty(selectedRace.playersexc))
                {
                    try
                    {
                        string[] playerExcs = selectedRace.playersexc.Split(',');

                        if (playerExcs.Contains(_strUserID))
                            return null;
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }

                PPRaceDetails ppRaceDetail = _ppPromoStatus.ActiveRaceDetails;
                PPRaceDetail selectedDetail = ppRaceDetail.details.Find(_ => _.id == selectedRace.id);
                if (selectedDetail == null)
                    return null;

                int prizeTypeCnt = selectedDetail.prizePool.prizesList.Count;
                if (prizeTypeCnt == 0)
                    return null;

                DateTime currentTime = DateTime.Now;

                double ticks = selectedRace.endDate * 1000.0;
                DateTime enddate = (new DateTime(1970, 1, 1)).AddMilliseconds(ticks);
                TimeSpan remainingTime = enddate - currentTime;

                if (remainingTime.TotalSeconds <= 0)
                {
                    _logger.Info("Selected Race has been finshed! RaceID : {0}", selectedDetail.id);
                    return null;
                }

                List<PPRacePrize> notFullPrizes = new List<PPRacePrize>();
                int prizeLeftCnt = 0;
                for (int i = 0; i < selectedDetail.prizePool.prizesList.Count; i++)
                {
                    int selectedPrizeTotalWinCnt = 0;
                    foreach (PPRaceWinner item in raceWinnerInfo.items)
                    {
                        if (item.prizeID == selectedDetail.prizePool.prizesList[i].prizeID)
                            selectedPrizeTotalWinCnt++;
                    }

                    prizeLeftCnt += (selectedDetail.prizePool.prizesList[i].count - selectedPrizeTotalWinCnt);
                    if (selectedPrizeTotalWinCnt < selectedDetail.prizePool.prizesList[i].count)
                        notFullPrizes.Add(selectedDetail.prizePool.prizesList[i]);
                }

                if (notFullPrizes.Count == 0)
                    return null;

                double rndDouble = Pcg.Default.NextDouble() / 1.5;
                double calcDouble = prizeLeftCnt / remainingTime.TotalMinutes;

                if (!string.IsNullOrEmpty(selectedRace.playersinc))
                    calcDouble = 0.1;

                if (calcDouble < rndDouble)
                    return null;

                _logger.Info("Random Val : {0}, Calc Val : {1}", rndDouble, calcDouble);
                PPRacePrize selectedPrize = notFullPrizes[Pcg.Default.Next(0, notFullPrizes.Count)];

                double altMinBetLimit = getRaceAltMinBetLimit(selectedDetail);
                double altPrizeAmount = getRaceAltPrizeAmount(selectedDetail, selectedPrize);

                int isAgent = (selectedRace.agentid == _agentName) ? 1 : 0;
                return new UserPPRacePrizeBonus(123123, ((Country)_currency).ToString(), _currency.ToString(), selectedRace.id, selectedPrize.prizeID, _agentName, selectedPrize.type, isAgent, altPrizeAmount, selectedPrize.betMultiplier, altMinBetLimit, selectedDetail.prizePool.maxBetLimitByMultiplier);
            }
            catch (Exception ex)
            {
                _logger.Info("Exception has been occured in pickUserBonus : {0}", ex);
                return null;
            }
        }

        private async Task<UserBonus> pickFreeSpin(int gameID)
        {
            try
            {
                PPFreeSpinInfo playingFreeSpinInfo = null;
                string strKey = string.Format("{0}_{1}_freespin", _strUserID, gameID);
                byte[] freeSpinInfoData = await RedisDatabase.RedisCache.StringGetAsync(strKey);
                if (freeSpinInfoData != null)
                {
                    using (var stream = new MemoryStream(freeSpinInfoData))
                    {
                        BinaryReader reader = new BinaryReader(stream);
                        playingFreeSpinInfo = new PPFreeSpinInfo();
                        playingFreeSpinInfo.SerializeFrom(reader);
                    }
                }

                int[] completedFreeSpinIDs = await _dbReader.Ask<int[]>(new GetPPFreeSpinReportsRequest(_strUserID));

                var freeSpins = _ppPromoStatus.ActiveFreeSpins;
                var userFreespins = freeSpins.Where(fs =>
                (playingFreeSpinInfo == null ? true : fs.id != playingFreeSpinInfo.BonusID)
                && (!completedFreeSpinIDs.Contains(fs.id))
                && (fs.agentid.IsNullOrEmpty() ? true : fs.agentid == _agentName)
                && (fs.games.IsNullOrEmpty() ? true : fs.games.Split(',').Contains(gameID.ToString()))
                && ((fs.playersinc.IsNullOrEmpty() && fs.playersexc.IsNullOrEmpty()) ? true :
                (fs.playersinc.IsNullOrEmpty() ? fs.playersexc.Split(',').Contains(_strUserID) : fs.playersinc.Split(',').Contains(_strUserID)))
                ).ToList();
                // Check and Remove received Freespins.
                if (userFreespins.Count() > 0)
                {
                    var resultFreeSpin = userFreespins[0];
                    return new UserFreeSpinBonus(resultFreeSpin.id, _agentName, ((Country)_currency).ToString(), gameID, resultFreeSpin.level, resultFreeSpin.fscount, resultFreeSpin.endDate);
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                _logger.Info("Exception has been occured in pickFreeSpin : {0}", ex);
                return null;
            }
        }

        private async Task<UserBonus> pickCashback(int gameID)
        {
            if (_userReport == null)
            {
                string strKey = string.Format("{0}_report", _strUserID);
                byte[] userReportData = await RedisDatabase.RedisCache.StringGetAsync(strKey);
                if (userReportData != null)
                {
                    using (var stream = new MemoryStream(userReportData))
                    {
                        BinaryReader reader = new BinaryReader(stream);
                        _userReport = new UserReportItem(_strUserID);
                        _userReport.SerializeFrom(reader);
                    }
                }
                else
                {
                    _userReport = new UserReportItem(_strUserID);
                }
            }
            UserBonus bonus = null;
            PPActivePromos activePromos = _ppPromoStatus.ActivePromos;
            var cashbacks = activePromos.races.Where(r => r.type == 1).ToList();

            foreach (PPActiveRace cashback in cashbacks)
            {

                if (cashback.status != "O" || (!string.IsNullOrEmpty(cashback.agentid) && cashback.agentid != _agentName))
                    continue;


                if (!string.IsNullOrEmpty(cashback.playersinc))
                {

                    string[] playerIncs = cashback.playersinc.Split(',');

                    if (!playerIncs.Contains(_strUserID))
                        continue;

                }
                else if (!string.IsNullOrEmpty(cashback.playersexc))
                {
                    string[] playerExcs = cashback.playersexc.Split(',');

                    if (playerExcs.Contains(_strUserID))
                        continue;
                }

                double net = 0;
                int rounds = 0;
                string ckey = "";

                if (cashback.cType == 0)
                {
                    if (cashback.period == 0)
                    {
                        net = _userReport.DailyBet - _userReport.DailyWin;
                        rounds = _userReport.DailyRound;
                        ckey = $"{cashback.id}_{_userReport.DailyKey}";
                    }
                    else if (cashback.period == 1)
                    {
                        net = _userReport.WeeklyBet - _userReport.WeeklyWin;
                        rounds = _userReport.WeeklyRound;
                        ckey = $"{cashback.id}_{_userReport.WeeklyKey}";
                    }
                    else if (cashback.period == 2)
                    {
                        net = _userReport.MonthlyBet - _userReport.MonthlyWin;
                        rounds = _userReport.MonthlyRound;
                        ckey = $"{cashback.id}_{_userReport.MonthlyKey}";
                    }

                }
                else
                {

                    if (cashback.period == 0)
                    {
                        net = _userReport.DailyWin - _userReport.DailyBet;
                        rounds = _userReport.DailyRound;
                        ckey = $"{cashback.id}_{_userReport.DailyKey}";
                    }
                    else if (cashback.period == 1)
                    {
                        net = _userReport.WeeklyWin - _userReport.WeeklyBet;
                        rounds = _userReport.WeeklyRound;
                        ckey = $"{cashback.id}_{_userReport.WeeklyKey}";
                    }
                    else if (cashback.period == 2)
                    {
                        net = _userReport.MonthlyWin - _userReport.MonthlyBet;
                        rounds = _userReport.MonthlyRound;
                        ckey = $"{cashback.id}_{_userReport.MonthlyKey}";
                    }
                }

                this._logger.Info($"PickCashback UserReport: {net}-->{_userReport.DailyKey}, {_userReport.DailyBet}, {_userReport.DailyWin}, " +
                    $"{_userReport.DailyRound}--- {cashback.minNet}, {cashback.rounds}, containsKey: {receivedCashbacks.ContainsKey(ckey)}");
                if (net > cashback.minNet && rounds > cashback.rounds && !receivedCashbacks.ContainsKey(ckey))
                {
                    int isAgent = (cashback.agentid == _agentName) ? 1 : 0;
                    string periodKey = _userReport.DailyKey;
                    if (cashback.period == 1)
                    {
                        periodKey = _userReport.WeeklyKey;
                    }
                    else if (cashback.period == 2)
                    {
                        periodKey = _userReport.MonthlyKey;
                    }
                    bonus = new UserPPCashback(123, cashback.id, Math.Round(net / 100 * 10, 2), _agentName, ((Country)_currency).ToString(), _currency.ToString(),
                        isAgent, cashback.period, periodKey);

                    break;
                }

            }
            if (bonus != null && bonus is UserPPCashback)
            {
                this._logger.Info("Picked Cashback");
            }


            return bonus;

        }
        #endregion

        #region 심리스콜백부분
        public static string createDataSign(string key, string message)
        {
            var hmac = System.Security.Cryptography.HMAC.Create("HMACSHA256");
            hmac.Key = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            return BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();
        }
        private async Task<SeamlessGetBalanceResponse> callGetBalance(int gameID)
        {
            try
            {
                AgentAPIConfig agentConfig = AgentSnapshot.Instance.findAgentConfig(_agentDBID);
                if (agentConfig == null)
                    return null;

                SeamlessGetBalanceRequest request = new SeamlessGetBalanceRequest();
                request.agentID = _agentName;
                request.userID = _strUserID;
                request.gameID = gameID;
                request.sign = createDataSign(agentConfig.SecretKey, string.Format("{0}{1}{2}", _agentName, _strUserID, gameID));

                HttpClient httpClient = new HttpClient();
                string strURL = string.Format("{0}/GetBalance", agentConfig.CallbackURL);

                HttpResponseMessage message = await httpClient.PostAsync(strURL, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
                message.EnsureSuccessStatusCode();

                string strContent = await message.Content.ReadAsStringAsync();
                SeamlessGetBalanceResponse response = JsonConvert.DeserializeObject<SeamlessGetBalanceResponse>(strContent);
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::callGetBalance {0}", ex);
                return null;
            }
        }
        private async Task<SeamlessWithdrawResponse> callWithdraw(int gameID, double amount, string roundID, string transactionID)
        {
            AgentAPIConfig agentConfig = AgentSnapshot.Instance.findAgentConfig(_agentDBID);
            if (agentConfig == null)
                return null;

            try
            {

                SeamlessWithdrawRequest request = new SeamlessWithdrawRequest();
                request.agentID = _agentName;
                request.userID = _strUserID;
                request.gameID = gameID;
                request.amount = (decimal)Math.Round(amount, 2);
                request.roundID = roundID;
                request.transactionID = transactionID;
                request.sign = createDataSign(agentConfig.SecretKey, string.Format("{0}{1}{2}{3}{4}{5}", _agentName, _strUserID, request.amount.ToString("0.00"),
                    transactionID, roundID, gameID));

                HttpClient httpClient = new HttpClient();
                string strURL = string.Format("{0}/Withdraw", agentConfig.CallbackURL);

                HttpResponseMessage message = await httpClient.PostAsync(strURL, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
                message.EnsureSuccessStatusCode();
                string strContent = await message.Content.ReadAsStringAsync();

                var response = JsonConvert.DeserializeObject<SeamlessWithdrawResponse>(strContent);
                if (response.code == 0)
                    _dbWriter.Tell(new ApiTransactionItem(_agentName, _strUserID, gameID, amount, transactionID, "", response.platformTransactionID, roundID, TransactionTypes.Withdraw, DateTime.UtcNow));

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::callWithdraw {0}", ex);
            }

            Context.System.ActorSelection("/user/retryWorkers").Tell(new RollbackRequestWithRetry(agentConfig.CallbackURL,
                _agentName, _strUserID, agentConfig.SecretKey, gameID, 50, DateTime.Now.Subtract(TimeSpan.FromSeconds(10.0)), transactionID, amount));

            return null;
        }
        private async Task<SeamlessDepositResponse> callDeposit(int gameID, double amount, string roundID, string betTransactionID, string transactionID)
        {
            AgentAPIConfig agentConfig = AgentSnapshot.Instance.findAgentConfig(_agentDBID);
            if (agentConfig == null)
                return null;

            try
            {

                SeamlessDepositRequest request = new SeamlessDepositRequest();
                request.agentID = _agentName;
                request.userID = _strUserID;
                request.gameID = gameID;
                request.amount = (decimal)Math.Round(amount, 2);
                request.roundID = roundID;
                request.transactionID = transactionID;
                request.refTransactionID = betTransactionID;

                request.sign = createDataSign(agentConfig.SecretKey, string.Format("{0}{1}{2}{3}{4}{5}{6}", _agentName, _strUserID, request.amount.ToString("0.00"),
                    betTransactionID, transactionID, roundID, gameID));

                HttpClient httpClient = new HttpClient();
                string strURL = string.Format("{0}/Deposit", agentConfig.CallbackURL);

                HttpResponseMessage message = await httpClient.PostAsync(strURL, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
                message.EnsureSuccessStatusCode();
                string strContent = await message.Content.ReadAsStringAsync();

                var response = JsonConvert.DeserializeObject<SeamlessDepositResponse>(strContent);
                if (response.code == 0 || response.code == 11)
                    _dbWriter.Tell(new ApiTransactionItem(_agentName, _strUserID, gameID, amount, transactionID, betTransactionID, response.platformTransactionID, roundID, TransactionTypes.Deposit, DateTime.UtcNow));
                else
                    _dbWriter.Tell(new FailedTransactionItem(_agentName, _strUserID, TransactionTypes.Deposit, transactionID, betTransactionID, amount, gameID, DateTime.UtcNow));
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::callDeposit {0}", ex);
            }

            _dbWriter.Tell(new ApiTransactionItem(_agentName, _strUserID, gameID, amount, transactionID, betTransactionID, "", roundID, TransactionTypes.Deposit, DateTime.UtcNow));
            Context.System.ActorSelection("/user/retryWorkers").Tell(new DepositRequestWithRetry(agentConfig.CallbackURL,
                _agentName, _strUserID, agentConfig.SecretKey, gameID, 50, DateTime.Now.Subtract(TimeSpan.FromSeconds(10.0)), transactionID, betTransactionID, roundID, amount));

            return null;
        }
        private async Task onCallbackGetBalance(CallbackGetBalanceRequest request)
        {
            try
            {
                string strToken = _strUserID + "apiToken";
                if (_userConnections.ContainsKey(strToken))
                    _userConnections[strToken].LastActiveTime = DateTime.Now;

                var agentAPIConfig = AgentSnapshot.Instance.findAgentConfig(_agentDBID);
                if (agentAPIConfig != null && agentAPIConfig.ApiMode == 1)
                {
                    var response = await callGetBalance(request.gameID);
                    if (response == null)
                        Sender.Tell(new CallbackGetBalanceResponse(1, "General Error", 0.0M));
                    else
                        Sender.Tell(new CallbackGetBalanceResponse(response.code, response.message, response.balance));
                }
                else
                {
                    Sender.Tell(new CallbackGetBalanceResponse(0, "", (decimal)_balance));
                }
            }
            catch
            {

            }
        }
        private async Task onCallbackWithdraw(CallbackWithdrawRequest request)
        {
            try
            {
                string strToken = _strUserID + "apiToken";
                if (_userConnections.ContainsKey(strToken))
                    _userConnections[strToken].LastActiveTime = DateTime.Now;

                var agentAPIConfig = AgentSnapshot.Instance.findAgentConfig(_agentDBID);
                double amount = (double)Math.Round(request.amount, 2);
                if (agentAPIConfig != null && agentAPIConfig.ApiMode == 1)
                {
                    var response = await callWithdraw(request.gameID, amount, request.roundID, request.transactionID);
                    if (response == null)
                    {
                        Sender.Tell(new CallbackWithdrawResponse(1, "General Error"));
                        return;
                    }
                    else if (response.code != 0)
                    {
                        Sender.Tell(new CallbackWithdrawResponse(response.code, response.message));
                        return;
                    }
                    _balance = (double)Math.Round(response.balance, 2);
                    _dbWriter.Tell(new PlayerBalanceResetItem(_userDBID, _balance));

                    var callbackResponse = new CallbackWithdrawResponse(0, "");
                    callbackResponse.balance = (decimal)Math.Round(_balance, 2);
                    callbackResponse.platformTransactionID = response.platformTransactionID;
                    Sender.Tell(callbackResponse);
                }
                else
                {
                    var redisValue = await RedisDatabase.RedisCache.StringGetAsync(request.transactionID);
                    if (!redisValue.IsNullOrEmpty)
                    {
                        Sender.Tell(new CallbackWithdrawResponse(11, "Duplicate transaction"));
                        return;
                    }
                    if (_balance.LT(amount, _epsilion))
                    {
                        Sender.Tell(new CallbackWithdrawResponse(6, "Insufficient funds"));
                        return;
                    }

                    _balance -= amount;
                    _dbWriter.Tell(new PlayerBalanceUpdateItem(_userDBID, -amount));
                    await RedisDatabase.RedisCache.StringSetAsync(request.transactionID, request.amount.ToString("0.00"), TimeSpan.FromMinutes(30.0));
                    var response = new CallbackWithdrawResponse(0, "");
                    response.balance = (decimal)Math.Round(_balance, 2);
                    response.platformTransactionID = Guid.NewGuid().ToString().Replace("-", "");
                    Sender.Tell(response);
                }

                DateTime nowReportTime = DateTime.UtcNow;
                DateTime nowHourReportTime = new DateTime(nowReportTime.Year, nowReportTime.Month, nowReportTime.Day, nowReportTime.Hour, 0, 0);
                _dbWriter.Tell(new ReportUpdateItem(_strUserID, _agentDBID, nowHourReportTime, amount, 0.0, amount));
                _dbWriter.Tell(new GameLogItem(_agentDBID, _strUserID, request.gameID, request.roundID, request.transactionID,
                    amount, 0.0,
                    Math.Round(_balance + amount, 2), Math.Round(_balance, 2)
                    , "Withdraw", nowReportTime));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::onCallbackWithdraw {0}", ex);
            }
        }
        protected async Task onCallbackDeposit(CallbackDepositRequest request)
        {
            try
            {
                string strToken = _strUserID + "apiToken";
                if (_userConnections.ContainsKey(strToken))
                    _userConnections[strToken].LastActiveTime = DateTime.Now;

                var agentAPIConfig = AgentSnapshot.Instance.findAgentConfig(_agentDBID);
                double amount = (double)Math.Round(request.amount, 2);
                DateTime nowReportTime = DateTime.UtcNow;
                DateTime nowHourReportTime = new DateTime(nowReportTime.Year, nowReportTime.Month, nowReportTime.Day, nowReportTime.Hour, 0, 0);

                if (agentAPIConfig != null && agentAPIConfig.ApiMode == 1)
                {
                    var response = await callDeposit(request.gameID, amount, request.roundID, request.refTransactionID, request.transactionID);
                    if (response == null || response.code != 0 || response.code != 11)
                        _balance = _balance + amount;
                    else
                        _balance = Math.Round((double)response.balance, 2);

                    _dbWriter.Tell(new PlayerBalanceResetItem(_userDBID, _balance));
                    if (response == null)
                    {
                        Sender.Tell(new CallbackDepositResponse(1, "General Error"));
                        return;
                    }
                    else
                    {
                        var callbackResponse = new CallbackDepositResponse(response.code, response.message);
                        callbackResponse.balance = response.balance;
                        callbackResponse.platformTransactionID = response.platformTransactionID;

                        Sender.Tell(callbackResponse);
                        if (response.code != 0)
                            return;
                    }
                }
                else
                {
                    var redisValue = await RedisDatabase.RedisCache.StringGetAsync(request.transactionID);
                    if (!redisValue.IsNullOrEmpty)
                    {
                        var depositResponse = new CallbackDepositResponse(11, "Duplicate transaction");
                        depositResponse.balance = (decimal)Math.Round(_balance, 2);
                        depositResponse.platformTransactionID = Guid.NewGuid().ToString().Replace("-", "");
                        Sender.Tell(depositResponse);
                        return;
                    }
                    _balance += amount;
                    _dbWriter.Tell(new PlayerBalanceUpdateItem(_userDBID, amount));
                    await RedisDatabase.RedisCache.StringSetAsync(request.transactionID, "Deposit", TimeSpan.FromMinutes(30.0));
                    var response = new CallbackDepositResponse(0, "");
                    response.balance = (decimal)Math.Round(_balance, 2);
                    response.platformTransactionID = Guid.NewGuid().ToString().Replace("-", "");
                    Sender.Tell(response);
                }
                _dbWriter.Tell(new ReportUpdateItem(_strUserID, _agentDBID, nowHourReportTime, 0.0, amount, 0.0));
                _dbWriter.Tell(new GameLogItem(_agentDBID, _strUserID, request.gameID, request.roundID, request.transactionID,
                    0.0, amount,
                    Math.Round(_balance - amount, 2), Math.Round(_balance, 2)
                    , "Deposit", nowReportTime));

            }
            catch
            {

            }
        }
        protected async Task onCallbackRollback(CallbackRollbackRequest request)
        {
            try
            {
                string strToken = _strUserID + "apiToken";
                if (_userConnections.ContainsKey(strToken))
                    _userConnections[strToken].LastActiveTime = DateTime.Now;

                var agentAPIConfig = AgentSnapshot.Instance.findAgentConfig(_agentDBID);
                double amount = 0.0;
                if (agentAPIConfig != null && agentAPIConfig.ApiMode == 1)
                {
                    var rollbackRequest = new SeamlessRollbackRequest();
                    rollbackRequest.agentID = _agentName;
                    rollbackRequest.userID = _strUserID;
                    rollbackRequest.refTransactionID = request.refTransactionID;
                    rollbackRequest.gameID = request.gameID;
                    rollbackRequest.sign = createDataSign(agentAPIConfig.SecretKey, string.Format("{0}{1}{2}{3}",
                        _agentName, _strUserID, request.refTransactionID, request.gameID));

                    HttpClient httpClient = new HttpClient();
                    string strURL = string.Format("{0}/RollbackTransaction", agentAPIConfig.CallbackURL);

                    HttpResponseMessage message = await httpClient.PostAsync(strURL, new StringContent(JsonConvert.SerializeObject(rollbackRequest), Encoding.UTF8, "application/json"));
                    message.EnsureSuccessStatusCode();

                    string strContent = await message.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<SeamlessRollbackResponse>(strContent);
                    Sender.Tell(new CallbackRollbackResponse(response.code, response.message));
                }
                else
                {
                    var redisValue = await RedisDatabase.RedisCache.StringGetAsync(request.refTransactionID);
                    if (redisValue.IsNullOrEmpty)
                    {
                        Sender.Tell(new CallbackRollbackResponse(8, "Could not find reference transaction id"));
                        return;
                    }
                    if (redisValue == "Rollbacked")
                    {
                        Sender.Tell(new CallbackRollbackResponse(9, "Transaction is already rolled back"));
                        return;
                    }
                    amount = double.Parse(redisValue);
                    _balance += amount;
                    _dbWriter.Tell(new PlayerBalanceUpdateItem(_userDBID, amount));
                    await RedisDatabase.RedisCache.StringSetAsync(request.refTransactionID, "Rollbacked", TimeSpan.FromMinutes(30.0));
                    Sender.Tell(new CallbackRollbackResponse(0, ""));

                    DateTime nowReportTime = DateTime.UtcNow;
                    DateTime nowHourReportTime = new DateTime(nowReportTime.Year, nowReportTime.Month, nowReportTime.Day, nowReportTime.Hour, 0, 0);
                    _dbWriter.Tell(new ReportUpdateItem(_strUserID, _agentDBID, nowHourReportTime, 0.0, amount, 0.0));
                    _dbWriter.Tell(new GameLogItem(_agentDBID, _strUserID, request.gameID, "", request.refTransactionID,
                        0.0, amount,
                        Math.Round(_balance - amount, 2), Math.Round(_balance, 2)
                        , "Rollback", nowReportTime));
                }
            }
            catch
            {

            }
        }
        #endregion

        #region 토너먼트,프라이즈드롭(레이스) 관련부분
        private async Task onPromoUpdateEvent(PPPromoUpdateEvent updateEvent)
        {
            try
            {

                _ppPromoStatus.ActivePromos = updateEvent.ActivePromos;
                _ppPromoStatus.ActiveTournamentDetails = updateEvent.TournamentDetails;
                _ppPromoStatus.ActiveRaceDetails = updateEvent.RaceDetails;
                _ppPromoStatus.ActiveRacePrizes = updateEvent.RacePrizes;
                _ppPromoStatus.CurrencyMap = new Dictionary<string, double>(updateEvent.CurrencyMap);
                _ppPromoStatus.ActiveRaceWinners = new Dictionary<int, string>(updateEvent.RaceWinners);
                _ppPromoStatus.DicRaceWinnersHistory = new Dictionary<int, Dictionary<string, string>>(updateEvent.DicRaceWinnersString);
                _ppPromoStatus.ActiveFreeSpins = updateEvent.ActiveFreeSpins;

                await addUserBalanceForTournament();

                if (_ppPromoStatus.OpenTournamentID != updateEvent.OpenTournament)
                {
                    try
                    {
                        string strTournamentInfoKey = string.Format("tournament_{0}_info", updateEvent.OpenTournament);
                        RedisValue tournamentInfoValue = await RedisDatabase.RedisCache.HashGetAsync(strTournamentInfoKey, _strUserID);
                        if (tournamentInfoValue.IsNullOrEmpty)
                        {
                            _ppPromoStatus.OpenTournamentLeaderItem = new PPTournamentLeaderItem();
                            _ppPromoStatus.OpenTournamentLeaderItem.playerID = PPPromoSnapshot.obfusticateUserID(_strUserID);
                            _ppPromoStatus.OpenTournamentLeaderItem.countryID = "MY";
                            _ppPromoStatus.OpenTournamentLeaderItem.memberCurrency = "MYR";
                            _ppPromoStatus.OpenTournamentLeaderItem.scoreBet = 0.0;
                            _ppPromoStatus.OpenTournamentLeaderItem.effectiveBetForBetMultiplier = 0.0;
                            _ppPromoStatus.OpenTournamentLeaderItem.effectiveBetForFreeRounds = 0.0;
                            _ppPromoStatus.OpenTournamentLeaderItem.position = 0;
                        }
                        else
                        {
                            _ppPromoStatus.OpenTournamentLeaderItem = Newtonsoft.Json.JsonConvert.DeserializeObject<PPTournamentLeaderItem>((string)tournamentInfoValue);
                        }
                    }
                    catch (Exception ex)
                    {
                        _ppPromoStatus.OpenTournamentLeaderItem = null;
                        _logger.Error("Exception has been occurred in UserActor::onPromoUpdateEvent {0}", ex);
                    }
                }

                _ppPromoStatus.OpenTournamentID = updateEvent.OpenTournament;
                _ppPromoStatus.OpenTournamentMinBet = updateEvent.OpenTournamentMinBet / 100.0;

                PPTournamentLeaderboards tourLeaderboards = new PPTournamentLeaderboards();
                tourLeaderboards.error = 0;
                tourLeaderboards.description = "OK";
                tourLeaderboards.leaderboards = new List<PPTournamentLeaderboard>();
                for (int i = 0; i < updateEvent.TournamentIDs.Count; i++)
                {
                    PPTournamentLeaderboard leaderBoard = await buildPPTournamentLeaderboard(updateEvent.TournamentIDs[i]);
                    if (leaderBoard != null)
                        tourLeaderboards.leaderboards.Add(leaderBoard);

                }
                _ppPromoStatus.ActiveTournamentLeaderboards = JsonConvert.SerializeObject(tourLeaderboards);

                PPTournamentLeaderboardsV3 tourLeaderboardsV3 = new PPTournamentLeaderboardsV3();
                tourLeaderboardsV3.error = 0;
                tourLeaderboardsV3.description = "OK";
                tourLeaderboardsV3.leaderboards = new List<PPTournamentLeaderboardV3>();
                for (int i = 0; i < updateEvent.TournamentIDs.Count; i++)
                {
                    PPTournamentLeaderboardV3 leaderBoard = await buildPPTournamentLeaderboardV3(updateEvent.TournamentIDs[i]);
                    if (leaderBoard != null)
                        tourLeaderboardsV3.leaderboards.Add(leaderBoard);
                }
                _ppPromoStatus.ActiveTournamentLeaderboardsV3 = JsonConvert.SerializeObject(tourLeaderboardsV3);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::onPromoUpdateEvent {0}", ex);
            }
        }
        private async Task<PPTournamentLeaderboard> buildPPTournamentLeaderboard(int tournamentID)
        {
            PPTournamentLeaderboard leaderBoard = new PPTournamentLeaderboard();
            leaderBoard.tournamentID = tournamentID;
            leaderBoard.index = -1;
            leaderBoard.items = new List<PPTournamentLeaderItem>();
            try
            {
                List<PPTournamentLeaderItem> items = new List<PPTournamentLeaderItem>();
                int index = PPPromoSnapshot.Instance.TournamentList.FindIndex(_ => _.id == tournamentID);

                if (index != -1)
                    items = PPPromoSnapshot.Instance.TournamentList[index].leaderBoard;

                for (int i = 0; i < items.Count; i++)
                {
                    PPTournamentLeaderItem newItem = JsonConvert.DeserializeObject<PPTournamentLeaderItem>(JsonConvert.SerializeObject(items[i]));
                    leaderBoard.items.Add(newItem);

                    if (newItem.playerID == _strUserID)
                        leaderBoard.index = i;
                }
                return leaderBoard;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::buildPPTournamentLeaderboard {0}", ex);
                return null;
            }
        }
        private async Task<PPTournamentLeaderboardV3> buildPPTournamentLeaderboardV3(int tournamentID)
        {
            PPTournamentLeaderboardV3 leaderBoard = new PPTournamentLeaderboardV3();
            leaderBoard.tournamentID = tournamentID;
            leaderBoard.index = -1;
            try
            {
                leaderBoard.items = new List<string>();

                List<PPTournamentLeaderItem> items = new List<PPTournamentLeaderItem>();
                int index = PPPromoSnapshot.Instance.TournamentList.FindIndex(_ => _.id == tournamentID);

                if (index != -1)
                    items = PPPromoSnapshot.Instance.TournamentList[index].leaderBoard;

                for (int i = 0; i < items.Count; i++)
                {
                    PPTournamentLeaderItem item = items[i];
                    string strItem = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}",
                        item.position, item.playerID, item.score, item.scoreBet, item.effectiveBetForFreeRounds, item.effectiveBetForBetMultiplier, item.memberCurrency, item.countryID);

                    leaderBoard.items.Add(strItem);

                    if (item.playerID == _strUserID)
                    {
                        leaderBoard.index = i;
                    }
                }
                return leaderBoard;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::buildPPTournamentLeaderboard {0}", ex);
                return null;
            }
        }
        private string convertRaceWinnerInfoToV2(string strWinnerInfo)
        {
            PPRaceWinnerInfo raceWinnerInfo = JsonConvert.DeserializeObject<PPRaceWinnerInfo>(strWinnerInfo);

            PPRaceWinnerInfoV2 raceWinnerInfoV2 = new PPRaceWinnerInfoV2();
            raceWinnerInfoV2.raceID = raceWinnerInfo.raceID;
            raceWinnerInfoV2.lastIdentity = raceWinnerInfo.lastIdentity;
            raceWinnerInfoV2.action = raceWinnerInfo.action;
            raceWinnerInfoV2.items = new List<string>();
            foreach (PPRaceWinner item in raceWinnerInfo.items)
            {
                int isMyWin = item.playerID == _strUserID ? 1 : 0;
                item.playerID = PPPromoSnapshot.obfusticateUserID(item.playerID);

                raceWinnerInfoV2.items.Add(string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}",
                    item.prizeID, item.playerID, item.bet, item.effectiveBetForBetMultiplier,
                    item.effectiveBetForFreeRounds, item.memberCurrency, item.countryID, isMyWin));
            }
            return JsonConvert.SerializeObject(raceWinnerInfoV2);
        }
        private GITMessage buildPPRaceWinnersV2(GITMessage requestMessage)
        {
            try
            {
                int raceCount = (int)requestMessage.Pop();
                List<string> winnersInfoArray = new List<string>();
                for (int i = 0; i < raceCount; i++)
                {
                    int raceID = (int)requestMessage.Pop();
                    string strGUID = (string)requestMessage.Pop();
                    string strWinnersInfo = null;
                    if (string.IsNullOrEmpty(strGUID))
                    {
                        if (_ppPromoStatus.ActiveRaceWinners.ContainsKey(raceID))
                            strWinnersInfo = _ppPromoStatus.ActiveRaceWinners[raceID];
                    }
                    else
                    {
                        if (_ppPromoStatus.DicRaceWinnersHistory.ContainsKey(raceID))
                        {
                            if (_ppPromoStatus.DicRaceWinnersHistory[raceID].ContainsKey(strGUID))
                                strWinnersInfo = _ppPromoStatus.DicRaceWinnersHistory[raceID][strGUID];
                            else
                                strWinnersInfo = _ppPromoStatus.ActiveRaceWinners[raceID];
                        }
                    }
                    if (strWinnersInfo != null)
                        winnersInfoArray.Add(convertRaceWinnerInfoToV2(strWinnersInfo));
                }

                string strData = string.Format("{{\"error\":0,\"description\":\"OK\",\"winners\":[{0}]}}", string.Join(",", winnersInfoArray.ToArray()));
                GITMessage respondMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_PROMORACEWINNER);
                respondMessage.Append(strData);
                return respondMessage;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::buildPPRaceWinners {0}", ex);
                return null;
            }
        }
        private GITMessage buildPPRaceWinners(GITMessage requestMessage)
        {
            try
            {
                int raceCount = (int)requestMessage.Pop();
                List<string> winnersInfoArray = new List<string>();
                for (int i = 0; i < raceCount; i++)
                {
                    int raceID = (int)requestMessage.Pop();
                    string strGUID = (string)requestMessage.Pop();
                    string strWinnersInfo = null;
                    if (string.IsNullOrEmpty(strGUID))
                    {
                        if (_ppPromoStatus.ActiveRaceWinners.ContainsKey(raceID))
                            strWinnersInfo = _ppPromoStatus.ActiveRaceWinners[raceID];
                    }
                    else
                    {
                        if (_ppPromoStatus.DicRaceWinnersHistory.ContainsKey(raceID))
                        {
                            if (_ppPromoStatus.DicRaceWinnersHistory[raceID].ContainsKey(strGUID))
                                strWinnersInfo = _ppPromoStatus.DicRaceWinnersHistory[raceID][strGUID];
                            else
                                strWinnersInfo = _ppPromoStatus.ActiveRaceWinners[raceID];
                        }
                    }

                    if (strWinnersInfo != null)
                    {
                        PPRaceWinnerInfo raceWinnerInfo = JsonConvert.DeserializeObject<PPRaceWinnerInfo>(strWinnersInfo);
                        foreach (PPRaceWinner item in raceWinnerInfo.items)
                        {
                            int isMyWin = item.playerID == _strUserID ? 1 : 0;
                            item.playerID = PPPromoSnapshot.obfusticateUserID(item.playerID);
                        }
                        strWinnersInfo = JsonConvert.SerializeObject(raceWinnerInfo);
                        winnersInfoArray.Add(strWinnersInfo);
                    }
                }

                string strData = string.Format("{{\"error\":0,\"description\":\"OK\",\"winners\":[{0}]}}", string.Join(",", winnersInfoArray.ToArray()));
                GITMessage respondMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_PROMORACEWINNER);
                respondMessage.Append(strData);
                return respondMessage;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::buildPPRaceWinners {0}", ex);
                return null;
            }
        }
        private async Task<string> buildActivePromos(UserConnection userConn)
        {
            try
            {
                if (_ppPromoStatus.ActivePromos == null)
                    return "{\"error\":0,\"description\":\"OK\",\"serverTime\":1676462782,\"tournaments\":[],\"races\":[]}";

                //PPActivePromos activePromos = JsonConvert.DeserializeObject<PPActivePromos>(JsonConvert.SerializeObject(_ppPromoStatus.ActivePromos));
                PPActivePromos activePromos = new PPActivePromos();
                activePromos.error = _ppPromoStatus.ActivePromos.error;
                activePromos.description = _ppPromoStatus.ActivePromos.description;
                activePromos.serverTime = _ppPromoStatus.ActivePromos.serverTime;
                activePromos.races = new List<PPActiveRace>();
                activePromos.tournaments = new List<PPActiveTournament>();
                foreach (PPActiveRace race in _ppPromoStatus.ActivePromos.races)
                {
                    PPActiveRace newRace = new PPActiveRace()
                    {
                        id = race.id,
                        name = race.name,
                        optin = race.optin,
                        showWinnersList = race.showWinnersList,
                        startDate = race.startDate,
                        endDate = race.endDate,
                        clientMode = race.clientMode,
                        clientStyle = race.clientStyle,
                        status = race.status,

                        agentid = race.agentid,
                        prizeslimit = race.prizeslimit,
                        games = race.games,
                        playersinc = race.playersinc,
                        playersexc = race.playersexc,
                    };
                    activePromos.races.Add(newRace);
                }

                foreach (PPActiveTournament tournament in _ppPromoStatus.ActivePromos.tournaments)
                {
                    PPActiveTournament newTournament = new PPActiveTournament();

                    newTournament.id = tournament.id;
                    newTournament.name = tournament.name;
                    newTournament.optJurisdiction = new List<string>();
                    foreach (string jurisdiction in tournament.optJurisdiction)
                        newTournament.optJurisdiction.Add(jurisdiction);
                    newTournament.optin = tournament.optin;
                    newTournament.startDate = tournament.startDate;
                    newTournament.endDate = tournament.endDate;
                    newTournament.status = tournament.status;
                    newTournament.clientMode = tournament.clientMode;

                    newTournament.agentid = tournament.agentid;
                    newTournament.prizeslimit = tournament.prizeslimit;
                    newTournament.games = tournament.games;
                    newTournament.playersinc = tournament.playersinc;
                    newTournament.playersexc = tournament.playersexc;

                    activePromos.tournaments.Add(newTournament);
                }

                int index = 0;
                while (activePromos.races.Count > index)
                {
                    PPActiveRace race = activePromos.races[index];
                    if (!string.IsNullOrEmpty(race.agentid) && race.agentid != _agentName)
                    {
                        activePromos.races.RemoveAt(index);
                        continue;
                    }
                    if (!string.IsNullOrEmpty(race.games))
                    {
                        try
                        {
                            string[] gameids = race.games.Split(',');
                            string curGameID = userConn.GameID.ToString();
                            if (!gameids.Contains(curGameID))
                            {
                                activePromos.races.RemoveAt(index);
                                continue;
                            }
                        }
                        catch (Exception e)
                        {
                        }
                    }
                    if (!string.IsNullOrEmpty(race.playersinc))
                    {
                        try
                        {
                            string[] playerIncs = race.playersinc.Split(',');

                            if (!playerIncs.Contains(_strUserID))
                            {
                                activePromos.races.RemoveAt(index);
                                continue;
                            }
                        }
                        catch (Exception e)
                        {
                        }
                    }
                    else if (!string.IsNullOrEmpty(race.playersexc))
                    {
                        try
                        {
                            string[] playerExcs = race.playersexc.Split(',');

                            if (playerExcs.Contains(_strUserID))
                            {
                                activePromos.races.RemoveAt(index);
                                continue;
                            }
                        }
                        catch (Exception e)
                        {
                        }
                    }

                    string strKey = string.Format("race_{0}_optin", race.id);
                    bool isOptIn = await RedisDatabase.RedisCache.HashExistsAsync(strKey, _strUserID);
                    if (isOptIn)
                        race.optin = true;
                    else
                        race.optin = false;
                    index++;
                }

                index = 0;
                while (activePromos.tournaments.Count > index)
                {
                    PPActiveTournament tournament = activePromos.tournaments[index];
                    if (!string.IsNullOrEmpty(tournament.agentid) && tournament.agentid != _agentName)
                    {
                        activePromos.tournaments.RemoveAt(index);
                        continue;
                    }
                    if (!string.IsNullOrEmpty(tournament.games))
                    {
                        try
                        {
                            string[] gameids = tournament.games.Split(',');
                            string curGameID = userConn.GameID.ToString();
                            if (!gameids.Contains(curGameID))
                            {
                                activePromos.tournaments.RemoveAt(index);
                                continue;
                            }
                        }
                        catch (Exception e)
                        {
                        }
                    }
                    if (!string.IsNullOrEmpty(tournament.playersinc))
                    {
                        try
                        {
                            string[] playerIncs = tournament.playersinc.Split(',');

                            if (!playerIncs.Contains(_strUserID))
                            {
                                activePromos.tournaments.RemoveAt(index);
                                continue;
                            }
                        }
                        catch (Exception e)
                        {
                        }
                    }
                    else if (!string.IsNullOrEmpty(tournament.playersexc))
                    {
                        try
                        {
                            string[] playerExcs = tournament.playersexc.Split(',');

                            if (playerExcs.Contains(_strUserID))
                            {
                                activePromos.tournaments.RemoveAt(index);
                                continue;
                            }
                        }
                        catch (Exception e)
                        {
                        }
                    }

                    string strKey = string.Format("tournament_{0}_optin", tournament.id);
                    bool isOptIn = await RedisDatabase.RedisCache.HashExistsAsync(strKey, _strUserID);
                    if (isOptIn)
                        tournament.optin = true;
                    else
                        tournament.optin = false;
                    index++;
                }

                return JsonConvert.SerializeObject(activePromos);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::buildActivePromos {0}", ex);
                return "{\"error\":0,\"description\":\"OK\",\"serverTime\":1676462782,\"tournaments\":[],\"races\":[]}";
            }
        }
        private async Task procOptIn(bool isTournament, int promoID)
        {
            try
            {
                string strHashKey = isTournament ? string.Format("tournament_{0}_optin", promoID) :
                    string.Format("race_{0}_optin", promoID);
                await RedisDatabase.RedisCache.HashSetAsync(strHashKey, _strUserID, 0);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::procOptIn {0}", ex);
            }
        }
        private async Task addUserBalanceForTournament()
        {
            try
            {
                foreach (PPTournament item in PPPromoSnapshot.Instance.TournamentList)
                {
                    if (item.status != "CO")
                        continue;

                    int index = item.leaderBoard.FindIndex(_ => _.playerID == _strUserID);
                    string strKey = string.Format("tournament_{0}_payed", item.id);

                    if (index == -1 || await RedisDatabase.RedisCache.HashExistsAsync(strKey, _strUserID))
                        continue;

                    if (item.leaderBoard[index].win != 0)
                        continue;

                    double winAmount = getTourWinAmount(item);
                    if (winAmount == 0.0)
                        continue;

                    item.leaderBoard[index].win = winAmount;

                    _balance += winAmount;
                    bool winUpdate = await _dbReader.Ask<bool>(new PPTourLeaderUpdateItem(item.id, _strUserID, (decimal)winAmount));
                    _dbWriter.Tell(new PlayerBalanceUpdateItem(_userDBID, winAmount));
                    _dbWriter.Tell(new GameLogItem(_agentDBID, _strUserID, 0, "Tournament" + item.id.ToString(), "Position" + item.leaderBoard[index].position,
                        0.0, Math.Round(Math.Max(winAmount, 0), 2), Math.Round(_balance - winAmount, 2), Math.Round(_balance, 2), "", DateTime.UtcNow));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::addUserBalanceForTournament {0}", ex);
            }
        }
        private async Task<string> buildTournamentAnnouncements()
        {
            try
            {
                TournamentAnnouncement announcement = new TournamentAnnouncement();

                foreach (PPTournament item in PPPromoSnapshot.Instance.TournamentList)
                {
                    if (item.status != "CO")
                        continue;

                    int index = item.leaderBoard.FindIndex(_ => _.playerID == _strUserID);
                    string strKey = string.Format("tournament_{0}_payed", item.id);

                    if (index == -1 || await RedisDatabase.RedisCache.HashExistsAsync(strKey, _strUserID))
                        continue;

                    await procTourPayedOn(item.id);

                    double winAmount = getTourWinAmount(item);
                    if (winAmount == 0.0)
                        continue;

                    string endDate = getDateTimeFromUnix(item.endDate).ToString("U", CultureInfo.CreateSpecificCulture("en-US"));
                    string strMsg = string.Format("Congratulations! <br/>You have won [{0}] in TOUR that ended on {1}", winAmount, endDate);
                    announcement.announcements.Add(new AnnouncementDetail("WTC", strMsg));
                }

                return JsonConvert.SerializeObject(announcement);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::buildTournamentAnnouncements {0}", ex);
                TournamentAnnouncement announcement = new TournamentAnnouncement();
                return JsonConvert.SerializeObject(announcement);
            }
        }
        private DateTime getDateTimeFromUnix(int unixTimeStamp)
        {

            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
        private async Task procTourPayedOn(int promoID)
        {
            try
            {
                string strHashKey = string.Format("tournament_{0}_payed", promoID);
                await RedisDatabase.RedisCache.HashSetAsync(strHashKey, _strUserID, 0);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::procTourPayedOn {0}", ex);
            }
        }
        private double getTourWinAmount(PPTournament item)
        {
            try
            {
                int index = item.leaderBoard.FindIndex(_ => _.playerID == _strUserID);
                if (index == -1)
                    return 0.0;

                int myPosition = item.leaderBoard[index].position;
                List<PPTournamentPrize> prizeList = item.prizePool.prizesList;
                PPTournamentPrize selectedPrize = null;
                foreach (PPTournamentPrize prize in prizeList)
                {
                    if (myPosition < prize.placeFrom || myPosition > prize.placeTo)
                        continue;

                    selectedPrize = prize;
                }

                if (selectedPrize == null)
                    return 0.0;

                int detailIndex = _ppPromoStatus.ActiveTournamentDetails.details.FindIndex(_ => _.id == item.id);
                if (detailIndex == -1)
                    return 0.0;

                if (selectedPrize.type == "BM")
                    return item.leaderBoard[index].scoreBet * selectedPrize.betMultiplier;

                return getTourAltPrizeAmount(_ppPromoStatus.ActiveTournamentDetails.details[detailIndex], selectedPrize);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::getTourWinAmount {0}", ex);
                return 0.0;
            }
        }
        private async Task<GITMessage> onPromoActive(GITMessage message, UserConnection userConn)
        {
            GITMessage respondMessage = new GITMessage((ushort)CSMSG_CODE.CS_PP_PROMOACTIVE);
            try
            {
                string strGameSymbol = (string)message.Pop();

                string str = await buildActivePromos(userConn);
                respondMessage.Append(str);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in UserActor::onPromoActive {0}", ex.ToString());
            }

            return respondMessage;
        }
        private double getRaceAltMinBetLimit(PPRaceDetail raceDetail)
        {
            try
            {
                Dictionary<string, double> currencyRateMap = raceDetail.currencyRateMap;

                string myCurrency = Convert.ToString(_currency);
                string raceCurrency = Convert.ToString(raceDetail.prizePool.currency);
                if (currencyRateMap != null && currencyRateMap.Keys.Contains(Convert.ToString(myCurrency)) && currencyRateMap.Keys.Contains(Convert.ToString(raceCurrency)))
                    return raceDetail.prizePool.minBetLimit * currencyRateMap[raceCurrency] / currencyRateMap[myCurrency];
            }
            catch (Exception ex)
            {
            }
            return 1.0;
        }
        private double getRaceAltPrizeAmount(PPRaceDetail raceDetail, PPRacePrize prize)
        {
            try
            {
                Dictionary<string, double> currencyRateMap = raceDetail.currencyRateMap;

                string myCurrency = Convert.ToString(_currency);
                string raceCurrency = Convert.ToString(raceDetail.prizePool.currency);
                if (currencyRateMap != null && currencyRateMap.Keys.Contains(Convert.ToString(myCurrency)) && currencyRateMap.Keys.Contains(Convert.ToString(raceCurrency)))
                    return prize.amount * currencyRateMap[raceCurrency] / currencyRateMap[myCurrency];
            }
            catch (Exception ex)
            {
            }
            return prize.amount;
        }
        private double getTourAltMinBetLimit(PPTournamentDetail tourDetail)
        {
            try
            {
                Dictionary<string, double> currencyRateMap = tourDetail.currencyRateMap;

                string myCurrency = Convert.ToString(_currency);
                string tourCurrency = Convert.ToString(tourDetail.prizePool.currency);
                if (currencyRateMap != null && currencyRateMap.Keys.Contains(Convert.ToString(myCurrency)) && currencyRateMap.Keys.Contains(Convert.ToString(tourCurrency)))
                    return tourDetail.prizePool.minBetLimit * currencyRateMap[tourCurrency] / currencyRateMap[myCurrency];
            }
            catch (Exception ex)
            {
            }
            return tourDetail.prizePool.minBetLimit;
        }
        private double getTourAltMaxBetLimit(PPTournamentDetail tourDetail)
        {
            try
            {
                Dictionary<string, double> currencyRateMap = tourDetail.currencyRateMap;

                string myCurrency = Convert.ToString(_currency);
                string tourCurrency = Convert.ToString(tourDetail.prizePool.currency);
                if (currencyRateMap != null && currencyRateMap.Keys.Contains(Convert.ToString(myCurrency)) && currencyRateMap.Keys.Contains(Convert.ToString(tourCurrency)))
                    return tourDetail.prizePool.maxBetLimitByMultiplier * currencyRateMap[tourCurrency] / currencyRateMap[myCurrency];
            }
            catch (Exception ex)
            {
            }
            return tourDetail.prizePool.maxBetLimitByMultiplier;
        }
        private double getTourAltPrizeAmount(PPTournamentDetail tourDetail, PPTournamentPrize prize)
        {
            try
            {
                Dictionary<string, double> currencyRateMap = tourDetail.currencyRateMap;

                string myCurrency = Convert.ToString(_currency);
                string raceCurrency = Convert.ToString(tourDetail.prizePool.currency);
                if (currencyRateMap != null && currencyRateMap.Keys.Contains(Convert.ToString(myCurrency)) && currencyRateMap.Keys.Contains(Convert.ToString(raceCurrency)))
                    return prize.amount * currencyRateMap[raceCurrency] / currencyRateMap[myCurrency];
            }
            catch (Exception ex)
            {
            }
            return prize.amount;
        }
        #endregion

        private async Task procPPPromoMsg(GITMessage message, UserConnection userConn)
        {
            GITMessage respondMessage = null;
            switch ((CSMSG_CODE)message.MsgCode)
            {
                case CSMSG_CODE.CS_PP_PROMOSTART:
                    {
                        respondMessage = await onPromoActive(message, userConn);
                    }
                    break;
                case CSMSG_CODE.CS_PP_PROMOTOURDETAIL:
                    {
                        respondMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_PROMOTOURDETAIL);

                        PPTournamentDetails ppTournamentDetails = _ppPromoStatus.ActiveTournamentDetails;
                        foreach (PPTournamentDetail tournamentDetail in ppTournamentDetails.details)
                        {
                            tournamentDetail.prizePool.minBetLimit = Math.Floor(getTourAltMinBetLimit(tournamentDetail) * 100);
                            tournamentDetail.prizePool.maxBetLimitByMultiplier = Math.Floor(getTourAltMaxBetLimit(tournamentDetail) * 100);
                        }

                        respondMessage.Append(JsonConvert.SerializeObject(ppTournamentDetails));
                    }
                    break;
                case CSMSG_CODE.CS_PP_PROMORACEDETAIL:
                    {
                        respondMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_PROMORACEDETAIL);

                        PPRaceDetails ppRaceDetail = _ppPromoStatus.ActiveRaceDetails;
                        foreach (PPRaceDetail raceDetail in ppRaceDetail.details)
                        {
                            raceDetail.prizePool.minBetLimit = Math.Floor(getRaceAltMinBetLimit(raceDetail) * 100);
                        }

                        respondMessage.Append(JsonConvert.SerializeObject(ppRaceDetail));
                    }
                    break;
                case CSMSG_CODE.CS_PP_PROMORACEPRIZES:
                    {
                        respondMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_PROMORACEPRIZES);
                        respondMessage.Append(JsonConvert.SerializeObject(_ppPromoStatus.ActiveRacePrizes));
                    }
                    break;
                case CSMSG_CODE.CS_PP_PROMOTOURLEADER:
                    {
                        respondMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_PROMORACEPRIZES);
                        respondMessage.Append(_ppPromoStatus.ActiveTournamentLeaderboards);
                    }
                    break;
                case CSMSG_CODE.CS_PP_PROMOV3TOURLEADER:
                    {
                        respondMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_PROMORACEPRIZES);
                        respondMessage.Append(_ppPromoStatus.ActiveTournamentLeaderboardsV3);
                    }
                    break;
                case CSMSG_CODE.CS_PP_PROMORACEWINNER:
                    {
                        respondMessage = buildPPRaceWinners(message);
                    }
                    break;
                case CSMSG_CODE.CS_PP_PROMOV2RACEWINNER:
                    {
                        respondMessage = buildPPRaceWinnersV2(message);
                    }
                    break;
                case CSMSG_CODE.CS_PP_PROMOTOUROPTIN:
                case CSMSG_CODE.CS_PP_PROMORACEOPTIN:
                    {
                        int promoID = (int)message.Pop();
                        if ((CSMSG_CODE)message.MsgCode == CSMSG_CODE.CS_PP_PROMOTOUROPTIN)
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
                    }
                    break;
                case CSMSG_CODE.CS_PP_PROMOANNOUNCEMENT:
                    {
                        respondMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_PROMOTOUROPTIN);
                        respondMessage.Append(await buildTournamentAnnouncements());
                    }
                    break;
            }
            if (respondMessage != null)
                Sender.Tell(new SendMessageToUser(respondMessage, _balance));
        }
        private async Task replaceSlotGameNode(string strSlotNodePath, UserConnection userConn)
        {
            int remainServerCount = 0;
            try
            {
                var routees = await Context.System.ActorSelection(Constants.SlotGameRouterPath).Ask<Routees>(new GetRoutees());
                foreach (Routee routee in routees.Members)
                    remainServerCount++;

                _logger.Info("{0} Exiting from slot game node {1}", _strUserID, strSlotNodePath);
                await userConn.GameActor.Ask<ExitGameResponse>(new ExitGameRequest(_strUserID, _agentDBID, Math.Round(_balance, 2), false, remainServerCount > 0), TimeSpan.FromSeconds(5));
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in UserActor::replaceSlotGameNode {0}", ex);
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
                    userConn.GameActor = null;
                    userConn.GameType = GAMETYPE.NONE;
                    userConn.GameID = 0;
                    return;
                }
                EnterGameRequest requestMsg = new EnterGameRequest(userConn.GameID, _strUserID, Self, false);
                EnterGameResponse responseMsg = await Context.System.ActorSelection(Constants.SlotGameRouterPath).Ask<EnterGameResponse>(requestMsg, Constants.RemoteTimeOut);
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
                object connection = _userConnections.Keys.ElementAt(i);
                UserConnection userConn = _userConnections[connection];

                if (userConn.GameActor == null || userConn.GameType == GAMETYPE.NONE)
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

        public class UserConnection
        {
            public object Connection { get; set; }
            public IActorRef GameActor { get; set; }
            public int GameID { get; set; }
            public GAMETYPE GameType { get; set; }
            public DateTime LastActiveTime { get; set; }
            public bool IsHttpSession => Connection is string;
            public UserConnection(object connection)
            {
                this.Connection = connection;
                this.GameActor = null;
                this.GameID = 0;
                this.GameType = GAMETYPE.NONE;
                this.LastActiveTime = DateTime.Now;
            }

            public void resetGame()
            {
                this.GameActor = null;
                this.GameID = 0;
                this.GameType = GAMETYPE.NONE;
            }
        }
        public class PPPromoStatus
        {
            public PPActivePromos ActivePromos { get; set; }
            public PPTournamentDetails ActiveTournamentDetails { get; set; }
            public PPRaceDetails ActiveRaceDetails { get; set; }
            public Dictionary<int, string> ActiveRaceWinners { get; set; }
            public Dictionary<int, Dictionary<string, string>> DicRaceWinnersHistory { get; set; }
            public Dictionary<string, double> CurrencyMap { get; set; }
            public PPRacePrizes ActiveRacePrizes { get; set; }
            public int OpenTournamentID { get; set; }
            public double OpenTournamentMinBet { get; set; }
            public string ActiveTournamentLeaderboards { get; set; }
            public string ActiveTournamentLeaderboardsV3 { get; set; }
            public PPTournamentLeaderItem OpenTournamentLeaderItem { get; set; }
            public List<PPFreespin> ActiveFreeSpins { get; set; }

            public List<PPFreespin> ReceivedFreeSpins { get; set; }
        }
        public class PPGameLogType
        {
            public string cr { get; set; }
            public string sr { get; set; }
        }
    }
    public class SeamlessGetBalanceRequest
    {
        public string agentID { get; set; }
        public string sign { get; set; }
        public string userID { get; set; }
        public int gameID { get; set; }
    }
    public class SeamlessGetBalanceResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public decimal balance { get; set; }
    }
    public class CallbackApiRequestWithRetry : IConsistentHashable
    {
        public string ApiURL { get; private set; }
        public string UserID { get; private set; }
        public string AgentID { get; private set; }
        public int GameID { get; private set; }
        public string SecretKey { get; private set; }
        public DateTime LastTriedTime { get; set; }
        public int RetryCount { get; set; }
        public object ConsistentHashKey => string.Format("{0}_{1}", AgentID, UserID);
        public CallbackApiRequestWithRetry(string apiURL, string agentID, string userID, string secretKey, int gameID, int retryCount, DateTime lastTriedTime)
        {
            this.ApiURL = apiURL;
            this.UserID = userID;
            this.AgentID = agentID;
            this.GameID = gameID;
            this.SecretKey = secretKey;
            this.RetryCount = retryCount;
            this.LastTriedTime = lastTriedTime;
        }
    }
    public class RollbackRequestWithRetry : CallbackApiRequestWithRetry
    {
        public double Amount { get; private set; }
        public string TransactionID { get; private set; }
        public RollbackRequestWithRetry(string apiURL, string agentID, string userID, string secretKey, int gameID, int retryCount, DateTime lastTriedTime, string transactionID, double amount) :
            base(apiURL, agentID, userID, secretKey, gameID, retryCount, lastTriedTime)
        {
            this.TransactionID = transactionID;
            this.Amount = amount;
        }
    }
    public class DepositRequestWithRetry : CallbackApiRequestWithRetry
    {
        public string TransactionID { get; private set; }
        public string RefTransactionID { get; private set; }
        public string RoundID { get; private set; }
        public double Amount { get; private set; }

        public DepositRequestWithRetry(string apiURL, string agentID, string userID, string secretKey, int gameID, int retryCount, DateTime lastTriedTime, string transactionID, string refTransactionID, string roundID, double amount) :
            base(apiURL, agentID, userID, secretKey, gameID, retryCount, lastTriedTime)
        {
            this.TransactionID = transactionID;
            this.RefTransactionID = refTransactionID;
            this.RoundID = roundID;
            this.Amount = amount;
        }
    }
    public class SeamlessWithdrawRequest
    {
        public string agentID { get; set; }
        public string sign { get; set; }
        public string userID { get; set; }
        public int gameID { get; set; }
        public decimal amount { get; set; }
        public string transactionID { get; set; }
        public string roundID { get; set; }
    }
    public class SeamlessWithdrawResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public string platformTransactionID { get; set; }
        public decimal balance { get; set; }
    }
    public class SeamlessDepositRequest
    {
        public string agentID { get; set; }
        public string sign { get; set; }
        public string userID { get; set; }
        public int gameID { get; set; }
        public decimal amount { get; set; }
        public string transactionID { get; set; }
        public string refTransactionID { get; set; }
        public string roundID { get; set; }
    }
    public class SeamlessDepositResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public string platformTransactionID { get; set; }
        public decimal balance { get; set; }
    }
    public class SeamlessRollbackRequest
    {
        public string agentID { get; set; }
        public string sign { get; set; }
        public string userID { get; set; }
        public string refTransactionID { get; set; }
        public int gameID { get; set; }
    }
    public class SeamlessRollbackResponse
    {
        public int code { get; set; }
        public string message { get; set; }
    }
    public class IPApiResponse
    {
        public string countryCode { get; set; }
    }
    public class TournamentAnnouncement
    {
        public int error { get; set; }
        public string description { get; set; }
        public List<AnnouncementDetail> announcements { get; set; }
        public TournamentAnnouncement()
        {
            error = 0;
            description = "OK";
            announcements = new List<AnnouncementDetail>();
        }
    }
    public class AnnouncementDetail
    {
        public string typeId { get; set; }
        public string message { get; set; }
        public AnnouncementDetail(string typeid, string msg)
        {
            typeId = typeid;
            message = msg;
        }
    }

    public class UserReportItem
    {
        public string UserID { get; private set; }
        public double DailyBet { get; private set; }
        public double DailyWin { get; private set; }
        public int DailyRound { get; private set; }
        public double WeeklyBet { get; private set; }
        public double WeeklyWin { get; private set; }
        public int WeeklyRound { get; private set; }
        public double MonthlyBet { get; private set; }
        public double MonthlyWin { get; private set; }
        public int MonthlyRound { get; private set; }
        public string DailyKey { get; private set; }
        public string WeeklyKey { get; private set; }
        public string MonthlyKey { get; private set; }

        public UserReportItem(string strUserID)
        {
            this.UserID = strUserID;
            DateTime date = DateTime.Now;
            this.DailyKey = date.ToString("yyyyMMdd");

            Calendar calendar = CultureInfo.InvariantCulture.Calendar;
            CalendarWeekRule weekRule = CalendarWeekRule.FirstFourDayWeek;
            DayOfWeek firstDayOfWeek = DayOfWeek.Monday;
            int weekNumber = calendar.GetWeekOfYear(date, weekRule, firstDayOfWeek);
            this.WeeklyKey = date.ToString("yyyy") + weekNumber;

            MonthlyKey = date.ToString("yyyyMM");
        }
        public UserReportItem(string strUserID, double bet, double win)
        {
            this.UserID = strUserID;
            this.DailyBet = bet;
            this.DailyWin = win;
            this.WeeklyBet = bet;
            this.WeeklyWin = win;
            this.MonthlyBet = bet;
            this.MonthlyWin = win;

            if (bet > 0)
            {
                this.DailyRound = 1;
                this.WeeklyRound = 1;
                this.MonthlyRound = 1;
                DateTime date = DateTime.UtcNow;
                this.DailyKey = date.ToString("yyyyMMdd");

                Calendar calendar = CultureInfo.InvariantCulture.Calendar;
                CalendarWeekRule weekRule = CalendarWeekRule.FirstFourDayWeek;
                DayOfWeek firstDayOfWeek = DayOfWeek.Monday;
                int weekNumber = calendar.GetWeekOfYear(date, weekRule, firstDayOfWeek);
                this.WeeklyKey = date.ToString("yyyy") + weekNumber;

                MonthlyKey = date.ToString("yyyyMM");
            }
        }

        public void mergeReport(ReportUpdateItem other)
        {
            DateTime date = DateTime.UtcNow;
            var dailyKey = date.ToString("yyyyMMdd");

            Calendar calendar = CultureInfo.InvariantCulture.Calendar;
            CalendarWeekRule weekRule = CalendarWeekRule.FirstFourDayWeek;
            DayOfWeek firstDayOfWeek = DayOfWeek.Monday;
            int weekNumber = calendar.GetWeekOfYear(date, weekRule, firstDayOfWeek);
            var weeklyKey = date.ToString("yyyy") + weekNumber;

            var monthlyKey = date.ToString("yyyyMM");
            if (dailyKey.Equals(this.DailyKey))
            {
                this.DailyBet = Math.Round(this.DailyBet + other.Bet, 2);
                this.DailyWin += other.Win;
                if (other.Bet > 0)
                {
                    this.DailyRound += 1;
                }
            }
            else
            {
                this.DailyKey = dailyKey;
                this.DailyBet = other.Bet;
                this.DailyWin = other.Win;
                if (other.Bet > 0)
                {
                    this.DailyRound = 1;
                }
                else
                {
                    this.DailyRound = 0;
                }
            }
            if (weeklyKey.Equals(this.WeeklyKey))
            {
                this.WeeklyBet += other.Bet;
                this.WeeklyWin += other.Win;
                if (other.Bet > 0)
                {
                    this.WeeklyRound += 1;
                }
            }
            else
            {
                this.WeeklyKey = weeklyKey;
                this.WeeklyBet = other.Bet;
                this.WeeklyWin = other.Win;
                if (other.Bet > 0)
                {
                    this.WeeklyRound = 1;
                }
                else
                {
                    this.WeeklyRound = 0;
                }

            }
            if (monthlyKey.Equals(this.MonthlyKey))
            {
                this.MonthlyBet += other.Bet;
                this.MonthlyWin += other.Win;
                if (other.Bet > 0)
                {
                    this.MonthlyRound += 1;
                }
            }
            else
            {
                this.MonthlyKey = monthlyKey;
                this.MonthlyBet = other.Bet;
                this.MonthlyWin = other.Win;
                if (other.Bet > 0)
                {
                    this.MonthlyRound = 1;
                }
                else
                {
                    this.MonthlyRound = 0;
                }

            }


        }

        public void SerializeFrom(BinaryReader reader)
        {
            this.UserID = reader.ReadString();
            this.DailyBet = reader.ReadDouble();
            this.DailyWin = reader.ReadDouble();
            this.DailyRound = reader.ReadInt32();
            this.WeeklyBet = reader.ReadDouble();
            this.WeeklyWin = reader.ReadDouble();
            this.WeeklyRound = reader.ReadInt32();
            this.MonthlyBet = reader.ReadDouble();
            this.MonthlyWin = reader.ReadDouble();
            this.MonthlyRound = reader.ReadInt32();
            this.DailyKey = reader.ReadString();
            this.WeeklyKey = reader.ReadString();
            this.MonthlyKey = reader.ReadString();
        }
        public void SerializeTo(BinaryWriter writer)
        {
            writer.Write(UserID);
            writer.Write(DailyBet);
            writer.Write(DailyWin);
            writer.Write(DailyRound);
            writer.Write(WeeklyBet);
            writer.Write(WeeklyWin);
            writer.Write(WeeklyRound);
            writer.Write(MonthlyBet);
            writer.Write(MonthlyWin);
            writer.Write(MonthlyRound);
            writer.Write(DailyKey);
            writer.Write(WeeklyKey);
            writer.Write(MonthlyKey);
        }
        public byte[] convertToByte()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    this.SerializeTo(bw);
                }
                return ms.ToArray();
            }
        }

    }
}
