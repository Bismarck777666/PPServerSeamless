using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace UserNode.Database
{
    public class DBProxyWriter : ReceiveActor
    {
        private string                      _strConnString = "";
        private readonly ILoggingAdapter    _logger                 = Context.GetLogger();
        private IActorRef                   _workerChild            = null;
        private IActorRef                   _workerChildForGameLog  = null;
        private IActorRef                   _workerChildForReport   = null;
        private bool                        _isShuttingDown         = false;

        public DBProxyWriter(string strConnStriing)
        {
            _strConnString = strConnStriing;

            Receive<string>(mesasge => processCommand(mesasge));
            Receive<BaseInsertItem>                 (item   => WriterSnapshot.Instance.insertDBItem(item));
            Receive<PlayerBalanceUpdateItem>        (item   => WriterSnapshot.Instance.updatePlayerBalanceUpdate(item));
            Receive<CompanyBalanceUpdateItem>       (item   => WriterSnapshot.Instance.updateCompanyBalanceUpdate(item));
            Receive<UserStateUpdateItem>            (item   => WriterSnapshot.Instance.updatePlayerState(item));
            Receive<GameLogItem>                    (item   => WriterSnapshot.Instance.insertGameLog(item));
            Receive<ReportUpdateItem>               (item   => ReportSnapshot.Instance.updateReports(item));
            Receive<List<ReportUpdateItem>>         (item   => ReportSnapshot.Instance.PushReportUpdateItems(item));
            Receive<GameReportItem>                 (item   => ReportSnapshot.Instance.updateGameReport(item));
            Receive<List<GameReportItem>>           (item   => ReportSnapshot.Instance.PushGameReportUpdates(item));
            Receive<List<BaseInsertItem>>           (item   => WriterSnapshot.Instance.PushDBInsertItems(item));
            Receive<List<UserStateUpdateItem>>      (item   => WriterSnapshot.Instance.PushPlayerStates(item));
            Receive<List<PlayerBalanceUpdateItem>>  (item   => WriterSnapshot.Instance.PushBalanceUpdateItems(item));
            Receive<List<GameLogItem>>              (item   => WriterSnapshot.Instance.PushGameLogItems(item));
            Receive<FetchUserBalanceUpdate>         (item   => 
            {
                Sender.Tell(WriterSnapshot.Instance.fetchUserBalanceUpdates(item.UserDBID));
            });
            Receive<ApiTransactionUpsertItem>       (item   => WriterSnapshot.Instance.insertApiTransaction(item));
            Receive<List<ApiTransactionUpsertItem>> (items  => WriterSnapshot.Instance.PushApiTransactionItems(items));
            Receive<Terminated>(_ =>
            {
                _logger.Info("DBProxyWriter::Terminated");
                if (!_isShuttingDown)
                    return;

                if (_workerChild != null && _.ActorRef.Equals(_workerChild))
                    _workerChild = null;

                if (_workerChildForGameLog != null && _.ActorRef.Equals(_workerChildForGameLog))
                    _workerChildForGameLog = null;

                if (_workerChildForReport != null && _.ActorRef.Equals(_workerChildForReport))
                    _workerChildForReport = null;

                if (_workerChild == null && _workerChildForGameLog == null && _workerChildForReport == null)
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

        private void processCommand(string strCommand)
        {
            if(strCommand == "PopPlayerStates")
            {
                Sender.Tell(WriterSnapshot.Instance.PopPlayerStates());
            }
            else if(strCommand == "PopBalanceUpdates")
            {
                Sender.Tell(WriterSnapshot.Instance.PopBalanceUpdates());
            }
            else if(strCommand == "PopCompanyBalanceUpdates")
            {
                Sender.Tell(WriterSnapshot.Instance.PopCompanyBalanceUpdates());
            }
            else if(strCommand == "PopGameLogItems")
            {
                Sender.Tell(WriterSnapshot.Instance.PopGameLogItems());
            }
            else if(strCommand == "PopInsertItems")
            {
                Sender.Tell(WriterSnapshot.Instance.PopDBInsertItems());
            }
            else if (strCommand == "PopReportUpdates")
            {
                Sender.Tell(ReportSnapshot.Instance.PopReportUpdates());
            }
            else if(strCommand == "PopGameReportUpdates")
            {
                Sender.Tell(ReportSnapshot.Instance.PopGameReportUpdates());
            }
            else if(strCommand == "PopApiTransactionItems")
            {
                Sender.Tell(WriterSnapshot.Instance.fetchApiTransactionItems());
            }
            else if(strCommand == "terminate")
            {
                _isShuttingDown     = true;

                _workerChild.Tell("flush");
                _workerChild.Tell(PoisonPill.Instance);
                
                _workerChildForGameLog.Tell("flush");
                _workerChildForGameLog.Tell(PoisonPill.Instance);
                
                _workerChildForReport.Tell("flush");
                _workerChildForReport.Tell(PoisonPill.Instance);
            }
        }

        protected override void PreStart()
        {
            base.PreStart();
            
            _workerChild            = Context.ActorOf(DBWriteWorker.Props(_strConnString),          "writeWorker");
            _workerChildForGameLog  = Context.ActorOf(DBGameLogWriteWorker.Props(_strConnString),   "writeWorkerForGameLog");
            _workerChildForReport   = Context.ActorOf(DBReportWriteWorker.Props(_strConnString),    "writeWorkerForReport");
            
            Context.Watch(_workerChild);
            Context.Watch(_workerChildForGameLog);
            Context.Watch(_workerChildForReport);
        }

        protected override void PreRestart(Exception reason, object message)
        {
            Self.Tell(message);
            base.PreRestart(reason, message);
        }
    }
}
