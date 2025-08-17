using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommNode
{
    public class AgentSnapshot
    {
        private static AgentSnapshot _sInstance = new AgentSnapshot();
        public static AgentSnapshot Instance 
        { 
            get { return _sInstance; } 
        }
        public DateTime LastUpdateTime { get; set; }
        protected ConcurrentDictionary<int, AgentAPIConfig> _agentConfigs = new ConcurrentDictionary<int, AgentAPIConfig>();

        public AgentSnapshot()
        {
            this.LastUpdateTime = new DateTime(1, 1, 1);
        }
        public void updateAgentConfig(int agentID, AgentAPIConfig agentConfig)
        {
            _agentConfigs.AddOrUpdate(agentID, agentConfig, (key, oldValue) => agentConfig);
        }
        public AgentAPIConfig findAgentConfig(int agentID)
        {
            AgentAPIConfig agentConfig = null;
            if (_agentConfigs.TryGetValue(agentID, out agentConfig))
                return agentConfig;
            return null;
        }
    }
    public class AgentAPIConfig
    {
        public int      ApiMode         { get; set; }
        public string   AuthToken       { get; set; }
        public string   CallbackURL     { get; set; }
        public string   ApiToken        { get; set; }
        public string   SecretKey       { get; set; }

        public AgentAPIConfig()
        {

        }
        public AgentAPIConfig(string authToken) 
        {
            this.ApiMode        = 0;
            this.AuthToken      = authToken;
        }
        public AgentAPIConfig(string apiToken, string secretKey, string callbackURL)
        {
            this.ApiMode        = 1;
            this.ApiToken       = apiToken;
            this.SecretKey      = secretKey;
            this.CallbackURL    = callbackURL;
        }

    }
}
