using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace QueenApiNode.Database
{
    internal class DBProxyWriter : ReceiveActor
    {
        private string                      _strConnString  = "";
        private readonly ILoggingAdapter    _logger         = Logging.GetLogger(Context);
        private IActorRef                   _workerChild    = null;
        private bool                        _isShuttingDown = false;

        public DBProxyWriter(string strConnStriing)
        {
            _strConnString = strConnStriing;

            Receive<string>(processCommand);
            Receive<AgentScoreUpdateItem>(_ =>
            {
                WriterSnapshot.Instance.updateAgentScore(_);
            });
            Receive<List<AgentScoreUpdateItem>>(_ =>
            {
                WriterSnapshot.Instance.pushAgentScoreUpdates(_);
            });
            Receive<AgentMoneyChangeItem>(_ =>
            {
                WriterSnapshot.Instance.insertAgentMoneyChangeItem(_);
            });
            Receive<List<AgentMoneyChangeItem>>(_ =>
            {
                WriterSnapshot.Instance.pushAgentMoneyChangeItems(_);
            });
            Receive<UserMoneyChangeItem>(_ =>
            {
                WriterSnapshot.Instance.insertUserMoneyChangeItem(_);
            });
            Receive<List<UserMoneyChangeItem>>(_ =>
            {
                WriterSnapshot.Instance.pushUserMoneyChangeItems(_);
            });
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
        private void processCommand(string strCommand)
        {
            if (strCommand == "PopAgentScoreUpdates")
            {
                List<AgentScoreUpdateItem> updateItems = WriterSnapshot.Instance.popAgentScoreUpdates();
                Sender.Tell(updateItems);
            }
            else if (strCommand == "PopAgentMoneyChangeItems")
            {
                List<AgentMoneyChangeItem> items = WriterSnapshot.Instance.popAgentMoneyChangeItems();
                Sender.Tell(items);
            }
            else if (strCommand == "PopUserMoneyChangeItems")
            {
                List<UserMoneyChangeItem> items = WriterSnapshot.Instance.popUserMoneyChangeItems();
                Sender.Tell(items);
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
            _workerChild = Context.ActorOf(DBProxyWriteWorker.Props(_strConnString), "writeWorker");
            Context.Watch(_workerChild);
        }
    }
}
