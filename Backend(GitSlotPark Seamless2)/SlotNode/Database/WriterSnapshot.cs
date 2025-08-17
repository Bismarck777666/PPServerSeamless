using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlotGamesNode.GameLogics;

namespace SlotGamesNode.Database
{
    public class WriterSnapshot
    {

        private static WriterSnapshot _sInstance = new WriterSnapshot();
        public static WriterSnapshot Instance
        {
            get
            {
                return _sInstance;
            }
        }

        private List<PPGameHistoryDBItem>           _ppGameHistoryItems     = new List<PPGameHistoryDBItem>();
        private List<PPGameRecentHistoryDBItem>     _ppRecentHistoryItems   = new List<PPGameRecentHistoryDBItem>();
        private UpdatePayoutPoolStatus              _updatePayoutPoolItem   = null;
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

        public void updatePayoutPoolStatus(UpdatePayoutPoolStatus item)
        {
            _updatePayoutPoolItem = item;
        }
        public UpdatePayoutPoolStatus PopUpdatePayoutPoolStatus()
        {
            if (_updatePayoutPoolItem == null)
                return null;

            UpdatePayoutPoolStatus item = _updatePayoutPoolItem;
            _updatePayoutPoolItem       = null;
            return item;
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
    }

    public class PPGameRecentHistoryDBItem
    {
        public int    AgentID   { get; set; }
        public string UserName  { get; set; }
        public int    GameID    { get; set; }
        public double Balance   { get; set; }
        public double Bet       { get; set; }
        public double Win       { get; set; }
        public long   DateTime  { get; set; }
        public string Hash      { get; set; }
        public string DetailLog { get; set; }
        public string Currency  { get; set; }
        public PPGameRecentHistoryDBItem(int agentID, string strUserName, int gameID, double balance, double bet, double win, string strHash, string strDetailLog, long dateTime, string currency)
        {
            this.AgentID        = agentID;
            this.UserName       = strUserName;
            this.GameID         = gameID;
            this.Balance        = balance;
            this.Bet            = bet;
            this.Win            = win;
            this.Hash           = strHash;
            this.DetailLog      = strDetailLog;
            this.DateTime       = dateTime;
            this.Currency       = currency;
        }
    }



    public class PPGameHistoryDBItem
    {
        public int    AgentID       { get; set; }
        public string UserName      { get; set; }
        public int    GameID        { get; set; }
        public double BaseBet       { get; set; }
        public double Bet           { get; set; }
        public double Win           { get; set; }
        public double RTP           { get; set; }
        public string DetailLog     { get; set; }
        public long   PlayedDate    { get; set; }
        public string Currency      { get; set; }
       public PPGameHistoryDBItem(int agentID, string strUserName, int gameID, double bet, double baseBet, double win, double rtp, string strDetailLog, long playedDate, string currency)
       {
            this.AgentID        = agentID;
            this.UserName       = strUserName;
            this.GameID         = gameID;
            this.Bet            = bet;
            this.BaseBet        = baseBet;
            this.Win            = win;
            this.RTP            = rtp;
            this.DetailLog      = strDetailLog;
            this.PlayedDate     = playedDate;
            this.Currency       = currency;
       }
    }
}
