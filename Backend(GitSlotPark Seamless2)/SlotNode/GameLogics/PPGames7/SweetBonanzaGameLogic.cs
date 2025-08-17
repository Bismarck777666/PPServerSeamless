using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class SweetBonanzaGameLogic : BasePPSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "vs20fruitsw";
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
                return 20;
            }
        }
        protected override int ServerResLineCount
        {
            get { return 20; }
        }
        protected override int ROWS
        {
            get
            {
                return 5;
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "reel_set=0&def_s=3,8,4,8,1,10,6,10,5,7,8,9,6,9,8,7,4,5,3,4,3,8,4,8,1,10,6,10,5,7&prg_m=wm&cfgs=5169&ver=2&prg=1&reel_set_size=5&def_sb=5,10,11,8,1,7&prm=12~2,3,4,5,6,8,10,12,15,20,25,50,100;12~2,3,4,5,6,8,10,12,15,20,25,50,100;12~2,3,4,5,6,8,10,12,15,20,25,50,100&def_sa=8,3,4,3,11,3&prg_cfg_m=wm&scatters=1~2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,2000,100,60,0,0,0~10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,0,0,0~1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1&gmb=0,0,0&rt=d&prg_cfg=1&sc=10.00,20.00,30.00,40.00,50.00,100.00,150.00,200.00,250.00,375.00,500.00,750.00,1250.00,2500.00,3750.00,5000.00&defc=50.00&fspps=2000~10~0&wilds=2~0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0~1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1&bonuses=0&fsbonus=&bls=20,25&paytable=0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,1000,500,500,200,200,0,0,0,0,0,0,0;500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,500,200,200,50,50,0,0,0,0,0,0,0;300,300,300,300,300,300,300,300,300,300,300,300,300,300,300,300,300,300,300,100,100,40,40,0,0,0,0,0,0,0;240,240,240,240,240,240,240,240,240,240,240,240,240,240,240,240,240,240,240,40,40,30,30,0,0,0,0,0,0,0;200,200,200,200,200,200,200,200,200,200,200,200,200,200,200,200,200,200,200,30,30,20,20,0,0,0,0,0,0,0;160,160,160,160,160,160,160,160,160,160,160,160,160,160,160,160,160,160,160,24,24,16,16,0,0,0,0,0,0,0;100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,100,20,20,10,10,0,0,0,0,0,0,0;80,80,80,80,80,80,80,80,80,80,80,80,80,80,80,80,80,80,80,18,18,8,8,0,0,0,0,0,0,0;40,40,40,40,40,40,40,40,40,40,40,40,40,40,40,40,40,40,40,15,15,5,5,0,0,0,0,0,0,0;0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0&fspps_mask=bet~fs_count~bet_level&total_bet_max=10,000,000.00&reel_set0=4,11,11,1,7,10,10,10,11,10,8,7,9,11,7,7,7,5,6,10,9,3,8,6,10~11,10,11,5,11,9,11,11,10,11,9,6,4,10,8,8,9,9,11,6,9,4,11,10,7,9,11,8,6,3,9,4,11,11,11,8,9,9,10,11,8,11,5,4,7,9,10,8,5,6,1,8,9,11,7,9,7,11,10,6,6,11,3,10,1,5,10,8,7~6,8,8,3,4,10,8,9,11,4,9,5,7,7,7,1,10,11,9,5,10,7,7,6,11,6,10,8,9,3,3,3,3,8,11,10,8,4,7,7,10,3,5,6,4,11,11~5,6,10,5,11,3,10,7,11,9,4,6,5,9,8,10,10,10,10,4,11,11,9,11,9,6,1,8,9,5,11,6,11,8,11,11,11,10,9,7,5,10,10,4,6,10,11,6,11,11,10,7,3,11,3~11,3,6,11,5,7,7,7,9,10,4,8,9,11,9,9,9,9,8,7,10,6,1,5~6,3,9,7,4,7,9,11,9,4,4,4,10,5,11,4,11,1,10,8,8,9,6&reel_set2=4,3,5,7,9,11,8,10,10,10,7,8,1,5,10,4,9,10,7,7,7,8,9,10,6,11,6,11,10,7,11~6,8,9,10,9,6,11,10,4,10,7,11,6,11,11,11,9,8,11,10,10,9,9,3,4,11,11,9,5,11,1,9,9,9,10,8,7,7,6,3,1,11,8,8,9,5,9,11,4,5~5,6,7,8,10,6,6,10,3,10,8,1,11,11,10,8,9,7,7,7,8,10,9,3,8,11,4,7,11,4,9,5,6,7,9,7,11,10,3,3,3,11,4,7,1,10,5,6,8,4,8,3,7,10,5,9,11,11,3,4,8~8,4,1,10,10,10,10,10,11,9,7,5,11,11,11,6,11,11,3,6,9~4,3,7,9,11,7,7,7,5,10,8,11,9,5,11,9,9,9,9,6,9,10,7,1,6,8~10,7,11,9,5,10,10,4,11,3,9,6,1,7,8,4,11,5,9,4,4,4,4,8,4,9,10,7,6,11,5,8,3,6,6,8,9,4,9,9,8,9,7,11,10&t=symbol_count&reel_set1=11,4,6,12,10,8,11,9,5,12,10,4,8,8,11,8,5,9,7,7,8,10,6,5,10,11,10,10,10,11,10,6,1,10,3,9,9,7,6,10,10,4,8,11,4,11,10,7,11,3,8,11,9,5,6,9,10,11~11,10,7,9,10,11,11,5,9,8,8,5,1,4,9,10,8,6,10,11,12,3,9,6,6,4~10,11,6,10,9,4,11,8,8,1,5,4,9,8,11,8,12,7,10,3,6,3,7,5~5,11,6,11,12,1,5,10,9,11,11,5,11,7,3,7,10,4,7,10,3,6,11,10,6,6,8,3,8,9,9,10,5,4,11,12,11,4,10,9,8,9~6,11,5,1,4,11,10,9,6,7,7,7,7,3,12,11,8,9,7,9,8,10,8,10~9,10,7,10,6,10,9,12,6,11,1,9,3,9,7,10,10,9,6,11,8,10,7,8,8,5,11,6,12,9,10,4,9,8,8,10,3,9,8,4,11,5,6,11,9,11,9,8,4,5,11,7,9,5,6,11,11,4,8,9&reel_set4=4,11,4,10,11,10,9,7,10,10,6,8,8,9,8,10,10,6,10,10,10,4,7,8,3,11,5,6,3,9,10,6,11,10,8,7,11,11,7,7,7,7,7,10,11,10,7,9,10,11,5,11,5,5,9,4,11,8,6,8,10,9,9~6,3,8,9,11,11,4,4,4,4,10,9,10,9,7,10,5,6,8,8,8,7,11,7,4,6,11,4,8,11,11,11,8,5,10,9,11,11,8,10,11,9~9,8,8,11,7,9,10,5,4,10,9,11,11,10,7,10,10,10,4,3,8,6,11,4,5,8,7,3,11,7,6,8,4,3,7,7,7,11,7,6,3,4,3,9,7,10,6,6,10,8,8,11,9,3,3,3,6,8,5,11,7,11,8,9,10,10,5,4,10,10,11,8,10~6,7,6,8,7,10,5,11,11,7,11,4,9,6,9,9,9,10,3,8,9,9,3,5,3,9,11,9,8,11,10,3,10,10,10,11,6,10,8,9,4,9,4,11,11,5,10,11,11,5,11,11,11,10,10,6,11,6,6,11,11,10,5,10,9,4,10,5,11,7~8,10,11,9,11,9,10,11,11,11,8,9,4,8,5,7,9,7,11,7,7,7,4,10,7,11,6,6,9,10,9,9,9,11,5,8,5,3,6,9,11,9,9~6,5,9,10,7,8,11,4,4,4,8,7,5,11,4,10,8,9,6,6,6,4,11,9,6,6,3,4,9,10&reel_set3=10,5,11,11,12,8,10,8,10,10,10,7,4,9,3,11,6,1,9,10,6~10,3,8,11,9,10,9,4,9,10,6,9,8,10,12,1,5,9,9,4,11,8,7,9,6,11,7,6,4,11,11,9,11,12,10,10,3,8,6,6,7,11,5,10,5,12,8,11,9,8,10,10,8,4,5,11~10,6,8,6,11,10,5,12,5,4,4,6,8,9,7,8,3,3,10,1,4,7,11,9,8,11,9,7~10,4,10,6,10,12,10,11,6,12,9,5,10,4,4,8,11,1,3,6,5,5,11,10,8,10,3,6,7,11,5,4,9,10,6,9,11,3,7,11,7,11,8,11,7,9,11,9,6,5,5,9,8,3,10,9,11,9,11,11,10~8,11,9,4,5,9,10,11,9,11,9,8,10,4,4,10,9,9,3,8,6,8,10,11,7,7,4,1,9,7,7,7,8,8,11,6,6,9,7,7,6,9,9,10,8,5,12,3,10,6,12,11,11,9,5,11,10,11,5,11,6,10,8,7~4,10,6,7,8,9,9,4,10,1,10,6,9,11,5,11,11,9,3,8,11,7,8,9,6,10,11,9,5,8,12&total_bet_min=10.00";
            }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 100.0; }
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
            get { return 1.25; }
        }
        protected override bool SupportMoreBet
        {
            get { return true; }
        }
        #endregion
        public SweetBonanzaGameLogic()
        {
            _gameID = GAMEID.SweetBonanza;
            GameName = "SweetBonanza";
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strGlobalUserID)
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
                    _logger.Error("{0} betInfo.BetPerLine <= 0 in SugarRushGameLogic::readBetInfoFromMessage {1}", strGlobalUserID, betInfo.BetPerLine);
                    return;
                }

                if (betInfo.MoreBet && betInfo.PurchaseFree)
                {
                    _logger.Error("{0} betInfo.MoreBet and  PurchasedFreeSpin is same time true in SugarRushGameLogic::readBetInfoFromMessage", strGlobalUserID);
                    return;
                }
                if (betInfo.LineCount != this.ClientReqLineCount)
                {
                    _logger.Error("{0} betInfo.LineCount is not matched {1}", strGlobalUserID, betInfo.LineCount);
                    return;
                }

                BasePPSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strGlobalUserID, out oldBetInfo))
                {
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.BetPerLine   = betInfo.BetPerLine;
                    oldBetInfo.LineCount    = betInfo.LineCount;
                    oldBetInfo.PurchaseFree = betInfo.PurchaseFree;
                    oldBetInfo.MoreBet      = betInfo.MoreBet;
                }
                else
                {
                    _dicUserBetInfos.Add(strGlobalUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in SugarRushGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override void convertWinsByBet(Dictionary<string, string> dicParams, float currentBet)
        {
            base.convertWinsByBet(dicParams, currentBet);
            if (dicParams.ContainsKey("tmb_win"))
                dicParams["tmb_win"] = convertWinByBet(dicParams["tmb_win"], currentBet);

            if (dicParams.ContainsKey("tmb_res"))
                dicParams["tmb_res"] = convertWinByBet(dicParams["tmb_res"], currentBet);
        }
    }
}
