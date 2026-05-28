using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using System.Net;
using Akka.Event;
using GITProtocol;
using CommNode.Database;
using StackExchange.Redis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCGSharp;

namespace CommNode
{
    public class AmaticWebsocketClientHandler : ReceiveActor
    {
        private IActorRef                   _connection                 = null;
        private EndPoint                    _remoteAddress              = null;
        private IActorRef                   _dbReaderProxy              = null;
        private IActorRef                   _dbWriterProxy              = null;
        private IActorRef                   _redisWriter                = null;
        private readonly ILoggingAdapter    _log                        = Logging.GetLogger(Context);
        private IActorRef                   _userActor                  = null;
        private string                      _strUserID                  = "";
        private DateTime                    _lastReceivedTime;
        private ICancelable                 _schedulerCancel;
        private int                         _redisCheckRetryCount       = 0;
        private ICancelable                 _redisWaitSchedulerCancel   = null;
        private ConnectionStatus            _connectionStatus           = ConnectionStatus.Connected;
        
        private AmaticDecrypt               _amaDecrypt                 = null;
        private string                      _gamesymbol                 = "";

        public AmaticWebsocketClientHandler(IActorRef connection, EndPoint remoteAddress, IActorRef dbReader, IActorRef dbWriter, IActorRef redisWriter)
        {
            _remoteAddress      = remoteAddress;
            _connection         = connection;
            _dbReaderProxy      = dbReader;
            _dbWriterProxy      = dbWriter;
            _redisWriter        = redisWriter;
            _lastReceivedTime   = DateTime.Now;

            initializeMessageProcs();
        }

        public static Props Props(IActorRef connection, EndPoint remoteAddress, IActorRef reader, IActorRef writer, IActorRef redisWriter)
        {
            return Akka.Actor.Props.Create(() => new AmaticWebsocketClientHandler(connection, remoteAddress, reader, writer, redisWriter));
        }

        private void initializeMessageProcs()
        {
            Receive<AmaticWsClientConnection.StringProtocalReceived>    (received   => onStringReceiveData(received));
            
            Receive<AmaticWsClientConnection.Disconnected>              (closed     => Context.Stop(Self));
            Receive<string>                                             (command    => processCommand(command));
            ReceiveAsync<UserLoginResponse>                             (procUserLoginResponse);
            ReceiveAsync<CheckUserPathFromRedis>                        (checkRegisteredUserPath);
            Receive<SendMessageToUser>                                  (message    =>
            {
                sendMessage(message.Message, message.Balance);
            });
        }

        protected override void PreStart()
        {
            _schedulerCancel = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(1000, 1000, Self, Constants.CheckConnectionCommand, Self);
            base.PreStart();
        }

        private async Task procUserLoginResponse(UserLoginResponse loginResponse)
        {
            if (loginResponse.Result != LoginResult.OK)
            {
                sendLoginResponse(loginResponse);

                //如果登录失败则断开连接。
                _connection.Tell("disconnected");
            }
            else
            {
                try
                {
                    string strUserID = loginResponse.UserID;
                    bool isNotOnline = await RedisDatabase.RedisCache.HashSetAsync("onlineusers", strUserID, true, StackExchange.Redis.When.NotExists);
                    if (isNotOnline)
                    {
                //如果登录成功则创建用户角色。
                        _userActor = await Context.System.ActorSelection("/user/userManager").Ask<IActorRef>(new CreateNewUserMessage(Self, _dbReaderProxy, _dbWriterProxy, _redisWriter, loginResponse, PlatformTypes.WEB));
                        _strUserID          = loginResponse.UserID;
                        _connectionStatus   = ConnectionStatus.Authenticated;
                        procEnterMessage();
                        return;
                    }

                    //如果已经登录则获取该用户的通行证。
                    //如果通行证未注册，则最多等待20秒。 (40 * 0.5秒)
                    if (_redisWaitSchedulerCancel != null)
                        _redisWaitSchedulerCancel.Cancel();

                    _redisCheckRetryCount       = 0;
                    _redisWaitSchedulerCancel   = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(0, 500, Self, new CheckUserPathFromRedis(loginResponse), Self);
                }
                catch (Exception ex)
                {
                    _log.Error("Exception has been occurred in AmaticWebsocketClientHandler::procUserLoginResponse {0}", ex);
                }
            }
        }

