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
    public class WaysOfFortuneGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGWaysOfFortune";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "b5eeae97-9314-4b07-9514-93afb67c198b";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "11209dfd139909f83043f6df4cbe517e2d9b373a";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.3133.234";
            }
        }
        protected override float MiniCoin
        {
            get
            {
                return 28.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 576;
            }
        }
        protected override string BetType {
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
                    {1,   new HabaneroLogSymbolIDName{id = "idWild",    name = "Wild"       } },    
                    {2,   new HabaneroLogSymbolIDName{id = "idScatter", name = "Scatter"    } },    
                    {3,   new HabaneroLogSymbolIDName{id = "idLion",    name = "Lion"       } },    
                    {4,   new HabaneroLogSymbolIDName{id = "idDragon",  name = "Dragon"     } },    
                    {5,   new HabaneroLogSymbolIDName{id = "idHelmet",  name = "Helmet"     } },    
                    {6,   new HabaneroLogSymbolIDName{id = "idSword",   name = "Sword"      } },    
                    {7,   new HabaneroLogSymbolIDName{id = "idHeart",   name = "Heart"      } },    
                    {8,   new HabaneroLogSymbolIDName{id = "idSpade",   name = "Spade"      } },    
                    {9,   new HabaneroLogSymbolIDName{id = "idClub",    name = "Club"       } },    
                    {10,  new HabaneroLogSymbolIDName{id = "idDiamond", name = "Diamond"    } },    
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 379;
            }
        }
        #endregion

        public WaysOfFortuneGameLogic()
        {
            _gameID     = GAMEID.WaysOfFortune;
            GameName    = "WaysOfFortune";
        }

        protected override JObject buildEventItem(string strGlobalUserID, int currentIndex)
        {
            dynamic response    = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserID].Responses[currentIndex].Response);
            dynamic eventItem   = base.buildEventItem(strGlobalUserID, currentIndex);

            if (!object.ReferenceEquals(response["currentfreegame"], null))
                eventItem["multiplier"] = 1;

            return eventItem;
        }
    }
}
