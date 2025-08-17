using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SlotGamesNode.GameLogics
{
    public class BigJuanGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs40bigjuan";
            }
        }
        protected override bool SupportReplay
        {
            get
            {
                return true;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 40;
            }
        }
        protected override int ServerResLineCount
        {
            get { return 40; }
        }
        protected override int ROWS
        {
            get
            {
                return 4;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=12,5,7,11,3,12,5,2,11,10,11,5,5,11,10,10,5,10,11,7&cfgs=4984&accm=cp~tp~lvl~sc;cp~tp~lvl~sc;cp~tp~lvl~sc;cp~tp~lvl~sc&ver=2&mo_s=15&acci=1;2;3;4&mo_v=20,40,80,120,200,320,400,600,800,1000,1600,2000,4000,5000,8000,10000&def_sb=4,10,10,7,8&reel_set_size=4&def_sa=8,8,9,6,3&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"3119152\",jp1:\"100000\",max_rnd_win:\"2600\",jp3:\"2000\",jp2:\"10000\",jp4:\"500\"}}&wl_i=tbm~2600&sc=5.00,10.00,20.00,30.00,40.00,50.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,2000.00,2500.00&defc=50.00&purInit_e=1&wilds=2~500,150,50,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;250,100,25,0,0;200,80,20,0,0;150,40,15,0,0;100,20,10,0,0;100,20,10,0,0;40,10,5,0,0;40,10,5,0,0;40,10,5,0,0;40,10,5,0,0;40,10,5,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=10,000,000.00&reel_set0=12,3,6,13,8,3,13,7,8,11,11,8,5,12,8,11,11,7,4,11,12,3,9,3,12,13,10,7,12,3,3,3,3,4,5,1,13,2,3,5,5,8,12,1,5,12,9,11,6,11,13,2,8,8,13,8,7,7,11,8,12,11,2,10,7~10,10,8,9,3,9,9,9,9,2,10,13,5,6,13,8,13,13,13,13,1,5,6,2,4,4,8,8,8,8,4,5,11,10,13,9,11,11,11,11,6,8,12,4,13,6,7,10,10,10,10,11,10,4,6,13,11,11,6,6,6,6,8,11,9,10,8,9,9,5~12,9,9,2,12,11,3,13,9,2,4,13,6,10,12,13,13,13,13,8,12,7,5,3,10,9,7,12,11,7,12,10,9,5,12,9,9,9,9,5,12,13,5,10,12,2,7,3,10,6,3,4,8,9,12,12,12,10,3,9,13,3,9,10,9,13,13,5,1,9,12,10,13,10~4,2,4,10,13,6,4,8,6,13,13,13,13,6,10,2,11,6,10,7,8,11,13,11,11,11,11,1,9,10,7,13,5,8,12,8,8,6,6,6,11,11,13,13,11,4,7,12,7,11,8,8,8,8,7,8,1,11,13,10,10,8,4,7,7,7,7,3,6,10,5,4,9,6,2,6,3,7~3,6,5,13,7,7,5,7,5,9,3,10,9,5,9,9,12,8,9,3,6,6,8,3,3,13,9,6,3,12,9,2,5,3,3,3,3,1,11,9,12,11,13,11,2,10,12,5,9,7,5,6,7,1,11,8,4,7,9,13,3,13,12,8,6,12,5,6,13,7,7,13,13,13,3,12,6,8,13,12,11,7,7,3,5,2,12,11,11,10,7,13,9,13,4,11,13,9,11,13,13,6,5,5,12,13,2,3,10&accInit=[{id:1,mask:\"cp;tp;lvl;sc;cl\"},{id:2,mask:\"cp;tp;lvl;sc;cl\"},{id:3,mask:\"cp;tp;lvl;sc;cl\"},{id:4,mask:\"cp;tp;lvl;sc;cl\"}]&reel_set2=23,22,14,23,14,23,23,23,22,22,14,14,23,14,23,22,22,22,14,22,22,14,23,14,14&reel_set1=14,14,14,14,14~14,14,14,14,14~14,14,14,14,14&purInit=[{type:\"fsbl\",bet:4000,bet_level:0}]&reel_set3=15,24,15,15,21,24,14,14,15,14,24,14,21,15,14,15,15,15,15,14,15,15,14,15,24,15,15,21,15,14,14,15,24,14,24,24,24,15,24,14,15,14,24,24,21,15,21,14,15,14,15,14,21,14&total_bet_min=5.00";
            }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 100.0; }
        }
        protected override bool SupportPurchaseFree
        {
            get {  return true; }   
        }
        protected override bool HasPurEnableOption
        {
            get { return true; }
        }
        #endregion
        public BigJuanGameLogic()
        {
            _gameID = GAMEID.BigJuan;
            GameName = "BigJuan";
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


                BasePPSlotBetInfo betInfo = new BasePPSlotBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();

                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in BigJuanGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }
                if (!isNotIntergerMultipleBetPerLine(betInfo.BetPerLine, minChip))
                {
                    _logger.Error("{0} betInfo.BetPerLine is illegual: {1} != {2} * integer", strGlobalUserID, betInfo.BetPerLine, minChip);
                    return;
                }

                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1} != {2}", strGlobalUserID, betInfo.LineCount, this.ClientReqLineCount);
                    return;
                }
                BasePPSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strGlobalUserID, out oldBetInfo))
                {
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine   = betInfo.BetPerLine;
                    oldBetInfo.LineCount    = betInfo.LineCount;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BigJuanGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["reel_set"] = "0";
            dicParams["st"] = "rect";
            dicParams["sw"] = "5";
            dicParams["accv"] = "0~5~0~0;0~5~0~0;0~4~0~0;0~3~0~0";
            dicParams["g"] = "{fs_collect:{def_s:\"22,14,14\",def_sa:\"23\",def_sb:\"22\",reel_set:\"2\",s:\"22,14,14\",sa:\"23\",sb:\"22\",sh:\"3\",st:\"rect\",sw:\"1\"},fs_main:{def_s:\"15,15,18,14,16,14,15,14,20\",def_sa:\"14,14,14\",def_sb:\"14,14,14\",reel_set:\"1\",s:\"15,15,18,14,16,14,15,14,20\",sa:\"14,14,14\",sb:\"14,14,14\",sh:\"3\",st:\"rect\",sw:\"3\"}}";
        }
        protected override void supplementInitResult(Dictionary<string, string> dicParams, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {
            if(!dicParams.ContainsKey("g"))
                dicParams["g"] = "{fs_collect:{def_s:\"22,14,14\",def_sa:\"23\",def_sb:\"22\",reel_set:\"2\",s:\"22,14,14\",sa:\"23\",sb:\"22\",sh:\"3\",st:\"rect\",sw:\"1\"},fs_main:{def_s:\"15,15,18,14,16,14,15,14,20\",def_sa:\"14,14,14\",def_sb:\"14,14,14\",reel_set:\"1\",s:\"15,15,18,14,16,14,15,14,20\",sa:\"14,14,14\",sb:\"14,14,14\",sh:\"3\",st:\"rect\",sw:\"3\"}}";
            if (!dicParams.ContainsKey("reel_set"))
                dicParams["reel_set"] = "0";
        }

        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            if (dicParams.ContainsKey("puri") && !dicParams.ContainsKey("purtr"))
                dicParams.Remove("puri");
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("apwa"))
            {
                string   strApwa  = dicParams["apwa"];
                string[] strParts = strApwa.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strParts.Length; i++)
                    strParts[i] = convertWinByBet(strParts[i], currentBet);

                dicParams["apwa"] = string.Join(",", strParts);
            }

            if (dicParams.ContainsKey("g"))
            {
                string strG = dicParams["g"];
                JToken gObj = JsonConvert.DeserializeObject<JToken>(strG);
                if(gObj["fs_main1"] != null && gObj["fs_main"]["mo_tw"] != null)
                {
                    gObj["fs_main"]["mo_tw"] = convertWinByBet(gObj["fs_main"]["mo_tw"].ToString(), currentBet);
                    dicParams["g"] = JsonConvert.SerializeObject(gObj);
                }
            }
        }
    }
}
