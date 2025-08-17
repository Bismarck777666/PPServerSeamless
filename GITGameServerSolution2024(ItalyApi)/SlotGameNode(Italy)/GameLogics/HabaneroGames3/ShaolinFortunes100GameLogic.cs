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
    public class ShaolinFortunes100GameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGShaolinFortunes100";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "94182979-83f2-4e8f-a3c1-a340ce5e2bc8";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "9fe721eb877c4a6d05e95bd979cc05e34a49dc54";
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
                return 100.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 100;
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
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 215;
            }
        }
        #endregion

        public ShaolinFortunes100GameLogic()
        {
            _gameID     = GAMEID.ShaolinFortunes100;
            GameName    = "ShaolinFortunes100";
        }
    }
}
