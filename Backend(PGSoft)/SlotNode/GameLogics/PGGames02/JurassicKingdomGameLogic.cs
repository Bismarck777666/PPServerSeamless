using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class JurassicKingdomGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"bwp\":null,\"lw\":null,\"lwa\":0.00,\"orl\":null,\"now\":1920,\"nowpr\":[4,2,4,5,3,4],\"snww\":null,\"esb\":{\"1\":[1,2,3],\"2\":[6,7,8],\"3\":[9,10,11],\"4\":[12,13,14],\"5\":[21,22],\"6\":[24,25],\"7\":[27,28,29],\"8\":[32,33,34]},\"ebb\":{\"1\":{\"fp\":1,\"lp\":3,\"bt\":2,\"ls\":2},\"2\":{\"fp\":6,\"lp\":8,\"bt\":1,\"ls\":1},\"3\":{\"fp\":9,\"lp\":11,\"bt\":1,\"ls\":2},\"4\":{\"fp\":12,\"lp\":14,\"bt\":2,\"ls\":2},\"5\":{\"fp\":21,\"lp\":22,\"bt\":2,\"ls\":2},\"6\":{\"fp\":24,\"lp\":25,\"bt\":1,\"ls\":2},\"7\":{\"fp\":27,\"lp\":29,\"bt\":1,\"ls\":1},\"8\":{\"fp\":32,\"lp\":34,\"bt\":2,\"ls\":1}},\"es\":{\"1\":[1,2,3],\"2\":[6,7,8],\"3\":[9,10,11],\"4\":[12,13,14],\"5\":[21,22],\"6\":[24,25],\"7\":[27,28,29],\"8\":[32,33,34]},\"eb\":{\"1\":{\"fp\":1,\"lp\":3,\"bt\":2,\"ls\":2},\"2\":{\"fp\":6,\"lp\":8,\"bt\":1,\"ls\":1},\"3\":{\"fp\":9,\"lp\":11,\"bt\":1,\"ls\":2},\"4\":{\"fp\":12,\"lp\":14,\"bt\":2,\"ls\":2},\"5\":{\"fp\":21,\"lp\":22,\"bt\":2,\"ls\":2},\"6\":{\"fp\":24,\"lp\":25,\"bt\":1,\"ls\":2},\"7\":{\"fp\":27,\"lp\":29,\"bt\":1,\"ls\":1},\"8\":{\"fp\":32,\"lp\":34,\"bt\":2,\"ls\":1}},\"ssaw\":0.00,\"rs\":null,\"fs\":null,\"pr\":null,\"sc\":0,\"mi\":0,\"ma\":[1,2,3,4,5],\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"ml\":3,\"cs\":10.0,\"rl\":[8,4,4,4,1,12,2,2,2,3,3,3,0,0,0,6,11,5,2,0,9,1,1,7,4,4,0,5,5,5,12,5,11,11,11,1],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":20794.50,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"bwp\":{\"11\":[[2],[3],[10,11],[13,14]]},\"lw\":null,\"lwa\":0.3,\"orl\":[8,4,4,4,1,12,2,2,2,3,3,3,5,4,4,4,11,8,2,2,9,12,7,7,4,4,10,5,5,5,12,5,11,11,11,1],\"now\":1920,\"nowpr\":[4,2,4,5,3,4],\"snww\":{\"11\":2},\"esb\":{\"1\":[1,2,3],\"2\":[6,7,8],\"3\":[9,10,11],\"4\":[13,14,15],\"5\":[18,19],\"6\":[24,25],\"7\":[27,28,29],\"8\":[32,33,34]},\"ebb\":{\"1\":{\"fp\":1,\"lp\":3,\"bt\":2,\"ls\":1},\"2\":{\"fp\":6,\"lp\":8,\"bt\":2,\"ls\":1},\"3\":{\"fp\":9,\"lp\":11,\"bt\":2,\"ls\":1},\"4\":{\"fp\":13,\"lp\":15,\"bt\":2,\"ls\":1},\"5\":{\"fp\":18,\"lp\":19,\"bt\":2,\"ls\":1},\"6\":{\"fp\":24,\"lp\":25,\"bt\":2,\"ls\":1},\"7\":{\"fp\":27,\"lp\":29,\"bt\":2,\"ls\":1},\"8\":{\"fp\":32,\"lp\":34,\"bt\":2,\"ls\":1}},\"es\":{\"1\":[1,2,3],\"2\":[6,7,8],\"3\":[9,10,11],\"4\":[13,14,15],\"5\":[18,19],\"6\":[24,25],\"7\":[27,28,29],\"8\":[32,33,34]},\"eb\":{\"1\":{\"fp\":1,\"lp\":3,\"bt\":2,\"ls\":1},\"2\":{\"fp\":6,\"lp\":8,\"bt\":2,\"ls\":1},\"3\":{\"fp\":9,\"lp\":11,\"bt\":2,\"ls\":1},\"4\":{\"fp\":13,\"lp\":15,\"bt\":2,\"ls\":1},\"5\":{\"fp\":18,\"lp\":19,\"bt\":2,\"ls\":1},\"6\":{\"fp\":24,\"lp\":25,\"bt\":2,\"ls\":1},\"7\":{\"fp\":27,\"lp\":29,\"bt\":2,\"ls\":1},\"8\":{\"fp\":32,\"lp\":34,\"bt\":2,\"ls\":1}},\"ssaw\":0.0,\"rs\":null,\"fs\":null,\"pr\":[2,3,10,11,13,14],\"sc\":2,\"mi\":0,\"ma\":[1,2,3,4,5],\"gwt\":-1,\"fb\":null,\"ctw\":0.3,\"pmt\":null,\"cwc\":1,\"fstc\":null,\"pcwc\":1,\"rwsp\":{\"11\":1.0},\"hashr\":null,\"ml\":3,\"cs\":0.05,\"rl\":[8,4,4,4,1,12,2,2,2,3,3,3,5,4,4,4,11,8,2,2,9,12,7,7,4,4,10,5,5,5,12,5,11,11,11,1],\"sid\":\"1762869782685696517\",\"psid\":\"1762869782685696517\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.72,\"max\":96.72}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public JurassicKingdomGameLogic()
        {
            _gameID = GAMEID.JurassicKingdom;
            GameName = "JurassicKingdom";
        }
        protected override void initGameData()
        {
            base.initGameData();
            _pgGameConfig.ml.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
            _pgGameConfig.cs.AddRange(new double[] { 0.01, 0.05, 0.1 });
        }
        protected override void convertWinsByBet(dynamic jsonParams, float currentBet)
        {
            base.convertWinsByBet((object) jsonParams, currentBet);
            if (!IsNullOrEmpty(jsonParams["lwa"]))
                jsonParams["lwa"] = convertWinByBet((double)jsonParams["lwa"], currentBet);
        }
    }
}
