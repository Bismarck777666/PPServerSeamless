using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using GITProtocol;
using Akka.Routing;
using Akka.Event;
using SlotGamesNode.GameLogics;
using PCGSharp;

namespace SlotGamesNode.Database
{
    public class RedisWriter : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Logging.GetLogger(Context);
        public RedisWriter()
        {
            ReceiveAsync<UserBetInfoWrite>      (onUserBetInfoWrite);
            ReceiveAsync<UserResultInfoWrite>   (onUserResultInfoWrite);
        }

        private async Task onUserBetInfoWrite(UserBetInfoWrite writeInfo)
        {
            try
            {
                string strKey = string.Format("{0}_{1}", writeInfo.UserID, writeInfo.GameID);
                if (writeInfo.BetInfoData != null)
                {
                    await RedisDatabase.RedisCache.StringSetAsync(strKey, writeInfo.BetInfoData, TimeSpan.FromDays(2));
                }
                else
                {
                    await RedisDatabase.RedisCache.KeyDeleteAsync(strKey);
                }
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in RedisWriter::onUserBetInfoWrite {0}", ex);
            }
            if(writeInfo.IsWait)
                Sender.Tell(true);
        }
        private async Task onUserResultInfoWrite(UserResultInfoWrite writeInfo)
        {
            try
            {
                string strKey = string.Format("{0}_{1}_result", writeInfo.UserID, writeInfo.GameID);
                if (writeInfo.ResultInfoData != null)
                {
                    await RedisDatabase.RedisCache.StringSetAsync(strKey, writeInfo.ResultInfoData, TimeSpan.FromDays(2));
                }
                else
                {
                    await RedisDatabase.RedisCache.KeyDeleteAsync(strKey);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in RedisWriter::onUserResultInfoWrite {0}", ex);
            }
            if(writeInfo.IsWait)
                Sender.Tell(true);
        }

        protected override void PostStop()
        {
            base.PostStop();
        }
    }

    public class UserBetInfoWrite : IConsistentHashable
    {
        public string UserID { get; private set; }
        public GAMEID GameID { get; private set; }
        public byte[] BetInfoData { get; private set; }

        public bool IsWait { get; private set; }
        public object ConsistentHashKey
        {
            get
            {
                return UserID;
            }
        }

        public UserBetInfoWrite()
        {

        }
        public UserBetInfoWrite(string strUserID, GAMEID gameID, byte[] betInfoData, bool isWait)
        {
            this.UserID         = strUserID;
            this.GameID         = gameID;
            this.BetInfoData    = betInfoData;
            this.IsWait         = isWait;
        }        
    }

    public class UserResultInfoWrite : IConsistentHashable
    {
        public string UserID            { get; private set; }
        public GAMEID GameID            { get; private set; }
        public byte[] ResultInfoData    { get; private set; }

        public bool IsWait { get; private set; }
        public object ConsistentHashKey
        {
            get
            {
                return UserID;
            }
        }

        public UserResultInfoWrite()
        {

        }
        public UserResultInfoWrite(string strUserID, GAMEID gameID, byte[] resultInfoData, bool isWait)
        {
            this.UserID         = strUserID;
            this.GameID         = gameID;
            this.ResultInfoData = resultInfoData;
            this.IsWait         = isWait;
        }
    }
}
