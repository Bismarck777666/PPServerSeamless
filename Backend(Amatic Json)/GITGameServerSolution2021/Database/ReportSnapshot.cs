using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommNode.Database
{
    public class ReportSnapshot
    {
        private Dictionary<string, ReportUpdateItem>    _dicReportUpdateItems       = new Dictionary<string, ReportUpdateItem>();
        private Dictionary<long, UserRollingAdded>      _dicUserRollingAdds         = new Dictionary<long, UserRollingAdded>();
        private Dictionary<int, AgentRollingAdded>      _dicAgentRollingAdds        = new Dictionary<int, AgentRollingAdded>();
        private Dictionary<string, GameReportItem>      _dicGameReports             = new Dictionary<string, GameReportItem>();

        private static ReportSnapshot _sInstance = new ReportSnapshot();
        public static ReportSnapshot Instance
        {
            get
            {
                return _sInstance;
            }
        }

        public void updateGameReport(GameReportItem reportItem)
        {
            string strKey = string.Format("{0}_{1}", reportItem.GameID, reportItem.ReportDate.ToString("yyyyMMddHHmm"));
            if (_dicGameReports.ContainsKey(strKey))
                _dicGameReports[strKey].mergeReport(reportItem);
            else
                _dicGameReports[strKey] = reportItem;

        }

        public List<GameReportItem> PopGameReportUpdates(int maxCount = 5000)
        {
            if (_dicGameReports.Count == 0)
                return null;
            
            List<GameReportItem> reportUpdates = new List<GameReportItem>();
            foreach (KeyValuePair<string, GameReportItem> pair in _dicGameReports)
            {
                reportUpdates.Add(pair.Value);
                if (reportUpdates.Count >= maxCount)
                    break;
            }
            for (int i = 0; i < reportUpdates.Count; i++)
            {
                string strKey = string.Format("{0}_{1}", reportUpdates[i].GameID, reportUpdates[i].ReportDate.ToString("yyyyMMddHHmm"));
                _dicGameReports.Remove(strKey);
            }
            return reportUpdates;
        }
        public void PushGameReportUpdates(List<GameReportItem> reportUpdateItems)
        {
            for (int i = 0; i < reportUpdateItems.Count; i++)
            {
                string strKey = string.Format("{0}_{1}", reportUpdateItems[i].GameID, reportUpdateItems[i].ReportDate.ToString("yyyyMMddHHmm"));
                if (_dicGameReports.ContainsKey(strKey))
                    _dicGameReports[strKey].mergeReport(reportUpdateItems[i]);
                else
                    _dicGameReports[strKey] = reportUpdateItems[i];
            }
        }
        public void updateReports(ReportUpdateItem reportUpdateItem)
        {
            string strKey = string.Format("{0}_{1}", reportUpdateItem.UserID, reportUpdateItem.ReportDateTime.ToString("yyyyMMddHHmm"));
            if (_dicReportUpdateItems.ContainsKey(strKey))
                _dicReportUpdateItems[strKey].mergeReport(reportUpdateItem);
            else
                _dicReportUpdateItems[strKey] = reportUpdateItem;
        }

        public List<ReportUpdateItem> PopReportUpdates(int maxCount = 5000)
        {
            if (_dicReportUpdateItems.Count == 0)
                return null;

            List<ReportUpdateItem> reportUpdates = new List<ReportUpdateItem>();
            foreach (KeyValuePair<string, ReportUpdateItem> pair in _dicReportUpdateItems)
            {                
                reportUpdates.Add(pair.Value);
                if (reportUpdates.Count >= maxCount)
                    break;
            }
            for (int i = 0; i < reportUpdates.Count; i++)
            {
                string strKey = string.Format("{0}_{1}", reportUpdates[i].UserID, reportUpdates[i].ReportDateTime.ToString("yyyyMMddHHmm"));
                _dicReportUpdateItems.Remove(strKey);
            }
            return reportUpdates;
        }
        public void PushReportUpdateItems(List<ReportUpdateItem> reportUpdateItems)
        {
            for (int i = 0; i < reportUpdateItems.Count; i++)
            {
                string strKey = string.Format("{0}_{1}", reportUpdateItems[i].UserID, reportUpdateItems[i].ReportDateTime.ToString("yyyyMMddHHmm"));
                if (_dicReportUpdateItems.ContainsKey(strKey))
                    _dicReportUpdateItems[strKey].mergeReport(reportUpdateItems[i]);
                else
                    _dicReportUpdateItems[strKey] = reportUpdateItems[i];
            }
        }

        public void addUserRolling(UserRollingAdded userRollingAdd)
        {
            if (_dicUserRollingAdds.ContainsKey(userRollingAdd.UserDBID))
                _dicUserRollingAdds[userRollingAdd.UserDBID].merge(userRollingAdd);
            else
                _dicUserRollingAdds[userRollingAdd.UserDBID] = userRollingAdd;
        }

        public List<UserRollingAdded> PopUserRollingAdds(int count = 5000)
        {
            if (_dicUserRollingAdds.Count == 0)
                return null;

            List<UserRollingAdded> userRollingAdds = new List<UserRollingAdded>();
            foreach (KeyValuePair<long, UserRollingAdded> pair in _dicUserRollingAdds)
            {
                userRollingAdds.Add(pair.Value);
                if (userRollingAdds.Count >= count)
                    break;
            }
            for (int i = 0; i < userRollingAdds.Count; i++)
                _dicUserRollingAdds.Remove(userRollingAdds[i].UserDBID);

            return userRollingAdds;
        }

        public void PushUserRollingAdds(List<UserRollingAdded> userRollingAdds)
        {
            for (int i = 0; i < userRollingAdds.Count; i++)
            {
                long userDBID = userRollingAdds[i].UserDBID;
                if (_dicUserRollingAdds.ContainsKey(userDBID))
                    _dicUserRollingAdds[userDBID].merge(userRollingAdds[i]);
                else
                    _dicUserRollingAdds[userDBID] = userRollingAdds[i];
            }
        }

        public void addAgentRolling(AgentRollingAdded agentRollingAdd)
        {
            if (_dicAgentRollingAdds.ContainsKey(agentRollingAdd.AgentDBID))
                _dicAgentRollingAdds[agentRollingAdd.AgentDBID].merge(agentRollingAdd);
            else
                _dicAgentRollingAdds[agentRollingAdd.AgentDBID] = agentRollingAdd;
        }

        public List<AgentRollingAdded> PopAgentRollingAdds(int count = 5000)
        {
            if (_dicAgentRollingAdds.Count == 0)
                return null;

            List<AgentRollingAdded> agentRollingAdds = new List<AgentRollingAdded>();
            foreach (KeyValuePair<int, AgentRollingAdded> pair in _dicAgentRollingAdds)
            {
                agentRollingAdds.Add(pair.Value);
                if (agentRollingAdds.Count >= count)
                    break;
            }
            for (int i = 0; i < agentRollingAdds.Count; i++)
                _dicAgentRollingAdds.Remove(agentRollingAdds[i].AgentDBID);

            return agentRollingAdds;
        }

        public void PushAgentRollingAdds(List<AgentRollingAdded> agentRollingAdds)
        {
            for (int i = 0; i < agentRollingAdds.Count; i++)
            {
                int agentDBID = agentRollingAdds[i].AgentDBID;
                if (_dicAgentRollingAdds.ContainsKey(agentDBID))
                    _dicAgentRollingAdds[agentDBID].merge(agentRollingAdds[i]);
                else
                    _dicAgentRollingAdds[agentDBID] = agentRollingAdds[i];
            }
        }


    }

    public class UserRollingAdded
    {
        public long     UserDBID    { get; private set; }
        public double   RollPoint   { get; private set; }

        public UserRollingAdded(long userDBID, double rollPoint)
        {
            this.UserDBID  = userDBID;
            this.RollPoint = rollPoint;
        }

        public void merge(UserRollingAdded userRolling)
        {
            this.RollPoint += userRolling.RollPoint;
        }
    }
    public class AgentRollingAdded
    {
        public int      AgentDBID   { get; private set; }        
        public double   RollPoint   { get; private set; }

        public AgentRollingAdded(int agentDBID, double rollPoint)
        {
            this.AgentDBID = agentDBID;
            this.RollPoint = rollPoint;
        }

        public void merge(AgentRollingAdded agentRolling)
        {
            this.RollPoint += agentRolling.RollPoint;
        }
    }

    public class GameReportItem
    {
        public int      GameID      { get; private set; }
        public int      AgentID     { get; private set; }
        public double   Turnover    { get; private set; }
        public DateTime ReportDate  { get; private set; }
        public GameReportItem(int gameID, int agentID, double turnover, DateTime reportDate)
        {
            this.GameID     = gameID;
            this.AgentID    = agentID;
            this.Turnover   = turnover;
            this.ReportDate = reportDate;
        }

        public void mergeReport(GameReportItem item)
        {
            this.Turnover += item.Turnover;
        }
    }

    public class ReportUpdateItem
    {
        public ReportUpdateItem(string strUserID, int agentID, DateTime reportTime, double bet, double win, double turnover)
        {
            this.UserID = strUserID;
            this.ReportDateTime = reportTime;
            this.Bet = bet;
            this.Win = win;
            this.Turnover = turnover;
            this.AgentID = agentID;
        }

        public void mergeReport(ReportUpdateItem other)
        {
            this.Bet += other.Bet;
            this.Win += other.Win;
            this.Turnover += other.Turnover;
        }

       
        public string UserID { get; private set; }
        public DateTime ReportDateTime { get; private set; }
        public double Bet { get; private set; }
        public double Win { get; private set; }
        public double Turnover { get; private set; }
        public int AgentID { get; private set; }
    }
    
}
