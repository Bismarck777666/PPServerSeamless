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
    public class TwelveZodiacsGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SG12Zodiacs";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "02c0a40f-c396-4db8-ba86-b4a4e149bb3a";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "828af768c4746786a711059f4241fb93e6d45b6c";
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
                return 18.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 18;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,     new HabaneroLogSymbolIDName{id = "idWild",      name = "Wild"       } },
                    {2,     new HabaneroLogSymbolIDName{id = "idScatter",   name = "Scatter"    } },
                    {3,     new HabaneroLogSymbolIDName{id = "idRat",       name = "Rat"        } },
                    {4,     new HabaneroLogSymbolIDName{id = "idOx",        name = "Ox"         } },
                    {5,     new HabaneroLogSymbolIDName{id = "idTiger",     name = "Tiger"      } },
                    {6,     new HabaneroLogSymbolIDName{id = "idRabbit",    name = "Rabbit"     } },
                    {7,     new HabaneroLogSymbolIDName{id = "idDragon",    name = "Dragon"     } },
                    {8,     new HabaneroLogSymbolIDName{id = "idSnake",     name = "Snake"      } },
                    {9,     new HabaneroLogSymbolIDName{id = "idHorse",     name = "Horse"      } },
                    {10,    new HabaneroLogSymbolIDName{id = "idSheep",     name = "Sheep"      } },
                    {11,    new HabaneroLogSymbolIDName{id = "idMonkey",    name = "Monkey"     } },
                    {12,    new HabaneroLogSymbolIDName{id = "idRooster",   name = "Rooster"    } },
                    {13,    new HabaneroLogSymbolIDName{id = "idDog",       name = "Dog"        } },
                    {14,    new HabaneroLogSymbolIDName{id = "idPig",       name = "Pig"        } },
                    {15,    new HabaneroLogSymbolIDName{id = "idSun",       name = "Sun"        } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 306;
            }
        }
        #endregion

        public TwelveZodiacsGameLogic()
        {
            _gameID     = GAMEID.TwelveZodiacs;
            GameName    = "12Zodiacs";
        }
    }
}
