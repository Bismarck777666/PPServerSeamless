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
    class DoubleJoker20GameLogic : BaseAmaticExtra1Game
    {
        #region 게임고유속성값
        protected override string SymbolName
        {
            get
            {
                return "DoubleJoker20";
            }
        }
        protected override long[] BettingButton
        {
            get
            {
                return new long[] { 1, 2, 3, 4, 5, 6, 8, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };
            }
        }
        protected override long[] LINES
        {
            get
            {
                return new long[] { 20 };
            }
        }
        protected override string InitString
        {
            get
            {
                return "0526d117774445553332666955561025666777044483336667770444866677704448666777444555333266695553332666955577744455511126866644455566602226666044455566667a6777444833377731a31555022266677731a3155504448333077731a314447770555833326f725559222555944411172a72911177711166644472a7233386664447a47771118666444022286665553332220111022255572a72555b5552682262a62444777055562a621118666444777111777333444077786665a6555333066655562a62333111866655544407770111022227f855533339666111444033344455511133307775550222011133307778444777844422201110666055562315666977701116662225552224449666222777333300301010101010104271010002142641410101010101010141414110010101010100000000000000000171112131415161718191a1f21421921e22322822d23223c24625025a264151010101010101010101010101010101010101010100101";
            }
        }
        protected override string ExtraString => "0101";
        #endregion

        public DoubleJoker20GameLogic()
        {
            _gameID     = GAMEID.DoubleJoker20;
            GameName    = "DoubleJoker20";
        }
    }
}
