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
    public class LittleGreenMoneyGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGLittleGreenMoney";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "5be3f1a6-23c9-4915-b8b1-46feb13536f4";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "79e19176fa99e2ff0f2c32d4e28e2227ac53590e";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.12512.857";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idWild",        name = "Wild"           } },
                    {2,   new HabaneroLogSymbolIDName{id = "idBonus",       name = "Bonus"          } },    
                    {3,   new HabaneroLogSymbolIDName{id = "idPlanet",      name = "Planet"         } },    
                    {4,   new HabaneroLogSymbolIDName{id = "idGun",         name = "Gun"            } },    
                    {5,   new HabaneroLogSymbolIDName{id = "idDevice",      name = "Device"         } },    
                    {6,   new HabaneroLogSymbolIDName{id = "idGreenAlien",  name = "GreenAlien"     } },
                    {7,   new HabaneroLogSymbolIDName{id = "idBadAlien",    name = "BadAlien"       } },
                    {8,   new HabaneroLogSymbolIDName{id = "idBarn",        name = "Barn"           } },
                    {9,   new HabaneroLogSymbolIDName{id = "idFarmer",      name = "Farmer"         } },    
                    {10,  new HabaneroLogSymbolIDName{id = "idCow",         name = "Cow"            } },
                    {11,  new HabaneroLogSymbolIDName{id = "idAgent",       name = "Agent"          } },
                    {12,  new HabaneroLogSymbolIDName{id = "idTopSecret",   name = "TopSecret"      } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 190;
            }
        }
        #endregion

        public LittleGreenMoneyGameLogic()
        {
            _gameID     = GAMEID.LittleGreenMoney;
            GameName    = "LittleGreenMoney";
        }

        protected override JArray buildHabaneroLogReels(string strUserId, int currentIndex, dynamic response, bool containWild = false)
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
            
            if (!object.ReferenceEquals(response["videoslotstate"]["expandingwildlist"], null))
            {
                for (int i = 0; i < response["videoslotstate"]["expandingwildlist"].Count; i++)
                {
                    int reelIndex   = Convert.ToInt32(response["videoslotstate"]["expandingwildlist"][i]["reelindex"]);
                    int symbolId    = Convert.ToInt32(response["videoslotstate"]["expandingwildlist"][i]["symbolid"]);
                    
                    for(int j = 0; j < reels[reelIndex].Count(); j++)
                    {
                        reels[reelIndex][j] = SymbolIdStringForLog[symbolId].id;
                    }
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
