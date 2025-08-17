using System;
using System.Collections.Generic;

namespace QueenApiNode.Database
{
    public class WriterSnapshot
    {
        private static WriterSnapshot       _instance               = new WriterSnapshot();
        private Dictionary<int, double>     _agentScoreUpdates      = new Dictionary<int, double>();
        private List<AgentMoneyChangeItem>  _agentMoneyChangeItems  = new List<AgentMoneyChangeItem>();
        private List<UserMoneyChangeItem>   _userMoneyChangeItems   = new List<UserMoneyChangeItem>();

        public static WriterSnapshot Instance => _instance;

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

            List<AgentScoreUpdateItem> agentScoreUpdateItemList = new List<AgentScoreUpdateItem>();
            foreach (KeyValuePair<int, double> pair in _agentScoreUpdates)
            {
                agentScoreUpdateItemList.Add(new AgentScoreUpdateItem(pair.Key, pair.Value));
                if (agentScoreUpdateItemList.Count >= count)
                    break;
            }

            for (int i = 0; i < agentScoreUpdateItemList.Count; i++)
                _agentScoreUpdates.Remove(agentScoreUpdateItemList[i].DBID);
            
            return agentScoreUpdateItemList;
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
            List<AgentMoneyChangeItem> agentMoneyChangeItemList = new List<AgentMoneyChangeItem>();
            if (_agentMoneyChangeItems.Count == 0)
                return agentMoneyChangeItemList;

            foreach (AgentMoneyChangeItem agentMoneyChangeItem in _agentMoneyChangeItems)
            {
                agentMoneyChangeItemList.Add(agentMoneyChangeItem);
                if (agentMoneyChangeItemList.Count >= count)
                    break;
            }
            
            _agentMoneyChangeItems.RemoveRange(0, agentMoneyChangeItemList.Count);
            return agentMoneyChangeItemList;
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
            List<UserMoneyChangeItem> userMoneyChangeItemList = new List<UserMoneyChangeItem>();
            if (_userMoneyChangeItems.Count == 0)
                return userMoneyChangeItemList;
            
            foreach (UserMoneyChangeItem userMoneyChangeItem in _userMoneyChangeItems)
            {
                userMoneyChangeItemList.Add(userMoneyChangeItem);
                if (userMoneyChangeItemList.Count >= count)
                    break;
            }
            
            _userMoneyChangeItems.RemoveRange(0, userMoneyChangeItemList.Count);
            return userMoneyChangeItemList;
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
            DBID    = dbID;
            Score   = score;
        }
    }
    public enum AgentMoneyChangeModes
    {
        UPDEPOSIT       = 0,
        UPWITHDRAW      = 1,
        USERDEPOSIT     = 2,
        USERWITHDRAW    = 3,
        EVENTSUBTRACT   = 4,
        EVENTADDLEFT    = 5,
    }
    public class AgentMoneyChangeItem
    {
        public string                   SubjectAccount  { get; private set; }
        public string                   OtherAccount    { get; private set; }
        public double                   Money           { get; private set; }
        public double                   BeforeMoney     { get; private set; }
        public double                   AfterMoney      { get; private set; }
        public AgentMoneyChangeModes    Mode            { get; private set; }
        public DateTime                 ProcTime        { get; private set; }

        public AgentMoneyChangeItem(string subjectAccount,string otherAccount,double money,double beforeMoney,double afterMoney,AgentMoneyChangeModes mode,DateTime procTime)
        {
            SubjectAccount  = subjectAccount;
            OtherAccount    = otherAccount;
            Money           = money;
            BeforeMoney     = beforeMoney;
            AfterMoney      = afterMoney;
            Mode            = mode;
            ProcTime        = procTime;
        }
    }
    public class UserMoneyChangeItem
    {
        public string               AgentID         { get; private set; }
        public string               UserID          { get; private set; }
        public double               Amount          { get; private set; }
        public UserMoneyChangeModes Mode            { get; private set; }
        public double               BeforeMoney     { get; private set; }
        public double               AfterMoney      { get; private set; }
        public DateTime             UpdateTime      { get; private set; }

        public UserMoneyChangeItem(string agentID,string userID,double amount,UserMoneyChangeModes mode,double beforeMoney,double afterMoney,DateTime updateTime)        
        {
            AgentID     = agentID;
            UserID      = userID;
            Amount      = amount;
            Mode        = mode;
            BeforeMoney = beforeMoney;
            AfterMoney  = afterMoney;
            UpdateTime  = updateTime;
        }
    }
    public enum UserMoneyChangeModes
    {
        DEPOSIT     = 0,
        WITHDRAW    = 1,
    }
}
