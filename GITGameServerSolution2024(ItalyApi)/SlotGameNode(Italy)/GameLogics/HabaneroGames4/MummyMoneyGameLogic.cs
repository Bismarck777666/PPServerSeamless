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
    public class MummyMoneyGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGMummyMoney";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "9a4d924c-0068-435e-944b-485afa13304e";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "04c0eb031ce617646bcbe8078a566933b194c41d";
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

                    {1,   new HabaneroLogSymbolIDName{id = "idWild",    name = "Wild"   } },
                    {2,   new HabaneroLogSymbolIDName{id = "idBonus",   name = "Bonus"  } },
                    {3,   new HabaneroLogSymbolIDName{id = "idScarab",  name = "Scarab" } },
                    {4,   new HabaneroLogSymbolIDName{id = "idUrn",     name = "Urn"    } },
                    {5,   new HabaneroLogSymbolIDName{id = "idJewel",   name = "Jewel"  } },    
                    {6,   new HabaneroLogSymbolIDName{id = "idAnkh",    name = "Ankh"   } },
                    {7,   new HabaneroLogSymbolIDName{id = "idBook",    name = "Book"   } },
                    {8,   new HabaneroLogSymbolIDName{id = "idHand",    name = "Hand"   } },
                    {9,   new HabaneroLogSymbolIDName{id = "idA",       name = "A"      } },
                    {10,  new HabaneroLogSymbolIDName{id = "idK",       name = "K"      } },
                    {11,  new HabaneroLogSymbolIDName{id = "idQ",       name = "Q"      } },
                    {12,  new HabaneroLogSymbolIDName{id = "idJ",       name = "J"      } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 153;
            }
        }
        #endregion

        public MummyMoneyGameLogic()
        {
            _gameID     = GAMEID.MummyMoney;
            GameName    = "MummyMoney";
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
