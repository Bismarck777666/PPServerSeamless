using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Routing;
using GITProtocol;
using StackExchange.Redis;
using Akka.Cluster;
using SlotGamesNode.Database;
using Akka.Event;

namespace SlotGamesNode
{
    public class UserManager : ReceiveActor
    {
        private HashSet<string> _userHashMap    = new HashSet<string>();
        private bool            _isShuttingDown = false;
        private readonly ILoggingAdapter    _logger         = Logging.GetLogger(Context);

        public UserManager()
        {
            Receive<CreateNewUserMessage>(message =>
            {
                if (Context.Child(message.LoginResponse.UserID) != ActorRefs.Nobody)
                {
                    Sender.Tell(ActorRefs.Nobody);
                    return;
                }
                var userActor = Context.ActorOf(UserActor.Props(message), message.LoginResponse.UserID);
                _userHashMap.Add(message.LoginResponse.UserID);
                Context.Watch(userActor);

                Sender.Tell(userActor);
            });
            Receive<GameJackpotBonusItem>(message =>
            {
                var userActor = Context.Child(message.UserID);
                if (userActor.Equals(ActorRefs.Nobody))
                    return;

                userActor.Tell(message);
            });

            Receive<string>(command =>
            {
                if (command != "terminate")
                    return;

                Context.ActorSelection("*").Tell(PoisonPill.Instance);
                _isShuttingDown = true;
                if (_userHashMap.Count == 0)
                    Context.Stop(Self);
            });

            Receive<Terminated>(terminated =>
            {
                _userHashMap.Remove(terminated.ActorRef.Path.Name);
                if (_isShuttingDown && _userHashMap.Count == 0)
                    Context.Stop(Self);
            });
        }

        private string getActorRemotePath(IActorRef actor)
        {
            string strActorPath      = actor.Path.ToString();
            string strClusterAddress = Cluster.Get(Context.System).SelfAddress.ToString();
            int    start             = strActorPath.IndexOf("/user");
            string strRemotePath     = strClusterAddress + strActorPath.Substring(start);
            return strRemotePath;
        }
        
        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new UserManager());
        }

        protected override void PreStart()
        {
            base.PreStart();
        }

    }    
}
