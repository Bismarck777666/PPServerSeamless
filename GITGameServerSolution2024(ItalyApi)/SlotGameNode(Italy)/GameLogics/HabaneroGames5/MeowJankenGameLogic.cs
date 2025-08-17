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
    public class MeowJankenGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGMeowJanken";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "2073e5d1-d966-42da-8cd7-718cf1049f07";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "d47ef1bff0ea8bdbfe90947a4f2662c52da956be";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.12302.0";
            }
        }
        protected override float MiniCoin
        {
            get
            {
                return 3.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 1;
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,     new HabaneroLogSymbolIDName{id = "idWild",              name = "Wild"           } },
                    {2,     new HabaneroLogSymbolIDName{id = "idStone",             name = "Stone"          } },
                    {3,     new HabaneroLogSymbolIDName{id = "idScissor",           name = "Scissor"        } },
                    {4,     new HabaneroLogSymbolIDName{id = "idPaper",             name = "Paper"          } },
                    {5,     new HabaneroLogSymbolIDName{id = "idUp3",               name = "Up3"            } },
                    {6,     new HabaneroLogSymbolIDName{id = "idUp2",               name = "Up2"            } },
                    {7,     new HabaneroLogSymbolIDName{id = "idUp1",               name = "Up1"            } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 965;
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double[] PurchaseFreeMultiple
        {
            get { return new double[] { 140 / 3.0 }; }
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

        public MeowJankenGameLogic()
        {
            _gameID     = GAMEID.MeowJanken;
            GameName    = "MeowJanken";
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
                        _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in FruityHalloweenGameLogic::readBetInfoFromMessage", strGlobalUserID);
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
                        _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in FruityHalloweenGameLogic::readBetInfoFromMessage", strGlobalUserID);
                        return;
                    }

                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SantasInnGameLogic::readBetInfoFromMessage {0}", ex);
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

            if (!object.ReferenceEquals(response["rpsSymbols"], null))
                eventItem["rpsSymbols"] = response["rpsSymbols"];

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
