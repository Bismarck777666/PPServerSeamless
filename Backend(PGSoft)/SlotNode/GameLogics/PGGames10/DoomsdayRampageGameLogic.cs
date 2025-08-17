using GITProtocol;
using Newtonsoft.Json.Linq;
using SlotGamesNode.GameLogics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendServer.GameLogics
{
    internal class DoomsdayRampageGameLogic : BasePGSlotGame
    {
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
                return 10;
            }
        }
        protected override string DefaultResult
        {
            get
            {
                return "{\"si\":{\"wp\":null,\"sw\":null,\"sc\":null,\"wpl\":null,\"lw\":null,\"orl\":[4,5,6,7,8,8,7,7,7,7,7,7,8,8,8],\"grs\":[false,false,true,false,false],\"gm\":1,\"twbm\":0,\"iwa\":[false,false,false],\"gmi\":0,\"crtw\":0,\"imw\":false,\"gwt\":-1,\"pmt\":null,\"ab\":null,\"ml\":1,\"cs\":0.1,\"rl\":[4,5,6,7,8,8,7,7,7,7,7,7,8,8,8],\"ctw\":0,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"fb\":null,\"sid\":\"1934920734518283778\",\"psid\":\"1934920734518283778\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":290231.9,\"blab\":290230.9,\"bl\":291590.57,\"tb\":1,\"tbb\":1,\"tw\":0,\"np\":-1,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"lw\":null,\"snww\":null,\"wpl\":null,\"ptbr\":null,\"ssaw\":0,\"orl\":[97,4,97,97,6,97,97,5,7,3,99,99,8,8,11,11,10,10,5,5,7,99,99,9,7,3,99,99,8,8,11,11,97,4,97,97,6,97,97,5],\"gm\":1,\"bpf\":{\"1\":[9,10,11],\"2\":[20,21,22],\"3\":[25,26,27]},\"bp\":{\"1\":[9,10,11],\"2\":[20,21,22],\"3\":[25,26,27]},\"mp\":[2,4,8,16,32,64,128,256,512,1024],\"mib\":-1,\"mi\":-1,\"rns\":null,\"twbm\":0,\"crtw\":0,\"imw\":false,\"sc\":0,\"now\":13824,\"nowpr\":[8,6,6,6,8],\"fs\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.5,\"rl\":[97,4,97,97,6,97,97,5,7,3,99,99,8,8,11,11,10,10,5,5,7,99,99,9,7,3,99,99,8,8,11,11,97,4,97,97,6,97,97,5],\"sid\":\"1849585488751627777\",\"psid\":\"1849585488751627777\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":16.42,\"blab\":16.42,\"bl\":16.42,\"tb\":0,\"tbb\":0,\"tw\":0,\"np\":0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.75,\"max\":96.75}},\"grtpi\":[{\"gt\":\"Default\",\"grtps\":[{\"t\":\"min\",\"tphr\":null,\"rtp\":96.75},{\"t\":\"max\",\"tphr\":null,\"rtp\":96.75}]}],\"ows\":{\"itare\":true,\"tart\":1,\"igare\":true,\"gart\":2160},\"jws\":null}";
            }
        }
        public DoomsdayRampageGameLogic()
        {
            _gameID     = GAMEID.DoomsdayRampage;
            GameName    = "DoomsdayRampage";
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

            if (!IsNullOrEmpty(jsonParams["sw"]) && IsArrayOrObject(jsonParams["sw"]))
            {
                var swValues = jsonParams["sw"].ToObject<Dictionary<string, object>>();
                foreach (KeyValuePair<string, object> pair in swValues)
                {
                        jsonParams["sw"][pair.Key] = convertWinByBet((double)pair.Value, currentBet);
                }
            }

            if (!IsNullOrEmpty(jsonParams["twbm"]))
                jsonParams["twbm"] = convertWinByBet((double)jsonParams["twbm"], currentBet);

            if (!IsNullOrEmpty(jsonParams["ctw"]))
                jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);

        }
    }
}
