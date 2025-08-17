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
    public class AllForOneGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGAllForOne";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "af59b911-3e55-4d15-acf1-28176b19582b";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "a8440157cbbb02851b63eaa53ba2df43d1a0258d";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idShieldofHonor",   name = "ShieldofHonor"  } },
                    {2,   new HabaneroLogSymbolIDName{id = "idDArtagnan",       name = "DArtagnan"      } },
                    {3,   new HabaneroLogSymbolIDName{id = "idPorthos",         name = "Porthos"        } },
                    {4,   new HabaneroLogSymbolIDName{id = "idAthos",           name = "Athos"          } },
                    {5,   new HabaneroLogSymbolIDName{id = "idCastle",          name = "Castle"         } },
                    {6,   new HabaneroLogSymbolIDName{id = "idGirl",            name = "Girl"           } },
                    {7,   new HabaneroLogSymbolIDName{id = "idCrown",           name = "Crown"          } },
                    {8,   new HabaneroLogSymbolIDName{id = "idGoldCoins",       name = "GoldCoins"      } },
                    {9,   new HabaneroLogSymbolIDName{id = "idMusket",          name = "Musket"         } },
                    {10,  new HabaneroLogSymbolIDName{id = "idTrumpet",         name = "Trumpet"        } },
                    {11,  new HabaneroLogSymbolIDName{id = "idMug",             name = "Mug"            } },
                    {12,  new HabaneroLogSymbolIDName{id = "idFruit",           name = "Fruit"          } },
                    
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 121;
            }
        }
        #endregion

        public AllForOneGameLogic()
        {
            _gameID     = GAMEID.AllForOne;
            GameName    = "AllForOne";
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
