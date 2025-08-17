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
    public class VikingsPlunderGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGVikingsPlunder";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "74a8f342-1d68-4fa5-8769-65edd583b2ec";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "0d8354c900fd9cc4c4ea31fa7b200ab0e394d355";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.12512.857";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idHammer",      name = "Hammer"     } },
                    {2,   new HabaneroLogSymbolIDName{id = "idViking",      name = "Viking"     } },
                    {3,   new HabaneroLogSymbolIDName{id = "idLongboat",    name = "Longboat"   } },
                    {4,   new HabaneroLogSymbolIDName{id = "idLady",        name = "Lady"       } },
                    {5,   new HabaneroLogSymbolIDName{id = "idBoar",        name = "Boar"       } },
                    {6,   new HabaneroLogSymbolIDName{id = "idShield",      name = "Shield"     } },
                    {7,   new HabaneroLogSymbolIDName{id = "idHelmet",      name = "Helmet"     } },
                    {8,   new HabaneroLogSymbolIDName{id = "idMug",         name = "Mug"        } },
                    {9,   new HabaneroLogSymbolIDName{id = "idA",           name = "A"          } },
                    {10,  new HabaneroLogSymbolIDName{id = "idK",           name = "K"          } },
                    {11,  new HabaneroLogSymbolIDName{id = "idQ",           name = "Q"          } },
                    {12,  new HabaneroLogSymbolIDName{id = "idJ",           name = "J"          } },
                    {13,  new HabaneroLogSymbolIDName{id = "id10",          name = "10"         } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 186;
            }
        }
        #endregion

        public VikingsPlunderGameLogic()
        {
            _gameID     = GAMEID.VikingsPlunder;
            GameName    = "VikingsPlunder";
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);
            HabaneroHistoryResponses responses = _dicUserHistory[strUserId].Responses[currentIndex];
            dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            if (!object.ReferenceEquals(resultContext["currentfreegame"], null))
                eventItem["multiplier"] = 1;

            return eventItem;
        }

        protected override JArray buildHabaneroLogReels(string strUserID,int currentIndex ,dynamic response, bool containWild = false)
        {
            HabaneroHistoryResponses currentRes = _dicUserHistory[strUserID].Responses[currentIndex];
            if (currentRes.Action != HabaneroActionType.RESPIN)
                return base.buildHabaneroLogReels(strUserID, currentIndex, response as JObject);

            JArray logReels = new JArray();
            for (int i = 0; i < response["reels"].Count; i++)
            {
                JArray col = new JArray();
                
                for (int j = 0; j < response["reels"][i].Count; j++)
                {
                    if (i == 0)
                        col.Add(SymbolIdStringForLog[1].id);
                    else
                    {
                        int symbol = Convert.ToInt32(response["reels"][i][j]["symbolid"]);
                        col.Add(SymbolIdStringForLog[symbol].id);
                    }
                }
                logReels.Add(col);
            }
            return logReels;
        }

        protected override JArray buildInitResumeGame(string strUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            JArray resumeGames = base.buildInitResumeGame(strUserID, betInfo, lastResult, gameinstanceid, roundid, currentAction);
            resumeGames[0]["gamemode"] = lastResult["nextgamestate"];
            return resumeGames;
        }
    }
}
