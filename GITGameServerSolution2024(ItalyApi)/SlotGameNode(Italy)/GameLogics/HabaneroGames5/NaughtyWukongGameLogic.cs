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
    public class NaughtyWukongGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGNaughtyWukong";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "6c6ca45b-89da-47dc-ab73-b81106a98056";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "f38841a49ca63aa92cf36b2cd1d1cefcb42f3193";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.10938.0";
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
                return 27;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,     new HabaneroLogSymbolIDName{id = "idWild",          name = "Wild"           } },
                    {2,     new HabaneroLogSymbolIDName{id = "idWukong",        name = "Wukong"         } },
                    {3,     new HabaneroLogSymbolIDName{id = "idGoldPagoda",    name = "GoldPagoda"     } },
                    {4,     new HabaneroLogSymbolIDName{id = "idBuddhaPalm",    name = "BuddhaPalm"     } },
                    {5,     new HabaneroLogSymbolIDName{id = "id3Peach",        name = "3Peach"         } },
                    {6,     new HabaneroLogSymbolIDName{id = "id2Peach",        name = "2Peach"         } },
                    {7,     new HabaneroLogSymbolIDName{id = "id1Peach",        name = "1Peach"         } },
                    {8,     new HabaneroLogSymbolIDName{id = "idLowCombined",   name = "LowCombined"    } },
                    {9,     new HabaneroLogSymbolIDName{id = "idLowCombined",   name = "LowCombined"    } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 763;
            }
        }
        protected override string BetType
        {
            get
            {
                return "Ways";
            }
        }
        #endregion

        public NaughtyWukongGameLogic()
        {
            _gameID     = GAMEID.NaughtyWukong;
            GameName    = "NaughtyWukong";
        }

        protected override JObject buildEventItem(string strGlobalUserID, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strGlobalUserID, currentIndex);
            
            HabaneroHistoryResponses responses = _dicUserHistory[strGlobalUserID].Responses[currentIndex];
            dynamic response = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            if (!object.ReferenceEquals(response["wukongPos"], null))
            {
                JArray reels1           = buildHabaneroLogReels(strGlobalUserID, currentIndex, response);
                
                JArray reelslist        = new JArray();
                JObject reelslistitem   = new JObject();
                for (int i = 0; i < response["reels"].Count; i++)
                {
                    for (int j = 0; j < response["reels"][i].Count; j++)
                    {
                        reels1[i][j] = null;
                    }
                }
                
                int reelindex   = response["wukongPos"]["reelindex"];
                int symbolindex = response["wukongPos"]["symbolindex"];
                reels1[reelindex][symbolindex] = SymbolIdStringForLog[2].id;
                reelslistitem["reels"] = reels1;
                reelslist.Add(reelslistitem);

                eventItem["reelslist"]  = reelslist;
            }

            if (!object.ReferenceEquals(response["wukongPos"], null))
            {
                JObject replay_meta = new JObject();
                replay_meta["wukongPos"] = response["wukongPos"];
                
                eventItem["replay_meta"] = replay_meta;
            }

            return eventItem;
        }
    }
}
