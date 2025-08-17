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
    class BeautyFairyGameLogic : BaseAmaticMultiBaseExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "BeautyFairy";
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
                return "062395556665511414485430033325772552244864225477777666666677762325511016667773334424220665551133666644402119997777723155506664440411133375552222333774466660444779997772345655666337110444740433322556677011666444221129997777232555114664444333507766722033666015550221144999777772307855536604142227771122244433355566677766677777776231555666551141448543003332577255224486422547777766622d55110666777333444220665551339666644421999777722d55506664444111333755592223337766664447799977722e565566633711044974433325567716664442212999777722c5551146644449333507766723366655522144999777722c785553660414222777112224443335556667776667770301010101010101042710100011264141010101010101014140111001010101010100000000000000000171112131415161718191a1f21421921e22322822d23223c24625025a26415101010101010101010101010101010101010101010160000001600000016000000";
            }
        }
        protected override int LineTypeCnt => 2;
        protected override int Cols => 6;
        protected override int FreeCols => 6;
        protected override string ExtraString => "160000001600000016000000";
        #endregion

        public BeautyFairyGameLogic()
        {
            _gameID     = GAMEID.BeautyFairy;
            GameName    = "BeautyFairy";
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
