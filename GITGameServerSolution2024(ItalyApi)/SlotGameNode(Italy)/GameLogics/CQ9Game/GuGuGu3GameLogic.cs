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
    class GuGuGu3GameLogic : BaseCQ9RespinSlotGame
    {

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "180";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 30;
            }
        }
        protected override int[] DenomDefine
        {
            get
            {
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 125, 250, 500, 750, 1250, 1, 2, 3, 5, 10, 30, 50, 80 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 1250;
            }
        }
        protected override string InitReelSetString
        {
            get
            {
                return "abde3ba940aff1daYwiD+rDrBTnZhcLWWHsv+Ccn2Y/AMdqCgphG9vUY1JJCH/ssa8tycTYyubaPBRa02vvFwKFzAhL3259WNqre6la5QgBh4toXoTj3dwGOhiDVKWRNcp6/zuVECJuJ9+L3wO/4EzSibmXgUJkQs1L9PgJEHdDKSnKY8v/EHo/fk2sSi7Fb6gIeQWB+IGpbQNEnHTNmd4apJ277ZCC5qmgoadhOSXvpVhkXc+sXCoOkxc69lioAZTOy+Qn6VY//Prec0XrxXZsNZqtevVEs9tl0ku3JMOJFqZS9bWuvF+oROhAgO5+1K8LC+CurlG7HvQ4FAaIIQo+wPzrL8bznhi8PV/rEKhW+vn3yDsqRGZ+C+GIVDf1nPmdWONrXmEl/9MrnzzLq0CFTL4cCOrQYUiZgCP8+4alOkOdI2Zac6lQC/+G3VsNY90GsbllXTX6VM0zFpUS2OYKMuEtPdO8wfMk0d+NPA+TuBC9OVdxsUY840fEkbny1fM7Hcn9EjEocDEQWGhejZO3n58TELtyKte5xQwxwof67Fd5zPuGqp16mQauAgBBAPomaf/o93rtxnm5t5CdjuRNuuvPlRnOB1QmqbtYNMNSN7/0fLBnicpF1B/C3VdJP9LEpK6J09z+jY/MRBAsSmHcl60vzd0ewQ2vcOQdyLNDB/ScOIijq57MEF9tWMfICSLoFBgPHnnJ3bwkzxp4CYeuC2pro8h6vWByCP5aQx8bl6BYNDcbLjMtGl76Q/pP2/oB7ZIW+GhWqnbiSV5maih++Ov1F/jswJg4NWFn9vHvd2dfPmHOjCsFR8KBnkHdob3FNNVbzpuNx9LEsi06Mg4YQYitqdGuBV20++gVuFx35y0KtZofJ1wvujk+oZX7N3oZsMgvkvELT4unu93/zN7fa4o8vh2z2TJvr5yaGuYYcDVJQUotj9Tzb1W34WCugNRpNjmXPjefY+//YdTCd+5uQ9FtLrWk1IiUgPtht/MwE3w5uxeeWnzt2TPOPmSYPThM6eDW5+x1tOGG9PxD5cP/ZZlKvcI/mHcgDdwpfzuAwNTOSvrZYXODbCESlmHl1+4Sl2VXwWkKTMG3hQmQnCNNasq8vV9GD43lvR1iWSuP/yALsq2DPSMxR7+kXylwaA/EIrcz0Tg0BbiRTLmX3MSeGiiD/0RWhS6MhCBmYQ7gtvM9R65bqp2t0Pfhm5ZFrVi1vvVsX6ztCfryseyNFfUc+d61I2J+FppfF77gDugjYj4ABXqsV2JscrHo/wVYxeHkWGGR3N1t+IyaFp70VbaB2Vhulyw91GMTpFuZpZLJnG5WynDPtWHCvr2Dq+bV7IpSsQLfw/TmTew06QMcg0nwOTgPEoWECwQ7XeNtxFvGUzpBk8q+AvH/qpxqre5xUI3cpUzEZYWQdrWr3twH62c/v38rQ4YIVxf4tStefX/g8LWuvrs4CPPQraWGpUpeVFpTZtrk6kSk4U9kGUXKxNW5AfOxpH16cw9uZO1lVqxhNGT0plOWU6ptFUm7IRBtaQsAD7l3cpA/iAMClLZXXSihDohd3uYQ9RzCDdO2Lshdj6P4Epe073q4oaC8ztFzyiMkKBGnjQ3ucSfQg2W3ZoRu/1vP+hpRVjiUJbwQDgpeGYi6sbSHuX1TTOn9/GXPQq9Et+5r57dGeiPHuGpXp8/5sA+K2hpsTXO9H5BnH94ah2JyNeMgn/C1NuxD+2ycSbb2W4ISyyeR7UoJ+kElQVckAzLR9Kfke/9Zm/CPkcXXIG7t0QvwYPA2kgGsInPK4eafNs+l9Vs4UKlJqPrWayyAF/iJLtPx6EhI/s7vcrl62g2NImrPs4xayEvsw8ZPlKtBz87ii2S8HLWO8y1FyGn4vX3ch4ls3f2+CS3bmMW0VGz3PGetWq9CkZXgNKkyssqZIkQIbHBSVDBXxynhKbdocIih1v+yfDTcb4hPE2+uYNapbbPij9/r/60HMaoiZhsm4zH9LmNoaIubpP5vI8sPBYTGYHaDEZByMBOnmroWBR0NPu+YwpeT+VdX369I75yVWpYgj6mh7lRKPnKbprcBuPMk94VnZ8SUK9VlkDlyauTUfCB4Ks393hs61zzctpuWULlK8NscOBNCrdztE2qJIb5B22EQEux4tlptzz/+wZINtIg5FrJh5qIvKIVRw3wVbX4tcgj26E1KqskPHHYdZRcDBwyIPf203Meg/7Sm91qIxEr0GP8QuBN2F8BaXGyZbdxIXeSrHHeolhx524FVb3lCmS0rfS25d+Gppi5fYl015MsfQFCBD9rGJAj5O9aFXZ+LjL2G9hQOzYcqn2Ll/HnU/MQH4CZwc6weOPECJkameW0z9Jh/43LV01XMsydWbdf00BIyPx/qUHBGi+Z6t5wD3EYU8gHwRxnJWuUEwbOAOCsZ6nZv2Np/oXDG1dxOcnnQ3JY8W+x77By9LbpOPsM/V2j0v3qjkxGI9LMSj2q1AXwCcjgB/OKGDfL7mo0n+5bnYeDIyfaBQ05TFbfj6zoWGygj7QspeJ9x6gI1X/X8cYRjfPRdwD75geKiPzngqFUMLVGrAStnhqUmJGYOKDH6vLoGAzgXhn1CHy17vh+Ea1GsRPU+DsYy2/7qs5cPnI5CN+kq13BjYA7PRX5vN72rO5nQuVmPVN0/nkH678RSWiYTHH8aioulyJUWBpPHfvHFVC92BEnmOvp1vxIj12OMn9/NEW/jf81z+1Z938Kvc1i6usCwQ6XDCTv97EW1lFNrVMTehqNS3YN3pvDh1CXrEl7gMFBuPh5LZRwT10WBunFWISP7qdaOsDUpFGmd8aPX3nxguTfJmOaO3vlZ0JAxQKtr0QXy4erNVnX4vW5uNuZwn1iItyXpmQ80+5Xt8+rSfjsdxsnfvT8hJwpRkJWxSKerXgyUVUA1dnrHug8sQt24PvCCqP2SmXygNZ+c/dCEWJsS6c6aNtNOhfpbOZRwBQ/6LjZjupYP/UAYn5CYPSUTJWo7c2oAaIaov+yNK1EMI8+AwU+rGnMF/fVLvrEeS9sLhGfQlEvpox9nZjADomdjWv0LfBgeqF+elBHzMvqqLd5KG0e/I88TxK3zjQL1uHueM5W5ZnKnnxTVyQItQvv6rrMMqQq7V/R+JlNnNCxyLSJqxISGaVj1BUoiyuj2QV1/VJTlOx9qZDduU0VHGLAzstJOUn3E/YZmkjxZ6fB5CVbov9HH8";
            }
        }
        protected override string CQ9GameName
        {
            get
            {
                return "Gu Gu Gu 3";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\":\"en\",\"name\":\"GuGuGu3\"}," +
                    "{\"lang\":\"es\",\"name\":\"GuGuGu3\"}," +
                    "{\"lang\":\"id\",\"name\":\"GuGuGu3\"}," +
                    "{\"lang\":\"ja\",\"name\":\"グググ3\"}," +
                    "{\"lang\":\"ko\",\"name\":\"구구구3\"}," +
                    "{\"lang\":\"th\",\"name\":\"กูกูกู3\"}," +
                    "{\"lang\":\"vn\",\"name\":\"GuGuGu3\"}," +
                    "{\"lang\":\"zh-cn\",\"name\":\"咕咕咕3\"}]";
            }
        }
        protected override string[][] BaseReelSet
        {
            get
            {
                return new string[][] {
                    new string[] { "11", "12", "1", "13", "14", "5", "15", "16", "4", "11", "15", "3", "12", "15", "2", "13", "14", "5", "11", "14", "4", "15", "16", "2", "13", "12", "5", "11", "14", "4", "13", "5", "12", "2", "16", "3", "15", "5", "13", "4", "16", "12", "F", "11", "12", "5", "14", "16", "1" },
                    new string[] { "13", "1", "14", "2", "13", "14", "3", "11", "12", "4", "15", "5", "16", "11", "W", "12", "15", "2", "14", "5", "16", "12", "4", "11", "15", "3", "14", "16", "F", "13", "1", "14", "5", "13", "14", "3", "11", "12", "4", "15", "5", "16", "11", "W", "12", "13", "2", "14", "5", "13", "12", "4", "11", "15", "3", "14", "16" },
                    new string[] { "12", "11", "3", "14", "15", "5", "16", "14", "5", "12", "16", "F", "14", "15", "5", "13", "11", "4", "12", "16", "3", "14", "13", "4", "11", "15", "5", "13", "12", "4", "14", "1", "15", "2", "16", "4", "13", "5", "11", "4", "12", "14", "F", "13", "14" },
                    new string[] { "11", "1", "13", "4", "16", "F", "15", "1", "16", "14", "3", "13", "2", "15", "W", "11", "12", "5", "16", "2", "15", "14", "F", "13", "3", "12", "5", "11", "F", "11", "3", "13", "4", "12", "2", "15", "1", "16", "14", "3", "13", "4", "12", "W", "11", "12", "5", "16", "2", "15", "14", "4", "13", "3", "12", "5", "11" },
                    new string[] { "13", "14", "2", "15", "11", "5", "14", "12", "4", "13", "14", "3", "15", "16", "F", "13", "11", "1", "16", "15", "3", "16", "12", "2", "13", "15", "5", "11", "14", "4", "15", "1", "12", "2", "16", "3", "13", "5", "14", "4", "15", "F", "13", "5", "14", "1", "12", "3", "15", "4", "16", "11", "1", "13", "14", "2" }
                };
            }
        }
        protected override Dictionary<string, int[]> PayTable
        {
            get
            {
                return new Dictionary<string, int[]>() {
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
        #endregion

        public GuGuGu3GameLogic()
        {
            _initData.BetButton      = BetButton;
            _initData.MaxBet         = MaxBet;
            _gameID  = GAMEID.GuGuGu3;
            GameName = "GuGuGu3";
        }

        protected override SortedDictionary<double,List<BaseCQ9SlotSpinResult>> getAllAvailableSpinResults(int respinColNo, BaseCQ9SlotSpinResult lastResult,BaseCQ9SlotBetInfo betInfo)
        {
            int[] oldReelStops  = new int[5];
            long[] reelPay      = new long[5];
            
            
            SortedDictionary<double, List<BaseCQ9SlotSpinResult>> dicResults = new SortedDictionary<double, List<BaseCQ9SlotSpinResult>>();
            for(int i = 0; i < BaseReelSet[respinColNo].Length; i++)
            {
                dynamic oldResultContext = JsonConvert.DeserializeObject<dynamic>(lastResult.ResultString);

                JArray oldReelStopArray = oldResultContext["RngData"];
                JArray oldReelPayArray = oldResultContext["ReelPay"];
                for (int j = 0; j < 5; j++)
                {
                    oldReelStops[j] = Convert.ToInt32(oldReelStopArray[j]);
                    reelPay[j]      = Convert.ToInt32(oldReelPayArray[j]);
                }

                dynamic newResultContext = oldResultContext;
                int[] reelStops = oldReelStops;
                JArray respinReels = new JArray() { 0, 0, 0, 0, 0 };
                respinReels[respinColNo] = 1;
                string[][] reelStatus = new string[3][] {
                    new string[5],
                    new string[5],
                    new string[5],
                };

                reelStops[respinColNo] = i;
                int row = 0;
                do
                {
                    for (int j = 0; j < reelStops.Length; j++)
                        reelStatus[row][j] = BaseReelSet[j][(reelStops[j] - 2 + row + BaseReelSet[j].Length) % BaseReelSet[j].Length];
                    row++;
                }
                while (row < 3);

                int scatterCnt = findAllScatterCnt(reelStatus);
                if (scatterCnt >= 3)
                    continue;

                string[] SymbolResult = new string[] {
                    string.Join(",", reelStatus[0]),
                    string.Join(",", reelStatus[1]),
                    string.Join(",", reelStatus[2]) 
                };

                int[] mulVals1  = new int[] { 1, 2, 3 };
                int[] probs1    = new int[] { 3, 2, 1 };
                int multiple1   = mulVals1[GameUtils.selectFromProbs(probs1)];

                int[] mulVals2  = new int[] { 2, 3, 5 };
                int[] probs2    = new int[] { 3, 1, 1 };
                int multiple2   = mulVals2[GameUtils.selectFromProbs(probs2)];

                JArray symJArray = newResultContext["SymbolResult"];
                JArray rngJArray = newResultContext["RngData"];
                for (int j = 0; j < 3; j++)
                    symJArray[j]    = SymbolResult[j];
                for(int j = 0; j < 5; j++)
                    rngJArray[j] = reelStops[j];
                newResultContext["SymbolResult"]   = symJArray;
                newResultContext["RngData"]        = rngJArray;
                newResultContext["Multiple"]       = string.Format("{0} {1}", multiple1, multiple2);

                JArray newReelPay = oldReelPayArray;
                for(int j = 0; j < 5; j++)
                {
                    if (j == respinColNo)
                        continue;

                    newReelPay[j] = calcColumnReelPay(reelStatus, j, betInfo, new int[] { 0, multiple1, 0, multiple2, 0 });
                }
                List<CQ9WinInfoItem> winInfos = calcWinInfoList(reelStatus, betInfo, new int[] { 0,multiple1,0,multiple2,0},respinColNo);
                int basewin = 0, totalwin = 0;
                foreach(CQ9WinInfoItem item in winInfos)
                {
                    basewin += item.LinePrize;
                    totalwin = basewin;
                }

                JArray udsOutputWinLine = JArray.FromObject(winInfos);
                newResultContext["udsOutputWinLine"]        = udsOutputWinLine; 
                newResultContext["BaseWin"]                 = basewin;
                newResultContext["TotalWin"]                = totalwin;
                newResultContext["WinLineCount"]            = winInfos.Count;
                newResultContext["WinType"]                 = winInfos.Count > 0 ? 1 : 0;
                newResultContext["RespinReels"]             = respinReels;
                newResultContext["ReelPay"]                 = newReelPay;
                newResultContext["GamePlaySerialNumber"]    = ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds / 10 + 455000000000);


                CQ9Actions currentAction                = CQ9Actions.NormalSpin;

                BaseCQ9SlotSpinResult newResult = new BaseCQ9SlotSpinResult();
                newResult.TotalWin      = totalwin;
                newResult.ResultString  = JsonConvert.SerializeObject(newResultContext);
                newResult.MessageID     = lastResult.MessageID;
                newResult.Action        = currentAction;

                double odd = (double)totalwin / (betInfo.TotalBet);
                if (!dicResults.ContainsKey(odd))
                    dicResults.Add(odd, new List<BaseCQ9SlotSpinResult>());
                dicResults[odd].Add(newResult);
            }
            return dicResults;
        }
        
        protected override int AddScatterAvalableMoney(int reelNo,BaseCQ9SlotBetInfo betInfo)
        {
            int[] scatterAvMaxMoneys = new int[] { 41963, 36073, 91385, 108219, 73434 };   //15000(최대)베팅때 스캐터가능 리스핀 모니
            return scatterAvMaxMoneys[reelNo] * (betInfo.MiniBet * betInfo.PlayBet * betInfo.PlayLine)/ 15000;
        }
        
        protected override int calcColumnReelPay(string[][] reelStatus,int calcColNo,BaseCQ9SlotBetInfo betInfo,int[] wildMultiple)
        {
            int reelPay = 0;

            int scatterCnt = findAllScatterCnt(reelStatus);
            if (scatterCnt == 2 && !checkScatterInColumn(reelStatus, calcColNo))
                reelPay += AddScatterAvalableMoney(calcColNo, betInfo);

            Dictionary<string, int> sameSymbolDic = findSameSymbolAndLenth(reelStatus, calcColNo);
            if (sameSymbolDic.Count == 0)
                return reelPay > 0 ? reelPay : 1;

            foreach(KeyValuePair<string,int> pair in sameSymbolDic)
            {
                if (pair.Value < calcColNo)
                    continue;
                int multiple    = 1;
                int col         = -1;
                do
                {
                    col++;
                    if (calcColNo == 0 && col == 1)
                        continue;
                    if (calcColNo != 0 && col == 0)
                        continue;

                    int symbolCnt = 0;
                    for (int row = 0; row < 3; row++)
                    {
                        if (reelStatus[row][col] == pair.Key)
                            symbolCnt++;
                        if (reelStatus[row][col] == "W")
                            symbolCnt += wildMultiple[col];
                    }
                    if (symbolCnt == 0)
                        break;
                    multiple *= symbolCnt;
                    if (col == 4)
                        break;
                }
                while (true);
                reelPay += CalcRespinOfSymbol(pair.Key, multiple, pair.Value,calcColNo,betInfo);
            }
            return reelPay > 0 ? reelPay : 1;
        }
        
        protected override List<string> AddAllSymbolToStart()
        {
            return new List<string>(){"1","2","3","4","5","11","12","13","14","15","16"};
        }
        
        protected override int CalcRespinOfSymbol(string symbol,int multiple,int length,int colNo,BaseCQ9SlotBetInfo betInfo)
        {
            int[] maxSymbolMaxBetAvMoney    = new int[]     { 63809,    146458, 34741,   235869, 111666 };   //15000(최대)베팅때 최대당첨심벌 값
            double[] additionalwildSymMul   = new double[]  {     0,  10.0/3.0,     0, 28.0/5.0,     0  };

            double maxSymbolCnt = findSymbolCntInReelSet("1", colNo);
            maxSymbolCnt        += additionalwildSymMul[colNo];
            
            double cnt          = findSymbolCntInReelSet(symbol, colNo);
            cnt                 += additionalwildSymMul[colNo];

            double money = (double)maxSymbolMaxBetAvMoney[colNo] * PayTable[symbol][length] / PayTable["1"][4]
                        * cnt / maxSymbolCnt * multiple
                        * betInfo.PlayBet * betInfo.PlayLine * betInfo.MiniBet / 15000;
            return (int)money;
        }

        protected override List<CQ9WinInfoItem> calcWinInfoList(string[][] reelStatus,BaseCQ9SlotBetInfo betInfo,int[] wildMultiple,int respinColNo)
        {
            List<CQ9WinInfoItem> winInfoList    = new List<CQ9WinInfoItem>();
            List<string> startSymbols       = new List<string>();
            
            for(int row = 0; row < 3; row++)
            {
                if (startSymbols.Contains(reelStatus[row][0]))
                    continue;
                startSymbols.Add(reelStatus[row][0]);
            }

            for(int i = 0; i < startSymbols.Count;i++)
            {
                if (startSymbols[i] == "F")
                    continue;

                string symbol = startSymbols[i];
                int setLength       = 0;
                int paySymbolCnt    = 0;
                int paySymMultiple  = 1;
                do
                {
                    int cntInCol    = 0;
                    for(int row = 0; row < 3; row++)
                    {
                        if (reelStatus[row][setLength] == symbol)
                        {
                            paySymbolCnt++;
                            cntInCol++;
                        }
                        if(reelStatus[row][setLength] == "W")
                        {
                            paySymbolCnt++;
                            cntInCol += wildMultiple[setLength];
                        }
                    }
                    if (cntInCol == 0)
                        break;
                    setLength++;
                    paySymMultiple *= cntInCol;
                    if (setLength == 5)
                        break;
                }
                while (true);
                if (setLength < 3)
                    continue;
                if (setLength < respinColNo + 1)
                    continue;

                int[][] winPostion = new int[][]
                {
                    new int[]{0,0,0,0,0},
                    new int[]{0,0,0,0,0},
                    new int[]{0,0,0,0,0},
                };
                int lineMultiplier = 1;
                for(int row = 0; row < 3; row++)
                {
                    for(int col = 0;col < 5; col++)
                    {
                        if (reelStatus[row][col] == symbol)
                            winPostion[row][col] = 1;
                        if (reelStatus[row][col] == "W")
                        {
                            winPostion[row][col] = 2;
                            lineMultiplier *= wildMultiple[col];
                        }
                    }
                }
                CQ9WinInfoItem winInfoItem = new CQ9WinInfoItem();
                winInfoItem.SymbolId        = symbol;
                winInfoItem.NumOfKind       = setLength;
                winInfoItem.SymbolCount     = paySymbolCnt;
                winInfoItem.WinLineNo       = i;
                winInfoItem.LineMultiplier  = lineMultiplier;
                winInfoItem.WinPosition     = winPostion;
                winInfoItem.LineExtraData   = new int[] { 0 };
                winInfoItem.LineType        = 0;
                winInfoItem.LinePrize       = PayTable[symbol][setLength - 1] * paySymMultiple
                                                * betInfo.PlayBet * betInfo.PlayLine;
                winInfoList.Add(winInfoItem);
            }
            return winInfoList;
        }
    }
}
