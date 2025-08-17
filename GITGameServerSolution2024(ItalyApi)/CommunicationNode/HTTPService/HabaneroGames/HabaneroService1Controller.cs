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
    [RoutePrefix("gitapi/habanero/service1")]
    public class HabaneroService1Controller : ApiController
    {
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
                    case "spin":
                        requestMessage = buildGameMessage(strContent, out strToken);
                        break;
                    case "client":
                        requestMessage = buildSelectOptMessage(strContent, out strToken);
                        break;
                    case "pickgamedonepay":
                        requestMessage = buildPickGameDonePayMessage(strContent, out strToken);
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
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_HABANERO_DOSPIN);
                message.Append(gameRequest.game.sessionid);
                message.Append(gameRequest.grid);
                token = Convert.ToString(gameRequest.header["player"]["ssotoken"]);
                message.Append(token);
                float coinValue = (float)Convert.ToDouble(gameRequest.game.play["betstate"]["bet"][0]["stake"]);
                int lineCount   = Convert.ToInt32(gameRequest.game.play["betstate"]["bet"][0]["numpaylines"]);
                int betLevel    = Convert.ToInt32(gameRequest.game.play["betstate"]["bet"][0]["betlevel"]);
                message.Append(coinValue);
                message.Append(lineCount);
                message.Append(betLevel);
                return message;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private GITMessage buildSelectOptMessage(string strContent,out string token)
        {
            token = null;
            try
            {
                HabaneroRequest gameRequest = JsonConvert.DeserializeObject<HabaneroRequest>(strContent);
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_HABANERO_DOCLIENT);
                message.Append(gameRequest.game.sessionid);
                message.Append(gameRequest.grid);
                token = Convert.ToString(gameRequest.header["player"]["ssotoken"]);
                message.Append(token);
                int clientpickdata = Convert.ToInt32(gameRequest.game.clientpickdata);
                message.Append(clientpickdata);
                return message;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private GITMessage buildPickGameDonePayMessage(string strContent, out string token)
        {
            token = null;
            try
            {
                HabaneroRequest gameRequest = JsonConvert.DeserializeObject<HabaneroRequest>(strContent);
                GITMessage message = new GITMessage((ushort)CSMSG_CODE.CS_HABANERO_DOPICKGAMEDONEPAY);
                message.Append(gameRequest.game.sessionid);
                message.Append(gameRequest.grid);
                token = Convert.ToString(gameRequest.header["player"]["ssotoken"]);
                message.Append(token);
                return message;
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
}
