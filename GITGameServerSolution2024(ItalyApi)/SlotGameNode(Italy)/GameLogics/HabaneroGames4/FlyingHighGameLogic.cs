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
    public class FlyingHighGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGFlyingHigh";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "621729cc-2d9c-433f-a4b4-c073b7b37dfe";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "36ff3b981dcfbaf9a9c0f46a49669bb2a0d480fb";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idPlane",       name = "Plane"          } },
                    {2,   new HabaneroLogSymbolIDName{id = "idEmblem",      name = "Emblem"         } },
                    {3,   new HabaneroLogSymbolIDName{id = "idStewardess",  name = "Stewardess"     } },
                    {4,   new HabaneroLogSymbolIDName{id = "idCaptain",     name = "Captain"        } },
                    {5,   new HabaneroLogSymbolIDName{id = "idPassenger",   name = "Passenger"      } },
                    {6,   new HabaneroLogSymbolIDName{id = "idSuitcase",    name = "Suitcase"       } },
                    {7,   new HabaneroLogSymbolIDName{id = "idCamera",      name = "Camera"         } },
                    {8,   new HabaneroLogSymbolIDName{id = "idPassport",    name = "Passport"       } },
                    {9,   new HabaneroLogSymbolIDName{id = "idTickets",     name = "Tickets"        } },
                    {10,  new HabaneroLogSymbolIDName{id = "idFood",        name = "Food"           } },
                    {11,  new HabaneroLogSymbolIDName{id = "idNewspaper",   name = "Newspaper"      } },
                    {12,  new HabaneroLogSymbolIDName{id = "idMask",        name = "Mask"           } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 123;
            }
        }
        #endregion

        public FlyingHighGameLogic()
        {
            _gameID     = GAMEID.FlyingHigh;
            GameName    = "FlyingHigh";
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);
            HabaneroHistoryResponses responses = _dicUserHistory[strUserId].Responses[currentIndex];
            dynamic response = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            if (!object.ReferenceEquals(response["videoslotstate"]["gamemultiplier"], null))
                eventItem["multiplier"] = response["videoslotstate"]["gamemultiplier"];

            return eventItem;
        }
    }
}
