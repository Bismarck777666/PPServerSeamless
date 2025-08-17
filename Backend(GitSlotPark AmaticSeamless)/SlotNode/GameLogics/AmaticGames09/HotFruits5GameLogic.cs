using GITProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlotGamesNode.GameLogics
{
    class HotFruits5GameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "HotFruits5";
            }
        }
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 2, 4, 8, 10, 16, 20, 24, 30, 32, 40, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000, 2000 };
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
                return "0522e205561667744220555556166666273377333332244444221a4144425557556066617373331721b616666770333333774444225555101000301010101010104271010001531f405101010101010100505051100101010101000000000000000001411121416181a21421e22823223c24625025a26427828c2a02b42c806101010101010";
            }
        }
        #endregion

        public HotFruits5GameLogic()
        {
            _gameID     = GAMEID.HotFruits5;
            GameName    = "HotFruits5";
        }
    }
}
