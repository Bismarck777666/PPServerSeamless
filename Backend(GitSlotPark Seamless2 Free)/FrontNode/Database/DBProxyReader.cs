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
using System.Drawing.Imaging;
using PCGSharp;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Drawing;

namespace FrontNode.Database
{
    public class DBProxyReader : ReceiveActor
    {
        private readonly ILoggingAdapter _logger                = Logging.GetLogger(Context);
        private string                   _strConnString         = "";
        public DBProxyReader(string strConnString)
        {
            _strConnString = strConnString;

            //유저로그인 요청을 처리한다.
            ReceiveAsync<UserLoginRequest>                  (doLoginRequest);
            ReceiveAsync<HTTPPPReplayListRequest>           (onRequestPPReplayList);
            ReceiveAsync<HTTPPPReplayDataRequest>           (onRequestPPReplayData);
            ReceiveAsync<HTTPPPHistoryGetLastItemsRequest>  (onRequestPPHistoryList);
            ReceiveAsync<HTTPPPHistoryGetItemDetailRequest> (onRequestPPHistoryData);
            ReceiveAsync<HTTPPPReplayMakeLinkRequest>       (onRequestPPReplayMakeLink);
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
                    string sharedLink   = "";
                    string strQuery     = "SELECT linktoken, detaillog, currency FROM ppusertopgamelog WHERE id=@id";
                    SqlCommand command  = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@id",          request.RoundID);
                    
                    string strDetailLog         = null;
                    string strBeforeLinkToken   = null;
                    string strCurrency          = null;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            strDetailLog = (string) reader["detaillog"];
                            strCurrency  = (string) reader["currency"];
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
                        sharedLink = strBeforeLinkToken;
                    else
                        sharedLink = string.Format("token={0}&symbol={1}&envID={2}&roundID={3}&currency={4}&lang={5}", strLinkToken, request.Symbol, request.EnvID, request.RoundID, strCurrency, request.Lang);

                    
                    if(strBeforeLinkToken == null)
                    {
                        strQuery = "UPDATE ppusertopgamelog SET linktoken=@linktoken WHERE id=@id";
                        command = new SqlCommand(strQuery, connection);
                        command.Parameters.AddWithValue("@id",       request.RoundID);
                        command.Parameters.AddWithValue("linktoken", sharedLink);
                        await command.ExecuteNonQueryAsync();

                        strQuery = "INSERT INTO ppusercreatedlinks (token, gameid, roundid, detail, createdtime) VALUES(@token, @gameid, @roundid,@detail,@createdtime)";
                        command = new SqlCommand(strQuery, connection);
                        command.Parameters.AddWithValue("@token",       strLinkToken);
                        command.Parameters.AddWithValue("@gameid",      request.GameID);
                        command.Parameters.AddWithValue("@roundid",     request.RoundID);
                        command.Parameters.AddWithValue("@detail",      strDetailLog);
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

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string strDetailLog = (string)reader["detaillog"];
                            string strCurrency  = (string)reader["currency"];
                            List<PPHistoryItemDetail> detailLogs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PPHistoryItemDetail>>(strDetailLog);
                            List<PPGameRecentHistoryDetailItem> items = new List<PPGameRecentHistoryDetailItem>();
                            for(int i = 0; i < detailLogs.Count; i++)
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
            string strSymbol = "";
            switch (strCurrency)
            {
                case "USD":
                case "AUD":
                case "CAD":
                case "NZD":
                case "HKD":
                case "SGD":
                    strSymbol = "$";
                    break;
                case "EUR":
                    strSymbol = "€";
                    break;
                case "KRW":
                    strSymbol = "₩";
                    break;
                case "TND":
                    strSymbol = "TND";
                    break;
                case "TRY":
                    strSymbol = "₺";
                    break;
                case "BRL":
                    strSymbol = "R$";
                    break;
                case "IDR":
                    strSymbol = "Rp";
                    break;
                case "CHF":
                    strSymbol = "CHF";
                    break;
                case "NOK":
                    strSymbol = "kr";
                    break;
                case "THB":
                    strSymbol = "฿";
                    break;
                case "INR":
                    strSymbol = "₹";
                    break;
                case "MYR":
                    strSymbol = "RM";
                    break;
                case "XOF":
                    strSymbol = "XOF";
                    break;
                case "NGN":
                    strSymbol = "NGN";
                    break;
                case "TVD":
                    strSymbol = "TVD";
                    break;
                case "RUB":
                    strSymbol = "₽";
                    break;
                case "MAD":
                    strSymbol = "MAD";
                    break;
                case "ILS":
                    strSymbol = "ILS";
                    break;
                case "ARS":
                    strSymbol = "ARS";
                    break;
                case "COP":
                    strSymbol = "COP";
                    break;
                case "RT":
                    strSymbol = "RT";
                    break;
                case "BWP":
                    strSymbol = "BWP";
                    break;
                case "AED":
                    strSymbol = "AED";
                    break;
                case "UAH":
                    strSymbol = "UAH";
                    break;
                case "UYU":
                    strSymbol = "UYU";
                    break;
                case "LBP":
                    strSymbol = "L£";
                    break;
                case "IQD":
                    strSymbol = "LIQD";
                    break;
                case "PHP":
                    strSymbol = "PHP";
                    break;
                case "EGP":
                    strSymbol = "EGP";
                    break;
                case "AZN":
                    strSymbol = "AZN";
                    break;
                case "PKR":
                    strSymbol = "PKR";
                    break;
                case "KZT":
                    strSymbol = "KZT";
                    break;
                case "UZS":
                    strSymbol = "UZS";
                    break;
                case "CNY":
                    strSymbol = "CNY";
                    break;
                case "PLN":
                    strSymbol = "PLN";
                    break;
                case "HUF":
                    strSymbol = "HUF";
                    break;
                case "CLP":
                    strSymbol = "CLP";
                    break;
                case "CZK":
                    strSymbol = "CZK";
                    break;
                case "GEL":
                    strSymbol = "GEL";
                    break;
                case "RON":
                    strSymbol = "RON";
                    break;
                case "BAM":
                    strSymbol = "BAM";
                    break;
                case "TOPIA":
                    strSymbol = "TOPIA";
                    break;
                case "SYP":
                    strSymbol = "SYP";
                    break;
                case "SEK":
                    strSymbol = "SEK";
                    break;
                case "ZAR":
                    strSymbol = "ZAR";
                    break;
                case "GBP":
                    strSymbol = "GBP";
                    break;
                case "GOF":
                    strSymbol = "GOF";
                    break;
                case "KES":
                    strSymbol = "KES";
                    break;
                case "ZWL":
                    strSymbol = "ZWL";
                    break;
                case "ZMW":
                    strSymbol = "ZMW";
                    break;
                case "AOA":
                    strSymbol = "AOA";
                    break;
                case "MZN":
                    strSymbol = "MZN";
                    break;
                case "NAD":
                    strSymbol = "NAD";
                    break;
                case "PYG":
                    strSymbol = "PYG";
                    break;
                case "BOB":
                    strSymbol = "BOB";
                    break;
                case "DZD":
                    strSymbol = "DZD";
                    break;
                case "ISK":
                    strSymbol = "ISK";
                    break;
                case "PEN":
                    strSymbol = "PEN";
                    break;
                case "BDT":
                    strSymbol = "BDT";
                    break;
                case "DKK":
                    strSymbol = "DKK";
                    break;
                case "MNT":
                    strSymbol = "MNT";
                    break;
                case "TMT":
                    strSymbol = "TMT";
                    break;
                case "MXN":
                    strSymbol = "MXN";
                    break;
                case "IRR":
                    strSymbol = "IRR";
                    break;
                case "IRT":
                    strSymbol = "IRT";
                    break;
                case "JPY":
                    strSymbol = "JPY";
                    break;
                case "VND":
                    strSymbol = "VND";
                    break;
                case "MMK":
                    strSymbol = "MMK";
                    break;
                case "UGX":
                    strSymbol = "UGX";
                    break;
                case "TZS":
                    strSymbol = "TZS";
                    break;
                case "GHS":
                    strSymbol = "GHS";
                    break;
                case "RSD":
                    strSymbol = "RSD";
                    break;
                case "HRK":
                    strSymbol = "HRK";
                    break;
                case "XAF":
                    strSymbol = "XAF";
                    break;
                case "GNF":
                    strSymbol = "GNF";
                    break;
                default:
                    strSymbol = "$";
                    break;
            }
            return strSymbol;
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
        private bool splitGlobalUserID(string strGlobalUserID, out int agentID, out string strUserID)
        {
            agentID = 0;
            strUserID = null;
            try
            {
                int index = strGlobalUserID.IndexOf("_");
                if (index <= 0 || index >= strGlobalUserID.Length - 1)
                    return false;

                agentID = int.Parse(strGlobalUserID.Substring(0, index));
                strUserID = strGlobalUserID.Substring(index + 1);
                return true;
            }
            catch (Exception)
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

                    int     agentID = 0;
                    string  strUserID = "";
                    if (!splitGlobalUserID(request.UserID, out agentID, out strUserID))
                    {
                        Sender.Tell(new List<PPGameRecentHistoryItem>());
                        return;
                    }

                    string strQuery = "SELECT id, balance, bet, win, timestamp,currency FROM ppuserrecentgamelog WHERE agentid=@agentid and username=@username and gameid=@gameid ORDER BY timestamp DESC";
                    SqlCommand command = new SqlCommand(strQuery, connection);
                    command.Parameters.AddWithValue("@agentid",     agentID);
                    command.Parameters.AddWithValue("@username",    strUserID);
                    command.Parameters.AddWithValue("@gameid",      request.GameID);
                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            PPGameRecentHistoryItem item = new PPGameRecentHistoryItem();
                            item.roundId        = (long)reader["id"];
                            item.balance        = Math.Round((double)(decimal)reader["balance"], 2).ToString();
                            item.bet            = Math.Round((double)(decimal)reader["bet"], 2).ToString();
                            item.win            = Math.Round((double)(decimal)reader["win"], 2).ToString();
                            item.currency       = (string) reader["currency"];
                            item.currencySymbol = convertCurrencyToSymbol(item.currency);
                            item.dateTime       = (long)   reader["timestamp"];
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

                    int     agentID = 0;
                    string  strUserID = "";
                    if (!splitGlobalUserID(request.UserID, out agentID, out strUserID))
                    {
                        Sender.Tell(new HTTPPPReplayListResponse(new List<PPGameHistoryTopListItem>()));
                        return;
                    }
                    List<PPGameHistoryTopListItem> items = new List<PPGameHistoryTopListItem>(); 
                    string strQuery = "SELECT id, bet, basebet,win, rtp, linktoken, playeddate FROM ppusertopgamelog WHERE agentid=@agentid and username=@username and gameid=@gameid ORDER BY playeddate DESC";
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
                            {
                                strLinkToken = string.Format("{0}?{1}", PragmaticConfig.Instance.ReplayURL, (string)reader["linktoken"]);
                            }

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
                    string      strQuery    = "SELECT * FROM users WHERE agentid=@agentid and username=@username";
                    SqlCommand  command     = new SqlCommand(strQuery, connection);

                    command.Parameters.AddWithValue("@agentid",  request.AgentID);
                    command.Parameters.AddWithValue("@username", request.UserID);

                    UserLoginResponse response = null;
                    LoginResult resultCode = LoginResult.IDPASSWORDMISMATCH;

                    long    dbID                = 0;
                    string  strUserID           = "";
                    double  balance             = 0.0;
                    string  strAgentName        = "";
                    int     agentDBID           = 0;
                    string  strPasswordMD5      = null;
                    long    lastScoreCounter    = 0;
                    int     currency            = 0;
                    bool    isAffiliate         = false;

                    do
                    {
                        using (DbDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (!await reader.ReadAsync())
                                break;

                            dbID             = (long)                reader["id"];
                            strUserID        = (string)              reader["username"];                         
                            balance          = (double) (decimal)    reader["balance"];
                            strAgentName     = (string)              reader["agentname"];
                            agentDBID        = (int)                 reader["agentid"];
                            lastScoreCounter = (long)                reader["lastscorecounter"];
                            strPasswordMD5   = (string)              reader["password"];
                            currency         = (int)                 reader["currency"];
                            isAffiliate      = (int)                 reader["isaffiliate"] == 1;

                            if (!MD5Utils.CompareMD5Hashes(request.Password, strPasswordMD5))
                                break;

                            int state = (int)reader["state"];
                            if (state != 1)
                            {
                                resultCode = LoginResult.ACCOUNTDISABLED;
                                break;
                            }
                        }
                        response = new UserLoginResponse(strAgentName, agentDBID, dbID, strUserID, strPasswordMD5, balance, lastScoreCounter, currency, isAffiliate);
                    } while (false);

                    if (response == null)
                        response = new UserLoginResponse(resultCode);

                    //응답을 보낸다.
                    Sender.Tell(response);
                }
            }
            catch(Exception ex)
            {
                _logger.Error("Exception has been occured in DBProxyReader::doLoginRequest {0}", ex);
            }
        }      
    }
    public class PPHistoryItemDetail
    {
        public string cr                { get; set; }  //client request
        public string sr                { get; set; }  //server response
        public string currency          { get; set; }
    }   
}
