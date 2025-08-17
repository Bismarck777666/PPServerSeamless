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
    public class TikiMadness100BetInfo : BaseAmaticSlotBetInfo
    {
        public override int RelativeTotalBet => PlayLine / 2;
    }
    
    class TikiMadness100GameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "TikiMadness100";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 6, 7, 8 ,9 ,10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };
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
                return "05256010b0c0d060606031112130505050008090a05050505010b0c0d040404020e0f10040404040008090a04040407010b0c0d06060606050505031112130008090a0311121306060607020e0f10050505040404020e0f1025603111213050505031112130404040008090a04040404060606020e0f100008090a020e0f100606060607010b0c0d05050505010b0c0d0505050008090a0505050311121307040404010b0c0d060606020e0f100404042560008090a040404031112130505050311121304040404020e0f100008090a060606010b0c0d0606060607010b0c0d020e0f10050505050008090a0505050704040403111213060606010b0c0d050505020e0f100404042550404040008090a050505010b0c0d0404040311121306060606020e0f100008090a0606060311121304040404010b0c0d05050505020e0f100008090a0505050704040403111213060606010b0c0d050505020e0f1025603111213040404031112130505050008090a04040404020e0f100008090a060606010b0c0d0706060606010b0c0d0705050505020e0f100505050008090a04040403111213060606010b0c0d050505020e0f10040404003010101010101042710100023233e864101010101010106464641100101010101000000000000000000a1112131415161718191a651010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010";
            }
        }

        protected override int ReelSetBitNum => 2;
        #endregion

        public TikiMadness100GameLogic()
        {
            _gameID     = GAMEID.TikiMadness100;
            GameName    = "TikiMadness100";
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strUserID, Currencies currency)
        {
            try
            {
                TikiMadness100BetInfo betInfo   = new TikiMadness100BetInfo();
                betInfo.PlayLine            = (int)message.Pop();
                betInfo.PlayBet             = (int)message.Pop();
                betInfo.PurchaseStep        = (int)message.Pop();
                betInfo.MoreBet             = (int)message.Pop();
                betInfo.CurrencyInfo        = currency;
                betInfo.GambleType          = 0;
                betInfo.GambleHalf          = false;

                if (BettingButton[betInfo.PlayBet] * betInfo.RelativeTotalBet <= 0)
                {
                    _logger.Error("{0} betInfo 0 or infinite in TikiMadness100GameLogic::readBetInfoFromMessage", strUserID);
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
                _logger.Error("Exception has been occurred in TikiMadness100GameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override BaseAmaticSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            TikiMadness100BetInfo betInfo = new TikiMadness100BetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
    }
}
