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

namespace FrontNode.WebsocketService
{
    public class WebsocketListener : ReceiveActor
    {
        private string              _listenURL;
        private int                 _acceptLimit;
        private Task[]              _acceptTasks;
        private IActorRef           _dbReader;
        private HttpListener        _httpListener = null;

        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        public WebsocketListener(Config config, IActorRef dbReader)
        {
            _dbReader       = dbReader;
            _acceptLimit    = config.GetInt("acceptlimit",  50);
            _listenURL      = config.GetString("url", "http://localhost");
            _httpListener   = new HttpListener();
            _httpListener.Prefixes.Add(_listenURL);
            
            Receive<string>                     (processCommand);
            ReceiveAsync<HttpListenerContext>   (processIncomingConnection);
            Receive<NonWebsocketAccepted>       (_ => acceptWebsocketTask());
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
                    _log.Info("Starting Greentube Websocket service for url {0}", _listenURL);
                }
                catch (Exception ex)
                {
                    _log.Error("Failed in starting Greentube Websocket service: Exception {0}", ex.Message);
                    return;
                }

                _log.Info("Listening greentube websocket service....");
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
                Task acceptTask = acceptWebsocketTask();
                yield return acceptTask;
            }
        }
        
        private async Task processIncomingConnection(HttpListenerContext context)
        {
            try
            {
                HttpListenerWebSocketContext websocketContext = await context.AcceptWebSocketAsync("mux");
                IPEndPoint remoteEndPoint = context.Request.RemoteEndPoint;
                var connection          = Context.ActorOf(Props.Create(() => new WsClientConnection(websocketContext.WebSocket)));
                var connectionHandler   = Context.ActorOf(WebsocketClientHandler.Props(connection, remoteEndPoint, _dbReader));
                connection.Tell(new WsClientConnection.RegisterHandler(connectionHandler));

                _log.Info("Greentube Websocket Connect request from {0}", remoteEndPoint.ToString());
            }
            catch(Exception ex)
            {
                _log.Error("Exception has been occurred in GreentubeWebsocketListener::processIncomingConnection {0}", (object)ex);
                context.Response.StatusCode = 500;
                context.Response.Close();

            }
            acceptWebsocketTask();
        }

        private Task acceptWebsocketTask()
        {
            return _httpListener.GetContextAsync().ContinueWith<object>((acceptTask =>
            {
                try
                {
                    HttpListenerContext result = acceptTask.Result;
                    if (result.Request.IsWebSocketRequest)
                        return result;
                    result.Response.StatusCode = 400;
                    result.Response.Close();
                }
                catch (Exception ex)
                {
                }

                return new NonWebsocketAccepted();
            }), TaskContinuationOptions.ExecuteSynchronously).PipeTo(Self);
        }
        
        public class NonWebsocketAccepted
        {

        }
    }
}
