using Akka.Actor;
using GITProtocol;
using GITProtocol.Utils;
using MongoDB.Bson;
using PCGSharp;
using SlotGamesNode.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class AztecBlazeGameLogic : BasePPSlotGame
    {
        protected int _normalMaxID2         = 0;
        protected int _naturalSpinCount2    = 0;
        protected int _emptySpinCount2      = 0;
        protected int _anteStartID          = 0;

        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs25kfruit";
            }
        }
        protected override bool SupportReplay
        {
            get
            {
                return true;
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 25;
            }
        }
        protected override int ServerResLineCount
        {
            get { return 25; }
        }
        protected override int ROWS
        {
            get
            {
                return 4;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=4,7,5,3,7,4,6,7,9,4,9,5,3,8,6,4,10,3,3,9,10,6,8,8&cfgs=6526&ver=3&def_sb=4,8,10,4,10,5&reel_set_size=1&def_sa=5,9,10,4,8,4&scatters=1~2,0~0,0~1,1&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"14285000\",max_rnd_win:\"2000\",max_rnd_win_a:\"1250\",max_rnd_hr_a:\"6100000\"}}&wl_i=tbm~2000;tbm_a~1250&sc=8.00,16.00,24.00,32.00,40.00,80.00,120.00,160.00,200.00,300.00,400.00,600.00,1000.00,2000.00,3000.00,4000.00&defc=40.00&purInit_e=1&wilds=2~200,100,50,25,0,0~1,1,1,1,1,1&bonuses=0&bls=25,40&paytable=0,0,0,0,0,0;0,0;0,0,0,0,0,0;200,100,50,25,0,0;150,75,40,20,0,0;100,50,25,15,0,0;100,50,25,15,0,0;35,25,15,4,0,0;30,20,12,4,0,0;25,18,10,3,0,0;20,15,8,3,0,0&total_bet_max=6,000,000.00&reel_set0=8,8,10,10,10,10,5,5,5,5,7,7,7,7,3,3,3,3,6,9,4,4,4,4,5,5,5,5,8,8,7,7,7,7,9,9,10,10,6,4,4,4,4,3,3,3,3,2,2,2,2,10,10,7,7,6,6,6,6,9,3,3,5,5,7,6,6,8,8,8,8,9,9,10,10,10,10,3,4,4,8,9,6,6,10,5,5,8,7,7,7,7,4,4,4,4,3,3,6,6,9,9,5,1,7,7,7,7,4,4,4,4,8,8,10,10,10,10,2,2,9,9,6,7,4,4,4,4,8,10,10,10,10,9,9,9,9,6,2,7,7,7,7,3,3,4,8,8,8,8,10,10,7,7~10,10,10,10,7,7,5,8,6,6,10,10,10,10,9,9,5,5,8,8,4,4,4,4,6,6,6,6,9,5,5,10,10,4,4,4,4,3,3,3,3,7,9,10,6,6,4,7,8,5,10,10,4,3,3,9,9,8,8,10,10,5,5,5,5,4,4,4,4,9,9,8,8,8,8,2,2,2,2,10,10,10,10,4,4,6,5,5,5,5,8,7,3,3,2,2,10,10,10,10,6,6,5,5,7,7,3,8,10,10,10,10,2,5,5,9,3,8,8,8,8,10,10,6,6,5,5,9,9,9,9,3,7,7,7,7,6,6,6,6,10,10,8,9,3,3,2,2,5,5,5,5~8,8,6,9,10,4,4,4,4,8,8,7,6,6,10,10,2,2,3,3,3,3,8,8,9,9,9,9,5,5,5,5,10,3,8,8,7,7,7,7,9,9,9,9,10,10,2,2,2,2,5,5,5,5,7,7,6,6,6,6,9,9,9,9,4,4,4,4,3,3,3,3,10,10,7,7,8,8,9,9,6,6,3,3,10,10,10,10,4,7,7,2,8,8,8,8,5,9,6,4,10,10,7,8,8,5,5,5,5,9,9,4,10,6,6,8,7,7,9,9,10,10,10,10,4,8,8,6,6,5,5,7,7,9,4,4,6,3,3,7,7,9,9,9,9,10,10,2,3,3,3,3,7,7,7,7~3,3,3,3,10,5,5,7,7,9,9,9,9,6,4,4,8,8,8,8,10,10,3,3,9,9,5,7,8,10,9,3,6,6,6,6,4,7,7,2,2,2,2,3,9,9,9,9,8,8,8,8,4,4,10,10,3,3,7,7,8,8,4,4,4,4,10,10,10,10,5,5,6,6,3,3,3,3,9,9,9,9,7,5,4,4,8,8,10,10,10,10,7,5,5,5,5,6,6,9,9,4,4,4,4,8,8,10,3,3,3,3,7,9,9,8,8,10,10,5,5,6,6,6,6,9,4,2,5,5,10,10,10,10,3,3,8,8,4,2,2,7,7,7,7,9,9,3,3,4,4,4,4~7,5,5,4,4,6,6,10,10,8,5,5,3,3,3,3,9,9,9,9,6,8,4,4,4,4,10,5,5,5,5,7,7,7,7,8,3,10,10,4,4,5,5,5,5,9,7,7,3,6,6,2,9,9,8,8,5,5,5,5,4,6,3,3,8,5,5,4,4,9,9,7,10,10,2,2,3,3,3,3,4,9,9,9,9,8,8,8,8,5,10,10,6,6,6,6,9,9,3,3,3,3,5,4,4,4,4,6,6,10,10,8,5,9,9,9,9,3,3,4,4,4,4,10,10,8,7,7,7,7,6,6,6,6,4,4,5,5,5,5,9,8,8,10,10,10,10,7,7,2,2,2,2~5,5,4,7,7,7,7,8,6,6,6,6,5,5,5,5,9,9,10,8,8,6,5,5,3,3,4,4,8,8,7,6,3,5,9,10,4,4,8,8,8,8,6,6,5,5,3,3,10,9,9,9,9,7,5,5,5,5,6,6,6,6,2,4,4,4,4,7,7,10,10,10,10,8,8,9,3,3,4,7,7,10,10,10,10,8,8,2,2,6,6,5,5,3,4,4,4,4,8,8,8,8,10,10,6,1,5,5,5,5,4,9,9,9,9,8,6,6,6,6,5,5,7,7,4,4,4,4,3,3,8,6,6,6,6,9,10,10,10,10,7,3,8,6,6,2,2,2,2,5,5,3,3,3,3&purInit=[{bet:1500,type:\"default\"}]&total_bet_min=8.00";
            }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 60.0; }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override bool HasPurEnableOption
        {
            get { return true; }
        }
        protected override double MoreBetMultiple
        {
            get { return 1.6; }
        }
        protected override bool SupportMoreBet
        {
            get { return true; }
        }
        #endregion
        public AztecBlazeGameLogic()
        {
            _gameID = GAMEID.AztecBlaze;
            GameName = "AztecBlaze";
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
            dicParams["reel_set"] = "0";
            dicParams["st"] = "rect";
            dicParams["sw"] = "6";
            dicParams["bl"] = "0";
            dicParams["g"] = "{overlay:{def_s:\"16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16,16\",def_sa:\"16,16,16,16,16,16\",def_sb:\"16,16,16,16,16,16\",sh:\"7\",st:\"rect\",sw:\"6\"}}";
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID)
        {
            try
            {
                BasePPSlotBetInfo betInfo = new BasePPSlotBetInfo();
                betInfo.BetPerLine = (float)message.Pop();
                betInfo.LineCount = (int)message.Pop();

                int bl = (int)message.Pop();
                if (bl == 0)
                    betInfo.MoreBet = false;
                else
                    betInfo.MoreBet = true;

                if (message.DataNum >= 3)
                    betInfo.PurchaseFree = true;
                else
                    betInfo.PurchaseFree = false;

                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in WildWestGoldMegaGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
                    return;
                }

                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in WildWestGoldMegaGameLogic::readBetInfoFromMessage", strUserID);
                    return;
                }
                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1}", strUserID, betInfo.LineCount);
                    return;
                }

                BasePPSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strUserID, out oldBetInfo))
                {
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine = betInfo.BetPerLine;
                    oldBetInfo.LineCount = betInfo.LineCount;
                    oldBetInfo.MoreBet = betInfo.MoreBet;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in WildWestGoldMegaGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBet = (double)infoDocument["defaultbet"];
                _normalMaxID        = (int)infoDocument["normalmaxid1"];
                _emptySpinCount     = (int)infoDocument["emptycount1"];
                _naturalSpinCount   = (int)infoDocument["normalselectcount1"];
                _normalMaxID2       = (int)infoDocument["normalmaxid2"];
                _emptySpinCount2    = (int)infoDocument["emptycount2"];
                _naturalSpinCount2  = (int)infoDocument["normalselectcount2"];

                if (SupportPurchaseFree)
                {
                    var purchaseOdds        = infoDocument["purchaseodds"] as BsonArray;
                    _totalFreeSpinWinRate   = (double)purchaseOdds[1];
                    _minFreeSpinWinRate     = (double)purchaseOdds[0];
                }

                if (this.SupportPurchaseFree && this.PurchaseFreeMultiple > _totalFreeSpinWinRate)
                    _logger.Error("freespin win rate doesn't satisfy condition {0}", this.GameName);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
            }
        }
        protected override async Task<BasePPSlotSpinData> selectEmptySpin(int websiteID, BasePPSlotBetInfo betInfo)
        {
            int id = Pcg.Default.Next(1, _emptySpinCount + 1);
            if(betInfo.MoreBet)
                id = Pcg.Default.Next(_anteStartID, _anteStartID + _emptySpinCount2);
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(new SelectSpinDataByIDRequest(GameName, id), TimeSpan.FromSeconds(10.0));
            return convertBsonToSpinData(spinDataDocument);
        }
        protected override async Task<OddAndIDData> selectRandomOddAndID(int websiteID, BasePPSlotBetInfo betInfo, bool isMoreBet)
        {
            double payoutRate   = getPayoutRate(websiteID);
            double randomDouble = Pcg.Default.NextDouble(0.0, 100.0);
            int selectedID = 0;
            if (randomDouble >= payoutRate || payoutRate == 0.0)
            {
                if (!isMoreBet)
                    selectedID = Pcg.Default.Next(1, _emptySpinCount + 1);
                else
                    selectedID = Pcg.Default.Next(_anteStartID, _anteStartID + _emptySpinCount2);

            }
            else if (isMoreBet)
            {
                selectedID = Pcg.Default.Next(_anteStartID, _anteStartID + _naturalSpinCount2);
            }
            else
            {
                selectedID = Pcg.Default.Next(1, _naturalSpinCount + 1);
            }

            OddAndIDData selectedOddAndID = new OddAndIDData();
            selectedOddAndID.ID = selectedID;
            return selectedOddAndID;
        }
        protected virtual async Task<BasePPSlotSpinData> selectRangeSpinDataByBetType(int websiteID, double minOdd, double maxOdd, int betType, BasePPSlotBetInfo betInfo)
        {
            var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                    new SelectSpinTypeOddRangeRequestWithBetType(GameName, -1, minOdd, maxOdd, -1, betType), TimeSpan.FromSeconds(10.0));

            if (spinDataDocument == null)
                return null;

            return convertBsonToSpinData(spinDataDocument);
        }
        public override async Task<BasePPSlotSpinData> selectRandomStop(int websiteID, UserBonus userBonus, double baseBet, bool isChangedLineCount, BasePPSlotBetInfo betInfo)
        {
            //프리스핀구입을 먼저 처리한다.
            if (this.SupportPurchaseFree && betInfo.PurchaseFree)
                return await selectPurchaseFreeSpin(websiteID, betInfo, baseBet, userBonus);

            if (userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus rangeOddBonus = userBonus as UserRangeOddEventBonus;
                if (baseBet.LE(rangeOddBonus.MaxBet, _epsilion))
                {
                    int betType = betInfo.MoreBet ? 1 : 0;
                    BasePPSlotSpinData spinDataEvent = await selectRangeSpinDataByBetType(websiteID, rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd, betType, betInfo);
                    if (spinDataEvent != null)
                    {
                        spinDataEvent.IsEvent = true;
                        return spinDataEvent;
                    }
                }
            }
            if (SupportMoreBet && betInfo.MoreBet)
                return await selectRandomStop(websiteID, betInfo, true);
            else
                return await selectRandomStop(websiteID, betInfo, false);
        }

    }
}
