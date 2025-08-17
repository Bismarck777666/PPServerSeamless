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
    public class LuckyDurianGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGLuckyDurian";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "6ebebcee-e25e-423b-90bf-f5f9cf6d9aeb";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "7ebd8004278d57f2caed00770eff46f3f8f3ccbc";
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
                return 15.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 15;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idWild",            name = "Wild"           } },    
                    {2,   new HabaneroLogSymbolIDName{id = "idRambutan",        name = "Rambutan"       } },    
                    {3,   new HabaneroLogSymbolIDName{id = "idJavaPlum",        name = "JavaPlum"       } },    
                    {4,   new HabaneroLogSymbolIDName{id = "idYellowCoconut",   name = "YellowCoconut"  } },    
                    {5,   new HabaneroLogSymbolIDName{id = "idAmbarella",       name = "Ambarella"      } },    
                    {6,   new HabaneroLogSymbolIDName{id = "idPomelo",          name = "Pomelo"         } },    
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 613;
            }
        }
        #endregion

        public LuckyDurianGameLogic()
        {
            _gameID     = GAMEID.LuckyDurian;
            GameName    = "LuckyDurian";
        }

        protected override JObject buildEventItem(string strGlobalUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strGlobalUserId, currentIndex);
            HabaneroHistoryResponses responses = _dicUserHistory[strGlobalUserId].Responses[currentIndex];
            dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            if (!object.ReferenceEquals(resultContext["currentfreegame"], null))
                eventItem["multiplier"] = 1;

            List<int> extendWilds = findExtendWilds(strGlobalUserId,_dicUserHistory[strGlobalUserId].Responses.Count - currentIndex);
            if(extendWilds != null)
            {
                JArray reels = buildHabaneroLogReels(strGlobalUserId,currentIndex,resultContext);

                for(int i = 0; i < extendWilds.Count; i++)
                {
                    for(int j = 0; j < 3;j++)
                        reels[extendWilds[i]][j]        = SymbolIdStringForLog[1].id;
                }
                eventItem["reels"] = reels;
            }

            if (!object.ReferenceEquals(resultContext["expandingwilds"], null))
            {
                JArray reelsList        = new JArray();
                JObject reelsListItem   = new JObject();
                reelsListItem["reels"]  = buildHabaneroLogReelslist(resultContext);
                if(extendWilds != null)
                {
                    for (int i = 0; i < extendWilds.Count; i++)
                    {
                        for (int j = 0; j < 3; j++)
                            reelsListItem["reels"][extendWilds[i]][j] = SymbolIdStringForLog[1].id;
                    }
                }
                reelsList.Add(reelsListItem);
                eventItem["reelslist"] = reelsList;
            }
            return eventItem;
        }
        
        protected override JArray buildHabaneroLogReels(string strGlobalUserId,int currentIndex ,dynamic response, bool containWild = false)
        {
            JArray logReels = this.buildHabaneroLogReelslist(response as JObject);
            if (!object.ReferenceEquals(response["expandingwilds"], null))
            {
                for(int i = 0; i < response["expandingwilds"].Count; i++)
                {
                    int reelIndex = (int)response["expandingwilds"][i]["reelindex"];
                    for(int j = 0; j < 3; j++)
                    {
                        logReels[reelIndex][j] = SymbolIdStringForLog[1].id;
                    }
                }
            }
            return logReels;
        }

        protected override JArray buildHabaneroLogReelslist(dynamic response)
        {
            JArray logReels = new JArray();
            for (int i = 0; i < response["reels"].Count; i++)
            {
                JArray col = new JArray();
                for (int j = 0; j < response["reels"][i].Count; j++)
                {
                    int symbol = Convert.ToInt32(response["reels"][i][j]["symbolid"]);
                    col.Add(SymbolIdStringForLog[symbol].id);
                }
                logReels.Add(col);
            }
            return logReels;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserId, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            JArray resumeGames = base.buildInitResumeGame(strGlobalUserId, betInfo, lastResult, gameinstanceid, roundid, currentAction);
            resumeGames[0]["gamemode"] = "respin";
            
            List<int> wildExtendsList = findExtendWilds(strGlobalUserId,0);
            if (!object.ReferenceEquals(wildExtendsList, null))
            {
                JArray LuckyDurian_wildReels = new JArray();
                foreach(int reelindex in wildExtendsList)
                    LuckyDurian_wildReels.Add(reelindex);
                resumeGames[0]["LuckyDurian_wildReels"] = LuckyDurian_wildReels;
            }

            return resumeGames;
        }

        private List<int> findExtendWilds(string strGlobalUserId, int backwardIndex)
        {
            List<int> extendWilds = new List<int>();
            if (!_dicUserHistory.ContainsKey(strGlobalUserId) || _dicUserHistory[strGlobalUserId].Responses == null || _dicUserHistory[strGlobalUserId].Responses.Count <= backwardIndex)
                return null;

            List<HabaneroHistoryResponses> habaneroHistories = _dicUserHistory[strGlobalUserId].Responses;

            for(int i = 0; i < _dicUserHistory[strGlobalUserId].Responses.Count - backwardIndex; i++)
            {
                dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(habaneroHistories[i].Response);
                if (!object.ReferenceEquals(resultContext["expandingwilds"], null))
                {
                    for (int j = 0; j < resultContext["expandingwilds"].Count; j++)
                    {
                        extendWilds.Add((int)resultContext["expandingwilds"][j]["reelindex"]);
                    }
                }
            }
            
            if (extendWilds.Count == 0)
                return null;
            return extendWilds;
        }
    }
}
