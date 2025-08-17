using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;
using System.IO;
using Akka.Actor;
using Newtonsoft.Json;
using Akka.Configuration;
using Newtonsoft.Json.Linq;
using SlotGamesNode.Database;

namespace SlotGamesNode.GameLogics
{
   
    class Thor2GameLogic : BaseCQ9SlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "182";
            }
        }
        protected override int ClientReqMinBet
        {
            get
            {
                return 30;
            }
        }
        protected override int[] DenomDefine
        {
            get
            {
                return new int[] { 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
            }
        }
        protected override int[] BetButton
        {
            get
            {
                return new int[] { 125, 250, 500, 750, 1250, 1, 2, 5, 10, 30, 50, 80 };
            }
        }
        protected override int MaxBet
        {
            get
            {
                return 1250;
            }
        }
        protected override string InitReelSetString
        {
            get
            {
                return "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";
            }
        }
        protected override string CQ9GameName
        {
            get
            {
                return "Thor 2";
            }
        }
        protected override string CQ9GameNameSet
        {
            get
            {
                return "[{\"lang\": \"ko\", \"name\": \"토르2\"}," +
                        "{ \"lang\": \"zh-cn\", \"name\": \"雷神 2\"}," +
                        "{ \"lang\": \"en\", \"name\": \"Thor 2\"}," +
                        "{ \"lang\": \"th\", \"name\": \"ธอร์ 2\"}," +
                        "{ \"lang\": \"es\", \"name\": \"Thor 2\"}," +
                        "{ \"lang\": \"ja\", \"name\": \"ソー 2\"}]";
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 4688.0 / 60; }
        }
        #endregion
        public Thor2GameLogic()
        {
            _initData.BetButton     = BetButton;
            _initData.MaxBet        = MaxBet;
            List<CQ9ExtendedFeature2> extendFeatureByGame2 = new List<CQ9ExtendedFeature2>()
            {
                new CQ9ExtendedFeature2(){ name = "FeatureMinBet",value = "2344" }
            };
            _initData.ExtendFeatureByGame2 = extendFeatureByGame2;

            _gameID                 = GAMEID.Thor2;
            GameName                = "Thor2";
        }

        protected override async Task<bool> loadSpinData()
        {
            try
            {
                _spinDatabase = Context.ActorOf(Akka.Actor.Props.Create(() => new SpinDatabase(_providerName, this.GameName)), "spinDatabase");
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

                    if(spinBaseData.SpinType < 2)
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

                _totalFreeSpinOddIds = new SortedDictionary<double, int[]>();
                foreach (KeyValuePair<double, List<int>> pair in freeSpinOddIds)
                    _totalFreeSpinOddIds.Add(pair.Key, pair.Value.ToArray());

                _freeSpinTotalCount     = freeSpinTotalCount;
                _minFreeSpinTotalCount  = minFreeSpinTotalCount;
                _totalFreeSpinWinRate   = freeSpinTotalOdd / freeSpinTotalCount;
                _minFreeSpinWinRate     = minFreeSpinTotalOdd / minFreeSpinTotalCount;

                if (_totalFreeSpinWinRate <= _minFreeSpinWinRate || _minFreeSpinTotalCount == 0)
                    _logger.Error("min freespin rate doesn't satisfy condition {0}", this.GameName);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
                return false;
            }
        }

        protected override string changeResultStringForHistory(string resultString, BaseCQ9SlotBetInfo betInfo)
        {
            dynamic resultContext = JsonConvert.DeserializeObject<dynamic>(resultString);
            int id = Convert.ToInt32(resultContext["ID"]);
            if (id == 142)
            {
                do
                {
                    JArray winLineArray = resultContext["udsOutputWinLine"] as JArray;
                    JArray lineExtraData = new JArray();
                    for (int i = 0; i < winLineArray.Count; i++)
                    {
                        if (string.Equals(Convert.ToString(winLineArray[i]["SymbolId"]), "47"))
                            lineExtraData = winLineArray[i]["LineExtraData"] as JArray;
                    }
                    if (lineExtraData.Count == 0)
                        break;

                    JArray symbolResult = resultContext["SymbolResult"];
                    for (int i = 0; i < symbolResult.Count; i++)
                    {
                        string[] row = symbolResult[i].ToString().Split(',');
                        for (int j = 0; j < row.Length; j++)
                        {
                            if (row[j] == "47" && Convert.ToInt32(lineExtraData[j]) == 1)
                                row[j] = "SC2";
                            else if (row[j] == "47" && Convert.ToInt32(lineExtraData[j]) == 2)
                                row[j] = "SC3";
                        }
                        string str = String.Join(",", row);
                        symbolResult[i] = str;
                    }
                    resultContext["SymbolResult"] = symbolResult;
                }
                while (false);
            }
            else if(id == 131)
            {
                JArray symbolResult     = resultContext["SymbolResult"];
                string specialSymbol    = Convert.ToString(resultContext["SpecialSymbol"]);
                for (int i = 0; i < symbolResult.Count; i++)
                {
                    string[] row = symbolResult[i].ToString().Split(',');
                    for (int j = 0; j < row.Length; j++)
                    {
                        if (row[j] == "7")
                            row[j] = specialSymbol;
                    }
                    string str = String.Join(",", row);
                    symbolResult[i] = str;
                }
                resultContext["SymbolResult"] = symbolResult;

            }
            
            return JsonConvert.SerializeObject(resultContext);
        }
    }
}
