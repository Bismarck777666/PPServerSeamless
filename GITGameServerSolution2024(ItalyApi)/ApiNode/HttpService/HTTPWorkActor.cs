using Akka.Actor;
using Akka.Configuration.Hocon;
using Akka.Event;
using Akka.Routing;
using GITProtocol;
using QueenApiNode.Agent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using QueenApiNode.Database;
using Newtonsoft.Json;

namespace QueenApiNode.HttpService
{
    public class HTTPWorkActor : ReceiveActor
    {
        private readonly ILoggingAdapter    _logger = Logging.GetLogger(Context);
        private IActorRef                   _dbReader;
        private IActorRef                   _dbWriter;
        public HTTPWorkActor(IActorRef dbReader, IActorRef dbWriter)
        {
 
            ReceiveAsync<ApiConsistentRequest>          (onApiRequest);
            ReceiveAsync<SubtractEventMoneyRequest>     (onSubtractEventMoney);
            ReceiveAsync<AddEventLeftMoneyRequest>      (onAddLeftEventMoney);

            Receive<string>                     (onProcCommand);

            _dbReader = dbReader;
            _dbWriter = dbWriter;
        }
        private void onProcCommand(string command)
        {
            if (command == "terminate")
                Self.Tell(PoisonPill.Instance);
        }

        private async Task<IActorRef> createOrGetAgentActor(int agentID)
        {
            try
            {
                IActorRef agentActor = await Context.System.ActorSelection("/user/agentManager").Ask<IActorRef>(new GetAgentActorByIDMsg(agentID), TimeSpan.FromSeconds(5.0));
                if (!agentActor.Equals(ActorRefs.Nobody))
                    return agentActor;

                DBAgentInfoResponse agentInfo = await _dbReader.Ask<DBAgentInfoResponse>(new DBGetAgentInfoByIDRequest(agentID), TimeSpan.FromSeconds(5));
                if (agentInfo == null || agentInfo.AgentID == null || agentInfo.AuthToken == null)
                    return ActorRefs.Nobody;

                agentActor = await Context.System.ActorSelection("/user/agentManager").Ask<IActorRef>(
                    new CreateNewAgentActorMsg(agentInfo.AgentDBID, agentInfo.AgentID,
                    agentInfo.AuthToken, agentInfo.Balance,agentInfo.Currency ,_dbReader, _dbWriter), TimeSpan.FromSeconds(5.0));

                return agentActor;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPWorkActor::createOrGetAgentActor {0}", ex);
                return ActorRefs.Nobody;
            }
        }

        private async Task<IActorRef> createOrGetAgentActor(string strAgentID)
        {
            try
            {
                IActorRef agentActor = await Context.System.ActorSelection("/user/agentManager").Ask<IActorRef>(new GetAgentActorMsg(strAgentID), TimeSpan.FromSeconds(5.0));
                if (!agentActor.Equals(ActorRefs.Nobody))
                    return agentActor;

                DBAgentInfoResponse agentInfo = await _dbReader.Ask<DBAgentInfoResponse>(new DBGetAgentInfoRequest(strAgentID), TimeSpan.FromSeconds(5));
                if (agentInfo == null || agentInfo.AgentID == null || agentInfo.AuthToken == null)
                    return ActorRefs.Nobody;

                agentActor = await Context.System.ActorSelection("/user/agentManager").Ask<IActorRef>(
                    new CreateNewAgentActorMsg(agentInfo.AgentDBID, agentInfo.AgentID,
                    agentInfo.AuthToken, agentInfo.Balance, agentInfo.Currency, _dbReader, _dbWriter), TimeSpan.FromSeconds(5.0));

                return agentActor;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPWorkActor::createOrGetAgentActor {0}", ex);
                return ActorRefs.Nobody;
            }
        }
        private async Task onApiRequest(ApiConsistentRequest request)
        {
            try
            {

                string[] strParts = request.AuthToken.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (strParts.Length != 2 || strParts[0] != "Bearer")
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidToken, "invalid token")));
                    return;
                }
                string strAgentID = "";
                if (request.Request is AgentWithdrawRequest || request.Request is AgentDepositRequest)
                {                    
                    if (strParts[1] != ApiConfig.AdminTokenKey)
                    {
                        Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidToken, "invalid token")));
                        return;
                    }
                    if (request.Request is AgentWithdrawRequest)
                        strAgentID = (request.Request as AgentWithdrawRequest).agentid;
                    else
                        strAgentID = (request.Request as AgentDepositRequest).agentid;
                }
                else
                {
                    strAgentID = await Context.System.ActorSelection("/user/agentManager").Ask<string>(strParts[1], TimeSpan.FromSeconds(5.0));
                }
                
                if (string.IsNullOrEmpty(strAgentID))
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidToken, "invalid token")));
                    return;
                }
                IActorRef agentActor = await createOrGetAgentActor(strAgentID);
                if (agentActor.Equals(ActorRefs.Nobody))
                {
                    Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.InvalidToken, "invalid token")));
                    return;
                }
                agentActor.Forward(request);
                return;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPWorkActor::onApiRequest {0}", ex);
            }
            Sender.Tell(JsonConvert.SerializeObject(new QueenResponse(ResponseCodes.GeneralError, "general error")));
        }

        private async Task onSubtractEventMoney(SubtractEventMoneyRequest request)
        {
            try
            {
                IActorRef websiteActor = await createOrGetAgentActor(request.AgentID);
                if (websiteActor.Equals(ActorRefs.Nobody))
                {
                    Sender.Tell(false);
                    return;
                }
                bool result = await websiteActor.Ask<bool>(request, TimeSpan.FromSeconds(5.0));
                Sender.Tell(result);
                return;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPWorkActor::onSubtractEventMoney {0}", ex);
            }
            Sender.Tell(false);
        }
        private async Task onAddLeftEventMoney(AddEventLeftMoneyRequest request)
        {
            try
            {

                IActorRef websiteActor = await createOrGetAgentActor(request.AgentID);
                if (websiteActor.Equals(ActorRefs.Nobody))
                    return;

                websiteActor.Tell(request);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in HTTPWorkActor::onAddLeftEventMoney {0}", ex);
            }
        }

    }

    public class ApiConsistentRequest : IConsistentHashable
    { 
        public string AuthToken     { get; set; }
        public object Request       { get; set; }

        public ApiConsistentRequest(string authToken, object request)
        {
            this.AuthToken  = authToken;
            this.Request    = request;
        }
        public object ConsistentHashKey
        {
            get
            {
                return AuthToken;
            }
        }
    }
}
