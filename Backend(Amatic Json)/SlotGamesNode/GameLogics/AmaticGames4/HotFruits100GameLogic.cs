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
    public class HotFruits100BetInfo : BaseAmaticSlotBetInfo
    {
        public override int RelativeTotalBet => PlayLine / 2;
    }

    class HotFruits100GameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "HotFruits100";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000 };
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
                return "0524d1111666333555000055551114442222444400444711166665553333000333666722255544422224d3333555333444000044446662220022226666711155551111555000555333744411166622244424d0000444333555333344442220066611116666711122225555000555744433366611155522244424c444000055511114443333666622200666333444411155552222000555744433366611155522224d3333444333555000044442220066611117666611175555222255500044433366611155522244400301010101010104271010001a33e864101010101010106464641100101010101000000000000000000a1112131415161718191a651010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010";
            }
        }
        #endregion

        public HotFruits100GameLogic()
        {
            _gameID     = GAMEID.HotFruits100;
            GameName    = "HotFruits100";
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strUserID, Currencies currency)
        {
            try
            {
                HotFruits100BetInfo betInfo   = new HotFruits100BetInfo();
                betInfo.PlayLine            = (int)message.Pop();
                betInfo.PlayBet             = (int)message.Pop();
                betInfo.PurchaseStep        = (int)message.Pop();
                betInfo.MoreBet             = (int)message.Pop();
                betInfo.CurrencyInfo        = currency;
                betInfo.GambleType          = 0;
                betInfo.GambleHalf          = false;

                if (BettingButton[betInfo.PlayBet] * betInfo.RelativeTotalBet <= 0)
                {
                    _logger.Error("{0} betInfo 0 or infinite in HotFruits100GameLogic::readBetInfoFromMessage", strUserID);
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
                _logger.Error("Exception has been occurred in HotFruits100GameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override BaseAmaticSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            HotFruits100BetInfo betInfo = new HotFruits100BetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
    }
}
