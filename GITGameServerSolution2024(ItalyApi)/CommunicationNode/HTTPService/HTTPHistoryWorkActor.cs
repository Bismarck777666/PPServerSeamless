using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using CommNode.Database;
using StackExchange.Redis;
using Akka.Event;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using GITProtocol;
using System.Net;

namespace CommNode.HTTPService
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
            ReceiveAsync<HTTPPPVerifyGetLastItemRequest>    (onPPVerifyGetLastItemRequest);
            
            ReceiveAsync<BNGTransactionListRequest>         (onBNGTransactionList);
            ReceiveAsync<BNGAggregateRequest>               (onBNGAggregateRequest);
            ReceiveAsync<BNGTransDetailRequest>             (onBNGTransDetailRequest);
            
            ReceiveAsync<CQ9RoundListRequest>               (onCQ9RoundListRequest);
            ReceiveAsync<CQ9RoundDetailRequest>             (onCQ9RoundDetailRequest);
            ReceiveAsync<CQ9SearchRoundRequest>             (onCQ9SearchRoundIDRequest);
            
            ReceiveAsync<HabaneroGetHistoryRequest>         (onHabaneroGetHistoryRequest);
            ReceiveAsync<HabaneroGetGameDetailRequest>      (onHabaneroGetGameDetailRequest);

            ReceiveAsync<PlaysonTransactionListRequest>     (onPlaysonTransactionList);
            ReceiveAsync<PlaysonAggregateRequest>           (onPlaysonAggregateRequest);
            ReceiveAsync<PlaysonGetTransRequest>            (onPlaysonGetTransRequest);
            ReceiveAsync<PlaysonTransDetailRequest>         (onPlaysonTransDetailRequest);
            
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
        private async Task onPPVerifyGetLastItemRequest(HTTPPPVerifyGetLastItemRequest request)
        {
            try
            {
                string strUserActorPath = await findUserActor(request.UserID, request.Token);
                if (strUserActorPath == null)
                {
                    Sender.Tell(new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized });
                    return;
                }

                PPGameVerifyDetail      verifyDetailItem = await _dbReaderProxy.Ask<PPGameVerifyDetail>(request, TimeSpan.FromSeconds(15));
                BalanceFromUserResponse userbalance      = await Context.ActorSelection(strUserActorPath).Ask<BalanceFromUserResponse>(new BalanceFromUserRequest(), TimeSpan.FromSeconds(5));
                PPGameVerifySettingItem verifySetting = new PPGameVerifySettingItem();
                verifySetting.displayRoundsEnabled = true;

                PPGameVerifyLastItem verifyLastItem = new PPGameVerifyLastItem();
                verifyLastItem.status           = "SUCCESS";
                verifyLastItem.balance          = userbalance.balance;
                verifyLastItem.currency         = "EUR";
                verifyLastItem.currencySymbol   = "";
                verifyLastItem.settings         = verifySetting;
                if (verifyDetailItem != null)
                {
                    verifyLastItem.rounds = new List<PPGameVerifyDetail>();
                    verifyLastItem.rounds.Add(verifyDetailItem);
                }
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(JsonConvert.SerializeObject(verifyLastItem)) });
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPHistoryWorkActor::onPPVerifyGetLastItemRequest {0}", ex);
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized });
            }
        }
        private async Task onPPHistoryGetItemDetailRequest(HTTPPPHistoryGetItemDetailRequest request)
        {
            try
            {
                string strUserActorPath = await findUserActor(request.UserID, request.Token);
                if (strUserActorPath == null)
                {
                    Sender.Tell(new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized });
                    return;
                }

                List<PPGameRecentHistoryDetailItem> historyItems = await _dbReaderProxy.Ask<List<PPGameRecentHistoryDetailItem>>(request, TimeSpan.FromSeconds(15));
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(JsonConvert.SerializeObject(historyItems))});
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPHistoryWorkActor::onPPHistoryGetLastItemsRequest {0}", ex);
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized });
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
        
        private async Task onBNGTransDetailRequest(BNGTransDetailRequest request)
        {
            try
            {
                BNGTransDetailResponse response = await _dbReaderProxy.Ask<BNGTransDetailResponse>(request, TimeSpan.FromSeconds(15));
                
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(JsonConvert.SerializeObject(response)) });
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPHistoryWorkActor::onBNGTransDetailRequest {0}", ex);
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized });
            }
        }
        private async Task onBNGAggregateRequest(BNGAggregateRequest request)
        {
            try
            {
                BNGAggregateResponse response = await _dbReaderProxy.Ask<BNGAggregateResponse>(request, TimeSpan.FromSeconds(15));
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(JsonConvert.SerializeObject(response)) });
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPHistoryWorkActor::onBNGAggregateRequest {0}", ex);
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized });
            }
        }
        private async Task onBNGTransactionList(BNGTransactionListRequest request)
        {
            try
            {
                string strResponse = await _dbReaderProxy.Ask<string>(request, TimeSpan.FromSeconds(15));
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(strResponse) });
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPHistoryWorkActor::onBNGTransactionList {0}", ex);
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized });
            }
        }
        
        private async Task onCQ9RoundDetailRequest(CQ9RoundDetailRequest request)
        {
            try
            {
                string roundDetail = await _dbReaderProxy.Ask<string>(request, TimeSpan.FromSeconds(30));
                if(string.IsNullOrEmpty(roundDetail))
                {
                    Sender.Tell(new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized });
                    return;
                }

                JObject ret     = new JObject();
                ret["status"]   = new JObject();
                ret["status"]["code"] = "0";
                ret["status"]["datetime"] = DateTime.UtcNow;
                ret["status"]["message"] = "Success";
                ret["data"] = JsonConvert.DeserializeObject<JObject>(roundDetail, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
                string strResponse = JsonConvert.SerializeObject(ret);
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(strResponse) });
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPHistoryWorkActor::onCQ9RoundDetailRequest {0}", ex);
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized });
            }
        }
        private async Task onCQ9SearchRoundIDRequest(CQ9SearchRoundRequest request)
        {
            try
            {
                string strRoundData = await _dbReaderProxy.Ask<string>(request, TimeSpan.FromSeconds(30));

                JObject ret = new JObject();
                ret["error_code"] = 1;
                ret["error_msg"] = "SUCCESS";
                ret["log_id"] = "";
                ret["result"] = new JObject();

                ret["result"]["status"] = new JObject();
                ret["result"]["status"]["code"] = "0";
                ret["result"]["status"]["datetime"] = DateTime.UtcNow;
                ret["result"]["status"]["message"] = "Success";
                ret["result"]["status"]["traceCode"] = "8d0aFbyxbxCm85";
                ret["result"]["data"] = new JObject();

                var list = new JArray();
                if(!string.IsNullOrEmpty(strRoundData))
                    list.Add(JsonConvert.DeserializeObject<JObject>(strRoundData, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None }));

                ret["result"]["data"] = list;
                string strResponse = JsonConvert.SerializeObject(ret);
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(strResponse) });
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPHistoryWorkActor::onCQ9RoundListRequest {0}", ex);
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized });
            }
        }
        private async Task onCQ9RoundListRequest(CQ9RoundListRequest request)
        {
            try
            {
                List<string> items = await _dbReaderProxy.Ask<List<string>>(request, TimeSpan.FromSeconds(30));

                JObject ret = new JObject();
                ret["error_code"] = 1;
                ret["error_msg"] = "SUCCESS";
                ret["log_id"] = "";
                ret["result"] = new JObject();

                ret["result"]["status"] = new JObject();
                ret["result"]["status"]["code"] = "0";
                ret["result"]["status"]["datetime"] = DateTime.UtcNow;
                ret["result"]["status"]["message"] = "Success";
                ret["result"]["status"]["traceCode"] = "8d0aFbyxbxCm85";
                ret["result"]["data"] = new JObject();

                var list = new JArray();
                for (int i = 0; i < items.Count; i++)
                    list.Add(JsonConvert.DeserializeObject<JObject>(items[i], new JsonSerializerSettings { DateParseHandling = DateParseHandling.None }));

                ret["result"]["data"]["list"] = list;

                string strResponse = JsonConvert.SerializeObject(ret);
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(strResponse) });
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPHistoryWorkActor::onCQ9RoundListRequest {0}", ex);
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized });
            }

        }
        
        private async Task onHabaneroGetHistoryRequest(HabaneroGetHistoryRequest request)
        {
            try
            {
                List<string> items = await _dbReaderProxy.Ask<List<string>>(request, TimeSpan.FromSeconds(30));

                var ret = new JObject();
                var list = new JArray();
                for (int i = 0; i < items.Count; i++)
                    list.Add(JsonConvert.DeserializeObject<JObject>(items[i], new JsonSerializerSettings { DateParseHandling = DateParseHandling.None }));

                ret["Games"]    = list;
                ret["Success"]  = true;

                string strResponse = JsonConvert.SerializeObject(ret);
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(strResponse, Encoding.UTF8, "application/json") });
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPHistoryWorkActor::onHabaneroGetHistoryRequest {0}", ex);
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent("{\"Games\":[],\"Success\": true}", Encoding.UTF8, "application/json"), StatusCode = System.Net.HttpStatusCode.Unauthorized });
            }
        }
        private async Task onHabaneroGetGameDetailRequest(HabaneroGetGameDetailRequest request)
        {
            try
            {
                string roundDetail = await _dbReaderProxy.Ask<string>(request, TimeSpan.FromSeconds(30));
                if (string.IsNullOrEmpty(roundDetail))
                {
                    Sender.Tell(new HttpResponseMessage() { Content = new StringContent("{\"d\":{\"GameType\":0,\"DtStarted\":0.0,\"DtCompleted\":0.0,\"RealStake\":0.0,\"RealPayout\":0.0,\"IsCheat\":false}}", Encoding.UTF8, "application/json"), StatusCode = System.Net.HttpStatusCode.Unauthorized });
                    return;
                }

                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(roundDetail, Encoding.UTF8, "application/json") });
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPHistoryWorkActor::onHabaneroGetGameDetailRequest {0}", ex);
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent("{\"d\":{\"GameType\":0,\"DtStarted\":0.0,\"DtCompleted\":0.0,\"RealStake\":0.0,\"RealPayout\":0.0,\"IsCheat\":false}}", Encoding.UTF8, "application/json"), StatusCode = System.Net.HttpStatusCode.Unauthorized });
            }
        }

        private async Task onPlaysonTransactionList(PlaysonTransactionListRequest request)
        {
            try
            {
                string strResponse = await _dbReaderProxy.Ask<string>(request, TimeSpan.FromSeconds(15));
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(strResponse) });
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPHistoryWorkActor::onPlaysonTransactionList {0}", ex);
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized });
            }
        }
        private async Task onPlaysonGetTransRequest(PlaysonGetTransRequest request)
        {
            try
            {
                PlaysonGetTransResponse response = await _dbReaderProxy.Ask<PlaysonGetTransResponse>(request, TimeSpan.FromSeconds(15));
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(JsonConvert.SerializeObject(response)) });
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPHistoryWorkActor::onPlaysonGetTransRequest {0}", ex);
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized });
            }
        }
        private async Task onPlaysonTransDetailRequest(PlaysonTransDetailRequest request)
        {
            try
            {
                PlaysonTransDetailResponse response = await _dbReaderProxy.Ask<PlaysonTransDetailResponse>(request, TimeSpan.FromSeconds(15));

                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(JsonConvert.SerializeObject(response)) });
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPHistoryWorkActor::onPlaysonTransDetailRequest {0}", ex);
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized });
            }
        }
        private async Task onPlaysonAggregateRequest(PlaysonAggregateRequest request)
        {
            try
            {
                PlaysonAggregateResponse response = await _dbReaderProxy.Ask<PlaysonAggregateResponse>(request, TimeSpan.FromSeconds(15));
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(JsonConvert.SerializeObject(response)) });
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPHistoryWorkActor::onPlaysonAggregateRequest {0}", ex);
                Sender.Tell(new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized });
            }
        }
    }
}
