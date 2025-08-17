using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GITProtocol;

namespace SlotGamesNode.GameLogics
{
    class HundredJokerStaxxGameLogic : BasePlaysonSlotGame
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "100_joker_staxx";
            }
        }
        protected override int[] StakeIncrement
        {
            get
            {
                return new int[] { 1, 2, 3, 4, 5, 8, 10, 20, 30, 40, 60, 90, 100, 200, 300, 400, 500 };
            }
        }
        protected override string InitDataString
        {
            get
            {
                return "<server><source game-ver=\"301018\"/><state current_state=\"idle\"/><slot><combinations><combination symbol=\"1\" name=\"joker\" count=\"3\" coef=\"20\"/><combination symbol=\"1\" name=\"joker\" count=\"4\" coef=\"100\"/><combination symbol=\"1\" name=\"joker\" count=\"5\" coef=\"1000\"/><combination symbol=\"2\" name=\"7red\" count=\"3\" coef=\"10\"/><combination symbol=\"2\" name=\"7red\" count=\"4\" coef=\"40\"/><combination symbol=\"2\" name=\"7red\" count=\"5\" coef=\"250\"/><combination symbol=\"3\" name=\"7gold\" count=\"3\" coef=\"10\"/><combination symbol=\"3\" name=\"7gold\" count=\"4\" coef=\"40\"/><combination symbol=\"3\" name=\"7gold\" count=\"5\" coef=\"250\"/><combination symbol=\"4\" name=\"bar\" count=\"3\" coef=\"10\"/><combination symbol=\"4\" name=\"bar\" count=\"4\" coef=\"30\"/><combination symbol=\"4\" name=\"bar\" count=\"5\" coef=\"150\"/><combination symbol=\"5\" name=\"bell\" count=\"3\" coef=\"10\"/><combination symbol=\"5\" name=\"bell\" count=\"4\" coef=\"30\"/><combination symbol=\"5\" name=\"bell\" count=\"5\" coef=\"150\"/><combination symbol=\"6\" name=\"melon\" count=\"3\" coef=\"2\"/><combination symbol=\"6\" name=\"melon\" count=\"4\" coef=\"10\"/><combination symbol=\"6\" name=\"melon\" count=\"5\" coef=\"100\"/><combination symbol=\"7\" name=\"grape\" count=\"3\" coef=\"2\"/><combination symbol=\"7\" name=\"grape\" count=\"4\" coef=\"10\"/><combination symbol=\"7\" name=\"grape\" count=\"5\" coef=\"100\"/><combination symbol=\"8\" name=\"plum\" count=\"3\" coef=\"1\"/><combination symbol=\"8\" name=\"plum\" count=\"4\" coef=\"5\"/><combination symbol=\"8\" name=\"plum\" count=\"5\" coef=\"50\"/><combination symbol=\"9\" name=\"orange\" count=\"3\" coef=\"1\"/><combination symbol=\"9\" name=\"orange\" count=\"4\" coef=\"5\"/><combination symbol=\"9\" name=\"orange\" count=\"5\" coef=\"50\"/><combination symbol=\"10\" name=\"lemon\" count=\"3\" coef=\"1\"/><combination symbol=\"10\" name=\"lemon\" count=\"4\" coef=\"5\"/><combination symbol=\"10\" name=\"lemon\" count=\"5\" coef=\"50\"/><combination symbol=\"11\" name=\"cherry\" count=\"3\" coef=\"1\"/><combination symbol=\"11\" name=\"cherry\" count=\"4\" coef=\"5\"/><combination symbol=\"11\" name=\"cherry\" count=\"5\" coef=\"50\"/><combination symbol=\"12\" name=\"disp\" count=\"3\" coef=\"4\"/><combination symbol=\"12\" name=\"disp\" count=\"4\" coef=\"20\"/><combination symbol=\"12\" name=\"disp\" count=\"5\" coef=\"200\"/></combinations><paylines><payline id=\"1\" path=\"1,1,1,1,1\"/><payline id=\"2\" path=\"4,4,4,4,4\"/><payline id=\"3\" path=\"2,2,2,2,2\"/><payline id=\"4\" path=\"3,3,3,3,3\"/><payline id=\"5\" path=\"2,1,1,1,1\"/><payline id=\"6\" path=\"1,1,1,1,2\"/><payline id=\"7\" path=\"1,2,1,1,1\"/><payline id=\"8\" path=\"1,1,1,2,1\"/><payline id=\"9\" path=\"2,2,1,1,1\"/><payline id=\"10\" path=\"1,1,1,2,2\"/><payline id=\"11\" path=\"1,1,2,1,1\"/><payline id=\"12\" path=\"4,4,3,4,4\"/><payline id=\"13\" path=\"2,1,2,1,1\"/><payline id=\"14\" path=\"1,1,2,1,2\"/><payline id=\"15\" path=\"1,2,2,1,1\"/><payline id=\"16\" path=\"1,1,2,2,1\"/><payline id=\"17\" path=\"2,2,2,1,1\"/><payline id=\"18\" path=\"1,1,2,2,2\"/><payline id=\"19\" path=\"2,1,1,2,1\"/><payline id=\"20\" path=\"1,2,1,1,2\"/><payline id=\"21\" path=\"1,2,1,2,1\"/><payline id=\"22\" path=\"4,3,4,3,4\"/><payline id=\"23\" path=\"2,2,1,2,1\"/><payline id=\"24\" path=\"1,2,1,2,2\"/><payline id=\"25\" path=\"2,1,2,2,1\"/><payline id=\"26\" path=\"1,2,2,1,2\"/><payline id=\"27\" path=\"1,2,2,2,1\"/><payline id=\"28\" path=\"4,3,3,3,4\"/><payline id=\"29\" path=\"2,2,2,2,1\"/><payline id=\"30\" path=\"1,2,2,2,2\"/><payline id=\"31\" path=\"1,2,3,2,1\"/><payline id=\"32\" path=\"4,3,2,3,4\"/><payline id=\"33\" path=\"2,1,1,1,2\"/><payline id=\"34\" path=\"3,4,4,4,3\"/><payline id=\"35\" path=\"2,2,1,1,2\"/><payline id=\"36\" path=\"2,1,1,2,2\"/><payline id=\"37\" path=\"2,1,2,1,2\"/><payline id=\"38\" path=\"3,4,3,4,3\"/><payline id=\"39\" path=\"2,2,2,1,2\"/><payline id=\"40\" path=\"2,1,2,2,2\"/><payline id=\"41\" path=\"4,3,2,1,2\"/><payline id=\"42\" path=\"1,2,3,4,3\"/><payline id=\"43\" path=\"2,2,1,2,2\"/><payline id=\"44\" path=\"3,3,4,3,3\"/><payline id=\"45\" path=\"3,2,2,2,2\"/><payline id=\"46\" path=\"2,2,2,2,3\"/><payline id=\"47\" path=\"2,3,2,2,2\"/><payline id=\"48\" path=\"2,2,2,3,2\"/><payline id=\"49\" path=\"3,3,2,2,2\"/><payline id=\"50\" path=\"2,2,2,3,3\"/><payline id=\"51\" path=\"2,2,3,2,2\"/><payline id=\"52\" path=\"3,3,2,3,3\"/><payline id=\"53\" path=\"3,2,3,2,2\"/><payline id=\"54\" path=\"2,2,3,2,3\"/><payline id=\"55\" path=\"2,3,3,2,2\"/><payline id=\"56\" path=\"2,2,3,3,2\"/><payline id=\"57\" path=\"3,3,3,2,2\"/><payline id=\"58\" path=\"2,2,3,3,3\"/><payline id=\"59\" path=\"3,2,2,3,2\"/><payline id=\"60\" path=\"2,3,2,2,3\"/><payline id=\"61\" path=\"2,3,2,3,2\"/><payline id=\"62\" path=\"3,2,3,2,3\"/><payline id=\"63\" path=\"3,3,2,3,2\"/><payline id=\"64\" path=\"2,3,2,3,3\"/><payline id=\"65\" path=\"3,2,3,3,2\"/><payline id=\"66\" path=\"2,3,3,2,3\"/><payline id=\"67\" path=\"2,3,3,3,2\"/><payline id=\"68\" path=\"3,2,2,2,3\"/><payline id=\"69\" path=\"3,3,3,3,2\"/><payline id=\"70\" path=\"2,3,3,3,3\"/><payline id=\"71\" path=\"2,3,4,3,2\"/><payline id=\"72\" path=\"3,2,1,2,3\"/><payline id=\"73\" path=\"3,3,2,2,3\"/><payline id=\"74\" path=\"3,2,2,3,3\"/><payline id=\"75\" path=\"3,3,3,2,3\"/><payline id=\"76\" path=\"3,2,3,3,3\"/><payline id=\"77\" path=\"4,3,3,3,3\"/><payline id=\"78\" path=\"3,3,3,3,4\"/><payline id=\"79\" path=\"3,4,3,3,3\"/><payline id=\"80\" path=\"3,3,3,4,3\"/><payline id=\"81\" path=\"4,4,3,3,3\"/><payline id=\"82\" path=\"3,3,3,4,4\"/><payline id=\"83\" path=\"4,3,4,3,3\"/><payline id=\"84\" path=\"3,3,4,3,4\"/><payline id=\"85\" path=\"3,4,4,3,3\"/><payline id=\"86\" path=\"3,3,4,4,3\"/><payline id=\"87\" path=\"4,4,4,3,3\"/><payline id=\"88\" path=\"3,3,4,4,4\"/><payline id=\"89\" path=\"4,3,3,4,3\"/><payline id=\"90\" path=\"3,4,3,3,4\"/><payline id=\"91\" path=\"4,4,3,4,3\"/><payline id=\"92\" path=\"3,4,3,4,4\"/><payline id=\"93\" path=\"4,3,4,4,3\"/><payline id=\"94\" path=\"3,4,4,3,4\"/><payline id=\"95\" path=\"4,4,4,4,3\"/><payline id=\"96\" path=\"3,4,4,4,4\"/><payline id=\"97\" path=\"4,4,3,3,4\"/><payline id=\"98\" path=\"4,3,3,4,4\"/><payline id=\"99\" path=\"4,4,4,3,4\"/><payline id=\"100\" path=\"4,3,4,4,4\"/></paylines><symbols><symbol id=\"1\" title=\"joker\"/><symbol id=\"2\" title=\"7red\"/><symbol id=\"3\" title=\"7gold\"/><symbol id=\"4\" title=\"bar\"/><symbol id=\"5\" title=\"bell\"/><symbol id=\"6\" title=\"melon\"/><symbol id=\"7\" title=\"grape\"/><symbol id=\"8\" title=\"plum\"/><symbol id=\"9\" title=\"orange\"/><symbol id=\"10\" title=\"lemon\"/><symbol id=\"11\" title=\"cherry\"/><symbol id=\"12\" title=\"disp\"/></symbols><reels id=\"1\"><reel id=\"1\" length=\"12\" symbols=\"1,8,4,11,2,10,3,12,7,5,9,6\"/><reel id=\"2\" length=\"12\" symbols=\"1,5,8,3,10,9,11,2,12,7,6,4\"/><reel id=\"3\" length=\"12\" symbols=\"1,4,7,5,6,8,10,11,2,9,12,3\"/><reel id=\"4\" length=\"12\" symbols=\"1,3,10,4,8,2,11,7,9,6,12,5\"/><reel id=\"5\" length=\"12\" symbols=\"1,6,12,8,3,4,11,7,9,2,10,5\"/></reels></slot><shift server=\"0,0,0,0,0\" reel_set=\"1\" reel1=\"10,10,10,11,11,4\" reel2=\"11,4,8,8,8,5\" reel3=\"9,3,3,8,8,8\" reel4=\"9,9,2,2,12,11\" reel5=\"1,6,6,12,8,6\"/><delivery id=\"1901152-288210043255192847461689\" action=\"create\"/></server>";
            }
        }
        #endregion

        public HundredJokerStaxxGameLogic()
        {
            _gameID     = GAMEID.HundredJokerStaxx;
            GameName    = "HundredJokerStaxx";
        }
    }
}
