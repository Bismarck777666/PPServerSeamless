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
    public class NuwaGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGNuwa";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "ef0a66d8-9144-4f6e-84cc-44e99fae6491";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "df1ba8e293e520af8b3188ab785f526a4d7aef40";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.4132.293";
            }
        }
        protected override float MiniCoin
        {
            get
            {
                return 28.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 28;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idNuwa",            name = "Nuwa"           } },    
                    {2,   new HabaneroLogSymbolIDName{id = "idSnake",           name = "Snake"          } },    
                    {3,   new HabaneroLogSymbolIDName{id = "idPlumBlossom",     name = "PlumBlossom"    } },    
                    {4,   new HabaneroLogSymbolIDName{id = "idOrchid",          name = "Orchid"         } },    
                    {5,   new HabaneroLogSymbolIDName{id = "idBamboo",          name = "Bamboo"         } },    
                    {6,   new HabaneroLogSymbolIDName{id = "idChrysanthemum",   name = "Chrysanthemum"  } },    

                    {7,   new HabaneroLogSymbolIDName{id = "idWaterElement",    name = "WaterElement"   } },
                    {8,   new HabaneroLogSymbolIDName{id = "idFireElement",     name = "FireElement"    } },
                    {9,   new HabaneroLogSymbolIDName{id = "idEarthElement",    name = "EarthElement"   } },
                    {10,  new HabaneroLogSymbolIDName{id = "idWindElement",     name = "WindElement"    } },
                    {11,  new HabaneroLogSymbolIDName{id = "idThunderElement",  name = "ThunderElement" } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 461;
            }
        }
        #endregion

        public NuwaGameLogic()
        {
            _gameID     = GAMEID.Nuwa;
            GameName    = "Nuwa";
        }

        protected override JObject buildEventItem(string strGlobalUserId, int currentIndex)
        {
            dynamic response    = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserId].Responses[currentIndex].Response);
            dynamic eventItem   = base.buildEventItem(strGlobalUserId, currentIndex);
            if (!object.ReferenceEquals(response["videoslotstate"]["expandingwildlist"], null) && response["videoslotstate"]["expandingwildlist"].Count > 0)
            {
                JArray reelsList = buildHabaneroLogReelslist(response);
                eventItem["reelslist"] = reelsList;
            }

            HabaneroActionType currentAction = _dicUserHistory[strGlobalUserId].Responses[currentIndex].Action;
            if(currentAction == HabaneroActionType.FREEGAME0)
                eventItem["multiplier"] = 2;
            
            if (currentAction == HabaneroActionType.FREEGAME0 || currentAction == HabaneroActionType.FREEGAME)
            {
                bool[] elements = new bool[5] { false, false, false, false, false };

                JObject customData      = new JObject();
                JArray customDataLst    = getNuwaGameData(strGlobalUserId,currentIndex);
                customData["lst"]       = customDataLst;

                JObject customsubeventItem = new JObject();
                customsubeventItem["type"] = "NuwaCollect";
                customsubeventItem["data"] = JsonConvert.SerializeObject(customData);
                
                JArray customSubEvent = new JArray();
                customSubEvent.Add(customsubeventItem);
                eventItem["customsubevents"] = customSubEvent;
            }

            return eventItem;
        }

        protected override JArray buildHabaneroLogReels(string strGlobalUserId, int currentIndex, dynamic response, bool containWild = false)
        {
            dynamic reels = base.buildHabaneroLogReels(strGlobalUserId, currentIndex, response as JObject, containWild);
            if(!object.ReferenceEquals(response["videoslotstate"]["expandingwildlist"],null) && response["videoslotstate"]["expandingwildlist"].Count > 0)
            {
                for(int i = 0; i < response["videoslotstate"]["expandingwildlist"].Count; i++)
                {
                    int reelIndex   = (int)response["videoslotstate"]["expandingwildlist"][i]["reelindex"];
                    int symbolId    = (int)response["videoslotstate"]["expandingwildlist"][i]["symbolid"];
                    for (int j = 0; j < 3; j++)
                        reels[reelIndex][j] = SymbolIdStringForLog[symbolId].id;
                }
            }
            return reels;
        }

        protected override JArray buildHabaneroLogReelslist(dynamic response)
        {
            JArray reelsList        = new JArray();
            JObject reelsListItem   = new JObject();
            JArray reels            = new JArray();
            for (int i = 0; i < response["videoslotstate"]["virtualreellist"].Count; i++)
            {
                JArray col = new JArray();
                for(int j = 2; j < response["videoslotstate"]["virtualreellist"][i].Count - 2; j++)
                {
                    int symbolid = response["videoslotstate"]["virtualreellist"][i][j];
                    col.Add(SymbolIdStringForLog[symbolid].id);
                }
                reels.Add(col);
            }
            reelsListItem["reels"] = reels;
            reelsList.Add(reelsListItem);
            return reelsList;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserId, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
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
            dynamic resumeGameState = new JObject();
            resumeGameState["name"]                 = convertActionToString(currentAction);
            resumeGameState["type"]                 = "freespin";
            resumeGameState["display_symbols"]      = new JArray();
            resumeGameState["pick_results"]         = new JArray();
            resumeGameState["free_games_remaining"] = lastResult["videoslotstate"]["bonusgamecount"];
            resumeGameState["trigger_path"]         = lastResult["videoslotstate"]["triggeranticiplationreellist"];
            resumeGameState["multiplier"]           = lastResult["videoslotstate"]["gamemultiplier"];
            resumeGameState["virtualreels"]         = lastResult["videoslotstate"]["virtualreellist"];

            if (!object.ReferenceEquals(lastResult["videoslotstate"]["expandingwildlist"], null) && (lastResult["videoslotstate"]["expandingwildlist"] as dynamic).Count > 0)
            {
                dynamic expandingWildList = lastResult["videoslotstate"]["expandingwildlist"] as dynamic;
                for (int i = 0; i < expandingWildList.Count; i++)
                {
                    int reelIndex = (int)expandingWildList[i]["reelindex"];
                    resumeGameState["virtualreels"][reelIndex][2] = resumeGameState["virtualreels"][reelIndex][3] = resumeGameState["virtualreels"][reelIndex][4] = 1;
                }
            }

            if (currentAction != HabaneroActionType.FREEGAME)
                resumeGameState["currentfreegame"] = lastResult["videoslotstate"]["freespinnumber"];
            else
                resumeGameState["currentfreegame"] = 0;

            resumeGameState["gamedata"] = JsonConvert.SerializeObject(getNuwaGameData(strGlobalUserId, _dicUserHistory[strGlobalUserId].Responses.Count - 1));

            resumeGameStates.Add(resumeGameState);
            resumeGame["states"]            = resumeGameStates;
            resumeGames.Add(resumeGame);
            return resumeGames;
        }
    
        private JArray getNuwaGameData(string strGlobalUserId,int endIndex)
        {
            JArray nuwaGameData = new JArray() { false, false, false, false, false };

            for (int i = 0; i < endIndex; i++)
            {
                dynamic stepResponse = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserId].Responses[i + 1].Response);
                if (!object.ReferenceEquals(stepResponse["videoslotstate"]["animatesymbollist"], null) && stepResponse["videoslotstate"]["animatesymbollist"].Count > 0)
                {
                    for (int j = 0; j < stepResponse["videoslotstate"]["animatesymbollist"].Count; j++)
                    {
                        int symbolId = (int)stepResponse["videoslotstate"]["animatesymbollist"][j]["symbolid"];
                        switch (symbolId)
                        {
                            case 7:
                                nuwaGameData[1] = true;
                                break;
                            case 8:
                                nuwaGameData[2] = true;
                                break;
                            case 9:
                                nuwaGameData[0] = true;
                                break;
                            case 10:
                                nuwaGameData[3] = true;
                                break;
                            case 11:
                                nuwaGameData[4] = true;
                                break;
                        }
                    }
                }
            }
            return nuwaGameData;
        }
    }
}
