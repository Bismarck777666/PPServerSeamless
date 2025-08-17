using Akka.Actor;
using Akka.Event;
using SlotGamesNode.GameLogics;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SlotGamesNode.Database
{
    public class DBProxyWriter : ReceiveActor
    {
        private string                      _strConnString      = "";
        private ICancelable                 _schedulerCancel    = null;
        private readonly ILoggingAdapter    _logger             = Context.GetLogger();
        private IActorRef                   _workerChild        = null;
        private bool                        _isShuttingDown     = false;

        public DBProxyWriter(string strConnStriing)
        {
            _strConnString = strConnStriing;

            Receive<string>                         (new Action<string>(processCommand));
            Receive<PPGameHistoryDBItem>            (_ => WriterSnapshot.Instance.insertPPGameHistory(_));
            Receive<List<PPGameHistoryDBItem>>      (_ => WriterSnapshot.Instance.PushPPGameHistoryItems(_));
            Receive<PPGameRecentHistoryDBItem>      (_ => WriterSnapshot.Instance.insertPPRecentGameHistory(_));
            Receive<List<PPGameRecentHistoryDBItem>>(_ => WriterSnapshot.Instance.PushPPRecentGameHistoryItems(_));
            Receive<UpdatePayoutPoolStatus>         (_ => WriterSnapshot.Instance.updatePayoutPoolStatus(_));
            Receive<Terminated>                     (_ =>
            {
                _logger.Info("DBProxyWriter::Terminated");
                if (!_isShuttingDown)
                    return;

                if (_workerChild != null && _.ActorRef.Equals(_workerChild))
                    _workerChild = null;

                if (_workerChild == null)
                {
                    _logger.Info("DB Proxy Terminated : Ended");
                    Self.Tell(PoisonPill.Instance);
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
            if(strCommand == "PopPPGameHistoryItems")
            {
                Sender.Tell(WriterSnapshot.Instance.PopPPGameHistoryItems());
            }
            else if(strCommand == "PopPPRecentGameHistoryItems")
            {
                Sender.Tell(WriterSnapshot.Instance.PopPPRecentGameHistoryItems());
            }
            else if(strCommand == "PopUpdatePayoutPoolStatus")
            {
                Sender.Tell(WriterSnapshot.Instance.PopUpdatePayoutPoolStatus());
            }
            else if(strCommand == "terminate")
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
