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

[assembly: OwinStartup(typeof(CommNode.HTTPService.PPReplayController))]
namespace CommNode.HTTPService
{
    [RoutePrefix("gitapi/pp/replay")]

    public class PPReplayController : ApiController
    {
        [HttpGet]
        [Route("list")]
        public async Task<HttpResponseMessage> getReplayList()
        {
            string strToken     = null;
            string strSymbol    = null;
            foreach (KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
            {
                if (pair.Key == "symbol")
                    strSymbol = pair.Value;
                else if (pair.Key == "mgckey")
                    strToken = pair.Value;
            }

            if (strToken == null || strSymbol == null)
                return new HttpResponseMessage() { Content = new StringContent("{\"error\":10,\"description\":\"Request is not authorized\"}") };

            string strUserID = findUserIDFromToken(strToken);
            if (strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent("{\"error\":10,\"description\":\"Request is not authorized\"}") };

            int gameID = DBMonitorSnapshot.Instance.getGameIDFromString(GAMETYPE.PP,strSymbol);
            if (gameID == 0)
                return new HttpResponseMessage() { Content = new StringContent("{\"error\":10,\"description\":\"Request is not authorized\"}")};

            try
            {
                string strResponse = await HTTPServiceConfig.Instance.ReplayWorkerGroup.Ask<string>(new HTTPPPReplayListRequest(strUserID, strToken, gameID), TimeSpan.FromSeconds(20));
                return new HttpResponseMessage() { Content = new StringContent(strResponse) };
            }
            catch (Exception)
            {
                return new HttpResponseMessage() { Content = new StringContent("{\"error\":10,\"description\":\"Request is not authorized\"}") };
            }
        }

        [HttpGet]
        [Route("data")]
        public async Task<HttpResponseMessage> getReplayData()
        {
            string  strToken    = null;
            long    roundID     = 0;
            string  strSymbol   = null;
            foreach (KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
            {
                if (pair.Key == "symbol")
                    strSymbol = pair.Value;
                else if (pair.Key == "token")
                    strToken = pair.Value;
                else if (pair.Key == "roundID")
                    roundID = long.Parse(pair.Value);
            }
            if(strToken == null || roundID == 0 || strSymbol == null)
                return new HttpResponseMessage() { Content = new StringContent("{\"error\":10,\"description\":\"Request is not authorized\"}") };

            string strUserID = null;
            if (!strToken.StartsWith("replay"))
            {
                strUserID = findUserIDFromToken(strToken);
                if (strUserID == null)
                    return new HttpResponseMessage() { Content = new StringContent("{\"error\":10,\"description\":\"Request is not authorized\"}") };
            }

            int gameID = DBMonitorSnapshot.Instance.getGameIDFromString(GAMETYPE.PP, strSymbol);
            if (gameID == 0)
                return new HttpResponseMessage() { Content = new StringContent("{\"error\":10,\"description\":\"Request is not authorized\"}") };

            try
            {
                string strResponse = await HTTPServiceConfig.Instance.ReplayWorkerGroup.Ask<string>(new HTTPPPReplayDataRequest(strUserID, strToken, gameID, roundID), TimeSpan.FromSeconds(20));
                return new HttpResponseMessage() { Content = new StringContent(strResponse) };
            }
            catch (Exception)
            {
                return new HttpResponseMessage() { Content = new StringContent("{\"error\":10,\"description\":\"Request is not authorized\"}") };
            }
        }

        [HttpGet]
        [Route("link")]
        public async Task<HttpResponseMessage> makeReplayLink()
        {
            try
            {
                string  strToken = null;
                long    roundID = 0;
                int     envID = 0;
                string  strSymbol = null;

                foreach (KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
                {
                    if (pair.Key == "symbol")
                        strSymbol = pair.Value;
                    else if (pair.Key == "mgckey")
                        strToken = pair.Value;
                    else if (pair.Key == "roundID")
                        roundID = long.Parse(pair.Value);
                    else if (pair.Key == "envID")
                        envID = int.Parse(pair.Value);
                }

                if (strToken == null || roundID == 0 || strSymbol == null)
                    return new HttpResponseMessage() { Content = new StringContent("{\"error\":10,\"description\":\"Request is not authorized\"}") };

                string strUserID = findUserIDFromToken(strToken);
                if (strUserID == null)
                    return new HttpResponseMessage() { Content = new StringContent("{\"error\":10,\"description\":\"Request is not authorized\"}") };

                int gameID = DBMonitorSnapshot.Instance.getGameIDFromString(GAMETYPE.PP, strSymbol);
                if (gameID == 0)
                    return new HttpResponseMessage() { Content = new StringContent("{\"error\":10,\"description\":\"Request is not authorized\"}") };

                try
                {
                    string strResponse = await HTTPServiceConfig.Instance.ReplayWorkerGroup.Ask<string>(new HTTPPPReplayMakeLinkRequest(strUserID, strToken, gameID, roundID, strSymbol, envID), TimeSpan.FromSeconds(20));
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
                catch (Exception)
                {
                    return new HttpResponseMessage() { Content = new StringContent("{\"error\":10,\"description\":\"Request is not authorized\"}") };
                }
            }
            catch(Exception)
            {
                return new HttpResponseMessage() { Content = new StringContent("{\"error\":10,\"description\":\"Request is not authorized\"}") };
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
