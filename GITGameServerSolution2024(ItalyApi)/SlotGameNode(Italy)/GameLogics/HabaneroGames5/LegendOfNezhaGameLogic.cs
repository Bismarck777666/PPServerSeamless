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
    public class LegendOfNezhaGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGLegendOfNezha";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "1f870235-70d8-4965-83da-6a1514345dd5";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "f8dc7aed4be65984def0191fa3761db9ee303e0e";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.12070.0";
            }
        }
        protected override float MiniCoin
        {
            get
            {
                return 28.0f;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 28;
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
                    {3,     new HabaneroLogSymbolIDName{id = "idWheel",     name = "Wheel"      } },
                    {4,     new HabaneroLogSymbolIDName{id = "idWeapon",    name = "Weapon"     } },
                    {5,     new HabaneroLogSymbolIDName{id = "idArtifact",  name = "Artifact"   } },
                    {6,     new HabaneroLogSymbolIDName{id = "idScarf",     name = "Scarf"      } },
                    {7,     new HabaneroLogSymbolIDName{id = "idAce",       name = "Ace"        } },
                    {8,     new HabaneroLogSymbolIDName{id = "idKing",      name = "King"       } },
                    {9,     new HabaneroLogSymbolIDName{id = "idQueen",     name = "Queen"      } },
                    {10,    new HabaneroLogSymbolIDName{id = "idJack",      name = "Jack"       } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 798;
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double[] PurchaseFreeMultiple
        {
            get { return new double[] { 888 / 28.0 }; }
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
                return 50 / 28.0;
            }
        }
        #endregion

        public LegendOfNezhaGameLogic()
        {
            _gameID     = GAMEID.LegendOfNezha;
            GameName    = "LegendOfNezha";
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

            if (!object.ReferenceEquals(response["nezha_wildReplicateList"], null) && response["nezha_wildReplicateList"].Count > 0)
            {
                for(int i = 0; i < response["nezha_wildReplicateList"].Count; i++)
                {
                        int reelindex   = response["nezha_wildReplicateList"][i]["replicatePos"]["reelindex"];
                        int symbolindex = response["nezha_wildReplicateList"][i]["replicatePos"]["symbolindex"];

                        eventItem["reels"][reelindex][symbolindex] = SymbolIdStringForLog[1].id;
                }

                eventItem["reelslist"] = buildHabaneroLogReelslist(response);
            }

            eventItem["multiplier"] = multiplier;
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
            
            resumeGames[0]["featureBuy"]    = betInfo.PurchaseFree;
            resumeGames[0]["superBet"]      = betInfo.MoreBet;
            resumeGames[0]["gamemode"]      = lastResult["nextgamestate"];
            return resumeGames;
        }
    }
}
