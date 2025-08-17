using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using System.Net.Http.Formatting;
using System.Web.Http;
using Microsoft.Owin;
using System.Net.Http;
using CommNode.HTTPService.Models;
using Newtonsoft.Json;
using System.Net;
using Akka.Actor;

[assembly: OwinStartup(typeof(CommNode.HTTPService.BNGHistoryController))]
namespace CommNode.HTTPService
{
    [RoutePrefix("gitapi/bng/history")]
    public class BNGHistoryController : ApiController
    {
        [HttpPost]
        [Route("v1/game/list/")]
        public HttpResponseMessage doGameList()
        {            
            return new HttpResponseMessage() { Content = new StringContent("[]", Encoding.UTF8, "application/json") };
        }
        [HttpPost]
        [Route("v1/transaction/list/")]
        public async Task<HttpResponseMessage> doTransactionList()
        {
            try
            {
                string strContent = await Request.Content.ReadAsStringAsync();
                BNGTransactionListRequest request = JsonConvert.DeserializeObject<BNGTransactionListRequest>(strContent);

                HttpResponseMessage response = await HTTPServiceConfig.Instance.HistoryWorkerGroup.Ask<HttpResponseMessage>(request, TimeSpan.FromSeconds(20));
                return response;
            }
            catch (Exception)
            {
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };
            }
        }
        [HttpPost]
        [Route("v1/playergame/aggregate/")]
        public async Task<HttpResponseMessage> doAggregate()
        {
            try
            {
                string strContent = await Request.Content.ReadAsStringAsync();
                BNGAggregateRequest request = JsonConvert.DeserializeObject<BNGAggregateRequest>(strContent);

                HttpResponseMessage response = await HTTPServiceConfig.Instance.HistoryWorkerGroup.Ask<HttpResponseMessage>(request, TimeSpan.FromSeconds(20));
                return response;
            }
            catch (Exception)
            {
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };
            }
        }

        [HttpGet]
        [Route("v1/transaction/getdetail")]
        public async Task<HttpResponseMessage> doGetDetail(string transactionid)
        {
            try
            {
                HttpResponseMessage response = await HTTPServiceConfig.Instance.HistoryWorkerGroup.Ask<HttpResponseMessage>(new BNGTransDetailRequest(transactionid), TimeSpan.FromSeconds(20));
                return response;
            }
            catch (Exception)
            {
                return new HttpResponseMessage() { Content = new StringContent("")};
            }
        }

    }
}
    
