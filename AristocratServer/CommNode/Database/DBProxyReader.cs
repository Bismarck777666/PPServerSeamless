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


namespace CommNode.Database
{
    public class DBProxyReader : ReceiveActor
    {
        private readonly ILoggingAdapter _logger = Logging.GetLogger(Context);
        private string _strConnString = "";
        public DBProxyReader(string strConnString)
        {
            _strConnString = strConnString;

            ReceiveAsync<UserLoginRequest>(doLoginRequest);
            ReceiveAsync<ApiUserLoginRequest>(doApiLoginRequest);
            ReceiveAsync<HTTPPPReplayListRequest>(onRequestPPReplayList);
            ReceiveAsync<HTTPPPReplayDataRequest>(onRequestPPReplayData);
            ReceiveAsync<HTTPPPHistoryGetLastItemsRequest>(onRequestPPHistoryList);
            ReceiveAsync<HTTPPPHistoryGetByRoundRequest>(onRequestGameLogByRoundID);
            ReceiveAsync<HTTPPPHistoryGetItemDetailRequest>(onRequestPPHistoryData);
            ReceiveAsync<HTTPPPHistoryGetItemDetailRequestByRoundID>(onRequestPPHistoryDataByRoundID);
            ReceiveAsync<HTTPPPReplayMakeLinkRequest>(onRequestPPReplayMakeLink);

            ReceiveAsync<PPTourLeaderUpdateItem>(doUpdatePPTournamentLeader);
            ReceiveAsync<GetPPFreeSpinReportsRequest>(onGetPPFreeSpinReports);
            ReceiveAsync<GetUserReceivedCashbacks>(onGetPPCashbackReports);
            
        }

