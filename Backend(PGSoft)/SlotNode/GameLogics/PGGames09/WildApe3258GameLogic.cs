using GITProtocol;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    internal class WildApe3258GameLogic : BasePGSlotGame
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
            get { return 0.1; }
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
                return "{\"si\":{\"wp\":null,\"lw\":null,\"bwp\":null,\"snww\":null,\"now\":3600,\"nowpr\":[3,4,5,5,4,3],\"es\":{\"1\":[0,1],\"2\":[2,3],\"3\":[4,5],\"4\":[7,8],\"5\":[9,10],\"6\":[14,15],\"7\":[16],\"8\":[20,21],\"9\":[22],\"10\":[25,26],\"11\":[27,28],\"12\":[30,31],\"13\":[32,33],\"14\":[34,35]},\"eb\":{\"1\":{\"fp\":0,\"lp\":1,\"bt\":2,\"ls\":1},\"2\":{\"fp\":2,\"lp\":3,\"bt\":2,\"ls\":1},\"3\":{\"fp\":4,\"lp\":5,\"bt\":2,\"ls\":1},\"4\":{\"fp\":7,\"lp\":8,\"bt\":3,\"ls\":1},\"5\":{\"fp\":9,\"lp\":10,\"bt\":2,\"ls\":1},\"6\":{\"fp\":14,\"lp\":15,\"bt\":2,\"ls\":1},\"7\":{\"fp\":16,\"lp\":16,\"bt\":3,\"ls\":1},\"8\":{\"fp\":20,\"lp\":21,\"bt\":2,\"ls\":1},\"9\":{\"fp\":22,\"lp\":22,\"bt\":3,\"ls\":1},\"10\":{\"fp\":25,\"lp\":26,\"bt\":3,\"ls\":1},\"11\":{\"fp\":27,\"lp\":28,\"bt\":2,\"ls\":1},\"12\":{\"fp\":30,\"lp\":31,\"bt\":2,\"ls\":1},\"13\":{\"fp\":32,\"lp\":33,\"bt\":2,\"ls\":1},\"14\":{\"fp\":34,\"lp\":35,\"bt\":2,\"ls\":1}},\"esb\":{\"1\":[0,1],\"2\":[2,3],\"3\":[4,5],\"4\":[7,8],\"5\":[9,10],\"6\":[14,15],\"7\":[16],\"8\":[20,21],\"9\":[22],\"10\":[25,26],\"11\":[27,28],\"12\":[30,31],\"13\":[32,33],\"14\":[34,35]},\"ebb\":{\"1\":{\"fp\":0,\"lp\":1,\"bt\":2,\"ls\":1},\"2\":{\"fp\":2,\"lp\":3,\"bt\":2,\"ls\":1},\"3\":{\"fp\":4,\"lp\":5,\"bt\":2,\"ls\":1},\"4\":{\"fp\":7,\"lp\":8,\"bt\":3,\"ls\":1},\"5\":{\"fp\":9,\"lp\":10,\"bt\":2,\"ls\":1},\"6\":{\"fp\":14,\"lp\":15,\"bt\":2,\"ls\":1},\"7\":{\"fp\":16,\"lp\":16,\"bt\":3,\"ls\":1},\"8\":{\"fp\":20,\"lp\":21,\"bt\":2,\"ls\":1},\"9\":{\"fp\":22,\"lp\":22,\"bt\":3,\"ls\":1},\"10\":{\"fp\":25,\"lp\":26,\"bt\":3,\"ls\":1},\"11\":{\"fp\":27,\"lp\":28,\"bt\":2,\"ls\":1},\"12\":{\"fp\":30,\"lp\":31,\"bt\":2,\"ls\":1},\"13\":{\"fp\":32,\"lp\":33,\"bt\":2,\"ls\":1},\"14\":{\"fp\":34,\"lp\":35,\"bt\":2,\"ls\":1}},\"ptbr\":null,\"orl\":[8,8,7,7,9,9,12,2,2,1,1,11,4,0,99,99,10,8,4,0,99,99,10,8,12,2,2,1,1,11,8,8,7,7,9,9],\"ssaw\":0.00,\"sc\":2,\"mwbs\":false,\"gm\":1,\"twbm\":0.0,\"crtw\":0.0,\"imw\":false,\"rsc\":0,\"rs\":null,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.01,\"rl\":[8,8,7,7,9,9,12,2,2,1,1,11,4,0,99,99,10,8,4,0,99,99,10,8,12,2,2,1,1,11,8,8,7,7,9,9],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":5.61,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"lw\":null,\"bwp\":null,\"snww\":null,\"now\":3600,\"nowpr\":[3,4,5,5,4,3],\"es\":{\"1\":[0,1],\"2\":[2,3],\"3\":[4,5],\"4\":[7,8],\"5\":[9,10],\"6\":[14,15],\"7\":[20,21],\"8\":[25,26],\"9\":[27,28],\"10\":[30,31],\"11\":[32,33],\"12\":[34,35]},\"eb\":{\"1\":{\"fp\":0,\"lp\":1,\"bt\":2,\"ls\":1},\"2\":{\"fp\":2,\"lp\":3,\"bt\":2,\"ls\":1},\"3\":{\"fp\":4,\"lp\":5,\"bt\":2,\"ls\":1},\"4\":{\"fp\":7,\"lp\":8,\"bt\":2,\"ls\":1},\"5\":{\"fp\":9,\"lp\":10,\"bt\":2,\"ls\":1},\"6\":{\"fp\":14,\"lp\":15,\"bt\":2,\"ls\":1},\"7\":{\"fp\":20,\"lp\":21,\"bt\":2,\"ls\":1},\"8\":{\"fp\":25,\"lp\":26,\"bt\":2,\"ls\":1},\"9\":{\"fp\":27,\"lp\":28,\"bt\":2,\"ls\":1},\"10\":{\"fp\":30,\"lp\":31,\"bt\":2,\"ls\":1},\"11\":{\"fp\":32,\"lp\":33,\"bt\":2,\"ls\":1},\"12\":{\"fp\":34,\"lp\":35,\"bt\":2,\"ls\":1}},\"esb\":{\"1\":[0,1],\"2\":[2,3],\"3\":[4,5],\"4\":[7,8],\"5\":[9,10],\"6\":[14,15],\"7\":[20,21],\"8\":[25,26],\"9\":[27,28],\"10\":[30,31],\"11\":[32,33],\"12\":[34,35]},\"ebb\":{\"1\":{\"fp\":0,\"lp\":1,\"bt\":2,\"ls\":1},\"2\":{\"fp\":2,\"lp\":3,\"bt\":2,\"ls\":1},\"3\":{\"fp\":4,\"lp\":5,\"bt\":2,\"ls\":1},\"4\":{\"fp\":7,\"lp\":8,\"bt\":2,\"ls\":1},\"5\":{\"fp\":9,\"lp\":10,\"bt\":2,\"ls\":1},\"6\":{\"fp\":14,\"lp\":15,\"bt\":2,\"ls\":1},\"7\":{\"fp\":20,\"lp\":21,\"bt\":2,\"ls\":1},\"8\":{\"fp\":25,\"lp\":26,\"bt\":2,\"ls\":1},\"9\":{\"fp\":27,\"lp\":28,\"bt\":2,\"ls\":1},\"10\":{\"fp\":30,\"lp\":31,\"bt\":2,\"ls\":1},\"11\":{\"fp\":32,\"lp\":33,\"bt\":2,\"ls\":1},\"12\":{\"fp\":34,\"lp\":35,\"bt\":2,\"ls\":1}},\"ptbr\":null,\"orl\":[8,8,7,7,9,9,12,2,2,5,5,11,4,3,99,99,10,8,4,3,99,99,10,8,12,2,2,5,5,11,8,8,7,7,9,9],\"ssaw\":0.00,\"sc\":0,\"mwbs\":false,\"gm\":1,\"twbm\":0.0,\"crtw\":0.0,\"imw\":false,\"rsc\":0,\"rs\":null,\"fs\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":10,\"cs\":0.5,\"rl\":[8,8,7,7,9,9,12,2,2,5,5,11,4,3,99,99,10,8,4,3,99,99,10,8,12,2,2,5,5,11,8,8,7,7,9,9],\"sid\":\"1775204964134692868\",\"psid\":\"1775204964134692868\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":5.61,\"blab\":5.61,\"bl\":5.61,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.74,\"max\":96.74}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public WildApe3258GameLogic()
        {
            _gameID = GAMEID.WildApe3258;
            GameName = "WildApe3258";
        }
        protected override void initGameData()
        {
            base.initGameData();
            _pgGameConfig.ml.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            _pgGameConfig.cs.AddRange(new double[] { 0.01, 0.05, 0.1 });
        }
        protected override void convertWinsByBet(dynamic jsonParams, float currentBet)
        {
            base.convertWinsByBet((object)jsonParams, currentBet);            
        }
    }
}
