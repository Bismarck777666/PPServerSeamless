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
    public class KanesInfernoGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGKanesInferno";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "75a95c9b-d6dc-4d67-bfc7-40c1f7763128";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "4a6aaf9b157080aa4735652b525cfeb1328f7dcb";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idWild",    name = "Wild"       } },
                    {2,   new HabaneroLogSymbolIDName{id = "idScatter", name = "Scatter"    } },
                    {3,   new HabaneroLogSymbolIDName{id = "idHi1",     name = "Hi1"        } },
                    {4,   new HabaneroLogSymbolIDName{id = "idHi2",     name = "Hi2"        } },
                    {5,   new HabaneroLogSymbolIDName{id = "idHi3",     name = "Hi3"        } },
                    {6,   new HabaneroLogSymbolIDName{id = "idAce",     name = "Ace"        } },
                    {7,   new HabaneroLogSymbolIDName{id = "idKing",    name = "King"       } },
                    {8,   new HabaneroLogSymbolIDName{id = "idQueen",   name = "Queen"      } },
                    {9,   new HabaneroLogSymbolIDName{id = "idJack",    name = "Jack"       } },    
                    {10,  new HabaneroLogSymbolIDName{id = "idTen",     name = "Ten"        } },
                    {11,  new HabaneroLogSymbolIDName{id = "idNine",    name = "Nine"       } },
                    {12,  new HabaneroLogSymbolIDName{id = "idEight",   name = "Eight"      } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 233;
            }
        }
        #endregion

        public KanesInfernoGameLogic()
        {
            _gameID     = GAMEID.KanesInferno;
            GameName    = "KanesInferno";
        }
    }
}
