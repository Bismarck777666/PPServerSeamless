using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommNode.Database
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

        private List<BaseInsertItem>                        _dbInsertItems              = new List<BaseInsertItem>();
        private Dictionary<long, UserStateUpdateItem>       _dicPlayerStateUpdates      = new Dictionary<long, UserStateUpdateItem>();
        private Dictionary<long, PlayerBalanceUpdateItem>   _dicPlayerBalanceUpates     = new Dictionary<long, PlayerBalanceUpdateItem>();
        private Dictionary<long, UserBetMoneyUpdateItem>    _dicUserBetMoneyUpdates     = new Dictionary<long, UserBetMoneyUpdateItem>();
        private List<BaseClaimedUserBonusUpdateItem>        _claimedBonusItems          = new List<BaseClaimedUserBonusUpdateItem>();
        private Dictionary<string, UserTournamentBetItem>   _dicUserTournamentBetItems  = new Dictionary<string, UserTournamentBetItem>();
        private List<GameLogItem>                           _gameLogItems               = new List<GameLogItem>();
        private List<DateTime>                              _createdGameLogTables       = new List<DateTime>();

        public void insertDBItem(BaseInsertItem insertItem)
        {
            _dbInsertItems.Add(insertItem);
        }

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

        public void updateUserBetMoney(UserBetMoneyUpdateItem item)
        {
            _dicUserBetMoneyUpdates[item.UserDBID] = item;
        }

        public void PushUserBetMoneyUpdates(List<UserBetMoneyUpdateItem> updateItems)
        {
            for(int i = 0; i < updateItems.Count; i++)
            {
                if (_dicUserBetMoneyUpdates.ContainsKey(updateItems[i].UserDBID))
                    continue;
                _dicUserBetMoneyUpdates[updateItems[i].UserDBID] = updateItems[i];
            }
        }
        public List<UserBetMoneyUpdateItem> PopUserBetMoneyUpdates(int count = 100)
        {
            if (_dicUserBetMoneyUpdates.Count == 0)
                return null;

            List<UserBetMoneyUpdateItem> updateItems = new List<UserBetMoneyUpdateItem>();
            foreach (KeyValuePair<long, UserBetMoneyUpdateItem> pair in _dicUserBetMoneyUpdates)
            {
                updateItems.Add(pair.Value);
                if (updateItems.Count >= count)
                    break;
            }
            for (int i = 0; i < updateItems.Count; i++)
                _dicUserBetMoneyUpdates.Remove(updateItems[i].UserDBID);

            return updateItems;
        }
        
        public void insertGameLog(GameLogItem gameLogItem)
        {
            _gameLogItems.Add(gameLogItem);
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

        public void insertUserTournamentBetItem(UserTournamentBetItem item)
        {
            string strKey = string.Format("{0}_{1}", item.UserID, item.TournamentID);
            if (_dicUserTournamentBetItems.ContainsKey(strKey))
                _dicUserTournamentBetItems[strKey].merge(item);
            else
                _dicUserTournamentBetItems[strKey] = item;
        }
        public List<UserTournamentBetItem> PopUserTournamentBetItems(int count = 2000)
        {
            List<UserTournamentBetItem> items = new List<UserTournamentBetItem>();
            foreach(KeyValuePair<string, UserTournamentBetItem> pair in _dicUserTournamentBetItems)
            {
                items.Add(pair.Value);
                if (items.Count >= count)
                    break;
            }
            for(int i = 0; i < items.Count; i++)
            {
                string strKey = string.Format("{0}_{1}", items[i].UserID, items[i].TournamentID);
                _dicUserTournamentBetItems.Remove(strKey);
            }
            return items;
        }
        public void PushUserTournamentBetItems(List<UserTournamentBetItem> items)
        {
            for(int i= 0; i < items.Count; i++)
            {
                string strKey = string.Format("{0}_{1}", items[i].UserID, items[i].TournamentID);
                if (_dicUserTournamentBetItems.ContainsKey(strKey))
                    _dicUserTournamentBetItems[strKey].merge(items[i]);
                else
                    _dicUserTournamentBetItems[strKey] = items[i];
            }
        }    
        public List<BaseInsertItem> PopDBInsertItems(int count = 2000)
        {
            if (_dbInsertItems.Count == 0)
                return null;

            List<BaseInsertItem> insertItems = new List<BaseInsertItem>();
            for (int i = 0; i < _dbInsertItems.Count; i++)
            {
                insertItems.Add(_dbInsertItems[i]);
                if (insertItems.Count >= count)
                    break;
            }
            _dbInsertItems.RemoveRange(0, insertItems.Count);
            return insertItems;
        }

        public void PushDBInsertItems(List<BaseInsertItem> items)
        {
            _dbInsertItems.InsertRange(0, items);
        }

        public List<GameLogItem> PopGameLogItems(int count = 5000)
        {
            if (_gameLogItems.Count == 0)
                return null;

            List<GameLogItem> gameLogItems = new List<GameLogItem>();
            foreach (GameLogItem logItem in _gameLogItems)
            {
                gameLogItems.Add(logItem);
                if (gameLogItems.Count >= count)
                    break;
            }

            _gameLogItems.RemoveRange(0, gameLogItems.Count);
            return gameLogItems;
        }

        public void PushGameLogItems(List<GameLogItem> gameLogItems)
        {
            _gameLogItems.InsertRange(0, gameLogItems);
        }

        public List<UserStateUpdateItem> PopPlayerStates(int count = 5000)
        {
            if (_dicPlayerStateUpdates.Count == 0)
                return null;

            List<UserStateUpdateItem> playerStateUpdates = new List<UserStateUpdateItem>();
            foreach (KeyValuePair<long, UserStateUpdateItem> pair in _dicPlayerStateUpdates)
            {
                //Changed by Foresight(2019.09.23)
                UserStateUpdateItem stateUpdateItem = pair.Value;
                if (stateUpdateItem is UserGameStateItem)
                {
                    UserGameStateItem gameStateItem = stateUpdateItem as UserGameStateItem;
                    if (gameStateItem.State == 0)
                    {
                        //유저가 오프라인될때 현재 펜딩중인 밸런스업데이트를 함께 진행하여 출금조작시 나타날수 있는 오류를 방지한다.
                        double balanceIncrement = 0.0;
                        if (_dicPlayerBalanceUpates.ContainsKey(gameStateItem.PlayerID))
                        {
                            balanceIncrement = _dicPlayerBalanceUpates[gameStateItem.PlayerID].BalanceIncrement;
                            _dicPlayerBalanceUpates.Remove(gameStateItem.PlayerID);
                        }
                        playerStateUpdates.Add(new UserOfflineStateItem(gameStateItem.PlayerID, gameStateItem.GameID, balanceIncrement));
                    }
                    else
                    {
                        playerStateUpdates.Add(pair.Value);
                    }
                }
                else
                {
                    playerStateUpdates.Add(pair.Value);
                }
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
                if (playerStateUpdates[i] is UserOfflineStateItem)
                {
                    UserOfflineStateItem offlineStateItem = playerStateUpdates[i] as UserOfflineStateItem;
                    if (offlineStateItem.BalanceIncrement != 0.0)
                    {
                        if (_dicPlayerBalanceUpates.ContainsKey(offlineStateItem.PlayerID))
                            _dicPlayerBalanceUpates[offlineStateItem.PlayerID].mergeUpdate(offlineStateItem.BalanceIncrement);
                        else
                            _dicPlayerBalanceUpates[offlineStateItem.PlayerID] = new PlayerBalanceUpdateItem(offlineStateItem.PlayerID, offlineStateItem.BalanceIncrement);
                    }

                    if (_dicPlayerStateUpdates.ContainsKey(offlineStateItem.PlayerID))
                        continue;

                    _dicPlayerStateUpdates[offlineStateItem.PlayerID] = new UserGameStateItem(offlineStateItem.PlayerID, 0, offlineStateItem.GameID);
                }
                else
                {
                    if (_dicPlayerStateUpdates.ContainsKey(playerStateUpdates[i].PlayerID))
                        continue;

                    _dicPlayerStateUpdates[playerStateUpdates[i].PlayerID] = playerStateUpdates[i];
                }
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

        public bool IsGameLogTableCreated(DateTime dateTime)
        {
            if (_createdGameLogTables.Contains(dateTime))
                return true;

            return false;
        }

        public void HasCreatedGameLogTable(DateTime dateTime)
        {
            if (!_createdGameLogTables.Contains(dateTime))
                _createdGameLogTables.Add(dateTime);
        }
    }

    public class BaseInsertItem
    {

    }
    public class LoginIPInsertItem : BaseInsertItem
    {
        public LoginIPInsertItem(string strUserID, string ipAddress, string strLoginData, DateTime loginTime)
        {
            this.UserID     = strUserID;
            this.IPAddress  = ipAddress;
            this.LoginData  = strLoginData;
            this.LoginTime  = loginTime;
        }

        public string       UserID      { get; private set; }
        public string       IPAddress   { get; private set; }
        public string       LoginData   { get; private set; }
        public DateTime     LoginTime   { get; private set; }
    }
    public class UserStateUpdateItem
    {
        public UserStateUpdateItem(long playerID)
        {
            this.PlayerID = playerID;
        }
        public long PlayerID { get; protected set; }
    }

    public class UserBetMoneyUpdateItem
    {
        public long     UserDBID        { get; private set; }
        public double   UserBetMoney    { get; private set; }

        public UserBetMoneyUpdateItem(long userDBID, double userBetMoney)
        {
            this.UserDBID       = userDBID;
            this.UserBetMoney   = userBetMoney;
        }
    }
    public class UserLoginStateItem : UserStateUpdateItem
    {
        public UserLoginStateItem(long playerID, int platform) : base(playerID)
        {
            this.Platform = platform;
        }
        public int Platform { get; private set; }

    }

    //유저가 게임에 입장하거나 게임
    public class UserGameStateItem : UserStateUpdateItem
    {
        public UserGameStateItem(long playerID, int state, int gameID) : base(playerID)
        {
            this.State      = state;
            this.GameID     = gameID;
        }
        public int State        { get; private set; }
        public int GameID       { get; private set; }
    }
    public class UserOfflineStateItem : UserStateUpdateItem
    {
        public UserOfflineStateItem(long playerID, int gameID, double balanceIncrement) : base(playerID)
        {
            this.GameID             = gameID;
            this.BalanceIncrement   = balanceIncrement;
        }

        public int      GameID           { get; private set; }
        public double   BalanceIncrement { get; private set; }
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
    public class GameLogItem
    {
        public GameLogItem(string strUserID, int gameID, string strGameName, string tableID, double bet, double win, double beginMoney, double endMoney, string gameLog, DateTime logTime)
        {
            this.UserID     = strUserID;
            this.GameID     = gameID;
            this.GameName   = strGameName;
            this.TableID    = tableID;
            this.Bet        = bet;
            this.Win        = win;
            this.BeginMoney = beginMoney;
            this.EndMoney   = endMoney;
            this.GameLog    = gameLog;
            this.LogTime    = logTime;
        }

        public string   UserID      { get; private set; }
        public int      GameID      { get; private set; }
        public string   GameName    { get; private set; }
        public string   TableID     { get; private set; }
        public double   Bet         { get; private set; }
        public double   Win         { get; private set; }
        public double   BeginMoney  { get; private set; }
        public double   EndMoney    { get; private set; }
        public string   GameLog     { get; private set; }
        public DateTime LogTime     { get; private set; }
    }
    public class BaseClaimedUserBonusUpdateItem
    {

    }

    public class UserTournamentBetItem
    {
        public string UserID        { get; set; }
        public int    TournamentID  { get; set; }
        public double AddScore      { get; set; }
        public double Bet           { get; set; }
        public string Country       { get; set; }
        public string Currency      { get; set; }
        public UserTournamentBetItem(string strUserID, int tournamentID, string strCountry, string strCurrency, double addScore, double bet)
        {
            this.UserID         = strUserID;
            this.TournamentID   = tournamentID;
            this.Country        = strCountry;
            this.Currency       = strCurrency;
            this.AddScore       = addScore;
            this.Bet            = bet;
        }

        public void merge(UserTournamentBetItem item)
        {
            this.AddScore += item.AddScore;
            if (item.Bet > this.Bet)
                this.Bet = item.Bet;
        }
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
    public class ClaimedRedPacketItem : BaseClaimedUserBonusUpdateItem
    {
        public long     RedPacketID     { get; private set; }
        public DateTime ClaimedTime     { get; private set; }
        public ClaimedRedPacketItem(long redPacketID, DateTime claimedTime)
        {
            this.RedPacketID = redPacketID;
            this.ClaimedTime = claimedTime;
        }
    }   
    public class ClaimedUserEventItem : BaseClaimedUserBonusUpdateItem
    {
        public long EventID         { get; private set; }
        public double RewardedMoney { get; private set; }
        public string GameName      { get; private set; }
        public DateTime ClaimedTime { get; private set; }
        public ClaimedUserEventItem(long eventID, double rewardedMoney, string strGameName, DateTime claimedTime)
        {
            this.EventID        = eventID;
            this.GameName       = strGameName;
            this.RewardedMoney  = rewardedMoney;
            this.ClaimedTime    = claimedTime;
        }
    }
    public class ClaimedUserRangeEventItem : BaseClaimedUserBonusUpdateItem
    {
        public long EventID { get; private set; }
        public double RewardedMoney { get; private set; }
        public string GameName { get; private set; }
        public DateTime ClaimedTime { get; private set; }
        public ClaimedUserRangeEventItem(long eventID, double rewardedMoney, string strGameName, DateTime claimedTime)
        {
            this.EventID = eventID;
            this.GameName = strGameName;
            this.RewardedMoney = rewardedMoney;
            this.ClaimedTime = claimedTime;
        }
    }
}
