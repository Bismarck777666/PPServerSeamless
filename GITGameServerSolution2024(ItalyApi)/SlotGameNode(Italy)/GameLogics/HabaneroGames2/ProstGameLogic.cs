using GITProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCGSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    public class ProstGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGProst";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "7b34ba9e-9062-44de-b825-ac43e5ee4cb1";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "0b6edfe31a5241ad8d27a90547ad6c5a4311031a";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.9206.401";
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
                    {1,   new HabaneroLogSymbolIDName{id = "idHans",        name = "Hans"       } },    //와일드(남)
                    {2,   new HabaneroLogSymbolIDName{id = "idHeidi",       name = "Heidi"      } },    //와일드(여)
                    {3,   new HabaneroLogSymbolIDName{id = "idProst",       name = "Prost"      } },    //프로스트(맥주)
                    {4,   new HabaneroLogSymbolIDName{id = "idAccordion",   name = "Accordion"  } },    //손풍금
                    {5,   new HabaneroLogSymbolIDName{id = "idBarrel",      name = "Barrel"     } },    //맥주통
                    {6,   new HabaneroLogSymbolIDName{id = "idPretzel",     name = "Pretzel"    } },    //빵
                    {7,   new HabaneroLogSymbolIDName{id = "idSausages",    name = "Sausages"   } },    //쏘세지
                    {8,   new HabaneroLogSymbolIDName{id = "idA",           name = "A"          } },
                    {9,   new HabaneroLogSymbolIDName{id = "idK",           name = "K"          } },
                    {10,  new HabaneroLogSymbolIDName{id = "idQ",           name = "Q"          } },
                    {11,  new HabaneroLogSymbolIDName{id = "idJ",           name = "J"          } },
                    {12,  new HabaneroLogSymbolIDName{id = "id10",          name = "10"         } },
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 603;
            }
        }

        private Dictionary<int, List<HouseType>> _prostMapList = null;
        
        #endregion

        public ProstGameLogic()
        {
            _gameID     = GAMEID.Prost;
            GameName    = "Prost";
            _prostMapList = new Dictionary<int, List<HouseType>>()
            {
                { 0, new List<HouseType>(){
                    new HouseType(15,   0),
                    new HouseType(3,    1),
                    new HouseType(6,    2),
                    new HouseType(19,   3),
                    new HouseType(16,   4),
                    new HouseType(7,    5),
                    new HouseType(5,    6),
                    new HouseType(8,    7),
                    new HouseType(2,    8),
                } },
                { 1, new List<HouseType>(){
                    new HouseType(17,   0),
                    new HouseType(18,   1),
                    new HouseType(14,   2),
                    new HouseType(19,   3),
                    new HouseType(2,    4),
                    new HouseType(16,   5),
                    new HouseType(7,    6),
                    new HouseType(8,    7),
                    new HouseType(6,    8),
                } },
            };
        }

        protected override void onDoInit(string strGlobalUserID, int currency, GITMessage message, UserBonus userBonus, double userBalance, bool isMustLose)
        {
            try
            {
                string strGrid          = (string)message.Pop();
                string strToken         = (string)message.Pop();

                HabaneroResponse response = new HabaneroResponse();
                
                HabaneroResponseHeader header = makeHabaneroResponseHeader(strGlobalUserID, currency, userBalance, strToken);

                JObject portMessage = new JObject();
                portMessage["reelid"]           = InitReelStatusNo;
                portMessage["pssid"]            = Guid.NewGuid().ToString();
                portMessage["Prost_betStates"]  = buildInitProstBet();
                
                if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
                {
                    BaseHabaneroSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID];
                    string gameInstanceId   = _dicUserResultInfos[strGlobalUserID].GameId;
                    string gameRoundId      = _dicUserResultInfos[strGlobalUserID].RoundId;

                    dynamic lastResult = JsonConvert.DeserializeObject<dynamic>(_dicUserResultInfos[strGlobalUserID].ResultString);
                    if(!object.ReferenceEquals(lastResult["isgamedone"], null) && !Convert.ToBoolean(lastResult["isgamedone"]))
                    {
                        JArray resumeGames = buildInitResumeGame(strGlobalUserID, betInfo,lastResult,gameInstanceId,gameRoundId,_dicUserResultInfos[strGlobalUserID].CurrentAction);
                        portMessage["resumegames"]  = resumeGames;
                        portMessage["gssid"]        = lastResult["gssid"];
                        if (!object.ReferenceEquals(lastResult["pssid"], null))
                            portMessage["pssid"] = lastResult["pssid"];
                    }
                }

                JObject game = new JObject();
                game["action"]      = "init";
                game["apiversion"]  = "5.1.10915.753";
                game["brandgameid"] = BrandGameId;
                game["friendlyid"]  = 0;
                game["gamehash"]    = GameHash;
                game["gameid"]      = "00000000-0000-0000-0000-000000000000";
                game["gameversion"] = GameVersion;
                game["jphash"]      = JPHash;
                game["jpversion"]   = JPVersion;
                game["rnghash"]     = RngHash;
                game["rngversion"]  = RngVersion;
                game["sessionid"]   = Guid.NewGuid().ToString();
                game["init"]        = new JObject();
                game["init"]["coinsincrement"]  = string.Join("|", CoinsIncrement);
                
                List<double> newStakeIncrement = new List<double>();
                for (int i = 0; i < StakeIncrement.Length; i++)
                {
                    newStakeIncrement.Add(StakeIncrement[i] * new Currencies()._currencyInfo[currency].Rate);
                }

                game["init"]["stakeincrement"]  = string.Join("|", StakeIncrement);
                game["init"]["configid"]        = Guid.NewGuid().ToString();
                game["init"]["defaultstake"]    = newStakeIncrement[4];
                game["init"]["maxpaylimit"]     = MaxPayLimit * newStakeIncrement[newStakeIncrement.Count - 1];
                game["init"]["maxstake"]        = MiniCoin * CoinsIncrement[CoinsIncrement.Length - 1] * newStakeIncrement[newStakeIncrement.Count - 1];
                game["init"]["minstake"]        = MiniCoin * CoinsIncrement[0] * newStakeIncrement[0];

                response.game           = game;
                response.header         = header;
                response.grid           = strGrid;
                response.portmessage    = portMessage;
                
                GITMessage responseMessage      = new GITMessage((ushort)SCMSG_CODE.SC_HABANERO_DOINIT);
                responseMessage.Append(JsonConvert.SerializeObject(response));
                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in ProstGameLogic::onDoInit GameID: {0}, {1}", _gameID, ex);
            }
        }
        
        private JObject buildInitProstBet()
        {
            JObject prostBet = new JObject();
            for(int i = 0; i < CoinsIncrement.Length; i++)
            {
                for(int j = 0; j < StakeIncrement.Length; j++)
                {
                    string strKey           = string.Format("{0};{1:N6}", CoinsIncrement[i], StakeIncrement[j]);
                    JObject prostBetItem    = new JObject();
                    JArray prostMapLists    = new JArray();

                    foreach(KeyValuePair<int,List<HouseType>> pair in _prostMapList)
                    {
                        JObject prostMapItem        = new JObject();
                        prostMapItem["charPos"]     = 4;
                        prostMapItem["mapId"]       = pair.Key;
                        prostMapItem["name"]        = pair.Key == 0 ? "HANS" : "HEIDI";

                        JArray nodeList = new JArray();
                        foreach(HouseType house in pair.Value)
                            nodeList.Add(JObject.FromObject(house));
                        prostMapItem["nodeList"]    = JArray.FromObject(pair.Value);
                        prostMapLists.Add(prostMapItem);
                    }
                    prostBetItem["prost_mapList"] = prostMapLists;
                    prostBet[strKey]        = prostBetItem;
                }
            }
            return prostBet;
        }

        protected override BaseHabaneroSlotSpinResult calculateResult(string strGlobalUserID, BaseHabaneroSlotBetInfo betInfo, string strSpinResponse, bool isFirst, HabaneroActionType currentAction)
        {
            try
            {
                BaseHabaneroSlotSpinResult spinResult = new BaseHabaneroSlotSpinResult();
                dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(strSpinResponse);
                if (!object.ReferenceEquals(resultContext["prost_mapActionList"], null) && resultContext["prost_mapActionList"].Count > 0)
                    changeProstMapActionList(resultContext);
                if (!object.ReferenceEquals(resultContext["prost_mapList"], null) && resultContext["prost_mapList"].Count > 0)
                    changeProstMapList(resultContext);

                //모든 당첨값들을 현재의 베팅금액상태로 전환한다.
                convertWinsByBet(resultContext, betInfo.TotalBet);

                string strNextAction    = (string)resultContext["nextgamestate"];
                spinResult.NextAction   = convertStringToAction(strNextAction);

                if (spinResult.NextAction == HabaneroActionType.NONE)
                {
                    _logger.Error("Unknown Action in ProstGameLogic::calculateResult Action is {0}", strNextAction);
                    return null;
                }

                spinResult.CurrentAction = currentAction;

                if (isFirst)
                {
                    spinResult.RoundId  = (((long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds) * 5).ToString();
                    spinResult.GameId   = Guid.NewGuid().ToString();
                }
                else
                {
                    spinResult.RoundId  = _dicUserResultInfos[strGlobalUserID].RoundId;
                    spinResult.GameId   = _dicUserResultInfos[strGlobalUserID].GameId;
                }

                if (spinResult.NextAction == HabaneroActionType.MAIN)
                    spinResult.TotalWin = Convert.ToDouble(resultContext["totalwincash"]);

                spinResult.ResultString = JsonConvert.SerializeObject(resultContext);
                return spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in ProstGameLogic::calculateResult {0}", ex);
                return null;
            }
        }

        private void changeProstMapActionList(dynamic resultContext)
        {
            for(int i = 0; i < resultContext["prost_mapActionList"].Count; i++)
            {
                int mapId       = (int)resultContext["prost_mapActionList"][i]["mapId"];
                int stepCount   = (int)resultContext["prost_mapActionList"][i]["stepCount"];
                List<int> availableNewHosueType = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
                foreach(HouseType house in _prostMapList[mapId])
                {
                    int index = availableNewHosueType.FindIndex(_ => _ == house.houseType);
                    if (index != -1)
                        availableNewHosueType.RemoveAt(index);
                }
                
                for(int j = 0; j < stepCount; j++)
                {
                    int index           = Pcg.Default.Next(0, availableNewHosueType.Count);
                    int newHouseType    = availableNewHosueType[index];
                    availableNewHosueType.RemoveAt(index);

                    resultContext["prost_mapActionList"][i]["removeNodeList"][j]["houseType"]   = _prostMapList[mapId][0].houseType;
                    resultContext["prost_mapActionList"][i]["removeNodeList"][j]["nodeId"]      = _prostMapList[mapId][0].nodeId;

                    resultContext["prost_mapActionList"][i]["addedNodeList"][j]["houseType"]    = newHouseType;
                    resultContext["prost_mapActionList"][i]["addedNodeList"][j]["nodeId"]       = _prostMapList[mapId].Last().nodeId + 1;

                    _prostMapList[mapId].RemoveAt(0);
                    _prostMapList[mapId].Add(new HouseType(newHouseType, _prostMapList[mapId].Last().nodeId + 1));
                }
            }
        }

        private void changeProstMapList(dynamic resultContext)
        {
            JArray prostMapLists    = new JArray();
            foreach(KeyValuePair<int,List<HouseType>> pair in _prostMapList)
            {
                JObject prostMapItem        = new JObject();
                prostMapItem["charPos"]     = 4;
                prostMapItem["mapId"]       = pair.Key;
                prostMapItem["name"]        = pair.Key == 0 ? "HANS" : "HEIDI";

                JArray nodeList = new JArray();
                foreach(HouseType house in pair.Value)
                    nodeList.Add(JObject.FromObject(house));
                prostMapItem["nodeList"]    = JArray.FromObject(pair.Value);
                prostMapLists.Add(prostMapItem);
            }
            resultContext["prost_mapList"] = prostMapLists;
        }

        protected override void convertWinsByBet(dynamic resultContext, float currentBet)
        {
            base.convertWinsByBet(resultContext as JObject, currentBet);

            if (!object.ReferenceEquals(resultContext["prost_cashPrizeList"], null) && resultContext["prost_cashPrizeList"].Count > 0)
            {
                for (int i = 0; i < resultContext["prost_cashPrizeList"].Count; i++)
                    resultContext["prost_cashPrizeList"][i]["winCash"] = convertWinByBet((double)resultContext["prost_cashPrizeList"][i]["winCash"], currentBet);
            }
        }
        protected override JObject buildEventItem(string strGlobalUserID, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strGlobalUserID, currentIndex);
            HabaneroHistoryResponses startResponses = _dicUserHistory[strGlobalUserID].Responses[0];
            HabaneroHistoryResponses responses      = _dicUserHistory[strGlobalUserID].Responses[currentIndex];

            dynamic startResponse   = JsonConvert.DeserializeObject<dynamic>(startResponses.Response);
            dynamic response        = JsonConvert.DeserializeObject<dynamic>(responses.Response);

            if (!object.ReferenceEquals(response["currentfreegame"], null))
                eventItem["multiplier"] = startResponse["winfreemultiplier"];
            else
            {
                if (!object.ReferenceEquals(response["prost_mapList"], null) && response["prost_mapList"].Count > 0)
                {
                    JArray customEvents = new JArray();
                    JObject customEventItem = new JObject();
                    customEventItem["type"] = "ProstGameReport";

                    for (int i = 0; i < response["prost_mapList"].Count; i++)
                    {
                        JObject prostLines = new JObject();
                        JArray prostLineArray = new JArray();
                        for (int j = 0; j < 5; j++)
                        {
                            JObject lItem = new JObject();
                            int charPos = (int)response["prost_mapList"][i]["charPos"];
                            lItem["ht"] = response["prost_mapList"][i]["nodeList"][(j + charPos) % 9]["houseType"];

                            prostLineArray.Add(lItem);
                        }

                        prostLines["l"] = prostLineArray;
                        customEventItem["m" + i] = prostLines;
                    }
                    customEvents.Add(customEventItem);
                    eventItem["customsubevents"] = customEvents;
                }
            }

            if (!object.ReferenceEquals(response["prost_mapActionList"], null) && response["prost_mapActionList"].Count > 0)
            {
                for(int i = 0; i < response["prost_mapActionList"].Count; i++)
                {
                    int mapId           = (int)response["prost_mapActionList"][i]["mapId"];
                    bool isFeature      = (bool)response["prost_mapActionList"][i]["isFeature"];
                    bool isCashPrize    = (bool)response["prost_mapActionList"][i]["isCashPrize"];
                    if (isFeature)
                        eventItem["customsubevents"][0]["m" + mapId]["l"][0]["f"] = true;
                    if(isCashPrize)
                        eventItem["customsubevents"][0]["m" + mapId]["l"][0]["cp"] = true;
                }
            }

            if (!object.ReferenceEquals(response["prost_prostFeature"], null))
            {
                JArray reelslist = buildHabaneroLogReelslist(response);
                eventItem["reelslist"] = reelslist;
            }

            if (!object.ReferenceEquals(response["prost_cashPrizeList"], null) && response["prost_cashPrizeList"].Count > 0)
            {
                JArray subEvents = new JArray();
                if (!object.ReferenceEquals(eventItem["subevents"], null))
                    subEvents = eventItem["subevents"] as JArray;

                for(int i = 0; i < response["prost_cashPrizeList"].Count; i++)
                {
                    JObject subEventItem = new JObject();
                    subEventItem["type"]    = "cashprize";
                    subEventItem["wincash"] = response["prost_cashPrizeList"][i]["winCash"];
                    subEvents.Add(subEventItem);
                }
                eventItem["subevents"] = subEvents;
            }
            return eventItem;
        }

        protected override JArray buildHabaneroLogReels(string strGlobalUserID,int currentIndex ,dynamic response, bool containWild = false)
        {
            JArray logReels = new JArray();
            for (int i = 0; i < response["reels"].Count; i++)
            {
                JArray col = new JArray();
                for (int j = 0; j < response["reels"][i].Count; j++)
                {
                    int symbol = Convert.ToInt32(response["reels"][i][j]["symbolid"]);
                    col.Add(SymbolIdStringForLog[symbol].id);
                }
                logReels.Add(col);
            }
            return logReels;
        }

        protected override JArray buildHabaneroLogReelslist(dynamic response)
        {
            JArray reelslist        = new JArray();
            JObject reelslistItem   = new JObject();
            JArray reelsListCols    = new JArray();

            for (int i = 0; i < response["virtualreels"].Count; i++)
            {
                JArray reelsCol = new JArray();
                for (int j = 2; j < response["virtualreels"][i].Count - 2; j++)
                    reelsCol.Add(SymbolIdStringForLog[(int)response["virtualreels"][i][j]].id);
                reelsListCols.Add(reelsCol);
            }
            reelslistItem["reels"] = reelsListCols;
            reelslist.Add(reelslistItem);
            
            return reelslist;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction = HabaneroActionType.FREEGAME)
        {
            JArray resumeGames  = base.buildInitResumeGame(strGlobalUserID, betInfo, lastResult, gameinstanceid, roundid, currentAction);
            dynamic lastContext = lastResult as dynamic;
            resumeGames[0]["gamemode"]      = lastContext["nextgamestate"];
            resumeGames[0]["numfreegames"]  = lastContext["totalwinfreegames"];
            if(object.ReferenceEquals(lastResult["currentfreegame"],null))
                resumeGames[0]["currfreegame"] = 0;
            return resumeGames;
        }
    }
    public class HouseType
    {
        public int  houseType   { get; set; }
        public long nodeId      { get; set; }

        public HouseType(int _houseType,long _nodeId)
        {
            this.houseType  = _houseType;
            this.nodeId     = _nodeId;
        }
    }
}
