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
    public class TheKoiGateGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGTheKoiGate";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "259a7746-f940-44ee-946a-df7a92fdebd4";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "0285e92ae36cdf9689bfcd133bc2edc6105cdd92";
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
                return 18.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 18;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idKoi",     name = "Koi"    } },    //금붕어
                    {2,   new HabaneroLogSymbolIDName{id = "idDragon",  name = "Dragon" } },    //룡
                    {3,   new HabaneroLogSymbolIDName{id = "idGate",    name = "Gate"   } },    //게이트
                    {4,   new HabaneroLogSymbolIDName{id = "idFlower",  name = "Flower" } },    //꽃
                    {5,   new HabaneroLogSymbolIDName{id = "idBamboo",  name = "Bamboo" } },    //참대
                    {6,   new HabaneroLogSymbolIDName{id = "idLeaves",  name = "Leaves" } },    //나무잎
                    {7,   new HabaneroLogSymbolIDName{id = "idWave",    name = "Wave"   } },    //파도
                    {8,   new HabaneroLogSymbolIDName{id = "idA",       name = "A"      } },    //A
                    {9,   new HabaneroLogSymbolIDName{id = "idK",       name = "K"      } },    //K
                    {10,  new HabaneroLogSymbolIDName{id = "idQ",       name = "Q"      } },    //Q
                    {11,  new HabaneroLogSymbolIDName{id = "idJ",       name = "J"      } },    //J
                    {12,  new HabaneroLogSymbolIDName{id = "id10",      name = "10"     } },    //10
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 328;
            }
        }
        #endregion

        public TheKoiGateGameLogic()
        {
            _gameID     = GAMEID.TheKoiGate;
            GameName    = "TheKoiGate";
        }

        protected override JArray buildHabaneroLogReels(string strGlobalUserID,int currentIndex ,dynamic response, bool containWild = false)
        {
            dynamic currentResult   = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserID].Responses[currentIndex].Response);
            double currentWin       = Convert.ToDouble(currentResult["wincash"]);
            if(currentWin == 0)
                return base.buildHabaneroLogReels(strGlobalUserID, currentIndex, response as JObject, containWild);

            JArray logReels = new JArray();
            for (int i = 0; i < response["reels"].Count; i++)
            {
                bool containKoi = false;
                JArray col = new JArray();
                for (int j = 0; j < response["reels"][i].Count; j++)
                {
                    int symbol      = Convert.ToInt32(response["reels"][i][j]["symbolid"]);

                    if(symbol == 1)
                    {
                        containKoi = true;
                        break;
                    }
                }

                for (int j = 0; j < response["reels"][i].Count; j++)
                {
                    if (containKoi)
                        col.Add(SymbolIdStringForLog[1].id);
                    else
                    {
                        int symbol = Convert.ToInt32(response["reels"][i][j]["symbolid"]);
                        col.Add(SymbolIdStringForLog[symbol].id);
                    }
                }

                logReels.Add(col);
            }
            return logReels;
        }
    }
}
