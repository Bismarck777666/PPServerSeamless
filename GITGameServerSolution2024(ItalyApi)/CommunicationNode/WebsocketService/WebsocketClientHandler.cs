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
using CQ9Protocol;

namespace CommNode
{
    public class WebsocketClientHandler : ReceiveActor
    {
        private IActorRef                   _connection     = null;
        private EndPoint                    _remoteAddress  = null;
        private IActorRef                   _dbReaderProxy  = null;
        private IActorRef                   _dbWriterProxy  = null;
        private IActorRef                   _redisWriter    = null;
        private readonly ILoggingAdapter    _log            = Logging.GetLogger(Context);
        private IActorRef                   _userActor      = null;
        private string                      _strUserID      = "";
        private DateTime                    _lastReceivedTime;
        private ICancelable                 _schedulerCancel;
        private int                         _redisCheckRetryCount = 0;
        private ICancelable                 _redisWaitSchedulerCancel = null;
        private ConnectionStatus            _connectionStatus       = ConnectionStatus.Connected;

        private string                      _token          = "";   //套接字连接时接收的令牌

        private CQ9Parse                    _parser         = new CQ9Parse();


        public WebsocketClientHandler(IActorRef connection, EndPoint remoteAddress, IActorRef dbReader, IActorRef dbWriter, IActorRef redisWriter)
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
            return Akka.Actor.Props.Create(() => new WebsocketClientHandler(connection, remoteAddress, reader, writer, redisWriter));
        }

        private void initializeMessageProcs()
        {
            Receive<WsClientConnection.BynaryProtocalReceived>          (received => onBynaryReceiveData(received));
            Receive<WsClientConnection.StringProtocalReceived>          (received => onStringReceiveData(received));
            
            Receive<WsClientConnection.Disconnected>(closed => Context.Stop(Self));
            Receive<string>                         (command => processCommand(command));
            ReceiveAsync<UserLoginResponse>         (procUserLoginResponse);
            ReceiveAsync<CheckUserPathFromRedis>    (checkRegisteredUserPath);
            Receive<SendMessageToUser>(message =>
            {
                sendMessage(message.Message, message.Balance);
            });

            Receive<SendCQ9MessageToUser>(message =>
            {
                //首次发送会话时使用
                this.sendMessage(message);
            });

        }

        protected override void PreStart()
        {
            _schedulerCancel = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(1000, 1000, Self, Constants.CheckConnectionCommand, Self);
            base.PreStart();
        }

        //接收数据处理函数
        private void onBynaryReceiveData(WsClientConnection.BynaryProtocalReceived received)
        {
        }

        private async Task procUserLoginResponse(UserLoginResponse loginResponse)
        {
            if (loginResponse.Result != LoginResult.OK)
            {
                sendLoginResponse(loginResponse.Result);

                //如果登录失败则断开连接。
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
                //如果登录成功则创建用户角色。
                        _userActor = await Context.System.ActorSelection("/user/userManager").Ask<IActorRef>(new CreateNewUserMessage(Self, _dbReaderProxy, _dbWriterProxy, _redisWriter, loginResponse, PlatformTypes.WEB));
                        _strUserID = loginResponse.UserID;
                        _connectionStatus = ConnectionStatus.Authenticated;
                        sendLoginResponse(LoginResult.OK);
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
                    _log.Error("Exception has been occurred in WebsocketCLientHandler::procUserLoginResponse {0}", ex);
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

                    //将该连接注册到用户角色。
                    _userActor.Tell(new SocketConnectionAdded());
 
                    _strUserID          = strGlobalUserID;
                    _connectionStatus   = ConnectionStatus.Authenticated;
                    sendLoginResponse(request.Response.Result);
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
                _log.Error("Exception has been occurred in WebsocketClientHandler::checkRegisteredUserPath {0}", ex);
            }

            //确认为登录失败。
            sendLoginResponse(LoginResult.UNKNOWNERROR);
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
            if(message.MsgCode == (ushort)CSMSG_CODE.CS_HEARTBEAT)
            {
                sendIrsMessage();
                sendEvtMessage(balance);
            }
            else if(message.MsgCode == (ushort)SCMSG_CODE.SC_ENTERGAME)
            {
                byte status             = (byte) message.Pop();
                if(status == 0)
                {
                    //如果进入游戏成功，则发送用户持有资金并请求该游戏初始化信息
                    sendEvtMessage(balance);
                    GITMessage initGameMsg = buildDefaultMessage(CSMSG_CODE.CS_CQ9_InitGame1Request);
                    initGameMsg.Append(_token);
                    _userActor.Tell(new FromConnRevMessage(Self, initGameMsg));
                }
                else
                {
                    sendResMessage("",2, status, "Can not enter game");
                    _log.Warning("Can not enter game from {0}", _remoteAddress);
                    _connection.Tell("disconnected");
                }
            }
            else
            {
                if (message.MsgCode == (ushort) SCMSG_CODE.SC_CQ9_NormalSpinResponse || message.MsgCode == (ushort) SCMSG_CODE.SC_CQ9_CollectResponse)
                    sendEvtMessage(balance);

                bool mustEncrypt = (bool)  message.Pop();
                string  val      = (string)message.Pop();
                if (!mustEncrypt)
                    sendResMessage(val);
                else
                    sendResMessage(_parser._Secure.doEncryptAndAddIV(val, 0));
            }
        }