        public static Props Props(string strConnString, int poolSize)
        {
            return Akka.Actor.Props.Create(() => new DBProxyReader(strConnString)).WithRouter(new RoundRobinPool(poolSize));
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
                    string sharedLink = string.Format("token={0}&symbol={1}&envID={2}&roundID={3}", strLinkToken, request.Symbol, request.EnvID, request.RoundID);

                    string strQuery = "UPDATE ppusertopgamelog SET linktoken=@linktoken OUTPUT INSERTED.detaillog, DELETED.linktoken WHERE id=@id";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@id", request.RoundID);
                    command.Parameters.AddWithValue("@linktoken", sharedLink);
                    string strDetailLog = null;
                    string strBeforeLinkToken = null;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            strDetailLog = (string)reader["detaillog"];
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
                    {
                        strQuery = "UPDATE ppusertopgamelog SET linktoken=@linktoken WHERE id=@id";
                        command = new SqlCommand(strQuery, connection);
                        command.Parameters.AddWithValue("@id", request.RoundID);
                        command.Parameters.AddWithValue("linktoken", strBeforeLinkToken);
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
        private Dictionary<string, string> splitFormUrlEncoded(string strContent)
        {
            string[] strParts = strContent.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, string> dicReturn = new Dictionary<string, string>();
            for (int i = 0; i < strParts.Length; i++)
            {
                string[] strParamValues = strParts[i].Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

                if (strParamValues.Length == 1)
                    dicReturn.Add(strParamValues[0], null);
                else if (strParamValues.Length == 2)
                    dicReturn.Add(strParamValues[0], strParamValues[1]);
            }
            return dicReturn;
        }

        private async Task onRequestPPHistoryDataByRoundID(HTTPPPHistoryGetItemDetailRequestByRoundID request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery = string.Format("SELECT id,  gamelog FROM gamelog_{0} WHERE id=@id", request.AgentID);
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@id", request.RoundID);

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string strDetailLog = (string)reader["gamelog"];

                            List<PPHistoryItemDetail> detailLogs = new List<PPHistoryItemDetail>();
                            try
                            {
                                detailLogs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PPHistoryItemDetail>>(strDetailLog);
                            }
                            catch
                            {

                            }
                            List<PPGameRecentHistoryDetailItem> items = new List<PPGameRecentHistoryDetailItem>();
                            for (int i = 0; i < detailLogs.Count; i++)
                            {
                                PPGameRecentHistoryDetailItem item = new PPGameRecentHistoryDetailItem();
                                if (request.Currency == "0")
                                {
                                    item.currency = "MYR";
                                    item.currencySymbol = "RM";
                                }
                                else if (request.Currency == "1")
                                {
                                    item.currency = "SGD";
                                    item.currencySymbol = "$";
                                }
                                else if (request.Currency == "2")
                                {
                                    item.currency = "AUD";
                                    item.currencySymbol = "A$";
                                }
                                else if (request.Currency == "3")
                                {
                                    item.currency = "THB";
                                    item.currencySymbol = "฿";
                                }
                                else if (request.Currency == "4")
                                {
                                    item.currency = "USD";
                                    item.currencySymbol = "$";
                                }
                                else if (request.Currency == "5")
                                {
                                    item.currency = "MMK";
                                    item.currencySymbol = "MMK";
                                }
                                else if (request.Currency == "6")
                                {
                                    item.currency = "HKD";
                                    item.currencySymbol = "$";
                                }
                                else if (request.Currency == "7")
                                {
                                    item.currency = "IDR";
                                    item.currencySymbol = "Rp";
                                }
                                else if (request.Currency == "8")
                                {
                                    item.currency = "BDT";
                                    item.currencySymbol = "BDT";
                                }
                                else if (request.Currency == "9")
                                {
                                    item.currency = "INR";
                                    item.currencySymbol = "INR";
                                }
                                else if (request.Currency == "10")
                                {
                                    item.currency = "CNY";
                                    item.currencySymbol = "¥";
                                }

                                item.roundId = request.RoundID;
                                item.request = splitFormUrlEncoded(detailLogs[i].cr);
                                item.response = splitFormUrlEncoded(detailLogs[i].sr);
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
                _logger.Error("Exception has been occured in DBProxyReader::onRequestPPHistoryData {0}", ex);
                Sender.Tell(new List<PPGameRecentHistoryDetailItem>());
            }
        }
        private async Task onRequestPPHistoryData(HTTPPPHistoryGetItemDetailRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery = "SELECT id,  detaillog FROM ppuserrecentgamelog WHERE id=@id";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@id", request.RoundID);

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string strDetailLog = (string)reader["detaillog"];

                            List<PPHistoryItemDetail> detailLogs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PPHistoryItemDetail>>(strDetailLog);
                            List<PPGameRecentHistoryDetailItem> items = new List<PPGameRecentHistoryDetailItem>();
                            for (int i = 0; i < detailLogs.Count; i++)
                            {
                                PPGameRecentHistoryDetailItem item = new PPGameRecentHistoryDetailItem();
                                if (request.Token.EndsWith("0"))
                                {
                                    item.currency = "MYR";
                                    item.currencySymbol = "RM";
                                }
                                else if (request.Token.EndsWith("1"))
                                {
                                    item.currency = "SGD";
                                    item.currencySymbol = "$";
                                }
                                else if (request.Token.EndsWith("2"))
                                {
                                    item.currency = "AUD";
                                    item.currencySymbol = "A$";
                                }
                                else if (request.Token.EndsWith("3"))
                                {
                                    item.currency = "THB";
                                    item.currencySymbol = "฿";
                                }
                                else if (request.Token.EndsWith("4"))
                                {
                                    item.currency = "USD";
                                    item.currencySymbol = "$";
                                }
                                else if (request.Token.EndsWith("5"))
                                {
                                    item.currency = "MMK";
                                    item.currencySymbol = "MMK";
                                }
                                else if (request.Token.EndsWith("6"))
                                {
                                    item.currency = "HKD";
                                    item.currencySymbol = "$";
                                }
                                else if (request.Token.EndsWith("7"))
                                {
                                    item.currency = "IDR";
                                    item.currencySymbol = "Rp";
                                }
                                else if (request.Token.EndsWith("8"))
                                {
                                    item.currency = "BDT";
                                    item.currencySymbol = "BDT";
                                }
                                else if (request.Token.EndsWith("9"))
                                {
                                    item.currency = "INR";
                                    item.currencySymbol = "INR";
                                }
                                else if (request.Token.EndsWith("0-"))
                                {
                                    item.currency = "CNY";
                                    item.currencySymbol = "¥";
                                }
                                item.roundId = request.RoundID;
                                item.request = splitFormUrlEncoded(detailLogs[i].cr);
                                item.response = splitFormUrlEncoded(detailLogs[i].sr);
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
                        command.Parameters.AddWithValue("@token", request.Token);
                        command.Parameters.AddWithValue("@gameid", request.GameID);
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

        private async Task onRequestGameLogByRoundID(HTTPPPHistoryGetByRoundRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery = string.Format("SELECT id, endmoney, bet, win, logtime FROM gamelog_{0} WHERE id=@id", request.AgentID);
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@id", request.RoundID);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            PPGameLogHistoryItem item = new PPGameLogHistoryItem();
                            item.roundId = request.RoundID;
                            item.balance = Math.Round((double)(decimal)reader["endmoney"], 2).ToString();
                            item.bet = Math.Round((double)(decimal)reader["bet"], 2).ToString();
                            item.win = Math.Round((double)(decimal)reader["win"], 2).ToString();
                            item.dateTime = (long)((DateTime)reader["logtime"]).Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

                            if (request.Currency == "0")
                            {
                                item.currency = "MYR";
                                item.currencySymbol = "RM";
                            }
                            else if (request.Currency == "1")
                            {
                                item.currency = "SGD";
                                item.currencySymbol = "$";
                            }
                            else if (request.Currency == "2")
                            {
                                item.currency = "AUD";
                                item.currencySymbol = "A$";
                            }
                            else if (request.Currency == "3")
                            {
                                item.currency = "THB";
                                item.currencySymbol = "฿";
                            }
                            else if (request.Currency == "4")
                            {
                                item.currency = "USD";
                                item.currencySymbol = "$";
                            }
                            else if (request.Currency == "5")
                            {
                                item.currency = "MMK";
                                item.currencySymbol = "MMK";
                            }
                            else if (request.Currency == "6")
                            {
                                item.currency = "HKD";
                                item.currencySymbol = "$";
                            }
                            else if (request.Currency == "7")
                            {
                                item.currency = "IDR";
                                item.currencySymbol = "Rp";
                            }
                            else if (request.Currency == "8")
                            {
                                item.currency = "BDT";
                                item.currencySymbol = "BDT";
                            }
                            else if (request.Currency == "9")
                            {
                                item.currency = "INR";
                                item.currencySymbol = "INR";
                            }
                            else if (request.Currency == "10")
                            {
                                item.currency = "CNY";
                                item.currencySymbol = "¥";
                            }
                            Sender.Tell(item);
                            return;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onRequestGameLogByRoundID {0}", ex);
            }
            Sender.Tell(null);
        }
        private async Task onRequestPPHistoryList(HTTPPPHistoryGetLastItemsRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    List<PPGameRecentHistoryItem> items = new List<PPGameRecentHistoryItem>();
                    string strQuery = "SELECT id, balance, bet, win, timestamp FROM ppuserrecentgamelog WHERE username=@username and gameid=@gameid ORDER BY timestamp DESC";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@username", request.UserID);
                    command.Parameters.AddWithValue("@gameid", request.GameID);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PPGameRecentHistoryItem item = new PPGameRecentHistoryItem();
                            item.roundId = (long)reader["id"];
                            item.balance = Math.Round((double)(decimal)reader["balance"], 2).ToString();
                            item.bet = Math.Round((double)(decimal)reader["bet"], 2).ToString();
                            item.win = Math.Round((double)(decimal)reader["win"], 2).ToString();
                            item.dateTime = (long)reader["timestamp"];
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

                    List<PPGameHistoryTopListItem> items = new List<PPGameHistoryTopListItem>();
                    string strQuery = "SELECT id, bet, basebet,win, rtp, linktoken, playeddate FROM ppusertopgamelog WHERE username=@username and gameid=@gameid ORDER BY playeddate DESC";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@username", request.UserID);
                    command.Parameters.AddWithValue("@gameid", request.GameID);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string strLinkToken = null;
                            if (!(reader["linktoken"] is DBNull))
                                strLinkToken = string.Format("{0}?{1}", PragmaticConfig.Instance.ReplayURL, (string)reader["linktoken"]);

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
        private async Task doLoginRequest(UserLoginRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();
                    string strQuery = "SELECT * FROM users WHERE username=@username";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@username", request.UserID);

                    UserLoginResponse response = null;
                    LoginResult resultCode = LoginResult.IDPASSWORDMISMATCH;

                    long dbID = 0;
                    string strUserID = "";
                    double balance = 0.0;
                    string strAgentName = "";
                    int agentID = 0;
                    long lastScoreID = 0;
                    string strUserToken = "";
                    do
                    {
                        using (DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (!await reader.ReadAsync())
                                break;

                            dbID = (long)reader["id"];
                            strUserID = (string)reader["username"];
                            balance = (double)(decimal)reader["balance"];
                            strAgentName = (string)reader["agentname"];
                            agentID = (int)reader["agentid"];
                            lastScoreID = (long)reader["lastscoreid"];

                            string strPasswordMD5 = (string)reader["password"];
                            if (!MD5Utils.CompareMD5Hashes(request.Password, strPasswordMD5))
                                break;

                            strUserToken = string.Format("{0}@{1}", strUserID, strPasswordMD5);
                            int state = (int)reader["state"];
                            if (state != 1)
                            {
                                resultCode = LoginResult.ACCOUNTDISABLED;
                                break;
                            }
                        }
                        response = new UserLoginResponse(dbID, strUserID, strUserToken, balance, strAgentName, agentID, lastScoreID, request.IPAddress);
                    } while (false);

                    if (response == null)
                        response = new UserLoginResponse(resultCode);

                    strQuery = "SELECT currency, iprestriction, moneymode FROM agents WHERE id=@id";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@id", response.AgentID);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            response.IPRestriction = ((int)reader["iprestriction"] == 1);
                            response.AgentCurrency = (Currencies)(int)reader["currency"];
                            response.AgentMoneyMode = (int)reader["moneymode"];
                        }
                        else
                        {
                            response.AgentCurrency = Currencies.MYR;
                            response.IPRestriction = false;
                            response.AgentMoneyMode = 0;

                        }
                    }
                    Sender.Tell(response);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::doLoginRequest {0}", ex);
            }
        }

        private async Task doApiLoginRequest(ApiUserLoginRequest request)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();
                    string strQuery = "SELECT * FROM users WHERE username=@username";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@username", request.UserID);

                    UserLoginResponse response = null;
                    LoginResult resultCode = LoginResult.IDPASSWORDMISMATCH;

                    long dbID = 0;
                    string strUserID = "";
                    double balance = 0.0;
                    string strAgentName = "";
                    int agentID = 0;
                    long lastScoreID = 0;
                    string strUserToken = "";
                    do
                    {
                        using (DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (!await reader.ReadAsync())
                                break;

                            dbID = (long)reader["id"];
                            strUserID = (string)reader["username"];
                            balance = (double)(decimal)reader["balance"];
                            strAgentName = (string)reader["agentname"];
                            agentID = (int)reader["agentid"];
                            lastScoreID = (long)reader["lastscoreid"];

                            string strPasswordMD5 = (string)reader["password"];

                            strUserToken = string.Format("{0}@{1}", strUserID, strPasswordMD5);
                            int state = (int)reader["state"];
                            if (state != 1)
                            {
                                resultCode = LoginResult.ACCOUNTDISABLED;
                                break;
                            }
                        }
                        response = new UserLoginResponse(dbID, strUserID, strUserToken, balance, strAgentName, agentID, lastScoreID, "");
                    } while (false);

                    if (response == null)
                        response = new UserLoginResponse(resultCode);

                    strQuery = "SELECT currency, iprestriction, moneymode FROM agents WHERE id=@id";
                    command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@id", response.AgentID);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            response.IPRestriction = ((int)reader["iprestriction"] == 1);
                            response.AgentCurrency = (Currencies)(int)reader["currency"];
                            response.AgentMoneyMode = (int)reader["moneymode"];
                        }
                        else
                        {
                            response.AgentCurrency = Currencies.MYR;
                            response.IPRestriction = false;
                            response.AgentMoneyMode = 0;
                        }
                    }

