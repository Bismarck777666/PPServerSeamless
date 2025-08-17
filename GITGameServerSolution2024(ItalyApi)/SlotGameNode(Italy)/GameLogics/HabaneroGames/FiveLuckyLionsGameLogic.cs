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
    public class FiveLuckyLionsGameLogic : BaseSelFreeHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SG5LuckyLions";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "e7c9cb37-5da5-49da-af66-498e08e46b1d";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "90dd32b2faf32d9d0a27bdf6cb6ccda6cc9aa05f";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.3539.273";
            }
        }
        protected override int[] CoinsIncrement
        {
            get
            {
                return new int[] { 1, 2, 5, 7, 10 };
            }
        }
        protected override double[] StakeIncrement
        {
            get
            {
                return new double[] { 0.01, 0.02, 0.05, 0.10, 0.25, 0.50, 1.0, 1.25 };
            }
        }
        protected override float MiniCoin
        {
            get
            {
                return 88.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 88;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idMan",         name = "Man"            } },    //와일드
                    {2,   new HabaneroLogSymbolIDName{id = "idDrum",        name = "Drum"           } },    //스캐터

                    {3,   new HabaneroLogSymbolIDName{id = "idRedLion",     name = "RedLion"        } },    //빨강
                    {4,   new HabaneroLogSymbolIDName{id = "idPinkLion",    name = "PinkLion"       } },    //핑크
                    {5,   new HabaneroLogSymbolIDName{id = "idYellowLion",  name = "YellowLion"     } },    //노랑
                    {6,   new HabaneroLogSymbolIDName{id = "idVioletLion",  name = "VioletLion"     } },    //보라
                    {7,   new HabaneroLogSymbolIDName{id = "idGreenLion",   name = "GreenLion"      } },    //녹색
                    {8,   new HabaneroLogSymbolIDName{id = "idAce",         name = "Ace"            } },    //A
                    {9,   new HabaneroLogSymbolIDName{id = "idKing",        name = "King"           } },    //K
                    {10,  new HabaneroLogSymbolIDName{id = "idQueen",       name = "Queen"          } },    //Q
                    {11,  new HabaneroLogSymbolIDName{id = "idJack",        name = "Jack"           } },    //J
                    {12,  new HabaneroLogSymbolIDName{id = "idTen",         name = "Ten"            } },    //10
                    {13,  new HabaneroLogSymbolIDName{id = "idNine",        name = "Nine"           } },    //9
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 440;
            }
        }
        protected override int FreeSpinTypeCount
        {
            get { return 5; }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            return new int[] { 200, 201, 202, 203, 204 };
        }

        #endregion

        public FiveLuckyLionsGameLogic()
        {
            _gameID     = GAMEID.FiveLuckyLions;
            GameName    = "FiveLuckyLions";
        }

        protected override JArray buildHabaneroLogReels(string strGlobalUserId, int currentIndex, dynamic response, bool containWild = false)
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
            if (!object.ReferenceEquals(response["videoslotstate"]["explodingsymbollist"], null))
            {
                for(int i = 0; i < response["videoslotstate"]["explodingsymbollist"].Count; i++)
                {
                    string  directionStr = response["videoslotstate"]["explodingsymbollist"][i]["data"];
                    int col         = Convert.ToInt32(response["videoslotstate"]["explodingsymbollist"][i]["position"][0]);
                    int row         = Convert.ToInt32(response["videoslotstate"]["explodingsymbollist"][i]["position"][1]);
                    int symbolId    = Convert.ToInt32(response["videoslotstate"]["explodingsymbollist"][i]["symbolid"]);
                    for (int j = 0; j < directionStr.Length; j++)
                    {
                        switch (directionStr[j])
                        {
                            case 'n':
                                reels[col][row - 1] = SymbolIdStringForLog[13].id;
                                break;
                            case 's':
                                reels[col][row + 1] = SymbolIdStringForLog[13].id;
                                break;
                            case 'e':
                                reels[col + 1][row] = SymbolIdStringForLog[13].id;
                                break;
                            case 'w':
                                reels[col - 1][row] = SymbolIdStringForLog[13].id;
                                break;
                        }
                    }
                }
            }
            return reels;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserId, BaseHabaneroSlotBetInfo betInfo, JObject lastResult,string gameinstanceid, string roundid,HabaneroActionType currentAction)
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
            resumeGameStates.Add(resumeGameState);
            resumeGame["states"]            = resumeGameStates;
            resumeGames.Add(resumeGame);
            return resumeGames;
        }

        protected override JObject buildEventItem(string strGlobalUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strGlobalUserId, currentIndex);
            HabaneroHistoryResponses responses = _dicUserHistory[strGlobalUserId].Responses[currentIndex];
            dynamic response        = JsonConvert.DeserializeObject<dynamic>(responses.Response);
            dynamic videoSlotState  = response["videoslotstate"];

            JArray customSubEvents = new JArray();
            if (responses.Action == HabaneroActionType.PICKOPTION)
            {
                JObject customSubEventItem = new JObject();
                customSubEventItem["type"] = "5LLPick";
                customSubEventItem["data"] = ((string)videoSlotState["gamemodename"]).Substring(5, 1);
                customSubEvents.Add(customSubEventItem);
            }
            else if(!(object.ReferenceEquals(videoSlotState["freespinnumber"],null)) && (int)videoSlotState["freespinnumber"] > 0)
            {
                if(!object.ReferenceEquals(videoSlotState["animatesymbollist"],null) && videoSlotState["animatesymbollist"].Count > 0)
                {
                    for(int i = 0; i < videoSlotState["animatesymbollist"].Count; i++)
                    {
                        JObject customSubEventItem = new JObject();
                        
                        JObject animationSymbol = new JObject();
                        animationSymbol["si"]   = (int)responses.Action - 15;//(보너스1:18~보너스5:22)
                        animationSymbol["ri"]   = (int)videoSlotState["animatesymbollist"][i]["y"] + 1;
                        
                        customSubEventItem["type"] = "5LL";
                        customSubEventItem["data"] = JsonConvert.SerializeObject(animationSymbol);
                        customSubEvents.Add(customSubEventItem);
                    }
                }
            }
            if (customSubEvents.Count > 0)
                eventItem["customsubevents"] = customSubEvents;

            return eventItem;
        }
    }
}
