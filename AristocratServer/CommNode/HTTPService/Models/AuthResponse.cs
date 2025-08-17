using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommNode.HTTPService.Models
{
    public class AuthResponse
    {
        public string   result          { get; set; }
        public string   sessiontoken    { get; set; }
        public int      currency        { get; set; }
        public string   gamedata        { get; set; }
        public string   dateTime        { get; set; }
        public AuthResponse(string strResult, string strSessionToken, int currency, string gamedata, string customDateTime = null)
        {
            this.result         = strResult;
            this.sessiontoken   = strSessionToken;
            this.currency       = currency;
            this.gamedata       = gamedata;
            this.dateTime       = customDateTime ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
