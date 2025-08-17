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
    public class MysticFortuneDeluxeGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGMysticFortuneDeluxe";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "c14e0fd2-0d22-45fe-92aa-556a099f3ba1";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "2d6a6498ce30331eed6fa6e0327fa7cdc4ac594c";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.8539.397";
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
                return 28;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idWild",      name = "Wild"       } },    //와일드
                    {2,   new HabaneroLogSymbolIDName{id = "idScatter",   name = "Scatter"    } },    //스캐터
                    {3,   new HabaneroLogSymbolIDName{id = "idLotus",     name = "Lotus"      } },    //머니심벌
                    {4,   new HabaneroLogSymbolIDName{id = "idFortune",   name = "Fortune"    } },    //엽전
                    {5,   new HabaneroLogSymbolIDName{id = "idLion",      name = "Lion"       } },    //사자
                    {6,   new HabaneroLogSymbolIDName{id = "idUrn",       name = "Urn"        } },    //항아리
                    {7,   new HabaneroLogSymbolIDName{id = "idLantern",   name = "Lantern"    } },    //축등
                    {8,   new HabaneroLogSymbolIDName{id = "idA",         name = "A"          } },    //A
                    {9,   new HabaneroLogSymbolIDName{id = "idK",         name = "K"          } },    //K
                    {10,  new HabaneroLogSymbolIDName{id = "idQ",         name = "Q"          } },    //Q
                    {11,  new HabaneroLogSymbolIDName{id = "idJ",         name = "J"          } },    //J
                    {12,  new HabaneroLogSymbolIDName{id = "id10",        name = "10"         } },    //10
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 588;
            }
        }
        #endregion

        public MysticFortuneDeluxeGameLogic()
        {
            _gameID     = GAMEID.MysticFortuneDeluxe;
            GameName    = "MysticFortuneDeluxe";
        }

        protected override JObject buildEventItem(string strGlobalUserID, int currentIndex)
        {
            HabaneroHistoryResponses responses = _dicUserHistory[strGlobalUserID].Responses[currentIndex];
            dynamic response = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            bool containWild    = false;
            bool containMoney   = false;
            for (int j = 0; j < response["reels"].Count; j++)
            {
                for (int k = 0; k < response["reels"][j].Count; k++)
                {
                    string symbol = Convert.ToString(response["reels"][j][k]["symbolid"]);
                    if (symbol == "1")
                        containWild = true;
                    if(symbol == "3")
                        containMoney = true;

                    if (containWild && containMoney)
                        break;
                }
                if (containWild && containMoney)
                    break;
            }

            JObject eventItem   = base.buildEventItem(strGlobalUserID,currentIndex);
            JArray reels        = buildHabaneroLogReels(strGlobalUserID, currentIndex,response as JObject,containWild);
            eventItem["reels"]  = reels;
            
            if (containWild)
            {
                JArray reelslist        = buildHabaneroLogReelslist(response as JObject);
                eventItem["reelslist"]  = reelslist;
            }

            if (containMoney)
            {
                JArray reelsmeta        = buildHabaneroLogReelsMeta(response as JObject);
                eventItem["reels_meta"] = reelsmeta;
            }

            return eventItem;
        }

        protected override JArray buildHabaneroLogReels(string strGlobalUserID,int currentIndex ,dynamic response,bool containWild = false)
        {
            JArray reels            = new JArray();

            if (containWild)
            {
                for ( int j = 0; j < response["reels"].Count; j++)
                {
                    int wildIndex = -1;
                    for (int k = 0; k < response["reels"][j].Count; k++)
                    {
                        string symbol = Convert.ToString(response["reels"][j][k]["symbolid"]);
                        if (symbol == "1")
                        {
                            wildIndex = k;
                            break;
                        }
                    }

                    JArray col = new JArray();
                    if(wildIndex != -1)
                    {
                        for (int k = 0; k < response["reels"][j].Count; k++)
                        {
                            string symbolid = SymbolIdStringForLog[1].id;
                            col.Add(symbolid);
                        }
                    }
                    else
                    {
                        for (int k = 0; k < response["reels"][j].Count; k++)
                        {
                            int symbol   = Convert.ToInt32(response["reels"][j][k]["symbolid"]);
                            string symbolid = SymbolIdStringForLog[symbol].id;
                            col.Add(symbolid);
                        }
                    }
                    reels.Add(col);
                }
            }
            else
            {
                for (int j = 0; j < response["reels"].Count; j++)
                {
                    JArray col = new JArray();
                    for (int k = 0; k < response["reels"][j].Count; k++)
                    {
                        int symbol   = Convert.ToInt32(response["reels"][j][k]["symbolid"]);
                        string symbolid = SymbolIdStringForLog[symbol].id;
                        col.Add(symbolid);
                    }
                    reels.Add(col);
                }
            }

            return reels;
        }

        protected override JArray buildHabaneroLogReelslist(dynamic response)
        {
            JArray reelslist        = new JArray();
            JObject reelslistItem   = new JObject();
            JArray reelsListCols    = new JArray();

            for (int j = 0; j < response["reels"].Count; j++)
            {
                int wildIndex   = -1;
                int wildOffset  = 100;
                for (int k = 0; k < response["reels"][j].Count; k++)
                {
                    string symbol = Convert.ToString(response["reels"][j][k]["symbolid"]);
                    if (symbol == "1")
                    {
                        wildIndex = k;
                        if (!object.ReferenceEquals(response["reels"][j][k]["meta"], null) && !object.ReferenceEquals(response["reels"][j][k]["meta"]["wildOffset"], null))
                            wildOffset = Convert.ToInt32(response["reels"][j][k]["meta"]["wildOffset"]);
                        break;
                    }
                }

                JArray reelsListCol = new JArray();
                for (int k = 0; k < response["reels"][j].Count; k++)
                {
                    int symbol   = Convert.ToInt32(response["reels"][j][k]["symbolid"]);
                    string symbolid = SymbolIdStringForLog[symbol].id;
                    if (wildIndex != -1)
                    {
                        if (symbol != 1)
                        {
                            if(wildOffset == 100 && k > wildIndex)
                            {
                                reelsListCol.Add(null);
                            }
                            else if(wildOffset != 100 && k < 4 - wildIndex + wildOffset)
                            {
                                reelsListCol.Add(null);
                            }
                            else
                            {
                                reelsListCol.Add(symbolid);
                            }
                        }
                        else
                        {
                            if(wildOffset == 100)
                            {
                                switch (wildIndex)
                                {
                                    case 1:
                                        reelsListCol.Add(string.Format("{0}{1}", symbolid, 123));
                                        break;
                                    case 2:
                                        reelsListCol.Add(string.Format("{0}{1}", symbolid, 12));
                                        break;
                                    case 3:
                                        reelsListCol.Add(string.Format("{0}{1}", symbolid, 1));
                                        break;
                                    default:
                                        reelsListCol.Add(symbolid);
                                        break;
                                }
                            }
                            else
                            {
                                switch (wildOffset)
                                {
                                    case -1:
                                        reelsListCol.Add(string.Format("{0}{1}", symbolid, 234));
                                        break;
                                    case -2:
                                        reelsListCol.Add(string.Format("{0}{1}", symbolid, 34));
                                        break;
                                    case -3:
                                        reelsListCol.Add(string.Format("{0}{1}", symbolid, 4));
                                        break;
                                    default:
                                        reelsListCol.Add(symbolid);
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        reelsListCol.Add(symbolid);
                    }
                }
                reelsListCols.Add(reelsListCol);
            }
            reelslistItem["reels"] = reelsListCols;
            reelslist.Add(reelslistItem);
            return reelslist;
        }

        protected override JArray buildHabaneroLogReelsMeta(dynamic response)
        {
            JArray reelsListMeta = new JArray();

            for (int j = 0; j < response["reels"].Count; j++)
            {
                bool hasMoneyInCol = false;
                for (int k = 0; k < response["reels"][j].Count; k++)
                {
                    string symbol = Convert.ToString(response["reels"][j][k]["symbolid"]);
                    if(symbol == "3")
                    {
                        hasMoneyInCol = true;
                        break;
                    }
                }
                
                if (!hasMoneyInCol)
                {
                    reelsListMeta.Add(null);
                    continue;
                }
                
                JArray reelsMetaCol = new JArray();
                for (int k = 0; k < response["reels"][j].Count; k++)
                {
                    string symbol = Convert.ToString(response["reels"][j][k]["symbolid"]);
                    if(symbol == "3")
                    {
                        JObject moneyMul = new JObject();
                        moneyMul["cash_linebet"] = response["reels"][j][k]["wincashmultiplier"];
                        reelsMetaCol.Add(moneyMul);
                    }
                    else
                        reelsMetaCol.Add(null);
                }
                reelsListMeta.Add(reelsMetaCol);
            }
            return reelsListMeta;
        }
    }
}
