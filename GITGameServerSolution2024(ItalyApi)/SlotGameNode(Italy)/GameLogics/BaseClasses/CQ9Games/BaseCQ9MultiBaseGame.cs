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
using PCGSharp;
using SlotGamesNode.Database;

namespace SlotGamesNode.GameLogics
{
   
    public class BaseCQ9MultiBaseGame : BaseCQ9SlotGame
    {
        protected virtual int _extraCount => 1;

        protected int[]                             _naturalSpinCountPerExtra       = new int[] { };
        protected SortedDictionary<double, int>[]   _naturalSpinOddProbsPerExtra    = new SortedDictionary<double, int>[] { };
        protected SortedDictionary<double, int[]>[] _totalSpinOddIdsPerExtra        = new SortedDictionary<double, int[]>[] { };
        protected List<int>[]                       _emptySpinIDsPerExtra           = new List<int>[] { };

        public BaseCQ9MultiBaseGame()
        {
            for(int i = 0; i < _extraCount; i++)
            {
                _naturalSpinCountPerExtra       = new int[_extraCount];
                _naturalSpinOddProbsPerExtra    = new SortedDictionary<double, int>[_extraCount]; 
                _totalSpinOddIdsPerExtra        = new SortedDictionary<double, int[]>[_extraCount];
                _emptySpinIDsPerExtra           = new List<int>[_extraCount];
            }
        }

        protected override async Task<bool> loadSpinData()
        {
            try
            {
                _spinDatabase   = Context.ActorOf(Props.Create(() => new SpinDatabase(this._providerName, this.GameName)), "spinDatabase");
                bool isSuccess  = await _spinDatabase.Ask<bool>("initialize", TimeSpan.FromSeconds(5.0));
                if (!isSuccess)
                {
                    _logger.Error("couldn't load spin data of game {0}", this.GameName);
                    return false;
                }

                ReadSpinInfoResponse response = await _spinDatabase.Ask<ReadSpinInfoResponse>(new ReadSpinInfoRequest(SpinDataTypes.MultiBase), TimeSpan.FromSeconds(30.0));
                if (response == null)
                {
                    _logger.Error("couldn't load spin odds information of game {0}", this.GameName);
                    return false;
                }

                _normalMaxID            = response.NormalMaxID;
                _spinDataDefaultBet     = response.DefaultBet;

                Dictionary<double, List<int>>[] totalSpinOddIdsPerExtra = new Dictionary<double, List<int>>[_extraCount];

                for (int i = 0; i < _extraCount; i++)
                {
                    _naturalSpinCountPerExtra[i]    = 0;
                    _naturalSpinOddProbsPerExtra[i] = new SortedDictionary<double, int>();
                    _emptySpinIDsPerExtra[i]        = new List<int>();
                    totalSpinOddIdsPerExtra[i]      = new Dictionary<double, List<int>>();
                }
                
                _naturalSpinCount = 0;
                
                for (int i = 0; i < response.SpinBaseDatas.Count; i++)
                {
                    SpinExtraData spinBaseData = response.SpinBaseDatas[i] as SpinExtraData;
                    int extra = spinBaseData.Extra;
                    if (spinBaseData.ID <= response.NormalMaxID)
                    {
                        _naturalSpinCountPerExtra[extra]++;
                        _naturalSpinCount++;

                        if (_naturalSpinOddProbsPerExtra[extra].ContainsKey(spinBaseData.Odd))
                            _naturalSpinOddProbsPerExtra[extra][spinBaseData.Odd]++;
                        else
                            _naturalSpinOddProbsPerExtra[extra][spinBaseData.Odd] = 1;
                    }
                    if (!totalSpinOddIdsPerExtra[extra].ContainsKey(spinBaseData.Odd))
                        totalSpinOddIdsPerExtra[extra].Add(spinBaseData.Odd, new List<int>());

                    if (spinBaseData.SpinType == 0 && spinBaseData.Odd == 0.0)
                        _emptySpinIDsPerExtra[extra].Add(spinBaseData.ID);

                    totalSpinOddIdsPerExtra[extra][spinBaseData.Odd].Add(spinBaseData.ID);
                }

                for(int i = 0; i < _extraCount; i++)
                {
                    _totalSpinOddIdsPerExtra[i] = new SortedDictionary<double, int[]>();
                    foreach (KeyValuePair<double, List<int>> pair in totalSpinOddIdsPerExtra[i])
                        _totalSpinOddIdsPerExtra[i].Add(pair.Key, pair.Value.ToArray());
                }

                double winRate = 0.0;
                for(int i = 0; i < _extraCount; i++)
                {
                    foreach (KeyValuePair<double, int> pair in _naturalSpinOddProbsPerExtra[i])
                        winRate += (pair.Key * pair.Value / _naturalSpinCount);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
                return false;
            }
        }

        public override async Task<BasePPSlotSpinData> selectRandomStop(int companyID, UserBonus userBonus, double baseBet, bool isChangedLineCount, bool isMustLose, BaseCQ9SlotBetInfo betInfo)
        {
            if(userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus  rangeOddBonus = userBonus as UserRangeOddEventBonus;
                int extra = betInfo.IsExtraBet;
                OddAndIDData oddAndID = selectOddAndIDFromProbsWithRange(_totalSpinOddIdsPerExtra[extra], rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd);
                if (oddAndID != null)
                {
                    BasePPSlotSpinData spinDataEvent = await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(oddAndID.ID), TimeSpan.FromSeconds(10.0));
                    spinDataEvent.IsEvent = true;
                    return spinDataEvent;
                }
            }

            return await selectRandomStop(companyID, betInfo.IsExtraBet);
        }

        protected override OddAndIDData selectRandomOddAndID(int companyID, int extra)
        {
            double payoutRate = _config.PayoutRate;
            if (_agentPayoutRates.ContainsKey(companyID))
                payoutRate = _agentPayoutRates[companyID];

            double randomDouble = Pcg.Default.NextDouble(0.0, 100.0);

            double selectedOdd = 0.0;
            if (randomDouble >= payoutRate || payoutRate == 0.0)
            {
                selectedOdd = 0.0;
            }
            else
            {
                selectedOdd = selectOddFromProbs(_naturalSpinOddProbsPerExtra[extra], _naturalSpinCountPerExtra[extra]);
            }

            if (!_totalSpinOddIdsPerExtra[extra].ContainsKey(selectedOdd))
                return null;

            int selectedID = _totalSpinOddIdsPerExtra[extra][selectedOdd][Pcg.Default.Next(0, _totalSpinOddIdsPerExtra[extra][selectedOdd].Length)];
            OddAndIDData selectedOddAndID = new OddAndIDData();
            selectedOddAndID.ID     = selectedID;
            selectedOddAndID.Odd    = selectedOdd;
            return selectedOddAndID;
        }

        protected override async Task<BasePPSlotSpinData> selectEmptySpin(BaseCQ9SlotBetInfo betInfo)
        {
            int id = _emptySpinIDsPerExtra[betInfo.IsExtraBet][Pcg.Default.Next(0, _emptySpinIDsPerExtra[betInfo.IsExtraBet].Count)];
            return await _spinDatabase.Ask<BasePPSlotSpinData>(new SelectSpinDataByIDRequest(id), TimeSpan.FromSeconds(10.0));
        }
    }
}
