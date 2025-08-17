using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using System.Web.Http;
using Microsoft.Owin;
using System.Net.Http;

[assembly: OwinStartup(typeof(CommNode.HTTPService.PPHistoryController))]
namespace CommNode.HTTPService
{
    [RoutePrefix("verify")]
    public class PPVerifyController : ApiController
    {
        [HttpGet]
        [Route("api/session")]
        public async Task<HttpResponseMessage> getLastItems()
        {
            string strToken = null;
            foreach (KeyValuePair<string, string> pair in Request.GetQueryNameValuePairs())
            {
                if (pair.Key == "mgckey")
                    strToken = pair.Value;
            }

            if (strToken == null)
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

            string strUserID = findUserIDFromToken(strToken);
            if (strUserID == null)
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };

            try
            {
                HttpResponseMessage response = await HTTPServiceConfig.Instance.HistoryWorkerGroup.Ask<HttpResponseMessage>(new HTTPPPVerifyGetLastItemRequest(strUserID, strToken), TimeSpan.FromSeconds(20));
                return response;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { Content = new StringContent(""), StatusCode = System.Net.HttpStatusCode.Unauthorized };
            }
        }

        private static string findUserIDFromToken(string strToken)
        {
            string[] strParts = strToken.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
            if (strParts.Length != 2)
                return null;

            return strParts[0];
        }
    }
}
