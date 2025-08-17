using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
using GITProtocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class PayoutPoolActor : ReceiveActor
    {
        protected readonly ILoggingAdapter _logger = Logging.GetLogger(Context);

        protected static int PoolCount = 2;

        #region 베팅풀정보(각 웹사이트마다 있다)
        //protected Dictionary<int, double[]> _websiteTotalBets  = new Dictionary<int, double[]>();
        //protected Dictionary<int, double[]> _websiteTotalWins  = new Dictionary<int, double[]>();
        protected Dictionary<GAMEID, Dictionary<int, double[]>> _websiteTotalBetsPerGames = new Dictionary<GAMEID, Dictionary<int, double[]>>();
        protected Dictionary<GAMEID, Dictionary<int, double[]>> _websiteTotalWinsPerGames = new Dictionary<GAMEID, Dictionary<int, double[]>>();

        protected Dictionary<int, double[]> _websiteRedundency = new Dictionary<int, double[]>();
        #endregion

        #region 베팅풀정보(한 게임당 최대 2개의 베팅풀이 있다. 기본풀, 보조풀)
        //protected double[] _totalBets       = new double[PoolCount];
        //protected double[] _totalWins       = new double[PoolCount];
        protected Dictionary<GAMEID, double[]> _totalBetsPerGame = new Dictionary<GAMEID, double[]>();
        protected Dictionary<GAMEID, double[]> _totalWinsPerGame = new Dictionary<GAMEID, double[]>();

        protected double[] _maxRedundencys  = new double[PoolCount];
        #endregion
        protected Dictionary<int, bool> _changePoolStatus = new Dictionary<int, bool>();

        public PayoutPoolActor()
        {            
            ReceiveAsync<CheckWebsitePayoutRequest>     (onCheckWebsitePayout);
            ReceiveAsync<SumUpWebsiteBetWinRequest>     (onSumUpWebsiteBetWin);
            Receive<ResetWebsitePayoutPoolRequest>      (onResetWebsitePayoutPool);
            Receive<WebsiteRedundencyUpdated>           (onWebsiteRedundencyUpdated);
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
                //Context.System.Scheduler.ScheduleTellOnce(60000, Self, "updatePool", Self);
            }
            else if(strCommand == "updatePool")
            {
                //Dictionary<int, double[]> payoutPoolStatus = new Dictionary<int, double[]>();
                //foreach (KeyValuePair<int, bool> pair in _changePoolStatus)
                //{
                //    int websiteID = pair.Key;
                //    if (websiteID == 0)
                //        payoutPoolStatus.Add(0, new double[] { _totalBets[0] + _totalBets[1], _totalWins[0] + _totalWins[1] });
                //    else
                //        payoutPoolStatus.Add(websiteID, new double[] { _websiteTotalBets[websiteID][0] + _websiteTotalBets[websiteID][1], _websiteTotalWins[websiteID][0] + _websiteTotalWins[websiteID][1]});                    
                //}
                //if (payoutPoolStatus.Count > 0)
                //    Context.System.ActorSelection("/user/dbproxy/writer").Tell(new UpdatePayoutPoolStatus(payoutPoolStatus));
                //_changePoolStatus.Clear();
                //Context.System.Scheduler.ScheduleTellOnce(60000, Self, "updatePool", Self);
            }
        }
        protected async Task onSumUpWebsiteBetWin(SumUpWebsiteBetWinRequest request)
        {
            int     websiteID   = request.WebsiteID;
            GAMEID  gameID      = request.GameID;
            double  betMoney    = request.BetMoney;
            double  winMoney    = request.WinMoney;
            int     poolIndex   = request.PoolID;
            if (websiteID == 0)
            {
                //_totalBets[poolIndex] += betMoney;
                //_totalWins[poolIndex] += winMoney;
                if (!_totalBetsPerGame.ContainsKey(gameID))
                {
                    _totalBetsPerGame[gameID] = new double[PoolCount];
                    _totalWinsPerGame[gameID] = new double[PoolCount];
                }

                _totalBetsPerGame[gameID][poolIndex] += betMoney;
                _totalWinsPerGame[gameID][poolIndex] += winMoney;
            }
            else
            {
                //if (!_websiteTotalBets.ContainsKey(websiteID))
                //    _websiteTotalBets[websiteID] = new double[PoolCount];

                //if (!_websiteTotalWins.ContainsKey(websiteID))
                //    _websiteTotalWins[websiteID] = new double[PoolCount];

                //_websiteTotalBets[websiteID][poolIndex] += betMoney;
                //_websiteTotalWins[websiteID][poolIndex] += winMoney;

                if (!_websiteTotalBetsPerGames.ContainsKey(gameID))
                    _websiteTotalBetsPerGames[gameID] = new Dictionary<int, double[]>();

                if (!_websiteTotalWinsPerGames.ContainsKey(gameID))
                    _websiteTotalWinsPerGames[gameID] = new Dictionary<int, double[]>();

                if (!_websiteTotalBetsPerGames[gameID].ContainsKey(websiteID))
                    _websiteTotalBetsPerGames[gameID][websiteID] = new double[PoolCount];

                if (!_websiteTotalWinsPerGames[gameID].ContainsKey(websiteID))
                    _websiteTotalWinsPerGames[gameID][websiteID] = new double[PoolCount];

                _websiteTotalBetsPerGames[gameID][websiteID][poolIndex] += betMoney;
                _websiteTotalWinsPerGames[gameID][websiteID][poolIndex] += winMoney;

                double newRTP = 0.0;
                if(_websiteTotalBetsPerGames[gameID][websiteID][poolIndex] > 0)
                {
                    newRTP = _websiteTotalWinsPerGames[gameID][websiteID][poolIndex] / _websiteTotalBetsPerGames[gameID][websiteID][poolIndex];
                }

                if(newRTP > request.PayoutRate)
                {
                    string strMsg = string.Format("agent {0} game {1} RTP value is greater than config value in SlotNode PayoutPoolActor::onSumUpWebsiteBetWin : {2}/{3}={4} > {5}", 
                        websiteID, gameID.ToString(), _websiteTotalWinsPerGames[gameID][websiteID][poolIndex], _websiteTotalWinsPerGames[gameID][websiteID][poolIndex], newRTP, request.PayoutRate);
                    await SendTelegramMessage(strMsg);
                }
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
                    //_totalBets[i] = 0.0;
                    //_totalWins[i] = 0.0;

                    foreach (KeyValuePair<GAMEID, double[]> pair in _totalBetsPerGame)
                        pair.Value[PoolCount] = 0.0;

                    foreach (KeyValuePair<GAMEID, double[]> pair in _totalWinsPerGame)
                        pair.Value[PoolCount] = 0.0;
                }
                return;
            }

            //if (!_websiteTotalBets.ContainsKey(websiteID) || !_websiteTotalWins.ContainsKey(websiteID))
            //    return;

            //for (int i = 0; i < PoolCount; i++)
            //{
            //    _websiteTotalBets[websiteID][i] = 0.0;
            //    _websiteTotalWins[websiteID][i] = 0.0;
            //}

            for (int i = 0; i < PoolCount; i++)
            {
                foreach (KeyValuePair<GAMEID, Dictionary<int, double[]>> pair in _websiteTotalBetsPerGames)
                {
                    if (!pair.Value.ContainsKey(websiteID))
                        continue;

                    pair.Value[websiteID][i] = 0.0;
                }

                foreach (KeyValuePair<GAMEID, Dictionary<int, double[]>> pair in _websiteTotalWinsPerGames)
                {
                    if (!pair.Value.ContainsKey(websiteID))
                        continue;

                    pair.Value[websiteID][i] = 0.0;
                }
            }
        }
        protected async Task onCheckWebsitePayout(CheckWebsitePayoutRequest request)
        {
            int     websiteID   = request.WebsiteID;
            GAMEID  gameID      = request.GameID;
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
                //double totalBet = _totalBets[poolIndex] + betMoney;
                //double totalWin = _totalWins[poolIndex] + winMoney;

                //double maxTotalWin = totalBet + redundency;
                //if (totalWin > maxTotalWin)
                //{
                //    Sender.Tell(false);
                //    return;
                //}

                //_totalBets[poolIndex] = totalBet;
                //_totalWins[poolIndex] = totalWin;
                //_changePoolStatus[0]  = true;
                //Sender.Tell(true);

                if(!_totalBetsPerGame.ContainsKey(gameID))
                    _totalBetsPerGame[gameID] = new double[PoolCount];

                if (!_totalWinsPerGame.ContainsKey(gameID))
                    _totalWinsPerGame[gameID] = new double[PoolCount];

                double totalBet = _totalBetsPerGame[gameID][poolIndex] + betMoney;
                double totalWin = _totalWinsPerGame[gameID][poolIndex] + winMoney;

                double maxTotalWin = (totalBet + redundency) * request.PayoutRate / 100;
                if (totalWin > maxTotalWin)
                {
                    Sender.Tell(false);
                    return;
                }

                _totalBetsPerGame[gameID][poolIndex] = totalBet;
                _totalWinsPerGame[gameID][poolIndex] = totalWin;
                _changePoolStatus[0]  = true;
                Sender.Tell(true);
            }
            else
            {
                //if (!_websiteTotalBets.ContainsKey(websiteID))
                //    _websiteTotalBets[websiteID] = new double[PoolCount];

                //if (!_websiteTotalWins.ContainsKey(websiteID))
                //    _websiteTotalWins[websiteID] = new double[PoolCount];

                //double totalBet = _websiteTotalBets[websiteID][poolIndex] + betMoney;
                //double totalWin = _websiteTotalWins[websiteID][poolIndex] + winMoney;

                //double maxTotalWin = totalBet + redundency;
                //if (totalWin > maxTotalWin)
                //{
                //    Sender.Tell(false);
                //    return;
                //}

                //_websiteTotalBets[websiteID][poolIndex] = totalBet;
                //_websiteTotalWins[websiteID][poolIndex] = totalWin;
                //_changePoolStatus[websiteID] = true;
                //Sender.Tell(true);

                if (!_websiteTotalBetsPerGames.ContainsKey(gameID))
                    _websiteTotalBetsPerGames[gameID] = new Dictionary<int, double[]>();

                if (!_websiteTotalWinsPerGames.ContainsKey(gameID))
                    _websiteTotalWinsPerGames[gameID] = new Dictionary<int, double[]>();

                if (!_websiteTotalBetsPerGames[gameID].ContainsKey(websiteID))
                    _websiteTotalBetsPerGames[gameID][websiteID] = new double[PoolCount];

                if (!_websiteTotalWinsPerGames[gameID].ContainsKey(websiteID))
                    _websiteTotalWinsPerGames[gameID][websiteID] = new double[PoolCount];

                double totalBet = _websiteTotalBetsPerGames[gameID][websiteID][poolIndex] + betMoney;
                double totalWin = _websiteTotalWinsPerGames[gameID][websiteID][poolIndex] + winMoney;

                double maxTotalWin = (totalBet + redundency) * request.PayoutRate / 100;
                if (totalWin > maxTotalWin)
                {
                    Sender.Tell(false);
                    return;
                }

                _websiteTotalBetsPerGames[gameID][websiteID][poolIndex] = totalBet;
                _websiteTotalWinsPerGames[gameID][websiteID][poolIndex] = totalWin;
                _changePoolStatus[websiteID] = true;
                Sender.Tell(true);

                double newRTP = 0.0;
                if (_websiteTotalBetsPerGames[gameID][websiteID][poolIndex] > 0)
                {
                    newRTP = _websiteTotalWinsPerGames[gameID][websiteID][poolIndex] / _websiteTotalBetsPerGames[gameID][websiteID][poolIndex];
                }

                if (newRTP > request.PayoutRate)
                {
                    string strMsg = string.Format("PPServer {6} agent {0} game {1} RTP value is greater than config value in SlotNode PayoutPoolActor::onCheckWebsitePayout : {2}/{3}={4} > {5}",
                        websiteID, gameID.ToString(), _websiteTotalWinsPerGames[gameID][websiteID][poolIndex], _websiteTotalWinsPerGames[gameID][websiteID][poolIndex], newRTP, request.PayoutRate,"Test");
                    await SendTelegramMessage(strMsg);
                }
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

        protected async Task SendTelegramMessage(string message)
        {
            // 봇 토큰과 채팅 ID 입력
            string botToken = "8122661989:AAHUiwsPVlxnCSB6nLHLrNax9W9BQzykOFQ";
            string chatId = "-4291691816"; // 그룹의 채트아이디

            string url = $"https://api.telegram.org/bot{botToken}/sendMessage?chat_id={chatId}&text={Uri.EscapeDataString(message)}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    string result = await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    _logger.Error("Exception has been occurred in UserActor::SendTelegramMessage {0}", ex.Message);
                }
            }
        }
    }
    class CheckWebsitePayoutRequest
    {
        public int      WebsiteID   { get; private set; }
        public GAMEID   GameID      { get; private set; }
        public double   BetMoney    { get; private set; }
        public double   WinMoney    { get; private set; }
        public int      PoolID      { get; private set; }
        public double   PayoutRate  { get; private set; }

        public CheckWebsitePayoutRequest(int websiteID, GAMEID gameID, double betMoney, double winMoney, int poolID, double payoutRate)
        {
            WebsiteID   = websiteID;
            GameID      = gameID;
            BetMoney    = Math.Round(betMoney, 2);
            WinMoney    = Math.Round(winMoney, 2);
            PoolID      = poolID;
            PayoutRate  = payoutRate;
        }
    }
    class SumUpWebsiteBetWinRequest
    {
        public int      WebsiteID   { get; private set; }
        public GAMEID   GameID      { get; private set; }
        public double   BetMoney    { get; private set; }
        public double   WinMoney    { get; private set; }
        public int      PoolID      { get; private set; }
        public double   PayoutRate  { get; private set; }

        public SumUpWebsiteBetWinRequest(int websiteID, GAMEID gameID, double betMoney, double winMoney, int poolID, double payoutRate)
        {
            WebsiteID   = websiteID;
            GameID      = gameID;
            BetMoney    = Math.Round(betMoney, 2);
            WinMoney    = Math.Round(winMoney, 2);
            PoolID      = poolID;
            PayoutRate  = payoutRate;
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
