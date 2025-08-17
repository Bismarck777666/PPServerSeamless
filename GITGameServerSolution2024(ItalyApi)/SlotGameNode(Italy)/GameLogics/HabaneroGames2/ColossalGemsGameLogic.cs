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
    public class ColossalGemsGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGColossalGems";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "ca1db2e5-216f-404a-b1d8-70c1ce893c2f";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "cb627c109cecb9d9f85089398d9d396c972b74c6";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.4222.307";
            }
        }
        protected override float MiniCoin
        {
            get
            {
                return 30.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 30;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idWild",    name = "Wild"   } },    
                    {2,   new HabaneroLogSymbolIDName{id = "idGem1",    name = "Gem1"   } },    
                    {3,   new HabaneroLogSymbolIDName{id = "idGem2",    name = "Gem2"   } },    
                    {4,   new HabaneroLogSymbolIDName{id = "idGem3",    name = "Gem3"   } },    
                    {5,   new HabaneroLogSymbolIDName{id = "idGem4",    name = "Gem4"   } },    
                    {6,   new HabaneroLogSymbolIDName{id = "idGem5",    name = "Gem5"   } },    
                    {7,   new HabaneroLogSymbolIDName{id = "idGem6",    name = "Gem6"   } },    
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 465;
            }
        }
        #endregion

        public ColossalGemsGameLogic()
        {
            _gameID     = GAMEID.ColossalGems;
            GameName    = "ColossalGems";
        }

        protected override JObject buildEventItem(string strGlobalUserId, int currentIndex)
        {
            dynamic response = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserId].Responses[currentIndex].Response);
            dynamic eventItem = base.buildEventItem(strGlobalUserId, currentIndex);
            if(!object.ReferenceEquals(response["videoslotstate"]["animatesymbollist"], null) && response["videoslotstate"]["animatesymbollist"].Count > 0)
            {
                JObject metaItem    = new JObject();
                JArray metaList     = new JArray();
                for(int i = 0; i < response["videoslotstate"]["animatesymbollist"].Count; i++)
                {
                    dynamic expandItem = response["videoslotstate"]["animatesymbollist"][i];
                    int reelIndex   = (int)expandItem["x"];
                    int symbolIndex = (int)expandItem["y"];
                    int symbolId    = (int)expandItem["symbolid"];
                    string type     = (string)expandItem["type"];
                    
                    JObject metaListItem = new JObject();
                    metaListItem["x"] = reelIndex;
                    metaListItem["y"] = symbolIndex;
                    metaListItem["s"] = type.Substring(0, 1);
                    metaList.Add(metaListItem);
                }
                metaItem["l"]       = metaList;
                eventItem["meta"]   = metaItem;
            }
            return eventItem;
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
            JObject resumeGameState = new JObject();
            resumeGameState["name"]                 = convertActionToString(currentAction);
            resumeGameState["type"]                 = "freespin";
            resumeGameState["display_symbols"]      = new JArray();
            resumeGameState["pick_results"]         = new JArray();
            resumeGameState["free_games_remaining"] = lastResult["videoslotstate"]["bonusgamecount"];
            resumeGameState["trigger_path"]         = lastResult["videoslotstate"]["triggeranticiplationreellist"];
            resumeGameState["multiplier"]           = lastResult["videoslotstate"]["gamemultiplier"];
            resumeGameState["virtualreels"]         = lastResult["videoslotstate"]["virtualreellist"];
            
            if (currentAction != HabaneroActionType.FREEGAME)
                resumeGameState["currentfreegame"] = lastResult["videoslotstate"]["freespinnumber"];
            else
                resumeGameState["currentfreegame"] = 0;

            if (!object.ReferenceEquals(lastResult["videoslotstate"]["animatesymbollist"], null))
                resumeGameState["gamedata"] = JsonConvert.SerializeObject(lastResult["videoslotstate"]["animatesymbollist"]);
            else
                resumeGameState["gamedata"] = "[]";

            resumeGameStates.Add(resumeGameState);
            resumeGame["states"]            = resumeGameStates;
            resumeGames.Add(resumeGame);
            return resumeGames;
        }
    }
}
