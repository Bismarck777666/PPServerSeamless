using System;
using System.Collections.Generic;

namespace UserNode.Database
{
    public class WriterSnapshot
    {
        private static WriterSnapshot   _sInstance      = new WriterSnapshot();
        private List<BaseInsertItem>    _dbInsertItems  = new List<BaseInsertItem>();
        private Dictionary<long, UserStateUpdateItem>       _dicPlayerStateUpdates      = new Dictionary<long, UserStateUpdateItem>();
        private Dictionary<long, PlayerBalanceUpdateItem>   _dicPlayerBalanceUpates     = new Dictionary<long, PlayerBalanceUpdateItem>();
        private Dictionary<int, CompanyBalanceUpdateItem>   _dicCompanyBalanceUpates    = new Dictionary<int, CompanyBalanceUpdateItem>();
        
        private List<GameLogItem>               _gameLogItems               = new List<GameLogItem>();
        private List<ApiTransactionUpsertItem>  _apiTransactions            = new List<ApiTransactionUpsertItem>();
        private List<int>                       _createdAgentGameLogTables  = new List<int>();

        public static WriterSnapshot Instance => _sInstance;

        public void insertDBItem(BaseInsertItem insertItem)
        {
            _dbInsertItems.Add(insertItem);
        }

        public void insertApiTransaction(ApiTransactionUpsertItem item)
        {
            _apiTransactions.Add(item);
        }

        public List<ApiTransactionUpsertItem> fetchApiTransactionItems(int count = 1000)
        {
            List<ApiTransactionUpsertItem> transactionUpsertItemList = new List<ApiTransactionUpsertItem>();
            for (int i = 0; i < _apiTransactions.Count; i++)
            {
                transactionUpsertItemList.Add(_apiTransactions[i]);
                if (transactionUpsertItemList.Count >= count)
                    break;
            }

            _apiTransactions.RemoveRange(0, transactionUpsertItemList.Count);
            return transactionUpsertItemList;
        }

        public void PushApiTransactionItems(List<ApiTransactionUpsertItem> items)
        {
            _apiTransactions.InsertRange(0, items);
        }

        public void updatePlayerBalanceUpdate(PlayerBalanceUpdateItem balanceUpdateItem)
        {
            long playerId = balanceUpdateItem.PlayerID;
            if (_dicPlayerBalanceUpates.ContainsKey(playerId))
                _dicPlayerBalanceUpates[playerId].mergeUpdate(balanceUpdateItem);
            else
                _dicPlayerBalanceUpates[playerId] = balanceUpdateItem;
        }

        public void updateCompanyBalanceUpdate(CompanyBalanceUpdateItem item)
        {
            int companyId = item.CompanyID;
            if (_dicCompanyBalanceUpates.ContainsKey(companyId))
                _dicCompanyBalanceUpates[companyId].mergeUpdate(item);
            else
                _dicCompanyBalanceUpates[companyId] = item;
        }

        public void updatePlayerState(UserStateUpdateItem stateUpdateItem)
        {
            _dicPlayerStateUpdates[stateUpdateItem.PlayerID] = stateUpdateItem;
        }

        public double fetchUserBalanceUpdates(long userDBID)
        {
            double balanceItem = 0.0;
            if (_dicPlayerBalanceUpates.ContainsKey(userDBID))
            {
                balanceItem = _dicPlayerBalanceUpates[userDBID].BalanceIncrement;
                _dicPlayerBalanceUpates.Remove(userDBID);
            }
            _dicPlayerStateUpdates.Remove(userDBID);
            return balanceItem;
        }

        public void insertGameLog(GameLogItem gameLogItem)
        {
            _gameLogItems.Add(gameLogItem);
        }

        public List<BaseInsertItem> PopDBInsertItems(int count = 2000)
        {
            if (_dbInsertItems.Count == 0)
                return null;

            List<BaseInsertItem> baseInsertItemList = new List<BaseInsertItem>();
            for (int i = 0; i < _dbInsertItems.Count; i++)
            {
                baseInsertItemList.Add(_dbInsertItems[i]);
                if (baseInsertItemList.Count >= count)
                    break;
            }

            _dbInsertItems.RemoveRange(0, baseInsertItemList.Count);
            return baseInsertItemList;
        }

