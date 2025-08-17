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
    public class WealthInnGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGWealthInn";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "3ba13048-defc-47ca-b624-1e555ed6d81e";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "0d74ae95978160fe13d09e21b6da71394022b052";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.6453.344";
            }
        }
        protected override float MiniCoin
        {
            get
            {
                return 8.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 8;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idWild",        name = "Wild"           } },    //와일드
                    {2,   new HabaneroLogSymbolIDName{id = "idPeach",       name = "Peach"          } },    //복숭아
                    {3,   new HabaneroLogSymbolIDName{id = "idRuyi",        name = "Ruyi"           } },    //루이
                    {4,   new HabaneroLogSymbolIDName{id = "idIngot",       name = "Ingot"          } },    //금낭
                    {5,   new HabaneroLogSymbolIDName{id = "idLow1",        name = "Low1"           } },    //노랑
                    {6,   new HabaneroLogSymbolIDName{id = "idLow2",        name = "Low2"           } },    //빨강
                    {7,   new HabaneroLogSymbolIDName{id = "idLow3",        name = "Low3"           } },    //파랑
                    {8,   new HabaneroLogSymbolIDName{id = "idLowCombined", name = "LowCombined"    } },    //애니
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 518;
            }
        }
        #endregion

        public WealthInnGameLogic()
        {
            _gameID     = GAMEID.WealthInn;
            GameName    = "WealthInn";
        }

        protected override JObject buildEventItem(string strGlobalUserID, int currentIndex)
        {
            dynamic response    = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserID].Responses[currentIndex].Response);
            dynamic eventItem   = base.buildEventItem(strGlobalUserID, currentIndex);

            if(!object.ReferenceEquals(response["expandingwilds"],null) && response["expandingwilds"].Count > 0)
                eventItem["reelslist"] = buildHabaneroLogReelslist(response);
            
            return eventItem;
        }

        protected override JArray buildHabaneroLogReelslist(dynamic response)
        {
            JArray reelslist        = new JArray();
            JObject reelslistItem   = new JObject();
            JArray reels            = new JArray();

            for (int j = 0; j < response["virtualreels"].Count; j++)
            {
                JArray col = new JArray();
                for (int k = 2; k < response["virtualreels"][j].Count - 2; k++)
                {
                    int symbol      = Convert.ToInt32(response["virtualreels"][j][k]);
                    string symbolid = SymbolIdStringForLog[symbol].id;
                    col.Add(symbolid);
                }
                reels.Add(col);
            }
            reelslistItem["reels"] = reels;
            reelslist.Add(reelslistItem);

            return reelslist;
        }

        protected override JArray buildHabaneroLogReels(string strGlobalUserID,int currentIndex ,dynamic response, bool containWild = false)
        {
            JArray logReels = base.buildHabaneroLogReels(strGlobalUserID, currentIndex, response as JObject, containWild);
            
            if (!object.ReferenceEquals(response["expandingwilds"], null) && response["expandingwilds"].Count > 0)
            {
                for(int i = 0; i < response["expandingwilds"].Count; i++)
                {
                    int reelIndex = response["expandingwilds"][i]["reelindex"];
                    for(int j = 0; j < 3; j++)
                        logReels[reelIndex][j] = "idWildExpand";
                }
            }
            return logReels;
        }
    }
}
