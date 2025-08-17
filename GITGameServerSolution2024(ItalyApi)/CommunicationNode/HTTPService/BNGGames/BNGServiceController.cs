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

[assembly: OwinStartup(typeof(CommNode.HTTPService.BNGServiceController))]
namespace CommNode.HTTPService
{
    [RoutePrefix("gitapi/bng/service")]
    public class BNGServiceController : ApiController
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

            if (agentID == 0 || strUserID == null || strPassword == null)
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
        [Route("{gameSymbol}/prod/")]
        public async Task<HttpResponseMessage> doService(string gameSymbol)
        {
            try
            {
                string strCommand = null;
                foreach (KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
                {
                    if (pair.Key == "gsc")
                        strCommand = pair.Value;
                }
                if (strCommand == null)
                    return new HttpResponseMessage() { Content = new StringContent("unlogged") };

                string strContent = await Request.Content.ReadAsStringAsync();
                BNGRequest request = JsonConvert.DeserializeObject<BNGRequest>(strContent);

                GITMessage requestMessage = null;
                string strToken = null;
                switch (request.command)
                {
                    case "login":
                        requestMessage = buildLoginMessage(strContent, out strToken);
                        break;
                    case "start":
                        requestMessage = buildStartMessage(strContent, out strToken);
                        break;
                    case "sync":
                        requestMessage = buildSyncMessage(strContent, out strToken);
                        break;
                    case "play":
                        requestMessage = buildPlayMessage(strContent, out strToken);
                        break;

                }
                if (requestMessage == null || strToken == null)
                    return new HttpResponseMessage() { Content = new StringContent("unlogged") };

                string strUserID = findUserIDFromToken(strToken);
                if (strUserID == null)
                    return new HttpResponseMessage() { Content = new StringContent("unlogged") };


                //로그인 메세지인경우 먼저 게임에 입장한다.
                if (request.command == "login")
                {
                    HTTPEnterGameResults enterGameResult = await HTTPServiceConfig.Instance.WorkerGroup.Ask<HTTPEnterGameResults>(new HTTPEnterGameRequest(strUserID, strToken, GAMETYPE.BNG, gameSymbol), TimeSpan.FromSeconds(10));
                    if (enterGameResult != HTTPEnterGameResults.OK)
                        return new HttpResponseMessage() { Content = new StringContent(buildErrorResponse("login", request.request_id), Encoding.UTF8, "application/json") };
                }

                ToHTTPSessionMessage response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<ToHTTPSessionMessage>(new FromHTTPSessionMessage(strUserID, strToken, requestMessage));
                if (response.Result == ToHTTPSessionMsgResults.OK)
                {
                    string strResponse = (string)response.Message.Pop();
                    return new HttpResponseMessage() { Content = new StringContent(strResponse, Encoding.UTF8, "application/json") };
                }
            }
            catch(Exception)
            {

            }
            return new HttpResponseMessage() { Content = new StringContent(buildErrorResponse("", ""), Encoding.UTF8, "application/json") };
        }
        private GITMessage buildLoginMessage(string strContent, out string token)
        {
            token = null;
            try
            {
                BNGLoginRequest loginRequest = JsonConvert.DeserializeObject<BNGLoginRequest>(strContent);
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_BNG_DOLOGIN);
                message.Append(loginRequest.request_id);
                message.Append(loginRequest.token);
                token = loginRequest.token;
                return message;
            }
            catch(Exception)
            {
                return null;
            }
        }
        private GITMessage buildStartMessage(string strContent, out string token)
        {
            token = null;
            try
            {
                BNGStartRequest loginRequest = JsonConvert.DeserializeObject<BNGStartRequest>(strContent);
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_BNG_DOSTART);
                message.Append(loginRequest.request_id);
                message.Append(loginRequest.session_id);
                message.Append(loginRequest.huid);
                message.Append(loginRequest.mode);
                token = loginRequest.session_id;
                return message;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private GITMessage buildSyncMessage(string strContent, out string token)
        {
            token = null;
            try
            {
                BNGSyncRequest loginRequest = JsonConvert.DeserializeObject<BNGSyncRequest>(strContent);
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_BNG_DOSYNC);
                message.Append(loginRequest.request_id);
                message.Append(loginRequest.session_id);
                token = loginRequest.session_id;
                return message;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private GITMessage buildPlayMessage(string strContent, out string token)
        {
            token = null;
            try
            {
                BNGPlayRequest playRequest = JsonConvert.DeserializeObject<BNGPlayRequest>(strContent);
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_BNG_DOPLAY);
                message.Append(playRequest.request_id);
                message.Append(playRequest.session_id);
                message.Append(playRequest.action.name);
                if(playRequest.action.param == null || playRequest.action.param.bet_per_line == 0.0)
                {
                    message.Append(0.0);
                    message.Append(0);
                }
                else
                {
                    message.Append(playRequest.action.param.bet_per_line);
                    message.Append(playRequest.action.param.lines);
                }
                message.Append(playRequest.autogame);
                message.Append(playRequest.mobile);
                message.Append(playRequest.portrait);
                message.Append(playRequest.quick_spin);
                message.Append(playRequest.sound);
                token = playRequest.session_id;
                return message;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string buildErrorResponse(string strCommand, string strRequestID)
        {
            BNGErrorResponse response   = new BNGErrorResponse();
            response.command            = strCommand;
            response.request_id         = strRequestID;
            response.status             = new BNGErrorResponse.Status();
            response.status.code        = "SESSION_CLOSED";
            response.status.type        = "crit";
            return JsonConvert.SerializeObject(response);
        }
        [HttpPost]
        [Route("log/{gameSymbol}/prod/", Name = "doLog")]
        public HttpResponseMessage doLog(string gameSymbol)
        {
            return new HttpResponseMessage() { Content = new StringContent("{}", Encoding.UTF8, "application/json") };
        }
        [HttpPost]
        [Route("process/", Name = "doProcess")]
        public HttpResponseMessage doProcess()
        {
            return new HttpResponseMessage() { Content = new StringContent(BNGPromoSnapshot.SnapShot, Encoding.UTF8, "application/json") };
        }
        private string findUserIDFromToken(string strToken)
        {
            string[] strParts = strToken.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
            if (strParts.Length != 2)
                return null;

            return strParts[0];
        }
    }
    public class BNGRequest
    {
        public string command       { get; set; }
        public string request_id    { get; set; }
    }
    public class BNGLoginRequest : BNGRequest
    {
        public string language  { get; set; }
        public string token     { get; set; }
    }
    public class BNGStartRequest: BNGRequest
    {
        public string huid { get; set; }
        public string mode { get; set; }
        public string session_id { get; set; }
    }
    public class BNGSyncRequest : BNGRequest
    {
        public string session_id { get; set; }
    }
    public class BNGPlayRequest : BNGRequest
    {
        public bool     autogame        { get; set; }
        public bool     mobile          { get; set; }
        public bool     portrait        { get; set; }
        public bool     quick_spin      { get; set; }
        public string   session_id      { get; set; }
        public int      set_denominator { get; set; }
        public bool     sound           { get; set; }
        public Action   action          { get; set; }
        public class Action
        {
            public string name { get; set; }

            [JsonProperty("params")]
            public Params param {get; set;}
            public class Params
            {
                public double   bet_per_line { get; set; }
                public int      lines        { get; set; }
            }
        }
    }

    public class BNGErrorResponse
    {
        public string command       { get; set; }
        public string request_id    { get; set; }
        public Status status        { get; set; }
        public class Status
        {
            public string code { get; set; }
            public string type { get; set; }
        }
    }

}
