using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using System.Collections.Concurrent;

namespace UserNode.Database
{
    public class DBMonitorSnapshot
    {
        private static DBMonitorSnapshot _sInstance = new DBMonitorSnapshot();
        public static DBMonitorSnapshot Instance
        {
            get
            {
                return _sInstance;
            }
        }

        public long             LastQuitUserID              { get; set; }
        public DateTime         LastUserUpdateTime          { get; set; }
        public long             LastRangeEventPlayerID      { get; set; }
        public DateTime         LastRangeEventUpdateTime    { get; set; }

        public DBMonitorSnapshot()
        {
            this.LastUserUpdateTime         = new DateTime(1, 1, 1);
            this.LastQuitUserID             = -1;
            this.LastRangeEventPlayerID     = -1;
            this.LastRangeEventUpdateTime   = new DateTime(1, 1, 1);
        }


    }    
}
