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
    public class OceansCallGameLogic : BaseHabanero1SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGOceansCall";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "8d1c369c-2177-4c58-ac69-53c7b95372a3";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "1e040da43a108c7cbb2266266a2328450d97dd0f";
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
                return 20.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 20;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,     new HabaneroLogSymbolIDName{id = "idHarpy_0",       name = "Harpy_0"    } },
                    {2,     new HabaneroLogSymbolIDName{id = "idHarpy_1",       name = "Harpy_1"    } },
                    {3,     new HabaneroLogSymbolIDName{id = "idHarpy_2",       name = "Harpy_2"    } },
                    {4,     new HabaneroLogSymbolIDName{id = "idHarp",          name = "Harp"       } },
                    {5,     new HabaneroLogSymbolIDName{id = "idScroll",        name = "Scroll"     } },
                    {6,     new HabaneroLogSymbolIDName{id = "idCrown",         name = "Crown"      } },
                    {7,     new HabaneroLogSymbolIDName{id = "idSailor_0",      name = "Sailor_0"   } },
                    {8,     new HabaneroLogSymbolIDName{id = "idSailor_1",      name = "Sailor_1"   } },
                    {9,     new HabaneroLogSymbolIDName{id = "idSailor_2",      name = "Sailor_2"   } },
                    {10,    new HabaneroLogSymbolIDName{id = "idSiren1_0",      name = "Siren1_0"   } },
                    {11,    new HabaneroLogSymbolIDName{id = "idSiren1_1",      name = "Siren1_1"   } },
                    {12,    new HabaneroLogSymbolIDName{id = "idSiren1_2",      name = "Siren1_2"   } },
                    {13,    new HabaneroLogSymbolIDName{id = "idSiren2_0",      name = "Siren2_0"   } },
                    {14,    new HabaneroLogSymbolIDName{id = "idSiren2_1",      name = "Siren2_1"   } },
                    {15,    new HabaneroLogSymbolIDName{id = "idSiren2_2",      name = "Siren2_2"   } },
                    {16,    new HabaneroLogSymbolIDName{id = "idSiren3_0",      name = "Siren3_0"   } },
                    {17,    new HabaneroLogSymbolIDName{id = "idSiren3_1",      name = "Siren3_1"   } },
                    {18,    new HabaneroLogSymbolIDName{id = "idSiren3_2",      name = "Siren3_2"   } },
                    {19,    new HabaneroLogSymbolIDName{id = "idSiren4_0",      name = "Siren4_0"   } },
                    {20,    new HabaneroLogSymbolIDName{id = "idSiren4_1",      name = "Siren4_1"   } },
                    {21,    new HabaneroLogSymbolIDName{id = "idSiren4_2",      name = "Siren4_2"   } },
                    {22,    new HabaneroLogSymbolIDName{id = "idAnchor",        name = "Anchor"     } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 334;
            }
        }
        #endregion

        public OceansCallGameLogic()
        {
            _gameID     = GAMEID.OceansCall;
            GameName    = "OceansCall";
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);

            dynamic resultContext   = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strUserId].Responses[currentIndex].Response);
            if (!object.ReferenceEquals(resultContext["videoslotstate"]["statemessage"], null))
                eventItem["statemessage"] = resultContext["videoslotstate"]["statemessage"];

            int anchorCnt       = 0;
            int newAnchorCnt    = 0;
            for(int i = 0; i <= currentIndex; i++)
            {
                dynamic stepResponse    = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strUserId].Responses[i].Response);
                dynamic reellist        = stepResponse["videoslotstate"]["reellist"];
                for(int j = 0; j < reellist.Count; j++)
                {
                    dynamic col = reellist[j]["symbols"]["symbol"];
                    for (int k = 0; k < col.Count; k++)
                    {
                        int symbolid = (int)col[k]["symbolid"];
                        if (symbolid == 22)
                        {
                            anchorCnt++;
                            if (i == currentIndex)
                                newAnchorCnt++;
                        }
                    }
                }
            }

            if(anchorCnt > 0)
            {
                JArray  customSubEvents     = new JArray();
                
                JObject customSubEventItem  = new JObject();
                customSubEventItem["type"] = "OceansCall";
                JObject data = new JObject();
                data["ac"] = anchorCnt % 3;
                if (newAnchorCnt > 0)
                    data["wag"] = (anchorCnt % 3) == 0 ? 1 : 0;
                else
                    data["wag"] = 0;
                customSubEventItem["type"] = "OceansCall";
                customSubEventItem["data"] = JsonConvert.SerializeObject(data);
                customSubEvents.Add(customSubEventItem);

                eventItem["customsubevents"] = customSubEvents;
            }

            dynamic eventContext    = eventItem as dynamic;
            if (!object.ReferenceEquals(eventContext["subevents"], null) && eventContext["subevents"].Count > 0)
            {
                for (int i = 0; i < eventContext["subevents"].Count; i++)
                {
                    if (eventContext["subevents"][i]["type"] == "scatter")
                    {
                        eventContext["subevents"][i]["symbol"] = SymbolIdStringForLog[4].name;
                    }
                }
            }

            return eventItem;
        }

        protected override JArray buildInitResumeGame(string strUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction)
        {
            JArray resumeGames = base.buildInitResumeGame(strUserID, betInfo, lastResult, gameinstanceid, roundid, currentAction);

            int anchorCnt   = 0;
            int historyCnt  = _dicUserHistory[strUserID].Responses.Count;
            for(int i = 1; i < historyCnt; i++)
            {
                dynamic stepResponse    = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strUserID].Responses[i].Response);
                dynamic reellist        = stepResponse["videoslotstate"]["reellist"];
                for(int j = 0; j < reellist.Count; j++)
                {
                    dynamic col = reellist[j]["symbols"]["symbol"];
                    for (int k = 0; k < col.Count; k++)
                    {
                        int symbolid = (int)col[k]["symbolid"];
                        if (symbolid == 22)
                            anchorCnt++;
                    }
                }
            }

            dynamic resultContext = lastResult;
            if(!object.ReferenceEquals(resultContext["videoslotstate"]["freespinnumber"], null))
                resumeGames[0]["states"][0]["gamedata"] = (anchorCnt % 3).ToString();

            return resumeGames;
        }
    }
}
