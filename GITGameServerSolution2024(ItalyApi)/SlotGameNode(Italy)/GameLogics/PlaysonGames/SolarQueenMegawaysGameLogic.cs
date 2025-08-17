using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Akka.Actor;
using GITProtocol;
using SlotGamesNode.Database;

namespace SlotGamesNode.GameLogics
{
    class SolarQueenMegawaysGameLogic : BasePlaysonGroupGameLogic
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "solar_queen_mega";
            }
        }
        protected override bool SupportPurchaseFree
        {
            get
            {
                return true;
            }
        }
        protected override double PurchaseFreeMultiple
        {
            get
            {
                return 100.0;
            }
        }
        protected override int[] StakeIncrement
        {
            get
            {
                return new int[] { 1, 2, 3, 4, 5, 8, 10, 20, 30, 40, 60, 90, 100, 200, 300, 400, 500 };
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "<server><state current_state=\"idle\"/><config><slot><combinations><combination symbol=\"1\" name=\"wild\" count=\"2\" coef=\"1\"/><combination symbol=\"1\" name=\"wild\" count=\"3\" coef=\"3\"/><combination symbol=\"1\" name=\"wild\" count=\"4\" coef=\"20\"/><combination symbol=\"1\" name=\"wild\" count=\"5\" coef=\"40\"/><combination symbol=\"2\" name=\"top1\" count=\"2\" coef=\"1\"/><combination symbol=\"2\" name=\"top1\" count=\"3\" coef=\"3\"/><combination symbol=\"2\" name=\"top1\" count=\"4\" coef=\"20\"/><combination symbol=\"2\" name=\"top1\" count=\"5\" coef=\"40\"/><combination symbol=\"3\" name=\"top2\" count=\"3\" coef=\"2\"/><combination symbol=\"3\" name=\"top2\" count=\"4\" coef=\"10\"/><combination symbol=\"3\" name=\"top2\" count=\"5\" coef=\"30\"/><combination symbol=\"4\" name=\"top3\" count=\"3\" coef=\"2\"/><combination symbol=\"4\" name=\"top3\" count=\"4\" coef=\"10\"/><combination symbol=\"4\" name=\"top3\" count=\"5\" coef=\"30\"/><combination symbol=\"5\" name=\"med1\" count=\"3\" coef=\"2\"/><combination symbol=\"5\" name=\"med1\" count=\"4\" coef=\"5\"/><combination symbol=\"5\" name=\"med1\" count=\"5\" coef=\"20\"/><combination symbol=\"6\" name=\"med2\" count=\"3\" coef=\"2\"/><combination symbol=\"6\" name=\"med2\" count=\"4\" coef=\"5\"/><combination symbol=\"6\" name=\"med2\" count=\"5\" coef=\"20\"/><combination symbol=\"7\" name=\"med3\" count=\"3\" coef=\"2\"/><combination symbol=\"7\" name=\"med3\" count=\"4\" coef=\"5\"/><combination symbol=\"7\" name=\"med3\" count=\"5\" coef=\"20\"/><combination symbol=\"8\" name=\"med4\" count=\"3\" coef=\"2\"/><combination symbol=\"8\" name=\"med4\" count=\"4\" coef=\"5\"/><combination symbol=\"8\" name=\"med4\" count=\"5\" coef=\"20\"/><combination symbol=\"9\" name=\"low1\" count=\"3\" coef=\"1\"/><combination symbol=\"9\" name=\"low1\" count=\"4\" coef=\"2\"/><combination symbol=\"9\" name=\"low1\" count=\"5\" coef=\"10\"/><combination symbol=\"10\" name=\"low2\" count=\"3\" coef=\"1\"/><combination symbol=\"10\" name=\"low2\" count=\"4\" coef=\"2\"/><combination symbol=\"10\" name=\"low2\" count=\"5\" coef=\"10\"/><combination symbol=\"11\" name=\"low3\" count=\"3\" coef=\"1\"/><combination symbol=\"11\" name=\"low3\" count=\"4\" coef=\"2\"/><combination symbol=\"11\" name=\"low3\" count=\"5\" coef=\"10\"/><combination symbol=\"12\" name=\"low4\" count=\"3\" coef=\"1\"/><combination symbol=\"12\" name=\"low4\" count=\"4\" coef=\"2\"/><combination symbol=\"12\" name=\"low4\" count=\"5\" coef=\"10\"/><combination symbol=\"13\" name=\"low5\" count=\"3\" coef=\"1\"/><combination symbol=\"13\" name=\"low5\" count=\"4\" coef=\"2\"/><combination symbol=\"13\" name=\"low5\" count=\"5\" coef=\"10\"/><combination symbol=\"15\" name=\"scatter\" count=\"3\" coef=\"10\" multi_coef=\"1\" multi_coef2=\"0\"/></combinations><symbols><symbol id=\"1\" title=\"wild\"/><symbol id=\"2\" title=\"top1\"/><symbol id=\"3\" title=\"top2\"/><symbol id=\"4\" title=\"top3\"/><symbol id=\"5\" title=\"med1\"/><symbol id=\"6\" title=\"med2\"/><symbol id=\"7\" title=\"med3\"/><symbol id=\"8\" title=\"med4\"/><symbol id=\"9\" title=\"low1\"/><symbol id=\"10\" title=\"low2\"/><symbol id=\"11\" title=\"low3\"/><symbol id=\"12\" title=\"low4\"/><symbol id=\"13\" title=\"low5\"/><symbol id=\"14\" title=\"bonus\"/><symbol id=\"15\" title=\"scatter\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"11\" symbols=\"8,5,4,10,9,3,6,13,12,7,11\"/><reel id=\"2\" length=\"11\" symbols=\"6,10,4,13,3,8,12,11,9,5,7\"/><reel id=\"3\" length=\"11\" symbols=\"3,12,8,4,9,5,13,6,7,11,10\"/><reel id=\"4\" length=\"11\" symbols=\"6,5,4,3,10,11,8,12,9,13,7\"/><reel id=\"5\" length=\"11\" symbols=\"10,8,11,4,9,12,13,5,6,3,7\"/></reels><reels id=\"2\"><reel id=\"1\" length=\"12\" symbols=\"3,11,4,13,7,6,8,9,12,2,5,10\"/><reel id=\"2\" length=\"12\" symbols=\"6,3,4,7,8,5,11,2,12,9,13,10\"/><reel id=\"3\" length=\"12\" symbols=\"4,11,2,9,8,3,10,5,7,6,12,13\"/><reel id=\"4\" length=\"12\" symbols=\"9,4,13,11,6,7,3,5,10,12,8,2\"/><reel id=\"5\" length=\"12\" symbols=\"7,9,10,13,6,4,5,3,12,11,8,2\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"13,3,3,11,8\" reel2=\"2,2,10\" reel3=\"4,7,10\" reel4=\"11,6,5,2,2,2\" reel5=\"3,5,3\"/><shift_ext><shift server=\"0,0,0,0,0\" reel_set=\"2\" reel1=\"4,4,13\" reel2=\"5,11,2,2\" reel3=\"8,3,9,3,8,11\" reel4=\"5,5,6\" reel5=\"2,2,2,12,12\"/><shift server=\"0,0,0,0,0\" reel_set=\"2\" reel1=\"12,3,4,6,6,13\" reel2=\"5,11,2\" reel3=\"2,9,8,11\" reel4=\"3,5\" reel5=\"5,4,8,7,2\"/><shift server=\"0,0,0,0,0\" reel_set=\"2\" reel1=\"3,12,9,9\" reel2=\"8,5\" reel3=\"4,11,2,2,2\" reel4=\"9,4,13\" reel5=\"7,7,9\"/><shift server=\"0,0,0,0,0\" reel_set=\"2\" reel1=\"7,2,2,9\" reel2=\"4,10,8\" reel3=\"7,5,3,3,11\" reel4=\"2,11\" reel5=\"6,4,9,13,5,3\"/><shift server=\"0,0,0,0,0\" reel_set=\"2\" reel1=\"3,12,9,9\" reel2=\"7,2\" reel3=\"4,11,7,4,13\" reel4=\"3,2,7\" reel5=\"7,7,9\"/><shift server=\"0,0,0,0,0\" reel_set=\"2\" reel1=\"12,3,7,6\" reel2=\"2,8,8,12,12,4\" reel3=\"4,11,2\" reel4=\"9,5,7,4\" reel5=\"5,7\"/></shift_ext></config><game state=\"slot\" free_game_cost=\"100\" feature_rounds_limit=\"10\" total_bet_mult=\"20\"><bets/></game><source game-ver=\"57735\"/></server>";
            }
        }
        #endregion
        
        public SolarQueenMegawaysGameLogic()
        {
            _gameID     = GAMEID.SolarQueenMegaways;
            GameName    = "SolarQueenMegaways";
        }

        protected override async Task<bool> loadSpinData()
        {
            try
            {
                _spinDatabase = Context.ActorOf(Props.Create(() => new SpinDatabase(_providerName, this.GameName)), "spinDatabase");
                bool isSuccess = await _spinDatabase.Ask<bool>("initialize", TimeSpan.FromSeconds(5.0));
                if (!isSuccess)
                {
                    _logger.Error("couldn't load spin data of game {0}", this.GameName);
                    return false;
                }

                ReadSpinInfoResponse response = await _spinDatabase.Ask<ReadSpinInfoResponse>(new ReadSpinInfoRequest(SpinDataTypes.Normal), TimeSpan.FromSeconds(30.0));
                if (response == null)
                {
                    _logger.Error("couldn't load spin odds information of game {0}", this.GameName);
                    return false;
                }

                _normalMaxID            = response.NormalMaxID;
                _naturalSpinOddProbs    = new SortedDictionary<double, int>();
                _spinDataDefaultBet     = response.DefaultBet;
                _naturalSpinCount       = 0;
                _emptySpinIDs           = new List<int>();

                Dictionary<double, List<int>> totalSpinOddIds   = new Dictionary<double, List<int>>();
                Dictionary<double, List<int>> freeSpinOddIds    = new Dictionary<double, List<int>>();

                double freeSpinTotalOdd     = 0.0;
                double minFreeSpinTotalOdd  = 0.0;
                int freeSpinTotalCount      = 0;
                int minFreeSpinTotalCount   = 0;
                for (int i = 0; i < response.SpinBaseDatas.Count; i++)
                {
                    SpinBaseData spinBaseData = response.SpinBaseDatas[i];
                    if (spinBaseData.ID <= response.NormalMaxID)
                    {
                        _naturalSpinCount++;
                        if (_naturalSpinOddProbs.ContainsKey(spinBaseData.Odd))
                            _naturalSpinOddProbs[spinBaseData.Odd]++;
                        else
                            _naturalSpinOddProbs[spinBaseData.Odd] = 1;
                    }

                    if (!totalSpinOddIds.ContainsKey(spinBaseData.Odd))
                        totalSpinOddIds.Add(spinBaseData.Odd, new List<int>());

                    if (spinBaseData.SpinType == 0 && spinBaseData.Odd == 0.0)
                        _emptySpinIDs.Add(spinBaseData.ID);

                    if (spinBaseData.SpinType < 2)
                    {
                        totalSpinOddIds[spinBaseData.Odd].Add(spinBaseData.ID);
                    }

                    if (spinBaseData.SpinType == 2)
                    {
                        freeSpinTotalCount++;
                        freeSpinTotalOdd += spinBaseData.Odd;
                        if (!freeSpinOddIds.ContainsKey(spinBaseData.Odd))
                            freeSpinOddIds.Add(spinBaseData.Odd, new List<int>());
                        freeSpinOddIds[spinBaseData.Odd].Add(spinBaseData.ID);

                        if (spinBaseData.Odd >= PurchaseFreeMultiple * 0.2 && spinBaseData.Odd <= PurchaseFreeMultiple * 0.5)
                        {
                            minFreeSpinTotalCount++;
                            minFreeSpinTotalOdd += spinBaseData.Odd;
                        }
                    }
                }

                _totalSpinOddIds = new SortedDictionary<double, int[]>();
                foreach (KeyValuePair<double, List<int>> pair in totalSpinOddIds)
                    _totalSpinOddIds.Add(pair.Key, pair.Value.ToArray());

                if (SupportPurchaseFree)
                {
                    _totalFreeSpinOddIds = new SortedDictionary<double, int[]>();
                    foreach (KeyValuePair<double, List<int>> pair in freeSpinOddIds)
                        _totalFreeSpinOddIds.Add(pair.Key, pair.Value.ToArray());

                    _freeSpinTotalCount     = freeSpinTotalCount;
                    _minFreeSpinTotalCount  = minFreeSpinTotalCount;
                    _totalFreeSpinWinRate   = freeSpinTotalOdd / freeSpinTotalCount;
                    _minFreeSpinWinRate     = minFreeSpinTotalOdd / minFreeSpinTotalCount;

                    if (_totalFreeSpinWinRate <= _minFreeSpinWinRate || _minFreeSpinTotalCount == 0)
                        _logger.Error("min freespin rate doesn't satisfy condition {0}", this.GameName);
                }

                if (SupportPurchaseFree && PurchaseFreeMultiple > _totalFreeSpinWinRate)
                    _logger.Error("freespin win rate doesn't satisfy condition {0}", GameName);

                double winRate = 0.0;
                foreach (KeyValuePair<double, int> pair in _naturalSpinOddProbs)
                    winRate += (pair.Key * pair.Value / _naturalSpinCount);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
                return false;
            }
        }

        protected override void addLastResultForStart(XmlDocument responseDoc, string strGlobalUserID)
        {
            base.addLastResultForStart(responseDoc, strGlobalUserID);
            XmlElement betsElement = responseDoc.CreateElement("bets");
            
            if (_dicUserBetInfos.ContainsKey(strGlobalUserID))
            {
                BasePlaysonGroupSlotBetInfo betInfo = _dicUserBetInfos[strGlobalUserID] as BasePlaysonGroupSlotBetInfo;
                foreach(KeyValuePair<float, string> pair in betInfo.SeqResultStrings)
                {
                    XmlDocument lastResult = new XmlDocument();
                    lastResult.LoadXml(pair.Value);

                    XmlNode lastGameNode = lastResult.SelectSingleNode("/server/game");

                    XmlElement betElement       = responseDoc.CreateElement("bet");
                    XmlElement betValueElement  = responseDoc.CreateElement("value");
                    betValueElement.InnerXml    = ((int)pair.Key).ToString();

                    XmlNode stepNode = lastGameNode.SelectSingleNode("step10spins");
                    int featureRoundProgress    = Convert.ToInt32(stepNode.InnerXml);
                    featureRoundProgress %= 10;
                    stepNode.InnerXml= featureRoundProgress.ToString();

                    XmlElement progressElement  = responseDoc.CreateElement("progress");
                    foreach(XmlNode xn in lastGameNode.ChildNodes)
                    {
                        XmlNode copiedNode          = responseDoc.ImportNode(xn, true);
                        progressElement.AppendChild(copiedNode);
                    }

                    betElement.AppendChild(betValueElement);
                    betElement.AppendChild(progressElement);
                    betsElement.AppendChild(betElement);
                }

                XmlNode serverGameNode = responseDoc.SelectSingleNode("/server/game");
                serverGameNode.AppendChild(betsElement);
            }
        }
    }
}
