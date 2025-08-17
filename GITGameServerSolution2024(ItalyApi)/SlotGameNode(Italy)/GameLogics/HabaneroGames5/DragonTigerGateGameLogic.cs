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
    public class DragonTigerGateGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGDragonTigerGate";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "f8e50532-aead-4f02-bf4b-969b1a680f44";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "cd011a313041cc333ceb4ba5f28a40b5c8ef0a1c";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.10613.423";
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
                return 25;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,     new HabaneroLogSymbolIDName{id = "idWild",          name = "Wild"       } },
                    {2,     new HabaneroLogSymbolIDName{id = "idDragon",        name = "Dragon"     } },
                    {3,     new HabaneroLogSymbolIDName{id = "idTiger",         name = "Tiger"      } },
                    {4,     new HabaneroLogSymbolIDName{id = "idDragonClaw",    name = "DragonClaw" } },
                    {5,     new HabaneroLogSymbolIDName{id = "idTigerClaw",     name = "TigerClaw"  } },
                    {6,     new HabaneroLogSymbolIDName{id = "idHeart",         name = "Heart"      } },
                    {7,     new HabaneroLogSymbolIDName{id = "idDiamond",       name = "Diamond"    } },
                    {8,     new HabaneroLogSymbolIDName{id = "idClub",          name = "Club"       } },
                    {9,     new HabaneroLogSymbolIDName{id = "idSpade",         name = "Spade"      } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 748;
            }
        }
        #endregion

        public DragonTigerGateGameLogic()
        {
            _gameID     = GAMEID.DragonTigerGate;
            GameName    = "DragonTigerGate";
        }

        protected override HabaneroLogItem buildHabaneroLogItem(BaseHabaneroSlotBetInfo betInfo, HabaneroHistoryItem history, int agentID, string strUserID, int currency, double balance, double betMoney, double winMoney)
        {
            HabaneroLogItem logItem = base.buildHabaneroLogItem(betInfo, history, agentID, strUserID, currency, balance, betMoney, winMoney);

            string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);

            dynamic logDetail   = JsonConvert.DeserializeObject<dynamic>(logItem.Detail);
            dynamic detailValue = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(logDetail["d"]));
            dynamic reportInfo  = JsonConvert.DeserializeObject<dynamic>(Convert.ToString(detailValue["VideoSlotGameDetails"]["ReportInfo"]));

            JArray eventArray       = reportInfo["events"] as JArray;
            JArray newEventArray    = new JArray();
            for(int i = 0; i < eventArray.Count; i++)
            {
                JObject eventItem       = eventArray[i] as JObject;
                dynamic resultContext   = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserID].Responses[i].Response);
                
                if (object.ReferenceEquals(resultContext["rounds"], null) || resultContext["rounds"].Count == 1)
                {
                    newEventArray.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(eventItem)));
                    continue;
                }

                for(int j = 0; j < resultContext["rounds"].Count; j++)
                {
                    JObject roundItem = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(eventItem));
                    for(int ii = 0; ii < resultContext["rounds"][j]["displayreels"].Count; ii++)
                    {
                        for(int jj = 0; jj < resultContext["rounds"][j]["displayreels"][ii].Count; jj++)
                        {
                            int symbolid = resultContext["rounds"][j]["displayreels"][ii][jj]["symbolid"];
                            roundItem["reels"][ii][jj] = SymbolIdStringForLog[symbolid].id;
                        }
                    }

                    JArray subEvents = new JArray();
                    if (!object.ReferenceEquals(resultContext["rounds"][j]["linewins"], null))
                    {
                        for (int k = 0; k < resultContext["rounds"][j]["linewins"].Count; k++)
                        {
                            JObject subEventItem = new JObject();
                            subEventItem["type"]    = "payline";
                            subEventItem["wincash"] = resultContext["rounds"][j]["linewins"][k]["wincash"];
                            
                            int symbol          = Convert.ToInt32(resultContext["rounds"][j]["linewins"][k]["symbolid"]);
                            string symbolName   = SymbolIdStringForLog[symbol].name;
                            subEventItem["symbol"]      = symbolName;
                            
                            subEventItem["multiplier"]  = resultContext["rounds"][j]["linewins"][k]["multiplier"];
                            subEventItem["lineno"]      = resultContext["rounds"][j]["linewins"][j]["paylineindex"];
                            JArray lineWinArray = new JArray();
                            for (int kk = 0; kk < resultContext["rounds"][j]["linewins"][k]["winningwindows"].Count; kk++)
                            {
                                JArray lineWinItem = new JArray();
                                lineWinItem.Add(resultContext["rounds"][j]["linewins"][k]["winningwindows"][kk]["reelindex"]);
                                lineWinItem.Add(resultContext["rounds"][j]["linewins"][k]["winningwindows"][kk]["symbolindex"]);
                                lineWinArray.Add(lineWinItem);
                            }
                            subEventItem["windows"] = lineWinArray;
                            subEventItem["lineno"]  = resultContext["rounds"][j]["linewins"][k]["paylineindex"];

                            subEvents.Add(subEventItem);
                        }
                    }

                    if (subEvents.Count > 0)
                        roundItem["subevents"] = subEvents;

                    if (!object.ReferenceEquals(resultContext["rounds"][j]["wincash"], null))
                        roundItem["wincash"] = resultContext["rounds"][j]["wincash"];
                    if (!object.ReferenceEquals(resultContext["rounds"][j]["multiplier"], null))
                        roundItem["multiplier"] = resultContext["rounds"][j]["multiplier"];


                    newEventArray.Add(roundItem);
                }
            }
            
            reportInfo["events"] = newEventArray;
            detailValue["VideoSlotGameDetails"]["ReportInfo"] = JsonConvert.SerializeObject(reportInfo);
            logDetail["d"] = JsonConvert.SerializeObject(detailValue);
            logItem.Detail = JsonConvert.SerializeObject(logDetail);
            return logItem;
        }

        protected override void convertWinsByBet(dynamic resultContext, float currentBet)
        {
            base.convertWinsByBet(resultContext as JObject, currentBet);

            if(!object.ReferenceEquals(resultContext["rounds"], null))
            {
                for(int i = 0; i < resultContext["rounds"].Count; i++)
                {
                    if (!object.ReferenceEquals(resultContext["rounds"][i]["wincash"], null))
                        resultContext["rounds"][i]["wincash"] = convertWinByBet((double)resultContext["rounds"][i]["wincash"], currentBet);
                    if (!object.ReferenceEquals(resultContext["rounds"][i]["linewinscash"], null))
                        resultContext["rounds"][i]["linewinscash"] = convertWinByBet((double)resultContext["rounds"][i]["linewinscash"], currentBet);
                    
                    if (!object.ReferenceEquals(resultContext["rounds"][i]["linewins"], null))
                    {
                        JArray lineWinsArray = resultContext["rounds"][i]["linewins"];
                        for (int j = 0; j < lineWinsArray.Count; j++)
                        {
                            if (!object.ReferenceEquals(lineWinsArray[j]["wincash"], null))
                                lineWinsArray[j]["wincash"] = convertWinByBet((double)lineWinsArray[j]["wincash"], currentBet);
                        }
                    }
                }
            }
        }
    }
}
