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
    public class DragonsThroneGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGDragonsThrone";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "5754ac29-eb37-44ed-b4da-010d1d488fa5";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "fd4f81fa31f1f71919d2ae00480a6d85ef91711f";
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
                return 50.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 50;
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
                    {3,     new HabaneroLogSymbolIDName{id = "idDragon1",   name = "Dragon1"    } },
                    {4,     new HabaneroLogSymbolIDName{id = "idDragon2",   name = "Dragon2"    } },
                    {5,     new HabaneroLogSymbolIDName{id = "idDragon3",   name = "Dragon3"    } },
                    {6,     new HabaneroLogSymbolIDName{id = "idDragon4",   name = "Dragon4"    } },
                    {7,     new HabaneroLogSymbolIDName{id = "idA",         name = "A"          } },
                    {8,     new HabaneroLogSymbolIDName{id = "idK",         name = "K"          } },
                    {9,     new HabaneroLogSymbolIDName{id = "idQ",         name = "Q"          } },
                    {10,    new HabaneroLogSymbolIDName{id = "idJ",         name = "J"          } },
                    {11,    new HabaneroLogSymbolIDName{id = "id10",        name = "10"         } },
                    {12,    new HabaneroLogSymbolIDName{id = "id9",         name = "9"          } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 278;
            }
        }
        #endregion

        public DragonsThroneGameLogic()
        {
            _gameID     = GAMEID.DragonsThrone;
            GameName    = "DragonsThrone";
        }

        protected override JObject buildEventItem(string strUserId, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strUserId, currentIndex);
            
            dynamic resultContext   = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strUserId].Responses[currentIndex].Response);
            if (!object.ReferenceEquals(resultContext["dragonduelstage"], null))
            {
                int level = (int)resultContext["dragonduelstage"];
                if(level == 1 || level == 2)
                    eventItem["multiplier"] = 1;
                else if(level == 3)
                    eventItem["multiplier"] = 2;
                else if (level == 4)
                    eventItem["multiplier"] = 3;
            }

            dynamic eventContext = eventItem as dynamic;
            if (!object.ReferenceEquals(resultContext["dragonduelplayerdragon"], null))
            {
                JObject subEventItem = new JObject();
                subEventItem["type"]    = "trigger";
                subEventItem["symbol"]  = string.Format("Dragon{0}", resultContext["dragonduelplayerdragon"]);
                JArray windows = new JArray();
                for(int i = 0; i < resultContext["reels"].Count; i++)
                {
                    for(int j = 0; j < resultContext["reels"][i].Count; j++)
                    {
                        dynamic reelItem = resultContext["reels"][i][j];
                        if (!object.ReferenceEquals(reelItem["wintypes"], null) && (string)reelItem["wintypes"][0] == "trigger")
                        {
                            JArray window = new JArray();
                            window.Add(i);
                            window.Add(j);
                            windows.Add(window);
                        }
                    }
                }
                subEventItem["windows"] = windows;

                if (!object.ReferenceEquals(eventContext["subevents"], null) && eventContext["subevents"].Count > 0)
                {
                    (eventContext["subevents"] as JArray).Add(subEventItem);
                }
                else
                {
                    JArray subEvents = new JArray();
                    subEvents.Add(subEventItem);

                    eventContext["subevents"] = subEvents;
                }
            }

            double wincash = 0.0;
            if (!object.ReferenceEquals(resultContext["dragonduelbonuswincash"], null))
                wincash += (double)resultContext["dragonduelbonuswincash"];

            if (!object.ReferenceEquals(resultContext["wincash"], null))
                wincash += (double)resultContext["wincash"];

            eventItem["wincash"] = wincash;
            return eventItem;
        }

        protected override JArray buildInitResumeGame(string strUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            dynamic triggerResult   = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strUserID].Responses[0].Response);
            dynamic dragonRoster    = triggerResult["dragonduelopponentroster"];
            dynamic playerDragon    = triggerResult["dragonduelplayerdragon"];

            JArray resumeGames = base.buildInitResumeGame(strUserID, betInfo, lastResult, gameinstanceid, roundid, currentAction);

            if (!object.ReferenceEquals(lastResult["dragonduelopponenthealth"], null))
                resumeGames[0]["dragonduel_opponenthealth"] = lastResult["dragonduelopponenthealth"];

            if (!object.ReferenceEquals(lastResult["dragonduelplayerhealth"], null))
                resumeGames[0]["dragonduel_playerhealth"] = lastResult["dragonduelplayerhealth"];

            if (!object.ReferenceEquals(lastResult["dragonduelstage"], null))
                resumeGames[0]["dragonduel_stage"] = lastResult["dragonduelstage"];

            if (!object.ReferenceEquals(triggerResult["dragonduelopponentroster"], null))
                resumeGames[0]["dragonduel_opponentroster"] = dragonRoster;
            
            if (!object.ReferenceEquals(triggerResult["dragonduelplayerdragon"], null))
                resumeGames[0]["dragonduel_playerdragon"] = playerDragon;

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
