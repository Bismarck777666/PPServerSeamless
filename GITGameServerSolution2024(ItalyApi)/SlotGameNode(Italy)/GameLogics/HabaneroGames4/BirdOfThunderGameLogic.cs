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
    public class BirdOfThunderGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGBirdOfThunder";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "441eb960-256f-49f5-80f0-958c7de92cc8";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "29da5a878a220b706e3182a47e0f7e04911e5955";
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
                return 30;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,     new HabaneroLogSymbolIDName{id = "idWild",      name = "Wild"       } },
                    {2,     new HabaneroLogSymbolIDName{id = "idScatter",   name = "Scatter"    } },
                    {3,     new HabaneroLogSymbolIDName{id = "idBear",      name = "Bear"       } },
                    {4,     new HabaneroLogSymbolIDName{id = "idBison",     name = "Bison"      } },
                    {5,     new HabaneroLogSymbolIDName{id = "idSnake",     name = "Snake"      } },
                    {6,     new HabaneroLogSymbolIDName{id = "idOwl",       name = "Owl"        } },
                    {7,     new HabaneroLogSymbolIDName{id = "idDeer",      name = "Deer"       } },
                    {8,     new HabaneroLogSymbolIDName{id = "idA",         name = "A"          } },
                    {9,     new HabaneroLogSymbolIDName{id = "idK",         name = "K"          } },
                    {10,    new HabaneroLogSymbolIDName{id = "idQ",         name = "Q"          } },
                    {11,    new HabaneroLogSymbolIDName{id = "idJ",         name = "J"          } },
                    {12,    new HabaneroLogSymbolIDName{id = "id10",        name = "10"         } },
                    {13,    new HabaneroLogSymbolIDName{id = "idScatterFG", name = "ScatterFG"  } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 314;
            }
        }
        #endregion

        public BirdOfThunderGameLogic()
        {
            _gameID     = GAMEID.BirdOfThunder;
            GameName    = "BirdOfThunder";
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);
            eventItem["multiplier"] = 1;

            dynamic resultContext   = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strUserId].Responses[currentIndex].Response);
            if(!object.ReferenceEquals(resultContext["TB_birdDst"], null) && resultContext["TB_birdDst"].Count > 0)
            {
                JArray oldReels = base.buildHabaneroLogReels(strUserId, currentIndex, resultContext as JObject);
                JArray newReels = base.buildHabaneroLogReels(strUserId, currentIndex, resultContext as JObject);

                for(int i = 0; i < resultContext["TB_birdDst"].Count; i++)
                {
                    int reelIndex   = (int)resultContext["TB_birdDst"][i] / 3;
                    int symbolIndex = (int)resultContext["TB_birdDst"][i] % 3;
                    newReels[reelIndex][symbolIndex] = SymbolIdStringForLog[1].id;
                }

                eventItem["reels"] = newReels;

                JArray  reelsList       = new JArray();
                JObject reelsListItem   = new JObject();
                JObject tlheader        = new JObject();
                JArray pList            = new JArray();
                JObject pListItem       = new JObject();
                pListItem["img"]    = SymbolIdStringForLog[1].id;
                pList.Add(pListItem);
                tlheader["tlkey"]   = "ReelsBeforeX";
                tlheader["plist"]   = pList;

                reelsListItem["tlheader"]   = tlheader;
                reelsListItem["reels"]      = oldReels;
                reelsList.Add(reelsListItem);

                eventItem["reelslist"] = reelsList;
            }
            
            return eventItem;
        }

        protected override JArray buildInitResumeGame(string strUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            dynamic triggerResult   = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strUserID].Responses[0].Response);
            JArray resumeGames = base.buildInitResumeGame(strUserID, betInfo, lastResult, gameinstanceid, roundid, currentAction);

            return resumeGames;
        }

        protected override void convertWinsByBet(dynamic resultContext, float currentBet)
        {
            base.convertWinsByBet(resultContext as JObject, currentBet);

            if(!object.ReferenceEquals(resultContext["dragonduelbonuswincash"], null))
                resultContext["dragonduelbonuswincash"] = convertWinByBet((double)resultContext["dragonduelbonuswincash"], currentBet);

            if (!object.ReferenceEquals(resultContext["dragonduelbonuswinincrementcash"], null))
                resultContext["dragonduelbonuswinincrementcash"] = convertWinByBet((double)resultContext["dragonduelbonuswinincrementcash"], currentBet);
        }
    }
}
