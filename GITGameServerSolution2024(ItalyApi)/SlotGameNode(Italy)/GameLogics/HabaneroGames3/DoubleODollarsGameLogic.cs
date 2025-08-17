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
    public class DoubleODollarsGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGDoubleODollars";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "0b7b7ce7-c88e-4811-b6c5-2be334976d65";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "dea06a9b5defeb23b5fdca3a0cf2e2ad2ec6a2e8";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idWatch",       name = "Watch"          } },
                    {2,   new HabaneroLogSymbolIDName{id = "idBonus",       name = "Bonus"          } },    
                    {3,   new HabaneroLogSymbolIDName{id = "idSpyGuy",      name = "SpyGuy"         } },    
                    {4,   new HabaneroLogSymbolIDName{id = "idGirl",        name = "Girl"           } },    
                    {5,   new HabaneroLogSymbolIDName{id = "idVillian",     name = "Villian"        } },    
                    {6,   new HabaneroLogSymbolIDName{id = "idDoomMoon",    name = "DoomMoon"       } },    
                    {7,   new HabaneroLogSymbolIDName{id = "idBinoculars",  name = "Binoculars"     } },    
                    {8,   new HabaneroLogSymbolIDName{id = "idSatellite",   name = "Satellite"      } },
                    {9,   new HabaneroLogSymbolIDName{id = "idGrapple",     name = "Grapple"        } },    
                    {10,  new HabaneroLogSymbolIDName{id = "idGun",         name = "Gun"            } },    
                    {11,  new HabaneroLogSymbolIDName{id = "idGlass",       name = "Glass"          } },    
                    {12,  new HabaneroLogSymbolIDName{id = "idMissile",     name = "Missile"        } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 224;
            }
        }
        #endregion

        public DoubleODollarsGameLogic()
        {
            _gameID     = GAMEID.DoubleODollars;
            GameName    = "DoubleODollars";
        }
    }
}
