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
   
    class LuckyJoker5GameLogic : BaseAmaticExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "LuckyJoker5";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 2, 4, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000 };
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
                return "0522b66677740231657444555123777034448366695553812263674583666777427444555032475631a3120732266664442172a621581277751a6755591342301222615a728166655547a6544477731026346126073222209106662351342155530270277783144400301010101010104271010001433e80510101010101010050505110010101010100000000000000000161416181a21421e22823223c24625025a26427828c2a02b42c8312c319031f433e80610101010101001";
            }
        }

        protected override int LineTypeCnt  => 1;

        protected override int FreeCols     => 0;

        protected override string ExtraString => "01";
        #endregion

        public LuckyJoker5GameLogic()
        {
            _gameID     = GAMEID.LuckyJoker5;
            GameName    = "LuckyJoker5";
        }
    }
}
