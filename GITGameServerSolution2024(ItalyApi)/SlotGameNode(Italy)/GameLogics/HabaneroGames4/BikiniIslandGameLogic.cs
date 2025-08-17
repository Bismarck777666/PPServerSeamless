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
    public class BikiniIslandGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGBikiniIsland";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "3b8ba858-ed5e-4600-bf0c-d7ec8f410688";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "9538570baa35e958b172414c91156a05fb136e61";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idBikiniBabe1",     name = "BikiniBabe1"    } },
                    {2,   new HabaneroLogSymbolIDName{id = "idBikiniBabe2",     name = "BikiniBabe2"    } },
                    {3,   new HabaneroLogSymbolIDName{id = "idBikiniBabe3",     name = "BikiniBabe3"    } },
                    {4,   new HabaneroLogSymbolIDName{id = "idIsland",          name = "Island"         } },
                    {5,   new HabaneroLogSymbolIDName{id = "idLifeguard",       name = "Lifeguard"      } },    
                    {6,   new HabaneroLogSymbolIDName{id = "idPartyHut",        name = "PartyHut"       } },
                    {7,   new HabaneroLogSymbolIDName{id = "idDeckChair",       name = "DeckChair"      } },
                    {8,   new HabaneroLogSymbolIDName{id = "idFish",            name = "Fish"           } },
                    {9,   new HabaneroLogSymbolIDName{id = "idShell",           name = "Shell"          } },
                    {10,  new HabaneroLogSymbolIDName{id = "idFlower",          name = "Flower"         } },
                    {11,  new HabaneroLogSymbolIDName{id = "idBeachBall",       name = "BeachBall"      } },
                    {12,  new HabaneroLogSymbolIDName{id = "idStarfish",        name = "Starfish"       } },
                    {13,  new HabaneroLogSymbolIDName{id = "idSunglasses",      name = "Sunglasses"     } },
                    {14,  new HabaneroLogSymbolIDName{id = "idSunscreen",       name = "Sunscreen"      } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 155;
            }
        }
        #endregion

        public BikiniIslandGameLogic()
        {
            _gameID     = GAMEID.BikiniIsland;
            GameName    = "BikiniIsland";
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);
            HabaneroHistoryResponses responses = _dicUserHistory[strUserId].Responses[currentIndex];
            dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            if (responses.Action == HabaneroActionType.FREEGAME)
                eventItem["multiplier"] = 2;

            dynamic eventContext = eventItem as dynamic;
            if(!object.ReferenceEquals(eventContext["subevents"], null) && eventContext["subevents"].Count > 0)
            {
                for(int i = 0; i < eventContext["subevents"].Count; i++)
                {
                    if(eventContext["subevents"][i]["type"] == "scatter")
                    {
                        eventContext["subevents"][i]["symbol"] = SymbolIdStringForLog[4].name;
                    }
                }
            }
            
            return eventItem;
        }
    }
}
