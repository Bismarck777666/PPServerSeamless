using System;
using System.Collections.Generic;

namespace UserNode.Database
{
    public class ReportSnapshot
    {
        private Dictionary<string, ReportUpdateItem>    _dicReportUpdateItems   = new Dictionary<string, ReportUpdateItem>();
        private Dictionary<string, GameReportItem>      _dicGameReports         = new Dictionary<string, GameReportItem>();
        private static ReportSnapshot                   _sInstance              = new ReportSnapshot();
        
        public static ReportSnapshot Instance => _sInstance;

        public void updateGameReport(GameReportItem reportItem)
        {
            string strKey = string.Format("{0}_{1}_{2}", reportItem.GameID, reportItem.AgentID, reportItem.ReportDate.ToString("yyyyMMddHHmm"));
            if (_dicGameReports.ContainsKey(strKey))
                _dicGameReports[strKey].mergeReport(reportItem);
            else
                _dicGameReports[strKey] = reportItem;
        }

        public List<GameReportItem> PopGameReportUpdates(int maxCount = 5000)
        {
            if (_dicGameReports.Count == 0)
                return null;

            List<GameReportItem> gameReportItemList = new List<GameReportItem>();
            foreach (KeyValuePair<string, GameReportItem> pair in _dicGameReports)
            {
                gameReportItemList.Add(pair.Value);
                if (gameReportItemList.Count >= maxCount)
                    break;
            }

            for (int i = 0; i < gameReportItemList.Count; i++)
                _dicGameReports.Remove(string.Format("{0}_{1}_{2}", gameReportItemList[i].GameID, gameReportItemList[i].AgentID, gameReportItemList[i].ReportDate.ToString("yyyyMMddHHmm")));
            
            return gameReportItemList;
        }

        public void PushGameReportUpdates(List<GameReportItem> reportUpdateItems)
        {
            for (int i = 0; i < reportUpdateItems.Count; i++)
            {
                string strKey = string.Format("{0}_{1}_{2}", reportUpdateItems[i].GameID, reportUpdateItems[i].AgentID, reportUpdateItems[i].ReportDate.ToString("yyyyMMddHHmm"));
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

            List<ReportUpdateItem> reportUpdateItemList = new List<ReportUpdateItem>();
            foreach (KeyValuePair<string, ReportUpdateItem> pair in _dicReportUpdateItems)
            {
                reportUpdateItemList.Add(pair.Value);
                if (reportUpdateItemList.Count >= maxCount)
                    break;
            }

            for (int i = 0; i < reportUpdateItemList.Count; i++)
                _dicReportUpdateItems.Remove(string.Format("{0}_{1}", reportUpdateItemList[i].UserID, reportUpdateItemList[i].ReportDateTime.ToString("yyyyMMddHHmm")));
            
            return reportUpdateItemList;
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
        public int      AgentID     { get; private set; }
        public double   BetMoney    { get; private set; }
        public double   WinMoney    { get; private set; }
        public DateTime ReportDate  { get; private set; }

        public GameReportItem(int gameID,int agentID,double betMoney,double winMoney,DateTime reportDate)
        {
            GameID      = gameID;
            AgentID     = agentID;
            BetMoney    = betMoney;
            WinMoney    = winMoney;
            ReportDate  = reportDate;
        }

        public void mergeReport(GameReportItem item)
        {
            BetMoney += item.BetMoney;
        }
    }

    public class ReportUpdateItem
    {
        public ReportUpdateItem(string strUserID,int agentID,DateTime reportTime,double bet,double win)
        {
            UserID          = strUserID;
            ReportDateTime  = reportTime;
            Bet             = bet;
            Win             = win;
            AgentID         = agentID;
        }

        public void mergeReport(ReportUpdateItem other)
        {
            Bet += other.Bet;
            Win += other.Win;
        }

        public string   UserID          { get; private set; }
        public DateTime ReportDateTime  { get; private set; }
        public double   Bet             { get; private set; }
        public double   Win             { get; private set; }
        public int      AgentID         { get; private set; }
    }
}
