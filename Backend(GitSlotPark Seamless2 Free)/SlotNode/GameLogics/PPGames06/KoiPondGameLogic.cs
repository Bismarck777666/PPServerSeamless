using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using System.IO;
using Akka.Actor;
using Newtonsoft.Json;
using SlotGamesNode.Database;

namespace SlotGamesNode.GameLogics
{
    public class KoiPondBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get
            {
                return BetPerLine * 88.0f;
            }
        }
    }
    public class KoiPondResult : BasePPSlotSpinResult
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
    public class KoiPondGameLogic : BaseSelFreePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs243koipond";
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
                return 243;
            }
        }
        protected override int ServerResLineCount
        {
            get { return 88; }
        }
        protected override int ROWS
        {
            get
            {
                return 6;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=7,5,4,3,6,5,9,9,4,8,6,6,4,9,6,5,5,9,3,8,7,7,4,4,6,5,6,9,9,8&cfgs=6528&ver=3&def_sb=3,8,4,7,3&reel_set_size=7&def_sa=7,4,6,3,3&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"50000000\",jp1:\"1000\",max_rnd_win:\"2500\",jp3:\"20\",jp2:\"100\",jp4:\"10\"}}&wl_i=tbm~2500&sc=3.00,5.00,7.00,10.00,12.00,25.00,35.00,50.00,60.00,90.00,125.00,200.00,300.00,600.00,850.00,1250.00&defc=12.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;800,200,100,0,0;400,100,50,0,0;200,75,30,0,0;150,50,25,0,0;100,25,15,0,0;20,10,5,0,0;20,10,5,0,0;20,10,5,0,0;15,10,5,0,0;15,10,5,0,0;15,10,5,0,0;4400,880,440,0,0;0,0,0,0,0&reel_set0=9,6,10,6,6,8,4,7,12,4,6,8,7,5,5,6,6,6,7,3,11,5,6,7,5,6,4,7,5,6,4,11,13,7,6,6,4,13,4,4,4,5,13,7,7,7,10,5,5,5,7,14,3,3,3~5,5,5,6,6,6,2,3,7,7,5,5,11,3,3,4,4,4,7,7,7,12,6,12,7,10,9,6,10,12,7,7,11,5,9,6,8,7,7,2,5,5,9,7,12,3,5,13,6,5,7,5,6,7,7,13,14,3,3,3~6,6,6,14,3,4,9,7,2,5,12,14,6,6,5,5,5,12,3,5,5,7,6,9,7,14,6,13,7,8,5,7,7,7,10,8,3,2,6,11,4,4,4,3,3~7,7,7,8,11,2,10,7,7,9,5,5,5,13,6,6,12,8,4,13,2,7,9,5,12,4,4,4,13,14,3,3,3,8,9,5,7,13,4,8,7,7,11,6,13,11,4,13,10,5,13,7,7,12,10,6,6,6~7,7,7,13,8,3,9,12,4,10,9,6,6,6,8,5,5,5,8,7,9,5,11,7,3,11,13,7,3,9,6,9,6,10,8,6,10,4,4,4,13,12,14,3,3,3&reel_set2=7,7,7,13,6,7,3,6,6,6,7,7,6,6,9,4,4,4,5,5,5,10,6,8,4,6,6,11,6,6,5,5,4,7,7,14,12,7,3,5,14,3,7,7,4,5,5,5,7,7,4,5,7,3,3,3~6,6,6,14,6,6,5,13,7,9,6,14,6,6,8,2,9,7,7,8,4,10,7,7,7,10,3,3,4,4,4,13,3,7,4,12,5,7,3,9,5,7,4,10,3,5,6,11,7,5,4,8,7,5,3,12,7,6,12,2,13,5,5,5~3,3,11,7,14,12,9,4,3,13,7,7,7,10,6,6,6,7,7,8,4,4,6,8,2,9,6,6,5,5,5~4,4,4,7,3,6,14,7,4,6,13,7,5,6,11,4,4,6,6,5,5,5,9,7,3,12,4,4,13,7,5,6,6,6,11,4,7,3,8,7,5,3,6,6,6,14,7,5,5,7,7,5,9,2,9,7,7,5,10,2,10,4,3,6,7,7,7,14,5,11,3,3,3~6,6,6,5,5,3,12,5,4,3,8,6,7,3,10,4,7,5,11,3,6,7,13,3,14,5,5,5,6,4,7,13,5,4,3,7,5,10,4,4,6,9,5,3,3,8,6,7,5,5,8,4,6,6,9,7,5,4,14,3,3,3,9,6,4,11,7,7,7&reel_set1=3,3,3,7,7,13,6,7,7,4,4,4,7,6,9,7,7,7,5,8,7,11,7,6,7,12,6,10,7,7,7,6,14,7,7,7,5,6,6~11,7,7,7,4,13,7,6,5,5,5,7,8,3,2,7,7,4,4,4,7,6,9,7,7,8,2,12,7,9,14,7,7,8,3,7,10,10,7,7,14,6,6,12,7,7,3,3~12,7,6,8,4,4,4,7,8,5,5,11,7,7,10,3,2,3,7,7,14,13,3,8,7,7,7,7,8,3,8,7,9~4,4,5,5,11,5,8,6,8,7,10,14,7,6,3,7,10,3,2,8,6,7,9,4,7,10,10,3,7,12,3,12,2,7,3,9,7,9,14,7,7,13~5,5,8,3,3,7,12,4,4,7,11,7,7,6,8,7,13,6,13,5,9,4,9,7,10,10,7,8,4,3,10,4,7,9,5,3,3,5,11,7,6,6,9,7,10,3,7,5,8,7,5,8,5,14,7,7,7&reel_set4=6,6,6,5,11,7,5,6,11,5,6,4,7,7,6,8,8,3,3,3,6,6,3,4,13,7,4,6,10,4,6,11,7,7,7,7,11,7,13,7,7,7,9,4,4,12,5,13,7,7,7,10,5,5,5,9,14~14,9,5,7,6,3,8,9,5,5,6,6,7,5,13,2,8,7,7,3,7,7,4,4,4,7,7,7,8,6,12,7,10,10,6,6,7,12,6,11,5,9,7,6,3,8,2,9,5,5,6,13,7,5,13,6,8,5,7,7,6,6,3,3,3,11,7,7,7,6,6,6,10,2,11,5,5,5~5,5,5,11,4,5,5,12,6,9,7,8,6,13,7,8,4,7,7,7,5,10,11,7,5,5,4,3,3,11,2,10,7,3,8,11,14,6,6,7,5,5,8,2,9,4,4,11,5,6,6,6,13,5,8,4,7,7,7,10,5,5,3,11,5,5,4,4,3,3,3~2,8,6,6,12,12,4,13,13,7,9,5,12,4,4,4,3,3,8,8,3,5,11,11,4,8,8,7,11,6,6,11,4,5,7,7,7,8,11,8,6,6,6,11,4,9,7,7,7,10,14,12,12,6,9,5,5,5~11,11,6,9,9,6,9,5,10,10,7,3,10,10,3,12,12,3,7,3,14,11,3,12,12,6,6,8,3,11,14,3,12,12,7,7,7,13,13,4,4,12,12,4,9,9,6,6,8,8,5,5&reel_set3=14,4,7,6,6,6,5,11,7,5,6,11,5,6,4,7,7,6,8,8,3,3,3,6,6,7,4,3,7,4,6,10,4,6,11,7,13,7,7,7,9,4,4,12,5,6,7,7,7,10,5,5,5~3,3,3,11,7,7,7,6,6,6,10,2,11,5,5,5,12,7,9,7,3,4,4,6,7,7,10,2,12,6,7,6,6,7,6,12,2,9,5,7,6,3,8,2,9,5,5,6,6,7,5,13,2,8,7,7,3,7,7,4,4,4,7,7,7,8,6,12,7,10,10,6,6,7,12,6,6,5,9,7,6,3,8,14~4,4,3,3,3,10,7,8,6,6,7,7,7,3,6,11,2,13,5,7,7,7,11,6,6,6,5,7,12,14,9,6,7,7,6,11,5,5,5~3,3,8,8,3,5,11,11,4,8,8,7,11,6,6,14,4,13,13,5,7,7,7,8,8,6,6,6,11,4,9,7,7,7,10,2,12,12,6,9,5,5,5,13,6,6,6,8,4,13,2,9,7,5,12,4,4,4~13,13,4,4,4,9,9,6,6,8,8,5,5,8,6,9,5,8,8,6,4,4,10,11,11,5,10,10,7,3,10,10,3,12,12,3,7,3,14,11,3,12,12,6,6,8,3,11,14,3,12,12,7,7,7&reel_set6=9,6,10,6,6,8,4,7,12,4,6,8,7,5,5,6,6,6,7,3,11,5,6,7,5,6,4,7,5,6,4,11,13,7,6,6,4,13,4,4,4,5,13,7,7,7,10,5,5,5,7,14,3,3,3~5,5,5,6,6,6,2,3,7,7,5,5,11,3,3,4,4,4,7,7,7,12,6,12,7,10,9,6,10,12,7,7,11,5,9,6,8,7,7,2,5,5,9,7,12,3,5,13,6,5,7,5,6,7,7,13,14,3,3,3~6,6,6,14,3,4,9,7,2,5,12,14,6,6,5,5,5,12,3,5,5,7,6,9,7,14,6,13,7,8,5,7,7,7,10,8,3,2,6,11,4,4,4,3,3~7,7,7,8,11,2,10,7,7,9,5,5,5,13,6,6,12,8,4,13,2,7,9,5,12,4,4,4,13,14,3,3,3,8,9,5,7,13,4,8,7,7,11,6,13,11,4,13,10,5,13,7,7,12,10,6,6,6~7,7,7,13,8,3,9,12,4,10,9,6,6,6,8,5,5,5,8,7,9,5,11,7,3,11,13,7,3,9,6,9,6,10,8,6,10,4,4,4,13,12,14,3,3,3&reel_set5=6,6,6,5,11,7,5,6,11,5,6,4,7,7,6,8,8,3,3,3,6,6,3,4,13,7,4,6,10,4,6,11,7,7,7,7,11,7,13,7,7,7,9,4,4,12,5,13,7,7,7,10,5,5,5,9,14~14,9,5,7,6,3,8,9,5,5,6,6,7,5,13,2,8,7,7,3,7,7,4,4,4,7,7,7,8,6,12,7,10,10,6,6,7,12,6,11,5,9,7,6,3,8,2,9,5,5,6,13,7,5,13,6,8,5,7,7,6,6,3,3,3,11,7,7,7,6,6,6,10,2,11,5,5,5~5,5,5,11,4,5,5,12,6,9,7,8,6,13,7,8,4,7,7,7,5,10,11,7,5,5,4,3,3,11,2,10,7,3,8,11,14,6,6,7,5,5,8,2,9,4,4,11,5,6,6,6,13,5,8,4,7,7,7,10,5,5,3,11,5,5,4,4,3,3,3~2,8,6,6,12,12,4,13,13,7,9,5,12,4,4,4,3,3,8,8,3,5,11,11,4,8,8,7,11,6,6,11,4,5,7,7,7,8,11,8,6,6,6,11,4,9,7,7,7,10,14,12,12,6,9,5,5,5~11,11,6,9,9,6,9,5,10,10,7,3,10,10,3,12,12,3,7,3,14,11,3,12,12,6,6,8,3,11,14,3,12,12,7,7,7,13,13,4,4,12,12,4,9,9,6,6,8,8,5,5";
            }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            return new int[] { 200, 201, 202, 203, 204 };
        }
        protected override int FreeSpinTypeCount
        {
            get
            {
                return 5; //유저가 선택가능한 프리스핀종류수
            }
        }
        #endregion
        public KoiPondGameLogic()
        {
            _gameID = GAMEID.KoiPond;
            GameName = "KoiPond";
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                string strInitString = ChipsetManager.Instance.convertTo(currency, this.InitDataString, this.SymbolName);
                var dicParams = splitResponseToParams(strInitString);

                double minChip = 0.0, maxChip = 0.0;
                getMinMaxChip(dicParams["sc"], ref minChip, ref maxChip);


                KoiPondBetInfo betInfo = new KoiPondBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in KoiPondGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
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

                    oldBetInfo.BetPerLine  = betInfo.BetPerLine;
                    oldBetInfo.LineCount   = betInfo.LineCount;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in KoiPondGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            KoiPondBetInfo betInfo = new KoiPondBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }

        protected override BasePPSlotBetInfo newBetInfo()
        {
            return new KoiPondBetInfo();
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["reel_set"] = "0";
            dicParams["st"] = "rect";
            dicParams["sw"] = "5";
        }

        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("wlc_v"))
            {
                string strWlc_v = dicParams["wlc_v"];
                string[] strParts = strWlc_v.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < strParts.Length; i++)
                {
                    string[] strSubParts = strParts[i].Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                    if (strSubParts.Length >= 2)
                        strSubParts[1] = convertWinByBet(strSubParts[1], currentBet);

                    strParts[i] = string.Join("~", strSubParts);
                }
                dicParams["wlc_v"] = string.Join(";", strParts);
            }
        }

        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            KoiPondResult result = new KoiPondResult();
            result.SerializeFrom(reader);
            return result;
        }
        protected override async Task onProcMessage(string strUserID, int websiteID, GITMessage message, UserBonus userBonus, double userBalance, Currencies currency, bool isAffiliate)
        {
            if (message.MsgCode == (ushort)CSMSG_CODE.CS_PP_DOBONUS)
                await onDoBonus(websiteID, strUserID, message, userBalance, isAffiliate);
            else
                await base.onProcMessage(strUserID, websiteID, message, userBonus, userBalance, currency, isAffiliate);
        }
        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst, PPFreeSpinInfo freeSpinInfo)
        {
            try
            {
                KoiPondResult spinResult = new KoiPondResult();
                Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(dicParams, betInfo.TotalBet);
                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                if (SupportPurchaseFree && betInfo.PurchaseFree && isFirst)
                    dicParams["purtr"] = "1";

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
                _logger.Error("Exception has been occurred in BasePPSlotGame::calculateResult {0}", ex);
                return null;
            }
        }
        protected async Task onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, bool isAffiliate)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                int ind     = (int)message.Pop();

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                double realWin = 0.0;
                string strGameLog = "";
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                ToUserResultMessage resultMsg = null;

                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID))                    
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    KoiPondResult           result          = _dicUserResultInfos[strGlobalUserID] as KoiPondResult;
                    BasePPSlotBetInfo       betInfo         = _dicUserBetInfos[strGlobalUserID];

                    Dictionary<string, string> dicLastResultParams = splitResponseToParams(result.ResultString);
                    if (result.HasBonusResult)
                    {
                        Dictionary<string, string> dicBonusParams = splitResponseToParams(result.BonusResultString);
                        dicLastResultParams = mergeSpinToBonus(dicLastResultParams, dicBonusParams);
                    }
                    if (dicLastResultParams.ContainsKey("trail"))
                    {
                        BasePPSlotStartSpinData startSpinData = betInfo.SpinData as BasePPSlotStartSpinData;
                        if (ind >= startSpinData.FreeSpins.Count)
                        {
                            responseMessage.Append("unlogged");
                        }
                        else
                        {
                            BasePPSlotSpinData freeSpinData = convertBsonToSpinData(startSpinData.FreeSpins[ind]);
                            preprocessSelectedFreeSpin(freeSpinData, betInfo);

                            betInfo.SpinData = freeSpinData;

                            List<string> freeSpinStrings = new List<string>();
                            for (int i = 0; i < freeSpinData.SpinStrings.Count; i++)
                                freeSpinStrings.Add(addStartWinToResponse(freeSpinData.SpinStrings[i], startSpinData.StartOdd));

                            string strSpinResponse = freeSpinStrings[0];
                            if (freeSpinStrings.Count > 1)
                                betInfo.RemainReponses = buildResponseList(freeSpinStrings);

                            double selectedWin = (startSpinData.StartOdd + freeSpinData.SpinOdd) * betInfo.TotalBet;
                            double maxWin      = startSpinData.MaxOdd * betInfo.TotalBet;

                            //시작스핀시에 최대의 오드에 해당한 윈값을 더해주었으므로 그 차분을 보상해준다.
                            if (!startSpinData.IsEvent && !isAffiliate)
                                sumUpWebsiteBetWin(agentID, 0.0, selectedWin - maxWin);
                            else if (maxWin > selectedWin)
                                addEventLeftMoney(agentID, strUserID, maxWin - selectedWin);

                            Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

                            convertWinsByBet(dicParams, betInfo.TotalBet);
                            convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);
                            result.BonusResultString = convertKeyValuesToString(dicParams);
                            addDefaultParams(dicParams, userBalance, index, counter);
                            ActionTypes nextAction = convertStringToActionType(dicParams["na"]);
                            string strResponse = convertKeyValuesToString(dicParams);

                            responseMessage.Append(strResponse);

                            //히스토리보관 및 초기화
                            if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                                addFSOptionActionHistory(strGlobalUserID, ind, strResponse, index, counter);

                            result.NextAction = nextAction;
                        }
                        if (!betInfo.HasRemainResponse)
                            betInfo.RemainReponses = null;

                        saveBetResultInfo(strGlobalUserID);
                    }
                    else
                    {
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
                            Dictionary<string, KoiPondBonusG> dicG = JsonConvert.DeserializeObject<Dictionary<string, KoiPondBonusG>>(dicParams["g"]);
                            KoiPondBonusG bonusParam = dicG["bg_0"];

                            if (bonusParam.status != null)
                            {
                                int[] status = new int[12];
                                for (int i = 0; i < result.BonusSelections.Count; i++)
                                    status[result.BonusSelections[i]] = i + 1;
                                bonusParam.status = string.Join(",", status);
                            }
                            if (bonusParam.ch_h != null && result.BonusSelections.Count > 0)
                            {
                                string[] chs = new string[result.BonusSelections.Count];
                                for (int i = 0; i < result.BonusSelections.Count; i++)
                                    chs[i] = string.Format("0~{0}", result.BonusSelections[i]);
                                bonusParam.ch_h = string.Join(",", chs);
                            }

                            if (bonusParam.wins != null)
                            {
                                string[] strWins = bonusParam.wins.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                string[] strNewWins = new string[12];
                                for (int i = 0; i < 12; i++)
                                    strNewWins[i] = "0";
                                for (int i = 0; i < result.BonusSelections.Count; i++)
                                    strNewWins[result.BonusSelections[i]] = strWins[i];
                                bonusParam.wins = string.Join(",", strNewWins);
                            }
                            if (bonusParam.wi != null)
                                bonusParam.wi = ind.ToString();

                            if (bonusParam.rw != null)
                                bonusParam.rw = convertWinByBet(bonusParam.rw, betInfo.TotalBet);

                            if (bonusParam.wins_mask != null)
                            {
                                string[] strWinsMask = bonusParam.wins_mask.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                string[] strNewWinsMask = new string[12];
                                for (int i = 0; i < 12; i++)
                                    strNewWinsMask[i] = "h";
                                for (int i = 0; i < result.BonusSelections.Count; i++)
                                    strNewWinsMask[result.BonusSelections[i]] = strWinsMask[i];
                                bonusParam.wins_mask = string.Join(",", strNewWinsMask);
                            }

                            dicParams["g"] = JsonConvert.SerializeObject(dicG);
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

                }
                if (resultMsg == null)
                    Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
                else
                    Sender.Tell(resultMsg);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in KoiPondGameLogic::onDoBonus {0}", ex);
            }
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set", "gsf_r", "gsf" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]))
                    continue;
                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }
            if (!resultParams.ContainsKey("s") && spinParams.ContainsKey("s"))
                resultParams["s"] = spinParams["s"];
            return resultParams;
        }

    }
    public class KoiPondBonusG
    {
        public string ack  { get; set; }
        public string bgid { get; set; }
        public string bgt  { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ch_h { get; set; }
        public string ch_k { get; set; }
        public string ch_v { get; set; }
        public string end { get; set; }
        public string level { get; set; }
        public string lifes { get; set; }
        public string rw     { get; set; }
        public string status { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string wi        { get; set; }
        public string wins      { get; set; }
        public string wins_mask { get; set; }

    }
}
