using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Akka.Actor;
using System.Web;
using GITProtocol;
using System.Net.Http.Formatting;
using System.Web.Http;
using Microsoft.Owin;
using System.Net.Http;
using CommNode.HTTPService.Models;
using Newtonsoft.Json;
using System.Globalization;
using Newtonsoft.Json.Linq;

[assembly: OwinStartup(typeof(CommNode.HTTPService.HabaneroServiceController))]
namespace CommNode.HTTPService
{
    [RoutePrefix("gitapi/habanero/service")]
    public class HabaneroServiceController : ApiController
    {
        [HttpGet]
        [Route("auth.do")]
        public async Task<HttpResponseMessage> doAuth()
        {
            int agentID         = 0;
            string strUserID    = null;
            string strPassword  = null;
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
        [Route("ps")]
        public async Task<HttpResponseMessage> doService()
        {
            try
            {
                string strContent = await Request.Content.ReadAsStringAsync();
                HabaneroRequest request = JsonConvert.DeserializeObject<HabaneroRequest>(strContent);

                GITMessage requestMessage   = null;
                string strToken             = null;
                switch (request.game.action)
                {
                    case "init":
                        requestMessage = buildInitMessage(strContent, out strToken);
                        break;
                    case "balance":
                        requestMessage = buildBalanceMessage(strContent, out strToken);
                        break;
                    case "game":
                        requestMessage = buildGameMessage(strContent, out strToken);
                        break;

                }
                if (requestMessage == null || strToken == null)
                    return new HttpResponseMessage() { Content = new StringContent(buildErrorResponse("init", request.grid), Encoding.UTF8, "application/json") };

                string strUserID = findUserIDFromToken(strToken);
                if (strUserID == null)
                    return new HttpResponseMessage() { Content = new StringContent(buildErrorResponse("init", request.grid), Encoding.UTF8, "application/json") };


                //인이트 메세지인경우 먼저 게임에 입장한다.
                if (request.game.action == "init")
                {
                    HabaneroRequest initRequest = JsonConvert.DeserializeObject<HabaneroRequest>(strContent);
                    HTTPEnterGameResults enterGameResult = await HTTPServiceConfig.Instance.WorkerGroup.Ask<HTTPEnterGameResults>(new HTTPEnterGameRequest(strUserID, strToken, GAMETYPE.HABANERO, initRequest.game.kn), TimeSpan.FromSeconds(10));
                    if (enterGameResult != HTTPEnterGameResults.OK)
                        return new HttpResponseMessage() { Content = new StringContent(buildErrorResponse("init", request.grid), Encoding.UTF8, "application/json") };
                }

                ToHTTPSessionMessage response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<ToHTTPSessionMessage>(new FromHTTPSessionMessage(strUserID, strToken, requestMessage));
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse, Encoding.UTF8, "application/json") };
                }
            }
            catch(Exception ex)
            {

            }
            return new HttpResponseMessage() { Content = new StringContent(buildErrorResponse("", ""), Encoding.UTF8, "application/json") };
        }
        private GITMessage buildInitMessage(string strContent, out string token)
        {
            token = null;
            try
            {
                HabaneroRequest initRequest = JsonConvert.DeserializeObject<HabaneroRequest>(strContent);
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_HABANERO_DOINIT);
                message.Append(initRequest.grid);
                token = Convert.ToString(initRequest.header["player"]["ssotoken"]);
                message.Append(token);
                return message;
            }
            catch(Exception)
            {
                return null;
            }
        }
        
        private GITMessage buildBalanceMessage(string strContent, out string token)
        {
            token = null;
            try
            {
                HabaneroRequest balanceRequest = JsonConvert.DeserializeObject<HabaneroRequest>(strContent);
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_HABANERO_DOBALANCE);
                message.Append(balanceRequest.game.sessionid);
                token = Convert.ToString(balanceRequest.header["player"]["ssotoken"]);
                message.Append(token);
                return message;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        private GITMessage buildGameMessage(string strContent, out string token)
        {
            token = null;
            try
            {
                HabaneroRequest gameRequest = JsonConvert.DeserializeObject<HabaneroRequest>(strContent);
                dynamic portMessage         = JsonConvert.DeserializeObject<dynamic>(gameRequest.portmessage);
                
                string gameMode = "";
                if (!object.ReferenceEquals(portMessage["gameMode"], null))
                    gameMode = Convert.ToString(portMessage["gameMode"]);

                if (gameMode == "free" || string.IsNullOrEmpty(gameMode))
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_HABANERO_DOSPIN);
                    message.Append(gameRequest.game.sessionid);
                    message.Append(gameRequest.grid);
                    token = Convert.ToString(gameRequest.header["player"]["ssotoken"]);
                    message.Append(token);
                    message.Append(0.0f);
                    message.Append(0);
                    message.Append(0);
                    return message;
                }
                else if (gameMode == "updateClientPick")
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_HABANERO_DOUPDATECLIENTPICK);
                    message.Append(gameRequest.game.sessionid);
                    message.Append(gameRequest.grid);
                    token = Convert.ToString(gameRequest.header["player"]["ssotoken"]);
                    message.Append(token);
                    message.Append(0.0f);
                    message.Append(0);
                    message.Append(0);
                    return message;
                }
                else if (gameMode == "pick")
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_HABANERO_DOCLIENT);
                    message.Append(gameRequest.game.sessionid);
                    message.Append(gameRequest.grid);
                    token = Convert.ToString(gameRequest.header["player"]["ssotoken"]);
                    message.Append(token);
                    if(!object.ReferenceEquals(portMessage["pickIdIndex"],null))
                        message.Append((int)portMessage["pickIdIndex"]);
                    else
                        message.Append(0);
                    return message;
                }
                else
                {
                    GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_HABANERO_DOSPIN);
                    message.Append(gameRequest.game.sessionid);
                    message.Append(gameRequest.grid);
                    token = Convert.ToString(gameRequest.header["player"]["ssotoken"]);
                    message.Append(token);
                    float coinValue = (float)portMessage["coinValue"];
                    int lineCount   = (int)portMessage["numLines"];
                    int betLevel    = (int)portMessage["betLevel"];
                    message.Append(coinValue);
                    message.Append(lineCount);
                    message.Append(betLevel);

                    if (!object.ReferenceEquals(portMessage["featureBuy"], null))
                        message.Append((int)portMessage["featureBuy"]);
                    else
                        message.Append(0);

                    if (!object.ReferenceEquals(portMessage["superBet"], null))
                        message.Append((int)portMessage["superBet"]);
                    else
                        message.Append(0);

                    return message;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string buildErrorResponse(string strCommand, string strRequestID)
        {
            HabaneroErrorResponse response  = new HabaneroErrorResponse();
            response.header.st      = DateTime.UtcNow.ToString("o", CultureInfo.GetCultureInfo("en-US"));
            response.header.time    = 210;
            response.header.player  = new JObject();
            response.header.player["brandid"]           = "00000000-0000-0000-0000-000000000000";
            response.header.player["gamesymbol"]        = "";
            response.header.player["curexp"]            = 2;
            response.header.player["displaybalance"]    = 0.0;
            response.header.player["realbalance"]       = 0.0;
            response.header.player["ssotoken"]          = "";

            response.error.code     = "GENERAL_SESSIONEXPIRED";
            response.error.message  = "User session expired";
            response.error.type     = 2;
            response.error.suppress = false;
            response.error.msgcode  = "U.01";
            return JsonConvert.SerializeObject(response);
        }
        
        private string findUserIDFromToken(string strToken)
        {
            string[] strParts = strToken.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
            if (strParts.Length != 2)
                return null;

            return strParts[0];
        }
    }

    public class HabaneroRequest
    {
        public HabaneroRequestGame  game        { get; set; }
        public string               grid        { get; set; }
        public dynamic              header      { get; set; }
        public string               portmessage { get; set; }
    }
    public class HabaneroRequestGame
    {
        public string   action          { get; set; }
        public string   brandgameid     { get; set; }
        public string   kn              { get; set; }
        public string   sessionid       { get; set; }
        public dynamic  play            { get; set; }
        public int      clientpickdata  { get; set; }
    }
    public class HabaneroErrorResponse
    {
        public HabaneroResponseHeader       header  { get; set; }
        public HabaneroResponseSessionError error   { get; set; }
        public HabaneroErrorResponse()
        {
            header  = new HabaneroResponseHeader();
            error   = new HabaneroResponseSessionError();
        }

    }
    public class HabaneroResponseHeader
    {
        public string   st      { get; set; }
        public int      time    { get; set; }
        public dynamic  player  { get; set; }
    }
    public class HabaneroResponseSessionError
    {
        public string   code        { get; set; }
        public string   message     { get; set; }
        public int      type        { get; set; }
        public bool     suppress    { get; set; }
        public string   msgcode     { get; set; }
    }
}
