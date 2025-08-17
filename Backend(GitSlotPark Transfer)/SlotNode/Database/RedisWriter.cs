using Akka.Actor;
using Akka.Event;
using Akka.Routing;
using GITProtocol;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace SlotGamesNode.Database
{
    public class RedisWriter : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public RedisWriter()
        {
            ReceiveAsync<UserBetInfoWrite>      (onUserBetInfoWrite);
            ReceiveAsync<UserResultInfoWrite>   (onUserResultInfoWrite);
            ReceiveAsync<UserSettingWrite>      (onUserSettingWrite);
            ReceiveAsync<UserHistoryWrite>      (onUserHistoryWrite);
        }

        private async Task onUserBetInfoWrite(UserBetInfoWrite writeInfo)
        {
            try
            {
                string strKey = string.Format("{0}_{1}", writeInfo.GlobalUserID, writeInfo.GameID);
                if (writeInfo.BetInfoData != null)
                {
                    await RedisDatabase.RedisCache.StringSetAsync(strKey, writeInfo.BetInfoData, TimeSpan.FromDays(2.0));
                }
                else
                {
                    await RedisDatabase.RedisCache.KeyDeleteAsync(strKey);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in RedisWriter::onUserBetInfoWrite {0}", ex);
            }

            if (writeInfo.IsWait)
                Sender.Tell(true);
        }

        private async Task onUserSettingWrite(UserSettingWrite writeInfo)
        {
            try
            {
                string strKey = string.Format("{0}_{1}_setting", writeInfo.GlobalUserID, writeInfo.GameID);
                if (writeInfo.Setting != null)
                    await RedisDatabase.RedisCache.StringSetAsync(strKey, writeInfo.Setting, TimeSpan.FromDays(2.0));
                else
                    await RedisDatabase.RedisCache.KeyDeleteAsync(strKey);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in RedisWriter::onUserSettingWrite {0}", ex);
            }

            if (writeInfo.IsWait)
                Sender.Tell(true);
        }

        private async Task onUserHistoryWrite(UserHistoryWrite writeInfo)
        {
            try
            {
                string strKey = string.Format("{0}_{1}_history", writeInfo.GlobalUserID, writeInfo.GameID);
                if (writeInfo.HistoryData != null)
                    await RedisDatabase.RedisCache.StringSetAsync(strKey, writeInfo.HistoryData, TimeSpan.FromDays(2.0));
                else
                    await RedisDatabase.RedisCache.KeyDeleteAsync(strKey);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in RedisWriter::onUserHistoryWrite {0}", ex);
            }

            if (writeInfo.IsWait)
                Sender.Tell(true);
        }

        private async Task onUserResultInfoWrite(UserResultInfoWrite writeInfo)
        {
            try
            {
                string strKey = string.Format("{0}_{1}_result", writeInfo.GlobalUserID, writeInfo.GameID);
                if (writeInfo.ResultInfoData != null)
                    await RedisDatabase.RedisCache.StringSetAsync(strKey, writeInfo.ResultInfoData, TimeSpan.FromDays(2.0));
                else
                    await RedisDatabase.RedisCache.KeyDeleteAsync((RedisKey)strKey);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in RedisWriter::onUserResultInfoWrite {0}", ex);
            }

            if (writeInfo.IsWait)
                Sender.Tell(true);
        }

        protected override void PostStop()
        {
            base.PostStop();
        }
    }

    public class UserBetInfoWrite : IConsistentHashable
    {
        public string   GlobalUserID    { get; private set; }
        public GAMEID   GameID          { get; private set; }
        public byte[]   BetInfoData     { get; private set; }
        public bool     IsWait          { get; private set; }
        public object   ConsistentHashKey => GlobalUserID;

        public UserBetInfoWrite()
        {
        }

        public UserBetInfoWrite(string strGlobalUserID,GAMEID gameID,byte[] betInfoData,bool isWait)
        {
            GlobalUserID    = strGlobalUserID;
            GameID          = gameID;
            BetInfoData     = betInfoData;
            IsWait          = isWait;
        }
    }
    public class UserSettingWrite : IConsistentHashable
    {
        public string   GlobalUserID    { get; private set; }
        public GAMEID   GameID          { get; private set; }
        public string   Setting         { get; private set; }
        public bool     IsWait          { get; private set; }
        public object ConsistentHashKey => GlobalUserID;

        public UserSettingWrite()
        {
        }

        public UserSettingWrite(string strGlobalUserID, GAMEID gameID, string strSetting, bool isWait)
        {
            GlobalUserID    = strGlobalUserID;
            GameID          = gameID;
            Setting         = strSetting;
            IsWait          = isWait;
        }
    }
    public class UserHistoryWrite : IConsistentHashable
    {
        public string   GlobalUserID    { get; private set; }
        public GAMEID   GameID          { get; private set; }
        public byte[]   HistoryData     { get; private set; }
        public bool     IsWait          { get; private set; }
        public object   ConsistentHashKey => GlobalUserID;

        public UserHistoryWrite()
        {
        }

        public UserHistoryWrite(string strGlobalUserID,GAMEID gameID,byte[] historyData,bool isWait)
        {
            GlobalUserID    = strGlobalUserID;
            GameID          = gameID;
            HistoryData     = historyData;
            IsWait          = isWait;
        }
    }
    public class UserResultInfoWrite : IConsistentHashable
    {
        public string   GlobalUserID    { get; private set; }
        public GAMEID   GameID          { get; private set; }
        public byte[]   ResultInfoData  { get; private set; }
        public bool     IsWait          { get; private set; }
        public object   ConsistentHashKey => GlobalUserID;

        public UserResultInfoWrite()
        {
        }

        public UserResultInfoWrite(string strGlobalUserID,GAMEID gameID,byte[] resultInfoData,bool isWait)
        {
            GlobalUserID    = strGlobalUserID;
            GameID          = gameID;
            ResultInfoData  = resultInfoData;
            IsWait          = isWait;
        }
    }
}
