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
    public class JungleRumbleGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGJungleRumble";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "883ede53-a43b-4a24-b99c-caf827040e30";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "548e599ebb5eba9dcbefe84183ce6bf5f5027bee";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idPygmy",       name = "Pygmy"      } },
                    {2,   new HabaneroLogSymbolIDName{id = "idCauldron",    name = "Cauldron"   } },    
                    {3,   new HabaneroLogSymbolIDName{id = "idMonkey",      name = "Monkey"     } },    
                    {4,   new HabaneroLogSymbolIDName{id = "idHut",         name = "Hut"        } },    
                    {5,   new HabaneroLogSymbolIDName{id = "idElephant",    name = "Elephant"   } },    
                    {6,   new HabaneroLogSymbolIDName{id = "idWarthog",     name = "Warthog"    } },
                    {7,   new HabaneroLogSymbolIDName{id = "idDrums",       name = "Drums"      } },
                    {8,   new HabaneroLogSymbolIDName{id = "idShield",      name = "Shield"     } },    
                    {9,   new HabaneroLogSymbolIDName{id = "idGun",         name = "Gun"        } },
                    {10,  new HabaneroLogSymbolIDName{id = "idNecklace",    name = "Necklace"   } },    
                    {11,  new HabaneroLogSymbolIDName{id = "idFlytrap",     name = "Flytrap"    } },
                    {12,  new HabaneroLogSymbolIDName{id = "idFlower",      name = "Flower"     } },

                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 163;
            }
        }
        #endregion

        public JungleRumbleGameLogic()
        {
            _gameID     = GAMEID.JungleRumble;
            GameName    = "JungleRumble";
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);
            HabaneroHistoryResponses responses = _dicUserHistory[strUserId].Responses[currentIndex];
            dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            if (!object.ReferenceEquals(resultContext["currentfreegame"], null))
            {
                if(resultContext["currentfreegame"] <= 10)
                    eventItem["multiplier"] = 2;
                else
                    eventItem["multiplier"] = 3;
            }

            return eventItem;
        }
    }
}
