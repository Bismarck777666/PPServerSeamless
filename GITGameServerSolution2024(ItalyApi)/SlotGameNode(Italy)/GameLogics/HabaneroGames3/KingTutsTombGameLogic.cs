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
    public class KingTutsTombGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGKingTutsTomb";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "f7ad8edc-9e19-46f9-afa6-c7246c9d42cb";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "5011b3610b36eab051ecd356ed3f639124d8a643";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idWild",            name = "Wild"           } },
                    {2,   new HabaneroLogSymbolIDName{id = "idScatter",         name = "Scatter"        } },
                    {3,   new HabaneroLogSymbolIDName{id = "idPyramidsOfGold",  name = "PyramidsOfGold" } },
                    {4,   new HabaneroLogSymbolIDName{id = "idAnkh",            name = "Ankh"           } },
                    {5,   new HabaneroLogSymbolIDName{id = "idScarab",          name = "Scarab"         } },
                    {6,   new HabaneroLogSymbolIDName{id = "idCobra",           name = "Cobra"          } },
                    {7,   new HabaneroLogSymbolIDName{id = "idPotOfGold",       name = "PotOfGold"      } },
                    {8,   new HabaneroLogSymbolIDName{id = "idEye",             name = "Eye"            } },
                    {9,   new HabaneroLogSymbolIDName{id = "idA",               name = "A"              } },
                    {10,  new HabaneroLogSymbolIDName{id = "idK",               name = "K"              } },
                    {11,  new HabaneroLogSymbolIDName{id = "idQ",               name = "Q"              } },
                    {12,  new HabaneroLogSymbolIDName{id = "idJ",               name = "J"              } },
                    {13,  new HabaneroLogSymbolIDName{id = "id10",              name = "10"             } },
                    {14,  new HabaneroLogSymbolIDName{id = "id9",               name = "9"              } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 205;
            }
        }
        #endregion

        public KingTutsTombGameLogic()
        {
            _gameID     = GAMEID.KingTutsTomb;
            GameName    = "KingTutsTomb";
        }

        protected override JArray buildHabaneroLogReels(string strUserId, int currentIndex, dynamic response, bool containWild = false)
        {
            JArray reels = base.buildHabaneroLogReels(strUserId, currentIndex, response as JObject, containWild);
            if (object.ReferenceEquals(response["expandingwilds"], null) || response["expandingwilds"].Count == 0)
                return reels;

            for (int i = 0; i < response["expandingwilds"].Count; i++)
            {
                int reelindex = (int)response["expandingwilds"][i]["reelindex"];
                int symbolid = (int)response["expandingwilds"][i]["symbolid"];

                for (int j = 0; j < 3; j++)
                {
                    reels[reelindex][j] = SymbolIdStringForLog[symbolid].id;
                    reels[reelindex][j] = SymbolIdStringForLog[symbolid].id;
                }
            }
            return reels;
        }
    }
}
