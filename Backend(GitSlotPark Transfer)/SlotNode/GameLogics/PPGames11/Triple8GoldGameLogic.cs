using GITProtocol;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class Triple8GoldGameLogic : BasePPClassicGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "cs5triple8gold";
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
                return "msg_code=0&pbalance=0.00&cfgs=1255&scw=1000~2000~3000~4000~6000^200^50^25^15^10^8^^&sc=50.00,125.00,250.00,500.00,1500.00,2500.00,5000.00,10000.00,25000.00,50000.00&a=0&b=0&gs=0";
            }
        }
	
	
        #endregion
        public Triple8GoldGameLogic()
        {
            _gameID = GAMEID.Triple8Gold;
            GameName = "Triple8Gold";
        }        
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
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
            dicParams.Add("s", "4,6,1,6,5,6,2,6,4");
            string[] chipsets = dicInitParams["sc"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            dicParams.Add("c", chipsets[0]);  //defc
            dicParams.Add("l", LineCount.ToString());
            if (index > 0)
            {
                dicParams.Add("index", index.ToString());
                dicParams.Add("counter", (counter + 1).ToString());
            }
        }
    }
}
