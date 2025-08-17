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

[assembly: OwinStartup(typeof(CommNode.HTTPService.CQ9HistoryController))]

namespace CommNode.HTTPService
{
    [RoutePrefix("gitapi/cq9/history")]
    public class CQ9HistoryController : ApiController
    {
        [HttpGet]
        [Route("player_betting/search_time")]
        public async Task<HttpResponseMessage> doRoundList(string token, string begin, string end, int offset, int count)
        {
            try
            {
                string[] strParts = token.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
                if(strParts.Length != 2)
                    return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

                string strGlobalUserID = strParts[0];
                DateTime beginTime;
                DateTime endTime;

                if(!DateTime.TryParse(begin, out beginTime) || !DateTime.TryParse(end, out endTime))
                    return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

                beginTime   = beginTime.ToUniversalTime();
                endTime     = endTime.ToUniversalTime();
                HttpResponseMessage response = await HTTPServiceConfig.Instance.HistoryWorkerGroup.Ask<HttpResponseMessage>(new CQ9RoundListRequest(strGlobalUserID, beginTime, endTime, offset, count),
                    TimeSpan.FromSeconds(30));
                return response;
            }
            catch (Exception)
            {
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };
            }
        }

        [HttpGet]
        [Route("player_betting/search_id")]
        public async Task<HttpResponseMessage> doSearchRound(string token, string id)
        {
            try
            {
                string[] strParts = token.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
                if (strParts.Length != 2 || string.IsNullOrEmpty(id))
                    return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

                string strUserID = strParts[0];            
                HttpResponseMessage response = await HTTPServiceConfig.Instance.HistoryWorkerGroup.Ask<HttpResponseMessage>(new CQ9SearchRoundRequest(strUserID, id),
                    TimeSpan.FromSeconds(30));
                return response;
            }
            catch (Exception)
            {
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };
            }
        }


        private string findString(string strData, string startPattern, string endPattern)
        {
            int startPos = strData.IndexOf(startPattern);
            if (startPos < 0)
                return null;
            startPos += startPattern.Length;
            int endPos = strData.IndexOf(endPattern, startPos);
            if (endPos < 0)
                return null;
            return strData.Substring(startPos, endPos - startPos);
        }
        [HttpGet]
        [Route("inquire/db/wager")]
        public async Task<HttpResponseMessage> doRoundDetail(string token, string id)
        {
            try
            {

                string[] strParts = token.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
                if(strParts == null || strParts.Length != 2)
                    return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };
                string strUserID  = strParts[0]; 
                string strRoundID = id;
                if(strRoundID == null)
                    return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

                HttpResponseMessage response = await HTTPServiceConfig.Instance.HistoryWorkerGroup.Ask<HttpResponseMessage>(new CQ9RoundDetailRequest(strUserID, strRoundID),TimeSpan.FromSeconds(30));
                return response;
            }
            catch (Exception)
            {
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };
            }
        }
    }
}
