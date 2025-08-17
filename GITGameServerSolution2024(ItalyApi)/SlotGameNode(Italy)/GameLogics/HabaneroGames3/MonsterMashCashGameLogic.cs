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
    public class MonsterMashCashGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGMonsterMashCash";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "490e28a8-4496-4fbf-ac9b-bcf6b464c016";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "21dec9b4dca8182aee364e1d307cd466264f6af0";
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
                return 50.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 50;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idJekyl",           name = "Jekyl"          } },
                    {2,   new HabaneroLogSymbolIDName{id = "idBonus",           name = "Bonus"          } },    
                    {3,   new HabaneroLogSymbolIDName{id = "idFrankenstein",    name = "Frankenstein"   } },
                    {4,   new HabaneroLogSymbolIDName{id = "idMummy",           name = "Mummy"          } },    
                    {5,   new HabaneroLogSymbolIDName{id = "idSwampThing",      name = "SwampThing"     } },
                    {6,   new HabaneroLogSymbolIDName{id = "idCastle",          name = "Castle"         } },
                    {7,   new HabaneroLogSymbolIDName{id = "idCoffin",          name = "Coffin"         } },
                    {8,   new HabaneroLogSymbolIDName{id = "idCandles",         name = "Candles"        } },
                    {9,   new HabaneroLogSymbolIDName{id = "idPumpkin",         name = "Pumpkin"        } },    
                    {10,  new HabaneroLogSymbolIDName{id = "idWerewolf",        name = "Werewolf"       } },
                    {11,  new HabaneroLogSymbolIDName{id = "idScientist",       name = "Scientist"      } },
                    {12,  new HabaneroLogSymbolIDName{id = "idZombie",          name = "Zombie"         } },
                    {13,  new HabaneroLogSymbolIDName{id = "idIgor",            name = "Igor"           } },
                    {14,  new HabaneroLogSymbolIDName{id = "idMicro",           name = "Micro"          } },
                    {15,  new HabaneroLogSymbolIDName{id = "idEyeball",         name = "Eyeball"        } },
                    {16,  new HabaneroLogSymbolIDName{id = "idHyde",            name = "Hyde"           } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 192;
            }
        }
        #endregion

        public MonsterMashCashGameLogic()
        {
            _gameID     = GAMEID.MonsterMashCash;
            GameName    = "MonsterMashCash";
        }

        protected override JArray buildHabaneroLogReels(string strUserId, int currentIndex, dynamic response, bool containWild = false)
        {
            JArray reels = new JArray();
            for (int j = 0; j < response["videoslotstate"]["virtualreellist"].Count; j++)
            {
                JArray col = new JArray();
                for (int k = 2; k < response["videoslotstate"]["virtualreellist"][j].Count - 2; k++)
                {
                    int     symbol      = Convert.ToInt32(response["videoslotstate"]["virtualreellist"][j][k]);
                    string  symbolid    = SymbolIdStringForLog[symbol].id;
                    col.Add(symbolid);
                }
                reels.Add(col);
            }
            
            if (!object.ReferenceEquals(response["videoslotstate"]["expandingwildlist"], null))
            {
                for (int i = 0; i < response["videoslotstate"]["expandingwildlist"].Count; i++)
                {
                    int reelIndex   = Convert.ToInt32(response["videoslotstate"]["expandingwildlist"][i]["reelindex"]);
                    int symbolId    = Convert.ToInt32(response["videoslotstate"]["expandingwildlist"][i]["symbolid"]);
                    
                    for(int j = 0; j < reels[reelIndex].Count(); j++)
                        reels[reelIndex][j] = SymbolIdStringForLog[symbolId].id;
                }
            }
            return reels;
        }

        protected override JArray buildInitResumeGame(string strUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult,string gameinstanceid, string roundid,HabaneroActionType currentAction)
        {
            JArray resumeGames = base.buildInitResumeGame(strUserID, betInfo, lastResult, gameinstanceid, roundid, currentAction);
            resumeGames[0]["states"][0]["name"] = lastResult["videoslotstate"]["gamemodename"];
            return resumeGames;
        }
    }
}
