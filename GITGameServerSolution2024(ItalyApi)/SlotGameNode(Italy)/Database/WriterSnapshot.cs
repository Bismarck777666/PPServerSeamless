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

        private static WriterSnapshot   _sInstance  = new WriterSnapshot();
        public static WriterSnapshot    Instance    => _sInstance;

        private List<PPGameHistoryDBItem>           _ppGameHistoryItems         = new List<PPGameHistoryDBItem>();
        private List<PPGameRecentHistoryDBItem>     _ppRecentHistoryItems       = new List<PPGameRecentHistoryDBItem>();
        private List<BNGHistoryItem>                _bngGameHistoryItems        = new List<BNGHistoryItem>();
        private List<CQ9GameLogItem>                _cq9GameHistoryItems        = new List<CQ9GameLogItem>();
        private List<HabaneroLogItem>               _habaneroGameHistoryItems   = new List<HabaneroLogItem>();
        private List<PlaysonHistoryItem>            _playsonGameHistoryItems    = new List<PlaysonHistoryItem>();

        public void insertBNGGameHistory(BNGHistoryItem historyItem)
        {
            _bngGameHistoryItems.Add(historyItem);
        }
        
        public void PushBNGGameHistoryItems(List<BNGHistoryItem> historyItems)
        {
            _bngGameHistoryItems.InsertRange(0, historyItems);
        }
        
        public List<BNGHistoryItem> PopBNGGameHistoryItems(int count = 1000)
        {
            List<BNGHistoryItem> items = new List<BNGHistoryItem>();

            if (_bngGameHistoryItems.Count < count)
                count = _bngGameHistoryItems.Count;

            if (count == 0)
                return null;

            items.AddRange(_bngGameHistoryItems.GetRange(0, count));
            _bngGameHistoryItems.RemoveRange(0, items.Count);
            return items;
        }

        public void insertCQ9GameHistory(CQ9GameLogItem historyItem)
        {
            _cq9GameHistoryItems.Add(historyItem);
        }
        
        public void PushCQ9GameHistoryItems(List<CQ9GameLogItem> historyItems)
        {
            _cq9GameHistoryItems.InsertRange(0, historyItems);
        }
        
        public List<CQ9GameLogItem> PopCQ9GameHistoryItems(int count = 1000)
        {
            List<CQ9GameLogItem> items = new List<CQ9GameLogItem>();
            if (_cq9GameHistoryItems.Count < count)
                count = _cq9GameHistoryItems.Count;
            if (count == 0)
                return null;
            items.AddRange(_cq9GameHistoryItems.GetRange(0, count));
            _cq9GameHistoryItems.RemoveRange(0, items.Count);
            return items;
        }
        
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

        public void insertHabaneroGameHistory(HabaneroLogItem historyItem)
        {
            _habaneroGameHistoryItems.Add(historyItem);
        }
        
        public void PushHabaneroGameHistoryItems(List<HabaneroLogItem> historyItems)
        {
            _habaneroGameHistoryItems.InsertRange(0, historyItems);
        }
        
        public List<HabaneroLogItem> PopHabaneroGameHistoryItems(int count = 1000)
        {
            List<HabaneroLogItem> items = new List<HabaneroLogItem>();
            if (_habaneroGameHistoryItems.Count < count)
                count = _habaneroGameHistoryItems.Count;
            if (count == 0)
                return null;
            items.AddRange(_habaneroGameHistoryItems.GetRange(0, count));
            _habaneroGameHistoryItems.RemoveRange(0, items.Count);
            return items;
        }


        public void insertPlaysonGameHistory(PlaysonHistoryItem historyItem)
        {
            _playsonGameHistoryItems.Add(historyItem);
        }
        public void PushPlaysonGameHistoryItems(List<PlaysonHistoryItem> historyItems)
        {
            _playsonGameHistoryItems.InsertRange(0, historyItems);
        }
        public List<PlaysonHistoryItem> PopPlaysonGameHistoryItems(int count = 1000)
        {
            List<PlaysonHistoryItem> items = new List<PlaysonHistoryItem>();

            if (_playsonGameHistoryItems.Count < count)
                count = _playsonGameHistoryItems.Count;

            if (count == 0)
                return null;

            items.AddRange(_playsonGameHistoryItems.GetRange(0, count));
            _playsonGameHistoryItems.RemoveRange(0, items.Count);
            return items;
        }

    }

    public class PPGameRecentHistoryDBItem
    {
        public int      AgentID     { get; set; }
        public string   UserName    { get; set; }
        public int      GameID      { get; set; }
        public double   Balance     { get; set; }
        public double   Bet         { get; set; }
        public double   Win         { get; set; }
        public long     DateTime    { get; set; }
        public string   Hash        { get; set; }
        public string   RoundID     { get; set; }
        public string   DetailLog   { get; set; }

        public PPGameRecentHistoryDBItem(int agentID, string strUserName, int gameID, double balance, double bet, double win, string strHash, string roundid, string strDetailLog, long dateTime)
        {
            this.AgentID        = agentID;
            this.UserName       = strUserName;
            this.GameID         = gameID;
            this.Balance        = balance;
            this.Bet            = bet;
            this.Win            = win;
            this.Hash           = strHash;
            this.RoundID        = roundid;
            this.DetailLog      = strDetailLog;
            this.DateTime       = dateTime;
        }
    }

    public class PPGameHistoryDBItem
    {
        public int      AgentID         { get; set; }
        public string   UserName        { get; set; }
        public int      GameID          { get; set; }
        public double   BaseBet         { get; set; }
        public double   Bet             { get; set; }
        public double   Win             { get; set; }
        public double   RTP             { get; set; }
        public string   RoundID         { get; set; }
        public string   DetailLog       { get; set; }
        public long     PlayedDate      { get; set; }

       public PPGameHistoryDBItem(int agentID, string strUserName, int gameID, double bet, double baseBet, double win, double rtp, string roundid, string strDetailLog, long playedDate)
       {
            this.AgentID        = agentID;
            this.UserName       = strUserName;
            this.GameID         = gameID;
            this.Bet            = bet;
            this.RoundID        = roundid;
            this.BaseBet        = baseBet;
            this.Win            = win;
            this.RTP            = rtp;
            this.DetailLog      = strDetailLog;
            this.PlayedDate     = playedDate;
       }
    }

    public class BNGHistoryItem
    {
        public int      AgentID         { get; set; }
        public string   UserID          { get; set; }
        public int      GameID          { get; set; }
        public DateTime Time            { get; set; }
        public string   RoundID         { get; set; }
        public string   TransactionID   { get; set; }
        public double   Bet             { get; set; }
        public double   Win             { get; set; }
        public string   Overview        { get; set; }
        public string   Detail          { get; set; }
    }

    public class BNGHistoryItemOverview
    {
        public string   balance_before      { get; set; }
        public string   balance_after       { get; set; }
        public string   bet                 { get; set; }
        public string   brand               { get; set; }
        public string   c_at                { get; set; }
        public string   currency            { get; set; }
        public string   exceed_code         { get; set; }
        public string   exceed_message      { get; set; }
        public string   game_id             { get; set; }
        public string   game_name           { get; set; }
        public bool     is_bonus            { get; set; }
        public string   mode                { get; set; }
        public string   outcome             { get; set; }
        public string   player_id           { get; set; }
        public string   profit              { get; set; }
        public bool     round_finished      { get; set; }
        public string   round_id            { get; set; }
        public bool     round_started       { get; set; }
        public string   status              { get; set; }
        public string   tag                 { get; set; }
        public string   transaction_id      { get; set; }
        public string   type                { get; set; }
        public string   win                 { get; set; }
    }

    public class PlaysonHistoryItem
    {
        public int      AgentID         { get; set; }
        public string   UserID          { get; set; }
        public int      GameID          { get; set; }
        public DateTime Time            { get; set; }
        public string   RoundID         { get; set; }
        public string   TransactionID   { get; set; }
        public double   Bet             { get; set; }
        public double   Win             { get; set; }
        public string   Overview        { get; set; }
        public string   Detail          { get; set; }
    }
    
    public class PlaysonHistoryItemOverview
    {
        public string   balance_before  { get; set; }
        public string   balance_after   { get; set; }
        public string   bet             { get; set; }
        public string   brand           { get; set; }
        public string   c_at            { get; set; }
        public string   currency        { get; set; }
        public string   exceed_code     { get; set; }
        public string   exceed_message  { get; set; }
        public string   game_id         { get; set; }
        public string   game_name       { get; set; }
        public bool     is_bonus        { get; set; }
        public string   mode            { get; set; }
        public string   outcome         { get; set; }
        public string   player_id       { get; set; }
        public string   profit          { get; set; }
        public bool     round_finished  { get; set; }
        public string   round_id        { get; set; }
        public bool     round_started   { get; set; }
        public string   status          { get; set; }
        public string   tag             { get; set; }
        public string   transaction_id  { get; set; }
        public string   type            { get; set; }
        public string   win             { get; set; }
    }
}
