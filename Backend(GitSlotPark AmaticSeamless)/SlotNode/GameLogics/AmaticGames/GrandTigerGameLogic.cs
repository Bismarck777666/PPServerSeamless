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
    public class GrandTigerBetInfo : BaseAmaticSlotBetInfo
    {
        public override int RelativeTotalBet => 1;
    }

    class GrandTigerGameLogic : BaseAmaticExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "GrandTiger";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000 };
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
                return "05247000006040700020700030701040509070207030704060504010601010504020105040407040207020408040808040804070604050404070402070204080408080408040706040525b070905060307050906070006030503050207010405070a0b0c0d060305070306060606060305050305030306030506050908080808080308030308030506060606060305050305030306030506050808080808030803030803050626302050704050607050400060307040609070406010706020206070e0f101112060005020707070707070905050505050502020602090704040803070308030807020808020507070707070505050505050202060205070404080307030803080702080825d03040705060007020307050904030701040705020504070a0b0c0d06020601060101060104010606060601030808080804080404060403080305030306020601060101060104010606060601030808080804080404060403080305030325a000205070306070406050409010603060200070703000504060307040407010703060601050902070307070505050308080208080407070300050406030704040701070306060105060207030707050505030808020808040707524e00040700020700030607010405090702070703040605040106010105040201040407040207020408040808080706070207070304060504010601010504020104040704020702040804080808070624107090506030705090607000607020104070a0b0c0d060307030606060505030506030509080808080308050603070306060605050305060305060808080803080524d020507040506070500030704090607040106020606070e0f1011120606050207070905050505020602090704080303080308080806060502070704050505050206020507040803030803080808245030407050600070203070509040307010407050205070a0b0c0d060206060401060606060103080808080406080304070506020606040106060606010308080808040608032540002050703060704060504090106030602070703050406030704040103060601050902070307070505050308080208080407070305040603070404010306060504020703070705050503080802080804070707070001010101010104b9be100121437d001101010111110100a0a0a1100101010101000000000000000002121421e22823223c24625025a2642962c8312c319031f433e837d03bb83fa0413884177041b5841f40423284271044e204753049c404c3504ea60511170513880515f905186a00a101010101010101010100101010101";
            }
        }

        protected override int LineTypeCnt  => 1;

        protected override int ReelSetBitNum => 2;

        protected override string ExtraString => "0101010101";
        #endregion

        public GrandTigerGameLogic()
        {
            _gameID     = GAMEID.GrandTiger;
            GameName    = "GrandTiger";
        }

        protected override void readBetInfoFromMessage(GITMessage message, string strUserID, Currencies currency)
        {
            try
            {
                GrandTigerBetInfo betInfo   = new GrandTigerBetInfo();
                betInfo.PlayLine            = (int)message.Pop();
                betInfo.PlayBet             = (int)message.Pop();
                betInfo.PurchaseStep        = (int)message.Pop();
                betInfo.MoreBet             = (int)message.Pop();
                betInfo.CurrencyInfo        = currency;
                betInfo.GambleType          = 0;
                betInfo.GambleHalf          = false;

                if (BettingButton[betInfo.PlayBet] * betInfo.RelativeTotalBet <= 0)
                {
                    _logger.Error("{0} betInfo 0 or infinite in GrandTigerGameLogic::readBetInfoFromMessage", strUserID);
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
                _logger.Error("Exception has been occurred in GrandTigerGameLogic::readBetInfoFromMessage {0}", ex);
            }
        }

        protected override BaseAmaticSlotBetInfo restoreBetInfo(string strUserID, BinaryReader reader)
        {
            GrandTigerBetInfo betInfo = new GrandTigerBetInfo();
            betInfo.SerializeFrom(reader);
            return betInfo;
        }
    }
}
