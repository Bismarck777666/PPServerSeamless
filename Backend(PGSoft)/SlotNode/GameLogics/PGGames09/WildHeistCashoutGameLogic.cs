using GITProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    internal class WildHeistCashoutGameLogic : BasePGSlotGame
    {
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 75.0; }
        }
        protected override bool HasPurEnableOption
        {
            get { return true; }
        }
        protected override double DefaultBetSize
        {
            get { return 0.2; }
        }
        protected override int DefaultBetLevel
        {
            get { return 5; }
        }
        protected override int BaseBet
        {
            get
            {
                return 20;
            }
        }
        protected override string DefaultResult
        {
            get
            {
                return "{\"si\":{\"trl\":[6,6,12,2,4,0,0,0,0,8,1,5,13,10,0],\"twbm\":0.0,\"rtwbm\":0.0,\"trtwbm\":0.0,\"rtw\":0.0,\"trtw\":0.0,\"wp\":null,\"trwp\":null,\"wpl\":null,\"trwpl\":null,\"lw\":null,\"trlw\":null,\"snww\":null,\"trsnww\":null,\"pswp\":0,\"swp\":99,\"ptrswp\":0,\"trswp\":99,\"ptbr\":null,\"trptbr\":null,\"wfrp\":null,\"wftrp\":null,\"fswp\":null,\"nrswp\":null,\"rsmw\":false,\"orl\":[6,6,12,2,4,0,0,0,0,8,1,5,13,10,0],\"otrl\":[6,6,12,2,4,0,0,0,0,8,1,5,13,10,0],\"rns\":null,\"trns\":null,\"ssaw\":0.00,\"sc\":0,\"gm\":1,\"gml\":[1,2,3,5],\"crtw\":0.0,\"imw\":false,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.5,\"rl\":[6,6,12,2,4,0,0,0,0,8,1,5,13,10,0],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":10004.39,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"trl\":[6,6,12,2,4,9,3,3,7,8,11,5,13,10,4],\"twbm\":1.2,\"rtwbm\":1.2,\"trtwbm\":0.0,\"rtw\":0.0,\"trtw\":0.0,\"wp\":null,\"trwp\":null,\"wpl\":null,\"trwpl\":null,\"lw\":null,\"trlw\":null,\"snww\":null,\"trsnww\":null,\"pswp\":99,\"swp\":99,\"ptrswp\":99,\"trswp\":99,\"ptbr\":null,\"trptbr\":null,\"wfrp\":null,\"wftrp\":null,\"fswp\":null,\"nrswp\":null,\"rsmw\":false,\"orl\":[6,6,12,2,4,9,3,3,7,8,11,5,13,10,4],\"otrl\":[6,6,12,2,4,9,3,3,7,8,11,5,13,10,4],\"rns\":null,\"trns\":null,\"ssaw\":0.0,\"sc\":0,\"gm\":1,\"gml\":[1,2,3,5],\"crtw\":0.0,\"imw\":false,\"fs\":null,\"gwt\":-1,\"fb\":null,\"ctw\":1.2,\"pmt\":null,\"cwc\":1,\"fstc\":null,\"pcwc\":1,\"rwsp\":{\"0\":{\"7\":2.0},\"1\":null},\"hashr\":null,\"ml\":3,\"cs\":0.1,\"rl\":[6,6,12,2,4,9,3,3,7,8,11,5,13,10,4],\"sid\":\"1762871380241596416\",\"psid\":\"1762871380241596416\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.77,\"max\":96.77}},\"ows\":{\"itare\":true,\"tart\":1,\"igare\":true,\"gart\":2161},\"jws\":null}";
            }
        }
        public WildHeistCashoutGameLogic()
        {
            _gameID = GAMEID.WildHeistCashout;
            GameName = "WildHeistCashout";
        }
        protected override void initGameData()
        {
            base.initGameData();
            _pgGameConfig.ml.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            _pgGameConfig.cs.AddRange(new double[] { 0.02, 0.1, 0.2 });
        }
        protected override void convertWinsByBet(dynamic jsonParams, float currentBet)
        {
            base.convertWinsByBet((object)jsonParams, currentBet);
            if (!IsNullOrEmpty(jsonParams["ctw"]))
                jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["rtw"]))
                jsonParams["rtw"] = convertWinByBet((double)jsonParams["rtw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["rtwbm"]))
                jsonParams["rtwbm"] = convertWinByBet((double)jsonParams["rtwbm"], currentBet);

            if (!IsNullOrEmpty(jsonParams["twbm"]))
                jsonParams["twbm"] = convertWinByBet((double)jsonParams["twbm"], currentBet);

            if (!IsNullOrEmpty(jsonParams["trtwbm"]))
                jsonParams["trtwbm"] = convertWinByBet((double)jsonParams["trtwbm"], currentBet);

            if (!IsNullOrEmpty(jsonParams["trtw"]))
                jsonParams["trtw"] = convertWinByBet((double)jsonParams["trtw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["trlw"]))
            {
                string strLw = jsonParams["trlw"].ToString();
                Dictionary<int, double> lineWins = JsonConvert.DeserializeObject<Dictionary<int, double>>(strLw);
                Dictionary<int, double> convertedLineWins = new Dictionary<int, double>();
                foreach (KeyValuePair<int, double> pair in lineWins)
                {
                    convertedLineWins[pair.Key] = convertWinByBet(pair.Value, currentBet);
                }
                jsonParams["trlw"] = JObject.FromObject(convertedLineWins);
            }
        }
    }
}
