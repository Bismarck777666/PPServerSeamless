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
   
    class Hot7DiceGameLogic : BaseAmaticSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "Hot7Dice";
            }
        }
        
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 4, 6, 8, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 1000, 2000 };
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
                return "0521933320227556665551141444302193333444242250555110166667219333226255505666744404113121933311044440433722555656662193335551147444743350666220003010101010101042710100a1131f4051010101010101a0505051100101010101000000000000000001411121416181a21421e22823223c24625025a26427828c2a02b42c806101010101010";
            }
        }
        #endregion

        public Hot7DiceGameLogic()
        {
            _gameID     = GAMEID.Hot7Dice;
            GameName    = "Hot7Dice";
        }
    }
}
