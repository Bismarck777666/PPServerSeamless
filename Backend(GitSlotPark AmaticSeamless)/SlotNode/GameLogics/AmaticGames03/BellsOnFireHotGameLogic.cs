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
   
    class BellsOnFireHotGameLogic : BaseAmaticMultiBaseGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "BellsOnFireHot";
            }
        }
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000,2000 };
            }
        }
        protected override long[] LINES
        {
            get
            {
                return new long[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "052255755666551141445430333225552244642254228551101666733344242206655511336664464021122755506667444041113335552262333446660442422a56556663311044440433372255660116664442211222857551146644443335066220336660155522114405215766655144303366655524216066733442265566655514621755566674401555226633446217554466631446033722555662185714664435550664462233550301010101010104271010001a33e8641010101010101064640a1100101010101000000000000000000a1112131415161718191a651010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010101010";
            }
        }
        protected override int LineTypeCnt => 10;
        #endregion

        public BellsOnFireHotGameLogic()
        {
            _gameID     = GAMEID.BellsOnFireHot;
            GameName    = "BellsOnFireHot";
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
                case 60:
                    return 5;
                case 70:
                    return 6;
                case 80:
                    return 7;
                case 90:
                    return 8;
                case 100:
                    return 9;
                default:
                    return 0;
            }
        }
    }
}
