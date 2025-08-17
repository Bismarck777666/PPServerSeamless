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
    class WildAnubisGameLogic : BaseAmaticExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "WildAnubis";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 6, 7, 8 ,9 ,10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 600, 700, 800, 900, 1000, 2000 };
            }
        }

        protected override long[] LINES
        {
            get
            {
                return new long[] { 25 };
            }
        }

        protected override string InitString
        {
            get
            {
                return "0524528857722855177366554660ab6698833664287443378577557722881988112550ab552496611788cde7884438770ab877339665cde66544624338855611497720ab44663377388855248661554cde55442255663335534770ab47739427788124912441133688cde68866332277723c7714cde1426623755775453255243669366cde5534771660ab3883988321254794255cde8847734883417669883677660ab25588245531427715577662416627724972488550ab14327524b274879624cde6246714786230ab58437186953764cde76443725967364836517752cde7524124b8796248716836714cde7140ab8436786953172cde1724372596736483651785cde78524127424b79683cde6836714786230ab58437486953764cde7641372596736483651743cde743241274824b367cde36730ab58437186953764cde7644372596736483651743cde7432412748796245716824b274879683cde68367147862358437186953172cde172437259673640ab83651851cde85124103010101010101042710100021931f419101010101010101919191100101010101000000000000000000811121314151a1f2141a10101010101010101010101010101010101010101010101010101200";
            }
        }

        protected override string ExtraString => "1255";
        #endregion

        public WildAnubisGameLogic()
        {
            _gameID     = GAMEID.WildAnubis;
            GameName    = "WildAnubis";
        }
    }
}
