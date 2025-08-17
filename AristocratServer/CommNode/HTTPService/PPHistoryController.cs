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
using CommNode.Database;

[assembly: OwinStartup(typeof(CommNode.HTTPService.PPHistoryController))]
namespace CommNode.HTTPService
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

            if (strToken == null)
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

            if(!strToken.StartsWith("agentgamelogview"))
            {
                if (strSymbol == null)
                    return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

                string strUserID = findUserIDFromToken(strToken);
                if (strUserID == null)
                    return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

                int gameID = DBMonitorSnapshot.Instance.getGameIDFromString(GAMETYPE.PP, strSymbol);
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
            else
            {
                try
                {
                    HttpResponseMessage response = await HTTPServiceConfig.Instance.HistoryWorkerGroup.Ask<HttpResponseMessage>(new HTTPPPHistoryGenralSettingRequest(null, strToken, 0), TimeSpan.FromSeconds(20));
                    return response;
                }
                catch (Exception)
                {
                    return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };
                }
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

            int gameID = DBMonitorSnapshot.Instance.getGameIDFromString(GAMETYPE.PP, strSymbol);
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
        [Route("play-session/by-round")]
        public async Task<HttpResponseMessage> getByRound()
        {
            string strToken     = null;
            string strRoundID   = null;
            foreach (KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
            {
                if (pair.Key == "id")
                    strRoundID = pair.Value;
                else if (pair.Key == "token")
                    strToken = pair.Value;
            }

            if (strToken == null || strRoundID == null)
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

            if (!strToken.StartsWith("agentgamelogview"))
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

            long roundID = 0;
            if (!long.TryParse(strRoundID, out roundID))
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

            string[] strParts = strToken.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            if (strParts.Length != 3)
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

            try
            {
                HttpResponseMessage response = await HTTPServiceConfig.Instance.HistoryWorkerGroup.Ask<HttpResponseMessage>(new HTTPPPHistoryGetByRoundRequest(strParts[1], strToken, roundID, strParts[2]), TimeSpan.FromSeconds(20));
                return response;
            }
            catch (Exception)
            {
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };
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

            if (strToken.StartsWith("agentgamelogview"))
            {
                if (strToken == null || strRoundID == null)
                    return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

                long roundID = 0;
                if (!long.TryParse(strRoundID, out roundID))
                    return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

                string[] strParts = strToken.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                if(strParts.Length != 3)
                    return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

                try
                {
                    HttpResponseMessage response = await HTTPServiceConfig.Instance.HistoryWorkerGroup.Ask<HttpResponseMessage>(new HTTPPPHistoryGetItemDetailRequestByRoundID(strParts[1], strToken, roundID, strParts[2]), TimeSpan.FromSeconds(20));
                    return response;
                }
                catch (Exception)
                {
                    return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };
                }
            }
            else
            {
                if (strToken == null || strSymbol == null || strRoundID == null)
                    return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

                long roundID = 0;
                if (!long.TryParse(strRoundID, out roundID))
                    return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

                string strUserID = findUserIDFromToken(strToken);
                if (strUserID == null)
                    return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

                int gameID = DBMonitorSnapshot.Instance.getGameIDFromString(GAMETYPE.PP, strSymbol);
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
