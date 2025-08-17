using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net;
using Akka.Actor;
using System.IO;
using System.Security.Cryptography;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Akka.Util.Internal;
using Topshelf.Hosts;
using QueenApiNode.Database;
using System.ServiceModel.Channels;

[assembly: OwinStartup(typeof(QueenApiNode.HttpService.QueenApiController))]
namespace QueenApiNode.HttpService
{
    public class QueenApiController : ApiController
    {
        /*
        [HttpGet]
        [Route("getAgentBalance")]
        public async Task<HttpResponseMessage> doGetAgentBalance()
        {
            try
            {
                if(!Request.Headers.Contains("Authorization"))
                    return Request.CreateResponse(HttpStatusCode.OK, new QueenResponse(ResponseCodes.InvalidToken, "invalid auth token"), Configuration.Formatters.JsonFormatter);

                string authToken        = Request.Headers.GetValues("Authorization").First();
                string response         = await ApiConfig.WorkActorGroup.Ask<string>(new ApiConsistentRequest(authToken, new GetAgentBalanceRequest()), TimeSpan.FromSeconds(10.0));
                var responseMessage     =  Request.CreateResponse(HttpStatusCode.OK);
                responseMessage.Content = new StringContent(response, Encoding.UTF8, "application/json");
                return responseMessage;
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new QueenResponse(ResponseCodes.GeneralError, "general error"), Configuration.Formatters.JsonFormatter);
            }
        }
        */

        [HttpPost]
        [Route("user/create")]
        public async Task<HttpResponseMessage> doUserCreate()
        {
            try
            {
                var owinContext = Request.GetOwinContext();
                var ipAddress   = owinContext.Get<string>("ClientIpAddress");

                if (!Request.Headers.Contains("Authorization"))
                    return Request.CreateResponse(HttpStatusCode.OK, new QueenResponse(ResponseCodes.InvalidToken, "invalid auth token"), Configuration.Formatters.JsonFormatter);

                string              authToken   = Request.Headers.GetValues("Authorization").First();
                string              strContent  = await Request.Content.ReadAsStringAsync();
                UserCreateRequest   request     = JsonConvert.DeserializeObject<UserCreateRequest>(strContent);
                string              response    = await ApiConfig.WorkActorGroup.Ask<string>(new ApiConsistentRequest(authToken, ipAddress, request), TimeSpan.FromSeconds(10.0));
                var responseMessage = Request.CreateResponse(HttpStatusCode.OK);
                responseMessage.Content = new StringContent(response, Encoding.UTF8, "application/json");
                return responseMessage;
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new QueenResponse(ResponseCodes.GeneralError, "general error"), Configuration.Formatters.JsonFormatter);
            }
        }

