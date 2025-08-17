using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class HawaiianTikiGameLogic : BasePGSlotGame
    {
        protected override bool SupportPurchaseFree
        {
            get { return false; }
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
                return "{\"si\":{\"wp\":null,\"lw\":null,\"snw\":null,\"wpl\":null,\"ptbr\":null,\"orl\":null,\"otw\":0.0,\"ewp\":[[7,8,9,10,11],[12,13,14,15,16]],\"ewpb\":[[7,8,9,10,11],[12,13,14,15,16]],\"ewl\":[5,5],\"oew\":[9,14],\"sc\":2,\"gm\":0,\"gmi\":0,\"gmib\":0,\"ssaw\":0.0,\"rs\":null,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.1,\"rl\":[9,6,11,2,1,12,4,0,0,0,0,0,0,0,0,0,0,3,1,9,7,5,10,8],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0.00,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.00,\"blab\":0.00,\"bl\":22.14,\"tb\":0.00,\"tbb\":0.00,\"tw\":0.00,\"np\":0.00,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"lw\":null,\"snw\":null,\"wpl\":null,\"ptbr\":null,\"orl\":[9,6,11,2,11,12,4,7,9,10,3,5,4,12,11,2,8,3,10,9,7,5,10,8],\"otw\":0.0,\"ewp\":null,\"ewpb\":null,\"ewl\":null,\"oew\":null,\"sc\":0,\"gm\":1,\"gmi\":0,\"gmib\":0,\"ssaw\":0.0,\"rs\":null,\"fs\":null,\"gwt\":-1,\"fb\":null,\"ctw\":0.0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.1,\"rl\":[9,6,11,2,11,12,4,7,9,10,3,5,4,12,11,2,8,3,10,9,7,5,10,8],\"sid\":\"1762870844503143425\",\"psid\":\"1762870844503143425\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0.0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0.0,\"blab\":0.0,\"bl\":0.0,\"tb\":0.0,\"tbb\":0.0,\"tw\":0.0,\"np\":0.0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.76,\"max\":96.76}},\"ows\":{\"itare\":false,\"tart\":0,\"igare\":false,\"gart\":0},\"jws\":null}";
            }
        }
        public HawaiianTikiGameLogic()
        {
            _gameID = GAMEID.HawaiianTiki;
            GameName = "HawaiianTiki";
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
            if (!IsNullOrEmpty(jsonParams["otw"]))
                jsonParams["otw"] = convertWinByBet((double)jsonParams["otw"], currentBet);

            if (!IsNullOrEmpty(jsonParams["ctw"]))
                jsonParams["ctw"] = convertWinByBet((double)jsonParams["ctw"], currentBet);

        }
    }
}
