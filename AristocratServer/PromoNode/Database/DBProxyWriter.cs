using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using System.Data.SqlClient;

namespace PromoNode.Database
{
    public class DBProxyWriter : ReceiveActor
    {

        private string                       _strConnString      = "";
        private ICancelable                  _schedulerCancel    = null;
        private readonly    ILoggingAdapter  _logger = Logging.GetLogger(Context);

        public DBProxyWriter(string strConnStriing)
        {
            _strConnString = strConnStriing;
            ReceiveAsync<string>                (processCommand);
        }

        public static Props Props(string strConnString)
        {
            return Akka.Actor.Props.Create(() => new DBProxyWriter(strConnString));
        }

        protected override void PreStart()
        {
            if(_schedulerCancel != null)
                _schedulerCancel.Cancel();

            _schedulerCancel = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(10, 1000, Self, "write", ActorRefs.NoSender);
        }

        private async Task processCommand(string strCommand)
        {
            if(strCommand == "write")
            {
                try
                {

                }
                catch (Exception ex)
                {
                    _logger.Error("Exception has been occured in DBProxyWriter::processCommand {0}", ex.ToString());
                }
            }
        }

        protected override void PreRestart(Exception reason, object message)
        {
            Self.Tell(message);
            base.PreRestart(reason, message);
        }

        protected override void PostStop()
        {
            if (_schedulerCancel != null)
            {
                _schedulerCancel.Cancel();
                _schedulerCancel = null;
            }
            base.PostStop();
        }
    }   
}
