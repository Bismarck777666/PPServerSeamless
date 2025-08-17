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
    public class EgyptianDreamsGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGEgyptianDreams";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "70f79e5b-6acd-4fbc-8981-3622794a1edb";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "b983d73c9f49f5549caeffc9b65dec6bf1a3a787";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idPharaoh",     name = "Pharaoh"    } },
                    {2,   new HabaneroLogSymbolIDName{id = "idPyramid",     name = "Pyramid"    } },
                    {4,   new HabaneroLogSymbolIDName{id = "idSphynx",      name = "Sphynx"     } },    
                    {3,   new HabaneroLogSymbolIDName{id = "idCleopatra",   name = "Cleopatra"  } },    
                    {5,   new HabaneroLogSymbolIDName{id = "idHeiroglyph",  name = "Heiroglyph" } },    
                    {6,   new HabaneroLogSymbolIDName{id = "idBoat",        name = "Boat"       } },
                    {7,   new HabaneroLogSymbolIDName{id = "idTablet",      name = "Tablet"     } },
                    {8,   new HabaneroLogSymbolIDName{id = "idCamel",       name = "Camel"      } },
                    {9,   new HabaneroLogSymbolIDName{id = "idCat",         name = "Cat"        } },
                    {10,  new HabaneroLogSymbolIDName{id = "idAnkh",        name = "Ankh"       } },
                    {11,  new HabaneroLogSymbolIDName{id = "idScorpion",    name = "Scorpion"   } },
                    {12,  new HabaneroLogSymbolIDName{id = "idNecklace",    name = "Necklace"   } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 157;
            }
        }
        #endregion

        public EgyptianDreamsGameLogic()
        {
            _gameID     = GAMEID.EgyptianDreams;
            GameName    = "EgyptianDreams";
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);
            HabaneroHistoryResponses responses = _dicUserHistory[strUserId].Responses[currentIndex];
            dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            if (responses.Action == HabaneroActionType.FREEGAME)
                eventItem["multiplier"] = 3;

            dynamic eventContext = eventItem as dynamic;
            if(!object.ReferenceEquals(eventContext["subevents"], null) && eventContext["subevents"].Count > 0)
            {
                for(int i = 0; i < eventContext["subevents"].Count; i++)
                {
                    if(eventContext["subevents"][i]["type"] == "scatter")
                    {
                        eventContext["subevents"][i]["symbol"] = SymbolIdStringForLog[2].name;
                    }
                }
            }
            
            return eventItem;
        }
    }
}
