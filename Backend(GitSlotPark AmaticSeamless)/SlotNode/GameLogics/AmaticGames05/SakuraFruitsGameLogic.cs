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
    public class SakuraFruitsBetInfo : BaseAmaticSlotBetInfo
    {
        public override int RelativeTotalBet    => 1;
    }

    class SakuraFruitsGameLogic : BaseAmaticExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SakuraFruits";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 10, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 250, 300, 350, 400, 450, 500, 600, 700, 800, 900, 1000, 1500, 2000, 3000, 4000, 5000, 10000, 20000 };
            }
        }

        protected override long[] LINES
        {
            get
            {
                return new long[] { 9 };
            }
        }

        protected override string InitString
        {
            get
            {
                return "0523c6bcd6645553557778266361888277111822253338444518bcd8546662777242889a5555116660777766835288881777bcd5544711116222283333554444669a782468459a5555544633336666077777188888228bcd61111162222277333338444445bcd7124111779a555506666227777384488887228bcd71111152222663333664444889a5523066bcd555276663587774611888445111522227744488333300301010101010104271010001a33e8091010101010101009090b110010101010100000000000000000131a21421e2282322642962c82fa312c315e319031c231f4325832bc3320338433e80910101010101010101013fff14ffff15fffff14ffff13fff13111141111151111114111113111";
            }
        }

        protected override string ExtraString => "13fff14ffff15fffff14ffff13fff13111141111151111114111113111";
        #endregion

        public SakuraFruitsGameLogic()
        {
            _gameID     = GAMEID.SakuraFruits;
            GameName    = "SakuraFruits";
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strUserID, Currencies currency)
        {
            try
            {
                SakuraFruitsBetInfo betInfo   = new SakuraFruitsBetInfo();
                betInfo.PlayLine            = (int)message.Pop();
                betInfo.PlayBet             = (int)message.Pop();
                betInfo.PurchaseStep        = (int)message.Pop();
                betInfo.MoreBet             = (int)message.Pop();
                betInfo.CurrencyInfo        = currency;
                betInfo.GambleType          = 0;
                betInfo.GambleHalf          = false;

                if (BettingButton[betInfo.PlayBet] * betInfo.RelativeTotalBet <= 0)
                {
                    _logger.Error("{0} betInfo 0 or infinite in SakuraFruitsGameLogic::readBetInfoFromMessage", strUserID);
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
                _logger.Error("Exception has been occurred in SakuraFruitsGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override BaseAmaticSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            SakuraFruitsBetInfo betInfo = new SakuraFruitsBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
    }
}
