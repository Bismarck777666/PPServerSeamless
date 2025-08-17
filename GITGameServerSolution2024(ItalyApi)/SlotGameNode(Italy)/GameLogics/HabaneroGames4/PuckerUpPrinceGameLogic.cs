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
    public class PuckerUpPrinceGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGPuckerUpPrince";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "8cf83d52-ad13-43db-90b0-9a0b4edf2769";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "a9a39f7ad8a3d731ba4a25af96bea219696aa7e1";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idPrincess",        name = "Princess"       } },
                    {2,   new HabaneroLogSymbolIDName{id = "idCrown",           name = "Crown"          } },
                    {3,   new HabaneroLogSymbolIDName{id = "idBonus",           name = "Bonus"          } },
                    {4,   new HabaneroLogSymbolIDName{id = "idKing",            name = "King"           } },
                    {5,   new HabaneroLogSymbolIDName{id = "idJester",          name = "Jester"         } },
                    {6,   new HabaneroLogSymbolIDName{id = "idTreasureChest",   name = "TreasureChest"  } },
                    {7,   new HabaneroLogSymbolIDName{id = "idGem",             name = "Gem"            } },
                    {8,   new HabaneroLogSymbolIDName{id = "idWell",            name = "Well"           } },
                    {9,   new HabaneroLogSymbolIDName{id = "idRing",            name = "Ring"           } },
                    {10,  new HabaneroLogSymbolIDName{id = "idMandolin",        name = "Mandolin"       } },
                    {11,  new HabaneroLogSymbolIDName{id = "idFish",            name = "Fish"           } },
                    {12,  new HabaneroLogSymbolIDName{id = "idWaterLilly",      name = "WaterLilly"     } },
                    {13,  new HabaneroLogSymbolIDName{id = "idMushroom",        name = "Mushroom"       } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 112;
            }
        }
        #endregion

        public PuckerUpPrinceGameLogic()
        {
            _gameID     = GAMEID.PuckerUpPrince;
            GameName    = "PuckerUpPrince";
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
