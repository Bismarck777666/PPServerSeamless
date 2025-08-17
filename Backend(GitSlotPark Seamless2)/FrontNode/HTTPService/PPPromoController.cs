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

[assembly: OwinStartup(typeof(FrontNode.HTTPService.PPPromoController))]
namespace FrontNode.HTTPService
{
    public class PPPromoController : ApiController
    {
        [HttpGet]
        [Route("gitapi/pp/promo/tournament/details")]
        public async Task<HttpResponseMessage> getTournamentDetails()
        {
            string strToken = null;
            string strSymbol = null;
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

            GITMessage gitMessage = new GITMessage((ushort)CSMSG_CODE.CS_PP_PROMOTOURDETAIL);
            try
            {
                ToHTTPSessionMessage response = await procMessage(strUserID, strToken, gitMessage);
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
            }
            catch (Exception)
            {

            }
            return new HttpResponseMessage() { Content = new StringContent("unlogged") };
        }

        [HttpGet]
        [Route("gitapi/pp/promo/frb/available")]
        public HttpResponseMessage getFrbAvailable()
        {
            string strToken  = null;
            foreach (KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
            {
                if (pair.Key == "mgckey")
                    strToken = pair.Value;
            }
            if (strToken == null )
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };
            string strUserID = findUserIDFromToken(strToken);
            if (strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };
            return new HttpResponseMessage() { Content = new StringContent("{\"error\":0,\"description\":\"OK\",\"freeRounds\":[]}") };
        }

        [HttpGet]
        [Route("gitapi/pp/promo/tournament/scores")]
        public async Task<HttpResponseMessage> getTournamentScores()
        {
            try
            {
                string strToken = null;
                string strSymbol = null;
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

                GITMessage  gitMessage  = new GITMessage((ushort)CSMSG_CODE.CS_PP_PROMOSCORES);
                string[]    strParts    = strTournamentIDs.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                gitMessage.Append(strParts.Length);
                for (int i = 0; i < strParts.Length; i++)
                    gitMessage.Append(int.Parse(strParts[i]));

                ToHTTPSessionMessage response = await procMessage(strUserID, strToken, gitMessage);

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
        [Route("gitapi/pp/promo/tournament/v3/leaderboard")]
        public async Task<HttpResponseMessage> getTournamentV3LeaderBoard()
        {
            string strToken         = null;
            string strSymbol        = null;

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
                ToHTTPSessionMessage response = await procMessage(strUserID, strToken, gitMessage);
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
            }
            catch (Exception)
            {

            }
            return new HttpResponseMessage() { Content = new StringContent("unlogged") };
        }

        [HttpGet]
        [Route("gitapi/pp/promo/v3/tournament/leaderboard")]
        public async Task<HttpResponseMessage> getTournamentV3LeaderBoard2()
        {
            string strToken = null;
            string strSymbol = null;

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
                ToHTTPSessionMessage response = await procMessage(strUserID, strToken, gitMessage);
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
            }
            catch (Exception)
            {

            }
            return new HttpResponseMessage() { Content = new StringContent("unlogged") };
        }

        [HttpGet]
        [Route("gitapi/pp/promo/tournament/leaderboard")]
        public async Task<HttpResponseMessage> getTournamentLeaderBoard()
        {
            string strToken = null;
            string strSymbol = null;

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

            GITMessage gitMessage = new GITMessage((ushort)CSMSG_CODE.CS_PP_PROMOTOURLEADER);
            try
            {
                ToHTTPSessionMessage response = await procMessage(strUserID, strToken, gitMessage);
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
            }
            catch (Exception)
            {

            }
            return new HttpResponseMessage() { Content = new StringContent("unlogged") };
        }
        [HttpGet]
        [Route("gitapi/pp/promo/race/details")]
        public async Task<HttpResponseMessage> getRaceDetails()
        {
            string strToken = null;
            string strSymbol = null;
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
                ToHTTPSessionMessage response = await procMessage(strUserID, strToken, gitMessage);
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
            }
            catch (Exception)
            {

            }
            return new HttpResponseMessage() { Content = new StringContent("unlogged") };
        }

