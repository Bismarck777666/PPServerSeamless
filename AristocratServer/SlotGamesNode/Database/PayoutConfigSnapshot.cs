﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using SlotGamesNode.GameLogics;

namespace SlotGamesNode.Database
{
    public class PayoutConfigSnapshot
    {
        private static PayoutConfigSnapshot _sInstance = new PayoutConfigSnapshot();
        public static PayoutConfigSnapshot Instance
        {
            get
            {
                return _sInstance;
            }
        }

        public Dictionary<GAMEID, GameConfig>               PayoutConfigs           { get; set; }
        public Dictionary<GAMEID, Dictionary<int, double>>  CompanyPayoutConfigs    { get; set; }
        public PayoutConfigSnapshot()
        {
            PayoutConfigs           = new Dictionary<GAMEID, GameConfig>();
            CompanyPayoutConfigs    = new Dictionary<GAMEID, Dictionary<int, double>>();
        }
    }
}
