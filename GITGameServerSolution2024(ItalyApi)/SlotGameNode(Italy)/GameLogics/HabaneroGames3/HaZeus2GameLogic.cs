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
    public class HaZeus2GameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGZeus2";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "94b55869-884b-43b9-a4e3-430e0cbb5f07";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "ff2fe0a268b6ede3ccaa43e57e0fb389c073a8cc";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idWarrior",     name = "Warrior"    } },
                    {2,   new HabaneroLogSymbolIDName{id = "idZeus",        name = "Zeus"       } },
                    {3,   new HabaneroLogSymbolIDName{id = "idAcropolis",   name = "Acropolis"  } },
                    {4,   new HabaneroLogSymbolIDName{id = "idMaiden",      name = "Maiden"     } },
                    {5,   new HabaneroLogSymbolIDName{id = "idPlate",       name = "Plate"      } },
                    {6,   new HabaneroLogSymbolIDName{id = "idShield",      name = "Shield"     } },
                    {7,   new HabaneroLogSymbolIDName{id = "idLyre",        name = "Lyre"       } },
                    {8,   new HabaneroLogSymbolIDName{id = "idVase",        name = "Vase"       } },
                    {9,   new HabaneroLogSymbolIDName{id = "idScrolls",     name = "Scrolls"    } },    
                    {10,  new HabaneroLogSymbolIDName{id = "idStatue",      name = "Statue"     } },
                    {11,  new HabaneroLogSymbolIDName{id = "idTrireme",     name = "Trireme"    } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 254;
            }
        }
        #endregion

        public HaZeus2GameLogic()
        {
            _gameID     = GAMEID.HaZeus2;
            GameName    = "HaZeus2";
        }
    }
}
