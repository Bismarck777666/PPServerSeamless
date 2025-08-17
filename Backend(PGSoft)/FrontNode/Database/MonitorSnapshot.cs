using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using System.Collections.Concurrent;

namespace FrontNode.Database
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
        public DateTime GameConfigUpdateTime { get; set; }
        public DBMonitorSnapshot()
        {
            this.GameConfigUpdateTime = new DateTime(1970, 1, 1);
        }

 
    }
}
