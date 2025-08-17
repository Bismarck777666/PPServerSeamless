using Akka.Actor;
using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace CommNode.HTTPService
{
    [RoutePrefix("gitapi/playson/history")]
    public class PlaysonHistoryController : ApiController
    {
        [HttpPost]
        [Route("game/list/")]
        public HttpResponseMessage doGameList()
        {
            return new HttpResponseMessage() { Content = new StringContent("[]", Encoding.UTF8, "application/json") };
        }

        [HttpPost]
        [Route("transaction/list/")]
        public async Task<HttpResponseMessage> doTransactionList()
        {
            try
            {
                string strContent = await Request.Content.ReadAsStringAsync();
                PlaysonTransactionListRequest request = JsonConvert.DeserializeObject<PlaysonTransactionListRequest>(strContent);

                HttpResponseMessage response = await HTTPServiceConfig.Instance.HistoryWorkerGroup.Ask<HttpResponseMessage>(request, TimeSpan.FromSeconds(20));
                return response;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };
            }
        }
        [HttpPost]
        [Route("playergame/aggregate/")]
        public async Task<HttpResponseMessage> doAggregate()
        {
            try
            {
                string strContent = await Request.Content.ReadAsStringAsync();
                PlaysonAggregateRequest request = JsonConvert.DeserializeObject<PlaysonAggregateRequest>(strContent);

                HttpResponseMessage response = await HTTPServiceConfig.Instance.HistoryWorkerGroup.Ask<HttpResponseMessage>(request, TimeSpan.FromSeconds(20));
                return response;
            }
            catch (Exception)
            {
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };
            }
        }

        [HttpGet]
        [Route("transaction/getroundid")]
        public async Task<HttpResponseMessage> doGetRoundId(string transactionid)
        {
            try
            {
                HttpResponseMessage response = await HTTPServiceConfig.Instance.HistoryWorkerGroup.Ask<HttpResponseMessage>(new PlaysonGetTransRequest(transactionid), TimeSpan.FromSeconds(20));
                return response;
            }
            catch (Exception)
            {
                return new HttpResponseMessage() { Content = new StringContent("") };
            }
        }

        [HttpGet]
        [Route("session-wlmsgid")]
        public async Task<HttpResponseMessage> doGetDetail(string site_message_id)
        {
            try
            {
                HttpResponseMessage response = await HTTPServiceConfig.Instance.HistoryWorkerGroup.Ask<HttpResponseMessage>(new PlaysonTransDetailRequest(site_message_id), TimeSpan.FromSeconds(20));
                return response;
            }
            catch (Exception)
            {
                return new HttpResponseMessage() { Content = new StringContent("") };
            }
        }

        [HttpGet]
        [Route("build")]
        public HttpResponseMessage doBuild()
        {
            return new HttpResponseMessage() { Content = new StringContent("{\"status\":\"ok\",\"totalcount\":7,\"data\":\"x.0.9.2\"}") };
        }
    }
}
