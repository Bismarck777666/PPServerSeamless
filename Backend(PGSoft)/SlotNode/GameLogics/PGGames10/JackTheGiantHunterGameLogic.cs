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
    internal class JackTheGiantHunterGameLogic : BasePGSlotGame
    {
        protected override bool HasPurEnableOption
        {
            get { return true; }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 75.0; }
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
                return "{\"si\":{\"wp\":null,\"lw\":null,\"orl\":[11,11,7,7,7,2,13,13,9,9,9,12,10,10,13,13,3,3,5,8,9,2,13,13,11,11,6,6,6,7,13,13,4,4,9,9,2,11,13,13],\"bwp\":null,\"snww\":null,\"es\":null,\"crtw\":0,\"imw\":false,\"ssaw\":0,\"gm\":2,\"fp\":{\"ft\":[[2,0,0,0,0,0,0,0],[2,0,0,0,0,0,0,0],[3,3,2,2,2,2,0,0],[2,0,0,0,0,0,0,0],[0,0,3,0,3,2,0,0]],\"afp\":[]},\"fpb\":null,\"crh\":[6,6,6,6,6],\"prh\":[6,6,6,6,6],\"ptbr\":null,\"sc\":0,\"rns\":null,\"iim\":false,\"now\":7776,\"nowpr\":[6,6,6,6,6],\"fs\":{\"aw\":1488.4,\"caw\":0,\"s\":0,\"ts\":12,\"as\":0},\"iwwm\":true,\"twbm\":0,\"bp\":null,\"mnoef\":[1,1,12,1,5],\"fsc\":[0,10,15,0,10,15],\"rsct\":0,\"fsct\":0,\"gwt\":-1,\"pmt\":null,\"ab\":null,\"ml\":1,\"cs\":0.05,\"rl\":[11,11,7,7,7,2,13,13,9,9,9,12,10,10,13,13,3,3,5,8,9,2,13,13,11,11,6,6,6,7,13,13,4,4,9,9,2,11,13,13],\"ctw\":0,\"cwc\":0,\"fstc\":{\"21\":12,\"22\":10},\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"fb\":2,\"sid\":\"1935041401943348225\",\"psid\":\"1935040799565788163\",\"st\":21,\"nst\":1,\"pf\":1,\"aw\":1488.4,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":291641.97,\"blab\":291641.97,\"bl\":291577.92,\"tb\":0,\"tbb\":75,\"tw\":0,\"np\":0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}}";
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
                return "{\"rtp\":{\"Default\":{\"min\":96.8,\"max\":96.8}},\"grtpi\":[{\"gt\":\"Default\",\"grtps\":[{\"t\":\"min\",\"tphr\":null,\"rtp\":96.8},{\"t\":\"max\",\"tphr\":null,\"rtp\":96.8}]}],\"ows\":{\"itare\":true,\"tart\":1,\"igare\":true,\"gart\":2160},\"jws\":null}";
            }
        }
        public JackTheGiantHunterGameLogic()
        {
            _gameID     = GAMEID.JackTheGiantHunter;
            GameName    = "JackTheGiantHunter";
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

            if (!IsNullOrEmpty(jsonParams["twbm"]))
                jsonParams["twbm"] = convertWinByBet((double)jsonParams["twbm"], currentBet);

            
        }
    }
}
