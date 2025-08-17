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
    public class TootyFruityFruitsGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGTootyFruityFruits";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "21e2af31-6c7d-4ae0-a693-9d641e9d3f49";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "f6e942fb941a33bf13cea14331a41a3a76cbf0ef";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.11745.0";
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
                    {1,     new HabaneroLogSymbolIDName{id = "idWild",          name = "Wild"       } },
                    {2,     new HabaneroLogSymbolIDName{id = "idScatter",       name = "Scatter"    } },
                    {3,     new HabaneroLogSymbolIDName{id = "idOrange",        name = "Orange"     } },
                    {4,     new HabaneroLogSymbolIDName{id = "idApple",         name = "Apple"      } },
                    {5,     new HabaneroLogSymbolIDName{id = "idPlum",          name = "Plum"       } },
                    {6,     new HabaneroLogSymbolIDName{id = "idPineapple",     name = "Pineapple"  } },
                    {7,     new HabaneroLogSymbolIDName{id = "idStrawberry",    name = "Strawberry" } },
                    {8,     new HabaneroLogSymbolIDName{id = "idBlueberry",     name = "Blueberry"  } },
                    {9,     new HabaneroLogSymbolIDName{id = "idBlank",         name = "Blank"      } },
                    
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 818;
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double[] PurchaseFreeMultiple
        {
            get { return new double[] { 51.0 }; }
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

        public TootyFruityFruitsGameLogic()
        {
            _gameID     = GAMEID.TootyFruityFruits;
            GameName    = "TootyFruityFruits";
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
                        _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in TootyFruityFruitsGameLogic::readBetInfoFromMessage", strGlobalUserID);
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
                        _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in TootyFruityFruitsGameLogic::readBetInfoFromMessage", strGlobalUserID);
                        return;
                    }

                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in TootyFruityFruitsGameLogic::readBetInfoFromMessage {0}", ex);
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

            if (!object.ReferenceEquals(response["TootyFruit_dropOutList"], null))
            {
                JObject replay_meta = new JObject();
                replay_meta["dropOutList"] = response["TootyFruit_dropOutList"];

                eventItem["replay_meta"] = replay_meta;
            }

            JArray reels = buildHabaneroLogReels(strGlobalUserID, currentIndex, response);
            if(currentIndex > 0)
            {
                HabaneroHistoryResponses beforeresponses = _dicUserHistory[strGlobalUserID].Responses[currentIndex - 1];
                dynamic beforeresponse = JsonConvert.DeserializeObject<dynamic>(beforeresponses.Response);

                if(!object.ReferenceEquals(beforeresponse["TootyFruit_dropOutList"], null))
                {
                    for(int i = 0; i < beforeresponse["TootyFruit_dropOutList"].Count; i++)
                    {
                        for(int j = 0; j < beforeresponse["TootyFruit_dropOutList"][i].Count; j++)
                        {
                            bool flag = beforeresponse["TootyFruit_dropOutList"][i][j];
                            if (flag)
                                reels[i][j] = SymbolIdStringForLog[9].id;
                        }
                    }

                    JArray reelslist        = new JArray();
                    JObject reelslistitem   = new JObject();
                    reelslistitem["reels"] = reels;
                    reelslist.Add(reelslistitem);

                    eventItem["reelslist"]  = reelslist;
                }
            }
                
            return eventItem;
        }

        protected override JArray buildHabaneroLogReelslist(dynamic response)
        {
            JArray reels = new JArray();
            for(int i = 0; i < response["virtualreels"].Count; i++)
            {
                JArray col = new JArray();
                for (int j = 2; j < response["virtualreels"][i].Count - 2; j++)
                {
                    int symbolid = response["virtualreels"][i][j];
                    col.Add(SymbolIdStringForLog[symbolid].id);
                }
                reels.Add(JsonConvert.DeserializeObject(JsonConvert.SerializeObject(col)));
            }
            JArray reelslist        = new JArray();
            JObject reelslistitem   = new JObject();
            reelslistitem["reels"]  = reels;
            reelslist.Add(reelslistitem);

            return reelslist;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            JArray resumeGames = base.buildInitResumeGame(strGlobalUserID, betInfo, lastResult, gameinstanceid, roundid, currentAction);
            
            resumeGames[0]["TootyFruit_featureBuy"] = betInfo.PurchaseFree;
            resumeGames[0]["TootyFruit_superBet"]   = betInfo.MoreBet;
            resumeGames[0]["gamemode"]              = lastResult["nextgamestate"];
            if(!object.ReferenceEquals(lastResult["TootyFruit_dropOutList"], null))
            {
                resumeGames[0]["TootyFruit_dropOutList"] = lastResult["TootyFruit_dropOutList"];
            }
            return resumeGames;
        }
    }
}
