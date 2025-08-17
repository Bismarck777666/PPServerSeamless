using Akka.Actor;
using SlotGamesNode.Jackpot;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

[assembly: OwinStartup(typeof(SlotGamesNode.HTTPService.PGJackpotApiController))]
namespace SlotGamesNode.HTTPService
{
    [RoutePrefix("jackpot-api")]
    public class PGJackpotApiController : ApiController
    {
        [HttpGet]
        [Route("getGameJackpotTheme")]
        public async Task<HttpResponseMessage> getGameJackpotTheme(int gameId)
        {
            if (gameId <= 0)
                return Request.CreateResponse(HttpStatusCode.OK, new GameJackpotThemeResult(0, 0, 0), Configuration.Formatters.JsonFormatter);

            try
            {
                GameJackpotThemeResult response = await HTTPServiceConfig.Instance.JackpotActor.Ask<GameJackpotThemeResult>(new GetGameJackpotThemeRequest(gameId),
                                                TimeSpan.FromSeconds(10));

                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception)
            {
            }
            return Request.CreateResponse(HttpStatusCode.OK, new GameJackpotThemeResult(0, 0, 0), Configuration.Formatters.JsonFormatter);
        }

        [HttpGet]
        [Route("getGameJackpotInfo")]
        public async Task<HttpResponseMessage> getGameJackpotInfo(int gameId)
        {
            if (gameId <= 0)
                return Request.CreateResponse(HttpStatusCode.OK, new List<GameJackpotTypeInfo>(), Configuration.Formatters.JsonFormatter);

            try
            {
                List<GameJackpotTypeInfo> response = await HTTPServiceConfig.Instance.JackpotActor.Ask<List<GameJackpotTypeInfo>>(new GetGameJackpotInfoRequest(gameId),
                                                TimeSpan.FromSeconds(10));

                return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, new List<GameJackpotTypeInfo>(), Configuration.Formatters.JsonFormatter);
        }
    }

    public class GameJackpotThemeResult
    {
        public int result       { get; set; }
        public int themeid      { get; set; }
        public int typeCount    { get; set; }

        public GameJackpotThemeResult(int result, int themeid, int typeCount)
        {
            this.result     = result;
            this.themeid    = themeid;
            this.typeCount  = typeCount;
        }
    }
}
