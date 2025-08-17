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
    public class QueenOfQueens243GameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGQueenOfQueens243";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "a01aef3f-c794-481d-98b8-030bac6e5fa8";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "7f6f9a47427f43f25152c49a90bdf4ab83647aef";
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
                return 30.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 243;
            }
        }
        protected override string BetType
        {
            get
            {
                return "Ways";
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idQueen",       name = "Queen"      } },
                    {2,   new HabaneroLogSymbolIDName{id = "idUrn",         name = "Urn"        } },
                    {3,   new HabaneroLogSymbolIDName{id = "idJackpot",     name = "Jackpot"    } },
                    {4,   new HabaneroLogSymbolIDName{id = "idCat",         name = "Cat"        } },
                    {5,   new HabaneroLogSymbolIDName{id = "idEagle",       name = "Eagle"      } },
                    {6,   new HabaneroLogSymbolIDName{id = "idSceptor",     name = "Sceptor"    } },
                    {7,   new HabaneroLogSymbolIDName{id = "idScarab",      name = "Scarab"     } },
                    {8,   new HabaneroLogSymbolIDName{id = "idRing",        name = "Ring"       } },
                    {9,   new HabaneroLogSymbolIDName{id = "idNecklace",    name = "Necklace"   } },
                    {10,  new HabaneroLogSymbolIDName{id = "idRedGem",      name = "RedGem"     } },
                    {11,  new HabaneroLogSymbolIDName{id = "idPurpleGem",   name = "PurpleGem"  } },
                    {12,  new HabaneroLogSymbolIDName{id = "idPinkGem",     name = "PinkGem"    } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 216;
            }
        }
        #endregion

        public QueenOfQueens243GameLogic()
        {
            _gameID     = GAMEID.QueenOfQueens243;
            GameName    = "QueenOfQueens243";
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
