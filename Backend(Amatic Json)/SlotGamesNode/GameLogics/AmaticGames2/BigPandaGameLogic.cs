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
    class BigPandaGameLogic : BaseAmaticMultiBaseExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "BigPanda";
            }
        }
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 30, 40, 50, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };
            }
        }
        protected override long[] LINES
        {
            get
            {
                return new long[] { 10, 20, 30, 40, 50 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "05256000065111168222275333385444476488865555637578134678757682575875876666577776888899996562430873218723684aaaaaaaaaaaaaaaaaa7647958655aaaaaaaaaaaaaaaaaaa655186725204aaaaaaaaaaaaaaaaaaaaaaaaaa4867761732542aaaaaaaaaaaaaaaaaaaaaaaaaaaa651689587835324c04645aaaaaaaaaaaaaaaaaaaaaaaaaaa783795781aaaaaaaaaaaaaaaaaaaaaaaaaaa5621283623901234aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa5677586592867834522917aaaa2483aaaa506aaaaa8567aaaa578678aaaaa23e448007aaaaa2263aaaaa35711aaaaa775568aaaaa686aaaaaaa567588aaaaa236aaaa665772aaaaaa885781aaaaa85021aaaaaaa6567304aaaaaa3424300611824aaaaa253374aaaa775aaaa8688655aaaaa6776aaaaa588aaaaa657aaaaa2410011223344aaaaaa55577766688865aaaaaaa7857680723451668875aaaaaaaaa0301010101010104271010001a5186a0321010101010101032320a110010101010100000000000000000191a1f21421921e22322822d23223c24625025a2642c8312c319031f4325832bc3320338433e835dc37d03310101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101015000001500000";
            }
        }
        protected override int LineTypeCnt => 5;
        protected override string ExtraString => "15000001500000";
        #endregion

        public BigPandaGameLogic()
        {
            _gameID     = GAMEID.BigPanda;
            GameName    = "BigPanda";
        }

        protected override int getLineTypeFromPlayLine(BaseAmaticSlotBetInfo betInfo)
        {
            switch (betInfo.PlayLine)
            {
                case 10:
                    return 0;
                case 20:
                    return 1;
                case 30:
                    return 2;
                case 40:
                    return 3;
                case 50:
                    return 4;
                default:
                    return -1;
            }
        }
        protected override async Task onProcMessage(string strUserID, int companyID, GITMessage message, UserBonus userBonus, double userBalance, Currencies currency)
        {
            if (message.MsgCode == (ushort)CSMSG_CODE.CS_AMATIC_FSOPTION)
                await onDoSpin(strUserID, companyID, message, userBonus, userBalance,currency);
            else
                await base.onProcMessage(strUserID, companyID, message, userBonus, userBalance, currency);
        }
    }
}
