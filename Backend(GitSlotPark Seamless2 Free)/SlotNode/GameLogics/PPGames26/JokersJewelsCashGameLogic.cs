using Akka.Actor;
using GITProtocol;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
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
    public class JokersJewelsCashResult : BasePPSlotSpinResult
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
    class JokersJewelsCashGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs5jokerjc";
            }
        }
        protected override int ClientReqLineCount
        {
            get { return 5; }
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
                return "def_s=985536466798893&cfgs=1&ver=4&reel_set_size=3&def_sb=53693&def_sa=58897&rt=d&gameInfo={rtps:{regular:\"96.51\"},props:{max_rnd_win:\"5000\",max_rnd_hr:\"369686\",max_rnd_sim:\"1\",jp6:\"10\",jp5:\"20\",jp4:\"40\",jp3:\"100\",jp2:\"300\",jp1:\"5000\"}}&wl_i=tbm~5000&sc=40.00,80.00,120.00,160.00,200.00,400.00,600.00,800.00,1000.00,1500.00,2000.00,3000.00,5000.00,10000.00,15000.00,20000.00&defc=200.00&ntp=0.00&paytable=3~3:100,4:1000,5:5000;4~3:50,4:200,5:1000;5~3:50,4:200,5:1000;6~3:20,4:50,5:200;7~3:20,4:50,5:200;8~3:20,4:40,5:200;9~2:5,3:20,4:40,5:200&reel_set0=979548467754647767474477937994467696499897477497B799964477396594744647674774979774B47446774B76494774479977~989956556956468589586696696969885958B88B365854578966999856B598968985669696857568485586863588688685886688595683~3656736789836863787673787467437737789373736B78388688776B78368698863683835683458367~63683434566766466468746568664379638486463575866565B87366756B7546756854364648739995~4376845368638459873759B74975355378734434B85845754&reel_set2=345649565945589B999574397B97559575574785~65856865535568855695665688465B487853988588685665568565B6758~7675985856856895497656859969859589685695B5957995938986958949695869989596756968799878~897364434843B7578943446B643584684493438464863464546~989863696575673783453434568735383894363497598686B85498475357843973734734865469363467964763638543B5945&reel_set1=6474776668397645777677398B888576575989998B9595756~56656866B57446669768646646469677748484464686696588866B9834646864669994646583643766766~6475456648585648684467684366636B37896643563635449463784777646476847697664668657884B9688864566568394437646458587483B6~798534797497958476987B67899899787589989373879873B5798389589539876759798~599B59973987973785877658697769957859783995797989884885479957885487839373B875788436";
            }
        }
        #endregion
        
        public JokersJewelsCashGameLogic()
        {
            _gameID = GAMEID.JokersJewelsCash;
            GameName = "JokersJewelsCash";
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
                JokersJewelsCashResult spinResult = new JokersJewelsCashResult();
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
                _logger.Error("Exception has been occurred in JokersJewelsCashGameLogic::calculateResult {0}", ex);
                return null;
            }
        }
        protected override BasePPSlotSpinResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            JokersJewelsCashResult result = new JokersJewelsCashResult();
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
                double              realWin    = 0.0;
                string              strGameLog = "";
                ToUserResultMessage resultMsg  = null;

                if (!_dicUserResultInfos.ContainsKey(strGlobalUserID) || !_dicUserBetInfos.ContainsKey(strGlobalUserID) ||
                    !_dicUserBetInfos[strGlobalUserID].HasRemainResponse)
                {
                    responseMessage.Append("unlogged");
                }
                else
                {
                    JokersJewelsCashResult result              = _dicUserResultInfos[strGlobalUserID] as JokersJewelsCashResult;
                    BasePPSlotBetInfo betInfo               = _dicUserBetInfos[strGlobalUserID];
                    BasePPActionToResponse actionResponse   = betInfo.pullRemainResponse();
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

                        if(result.BonusSelections.Contains(ind))
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
                                    int[] status = new int[18];
                                    for (int i = 0; i < result.BonusSelections.Count; i++)
                                        status[result.BonusSelections[i]] = i + 1;
                                    gParam["bg_0"]["status"] = string.Join(",", status);

                                    List<string> strNewChh = new List<string>();
                                    for (int i = 0; i < result.BonusSelections.Count; i++)
                                    {
                                        strNewChh.Add(string.Format("0~{0}", result.BonusSelections[i]));
                                    }
                                    string strChh = string.Join(",",strNewChh.ToArray());
                                    gParam["bg_0"]["ch_h"] = strChh;
                                }

                                if (gParam["bg_0"]["wins_mask"] != null)
                                {
                                    string[] strWinsMask    = gParam["bg_0"]["wins_mask"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    string[] strNewWinsMask = new string[18];

                                    for (int i = 0; i < 18; i++)
                                        strNewWinsMask[i] = "h";

                                    for (int i = 0; i < result.BonusSelections.Count; i++)
                                        strNewWinsMask[result.BonusSelections[i]] = strWinsMask[i];

                                    gParam["bg_0"]["wins_mask"] = string.Join(",", strNewWinsMask);
                                }
                                if (gParam["bg_0"]["wins"] != null)
                                {
                                    string[] strWins    = gParam["bg_0"]["wins"].ToString().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                    string[] strNewWins = new string[18];

                                    for (int i = 0; i < 18; i++)
                                        strNewWins[i] = "0";

                                    for (int i = 0; i < result.BonusSelections.Count; i++)
                                        strNewWins[result.BonusSelections[i]] = strWins[i];

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
                            resultMsg                   = new ToUserResultMessage((int)_gameID, responseMessage, 0.0, realWin, new GameLogInfo(this.GameName, "0", strGameLog), UserBetTypes.Normal); ;
                            resultMsg.BetTransactionID  = betInfo.BetTransactionID;
                            resultMsg.RoundID           = betInfo.RoundID;
                            resultMsg.TransactionID     = createTransactionID();
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
                _logger.Error("Exception has been occurred in JokersJewelsCashGameLogic::onDoBonus {0}", ex);
            }
        }
    }
}
