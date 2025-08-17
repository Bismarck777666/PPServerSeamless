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
            string strKey = string.Format("{0}_{1}_{2}", reportItem.GameID, reportItem.Currency, reportItem.ReportDate.ToString("yyyyMMddHHmm"));
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
                string strKey = string.Format("{0}_{1}_{2}", reportUpdates[i].GameID, reportUpdates[i].Currency, reportUpdates[i].ReportDate.ToString("yyyyMMddHHmm"));
                _dicGameReports.Remove(strKey);
            }
            return reportUpdates;
        }
        public void PushGameReportUpdates(List<GameReportItem> reportUpdateItems)
        {
            for (int i = 0; i < reportUpdateItems.Count; i++)
            {
                string strKey = string.Format("{0}_{1}_{2}", reportUpdateItems[i].GameID, reportUpdateItems[i].Currency, reportUpdateItems[i].ReportDate.ToString("yyyyMMddHHmm"));
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
    }

    public class GameReportItem
    {
        public int      GameID      { get; private set; }
        public int      Currency    { get; private set; }
        public double   Turnover    { get; private set; }
        public DateTime ReportDate  { get; private set; }

        public GameReportItem(int gameID, int currency, double turnover, DateTime reportDate)
        {
            this.GameID     = gameID;
            this.Currency   = currency;
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
