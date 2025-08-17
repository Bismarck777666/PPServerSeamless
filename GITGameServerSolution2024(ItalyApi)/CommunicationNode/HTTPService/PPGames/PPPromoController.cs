using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using System.Net.Http;
using GITProtocol;
using Akka.Actor;
using Newtonsoft.Json;

[assembly: OwinStartup(typeof(CommNode.HTTPService.PPPromoController))]
namespace CommNode.HTTPService
{
    [RoutePrefix("gitapi/pp/promo")]
    public class PPPromoController : ApiController
    {
        [HttpGet]
        [Route("tournament/details")]
        public async Task<HttpResponseMessage> getTournamentDetails()
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
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            string strUserID = findUserIDFromToken(strToken);
            if (strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_PP_PROMOTOURDETAIL);
            try
            {
                ToHTTPSessionMessage response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<ToHTTPSessionMessage>(new FromHTTPSessionMessage(strUserID, strToken, message), TimeSpan.FromSeconds(10.0));
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
            }
            catch (Exception ex)
            {
            }
            return new HttpResponseMessage() { Content = new StringContent("unlogged") };
        }

        [HttpGet]
        [Route("frb/available")]
        public HttpResponseMessage getFrbAvailable()
        {
            string strToken = null;
            foreach (KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
            {
                if (pair.Key == "mgckey")
                    strToken = pair.Value;
            }

            if (strToken == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            if (findUserIDFromToken(strToken) == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            return new HttpResponseMessage() { Content = new StringContent("{\"error\":0,\"description\":\"OK\",\"freeRounds\":[]}") };
        }

        [HttpGet]
        [Route("tournament/scores")]
        public async Task<HttpResponseMessage> getTournamentScores()
        {
            try
            {
                string strToken         = null;
                string strSymbol        = null;
                string strTournamentIDs = null;
                foreach (KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
                {
                    if (pair.Key == "symbol")
                        strSymbol = pair.Value;
                    else if (pair.Key == "mgckey")
                        strToken = pair.Value;
                    else if (pair.Key == "tournamentIDs")
                        strTournamentIDs = pair.Value;
                }

                if (strToken == null || strSymbol == null)
                    return new HttpResponseMessage() { Content = new StringContent("unlogged") };

                string strUserID = findUserIDFromToken(strToken);

                if (strUserID == null)
                    return new HttpResponseMessage() { Content = new StringContent("unlogged") };

                GITMessage gitMessage = new GITMessage((ushort)CSMSG_CODE.CS_PP_PROMOSCORES);
                string[] strParts = strTournamentIDs.Split(new string[1] { "," }, StringSplitOptions.RemoveEmptyEntries);

                gitMessage.Append(strParts.Length);
                for (int i = 0; i < strParts.Length; ++i)
                    gitMessage.Append(int.Parse(strParts[i]));

                ToHTTPSessionMessage response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<ToHTTPSessionMessage>(new FromHTTPSessionMessage(strUserID, strToken, gitMessage), TimeSpan.FromSeconds(10.0));
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
            }
            catch
            {
            }

            return new HttpResponseMessage() { Content = new StringContent("{\"error\":0,\"description\":\"OK\"}") };
        }

        [HttpGet]
        [Route("tournament/v3/leaderboard")]
        public async Task<HttpResponseMessage> getTournamentV3LeaderBoard()
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
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            string strUserID = findUserIDFromToken(strToken);
            if (strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            GITMessage gitMessage = new GITMessage((ushort)CSMSG_CODE.CS_PP_PROMOV3TOURLEADER);
            try
            {
                ToHTTPSessionMessage response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<ToHTTPSessionMessage>((object)new FromHTTPSessionMessage(strUserID, strToken, gitMessage), TimeSpan.FromSeconds(10.0));
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
            }
            catch (Exception ex)
            {
            }

            return new HttpResponseMessage() { Content = new StringContent("unlogged") };
        }

        [HttpGet]
        [Route("v3/tournament/leaderboard")]
        public async Task<HttpResponseMessage> getTournamentV3LeaderBoard2()
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
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            string strUserID = findUserIDFromToken(strToken);
            if (strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_PP_PROMOV3TOURLEADER);
            
            try
            {
                ToHTTPSessionMessage response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<ToHTTPSessionMessage>(new FromHTTPSessionMessage(strUserID, strToken, message), TimeSpan.FromSeconds(10.0));
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
            }
            catch (Exception ex)
            {
            }

            return new HttpResponseMessage() { Content = new StringContent("unlogged") };
        }

        [HttpGet]
        [Route("tournament/leaderboard")]
        public async Task<HttpResponseMessage> getTournamentLeaderBoard()
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
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            string strUserID = findUserIDFromToken(strToken);
            if (strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_PP_PROMOTOURLEADER);
            try
            {
                ToHTTPSessionMessage response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<ToHTTPSessionMessage>(new FromHTTPSessionMessage(strUserID, strToken, message), TimeSpan.FromSeconds(10.0));
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
            }
            catch (Exception ex)
            {
            }

            return new HttpResponseMessage() { Content = new StringContent("unlogged") };
        }

        [HttpGet]
        [Route("race/details")]
        public async Task<HttpResponseMessage> getRaceDetails()
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
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            string strUserID = findUserIDFromToken(strToken);
            if (strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            GITMessage gitMessage = new GITMessage((ushort)CSMSG_CODE.CS_PP_PROMORACEDETAIL);
            try
            {
                ToHTTPSessionMessage response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<ToHTTPSessionMessage>(new FromHTTPSessionMessage(strUserID, strToken, gitMessage), TimeSpan.FromSeconds(10.0));
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
            }
            catch (Exception ex)
            {
            }

            return new HttpResponseMessage() { Content = new StringContent("unlogged") };
        }

        [HttpGet]
        [Route("race/prizes")]
        public async Task<HttpResponseMessage> getRacePrizes()
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
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            string strUserID = findUserIDFromToken(strToken);
            if (strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            GITMessage gitMessage = new GITMessage((ushort)CSMSG_CODE.CS_PP_PROMORACEPRIZES);

            try
            {
                ToHTTPSessionMessage response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<ToHTTPSessionMessage>(new FromHTTPSessionMessage(strUserID, strToken, gitMessage), TimeSpan.FromSeconds(10.0));
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
            }
            catch (Exception ex)
            {
            }

            return new HttpResponseMessage() { Content = new StringContent("unlogged") };
        }

        [HttpPost]
        [Route("race/winners")]
        public async Task<HttpResponseMessage> getRaceWinners(RaceWinnerRequest request)
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
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            if (request == null || request.latestIdentity == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            string strUserID = findUserIDFromToken(strToken);
            if (strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            GITMessage gitMessage = new GITMessage((ushort)CSMSG_CODE.CS_PP_PROMORACEWINNER);
            gitMessage.Append(request.latestIdentity.Count);
            
            foreach (KeyValuePair<string, string> pair in request.latestIdentity)
            {
                gitMessage.Append(int.Parse(pair.Key));
                if (pair.Value == null)
                    gitMessage.Append("");
                else
                    gitMessage.Append(pair.Value);
            }

            try
            {
                ToHTTPSessionMessage response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<ToHTTPSessionMessage>(new FromHTTPSessionMessage(strUserID, strToken, gitMessage), TimeSpan.FromSeconds(10.0));
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
            }
            catch (Exception ex)
            {
            }

            return new HttpResponseMessage() { Content = new StringContent("unlogged") };
        }

        [HttpPost]
        [Route("race/v2/winners")]
        public async Task<HttpResponseMessage> getRaceWinnersV2(RaceWinnerRequest request)
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
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            if (request == null || request.latestIdentity == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            string strUserID = findUserIDFromToken(strToken);
            if (strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            GITMessage gitMessage = new GITMessage((ushort)CSMSG_CODE.CS_PP_PROMOV2RACEWINNER);
            gitMessage.Append(request.latestIdentity.Count);
            
            foreach (KeyValuePair<string, string> pair in request.latestIdentity)
            {
                gitMessage.Append(int.Parse(pair.Key));
                if (pair.Value == null)
                    gitMessage.Append("");
                else
                    gitMessage.Append(pair.Value);
            }

            try
            {
                ToHTTPSessionMessage response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<ToHTTPSessionMessage>(new FromHTTPSessionMessage(strUserID, strToken, gitMessage), TimeSpan.FromSeconds(10.0));
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
            }
            catch (Exception ex)
            {
            }

            return new HttpResponseMessage() { Content = new StringContent("unlogged") };
        }

        [HttpGet]
        [Route("active")]
        public async Task<HttpResponseMessage> getActivePromotions()
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
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            string strUserID = findUserIDFromToken(strToken);
            if (strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            GITMessage gitMessage = new GITMessage((ushort)CSMSG_CODE.CS_PP_PROMOSTART);
            gitMessage.Append(strSymbol);
            
            try
            {
                ToHTTPSessionMessage response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<ToHTTPSessionMessage>(new FromHTTPSessionMessage(strUserID, strToken, gitMessage), TimeSpan.FromSeconds(10.0));
                
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
            }
            catch (Exception ex)
            {
            }

            return new HttpResponseMessage() { Content = new StringContent("unlogged") };
        }

        [HttpPost]
        [Route("tournament/player/choice/OPTIN")]
        public async Task<HttpResponseMessage> postTournamentOptIn()
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
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            string strUserID = findUserIDFromToken(strToken);
            if (strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            GITMessage gitMessage = new GITMessage((ushort)CSMSG_CODE.CS_PP_PROMOTOUROPTIN);

            try
            {
                string strContent = await Request.Content.ReadAsStringAsync();
                PromoIDInfo promoIDInfo = JsonConvert.DeserializeObject<PromoIDInfo>(strContent);
                gitMessage.Append(promoIDInfo.promoID);

                ToHTTPSessionMessage response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<ToHTTPSessionMessage>(new FromHTTPSessionMessage(strUserID, strToken, gitMessage), TimeSpan.FromSeconds(10.0));
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
            }
            catch (Exception ex)
            {
            }

            return new HttpResponseMessage() { Content = new StringContent("unlogged") };
        }

        [HttpPost]
        [Route("race/player/choice/OPTIN")]
        public async Task<HttpResponseMessage> postRaceOptIn()
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
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            string strUserID = findUserIDFromToken(strToken);
            if (strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            GITMessage gitMessage = new GITMessage((ushort)CSMSG_CODE.CS_PP_PROMORACEOPTIN);
            try
            {
                string strContent = await Request.Content.ReadAsStringAsync();
                PromoIDInfo promoIDInfo = JsonConvert.DeserializeObject<PromoIDInfo>(strContent);
                gitMessage.Append(promoIDInfo.promoID);
                ToHTTPSessionMessage response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<ToHTTPSessionMessage>(new FromHTTPSessionMessage(strUserID, strToken, gitMessage), TimeSpan.FromSeconds(10.0));
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
            }
            catch (Exception ex)
            {
            }

            return new HttpResponseMessage() { Content = new StringContent("unlogged") };
        }

        [HttpGet]
        [Route("gs2c/announcements/unread")]
        public HttpResponseMessage unRead()
        {
            return new HttpResponseMessage() { Content = new StringContent("{\"error\":0,\"description\":\"OK\",\"announcements\":[]}") };
        }

        private static string findUserIDFromToken(string strToken)
        {
            string[] strParts = strToken.Split(new string[1] { "@" }, StringSplitOptions.RemoveEmptyEntries);

            if (strParts.Length != 2)
                return null;

            return strParts[0];
        }

        public class RaceWinnerRequest
        {
            public Dictionary<string, string> latestIdentity { get; set; }
        }

        public class PromoIDInfo
        {
            public int promoID { get; set; }
        }
    }
}