        public void PushDBInsertItems(List<BaseInsertItem> items)
        {
            _dbInsertItems.InsertRange(0, items);
        }

        public List<GameLogItem> PopGameLogItems(int count = 5000)
        {
            if (_gameLogItems.Count == 0)
                return null;

            List<GameLogItem> gameLogItemList = new List<GameLogItem>();
            foreach (GameLogItem gameLogItem in _gameLogItems)
            {
                gameLogItemList.Add(gameLogItem);
                if (gameLogItemList.Count >= count)
                    break;
            }

            _gameLogItems.RemoveRange(0, gameLogItemList.Count);
            return gameLogItemList;
        }

        public void PushGameLogItems(List<GameLogItem> gameLogItems)
        {
            _gameLogItems.InsertRange(0, gameLogItems);
        }

        public List<UserStateUpdateItem> PopPlayerStates(int count = 5000)
        {
            if (_dicPlayerStateUpdates.Count == 0)
                return null;

            List<UserStateUpdateItem> userStateUpdateItemList = new List<UserStateUpdateItem>();
            foreach (KeyValuePair<long, UserStateUpdateItem> pair in _dicPlayerStateUpdates)
            {
                UserStateUpdateItem userStateUpdateItem = pair.Value;
                if (userStateUpdateItem is UserGameStateItem)
                {
                    UserGameStateItem userGameStateItem = userStateUpdateItem as UserGameStateItem;
                    if (userGameStateItem.State == 0)
                    {
                        double balanceIncrement = 0.0;
                        if (_dicPlayerBalanceUpates.ContainsKey(userGameStateItem.PlayerID))
                        {
                            balanceIncrement = _dicPlayerBalanceUpates[userGameStateItem.PlayerID].BalanceIncrement;
                            _dicPlayerBalanceUpates.Remove(userGameStateItem.PlayerID);
                        }

                        userStateUpdateItemList.Add(new UserOfflineStateItem(userGameStateItem.PlayerID, userGameStateItem.GameID, balanceIncrement));
                    }
                    else
                        userStateUpdateItemList.Add(pair.Value);
                }
                else
                    userStateUpdateItemList.Add(pair.Value);

                if (userStateUpdateItemList.Count >= count)
                    break;
            }

            for (int i = 0; i < userStateUpdateItemList.Count; i++)
                _dicPlayerStateUpdates.Remove(userStateUpdateItemList[i].PlayerID);
            
            return userStateUpdateItemList;
        }

        public void PushPlayerStates(List<UserStateUpdateItem> playerStateUpdates)
        {
            for (int i = 0; i < playerStateUpdates.Count; i++)
            {
                if (playerStateUpdates[i] is UserOfflineStateItem)
                {
                    UserOfflineStateItem playerStateUpdate = playerStateUpdates[i] as UserOfflineStateItem;
                    if (playerStateUpdate.BalanceIncrement != 0.0)
                    {
                        if (_dicPlayerBalanceUpates.ContainsKey(playerStateUpdate.PlayerID))
                            _dicPlayerBalanceUpates[playerStateUpdate.PlayerID].mergeUpdate(playerStateUpdate.BalanceIncrement);
                        else
                            _dicPlayerBalanceUpates[playerStateUpdate.PlayerID] = new PlayerBalanceUpdateItem(playerStateUpdate.PlayerID, playerStateUpdate.BalanceIncrement);
                    }

                    if (!_dicPlayerStateUpdates.ContainsKey(playerStateUpdate.PlayerID))
                        _dicPlayerStateUpdates[playerStateUpdate.PlayerID] = new UserGameStateItem(playerStateUpdate.PlayerID, 0, playerStateUpdate.GameID);
                }
                else if (!_dicPlayerStateUpdates.ContainsKey(playerStateUpdates[i].PlayerID))
                    _dicPlayerStateUpdates[playerStateUpdates[i].PlayerID] = playerStateUpdates[i];
            }
        }

