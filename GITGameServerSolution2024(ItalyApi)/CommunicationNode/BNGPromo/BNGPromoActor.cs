using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using CommNode.Database;
namespace CommNode
{
    public class BNGPromoActor : ReceiveActor
    {
        public BNGPromoActor()
        {
            ReceiveAsync<string>(onCommand);
        }
        private async Task onCommand(string command)
        {
            if (command != "fetch")
                return;

            try
            {
                string strPromo = await RedisDatabase.RedisCache.StringGetAsync("bngPromo");
                if(strPromo != null)
                BNGPromoSnapshot.SnapShot = strPromo;
            }
            catch(Exception)
            {

            }
            Context.System.Scheduler.ScheduleTellOnce(1000, Self, "fetch", Self);
        }
    }
}
