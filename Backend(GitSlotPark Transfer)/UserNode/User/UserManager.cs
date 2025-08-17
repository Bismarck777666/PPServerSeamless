using Akka.Actor;
using Akka.Event;
using GITProtocol;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UserNode.Database;

namespace UserNode
{
    public class UserManager : ReceiveActor
    {
        private HashSet<string>             _userHashMap    = new HashSet<string>();
        private bool                        _isShuttingDown = false;
        private readonly ILoggingAdapter    _logger         = Context.GetLogger();
        private IActorRef                   _dbReader;
        private IActorRef                   _dbWriter;

        public UserManager(IActorRef dbReader, IActorRef dbWriter)
        {
            Receive<CreateNewUserMessage>(message =>
            {
                if (Context.Child(message.GlobalUserID) != ActorRefs.Nobody)
                {
                    Sender.Tell(ActorRefs.Nobody);
                    return;
                }

                var userActor = Context.ActorOf(UserActor.Props(message, _dbReader, _dbWriter), message.GlobalUserID);
                _userHashMap.Add(message.GlobalUserID);
                Context.Watch(userActor);

                //액터의 패스를 레디스에 등록한후에 리턴한다.
                registerUserPathToRedis(message.GlobalUserID, userActor).PipeTo(Sender);
            });
            Receive<ForceLogoutMesssage>(message =>
            {
                var userActor = Context.Child(message.GlobalUserID);
                if (userActor.Equals(ActorRefs.Nobody))
                    return;
                
                userActor.Tell(new QuitUserMessage(message.AgentID, message.UserID));
            });
            Receive<QuitUserMessage>(message =>
            {
                var userActor = Context.Child(message.GlobalUserID);
                if (userActor.Equals(ActorRefs.Nobody))
                    return;

                userActor.Tell(message);
            });
            Receive<string>(command =>
            {
                if (command == "terminate")
                {
                    Context.ActorSelection("*").Tell(PoisonPill.Instance);
                    _isShuttingDown = true;
                    if (_userHashMap.Count == 0)
                        Context.Stop(Self);
                }
            });
            Receive<SlotGamesNodeShuttingdown>(message =>
            {
                if (_userHashMap.Count > 0)
                    Context.ActorSelection("*").Tell(message);
                else
                    _logger.Info("SlotGamesNodeShuttingdown Message Received but no users online for now");
            });
            Receive<ApiUserDepositMessage>(message =>
            {
                IActorRef userActor = Context.Child(message.GlobalUserID);
                if (userActor.Equals(ActorRefs.Nobody))
                    return;

                userActor.Tell(message);
            });
            Receive<Terminated>(terminated =>
            {
                _userHashMap.Remove(terminated.ActorRef.Path.Name);
                if (_isShuttingDown && _userHashMap.Count == 0)
                    Context.Stop(Self);
            });

            _dbReader = dbReader;
            _dbWriter = dbWriter;
        }

        private string getActorRemotePath(IActorRef actor)
        {
            string strActorPath         = actor.Path.ToString();
            string strClusterAddress    = Akka.Cluster.Cluster.Get(Context.System).SelfAddress.ToString();
            int startIndex              = strActorPath.IndexOf("/user");
            string strRemotePath = strClusterAddress + strActorPath.Substring(startIndex);
            return strRemotePath;
        }

        private async Task<IActorRef> registerUserPathToRedis(string strGlobalUserID, IActorRef userActor)
        {
            try
            {
                //레디스에 액터패스, 유저토큰을 등록한다.
                string strUserPathFieldName = string.Format("{0}_path", strGlobalUserID);
                await RedisDatabase.RedisCache.HashSetAsync("onlineusers", strUserPathFieldName, getActorRemotePath(userActor));

                //이미 등록됬던 유저토큰들을 모두 삭제한다.
                await RedisDatabase.RedisCache.KeyDeleteAsync(strGlobalUserID + "_tokens");
                return userActor;
            }
            catch (Exception ex)
            {
                return ActorRefs.Nobody;
            }
        }

        public static Props Props(IActorRef dbReader, IActorRef dbWriter)
        {
            return Akka.Actor.Props.Create(() => new UserManager(dbReader, dbWriter));
        }

        protected override void PreStart()
        {
            base.PreStart();
        }
    }
}
