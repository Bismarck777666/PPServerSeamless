using Akka.Actor;
using Akka.Event;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace FrontNode.HTTPService
{
    internal class HTTPReplayWorkActor : ReceiveActor
    {
        private readonly ILoggingAdapter    _logger         = Context.GetLogger();
        private IActorRef                   _dbReaderProxy  = null;

        public HTTPReplayWorkActor(IActorRef dbReaderProxy)
        {
            _dbReaderProxy = dbReaderProxy;
            ReceiveAsync<HTTPPPReplayListRequest>       (onPPReplayListRequest);
            ReceiveAsync<HTTPPPReplayDataRequest>       (onPPReplayDataRequest);
            ReceiveAsync<HTTPPPReplayMakeLinkRequest>   (onPPReplayMakeLinkRequest);
        }

        private async Task<string> findUserActor(string strUserID, string strSessionToken)
        {
            try
            {
                RedisValue redisValue = await Database.RedisDatabase.RedisCache.HashGetAsync((RedisKey)(strUserID + "_tokens"), (RedisValue)strSessionToken);
                if (redisValue.IsNullOrEmpty)
                    return null;

                string strUserActorPath = (string)redisValue;
                return strUserActorPath;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPReplayWorkActor::findUserActor {0}", ex);
                return null;
            }
        }

        private async Task onPPReplayMakeLinkRequest(HTTPPPReplayMakeLinkRequest request)
        {
            try
            {
                string strUserActorPath = await findUserActor(request.UserID, request.Token);
                if (strUserActorPath == null)
                {
                    Sender.Tell("{\"error\":10,\"description\":\"Request is not authorized\"}");
                    return;
                }
                
                HTTPPPReplayMakeLinkResponse response = await _dbReaderProxy.Ask<HTTPPPReplayMakeLinkResponse>(request, TimeSpan.FromSeconds(15.0));
                Sender.Tell(response.Response);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPReplayWorkActor::onPPReplayMakeLinkRequest {0}", ex);
                Sender.Tell("{\"error\":10,\"description\":\"Request is not authorized\"}");
            }
        }

        private async Task onPPReplayDataRequest(HTTPPPReplayDataRequest request)
        {
            try
            {
                if (request.UserID != null)
                {
                    string strUserActorPath = await findUserActor(request.UserID, request.Token);
                    if (strUserActorPath == null)
                    {
                        Sender.Tell("{\"error\":10,\"description\":\"Request is not authorized\"}");
                        return;
                    }
                }

                HTTPPPReplayDataResponse response = await _dbReaderProxy.Ask<HTTPPPReplayDataResponse>(request, TimeSpan.FromSeconds(15.0));
                if (response.Response != null)
                    Sender.Tell(response.Response);
                else
                    Sender.Tell("{\"error\":10,\"description\":\"Request is not authorized\"}");
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPReplayWorkActor::findUserActor {0}", ex);
                Sender.Tell("{\"error\":10,\"description\":\"Request is not authorized\"}");
            }
        }

        private async Task onPPReplayListRequest(HTTPPPReplayListRequest request)
        {
            try
            {
                string strUserActorPath = await findUserActor(request.UserID, request.Token);
                if (strUserActorPath == null)
                {
                    Sender.Tell("{\"error\":10,\"description\":\"Request is not authorized\"}");
                    return;
                }

                HTTPPPReplayListResponse response   = await _dbReaderProxy.Ask<HTTPPPReplayListResponse>(request, TimeSpan.FromSeconds(15.0));
                PPReplayListResponse replayResponse = new PPReplayListResponse();
                replayResponse.error        = 0;
                replayResponse.description  = "OK";
                replayResponse.topList      = response.Items;
                string strResponse = JsonConvert.SerializeObject(replayResponse, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                Sender.Tell(strResponse);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPReplayWorkActor::findUserActor {0}", ex);
                Sender.Tell("{\"error\":10,\"description\":\"Request is not authorized\"}");
            }
        }
    }
}
