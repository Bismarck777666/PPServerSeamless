using GITProtocol;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SlotGamesNode.GameLogics;

namespace BackendServer.GameLogics
{
    internal class SharkHunterGameLogic : BasePGSlotGame
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
                return "{\"si\":{\"wp\":null,\"sw\":null,\"wsc\":null,\"wpl\":null,\"ssaw\":0,\"orl\":[9,9,9,3,3,2,2,2,50,8,10,1,10,5,5,10,1,10,5,5,2,2,2,50,8,9,9,9,3,3],\"rns\":null,\"crtw\":0,\"imw\":false,\"gm\":1,\"ptbr\":null,\"pfmf\":{},\"fmf\":{},\"nfmf\":null,\"mfmf\":null,\"cf\":[false,true,false,false,true,false],\"icp\":false,\"ctbb\":6,\"sc\":0,\"imff\":false,\"fs\":null,\"gwt\":0,\"fb\":null,\"ctw\":0,\"pmt\":null,\"cwc\":0,\"fstc\":null,\"pcwc\":0,\"rwsp\":null,\"hashr\":null,\"ml\":3,\"cs\":0.1,\"rl\":[9,9,9,3,3,2,2,2,50,8,10,1,10,5,5,10,1,10,5,5,2,2,2,50,8,9,9,9,3,3],\"sid\":\"0\",\"psid\":\"0\",\"st\":1,\"nst\":1,\"pf\":0,\"aw\":0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":0,\"blab\":0,\"bl\":16.42,\"tb\":0,\"tbb\":0,\"tw\":0,\"np\":0,\"ocr\":null,\"mr\":null,\"ge\":null}}";
            }
        }
        protected override string ErrorResultString
        {
            get { return "{\"wp\":null,\"sw\":null,\"wsc\":null,\"wpl\":null,\"ssaw\":0,\"orl\":[9,9,9,3,3,2,2,2,8,8,10,10,10,5,5,10,10,10,5,5,2,2,2,8,8,9,9,9,3,3],\"rns\":null,\"crtw\":0,\"imw\":false,\"gm\":1,\"ptbr\":null,\"pfmf\":{},\"fmf\":{},\"nfmf\":null,\"mfmf\":null,\"cf\":[false,false,false,false,false,false],\"icp\":false,\"ctbb\":100,\"sc\":0,\"imff\":false,\"fs\":null,\"gwt\":-1,\"fb\":null,\"ctw\":40,\"pmt\":null,\"cwc\":1,\"fstc\":null,\"pcwc\":1,\"rwsp\":{\"6\":5,\"10\":3},\"hashr\":null,\"ml\":10,\"cs\":0.5,\"rl\":[9,9,9,3,3,2,2,2,8,8,10,10,10,5,5,10,10,10,5,5,2,2,2,8,8,9,9,9,3,3],\"sid\":\"1849579533989189121\",\"psid\":\"1849579533989189121\",\"st\":1,\"nst\":1,\"pf\":1,\"aw\":0,\"wid\":0,\"wt\":\"C\",\"wk\":\"0_C\",\"wbn\":null,\"wfg\":null,\"blb\":16.42,\"blab\":16.42,\"bl\":16.42,\"tb\":0,\"tbb\":0,\"tw\":0,\"np\":0,\"ocr\":null,\"mr\":null,\"ge\":[1,11]}"; }
        }
        protected override string GameRuleString
        {
            get
            {
                return "{\"rtp\":{\"Default\":{\"min\":96.73,\"max\":96.73}},\"ows\":{\"itare\":true,\"tart\":1,\"igare\":true,\"gart\":2160},\"jws\":null}";
            }
        }
        public SharkHunterGameLogic()
        {
            _gameID = GAMEID.SharkHunter;
            GameName = "SharkHunter";
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

            if (!IsNullOrEmpty(jsonParams["ctbb"]))
                jsonParams["ctbb"] = convertWinByBet((double)jsonParams["ctbb"], currentBet);

            jsonParams["hashr"] = null;

            if (!IsNullOrEmpty(jsonParams["sw"]) && IsArrayOrObject(jsonParams["sw"]))
            {
                var swValues = jsonParams["sw"].ToObject<Dictionary<string, object>>();
                foreach (KeyValuePair<string, object> pair in swValues)
                {
                    jsonParams["sw"][pair.Key] = convertWinByBet((double)pair.Value, currentBet);
                }
            }

            if (!IsNullOrEmpty(jsonParams["fs"]) && !IsNullOrEmpty(jsonParams["fs"]["cmbv"]))
                jsonParams["fs"]["cmbv"] = convertWinByBet((double)jsonParams["fs"]["cmbv"], currentBet);

            if (!IsNullOrEmpty(jsonParams["fs"]) && !IsNullOrEmpty(jsonParams["fs"]["pmbv"]))
                jsonParams["fs"]["pmbv"] = convertWinByBet((double)jsonParams["fs"]["pmbv"], currentBet);
        }
    }
}
