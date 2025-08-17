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
    class SunGoddessIIGameLogic : BaseAmaticMultiBaseExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SunGoddessII";
            }
        }
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 250, 300, 350, 400, 450, 500, 600, 700, 800, 900, 1000 };
            }
        }
        protected override long[] LINES
        {
            get
            {
                return new long[] { 10, 20, 30, 40, 50 };
            }
        }
        protected override int LineTypeCnt => 5;
        protected override string InitString
        {
            get
            {
                return "052380abc664477222255833336666833115555811117744446677778777723777760abc668222999966665577787774448111833318445466685552450abc7776755522222999977775566686667677744484111186668633338144448666623c0abc777766662222299997777855558777784411116663333184444866662390abc5577111144445533668112222866668777733338442285599995552360abc6644772222553333666687777555511117744448663311777723777760abc662229999777655666877744411133319999444866655552450abc777766662222299997777555558666677774444111186666999933331444466662380abc77776666222229999777755558777744111166333318444466662360abc557711114444553366811222266668777733334422555599990301010101010104271010001a5186a0321010101010101032320a110010101010100000000000000000191a1f21421921e22322822d23223c24625025a2642c8312c319031f4325832bc3320338433e835dc37d03310101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010100000000000";
            }
        }
        protected override string ExtraString => "10100000000000";
        #endregion

        public SunGoddessIIGameLogic()
        {
            _gameID     = GAMEID.SunGoddessII;
            GameName    = "SunGoddessII";
        }

        protected override int getLineTypeFromPlayLine(int playline)
        {
            switch (playline)
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
                    return 0;
            }
        }
    }
}
