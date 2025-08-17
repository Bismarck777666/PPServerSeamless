using Akka.Actor;
using System;
using System.Collections.Generic;

namespace SlotGamesNode.GameLogics
{
    internal class PayoutPoolActor : ReceiveActor
    {
        protected static int PoolCount = 2;
        protected Dictionary<int, double[]> _websiteTotalBets   = new Dictionary<int, double[]>();
        protected Dictionary<int, double[]> _websiteTotalWins   = new Dictionary<int, double[]>();
        protected Dictionary<int, double[]> _websiteRedundency  = new Dictionary<int, double[]>();

        protected double[] _totalBets       = new double[PoolCount];
        protected double[] _totalWins       = new double[PoolCount];
        protected double[] _maxRedundencys  = new double[PoolCount];

        protected Dictionary<int, bool> _changePoolStatus = new Dictionary<int, bool>();

        public PayoutPoolActor()
        {
            Receive<CheckWebsitePayoutRequest>      (onCheckWebsitePayout);
            Receive<SumUpWebsiteBetWinRequest>      (onSumUpWebsiteBetWin);
            Receive<ResetWebsitePayoutPoolRequest>  (onResetWebsitePayoutPool);
            Receive<WebsiteRedundencyUpdated>       (onWebsiteRedundencyUpdated);

            Receive<string> (onProcCommand);
        }

        protected override void PreStart()
        {
            base.PreStart();

            Self.Tell("start");
        }

        protected void onProcCommand(string strCommand)
        {
            if(strCommand == "start")
            {
                _maxRedundencys[0] = 0.0;
                _maxRedundencys[1] = 0.0;
                foreach (KeyValuePair<int, double> pair in PayoutPoolConfig.Instance.WebsitePayoutRedundency)
                {
                    if (pair.Key == 0)
                    {
                        _maxRedundencys[0] = pair.Value;
                        _maxRedundencys[1] = pair.Value * 0.2;
                    }
                    else
                    {
                        if (!_websiteRedundency.ContainsKey(pair.Key))
                            _websiteRedundency.Add(pair.Key, new double[2]);

                        _websiteRedundency[pair.Key][0] = pair.Value;
                        _websiteRedundency[pair.Key][1] = pair.Value * 0.2;
                    }
                }
                
                Context.System.Scheduler.ScheduleTellOnce(60000, Self, "updatePool", Self);
            }
            else if(strCommand == "updatePool")
            {
                Dictionary<int, double[]> payoutPoolStatus = new Dictionary<int, double[]>();
                foreach (KeyValuePair<int, bool> pair in _changePoolStatus)
                {
                    int websiteID = pair.Key;
                    if (websiteID == 0)
                        payoutPoolStatus.Add(0, new double[] { _totalBets[0] + _totalBets[1], _totalWins[0] + _totalWins[1] });
                    else
                        payoutPoolStatus.Add(websiteID, new double[] { _websiteTotalBets[websiteID][0] + _websiteTotalBets[websiteID][1], _websiteTotalWins[websiteID][0] + _websiteTotalWins[websiteID][1] });
                }
                
                if (payoutPoolStatus.Count > 0)
                    Context.System.ActorSelection("/user/dbproxy/writer").Tell(new UpdatePayoutPoolStatus(payoutPoolStatus));
                
                _changePoolStatus.Clear();
                Context.System.Scheduler.ScheduleTellOnce(60000, Self, "updatePool", Self);
            }
        }
        protected void onSumUpWebsiteBetWin(SumUpWebsiteBetWinRequest request)
        {
            int     websiteId   = request.WebsiteID;
            double  betMoney    = request.BetMoney;
            double  winMoney    = request.WinMoney;
            int     poolId      = request.PoolID;

            if (websiteId == 0)
            {
                _totalBets[poolId] += betMoney;
                _totalWins[poolId] += winMoney;
            }
            else
            {
                if (!_websiteTotalBets.ContainsKey(websiteId))
                    _websiteTotalBets[websiteId] = new double[PoolCount];

                if (!_websiteTotalWins.ContainsKey(websiteId))
                    _websiteTotalWins[websiteId] = new double[PoolCount];

                _websiteTotalBets[websiteId][poolId] += betMoney;
                _websiteTotalWins[websiteId][poolId] += winMoney;
            }

            _changePoolStatus[websiteId] = true;
        }
        protected void onResetWebsitePayoutPool(ResetWebsitePayoutPoolRequest request)
        {
            int websiteId = request.WebsiteID;
            if (websiteId == 0)
            {
                for (int i = 0; i < PoolCount; i++)
                {
                    _totalBets[i] = 0.0;
                    _totalWins[i] = 0.0;
                }
            }
            else
            {
                if (!_websiteTotalBets.ContainsKey(websiteId) || !_websiteTotalWins.ContainsKey(websiteId))
                    return;

                for (int i = 0; i < PoolCount; i++)
                {
                    _websiteTotalBets[websiteId][i] = 0.0;
                    _websiteTotalWins[websiteId][i] = 0.0;
                }
            }
        }
        protected void onCheckWebsitePayout(CheckWebsitePayoutRequest request)
        {
            int     websiteId   = request.WebsiteID;
            double  betMoney    = request.BetMoney;
            double  winMoney    = request.WinMoney;
            int     poolId      = request.PoolID;

            if (betMoney == 0.0 && winMoney == 0.0)
            {
                Sender.Tell(true);
            }
            else
            {
                double redundency = getRedundency(websiteId, poolId);
                if (websiteId == 0)
                {
                    double sumBetMoney = _totalBets[poolId] + betMoney;
                    double sumWinMoney = _totalWins[poolId] + winMoney;
                    if (sumWinMoney > sumBetMoney + redundency)
                    {
                        Sender.Tell(false);
                    }
                    else
                    {
                        _totalBets[poolId]      = sumBetMoney;
                        _totalWins[poolId]      = sumWinMoney;
                        _changePoolStatus[0]    = true;
                        Sender.Tell(true);
                    }
                }
                else
                {
                    if (!_websiteTotalBets.ContainsKey(websiteId))
                        _websiteTotalBets[websiteId] = new double[PoolCount];

                    if (!_websiteTotalWins.ContainsKey(websiteId))
                        _websiteTotalWins[websiteId] = new double[PoolCount];

                    double sumBetMoney = _websiteTotalBets[websiteId][poolId] + betMoney;
                    double sumWinMoney = _websiteTotalWins[websiteId][poolId] + winMoney;
                    if (sumWinMoney > sumBetMoney + redundency)
                    {
                        Sender.Tell(false);
                    }
                    else
                    {
                        _websiteTotalBets[websiteId][poolId]    = sumBetMoney;
                        _websiteTotalWins[websiteId][poolId]    = sumWinMoney;
                        _changePoolStatus[websiteId]            = true;
                        Sender.Tell(true);
                    }
                }
            }
        }
        protected void onWebsiteRedundencyUpdated(WebsiteRedundencyUpdated request)
        {
            if (request.WebsiteID == 0)
            {
                _maxRedundencys[0] = request.Redundency;
                _maxRedundencys[1] = request.Redundency * 0.2;
            }
            else
            {
                if (!_websiteRedundency.ContainsKey(request.WebsiteID))
                    _websiteRedundency.Add(request.WebsiteID, new double[2]);

                _websiteRedundency[request.WebsiteID][0] = request.Redundency;
                _websiteRedundency[request.WebsiteID][1] = request.Redundency * 0.2;
            }
        }
        protected double getRedundency(int websiteID, int poolID)
        {
            if (websiteID == 0)
                return _maxRedundencys[poolID];

            if (_websiteRedundency.ContainsKey(websiteID))
                return _websiteRedundency[websiteID][poolID];

            return 0.0;
        }
    }

