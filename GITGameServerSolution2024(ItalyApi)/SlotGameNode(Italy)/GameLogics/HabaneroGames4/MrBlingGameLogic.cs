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
    public class MrBlingGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGMrBling";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "7746bb3e-6cf5-4bd8-8f88-5d9800a8ba2e";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "db1ebec7b11eca4812c8aab8d34b4ee5dfeffb94";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idGangster",    name = "Gangster"   } },
                    {2,   new HabaneroLogSymbolIDName{id = "idDollarSign",  name = "DollarSign" } },
                    {3,   new HabaneroLogSymbolIDName{id = "idSidekick",    name = "Sidekick"   } },
                    {4,   new HabaneroLogSymbolIDName{id = "idRecord",      name = "Record"     } },
                    {5,   new HabaneroLogSymbolIDName{id = "idCar",         name = "Car"        } },
                    {6,   new HabaneroLogSymbolIDName{id = "idGirl",        name = "Girl"       } },
                    {7,   new HabaneroLogSymbolIDName{id = "idA",           name = "A"          } },
                    {8,   new HabaneroLogSymbolIDName{id = "idK",           name = "K"          } },
                    {9,   new HabaneroLogSymbolIDName{id = "idQ",           name = "Q"          } },
                    {10,  new HabaneroLogSymbolIDName{id = "idJ",           name = "J"          } },
                    {11,  new HabaneroLogSymbolIDName{id = "idTen",         name = "Ten"        } },
                    {12,  new HabaneroLogSymbolIDName{id = "idNine",        name = "Nine"       } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 125;
            }
        }
        #endregion

        public MrBlingGameLogic()
        {
            _gameID     = GAMEID.MrBling;
            GameName    = "MrBling";
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);
            HabaneroHistoryResponses responses = _dicUserHistory[strUserId].Responses[currentIndex];
            dynamic response = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            if (!object.ReferenceEquals(response["videoslotstate"]["gamemultiplier"], null))
            {
                int multiplier = Convert.ToInt32(response["videoslotstate"]["gamemultiplier"]);
                if (multiplier > 1)
                    multiplier -= 1;

                eventItem["multiplier"] = multiplier;
            }

            return eventItem;
        }
    }
}
