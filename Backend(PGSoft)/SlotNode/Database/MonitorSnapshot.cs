using GITProtocol;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.Database
{
    public class DBMonitorSnapshot
    {
        private static  DBMonitorSnapshot _sInstance = new DBMonitorSnapshot();
        public static DBMonitorSnapshot Instance
        {
            get
            {
                return _sInstance;
            }
        }
        
        public DateTime         GameConfigUpdateTime        { get; set; }        
        public DateTime         AgentGameConfigUpdateTime   { get; set; }
        public DateTime         PayoutPoolConfigUpdateTime  { get; set; }
        public int              PayoutPoolResetLastID       { get; set; }
        


        public DBMonitorSnapshot()
        {
            this.GameConfigUpdateTime           = new DateTime(1, 1, 1);
            this.AgentGameConfigUpdateTime      = new DateTime(1, 1, 1);
            this.PayoutPoolConfigUpdateTime     = new DateTime(1, 1, 1);
            this.PayoutPoolResetLastID          = 0;
        }
    }
}
