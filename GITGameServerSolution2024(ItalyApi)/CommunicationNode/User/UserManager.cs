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
                //检查该用户Actor是否已经存在。
                if (Context.Child(message.LoginResponse.GlobalUserID) != ActorRefs.Nobody)
                {
                    Sender.Tell(ActorRefs.Nobody);
                    return;
                }

                var userActor = Context.ActorOf(UserActor.Props(message), message.LoginResponse.GlobalUserID);
                _userHashMap.Add(message.LoginResponse.GlobalUserID);
                Context.Watch(userActor);
                
                //将Actor的路径注册到Redis后返回。
                registerUserPathToRedis(message.LoginResponse.GlobalUserID, userActor).PipeTo(Sender);
            });

            Receive<QuitUserMessage>(message =>
            {
                var userActor = Context.Child(message.GlobalUserID);
                //如果该用户不是登录状态
                if (userActor.Equals(ActorRefs.Nobody))
                    return;

                userActor.Tell(message);
            });
            
            Receive<UserRangeOddEventItem>(message =>
            {
                var userActor = Context.Child(message.GlobalUserID);
                //如果该用户不是登录状态
                if (userActor.Equals(ActorRefs.Nobody))
                    return;

                userActor.Tell(message);
            });

            Receive<UserEventCancelled>(message =>
            {
                IActorRef userActor = Context.Child(message.GlobalUserID);
                if (!userActor.Equals(ActorRefs.Nobody))
                    userActor.Tell(message);
            });

            Receive<ApiDepositMessage>(message =>
            {
                IActorRef userActor = Context.Child(message.GlobalUserID);
                if (!userActor.Equals(ActorRefs.Nobody))
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

        private async Task<IActorRef> registerUserPathToRedis(string strGlobalUserID, IActorRef userActor)
        {
            try
            {
                //在Redis中注册ActorPath、UserToken。
                string strUserPathFieldName  = string.Format("{0}_path", strGlobalUserID);
                await RedisDatabase.RedisCache.HashSetAsync("onlineusers", strUserPathFieldName, getActorRemotePath(userActor));

                //删除所有已注册过的UserToken。
                await RedisDatabase.RedisCache.KeyDeleteAsync(strGlobalUserID + "_tokens");

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
