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
    public class WickedWitchGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGWickedWitch";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "f8e5ebb1-f304-42a3-8d8b-af5d4afde3a4";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "fbb6fbd4558dd881dbe25c98762bc8453eacff2c";
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
                return 20.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 20;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idWitch",       name = "Witch"      } },
                    {2,   new HabaneroLogSymbolIDName{id = "idCouldron",    name = "Couldron"   } },
                    {3,   new HabaneroLogSymbolIDName{id = "idBlackCat",    name = "BlackCat"   } },
                    {4,   new HabaneroLogSymbolIDName{id = "idFireplace",   name = "Fireplace"  } },
                    {5,   new HabaneroLogSymbolIDName{id = "idBroom",       name = "Broom"      } },
                    {6,   new HabaneroLogSymbolIDName{id = "idBook",        name = "Book"       } },
                    {7,   new HabaneroLogSymbolIDName{id = "idAce",         name = "Ace"        } },
                    {8,   new HabaneroLogSymbolIDName{id = "idKing",        name = "King"       } },
                    {9,   new HabaneroLogSymbolIDName{id = "idQueen",       name = "Queen"      } },
                    {10,  new HabaneroLogSymbolIDName{id = "idJack",        name = "Jack"       } },
                    {11,  new HabaneroLogSymbolIDName{id = "idTen",         name = "Ten"        } },
                    {12,  new HabaneroLogSymbolIDName{id = "idNine",        name = "Nine"       } },

                    {13,  new HabaneroLogSymbolIDName{id = "idEyeBall",     name = "EyeBall"    } },
                    {14,  new HabaneroLogSymbolIDName{id = "idFrog",        name = "Frog"       } },
                    {15,  new HabaneroLogSymbolIDName{id = "idPoison",      name = "Poison"     } },
                    {16,  new HabaneroLogSymbolIDName{id = "idMushroom",    name = "Mushroom"   } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 274;
            }
        }
        #endregion

        public WickedWitchGameLogic()
        {
            _gameID     = GAMEID.WickedWitch;
            GameName    = "WickedWitch";
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);

            dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strUserId].Responses[currentIndex].Response);

            if (!object.ReferenceEquals(resultContext["currentfreegame"], null))
            {
                eventItem["multiplier"] = 1;

                JArray customSubEvents = new JArray();
                JObject customSubEventItem = new JObject();
                customSubEventItem["type"]          = "WickedWitchSymbolCounts";
                customSubEventItem["WW_numEyeBall"]     = resultContext["WW_ingredientCount0"];
                customSubEventItem["WW_numFrog"]        = resultContext["WW_ingredientCount1"];
                customSubEventItem["WW_numShrub"]       = resultContext["WW_ingredientCount2"];
                customSubEventItem["WW_numMushroom"]    = resultContext["WW_ingredientCount3"];
                customSubEvents.Add(customSubEventItem);
                eventItem["customsubevents"] = customSubEvents;
            }

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

            return eventItem;
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
                
                if (object.ReferenceEquals(resultContext["WW_isCollectingIngredients"], null))
                {
                    newEventArray.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(eventItem)));
                    continue;
                }

                int WW_ingredientType           = Convert.ToInt32(resultContext["WW_ingredientType"]);
                int WW_ingredientReelIndex      = Convert.ToInt32(resultContext["WW_ingredientReelIndex"]);
                int WW_ingredientSymbolIndex    = Convert.ToInt32(resultContext["WW_ingredientSymbolIndex"]);

                JObject sceneAItem  = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(eventItem));
                sceneAItem["sceneno"]       = "A";
                if(Convert.ToInt32(sceneAItem["winfreegames"]) > 0)
                {
                    sceneAItem["customsubevents"][0]["WW_numEyeBall"]   = Convert.ToInt32(sceneAItem["customsubevents"][0]["WW_numEyeBall"]) + 1;
                    sceneAItem["customsubevents"][0]["WW_numFrog"]      = Convert.ToInt32(sceneAItem["customsubevents"][0]["WW_numFrog"]) + 1;
                    sceneAItem["customsubevents"][0]["WW_numShrub"]     = Convert.ToInt32(sceneAItem["customsubevents"][0]["WW_numShrub"]) + 1;
                    sceneAItem["customsubevents"][0]["WW_numMushroom"]  = Convert.ToInt32(sceneAItem["customsubevents"][0]["WW_numMushroom"]) + 1;
                }
                sceneAItem["winfreegames"] = 0;
                sceneAItem["subevents"]     = new JArray();
                sceneAItem["wincash"] = 0;

                sceneAItem["reels"][WW_ingredientReelIndex][WW_ingredientSymbolIndex] = SymbolIdStringForLog[13 + WW_ingredientType].id;
                
                JObject sceneBItem  = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(eventItem));
                sceneBItem["sceneno"] = "B";

                newEventArray.Add(sceneAItem);
                newEventArray.Add(sceneBItem);
            }
            
            reportInfo["events"] = newEventArray;
            detailValue["VideoSlotGameDetails"]["ReportInfo"] = JsonConvert.SerializeObject(reportInfo);
            logDetail["d"] = JsonConvert.SerializeObject(detailValue);
            logItem.Detail = JsonConvert.SerializeObject(logDetail);
            return logItem;
        }

        protected override JArray buildInitResumeGame(string strUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            JArray resumeGames = base.buildInitResumeGame(strUserID, betInfo, lastResult, gameinstanceid, roundid, currentAction);

            if(!object.ReferenceEquals(lastResult["currentfreegame"], null))
            {
                resumeGames[0]["WW_ingredientCount0"]           = lastResult["WW_ingredientCount0"];
                resumeGames[0]["WW_ingredientCount1"]           = lastResult["WW_ingredientCount1"];
                resumeGames[0]["WW_ingredientCount2"]           = lastResult["WW_ingredientCount2"];
                resumeGames[0]["WW_ingredientCount3"]           = lastResult["WW_ingredientCount3"];
                resumeGames[0]["WW_isCollectingIngredients"]    = lastResult["WW_isCollectingIngredients"];
            }
            else
            {
                resumeGames[0]["WW_ingredientCount0"]           = 0;
                resumeGames[0]["WW_ingredientCount1"]           = 0;
                resumeGames[0]["WW_ingredientCount2"]           = 0;
                resumeGames[0]["WW_ingredientCount3"]           = 0;
                resumeGames[0]["WW_isCollectingIngredients"]    = true;
            }
            return resumeGames;
        }
    }
}
