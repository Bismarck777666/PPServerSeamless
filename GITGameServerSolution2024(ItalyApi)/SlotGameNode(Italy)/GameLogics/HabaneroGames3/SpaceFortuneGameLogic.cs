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
    public class SpaceFortuneGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGSpaceFortune";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "aa217f8b-bd01-4d63-a68e-11e28c3aff21";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "88c5afab3fcb98425f4f1d4631bc3e96935ec0aa";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idHero",        name = "Hero"       } },
                    {2,   new HabaneroLogSymbolIDName{id = "idSpaceport",   name = "Spaceport"  } },
                    {3,   new HabaneroLogSymbolIDName{id = "idSpacegirl",   name = "Spacegirl"  } },
                    {4,   new HabaneroLogSymbolIDName{id = "idGreenalien",  name = "Greenalien" } },
                    {5,   new HabaneroLogSymbolIDName{id = "idCat",         name = "Cat"        } },
                    {6,   new HabaneroLogSymbolIDName{id = "idDog",         name = "Dog"        } },
                    {7,   new HabaneroLogSymbolIDName{id = "idBluealien",   name = "Bluealien"  } },
                    {8,   new HabaneroLogSymbolIDName{id = "idSpaceship",   name = "Spaceship"  } },
                    {9,   new HabaneroLogSymbolIDName{id = "idPlanet",      name = "Planet"     } },
                    {10,  new HabaneroLogSymbolIDName{id = "idBurger",      name = "Burger"     } },
                    {11,  new HabaneroLogSymbolIDName{id = "idMilkshake",   name = "Milkshake"  } },
                    {12,  new HabaneroLogSymbolIDName{id = "idRaygun",      name = "Raygun"     } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 165;
            }
        }
        #endregion

        public SpaceFortuneGameLogic()
        {
            _gameID     = GAMEID.SpaceFortune;
            GameName    = "SpaceFortune";
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);
            HabaneroHistoryResponses responses = _dicUserHistory[strUserId].Responses[currentIndex];
            dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            if (!object.ReferenceEquals(resultContext["currentfreegame"], null))
                eventItem["multiplier"] = 2;

            return eventItem;
        }
    }
}
