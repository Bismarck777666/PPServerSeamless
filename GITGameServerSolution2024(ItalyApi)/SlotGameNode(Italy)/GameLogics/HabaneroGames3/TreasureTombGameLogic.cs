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
    public class TreasureTombGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGTreasureTomb";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "0dd01b41-d0e4-431c-a5d3-495452995dd2";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "0e3336f379589f0dfc1b3fc1d7cd1125acff8718";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idMummy",           name = "Mummy"          } },
                    {2,   new HabaneroLogSymbolIDName{id = "idPyramid",         name = "Pyramid"        } },
                    {3,   new HabaneroLogSymbolIDName{id = "idOasis",           name = "Oasis"          } },
                    {4,   new HabaneroLogSymbolIDName{id = "idPharoah",         name = "Pharoah"        } },
                    {5,   new HabaneroLogSymbolIDName{id = "idPharoahsQueen",   name = "PharoahsQueen"  } },
                    {6,   new HabaneroLogSymbolIDName{id = "idHorus",           name = "Horus"          } },
                    {7,   new HabaneroLogSymbolIDName{id = "idEye",             name = "Eye"            } },
                    {8,   new HabaneroLogSymbolIDName{id = "idJewel",           name = "Jewel"          } },
                    {9,   new HabaneroLogSymbolIDName{id = "idAce",             name = "Ace"            } },
                    {10,  new HabaneroLogSymbolIDName{id = "idKing",            name = "King"           } },
                    {11,  new HabaneroLogSymbolIDName{id = "idQueen",           name = "Queen"          } },
                    {12,  new HabaneroLogSymbolIDName{id = "idJack",            name = "Jack"           } },
                    {13,  new HabaneroLogSymbolIDName{id = "idTen",             name = "Ten"            } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 236;
            }
        }
        #endregion

        public TreasureTombGameLogic()
        {
            _gameID     = GAMEID.TreasureTomb;
            GameName    = "TreasureTomb";
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