        /*
         *
         *
         *      Added By Bismarck 2022.01.11
         *
         *
         */

        private void sendLoginResponse(LoginResult result)
        {
            string encryptStr = _parser._Secure.doEncryptAndAddIV(makeEncryptKeyPacket(),0);

            CQ9ResponseResPacket resPacket = new CQ9ResponseResPacket();
            if (result == LoginResult.OK)
            {
                resPacket.err   = 200;
                resPacket.res   = 1;
                resPacket.msg   = null;
                resPacket.vals  = new object[] { 1, encryptStr };
        }
            else
            {
                resPacket.err   = 403;
                resPacket.res   = 1;
                resPacket.msg   = "Login Failed";
                resPacket.vals  = new object[] { 1, encryptStr };
            }

            string msg = JsonConvert.SerializeObject(resPacket);
            msg = string.Format("~j~{0}", msg);
            sendMessage(msg);
        }

        private void onStringReceiveData(WsClientConnection.StringProtocalReceived received)
        {
            _lastReceivedTime = DateTime.Now;
            string message = received.ReceivedData;

            List<string> strMessages = _parser.removeFrame(message);
            if (strMessages == null || strMessages.Count == 0)
                return;

            for (int i = 0; i < strMessages.Count; i++)
            {
                CQ9RequestPacket packet = _parser.parseMessage(strMessages[i]);
                if (packet is CQ9RequestReqPacket)
                {
                    CQ9RequestReqPacket reqPacket = packet as CQ9RequestReqPacket;
                    if (_connectionStatus == ConnectionStatus.Connected)
                    {
                        onProcMessageBeforeAuth(packet);
                    }
                    else if (_connectionStatus == ConnectionStatus.Authenticating)
                    {
                        //在查找用户操作期间，无法处理任何消息
                        _log.Warning("Unauthorized Message has been received from {0}", _remoteAddress);
                        _connection.Tell("disconnected");
                        return;
                    }
                    else
                        onProcMessage(packet);
                }
                else if (packet is CQ9RequestIrqPacket)
                {
                    if (_connectionStatus == ConnectionStatus.Authenticated)
                        onProcMessage(packet);
                }
            }

        }

