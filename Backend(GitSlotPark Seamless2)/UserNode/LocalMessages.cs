using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using GITProtocol;
using Akka.Routing;
using Newtonsoft.Json;

/****
 * 
 *          Created by Foresight(2021.03.12)
 *          해당 노드내부에서 교환되는 메세지들을 정의한다.
 * 
 */

namespace UserNode
{            
    public class QuitUserMessage
    {
        public int    AgentID   { get; private set; }
        public string UserID    { get; private set; }

        public string GlobalUserID
        {
            get { return string.Format("{0}_{1}", AgentID, UserID); }
        }
        public QuitUserMessage(int agentID, string strUserID)
        {
            this.AgentID    = agentID;
            this.UserID     = strUserID;
        }
    }
    
    public class SlotGamesNodeShuttingdown
    {
        public string Path { get; private set; }
        public SlotGamesNodeShuttingdown(string strPath)
        {
            this.Path = strPath;
        }
    }
    
    public class QuitAndNotifyMessage
    {
    }
    
    public class BaseBonusItem
    {
        public int      AgentID     { get; protected set; }
        public long     BonusID     { get; protected set; }
        public string   UserID      { get; protected set; }

        public string GlobalUserID
        {
            get
            {
                return string.Format("{0}_{1}", this.AgentID, this.UserID);
            }
        }
        public BaseBonusItem(long bonusID, int agentID, string strUserID)
        {
            this.AgentID    = agentID;
            this.BonusID    = bonusID;
            this.UserID     = strUserID;
        }
    }
    public class UserEventCancelled
    {
        public int      AgentID     { get; private set; }
        public string   UserID      { get; private set; }
        public long     BonusID     { get; private set; }

        public string GlobalUserID
        {
            get
            {
                return string.Format("{0}_{1}", this.AgentID, this.UserID);
            }
        }
        public UserEventCancelled(int agentID, string strUserID, long bonusID)
        {
            this.AgentID    = agentID;
            this.UserID     = strUserID;
            this.BonusID    = bonusID;
        }
    }
    public class UserRangeOddEventItem : BaseBonusItem
    {

        public double MinOdd { get; private set; }
        public double MaxOdd { get; private set; }
        public double MaxBet { get; private set; }

        public UserRangeOddEventItem(long bonusID, int agentID, string strUserID, double minOdd, double maxOdd, double maxBet) : base(bonusID, agentID, strUserID)
        {
            this.MinOdd = minOdd;
            this.MaxOdd = maxOdd;
            this.MaxBet = maxBet;
        }
    }
    public class BaseClaimedUserBonusUpdateItem
    {

    }
    public class ClaimedUserRangeEventItem : BaseClaimedUserBonusUpdateItem
    {
        public long     EventID         { get; private set; }
        public double   RewardedMoney   { get; private set; }
        public string   GameName        { get; private set; }
        public DateTime ClaimedTime     { get; private set; }
        public ClaimedUserRangeEventItem(long eventID, double rewardedMoney, string strGameName, DateTime claimedTime)
        {
            this.EventID        = eventID;
            this.GameName       = strGameName;
            this.RewardedMoney  = rewardedMoney;
            this.ClaimedTime    = claimedTime;
        }
    }
    public class ClaimedUserRangeEventMessage
    {
        public long     ID              { get; private set; }
        public int      AgentID         { get; private set; }
        public string   UserID          { get; private set; }
        public double   RewardedMoney   { get; private set; }
        public string   GameName        { get; private set; }
        public ClaimedUserRangeEventMessage(long id, int agentID, string strUserID, double rewardedMoney, string strGameName)
        {
            this.ID             = id;
            this.AgentID        = agentID;
            this.UserID         = strUserID;
            this.RewardedMoney  = rewardedMoney;
            this.GameName       = strGameName;
        }
    }
    public class GetUserBonusItems
    {
        public int    AgentID   { get; private set; }
        public string UserID    { get; private set; }
        public GetUserBonusItems(int agentID, string strUserID)
        {
            this.AgentID = agentID;
            this.UserID  = strUserID;
        }
    }

}
