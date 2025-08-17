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
    public class TotemTowersGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGTotemTowers";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "7912ea2a-2045-4364-906a-1dad4516240e";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "d428624957620cf6e067c241f539f11b5d012805";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.7182.364";
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
                return 101;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1, new HabaneroLogSymbolIDName{id = "idWild",      name = "Wild"       } },    
                    {2, new HabaneroLogSymbolIDName{id = "idScatter",   name = "Scatter"    } },    
                    {3, new HabaneroLogSymbolIDName{id = "idHi1",       name = "Hi1"        } },    
                    {4, new HabaneroLogSymbolIDName{id = "idHi2",       name = "Hi2"        } },
                    {5, new HabaneroLogSymbolIDName{id = "idHi3",       name = "Hi3"        } },
                    {6, new HabaneroLogSymbolIDName{id = "idHi4",       name = "Hi4"        } },
                    {7, new HabaneroLogSymbolIDName{id = "idLo1",       name = "Lo1"        } },
                    {8, new HabaneroLogSymbolIDName{id = "idLo2",       name = "Lo2"        } },
                    {9, new HabaneroLogSymbolIDName{id = "idLo3",       name = "Lo3"        } },
                    {10,new HabaneroLogSymbolIDName{id = "idLo4",       name = "Lo4"        } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 548;
            }
        }
        #endregion

        public TotemTowersGameLogic()
        {
            _gameID     = GAMEID.TotemTowers;
            GameName    = "TotemTowers";
        }

        protected override JObject buildEventItem(string strGlobalUserID, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strGlobalUserID, currentIndex);

            HabaneroHistoryResponses responses = _dicUserHistory[strGlobalUserID].Responses[currentIndex];
            dynamic response = JsonConvert.DeserializeObject<dynamic>(responses.Response);
            if (!object.ReferenceEquals(response["linewins"], null) && response["linewins"].Count > 0)
            {
                int winLineCnt = response["linewins"].Count;
                eventItem["numwinlines"] = winLineCnt;
                if (winLineCnt >= 5 && winLineCnt < 20)
                    eventItem["numlines"] = 40;
                else if(winLineCnt >= 20 && winLineCnt < 25)
                    eventItem["numlines"] = 50;
                else if (winLineCnt >= 25 && winLineCnt < 40)
                    eventItem["numlines"] = 60;
                else if (winLineCnt >= 40 && winLineCnt < 50)
                    eventItem["numlines"] = 70;
                else if (winLineCnt >= 50 && winLineCnt < 70)
                    eventItem["numlines"] = 90;
                else if (winLineCnt >= 70)
                    eventItem["numlines"] = 101;
            }

            if (!object.ReferenceEquals(response["TotemTowers_totemReels"], null) && response["TotemTowers_totemReels"].Count > 0)
                eventItem["reelslist"] = buildHabaneroLogReelslist(response);

            if (!object.ReferenceEquals(response["currentfreegame"], null))
                eventItem["multiplier"] = 1;

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

        protected override JArray buildHabaneroLogReels(string strGlobalUserID, int currentIndex, dynamic response, bool containWild = false)
        {
            dynamic reels = base.buildHabaneroLogReels(strGlobalUserID, currentIndex, response as JObject, containWild);
            if (object.ReferenceEquals(response["TotemTowers_totemReels"], null) || response["TotemTowers_totemReels"].Count == 0)
                return reels;
            
            for(int i = 0; i < response["TotemTowers_totemReels"].Count; i++)
            {
                int reelindex = (int)response["TotemTowers_totemReels"][i];
                for(int j = 0; j < 3; j++)
                    reels[reelindex][j] = SymbolIdStringForLog[1].id;
            }
            return reels;
        }
    }
}
