using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlotGamesNode.GameLogics;

namespace SlotGamesNode
{
    public class HabaneroConfig
    {
        private static HabaneroConfig _sInstance = new HabaneroConfig();
        public static HabaneroConfig Instance
        {
            get
            {
                return _sInstance;
            }
        }
        public string ReplayURL     { get; set; }
    }
}
