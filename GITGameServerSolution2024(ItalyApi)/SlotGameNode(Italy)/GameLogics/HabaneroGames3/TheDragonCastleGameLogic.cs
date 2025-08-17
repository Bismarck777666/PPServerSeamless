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
    public class TheDragonCastleGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGTheDragonCastle";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "ba406f06-faa7-446b-97e0-809e124a35b6";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "616e77711a4c9142ae4d719d422262a73a9a1fe1";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.1449.118";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idWarrior",     name = "Warrior"        } },
                    {2,   new HabaneroLogSymbolIDName{id = "idCrystalBall", name = "CrystalBall"    } },
                    {3,   new HabaneroLogSymbolIDName{id = "idUnicorn",     name = "Unicorn"        } },
                    {4,   new HabaneroLogSymbolIDName{id = "idDragon",      name = "Dragon"         } },
                    {5,   new HabaneroLogSymbolIDName{id = "idJewel",       name = "Jewel"          } },
                    {6,   new HabaneroLogSymbolIDName{id = "idDarkKnight",  name = "DarkKnight"     } },
                    {7,   new HabaneroLogSymbolIDName{id = "idLady",        name = "Lady"           } },
                    {8,   new HabaneroLogSymbolIDName{id = "idShield",      name = "Shield"         } },
                    {9,   new HabaneroLogSymbolIDName{id = "idGoblin",      name = "Goblin"         } },
                    {10,  new HabaneroLogSymbolIDName{id = "idA",           name = "A"              } },
                    {11,  new HabaneroLogSymbolIDName{id = "idK",           name = "K"              } },
                    {12,  new HabaneroLogSymbolIDName{id = "idQ",           name = "Q"              } },
                    {13,  new HabaneroLogSymbolIDName{id = "idJ",           name = "J"              } },
                    {14,  new HabaneroLogSymbolIDName{id = "id10",          name = "10"             } },
                    {15,  new HabaneroLogSymbolIDName{id = "id9",           name = "9"              } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 200;
            }
        }
        #endregion

        public TheDragonCastleGameLogic()
        {
            _gameID     = GAMEID.TheDragonCastle;
            GameName    = "TheDragonCastle";
        }

        protected override JArray buildHabaneroLogReels(string strUserId, int currentIndex, dynamic response, bool containWild = false)
        {
            JArray reels = base.buildHabaneroLogReels(strUserId, currentIndex, response as JObject, containWild);
            if (object.ReferenceEquals(response["expandingwilds"], null) || response["expandingwilds"].Count == 0)
                return reels;

            for (int i = 0; i < response["expandingwilds"].Count; i++)
            {
                int reelindex   = (int)response["expandingwilds"][i]["reelindex"];
                int symbolid    = (int)response["expandingwilds"][i]["symbolid"];

                for(int j = 0; j < 3; j++)
                {
                    reels[reelindex][j] = SymbolIdStringForLog[symbolid].id;
                    reels[reelindex][j] = SymbolIdStringForLog[symbolid].id;
                }
            }
            return reels;
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);
            HabaneroHistoryResponses responses = _dicUserHistory[strUserId].Responses[currentIndex];
            dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            if (!object.ReferenceEquals(resultContext["currentfreegame"], null))
                eventItem["multiplier"] = 3;

            return eventItem;
        }
    }
}
