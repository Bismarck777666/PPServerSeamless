using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PromoNode.Database
{
    public class DBMonitorSnapshot
    {
        private static  DBMonitorSnapshot   _sInstance  = new DBMonitorSnapshot();
        public static DBMonitorSnapshot     Instance    => _sInstance;
        public DBMonitorSnapshot()
        {

        }

    }
}
