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
    public class BikiniIslandDeluxeGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGBikiniIslandDeluxe";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "83e7d040-28cf-4b36-a620-8411cfbc05f8";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "a0af3477f33bddc712f19fd028ae9994068368c9";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.12299.0";
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
                return 178;
            }
        }
        protected override string BetType
        {
            get
            {
                return "HorizontalVerticalPays";
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,     new HabaneroLogSymbolIDName{id = "idWild1",     name = "Wild1"      } },
                    {2,     new HabaneroLogSymbolIDName{id = "idWild2",     name = "Wild2"      } },
                    {3,     new HabaneroLogSymbolIDName{id = "idWild3",     name = "Wild3"      } },
                    {4,     new HabaneroLogSymbolIDName{id = "idScatter",   name = "Scatter"    } },
                    {5,     new HabaneroLogSymbolIDName{id = "idLifeguard", name = "Lifeguard"  } },
                    {6,     new HabaneroLogSymbolIDName{id = "idPartyHut",  name = "PartyHut"   } },
                    {7,     new HabaneroLogSymbolIDName{id = "idDeckChair", name = "DeckChair"  } },
                    {8,     new HabaneroLogSymbolIDName{id = "idBlueFish",  name = "BlueFish"   } },
                    {9,     new HabaneroLogSymbolIDName{id = "idSeashell",  name = "Seashell"   } },
                    {10,    new HabaneroLogSymbolIDName{id = "idFlower",    name = "Flower"     } },
                    {11,    new HabaneroLogSymbolIDName{id = "idBeachBall", name = "BeachBall"  } },
                    {12,    new HabaneroLogSymbolIDName{id = "idStarfish",  name = "Starfish"   } },
                    {13,    new HabaneroLogSymbolIDName{id = "idSunglasses",name = "Sunglasses" } },
                    {14,    new HabaneroLogSymbolIDName{id = "idSunscreen", name = "Sunscreen"  } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 970;
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double[] PurchaseFreeMultiple
        {
            get { return new double[] { 248 / 5.0 }; }
        }
        protected override bool SupportMoreBet
        {
            get
            {
                return true;
            }
        }
        protected override double MoreBetMultiple
        {
            get
            {
                return 2.0;
            }
        }
        #endregion

        public BikiniIslandDeluxeGameLogic()
        {
            _gameID     = GAMEID.BikiniIslandDeluxe;
            GameName    = "BikiniIslandDeluxe";
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
        {
            try
            {
                BaseHabaneroSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strGlobalUserID, out oldBetInfo))
                {
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    float coinValue = (float)message.Pop();
                    int lineCount   = (int)message.Pop();
                    int betLevel    = (int)message.Pop();
                    oldBetInfo.CoinValue    = coinValue;
                    oldBetInfo.LineCount    = lineCount;
                    oldBetInfo.BetLevel     = betLevel;
                    oldBetInfo.PurchaseFree = (int)message.Pop();
                    oldBetInfo.MoreBet      = (int)message.Pop();

                    if (oldBetInfo.MoreBet != 0 && oldBetInfo.PurchaseFree != 0)
                    {
                        _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in BikiniIslandDeluxeGameLogic::readBetInfoFromMessage", strGlobalUserID);
                        return;
                    }
                }
                else
                {
                    float coinValue = (float)message.Pop();
                    int lineCount   = (int)message.Pop();
                    int betLevel    = (int)message.Pop();

                    BaseHabaneroSlotBetInfo betInfo  = new BaseHabaneroSlotBetInfo(MiniCoin);
                    betInfo.CoinValue       = coinValue;
                    betInfo.LineCount       = lineCount;
                    betInfo.BetLevel        = betLevel;
                    betInfo.PurchaseFree    = (int)message.Pop();
                    betInfo.MoreBet         = (int)message.Pop();

                    if (betInfo.MoreBet != 0 && betInfo.PurchaseFree != 0)
                    {
                        _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in BikiniIslandDeluxeGameLogic::readBetInfoFromMessage", strGlobalUserID);
                        return;
                    }

                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BikiniIslandDeluxeGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override JObject buildEventItem(string strGlobalUserID, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strGlobalUserID, currentIndex);
            
            HabaneroHistoryResponses responses = _dicUserHistory[strGlobalUserID].Responses[currentIndex];
            dynamic response = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            int multiplier = 1;
            if (!object.ReferenceEquals(response["currgamemultiplier"], null))
                multiplier = response["currgamemultiplier"];

            if (!object.ReferenceEquals(response["upgradeList"], null) && response["upgradeList"].Count > 0)
            {
                for(int i = 0; i < response["upgradeList"].Count; i++)
                {
                    int symbolId = response["upgradeList"][i]["symbolId"];
                    for(int j = 0; j < response["upgradeList"][i]["posList"].Count; j++)
                    {
                        int reelindex   = response["upgradeList"][i]["posList"][j]["reelindex"];
                        int symbolindex = response["upgradeList"][i]["posList"][j]["symbolindex"];

                        eventItem["reels"][reelindex][symbolindex] = SymbolIdStringForLog[symbolId].id;
                    }
                }
            }

            eventItem["multiplier"] = multiplier;
            return eventItem;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            JArray resumeGames = base.buildInitResumeGame(strGlobalUserID, betInfo, lastResult, gameinstanceid, roundid, currentAction);
            
            resumeGames[0]["featureBuy"]    = betInfo.PurchaseFree;
            resumeGames[0]["superBet"]      = betInfo.MoreBet;
            resumeGames[0]["gamemode"]      = lastResult["nextgamestate"];
            return resumeGames;
        }
    }
}
