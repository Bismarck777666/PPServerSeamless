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
    public class GoldRushGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGGoldRush";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "a981909f-1f4a-4d32-b9a0-254c42665b08";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "cdc4afc456485cf8b845bd64630ba3edfa961e75";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idNugget",      name = "Nugget"     } },
                    {2,   new HabaneroLogSymbolIDName{id = "idDynamite",    name = "Dynamite"   } },
                    {3,   new HabaneroLogSymbolIDName{id = "idMine",        name = "Mine"       } },
                    {4,   new HabaneroLogSymbolIDName{id = "idDigger",      name = "Digger"     } },
                    {5,   new HabaneroLogSymbolIDName{id = "idRobber",      name = "Robber"     } },
                    {6,   new HabaneroLogSymbolIDName{id = "idDonkey",      name = "Donkey"     } },
                    {7,   new HabaneroLogSymbolIDName{id = "idCart",        name = "Cart"       } },
                    {8,   new HabaneroLogSymbolIDName{id = "idBarrel",      name = "Barrel"     } },
                    {9,   new HabaneroLogSymbolIDName{id = "idDog",         name = "Dog"        } },
                    {10,  new HabaneroLogSymbolIDName{id = "idPan",         name = "Pan"        } },
                    {11,  new HabaneroLogSymbolIDName{id = "idPick",        name = "Pick"       } },
                    {12,  new HabaneroLogSymbolIDName{id = "idLantern",     name = "Lantern"    } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 286;
            }
        }
        #endregion

        public GoldRushGameLogic()
        {
            _gameID     = GAMEID.GoldRush;
            GameName    = "GoldRush";
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);
            if ((string)eventItem["gamemode"] == "FREEGAME")
            {
                eventItem["multiplier"] = 1;
            }
            return eventItem;
        }
    }
}
