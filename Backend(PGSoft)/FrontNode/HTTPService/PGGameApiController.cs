using Akka.Actor;
using GITProtocol;
using Microsoft.Owin;
using Newtonsoft.Json;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;

[assembly: OwinStartup(typeof(FrontNode.HTTPService.PGGameApiController))]
namespace FrontNode.HTTPService
{
    [RoutePrefix("game-api")]
    public class PGGameApiController : ApiController
    {
        [HttpPost]
        [Route("{gameSymbol}/v2/GameInfo/Get")]
        public async Task<HttpResponseMessage> doGetGameInfo([FromUri] string gameSymbol, [FromUri] string traceId, [FromBody] GetGameInfoRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(traceId) || string.IsNullOrEmpty(gameSymbol))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);
            
                if (string.IsNullOrEmpty(request.atk))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                int     agentID = 0;
                string  strUserID = null;
                if (!findUserIDFromToken(request.atk, ref agentID, ref strUserID))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                HTTPEnterGameResponse enterGameResult = await HTTPServiceConfig.Instance.WorkerGroup.Ask<HTTPEnterGameResponse>(new HTTPEnterGameRequest(agentID,strUserID, request.atk, gameSymbol), TimeSpan.FromSeconds(20));
                if (enterGameResult.Result != HTTPEnterGameResults.OK)
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                GetGameInfoResponse response = new GetGameInfoResponse(enterGameResult);
                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);

            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [Route("{gameSymbol}/v2/GameInfo/Update")]
        public async Task<HttpResponseMessage> doUpdateGameInfo([FromUri] string gameSymbol, [FromUri] string traceId, [FromBody] UpdateGameInfoRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(traceId) || string.IsNullOrEmpty(gameSymbol))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                if (string.IsNullOrEmpty(request.atk))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                int     agentID     = 0;
                string  strUserID   = null;
                if (!findUserIDFromToken(request.atk, ref agentID, ref strUserID))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                GITMessage message = new GITMessage(MsgCodes.UPDATEGAMEINFO);
                message.Append(request.cs);
                message.Append(request.ml);
                message.Append(request.wk);
                SpinResponseData response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<SpinResponseData>(new FromHTTPSessionMessage(agentID, strUserID, request.atk, message), TimeSpan.FromSeconds(20));
                return Request.CreateResponse(HttpStatusCode.OK, convertDataToResponse(response), Configuration.Formatters.JsonFormatter);
            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);
        }
        private SpinResponse convertDataToResponse(SpinResponseData responseData)
        {
            var response = new SpinResponse();
            response.err = responseData.err;
            response.dt  = JsonConvert.DeserializeObject<dynamic>(responseData.dt);
            return response;
        }

        [HttpPost]
        [Route("{gameSymbol}/v2/Spin")]
        public async Task<HttpResponseMessage> doSpin([FromUri] string gameSymbol, [FromUri] string traceId, [FromBody] SpinRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(traceId) || string.IsNullOrEmpty(gameSymbol))
                    return Request.CreateResponse(HttpStatusCode.OK, new SpinResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                if (string.IsNullOrEmpty(request.atk))
                    return Request.CreateResponse(HttpStatusCode.OK, new SpinResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                int     agentID     = 0;
                string  strUserID   = null;
                if (!findUserIDFromToken(request.atk, ref agentID, ref strUserID))
                    return Request.CreateResponse(HttpStatusCode.OK, new SpinResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                GITMessage message = new GITMessage(MsgCodes.SPIN);
                message.Append(request.id);
                message.Append(request.cs);
                message.Append(request.ml);
                message.Append(request.fb);
                message.Append(request.wk);
                message.Append(request.ig);
                message.Append(request.gt);
                if (request.ps >= 0)
                    message.Append(request.ps);
                else
                    message.Append(request.fss);
                if (request.ns >= 0)
                    message.Append(request.ns);
                if (request.mls >= 0)
                    message.Append(request.mls);
                if (request.gpt >= 0)
                    message.Append(request.gpt);
                if (request.bn >= 0)
                    message.Append(request.bn);                
                if (request.uft >= 0)
                    message.Append(request.uft);                
                if (request.fp >= 0)
                    message.Append(request.fp);
                if (request.ab >= 0)
                    message.Append(request.ab);
                SpinResponseData response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<SpinResponseData>(new FromHTTPSessionMessage(agentID, strUserID, request.atk,  message), TimeSpan.FromSeconds(20));
                return Request.CreateResponse(HttpStatusCode.OK, convertDataToResponse(response), Configuration.Formatters.JsonFormatter);

            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, new SpinResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [Route("{gameSymbol}/v2/Spin/SelectPot")]
        public async Task<HttpResponseMessage> doSelectPot([FromUri] string gameSymbol, [FromUri] string traceId, [FromBody] SpinRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(traceId) || string.IsNullOrEmpty(gameSymbol))
                    return Request.CreateResponse(HttpStatusCode.OK, new SpinResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                if (string.IsNullOrEmpty(request.atk))
                    return Request.CreateResponse(HttpStatusCode.OK, new SpinResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                int     agentID = 0;
                string  strUserID = null;
                if (!findUserIDFromToken(request.atk, ref agentID, ref strUserID))
                    return Request.CreateResponse(HttpStatusCode.OK, new SpinResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                GITMessage message = new GITMessage(MsgCodes.SPIN);
                message.Append(request.id);
                message.Append(request.cs);
                message.Append(request.ml);
                message.Append(request.fb);
                message.Append(request.wk);
                message.Append(request.ig);
                message.Append(request.gt);
                if (request.ps >= 0)
                    message.Append(request.ps);
                else
                    message.Append(request.fss);
                if (request.ns >= 0)
                    message.Append(request.ns);
                if (request.mls >= 0)
                    message.Append(request.mls);
                if (request.gpt >= 0)
                    message.Append(request.gpt);
                if (request.bn >= 0)
                    message.Append(request.bn);
                if (request.uft >= 0)
                    message.Append(request.uft);
                if (request.fp >= 0)
                    message.Append(request.fp);
                SpinResponseData response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<SpinResponseData>(new FromHTTPSessionMessage(agentID, strUserID, request.atk, message), TimeSpan.FromSeconds(20));
                return Request.CreateResponse(HttpStatusCode.OK, convertDataToResponse(response), Configuration.Formatters.JsonFormatter);

            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, new SpinResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [Route("{gameSymbol}/v2/selectcharacter")]
        public async Task<HttpResponseMessage> doSelectCharacter([FromUri] string gameSymbol, [FromUri] string traceId, [FromBody] SelectCharacterRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(traceId) || string.IsNullOrEmpty(gameSymbol))
                    return Request.CreateResponse(HttpStatusCode.OK, new SpinResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                if (string.IsNullOrEmpty(request.atk))
                    return Request.CreateResponse(HttpStatusCode.OK, new SpinResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                int     agentID     = 0;
                string  strUserID   = null;
                if (!findUserIDFromToken(request.atk, ref agentID, ref strUserID))
                    return Request.CreateResponse(HttpStatusCode.OK, new SpinResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                GITMessage message = new GITMessage(MsgCodes.SELECTCHARACTER);
                message.Append(request.id);
                message.Append(request.cs);
                message.Append(request.ml);
                message.Append(request.wk);
                message.Append(request.s);

                SpinResponseData response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<SpinResponseData>(new FromHTTPSessionMessage(agentID, strUserID, request.atk, message), TimeSpan.FromSeconds(20));
                return Request.CreateResponse(HttpStatusCode.OK, convertDataToResponse(response), Configuration.Formatters.JsonFormatter);

            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, new SpinResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [Route("{gameSymbol}/v2/Spin/Free/PlayerSelection")]
        public async Task<HttpResponseMessage> doFreeSelection([FromUri] string gameSymbol, [FromUri] string traceId, [FromBody] SpinRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(traceId) || string.IsNullOrEmpty(gameSymbol))
                    return Request.CreateResponse(HttpStatusCode.OK, new SpinResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                if (string.IsNullOrEmpty(request.atk))
                    return Request.CreateResponse(HttpStatusCode.OK, new SpinResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                int     agentID     = 0;
                string  strUserID   = null;
                if (!findUserIDFromToken(request.atk, ref agentID, ref strUserID))
                    return Request.CreateResponse(HttpStatusCode.OK, new SpinResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                GITMessage message = new GITMessage(MsgCodes.SPIN);
                message.Append(request.id);
                message.Append(request.cs);
                message.Append(request.ml);
                message.Append(request.fb);
                message.Append(request.wk);
                message.Append(request.ig);
                message.Append(request.gt);
                if (request.ps >= 0)
                    message.Append(request.ps);
                else
                    message.Append(request.fss);

                SpinResponseData response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<SpinResponseData>(new FromHTTPSessionMessage(agentID, strUserID, request.atk, message), TimeSpan.FromSeconds(20));
                return Request.CreateResponse(HttpStatusCode.OK, convertDataToResponse(response), Configuration.Formatters.JsonFormatter);

            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, new SpinResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);
        }


        [HttpPost]
        [Route("{gameSymbol}/v2/Spin/Bonus")]
        public async Task<HttpResponseMessage> doBonus([FromUri] string gameSymbol, [FromUri] string traceId, [FromBody] SpinRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(traceId) || string.IsNullOrEmpty(gameSymbol))
                    return Request.CreateResponse(HttpStatusCode.OK, new SpinResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                if (string.IsNullOrEmpty(request.atk))
                    return Request.CreateResponse(HttpStatusCode.OK, new SpinResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                int     agentID     = 0;
                string  strUserID   = null;
                if (!findUserIDFromToken(request.atk, ref agentID, ref strUserID))
                    return Request.CreateResponse(HttpStatusCode.OK, new SpinResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                GITMessage message = new GITMessage(MsgCodes.SPIN);
                message.Append(request.id);
                message.Append(request.cs);
                message.Append(request.ml);
                message.Append(request.fb);
                message.Append(request.wk);
                message.Append(request.ig);
                message.Append(request.gt);
                message.Append(request.pft);

                SpinResponseData response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<SpinResponseData>(new FromHTTPSessionMessage(agentID, strUserID, request.atk, message), TimeSpan.FromSeconds(20));
                return Request.CreateResponse(HttpStatusCode.OK, convertDataToResponse(response), Configuration.Formatters.JsonFormatter);

            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, new SpinResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);
        }

        private bool findUserIDFromToken(string strToken, ref int agentID, ref string strUserID)
        {
            string[] strParts = strToken.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
            if (strParts.Length != 2)
                return false;

            string[] strSubParts = strParts[0].Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            agentID = int.Parse(strSubParts[0]);
            strUserID = strSubParts[1];
            return true;
        }
    }

    public class SpinRequest
    {
        public long     id  { get; set; }
        public double   cs  { get; set; }   //selectedBetSizeValue
        public int      ml  { get; set; }   //selectedBetLevelValue
        public string   wk  { get; set; }   //walletKey
        public int      fb  { get; set; }   //featurebuy
        public int      ab  { get; set; }   //AnteBet
        public int      btt { get; set; }   //bettype
        public string   atk { get; set; }
        public int      pf  { get; set; }
        public bool     ig  { get; set; }
        public int      gt  { get; set; }
        public int      fss { get; set; }
        public int      ps  { get; set; }
        public int      pft { get; set; }
        public int      ns  { get; set; }
        public int      mls { get; set; }
        public int      gpt { get; set; }
        public int      bn  { get; set; }
        public int      uft { get; set; }
        public int      fp  { get; set; }
        public SpinRequest()
        {
            fss = -1;
            ps  = -1;
            mls = -1;
            ns  = -1;
            gpt = -1;
            bn  = -1;
            uft = -1;
            fp  = -1;
        }
    }
    public class GetGameInfoRequest
    {
        public int      btt     { get; set; }
        public string   atk     { get; set; }
        public int      pf      { get; set; }

    }
    public class SelectCharacterRequest
    {
        public long     id  { get; set; }
        public double   cs  { get; set; }   //selectedBetSizeValue
        public int      ml  { get; set; }   //selectedBetLevelValue
        public string   wk  { get; set; }   //walletKey
        public int      btt { get; set; }   //bettype
        public string   atk { get; set; }
        public int      pf  { get; set; }
        public int      s   { get; set; }   //selected id
        public SelectCharacterRequest()
        {
        }
    }

    public class UpdateGameInfoRequest
    {
        public double   cs  { get; set; }
        public int      ml  { get; set; }
        public int      btt { get; set; }
        public string   atk { get; set; }
        public int      pf  { get; set; }
        public string   wk  { get; set; }
    }

}
