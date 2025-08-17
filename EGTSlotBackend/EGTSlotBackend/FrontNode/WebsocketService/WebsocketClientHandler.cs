using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using System.Net;
using Akka.Event;
using GITProtocol;
using StackExchange.Redis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCGSharp;
using FrontNode.Database;
using System.Security.Cryptography;
using System.IO;
using System.Web;
using Microsoft.SqlServer.Server;

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
        private int                         _gameid                     = 0;
        private string                      _sessionkey                 = "";
        private string                      _messageid                  = "";

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
                _log.Error("Exception has been occurred in EGTWebsocketClientHandler::checkRegisteredUserPath {0}", ex);
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
                onProcEGTMessageBeforeAuth(message);
            }
            else if (_connectionStatus == ConnectionStatus.Authenticating)
            {
                _log.Warning("Unauthorized Message has been received from {0}", _remoteAddress);
                _connection.Tell("disconnected");
            }
            else
                onProcMessage(message);
        }
        private void onProcEGTMessageBeforeAuth(string message)
        {
            var json = JObject.Parse(message);
            string command = json["command"]?.ToString() ?? "";
            
            if (command != Command.LOGIN)
            {
                _log.Warning("Unauthorized Token has been received from {0}", _remoteAddress);
                sendMessage("-1Unauthorized");
                _connection.Tell("disconnected");
                return;
            }

            string sessionKey = json["sessionKey"]?.ToString() ?? "";
            var sections = sessionKey.Split('ÿ');

            string strPassword = "";
            DateTime expireTime = DateTime.Now;
            if (sections.Length != 4 || !deciperInfo(sections[2], ref strPassword, ref expireTime) || expireTime <= DateTime.UtcNow)
            {
                sendMessage("-1Unauthorized");
                _connection.Tell("disconnected");
                return;
            }
            
            var userID = sections[1];
            _gamesymbol = sections[3];
            _sessionkey = sessionKey;
            _messageid = json["messageId"]?.ToString() ?? "";

            try
            {
                _connectionStatus = ConnectionStatus.Authenticating;
                _dbReaderProxy.Tell(new UserLoginRequest(userID.ToString().Trim()));
            }
            catch
            {
                _log.Warning("Unauthorized Token has been received from {0}", _remoteAddress);
                sendMessage("-1Unauthorized");
                _connection.Tell("disconnected");
            }
        }
        private void procEnterMessage()
        {
            try
            {
                _gameid = DBMonitorSnapshot.Instance.getGameIDFromString(GameProviders.EGT, _gamesymbol);
                if(_gameid == 0)
                {
                    _log.Warning("Invalid Game enter request from {0}", _remoteAddress);
                    _connection.Tell("disconnected");
                    return;
                }

                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_ENTERGAME);
                message.Append((ushort)_gameid);
                _userActor.Tell(new FromConnRevMessage(Self, message));
            }
            catch (Exception ex)
            {
                _log.Error("Exception has been occurred in EGTWebsocketClientHandler::procEnterMessage {0}", ex);
            }
        }
        private void procInitMessage()
        {
            try
            {
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_EGT_DOINIT);
                message.Append(_sessionkey);
                message.Append(_messageid);
                _userActor.Tell(new FromConnRevMessage(Self, message));
            }
            catch (Exception ex)
            {
                _log.Error("Exception has been occurred in EGTWebsocketClientHandler::procInitMessage {0}", ex);
            }
        }
        private void onProcMessage(string strMsg)
        {
            try
            {
                JObject json = JObject.Parse(strMsg);
                string command = json["command"]?.ToString() ?? "";
                _messageid = json["messageId"]?.ToString() ?? "";

                if(command == Command.PING)
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_HEARTBEAT);
                    message.Append(_sessionkey);
                    message.Append(_messageid);
                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }

                if(command == Command.SUBSCRIBE)
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_EGT_SUBSCRIBE);
                    message.Append(_sessionkey);
                    message.Append(_messageid);
                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }

                if (command == Command.SETTINGS)
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_EGT_SETTINGS);
                    message.Append(_sessionkey);
                    message.Append(_messageid);
                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }

                if(command == Command.BET)
                {
                    string gameCommand = JObject.Parse(json["bet"]?.ToString() ?? "")["gameCommand"]?.ToString()??"";
                    
                    if (gameCommand == GameCommand.BET)
                    {
                        double betAmount = double.Parse(JObject.Parse(json["bet"]?.ToString() ?? "")["bet"]?.ToString() ?? "");
                        double betMultiplier = double.Parse(JObject.Parse(json["bet"]?.ToString() ?? "")["betMultiplier"]?.ToString() ?? "");
                        int featureId = int.Parse(JObject.Parse(json["bet"]?.ToString() ?? "")["featureId"]?.ToString() ?? "");
                        string gameNumber = json["gameNumber"]?.ToString() ?? "";

                        GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_EGT_DOSPIN);
                        message.Append(_sessionkey);
                        message.Append(_messageid);
                        message.Append(betAmount);
                        message.Append(featureId);
                        message.Append(betMultiplier);
                        _userActor.Tell(new FromConnRevMessage(Self, message));
                    }

                    if(gameCommand == GameCommand.COLLECT)
                    {
                        GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_EGT_DOCOLLECT);
                        message.Append(_sessionkey);
                        message.Append(_messageid);
                        _userActor.Tell(new FromConnRevMessage(Self, message));
                    }

                    if(gameCommand == GameCommand.GAMBLE)
                    {
                        int color = int.Parse(JObject.Parse(json["bet"]?.ToString() ?? "")["color"]?.ToString() ?? "");

                        GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_EGT_DOGAMBLE);
                        message.Append(_sessionkey);
                        message.Append(_messageid);
                        message.Append(color);
                        _userActor.Tell(new FromConnRevMessage(Self, message));
                    }
                }
            }
            catch (Exception ex)
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
