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
    public class FourDivineBeastsGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGFourDivineBeasts";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "33cfbf2e-4319-4fdd-a213-db03a3fd2846";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "8049efb9f6af3ab11284a6fd1a38bca738ae6323";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.2606.233";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idWild",                name = "Wild"               } },    //와일드
                    {2,   new HabaneroLogSymbolIDName{id = "idYinYan",              name = "YinYan"             } },    //스캐터

                    {3,   new HabaneroLogSymbolIDName{id = "idAzureDragon_1",       name = "AzureDragon_1"      } },    //청룡1
                    {4,   new HabaneroLogSymbolIDName{id = "idAzureDragon_2",       name = "AzureDragon_2"      } },    //청룡2
                    {5,   new HabaneroLogSymbolIDName{id = "idVermillionBird_1",    name = "VermillionBird_1"   } },    //주작1
                    {6,   new HabaneroLogSymbolIDName{id = "idVermillionBird_2",    name = "VermillionBird_2"   } },    //주작2
                    {7,   new HabaneroLogSymbolIDName{id = "idWhiteTiger_1",        name = "WhiteTiger_1"       } },    //백호1
                    {8,   new HabaneroLogSymbolIDName{id = "idWhiteTiger_2",        name = "WhiteTiger_2"       } },    //백호2
                    {9,   new HabaneroLogSymbolIDName{id = "idBlackTortoise_1",     name = "BlackTortoise_1"    } },    //현무1
                    {10,  new HabaneroLogSymbolIDName{id = "idBlackTortoise_2",     name = "BlackTortoise_2"    } },    //현무2
                    
                    {11,  new HabaneroLogSymbolIDName{id = "idAce",                 name = "Ace"                } },    //A
                    {12,  new HabaneroLogSymbolIDName{id = "idKing",                name = "King"               } },    //K
                    {13,  new HabaneroLogSymbolIDName{id = "idQueen",               name = "Queen"              } },    //Q
                    {14,  new HabaneroLogSymbolIDName{id = "idJack",                name = "Jack"               } },    //J

                    {15,  new HabaneroLogSymbolIDName{id = "idTree",                name = "Tree"               } },    //나무 와일드
                    {16,  new HabaneroLogSymbolIDName{id = "idRovingWild",          name = "RovingWild"         } },    //러빙 와일드
                    {17,  new HabaneroLogSymbolIDName{id = "idWaterWild",           name = "WaterWild"          } },    //물 와일드
                    {18,  new HabaneroLogSymbolIDName{id = "idFireWild",            name = "FireWild"           } },    //불 와일드
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 372;
            }
        }
        #endregion

        public FourDivineBeastsGameLogic()
        {
            _gameID     = GAMEID.FourDivineBeasts;
            GameName    = "FourDivineBeasts";
        }

        protected override JObject buildEventItem(string strGlobalUserId, int currentIndex)
        {
            HabaneroHistoryResponses responses = _dicUserHistory[strGlobalUserId].Responses[currentIndex];
            dynamic response = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            JObject eventItem   = base.buildEventItem(strGlobalUserId, currentIndex);
            
            int wildSymbol = 0;
            if (!object.ReferenceEquals(response["videoslotstate"]["animatesymbollist"], null))
            {
                for (int i = 0; i < response["videoslotstate"]["animatesymbollist"].Count; i++)
                {
                    wildSymbol = (int)response["videoslotstate"]["animatesymbollist"][i]["symbolid"];
                    if(wildSymbol != 0)
                        break;
                }
            }

            if (!object.ReferenceEquals(response["videoslotstate"]["expandingwildlist"], null))
            {
                for(int i = 0; i < response["videoslotstate"]["expandingwildlist"].Count; i++)
                {
                    wildSymbol = (int)response["videoslotstate"]["expandingwildlist"][i]["symbolid"];
                    if (wildSymbol != 0)
                        break;
                }
            }

            JArray reels = buildHabaneroLogReels(strGlobalUserId, currentIndex, response);
            
            //러빙,파이어,워터 일때
            if (wildSymbol == 16 || wildSymbol == 18 || wildSymbol == 17)
            {
                JArray  reelsList       = new JArray();
                JObject reelsListItem   = new JObject();
                JArray  reelsListReel   = buildHabaneroLogReelslist(response as JObject);

                reelsListItem["reels"]  = reelsListReel;
                reelsList.Add(reelsListItem);
                eventItem["reelslist"]  = reelsList;

                addWildToReels(reels,wildSymbol ,response["videoslotstate"]["animatesymbollist"] as JArray);
            }
            //나무
            else if(wildSymbol == 15)
            {
                JArray  reelsList       = new JArray();
                JObject reelsListItem   = new JObject();
                JArray  reelsListReel   = buildHabaneroLogReelslist(response as JObject);

                reelsListItem["reels"]  = reelsListReel;
                reelsList.Add(reelsListItem);
                eventItem["reelslist"]  = reelsList;

                addWildToReels(reels,wildSymbol ,response["videoslotstate"]["expandingwildlist"] as JArray);
            }

            eventItem["reels"] = reels;
            return eventItem;
        }
        
        private void addWildToReels(JArray reels,int wildSymbol, JArray changeLists)
        {
            if(wildSymbol != 15)
            {
                for (int i = 0; i < changeLists.Count; i++)
                {
                    if ((int)changeLists[i]["symbolid"] != wildSymbol)
                        continue;

                    int col = (int)changeLists[i]["x"];
                    int row = (int)changeLists[i]["y"];
                    reels[col][row] = SymbolIdStringForLog[wildSymbol].id;
                }
            }
            else
            {
                for (int i = 0; i < changeLists.Count; i++)
                {
                    if ((int)changeLists[i]["symbolid"] != wildSymbol)
                        continue;

                    int col = (int)changeLists[i]["reelindex"];
                    for(int j = 0; j < 3; j++)
                        reels[col][j] = SymbolIdStringForLog[wildSymbol].id;
                }
            }
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
            resumeGameState["currentfreegame"]      = lastResult["videoslotstate"]["freespinnumber"];
            resumeGameState["free_games_remaining"] = lastResult["videoslotstate"]["bonusgamecount"];
            resumeGameState["trigger_path"]         = lastResult["videoslotstate"]["triggeranticiplationreellist"];
            resumeGameState["multiplier"]           = lastResult["videoslotstate"]["gamemultiplier"];

            JArray virtulaReelList = lastResult["videoslotstate"]["virtualreellist"] as JArray;

            JArray changeLists = new JArray();
            int wildSymbol = 0;
            if (!object.ReferenceEquals(lastResult["videoslotstate"]["animatesymbollist"], null))
            {
                if ((lastResult["videoslotstate"]["animatesymbollist"] as JArray).Count > 0)
                    changeLists = lastResult["videoslotstate"]["animatesymbollist"] as JArray;

                for (int i = 0; i < (lastResult["videoslotstate"]["animatesymbollist"] as JArray).Count; i++)
                {
                    wildSymbol = (int)lastResult["videoslotstate"]["animatesymbollist"][i]["symbolid"];
                    if (wildSymbol != 0)
                        break;
                }
            }

            if (!object.ReferenceEquals(lastResult["videoslotstate"]["expandingwildlist"], null))
            {
                if ((lastResult["videoslotstate"]["expandingwildlist"] as JArray).Count > 0)
                    changeLists = lastResult["videoslotstate"]["expandingwildlist"] as JArray;

                for (int i = 0; i < (lastResult["videoslotstate"]["expandingwildlist"] as JArray).Count; i++)
                {
                    wildSymbol = (int)lastResult["videoslotstate"]["expandingwildlist"][i]["symbolid"];
                    if (wildSymbol != 0)
                        break;
                }
            }

            if(wildSymbol != 0)
                getVirtualReelList(lastResult,wildSymbol,changeLists,virtulaReelList);
            if(wildSymbol == 16)//러빙(현무)일때
                resumeGameState["gamedata"] = getRovingData(changeLists);

            resumeGameState["virtualreels"]         = lastResult["videoslotstate"]["virtualreellist"];
            resumeGameStates.Add(resumeGameState);
            resumeGame["states"]            = resumeGameStates;
            resumeGames.Add(resumeGame);
            return resumeGames;
        }

        private void getVirtualReelList(dynamic response,int wildSymbol,JArray changeLists, JArray virtulaReelList)
        {
            if (wildSymbol == 15)
            {
                for (int i = 0; i < changeLists.Count; i++)
                {
                    if ((int)changeLists[i]["symbolid"] != wildSymbol)
                        continue;

                    int col = (int)changeLists[i]["reelindex"];
                    for (int j = 0; j < 3; j++)
                        virtulaReelList[col][j + 2] = wildSymbol;
                }
            }
            else
            {
                for (int i = 0; i < changeLists.Count; i++)
                {
                    if ((int)changeLists[i]["symbolid"] != wildSymbol)
                        continue;

                    int col = (int)changeLists[i]["x"];
                    int row = (int)changeLists[i]["y"];
                    virtulaReelList[col][row + 2] = wildSymbol;
                }
            }
        }

        private string getRovingData(JArray changeLists)
        {
            List<int> rovingList = new List<int>();
            for (int i = 0; i < changeLists.Count; i++)
            {
                if ((int)changeLists[i]["symbolid"] != 16)
                    continue;

                int col = (int)changeLists[i]["x"];
                int row = (int)changeLists[i]["y"];

                rovingList.Add(col);
                rovingList.Add(row);
            }
            return string.Join("|", rovingList.ToArray());
        }
    }
}
