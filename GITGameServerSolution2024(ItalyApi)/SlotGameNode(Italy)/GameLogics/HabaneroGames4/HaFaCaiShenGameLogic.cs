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
    public class HaFaCaiShenGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGFaCaiShen";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "26943498-c9d4-48f4-ba64-add6d01b7de1";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "03a235809ce4c167dc9955db274c2f8cc2516220";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.1465.119";
            }
        }
        protected override float MiniCoin
        {
            get
            {
                return 28.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 28;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,     new HabaneroLogSymbolIDName{id = "idGod",       name = "God"    } },
                    {2,     new HabaneroLogSymbolIDName{id = "idIngot",     name = "Ingot"  } },
                    {3,     new HabaneroLogSymbolIDName{id = "idDragon",    name = "Dragon" } },
                    {4,     new HabaneroLogSymbolIDName{id = "idCoin",      name = "Coin"   } },
                    {5,     new HabaneroLogSymbolIDName{id = "idFish",      name = "Fish"   } },
                    {6,     new HabaneroLogSymbolIDName{id = "idTree",      name = "Tree"   } },
                    {7,     new HabaneroLogSymbolIDName{id = "idBook",      name = "Book"   } },
                    {8,     new HabaneroLogSymbolIDName{id = "idRing",      name = "Ring"   } },
                    {9,     new HabaneroLogSymbolIDName{id = "idA",         name = "A"      } },
                    {10,    new HabaneroLogSymbolIDName{id = "idK",         name = "K"      } },
                    {11,    new HabaneroLogSymbolIDName{id = "idQ",         name = "Q"      } },
                    {12,    new HabaneroLogSymbolIDName{id = "idJ",         name = "J"      } },
                    {13,    new HabaneroLogSymbolIDName{id = "id10",        name = "10"     } },
                    {14,    new HabaneroLogSymbolIDName{id = "id9",         name = "9"      } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 301;
            }
        }
        #endregion

        public HaFaCaiShenGameLogic()
        {
            _gameID     = GAMEID.HaFaCaiShen;
            GameName    = "HaFaCaiShen";
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);
            
            dynamic resultContext   = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strUserId].Responses[currentIndex].Response);
            if (!object.ReferenceEquals(resultContext["currentfreegame"], null))
                eventItem["multiplier"] = 1;

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

        protected override JArray buildHabaneroLogReels(string strUserID,int currentIndex ,dynamic response, bool containWild = false)
        {
            JArray reels = base.buildHabaneroLogReels(strUserID, currentIndex, response as JObject, containWild);
            if (object.ReferenceEquals(response["expandingwilds"], null) || response["expandingwilds"].Count == 0)
                return reels;

            for(int i = 0; i < response["expandingwilds"].Count; i++)
            {
                int reelindex   = (int)response["expandingwilds"][i]["reelindex"];
                int symbolindex = (int)response["expandingwilds"][i]["symbolindex"];
                
                reels[reelindex][0] = SymbolIdStringForLog[1].id;
                reels[reelindex][1] = SymbolIdStringForLog[1].id; 
                reels[reelindex][2] = SymbolIdStringForLog[1].id;
            }
            return reels;
        }
    }
}
