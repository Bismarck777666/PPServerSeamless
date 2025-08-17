using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using System.Threading;
using System.IO;
using System.Net.WebSockets;
using Akka.Event;
using Akka.IO;
using CQ9Protocol;

namespace CommNode
{
    public class AmaticWsClientConnection : ReceiveActor
    {
        private WebSocket                   _wsClient;
        private CancellationTokenSource     _readCancelToken    = new CancellationTokenSource();
        private IActorRef                   _connectionHandler;
        private readonly ILoggingAdapter    _log                = Logging.GetLogger(Context);
        
        private byte[]                      _recvBuffer         = new byte[8 * 12040];

        public AmaticWsClientConnection(WebSocket wsClient)
        {
            _wsClient = wsClient;

            Receive<RegisterHandler>(registerHandler =>
            {
                _connectionHandler = registerHandler.ConnectionHandler;

                //자료읽기를 시작한다.
                Self.Tell("read");
            });

            Receive<string>                     (processCommand);
            Receive<ReadStreamMessage>          (onReceiveReadStream);
            ReceiveAsync<StringProtocalWrite>   (onStringWriteData);
        }
        
        protected override void PreStart()
        {
            base.PreStart();
        }
        
        private void processCommand(string command)
        {
            if(command == "read")
            {
                startReceive();
            }
            else if(command == "disconnected")
            {
                _readCancelToken.Cancel();
                _connectionHandler.Tell(new Disconnected());
                _wsClient.Dispose();
                Context.Stop(Self);
            }
            else if(command == "close")
            {
                _wsClient.Dispose();
            }
        }

        private void startReceive()
        {
            if (_wsClient.State != WebSocketState.Open)
            {
                Self.Tell("disconnected");
                return;
            }
            
            try
            {
                _wsClient.ReceiveAsync(new ArraySegment<byte>(_recvBuffer), _readCancelToken.Token).ContinueWith((readTask =>
                {
                    if(readTask.Status == TaskStatus.Canceled)
                    {
                        _wsClient.Dispose();
                        _connectionHandler.Tell("disconnected");
                        return new ReadStreamMessage(null);
                    }

                    return new ReadStreamMessage(readTask.Result);
                }), TaskContinuationOptions.ExecuteSynchronously).PipeTo(Self);
            }
            catch (TaskCanceledException)
            {

            }
            catch (Exception ex)
            {
                //웹소켓에서 자료를 읽는 과정에 례외발생
                _log.Error("Error Reading from web socket " + ex.Message);
                try
                {
                    _wsClient.Dispose();
                    _connectionHandler.Tell("disconnected");
                }
                catch
                {
                }
            }
        }
        
        private void onReceiveReadStream(ReadStreamMessage msgReadStream)
        {
            if(msgReadStream.ReadStream == null)
            {
                Self.Tell("disconnected");
                return;
            }

            try
            {
                if (msgReadStream.ReadStream.Count == 0)
                {
                    Self.Tell("read");
                }
                else if (msgReadStream.ReadStream.MessageType == WebSocketMessageType.Text)
                {
                    onReceiveStringData(Encoding.UTF8.GetString(_recvBuffer, 0, msgReadStream.ReadStream.Count));
                }
                else
                {
                    Self.Tell("disconnected");
                    return;
                }
            }
            catch
            {
                Self.Tell("read");
            }
        }
        
        private async Task onStringWriteData(StringProtocalWrite writeData)
        {
            if (_wsClient.State != WebSocketState.Open)
            {
                Self.Tell("disconnected");
                return;
            }

            try
            {
                await _wsClient.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(writeData.Data)), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch(Exception ex)
            {
                //웹소켓에서 자료를 읽는 과정에 례외발생
                _log.Error("AmaticWsClientConnection::onWriteData " + ex.ToString());

                //웹소켓을 닫기한다.
                try
                {
                    _wsClient.Dispose();
                    _connectionHandler.Tell("disconnected");
                }
                catch
                {

                }
            }
        }
        
        private void onReceiveStringData(string receivedData)
        {
            _connectionHandler.Tell(new StringProtocalReceived(receivedData));

            Self.Tell("read");
        }

        public class ReadStreamMessage
        {
            public WebSocketReceiveResult ReadStream { get; private set; }
            public ReadStreamMessage(WebSocketReceiveResult readStream)
            {
                this.ReadStream = readStream;
            }
        }
        
        public class RegisterHandler
        {
            public IActorRef ConnectionHandler { get; private set; }
            public RegisterHandler(IActorRef connectionHandler)
            {
                this.ConnectionHandler = connectionHandler;
            }
        }
        
        public class Disconnected
        {

        }
        
        public class StringProtocalWrite
        {
            public string Data  { get; set; }
            public StringProtocalWrite(string data)
            {
                this.Data = data;
            }
        }
        
        public class StringProtocalReceived
        {
            public string ReceivedData      { get; set; }
            public StringProtocalReceived(string receivedData)
            {
                this.ReceivedData = receivedData;
            }
        }
    }
}
