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
    public class TowerOfPizzaGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGTowerOfPizza";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "2d04a4fe-c3ef-4394-9d89-99da7d5c7df4";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "1008fa17ee34c9923f3740b467e313c249c77141";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idTowerofPizza",    name = "TowerofPizza"   } },
                    {2,   new HabaneroLogSymbolIDName{id = "idChef",            name = "Chef"           } },
                    {3,   new HabaneroLogSymbolIDName{id = "idLady",            name = "Lady"           } },
                    {4,   new HabaneroLogSymbolIDName{id = "idPoodle",          name = "Poodle"         } },
                    {5,   new HabaneroLogSymbolIDName{id = "idWine",            name = "Wine"           } },    
                    {6,   new HabaneroLogSymbolIDName{id = "idGarlicBread",     name = "GarlicBread"    } },
                    {7,   new HabaneroLogSymbolIDName{id = "idA",               name = "A"              } },
                    {8,   new HabaneroLogSymbolIDName{id = "idK",               name = "K"              } },
                    {9,   new HabaneroLogSymbolIDName{id = "idQ",               name = "Q"              } },
                    {10,  new HabaneroLogSymbolIDName{id = "idJ",               name = "J"              } },
                    {11,  new HabaneroLogSymbolIDName{id = "id10",              name = "10"             } },
                    {12,  new HabaneroLogSymbolIDName{id = "id9",               name = "9"              } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 151;
            }
        }
        #endregion

        public TowerOfPizzaGameLogic()
        {
            _gameID     = GAMEID.TowerOfPizza;
            GameName    = "TowerOfPizza";
        }

        protected override JArray buildHabaneroLogReels(string strUserId, int currentIndex, dynamic response, bool containWild = false)
        {
            JArray reels = base.buildHabaneroLogReels(strUserId, currentIndex, response as JObject, containWild);
            if (object.ReferenceEquals(response["expandingwilds"], null) || response["expandingwilds"].Count == 0)
                return reels;

            for (int i = 0; i < response["expandingwilds"].Count; i++)
            {
                int reelindex   = (int)response["expandingwilds"][i]["reelindex"];
                int symbolid    = (int)response["expandingwilds"][i]["symbolid"];

                for (int j = 0; j < 3; j++)
                {
                    reels[reelindex][j] = SymbolIdStringForLog[symbolid].id;
                    reels[reelindex][j] = SymbolIdStringForLog[symbolid].id;
                }
            }
            return reels;
        }
    }
}
