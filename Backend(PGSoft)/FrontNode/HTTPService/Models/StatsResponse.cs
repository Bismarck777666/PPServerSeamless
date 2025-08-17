using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontNode.HTTPService.Models
{
    public class StatsResponse
    {
        public int error            { get; set; }
        public string description   { get; set; }

        public StatsResponse(int errorCode, string strDescription)
        {
            this.error          = errorCode;
            this.description    = strDescription;
        }
    }
}
