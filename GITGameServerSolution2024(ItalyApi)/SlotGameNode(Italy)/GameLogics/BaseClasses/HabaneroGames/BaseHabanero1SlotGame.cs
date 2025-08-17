using Akka.Actor;
using GITProtocol;
using GITProtocol.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCGSharp;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    public class BaseHabanero1SlotGame : BaseHabaneroSlotGame
    {
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
                    if(!object.ReferenceEquals(lastResult["videoslotstate"]["isgamedone"], null) && !Convert.ToBoolean(lastResult["videoslotstate"]["isgamedone"]))
                    {
                        HabaneroActionType nextAction = _dicUserResultInfos[strGlobalUserID].NextAction;
                        JArray resumeGames = buildInitResumeGame(strGlobalUserID, betInfo,lastResult,gameInstanceId,gameRoundId,nextAction);
                        response.resumevsgamelist = resumeGames;
                    }
                }

                JObject game = new JObject();
                game["action"]      = "init";
                game["apiversion"]  = "5.1.10768.643";
                game["brandgameid"] = BrandGameId;
                game["friendlyid"]  = 0;
                game["gamehash"]    = GameHash;
                game["gameid"]      = "00000000-0000-0000-0000-000000000000";
                game["gameversion"] = GameVersion;
                game["jphash"]      = JPHash;
                game["jpversion"]   = JPVersion;
                game["rnghash"]     = RngHash;
                game["rngversion"]  = RngVersion;
                game["sessionid"]   = Guid.NewGuid().ToString();
                
                game["init"]        = new JObject();
                game["init"]["coinsincrement"]  = string.Join("|", CoinsIncrement);

                List<double> newStakeIncrement = new List<double>();
                for(int i = 0; i < StakeIncrement.Length; i++)
                {
                    newStakeIncrement.Add(StakeIncrement[i] * new Currencies()._currencyInfo[currency].Rate);
                }

                game["init"]["stakeincrement"]  = string.Join("|", newStakeIncrement);
                game["init"]["configid"]        = Guid.NewGuid().ToString();
                game["init"]["defaultstake"]    = newStakeIncrement[4];
                game["init"]["maxpaylimit"]     = MaxPayLimit * newStakeIncrement[newStakeIncrement.Count - 1];
                game["init"]["maxstake"]        = MiniCoin * CoinsIncrement[CoinsIncrement.Length - 1] * newStakeIncrement[newStakeIncrement.Count - 1];
                game["init"]["minstake"]        = MiniCoin * CoinsIncrement[0] * newStakeIncrement[0];
                game["videoslotgameinit"] = portMessage;

                response.game   = game;
                response.header = header;
                response.grid   = strGrid;
                
                GITMessage responseMessage      = new GITMessage((ushort)SCMSG_CODE.SC_HABANERO_DOINIT);
                responseMessage.Append(JsonConvert.SerializeObject(response));
                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseHabanero1SlotGame::onDoInit GameID: {0}, {1}", _gameID, ex);
            }
        }

        protected override BaseHabaneroSlotSpinResult calculateResult(string strGlobalUserID, BaseHabaneroSlotBetInfo betInfo, string strSpinResponse, bool isFirst,HabaneroActionType currentAction)
        {
            try
            {
                BaseHabaneroSlotSpinResult spinResult = new BaseHabaneroSlotSpinResult();
                dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(strSpinResponse);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(resultContext, betInfo.TotalBet);

                string strNextAction = (string)resultContext["videoslotstate"]["gamemodename"];
                spinResult.NextAction = convertStringToAction(strNextAction);
                if(spinResult.NextAction == HabaneroActionType.NONE)
                {
                    _logger.Error("Unknown Action in BaseHabanero1SlotGame::calculateResult Action is {0}", strNextAction);
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
                    spinResult.RoundId  = _dicUserResultInfos[strGlobalUserID].RoundId;
                    spinResult.GameId   = _dicUserResultInfos[strGlobalUserID].GameId;
                }

                if (spinResult.NextAction == HabaneroActionType.MAIN)
                    spinResult.TotalWin = Convert.ToDouble(resultContext["totalpayout"]);

                spinResult.ResultString = JsonConvert.SerializeObject(resultContext);
                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseHabanero1SlotGame::calculateResult {0}", ex);
                return null;
            }
        }

        protected override string makeSpinResultString(BaseHabaneroSlotBetInfo betInfo, BaseHabaneroSlotSpinResult spinResult, double betMoney, double userBalance, string strUserID, string strSessionID, string strToken, string strGrid, HabaneroResponseHeader responseHeader)
        {
            HabaneroResponse response = new HabaneroResponse();
            HabaneroResponseGame game = new HabaneroResponseGame();
            game.action         = spinResult.CurrentAction == HabaneroActionType.PICKOPTION ? "client" : "game";
            game.brandgameid    = BrandGameId;
            game.dts            = DateTime.UtcNow.ToString("o", CultureInfo.GetCultureInfo("en-US"));
            game.gameid         = spinResult.GameId;
            game.sessionid      = strSessionID;
            game.friendlyid     = Convert.ToInt64(spinResult.RoundId);
            game.play           = JsonConvert.DeserializeObject<dynamic>(spinResult.ResultString);

            response.game           = game;
            response.header         = responseHeader;
            response.grid           = strGrid;
            return JsonConvert.SerializeObject(response);
        }

        protected override List<BaseHabaneroActionToResponse> buildResponseList(List<string> responseList)
        {
            List<BaseHabaneroActionToResponse> actionResponseList = new List<BaseHabaneroActionToResponse>();
            for (int i = 1; i < responseList.Count; i++)
            {
                dynamic resultContext   = JsonConvert.DeserializeObject<dynamic>(responseList[i - 1]);
                string strNextAction    = resultContext["videoslotstate"]["gamemodename"];
                HabaneroActionType actionType = convertStringToAction(strNextAction);

                actionResponseList.Add(new BaseHabaneroActionToResponse(actionType, responseList[i]));
            }
            return actionResponseList;
        }
        
        protected override HabaneroActionType convertStringToAction(string strAction)
        {
            switch (strAction)
            {
                case "bonus":
                    return HabaneroActionType.BONUS;
                default:
                    return base.convertStringToAction(strAction);
            }
        }

        protected override string convertActionToString(HabaneroActionType action)
        {
            switch (action)
            {
                case HabaneroActionType.BONUS:
                    return "bonus";
                default:
                    return base.convertActionToString(action);
            }
        }

        protected override void convertWinsByBet(dynamic resultContext, float currentBet)
        {
            if (!object.ReferenceEquals(resultContext["totalstake"], null))
                resultContext["totalstake"] = convertWinByBet((double)resultContext["totalstake"], currentBet);
            
            if (!object.ReferenceEquals(resultContext["totalpayout"], null))
                resultContext["totalpayout"] = convertWinByBet((double)resultContext["totalpayout"], currentBet);

            if (!object.ReferenceEquals(resultContext["videoslotstate"], null))
            {
                if (!object.ReferenceEquals(resultContext["videoslotstate"]["totalpayout"], null))
                    resultContext["videoslotstate"]["totalpayout"] = convertWinByBet((double)resultContext["videoslotstate"]["totalpayout"], currentBet);

                if (!object.ReferenceEquals(resultContext["videoslotstate"]["scatterpaylist"], null))
                {
                    for(int i = 0; i < resultContext["videoslotstate"]["scatterpaylist"].Count; i++)
                        resultContext["videoslotstate"]["scatterpaylist"][i]["totalpayout"] = convertWinByBet((double)resultContext["videoslotstate"]["scatterpaylist"][i]["totalpayout"], currentBet);
                }
                
                if (!object.ReferenceEquals(resultContext["videoslotstate"]["paylinelist"], null))
                {
                    for (int i = 0; i < resultContext["videoslotstate"]["paylinelist"].Count; i++)
                        resultContext["videoslotstate"]["paylinelist"][i]["totalpayout"] = convertWinByBet((double)resultContext["videoslotstate"]["paylinelist"][i]["totalpayout"], currentBet);
                }

                if (!object.ReferenceEquals(resultContext["videoslotstate"]["pickresultlist"], null))
                {
                    for (int i = 0; i < resultContext["videoslotstate"]["pickresultlist"].Count; i++)
                    {
                        if(!object.ReferenceEquals(resultContext["videoslotstate"]["pickresultlist"][i]["wincash"], null))
                            resultContext["videoslotstate"]["pickresultlist"][i]["wincash"] = convertWinByBet((double)resultContext["videoslotstate"]["pickresultlist"][i]["wincash"], currentBet);
                    }
                }
            }
        }

        protected override HabaneroLogItem buildHabaneroLogItem(BaseHabaneroSlotBetInfo betInfo, HabaneroHistoryItem history,int agentID ,string strUserID, int currency, double balance,double betMoney ,double winMoney)
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
                historyItem.Win         = winMoney;

                HabaneroLogItemOverview overView = new HabaneroLogItemOverview();
                overView.CurrencyCode       = new Currencies()._currencyInfo[currency].CurrencyText;
                overView.DateToShow         = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
                overView.DtCompleted        = string.Format("/Date({0})/",overView.DateToShow);
                overView.Exponent           = 0;
                overView.ExtRoundId         = null;
                overView.FriendlyId         = Convert.ToInt64(history.RoundId);
                overView.GameInstanceId     = history.GameId;
                overView.GameKeyName        = this.SymbolName;
                overView.GameStateId        = 3;
                overView.IsTestSite         = false;
                overView.RealPayout         = winMoney;
                overView.RealStake          = betInfo.TotalBet;
                overView.ReplayURL          = HabaneroConfig.Instance.ReplayURL + history.GameId;
                overView.BrandGameId        = BrandGameId;
                overView.IsSpecialBrandGame = false;
                overView.GameTypeId         = 11;

                historyItem.Overview = JsonConvert.SerializeObject(overView);

                JObject detailValue = new JObject();
                detailValue["GameType"]              = 11;
                detailValue["GameState"]             = "GameState_3";
                detailValue["GameKeyName"]           = SymbolName;
                detailValue["CurrencyCode"]          = new Currencies()._currencyInfo[currency].CurrencyText;
                detailValue["FriendlyId"]            = history.RoundId;
                detailValue["DtStarted"]             = overView.DateToShow - 10;
                detailValue["DtCompleted"]           = overView.DateToShow;
                detailValue["RealStake"]             = betInfo.TotalBet;
                detailValue["RealPayout"]            = winMoney;
                detailValue["BonusStake"]            = 0;
                detailValue["BonusPayout"]           = 0;
                detailValue["BonusToReal"]           = 0;
                detailValue["CurrencyExponent"]      = 2;
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
                detailResultReportInfo["totalbet"]          = betInfo.TotalBet;
                
                JArray eventArray = new JArray();
                for(int i = 0; i < history.Responses.Count; i++)
                {
                    JObject eventItem = buildEventItem(strGlobalUserID, i);
                    eventArray.Add(eventItem);
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

        protected override JObject buildEventItem(string strGlobalUserID, int currentIndex)
        {
            HabaneroHistoryResponses responses = _dicUserHistory[strGlobalUserID].Responses[currentIndex];
            JObject eventItem = new JObject();
            eventItem["type"]   = "spin";
            switch (responses.Action)
            {
                case HabaneroActionType.FREEGAME:
                    eventItem["gamemode"] = "FREEGAME";
                    break;
                case HabaneroActionType.RESPIN:
                    eventItem["gamemode"] = "RESPIN";
                    break;
                case HabaneroActionType.PICKOPTION:
                    eventItem["gamemode"] = "pick";
                    break;
                case HabaneroActionType.BONUS1:
                    eventItem["gamemode"] = "bonus1";
                    break;
                case HabaneroActionType.BONUS2:
                    eventItem["gamemode"] = "bonus2";
                    break;
                case HabaneroActionType.BONUS3:
                    eventItem["gamemode"] = "bonus3";
                    break;
                case HabaneroActionType.BONUS4:
                    eventItem["gamemode"] = "bonus4";
                    break;
                case HabaneroActionType.BONUS5:
                    eventItem["gamemode"] = "bonus5";
                    break;
                case HabaneroActionType.BONUS1RESPIN:
                    eventItem["gamemode"] = "bonus1respin";
                    break;
                case HabaneroActionType.BONUS2RESPIN:
                    eventItem["gamemode"] = "bonus2respin";
                    break;
                case HabaneroActionType.BONUS3RESPIN:
                    eventItem["gamemode"] = "bonus3respin";
                    break;
                case HabaneroActionType.BONUS4RESPIN:
                    eventItem["gamemode"] = "bonus4respin";
                    break;
                case HabaneroActionType.MAIN:
                default:
                    eventItem["gamemode"] = "MAIN";
                    break;
            }
            eventItem["dt"]     = 637957353915568900;
            eventItem["rng"]    = "1,2,3,4,5";
            dynamic response    = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            if (!object.ReferenceEquals(response["videoslotstate"]["reellist"], null) && responses.Action != HabaneroActionType.PICKOPTION)
            {
                JArray reels = buildHabaneroLogReels(strGlobalUserID, currentIndex, response);
                eventItem["reels"] = reels;
            }

            eventItem["wincash"]        = 0;
            eventItem["winfreegames"]   = 0;

            if (!object.ReferenceEquals(response["videoslotstate"]["totalpayout"], null))
                eventItem["wincash"] = response["videoslotstate"]["totalpayout"];
            if (!object.ReferenceEquals(response["videoslotstate"]["winfreegame"], null))
                eventItem["winfreegames"] = response["videoslotstate"]["winfreegame"];
            if (!object.ReferenceEquals(response["videoslotstate"]["freespinnumber"], null))
                eventItem["spinno"] = response["videoslotstate"]["freespinnumber"];

            JArray subEvents = new JArray();
            if (!object.ReferenceEquals(response["videoslotstate"]["paylinelist"], null))
            {
                for (int j = 0; j < response["videoslotstate"]["paylinelist"].Count; j++)
                {
                    JObject subEventItem = new JObject();
                    subEventItem["type"]        = "payline";
                    subEventItem["lineno"]      = response["videoslotstate"]["paylinelist"][j]["paylineindex"];
                    subEventItem["wincash"]     = response["videoslotstate"]["paylinelist"][j]["totalpayout"];
                    subEventItem["symbol"]      = response["videoslotstate"]["paylinelist"][j]["winsymbol"];
                    subEventItem["multiplier"]  = response["videoslotstate"]["paylinelist"][j]["multiplier"];
                    JArray lineWinArray = new JArray();
                    for (int k = 0; k < response["videoslotstate"]["paylinelist"][j]["winningwindows"].Count; k++)
                    {
                        JArray lineWinItem = new JArray();
                        lineWinItem.Add(response["videoslotstate"]["paylinelist"][j]["winningwindows"][k][0]);
                        lineWinItem.Add(response["videoslotstate"]["paylinelist"][j]["winningwindows"][k][1]);
                        lineWinArray.Add(lineWinItem);
                    }
                    subEventItem["windows"] = lineWinArray;
                    subEvents.Add(subEventItem);
                }
            }
            
            if (!object.ReferenceEquals(response["videoslotstate"]["scatterpaylist"], null))
            {
                for (int j = 0; j < response["videoslotstate"]["scatterpaylist"].Count; j++)
                {
                    JObject subEventItem = new JObject();
                    subEventItem["type"]        = "payline";
                    subEventItem["lineno"]      = response["videoslotstate"]["scatterpaylist"][j]["paylineindex"];
                    subEventItem["wincash"]     = response["videoslotstate"]["scatterpaylist"][j]["totalpayout"];
                    subEventItem["symbol"]      = response["videoslotstate"]["scatterpaylist"][j]["winsymbol"];
                    subEventItem["multiplier"]  = response["videoslotstate"]["scatterpaylist"][j]["multiplier"];
                    JArray lineWinArray = new JArray();
                    for (int k = 0; k < response["videoslotstate"]["scatterpaylist"][j]["winningwindows"].Count; k++)
                    {
                        JArray lineWinItem = new JArray();
                        lineWinItem.Add(response["videoslotstate"]["scatterpaylist"][j]["winningwindows"][k][0]);
                        lineWinItem.Add(response["videoslotstate"]["scatterpaylist"][j]["winningwindows"][k][1]);
                        lineWinArray.Add(lineWinItem);
                    }
                    subEventItem["windows"] = lineWinArray;
                    subEvents.Add(subEventItem);
                }
            }

            if(responses.Action == HabaneroActionType.PICKOPTION)
            {
                JObject subEventItem = new JObject();
                subEventItem["type"]            = "pick";
                subEventItem["multiplier"]      = 0;
                subEventItem["picktype"]        = "freegames";
                subEventItem["winfreegames"]    = eventItem["winfreegames"];
                subEvents.Add(subEventItem);
            }

            if (subEvents.Count > 0)
                eventItem["subevents"] = subEvents;

            return eventItem;
        }

        protected override JArray buildHabaneroLogReels(string strGlobalUserID, int currentIndex, dynamic response, bool containWild = false)
        {
            JArray reels = new JArray();
            for (int j = 0; j < response["videoslotstate"]["virtualreellist"].Count; j++)
            {
                JArray col = new JArray();
                for (int k = 2; k < response["videoslotstate"]["virtualreellist"][j].Count - 2; k++)
                {
                    int symbol = Convert.ToInt32(response["videoslotstate"]["virtualreellist"][j][k]);
                    string symbolid = SymbolIdStringForLog[symbol].id;
                    col.Add(symbolid);
                }
                reels.Add(col);
            }
            return reels;
        }

        protected override JArray buildInitResumeGame(string strUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult,string gameinstanceid, string roundid,HabaneroActionType currentAction)
        {
            JArray resumeGames = new JArray();
            JObject resumeGame = new JObject();

            resumeGame["game_instance_id"]  = gameinstanceid;
            resumeGame["friendly_id"]       = roundid;
            resumeGame["selected_lines"]    = this.ClientReqLineCount;
            resumeGame["bet_level"]         = betInfo.BetLevel;
            resumeGame["coin_denomination"] = betInfo.CoinValue;
            resumeGame["total_win_cash"]    = lastResult["totalpayout"];
            resumeGame["is_ok"]             = true;
            resumeGame["resumeobject"]      = "{}";

            JArray resumeGameStates = new JArray();
            JObject resumeGameState = new JObject();
            resumeGameState["name"]                 = "freegame";
            resumeGameState["type"]                 = "freespin";
            resumeGameState["display_symbols"]      = new JArray();
            resumeGameState["pick_results"]         = new JArray();
            resumeGameState["free_games_remaining"] = lastResult["videoslotstate"]["bonusgamecount"];
            resumeGameState["trigger_path"]         = lastResult["videoslotstate"]["triggeranticiplationreellist"];
            resumeGameState["multiplier"]           = lastResult["videoslotstate"]["gamemultiplier"];
            resumeGameState["virtualreels"]         = lastResult["videoslotstate"]["virtualreellist"];
            resumeGameStates.Add(resumeGameState);
            resumeGame["states"]            = resumeGameStates;
            resumeGames.Add(resumeGame);
            return resumeGames;
        }
    }

    public enum HabaneroLastSlotEventType
    {
        NormalSpin      = 1,
        BonusSpin       = 2,
        TriggerBonus    = 3,
        SwitchToNormal  = 5,
        PickGameDone    = 6,
    }
}
