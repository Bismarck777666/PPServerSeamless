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
using CommNode.Database;
using Akka.Event;

namespace CommNode
{
    public class UserManager : ReceiveActor
    {
        private HashSet<string>             _userHashMap    = new HashSet<string>();
        private bool                        _isShuttingDown = false;
        private readonly ILoggingAdapter    _logger         = Logging.GetLogger(Context);

        public UserManager()
        {
            Receive<CreateNewUserMessage>(message =>
            {
                //해당 유저액터가 이미 존재하는가를 검사한다.
                if (Context.Child(message.LoginResponse.UserID) != ActorRefs.Nobody)
                {
                    Sender.Tell(ActorRefs.Nobody);
                    return;
                }
                var userActor = Context.ActorOf(UserActor.Props(message), message.LoginResponse.UserID);
                _userHashMap.Add(message.LoginResponse.UserID);
                Context.Watch(userActor);
                
                //액터의 패스를 레디스에 등록한후에 리턴한다.
                registerUserPathToRedis(message.LoginResponse.UserID, message.LoginResponse.UserToken, userActor).PipeTo(Sender);

            });
            Receive<List<SetScoreData>>(message =>
            {
                List<SetScoreData> toCachedData = new List<SetScoreData>();
                foreach (SetScoreData scoreData in message)
                {
                    var userActor = Context.Child(scoreData.UserID);
                    //만일 해당 유저가 로그인한 상태가 아니라면 
                    if (userActor.Equals(ActorRefs.Nobody))
                    {
                        toCachedData.Add(scoreData);
                        continue;
                    }
                    userActor.Tell(new AddScoreMessage(scoreData.UserID, scoreData.ID, scoreData.Score));
                }
                Context.System.ActorSelection("/user/scoreCacheActor").Tell(toCachedData);
            });
            Receive<QuitUserMessage>(message =>
            {
                var userActor = Context.Child(message.UserID);
                //만일 해당 유저가 로그인한 상태가 아니라면 
                if (userActor.Equals(ActorRefs.Nobody))
                    return;

                userActor.Tell(message);
            });
            Receive<ServerMaintenanceNotify>(_ =>
            {
                if (_userHashMap.Count > 0)
                    Context.ActorSelection("*").Tell(new QuitUserMessage(""));
            });
            Receive<UserRollingPerUpdated>(message =>
            {
                var userActor = Context.Child(message.UserID);
                //만일 해당 유저가 로그인한 상태가 아니라면 
                if (userActor.Equals(ActorRefs.Nobody))
                    return;

                userActor.Tell(message);
            });
            Receive<AgentRollingPerUpdated>(message =>
            {
                if (_userHashMap.Count > 0)
                    Context.ActorSelection("*").Tell(message);
            });
            Receive<UserRangeOddEventItem>(message =>
            {
                var userActor = Context.Child(message.UserID);
                //만일 해당 유저가 로그인한 상태가 아니라면 
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

            Receive<SlotGamesNodeShuttingdown>(message =>
            {
                if (_userHashMap.Count > 0)
                Context.ActorSelection("*").Tell(message);
                else
                    _logger.Info("SlotGamesNodeShuttingdown Message Received but no users online for now");
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

        private async Task<IActorRef> registerUserPathToRedis(string strUserID, string strUserToken, IActorRef userActor)
        {
            try
            {
                //레디스에 액터패스, 유저토큰을 등록한다.
                string strUserPathFieldName  = string.Format("{0}_path", strUserID);
                await RedisDatabase.RedisCache.HashSetAsync("onlineusers", strUserPathFieldName, getActorRemotePath(userActor));

                //이미 등록됬던 유저토큰들을 모두 삭제한다.
                await RedisDatabase.RedisCache.KeyDeleteAsync(strUserID + "_tokens");

                return userActor;
            }
            catch (Exception)
            {
                return ActorRefs.Nobody;
            }
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
