using Akka.Actor;
using Akka.Event;
using GITProtocol;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FrontNode.HTTPService
{
    internal class HTTPHistoryWorkActor : ReceiveActor
    {
        private readonly ILoggingAdapter    _logger         = Context.GetLogger();
        private IActorRef                   _dbReaderProxy  = null;

        public HTTPHistoryWorkActor(IActorRef dbReaderProxy)
        {
            _dbReaderProxy = dbReaderProxy;
            ReceiveAsync<HTTPPPHistoryGenralSettingRequest>     (onPPHistorySettingRequest);
            ReceiveAsync<HTTPPPHistoryGetLastItemsRequest>      (onPPHistoryGetLastItemsRequest);
            ReceiveAsync<HTTPPPHistoryGetItemDetailRequest>     (onPPHistoryGetItemDetailRequest);
        }

        private async Task<string> findUserActor(string strGlobalUserID, string strSessionToken)
        {
            try
            {
                RedisValue redisValue = await Database.RedisDatabase.RedisCache.HashGetAsync((RedisKey)(strGlobalUserID + "_tokens"), (RedisValue)strSessionToken);
                if (redisValue.IsNullOrEmpty)
                    return null;

                string strUserActorPath = (string)redisValue;
                return strUserActorPath;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPHistoryWorkActor::findUserActor {0}", ex);
                return null;
            }
        }

        private async Task onPPHistoryGetItemDetailRequest(HTTPPPHistoryGetItemDetailRequest request)
        {
            try
            {
                string strUserActorPath = await findUserActor(request.UserID, request.Token);
                if (strUserActorPath == null)
                {
                    Sender.Tell(new HttpResponseMessage() { Content = new StringContent(""), StatusCode = HttpStatusCode.Unauthorized });
                    return;
                }
                    
                List<PPGameRecentHistoryDetailItem> historyItems = await _dbReaderProxy.Ask<List<PPGameRecentHistoryDetailItem>>(request, TimeSpan.FromSeconds(15.0));
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(JsonConvert.SerializeObject(historyItems)) });
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPHistoryWorkActor::onPPHistoryGetLastItemsRequest {0}", ex);
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(""), StatusCode = HttpStatusCode.Unauthorized });
            }
        }

        private async Task onPPHistoryGetLastItemsRequest(HTTPPPHistoryGetLastItemsRequest request)
        {
            try
            {
                string strUserActorPath = await findUserActor(request.UserID, request.Token);
                if (strUserActorPath == null)
                {
                    Sender.Tell(new HttpResponseMessage() { Content = new StringContent(""), StatusCode = HttpStatusCode.Unauthorized });
                    return;
                }
                
                List<PPGameRecentHistoryItem> historyItems = await _dbReaderProxy.Ask<List<PPGameRecentHistoryItem>>(request, TimeSpan.FromSeconds(15.0));
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(JsonConvert.SerializeObject(historyItems)) });
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPHistoryWorkActor::onPPHistoryGetLastItemsRequest {0}", ex);
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(""), StatusCode = HttpStatusCode.Unauthorized });
            }
        }

        private async Task onPPHistorySettingRequest(HTTPPPHistoryGenralSettingRequest request)
        {
            try
            {
                string strUserActorPath = await findUserActor(request.UserID, request.Token);
                if (strUserActorPath == null)
                {
                    Sender.Tell(new HttpResponseMessage() { Content = new StringContent(""), StatusCode = HttpStatusCode.Unauthorized });
                    return;
                }

                Sender.Tell(new HttpResponseMessage() { Content = new StringContent("{\"language\":\"en\",\"jurisdiction\":\"99\",\"jurisdictionRequirements\":[],\"brandRequirements\":[]}") });
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPHistoryWorkActor::onPPHistorySettingRequest {0}", ex);
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(""), StatusCode = HttpStatusCode.Unauthorized });
            }
        }
    }
}
