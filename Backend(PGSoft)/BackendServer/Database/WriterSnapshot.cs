using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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

        private Dictionary<long, UserStateUpdateItem>       _dicPlayerStateUpdates      = new Dictionary<long, UserStateUpdateItem>();
        private Dictionary<long, PlayerBalanceUpdateItem>   _dicPlayerBalanceUpates     = new Dictionary<long, PlayerBalanceUpdateItem>();
        private List<BaseClaimedUserBonusUpdateItem>        _claimedBonusItems          = new List<BaseClaimedUserBonusUpdateItem>();
        private List<PGGameHistoryDBItem>                   _pgGameHistoryItems         = new List<PGGameHistoryDBItem>();


        public void updatePlayerBalanceUpdate(PlayerBalanceUpdateItem balanceUpdateItem)
        {
            long playerID = balanceUpdateItem.PlayerID;
            if (_dicPlayerBalanceUpates.ContainsKey(playerID))
                _dicPlayerBalanceUpates[playerID].mergeUpdate(balanceUpdateItem);
            else
                _dicPlayerBalanceUpates[playerID] = balanceUpdateItem;
        }
        public void updatePlayerState(UserStateUpdateItem stateUpdateItem)
        {
            _dicPlayerStateUpdates[stateUpdateItem.PlayerID] = stateUpdateItem;
        }
        public void claimBonusItem(BaseClaimedUserBonusUpdateItem item)
        {
            _claimedBonusItems.Add(item);
        }

        public List<BaseClaimedUserBonusUpdateItem> PopClaimedBonusItems(int count = 1000)
        {
            if (_claimedBonusItems.Count == 0)
                return null;

            List<BaseClaimedUserBonusUpdateItem> insertItems = new List<BaseClaimedUserBonusUpdateItem>();
            for (int i = 0; i < _claimedBonusItems.Count; i++)
            {
                insertItems.Add(_claimedBonusItems[i]);
                if (insertItems.Count >= count)
                    break;
            }
            _claimedBonusItems.RemoveRange(0, insertItems.Count);
            return insertItems;
        }
        public void PushClaimedBonusItems(List<BaseClaimedUserBonusUpdateItem> items)
        {
            _claimedBonusItems.InsertRange(0, items);
        }
        
        public List<UserStateUpdateItem> PopPlayerStates(int count = 5000)
        {
            if (_dicPlayerStateUpdates.Count == 0)
                return null;

            List<UserStateUpdateItem> playerStateUpdates = new List<UserStateUpdateItem>();
            foreach (KeyValuePair<long, UserStateUpdateItem> pair in _dicPlayerStateUpdates)
            {
                playerStateUpdates.Add(pair.Value);
                if (playerStateUpdates.Count >= count)
                    break;
            }

            for (int i = 0; i < playerStateUpdates.Count; i++)
                _dicPlayerStateUpdates.Remove(playerStateUpdates[i].PlayerID);

            return playerStateUpdates;
        }
        public void PushPlayerStates(List<UserStateUpdateItem> playerStateUpdates)
        {
            for (int i = 0; i < playerStateUpdates.Count; i++)
            {
                if (_dicPlayerStateUpdates.ContainsKey(playerStateUpdates[i].PlayerID))
                    continue;

                _dicPlayerStateUpdates[playerStateUpdates[i].PlayerID] = playerStateUpdates[i];
            }
        }
        public List<PlayerBalanceUpdateItem> PopBalanceUpdates(int count = 5000)
        {
            if (_dicPlayerBalanceUpates.Count == 0)
                return null;

            List<PlayerBalanceUpdateItem> balanceUpdates = new List<PlayerBalanceUpdateItem>();
            foreach (KeyValuePair<long, PlayerBalanceUpdateItem> pair in _dicPlayerBalanceUpates)
            {
                balanceUpdates.Add(pair.Value);
                if (balanceUpdates.Count >= count)
                    break;
            }
            for (int i = 0; i < balanceUpdates.Count; i++)
                _dicPlayerBalanceUpates.Remove(balanceUpdates[i].PlayerID);

            return balanceUpdates;
        }       
        public void PushBalanceUpdateItems(List<PlayerBalanceUpdateItem> balanceUpdates)
        {
            for (int i = 0; i < balanceUpdates.Count; i++)
            {
                long playerID = balanceUpdates[i].PlayerID;
                if (_dicPlayerBalanceUpates.ContainsKey(playerID))
                    _dicPlayerBalanceUpates[playerID].mergeUpdate(balanceUpdates[i]);
                else
                    _dicPlayerBalanceUpates[playerID] = balanceUpdates[i];
            }
        }

        public void insertPGGameHistory(PGGameHistoryDBItem item)
        {
            _pgGameHistoryItems.Add(item);
        }
        public List<PGGameHistoryDBItem> PopPGGameHistoryItems(int count = 1000)
        {
            List<PGGameHistoryDBItem> items = new List<PGGameHistoryDBItem>();

            if (_pgGameHistoryItems.Count < count)
                count = _pgGameHistoryItems.Count;

            if (count == 0)
                return null;

            items.AddRange(_pgGameHistoryItems.GetRange(0, count));
            _pgGameHistoryItems.RemoveRange(0, items.Count);
            return items;
        }
        public void PushPGGameHistoryItems(List<PGGameHistoryDBItem> items)
        {
            _pgGameHistoryItems.InsertRange(0, items);
        }
    }

    public class UserStateUpdateItem
    {
        public UserStateUpdateItem(long playerID, int isOnline)
        {
            this.PlayerID = playerID;
            this.IsOnline = isOnline;
        }
        public long PlayerID { get; protected set; }
        public int  IsOnline { get; protected set; }
    }
    public class PlayerBalanceUpdateItem
    {
        public PlayerBalanceUpdateItem(long playerID, double balanceIncrement)
        {
            this.PlayerID           = playerID;
            this.BalanceIncrement   = balanceIncrement;
        }
            
        public void mergeUpdate(PlayerBalanceUpdateItem updateItem)
        {
            this.BalanceIncrement += updateItem.BalanceIncrement;
        }

        public void mergeUpdate(double balanceUpdate)
        {
            this.BalanceIncrement += balanceUpdate;
        }
        public long     PlayerID            { get; private set; }            
        public double   BalanceIncrement    { get; private set; }
    }
    public class BaseClaimedUserBonusUpdateItem
    {

    }
    public class ClaimedGameJackpotItem : BaseClaimedUserBonusUpdateItem
    {
        public long         BonusID     { get; private set; }
        public DateTime     ClaimedTime { get; private set; }
        public int          GameID      { get; private set; }
        public ClaimedGameJackpotItem(long bonusID, int gameID, DateTime claimedTime)
        {
            this.BonusID     = bonusID;
            this.GameID      = gameID;
            this.ClaimedTime = claimedTime;
        }
    }

    public class PGGameHistoryDBItem
    {
        public string   UserID          { get; set; }
        public int      GameID          { get; set; }
        public double   Bet             { get; set; }
        public double   Profit          { get; set; }
        public long     TransactionID   { get; set; }
        public long     Timestamp       { get; set; }
        public string   Data            { get; set; }

        public PGGameHistoryDBItem(string strUserName, int gameID, double bet, double profit, string transactionID, long timestamp, string strData)
        {
            this.UserID         = strUserName;
            this.GameID         = gameID;
            this.Bet            = bet;
            this.Profit         = profit;
            this.Timestamp      = timestamp;
            this.TransactionID  = long.Parse(transactionID);
            this.Data           = strData;
        }
    }

}
