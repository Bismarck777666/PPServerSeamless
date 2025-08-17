using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using System.Data.SqlClient;
using System.Data.Common;
using GITProtocol;
using SlotGamesNode.GameLogics;
using Akka.Routing;
using Akka.Event;
using Newtonsoft.Json;
using Akka.Configuration;

namespace SlotGamesNode.Database
{
    public class RedisMonitor : ReceiveActor
    {
        private ICancelable                 _monitorCancelable      = null;
        private readonly ILoggingAdapter    _logger                 = Logging.GetLogger(Context);

        public RedisMonitor()
        {
            ReceiveAsync<string>(processCommand);
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new RedisMonitor());
        }

        protected override void PreStart()
        {
            base.PreStart();
        }

        protected override void PostStop()
        {
            if (_monitorCancelable != null)
                _monitorCancelable.Cancel();

            base.PostStop();
        }

        protected override void PostRestart(Exception reason)
        {
            base.PostRestart(reason);
            Self.Tell("start");
        }

        private async Task processCommand(string strCommand)
        {
            if(strCommand == "start")
            {
                //_monitorCancelable = Context.System.Scheduler.ScheduleTellOnceCancelable(5000, Self, "monitor", ActorRefs.NoSender);
            }
            else if(strCommand == "monitor")
            {
                await monitorTables();
            }
        }
        
        private async Task monitorTables()
        {
            try
            {
                string strKey       = "CQ9Promotion";
                if(await RedisDatabase.RedisCache.KeyExistsAsync(strKey))
                {
                    string promotion = await RedisDatabase.RedisCache.StringGetAsync(strKey);
                    CQ9Config.Instance.Promotion = JsonConvert.DeserializeObject<CQ9PromotionItem[]>(promotion);
                }

                strKey                  = "CQ9RecommendList";
                if (await RedisDatabase.RedisCache.KeyExistsAsync(strKey))
                {
                    //API게임때는 이용안함
                    //string recommendList    = await RedisDatabase.RedisCache.StringGetAsync(strKey);
                    //CQ9Config.Instance.RecommendList    = JsonConvert.DeserializeObject<CQ9RecommendItem>(recommendList);
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in RedisMonitor::monitorTables {0}", ex);
            }
            _monitorCancelable = Context.System.Scheduler.ScheduleTellOnceCancelable(120000, Self, "monitor", ActorRefs.NoSender);
        }
    }
}
