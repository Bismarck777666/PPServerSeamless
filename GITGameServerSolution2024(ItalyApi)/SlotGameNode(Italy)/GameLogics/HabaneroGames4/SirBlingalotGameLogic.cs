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
    public class SirBlingalotGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGSirBlingalot";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "fdb53ac5-02fe-4a27-a96b-662d956e128a";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "7bc25931cf9f5d269adfb8327512651a747b36bf";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idWild",        name = "Wild"           } },
                    {2,   new HabaneroLogSymbolIDName{id = "idBonus",       name = "Bonus"          } },
                    {3,   new HabaneroLogSymbolIDName{id = "idQueen",       name = "Queen"          } },
                    {4,   new HabaneroLogSymbolIDName{id = "idPrince",      name = "Prince"         } },
                    {5,   new HabaneroLogSymbolIDName{id = "idPrincess",    name = "Princess"       } },
                    {6,   new HabaneroLogSymbolIDName{id = "idJester",      name = "Jester"         } },
                    {7,   new HabaneroLogSymbolIDName{id = "idHelmet",      name = "Helmet"         } },
                    {8,   new HabaneroLogSymbolIDName{id = "idGoblet",      name = "Goblet"         } },
                    {9,   new HabaneroLogSymbolIDName{id = "idAxeShield",   name = "AxeShield"      } },
                    {10,  new HabaneroLogSymbolIDName{id = "idSwordShield", name = "SwordShield"    } },
                    {11,  new HabaneroLogSymbolIDName{id = "idCrossBow",    name = "CrossBow"       } },
                    {12,  new HabaneroLogSymbolIDName{id = "idScroll",      name = "Scroll"         } },
                    {13,  new HabaneroLogSymbolIDName{id = "idRoyalSeal",   name = "RoyalSeal"      } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 114;
            }
        }
        #endregion

        public SirBlingalotGameLogic()
        {
            _gameID     = GAMEID.SirBlingalot;
            GameName    = "SirBlingalot";
        }
    }
}
