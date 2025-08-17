using GITProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    public class RuffledUpGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGRuffledUp";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "170a632f-b772-46fa-b3d4-49ec57afd5da";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "61eb88c46be640f8bae7735867f50694168684a7";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.1331.93";
            }
        }
        protected override float MiniCoin
        {
            get
            {
                return 25.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 243;
            }
        }
        protected override string BetType
        {
            get
            {
                return "Ways";
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idWildBird",    name = "WildBird"       } },
                    {2,   new HabaneroLogSymbolIDName{id = "idScatterBird", name = "ScatterBird"    } },
                    {3,   new HabaneroLogSymbolIDName{id = "idBird1",       name = "Bird1"          } },
                    {4,   new HabaneroLogSymbolIDName{id = "idBird2",       name = "Bird2"          } },
                    {5,   new HabaneroLogSymbolIDName{id = "idBird3",       name = "Bird3"          } },
                    {6,   new HabaneroLogSymbolIDName{id = "idBird4",       name = "Bird4"          } },
                    {7,   new HabaneroLogSymbolIDName{id = "idBird5",       name = "Bird5"          } },
                    {8,   new HabaneroLogSymbolIDName{id = "idBird6",       name = "Bird6"          } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 297;
            }
        }
        #endregion

        public RuffledUpGameLogic()
        {
            _gameID     = GAMEID.RuffledUp;
            GameName    = "RuffledUp";
        }

        #region 스핀조작
        protected override BaseHabaneroSlotSpinResult calculateResult(string strUserID, BaseHabaneroSlotBetInfo betInfo, string strSpinResponse, bool isFirst,HabaneroActionType currentAction)
        {
            try
            {
                BaseHabaneroSlotSpinResult spinResult     = new BaseHabaneroSlotSpinResult();
                dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(resultContext, betInfo.TotalBet);

                string strNextAction    = (string)resultContext["nextgamestate"];
                spinResult.NextAction   = convertStringToAction(strNextAction);

                if (spinResult.NextAction == HabaneroActionType.NONE)
                {
                    _logger.Error("Unknown Action in BaseHabaneroSlotGame::calculateResult Action is {0}", strNextAction);
                    return null;
                }

                spinResult.CurrentAction = currentAction;

                if (isFirst)
                {
                    spinResult.RoundId  = (((long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds) * 5).ToString();
                    spinResult.GameId   = Guid.NewGuid().ToString();
                }
                else
                {
                    spinResult.RoundId  = _dicUserResultInfos[strUserID].RoundId;
                    spinResult.GameId   = _dicUserResultInfos[strUserID].GameId;
                }
                
                if ((bool)resultContext["isgamedone"])
                    spinResult.TotalWin = Convert.ToDouble(resultContext["totalwincash"]);
                
                spinResult.ResultString = JsonConvert.SerializeObject(resultContext);
                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseHabaneroSlotGame::calculateResult {0}", ex);
                return null;
            }
        }

        protected override void sendGameResult(BaseHabaneroSlotBetInfo betInfo, BaseHabaneroSlotSpinResult spinResult,int agentID, string strUserID,int currency, double betMoney, double winMoney, string strGameLog, double userBalance, string strSessionID, string strToken,string strGrid ,HabaneroResponseHeader responseHeader)
        {
            string strGlobalUserID  = string.Format("{0}_{1}", agentID, strUserID);
            string strSpinResult    = makeSpinResultString(betInfo, spinResult, betMoney, userBalance, strGlobalUserID, strSessionID, strToken,strGrid ,responseHeader);
            GITMessage message      = new GITMessage((ushort)SCMSG_CODE.SC_HABANERO_DOSPIN);
            message.Append(strSpinResult);

            ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, message, betMoney, winMoney, new GameLogInfo(GameName, "0", strGameLog),UserBetTypes.Normal);
            if (_isRewardedBonus)
            {
                toUserResult.setBonusReward(_rewardedBonusMoney);
                toUserResult.insertFirstMessage(_bonusSendMessage);
            }

            if (_dicUserHistory.ContainsKey(strGlobalUserID))
                _dicUserHistory[strGlobalUserID].Responses.Add(new HabaneroHistoryResponses(spinResult.CurrentAction, DateTime.UtcNow, spinResult.ResultString));

            dynamic portMessage = JsonConvert.DeserializeObject<dynamic>(spinResult.ResultString);
            bool isgamedone     = (bool)portMessage["isgamedone"];

            if (isgamedone)
                saveHistory(agentID, strUserID, currency, userBalance, betMoney, spinResult.TotalWin);

            Sender.Tell(toUserResult, Self);
        }

        protected override void convertWinsByBet(dynamic resultContext, float currentBet)
        {
            base.convertWinsByBet(resultContext as JObject, currentBet);

            if (!object.ReferenceEquals(resultContext["RU_lightningStrikes"], null))
            {
                for(int i = 0; i < resultContext["RU_lightningStrikes"].Count; i++)
                {
                    double winCash   = (double)resultContext["RU_lightningStrikes"][i]["winCash"];
                    resultContext["RU_lightningStrikes"][i]["winCash"] = convertWinByBet(winCash, currentBet);
                }
            }
        }
        #endregion

        #region 리력쓰기
        protected override JArray buildInitResumeGame(string strGlobalUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {

            JArray resumeGames = base.buildInitResumeGame(strGlobalUserID, betInfo, lastResult, gameinstanceid, roundid, currentAction);
            int RU_yellowBirdIndex = -1;
            if(!object.ReferenceEquals(lastResult["RU_yellowBirdIndex"], null))
                RU_yellowBirdIndex = (int)lastResult["RU_yellowBirdIndex"];
            resumeGames[0]["RU_yellowBirdIndex"] = RU_yellowBirdIndex;

            return resumeGames;
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);

            dynamic eventContext = eventItem as dynamic;
            if (!object.ReferenceEquals(eventContext["subevents"], null) && eventContext["subevents"].Count > 0)
            {
                for (int i = 0; i < eventContext["subevents"].Count; i++)
                {
                    if (eventContext["subevents"][i]["type"] == "scatter")
                    {
                        eventContext["subevents"][i]["symbol"] = SymbolIdStringForLog[2].name;
                    }
                }
            }

            if (Convert.ToString(eventItem["gamemode"]) == "FREEGAME")
                eventItem["multiplier"] = 1;

            return eventItem;
        }

        protected override JArray buildHabaneroLogReels(string strGlobalUserID, int currentIndex, dynamic response,bool containWild = false)
        {

            if (object.ReferenceEquals(response["RU_yellowBirdIndex"], null))
                return base.buildHabaneroLogReels(strGlobalUserID, currentIndex, response as JObject, containWild);
            
            int yellowIndex = Convert.ToInt32(response["RU_yellowBirdIndex"]);

            JArray reels = new JArray();
            for (int j = 0; j < response["virtualreels"].Count; j++)
            {
                JArray col = new JArray();
                for (int k = 2; k < response["virtualreels"][j].Count - 2; k++)
                {
                    int symbol  = Convert.ToInt32(response["virtualreels"][j][k]);
                    if (yellowIndex == j)
                        symbol  = 1;
                    string symbolid = SymbolIdStringForLog[symbol].id;
                    col.Add(symbolid);
                }
                reels.Add(col);
            }
            return reels;
        }

        protected override HabaneroLogItem buildHabaneroLogItem(BaseHabaneroSlotBetInfo betInfo, HabaneroHistoryItem history, int agentID, string strUserID, int currency, double balance, double betMoney, double winMoney)
        {
            string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
            HabaneroLogItem logItem = base.buildHabaneroLogItem(betInfo, history,agentID, strUserID, currency, balance, betMoney, winMoney);

            dynamic logDetail   = JsonConvert.DeserializeObject<dynamic>(logItem.Detail);
            dynamic detailValue = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(logDetail["d"]));
            dynamic reportInfo  = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(detailValue["VideoSlotGameDetails"]["ReportInfo"]));

            JArray eventArray       = reportInfo["events"] as JArray;
            JArray newEventArray    = new JArray();
            for(int i = 0; i < eventArray.Count; i++)
            {
                JObject eventItem       = eventArray[i] as JObject;
                dynamic resultContext   = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserID].Responses[i].Response);
                
                if (object.ReferenceEquals(resultContext["RU_stormBringerPositions"], null) && object.ReferenceEquals(resultContext["RU_lightningStrikes"], null))
                {
                    newEventArray.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(eventItem)));
                    continue;
                }

                JObject sceneAItem  = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(eventItem));
                JObject sceneBItem  = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(eventItem));
                sceneAItem["sceneno"] = "A";
                sceneBItem["sceneno"] = "B";

                if (!object.ReferenceEquals(resultContext["RU_yellowBirdIndex"], null))
                {
                    JArray reels = base.buildHabaneroLogReels(strUserID, i, resultContext as JObject);
                    sceneAItem["reels"] = reels;
                }

                JArray subAEvents = new JArray();
                JArray subBEvents = new JArray();
                if (!object.ReferenceEquals(resultContext["RU_stormBringerPositions"], null))
                {
                    for (int j = 0; j < resultContext["RU_stormBringerPositions"].Count; j++)
                    {
                        int reelindex   = Convert.ToInt32(resultContext["RU_stormBringerPositions"][j]["reelindex"]);
                        int symbolindex = Convert.ToInt32(resultContext["RU_stormBringerPositions"][j]["symbolindex"]);
                        sceneAItem["reels"][reelindex][symbolindex] = SymbolIdStringForLog[2].id;
                    }

                    if (!object.ReferenceEquals(eventItem["subevents"], null))
                    {
                        for (int j = 0; j < (eventItem["subevents"] as dynamic).Count; j++)
                        {
                            if(Convert.ToString(eventItem["subevents"][j]["type"]) == "scatter")
                                subAEvents.Add(JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(eventItem["subevents"][j])));
                            else
                                subBEvents.Add(JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(eventItem["subevents"][j])));
                        }
                    }
                    sceneAItem["wincash"] = resultContext["scatterwinscash"];
                    sceneBItem["wincash"] = resultContext["linewinscash"];
                }

                if(!object.ReferenceEquals(resultContext["RU_lightningStrikes"], null))
                {
                    double lightWin = 0.0;
                    for (int j = 0; j < resultContext["RU_lightningStrikes"].Count; j++)
                    {
                        int reelindex   = Convert.ToInt32(resultContext["RU_lightningStrikes"][j]["reelIndex"]);
                        int symbolindex = Convert.ToInt32(resultContext["RU_lightningStrikes"][j]["symbolIndex"]);
                        double winCash  = Convert.ToDouble(resultContext["RU_lightningStrikes"][j]["winCash"]);
                        lightWin += winCash;

                        JObject subEventItem = new JObject();
                        subEventItem["type"]                = "lightningstrike";
                        subEventItem["wincash"]             = winCash;
                        int symbolid = Convert.ToInt32(resultContext["reels"][reelindex][symbolindex]["symbolid"]);
                        subEventItem["symbol"]              = SymbolIdStringForLog[symbolid].name;
                        subEventItem["multiplier"]          = 1;
                        subEventItem["wincashmultiplier"]   = 1;
                        subEventItem["winfreegames"]        = 0;

                        JArray windows  = new JArray();
                        JArray window   = new JArray();
                        window.Add(reelindex);
                        window.Add(symbolindex);
                        windows.Add(window);
                        subEventItem["windows"]             = windows;
                        subAEvents.Add(subEventItem);
                    }

                    if (!object.ReferenceEquals(eventItem["subevents"], null))
                    {
                        for (int j = 0; j < (eventItem["subevents"] as dynamic).Count; j++)
                        {
                            subBEvents.Add(JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(eventItem["subevents"][j])));
                        }
                    }

                    sceneAItem["wincash"] = lightWin;
                    sceneBItem["wincash"] = resultContext["linewinscash"];
                }

                sceneAItem["subevents"] = subAEvents;
                sceneBItem["subevents"] = subBEvents;
                
                newEventArray.Add(sceneAItem);
                newEventArray.Add(sceneBItem);
            }
            
            reportInfo["events"] = newEventArray;
            detailValue["VideoSlotGameDetails"]["ReportInfo"] = JsonConvert.SerializeObject(reportInfo);
            logDetail["d"] = JsonConvert.SerializeObject(detailValue);
            logItem.Detail = JsonConvert.SerializeObject(logDetail);
            return logItem;
        }
        #endregion
    }
}
