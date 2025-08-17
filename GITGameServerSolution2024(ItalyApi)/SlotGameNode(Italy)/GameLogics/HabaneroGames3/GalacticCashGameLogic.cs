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
    public class GalacticCashGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGGalacticCash";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "0fbbfa6a-4ee9-4982-bb5c-80ba07975d66";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "71cca98a9903c6195635021d8818249a161aefa2";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idGirl",            name = "Girl"           } },
                    {2,   new HabaneroLogSymbolIDName{id = "idBonus",           name = "Bonus"          } },
                    {3,   new HabaneroLogSymbolIDName{id = "idAlienChief",      name = "AlienChief"     } },
                    {4,   new HabaneroLogSymbolIDName{id = "idScientist",       name = "Scientist"      } },
                    {5,   new HabaneroLogSymbolIDName{id = "idRobot",           name = "Robot"          } },
                    {6,   new HabaneroLogSymbolIDName{id = "idEnemyShip",       name = "EnemyShip"      } },
                    {7,   new HabaneroLogSymbolIDName{id = "idEnemyRobot",      name = "EnemyRobot"     } },
                    {8,   new HabaneroLogSymbolIDName{id = "idGlorkaShnork",    name = "GlorkaShnork"   } },
                    {9,   new HabaneroLogSymbolIDName{id = "idGrunt",           name = "Grunt"          } },    
                    {10,  new HabaneroLogSymbolIDName{id = "idFishyLion",       name = "FishyLion"      } },
                    {11,  new HabaneroLogSymbolIDName{id = "idSpaceFighter",    name = "SpaceFighter"   } },
                    {12,  new HabaneroLogSymbolIDName{id = "idTranslator",      name = "Translator"     } },
                    {13,  new HabaneroLogSymbolIDName{id = "idBlaster",         name = "Blaster"        } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 251;
            }
        }
        #endregion

        public GalacticCashGameLogic()
        {
            _gameID     = GAMEID.GalacticCash;
            GameName    = "GalacticCash";
        }
    }
}
