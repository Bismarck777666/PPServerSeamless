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
    public class SlimePartyGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGSlimeParty";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "5feb5917-0f12-477c-9ac3-4824a77c94d9";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "77be3d1c37df413048c48ebadcb0d40ae8a19096";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.12613.0";
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
                return 0;
            }
        }
        protected override string BetType
        {
            get
            {
                return "Anywhere";
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,     new HabaneroLogSymbolIDName{id = "idWild",              name = "Wild"           } },
                    {2,     new HabaneroLogSymbolIDName{id = "idScatter",           name = "Scatter"        } },
                    {3,     new HabaneroLogSymbolIDName{id = "idAngelSlime",        name = "AngelSlime"     } },
                    {4,     new HabaneroLogSymbolIDName{id = "idDevilSlime",        name = "DevilSlime"     } },
                    {5,     new HabaneroLogSymbolIDName{id = "idNobilitySlime",     name = "NobilitySlime"  } },
                    {6,     new HabaneroLogSymbolIDName{id = "idWizardSlime",       name = "WizardSlime"    } },
                    {7,     new HabaneroLogSymbolIDName{id = "idHeartFruit",        name = "HeartFruit"     } },
                    {8,     new HabaneroLogSymbolIDName{id = "idGrapesFruit",       name = "GrapesFruit"    } },
                    {9,     new HabaneroLogSymbolIDName{id = "idWatermelonFruit",   name = "WatermelonFruit"} },
                    {10,    new HabaneroLogSymbolIDName{id = "idLemonFruit",        name = "LemonFruit"     } },
                    {11,    new HabaneroLogSymbolIDName{id = "idAvocadoFruit",      name = "AvocadoFruit"   } },
                    {12,    new HabaneroLogSymbolIDName{id = "idWild2X",            name = "Wild2X"         } },
                    {13,    new HabaneroLogSymbolIDName{id = "idWild5X",            name = "Wild5X"         } },
                    {14,    new HabaneroLogSymbolIDName{id = "idWild10X",           name = "Wild10X"        } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 948;
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double[] PurchaseFreeMultiple
        {
            get { return new double[] { 999 / 20.0 }; }
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
                return 2.5;
            }
        }
        #endregion

        public SlimePartyGameLogic()
        {
            _gameID     = GAMEID.SlimeParty;
            GameName    = "SlimeParty";
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
                        _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in SlimePartyGameLogic::readBetInfoFromMessage", strGlobalUserID);
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
                        _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in SlimePartyGameLogic::readBetInfoFromMessage", strGlobalUserID);
                        return;
                    }

                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SlimePartyGameLogic::readBetInfoFromMessage {0}", ex);
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

            eventItem["multiplier"] = multiplier;
            return eventItem;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            JArray resumeGames = base.buildInitResumeGame(strGlobalUserID, betInfo, lastResult, gameinstanceid, roundid, currentAction);
            
            resumeGames[0]["SlimeParty_featureBuy"] = betInfo.PurchaseFree;
            resumeGames[0]["SlimeParty_superBet"]   = betInfo.MoreBet;
            resumeGames[0]["gamemode"]              = lastResult["nextgamestate"];
            return resumeGames;
        }
    }
}
