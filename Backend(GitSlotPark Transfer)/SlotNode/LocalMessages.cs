using Akka.Actor;

namespace SlotGamesNode
{
    public class DBProxyInform
    {
        public IActorRef DBReader       { get; private set; }
        public IActorRef DBWriter       { get; private set; }
        public IActorRef RedisWriter    { get; private set; }

        public DBProxyInform(IActorRef dbReader, IActorRef dbWriter, IActorRef redisWriter)
        {
            DBReader    = dbReader;
            DBWriter    = dbWriter;
            RedisWriter = redisWriter;
        }
    }
}
