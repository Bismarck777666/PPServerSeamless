using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using System.Collections.Concurrent;

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
        public long             LastGameJackpotID           { get; set; }
        public DateTime         LastGameJackpotThemeTime    { get; set; }
        public DateTime         LastGameJackpotConfigTime   { get; set; }

        public DBMonitorSnapshot()
        {
            this.LastGameJackpotID              = -1;
            this.LastGameJackpotThemeTime       = new DateTime(1, 1, 1);
            this.LastGameJackpotConfigTime      = new DateTime(1, 1, 1);
        }
    }
}
