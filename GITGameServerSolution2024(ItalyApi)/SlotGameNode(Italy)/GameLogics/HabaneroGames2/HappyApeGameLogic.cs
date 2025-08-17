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
    public class HappyApeGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGHappyApe";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "71c9488b-2a74-4b8d-9628-2d00601d280b";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "4fd097c9118813e837355eb99140b109ba0be3ab";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.6640.348";
            }
        }
        protected override float MiniCoin
        {
            get
            {
                return 15.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 15;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idBanana",      name = "Banana"     } },    
                    {2,   new HabaneroLogSymbolIDName{id = "idIdol",        name = "Idol"       } },    
                    {3,   new HabaneroLogSymbolIDName{id = "idCoconut",     name = "Coconut"    } },    
                    {4,   new HabaneroLogSymbolIDName{id = "idOrange",      name = "Orange"     } },    
                    {5,   new HabaneroLogSymbolIDName{id = "idPlum",        name = "Plum"       } },    
                    {6,   new HabaneroLogSymbolIDName{id = "idPear",        name = "Pear"       } },    
                    {7,   new HabaneroLogSymbolIDName{id = "idSpade",       name = "Spade"      } },    
                    {8,   new HabaneroLogSymbolIDName{id = "idHeart",       name = "Heart"      } },    
                    {9,   new HabaneroLogSymbolIDName{id = "idClub",        name = "Club"       } },    
                    {10,  new HabaneroLogSymbolIDName{id = "idDiamond",     name = "Diamond"    } },
                    {11,  new HabaneroLogSymbolIDName{id = "idKey",         name = "Key"        } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 523;
            }
        }
        #endregion

        public HappyApeGameLogic()
        {
            _gameID     = GAMEID.HappyApe;
            GameName    = "HappyApe";
        }

        protected override JObject buildEventItem(string strGlobalUserId, int currentIndex)
        {
            dynamic eventItem   = base.buildEventItem(strGlobalUserId, currentIndex);
            dynamic response    = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserId].Responses[currentIndex].Response);
            dynamic apeData     = response["videoslotstate"]["data"];
            
            if(apeData["keyposlist"].Count > 0 || apeData["btosslist"].Count > 0 || apeData["apesmashlist"].Count > 0)
            {
                JArray reelsList = new JArray();
                dynamic reels = buildHabaneroLogReelslist(response);

                if (!object.ReferenceEquals(apeData["keyposlist"],null) && apeData["keyposlist"].Count > 0)
                {
                    JObject reelListItem    = new JObject();
                    reelListItem["reels"]   = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(reels));
                    reelsList.Add(reelListItem);

                    for (int i = 0; i < apeData["keyposlist"].Count; i++)
                    {
                        int reelIndex       = apeData["keyposlist"][i][0];
                        int symbolIndex     = apeData["keyposlist"][i][1];
                        int keySymbolId     = apeData["keyposlist"][i][2];
                        reels[reelIndex][symbolIndex] = SymbolIdStringForLog[keySymbolId].id;
                    }
                }
                
                if(!object.ReferenceEquals(apeData["btosslist"],null) && apeData["btosslist"].Count > 0)
                {
                    JObject reelListItem    = new JObject();
                    reelListItem["reels"]   = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(reels));
                    reelsList.Add(reelListItem);
                    
                    for (int i = 0; i < apeData["btosslist"].Count; i++)
                    {
                        int reelIndex       = apeData["btosslist"][i][0];
                        int symbolIndex     = apeData["btosslist"][i][1];
                        int keySymbolId     = apeData["btosslist"][i][2];
                        reels[reelIndex][symbolIndex] = SymbolIdStringForLog[keySymbolId].id;
                    }
                }

                if(!object.ReferenceEquals(apeData["apesmashlist"],null) && apeData["apesmashlist"].Count > 0)
                {
                    JObject reelListItem    = new JObject();
                    reelListItem["reels"]   = reels;
                    reelsList.Add(reelListItem);
                }

                eventItem["reelslist"] = reelsList;
            }
            JArray customSubEvents      = new JArray();
            JObject customSubEventItem  = new JObject();
            string customEventData      = buildHabaneroCustomData(response);
            customSubEventItem["type"]  = "HappyApe";
            customSubEventItem["data"]  = customEventData;
            customSubEvents.Add(customSubEventItem);
            eventItem["customsubevents"] = customSubEvents;
            int multiplier              = calcWinMultiplier(response);
            eventItem["multiplier"]     = multiplier;
            return eventItem;
        }

        protected override JArray buildHabaneroLogReels(string strGlobalUserId, int currentIndex, dynamic response, bool containWild = false)
        {
            JArray reels = new JArray();
            for(int i = 0; i < response["videoslotstate"]["reellist"].Count; i++)
            {
                JArray reelsCol = new JArray();
                for (int j = 0; j < response["videoslotstate"]["reellist"][i]["symbols"]["symbol"].Count; j++)
                {
                    int symbolId = (int)response["videoslotstate"]["reellist"][i]["symbols"]["symbol"][j]["symbolid"];
                    reelsCol.Add(SymbolIdStringForLog[symbolId].id);
                }
                reels.Add(reelsCol);
            }
            return reels;
        }

        protected override JArray buildHabaneroLogReelslist(dynamic response)
        {
            JArray reels = new JArray();
            for (int j = 0; j < response["videoslotstate"]["virtualreellist"].Count; j++)
            {
                JArray col = new JArray();
                for (int k = 2; k < response["videoslotstate"]["virtualreellist"][j].Count - 2; k++)
                {
                    int symbol      = Convert.ToInt32(response["videoslotstate"]["virtualreellist"][j][k]);
                    string symbolid = SymbolIdStringForLog[symbol].id;
                    col.Add(symbolid);
                }
                reels.Add(col);
            }
            return reels;
        }

        private string buildHabaneroCustomData(dynamic response)
        {
            dynamic apeData = response["videoslotstate"]["data"];
            JObject data    = new JObject();
            JArray cp       = new JArray();

            for (int i = 0; i < 5; i++)
                cp.Add(-1);

            if (!object.ReferenceEquals(apeData["keyposlist"],null) && apeData["keyposlist"].Count > 0)
            {
                for(int i = 0; i < apeData["keyposlist"].Count; i++)
                {
                    int reelIndex       = apeData["keyposlist"][i][0];
                    int keySymbolType   = apeData["keyposlist"][i][3];
                    cp[reelIndex]       = keySymbolType;
                }
            }
            data["cp"] = cp;
            if(!object.ReferenceEquals(apeData["btosslist"],null) && apeData["btosslist"].Count > 0)
                data["bt"] = true;

            if (!object.ReferenceEquals(apeData["apesmashlist"], null) && apeData["apesmashlist"].Count > 0)
                data["as"] = true;

            return JsonConvert.SerializeObject(data);
        }

        private int calcWinMultiplier(dynamic response)
        {
            int multiplier = 1;
            dynamic apeData = response["videoslotstate"]["data"];
            
            if (!object.ReferenceEquals(apeData["keyposlist"],null) && apeData["keyposlist"].Count > 0)
            {
                for(int i = 0; i < apeData["keyposlist"].Count; i++)
                {
                    int keySymbolType   = apeData["keyposlist"][i][3];
                    if (keySymbolType == 0)
                        multiplier *= 2;
                    if (keySymbolType == 1)
                        multiplier *= 3;
                }
            }
            return multiplier;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserId, BaseHabaneroSlotBetInfo betInfo, JObject lastResult,string gameinstanceid, string roundid,HabaneroActionType currentAction)
        {
            dynamic response = lastResult as dynamic;
            JArray resumeGames = new JArray();
            JObject resumeGame = new JObject();

            resumeGame["game_instance_id"]  = gameinstanceid;
            resumeGame["friendly_id"]       = roundid;
            resumeGame["selected_lines"]    = this.ClientReqLineCount;
            resumeGame["bet_level"]         = betInfo.BetLevel;
            resumeGame["coin_denomination"] = betInfo.CoinValue;
            resumeGame["total_win_cash"]    = response["totalpayout"];
            resumeGame["is_ok"]             = true;
            resumeGame["resumeobject"]      = "{}";

            JArray resumeGameStates = new JArray();
            JObject resumeGameState = new JObject();
            resumeGameState["name"]                 = "freegame";
            resumeGameState["type"]                 = "freespin";
            resumeGameState["display_symbols"]      = new JArray();
            resumeGameState["pick_results"]         = new JArray();
            resumeGameState["currentfreegame"]      = 0;
            if(!object.ReferenceEquals(lastResult["videoslotstate"]["freespinnumber"],null))
                resumeGameState["currentfreegame"]      = response["videoslotstate"]["freespinnumber"];
            
            resumeGameState["free_games_remaining"] = response["videoslotstate"]["bonusgamecount"];
            resumeGameState["trigger_path"]         = response["videoslotstate"]["triggeranticiplationreellist"];
            resumeGameState["multiplier"]           = response["videoslotstate"]["gamemultiplier"];
            dynamic virualreels = response["videoslotstate"]["virtualreellist"];
            for(int i = 0; i < response["videoslotstate"]["reellist"].Count; i++)
            {
                for (int j = 0; j < response["videoslotstate"]["reellist"][i]["symbols"]["symbol"].Count; j++)
                    virualreels[i][j + 2] = response["videoslotstate"]["reellist"][i]["symbols"]["symbol"][j]["symbolid"];
            }
            resumeGameState["virtualreels"] = virualreels;
            resumeGameStates.Add(resumeGameState);
            resumeGame["states"]    = resumeGameStates;
            resumeGames.Add(resumeGame);
            return resumeGames;
        }
    }
}
