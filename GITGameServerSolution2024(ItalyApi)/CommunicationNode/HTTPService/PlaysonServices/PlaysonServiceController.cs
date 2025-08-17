using Akka.Actor;
using CommNode.HTTPService.Models;
using GITProtocol;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;

[assembly: OwinStartup(typeof(CommNode.HTTPService.PlaysonServiceController))]
namespace CommNode.HTTPService
{
    [RoutePrefix("gitapi/playson/service")]
    public class PlaysonServiceController : ApiController
    {
        [HttpGet]
        [Route("auth.do")]
        public async Task<HttpResponseMessage> doAuth()
        {
            int     agentID     = 0;
            string  strUserID   = null;
            string  strPassword = null;
            foreach (KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
            {
                if (pair.Key == "agentid")
                    agentID = Convert.ToInt32(pair.Value);
                if (pair.Key == "userid")
                    strUserID = pair.Value;
                else if (pair.Key == "password")
                    strPassword = pair.Value;
            }

            if (strUserID == null || strPassword == null)
                return Request.CreateResponse(HttpStatusCode.OK, new AuthResponse("fail", ""), Configuration.Formatters.JsonFormatter);

            string strIPAddress = Request.GetOwinContext().Request.RemoteIpAddress;
            try
            {
                HTTPAuthResponse response = await HTTPServiceConfig.Instance.AuthWorkerGroup.Ask<HTTPAuthResponse>(new HTTPAuthRequest(agentID, strUserID, strPassword, strIPAddress),
                                                TimeSpan.FromSeconds(10));

                if (response.Result == HttpAuthResults.OK)
                    return Request.CreateResponse(HttpStatusCode.OK, new AuthResponse("success", response.SessionToken), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, new AuthResponse("fail", ""), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [Route("connect/server.cgi")]
        public async Task<HttpResponseMessage> doConnectService()
        {
            return await doServiceParse();
        }

        [HttpPost]
        [Route("reconnect/server.cgi")]
        public async Task<HttpResponseMessage> doReconnectService()
        {
            return await doServiceParse();
        }

        [HttpPost]
        [Route("start/server.cgi")]
        public async Task<HttpResponseMessage> doStartService()
        {
            return await doServiceParse();
        }

        [HttpPost]
        [Route("sync/server.cgi")]
        public async Task<HttpResponseMessage> doSyncService()
        {
            return await doServiceParse();
        }

        [HttpPost]
        [Route("bet/server.cgi")]
        public async Task<HttpResponseMessage> doBetService()
        {
            return await doServiceParse();
        }

        [HttpPost]
        [Route("bonus/server.cgi")]
        public async Task<HttpResponseMessage> doBonusService()
        {
            return await doServiceParse();
        }

        [HttpPost]
        [Route("next/server.cgi")]
        public async Task<HttpResponseMessage> doNextService()
        {
            return await doServiceParse();
        }

        [HttpPost]
        [Route("server.cgi")]
        public async Task<HttpResponseMessage> doService()
        {
            return await doServiceParse();
        }

        private async Task<HttpResponseMessage> doServiceParse()
        {
            try
            {
                string strContent   = await Request.Content.ReadAsStringAsync();
                string gameSymbol   = "";

                XmlDocument resultDoc = new XmlDocument();
                resultDoc.LoadXml(strContent);

                XmlNode clientNode  = resultDoc.SelectSingleNode("/client");
                string strCommand   = clientNode.Attributes["command"].Value;

                if (strCommand == null)
                    return new HttpResponseMessage() { Content = new StringContent(buildErrorResponse(0,"unlogged")) };

                GITMessage  requestMessage  = null;
                string      strToken        = null;
                switch (strCommand)
                {
                    case "connect":
                        requestMessage = buildConnectMessage(clientNode, out strToken);
                        gameSymbol = clientNode.Attributes["gameid"].Value;
                        break;
                    case "reconnect":
                        requestMessage = buildReconnectMessage(clientNode, out strToken);
                        break;
                    case "start":
                        requestMessage = buildStartMessage(clientNode, out strToken);
                        break;
                    case "sync":
                        requestMessage = buildSyncMessage(clientNode, out strToken);
                        break;
                    case "bet":
                    case "bonus":
                    case "next":
                        requestMessage = buildPlayMessage(clientNode, out strToken);
                        break;
                }

                if (requestMessage == null || strToken == null)
                    return new HttpResponseMessage() { Content = new StringContent(buildErrorResponse(), Encoding.UTF8, "application/json") };

                string strUserID = findUserIDFromToken(strToken);
                if (strUserID == null)
                    return new HttpResponseMessage() { Content = new StringContent(buildErrorResponse(), Encoding.UTF8, "application/json") };

                //로그인 메세지인경우 먼저 게임에 입장한다.
                if (strCommand == "connect")
                {
                    HTTPEnterGameResults enterGameResult = await HTTPServiceConfig.Instance.WorkerGroup.Ask<HTTPEnterGameResults>(new HTTPEnterGameRequest(strUserID, strToken, GAMETYPE.PLAYSON, gameSymbol), TimeSpan.FromSeconds(10));
                    if (enterGameResult != HTTPEnterGameResults.OK)
                        return new HttpResponseMessage() { Content = new StringContent(buildErrorResponse(0,"Game Not Ready"), Encoding.UTF8, "application/json") };
                }

                ToHTTPSessionMessage response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<ToHTTPSessionMessage>(new FromHTTPSessionMessage(strUserID, strToken, requestMessage));
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse, Encoding.UTF8, "application/json") };
                }
            }
            catch (Exception)
            {

            }
            return new HttpResponseMessage() { Content = new StringContent(buildErrorResponse(), Encoding.UTF8, "application/json") };
        }

        private GITMessage buildConnectMessage(XmlNode clientNode, out string token)
        {
            token = null;
            try
            {
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_PLAYSON_DOCONNECT);
                string strRnd       = clientNode.Attributes["rnd"].Value;
                string playerguid   = clientNode.Attributes["playerguid"].Value;
                string gameid       = clientNode.Attributes["gameid"].Value;
                message.Append(strRnd);
                message.Append(playerguid);
                message.Append(gameid);

                token = playerguid;
                return message;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private GITMessage buildReconnectMessage(XmlNode clientNode, out string token)
        {
            token = null;
            try
            {
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_PLAYSON_DORECONNECT);
                string session  = clientNode.Attributes["session"].Value;
                string strRnd   = clientNode.Attributes["rnd"].Value;
                message.Append(strRnd);
                message.Append(session);
                token = session;
                return message;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private GITMessage buildStartMessage(XmlNode clientNode, out string token)
        {
            token = null;
            try
            {
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_PLAYSON_DOSTART);
                string session  = clientNode.Attributes["session"].Value;
                string strRnd   = clientNode.Attributes["rnd"].Value;
                message.Append(strRnd);
                message.Append(session);
                token = session;
                return message;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private GITMessage buildSyncMessage(XmlNode clientNode, out string token)
        {
            token = null;
            try
            {
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_PLAYSON_DOSYNC);
                string session  = clientNode.Attributes["session"].Value;
                string strRnd   = clientNode.Attributes["rnd"].Value;
                message.Append(strRnd);
                message.Append(session);
                token = session;
                return message;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private GITMessage buildPlayMessage(XmlNode clientNode, out string token)
        {
            token = null;
            try
            {
                XmlNode betNode     = clientNode.OwnerDocument.SelectSingleNode("/client/bet");
                XmlNode debugNode   = clientNode.OwnerDocument.SelectSingleNode("/client/debug");
                XmlNode buyNode     = clientNode.OwnerDocument.SelectSingleNode("/client/buy");

                string session  = clientNode.Attributes["session"].Value;
                string strRnd   = clientNode.Attributes["rnd"].Value;
                int betPerLine      = 0;
                int betMoney        = 0;
                bool buyFreeGame    = false;
                
                if (betNode != null && betNode.Attributes["cash"] != null)
                    betPerLine  = Convert.ToInt32(betNode.Attributes["cash"].Value);
                if (debugNode != null && debugNode.Attributes["bet_cash"] != null)
                    betMoney    = Convert.ToInt32(debugNode.Attributes["bet_cash"].Value);

                if(buyNode != null)
                {
                    if(buyNode.Attributes["freegame"] != null)
                        buyFreeGame = Convert.ToBoolean(buyNode.Attributes["freegame"].Value);
                    else if (buyNode.Attributes["bonus_game"] != null)
                        buyFreeGame = Convert.ToBoolean(buyNode.Attributes["bonus_game"].Value);
                }

                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_PLAYSON_DOPLAY);
                message.Append(strRnd);
                message.Append(session);
                message.Append(betPerLine);
                message.Append(betMoney);
                if(buyFreeGame)
                    message.Append(1);
                else
                    message.Append(0);
                message.Append(clientNode.OuterXml);

                token = session;
                return message;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private string buildErrorResponse(long rnd = 0, string reason = "OTHER_ERROR")
        {
            XmlDocument responseDoc = new XmlDocument();
            
            XmlElement serverNode   = responseDoc.CreateElement("server");
            if (rnd == 0)
                rnd = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            
            serverNode.SetAttribute("rnd",      rnd.ToString());
            serverNode.SetAttribute("status",   "sessionlost");

            XmlElement extraNode = responseDoc.CreateElement("extra");
            XmlElement errorNode = responseDoc.CreateElement("error");
            errorNode.SetAttribute("code",      reason);

            extraNode.AppendChild(errorNode);
            serverNode.AppendChild(extraNode);
            responseDoc.AppendChild(serverNode);

            return responseDoc.InnerXml;
        }

        private string findUserIDFromToken(string strToken)
        {
            string[] strParts = strToken.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
            if (strParts.Length != 2)
                return null;

            return strParts[0];
        }
    }
}
