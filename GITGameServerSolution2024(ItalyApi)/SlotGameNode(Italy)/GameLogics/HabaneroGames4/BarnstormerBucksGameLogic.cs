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
    public class BarnstormerBucksGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGBarnstormerBucks";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "d20996dc-1963-4f59-b9d9-6240741277fc";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "80ff3397a6e7d6b6e8158dfc9887a06a49b885ad";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idPilot",       name = "Pilot"          } },
                    {2,   new HabaneroLogSymbolIDName{id = "idPlane",       name = "Plane"          } },
                    {3,   new HabaneroLogSymbolIDName{id = "idFarmersWife", name = "FarmersWife"    } },    
                    {4,   new HabaneroLogSymbolIDName{id = "idBarn",        name = "Barn"           } },    
                    {5,   new HabaneroLogSymbolIDName{id = "idTractor",     name = "Tractor"        } },    
                    {6,   new HabaneroLogSymbolIDName{id = "idCow",         name = "Cow"            } },
                    {7,   new HabaneroLogSymbolIDName{id = "idDog",         name = "Dog"            } },
                    {8,   new HabaneroLogSymbolIDName{id = "idPig",         name = "Pig"            } },
                    {9,   new HabaneroLogSymbolIDName{id = "idSheep",       name = "Sheep"          } },
                    {10,  new HabaneroLogSymbolIDName{id = "idChicken",     name = "Chicken"        } },
                    {11,  new HabaneroLogSymbolIDName{id = "idOilCan",      name = "OilCan"         } },
                    {12,  new HabaneroLogSymbolIDName{id = "idGrain",       name = "Grain"          } },
                    {13,  new HabaneroLogSymbolIDName{id = "idHayBale",     name = "HayBale"        } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 159;
            }
        }
        #endregion

        public BarnstormerBucksGameLogic()
        {
            _gameID     = GAMEID.BarnstormerBucks;
            GameName    = "BarnstormerBucks";
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
