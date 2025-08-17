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
    public class HaZeusGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGZeus";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "d155bf40-3a90-42c5-9ca0-8c7f5d12f0e1";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "5622eb293c649736c6c8ea1dc434a99685dbb5d8";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idZeus",    name = "Zeus"   } },
                    {2,   new HabaneroLogSymbolIDName{id = "idTemple",  name = "Temple" } },
                    {3,   new HabaneroLogSymbolIDName{id = "idHera",    name = "Hera"   } },
                    {4,   new HabaneroLogSymbolIDName{id = "idShield",  name = "Shield" } },
                    {5,   new HabaneroLogSymbolIDName{id = "idVase",    name = "Vase"   } },
                    {6,   new HabaneroLogSymbolIDName{id = "idStatue",  name = "Statue" } },
                    {7,   new HabaneroLogSymbolIDName{id = "idA",       name = "A"      } },
                    {8,   new HabaneroLogSymbolIDName{id = "idK",       name = "K"      } },
                    {9,   new HabaneroLogSymbolIDName{id = "idQ",       name = "Q"      } },
                    {10,  new HabaneroLogSymbolIDName{id = "idJ",       name = "J"      } },
                    {11,  new HabaneroLogSymbolIDName{id = "id10",      name = "10"     } },
                    {12,  new HabaneroLogSymbolIDName{id = "id9",       name = "9"      } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 169;
            }
        }
        #endregion

        public HaZeusGameLogic()
        {
            _gameID     = GAMEID.HaZeus;
            GameName    = "HaZeus";
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
