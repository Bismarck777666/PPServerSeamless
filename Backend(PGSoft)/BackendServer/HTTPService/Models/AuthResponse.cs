using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.HTTPService.Models
{
    public class AuthResponse
    {
        public string result        { get; set; }
        public string sessiontoken  { get; set; }

        public AuthResponse(string strResult, string strSessionToken)
        {
            this.result         = strResult;
            this.sessiontoken   = strSessionToken;
        }
    }
}
