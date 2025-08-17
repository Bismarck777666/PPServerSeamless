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
    public class LuckyLuckyGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGLuckyLucky";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "1b2991a3-b189-451e-a13d-a2d9e6cc440b";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "6cec0dd17b2e1074d08c0c2c4a94e9bcb46712cb";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.3886.285";
            }
        }
        protected override float MiniCoin
        {
            get
            {
                return 1.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 1;
            }
        }
        protected override int[] CoinsIncrement
        {
            get
            {
                return new int[] { 1, 2, 3, 5, 10 };
            }
        }
        protected override double[] StakeIncrement
        {
            get
            {
                return new double[] { 0.50, 1.0, 2.5, 5.0, 10.0, 25.0 };
            }
        }

        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idWild1",       name = "Wild1"          } },
                    {2,   new HabaneroLogSymbolIDName{id = "idWild2",       name = "Wild2"          } },
                    {3,   new HabaneroLogSymbolIDName{id = "idWild3",       name = "Wild3"          } },
                    {4,   new HabaneroLogSymbolIDName{id = "idHigh1",       name = "High1"          } },    
                    {5,   new HabaneroLogSymbolIDName{id = "idHigh2",       name = "High2"          } },    
                    {6,   new HabaneroLogSymbolIDName{id = "idHigh3",       name = "High3"          } },    
                    {7,   new HabaneroLogSymbolIDName{id = "idLow1",        name = "Low1"           } },    
                    {8,   new HabaneroLogSymbolIDName{id = "idLow2",        name = "Low2"           } },    
                    {9,   new HabaneroLogSymbolIDName{id = "idLow3",        name = "Low3"           } },    
                    {10,  new HabaneroLogSymbolIDName{id = "idLowCombined", name = "LowCombined"    } },    //애니
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 456;
            }
        }
        #endregion

        public LuckyLuckyGameLogic()
        {
            _gameID     = GAMEID.LuckyLucky;
            GameName    = "LuckyLucky";
        }
    }
}
