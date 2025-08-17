using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;

namespace CommNode.Database
{
    public class DBProxyWriter : ReceiveActor
    {

        private string                      _strConnString                  = "";
        private readonly  ILoggingAdapter   _logger                         = Logging.GetLogger(Context);
        private IActorRef                   _workerChild                    = null;
        private IActorRef                   _workerChildForGameLog          = null;
        private IActorRef                   _workerChildForReport           = null;
        private bool                        _isShuttingDown                 = false;

        public DBProxyWriter(string strConnStriing)
        {
            _strConnString = strConnStriing;

            Receive<string>(mesasge => processCommand(mesasge));
            Receive<BaseInsertItem>(item =>
            {
                WriterSnapshot.Instance.insertDBItem(item);
            });
            Receive<PlayerBalanceUpdateItem>(updateItem =>
            {
                WriterSnapshot.Instance.updatePlayerBalanceUpdate(updateItem);
            });
            Receive<BaseClaimedUserBonusUpdateItem>(item =>
            {
                WriterSnapshot.Instance.claimBonusItem(item);
            });
            Receive<UserStateUpdateItem>(updateItem =>
            {
                WriterSnapshot.Instance.updatePlayerState(updateItem);
            });
            Receive<GameLogItem>(item =>
            {
                WriterSnapshot.Instance.insertGameLog(item);
            });
            Receive<UserTournamentBetItem>(item =>
            {
                WriterSnapshot.Instance.insertUserTournamentBetItem(item);
            });
            Receive<ReportUpdateItem>(updateItem =>
            {
                ReportSnapshot.Instance.updateReports(updateItem);
            });
            Receive<List<ReportUpdateItem>>(message =>
            {
                ReportSnapshot.Instance.PushReportUpdateItems(message);
            });
            Receive<GameReportItem>(updateItem =>
            {
                ReportSnapshot.Instance.updateGameReport(updateItem);
            });
            Receive<List<GameReportItem>>(message =>
            {
                ReportSnapshot.Instance.PushGameReportUpdates(message);
            });
            Receive<UserRollingAdded>(_ =>
            {
                ReportSnapshot.Instance.addUserRolling(_);
            });
            Receive<List<UserRollingAdded>>(_ =>
            {
                ReportSnapshot.Instance.PushUserRollingAdds(_);
            });
            Receive<AgentRollingAdded>(_ =>
            {
                ReportSnapshot.Instance.addAgentRolling(_);
            });
            Receive<List<AgentRollingAdded>>(_ =>
            {
                ReportSnapshot.Instance.PushAgentRollingAdds(_);
            });
            Receive<List<BaseInsertItem>>(message =>
            {
                WriterSnapshot.Instance.PushDBInsertItems(message);
            });
            Receive<List<BaseClaimedUserBonusUpdateItem>>(message =>
            { 
                WriterSnapshot.Instance.PushClaimedBonusItems(message);
            });                
            Receive<List<UserStateUpdateItem>>(message =>
            {
                WriterSnapshot.Instance.PushPlayerStates(message);
            });
            Receive<List<PlayerBalanceUpdateItem>>(message =>
            {
                WriterSnapshot.Instance.PushBalanceUpdateItems(message);
            });
            Receive<List<GameLogItem>>(message =>
            {
                WriterSnapshot.Instance.PushGameLogItems(message);
            });
            Receive<List<UserTournamentBetItem>>(message =>
            {
                WriterSnapshot.Instance.PushUserTournamentBetItems(message);
            });
            Receive<UserBetMoneyUpdateItem>(item =>
            {
                WriterSnapshot.Instance.updateUserBetMoney(item);
            });
            Receive<Terminated>(_ =>
            {
                _logger.Info("DBProxyWriter::Terminated");
                if(_isShuttingDown)
                {
                    if (_workerChild != null &&  _.ActorRef.Equals(_workerChild))
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
                }
            });
        }

        public static Props Props(string strConnString)
        {
            return Akka.Actor.Props.Create(() => new DBProxyWriter(strConnString));
        }


        private void processCommand(string strCommand)
        {
            if (strCommand == "PopPlayerStates")
            {
                List<UserStateUpdateItem> updatedPlayerStates = WriterSnapshot.Instance.PopPlayerStates();
                Sender.Tell(updatedPlayerStates);
            }
            else if (strCommand == "PopBalanceUpdates")
            {
                List<PlayerBalanceUpdateItem> updateBalanceItems = WriterSnapshot.Instance.PopBalanceUpdates();
                Sender.Tell(updateBalanceItems);
            }
            else if (strCommand == "PopGameLogItems")
            {
                List<GameLogItem> gameLogItems = WriterSnapshot.Instance.PopGameLogItems();
                Sender.Tell(gameLogItems);
            }
            else if (strCommand == "PopInsertItems")
            {
                List<BaseInsertItem> insertItems = WriterSnapshot.Instance.PopDBInsertItems();
                Sender.Tell(insertItems);
            }
            else if (strCommand == "PopReportUpdates")
            {
                List<ReportUpdateItem> updateReportItems = ReportSnapshot.Instance.PopReportUpdates();
                Sender.Tell(updateReportItems);
            }
            else if(strCommand == "PopClaimedBonusItems")
            {
                List<BaseClaimedUserBonusUpdateItem> updateClaimedBonusItems = WriterSnapshot.Instance.PopClaimedBonusItems();
                Sender.Tell(updateClaimedBonusItems);
            }
            else if(strCommand == "PopUserTournamentBetItems")
            {
                List<UserTournamentBetItem> items = WriterSnapshot.Instance.PopUserTournamentBetItems();
                Sender.Tell(items);
            }
            else if(strCommand == "PopUserRollingAdds")
            {
                List<UserRollingAdded> items = ReportSnapshot.Instance.PopUserRollingAdds();
                Sender.Tell(items);
            }
            else if (strCommand == "PopAgentRollingAdds")
            {
                List<AgentRollingAdded> items = ReportSnapshot.Instance.PopAgentRollingAdds();
                Sender.Tell(items);
            }
            else if(strCommand == "PopGameReportUpdates")
            {
                List<GameReportItem> items = ReportSnapshot.Instance.PopGameReportUpdates();
                Sender.Tell(items);
            }
            else if(strCommand == "PopUserBetMoneyUpdates")
            {
                List<UserBetMoneyUpdateItem> items = WriterSnapshot.Instance.PopUserBetMoneyUpdates();
                Sender.Tell(items);
            }
            else if (strCommand == "terminate")
            {
                _isShuttingDown = true;
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
            _workerChild                = Context.ActorOf(DBWriteWorker.Props(_strConnString), "writeWorker");
            _workerChildForGameLog      = Context.ActorOf(DBGameLogWriteWorker.Props(_strConnString), "writeWorkerForGameLog");
            _workerChildForReport       = Context.ActorOf(DBReportWriteWorker.Props(_strConnString), "writeWorkerForReport");
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
