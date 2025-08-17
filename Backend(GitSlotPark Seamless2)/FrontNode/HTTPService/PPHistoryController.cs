using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using Akka.Actor;
using System.Web.Http;
using Microsoft.Owin;
using System.Net.Http;
using FrontNode.Database;

[assembly: OwinStartup(typeof(FrontNode.HTTPService.PPHistoryController))]
namespace FrontNode.HTTPService
{
    [RoutePrefix("gitapi/pp/history/v2")]
    public class PPHistoryController : ApiController
    {
        [HttpGet]
        [Route("settings/general")]
        public async Task<HttpResponseMessage> getGeneralSettings()
        {
            string strToken = null;
            string strSymbol = null;
            foreach (KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
            {
                if (pair.Key == "symbol")
                    strSymbol = pair.Value;
                else if (pair.Key == "token")
                    strToken = pair.Value;
            }

            if (strToken == null || strSymbol == null)
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

            string strUserID = findUserIDFromToken(strToken);
            if (strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

            //게임아이디유효성검사
            int gameID = DBMonitorSnapshot.Instance.getGameIDFromString(GameProviders.PP, strSymbol);
            if (gameID == 0)
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

            try
            {
                HttpResponseMessage response = await HTTPServiceConfig.Instance.HistoryWorkerGroup.Ask<HttpResponseMessage>(new HTTPPPHistoryGenralSettingRequest(strUserID, strToken, gameID), TimeSpan.FromSeconds(20));
                return response;
            }
            catch (Exception)
            {
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };
            }
        }

        [HttpGet]
        [Route("play-session/last-items")]
        public async Task<HttpResponseMessage> getLastItems()
        {
            string strToken = null;
            string strSymbol = null;
            foreach (KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
            {
                if (pair.Key == "symbol")
                    strSymbol = pair.Value;
                else if (pair.Key == "token")
                    strToken = pair.Value;
            }

            if (strToken == null || strSymbol == null)
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

            string strUserID = findUserIDFromToken(strToken);
            if (strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

            //게임아이디유효성검사
            int gameID = DBMonitorSnapshot.Instance.getGameIDFromString(GameProviders.PP, strSymbol);
            if (gameID == 0)
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

            try
            {
                HttpResponseMessage response = await HTTPServiceConfig.Instance.HistoryWorkerGroup.Ask<HttpResponseMessage>(new HTTPPPHistoryGetLastItemsRequest(strUserID, strToken, gameID), TimeSpan.FromSeconds(20));
                return response;
            }
            catch (Exception)
            {
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };
            }
        }

        [HttpGet]
        [Route("action/children")]
        public async Task<HttpResponseMessage> getItemDetail ()
        {
            string strToken     = null;
            string strSymbol    = null;
            string strRoundID   = null;
            foreach (KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
            {
                if (pair.Key == "symbol")
                    strSymbol = pair.Value;
                else if (pair.Key == "token")
                    strToken = pair.Value;
                else if (pair.Key == "id")
                    strRoundID = pair.Value;
            }

            if (strToken == null || strSymbol == null || strRoundID == null)
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

            long roundID = 0;
            if(!long.TryParse(strRoundID, out roundID))
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

            string strUserID = findUserIDFromToken(strToken);
            if (strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

            //게임아이디유효성검사
            int gameID = DBMonitorSnapshot.Instance.getGameIDFromString(GameProviders.PP, strSymbol);
            if (gameID == 0)
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

            try
            {
                HttpResponseMessage response = await HTTPServiceConfig.Instance.HistoryWorkerGroup.Ask<HttpResponseMessage>(new HTTPPPHistoryGetItemDetailRequest(strUserID, strToken, gameID, roundID), TimeSpan.FromSeconds(20));
                return response;
            }
            catch (Exception)
            {
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };
            }
        }

        private static string findUserIDFromToken(string strToken)
        {
            string[] strParts = strToken.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
            if (strParts.Length != 2)
                return null;

            return strParts[0];
        }       
    }
}
