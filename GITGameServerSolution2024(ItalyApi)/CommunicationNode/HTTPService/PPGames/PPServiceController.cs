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

[assembly: OwinStartup(typeof(CommNode.HTTPService.PPServiceController))]
namespace CommNode.HTTPService
{
    [RoutePrefix("gitapi/pp/service")]
    public class PPServiceController : ApiController
    {
        [HttpGet]
        [Route("auth.do")]
        public async Task<HttpResponseMessage> doAuth()
        {
            int     agentID     = 0;
            string  strUserID   = null;
            string  strPassword = null;
            foreach(KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
            {
                if (pair.Key == "agentid")
                    agentID = Convert.ToInt32(pair.Value);
                if (pair.Key == "userid")
                    strUserID = pair.Value;
                else if (pair.Key == "password")
                    strPassword = pair.Value;
            }

            if(agentID == 0 || strUserID == null || strPassword == null)
                return Request.CreateResponse(HttpStatusCode.OK, new AuthResponse("fail", ""), Configuration.Formatters.JsonFormatter);

            string strIPAddress = Request.GetOwinContext().Request.RemoteIpAddress;
            try
            {
                HTTPAuthResponse response = await HTTPServiceConfig.Instance.AuthWorkerGroup.Ask<HTTPAuthResponse>(new HTTPAuthRequest(agentID, strUserID, strPassword, strIPAddress), 
                                                TimeSpan.FromSeconds(10));

                if(response.Result == HttpAuthResults.OK)
                    return Request.CreateResponse(HttpStatusCode.OK, new AuthResponse("success", response.SessionToken), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, new AuthResponse("fail", ""), Configuration.Formatters.JsonFormatter);
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
                if (pair.Key == "mgckey")
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
                ToHTTPSessionMessage response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<ToHTTPSessionMessage>(new FromHTTPSessionMessage(strUserID, strToken, message), TimeSpan.FromSeconds(10));
                if(response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string) response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
            }
            catch(Exception)
            {
            }
            return new HttpResponseMessage() { Content = new StringContent("unlogged") };
        }
        
        [HttpGet]
        [Route("minilobbyGames")]
        public async Task<HttpResponseMessage> getMiniLobbyGames()
        {
            int paramCount = 0;
            string strToken = null;
            foreach (KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
            {
                if (pair.Key == "mgckey")
                    strToken = pair.Value;
                paramCount++;
            }

            if (paramCount != 1 || string.IsNullOrEmpty(strToken))
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            string strUserID = findUserIDFromToken(strToken);
            if (strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            string strResponse = await HTTPServiceConfig.Instance.WorkerGroup.Ask<string>(new HttpGetMiniLobbyGamesMessage(strUserID, strToken), TimeSpan.FromSeconds(10));

            if(strResponse == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            return new HttpResponseMessage() { Content = new StringContent(strResponse) };
        }

        [HttpPost]
        [Route("saveSettings.do")]
        public async Task<HttpResponseMessage> saveSettings(FormDataCollection postData)
        {
            Dictionary<string, string> dicParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in postData)
                dicParams.Add(pair.Key, pair.Value);

            if (!dicParams.ContainsKey("mgckey") || !dicParams.ContainsKey("id")) 
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            string strToken     = dicParams["mgckey"];
            string strUserID    = findUserIDFromToken(strToken);
            if (strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            GITMessage message = buildSaveSettingMessage(dicParams);
            if(message == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

            try
            {
                ToHTTPSessionMessage response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<ToHTTPSessionMessage>(new FromHTTPSessionMessage(strUserID, strToken, message), TimeSpan.FromSeconds(10));
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
        private GITMessage buildSaveSettingMessage(Dictionary<string, string> dicParams)
        {
            try
            {
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_PP_SAVESETTING);
                if (dicParams.ContainsKey("method") && dicParams["method"] == "load")
                {
                    message.Append(true);   //로드
                }
                else
                {
                    message.Append(false);  //보관
                    message.Append(dicParams["settings"]);
                }
                return message;
            }
            catch(Exception)
            {
                return null;
            }
        }
        private GITMessage buildSpinMessage(Dictionary<string, string> dicParams)
        {
            try
            {
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_PP_DOSPIN);
                message.Append(float.Parse(dicParams["c"]));                //BetPerLine
                message.Append(int.Parse(dicParams["l"]));                  //Line

                if (dicParams.ContainsKey("bl"))
                    message.Append(int.Parse(dicParams["bl"]));

                message.Append(int.Parse(dicParams["index"]));
                message.Append(int.Parse(dicParams["counter"]));


                if (dicParams.ContainsKey("pur"))
                    message.Append(int.Parse(dicParams["pur"]));
                else if(dicParams.ContainsKey("fsp"))
                    message.Append(int.Parse(dicParams["fsp"]));
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
                message.Append(int.Parse(dicParams["index"]));
                message.Append(int.Parse(dicParams["counter"]));
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
                return message;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private GITMessage buildFSOptionMessage(Dictionary<string, string> dicParams)
        {
            try
            {
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_PP_FSOPTION);
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
        
        private GITMessage buildCollectBonusMessage(Dictionary<string, string> dicParams)
        {
            try
            {
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_PP_DOCOLLECTBONUS);
                message.Append(int.Parse(dicParams["index"]));
                message.Append(int.Parse(dicParams["counter"]));
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

                if(!dicParams.ContainsKey("mgckey") || !dicParams.ContainsKey("action") || !dicParams.ContainsKey("index") || !dicParams.ContainsKey("counter") || !dicParams.ContainsKey("symbol"))
                    return new HttpResponseMessage() { Content = new StringContent("unlogged") };

                string strUserID = findUserIDFromToken(dicParams["mgckey"]);
                if (strUserID == null)
                    return new HttpResponseMessage() { Content = new StringContent("unlogged") };

                GITMessage requestMessage = null;
                switch(dicParams["action"])
                {
                    case "doSpin":
                        requestMessage = buildSpinMessage(dicParams);
                        break;

                    case "doInit":
                        requestMessage = buildInitMessage(dicParams);
                        break;
                    case "doFSOption":
                        requestMessage = buildFSOptionMessage(dicParams);
                        break;

                    case "doCollect":
                        requestMessage = buildCollectMessage(dicParams);
                        break;

                    case "doCollectBonus":
                        requestMessage = buildCollectBonusMessage(dicParams);
                        break;

                    case "doBonus":
                        requestMessage = buildBonuseMessage(dicParams);
                        break;
                    case "doMysteryScatter":
                        requestMessage = buildMysteryScatterMessage(dicParams);
                        break;

                }

                if (requestMessage == null)
                    return new HttpResponseMessage() { Content = new StringContent("unlogged") };

                //init인 경우 게임입장부터 먼저 진행한다.
                if(dicParams["action"] == "doInit")
                {
                    HTTPEnterGameResults enterGameResult = await HTTPServiceConfig.Instance.WorkerGroup.Ask<HTTPEnterGameResults>(new HTTPEnterGameRequest(strUserID, dicParams["mgckey"], GAMETYPE.PP, dicParams["symbol"]), TimeSpan.FromSeconds(10));
                    if(enterGameResult != HTTPEnterGameResults.OK)
                        return new HttpResponseMessage() { Content = new StringContent("unlogged") };
                }

                ToHTTPSessionMessage response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<ToHTTPSessionMessage>(new FromHTTPSessionMessage(strUserID, dicParams["mgckey"], requestMessage));
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



    }
}
