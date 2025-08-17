using Akka.Actor;
using GITProtocol;
using Newtonsoft.Json.Linq;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class JumboSafariBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get
            {
                return BetPerLine * 5;
            }
        }

        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
        }
    }

    public class JumboSafariResult : BasePPSlotSpinResult
    {
        public List<int> BonusSelections { get; set; }

        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);
            BonusSelections = SerializeUtils.readIntList(reader);
        }
        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);
            SerializeUtils.writeIntList(writer, BonusSelections);
        }
    }

    class JumboSafariGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20jjjack";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 10; }
        }
        protected override int ServerResLineCount
        {
            get { return 5; }
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
                return "def_s=5,12,8,5,12,1,14,6,7,11,4,13,2,5,12&cfgs=1&ver=3&def_sb=6,12,12,6,10&reel_set_size=12&def_sa=3,8,6,7,10&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={rtps:{ante:\"96.52\",purchase:\"96.52\",regular:\"96.52\"},props:{max_rnd_sim:\"1\",max_rnd_hr:\"1029336\",max_rnd_win:\"3000\",max_rnd_win_a:\"1500\",max_rnd_hr_a:\"411759\"}}&wl_i=tbm~3000;tbm_a~1500&reel_set10=9,9,10,7,9,9,8,5,6,7,9,9,10,9,9,9,4,10,7,9,4,5,9,9,8,10,8,3,8,9,6~9,4,9,9,7,9,6,9,8,3,8,7,6,9,9,9,5,9,10,9,7,9,9,7,4,6,8,10,8,10,9,5,10~4,9,9,10,7,9,9,10,6,9,6,8,7,9,9,9,8,9,5,9,10,9,4,6,9,9,10,7,8,3,9,5,8~6,8,4,9,9,8,10,7,9,7,8,7,8,9,9,9,6,10,9,3,5,10,9,6,5,9,7,4,9,10,9,9~8,5,6,8,9,9,7,9,4,5,6,10,9,8,9,9,9,7,9,8,7,9,4,9,9,10,7,6,9,3,10,9,9&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&reel_set11=7,10,3,8,6,10,9,8,5,10,10,10,9,5,10,4,7,10,8,6,10,4,9,7~8,7,10,9,6,10,6,7,9,3,10,10,10,5,8,10,7,4,10,8,10,10,6,10,4,5,10~7,10,9,10,10,6,10,5,8,10,10,10,9,7,8,6,5,7,10,3,10,4,10,4,9~5,10,10,5,10,10,6,9,10,10,6,8,3,8,10,10,10,4,7,10,9,7,10,9,8,9,6,8,10,7,4,10,10~9,8,4,7,10,5,8,3,10,5,10,10,10,7,10,10,9,8,10,10,6,4,7,10,6,9&purInit_e=1&wilds=2~2500,500,100,0,0~1,1,1,1,1&bonuses=0&bls=5,10&ntp=0.00&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;2500,500,100,0,0;250,100,50,0,0;100,50,20,0,0;75,20,10,0,0;50,10,5,0,0;50,10,5,0,0;50,10,5,0,0;50,10,5,0,0;20,2,1,0,0;20,2,1,0,0;20,2,1,0,0;20,2,1,0,0&total_bet_max=2,500,000.00&reel_set0=11,10,13,12,5,9,12,6,11,13,7,14,13,9,12,1,12,5,13,13,13,11,8,6,7,13,4,2,10,14,11,7,10,9,10,8,14,1,3,14,9,8~7,5,9,4,9,14,11,11,11,10,3,10,11,12,6,12,12,12,13,8,12,9,6,1,10,11,13,13,13,2,14,9,11,11,8,13,14,14,14,5,7,14,13,13,14,10,8,12~14,12,9,14,10,8,5,11,11,11,8,13,10,7,11,12,8,4,13,12,12,12,7,6,13,12,3,11,13,1,12,13,13,13,14,10,2,9,9,13,6,11,14,14,14,9,14,11,5,14,9,7,10,11,12~13,4,11,6,9,11,9,7,11,11,11,9,7,14,12,12,10,8,12,13,12,12,12,13,12,8,14,14,7,1,12,10,13,13,13,11,13,10,14,6,8,5,9,14,14,14,11,3,14,9,2,10,8,5,11~2,12,4,11,13,9,11,11,9,5,10,12,10,13,8,11,14,1,13,13,13,8,9,13,5,14,12,6,3,12,7,13,7,10,14,8,9,6,8,1,7,14&reel_set2=1,8,10,12,11,14,10,14,13,11,10,12,9,5,13,11,1,9,8,3,12,13,13,13,2,8,12,14,6,11,14,12,13,8,13,7,9,13,14,9,7,6,11,5,4,10,7,10~12,8,14,13,7,14,10,11,11,11,7,13,9,11,6,9,13,12,5,12,12,12,9,10,13,8,14,13,7,10,1,13,13,13,12,4,12,11,2,12,14,8,5,14,14,14,6,8,9,1,9,11,10,14,10,3,11~9,7,5,13,7,11,11,11,8,9,12,13,6,12,12,12,14,14,10,3,10,9,13,13,13,6,14,12,11,8,13,14,14,14,12,5,10,1,2,11,8,4~10,13,13,4,6,12,11,11,11,10,9,11,14,14,12,1,9,12,12,12,13,8,10,6,8,8,13,13,13,11,11,5,11,10,7,1,7,14,14,14,2,14,9,3,9,14,5,13,12~8,14,13,12,14,7,9,11,5,6,11,7,12,13,13,13,9,10,4,8,13,1,14,2,3,9,10,13,10,11,12&reel_set1=12,6,13,12,14,5,9,2,12,10,8,7,13,10,9,1,14,3,13,13,13,8,14,9,4,14,6,12,11,1,10,7,9,13,13,8,11,5,11,11~12,6,12,14,12,13,11,10,11,11,11,13,9,3,14,11,13,8,5,13,12,12,12,14,8,9,7,10,6,14,7,13,13,13,5,13,9,8,4,12,9,10,9,14,14,14,8,2,7,10,11,1,11,11,10,14~8,10,11,3,10,11,11,11,14,2,14,11,11,4,12,12,12,9,10,8,8,12,5,13,13,13,1,12,9,6,9,14,14,14,7,5,1,7,13,14,12,13~12,1,4,14,11,11,11,7,14,10,3,6,10,12,12,12,2,9,11,11,5,13,13,13,8,13,13,9,12,9,14,14,14,12,14,7,11,8,10,13~14,10,6,14,7,12,2,11,14,13,14,6,9,10,12,7,13,5,13,13,13,12,5,3,11,9,1,8,9,10,8,13,12,11,4,13,10,8,1,11,9&reel_set4=4,5,10,9,10,5,7,9,6,3,3,3,9,8,4,7,8,10,7,6,8~9,8,6,3,4,6,7,3,7,5,8,9,3,3,3,10,8,3,10,9,7,4,6,9,10,5,7~6,3,10,8,10,7,8,5,3,3,3,7,9,10,9,6,3,8,7,4,9~6,5,8,3,3,8,10,5,7,3,3,3,4,8,9,7,9,7,10,6,4~8,7,9,6,5,7,10,8,6,3,3,3,5,4,3,8,9,7,10,9,4,3,10&purInit=[{bet:500}]&reel_set3=5,14,14,10,13,11,13,6,1,11,14,12,8,13,13,13,7,10,9,4,12,8,12,7,2,9,3,10,9,11,13~9,1,3,10,14,11,11,11,8,14,12,4,6,5,12,12,12,14,11,7,6,8,11,1,13,13,13,10,9,11,2,13,10,14,14,14,9,13,7,12,13,12,8,5~9,1,10,14,12,3,8,11,11,11,8,10,2,5,10,9,13,12,12,12,14,1,11,12,13,1,14,7,13,13,13,12,6,14,8,11,7,13,5,14,14,14,12,4,9,6,11,9,10,13,7~8,11,9,5,13,11,11,11,9,11,11,10,7,10,12,12,12,3,7,5,2,1,14,8,13,13,13,6,10,13,14,12,14,14,14,12,9,12,13,4,8,1,6~8,14,9,7,10,7,5,4,14,12,5,13,6,11,1,13,13,13,10,6,9,3,1,11,10,13,12,14,2,13,11,12,8,9,8&reel_set6=6,5,8,3,9,6,4,5,5,10,4,5,5,5,6,10,5,7,9,10,7,5,8,7,9,7,9,8~8,5,9,7,9,8,10,4,7,5,5,5,8,5,4,5,6,3,5,7,9,10,6,10~7,6,5,9,8,10,7,4,5,7,5,10,5,5,5,6,8,5,8,4,5,10,9,3,8,10,7,6,9,5~7,4,9,6,10,8,10,5,5,5,7,8,5,9,10,5,3,5,5,7,4~6,9,10,8,6,4,8,4,8,5,5,5,9,5,3,7,10,5,7,9&reel_set5=9,4,8,6,5,7,6,8,4,5,9,4,4,4,10,3,7,4,7,10,4,8,9,4,8,4,9~5,8,10,8,7,4,4,9,8,4,4,4,6,9,7,6,10,7,4,3,10,5,9,4~4,8,10,4,4,8,9,7,4,4,4,8,5,3,9,6,4,7,6,10,9~9,4,3,5,9,4,7,5,8,4,4,4,6,10,8,6,4,7,10,9,7~10,9,5,8,3,9,7,8,4,4,4,7,4,4,6,8,5,10,6,4,10,7,9&reel_set8=7,4,5,10,7,7,5,10,8,7,6,7,7,7,3,7,9,8,7,8,10,7,4,6,7,9~9,8,7,3,7,10,9,7,7,9,6,7,10,8,7,7,7,6,7,4,7,8,7,5,7,4,7,10,5,10,8,6,9,7~8,5,3,8,4,9,4,7,8,7,7,7,10,9,6,7,7,9,7,6,10,7,7,5,10~8,7,7,6,7,10,3,8,9,5,7,7,4,7,7,7,9,7,6,7,4,6,10,5,8,10,7,9,7,10,9,7~4,7,7,9,6,5,9,8,3,7,10,9,7,7,7,6,5,6,10,8,7,7,4,7,7,8&reel_set7=6,10,8,3,4,7,6,4,6,8,6,6,6,7,10,9,6,9,6,6,9,8,7,5~4,9,10,6,7,6,7,6,4,7,8,5,6,6,6,3,9,6,10,6,10,9,5,8,6,8,10,8,9,6~6,9,7,9,7,4,8,6,7,3,6,6,6,9,8,10,6,10,5,10,6,8,6,4,5,6~6,8,6,9,6,6,8,10,4,9,6,9,8,6,6,6,10,5,7,3,5,4,6,6,9,7,6,8,7,6,10~3,6,8,6,5,8,10,9,10,7,6,6,6,7,6,8,6,4,5,9,6,4,10,9,7,6&reel_set9=9,7,4,3,8,10,8,8,5,8,8,8,6,10,8,5,7,9,6,9,7,8,10,8~4,10,8,7,5,7,9,8,6,8,6,9,8,7,8,8,8,4,8,8,9,8,7,10,5,9,10,8,8,10,3,6,8~9,7,8,7,10,4,5,8,3,5,8,8,8,6,8,10,9,4,7,8,8,6,8,9,8,10~10,5,8,4,6,8,5,8,4,6,7,8,8,8,9,8,7,8,7,9,10,8,9,8,10,3~8,8,6,7,8,8,9,8,8,8,7,4,8,5,8,6,10,3,10,9&total_bet_min=50.00";
            }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 100; }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override bool HasPurEnableOption
        {
            get { return true; }
        }
        protected override double MoreBetMultiple
        {
            get { return 2; }
        }
        protected override bool SupportMoreBet
        {
            get { return true; }
        }
        #endregion
        
        public JumboSafariGameLogic()
        {
            _gameID     = GAMEID.JumboSafari;
            GameName    = "JumboSafari";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
	        dicParams["bl"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);

            if (dicParams.ContainsKey("pw"))
                dicParams["pw"] = convertWinByBet(dicParams["pw"].ToString(), currentBet);

            if (dicParams.ContainsKey("g"))
            {
                var gParam = JToken.Parse(dicParams["g"]);
                if (gParam["bg_0"] != null)
                {
                    if (gParam["bg_0"]["rw"] != null)
                        gParam["bg_0"]["rw"] = convertWinByBet(gParam["bg_0"]["rw"].ToString(), currentBet);
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


                JumboSafariBetInfo betInfo = new JumboSafariBetInfo();
                betInfo.BetPerLine  = (float)message.Pop();
                betInfo.LineCount   = (int)message.Pop();

                int bl = (int)message.Pop();
                if (bl == 0)
                    betInfo.MoreBet = false;
                else
                    betInfo.MoreBet = true;

                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;
		
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in JumboSafariGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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

                    oldBetInfo.BetPerLine   = betInfo.BetPerLine;
                    oldBetInfo.LineCount    = betInfo.LineCount;
                    oldBetInfo.MoreBet      = betInfo.MoreBet;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in JumboSafariGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strGlobalUserID, BinaryReader reader)
        {
            JumboSafariBetInfo betInfo = new JumboSafariBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new JumboSafariBetInfo();
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "s", "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set", "bw" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]))
                    continue;
                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }

            if (!resultParams.ContainsKey("g") && spinParams.ContainsKey("g"))
                resultParams["g"] = spinParams["g"];
            return resultParams;
        }
        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst, PPFreeSpinInfo freeSpinInfo)
        {
            try
            {
                JumboSafariResult spinResult = new JumboSafariResult();
                Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(dicParams, betInfo.TotalBet);

                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                spinResult.NextAction = convertStringToActionType(dicParams["na"]);
                if (spinResult.NextAction == ActionTypes.DOCOLLECT || spinResult.NextAction == ActionTypes.DOCOLLECTBONUS)
                    spinResult.TotalWin = double.Parse(dicParams["tw"]);
                else
                    spinResult.TotalWin = 0.0;

                if (freeSpinInfo != null)
                {

                    dicParams["fra"] = Math.Round(freeSpinInfo.TotalWin, 2).ToString();
                    dicParams["frn"] = freeSpinInfo.RemainCount.ToString();
                    dicParams["frt"] = "N";
                }
                spinResult.ResultString = convertKeyValuesToString(dicParams);

                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in JumboSafariGameLogic::calculateResult {0}", ex);
                return null;
            }
        }
        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            JumboSafariResult result = new JumboSafariResult();
            result.SerializeFrom(reader);
            return result;
        }
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency, bool isAffiliate)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                int ind     = (int)message.Pop();

                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                double realWin      = 0.0;
                string strGameLog   = "";
                ToUserResultMessage resultMsg = null;

                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) ||
                    !_dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    JumboSafariResult result    = _dicUserResultInfos[strGlobalUserID] as JumboSafariResult;
                    BasePPSlotBetInfo betInfo   = _dicUserBetInfos[strGlobalUserID];
                    BasePPActionToResponse actionResponse = betInfo.pullRemainResponse();
                    if (actionResponse.ActionType != ActionTypes.DOBONUS)
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        Dictionary<string, string> dicParams = splitResponseToParams(actionResponse.Response);

                        convertWinsByBet(dicParams, betInfo.TotalBet);
                        convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                        if (result.BonusSelections == null)
                            result.BonusSelections = new List<int>();

                        if (result.BonusSelections.Contains(ind))
                        {
                            betInfo.pushFrontResponse(actionResponse);
                            saveBetResultInfo(strGlobalUserID);
                            throw new Exception(string.Format("{0} User selected already selected position, Malicious Behavior {1}", strGlobalUserID, ind));
                        }

                        result.BonusSelections.Add(ind);

                        if (dicParams.ContainsKey("g"))
                        {
                            var gParam = JToken.Parse(dicParams["g"]);
                            if (gParam["bg_0"] != null)
                            {
                                if (gParam["bg_0"]["status"] != null)
                                {
                                    int[] status = new int[8];
                                    for (int i = 0; i < result.BonusSelections.Count; i++)
                                        status[result.BonusSelections[i]] = i + 1;
                                    gParam["bg_0"]["status"] = string.Join(",", status);
                                }

                                if (gParam["bg_0"]["ch_h"] != null)
                                {
                                    string strChh = string.Format("0~{0}", result.BonusSelections[0]);
                                    gParam["bg_0"]["ch_h"] = strChh;
                                }

                                if (gParam["bg_0"]["wins_mask"] != null)
                                {
                                    string[] strWinsMask    = gParam["bg_0"]["wins_mask"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    string[] strNewWinsMask = new string[8];

                                    for (int i = 0; i < 8; i++)
                                        strNewWinsMask[i] = strWinsMask[i];

                                    for (int i = 0; i < result.BonusSelections.Count; i++)
                                    {
                                        string strBuf = strNewWinsMask[result.BonusSelections[i]];
                                        strNewWinsMask[result.BonusSelections[i]] = strWinsMask[i];
                                        strNewWinsMask[0] = strBuf;
                                    }

                                    gParam["bg_0"]["wins_mask"] = string.Join(",", strNewWinsMask);
                                }

                                if (gParam["bg_0"]["wins"] != null)
                                {
                                    string[] strWins    = gParam["bg_0"]["wins"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    string[] strNewWins = new string[8];

                                    for (int i = 0; i < 8; i++)
                                        strNewWins[i] = strWins[i];

                                    for (int i = 0; i < result.BonusSelections.Count; i++)
                                    {
                                        string strBuf = strNewWins[result.BonusSelections[i]];
                                        strNewWins[result.BonusSelections[i]] = strWins[i];
                                        strNewWins[0] = strBuf;
                                    }

                                    gParam["bg_0"]["wins"] = string.Join(",", strNewWins);
                                }
                            }
                            dicParams["g"] = serializeJsonSpecial(gParam);
                        }

                        result.BonusResultString = convertKeyValuesToString(dicParams);
                        addDefaultParams(dicParams, userBalance, index, counter);

                        ActionTypes nextAction = convertStringToActionType(dicParams["na"]);
                        string strResponse = convertKeyValuesToString(dicParams);

                        responseMessage.Append(strResponse);

                        //히스토리보관 및 초기화
                        if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                            addActionHistory(strGlobalUserID, "doBonus", strResponse, index, counter);

                        if (nextAction == ActionTypes.DOCOLLECT || nextAction == ActionTypes.DOCOLLECTBONUS)
                        {
                            realWin = double.Parse(dicParams["tw"]);
                            strGameLog = strResponse;

                            if (realWin > 0.0f)
                            {
                                _dicUserHistory[strGlobalUserID].baseBet = betInfo.TotalBet;
                                _dicUserHistory[strGlobalUserID].win = realWin;
                            }
                            resultMsg = new ToUserResultMessage((int)_gameID, responseMessage, 0.0, realWin, new GameLogInfo(this.GameName, "0", strGameLog), UserBetTypes.Normal); ;
                            resultMsg.BetTransactionID = betInfo.BetTransactionID;
                            resultMsg.RoundID = betInfo.RoundID;
                            resultMsg.TransactionID = createTransactionID();
                            if (_dicUserFreeSpinInfos.ContainsKey(strGlobalUserID) && !_dicUserFreeSpinInfos[strGlobalUserID].Pending)
                            {
                                resultMsg.FreeSpinID = _dicUserFreeSpinInfos[strGlobalUserID].FreeSpinID;
                                addFreeSpinBonusParams(responseMessage, _dicUserFreeSpinInfos[strGlobalUserID], strGlobalUserID, realWin);
                            }
                        }
                        copyBonusParamsToResult(dicParams, result);
                        result.NextAction = nextAction;
                    }
                    if (!betInfo.HasRemainResponse)
                        betInfo.RemainReponses = null;

                    saveBetResultInfo(strGlobalUserID);
                }
                if (resultMsg == null)
                    Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
                else
                    Sender.Tell(resultMsg);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in JumboSafariGameLogic::onDoBonus {0}", ex);
            }
        }
    }
}
