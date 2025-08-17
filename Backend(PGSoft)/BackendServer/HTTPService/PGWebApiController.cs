using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Formatting;
using System.Web.Http;
using Microsoft.Owin;
using System.Net.Http;
using SlotGamesNode.HTTPService.Models;
using Newtonsoft.Json;
using System.Net;
using Akka.Actor;
using Google.Protobuf.WellKnownTypes;
using System.Reflection.Emit;
using System.Security.Cryptography;
using GITProtocol;
using Newtonsoft.Json.Linq;
using SlotGamesNode.Database;
using Amazon.Runtime.Internal;

[assembly: OwinStartup(typeof(SlotGamesNode.HTTPService.PGWebApiController))]
namespace SlotGamesNode.HTTPService
{
    [RoutePrefix("web-api")]
    public class PGWebApiController : ApiController
    {
        [HttpPost]
        [Route("auth/session/v2/verifyOperatorPlayerSession")]
        public async Task<HttpResponseMessage> verifyOperatorSession([FromUri] string traceId, [FromBody] VerifyOperatorSessionRequest request )
        {
            if (string.IsNullOrEmpty(traceId))
                return Request.CreateResponse(HttpStatusCode.OK, new VerifySessionResponse(1700, "OERR: Operator return an error. Failed to verify operator player session", traceId), Configuration.Formatters.JsonFormatter);

            if (request == null || string.IsNullOrEmpty(request.os))
                return Request.CreateResponse(HttpStatusCode.OK, new VerifySessionResponse(1700, "OERR: Operator return an error. Failed to verify operator player session", traceId), Configuration.Formatters.JsonFormatter);

            string strUserID    = "";
            string strPassword  = "";
            if(!splitUserIDAndPassword(request.os, ref strUserID, ref strPassword))
                return Request.CreateResponse(HttpStatusCode.OK, new VerifySessionResponse(1700, "OERR: Operator return an error. Failed to verify operator player session", traceId), Configuration.Formatters.JsonFormatter);

            try
            {
                HTTVerifyResponse response = await HTTPServiceConfig.Instance.AuthWorkerGroup.Ask<HTTVerifyResponse>(new HTTPOperatorVerifyRequest(strUserID, strPassword, request.gi),
                                                TimeSpan.FromSeconds(10));

                if (response.Result == HttpVerifyResults.OK)
                {
                    VerifySessionResponse verifyResponse = new VerifySessionResponse();
                    verifyResponse.dt.geu                = string.Format("game-api/{0}/", response.GameStringID);
                    (verifyResponse.dt.gm[0] as PGGameStatusInfo).gid = response.GameID;
                    verifyResponse.dt.nkn                = response.NickName;
                    verifyResponse.dt.ioph               = response.UserID;
                    verifyResponse.dt.pcd                = response.UserID;
                    verifyResponse.dt.pid                = response.UserID;
                    verifyResponse.dt.tk                 = response.SessionToken;
                    return Request.CreateResponse(HttpStatusCode.OK, verifyResponse, Configuration.Formatters.JsonFormatter);
                }
            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, new VerifySessionResponse(1700, "OERR: Operator return an error. Failed to verify operator player session", traceId), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [Route("auth/session/v2/verifySession")]
        public async Task<HttpResponseMessage> verifySession([FromUri] string traceId, [FromBody] VerifySessionRequest request)
        {
            if (string.IsNullOrEmpty(traceId))
                return Request.CreateResponse(HttpStatusCode.OK, new VerifySessionResponse(1700, "OERR: Operator return an error. Failed to verify operator player session", traceId), Configuration.Formatters.JsonFormatter);

            if (request == null || string.IsNullOrEmpty(request.tk))
                return Request.CreateResponse(HttpStatusCode.OK, new VerifySessionResponse(1700, "OERR: Operator return an error. Failed to verify operator player session", traceId), Configuration.Formatters.JsonFormatter);


            try
            {

                HTTVerifyResponse response = await HTTPServiceConfig.Instance.AuthWorkerGroup.Ask<HTTVerifyResponse>(new HTTPVerifyRequest(strUserID, request.tk, request.gi),
                                                TimeSpan.FromSeconds(10));

                if (response.Result == HttpVerifyResults.OK)
                {
                    VerifySessionResponse verifyResponse = new VerifySessionResponse();
                    verifyResponse.dt.geu = string.Format("game-api/{0}/", response.GameStringID);
                    (verifyResponse.dt.gm[0] as PGGameStatusInfo).gid = response.GameID;
                    verifyResponse.dt.nkn                             = response.NickName;
                    verifyResponse.dt.ioph                            = response.UserID;
                    verifyResponse.dt.pcd                             = response.UserID;
                    verifyResponse.dt.pid                             = response.UserID;
                    verifyResponse.dt.tk                              = response.SessionToken;
                    verifyResponse.dt.
                    return Request.CreateResponse(HttpStatusCode.OK, verifyResponse, Configuration.Formatters.JsonFormatter);
                }
            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, new VerifySessionResponse(1700, "OERR: Operator return an error. Failed to verify operator player session", traceId), Configuration.Formatters.JsonFormatter);
        }

        [HttpPost]
        [Route("game-proxy/v2/GameRule/Get")]
        public async Task<HttpResponseMessage> doGetGameRule([FromUri] string traceId, [FromBody] GetGameRuleRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(traceId))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameRuleResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                if (string.IsNullOrEmpty(request.atk))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameRuleResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                string strUserID = findUserIDFromToken(request.atk);
                if (string.IsNullOrEmpty(strUserID))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameRuleResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                GITMessage message = new GITMessage(MsgCodes.GETGAMERULE);
                message.Append(request.gid);
                message.Append(traceId);

                object response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<object>(new FromHTTPSessionMessage(strUserID, request.atk, message), TimeSpan.FromSeconds(20));
                if (response is ToHTTPSessionMsgResults)
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameRuleResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);
                else
                    return Request.CreateResponse(HttpStatusCode.OK, response as GetGameRuleResponse, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new GetGameRuleResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);
            }
        }
        [HttpPost]
        [Route("game-proxy/v2/GameName/Get")]
        public async Task<HttpResponseMessage> doGetGameNames([FromUri] string traceId, [FromBody] GetGameNameRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(traceId))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameNameResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                if (string.IsNullOrEmpty(request.atk))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameNameResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                string strUserID = findUserIDFromToken(request.atk);
                if(string.IsNullOrEmpty(strUserID))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameNameResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                GITMessage          message     = new GITMessage(MsgCodes.GETGAMENAME);
                object              response    = await HTTPServiceConfig.Instance.WorkerGroup.Ask<object>(new FromHTTPSessionMessage(strUserID, request.atk, message), TimeSpan.FromSeconds(20));
                if (response is ToHTTPSessionMsgResults)
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameNameResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);
                else
                    return Request.CreateResponse(HttpStatusCode.OK, response as GetGameNameResponse, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new GetGameNameResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);
            }
        }

        [HttpPost]
        [Route("game-proxy/v2/GameWallet/Get")]
        public async Task<HttpResponseMessage> doGetGameWallet([FromUri] string traceId, [FromBody] GetGameWalletRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(traceId))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameWalletResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                if (string.IsNullOrEmpty(request.atk))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameWalletResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                string strUserID = findUserIDFromToken(request.atk);
                if (string.IsNullOrEmpty(strUserID))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameWalletResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                GITMessage message = new GITMessage(MsgCodes.GETGAMEWALLET);
                object response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<object>(new FromHTTPSessionMessage(strUserID, request.atk, message), TimeSpan.FromSeconds(20));
                if (response is ToHTTPSessionMsgResults)
                    return Request.CreateResponse(HttpStatusCode.OK, new GetGameWalletResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);
                else
                    return Request.CreateResponse(HttpStatusCode.OK, response as GetGameWalletResponse, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new GetGameNameResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);
            }
        }

        [HttpPost]
        [Route("game-proxy/v2/Resources/GetByResourcesTypeIds")]
        public async Task<HttpResponseMessage> doGetyResourcesTypeIds([FromUri] string traceId, [FromBody] GetByResourcesRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(traceId))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetByResourcesResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                if (string.IsNullOrEmpty(request.atk))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetByResourcesResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                string strUserID = findUserIDFromToken(request.atk);
                if(string.IsNullOrEmpty(strUserID))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetByResourcesResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                GITMessage message = new GITMessage(MsgCodes.GETGAMERESOURCE);

                object response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<object>(new FromHTTPSessionMessage(strUserID, request.atk, message), TimeSpan.FromSeconds(20));
                if(response is ToHTTPSessionMsgResults)
                    return Request.CreateResponse(HttpStatusCode.OK, new GetByResourcesResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);
                else
                    return Request.CreateResponse(HttpStatusCode.OK, response as GetByResourcesResponse, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new GetByResourcesResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);
            }
        }
        [HttpPost]
        [Route("game-proxy/v2/BetSummary/Get")]
        public async Task<HttpResponseMessage> doGetBetSummary([FromUri] string traceId, [FromBody] GetBetSummaryRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(traceId))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetBetSummaryResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                if (string.IsNullOrEmpty(request.atk))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetBetSummaryResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                string strUserID = findUserIDFromToken(request.atk);
                if (string.IsNullOrEmpty(strUserID))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetBetSummaryResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                GITMessage message = new GITMessage(MsgCodes.GETHISTORYSUMMARY);
                message.Append(request.gid);
                message.Append(request.dtf);
                message.Append(request.dtt);

                object response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<object>(new FromHTTPSessionMessage(strUserID, request.atk, message), TimeSpan.FromSeconds(20));
                if (response is ToHTTPSessionMsgResults)
                    return Request.CreateResponse(HttpStatusCode.OK, new GetBetSummaryResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);
                else
                    return Request.CreateResponse(HttpStatusCode.OK, response as GetBetSummaryResponse, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new GetBetSummaryResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);
            }
        }

        [HttpPost]
        [Route("game-proxy/v2/BetHistory/Get")]
        public async Task<HttpResponseMessage> doGetBetHistory([FromUri] string traceId, [FromBody] GetBetHistoryRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(traceId))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetBetHistoryResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                if (string.IsNullOrEmpty(request.atk))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetBetHistoryResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                string strUserID = findUserIDFromToken(request.atk);
                if (string.IsNullOrEmpty(strUserID))
                    return Request.CreateResponse(HttpStatusCode.OK, new GetBetHistoryResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);

                GITMessage message = new GITMessage(MsgCodes.GETHISTORYITEMS);
                message.Append(request.gid);
                message.Append(request.dtf);
                message.Append(request.dtt);
                message.Append(request.bn);
                message.Append(request.rc);


                object response = await HTTPServiceConfig.Instance.WorkerGroup.Ask<object>(new FromHTTPSessionMessage(strUserID, request.atk, message), TimeSpan.FromSeconds(20));
                if (response is ToHTTPSessionMsgResults)
                    return Request.CreateResponse(HttpStatusCode.OK, new GetBetHistoryResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);
                else
                    return Request.CreateResponse(HttpStatusCode.OK, response as GetBetHistoryResponse, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new GetBetHistoryResponse(1302, "Invalid player session", traceId), Configuration.Formatters.JsonFormatter);
            }
        }

        //Lobby API
        [HttpPost]
        [Route("auth/session/v1/verifyOperatorPlayerSession")]
        public async Task<HttpResponseMessage> verifyOperatorSessionV1()
        {
            VerifyOperatorSessionRequestV1 request = new VerifyOperatorSessionRequestV1();
            try
            {
                var formData = await Request.Content.ReadAsMultipartAsync();
                foreach(var content in formData.Contents)
                {
                    string  strData     = await content.ReadAsStringAsync();
                    var     paramName   = content.Headers.ContentDisposition.Name;
                    switch(paramName.Replace("\"", ""))
                    {
                        case "os":
                            request.os = strData;
                            break;
                        case "otk":
                            request.otk = strData;
                            break;
                        case "btt":
                            request.btt = int.Parse(strData);
                            break;
                        case "cp":
                            request.cp = strData;
                            break;
                        case "gi":
                            request.gi = int.Parse(strData);
                            break;

                    }
                }
                if (request == null || string.IsNullOrEmpty(request.os))
                    return Request.CreateResponse(HttpStatusCode.OK, new VerifySessionResponse(1700, "OERR: Operator return an error. Failed to verify operator player session", ""), Configuration.Formatters.JsonFormatter);

                string strUserID = "";
                string strPassword = "";
                if (!splitUserIDAndPassword(request.os, ref strUserID, ref strPassword))
                    return Request.CreateResponse(HttpStatusCode.OK, new VerifySessionResponse(1700, "OERR: Operator return an error. Failed to verify operator player session", ""), Configuration.Formatters.JsonFormatter);

                HTTVerifyResponse response = await HTTPServiceConfig.Instance.AuthWorkerGroup.Ask<HTTVerifyResponse>(new HTTPOperatorVerifyRequest(strUserID, strPassword, request.gi),
                                                TimeSpan.FromSeconds(10));

                if (response.Result == HttpVerifyResults.OK)
                {
                    VerifySessionResponse verifyResponse = new VerifySessionResponse();
                    verifyResponse.dt.geu       = string.Format("game-api/{0}/", response.GameStringID);
                    verifyResponse.dt.nkn       = response.NickName;
                    verifyResponse.dt.ioph      = response.UserID;
                    verifyResponse.dt.pcd       = response.UserID;
                    verifyResponse.dt.pid       = response.UserID;
                    verifyResponse.dt.tk        = response.SessionToken;
                    verifyResponse.dt.gm.Clear();

                    List<GAMEID> allGameIds =  PGGamesSnapshot.Instance.getAllGameIDs();
                    foreach(GAMEID gameID in allGameIds)
                    {
                        PGLobbyGameStatusInfo gameInfo = new PGLobbyGameStatusInfo();
                        gameInfo.gid                   = (int)gameID;
                        verifyResponse.dt.gm.Add(gameInfo);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, verifyResponse, Configuration.Formatters.JsonFormatter);
                }
            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, new VerifySessionResponse(1700, "OERR: Operator return an error. Failed to verify operator player session", ""), Configuration.Formatters.JsonFormatter);
        }

        private bool splitUserIDAndPassword(string strToken, ref string strUserID, ref string strPassword)
        {
            string[] strParts = strToken.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            if (strParts.Length != 2)
                return false;

            strUserID   = strParts[0];
            strPassword = strParts[1];
            return true;
        }

        private bool findUserIDFromToken(string strToken, ref int agentID, ref string strUserID)
        {
            string[] strParts = strToken.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
            if (strParts.Length != 2)
                return false;
    
            string[] strSubParts    = strParts[0].Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            agentID                 = int.Parse(strSubParts[0]);
            strUserID               = strSubParts[1];
            return true;
        }

    }
    public class GetBetHistoryRequest
    {
        public int      gid { get; set; }
        public long     dtf { get; set; }   //startDate
        public long     dtt { get; set; }   //endDate
        public string   atk { get; set; }
        public int      pf { get; set; }
        public string   wk { get; set; }
        public string   btt { get; set; }
        public int      bn { get; set; }     //history page number
        public int      rc { get; set; }     //count per page
    }
    public class GetBetHistoryResponse
    {
        public GetBetHistoryResponseData dt { get; set; }
        public ErrorMsgData             err { get; set; }

        public GetBetHistoryResponse(GetBetHistoryResponseData dt)
        {
            this.dt     = dt;
            this.err    = null;
        }
        public GetBetHistoryResponse(int errorCode, string strErrMsg, string tid)
        {
            this.dt     = null;
            this.err    = new ErrorMsgData(errorCode, strErrMsg, tid);
        }
    }
    public class GetBetHistoryResponseData
    {
        public List<dynamic> bh { get; set; }

        public GetBetHistoryResponseData()
        {
            this.bh = new List<dynamic>();
        }

    }
    public class GetBetSummaryRequest
    {
        public int      gid     { get; set; }
        public long     dtf     { get; set; }   //startDate
        public long     dtt     { get; set; }   //endDate
        public string   atk     { get; set; }
        public int      pf      { get; set; }
        public string   wk      { get; set; }
        public string   btt     { get; set; }
    }
    public class GetBetSummaryResponse
    {
        public GetBetSummaryResponseData    dt  { get; set; }
        public ErrorMsgData                 err { get; set; }
        public GetBetSummaryResponse(GetBetSummaryResponseData dt)
        {
            this.dt = dt;
            this.err = null;
        }
        public GetBetSummaryResponse(int errorCode, string strErrMsg, string tid)
        {
            this.dt = null;
            this.err = new ErrorMsgData(errorCode, strErrMsg, tid);
        }
    }
    public class GetBetSummaryResponseData
    {
        public BetSummaryData   bs  { get; set; }
        public long             lut { get; set; }   //last unix timestamp
        
    }
    public class BetSummaryData
    {
        public int      bc      { get; set; } //betCount
        public double   btba    { get; set; } //batchTotalBetAmount
        public double   btwla   { get; set; } //batchTotalWinLossAmount
        public int      gid     { get; set; }
        public long     lbid    { get; set; } //lastBetId
    }

    public class GetByResourcesRequest
    {
        public string   du      { get; set; }
        public int      rtids   { get; set; }  //typeid
        public string   otk     { get; set; }
        public int      btt     { get; set; }
        public string   wk      { get; set; }  //walletKey
        public string   atk     { get; set; }
        public int      pf      { get; set; }
        public int      gid     { get; set; }
    }
    public class GetByResourcesResponse
    {
        public dynamic      dt  { get; set; }
        public ErrorMsgData err { get; set; }

        public GetByResourcesResponse(dynamic dt)
        {
            this.dt  = dt;
            this.err = null;
        }
        public GetByResourcesResponse(int errorCode, string strErrMsg, string tid)
        {
            this.dt  = null;
            this.err = new ErrorMsgData(errorCode, strErrMsg, tid);
        }
    }
    public class GetGameNameRequest
    {
        public string   lang    { get; set; }
        public int      btt     { get; set; }
        public string   atk     { get; set; }
        public int      pf      { get; set; }
        public int      gid     { get; set; }
    }
    public class GetGameWalletRequest
    {
        public string wk { get; set; }
        public int btt { get; set; }
        public string atk { get; set; }
        public int pf { get; set; }
        public int gid { get; set; }
    }
    public class GetGameRuleRequest
    {
        public int btt { get; set; }
        public string atk { get; set; }
        public int pf { get; set; }
        public int gid { get; set; }
    }
    public class GetGameNameResponse
    {
        public Dictionary<string, string>   dt  { get; set; }
        public ErrorMsgData                 err { get; set; }

        public GetGameNameResponse(Dictionary<string, string> dt)
        {
            this.dt  = dt;
            this.err = null;
        }
        public GetGameNameResponse(int errorCode, string strErrMsg, string tid)
        {
            this.dt  = null;
            this.err = new ErrorMsgData(errorCode, strErrMsg, tid);
        }
    }
    public class GetGameWalletResponse
    {
        public JToken    dt  { get; set; }
        public ErrorMsgData err { get; set; }

        public GetGameWalletResponse(JToken dt)
        {
            this.dt  = dt;
            this.err = null;
        }
        public GetGameWalletResponse(int errorCode, string strErrMsg, string tid)
        {
            this.dt = null;
            this.err = new ErrorMsgData(errorCode, strErrMsg, tid);
        }
    }
    public class GetGameRuleResponse
    {
        public dynamic      dt { get; set; }
        public ErrorMsgData err { get; set; }

        public GetGameRuleResponse(dynamic dt)
        {
            this.dt = dt;
            this.err = null;
        }
        public GetGameRuleResponse(int errorCode, string strErrMsg, string tid)
        {
            this.dt = null;
            this.err = new ErrorMsgData(errorCode, strErrMsg, tid);
        }
    }
    public class VerifyOperatorSessionRequest
    {
        public int      btt     { get; set; }
        public int      vc      { get; set; }
        public int      pf      { get; set; }
        public string   l       { get; set; }
        public int      gi      { get; set; }
        public string   os      { get; set; }
        public string   otk     { get; set; }
    }
    public class VerifyOperatorSessionRequestV1
    {
        public int      btt     { get; set; }
        public string   cp      { get; set; }
        public int      gi      { get; set; }
        public string   os      { get; set; }
        public string   otk     { get; set; }
    }

    public class VerifySessionRequest
    {
        public int btt { get; set; }
        public int vc { get; set; }
        public int pf { get; set; }
        public string l { get; set; }
        public int gi { get; set; }
        public string tk { get; set; }
        public string otk { get; set; }
    }
    public class VerifySessionResponse
    {
        public VerifySessionResponseData    dt  { get; set; }
        public ErrorMsgData                 err { get; set; }

        public VerifySessionResponse()
        {
            this.dt  = new VerifySessionResponseData();
            this.err = null;
        }
        public VerifySessionResponse(int errorCode, string strErrMsg, string tid)
        {
            this.dt  = null;
            this.err = new ErrorMsgData(errorCode, strErrMsg, tid);
        }
    }
    public class VerifySessionResponseData
    {
        public string                   bau     { get; set; } //betHistoryApiUrl
        public string                   cc      { get; set; } //currencyCode
        public string                   cs      { get; set; } //currencySymbol
        public List<string>             ec      { get; set; } //elementCategory
        public string                   gcv     { get; set; } //gameCertificateVersion
        public string                   geu     { get; set; } //gameEngineUrl
        public List<object>             gm      { get; set; }
        public string                   ioph    { get; set; }  //idOperatorPlayerHash
        public string                   lau     { get; set; }   //lobbyApiUrl
        public string                   nkn     { get; set; }   //nickname
        public OperatorCustomConfig     occ     { get; set; }
        public OperatorJuridication     oj      { get; set; }
        public string                   pcd     { get; set; } //player name
        public string                   pid     { get; set; } //playerId
        public int                      st      { get; set; } //sessionStatus
        public string                   tk      { get; set; } //sessionToken

        public UIOperatorGameComponents uiogc   { get; set; }

        public VerifySessionResponseData()
        {
            this.bau    = "web-api/game-proxy/";
            this.cc     = "MYR";
            this.cs     = "RM";
            this.gcv    = "1.2.0.1";
            this.gm     = new List<object>();
            this.gm.Add(new PGGameStatusInfo());
            this.lau    = "/game-api/lobby/";
            this.occ    = new OperatorCustomConfig();
            this.oj     = new OperatorJuridication();
            this.st     = 1;
            this.uiogc  = new UIOperatorGameComponents();
            this.ec     = new List<string>();
        }
    }
    public class PGLobbyGameStatusInfo
    {
        public int      gid         { get; set; } //gameid
        public long     msdt        { get; set; } //maintenanceStartDate
        public long     medt        { get; set; } //maintenanceEndDate
        public int      st          { get; set; } //gameStatus
        public string   amsg        { get; set; }

        public PGLobbyGameStatusInfo()
        {
            this.gid  = 0;
            this.msdt = 1536917400000;
            this.msdt = 1545714000000;
            this.st   = 1;
            this.amsg = "";
        }
    }
    public class PGGameStatusInfo
    {
        public int                          gid     { get; set; } //gameid
        public long                         msdt    { get; set; } //maintenanceStartDate
        public long                         medt    { get; set; } //maintenanceEndDate
        public int                          st      { get; set; } //gameStatus
        public int                          mxe     { get; set; } //maxPayout
        public long                         mxehr   { get; set; } //maxPayoutProbability
        public Dictionary<string, RTPInfo>  rtp     { get; set; } //rtp

        public PGGameStatusInfo()
        {
            this.msdt  = 1597288446000;
            this.medt  = 1597288446000;
            this.mxe   = 9071;
            this.mxehr = 1000000000;
            this.rtp   = new Dictionary<string, RTPInfo>();
            this.rtp.Add("df", new RTPInfo());
            this.st    = 1;
        }
    }
    public class RTPInfo
    {
        public double min { get; set; }
        public double max { get; set; }

        public RTPInfo()
        {
            this.min = 96.71;
            this.max = 96.71;

        }
    }
    public class OperatorCustomConfig
    {
        public string   rurl { get; set; } //realURL
        public string   tcm  { get; set; } //dialogMessage
        public int      tsc  { get; set; } //triggerSpinCount
        public int      ttp  { get; set; } //triggerDuration
        public string   tlb  { get; set; } //leftButtonLabel
        public string   trb  { get; set; } //rightButtonLabel
        public OperatorCustomConfig()
        {
            this.rurl = "";
            this.tcm  = "";
            this.tsc  = 0;
            this.tlb  = "";
            this.trb  = "";
            this.ttp  = 0;
        }
    }
    public class OperatorJuridication
    {
        public int jid { get; set; }
        public OperatorJuridication()
        {
            jid = 1;
        }
    }

    public class UIOperatorGameComponents
    {
        public int bb   { get; set; }   //backButton
        public int grtp { get; set; }   //gameReturnToPlayer
        public int gec  { get; set; }   //gameExitConfirmation
        public int cbu { get; set; }    //currencyBaseUnit
        public int cl { get; set; }
        public int bf { get; set; }     //buyFeature
        public int mr { get; set; }     //markRead
        public int phtr { get; set; }
        public int vc { get; set; }
        public int bfbsi { get; set; }
        public int bfbli { get; set; }
        public int il { get; set; }
        public int rp { get; set; }     //replayVersion
        public int gc { get; set; }     //gameClock
        public int ign { get; set; }    //gameName
        public int tsn { get; set; }    //turboSpinSuggest
        public int we { get; set; }     //walletSocketEnable
        public int gsc { get; set; }    //globalSocketEnable
        public int bu { get; set; }     //balanceUpdateEnable
        public int pwr { get; set; }    //newWalletNotificationEnable
        public int hd { get; set; }     //hideCurrencyDecimal
        public int et { get; set; }     //elapsedTimeState
        public int np { get; set; }     //netProfitState
        public int igv { get; set; }    //gameVersion

        [JsonProperty(PropertyName = "as")]
        public int as1 { get; set; }    //autoPlayMaxNum
        public int asc { get; set; }    //autoPlayConfig
        public int std { get; set; }    //singlePlayMinDuration
        public int hnp { get; set; }    //hideNonProfitEffect
        public int ts { get; set; }     //turboSpinEnable
        public int smpo { get; set; }   //maxPayoutEnable
        public int ivs { get; set; }
        public int hn { get; set; }

        public UIOperatorGameComponents()
        {
            this.as1    = 0;
            this.asc    = 0;
            this.bb     = 1;
            this.bf     = 1;
            this.bfbli  = 2;
            this.bfbsi  = 2;
            this.bu     = 0;
            this.cbu    = 0;
            this.cl     = 0;
            this.et     = 0;
            this.gc     = 1;
            this.gec    = 0;
            this.grtp   = 1;
            this.gsc    = 0;
            this.hd     = 0;
            this.hn     = 1;
            this.hnp    = 0;
            this.ign    = 1; //1: show game name, 0: hide
            this.igv    = 0;
            this.il     = 0;
            this.mr     = 0;
            this.np     = 0;
            this.phtr   = 0;
            this.pwr    = 0;
            this.rp     = 0;
            this.smpo   = 0;
            this.std    = 0;
            this.ts     = 0;
            this.tsn    = 0;
            this.vc     = 0;
            this.we     = 0;

        }
    }
}
