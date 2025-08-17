using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using System.IO;
using Akka.Actor;

namespace SlotGamesNode.GameLogics
{
    public class MustangGoldResult : BasePPSlotSpinResult
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

    public class MustangGoldGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25mustang";
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
                return 25;
            }
        }
        protected override int ServerResLineCount
        {
            get { return 25; }
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
                return "def_s=6,7,4,2,8,9,8,5,6,7,8,6,7,3,9&bgid=0&cfgs=2491&ver=2&mo_s=11&reel_set_size=2&def_sb=10,11,9,6,8&mo_v=25,50,75,125,200,250,300,375,450,500,625,750,875,0&def_sa=7,5,4,4,3&bonusInit=[{bgid:0,bgt:18,bg_i:\"1000,200,100,50\",bg_i_mask:\"pw,pw,pw,pw\"}]&mo_jp=0&scatters=1~0,0,1,0,0~0,0,8,0,0~1,1,1,1,1&gmb=0,0,0&bg_i=1000,200,100,50&rt=d&mo_jp_mask=jpb&bgt=18&sc=10.00,20.00,50.00,100.00,250.00,500.00,1000.00,3000.00,5000.00&defc=100.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&bg_i_mask=pw,pw,pw,pw&paytable=0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;500,50,10,0,0;300,50,10,0,0;250,25,10,0,0;200,25,10,0,0;100,15,5,0,0;100,15,5,0,0;100,15,5,0,0;100,15,5,0,0;0,0,0,0,0;0,0,0,0,0&rtp=95.54&reel_set0=5,7,6,3,10,9,4,5,7,6,9,7,11,11,11,11,8,10,6,9,5,10,6,8,3,4,10,5,7,9,6,10,4,9,5,8,9,6,10,3,7,6,8,10,8,4,7,10,6,9,5,3,10,7~7,2,2,2,2,2,9,3,8,5,9,1,10,6,9,4,7,8,11,11,11,11,7,9,4,8,3,9,5,10,4,9,1,7,5,9,3,8,5,4,9,10,6,9,3,7,4,8,5,9,6,10~3,2,2,2,2,2,8,5,9,1,10,4,8,7,3,8,6,5,8,4,7,6,8,5,9,6,3,9,8,3,10,2,2,2,2,2,8,6,8,5,7,1,8,6,9,3,10,11,11~10,2,2,2,2,2,9,8,5,4,7,1,10,3,7,11,11,11,11,4,9,3,10,1,7,6,9,4,7,8,6,9,3,7,4,10,6,7,1,8,7,3,9,5,7,1,8,6,7,9,5,7,9,6,7,4,5,8,3~7,2,2,2,2,8,6,10,3,9,5,8,3,4,7,6,10,9,5,10,7,6,9,4,3,7,6,10,5,7,12,10,6,7,4,9,6,8,7,6,9,5,3,10,6,7,5,3,8,9,6,10,4,7,3,9,4,9,8&reel_set1=5,7,6,3,10,9,4,5,7,6,9,7,11,11,11,5,8,10,6,9,5,6,10,8,3,4,10,11,11~7,2,2,2,2,2,9,3,8,5,9,1,10,6,9,4,7,8,11,11,11,5,7,9,4,8,3,9~3,2,2,2,2,2,8,5,9,1,10,4,8,11,11,11,6,5,8,4,7,1,8,5,9,6,3,9~10,2,2,2,2,2,2,2,2,4,7,1,10,3,7,5,11,11,11,4,9,3,10,1,7,6,9,4,8~12,2,2,9,6,10,12,9,5,8,3,4,9,6,10,3,9,10,12,6,9,3,7,12,10,8,8,3";
            }
        }
        #endregion
        public MustangGoldGameLogic()
        {
            _gameID = GAMEID.MustangGold;
            GameName = "MustangGold";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("mo_tw"))
                dicParams["mo_tw"] = convertWinByBet(dicParams["mo_tw"], currentBet);

        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams.Add("n_reel_set", "0");
        }
        protected override void onDoBonus(int agentID, string strUserID, GITMessage message, double userBalance, Currencies currency, bool isAffiliate)
        {
            try
            {
                int index   = (int) message.Pop();
                int counter = (int) message.Pop();
                int ind     = (int) message.Pop();

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
                    MustangGoldResult       result          = _dicUserResultInfos[strGlobalUserID] as MustangGoldResult;
                    BasePPSlotBetInfo       betInfo         = _dicUserBetInfos[strGlobalUserID];
                    BasePPActionToResponse  actionResponse  = betInfo.pullRemainResponse();
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
                        string      strResponse = convertKeyValuesToString(dicParams);

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
                _logger.Error("Exception has been occurred in DiamondStrikeGameLogic::onDoBonus {0}", ex);
            }
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "n_reel_set", "mo", "mo_t", "mo_c"};
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
        protected override BasePPSlotSpinResult calculateResult(BasePPSlotBetInfo betInfo, string strSpinResponse, bool isFirst, PPFreeSpinInfo freeSpinInfo)
        {
            try
            {
                MustangGoldResult           spinResult  = new MustangGoldResult();
                Dictionary<string, string>  dicParams   = splitResponseToParams(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(dicParams, betInfo.TotalBet);
                convertBetsByBet(dicParams, betInfo.BetPerLine, betInfo.TotalBet);

                spinResult.NextAction       = convertStringToActionType(dicParams["na"]);
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
                _logger.Error("Exception has been occurred in MustangGoldGameLogic::calculateResult {0}", ex);
                return null;
            }
        }
        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            MustangGoldResult result = new MustangGoldResult();
            result.SerializeFrom(reader);
            return result;
        }

    }
}
