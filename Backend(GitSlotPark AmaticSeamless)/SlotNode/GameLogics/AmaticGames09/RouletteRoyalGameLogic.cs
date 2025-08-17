using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using System.IO;
using Akka.Actor;
using Newtonsoft.Json;
using Akka.Configuration;
using SlotGamesNode.Database;
using PCGSharp;

namespace SlotGamesNode.GameLogics
{
    public enum RouletteBetTypes
    {
        None        = -1,   //없음
        Straight    = 0,    //직접 35배
        Split       = 1,    //2중하나 17배
        Street      = 2,    //한행에있는 3중 하나 11배
        Square      = 3,    //점주위에있는 4중 하나 8배
        Line        = 4,    //연속인 6숫자 5배
        Column      = 5,    //3으로 나눈 나머지별로 2배
        Dozen       = 6,    //1-12,13-24,25-36 2배
        HighLow     = 7,    //1-18,19-36 1배
        EvenOdd     = 8,    //홋수,짝수 1배
        Color       = 9,    //Red/Black 1배
    }

    public class RouletteRoyalBetInfo
    {
        public string           BetGroup            { get; set; }
        public Currencies       CurrencyInfo        { get; set; }   //화페
        public string           RoundID             { get; set; }
        public string           BetTransactionID    { get; set; }

        public RouletteRoyalBetInfo()
        {
        }

        public virtual void SerializeFrom(BinaryReader reader)
        {
            BetGroup       = reader.ReadString();
            CurrencyInfo   = (Currencies)reader.ReadInt32();
            
            bool hasValue = reader.ReadBoolean();
            if (hasValue)
                RoundID = reader.ReadString();

            hasValue = reader.ReadBoolean();
            if (hasValue)
                BetTransactionID = reader.ReadString();
        }

        public virtual void SerializeTo(BinaryWriter writer)
        {
            writer.Write(BetGroup);
            writer.Write((int)CurrencyInfo);

            if (RoundID == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                writer.Write(RoundID);
            }
            if (BetTransactionID == null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                writer.Write(BetTransactionID);
            }
        }
        
