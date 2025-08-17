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
    public class ScruffyScallywagsGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGScruffyScallywags";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "0a6b43f7-559d-44f2-9ae0-f802945b964d";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "d16b3cb8cd9be2beacd361757b3ad9cf86d1f2b2";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.3004.231";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idAnchor",          name = "Anchor"         } },
                    {2,   new HabaneroLogSymbolIDName{id = "idRum",             name = "Rum"            } },
                    
                    {3,   new HabaneroLogSymbolIDName{id = "idCaptain",         name = "Captain"        } },
                    {4,   new HabaneroLogSymbolIDName{id = "idPrisoner",        name = "Prisoner"       } },
                    {5,   new HabaneroLogSymbolIDName{id = "idDrunkPirate",     name = "DrunkPirate"    } },
                    {6,   new HabaneroLogSymbolIDName{id = "idMonkeyPirate",    name = "MonkeyPirate"   } },
                    {7,   new HabaneroLogSymbolIDName{id = "idShip",            name = "Ship"           } },
                    {8,   new HabaneroLogSymbolIDName{id = "idTreasureChest",   name = "TreasureChest"  } },
                    {9,   new HabaneroLogSymbolIDName{id = "idCannon",          name = "Cannon"         } },
                    {10,  new HabaneroLogSymbolIDName{id = "idGuns",            name = "Guns"           } },
                    {11,  new HabaneroLogSymbolIDName{id = "idCannonBalls",     name = "CannonBalls"    } },
                    {12,  new HabaneroLogSymbolIDName{id = "idTreasureMap",     name = "TreasureMap"    } },
                    
                    {13,  new HabaneroLogSymbolIDName{id = "idKraken",          name = "Kraken"         } },
                    {14,  new HabaneroLogSymbolIDName{id = "idCompass",         name = "Compass"        } },
                    
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 342;
            }
        }
        #endregion

        public ScruffyScallywagsGameLogic()
        {
            _gameID     = GAMEID.ScruffyScallywags;
            GameName    = "ScruffyScallywags";
        }

        protected override JObject buildEventItem(string strGlobalUserID, int currentIndex)
        {
            dynamic eventItem   = base.buildEventItem(strGlobalUserID, currentIndex);
            dynamic response    = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserID].Responses[currentIndex].Response);

            bool hasKraken = false;
            for(int i = 0; i < response["videoslotstate"]["reellist"].Count; i++)
            {
                dynamic col = response["videoslotstate"]["reellist"][i]["symbols"]["symbol"];
                for(int j = 0; j < col.Count; j++)
                {
                    if (col[j]["symbolid"] == 13)
                    {
                        hasKraken = true;
                        break;
                    }
                }

                if (hasKraken)
                    break;
            }

            if(hasKraken)
            {
                dynamic reelsList       = buildHabaneroLogReelslist(response);
                eventItem["reelslist"]  = reelsList;
            }

            bool isFreeSpin = false;
            if (!object.ReferenceEquals(response["videoslotstate"]["freespinnumber"], null) && (int)response["videoslotstate"]["freespinnumber"] > 0)
                isFreeSpin = true;

            if (isFreeSpin)
                eventItem["multiplier"] = 2;

            JArray subEvents = new JArray();
            if (!object.ReferenceEquals(eventItem["subevents"], null))
                subEvents = eventItem["subevents"];

            JArray scatterWindows = new JArray();
            JArray compassWindows = new JArray();
            for (int i = 0; i < response["videoslotstate"]["reellist"].Count; i++)
            {
                dynamic col = response["videoslotstate"]["reellist"][i]["symbols"]["symbol"];
                for (int j = 0; j < col.Count; j++)
                {
                    if (isFreeSpin && (int)col[j]["symbolid"] == 2)
                    {
                        JArray scaterItem = new JArray();
                        scaterItem.Add(i);
                        scaterItem.Add(j);
                        scatterWindows.Add(scaterItem);
                    }

                    if ((int)col[j]["symbolid"] == 14)
                    {
                        JArray compassItem = new JArray();
                        compassItem.Add(i);
                        compassItem.Add(j);
                        compassWindows.Add(compassItem);
                    }
                }
            }

            JObject subEventItem = new JObject();
            subEventItem["type"]            = "payline";
            subEventItem["lineno"]          = "";
            subEventItem["wincash"]         = 0;
            subEventItem["multiplier"]      = 1;

            if(scatterWindows.Count > 0)
            {
                subEventItem["windows"]         = scatterWindows;
                subEventItem["symbol"]          = "Rum";
                subEventItem["winfreegames"]    = scatterWindows.Count;
                subEvents.Add(JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(subEventItem)));
            }

            if (compassWindows.Count > 0)
            {
                subEventItem["windows"]         = compassWindows;
                subEventItem["symbol"]          = "Compass";
                subEventItem["winfreegames"]    = (int)response["videoslotstate"]["winfreegame"] - scatterWindows.Count;
                subEvents.Add(JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(subEventItem)));
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
                    int symbol      = Convert.ToInt32(response["videoslotstate"]["virtualreellist"][j][k]);
                    string symbolid = SymbolIdStringForLog[symbol].id;
                    col.Add(symbolid);
                }
                reels.Add(col);
            }
            
            if (!object.ReferenceEquals(response["videoslotstate"]["animatesymbollist"], null))
            {
                for(int i = 0; i < response["videoslotstate"]["animatesymbollist"].Count; i++)
                {
                    int col         = (int)response["videoslotstate"]["animatesymbollist"][i]["x"];
                    int row         = (int)response["videoslotstate"]["animatesymbollist"][i]["y"];
                    string type     = (string)response["videoslotstate"]["animatesymbollist"][i]["type"];
                    if(type == "kraken")
                        reels[col][row] = SymbolIdStringForLog[13].id;
                }
            }
            return reels;
        }

        protected override JArray buildHabaneroLogReelslist(dynamic response)
        {
            JArray reelsList        = new JArray();
            JObject reelsListItem   = new JObject();

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

            reelsListItem["reels"] = reels;
            reelsList.Add(reelsListItem);

            return reelsList;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult,string gameinstanceid, string roundid,HabaneroActionType currentAction)
        {
            JArray resumeGames = new JArray();
            JObject resumeGame = new JObject();

            JArray resumeGameStates = new JArray();
            JObject resumeGameState = new JObject();
            
            resumeGameState["type"]                 = "freespin";
            resumeGameState["name"]                 = lastResult["videoslotstate"]["gamemodename"];
            resumeGameState["pick_results"]         = new JArray();
            resumeGameState["display_symbols"]      = new JArray();
            resumeGameState["currentfreegame"]      = lastResult["videoslotstate"]["freespinnumber"];
            resumeGameState["free_games_remaining"] = (int)lastResult["videoslotstate"]["bonusgamecount"] + (int)lastResult["videoslotstate"]["freespinnumber"];
            resumeGameState["trigger_path"]         = lastResult["videoslotstate"]["triggeranticiplationreellist"];
            resumeGameState["multiplier"]           = lastResult["videoslotstate"]["gamemultiplier"];
            resumeGameState["virtualreels"]         = lastResult["videoslotstate"]["virtualreellist"];
            resumeGameStates.Add(resumeGameState);
            
            resumeGame["states"]                = resumeGameStates;
            resumeGame["game_instance_id"]      = gameinstanceid;
            resumeGame["friendly_id"]           = roundid;
            resumeGame["selected_lines"]        = this.ClientReqLineCount;
            resumeGame["bet_level"]             = betInfo.BetLevel;
            resumeGame["coin_denomination"]     = betInfo.CoinValue;
            resumeGame["total_win_cash"]        = lastResult["totalpayout"];
            resumeGame["is_ok"]                 = true;

            resumeGames.Add(resumeGame);
            return resumeGames;
        }
    }
}
