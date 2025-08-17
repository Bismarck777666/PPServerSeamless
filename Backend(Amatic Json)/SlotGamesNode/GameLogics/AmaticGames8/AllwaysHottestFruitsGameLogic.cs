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
    public class AllwaysHottestFruitsBetInfo : BaseAmaticSlotBetInfo
    {
        public override int RelativeTotalBet    => 1;
    }

    class AllwaysHottestFruitsGameLogic : BaseAmaticSpecAnteGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "AllwaysHottestFruits";
            }
        }
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 250, 300, 350, 400, 450, 500, 600, 700, 800, 900, 1000, 1500, 2000, 3000, 4000, 5000, 10000, 20000 };
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
                return "0522b36587938145a6784367a3423746047567158675034522a12304526781923405348a483412312042013461207228528692157082567823a47867a856758a25675678229045867849712358372348673567812563763456782281238465981463145872832458971234678102873522c36587136145a6784367a84237460475671586750346522a12304586781623408348a48341231204201348612022c57865215708256782364786785a8567568275b756786229045865719712356172345678567812563783456782271235467815231456728324569562346781021340301010101010104271010001131f409101010101010100909091100101010101000000000000000001011151a21421e2282322642962c82fa312c315e319031c231f40a10101010101010101010522b36587938145a6784367a3423746047567158675034522a12304526781923405348a483412312042013461207228528692157082567823a47867a856758a25675678229045867849712358372348673567812563763456782281238465981463145872832458971234678102873102780f10";
            }
        }
        protected override string ExtraAntePurString => "522b36587938145a6784367a3423746047567158675034522a12304526781923405348a483412312042013461207228528692157082567823a47867a856758a2567567822904586784971235837234867356781256376345678228123846598146314587283245897123467810287310278";
        protected override bool SupportMoreBet => true;
        protected override double MoreBetMultiple => 1.2;
        #endregion

        public AllwaysHottestFruitsGameLogic()
        {
            _gameID     = GAMEID.AllwaysHottestFruits;
            GameName    = "AllwaysHottestFruits";
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strUserID, Currencies currency)
        {
            try
            {
                AllwaysHottestFruitsBetInfo betInfo   = new AllwaysHottestFruitsBetInfo();
                betInfo.PlayLine            = (int)message.Pop();
                betInfo.PlayBet             = (int)message.Pop();
                betInfo.PurchaseStep        = (int)message.Pop();
                betInfo.MoreBet             = (int)message.Pop();
                betInfo.CurrencyInfo        = currency;
                betInfo.GambleType          = 0;
                betInfo.GambleHalf          = false;

                if (BettingButton[betInfo.PlayBet] * betInfo.RelativeTotalBet <= 0)
                {
                    _logger.Error("{0} betInfo 0 or infinite in AllwaysHottestFruitsGameLogic::readBetInfoFromMessage", strUserID);
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
                _logger.Error("Exception has been occurred in AllwaysHottestFruitsGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }
        protected override BaseAmaticSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            AllwaysHottestFruitsBetInfo betInfo = new AllwaysHottestFruitsBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
    }
}