    class CheckWebsitePayoutRequest
    {
        public int      WebsiteID   { get; private set; }
        public double   BetMoney    { get; private set; }
        public double   WinMoney    { get; private set; }
        public int      PoolID      { get; private set; }

        public CheckWebsitePayoutRequest(int websiteID, double betMoney, double winMoney, int poolID)
        {
            WebsiteID   = websiteID;
            BetMoney    = Math.Round(betMoney, 2);
            WinMoney    = Math.Round(winMoney, 2);
            PoolID      = poolID;
        }
    }
    class SumUpWebsiteBetWinRequest
    {
        public int      WebsiteID   { get; private set; }
        public double   BetMoney    { get; private set; }
        public double   WinMoney    { get; private set; }
        public int      PoolID      { get; private set; }

        public SumUpWebsiteBetWinRequest(int websiteID, double betMoney, double winMoney, int poolID)
        {
            WebsiteID   = websiteID;
            BetMoney    = Math.Round(betMoney, 2);
            WinMoney    = Math.Round(winMoney, 2);
            PoolID      = poolID;
        }

    }
    class ResetWebsitePayoutPoolRequest
    {
        public int WebsiteID { get; private set; }
        public ResetWebsitePayoutPoolRequest(int websiteID)
        {
            WebsiteID = websiteID;
        }
    }
    class WebsiteRedundencyUpdated
    {
        public int      WebsiteID   { get; private set; }
        public double   Redundency  { get; private set; }

        public WebsiteRedundencyUpdated(int websiteID, double redundency)
        {
            WebsiteID   = websiteID;
            Redundency  = redundency;
        }
    }
    public class UpdatePayoutPoolStatus
    {
        public Dictionary<int, double[]> PoolStatus { get; set; }

        public UpdatePayoutPoolStatus(Dictionary<int, double[]> poolStatus)
        {
            this.PoolStatus = poolStatus;
        }
    }
}