        [HttpPost]
        [Route("user/balance")]
        public async Task<HttpResponseMessage> doUserGetBalance()
        {
            try
            {
                var owinContext = Request.GetOwinContext();
                var ipAddress   = owinContext.Get<string>("ClientIpAddress");

                if (!Request.Headers.Contains("Authorization"))
                    return Request.CreateResponse(HttpStatusCode.OK, new QueenResponse(ResponseCodes.InvalidToken, "invalid auth token"), Configuration.Formatters.JsonFormatter);

                string authToken                = Request.Headers.GetValues("Authorization").First();
                string strContent               = await Request.Content.ReadAsStringAsync();
                UserGetBalanceRequest request   = JsonConvert.DeserializeObject<UserGetBalanceRequest>(strContent);
                string response = await ApiConfig.WorkActorGroup.Ask<string>(new ApiConsistentRequest(authToken, ipAddress, request), TimeSpan.FromSeconds(10.0));
                var responseMessage = Request.CreateResponse(HttpStatusCode.OK);
                responseMessage.Content = new StringContent(response, Encoding.UTF8, "application/json");
                return responseMessage;
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new QueenResponse(ResponseCodes.GeneralError, "general error"), Configuration.Formatters.JsonFormatter);
            }

        }

        [HttpGet]
        [Route("game/list")]
        public async Task<HttpResponseMessage> doGetGameList()
        {
            try
            {
                var owinContext = Request.GetOwinContext();
                var ipAddress   = owinContext.Get<string>("ClientIpAddress");

                if (!Request.Headers.Contains("Authorization"))
                    return Request.CreateResponse(HttpStatusCode.OK, new QueenResponse(ResponseCodes.InvalidToken, "invalid auth token"), Configuration.Formatters.JsonFormatter);

                string authToken    = Request.Headers.GetValues("Authorization").First();
                string response     = await ApiConfig.WorkActorGroup.Ask<string>(new ApiConsistentRequest(authToken, ipAddress, new GetGameListRequest()), TimeSpan.FromSeconds(10.0));
                var responseMessage = Request.CreateResponse(HttpStatusCode.OK);
                responseMessage.Content = new StringContent(response, Encoding.UTF8, "application/json");
                return responseMessage;
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new QueenResponse(ResponseCodes.GeneralError, "general error"), Configuration.Formatters.JsonFormatter);
            }
        }

        [HttpPost]
        [Route("user/deposit")]
        public async Task<HttpResponseMessage> doUserDeposit()
        {
            try
            {
                var owinContext = Request.GetOwinContext();
                var ipAddress   = owinContext.Get<string>("ClientIpAddress");

                if (!Request.Headers.Contains("Authorization"))
                    return Request.CreateResponse(HttpStatusCode.OK, new QueenResponse(ResponseCodes.InvalidToken, "invalid auth token"), Configuration.Formatters.JsonFormatter);

                string authToken = Request.Headers.GetValues("Authorization").First();
                string strContent = await Request.Content.ReadAsStringAsync();

                UserDepositRequest  request     = JsonConvert.DeserializeObject<UserDepositRequest>(strContent);
                string response     = await ApiConfig.WorkActorGroup.Ask<string>(new ApiConsistentRequest(authToken, ipAddress, request), TimeSpan.FromSeconds(10.0));
                var responseMessage = Request.CreateResponse(HttpStatusCode.OK);
                responseMessage.Content = new StringContent(response, Encoding.UTF8, "application/json");
                return responseMessage;
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new QueenResponse(ResponseCodes.GeneralError, "general error"), Configuration.Formatters.JsonFormatter);
            }
        }
        [HttpPost]
        [Route("user/withdraw")]
        public async Task<HttpResponseMessage> doUserWithdraw()
        {
            try
            {
                var owinContext = Request.GetOwinContext();
                var ipAddress   = owinContext.Get<string>("ClientIpAddress");

                if (!Request.Headers.Contains("Authorization"))
                    return Request.CreateResponse(HttpStatusCode.OK, new QueenResponse(ResponseCodes.InvalidToken, "invalid auth token"), Configuration.Formatters.JsonFormatter);

                string authToken = Request.Headers.GetValues("Authorization").First();
                string strContent = await Request.Content.ReadAsStringAsync();

                UserWithdrawRequest request     = JsonConvert.DeserializeObject<UserWithdrawRequest>(strContent);
                string response     = await ApiConfig.WorkActorGroup.Ask<string>(new ApiConsistentRequest(authToken,ipAddress, request), TimeSpan.FromSeconds(10.0));
                var responseMessage = Request.CreateResponse(HttpStatusCode.OK);
                responseMessage.Content = new StringContent(response, Encoding.UTF8, "application/json");
                return responseMessage;
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new QueenResponse(ResponseCodes.GeneralError, "general error"), Configuration.Formatters.JsonFormatter);
            }
        }
        [HttpPost]
        [Route("game/start")]
        public async Task<HttpResponseMessage> doGetGameURL()
        {
            try
            {
                var owinContext = Request.GetOwinContext();
                var ipAddress   = owinContext.Get<string>("ClientIpAddress");

                if (!Request.Headers.Contains("Authorization"))
                    return Request.CreateResponse(HttpStatusCode.OK, new QueenResponse(ResponseCodes.InvalidToken, "invalid auth token"), Configuration.Formatters.JsonFormatter);

                string authToken = Request.Headers.GetValues("Authorization").First();
                string strContent = await Request.Content.ReadAsStringAsync();

                GetGameURLRequest   request     = JsonConvert.DeserializeObject<GetGameURLRequest>(strContent);
                string response     = await ApiConfig.WorkActorGroup.Ask<string>(new ApiConsistentRequest(authToken, ipAddress, request), TimeSpan.FromSeconds(10.0));
                var responseMessage = Request.CreateResponse(HttpStatusCode.OK);
                responseMessage.Content = new StringContent(response, Encoding.UTF8, "application/json");
                return responseMessage;
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new QueenResponse(ResponseCodes.GeneralError, "general error"), Configuration.Formatters.JsonFormatter);
            }
        }
        [HttpPost]
        [Route("game/log")]
        public async Task<HttpResponseMessage> doGetBetHistory()
        {
            try
            {
                var owinContext = Request.GetOwinContext();
                var ipAddress   = owinContext.Get<string>("ClientIpAddress");

                if (!Request.Headers.Contains("Authorization"))
                    return Request.CreateResponse(HttpStatusCode.OK, new QueenResponse(ResponseCodes.InvalidToken, "invalid auth token"), Configuration.Formatters.JsonFormatter);

                string authToken = Request.Headers.GetValues("Authorization").First();
                string strContent = await Request.Content.ReadAsStringAsync();

                GetBetHistoryRequest request  = JsonConvert.DeserializeObject<GetBetHistoryRequest>(strContent);
                string               response = await ApiConfig.WorkActorGroup.Ask<string>(new ApiConsistentRequest(authToken, ipAddress, request), TimeSpan.FromSeconds(10.0));
                var responseMessage = Request.CreateResponse(HttpStatusCode.OK);
                responseMessage.Content = new StringContent(response, Encoding.UTF8, "application/json");
                return responseMessage;
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new QueenResponse(ResponseCodes.GeneralError, "general error"), Configuration.Formatters.JsonFormatter);
            }
        }

        [HttpPost]
        [Route("adminapi/agent/deposit")]
        public async Task<HttpResponseMessage> doAgentDeposit()
        {
            try
            {
                var owinContext = Request.GetOwinContext();
                var ipAddress   = owinContext.Get<string>("ClientIpAddress");

                if (!Request.Headers.Contains("Authorization"))
                    return Request.CreateResponse(HttpStatusCode.OK, new QueenResponse(ResponseCodes.InvalidToken, "invalid auth token"), Configuration.Formatters.JsonFormatter);

                string authToken  = Request.Headers.GetValues("Authorization").First();
                string strContent = await Request.Content.ReadAsStringAsync();

                AgentDepositRequest request = JsonConvert.DeserializeObject<AgentDepositRequest>(strContent);
                string response             = await ApiConfig.WorkActorGroup.Ask<string>(new ApiConsistentRequest(authToken, ipAddress, request), TimeSpan.FromSeconds(10.0));
                var responseMessage         = Request.CreateResponse(HttpStatusCode.OK);
                responseMessage.Content     = new StringContent(response, Encoding.UTF8, "application/json");
                return responseMessage;
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new QueenResponse(ResponseCodes.GeneralError, "general error"), Configuration.Formatters.JsonFormatter);
            }
        }
        [HttpPost]
        [Route("adminapi/agent/withdraw")]
        public async Task<HttpResponseMessage> doAgentWithdraw()
        {
            try
            {
                var owinContext = Request.GetOwinContext();
                var ipAddress   = owinContext.Get<string>("ClientIpAddress");

                if (!Request.Headers.Contains("Authorization"))
                    return Request.CreateResponse(HttpStatusCode.OK, new QueenResponse(ResponseCodes.InvalidToken, "invalid auth token"), Configuration.Formatters.JsonFormatter);

                string authToken = Request.Headers.GetValues("Authorization").First();
                string strContent = await Request.Content.ReadAsStringAsync();

                AgentWithdrawRequest request = JsonConvert.DeserializeObject<AgentWithdrawRequest>(strContent);
                string response              = await ApiConfig.WorkActorGroup.Ask<string>(new ApiConsistentRequest(authToken, ipAddress, request), TimeSpan.FromSeconds(10.0));
                var responseMessage          = Request.CreateResponse(HttpStatusCode.OK);
                responseMessage.Content      = new StringContent(response, Encoding.UTF8, "application/json");
                return responseMessage;
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new QueenResponse(ResponseCodes.GeneralError, "general error"), Configuration.Formatters.JsonFormatter);
            }
        }
    }
    public enum ResponseCodes
    {
        OK                              = 0,
        InvalidToken                    = 101,
        DuplicateUserID                 = 102,
        UserIDMismatchRule              = 103,
        InvalidUserID                   = 104,
        InvalidVendorID                 = 105,
        InvalidGameID                   = 106,
        NotEnoughAgentBalance           = 107,
        InvalidObjectID                 = 108,
        AgentDisabled                   = 109,
        RateLimited                     = 110,
        InvalidCategory                 = 111,
        InvalidAmount                   = 112,
        GeneralError                    = 130,
    }
    public class QueenResponse
    {
        [JsonProperty(Order = 1)]
        public int      code    { get; set; }

        [JsonProperty(Order = 2)]
        public string   message { get; set; }

        public QueenResponse()
        {

        }
        public QueenResponse(ResponseCodes code, string message)
        {
            this.code       = (int) code;
            this.message    = message;
        }
    }
    public class GetAgentBalanceRequest
    {
    }
    public class UserCreateRequest
    {
        public string userid { get; set; }
    }
    public class UserGetBalanceRequest
    {
        public string userid { get; set; }
    }
    public class GetBalanceResponse : QueenResponse
    {
        [JsonProperty(Order = 3)]
        public double balance { get; set; }
        public GetBalanceResponse()
        {

        }
        public GetBalanceResponse(double balance) : base(ResponseCodes.OK, "OK")
        {
            this.balance = balance;
        }
    }
    public class UserWithdrawResponse : QueenResponse
    {
        [JsonProperty(Order = 4)]
        public double balance           { get; set; }
        [JsonProperty(Order = 3)]
        public double withdrawAmount { get; set; }
        public UserWithdrawResponse()
        {

        }
        public UserWithdrawResponse(double withdrawAmount, double balance) : base(ResponseCodes.OK, "OK")
        {
            this.withdrawAmount = withdrawAmount;
            this.balance        = balance;
        }
    }
    public class GetVendorResponse : QueenResponse
    {
        public GetVendorResponse()
        {

        }
        public GetVendorResponse(List<ApiProvider> data) : base(ResponseCodes.OK, "OK")
        {
            this.data = data;
        }
        [JsonProperty(Order = 3)]
        public List<ApiProvider> data { get; set; }

    }
    public class GetGameListResponse : QueenResponse
    {
        [JsonProperty(Order = 3)]
        public List<ApiGame> data { get; set; }

        public GetGameListResponse()
        {

        }
        public GetGameListResponse(List<ApiGame> data) : base(ResponseCodes.OK, "OK")
        {
            this.data = data;
        }
    }
    public class GetGameURLResponse : QueenResponse
    {
        [JsonProperty(Order = 3)]
        public string url { get; set; }

        public GetGameURLResponse()
        {

        }
        public GetGameURLResponse(string url) : base(ResponseCodes.OK, "OK")
        {
            this.url = url;
        }
    }
    public class GetGameListRequest
    {
    }
    public class UserDepositRequest
    {
        public string userid { get; set; }
        public double amount { get; set; }
    }
    public class UserWithdrawRequest
    {
        public string userid { get; set; }
        public double amount { get; set; }  
        public UserWithdrawRequest()
        {
            this.amount = -1.0;
        }
    }
    public class GetGameURLRequest
    {
        public string   userid    { get; set; }
        public int      gameid    { get; set; }
        public string   lang      { get; set; }
        public GetGameURLRequest()
        {
            gameid = -1;
        }
    }
    public class GetBetHistoryRequest
    {
        public int  logcount        { get; set; }
        public long lastobjectid    { get; set; }
        public string startdate { get; set; }
        public string enddate { get; set; }
    }
    public class GetBetHistoryResponse : QueenResponse
    {
        [JsonProperty(Order = 3)]
        public long totalCount      { get; set; }
        [JsonProperty(Order = 4)]
        public long lastObjectID    { get; set; }
        [JsonProperty(Order = 5)]
        public List<BetHistoryItem> data { get; set; }

        public GetBetHistoryResponse()
        {

        }
        public GetBetHistoryResponse(long totalCount, long lastObjectID, List<BetHistoryItem> items):
            base(ResponseCodes.OK, "OK")
        {
            this.totalCount     = totalCount;
            this.lastObjectID   = lastObjectID;
            this.data           = items;
        }    
    }
    public class BetHistoryItem
    {
        public long     objectID            { get; set; }
        public string   userid              { get; set; }        
        public string   gameid              { get; set; }
        public double   betAmount           { get; set; }
        public double   winAmount           { get; set; }
        public string   roundid             { get; set; }
        public bool?  isBuyFeature         { get; set; }
        public bool  isFinished           { get; set; }
        public string   transactionTime     { get; set; }
    }
    public class AgentDepositRequest
    {
        public string agentid { get; set; }
        public double amount  { get; set; }
    }
    public class AgentWithdrawRequest
    {
        public string agentid { get; set; }
        public double amount  { get; set; }
    }
    
}

