using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using System.Data.Common;
using Akka.Routing;
using GITProtocol;
using Akka.Event;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace CommNode.Database
{
    public class DBProxyReader : ReceiveActor
    {
        private readonly ILoggingAdapter _logger                = Logging.GetLogger(Context);
        private string                   _strConnString         = "";

        protected enum CurrencyEnum
        {
            USD = 0,    //미딸라
            EUR = 1,    //유로
            TND = 2,    //뜌니지(디나르)
            KRW = 3,    //코리아(원)
            GMD = 4,    //감비아(다르실)
            CNY = 5,    //중국(위엔)
            JPY = 6,    //일본(엔)
            MYR = 7,    //말레이시아(링기트)
            THB = 8,    //타이(바흐트)
            PHP = 9,    //필리핀(페소)
            VND = 10,   //베트남(동)
            INR = 11,   //인디아(루피)
            IDR = 12,   //인도네시아(루피아)
            PKR = 13,   //파키스탄(루피)
            BDT = 14,   //방글라데슈(타카)
            NPR = 15,   //네팔(루피)
            UGX = 16,   //우간다(쉴링)
            TRY = 17,   //터키(리라)
            RUB = 18,   //러시아(루불)
        }

        protected Dictionary<int, CurrencyObj> _currencyInfo = new Dictionary<int, CurrencyObj>()
        {
            { (int)CurrencyEnum.USD, new CurrencyObj(){ CurrencyText = "USD",CurrencySymbol = "$",  Rate = 1        } },
            { (int)CurrencyEnum.EUR, new CurrencyObj(){ CurrencyText = "EUR",CurrencySymbol = "€",  Rate = 1        } },
            { (int)CurrencyEnum.TND, new CurrencyObj(){ CurrencyText = "TND",CurrencySymbol = "D",  Rate = 3        } },
            { (int)CurrencyEnum.KRW, new CurrencyObj(){ CurrencyText = "KRW",CurrencySymbol = "₩",  Rate = 1000     } },
            { (int)CurrencyEnum.GMD, new CurrencyObj(){ CurrencyText = "GMD",CurrencySymbol = "D",  Rate = 60       } },
            { (int)CurrencyEnum.CNY, new CurrencyObj(){ CurrencyText = "CNY",CurrencySymbol = "¥",  Rate = 10       } },
            { (int)CurrencyEnum.JPY, new CurrencyObj(){ CurrencyText = "JPY",CurrencySymbol = "¥",  Rate = 100      } },
            { (int)CurrencyEnum.MYR, new CurrencyObj(){ CurrencyText = "MYR",CurrencySymbol = "RM", Rate = 5        } },
            { (int)CurrencyEnum.THB, new CurrencyObj(){ CurrencyText = "THB",CurrencySymbol = "฿",  Rate = 30       } },
            { (int)CurrencyEnum.PHP, new CurrencyObj(){ CurrencyText = "PHP",CurrencySymbol = "₱",  Rate = 60       } },
            { (int)CurrencyEnum.VND, new CurrencyObj(){ CurrencyText = "VND",CurrencySymbol = "₫",  Rate = 25000    } },
            { (int)CurrencyEnum.INR, new CurrencyObj(){ CurrencyText = "INR",CurrencySymbol = "₹",  Rate = 80       } },
            { (int)CurrencyEnum.IDR, new CurrencyObj(){ CurrencyText = "IDR",CurrencySymbol = "Rp", Rate = 15000    } },
            { (int)CurrencyEnum.PKR, new CurrencyObj(){ CurrencyText = "PKR",CurrencySymbol = "₨",  Rate = 300      } },
            { (int)CurrencyEnum.BDT, new CurrencyObj(){ CurrencyText = "BDT",CurrencySymbol = "৳",  Rate = 100      } },
            { (int)CurrencyEnum.NPR, new CurrencyObj(){ CurrencyText = "NPR",CurrencySymbol = "रु",  Rate = 100      } },
            { (int)CurrencyEnum.UGX, new CurrencyObj(){ CurrencyText = "UGX",CurrencySymbol = "UGX",Rate = 1000     } },
            { (int)CurrencyEnum.TRY, new CurrencyObj(){ CurrencyText = "TRY",CurrencySymbol = "₺",  Rate = 10       } },
            { (int)CurrencyEnum.RUB, new CurrencyObj(){ CurrencyText = "RUB",CurrencySymbol = "₽",  Rate = 100      } },
        };

        public DBProxyReader(string strConnString)
        {
            _strConnString = strConnString;

            //유저로그인 요청을 처리한다.
            ReceiveAsync<UserLoginRequest>                  (doLoginRequest);
            ReceiveAsync<GetUserBonusItems>                 (getUserBonusItems);
            ReceiveAsync<HTTPPPReplayListRequest>           (onRequestPPReplayList);
            ReceiveAsync<HTTPPPReplayDataRequest>           (onRequestPPReplayData);
            ReceiveAsync<HTTPPPReplayMakeLinkRequest>       (onRequestPPReplayMakeLink);
            ReceiveAsync<HTTPPPHistoryGetLastItemsRequest>  (onRequestPPHistoryList);
            ReceiveAsync<HTTPPPHistoryGetItemDetailRequest> (onRequestPPHistoryData);
            ReceiveAsync<HTTPPPVerifyGetLastItemRequest>    (onRequestPPVerifyItem);
            ReceiveAsync<ClaimedUserRangeEventMessage>      (fetchNewUserRangeEvent);
            ReceiveAsync<ApiWithdrawRequest>                (onUserApiWithdraw);

            ReceiveAsync<BNGTransactionListRequest>         (onBNGTransactionListRequest);
            ReceiveAsync<BNGAggregateRequest>               (onBNGAggregateRequest);
            ReceiveAsync<BNGTransDetailRequest>             (onBNGTransDetailRequest);

            ReceiveAsync<CQ9RoundListRequest>               (onCQ9RoundListRequest);
            ReceiveAsync<CQ9RoundDetailRequest>             (onCQ9RoundDetailRequest);
            ReceiveAsync<CQ9SearchRoundRequest>             (onCQ9SearchRoundRequest);

            ReceiveAsync<HabaneroGetHistoryRequest>         (onHabaneroGetHistoryRequest);
            ReceiveAsync<HabaneroGetGameDetailRequest>      (onHabaneroGetGameDetailRequest);

            ReceiveAsync<PlaysonTransactionListRequest>     (onPlaysonTransactionListRequest);
            ReceiveAsync<PlaysonAggregateRequest>           (onPlaysonAggregateRequest);
            ReceiveAsync<PlaysonGetTransRequest>            (onPlaysonGetTransRequest);
            ReceiveAsync<PlaysonTransDetailRequest>         (onPlaysonTransDetailRequest);

        }

        public static Props Props(string strConnString, int poolSize)
        {
            return Akka.Actor.Props.Create(() => new DBProxyReader(strConnString)).WithRouter(new RoundRobinPool(poolSize));
        }

        private async Task onUserApiWithdraw(ApiWithdrawRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(this._strConnString))
                {
                    await connection.OpenAsync();
                    SqlCommand command = null;
                    if (request.Amount < 0L)
                    {
                        string strQuery = "UPDATE users SET balance=0, lastscorecounter=lastscorecounter+1 OUTPUT inserted.balance as after, deleted.balance as before WHERE agentid=@agentid and username=@username";
                        command = new SqlCommand(strQuery, connection);
                    }
                    else
                    {
                        string strQuery = "UPDATE users SET balance=balance-@amount, lastscorecounter=lastscorecounter+1 OUTPUT inserted.balance as after, deleted.balance as before WHERE agentid=@agentid and username=@username and balance >= @amount";
                        command = new SqlCommand(strQuery, connection);
                        command.Parameters.AddWithValue("@amount", request.Amount);
                    }

                    command.Parameters.AddWithValue("@agentid",     request.AgentID);
                    command.Parameters.AddWithValue("@username",    request.UserID);

                    SqlDataReader sqlDataReader = await command.ExecuteReaderAsync();
                    DbDataReader reader = sqlDataReader;
                    
                    try
                    {
                        if (await reader.ReadAsync())
                        {
                            double afterscore   = (double)(Decimal)reader["after"];
                            double beforescore  = (double)(Decimal)reader["before"];
                            
                            Sender.Tell(new ApiWithdrawResponse(0, beforescore, afterscore));
                        }
                        else
                        {
                            Sender.Tell(new ApiWithdrawResponse(1, 0.0, 0.0));
                        }
                    }
                    finally
                    {
                        reader?.Dispose();
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onUserApiWithdraw {0}", ex);
            }

            Sender.Tell(new ApiWithdrawResponse(2, 0.0, 0.0));
        }

        #region 로그인부분
        private async Task doLoginRequest(UserLoginRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(this._strConnString))
                {
                    await connection.OpenAsync();
                    string strQuery     = "SELECT * FROM users WHERE agentid=@agentid AND username=@username";
                    SqlCommand command  = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid", request.AgentID);
                    command.Parameters.AddWithValue("@username", request.UserID);

                    UserLoginResponse response = null;
                    LoginResult resultCode  = LoginResult.IDPASSWORDMISMATCH;
                    long    dbID                = 0;
                    string  strUserID           = "";
                    double  balance             = 0.0;
                    int     currency            = 0;
                    string  strAgentName        = "";
                    int     agentDBID           = 0;
                    string  strPasswordMD5      = null;
                    long    lastScoreCounter    = 0;

                    do
                    {
                        using (DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (!await reader.ReadAsync())
                                break;

                            dbID                = (long)reader["id"];
                            strUserID           = (string)reader["username"];
                            balance             = (double)(Decimal)reader["balance"];
                            currency            = (int)reader["currency"];
                            strAgentName        = (string)reader["agentname"];
                            agentDBID           = (int)reader["agentid"];
                            lastScoreCounter    = (long)reader["lastscorecounter"];
                            strPasswordMD5      = (string)reader["password"];

                            if (!MD5Utils.CompareMD5Hashes(request.Password, strPasswordMD5))
                                break;

                            int state = (int)reader["state"];
                            if (state != 1)
                            {
                                resultCode = LoginResult.ACCOUNTDISABLED;
                                break;
                            }
                        }
                    }
                    while (false);

                    response = new UserLoginResponse(agentDBID, dbID, strUserID, strPasswordMD5, balance, currency, lastScoreCounter);

                    if (response == null)
                        response = new UserLoginResponse(resultCode);

                    Sender.Tell(response);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::doLoginRequest {0}", ex);
            }
        }
        #endregion

        #region 보너스부분
        private async Task getUserBonusItems(GetUserBonusItems request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();
                    UserRangeOddEventItem userRangeOddEventItem = await fetchUserRangeOddEventItem(connection, request.AgentID, request.UserID);
                    
                    if (userRangeOddEventItem != null)
                        Sender.Tell(userRangeOddEventItem);
                }
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::getUserBonusItems {0}", ex);
            }
        }

        private async Task<UserRangeOddEventItem> fetchUserRangeOddEventItem(SqlConnection connection,int agentID ,string strUserID)
        {
            string strQuery = "SELECT TOP (1) id,  minodd, maxodd,maxbet FROM userrangeevents WHERE agentid=@agentid AND username=@username AND processed=0 ORDER BY id";
            SqlCommand command = new SqlCommand(strQuery, connection);
            command.Parameters.AddWithValue("@agentid",     agentID);
            command.Parameters.AddWithValue("@username",    strUserID);

            using (DbDataReader reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    long eventBonusID   = (long)reader["id"];
                    double minOdd       = (double)(decimal)reader["minodd"];
                    double maxOdd       = (double)(decimal)reader["maxodd"];
                    double maxBet       = (double)(decimal)reader["maxbet"];

                    UserRangeOddEventItem userEventItem = new UserRangeOddEventItem(eventBonusID, agentID, strUserID, minOdd, maxOdd, maxBet);
                    return userEventItem;
                }
            }
            return null;
        }
        
        private async Task fetchNewUserRangeEvent(ClaimedUserRangeEventMessage request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery = "SELECT id,  minodd, maxodd, maxbet FROM userrangeevents WHERE agentid=@agentid AND username=@username AND processed=0 AND id>@id";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid",     request.AgentID);
                    command.Parameters.AddWithValue("@username",    request.UserID);
                    command.Parameters.AddWithValue("@id",          request.ID);

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            long eventBonusID   = (long)reader["id"];
                            double minOdd       = (double)(decimal)reader["minodd"];
                            double maxOdd       = (double)(decimal)reader["maxodd"];
                            double maxBet       = (double)(decimal)reader["maxbet"];

                            UserRangeOddEventItem userEventItem = new UserRangeOddEventItem(eventBonusID, request.AgentID, request.UserID, minOdd, maxOdd, maxBet);
                            Sender.Tell(userEventItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::fetchNewUserRangeEvent {0}", ex);
            }
        }
        #endregion

        private bool splitGlobalUserID(string strGlobalUserID, out int agentID, out string strUserID)
        {
            agentID = 0;
            strUserID = null;

            try
            {
                int length = strGlobalUserID.IndexOf("_");
                if (length <= 0 || length >= strGlobalUserID.Length - 1)
                    return false;

                agentID = int.Parse(strGlobalUserID.Substring(0, length));
                strUserID = strGlobalUserID.Substring(length + 1);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private Dictionary<string, string> splitFormUrlEncoded(string strContent)
        {
            string[] strParts = strContent.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, string> dicReturn = new Dictionary<string, string>();
            for(int i = 0; i < strParts.Length; i++)
            {
                string[] strParamValues = strParts[i].Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

                if (strParamValues.Length == 1)
                    dicReturn.Add(strParamValues[0], null);
                else if(strParamValues.Length == 2)
                    dicReturn.Add(strParamValues[0], strParamValues[1]);
            }
            return dicReturn;
        }
        
        #region PP부분
        private async Task onRequestPPReplayList(HTTPPPReplayListRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    int     agentID     = 0;
                    string  strUserID   = "";

                    if (!splitGlobalUserID(request.UserID, out agentID, out strUserID))
                    {
                        Sender.Tell(new HTTPPPReplayListResponse(new List<PPGameHistoryTopListItem>()));
                        return;
                    }

                    List<PPGameHistoryTopListItem> items = new List<PPGameHistoryTopListItem>(); 
                    string strQuery = "SELECT id, bet, basebet,win, rtp, linktoken, playeddate FROM ppusertopgamelog WHERE agentid=@agentid AND username=@username AND gameid=@gameid ORDER BY playeddate DESC";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid",     agentID);
                    command.Parameters.AddWithValue("@username",    strUserID);
                    command.Parameters.AddWithValue("@gameid",      request.GameID);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string strLinkToken = null;
                            if (!(reader["linktoken"] is DBNull))
                                strLinkToken = string.Format("{0}?{1}", PragmaticConfig.Instance.ReplayURL, (string) reader["linktoken"]);

                            items.Add(new PPGameHistoryTopListItem((long)reader["id"], (double)(decimal)reader["bet"], (double)(decimal)reader["basebet"],
                                (double)(decimal)reader["win"], (double)(decimal)reader["rtp"], strLinkToken, (long)reader["playeddate"]));
                        }
                    }
                    Sender.Tell(new HTTPPPReplayListResponse(items));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onRequestPPReplayList {0}", ex);
                Sender.Tell(new HTTPPPReplayListResponse(new List<PPGameHistoryTopListItem>()));
            }
        }

        private async Task onRequestPPReplayData(HTTPPPReplayDataRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    SqlCommand command = null;
                    if (request.UserID != null)
                    {
                        string strQuery = "SELECT detaillog FROM ppusertopgamelog WHERE id=@id";
                        command = new SqlCommand(strQuery, connection);
                        command.Parameters.AddWithValue("@id", request.RoundID);
                    }
                    else
                    {
                        string strQuery = "SELECT detail FROM ppusercreatedlinks WHERE token=@token AND gameid=@gameid AND roundid=@roundid";
                        command = new SqlCommand(strQuery, connection);
                        command.Parameters.AddWithValue("@token",   request.Token);
                        command.Parameters.AddWithValue("@gameid",  request.GameID);
                        command.Parameters.AddWithValue("@roundid", request.RoundID);
                    }
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            Sender.Tell(new HTTPPPReplayDataResponse((string)reader[0]));
                            return;
                        }
                    }
                }
                Sender.Tell(new HTTPPPReplayDataResponse(null));

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onRequestPPReplayData {0}", ex);
                Sender.Tell(new HTTPPPReplayDataResponse(null));
            }
        }

        private async Task onRequestPPReplayMakeLink(HTTPPPReplayMakeLinkRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    List<PPGameHistoryTopListItem> items = new List<PPGameHistoryTopListItem>();

                    string strLinkToken = "replay" + MD5Utils.GetMD5Hash(string.Format("{0}_{1}_{2}", request.UserID, request.GameID, request.RoundID));
                    string sharedLink   = string.Format("token={0}&symbol={1}&envID={2}&roundID={3}&lang={4}", strLinkToken, request.Symbol, request.EnvID, request.RoundID, request.Lang);

                    string strQuery     = "UPDATE ppusertopgamelog SET linktoken=@linktoken OUTPUT INSERTED.detaillog, DELETED.linktoken WHERE id=@id";
                    SqlCommand command  = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@id",          request.RoundID);
                    command.Parameters.AddWithValue("@linktoken",   sharedLink);

                    string strDetailLog         = null;
                    string strBeforeLinkToken   = null;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            strDetailLog = (string)reader["detaillog"];
                            if (!(reader["linktoken"] is DBNull))
                                strBeforeLinkToken = (string) reader["linktoken"];
                        }
                    }

                    if (strDetailLog == null)
                    {
                        Sender.Tell(new HTTPPPReplayMakeLinkResponse("{\"error\":10,\"description\":\"Request is not authorized\"}"));
                        return;
                    }

                    if(strBeforeLinkToken != null)
                    {
                        strQuery = "UPDATE ppusertopgamelog SET linktoken=@linktoken WHERE id=@id";
                        command = new SqlCommand(strQuery, connection);
                        command.Parameters.AddWithValue("@id",          request.RoundID);
                        command.Parameters.AddWithValue("linktoken",    strBeforeLinkToken);
                        await command.ExecuteNonQueryAsync();

                        sharedLink = strBeforeLinkToken;
                    }
                    else
                    {
                        strQuery = "INSERT INTO ppusercreatedlinks (token, gameid, roundid, detail, createdtime) VALUES(@token, @gameid, @roundid,@detail,@createdtime)";
                        command = new SqlCommand(strQuery, connection);
                        command.Parameters.AddWithValue("@token", strLinkToken);
                        command.Parameters.AddWithValue("@gameid", request.GameID);
                        command.Parameters.AddWithValue("@roundid", request.RoundID);
                        command.Parameters.AddWithValue("@detail", strDetailLog);
                        command.Parameters.AddWithValue("@createdtime", DateTime.UtcNow);

                        await command.ExecuteNonQueryAsync();
                    }

                    string strReponse = string.Format("{{\"error\":0, \"description\":\"OK\",\"sharedLink\":\"{0}?{1}\"}}", PragmaticConfig.Instance.ReplayURL, sharedLink);
                    Sender.Tell(new HTTPPPReplayMakeLinkResponse(strReponse));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onRequestPPReplayData {0}", ex);
                Sender.Tell(new HTTPPPReplayMakeLinkResponse(""));
            }
        }
        
        private async Task onRequestPPHistoryList(HTTPPPHistoryGetLastItemsRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    List<PPGameRecentHistoryItem> items = new List<PPGameRecentHistoryItem>();
                    
                    int     agentID     = 0;
                    string  strUserID   = "";
                    
                    if (!splitGlobalUserID(request.UserID, out agentID, out strUserID))
                    {
                        Sender.Tell(new List<PPGameRecentHistoryItem>());
                        return;
                    }

                    string strQuery = "SELECT roundid, balance, bet, win, timestamp FROM ppuserrecentgamelog WHERE agentid=@agentid AND username=@username AND gameid=@gameid ORDER BY timestamp DESC";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid",     agentID);
                    command.Parameters.AddWithValue("@username",    strUserID);
                    command.Parameters.AddWithValue("@gameid",      request.GameID);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PPGameRecentHistoryItem item = new PPGameRecentHistoryItem();
                            item.roundId    = (string)reader["roundid"];
                            item.balance    = Math.Round((double)(decimal)reader["balance"], 2).ToString();
                            item.bet        = Math.Round((double)(decimal)reader["bet"], 2).ToString();
                            item.win        = Math.Round((double)(decimal)reader["win"], 2).ToString();
                            item.dateTime   = (long)reader["timestamp"];
                            items.Add(item);
                        }
                    }
                    Sender.Tell(items);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onRequestPPHistoryList {0}", ex);
                Sender.Tell(new List<PPGameRecentHistoryItem>());
            }
        }

        private async Task onRequestPPHistoryData(HTTPPPHistoryGetItemDetailRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string      strQuery    = "SELECT agentid, username, id, detaillog FROM ppuserrecentgamelog WHERE roundid=@roundid";
                    SqlCommand  command     = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@roundid", request.RoundID);

                    int agentid         = 0;
                    string strUserName  = "";
                    string strDetailLog = "";
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            agentid         = (int)reader["agentid"];
                            strUserName     = (string)reader["username"];
                            strDetailLog    = (string)reader["detaillog"];
                        }
                    }

                    int currency = 0;
                    strQuery = "SELECT currency FROM users WHERE agentid=@agentid AND username=@username";
                    SqlCommand command1     = new SqlCommand(strQuery, connection);
                    command1.Parameters.AddWithValue("@agentid",    agentid);
                    command1.Parameters.AddWithValue("@username",   strUserName);

                    using(DbDataReader reader = await command1.ExecuteReaderAsync())
                    {
                        if(await reader.ReadAsync())
                        {
                            currency = (int)reader["currency"];
                        }
                    }

                    List<PPHistoryItemDetail> detailLogs        = JsonConvert.DeserializeObject<List<PPHistoryItemDetail>>(strDetailLog);
                    List<PPGameRecentHistoryDetailItem> items   = new List<PPGameRecentHistoryDetailItem>();

                    for(int i = 0; i < detailLogs.Count; i++)
                    {
                        PPGameRecentHistoryDetailItem item = new PPGameRecentHistoryDetailItem();
                        item.currency       = _currencyInfo[currency].CurrencyText;
                        item.currencySymbol = _currencyInfo[currency].CurrencySymbol;
                        item.roundId        = request.RoundID;
                        item.request        = splitFormUrlEncoded(detailLogs[i].cr);
                        item.response       = splitFormUrlEncoded(detailLogs[i].sr);
                        items.Add(item);
                    }

                    Sender.Tell(items);
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onRequestPPHistoryData {0} {1} {2} {3}", ex, request.UserID, request.GameID, request.RoundID);
                Sender.Tell(new List<PPGameRecentHistoryDetailItem>());
            }
        }
        
        private async Task onRequestPPVerifyItem(HTTPPPVerifyGetLastItemRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    int     agentID     = 0;
                    string  strUserID   = "";
                    
                    if (!splitGlobalUserID(request.UserID, out agentID, out strUserID))
                    {
                        Sender.Tell(null);
                        return;
                    }

                    PPGameVerifyDetail item = new PPGameVerifyDetail();
                    string strQuery     = "SELECT TOP 1 l.id, l.bet, l.timestamp, c.gamename, c.gamesymbol FROM ppuserrecentgamelog l LEFT JOIN gameconfigs c ON l.gameid=c.gameid WHERE l.agentid=@agentid AND l.username=@username ORDER BY l.timestamp DESC";
                    SqlCommand command  = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid",     agentID);
                    command.Parameters.AddWithValue("@username",    strUserID);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            item.id         = (long)reader["id"];
                            item.date       = (long)reader["timestamp"];
                            item.betAmount  = Math.Round((double)(decimal)reader["bet"], 2);
                            item.name       = (string)reader["gamename"];
                            item.symbol     = (string)reader["gamesymbol"];
                            Sender.Tell(item);
                        }
                        else
                        {
                            Sender.Tell(null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onRequestPPVerifyItem {0}", ex);
                Sender.Tell(null);
            }
        }
        #endregion

        #region BNG 부분
        private async Task onBNGTransDetailRequest(BNGTransDetailRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery    = "SELECT TOP 1 detail, gameid, roundid FROM bnggamehistory WHERE transactionid=@transid";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@transid", request.TransID);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string strGameSymbol = DBMonitorSnapshot.Instance.getGameSymbolFromID(GAMETYPE.BNG, (int)reader["gameid"]);
                            string strDrawVersion = DBMonitorSnapshot.Instance.getBNGGameDrawVer(strGameSymbol);

                            Sender.Tell(new BNGTransDetailResponse((int)reader["gameid"], (string)reader["detail"], (string)reader["roundid"], strGameSymbol, strDrawVersion));
                            return;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onRequestPPHistoryList {0}", ex);
            }
            Sender.Tell(new BNGTransDetailResponse(0, null, "", "", ""));
        }
        
        private async Task onBNGAggregateRequest(BNGAggregateRequest request)
        {
            BNGAggregateResponse response = new BNGAggregateResponse();

            try
            {
                int     agentID     = 0;
                string  strUserID   = "";

                if (!splitGlobalUserID(request.player_id, out agentID, out strUserID))
                {
                    Sender.Tell(response);
                    return;
                }

                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery = "SELECT SUM(bet) as bets, SUM(win) as wins, COUNT(id) as count, COUNT( DISTINCT roundid) as roundcount FROM bnggamehistory WHERE agentid=@agentid AND userid=@userid AND gameid=@gameid";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid", agentID);
                    command.Parameters.AddWithValue("@userid", strUserID);
                    command.Parameters.AddWithValue("@gameid", int.Parse(request.game_id));

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            if((int)reader["count"] > 0)
                            {
                                response.transactions   = ((int)reader["count"]).ToString();
                                response.rounds         = ((int)reader["roundcount"]).ToString();

                                double bets = (double)(decimal)reader["bets"];
                                double wins = (double)(decimal)reader["wins"];

                                response.bets = Math.Round(bets, 2).ToString();
                                response.wins = Math.Round(wins, 2).ToString();
                                response.outcome = Math.Round(wins - bets, 2).ToString();
                                response.profit = Math.Round(wins - bets, 2).ToString();

                                if (bets > 0.0)
                                    response.payout = Math.Round(wins / bets * 100.0, 2).ToString();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onRequestPPHistoryList {0}", ex);
            }
            Sender.Tell(response);
        }

        private async Task onBNGTransactionListRequest(BNGTransactionListRequest request)
        {
            List<string> items = new List<string>();
            string strLastTime = "";

            try
            {
                int     agentID     = 0;
                string  strUserID   = "";

                if (!splitGlobalUserID(request.player_id, out agentID, out strUserID))
                {
                    Sender.Tell(string.Format("{{\"fetch_state\":\"{0}\", \"items\":[{1}]}}", strLastTime, string.Join(",", items)));
                    return;
                }
                
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery = "";
                    if (string.IsNullOrEmpty(request.fetch_state))
                        strQuery = string.Format("SELECT Top {0} time, overview FROM bnggamehistory WHERE agentid=@agentid AND userid=@userid AND gameid=@gameid ORDER BY time DESC", request.fetch_size);
                    else
                        strQuery = string.Format("SELECT Top {0} time, overview FROM bnggamehistory WHERE agentid=@agentid AND userid=@userid AND gameid=@gameid and time < @time ORDER BY time DESC", request.fetch_size);

                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid", agentID);
                    command.Parameters.AddWithValue("@userid", strUserID);
                    command.Parameters.AddWithValue("@gameid", int.Parse(request.game_id));
                    if (!string.IsNullOrEmpty(request.fetch_state))
                        command.Parameters.AddWithValue("@time", DateTime.Parse(request.fetch_state));

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            strLastTime = ((DateTime)reader["time"]).ToString();
                            items.Add((string)reader["overview"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onBNGTransactionListRequest {0}", ex);
            }
            string strResponse = string.Format("{{\"fetch_state\":\"{0}\", \"items\":[{1}]}}", strLastTime, string.Join(",", items));
            Sender.Tell(strResponse);
        }
        #endregion

        #region CQ9 부분
        private async Task onCQ9RoundDetailRequest(CQ9RoundDetailRequest request)
        {
            try
            {
                int     agentID     = 0;
                string  strUserID   = "";

                if (!splitGlobalUserID(request.GlobalUserID, out agentID, out strUserID))
                {
                    Sender.Tell("");
                    return;
                }
                
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery = "SELECT detail FROM cq9gamehistory WHERE agentid=@agentid AND userid=@userid AND roundid=@roundid";

                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid", agentID);
                    command.Parameters.AddWithValue("@userid", strUserID);
                    command.Parameters.AddWithValue("@roundid", request.RoundID);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            Sender.Tell((string)reader["detail"]);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onCQ9RoundDetailRequest {0}", ex);
            }
            Sender.Tell("");
        }
        
        private async Task onCQ9SearchRoundRequest(CQ9SearchRoundRequest request)
        {
            try
            {
                int     agentID     = 0;
                string  strUserID   = "";

                if (!splitGlobalUserID(request.GlobalUserID, out agentID, out strUserID))
                {
                    Sender.Tell("");
                    return;
                }
                
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery = "SELECT TOP 1 overview FROM cq9gamehistory WHERE agentid=@agentid AND userid=@userid AND roundid=@roundid";

                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid", agentID);
                    command.Parameters.AddWithValue("@userid", strUserID);
                    command.Parameters.AddWithValue("@roundid", request.RoundID);

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            Sender.Tell((string)reader["overview"]);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onCQ9SearchRoundRequest {0}", ex);
            }
            Sender.Tell("");
        }
        
        private async Task onCQ9RoundListRequest(CQ9RoundListRequest request)
        {
            List<string> items = new List<string>();
            try
            {
                int     agentID     = 0;
                string  strUserID   = "";

                if (!splitGlobalUserID(request.GlobalUserID, out agentID, out strUserID))
                {
                    Sender.Tell(new List<string>());
                    return;
                }
                
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery = "SELECT overview FROM cq9gamehistory WHERE agentid=@agentid AND userid=@userid AND time >= @begintime AND time <= @endtime ORDER BY time DESC OFFSET @offset ROWS FETCH NEXT @count ROWS ONLY";

                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid",     agentID);
                    command.Parameters.AddWithValue("@userid",      strUserID);
                    command.Parameters.AddWithValue("@begintime",   request.BeginTime);
                    command.Parameters.AddWithValue("@endtime",     request.EndTime);
                    command.Parameters.AddWithValue("@offset",      request.Offset);
                    command.Parameters.AddWithValue("@count",       request.Count);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            items.Add((string)reader["overview"]);
                        }
                    }
                }
                Sender.Tell(items);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onCQ9RoundListRequest {0}", ex);
                Sender.Tell(new List<string>());
            }
        }
        #endregion

        #region Habanero 부분
        private async Task onHabaneroGetHistoryRequest(HabaneroGetHistoryRequest request)
        {
            List<string> items = new List<string>();
            try
            {
                int     agentID     = 0;
                string  strUserID   = "";

                if (!splitGlobalUserID(request.GlobalUserID, out agentID, out strUserID))
                {
                    Sender.Tell(new List<string>());
                    return;
                }
                
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery = "SELECT overview FROM habanerogamehistory WHERE agentid=@agentid AND userid=@userid AND time >= @begintime AND time <= @endtime ORDER BY time DESC";

                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid",     agentID);
                    command.Parameters.AddWithValue("@userid",      strUserID);
                    command.Parameters.AddWithValue("@begintime",   request.BeginTime);
                    command.Parameters.AddWithValue("@endtime",     request.EndTime);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            items.Add((string)reader["overview"]);
                        }
                    }
                }
                Sender.Tell(items);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onHabaneroGetHistoryRequest {0}", ex);
                Sender.Tell(new List<string>());
            }
        }
        
        private async Task onHabaneroGetGameDetailRequest(HabaneroGetGameDetailRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery = "SELECT detail FROM habanerogamehistory WHERE gamelogid=@gamelogid";

                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@gamelogid", request.GameInstanceId);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            Sender.Tell((string)reader["detail"]);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onHabaneroGetGameDetailRequest {0}", ex);
            }
            Sender.Tell("");
        }
        #endregion

        #region Playson 부분
        private async Task onPlaysonTransDetailRequest(PlaysonTransDetailRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery = "SELECT TOP 1 bet,win,detail, gameid, roundid,overview FROM playsongamehistory WHERE roundid=@roundid";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@roundid", request.TransID);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string strGameSymbol    = DBMonitorSnapshot.Instance.getGameSymbolFromID(GAMETYPE.PLAYSON, (int)reader["gameid"]);
                            dynamic overView        = JsonConvert.DeserializeObject((string)reader["overview"]);

                            PlaysonTransDetailItem detailItem = new PlaysonTransDetailItem();

                            detailItem.bet          = (decimal)reader["bet"];
                            detailItem.cash         = (decimal)overView["balance_after"];
                            detailItem.currency     = (string)overView["currency"];
                            detailItem.datetime     = (string)overView["c_at"];
                            detailItem.entergame    = "";
                            detailItem.exitgame     = "";
                            detailItem.gameid       = Convert.ToInt32((string)overView["game_id"]);
                            detailItem.gametitle    = strGameSymbol;
                            detailItem.id           = Convert.ToInt64(overView["round_id"]);
                            detailItem.info         = (string)reader["detail"];
                            detailItem.platform     = "desk";
                            detailItem.roundnum     = overView["round_id"];
                            detailItem.totalcount   = 1.ToString();
                            detailItem.userid       = (string)overView["player_id"];
                            detailItem.win          = Convert.ToInt64((decimal)reader["win"]);
                            detailItem.wlcode       = "star";

                            List<PlaysonTransDetailItem> detailList = new List<PlaysonTransDetailItem>();
                            detailList.Add(detailItem);

                            Sender.Tell(new PlaysonTransDetailResponse(1, "EUR", "ok", detailList));
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onPlaysonTransDetailRequest {0}", ex);
            }
            Sender.Tell(new PlaysonTransDetailResponse(0, "EUR", "ok",new List<PlaysonTransDetailItem>()));
        }
        private async Task onPlaysonGetTransRequest(PlaysonGetTransRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery = "SELECT TOP 1 roundid FROM playsongamehistory WHERE transactionid=@transid";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@transid", request.TransID);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string roundid = (string)reader["roundid"];
                            Sender.Tell(new PlaysonGetTransResponse(roundid));
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onPlaysonGetTransRequest {0}", ex);
            }
            Sender.Tell(new PlaysonGetTransResponse(""));
        }
        private async Task onPlaysonAggregateRequest(PlaysonAggregateRequest request)
        {
            PlaysonAggregateResponse response = new PlaysonAggregateResponse();

            try
            {
                int     agentID     = 0;
                string  strUserID   = "";

                if (!splitGlobalUserID(request.player_id, out agentID, out strUserID))
                {
                    Sender.Tell(response);
                    return;
                }
                
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery = "SELECT SUM(bet) as bets, SUM(win) as wins, COUNT(id) as count, COUNT( DISTINCT roundid) as roundcount FROM playsongamehistory WHERE agentid=@agentid AND userid=@userid AND gameid=@gameid";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid", agentID);
                    command.Parameters.AddWithValue("@userid", strUserID);
                    command.Parameters.AddWithValue("@gameid", int.Parse(request.game_id));

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            if ((int)reader["count"] > 0)
                            {
                                response.transactions = ((int)reader["count"]).ToString();
                                response.rounds = ((int)reader["roundcount"]).ToString();

                                double bets = (double)(decimal)reader["bets"];
                                double wins = (double)(decimal)reader["wins"];

                                response.bets       = Math.Round(bets, 2).ToString();
                                response.wins       = Math.Round(wins, 2).ToString();
                                response.outcome    = Math.Round(wins - bets, 2).ToString();
                                response.profit     = Math.Round(wins - bets, 2).ToString();

                                if (bets > 0.0)
                                    response.payout = Math.Round(wins / bets * 100.0, 2).ToString();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onPlaysonAggregateRequest {0}", ex);
            }
            Sender.Tell(response);
        }
        private async Task onPlaysonTransactionListRequest(PlaysonTransactionListRequest request)
        {
            List<string> items = new List<string>();
            string strLastTime = "";

            try
            {
                int     agentID     = 0;
                string  strUserID   = "";

                if (!splitGlobalUserID(request.player_id, out agentID, out strUserID))
                {
                    Sender.Tell(string.Format("{{\"fetch_state\":\"{0}\", \"items\":[{1}]}}", strLastTime, string.Join(",", items)));
                    return;
                }
                
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery = "";
                    if (string.IsNullOrEmpty(request.fetch_state))
                        strQuery = string.Format("SELECT Top {0} time, overview FROM playsongamehistory WHERE agentid=@agentid AND userid=@userid AND gameid=@gameid ORDER BY time DESC", request.fetch_size);
                    else
                        strQuery = string.Format("SELECT Top {0} time, overview FROM playsongamehistory WHERE agentid=@agentid AND userid=@userid AND gameid=@gameid AND time < @time ORDER BY time DESC", request.fetch_size);

                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid", agentID);
                    command.Parameters.AddWithValue("@userid", strUserID);
                    command.Parameters.AddWithValue("@gameid", int.Parse(request.game_id));
                    if (!string.IsNullOrEmpty(request.fetch_state))
                        command.Parameters.AddWithValue("@time", DateTime.Parse(request.fetch_state));

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            strLastTime = ((DateTime)reader["time"]).ToString();
                            items.Add((string)reader["overview"]);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onPlaysonTransactionListRequest {0}", ex);
            }
            string strResponse = string.Format("{{\"fetch_state\":\"{0}\", \"items\":[{1}]}}", strLastTime, string.Join(",", items));
            Sender.Tell(strResponse);
        }
        #endregion
    }

    public class PPHistoryItemDetail
    {
        public string cr { get; set; }  //client request
        public string sr { get; set; }  //server response
    }

    public class CurrencyObj
    {
        public string   CurrencyText    { get; set; }
        public string   CurrencySymbol  { get; set; }
        public int      Rate            { get; set; }
    }
}
