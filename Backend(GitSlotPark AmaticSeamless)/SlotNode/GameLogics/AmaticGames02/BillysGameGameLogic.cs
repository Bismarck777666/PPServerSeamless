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
    public class BillysGameBetInfo : BaseAmaticSlotBetInfo
    {
        public override int RelativeTotalBet    => 1;
    }

    class BillysGameGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "BillysGame";
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
                return "0522433337777711188888555506664444222266623144444555552226666777778883331111888866220335588772235555222444066667888333011188877788821888888888888888888888888821888888888888888888888888800301010101010104ade8100121437d009101010101010100909511100101010101000000000000000002121421e22823223c24625025a2642962c8312c319031f433e837d03bb83fa0413884177041b5841f40423284271044e204753049c404c3504ea60511170513880515f905186a009101010101010101010";
            }
        }

        protected override int FreeCols => 0;
        #endregion

        public BillysGameGameLogic()
        {
            _gameID     = GAMEID.BillysGame;
            GameName    = "BillysGame";
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strUserID, Currencies currency)
        {
            try
            {
                BillysGameBetInfo betInfo   = new BillysGameBetInfo();
                betInfo.PlayLine            = (int)message.Pop();
                betInfo.PlayBet             = (int)message.Pop();
                betInfo.PurchaseStep        = (int)message.Pop();
                betInfo.MoreBet             = (int)message.Pop();
                betInfo.CurrencyInfo        = currency;
                betInfo.GambleType          = 0;
                betInfo.GambleHalf          = false;

                if (BettingButton[betInfo.PlayBet] * betInfo.RelativeTotalBet <= 0)
                {
                    _logger.Error("{0} betInfo 0 or infinite in BillysGameGameLogic::readBetInfoFromMessage", strUserID);
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
                _logger.Error("Exception has been occurred in BillysGameGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override BaseAmaticSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            BillysGameBetInfo betInfo = new BillysGameBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
    }
}
