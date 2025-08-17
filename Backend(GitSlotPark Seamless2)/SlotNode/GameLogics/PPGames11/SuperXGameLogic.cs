using Akka.Actor;
using GITProtocol;
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
    //앤티베팅을 프리스핀구매방식으로 처리한다.(앤티베팅이 이벤트에 들어가서는 안된다)
    class SuperXGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20superx";
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
            get { return 20; }
        }
        protected override int ServerResLineCount
        {
            get { return 20; }
        }
        protected override int ROWS
        {
            get
            {
                return 3;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "def_s=8,9,7,3,4,5,6,7,8,9,5,3,4,5,6&cfgs=4795&ver=2&def_sb=4,5,6,7,8&reel_set_size=1&def_sa=6,7,8,9,3&scatters=1~0,0,0,0,0~0,0,0,0,0~1,1,1,1,1&gmb=0,0,0&rt=d&gameInfo={props:{max_rnd_sim:\"1\",max_rnd_hr:\"279546174\",max_rnd_win:\"9000\"}}&wl_i=tbm~9000;tbm_a~900&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&wilds=2~0,0,0,0,0~1,1,1,1,1&bonuses=0&fsbonus=&bls=20,200&paytable=0,0,0,0,0,0;0,0,0,0,0,0;0,0,0,0,0,0;400,150,25,0,0;300,80,20,0,0;200,50,15,0,0;100,30,10,0,0;50,15,5,0,0;20,5,2,0,0;20,5,2,0,0;20,5,2,0,0;20,5,2,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0;0,0,0,0,0&total_bet_max=1,000,000.00&reel_set0=4,10,4,11,8,6,11,11,10,7,8,5,11,9,9,4,4,7,6,6,6,6,6,5,8,10,10,11,10,9,10,8,3,10,9,9,8,8,7,4,9,7,8,8,8,10,8,5,8,9,10,10,4,8,10,9,5,8,1,1,10,11,8,7,4,8,5,5,5,3,10,1,7,3,11,5,3,9,1,3,9,9,6,9,8,12,5,4,8,9,9,9,5,6,5,7,7,11,12,5,8,9,8,1,8,6,11,11,4,10,8,10,11,11,11,12,7,10,11,10,10,6,4,7,7,9,10,7,11,11,9,10,11,3,3,5,10,10,10,11,9,11,6,8,6,10,3,7,9,8,8,5,11,8,10,9,5,8,10,3,3,3,9,7,6,8,1,6,1,9,1,12,10,8,4,8,9,10,12,11,7,6,7,7,7,1,5,11,10,10,11,5,9,1,7,1,9,7,4,11,9,7,9,11,1,11,4,4,4,5,3,8,11,11,6,9,7,6,9,1,10,5,11,7,6,4,10,5,11,12,12,12,10,9,7,6,6,1,5,8,6,7,11,8,3,4,9,5,6,8,11,11,9,7~2,8,6,6,6,7,9,8,9,8,8,8,2,9,6,10,2,2,2,10,4,3,9,9,9,11,10,10,5,11,11,11,8,7,11,6,10,10,10,8,11,6,9,3,3,3,11,8,2,7,7,7,2,5,9,4,5,5,5,2,11,7,3,4,4,4,11,8,10,7,9~7,11,6,6,6,5,8,9,2,8,8,8,8,3,9,9,5,5,5,11,11,2,8,9,9,9,7,9,5,5,11,11,11,10,10,7,10,10,10,10,8,5,1,3,3,3,11,4,10,6,7,7,7,1,8,11,4,2,2,2,11,6,9,10,4,4,4,6,3,2,2,9~11,6,6,6,11,8,8,8,8,7,8,5,5,5,5,11,9,9,9,8,6,11,11,11,9,5,10,10,10,9,4,3,3,3,6,2,2,2,2,3,10,4,4,4,10,4,7,7,7,7,9,10~4,6,6,11,9,6,6,6,9,11,7,5,5,11,8,8,8,10,4,9,9,5,9,5,5,5,3,10,3,8,7,8,9,9,9,7,7,9,4,8,4,10,11,11,11,10,6,1,1,8,11,10,10,10,6,11,1,9,10,5,3,3,3,3,11,5,11,6,7,7,7,7,9,9,10,7,6,10,4,4,4,5,4,8,11,11,8,9,8&total_bet_min=10.00";
            }
        }
	
	
        protected override double MoreBetMultiple
        {
            get { return 10; }
        }
        protected override bool SupportMoreBet
        {
            get { return true; }
        }
	
        #endregion
        public SuperXGameLogic()
        {
            _gameID = GAMEID.SuperX;
            GameName = "SuperX";
        }
        protected override async Task onLoadSpinData(BsonDocument infoDocument)
        {
            try
            {
                _spinDataDefaultBet = (double)infoDocument["defaultbet"];
                _normalMaxID = (int)infoDocument["normalmaxid"];
                _emptySpinCount = (int)infoDocument["emptycount"];
                _naturalSpinCount = (int)infoDocument["normalselectcount"];
                var purchaseOdds = infoDocument["purchaseodds"] as BsonArray;
                _totalFreeSpinWinRate = (double)purchaseOdds[1];
                _minFreeSpinWinRate = (double)purchaseOdds[0];

                if (this.MoreBetMultiple > _totalFreeSpinWinRate)
                    _logger.Error("freespin win rate doesn't satisfy condition {0}", this.GameName);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occured in loading spin data of game {0} {1}", GameName, ex);
            }
        }
        protected override void setupDefaultResultParams(Dictionary<string, string> dicParams, double userBalance, int index, int counter, string initString)
        {
            base.setupDefaultResultParams(dicParams, userBalance, index, counter, initString);
	    	dicParams["reel_set"] = "0";
	        dicParams["st"] = "rect";
	        dicParams["sw"] = "5";
	        dicParams["bl"] = "0";
        }
        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
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
			
                if (betInfo.BetPerLine <= 0.0f)
                {
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in SuperXGameLogic::readBetInfoFromMessage {1}", strUserID, betInfo.BetPerLine);
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
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine   = betInfo.BetPerLine;
                    oldBetInfo.LineCount    = betInfo.LineCount;
                    oldBetInfo.MoreBet      = betInfo.MoreBet;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SuperXGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override async Task<BasePPSlotSpinData> selectEmptySpin(int companyID, BasePPSlotBetInfo betInfo)
        {
            if (!betInfo.MoreBet)
                return await base.selectEmptySpin(companyID, betInfo);

            return await selectMinStartFreeSpinData(betInfo);
        }
        protected override async Task<BasePPSlotSpinData> selectMinStartFreeSpinData(BasePPSlotBetInfo betInfo)
        {
            try
            {
                BsonDocument spinDataDocument = null;
                spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, -1, MoreBetMultiple * 0.2, MoreBetMultiple * 0.5, 0), TimeSpan.FromSeconds(10.0));
                return convertBsonToSpinData(spinDataDocument);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SuperXGameLogic::selectMinStartFreeSpinData {0}", ex);
                return null;
            }
        }        
        public override async Task<BasePPSlotSpinData> selectRandomStop(int companyID, UserBonus userBonus, double baseBet, bool isChangedLineCount, BasePPSlotBetInfo betInfo)
        {
            if (!betInfo.MoreBet)
                return await base.selectRandomStop(companyID, userBonus, baseBet, isChangedLineCount, betInfo);

            if (userBonus != null && userBonus is UserRangeOddEventBonus)
            {
                UserRangeOddEventBonus rangeOddBonus = userBonus as UserRangeOddEventBonus;
                var spinDataDocument = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                        new SelectSpinTypeOddRangeRequest(GameName, -1, rangeOddBonus.MinOdd, rangeOddBonus.MaxOdd, 0), TimeSpan.FromSeconds(10.0));

                if (spinDataDocument != null)
                {
                    BasePPSlotSpinData spinDataEvent = convertBsonToSpinData(spinDataDocument);
                    spinDataEvent.IsEvent = true;
                    return spinDataEvent;
                }
            }

            var spinDataDocument2 = await Context.System.ActorSelection("/user/spinDBReaders").Ask<BsonDocument>(
                    new SelectPurchaseSpinRequest(GameName, StartSpinSearchTypes.SPECIFIC),TimeSpan.FromSeconds(10.0));
            return convertBsonToSpinData(spinDataDocument2);
        }

        protected override void supplementInitResult(Dictionary<string, string> dicParams, BasePPSlotBetInfo betInfo, BasePPSlotSpinResult spinResult)
        {
            base.supplementInitResult(dicParams, betInfo, spinResult);
            if (!dicParams.ContainsKey("reel_set"))
                dicParams["reel_set"] = "0";
        }
        protected override Dictionary<string, string> mergeSpinToBonus(Dictionary<string, string> spinParams, Dictionary<string, string> bonusParams)
        {
            Dictionary<string, string> resultParams = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in bonusParams)
                resultParams.Add(pair.Key, pair.Value);

            string[] toCopyParams = new string[] { "s", "sa", "sb", "fs", "fsmul", "fsmax", "fsres", "fswin", "fs_total", "fsmul_total", "fsres_total", "fswin_total", "reel_set", "tw", "sw", "st", "wmt", "wmv", "rs_t", "rs_win", "gwm", "bw" };
            for (int i = 0; i < toCopyParams.Length; i++)
            {
                if (!spinParams.ContainsKey(toCopyParams[i]))
                    continue;
                resultParams[toCopyParams[i]] = spinParams[toCopyParams[i]];
            }
            return resultParams;
        }
    }
}
