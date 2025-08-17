using Akka.Actor;
using GITProtocol;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCGSharp;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    public class LaughingBuddhaGameLogic : BaseSelFreeHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGLaughingBuddha";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "d1f6e4ba-3dfb-4374-906f-103b46aa74cd";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "6701f20f22fea5baf57b84917600f1e79715a518";
            }
        }
        protected override string GameVersion
        {
            get
            {
                return "5.1.10227.416";
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
        protected override int FreeSpinTypeCount
        {
            get { return 9; }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 663;
            }
        }
        protected override int[] PossibleFreeSpinTypes(int freeSpinGroup)
        {
            return new int[] { 205, 206, 207, 208, 209, 210, 211, 212, 213 };
        }
        protected override Dictionary<int, HabaneroLogSymbolIDName> SymbolIdStringForLog
        {
            get
            {
                return new Dictionary<int, HabaneroLogSymbolIDName>()
                {
                    {1,   new HabaneroLogSymbolIDName{id = "idWildX1",          name = "WildX1"         } },    //와일드1
                    {2,   new HabaneroLogSymbolIDName{id = "idWildX2",          name = "WildX2"         } },    //와일드X2
                    {3,   new HabaneroLogSymbolIDName{id = "idWildX3",          name = "WildX3"         } },    //와일드X3
                    {4,   new HabaneroLogSymbolIDName{id = "idScatter",         name = "Scatter"        } },    //스캐터
                    {5,   new HabaneroLogSymbolIDName{id = "idLaughingBuddha",  name = "LaughingBuddha" } },    //부처님
                    {6,   new HabaneroLogSymbolIDName{id = "idGoldenDragon",    name = "GoldenDragon"   } },    //나무잎
                    {7,   new HabaneroLogSymbolIDName{id = "idGoldIngot",       name = "GoldIngot"      } },    //금괴
                    {8,   new HabaneroLogSymbolIDName{id = "idJadeRuyi",        name = "JadeRuyi"       } },    //옥패
                    {9,   new HabaneroLogSymbolIDName{id = "idA",               name = "A"              } },    //A
                    {10,  new HabaneroLogSymbolIDName{id = "idK",               name = "K"              } },    //K
                    {11,  new HabaneroLogSymbolIDName{id = "idQ",               name = "Q"              } },    //J
                    {12,  new HabaneroLogSymbolIDName{id = "idJ",               name = "J"              } },    //J
                    {13,  new HabaneroLogSymbolIDName{id = "id10",              name = "10"             } },    //10
                };
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double[] PurchaseFreeMultiple
        {
            get { return new double[] { 1300 / 28.0 }; }
        }
        #endregion

        public LaughingBuddhaGameLogic()
        {
            _gameID     = GAMEID.LaughingBuddha;
            GameName    = "LaughingBuddha";
        }

        protected override async Task<bool> loadSpinData()
        {
            try
            {
                _spinDatabase = Context.ActorOf(Akka.Actor.Props.Create(() => new SpinDatabase(_providerName, this.GameName)), "spinDatabase");
                bool isSuccess = await _spinDatabase.Ask<bool>("initialize", TimeSpan.FromSeconds(5.0));
                if (!isSuccess)
                {
                    _logger.Error("couldn't load spin data of game {0}", this.GameName);
                    return false;
                }

                ReadSpinInfoResponse response = await _spinDatabase.Ask<ReadSpinInfoResponse>(new ReadSpinInfoRequest(SpinDataTypes.SelFreeSpin), TimeSpan.FromSeconds(30.0));
                if (response == null)
                {
                    _logger.Error("couldn't load spin odds information of game {0}", this.GameName);
                    return false;
                }

                _naturalSpinOddProbs            = new SortedDictionary<double, int>();
                _naturalChildFreeSpinOddProbs   = new SortedDictionary<double, int>[FreeSpinTypeCount];
                _naturalChildFreeSpinCounts     = new int[FreeSpinTypeCount];
                _totalChildFreeSpinCounts       = new int[FreeSpinTypeCount];
                _totalChildFreeSpinIDs          = new SortedDictionary<double, int[]>[FreeSpinTypeCount];

                _spinDataDefaultBet             = response.DefaultBet;
                _naturalSpinCount               = 0;
                _emptySpinIDs                   = new List<int>();

                Dictionary<double, List<int>>   totalSpinOddIds         = new Dictionary<double, List<int>>();
                Dictionary<double, List<int>>   freeSpinOddIds          = new Dictionary<double, List<int>>();
                Dictionary<double, List<int>>[] totalChildSpinOddIDs    = new Dictionary<double, List<int>>[FreeSpinTypeCount];

                double freeSpinTotalOdd         = 0.0;
                double minFreeSpinTotalOdd      = 0.0;
                int    freeSpinTotalCount       = 0;
                int    minFreeSpinTotalCount    = 0;

                _minStartFreeSpinIDs = new List<int>();
                for (int i = 0; i < response.SpinBaseDatas.Count; i++)
                {
                    SpinBaseData spinBaseData = response.SpinBaseDatas[i];
                    if (spinBaseData.ID <= response.NormalMaxID)
                    {
                        if (spinBaseData.SpinType <= 100)
                        {
                            _naturalSpinCount++;
                            if (_naturalSpinOddProbs.ContainsKey(spinBaseData.Odd))
                                _naturalSpinOddProbs[spinBaseData.Odd]++;
                            else
                                _naturalSpinOddProbs[spinBaseData.Odd] = 1;
                        }
                        else if (spinBaseData.SpinType >= 200)
                        {
                            int freeSpinType = spinBaseData.SpinType - 200;
                            if (_naturalChildFreeSpinOddProbs[freeSpinType - 5] == null)
                                _naturalChildFreeSpinOddProbs[freeSpinType - 5] = new SortedDictionary<double, int>();

                            if (_naturalChildFreeSpinOddProbs[freeSpinType - 5].ContainsKey(spinBaseData.Odd))
                                _naturalChildFreeSpinOddProbs[freeSpinType - 5][spinBaseData.Odd]++;
                            else
                                _naturalChildFreeSpinOddProbs[freeSpinType - 5][spinBaseData.Odd] = 1;
                        }
                    }

                    if (spinBaseData.SpinType == 0 && spinBaseData.Odd == 0.0)
                        _emptySpinIDs.Add(spinBaseData.ID);

                    if (spinBaseData.SpinType <= 100)
                    {
                        if (!totalSpinOddIds.ContainsKey(spinBaseData.Odd))
                            totalSpinOddIds.Add(spinBaseData.Odd, new List<int>());

                        totalSpinOddIds[spinBaseData.Odd].Add(spinBaseData.ID);
                    }
                    else
                    {
                        int freeSpinType = spinBaseData.SpinType - 200;
                        if (totalChildSpinOddIDs[freeSpinType - 5] == null)
                            totalChildSpinOddIDs[freeSpinType - 5] = new Dictionary<double, List<int>>();

                        if (!totalChildSpinOddIDs[freeSpinType - 5].ContainsKey(spinBaseData.Odd))
                            totalChildSpinOddIDs[freeSpinType - 5][spinBaseData.Odd] = new List<int>();

                        totalChildSpinOddIDs[freeSpinType - 5][spinBaseData.Odd].Add(spinBaseData.ID);

                        if (spinBaseData.ID <= response.NormalMaxID)
                            _naturalChildFreeSpinCounts[freeSpinType - 5]++;

                        _totalChildFreeSpinCounts[freeSpinType - 5]++;
                    }

                    if (SupportPurchaseFree && spinBaseData.SpinType == 100)
                    {
                        freeSpinTotalCount++;
                        freeSpinTotalOdd += spinBaseData.Odd;
                        if (!freeSpinOddIds.ContainsKey(spinBaseData.Odd))
                            freeSpinOddIds.Add(spinBaseData.Odd, new List<int>());
                        freeSpinOddIds[spinBaseData.Odd].Add(spinBaseData.ID);

                        if (spinBaseData is StartSpinBaseData)
                        {
                            minFreeSpinTotalCount++;
                            minFreeSpinTotalOdd += (spinBaseData as StartSpinBaseData).MinRate;
                            _minStartFreeSpinIDs.Add(spinBaseData.ID);
                        }
                    }
                }
                _totalSpinOddIds = new SortedDictionary<double, int[]>();
                foreach (KeyValuePair<double, List<int>> pair in totalSpinOddIds)
                    _totalSpinOddIds.Add(pair.Key, pair.Value.ToArray());

                for (int i = 0; i < FreeSpinTypeCount; i++)
                {
                    if (totalChildSpinOddIDs[i] == null)
                        continue;

                    _totalChildFreeSpinIDs[i] = new SortedDictionary<double, int[]>();
                    foreach (KeyValuePair<double, List<int>> pair in totalChildSpinOddIDs[i])
                        _totalChildFreeSpinIDs[i].Add(pair.Key, pair.Value.ToArray());
                }

                if (SupportPurchaseFree)
                {
                    _totalFreeSpinOddIds = new SortedDictionary<double, int[]>();
                    foreach (KeyValuePair<double, List<int>> pair in freeSpinOddIds)
                        _totalFreeSpinOddIds.Add(pair.Key, pair.Value.ToArray());

                    _freeSpinTotalCount     = freeSpinTotalCount;
                    _minFreeSpinTotalCount  = minFreeSpinTotalCount;
                    _totalFreeSpinWinRate   = freeSpinTotalOdd / freeSpinTotalCount;
                    _minFreeSpinWinRate     = minFreeSpinTotalOdd / minFreeSpinTotalCount;

                    if (_totalFreeSpinWinRate <= _minFreeSpinWinRate || _minFreeSpinTotalCount == 0)
                        _logger.Error("min freespin rate doesn't satisfy condition {0}", this.GameName);
                }

                double winRate = 0.0;
                foreach (KeyValuePair<double, int> pair in _naturalSpinOddProbs)
                    winRate += (pair.Key * pair.Value / _naturalSpinCount);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
                return false;
            }
        }

        protected override async Task<BasePPSlotSpinData> selectMinStartFreeSpinData(BaseHabaneroSlotBetInfo betInfo)
        {
            try
            {
                int                 spinDataID  = _minStartFreeSpinIDs[Pcg.Default.Next(0, _minFreeSpinTotalCount)];
                BasePPSlotSpinData  spinData    =  await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(spinDataID), TimeSpan.FromSeconds(10.0));

                if (!(spinData is BasePPSlotStartSpinData))
                    return null;

                BasePPSlotStartSpinData minStartSpinData = spinData as BasePPSlotStartSpinData;

                double minFreeOdd = 0.2 * PurchaseFreeMultiple[betInfo.PurchaseFree - 1] - minStartSpinData.StartOdd;
                //double minFreeOdd = 0.1 * PurchaseFreeMultiple - minStartSpinData.StartOdd;
                if (minFreeOdd < 0.0)
                    minFreeOdd = 0.0;

                double maxFreeOdd = 0.5 * PurchaseFreeMultiple[betInfo.PurchaseFree - 1] - minStartSpinData.StartOdd;
                if (maxFreeOdd < 0.0)
                    maxFreeOdd = 0.0;

                minStartSpinData.FreeSpins    = new List<OddAndIDData>();
                int[]   freeSpinTypes         = PossibleFreeSpinTypes(minStartSpinData.FreeSpinGroup);
                double  maxOdd                = 0.0;

                string startFreeSpinString = minStartSpinData.SpinStrings[0];
                dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(startFreeSpinString);
                List<int> pickableIds = new List<int>();
                
                for (int i = 0; i < resultContext["pickableIds"].Count; i++)
                    pickableIds.Add((int)resultContext["pickableIds"][i]);
                
                for (int i = 0; i < pickableIds.Count; i++)
                {
                    int freeSpinType = pickableIds[i];
                    OddAndIDData childFreeSpin  = selectOddAndIDFromProbsWithRange(_totalChildFreeSpinIDs[freeSpinType - 5], minFreeOdd, maxFreeOdd);

                    if (childFreeSpin.Odd > maxOdd)
                        maxOdd = childFreeSpin.Odd;
                    minStartSpinData.FreeSpins.Add(childFreeSpin);
                }
                minStartSpinData.MaxOdd = minStartSpinData.StartOdd + maxOdd;
                return minStartSpinData;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in LaughingBuddhaGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }

        protected override void buildStartFreeSpinData(BasePPSlotStartSpinData startSpinData, StartSpinBuildTypes buildType, double minOdd, double maxOdd)
        {
            string startFreeSpinString = startSpinData.SpinStrings[0];

            dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(startFreeSpinString);

            List<int> pickableIds = new List<int>();
            for(int i = 0; i < resultContext["pickableIds"].Count; i++)
                pickableIds.Add((int)resultContext["pickableIds"][i]);

            startSpinData.FreeSpins = new List<OddAndIDData>();
            int[] freeSpinTypes = PossibleFreeSpinTypes(startSpinData.FreeSpinGroup);
            double maxFreeOdd = 0.0;
            for (int i = 0; i < pickableIds.Count; i++)
            {
                int freeSpinType = pickableIds[i];
                OddAndIDData childFreeSpin = null;
                if (buildType == StartSpinBuildTypes.IsNaturalRandom)
                {
                    double odd  = selectOddFromProbs(_naturalChildFreeSpinOddProbs[freeSpinType - 5], _naturalChildFreeSpinCounts[freeSpinType - 5]);
                    int id      = _totalChildFreeSpinIDs[freeSpinType - 5][odd][Pcg.Default.Next(0, _totalChildFreeSpinIDs[freeSpinType - 5][odd].Length)];
                    childFreeSpin = new OddAndIDData(id, odd);
                }
                else if (buildType == StartSpinBuildTypes.IsTotalRandom)
                {
                    childFreeSpin = selectOddAndIDFromProbs(_totalChildFreeSpinIDs[freeSpinType - 5], _totalChildFreeSpinCounts[freeSpinType - 5]);
                }
                else
                {
                    childFreeSpin = selectOddAndIDFromProbsWithRange(_totalChildFreeSpinIDs[freeSpinType - 5], minOdd, maxOdd);
                }

                if (childFreeSpin.Odd > maxFreeOdd)
                    maxFreeOdd = childFreeSpin.Odd;

                startSpinData.FreeSpins.Add(childFreeSpin);
            }
            startSpinData.MaxOdd = startSpinData.StartOdd + maxFreeOdd;
        }

        protected override JObject buildEventItem(string strGlobalUserID, int currentIndex)
        {
            JObject eventItem = base.buildEventItem(strGlobalUserID, currentIndex);
            HabaneroHistoryResponses responses = _dicUserHistory[strGlobalUserID].Responses[currentIndex];
            dynamic response        = JsonConvert.DeserializeObject<dynamic>(responses.Response);
                
            if(currentIndex == 1)
            {
                JArray customSubEvents = new JArray();
                JObject customSubEventItem = new JObject();
                customSubEventItem["type"]  = "LaughingBuddhaPick";
                customSubEventItem["pId"]   = (int)response["pickId"];
                customSubEvents.Add(customSubEventItem);
                eventItem["customsubevents"] = customSubEvents;
            }
            else
            {
                JArray reels            = buildHabaneroLogReels(strGlobalUserID, currentIndex, response as JObject);
                JArray reelsList        = new JArray();

                if (!object.ReferenceEquals(response["copyList"], null))
                {
                    JObject reelsListItem   = new JObject();
                    reelsListItem["reels"]  = buildHabaneroLogReelslist(response);
                    reelsList.Add(reelsListItem);
                    eventItem["reelslist"]  = reelsList;
                }
                eventItem["reels"]      = reels;
                eventItem["multiplier"] = 1;

                JArray subEvents = new JArray();

                if (subEvents.Count > 0)
                    eventItem["subevents"] = subEvents;
            }
            return eventItem;
        }

        protected override JArray buildHabaneroLogReels(string strGlobalUserID,int currentIndex ,dynamic response, bool containWild = false)
        {
            JArray reels = base.buildHabaneroLogReels(strGlobalUserID, currentIndex, response as JObject, containWild);
            if (object.ReferenceEquals(response["copyList"], null))
                return reels;
            string freePickString   = _dicUserHistory[strGlobalUserID].Responses[1].Response;
            dynamic resultContext   = JsonConvert.DeserializeObject<dynamic>(freePickString);
            int pickedSymbol        = (int)resultContext["pickId"];
            
            for(int i = 0; i < response["copyList"].Count; i++)
            {
                int col = (int)response["copyList"][i];
                for (int j = 0; j < 3; j++)
                {
                    if ((int)response["virtualreels"][col][j + 2] > 4)//와일드,스캐터가 아니면 바꾸기
                        reels[col][j] = SymbolIdStringForLog[pickedSymbol].id;
                }
            }
            return reels;
        }

        protected override JArray buildHabaneroLogReelslist(dynamic response)
        {
            JArray reels = new JArray();
            for (int j = 0; j < response["virtualreels"].Count; j++)
            {
                JArray col = new JArray();
                for (int k = 2; k < response["virtualreels"][j].Count - 2; k++)
                {
                    int symbol = Convert.ToInt32(response["virtualreels"][j][k]);
                    string symbolid = SymbolIdStringForLog[symbol].id;
                    col.Add(symbolid);
                }
                reels.Add(col);
            }
            return reels;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserID, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid, HabaneroActionType currentAction)
        {
            dynamic lastContext = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserID].Responses.Last().Response);
            HabaneroActionType nextAction = convertStringToAction((string)lastContext["nextgamestate"]);
            JArray resumeGames = base.buildInitResumeGame(strGlobalUserID, betInfo, lastResult, gameinstanceid, roundid, nextAction);
            if (nextAction == HabaneroActionType.SYMBOLPICK)
            {
                resumeGames[0]["gamemode"]      = convertActionToString(nextAction);
                resumeGames[0]["numfreegames"]  = 8;
                resumeGames[0]["currfreegame"]  = 0;
            }

            dynamic freeStartSpin           = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserID].Responses.First().Response);
            resumeGames[0]["pickableIds"]   = freeStartSpin["pickableIds"];
            resumeGames[0]["pickId"]        = 0;
            if (nextAction == HabaneroActionType.FREEGAME)
            {
                dynamic pickSpin = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserID].Responses[1].Response);
                resumeGames[0]["pickId"] = pickSpin["pickId"];
            }
            return resumeGames;
        }
    }
}
