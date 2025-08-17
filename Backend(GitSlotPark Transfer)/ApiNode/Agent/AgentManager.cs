using Akka.Actor;
using Akka.Event;
using QueenApiNode.Database;
using System;
using System.Collections.Generic;

namespace QueenApiNode.Agent
{
    class AgentManager : ReceiveActor
    {
        private readonly ILoggingAdapter        _logger         = Context.GetLogger();
        private bool                            _isShuttingDown = false;
        private Dictionary<string, IActorRef>   _agentActors    = new Dictionary<string, IActorRef>();
        private Dictionary<string, string>      _authTokenMaps  = new Dictionary<string, string>();
        private Dictionary<int, string>         _agentIDsMap    = new Dictionary<int, string>();

        public AgentManager()
        {
            Receive<CreateNewAgentActorMsg>(message =>
            {
                //해당 유저액터가 이미 존재하는가를 검사한다.
                IActorRef agentActor = Context.Child(message.AgentID);
                if (agentActor != ActorRefs.Nobody)
                {
                    Sender.Tell(agentActor);
                    return;
                }
                
                agentActor = Context.ActorOf(AgentActor.Props(message), message.AgentID);
                _agentIDsMap[message.AgentDBID] = message.AgentID;
                Context.Watch(agentActor);
                _agentActors[message.AgentID]   = agentActor;
                Sender.Tell(agentActor);
            });
            Receive<GetAgentActorMsg>(message =>
            {
                Sender.Tell(Context.Child(message.AgentID));
            });
            Receive<GetAgentActorByIDMsg>(message =>
            {
                if (!_agentIDsMap.ContainsKey(message.AgentID))
                    Sender.Tell(ActorRefs.Nobody);
                else
                    Sender.Tell(Context.Child(_agentIDsMap[message.AgentID]));
            });
            Receive<Terminated>(terminated =>
            {
                int agentDBID = 0;
                foreach (KeyValuePair<int, string> agentIds in _agentIDsMap)
                {
                    if (agentIds.Value == terminated.ActorRef.Path.Name)
                    {
                        agentDBID = agentIds.Key;
                        break;
                    }
                }

                if (agentDBID > 0)
                    _agentIDsMap.Remove(agentDBID);
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
            foreach (KeyValuePair<string, string> agentAuthToken in agentAuthTokens)
                _authTokenMaps[agentAuthToken.Value] = agentAuthToken.Key;
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

            if (strAuthKey == null)
            {
                _authTokenMaps[updated.AuthToken] = updated.AgentID;
            }
            else
            {
                _authTokenMaps.Remove(strAuthKey);
                _authTokenMaps[updated.AuthToken] = updated.AgentID;
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
        public double       Balance     { get; private set; }
        public IActorRef    DBReader    { get; private set; }
        public IActorRef    DBWriter    { get; private set; }
        public int          Currency    { get; private set; }
        public int          Language    { get; private set; }

        public CreateNewAgentActorMsg(int dbID,string strAgentID,string authToken,double balance,int currency,int language,IActorRef dbReader,IActorRef dbWriter)
        {
            AgentDBID   = dbID;
            AgentID     = strAgentID;
            AuthToken   = authToken;
            Balance     = balance;
            DBReader    = dbReader;
            DBWriter    = dbWriter;
            Currency    = currency;
            Language    = language;
        }
    }

    public class GetAgentActorByIDMsg
    {
        public int AgentID { get; private set; }

        public GetAgentActorByIDMsg(int agentID)
        {
            AgentID = agentID;
        }
    }

    public class GetAgentActorMsg
    {
        public string AgentID { get; private set; }

        public GetAgentActorMsg(string strWebsiteID)
        {
            AgentID = strWebsiteID;
        }
    }
}
