using Akka.Actor;
using Akka.Event;
using Akka.Routing;
using GITProtocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace UserNode.Database
{
    public class DBProxyReader : ReceiveActor
    {
        private readonly ILoggingAdapter    _logger         = Context.GetLogger();
        private string                      _strConnString  = "";

        public DBProxyReader(string strConnString)
        {
            _strConnString = strConnString;

            ReceiveAsync<HTTPPPReplayListRequest>           (onRequestPPReplayList);
            ReceiveAsync<HTTPPPReplayDataRequest>           (onRequestPPReplayData);
            ReceiveAsync<HTTPPPHistoryGetLastItemsRequest>  (onRequestPPHistoryList);
            ReceiveAsync<HTTPPPHistoryGetItemDetailRequest> (onRequestPPHistoryData);
            ReceiveAsync<HTTPPPReplayMakeLinkRequest>       (onRequestPPReplayMakeLink);
            ReceiveAsync<UserOfflineStateItem>              (onUserOfflineUpdate);
        }

        public static Props Props(string strConnString, int poolSize)
        {
            return Akka.Actor.Props.Create(() => new DBProxyReader(strConnString)).WithRouter(new RoundRobinPool(poolSize));
        }

        private async Task onUserOfflineUpdate(UserOfflineStateItem request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string      strQuery    = "UPDATE users SET balance=balance+@balanceupdate, isonline=0, lastgameid=@lastgameid WHERE id=@id";
                    SqlCommand  command     = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@id",              request.PlayerID);
                    command.Parameters.AddWithValue("@balanceupdate",   request.BalanceIncrement);
                    command.Parameters.AddWithValue("@lastgameid",      request.GameID);
                    await command.ExecuteNonQueryAsync();
                    
                    Sender.Tell(true);
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onUserOfflineUpdate {0}", ex);
            }
            Sender.Tell(false);
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
                    string sharedLink   = "";
                    string strQuery     = "SELECT linktoken, detaillog, currency FROM ppusertopgamelog WHERE id=@id";
                    SqlCommand command  = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@id", request.RoundID);

                    string strDetailLog         = null;
                    string strBeforeLinkToken   = null;
                    string strCurrency          = null;
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            strDetailLog    = (string)reader["detaillog"];
                            strCurrency     = (string)reader["currency"];
                            if (!(reader["linktoken"] is DBNull))
                                strBeforeLinkToken = (string)reader["linktoken"];
                        }
                    }

                    if (strDetailLog == null)
                    {
                        Sender.Tell(new HTTPPPReplayMakeLinkResponse("{\"error\":10,\"description\":\"Request is not authorized\"}"));
                        return;
                    }

                    if (strBeforeLinkToken != null)
                        sharedLink = strBeforeLinkToken;
                    else
                        sharedLink = string.Format("token={0}&symbol={1}&envID={2}&roundID={3}&currency={4}&lang={5}", strLinkToken, request.Symbol, request.EnvID, request.RoundID, strCurrency, request.Lang);
                    
                    if (strBeforeLinkToken == null)
                    {
                        strQuery    = "UPDATE ppusertopgamelog SET linktoken=@linktoken WHERE id=@id";
                        command     = new SqlCommand(strQuery, connection);
                        command.Parameters.AddWithValue("@id",          request.RoundID);
                        command.Parameters.AddWithValue("linktoken",    sharedLink);
                        await command.ExecuteNonQueryAsync();
                        
                        strQuery    = "INSERT INTO ppusercreatedlinks (token, gameid, roundid, detail, createdtime) VALUES(@token, @gameid, @roundid,@detail,@createdtime)";
                        command     = new SqlCommand(strQuery, connection);
                        command.Parameters.AddWithValue("@token",       strLinkToken);
                        command.Parameters.AddWithValue("@gameid",      request.GameID);
                        command.Parameters.AddWithValue("@roundid",     request.RoundID);
                        command.Parameters.AddWithValue("@detail",      strDetailLog);
                        command.Parameters.AddWithValue("@createdtime", DateTime.UtcNow);
                        await command.ExecuteNonQueryAsync();
                    }

                    sharedLink          = sharedLink + "&lang=" + request.Lang;
                    string strReponse   = string.Format("{{\"error\":0, \"description\":\"OK\",\"sharedLink\":\"{0}?{1}\"}}", PragmaticConfig.Instance.ReplayURL, sharedLink);
                    
                    Sender.Tell(new HTTPPPReplayMakeLinkResponse(strReponse));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onRequestPPReplayData {0}", ex);
                Sender.Tell(new HTTPPPReplayMakeLinkResponse(""));
            }
        }

        private Dictionary<string, string> splitFormUrlEncoded(string strContent)
        {
            string[] strParams = strContent.Split(new string[1]{"&"}, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, string> dicParams = new Dictionary<string, string>();
            
            for (int i = 0; i < strParams.Length; i++)
            {
                string[] strParts = strParams[i].Split(new string[1] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (strParts.Length == 1)
                    dicParams.Add(strParts[0], null);
                else if (strParts.Length == 2)
                    dicParams.Add(strParts[0], strParts[1]);
            }
            return dicParams;
        }

        private async Task onRequestPPHistoryData(HTTPPPHistoryGetItemDetailRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();
                    string      strQuery    = "SELECT id, detaillog, currency FROM ppuserrecentgamelog WHERE id=@id";
                    SqlCommand  command     = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@id", request.RoundID);

                    using(DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string strDetailLog = (string)reader["detaillog"];
                            string strCurrency  = (string)reader["currency"];
                            List<PPHistoryItemDetail>           detailLogs  = JsonConvert.DeserializeObject<List<PPHistoryItemDetail>>(strDetailLog);
                            List<PPGameRecentHistoryDetailItem> items       = new List<PPGameRecentHistoryDetailItem>();
                            for (int i = 0; i < detailLogs.Count; i++)
                            {
                                PPGameRecentHistoryDetailItem item = new PPGameRecentHistoryDetailItem();
                                item.currency       = strCurrency;
                                item.currencySymbol = convertCurrencyToSymbol(strCurrency);
                                item.roundId        = request.RoundID;
                                item.request        = splitFormUrlEncoded(detailLogs[i].cr);
                                item.response       = splitFormUrlEncoded(detailLogs[i].sr);
                                items.Add(item);
                            }
                            Sender.Tell(items);
                            return;
                        }
                    }
                }
                Sender.Tell(new List<PPGameRecentHistoryDetailItem>());
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onRequestPPHistoryData {0} {1} {2} {3}", ex, request.UserID, request.GameID, request.RoundID);
                Sender.Tell(new List<PPGameRecentHistoryDetailItem>());
            }
        }

        private string convertCurrencyToSymbol(string strCurrency)
        {
            string symbol;
            switch (strCurrency)
            {
                case "USD":
                    symbol = "$";
                    break;
                case "EUR":
                    symbol = "€";
                    break;
                case "KRW":
                    symbol = "₩";
                    break;
                case "TND":
                    symbol = "TND";
                    break;
                default:
                    symbol = "$";
                    break;
            }
            return symbol;
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
                        string strQuery = "SELECT detail FROM ppusercreatedlinks WHERE token=@token and gameid=@gameid and roundid=@roundid";
                        command = new SqlCommand(strQuery, connection);
                        command.Parameters.AddWithValue("@token",   request.Token);
                        command.Parameters.AddWithValue("@gameid",  request.GameID);
                        command.Parameters.AddWithValue("@roundid", request.RoundID);
                    }

                    using(var reader = await command.ExecuteReaderAsync())
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

        private bool splitGlobalUserID(string strGlobalUserID, out int agentID, out string strUserID)
        {
            agentID     = 0;
            strUserID   = null;
            try
            {
                int length = strGlobalUserID.IndexOf("_");
                if (length <= 0 || length >= strGlobalUserID.Length - 1)
                    return false;

                agentID     = int.Parse(strGlobalUserID.Substring(0, length));
                strUserID   = strGlobalUserID.Substring(length + 1);
                return true;
            }
            catch (Exception ex)
            {
                return false;
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

                    string strQuery     = "SELECT id, balance, bet, win, timestamp,currency FROM ppuserrecentgamelog WHERE agentid=@agentid and username=@username and gameid=@gameid ORDER BY timestamp DESC";
                    SqlCommand command  = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid", agentID);
                    command.Parameters.AddWithValue("@username",strUserID);
                    command.Parameters.AddWithValue("@gameid",  request.GameID);

                    using (var reader = await command.ExecuteReaderAsync()) 
                    {
                        while (await reader.ReadAsync())
                        {
                            PPGameRecentHistoryItem item = new PPGameRecentHistoryItem();
                            item.roundId        = (long)reader["id"];
                            item.balance        = Math.Round((double)(Decimal)reader["balance"], 2).ToString();
                            item.bet            = Math.Round((double)(Decimal)reader["bet"], 2).ToString();
                            item.win            = Math.Round((double)(Decimal)reader["win"], 2).ToString();
                            item.currency       = (string)reader["currency"];
                            item.currencySymbol = convertCurrencyToSymbol(item.currency);
                            item.dateTime       = (long)reader["timestamp"];
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
                    string strQuery     = "SELECT id, bet, basebet,win, rtp, linktoken, playeddate FROM ppusertopgamelog WHERE agentid=@agentid and username=@username and gameid=@gameid ORDER BY playeddate DESC";
                    SqlCommand command  = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid",     agentID);
                    command.Parameters.AddWithValue("@username",    strUserID);
                    command.Parameters.AddWithValue("@gameid",      request.GameID);

                    using(var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string strLinkToken = null;
                            if (!(reader["linktoken"] is DBNull))
                                strLinkToken = string.Format("{0}?{1}", PragmaticConfig.Instance.ReplayURL, (string)reader["linktoken"]);
                            
                            items.Add(new PPGameHistoryTopListItem((long)reader["id"], (double)(Decimal)reader["bet"], (double)(Decimal)reader["basebet"], (double)(Decimal)reader["win"], (double)(Decimal)reader["rtp"], strLinkToken, (long)reader["playeddate"]));
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
    }

    public class PPHistoryItemDetail
    {
        public string cr        { get; set; }
        public string sr        { get; set; }
        public string currency  { get; set; }
    }
}
