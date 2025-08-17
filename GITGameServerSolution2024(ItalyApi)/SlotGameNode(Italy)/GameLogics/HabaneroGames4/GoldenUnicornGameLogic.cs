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
    public class GoldenUnicornGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGGoldenUnicorn";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "4b5c6092-f619-4832-8211-bf96204064c8";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "eb9e2517a7d7e5f059c1f8f81f6e65fe9affa80b";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.9652.404";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idUnicorn",     name = "Unicorn"        } },
                    {2,   new HabaneroLogSymbolIDName{id = "idGoldUnicorn", name = "GoldUnicorn"    } },
                    {3,   new HabaneroLogSymbolIDName{id = "idPalace",      name = "Palace"         } },
                    {4,   new HabaneroLogSymbolIDName{id = "idPrincess",    name = "Princess"       } },
                    {5,   new HabaneroLogSymbolIDName{id = "idFairy",       name = "Fairy"          } },
                    {6,   new HabaneroLogSymbolIDName{id = "idDeer",        name = "Deer"           } },
                    {7,   new HabaneroLogSymbolIDName{id = "idBeetle",      name = "Beetle"         } },
                    {8,   new HabaneroLogSymbolIDName{id = "idA",           name = "A"              } },
                    {9,   new HabaneroLogSymbolIDName{id = "idK",           name = "K"              } },
                    {10,  new HabaneroLogSymbolIDName{id = "idQ",           name = "Q"              } },
                    {11,  new HabaneroLogSymbolIDName{id = "idJ",           name = "J"              } },
                    {12,  new HabaneroLogSymbolIDName{id = "id10",          name = "10"             } },
                    {13,  new HabaneroLogSymbolIDName{id = "id9",           name = "9"              } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 184;
            }
        }
        #endregion

        public GoldenUnicornGameLogic()
        {
            _gameID     = GAMEID.GoldenUnicorn;
            GameName    = "GoldenUnicorn";
        }
    }
}
