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
    public class FiveMariachisGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SG5Mariachis";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "ad619810-2b4e-4da0-a4c7-086d102ea890";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "ee5496c330d8e2a0b73fec1cd894b21802d4843c";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.1822.143";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idMask",            name = "Mask"           } },
                    {2,   new HabaneroLogSymbolIDName{id = "idPinata",          name = "Pinata"         } },
                
                    {3,   new HabaneroLogSymbolIDName{id = "idMariachi1",       name = "Mariachi1"      } },
                    {4,   new HabaneroLogSymbolIDName{id = "idMariachi2",       name = "Mariachi2"      } },
                    {5,   new HabaneroLogSymbolIDName{id = "idMariachi3",       name = "Mariachi3"      } },
                    {6,   new HabaneroLogSymbolIDName{id = "idMariachi4",       name = "Mariachi4"      } },
                    {7,   new HabaneroLogSymbolIDName{id = "idMariachi5",       name = "Mariachi5"      } },
                
                    {8,   new HabaneroLogSymbolIDName{id = "idTequila",         name = "Tequila"        } },
                    {9,   new HabaneroLogSymbolIDName{id = "idGuitar",          name = "Guitar"         } },
                    {10,  new HabaneroLogSymbolIDName{id = "idChili",           name = "Chili"          } },
                    {11,  new HabaneroLogSymbolIDName{id = "idMaracas",         name = "Maracas"        } },
                    {12,  new HabaneroLogSymbolIDName{id = "idCactus",          name = "Cactus"         } },
                    {13,  new HabaneroLogSymbolIDName{id = "idAce",             name = "Ace"            } },
                    {14,  new HabaneroLogSymbolIDName{id = "idKing",            name = "King"           } },
                    {15,  new HabaneroLogSymbolIDName{id = "idQueen",           name = "Queen"          } },
                    {16,  new HabaneroLogSymbolIDName{id = "idJack",            name = "Jack"           } },
                    {17,  new HabaneroLogSymbolIDName{id = "idGirl",            name = "Girl"           } },

                    {103, new HabaneroLogSymbolIDName{id = "idFreeMariachi1",   name = "FreeMariachi1"  } },
                    {104, new HabaneroLogSymbolIDName{id = "idFreeMariachi2",   name = "FreeMariachi2"  } },
                    {105, new HabaneroLogSymbolIDName{id = "idFreeMariachi3",   name = "FreeMariachi3"  } },
                    {106, new HabaneroLogSymbolIDName{id = "idFreeMariachi4",   name = "FreeMariachi4"  } },
                    {107, new HabaneroLogSymbolIDName{id = "idFreeMariachi5",   name = "FreeMariachi5"  } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 369;
            }
        }
        #endregion

        public FiveMariachisGameLogic()
        {
            _gameID     = GAMEID.FiveMariachis;
            GameName    = "5Mariachis";
        }

        protected override async Task onProcMessage(string strUserID, int agentID, CurrencyEnum currency, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
            if (message.MsgCode == (ushort)CSMSG_CODE.CS_HABANERO_DOPICKGAMEDONEPAY)
            {
                await onDoSpin(strGlobalUserID, (int)currency, agentID, message, userBonus, userBalance, isMustLose);
            }
            else
            {
                await base.onProcMessage(strUserID, agentID, currency, message, userBonus, userBalance, isMustLose);
            }
        }
        protected override JObject buildEventItem(string strGlobalUserId, int currentIndex)
        {
            dynamic eventItem   = base.buildEventItem(strGlobalUserId, currentIndex);
            dynamic response    = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserId].Responses[currentIndex].Response);

            bool isPick = false;
            if (_dicUserHistory[strGlobalUserId].Responses[currentIndex].Action == HabaneroActionType.PICKOPTION)
                isPick = true;

            if (!isPick)
            {
                if (!object.ReferenceEquals(response["videoslotstate"]["animatesymbollist"], null) && response["videoslotstate"]["animatesymbollist"].Count > 0)
                {
                    JArray reelsList = buildHabaneroLogReelslist(response);
                    eventItem["reelslist"] = reelsList;
                }
            }
            else
            {
                if(!object.ReferenceEquals(eventItem["spinno"],null))
                    (eventItem as JObject).Property("spinno").Remove();
                eventItem["type"] = "pick";

                dynamic beforeResponse  = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserId].Responses[currentIndex - 1].Response);

                JArray subEvents        = new JArray();
                JObject subEventItem    = new JObject();
                subEventItem["type"]        = "pick";
                subEventItem["multiplier"]  = 0;
                subEventItem["picktype"]    = "cash";
                subEventItem["wincash"]     = beforeResponse["videoslotstate"]["pickresultlist"][0]["wincash"];
                subEvents.Add(subEventItem);
                
                eventItem["subevents"] = subEvents;
            }

            return eventItem;
        }

        protected override JArray buildHabaneroLogReels(string strGlobalUserId, int currentIndex, dynamic response, bool containWild = false)
        {
            bool isFreeGame = false;
            if (!object.ReferenceEquals(response["videoslotstate"]["freespinnumber"], null))
                isFreeGame = true;

            JArray reels = new JArray();
            for (int j = 0; j < response["videoslotstate"]["virtualreellist"].Count; j++)
            {
                JArray col = new JArray();
                for (int k = 2; k < response["videoslotstate"]["virtualreellist"][j].Count - 2; k++)
                {
                    int symbol      = Convert.ToInt32(response["videoslotstate"]["virtualreellist"][j][k]);
                    string symbolid = SymbolIdStringForLog[symbol].id;
                    
                    if (isFreeGame && symbol >= 3 && symbol <= 7)
                        symbolid = SymbolIdStringForLog[symbol + 100].id;

                    col.Add(symbolid);
                }
                reels.Add(col);
            }
            
            if (!object.ReferenceEquals(response["videoslotstate"]["animatesymbollist"], null))
            {
                for(int i = 0; i < response["videoslotstate"]["animatesymbollist"].Count; i++)
                {
                    int symbolId    = (int)response["videoslotstate"]["animatesymbollist"][i]["symbolid"];
                    int col         = (int)response["videoslotstate"]["animatesymbollist"][i]["x"];
                    int row         = (int)response["videoslotstate"]["animatesymbollist"][i]["y"];
                    
                    reels[col][row] = SymbolIdStringForLog[symbolId].id;
                }
            }
            return reels;
        }

        protected override JArray buildHabaneroLogReelslist(dynamic response)
        {
            JArray reelsList        = new JArray();
            JObject reelsListItem   = new JObject();

            bool isFreeGame = false;
            if (!object.ReferenceEquals(response["videoslotstate"]["freespinnumber"], null))
                isFreeGame = true;

            JArray reels = new JArray();
            for (int j = 0; j < response["videoslotstate"]["virtualreellist"].Count; j++)
            {
                JArray col = new JArray();
                for (int k = 2; k < response["videoslotstate"]["virtualreellist"][j].Count - 2; k++)
                {
                    int symbol = Convert.ToInt32(response["videoslotstate"]["virtualreellist"][j][k]);
                    string symbolid = SymbolIdStringForLog[symbol].id;

                    if (isFreeGame && symbol >= 3 && symbol <= 7)
                        symbolid = SymbolIdStringForLog[symbol + 100].id;

                    col.Add(symbolid);
                }
                reels.Add(col);
            }

            reelsListItem["reels"] = reels;
            reelsList.Add(reelsListItem);

            return reelsList;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserId, BaseHabaneroSlotBetInfo betInfo, JObject lastResult,string gameinstanceid, string roundid,HabaneroActionType currentAction)
        {
            JArray resumeGames = new JArray();
            JObject resumeGame = new JObject();

            bool isPick = false;
            if (!object.ReferenceEquals(lastResult["videoslotstate"]["gamemodename"], null) && (string)lastResult["videoslotstate"]["gamemodename"] == "pick")
                isPick = true;

            JArray resumeGameStates = new JArray();
            JObject resumeGameState = new JObject();
            if (!isPick)
                resumeGameState["name"] = "freegame";
            else
                resumeGameState["name"] = "pick";

            resumeGameState["type"]                 = "freespin";
            resumeGameState["display_symbols"]      = new JArray();
            JArray pickResult = new JArray();
            resumeGameState["free_games_remaining"] = lastResult["videoslotstate"]["bonusgamecount"];
            resumeGameState["trigger_path"]         = lastResult["videoslotstate"]["triggeranticiplationreellist"];
            resumeGameState["multiplier"]           = lastResult["videoslotstate"]["gamemultiplier"];

            if (!object.ReferenceEquals(lastResult["videoslotstate"]["pickresultlist"],null))
                pickResult = lastResult["videoslotstate"]["pickresultlist"] as JArray;

            if(isPick)
            {
                if (!object.ReferenceEquals(lastResult["videoslotstate"]["freespinnumber"], null))
                    resumeGameState["gamedata"] = "freegame";
                else
                    resumeGameState["gamedata"] = "main";
            }
            resumeGameState["pick_results"]         = pickResult;
            resumeGameState["virtualreels"]         = lastResult["videoslotstate"]["virtualreellist"];
            resumeGameStates.Add(resumeGameState);
            
            resumeGame["states"]            = resumeGameStates;
            resumeGame["game_instance_id"]  = gameinstanceid;
            resumeGame["friendly_id"]       = roundid;
            resumeGame["selected_lines"]    = this.ClientReqLineCount;
            resumeGame["bet_level"]         = betInfo.BetLevel;
            resumeGame["coin_denomination"] = betInfo.CoinValue;
            resumeGame["total_win_cash"]    = lastResult["totalpayout"];
            resumeGame["is_ok"]             = true;

            resumeGames.Add(resumeGame);
            return resumeGames;
        }
    }
}
