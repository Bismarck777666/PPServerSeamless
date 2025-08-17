using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using SlotGamesNode.GameLogics;

namespace SlotGamesNode.Database
{
    public class DBProxyWriter : ReceiveActor
    {

        private string                       _strConnString      = "";
        private ICancelable                  _schedulerCancel    = null;
        private readonly ILoggingAdapter     _logger             = Logging.GetLogger(Context);
        private IActorRef                    _workerChild        = null;
        private bool                         _isShuttingDown     = false;

        public DBProxyWriter(string strConnStriing)
        {
            _strConnString = strConnStriing;
            Receive<string>                         (processCommand);
            Receive<PPGameHistoryDBItem>            (_ => { WriterSnapshot.Instance.insertPPGameHistory(_);     });
            Receive<List<PPGameHistoryDBItem>>      (_ => { WriterSnapshot.Instance.PushPPGameHistoryItems(_);  });
            Receive<PPGameRecentHistoryDBItem>      (_ => { WriterSnapshot.Instance.insertPPRecentGameHistory(_); });
            Receive<List<PPGameRecentHistoryDBItem>>(_ => { WriterSnapshot.Instance.PushPPRecentGameHistoryItems(_); });
            
            Receive<BNGHistoryItem>                 (_ => { WriterSnapshot.Instance.insertBNGGameHistory(_); });
            Receive<List<BNGHistoryItem>>           (_ => { WriterSnapshot.Instance.PushBNGGameHistoryItems(_); });
            
            Receive<CQ9GameLogItem>                 (_ => { WriterSnapshot.Instance.insertCQ9GameHistory(_); });
            Receive<List<CQ9GameLogItem>>           (_ => { WriterSnapshot.Instance.PushCQ9GameHistoryItems(_); });
            
            Receive<HabaneroLogItem>                (_ => { WriterSnapshot.Instance.insertHabaneroGameHistory(_); });
            Receive<List<HabaneroLogItem>>          (_ => { WriterSnapshot.Instance.PushHabaneroGameHistoryItems(_); });

            Receive<PlaysonHistoryItem>             (_ => { WriterSnapshot.Instance.insertPlaysonGameHistory(_); });
            Receive<List<PlaysonHistoryItem>>       (_ => { WriterSnapshot.Instance.PushPlaysonGameHistoryItems(_); });

            Receive<Terminated>(_ =>
            {
                _logger.Info("DBProxyWriter::Terminated");
                if (_isShuttingDown)
                {
                    if (_workerChild != null && _.ActorRef.Equals(_workerChild))
                        _workerChild = null;

                    if (_workerChild == null)
                    {
                        _logger.Info("DB Proxy Terminated : Ended");
                        Self.Tell(PoisonPill.Instance);
                    }
                }

            });
        }

        public static Props Props(string strConnString)
        {
            return Akka.Actor.Props.Create(() => new DBProxyWriter(strConnString));
        }

        protected override void PreStart()
        {
            base.PreStart();
            _workerChild = Context.ActorOf(DBProxyWriteWorker.Props(_strConnString), "writeWorker");
        }

        private void processCommand(string strCommand)
        {
            if (strCommand == "PopPPGameHistoryItems")
            {
                Sender.Tell(WriterSnapshot.Instance.PopPPGameHistoryItems());
            }
            else if (strCommand == "PopPPRecentGameHistoryItems")
            {
                Sender.Tell(WriterSnapshot.Instance.PopPPRecentGameHistoryItems());
            }
            else if(strCommand == "PopBNGGameHistoryItems")
            {
                Sender.Tell(WriterSnapshot.Instance.PopBNGGameHistoryItems());
            }
            else if (strCommand == "PopCQ9GameHistoryItems")
            {
                Sender.Tell(WriterSnapshot.Instance.PopCQ9GameHistoryItems());
            }
            else if (strCommand == "PopHabaneroGameHistoryItems")
            {
                Sender.Tell(WriterSnapshot.Instance.PopHabaneroGameHistoryItems());
            }
            else if (strCommand == "PopPlaysonGameHistoryItems")
            {
                Sender.Tell(WriterSnapshot.Instance.PopPlaysonGameHistoryItems());
            }
            else if (strCommand == "terminate")
            {
                _isShuttingDown = true;
                _workerChild.Tell("flush");
                _workerChild.Tell(PoisonPill.Instance);
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
