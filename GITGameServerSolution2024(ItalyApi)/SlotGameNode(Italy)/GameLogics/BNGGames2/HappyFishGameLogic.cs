using GITProtocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    public class HappyFishBetInfo : BaseBNGSlotBetInfo
    {
        public override float TotalBet
        {
            get
            {
                return BetPerLine * 20.0f / 100.0f;
            }
        }

    }
    class HappyFishGameLogic : BaseBNGSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "happy_fish";
            }
        }
        protected override int ClientReqLineCount
        {
            get
            {
                return 117649;
            }
        }
        //Modified by Foresight(2022.08.05)
        protected override string InitResultString
        {
            get
            {
                return "{\"actions\":[\"spin\",\"buy_spin\"],\"current\":\"spins\",\"last_action\":\"init\",\"last_args\":{},\"math_version\":\"a\",\"round_finished\":true,\"spins\":{\"bet_per_line\":4000,\"board\":[[2,10,1,10],[7,1,14,3,3],[14,4],[10,4,10,10,11],[2,2,12,10,10,7],[10,1,1,1,10,10,4]],\"lines\":117649,\"mult_values\":[[0,0,0,0,0,0,0],[0,0,0,0,0,0,0],[0,0,0,0,0,0,0],[0,0,0,0,0,0,0],[0,0,0,0,0,0,0],[0,0,0,0,0,0,0]],\"paid\":false,\"reels_len\":[4,5,2,5,6,7],\"round_bet\":80000,\"round_win\":0,\"total_win\":0,\"win\":0},\"version\":2}";
            }
        }
        //Modified by Foresight(2022.08.05)
        protected override string SettingString
        {
            get
            {
                return "{\"bet_factor\":[20],\"bets\":[1000,2000,3000,4000,5000,10000,15000,25000,35000,50000],\"big_win\":[30,60,100],\"cols\":6,\"currency_format\":{\"currency_style\":\"symbol\",\"denominator\":100,\"style\":\"money\"},\"freespins_buying_price\":100,\"fs_retrigger\":0,\"lines\":[117649],\"paytable\":{\"1\":[{\"multiplier\":1,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":2,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":4,\"occurrences\":5,\"type\":\"lb\"},{\"multiplier\":10,\"occurrences\":6,\"type\":\"lb\"}],\"2\":[{\"multiplier\":1,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":2,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":4,\"occurrences\":5,\"type\":\"lb\"},{\"multiplier\":10,\"occurrences\":6,\"type\":\"lb\"}],\"3\":[{\"multiplier\":1,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":2,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":6,\"occurrences\":5,\"type\":\"lb\"},{\"multiplier\":15,\"occurrences\":6,\"type\":\"lb\"}],\"4\":[{\"multiplier\":1,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":2,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":6,\"occurrences\":5,\"type\":\"lb\"},{\"multiplier\":15,\"occurrences\":6,\"type\":\"lb\"}],\"5\":[{\"multiplier\":2,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":4,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":8,\"occurrences\":5,\"type\":\"lb\"},{\"multiplier\":20,\"occurrences\":6,\"type\":\"lb\"}],\"6\":[{\"multiplier\":2,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":4,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":8,\"occurrences\":5,\"type\":\"lb\"},{\"multiplier\":20,\"occurrences\":6,\"type\":\"lb\"}],\"7\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":10,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":20,\"occurrences\":5,\"type\":\"lb\"},{\"multiplier\":40,\"occurrences\":6,\"type\":\"lb\"}],\"8\":[{\"multiplier\":5,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":15,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":30,\"occurrences\":5,\"type\":\"lb\"},{\"multiplier\":60,\"occurrences\":6,\"type\":\"lb\"}],\"9\":[{\"multiplier\":10,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":20,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":40,\"occurrences\":5,\"type\":\"lb\"},{\"multiplier\":80,\"occurrences\":6,\"type\":\"lb\"}],\"10\":[{\"multiplier\":15,\"occurrences\":3,\"type\":\"lb\"},{\"multiplier\":30,\"occurrences\":4,\"type\":\"lb\"},{\"multiplier\":60,\"occurrences\":5,\"type\":\"lb\"},{\"multiplier\":100,\"occurrences\":6,\"type\":\"lb\"}],\"14\":[{\"freespins\":8,\"multiplier\":0,\"occurrences\":3,\"trigger\":\"freespins\",\"type\":\"tb\"},{\"freespins\":10,\"multiplier\":0,\"occurrences\":4,\"trigger\":\"freespins\",\"type\":\"tb\"},{\"freespins\":12,\"multiplier\":0,\"occurrences\":5,\"trigger\":\"freespins\",\"type\":\"tb\"},{\"freespins\":14,\"multiplier\":0,\"occurrences\":6,\"trigger\":\"freespins\",\"type\":\"tb\"}]},\"reelsamples\":{\"buy\":[[1,2,3,4,5,6,7,8,9,10,14],[1,2,3,4,5,6,7,8,9,10,11,14,11,12,13],[1,2,3,4,5,6,7,8,9,10,11,14,11,12,13],[1,2,3,4,5,6,7,8,9,10,11,14,11,12,13],[1,2,3,4,5,6,7,8,9,10,11,14,11,12,13],[1,2,3,4,5,6,7,8,9,10,14]],\"freespins\":[[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10,11,12,13],[1,2,3,4,5,6,7,8,9,10,11,12,13],[1,2,3,4,5,6,7,8,9,10,11,12,13],[1,2,3,4,5,6,7,8,9,10,11,12,13],[1,2,3,4,5,6,7,8,9,10]],\"freespins_paid\":[[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10],[1,2,3,4,5,6,7,8,9,10]],\"spins\":[[1,2,3,4,5,6,7,8,9,10,14],[1,2,3,4,5,6,7,8,9,10,11,14,11,12,13],[1,2,3,4,5,6,7,8,9,10,11,14,11,12,13],[1,2,3,4,5,6,7,8,9,10,11,14,11,12,13],[1,2,3,4,5,6,7,8,9,10,11,14,11,12,13],[1,2,3,4,5,6,7,8,9,10,14]]},\"rows\":7,\"symbols\":[{\"id\":1,\"name\":\"el_01\",\"type\":\"line\"},{\"id\":2,\"name\":\"el_02\",\"type\":\"line\"},{\"id\":3,\"name\":\"el_03\",\"type\":\"line\"},{\"id\":4,\"name\":\"el_04\",\"type\":\"line\"},{\"id\":5,\"name\":\"el_05\",\"type\":\"line\"},{\"id\":6,\"name\":\"el_06\",\"type\":\"line\"},{\"id\":7,\"name\":\"el_07\",\"type\":\"line\"},{\"id\":8,\"name\":\"el_08\",\"type\":\"line\"},{\"id\":9,\"name\":\"el_09\",\"type\":\"line\"},{\"id\":10,\"name\":\"el_10\",\"type\":\"line\"},{\"id\":11,\"name\":\"el_wild_1\",\"type\":\"wild\"},{\"id\":12,\"name\":\"el_wild_2\",\"type\":\"wild\"},{\"id\":13,\"name\":\"el_wild_3\",\"type\":\"wild\"},{\"id\":14,\"name\":\"el_scatter\",\"type\":\"scat\"}],\"symbols_line\":[1,2,3,4,5,6,7,8,9,10],\"symbols_scat\":[14],\"symbols_wild\":[11,12,13],\"version\":\"a\"}";
            }
        }
        protected override bool SupportPurchaseFree
        {
            get { return true; }
        }
        protected override double PurchaseFreeMultiple
        {
            get { return 100.0; }
        }
        #endregion


        public HappyFishGameLogic()
        {
            _gameID = GAMEID.HappyFish;
            GameName = "HappyFish";
        }
        protected override void readBetInfoFromMessage(GITMessage message, string strUserID, BNGActionTypes action)
        {
            try
            {
                float betPerLine = (float)(double)message.Pop();
                int lineCount = (int)message.Pop();

                BaseBNGSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strUserID, out oldBetInfo))
                {
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse || (action != BNGActionTypes.SPIN && action != BNGActionTypes.BUYSPIN))
                        return;

                    if (betPerLine <= 0.0f)
                    {
                        _logger.Error("{0} betInfo.BetPerLine <= 0 in BaseBNGSlotGame::readBetInfoFromMessage {1}", strUserID, betPerLine);
                        return;
                    }

                    if (lineCount != this.ClientReqLineCount && action == BNGActionTypes.SPIN)
                    {
                        _logger.Error("{0} betInfo.LineCount is not matched {1}", strUserID, lineCount);
                        return;
                    }

                    oldBetInfo.BetPerLine = betPerLine;
                    oldBetInfo.LineCount  = lineCount;

                }
                else if (action == BNGActionTypes.SPIN || action == BNGActionTypes.BUYSPIN)
                {
                    if (betPerLine <= 0.0f)
                    {
                        _logger.Error("{0} betInfo.BetPerLine <= 0 in BaseBNGSlotGame::readBetInfoFromMessage {1}", strUserID, betPerLine);
                        return;
                    }
                    if (lineCount != this.ClientReqLineCount && action == BNGActionTypes.SPIN)
                    {
                        _logger.Error("{0} betInfo.LineCount is not matched {1}", strUserID, lineCount);
                        return;
                    }
                    HappyFishBetInfo betInfo = new HappyFishBetInfo();
                    betInfo.BetPerLine       = betPerLine;
                    betInfo.LineCount        = lineCount;
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BaseBNGSlotGame::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override BaseBNGSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            HappyFishBetInfo betInfo = new HappyFishBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
    }
}
