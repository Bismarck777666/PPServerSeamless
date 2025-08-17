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
    class SunGoddessGameLogic : BaseAmaticExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "SunGoddess";
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
                return new long[] { 50 };
            }
        }

        protected override string InitString
        {
            get
            {
                return "052380abc66447722225583333666687777555581111774444663311877772377776def6682229999777655966687774448111833318445466685552450abc7776766622222999977775555586667677744484111186668633338144446666823c0abc777766662222299997777855558777784411116693333184444666682390abc5577111144445533668112222866668777733338442285599995552360abc664477222255333366668777755551111774444866331177772377776def6622299997776955666877744491113331944486665555992450abc777766662222299997777555558666677774444111186666933331444466669992380abc77776666222229999777755558777744111166333318444466662360abc557711114444553366811222266668777733334422555599990301010101010104271010001a5186a03210101010101010323232110010101010100000000000000000191a1f21421921e22322822d23223c24625025a2642c8312c319031f4325832bc3320338433e835dc37d033101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010100000000000";
            }
        }
        protected override string ExtraString => "100000000000";
        #endregion

        public SunGoddessGameLogic()
        {
            _gameID     = GAMEID.SunGoddess;
            GameName    = "SunGoddess";
        }
    }
}