        public List<PlayerBalanceUpdateItem> PopBalanceUpdates(int count = 5000)
        {
            if (_dicPlayerBalanceUpates.Count == 0)
                return null;

            List<PlayerBalanceUpdateItem> balanceUpdateItemList = new List<PlayerBalanceUpdateItem>();
            foreach (KeyValuePair<long, PlayerBalanceUpdateItem> pair in _dicPlayerBalanceUpates)
            {
                balanceUpdateItemList.Add(pair.Value);
                if (balanceUpdateItemList.Count >= count)
                    break;
            }

            for (int i = 0; i < balanceUpdateItemList.Count; i++)
                _dicPlayerBalanceUpates.Remove(balanceUpdateItemList[i].PlayerID);

            return balanceUpdateItemList;
        }

        public void PushBalanceUpdateItems(List<PlayerBalanceUpdateItem> balanceUpdates)
        {
            for (int i = 0; i < balanceUpdates.Count; i++)
            {
                long playerId = balanceUpdates[i].PlayerID;
                if (_dicPlayerBalanceUpates.ContainsKey(playerId))
                    _dicPlayerBalanceUpates[playerId].mergeUpdate(balanceUpdates[i]);
                else
                    _dicPlayerBalanceUpates[playerId] = balanceUpdates[i];
            }
        }

        public List<CompanyBalanceUpdateItem> PopCompanyBalanceUpdates(int count = 5000)
        {
            if (_dicCompanyBalanceUpates.Count == 0)
                return null;

            List<CompanyBalanceUpdateItem> balanceUpdateItemList = new List<CompanyBalanceUpdateItem>();
            foreach (KeyValuePair<int, CompanyBalanceUpdateItem> pair in _dicCompanyBalanceUpates)
            {
                balanceUpdateItemList.Add(pair.Value);
                if (balanceUpdateItemList.Count >= count)
                    break;
            }

            for (int i = 0; i < balanceUpdateItemList.Count; i++)
                _dicCompanyBalanceUpates.Remove(balanceUpdateItemList[i].CompanyID);
            
            return balanceUpdateItemList;
        }

        public void PushCompanyBalanceUpdateItems(List<CompanyBalanceUpdateItem> balanceUpdates)
        {
            for (int i = 0; i < balanceUpdates.Count; i++)
            {
                int companyId = balanceUpdates[i].CompanyID;
                if (_dicCompanyBalanceUpates.ContainsKey(companyId))
                    _dicCompanyBalanceUpates[companyId].mergeUpdate(balanceUpdates[i]);
                else
                    _dicCompanyBalanceUpates[companyId] = balanceUpdates[i];
            }
        }

        public bool IsAgentGameLogTableCreated(int agentID)
        {
            return _createdAgentGameLogTables.Contains(agentID);
        }

        public void HasCreatedAgentGameLogTable(int agentID)
        {
            if (!_createdAgentGameLogTables.Contains(agentID))
                _createdAgentGameLogTables.Add(agentID);
        }
    }

    public class BaseInsertItem
    {
    }
    public class LoginIPInsertItem : BaseInsertItem
    {
        public LoginIPInsertItem(string strUserID,string ipAddress,string strLoginData,DateTime loginTime)
        {
            UserID      = strUserID;
            IPAddress   = ipAddress;
            LoginData   = strLoginData;
            LoginTime   = loginTime;
        }

        public string   UserID      { get; private set; }
        public string   IPAddress   { get; private set; }
        public string   LoginData   { get; private set; }
        public DateTime LoginTime   { get; private set; }
    }
    public class UserStateUpdateItem
    {
        public UserStateUpdateItem(long playerID)
        {
            PlayerID = playerID;
        }

        public long PlayerID { get; protected set; }
    }
    public class UserLoginStateItem : UserStateUpdateItem
    {
        public UserLoginStateItem(long playerID)
          : base(playerID)
        {
        }
    }
    public class UserGameStateItem : UserStateUpdateItem
    {
        public UserGameStateItem(long playerID, int state, int gameID)
          : base(playerID)
        {
            State   = state;
            GameID  = gameID;
        }

        public int State    { get; private set; }
        public int GameID   { get; private set; }
    }
    public class UserOfflineStateItem : UserStateUpdateItem
    {
        public UserOfflineStateItem(long playerID, int gameID, double balanceIncrement)
          : base(playerID)
        {
            GameID              = gameID;
            BalanceIncrement    = balanceIncrement;
        }

        public int      GameID              { get; private set; }
        public double   BalanceIncrement    { get; private set; }
    }
    public class PlayerBalanceUpdateItem
    {
        public PlayerBalanceUpdateItem(long playerID, double balanceIncrement)
        {
            PlayerID            = playerID;
            BalanceIncrement    = balanceIncrement;
        }

