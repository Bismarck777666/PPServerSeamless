using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using GITProtocol;
using SlotGamesNode.GameLogics;

namespace SlotGamesNode.Database
{
    public class WriterSnapshot
    {

        private static WriterSnapshot _sInstance = new WriterSnapshot();
        public static WriterSnapshot Instance => _sInstance;

        private List<PPGameHistoryDBItem> _ppGameHistoryItems = new List<PPGameHistoryDBItem>();
        private List<PPGameRecentHistoryDBItem> _ppRecentHistoryItems = new List<PPGameRecentHistoryDBItem>();
        private List<PPRaceWinnerDBItem> _ppRaceWinnerItems = new List<PPRaceWinnerDBItem>();
        private List<PPCashbackWinnerDBItem> _ppCashbackWinnerItems = new List<PPCashbackWinnerDBItem> { };
        private List<PPFreeSpinReportDBItem> _ppFreeSpinReports = new List<PPFreeSpinReportDBItem>();

        public void insertPPGameHistory(PPGameHistoryDBItem item)
        {
            _ppGameHistoryItems.Add(item);
        }
        public List<PPGameHistoryDBItem> PopPPGameHistoryItems(int count = 1000)
        {
            List<PPGameHistoryDBItem> items = new List<PPGameHistoryDBItem>();

            if (_ppGameHistoryItems.Count < count)
                count = _ppGameHistoryItems.Count;

            if (count == 0)
                return null;

            items.AddRange(_ppGameHistoryItems.GetRange(0, count));
            _ppGameHistoryItems.RemoveRange(0, items.Count);
            return items;
        }
        public void PushPPGameHistoryItems(List<PPGameHistoryDBItem> items)
        {
            _ppGameHistoryItems.InsertRange(0, items);
        }

        public void insertPPRecentGameHistory(PPGameRecentHistoryDBItem item)
        {
            _ppRecentHistoryItems.Add(item);
        }
        public List<PPGameRecentHistoryDBItem> PopPPRecentGameHistoryItems(int count = 1000)
        {
            List<PPGameRecentHistoryDBItem> items = new List<PPGameRecentHistoryDBItem>();

            if (_ppRecentHistoryItems.Count < count)
                count = _ppRecentHistoryItems.Count;

            if (count == 0)
                return null;

            items.AddRange(_ppRecentHistoryItems.GetRange(0, count));
            _ppRecentHistoryItems.RemoveRange(0, items.Count);
            return items;
        }
        public void PushPPRecentGameHistoryItems(List<PPGameRecentHistoryDBItem> items)
        {
            _ppRecentHistoryItems.InsertRange(0, items);
        }

        public void insertPPRaceWinners(PPRaceWinnerDBItem item)
        {
            _ppRaceWinnerItems.Add(item);
        }       
        
        public List<PPRaceWinnerDBItem> PopPPRaceWinnerItems(int count = 1000)
        {
            List<PPRaceWinnerDBItem> items = new List<PPRaceWinnerDBItem>();
            if (_ppRaceWinnerItems.Count < count)
                count = _ppRaceWinnerItems.Count;

            if (count == 0)
                return null;

            items.AddRange(_ppRaceWinnerItems.GetRange(0, count));
            _ppRaceWinnerItems.RemoveRange(0, items.Count);
            return items;
        }

        public void PushPPRaceWinners(List<PPRaceWinnerDBItem> items)
        {
            _ppRaceWinnerItems.InsertRange(0, items);
        }

        public void insertPPCashbackWinners(PPCashbackWinnerDBItem item)
        {
            _ppCashbackWinnerItems.Add(item);
        }
        public List<PPCashbackWinnerDBItem> PopPPCashbackWinnerItems(int count = 1000)
        {
            List<PPCashbackWinnerDBItem> items = new List<PPCashbackWinnerDBItem>();
            if (_ppCashbackWinnerItems.Count < count)
                count = _ppCashbackWinnerItems.Count;

            if (count == 0)
                return null;

            items.AddRange(_ppCashbackWinnerItems.GetRange(0, count));
            _ppCashbackWinnerItems.RemoveRange(0, items.Count);
            return items;
        }

        public void insertPPFreeSpinReport(PPFreeSpinReportDBItem item)
        {
            _ppFreeSpinReports.Add(item);
        }
        public List<PPFreeSpinReportDBItem> PopPPFreeSpinReports(int count = 1000)
        {
            List<PPFreeSpinReportDBItem> items = new List<PPFreeSpinReportDBItem>();
            if (_ppFreeSpinReports.Count < count)
                count = _ppFreeSpinReports.Count;

            if (count == 0)
                return null;

            items.AddRange(_ppFreeSpinReports.GetRange(0, count));
            _ppFreeSpinReports.RemoveRange(0, items.Count);
            return items;
        }
        public void PushPPFreeSpinReports(List<PPFreeSpinReportDBItem> items)
        {
            _ppFreeSpinReports.InsertRange(0, items);
        }
    }

