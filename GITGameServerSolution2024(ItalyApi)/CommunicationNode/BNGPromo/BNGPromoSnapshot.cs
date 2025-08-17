using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommNode
{
    public class BNGPromoSnapshot
    {
        public static string SnapShot { get; set; }

        static BNGPromoSnapshot()
        {
            SnapShot = "[]";
        }
    }
}
