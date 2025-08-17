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
    public class TabernaDeLosMuertosGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGTabernaDeLosMuertos";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "161d3d02-e3f8-41fb-aadd-f2cf49b25b52";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "fceb97611207f9551f7bc4bd8cc664e7a34826e7";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.6698.360";
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
                return 101;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idWild",    name = "Wild"       } },    //와일드
                    {2,   new HabaneroLogSymbolIDName{id = "idScatter", name = "Scatter"    } },    //스캐터
                    {3,   new HabaneroLogSymbolIDName{id = "idGun",     name = "Gun"        } },    //총
                    {4,   new HabaneroLogSymbolIDName{id = "idTequila", name = "Tequila"    } },    //술병
                    {5,   new HabaneroLogSymbolIDName{id = "idTaco",    name = "Taco"       } },    //튀기
                    {6,   new HabaneroLogSymbolIDName{id = "idDice",    name = "Dice"       } },    //주사위
                    {7,   new HabaneroLogSymbolIDName{id = "idHeart",   name = "Heart"      } },    //허트
                    {8,   new HabaneroLogSymbolIDName{id = "idDiamond", name = "Diamond"    } },    //다이야
                    {9,   new HabaneroLogSymbolIDName{id = "idClub",    name = "Club"       } },    //크로바
                    {10,  new HabaneroLogSymbolIDName{id = "idSpade",   name = "Spade"      } },    //스페이드
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 538;
            }
        }
        #endregion

        public TabernaDeLosMuertosGameLogic()
        {
            _gameID     = GAMEID.TabernaDeLosMuertos;
            GameName    = "TabernaDeLosMuertos";
        }

        protected override JObject buildEventItem(string strGlobalUserID, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strGlobalUserID, currentIndex);
            HabaneroHistoryResponses responses = _dicUserHistory[strGlobalUserID].Responses[currentIndex];
            dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            JArray reels        = eventItem["reels"] as JArray;
            JArray reels_srij   = new JArray();
            for (int i = 0; i < reels.Count; i++)
            {
                JArray reels_srijCol    = new JArray();
                JArray colum            = reels[i] as JArray;
                for(int j = 0; j < colum.Count; j++)
                    reels_srijCol.Add(colum[j]);
                reels_srij.Add(reels_srijCol);
            }

            if (!object.ReferenceEquals(resultContext["wildFeature"], null))
            {
                JArray reels10 = new JArray() { 
                    new JArray(){null,null,null},
                    new JArray(){null,null,null},
                    new JArray(){null,null,null},
                    new JArray(){null,null,null},
                    new JArray(){null,null,null},
                };

                for(int i = 0; i < resultContext["wildFeature"]["wildPostList"].Count; i++)
                {
                    int col = (int)resultContext["wildFeature"]["wildPostList"][i]["reelindex"];
                    int row = (int)resultContext["wildFeature"]["wildPostList"][i]["symbolindex"];
                    reels10[col][row]       = "idWild2";
                    reels_srij[col][row]    = "idWild";
                }
                eventItem["reels10"] = reels10;

                JArray reelsList        = new JArray();
                JObject reelsListItem   = new JObject();
                reelsListItem["reels"]  = reels;
                reelsList.Add(reelsListItem);
                
                eventItem["reelslist"]  = reelsList;
            }
            eventItem["reels_srij"] = reels_srij;

            if (!object.ReferenceEquals(resultContext["currentfreegame"], null))
            {
                eventItem["multiplier"] = 1;
                if (!object.ReferenceEquals(resultContext["multiplierFeature"], null))
                    eventItem["multiplier"] = (int)((double)resultContext["multiplierFeature"]["winCashEnd"] / (double)resultContext["multiplierFeature"]["winCashStart"]);

                if (!object.ReferenceEquals(resultContext["linewins"], null) && resultContext["linewins"].Count > 0)
                    eventItem["numwinlines"] = resultContext["linewins"].Count;
            }

            return eventItem;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            JArray resumeGames = base.buildInitResumeGame(strGlobalUserID, betInfo, lastResult, gameinstanceid, roundid, currentAction);

            if (!object.ReferenceEquals(lastResult["wildFeature"], null))
            {
                JArray lastWildPosList = lastResult["wildFeature"]["wildPostList"] as JArray;
                resumeGames[0]["lastWildPosList"] = lastWildPosList;
            }
            return resumeGames;
        }
    }
}
