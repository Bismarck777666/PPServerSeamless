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
    public class FireRoosterGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGFireRooster";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "e455de81-b082-4dda-bfb0-bbb55d7fa451";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "22908e1f29b118bda0bdcd36c1f6db16c78caed2";
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
                return 30.0f;
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
                    {1,   new HabaneroLogSymbolIDName{id = "idRooster",     name = "Rooster"        } },    //수탉
                    {2,   new HabaneroLogSymbolIDName{id = "idCrown",       name = "Crown"          } },    //스캐터

                    {3,   new HabaneroLogSymbolIDName{id = "idDragon",      name = "Dragon"         } },    //드라곤
                    {4,   new HabaneroLogSymbolIDName{id = "idCentipede",   name = "Centipede"      } },    //지네
                    {5,   new HabaneroLogSymbolIDName{id = "idBlueGem",     name = "BlueGem"        } },    //청보석
                    {6,   new HabaneroLogSymbolIDName{id = "idPurpleGem",   name = "PurpleGem"      } },    //홍보석
                    {7,   new HabaneroLogSymbolIDName{id = "idGreenGem",    name = "GreenGem"       } },    //록보석
                    {8,   new HabaneroLogSymbolIDName{id = "idAce",         name = "Ace"            } },    //A
                    {9,   new HabaneroLogSymbolIDName{id = "idKing",        name = "King"           } },    //K
                    {10,  new HabaneroLogSymbolIDName{id = "idQueen",       name = "Queen"          } },    //Q
                    {11,  new HabaneroLogSymbolIDName{id = "idJack",        name = "Jack"           } },    //J
                    {12,  new HabaneroLogSymbolIDName{id = "idTen",         name = "Ten"            } },    //10
                    {13,  new HabaneroLogSymbolIDName{id = "idFireRooster", name = "FireRooster"    } },    //FireRooster
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 345;
            }
        }
        #endregion

        public FireRoosterGameLogic()
        {
            _gameID     = GAMEID.FireRooster;
            GameName    = "FireRooster";
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
}
