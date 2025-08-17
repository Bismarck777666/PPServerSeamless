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
    public class TreasureDiverGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGTreasureDiver";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "e2198148-fe9c-4111-ae28-a5d75b1ed9be";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "f32630dad198b7625f4bcc9d76ef8028bef456cc";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idDiver",       name = "Diver"      } },
                    {2,   new HabaneroLogSymbolIDName{id = "idChest",       name = "Chest"      } },    
                    {3,   new HabaneroLogSymbolIDName{id = "idMermaid",     name = "Mermaid"    } },
                    {4,   new HabaneroLogSymbolIDName{id = "idShark",       name = "Shark"      } },    
                    {5,   new HabaneroLogSymbolIDName{id = "idClam",        name = "Clam"       } },
                    {6,   new HabaneroLogSymbolIDName{id = "idShip",        name = "Ship"       } },
                    {7,   new HabaneroLogSymbolIDName{id = "idAnchor",      name = "Anchor"     } },
                    {8,   new HabaneroLogSymbolIDName{id = "idFish",        name = "Fish"       } },
                    {9,   new HabaneroLogSymbolIDName{id = "idGoggles",     name = "Goggles"    } },    
                    {10,  new HabaneroLogSymbolIDName{id = "idKnives",      name = "Knives"     } },
                    {11,  new HabaneroLogSymbolIDName{id = "idWheel",       name = "Wheel"      } },
                    {12,  new HabaneroLogSymbolIDName{id = "idSeahorse",    name = "Seahorse"   } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 238;
            }
        }
        #endregion

        public TreasureDiverGameLogic()
        {
            _gameID     = GAMEID.TreasureDiver;
            GameName    = "TreasureDiver";
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
