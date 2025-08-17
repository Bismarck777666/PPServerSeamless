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
    public class EyeOfRaBetInfo : BaseAmaticSlotBetInfo
    {
        public override int RelativeTotalBet    => 1;
    }

    class EyeOfRaGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "EyeOfRa";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 250, 300, 350, 400, 450, 500, 600, 700, 800, 900, 1000, 1500, 2000, 3000, 4000, 5000, 10000, 20000 };
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
                return "05223990772aaa386145999b88277384aa5888862319933ab7755a677096aa33521994578cccc6aa3599aaaa976623e2a4955588770937448a9ba96674aa188822966779accccc6609aa6885779992359384aa560772883775b94993a188475a92aa57849cccc992aa66623299028577366844aa5b81a662077139905846aa3746658291115223990772aaa386145999b88277384aa5888862319933ab7755a677096aa33521994578cccc6aa3599aaaa9766214cccccccccccccccccccc2359384aa560772883775b94993a188475a92aa57849cccc992aa66623299028577366844aa5b81a662077139905846aa3746658291110301010101010104271010001a33e80b101010101010100b0b0b110010101010100000000000000000131a21421e2282322642962c82fa312c315e319031c231f4325832bc3320338433e80c101010101010101010101010";
            }
        }
        #endregion

        public EyeOfRaGameLogic()
        {
            _gameID     = GAMEID.EyeOfRa;
            GameName    = "EyeOfRa";
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strUserID, Currencies currency)
        {
            try
            {
                EyeOfRaBetInfo betInfo   = new EyeOfRaBetInfo();
                betInfo.PlayLine            = (int)message.Pop();
                betInfo.PlayBet             = (int)message.Pop();
                betInfo.PurchaseStep        = (int)message.Pop();
                betInfo.MoreBet             = (int)message.Pop();
                betInfo.CurrencyInfo        = currency;
                betInfo.GambleType          = 0;
                betInfo.GambleHalf          = false;

                if (BettingButton[betInfo.PlayBet] * betInfo.RelativeTotalBet <= 0)
                {
                    _logger.Error("{0} betInfo 0 or infinite in EyeOfRaGameLogic::readBetInfoFromMessage", strUserID);
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
                _logger.Error("Exception has been occurred in EyeOfRaGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override BaseAmaticSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            EyeOfRaBetInfo betInfo = new EyeOfRaBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
    }
}
