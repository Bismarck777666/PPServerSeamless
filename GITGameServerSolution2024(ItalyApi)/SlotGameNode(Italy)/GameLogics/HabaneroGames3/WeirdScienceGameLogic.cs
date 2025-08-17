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
    public class WeirdScienceGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGWeirdScience";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "439217f3-8876-41e3-addd-b152969ba0e4";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "5da663dbde59d1b53785c7a53a3b8af40f2f7887";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.12512.857";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idScientist",       name = "Scientist"      } },
                    {2,   new HabaneroLogSymbolIDName{id = "idOrb",             name = "Orb"            } },
                    {3,   new HabaneroLogSymbolIDName{id = "idAssistant",       name = "Assistant"      } },
                    {4,   new HabaneroLogSymbolIDName{id = "idTestTubes",       name = "TestTubes"      } },
                    {5,   new HabaneroLogSymbolIDName{id = "idMouse",           name = "Mouse"          } },
                    {6,   new HabaneroLogSymbolIDName{id = "idFrog",            name = "Frog"           } },
                    {7,   new HabaneroLogSymbolIDName{id = "idOctopus",         name = "Octopus"        } },
                    {8,   new HabaneroLogSymbolIDName{id = "idEyes",            name = "Eyes"           } },
                    {9,   new HabaneroLogSymbolIDName{id = "idMicroscope",      name = "Microscope"     } },
                    {10,  new HabaneroLogSymbolIDName{id = "idToxicWaste",      name = "ToxicWaste"     } },
                    {11,  new HabaneroLogSymbolIDName{id = "idPoison",          name = "Poison"         } },
                    {12,  new HabaneroLogSymbolIDName{id = "idMagnifyingGlass", name = "MagnifyingGlass"} },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 175;
            }
        }
        #endregion

        public WeirdScienceGameLogic()
        {
            _gameID     = GAMEID.WeirdScience;
            GameName    = "WeirdScience";
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);
            HabaneroHistoryResponses responses = _dicUserHistory[strUserId].Responses[currentIndex];
            dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            if (!object.ReferenceEquals(resultContext["currentfreegame"], null))
                eventItem["multiplier"] = 3;

            return eventItem;
        }
    }
}
