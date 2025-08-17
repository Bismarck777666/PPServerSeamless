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
    public class SojuBombGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGSojuBomb";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "2517a00a-7545-4048-bde5-776724653ea0";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "24c0003839bf5cdecd7cd6a0fb2c87abfe53ed21";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.10548.421";
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
                    {1,     new HabaneroLogSymbolIDName{id = "idWild",          name = "Wild"           } },
                    {2,     new HabaneroLogSymbolIDName{id = "idScatter",       name = "Scatter"        } },
                    {3,     new HabaneroLogSymbolIDName{id = "idPeach",         name = "Peach"          } },
                    {4,     new HabaneroLogSymbolIDName{id = "idCherry",        name = "Cherry"         } },
                    {5,     new HabaneroLogSymbolIDName{id = "idGrape",         name = "Grape"          } },
                    {6,     new HabaneroLogSymbolIDName{id = "idLemon",         name = "Lemon"          } },
                    {7,     new HabaneroLogSymbolIDName{id = "idBlueDrink",     name = "BlueDrink"      } },
                    {8,     new HabaneroLogSymbolIDName{id = "idRedDrink",      name = "RedDrink"       } },
                    {9,     new HabaneroLogSymbolIDName{id = "idPurpleDrink",   name = "PurpleDrink"    } },
                    {10,    new HabaneroLogSymbolIDName{id = "idYellowDrink",   name = "YellowDrink"    } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 703;
            }
        }
        #endregion

        public SojuBombGameLogic()
        {
            _gameID     = GAMEID.SojuBomb;
            GameName    = "SojuBomb";
        }

        protected override async Task onProcMessage(string strUserID, int agentID, CurrencyEnum currency, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            if (message.MsgCode == (ushort)CSMSG_CODE.CS_HABANERO_DOCLIENT)
            {
                await onDoSpin(strUserID, agentID, (int)currency, message, userBonus, userBalance, isMustLose);
            }
            else
            {
                await base.onProcMessage(strUserID, agentID, currency, message, userBonus, userBalance, isMustLose);
            }
        }

        protected override HabaneroActionType convertStringToAction(string strAction)
        {
            if (string.IsNullOrEmpty(strAction))
                return HabaneroActionType.PICKOPTION;

            return base.convertStringToAction(strAction);
        }

        protected override JObject buildEventItem(string strGlobalUserID, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strGlobalUserID, currentIndex);

            HabaneroHistoryResponses startResponses = _dicUserHistory[strGlobalUserID].Responses[0];
            dynamic startResponse = JsonConvert.DeserializeObject<dynamic>(startResponses.Response);

            HabaneroHistoryResponses responses = _dicUserHistory[strGlobalUserID].Responses[currentIndex];
            dynamic response = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            int multiplier = 1;
            if(responses.Action != HabaneroActionType.MAIN && responses.Action != HabaneroActionType.PICKOPTION)
            {
                if (!object.ReferenceEquals(startResponse["SojuBomb_PickResults"], null))
                {
                    for (int i = 0; i < startResponse["SojuBomb_PickResults"].Count; i++)
                    {
                        if (startResponse["SojuBomb_PickResults"][i]["type"] == "multiplier")
                        {
                            multiplier = startResponse["SojuBomb_PickResults"][i]["winmultiplier"];
                            break;
                        }
                    }
                }
            }
            eventItem["multiplier"] = multiplier;

            JArray subEvents = new JArray();
            if (!object.ReferenceEquals(response["SojuBomb_PickResults"], null))
            {
                for (int i = 0; i < response["SojuBomb_PickResults"].Count; i++)
                {
                    JObject subEventItem = new JObject();
                    subEventItem["type"]        = "pick";
                    subEventItem["picktype"]    = response["SojuBomb_PickResults"][i]["type"];
                    
                    if(!object.ReferenceEquals(response["SojuBomb_PickResults"][i]["winfreegames"], null))
                        subEventItem["winfreegames"] = response["SojuBomb_PickResults"][i]["winfreegames"];
                    if (!object.ReferenceEquals(response["SojuBomb_PickResults"][i]["winmultiplier"], null))
                        subEventItem["winmultiplier"] = response["SojuBomb_PickResults"][i]["winmultiplier"];

                    subEvents.Add(subEventItem);
                }
            }

            if(subEvents.Count > 0)
            {
                if(!object.ReferenceEquals(eventItem["subevents"], null))
                {
                    for (int i = 0; i < subEvents.Count; i++)
                    {
                        (eventItem["subevents"] as dynamic).Add(subEvents[i]);
                    }
                }
                else
                {
                    eventItem["subevents"] = subEvents;
                }
            }

            return eventItem;
        }

        protected override HabaneroLogItem buildHabaneroLogItem(BaseHabaneroSlotBetInfo betInfo, HabaneroHistoryItem history,int agentID, string strUserID, int currency, double balance,double betMoney ,double winMoney)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);

                HabaneroLogItem historyItem = new HabaneroLogItem();
                historyItem.AgentID     = agentID;
                historyItem.UserID      = strUserID;
                historyItem.GameID      = (int)this._gameID;
                historyItem.Time        = DateTime.UtcNow;
                historyItem.RoundID     = history.RoundId;
                historyItem.GameLogID   = history.GameId;
                historyItem.Bet         = betInfo.TotalBet;
                if (betInfo.MoreBet > 0)
                    historyItem.Bet = betInfo.TotalBet * this.MoreBetMultiple;
                if (betInfo.PurchaseFree > 0)
                    historyItem.Bet = betInfo.TotalBet * getPurchaseMultiple(betInfo);
                historyItem.Win         = winMoney;

                HabaneroLogItemOverview overView = new HabaneroLogItemOverview();
                overView.CurrencyCode   = new Currencies()._currencyInfo[currency].CurrencyText;
                overView.DateToShow     = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
                overView.DtCompleted    = string.Format("/Date({0})/",overView.DateToShow);
                overView.Exponent       = 0;
                overView.ExtRoundId     = null;
                overView.FriendlyId     = Convert.ToInt64(history.RoundId);
                overView.GameInstanceId = history.GameId;
                overView.GameKeyName    = this.SymbolName;
                overView.GameStateId    = 3;
                overView.IsTestSite     = false;
                overView.RealPayout     = winMoney;
                overView.RealStake      = historyItem.Bet;
                overView.ReplayURL      = HabaneroConfig.Instance.ReplayURL + history.GameId;
                overView.BrandGameId    = BrandGameId;
                overView.IsSpecialBrandGame = false;
                overView.GameTypeId = 11;

                historyItem.Overview = JsonConvert.SerializeObject(overView);

                JObject detailValue = new JObject();
                detailValue["GameType"]              = 11;
                detailValue["GameState"]             = "GameState_3";
                detailValue["GameKeyName"]           = SymbolName;
                detailValue["CurrencyCode"]          = new Currencies()._currencyInfo[currency].CurrencyText;
                detailValue["FriendlyId"]            = history.RoundId;
                detailValue["DtStarted"]             = overView.DateToShow - 10;
                detailValue["DtCompleted"]           = overView.DateToShow;
                detailValue["RealStake"]             = historyItem.Bet;
                detailValue["RealPayout"]            = winMoney;
                detailValue["BonusStake"]            = 0;
                detailValue["BonusPayout"]           = 0;
                detailValue["BonusToReal"]           = 0;
                detailValue["CurrencyExponent"]      = 0;
                detailValue["BalanceAfter"]          = Math.Round(balance - betMoney + winMoney, 2);
                detailValue["IsCheat"]               = false;

                dynamic detailResult            = new JObject();
                dynamic detailResultReportInfo  = new JObject();
                detailResultReportInfo["numcoins"]          = this.MiniCoin;
                detailResultReportInfo["bettype"]           = this.BetType;
                detailResultReportInfo["paylinecount"]      = this.ClientReqLineCount;
                detailResultReportInfo["coindenomination"]  = betInfo.CoinValue;
                detailResultReportInfo["linebet"]           = betInfo.CoinValue * betInfo.BetLevel;
                detailResultReportInfo["betlevel"]          = betInfo.BetLevel;
                detailResultReportInfo["totalbet"]          = historyItem.Bet;
                detailResultReportInfo["wincash"]           = winMoney;
                if(betInfo.MoreBet > 0)
                    detailResultReportInfo["superbet"] = betInfo.MoreBet;

                int winFreeGames = 0;
                dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(history.Responses[0].Response);
                if (!object.ReferenceEquals(resultContext["winfreegames"], null))
                    winFreeGames = Convert.ToInt32(resultContext["winfreegames"]);
                detailResultReportInfo["winfreegames"]      = winFreeGames;

                JArray eventArray = new JArray();
                for(int i = 0; i < history.Responses.Count; i++)
                {
                    dynamic response = JsonConvert.DeserializeObject<dynamic>(history.Responses[i].Response);
                    if(!object.ReferenceEquals(response["SojuBomb_PickStep"], null))
                        continue;

                    JObject eventItem = buildEventItem(strGlobalUserID, i);
                    eventArray.Add(eventItem);
                }

                dynamic startResponse = JsonConvert.DeserializeObject<dynamic>(history.Responses[0].Response);
                if (!object.ReferenceEquals(startResponse["SojuBomb_PickResults"], null))
                {
                    dynamic eventItem1 = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(eventArray[0]));
                    dynamic eventItem2 = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(eventArray[0]));
                    eventArray.RemoveAt(0);

                    JArray subEvents1 = new JArray();
                    for(int i = 0; i < eventItem1["subevents"].Count; i++)
                    {
                        if(eventItem1["subevents"][i]["type"] != "pick")
                        {
                            subEvents1.Add(eventItem1["subevents"][i]);
                        }
                    }
                    eventItem1["subevents"] = subEvents1;

                    JArray subEvents2 = new JArray();
                    for (int i = 0; i < eventItem2["subevents"].Count; i++)
                    {
                        if (eventItem2["subevents"][i]["type"] == "pick")
                        {
                            subEvents2.Add(eventItem2["subevents"][i]);
                        }
                    }
                    eventItem2["subevents"] = subEvents2;
                    eventItem2["type"]      = "pick";
                    eventItem2["gamemode"]  = "PICK";
                    eventItem2["wincash"]   = 0;

                    eventArray.Insert(0, eventItem1);
                    eventArray.Insert(1, eventItem2);
                }

                detailResultReportInfo["events"]    = eventArray;
                detailResult["ReportInfo"]          = JsonConvert.SerializeObject(detailResultReportInfo);
                detailValue["VideoSlotGameDetails"] = detailResult;

                JObject detail = new JObject();
                detail["d"]         = JsonConvert.SerializeObject(detailValue);
                historyItem.Detail  = JsonConvert.SerializeObject(detail);

                return historyItem;
            }
            catch (Exception ex)
            {
                 _logger.Error("Exception has been occurred in BaseHabaneroSlotGame::saveResultToHistory {0}", ex);
                return null;
            }
        }

        protected override void onDoInit(string strGlobalUserID, int currency, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            try
            {
                string strGrid          = (string)message.Pop();
                string strToken         = (string)message.Pop();

                HabaneroResponse response = new HabaneroResponse();
                
                HabaneroResponseHeader header = makeHabaneroResponseHeader(strGlobalUserID, currency, userBalance, strToken);

                JObject portMessage = new JObject();
                portMessage["reelid"] = InitReelStatusNo;
                if(_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    BaseHabaneroSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
                    string gameInstanceId   = _dicUserResultInfos[strGlobalUserID].GameId;
                    string gameRoundId      = _dicUserResultInfos[strGlobalUserID].RoundId;

                    dynamic lastResult = JsonConvert.DeserializeObject<dynamic>(_dicUserResultInfos[strGlobalUserID].ResultString);
                    if(object.ReferenceEquals(lastResult["isgamedone"], null) || !Convert.ToBoolean(lastResult["isgamedone"]))
                    {
                        JArray resumeGames = buildInitResumeGame(strGlobalUserID, betInfo,lastResult,gameInstanceId,gameRoundId);
                        portMessage["resumegames"]  = resumeGames;
                        portMessage["gssid"]        = lastResult["gssid"];
                    }
                }

                JObject game = new JObject();
                game["action"]      = "init";
                game["apiversion"]  = "5.1.10768.643";
                game["brandgameid"] = BrandGameId;
                game["friendlyid"]  = 0;
                game["gamehash"]    = GameHash;
                game["gameid"]      = "00000000-0000-0000-0000-000000000000";
                game["gameversion"] = this.GameVersion;
                game["jphash"]      = JPHash;
                game["jpversion"]   = JPVersion;
                game["rnghash"]     = RngHash;
                game["rngversion"]  = RngVersion;
                game["sessionid"]   = Guid.NewGuid().ToString();
                game["init"]        = new JObject();
                game["init"]["coinsincrement"]  = string.Join("|", CoinsIncrement);

                List<double> newStakeIncrement = new List<double>();
                for (int i = 0; i < StakeIncrement.Length; i++)
                {
                    newStakeIncrement.Add(StakeIncrement[i] * new Currencies()._currencyInfo[currency].Rate);
                }

                game["init"]["stakeincrement"]  = string.Join("|", newStakeIncrement);
                game["init"]["configid"]        = Guid.NewGuid().ToString();
                game["init"]["defaultstake"]    = newStakeIncrement[4];
                game["init"]["maxpaylimit"]     = MaxPayLimit * newStakeIncrement[newStakeIncrement.Count - 1];
                game["init"]["maxstake"]        = MiniCoin * CoinsIncrement[CoinsIncrement.Length - 1] * newStakeIncrement[newStakeIncrement.Count - 1];
                game["init"]["minstake"]        = MiniCoin * CoinsIncrement[0] * newStakeIncrement[0];

                response.game           = game;
                response.header         = header;
                response.grid           = strGrid;
                response.portmessage    = portMessage;
                
                GITMessage responseMessage      = new GITMessage((ushort)SCMSG_CODE.SC_HABANERO_DOINIT);
                responseMessage.Append(JsonConvert.SerializeObject(response));
                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseHabaneroSlotGame::onDoInit GameID: {0}, {1}", _gameID, ex);
            }
        }

        protected override JArray buildInitResumeGame(string strGlobalUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            JArray resumeGames = base.buildInitResumeGame(strGlobalUserID, betInfo, lastResult, gameinstanceid, roundid, currentAction);

            HabaneroHistoryResponses startResponses = _dicUserHistory[strGlobalUserID].Responses[0];
            dynamic startResponse = JsonConvert.DeserializeObject<dynamic>(startResponses.Response);

            if(!object.ReferenceEquals(lastResult["SojuBomb_PickResults"], null))
            {
                resumeGames[0]["SojuBomb_PickResults"]  = lastResult["SojuBomb_PickResults"];
                resumeGames[0]["SojuBomb_PickStep"]     = 0;
                resumeGames[0]["currfreegame"]          = 0;
                resumeGames[0]["numfreegames"]          = 0;
                resumeGames[0]["gamemode"]              = "pick";
            }
            else if(!object.ReferenceEquals(lastResult["SojuBomb_PickStep"], null))
            {
                resumeGames[0]["SojuBomb_PickStep"]     = lastResult["SojuBomb_PickStep"];
                resumeGames[0]["SojuBomb_PickResults"]  = startResponse["SojuBomb_PickResults"];
                resumeGames[0]["totalwincash"]          = startResponse["totalwincash"];
                resumeGames[0]["virtualreels"]          = startResponse["virtualreels"];
                resumeGames[0]["currfreegame"]          = 0;
                resumeGames[0]["numfreegames"]          = 0;
                resumeGames[0]["gamemode"]              = "pick";
                if ((int)lastResult["SojuBomb_PickStep"] == 2)
                {
                    for(int i = 0; i < startResponse["SojuBomb_PickResults"].Count; i++)
                    {
                        if(!object.ReferenceEquals(startResponse["SojuBomb_PickResults"][i]["winfreegames"], null))
                        {
                            resumeGames[0]["numfreegames"] = startResponse["SojuBomb_PickResults"][i]["winfreegames"];
                            break;
                        }
                    }
                    resumeGames[0]["gamemode"] = "freegame";
                }
            }

            return resumeGames;
        }
    }
}
