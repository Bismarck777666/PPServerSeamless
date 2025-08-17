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
    public class NewYearsBashGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGNewYearsBash";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "10f324b8-0729-4389-a883-30201c390bf9";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "94e7eae9161635a5c503a704d911a503d1882c21";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.9206.401";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idChampagne",   name = "Champagne"      } },    //와일드
                    {2,   new HabaneroLogSymbolIDName{id = "idClock",       name = "Clock"          } },    //스캐터
                    {3,   new HabaneroLogSymbolIDName{id = "idDubai",       name = "Dubai"          } },    //아파트
                    {4,   new HabaneroLogSymbolIDName{id = "idSydney",      name = "Sydney"         } },    //배
                    {5,   new HabaneroLogSymbolIDName{id = "idSanFrancisco",name = "SanFrancisco"   } },    //다리
                    {6,   new HabaneroLogSymbolIDName{id = "idNewYork",     name = "NewYork"        } },    //여신상
                    {7,   new HabaneroLogSymbolIDName{id = "idLondon",      name = "London"         } },    //시계탑
                    {8,   new HabaneroLogSymbolIDName{id = "idA",       name = "A"                  } },    //A
                    {9,   new HabaneroLogSymbolIDName{id = "idK",       name = "K"                  } },    //K
                    {10,  new HabaneroLogSymbolIDName{id = "idQ",       name = "Q"                  } },    //Q
                    {11,  new HabaneroLogSymbolIDName{id = "idJ",       name = "J"                  } },    //J
                    {12,  new HabaneroLogSymbolIDName{id = "id10",      name = "10"                 } },    //10
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 608;
            }
        }
        #endregion

        public NewYearsBashGameLogic()
        {
            _gameID     = GAMEID.NewYearsBash;
            GameName    = "NewYearsBash";
        }

        protected override JObject buildEventItem(string strGlobalUserId, int currentIndex)
        {
            dynamic eventItem = base.buildEventItem(strGlobalUserId, currentIndex);

            HabaneroHistoryResponses responses = _dicUserHistory[strGlobalUserId].Responses[currentIndex];
            dynamic response = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            if (!object.ReferenceEquals(response["currentfreegame"], null))
            {
                eventItem["multiplier"] = 1;
                JArray customSubEvents      = new JArray();
                JObject customSubEventItem  = new JObject();
                customSubEventItem["type"]  = "NewYearsBashReport";
                JArray countAfter = new JArray();
                dynamic reelTriggerList = response["NewYearsBash_FGMessage"]["reelTriggerList"];
                for(int i = 0; i < reelTriggerList.Count; i++)
                {
                    if ((int)reelTriggerList[i]["countBefore"] == 10)
                        countAfter.Add(10);
                    else
                        countAfter.Add((int)reelTriggerList[i]["countAfter"]);
                }

                customSubEventItem["countAfter"] = countAfter;
                customSubEvents.Add(customSubEventItem);
                eventItem["customsubevents"] = customSubEvents;
            }
            return eventItem;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserId, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            dynamic resumeGames = base.buildInitResumeGame(strGlobalUserId, betInfo, lastResult, gameinstanceid, roundid, currentAction);

            if (object.ReferenceEquals(lastResult["currentfreegame"], null))
                return resumeGames;

            if(!object.ReferenceEquals(lastResult["NewYearsBash_FGMessage"], null) && !object.ReferenceEquals(lastResult["NewYearsBash_FGMessage"]["reelTriggerList"], null))
            {
                JArray NewYearsBash_count       = new JArray();
                JArray NewYearsBash_triggerReel = new JArray();
                for (int i = 0; i < lastResult["NewYearsBash_FGMessage"]["reelTriggerList"].Count(); i++)
                {
                    if(!object.ReferenceEquals(lastResult["NewYearsBash_FGMessage"]["reelTriggerList"][i]["countAfter"], null))
                    {
                        int beforeCnt   = (int)lastResult["NewYearsBash_FGMessage"]["reelTriggerList"][i]["countBefore"];
                        int afterCnt    = (int)lastResult["NewYearsBash_FGMessage"]["reelTriggerList"][i]["countAfter"];
                        if(beforeCnt == 10)
                        {
                            NewYearsBash_count.Add(10);
                            NewYearsBash_triggerReel.Add(true);
                        }
                        else
                        {
                            NewYearsBash_count.Add(afterCnt);
                            NewYearsBash_triggerReel.Add(false);
                        }
                    }
                    else
                    {
                        NewYearsBash_count.Add(0);
                        NewYearsBash_triggerReel.Add(false);
                    }
                }
                resumeGames[0]["NewYearsBash_count"]        = NewYearsBash_count;
                resumeGames[0]["NewYearsBash_triggerReel"]  = NewYearsBash_triggerReel;
            }
            return resumeGames;
        }
    }
}
