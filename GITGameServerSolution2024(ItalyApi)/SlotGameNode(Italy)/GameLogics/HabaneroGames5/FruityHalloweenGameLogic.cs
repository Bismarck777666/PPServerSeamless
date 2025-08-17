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
    public class FruityHalloweenGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGFruityHalloween";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "5ed1b911-2509-442c-a891-da7e9fd4e312";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "29fa8b35773b23bb469ee34486de49f5f61f7554";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.12535.0";
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
                    {1,     new HabaneroLogSymbolIDName{id = "idWild",          name = "Wild"       } },
                    {2,     new HabaneroLogSymbolIDName{id = "idScatter",       name = "Scatter"    } },
                    {3,     new HabaneroLogSymbolIDName{id = "idMoon",          name = "Moon"       } },
                    {4,     new HabaneroLogSymbolIDName{id = "idPumpkin",       name = "Pumpkin"    } },
                    {5,     new HabaneroLogSymbolIDName{id = "idApple",         name = "Apple"      } },
                    {6,     new HabaneroLogSymbolIDName{id = "idWatermelon",    name = "Watermelon" } },
                    {7,     new HabaneroLogSymbolIDName{id = "idPear",          name = "Pear"       } },
                    {8,     new HabaneroLogSymbolIDName{id = "idLo1",           name = "Lo1"        } },
                    {9,     new HabaneroLogSymbolIDName{id = "idLo2",           name = "Lo2"        } },
                    {10,    new HabaneroLogSymbolIDName{id = "idLo3",           name = "Lo3"        } },
                    {11,    new HabaneroLogSymbolIDName{id = "idLo4",           name = "Lo4"        } },
                    {12,    new HabaneroLogSymbolIDName{id = "idMoneyBlank",    name = "MoneyBlank" } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 978;
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double[] PurchaseFreeMultiple
        {
            get { return new double[] { 60.0 }; }
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

        public FruityHalloweenGameLogic()
        {
            _gameID     = GAMEID.FruityHalloween;
            GameName    = "FruityHalloween";
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

            bool containMoney = false;
            for (int j = 0; j < response["reels"].Count; j++)
            {
                for (int k = 0; k < response["reels"][j].Count; k++)
                {
                    string symbol = Convert.ToString(response["reels"][j][k]["symbolid"]);
                    if (symbol == "3")
                        containMoney = true;

                    if (containMoney)
                        break;
                }
                if (containMoney)
                    break;
            }

            if (containMoney)
            {
                JArray reelsmeta = buildHabaneroLogReelsMeta(response as JObject);
                eventItem["reels_meta"] = reelsmeta;
            }

            int multiplier = 1;
            if (!object.ReferenceEquals(response["currgamemultiplier"], null))
                multiplier = response["currgamemultiplier"];

            eventItem["multiplier"] = multiplier;

            if (!object.ReferenceEquals(response["wildPosData"], null))
                eventItem["replay_meta"] = response["wildPosData"];

            return eventItem;
        }

        protected override JArray buildHabaneroLogReels(string strGlobalUserID, int currentIndex, dynamic response, bool containWild = false)
        {
            JArray reels = base.buildHabaneroLogReels(strGlobalUserID, currentIndex, response as JObject, containWild);

            if(!object.ReferenceEquals(response["wildPosData"], null))
            {
                for(int i = 0; i < response["wildPosData"]["posList"].Count; i++)
                {
                    int reelindex   = response["wildPosData"]["posList"][i]["reelindex"];
                    int symbolindex = response["wildPosData"]["posList"][i]["symbolindex"];

                    reels[reelindex][symbolindex] = SymbolIdStringForLog[1].id;
                }

                for(int i = 0; i < response["wildPosData"]["clonePosList"].Count; i++)
                {
                    int reelindex   = response["wildPosData"]["clonePosList"][i]["reelindex"];
                    int symbolindex = response["wildPosData"]["clonePosList"][i]["symbolindex"];

                    reels[reelindex][symbolindex] = SymbolIdStringForLog[1].id;
                }
            }

            return reels;
        }

        protected override JArray buildHabaneroLogReelsMeta(dynamic response)
        {
            JArray reelsListMeta = new JArray();

            for (int j = 0; j < response["reels"].Count; j++)
            {
                bool hasMoneyInCol = false;
                for (int k = 0; k < response["reels"][j].Count; k++)
                {
                    string symbol = Convert.ToString(response["reels"][j][k]["symbolid"]);
                    if(symbol == "3")
                    {
                        hasMoneyInCol = true;
                        break;
                    }
                }
                
                if (!hasMoneyInCol)
                {
                    reelsListMeta.Add(null);
                    continue;
                }
                
                JArray reelsMetaCol = new JArray();
                for (int k = 0; k < response["reels"][j].Count; k++)
                {
                    string symbol = Convert.ToString(response["reels"][j][k]["symbolid"]);
                    if(symbol == "3")
                    {
                        JObject moneyMul = new JObject();
                        moneyMul["cash_linebet"] = response["reels"][j][k]["wincashmultiplier"];
                        reelsMetaCol.Add(moneyMul);
                    }
                    else
                        reelsMetaCol.Add(null);
                }

                reelsListMeta.Add(reelsMetaCol);
            }

            if(!object.ReferenceEquals(response["respinState"], null))
            {
                for(int i = 0; i < response["respinState"].Count; i++)
                {
                    for(int j = 0; j < response["respinState"][i].Count; j++)
                    {
                        if((int)response["respinState"][i][j] != 0)
                        {
                            JObject item = new JObject();
                            item["cash_linebet"] = response["respinState"][i][j];
                            reelsListMeta[i][j] = item;
                        }
                    }
                }
                
            }

            return reelsListMeta;
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
