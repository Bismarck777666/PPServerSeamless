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
    public class CoyoteCrashGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGCoyoteCrash";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "dfbf3b26-70a4-48f8-9857-a7ceaaee4f30";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "efd526d09ce908274c4ac54c95051eabd4af7a9e";
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
                    {1,     new HabaneroLogSymbolIDName{id = "idCoyote",    name = "Coyote"     } },
                    {2,     new HabaneroLogSymbolIDName{id = "idTNT",       name = "TNT"        } },
                    {3,     new HabaneroLogSymbolIDName{id = "idBear",      name = "Bear"       } },
                    {4,     new HabaneroLogSymbolIDName{id = "idLizard",    name = "Lizard"     } },
                    {5,     new HabaneroLogSymbolIDName{id = "idPoliceCar", name = "PoliceCar"  } },
                    {6,     new HabaneroLogSymbolIDName{id = "idCactus",    name = "Cactus"     } },
                    {7,     new HabaneroLogSymbolIDName{id = "idScorpion",  name = "Scorpion"   } },
                    {8,     new HabaneroLogSymbolIDName{id = "idAce",       name = "Ace"        } },
                    {9,     new HabaneroLogSymbolIDName{id = "idKing",      name = "King"       } },
                    {10,    new HabaneroLogSymbolIDName{id = "idQueen",     name = "Queen"      } },
                    {11,    new HabaneroLogSymbolIDName{id = "idJack",      name = "Jack"       } },
                    {12,    new HabaneroLogSymbolIDName{id = "idTen",       name = "Ten"        } },
                    {13,    new HabaneroLogSymbolIDName{id = "idNine",      name = "Nine"       } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 270;
            }
        }
        #endregion

        public CoyoteCrashGameLogic()
        {
            _gameID     = GAMEID.CoyoteCrash;
            GameName    = "CoyoteCrash";
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);
            eventItem["multiplier"] = 4;

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