        private async Task checkRegisteredUserPath(CheckUserPathFromRedis request)
        {
            try
            {
                string strUserID = request.Response.UserID;
                RedisValue userPath = await RedisDatabase.RedisCache.HashGetAsync("onlineusers", strUserID + "_path");
                if (!userPath.IsNullOrEmpty)
                {
                    _userActor          = await Context.System.ActorSelection((string)userPath).ResolveOne(TimeSpan.FromSeconds(5));

                    //将该连接注册到用户角色。
                    _userActor.Tell(new SocketConnectionAdded());
 
                    _strUserID          = strUserID;
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

                //如果20秒已过
                if (_redisCheckRetryCount < 40)
                    return;
            }
            catch (Exception ex)
            {
                _log.Error("Exception has been occurred in AmaticWebsocketClientHandler::checkRegisteredUserPath {0}", ex);
            }

            //确认为登录失败。
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
                        _log.Info("Heartbeat timeout  has been detected from {0} user", _strUserID);

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
                string strMsg = (string)message.Pop();
                sendMessage(strMsg);
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

        private void onStringReceiveData(AmaticWsClientConnection.StringProtocalReceived received)
        {
            _lastReceivedTime = DateTime.Now;
            string message = received.ReceivedData;

            if (message == null)
                return;

            if (_connectionStatus == ConnectionStatus.Connected)
            {
                onProcMessageBeforeAuth(message);
            }
            else if (_connectionStatus == ConnectionStatus.Authenticating)
            {
                //在找到用户角色期间无法处理任何消息
                _log.Warning("Unauthorized Message has been received from {0}", _remoteAddress);
                _connection.Tell("disconnected");
            }
            else
                onProcMessage(message);
        }

        private void onProcMessageBeforeAuth(string message)
        {
            try
            {
                string[] messageParams = message.Split(new string[] { "," }, StringSplitOptions.None);
                if(messageParams[0] != "A/u25")
                {
                    _log.Warning("Unauthorized Token has been received from {0}", _remoteAddress);
                    _connection.Tell("disconnected");
                    return;
                }
                string hash     = messageParams[2];
                _gamesymbol     = messageParams[3];
                string strUserID    = hash.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[0];
                string strPassword  = hash.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[1];
                
                //将连接的状态转换为等待认证状态。
                _connectionStatus = ConnectionStatus.Authenticating;
                _dbReaderProxy.Tell(new UserLoginRequest(strUserID.Trim(), strPassword, (_remoteAddress as IPEndPoint).Address.ToString(), PlatformTypes.WEB));
            }
            catch
            {
                _log.Warning("Unauthorized Token has been received from {0}", _remoteAddress);
                _connection.Tell("disconnected");
            }
        }

        private void procEnterMessage()
        {
            try
            {
                int gameID = DBMonitorSnapshot.Instance.getGameIDFromString(GameProviders.AMATIC, _gamesymbol);
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
                _log.Error("Exception has been occurred in AmaticWebsocketClientHandler::procEnterMessage {0}", ex);
            }
        }

        private void procInitMessage()
        {
            try
            {
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_AMATIC_DOINIT);
                _userActor.Tell(new FromConnRevMessage(Self, message));
            }
            catch (Exception ex)
            {
                _log.Error("Exception has been occurred in AmaticWebsocketClientHandler::procInitMessage {0}", ex);
            }
        }

        private void onProcMessage(string strMsg)
        {
            try
            {
                string[] messageParams = strMsg.Split(new string[] { "," }, StringSplitOptions.None);
                if(messageParams[0] == "A/u250")
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_AMATIC_DOHEARTBEAT);
                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }
                else if(messageParams[0] == "A/u251")
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_AMATIC_DOSPIN);

