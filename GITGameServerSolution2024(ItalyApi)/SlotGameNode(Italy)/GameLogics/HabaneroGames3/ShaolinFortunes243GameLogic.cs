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
    public class ShaolinFortunes243GameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGShaolinFortunes243";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "9ccad5a4-09ac-4d39-903e-61977bb3fc47";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "e582caa64a4e0c59b6dce95a147b2714f3c22a1d";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idBell",        name = "Bell"       } },
                    {2,   new HabaneroLogSymbolIDName{id = "idShaolin",     name = "Shaolin"    } },    
                    {3,   new HabaneroLogSymbolIDName{id = "idDragon",      name = "Dragon"     } },
                    {4,   new HabaneroLogSymbolIDName{id = "idSwords",      name = "Swords"     } },    
                    {5,   new HabaneroLogSymbolIDName{id = "idBuddah",      name = "Buddah"     } },
                    {6,   new HabaneroLogSymbolIDName{id = "idCoin",        name = "Coin"       } },
                    {7,   new HabaneroLogSymbolIDName{id = "idYinYang",     name = "YinYang"    } },
                    {8,   new HabaneroLogSymbolIDName{id = "idDummies",     name = "Dummies"    } },
                    {9,   new HabaneroLogSymbolIDName{id = "idPagoda",      name = "Pagoda"     } },    
                    {10,  new HabaneroLogSymbolIDName{id = "idNunchuck",    name = "Nunchuck"   } },
                    {11,  new HabaneroLogSymbolIDName{id = "idFuShoes",     name = "FuShoes"    } },
                    {12,  new HabaneroLogSymbolIDName{id = "idPike",        name = "Pike"       } },
                    {13,  new HabaneroLogSymbolIDName{id = "idBellx3",      name = "Bellx3"     } },
                    {14,  new HabaneroLogSymbolIDName{id = "idBellx5",      name = "Bellx5"     } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 208;
            }
        }
        #endregion

        public ShaolinFortunes243GameLogic()
        {
            _gameID     = GAMEID.ShaolinFortunes243;
            GameName    = "ShaolinFortunes243";
        }
    }
}
