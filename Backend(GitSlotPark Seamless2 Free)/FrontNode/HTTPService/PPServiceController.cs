using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using System.Net.Http;
using FrontNode.HTTPService.Models;
using System.Net;
using Akka.Actor;
using System.Web;
using GITProtocol;
using System.Net.Http.Formatting;
using System.Runtime.Remoting.Contexts;
using StackExchange.Redis;
using FrontNode.Database;

[assembly: OwinStartup(typeof(FrontNode.HTTPService.PPServiceController))]
namespace FrontNode.HTTPService
{
    [RoutePrefix("gitapi/pp/service")]
    public class PPServiceController : ApiController
    {
        [HttpGet]
        [Route("auth.do")]
        public async Task<HttpResponseMessage> doAuth()
        {
            string strAgentID   = null;
            string strUserID    = null;
            string strPassword  = null;
            string strSymbol    = null;
            foreach (KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
            {
                if (pair.Key == "agentid")
                    strAgentID = pair.Value;
                else if (pair.Key == "userid")
                    strUserID = pair.Value;
                else if (pair.Key == "password")
                    strPassword = pair.Value;
                else if (pair.Key == "symbol")
                    strSymbol = pair.Value;
            }
            int agentID = 0;
            if (strAgentID == null || strUserID == null || strPassword == null || !int.TryParse(strAgentID, out agentID) || strSymbol == null)
                return Request.CreateResponse(HttpStatusCode.OK, new AuthResponse("fail", "", "", "", ""), Configuration.Formatters.JsonFormatter);

            string strIPAddress = Request.GetOwinContext().Request.RemoteIpAddress;
            try
            {
                HTTPAuthResponse response = await HTTPServiceConfig.Instance.AuthWorkerGroup.Ask<HTTPAuthResponse>(new HTTPAuthRequest(agentID, strUserID, strPassword, strIPAddress, strSymbol),
                                                TimeSpan.FromSeconds(20));

                if (response.Result == HttpAuthResults.OK)
                    return Request.CreateResponse(HttpStatusCode.OK, new AuthResponse("success", response.SessionToken, response.Currency, response.GameName, response.GameData), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, new AuthResponse("fail", "", "", "", ""), Configuration.Formatters.JsonFormatter);
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
                ToHTTPSessionMessage response = await procMessage(strUserID, strToken, message);
                if (response.Result == ToHTTPSessionMsgResults.OK)
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
            try
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

                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_PP_GETMINILOBBY);
                ToHTTPSessionMessage response = await procMessage(strUserID, strToken, message);
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse) };
                }
            }
            catch
            {
            }
            return new HttpResponseMessage() { Content = new StringContent("unlogged") };
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

            string strToken = dicParams["mgckey"];
            string strUserID = findUserIDFromToken(strToken);
            if (strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };


            GITMessage message = buildSaveSettingMessage(dicParams);
            if(message == null)
                return new HttpResponseMessage() { Content = new StringContent("unlogged") };

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
                {
                    message.Append(int.Parse(dicParams["pur"]));
                    if (dicParams.ContainsKey("ind"))
                        message.Append(int.Parse(dicParams["ind"]));
                }
                else if(dicParams.ContainsKey("fsp"))
                {
                    message.Append(int.Parse(dicParams["fsp"]));
                }
                else if (dicParams.ContainsKey("ind"))   //Added by Foresight(2023.10.06)
                {
                    message.Append(int.Parse(dicParams["ind"]));
                }

                if (dicParams.ContainsKey("fr_play_later") && dicParams["fr_play_later"] == "1")
                    message.FreeSpinPlayLater = true;
                else
                    message.FreeSpinPlayLater = false;
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
                if(dicParams.ContainsKey("ind"))
                    message.Append(int.Parse(dicParams["ind"]));
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
        private GITMessage buildFSBonusMessage(Dictionary<string, string> dicParams)
        {
            try
            {
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_PP_DOFSBONUS);
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
        private GITMessage buildGambleOptionMessage(Dictionary<string, string> dicParams)
        {
            try
            {
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_PP_DOGAMBLEOPTION);
                message.Append(int.Parse(dicParams["index"]));
                message.Append(int.Parse(dicParams["counter"]));
                message.Append(int.Parse(dicParams["g_o_ind"]));
                return message;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private GITMessage buildGambleMessage(Dictionary<string, string> dicParams)
        {
            try
            {
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_PP_DOGAMBLE);
                message.Append(int.Parse(dicParams["index"]));
                message.Append(int.Parse(dicParams["counter"]));
                message.Append(int.Parse(dicParams["g_ind"]));
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
                    case "doFSBonus":
                        requestMessage = buildFSBonusMessage(dicParams);
                        break;
                    case "doGambleOption":
                        requestMessage = buildGambleOptionMessage(dicParams);
                        break;
                    case "doGamble":
                        requestMessage = buildGambleMessage(dicParams);
                        break;
                }

                if (requestMessage == null)
                    return new HttpResponseMessage() { Content = new StringContent("unlogged") };

                //init인 경우 게임입장부터 먼저 진행한다.
                if(dicParams["action"] == "doInit")
                {
                    HTTPEnterGameResults enterGameResult = await procEnterGame(strUserID, dicParams);                    
                    if (enterGameResult != HTTPEnterGameResults.OK)
                    {
                        return new HttpResponseMessage() { Content = new StringContent("unlogged") };
                    }
                }

                ToHTTPSessionMessage response = await procMessage(strUserID, dicParams["mgckey"], requestMessage);
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
        private async Task<HTTPEnterGameResults> procEnterGame(string strUserID, Dictionary<string, string> dicParams)
        {
            HTTPEnterGameResults enterGameResult = HTTPEnterGameResults.INVALIDTOKEN;
            try
            {
                var response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<object>(new HTTPEnterGameRequest(strUserID, dicParams["mgckey"], GameProviders.PP, dicParams["symbol"]), TimeSpan.FromSeconds(10));
                //실패(유저의 불법액션이므로 세션을 종료한다.)
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
            catch
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
