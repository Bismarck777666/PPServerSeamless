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
    public class PandaPandaGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGPandaPanda";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "8f5fe8d8-d897-4ae0-82a5-b23a8099cd9c";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "a59ec9c813db6cf1d9c96a33d82c8f5b6a651eaa";
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
                return 30.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 243;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idPanda",           name = "Panda"          } },    //와일드(판더)
                    {3,   new HabaneroLogSymbolIDName{id = "idScatter",         name = "Scatter"        } },    //스캐터
                    {4,   new HabaneroLogSymbolIDName{id = "idTreasureBowl",    name = "TreasureBowl"   } },    //금전
                    {5,   new HabaneroLogSymbolIDName{id = "idHulu",            name = "Hulu"           } },    //호로병
                    {6,   new HabaneroLogSymbolIDName{id = "idWaterfall",       name = "Waterfall"      } },    //폭폭
                    {7,   new HabaneroLogSymbolIDName{id = "idCherryBlossoms",  name = "CherryBlossoms" } },    //꽃
                    {8,   new HabaneroLogSymbolIDName{id = "idBamboo",          name = "Bamboo"         } },    //참대
                    {9,   new HabaneroLogSymbolIDName{id = "idAce",             name = "Ace"            } },    //A
                    {10,  new HabaneroLogSymbolIDName{id = "idKing",            name = "King"           } },    //K
                    {11,  new HabaneroLogSymbolIDName{id = "idQueen",           name = "Queen"          } },    //Q
                    {12,  new HabaneroLogSymbolIDName{id = "idJack",            name = "Jack"           } },    //J
                    {13,  new HabaneroLogSymbolIDName{id = "idTen",             name = "Ten"            } },    //10
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 321;
            }
        }
        #endregion

        public PandaPandaGameLogic()
        {
            _gameID     = GAMEID.PandaPanda;
            GameName    = "PandaPanda";
        }

        protected override JObject buildEventItem(string strGlobalUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strGlobalUserId, currentIndex);
            HabaneroHistoryResponses responses = _dicUserHistory[strGlobalUserId].Responses[currentIndex];
            dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            if (!object.ReferenceEquals(resultContext["currentfreegame"], null))
                eventItem["multiplier"] = 1;
            
            return eventItem;
        }
        protected override JArray buildHabaneroLogReels(string strGlobalUserId,int currentIndex ,dynamic response, bool containWild = false)
        {
            JArray reels = base.buildHabaneroLogReels(strGlobalUserId, currentIndex, response as JObject, containWild);

            for (int j = 0; j < response["reels"].Count; j++)
            {
                for (int k = 0; k < response["reels"][j].Count; k++)
                {
                    int symbol      = Convert.ToInt32(response["reels"][j][k]["symbolid"]);
                    if (symbol != 1)
                        continue;

                    bool isPandaPanda = false;
                    if(!object.ReferenceEquals(response["reels"][j][k]["meta"], null))
                        isPandaPanda = Convert.ToBoolean(response["reels"][j][k]["meta"]["isPandaPanda"]);

                    string symbolid = SymbolIdStringForLog[symbol].id + (isPandaPanda ? "2" : "1");
                    reels[j][k] = symbolid;
                }
            }

            return reels;
        }
        protected override JArray buildInitResumeGame(string strGlobalUserId, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            JArray resumeGames = base.buildInitResumeGame(strGlobalUserId, betInfo, lastResult, gameinstanceid, roundid, currentAction);

            JArray pandaPandaPosition = FindPandaPandaPositions(strGlobalUserId);
            if(pandaPandaPosition != null)
                resumeGames[0]["PandaPanda_triggerPositions"] = pandaPandaPosition;
            
            return resumeGames;
        }
        private JArray FindPandaPandaPositions(string strGlobalUserId)
        {
            if (!_dicUserHistory.ContainsKey(strGlobalUserId) || _dicUserHistory[strGlobalUserId].Responses == null || _dicUserHistory[strGlobalUserId].Responses.Count == 0)
                return null;

            HabaneroHistoryResponses freeStartHistory = _dicUserHistory[strGlobalUserId].Responses[0];
            dynamic freeTriggerSpin = JsonConvert.DeserializeObject<dynamic>(freeStartHistory.Response);
            return freeTriggerSpin["PandaPanda_triggerPositions"];
        }
    }
}
