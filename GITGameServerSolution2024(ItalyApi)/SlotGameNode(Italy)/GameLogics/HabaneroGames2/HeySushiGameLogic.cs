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
    public class HeySushiGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGHeySushi";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "41944e61-1acb-4911-b0df-67ea5aa085e0";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "06a92b0ae0e6113e10b7ed1ae5d2288e04d3ae22";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.6465.345";
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
                    {1,     new HabaneroLogSymbolIDName{id = "idWild",          name = "Wild"           } },
                    {2,     new HabaneroLogSymbolIDName{id = "idScatter",       name = "Scatter"        } },
                    {3,     new HabaneroLogSymbolIDName{id = "idEelRiceBowl",   name = "EelRiceBowl"    } },    //장어밥
                    {4,     new HabaneroLogSymbolIDName{id = "idBluefinTuna",   name = "BluefinTuna"    } },    //참다랑어
                    {5,     new HabaneroLogSymbolIDName{id = "idEggOmelet",     name = "EggOmelet"      } },    //닭부침
                    {6,     new HabaneroLogSymbolIDName{id = "idPrawn",         name = "Prawn"          } },    //새우
                    {7,     new HabaneroLogSymbolIDName{id = "idSeaUrchin",     name = "SeaUrchin"      } },    //성게
                    {8,     new HabaneroLogSymbolIDName{id = "idHandRoll",      name = "HandRoll"       } },    //핸드롤
                    {9,     new HabaneroLogSymbolIDName{id = "idTokoyaki",      name = "Tokoyaki"       } },    //빵
                    {10,    new HabaneroLogSymbolIDName{id = "idA",             name = "A"              } },
                    {11,    new HabaneroLogSymbolIDName{id = "idK",             name = "K"              } },
                    {12,    new HabaneroLogSymbolIDName{id = "idQ",             name = "Q"              } },
                    {13,    new HabaneroLogSymbolIDName{id = "idJ",             name = "J"              } },
                    {14,    new HabaneroLogSymbolIDName{id = "id10",            name = "10"             } },
                    {15,    new HabaneroLogSymbolIDName{id = "id9",             name = "9"              } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 498;
            }
        }

        private List<List<int>> MultiplierGroup
        {
            get
            {
                return new List<List<int>>()
                {
                    new List<int>(){1,  2,  3,  5   },
                    new List<int>(){2,  3,  5,  7   },
                    new List<int>(){3,  5,  7,  10  },
                    new List<int>(){5,  7,  10, 15  },
                };
            }

        }
        #endregion

        public HeySushiGameLogic()
        {
            _gameID     = GAMEID.HeySushi;
            GameName    = "HeySushi";
        }

        protected override JObject buildEventItem(string strGlobalUserId, int currentIndex)
        {
            dynamic eventItem   = base.buildEventItem(strGlobalUserId, currentIndex);

            int caseCadeNo = 1;
            int multiplier = 1;
            if(_dicUserHistory[strGlobalUserId].Responses[currentIndex].Action == HabaneroActionType.MAINCASECADE ||
                _dicUserHistory[strGlobalUserId].Responses[currentIndex].Action == HabaneroActionType.FREEGAMECASCADE)
            {
                caseCadeNo = findCasCadeNoForLog(strGlobalUserId, currentIndex,caseCadeNo);
            }
            multiplier = findMultipleForLog(strGlobalUserId, currentIndex, multiplier);

            eventItem["cascadeno"]  = caseCadeNo;
            eventItem["multiplier"] = multiplier;

            dynamic response = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserId].Responses[currentIndex].Response);

            if (!object.ReferenceEquals(response["wincash"], null) && (double)response["wincash"] > 0)
            {
                if (!object.ReferenceEquals(response["currentfreegame"], null))
                    eventItem["spinno"] = (int)response["currentfreegame"] + 1;
            }

            if (!object.ReferenceEquals(response["prizelist"], null) && response["prizelist"].Count > 0)
            {
                JArray srijbonuspays = new JArray();
                for(int i = 0; i < response["prizelist"].Count; i++)
                {
                    JObject item = new JObject();
                    item["wincash"] = response["prizelist"][i]["wincash"];
                    item["symbol"]  = response["prizelist"][i]["symbol"];
                    srijbonuspays.Add(item);
                }
                eventItem["srijbonuspays"] = srijbonuspays;

                JArray customSubEvents  = new JArray();
                JObject customSubItem   = new JObject();
                customSubItem["type"]   = "HeySushi";
                JArray bpayl = new JArray();
                for (int i = 0; i < response["prizelist"].Count; i++)
                {
                    JObject item = new JObject();
                    item["pay"] = response["prizelist"][i]["wincash"];
                    item["sid"] = SymbolIdStringForLog[(int)response["prizelist"][i]["symbolid"]].id;
                    bpayl.Add(item);
                }
                customSubItem["bpayl"] = bpayl;
                customSubEvents.Add(customSubItem);
                eventItem["customsubevents"] = customSubEvents;
            }

            return eventItem;
        }

        private int findCasCadeNoForLog(string strGlobalUserId,int currentIndex,int casCadeNo)
        {
            for (int i = currentIndex; i > 0; i--)
            {
                HabaneroActionType stepAction = _dicUserHistory[strGlobalUserId].Responses[i].Action;
                if (stepAction == HabaneroActionType.MAIN || stepAction == HabaneroActionType.FREEGAME)
                    break;
                casCadeNo ++;
            }
            return casCadeNo;
        }

        private int findMultipleForLog(string strGlobalUserId, int currentIndex, int multiplier)
        {
            if (currentIndex == 0)
                return multiplier;

            dynamic response = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserId].Responses[currentIndex - 1].Response);
            int groupId = response["multipliergroupid"];
            int index   = response["multiplierindex"];
            multiplier  = MultiplierGroup[groupId][index];
            return multiplier;
        }

        protected override void convertWinsByBet(dynamic resultContext, float currentBet)
        {
            base.convertWinsByBet(resultContext as JObject, currentBet);

            if (!object.ReferenceEquals(resultContext["prizelist"], null))
            {
                for (int i = 0; i < resultContext["prizelist"].Count; i++)
                    resultContext["prizelist"][i]["wincash"] = convertWinByBet((double)resultContext["prizelist"][i]["wincash"], currentBet);
            }
        }

        protected override JArray buildInitResumeGame(string strGlobalUserId, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            dynamic response    = lastResult as dynamic;
            dynamic resumeGames = base.buildInitResumeGame(strGlobalUserId, betInfo, lastResult, gameinstanceid, roundid, currentAction);
            resumeGames[0]["multipliergroupid"] = response["multipliergroupid"];
            resumeGames[0]["multiplierindex"]   = response["multiplierindex"];

            bool hasCasCade = false;
            JArray casCadeDataList = new JArray();
            for(int i = 0; i < 5; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    JObject item = new JObject();
                    item["x"] = i;
                    item["y"] = j;
                    if (!object.ReferenceEquals(response["reels"][i][j]["cascade"],null))
                    {
                        if(!object.ReferenceEquals(response["reels"][i][j]["cascade"]["dropout"], null))
                        {
                            item["dropOut"] = true;
                            hasCasCade      = true;
                        }
                        if (!object.ReferenceEquals(response["reels"][i][j]["cascade"]["dropin"], null))
                            item["dropIn"] = true;
                        if (!object.ReferenceEquals(response["reels"][i][j]["cascade"]["dropdownindex"], null))
                            item["dropDownIndex"] = response["reels"][i][j]["cascade"]["dropdownindex"];

                    }
                    casCadeDataList.Add(item);
                }
            }
            
            resumeGames[0]["cascadedatalist"] = casCadeDataList;

            if (hasCasCade)
            {
                if (!object.ReferenceEquals(response["currentfreegame"], null))
                    resumeGames[0]["gamemode"] = "freegamecascade";
                else
                    resumeGames[0]["gamemode"] = "maincascade";
            }
            return resumeGames;
        }
    }
}
