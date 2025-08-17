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
    public class CarnivalCashGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGCarnivalCash";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "fd53fd77-68c4-486e-8e4a-321b467a4369";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "25e752ab674bbf041910963f9ab4cbe0a0a23821";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idElephant",    name = "Elephant"       } },
                    {2,   new HabaneroLogSymbolIDName{id = "idRingmaster",  name = "Ringmaster"     } },    
                    {3,   new HabaneroLogSymbolIDName{id = "idBalloons",    name = "Balloons"       } },
                    {4,   new HabaneroLogSymbolIDName{id = "idCaravan",     name = "Caravan"        } },    
                    {5,   new HabaneroLogSymbolIDName{id = "idBeardedLady", name = "BeardedLady"    } },
                    {6,   new HabaneroLogSymbolIDName{id = "idMerryHorse",  name = "MerryHorse"     } },
                    {7,   new HabaneroLogSymbolIDName{id = "idFerrisWheel", name = "FerrisWheel"    } },
                    {8,   new HabaneroLogSymbolIDName{id = "idLion",        name = "Lion"           } },
                    {9,   new HabaneroLogSymbolIDName{id = "idStrongman",   name = "Strongman"      } },    
                    {10,  new HabaneroLogSymbolIDName{id = "idTent",        name = "Tent"           } },
                    {11,  new HabaneroLogSymbolIDName{id = "idTicket",      name = "Ticket"         } },
                    {12,  new HabaneroLogSymbolIDName{id = "idVendor",      name = "Vendor"         } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 226;
            }
        }
        #endregion

        public CarnivalCashGameLogic()
        {
            _gameID     = GAMEID.CarnivalCash;
            GameName    = "CarnivalCash";
        }
    }
}
