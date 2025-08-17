using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueenApiNode.Database
{
    public class WriterSnapshot
    {
        private static WriterSnapshot _instance = new WriterSnapshot();
        public static WriterSnapshot Instance
        {
            get
            {
                return _instance;
            }
        }
        private Dictionary<int, double>         _agentScoreUpdates      = new Dictionary<int, double>();
        private List<AgentMoneyChangeItem>      _agentMoneyChangeItems  = new List<AgentMoneyChangeItem>();
        private List<UserMoneyChangeItem>       _userMoneyChangeItems   = new List<UserMoneyChangeItem>();

        public void updateAgentScore(AgentScoreUpdateItem update)
        {
            if (!_agentScoreUpdates.ContainsKey(update.DBID))
                _agentScoreUpdates.Add(update.DBID, update.Score);
            else
                _agentScoreUpdates[update.DBID] += update.Score;
        }
        public List<AgentScoreUpdateItem> popAgentScoreUpdates(int count = 5000)
        {
            if (_agentScoreUpdates.Count == 0)
                return null;

            List<AgentScoreUpdateItem> agentScoreUpdateItems = new List<AgentScoreUpdateItem>();
            foreach (KeyValuePair<int, double> pair in _agentScoreUpdates)
            {
                agentScoreUpdateItems.Add(new AgentScoreUpdateItem(pair.Key, pair.Value));
                if (agentScoreUpdateItems.Count >= count)
                    break;
            }
            for (int i = 0; i < agentScoreUpdateItems.Count; i++)
                _agentScoreUpdates.Remove(agentScoreUpdateItems[i].DBID);

            return agentScoreUpdateItems;
        }
        public void pushAgentScoreUpdates(List<AgentScoreUpdateItem> agentScoreUpdateItems)
        {
            for (int i = 0; i < agentScoreUpdateItems.Count; i++)
            {
                if (_agentScoreUpdates.ContainsKey(agentScoreUpdateItems[i].DBID))
                    _agentScoreUpdates[agentScoreUpdateItems[i].DBID] += agentScoreUpdateItems[i].Score;
                else
                    _agentScoreUpdates[agentScoreUpdateItems[i].DBID] = agentScoreUpdateItems[i].Score;
            }
        }
        public void insertAgentMoneyChangeItem(AgentMoneyChangeItem item)
        {
            _agentMoneyChangeItems.Add(item);
        }
        public List<AgentMoneyChangeItem> popAgentMoneyChangeItems(int count = 1000)
        {
            List<AgentMoneyChangeItem> agentMoneyChangeItems = new List<AgentMoneyChangeItem>();
            if (_agentMoneyChangeItems.Count == 0)
                return agentMoneyChangeItems;

            foreach (AgentMoneyChangeItem item in _agentMoneyChangeItems)
            {
                agentMoneyChangeItems.Add(item);
                if (agentMoneyChangeItems.Count >= count)
                    break;
            }
            _agentMoneyChangeItems.RemoveRange(0, agentMoneyChangeItems.Count);
            return agentMoneyChangeItems;
        }
        public void pushAgentMoneyChangeItems(List<AgentMoneyChangeItem> items)
        {
            _agentMoneyChangeItems.InsertRange(0, items);
        }
        public void insertUserMoneyChangeItem(UserMoneyChangeItem item)
        {
            _userMoneyChangeItems.Add(item);
        }
        public List<UserMoneyChangeItem> popUserMoneyChangeItems(int count = 1000)
        {
            List<UserMoneyChangeItem> userMoneyChangeItems = new List<UserMoneyChangeItem>();
            if (_userMoneyChangeItems.Count == 0)
                return userMoneyChangeItems;

            foreach (UserMoneyChangeItem item in _userMoneyChangeItems)
            {
                userMoneyChangeItems.Add(item);
                if (userMoneyChangeItems.Count >= count)
                    break;
            }
            _userMoneyChangeItems.RemoveRange(0, userMoneyChangeItems.Count);
            return userMoneyChangeItems;
        }
        public void pushUserMoneyChangeItems(List<UserMoneyChangeItem> items)
        {
            _userMoneyChangeItems.InsertRange(0, items);
        }
    }
    public class AgentScoreUpdateItem
    {
        public int      DBID    { get; private set; }
        public double   Score   { get; private set; }
        public AgentScoreUpdateItem(int dbID, double score)
        {
            this.DBID = dbID;
            this.Score = score;
        }
    }
    public enum AgentMoneyChangeModes
    {
        UPDEPOSIT       = 0,
        UPWITHDRAW      = 1,
        USERDEPOSIT     = 2,
        USERWITHDRAW    = 3,
    }
    public class AgentMoneyChangeItem
    {
        public string SubjectAccount        { get; private set; }
        public string OtherAccount          { get; private set; }
        public double Money                 { get; private set; }
        public double BeforeMoney           { get; private set; }
        public double AfterMoney            { get; private set; }
        public AgentMoneyChangeModes Mode   { get; private set; }
        public DateTime ProcTime            { get; private set; }

        public AgentMoneyChangeItem(string subjectAccount, string otherAccount, double money, double beforeMoney, double afterMoney, AgentMoneyChangeModes mode, DateTime procTime)
        {
            this.SubjectAccount = subjectAccount;
            this.OtherAccount   = otherAccount;
            this.Money          = money;
            this.BeforeMoney    = beforeMoney;
            this.AfterMoney     = afterMoney;
            this.Mode           = mode;
            this.ProcTime       = procTime;
        }
    }
    public enum UserMoneyChangeModes
    {
        DEPOSIT     = 0,
        WITHDRAW    = 1,
    }
    public class UserMoneyChangeItem
    {
        public string               AgentID     { get; private set; }
        public string               UserID      { get; private set; }
        public double               Amount      { get; private set; }
        public UserMoneyChangeModes Mode        { get; private set; }
        public double               BeforeMoney { get; private set; }
        public double               AfterMoney  { get; private set; }
        public string               IpAddress   { get; private set; }
        public DateTime             UpdateTime  { get; private set; }

        public UserMoneyChangeItem(string agentID, string userID, double amount, UserMoneyChangeModes mode, double beforeMoney, double afterMoney, string ipAddress, DateTime updateTime)
        {
            AgentID         = agentID;
            UserID          = userID;
            Amount          = amount;
            Mode            = mode;
            BeforeMoney     = beforeMoney;
            AfterMoney      = afterMoney;
            IpAddress       = ipAddress;
            UpdateTime      = updateTime;
        }
    }
}