        public byte[] convertToByte()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    this.SerializeTo(bw);
                }
                return ms.ToArray();
            }
        }
    }

    public class RouletteRoyalResult : BaseAmaticSlotSpinResult
    {
        public List<int> WinNumberLogs { get; set; }

        public override void SerializeTo(BinaryWriter writer)
        {
            base.SerializeTo(writer);

            if(WinNumberLogs == null)
            {
                writer.Write(0);
                return;
            }

            writer.Write(WinNumberLogs.Count);
            for(int i = 0; i < WinNumberLogs.Count; i++)
            {
                writer.Write(WinNumberLogs[i]);
            }
        }
        public override void SerializeFrom(BinaryReader reader)
        {
            base.SerializeFrom(reader);

            WinNumberLogs = new List<int>();
            int length      = reader.ReadInt32();
            for(int i = 0; i < length; i++)
            {
                WinNumberLogs.Add(reader.ReadInt32());
            }
        }
    }

    class RouletteRoyalGameLogic : IGameLogicActor
    {
        #region 게임고유속성값
        protected string SymbolName
        {
            get
            {
                return "RouletteRoyal";
            }
        }
        protected string InitString
        {
            get
            {
                return "005073b30b5841a32338421b101a24b296312c32ee4c350413884271043a9844e204753049c404c350101010101010101021d21f21c22321d219191b181f1a2231821f21421421b";
            }
        }
        protected long[] BettingButton
        {
            get
            {
                return new long[] { 10, 75, 150, 300, 750 };
            }
        }
        protected long[] BetLimints
        {
            get
            {
                return new long[] { 5000, 10000, 15000, 20000, 30000, 40000, 50000 };
            }
        }
        //매유저의 베팅정보 
        protected Dictionary<string, RouletteRoyalBetInfo>      _dicUserBetInfos                = new Dictionary<string, RouletteRoyalBetInfo>();

        //유정의 마지막결과정보
        protected Dictionary<string, RouletteRoyalResult>       _dicUserResultInfos             = new Dictionary<string, RouletteRoyalResult>();

        //백업정보
        protected Dictionary<string, BaseAmaticSlotSpinResult>  _dicUserLastBackupResultInfos   = new Dictionary<string, BaseAmaticSlotSpinResult>();
        protected Dictionary<string, byte[]>                    _dicUserLastBackupBetInfos      = new Dictionary<string, byte[]>();

        private Dictionary<RouletteBetTypes, RouletteStrAndMultiple> DicRouletteRoyal
        {
            get
            {
                return new Dictionary<RouletteBetTypes, RouletteStrAndMultiple>() {
                    { 
                        RouletteBetTypes.Straight, new RouletteStrAndMultiple(new List<string>()
                        {
                            "0",
                            "1",
                            "2",
                            "3",
                            "4",
                            "5",
                            "6",
                            "7",
                            "8",
                            "9",
                            "10",
                            "11",
                            "12",
                            "13",
                            "14",
                            "15",
                            "16",
                            "17",
                            "18",
                            "19",
                            "20",
                            "21",
                            "22",
                            "23",
                            "24",
                            "25",
                            "26",
                            "27",
                            "28",
                            "29",
                            "30",
                            "31",
                            "32",
                            "33",
                            "34",
                            "35",
                            "36",
                        }, 35)
                    },
                    {
                        RouletteBetTypes.Split, new RouletteStrAndMultiple(new List<string>() 
                        {
                            "0,1",
                            "0,2",
                            "0,3",
                            "1,2",
                            "1,4",
                            "2,3",
                            "2,5",
                            "3,6",
                            "4,5",
                            "4,7",
                            "5,6",
                            "5,8",
                            "6,9",
                            "7,8",
                            "7,10",
                            "8,9",
                            "8,11",
                            "9,12",
                            "10,11",
                            "10,13",
                            "11,12",
                            "11,14",
                            "12,15",
                            "13,14",
                            "13,16",
                            "14,15",
                            "14,17",
                            "15,18",
                            "16,17",
                            "16,19",
                            "17,18",
                            "17,20",
                            "18,21",
                            "19,20",
                            "19,22",
                            "20,21",
                            "20,23",
                            "21,24",
                            "22,23",
                            "22,25",
                            "23,24",
                            "23,26",
                            "24,27",
                            "25,26",
                            "25,28",
                            "26,27",
                            "26,29",
                            "27,30",
                            "28,29",
                            "28,31",
                            "29,30",
                            "29,32",
                            "30,33",
                            "31,32",
                            "31,34",
                            "32,33",
                            "32,35",
                            "33,36",
                            "34,35",
                            "35,36",
                        }, 17)
                    },
                    {
                        RouletteBetTypes.Street, new RouletteStrAndMultiple(new List<string>()
                        {
                            "1-3",
                            "4-6",
                            "7-9",
                            "10-12",
                            "13-15",
                            "16-18",
                            "19-21",
                            "22-24",
                            "25-27",
                            "28-30",
                            "31-33",
                            "34-36",
                        }, 11)
                    },
                    {
                        RouletteBetTypes.Square, new RouletteStrAndMultiple(new List<string>()
                        {
                            "1.5",
                            "2.6",
                            "4.8",
                            "5.9",
                            "7.11",
                            "8.12",
                            "10.14",
                            "11.15",
                            "13.17",
                            "14.18",
                            "16.20",
                            "17.21",
                            "19.23",
                            "20.24",
                            "22.26",
                            "23.27",
                            "25.29",
                            "26.30",
                            "28.32",
                            "29.33",
                            "31.35",
                            "32.36",
                        }, 9)
                    },
                    {
                        RouletteBetTypes.Line, new RouletteStrAndMultiple(new List<string>()
                        {
                            "1-6",
                            "4-9",
                            "7-12",
                            "10-15",
                            "13-18",
                            "16-21",
                            "19-24",
                            "22-27",
                            "25-30",
                            "28-33",
                            "31-36",
                        }, 5)
                    },
                    {
                        RouletteBetTypes.Column, new RouletteStrAndMultiple(new List<string>()
                        {
                            "1/12",//3으로 나눈 나머지가 1
                            "2/12",//3으로 나눈 나머지가 2
                            "3/12",//3으로 나눈 나머지가 0,(숫자 0은 제외)
                        }, 2)
                    },
                    {
                        RouletteBetTypes.Dozen, new RouletteStrAndMultiple(new List<string>()
                        {
                            "1-12",//1st
                            "13-24",//2nd
                            "25-36",//3rd
                        }, 2)
                    },
                    {
                        RouletteBetTypes.HighLow, new RouletteStrAndMultiple(new List<string>()
                        {
                            "1-18",//Low
                            "19-36",//High
                        }, 1)
                    },
                    {
                        RouletteBetTypes.EvenOdd, new RouletteStrAndMultiple(new List<string>()
                        {
                            "even",
                            "odd",
                        }, 1)
                    },
                    {
                        RouletteBetTypes.Color, new RouletteStrAndMultiple(new List<string>()
                        {
                            "red",
                            "black",
                        }, 1)
                    },
                };
            }
        }
        #endregion

        public RouletteRoyalGameLogic()
        {
            _gameID     = GAMEID.RouletteRoyal;
            GameName    = "RouletteRoyal";
        }

        protected override async Task onProcMessage(string strUserID, int websiteID, GITMessage message, double userBalance, Currencies currency)
        {
            string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);
            if (message.MsgCode == (ushort)CSMSG_CODE.CS_AMATIC_DOINIT)
                onDoInit(strGlobalUserID, message, userBalance, currency);
            else if(message.MsgCode == (ushort)CSMSG_CODE.CS_AMATIC_DOHEARTBEAT)
                    onDoHeartBeat(strGlobalUserID, message, userBalance, currency);
            else if (message.MsgCode == (ushort)CSMSG_CODE.CS_AMATIC_DOSPIN)
                await onDoSpin(strUserID, websiteID, message, userBalance, currency);
        }
        protected void onDoInit(string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                string strResponse = buildInitString(strUserID, userBalance, currency);

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_AMATIC_DOINIT);
                responseMessage.Append(strResponse);

                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in RouletteRoyalGameLogic::onDoInit GameID: {0}, {1}", _gameID, ex);
            }
        }
        protected void onDoHeartBeat(string strUserID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                RouletteRoyalBetInfo betInfo = new RouletteRoyalBetInfo() { CurrencyInfo = currency };
                string strResponse = buildResMsgString(betInfo, strUserID, userBalance, -1, 0, AmaticMessageType.HeartBeat);

                GITMessage responseMessage = new GITMessage((ushort)SCMSG_CODE.SC_AMATIC_DOHEARTBEAT);
                responseMessage.Append(strResponse);

                Sender.Tell(new ToUserMessage((int)_gameID, responseMessage), Self);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in RouletteRoyalGameLogic::onDoHeartBeat GameID: {0}, {1}", _gameID, ex);
            }
        }
        protected async Task onDoSpin(string strUserID, int websiteID, GITMessage message, double userBalance, Currencies currency)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);
                if (message.MsgCode == (ushort)CSMSG_CODE.CS_AMATIC_DOSPIN)
                    readBetInfoFromMessage(message, strGlobalUserID, currency);

                await spinGame(strUserID, websiteID, userBalance);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in RouletteRoyalGameLogic::onDoSpin GameID: {0}, {1}", _gameID, ex);
            }
        }
        protected async Task spinGame(string strUserID, int websiteID, double userBalance)
        {
            try
            {
                string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);

                //해당 유저의 베팅정보를 얻는다. 만일 베팅정보가 없다면(례외상황) 그대로 리턴한다.
                RouletteRoyalBetInfo betInfo = null;
                if (!_dicUserBetInfos.TryGetValue(strGlobalUserID, out betInfo))
                    return;

                byte[] betInfoBytes = backupBetInfo(betInfo);

                RouletteRoyalResult lastResult = null;
                if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
                    lastResult = _dicUserResultInfos[strGlobalUserID];

                betInfo.BetTransactionID    = createTransactionID();
                betInfo.RoundID             = createRoundID();

                //결과를 생성한다.
                RouletteRoyalResult spinResult = await generateSpinResult(betInfo, strUserID, websiteID, userBalance);

                //게임로그
                string strGameLog = string.Format("{0}:{1}", spinResult.ResultString, betInfo.BetGroup);
                _dicUserResultInfos[strGlobalUserID]  = spinResult;

                //결과를 보내기전에 베팅정보를 디비에 보관한다
                saveBetResultInfo(strGlobalUserID);

                //생성된 게임결과를 유저에게 보낸다.
                long totalBet = getTotalBet(betInfo);
                sendGameResult(betInfo, spinResult, totalBet * getPointUnit(betInfo), spinResult.TotalWin, strGameLog);

                _dicUserLastBackupBetInfos[strGlobalUserID]       = betInfoBytes;
                _dicUserLastBackupResultInfos[strGlobalUserID]    = lastResult;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in RouletteRoyalGameLogic::spinGame {0}", ex);
            }
        }
        protected async Task<RouletteRoyalResult> generateSpinResult(RouletteRoyalBetInfo betInfo, string strUserID, int websiteID, double userBalance)
        {
            RouletteRoyalResult result = null;

            List<int> whiteList = findWhiteNumList(betInfo);
            List<int> blackList = findBlackNumList(whiteList);
            int rndNum          = selectRandomNum(websiteID, blackList);

            long totalBet       = getTotalBet(betInfo);
            long totalWin       = getTotalWin(betInfo, rndNum);
            double pointUnit    = getPointUnit(betInfo);

            string strGlobalUserID = string.Format("{0}_{1}", websiteID, strUserID);
            List<int> winNumberLogs = new List<int>();
            if (_dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                foreach (int winNum in _dicUserResultInfos[strGlobalUserID].WinNumberLogs)
                    winNumberLogs.Add(winNum);
            }

            if (await checkWebsitePayoutRate(websiteID, totalBet * pointUnit, totalWin * pointUnit))
            {
                result = calculateResult(betInfo, strGlobalUserID, userBalance + (totalWin - totalBet) * pointUnit, totalWin, rndNum);
                
                winNumberLogs.Add(rndNum);
                if (winNumberLogs.Count > 17)
                    winNumberLogs.RemoveAt(0);
                result.WinNumberLogs = winNumberLogs;
                
                return result;
            }

            int emptyNum = -1;
            if(blackList.Count > 1)
                emptyNum = blackList[Pcg.Default.Next(0, blackList.Count)];
            else
                emptyNum = Pcg.Default.Next(0, 37);

            long emptyWin = getTotalWin(betInfo, emptyNum);
            result = calculateResult(betInfo, strGlobalUserID, userBalance + (emptyWin - totalBet) * pointUnit, emptyWin, emptyNum);
            
            winNumberLogs.Add(emptyNum);
            if (winNumberLogs.Count > 17)
                winNumberLogs.RemoveAt(0);
            result.WinNumberLogs = winNumberLogs;
            
            sumUpWebsiteBetWin(websiteID, totalBet * pointUnit, emptyWin * pointUnit);
            return result;
        }
        protected RouletteRoyalResult calculateResult(RouletteRoyalBetInfo betInfo, string strGlobalUserID, double userBalance, long relWinMoney, int rnd)
        {
            try
            {
                RouletteRoyalResult spinResult  = new RouletteRoyalResult();
                double pointUnit = getPointUnit(betInfo);
                
                spinResult.TotalWin     = Math.Round(relWinMoney * pointUnit, 2);
                spinResult.ResultString = buildResMsgString(betInfo, strGlobalUserID, userBalance, rnd, relWinMoney, AmaticMessageType.FreeTrigger);
                spinResult.Action       = AmaticMessageType.FreeTrigger;
                return spinResult;
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occurred in RouletteRoyalGameLogic::calculateResult {0}", ex);
                return null;
            }
        }
        private long getUnixMiliTimestamp()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            long unixTimeMilliseconds = now.ToUnixTimeMilliseconds();
            return unixTimeMilliseconds;
        }
        protected string createRoundID()
        {
            return string.Format("{0}{1}", getUnixMiliTimestamp(), Guid.NewGuid().ToString().Replace("-", ""));
        }
        protected string createTransactionID()
        {
            return string.Format("{0}{1}", getUnixMiliTimestamp(), Guid.NewGuid().ToString().Replace("-", ""));
        }

        protected void sendGameResult(RouletteRoyalBetInfo betInfo, RouletteRoyalResult spinResult, double betMoney, double winMoney, string strGameLog)
        {
            GITMessage message = new GITMessage((ushort)SCMSG_CODE.SC_AMATIC_DOSPIN);
            message.Append(spinResult.ResultString);

            ToUserResultMessage toUserResult = new ToUserResultMessage((int)_gameID, message, betMoney, winMoney, new GameLogInfo(GameName, "0", strGameLog), UserBetTypes.Normal);
            toUserResult.BetTransactionID   = betInfo.BetTransactionID;
            toUserResult.RoundID            = betInfo.RoundID;
            toUserResult.TransactionID      = createTransactionID();
            toUserResult.RoundEnd           = true;

            Sender.Tell(toUserResult, Self);
        }
        
        private int selectRandomNum(int websiteID, List<int> blackList)
        {
            double payoutRate   = getPayoutRate(websiteID);
            double randomDouble = Pcg.Default.NextDouble(0.0, 100.0);
            
            if(blackList.Count > 1 && (randomDouble >= payoutRate || payoutRate == 0))
            {
                int rnd = Pcg.Default.Next(0, blackList.Count);
                return blackList[rnd];
            }
            else
            {
                int rnd = Pcg.Default.Next(0, 37);
                return rnd;
            }
        }
        private List<int> findWhiteNumList(RouletteRoyalBetInfo betInfo)
        {
            List<int> whiteList = new List<int>();

            string[] betGroups  = betInfo.BetGroup.Split(new string[] { "|" }, StringSplitOptions.None);
            foreach(string betChip in betGroups)
            {
                if (string.IsNullOrEmpty(betChip) && !betChip.Contains("$"))
                    continue;

                string strBetCat    = betChip.Split(new string[] { "$" }, StringSplitOptions.None)[0];
                List<int> betList   = findNumListFromBetCat(strBetCat);
                foreach(int num in betList)
                {
                    if (!whiteList.Contains(num))
                        whiteList.Add(num);
                }
            }

            return whiteList;
        }
        private List<int> findBlackNumList(List<int> whiteList)
        {
            List<int> blackList = new List<int>();
            for(int i = 0; i < 37; i++)
            {
                if (!whiteList.Contains(i))
                    blackList.Add(i);
            }
            return blackList;
        }
        private List<int> findNumListFromBetCat(string betCat)
        {
            List<int> numList = new List<int>();
            RouletteBetTypes betType = RouletteBetTypes.None;
            foreach (RouletteBetTypes type in DicRouletteRoyal.Keys)
            {
                int index = DicRouletteRoyal[type].KeyStr.FindIndex(_ => _ == betCat);
                if(index != -1)
                {
                    betType = type;
                    break;
                }
            }

            if (betType == RouletteBetTypes.Straight)
            {
                numList.Add(Convert.ToInt32(betCat));
            }
            else if(betType == RouletteBetTypes.Split)
            {
                int num1 = Convert.ToInt32(betCat.Split(new string[] { "," }, StringSplitOptions.None)[0]);
                int num2 = Convert.ToInt32(betCat.Split(new string[] { "," }, StringSplitOptions.None)[1]);
                numList.Add(num1);
                numList.Add(num2);
            }
            else if (betType == RouletteBetTypes.Street || betType == RouletteBetTypes.Line || betType == RouletteBetTypes.Dozen || betType == RouletteBetTypes.HighLow)
            {
                int from    = Convert.ToInt32(betCat.Split(new string[] { "-" }, StringSplitOptions.None)[0]);
                int to      = Convert.ToInt32(betCat.Split(new string[] { "-" }, StringSplitOptions.None)[1]);
                for(int i = from; i <= to; i++)
                    numList.Add(i);
            }
            else if(betType == RouletteBetTypes.Square)
            {
                int num1 = Convert.ToInt32(betCat.Split(new string[] { "." }, StringSplitOptions.None)[0]);
                int num2 = Convert.ToInt32(betCat.Split(new string[] { "." }, StringSplitOptions.None)[1]);
                numList.Add(num1);
                numList.Add(num2);
                numList.Add(num1 + 1);
                numList.Add(num2 - 1);
            }
            else if(betType == RouletteBetTypes.Column)
            {
                int remain = Convert.ToInt32(betCat.Split(new string[] { "/" }, StringSplitOptions.None)[0]);
                remain %= 3;
                for(int i = 1; i < 37; i++)
                {
                    if (i % 3 == remain)
                        numList.Add(i);
                }
            }
            else if(betType == RouletteBetTypes.EvenOdd)
            {
                if(betCat == "even")
                {
                    for (int i = 1; i < 37; i++)
                    {
                        if (i % 2 == 0)
                            numList.Add(i);
                    }
                }
                else if(betCat == "odd")
                {
                    for (int i = 1; i < 37; i++)
                    {
                        if (i % 2 == 1)
                            numList.Add(i);
                    }
                }
            }
            else if (betType == RouletteBetTypes.Color)
            {
                if (betCat == "red")
                {
                    numList.Add(1);
                    numList.Add(3);
                    numList.Add(5);
                    numList.Add(7);
                    numList.Add(9);
                    numList.Add(12);
                    numList.Add(14);
                    numList.Add(16);
                    numList.Add(18);
                    numList.Add(19);
                    numList.Add(21);
                    numList.Add(23);
                    numList.Add(25);
                    numList.Add(27);
                    numList.Add(30);
                    numList.Add(32);
                    numList.Add(34);
                    numList.Add(36);
                }
                else if (betCat == "black")
                {
                    numList.Add(2);
                    numList.Add(4);
                    numList.Add(6);
                    numList.Add(8);
                    numList.Add(10);
                    numList.Add(11);
                    numList.Add(13);
                    numList.Add(15);
                    numList.Add(17);
                    numList.Add(20);
                    numList.Add(22);
                    numList.Add(24);
                    numList.Add(26);
                    numList.Add(28);
                    numList.Add(29);
                    numList.Add(31);
                    numList.Add(33);
                    numList.Add(35);
                }
            }

            return numList;
        }
        private bool isLegualBet(string betGroup)
        {
            try
            {
                string[] betGroups = betGroup.Split(new string[] { "|" }, StringSplitOptions.None);
                foreach (string betChip in betGroups)
                {
                    if (string.IsNullOrEmpty(betChip) && !betChip.Contains("$"))
                        return false;

                    string strBetCat = betChip.Split(new string[] { "$" }, StringSplitOptions.None)[0];
                    List<int> betList = findNumListFromBetCat(strBetCat);
                    if(betList.Count == 0)
                        return false;

                    string strBetAmount = betChip.Split(new string[] { "$" }, StringSplitOptions.None)[1];
                    long betAmount      = Convert.ToInt64(strBetAmount);

                    if(betAmount <= 0) 
                        return false;
                }
                return true;
            }
            catch 
            { 
                return false;
            }
        }
        private long getTotalBet(RouletteRoyalBetInfo betInfo)
        {
            long sum            = 0;
            string[] betGroups = betInfo.BetGroup.Split(new string[] { "|" }, StringSplitOptions.None);
            foreach (string betChip in betGroups)
            {
                try
                {
                    if (string.IsNullOrEmpty(betChip) && !betChip.Contains("$"))
                        continue;

                    string strBetAmount = betChip.Split(new string[] { "$" }, StringSplitOptions.None)[1];
                    long betAmount = Convert.ToInt64(strBetAmount);
                    sum += betAmount;
                }
                catch (Exception ex)
                {
                    _logger.Error("Exception in read betinfo {0} {1}", betInfo.BetGroup, ex);
                }
            }
            return sum;
        }
        private long getTotalWin(RouletteRoyalBetInfo betInfo, int rouletteNum)
        {
            long sum            = 0;
            string[] betGroups = betInfo.BetGroup.Split(new string[] { "|" }, StringSplitOptions.None);
            foreach (string betChip in betGroups)
            {
                if (string.IsNullOrEmpty(betChip) && !betChip.Contains("$"))
                    continue;

                string strBetCat    = betChip.Split(new string[] { "$" }, StringSplitOptions.None)[0];
                string strBetAmount = betChip.Split(new string[] { "$" }, StringSplitOptions.None)[1];

                List<int> betList = findNumListFromBetCat(strBetCat);
                if (betList.Contains(rouletteNum))
                {
                    long betAmount = Convert.ToInt64(strBetAmount);
                    
                    RouletteBetTypes betType = RouletteBetTypes.None;
                    foreach (RouletteBetTypes type in DicRouletteRoyal.Keys)
                    {
                        int index = DicRouletteRoyal[type].KeyStr.FindIndex(_ => _ == strBetCat);
                        if (index != -1)
                        {
                            betType = type;
                            break;
                        }
                    }
                    if(betType != RouletteBetTypes.None)
                        sum += (betAmount * (DicRouletteRoyal[betType].Multiple + 1));
                }
            }
            return sum;
        }
        protected virtual void readBetInfoFromMessage(GITMessage message, string strGlobalUserID, Currencies currency)
        {
            try
            {
                RouletteRoyalBetInfo betInfo   = new RouletteRoyalBetInfo();
                betInfo.BetGroup        = (string)message.Pop();
                betInfo.CurrencyInfo    = currency;

                if (!isLegualBet(betInfo.BetGroup))
                {
                    _logger.Error("{0} betInfo {1} or infinite in RouletteRoyalGameLogic::readBetInfoFromMessage", strGlobalUserID, betInfo.BetGroup);
                    return;
                }

                RouletteRoyalBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strGlobalUserID, out oldBetInfo))
                {
                    oldBetInfo.BetGroup     = betInfo.BetGroup;
                    oldBetInfo.CurrencyInfo = betInfo.CurrencyInfo;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in RouletteRoyalGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected string buildInitString(string strGlobalUserID, double balance, Currencies currency)
        {
            AmaticEncrypt encrypt = new AmaticEncrypt();
            string initString = string.Empty;

            RouletteInitPacket initPacket = new RouletteInitPacket(InitString);
            initPacket.betbuttons   = BettingButton.ToList();
            initPacket.betlimits    = BetLimints.ToList();
            initPacket.maxbetamount = BetLimints.Last();
            initPacket.win          = 0;

            if (_dicUserBetInfos.ContainsKey(strGlobalUserID) && _dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                RouletteRoyalBetInfo betInfo    = _dicUserBetInfos[strGlobalUserID];
                RouletteRoyalResult spinResult  = _dicUserResultInfos[strGlobalUserID];

                RoulettePacket amaPacket = new RoulettePacket(spinResult.ResultString);
                initPacket.messagetype  = amaPacket.messagetype;
                initPacket.winnumber    = amaPacket.winnumber;
                foreach(int winnum in spinResult.WinNumberLogs)
                {
                    initPacket.winnumberlogs.Add(winnum);
                    initPacket.winnumberlogs.RemoveAt(0);
                }
            }

            initString = encrypt.WriteDecHex(initString, initPacket.messageheader);
            initString = encrypt.WriteDec2Hex(initString, initPacket.messagetype);
            initString = encrypt.WriteDecHex(initString, initPacket.sessionclose);
            initString = encrypt.WriteLengthAndDec(initString, initPacket.messageid);

            double pointUnit = getPointUnit(new RouletteRoyalBetInfo() { CurrencyInfo = currency });
            long balanceUnit = (long)Math.Round(balance / pointUnit, 0);
            initString = encrypt.WriteLengthAndDec(initString, balanceUnit);                //현재 화페와 단위금액으로 변환된 발란스
            initString = encrypt.WriteLengthAndDec(initString, initPacket.win);             //당첨금
            initString = encrypt.WriteLengthAndDec(initString, initPacket.winnumber);       //당첨금
            initString = encrypt.WriteLengthAndDec(initString, initPacket.unknownparam1);   //0
            for (int i = 0; i < initPacket.betbuttons.Count; i++)
            {
                initString = encrypt.WriteLengthAndDec(initString, initPacket.betbuttons[i]);
            }
            initString = encrypt.WriteLengthAndDec(initString, initPacket.maxbetamount);
            for (int i = 0; i < initPacket.betlimits.Count; i++)
            {
                initString = encrypt.WriteLengthAndDec(initString, initPacket.betlimits[i]);
            }
            for (int i = 0; i < initPacket.gamblelogs.Count; i++)
            {
                initString = encrypt.WriteLengthAndDec(initString, initPacket.gamblelogs[i]);
            }
            for (int i = 0; i < initPacket.winnumberlogs.Count; i++)
            {
                initString = encrypt.WriteLengthAndDec(initString, initPacket.winnumberlogs[i]);
            }
            return initString;
        }
        protected string buildResMsgString(RouletteRoyalBetInfo betInfo, string strGlobalUserID, double balance,int winNumber, long winMoney, AmaticMessageType type)
        {
            RoulettePacket packet = new RoulettePacket();
            double pointUnit = getPointUnit(betInfo);
            packet.messageheader    = 1;
            packet.messagetype      = (int)type;
            packet.sessionclose     = 0;
            packet.messageid        = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            packet.balance          = (long)Math.Round(balance / pointUnit, 0);
            packet.win              = winMoney;
            packet.winnumber        = winNumber;

            if (type == AmaticMessageType.HeartBeat && _dicUserResultInfos.ContainsKey(strGlobalUserID))
            {
                BaseAmaticSlotSpinResult spinResult = _dicUserResultInfos[strGlobalUserID];

                RoulettePacket oldPacket = new RoulettePacket(spinResult.ResultString);
                packet.winnumber    = oldPacket.winnumber;
            }
            return buildSpinString(packet);
        }
        protected string buildSpinString(RoulettePacket packet)
        {
            AmaticEncrypt encrypt = new AmaticEncrypt();
            string newSpinString = string.Empty;

            newSpinString = encrypt.WriteDecHex(newSpinString, packet.messageheader);
            newSpinString = encrypt.WriteDec2Hex(newSpinString, packet.messagetype);
            newSpinString = encrypt.WriteDecHex(newSpinString, packet.sessionclose);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.messageid);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.balance);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.win);
            newSpinString = encrypt.WriteLengthAndDec(newSpinString, packet.winnumber);
            newSpinString = string.Format("{0}-", newSpinString);
            return newSpinString;
        }
        private double getPointUnit(RouletteRoyalBetInfo betInfo)
        {
            double pointUnit = DicCurrencyInfo.Instance._currencyInfo[betInfo.CurrencyInfo].Rate / 100.0;
            return pointUnit;
        }

        protected byte[] backupBetInfo(RouletteRoyalBetInfo betInfo)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    betInfo.SerializeTo(writer);
                }
                return ms.ToArray();
            }
        }
        protected override async Task<bool> loadUserHistoricalData(string strUserID, bool isNewEnter)
        {
            try
            {
                string strKey = string.Format("{0}_{1}", strUserID, _gameID);
                byte[] betInfoData = await RedisDatabase.RedisCache.StringGetAsync(strKey);
                if (betInfoData != null)
                {
                    using (var stream = new MemoryStream(betInfoData))
                    {
                        BinaryReader reader = new BinaryReader(stream);
                        RouletteRoyalBetInfo betInfo = restoreBetInfo(strUserID, reader);
                        if (betInfo != null)
                            _dicUserBetInfos[strUserID] = betInfo;
                    }
                }

                strKey = string.Format("{0}_{1}_result", strUserID, _gameID);
                byte[] resultInfoData = await RedisDatabase.RedisCache.StringGetAsync(strKey);
                if (resultInfoData != null)
                {
                    using (var stream = new MemoryStream(resultInfoData))
                    {
                        BinaryReader reader = new BinaryReader(stream);
                        RouletteRoyalResult resultInfo = restoreResultInfo(strUserID, reader);
                        if (resultInfo != null)
                            _dicUserResultInfos[strUserID] = resultInfo;
                    }
                }
            }

            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseAmaticSlotGame::loadUserHistoricalData {0}", ex);
                return false;
            }
            return await base.loadUserHistoricalData(strUserID, isNewEnter);
        }
        protected RouletteRoyalBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            RouletteRoyalBetInfo betInfo = new RouletteRoyalBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
        protected RouletteRoyalResult restoreResultInfo(string strUserID, BinaryReader reader)
        {
            RouletteRoyalResult result = new RouletteRoyalResult();
            result.SerializeFrom(reader);
            return result;
        }
        protected void saveBetResultInfo(string strUserID)
        {
            try
            {
                if (_dicUserBetInfos.ContainsKey(strUserID))
                {
                    byte[] betInfoBytes = _dicUserBetInfos[strUserID].convertToByte();
                    _redisWriter.Tell(new UserBetInfoWrite(strUserID, _gameID, betInfoBytes, false), Self);
                }
                else
                {
                    _redisWriter.Tell(new UserBetInfoWrite(strUserID, _gameID, null, false), Self);
                }
                if (_dicUserResultInfos.ContainsKey(strUserID))
                {
                    byte[] resultInfoBytes = _dicUserResultInfos[strUserID].convertToByte();
                    _redisWriter.Tell(new UserResultInfoWrite(strUserID, _gameID, resultInfoBytes, false), Self);
                }
                else
                {
                    _redisWriter.Tell(new UserResultInfoWrite(strUserID, _gameID, null, false), Self);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in RouletteRoyalGameLogic::saveBetInfo {0}", ex);
            }
        }
    }

    public class RouletteStrAndMultiple
    {
        public List<string> KeyStr      { get; set; }
        public int          Multiple    { get; set; }

        public RouletteStrAndMultiple(List<string> keystr, int multiple)
        {
            this.KeyStr     = keystr;
            this.Multiple   = multiple;
        }
    }
}
