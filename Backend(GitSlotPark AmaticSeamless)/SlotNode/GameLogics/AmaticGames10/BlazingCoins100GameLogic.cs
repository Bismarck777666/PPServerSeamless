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
using SlotGamesNode.Database;

namespace SlotGamesNode.GameLogics
{
    public class BlazingCoins100BetInfo : BaseAmaticSlotBetInfo
    {
        public override int RelativeTotalBet => PlayLine / 2;
    }

    class BlazingCoins100GameLogic : BaseAmaticSpecAnteGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "BlazingCoins100";
            }
        }
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000, 2000 };
            }
        }
        protected override long[] LINES
        {
            get
            {
                return new long[] { 100 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "052a001010101060606030303050505000000000505050501010104040402020202040404040000040404070101010606060610050505030303030000000303030606060809020202050505040404020202010101010606060e0f0303030505050000000005050505010101040404020202020404040400000404040a0b0c010101060606060d050505030303030000000303030606060202020505050404040202022a1030303030505050303030404040000000004040404100606060202020000020202020606060607010101050505050101010105050500000005050503030308090404040101010606060202020e0f0404040303030305050503030304040400000000040404040d060606020202000002020202060606060a0b0101010105050505010101010505050000000505050303030c0404040101010606060202020404042a10000000004040403030305050503030303040404040202020000060606010101010606060607010101020202020505050500000005050508090404040303030606060f010101050505020202040404000000000404040303030e0505050303030304040404020202000006060601010101060606060a0b010101011002020202050505050000000505050404040303030c0d0606060101010505050202020404042a1040404000000000505050101010104040403030303060606060202020000060606030303040404040c0101010505050502020202000000050505070404040303030606060101010d05050502020204040400000000050505010101010809040404030303030606060602020200000606060303030a0b040404040401010105050505020202020000000505050404040e0f030303060606100101010505050202022a0030303030404040303030505050000000004040404020202000006060601010101070606060601010108090505050502020202050505000000040404030303060606100101010505050202020404040303030304040403030305050500000000040404040202020000060606010101010a0b0c0606060601010105050505020202020505050000000404040303030d0e0f06060601010105050502020204040400301010101010104271010001a33e864101010101010106464641100101010101000000000000000000a1112131415161718191a651010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010102960f1a01030805040f020104011a01030805040201010a051a01030802010a010302021a04020f14010a050101011a01030504020f01010204";
            }
        }
        protected override int ReelSetBitNum => 2;
        protected override string ExtraAntePurString => "10296";
        protected override string ExtraString => "0f1a01030805040f020104011a01030805040201010a051a01030802010a010302021a04020f14010a050101011a01030504020f01010204";
        protected override bool SupportMoreBet => true;
        protected override double MoreBetMultiple => 1.5;
        #endregion

        public BlazingCoins100GameLogic()
        {
            _gameID     = GAMEID.BlazingCoins100;
            GameName    = "BlazingCoins100";
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strUserID, Currencies currency)
        {
            try
            {
                BlazingCoins100BetInfo betInfo   = new BlazingCoins100BetInfo();
                betInfo.PlayLine            = (int)message.Pop();
                betInfo.PlayBet             = (int)message.Pop();
                betInfo.PurchaseStep        = (int)message.Pop();
                betInfo.MoreBet             = (int)message.Pop();
                betInfo.CurrencyInfo        = currency;
                betInfo.GambleType          = 0;
                betInfo.GambleHalf          = false;

                if (BettingButton[betInfo.PlayBet] * betInfo.RelativeTotalBet <= 0)
                {
                    _logger.Error("{0} betInfo 0 or infinite in BlazingCoins100GameLogic::readBetInfoFromMessage", strUserID);
                    return;
                }

                BaseAmaticSlotBetInfo oldBetInfo = null;
                if (_dicUserBetInfos.TryGetValue(strUserID, out oldBetInfo))
                {
                    //만일 유저에게 남은 응답이 존재하는 경우
                    if (oldBetInfo.HasRemainResponse)
                        return;

                    oldBetInfo.PlayLine     = betInfo.PlayLine;
                    oldBetInfo.PlayBet      = betInfo.PlayBet;
                    oldBetInfo.PurchaseStep = betInfo.PurchaseStep;
                    oldBetInfo.MoreBet      = betInfo.MoreBet;
                    oldBetInfo.CurrencyInfo = betInfo.CurrencyInfo;
                    oldBetInfo.GambleType   = betInfo.GambleType;
                    oldBetInfo.GambleHalf   = betInfo.GambleHalf;
                }
                else
                {
                    _dicUserBetInfos.Add(strUserID, betInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception has been occurred in BlazingCoins100GameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override BaseAmaticSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            BlazingCoins100BetInfo betInfo = new BlazingCoins100BetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
    }
}
