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

namespace SlotGamesNode.GameLogics
{
    public class DragonsGiftBetInfo : BaseAmaticSlotBetInfo
    {
        public override int RelativeTotalBet    => 1;
    }

    class DragonsGiftGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "DragonsGift";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 250, 300, 350, 400, 450, 500, 600, 700, 800, 900, 1000, 1500, 2000, 3000, 4000, 5000, 10000, 20000 };
            }
        }

        protected override long[] LINES
        {
            get
            {
                return new long[] { 11 };
            }
        }

        protected override string InitString
        {
            get
            {
                return "052230909000707020a0a0a0308060104050909090b08080207070308040a0a050808080806231090903030a0b070705050a0607070009060a0a03030502010909040507080c0d0e0f060a0a030509090a0a0a0a0907060623e020a040905050508080707000903070404080a090b0a09060607040a0a0108080802020906060707090a1011121314060600090a0a060808050707090909235090308040a0a0506000707020808030707050b09040909030a0108080407050a09020a0a05070804090c0d0e0f0909020a0a06060623209090002080507070306060804040a0a050b08010a0606020007070103090900050804060a0a03070406060508020901010152230909000707020a0a0a0308060104050909090b08080207070308040a0a050808080806231090903030a0b070705050a0607070009060a0a03030502010909040507080c0d0e0f060a0a030509090a0a0a0a09070606151011121314235090308040a0a0506000707020808030707050b09040909030a0108080407050a09020a0a05070804090c0d0e0f0909020a0a06060623209090002080507070306060804040a0a050b08010a0606020007070103090900050804060a0a0307040606050802090101010301010101010104271010001a33e80b101010101010100b0b0b110010101010100000000000000000131a21421e2282322642962c82fa312c315e319031c231f4325832bc3320338433e80c101010101010101010101010";
            }
        }

        protected override int ReelSetBitNum => 2;
        #endregion

        public DragonsGiftGameLogic()
        {
            _gameID     = GAMEID.DragonsGift;
            GameName    = "DragonsGift";
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strUserID, Currencies currency)
        {
            try
            {
                DragonsGiftBetInfo betInfo   = new DragonsGiftBetInfo();
                betInfo.PlayLine            = (int)message.Pop();
                betInfo.PlayBet             = (int)message.Pop();
                betInfo.PurchaseStep        = (int)message.Pop();
                betInfo.MoreBet             = (int)message.Pop();
                betInfo.CurrencyInfo        = currency;
                betInfo.GambleType          = 0;
                betInfo.GambleHalf          = false;

                if (BettingButton[betInfo.PlayBet] * betInfo.RelativeTotalBet <= 0)
                {
                    _logger.Error("{0} betInfo 0 or infinite in DragonsGiftGameLogic::readBetInfoFromMessage", strUserID);
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
                _logger.Error("Exception has been occurred in DragonsGiftGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override BaseAmaticSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            DragonsGiftBetInfo betInfo = new DragonsGiftBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
    }
}
