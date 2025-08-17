using Akka.Actor;
using FrontNode.Database;
using GITProtocol;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace FrontNode.HTTPService
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

            //게임아이디유효성검사
            int gameID = DBMonitorSnapshot.Instance.getGameIDFromString(GameProviders.PP,strSymbol);
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

            string  strUserID       = null;
            if (!strToken.StartsWith("replay"))
            {
                strUserID = findUserIDFromToken(strToken);
                if (strUserID == null)
                    return new HttpResponseMessage() { Content = new StringContent("{\"error\":10,\"description\":\"Request is not authorized\"}") };
            }

            int gameID = DBMonitorSnapshot.Instance.getGameIDFromString(GameProviders.PP, strSymbol);
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
                string  strToken    = null;
                long    roundID     = 0;
                int     envID       = 0;
                string  strSymbol   = null;
                string  strLang     = null;
                string  strCurrency = null;

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
                    else if (pair.Key == "lang")
                        strLang = pair.Value;
                    else if (pair.Key == "currency")
                        strCurrency = pair.Value;
                }

                if (strToken == null || roundID == 0 || strSymbol == null || strLang == null || strCurrency == null)
                    return new HttpResponseMessage() { Content = new StringContent("{\"error\":10,\"description\":\"Request is not authorized\"}") };

                string strUserID = findUserIDFromToken(strToken);
                if (strUserID == null)
                    return new HttpResponseMessage() { Content = new StringContent("{\"error\":10,\"description\":\"Request is not authorized\"}") };

                int gameID = DBMonitorSnapshot.Instance.getGameIDFromString(GameProviders.PP, strSymbol);
                if (gameID == 0)
                    return new HttpResponseMessage() { Content = new StringContent("{\"error\":10,\"description\":\"Request is not authorized\"}") };

                try
                {
                    string strResponse = await HTTPServiceConfig.Instance.ReplayWorkerGroup.Ask<string>(new HTTPPPReplayMakeLinkRequest(strUserID, strToken, gameID, roundID, strSymbol, envID,  strLang), TimeSpan.FromSeconds(20));
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
