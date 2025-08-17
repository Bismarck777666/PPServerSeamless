using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using Akka.Configuration;
using GITProtocol.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCGSharp;
namespace SlotGamesNode.GameLogics
{
    public class BaseCQ9RespinSlotGame : BaseCQ9SlotGame
    {
        protected virtual string[][] BaseReelSet
        {
            get
            {
                return new string[][]
                {
                    new string[]{ "11", "12", "1", "13", "14", "5", "15", "16", "4", "11", "15", "3", "12", "15", "2", "13", "14", "5", "11", "14", "4", "15", "16", "2", "13", "12", "5", "11", "14", "4", "13", "5", "12", "2", "16", "3", "15", "5", "13", "4", "16", "12", "F", "11", "12", "5", "14", "16", "1" },
                    new string[]{ "13", "1", "14", "2", "13", "14", "3", "11", "12", "4", "15", "5", "16", "11", "W", "12", "15", "2", "14", "5", "16", "12", "4", "11", "15", "3", "14", "16", "F", "13", "1", "14", "5", "13", "14", "3", "11", "12", "4", "15", "5", "16", "11", "W", "12", "13", "2", "14", "5", "13", "12", "4", "11", "15", "3", "14", "16"},
                    new string[]{ "12", "11", "3", "14", "15", "5", "16", "14", "5", "12", "16", "F", "14", "15", "5", "13", "11", "4", "12", "16", "3", "14", "13", "4", "11", "15", "5", "13", "12", "4", "14", "1", "15", "2", "16", "4", "13", "5", "11", "4", "12", "14", "F", "13", "14"},
                    new string[]{ "11", "1", "13", "4", "16", "F", "15", "1", "16", "14", "3", "13", "2", "15", "W", "11", "12", "5", "16", "2", "15", "14", "F", "13", "3", "12", "5", "11", "F", "11", "3", "13", "4", "12", "2", "15", "1", "16", "14", "3", "13", "4", "12", "W", "11", "12", "5", "16", "2", "15", "14", "4", "13", "3", "12", "5", "11"},
                    new string[]{ "13", "14", "2", "15", "11", "5", "14", "12", "4", "13", "14", "3", "15", "16", "F", "13", "11", "1", "16", "15", "3", "16", "12", "2", "13", "15", "5", "11", "14", "4", "15", "1", "12", "2", "16", "3", "13", "5", "14", "4", "15", "F", "13", "5", "14", "1", "12", "3", "15", "4", "16", "11", "1", "13", "14", "2" }
                };
            }
        }

        protected virtual Dictionary<string, int[]> PayTable
        {
            get
            {
                return new Dictionary<string, int[]>()
                {
                    { "1",  new int[]{ 0,0, 50, 100,1000} },
                    { "2",  new int[]{ 0,0, 35, 100,800} },
                    { "3",  new int[]{ 0,0, 30, 100,800} },
                    { "4",  new int[]{ 0,0, 20, 50, 300} },
                    { "5",  new int[]{ 0,0, 15, 35, 300} },
                    { "11", new int[]{ 0,0, 10, 30, 200} },
                    { "12", new int[]{ 0,0, 10, 20, 200} },
                    { "13", new int[]{ 0,0, 10, 15, 100} },
                    { "14", new int[]{ 0,0, 10, 15, 100} },
                    { "15", new int[]{ 0,0, 5,  15, 100} },
                    { "16", new int[]{ 0,0, 5,  10, 100} },
                };
            }
        }

        protected override async Task spinGame(string strUserID, int agentID, UserBonus userBonus, bool isMustLose, double userBalance)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
                //해당 유저의 베팅정보를 얻는다. 만일 베팅정보가 없다면(례외상황) 그대로 리턴한다.
                BaseCQ9SlotBetInfo betInfo = null;
                if (!_dicUserBetInfos.TryGetValue(strGlobalUserID, out betInfo))
                    return;

                byte[] betInfoBytes = backupBetInfo(betInfo);

                BaseCQ9SlotSpinResult lastResult = null;
                if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                    lastResult = _dicUserResultInfos[strGlobalUserID];

                double betMoney = betInfo.TotalBet;

                if (betInfo.ReelPay > 0)
                    betMoney = betInfo.ReelPay;
                if (betInfo.HasRemainResponse)
                    betMoney = 0.0;

