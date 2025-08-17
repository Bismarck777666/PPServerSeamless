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
    class CrystalFruitsGameLogic : BaseAmaticMultiBaseExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "CrystalFruits";
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
                return new long[] { 10, 20 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "062220505050406060601010107040404030200000005040303030706020202050707070622e0501010100000006060607070706030303060404040706020202070505050305060401040203040707020808080522705050500000006060605040404060501010103030302070202020403070604070808080707070622e060606050505030307060204040407010000000603030305020506070601010104040405020202070808080707072270505050406010101040604040403030305070000000703070202020606060602080808070707052140a090a0b0c0a0b0a0d0a0a0a0e0a0e0b0d0a0b0a6237050505040606060501010104040405030000000507030303050702050607020202050406040206050407060704070707020606060707062320501010105000000060606070707060103030306010404040706020202070505050103060406040602030407010708080807231050505000000060606050404040604050101010303030702040507020202060304070307040306060407040808080707072340606060505050303070601020404040107040000000604030303050206050607060701010106040404020202010708080807070723205050504060101010406040404030303050700000007060307020202060306060605000702010104040508080807070705072140d13100e140b160c12100f0b130b0d111511140c0301010101010101042710100011264141010101010101014140111001010101010100000000000000000171112131415161718191a1f21421921e22322822d23223c24625025a2641510101010101010101010101010101010101010101011";
            }
        }
        protected override int LineTypeCnt => 2;
        protected override int Cols => 6;
        protected override int FreeCols => 6;
        protected override int ReelSetBitNum => 2;
        protected override string ExtraString => "11";
        #endregion

        public CrystalFruitsGameLogic()
        {
            _gameID     = GAMEID.CrystalFruits;
            GameName    = "CrystalFruits";
        }

        protected override int getLineTypeFromPlayLine(int playline)
        {
            switch (playline)
            {
                case 10:
                    return 0;
                case 20:
                    return 1;
                default:
                    return 0;
            }
        }
    }
}
