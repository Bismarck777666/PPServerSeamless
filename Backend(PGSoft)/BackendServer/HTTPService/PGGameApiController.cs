using Akka.Actor;
using Amazon.Runtime.Internal;
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
using WebSocketSharp;

[assembly: OwinStartup(typeof(SlotGamesNode.HTTPService.PGGameApiController))]
namespace SlotGamesNode.HTTPService
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

                string strUserID = findUserIDFromToken(request.atk);
                if (strUserID == null)
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                HTTPEnterGameResponse enterGameResult = await HTTPServiceConfig.Instance.WorkerGroup.Ask<HTTPEnterGameResponse>(new HTTPEnterGameRequest(strUserID, request.atk, gameSymbol), TimeSpan.FromSeconds(10));
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

                string strUserID = findUserIDFromToken(request.atk);
                if (strUserID == null)
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                GITMessage message = new GITMessage(MsgCodes.UPDATEGAMEINFO);
                message.Append(request.cs);
                message.Append(request.ml);
                message.Append(request.wk);
                SpinResponse response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<SpinResponse>(new FromHTTPSessionMessage(strUserID, request.atk, message), TimeSpan.FromSeconds(10));
                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);
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

                string strUserID = findUserIDFromToken(request.atk);
                if (strUserID == null)
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
                SpinResponse response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<SpinResponse>(new FromHTTPSessionMessage(strUserID, request.atk,  message), TimeSpan.FromSeconds(10));
                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);

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

                string strUserID = findUserIDFromToken(request.atk);
                if (strUserID == null)
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
                SpinResponse response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<SpinResponse>(new FromHTTPSessionMessage(strUserID, request.atk, message), TimeSpan.FromSeconds(10));
                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);

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

                string strUserID = findUserIDFromToken(request.atk);
                if (strUserID == null)
                    return Request.CreateResponse(HttpStatusCode.OK, new SpinResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                GITMessage message = new GITMessage(MsgCodes.SELECTCHARACTER);
                message.Append(request.id);
                message.Append(request.cs);
                message.Append(request.ml);
                message.Append(request.wk);
                message.Append(request.s);

                SpinResponse response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<SpinResponse>(new FromHTTPSessionMessage(strUserID, request.atk, message), TimeSpan.FromSeconds(10));
                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);

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

                string strUserID = findUserIDFromToken(request.atk);
                if (strUserID == null)
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

                SpinResponse response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<SpinResponse>(new FromHTTPSessionMessage(strUserID, request.atk, message), TimeSpan.FromSeconds(10));
                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);

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

                string strUserID = findUserIDFromToken(request.atk);
                if (strUserID == null)
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

                SpinResponse response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<SpinResponse>(new FromHTTPSessionMessage(strUserID, request.atk, message), TimeSpan.FromSeconds(10));
                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);

            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, new SpinResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);
        }

        #region Lobby Callback APIS
        protected string genRandomId(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Pcg.Default.Next(s.Length)]).ToArray());
        }

        [HttpPost]
        [Route("lobby/WebLobby/Get")]
        public async Task<HttpResponseMessage> doWebLobbyGet(WebLobbyGetRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.atk))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", genRandomId(8)), Configuration.Formatters.JsonFormatter);

                string strUserID = findUserIDFromToken(request.atk);
                if (strUserID == null)
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", genRandomId(8)), Configuration.Formatters.JsonFormatter);

                GITMessage message = new GITMessage(MsgCodes.GETWEBLOBBYGET);
                message.Append(request.cc);
                message.Append(request.lang);
                var response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<object>(new FromHTTPSessionMessage(strUserID, request.atk, message), TimeSpan.FromSeconds(10));
                if (response is ToHTTPSessionMsgResults)
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", genRandomId(8)), Configuration.Formatters.JsonFormatter);
                else
                    return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);

            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", genRandomId(8)), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [Route("lobby/gameInfo/v1/getGameDetails")]
        public HttpResponseMessage doWebLobbyGetGameDetails(WebLobbyGetGameDetailsRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.otk) || string.IsNullOrEmpty(request.lang))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", genRandomId(8)), Configuration.Formatters.JsonFormatter);

                string strJson  = LobbyDataSnapshot.Instance.getLobbyGameDetailsJson(request.lang);
                JToken response = JToken.Parse(strJson);
                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);

            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", genRandomId(8)), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [Route("lobby/Resource/GetAllGamesResources")]
        public HttpResponseMessage doWebLobbyGetGameResources(WebLobbyGetGameResourcesRequest request)
        {
            try
            {
                
                if (string.IsNullOrEmpty(request.otk) || string.IsNullOrEmpty(request.lang))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", genRandomId(8)), Configuration.Formatters.JsonFormatter);

                string strJson = LobbyDataSnapshot.Instance.getLobbyGameResourcesJson(request.lang);
                JToken response = JToken.Parse(strJson);
                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);

            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", genRandomId(8)), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [Route("lobby/tournament/v1/GetInitTournaments")]
        public async Task<HttpResponseMessage> doGetInitTournaments(WebLobbyGetInitTournamnentsRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.atk))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", genRandomId(8)), Configuration.Formatters.JsonFormatter);

                string strUserID = findUserIDFromToken(request.atk);
                if (strUserID == null)
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", genRandomId(8)), Configuration.Formatters.JsonFormatter);

                GITMessage message = new GITMessage(MsgCodes.GETINITTOURNAMENTS);
                message.Append(request.barc);
                message.Append(request.st);
                message.Append(request.pf);
                message.Append(request.pn);
                message.Append(request.rc);
                message.Append(request.lang);
                var response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<object>(new FromHTTPSessionMessage(strUserID, request.atk, message), TimeSpan.FromSeconds(10));
                if (response is ToHTTPSessionMsgResults)
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", genRandomId(8)), Configuration.Formatters.JsonFormatter);
                else
                    return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", genRandomId(8)), Configuration.Formatters.JsonFormatter);
        }
        [HttpPost]
        [Route("lobby/Rating/Add")]
        public HttpResponseMessage doLobbyRatingAdd(WebLobbyRatingAddRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.atk))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", genRandomId(8)), Configuration.Formatters.JsonFormatter);

                string strUserID = findUserIDFromToken(request.atk);
                if (strUserID == null)
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", genRandomId(8)), Configuration.Formatters.JsonFormatter);


                var response = JToken.Parse("{\"dt\":true,\"err\":null}");
                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", genRandomId(8)), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [Route("lobby/Rating/Remove")]
        public HttpResponseMessage doLobbyRatingRemove(WebLobbyRatingRemoveRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.atk))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", genRandomId(8)), Configuration.Formatters.JsonFormatter);

                string strUserID = findUserIDFromToken(request.atk);
                if (strUserID == null)
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", genRandomId(8)), Configuration.Formatters.JsonFormatter);

                var response = JToken.Parse("{\"dt\":true,\"err\":null}");
                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, new GetGameInfoResponse(1302, "Invalid player session", genRandomId(8)), Configuration.Formatters.JsonFormatter);
        }
        #endregion
        private string findUserIDFromToken(string strToken)
        {
            string[] strParts = strToken.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
            if (strParts.Length != 2)
                return null;

            return strParts[0];
        }
    }

    public class SpinRequest
    {
        public long     id  { get; set; }
        public double   cs  { get; set; }   //selectedBetSizeValue
        public int      ml  { get; set; }   //selectedBetLevelValue
        public string   wk  { get; set; }   //walletKey
        public int      fb  { get; set; }   //featurebuy
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
    public class SpinResponse
    {
        public dynamic      dt  { get; set; }
        public ErrorMsgData err { get; set; }

        public SpinResponse(int errorCode, string strErrMsg, string tid)
        {
            this.dt = null;
            this.err = new ErrorMsgData(errorCode, strErrMsg, tid);
        }
        public SpinResponse(dynamic dt)
        {
            this.dt     = dt;
            this.err    = null;
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

    public class WebLobbyGetRequest
    {
        public string otk   { get; set; }
        public string atk   { get; set; }
        public string lang  { get; set; }
        public string du    { get; set; }
        public string cc    { get; set; }
        public int    pf    { get; set; }
    }
    public class WebLobbyGetGameDetailsRequest
    {
        public string otk  { get; set; }
        public string lang { get; set; }
        public int    pf   { get; set; }
    }
    public class WebLobbyGetGameResourcesRequest
    {
        public string   otk     { get; set; }
        public string   lang    { get; set; }
        public int      pf      { get; set; }
        public string   du      { get; set; }
    }
    public class WebLobbyGetInitTournamnentsRequest
    {
        public string otk   { get; set; }
        public string atk   { get; set; }
        public string lang  { get; set; }
        public int    rc    { get; set; }
        public int    st    { get; set; }
        public int    pn    { get; set; }
        public int    pf    { get; set; }
        public int    barc  { get; set; }
    }
    public class WebLobbyRatingAddRequest
    {
        public string otk   { get; set; }
        public string atk   { get; set; }
        public int    gid   { get; set; }
        public int    pf    { get; set; }
        public int    r     { get; set; }
    }
    public class WebLobbyRatingRemoveRequest
    {
        public string   otk { get; set; }
        public string   atk { get; set; }
        public int      gid { get; set; }
        public int      pf  { get; set; }
    }
}
