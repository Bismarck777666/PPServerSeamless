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
    public class HotFruitsDeluxeBetInfo : BaseAmaticSlotBetInfo
    {
        public override int RelativeTotalBet => 1;
    }

    class HotFruitsDeluxeGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "HotFruitsDeluxe";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000, 2000, 4000, 6000, 8000, 10000, 15000, 20000 };
            }
        }

        protected override long[] LINES
        {
            get
            {
                return new long[] { 5 };
            }
        }

        protected override string InitString
        {
            get
            {
                return "0521b61237325104445556668577788821b81203542384445556665777888721c7123183544428256667770888555101000301010101010104271010001a33e805101010101010100505051100101010101000000000000000000e1a1f21421921e22322822d23223c24625025a26406101010101010";
            }
        }
        #endregion

        public HotFruitsDeluxeGameLogic()
        {
            _gameID     = GAMEID.HotFruitsDeluxe;
            GameName    = "HotFruitsDeluxe";
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strUserID, Currencies currency)
        {
            try
            {
                HotFruitsDeluxeBetInfo betInfo   = new HotFruitsDeluxeBetInfo();
                betInfo.PlayLine            = (int)message.Pop();
                betInfo.PlayBet             = (int)message.Pop();
                betInfo.PurchaseStep        = (int)message.Pop();
                betInfo.MoreBet             = (int)message.Pop();
                betInfo.CurrencyInfo        = currency;
                betInfo.GambleType          = 0;
                betInfo.GambleHalf          = false;

                if (BettingButton[betInfo.PlayBet] * betInfo.RelativeTotalBet <= 0)
                {
                    _logger.Error("{0} betInfo 0 or infinite in HotFruitsDeluxeGameLogic::readBetInfoFromMessage", strUserID);
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
                _logger.Error("Exception has been occurred in HotFruitsDeluxeGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override BaseAmaticSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            HotFruitsDeluxeBetInfo betInfo = new HotFruitsDeluxeBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
    }
}
