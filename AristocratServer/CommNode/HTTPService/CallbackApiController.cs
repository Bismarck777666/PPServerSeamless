using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using System.Net.Http;
using System.Net;
using Akka.Actor;
using System.Web;
using GITProtocol;
using System.Net.Http.Formatting;
using Akka.Routing;

[assembly: OwinStartup(typeof(CommNode.HTTPService.CallbackApiController))]
namespace CommNode.HTTPService
{
    public class CallbackApiController : ApiController
    {
        [HttpPost]
        [Route("GetBalance")]
        public async Task<HttpResponseMessage> doGetBalance(CallbackGetBalanceRequest request)
        {            
            try
            {
                CallbackGetBalanceResponse response = await HTTPServiceConfig.Instance.CallbackWorkActor.Ask<CallbackGetBalanceResponse>(request, TimeSpan.FromSeconds(10));
                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, new CallbackGetBalanceResponse(1, "General Error", 0.0M), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [Route("Withdraw")]
        public async Task<HttpResponseMessage> doWithdraw(CallbackWithdrawRequest request)
        {
            try
            {
                CallbackWithdrawResponse response = await HTTPServiceConfig.Instance.CallbackWorkActor.Ask<CallbackWithdrawResponse>(request, TimeSpan.FromSeconds(10));
                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.Unauthorized, new CallbackWithdrawResponse(1, "General Error"), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [Route("Deposit")]
        public async Task<HttpResponseMessage> doDeposit(CallbackDepositRequest request)
        {
            try
            {
                CallbackDepositResponse response = await HTTPServiceConfig.Instance.CallbackWorkActor.Ask<CallbackDepositResponse>(request, TimeSpan.FromSeconds(10));
                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.Unauthorized, new CallbackDepositResponse(1, "General Error"), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [Route("RollbackTransaction")]
        public async Task<HttpResponseMessage> doRollback(CallbackRollbackRequest request)
        {
            try
            {
                CallbackRollbackResponse response = await HTTPServiceConfig.Instance.CallbackWorkActor.Ask<CallbackRollbackResponse>(request, TimeSpan.FromSeconds(10));
                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.Unauthorized, new CallbackRollbackResponse(1, "General Error"), Configuration.Formatters.JsonFormatter);
        }
    }

    public class CallbackGetBalanceRequest : IConsistentHashable
    {
        public string agentID   { get; set; }
        public string sign      { get; set; }
        public string userID    { get; set; }
        public int    gameID    { get; set; }

        public object ConsistentHashKey
        {
            get
            {
                return string.Format("{0}_{1}", agentID, userID);
            }
        }
    }
    public class CallbackGetBalanceResponse
    {
        public int      code    { get; set; }
        public string   message { get; set; }
        public decimal  balance { get; set; }

        public CallbackGetBalanceResponse()
        {

        }
        public CallbackGetBalanceResponse(int code, string message, decimal balance)
        {
            this.code       = code;
            this.message    = message;
            this.balance    = balance;
        }
    }

    public class CallbackWithdrawRequest : IConsistentHashable
    {
        public string   agentID         { get; set; }
        public string   sign            { get; set; }
        public string   userID          { get; set; }
        public decimal  amount          { get; set;}
        public string   transactionID   { get; set; }
        public string   roundID         { get; set; }
        public int      gameID          { get; set; }

        public object ConsistentHashKey
        {
            get
            {
                return string.Format("{0}_{1}", agentID, userID);
            }
        }
    }
    public class CallbackWithdrawResponse
    {
        public int      code                    { get; set; }
        public string   message                 { get; set; }
        public string   platformTransactionID   { get; set; }
        public decimal  balance                 { get; set; }

        public CallbackWithdrawResponse()
        {

        }
        public CallbackWithdrawResponse(int code, string message)
        {
            this.code = code;
            this.message = message;
        }
    }

    public class CallbackDepositRequest : IConsistentHashable
    {
        public string   agentID             { get; set; }
        public string   sign                { get; set; }
        public string   userID              { get; set; }
        public decimal  amount              { get; set; }
        public string   transactionID       { get; set; }
        public string   refTransactionID    { get; set; }
        public string   roundID             { get; set; }
        public int      gameID              { get; set; }

        public object ConsistentHashKey
        {
            get
            {
                return string.Format("{0}_{1}", agentID, userID);
            }
        }
    }
    public class CallbackDepositResponse
    {
        public int      code                    { get; set; }
        public string   message                 { get; set; }
        public string   platformTransactionID   { get; set; }
        public decimal  balance                 { get; set; }

        public CallbackDepositResponse()
        {

        }
        public CallbackDepositResponse(int code, string message)
        {
            this.code = code;
            this.message = message;
        }
    }

    public class CallbackRollbackRequest : IConsistentHashable
    {
        public string agentID { get; set; }
        public string sign { get; set; }
        public string userID { get; set; }
        public string refTransactionID { get; set; }
        public int gameID { get; set; }

        public object ConsistentHashKey
        {
            get
            {
                return string.Format("{0}_{1}", agentID, userID);
            }
        }
    }
    public class CallbackRollbackResponse
    {
        public int code { get; set; }
        public string message { get; set; }

        public CallbackRollbackResponse()
        {

        }
        public CallbackRollbackResponse(int code, string message)
        {
            this.code = code;
            this.message = message;
        }
    }
}
