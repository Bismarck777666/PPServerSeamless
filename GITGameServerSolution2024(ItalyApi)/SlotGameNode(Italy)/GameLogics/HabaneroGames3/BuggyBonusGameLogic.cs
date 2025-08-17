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
    public class BuggyBonusGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGBuggyBonus";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "933f9f46-b716-4a63-bec7-a011d87e9f0e";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "f47cabb19aceaa87b9f3d2567c3a132cc686fbc0";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idBeehive",     name = "Beehive"        } },
                    {2,   new HabaneroLogSymbolIDName{id = "idCaterpillar", name = "Caterpillar"    } },
                    {3,   new HabaneroLogSymbolIDName{id = "idLadyBug",     name = "LadyBug"        } },
                    {4,   new HabaneroLogSymbolIDName{id = "idMantis",      name = "Mantis"         } },
                    {5,   new HabaneroLogSymbolIDName{id = "idMagnifyer",   name = "Magnifyer"      } },
                    {6,   new HabaneroLogSymbolIDName{id = "idAnt",         name = "Ant"            } },
                    {7,   new HabaneroLogSymbolIDName{id = "idSpider",      name = "Spider"         } },
                    {8,   new HabaneroLogSymbolIDName{id = "idSnail",       name = "Snail"          } },
                    {9,   new HabaneroLogSymbolIDName{id = "idDragonfly",   name = "Dragonfly"      } },    
                    {10,  new HabaneroLogSymbolIDName{id = "idThornBeetle", name = "ThornBeetle"    } },
                    {11,  new HabaneroLogSymbolIDName{id = "idMoth",        name = "Moth"           } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 252;
            }
        }
        #endregion

        public BuggyBonusGameLogic()
        {
            _gameID     = GAMEID.BuggyBonus;
            GameName    = "BuggyBonus";
        }
    }
}
