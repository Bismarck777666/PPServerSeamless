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
    public class SOSGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGSOS";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "b0d7cdbe-3ab6-4961-80e2-04d45fc0354d";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "13683463b9f4242c1e8f29caeb02537118af9026";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idPilot",       name = "Pilot"      } },
                    {2,   new HabaneroLogSymbolIDName{id = "idWhirlpool",   name = "Whirlpool"  } },
                    {4,   new HabaneroLogSymbolIDName{id = "idPlane",       name = "Plane"      } },
                    {5,   new HabaneroLogSymbolIDName{id = "idCaptain",     name = "Captain"    } },
                    {6,   new HabaneroLogSymbolIDName{id = "idLiner",       name = "Liner"      } },
                    {7,   new HabaneroLogSymbolIDName{id = "idShipwreck",   name = "Shipwreck"  } },
                    {8,   new HabaneroLogSymbolIDName{id = "idSeagull",     name = "Seagull"    } },
                    {9,   new HabaneroLogSymbolIDName{id = "idLifeJacket",  name = "LifeJacket" } },
                    {10,  new HabaneroLogSymbolIDName{id = "idLifeBuoy",    name = "LifeBuoy"   } },
                    {11,  new HabaneroLogSymbolIDName{id = "idCompass",     name = "Compass"    } },
                    {12,  new HabaneroLogSymbolIDName{id = "idMap",         name = "Map"        } },
                    {13,  new HabaneroLogSymbolIDName{id = "idS",           name = "idS"        } },
                    {14,  new HabaneroLogSymbolIDName{id = "idO",           name = "idO"        } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 171;
            }
        }
        #endregion

        public SOSGameLogic()
        {
            _gameID     = GAMEID.SOS;
            GameName    = "SOS";
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
