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
    public class PoolSharkGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGPoolShark";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "827d2608-7af4-4e9c-92e2-efb2197cab3e";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "87634cf5ab51f5e88a2e82c0858eb98408315188";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idPoolShark",   name = "PoolShark"  } },
                    {2,   new HabaneroLogSymbolIDName{id = "idBlackBall",   name = "BlackBall"  } },
                    {3,   new HabaneroLogSymbolIDName{id = "idHammerhead",  name = "Hammerhead" } },
                    {4,   new HabaneroLogSymbolIDName{id = "idWaiter",      name = "Waiter"     } },
                    {5,   new HabaneroLogSymbolIDName{id = "idBalls",       name = "Balls"      } },
                    {6,   new HabaneroLogSymbolIDName{id = "idCocktail",    name = "Cocktail"   } },
                    {7,   new HabaneroLogSymbolIDName{id = "idChalk",       name = "Chalk"      } },
                    {8,   new HabaneroLogSymbolIDName{id = "idA",           name = "A"          } },
                    {9,   new HabaneroLogSymbolIDName{id = "idK",           name = "K"          } },
                    {10,  new HabaneroLogSymbolIDName{id = "idQ",           name = "Q"          } },
                    {11,  new HabaneroLogSymbolIDName{id = "idJ",           name = "J"          } },
                    {12,  new HabaneroLogSymbolIDName{id = "id10",          name = "10"         } },
                    {13,  new HabaneroLogSymbolIDName{id = "id9",           name = "9"          } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 173;
            }
        }
        #endregion

        public PoolSharkGameLogic()
        {
            _gameID     = GAMEID.PoolShark;
            GameName    = "PoolShark";
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);
            HabaneroHistoryResponses responses = _dicUserHistory[strUserId].Responses[currentIndex];
            dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            if (!object.ReferenceEquals(resultContext["freegamemultiplier"], null))
                eventItem["multiplier"] = resultContext["freegamemultiplier"];

            return eventItem;
        }
    }
}
