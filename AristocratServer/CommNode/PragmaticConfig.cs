using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommNode
{
    public class PragmaticConfig
    {
        private static PragmaticConfig _sInstance = new PragmaticConfig();

        public static PragmaticConfig Instance
        {
            get
            {
                return _sInstance;
            }
        }
        public string ReplayURL { get; set; }
    }
}
