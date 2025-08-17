using Akka.Actor;
using Akka.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CommNode.Database;

namespace CommNode
{
    public class RetryWorker : ReceiveActor
    {
        private IActorRef                           _dbWriter   = null;
        private List<CallbackApiRequestWithRetry>   _retryTasks = new List<CallbackApiRequestWithRetry>();

        public RetryWorker(IActorRef dbWriter)
        {
            Receive<CallbackApiRequestWithRetry>(_ =>
            {
                _retryTasks.Add(_);
            });
            ReceiveAsync<string>(onProcCommand);
            _dbWriter = dbWriter;
        }
        private async Task onProcCommand(string strCommand)
        {
            try
            {
                List<CallbackApiRequestWithRetry> toDelItems = new List<CallbackApiRequestWithRetry>();
                foreach (var item in _retryTasks)
                {
                    if (DateTime.Now.Subtract(item.LastTriedTime) < TimeSpan.FromSeconds(10.0))
                        continue;

                    if (item is RollbackRequestWithRetry)
                    {
                        var request = item as RollbackRequestWithRetry;
                        var response = await onRollback(request as RollbackRequestWithRetry);
                        if (response == null)
                        {
                            request.RetryCount--;
                            if (request.RetryCount < 0)
                            {
                                toDelItems.Add(request);
                                _dbWriter.Tell(new FailedTransactionItem(request.AgentID, request.UserID, TransactionTypes.Rollback, request.TransactionID, "", request.Amount, request.GameID, DateTime.UtcNow));
                            }
                        }
                        else if (response.code == 0 || response.code == 8 || response.code == 9)
                        {
                            toDelItems.Add(request);
                        }
                        else
                        {
                            toDelItems.Add(request);
                            _dbWriter.Tell(new FailedTransactionItem(request.AgentID, request.UserID, TransactionTypes.Rollback, request.TransactionID, "", request.Amount, request.GameID, DateTime.UtcNow));
                        }
                    }
                    else
                    {
                        var request = item as DepositRequestWithRetry;
                        var response = await onDeposit(request);
                        if (response == null)
                        {
                            request.RetryCount--;
                            if (request.RetryCount < 0)
                            {
                                toDelItems.Add(request);
                                _dbWriter.Tell(new FailedTransactionItem(request.AgentID, request.UserID, TransactionTypes.Deposit, request.TransactionID, request.RefTransactionID, request.Amount, request.GameID, DateTime.UtcNow));
                            }
                        }
                        else if (response.code == 0 || response.code == 11)
                        {
                            toDelItems.Add(request);
                            _dbWriter.Tell(new DepositTransactionUpdateItem(request.TransactionID, response.platformTransactionID, DateTime.UtcNow));
                        }
                        else
                        {
                            toDelItems.Add(request);
                            _dbWriter.Tell(new FailedTransactionItem(request.AgentID, request.UserID, TransactionTypes.Deposit, request.TransactionID, request.RefTransactionID, request.Amount, request.GameID, DateTime.UtcNow));
                        }
                    }
                }

                foreach (var item in toDelItems)
                    _retryTasks.Remove(item);
            }
            catch(Exception ex)
            {

            }
            finally
            {
                Context.System.Scheduler.ScheduleTellOnce(500, Self, "tick", Self);
            }
        }
        private async Task<SeamlessRollbackResponse> onRollback(RollbackRequestWithRetry request)
        {
            try
            {
                var rollbackRequest                 = new SeamlessRollbackRequest();
                rollbackRequest.agentID             = request.AgentID;
                rollbackRequest.userID              = request.UserID;
                rollbackRequest.refTransactionID    = request.TransactionID;
                rollbackRequest.gameID              = request.GameID;
                rollbackRequest.sign                = UserActor.createDataSign(request.SecretKey, string.Format("{0}{1}{2}{3}",
                    request.AgentID, request.UserID, request.TransactionID, request.GameID));

                HttpClient httpClient = new HttpClient();
                string strURL = string.Format("{0}/RollbackTransaction", request.ApiURL);

                HttpResponseMessage message = await httpClient.PostAsync(strURL, new StringContent(JsonConvert.SerializeObject(rollbackRequest), Encoding.UTF8, "application/json"));
                message.EnsureSuccessStatusCode();

                string  strContent  = await message.Content.ReadAsStringAsync();
                var     response    = JsonConvert.DeserializeObject<SeamlessRollbackResponse>(strContent);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private async Task<SeamlessDepositResponse> onDeposit(DepositRequestWithRetry request)
        {
            try
            {
                var depositRequest              = new SeamlessDepositRequest();
                depositRequest.agentID          = request.AgentID;
                depositRequest.userID           = request.UserID;
                depositRequest.transactionID    = request.TransactionID;
                depositRequest.refTransactionID = request.RefTransactionID;
                depositRequest.gameID           = request.GameID;
                depositRequest.amount           = (decimal) Math.Round(request.Amount, 2);
                depositRequest.roundID          = request.RoundID;
                depositRequest.sign             = UserActor.createDataSign(request.SecretKey, string.Format("{0}{1}{2}{3}{4}{5}{6}",
                    request.AgentID, request.UserID, request.Amount.ToString("0.00"), request.RefTransactionID, request.TransactionID, request.RoundID, request.GameID));

                HttpClient httpClient = new HttpClient();
                string strURL = string.Format("{0}/Deposit", request.ApiURL);

                HttpResponseMessage message = await httpClient.PostAsync(strURL, new StringContent(JsonConvert.SerializeObject(depositRequest), Encoding.UTF8, "application/json"));
                message.EnsureSuccessStatusCode();

                string strContent = await message.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<SeamlessDepositResponse>(strContent);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
    }

   
}
