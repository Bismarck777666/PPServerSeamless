using GITProtocol;
using Newtonsoft.Json.Linq;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol.Utils;

namespace SlotGamesNode.GameLogics
{
    public class SpellMasterBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get
            {
                return BetPerLine * 10.0f;
            }
        }
    }
    class SpellMasterGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs10spellmastr";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 50; }
        }
        protected override int ServerResLineCount
        {
            get { return 10; }
        }
        protected override int ROWS
        {
            get
            {
                return 3;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=4,5,8,12,11,3,9,9,3,9,12,11,12,4,7&cfgs=1&ver=3&def_sb=12,9,11,6,11&reel_set_size=4&def_sa=10,5,10,10,11&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1;14~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1;15~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1;16~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{purchase:\"96.50\",regular:\"96.50\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"24199902\",max_rnd_win:\"40000\"}}&wl_i=tbm~40000&sc=20.00,40.00,60.00,80.00,100.00,200.00,300.00,400.00,500.00,750.00,1000.00,1500.00,2500.00,5000.00,7500.00,10000.00&defc=100.00&purInit_e=1&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;40,10,3,0,0;20,5,2,0,0;20,5,2,0,0;20,5,2,0,0;10,2,1,0,0;10,2,1,0,0;10,2,1,0,0;10,2,1,0,0;10,2,1,0,0;10,2,1,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=5,000,000.00&reel_set0=3,9,12,6,12,3,5,8,12,5,11,10,7,8,9,6,7,8,9,4,10,4,15,3,5,7,4,7,4,8,10,12,8,4,10,4,9,3,7,10,3,8,5,7,5,12,3,10,6,11,5,11,10,8,7,11,14,11,12,3,4,5,4,11,12,6,3,10,12,5,4,7,4,6,10,3,8,5,11,9,10,4,16~2,9,12,3,7,9,9,7,3,4,6,12,2,4,6,9,9,3,6,2,10,7,12,5,7,11,8,11,12,10,5,12,9,4,16,4,9,9,10,4,8,12,7,12,11,6,9,4,2,2,2,2,3,7,9,12,8,12,11,10,8,3,10,15,7,9,7,5,11,3,6,11,2,11,3,10,6,2,2,6,7,14,8,9,6,4,6,9,11,4,10,7,5,11,8,3,2,4,2,9,7~2,3,5,10,7,5,7,8,7,11,3,4,3,6,8,14,8,3,7,4,9,5,8,10,4,11,10,4,11,9,3,2,5,7,2,7,2,2,2,10,5,10,12,16,4,5,6,4,11,2,11,2,11,2,2,6,11,5,9,2,10,9,6,7,5,9,10,15,6,9,8,4,2,4,11,6,11,6,5,2~5,4,10,8,6,10,6,4,12,4,12,11,8,4,6,10,11,4,3,5,9,10,8,9,2,9,10,6,8,6,12,2,4,16,2,2,2,2,5,15,6,2,8,11,12,8,9,3,2,4,7,11,8,2,5,7,5,12,14,5,7,12,7,5,8,5,7,2,6,7,12,2,2,2~12,2,8,10,6,3,7,9,8,12,12,2,9,3,8,7,2,9,14,12,3,7,11,9,15,12,11,7,6,12,2,9,3,9,4,11,12,7,4,3,9,4,2,6,12,7,2,2,2,2,3,7,9,12,6,4,2,8,3,6,7,4,10,2,12,5,2,5,9,3,4,11,2,2,2,12,10,4,3,8,12,7,3,12,5,9,10,6,6,7,3,6,6,7,16,5,9,8,7,2,10&accInit=[{id:0,mask:\"bcp;bcd\"},{id:1,mask:\"bjp;bjd\"},{id:2,mask:\"bmp;bmd\"},{id:3,mask:\"jpminp;jpmind\"},{id:4,mask:\"jpmnrp;jpmnrd\"},{id:5,mask:\"jpmjrp;jpmjrd\"},{id:6,mask:\"jpmegp;jpmegd\"},{id:7,mask:\"jpgrnp;jpgrnd\"}]&reel_set2=5,1,12,4,1,9,11,3,10,6,1,7,3,1,5,1,7,4,1,4,1,10,1,11,4,3,12,1,4,1,11,6,1,8,1,9,3,10,4,1,8,1,8,1,8,12,1,10,1,12,1,9,11,1,9,1,4,1,7,5,9,1,3,5,3,5,3,10,5,11,6,1,3,7,10,1,7,1,5,4,12,4,3~1,7,5,1,8,5,12,1,12,1,11,1,11,3,7,1,7,10,12,7,6,1,3,9,1,11,6,12,4,9,8,5,1,9,1,3,12,1,7,6,5,11,9,7,5,1,6,1,12,1,12,1,11,12,9,3,10,9,4,7,1,4,9,1,11,1,12,1,4,1,8,1,8,1,7,5,10,9,1,11,1,11,9,10,1,3,6,1,11,1,9,7,12,1,11,1,4,1,11,8~6,1,4,10,1,10,12,1,9,1,5,11,1,8,9,1,6,3,11,10,11,3,1,3,1,7,1,4,1,6,11,10,6,8,11,10,9,1,4,3,5,9,11,7,5,1,12,3,10,1,7,8,3,9~5,1,4,1,6,1,5,7,1,12,5,3,1,12,5,8,5,8,6,1,10,4,10,6,1,5,1,11,12,7,1,9,1,9,1,5,7,1,8,5,1,9,11,8,9,1,11,1,6,1,4,6,1,12,1,12,5,1,6~1,3,12,7,1,9,6,4,9,1,7,6,3,9,1,12,1,12,7,1,6,9,1,12,8,1,9,10,1,9,3,10,3,9,1,12,4,12,6,1,9,1,9,4,11,1,12,1,12,1,6,5,11,1,8,7,8,3,1,6,7,1,7,10,3,5,6,3,1,10,6,1,9,7,3,7,12,9,1,9,1,12,10,1,12,3,8,1,12,4,1,11,3,1,12,12,1,8,3,1,7,1,8,1,8,1,9,3,1,7,1,6,7&reel_set1=12,8,14,7,4,5,15,8,14,4,9,16,5,15,12,10,14,12,15,3,16,7,8,3,14,6,9,5,15,8,16,9,7,11,6,7,12,5,16,8,9,3,15,11,7,14,5,4,11,10,12,16,8,4,12,10,15,3,14,4,5,7,3,16,10,8,3,4,12,4,9,4,14,7,8,12,3,8,15,6,5,16,4,11,9,8,9,16,7,5,12,8,4,14,6,10,11,16,5,15,4,11,11,10~8,11,15,9,9,14,10,3,5,9,16,2,6,15,5,10,14,12,8,7,4,14,6,16,3,12,9,16,10,4,14,10,15,12,2,2,2,2,10,16,3,8,11,15,7,12,14,7,14,11,12,7,16,9,7,15,9,11,14,2,9,7,6,16,4,5,12,15,12,6,4,6,4,2~4,16,4,15,7,12,14,10,4,2,7,2,14,4,12,2,8,6,16,2,10,11,10,5,14,10,15,6,2,5,10,9,2,16,2,15,4,2,2,2,11,16,2,15,10,4,3,2,14,9,11,14,6,5,10,3,6,3,16,11,3,7,9,3,4,15,7,9,3,16,5,8,11,5,2,5,14,5,11,5~8,14,2,15,2,5,16,5,14,8,14,12,4,4,10,15,9,8,2,7,16,11,14,10,9,5,12,10,5,3,15,9,12,4,16,5,3,11,15,2,2,2,2,6,14,8,10,9,6,9,16,4,6,5,8,2,7,15,8,12,6,3,14,2,3,2,16,15,2,4,7,14,6,12,2,2,9,16,11,7,14,11,5,12~2,16,7,12,3,15,6,14,9,12,12,9,11,15,7,16,6,12,3,12,14,3,12,9,15,4,2,12,14,6,5,8,11,6,6,15,3,4,8,4,9,2,8,2,12,12,9,15,10,16,7,5,3,2,2,2,2,4,14,9,11,7,2,11,12,16,9,2,12,15,12,6,9,4,2,8,2,14,9,2,7,16,3,10,2,15,7,9,2,14,10,3,9,5,8,6,12,2,3,12,6,16,4,9,15,12,7,10,14,4,7,9&purInit=[{bet:500,type:\"default\"}]&reel_set3=9,14,3,16,10,9,3,15,9,15,4,5,4,5,3,9,5,10,9,5,3,16,3,15,5,14,10,4,3,14,4,16,9,15,4,3,16,4,5,16,9,14,5,4,10,4,15,5,16,5,4,5,4,5,15,5,10,4,14,9,3,16,10,9,4,14,10,14,3,16,10,15,5,15,9,4,10,9,4,3,14,5,3,9,4,5,16,15,5,3,14,10,4~8,6,12,7,14,12,7,6,12,16,12,15,7,16,11,12,14,6,12,15,11,12,16,12,7,15,8,12,6,12,15,12,7,14,12,16,12,16,11,14,7,6,11,14,11,6,11,8,12,15,11,6,16,7,11,6,8,7,11,15,7,11,16,12,8,6,11,14,12,8,6,12,7,16,7,15,8,14,7,15,16,11,15,14,7~14,3,10,15,6,8,15,5,9,10,16,4,11,7,3,11,14,9,10,4,16,9,6,14,3,9,3,15,11,16,12,14,10,11,3,10,15,8,14,11,14,5,3,8,16,7,4,11,6,7,15,6,12,10~11,8,2,12,10,8,5,12,6,12,6,8,7,2,6,5,11,3,9,4,5,2,5,8,4,5,4,12,11,3,4,7,4,10,9,12,2,5,2,2,2,2,6,11,9,10,5,4,3,8,2,12,5,2,12,2,3,2,9,5,12,6,8,10,7,9,11,8,7,6,5,6,10,9,4,8,4,3,12,7,6,8~7,3,7,12,3,10,2,8,9,7,6,2,8,6,2,7,12,9,8,12,4,9,7,3,10,2,12,9,6,2,2,12,5,11,9,4,7,6,12,3,5,7,9,6,2,3,9,5,8,9,10,11,3,4,2,2,2,2,7,12,8,4,11,9,12,5,7,6,7,4,2,3,6,3,2,4,12,12,3,2,3,7,2,11,12,6,11,7,9,12,8,10,12,3,9,8,10,12,6,12,3,9,4,12,3,4,7,6,9,10,7,6,2&total_bet_min=200.00";
            }
        }
	
        protected override double PurchaseFreeMultiple
        {
            get { return 50; }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override bool HasPurEnableOption
        {
            get { return true; }
        }
	
	
        #endregion
        public SpellMasterGameLogic()
        {
            _gameID = GAMEID.SpellMaster;
            GameName = "SpellMaster";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["g"] = "{jp:{def_s:\"9,12,10,3,12,8,4,7,5,7,3,12,3,9,10,5,7,6,5,4,3,12,8,9,12,11,10,7,5,10\",def_sa:\"3,12,10,9,4\",def_sb:\"8,6,12,5,7\",s:\"9,12,10,3,12,8,4,7,5,7,3,12,3,9,10,5,7,6,5,4,3,12,8,9,12,11,10,7,5,10\",sa:\"3,12,10,9,4\",sb:\"8,6,12,5,7\",sh:\"6\",st:\"rect\",sw:\"5\"}}";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            
            if (dicParams.ContainsKey("mo_tw"))
                dicParams["mo_tw"] = convertWinByBet(dicParams["mo_tw"], currentBet);
            if (dicParams.ContainsKey("rs_win"))
                dicParams["rs_win"] = convertWinByBet(dicParams["rs_win"], currentBet);
            if (dicParams.ContainsKey("g"))
            {
                var gParam = JToken.Parse(dicParams["g"]);

                foreach (var wParam in gParam.Children<JProperty>().Select(p => p.Value))
                {
                    if (wParam != null)
                    {
                        if (wParam["mo_tw"] != null)
                            wParam["mo_tw"] = convertWinByBet(wParam["mo_tw"].ToString(), currentBet);
                    }
                }
                dicParams["g"] = serializeJsonSpecial(gParam);
            }
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);

                SpellMasterBetInfo betInfo = new SpellMasterBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();


                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in IceMintsGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine = betInfo.BetPerLine;
                    oldBetInfo.LineCount = betInfo.LineCount;
                    oldBetInfo.MoreBet = betInfo.MoreBet;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in IceMintsGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            SpellMasterBetInfo betInfo = new SpellMasterBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new SpellMasterBetInfo();
        }
        protected override void overrideSomeParams(BasePPSlotBetInfo betInfo, Dictionary<string, string> dicParams)
        {
            base.overrideSomeParams(betInfo, dicParams);
            if (!dicParams.ContainsKey("reel_set"))
                dicParams["reel_set"] = "0";

        }
    }
}