        public void mergeUpdate(PlayerBalanceUpdateItem updateItem)
        {
            BalanceIncrement += updateItem.BalanceIncrement;
        }

        public void mergeUpdate(double balanceUpdate)
        {
            BalanceIncrement += balanceUpdate;
        }

        public long     PlayerID            { get; private set; }
        public double   BalanceIncrement    { get; private set; }
    }
    public class GameLogItem
    {
        public GameLogItem(string strUserID,int gameID,string strGameName,double bet,double win,double beginMoney,double endMoney,string gameLog,int betType,DateTime logTime,int agentID)
        {
            UserID      = strUserID;
            GameID      = gameID;
            GameName    = strGameName;
            Bet         = bet;
            Win         = win;
            BeginMoney  = beginMoney;
            EndMoney    = endMoney;
            GameLog     = gameLog;
            LogTime     = logTime;
            AgentID     = agentID;
            BetType     = betType;
        }

        public string   UserID      { get; private set; }
        public int      GameID      { get; private set; }
        public string   GameName    { get; private set; }
        public int      BetType     { get; private set; }
        public double   Bet         { get; private set; }
        public double   Win         { get; private set; }
        public double   BeginMoney  { get; private set; }
        public double   EndMoney    { get; private set; }
        public string   GameLog     { get; private set; }
        public DateTime LogTime     { get; private set; }
        public int      AgentID     { get; private set; }
    }
    public class ApiTransactionUpsertItem
    {
    }
    public class ApiTransactionUpdateItem : ApiTransactionUpsertItem
    {
        public string   UserID  { get; set; }
        public long     TransID { get; set; }

        public ApiTransactionUpdateItem(string userID, long transID)
        {
            UserID  = userID;
            TransID = transID;
        }
    }
    public class ApiTransactionItem : ApiTransactionUpsertItem
    {
        public ApiTransactionItem(string strUserID,long transID,TransactionTypes transType,TransactionTypeIDs transTypeID,long relTransID,int gameID,double withAmount,double depAmount,DateTime timestamp)
        {
            UserID              = strUserID;
            TransactionID       = transID;
            TransactionType     = transType;
            TransTypeID         = transTypeID;
            RelTransactionID    = relTransID;
            WithAmount          = withAmount;
            DepAmount           = depAmount;
            Timestamp           = timestamp;
            GameID              = gameID;
        }

        public TransactionTypes     TransactionType     { get; private set; }
        public TransactionTypeIDs   TransTypeID         { get; private set; }
        public long                 TransactionID       { get; private set; }
        public long                 RelTransactionID    { get; private set; }
        public int                  GameID              { get; private set; }
        public double               WithAmount          { get; private set; }
        public double               DepAmount           { get; private set; }
        public string               UserID              { get; private set; }
        public DateTime             Timestamp           { get; private set; }
    }
    public enum TransactionTypes
    {
        Withdraw    = 0,
        Deposit     = 1,
        WithDep     = 2,
    }
    public enum TransactionTypeIDs
    {
        Tip             = -9, // 0xFFFFFFF7
        JoinTour        = -4, // 0xFFFFFFFC
        SitBuyIn        = -2, // 0xFFFFFFFE
        StandardBet     = -1, // 0xFFFFFFFF
        StandardBetWin  = 0,
        StandardWin     = 1,
        TableWin        = 2,
        TourWin         = 3,
        UnRegTour       = 4,
        CashbackBonus   = 9,
    }
    public class CompanyBalanceUpdateItem
    {
        public CompanyBalanceUpdateItem(int companyID, double balanceIncrement)
        {
            CompanyID           = companyID;
            BalanceIncrement    = balanceIncrement;
        }

        public void mergeUpdate(CompanyBalanceUpdateItem updateItem)
        {
            BalanceIncrement += updateItem.BalanceIncrement;
        }

        public int      CompanyID           { get; private set; }
        public double   BalanceIncrement    { get; private set; }
    }
    public class FetchUserBalanceUpdate
    {
        public long UserDBID { get; private set; }
        public FetchUserBalanceUpdate(long userDBID)
        {
            UserDBID = userDBID;
        }
    }

}
