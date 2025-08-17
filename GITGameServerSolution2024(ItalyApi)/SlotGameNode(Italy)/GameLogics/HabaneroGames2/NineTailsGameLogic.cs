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
    public class NineTailsGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGNineTails";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "35805392-a731-4f74-beac-7d449ef141c3";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "6abf60b99e04b7701260f005880707c6e2277d9e";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.8977.400";
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
                return 259;
            }
        }
        protected override string BetType
        {
            get
            {
                return "Ways";
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idWild",    name = "Wild"       } },    //흰여우(와일드)
                    {2,   new HabaneroLogSymbolIDName{id = "idScatter", name = "Scatter"    } },    //스캐터
                    {3,   new HabaneroLogSymbolIDName{id = "idMask",    name = "Mask"       } },    //가면
                    {4,   new HabaneroLogSymbolIDName{id = "idFan",     name = "Fan"        } },    //부채
                    {5,   new HabaneroLogSymbolIDName{id = "idBells",   name = "Bells"      } },    //방울
                    {6,   new HabaneroLogSymbolIDName{id = "idLantern", name = "Lantern"    } },    //야경등
                    {7,   new HabaneroLogSymbolIDName{id = "idRed",     name = "Red"        } },    //빨강불
                    {8,   new HabaneroLogSymbolIDName{id = "idBlue",    name = "Blue"       } },    //파란불
                    {9,   new HabaneroLogSymbolIDName{id = "idGreen",   name = "Green"      } },    //풀색불
                    {10,  new HabaneroLogSymbolIDName{id = "idPurple",  name = "Purple"     } },    //보라색불
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 598;
            }
        }
        #endregion

        public NineTailsGameLogic()
        {
            _gameID     = GAMEID.NineTails;
            GameName    = "NineTails";
        }

        protected override JObject buildEventItem(string strGlobalUserId, int currentIndex)
        {
            JObject eventItem   = base.buildEventItem(strGlobalUserId, currentIndex);

            HabaneroHistoryResponses responses = _dicUserHistory[strGlobalUserId].Responses[currentIndex];
            dynamic response    = JsonConvert.DeserializeObject<dynamic>(responses.Response);
            JArray reels        = buildHabaneroLogReels(strGlobalUserId, currentIndex, response as JObject);
            eventItem["reels"]  = reels;

            if (!object.ReferenceEquals(response["NineTails_addSymbols"], null))
            {
                JArray reelslist = buildHabaneroLogReelslist(response as JObject);
                eventItem["reelslist"] = reelslist;
            }

            if (!object.ReferenceEquals(response["currentfreegame"], null))
                eventItem["multiplier"] = 1;

            JArray subEvents = new JArray();

            if (!object.ReferenceEquals(response["NineTails_prizesAwarded"], null))
            {
                if (!object.ReferenceEquals(eventItem["subevents"], null))
                    subEvents = eventItem["subevents"] as JArray;
                
                for(int i = 0; i < response["NineTails_prizesAwarded"].Count; i++)
                {
                    JObject subEventItem = new JObject();
                    subEventItem["type"]    = "cashprize";
                    subEventItem["wincash"] = response["NineTails_prizesAwarded"][i]["amount"];
                    subEvents.Add(subEventItem);
                }
            }

            if (subEvents.Count > 0)
                eventItem["subevents"] = subEvents;

            return eventItem;
        }
        
        protected override JArray buildHabaneroLogReelslist(dynamic response)
        {
            JArray reelslist        = new JArray();
            JObject reelslistItem   = new JObject();
            JArray reelsListCols    = new JArray();

            for(int i = 0; i < response["reels"].Count; i++)
            {
                JArray reelsCol = new JArray();
                for(int j = 0; j < response["reels"][i].Count; j++)
                    reelsCol.Add("idBlank");
                reelsListCols.Add(reelsCol);
            }

            for (int i = 0; i < response["NineTails_addSymbols"].Count; i++)
            {
                int col     = (int)response["NineTails_addSymbols"][i]["x"];
                int row     = (int)response["NineTails_addSymbols"][i]["y"];
                int type    = (int)response["NineTails_addSymbols"][i]["type"];
                
                reelsListCols[col][row] = SymbolIdStringForLog[type].id;
            }
            
            reelslistItem["reels"] = reelsListCols;
            reelslist.Add(reelslistItem);
            return reelslist;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserId, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            JArray resumeGames = base.buildInitResumeGame(strGlobalUserId, betInfo, lastResult, gameinstanceid, roundid, currentAction);

            JArray nineTailPrizeList = FindNineTailPrizeList(strGlobalUserId);
            if (nineTailPrizeList != null)
                resumeGames[0]["NineTails_prizeList"] = nineTailPrizeList;

            return resumeGames;
        }

        private JArray FindNineTailPrizeList(string strGlobalUserId)
        {
            if (!_dicUserHistory.ContainsKey(strGlobalUserId) || _dicUserHistory[strGlobalUserId].Responses == null || _dicUserHistory[strGlobalUserId].Responses.Count == 0)
                return null;

            JArray nineTalsPrizeList = new JArray();
            foreach(HabaneroHistoryResponses responses in _dicUserHistory[strGlobalUserId].Responses)
            {
                dynamic curStepResult = JsonConvert.DeserializeObject<dynamic>(responses.Response);
                if (!object.ReferenceEquals(curStepResult["NineTails_prizes"], null))
                {
                    for(int i = 0; i < curStepResult["NineTails_prizes"].Count; i++)
                    {
                        nineTalsPrizeList.Add(curStepResult["NineTails_prizes"][i]);
                    }
                }

                if(!object.ReferenceEquals(curStepResult["NineTails_prizesAwarded"], null))
                {
                    int prizedCnt = curStepResult["NineTails_prizesAwarded"].Count;
                    for(int i = 0; i < prizedCnt; i++)
                        nineTalsPrizeList.RemoveAt(0);
                }

            }
            if (nineTalsPrizeList.Count > 0)
                return nineTalsPrizeList;
            
            return null;
        }

        protected override void convertWinsByBet(dynamic resultContext, float currentBet)
        {
            base.convertWinsByBet(resultContext as JObject, currentBet);

            if (!object.ReferenceEquals(resultContext["NineTails_prizes"], null))
            {
                for(int i = 0; i < resultContext["NineTails_prizes"].Count; i++)
                    resultContext["NineTails_prizes"][i] = convertWinByBet((double)resultContext["NineTails_prizes"][i], currentBet);
            }

            if (!object.ReferenceEquals(resultContext["NineTails_prizesAwarded"], null))
            {
                for (int i = 0; i < resultContext["NineTails_prizesAwarded"].Count; i++)
                {
                    if (!object.ReferenceEquals(resultContext["NineTails_prizesAwarded"][i]["amount"], null))
                        resultContext["NineTails_prizesAwarded"][i]["amount"] = convertWinByBet((double)resultContext["NineTails_prizesAwarded"][i]["amount"], currentBet);

                    if (!object.ReferenceEquals(resultContext["NineTails_prizesAwarded"][i]["doubled"], null))
                        resultContext["NineTails_prizesAwarded"][i]["doubled"] = convertWinByBet((double)resultContext["NineTails_prizesAwarded"][i]["doubled"], currentBet);

                    if (!object.ReferenceEquals(resultContext["NineTails_prizesAwarded"][i]["upgraded"], null))
                        resultContext["NineTails_prizesAwarded"][i]["upgraded"] = convertWinByBet((double)resultContext["NineTails_prizesAwarded"][i]["upgraded"], currentBet);
                }
            }
        }
    }
}