        private void onProcMessageBeforeAuth(object obj)
        {
            if(obj is CQ9RequestReqPacket)
            {
                CQ9RequestReqPacket reqPacket = obj as CQ9RequestReqPacket;
                if(reqPacket.req != 1)
                {
                    //判断为非法用户并阻断连接。
                    _log.Warning("Unauthorized Message has been received from {0}", _remoteAddress);
                    _connection.Tell("disconnected");
                    return;
                }
                try
                {
                    _token = reqPacket.vals[3];
                    string strGlobalUserID  = findUserIDFromToken(_token);
                    string strPassword      = _token.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    strPassword             = strPassword.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    int agentDbId           = Convert.ToInt32(strGlobalUserID.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                    string strUserId        = strGlobalUserID.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    //将连接的状态转换为认证等待状态。
                    _connectionStatus = ConnectionStatus.Authenticating;
                    _dbReaderProxy.Tell(new UserLoginRequest(agentDbId, strUserId.Trim(), strPassword, (_remoteAddress as IPEndPoint).Address.ToString(), PlatformTypes.WEB));
                }
                catch
                {
                    _log.Warning("Unauthorized Token has been received from {0}", _remoteAddress);
                    _connection.Tell("disconnected");
                }
            }
            else
            {
                //判断为非法用户并阻断连接。
                _log.Warning("Unauthorized Message has been received from {0}", _remoteAddress);
                _connection.Tell("disconnected");
                return;
            }
        }
        
        private void procInit1Message(JObject reqParam)
        {
            try
            {
                string GameSymbol = reqParam["GameID"].Value<string>();
                int gameID = DBMonitorSnapshot.Instance.getGameIDFromString(GAMETYPE.CQ9, GameSymbol);
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
            catch(Exception ex)
            {
                _log.Error("Exception has been occurred in WebsocketClientHandler::procInit1Message {0}", ex);
            }
        }

        private void procMiniLobbyMessage(JObject reqParam)
        {
            try
            {
                string GameSymbol = reqParam["GameID"].Value<string>();
                int gameID = DBMonitorSnapshot.Instance.getGameIDFromString(GAMETYPE.CQ9, GameSymbol);
                if (gameID == 0)
                {
                    _log.Warning("Invalid Game minilobby enter request from {0}", _remoteAddress);
                    _connection.Tell("disconnected");
                    return;
                }

                string val = string.Format("/cq9/reOpenGame?token={0}&gameSymbol={1}", _token, GameSymbol);
                this.sendResMessage(_parser._Secure.doEncryptAndAddIV(val, 0), 202);
            }
            catch (Exception ex)
            {
                _log.Error("Exception has been occurred in WebsocketClientHandler::procInit1Message {0}", ex);
            }

        }
        
        private void onProcMessage(CQ9RequestPacket packet)
        {
            try
            {
                if (packet is CQ9RequestIrqPacket)
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_HEARTBEAT);
                    _userActor.Tell(new FromConnRevMessage(Self, message));
                    return;
                }

                if (!(packet is CQ9RequestReqPacket))
                    return;

                CQ9RequestReqPacket reqPacket = packet as CQ9RequestReqPacket;
                if (reqPacket.req == 2)
                {
                    JObject reqParam = (JObject)JsonConvert.DeserializeObject(_parser._Secure.doGetIVAndDecrypt(reqPacket.vals[1], 0));
                    int msgType = reqParam["Type"].Value<int>();
                    int msgCode = reqParam["ID"].Value<int>();
                    if (msgType == 1)
                    {
                        if (msgCode == (int)CQ9MessageCode.InitGame1Request)
                        {
                            procInit1Message(reqParam);
                        }
                        else if (msgCode == (int)CQ9MessageCode.InitGame2Request)
                        {
                            GITMessage message = buildDefaultMessage(CSMSG_CODE.CS_CQ9_InitGame2Request);
                            _userActor.Tell(new FromConnRevMessage(Self, message));
                        }
                    }
                    else if (msgType == 3)
                    {
                        GITMessage message = null;
                        if (msgCode == (int)CQ9MessageCode.NormalSpinRequest)
                            message = buildNormalSpinMessage(reqParam);
                        else if (msgCode == (int)CQ9MessageCode.FreeSpinOptionSelectRequest)
                            message = buildFreeOptSelectMessage(reqParam);
                        else if (msgCode == (int)CQ9MessageCode.TembleSpinRequest)
                            message = buildTembleSpinMessage(reqParam);
                        else
                            message = buildDefaultMessage((CSMSG_CODE)(msgCode + 3000));

                        _userActor.Tell(new FromConnRevMessage(Self, message));

                    }
                }
                else if(reqPacket.req == 202)//迷你大厅消息
                {
                    JObject reqParam = (JObject)JsonConvert.DeserializeObject(_parser._Secure.doGetIVAndDecrypt(reqPacket.vals[1], 0));
                    procMiniLobbyMessage(reqParam);
                }
            }
            catch(Exception ex)
            {
                _log.Error("Exception has been occurred in WebsocketClientHandler::onProcMessage {0}", ex);
            }
        }
        
        private void sendMessage(SendCQ9MessageToUser message)
        {
            sendMessage(message.Message);
        }

        private void sendIrsMessage()
        {
            CQ9ResponseIrsPacket resPacket = new CQ9ResponseIrsPacket();
            resPacket.err   = 0;
            resPacket.msg   = null;
            resPacket.irs   = 1;
            resPacket.vals  = new long[] { 1, -2147483648, 2, Pcg.Default.Next(99999999) };

            sendMessage(string.Format("~j~{0}", JsonConvert.SerializeObject(resPacket)));
        }
        
        private void sendResMessage(string resVal,int resCode = 2,int err = 200,string msg = null)
        {
            CQ9ResponseResPacket resPacket = new CQ9ResponseResPacket();
            resPacket.err   = err;
            resPacket.res   = resCode;
            resPacket.msg   = msg;
            resPacket.vals  = new object[] { 1, resVal };

            sendMessage(string.Format("~j~{0}",JsonConvert.SerializeObject(resPacket)));
        }
        
