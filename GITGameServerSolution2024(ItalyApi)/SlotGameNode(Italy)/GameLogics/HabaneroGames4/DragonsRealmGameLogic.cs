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
    public class DragonsRealmGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGDragonsRealm";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "65d1a0de-9b3b-4524-b7cb-1c681d8115cc";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "1bc07aa3fd10d66aa28c2626dff75c6f7dfad766";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idDragon",      name = "Dragon"     } },
                    {2,   new HabaneroLogSymbolIDName{id = "idShield",      name = "Shield"     } },
                    {3,   new HabaneroLogSymbolIDName{id = "idCastle",      name = "Castle"     } },
                    {4,   new HabaneroLogSymbolIDName{id = "idKnight",      name = "Knight"     } },
                    {5,   new HabaneroLogSymbolIDName{id = "idMaiden",      name = "Maiden"     } },
                    {6,   new HabaneroLogSymbolIDName{id = "idHorse",       name = "Horse"      } },
                    {7,   new HabaneroLogSymbolIDName{id = "idTreasure",    name = "Treasure"   } },
                    {8,   new HabaneroLogSymbolIDName{id = "idGoblet",      name = "Goblet"     } },
                    {9,   new HabaneroLogSymbolIDName{id = "idDagger",      name = "Dagger"     } },
                    {10,  new HabaneroLogSymbolIDName{id = "idA",           name = "A"          } },
                    {11,  new HabaneroLogSymbolIDName{id = "idK",           name = "K"          } },
                    {12,  new HabaneroLogSymbolIDName{id = "idQ",           name = "Q"          } },
                    {13,  new HabaneroLogSymbolIDName{id = "idJ",           name = "J"          } },
                    {14,  new HabaneroLogSymbolIDName{id = "idTen",         name = "Ten"        } },
                    {15,  new HabaneroLogSymbolIDName{id = "idNine",        name = "Nine"       } },
                    {16,  new HabaneroLogSymbolIDName{id = "idGoldDragon",  name = "GoldDragon" } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 118;
            }
        }
        #endregion

        public DragonsRealmGameLogic()
        {
            _gameID     = GAMEID.DragonsRealm;
            GameName    = "DragonsRealm";
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

        protected override JArray buildHabaneroLogReels(string strUserId, int currentIndex, dynamic response, bool containWild = false)
        {
            dynamic reels = base.buildHabaneroLogReels(strUserId, currentIndex, response as JObject, containWild);
            if (!object.ReferenceEquals(response["videoslotstate"]["expandingwildlist"], null) && response["videoslotstate"]["expandingwildlist"].Count > 0)
            {
                for (int i = 0; i < response["videoslotstate"]["expandingwildlist"].Count; i++)
                {
                    int reelIndex   = (int)response["videoslotstate"]["expandingwildlist"][i]["reelindex"];
                    int symbolId    = (int)response["videoslotstate"]["expandingwildlist"][i]["symbolid"];
                    for (int j = 0; j < 3; j++)
                        reels[reelIndex][j] = SymbolIdStringForLog[symbolId].id;
                }
            }
            return reels;
        }
    }
}
