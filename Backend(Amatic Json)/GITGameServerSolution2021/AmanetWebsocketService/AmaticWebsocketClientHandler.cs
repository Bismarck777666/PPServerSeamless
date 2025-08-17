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

                //로그인실패라면 련결을 끊는다.
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
                        //로그인성공이면 유저액터를 창조한다.
                        _userActor = await Context.System.ActorSelection("/user/userManager").Ask<IActorRef>(new CreateNewUserMessage(Self, _dbReaderProxy, _dbWriterProxy, _redisWriter, loginResponse, PlatformTypes.WEB));
                        _strUserID          = loginResponse.UserID;
                        _connectionStatus   = ConnectionStatus.Authenticated;
                        procEnterMessage();
                        return;
                    }

                    //이미 로그인되였다면 해당 유저의 패스를 얻는다.
                    //만일 패스가 등록되지 않은 경우 최대 20초동안 대기한다. (40 * 0.5초)
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

                    //해당 연결을 유저액터에 등록한다.
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

                //20초가 지났다면
                if (_redisCheckRetryCount < 40)
                    return;
            }
            catch (Exception ex)
            {
                _log.Error("Exception has been occurred in AmaticWebsocketClientHandler::checkRegisteredUserPath {0}", ex);
            }

            //로그인 실패로 확정한다.
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
                //유저액토를 찾을동안에는 그어떤 메시지도 처리할수없다
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
                
                //련결의 상태를 인증대기 상태로 전환한다.
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
                        message.Append(Convert.ToInt32(messageParams[1]));  //라인
                        message.Append(Convert.ToInt32(messageParams[2]));  //벳스텝
                        message.Append(Convert.ToInt32(-1));                //구매스텝(0,1,2)
                        message.Append(Convert.ToInt32(-1));                //앤티스텝(일반 : -1, 앤티 : 0)
                    }
                    else if(messageParams.Length == 5)
                    {
                        message.Append(Convert.ToInt32(messageParams[1]));  //라인
                        message.Append(Convert.ToInt32(messageParams[2]));  //벳스텝
                        message.Append(Convert.ToInt32(-1));                //구매스텝(0,1,2)
                        message.Append(Convert.ToInt32(messageParams[4]));  //앤티스텝(일반 : -1, 앤티 : 0)
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
                        message.Append(Convert.ToInt32(messageParams[1]));  //라인
                        message.Append(Convert.ToInt32(messageParams[2]));  //벳스텝
                        message.Append(Convert.ToInt32(-1));                //구매스텝(0,1,2)
                        message.Append(Convert.ToInt32(-1));                //앤티스텝(일반 : -1, 앤티 : 0)
                    }
                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }
                else if (messageParams[0] == "A/u257")
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_AMATIC_DOGAMBLEPICK);
                    message.Append(Convert.ToInt32(messageParams[1]));  //픽크(1:Red, 2:Balck, 3:Diamond, 4:Heart, 5:Crob, 6:Spade)
                    
                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }
                else if (messageParams[0] == "A/u258")
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_AMATIC_DOGAMBLEHALF);

                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }
                else if (messageParams[0] == "A/u2510") //휠돌리기
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_AMATIC_DOSPIN);

                    if (messageParams.Length == 4)
                    {
                        message.Append(Convert.ToInt32(messageParams[1]));  //라인
                        message.Append(Convert.ToInt32(messageParams[2]));  //벳스텝
                        message.Append(Convert.ToInt32(-1));                
                        message.Append(Convert.ToInt32(-1));                
                    }
                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }
                else if (messageParams[0] == "A/u2517") //옵션선택
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_AMATIC_FSOPTION);
                    message.Append(Convert.ToInt32(messageParams[1]));  //옵션인덱스
                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }
                else if (messageParams[0] == "A/u2531") //리스핀
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_AMATIC_DOSPIN);

                    if (messageParams.Length == 4)
                    {
                        message.Append(Convert.ToInt32(messageParams[1]));  //라인
                        message.Append(Convert.ToInt32(messageParams[2]));  //벳스텝
                        message.Append(Convert.ToInt32(-1));
                        message.Append(Convert.ToInt32(-1));
                    }
                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }
                else if (messageParams[0] == "A/u2538") //파워스핀
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_AMATIC_DOSPIN);

                    if (messageParams.Length == 4)
                    {
                        message.Append(Convert.ToInt32(messageParams[1]));  //라인
                        message.Append(Convert.ToInt32(messageParams[2]));  //벳스텝
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
                        message.Append(Convert.ToInt32(messageParams[1]));  //라인
                        message.Append(Convert.ToInt32(messageParams[2]));  //벳스텝
                        message.Append(Convert.ToInt32(messageParams[4]));  //구매스텝(0,1,2)
                        message.Append(Convert.ToInt32(-1));                //앤티스텝(일반 : -1, 앤티 : 0)
                    }

                    _userActor.Tell(new FromConnRevMessage(Self, message));
                }
                else if (messageParams[0] == "A/u291")
                {
                    //룰렛메시지
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_AMATIC_DOSPIN);
                    strMsg = strMsg.Substring("A/u291,".Length);
                    message.Append(strMsg);  //라인

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
