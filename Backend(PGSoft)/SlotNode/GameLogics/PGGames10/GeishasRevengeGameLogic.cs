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
    internal class GeishasRevengeGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"lw\":null,\"bwp\":null,\"orl\":[3,3,7,2,9,8,8,8,6,6,8,5,5,5,5,9,9,6,6,6,7,8,8,6,6,4,7,3,3],\"now\":1800,\"nowpr\":[5,5,3,4,6],\"snww\":null,\"esb\":{\"1\":[6,7],\"2\":[11,12],\"3\":[13,14],\"4\":[15,16],\"5\":[18,19],\"6\":[21,22]},\"ebb\":{\"1\":{\"fp\":6,\"lp\":7,\"bt\":2,\"ls\":1},\"2\":{\"fp\":11,\"lp\":12,\"bt\":2,\"ls\":1},\"3\":{\"fp\":13,\"lp\":14,\"bt\":2,\"ls\":1},\"4\":{\"fp\":15,\"lp\":16,\"bt\":2,\"ls\":1},\"5\":{\"fp\":18,\"lp\":19,\"bt\":2,\"ls\":1},\"6\":{\"fp\":21,\"lp\":22,\"bt\":2,\"ls\":1}},\"es\":{\"1\":[6,7],\"2\":[11,12],\"3\":[13,14],\"4\":[15,16],\"5\":[18,19],\"6\":[21,22]},\"eb\":{\"1\":{\"fp\":6,\"lp\":7,\"bt\":2,\"ls\":1},\"2\":{\"fp\":11,\"lp\":12,\"bt\":2,\"ls\":1},\"3\":{\"fp\":13,\"lp\":14,\"bt\":2,\"ls\":1},\"4\":{\"fp\":15,\"lp\":16,\"bt\":2,\"ls\":1},\"5\":{\"fp\":18,\"lp\":19,\"bt\":2,\"ls\":1},\"6\":{\"fp\":21,\"lp\":22,\"bt\":2,\"ls\":1}},\"ssaw\":0,\"ptbr\":null,\"sc\":0,\"crtw\":0,\"imw\":false,\"mf\":{\"0\":1,\"1\":4,\"2\":4,\"3\":2,\"4\":2},\"omf\":{\"0\":0,\"1\":2,\"2\":4,\"3\":2,\"4\":2},\"cgm\":12,\"gm\":0,\"ir\":false,\"rs\":null,\"fs\":{\"aw\":56.45,\"s\":0,\"ts\":10,\"as\":0},\"gwt\":-1,\"pmt\":null,\"ab\":null,\"ml\":1,\"cs\":0.05,\"rl\":[3,3,7,2,9,8,8,8,6,6,8,5,5,5,5,9,9,6,6,6,7,8,8,6,6,4,7,3,3],\"ctw\":0,\"cwc\":0,\"fstc\":{\"21\":10,\"22\":5},\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"fb\":2,\"sid\":\"1934874313274526211\",\"psid\":\"1934873891621056516\",\"st\":21,\"nst\":1,\"pf\":1,\"aw\":56.45,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":290168.7,\"blab\":290168.7,\"bl\":291603.67,\"tb\":0,\"tbb\":75,\"tw\":0,\"np\":0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}}";
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
                return "{\"rtp\":{\"Default\":{\"min\":96.78,\"max\":96.78}},\"grtpi\":[{\"gt\":\"Default\",\"grtps\":[{\"t\":\"min\",\"tphr\":null,\"rtp\":96.78},{\"t\":\"max\",\"tphr\":null,\"rtp\":96.78}]}],\"ows\":{\"itare\":true,\"tart\":1,\"igare\":true,\"gart\":2160},\"jws\":null}";
            }
        }
        public GeishasRevengeGameLogic()
        {
            _gameID     = GAMEID.GeishasRevenge;
            GameName    = "GeishasRevenge";
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
