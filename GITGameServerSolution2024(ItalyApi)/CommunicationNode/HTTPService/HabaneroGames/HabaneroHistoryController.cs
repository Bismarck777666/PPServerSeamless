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

[assembly: OwinStartup(typeof(CommNode.HTTPService.HabaneroHistoryController))]
namespace CommNode.HTTPService
{
    [RoutePrefix("gitapi/habanero/history")]
    public class HabaneroHistoryController : ApiController
    {
        [HttpPost]
        [Route("GetHistory")]
        public async Task<HttpResponseMessage> doGetHistory()
        {
            try
            {
                string strContent = await Request.Content.ReadAsStringAsync();
                HabaneroHistoryRequest request = JsonConvert.DeserializeObject<HabaneroHistoryRequest>(strContent);

                string[] strParts = request.playerid.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
                if (strParts.Length != 2)
                    return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

                string strGlobalUserID = strParts[0];
                DateTime beginTime;
                DateTime endTime;

                beginTime   = new DateTime(Convert.ToInt32(request.dtStartUtc.Substring(0, 4)), Convert.ToInt32(request.dtStartUtc.Substring(4, 2)), Convert.ToInt32(request.dtStartUtc.Substring(6, 2)), Convert.ToInt32(request.dtStartUtc.Substring(8, 2)), Convert.ToInt32(request.dtStartUtc.Substring(10, 2)), Convert.ToInt32(request.dtStartUtc.Substring(12, 2)));
                endTime     = new DateTime(Convert.ToInt32(request.dtEndUtc.Substring(0, 4)), Convert.ToInt32(request.dtEndUtc.Substring(4, 2)), Convert.ToInt32(request.dtEndUtc.Substring(6, 2)), Convert.ToInt32(request.dtEndUtc.Substring(8, 2)), Convert.ToInt32(request.dtEndUtc.Substring(10, 2)), Convert.ToInt32(request.dtEndUtc.Substring(12, 2)));
                //beginTime   = beginTime.AddMinutes(-request.tz);
                //endTime     = endTime.AddMinutes(-request.tz);

                HttpResponseMessage response = await HTTPServiceConfig.Instance.HistoryWorkerGroup.Ask<HttpResponseMessage>(new HabaneroGetHistoryRequest(strGlobalUserID, beginTime, endTime, 1),
                    TimeSpan.FromSeconds(30));
                return response;
            }
            catch(Exception ex)
            {
                return new HttpResponseMessage() { Content = new StringContent("{\"Games\":[],\"Success\": true}", Encoding.UTF8, "application/json") };
            }
            
        }

        [HttpPost]
        [Route("GetGameDetails")]
        public async Task<HttpResponseMessage> doGetGameDetail()
        {
            try
            {
                string strContent = await Request.Content.ReadAsStringAsync();
                HabaneroDetailRequest request = JsonConvert.DeserializeObject<HabaneroDetailRequest>(strContent);

                HttpResponseMessage response = await HTTPServiceConfig.Instance.HistoryWorkerGroup.Ask<HttpResponseMessage>(new HabaneroGetGameDetailRequest(request.gameInstanceId), TimeSpan.FromSeconds(20));
                return response;
            }
            catch (Exception)
            {
                return new HttpResponseMessage() { Content = new StringContent("")};
            }
        }

    }
    public class HabaneroHistoryRequest
    {
        public string   playerid    { get; set; }
        public string   brandId     { get; set; }
        public string   dtStartUtc  { get; set; }
        public string   dtEndUtc    { get; set; }
    }
    public class HabaneroDetailRequest
    {
        public string gameInstanceId    { get; set; }
 	    public string mode              { get; set; }
	    public string showUsername      { get; set; }
    }
}
    
