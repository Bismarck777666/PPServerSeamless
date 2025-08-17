using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using System.Net;
using System.Threading;
using Akka.Event;
using System.Net.WebSockets;
using System.Runtime.Remoting.Contexts;
using Newtonsoft.Json.Linq;
using static FrontNode.WebsocketService.WebsocketListener;
using System.Web;
using FrontNode.Database;
using StackExchange.Redis;
using GITProtocol;
using NLog.Fluent;
using Akka.Util;
using System.Windows.Interop;

namespace FrontNode.WebsocketService
{
    public class HttpListener : ReceiveActor
    {
        private string _listenURL;
        private int _acceptLimit;
        private Task[] _acceptTasks;
        private IActorRef _dbReaderProxy;
        private System.Net.HttpListener _httpListener = null;

        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        public HttpListener(Config config, IActorRef dbReader)
        {
            _dbReaderProxy = dbReader;
            _acceptLimit = config.GetInt("acceptlimit", 50);
            _listenURL = config.GetString("url", "http://localhost/");
            _httpListener = new System.Net.HttpListener();
            _httpListener.Prefixes.Add(_listenURL);

            Receive<string>(processCommand);
            ReceiveAsync<HttpListenerContext>(processIncomingRequest);
            Receive<NonHttpAccepted>(_ => acceptHttpTask());
        }

        protected override void PreStart()
        {
            base.PreStart();
        }

        private void processCommand(string strCommand)
        {
            if (strCommand == "start")
            {
                try
                {
                    _httpListener.Start();
                    _log.Info("Starting EGT HTTP service for url {0}", _listenURL);
                }
                catch (Exception ex)
                {
                    _log.Error("Failed in starting EGT HTTP service: Exception {0}", ex.Message);
                    return;
                }

                _log.Info("Listening EGT HTTP service....");
                _acceptTasks = accept().ToArray();
            }
            else if (strCommand == "stop")
            {
                _httpListener.Stop();
            }
        }

        private IEnumerable<Task> accept()
        {
            for (var i = 0; i < _acceptLimit; i++)
            {
                Task acceptTask = acceptHttpTask();
                yield return acceptTask;
            }
        }

        private async Task processIncomingRequest(HttpListenerContext context)
        {
            try
            {
                IPEndPoint remoteEndPoint = context.Request.RemoteEndPoint;
                string path = context.Request.Url.AbsolutePath.ToLower();   
                var queryParams = HttpUtility.ParseQueryString(context.Request.Url.Query);
                var sessionKey = queryParams["sessionKey"];
                var sections = sessionKey.Split('ÿ');
                
                if (path == "/jackpotstats" && context.Request.HttpMethod == "GET" && sections != null && sections.Length == 4)
                {
                    int gameid = DBMonitorSnapshot.Instance.getGameIDFromString(GameProviders.EGT, sections[3]);
                    string userid = sections[1];
                    UserLoginResponse loginResponse = await _dbReaderProxy.Ask<UserLoginResponse>(new UserLoginRequest(userid.ToString().Trim()), TimeSpan.FromSeconds(5));
                    
                    if (loginResponse.Result != LoginResult.OK)
                    {
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                    }
                    else
                    {
                        try
                        {
                            string strGlobalUserid = loginResponse.GlobalUserID;
                            RedisValue isUserOnline = await RedisDatabase.RedisCache.HashGetAsync("onlineusers", strGlobalUserid);
                            if(isUserOnline.IsNullOrEmpty)
                            {
                                context.Response.StatusCode = 400;
                                context.Response.Close();
                                acceptHttpTask();
                                return;
                            }

                            RedisValue userPath = await RedisDatabase.RedisCache.HashGetAsync("onlineusers", strGlobalUserid + "_path");
                            if(!userPath.IsNullOrEmpty)
                            {
                                IActorRef userActor = await Context.System.ActorSelection((string)userPath).ResolveOne(TimeSpan.FromSeconds(5));
                                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_EGT_JACKPOTSTATE);
                                message.Append(gameid);
                                SendMessageToUser resMessage = await userActor.Ask<SendMessageToUser>(new FromHttpRevMessage(message), TimeSpan.FromSeconds(5));
                                string strMsg = resMessage.Message.Pop().ToString();

                                byte[] buffer = Encoding.UTF8.GetBytes(strMsg);

                                context.Response.AddHeader("Access-Control-Allow-Origin", "*");
                                context.Response.ContentType = "application/json";
                                context.Response.StatusCode = 200;
                                context.Response.ContentEncoding = Encoding.UTF8;
                                context.Response.ContentLength64 = buffer.Length;

                                await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                                context.Response.Close();
                            }
                            else
                            {
                                context.Response.StatusCode = 401;
                                context.Response.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Error("Exception has been occurred in HttpListener::processIncomingRequest {0}", ex);
                            context.Response.StatusCode = 500;
                            context.Response.Close();
                        }
                    }
                }
                else
                {
                    context.Response.StatusCode = 403;
                    context.Response.Close();
                }
            }
            catch (Exception ex)
            {
                _log.Error("Exception has been occurred in EGTHttpListener::processIncomingRequest {0}", (object)ex);
                context.Response.StatusCode = 500;
                context.Response.Close();
            }

            acceptHttpTask();
        }

        private Task acceptHttpTask()
        {
            var _self = Self;
            return _httpListener.GetContextAsync().ContinueWith<object>(acceptTask =>
            {
                if (acceptTask.IsFaulted)
                {
                    _log.Error("GetContextAsync failed: {0}", acceptTask.Exception?.GetBaseException().Message);
                    return new NonHttpAccepted();
                }

                if (acceptTask.IsCanceled)
                {
                    _log.Warning("GetContextAsync was canceled");
                    return new NonHttpAccepted();
                }

                try
                {
                    HttpListenerContext result = acceptTask.Result;

                    if (!result.Request.IsWebSocketRequest)
                        return result;
                    else
                    {
                        result.Response.StatusCode = 400;
                        result.Response.Close();
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Unexpected error in acceptHttpTask: {0}", ex.Message);
                }

                return new NonHttpAccepted();
            }, TaskContinuationOptions.ExecuteSynchronously).PipeTo(_self);
        }

        protected override void PostStop()
        {
            base.PostStop();
        }

        public class NonHttpAccepted
        {

        }
    }
}