    public class PPGameRecentHistoryDBItem
    {
        public string UserName { get; set; }
        public int GameID { get; set; }
        public double Balance { get; set; }
        public double Bet { get; set; }
        public double Win { get; set; }
        public long DateTime { get; set; }
        public string Hash { get; set; }

        public string DetailLog { get; set; }

        public PPGameRecentHistoryDBItem(string strUserName, int gameID, double balance, double bet, double win, string strHash, string strDetailLog, long dateTime)
        {
            this.UserName = strUserName;
            this.GameID = gameID;
            this.Balance = balance;
            this.Bet = bet;
            this.Win = win;
            this.Hash = strHash;
            this.DetailLog = strDetailLog;
            this.DateTime = dateTime;
        }
    }

    public class PPGameHistoryDBItem
    {
        public string UserName { get; set; }
        public int GameID { get; set; }
        public double BaseBet { get; set; }
        public double Bet { get; set; }
        public double Win { get; set; }
        public double RTP { get; set; }
        public string DetailLog { get; set; }
        public long PlayedDate { get; set; }

        public PPGameHistoryDBItem(string strUserName, int gameID, double bet, double baseBet, double win, double rtp, string strDetailLog, long playedDate)
        {
            this.UserName = strUserName;
            this.GameID = gameID;
            this.Bet = bet;
            this.BaseBet = baseBet;
            this.Win = win;
            this.RTP = rtp;
            this.DetailLog = strDetailLog;
            this.PlayedDate = playedDate;
        }
    }

    public class PPRaceWinnerDBItem
    {
        public int RaceID { get; set; }
        public int PrizeID { get; set; }
        public string AgentID { get; set; }
        public string UserName { get; set; }
        public int UserType { get; set; }
        public string Country { get; set; }
        public string Currency { get; set; }
        public double Bet { get; set; }
        public double Win { get; set; }
        public int Processed { get; set; }
        public string GameName { get; set; }
        public string PrizeType { get; set; }
        public int IsAgent { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime ProcessedTime { get; set; }

        public PPRaceWinnerDBItem(int raceID, int prizeID, string agentID, string userName, int userType, string country, string currency, double bet, double win, int processed, string gameName, string prizeType, int isAgent, DateTime updateTime, DateTime processedTime)
        {
            RaceID = raceID;
            PrizeID = prizeID;
            AgentID = agentID;
            UserName = userName;
            UserType = userType;
            Country = country;
            Currency = currency;
            Bet = bet;
            Win = win;
            Processed = processed;
            GameName = gameName;
            PrizeType = prizeType;
            IsAgent = isAgent;
            UpdateTime = updateTime;
            ProcessedTime = processedTime;
        }
    }

    public class PPCashbackWinnerDBItem
    {
        public int RaceID { get; set; }

        public string AgentID { get; set; }
        public string UserName { get; set; }        
        public string Country { get; set; }
        public string Currency { get; set; }
        public double Cashback { get; set; }  
        public string GameName { get; set; }
        public int IsAgent { get; set; }
        public int Period { get; set; }
        public string PeriodKey { get; set; }
        public DateTime UpdateTime { get; set; }


        public PPCashbackWinnerDBItem(int raceID, string agentID, string userName, string country, string currency, double cashback, string gameName, int isAgent, int period, string periodkey, DateTime updateTime)
        {
            RaceID = raceID;
            AgentID = agentID;
            UserName = userName;            
            Country = country;
            Currency = currency;
            Cashback = cashback;
            GameName = gameName;
            IsAgent = isAgent;
            Period = period;
            PeriodKey = periodkey;
            UpdateTime = updateTime;
        }
    }
    public class PPFreeSpinReportDBItem
    {
        public long fsID { get; private set; }
        public string AgentID { get; private set; }
        public string Username { get; private set; }
        public string Currency { get; private set; }
        public double Bet { get; private set; }
        public double Win { get; private set; }
        public int AwardedCount { get; private set; }
        public int RemainCount { get; private set; }
        public string Games { get; private set; }
        public DateTime Startdate { get; private set; }
        public DateTime Updatetime { get; private set; }
        public PPFreeSpinReportDBItem(long fsid, string agentid, string username, string currency, double bet, double win, int awardedcount, int remaincount, string games, DateTime startdate, DateTime updatetime)
        {
            this.fsID = fsid;
            this.AgentID = agentid;
            this.Username = username;
            this.Currency = currency;
            this.Bet = bet;
            this.Win = win;
            this.AwardedCount = awardedcount;
            this.RemainCount = remaincount;
            this.Games = games;
            this.Startdate = startdate;
            this.Updatetime = updatetime;
        }
    }

}
