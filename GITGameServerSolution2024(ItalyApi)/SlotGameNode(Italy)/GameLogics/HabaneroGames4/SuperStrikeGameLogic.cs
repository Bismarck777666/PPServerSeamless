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
    public class SuperStrikeGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGSuperStrike";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "4401a893-bdd7-4a2b-9c1d-90d54048d8fe";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "9dc41e4c61ef3ed2cb7433f68fba3ac0df0f276d";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idBowler",      name = "Bowler"         } },
                    {2,   new HabaneroLogSymbolIDName{id = "idPins",        name = "Pins"           } },
                    {3,   new HabaneroLogSymbolIDName{id = "idX",           name = "X"              } },    
                    {4,   new HabaneroLogSymbolIDName{id = "idOpponent",    name = "Opponent"       } },    
                    {5,   new HabaneroLogSymbolIDName{id = "idGirl",        name = "Girl"           } },    
                    {6,   new HabaneroLogSymbolIDName{id = "idBurger",      name = "Burger"         } },
                    {7,   new HabaneroLogSymbolIDName{id = "idDrink",       name = "Drink"          } },
                    {8,   new HabaneroLogSymbolIDName{id = "idHotDog",      name = "HotDog"         } },
                    {9,   new HabaneroLogSymbolIDName{id = "idBowlingBall", name = "BowlingBall"    } },
                    {10,  new HabaneroLogSymbolIDName{id = "idTrophy",      name = "Trophy"         } },
                    {11,  new HabaneroLogSymbolIDName{id = "idCase",        name = "Case"           } },
                    {12,  new HabaneroLogSymbolIDName{id = "idShoes",       name = "Shoes"          } },

                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 161;
            }
        }
        #endregion

        public SuperStrikeGameLogic()
        {
            _gameID     = GAMEID.SuperStrike;
            GameName    = "SuperStrike";
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);
            HabaneroHistoryResponses responses = _dicUserHistory[strUserId].Responses[currentIndex];
            dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            if (!object.ReferenceEquals(resultContext["currentfreegame"], null))
                eventItem["multiplier"] = resultContext["currentfreegame"];

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
