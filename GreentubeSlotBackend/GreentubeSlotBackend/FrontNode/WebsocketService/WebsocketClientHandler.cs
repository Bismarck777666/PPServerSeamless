using Akka.Actor;
using Akka.Configuration.Hocon;
using Akka.Event;
using FrontNode.Database;
using GITProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCGSharp;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FrontNode.WebsocketService
{
    public class WebsocketClientHandler : ReceiveActor
    {
        private IActorRef                   _connection                 = null;
        private EndPoint                    _remoteAddress              = null;
        private IActorRef                   _dbReaderProxy              = null;
        private readonly ILoggingAdapter    _log                        = Logging.GetLogger(Context);
        private IActorRef                   _userActor                  = null;
        private string                      _strGlobalUserID            = "";
        private DateTime                    _lastReceivedTime;
        private ICancelable                 _schedulerCancel;
        private int                         _redisCheckRetryCount       = 0;
        private ICancelable                 _redisWaitSchedulerCancel   = null;
        private ConnectionStatus            _connectionStatus           = ConnectionStatus.Connected;
        private string                      _gamesymbol                 = "";

        public WebsocketClientHandler(IActorRef connection, EndPoint remoteAddress, IActorRef dbReader)
        {
            _remoteAddress      = remoteAddress;
            _connection         = connection;
            _dbReaderProxy      = dbReader;
            _lastReceivedTime   = DateTime.Now;

            initializeMessageProcs();
        }
        public static Props Props(IActorRef connection, EndPoint remoteAddress, IActorRef reader)
        {
            return Akka.Actor.Props.Create(() => new WebsocketClientHandler(connection, remoteAddress, reader));
        }
        
        private void initializeMessageProcs()
        {
            Receive<WsClientConnection.StringProtocalReceived>  (received   => onStringReceiveData(received));
            Receive<WsClientConnection.Disconnected>            (closed     => Context.Stop(Self));
            Receive<string>                                     (processCommand);
            ReceiveAsync<UserLoginResponse>                     (procUserLoginResponse);
            ReceiveAsync<CheckUserPathFromRedis>                (checkRegisteredUserPath);
            Receive<SendMessageToUser>                          (message    =>
            {
                sendMessage(message.Message, message.Balance);
            });
        }
        protected override void PreStart()
        {
            _schedulerCancel = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(1000, 1000, Self, Constants.CheckConnectionCommand, Self);
            base.PreStart();
        }
        protected override void PostStop()
        {
            if (_userActor != null)
            {
                _userActor.Tell(new SocketConnectionClosed());
            }

            if (_schedulerCancel != null)
                _schedulerCancel.Cancel();

            if(_redisWaitSchedulerCancel != null)
                _redisWaitSchedulerCancel.Cancel();
    
            base.PostStop();
        }
        private async Task procUserLoginResponse(UserLoginResponse loginResponse)
        {
            if (loginResponse.Result != LoginResult.OK)
            {
                sendLoginResponse(loginResponse);

                _connection.Tell("disconnected");
            }
            else
            {
                try
                {
                    string strGlobalUserID = loginResponse.GlobalUserID;
                    bool isNotOnline = await RedisDatabase.RedisCache.HashSetAsync("onlineusers", strGlobalUserID, true, StackExchange.Redis.When.NotExists);
                    if (isNotOnline)
                    {
                        _userActor          = await Context.System.ActorSelection("/user/userRouter").Ask<IActorRef>(new CreateNewUserMessage(Self, loginResponse.UserDBID, loginResponse.UserID, loginResponse.UserBalance, loginResponse.PassToken, loginResponse.AgentDBID, loginResponse.AgentID, loginResponse.LastScoreCounter, loginResponse.Currency), TimeSpan.FromSeconds(10.0));
                        _strGlobalUserID    = loginResponse.GlobalUserID;
                        _connectionStatus   = ConnectionStatus.Authenticated;
                        procEnterMessage();
                        return;
                    }

                    if (_redisWaitSchedulerCancel != null)
                        _redisWaitSchedulerCancel.Cancel();

                    _redisCheckRetryCount       = 0;
                    _redisWaitSchedulerCancel   = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(0, 500, Self, new CheckUserPathFromRedis(loginResponse), Self);
                }
                catch (Exception ex)
                {
                    _log.Error("Exception has been occurred in WebsocketClientHandler::procUserLoginResponse {0}", ex);
                }
            }
        }
        private async Task checkRegisteredUserPath(CheckUserPathFromRedis request)
        {
            try
            {
                string strGlobalUserID = request.Response.GlobalUserID;
                RedisValue userPath = await RedisDatabase.RedisCache.HashGetAsync("onlineusers", strGlobalUserID + "_path");
                if (!userPath.IsNullOrEmpty)
                {
                    _userActor          = await Context.System.ActorSelection((string)userPath).ResolveOne(TimeSpan.FromSeconds(5));

                    _userActor.Tell(new SocketConnectionAdded());
 
                    _strGlobalUserID          = strGlobalUserID;
                    _connectionStatus   = ConnectionStatus.Authenticated;
                    procEnterMessage();

                    if(_redisWaitSchedulerCancel != null)
                    {
                        _redisWaitSchedulerCancel.Cancel();
                        _redisWaitSchedulerCancel = null;
                    }
                    return;
                }

                _redisCheckRetryCount++;

                if (_redisCheckRetryCount < 40)
                    return;
            }
            catch (Exception ex)
            {
                _log.Error("Exception has been occurred in GreentubeWebsocketClientHandler::checkRegisteredUserPath {0}", ex);
            }

            sendLoginResponse(new UserLoginResponse(LoginResult.UNKNOWNERROR));
            _connection.Tell("disconnected");

            _redisWaitSchedulerCancel.Cancel();
            _redisWaitSchedulerCancel = null;
        }
        private void processCommand(string strCommand)
        {
            if (strCommand == Constants.CheckConnectionCommand)
            {
                if (DateTime.Now.Subtract(_lastReceivedTime) >= Constants.HeartbeatTimeout)
                {
                    if (_userActor == null)
                        _log.Info("Heartbeat timeout  has been detected from  {0}", _remoteAddress);
                    else
                        _log.Info("Heartbeat timeout  has been detected from {0} user", _strGlobalUserID);

                    _connection.Tell("disconnected");
                }
            }
            else if(strCommand == "closeConnection")
            {
                _connection.Tell("disconnected");
            }
        }
        private void sendMessage(GITMessage message, double balance)
        {
            if(message.MsgCode == (ushort)SCMSG_CODE.SC_ENTERGAME)
            {
                byte status             = (byte) message.Pop();
                if(status == 0)
                {
                    procInitMessage();
                }
                else
                {
                    _log.Warning("Can not enter game from {0}", _remoteAddress);
                    _connection.Tell("disconnected");
                }
            }
            else
            {
                int index = message.DataNum;
                for (int i = 0; i < index; i++)
                {
                    string strMsg = (string)message.Pop();
                    sendMessage(strMsg);
                }
            }
        }
        private void sendLoginResponse(UserLoginResponse loginResponse)
        {
            string responseMessage = string.Empty;
            if(loginResponse.Result != LoginResult.OK)
            {
                responseMessage = "-1Invalid item hash value";
                sendMessage(responseMessage);
            }
        }
        private void onStringReceiveData(WsClientConnection.StringProtocalReceived received)
        {
            _lastReceivedTime = DateTime.Now;
            string message = received.ReceivedData;

            if (message == null)
                return;

            if (_connectionStatus == ConnectionStatus.Connected)
            {
                onProcGreentubeMessageBeforeAuth(message);
            }
            else if (_connectionStatus == ConnectionStatus.Authenticating)
            {
                _log.Warning("Unauthorized Message has been received from {0}", _remoteAddress);
                _connection.Tell("disconnected");
            }
            else
                onProcMessage(message);
        }
        private void onProcGreentubeMessageBeforeAuth(string message)
        {
            var sections = message.Split('ÿ');
            foreach(var section in sections)
            {
                if (section.Length > 0 && section[0] == '\a')
                {
                    //229592ÿ597406690ÿCB690FBC-3B3A-48AA-ADF3-7A5232365BA5ÿ1ÿ0
                    //var userID = int.Parse(sections[1]) - 70000;
                    string[] userInfo = sections[1].Split('_');
                    if (userInfo.Length != 2) {
                        _log.Warning("Unauthorized Token has been received from {0}", _remoteAddress);
                        sendMessage("-1Unauthorized");
                        _connection.Tell("disconnected");
                    }
                    int agentID = int.Parse(userInfo[0]);
                    string strUserId = userInfo[1];
                    string strToken = sections[2];

                    string strPassword = "";
                    DateTime expireTime = DateTime.Now;
                    if (!deciperInfo(strToken, ref strPassword, ref expireTime) || expireTime <= DateTime.UtcNow)
                    {
                        sendMessage("-1Unauthorized");
                        _connection.Tell("disconnected");
                        return;
                    }

                    try
                    {
                        //_gamesymbol = "sizzlinghotdeluxe";
                        _gamesymbol = section.Substring(1);

                        _connectionStatus = ConnectionStatus.Authenticating;
                        _dbReaderProxy.Tell(new UserLoginRequest(agentID, strUserId.Trim(), strPassword, (_remoteAddress as IPEndPoint).Address.ToString(), PlatformTypes.WEB));
                        //_dbReaderProxy.Tell(new UserLoginRequest(userID.ToString().Trim()));
                    }
                    catch
                    {
                        _log.Warning("Unauthorized Token has been received from {0}", _remoteAddress);
                        sendMessage("-1Unauthorized");
                        _connection.Tell("disconnected");
                    }
                }
            }
        }
        private void procEnterMessage()
        {
            try
            {
                int gameID = DBMonitorSnapshot.Instance.getGameIDFromString(GameProviders.GREENTUBE, _gamesymbol);
                if (gameID == 0)
                {
                    _log.Warning("Invalid Game enter request from {0}", _remoteAddress);
                    _connection.Tell("disconnected");
                    return;
                }

                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_ENTERGAME);
                message.Append((ushort)gameID);
                _userActor.Tell(new FromConnRevMessage(Self, message));
            }
            catch (Exception ex)
            {
                _log.Error("Exception has been occurred in GreentubeWebsocketClientHandler::procEnterMessage {0}", ex);
            }
        }
        private void procInitMessage()
        {
            try
            {
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_GREENTUBE_DOINIT);
                _userActor.Tell(new FromConnRevMessage(Self, message));
            }
            catch (Exception ex)
            {
                _log.Error("Exception has been occurred in GreentubeWebsocketClientHandler::procInitMessage {0}", ex);
            }
        }
        private void onProcMessage(string strMsg)
        {
            try
            {
                if (strMsg[0] == '' || strMsg[0] == '')
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_GREENTUBE_BALANCECONFIRM);
                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }
                string[] messageParams = strMsg.Split('ÿ');
                if(messageParams.Length == 3 && messageParams[0].Substring(0, 1) == "2")
                {
                    int betPerLine = int.Parse(messageParams[0].Substring(1));
                    int lineIndex = int.Parse(messageParams[2]);
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_GREENTUBE_CHANGELINEBET);
                    message.Append(betPerLine);
                    message.Append(lineIndex);
                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }
                // spin and freespin
                if(strMsg == "30" || strMsg == "60" || strMsg[0] == ':')
                {
                    
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_GREENTUBE_DOSPIN);
                    if (strMsg[0] == ':')
                        message.Append(strMsg.Substring(1));
                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }
                // collect 
                if (strMsg == "40")
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_GREENTUBE_DOCOLLECT);
                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }
                // gamble game
                if (strMsg == "9")
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_GREENTUBE_DOGAMBLEPICK);
                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }
                // gamble result
                if (strMsg == "5b" || strMsg == "5r")
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_GREENTUBE_DOGAMBLEHALF);
                    message.MessageContent = strMsg;
                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }
            }
            catch
            {
                _log.Warning("Unauthorized Token has been received from {0}", _remoteAddress);
                _connection.Tell("disconnected");
            }
        }
        private void sendMessage(string message)
        {
            _connection.Tell(new WsClientConnection.StringProtocalWrite(message));
        }
        private string decryptString(byte[] cipherData, string key)
        {
            byte[] iv = new byte[16];
            byte[] buffer = cipherData;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
        private bool deciperInfo(string strCipherText, ref string strPassword, ref DateTime expireTime)
        {
            try
            {
                string strTokenData = decryptString(HttpServerUtility.UrlTokenDecode(strCipherText), FrontConfig.FrontTokenKey);
                string[] strParts = strTokenData.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                if (strParts.Length != 2)
                    return false;

                strPassword = strParts[0];
                expireTime = DateTime.ParseExact(strParts[1], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public enum ConnectionStatus
    {
        Connected       = 0,
        Authenticating  = 1,
        Authenticated   = 2,
    }
}
