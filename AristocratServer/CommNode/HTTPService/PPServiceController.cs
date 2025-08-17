using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using System.Net.Http;
using CommNode.HTTPService.Models;
using System.Net;
using Akka.Actor;
using System.Web;
using GITProtocol;
using System.Net.Http.Formatting;
using System.Runtime.Remoting.Contexts;
using System.IO;
using System.Security.Cryptography;
using System.Web;

[assembly: OwinStartup(typeof(CommNode.HTTPService.PPServiceController))]
namespace CommNode.HTTPService
{
    [RoutePrefix("gitapi/aristo/service")]
    public class PPServiceController : ApiController
    {
        [HttpGet]
        [Route("auth.do")]
        public async Task<HttpResponseMessage> doAuth()
        {
            string strUserID     = null;
            string strPassword   = null;
            string strIPAddress  = null;
            string strGameSymbol = null;

            foreach(KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
            {
                if (pair.Key == "userid")
                    strUserID = pair.Value;
                else if (pair.Key == "password")
                    strPassword = pair.Value;
                else if (pair.Key == "ip")
                    strIPAddress = pair.Value;
                else if (pair.Key == "gamesymbol")
                    strGameSymbol = pair.Value;
            }

            if(strUserID == null || strPassword == null)
                return Request.CreateResponse(HttpStatusCode.OK, new AuthResponse("fail", "", 0, "0"), Configuration.Formatters.JsonFormatter);

            if (string.IsNullOrEmpty(strIPAddress))
                strIPAddress = "";
            try
            {
                HTTPAuthResponse response = await HTTPServiceConfig.Instance.AuthWorkerGroup.Ask<HTTPAuthResponse>(new HTTPAuthRequest(strUserID, strPassword, strIPAddress, strGameSymbol), 
                                                TimeSpan.FromSeconds(10));

                if(response.Result == HttpAuthResults.OK)
                    return Request.CreateResponse(HttpStatusCode.OK, new AuthResponse("success", response.SessionToken, response.Currency, response.GameData), Configuration.Formatters.JsonFormatter);
                else if(response.Result == HttpAuthResults.COUNTRYMISMATCH)
                    return Request.CreateResponse(HttpStatusCode.OK, new AuthResponse("countrymismatch", "", 0, "0"), Configuration.Formatters.JsonFormatter);
                else if(response.Result == HttpAuthResults.SERVERMAINTENANCE)
                    return Request.CreateResponse(HttpStatusCode.OK, new AuthResponse("maintenance", "", 0, "0"), Configuration.Formatters.JsonFormatter);

            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, new AuthResponse("fail", "", 0, "0"), Configuration.Formatters.JsonFormatter);
        }

        [HttpGet]
        [Route("withdraw.do.begin")]
        public async Task<HttpResponseMessage> doWithdrawBegin()
        {
            string strUserID = null;
            string strPassword = null;
            foreach (KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
            {
                if (pair.Key == "userid")
                    strUserID = pair.Value;
                else if (pair.Key == "password")
                    strPassword = pair.Value;
            }

            if (strUserID == null || strPassword == null)
                return new HttpResponseMessage() { Content = new StringContent("3") };

            try
            {
                WithdrawResults result = await HTTPServiceConfig.Instance.AuthWorkerGroup.Ask<WithdrawResults>(new WithdrawRequest(strUserID, strPassword, true),
                                                TimeSpan.FromSeconds(10));

                return new HttpResponseMessage() { Content = new StringContent(string.Format("{0}", (int) result)) };
            }
            catch (Exception)
            {
            }
            return new HttpResponseMessage() { Content = new StringContent(string.Format("{0}", (int) WithdrawResults.OTHERERROR)) };
        }

        [HttpGet]
        [Route("withdraw.do.end")]
        public async Task<HttpResponseMessage> doWithdrawEnd()
        {
            string strUserID = null;
            string strPassword = null;
            foreach (KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
            {
                if (pair.Key == "userid")
                    strUserID = pair.Value;
                else if (pair.Key == "password")
                    strPassword = pair.Value;
            }

            if (strUserID == null || strPassword == null)
                return new HttpResponseMessage() { Content = new StringContent("3") };

            try
            {
                WithdrawResults result = await HTTPServiceConfig.Instance.AuthWorkerGroup.Ask<WithdrawResults>(new WithdrawRequest(strUserID, strPassword, false),
                                                TimeSpan.FromSeconds(10));

                return new HttpResponseMessage() { Content = new StringContent(string.Format("{0}", (int)result)) };
            }
            catch (Exception)
            {
            }
            return new HttpResponseMessage() { Content = new StringContent(string.Format("{0}", (int)WithdrawResults.OTHERERROR)) };
        }

        [HttpPost]
        [Route("stats.do")]
        public HttpResponseMessage doStats()
        {
            string strToken = null;
            foreach (KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
            {
                if (pair.Key == "mgckey")
                    strToken = pair.Value;
            }

            if (string.IsNullOrEmpty(strToken))
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            return Request.CreateResponse(HttpStatusCode.OK, new StatsResponse(0, "OK"), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [Route("logout.do")]
        public HttpResponseMessage doLogout()
        {
            string strToken = null;
            foreach (KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
            {
                if (pair.Key == "mgckey")
                    strToken = pair.Value;
            }

            if (string.IsNullOrEmpty(strToken))
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            string strUserID = findUserIDFromToken(strToken);
            if (strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            HTTPServiceConfig.Instance.WorkerGroup.Tell(new HttpLogoutRequest(strUserID, strToken));
            return new HttpResponseMessage() { Content = new StringContent("OK") };

        }
        [HttpGet]
        [Route("reloadBalance.do")]
        public async Task<HttpResponseMessage> reloadBalance()
        {
            int     paramCount  = 0;
            string  strToken    = null;
            foreach(KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
            {
                if (pair.Key == "session")
                    strToken = pair.Value;
                paramCount++;
            }

            if(paramCount != 1 || string.IsNullOrEmpty(strToken))
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            string strUserID = findUserIDFromToken(strToken);
            if(strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_PP_RELOADBALANCE);
            try
            {
                ToHTTPSessionMessage response = await procMessage(strUserID, strToken, message);
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

        private string findUserIDFromToken(string strToken)
        {
            string[] strParts = strToken.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
            if (strParts.Length != 2)
                return null;

            return strParts[0];
        }
        private GITMessage buildSpinMessage(Dictionary<string, string> dicParams)
        {
            try
            {
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_PP_DOSPIN);
                message.Append(float.Parse(dicParams["bet"]));                //line 5
                message.Append(int.Parse(dicParams["lines"]));                //BetPerLine
                message.Append(float.Parse(dicParams["denomination"]));      //Denomination 0.01
                if (dicParams.ContainsKey("pick")) message.Append(int.Parse(dicParams["pick"]));
                return message;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private GITMessage buildInitMessage(Dictionary<string, string> dicParams)
        {
            try
            {
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_PP_DOINIT);
                return message;
            }
            catch (Exception)
            {
                return null;
            }
        }

        
        private GITMessage buildMysteryScatterMessage(Dictionary<string, string> dicParams)
        {
            try
            {
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_PP_DOMYSTERYSCATTER);
                message.Append(int.Parse(dicParams["index"]));
                message.Append(int.Parse(dicParams["counter"]));
                if (dicParams.ContainsKey("ind"))
                    message.Append(int.Parse(dicParams["ind"]));
                return message;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private GITMessage buildBonuseMessage(Dictionary<string, string> dicParams)
        {
            try
            {
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_PP_DOBONUS);
                message.Append(dicParams["color"].ToString());

                return message;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        private GITMessage buildCollectBonusMessage(Dictionary<string, string> dicParams)
        {
            try
            {
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_PP_DOCOLLECTBONUS);
                return message;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private GITMessage buildCollectMessage(Dictionary<string, string> dicParams)
        {
            try
            {
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_PP_DOCOLLECT);
                message.Append(int.Parse(dicParams["index"]));
                message.Append(int.Parse(dicParams["counter"]));
                return message;
            }
            catch (Exception)
            {
                return null;
            }
        }
        [HttpPost]
        [Route("gameService")]
        public async Task<HttpResponseMessage> gameService(FormDataCollection postData)
        {
            try
            {
                Dictionary<string, string> dicParams = new Dictionary<string, string>();
                foreach (KeyValuePair<string, string> pair in postData)
                    dicParams.Add(pair.Key, pair.Value);

                if(!dicParams.ContainsKey("session") || !dicParams.ContainsKey("action") || !dicParams.ContainsKey("gameid"))
                    return new HttpResponseMessage() { Content = new StringContent("unlogged") };

                string strUserID = findUserIDFromToken(dicParams["session"]);
                if (strUserID == null)
                    return new HttpResponseMessage() { Content = new StringContent("unlogged") };

                GITMessage requestMessage = null;
                switch(dicParams["action"])
                {
                    case "gameSpin":
                    case "gamePick":
                        requestMessage = buildSpinMessage(dicParams);
                        break;

                    case "gameInit":
                        requestMessage = buildInitMessage(dicParams);
                        break;

                    case "doCollect":
                        requestMessage = buildCollectMessage(dicParams);
                        break;

                    case "gameTakeWin":
                        requestMessage = buildCollectBonusMessage(dicParams);
                        break;

                    case "gameGamble":
                        requestMessage = buildBonuseMessage(dicParams);
                        break;
                    case "doMysteryScatter":
                        requestMessage = buildMysteryScatterMessage(dicParams);
                        break;

                }

                if (requestMessage == null)
                    return new HttpResponseMessage() { Content = new StringContent("unlogged") };

                if(dicParams["action"] == "gameInit")
                {
                    HTTPEnterGameResults enterGameResult = await procEnterGame(strUserID, dicParams);
                    if (enterGameResult != HTTPEnterGameResults.OK)
                        return new HttpResponseMessage() { Content = new StringContent("unlogged") };
                }

                ToHTTPSessionMessage response = await procMessage(strUserID, dicParams["session"], requestMessage);
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

        private string getIpAddress(HttpRequestMessage request)
        {
            try
            {
                // Try to get the client IP address from the CF-Connecting-IP header
                string ipAddress = getHeaderValue(request, "CF-Connecting-IP");

                if (string.IsNullOrEmpty(ipAddress))
                {
                    // If CF-Connecting-IP is not present, try X-Forwarded-For
                    ipAddress = getHeaderValue(request, "X-Forwarded-For");
                }

                if (string.IsNullOrEmpty(ipAddress))
                {
                    // If CF-Connecting-IP is not present, try X-Forwarded-For
                    ipAddress = getHeaderValue(request, "X-FORWARDED-FOR");
                }

                if (string.IsNullOrEmpty(ipAddress))
                {
                    // If neither header is present, fall back to the remote IP address
                    ipAddress = request.GetOwinContext().Request.RemoteIpAddress;
                }

                return ExpandIPAddress(ipAddress);
            }
            catch (Exception)
            {

                return "";
            }
            
        }
        private string getHeaderValue(HttpRequestMessage request, string headerName)
        {
            //if (request.Headers.TryGetValues(headerName, out var headerValues))
            //{
            //    // Get the first value of the header (if there are multiple values)
            //    string headerValue = headerValues.FirstOrDefault(null);
            //    return headerValue;
            //}
            //else
            //{
                return null;
            //}
        }

        private string ExpandIPAddress(string ipAddress)
        {
            if (!IsValidIPv6(ipAddress)) return ipAddress;
            // Parse the compressed IPv6 address to an IPAddress object
            IPAddress ip = IPAddress.Parse(ipAddress);

            // Convert the address bytes to a full, expanded format
            byte[] addressBytes = ip.GetAddressBytes();
            string fullAddress = "";

            for (int i = 0; i < addressBytes.Length; i += 2)
            {
                // Combine two bytes into a 16-bit number and format as a 4-digit hex number
                ushort segment = (ushort)((addressBytes[i] << 8) + addressBytes[i + 1]);
                fullAddress += segment.ToString("x4");

                if (i < addressBytes.Length - 2)
                {
                    fullAddress += ":";
                }
            }

            return fullAddress;
        }
        private bool IsValidIPv6(string address)
        {
            return IPAddress.TryParse(address, out IPAddress ipAddress) && ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6;
        }
        //private string getIpAddress(HttpRequestMessage request)
        //{
        //    try
        //    {
        //        var values = request.Headers.GetValues("CF-CONNECTING-IP");
        //        if (values != null && values.Count() > 0)
        //            return values.ElementAt(0);
        //    }
        //    catch
        //    {
            
        //    }
        //    return "";
        //}
        private async Task<HTTPEnterGameResults> procEnterGame(string strUserID, Dictionary<string, string> dicParams)
        {
            HTTPEnterGameResults enterGameResult = HTTPEnterGameResults.INVALIDTOKEN;
            try
            {
                string strIPAddress = getIpAddress(Request);
                var response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<object>(new HTTPEnterGameRequest(strUserID, dicParams["session"], GAMETYPE.ARISTO, dicParams["gameid"], strIPAddress), TimeSpan.FromSeconds(30));
                
                if (response is string)
                {
                    enterGameResult = HTTPEnterGameResults.INVALIDACTION;
                }
                else
                {
                    GITMessage responseMessage = (response as SendMessageToUser).Message;
                    byte result = (byte)responseMessage.Pop();
                    if (result == 0)
                        enterGameResult = HTTPEnterGameResults.OK;
                    else
                        enterGameResult = HTTPEnterGameResults.INVALIDGAMEID;
                }
            }
            catch(Exception ex)
            {
                enterGameResult = HTTPEnterGameResults.INVALIDGAMEID;
            }
            return enterGameResult;
        }
        private async Task<ToHTTPSessionMessage> procMessage(string strUserID, string strSessionToken, GITMessage requestMessage)
        {
            try
            {
                var response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<object>(new FromHTTPSessionMessage(strUserID, strSessionToken, requestMessage), TimeSpan.FromSeconds(25.0));
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


    }
}
