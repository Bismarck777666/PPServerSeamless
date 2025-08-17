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
    public class LanternLuckGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGLanternLuck";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "bda45c89-8717-4138-ad7e-c8ece6b95f23";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "40166e9fc27018ca81ee64287484a0544d9ae5e6";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.9447.402";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idWild",        name = "Wild"           } },    //와일드
                    
                    {2,   new HabaneroLogSymbolIDName{id = "idLantern1",    name = "Lantern1"       } },
                    {3,   new HabaneroLogSymbolIDName{id = "idLantern2",    name = "Lantern2"       } },    
                    {4,   new HabaneroLogSymbolIDName{id = "idLantern3",    name = "Lantern3"       } },    
                    {5,   new HabaneroLogSymbolIDName{id = "idLantern4",    name = "Lantern4"       } },    
                    {6,   new HabaneroLogSymbolIDName{id = "idLantern5",    name = "Lantern5"       } },    
                    {7,   new HabaneroLogSymbolIDName{id = "idLantern6",    name = "Lantern6"       } },    
                    
                    {8,   new HabaneroLogSymbolIDName{id = "idLantern1Lit", name = "Lantern1Lit"    } },    
                    {9,   new HabaneroLogSymbolIDName{id = "idLantern2Lit", name = "Lantern2Lit"    } },    
                    {10,  new HabaneroLogSymbolIDName{id = "idLantern3Lit", name = "Lantern3Lit"    } },    
                    {11,  new HabaneroLogSymbolIDName{id = "idLantern4Lit", name = "Lantern4Lit"    } },    
                    {12,  new HabaneroLogSymbolIDName{id = "idLantern5Lit", name = "Lantern5Lit"    } },    
                    {13,  new HabaneroLogSymbolIDName{id = "idLantern6Lit", name = "Lantern6Lit"    } },    
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 618;
            }
        }
        #endregion

        public LanternLuckGameLogic()
        {
            _gameID     = GAMEID.LanternLuck;
            GameName    = "LanternLuck";
        }

        protected override JObject buildEventItem(string strGlobalUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strGlobalUserId, currentIndex);
            HabaneroHistoryResponses responses = _dicUserHistory[strGlobalUserId].Responses[currentIndex];
            dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            if (!object.ReferenceEquals(resultContext["currentfreegame"], null))
                eventItem["multiplier"] = 1;

            if (!object.ReferenceEquals(resultContext["wildPosList"], null))
            {
                JArray reelsList = buildHabaneroLogReelslist(resultContext);
                eventItem["reelslist"] = reelsList;
            }

            return eventItem;
        }

        protected override JArray buildHabaneroLogReelslist(dynamic response)
        {
            JArray reelsList            = new JArray();
            JObject beforeReelsListItem = new JObject();
            JObject reelsListItem       = new JObject();

            JArray beforeReels = new JArray();
            for (int i = 0; i < response["reels"].Count; i++)
            {
                JArray reelsCol = new JArray();
                for (int j = 0; j < response["reels"][i].Count; j++)
                    reelsCol.Add("idBlank");
                beforeReels.Add(reelsCol);
            }

            for (int i = 0; i < response["wildPosList"].Count; i++)
            {
                int reelIndex   = (int)response["wildPosList"][i]["reelindex"];
                int symbolIndex = (int)response["wildPosList"][i]["symbolindex"];
                beforeReels[reelIndex][symbolIndex] = "idWild";
            }

            beforeReelsListItem["reels"] = beforeReels;
            reelsList.Add(beforeReelsListItem);

            JArray reels = new JArray();
            for (int i = 0; i < response["reels"].Count; i++)
            {
                JArray reelsCol = new JArray();
                for (int j = 0; j < response["reels"][i].Count; j++)
                {
                    int symbol = (int)response["reels"][i][j]["symbolid"];
                    if (symbol != 1)
                        reelsCol.Add("idBlank");
                    else
                        reelsCol.Add("idWild");
                }
                reels.Add(reelsCol);
            }
            reelsListItem["reels"] = reels;
            reelsList.Add(reelsListItem);

            return reelsList;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserId, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            JArray resumeGames = base.buildInitResumeGame(strGlobalUserId, betInfo, lastResult, gameinstanceid, roundid, currentAction);

            JArray resumeWildPosList = new JArray();

            dynamic resultContext = lastResult as dynamic;
            for(int i = 0; i < resultContext["reels"].Count; i++)
            {
                for(int j = 0; j < resultContext["reels"][i].Count; j++)
                {
                    if((int)resultContext["reels"][i][j]["symbolid"] == 1)
                    {
                        JObject wildItem = new JObject();
                        wildItem["reelindex"]   = i;
                        wildItem["symbolindex"] = j;
                        resumeWildPosList.Add(wildItem);
                    }
                }
            }
            if (resumeWildPosList.Count > 0)
                resumeGames[0]["resumeWildPosList"] = resumeWildPosList;
            
            return resumeGames;
        }
    }
}