        private void sendEvtMessage(double balance)
        {
            CQ9ResponseEvtPacket resPacket = new CQ9ResponseEvtPacket();
            resPacket.evt   = 1;
            resPacket.vals  = new double[] { 1, Math.Round(balance, 2) };

            sendMessage(string.Format("~j~{0}", JsonConvert.SerializeObject(resPacket)));
        }
        
        private void sendMessage(string message)
        {
            int len     = message.Length;
            string msg  = string.Format("{0}{1}{2}{3}", "~m~", len.ToString(), "~m~", message);
            _connection.Tell(new WsClientConnection.StringProtocalWrite(msg));
        }
        
        private string makeEncryptKeyPacket()
        {
            JObject obj = new JObject();
            obj["E"] = "60A299E7243EDEA7";
            obj["V"] = 0;
            return JsonConvert.SerializeObject(obj);
        }

        private GITMessage buildDefaultMessage(CSMSG_CODE msgcode)
        {
            //11,12,32,41,42,43,44,46
            GITMessage message = new GITMessage((ushort)msgcode);
            return message;
        }
        
        private GITMessage buildFreeOptSelectMessage(JObject reqParam)
        {
            GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_CQ9_FreeSpinOptSelectRequest);
            message.Append((int)reqParam["PlayerSelectIndex"]);
            return message;
        }
        
        private GITMessage buildNormalSpinMessage(JObject reqParam)
        {
            int playLine        = reqParam["PlayLine"].Value<int>();
            int isExtraBet      = reqParam["IsExtraBet"].Value<int>();
            int playBet         = reqParam["PlayBet"].Value<int>();
            int playDenom       = reqParam["PlayDenom"].Value<int>();
            int miniBet         = reqParam["MiniBet"].Value<int>();
            double reelPay      = 0.0f;
            if(reqParam.ContainsKey("ReelPay"))
                reelPay = reqParam["ReelPay"].Value<int>();

            JArray reelSelected = new JArray();
            if (reqParam.ContainsKey("ReelSelected"))
                reelSelected = reqParam["ReelSelected"].Value<JArray>();

            GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_CQ9_NormalSpinRequest);
            message.Append(playLine);
            message.Append(isExtraBet);
            message.Append(playBet);
            message.Append(playDenom);
            message.Append(miniBet);
            message.Append(reelPay);
            message.Append(reelSelected.Count);
            for (int i = 0; i < reelSelected.Count; i++)
                message.Append((int)reelSelected[i]);
            return message;
        }
        
        private GITMessage buildTembleSpinMessage(JObject reqParam)
        {
            int playLine        = reqParam["PlayLine"].Value<int>();
            int isExtraBet      = reqParam["IsExtraBet"].Value<int>();
            int playBet         = reqParam["PlayBet"].Value<int>();
            int miniBet         = reqParam["MiniBet"].Value<int>();
            double reelPay      = 0.0f;
            if(reqParam.ContainsKey("ReelPay"))
                reelPay = reqParam["ReelPay"].Value<int>();

            JArray reelSelected = new JArray();
            if (reqParam.ContainsKey("ReelSelected"))
                reelSelected = reqParam["ReelSelected"].Value<JArray>();


            string ticket = "";
            if (reqParam.ContainsKey("Ticket"))
                ticket = reqParam["Ticket"].Value<string>();

            GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_CQ9_TembleSpinRequest);
            message.Append(playLine);
            message.Append(isExtraBet);
            message.Append(playBet);
            message.Append(miniBet);
            message.Append(reelPay);
            message.Append(reelSelected.Count);
            for (int i = 0; i < reelSelected.Count; i++)
                message.Append((int)reelSelected[i]);
            message.Append(ticket);
            return message;
        }
        
        protected override void PostStop()
        {
            if (_userActor != null)
            {
                //当前认证已完成的状态下
                _userActor.Tell(new SocketConnectionClosed());
            }

            if (_schedulerCancel != null)
                _schedulerCancel.Cancel();

            if(_redisWaitSchedulerCancel != null)
                _redisWaitSchedulerCancel.Cancel();
    
            base.PostStop();
        }

        private string findUserIDFromToken(string strToken)
        {
            string[] strParts = strToken.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
            if (strParts.Length != 2)
                return null;

            return strParts[0];
        }
    }

    public enum ConnectionStatus
    {
        Connected       = 0,
        Authenticating  = 1,
        Authenticated   = 2,
    }
}
