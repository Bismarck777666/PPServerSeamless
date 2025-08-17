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
    public class TheBigDealGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGTheBigDeal";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "51f78713-4f56-4168-af4f-c09e01c9179d";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "efcb5b384ada59f0f87d527aa7e3607804c36669";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idCardsharp",       name = "Cardsharp"      } },
                    {2,   new HabaneroLogSymbolIDName{id = "idCard",            name = "Card"           } },
                    {3,   new HabaneroLogSymbolIDName{id = "idCasino",          name = "Casino"         } },
                    {4,   new HabaneroLogSymbolIDName{id = "idGirl",            name = "Girl"           } },
                    {5,   new HabaneroLogSymbolIDName{id = "idSecurityGuard",   name = "SecurityGuard"  } },
                    {6,   new HabaneroLogSymbolIDName{id = "idCar",             name = "Car"            } },
                    {7,   new HabaneroLogSymbolIDName{id = "idChips",           name = "Chips"          } },
                    {8,   new HabaneroLogSymbolIDName{id = "idCash",            name = "Cash"           } },
                    {9,   new HabaneroLogSymbolIDName{id = "idWatch",           name = "Watch"          } },
                    {10,  new HabaneroLogSymbolIDName{id = "idRing",            name = "Ring"           } },
                    {11,  new HabaneroLogSymbolIDName{id = "idCufflinks",       name = "Cufflinks"      } },
                    {12,  new HabaneroLogSymbolIDName{id = "idMartini",         name = "Martini"        } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 83;
            }
        }
        #endregion

        public TheBigDealGameLogic()
        {
            _gameID     = GAMEID.TheBigDeal;
            GameName    = "TheBigDeal";
        }
    }
}
