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
    public class CashosaurusGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGCashosaurus";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "33b0d5f5-5656-477d-985a-47042c9de88e";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "da58477e878793fd647e3495b7ae635e2ae6d218";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idCashosaurus",     name = "Cashosaurus"        } },
                    {2,   new HabaneroLogSymbolIDName{id = "idEgg",             name = "Egg"                } },
                    {3,   new HabaneroLogSymbolIDName{id = "idLady",            name = "Lady"               } },
                    {4,   new HabaneroLogSymbolIDName{id = "idRat",             name = "Rat"                } },
                    {5,   new HabaneroLogSymbolIDName{id = "idCarnivorousPlant",name = "CarnivorousPlant"   } },
                    {6,   new HabaneroLogSymbolIDName{id = "idCrystal",         name = "Crystal"            } },
                    {7,   new HabaneroLogSymbolIDName{id = "idA",               name = "A"                  } },
                    {8,   new HabaneroLogSymbolIDName{id = "idK",               name = "K"                  } },
                    {9,   new HabaneroLogSymbolIDName{id = "idQ",               name = "Q"                  } },
                    {10,  new HabaneroLogSymbolIDName{id = "idJ",               name = "J"                  } },
                    {11,  new HabaneroLogSymbolIDName{id = "id10",              name = "10"                 } },
                    {12,  new HabaneroLogSymbolIDName{id = "id9",               name = "9"                  } },
                    {13,  new HabaneroLogSymbolIDName{id = "idFreeEgg",         name = "FreeEgg"            } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 58;
            }
        }
        #endregion

        public CashosaurusGameLogic()
        {
            _gameID     = GAMEID.Cashosaurus;
            GameName    = "Cashosaurus";
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);

            dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strUserId].Responses[currentIndex].Response);

            if (!object.ReferenceEquals(resultContext["currentfreegame"], null))
            {
                int multiplier = 2;
                if (Convert.ToInt32(resultContext["currentfreegame"]) > 8)
                    multiplier = 3;

                eventItem["multiplier"] = multiplier;
            }

            dynamic eventContext = eventItem as dynamic;
            if (!object.ReferenceEquals(eventContext["subevents"], null) && eventContext["subevents"].Count > 0)
            {
                for (int i = 0; i < eventContext["subevents"].Count; i++)
                {
                    if (eventContext["subevents"][i]["type"] == "scatter")
                    {
                        eventContext["subevents"][i]["symbol"] = SymbolIdStringForLog[2].name;
                    }
                }
            }

            return eventItem;
        }
    }
}
