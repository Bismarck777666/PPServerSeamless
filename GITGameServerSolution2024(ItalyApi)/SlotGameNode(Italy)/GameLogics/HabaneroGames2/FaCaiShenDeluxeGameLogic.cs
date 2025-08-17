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
    public class FaCaiShenDeluxeGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGFaCaiShenDeluxe";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "28171e75-007e-4e40-abf5-49886705274f";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "e56549135602ab4df5c62594901a17917d66575a";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.5276.322";
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
                return 0;
            }
        }
        protected override string BetType
        {
            get
            {
                return "HorizontalVerticalPays";
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idGod",     name = "God"    } },    
                    {2,   new HabaneroLogSymbolIDName{id = "idIngot",   name = "Ingot"  } },    
                    {3,   new HabaneroLogSymbolIDName{id = "idDragon",  name = "Dragon" } },    
                    {4,   new HabaneroLogSymbolIDName{id = "idCoin",    name = "Coin"   } },    
                    {5,   new HabaneroLogSymbolIDName{id = "idFish",    name = "Fish"   } },    
                    {6,   new HabaneroLogSymbolIDName{id = "idTree",    name = "Tree"   } },    
                    {7,   new HabaneroLogSymbolIDName{id = "idKnot",    name = "Knot"   } },    
                    {8,   new HabaneroLogSymbolIDName{id = "idRing",    name = "Ring"   } },    
                    {9,   new HabaneroLogSymbolIDName{id = "idA",       name = "A"      } },    
                    {10,  new HabaneroLogSymbolIDName{id = "idK",       name = "K"      } },    
                    {11,  new HabaneroLogSymbolIDName{id = "idQ",       name = "Q"      } },    
                    {12,  new HabaneroLogSymbolIDName{id = "idJ",       name = "J"      } },    
                    {13,  new HabaneroLogSymbolIDName{id = "id10",      name = "10"     } },    
                    {14,  new HabaneroLogSymbolIDName{id = "id9",       name = "9"      } },    
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 493;
            }
        }
        #endregion

        public FaCaiShenDeluxeGameLogic()
        {
            _gameID     = GAMEID.FaCaiShenDeluxe;
            GameName    = "FaCaiShenDeluxe";
        }

        protected override JArray buildHabaneroLogReels(string strGlobalUserId,int currentIndex ,dynamic response, bool containWild = false)
        {
            JArray reels = base.buildHabaneroLogReels(strGlobalUserId, currentIndex, response as JObject, containWild);
            if (object.ReferenceEquals(response["expandingwilds"], null) || response["expandingwilds"].Count == 0)
                return reels;

            for(int i = 0; i < response["expandingwilds"].Count; i++)
            {
                int reelindex   = (int)response["expandingwilds"][i]["reelindex"];
                int symbolindex = (int)response["expandingwilds"][i]["symbolindex"];
                
                if (symbolindex < 1)
                    symbolindex = 1;
                
                if (symbolindex > 3)
                    symbolindex = 3;
                
                reels[reelindex][symbolindex - 1] = SymbolIdStringForLog[1].id;
                reels[reelindex][symbolindex + 1] = SymbolIdStringForLog[1].id;
            }
            return reels;
        }
    }
}