        [HttpGet]
        [Route("gitapi/pp/promo/race/prizes")]
        public async Task<HttpResponseMessage> getRacePrizes()
        {
            string strToken = null;
            string strSymbol = null;
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
                ToHTTPSessionMessage response = await procMessage(strUserID, strToken, gitMessage);
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
            }
            catch (Exception)
            {

            }
            return new HttpResponseMessage() { Content = new StringContent("unlogged") };
        }

        [HttpPost]
        [Route("gitapi/pp/promo/race/winners")]
        public async Task<HttpResponseMessage> getRaceWinners(RaceWinnerRequest request)
        {
            string strToken = null;
            string strSymbol = null;
            foreach (KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
            {
                if (pair.Key == "symbol")
                    strSymbol = pair.Value;
                else if (pair.Key == "mgckey")
                    strToken = pair.Value;
            }

            if (strToken == null || strSymbol == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };
            
            if(request == null || request.latestIdentity == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };


            string strUserID = findUserIDFromToken(strToken);
            if (strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            GITMessage gitMessage = new GITMessage((ushort)CSMSG_CODE.CS_PP_PROMORACEWINNER);
            gitMessage.Append(request.latestIdentity.Count);
            foreach(KeyValuePair<string, string> pair in request.latestIdentity)
            {
                gitMessage.Append(int.Parse(pair.Key));
                if (pair.Value == null)
                    gitMessage.Append("");
                else
                    gitMessage.Append(pair.Value);
            }

            try
            {
                ToHTTPSessionMessage response = await procMessage(strUserID, strToken, gitMessage);
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
            }
            catch (Exception)
            {

            }
            return new HttpResponseMessage() { Content = new StringContent("unlogged") };
        }

        [HttpPost]
        [Route("gitapi/pp/promo/race/v2/winners")]
        public async Task<HttpResponseMessage> getRaceWinnersV2(RaceWinnerRequest request)
        {
            string strToken = null;
            string strSymbol = null;
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
                ToHTTPSessionMessage response = await procMessage(strUserID, strToken, gitMessage);
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
            }
            catch (Exception)
            {

            }
            return new HttpResponseMessage() { Content = new StringContent("unlogged") };
        }
        [HttpGet]
        [Route("gitapi/pp/promo/active")]
        public async Task<HttpResponseMessage> getActivePromotions()
        {
            string strToken = null;
            string strSymbol  = null;
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

            GITMessage gitMessage = new GITMessage((ushort)CSMSG_CODE.CS_PP_PROMOACTIVE);
            gitMessage.Append(strSymbol);
            try
            {
                ToHTTPSessionMessage response = await procMessage(strUserID, strToken, gitMessage);
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
            }
            catch (Exception)
            {

            }
            return new HttpResponseMessage() { Content = new StringContent("unlogged") };
        }

        [HttpPost]
        [Route("gitapi/pp/promo/tournament/player/choice/OPTIN")]
        public async Task<HttpResponseMessage> postTournamentOptIn()
        {
            string strToken = null;
            string strSymbol = null;
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
                ToHTTPSessionMessage response = await procMessage(strUserID, strToken, gitMessage);
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
            }
            catch (Exception)
            {

            }
            return new HttpResponseMessage() { Content = new StringContent("unlogged") };
        }

        [HttpPost]
        [Route("gitapi/pp/promo/race/player/choice/OPTIN")]
        public async Task<HttpResponseMessage> postRaceOptIn()
        {
            string strToken = null;
            string strSymbol = null;
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

                ToHTTPSessionMessage response = await procMessage(strUserID, strToken, gitMessage);
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
            }
            catch (Exception)
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
        private async Task<ToHTTPSessionMessage> procMessage(string strUserID, string strSessionToken, GITMessage requestMessage)
        {
            try
            {
                var response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<object>(new FromHTTPSessionMessage(strUserID, strSessionToken, requestMessage), TimeSpan.FromSeconds(12.0));
                if (response is string)
                {
                    return new ToHTTPSessionMessage(ToHTTPSessionMsgResults.INVALIDTOKEN);
                }
                else
                {
                    GITMessage message = (response as SendMessageToUser).Message;
                    return new ToHTTPSessionMessage((response as SendMessageToUser).Message);
                }
            }
            catch
            {
                return new ToHTTPSessionMessage(ToHTTPSessionMsgResults.INVALIDTOKEN);
            }
        }

        private static string findUserIDFromToken(string strToken)
        {
            string[] strParts = strToken.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
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
