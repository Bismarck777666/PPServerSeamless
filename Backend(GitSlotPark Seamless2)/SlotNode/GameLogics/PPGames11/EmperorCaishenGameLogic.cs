using Akka.Actor;
using GITProtocol;
using PCGSharp;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class EmperorCaishenBetInfo : BasePPSlotBetInfo
    {
        public override float TotalBet
        {
            get { return this.BetPerLine * 88.0f; }
        }
    }
    class EmperorCaishenResult : BasePPSlotSpinResult
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
    class EmperorCaishenGameLogic : BaseSelFreePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs243empcaishen";
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
            get { return 243; }
        }
        protected override int ServerResLineCount
        {
            get { return 88; }
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
                return "def_s=7,5,4,3,6,7,9,3,3,8,6,6,4,9,8&cfgs=5100&ver=2&def_sb=3,8,4,7,3&reel_set_size=8&def_sa=7,4,6,3,3&bonusInit=[{bgid:0,bgt:18,bg_i:\"1000,100,20,10\",bg_i_mask:\"pw,pw,pw,pw\"}]&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&gameInfo={props:{max_rnd_hr_op5:\"50000000\",max_rnd_sim:\"1\",max_rnd_hr_op4:\"50000000\",max_rnd_hr_op1:\"520833\",max_rnd_hr_op3:\"50000000\",fs_options:\"0:S1_88;1:S2_98;2:S3_108;3:S4_118;4:S5_128\",max_rnd_win:\"2500\",max_rnd_hr_op2:\"485436\"}}&wl_i=tbm~2500&sc=3.00,5.00,7.00,10.00,12.00,25.00,35.00,50.00,60.00,90.00,125.00,200.00,300.00,600.00,850.00,1250.00&defc=12.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;750,150,100,0,0;450,100,50,0,0;300,80,40,0,0;200,50,20,0,0;138,25,10,0,0;30,10,5,0,0;30,10,5,0,0;25,10,5,0,0;25,10,5,0,0;20,10,5,0,0;20,10,5,0,0;4400,880,440,0,0&reel_set0=5,11,13,14,6,7,7,6,7,6,7,8,9,5,12,4,10,5,7,6,8,5,13,4,6,6,7,9,3,5,5,7,12,4,4,3,7,4,10,11,6,3,14,4~3,6,5,7,7,11,10,7,2,8,5,6,3,5,5,7,6,9,6,13,5,7,5,6,13,12,4,12,10,9,3,14,4,6,6,14,7,9,2,7,5,7,3,4,5,3,6,8,11,8~13,5,14,5,6,5,6,3,5,4,10,12,7,6,6,7,7,2,3,14,8,4,12,4,6,3,5,13,10,7,5,9,6,11,5,4,9,7,12,4,8,7,6,14,7,11~12,13,14,4,7,5,3,7,5,8,13,11,8,4,13,4,7,13,5,3,7,7,12,6,6,5,11,9,3,12,2,6,8,10,9,2,4,14,6~11,7,3,6,13,8,10,5,12,8,3,9,6,5,4,10,3,13,5,6,14,7,11,8,9,13,12,14,11,12,3,8,4,7,4,6,10,7,13,6,9,4&reel_set2=4,13,4,12,11,11,11,4,14,4,9,9,12,12,12,8,11,4,4,13,4,4,4,9,4,4,11,4,4,10,10,10,8,9,10,11,4,9,9,9,4,4,10,4,13,13,13,8,12,4,10,14,12,13~13,4,9,4,4,4,11,8,8,13,11,8,8,8,10,4,13,9,4,10,11,11,11,8,4,11,4,13,9,9,9,4,2,12,4,12,13,13,13,14,12,4,4,10,11,9~13,4,2,9,4,11,12,11,11,11,14,4,11,4,12,4,10,9,12,8,8,8,4,4,8,13,14,4,12,4,12,12,12,9,13,10,4,4,8,4,12,8,4,4,4,11,4,4,11,10,4,4,8,10,10,13,12,9,8,2,4,4,10,10,10,14,11~4,4,8,9,2,4,13,12,13,8,8,8,2,13,12,4,11,11,13,9,8,11,4,4,4,8,4,8,13,14,9,4,4,10,8,11,11,11,12,11,13,4,14,4,10,10,4,4,9~8,13,8,10,10,11,4,8,8,8,10,11,4,4,14,4,4,9,9,10,10,10,12,10,13,8,4,13,4,4,11,4,4,4,10,11,9,8,9,12,4,9,12,12,12,11,12,4,8,10,11,14,4,13,11,11,11,13,12,11,4,12,4,4,9,12,9,9,9,10,4,11,11,9,4,8,10,8,12&t=243&reel_set1=10,12,12,3,11,11,11,3,13,9,10,3,10,12,12,12,10,3,3,11,8,13,3,3,3,13,12,3,14,9,10,10,10,8,13,12,9,9,13,9,9,9,11,3,9,3,14,3,13,13,13,3,12,3,11,8,3,8,8,8,3,11,11,3,3,8,9~3,3,8,3,3,13,3,3,3,13,9,8,3,12,11,3,8,8,8,9,3,13,12,3,11,10,3,11,11,11,10,3,13,2,14,11,13,9,9,9,8,9,3,10,3,13,3,13,13,13,8,10,11,12,13,14,3,2,11,12~9,9,11,3,11,8,11,11,11,14,2,3,3,12,13,10,12,8,8,8,3,13,8,3,3,8,12,10,9,9,9,3,10,10,10,3,8,11,13,12,12,12,10,12,11,11,3,13,14,3,3,3,8,3,3,8,3,3,2,3,10,10,3,9,12,12,3,14,9,3,8~14,12,8,8,3,8,8,8,10,9,8,3,9,8,13,9,9,9,13,3,13,9,11,11,2,3,3,3,14,3,8,12,10,10,11,11,11,13,3,3,12,3,3,13,13,11,3,2,13,13,13,9,3,13,9~9,11,10,8,3,3,8,8,8,13,11,11,3,3,12,14,8,3,3,3,13,3,8,3,11,8,8,12,12,12,9,3,9,13,10,13,11,11,11,9,10,8,10,3,12,11,10,10,10,9,3,3,10,11,12,10,3,9,9,9,3,12,11,9,14,12,13,10,13,13,13,13,9,11,3,8,3,8,13,11,13&reel_set4=10,8,6,6,9,6,6,6,9,6,11,6,12,8,12,11,13,10,10,10,6,11,6,9,6,6,10,6,6,13,13,13,8,12,13,6,10,6,9,14,6,13~13,13,10,6,11,6,6,9,6,6,6,10,6,2,8,14,6,6,11,13,6,8,11,11,11,6,13,12,10,6,12,8,14,13,6,6,13,13,13,2,12,6,11,6,6,11,6,6,9,6,9~8,12,6,9,6,10,6,14,11,11,11,6,6,8,12,11,8,6,12,6,6,6,13,12,13,6,10,13,6,6,11,12,12,12,6,14,6,11,2,10,6,9,9,6~13,6,10,13,6,6,13,12,6,14,9,11,12,6,6,6,8,6,9,6,10,11,9,2,9,6,11,10,6,6,14,11,11,11,12,6,6,8,6,11,6,8,9,6,13,13,8,10,8,2~11,6,12,6,9,12,13,8,6,13,8,8,8,6,11,14,6,12,6,10,9,6,6,13,8,6,6,6,12,10,6,8,6,13,10,6,11,6,9,10,10,10,6,8,6,12,6,6,11,8,6,13,11,6,9,9,9,14,6,6,12,13,8,6,13,10,10,6,13,13,13,13,9,10,11,10,6,9,8,13,9,11,10,9,11&reel_set3=5,10,5,5,14,9,13,12,11,11,11,5,5,12,11,5,10,5,5,12,12,12,5,13,5,5,12,5,11,5,11,5,5,5,11,14,9,5,9,9,5,8,9,10,10,10,5,8,9,5,10,5,8,8,5,13,13,13,5,10,13,13,5,5,11,10,13,12,5~12,5,5,10,5,9,8,5,5,5,2,5,5,9,14,5,5,11,11,11,9,2,13,8,5,8,13,10,9,9,9,5,13,14,5,12,5,5,13,13,13,5,11,11,10,5,13,12,5~12,10,10,10,5,5,12,12,11,11,11,5,5,8,13,9,5,13,11,5,5,5,10,5,12,13,5,14,5,9,12,12,12,5,2,9,5,8,11,11,5,10,10,5,8,12,2,5,5,14,5,10~11,5,5,8,13,5,10,5,8,5,14,5,9,5,8,5,5,5,13,10,5,9,9,5,11,12,9,11,10,13,11,2,12,2,11,11,11,8,9,5,12,13,12,5,13,8,11,14,5,5,13,5,5,10,13~10,11,5,13,8,8,8,9,10,12,11,13,11,5,5,5,9,11,8,12,13,14,11,11,11,5,9,5,5,10,10,10,9,13,12,8,13,11,9,9,9,8,5,5,10,5,5,13,13,13,13,10,5,5,10,5,8,5&reel_set6=12,10,3,3,11,11,11,12,3,9,3,3,11,12,12,12,3,3,11,8,3,3,3,9,8,12,11,13,12,10,10,10,3,9,13,3,10,9,9,9,3,13,12,8,14,13,13,13,11,8,3,9,11,13,8,8,8,10,3,13,3,3,14,10~3,8,11,11,3,3,3,8,13,10,12,8,8,8,2,8,14,10,11,13,11,11,11,3,12,13,3,3,9,9,9,12,9,3,3,9,13,13,13,3,13,10,3,9,3~3,14,3,8,11,11,11,3,12,12,3,8,12,8,8,8,9,11,9,3,13,9,9,9,3,8,12,3,3,2,12,12,12,8,14,11,3,10,12,3,3,3,11,8,9,3,3,2,10,10,11,3,10,10,10,13,13~8,11,3,13,8,3,10,8,8,8,3,13,10,12,10,11,14,3,8,9,9,9,12,9,3,3,14,3,8,12,3,3,3,8,13,13,9,10,2,8,3,9,11,11,11,2,3,13,3,9,11,13,11,3,11~11,13,3,9,9,8,3,8,8,8,12,8,12,9,11,13,9,12,3,3,3,9,8,10,11,3,3,8,9,12,12,12,11,13,11,12,9,8,3,3,10,11,11,11,10,12,13,3,3,8,11,10,10,10,8,12,11,3,13,13,3,3,11,9,9,9,11,3,10,3,8,14,13,13,13,13,10,10,3,13,10,12,3,12,14,8&reel_set5=7,13,7,13,7,8,7,9,13,11,7,13,11,9,12,10,7,9,7,7,10,9,7,12,7,7,7,11,7,10,11,7,7,10,8,7,8,7,14,7,9,7,12,7,7,12,7,7,14,7,7,8,7,9,7~13,7,7,11,7,7,13,11,7,7,9,7,7,2,12,8,9,7,12,7,14,11,11,7,7,2,7,7,7,13,9,7,13,7,7,13,12,7,10,7,7,10,7,7,8,14,8,12,7,7,8,7,13,7,7,10,9,10,7~7,7,9,7,10,8,7,11,7,14,11,7,14,8,7,12,10,9,8,7,7,2,12,7,2,9,11,7,7,7,14,7,9,7,7,13,7,13,7,7,8,7,10,7,12,11,7,7,13,7,7,8,12,7,12,7,13,7,7,10~8,10,9,11,14,9,7,11,7,7,12,13,7,9,8,7,12,7,7,12,13,13,7,11,10,10,7,7,7,13,10,7,8,9,8,7,7,13,8,11,9,13,7,9,7,7,8,7,7,2,14,2,10,13,12,7,7~8,7,9,7,10,10,7,7,10,12,13,7,7,7,12,13,7,11,7,11,9,13,12,10,7,7,8,10,10,10,13,7,14,7,10,10,7,9,7,7,11,9,9,9,7,11,7,11,12,13,9,7,13,11,7,12,7,13,13,13,13,8,9,8,7,8,7,7,9,10,7,11,9,7,13&reel_set7=13,9,4,9,4,10,11,11,11,10,4,4,8,13,10,13,11,12,12,12,4,4,12,14,9,12,4,9,4,4,4,9,4,4,11,9,12,13,4,10,10,10,4,10,4,12,8,8,4,4,9,9,9,10,13,12,4,4,14,4,9,13,13,13,4,11,4,4,11,8,4,11,4~9,4,13,4,4,4,13,4,9,13,4,4,8,8,8,14,13,11,8,4,9,11,11,11,4,12,4,13,11,9,9,9,10,12,4,8,11,8,13,13,13,11,10,2,12,4,4,10~4,8,4,4,12,11,11,11,4,10,13,4,8,4,10,10,10,8,8,8,4,9,11,4,13,4,12,12,12,9,11,12,12,14,8,4,4,4,2,12,9,2,12,13,10,10,4,4,14,11,11,8,4,10~14,4,4,11,10,8,4,4,13,13,4,8,8,8,4,9,12,4,14,8,10,10,4,9,13,4,4,4,13,9,9,2,11,8,4,11,11,4,4,12,11,11,11,8,12,4,10,13,4,13,8,8,12,4,9,11,2~12,4,9,13,8,11,10,8,8,8,11,4,4,11,10,8,10,12,10,10,10,13,12,8,4,13,10,4,12,4,4,4,9,4,13,8,8,4,4,12,12,12,4,11,4,10,9,8,4,11,11,11,10,4,11,14,10,9,9,13,9,9,9,4,4,14,11,8,4,13,12,9,9";
            }
        }
        protected override int FreeSpinTypeCount
        {
            get { return 5; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            return new int[] { 200, 201, 202, 203, 204 };
        }
        #endregion
        public EmperorCaishenGameLogic()
        {
            _gameID  = GAMEID.EmperorCaishen;
            GameName = "EmperorCaishen";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
        }
        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst)
        {
            try
            {
                EmperorCaishenResult spinResult = new EmperorCaishenResult();
                Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(dicParams, betInfo.TotalBet);

                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                if (SupportPurchaseFree && betInfo.PurchaseFree && isFirst)
                    dicParams["purtr"] = "1";

                spinResult.NextAction = convertStringToActionType(dicParams["na"]);
                spinResult.ResultString = convertKeyValuesToString(dicParams);
                if (spinResult.NextAction == ActionTypes.DOCOLLECT || spinResult.NextAction == ActionTypes.DOCOLLECTBONUS)
                    spinResult.TotalWin = double.Parse(dicParams["tw"]);
                else
                    spinResult.TotalWin = 0.0;

                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in EmperorCaishenGameLogic::calculateResult {0}", ex);
                return null;
            }
        }
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                int index   = (int)message.Pop();
                int counter = (int)message.Pop();
                int ind     = (int)message.Pop();

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_PP_DOBONUS);
                double realWin      = 0.0;
                string strGameLog   = "";
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                ToUserResultMessage resultMsg = null;

                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID))
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    BasePPSlotBetInfo       betInfo = _dicUserBetInfos[strGlobalUserID];
                    EmperorCaishenResult    result  = _dicUserResultInfos[strGlobalUserID] as EmperorCaishenResult;
                    if (result.NextAction != ActionTypes.DOBONUS)
                    {
                        responseMessage.Append("unlogged");
                    }
                    else
                    {
                        if (betInfo.HasRemainResponse)
                        {
                            BasePPActionToResponse actionResponse = betInfo.pullRemainResponse();
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
                            if (dicParams.ContainsKey("status"))
                            {
                                int[] status = new int[12];
                                for (int i = 0; i < result.BonusSelections.Count; i++)
                                    status[result.BonusSelections[i]] = i + 1;
                                dicParams["status"] = string.Join(",", status);
                            }
                            if (dicParams.ContainsKey("wins"))
                            {
                                string[] strWins = dicParams["wins"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                string[] strNewWins = new string[12];
                                for (int i = 0; i < 12; i++)
                                    strNewWins[i] = "0";
                                for (int i = 0; i < result.BonusSelections.Count; i++)
                                    strNewWins[result.BonusSelections[i]] = strWins[i];
                                dicParams["wins"] = string.Join(",", strNewWins);
                            }
                            if (dicParams.ContainsKey("wins_mask"))
                            {
                                string[] strWinsMask = dicParams["wins_mask"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                string[] strNewWinsMask = new string[12];
                                for (int i = 0; i < 12; i++)
                                    strNewWinsMask[i] = "h";
                                for (int i = 0; i < result.BonusSelections.Count; i++)
                                    strNewWinsMask[result.BonusSelections[i]] = strWinsMask[i];
                                dicParams["wins_mask"] = string.Join(",", strNewWinsMask);
                            }

                            result.BonusResultString = convertKeyValuesToString(dicParams);
                            addDefaultParams(dicParams, userBalance, index, counter);
                            ActionTypes nextAction  = convertStringToActionType(dicParams["na"]);
                            string strResponse      = convertKeyValuesToString(dicParams);

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
                            }
                            copyBonusParamsToResult(dicParams, result);
                            result.NextAction = nextAction;
                        }
                        else
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
                                double maxWin = startSpinData.MaxOdd * betInfo.TotalBet;

                                //시작스핀시에 최대의 오드에 해당한 윈값을 더해주었으므로 그 차분을 보상해준다.
                                if (!startSpinData.IsEvent)
                                    sumUpWebsiteBetWin(agentID, 0.0, selectedWin - maxWin);
                                else if (maxWin > selectedWin)
                                    addEventLeftMoney(agentID, strUserID, maxWin - selectedWin);

                                Dictionary<string, string> dicParams = splitResponseToParams(strSpinResponse);

                                convertWinsByBet(dicParams, betInfo.TotalBet);
                                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);
                                if (SupportMoreBet)
                                {
                                    if (betInfo.MoreBet)
                                        dicParams["bl"] = "1";
                                    else
                                        dicParams["bl"] = "0";
                                }
                                result.BonusResultString = convertKeyValuesToString(dicParams);
                                addDefaultParams(dicParams, userBalance, index, counter);
                                ActionTypes nextAction  = convertStringToActionType(dicParams["na"]);
                                string      strResponse = convertKeyValuesToString(dicParams);

                                responseMessage.Append(strResponse);
                                if (_dicUserHistory.ContainsKey(strGlobalUserID) && _dicUserHistory[strGlobalUserID].log.Count > 0)
                                    addDoBonusActionHistory(strGlobalUserID, ind, strResponse, index, counter);
                                result.NextAction = nextAction;
                            }
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
                _logger.Error("Exception has been occurred in EmperorCaishenGameLogic::onDoBonus {0}", ex);
            }
        }

        protected override async Task buildStartFreeSpinData(BasePPSlotStartSpinData startSpinData, StartSpinBuildTypes buildType, double minOdd, double maxOdd)
        {
            if (buildType == StartSpinBuildTypes.IsNaturalRandom)
                await base.buildStartFreeSpinData(startSpinData, StartSpinBuildTypes.IsTotalRandom, minOdd, maxOdd);
            else
                await base.buildStartFreeSpinData(startSpinData, buildType, minOdd, maxOdd);
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

        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            EmperorCaishenResult result = new EmperorCaishenResult();
            result.SerializeFrom(reader);
            return result;
        }
        protected override BasePPSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            EmperorCaishenBetInfo betInfo = new EmperorCaishenBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                EmperorCaishenBetInfo betInfo = new EmperorCaishenBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();

                if (betInfo.BetPerLine <= 0.0f || float.IsNaN(betInfo.BetPerLine) || float.IsInfinity(betInfo.BetPerLine))
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in EmperorCaishenGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }

                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1}", strUserID, betInfo.LineCount);
                    return;
                }

                BasePPSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strUserID, out oldBetInfo))
                {
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine = betInfo.BetPerLine;
                    oldBetInfo.LineCount = betInfo.LineCount;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in EmperorCaishenGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
    }
}
