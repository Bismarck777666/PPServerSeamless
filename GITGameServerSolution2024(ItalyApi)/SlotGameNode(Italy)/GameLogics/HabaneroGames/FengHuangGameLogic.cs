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
    public class FengHuangGameLogic : BaseHabaneroSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SGFenghuang";
            }
        }
        protected override string BrandGameId
        {
            get
            {
                return "38ea48d5-b928-46c4-8e61-2cf85af20d62";
            }
        }
        protected override string GameHash
        {
            get
            {
                return "fcf3c315c8579733156a55d2d8377542f5324414";
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
                return 20.0f;
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
                    {0,   new HabaneroLogSymbolIDName{id = "idExpand",    name = "idExpand"   } },    //(봉황 || 룡)프리게임중
                    {1,   new HabaneroLogSymbolIDName{id = "idPhoenix",   name = "Phoenix"    } },    //봉황
                    {2,   new HabaneroLogSymbolIDName{id = "idDragon",    name = "Dragon"     } },    //룡
                    {3,   new HabaneroLogSymbolIDName{id = "idFire",      name = "Fire"       } },    //불
                    {4,   new HabaneroLogSymbolIDName{id = "idEarth",     name = "Earth"      } },    //땅
                    {5,   new HabaneroLogSymbolIDName{id = "idMetal",     name = "Metal"      } },    //철
                    {6,   new HabaneroLogSymbolIDName{id = "idWater",     name = "Water"      } },    //폭포
                    {7,   new HabaneroLogSymbolIDName{id = "idWood",      name = "Wood"       } },    //나무
                    {8,   new HabaneroLogSymbolIDName{id = "idA",         name = "A"          } },    //A
                    {9,   new HabaneroLogSymbolIDName{id = "idK",         name = "K"          } },    //K
                    {10,  new HabaneroLogSymbolIDName{id = "idQ",         name = "Q"          } },    //Q
                    {11,  new HabaneroLogSymbolIDName{id = "idJ",         name = "J"          } },    //J
                    {12,  new HabaneroLogSymbolIDName{id = "id10",        name = "10"         } },    //10
                };
            }
        }
        protected override int InitReelStatusNo
        {
            get
            {
                return 310;
            }
        }
        #endregion

        public FengHuangGameLogic()
        {
            _gameID     = GAMEID.FengHuang;
            GameName    = "FengHuang";
        }

        protected override JArray buildHabaneroLogReels(string strGlobalUserId,int currentIndex ,dynamic response, bool containWild = false)
        {

            dynamic currentResult               = JsonConvert.DeserializeObject<dynamic>(_dicUserHistory[strGlobalUserId].Responses[currentIndex].Response);
            List<List<int>> currentVirtualReels = convertJArrayToListIntArray(currentResult["virtualreels"] as JArray);

            List<ReelIndexAndSymbol> phoenixPositionList = new List<ReelIndexAndSymbol>();
            List<ReelIndexAndSymbol> dragonPositionList = new List<ReelIndexAndSymbol>();
            List<int> expandPositions = new List<int>();

            for (int i = 0; i < currentVirtualReels.Count; i++)
            {
                for (int j = 2; j < currentVirtualReels[i].Count - 2; j++)
                {
                    if (currentVirtualReels[i][j] == 0)
                    {
                        if (checkOriginIsDragonSymbol(strGlobalUserId, i, j, 1, _dicUserHistory[strGlobalUserId].Responses.Count - currentIndex))
                            dragonPositionList.Add(new ReelIndexAndSymbol() { reelindex = i, symbolindex = j });

                        if (checkOriginIsPhoenixSymbol(strGlobalUserId, i, j, 1, _dicUserHistory[strGlobalUserId].Responses.Count - currentIndex))
                            phoenixPositionList.Add(new ReelIndexAndSymbol() { reelindex = i, symbolindex = j });
                    }
                }
            }

            for (int i = 0; i < dragonPositionList.Count; i++)
            {
                int reelIndex = dragonPositionList[i].reelindex;
                int symbolIndex = dragonPositionList[i].symbolindex;
                int acrossedIndex = phoenixPositionList.FindIndex(_ => ((_.reelindex == reelIndex) || (_.reelindex - 1) == reelIndex) && _.symbolindex == symbolIndex);

                if (acrossedIndex != -1 && !expandPositions.Contains(reelIndex))
                    expandPositions.Add(reelIndex);
            }

            JArray logReels = new JArray();
            for (int i = 0; i < response["reels"].Count; i++)
            {
                JArray col = new JArray();
                for (int j = 0; j < response["reels"][i].Count; j++)
                {
                    int symbol   = Convert.ToInt32(response["reels"][i][j]["symbolid"]);
                    string symbolid = "";
                    if (symbol > 0)//봉황 + 룡 이 아니면
                    {
                        symbolid = SymbolIdStringForLog[symbol].id;
                    }
                    else
                    {
                        int wrapedExpandIndex = expandPositions.FindIndex(_ => _ == i || _ + 1 == i);
                        
                        if(wrapedExpandIndex == -1)
                        {
                            if (dragonPositionList.Any(_ => _.reelindex == i && _.symbolindex == j + 2))
                                symbolid = SymbolIdStringForLog[2].id;
                            else if(phoenixPositionList.Any(_ => _.reelindex == i && _.symbolindex == j + 2))
                                symbolid = SymbolIdStringForLog[1].id;
                        }
                        else if (expandPositions[wrapedExpandIndex] == i)
                            symbolid = string.Format("idWild_1_{0}", j + 1);
                        else
                            symbolid = string.Format("idWild_2_{0}", j + 1);
                    }
                    col.Add(symbolid);
                }
                logReels.Add(col);
            }
            return logReels;
        }

        protected override JArray buildInitResumeGame(string strGlobalUserId, BaseHabaneroSlotBetInfo betInfo, JObject lastResult, string gameinstanceid, string roundid,HabaneroActionType currentAction)
        {
            JArray resumeGames                  = base.buildInitResumeGame(strGlobalUserId, betInfo, lastResult, gameinstanceid, roundid);
            List<List<int>> lastVirtualReels    = convertJArrayToListIntArray(lastResult["virtualreels"] as JArray);
            
            List<ReelIndexAndSymbol> phoenixPositionList    = new List<ReelIndexAndSymbol>();
            List<ReelIndexAndSymbol> dragonPositionList     = new List<ReelIndexAndSymbol>();
            List<int> expandPositions                       = new List<int>();

            for (int i = 0; i < lastVirtualReels.Count; i++)
            {
                for(int j = 2; j < lastVirtualReels[i].Count - 2; j++)
                {
                    if(lastVirtualReels[i][j] == 0)
                    {
                        if (checkOriginIsDragonSymbol(strGlobalUserId, i, j, 1, 1))
                            dragonPositionList.Add(new ReelIndexAndSymbol() { reelindex = i, symbolindex = j });
                        
                        if (checkOriginIsPhoenixSymbol(strGlobalUserId, i, j, 1, 1))
                            phoenixPositionList.Add(new ReelIndexAndSymbol() { reelindex = i, symbolindex = j });
                    }
                }
            }

            for(int i = 0; i < dragonPositionList.Count; i++)
            {
                int reelIndex   = dragonPositionList[i].reelindex;
                int symbolIndex = dragonPositionList[i].symbolindex;
                int acrossedIndex = phoenixPositionList.FindIndex(_ => ((_.reelindex == reelIndex) || (_.reelindex - 1) == reelIndex) && _.symbolindex == symbolIndex);

                if (acrossedIndex != -1 && !expandPositions.Contains(reelIndex))
                    expandPositions.Add(reelIndex);
            }

            JObject Fenghuang_fgWildsState  = new JObject();
            JArray expandList               = new JArray();
            JArray phoenixPositions         = new JArray();
            JArray dragonPositions          = new JArray();
            
            for(int i = 0; i < expandPositions.Count; i++)
                expandList.Add(expandPositions[i]);

            for(int i = 0; i < phoenixPositionList.Count; i++)
            {
                JObject phoenixItem = new JObject();
                phoenixItem["reelindex"]    = phoenixPositionList[i].reelindex;
                phoenixItem["symbolindex"]  = phoenixPositionList[i].symbolindex - 2;
                phoenixPositions.Add(phoenixItem);
            }

            for(int i = 0; i < dragonPositionList.Count; i++)
            {
                JObject dragonItem = new JObject();
                dragonItem["reelindex"]     = dragonPositionList[i].reelindex;
                dragonItem["symbolindex"]   = dragonPositionList[i].symbolindex - 2;
                dragonPositions.Add(dragonItem);
            }

            Fenghuang_fgWildsState["phoenixPositions"]  = phoenixPositions;
            Fenghuang_fgWildsState["dragonPositions"]   = dragonPositions;
            Fenghuang_fgWildsState["expandList"]        = expandList;
            resumeGames[0]["Fenghuang_fgWildsState"]    = Fenghuang_fgWildsState;
            return resumeGames;
        }

        //0번심벌이 드래곤 인가를확인 (depth = 1부터 시작)
        private bool checkOriginIsDragonSymbol(string strGlobalUserId, int col,int row,int depth,int backwardIndex)
        {
            if (col >= 4)//맨오른쪽릴이면
                return false;

            List<HabaneroHistoryResponses> habaneroHistories = _dicUserHistory[strGlobalUserId].Responses;
            if (habaneroHistories.Count < (backwardIndex + depth))
                return false;

            dynamic currentDepthResult                  = JsonConvert.DeserializeObject<dynamic>(habaneroHistories[habaneroHistories.Count - backwardIndex - depth].Response);
            List<List<int>> currentDepthVirtualReels    = convertJArrayToListIntArray(currentDepthResult["virtualreels"] as JArray);
            
            if (currentDepthVirtualReels[col + 1][row] == 2)
                return true;
            
            if (currentDepthVirtualReels[col + 1][row] == 0)
                return checkOriginIsDragonSymbol(strGlobalUserId, col + 1, row, depth + 1, backwardIndex);
            
            return false;
        }

        //0번심벌이 포에닉스 인가를확인 (depth = 1부터 시작)
        private bool checkOriginIsPhoenixSymbol(string strGlobalUserId, int col,int row,int depth,int backwardIndex)
        {
            if (col <= 0)//맨왼쪽릴이면
                return false;

            List<HabaneroHistoryResponses> habaneroHistories = _dicUserHistory[strGlobalUserId].Responses;
            if (habaneroHistories.Count < (backwardIndex + depth))
                return false;

            dynamic currentDepthResult                  = JsonConvert.DeserializeObject<dynamic>(habaneroHistories[habaneroHistories.Count - backwardIndex - depth].Response);
            List<List<int>> currentDepthVirtualReels    = convertJArrayToListIntArray(currentDepthResult["virtualreels"] as JArray);
            
            if (currentDepthVirtualReels[col - 1][row] == 1)
                return true;
            
            if (currentDepthVirtualReels[col - 1][row] == 0)
                return checkOriginIsPhoenixSymbol(strGlobalUserId, col - 1, row, depth + 1, backwardIndex);
            
            return false;
        }
    }
    public class ReelIndexAndSymbol
    {
        public int reelindex    { get; set; }
        public int symbolindex  { get; set; }
    }
}
