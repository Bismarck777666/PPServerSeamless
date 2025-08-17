using Akka.Actor;
using Akka.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class PayoutPoolActor : ReceiveActor
    {
        protected static int PoolCount = 2;

        #region 베팅풀정보(각 웹사이트마다 있다)
        protected Dictionary<int, double[]> _websiteTotalBets  = new Dictionary<int, double[]>();
        protected Dictionary<int, double[]> _websiteTotalWins  = new Dictionary<int, double[]>();
        protected Dictionary<int, double[]> _websiteRedundency = new Dictionary<int, double[]>();
        #endregion

        #region 베팅풀정보(한 게임당 최대 2개의 베팅풀이 있다. 기본풀, 보조풀)
        protected double[] _totalBets       = new double[PoolCount];
        protected double[] _totalWins       = new double[PoolCount];
        protected double[] _maxRedundencys  = new double[PoolCount];
        #endregion
        protected Dictionary<int, bool> _changePoolStatus = new Dictionary<int, bool>();

        public PayoutPoolActor()
        {            
            Receive<CheckWebsitePayoutRequest>      (onCheckWebsitePayout);
            Receive<SumUpWebsiteBetWinRequest>      (onSumUpWebsiteBetWin);
            Receive<ResetWebsitePayoutPoolRequest>  (onResetWebsitePayoutPool);
            Receive<WebsiteRedundencyUpdated>       (onWebsiteRedundencyUpdated);
            Receive<string>(onProcCommand);
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
                    int websiteID = pair.Key;
                    if (websiteID == 0)
                    {
                        _maxRedundencys[0] = pair.Value;
                        _maxRedundencys[1] = pair.Value * 0.2;
                    }
                    else
                    {
                        if (!_websiteRedundency.ContainsKey(websiteID))
                            _websiteRedundency.Add(websiteID, new double[2]);

                        _websiteRedundency[websiteID][0] = pair.Value;
                        _websiteRedundency[websiteID][1] = pair.Value * 0.2;
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
                        payoutPoolStatus.Add(websiteID, new double[] { _websiteTotalBets[websiteID][0] + _websiteTotalBets[websiteID][1], _websiteTotalWins[websiteID][0] + _websiteTotalWins[websiteID][1]});                    
                }
                if (payoutPoolStatus.Count > 0)
                    Context.System.ActorSelection("/user/dbproxy/writer").Tell(new UpdatePayoutPoolStatus(payoutPoolStatus));
                _changePoolStatus.Clear();
                Context.System.Scheduler.ScheduleTellOnce(60000, Self, "updatePool", Self);
            }
        }
        protected void onSumUpWebsiteBetWin(SumUpWebsiteBetWinRequest request)
        {
            int     websiteID   = request.WebsiteID;
            double  betMoney    = request.BetMoney;
            double  winMoney    = request.WinMoney;
            int     poolIndex   = request.PoolID;
            if (websiteID == 0)
            {
                _totalBets[poolIndex] += betMoney;
                _totalWins[poolIndex] += winMoney;
            }
            else
            {
                if (!_websiteTotalBets.ContainsKey(websiteID))
                    _websiteTotalBets[websiteID] = new double[PoolCount];

                if (!_websiteTotalWins.ContainsKey(websiteID))
                    _websiteTotalWins[websiteID] = new double[PoolCount];

                _websiteTotalBets[websiteID][poolIndex] += betMoney;
                _websiteTotalWins[websiteID][poolIndex] += winMoney;
            }
            _changePoolStatus[websiteID] = true;
        }
        protected void onResetWebsitePayoutPool(ResetWebsitePayoutPoolRequest request)
        {
            int websiteID = request.WebsiteID;
            if(websiteID == 0)
            {
                for (int i = 0; i < PoolCount; i++)
                {
                    _totalBets[i] = 0.0;
                    _totalWins[i] = 0.0;
                }
                return;
            }

            if (!_websiteTotalBets.ContainsKey(websiteID) || !_websiteTotalWins.ContainsKey(websiteID))
                return;

            for (int i = 0; i < PoolCount; i++)
            {
                _websiteTotalBets[websiteID][i] = 0.0;
                _websiteTotalWins[websiteID][i] = 0.0;
            }
        }
        protected void onCheckWebsitePayout(CheckWebsitePayoutRequest request)
        {
            int     websiteID   = request.WebsiteID;
            double  betMoney    = request.BetMoney;
            double  winMoney    = request.WinMoney;
            int     poolIndex   = request.PoolID;

            if (betMoney == 0.0 && winMoney == 0.0)
            {
                Sender.Tell(true);
                return;
            }
            double redundency   = getRedundency(websiteID, poolIndex);
            if (websiteID == 0)
            {
                double totalBet = _totalBets[poolIndex] + betMoney;
                double totalWin = _totalWins[poolIndex] + winMoney;

                double maxTotalWin = totalBet + redundency;
                if (totalWin > maxTotalWin)
                {
                    Sender.Tell(false);
                    return;
                }

                _totalBets[poolIndex] = totalBet;
                _totalWins[poolIndex] = totalWin;
                _changePoolStatus[0]  = true;
                Sender.Tell(true);
            }
            else
            {
                if (!_websiteTotalBets.ContainsKey(websiteID))
                    _websiteTotalBets[websiteID] = new double[PoolCount];

                if (!_websiteTotalWins.ContainsKey(websiteID))
                    _websiteTotalWins[websiteID] = new double[PoolCount];

                double totalBet = _websiteTotalBets[websiteID][poolIndex] + betMoney;
                double totalWin = _websiteTotalWins[websiteID][poolIndex] + winMoney;

                double maxTotalWin = totalBet + redundency;
                if (totalWin > maxTotalWin)
                {
                    Sender.Tell(false);
                    return;
                }

                _websiteTotalBets[websiteID][poolIndex] = totalBet;
                _websiteTotalWins[websiteID][poolIndex] = totalWin;
                _changePoolStatus[websiteID] = true;
                Sender.Tell(true);
            }
        }
        protected void onWebsiteRedundencyUpdated(WebsiteRedundencyUpdated request)
        {
            if(request.WebsiteID == 0)
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
            if (!_websiteRedundency.ContainsKey(websiteID))
                return 0.0;
            return _websiteRedundency[websiteID][poolID];
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
            WebsiteID = websiteID;
            BetMoney  = Math.Round(betMoney, 2);
            WinMoney  = Math.Round(winMoney, 2);
            PoolID    = poolID;
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
            WebsiteID = websiteID;
            BetMoney  = Math.Round(betMoney, 2);
            WinMoney  = Math.Round(winMoney, 2);
            PoolID = poolID;
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
        public double   Redundency { get; private set; }
        public WebsiteRedundencyUpdated(int websiteID, double redundency)
        {
            WebsiteID = websiteID;
            Redundency = redundency;
        }
    }
    public class UpdatePayoutPoolStatus
    {
        public Dictionary<int, double[]> PoolStatus { get; set; }
        public UpdatePayoutPoolStatus(Dictionary<int, double[]> poolStatus)
        {
            PoolStatus = poolStatus;
        }
    }
}
