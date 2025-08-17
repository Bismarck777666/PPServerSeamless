using Akka.Actor;
using CommNode.HTTPService.Models;
using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

[assembly: OwinStartup(typeof(CommNode.HTTPService.CQ9ServiceController))]
namespace CommNode.HTTPService
{
    [RoutePrefix("gitapi/cq9/service")]
    public class CQ9ServiceController : ApiController
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
        [Route("ws")]
        public WSInfoResponse WS(WSInfoRequest req)
        {
            WSInfoResponse msg = new WSInfoResponse(0, "Success", "wss://bismarck.com:65535");

            return msg;
        }
        
        [HttpGet]
        [Route("clientinfo")]
        public ClientInfoResponse clientinfo()
        {
            string strToken     = null;
            foreach (KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
            {
                if (pair.Key == "token")
                    strToken = pair.Value;
            }
            string strIPAddress = Request.GetOwinContext().Request.RemoteIpAddress;
            return new ClientInfoResponse(strIPAddress,"");
        }
    }

    public class WSInfoResponse
    {
        public int      code        { get; set; }
        public string   message     { get; set; }
        public WSItem   data        { get; set; }
        public WSInfoResponse(int _code,string _message,string wsinfo)
        {
            code    = _code;
            message = _message;
            data    = new WSItem(wsinfo);
        }
    }
    public class WSItem
    {
        public string ws   { get; set; }
        public WSItem(string _ws)
        {
            ws = _ws;
        }
    }
    public class WSInfoRequest { 
        public string cdn   { get; set; }
    } 

    public class ClientInfoResponse
    {
        public ClientInfoItem   data    { get; set; }
        public ClientInfoResponse(string _ip,string _code)
        {
            this.data       = new ClientInfoItem(_ip, _code);
        }
    }
    public class ClientInfoItem
    {
        public string ip            { get; set; }
        public string code          { get; set; }
        public string datetime      { get; set; }
        public ClientInfoItem(string _ip,string _code)
        {
            this.ip         = _ip;
            this.code       = _code;
            this.datetime   = DateTime.UtcNow.ToString();
        }
    }
}
