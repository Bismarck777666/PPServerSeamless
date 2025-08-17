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
    public class WitchesTomeGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGWitchesTome";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "a4b15b5c-80ea-4521-b0bd-4a7b734568b6";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "38a6ea849e8d45b5c27a82e16fe92c0501a1bba0";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.11914.0";
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
                return 243;
            }
        }
        protected override string BetType
        {
            get
            {
                return "Ways";
            }
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,     new HabaneroLogSymbolIDName{id = "idWild",          name = "Wild"           } },
                    {2,     new HabaneroLogSymbolIDName{id = "idScatter",       name = "Scatter"        } },
                    {3,     new HabaneroLogSymbolIDName{id = "idTombstone",     name = "Tombstone"      } },
                    {4,     new HabaneroLogSymbolIDName{id = "idBroom",         name = "Broom"          } },
                    {5,     new HabaneroLogSymbolIDName{id = "idCrystal",       name = "Crystal"        } },
                    {6,     new HabaneroLogSymbolIDName{id = "idHat",           name = "Hat"            } },
                    {7,     new HabaneroLogSymbolIDName{id = "idPotionBlue",    name = "PotionBlue"     } },
                    {8,     new HabaneroLogSymbolIDName{id = "idPotionGreen",   name = "PotionGreen"    } },
                    {9,     new HabaneroLogSymbolIDName{id = "idPotionPurple",  name = "PotionPurple"   } },
                    {10,    new HabaneroLogSymbolIDName{id = "idPotionRed",     name = "PotionRed"      } },
                    {11,    new HabaneroLogSymbolIDName{id = "idWildFree",      name = "WildFree"       } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 877;
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double[] PurchaseFreeMultiple
        {
            get { return new double[] { 50.0 }; }
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

        public WitchesTomeGameLogic()
        {
            _gameID     = GAMEID.WitchesTome;
            GameName    = "WitchesTome";
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
                        _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in WitchesTomeGameLogic::readBetInfoFromMessage", strGlobalUserID);
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
                        _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in WitchesTomeGameLogic::readBetInfoFromMessage", strGlobalUserID);
                        return;
                    }

                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in WitchesTomeGameLogic::readBetInfoFromMessage {0}", ex);
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

            if (!object.ReferenceEquals(response["newWildPosList"], null) && response["newWildPosList"].Count > 0)
            {
                for(int i = 0; i < response["newWildPosList"].Count; i++)
                {
                        int reelindex   = response["newWildPosList"][i]["reelindex"];
                        int symbolindex = response["newWildPosList"][i]["symbolindex"];

                        eventItem["reels"][reelindex][symbolindex] = SymbolIdStringForLog[11].id;
                }

                eventItem["reelslist"] = buildHabaneroLogReelslist(response);
            }

            eventItem["multiplier"] = multiplier;
            return eventItem;
        }

        protected override JArray buildHabaneroLogReelslist(dynamic response)
        {
            JArray reels = new JArray();
            for(int i = 0; i < response["reels"].Count; i++)
            {
                JArray col = new JArray();
                for (int j = 0; j < response["reels"][i].Count; j++)
                {
                    int symbolid = response["reels"][i][j]["symbolid"];
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
            
            resumeGames[0]["featureBuy"]    = betInfo.PurchaseFree;
            resumeGames[0]["superBet"]      = betInfo.MoreBet;
            resumeGames[0]["gamemode"]      = lastResult["nextgamestate"];
            return resumeGames;
        }
    }
}
