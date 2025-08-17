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

        }

        public static Props Props(string strConnString, int poolSize)
        {
            return Akka.Actor.Props.Create(() => new DBProxyReader(strConnString)).WithRouter(new RoundRobinPool(poolSize));
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
}
