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
    public class BellsOnFireRomboBetInfo : BaseAmaticSlotBetInfo
    {
        public override int RelativeTotalBet    => 1;
    }

    class BellsOnFireRomboGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "BellsOnFireRombo";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000, 1500, 2000, 5000, 10000 };
            }
        }

        protected override long[] LINES
        {
            get
            {
                return new long[] { 8 };
            }
        }

        protected override string InitString
        {
            get
            {
                return "052290047027036714582777346541611542144474272422d7856375670635732145799996357666655553333365652382574567540374867412266677999996605777777555552222284433322c3475607237758431475257499992666611111146666323002573674654816362077305463744136651827377399955552290470270368714578273465417614154214474272422d7856375670635732145799996357666655553333365652149999999999999999999922c347560723775843147525749999266661111114666632300257367465481636207730546374413665182737739995550001010101010104c33c100121437d001101010111110100808081100101010101000000000000000001c21421e22823223c24625025a2642962c8312c319031f433e835dc37d0413884271044e204753049c404c3504ea60511170513880515f905186a0081010101010101010";
            }
        }

        protected override int LineTypeCnt
        {
            get
            {
                return 1;
            }
        }
        #endregion

        public BellsOnFireRomboGameLogic()
        {
            _gameID     = GAMEID.BellsOnFireRombo;
            GameName    = "BellsOnFireRombo";
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strUserID, Currencies currency)
        {
            try
            {
                BellsOnFireRomboBetInfo betInfo   = new BellsOnFireRomboBetInfo();
                betInfo.PlayLine            = (int)message.Pop();
                betInfo.PlayBet             = (int)message.Pop();
                betInfo.PurchaseStep        = (int)message.Pop();
                betInfo.MoreBet             = (int)message.Pop();
                betInfo.CurrencyInfo        = currency;
                betInfo.GambleType          = 0;
                betInfo.GambleHalf          = false;

                if (BettingButton[betInfo.PlayBet] * betInfo.RelativeTotalBet <= 0)
                {
                    _logger.Error("{0} betInfo 0 or infinite in BellsOnFireRomboGameLogic::readBetInfoFromMessage", strUserID);
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
                _logger.Error("Exception has been occurred in BellsOnFireRomboGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override BaseAmaticSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            BellsOnFireRomboBetInfo betInfo = new BellsOnFireRomboBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
    }
}
