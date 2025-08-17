using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using FrontNode.Database;
using StackExchange.Redis;
using Akka.Event;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using GITProtocol;
using System.Net;

namespace FrontNode.HTTPService
{
    class HTTPHistoryWorkActor : ReceiveActor
    {
        private readonly ILoggingAdapter    _logger         = Logging.GetLogger(Context);
        private IActorRef                   _dbReaderProxy  = null;
        public HTTPHistoryWorkActor(IActorRef dbReaderProxy)
        {
            _dbReaderProxy = dbReaderProxy;
            ReceiveAsync<HTTPPPHistoryGenralSettingRequest> (onPPHistorySettingRequest);
            ReceiveAsync<HTTPPPHistoryGetLastItemsRequest>  (onPPHistoryGetLastItemsRequest);
            ReceiveAsync<HTTPPPHistoryGetItemDetailRequest> (onPPHistoryGetItemDetailRequest);

        }
        private async Task<string> findUserActor(string strGlobalUserID, string strSessionToken)
        {
            try
            {
                RedisValue redisValue = await RedisDatabase.RedisCache.HashGetAsync(strGlobalUserID + "_tokens", strSessionToken);
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

                List<PPGameRecentHistoryDetailItem> historyItems = await _dbReaderProxy.Ask<List<PPGameRecentHistoryDetailItem>>(request, TimeSpan.FromSeconds(15));
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(JsonConvert.SerializeObject(historyItems))});
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
                    Sender.Tell(new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized });
                    return;
                }

                List<PPGameRecentHistoryItem>  historyItems = await _dbReaderProxy.Ask<List<PPGameRecentHistoryItem>>(request, TimeSpan.FromSeconds(15));

                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(JsonConvert.SerializeObject(historyItems))});
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPHistoryWorkActor::onPPHistoryGetLastItemsRequest {0}", ex);
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized });
            }
        }
        private async Task onPPHistorySettingRequest(HTTPPPHistoryGenralSettingRequest request)
        {
            try
            {
                string strUserActorPath = await findUserActor(request.UserID, request.Token);
                if (strUserActorPath == null)
                {
                    Sender.Tell(new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized });
                    return;
                }

                Sender.Tell(new HttpResponseMessage() { Content = new StringContent("{\"language\":\"en\",\"jurisdiction\":\"99\",\"jurisdictionRequirements\":[],\"brandRequirements\":[]}") });
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPHistoryWorkActor::onPPHistorySettingRequest {0}", ex);
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized });
            }
        }
    }
}
