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
    public class MysticFortuneGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGMysticFortune";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "6c35cd43-894c-491d-bb24-170b36cd1e5a";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "45b67ce211992ece5c04633901642b29ef53aac0";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idBird",        name = "Bird"       } },
                    {2,   new HabaneroLogSymbolIDName{id = "idPrincess",    name = "Princess"   } },
                    {3,   new HabaneroLogSymbolIDName{id = "idGlyph",       name = "Glyph"      } },
                    {4,   new HabaneroLogSymbolIDName{id = "idLion",        name = "Lion"       } },
                    {5,   new HabaneroLogSymbolIDName{id = "idUrn",         name = "Urn"        } },
                    {6,   new HabaneroLogSymbolIDName{id = "idLantern",     name = "Lantern"    } },
                    {7,   new HabaneroLogSymbolIDName{id = "idA",           name = "A"          } },
                    {8,   new HabaneroLogSymbolIDName{id = "idK",           name = "K"          } },
                    {9,   new HabaneroLogSymbolIDName{id = "idQ",           name = "Q"          } },
                    {10,  new HabaneroLogSymbolIDName{id = "idJ",           name = "J"          } },
                    {11,  new HabaneroLogSymbolIDName{id = "id10",          name = "10"         } },
                    {12,  new HabaneroLogSymbolIDName{id = "idNine",         name = "Nine"      } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 127;
            }
        }
        #endregion

        public MysticFortuneGameLogic()
        {
            _gameID     = GAMEID.MysticFortune;
            GameName    = "MysticFortune";
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
