using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics.AristoGames
{
    internal class DragonEmperorGameLogic : BasePPSlotGame
    {
        protected override string SymbolName
        {
            get
            {
                return "dragonemperor";
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "&AB=998584&B=998584&BD=1|2|3|4|5|10|20|30|40|50|100|200|300|400|500|600|700|800|900|1000|2000|3000|4000|5000|10000&BDD=1&CUR=ISO:USD|,|.|36;|L|32;|R&FRBAL=0&GA=0&LIM=1|12000|&MSGID=INIT&RBM=30|30|30|30|30|&RSTM=1;0|&SID=123123123&UGB=1&VER=2.6.52-2.7.1-2.6.3-2.5.1795-1795-4&";
            }
        }
        public DragonEmperorGameLogic()
        {
            _gameID = GAMEID.DragonEmperor;
            GameName = "DragonEmperor";
        }
    }
}
