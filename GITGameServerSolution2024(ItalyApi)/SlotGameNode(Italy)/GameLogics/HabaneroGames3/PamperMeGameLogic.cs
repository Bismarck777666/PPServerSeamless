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
    public class PamperMeGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGPamperMe";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "5f04c4cd-434c-4e3f-ab47-387a25019206";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "68bca05dbfcc8a5ee8341d04ae3bbfd3cf206c40";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idLady",        name = "Lady"       } },
                    {2,   new HabaneroLogSymbolIDName{id = "idDog",         name = "Dog"        } },
                    {3,   new HabaneroLogSymbolIDName{id = "idChocolates",  name = "Chocolates" } },
                    {4,   new HabaneroLogSymbolIDName{id = "idRing",        name = "Ring"       } },
                    {5,   new HabaneroLogSymbolIDName{id = "idPerfume",     name = "Perfume"    } },
                    {6,   new HabaneroLogSymbolIDName{id = "idShoes",       name = "Shoes"      } },
                    {7,   new HabaneroLogSymbolIDName{id = "idHairbrush",   name = "Hairbrush"  } },
                    {8,   new HabaneroLogSymbolIDName{id = "idBags",        name = "Bags"       } },
                    {9,   new HabaneroLogSymbolIDName{id = "idLipstick",    name = "Lipstick"   } },
                    {10,  new HabaneroLogSymbolIDName{id = "idFlowers",     name = "Flowers"    } },
                    {11,  new HabaneroLogSymbolIDName{id = "idEarrings",    name = "Earrings"   } },
                    {12,  new HabaneroLogSymbolIDName{id = "idLetter",      name = "Letter"     } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 167;
            }
        }
        #endregion

        public PamperMeGameLogic()
        {
            _gameID     = GAMEID.PamperMe;
            GameName    = "PamperMe";
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
