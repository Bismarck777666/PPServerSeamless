using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace PromoNode.Database
{
    public class RedisDatabase
    {
        private static string   _sRedisIP;
        private static int      _sRedisPort;
        private static string   _sPassword;

        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect(string.Format("{0}:{1},password={2}", _sRedisIP, _sRedisPort, _sPassword));
        });

        static RedisDatabase()
        {
            
        }
        public static void setRedisInfo(string strIP, int port, string strPassword)
        {
            _sRedisIP   = strIP;
            _sRedisPort = port;
            _sPassword  = strPassword;
        }
        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
        public static IDatabase RedisCache
        {
            get
            {
                return Connection.GetDatabase();
            }
        }
    }
}
