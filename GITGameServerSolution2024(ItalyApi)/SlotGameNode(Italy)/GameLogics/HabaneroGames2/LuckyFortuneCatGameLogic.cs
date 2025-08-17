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
    public class LuckyFortuneCatGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGLuckyFortuneCat";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "a13786a3-bd33-46dc-b54f-391b3c72de3b";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "0fc11ca8b69007566380f5feaab0273e541f494e";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.5881.341";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idCat",         name = "Cat"    } },    
                    {2,   new HabaneroLogSymbolIDName{id = "idDaruma",      name = "Daruma" } },    
                    {3,   new HabaneroLogSymbolIDName{id = "idIngot",       name = "Ingot"  } },    
                    {4,   new HabaneroLogSymbolIDName{id = "idBag",         name = "Bag"    } },    
                    {5,   new HabaneroLogSymbolIDName{id = "idKoi",         name = "Koi"    } },    
                    {6,   new HabaneroLogSymbolIDName{id = "idFan",         name = "Fan"    } },    

                    {7,   new HabaneroLogSymbolIDName{id = "idDice6",       name = "Dice6"  } },    
                    {8,   new HabaneroLogSymbolIDName{id = "idDice5",       name = "Dice5"  } },    
                    {9,   new HabaneroLogSymbolIDName{id = "idDice4",       name = "Dice4"  } },    
                    {10,  new HabaneroLogSymbolIDName{id = "idDice3",       name = "Dice3"  } },    
                    {11,  new HabaneroLogSymbolIDName{id = "idDice2",       name = "Dice2"  } },    
                    {12,  new HabaneroLogSymbolIDName{id = "idDice1",       name = "Dice1"  } },    
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 480;
            }
        }
        #endregion

        public LuckyFortuneCatGameLogic()
        {
            _gameID     = GAMEID.LuckyFortuneCat;
            GameName    = "LuckyFortuneCat";
        }

        protected override JObject buildEventItem(string strGlobalUserId, int currentIndex)
        {
            dynamic eventItem = base.buildEventItem(strGlobalUserId, currentIndex);
            dynamic currentResponse = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserId].Responses[currentIndex].Response);
            
            bool hasDaruma = false;
            for(int i = 0; i < currentResponse["videoslotstate"]["virtualreellist"].Count; i++)
            {
                for(int j = 2; j < currentResponse["videoslotstate"]["virtualreellist"][i].Count - 2; j++)
                {
                    if ((int)currentResponse["videoslotstate"]["virtualreellist"][i][j] == 2)
                    {
                        hasDaruma = true;
                        break;
                    }
                }
                if (hasDaruma)
                    break;
            }

            if (hasDaruma)
            {
                JArray reels = new JArray();
                if (currentIndex < 2)
                {
                    JArray  reelsList       = new JArray();
                    JObject reelsListItem   = new JObject();
                    JArray  reelsListArray  = new JArray();

                    for (int i = 0; i < currentResponse["videoslotstate"]["virtualreellist"].Count; i++)
                    {
                        JArray reelsCol = new JArray();
                        JArray reelsListCol = new JArray();
                        for (int j = 2; j < currentResponse["videoslotstate"]["virtualreellist"][i].Count - 2; j++)
                        {
                            int symbolid = (int)currentResponse["videoslotstate"]["virtualreellist"][i][j];
                            if (symbolid == 2)
                            {
                                reelsCol.Add(string.Format("{0}{1}", SymbolIdStringForLog[symbolid].id,currentIndex + 1));
                                reelsListCol.Add(string.Format("{0}{1}", SymbolIdStringForLog[symbolid].id, currentIndex));
                            }
                            else
                            {
                                reelsCol.Add(SymbolIdStringForLog[symbolid].id);
                                reelsListCol.Add(SymbolIdStringForLog[symbolid].id);
                            }
                        }
                        reels.Add(reelsCol);
                        reelsListArray.Add(reelsListCol);
                    }
                    reelsListItem["reels"] = reelsListArray;
                    reelsList.Add(reelsListItem);
                    eventItem["reelslist"]  = reelsList;
                }
                else
                {
                    for (int i = 0; i < currentResponse["videoslotstate"]["virtualreellist"].Count; i++)
                    {
                        JArray reelsCol = new JArray();
                        for (int j = 2; j < currentResponse["videoslotstate"]["virtualreellist"][i].Count - 2; j++)
                        {
                            int symbolid = (int)currentResponse["videoslotstate"]["virtualreellist"][i][j];
                            if (symbolid == 2)
                                reelsCol.Add(string.Format("{0}{1}", SymbolIdStringForLog[symbolid].id,2));
                            else
                                reelsCol.Add(SymbolIdStringForLog[symbolid].id);
                        }
                        reels.Add(reelsCol);
                    }
                }
                eventItem["reels"]      = reels;
            }
            return eventItem;
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
            resumeGameState["currentfreegame"]      = 0;
            if(!object.ReferenceEquals(lastResult["videoslotstate"]["freespinnumber"],null))
                resumeGameState["currentfreegame"]      = lastResult["videoslotstate"]["freespinnumber"];
            
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
