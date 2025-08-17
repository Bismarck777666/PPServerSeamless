using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using GITProtocol;
using Akka.Routing;
using StackExchange.Redis;
using Akka.Event;
using System.Diagnostics;

namespace CommNode.Database
{
    public class RedisWriter : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Logging.GetLogger(Context);
        public RedisWriter()
        {
            ReceiveAsync<UserReportinfoWrite>(onUserReportInfoWrite);
        }

        protected override void PreStart()
        {
            base.PreStart();

        }

        private async Task onUserReportInfoWrite(UserReportinfoWrite writeInfo)
        {
            try
            {
                string strKey = string.Format("{0}_report", writeInfo.UserID);
                if (writeInfo.ReportInfoData != null)
                {
                    await RedisDatabase.RedisCache.StringSetAsync(strKey, writeInfo.ReportInfoData, TimeSpan.FromDays(2));
                }
                else
                {
                    await RedisDatabase.RedisCache.KeyDeleteAsync(strKey);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in RedisWriter::onUserReportInfoWrite {0}", ex);
            }
            if (writeInfo.IsWait)
                Sender.Tell(true);
        }
    }

    public class UserReportinfoWrite : IConsistentHashable
    {
        public string UserID { get; private set; }
        
        public byte[] ReportInfoData { get; private set; }

        public bool IsWait { get; private set; }
        public object ConsistentHashKey
        {
            get
            {
                return UserID;
            }
        }

        public UserReportinfoWrite()
        {

        }
        public UserReportinfoWrite(string strUserID, byte[] reportInfoData, bool isWait)
        {
            this.UserID = strUserID;            
            this.ReportInfoData = reportInfoData;
            this.IsWait = isWait;
        }
    }
}
