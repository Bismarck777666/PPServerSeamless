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
   
    class FruitBoxGameLogic : BaseAmaticExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "FruitBox";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000, 2000 };
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
                return "0523800006644773333558222266668777755558111177444466221187777237777600066833399997776559666877744481118222184454666855524500007776766633333999977775555586667677744484111186668622228144446666823c00007777666633333999977778555587777844111166922221844446666823900005577111144445522668113333866668777722228443385599995552360000664477333355222266668777755551111774444866221177772377776000663339999777695566687774449111222194448666555599245000077776666333339999777755555866667777444411118666692222144446666999238000077776666333339999777755558777744111166222218444466662360000557711114444552266811333366668777722224433555599990301010101010104271010001a5186a03210101010101010323232110010101010100000000000000000191a1f21421921e22322822d23223c24625025a2642c8312c319031f4325832bc3320338433e835dc37d033101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010100101010101";
            }
        }

        protected override string ExtraString => "100101010101";
        #endregion

        public FruitBoxGameLogic()
        {
            _gameID     = GAMEID.FruitBox;
            GameName    = "FruitBox";
        }
    }
}
