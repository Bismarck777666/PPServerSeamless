using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;

namespace SlotGamesNode.Database
{
    public class DBProxyWriter : ReceiveActor
    {

        private string                      _strConnString                  = "";
        private readonly  ILoggingAdapter   _logger                         = Logging.GetLogger(Context);
        private IActorRef                   _workerChild                    = null;
        private bool                        _isShuttingDown                 = false;

        public DBProxyWriter(string strConnStriing)
        {
            _strConnString = strConnStriing;

            Receive<string>(mesasge => processCommand(mesasge));
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

            Receive<List<PGGameHistoryDBItem>>(_ => { WriterSnapshot.Instance.PushPGGameHistoryItems(_); });
            Receive<PGGameHistoryDBItem>(_ => { WriterSnapshot.Instance.insertPGGameHistory(_); });

            Receive<Terminated>(_ =>
            {
                _logger.Info("DBProxyWriter::Terminated");
                if(_isShuttingDown)
                {
                    if (_workerChild != null &&  _.ActorRef.Equals(_workerChild))
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
            else if(strCommand == "PopClaimedBonusItems")
            {
                List<BaseClaimedUserBonusUpdateItem> updateClaimedBonusItems = WriterSnapshot.Instance.PopClaimedBonusItems();
                Sender.Tell(updateClaimedBonusItems);
            }
            else if (strCommand == "PopPGGameHistoryItems")
            {
                Sender.Tell(WriterSnapshot.Instance.PopPGGameHistoryItems());
            }
            else if (strCommand == "terminate")
            {
                _isShuttingDown = true;
                _workerChild.Tell("flush");
                _workerChild.Tell(PoisonPill.Instance);
            }
        }

        protected override void PreStart()
        {
            base.PreStart();
            _workerChild                = Context.ActorOf(DBWriteWorker.Props(_strConnString), "writeWorker");
            Context.Watch(_workerChild);
        }

        protected override void PreRestart(Exception reason, object message)
        {
            Self.Tell(message);
            base.PreRestart(reason, message);
        }
    }


}