                    Sender.Tell(response);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::doApiLoginRequest {0}", ex);
            }
        }
        private async Task doUpdatePPTournamentLeader(PPTourLeaderUpdateItem item)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    string strQuery = "UPDATE pptournamentleaders SET win=@win WHERE tournamentid=@tournamentid AND username=@username";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@win", item.win);
                    command.Parameters.AddWithValue("@tournamentid", item.tournamentID);
                    command.Parameters.AddWithValue("@username", item.userID);
                    await command.ExecuteNonQueryAsync();

                    Sender.Tell(true);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::doUpdatePPTournamentLeader {0}", ex);
                Sender.Tell(false);
            }
        }

        private async Task onGetPPFreeSpinReports(GetPPFreeSpinReportsRequest _)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                    List<PPFreeSpinInfo> items = new List<PPFreeSpinInfo>();
                    string strQuery = "SELECT id, fsid, agentid, username FROM ppfreespinreports WHERE username=@username";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@username", _.Username);

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var item = new PPFreeSpinInfo();
                            item.BonusID = (int)reader["fsid"];
                            items.Add(item);

                        }
                    }

                    Sender.Tell(items.Select(obj => obj.BonusID).ToArray());
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onGetPPFreeSpinReports {0}", ex);
                int[] noFSReports = { };
                Sender.Tell(noFSReports);
            }
        }

        private async Task onGetPPCashbackReports(GetUserReceivedCashbacks _)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_strConnString))
                {
                    await connection.OpenAsync();

                   Dictionary<string, bool> items = new Dictionary<string, bool>();
                    string strQuery = "SELECT id, raceid, agentid, username, period, periodkey FROM ppcashbackreports WHERE username=@username";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@username", _.Username);

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            items.Add($"{(int)reader["raceid"]}_{(string)reader["periodkey"]}", true);
                        }
                    }

                    Sender.Tell(items);
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::onGetPPCashbackReports {0}", ex);
            
                Sender.Tell(new Dictionary<string, bool>());
            }
        }
    }
    public class PPHistoryItemDetail
    {
        public string cr { get; set; }  //client request
        public string sr { get; set; }  //server response
    }
    public class PPTourLeaderUpdateItem
    {
        public int tournamentID { get; set; }
        public string userID { get; set; }
        public decimal win { get; set; }

        public PPTourLeaderUpdateItem(int id, string userid, decimal winmoney)
        {
            tournamentID = id;
            userID = userid;
            win = winmoney;
        }
    }
    public class GetPPFreeSpinReportsRequest
    {
        public string Username { get; private set; }
        public GetPPFreeSpinReportsRequest(string username)
        {
            Username = username;
        }
    }
    public class GetUserReceivedCashbacks
    {
        public string Username { get; private set; }
        public GetUserReceivedCashbacks(string username)
        {
            Username = username;
        }
    }
}
