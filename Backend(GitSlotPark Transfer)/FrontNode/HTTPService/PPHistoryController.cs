using Akka.Actor;
using FrontNode.Database;
using GITProtocol;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace FrontNode.HTTPService
{
    [RoutePrefix("gitapi/pp/history/v2")]
    public class PPHistoryController : ApiController
    {
        [HttpGet]
        [Route("settings/general")]
        public async Task<HttpResponseMessage> getGeneralSettings()
        {
            string strToken     = null;
            string strSymbol    = null;
            foreach (KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
            {
                if (pair.Key == "symbol")
                    strSymbol = pair.Value;
                else if (pair.Key == "token")
                    strToken = pair.Value;
            }

            if (strToken == null || strSymbol == null)
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = HttpStatusCode.Unauthorized };
            
            string strUserID = findUserIDFromToken(strToken);
            if (strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = HttpStatusCode.Unauthorized };
            
            int gameID = DBMonitorSnapshot.Instance.getGameIDFromString(GameProviders.PP, strSymbol);
            if (gameID == 0)
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = HttpStatusCode.Unauthorized };
            
            try
            {
                HttpResponseMessage response = await HTTPServiceConfig.Instance.HistoryWorkerGroup.Ask<HttpResponseMessage>(new HTTPPPHistoryGenralSettingRequest(strUserID, strToken, gameID), TimeSpan.FromSeconds(20.0));
                return response;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = HttpStatusCode.Unauthorized };
            }
        }

        [HttpGet]
        [Route("play-session/last-items")]
        public async Task<HttpResponseMessage> getLastItems()
        {
            string strToken     = null;
            string strSymbol    = null;
            foreach (KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
            {
                if (pair.Key == "symbol")
                    strSymbol = pair.Value;
                else if (pair.Key == "token")
                    strToken = pair.Value;
            }

            if (strToken == null || strSymbol == null)
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = HttpStatusCode.Unauthorized };
            
            string strUserID = findUserIDFromToken(strToken);
            if (strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = HttpStatusCode.Unauthorized };

            int gameID = DBMonitorSnapshot.Instance.getGameIDFromString(GameProviders.PP, strSymbol);
            if (gameID == 0)
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = HttpStatusCode.Unauthorized };

            try
            {
                HttpResponseMessage response = await HTTPServiceConfig.Instance.HistoryWorkerGroup.Ask<HttpResponseMessage>(new HTTPPPHistoryGetLastItemsRequest(strUserID, strToken, gameID), TimeSpan.FromSeconds(20.0));
                return response;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = HttpStatusCode.Unauthorized };
            }
        }

        [HttpGet]
        [Route("action/children")]
        public async Task<HttpResponseMessage> getItemDetail()
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
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = HttpStatusCode.Unauthorized };
            
            long roundID = 0;
            if (!long.TryParse(strRoundID, out roundID))
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = HttpStatusCode.Unauthorized };
            string strUserID = findUserIDFromToken(strToken);
            if (strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = HttpStatusCode.Unauthorized };

            int gameID = DBMonitorSnapshot.Instance.getGameIDFromString(GameProviders.PP, strSymbol);
            if (gameID == 0)
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = HttpStatusCode.Unauthorized };

            try
            {
                HttpResponseMessage response = await HTTPServiceConfig.Instance.HistoryWorkerGroup.Ask<HttpResponseMessage>(new HTTPPPHistoryGetItemDetailRequest(strUserID, strToken, gameID, roundID), TimeSpan.FromSeconds(20.0));
                return response;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = HttpStatusCode.Unauthorized };
            }
        }

        private static string findUserIDFromToken(string strToken)
        {
            string[] strArray = strToken.Split(new string[1]{"@"}, StringSplitOptions.RemoveEmptyEntries);
            return strArray.Length != 2 ? null : strArray[0];
        }
    }
}
