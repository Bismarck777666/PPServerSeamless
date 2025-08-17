using Akka.Actor;
using Akka.Routing;
using System;
using System.Linq.Expressions;

namespace SlotGamesNode.Database
{
    public class DBProxyReader : ReceiveActor
    {
        private string _strConnString = "";
        public DBProxyReader(string strConnString)
        {
            _strConnString = strConnString;
        }

        public static Props Props(string strConnString, int poolSize)
        {
            return Akka.Actor.Props.Create(() => new DBProxyReader(strConnString)).WithRouter(new RoundRobinPool(poolSize));
        }
    }
}