                    if(messageParams.Length == 4)
                    {
                        message.Append(Convert.ToInt32(messageParams[1]));  //行
                        message.Append(Convert.ToInt32(messageParams[2]));  //步骤
                        message.Append(Convert.ToInt32(-1));                //购买步骤(0,1,2)
                        message.Append(Convert.ToInt32(-1));                //反步骤(普通 : -1, 反 : 0)
                    }
                    else if(messageParams.Length == 5)
                    {
                        message.Append(Convert.ToInt32(messageParams[1]));  //行
                        message.Append(Convert.ToInt32(messageParams[2]));  //步骤
                        message.Append(Convert.ToInt32(-1));                //购买步骤(0,1,2)
                        message.Append(Convert.ToInt32(messageParams[4]));  //反步骤(普通 : -1, 反 : 0)
                    }

                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }
                else if (messageParams[0] == "A/u254")
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_AMATIC_DOCOLLECT);
                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }
                else if (messageParams[0] == "A/u256")
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_AMATIC_DOSPIN);

                    if (messageParams.Length == 4)
                    {
                        message.Append(Convert.ToInt32(messageParams[1]));  //行
                        message.Append(Convert.ToInt32(messageParams[2]));  //步骤
                        message.Append(Convert.ToInt32(-1));                //购买步骤(0,1,2)
                        message.Append(Convert.ToInt32(-1));                //反步骤(普通 : -1, 反 : 0)
                    }
                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }
                else if (messageParams[0] == "A/u257")
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_AMATIC_DOGAMBLEPICK);
                    message.Append(Convert.ToInt32(messageParams[1]));  //花色(1:Red, 2:Balck, 3:Diamond, 4:Heart, 5:Crob, 6:Spade)
                    
                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }
                else if (messageParams[0] == "A/u258")
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_AMATIC_DOGAMBLEHALF);

                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }
                else if (messageParams[0] == "A/u2510") //转轮
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_AMATIC_DOSPIN);

                    if (messageParams.Length == 4)
                    {
                        message.Append(Convert.ToInt32(messageParams[1]));  //行
                        message.Append(Convert.ToInt32(messageParams[2]));  //步骤
                        message.Append(Convert.ToInt32(-1));                
                        message.Append(Convert.ToInt32(-1));                
                    }
                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }
                else if (messageParams[0] == "A/u2517") //选项选择
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_AMATIC_FSOPTION);
                    message.Append(Convert.ToInt32(messageParams[1]));  //选项索引
                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }
                else if (messageParams[0] == "A/u2531") //重新旋转
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_AMATIC_DOSPIN);

                    if (messageParams.Length == 4)
                    {
                        message.Append(Convert.ToInt32(messageParams[1]));  //行
                        message.Append(Convert.ToInt32(messageParams[2]));  //步骤
                        message.Append(Convert.ToInt32(-1));
                        message.Append(Convert.ToInt32(-1));
                    }
                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }
                else if (messageParams[0] == "A/u2538") //强力旋转
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_AMATIC_DOSPIN);

                    if (messageParams.Length == 4)
                    {
                        message.Append(Convert.ToInt32(messageParams[1]));  //行
                        message.Append(Convert.ToInt32(messageParams[2]));  //步骤
                        message.Append(Convert.ToInt32(-1));
                        message.Append(Convert.ToInt32(-1));
                    }
                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }
                else if (messageParams[0] == "A/u2566")
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_AMATIC_DOSPIN);

                    if (messageParams.Length == 5)
                    {
                        message.Append(Convert.ToInt32(messageParams[1]));  //行
                        message.Append(Convert.ToInt32(messageParams[2]));  //步骤
                        message.Append(Convert.ToInt32(messageParams[4]));  //购买步骤(0,1,2)
                        message.Append(Convert.ToInt32(-1));                //反步骤(普通 : -1, 反 : 0)
                    }

                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }
                else if (messageParams[0] == "A/u291")
                {
                    //轮盘消息
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_AMATIC_DOSPIN);
                    strMsg = strMsg.Substring("A/u291,".Length);
                    message.Append(strMsg);  //行

                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }
                else if (messageParams[0] == "A/u260")
                {

                }
                else
                {
                    _log.Warning("Unauthorized Token has been received from {0}", _remoteAddress);
                    _connection.Tell("disconnected");
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
            _connection.Tell(new AmaticWsClientConnection.StringProtocalWrite(message));
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
    }

    public enum ConnectionStatus
    {
        Connected       = 0,
        Authenticating  = 1,
        Authenticated   = 2,
    }
}
