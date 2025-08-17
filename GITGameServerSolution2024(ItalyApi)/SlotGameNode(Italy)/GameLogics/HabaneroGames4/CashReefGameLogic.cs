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
    public class CashReefGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGCashReef";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "7fc5a6da-473a-4742-ba9c-d4e50e52612d";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "264e8b78b4cff6f0b2317ab01fd1e8d2624062eb";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idWildPinkClam",    name = "WildPinkClam"   } },
                    {2,   new HabaneroLogSymbolIDName{id = "idTreasureChest",   name = "TreasureChest"  } },
                    {3,   new HabaneroLogSymbolIDName{id = "idBlondMermaid",    name = "BlondMermaid"   } },
                    {4,   new HabaneroLogSymbolIDName{id = "idMan",             name = "Man"            } },
                    {5,   new HabaneroLogSymbolIDName{id = "idSphinx",          name = "Sphinx"         } },
                    {6,   new HabaneroLogSymbolIDName{id = "idStarfish",        name = "Starfish"       } },
                    {7,   new HabaneroLogSymbolIDName{id = "idTurtle",          name = "Turtle"         } },
                    {8,   new HabaneroLogSymbolIDName{id = "idOctupus",         name = "Octupus"        } },
                    {9,   new HabaneroLogSymbolIDName{id = "idEel",             name = "Eel"            } },
                    {10,  new HabaneroLogSymbolIDName{id = "idMoosehorn",       name = "Moosehorn"      } },
                    {11,  new HabaneroLogSymbolIDName{id = "idCrown",           name = "Crown"          } },
                    {12,  new HabaneroLogSymbolIDName{id = "idUrn",             name = "Urn"            } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 212;
            }
        }
        #endregion

        public CashReefGameLogic()
        {
            _gameID     = GAMEID.CashReef;
            GameName    = "CashReef";
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
