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
    public class BeforeTimeRunsOutGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGBeforeTimeRunsOut";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "38652f48-edcf-4cc6-8a04-db594f28b0f5";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "24b7a91aa9492331e01fac86e826c50edf130b7b";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.7781.375";
            }
        }
        protected override float MiniCoin
        {
            get
            {
                return 15.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 10;
            }
        }
        protected override string BetType
        {
            get
            {
                return "HorizontalVerticalPays";
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idWild",        name = "Wild"           } },
                    {2,   new HabaneroLogSymbolIDName{id = "idScatter",     name = "Scatter"        } },    
                    {3,   new HabaneroLogSymbolIDName{id = "idPrincess",    name = "Princess"       } },    
                    {4,   new HabaneroLogSymbolIDName{id = "idPrince",      name = "Prince"         } },
                    {5,   new HabaneroLogSymbolIDName{id = "idVizier",      name = "Vizier"         } },    
                    {6,   new HabaneroLogSymbolIDName{id = "idSword",       name = "Sword"          } },    
                    {7,   new HabaneroLogSymbolIDName{id = "idCoinGold",    name = "CoinGold"       } },
                    {8,   new HabaneroLogSymbolIDName{id = "idCoinSilver",  name = "CoinSilver"     } },   
                    {9,   new HabaneroLogSymbolIDName{id = "idCoinBronze",  name = "CoinBronze"     } },
                    
                    {10,  new HabaneroLogSymbolIDName{id = "idPotionRed",   name = "PotionRed"      } },    
                    {11,  new HabaneroLogSymbolIDName{id = "idPotionBlue",  name = "PotionBlue"     } },    
                    {12,  new HabaneroLogSymbolIDName{id = "idBlank",       name = "Blank"          } },    
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 563;
            }
        }
        #endregion

        public BeforeTimeRunsOutGameLogic()
        {
            _gameID     = GAMEID.BeforeTimeRunsOut;
            GameName    = "BeforeTimeRunsOut";
        }

        protected override JObject buildEventItem(string strGlobalUserID, int currentIndex)
        {
            dynamic eventItem = base.buildEventItem(strGlobalUserID, currentIndex);

            HabaneroHistoryResponses responses  = _dicUserHistory[strGlobalUserID].Responses[currentIndex];
            dynamic response                    = JsonConvert.DeserializeObject<dynamic>(responses.Response);
            if (!object.ReferenceEquals(response["currentfreegame"], null))
            {
                eventItem["multiplier"] = 1;
                if (!object.ReferenceEquals(response["gemCount"], null))
                {
                    JArray customSubEvents      = new JArray();
                    JObject customSubEventItem  = new JObject();
                    customSubEventItem["type"]  = "BeforeTimeRunsOutGemCount";
                    customSubEventItem["gemC"]  = response["gemCount"];
                    customSubEvents.Add(customSubEventItem);
                    eventItem["customsubevents"] = customSubEvents;
                }
                if(!object.ReferenceEquals(response["scatterwins"],null) && response["scatterwins"].Count > 0)
                {
                    string symbolName = SymbolIdStringForLog[(int)response["scatterwins"][0]["symbolid"]].name;
                    for(int i = 0; i < eventItem["subevents"].Count; i++)
                    {
                        if (eventItem["subevents"][i]["type"] != "scatter")
                            continue;
                        eventItem["subevents"][i]["type"]   = "cashprize";
                        eventItem["subevents"][i]["symbol"] = symbolName;
                    }
                }
            }
            
            if (!object.ReferenceEquals(response["vizierList"], null) && (int)response["vizierList"].Count > 0)
            {
                JArray reelsList        = buildHabaneroLogReelslist(response);
                eventItem["reelslist"]  = reelsList;
            }

            return eventItem;
        }

        protected override JArray buildHabaneroLogReels(string strGlobalUserId, int currentIndex, dynamic response, bool containWild = false)
        {
            JArray reels = new JArray();
            for (int i = 0; i < response["reels"].Count; i++)
            {
                JArray col = new JArray();
                for (int j = 0; j < response["reels"][i].Count; j++)
                {
                    int symbol      = (int)response["reels"][i][j]["symbolid"];
                    string symbolid = SymbolIdStringForLog[symbol].id;
                    col.Add(symbolid);
                }
                reels.Add(col);
            }
            return reels;
        }

        protected override JArray buildHabaneroLogReelslist(dynamic response)
        {
            JArray  reelslist       = new JArray();

            JObject reelslistItem   = new JObject();
            JArray  reels           = new JArray();
            for (int j = 0; j < response["virtualreels"].Count; j++)
            {
                JArray col = new JArray();
                for (int k = 2; k < response["virtualreels"][j].Count - 2; k++)
                {
                    int symbol = Convert.ToInt32(response["virtualreels"][j][k]);
                    string symbolid = SymbolIdStringForLog[symbol].id;
                    col.Add(symbolid);
                }
                reels.Add(col);
            }
            reelslistItem["reels"] = reels;
            reelslist.Add(reelslistItem);

            for(int i = 0; i < response["vizierList"].Count - 1; i++)
            {
                dynamic buffLastReels = reelslist[reelslist.Count - 1]["reels"];
                JArray bufNewReel = new JArray();
                for (int j = 0; j < buffLastReels.Count; j++)
                {
                    JArray bufNewCol = new JArray();
                    for(int k = 0; k < buffLastReels[j].Count; k++)
                        bufNewCol.Add(buffLastReels[j][k]);
                    bufNewReel.Add(bufNewCol);
                }

                if(i == 0)
                {
                    int reelindex   = response["vizierList"][i]["princePos"]["reelindex"];
                    int symbolindex = response["vizierList"][i]["princePos"]["symbolindex"];
                    int princeOriginSymbol = (int)response["reels"][reelindex][symbolindex]["symbolid"];
                    bufNewReel[reelindex][symbolindex] = SymbolIdStringForLog[princeOriginSymbol].id;
                }

                if(response["vizierList"][i]["turnWildList"].Count > 0)
                {
                    //와일드심벌 추가
                    for (int j = 0; j < response["vizierList"][i]["turnWildList"].Count; j++)
                    {
                        int reelindex   = response["vizierList"][i]["turnWildList"][j]["reelindex"];
                        int symbolindex = response["vizierList"][i]["turnWildList"][j]["symbolindex"];
                        bufNewReel[reelindex][symbolindex] = SymbolIdStringForLog[1].id;
                    }
                }

                if ((bool)response["vizierList"][i]["isWin"])
                {
                    //승인경우 왕자심벌 새위치 추가,몬스터심벌 삭제
                    dynamic princePath      = response["vizierList"][i]["pathToVizierList"];
                    int reelindex   = princePath[princePath.Count - 1]["pathPos"]["reelindex"];
                    int symbolindex = princePath[princePath.Count - 1]["pathPos"]["symbolindex"];
                    bufNewReel[reelindex][symbolindex] = SymbolIdStringForLog[4].id;
                    
                    reelindex       = response["vizierList"][i]["vizierPos"]["reelindex"];
                    symbolindex     = response["vizierList"][i]["vizierPos"]["symbolindex"];
                    int symbolId    = (int)response["vizierList"][i]["origVizierSymbolId"];
                    bufNewReel[reelindex][symbolindex] = SymbolIdStringForLog[symbolId].id;
                }
                else
                {
                    //패인경우 왕자심벌 삭제
                    int reelindex   = response["vizierList"][i]["princePos"]["reelindex"];
                    int symbolindex = response["vizierList"][i]["princePos"]["symbolindex"];
                    int princeOriginSymbol = (int)response["reels"][reelindex][symbolindex]["symbolid"];
                    bufNewReel[reelindex][symbolindex] = SymbolIdStringForLog[princeOriginSymbol].id;
                }
                JObject newReelListItem = new JObject();
                newReelListItem["reels"] = bufNewReel;
                reelslist.Add(newReelListItem);
            }
            return reelslist;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserId, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            dynamic resumeGames = base.buildInitResumeGame(strGlobalUserId, betInfo, lastResult, gameinstanceid, roundid, currentAction);
            resumeGames[0]["gemCount"] = lastResult["gemCount"];

            dynamic resultContext = lastResult as dynamic;
            
            if (!object.ReferenceEquals(resultContext["coinRespin"], null))
            {
                resumeGames[0]["stickySymbolMatrix"] = resultContext["coinRespin"]["stickySymbolMatrix"];
                for(int i = 0; i < resultContext["coinRespin"]["stickySymbolMatrix"].Count; i++)
                {
                    for(int j = 0; j < resultContext["coinRespin"]["stickySymbolMatrix"][i].Count; j++)
                    {
                        int symbol = (int)resultContext["coinRespin"]["stickySymbolMatrix"][i][j];
                        if (symbol == 0)
                            symbol = 12;
                        resumeGames[0]["virtualreels"][i][j + 2] = symbol;
                    }
                }
                resumeGames[0]["gamemode"] = "respin";
            }
            else
            {
                for (int i = 0; i < resultContext["reels"].Count; i++)
                {
                    for (int j = 0; j < resultContext["reels"][i].Count; j++)
                        resumeGames[0]["virtualreels"][i][j + 2] = resultContext["reels"][i][j]["symbolid"];
                }
            }

            return resumeGames;
        }
    }
}
