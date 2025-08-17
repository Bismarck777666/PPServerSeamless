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
    public class BeautyWarriorBetInfo : BaseAmaticSlotBetInfo
    {
        public override int RelativeTotalBet => 1;
    }

    class BeautyWarriorGameLogic : BaseAmaticExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "BeautyWarrior";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 2000 };
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
                return "0523400000604070002070003070104050907020703070406050401060101050402010504040704020702040804080804080407060405243070905060307050906070006030503050207010405070a0b0c0d060305070306060606060305050305030306030506050908080808080308030308030506050505050524302050704050607050400060307040609070406010706020206070e0f10111206000502070707070707090505050505050202060209070404080307030803080702080823c03040705060007020307050904030701040705020504070a0b0c0d0602060106010106010401060606060103080808080408040406040308030503032370002050703060704060504090106030602000707030005040603070404070107030606010509020703070705050503080802080804070700301010101010104271010001a33e80910101010101010090909110010101010100000000000000000131a21421e2282322642962c82fa312c315e319031c231f4325832bc3320338433e80a101010101010101010100101010101";
            }
        }

        protected override string ExtraString => "0101010101";

        protected override int ReelSetBitNum => 2;
        #endregion

        public BeautyWarriorGameLogic()
        {
            _gameID     = GAMEID.BeautyWarrior;
            GameName    = "BeautyWarrior";
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strUserID, Currencies currency)
        {
            try
            {
                BeautyWarriorBetInfo betInfo   = new BeautyWarriorBetInfo();
                betInfo.PlayLine            = (int)message.Pop();
                betInfo.PlayBet             = (int)message.Pop();
                betInfo.PurchaseStep        = (int)message.Pop();
                betInfo.MoreBet             = (int)message.Pop();
                betInfo.CurrencyInfo        = currency;
                betInfo.GambleType          = 0;
                betInfo.GambleHalf          = false;

                if (BettingButton[betInfo.PlayBet] * betInfo.RelativeTotalBet <= 0)
                {
                    _logger.Error("{0} betInfo 0 or infinite in BeautyWarriorGameLogic::readBetInfoFromMessage", strUserID);
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
                _logger.Error("Exception has been occurred in BeautyWarriorGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override BaseAmaticSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            BeautyWarriorBetInfo betInfo = new BeautyWarriorBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
    }
}
