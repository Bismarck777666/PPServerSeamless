using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class MysticalSpiritsGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"wpl\":null,\"lw\":null,\"bwp\":null,\"snww\":null,\"now\":2700,\"nowpr\":[5,3,3,3,4,5],\"ssaw\":0.00,\"esb\":{\"1\":[6,7,8],\"2\":[11,12,13],\"3\":[16,17,18],\"4\":[20,21]},\"ebb\":{\"1\":{\"fp\":6,\"lp\":8,\"bt\":2,\"ls\":1},\"2\":{\"fp\":11,\"lp\":13,\"bt\":2,\"ls\":1},\"3\":{\"fp\":16,\"lp\":18,\"bt\":2,\"ls\":1},\"4\":{\"fp\":20,\"lp\":21,\"bt\":2,\"ls\":1}},\"es\":{\"1\":[6,7,8],\"2\":[11,12,13],\"3\":[16,17,18],\"4\":[20,21]},\"eb\":{\"1\":{\"fp\":6,\"lp\":8,\"bt\":2,\"ls\":1},\"2\":{\"fp\":11,\"lp\":13,\"bt\":2,\"ls\":1},\"3\":{\"fp\":16,\"lp\":18,\"bt\":2,\"ls\":1},\"4\":{\"fp\":20,\"lp\":21,\"bt\":2,\"ls\":1}},\"orl\":[8,9,10,9,8,5,2,2,2,8,6,3,3,3,9,7,4,4,4,10,10,10,0,1,9,8,9,10,9,8],\"sc\":1,\"em\":{\"2\":1,\"3\":1,\"4\":1},\"emb\":null,\"eo\":{\"2\":3,\"3\":3,\"4\":3},\"eob\":null,\"gm\":1,\"imw\":false,\"rs\":null,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.1,\"rl\":[8,9,10,9,8,5,2,2,2,8,6,3,3,3,9,7,4,4,4,10,10,10,0,1,9,8,9,10,9,8],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":22.14,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"wpl\":null,\"lw\":null,\"bwp\":null,\"snww\":null,\"now\":3375,\"nowpr\":[5,3,3,3,5,5],\"ssaw\":0.0,\"esb\":{\"1\":[6,7,8],\"2\":[11,12,13],\"3\":[16,17,18]},\"ebb\":{\"1\":{\"fp\":6,\"lp\":8,\"bt\":2,\"ls\":1},\"2\":{\"fp\":11,\"lp\":13,\"bt\":2,\"ls\":1},\"3\":{\"fp\":16,\"lp\":18,\"bt\":2,\"ls\":1}},\"es\":{\"1\":[6,7,8],\"2\":[11,12,13],\"3\":[16,17,18]},\"eb\":{\"1\":{\"fp\":6,\"lp\":8,\"bt\":2,\"ls\":1},\"2\":{\"fp\":11,\"lp\":13,\"bt\":2,\"ls\":1},\"3\":{\"fp\":16,\"lp\":18,\"bt\":2,\"ls\":1}},\"orl\":[8,9,10,9,8,2,5,5,5,8,3,6,6,6,9,4,7,7,7,10,8,9,10,9,8,8,9,10,9,8],\"sc\":0,\"em\":{\"2\":1,\"3\":1,\"4\":1},\"emb\":{\"2\":1,\"3\":1,\"4\":1},\"eo\":{\"2\":1,\"3\":1,\"4\":1},\"eob\":{\"2\":0,\"3\":0,\"4\":0},\"gm\":1,\"imw\":false,\"rs\":null,\"fs\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.1,\"rl\":[8,9,10,9,8,2,5,5,5,8,3,6,6,6,9,4,7,7,7,10,8,9,10,9,8,8,9,10,9,8],\"sid\":\"1762871162326495234\",\"psid\":\"1762871162326495234\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.75,\"max\":96.75}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public MysticalSpiritsGameLogic()
        {
            _gameID = GAMEID.MysticalSpirits;
            GameName = "MysticalSpirits";
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

        }
    }
}
