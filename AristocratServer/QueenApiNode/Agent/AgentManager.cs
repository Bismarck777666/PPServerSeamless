using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QueenApiNode.Database;

namespace QueenApiNode.Agent
{
    class AgentManager : ReceiveActor
    {
        private readonly ILoggingAdapter        _logger            = Logging.GetLogger(Context);
        private bool                            _isShuttingDown    = false;
        private Dictionary<string, IActorRef>   _agentActors       = new Dictionary<string, IActorRef>();
        private Dictionary<string, string>      _authTokenMaps     = new Dictionary<string, string>();

        public AgentManager()
        {
            Receive<CreateNewAgentActorMsg>(message =>
            {
                IActorRef agentActor = Context.Child(message.AgentID);
                if (agentActor != ActorRefs.Nobody)
                {
                    Sender.Tell(agentActor);
                    return;
                }
                agentActor = Context.ActorOf(AgentActor.Props(message), message.AgentID);
                Context.Watch(agentActor);
                _agentActors[message.AgentID] = agentActor;
                Sender.Tell(agentActor);
            });
            Receive<GetAgentActorMsg>(message =>
            {
                var websiteActor = Context.Child(message.AgentID);
                Sender.Tell(websiteActor);
            });
            Receive<Terminated>(terminated =>
            {
                _agentActors.Remove(terminated.ActorRef.Path.Name);
                if (_isShuttingDown && _agentActors.Count == 0)
                    Context.Stop(Self);
            });
            Receive<Dictionary<string, string>> (onAgentAuthTokens);
            Receive<AuthTokenUpdated>           (onAuthTokenUpdated);
            Receive<string>                     (onGetAgentID);
        }
        private void onAgentAuthTokens(Dictionary<string, string> agentAuthTokens)
        {
            _authTokenMaps.Clear();
            foreach (KeyValuePair<string, string> pair in agentAuthTokens)
                _authTokenMaps[pair.Value] = pair.Key;
        }
        private void onAuthTokenUpdated(AuthTokenUpdated updated)
        {
            string strAuthKey = null;
            foreach (KeyValuePair<string, string> pair in _authTokenMaps)
            {
                if (pair.Value == updated.AgentID)
                {
                    strAuthKey = pair.Key;
                    break;
                }
            }

            if(updated.State == 1)
            {
                if (strAuthKey == null)
                {
                    _authTokenMaps[updated.AuthToken] = updated.AgentID;
                }
                else
                {
                    _authTokenMaps.Remove(strAuthKey);
                    _authTokenMaps[updated.AuthToken] = updated.AgentID;
                }

                Context.Child(updated.AgentID).Tell(updated);
            }
            else
            {
                _authTokenMaps.Remove(strAuthKey);

                var childActor = Context.Child(updated.AgentID);
                if (childActor != ActorRefs.Nobody)
                    childActor.Tell(PoisonPill.Instance);
            }
        }

        private void onGetAgentID(string strAuthToken)
        {
            if (_authTokenMaps.ContainsKey(strAuthToken))
                Sender.Tell(_authTokenMaps[strAuthToken]);
            else
                Sender.Tell(string.Empty);
        }
    }
    public class CreateNewAgentActorMsg
    {
        public string       AgentID     { get; private set; }
        public int          AgentDBID   { get; private set; }
        public string       AuthToken   { get; private set; }
        public string       WhiteList   { get; private set; }
        public double       Balance     { get; private set; }
        public int          MoneyMode   { get; private set; }
        public IActorRef    DBReader    { get; private set; }
        public IActorRef    DBWriter    { get; private set; }

        public int Currency { get; private set; }


        public CreateNewAgentActorMsg(int dbID, string strAgentID, string authToken, string whiteList, double balance, int moneyMode, int currency, IActorRef dbReader, IActorRef dbWriter)
        {
            this.AgentDBID  = dbID;
            this.AgentID    = strAgentID;
            this.AuthToken  = authToken;
            this.WhiteList  = whiteList;
            this.Balance    = balance;
            this.DBReader   = dbReader;
            this.DBWriter   = dbWriter;
            this.MoneyMode  = moneyMode;
            this.Currency = currency;
        }
    }
    public class GetAgentActorMsg
    {
        public string AgentID { get; private set; }
        public GetAgentActorMsg(string strWebsiteID)
        {
            this.AgentID = strWebsiteID;
        }
    }
}
