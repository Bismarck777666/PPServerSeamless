using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class MoneyRollGameLogic : BasePPClassicGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "cs5moneyroll";
            }
        }
        protected override bool SupportReplay
        {
            get
            {
                return false;
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 0; }
        }
        protected override int ServerResLineCount
        {
            get { return 0; }
        }
        protected override int ROWS
        {
            get
            {
                return 0;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "msg_code=0&pbalance=0.00&cfgs=1454&scw=1000~2000~3000~4000~6000^200^50^25^15^10^7^2^1&sc=50.00,125.00,250.00,500.00,1500.00,2500.00,5000.00,10000.00,25000.00,50000.00&a=0&b=0&gs=0";
            }
        }
        protected override int LineCount
        {
            get { return 5;  }
        }
        #endregion
        public MoneyRollGameLogic()
        {
            _gameID = GAMEID.MoneyRoll;
            GameName = "MoneyRoll";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            Dictionary<string, string> dicInitParams = splitResponseToParams(initString);
            dicParams.Add("balance", Math.Round(userBalance, 2).ToString());
            dicParams.Add("balance_cash", Math.Round(userBalance, 2).ToString());
            dicParams.Add("balance_bonus", "0.00");
            dicParams.Add("na", "s");
            dicParams.Add("stime", GameUtils.GetCurrentUnixTimestampMillis().ToString());
            dicParams.Add("sver", "5");
            dicParams.Add("s", "6,6,6,1,3,2,6,6,6");
            dicParams.Add("c", "500.00");  //defc
            dicParams.Add("l", LineCount.ToString());
            if (index > 0)
            {
                dicParams.Add("index", index.ToString());
                dicParams.Add("counter", (counter + 1).ToString());
            }

        }	
    }
}