                //만일 베팅머니가 유저의 밸런스보다 크다면 끝낸다.(2020.02.15)
                if (userBalance.LT(betMoney, _epsilion) || betMoney < 0.0)
                {
                    _logger.Error("user balance is less than bet money in BaseCQ9ReelPaySlotGame::spinGame {0} balance:{1}, bet money: {2} game id:{3}", strGlobalUserID, userBalance, betMoney, _gameID);
                    return;
                }

                if (betMoney > 0.0)
                    _dicUserHistory[strGlobalUserID] = new CQ9HistoryItem();

                //결과를 생성한다.
                BaseCQ9SlotSpinResult spinResult = await this.generateSpinResult(betInfo, strUserID, agentID, userBonus, true, isMustLose);

                //게임로그
                string strGameLog = spinResult.ResultString;
                _dicUserResultInfos[strGlobalUserID] = spinResult;

                //생성된 게임결과를 유저에게 보낸다.(윈은 Collect요청시에 처리한다.)
                sendGameResult(betInfo, spinResult, strGlobalUserID, betMoney, 0.0, strGameLog, userBalance);

                _dicUserLastBackupBetInfos[strGlobalUserID]       = betInfoBytes;
                _dicUserLastBackupResultInfos[strGlobalUserID]    = spinResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseCQ9ReelPaySlotGame::spinGame {0}", ex);
            }
        }

        protected override async Task<BaseCQ9SlotSpinResult> generateSpinResult(BaseCQ9SlotBetInfo betInfo, string strUserID, int agentID, UserBonus userBonus, bool usePayLimit, bool isMustLose)
        {
            string strGlobalUserID = string.Format("{0}_{1}", agentID, strUserID);
            //프리스핀이거나 일반스핀에서 릴이 선택되지 않았으면 일반방식으로 처리
            if(betInfo.HasRemainResponse || !betInfo.ReelSelected.Contains(1) || betInfo.ReelPay == 0 || _dicUserResultInfos[strGlobalUserID] == null)
                return await base.generateSpinResult(betInfo, strUserID, agentID, userBonus, usePayLimit, isMustLose);

            BaseCQ9SlotSpinResult   result      = null;

            //유저의 총 베팅액을 얻는다.
            float totalBet      = (float)betInfo.TotalBet;
            double realBetMoney = betInfo.ReelPay;

            int respinColNo = betInfo.ReelSelected.FindIndex(_ => _ == 1);
            SortedDictionary<double, List<BaseCQ9SlotSpinResult>> dicAllAvResults = getAllAvailableSpinResults(respinColNo, _dicUserResultInfos[strGlobalUserID], betInfo);

            double randomOdd    = 0.0;
            List<int> cntList   = new List<int>();
            foreach(KeyValuePair<double, List<BaseCQ9SlotSpinResult>> pair in dicAllAvResults)
            {
                cntList.Add(pair.Value.Count);
            }
            int[] probs = cntList.ToArray();
            int randomKeyIndex      = GameUtils.selectFromProbs(probs);

            List<BaseCQ9SlotSpinResult> rndOddResultList = dicAllAvResults[dicAllAvResults.Keys.ElementAt(randomKeyIndex)];
            int randomReulstIndex   = Pcg.Default.Next(0, rndOddResultList.Count);
            randomOdd               = dicAllAvResults.Keys.ElementAt(randomKeyIndex);
            result                  = rndOddResultList[randomReulstIndex];
            
            double totalWin = totalBet * randomOdd;
            
            if (checkCompanyPayoutRate(agentID, realBetMoney, totalWin))
                return result;

            List<BaseCQ9SlotSpinResult> minOddResultList = dicAllAvResults[dicAllAvResults.Keys.First()];
            randomKeyIndex = Pcg.Default.Next(0, minOddResultList.Count);

            double minOdd = 0.0f;
            minOdd              = dicAllAvResults.Keys.First();
            int minReulstIndex  = Pcg.Default.Next(0, minOddResultList.Count);
            result              = minOddResultList[minReulstIndex];
            double minWin       = totalBet * minOdd;
            sumUpCompanyBetWin(agentID, realBetMoney, minWin);
            return result;
        }
        
        protected virtual SortedDictionary<double,List<BaseCQ9SlotSpinResult>> getAllAvailableSpinResults(int respinColNo, BaseCQ9SlotSpinResult lastResult,BaseCQ9SlotBetInfo betInfo)
        {
            return new SortedDictionary<double, List<BaseCQ9SlotSpinResult>>();
        }
        
        protected virtual int findAllScatterCnt(string[][] reelStatus)
        {
            int scatterCnt = 0;
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 5; k++)
                {
                    if (reelStatus[j][k] == "F")
                        scatterCnt++;
                }
            }
            return scatterCnt;
        }
        
        protected virtual bool checkScatterInColumn(string[][] reelStatus,int colNo)
        {
            bool contain = false;
            for(int i = 0; i < 3; i++)
            {
                if(reelStatus[i][colNo] == "F")
                {
                    contain = true;
                    break;
                }
            }
            return contain;
        }
        
        protected virtual int AddScatterAvalableMoney(int reelNo,BaseCQ9SlotBetInfo betInfo)
        {
            return 0;
        }
        
        protected virtual int findSymbolCntInReelSet(string symbol,int reelNo)
        {
            int cnt = 0;
            for(int i = 0; i < BaseReelSet[reelNo].Length; i++)
            {
                if (BaseReelSet[reelNo][i] == symbol)
                    cnt++;
            }
            return cnt;
        }
        
        protected virtual int calcColumnReelPay(string[][] reelStatus,int calcColNo,BaseCQ9SlotBetInfo betInfo,int[] wildMultiple)
        {
            return 1;
        }
        
        protected virtual Dictionary<string,int> findSameSymbolAndLenth(string[][] reelStatus,int exceptColNo)
        {
            Dictionary<string, int> sameSymbolDic   = new Dictionary<string, int>();
            Dictionary<string, int> bufSymbolDic    = null;

            List<string> startSymbols = new List<string>();
            int startCol = exceptColNo == 0 ? 1 : 0;
            for (int i = 0; i < 5; i++)
            {
                if (i == exceptColNo)
                    continue;
                for (int row = 0; row < 3; row++)
                {
                    if (reelStatus[row][i] == "F")
                        continue;
                    if (reelStatus[row][i] == "W")
                    {
                        startSymbols = AddAllSymbolToStart();
                        break;
                    }

                    if (!startSymbols.Contains(reelStatus[row][i]))
                        startSymbols.Add(reelStatus[row][i]);
                }
                break;
            }
            int setLength = 2;

            try
            {
                for (int col = 0; col < 5; col++)
                {
                    if (col == startCol || col == exceptColNo)
                        continue;
                    if (setLength == 2)
                    {
                        for (int row = 0; row < 3; row++)
                        {
                            if (!startSymbols.Contains(reelStatus[row][col]) && reelStatus[row][col] != "W")
                                continue;
                            if (reelStatus[row][col] == "W")
                            {
                                foreach (string startSymbol in startSymbols)
                                {
                                    if (!sameSymbolDic.ContainsKey(startSymbol))
                                        sameSymbolDic.Add(startSymbol, setLength);
                                }
                                break;
                            }
                            sameSymbolDic.Add(reelStatus[row][col], setLength);
                        }
                        if (sameSymbolDic.Count == 0)
                            break;
                        setLength++;
                        continue;
                    }

                    bufSymbolDic = sameSymbolDic;
                    for (int row = 0; row < 3; row++)
                    {
                        if (!sameSymbolDic.ContainsKey(reelStatus[row][col]) && reelStatus[row][col] != "W")
                            continue;
                        if (reelStatus[row][col] == "W")
                        {
                            foreach (KeyValuePair<string, int> pair in sameSymbolDic.ToArray())
                            {
                                if (sameSymbolDic[pair.Key] < (setLength - 1))
                                    continue;
                                bufSymbolDic[pair.Key] = setLength;
                            }
                            break;
                        }
                        if (sameSymbolDic[reelStatus[row][col]] < (setLength - 1))
                            continue;
                        bufSymbolDic[reelStatus[row][col]] = setLength;
                    }
                    setLength++;
                    sameSymbolDic = bufSymbolDic;
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
            return sameSymbolDic;
        }
        
        protected virtual List<string> AddAllSymbolToStart()
        {
            return new List<string>(){};
        }
        
        protected virtual int CalcRespinOfSymbol(string symbol,int multiple,int length,int colNo,BaseCQ9SlotBetInfo betInfo)
        {
            return 0;
        }
        
        protected virtual List<CQ9WinInfoItem> calcWinInfoList(string[][] reelStatus,BaseCQ9SlotBetInfo betInfo,int[] wildMultiple,int respinColNo)
        {
            return new List<CQ9WinInfoItem>();
        }
    }
}
